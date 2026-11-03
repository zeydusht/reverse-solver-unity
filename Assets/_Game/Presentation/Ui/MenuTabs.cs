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
            var t = TextView.Create(root, "banner", text, size, Palette.Cream, false, order: order).Banner(green ? TextView.OnGreen : TextView.OnTeal);
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
                r.color = new Color(1, 1, 1, 0);
                Tweens.Run(r, .2f, t => { if (r != null) r.color = new Color(1, 1, 1, t); });
            };
            var ready = MenuArt.Instance.Get("t20-scene-" + key + ".jpg", show);
            if (ready != null) show(ready);
        }

        public static void Title(Transform root, Vector2 screen, float top, string title, int order, bool heading = false)
        {
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
            if (!string.IsNullOrEmpty(text)) Banner(root, text, c, 18, order + 1, size.x - 18);
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
                Tweens.Run(r, .25f, t => { if (r != null) r.color = new Color(1, 1, 1, t); });
            };
            var pic = images.Get($"painting-{index + 1:00}", show);
            if (pic != null) show(pic);
        }
    }

    public sealed class MenuTabs
    {
        public readonly float Top;
        readonly float _width, _bottom;
        readonly SpriteRenderer[] _slots = new SpriteRenderer[4], _icons = new SpriteRenderer[4], _dividers = new SpriteRenderer[3];
        readonly TextView[] _labels = new TextView[4];
        readonly float[] _edges = new float[5];
        readonly string[] _keys = { "home", "collection", "jiggles", "trophy" };
        readonly float[] _weights = new float[4];
        readonly SpriteRenderer _badge;
        readonly TextView _badgeText;
        int _selected;

        public MenuTabs(Transform root, Vector2 screen, Vector4 safe, MenuTab selected, int order, int TrophyBadge = 0)
        {
            _width = screen.x; _bottom = screen.y; _selected = (int)selected;
            Top = screen.y - Mathf.Max(34, safe.w) - 52;
            // The importer pads 1125x258 to 1128x260; push that transparent right/bottom padding offscreen.
            MenuUi.Sprite21(root, "tabbar-bg", new Vector2((screen.x + 1) / 2, (Top + screen.y + 2f / 3) / 2), new Vector2(screen.x + 1, screen.y - Top + 2f / 3), order, true);
            for (int i = 0; i < 4; i++)
            {
                _weights[i] = i == _selected ? 1 : 0;
                _slots[i] = MenuUi.Sprite21(root, "tab-slot", Vector2.zero, Vector2.one, order + 1, true);
                _icons[i] = MenuUi.Sprite21(root, "tab-icon-" + _keys[i] + "-normal", Vector2.zero, Vector2.one, order + 3);
                _labels[i] = MenuUi.Banner(root, Texts.Tab(i), Vector2.zero, 15, order + 5, screen.x * .4f - 12);
                if (i > 0) _dividers[i - 1] = MenuUi.Sprite21(root, "tab-divider", Vector2.zero, Vector2.one, order + 2, true);
            }
            if (TrophyBadge > 0)
            {
                _badge = MenuUi.Sprite21(root, "tab-badge", Vector2.zero, Vector2.one * 19, order + 6);
                _badgeText = MenuUi.Label(root, TrophyBadge.ToString(), Vector2.zero, 12, order + 7, width: 16);
            }
            Arrange();
        }
        void Arrange()
        {
            float x = 0, slotTop = Top + 10f / 3, slotHeight = _bottom - slotTop;
            for (int i = 0; i < 4; i++)
            {
                float active = _weights[i], width = _width * (1 + active) / 5, cx = x + width / 2;
                _edges[i] = x; x += width;
                if (_slots[i] != null)
                {
                    _slots[i].sprite = Art.Get("t21-tab-slot" + (i == _selected ? "-selected" : ""));
                    _slots[i].size = new Vector2(width, slotHeight);
                    _slots[i].transform.localPosition = Draw.W(cx, slotTop + slotHeight / 2);
                }
                // PM_MESAJ_29: the selected icon rises at most a quarter of its height over the bar
                float height = Mathf.Lerp(49, 56, active), y = Top + Mathf.Lerp(3, -14, active);
                var icon = _icons[i];
                if (icon != null)
                {
                    icon.sprite = Art.Get("t21-tab-icon-" + _keys[i] + (i == _selected ? "-selected-large" : "-normal"));
                    float scale = height / icon.sprite.bounds.size.y;
                    icon.transform.localScale = Vector3.one * scale;
                    icon.transform.localPosition = Draw.W(cx, y + height / 2);
                }
                _labels[i].Alpha = active;
                _labels[i].transform.localPosition = Draw.W(cx, Top + 50);
                if (i > 0 && _dividers[i - 1] != null)
                {
                    _dividers[i - 1].size = new Vector2(4f / 3, slotHeight);
                    _dividers[i - 1].transform.localPosition = Draw.W(_edges[i], slotTop + slotHeight / 2);
                }
                if (i == 3)
                {
                    float iw = icon != null ? icon.sprite.bounds.size.x * icon.transform.localScale.x : height;
                    var badgeC = new Vector2(cx + iw / 2 - 3, y + 5);
                    if (_badge != null) _badge.transform.localPosition = Draw.W(badgeC);
                    if (_badgeText != null) _badgeText.transform.localPosition = Draw.W(badgeC);
                }
            }
            _edges[4] = _width;
        }
        public int Hit(Vector2 pt)
        {
            if (pt.y < Top - 14 || pt.y > _bottom || pt.x < 0 || pt.x >= _width) return -1;
            for (int i = 0; i < 4; i++) if (pt.x < _edges[i + 1]) return i;
            return -1;
        }
        public void Press(int index, Action done)
        {
            if (index < 0 || index >= 4) return;
            var from = (float[])_weights.Clone();
            _selected = index;
            Tweens.Run(this, .15f, t =>
            {
                if (_labels[0] == null) return;
                for (int i = 0; i < 4; i++) _weights[i] = Mathf.Lerp(from[i], i == index ? 1 : 0, t);
                Arrange();
            }, done);
        }
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
