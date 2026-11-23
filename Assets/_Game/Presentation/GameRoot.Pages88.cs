using System.Collections.Generic;
using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_88 §3 (Zeyd: "I tap and it opens late. Like the video: tap tap tap, smooth"): the menu's four pages are all
       built and live side by side, in the bar's order (Home, Collection, Jiggles, Trophies), off the screen but
       there; a tab press only moves them. Nothing is built and nothing is drawn into a picture on the press, so the
       pages move on the press's own frame. The move is .35 s, an ease-out quint (~97 % of the way in .18 s); a new
       press during it turns from wherever the pages are to the new tab. The bar is outside the pages (it stays
       put); its choice moves on the same frame, .15 s.
       The page dragged sideways follows the finger and settles on the next tab past half way or with a flick, else
       springs back. A vertical drag on a list scrolls it: whichever way the finger first goes 10 pt locks.
       The pages not shown are built on the frames after the menu's (one a frame, after its entrance), never while
       the pages move; heavy work (MenuArt's decoding) waits while they move. */
    public sealed partial class GameRoot
    {
        public static float PageSeconds = .35f;   // &sekmesure= (s)
        public static bool PageLog;               // &sekmelog=1: every frame of a move
        Transform _pages;
        readonly Transform[] _page = new Transform[4];
        readonly bool[] _pageBuilt = new bool[4];
        float _pagePos, _pageFrom, _pageTo, _pageT, _pageBuildAt;
        bool _pageAnim;
        int _pageAnimFrame = -1, _pageFrames, _pageSlow;
        float _pageMaxDt, _pageInputMs;
        Vector4 _pageSafe;
        float _pageBottom;
        bool _swipe, _swipeDead;
        float _swipeBase;
        readonly List<(float t, float x)> _swipeSamples = new List<(float, float)>();
        const float SwipeFlick = 300;   // pt/s toward the next page: a flick settles there from any distance

        bool PagesOn => _pages != null;
        bool PagesMoving => _pageAnim || _swipe;
        bool SwipeAllowed => _mode == Mode.Menu && Art.On && PagesOn && !_levelsMapOpen && _menuSettings == null &&
                             _paintingDetail == null && _profileCard == null && _card == null && !_diving;

        static float OutQuint(float k) { k = 1 - Mathf.Clamp01(k); return 1 - k * k * k * k * k; }

        /* From Build: the four pages, the one shown now, the others shortly after. */
        void BuildPages88(Vector4 menuSafe, Vector4 homeSafe, float bottom)
        {
            _pageAnim = _swipe = false;
            MenuTabs.Sliding = false;
            _pages = new GameObject("pages").transform;
            _pages.SetParent(_world, false);
            for (int i = 0; i < 4; i++)
            {
                _page[i] = new GameObject("page " + i).transform;
                _page[i].SetParent(_pages, false);
                _page[i].localPosition = Draw.W(i * _screen.x, 0);
                _pageBuilt[i] = false;
            }
            _pageSafe = menuSafe; _homeSafe88 = homeSafe; _pageBottom = bottom;
            _pagePos = (int)_tab;
            PlacePages();
            BuildPage88((int)_tab, true);
            _pageBuildAt = Time.unscaledTime + .6f;   // after the menu's entrance
        }

        Vector4 _homeSafe88;

        void BuildPage88(int i, bool now)
        {
            if (!PagesOn || _pageBuilt[i]) return;
            _pageBuilt[i] = true;
            var clock = System.Diagnostics.Stopwatch.StartNew();
            var root = _page[i];
            float top = _pageSafe.y;
            if (i == 1) _collection = new CollectionScreen(root, _screen, top, _pageBottom, _set.Levels, _progress, _images, HudOrder, _collectionOffset);
            else if (i == 2) _jiggles = new JigglesScreen(root, _screen, top, _pageBottom, SavedJiggles(), HudOrder, _jigglesOffset);
            else if (i == 3) _achievements = new AchievementsScreen(root, _screen, top, _pageBottom, CurrentAchievements(), HudOrder, _achievementsOffset);
            else BuildHome88(root, _homeSafe88);
            if (PageLog || !now) Debug.Log($"[sekme] page {i} built {(now ? "with the menu" : "ahead")}: {clock.Elapsed.TotalMilliseconds:0.0} ms");
        }

        /* The Home page (Screens.Menu) and what hangs on it. */
        void BuildHome88(Transform parent, Vector4 menuSafe)
        {
            _menu = new Screens.Menu(parent, _screen, menuSafe, _p21Flow ? _progress.Next() + 1 : _set.Levels[MenuNext82()].Number, HudOrder, _newArt, _save.Player, SavedJiggles(), TotalStars(), _showSet ? "Demo" : null, _set.Levels[MenuNext82()].Hard, levelsButton: _testMode || !_p21Lessons, coins: _save.Coins,
                veryHard: _set.Levels[MenuNext82()].Role == "veryhard");
            HomeExtras();
        }

        void PlacePages()
        {
            if (_pages != null) _pages.localPosition = Draw.W(-_pagePos * _screen.x, 0);
        }

        /* Each frame (after the pointer): the move, and the pages not built yet one at a time. */
        void TickPages88()
        {
            if (!PagesOn) return;
            if (_pageAnim && Time.frameCount != _pageAnimFrame)
            {
                float dt = Time.unscaledDeltaTime;
                _pageFrames++; if (dt > .02f) _pageSlow++; _pageMaxDt = Mathf.Max(_pageMaxDt, dt);
                _pageT += Mathf.Min(dt, 1 / 30f);
                float k = Mathf.Clamp01(_pageT / Mathf.Max(.05f, PageSeconds));
                _pagePos = Mathf.LerpUnclamped(_pageFrom, _pageTo, OutQuint(k));
                PlacePages();
                if (PageLog) Debug.Log($"[sekme] {Time.realtimeSinceStartup * 1000:0} {dt * 1000:0.0} p {_pagePos:0.0000}");
                if (k >= 1)
                {
                    _pageAnim = false; MenuTabs.Sliding = false;
                    Debug.Log($"[sekme] slide to {_pageTo:0}: input {_pageInputMs:0.0} ms before its frame, moving on that frame; then {_pageFrames} frames, {_pageSlow} over 20 ms, longest {_pageMaxDt * 1000:0.0} ms");
                }
            }
            if (!PagesMoving && Time.unscaledTime >= _pageBuildAt && _mode == Mode.Menu)
                for (int d = 1; d <= 3; d++)
                {
                    // the nearest first: a swipe's neighbours before the far page
                    int a = (int)_tab + d, b = (int)_tab - d;
                    if (a <= 3 && !_pageBuilt[a]) { BuildPage88(a, false); return; }
                    if (b >= 0 && !_pageBuilt[b]) { BuildPage88(b, false); return; }
                }
        }

        /* The pages to `to` from wherever they are now; the first step on this very frame. */
        void MovePages88(int to, float seconds)
        {
            _pageFrom = _pagePos; _pageTo = to;
            _pageT = 1 / 60f;   // this frame already shows the move
            _pageAnim = Mathf.Abs(_pageTo - _pageFrom) > 1e-4f;
            _pageAnimFrame = Time.frameCount;
            _pageFrames = _pageSlow = 0; _pageMaxDt = 0;
            MenuTabs.Sliding = _pageAnim;
            if (_pageAnim && Mathf.Abs(_pageTo - _pageFrom) > .3f) Gecis100.Play(Gecis100.Kind.Sekme);   // PM_100 §4: a short swish
            float k = Mathf.Clamp01(_pageT / Mathf.Max(.05f, seconds));
            _pagePos = _pageAnim ? Mathf.LerpUnclamped(_pageFrom, _pageTo, OutQuint(k)) : to;
            PlacePages();
        }

        /* The chosen tab changes: the bar at once, the trophies seen. */
        void ChooseTab88(int tab)
        {
            if (!_pageBuilt[tab]) BuildPage88(tab, false);   // pressed before it was built ahead (rare): built now
            _tab = (MenuTab)tab;
            _tabs?.Select(tab);
            if (_tab == MenuTab.Trophies && !_previewWins)
            {
                int completed = Achievements.Completed(CurrentAchievements());
                Host.Current.Store.Set("rs_ach_seen", completed.ToString()); Host.Current.Store.Save();
                _tabs?.SetBadge(0);
            }
        }

        /* A tab pressed (on the touch's own frame). */
        void PressTab88(int tab)
        {
            RememberMenuScroll();
            if (tab != (int)_tab) ChooseTab88(tab);
            MovePages88(tab, PageSeconds);
        }

        /* The touch went down on the bar: the tab acts now, not on the release. True when it did. */
        bool PressBar88(Vector2 pt, double inputAge)
        {
            if (!SwipeAllowed && !(_levelsMapOpen && _menuSettings == null && _paintingDetail == null && _profileCard == null && _card == null && !_diving)) return false;
            int tab = _tabs?.Hit(pt) ?? -1;
            if (tab < 0) return false;
            var clock = System.Diagnostics.Stopwatch.StartNew();
            Haptics.Button();
            float haptics = (float)clock.Elapsed.TotalMilliseconds;
            _pageInputMs = (float)(inputAge * 1000);
            if (_levelsMapOpen || !PagesOn)
            {
                RememberMenuScroll();
                _tabs.Press(tab, () => { if (_mode != Mode.Menu) return; _tab = (MenuTab)tab; _levelsMapOpen = false; Layout(); });
                return true;
            }
            PressTab88(tab);
            // PM_88 §3: what the press costs on its own frame (the move starts on it)
            Debug.Log($"[sekme] press {tab}: {clock.Elapsed.TotalMilliseconds:0.0} ms on the touch's frame (sound and haptics {haptics:0.0}), input {_pageInputMs:0.0} ms old, pages at {_pagePos:0.000}");
            return true;
        }

        /* The finger has gone 10 pt sideways: the pages follow it (from where they are, a move under way stops). */
        void BeginSwipe88(Vector2 start, Vector2 pt, float now)
        {
            if (!PagesOn) { _swipeDead = true; return; }
            _pageAnim = false;
            _swipe = true;
            MenuTabs.Sliding = true;
            _swipeBase = Mathf.Round(_pagePos);
            _swipeSamples.Clear(); _swipeSamples.Add((now, pt.x));
            DragSwipe88(start, pt, now);
        }

        void DragSwipe88(Vector2 start, Vector2 pt, float now)
        {
            float lo = Mathf.Max(0, _swipeBase - 1), hi = Mathf.Min(3, _swipeBase + 1);
            _pagePos = Mathf.Clamp(_swipeBase - (pt.x - start.x) / Mathf.Max(1, _screen.x), lo, hi);
            PlacePages();
            _swipeSamples.Add((now, pt.x));
            while (_swipeSamples.Count > 2 && now - _swipeSamples[1].t >= MenuScroll.Window) _swipeSamples.RemoveAt(0);
        }

        void ReleaseSwipe88(float now)
        {
            _swipe = false;
            float v = 0;   // pt/s toward the higher tabs (the content flowing left)
            if (_swipeSamples.Count >= 2 && now - _swipeSamples[_swipeSamples.Count - 1].t <= MenuScroll.Window)
            {
                var b = _swipeSamples[_swipeSamples.Count - 1]; var a = _swipeSamples[0];
                if (b.t - a.t > 1e-3f) v = -(b.x - a.x) / (b.t - a.t);
            }
            float d = _pagePos - _swipeBase;
            int target = (int)_swipeBase;
            if (v > SwipeFlick && d > .02f || v >= -SwipeFlick && d > .5f) target++;
            else if (v < -SwipeFlick && d < -.02f || v <= SwipeFlick && d < -.5f) target--;
            target = Mathf.Clamp(target, 0, 3);
            Debug.Log($"[sekme] swipe {(target != (int)_swipeBase ? "settles" : "springs back")} at {d:0.00}, {v:0} pt/s");
            if (target != (int)_tab) { Haptics.Button(); RememberMenuScroll(); ChooseTab88(target); }
            _pageInputMs = 0;
            MovePages88(target, PageSeconds);
            if (!_pageAnim) MenuTabs.Sliding = false;
        }
    }
}
