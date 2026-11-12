using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using ReverseSolver.Core;
using ReverseSolver.Presentation;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace ReverseSolver.PlayMode.Tests
{
    public sealed class OpeningFlowTests
    {
        IHost _previousHost;
        LocalHost _host;
        Mouse _mouse;
        InputSettings.UpdateMode _previousUpdateMode;
        InputSettings.BackgroundBehavior _previousBackground;
#if UNITY_EDITOR
        InputSettings.EditorInputBehaviorInPlayMode _previousEditorInput;
#endif

        [UnitySetUp]
        public IEnumerator OpenGame()
        {
            _previousHost = Host.Current;
            Host.Current = _host = new LocalHost();
            var input = InputSystem.settings;
            _previousUpdateMode = input.updateMode;
            _previousBackground = input.backgroundBehavior;
#if UNITY_EDITOR
            _previousEditorInput = input.editorInputBehaviorInPlayMode;
            // Batch mode has no focused Game view.
            input.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
#endif
            input.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
            input.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            _mouse = InputSystem.AddDevice<Mouse>("OpeningTestMouse");
            yield return SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
        }

        [UnityTearDown]
        public IEnumerator RestoreHost()
        {
            if (_mouse != null && _mouse.added) InputSystem.RemoveDevice(_mouse);
            var input = InputSystem.settings;
            input.updateMode = _previousUpdateMode;
            input.backgroundBehavior = _previousBackground;
#if UNITY_EDITOR
            input.editorInputBehaviorInPlayMode = _previousEditorInput;
#endif
            var game = UnityEngine.Object.FindAnyObjectByType<GameRoot>();
            if (game != null) UnityEngine.Object.Destroy(game.gameObject);
            yield return null;
            Host.Current = _previousHost;
        }

        [UnityTest]
        public IEnumerator GuidedSwipesWinAndNextButtonStartsEasyLevel()
        {
            yield return WaitFor(() => MenuPlay != null, "the menu play button");
            yield return Click(LayoutPoint(MenuPlay));
            yield return WaitFor(() => Board != null && Board.Session.Level.Number == 1, "the first board");
            var first = Board;
            var session = first.Session;
            Assert.That((session.Level.Width, session.Level.Height, session.Board.PresentCount), Is.EqualTo((2, 2, 4)));
            Assert.That(first.Cell, Is.GreaterThan(0));
            int initialTime = session.TimeLeft, jams = 0;
            session.EventRaised += e => { if (e.Kind == EventKind.Jammed) jams++; };
            yield return WaitFor(() => GuideText.Contains(Lang.T("glowing piece down", "parçayı aşağı kaydır")), "the first swipe cue");
            yield return Swipe(first, 2, Dir.D);
            Assert.That(session.Moves, Is.EqualTo(1));

            yield return WaitFor(() => GuideText.Contains(Lang.T("piece up", "yukarı kaydırmayı")), "the first blocked swipe cue");
            yield return Swipe(first, 3, Dir.U);
            Assert.That(session.Moves, Is.EqualTo(1));
            Assert.That(jams, Is.EqualTo(1));
            yield return WaitFor(() => GuideText.Contains(Lang.T("bottom is open", "Altı açık")), "the first recovery cue");
            yield return Swipe(first, 3, Dir.D);
            Assert.That(session.Moves, Is.EqualTo(2));

            yield return WaitFor(() => GuideText.Contains(Lang.T("upper-right", "Sağ üstteki")), "the second blocked swipe cue");
            yield return Swipe(first, 1, Dir.L);
            Assert.That(session.Moves, Is.EqualTo(2));
            Assert.That(jams, Is.EqualTo(2));
            yield return WaitFor(() => GuideText.Contains(Lang.T("right side is open", "Sağ taraf açık")), "the second recovery cue");
            yield return Swipe(first, 1, Dir.R);
            Assert.That(session.Moves, Is.EqualTo(3));

            yield return WaitFor(() => GuideText == Texts.P21StoryLast, "the final rescue cue");
            yield return Swipe(first, 0, Dir.L);
            Assert.That(session.Outcome, Is.EqualTo(Outcome.Win));
            Assert.That(session.Jams, Is.Zero);
            Assert.That(session.TimeLeft, Is.EqualTo(initialTime));

            yield return WaitFor(() => Next != null, "the victory screen and its next button");
            yield return new WaitForSecondsRealtime(.3f);
            yield return Click(LayoutPoint(Next));
            yield return WaitFor(() => Board != null && Board.Session.Level.Number == 2, "the easy board via Next");
            var easy = Board;
            var easySession = easy.Session;
            Assert.That((easySession.Level.Width, easySession.Level.Height, easySession.Board.PresentCount), Is.EqualTo((3, 3, 9)));
            Assert.That(easySession.Level.Nails.Count + easySession.Level.Bombs.Count + easySession.Level.Sealed.Count, Is.Zero);
            initialTime = easySession.TimeLeft;
            while (!easySession.IsOver)
            {
                int piece = TravelRule.FreePieces(easySession.Board).Last();
                Dir dir = TravelRule.ExitDirs(easySession.Board, piece)[0];
                int moves = easySession.Moves;
                yield return Swipe(easy, piece, dir);
                Assert.That(easySession.Moves, Is.EqualTo(moves + 1), $"piece {piece} {dir}");
            }
            Assert.That(easySession.Outcome, Is.EqualTo(Outcome.Win));
            Assert.That(easySession.TimeLeft, Is.EqualTo(initialTime));
            var save = new SaveData(_host.Store, () => "unused");
            Assert.That(save.Best.Keys, Is.EquivalentTo(new[] { session.Level.Id, easySession.Level.Id }));
        }

        static BoardView Board => UnityEngine.Object.FindAnyObjectByType<BoardView>();
        static Transform MenuPlay => GameObject.Find("Menu")?.transform.Find("play");
        static Transform Next => GameObject.Find("P21 win and chest")?.transform.Find("surface/next green");
        static string GuideText => GameObject.Find("P21 story")?.GetComponentInChildren<TextView>()?.Text ?? "";
        static Vector2 LayoutPoint(Transform element) => new Vector2(element.position.x, -element.position.y);

        static IEnumerator WaitFor(Func<bool> condition, string description)
        {
            float deadline = Time.realtimeSinceStartup + 6f;
            while (!condition() && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(condition(), Is.True, $"Expected {description}");
        }

        IEnumerator Swipe(BoardView board, int piece, Dir dir)
        {
            var c = board.Session.Level.Pieces[piece][0];
            var start = board.Origin + new Vector2(c.X + .5f, c.Y + .5f) * board.Cell;
            yield return Point(start, true);
            yield return Point(start + new Vector2(dir.Dx(), dir.Dy()) * 20, false);
        }

        IEnumerator Click(Vector2 point)
        {
            yield return Point(point, true);
            yield return Point(point, false);
        }

        IEnumerator Point(Vector2 point, bool down)
        {
            _mouse.MakeCurrent();
            InputSystem.QueueStateEvent(_mouse, new MouseState
            {
                position = new Vector2(point.x, Screen.height - point.y)
            }.WithButton(MouseButton.Left, down));
            yield return null;
            Assert.That(_mouse.leftButton.isPressed, Is.EqualTo(down));
            Assert.That(Pointer.current, Is.SameAs(_mouse));
        }

        sealed class LocalHost : IHost
        {
            public string Query(string name) => name == "debug" ? "1" : null;
            public float PixelRatio => 1;
            public Vector4 SafeInsets => Vector4.zero;
            public IKeyValueStore Store { get; } = new MemoryStore();
            public ITelemetryTransport Transport => null;
            public string SupabaseUrl => null;
            public string AnonKey => null;
            public bool SendEnabled => false;
            public event Action<bool> VisibilityChanged { add { } remove { } }
            public void ReportBoot(string message) { }
            public void AskName(Action<string> done) => done("Local QA");
            public void PlayIntro(string parts, string wake, string skip, string ending, bool sound, Action<string> events) => events("done");
        }
    }
}
