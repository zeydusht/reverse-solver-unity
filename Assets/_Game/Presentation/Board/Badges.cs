using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* The web game's .nail and .bomb badges (index.html CSS), drawn on the
       piece's first cell and moving with it. */
    public sealed class NailBadge
    {
        public readonly Transform Root;
        readonly TextView _n;
        readonly float _one, _two;                  // counter size (pt) for one digit / two digits (art counters)

        public NailBadge(Transform parent, Vector2 center, float cell, int order)
        {
            Root = new GameObject("Nail").transform;
            Root.SetParent(parent, false);
            Root.localPosition = Draw.W(center);
            float d = Mathf.Max(22f, cell * 0.46f);
            // nail head seen from above (head = 80 % of the canvas), navy digits ~55 % of the head with a 2 px cream rim
            if (ArtBadge(Root, "board-nail", cell, .8f, 0, order, Draw.Hex("#0B2A36"), Draw.Hex("#fff4da"), out _n))
            { _one = cell * .46f; _two = cell * .36f; return; }
            if (ArtBadge(Root, "icon-nail", cell, new Vector2(-.2f, -.2f), order, out _n)) return;   // count on the nail head (upper left)
            // radial-gradient(60% 50% at 32% 26%, #f2f5f8, #8d9aa6 60%, #5b6874)
            ShapeView.Create(Root, "disc", new Vector2(d, d), 8, order)
                .Radius(d / 2)
                .Radial(Draw.Hex("#f2f5f8"), Draw.Hex("#8d9aa6"), Draw.Hex("#5b6874"), new Vector4(.32f, .26f, .6f, .5f))
                .Shadow(Draw.Rgba(0, 0, 0, .5f), 0, 2, 4)
                .Inner(Draw.Rgba(0, 0, 0, .35f), -2, 3);
            _n = TextView.Create(Root, "count", "", 13, Draw.Hex("#1d2b31"), true, order: order + 1);
        }

        public int Count { set { _n.Text = value.ToString(); Fit(_n, value, _one, _two); } }

        /* Two digits get a smaller size so they never crowd the disc. */
        internal static void Fit(TextView t, int value, float one, float two)
        {
            if (one > 0) t.SizePt = value >= 10 ? two : one;
        }

        /* Preview art (#10): the icon a bit larger than the drawn disc, the counter on a dark chip at its
           lower right so the number stays readable on a phone-size cell. */
        /* Codex board art (PM_MESAJ_11/12): the sprite sized so its body is `body` of the cell, shifted up by `lift`
           (share of the sprite) so the body, not the canvas, sits on the cell centre; the counter in Fredoka on top. */
        internal static bool ArtBadge(Transform root, string key, float cell, float body, float lift, int order,
                                      Vector4 ink, Vector4? rim, out TextView count)
        {
            count = null;
            float size = cell * body;
            var r = Art.Place(root, key, new Vector2(size, size), order);
            if (r == null) return false;
            r.transform.localPosition = Draw.W(0, -lift * size);
            count = TextView.Create(root, "count", "", cell * .4f, ink, true, order: order + 2)
                .Font(Resources.Load<TMPro.TMP_FontAsset>("Fonts/Counter"));
            if (rim.HasValue) count.Outline(rim.Value, .22f);
            return true;
        }

        // legacy icon badge (no Codex board art): kept as the fallback
        internal static bool ArtBadge(Transform root, string key, float cell, Vector2 at, int order, out TextView count)
        {
            count = null;
            float d = cell * .8f;
            if (Art.Place(root, key, new Vector2(d, d), order) == null) return false;
            count = TextView.Create(root, "count", "", cell * .4f, Draw.Hex("#ffffff"), true, order: order + 2)
                .Outline(Draw.Hex("#0c1418"), .3f);
            count.transform.localPosition = Draw.W(at * d);
            return true;
        }

        /* .nail.tick: scale 1.45 -> 1 over .32s cubic-bezier(.3,1.5,.5,1) on every count. */
        public void Tick() => Tweens.Run(this, .32f, t => { if (Root != null) Root.localScale = Vector3.one * Mathf.LerpUnclamped(1.45f, 1f, Tweens.Tick(t)); });
    }

    public sealed class BombBadge
    {
        public readonly Transform Root;
        readonly TextView _n;
        readonly float _one, _two;
        readonly Transform _body;
        bool _hot;

        public BombBadge(Transform parent, Vector2 center, float cell, int order)
        {
            Root = new GameObject("Bomb").transform;
            Root.SetParent(parent, false);
            Root.localPosition = Draw.W(center);
            _body = new GameObject("body").transform;
            _body.SetParent(Root, false);
            float d = Mathf.Max(24f, cell * 0.52f);
            // body = 71 % of the canvas, its centre 39 px (10 %) below the canvas centre (the fuse is on top): body 85 % of
            // the cell, sprite lifted so the body is centred; navy digits ~70 % of the cream disc, no rim
            if (NailBadge.ArtBadge(_body, Art.Get("bomb-v2") != null ? "bomb-v2" : "board-bomb", cell, Art.Get("bomb-v2") != null ? .85f : 1.19f,
                                   Art.Get("bomb-v2") != null ? 0 : .1f, order, Draw.Hex("#0B2A36"), null, out _n))
            { _one = cell * .5f; _two = cell * .38f; return; }
            if (NailBadge.ArtBadge(_body, "icon-bomb", cell, new Vector2(-.03f, .06f), order, out _n)) return;   // count on the bomb's round body
            // radial-gradient(58% 48% at 32% 26%, #6c7a84, #232f36 62%, #111a1f); ring 2px #0c1418;
            // 0 3px 6px rgba(0,0,0,.6); 0 0 14px rgba(226,74,60,.55)
            ShapeView.Create(_body, "disc", new Vector2(d, d), 16, order)
                .Radius(d / 2)
                .Radial(Draw.Hex("#6c7a84"), Draw.Hex("#232f36"), Draw.Hex("#111a1f"), new Vector4(.32f, .26f, .58f, .48f))
                .Stroke(Draw.Hex("#0c1418"), 2)
                .Shadow(Draw.Rgba(0, 0, 0, .6f), 0, 3, 6)
                .Glow(Draw.Rgba(226, 74, 60, .30f), 9);          // a CSS 14px blur spreads ~half that
            // wick: top -22%, left 52%, 16% x 34%, rotate(18deg), #ffd76a -> #e2604f, glow #ffb03a
            var wick = ShapeView.Create(_body, "wick", new Vector2(d * .16f, d * .34f), 8, order + 1)
                .Radius(3).Gradient(Draw.Hex("#ffd76a"), Draw.Hex("#e2604f")).Glow(Draw.Hex("#ffb03a", .5f), 4);
            wick.transform.localPosition = Draw.W(d * (.52f + .08f) - d / 2, d * (-.22f + .17f) - d / 2);
            wick.transform.localRotation = Quaternion.Euler(0, 0, -18);
            _n = TextView.Create(_body, "fuse", "", 14, Draw.Hex("#ffd9d2"), true, order: order + 2);
        }

        public int Fuse
        {
            set
            {
                _n.Text = value.ToString();
                NailBadge.Fit(_n, value, _one, _two);
                bool hot = value <= 4;                          // .bomb.hot
                if (hot && !_hot) Pulse();
                if (!hot) { Tweens.Kill(this); _body.localScale = Vector3.one; }
                _hot = hot;
            }
        }

        // @keyframes fuse { 0%,100% scale 1; 50% scale 1.16 } .7s infinite
        void Pulse()
        {
            Tweens.Run(this, .7f, t => _body.localScale = Vector3.one * (1f + .16f * Mathf.Sin(t * Mathf.PI)),
                       () => { if (_hot && _body != null) Pulse(); });
        }

        /* .bomb.tick: the same pop as the nail on every fuse step (on the root, so the hot pulse keeps its own scale). */
        public void Tick() => Tweens.Run(Root, .32f, t => { if (Root != null) Root.localScale = Vector3.one * Mathf.LerpUnclamped(1.45f, 1f, Tweens.Tick(t)); });

        /* Bomb feedback: a quick swell before the result card. */
        public void Explode()
        {
            Tweens.Run(this, .35f, t => _body.localScale = Vector3.one * (1f + 1.2f * t));
        }
    }
}
