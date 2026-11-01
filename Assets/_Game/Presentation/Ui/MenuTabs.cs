using System;
using System.Collections.Generic;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    public enum MenuTab { Home, Collection, Jiggles, Trophies }

    // Coordinates and hit regions use points, y down, like Screens.Menu.
    public static class MenuUi
    {
        public static SpriteRenderer Sprite(Transform root, string key, Vector2 c, Vector2 size, int order, bool sliced = false)
        {
            var r = Art.Place(root, "t20-" + key, size, order, sliced);
            if (r != null) r.transform.localPosition = Draw.W(c);
            return r;
        }

        public static TextView Label(Transform root, string text, Vector2 c, float size, int order, bool dark = false, float width = 0)
        {
            var t = TextView.Create(root, "label", text, size, dark ? Palette.Navy : Palette.Cream, true, order: order);
            if (width > 0) t.FitWidth(width, 8);
            t.transform.localPosition = Draw.W(c);
            return t;
        }

        public static void Scene(Transform root, string key, Vector2 screen, int order)
        {
            var r = Sprite(root, "scene-" + key, screen / 2, screen, order);
            if (r != null) r.transform.localScale = new Vector3(screen.x / r.sprite.bounds.size.x, screen.y / r.sprite.bounds.size.y, 1);
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
            var source = Art.Get("t20-scene-" + key);
            if (source == null || to <= from) return;
            var rect = source.rect;
            var crop = new Rect(rect.x, rect.y + rect.height * (1 - to / screen.y), rect.width, rect.height * (to - from) / screen.y);
            var sprite = UnityEngine.Sprite.Create(source.texture, crop, new Vector2(.5f, .5f), 1);
            var r = new GameObject("viewport-cover").AddComponent<SpriteRenderer>();
            r.transform.SetParent(root, false); r.sprite = sprite; r.sortingOrder = order;
            r.transform.localPosition = Draw.W(screen.x / 2, (from + to) / 2);
            r.transform.localScale = new Vector3(screen.x / crop.width, (to - from) / crop.height, 1);
            r.gameObject.AddComponent<MenuPainting>().Owned = sprite;
        }

        public static void Title(Transform root, Vector2 screen, float top, string title, int order)
        {
            Sprite(root, "title-plaque", new Vector2(screen.x / 2, top + 34), new Vector2(240, 60), order, true);
            Label(root, title, new Vector2(screen.x / 2, top + 33), 27, order + 1, width: 220);
        }

        public static Rect Button(Transform root, string key, string text, Vector2 c, Vector2 size, int order)
        {
            Sprite(root, key, c, size, order, key == "button-secondary" || key == "button-small");
            if (!string.IsNullOrEmpty(text)) Label(root, text, c, 18, order + 1, width: size.x - 18);
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
        readonly float _width;
        readonly Transform[] _slots = new Transform[4];
        public MenuTabs(Transform root, Vector2 screen, Vector4 safe, MenuTab selected, int order, int TrophyBadge = 0)
        {
            _width = screen.x;
            Top = screen.y - Mathf.Max(34, safe.w) - 60;
            MenuUi.Sprite(root, "nav-bar-base", new Vector2(screen.x / 2, Top + 30), new Vector2(screen.x, 60), order, true);
            string[] keys = { "home", "collection", "jiggles", "trophy" };
            for (int i = 0; i < 4; i++)
            {
                bool active = i == (int)selected;
                float x = screen.x * (i + .5f) / 4;
                var slot = new GameObject("tab-" + keys[i]).transform;
                slot.SetParent(root, false); slot.localPosition = Draw.W(x, Top + 30); _slots[i] = slot;
                MenuUi.Sprite(slot, "nav-slot-" + (active ? "selected" : "normal"), Vector2.zero, new Vector2(screen.x / 4 - 3, 47.333f), order + 1, true);
                if (active) MenuUi.Sprite(slot, "nav-bubble", new Vector2(0, -20), Vector2.one * 58, order + 2);
                MenuUi.Sprite(slot, "nav-icon-" + keys[i] + (active ? "-selected" : "-normal"), new Vector2(0, -24), Vector2.one * (active ? 61.667f : 47.333f), order + 3);
                MenuUi.Label(slot, Texts.Tab(i), new Vector2(0, 19), 16, order + 5, width: screen.x / 4 - 6);
                if (i == 3 && TrophyBadge > 0)
                {
                    MenuUi.Sprite(slot, "notification-badge", new Vector2(26, -38), Vector2.one * 24, order + 6);
                    MenuUi.Label(slot, TrophyBadge.ToString(), new Vector2(26, -38), 14, order + 7);
                }
            }
        }
        public int Hit(Vector2 pt) => pt.y >= Top - 24 && pt.x >= 0 && pt.x < _width ? Mathf.Min(3, (int)(pt.x * 4 / _width)) : -1;
        public void Press(int index, Action done)
        {
            var slot = _slots[index];
            Tweens.Run(this, .12f, t => { if (slot != null) slot.localScale = Vector3.one * (1 - .1f * Mathf.Sin(t * Mathf.PI)); }, done);
        }
    }

    // The transform moves as a unit. Fully offscreen cells are inactive; scene slices clip the visible edges.
    public sealed class MenuScroll
    {
        public readonly Transform Root;
        public readonly float Top, Bottom;
        public float Offset { get; private set; }
        readonly float _max;
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
        public void Move(float delta) => Set(Offset - delta);
        public void Set(float offset)
        {
            Offset = Mathf.Clamp(offset, 0, _max); Root.localPosition = Draw.W(0, -Offset);
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
