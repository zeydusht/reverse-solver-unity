#if UNITY_EDITOR
using System.Collections;
using System.Text;
using ReverseSolver.Core;
using ReverseSolver.Presentation;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace ReverseSolver.Presentation.Dev
{
    /* Play-mode check of the real input path (Input System -> GameRoot ->
       DragModel -> BoardView): a virtual mouse drags pieces of the current
       level out, frame by frame. Checks that the piece is exactly under the
       finger on every frame (no smoothing), that a short drag springs back
       and that a full drag removes the piece. Started from the editor with
       DragSmokeTest.Run(); the result goes to the log. */
    public sealed class DragSmokeTest : MonoBehaviour
    {
        public static string Result { get; private set; }

        public static void Run()
        {
            Result = null;
            new GameObject("DragSmokeTest").AddComponent<DragSmokeTest>();
        }

        IEnumerator Start()
        {
            var sb = new StringBuilder();
            var mouse = InputSystem.AddDevice<Mouse>("SmokeMouse");
            mouse.MakeCurrent();
            yield return null;

            var root = FindFirstObjectByType<GameRoot>();
            if (FindFirstObjectByType<BoardView>() == null)
            {
                // the game opens on the menu: start level 1 the way "Oyna" does
                typeof(GameRoot).GetMethod("StartLevel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .Invoke(root, new object[] { 0 });
                yield return null;
                yield return null;
            }
            var board = FindFirstObjectByType<BoardView>();
            var s = board.Session;
            float dpr = Host.Current.PixelRatio;
            float screenH = Screen.height / dpr;
            int before = s.Board.PresentCount;
            float worst = 0;
            int frames = 0;

            // first free, un-nailed piece and its exit direction
            int piece = -1; Dir dir = Dir.U;
            foreach (var p in TravelRule.FreePieces(s.Board))
                if (s.NailAt(p) <= 0) { piece = p; dir = TravelRule.ExitDirs(s.Board, p)[0]; break; }
            var cell = s.Level.Pieces[piece][0];
            var start = board.Origin + new Vector2((cell.X + .5f) * board.Cell, (cell.Y + .5f) * board.Cell);
            var pv = board.transform.Find($"Piece {piece}");
            var lim = s.Probe(piece, dir);
            float full = (lim.Cells + DragModel.ExitOvershoot) * board.Cell;

            // 1. short drag (30% of the exit distance): must spring back
            yield return Press(mouse, start, screenH, dpr, true);
            for (int k = 1; k <= 6; k++)
            {
                var at = start + new Vector2(dir.Dx(), dir.Dy()) * (full * .30f * k / 6);
                yield return Move(mouse, at, screenH, dpr, true);
                yield return null;
                worst = Mathf.Max(worst, Lag(pv, at - start, dir));
                frames++;
            }
            yield return Press(mouse, start + new Vector2(dir.Dx(), dir.Dy()) * full * .30f, screenH, dpr, false);
            yield return new WaitForSecondsRealtime(.35f);
            bool sprangBack = s.Board.IsPresent(piece) && pv != null && pv.localPosition.sqrMagnitude < .01f;

            // 2. full drag: must leave
            yield return Press(mouse, start, screenH, dpr, true);
            for (int k = 1; k <= 10; k++)
            {
                var at = start + new Vector2(dir.Dx(), dir.Dy()) * (full * k / 10);
                yield return Move(mouse, at, screenH, dpr, true);
                yield return null;
                if (k < 10) { worst = Mathf.Max(worst, Lag(pv, at - start, dir)); frames++; }
            }
            yield return Press(mouse, start + new Vector2(dir.Dx(), dir.Dy()) * full, screenH, dpr, false);
            yield return new WaitForSecondsRealtime(.4f);
            bool removed = !s.Board.IsPresent(piece) && s.Board.PresentCount == before - 1;

            sb.Append($"level {s.Level.Id} piece {piece} {dir}: ");
            sb.Append($"follow error max {worst:0.00} pt over {frames} frames; ");
            sb.Append($"short drag sprang back {sprangBack}; full drag removed {removed}; jams {s.Jams}");
            Result = sb.ToString();
            Debug.Log("[smoke] " + Result);
            InputSystem.RemoveDevice(mouse);
            Destroy(gameObject);
        }

        /* Distance between where the finger is (relative to the press) and where
           the piece is drawn. Inside the 7pt dead zone the piece stays put by
           design (web MIN_SWIPE), so those frames do not count. */
        static float Lag(Transform pv, Vector2 fingerDelta, Dir dir)
        {
            if (fingerDelta.magnitude < DragModel.MinSwipe) return 0;
            var drawn = new Vector2(pv.localPosition.x, -pv.localPosition.y);
            var along = new Vector2(dir.Dx(), dir.Dy());
            float finger = Vector2.Dot(fingerDelta, along);
            return Mathf.Abs(Vector2.Dot(drawn, along) - Mathf.Max(0, finger));
        }

        static IEnumerator Press(Mouse m, Vector2 pt, float screenH, float dpr, bool down)
        {
            m.MakeCurrent();     // the editor's real mouse can take Pointer.current back
            InputSystem.QueueStateEvent(m, new MouseState { position = ToPx(pt, screenH, dpr) }.WithButton(MouseButton.Left, down));
            yield return null;
        }

        static IEnumerator Move(Mouse m, Vector2 pt, float screenH, float dpr, bool down)
        {
            m.MakeCurrent();
            InputSystem.QueueStateEvent(m, new MouseState { position = ToPx(pt, screenH, dpr) }.WithButton(MouseButton.Left, down));
            yield return null;
        }

        static Vector2 ToPx(Vector2 pt, float screenH, float dpr) => new Vector2(pt.x * dpr, (screenH - pt.y) * dpr);
    }
}
#endif
