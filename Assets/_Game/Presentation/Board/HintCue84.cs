using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_84 §1: the hint without the dark: the piece glows gold (the sl100 glow Zeyd liked) and beats like a pulse
       (1.0 <-> 1.06, .8 s), and the arrow (GuideArrow85) runs out of it the way it leaves, nudging twice. A touch
       takes it away at once (Remove). Only graphics. */
    public sealed class HintCue84 : MonoBehaviour
    {
        BoardView _board;
        PieceView _piece;
        GuideArrow85 _arrow;
        float _born;
        public int Piece { get; private set; } = -1;

        public static HintCue84 Show(Transform parent, BoardView board, Vector2 screen, Move move, int order)
        {
            var h = new GameObject("PM84 hint").AddComponent<HintCue84>();
            h.transform.SetParent(parent, false);
            h._board = board; h.Piece = move.Piece; h._born = Time.unscaledTime;
            h._piece = board.PieceViewOf(move.Piece);
            h._arrow = GuideArrow85.ForMove(h.transform, board, screen, move, order, GuideArrow85.Mode.Nudge);
            h.Update();
            return h;
        }

        void Update()
        {
            if (_piece == null) return;
            float p = Mathf.Repeat(Time.unscaledTime - _born, .8f) / .8f;
            float beat = .5f - .5f * Mathf.Cos(p * 2 * Mathf.PI);
            _piece.SetGlow(Draw.Hex("#ffcf59"), .7f + .3f * beat);
            _piece.SetPulse(1 + .06f * beat);
        }

        public void Remove()
        {
            if (this == null) return;
            if (_piece != null) { _piece.SetGlow(Vector4.zero, 0); _piece.SetPulse(1); }
            _piece = null;
            if (_arrow != null) _arrow.FadeOut();
            _arrow = null;
            Destroy(gameObject, .2f);
            enabled = false;
        }
    }
}
