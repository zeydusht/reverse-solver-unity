using UnityEngine;

namespace ReverseSolver.Presentation
{
    sealed class WinCelebration54
    {
        const float Duration = 2;
        readonly Transform _root;
        readonly SpriteRenderer _jigo, _rays;
        readonly Sprite[] _frames = new Sprite[8];
        readonly SpriteRenderer[] _confetti = new SpriteRenderer[36];
        readonly Vector2[] _from = new Vector2[36], _travel = new Vector2[36];
        readonly float[] _spin = new float[36];
        readonly float _start;
        readonly ShapeView _shade;
        bool _skipped;
        int _frame = -1;

        public WinCelebration54(Transform parent, Vector2 screen, int order)
        {
            _root = new GameObject("Win celebration").transform;
            _root.SetParent(parent, false);
            _start = Time.unscaledTime;
            _shade = ShapeView.Create(_root, "shade", screen, 0, order).Fill(Draw.Rgba(10, 18, 46, .65f));
            _rays = Art.Place(_root, "jigo/sevinc/fx/light-rays", Vector2.one * 360, order + 1);
            _jigo = Art.Place(_root, "jigo/sevinc/01", Vector2.one * 320, order + 2);
            for (int i = 0; i < _frames.Length; i++) _frames[i] = Art.Get("jigo/sevinc/" + (i + 1).ToString("00"));
            string[] colours = { "coral", "gold", "purple", "teal" };
            for (int i = 0; i < _confetti.Length; i++)
            {
                _confetti[i] = Art.Place(_root, "jigo/sevinc/fx/confetti-" + colours[i % 4], Vector2.one * Random.Range(9f, 14f), order + 3);
                _from[i] = new Vector2(Random.Range(-.5f, .5f), Random.Range(-.65f, -.25f));
                _travel[i] = new Vector2(Random.Range(-.2f, .2f), Random.Range(.8f, 1.2f));
                _spin[i] = Random.Range(-720f, 720f);
            }
            Layout(screen);
            Tick(screen);
        }

        public void Layout(Vector2 screen)
        {
            _root.localPosition = Draw.W(screen * .5f);
            _shade.Resize(screen, 0);
            float size = Mathf.Min(screen.x * .86f, screen.y * .6f);
            Fit(_jigo, size);
            Fit(_rays, size * 1.25f);
        }

        static void Fit(SpriteRenderer r, float size)
        {
            if (r != null) r.transform.localScale = Vector3.one * (size / Mathf.Max(r.sprite.bounds.size.x, r.sprite.bounds.size.y));
        }

        public bool Tick(Vector2 screen)
        {
            float t = Mathf.Clamp01((Time.unscaledTime - _start) / Duration);
            int frame = Mathf.FloorToInt((Time.unscaledTime - _start) * 10) % 8;
            if (_jigo != null && frame != _frame) _jigo.sprite = _frames[frame];
            _frame = frame;
            if (_rays != null) _rays.transform.localRotation = Quaternion.Euler(0, 0, t * 30);
            // Same falling, spinning, fading style as Screens.Confetti.
            for (int i = 0; i < _confetti.Length; i++)
            {
                var r = _confetti[i];
                if (r == null) continue;
                r.transform.localPosition = Draw.W(Vector2.Scale(_from[i] + _travel[i] * t, screen));
                r.transform.localRotation = Quaternion.Euler(0, 0, _spin[i] * t);
                r.color = new Color(1, 1, 1, 1 - t * t);
            }
            return _skipped || t >= 1;
        }

        public void Skip() { _skipped = true; }
        public void Destroy() { if (_root != null) Object.Destroy(_root.gameObject); }
    }
}
