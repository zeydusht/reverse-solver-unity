using System;
using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    // The same short result surface owns the chest; no intermediate celebration.
    sealed class P21WinView
    {
        const int Order = 5600;
        readonly Transform _root;
        readonly SaveData _save;
        readonly int _rescued;
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
        Rect _nextRect, _menuRect, _chestRect;

        public P21WinView(Transform parent, Vector2 screen, Rect[] targets, SaveData save, int rescued,
            bool chest, Action next, Action refresh, Action menu)
        {
            _root = new GameObject("P21 win and chest").transform; _root.SetParent(parent, false);
            _save = save; _rescued = rescued; _next = next; _refresh = refresh; _menu = menu;
            _started = Time.unscaledTime;
            if (chest) BeginChest();
            Layout(screen, targets);
        }
        Vector2 At(float x, float y) => new Vector2(_screen.x * .5f + (x - 195) * _scale, _top + y * _scale);
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
            ShapeView.Create(_content, "shade", screen, 0, Order).Fill(Draw.Rgba(10, 18, 46, .88f)).transform.localPosition = Draw.W(screen * .5f);
            if (!_chestMode && T48) { LayoutT48(); _language = Lang.Revision; Tick(); return; }
            Image("Victory/panel", 195, 380, 370, 740);
            Image("Titles/light", 195, 105, 340, 125, 2);
            // Live Baloo lettering: gold extrusion behind the blue rim and cream face.
            Label(_chestMode ? Texts.Chest21 : Texts.Win21, 195, 112, 48, 3).Heading(Draw.Hex("#D79A26"));
            Label(_chestMode ? Texts.Chest21 : Texts.Win21, 195, 105, 48, 4).Heading(Draw.Hex("#153E75"));
            _hint = Label("", 195, 575, 22);
            _amount = Label("", 195, 458, 30, 16).Heading(TextView.RimTeal);
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
                Image("Reward/Flight/coin-counter", 290, 52, 116, 37, 8);
                Image("Reward/Flight/coin", 251, 52, 30, 30, 9);
                _coins = Label(_save.Coins.ToString(), 299, 52, 18);
                if (_done) NextButton();
                foreach (var b in BoosterExt.All)
                {
                    var r = Layer("destination", Order + 8);
                    Paint(r, "Reward/Reveal/Items/" + b.Id(), _targets[(int)b].center, _targets[(int)b].size * .8f);
                }
            }
            _language = Lang.Revision; Tick();
        }
        /* PM_67: the T48 win card (docs/art/task48/LISTE.md, 390x844 mock-up; y here = mock-up y - 42). Cream panel
           with a blue frame, 3D "Great!", happy Jigo in a gold glow, chest progress in a cream tray, the rescued count
           in dark blue, a layered green Next Level and a small blue Menu. */
        static bool T48 => Art.Get("T48/win/next-button") != null;
        SpriteRenderer T48Image(string key, float x, float y, float w, float h, int layer, bool sliced = false)
        {
            var r = Art.Place(_content, key.StartsWith("devam_v2") ? key : "T48/win/" + key, new Vector2(w, h) * _scale, Order + layer, sliced);
            if (r != null) r.transform.localPosition = Draw.W(At(x, y));
            return r;
        }
        TextView Dark(string text, float x, float y, float size, int layer = 10)
        {
            var t = TextView.Create(_content, "label", text, size * _scale, Draw.Hex("#234670"), order: Order + layer);
            if (TextView.HeadingFont != null) t.Font(TextView.HeadingFont);
            t.transform.localPosition = Draw.W(At(x, y)); return t;
        }
        void LayoutT48()
        {
            T48Image("devam_v2/panel", 195, 380, 342, 684, 1, sliced: true);
            T48Image("title-light", 195, 109, 344, 144, 2);
            Label(Texts.Win21, 195, 116, 50, 3).Heading(Draw.Hex("#D79A26"));
            Label(Texts.Win21, 195, 109, 50, 4).Heading(Draw.Hex("#153E75"));
            T48Image("portrait-light", 195, 272, 318, 264, 2);
            T48Image("jigo-happy", 195, 272, 210, 210, 3);
            T48Image("progress-tray", 195, 457.5f, 282, 107, 2, sliced: true);
            int progress = _save.ChestProgress21, pending = _save.PendingChest21;
            int next = pending > 0 ? pending : P21ChestRules.Next(progress), previous = P21ChestRules.Previous(next - 1);
            int slots = next - previous;
            float slotStep = Mathf.Min(44, 190f / Mathf.Max(1, slots));
            for (int i = 0; i < slots; i++)
                T48Image(previous + i < progress ? "slot-complete" : "slot-empty", 85 + i * slotStep, 436, 40, 39, 3);
            T48Image("chest", 300, 433, 62, 58, 3);
            Dark(pending > 0 ? Texts.Chest21 : Texts.ChestIn21(next - progress), 195, 483, 17);
            _rescuedLabel = Dark(Texts.Rescued21(0), 207, 535, 22);
            var icon = T48Image("jiggle-icon", 82, 535, 32, 32, 5);
            _nextRect = new Rect(At(60, 577), new Vector2(270, 64) * _scale);
            _menuRect = new Rect(At(135, 662), new Vector2(120, 40) * _scale);
            T48Image("next-button", 195, 609, 270, 64, 3, sliced: true);
            Label(Texts.NextLevel21, 195, 607, 25).Heading(TextView.RimGreen).FitWidth(230 * _scale, 16 * _scale);
            T48Image("menu-button", 195, 682, 120, 40, 3, sliced: true);
            Label(Lang.T("Menu", "Menü"), 195, 681, 18).Heading(Draw.Hex("#153E75"));
        }

        void NextButton()
        {
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
            if (Time.unscaledTime - _started < .25f) return;
            if ((!_chestMode || _done) && _menuRect.Contains(point)) { _menu(); return; }
            if (!_chestMode)
            {
                if (!_nextRect.Contains(point)) return;
                if (_save.PendingChest21 > 0) { BeginChest(); Layout(_screen, _targets); }
                else _next();
                return;
            }
            if (_done) { if (_nextRect.Contains(point)) _next(); return; }
            if (_taps >= 3 || !_chestRect.Contains(point) || Time.unscaledTime - _tapAt < 4f / 14) return;
            _taps++; _tapAt = Time.unscaledTime;
            if (_taps == 3)
            {
                if (_chest > 0 && _save.ClaimChest21(_chest, _booster)) _refresh();
            }
        }
        static string Frame(string path, int frame) => path + "/" + frame.ToString("00");
        public void Tick()
        {
            if (_language != Lang.Revision) { Layout(_screen, _targets); return; }
            float now = Time.unscaledTime, age = now - _tapAt;
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
            Vector2 center = At(195, 375), target = reward == 0 ? At(290, 52) : _targets[(int)_booster].center;
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
