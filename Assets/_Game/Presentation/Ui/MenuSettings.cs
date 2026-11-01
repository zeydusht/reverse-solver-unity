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
        public MenuSettings(Transform parent, Vector2 screen, int order)
        {
            Root = new GameObject("menu-settings").transform; Root.SetParent(parent, false);
            var c = screen / 2;
            ShapeView.Create(Root, "veil", screen, 0, order).Fill(Draw.Rgba(0, 0, 0, .65f)).transform.localPosition = Draw.W(c);
            float w = Mathf.Min(330, screen.x - 30), h = Mathf.Min(560, screen.y - 60), top = c.y - h / 2;
            MenuUi.Sprite(Root, "modal-panel", c, new Vector2(w, h), order + 1, true);
            MenuUi.Label(Root, Texts.Settings, new Vector2(c.x, top + 40), 30, order + 3, true);
            Add("close", "button-close", null, new Vector2(c.x + w / 2 - 23, top + 22), Vector2.one * 36, order + 5);
            string[] keys = { "sound", "music", "vibration" };
            string[] ids = { "sound", "music", "haptics" };
            bool[] states = { !Art.Muted, Music.On, Haptics.On };
            for (int i = 0; i < 3; i++)
            {
                float y = top + 102 + i * 60;
                MenuUi.Sprite(Root, "settings-icon-" + keys[i], new Vector2(c.x - w / 2 + 42, y), Vector2.one * 34, order + 2);
                MenuUi.Label(Root, Texts.SettingsRow(i), new Vector2(c.x - 28, y), 19, order + 3, true, 110);
                float tx = c.x + w / 2 - 56;
                Add(ids[i], states[i] ? "toggle-on-track" : "toggle-off-track", null, new Vector2(tx, y), new Vector2(65, 36), order + 2);
                MenuUi.Sprite(Root, "toggle-knob", new Vector2(tx + (states[i] ? 14 : -14), y), Vector2.one * 28, order + 3);
            }
            float ly = top + 302;
            MenuUi.Label(Root, Texts.SettingsLanguage, new Vector2(c.x - 110, ly), 17, order + 3, true, 55);
            MenuUi.Sprite(Root, Lang.Tr ? "language-left-selected" : "language-right-selected", new Vector2(c.x + 27, ly), new Vector2(190, 50), order + 2, true);
            MenuUi.Label(Root, "Türkçe", new Vector2(c.x - 20, ly), 17, order + 3, true);
            MenuUi.Label(Root, "English", new Vector2(c.x + 74, ly), 17, order + 3, true);
            _buttons.Add((new Rect(c.x - 68, ly - 25, 95, 50), "tr"));
            _buttons.Add((new Rect(c.x + 27, ly - 25, 95, 50), "en"));
            Add("privacy", "button-secondary", Texts.SettingsPrivacy, new Vector2(c.x, top + h - 139), new Vector2(w - 66, 49), order + 3);
            Add("contact", "button-secondary", Texts.SettingsContact, new Vector2(c.x, top + h - 79), new Vector2(w - 66, 49), order + 3);
            MenuUi.Label(Root, BuildStamp.Value, new Vector2(c.x, top + h - 29), 12, order + 4, true);
        }
        void Add(string id, string key, string label, Vector2 c, Vector2 size, int order) => _buttons.Add((MenuUi.Button(Root, key, label, c, size, order), id));
        public string Hit(Vector2 pt) { foreach (var b in _buttons) if (b.rect.Contains(pt)) return b.id; return null; }
        public void Destroy() { if (Root != null) Object.Destroy(Root.gameObject); }
    }
}
