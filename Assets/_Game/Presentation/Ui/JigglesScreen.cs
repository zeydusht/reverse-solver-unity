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
            int total = 0;
            for (int i = 0; i < levels.Count; i++) if (progress.Won(i)) total += levels[i].Pieces.Count;
            // PM_MESAJ_27 A4: the total in the banner style, the Jiggle icon on its left; no per-colour counts
            var saved = MenuUi.Banner(root, Texts.SavedTotal(total), Vector2.zero, 27, order + 31, screen.x - 80);
            float tw = saved.Width, ix = screen.x / 2 - (tw + 40) / 2 + 17;
            saved.transform.localPosition = Draw.W(ix + 23 + tw / 2, top + 88);
            var icon = Art.Place(root, "jiggle-counter", new Vector2(38, 38), order + 31) ?? Art.Avatar(root, Art.AvatarIndex, new Vector2(34, 34), order + 31);
            if (icon != null) icon.transform.localPosition = Draw.W(ix, top + 88);
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
            }
            float ay = start + sh + 59;
            var avatar = Scroll.Cell(screen.x / 2, ay, 57);
            MenuUi.Banner(avatar, Texts.MyAvatar, new Vector2(0, -42), 22, order + 2);
            float step = w / 5, size = Mathf.Min(58, step - 5);
            for (int i = 0; i < 5; i++)
            {
                float x = (i - 2) * step;
                Avatar(avatar, i, new Vector2(x, 4), size, order + 1, i == Art.AvatarIndex);
                _avatars[i] = new Rect(screen.x / 2 + x - size / 2, ay + 4 - size / 2, size, size);
            }
            float my = ay + 107;
            var milestones = Scroll.Cell(screen.x / 2, my, 74);
            for (int i = 0; i < 2; i++)
                MenuUi.Sprite21(milestones, "milestone-dots", new Vector2(screen.x * ((127 + i * 120) / 375f - .5f), 0), new Vector2(70 * screen.x / 375, 6), order);
            string[] keys = { "frame", "badge", "portrait" }; int[] targets = { 500, 1000, 2000 };
            for (int i = 0; i < 3; i++)
            {
                float x = screen.x * ((67 + i * 120) / 375f - .5f);
                MenuUi.Sprite21(milestones, "milestone-medal-" + keys[i] + (total < targets[i] ? "-locked" : ""), new Vector2(x, 0), Vector2.one * 52, order + 1);
                MenuUi.Label(milestones, Texts.Count(targets[i]), new Vector2(x, 42), 19, order + 2, true);
                MenuUi.Label(milestones, Texts.Milestone(i), new Vector2(x, 65), 17, order + 2, true, w * .31f);
            }
            MenuUi.Viewport(root, "jiggles", screen, start, bottom, order + 20);
            Scroll.Set(offset);
        }
        public static void Avatar(Transform root, int index, Vector2 c, float size, int order, bool selected = false)
        {
            MenuUi.Sprite(root, "avatar-disc", c, Vector2.one * size, order);
            MenuUi.Sprite21(root, "avatar-" + Colors[index] + "-round", c, Vector2.one * size, order + 1);
            MenuUi.Sprite21(root, selected ? "avatar-ring-selected" : "avatar-ring", c, Vector2.one * size, order + 2);
            if (selected)
                MenuUi.Sprite21(root, "check-badge", c + Vector2.one * (size * .36f), Vector2.one * (size * 48 / 180), order + 3);
        }
        public int Hit(Vector2 pt)
        {
            if (!Scroll.Contains(pt)) return -1;
            for (int i = 0; i < 5; i++) if (_avatars[i].Contains(pt + new Vector2(0, Scroll.Offset))) return i;
            return -1;
        }
    }
}
