using ReverseSolver.Core;
using TMPro;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* The web game's result veil and card (index.html #veil, finish()).
       Texts are the web game's. Two buttons: primary (next / retry) and ghost
       (play again). Both are at least 44pt tall. */
    public sealed class EndCard
    {
        public readonly Transform Root;
        public Rect PrimaryRect { get; }
        public Rect SecondaryRect { get; }
        public bool PrimaryIsNext { get; }

        public EndCard(Transform parent, Vector2 screen, GameSession s, bool isLastLevel, int order)
        {
            Root = new GameObject("EndCard").transform;
            Root.SetParent(parent, false);

            // .veil: rgba(8,32,34,.9)
            ShapeView.Create(Root, "veil", screen, 0, order).Radius(0).Fill(Draw.Rgba(8, 32, 34, .9f))
                .transform.localPosition = Draw.W(screen / 2);

            bool won = s.Outcome == Outcome.Win;
            int stars = s.Stars;
            string title = won ? "Tahta boşaldı" : s.Outcome == Outcome.Bomb ? "Bomba patladı" : "Süre doldu";
            string sub = won ? $"{s.TimeLeft} saniye kaldı, {s.Jams} kez takıldın."
                : s.Outcome == Outcome.Bomb ? "Fitil bitmeden bombanın yolunu açman gerekiyordu. Çekiç onu tek hamlede söker."
                : $"{s.Board.PresentCount} gövde kaldı. Saat booster'ı 20 saniye ekler.";
            PrimaryIsNext = won && !isLastLevel;
            string primary = won ? (isLastLevel ? "Tebrikler, 40 bölüm bitti" : "Sonraki bölüm") : "Tekrar dene";

            // .card: max 340 wide, #14484c, 1px rgba(255,255,255,.16), radius 18, padding 24 22 18
            float w = Mathf.Min(340, screen.x - 52), h = 268;
            var c = screen / 2;
            float top = c.y - h / 2;
            ShapeView.Create(Root, "card", new Vector2(w, h), 4, order + 1)
                .Radius(18).Fill(Draw.Hex("#14484c")).Stroke(Draw.Rgba(255, 255, 255, .16f), 1)
                .transform.localPosition = Draw.W(c);

            TextView.Create(Root, "title", title, 20, Draw.Hex("#eaf3f2"), order: order + 2)
                .transform.localPosition = Draw.W(c.x, top + 24 + 12);

            // .stars: 32px, gap 9, off at .2 opacity
            for (int i = 0; i < 3; i++)
            {
                bool on = i < stars;
                var star = SolidView.Polygon(Root, "star", SolidView.Star(15, 6.4f),
                    on ? Draw.Hex("#f0a742") : Draw.Rgba(255, 255, 255, .2f), order + 2);
                star.transform.localPosition = Draw.W(c.x + (i - 1) * 41, top + 24 + 24 + 4 + 16);
            }

            var subView = TextView.Create(Root, "sub", sub, 13, Draw.Hex("#8fb2b3"), false, order: order + 2);
            var tmp = subView.GetComponent<TextMeshPro>();
            tmp.textWrappingMode = TextWrappingModes.Normal;
            tmp.rectTransform.sizeDelta = new Vector2(w - 44, 40);
            tmp.lineSpacing = 10;
            subView.transform.localPosition = Draw.W(c.x, top + 120);

            float bw = w - 44, bh = 46;
            float y1 = top + 160 + bh / 2, y2 = y1 + bh + 8;
            // .btn: amber #f0a742, text #33220a, radius 11; .btn.ghost: transparent, 1px rgba(255,255,255,.22)
            ShapeView.Create(Root, "primary", new Vector2(bw, bh), 2, order + 2).Radius(11).Fill(Draw.Hex("#f0a742"))
                .transform.localPosition = Draw.W(c.x, y1);
            TextView.Create(Root, "primaryText", primary, 15, Draw.Hex("#33220a"), order: order + 3)
                .transform.localPosition = Draw.W(c.x, y1);
            ShapeView.Create(Root, "secondary", new Vector2(bw, bh), 2, order + 2).Radius(11)
                .Fill(new Vector4(0, 0, 0, 0)).Stroke(Draw.Rgba(255, 255, 255, .22f), 1)
                .transform.localPosition = Draw.W(c.x, y2);
            TextView.Create(Root, "secondaryText", "Tekrar oyna", 15, Draw.Hex("#8fb2b3"), order: order + 3)
                .transform.localPosition = Draw.W(c.x, y2);

            PrimaryRect = new Rect(c.x - bw / 2, y1 - bh / 2, bw, bh);
            SecondaryRect = new Rect(c.x - bw / 2, y2 - bh / 2, bw, bh);
        }
    }
}
