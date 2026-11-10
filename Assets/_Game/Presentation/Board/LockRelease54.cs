using UnityEngine;

namespace ReverseSolver.Presentation
{
    public sealed class LockRelease54 : MonoBehaviour
    {
        const float Opening = 4f / 14, Fall = .45f, CrumbLife = .4f;
        readonly Sprite[] _frames = new Sprite[4];
        readonly SpriteRenderer[] _crumbs = new SpriteRenderer[7];
        readonly Vector2[] _velocities = new Vector2[7];
        readonly float[] _spins = new float[7];
        SpriteRenderer _lock;
        Vector3 _origin;
        float _start, _cell, _angle;
        int _frame = -1;
        bool _falling;

        public void Init(SpriteRenderer sprite, float cell)
        {
            _lock = sprite; _cell = cell; _start = Time.unscaledTime;
            _origin = transform.localPosition;
            _angle = Random.Range(-25f, 25f);
            for (int i = 0; i < 4; i++) _frames[i] = Art.Get("obstacles_v3/padlock-b/open-" + (i + 1).ToString("00"));
            for (int i = 0; i < _crumbs.Length; i++)
            {
                var r = _crumbs[i] = Art.Place(transform, "obstacles_v3/padlock-b/wax-" + (i % 3 + 1).ToString("00"),
                    Vector2.one * cell * Random.Range(.07f, .12f), sprite != null ? sprite.sortingOrder + 1 : 1801);
                if (r != null) r.gameObject.SetActive(false);
                _velocities[i] = new Vector2(Random.Range(-1.6f, 1.6f), Random.Range(-2.4f, -.7f)) * cell;
                _spins[i] = Random.Range(-540f, 540f);
            }
            Advance(0);
        }

        void Update() { Advance(Time.unscaledTime - _start); }

        void Advance(float seconds)
        {
            if (seconds < Opening)
            {
                int frame = Mathf.Min(3, Mathf.FloorToInt(seconds * 14));
                if (frame != _frame && _lock != null) _lock.sprite = _frames[frame];
                _frame = frame;
                return;
            }
            float t = seconds - Opening;
            if (!_falling)
            {
                _falling = true;
                if (_lock != null) _lock.sprite = _frames[3];
                // Crumbs are siblings of the falling lock, owned by the board.
                for (int i = 0; i < _crumbs.Length; i++)
                    if (_crumbs[i] != null)
                    {
                        _crumbs[i].transform.SetParent(transform.parent, true);
                        _crumbs[i].gameObject.SetActive(true);
                    }
            }
            float progress = Mathf.Clamp01(t / Fall);
            transform.localPosition = _origin + Draw.W(0, _cell * 6 * t * t);
            transform.localRotation = Quaternion.Euler(0, 0, _angle * progress);
            if (_lock != null) _lock.color = new Color(1, 1, 1, 1 - Mathf.InverseLerp(.7f, 1, progress));
            for (int i = 0; i < _crumbs.Length; i++)
            {
                var r = _crumbs[i];
                if (r == null) continue;
                if (t >= CrumbLife) { Destroy(r.gameObject); continue; }
                r.transform.localPosition = _origin + Draw.W(_velocities[i] * t + new Vector2(0, _cell * 6 * t * t));
                r.transform.localRotation = Quaternion.Euler(0, 0, _spins[i] * t);
                r.color = new Color(1, 1, 1, 1 - t / CrumbLife);
            }
            if (t >= Fall) Destroy(gameObject);
        }

        void OnDestroy()
        {
            for (int i = 0; i < _crumbs.Length; i++)
                if (_crumbs[i] != null) Destroy(_crumbs[i].gameObject);
        }
    }
}
