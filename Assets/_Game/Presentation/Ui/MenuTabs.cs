using System;
using System.Collections.Generic;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    public enum MenuTab { Home, Collection, Jiggles, Trophies }

    // Coordinates and hit regions use points, y down, like Screens.Menu.
    public static class MenuUi
    {
        public static SpriteRenderer Sprite21(Transform root, string key, Vector2 c, Vector2 size, int order, bool sliced = false)
        {
            var r = Art.Place(root, "t21-" + key, size, order, sliced);
            if (r != null) r.transform.localPosition = Draw.W(c);
            return r;
        }

        public static SpriteRenderer Sprite(Transform root, string key, Vector2 c, Vector2 size, int order, bool sliced = false)
        {
            // PM_67: the turquoise pills and small buttons become T48's blue plate.
            if (T48 && (key == "counter-pill" || key == "button-small")) return Put48(root, "profile-pill", c, size, order, true);
            var r = Art.Place(root, "t20-" + key, size, order, sliced);
            if (r != null) r.transform.localPosition = Draw.W(c);
            return r;
        }

        public static TextView Label(Transform root, string text, Vector2 c, float size, int order, bool dark = false, float width = 0, bool titleFont = false)
        {
            var t = TextView.Create(root, "label", text, size, dark ? Palette.Navy : Palette.Cream, true, order: order);
            if (titleFont) t.Font(TextView.TitleFont);
            if (width > 0) t.FitWidth(width, 8);
            t.transform.localPosition = Draw.W(c);
            return t;
        }

        /* PM_MESAJ_27 A3: banners, counters and button labels (TextView.Banner). */
        public static TextView Banner(Transform root, string text, Vector2 c, float size, int order, float width = 0, bool green = false)
        {
            var t = TextView.Create(root, "banner", text, size, Palette.Cream, false, order: order);
            if (T48) t.Heading(green ? TextView.RimGreen : Draw.Hex("#0E2244")); else t.Banner(green ? TextView.OnGreen : TextView.OnTeal);
            if (width > 0) t.FitWidth(width, 8);
            t.transform.localPosition = Draw.W(c);
            return t;
        }

        public static void Scene(Transform root, string key, Vector2 screen, int order)
        {
            SceneRegion(root, key, screen, 0, screen.y, order, true);
        }

        // Opaque slices of the same stationary scene cover scrolling art and text at the viewport edges.
        // They sit below the fixed title/nav, so both sprite and TMP renderers clip identically.
        public static void Viewport(Transform root, string key, Vector2 screen, float top, float bottom, int order)
        {
            SceneSlice(root, key, screen, 0, top, order);
            SceneSlice(root, key, screen, bottom, screen.y, order);
        }

        static void SceneSlice(Transform root, string key, Vector2 screen, float from, float to, int order)
        {
            SceneRegion(root, key, screen, from, to, order, false);
        }

        static void SceneRegion(Transform root, string key, Vector2 screen, float from, float to, int order, bool fade)
        {
            if (to <= from) return;
            var fallback = ShapeView.Create(root, "scene-colour", new Vector2(screen.x, to - from), 0, order).Fill(MenuArt.Background(key));
            fallback.transform.localPosition = Draw.W(screen.x / 2, (from + to) / 2);
            bool late = true;   // PM_86 §7: a picture already in memory shows on the same frame, whole (no fade)
            Action<Sprite> show = source =>
            {
                if (root == null || fallback == null) return;
                var rect = source.rect;
                var crop = new Rect(rect.x, rect.y + rect.height * (1 - to / screen.y), rect.width, rect.height * (to - from) / screen.y);
                var sprite = fade ? source : UnityEngine.Sprite.Create(source.texture, crop, new Vector2(.5f, .5f), 1);
                var r = new GameObject(fade ? "scene-" + key : "viewport-cover").AddComponent<SpriteRenderer>();
                r.transform.SetParent(fallback.transform, false); r.sprite = sprite; r.sortingOrder = order + (fade ? 1 : 0);
                r.transform.localScale = new Vector3(screen.x / crop.width, (to - from) / crop.height, 1);
                if (!fade)
                {
                    r.gameObject.AddComponent<MenuPainting>().Owned = sprite;
                    fallback.GetComponent<MeshRenderer>().enabled = false;
                    return;
                }
                if (!late) return;
                r.color = new Color(1, 1, 1, 0);
                Tweens.Run(r, .2f, t => { if (r != null) r.color = new Color(1, 1, 1, t * t * (3 - 2 * t)); });
            };
            var ready = MenuArt.Instance.Get("t20-scene-" + key + ".jpg", show);
            if (ready != null) { late = false; show(ready); }
        }

        /* PM_67: TASK48 menu (old money blue strips, ribbonless 3D tab titles). */
        public static bool T48 => Art.Get("T48/menu/bottom-strip") != null;
        public static SpriteRenderer Put48(Transform root, string key, Vector2 c, Vector2 size, int order, bool sliced = false)
        {
            var r = Art.Place(root, "T48/menu/" + key, size, order, sliced);
            if (r != null) r.transform.localPosition = Draw.W(c);
            return r;
        }

        public static void Title(Transform root, Vector2 screen, float top, string title, int order, bool heading = false)
        {
            if (T48)
            {
                Put48(root, "menu_baslik_isik", new Vector2(screen.x / 2, top + 32), new Vector2(screen.x, 56), order);
                var t3 = TextView.Create(root, "banner", title, 32, Palette.Cream, false, order: order + 1).Title3D().FitWidth(screen.x - 120, 20);
                t3.transform.localPosition = Draw.W(screen.x / 2, top + 31);
                return;
            }
            Sprite(root, "title-plaque", new Vector2(screen.x / 2, top + 34), new Vector2(240, 60), order, true);
            if (heading)   // PM_MESAJ_42: Baloo heading style
            {
                var t = TextView.Create(root, "banner", title, 28, Palette.Cream, false, order: order + 1).Heading(TextView.RimTeal).FitWidth(220, 19);
                t.transform.localPosition = Draw.W(screen.x / 2, top + 32);
                return;
            }
            Banner(root, title, new Vector2(screen.x / 2, top + 33), 27, order + 1, 220);
        }

        public static Rect Button(Transform root, string key, string text, Vector2 c, Vector2 size, int order)
        {
            Sprite(root, key, c, size, order, key == "button-secondary" || key == "button-small");
            if (!string.IsNullOrEmpty(text))
            {
                if (T48 && key == "button-small")
                {
                    var t = TextView.Create(root, "banner", text, 16, Palette.Cream, false, order: order + 1).Heading(Draw.Hex("#0E2244")).FitWidth(size.x - 18, 8);
                    t.transform.localPosition = Draw.W(c);
                }
                else Banner(root, text, c, 18, order + 1, size.x - 18);
            }
            return new Rect(c - size / 2, size);
        }

        public static void Painting(Transform root, LevelImages images, int index, Vector2 c, Vector2 size, int order)
        {
            Action<Texture2D> show = tex =>
            {
                if (root == null || tex == null) return;
                float h = Mathf.Min(tex.height, tex.width * 4f / 3f), w = h * .75f;
                var sp = UnityEngine.Sprite.Create(tex, new Rect((tex.width - w) / 2, (tex.height - h) / 2, w, h), new Vector2(.5f, .5f), h / size.y);
                var r = new GameObject("painting").AddComponent<SpriteRenderer>();
                r.transform.SetParent(root, false);
                r.transform.localPosition = Draw.W(c);
                r.sprite = sp;
                r.sortingOrder = order;
                r.gameObject.AddComponent<MenuPainting>().Owned = sp;
                r.color = new Color(1, 1, 1, 0);
                Tweens.Run(r, .25f, t => { if (r != null) r.color = new Color(1, 1, 1, t * t * (3 - 2 * t)); });
            };
            var pic = images.Get($"painting-{index + 1:00}", show);
            if (pic != null) show(pic);
        }
    }

    /* PM_86 §8: the bottom bar. Four tabs with cream dividers between them; the chosen tab's face widens evenly to
       both sides about its own middle (kept on the screen at the ends), the others share the rest, narrower; a warm
       light behind its icon,
       which rises and grows, its label under it; the others are a little dim. 0.25 s, a soft ease-out-back ending,
       the others narrowing at the same time; the menu's content changes at once (the new bar carries on the move).
       Codex's parts in Art/yeni/pm86/menu (strip, selected, divider, glow); TASK48's when they are missing. */
    public sealed class MenuTabs
    {
        public const float Seconds = .25f;
        public readonly float Top;
        readonly float _width, _bottom;
        readonly SpriteRenderer[] _icons = new SpriteRenderer[4], _dividers = new SpriteRenderer[3];
        readonly TextView[] _labels = new TextView[4];
        readonly SpriteRenderer _face, _glow;
        readonly string[] _keys = { "home", "collection", "jiggles", "trophy" };
        readonly float[] _weights = new float[4];
        readonly SpriteRenderer _badge;
        readonly TextView _badgeText;
        int _selected;
        static float[] _carry;   // the weights the last bar had when a tab was pressed: the new bar moves on from them
        float[] _from;
        float _born;

        public readonly Transform Root;   // PM_76: one group, so the bar can rise in (MenuEnter76)
        static bool New86 => Art.Get("yeni/pm86/menu/pm86-tab-strip") != null;
        static SpriteRenderer Part(Transform root, string key, int order, bool sliced)
        {
            if (New86) return Art.Place(root, "yeni/pm86/menu/pm86-tab-" + key, Vector2.one, order, sliced);
            string old = key == "strip" ? "bottom-strip" : key == "selected" ? "tab-selected" : key == "divider" ? "tab-divider" : null;
            return old != null && MenuUi.T48 ? MenuUi.Put48(root, old, Vector2.zero, Vector2.one, order, true) : null;
        }

        public MenuTabs(Transform root, Vector2 screen, Vector4 safe, MenuTab selected, int order, int TrophyBadge = 0)
        {
            Root = new GameObject("tabs").transform; Root.SetParent(root, false); root = Root;
            _width = screen.x; _bottom = screen.y; _selected = (int)selected;
            Top = screen.y - Mathf.Max(34, safe.w) - 52;
            var strip = Part(root, "strip", order, true);
            if (strip != null)
            {
                strip.size = new Vector2(screen.x + 2, screen.y - Top + 6);
                strip.transform.localPosition = Draw.W(screen.x / 2, (Top - 6 + screen.y) / 2);
            }
            else MenuUi.Sprite21(root, "tabbar-bg", new Vector2((screen.x + 1) / 2, (Top + screen.y + 2f / 3) / 2), new Vector2(screen.x + 1, screen.y - Top + 2f / 3), order, true);
            _glow = Part(root, "glow", order + 1, false);
            _face = Part(root, "selected", order + 2, true);
            for (int i = 0; i < 3; i++) _dividers[i] = Part(root, "divider", order + 1, true);
            for (int i = 0; i < 4; i++)
            {
                _weights[i] = i == _selected ? 1 : 0;
                _icons[i] = MenuUi.Sprite21(root, "tab-icon-" + _keys[i] + "-normal", Vector2.zero, Vector2.one, order + 4);
                _labels[i] = TextView.Create(root, "banner", Texts.Tab(i), 15, Palette.Cream, false, order: order + 6).Heading(Draw.Hex("#0E2244")).FitWidth(screen.x * .34f, 8);
            }
            if (TrophyBadge > 0)
            {
                _badge = MenuUi.Sprite21(root, "tab-badge", Vector2.zero, Vector2.one * 19, order + 7);
                _badgeText = MenuUi.Label(root, TrophyBadge.ToString(), Vector2.zero, 12, order + 8, width: 16);
            }
            if (_carry != null)
            {
                _from = _carry; _carry = null; _born = Time.unscaledTime;
                Tweens.Run(this, Seconds, t =>
                {
                    if (_labels[0] == null) return;
                    float e = PanelMotion76.OutBack(t, .9f);   // a soft overshoot, nothing jumps
                    for (int i = 0; i < 4; i++) _weights[i] = Mathf.LerpUnclamped(_from[i], i == _selected ? 1 : 0, e);
                    Arrange();
                });
                for (int i = 0; i < 4; i++) _weights[i] = _from[i];
            }
            Arrange();
        }

        float Quarter => _width / 4;
        float Centre(int i) => Quarter * (i + .5f);
        readonly float[] _edges = { 0, 0, 0, 0, 0 };   // the tabs' spans now (the chosen one wide, the others share the rest)

        void Arrange()
        {
            float slotTop = Top + 2, slotHeight = _bottom - slotTop;
            // the face: between the tabs by their weights (it slides from the old one to the new), 1.45 quarters wide,
            // its middle on the chosen tab's own middle (kept on the screen at the ends); the other tabs share the
            // rest on either side, narrower
            float sum = 0, cx = 0;
            for (int i = 0; i < 4; i++) { float w = Mathf.Max(0, _weights[i]); sum += w; cx += w * Centre(i); }
            cx = sum > 1e-4f ? cx / sum : Centre(_selected);
            float faceW = Quarter * Mathf.LerpUnclamped(1, 1.45f, Mathf.Clamp(sum, 0, 1.2f)), faceH = slotHeight + 10;
            float fx = Mathf.Clamp(cx, faceW / 2, _width - faceW / 2);
            float left = fx - faceW / 2, right = fx + faceW / 2;
            int sel = _selected;
            _edges[0] = 0; _edges[4] = _width;
            for (int i = 1; i <= sel; i++) _edges[i] = sel > 0 ? left * i / sel : 0;
            for (int i = sel + 1; i < 4; i++) _edges[i] = right + (_width - right) * (i - sel - 1) / (3 - sel);
            _edges[sel] = left; _edges[sel + 1] = right;
            if (_face != null)
            {
                _face.size = new Vector2(faceW, faceH);
                _face.transform.localPosition = Draw.W(fx, _bottom - faceH / 2);
            }
            if (_glow != null)
            {
                _glow.transform.localScale = Vector3.one * (faceW * 1.05f / Mathf.Max(1, _glow.sprite.bounds.size.x));
                _glow.transform.localPosition = Draw.W(fx, Top + 6);
            }
            for (int i = 0; i < 3; i++)
            {
                var d = _dividers[i];
                if (d == null) continue;
                float x = _edges[i + 1];
                d.size = new Vector2(4, slotHeight * .74f);
                d.transform.localPosition = Draw.W(x, slotTop + slotHeight * .5f);
                // the face's own gold edge is the line beside the chosen tab
                bool byFace = i + 1 == sel || i + 1 == sel + 1;
                d.color = new Color(1, 1, 1, byFace ? 0 : 1);
            }
            for (int i = 0; i < 4; i++)
            {
                float active = Mathf.Clamp(_weights[i], -.2f, 1.2f);
                float x = i == sel ? fx : (_edges[i] + _edges[i + 1]) / 2;
                // the chosen icon rises and grows (at most a quarter of its height over the bar); the others are a
                // little smaller and dim
                float height = Mathf.LerpUnclamped(42, 58, active), y = Top + Mathf.LerpUnclamped(7, -16, active);
                var icon = _icons[i];
                if (icon != null)
                {
                    icon.sprite = Art.Get("t21-tab-icon-" + _keys[i] + (i == sel ? "-selected-large" : "-normal"));
                    icon.transform.localScale = Vector3.one * (height / icon.sprite.bounds.size.y);
                    icon.transform.localPosition = Draw.W(x, y + height / 2);
                    float lit = Mathf.Lerp(.72f, 1, Mathf.Clamp01(active));
                    icon.color = new Color(lit, lit, lit, 1);
                }
                _labels[i].Alpha = Mathf.Clamp01(active);
                _labels[i].transform.localPosition = Draw.W(x, Top + 52);
                if (i == 3)
                {
                    float iw = icon != null ? icon.sprite.bounds.size.x * icon.transform.localScale.x : height;
                    var badgeC = new Vector2(x + iw / 2 - 3, y + 5);
                    if (_badge != null) _badge.transform.localPosition = Draw.W(badgeC);
                    if (_badgeText != null) _badgeText.transform.localPosition = Draw.W(badgeC);
                }
            }
        }

        public int Hit(Vector2 pt)
        {
            if (pt.y < Top - 14 || pt.y > _bottom || pt.x < 0 || pt.x >= _width) return -1;
            for (int i = 0; i < 4; i++) if (pt.x < _edges[i + 1]) return i;
            return 3;
        }

        /* A tab pressed: select tap and click; the menu's content changes now, the next bar finishes the move. */
        public void Press(int index, Action done)
        {
            if (index < 0 || index >= 4) return;
            _carry = (float[])_weights.Clone();
            done?.Invoke();
        }

        /* PM_86 test: where tab i is (its middle, layout points). */
        public Vector2 TabCentre(int i) => new Vector2((_edges[i] + _edges[i + 1]) / 2, Top + 26);
    }

    // The transform moves as a unit. Fully offscreen cells are inactive; scene slices clip the visible edges.
    // PM_MESAJ_27 A2: phone feel. A flick keeps going from the finger's speed over its last 80 ms and slows down
    // exponentially; past either end the list stretches with resistance and springs back (Unity ScrollRect's
    // rubber band and elasticity 0.1).
    public sealed class MenuScroll
    {
        public const float Decay = 2f, Elasticity = .1f, Window = .08f, StopSpeed = 20f;
        public readonly Transform Root;
        public readonly float Top, Bottom;
        public float Offset { get; private set; }
        public float Velocity { get; private set; }
        public bool Held { get; private set; }
        public bool Moving => !Held && (Mathf.Abs(Velocity) > StopSpeed || Offset < 0 || Offset > _max);
        readonly float _max;
        float _grab;
        readonly List<(float t, float y)> _samples = new List<(float, float)>();
        sealed class CellEntry
        {
            public Transform Root;
            public float Y, Half;
            public Action<Transform> Visible;
        }
        readonly List<CellEntry> _cells = new List<CellEntry>();
        public MenuScroll(Transform parent, float top, float bottom, float height)
        {
            Root = new GameObject("scroll-content").transform; Root.SetParent(parent, false);
            Top = top; Bottom = bottom; _max = Mathf.Max(0, height - (bottom - top));
        }
        public Transform Cell(float x, float y, float half, Action<Transform> visible = null)
        {
            var t = new GameObject("cell").transform; t.SetParent(Root, false); t.localPosition = Draw.W(x, y);
            _cells.Add(new CellEntry { Root = t, Y = y, Half = half, Visible = visible }); return t;
        }
        public bool Contains(Vector2 pt) => pt.y >= Top && pt.y <= Bottom;
        public float Resting => Mathf.Clamp(Offset, 0, _max);

        /* Finger down: whatever motion there was stops. */
        public void Grab(float y, float time)
        {
            Tweens.Kill(this);
            Held = true; Velocity = 0; _grab = Offset + y;
            _samples.Clear(); _samples.Add((time, y));
        }
        /* Finger at y: the content follows it, with resistance beyond the ends. */
        public void Drag(float y, float time)
        {
            float raw = _grab - y;
            Apply(raw < 0 ? -Rubber(-raw) : raw > _max ? _max + Rubber(raw - _max) : raw);
            _samples.Add((time, y));
            while (_samples.Count > 2 && time - _samples[1].t >= Window) _samples.RemoveAt(0);
        }
        public void Release(float time)
        {
            Held = false; Velocity = 0;
            if (_samples.Count < 2 || time - _samples[_samples.Count - 1].t > Window) return;   // the finger rested before lifting
            var b = _samples[_samples.Count - 1]; var a = _samples[0];
            foreach (var x in _samples) if (b.t - x.t <= Window && x.t < b.t) { a = x; break; }   // the oldest one inside the last 80 ms
            if (b.t - a.t > 1e-3f) Velocity = -(b.y - a.y) / (b.t - a.t);
            if (Host.Debug && Mathf.Abs(Velocity) > StopSpeed) { _flickFrom = Offset; _flickAt = time; _flickV = Velocity; }
        }
        float _flickFrom, _flickAt = -1, _flickV;
        /* Mouse wheel: a nudge that keeps the same easing as a flick. */
        public void Wheel(float delta) { if (!Held) Velocity += delta * Decay; }

        public void Tick(float dt)
        {
            if (Held || dt <= 0) return;
            float bound = Offset < 0 ? 0 : Offset > _max ? _max : Offset;
            if (bound != Offset)
            {
                float v = Velocity;
                float o = Mathf.SmoothDamp(Offset, bound, ref v, Elasticity, Mathf.Infinity, dt);
                Velocity = v;
                if (Mathf.Abs(o - bound) < .5f) { o = bound; Velocity = 0; }
                Apply(o);
            }
            else if (Mathf.Abs(Velocity) > StopSpeed)
            {
                Velocity *= Mathf.Exp(-Decay * dt);
                Apply(Offset + Velocity * dt);
            }
            else Velocity = 0;
            if (_flickAt >= 0 && !Moving)
            {
                // ?debug=1: the flick test in Tools/verify_build.js reads this
                Debug.Log(string.Format(System.Globalization.CultureInfo.InvariantCulture, "[scroll] v {0:0} pt/s, {1:0} pt, {2:0.00} s",
                                        _flickV, Offset - _flickFrom, Time.unscaledTime - _flickAt));
                _flickAt = -1;
            }
        }

        float Rubber(float over)
        {
            float view = Bottom - Top;
            return (1 - 1 / (over * .55f / view + 1)) * view;
        }

        public void Move(float delta) => Set(Offset - delta);
        public void Set(float offset) { Velocity = 0; Apply(Mathf.Clamp(offset, 0, _max)); }
        void Apply(float offset)
        {
            Offset = offset; Root.localPosition = Draw.W(0, -Offset);
            foreach (var c in _cells)
            {
                bool active = c.Y - Offset + c.Half >= Top && c.Y - Offset - c.Half <= Bottom;
                c.Root.gameObject.SetActive(active);
                if (active && c.Visible != null)
                {
                    var show = c.Visible; c.Visible = null; show(c.Root);
                }
            }
        }
    }
}
