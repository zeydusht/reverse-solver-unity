using System.Linq;
using NUnit.Framework;
using ReverseSolver.Editing;
using ReverseSolver.Generation;

namespace ReverseSolver.Core.Tests
{
    public class P21OnboardingTests
    {
        [Test]
        public void LockBubbleShowsTwicePerVisitAndIgnoresUnlockedPieces()
        {
            var hints = new P21LockHints();
            Assert.That(hints.TryShow(0), Is.False);
            Assert.That(hints.TryShow(-1), Is.False);
            Assert.That(hints.TryShow(3), Is.True);
            Assert.That(hints.TryShow(2), Is.True);
            Assert.That(hints.TryShow(1), Is.False);
            Assert.That(hints.TryShow(9), Is.False);
            Assert.That(new P21LockHints().TryShow(1), Is.True);
        }

        [Test]
        public void LockedPressKeepsMovesAndCountersUnchanged()
        {
            var level = P21Onboarding.Grid("locked-test", 2, 1);
            level.Nails = new System.Collections.Generic.Dictionary<int, int> { [0] = 3 };
            var session = Session(level);
            var drag = new DragModel(new SessionDragTarget(session), 60);
            for (int i = 0; i < 3; i++) Assert.That(drag.Press(0, 30, 30), Is.False);
            Assert.That(session.Moves, Is.Zero);
            Assert.That(session.NailAt(0), Is.EqualTo(3));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void ObstacleAndLockBubbleCopyExistsInBothLanguages(bool turkish)
        {
            bool original = ReverseSolver.Presentation.Lang.Tr;
            try
            {
                ReverseSolver.Presentation.Lang.Tr = turkish;
                foreach (var scene in new[] { "bomb", "nail", "chain" })
                {
                    string text = ReverseSolver.Presentation.Texts.ObstacleIntro(scene);
                    Assert.That(text, Is.Not.Empty);
                    Assert.That(text, Does.Contain("<b>"));
                    Assert.That(text, Does.Contain(turkish ? (scene == "bomb" ? "bitmeden" : scene == "nail" ? "N hamle" : "birlikte")
                        : (scene == "bomb" ? "before" : scene == "nail" ? "N moves" : "together")));
                }
                Assert.That(ReverseSolver.Presentation.Texts.LockWarning(7), Is.EqualTo(turkish ? "Kilit 7 hamle sonra açılacak." : "The lock opens in 7 moves."));
                Assert.That(ReverseSolver.Presentation.Texts.LockIntro(7), Does.Contain(turkish ? "<b>7 hamle</b>" : "<b>7 moves</b>"));
            }
            finally { ReverseSolver.Presentation.Lang.Tr = original; }
        }

        static LevelSet _original;
        static LevelSet Original => _original ??= LevelParser.Parse(System.IO.File.ReadAllText("Assets/_Game/Levels/redesign.json"));
        static LevelSet Flow() => P21Onboarding.Prepend(Original);
        static GameSession Session(LevelData level) => new GameSession(level, 1, "test", new Mulberry32(1));

        [Test]
        public void StoryTargetsOpenUnlockedPiecesForAtMostThreeRounds()
        {
            var level = P21Onboarding.Grid("story", 3, 1);
            level.Nails = new System.Collections.Generic.Dictionary<int, int> { [0] = 4 };
            var s = Session(level);
            var story = new P21Story();
            for (int step = 1; step <= 3; step++)
            {
                story.Refresh(s);
                Assert.That(story.Step, Is.EqualTo(step));
                int target = story.Target;
                Assert.That(target, Is.GreaterThanOrEqualTo(0));
                Assert.That(s.NailAt(target), Is.Zero);
                var exits = TravelRule.ExitDirs(s.Board, target);
                Assert.That(exits, Is.Not.Empty);
                story.Refresh(s);
                Assert.That(story.Target, Is.EqualTo(target), "redraws do not consume glow rounds");
                Assert.That(s.TryExit(target, exits[0]), Is.EqualTo(CommandResult.Ok));
                story.Refresh(s, target);
            }
            Assert.That(story.Step, Is.EqualTo(3));
            Assert.That(story.Target, Is.EqualTo(-1));
        }

        [Test]
        public void StoryClockRunsToZeroWithoutLossAndStillAllowsWinning()
        {
            var s = Session(P21Onboarding.Grid("story-time", 2, 1));
            s.PreventTimeout = true; s.ClockOnFirstMove = false;
            s.Tick(2);
            Assert.That(s.TimeLeft, Is.EqualTo(178));
            s.Tick(1000);
            Assert.That(s.TimeLeft, Is.Zero);
            Assert.That(s.IsOver, Is.False);
            Assert.That(s.ElapsedSeconds, Is.EqualTo(1002));
            var clone = s.Clone();
            clone.Tick(2);
            Assert.That(clone.IsOver, Is.False);
            foreach (int p in new[] { 2, 1, 0, 3 })
                s.TryExit(p, TravelRule.ExitDirs(s.Board, p)[0]);
            Assert.That(s.Outcome, Is.EqualTo(Outcome.Win));
            var normal = Session(s.Level); normal.ClockOnFirstMove = false;
            normal.Tick(1000);
            Assert.That(normal.Outcome, Is.EqualTo(Outcome.Time));
        }

        [Test]
        public void RemovingOtherPiecesDoesNotSpendAnotherGlowRound()
        {
            var s = Session(P21Onboarding.Grid("story-other", 3, 1));
            var story = new P21Story(); story.Refresh(s);
            int target = story.Target;
            int other = TravelRule.FreePieces(s.Board).First(p => p != target);
            s.TryExit(other, TravelRule.ExitDirs(s.Board, other)[0]);
            story.Refresh(s, other);
            Assert.That(story.Target, Is.EqualTo(target));
            Assert.That(story.Step, Is.EqualTo(1));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void StoryCopyIsLocalized(bool turkish)
        {
            bool original = ReverseSolver.Presentation.Lang.Tr;
            try
            {
                ReverseSolver.Presentation.Lang.Tr = turkish;
                Assert.That(ReverseSolver.Presentation.Texts.P21StoryStart, Is.EqualTo(turkish
                    ? "Bu Jiggle'ları kurtaralım! Önü açık bir parçayı kaydır."
                    : "Let's rescue these Jiggles! Slide a piece with an open path."));
                Assert.That(ReverseSolver.Presentation.Texts.P21StoryLast, Is.EqualTo(turkish
                    ? "Bütün parçaları çıkarınca resim kurtulur!" : "Clear every piece to free the picture!"));
            }
            finally { ReverseSolver.Presentation.Lang.Tr = original; }
        }

        [Test]
        public void PreludeKeepsTheFortyAuthoredLevelsAndOnlyEnablesForP21()
        {
            foreach (var cohort in new[] { null, "p20", "pt1", "root" })
                Assert.That(P21Onboarding.Enabled(cohort, true, false), Is.False);
            Assert.That(P21Onboarding.Enabled("p21", true, false), Is.False);
            Assert.That(P21Onboarding.Enabled("p21", true, false, true, "1"), Is.True);
            foreach (var cohort in new[] { null, "p20", "pt1", "root" })
                Assert.That(P21Onboarding.Enabled(cohort, true, false, true, "1"), Is.False);
            Assert.That(P21Onboarding.Enabled("p21", true, false, false), Is.True, "art=eski keeps its existing PM59 flow");
            Assert.That(P21Onboarding.Enabled("p21", true, false, true, "0"), Is.False);
            Assert.That(P21Onboarding.Enabled("p21", false, false, true, "1"), Is.False);
            Assert.That(P21Onboarding.Enabled("p21", true, true, true, "1"), Is.False);
            Assert.That(P21Onboarding.Enabled("p21", false, false), Is.False);
            Assert.That(P21Onboarding.Enabled("p21", true, true), Is.False);
            var original = Original;
            var flow = Flow();
            Assert.That(original.Levels.Count, Is.EqualTo(40));
            Assert.That(TestData.Levels.Levels.Count, Is.EqualTo(40), "classic boot/golden set is unchanged too");
            Assert.That(flow.Levels.Count, Is.EqualTo(42));
            Assert.That(flow.Levels.Select(l => l.Id).Distinct().Count(), Is.EqualTo(42));
            Assert.That(flow.Levels[0].Id, Is.EqualTo(P21Onboarding.TutorialId));
            Assert.That(flow.Levels[1].Id, Is.EqualTo(P21Onboarding.EasyId));
            foreach (var l in flow.Levels.Take(2)) Assert.That(l.Version, Is.EqualTo(1));
            for (int i = 0; i < 40; i++) Assert.That(flow.Levels[i + 2], Is.SameAs(original.Levels[i]));
        }

        [Test]
        public void GuidedOpeningPreservesSaveIdsAndGivesTheFirstTenLevelsMoreTime()
        {
            // PM_64: the 2x2 guided opening is opt-in (&tut=2x2); obstacle lessons are on by default.
            Assert.That(P21Onboarding.UseGuided(null, true, false, true, null), Is.False);
            Assert.That(P21Onboarding.UseGuided("p21", true, false, true, null), Is.False);
            Assert.That(P21Onboarding.UseGuided("p21", true, false, true, "1"), Is.True);
            Assert.That(P21Onboarding.UseGuided("p22", true, false, true, "1"), Is.True);
            Assert.That(P21Onboarding.UseGuided("pt1", true, false, true, "1"), Is.False);
            Assert.That(P21Onboarding.UseGuided("p21", true, true, true, "1"), Is.False);
            Assert.That(P21Onboarding.UseLessons("p22", true, false, true, null), Is.True);
            Assert.That(P21Onboarding.UseLessons("p22", true, false, true, "0"), Is.False);
            Assert.That(P21Onboarding.UseLessons("pt1", true, false, true, null), Is.False);
            var opening = P21Onboarding.ReplaceOpening(Original).Levels;
            Assert.That(opening.Select(l => l.Id), Is.EqualTo(Original.Levels.Select(l => l.Id)));
            Assert.That(opening.Select(l => l.Number), Is.EqualTo(Original.Levels.Select(l => l.Number)));
            Assert.That(opening.Take(2).Select(l => l.Width), Is.EqualTo(new[] { 2, 3 }));
            for (int i = 2; i < 10; i++)
            {
                Assert.That(opening[i].TimerSeconds, Is.GreaterThanOrEqualTo(4 * opening[i].Pieces.Count));
                Assert.That(opening[i].TimerSeconds, Is.GreaterThanOrEqualTo(90));
                Assert.That(opening[i].Pieces, Is.SameAs(Original.Levels[i].Pieces));
            }
            Assert.That(opening.Skip(10), Is.EqualTo(Original.Levels.Skip(10)));
        }

        [Test]
        public void GuidedOpeningRequiresTwoRealJamsThenClearsAllFourPieces()
        {
            var s = Session(P21Onboarding.ReplaceOpening(Original).Levels[0]);
            s.TutorialMove = true;
            var drag = new DragModel(new SessionDragTarget(s), 90);
            var expectedJams = new[] { (Piece: 3, Blocker: 1), (Piece: 1, Blocker: 0) };
            int jamCount = 0;
            for (int i = 0; i < 6; i++)
            {
                var action = P21Onboarding.GuidedSlide(i);
                var dir = action.Direction ?? Dir.U;
                Assert.That(drag.Press(action.Piece, 0, 0), Is.True);
                drag.Move(320 * dir.Dx(), 320 * dir.Dy());
                if (action.Blocked)
                {
                    Assert.That((action.Piece, drag.Blocker), Is.EqualTo(expectedJams[jamCount++]));
                    Assert.That(drag.Release(out _, out _), Is.EqualTo(DragRelease.SnappedBack));
                }
                else Assert.That(drag.Release(out _, out _), Is.EqualTo(DragRelease.Exited));
            }
            Assert.That(jamCount, Is.EqualTo(2));
            Assert.That(s.Outcome, Is.EqualTo(Outcome.Win));
            Assert.That(s.Moves, Is.EqualTo(4));
            Assert.That(s.Jams, Is.Zero);
        }

        [Test]
        public void FirstObstacleAppearancesFollowTheLessonSlotsAndEachTeachesOneObstacle()
        {
            var levels = P21Onboarding.SpaceObstacleLessons(P21Onboarding.ReplaceOpening(Original)).Levels;
            int First(System.Func<LevelData, bool> has) => levels.ToList().FindIndex(l => has(l)) + 1;
            Assert.That(First(l => l.Nails.Count > 0), Is.EqualTo(3));
            Assert.That(First(l => l.Pieces.Any(p => p.Length > 1)), Is.EqualTo(10));
            Assert.That(First(l => l.Sealed.Count > 0), Is.EqualTo(14));
            Assert.That(First(l => l.Bombs.Count > 0), Is.EqualTo(20));
            foreach (int slot in new[] { 3, 10, 14, 20 })
            {
                var l = levels[slot - 1];
                int obstacles = new[] { l.Nails.Count > 0, l.Pieces.Any(p => p.Length > 1), l.Sealed.Count > 0, l.Bombs.Count > 0 }.Count(has => has);
                Assert.That(obstacles, Is.EqualTo(1), l.ToString());
                Assert.That(P21Onboarding.IsObstacleLesson(slot), Is.True);
            }
            Assert.That(levels[5].Intro, Is.Null);
            Assert.That(levels[19].Intro.Icon, Is.EqualTo("bomb"));
            Assert.That(levels[20], Is.SameAs(Original.Levels[20]));
        }

        [Test]
        public void ObstacleLessonsClearOnRecordedPathsWithoutABoosterOrValve()
        {
            var levels = P21Onboarding.SpaceObstacleLessons(P21Onboarding.ReplaceOpening(Original)).Levels;
            foreach (int slot in new[] { 3, 10, 14, 20 })
            {
                var level = levels[slot - 1];
                var s = Session(level);
                foreach (var move in level.Solution)
                    Assert.That(s.TryExit(move.Piece, move.Dir), Is.EqualTo(CommandResult.Ok), $"{level} {move}");
                Assert.That(s.Outcome, Is.EqualTo(Outcome.Win), level.ToString());
                Assert.That(s.ValveCount, Is.Zero, level.ToString());
            }
        }

        [Test]
        public void BombLessonCanLeaveItsBombUntilNoOtherPieceCanGo()
        {
            var level = P21Onboarding.SpaceObstacleLessons(Original).Levels[19];
            var s = Session(level);
            int bomb = level.Bombs.Keys.Single();
            while (!s.IsOver)
            {
                if (s.Board.IsPresent(bomb))
                    Assert.That(s.FuseAt(bomb), Is.EqualTo(s.Board.PresentCount));
                var piece = TravelRule.FreePieces(s.Board).OrderBy(p => p == bomb).First();
                var dir = TravelRule.ExitDirs(s.Board, piece)[0];
                Assert.That(s.TryExit(piece, dir), Is.EqualTo(CommandResult.Ok));
            }
            Assert.That(s.Outcome, Is.EqualTo(Outcome.Win));
        }

        [Test]
        public void EarlyUnassistedBoardsReachTheFirstWinTargetInSimulation()
        {
            var levels = P21Onboarding.SpaceObstacleLessons(P21Onboarding.ReplaceOpening(Original)).Levels;
            var players = FunSim.Population(96);
            foreach (var level in levels.Skip(2).Take(8))
            {
                var r = FunSim.Run(level, Genre.Kolay, players);
                TestContext.WriteLine($"{level} first-win={r.FirstWinRate:0.000}");
                Assert.That(r.FirstWinRate, Is.GreaterThanOrEqualTo(.85), level.ToString());
            }
        }

        [Test]
        public void TutorialFourStepsUseTheActualTravelAndSessionRules()
        {
            var s = Session(Flow().Levels[0]);
            Assert.That(s.Level.Pieces.Select(p => p[0]), Is.EqualTo(new[] { new Cell(0, 0), new Cell(1, 0), new Cell(0, 1), new Cell(1, 1) }));
            Assert.That(s.Probe(2, Dir.D).Exit, Is.True, "C down");
            Assert.That(s.TryExit(2, Dir.D), Is.EqualTo(CommandResult.Ok));
            var jam = s.Probe(1, Dir.L);
            Assert.That(jam.Exit, Is.False);
            Assert.That(jam.Blocker, Is.EqualTo(0), "B left hits A");
            Assert.That(s.TryExit(1, Dir.L), Is.EqualTo(CommandResult.Blocked));
            s.TutorialMove = true;
            Assert.That(s.RecordJam(1), Is.EqualTo(CommandResult.Ok));
            Assert.That(s.Moves, Is.EqualTo(1));
            Assert.That(s.Jams, Is.Zero);
            Assert.That(s.TryExit(1, Dir.R), Is.EqualTo(CommandResult.Ok));
            foreach (var dir in DirExt.All) Assert.That(s.Probe(0, dir).Exit, Is.True, "A can explore any open direction after C/B");
            Assert.That(s.TryExit(0, Dir.U), Is.EqualTo(CommandResult.Ok));
            Assert.That(s.TryExit(3, Dir.D), Is.EqualTo(CommandResult.Ok));
            Assert.That(s.Outcome, Is.EqualTo(Outcome.Win));
            Assert.That(s.Moves, Is.EqualTo(4));
        }

        [Test]
        public void ACanExitUpOrLeftInitiallyAndDownAfterC()
        {
            var s = Session(Flow().Levels[0]);
            Assert.That(s.Probe(0, Dir.U).Exit, Is.True);
            Assert.That(s.Probe(0, Dir.L).Exit, Is.True);
            Assert.That(s.Probe(0, Dir.D).Exit, Is.False);
            s.TryExit(2, Dir.D);
            Assert.That(s.Probe(0, Dir.D).Exit, Is.True);
        }

        [Test]
        public void EasyLevelSolvesWithManyDifferentRemovalOrders()
        {
            var level = Flow().Levels[1];
            Assert.That((level.Width, level.Height, level.TimerSeconds), Is.EqualTo((3, 3, 180)));
            Assert.That(level.Nails.Count + level.Bombs.Count + level.Sealed.Count, Is.Zero);
            Assert.That(GreedySolver.Solve(level).Solved, Is.True);
            var start = Session(level);
            Assert.That(TravelRule.FreePieces(start.Board).Count, Is.EqualTo(8));
            // Exhaustive memoized count of distinct piece orders; directions do not inflate the count.
            var memo = new System.Collections.Generic.Dictionary<string, int>();
            int Orders(GameSession s)
            {
                if (s.Outcome == Outcome.Win) return 1;
                string key = string.Join(",", s.Board.PresentPieces());
                if (memo.TryGetValue(key, out var count)) return count;
                count = 0;
                foreach (int p in s.Board.PresentPieces())
                {
                    var dirs = TravelRule.ExitDirs(s.Board, p);
                    if (dirs.Count == 0) continue;
                    var next = s.Clone();
                    Assert.That(next.TryExit(p, dirs[0]), Is.EqualTo(CommandResult.Ok));
                    count += Orders(next);
                }
                return memo[key] = count;
            }
            Assert.That(Orders(start), Is.EqualTo(290304));
        }

        [Test]
        public void ResetQueryClearsProgressSeenAndCoinsButKeepsIdentityAndTelemetry()
        {
            var store = new MemoryStore();
            var save = new SaveData(store, () => "device");
            Assert.That(P21Onboarding.IsNewFlow(save), Is.True);
            save.SetPlayer("tester"); save.SetCoins(42);
            var log = save.Restore();
            var s = log.Start(Flow().Levels[0], "test", new Mulberry32(1));
            foreach (int p in new[] { 2, 1, 0, 3 }) s.TryExit(p, TravelRule.ExitDirs(s.Board, p)[0]);
            save.Store(log); save.RecordLoss(P21Onboarding.EasyId); save.MarkFunAsked(P21Onboarding.EasyId);
            Assert.That(P21Onboarding.IsNewFlow(save), Is.True, "keep the prelude across reloads after tutorial win");
            foreach (var key in new[] { "rs_intro_shown", "rs_intro_end_shown", "rs_boosters", "rs_p21_tutorial_seen", "rs_profile_p21", SaveData.Key("stats") }) store.Set(key, "1");
            store.Set(SaveData.Key("queue"), "pending");
            foreach (var c in new[] { null, "p20", "pt1" }) Assert.That(SaveData.ResetForQuery(store, "1", c, Flow().Levels), Is.False);
            Assert.That(SaveData.ResetForQuery(store, "0", "p21", Flow().Levels), Is.False);
            Assert.That(SaveData.ResetForQuery(store, "1", "p21", Flow().Levels), Is.True);
            var reset = new SaveData(store, () => "unexpected");
            Assert.That(reset.Attempts.Count + reset.Best.Count + reset.Losses.Count + reset.FunAsked.Count, Is.Zero);
            Assert.That(reset.Coins, Is.EqualTo(new ContinueRules().StartCoins));
            Assert.That((reset.Player, reset.Install), Is.EqualTo(("tester", "device")));
            Assert.That(store.Get("rs_intro_shown"), Is.Null);
            Assert.That(store.Get("rs_intro_end_shown"), Is.Null);
            Assert.That(store.Get("rs_boosters"), Is.Null);
            Assert.That(store.Get("rs_p21_tutorial_seen"), Is.Null);
            Assert.That(store.Get(SaveData.Key("stats")), Is.Null);
            Assert.That(store.Get(SaveData.Key("queue")), Is.EqualTo("pending"));
        }

        [Test]
        public void ReturningP21PlayersKeepTheirExistingFlowUntilReset()
        {
            var store = new MemoryStore();
            store.Set(SaveData.Key("best"), "{\"S41\":3}");
            var save = new SaveData(store, () => "device");
            Assert.That(P21Onboarding.IsNewFlow(save), Is.False);
            SaveData.ResetForQuery(store, "1", "p21", Flow().Levels);
            Assert.That(P21Onboarding.IsNewFlow(new SaveData(store, () => "device")), Is.True);
        }

        [Test]
        public void InputColumnOffIsByteIdenticalAndOnCarriesOnlyValidKinds()
        {
            bool before = MoveRow.INPUT_COL;
            var meta = TelemetryMeta.Current;
            try
            {
                TelemetryMeta.Current = null;
                string Row(string input) => MoveRow.Json("r", "p", "l", 1, 2, 3, "exit", 2, "D", 10, 5, 180, "", null, input);
                const string old = "{\"row_id\":\"r\",\"player\":\"p\",\"level_id\":\"l\",\"level_version\":1,\"attempt\":2,\"move_no\":3,\"kind\":\"exit\",\"piece\":2,\"dir\":\"D\",\"t_ms\":10,\"think_ms\":5,\"time_left\":180,\"fuses\":\"\"}";
                MoveRow.INPUT_COL = false;
                Assert.That(Row("flick"), Is.EqualTo(old)); Assert.That(Row("drag"), Is.EqualTo(old));
                MoveRow.INPUT_COL = true;
                Assert.That(Row("flick"), Is.EqualTo(old.Substring(0, old.Length - 1) + ",\"input\":\"flick\"}"));
                Assert.That(Row("drag"), Does.Contain("\"input\":\"drag\""));
                Assert.That(Row(null), Is.EqualTo(old)); Assert.That(Row("invalid"), Is.EqualTo(old));
            }
            finally { MoveRow.INPUT_COL = before; TelemetryMeta.Current = meta; }
        }
    }
}
