using System.Collections.Generic;
using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* Unlocks and stars for the menu and level list. Kept in memory for now;
       persisting it is M5. A level is open once the one before it has been
       won (the first is always open); ?debug=1 opens all of them, as the web
       playtest build let testers jump anywhere. */
    public sealed class Progress
    {
        readonly AttemptLog _log;
        readonly IReadOnlyList<LevelData> _levels;
        public bool UnlockAll { get; set; }

        public Progress(AttemptLog log, IReadOnlyList<LevelData> levels) { _log = log; _levels = levels; }

        public int Stars(int index) => _log.BestStars(_levels[index].Id);
        public bool Won(int index) => Stars(index) > 0;
        public bool Open(int index) => UnlockAll || index == 0 || Won(index - 1);

        /* First open level not yet won, or the last level. */
        public int Next()
        {
            for (int i = 0; i < _levels.Count; i++) if (Open(i) && !Won(i)) return i;
            return _levels.Count - 1;
        }
    }

    public static class Screens
    {
        // ---- main menu ----------------------------------------------------------------

        public sealed class Menu
        {
            public readonly Transform Root;
            public Rect Play, Levels;

            public Menu(Transform parent, Vector2 screen, Vector4 safe, int nextNumber, int order)
            {
                Root = new GameObject("Menu").transform;
                Root.SetParent(parent, false);
                float cx = screen.x / 2, cy = (screen.y + safe.y - safe.w) / 2;

                // the strip's amber jigsaw, large
                var icon = new GameObject("logo").transform;
                icon.SetParent(Root, false);
                icon.localPosition = Draw.W(cx, cy - 150);
                icon.localScale = Vector3.one * 4.5f;
                var fill = Draw.Hex("#f0a742");
                ShapeView.Create(icon, "sq", new Vector2(16, 16), 2, order).Radius(1.2f).Fill(fill)
                    .Shadow(Draw.Rgba(0, 0, 0, .35f), 0, .8f, 2);
                foreach (var o in new[] { new Vector2(0, -8), new Vector2(8, 0), new Vector2(0, 8), new Vector2(-8, 0) })
                    ShapeView.Create(icon, "notch", new Vector2(3.6f, 3.6f), 1, order + 1).Radius(1.8f).Fill(Draw.Hex("#123c42"))
                        .transform.localPosition = Draw.W(o);

                TextView.Create(Root, "title", "Reverse Solver", 34, Draw.Hex("#eaf3f2"), order: order)
                    .transform.localPosition = Draw.W(cx, cy - 64);
                TextView.Create(Root, "sub", "Her parçayı doğru yönde sürükleyip tahtayı boşalt.", 13, Draw.Hex("#8fb2b3"), false, order: order)
                    .transform.localPosition = Draw.W(cx, cy - 30);

                float w = Mathf.Min(296f, screen.x - 80f);
                Play = Button(Root, new Vector2(cx, cy + 30), w, $"Oyna · Bölüm {nextNumber}", true, order);
                Levels = Button(Root, new Vector2(cx, cy + 30 + 46 + 10), w, "Bölümler", false, order);
            }
        }

        static Rect Button(Transform root, Vector2 c, float w, string label, bool primary, int order)
        {
            var b = ShapeView.Create(root, "btn", new Vector2(w, 46), 2, order).Radius(11);
            if (primary) b.Fill(Draw.Hex("#f0a742"));
            else b.Fill(new Vector4(0, 0, 0, 0)).Stroke(Draw.Rgba(255, 255, 255, .22f), 1);
            b.transform.localPosition = Draw.W(c);
            TextView.Create(root, "label", label, 15, primary ? Draw.Hex("#33220a") : Draw.Hex("#8fb2b3"), order: order + 1)
                .transform.localPosition = Draw.W(c);
            return new Rect(c.x - w / 2, c.y - 23, w, 46);
        }

        // ---- level list (web #selVeil) ------------------------------------------------

        public sealed class LevelList
        {
            public readonly Card Card;
            readonly List<(Rect rect, int index)> _cells = new List<(Rect, int)>();
            readonly Progress _progress;

            public LevelList(Transform parent, Vector2 screen, IReadOnlyList<LevelData> levels, Progress progress, int current, int order)
            {
                _progress = progress;
                Card = new Card(parent, screen, order, "LevelList");
                int rows = Mathf.CeilToInt(levels.Count / 5f);
                float gap = 9f;
                float cellW = (Card.InnerWidth - gap * 4) / 5f;
                Card.Title("Bölümler")
                    .Body(progress.UnlockAll ? "Test için istediğin bölüme atla." : "Bir bölümü geçince sonraki açılır.")
                    .Custom(rows * (cellW + gap) - gap + 18, (root, y, w, o) =>
                    {
                        for (int i = 0; i < levels.Count; i++)
                        {
                            int r = i / 5, c = i % 5;
                            var center = new Vector2(-w / 2 + c * (cellW + gap) + cellW / 2, y + r * (cellW + gap) + cellW / 2);
                            bool open = progress.Open(i), done = progress.Won(i), now = i == current;
                            // .sel: radius 11, rgba(255,255,255,.07), 1px rgba(255,255,255,.18)
                            // .sel.done: amber .18 / .5; .sel.now: 2px amber outline
                            var cell = ShapeView.Create(root, "sel", new Vector2(cellW, cellW), 4, o).Radius(11)
                                .Fill(done ? Draw.Rgba(240, 167, 66, .18f) : Draw.Rgba(255, 255, 255, .07f))
                                .Stroke(now ? Draw.Hex("#f0a742") : done ? Draw.Rgba(240, 167, 66, .5f) : Draw.Rgba(255, 255, 255, .18f), now ? 2 : 1);
                            cell.transform.localPosition = Draw.W(center);
                            if (!open) cell.Alpha(.45f);
                            var label = TextView.Create(root, "n", levels[i].Number.ToString(), 15,
                                                        done ? Draw.Hex("#f0a742") : Draw.Hex("#eaf3f2"), order: o + 1);
                            label.transform.localPosition = Draw.W(center + (open ? Vector2.zero : new Vector2(0, -5)));
                            if (!open)
                            {
                                label.Alpha = .45f;
                                var lk = new GameObject("lock").transform;
                                lk.SetParent(root, false);
                                lk.localPosition = Draw.W(center + new Vector2(0, 11));
                                Icons.Draw("lock", lk, 11, o + 1);
                            }
                            _cells.Add((new Rect(center.x - cellW / 2, center.y - cellW / 2, cellW, cellW), i));
                        }
                    })
                    .Button("close", "Kapat", false)
                    .Finish();
                // cell rects are relative to the card's centre line and top
                for (int i = 0; i < _cells.Count; i++)
                {
                    var (r, idx) = _cells[i];
                    _cells[i] = (new Rect(r.x + screen.x / 2, r.y + Card.Top, r.width, r.height), idx);
                }
            }

            /* Level index tapped (open levels only), -1 for nothing, -2 for close. */
            public int Hit(Vector2 pt)
            {
                if (Card.HitButton(pt) == "close") return -2;
                foreach (var (r, i) in _cells) if (r.Contains(pt)) return _progress.Open(i) ? i : -1;
                return -1;
            }
        }

        // ---- result (web #veil / finish()) --------------------------------------------

        public static Card Result(Transform parent, Vector2 screen, GameSession s, bool isLast, int order)
        {
            bool won = s.Outcome == Outcome.Win;
            int stars = s.Stars;
            string title = won ? "Tahta boşaldı" : s.Outcome == Outcome.Bomb ? "Bomba patladı" : "Süre doldu";
            string sub = won ? $"{s.TimeLeft} saniye kaldı, {s.Jams} kez takıldın."
                : s.Outcome == Outcome.Bomb ? "Fitil bitmeden bombanın yolunu açman gerekiyordu. Çekiç onu tek hamlede söker."
                : $"{s.Board.PresentCount} gövde kaldı. Saat booster'ı 20 saniye ekler.";
            string primary = won ? (isLast ? "Tebrikler, 40 bölüm bitti" : "Sonraki bölüm") : "Tekrar dene";
            string primaryId = won ? (isLast ? "menu" : "next") : "retry";

            return new Card(parent, screen, order, "Result")
                .Title(title)
                .Custom(32 + 16, (root, y, w, o) =>
                {
                    // .stars: 32px, gap 9; .off at .2 opacity
                    for (int i = 0; i < 3; i++)
                    {
                        var star = SolidView.Polygon(root, "star", SolidView.Star(15, 6.4f),
                            i < stars ? Draw.Hex("#f0a742") : Draw.Rgba(255, 255, 255, .2f), o);
                        star.transform.localPosition = Draw.W((i - 1) * 41, y + 16);
                    }
                })
                .Body(sub)
                .Button(primaryId, primary, true)
                .Button("retry", "Tekrar oyna", false)
                .Button("menu", "Menü", false)
                .Finish();
        }

        // ---- obstacle / booster introduction (web #introVeil) -----------------------------

        public static Card Intro(Transform parent, Vector2 screen, LevelData l, int order)
        {
            bool booster = l.BoosterIntro != null;
            var info = booster ? l.BoosterIntro : l.Intro;
            string icon = booster ? info.Id : (info.Icon ?? "hammer");
            var c = new Card(parent, screen, order, "Intro")
                .Icon((t, size, o) => Icons.Draw(icon, t, size, o))
                .Title(booster ? "Yeni booster: " + info.Title : info.Title)
                .Body(info.Body ?? "", gapAfter: string.IsNullOrEmpty(info.Tip) ? 18 : 6);
            if (!string.IsNullOrEmpty(info.Tip)) c.Body(info.Tip, Draw.Hex("#f0a742"));
            return c.Button("ok", "Anladım", true).Finish();
        }

        public static bool HasIntro(LevelData l) => l.BoosterIntro != null || l.Intro != null;

        // ---- booster confirmation (web #ask) ----------------------------------------------

        public static Card Ask(Transform parent, Vector2 screen, BoosterControls bar, Booster b, int order)
        {
            var (title, body) = bar.Question(b);
            return new Card(parent, screen, order, "Ask")
                .Icon((t, size, o) => Icons.Draw(b.Id(), t, size, o))
                .Title(title)
                .Body(body)
                .Button("yes", "Kullan", true)
                .Button("no", "Vazgeç", false)
                .Finish();
        }
    }
}
