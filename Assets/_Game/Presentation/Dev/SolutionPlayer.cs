#if UNITY_EDITOR
using System.Collections;
using ReverseSolver.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace ReverseSolver.Presentation.Dev
{
    /* Plays the level on screen through its stored solution with a virtual
       mouse, the same input path a player's finger takes (Input System ->
       GameRoot -> DragModel -> GameSession). Used to check end to end that a
       level-editor design can be won in the game. Result goes to Result and
       the log. Editor only. */
    public sealed class SolutionPlayer : MonoBehaviour
    {
        public static string Result { get; private set; }

        public static void Run()
        {
            Result = null;
            new GameObject("SolutionPlayer").AddComponent<SolutionPlayer>();
        }

        IEnumerator Start()
        {
            var mouse = InputSystem.AddDevice<Mouse>("SolutionMouse");
            yield return null;
            var board = FindFirstObjectByType<BoardView>();
            if (board == null) { Finish(mouse, "no board on screen"); yield break; }
            var s = board.Session;
            float dpr = Host.Current.PixelRatio, screenH = Screen.height / dpr;
            int done = 0;
            foreach (var m in s.Level.Solution)
            {
                if (s.IsOver) break;
                var cell = s.Level.Pieces[m.Piece][0];
                var start = board.Origin + new Vector2((cell.X + .5f) * board.Cell, (cell.Y + .5f) * board.Cell);
                float full = (s.Probe(m.Piece, m.Dir).Cells + DragModel.ExitOvershoot) * board.Cell;
                var step = new Vector2(m.Dir.Dx(), m.Dir.Dy());
                yield return Send(mouse, start, screenH, dpr, true);
                for (int k = 1; k <= 6; k++) yield return Send(mouse, start + step * (full * k / 6), screenH, dpr, true);
                yield return Send(mouse, start + step * full, screenH, dpr, false);
                yield return new WaitForSecondsRealtime(.3f);
                if (s.Board.IsPresent(m.Piece)) { Finish(mouse, $"{s.Level.Id}: move {done + 1} ({m}) did not remove the piece"); yield break; }
                done++;
            }
            yield return new WaitForSecondsRealtime(.8f);
            Finish(mouse, $"{s.Level.Id}: {done} moves by drag, outcome {s.Outcome}, time left {s.TimeLeft}s");
        }

        void Finish(Mouse mouse, string result)
        {
            Result = result;
            Debug.Log("[solution] " + result);
            InputSystem.RemoveDevice(mouse);
            Destroy(gameObject);
        }

        static IEnumerator Send(Mouse m, Vector2 pt, float screenH, float dpr, bool down)
        {
            m.MakeCurrent();
            var px = new Vector2(pt.x * dpr, (screenH - pt.y) * dpr);
            InputSystem.QueueStateEvent(m, new MouseState { position = px }.WithButton(MouseButton.Left, down));
            yield return null;
            yield return null;
        }
    }
}
#endif
