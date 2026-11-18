using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_86 §9: Codex's simpler set (Art/yeni/pm86/basari, option A): a cream row in a royal blue frame, navy medallions
       with a gold rim, a cream-brown groove with a gold fill; TASK20's art stays where a part is missing. Looks only. */
    public sealed class AchievementsScreen
    {
        static SpriteRenderer Part(Transform root, string key, string old, Vector2 c, Vector2 size, int order, bool sliced = false)
        {
            var r = Art.Place(root, "yeni/pm86/basari/pm86-ach-" + key, size, order, sliced);
            if (r == null) return MenuUi.Sprite(root, old, c, size, order, sliced);
            r.transform.localPosition = Draw.W(c);
            return r;
        }

        public readonly MenuScroll Scroll;
        public AchievementsScreen(Transform root, Vector2 screen, float top, float bottom, AchievementProgress[] achievements, int order, float offset)
        {
            MenuUi.Scene(root, "achievements", screen, order - 10);
            MenuUi.Title(root, screen, top, Texts.Tab(3), order + 30);
            MenuUi.Sprite(root, "counter-pill", new Vector2(screen.x / 2, top + 88), new Vector2(96, 36), order + 30, true);
            MenuUi.Banner(root, $"{Achievements.Completed(achievements)} / 9", new Vector2(screen.x / 2, top + 88), 19, order + 31);
            float start = top + 120, w = Mathf.Min(365, screen.x - 24), pitch = 108;
            Scroll = new MenuScroll(root, start, bottom, achievements.Length * pitch);
            string[] keys = { "first-picture", "collector", "museum", "bomb", "hourglass", "flawless", "speed", "chain", "baron-hat" };
            for (int i = 0; i < achievements.Length; i++)
            {
                var a = achievements[i];
                var cell = Scroll.Cell(screen.x / 2, start + 50 + i * pitch, 50);
                Part(cell, "tile", "achievement-tile", Vector2.zero, new Vector2(w, 100), order, true);
                Part(cell, "badge-" + keys[i], "achievement-" + keys[i], new Vector2(-w / 2 + 45, 0), Vector2.one * 72, order + 1);
                float left = -w / 2 + 86, tw = w - 126, cx = left + tw / 2;
                MenuUi.Label(cell, Texts.AchievementName(i), new Vector2(cx, -29), 19, order + 2, true, tw);
                var condition = MenuUi.Label(cell, Texts.AchievementCondition(i), new Vector2(cx, -3), 13, order + 2, true);
                var tmp = condition.GetComponent<TMPro.TextMeshPro>();
                tmp.textWrappingMode = TMPro.TextWrappingModes.Normal;
                tmp.rectTransform.sizeDelta = new Vector2(tw, 38);
                tmp.enableAutoSizing = true;
                tmp.fontSizeMin = 12 * TextView.PointToFontSize;
                tmp.fontSizeMax = 13 * TextView.PointToFontSize;
                Part(cell, "track", "progress-track", new Vector2(cx, 27), new Vector2(tw, 20), order + 1, true);
                float fill = a.Value > 0 ? Mathf.Max(16, (tw - 8) * a.Value / a.Target) : 0;
                if (fill > 0) Part(cell, "fill", "progress-fill", new Vector2(left + 4 + fill / 2, 27), new Vector2(fill, 13), order + 2, true);
                MenuUi.Label(cell, $"{a.Value}/{a.Target}", new Vector2(cx, 27), 14, order + 3, true);
                if (a.Done) Part(cell, "check", "icon-green-check", new Vector2(w / 2 - 24, -22), Vector2.one * 28, order + 3);
                else if (a.Value == 0)
                {
                    Part(cell, "lock", "icon-lock", new Vector2(w / 2 - 24, -22), Vector2.one * 26, order + 3);
                    foreach (var r in cell.GetComponentsInChildren<SpriteRenderer>()) r.color = new Color(.82f, .82f, .82f, 1);
                }
            }
            MenuUi.Viewport(root, "achievements", screen, start, bottom, order + 20);
            Scroll.Set(offset);
        }
    }
}
