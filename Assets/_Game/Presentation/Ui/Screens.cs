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
            public Rect Play, Levels, Settings, Profile;

            public Menu(Transform parent, Vector2 screen, Vector4 safe, int nextNumber, int order, bool newArt = false,
                        string player = null, int saved = 0, int stars = 0)
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
                    Task13(screen, safe, nextNumber, order, player, saved, stars);
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

            // ---- Codex Task 13 menu (approved 2026-10-26; bottom navigation hidden until its screens exist) ----
            readonly List<(Rect r, SpriteRenderer sr, string normal, string pressed, Transform label)> _press =
                new List<(Rect, SpriteRenderer, string, string, Transform)>();

            /* A pressed look for the control under pt (true when there is one); the caller acts a moment later. */
            public bool Press(Vector2 pt)
            {
                foreach (var p in _press)
                    if (p.r.Contains(pt))
                    {
                        var s = Art.Get(p.pressed);
                        if (s != null) p.sr.sprite = s;
                        if (p.label != null) p.label.localPosition += Draw.W(0, 2);
                        return true;
                    }
                return false;
            }

            SpriteRenderer Sprite(string key, Vector2 c, Vector2 size, int order, bool sliced = false)
            {
                var r = Art.Place(Root, key, size, order, sliced);
                if (r != null) r.transform.localPosition = Draw.W(c);
                return r;
            }

            /* A Jiggle avatar on a teal radial ground (#1C7F8C centre -> #0F4F5A edge) filling the gold frame's opening;
               the Jiggle ~85 % of the opening, centred, clear of the frame. */
            public static void AvatarTile(Transform root, Vector2 c, float size, int index, int order, bool frame)
            {
                float inner = size * .8f;
                ShapeView.Create(root, "avatarGround", new Vector2(inner, inner), 2, order).Radius(inner * .2f)
                    .Radial(Palette.Teal, Vector4.Lerp(Palette.Teal, Palette.TealDark, .5f), Palette.TealDark, new Vector4(.5f, .5f, .75f, .75f))
                    .transform.localPosition = Draw.W(c);
                var a = Art.Avatar(root, index, new Vector2(inner * .85f, inner * .85f), order + 1);
                if (a != null) a.transform.localPosition = Draw.W(c);
                if (!frame) return;
                var f = Art.Place(root, "m13-profile-frame", new Vector2(size, size), order + 2);
                if (f != null) f.transform.localPosition = Draw.W(c);
            }

            static TMPro.TMP_FontAsset Lilita => Resources.Load<TMPro.TMP_FontAsset>("Fonts/Lilita");

            void Task13(Vector2 screen, Vector4 safe, int next, int order, string player, int saved, int stars)
            {
                float w = screen.x, top = safe.y, hy = top + 34;
                // header: avatar (tap: choose a Jiggle) + profile box with the name centred, Jiggles counter, settings
                var box = Sprite("m13-profile-box", new Vector2(40 + 66, hy), new Vector2(132, 46), order, sliced: true);
                AvatarTile(Root, new Vector2(40, hy), 60, Art.AvatarIndex, order + 1, frame: true);   // teal ground, centred Jiggle, gold frame
                var name = TextView.Create(Root, "name", string.IsNullOrEmpty(player) ? "Oyuncu" : player, 15, Palette.Navy, true, order: order + 4);
                name.transform.localPosition = Draw.W(40 + 66 + 12, hy);       // centre of the box's free part (right of the frame)
                Profile = new Rect(10, hy - 30, 164, 60);
                if (box != null) _press.Add((Profile, box, "m13-profile-box", "m13-profile-box-pressed", null));

                float gx = w - 16 - 24;
                var gear = Sprite("m13-settings-button", new Vector2(gx, hy), new Vector2(48, 48), order);
                Settings = new Rect(gx - 24, hy - 24, 48, 48);
                if (gear != null) _press.Add((Settings, gear, "m13-settings-button", "m13-settings-button-pressed", null));

                float pl = 40 + 132 + 10, pr = gx - 24 - 10, pw = Mathf.Min(120, pr - pl), px = (pl + pr) / 2;
                Sprite("m13-counter-pill", new Vector2(px + 8, hy), new Vector2(pw - 10, 34), order, sliced: true);
                var tok = Art.T15 ? Art.Place(Root, "jiggle-counter", new Vector2(40, 40), order + 1)
                                  : Art.Avatar(Root, Art.AvatarIndex, new Vector2(36, 36), order + 1);   // Jiggles counter (PM_MESAJ_16)
                if (tok != null) tok.transform.localPosition = Draw.W(px - pw / 2 + 14, hy);
                var st = TextView.Create(Root, "jiggles", saved.ToString(), 18, Palette.Navy, true, order: order + 2)
                    .Font(Resources.Load<TMPro.TMP_FontAsset>("Fonts/Counter"));
                st.transform.localPosition = Draw.W(px + 14, hy);

                // bottom: big Level button and the Levels shortcut, above the safe area
                float bw = Mathf.Min(228f, w - 130f), bh = bw * 112f / 228f;
                float x0 = (w - bw) / 2, cy = screen.y - safe.w - 22 - bh / 2;   // centred; the Levels shortcut is gone (Zeyd)
                var play = Sprite("level-button", new Vector2(x0 + bw / 2, cy), new Vector2(bw, bh), order, sliced: true);
                var label = TextView.Create(Root, "play", $"Level {next}", bh * .34f, Palette.Cream, false, order: order + 2)
                    .Font(Lilita).Outline(Draw.Hex("#1E5C0E"), .22f);
                label.transform.localPosition = Draw.W(x0 + bw / 2, cy - 2);
                Play = new Rect(x0, cy - bh / 2, bw, bh);
                if (play != null) _press.Add((Play, play, "level-button", "level-button-pressed", label.transform));


                // PM_MESAJ_16: no "Jiggles rescued" bar on the menu
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
            TextView.Create(root, "label", label, font, art != null ? Palette.Navy : primary ? Draw.Hex("#33220a") : Draw.Hex("#8fb2b3"), order: order + 1)
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
                                                        now && Art.On ? Palette.Navy : done ? Draw.Hex("#f0a742") : Draw.Hex("#eaf3f2"), order: o + 1);   // the next cell is cream
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

            var card = new Card(parent, screen, order, "Result") { Compact = askFun && !thanked || askHard && !hardAnswered };
            // preview art (#8, #9): victory / defeat scene when the level has no picture of its own
            if (!withPicture && !Art.T14) Picture(card, won ? "victory" : s.Outcome == Outcome.Bomb ? "defeat-bomb" : "defeat-time", 120);
            if (!won && Art.T14) Picture(card, s.Outcome == Outcome.Bomb ? "loss-reason-bomb" : "loss-reason-time", 80);   // Task 14 loss reason
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
                    .ButtonRow(("fun_up", "Beğendim"), ("fun_down", "Beğenmedim"));
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
            float h = height > 0 ? height : c.InnerWidth * s.bounds.size.y / s.bounds.size.x;
            float w = height > 0 ? Mathf.Min(c.InnerWidth, h * s.bounds.size.x / s.bounds.size.y) : c.InnerWidth;
            return c.Custom(h + 10, (root, y, cw, o) =>
            {
                var r = Art.Place(root, key, new Vector2(w, h), o + 2);
                r.transform.localPosition = Draw.W(0, y + h / 2);
            });
        }

        public static bool HasIntro(LevelData l) => l.BoosterIntro != null || l.Intro != null || l.Hard || Art.On && l.Tutorial == 0;

        /* PM_MESAJ_16 avatar picker: five Jiggles in a row, the chosen one framed; ids av0..av4. */
        public static Card Avatars(Transform parent, Vector2 screen, int order)
        {
            var c = new Card(parent, screen, order, "Avatars").Title(Texts.Avatar);
            c.Custom(76, (root, y, w, o) =>
            {
                float cell = w / 5f;
                for (int i = 0; i < 5; i++)
                {
                    float x = -w / 2 + cell * (i + .5f);
                    Menu.AvatarTile(root, new Vector2(x, y + 34), cell - 6, i, o + 2, frame: false);
                    if (i == Art.AvatarIndex)
                    {
                        var f = Art.Place(root, "m13-profile-frame", new Vector2(cell + 2, cell + 2), o + 5);
                        if (f != null) f.transform.localPosition = Draw.W(x, y + 34);
                    }
                    c.Buttons.Add((new Rect(x - cell / 2, y, cell, 72), "av" + i));
                }
            });
            return c.Button("close", Texts.Close, true).Finish();
        }

        /* PM_MESAJ_9 level start card (preview art only): "Level X" ribbon (gold on hard levels), the goal, the
           level's obstacles, the hard warning when flagged, Başla. The timer still starts on the first move. */
        public static Card StartCard(Transform parent, Vector2 screen, LevelData l, int order)
        {
            var c = new Card(parent, screen, order, "Intro");
            if (Art.T14) c.Title(string.Format(Texts.Level, l.Number));
            else c.Custom(70, (root, y, w, o) =>
            {
                var r = Art.Place(root, l.Hard ? "level-header-hard" : "level-header-normal", new Vector2(w, 60), o + 2, sliced: true);
                if (r != null) r.transform.localPosition = Draw.W(0, y + 30);
                TextView.Create(root, "level", string.Format(Texts.Level, l.Number), 24, Palette.Navy, true, order: o + 3)
                    .transform.localPosition = Draw.W(0, y + 30);
            });
            c.Custom(52, (root, y, w, o) =>
            {
                Art.Icon(root, "goal-jiggle", new Vector2(-w / 2 + 22, y + 22), 40, o + 2);
                TextView.Create(root, "goal", Texts.Goal, 16, Card.Ink(Draw.Hex("#eaf3f2"), "#0B2A36"), true, TMPro.TextAlignmentOptions.Left, o + 2)
                    .transform.localPosition = Draw.W(-w / 2 + 52, y + 22);
            });
            var obstacles = new List<string>();
            if (l.Nails.Count > 0) obstacles.Add("nail");
            if (l.Bombs.Count > 0) obstacles.Add("bomb");
            if (l.CellCount > l.Pieces.Count) obstacles.Add("chain");
            if (l.Sealed != null && l.Sealed.Count > 0) obstacles.Add("wall");
            // Zeyd 2026-10-26: obstacle icons twice as big (80 pt, smaller only when four do not fit)
            float isz = obstacles.Count == 0 ? 0 : Mathf.Min(80f, (c.InnerWidth - 8 * (obstacles.Count - 1)) / obstacles.Count);
            if (obstacles.Count > 0)
                c.Custom(isz + 8, (root, y, w, o) =>
                {
                    for (int i = 0; i < obstacles.Count; i++)
                        Art.Icon(root, "icon-" + obstacles[i], new Vector2((i - (obstacles.Count - 1) / 2f) * (isz + 8), y + isz / 2), isz, o + 2);
                });
            if (l.Hard) c.Body(Texts.HardBody, Art.T14 ? Palette.Coral : Draw.Hex("#f0a742"), Art.T14 ? 12 : 13);   // coral reads on the cream card
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
