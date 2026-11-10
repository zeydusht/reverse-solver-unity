using UnityEngine;

namespace ReverseSolver.Presentation
{
    public static class ChainTuning
    {
        public const double FixedStep = 1.0 / 120;
        public const float Step = (float)FixedStep, Damping = .99f, Gravity = 18f; // .99 per 120 Hz substep = .98 per 60 Hz frame; 18 cell/s² swings ~1 Hz
        public const int Iterations = 20, MaxSteps = 8, MinNodes = 8, MaxNodes = 16;
        public const float NodesPerCell = 3, PreSettle = 1, BoundsInset = .13f;
        public const float Sag = .30f, DiagonalSag = .35f, MinimumSag = .28f;
        public const float Kick = 2.4f, Tremble = .006f, TrembleHz = 27f;
        public const float Hang = .4f, BreakLife = 1.2f, BreakGravity = 12f, AnchorScale = 1.6f, LinkScale = 1.15f;

        public static float RestSag(Vector2 a, Vector2 b, float cell) =>
            cell * Mathf.Max(MinimumSag, Mathf.Abs(b.x - a.x) > .01f * cell &&
                Mathf.Abs(b.y - a.y) > .01f * cell ? DiagonalSag : Sag);
    }

    // World-space Verlet particles. Construction is the only allocation; no engine objects.
    public sealed class RopeSim
    {
        readonly Vector2[] _positions, _previous;
        readonly float _cell;
        float _segment;
        readonly Vector2 _min, _max;
        double _accumulator;
        float _time;
        int _cut = -1;
        bool _released;
        public int Count => _positions.Length;
        public float Length { get; private set; }
        public Vector2 this[int i] => _positions[i];

        public RopeSim(Vector2 a, Vector2 b, float cell, Vector2 min, Vector2 max)
        {
            _cell = cell; _min = min; _max = max;
            float distance = Vector2.Distance(a, b), sag = ChainTuning.RestSag(a, b, cell);
            Length = distance + 8 * sag * sag / (3 * Mathf.Max(distance, .01f * cell));
            int count = Mathf.Clamp(Mathf.CeilToInt(Length / cell * ChainTuning.NodesPerCell) + 1,
                ChainTuning.MinNodes, ChainTuning.MaxNodes);
            _positions = new Vector2[count]; _previous = new Vector2[count];
            _segment = Length / (count - 1);
            Vector2 side = new Vector2(b.y - a.y, a.x - b.x).normalized;
            if (side.y > 0) side = -side;
            for (int i = 0; i < count; i++)
            {
                float t = (float)i / (count - 1);
                _positions[i] = _previous[i] = Vector2.Lerp(a, b, t) + side * (4 * sag * t * (1 - t));
            }
            int settle = Mathf.CeilToInt(ChainTuning.PreSettle / ChainTuning.Step);
            for (int i = 0; i < settle; i++) Substep(a, b);
            // Verlet stretches slightly under gravity; trim the length until the settled sag hits the target.
            if (Mathf.Abs(b.x - a.x) > .01f * cell)
                for (int round = 0; round < 6; round++)
                {
                    float measured = Mathf.Abs(Vector2.Dot(_positions[count / 2] - Vector2.Lerp(a, b, .5f), side));
                    float d = Mathf.Max(distance, .01f * cell);
                    Length = Mathf.Max(distance, Length - 8 * (measured * measured - sag * sag) / (3 * d));
                    _segment = Length / (count - 1);
                    for (int i = 0; i < settle; i++) Substep(a, b);
                }
            // A settled level starts without inherited velocity or a fractional time step.
            for (int i = 0; i < count; i++) _previous[i] = _positions[i];
            _time = 0;
        }

        public void Advance(float delta, Vector2 a, Vector2 b)
        {
            _accumulator += System.Math.Min(System.Math.Max(0, delta), ChainTuning.FixedStep * ChainTuning.MaxSteps);
            int steps = 0;
            while (_accumulator + 1e-8 >= ChainTuning.FixedStep && steps++ < ChainTuning.MaxSteps)
            {
                Substep(a, b);
                _accumulator -= ChainTuning.FixedStep;
            }
        }

        public void Kick(Vector2 velocity)
        {
            for (int i = 1; i < Count - 1; i++)
                _previous[i] -= velocity * (ChainTuning.Step * Mathf.Sin(Mathf.PI * i / (Count - 1)));
        }

        public void Split() { _cut = (Count - 1) / 2; }
        public void Release() { _released = true; }

        bool Pinned(int i) => !_released && (i == 0 || i == Count - 1);
        Vector2 Clamp(Vector2 p) => new Vector2(Mathf.Clamp(p.x, _min.x, _max.x), Mathf.Clamp(p.y, _min.y, _max.y));

        static Vector2 Tether(Vector2 p, Vector2 pin, float max)
        {
            var d = p - pin; float m = d.magnitude;
            return m > max ? pin + d * (max / m) : p;
        }

        void Substep(Vector2 a, Vector2 b)
        {
            _time += ChainTuning.Step;
            if (_cut < 0) { a = Clamp(a); b = Clamp(b); }
            bool taut = _cut < 0 && Vector2.Distance(a, b) >= Length;
            Vector2 normal = new Vector2(b.y - a.y, a.x - b.x).normalized;
            for (int i = 0; i < Count; i++)
            {
                if (Pinned(i)) { _positions[i] = _previous[i] = i == 0 ? a : b; continue; }
                var p = _positions[i];
                _positions[i] += (p - _previous[i]) * ChainTuning.Damping + Vector2.down *
                    (_cell * (_cut < 0 ? ChainTuning.Gravity : ChainTuning.BreakGravity) * ChainTuning.Step * ChainTuning.Step);
                _previous[i] = p;
            }
            if (taut)
            {
                // The inextensible limit has an analytic solution; jitter is transverse and tiny.
                for (int i = 1; i < Count - 1; i++)
                {
                    float t = (float)i / (Count - 1);
                    _positions[i] = Clamp(Vector2.Lerp(a, b, t) + normal * (_cell * ChainTuning.Tremble *
                        Mathf.Sin(_time * 2 * Mathf.PI * ChainTuning.TrembleHz + i) * Mathf.Sin(t * Mathf.PI)));
                    _previous[i] = _positions[i];
                }
                return;
            }
            for (int iteration = 0; iteration < ChainTuning.Iterations; iteration++)
            {
                // Alternate sweep direction to avoid favouring one pinned end.
                for (int k = 0; k < Count - 1; k++)
                {
                    int i = iteration % 2 == 0 ? k : Count - 2 - k;
                    if (i == _cut) continue;
                    var d = _positions[i + 1] - _positions[i];
                    float length = d.magnitude;
                    if (length < .000001f) continue;
                    var correction = d * ((length - _segment) / length);
                    bool left = Pinned(i), right = Pinned(i + 1);
                    if (!left) _positions[i] += correction * (right ? 1 : .5f);
                    if (!right) _positions[i + 1] -= correction * (left ? 1 : .5f);
                }
                if (_cut < 0)
                    for (int i = 1; i < Count - 1; i++) _positions[i] = Clamp(_positions[i]);
            }
            // Tethers: no node may be farther from a pin than the rope between them. Stops long ropes creeping longer.
            if (_cut < 0 && !_released)
                for (int i = 1; i < Count - 1; i++)
                {
                    _positions[i] = Tether(_positions[i], a, i * _segment);
                    _positions[i] = Tether(_positions[i], b, (Count - 1 - i) * _segment);
                }
        }
    }
}


