using System.Collections.Generic;
using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    public sealed class JigglesScreen
    {
        public readonly MenuScroll Scroll;
        public static readonly string[] Colors = { "blue", "pink", "red", "green", "purple" };
        readonly Rect[] _avatars = new Rect[5];
        public JigglesScreen(Transform root, Vector2 screen, float top, float bottom, IReadOnlyList<LevelData> levels, Progress progress, int order, float offset)
        {
            MenuUi.Scene(root, "jiggles", screen, order - 10);
            MenuUi.Title(root, screen, top, Texts.Tab(2), order + 30);
            int[] counts = new int[5]; int total = 0;
            for (int i = 0; i < levels.Count; i++) if (progress.Won(i)) { counts[i % 5] += levels[i].Pieces.Count; total += levels[i].Pieces.Count; }
            MenuUi.Label(root, Texts.SavedTotal(total), new Vector2(screen.x / 2, top + 87), 22, order + 31, width: screen.x - 24);
            float start = top + 116, w = Mathf.Min(360, screen.x - 20), sh = w * 816 / 1080;
            float shelfY = start + sh / 2;
            Scroll = new MenuScroll(root, start, bottom, sh + 250);
            var shelf = Scroll.Cell(screen.x / 2, shelfY, sh / 2);
            MenuUi.Sprite(shelf, "jiggles-shelf", Vector2.zero, new Vector2(w, sh), order);
            for (int i = 0; i < 5; i++)
            {
                float x = i < 3 ? (i - 1) * w * .27f : (i - 3.5f) * w * .34f;
                float y = sh * (i < 3 ? .36f : -.08f);
                MenuUi.Sprite(shelf, "jiggle-" + Colors[i] + "-standing", new Vector2(x, y), new Vector2(w * .22f, sh * .40f), order + 1);
                MenuUi.Sprite(shelf, "color-count-pill", new Vector2(x, y + 7), new Vector2(49, 23), order + 2, true);
                MenuUi.Label(shelf, counts[i].ToString(), new Vector2(x, y + 7), 17, order + 3, true, 44);
            }
            float ay = start + sh + 59;
            var avatar = Scroll.Cell(screen.x / 2, ay, 57);
            MenuUi.Label(avatar, Texts.MyAvatar, new Vector2(0, -42), 22, order + 2, true);
            float step = w / 5, size = Mathf.Min(58, step - 5);
            for (int i = 0; i < 5; i++)
            {
                float x = (i - 2) * step;
                Avatar(avatar, i, new Vector2(x, 4), size, order + 1, i == Art.AvatarIndex);
                _avatars[i] = new Rect(screen.x / 2 + x - size / 2, ay + 4 - size / 2, size, size);
            }
            float my = ay + 107;
            var milestones = Scroll.Cell(screen.x / 2, my, 74);
            MenuUi.Sprite(milestones, "milestone-rail", Vector2.zero, new Vector2(w - 30, 40), order, true);
            string[] keys = { "frame", "badge", "portrait" }; int[] targets = { 500, 1000, 2000 };
            for (int i = 0; i < 3; i++)
            {
                float x = (i - 1) * w * .32f;
                MenuUi.Sprite(milestones, "milestone-" + keys[i], new Vector2(x, 0), Vector2.one * 60, order + 1);
                MenuUi.Label(milestones, Texts.Count(targets[i]), new Vector2(x, 42), 19, order + 2, true);
                MenuUi.Label(milestones, Texts.Milestone(i), new Vector2(x, 65), 17, order + 2, true, w * .31f);
                if (total >= targets[i]) MenuUi.Sprite(milestones, "icon-green-check", new Vector2(x + 22, -20), Vector2.one * 24, order + 3);
            }
            MenuUi.Viewport(root, "jiggles", screen, start, bottom, order + 20);
            Scroll.Set(offset);
        }
        public static void Avatar(Transform root, int index, Vector2 c, float size, int order, bool selected = false)
        {
            MenuUi.Sprite(root, "avatar-disc", c, Vector2.one * size, order);
            MenuUi.Sprite(root, "avatar-" + Colors[index], c, Vector2.one * (size * .88f), order + 1);
            MenuUi.Sprite(root, selected ? "avatar-ring-selected" : "avatar-ring", c, Vector2.one * size, order + 2);
        }
        public int Hit(Vector2 pt)
        {
            if (!Scroll.Contains(pt)) return -1;
            for (int i = 0; i < 5; i++) if (_avatars[i].Contains(pt + new Vector2(0, Scroll.Offset))) return i;
            return -1;
        }
    }
}
