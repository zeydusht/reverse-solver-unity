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
            public Rect Play, Levels, Settings;

            public Menu(Transform parent, Vector2 screen, Vector4 safe, int nextNumber, int order, bool newArt = false,
                        string player = null, int saved = 0)
            {
                Root = new GameObject("Menu").transform;
                Root.SetParent(parent, false);
                float cx = screen.x / 2, cy = (screen.y + safe.y - safe.w) / 2;

                // approved corridor art (PM_MESAJ_7), static, cover-fitted under everything; texts stay dynamic
                var art = newArt ? Resources.Load<Sprite>("menu_bg") : null;
                if (art != null)
                {
                    var bg = new GameObject("background").AddComponent<SpriteRenderer>();
                    bg.transform.SetParent(Root, false);
                    bg.sprite = art;
                    bg.sortingOrder = order - 2;
                    float k = Mathf.Max(screen.x / art.rect.width, screen.y / art.rect.height);
                    bg.transform.localScale = new Vector3(k, k, 1);
                    bg.transform.localPosition = Draw.W(screen.x / 2, screen.y / 2);
                    // PM_MESAJ_8: only the corridor and one chunky "Level X" button just above the bottom safe area
                    // (Royal Match style, ~200x72 pt); no title, text, panel or emblem, no level list button
                    const float bw = 200f, bh = 72f;
                    Play = Button(Root, new Vector2(cx, screen.y - safe.w - 28 - bh / 2), bw, $"Level {nextNumber}", true, order, bh, 26);
                    // PM_MESAJ_9: profile box top left (a Jiggle avatar, rescued count), settings gear top right
                    float top = safe.y + 12;
                    var box = ShapeView.Create(Root, "profile", new Vector2(160, 52), 4, order).Radius(16)
                        .Fill(Draw.Rgba(14, 42, 46, .8f)).Skin("profile-normal");
                    box.transform.localPosition = Draw.W(16 + 80, top + 26);
                    Art.Icon(Root, "avatar", new Vector2(16 + 26, top + 26), 44, order + 1);
                    TextView.Create(Root, "name", string.IsNullOrEmpty(player) ? "Oyuncu" : player, 13, Draw.Hex("#fff4da"), true, TMPro.TextAlignmentOptions.Left, order + 1)
                        .transform.localPosition = Draw.W(16 + 54, top + 18);
                    TextView.Create(Root, "saved", string.Format(Texts.Saved, saved), 10, Draw.Hex("#d5e4df"), false, TMPro.TextAlignmentOptions.Left, order + 1)
                        .transform.localPosition = Draw.W(16 + 54, top + 36);
                    float gx = screen.x - 16 - 23;
                    var gear = ShapeView.Create(Root, "settings", new Vector2(46, 46), 4, order).Radius(23)
                        .Fill(Draw.Rgba(14, 42, 46, .8f)).Skin("settings-normal", false);
                    gear.transform.localPosition = Draw.W(gx, top + 26);
                    Settings = new Rect(gx - 23, top + 3, 46, 46);
                    return;
                }

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

        static Rect Button(Transform root, Vector2 c, float w, string label, bool primary, int order, float h = 46, float font = 15)
        {
            var b = ShapeView.Create(root, "btn", new Vector2(w, h), 2, order).Radius(11);
            var art = primary ? Art.Place(root, h > 46 ? "play-normal" : "primary-normal", new Vector2(w, h), order, sliced: true) : null;   // preview art (#3, Task 5)
            if (art != null) { art.transform.localPosition = Draw.W(c); b.Fill(new Vector4(0, 0, 0, 0)); }
            else if (primary) b.Fill(Draw.Hex("#f0a742"));
            else b.Fill(new Vector4(0, 0, 0, 0)).Stroke(Draw.Rgba(255, 255, 255, .22f), 1);
            b.transform.localPosition = Draw.W(c);
            TextView.Create(root, "label", label, font, art != null ? Draw.Hex("#ffffff") : primary ? Draw.Hex("#33220a") : Draw.Hex("#8fb2b3"), order: order + 1)
                .transform.localPosition = Draw.W(c);
            return new Rect(c.x - w / 2, c.y - h / 2, w, h);
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
                                .Stroke(now ? Draw.Hex("#f0a742") : done ? Draw.Rgba(240, 167, 66, .5f) : Draw.Rgba(255, 255, 255, .18f), now ? 2 : 1)
                                .Skin(now ? "level-cell-next" : done ? "level-cell-complete" : open ? "level-cell-open" : "level-cell-locked", false);
                            cell.transform.localPosition = Draw.W(center);
                            if (!open) cell.Alpha(.45f);
                            var label = TextView.Create(root, "n", levels[i].Number.ToString(), 15,
                                                        now && Art.On ? Draw.Hex("#123f46") : done ? Draw.Hex("#f0a742") : Draw.Hex("#eaf3f2"), order: o + 1);   // the next cell is cream
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

        /* lastLabel: the primary button after winning the last level of the set. */
        /* G1-b: the result card shows the whole picture only for a won level
           whose picture really arrived; otherwise the card is as before. */
        public static bool ShowsPicture(GameSession s, Texture picture) =>
            s.Outcome == Outcome.Win && !string.IsNullOrEmpty(s.Level.Image) && picture != null;

        public const float PictureMaxHeight = 190f;
        /* the white rim reaches about 6 pt past the picture: room above it so it never touches the title */
        public const float PictureTopGap = 8f;

        /* askFun (Faz 4 G8, SetFlags.AskFun, FunAsk.ShouldAsk): "Bu bölümü beğendin mi?" with
           two buttons (fun_up / fun_down) above the usual ones, on the won attempt or after the
           third loss; the retry button stays where it is. thanked: the answer was given. */
        public static Card Result(Transform parent, Vector2 screen, GameSession s, bool isLast, int order,
                                  string lastLabel = "Tebrikler, 40 bölüm bitti", Texture picture = null,
                                  bool askFun = false, bool thanked = false, bool askHard = false, bool hardAnswered = false)
        {
            bool won = s.Outcome == Outcome.Win;
            int stars = s.Stars;
            bool withPicture = ShowsPicture(s, picture);
            string title = withPicture ? "Resmi tamamladın"
                : won ? "Tahta boşaldı" : s.Outcome == Outcome.Bomb ? "Bomba patladı" : "Süre doldu";
            string sub = won ? $"{s.TimeLeft} saniye kaldı, {s.Jams} kez takıldın."
                : s.Outcome == Outcome.Bomb ? "Fitil bitmeden bombanın yolunu açman gerekiyordu. Çekiç onu tek hamlede söker."
                : $"{s.Board.PresentCount} gövde kaldı. Saat booster'ı 20 saniye ekler.";
            string primary = won ? (isLast ? lastLabel : "Sonraki bölüm") : "Tekrar dene";
            string primaryId = won ? (isLast ? "menu" : "next") : "retry";

            var card = new Card(parent, screen, order, "Result");
            // preview art (#8, #9): victory / defeat scene when the level has no picture of its own
            if (!withPicture) Picture(card, won ? "victory" : s.Outcome == Outcome.Bomb ? "defeat-bomb" : "defeat-time", 120);
            card.Title(title);
            if (withPicture)
            {
                // the whole picture, as wide as the card allows and never taller than PictureMaxHeight
                float aspect = (float)picture.width / Mathf.Max(1, picture.height);
                float pw = card.InnerWidth, ph = pw / aspect;
                if (ph > PictureMaxHeight) { ph = PictureMaxHeight; pw = ph * aspect; }
                card.Custom(PictureTopGap + ph + 14, (root, y, w, o) =>
                {
                    var p = PicturePiece.Create(root, picture, new Vector2(pw, ph), o);
                    p.transform.localPosition = Draw.W(0, y + PictureTopGap + ph / 2);
                });
            }
            card = card
                .Custom(32 + 16, (root, y, w, o) =>
                {
                    // .stars: 32px, gap 9; .off at .2 opacity
                    for (int i = 0; i < 3; i++)
                    {
                        if (Art.Icon(root, i < stars ? "star-full" : "star-empty", new Vector2((i - 1) * 41, y + 16), 34, o))
                        {
                            if (i < stars) PopStar(root.GetChild(root.childCount - 1), i, o, won);
                            continue;
                        }
                        var star = SolidView.Polygon(root, "star", SolidView.Star(15, 6.4f),
                            i < stars ? Draw.Hex("#f0a742") : Draw.Rgba(255, 255, 255, .2f), o);
                        star.transform.localPosition = Draw.W((i - 1) * 41, y + 16);
                    }
                })
                .Body(sub);
            if (askFun && !thanked)
                card.Body("Bu bölümü beğendin mi?", sizePt: 13, gapAfter: 8)
                    .Button("fun_up", "Beğendim", false)
                    .Button("fun_down", "Beğenmedim", false);
            if (askFun && thanked) card.Body("Teşekkürler!", sizePt: 13, gapAfter: 8);
            // ZeydBot: optional 1-10 difficulty (1 çok kolay, 10 çok zor); the row waits for it or for the next button
            if (askHard && !hardAnswered)
                // on a loss too (PM 2026-10-20): Zeyd skipped it after losses, the text says the answer counts
                card.Body(won ? "Ne kadar zordu? 1 çok kolay · 10 çok zor" : "Kaybettin ama puanın önemli: ne kadar zordu? (1–10)", sizePt: 13, gapAfter: 8)
                    .ButtonRow(("hard_1", "1"), ("hard_2", "2"), ("hard_3", "3"), ("hard_4", "4"), ("hard_5", "5"))
                    .ButtonRow(("hard_6", "6"), ("hard_7", "7"), ("hard_8", "8"), ("hard_9", "9"), ("hard_10", "10"));
            if (askHard && hardAnswered) card.Body("Kaydedildi, teşekkürler!", sizePt: 13, gapAfter: 8);
            // ZeydBot, on a loss (PM 2026-10-21: Z34's bomb row came without the answer, retry was tapped first):
            // the retry buttons wait for the 1-10 answer; Menü stays
            if (askHard && !hardAnswered && !won) return card.Button("menu", "Menü", false).Finish();
            // QA 2026-10-26: on a loss the primary is already "Tekrar dene"; the second retry button only doubled it
            card.Button(primaryId, primary, true);
            if (won || !Art.On) card.Button("retry", "Tekrar oyna", false);
            return card.Button("menu", "Menü", false).Finish();
        }

        // ---- obstacle / booster introduction (web #introVeil) -----------------------------

        public static Card Intro(Transform parent, Vector2 screen, LevelData l, int order)
        {
            if (l.BoosterIntro == null && l.Intro == null) return Art.On ? StartCard(parent, screen, l, order) : HardWarning(parent, screen, order);
            bool booster = l.BoosterIntro != null;
            var info = booster ? l.BoosterIntro : l.Intro;
            string icon = booster ? info.Id : (info.Icon ?? "hammer");
            var c = new Card(parent, screen, order, "Intro");
            if (!booster) Picture(c, "intro-" + (icon == "hammer" ? "nail" : icon));   // preview art (#5)
            // the nail card's icon is the hammer in the web data; with the preview art it shows the nail itself
            string glyph = !booster && icon == "hammer" && Art.Get("icon-nail") != null ? "nail" : icon;
            c.Icon((t, size, o) => Icons.Draw(glyph, t, size, o))
                .Title(booster ? "Yeni booster: " + info.Title : info.Title)
                .Body(info.Body ?? "", gapAfter: string.IsNullOrEmpty(info.Tip) ? 18 : 6);
            if (!string.IsNullOrEmpty(info.Tip)) c.Body(info.Tip, Draw.Hex("#f0a742"));
            return c.Button("ok", "Anladım", true).Finish();
        }

        /* PM_MESAJ_9 win polish: earned stars grow in one after another, with a burst; confetti on a win. */
        static void PopStar(Transform star, int i, int order, bool won)
        {
            var end = star.localScale;
            star.localScale = Vector3.zero;
            float delay = .25f + i * .3f;
            Tweens.Run(star, delay + .35f, t =>
            {
                if (star == null) return;
                float u = Mathf.Clamp01((t * (delay + .35f) - delay) / .35f);
                star.localScale = end * Tweens.Tick(u);
            });
            if (i == 0 && won) Confetti(star.parent, order);
        }

        static void Confetti(Transform root, int order)
        {
            var rng = new System.Random();
            for (int k = 0; k < 24; k++)
            {
                var r = Art.Place(root, "confetti-" + (1 + k % 6), new Vector2(12, 12), order + 5);
                if (r == null) return;
                var p = r.transform;
                Vector3 from = Draw.W((float)(rng.NextDouble() - .5) * 300, -60 - (float)rng.NextDouble() * 80);
                Vector3 to = from + Draw.W((float)(rng.NextDouble() - .5) * 80, 420);
                float spin = (float)(rng.NextDouble() - .5) * 720;
                p.localPosition = from;
                Tweens.Run(p, 1.6f + (float)rng.NextDouble() * .6f, t =>
                {
                    if (p == null) return;
                    p.localPosition = Vector3.Lerp(from, to, t);
                    p.localRotation = Quaternion.Euler(0, 0, spin * t);
                    r.color = new Color(1, 1, 1, 1 - t * t);
                });
            }
        }

        /* A preview-art picture row on top of a card (nothing when the art is off or missing). */
        static Card Picture(Card c, string key, float height = 0)
        {
            var s = Art.Get(key);
            if (s == null) return c;
            float w = c.InnerWidth, h = height > 0 ? height : w * s.bounds.size.y / s.bounds.size.x;
            return c.Custom(h + 10, (root, y, cw, o) =>
            {
                var r = Art.Place(root, key, new Vector2(w, h), o + 2);
                r.transform.localPosition = Draw.W(0, y + h / 2);
            });
        }

        public static bool HasIntro(LevelData l) => l.BoosterIntro != null || l.Intro != null || l.Hard || Art.On && l.Tutorial == 0;

        /* PM_MESAJ_9 level start card (preview art only): "Level X" ribbon (gold on hard levels), the goal, the
           level's obstacles, the hard warning when flagged, Başla. The timer still starts on the first move. */
        public static Card StartCard(Transform parent, Vector2 screen, LevelData l, int order)
        {
            var c = new Card(parent, screen, order, "Intro");
            c.Custom(70, (root, y, w, o) =>
            {
                var r = Art.Place(root, l.Hard ? "level-header-hard" : "level-header-normal", new Vector2(w, 60), o + 2, sliced: true);
                if (r != null) r.transform.localPosition = Draw.W(0, y + 30);
                TextView.Create(root, "level", string.Format(Texts.Level, l.Number), 24, Draw.Hex("#fff4da"), true, order: o + 3)
                    .transform.localPosition = Draw.W(0, y + 30);
            });
            c.Custom(52, (root, y, w, o) =>
            {
                Art.Icon(root, "goal-jiggle", new Vector2(-w / 2 + 22, y + 22), 40, o + 2);
                TextView.Create(root, "goal", Texts.Goal, 16, Card.Ink(Draw.Hex("#eaf3f2"), "#123f46"), true, TMPro.TextAlignmentOptions.Left, o + 2)
                    .transform.localPosition = Draw.W(-w / 2 + 52, y + 22);
            });
            var obstacles = new List<string>();
            if (l.Nails.Count > 0) obstacles.Add("nail");
            if (l.Bombs.Count > 0) obstacles.Add("bomb");
            if (l.CellCount > l.Pieces.Count) obstacles.Add("chain");
            if (l.Sealed != null && l.Sealed.Count > 0) obstacles.Add("wall");
            if (obstacles.Count > 0)
                c.Custom(48, (root, y, w, o) =>
                {
                    for (int i = 0; i < obstacles.Count; i++)
                        Art.Icon(root, "icon-" + obstacles[i], new Vector2((i - (obstacles.Count - 1) / 2f) * 50, y + 20), 40, o + 2);
                });
            if (l.Hard) c.Body(Texts.HardBody, Draw.Hex("#f0a742"));
            return c.Button("ok", Texts.HardStart, true).Finish();
        }

        /* SET40 v3: the "zor bölüm" warning (a level with the hard flag and no other card). The timer
           still starts on the first move, after "Başla". */
        public static Card HardWarning(Transform parent, Vector2 screen, int order) =>
            Picture(new Card(parent, screen, order, "Intro"), "hard")                     // preview art (#6)
                .Icon((t, size, o) => Icons.Draw("bomb", t, size, o))
                .Title(Texts.HardTitle)
                .Body(Texts.HardBody, gapAfter: 18)
                .Button("ok", Texts.HardStart, true).Finish();

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
