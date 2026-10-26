using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* The web game's top strip (index.html .strip): level badge, pieces left,
       clock and the restart button. Layout in screen points, y down. */
    public sealed class Hud
    {
        public const float Height = 46f, TopPad = 10f, Side = 13f, Gap = 9f;

        readonly Transform _root;
        readonly TextView _level, _left, _clock;
        public Rect RestartRect { get; }
        public Rect LevelRect { get; }

        public Hud(Transform parent, float screenW, float top, int order)
        {
            _root = new GameObject("Hud").transform;
            _root.SetParent(parent, false);
            float cy = top + TopPad + Height / 2;

            // .avatar: 52px circle, gradient #ffd873 -> #f0a742, 3px white border, shadow 0 3px 9px .35
            float ax = Side + 26;
            ShapeView.Create(_root, "badge", new Vector2(52, 52), 12, order)
                .Radius(26).Gradient(Draw.Hex("#ffd873"), Draw.Hex("#f0a742"))
                .Stroke(Draw.Rgba(255, 255, 255, .92f), 3).Shadow(Draw.Rgba(0, 0, 0, .35f), 0, 3, 9).Skin("level-badge", false)
                .transform.localPosition = Draw.W(ax, cy);
            TextView.Create(_root, "lv", "LV", 9, Palette.Or(Draw.Hex("#5a3c06"), Palette.TealDark), order: order + 1).transform.localPosition = Draw.W(ax, cy - 9);   // Task 11 badge has an LV area
            _level = TextView.Create(_root, "lvnum", "1", 20, Palette.Or(Draw.Hex("#3a2704"), Palette.Navy), order: order + 1).Font(Art.Lilita);
            _level.transform.localPosition = Draw.W(ax, cy + 6);
            LevelRect = new Rect(ax - 26, cy - 26, 52, 52);                // web #lvBtn opens the level list

            float resetX = screenW - Side - 23;
            float clockW = 118, clockX = resetX - 23 - Gap - clockW / 2;
            float pillL = Side + 52 + Gap, pillR = clockX - clockW / 2 - Gap;
            float pillX = (pillL + pillR) / 2, pillW = pillR - pillL;

            Pill(new Vector2(pillX, cy), pillW, order, "hud-moves-pill");
            // preview: the icon centred in the pill's left end, the number centred in the rest
            // v3 pill: 16 pt ornaments at both ends; the icon sits just inside the left one, the number between the two
            const float end = 12f;   // Task 11 pill: 24 pt rounded ends, the icon sits on the surface just inside
            float pillIcon = Art.On ? pillL + end + 12 : pillX - 22, pillText = Art.On ? (pillL + end + 24 + pillR - end) / 2 : pillX + 12;
            if (Art.T14) { pillIcon = pillL + 20; pillText = (pillL + 40 + pillR) / 2; }   // Task 14 pill: icon baked in its 40 pt left end
            else PieceIcon(_root, new Vector2(pillIcon, cy), order + 2);
            _left = TextView.Create(_root, "left", "0", 22, Art.T14 ? Palette.Navy : Palette.Or(Draw.Hex("#eaf3f2"), Palette.Cream), order: order + 2);
            _left.Font(Art.Lilita);
            _left.transform.localPosition = Draw.W(pillText, cy);

            _clockPill = Pill(new Vector2(clockX, cy), clockW, order, "hud-time-pill");
            float clockL = clockX - clockW / 2;
            if (!Art.T14) ClockIcon(_root, new Vector2(Art.On ? clockL + end + 12 : clockX - 30, cy), order + 2);
            _clock = TextView.Create(_root, "clock", "0:00", Art.On ? 20 : 22, Palette.Or(Draw.Hex("#eaf3f2"), Palette.Cream), order: order + 2);
            _clock.Font(Art.Lilita);
            _clock.transform.localPosition = Draw.W(Art.T14 ? (clockL + 42 + clockX + clockW / 2) / 2 : Art.On ? (clockL + end + 24 + clockX + clockW / 2 - end) / 2 : clockX + 12, cy);

            // .reset: 46px circle, rgba(255,255,255,.10), 2px rgba(255,255,255,.55)
            ShapeView.Create(_root, "reset", new Vector2(46, 46), 4, order)
                .Radius(23).Fill(Draw.Rgba(255, 255, 255, .10f)).Stroke(Draw.Rgba(255, 255, 255, .55f), 2).Skin(Art.On ? "pause-normal" : "reset-normal", false)   // pause-normal: Codex pause button when it arrives
                .transform.localPosition = Draw.W(resetX, cy);
            RestartIcon(_root, new Vector2(resetX, cy), order + 1);
            RestartRect = new Rect(resetX - 23, cy - 23, 46, 46);
        }

        ShapeView _clockPill;

        ShapeView Pill(Vector2 c, float w, int order, string t14Key)
        {
            // .pill: 46px high, radius 23, white .16 -> .05, 2px border rgba(255,255,255,.55), shadow 0 3px 8px .28
            var pill = ShapeView.Create(_root, "pill", new Vector2(w, Height), 12, order)
                .Radius(23).Gradient(Draw.Rgba(255, 255, 255, .16f), Draw.Rgba(255, 255, 255, .05f))
                .Stroke(Draw.Rgba(255, 255, 255, .55f), 2).Shadow(Draw.Rgba(0, 0, 0, .28f), 0, 3, 8).Skin(Art.T14 ? t14Key : "hud-pill-normal");
            pill.transform.localPosition = Draw.W(c);
            return pill;
        }

        /* The strip's amber jigsaw icon (web: a 16pt square with a round notch
           in the middle of every side). The notches are discs in the pill's
           own colour laid over the square. */
        static void PieceIcon(Transform root, Vector2 c, int order)
        {
            if (Art.Icon(root, "icon-piece", c, 16, order)) return;
            var fill = Draw.Hex("#f0a742");
            var notch = Draw.Hex("#3c5c5f");
            ShapeView.Create(root, "pieceIcon", new Vector2(16, 16), 2, order).Radius(1).Fill(fill)
                .transform.localPosition = Draw.W(c);
            foreach (var o in new[] { new Vector2(0, -8), new Vector2(8, 0), new Vector2(0, 8), new Vector2(-8, 0) })
                ShapeView.Create(root, "notch", new Vector2(3.6f, 3.6f), 1, order + 1).Radius(1.8f).Fill(notch)
                    .transform.localPosition = Draw.W(c + o);
        }

        static void ClockIcon(Transform root, Vector2 c, int order)
        {
            if (Art.Icon(root, "hud-icon-clock", c, 16, order)) return;
            var col = Draw.Hex("#ffd489");
            ShapeView.Create(root, "dial", new Vector2(15, 15), 2, order).Radius(7.5f).Ring(col, 1.9f)
                .transform.localPosition = Draw.W(c + new Vector2(0, 1.6f));
            var hand = ShapeView.Create(root, "hand", new Vector2(1.9f, 5), 1, order).Radius(1).Fill(col);
            hand.transform.localPosition = Draw.W(c + new Vector2(0, -0.4f));
            var hand2 = ShapeView.Create(root, "hand2", new Vector2(1.9f, 4), 1, order).Radius(1).Fill(col);
            hand2.transform.localPosition = Draw.W(c + new Vector2(1.3f, 2.6f));
            hand2.transform.localRotation = Quaternion.Euler(0, 0, 54);
            ShapeView.Create(root, "knob", new Vector2(3.6f, 2.6f), 1, order).Radius(1).Fill(col)
                .transform.localPosition = Draw.W(c + new Vector2(0, -8.7f));
        }

        static void RestartIcon(Transform root, Vector2 c, int order)
        {
            if (Art.On && Art.Get("pause-normal") != null) return;   // Codex's pause button carries its own glyph
            if (Art.On)
            {
                // pause glyph until Codex's pause icon: two cream bars on the round button
                foreach (float dx in new[] { -4.5f, 4.5f })
                    ShapeView.Create(root, "pause", new Vector2(5, 16), 1, order).Radius(1.5f).Fill(Palette.Cream)
                        .transform.localPosition = Draw.W(c.x + dx, c.y);
                return;
            }
            if (Art.Icon(root, "icon-reset", c, 17, order)) return;
            var col = Draw.Hex("#eaf3f2");
            // an open circle (gap at the upper right) and an arrow head on its end
            ShapeView.Create(root, "arc", new Vector2(17, 17), 2, order).Radius(8.5f).Ring(col, 2.1f, -1.3f, -0.35f)
                .transform.localPosition = Draw.W(c);
            SolidView.Polygon(root, "head", new[] { new Vector2(0, 0), new Vector2(-5, -2), new Vector2(1, -6) }, col, order)
                .transform.localPosition = Draw.W(c + new Vector2(7.5f, -4.5f));
        }

        public void SetLevel(int number) => _level.Text = number.ToString();
        public void SetLeft(int n) => _left.Text = n.ToString();

        public void SetTime(int seconds)
        {
            int t = Mathf.Max(0, seconds);
            _clock.Text = $"{t / 60}:{t % 60:00}";
            _clock.Color = Art.T14 ? (t <= 20 ? Palette.Cream : Palette.Navy) : t <= 20 ? Palette.Or(Draw.Hex("#ffb4a6"), Palette.Coral) : Palette.Or(Draw.Hex("#eaf3f2"), Palette.Cream);   // .pill b.warn; PALETTE urgent = coral
            _clockPill?.SkinAs(Art.T14 ? (t <= 20 ? "hud-time-pill-urgent" : "hud-time-pill") : t <= 20 ? "hud-pill-hot" : "hud-pill-normal");         // Codex Task 5 urgent pill (Task 7's has the shield slot)
        }
    }
}
