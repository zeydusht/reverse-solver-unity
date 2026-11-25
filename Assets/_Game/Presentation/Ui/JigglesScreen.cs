using UnityEngine;

namespace ReverseSolver.Presentation
{
    public sealed class JigglesScreen
    {
        public readonly MenuScroll Scroll;
        public static readonly string[] Colors = { "blue", "pink", "red", "green", "purple" };
        readonly Rect[] _avatars = new Rect[5];
        public JigglesScreen(Transform root, Vector2 screen, float top, float bottom, int total, int order, float offset)
        {
            MenuUi.Scene(root, "jiggles", screen, order - 10);
            MenuUi.Title(root, screen, top, Texts.Tab(2), order + 30);
            // PM_MESAJ_27 A4: the total in the banner style, the Jiggle icon on its left; no per-colour counts
            var saved = MenuUi.Banner(root, Texts.SavedTotal(total), Vector2.zero, 27, order + 31, screen.x - 80);
            float tw = saved.Width, ix = screen.x / 2 - (tw + 40) / 2 + 17;
            saved.transform.localPosition = Draw.W(ix + 23 + tw / 2, top + 88);
            var icon = Art.Place(root, "jiggle-counter", new Vector2(38, 38), order + 31) ?? Art.Avatar(root, Art.AvatarIndex, new Vector2(34, 34), order + 31);
            if (icon != null) icon.transform.localPosition = Draw.W(ix, top + 88);
            float start = top + 116, w = Mathf.Min(360, screen.x - 20), sh = w * 816 / 1080;
            float shelfY = start + sh / 2;
            Scroll = new MenuScroll(root, start, bottom, sh + 262);
            var shelf = Scroll.Cell(screen.x / 2, shelfY, sh / 2);
            // PM_89 §3: Codex's navy velvet shelf with gold consoles (option B); the old wooden one stays as the fallback
            var shelfArt = Art.Place(shelf, Key89 + "shelf", new Vector2(w, sh), order);
            if (shelfArt == null) MenuUi.Sprite(shelf, "jiggles-shelf", Vector2.zero, new Vector2(w, sh), order);
            for (int i = 0; i < 5; i++)
            {
                float x = i < 3 ? (i - 1) * w * .27f : (i - 3.5f) * w * .34f;
                // PM_89: on the new shelf the boards' top faces are at .50 and .93 of its height (feet a hair into them)
                float y = sh * (i < 3 ? (shelfArt != null ? .445f : .36f) : (shelfArt != null ? .015f : -.08f));
                MenuUi.Sprite(shelf, "jiggle-" + Colors[i] + "-standing", new Vector2(x, y), new Vector2(w * .22f, sh * .40f), order + 1);
            }
            float ay = start + sh + 59;
            var avatar = Scroll.Cell(screen.x / 2, ay, 57);
            MenuUi.Banner(avatar, Texts.MyAvatar, new Vector2(0, -42), 22, order + 2);
            // PM_89 §3: five equal places across the shelf's width, the chosen one in a gold ring with a soft light
            float step = w / 5, size = Mathf.Min(54, step - 12);
            for (int i = 0; i < 5; i++)
            {
                float x = (i - 2) * step;
                Avatar(avatar, i, new Vector2(x, 4), size, order + 1, i == Art.AvatarIndex);
                _avatars[i] = new Rect(screen.x / 2 + x - step / 2, ay + 4 - step / 2, step, step);   // the whole place answers
            }
            float my = ay + 104;   // PM_MESAJ_33: the bar fits above the tab bar at 375x667
            var bar = Scroll.Cell(screen.x / 2, my, 74);
            ProgressBar89(bar, total, Mathf.Min(w + 6, screen.x - 20), order);
            MenuUi.Viewport(root, "jiggles", screen, start, bottom, order + 20);
            Scroll.Set(offset);
        }
        const string Key89 = "yeni/pm89/jiggles/";

        public static void Avatar(Transform root, int index, Vector2 c, float size, int order, bool selected = false)
        {
            // PM_107 §D6: the ones not chosen sat in the old turquoise disc and ring: a navy disc with a thin gold ring
            bool plain = !selected && MenuUi.T48;
            if (plain)
                ShapeView.Create(root, "avatar-plate", Vector2.one * size, 3, order).Radius(size / 2)
                    .Fill(Draw.Hex("#234670")).Stroke(Draw.Hex("#E3B54E"), 3).transform.localPosition = Draw.W(c);
            else MenuUi.Sprite(root, "avatar-disc", c, Vector2.one * size, order);
            MenuUi.Sprite21(root, "avatar-" + Colors[index] + "-round", c, Vector2.one * size * (plain ? .9f : 1), order + 1);
            // PM_89 §3: the chosen one in Codex's thick gold ring with its soft light (its hole is 144 of 220 px)
            var ring = selected ? Art.Place(root, Key89 + "avatar_ring_gold", Vector2.one * size * 220 / 140, order + 2) : null;
            if (ring != null) ring.transform.localPosition = Draw.W(c);
            else if (!plain) MenuUi.Sprite21(root, selected ? "avatar-ring-selected" : "avatar-ring", c, Vector2.one * size, order + 2);
            if (selected)
                MenuUi.Sprite21(root, "check-badge", c + Vector2.one * (size * .40f), Vector2.one * (size * 44 / 180), order + 3);
        }

        /* PM_89 §3 (Zeyd: the bar at the bottom still very bad): the Achievements screen's bar (a cream groove, a gold
           fill) on a cream card in a royal blue frame, the count over it in the thick 3D lettering; the milestones on the
           bar as rewards: a chest (500), the thank-you gift (1.000, the story: bigger, in a warm glow) and the medal at the
           end, each number under it; not reached yet: greyed with a small lock. */
        static void ProgressBar89(Transform bar, int total, float width, int order)
        {
            int target = total > 2000 ? 5000 : 2000;
            int[] marks = target == 2000 ? new[] { 500, 1000, 2000 } : new[] { 2000, 3500, 5000 };
            string[] icons = { "reward_chest", "reward_gift", "reward_medal" };
            float[] sizes = { 34, 46, 40 };
            var numbers = System.Globalization.CultureInfo.GetCultureInfo("tr-TR");
            var card = Art.Place(bar, "yeni/pm86/basari/pm86-ach-tile", new Vector2(width, 118), order - 1, true);
            if (card != null) card.transform.localPosition = Draw.W(0, 2);
            float bw = width - 56, left = -bw / 2, barY = 8, fill = (bw - 8) * Mathf.Clamp01((float)total / target);
            MenuUi.Banner(bar, $"{total.ToString("N0", numbers)} / {target.ToString("N0", numbers)}", new Vector2(0, -36), 23, order + 4, bw * .7f);
            var track = Art.Place(bar, "yeni/pm86/basari/pm86-ach-track", new Vector2(bw, 22), order, true) ?? Art.Place(bar, "m13-progress-track", new Vector2(bw, 22), order, true);
            if (track != null) track.transform.localPosition = Draw.W(0, barY);
            if (fill > 0)
            {
                fill = Mathf.Max(16, fill);
                var filled = Art.Place(bar, "yeni/pm86/basari/pm86-ach-fill", new Vector2(fill, 14), order + 1, true) ?? Art.Place(bar, "m13-progress-fill", new Vector2(fill, 14), order + 1, true);
                if (filled != null) filled.transform.localPosition = Draw.W(left + 4 + fill / 2, barY);
            }
            for (int i = 0; i < marks.Length; i++)
            {
                float x = left + 4 + (bw - 8) * marks[i] / target;
                bool got = total >= marks[i], story = i == 1;
                var at = new Vector2(x, barY - 1);
                if (story)
                {
                    // the thank-you gift's warm glow, behind it
                    var glow = ShapeView.Create(bar, "gift glow", Vector2.one * 70, 0, order + 1).Radius(35)
                        .Radial(new Vector4(1, .86f, .45f, got ? .75f : .45f), new Vector4(1, .8f, .35f, got ? .3f : .16f), new Vector4(1, .78f, .3f, 0), new Vector4(.5f, .5f, .5f, .5f));
                    glow.transform.localPosition = Draw.W(at);
                }
                var icon = Art.Place(bar, Key89 + icons[i], Vector2.one * sizes[i], order + 2);
                if (icon == null) continue;
                icon.transform.localPosition = Draw.W(at);
                if (!got)
                {
                    icon.color = story ? new Color(.86f, .84f, .8f, 1) : new Color(.62f, .64f, .68f, 1);   // the gift stays warm: it is the one to look forward to
                    var lockIcon = Art.Place(bar, Key89 + "lock_small", Vector2.one * 15, order + 3);
                    if (lockIcon != null) lockIcon.transform.localPosition = Draw.W(at + new Vector2(sizes[i] * .34f, sizes[i] * .30f));
                }
                var label = MenuUi.Label(bar, marks[i].ToString("N0", numbers), new Vector2(Mathf.Min(x, bw / 2 - 6), barY + 27), story ? 15 : 13, order + 3, true, 60);
                label.Color = got || story ? Palette.Navy : Draw.Hex("#5C6475");
            }
        }
        public int Hit(Vector2 pt)
        {
            if (!Scroll.Contains(pt)) return -1;
            for (int i = 0; i < 5; i++) if (_avatars[i].Contains(pt + new Vector2(0, Scroll.Offset))) return i;
            return -1;
        }
    }
}
