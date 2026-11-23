using System;
using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    // The same short result surface owns the chest; no intermediate celebration.
    public sealed class P21WinView
    {
        const int Order = 5600;
        readonly Transform _root;
        readonly SaveData _save;
        readonly int _rescued;
        int _winCoins;
        readonly Action _next, _refresh, _menu;
        Transform _content;
        Vector2 _screen;
        Rect[] _targets;
        float _scale, _top, _started, _tapAt = -10;
        int _chest, _taps, _language = -1, _shownReward = -1;
        Booster _booster;
        bool _chestMode, _done;
        SpriteRenderer _case, _light, _hand, _item, _glow, _trail, _arrival;
        TextView _rescuedLabel;
        TextView _hint, _amount, _coins;
        Rect _nextRect, _menuRect, _chestRect, _doubleRect;
        /* PM_81 R2: "Watch an ad, 2x gold" beside +50 when an ad is ready (GameRoot sets these before the view). */
        public static Func<bool> DoubleOffer;
        public static Action<Action<bool>> DoubleWatch;
        bool _double, _doubled, _doubleBusy;
        Transform _doubleButton;

        /* PM_83: the sl100 economy (GameRoot sets these before the view): the chest of this level (null: none), opened
           in this screen, the hard level bonus, the level's number (the chest bar); Sl83 keeps the old chest away. */
        public static ChestReward Chest83;
        public static int HardBonus83, Level83;
        public static bool Sl83;
        readonly ChestReward _c83;
        readonly int _hardBonus, _level83;
        readonly bool _sl83;

        public P21WinView(Transform parent, Vector2 screen, Rect[] targets, SaveData save, int rescued, int winCoins,
            bool chest, Action next, Action refresh, Action menu)
        {
            _root = new GameObject("P21 win and chest").transform; _root.SetParent(parent, false);
            _save = save; _rescued = rescued; _winCoins = winCoins; _next = next; _refresh = refresh; _menu = menu;
            _started = Time.unscaledTime;
            _sl83 = Sl83; _c83 = Sl83 && B ? Chest83 : null; _hardBonus = Sl83 ? HardBonus83 : 0; _level83 = Level83;
            if (_c83 != null && _c83.Kind == ChestKind.None) _c83 = null;
            Chest83 = null; HardBonus83 = 0; Sl83 = false;
            if (_sl83) chest = false;
            if (chest) BeginChest();
            Layout(screen, targets);
        }
        Vector2 At(float x, float y) => new Vector2(_screen.x * .5f + (x - 195) * _scale, _top + y * _scale);
        bool BChest => _chestMode && B;
        float CoinX => BChest ? 195 : 290;
        float CoinY => BChest ? 168 : 52;
        SpriteRenderer Layer(string name, int order) { var r = Draw.Child<SpriteRenderer>(_content, name); r.sortingOrder = order; return r; }
        void Paint(SpriteRenderer r, string key, Vector2 point, Vector2 size, float alpha = 1)
        {
            r.sprite = key == null ? null : Art.Get("win21/" + key);
            r.enabled = r.sprite != null && alpha > 0;
            if (!r.enabled) return;
            r.transform.localPosition = Draw.W(point);
            float k = Mathf.Min(size.x / r.sprite.bounds.size.x, size.y / r.sprite.bounds.size.y);
            r.transform.localScale = Vector3.one * k; r.color = new Color(1, 1, 1, alpha);
        }
        SpriteRenderer Image(string key, float x, float y, float w, float h, int layer = 1)
        {
            var r = Layer(key, Order + layer); Paint(r, key, At(x, y), new Vector2(w, h) * _scale); return r;
        }
        TextView Label(string text, float x, float y, float size, int layer = 10)
        {
            var t = TextView.Create(_content, "label", text, size * _scale, Palette.Cream, order: Order + layer);
            t.transform.localPosition = Draw.W(At(x, y)); return t;
        }
        public void Layout(Vector2 screen, Rect[] targets)
        {
            _screen = screen; _targets = targets; _scale = Mathf.Min(screen.x / 390, screen.y / 780);
            _top = (screen.y - 760 * _scale) * .5f;
            if (_content != null) UnityEngine.Object.Destroy(_content.gameObject);
            _content = new GameObject("surface").transform; _content.SetParent(_root, false);
            _bases.Clear(); _bGlow = _bTitle = _bJigo = _bCoin = _bNewSlot = null; _bPlus = null; _bBar = _bButtons = null; _bSettled = false;
            ShapeView.Create(_content, "shade", screen, 0, Order).Fill(B ? Draw.Rgba(8, 14, 40, .72f) : Draw.Rgba(10, 18, 46, .88f)).transform.localPosition = Draw.W(screen * .5f);
            if (!_chestMode && B) { LayoutB(); _language = Lang.Revision; Tick(); return; }
            if (_chestMode && B)
            {
                // PM_73: the chest in the T50 B language: no card, the blue light, a 3D title, cream text on blue.
                Glow();
                Label(Texts.Chest21, 195, 105, 52, 4).Title3D();
                _hint = Label("", 195, 560, 22).Heading(Draw.Hex("#153E75"));
                _amount = Label("", 195, 458, 32, 16).Title3D();
            }
            else
            {
            Image("Victory/panel", 195, 380, 370, 740);
            Image("Titles/light", 195, 105, 340, 125, 2);
            // Live Baloo lettering: gold extrusion behind the blue rim and cream face.
            Label(_chestMode ? Texts.Chest21 : Texts.Win21, 195, 112, 48, 3).Heading(Draw.Hex("#D79A26"));
            Label(_chestMode ? Texts.Chest21 : Texts.Win21, 195, 105, 48, 4).Heading(Draw.Hex("#153E75"));
            _hint = Label("", 195, 575, 22);
            _amount = Label("", 195, 458, 30, 16).Heading(TextView.RimTeal);
            }
            _case = Layer("chest", Order + 5); _light = Layer("light", Order + 6); _hand = Layer("hand", Order + 7);
            _glow = Layer("reward glow", Order + 12); _item = Layer("reward", Order + 14);
            _trail = Layer("flight trail", Order + 13); _arrival = Layer("arrival", Order + 15);
            _nextRect = new Rect(At(75, 595), new Vector2(240, 70) * _scale);
            _menuRect = new Rect(At(120, 680), new Vector2(150, 44) * _scale);
            _chestRect = new Rect(At(40, 205), new Vector2(310, 340) * _scale);
            if (!_chestMode)
            {
                Image("Victory/portrait-glow", 195, 285, 260, 250, 2);
                Image("Jigo/mutlu/03", 195, 280, 235, 235, 3);
                Image("Victory/Progress/panel", 195, 455, 325, 110, 2);
                int progress = _save.ChestProgress21, pending = _save.PendingChest21;
                int next = pending > 0 ? pending : P21ChestRules.Next(progress), previous = P21ChestRules.Previous(next - 1);
                int slots = next - previous;
                for (int i = 0; i < slots; i++) Image("Victory/Progress/slot-" + (previous + i < progress ? "complete" : "empty"), 72 + i * 43, 435, 39, 39, 3);
                Image("Victory/Progress/chest", 315, 435, 58, 55, 3);
                Label(pending > 0 ? Texts.Chest21 : Texts.ChestIn21(next - progress), 195, 481, 18);
                var icon = Art.Place(_content, "jiggle-counter", Vector2.one * (42 * _scale), Order + 5)
                    ?? Art.Place(_content, "avatar-blue", Vector2.one * (42 * _scale), Order + 5);
                if (icon != null) icon.transform.localPosition = Draw.W(At(195, 522));
                _rescuedLabel = Label(Texts.Rescued21(0), 195, 562, 24).Heading(TextView.RimTeal);
                NextButton();
            }
            else
            {
                _rescuedLabel = null;
                // PM_74: the coin counter under the title, clear of the HUD's Time box (it sat on it at y 52)
                Image("Reward/Flight/coin-counter", CoinX, CoinY, 116, 37, 8);
                Image("Reward/Flight/coin", CoinX - 39, CoinY, 30, 30, 9);
                _coins = Label(_save.Coins.ToString(), CoinX + 9, CoinY, 18);
                if (_done) NextButton();
                foreach (var b in BoosterExt.All)
                {
                    var r = Layer("destination", Order + 8);
                    Paint(r, "Reward/Reveal/Items/" + b.Id(), _targets[(int)b].center, _targets[(int)b].size * .8f);
                }
            }
            _language = Lang.Revision; Tick();
        }
        /* PM_73: the T50 win screen, option B (docs/art/task50/SECENEKLER.md; 390x844 mock-up coordinates). No card:
           the blue light with gold rays and confetti over the shaded board, the 3D "Great!" lettering, Jigo jumping,
           +50 in the game's font (cream, blue rim) with a coin, the five-slot chest bar, a green Next Level and a small
           blue Menu (labels in the game's font on blank faces, so English works). Entrance: the light opens, the title
           settles from 1.3x, Jigo jumps, the gold counts, the buttons come; a tap finishes it. PM_96: on the fanfare's
           beats (TickB), the rescued Jiggles too. */
        public static bool B => Art.Get(WinArt + "btn_green_big") != null;
        const string WinArt = "T50/win/";
        float _bScale, _bTop;
        SpriteRenderer _bGlow, _bTitle, _bJigo, _bCoin, _bNewSlot;
        TextView _bPlus;
        Transform _bBar, _bButtons;
        Vector2 _bJigoAt;
        bool _bSettled;
        Vector2 AtB(float x, float y) => new Vector2(_screen.x * .5f + (x - 195) * _bScale, _bTop + y * _bScale);
        SpriteRenderer PlaceB(Transform parent, string key, Vector2 at, float w, float h, int layer)
        {
            var r = Art.Place(parent, WinArt + key, new Vector2(w, h) * _bScale, Order + layer);
            if (r != null) r.transform.localPosition = Draw.W(at);
            return r;
        }
        Transform Group(string name)
        {
            var t = new GameObject(name).transform; t.SetParent(_content, false); return t;
        }
        void Glow()
        {
            _bScale = Mathf.Min(_screen.x / 390, _screen.y / 844); _bTop = (_screen.y - 844 * _bScale) * .5f;
            // the light covers the screen (its own rays fade to the edges)
            float cover = Mathf.Max(_screen.x / 390, _screen.y / 844);
            _bGlow = PlaceB(_content, "win_bg_glow", _screen * .5f, 390 * cover / _bScale, 844 * cover / _bScale, 1);
        }
        /* PM_82: with the bigger buttons the coin row, the chest bar and the buttons sit a little higher. */
        static bool BigButtons => Art.Get("win-next") != null;
        static float RowY => BigButtons ? 560 : 578;
        static float BarY => BigButtons ? 622 : 646;
        static float NextY => BigButtons ? 700 : 724;
        static float MenuY => BigButtons ? 778 : 792;
        void LayoutB()
        {
            Glow();
            _bTitle = PlaceB(_content, Lang.T("win_title_en", "win_title_tr"), AtB(195, 158), 320, 102, 4);
            _bJigoAt = AtB(195, 388);
            _bJigo = _c83 == null ? PlaceB(_content, "win_jigo_jump", _bJigoAt, 262, 294, 3) : null;   // PM_83: the chest in Jigo's place
            if (_c83 != null) LayoutChest83();
            LayoutJiggles();   // PM_96: the rescued Jiggles, one per beat
            _beats = null;
            _bHard = null;
            if (_hardBonus > 0)
            {
                // PM_83: a hard level won: +100 more gold under the title
                _bHard = Group("hard bonus");
                var t = TextView.Create(_bHard, "hard bonus", Lang.T("Hard level bonus", "Zor bölüm bonusu") + "  +" + _hardBonus, 20 * _bScale, Palette.Cream, order: Order + 10)
                    .Heading(Draw.Hex("#7A1418"));
                float half83 = (t.Width + 30 * _bScale) * .5f;
                t.transform.localPosition = Draw.W(AtB(195, 232) + new Vector2(-half83 + t.Width * .5f, 0));
                PlaceB(_bHard, "coin", AtB(195, 232) + new Vector2(half83 - 12 * _bScale, 0), 26, 26, 10);
            }
            // +50: the game's font, not baked art (PM_73: the plus sign must read as a plus)
            _bPlus = TextView.Create(_content, "win coins", "+" + _winCoins, 36 * _bScale, Palette.Cream, order: Order + 10).Heading(Draw.Hex("#153E75"));
            _double = !_doubled && _winCoins > 0 && Button75.Available && DoubleOffer != null && DoubleOffer();
            float plusX = _double ? 118 : 195;
            float half = (_bPlus.Width + (6 + 36) * _bScale) * .5f;
            _bPlus.transform.localPosition = Draw.W(AtB(plusX, RowY) + new Vector2(-half + _bPlus.Width * .5f, 0));
            _bCoin = PlaceB(_content, "coin", AtB(plusX, RowY) + new Vector2(half - 18 * _bScale, 0), 36, 36, 9);
            _doubleButton = null;
            if (_double)
            {
                // PM_81 R2: the gold doubled with an ad; Next stays one tap below
                _doubleButton = Group("double gold");
                var b = Button75.Small(_doubleButton, AtB(282, RowY), new Vector2(170, 58) * _bScale, "", Order + 9);
                _doubleRect = b.Rect;
                TextView.Create(_doubleButton, "ad", Lang.T("Watch an ad", "Reklam izle"), 14 * _bScale, Palette.Cream, order: Order + 11)
                    .Heading(Draw.Hex("#0E2244")).FitWidth(140 * _bScale, 9 * _bScale).transform.localPosition = Draw.W(AtB(282, RowY - 11));
                TextView.Create(_doubleButton, "x2", Lang.T("2x gold", "2× altın"), 21 * _bScale, Palette.Cream, order: Order + 11)
                    .Heading(Draw.Hex("#0E2244")).FitWidth(140 * _bScale, 12 * _bScale).transform.localPosition = Draw.W(AtB(282, RowY + 10));
            }
            if (_winCoins <= 0) { _bPlus.gameObject.SetActive(false); if (_bCoin != null) _bCoin.gameObject.SetActive(false); }
            // chest bar: five slots; between two chests (3, 5, 10, 15 ...) the share of the levels won fills them
            _bBar = Group("chest bar");
            var barAt = AtB(195, BarY);
            PlaceB(_bBar, "chest_bar_bg", barAt, 240, 52, 3);
            int progress = _save.ChestProgress21, pending = _save.PendingChest21;
            int next = pending > 0 ? pending : P21ChestRules.Next(progress), previous = P21ChestRules.Previous(next - 1);
            int filled = pending > 0 ? 5 : Mathf.Clamp(Mathf.RoundToInt(5f * (progress - previous) / Mathf.Max(1, next - previous)), 0, 5);
            if (_sl83)
            {
                // PM_83: a chest every 5th level; the bar counts towards it
                int every = Mathf.Max(1, Live.Config.ChestEvery);
                filled = Mathf.Clamp(_level83 % every == 0 ? 5 : _level83 % every * 5 / every, 0, 5);
            }
            _bNewSlot = null;
            for (int i = 0; i < filled; i++)
            {
                var slot = PlaceB(_bBar, "chest_bar_fill", barAt + new Vector2((-91.7f + i * 33.3f) * _bScale, .7f * _bScale), 27, 27, 4);
                if (i == filled - 1) _bNewSlot = slot;
            }
            _bButtons = Group("buttons");
            Buttons(AtB(195, NextY), AtB(195, MenuY), Texts.NextLevel21, _bButtons);
            _bSettled = false;
        }
        /* Green Next Level (300x72) and blue Menu (136x48) with their labels in the game's font. */
        /* PM_75: "Level N+1" on the Next button (GameRoot sets it before the view is built); null: "Next Level". */
        public static string NextTitle;
        Button75.View _pressNext, _pressMenu;   // PM_82: the faces that sink when pressed

        void Buttons(Vector2 next, Vector2 menu, string nextText, Transform parent = null)
        {
            parent = parent != null ? parent : _content;
            if (_bScale <= 0) _bScale = _scale;
            if (Art.Get("win-next") != null && Art.Get("win-menu") != null)
            {
                // PM_82 (Zeyd: small and dull): Codex's option A: a thick green Next ~70 % of the width, 76 pt tall, and a
                // blue Menu 196 x 60, both with a deep bottom edge that sinks when pressed; labels in the game's 3D font
                if (nextText == Texts.NextLevel21 && !string.IsNullOrEmpty(NextTitle)) nextText = NextTitle;
                var mainB = Button75.Custom(parent, next, new Vector2(272, 76) * _bScale, nextText, Order + 9, "win-next", "win-next-pressed", TextView.RimGreen, .40f);
                var smallB = Button75.Custom(parent, menu, new Vector2(196, 60) * _bScale, Lang.T("Menu", "Menü"), Order + 9, "win-menu", "win-menu-pressed", Draw.Hex("#0E2244"), .42f);
                _nextRect = mainB.Rect;
                float menuTop = Mathf.Max(smallB.Rect.yMin, _nextRect.yMax + 2);
                _menuRect = new Rect(smallB.Rect.xMin, menuTop, smallB.Rect.width, Mathf.Max(48f, smallB.Rect.yMax - menuTop));
                _pressNext = mainB; _pressMenu = smallB;
                return;
            }
            if (Button75.Available)
            {
                // PM_75: the same buttons as the menu's "Level N": green for Next, its navy twin for Menu
                if (nextText == Texts.NextLevel21 && !string.IsNullOrEmpty(NextTitle)) nextText = NextTitle;
                next += new Vector2(0, -4 * _bScale);
                var main = Button75.Main(parent, next, new Vector2(262, 92) * _bScale, nextText, Order + 9);
                var small = Button75.Small(parent, menu, new Vector2(150, 50) * _bScale, Lang.T("Menu", "Menü"), Order + 9);
                _nextRect = main.Rect;
                float h = Mathf.Max(small.Rect.height, 48f), w = Mathf.Max(small.Rect.width, 150f);
                float y0 = Mathf.Max(menu.y - h / 2, _nextRect.yMax + 2);
                _menuRect = new Rect(menu.x - w / 2, y0, w, Mathf.Max(44f, menu.y + h / 2 - y0));
                return;
            }
            PlaceB(parent, "btn_green_big", next, 300, 72, 6);
            var n = TextView.Create(parent, "next label", nextText, 28 * _bScale, Palette.Cream, order: Order + 11).Heading(TextView.RimGreen).FitWidth(250 * _bScale, 16 * _bScale);
            n.transform.localPosition = Draw.W(next + new Vector2(0, -2 * _bScale));
            PlaceB(parent, "btn_blue_small", menu, 136, 48, 6);
            var m = TextView.Create(parent, "menu label", Lang.T("Menu", "Menü"), 21 * _bScale, Palette.Cream, order: Order + 11).Heading(Draw.Hex("#153E75"));
            m.transform.localPosition = Draw.W(menu + new Vector2(0, -2 * _bScale));
            _nextRect = new Rect(next - new Vector2(150, 36) * _bScale, new Vector2(300, 72) * _bScale);
            _menuRect = new Rect(menu - new Vector2(68, 24) * _bScale, new Vector2(136, 48) * _bScale);
            // PM_74: the touch area is at least 44 pt tall (and 150 wide), never over the Next button
            float hh = Mathf.Max(_menuRect.height, 48f), ww = Mathf.Max(_menuRect.width, 150f);
            float top = Mathf.Max(menu.y - hh / 2, _nextRect.yMax + 2);
            _menuRect = new Rect(menu.x - ww / 2, top, ww, Mathf.Max(44f, menu.y + hh / 2 - top));
        }
        static float Ease(float t) => 1 - (1 - t) * (1 - t) * (1 - t);
        static float Span(float t, float a, float b) => Mathf.Clamp01((t - a) / (b - a));
        static void Fade(Transform root, float alpha)
        {
            foreach (var r in root.GetComponentsInChildren<SpriteRenderer>(true)) { var c = r.color; c.a = alpha; r.color = c; }
            foreach (var t in root.GetComponentsInChildren<TextView>(true)) t.Alpha = alpha;
        }
        static void Alpha(SpriteRenderer r, float a) { if (r == null) return; var c = r.color; c.a = a; r.color = c; }
        public bool Entering => B && !_chestMode && !_bSettled && !_bSkip;

        // ---- PM_96 §5: the entrance on the fanfare's beats ------------------------------------------------------

        /* GameRoot asks for the fanfare (PlayFanfare); the view starts it (Music.Fanfare) on its second frame, once the
           first one (the heavy one: the screen built, its pictures sent to the GPU) is drawn, so no hitch falls on a beat.
           The screen's clock is then the fanfare's position at the speaker (Music.FanfareTime); without one (no fanfare,
           the audio stopped) the wall clock goes on from where it was. Each event starts Lead before its beat, and a frame more
           (the one still to be shown), so the eye and the ear take them as one:
             beat 1 the light opens · 2 "Great!" lands from 1.3x · then one Jiggle per beat · the strongest beat:
             confetti bursts, Jigo jumps · the last beat: +50 counts, the chest bar and the buttons come (the chest,
             every 5th level, after the fanfare). A tap finishes it all; the fanfare plays on. */
        public static bool PlayFanfare;
        public const float Lead = .015f;
        readonly bool _fanWanted = TakeFanfare();
        readonly int _builtFrame = Time.frameCount;
        static bool TakeFanfare() { bool on = PlayFanfare; PlayFanfare = false; return on; }
        bool _fanOn, _fanAsked, _bSkip, _wallOnly;
        float _audioLast = -100, _audioLastWall, _wallOffset = -.03f, _settledAt;
        float[] _beats;
        int _strongBeat;
        float _fanLength;
        int[] _jiggleBeat;
        SpriteRenderer[] _bJiggles;
        Vector2[] _bJiggleAt;
        Confetti96 _confetti;
        /* The PM_96 measurement: each event's name, the fanfare clock it was first shown at and its beat. */
        public static Action<string, float, float> EventShown;
        readonly System.Collections.Generic.HashSet<string> _shown = new System.Collections.Generic.HashSet<string>();

        float FanfareClock(float now)
        {
            if (_fanWanted && !_fanAsked) return -1;   // the first frame: nothing yet
            float wall = now - _started;
            if (_fanOn && !_wallOnly)
            {
                float a = Music.FanfareTime;
                if (a > -10)
                {
                    if (a != _audioLast || _audioLast < -10) { _audioLast = a; _audioLastWall = wall; return a; }
                    if (wall - _audioLastWall < .25f) return a + (wall - _audioLastWall);   // a coarse clock, carried on
                }
                _wallOnly = true;   // the audio stopped or stands (the page hidden, no audio device): the wall clock from here
                if (_audioLast > -10) _wallOffset = _audioLast - _audioLastWall;
            }
            return wall + _wallOffset;
        }

        /* Beats -> events: 0 the light, 1 the title, the strongest (from the third on) confetti and Jigo, the last the
           gold and the buttons; the beats between the title and the last (but the strongest) take the Jiggles. */
        void PlanBeats()
        {
            var map = _fanOn || _fanWanted && !_fanAsked ? BeatMap.Of(Music.FanfareVariant) : BeatMap.Fallback;
            _beats = map.beats; _fanLength = map.length;
            int n = _beats.Length;
            _strongBeat = map.strong;
            if (_strongBeat < 2 || _strongBeat >= n)
            {
                _strongBeat = n - 1;
                for (int i = 2; i < n; i++)
                    if (map.strength != null && i < map.strength.Length && map.strength[i] > map.strength[_strongBeat]) _strongBeat = i;
            }
            var free = new System.Collections.Generic.List<int>();
            for (int i = 2; i < n - 1; i++) if (i != _strongBeat) free.Add(i);
            int k = _bJiggles == null ? 0 : _bJiggles.Length;
            _jiggleBeat = new int[k];
            for (int j = 0; j < k; j++) _jiggleBeat[j] = free.Count == 0 ? 1 : free[j * free.Count / k];
        }

        /* Up to four of the rescued Jiggles at Jigo's feet, the outer ones first. */
        void LayoutJiggles()
        {
            _bJiggles = null; _bJiggleAt = null;
            if (_c83 != null || _rescued <= 0) return;
            string[] colours = { "blue", "pink", "green", "purple" };
            Vector2[] at = { new Vector2(58, 488), new Vector2(332, 488), new Vector2(112, 514), new Vector2(278, 514) };
            var list = new System.Collections.Generic.List<SpriteRenderer>();
            var points = new System.Collections.Generic.List<Vector2>();
            // one per beat between the title and the last one (but the strongest), at most four
            var map = BeatMap.Of(Music.FanfareVariant);
            int strong = map.strong >= 2 && map.strong < map.beats.Length ? map.strong : map.beats.Length - 1, beats = 0;
            for (int i = 2; i < map.beats.Length - 1; i++) if (i != strong) beats++;
            for (int i = 0; i < Mathf.Min(Mathf.Min(4, _rescued), Mathf.Max(1, beats)); i++)
            {
                var r = Art.Place(_content, "t20-jiggle-" + colours[i] + "-standing", new Vector2(74, 111) * _bScale, Order + 4);
                if (r == null) continue;
                r.transform.localPosition = Draw.W(AtB(at[i].x, at[i].y));
                Alpha(r, 0);
                list.Add(r); points.Add(AtB(at[i].x, at[i].y));
            }
            _bJiggles = list.ToArray(); _bJiggleAt = points.ToArray();
        }

        /* Time since the event of `beat` started (< 0: not yet). */
        float Since(float clock, int beat) => _beats == null || beat < 0 || beat >= _beats.Length ? -1 : clock - (_beats[beat] - Lead - Time.smoothDeltaTime);
        void Shown(string what, int beat, float u, float clock)
        {
            if (u < 0 || _bSkip || !_shown.Add(what)) return;
            EventShown?.Invoke(what, clock, _beats[beat]);
        }

        void TickB(float now)
        {
            if (_c83 != null) TickChest83(now, _bSettled);
            if (_confetti != null && !_confetti.Tick(now)) { _confetti.Destroy(); _confetti = null; }
            if (_bSettled)
            {
                // idle: Jigo bobs softly, the Jiggles hop now and then
                float idle = now - _settledAt;
                if (_bJigo != null) _bJigo.transform.localPosition = Draw.W(_bJigoAt + new Vector2(0, -4 * _bScale * Mathf.Sin(idle * Mathf.PI * 2 / 1.6f)));
                if (_bJiggles != null)
                    for (int i = 0; i < _bJiggles.Length; i++)
                    {
                        float h = Mathf.Repeat(idle + i * .37f, 1.6f);
                        _bJiggles[i].transform.localPosition = Draw.W(_bJiggleAt[i] + new Vector2(0, h < .3f ? -9 * _bScale * Mathf.Sin(h / .3f * Mathf.PI) : 0));
                    }
                return;
            }
            if (_beats == null) PlanBeats();
            float clock = _bSkip ? 1000 : FanfareClock(now);
            int last = _beats.Length - 1;
            // beat 1: the light opens
            float uLight = Since(clock, 0), g = Ease(Mathf.Clamp01(uLight / .3f));
            if (_bGlow != null) { Alpha(_bGlow, uLight < 0 ? 0 : Mathf.Max(.25f, g)); _bGlow.transform.localScale = Vector3.Scale(Base(_bGlow), new Vector3(.9f + .1f * g, .35f + .65f * g, 1)); }
            Shown("light", 0, uLight, clock);
            // Jigo comes up with the light, stands, and jumps on the strongest beat (pushes off on it)
            float uJump = Since(clock, _strongBeat);
            if (_bJigo != null)
            {
                float a = Mathf.Clamp01(uLight / .22f), y = 30 * (1 - Ease(a)), sx = 1, sy = 1;
                if (uJump >= 0)
                {
                    float j = Mathf.Clamp01(uJump / .52f);
                    if (uJump < .06f) { sx = 1.1f; sy = .86f; }
                    else y = -78 * Mathf.Sin(Mathf.PI * j);
                    if (j > .88f && j < 1) { float q = Mathf.Sin((j - .88f) / .12f * Mathf.PI) * .07f; sx = 1 + q; sy = 1 - q; }
                }
                _bJigo.transform.localPosition = Draw.W(_bJigoAt + new Vector2(0, y * _bScale));
                _bJigo.transform.localScale = Vector3.Scale(Base(_bJigo) * Mathf.Lerp(.85f, 1f, Ease(a)), new Vector3(sx, sy, 1));
                Alpha(_bJigo, uLight < 0 ? 0 : Mathf.Max(.25f, a));
            }
            // beat 2: the title lands from 1.3x ("tak")
            float uTitle = Since(clock, 1);
            if (_bTitle != null)
            {
                Alpha(_bTitle, uTitle < 0 ? 0 : 1);
                _bTitle.transform.localScale = Base(_bTitle) * Mathf.LerpUnclamped(1.3f, 1f, PanelMotion76.OutBack(Mathf.Clamp01(uTitle / .16f), 1.6f));
            }
            Shown("title", 1, uTitle, clock);
            if (_bHard != null) Fade(_bHard, uTitle < 0 ? 0 : Mathf.Clamp01(.3f + uTitle / .2f));
            // then one Jiggle per beat, hopping in
            if (_bJiggles != null)
                for (int i = 0; i < _bJiggles.Length; i++)
                {
                    float u = Since(clock, _jiggleBeat[i]), p = Mathf.Clamp01(u / .2f);
                    var r = _bJiggles[i];
                    Alpha(r, u < 0 ? 0 : 1);
                    float pop = u < 0 ? .55f : p < .45f ? Mathf.Lerp(.55f, 1.18f, p / .45f) : Mathf.Lerp(1.18f, 1f, (p - .45f) / .55f);
                    r.transform.localScale = Base(r) * pop;
                    r.transform.localPosition = Draw.W(_bJiggleAt[i] + new Vector2(0, -26 * _bScale * Mathf.Sin(Mathf.PI * p)));
                    Shown("jiggle" + (i + 1), _jiggleBeat[i], u, clock);
                }
            // the strongest beat: confetti
            if (uJump >= 0 && !_bSkip && !_shown.Contains("confetti"))
                _confetti = new Confetti96(_content, _bJigo != null ? _bJigoAt + new Vector2(0, -70 * _bScale) : AtB(195, 330), _bScale, Order + 2, now);
            Shown("confetti", _strongBeat, uJump, clock);
            // the last beat: the gold counts up, the bar comes in, the newest slot pops, the buttons spring up
            float uLast = Since(clock, last), c = Mathf.Clamp01(uLast / .3f);
            if (_bPlus != null)
            {
                _bPlus.Alpha = uLast < 0 ? 0 : 1; _bPlus.Text = "+" + Mathf.RoundToInt(_winCoins * Ease(c));
                _bPlus.transform.localScale = Vector3.one * (uLast < 0 ? 1 : 1 + .18f * Mathf.Sin(Mathf.PI * Mathf.Clamp01(uLast / .22f)));
            }
            Alpha(_bCoin, uLast < 0 ? 0 : 1);
            if (_doubleButton != null && _doubleButton.gameObject.activeSelf) Fade(_doubleButton, Mathf.Clamp01(uLast / .2f));
            if (_bBar != null) Fade(_bBar, Mathf.Clamp01(uLast / .2f));
            if (_bNewSlot != null)
            {
                float p = Mathf.Clamp01((uLast - .12f) / .15f);
                _bNewSlot.transform.localScale = Base(_bNewSlot) * (p <= 0 ? 0 : 1 + .35f * Mathf.Sin(p * Mathf.PI));
            }
            Shown("gold", last, uLast, clock);
            // PM_83: the chest comes after the fanfare
            float uChest = _bSkip ? 1 : clock - Mathf.Max(_beats[last] + .2f, _fanLength - .1f);
            if (_c83Case != null) _c83Case.transform.localScale = Base(_c83Case) * Mathf.Max(0, PanelMotion76.OutBack(Mathf.Clamp01(uChest / .4f), 1.4f));
            float b = Mathf.Clamp01(uLast / .35f);
            if (_bButtons != null && _c83 != null && !_c83Done) Fade(_bButtons, 0);   // PM_83: the buttons wait for the chest
            else if (_bButtons != null) { Fade(_bButtons, uLast < 0 ? 0 : Ease(b)); _bButtons.localPosition = Draw.W(0, 36 * _bScale * (1 - PanelMotion76.OutBack(b, 1.25f))); }
            if (uLast >= .4f && (_c83Case == null || uChest >= .4f))
            {
                _bSettled = true; _settledAt = now;
                if (_bJigo != null) { _bJigo.transform.localScale = Base(_bJigo); Alpha(_bJigo, 1); }
                if (_bTitle != null) { _bTitle.transform.localScale = Base(_bTitle); Alpha(_bTitle, 1); }
                if (_bJiggles != null) for (int i = 0; i < _bJiggles.Length; i++) { _bJiggles[i].transform.localScale = Base(_bJiggles[i]); _bJiggles[i].transform.localPosition = Draw.W(_bJiggleAt[i]); Alpha(_bJiggles[i], 1); }
                if (_c83Case != null) _c83Case.transform.localScale = Base(_c83Case);
                if (_bPlus != null) _bPlus.transform.localScale = Vector3.one;
            }
        }
        Transform _bHard;
        readonly System.Collections.Generic.Dictionary<SpriteRenderer, Vector3> _bases = new System.Collections.Generic.Dictionary<SpriteRenderer, Vector3>();
        Vector3 Base(SpriteRenderer r)
        {
            if (!_bases.TryGetValue(r, out var v)) _bases[r] = v = r.transform.localScale;
            return v;
        }

        void NextButton()
        {
            if (B)
            {
                Buttons(At(195, 609), At(195, 684), _chestMode ? Texts.Collect21 : Texts.NextLevel21);
                return;
            }
            ShapeView.Create(_content, "next green", new Vector2(250, 70) * _scale, 0, Order + 3).Radius(18 * _scale).Fill(Draw.Hex("#398A49")).transform.localPosition = Draw.W(At(195, 630));
            Label(Texts.NextLevel21, 195, 630, 25).Heading(TextView.RimGreen);
            ShapeView.Create(_content, "menu secondary", new Vector2(150, 44) * _scale, 0, Order + 3).Radius(14 * _scale).Fill(Draw.Hex("#274B68")).transform.localPosition = Draw.W(At(195, 702));
            Label(Lang.T("Menu", "Menü"), 195, 702, 20);
        }
        void BeginChest()
        {
            _chestMode = true; _chest = _save.PendingChest21; _taps = 0; _done = false;
            _booster = (Booster)UnityEngine.Random.Range(0, 4); _shownReward = -1;
        }
        public void Press(Vector2 point)
        {
            if (Entering) { _bSkip = true; return; }   // PM_73: a tap finishes the entrance (PM_96: the fanfare plays on)
            if (Time.unscaledTime - _started < .25f) return;
            if (_double && !_doubled && !_doubleBusy && !_chestMode && _doubleRect.Contains(point) && DoubleWatch != null)
            {
                Haptics.Button();   // PM_98 §4: the ad button ticks
                _doubleBusy = true;
                DoubleWatch(ok =>
                {
                    _doubleBusy = false;
                    if (!ok || _root == null) return;
                    _doubled = true;
                    _winCoins *= 2;
                    if (_doubleButton != null) _doubleButton.gameObject.SetActive(false);
                    if (_bPlus != null) _bPlus.Text = "+" + _winCoins;
                    if (_coins != null) _coins.Text = _save.Coins.ToString();
                });
                return;
            }
            if (_pendingAt >= 0) return;
            if (_c83 != null && !_c83Done) { PressChest83(point); return; }   // PM_83: the chest first, then the buttons
            if ((!_chestMode || _done) && _menuRect.Contains(point)) { Sink(_pressMenu, _menu); return; }
            if (!_chestMode)
            {
                if (!_nextRect.Contains(point)) return;
                if (!_sl83 && _save.PendingChest21 > 0) { BeginChest(); Layout(_screen, _targets); }
                else Sink(_pressNext, _next);
                return;
            }
            if (_done) { if (_nextRect.Contains(point)) Sink(_pressNext, _next); return; }
            if (_taps >= 3 || !_chestRect.Contains(point) || Time.unscaledTime - _tapAt < 4f / 14) return;
            _taps++; _tapAt = Time.unscaledTime;
            if (_taps == 3)
            {
                if (_chest > 0 && _save.ClaimChest21(_chest, _booster)) _refresh();
            }
        }
        static string Frame(string path, int frame) => path + "/" + frame.ToString("00");
        /* PM_82: the pressed face shows for a moment, then the action. */
        Action _pending;
        float _pendingAt = -1;
        void Sink(Button75.View b, Action then)
        {
            Haptics.Button();   // PM_86 §4: every button
            if (b == null || b.Body == null) { then(); return; }
            b.Press();
            _pending = then; _pendingAt = Time.unscaledTime + .12f;
        }

        public void Tick()
        {
            if (_fanWanted && !_fanAsked && Time.frameCount > _builtFrame)
            {
                _fanAsked = true; _fanOn = Music.Fanfare(); _started = Time.unscaledTime; _beats = null;
            }
            if (_pendingAt >= 0 && Time.unscaledTime >= _pendingAt)
            {
                _pendingAt = -1;
                var a = _pending; _pending = null;
                a?.Invoke();
                return;
            }
            if (_language != Lang.Revision) { Layout(_screen, _targets); return; }
            float now = Time.unscaledTime, age = now - _tapAt;
            if (!_chestMode && B) { TickB(now); return; }
            if (!_chestMode)
            {
                float countProgress = Mathf.Clamp01((now - _started) / 1.1f);
                _rescuedLabel.Text = Texts.Rescued21(Mathf.RoundToInt(_rescued * countProgress));
                float countPop = countProgress < 1 ? 1 : 1 + .1f * Mathf.Sin(Mathf.Clamp01((now - _started - 1.1f) / .25f) * Mathf.PI);
                _rescuedLabel.transform.localScale = Vector3.one * countPop;
                return;
            }
            string key = "Reward/Chest/closed", light = null;
            if (_taps > 0 && _taps < 3)
            {
                int frame = Mathf.Min(4, 1 + (int)(age * 14));
                key = age < 4f / 14 ? Frame("Reward/Chest/Shake" + _taps, frame) : "Reward/Chest/closed";
                if (_taps == 2) light = Frame("Reward/Chest/Leak", frame);
            }
            if (_taps == 3)
            {
                int frame = Mathf.Min(8, 1 + (int)(age * 12)); key = Frame("Reward/Chest/Open", frame);
                if (frame >= 3) light = Frame("Reward/Chest/OpenLight", frame);
            }
            Paint(_case, key, At(195, 350), new Vector2(310, 268) * _scale);
            Paint(_light, light, At(195, 350), new Vector2(310, 268) * _scale);
            // PM_65: no gloved hand anywhere (PM_60); the wobbling chest and "Tap the chest!" carry the hint.
            Paint(_hand, null, At(252, 460), new Vector2(85, 100) * _scale);
            _hint.Text = _taps < 3 ? Texts.TapChest21 : "";
            _hint.Alpha = .8f + .2f * Mathf.Sin(now * 4);
            if (_taps < 3 || age < .67f || _done) return;
            const float pop = .34f, hold = .55f, fly = .45f, arrival = .34f;
            const float cycle = pop + hold + fly + arrival;
            float rewardAge = age - .67f;
            int reward = (int)(rewardAge / cycle);
            if (reward >= 2)
            {
                Paint(_item, null, default, default); Paint(_glow, null, default, default); Paint(_trail, null, default, default); Paint(_arrival, null, default, default);
                _amount.Text = ""; _done = true;
                NextButton(); return;
            }
            float t = rewardAge - reward * cycle;
            string item = reward == 0 ? "gold" : _booster.Id();
            Vector2 center = At(195, 375), target = reward == 0 ? At(CoinX, CoinY) : _targets[(int)_booster].center;
            Vector2 position = t < pop ? Vector2.Lerp(At(195, 350), center, t / pop) : t < pop + hold ? center : Vector2.Lerp(center, target, Mathf.Clamp01((t - pop - hold) / fly));
            float size = t < pop + hold ? 135 : Mathf.Lerp(135, 32, Mathf.Clamp01((t - pop - hold) / fly));
            Paint(_item, t >= pop + hold + fly ? null : t < pop ? Frame("Reward/Reveal/Pop/" + item, Mathf.Min(4, 1 + (int)(t * 12))) : "Reward/Reveal/Items/" + item, position, Vector2.one * size * _scale);
            Paint(_glow, t < pop + hold ? Frame("Reward/Reveal/Glow", 1 + (int)(t * 8) % 4) : null, center, Vector2.one * 195 * _scale);
            _amount.Text = t < pop + hold ? (reward == 0 ? "+" + P21ChestRules.Coins : "x" + P21ChestRules.BoosterCount) : "";
            Paint(_trail, t >= pop + hold && t < pop + hold + fly ? "Reward/Flight/trail" : null, position, new Vector2(90, 28) * _scale);
            _trail.transform.localRotation = Quaternion.Euler(0, 0, -Mathf.Atan2(target.y - center.y, target.x - center.x) * Mathf.Rad2Deg);
            Paint(_arrival, t >= pop + hold + fly ? Frame("Reward/Flight/arrival", Mathf.Min(4, 1 + (int)((t - pop - hold - fly) * 12))) : null, target, Vector2.one * 80 * _scale);
            if (_shownReward != reward && t >= pop + hold + fly)
            {
                _shownReward = reward; if (reward == 0) _coins.Text = _save.Coins.ToString();
            }
        }
        public void Destroy() { if (_root != null) UnityEngine.Object.Destroy(_root.gameObject); }

        // ---- PM_83: the chest inside the win screen ---------------------------------------------------------------

        /* Tap it three times (it shakes, light leaks, it opens); the gold flies coin by coin to the counter at the top,
           which ticks up, and the boosters to their slots; then Next and Menu come up and the player goes on. */
        SpriteRenderer _c83Case, _c83Light, _c83Seal;
        TextView _c83Hint, _c83Amount, _c83Counter;
        Vector2 _c83At, _c83Size, _c83CounterAt;
        Rect _c83Rect;
        int _c83Taps, _c83Shown;
        float _c83TapAt = -10, _c83OpenAt = -1, _c83DoneAt = -1;
        bool _c83Done, _c83Claimed;
        float _c83Speed = 1, _c83Clock, _c83Last = -1;   // PM_86 §3: a tap while the rewards fly speeds them up
        TextView _c83Tap;                                 // PM_86 §3: "Tap!" at the bottom, on the first chest
        readonly System.Collections.Generic.List<Flight83> _c83Flights = new System.Collections.Generic.List<Flight83>();
        sealed class Flight83 { public SpriteRenderer R, Pop; public Vector2 To; public float At, Fly, Size; public int Coins; public bool Arrived; public string Key; }
        string ChestArt => _c83.Kind == ChestKind.Count ? "Reward/ChestKont" : "Reward/Chest";
        public bool ChestOpen83 => _c83 == null || _c83Done;
        public bool HasChest83 => _c83 != null;
        public const string CoinKey = "yeni/T50/win/coin";

        void LayoutChest83()
        {
            _c83At = AtB(195, 384); _c83Size = new Vector2(280, 242) * _bScale;
            _c83Rect = new Rect(_c83At - _c83Size * .62f, _c83Size * 1.24f);
            _c83Case = Layer("chest83", Order + 5); _c83Light = Layer("chest83 light", Order + 6);
            Paint(_c83Case, _c83Taps >= 3 ? ChestArt + "/Open/08" : ChestArt + "/closed", _c83At, _c83Size);
            _c83Seal = null;
            if (_c83.Kind == ChestKind.Count)
            {
                // the Count's seal over the lock
                _c83Seal = Art.Place(_content, "yeni/T50/map/badge_kont", Vector2.one * 66 * _bScale, Order + 7);
                if (_c83Seal != null) _c83Seal.transform.localPosition = Draw.W(_c83At + new Vector2(0, _c83Size.y * .05f));
            }
            string title = _c83.Kind == ChestKind.Count ? Lang.T("The Count's chest!", "Kont'un sandığı!") : Lang.T("A chest!", "Sandık!");
            var name = TextView.Create(_content, "chest name", title, 22 * _bScale, Palette.Cream, order: Order + 10)
                .Heading(_c83.Kind == ChestKind.Count ? Draw.Hex("#7A1418") : Draw.Hex("#153E75"));
            name.transform.localPosition = Draw.W(AtB(195, _hardBonus > 0 ? 262 : 240));
            _c83Hint = TextView.Create(_content, "chest hint", "", 22 * _bScale, Palette.Cream, order: Order + 10).Heading(Draw.Hex("#153E75"));
            _c83Hint.transform.localPosition = Draw.W(AtB(195, 524));
            if (PlayerPrefs.GetInt("rs_c86_tap", 0) == 0)
            {
                _c83Tap = TextView.Create(_content, "chest tap", Lang.T("Tap!", "Dokun!"), 18 * _bScale, Palette.Cream, order: Order + 10).Heading(Draw.Hex("#153E75"));
                _c83Tap.transform.localPosition = Draw.W(_screen.x / 2, _screen.y - 128 * _bScale);   // above the booster bar
            }
            _c83Amount = TextView.Create(_content, "chest amount", "", 26 * _bScale, Palette.Cream, order: Order + 16).Title3D();
            _c83Amount.transform.localPosition = Draw.W(AtB(195, 524));   // where "Tap the chest!" was, clear of the title
            // the gold counter at the top: the coins fly here
            _c83CounterAt = AtB(195, 64);
            var pill = Art.Place(_content, "yeni/win21/Reward/Flight/coin-counter", new Vector2(132, 42) * _bScale, Order + 8);
            if (pill != null) pill.transform.localPosition = Draw.W(_c83CounterAt);
            var coin = Art.Place(_content, CoinKey, Vector2.one * 40 * _bScale, Order + 9);
            if (coin != null) coin.transform.localPosition = Draw.W(_c83CounterAt + new Vector2(-54 * _bScale, 0));
            _c83Counter = TextView.Create(_content, "chest counter", Shown83().ToString(), 20 * _bScale, Palette.Cream, order: Order + 10);
            _c83Counter.transform.localPosition = Draw.W(_c83CounterAt + new Vector2(10 * _bScale, 0));
        }

        int Shown83() => _c83Claimed && !_c83Done ? _save.Coins - _c83.Coins + _c83Shown : _save.Coins;

        void PressChest83(Vector2 point)
        {
            // PM_86 §3: a tap anywhere counts (light, medium, then the opening: heavy and success); while the rewards
            // fly a tap makes them twice as fast (none is lost: every flight still lands and counts)
            if (_c83Taps >= 3) { if (_c83OpenAt >= 0) { _c83Speed = 2.5f; UiSounds.Request(UiSounds.Kind.Tick); Haptics.Play(Haptics.Kind.Select); } return; }
            if (Time.unscaledTime - _c83TapAt < 4f / 14) return;
            _c83Taps++; _c83TapAt = Time.unscaledTime;
            UiSounds.Request(UiSounds.Kind.Tick);   // PM_98 §4: every chest tap sounds (its taps: light, medium, heavy)
            Haptics.Play(_c83Taps == 1 ? Haptics.Kind.Light : _c83Taps == 2 ? Haptics.Kind.Medium : Haptics.Kind.Heavy);
            if (_c83Taps < 3) return;
            Haptics.PlayLater(Haptics.Kind.Success, .1f);
            PlayerPrefs.SetInt("rs_c86_tap", 1);
            _c83OpenAt = Time.unscaledTime; _c83Clock = 0; _c83Last = Time.unscaledTime;
            _c83Claimed = _save.ClaimChest83(_level83, _c83);
            if (!_c83Claimed) { _c83Done = true; _c83DoneAt = Time.unscaledTime; return; }
            // 5–8 coins, one by one, then the boosters
            int n = Mathf.Clamp(_c83.Coins / 25, 5, 8), left = _c83.Coins;
            for (int i = 0; i < n; i++)
            {
                int share = i == n - 1 ? left : _c83.Coins / n; left -= share;
                _c83Flights.Add(new Flight83 { Key = CoinKey, To = _c83CounterAt + new Vector2(-54 * _bScale, 0), At = .62f + i * .1f, Fly = .5f, Coins = share, Size = 44 });
            }
            float after = .62f + n * .1f + .45f;
            for (int j = 0; j < _c83.Boosters.Length; j++)
            {
                var b = _c83.Boosters[j];
                _c83Flights.Add(new Flight83 { Key = "yeni/win21/Reward/Reveal/Items/" + b.Id(), To = _targets[(int)b].center, At = after + j * .45f, Fly = .6f, Size = 96 });
            }
        }

        void TickChest83(float now, bool settled)
        {
            if (_c83Case == null) return;
            float age = now - _c83TapAt;
            string key = ChestArt + "/closed", light = null;
            if (_c83Taps > 0 && _c83Taps < 3)
            {
                int frame = Mathf.Min(4, 1 + (int)(age * 14));
                key = age < 4f / 14 ? Frame(ChestArt + "/Shake" + _c83Taps, frame) : ChestArt + "/closed";
                if (_c83Taps == 2) light = Frame(ChestArt + "/Leak", frame);
            }
            else if (_c83Taps >= 3)
            {
                int frame = Mathf.Min(8, 1 + (int)(age * 12)); key = Frame(ChestArt + "/Open", frame);
                if (frame >= 3) light = Frame(ChestArt + "/OpenLight", frame);
            }
            Paint(_c83Case, key, _c83At, _c83Size);
            Paint(_c83Light, light, _c83At, _c83Size);
            if (_c83Seal != null) _c83Seal.gameObject.SetActive(_c83Taps < 3);
            if (_c83Hint != null)
            {
                _c83Hint.Text = _c83Taps < 3 ? Lang.T("Tap the chest!", "Sandığa dokun!") : "";
                _c83Hint.Alpha = .75f + .25f * Mathf.Sin(now * 4);
            }
            // idle: the closed chest wobbles a little every 1.6 s
            float w = Mathf.Repeat(now, 1.6f);
            _c83Case.transform.localRotation = _c83Taps == 0 && settled && w < .3f ? Quaternion.Euler(0, 0, 4 * Mathf.Sin(w / .3f * Mathf.PI * 2)) : Quaternion.identity;
            if (_c83Counter != null) _c83Counter.transform.localScale = Vector3.Lerp(_c83Counter.transform.localScale, Vector3.one, .2f);
            if (_c83Tap != null)
            {
                _c83Tap.Alpha = _c83Taps < 3 ? .6f + .4f * Mathf.Sin(now * 5) : 0;
                _c83Tap.transform.localScale = Vector3.one * (1 + .06f * Mathf.Sin(now * 5));
            }
            if (_c83OpenAt < 0) return;
            _c83Clock += (now - _c83Last) * _c83Speed; _c83Last = now;
            float t = _c83Clock;
            if (_c83Amount != null)
            {
                string amount = "+" + _c83.Coins;
                if (_c83.Boosters.Length > 0) amount += "  +" + _c83.Boosters.Length + " booster";
                _c83Amount.Text = _c83Claimed && t > .45f && t < 2.4f + _c83.Boosters.Length * .45f ? amount : "";
                _c83Amount.transform.localScale = Vector3.one * Mathf.Lerp(.6f, 1f, Span(t, .45f, .62f));
            }
            bool all = true;
            foreach (var f in _c83Flights)
            {
                float u = (t - f.At) / f.Fly;
                if (u < 0) { all = false; continue; }
                if (f.R == null && !f.Arrived)
                {
                    f.R = Art.Place(_content, f.Key, Vector2.one * f.Size * _bScale, Order + 18);
                    if (f.R == null) { f.Arrived = true; _c83Shown += f.Coins; continue; }
                }
                if (u < 1)
                {
                    all = false;
                    var from = _c83At + new Vector2(0, -_c83Size.y * .15f);
                    float e = u * u * (3 - 2 * u);
                    var mid = (from + f.To) * .5f + new Vector2((f.Coins > 0 ? -1 : 1) * 70 * _bScale, -60 * _bScale);
                    var p = Vector2.Lerp(Vector2.Lerp(from, mid, e), Vector2.Lerp(mid, f.To, e), e);
                    f.R.transform.localPosition = Draw.W(p);
                    float pop = u < .2f ? Mathf.Lerp(.4f, 1.15f, u / .2f) : Mathf.Lerp(1.15f, f.Coins > 0 ? .8f : .45f, (u - .2f) / .8f);
                    f.R.transform.localScale = Base(f.R) * pop;
                }
                else if (!f.Arrived)
                {
                    f.Arrived = true;
                    if (f.R != null) UnityEngine.Object.Destroy(f.R.gameObject);
                    f.R = null;
                    if (f.Coins > 0)
                    {
                        _c83Shown += f.Coins;
                        if (_c83Counter != null) { _c83Counter.Text = Shown83().ToString(); _c83Counter.transform.localScale = Vector3.one * 1.3f; }
                        Haptics.Play(Haptics.Kind.Light);
                    }
                    else
                    {
                        f.Pop = Art.Place(_content, f.Key, Vector2.one * 70 * _bScale, Order + 18);
                        if (f.Pop != null) f.Pop.transform.localPosition = Draw.W(f.To);
                        _refresh?.Invoke();   // the bar shows the new count
                        Haptics.Play(Haptics.Kind.Medium);
                    }
                }
                if (f.Pop != null)
                {
                    float q = (t - f.At - f.Fly) / .5f;
                    if (q >= 1) { UnityEngine.Object.Destroy(f.Pop.gameObject); f.Pop = null; }
                    else f.Pop.transform.localScale = Base(f.Pop) * (1 + .3f * Mathf.Sin(Mathf.Clamp01(q) * Mathf.PI)) * (1 - .3f * Mathf.Clamp01(q));
                }
            }
            if (all && t > 1.1f && !_c83Done)
            {
                _c83Done = true; _c83DoneAt = now;
                if (_c83Counter != null) _c83Counter.Text = _save.Coins.ToString();
            }
            if (_c83Done && _bButtons != null)
            {
                float k = Span(now - _c83DoneAt, 0, .35f);
                Fade(_bButtons, Ease(k));
                _bButtons.localPosition = Draw.W(0, 36 * _bScale * (1 - PanelMotion76.OutBack(k, 1.25f)));
            }
        }

    }
}
