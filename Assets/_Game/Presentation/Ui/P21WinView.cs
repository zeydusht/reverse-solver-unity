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

        public P21WinView(Transform parent, Vector2 screen, Rect[] targets, SaveData save, int rescued, int winCoins,
            bool chest, Action next, Action refresh, Action menu)
        {
            _root = new GameObject("P21 win and chest").transform; _root.SetParent(parent, false);
            _save = save; _rescued = rescued; _winCoins = winCoins; _next = next; _refresh = refresh; _menu = menu;
            _started = Time.unscaledTime;
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
           settles from 1.3x, Jigo jumps, the gold counts, the buttons come; 1.15 s in all, a tap finishes it. */
        public static bool B => Art.Get(WinArt + "btn_green_big") != null;
        const string WinArt = "T50/win/";
        // PM_76: the same timeline played in 1.0 s (screen arrivals .6–1.0 s); the title and the buttons spring into place
        public const float EntranceSeconds = 1.0f;
        const float Timeline = 1.15f;
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
            _bJigo = PlaceB(_content, "win_jigo_jump", _bJigoAt, 262, 294, 3);
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
        public bool Entering => B && !_chestMode && Time.unscaledTime - _started < EntranceSeconds;
        void TickB(float now)
        {
            float t = (now - _started) * (Timeline / EntranceSeconds);
            if (_bSettled)
            {
                // idle: Jigo bobs softly
                if (_bJigo != null) _bJigo.transform.localPosition = Draw.W(_bJigoAt + new Vector2(0, -4 * _bScale * Mathf.Sin((t - Timeline) * Mathf.PI * 2 / 1.6f)));
                return;
            }
            // the light opens
            float g = Ease(Span(t, 0, .3f));
            if (_bGlow != null) { Alpha(_bGlow, g); _bGlow.transform.localScale = Vector3.Scale(Base(_bGlow), new Vector3(.9f + .1f * g, .35f + .65f * g, 1)); }
            // the title settles from 1.3x
            float h = Span(t, .12f, .45f);
            if (_bTitle != null) { Alpha(_bTitle, Mathf.Clamp01(h * 3)); _bTitle.transform.localScale = Base(_bTitle) * Mathf.LerpUnclamped(1.3f, 1f, PanelMotion76.OutBack(h, 1.2f)); }
            // Jigo jumps: up from below, over the top, lands with a small squash
            float j = Span(t, .3f, .78f);
            if (_bJigo != null)
            {
                float y = 40 * (1 - j) - 60 * Mathf.Sin(Mathf.PI * j);
                _bJigo.transform.localPosition = Draw.W(_bJigoAt + new Vector2(0, y * _bScale));
                float squash = j > .85f && j < 1 ? Mathf.Sin((j - .85f) / .15f * Mathf.PI) * .07f : 0;
                float grow = Mathf.Lerp(.7f, 1f, Ease(Mathf.Clamp01(j / .35f)));
                _bJigo.transform.localScale = Vector3.Scale(Base(_bJigo) * grow, new Vector3(1 + squash, 1 - squash, 1));
                Alpha(_bJigo, Mathf.Clamp01(j * 4));
            }
            // the gold counts up, the bar comes in, the newest slot pops
            float c = Span(t, .62f, .92f);
            if (_bPlus != null) { _bPlus.Alpha = Mathf.Clamp01(c * 4); _bPlus.Text = "+" + Mathf.RoundToInt(_winCoins * Ease(c)); }
            Alpha(_bCoin, Mathf.Clamp01(c * 4));
            if (_doubleButton != null && _doubleButton.gameObject.activeSelf) Fade(_doubleButton, Mathf.Clamp01(c * 4));
            if (_bBar != null) Fade(_bBar, Span(t, .6f, .8f));
            if (_bNewSlot != null)
            {
                float p = Span(t, .9f, 1.05f);
                _bNewSlot.transform.localScale = Base(_bNewSlot) * (p <= 0 ? 0 : 1 + .35f * Mathf.Sin(p * Mathf.PI));
            }
            // the buttons come up
            float b = Ease(Span(t, .82f, Timeline));
            if (_bButtons != null) { Fade(_bButtons, b); _bButtons.localPosition = Draw.W(0, 36 * _bScale * (1 - PanelMotion76.OutBack(Span(t, .82f, Timeline), 1.25f))); }
            if (t >= Timeline) { _bSettled = true; if (_bJigo != null) _bJigo.transform.localScale = Base(_bJigo); }
        }
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
            if (Entering) { _started = Time.unscaledTime - EntranceSeconds; return; }   // PM_73: a tap finishes the entrance
            if (Time.unscaledTime - _started < .25f) return;
            if (_double && !_doubled && !_doubleBusy && !_chestMode && _doubleRect.Contains(point) && DoubleWatch != null)
            {
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
            if ((!_chestMode || _done) && _menuRect.Contains(point)) { Sink(_pressMenu, _menu); return; }
            if (!_chestMode)
            {
                if (!_nextRect.Contains(point)) return;
                if (_save.PendingChest21 > 0) { BeginChest(); Layout(_screen, _targets); }
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
            if (b == null || b.Body == null) { then(); return; }
            b.Press();
            _pending = then; _pendingAt = Time.unscaledTime + .12f;
        }

        public void Tick()
        {
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
    }
}
