using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_74: the loss and the continue offer in the T50 B win language (PM_73): no card; the shaded board under the
       blue light, the 3D title, Jigo (sad) in the middle, one line of help, a big green button and a small blue one.
       Entrance ≤ .7 s (title settles from 1.3x, Jigo drops in, the buttons come); a tap finishes it. 390x844 mock-up
       coordinates, scaled like the win screen. */
    sealed class LossView74
    {
        public const float EntranceSeconds = .55f;   // PM_76: was .7 s; the title and the buttons spring into place
        const string WinArt = "T50/win/";
        readonly Transform _root;
        readonly Vector2 _screen;
        readonly float _k, _top, _started;
        readonly int _order;
        Transform _title, _jigo, _buttons;
        SpriteRenderer _jigoSprite;
        readonly Sprite[] _sad = new Sprite[6];
        Rect _primary, _secondary, _close, _ad;
        bool _adOffer;
        bool _settled;
        readonly bool _offer, _affordable;

        public static bool Available => Art.Get(WinArt + "btn_green_big") != null;

        LossView74(Transform parent, Vector2 screen, int order, bool offer, bool affordable)
        {
            _screen = screen; _order = order; _offer = offer; _affordable = affordable;
            _k = Mathf.Min(screen.x / 390, screen.y / 844); _top = (screen.y - 844 * _k) * .5f;
            _started = Time.unscaledTime;
            _root = new GameObject(offer ? "PM74 continue" : "PM74 loss").transform; _root.SetParent(parent, false);
            ShapeView.Create(_root, "shade", screen, 0, order).Fill(Draw.Rgba(8, 14, 40, .78f)).transform.localPosition = Draw.W(screen * .5f);
            float cover = Mathf.Max(screen.x / 390, screen.y / 844);
            var glow = Place(_root, "win_bg_glow", screen * .5f, 390 * cover / _k, 844 * cover / _k, 1);
            if (glow != null) glow.color = new Color(.78f, .86f, 1f, .62f);   // the win light, cooler and dimmer
            for (int i = 0; i < 6; i++) _sad[i] = Art.Get("jigo/uzgun/" + (i + 1).ToString("00"));
        }

        Vector2 At(float x, float y) => new Vector2(_screen.x * .5f + (x - 195) * _k, _top + y * _k);
        SpriteRenderer Place(Transform parent, string key, Vector2 at, float w, float h, int layer)
        {
            var r = Art.Place(parent, WinArt + key, new Vector2(w, h) * _k, _order + layer);
            if (r != null) r.transform.localPosition = Draw.W(at);
            return r;
        }
        Transform Group(string name) { var t = new GameObject(name).transform; t.SetParent(_root, false); return t; }
        TextView Text(Transform parent, string text, Vector2 at, float size, int layer)
        {
            var t = TextView.Create(parent, "label", text, size * _k, Palette.Cream, order: _order + layer);
            t.transform.localPosition = Draw.W(at); return t;
        }

        void Title(string text, float width = 340)
        {
            _title = Group("title");
            _title.localPosition = Draw.W(At(width < 340 ? 182 : 195, 150));
            TextView.Create(_title, "title", text, 50 * _k, Palette.Cream, order: _order + 6).Title3D().FitWidth(width * _k, 26 * _k);
        }

        void Jigo(float y, float size)
        {
            _jigo = Group("jigo");
            _jigo.localPosition = Draw.W(At(195, y));
            if (_sad[0] == null) return;
            _jigoSprite = Draw.Child<SpriteRenderer>(_jigo, "jigo sad");
            _jigoSprite.sprite = _sad[0]; _jigoSprite.sortingOrder = _order + 4;
            _jigoSprite.transform.localScale = Vector3.one * (size * _k / _sad[0].bounds.size.y);
        }

        void Buttons(string primary, string secondary, float y)
        {
            _buttons = Group("buttons");
            if (Button75.Available)
            {
                // PM_75: the menu's button family (green main, navy small)
                var main = Button75.Main(_buttons, At(195, y), new Vector2(262, 88) * _k, primary, _order + 6);
                if (_offer && !_affordable && main.Body != null) main.Body.color = new Color(.5f, .5f, .5f, .95f);
                _primary = main.Rect;
                if (secondary == null) return;
                var small = Button75.Small(_buttons, At(195, y + 86), new Vector2(150, 50) * _k, secondary, _order + 6);
                var sc = small.Rect.center;
                float sTop = Mathf.Max(sc.y - 26, _primary.yMax + 2);
                _secondary = new Rect(sc.x - 80, sTop, 160, Mathf.Max(44, sc.y + 26 - sTop));
                return;
            }
            var g = Place(_buttons, "btn_green_big", At(195, y), 300, 72, 6);
            if (g != null && _offer && !_affordable) g.color = new Color(.5f, .5f, .5f, .95f);
            var p = Text(_buttons, primary, At(195, y - 2), 28, 8).Heading(TextView.RimGreen).FitWidth(250 * _k, 16 * _k);
            _primary = new Rect(At(195, y) - new Vector2(150, 36) * _k, new Vector2(300, 72) * _k);
            if (secondary == null) return;
            Place(_buttons, "btn_blue_small", At(195, y + 88), 136, 48, 6);
            Text(_buttons, secondary, At(195, y + 86), 21, 8).Heading(Draw.Hex("#153E75")).FitWidth(120 * _k, 14 * _k);
            var c = At(195, y + 88);
            float top = Mathf.Max(c.y - 26, _primary.yMax + 2);
            _secondary = new Rect(c.x - 80, top, 160, Mathf.Max(44, c.y + 26 - top));
        }

        /* The loss: "Time's up" / "The bomb exploded", the pieces left, a tip, Try again, Menu. */
        public static LossView74 Loss(Transform parent, Vector2 screen, int order, GameSession s)
        {
            var v = new LossView74(parent, screen, order, false, true);
            bool bomb = s.Outcome == Outcome.Bomb;
            v.Title(bomb ? Lang.T("The bomb exploded", "Bomba patladı") : Lang.T("Time's up", "Süre doldu"));
            v.Jigo(340, 250);
            v.Text(v._root, string.Format(Lang.T("{0} pieces left", "{0} parça kaldı"), s.Board.PresentCount), v.At(195, 498), 30, 6)
                .Heading(Draw.Hex("#153E75"));
            v.Text(v._root, bomb ? Lang.T("Clear the bomb first. The hammer removes it in one move.", "Önce bombayı çıkar. Çekiç onu tek hamlede söker.")
                                 : Lang.T("The clock booster stops the time for 10 seconds.", "Saat booster'ı süreyi 10 saniye durdurur."),
                v.At(195, 540), 17, 6).Heading(TextView.RimTeal).FitWidth(330 * v._k, 12 * v._k);
            v.Buttons(Texts.Retry, Lang.T("Menu", "Menü"), 640);
            v.Tick();
            return v;
        }

        /* The continue offer: the title, Jigo, "+30 s" (or the bomb reset), the price on the green button, the coins,
           a close button (and "No, thanks" with &devamhayir=1). */
        TextView _coins;
        bool _showNo;
        public static LossView74 Continue(Transform parent, Vector2 screen, Vector4 safe, int order, ContinueOffer offer, int coins, bool showNo, bool preview, bool ad = false)
        {
            var v = new LossView74(parent, screen, order, true, offer.Affordable || preview);
            v._showNo = showNo;
            v._adOffer = ad && Button75.Available;
            bool time = offer.Kind == ContinueKind.Time;
            v.Title(time ? Lang.T("Time's up", "Süre doldu") : Lang.T("Bomb exploded", "Bomba patladı"), 250);   // room for the close button
            v.Jigo(330, 230);
            string benefit = time ? string.Format(Lang.T("+{0} s", "+{0} sn"), offer.Seconds) : Lang.T("Reset the bomb", "Bombayı sıfırla");
            var b = v.Text(v._root, benefit, v.At(time ? 215 : 195, 490), 40, 7).Title3D();
            if (time) Art54.Place(v._root, "devam_v2/continue-clock", v.At(215, 490) - new Vector2(b.Width / 2 + 30 * v._k, 0), Vector2.one * 46 * v._k, order + 7);
            else v.Text(v._root, string.Format(Lang.T("Fuse: {0}", "Sayaç: {0}"), new ContinueRules().BombFuse), v.At(195, 530), 20, 7).Heading(TextView.RimTeal);
            if (v._adOffer)
            {
                // PM_81 R1: the gold button and, beside it, "Watch an ad +30 s" (once per attempt)
                v._buttons = v.Group("buttons");
                var gold = Button75.Main(v._buttons, v.At(110, 610), new Vector2(176, 84) * v._k, "", order + 6);
                if (!v._affordable && gold.Body != null) gold.Body.color = new Color(.5f, .5f, .5f, .95f);
                v._primary = gold.Rect;
                Art54.Place(v._buttons, "devam_v2/continue-coin", v.At(82, 608), Vector2.one * 34 * v._k, order + 8);
                v.Text(v._buttons, offer.Price.ToString(), v.At(130, 607), 28, 8).Heading(TextView.RimGreen);
                var watch = Button75.Small(v._buttons, v.At(286, 610), new Vector2(176, 84) * v._k, "", order + 6);
                v._ad = watch.Rect;
                v.Text(v._buttons, Lang.T("Watch an ad", "Reklam izle"), v.At(286, 594), 17, 8).Heading(Draw.Hex("#0E2244")).FitWidth(150 * v._k, 11 * v._k);
                v.Text(v._buttons, string.Format(Lang.T("+{0} s", "+{0} sn"), offer.Seconds), v.At(286, 620), 26, 8).Heading(Draw.Hex("#0E2244"));
                if (showNo)
                {
                    var no = Button75.Small(v._buttons, v.At(195, 696), new Vector2(150, 50) * v._k, Lang.T("No, thanks", "Hayır"), order + 6);
                    v._secondary = no.Rect;
                }
            }
            else
            {
                v.Buttons("", showNo ? Lang.T("No, thanks", "Hayır") : null, 610);
                // the price on the green button: a coin and the number
                Art54.Place(v._buttons, "devam_v2/continue-coin", v.At(160, 608), Vector2.one * 38 * v._k, order + 8);
                v.Text(v._buttons, offer.Price.ToString(), v.At(214, 607), 30, 8).Heading(TextView.RimGreen);
            }
            Art54.Place(v._root, "devam_v2/continue-coin", v.At(140, 668), Vector2.one * 24 * v._k, order + 7);
            v._coins = v.Text(v._root, string.Format(Lang.T("{0} coins", "{0} altın"), coins), v.At(210, 668), 18, 7).Heading(TextView.RimTeal);
            // close: on the title row, at the right (it used to sit on the HUD's Time box)
            var close = new Vector2(Mathf.Min(screen.x - 28, v.At(354, 150).x), v.At(354, 150).y);
            Art54.Place(v._root, "devam_v2/close-button", close, Vector2.one * 48, order + 7);
            Art54.Place(v._root, "devam_v2/icon-close", close, Vector2.one * 25, order + 8);
            v._close = new Rect(close - Vector2.one * 26, Vector2.one * 52);
            v.Tick();
            return v;
        }

        public bool Entering => !_settled && Time.unscaledTime - _started < EntranceSeconds;

        public void Tick()
        {
            float t = Time.unscaledTime - _started;
            float e = Mathf.Clamp01(t / EntranceSeconds);
            if (_settled) e = 1;
            if (_title != null) _title.localScale = Vector3.one * Mathf.LerpUnclamped(1.3f, 1, PanelMotion76.OutBack(Mathf.Clamp01(e / .5f), 1.2f));
            if (_title != null) foreach (var tv in _title.GetComponentsInChildren<TextView>()) tv.Alpha = Mathf.Clamp01(e / .25f);
            if (_jigo != null)
            {
                float j = BoosterFx74.OutCubic(Mathf.Clamp01((e - .15f) / .5f));
                var home = At(195, _offer ? 330 : 340);
                _jigo.localPosition = Draw.W(home + new Vector2(0, -60 * _k * (1 - j)));
                if (_jigoSprite != null)
                {
                    _jigoSprite.color = new Color(1, 1, 1, j);
                    // a slow sad sway; a blink every ~4 s (frame 4 of the sad set)
                    _jigoSprite.transform.localRotation = Quaternion.Euler(0, 0, 2 * Mathf.Sin(Time.unscaledTime * 1.6f));
                    float cycle = Time.unscaledTime % 4.2f;
                    var f = cycle < .12f && _sad[3] != null ? _sad[3] : _sad[0];
                    if (f != null) _jigoSprite.sprite = f;
                }
            }
            if (_buttons != null)
            {
                float b = BoosterFx74.OutCubic(Mathf.Clamp01((e - .5f) / .5f));
                _buttons.localPosition = Draw.W(0, 24 * _k * (1 - PanelMotion76.OutBack(Mathf.Clamp01((e - .5f) / .5f), 1.25f)));
                foreach (var r in _buttons.GetComponentsInChildren<SpriteRenderer>()) { var c = r.color; c.a = b; r.color = c; }
                foreach (var tv in _buttons.GetComponentsInChildren<TextView>()) tv.Alpha = b;
            }
            if (e >= 1) _settled = true;
        }

        /* "retry" / "menu" for the loss; "accepted" / "declined" for the offer; null otherwise. A tap during the
           entrance finishes it first. */
        public string Hit(Vector2 pt)
        {
            if (Entering) { _settled = true; Tick(); return null; }
            if (_offer)
            {
                if (_close.Contains(pt) || _showNo && _secondary.Contains(pt)) return "declined";
                if (_adOffer && _ad.Contains(pt)) return "ad";   // PM_81 R1
                return _primary.Contains(pt) && _affordable ? "accepted" : null;
            }
            if (_primary.Contains(pt)) return "retry";
            if (_secondary.Contains(pt)) return "menu";
            return null;
        }

        public void Destroy() { if (_root != null) Object.Destroy(_root.gameObject); }
    }
}
