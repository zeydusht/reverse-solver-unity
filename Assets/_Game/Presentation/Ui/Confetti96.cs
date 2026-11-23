using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_96 §5: the win's strongest beat: a confetti burst from behind Jigo (the game's confetti pieces, thrown out in a
       ring, slowed by the air, falling, spinning, fading) and a quick white flash where they come from. ~1.6 s. */
    public sealed class Confetti96
    {
        const int Count = 64;
        const float Life = 1.6f;
        readonly Transform _root;
        readonly SpriteRenderer[] _bits = new SpriteRenderer[Count];
        readonly Vector2[] _velocity = new Vector2[Count];
        readonly float[] _spin = new float[Count], _delay = new float[Count];
        readonly SpriteRenderer _flash;
        readonly Vector2 _from;
        readonly float _start, _scale;

        public Confetti96(Transform parent, Vector2 from, float scale, int order, float now)
        {
            _root = new GameObject("PM96 confetti").transform; _root.SetParent(parent, false);
            _from = from; _scale = scale; _start = now;
            string[] colours = { "coral", "gold", "purple", "teal" };
            for (int i = 0; i < Count; i++)
            {
                var r = Art.Place(_root, "jigo/sevinc/fx/confetti-" + colours[i % 4], Vector2.one * Random.Range(10f, 16f) * scale, order + (i % 3 == 0 ? 20 : 0));
                if (r == null) continue;
                _bits[i] = r;
                float angle = Random.Range(0f, Mathf.PI * 2), speed = Random.Range(260f, 720f) * scale;
                _velocity[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle) * .8f - .35f) * speed;   // a little more upwards
                _spin[i] = Random.Range(-900f, 900f);
                _delay[i] = i < Count / 2 ? 0 : Random.Range(0f, .05f);
                r.transform.localPosition = Draw.W(from);
            }
            _flash = Art.Place(_root, "T50/win/win_bg_glow", new Vector2(240, 240) * scale, order + 1);
            if (_flash != null) _flash.transform.localPosition = Draw.W(from);
            Tick(now);
        }

        /* false: it is over. */
        public bool Tick(float now)
        {
            float age = now - _start;
            if (_flash != null)
            {
                float f = Mathf.Clamp01(age / .28f);
                _flash.color = new Color(1, 1, 1, (1 - f) * .9f);
                _flash.transform.localScale = Vector3.one * Mathf.Lerp(.35f, 1.1f, f) * _scale * 240 / Mathf.Max(1, _flash.sprite.bounds.size.x);
            }
            for (int i = 0; i < Count; i++)
            {
                var r = _bits[i];
                if (r == null) continue;
                float t = Mathf.Max(0, age - _delay[i]);
                // the air slows them (v e^-kt), gravity pulls them down
                const float k = 3.2f, gravity = 520f;
                float drag = (1 - Mathf.Exp(-k * t)) / k;
                var p = _from + _velocity[i] * drag + new Vector2(0, gravity * _scale * t * t * .5f);
                r.transform.localPosition = Draw.W(p);
                r.transform.localRotation = Quaternion.Euler(0, 0, _spin[i] * t);
                r.color = new Color(1, 1, 1, Mathf.Clamp01((Life - t) / .5f));
            }
            return age < Life + .1f;
        }

        public void Destroy() { if (_root != null) Object.Destroy(_root.gameObject); }
    }
}
