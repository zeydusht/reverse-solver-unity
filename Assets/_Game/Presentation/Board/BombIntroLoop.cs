using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* The bomb card's demo badge. PM_73: it shows the level's real fuse (the same number as the card's text, PM_72
       found "2" next to "16 moves") and ticks in place every 1.6 s instead of counting 3-2-1. */
    public sealed class BombIntroLoop : MonoBehaviour
    {
        public const float TickSeconds = 1.6f;
        BombBadge _badge;
        float _start;
        int _ticks;
        public int Fuse { get; private set; }
        public void Init(Vector2 center, int order, int fuse)
        {
            Fuse = Mathf.Max(1, fuse);
            _badge = new BombBadge(transform, center, 88, order);
            _badge.Fuse = Fuse; _start = Time.unscaledTime;
        }
        void Update()
        {
            int ticks = (int)((Time.unscaledTime - _start) / TickSeconds);
            if (ticks == _ticks) return;
            _ticks = ticks; _badge.Tick();
        }
    }
}
