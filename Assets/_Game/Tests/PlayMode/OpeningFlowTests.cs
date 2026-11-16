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

        [SetUp]
        public void ConfigureInputAndHost()
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

        /* PM_72: whatever the link, level 1 is the PM_60 story opening: the Jigo chat box and the gold glow; no hand, no
           big arrow, no darkening. The device carries an old P21_T01 win (an &tut=2x2 visit), which used to bring the
           prelude with the hand back on every visit without reset=1. */
        [UnityTest]
        public IEnumerator LevelOneIsTheStoryOpeningOnEveryLink()
        {
            var links = new[] { "c=p22", "c=p22&reset=1", "c=p22&test=1", "", "c=p22&tut=2x2", "c=p22&tut2x2=1" };
            foreach (string link in links)
            {
                _host.Links = link;
                _host.Store = new MemoryStore();
                var save = new SaveData(_host.Store, () => "seed");
                save.SetPlayer("QA");
                save.MergeBest(new System.Collections.Generic.Dictionary<string, int> { [P21Onboarding.TutorialId] = 3 });
                var old = UnityEngine.Object.FindAnyObjectByType<GameRoot>();
                if (old != null) { UnityEngine.Object.Destroy(old.gameObject); yield return null; }
                yield return SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
                yield return WaitFor(() => MenuPlay != null, $"the menu play button ({link})");
                yield return Click(LayoutPoint(MenuPlay));
                yield return WaitFor(() => Board != null && Board.Session.Level.Number == 1, $"the first board ({link})");
                yield return new WaitForSecondsRealtime(.6f);
                string id = Board.Session.Level.Id;
                Assert.That(id, Is.Not.EqualTo(P21Onboarding.TutorialId).And.Not.EqualTo(P21Onboarding.EasyId), link);
                Assert.That(UnityEngine.Object.FindObjectsByType<TutorialHand>(FindObjectsSortMode.None), Is.Empty, $"old hand ({link})");
                Assert.That(UnityEngine.Object.FindObjectsByType<TutorialSpotlight>(FindObjectsSortMode.None), Is.Empty, $"spotlight ({link})");
                foreach (string name in new[] { "Veil", "FingerPivot", "ArrowShaft", "ArrowHead", "Swipe path", "guide", "P21 tutorial" })
                    Assert.That(GameObject.Find(name), Is.Null, $"{name} ({link})");
                var hands = UnityEngine.Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None)
                    .Where(r => r.sprite != null && (r.sprite.name.ToLowerInvariant().Contains("hand") || r.sprite.name.ToLowerInvariant().Contains("glove")));
                Assert.That(hands.Select(r => r.sprite.name), Is.Empty, $"hand sprites ({link})");
                var story = GameObject.Find("P21 story");
                Assert.That(story, Is.Not.Null, $"chat box ({link})");
                Assert.That(story.transform.Find("chat").gameObject.activeSelf, Is.True, $"chat box shown ({link})");
                Assert.That(story.GetComponent<P21TutorialView>().Glowing, Is.True, $"gold glow on the target piece ({link})");
            }
        }

        [UnityTest]
        public IEnumerator ScissorsTargetingAndFlightKeepTheClockRunning()
        {
            yield return OpenTimedBooster("makas");
            var board = Board;
            var session = board.Session;
            var target = (from y in Enumerable.Range(0, session.Level.Height)
                          from x in Enumerable.Range(1, session.Level.Width - 1)
                          let point = board.Origin + new Vector2(x, y + .5f) * board.Cell
                          where board.JointAt(point).HasValue
                          select point).First();
            int before = session.TimeLeft;
            yield return WaitFor(() => session.TimeLeft < before, "the countdown during scissors targeting");
            int stock = session.Stock(Booster.Scissors);
            int afterTargeting = session.TimeLeft;
            yield return Click(target);
            yield return WaitFor(() => session.Stock(Booster.Scissors) < stock, "the scissors cutting the joint");
            // PM_72: the flight is shorter than a second; the clock keeps running through and after the cut
            yield return WaitFor(() => session.TimeLeft < afterTargeting, "the countdown after the cut");
            Assert.That(GameObject.Find("P21 boosters").transform.Find("trail").GetComponent<SpriteRenderer>().enabled, Is.False);
        }

        [UnityTest]
        public IEnumerator HammerTargetingTimesOutWithoutSpendingStock()
        {
            yield return OpenTimedBooster("cekic");
            var session = Board.Session;
            int stock = session.Stock(Booster.Hammer);
            session.Tick(session.TimeLeft - 1);
            yield return WaitFor(() => session.Outcome == Outcome.Time, "a timeout with hammer targeting open");
            Assert.That(session.Stock(Booster.Hammer), Is.EqualTo(stock));
        }

        [UnityTest]
        public IEnumerator ClockShieldFreezesOnContactAndKeepsThePortraitVisible()
        {
            yield return OpenTimedBooster("cekic");
            var session = Board.Session;
            session.TestBoosterUnlock = Booster.Clock;
            session.SetStock(Booster.Clock, 1);
            var clockSlot = UnityEngine.Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None)
                .Single(r => r.sortingOrder == 3203 && r.sprite != null && r.sprite.name == "clock");
            Assert.That(session.FreezeLeft, Is.Zero);
            yield return Click(LayoutPoint(clockSlot.transform));
            yield return WaitFor(() => session.FreezeLeft > 0, "the flying clock reaching the timer");
            int frozenAt = session.TimeLeft;
            // PM_74: the alarm clock rings over the Time box and glazes it; Jigo keeps his portrait
            yield return new WaitForSecondsRealtime(.3f);
            var glaze = GameObject.Find("fx glaze").GetComponent<SpriteRenderer>();
            Assert.That(glaze.enabled, Is.True, "the Time box glazed while frozen");
            yield return WaitFor(() => session.FreezeLeft < 9.5, "the freeze bar decreasing");
            Assert.That(session.TimeLeft, Is.EqualTo(frozenAt));
            Assert.That(session.FreezeLeft, Is.GreaterThan(0));
            Assert.That(GameObject.Find("fx freeze chip text").activeInHierarchy, Is.True, "the +10 s chip under the Time box");
            yield return new WaitForSecondsRealtime(.5f);
            Assert.That(GameObject.Find("fx clock").GetComponent<SpriteRenderer>().enabled, Is.False, "the flying clock is gone after 1.35 s");
            Assert.That(session.FreezeLeft, Is.GreaterThan(0));
            session.Tick(session.FreezeLeft);
            yield return WaitFor(() => session.TimeLeft < frozenAt, "the countdown resuming after thaw");
        }

        /* PM_74: on the test link (stock 999) a booster lesson used to stay on its "tap the booster" step after the use,
           and that step takes every tap: no piece, booster or pause could be pressed. After the use the game is back to
           normal input, the stock is unchanged, and one tap off the board cancels a booster selection. */
        [UnityTest]
        public IEnumerator BoosterLessonOnTheTestLinkGivesInputBack()
        {
            foreach (var (level, booster) in new[] { (9, Booster.Wand), (15, Booster.Clock) })
            {
                _host.Links = $"c=p22&test=1&lv={level}";
                _host.Store = new MemoryStore();
                var old = UnityEngine.Object.FindAnyObjectByType<GameRoot>();
                if (old != null) { UnityEngine.Object.Destroy(old.gameObject); yield return null; }
                yield return SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
                yield return WaitFor(() => Board != null && Board.Session.Level.Number == level, $"level {level}");
                yield return new WaitForSecondsRealtime(.5f);
                var card = Field<Card>("_card");
                if (card != null) { yield return Click(card.Buttons[0].rect.center); yield return WaitFor(() => Field<Card>("_card") == null, "the lesson card closed"); }
                yield return new WaitForSecondsRealtime(.8f);
                var session = Board.Session;
                var bar = Field<BoosterBar>("_bar");
                int stock = session.Stock(booster);
                Assert.That(Prop("TutorialActive"), Is.True, $"the {booster} lesson");
                yield return Click(bar.RectOf(booster).center);
                if (booster == Booster.Wand)
                {
                    yield return WaitFor(() => Field<Booster?>("_selected21") == Booster.Wand,
                        $"the wand selection (hit {bar.Hit(bar.RectOf(booster).center)}, step {Field<int>("_tutorialAt")}, card {Field<string>("_cardKind")}, intro {Field<bool>("_introOpen")})");
                    yield return new WaitForSecondsRealtime(.4f);
                    var view = Field<object>("_boosterView21");
                    var regions = Field<bool[]>("_regions21");
                    int zone = Field<int>("_region21");
                    if (zone < 0)
                    {
                        zone = Array.IndexOf(regions, true);
                        Assert.That(zone, Is.GreaterThanOrEqualTo(0), "a wand region");
                        var rect = (Rect)view.GetType().GetMethod("ZoneRect").Invoke(view, new object[] { zone });
                        yield return Click(rect.center);
                        yield return WaitFor(() => Field<int>("_region21") == zone, "the region chosen");
                        yield return new WaitForSecondsRealtime(.3f);
                    }
                    yield return Click(((Rect)view.GetType().GetProperty("ConfirmRect").GetValue(view)).center);
                }
                yield return WaitFor(() => session.Used(booster) > 0 || Field<Booster?>("_effect21") == null && !Prop("TutorialActive"), $"the {booster} used");
                float until = Time.realtimeSinceStartup + 4f;
                while ((Prop("TutorialActive") || Field<Booster?>("_effect21") != null) && Time.realtimeSinceStartup < until) yield return null;
                Assert.That(Prop("TutorialActive"), Is.False, $"the {booster} lesson ended after the use");
                Assert.That(session.Stock(booster), Is.EqualTo(stock), "the test stock is unchanged");
                // normal input: the hammer arms, one tap off the board disarms it
                yield return Click(bar.RectOf(Booster.Hammer).center);
                yield return WaitFor(() => Field<Booster?>("_selected21") == Booster.Hammer, $"the hammer selected after the {booster} lesson");
                yield return Click(new Vector2(Board.Origin.x + Board.Size.x / 2, Board.Origin.y - 30));
                yield return WaitFor(() => Field<Booster?>("_selected21") == null, "one tap off the board cancels the hammer");
                var clockSlot = bar.RectOf(Booster.Clock);
                var pause = Enumerable.Range(0, 200).Select(i => new Vector2(clockSlot.xMax + i, clockSlot.center.y))
                    .First(pt => bar.HitAction(pt) == BarAction.Pause);
                yield return Click(pause);
                yield return WaitFor(() => Field<string>("_cardKind") == "pause", "the pause button opens the pause card");
                // PM_76: the panel opens and closes with a short motion; Resume really takes it off the screen
                yield return new WaitForSecondsRealtime(.6f);
                var pauseCard = Field<Card>("_card");
                var resume = pauseCard.Buttons.Find(button => button.id == "resume");
                yield return Click(resume.rect.center);
                yield return WaitFor(() => Field<Card>("_card") == null, "Resume closes the pause card");
                float gone = Time.realtimeSinceStartup + 1f;
                while (GameObject.Find("Pause") != null && Time.realtimeSinceStartup < gone) yield return null;
                Assert.That(GameObject.Find("Pause"), Is.Null, "the pause panel leaves the screen within a second");
            }
        }

        /* PM_75/PM_76: on the test link the wand works. Levels 9, 15, 23 and ten more: even at the start (pieces
           packed) it is lit and a press offers a region that frees at least one piece (PM_76: by flattening a piece's
           tabs). With &degnek=eski (PM_75) it is dimmed at the start, a press explains and selects nothing; pieces are
           taken out along the solution until it lights up, then it works the same way. */
        [UnityTest]
        public IEnumerator WandWorksOrExplainsOnTheTestLink()
        {
            var random = new System.Random(75);
            var levels = new System.Collections.Generic.List<int> { 9, 15, 23 };
            while (levels.Count < 13) { int n = random.Next(10, 41); if (!levels.Contains(n)) levels.Add(n); }
            int used = 0, flattenedAtStart = 0;
            var report = new System.Text.StringBuilder();
            var runs = new System.Collections.Generic.List<(int level, bool old)> { (9, true) };
            foreach (int n in levels) runs.Add((n, false));
            foreach (var (level, oldWand) in runs)
            {
                _host.Links = $"c=p23&test=1&lv={level}" + (oldWand ? "&degnek=eski" : "");
                _host.Store = new MemoryStore();
                var old = UnityEngine.Object.FindAnyObjectByType<GameRoot>();
                if (old != null) { UnityEngine.Object.Destroy(old.gameObject); yield return null; }
                yield return SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
                yield return WaitFor(() => Board != null && Board.Session.Level.Number == level, $"level {level}");
                yield return new WaitForSecondsRealtime(.5f);
                var card = Field<Card>("_card");
                if (card != null) { yield return Click(card.Buttons[0].rect.center); yield return WaitFor(() => Field<Card>("_card") == null, "the lesson card closed"); }
                // a booster lesson (9 wand, 15 clock, 23 hammer) is not what this test is about: end it
                var root = UnityEngine.Object.FindAnyObjectByType<GameRoot>();
                var steps = (System.Collections.IList)typeof(GameRoot).GetField("_tutorialSteps", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(root);
                typeof(GameRoot).GetField("_tutorialAt", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(root, steps.Count);
                typeof(GameRoot).GetMethod("ShowScriptedTutorial", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(root, null);
                var session = Board.Session;
                session.FreeUse = null;
                yield return new WaitForSecondsRealtime(.6f);
                var bar = Field<BoosterBar>("_bar");
                if (!oldWand)
                {
                    Assert.That(session.WandCanHelp(), Is.True, $"level {level}: PM_76: the wand can help even on a packed board");
                    Assert.That(bar.Idle, Is.Null, $"level {level}: the wand is lit at the start");
                }
                else
                {
                Assert.That(session.WandCanHelp(), Is.False, $"level {level}: no tab change frees a piece while the board is packed");
                Assert.That(bar.Idle, Is.EqualTo((Booster?)Booster.Wand), $"level {level}: the wand looks dimmed at the start");
                yield return Click(bar.RectOf(Booster.Wand).center);
                yield return null;
                Assert.That(Field<Booster?>("_selected21"), Is.Null, $"level {level}: a dimmed wand selects nothing");
                Assert.That(GameObject.Find("Booster explanation"), Is.Not.Null, $"level {level}: the press explains (hit {bar.Hit(bar.RectOf(Booster.Wand).center)}, " +
                    $"card {Field<string>("_cardKind")}, mode {typeof(GameRoot).GetField("_mode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(root)}, " +
                    $"tutorial {Prop("TutorialActive")}, over {session.IsOver}, intro {Field<bool>("_introOpen")}, stock {session.Stock(Booster.Wand)}, unlocked {session.IsUnlocked(Booster.Wand)}, spot {Field<object>("_spotlight") != null}, " +
                    $"diving {Field<bool>("_diving")}, effect {Field<Booster?>("_effect21")}, committed {Field<bool>("_committedTouch")}, cardTap {Field<object>("_cardTap") != null}, gateRelease {Field<bool>("_gateRelease")})");
                }
                // take pieces out along the solution until the wand can help
                var plan = SmartPlayer.Play(session.Level).Moves;
                int at = 0;
                while (!session.WandCanHelp() && at < plan.Count && session.Board.PresentCount > 2 && !session.IsOver)
                {
                    session.TryExit(plan[at].Piece, plan[at].Dir); at++;
                    yield return null;
                }
                yield return new WaitForSecondsRealtime(.4f);
                if (!session.WandCanHelp()) { report.Append($" {level}:never"); continue; }
                Assert.That(bar.Idle, Is.Null, $"level {level}: the wand is lit once it can help ({at} pieces out)");
                yield return Click(bar.RectOf(Booster.Wand).center);
                yield return WaitFor(() => Field<Booster?>("_selected21") == Booster.Wand, $"level {level}: the wand selection");
                yield return new WaitForSecondsRealtime(.3f);
                var view = Field<object>("_boosterView21");
                int zone = Field<int>("_region21");
                if (zone < 0)
                {
                    zone = Array.IndexOf(Field<bool[]>("_regions21"), true);
                    Assert.That(zone, Is.GreaterThanOrEqualTo(0), $"level {level}: a wand region");
                    var rect = (Rect)view.GetType().GetMethod("ZoneRect").Invoke(view, new object[] { zone });
                    yield return Click(rect.center);
                    yield return WaitFor(() => Field<int>("_region21") == zone, "the region chosen");
                    yield return new WaitForSecondsRealtime(.3f);
                }
                yield return Click(((Rect)view.GetType().GetProperty("ConfirmRect").GetValue(view)).center);
                yield return WaitFor(() => session.WandFreed.Count > 0, $"level {level}: the wand freed a piece");
                float until = Time.realtimeSinceStartup + 4f;
                while (Field<Booster?>("_effect21") != null && Time.realtimeSinceStartup < until) yield return null;
                Assert.That(Field<Booster?>("_effect21"), Is.Null, $"level {level}: the wand effect ends");
                used++;
                if (!oldWand && at == 0 && session.WandFlattened.Count > 0) flattenedAtStart++;
                report.Append($" {level}{(oldWand ? "(eski)" : "")}:{at}out/q{zone}/freed{session.WandFreed.Count}/flat{session.WandFlattened.Count}");
            }
            Debug.Log("[wand76] play mode:" + report);
            Assert.That(used, Is.GreaterThanOrEqualTo(13), "the wand worked on the 13 levels (and the old one once pieces were out)");
            Assert.That(flattenedAtStart, Is.EqualTo(13), "PM_76: at the start of each level it flattened a piece's tabs");
        }

        static T Field<T>(string name)
        {
            var root = UnityEngine.Object.FindAnyObjectByType<GameRoot>();
            var f = typeof(GameRoot).GetField(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (T)f.GetValue(root);
        }
        static bool Prop(string name)
        {
            var root = UnityEngine.Object.FindAnyObjectByType<GameRoot>();
            return (bool)typeof(GameRoot).GetProperty(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(root);
        }

        IEnumerator OpenTimedBooster(string booster)
        {
            _host.Booster = booster;
            yield return SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
            yield return WaitFor(() => Board != null && Board.Session.Level.Id == "S10", "the timed booster test board");
            yield return new WaitForSecondsRealtime(.3f);
            var session = Board.Session;
            Assert.That(session.RecordJam(0), Is.EqualTo(CommandResult.Ok));
            Assert.That(session.ClockStarted, Is.True);
        }

        // PM_74: the menu's one-time warm-up board (under a SortingGroup, two frames) is not the game's board
        static BoardView Board => UnityEngine.Object.FindObjectsByType<BoardView>(FindObjectsSortMode.None)
            .FirstOrDefault(b => b.GetComponentInParent<UnityEngine.Rendering.SortingGroup>() == null);
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
            public string Booster;
            public string Links;   // a link's query ("c=p22&test=1"); null: the debug QA host
            public string Query(string name) => Links != null ? Link(name) : name switch
            {
                "debug" => "1",
                "lv" when Booster != null => "S10",
                "booster" => Booster,
                _ => null
            };
            public float PixelRatio => 1;
            public Vector4 SafeInsets => Vector4.zero;
            string Link(string name)
            {
                if (name == "intro") return "0";
                foreach (string pair in Links.Split('&'))
                {
                    int eq = pair.IndexOf('=');
                    if (eq > 0 && pair.Substring(0, eq) == name) return pair.Substring(eq + 1);
                }
                return null;
            }
            public IKeyValueStore Store { get; set; } = new MemoryStore();
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
