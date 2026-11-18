using System.Collections.Generic;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_87 §1: the menu's tabs as pages side by side (TabSlide87). A tab pressed slides to it (one slide however far,
       1 -> 3 too); the page dragged sideways follows the finger and settles on the next tab past half way or with a
       flick, else springs back. A vertical drag on a list scrolls it: whichever way the finger first goes 10 pt locks. */
    public sealed partial class GameRoot
    {
        TabSlide87 _tabSlide;
        bool _swipe, _swipeDead;
        int _swipeFrom;
        readonly List<(float t, float x)> _swipeSamples = new List<(float, float)>();
        const float SwipeFlick = 300;   // pt/s toward the new page: a flick settles there from any distance

        bool SwipeAllowed => _mode == Mode.Menu && Art.On && _tabs != null && !_levelsMapOpen && _menuSettings == null &&
                             _paintingDetail == null && _profileCard == null && _card == null && !_diving;

        /* Cut short a slide still settling (another press, or leaving the menu). */
        void EndTabSlide87()
        {
            if (_tabSlide == null) return;
            var s = _tabSlide; _tabSlide = null;
            s.End();
        }

        /* The old page's picture, the new page built in place of it, held at progress 0. */
        TabSlide87 StartTabSlide87(int tab)
        {
            EndTabSlide87();
            int from = (int)_tab;
            RememberMenuScroll();
            Material mat = null;
            foreach (var r in _world.GetComponentsInChildren<SpriteRenderer>()) if (r.sharedMaterial != null) { mat = r.sharedMaterial; break; }
            if (_tabs != null) _tabs.Root.gameObject.SetActive(false);   // the bar stays put: not in the picture
            var slide = mat != null ? TabSlide87.Begin(_cam, _screen, BarOrder + 99, tab > from ? 1 : -1, transform, mat) : null;
            var clock = System.Diagnostics.Stopwatch.StartNew();
            _tabs?.Press(tab, () => { _tab = (MenuTab)tab; _levelsMapOpen = false; Layout(); });
            if (slide != null) slide.BuildMs = (float)clock.Elapsed.TotalMilliseconds;
            _tabAnimating = false;
            if (slide == null) return null;
            slide.Attach(_world, _tabs?.Root);
            _tabSlide = slide;
            return slide;
        }

        /* A tab pressed. */
        void PressTab87(int tab)
        {
            if (tab == (int)_tab && _tabSlide == null) { RememberMenuScroll(); _tabs.Press(tab, () => { _levelsMapOpen = false; Layout(); }); return; }
            var slide = StartTabSlide87(tab);
            slide?.Animate(1, TabSlide87.Seconds, () => { if (_tabSlide == slide) _tabSlide = null; });
        }

        /* The finger has gone 10 pt sideways from `start`: a page drag toward the neighbour that way, if there is one. */
        void BeginSwipe87(Vector2 start, Vector2 pt, float now)
        {
            int to = (int)_tab + (pt.x < start.x ? 1 : -1);
            if (to < 0 || to > 3) { _swipeDead = true; return; }
            _swipeFrom = (int)_tab;
            var slide = StartTabSlide87(to);
            if (slide == null) { _swipeDead = true; return; }
            _swipe = true;
            _menuPointer = _menuDragging = true;   // the build reset them; the finger is still down
            _swipeSamples.Clear(); _swipeSamples.Add((now, pt.x));
            DragSwipe87(start, pt, now);
        }

        void DragSwipe87(Vector2 start, Vector2 pt, float now)
        {
            if (_tabSlide == null) return;
            float dx = pt.x - start.x;
            float p = -dx * _tabSlide.Dir / Mathf.Max(1, _screen.x);
            _tabSlide.Set(Mathf.Clamp01(p));   // back past the start: the old page stays (nothing beside it to show)
            _swipeSamples.Add((now, pt.x));
            while (_swipeSamples.Count > 2 && now - _swipeSamples[1].t >= MenuScroll.Window) _swipeSamples.RemoveAt(0);
        }

        void ReleaseSwipe87(float now)
        {
            _swipe = false;
            var slide = _tabSlide;
            if (slide == null) return;
            float v = 0;
            if (_swipeSamples.Count >= 2 && now - _swipeSamples[_swipeSamples.Count - 1].t <= MenuScroll.Window)
            {
                var b = _swipeSamples[_swipeSamples.Count - 1]; var a = _swipeSamples[0];
                if (b.t - a.t > 1e-3f) v = -(b.x - a.x) / (b.t - a.t) * slide.Dir;   // toward the new page, pt/s
            }
            bool settle = slide.Progress > .5f || v > SwipeFlick && slide.Progress > .02f;
            if (v < -SwipeFlick) settle = false;
            Debug.Log($"[sekme] swipe {(settle ? "settles" : "springs back")} at {slide.Progress:0.00}, {v:0} pt/s");
            if (settle)
            {
                Haptics.Button();
                // the rest of the way at the finger's pace at most, the same ease-out
                float rest = 1 - slide.Progress;
                float sec = Mathf.Clamp(TabSlide87.Seconds * rest, .18f, TabSlide87.Seconds);
                slide.Animate(1, sec, () => { if (_tabSlide == slide) _tabSlide = null; });
                return;
            }
            int back = _swipeFrom;
            slide.Animate(0, Mathf.Clamp(.45f * Mathf.Max(.2f, slide.Progress) * 2, .2f, .45f), () =>
            {
                if (_tabSlide == slide) _tabSlide = null;
                if (_mode != Mode.Menu) return;
                // the old page live again in its place (its picture was all there was of it)
                RememberMenuScroll();
                _tabs?.Press(back, () => { _tab = (MenuTab)back; Layout(); });
            });
        }
    }
}
