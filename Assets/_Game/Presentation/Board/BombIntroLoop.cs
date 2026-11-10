using UnityEngine;

namespace ReverseSolver.Presentation
{
    public sealed class BombIntroLoop : MonoBehaviour
    {
        BombBadge _badge;
        float _start;
        int _last;
        public void Init(Vector2 center, int order)
        {
            _badge = new BombBadge(transform, center, 88, order);
            _badge.Fuse = _last = 3; _start = Time.unscaledTime;
        }
        void Update()
        {
            int number = 3 - (int)((Time.unscaledTime - _start) / .8f) % 3;
            if (number == _last) return;
            _last = number; _badge.Fuse = number; _badge.Tick();
        }
    }
}
