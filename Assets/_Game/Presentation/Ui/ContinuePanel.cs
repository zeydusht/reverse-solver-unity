using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    sealed class ContinuePanel
    {
        readonly Transform _root;
        readonly SpriteRenderer _jigo;
        readonly Sprite[] _frames = new Sprite[6];
        readonly Rect _close, _buy, _no;
        readonly bool _affordable, _showNo;

        public ContinuePanel(Transform parent, Vector2 screen, Vector4 safe, int order,
            ContinueOffer offer, int coins, bool showNo, bool preview = false)
        {
            _affordable = offer.Affordable || preview; _showNo = showNo;
            _root = new GameObject("Continue offer").transform;
            _root.SetParent(parent, false);
            ShapeView.Create(_root, "shade", screen, 0, order).Fill(Draw.Rgba(10, 18, 46, .75f))
                .transform.localPosition = Draw.W(screen * .5f);
            float w = Mathf.Min(340, screen.x - safe.x - safe.z - 20);
            float h = Mathf.Min(470, screen.y - safe.y - safe.w - 20);
            Vector2 c = new Vector2((screen.x + safe.x - safe.z) / 2, (screen.y + safe.y - safe.w) / 2);
            var content = new GameObject("Panel").transform;
            content.SetParent(_root, false);
            content.localPosition = Draw.W(c);
            Art54.Stretch(content, "devam_v2/panel", Vector2.zero, new Vector2(w, h), order + 1);
            Art54.Place(content, "hud40/headers/continue", new Vector2(0, -h / 2 + 26), new Vector2(w, 78), order + 2);
            Label(content, Lang.T(offer.Kind == ContinueKind.Time ? "Time's up" : "Bomb exploded",
                offer.Kind == ContinueKind.Time ? "Süre doldu" : "Bomba patladı"), new Vector2(0, -h / 2 + 24), 23, w - 96, order + 3);
            Vector2 close = new Vector2(w / 2 - 22, -h / 2 + 6);
            Art54.Place(content, "devam_v2/close-button", close, Vector2.one * 48, order + 4);
            Art54.Place(content, "devam_v2/icon-close", close, Vector2.one * 25, order + 5);
            _close = Target(c + close, 48, 48);
            float jigoSize = Mathf.Min(210, h * .45f);
            _jigo = Art54.Place(content, "jigo/uzgun/01", new Vector2(0, -h * .12f), Vector2.one * jigoSize, order + 2);
            for (int i = 0; i < 6; i++) _frames[i] = Art.Get("jigo/uzgun/" + (i + 1).ToString("00"));
            string benefit = offer.Kind == ContinueKind.Time ? string.Format(Lang.T("+{0} s", "+{0} sn"), offer.Seconds)
                : Lang.T("Reset the bomb", "Bombayı sıfırla");
            Label(content, benefit, new Vector2(0, h * .13f), 26, w - 44, order + 3);
            if (offer.Kind == ContinueKind.Bomb)
                Label(content, string.Format(Lang.T("Fuse: {0}", "Sayaç: {0}"), new ContinueRules().BombFuse),
                    new Vector2(0, h * .13f + 29), 17, w - 44, order + 3);
            else Art54.Place(content, "devam_v2/continue-clock", new Vector2(-w * .30f, h * .13f), Vector2.one * 36, order + 3);
            Vector2 buy = new Vector2(0, h * .30f);
            var button = Art54.Place(content, "devam_v2/continue-primary", buy, new Vector2(w * .78f, 82), order + 2);
            if (!_affordable && button != null) button.color = new Color(.45f, .45f, .45f, .9f);
            Art54.Place(content, "devam_v2/continue-coin", buy + new Vector2(-48, -2), Vector2.one * 34, order + 3);
            Label(content, string.Format(Lang.T("{0}", "{0}"), offer.Price), buy + new Vector2(18, -2), 29, 110, order + 3);
            _buy = Target(c + buy, w * .78f, 64);
            Art54.Place(content, "devam_v2/continue-coin", new Vector2(-48, -h * .35f), Vector2.one * 25, order + 3);
            Label(content, string.Format(Lang.T("{0} coins", "{0} altın"), coins), new Vector2(22, -h * .35f), 18, 132, order + 3);
            Vector2 no = new Vector2(0, h / 2 - 26);
            if (showNo) Label(content, Lang.T("No, thanks", "Hayır, teşekkürler"), no, 17, w - 40, order + 3);
            _no = Target(c + no, w - 40, 44);
        }

        static Rect Target(Vector2 c, float w, float h) => new Rect(c.x - w / 2, c.y - h / 2, w, h);
        static void Label(Transform parent, string text, Vector2 at, float size, float width, int order)
        {
            TextView.Create(parent, "label", text, size, Palette.Cream, true, order: order)
                .Heading(TextView.RimTeal).FitWidth(width, size * .7f).transform.localPosition = Draw.W(at);
        }
        public void Tick() { if (_jigo != null) _jigo.sprite = _frames[(int)(Time.unscaledTime * 8) % 6]; }
        public string Hit(Vector2 pt) => _close.Contains(pt) || _showNo && _no.Contains(pt) ? "declined"
            : _buy.Contains(pt) && _affordable ? "accepted" : null;
        public void Destroy() { if (_root != null) Object.Destroy(_root.gameObject); }
    }
}
