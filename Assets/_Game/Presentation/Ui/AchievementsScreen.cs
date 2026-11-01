using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    public sealed class AchievementsScreen
    {
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
                MenuUi.Sprite(cell, "achievement-tile", Vector2.zero, new Vector2(w, 100), order, true);
                MenuUi.Sprite(cell, "achievement-" + keys[i], new Vector2(-w / 2 + 43, 0), Vector2.one * 70, order + 1);
                float left = -w / 2 + 86, tw = w - 126, cx = left + tw / 2;
                MenuUi.Label(cell, Texts.AchievementName(i), new Vector2(cx, -29), 19, order + 2, true, tw);
                MenuUi.Label(cell, Texts.AchievementCondition(i), new Vector2(cx, -3), 13, order + 2, true, tw);
                MenuUi.Sprite(cell, "progress-track", new Vector2(cx, 27), new Vector2(tw, 20), order + 1, true);
                float fill = (tw - 8) * a.Value / a.Target;
                if (fill > 0) MenuUi.Sprite(cell, "progress-fill", new Vector2(left + 4 + fill / 2, 27), new Vector2(fill, 12), order + 2, true);
                MenuUi.Label(cell, $"{a.Value}/{a.Target}", new Vector2(cx, 27), 15, order + 3);
                if (a.Done) MenuUi.Sprite(cell, "icon-green-check", new Vector2(w / 2 - 22, 0), Vector2.one * 25, order + 3);
                else if (a.Value == 0)
                {
                    MenuUi.Sprite(cell, "icon-lock", new Vector2(w / 2 - 22, 0), Vector2.one * 24, order + 3);
                    foreach (var r in cell.GetComponentsInChildren<SpriteRenderer>()) r.color = new Color(.7f, .7f, .7f, 1);
                }
            }
            MenuUi.Viewport(root, "achievements", screen, start, bottom, order + 20);
            Scroll.Set(offset);
        }
    }
}
