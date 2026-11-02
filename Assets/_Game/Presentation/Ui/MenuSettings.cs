using System.Collections.Generic;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    public sealed class MenuSettings
    {
        public const string PrivacyUrl = "https://zeydusht.github.io/reverse-solver-unity/privacy/";
        public const string ContactUrl = "mailto:zeydusht@gmail.com";
        public readonly Transform Root;
        readonly List<(Rect rect, string id)> _buttons = new List<(Rect, string)>();
        readonly List<(Rect rect, SpriteRenderer button, Transform icon)> _iconButtons = new List<(Rect, SpriteRenderer, Transform)>();
        bool _pressAnimating;
        public MenuSettings(Transform parent, Vector2 screen, int order)
        {
            Root = new GameObject("menu-settings").transform; Root.SetParent(parent, false);
            var c = screen / 2;
            ShapeView.Create(Root, "veil", screen, 0, order).Fill(Draw.Rgba(0, 0, 0, .65f)).transform.localPosition = Draw.W(c);
            c.y += 17;
            float w = Mathf.Min(329, screen.x - 30), h = Mathf.Min(500, screen.y - 100), top = c.y - h / 2;
            MenuUi.Sprite21(Root, "settings-panel", c, new Vector2(w, h), order + 1, true);
            MenuUi.Banner(Root, Texts.Settings, new Vector2(c.x, top - 18), 30, order + 3);
            Add("close", "button-close", null, new Vector2(c.x + w / 2 - 4, top + 4), Vector2.one * 36, order + 5);
            string[] keys = { "sound", "music", "vibration" };
            string[] ids = { "sound", "music", "haptics" };
            bool[] states = { !Art.Muted, Music.On, Haptics.On };
            for (int i = 0; i < 3; i++)
            {
                float y = top + h * (.16f + i * .157f);
                MenuUi.Sprite21(Root, "settings-row", new Vector2(c.x, y), new Vector2(w - 30, 65), order + 2, true);
                MenuUi.Sprite21(Root, "settings-icon-disc", new Vector2(c.x - w / 2 + 42, y), Vector2.one * 44, order + 3);
                MenuUi.Sprite21(Root, "settings-symbol-" + keys[i], new Vector2(c.x - w / 2 + 42, y), Vector2.one * 24, order + 4);
                MenuUi.Label(Root, Texts.SettingsRow(i), new Vector2(c.x - 28, y), 19, order + 3, true, 110, true);
                float tx = c.x + w / 2 - 56;
                Add(ids[i], states[i] ? "toggle-on-track" : "toggle-off-track", null, new Vector2(tx, y), new Vector2(65, 36), order + 3);   // above the row tile
                MenuUi.Sprite(Root, "toggle-knob", new Vector2(tx + (states[i] ? 14 : -14), y), Vector2.one * 28, order + 4);
            }
            float ly = top + h * .648f;
            MenuUi.Label(Root, Texts.SettingsLanguage, new Vector2(c.x - 110, ly), 17, order + 3, true, 55, true);
            MenuUi.Sprite(Root, Lang.Tr ? "language-left-selected" : "language-right-selected", new Vector2(c.x + 27, ly), new Vector2(190, 50), order + 2, true);
            MenuUi.Label(Root, "Türkçe", new Vector2(c.x - 20, ly), 17, order + 3, true, titleFont: true);
            MenuUi.Label(Root, "English", new Vector2(c.x + 74, ly), 17, order + 3, true, titleFont: true);
            _buttons.Add((new Rect(c.x - 68, ly - 25, 95, 50), "tr"));
            _buttons.Add((new Rect(c.x + 27, ly - 25, 95, 50), "en"));
            IconButton("privacy", "icon-privacy", Texts.SettingsPrivacy, new Vector2(c.x - w * .225f, top + h - 102), order + 3);
            IconButton("contact", "icon-mail", Texts.SettingsContact, new Vector2(c.x + w * .225f, top + h - 102), order + 3);
            MenuUi.Label(Root, BuildStamp.Value, new Vector2(c.x, top + h - 29), 12, order + 4, true, titleFont: true);
        }
        void IconButton(string id, string key, string label, Vector2 c, int order)
        {
            var button = MenuUi.Sprite21(Root, "settings-icon-btn", c, Vector2.one * 54, order, true);
            var icon = MenuUi.Sprite21(Root, key, c, Vector2.one * 32, order + 1);
            var rect = new Rect(c - Vector2.one * 27, Vector2.one * 54);
            _buttons.Add((rect, id));
            if (button != null && icon != null) _iconButtons.Add((rect, button, icon.transform));
            MenuUi.Label(Root, label, c + new Vector2(0, 42), 14, order + 1, true, 110, true);
        }
        public bool Press(Vector2 pt, System.Action done)
        {
            if (_pressAnimating) return true;
            foreach (var b in _iconButtons)
            {
                if (!b.rect.Contains(pt)) continue;
                _pressAnimating = true;
                b.button.sprite = Art.Get("t21-settings-icon-btn-pressed");
                b.icon.localPosition += Draw.W(0, 2);
                Tweens.Run(this, .12f, _ => { }, () =>
                {
                    _pressAnimating = false;
                    if (Root == null) return;
                    b.button.sprite = Art.Get("t21-settings-icon-btn");
                    b.icon.localPosition -= Draw.W(0, 2);
                    done?.Invoke();
                });
                return true;
            }
            return false;
        }
        void Add(string id, string key, string label, Vector2 c, Vector2 size, int order)
        {
            MenuUi.Button(Root, key, label, c, size, order);
            var hitSize = new Vector2(Mathf.Max(44, size.x), Mathf.Max(44, size.y));
            _buttons.Add((new Rect(c - hitSize / 2, hitSize), id));
        }
        public string Hit(Vector2 pt) { foreach (var b in _buttons) if (b.rect.Contains(pt)) return b.id; return null; }
        public void Destroy() { Tweens.Kill(this); if (Root != null) Object.Destroy(Root.gameObject); }
    }
}
