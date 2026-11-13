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
                        string player = null, int saved = 0, int stars = 0, string playLabel = null, bool hard = false)
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
                    _bg = bg;
                    Task13(screen, safe, nextNumber, order, player, saved, stars, playLabel, hard);
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
                TextView.Create(Root, "sub", Texts.MenuHint, 13, Draw.Hex("#8fb2b3"), false, order: order)
                    .transform.localPosition = Draw.W(cx, cy - 30);

                float w = Mathf.Min(296f, screen.x - 80f);
                Play = Button(Root, new Vector2(cx, cy + 30), w, string.Format(Texts.PlayLevel, nextNumber), true, order);
                Levels = Button(Root, new Vector2(cx, cy + 30 + 46 + 10), w, Texts.Levels, false, order);
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
            public static void AvatarTile(Transform root, Vector2 c, float size, int index, int order, bool frame, bool round = false)
            {
                float inner = size * .8f;
                ShapeView.Create(root, "avatarGround", new Vector2(inner, inner), 2, order).Radius(round ? inner / 2 : inner * .2f)
                    .Radial(Palette.Teal, Vector4.Lerp(Palette.Teal, Palette.TealDark, .5f), Palette.TealDark, new Vector4(.5f, .5f, .75f, .75f))
                    .transform.localPosition = Draw.W(c);
                var a = Art.Avatar(root, index, Vector2.one * (inner * .85f), order + 1);
                if (a != null) a.transform.localPosition = Draw.W(c);
                if (!frame) return;
                var f = Art.Place(root, "m13-profile-frame", new Vector2(size, size), order + 2);
                if (f != null) f.transform.localPosition = Draw.W(c);
            }

            static TMPro.TMP_FontAsset Lilita => Resources.Load<TMPro.TMP_FontAsset>("Fonts/Lilita");

            SpriteRenderer _bg, _painting;

            /* PM_MESAJ_18: a painting inside the corridor's big gold frame. The opening (menu_bg pixels, y down) has
               upright sides and a top and bottom in perspective; the picture is centre-cropped to the opening's
               bounding box and its sprite outline cut to the opening, the frame itself untouched. */
            public void SetPainting(Texture2D tex)
            {
                if (_bg == null || tex == null) return;
                if (_painting != null) Object.Destroy(_painting.gameObject);
                var q = new[] { new Vector2(349, 296), new Vector2(662, 359), new Vector2(663, 832), new Vector2(348, 815) };
                const float x0 = 348, x1 = 663, y0 = 296, y1 = 832;
                float bw = x1 - x0, bh = y1 - y0, s = Mathf.Min(tex.width / bw, tex.height / bh), rw = bw * s, rh = bh * s;
                var sp = UnityEngine.Sprite.Create(tex, new Rect((tex.width - rw) / 2, (tex.height - rh) / 2, rw, rh), new Vector2(.5f, .5f),
                                       s * _bg.sprite.pixelsPerUnit, 0, SpriteMeshType.FullRect);
                var v = new Vector2[4];
                for (int i = 0; i < 4; i++) v[i] = new Vector2(Mathf.Clamp((q[i].x - x0) * s, 0, rw), Mathf.Clamp((y1 - q[i].y) * s, 0, rh));
                sp.OverrideGeometry(v, new ushort[] { 0, 1, 2, 0, 2, 3 });
                _painting = new GameObject("painting").AddComponent<SpriteRenderer>();
                _painting.transform.SetParent(_bg.transform, false);
                _painting.sprite = sp;
                _painting.sortingOrder = _bg.sortingOrder + 1;
                var size = _bg.sprite.rect.size;
                _painting.transform.localPosition = new Vector3((x0 + x1) / 2 - size.x / 2, size.y / 2 - (y0 + y1) / 2) / _bg.sprite.pixelsPerUnit;
            }

            void Task13(Vector2 screen, Vector4 safe, int next, int order, string player, int saved, int stars, string playLabel, bool hard = false)
            {
                float w = screen.x, top = safe.y, hy = top + 34;
                if (Art.On)
                {
                    hy = top + 45;
                    const float avatarSize = 62, overlap = 8, gap = 10, gearSize = 48;
                    const float counterIconSize = 46, counterIconInset = 12;
                    float left = Mathf.Max(8, safe.x), right = Mathf.Max(10, safe.z);
                    var ac = new Vector2(left + avatarSize / 2, hy);
                    // The square frame's opening is about 67% of its size; centre both ground and portrait there.
                    AvatarTile(Root, ac, avatarSize * .84f, Art.AvatarIndex, order + 3, frame: false, round: true);
                    MenuUi.Sprite21(Root, "profile-square-frame", ac, Vector2.one * avatarSize, order + 5);
                    float gx = w - right - gearSize / 2, counterRight = gx - gearSize / 2 - gap;
                    float counterWidth = Mathf.Min(102, w * .272f), counterLeft = counterRight - counterWidth;
                    // Equal visible gaps, including the icon's 11 pt overhang to the left of the pill.
                    float plateLeft = left + avatarSize - overlap;
                    float plateRight = counterLeft - (counterIconSize / 2 - counterIconInset) - gap;
                    float plateWidth = plateRight - plateLeft, plateHeight = 34;
                    MenuUi.Sprite21(Root, "profile-name-plate", new Vector2((plateLeft + plateRight) / 2, hy), new Vector2(plateWidth, plateHeight), order + 2, true);
                    float textLeft = plateLeft + overlap + 8, textWidth = plateRight - 8 - textLeft;
                    var name = TextView.Create(Root, "name", string.IsNullOrEmpty(player) ? Texts.Player : player, 15, Palette.Cream, false,
                        align: TMPro.TextAlignmentOptions.Left, order: order + 3).Banner(TextView.OnTeal);
                    name.GetComponent<TMPro.TextMeshPro>().richText = false;
                    name.GetComponent<TMPro.TextMeshPro>().rectTransform.sizeDelta = new Vector2(textWidth, plateHeight);
                    name.FitWidth(textWidth, 11);
                    name.transform.localPosition = Draw.W(textLeft + textWidth / 2, hy);
                    Profile = new Rect(left, hy - avatarSize / 2, plateRight - left, avatarSize);
                    MenuUi.Sprite21(Root, "counter-pill-v2", new Vector2(counterRight - counterWidth / 2, hy), new Vector2(counterWidth, 30), order + 2, true);
                    var tok = Art.Place(Root, "jiggle-counter", Vector2.one * counterIconSize, order + 3);
                    if (tok != null) tok.transform.localPosition = Draw.W(counterLeft + counterIconInset, hy);
                    MenuUi.Label(Root, saved.ToString(), new Vector2(counterRight - (counterWidth - 38) / 2 - 4, hy), 18, order + 4, true, counterWidth - 42, true);
                    var gear = Sprite("m13-settings-button", new Vector2(gx, hy), Vector2.one * gearSize, order + 2);
                    Settings = new Rect(gx - gearSize / 2, hy - gearSize / 2, gearSize, gearSize);
                    if (gear != null) _press.Add((Settings, gear, "m13-settings-button", "m13-settings-button-pressed", null));
                }
                else
                {
                // header: avatar (tap: profile) + profile box with the name centred, Jiggles counter, settings
                var box = Sprite("m13-profile-box", new Vector2(40 + 66, hy), new Vector2(132, 46), order, sliced: true);
                AvatarTile(Root, new Vector2(40, hy), 60, Art.AvatarIndex, order + 1, frame: true);   // teal ground, centred Jiggle, gold frame
                var name = TextView.Create(Root, "name", string.IsNullOrEmpty(player) ? Texts.Player : player, 15, Palette.Navy, true, order: order + 4);
                name.GetComponent<TMPro.TextMeshPro>().richText = false;
                if (Art.On && name.Width > 84) name.SizePt = 15 * 84 / name.Width;
                else name.FitWidth(84, 9);
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
                var st = TextView.Create(Root, "jiggles", saved.ToString(), 18, Palette.Navy, true, order: order + 2);
                if (Art.On) st.Banner(TextView.OnTeal);
                else st.Font(Resources.Load<TMPro.TMP_FontAsset>("Fonts/Counter"));
                st.transform.localPosition = Draw.W(px + 14, hy);

                }

                // bottom: big Level button and the Levels shortcut, above the safe area
                float bw = Mathf.Min(228f, w - 130f), bh = bw * 112f / 228f;
                // PM_MESAJ_29: 12 pt clear of the selected tab icon that rises over the bar
                float x0 = (w - bw) / 2, cy = screen.y - safe.w - (Art.On ? 28 : 22) - bh / 2;
                var play = Sprite("level-button", new Vector2(x0 + bw / 2, cy), new Vector2(bw, bh), order, sliced: true);
                // PM_MESAJ_42: Baloo heading; a hard level adds "Hard level" under it (40 % / 27 % of the inner height)
                bool second = Art.On && hard && playLabel == null;
                var label = TextView.Create(Root, "play", playLabel ?? string.Format(Texts.Level, next), bh * (second ? .27f : .34f), Palette.Cream, false, order: order + 2);
                if (Art.On) label.Heading(TextView.RimGreen).FitWidth(bw - 40, bh * (second ? .19f : .24f));
                else label.Font(Lilita).Outline(Draw.Hex("#1E5C0E"), .22f);
                label.transform.localPosition = Draw.W(x0 + bw / 2, cy - (second ? bh * .12f : 2));
                if (second)
                {
                    var sub = TextView.Create(Root, "play-hard", Texts.HardTitle, bh * .18f, Palette.Cream, false, order: order + 2)
                        .Heading(TextView.RimGreen).FitWidth(bw - 50, bh * .13f);
                    sub.transform.localPosition = Draw.W(x0 + bw / 2, cy + bh * .17f);
                }
                Play = new Rect(x0, cy - bh / 2, bw, bh);
                if (play != null) _press.Add((Play, play, "level-button", "level-button-pressed", label.transform));

                if (Art.On)
                {
                    var lc = new Vector2(x0 - 29, cy);
                    var levels = Sprite("t20-button-levels", lc, Vector2.one * 44, order);
                    Levels = new Rect(lc.x - 22, lc.y - 22, 44, 44);
                    MenuUi.Label(Root, Texts.Levels, lc + new Vector2(0, 33), 12, order + 2, width: 70);
                    if (levels != null) _press.Add((Levels, levels, "t20-button-levels", "t20-button-levels-pressed", null));
                }

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
                int rows = Mathf.CeilToInt(levels.Count / 5f);
                float gap = 9f;
                float cellW = -1;
                // PM_MESAJ_38 (UX #24): if the card is taller than the screen, the cells shrink and it is built again
                for (int pass = 0; pass < 2; pass++)
                {
                    Card = new Card(parent, screen, order, "LevelList");
                    if (cellW < 0) cellW = (Card.InnerWidth - gap * 4) / 5f;
                    float gridX = -(cellW * 5 + gap * 4) / 2;
                    _cells.Clear();
                    Card.Title(Texts.Levels)
                        .Body(progress.UnlockAll ? Texts.TestLevels : Texts.UnlockHint)
                        .Custom(rows * (cellW + gap) - gap + 18, (root, y, w, o) =>
                        {
                            for (int i = 0; i < levels.Count; i++)
                            {
                                int r = i / 5, c = i % 5;
                                var center = new Vector2(gridX + c * (cellW + gap) + cellW / 2, y + r * (cellW + gap) + cellW / 2);
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
                        .Button("close", Texts.Close, false)
                        .Finish();
                    float over = Card.Height - (screen.y - 32);
                    if (over <= 0 || pass == 1) break;
                    Card.Destroy();
                    cellW = Mathf.Max(30, cellW - over / rows);
                }
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
        public const float PictureTopGap = 10f;   // PM_MESAJ_53: title-frame gap >= 8 pt; taken from the row below, the card keeps its height

        /* askFun (Faz 4 G8, SetFlags.AskFun, FunAsk.ShouldAsk): "Bu bölümü beğendin mi?" with
           two buttons (fun_up / fun_down) above the usual ones, on the won attempt or after the
           third loss; the retry button stays where it is. thanked: the answer was given. */
        public static Card Result(Transform parent, Vector2 screen, GameSession s, bool isLast, int order,
                                  string lastLabel = null, Texture picture = null,
                                  bool askFun = false, bool thanked = false, bool askHard = false, bool hardAnswered = false, bool demo = false, int selectedHard = -1)
        {
            lastLabel ??= string.Format(Texts.SetComplete, 40);
            bool won = s.Outcome == Outcome.Win;
            int stars = s.Stars;
            bool withPicture = ShowsPicture(s, picture);
            string title = withPicture ? Texts.PictureComplete
                : won ? Texts.BoardCleared : s.Outcome == Outcome.Bomb ? Texts.BombExploded : Texts.TimeUp;
            string sub = won ? string.Format(Texts.WinStats, s.TimeLeft, s.Jams)
                : s.Outcome == Outcome.Bomb ? Texts.BombHint
                : string.Format(Texts.TimeHint, s.Board.PresentCount);
            string primary = won ? (isLast ? lastLabel : Texts.NextLevel) : Texts.Retry;
            string primaryId = won ? (isLast ? "menu" : "next") : "retry";

            if (demo && won) title = Texts.Congratulations;   // the banner is short; the demo line goes into the card
            var card = new Card(parent, screen, order, "Result", won ? Art.T23Win : s.Outcome == Outcome.Bomb ? Art.T23LossBomb : Art.T23LossTime)
            {
                Compact = askFun && !thanked || askHard && !hardAnswered,
                Dense = askFun && askHard && !thanked && !hardAnswered
            };
            // preview art (#8, #9): victory / defeat scene when the level has no picture of its own
            if (!card.T23 && !card.Compact && !withPicture && !Art.T14) Picture(card, won ? "victory" : s.Outcome == Outcome.Bomb ? "defeat-bomb" : "defeat-time", 120);
            if (card.T23 && !won) Picture(card, (s.Outcome == Outcome.Bomb ? Art.T23LossBomb : Art.T23LossTime) + "-loss-reason", card.Compact ? 44 : 66);
            else if (!card.Compact && !won && Art.T14) Picture(card, s.Outcome == Outcome.Bomb ? "loss-reason-bomb" : "loss-reason-time", 80);   // Task 14 loss reason
            card.Title(title);
            if (withPicture && !card.Compact)
            {
                // the whole picture, as wide as the card allows and never taller than PictureMaxHeight
                float aspect = (float)picture.width / Mathf.Max(1, picture.height);
                float pw = card.InnerWidth, ph = pw / aspect;
                float pictureMax = card.T23 ? (demo ? 110 : 160) : PictureMaxHeight;
                if (ph > pictureMax) { ph = pictureMax; pw = ph * aspect; }
                card.Custom(PictureTopGap + ph + 12, (root, y, w, o) =>
                {
                    var p = PicturePiece.Create(root, picture, new Vector2(pw, ph), o);
                    p.transform.localPosition = Draw.W(0, y + PictureTopGap + ph / 2);
                });
            }
            if (won) card.Custom(32 + (card.Compact ? 4 : 16), (root, y, w, o) =>
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
                });
            card.Body(sub, gapAfter: card.Compact ? 6 : 18);
            if (demo && won) card.Body(Texts.Demo, null, 16);
            card.Button(demo ? "retry" : primaryId, demo && won ? Texts.PlayAgain : primary, true);
            // ZeydBot: optional 1-10 difficulty (1 çok kolay, 10 çok zor); the row waits for it or for the next button
            if (askHard && !hardAnswered)
                // on a loss too (PM 2026-10-20): Zeyd skipped it after losses, the text says the answer counts
            {
                card.Body(won ? Texts.AskHard : Texts.AskHardLost, sizePt: 13, gapAfter: 8);
                card.ChipRow("hard_", 10, selectedHard);
            }
            if (askHard && hardAnswered) card.Body(Texts.RatingSaved, sizePt: 13, gapAfter: 8);
            if (askFun && !thanked)
            {
                card.Body(Texts.AskFun, sizePt: 13, gapAfter: 8);
                // PM_MESAJ_19: two small drawn thumbs instead of two wide buttons
                if (card.T23 || Art.T15) card.ThumbRow("fun_up", "fun_down");
                else card.ButtonRow(("fun_up", Texts.Liked), ("fun_down", Texts.Disliked));
            }
            if (askFun && thanked) card.Body(Texts.Thanks, sizePt: 13, gapAfter: 8);
            if (demo) card.Button("menu", Texts.MainMenu, false);
            else if (won && (card.T23 || Art.T15)) card.ButtonPair(("retry", Texts.PlayAgain), ("menu", Texts.Menu));
            else
            {
                if (won || !Art.On) card.Button("retry", Texts.PlayAgain, false);
                card.Button("menu", Texts.Menu, false);
            }
            if (askHard && !hardAnswered) card.TextLink("hard_skip", Texts.SkipRating);
            return card.Finish();
        }

        // ---- obstacle / booster introduction (web #introVeil) -----------------------------

        public static Card Intro(Transform parent, Vector2 screen, LevelData l, int order, bool obstacleLesson = false)
        {
            // Hard levels get one opening card; booster training follows through tutorial.json.
            if (l.Hard) return HardWarning(parent, screen, order);
            if (l.BoosterIntro == null && l.Intro == null) return StartCard(parent, screen, l, order);
            bool booster = l.BoosterIntro != null;
            var info = LevelTexts.Get(l, booster);
            string icon = booster ? info.Id : (info.Icon ?? "hammer");
            string scene = !booster && icon == "hammer" ? "nail" : icon;
            if (!booster && (obstacleLesson || P21Obstacle(scene)))
            {
                string title = obstacleLesson && scene == "bomb" ? Lang.T("Bomb piece", "Bombalı parça") : info.Title;
                return ObstacleCard(parent, screen, order, scene, title, Texts.GotIt, l, obstacleLesson);
            }
            if (Art.Get(Art.T23Intro + "-" + scene) != null)
                return IntroScene(parent, screen, order, scene,
                    booster ? string.Format(Texts.NewBooster, info.Title) : info.Title,
                    Texts.TutorialCopy(scene), Texts.GotIt);
            var c = new Card(parent, screen, order, "Intro");
            if (!booster) Picture(c, "intro-" + (icon == "hammer" ? "nail" : icon));   // preview art (#5)
            // the nail card's icon is the hammer in the web data; with the preview art it shows the nail itself
            string glyph = !booster && icon == "hammer" && Art.Get("icon-nail") != null ? "nail" : icon;
            c.Icon((t, size, o) => Icons.Draw(glyph, t, size, o))
                .Title(string.Format(Texts.Level, l.Number))
                .Body(booster ? string.Format(Texts.NewBooster, info.Title) : info.Title, gapAfter: 6)
                .Body(!booster && (icon == "hammer" || icon == "nail") ? Texts.NailBody : info.Body ?? "", gapAfter: string.IsNullOrEmpty(info.Tip) ? 18 : 6);
            if (!string.IsNullOrEmpty(info.Tip)) c.Body(info.Tip, Draw.Hex("#f0a742"));
            if (l.Hard) c.Body(Texts.HardBody);
            return c.Button("ok", Texts.GotIt, true).Finish();
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
            return c.Custom(h + (c.T23 && c.Compact ? 4 : 10), (root, y, cw, o) =>
            {
                var r = Art.Place(root, key, new Vector2(w, h), o + 2);
                r.transform.localPosition = Draw.W(0, y + h / 2);
            });
        }

        // P20-E: the sole list of first-visit start-card slots.
        static readonly int[] IntroSlots = { 1, 3, 5, 6, 9, 10, 14, 15, 19, 23, 25, 30, 35, 36, 38 };
        // PM_MESAJ_29 3.4: a level's own intro / booster / hard flag always shows it; the slot list adds the rest,
        // never on a tutorial level (the guided steps open it)
        public static bool HasIntro(LevelData l, bool allowGeneric = true) => l.BoosterIntro != null || l.Intro != null || l.Hard ||
            allowGeneric && Art.On && l.Tutorial == 0 && System.Array.IndexOf(IntroSlots, l.Number) >= 0;

        /* PM_MESAJ_9 level start card (preview art only): "Level X" ribbon (gold on hard levels), the goal, the
           level's obstacles, the hard warning when flagged, Başla. The timer still starts on the first move. */
        public static Card StartCard(Transform parent, Vector2 screen, LevelData l, int order)
        {
            if (l.Hard) return HardWarning(parent, screen, order);
            if (l.Intro != null || l.BoosterIntro != null) return Intro(parent, screen, l, order);
            string scene = l.Nails.Count > 0 ? "nail" : l.Bombs.Count > 0 ? "bomb" :
                l.CellCount > l.Pieces.Count ? "chain" : l.Sealed != null && l.Sealed.Count > 0 ? "wall" : null;
            if (P21Obstacle(scene))
                return ObstacleCard(parent, screen, order, scene, string.Format(Texts.Level, l.Number), Texts.HardStart, l);
            if (scene != null && Art.Get(Art.T23Intro + "-" + scene) != null)
                return IntroScene(parent, screen, order, scene, string.Format(Texts.Level, l.Number),
                    Texts.TutorialCopy(scene), Texts.HardStart);
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
            if (l.Hard) c.Body(Texts.HardBody, Art.T14 ? Palette.Coral : Draw.Hex("#f0a742"), 13);   // coral reads on the cream card
            return c.Button("ok", Texts.HardStart, true).Finish();
        }

        /* SET40 v3: the sole opening card for a level with the hard flag. The timer
           still starts on the first move, after "Başla". */
        static bool P21Obstacle(string scene) => Art.T54 && TelemetryMeta.P21Line(Art.GameplayCohort) &&
            (scene == "bomb" || scene == "nail" || scene == "chain");

        static Card ObstacleCard(Transform parent, Vector2 screen, int order, string scene, string title, string button,
            LevelData level, bool obstacleLesson = false)
        {
            var card = new Card(parent, screen, order, "Intro", Art.T23Intro);
            if (scene == "bomb")
                card.Custom(112, (root, y, w, o) =>
                    Draw.Child<BombIntroLoop>(root, "Bomb countdown demo").Init(new Vector2(0, y + 56), o + 2));
            else Picture(card, Art.T23Intro + "-" + scene, 144);
            // Use the actual count when all locks share it; differing counters retain the generic N.
            int lockMoves = 0;
            foreach (int count in level.Nails.Values)
            {
                if (lockMoves != 0 && lockMoves != count) { lockMoves = 0; break; }
                lockMoves = count;
            }
            string body = scene == "nail" ? Texts.LockIntro(lockMoves) : Texts.ObstacleIntro(scene);
            if (obstacleLesson && scene == "bomb")
                foreach (int fuse in level.Bombs.Values) { body = Texts.BombLessonIntro(fuse); break; }
            return card.Title(title).Body(body, sizePt: 18, gapAfter: 18).Button("ok", button, true).Finish();
        }

        static Card IntroScene(Transform parent, Vector2 screen, int order, string scene,
                               string title, string body, string button) =>
            Picture(new Card(parent, screen, order, "Intro", Art.T23Intro), Art.T23Intro + "-" + scene, 192)
                .Title(title).Body(body, sizePt: 14, gapAfter: 18, maxLines: 2)
                .Button("ok", button, true).Finish();

        public static Card HardWarning(Transform parent, Vector2 screen, int order) =>
            Art.Get(Art.T23Intro + "-hard") != null
                ? IntroScene(parent, screen, order, "hard", Texts.HardTitle, Texts.HardBody, Texts.HardStart)
                : Picture(new Card(parent, screen, order, "Intro"), "hard")
                .Title(Texts.HardTitle)
                .Body(Texts.HardBody, gapAfter: 18)
                .Button("ok", Texts.HardStart, true).Finish();

        // ---- booster confirmation (web #ask) ----------------------------------------------

        public static Card Ask(Transform parent, Vector2 screen, BoosterControls bar, Booster b, int order)
        {
            var (title, body) = Texts.BoosterQuestion(bar, b);
            return new Card(parent, screen, order, "Ask")
                .Icon((t, size, o) => Icons.Draw(b.Id(), t, size, o))
                .Title(title)
                .Body(body)
                .Button("yes", Texts.Use, true)
                .Button("no", Texts.Cancel, false)
                .Finish();
        }
    }
}
