using UnityEngine;

namespace ReverseSolver.Presentation
{
    public sealed class BombMotion : MonoBehaviour
    {
        Transform _body;
        SpriteRenderer _shell, _fuse;
        float _width, _start;
        bool _urgent, _exploding;

        public void Init(Transform body, float width, int order)
        {
            _body = body; _width = width; _start = Time.unscaledTime;
            _shell = Art.Place(body, "bomb-v2", Vector2.one * width, order);
            _fuse = Art.Place(body, "bomb-fuse-08", Vector2.one * width, order + 1);
        }

        public void SetFuse(int value)
        {
            bool urgent = value <= 3;
            if (_urgent != urgent)
            {
                Tweens.Kill(this);
                _body.localScale = Vector3.one;
                _body.localPosition = Vector3.zero;
            }
            _urgent = urgent;
            if (_shell != null) _shell.sprite = Art.Get(urgent ? "bomb-v2-urgent" : "bomb-v2");
            if (urgent && _fuse != null) _fuse.sprite = Art.Get("bomb-fuse-urgent");
        }

        void Update()
        {
            if (_exploding || _urgent) return;
            float seconds = Time.unscaledTime - _start;
            _body.localScale = Vector3.one * (1 + .03f * Mathf.Sin(seconds * 2 * Mathf.PI / 1.2f));
            if (_fuse != null) _fuse.sprite = Art.Get("bomb-fuse-" + (8 - Mathf.FloorToInt(seconds * 11) % 8).ToString("00"));
        }

        public void Tick()
        {
            if (!_urgent || _exploding) return;
            Tweens.Run(this, .28f, t =>
            {
                if (this == null) return;
                _body.localScale = Vector3.one * (1 + .08f * Mathf.Sin(t * Mathf.PI));
                float seconds = t * .28f;
                _body.localPosition = Draw.W(_width * .03f * Mathf.Max(0, 1 - seconds / .2f) * Mathf.Sin(seconds * 100), 0);
            }, () => { if (this != null) { _body.localScale = Vector3.one; _body.localPosition = Vector3.zero; } });
        }

        public void Stop() { _exploding = true; Tweens.Kill(this); _body.localPosition = Vector3.zero; }
        void OnDestroy() { Tweens.Kill(this); }
    }
}
