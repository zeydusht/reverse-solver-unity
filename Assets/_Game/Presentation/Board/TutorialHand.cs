using System.Collections.Generic;
using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* The web game's .tut overlay on level 1: a white pointing hand on the
       piece and an amber arrow one cell ahead in the exit direction, both
       nudging along that direction (@keyframes nudge 1.5s ease-in-out:
       0%/100% at rest, opacity .75; 45% moved S*.55, opacity 1), and the
       target piece glowing amber (.pc.target drop-shadow 0 0 10px).

       Drawn from shapes like the icons: the hand's outline is the same shapes
       grown and dark, laid under the white ones, so the parts merge into one
       silhouette. Only graphics: input goes through as before.

       One difference from the web: the arrow sits a cell beyond the piece, so
       on a piece at the board's edge it would hang off a phone screen (L01's
       first move is the bottom-left piece, leftwards: 375 pt leaves 31 pt
       beside the board). Then the arrow is held inside the screen at the
       board's edge and only the hand nudges. */
    public sealed class TutorialHand : MonoBehaviour
    {
        const float Period = 1.5f, Peak = .45f;

        readonly List<ShapeView> _shapes = new List<ShapeView>();
        readonly List<SolidView> _solids = new List<SolidView>();
        Transform _move;
        Vector2 _nudge;
        float _t;

        /* visible: the screen in the board's local points (y down). */
        public static TutorialHand Show(BoardView board, PieceView piece, Move m, int order, Rect visible)
        {
            float S = board.Cell;
            var h = new GameObject("Tutorial").AddComponent<TutorialHand>();
            h.transform.SetParent(board.transform, false);
            var cell = board.Session.Level.Pieces[m.Piece][0];
            var dir = new Vector2(m.Dir.Dx(), m.Dir.Dy());
            h._nudge = dir * S * .55f;

            // the target piece glows; parented to the piece so it follows a drag
            if (piece != null)
            {
                var glow = ShapeView.Create(piece.transform, "TargetGlow", new Vector2(S, S), 24, piece.SortingOrder - 1)
                    .Radius(S * .12f).Fill(Draw.Rgba(240, 167, 66, 0)).Stroke(Draw.Rgba(240, 167, 66, .9f), 2.5f)
                    .Glow(Draw.Rgba(240, 167, 66, 1f), 16);
                glow.transform.localPosition = Draw.W((cell.X + .5f) * S, (cell.Y + .5f) * S);
                h._shapes.Add(glow);
                h._glow = glow;
            }

            h._move = new GameObject("Nudge").transform;
            h._move.SetParent(h.transform, false);

            // arrow: SVG 24x24 "M12 2 L20.5 12 H15.4 V22 H8.6 V12 H3.5 Z", S*.68 wide, one cell ahead
            var arrow = new GameObject("Arrow").transform;
            float half = S * .34f + 4;
            var rest = new Vector2((cell.X + .5f + dir.x) * S, (cell.Y + .5f + dir.y) * S);
            var far = rest + h._nudge;
            bool fits = visible.xMin + half <= Mathf.Min(rest.x, far.x) && Mathf.Max(rest.x, far.x) <= visible.xMax - half &&
                        visible.yMin + half <= Mathf.Min(rest.y, far.y) && Mathf.Max(rest.y, far.y) <= visible.yMax - half;
            if (!fits)
                rest = new Vector2(Mathf.Clamp(rest.x, visible.xMin + half, visible.xMax - half),
                                   Mathf.Clamp(rest.y, visible.yMin + half, visible.yMax - half));
            arrow.SetParent(fits ? h._move : h.transform, false);
            arrow.localPosition = Draw.W(rest);
            float rot = m.Dir == Dir.U ? 0 : m.Dir == Dir.R ? 90 : m.Dir == Dir.D ? 180 : 270;
            arrow.localRotation = Quaternion.Euler(0, 0, -rot);
            float u = S * .68f / 24f;
            Vector2 A(float x, float y) => new Vector2((x - 12) * u, (y - 12) * u);
            var halo = ShapeView.Create(arrow, "halo", new Vector2(S * .5f, S * .5f), 16, order)
                .Radius(S * .25f).Fill(Draw.Rgba(240, 167, 66, 0)).Glow(Draw.Rgba(240, 167, 66, .7f), 8);
            h._shapes.Add(halo);
            var head = SolidView.Polygon(arrow, "head", new[] { A(12, 1.3f), A(21.6f, 12.5f), A(2.4f, 12.5f) }, Draw.Hex("#fff5dc"), order + 1);
            var headIn = SolidView.Polygon(arrow, "headIn", new[] { A(12, 2.6f), A(19.9f, 11.8f), A(4.1f, 11.8f) }, Draw.Hex("#ffc247"), order + 2);
            h._solids.Add(head); h._solids.Add(headIn);
            var shaft = ShapeView.Create(arrow, "shaft", new Vector2(6.8f * u, 10.5f * u), 2, order + 2).Radius(.5f * u)
                .Fill(Draw.Hex("#ffc247")).Stroke(Draw.Hex("#fff5dc"), 1.1f * u);
            shaft.transform.localPosition = Draw.W(A(12, 16.9f));
            h._shapes.Add(shaft);

            // hand: SVG 64x64 at S*1.05, its box's top-left at the cell's (.55, .5)
            var hand = new GameObject("Hand").transform;
            hand.SetParent(h._move, false);
            float k = S * 1.05f / 64f;
            hand.localPosition = Draw.W((cell.X + .55f) * S, (cell.Y + .5f) * S);
            Vector2 H(float x, float y) => new Vector2(x * k, y * k);
            // (centre a, centre b, width) capsules approximating the path, plus the palm
            var parts = new (Vector2 a, Vector2 b, float w)[]
            {
                (H(29, 6), H(29, 30), 10 * k),          // index finger
                (H(11, 22), H(11, 36), 10 * k),         // folded finger on the left
                (H(36, 40), H(45.5f, 23.5f), 10 * k),   // thumb
            };
            Rect palm = new Rect(6 * k, 27 * k, 33 * k, 25 * k);
            float outline = 3.2f * k;
            for (int pass = 0; pass < 2; pass++)
            {
                bool dark = pass == 0;
                var col = dark ? Draw.Hex("#22484a") : Draw.Hex("#ffffff");
                float grow = dark ? outline : 0;
                int o = order + 4 + pass;
                var p = ShapeView.Create(hand, "palm", palm.size + Vector2.one * grow * 2, 4, o)
                    .Radius(12 * k + grow).Fill(col);
                p.transform.localPosition = Draw.W(palm.center);
                if (dark) p.Shadow(Draw.Rgba(0, 0, 0, .5f), 0, 4 * k, 7 * k);
                h._shapes.Add(p);
                foreach (var (a, b, w) in parts)
                {
                    var d = b - a;
                    var c = ShapeView.Create(hand, "part", new Vector2(d.magnitude + w + grow * 2, w + grow * 2), 4, o)
                        .Radius(w / 2 + grow).Fill(col);
                    c.transform.localPosition = Draw.W((a + b) / 2);
                    c.transform.localRotation = Quaternion.Euler(0, 0, -Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg);
                    h._shapes.Add(c);
                }
            }
            h.Apply(0);
            return h;
        }

        ShapeView _glow;

        void Update()
        {
            _t = (_t + Time.unscaledDeltaTime) % Period;
            Apply(_t / Period);
        }

        /* phase 0..1 of the nudge cycle */
        void Apply(float phase)
        {
            float p = phase < Peak ? Ease(phase / Peak) : 1 - Ease((phase - Peak) / (1 - Peak));
            _move.localPosition = Draw.W(_nudge * p);
            float a = .75f + .25f * p;
            foreach (var s in _shapes) if (s != null && s != _glow) s.Alpha(a);
            foreach (var s in _solids) if (s != null) s.Alpha(a);
        }

        static float Ease(float x) => x * x * (3 - 2 * x);     // CSS ease-in-out, closely

        public void Remove()
        {
            if (_glow != null) Destroy(_glow.gameObject);
            Destroy(gameObject);
        }
    }
}
