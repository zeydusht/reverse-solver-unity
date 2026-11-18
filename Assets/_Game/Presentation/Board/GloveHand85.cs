using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_85 §1: the guide's hand, the default: p20's glove (&el=d's drawing, hand-glove-back / -press) with TASK41's
       move (&el=a), one loop:
         .45 s  in from the right, it settles over the piece, leaning across it (the fingertip on the piece)
         .12 s  presses: the pressed glove, a little smaller, a small ring under the fingertip
         .25 s  holds
         .70 s  drags the way out, the piece's ghost going with it (the real piece stays home)
         .15 s  stops
         .30 s  lifts and fades
         1.8 s  nothing, then again
       It waits for the arrow (GuideArrow85) to open the way first (delay). Only graphics. */
    public sealed class GloveHand85 : MonoBehaviour
    {
        const float In = .45f, Press = .12f, Hold = .25f, Drag = .7f, Stop = .15f, Lift = .3f, Rest = 1.8f;
        const float Cycle = In + Press + Hold + Drag + Stop + Lift + Rest;
        const float Lean = 24f;                                       // degrees, the wrist to the lower right
        static readonly Vector2 Tip = new Vector2(128f / 256, 6f / 360);   // the fingertip in the art, from its top left

        SpriteRenderer _r;
        Sprite _up, _down;
        ShapeView _ring;
        MeshRenderer _ghost;
        Transform _ghostT;
        Vector2 _tip, _drag;
        float _born, _height;

        public static GloveHand85 Show(Transform parent, BoardView board, Vector2 screen, Move move, int order, float delay = 0)
        {
            var up = Resources.Load<Sprite>("Art/hand-glove-back");
            if (up == null) return null;
            var h = new GameObject("PM85 glove").AddComponent<GloveHand85>();
            h.transform.SetParent(parent, false);
            h._up = up; h._down = Resources.Load<Sprite>("Art/hand-glove-back-press") ?? up;
            h._born = Time.unscaledTime + delay;
            float s = board.Cell;
            var cells = board.Session.Level.Pieces[move.Piece];
            float x0 = float.MaxValue, y0 = float.MaxValue, x1 = 0, y1 = 0;
            foreach (var c in cells) { x0 = Mathf.Min(x0, c.X * s); y0 = Mathf.Min(y0, c.Y * s); x1 = Mathf.Max(x1, (c.X + 1) * s); y1 = Mathf.Max(y1, (c.Y + 1) * s); }
            h._tip = board.Origin + new Vector2((x0 + x1) / 2, (y0 + y1) / 2);
            // the drag: on to the board's edge (at least a cell, at most two and a half)
            var dir = new Vector2(move.Dir.Dx(), move.Dir.Dy());
            float room = move.Dir switch
            {
                Dir.L => h._tip.x - board.Origin.x,
                Dir.R => board.Origin.x + board.Size.x - h._tip.x,
                Dir.U => h._tip.y - board.Origin.y,
                _ => board.Origin.y + board.Size.y - h._tip.y
            };
            h._drag = dir * Mathf.Clamp(room, s, s * 2.5f);
            h._height = Mathf.Clamp(s * 1.55f, 92, 150);
            h._r = Draw.Child<SpriteRenderer>(h.transform, "glove");
            h._r.sprite = up; h._r.sortingOrder = order;
            h._ring = ShapeView.Create(h.transform, "press ring", Vector2.one * s * .7f, 6, order - 1).Radius(s * .35f)
                .Fill(Draw.Rgba(255, 243, 214, 0)).Stroke(Draw.Rgba(255, 236, 190, .95f), 3f);
            h._ring.transform.localPosition = Draw.W(h._tip);
            // the piece's ghost slides with the hand (the real one stays home); over the dark, under the hand
            var pv = board.PieceViewOf(move.Piece);
            if (pv != null)
            {
                h._ghostT = new GameObject("ghost root").transform;
                h._ghostT.SetParent(board.transform, false);
                h._ghost = pv.Ghost(h._ghostT, order - 4);
                PieceView.SetGhostAlpha(h._ghost, 0);
            }
            h.Update();
            return h;
        }

        static float Ease(float t) { t = Mathf.Clamp01(t); return t * t * (3 - 2 * t); }

        void Update()
        {
            float age = Time.unscaledTime - _born;
            if (age < 0) { _r.color = new Color(1, 1, 1, 0); _ring.Alpha(0); if (_ghost != null) PieceView.SetGhostAlpha(_ghost, 0); return; }
            float t = Mathf.Repeat(age, Cycle);
            float t1 = In, t2 = t1 + Press, t3 = t2 + Hold, t4 = t3 + Drag, t5 = t4 + Stop, t6 = t5 + Lift;
            Vector2 tip = _tip; float alpha = 1, scale = 1, lean = Lean, ghostA = 0;
            bool pressed = false;
            if (t < t1)
            {
                // in from the right and a little below, settling (out-cubic), leaning a bit more on the way
                float e = 1 - Mathf.Pow(1 - t / In, 3);
                tip += new Vector2(_height * .9f, _height * .35f) * (1 - e);
                alpha = Mathf.Clamp01(t / (In * .5f));
                lean = Lean + 10 * (1 - e);
            }
            else if (t < t2) { float k = (t - t1) / Press; scale = 1 - .07f * Ease(k); pressed = k > .4f; }
            else if (t < t4)
            {
                scale = .93f; pressed = true;
                float k = t < t3 ? 0 : Ease((t - t3) / Drag);
                tip += _drag * k;
                ghostA = .6f;
            }
            else if (t < t5) { scale = .93f; pressed = true; tip += _drag; ghostA = .6f * (1 - (t - t4) / Stop * .3f); }
            else if (t < t6)
            {
                float k = (t - t5) / Lift;
                tip += _drag + new Vector2(0, -14) * Ease(k);
                scale = Mathf.Lerp(.93f, 1.04f, Ease(k));
                alpha = 1 - Ease(k);
                ghostA = .42f * (1 - Ease(k * 1.6f));
            }
            else alpha = 0;
            _r.sprite = pressed ? _down : _up;
            float k2 = _height / _r.sprite.bounds.size.y * scale;
            _r.transform.localScale = new Vector3(k2, k2, 1);
            _r.transform.localRotation = Quaternion.Euler(0, 0, lean);
            // the pivot is the sprite's centre: turn the fingertip offset with the hand and put the tip on the point
            var b = _r.sprite.bounds.size * k2;
            var off = new Vector2((.5f - Tip.x) * b.x, (.5f - Tip.y) * b.y);   // layout (y down): centre from the tip
            float a = -lean * Mathf.Deg2Rad;                                    // layout's y is down: the turn flips
            var rot = new Vector2(off.x * Mathf.Cos(a) - off.y * Mathf.Sin(a), off.x * Mathf.Sin(a) + off.y * Mathf.Cos(a));
            _r.transform.localPosition = Draw.W(tip + rot);
            _r.color = new Color(1, 1, 1, alpha);
            // the ring: out of the fingertip at the press, gone by the drag's start
            float rk = (t - t1) / (Press + Hold);
            _ring.transform.localPosition = Draw.W(_tip);
            _ring.transform.localScale = Vector3.one * Mathf.Lerp(.45f, 1.1f, Ease(rk));
            _ring.Alpha(rk > 0 && rk < 1 ? (1 - rk) * .95f : 0);
            if (_ghost != null)
            {
                _ghostT.localPosition = Draw.W(_drag * (t < t3 ? 0 : t < t4 ? Ease((t - t3) / Drag) : 1));
                PieceView.SetGhostAlpha(_ghost, ghostA);
            }
        }

        public void Remove() { if (this != null) Destroy(gameObject); }

        void OnDestroy() { if (_ghostT != null) Destroy(_ghostT.gameObject); }
    }
}
