using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* The web game's .nail and .bomb badges (index.html CSS), drawn on the
       piece's first cell and moving with it. */
    public sealed class NailBadge
    {
        public readonly Transform Root;
        readonly TextView _n;

        public NailBadge(Transform parent, Vector2 center, float cell, int order)
        {
            Root = new GameObject("Nail").transform;
            Root.SetParent(parent, false);
            Root.localPosition = Draw.W(center);
            float d = Mathf.Max(22f, cell * 0.46f);
            // radial-gradient(60% 50% at 32% 26%, #f2f5f8, #8d9aa6 60%, #5b6874)
            ShapeView.Create(Root, "disc", new Vector2(d, d), 8, order)
                .Radius(d / 2)
                .Radial(Draw.Hex("#f2f5f8"), Draw.Hex("#8d9aa6"), Draw.Hex("#5b6874"), new Vector4(.32f, .26f, .6f, .5f))
                .Shadow(Draw.Rgba(0, 0, 0, .5f), 0, 2, 4)
                .Inner(Draw.Rgba(0, 0, 0, .35f), -2, 3);
            _n = TextView.Create(Root, "count", "", 13, Draw.Hex("#1d2b31"), true, order: order + 1);
        }

        public int Count { set => _n.Text = value.ToString(); }

        /* .nail.tick: scale 1.45 -> 1 over .32s cubic-bezier(.3,1.5,.5,1) on every count. */
        public void Tick() => Tweens.Run(this, .32f, t => { if (Root != null) Root.localScale = Vector3.one * Mathf.LerpUnclamped(1.45f, 1f, Tweens.Tick(t)); });
    }

    public sealed class BombBadge
    {
        public readonly Transform Root;
        readonly TextView _n;
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
