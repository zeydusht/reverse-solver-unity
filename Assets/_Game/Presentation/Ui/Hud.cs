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
                .Stroke(Draw.Rgba(255, 255, 255, .92f), 3).Shadow(Draw.Rgba(0, 0, 0, .35f), 0, 3, 9)
                .transform.localPosition = Draw.W(ax, cy);
            TextView.Create(_root, "lv", "LV", 9, Draw.Hex("#5a3c06"), order: order + 1).transform.localPosition = Draw.W(ax, cy - 9);
            _level = TextView.Create(_root, "lvnum", "1", 20, Draw.Hex("#3a2704"), order: order + 1);
            _level.transform.localPosition = Draw.W(ax, cy + 6);
            LevelRect = new Rect(ax - 26, cy - 26, 52, 52);                // web #lvBtn opens the level list

            float resetX = screenW - Side - 23;
            float clockW = 118, clockX = resetX - 23 - Gap - clockW / 2;
            float pillL = Side + 52 + Gap, pillR = clockX - clockW / 2 - Gap;
            float pillX = (pillL + pillR) / 2, pillW = pillR - pillL;

            Pill(new Vector2(pillX, cy), pillW, order);
            PieceIcon(_root, new Vector2(pillX - 22, cy), order + 2);
            _left = TextView.Create(_root, "left", "0", 22, Draw.Hex("#eaf3f2"), order: order + 2);
            _left.transform.localPosition = Draw.W(pillX + 12, cy);

            Pill(new Vector2(clockX, cy), clockW, order);
            ClockIcon(_root, new Vector2(clockX - 30, cy), order + 2);
            _clock = TextView.Create(_root, "clock", "0:00", 22, Draw.Hex("#eaf3f2"), order: order + 2);
            _clock.transform.localPosition = Draw.W(clockX + 12, cy);

            // .reset: 46px circle, rgba(255,255,255,.10), 2px rgba(255,255,255,.55)
            ShapeView.Create(_root, "reset", new Vector2(46, 46), 4, order)
                .Radius(23).Fill(Draw.Rgba(255, 255, 255, .10f)).Stroke(Draw.Rgba(255, 255, 255, .55f), 2)
                .transform.localPosition = Draw.W(resetX, cy);
            RestartIcon(_root, new Vector2(resetX, cy), order + 1);
            RestartRect = new Rect(resetX - 23, cy - 23, 46, 46);
        }

        void Pill(Vector2 c, float w, int order)
        {
            // .pill: 46px high, radius 23, white .16 -> .05, 2px border rgba(255,255,255,.55), shadow 0 3px 8px .28
            ShapeView.Create(_root, "pill", new Vector2(w, Height), 12, order)
                .Radius(23).Gradient(Draw.Rgba(255, 255, 255, .16f), Draw.Rgba(255, 255, 255, .05f))
                .Stroke(Draw.Rgba(255, 255, 255, .55f), 2).Shadow(Draw.Rgba(0, 0, 0, .28f), 0, 3, 8)
                .transform.localPosition = Draw.W(c);
        }

        /* The strip's amber jigsaw icon (web: a 16pt square with a round notch
           in the middle of every side). The notches are discs in the pill's
           own colour laid over the square. */
        static void PieceIcon(Transform root, Vector2 c, int order)
        {
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
            _clock.Color = t <= 20 ? Draw.Hex("#ffb4a6") : Draw.Hex("#eaf3f2");   // .pill b.warn
        }
    }
}
