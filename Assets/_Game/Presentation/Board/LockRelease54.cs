using UnityEngine;

namespace ReverseSolver.Presentation
{
    public sealed class LockRelease54 : MonoBehaviour
    {
        const float Opening = 4f / 14, CrumbLife = .4f, MidFall = .85f;
        const int FallOrder = 2600; // Above lifted board art, below HudOrder (3000) and the bottom strip.
        readonly Sprite[] _frames = new Sprite[4];
        readonly SpriteRenderer[] _crumbs = new SpriteRenderer[7];
        readonly Vector2[] _velocities = new Vector2[7];
        readonly float[] _spins = new float[7];
        SpriteRenderer _lock;
        Vector3 _origin;
        float _start, _cell, _angle, _bottom, _gravity, _margin;
        Vector3 _crumbOrigin;
        int _frame = -1;
        bool _falling;

        public void Init(SpriteRenderer sprite, float cell)
        {
            _lock = sprite; _cell = cell; _start = Time.unscaledTime;
            _origin = transform.position;
            _crumbOrigin = transform.localPosition;
            var camera = Camera.main;
            _bottom = camera != null ? camera.ViewportToWorldPoint(new Vector3(.5f, 0,
                Mathf.Abs(camera.transform.position.z - transform.position.z))).y : _origin.y - cell * 12;
            _margin = sprite != null ? sprite.bounds.size.magnitude : cell;
            var board = GetComponentInParent<BoardView>();
            float middle = board != null ? board.transform.TransformPoint(Draw.W(board.Size * .5f)).y : _origin.y;
            _gravity = 2 * Mathf.Max(cell, middle - _bottom + _margin) / (MidFall * MidFall);
            _angle = Random.Range(-25f, 25f);
            for (int i = 0; i < 4; i++) _frames[i] = Art.Get((Art.T54 ? "obstacles_v3/padlock-b/open-" : "padlock-open-") + (i + 1).ToString("00"));
            for (int i = 0; i < _crumbs.Length; i++)
            {
                if (!Art.T54) break;
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
                // Keep ownership under the screen root, independent of piece/board motion.
                var board = GetComponentInParent<BoardView>();
                transform.SetParent(board != null ? board.transform.parent : null, true);
                _origin = transform.position;
                if (_lock != null) _lock.sortingOrder = FallOrder;
            }
            transform.position = _origin + Vector3.down * (.5f * _gravity * t * t);
            transform.rotation = Quaternion.Euler(0, 0, _angle * Mathf.Clamp01(t / MidFall));
            for (int i = 0; i < _crumbs.Length; i++)
            {
                var r = _crumbs[i];
                if (r == null) continue;
                if (t >= CrumbLife) { Destroy(r.gameObject); continue; }
                r.transform.localPosition = _crumbOrigin + Draw.W(_velocities[i] * t + new Vector2(0, _cell * 6 * t * t));
                r.transform.localRotation = Quaternion.Euler(0, 0, _spins[i] * t);
                r.color = new Color(1, 1, 1, 1 - t / CrumbLife);
            }
            if (transform.position.y + _margin < _bottom) Destroy(gameObject);
        }

        void OnDestroy()
        {
            for (int i = 0; i < _crumbs.Length; i++)
                if (_crumbs[i] != null) Destroy(_crumbs[i].gameObject);
        }
    }
}
