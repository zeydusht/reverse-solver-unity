using UnityEngine;

namespace ReverseSolver.Presentation
{
    public sealed class LockWarningMotion : MonoBehaviour
    {
        SpriteRenderer _lock, _ring;
        Sprite _closed;
        Vector3 _scale;
        Vector2 _size;
        readonly Sprite[] _frames = new Sprite[3];
        float _until;

        public void Play(SpriteRenderer padlock, Vector3 counter, float cell)
        {
            if (_lock == null)
            {
                _lock = padlock; _closed = padlock.sprite; _scale = padlock.transform.localScale;
                _size = Vector2.Scale(_closed.bounds.size, _scale);
                for (int i = 0; i < 3; i++) _frames[i] = Resources.Load<Sprite>("Art/yeni/lockwarning/shake/" + (i + 1).ToString("00"));
                _ring = Draw.Child<SpriteRenderer>(transform, "warning ring");
                _ring.sprite = Resources.Load<Sprite>("Art/yeni/lockwarning/count-ring");
                _ring.sortingOrder = padlock.sortingOrder + 1;
                _ring.transform.localPosition = counter;
                if (_ring.sprite != null) _ring.transform.localScale = new Vector3(cell * .58f / _ring.sprite.bounds.size.x, cell * .58f / _ring.sprite.bounds.size.y, 1);
            }
            _until = Time.unscaledTime + .6f;
            enabled = true;
            Update();
        }

        void Update()
        {
            if (_lock == null) { enabled = false; return; }
            float left = _until - Time.unscaledTime;
            if (left <= 0) { enabled = false; return; }
            var frame = _frames[(int)((.6f - left) * 15) % 3];
            if (frame != null)
            {
                _lock.sprite = frame;
                _lock.transform.localScale = new Vector3(_size.x / frame.bounds.size.x, _size.y / frame.bounds.size.y, 1);
            }
            _ring.color = new Color(1, 1, 1, Mathf.Sin((1 - left / .6f) * Mathf.PI));
        }

        void OnDisable()
        {
            if (_lock != null) { _lock.sprite = _closed; _lock.transform.localScale = _scale; }
            if (_ring != null) _ring.color = new Color(1, 1, 1, 0);
        }
    }
}
