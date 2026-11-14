using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ReverseSolver.Core.Tests
{
    public class P21BoosterTests
    {
        static LevelData Grid(int w = 4, int h = 4, bool chain = false, bool sealedBoard = false)
        {
            var pieces = new List<string>();
            for (int y = 0; y < h; y++) for (int x = 0; x < w; x++)
                if (!chain || y != 0 || (x != 1 && x != 2)) pieces.Add($"[[{x},{y}]]");
            if (chain) pieces.Add("[[1,0],[2,0]]");
            string v = "[null," + string.Join(",", Enumerable.Repeat("1", w - 1)) + ",null]";
            string edge = "[" + string.Join(",", Enumerable.Repeat("null", w)) + "]";
            string middle = "[" + string.Join(",", Enumerable.Repeat("0", w)) + "]";
            string walls = sealedBoard ? ",\"sealed\":{\"U\":[0,1,2,3],\"D\":[0,1,2,3],\"L\":[0,1,2,3],\"R\":[0,1,2,3]}" : "";
            string json = $"{{\"format\":1,\"levels\":[{{\"id\":\"W\",\"version\":1,\"lv\":50,\"w\":{w},\"h\":{h},\"timer\":10," +
                $"\"pieces\":[{string.Join(",", pieces)}],\"vEdge\":[{string.Join(",", Enumerable.Repeat(v, h))}]," +
                $"\"hEdge\":[{edge},{string.Join(",", Enumerable.Repeat(middle, h - 1))},{edge}]{walls}}}]}}";
            return LevelParser.Parse(json).Levels[0];
        }

        // These cover the wand's joint mechanics on full boards; "must free a piece" (PM_66) is in WandFreedTests.
        static GameSession Session(LevelData level = null, uint seed = 1) => new GameSession(level ?? Grid(), 1, "test", new Mulberry32(seed)) { WandMustFree = false };
        static int[] Joints(Board b) => Enumerable.Range(0, b.Level.Height).SelectMany(y => Enumerable.Range(0, b.Level.Width + 1).Select(x => b.VJoint(x, y)))
            .Concat(Enumerable.Range(0, b.Level.Height + 1).SelectMany(y => Enumerable.Range(0, b.Level.Width).Select(x => b.HJoint(x, y)))).ToArray();

        [Test]
        public void QuadrantsCoverOddBoardsOnceAndSplitEvenly()
        {
            var level = Grid(5, 3);
            var regions = Enumerable.Range(0, 4).Select(q => WandRegion.Quadrant(level, q)).ToArray();
            for (int y = 0; y < 3; y++) for (int x = 0; x < 5; x++)
                Assert.That(regions.Count(r => r.Contains(new Cell(x, y))), Is.EqualTo(1));
            Assert.That(regions.Select(r => r.Width).Max() - regions.Select(r => r.Width).Min(), Is.LessThanOrEqualTo(1));
            Assert.That(regions.Select(r => r.Height).Max() - regions.Select(r => r.Height).Min(), Is.LessThanOrEqualTo(1));
        }

        [Test]
        public void RegionWandChangesOnlyInternalJointsAndKeepsSolvability()
        {
            foreach (uint seed in new uint[] { 1, 2, 7, 21, 99 }) for (int q = 0; q < 4; q++)
            {
                var s = Session(seed: seed); var before = s.Board.Clone(); var region = WandRegion.Quadrant(s.Level, q);
                int stock = s.Stock(Booster.Wand);
                Assert.That(s.CanWandRegion(q), Is.True);
                Assert.That(s.UseWandRegion(q), Is.EqualTo(CommandResult.Ok));
                Assert.That(Joints(s.Board), Is.Not.EqualTo(Joints(before)), "must produce an actual change");
                Assert.That(s.Stock(Booster.Wand), Is.EqualTo(stock - 1));
                Assert.That(GreedySolver.Solve(s.Board).Solved, Is.True);
                for (int y = 0; y < s.Level.Height; y++) for (int x = 1; x < s.Level.Width; x++)
                    if (!region.Contains(new Cell(x - 1, y)) || !region.Contains(new Cell(x, y)))
                        Assert.That(s.Board.VJoint(x, y), Is.EqualTo(before.VJoint(x, y)));
                for (int y = 1; y < s.Level.Height; y++) for (int x = 0; x < s.Level.Width; x++)
                    if (!region.Contains(new Cell(x, y - 1)) || !region.Contains(new Cell(x, y)))
                        Assert.That(s.Board.HJoint(x, y), Is.EqualTo(before.HJoint(x, y)));
                Assert.That(s.Board.PresentPieces(), Is.EqualTo(before.PresentPieces()));
            }
        }

        [Test]
        public void CrossingChainAndItsJointsStayUntouched()
        {
            var s = Session(Grid(chain: true)); var before = s.Board.Clone();
            Assert.That(s.UseWandRegion(0), Is.EqualTo(CommandResult.Ok));
            Assert.That(s.Board.VJoint(1, 0), Is.EqualTo(before.VJoint(1, 0)));
            Assert.That(s.Board.VJoint(2, 0), Is.EqualTo(before.VJoint(2, 0)));
            Assert.That(s.Board.VJoint(3, 0), Is.EqualTo(before.VJoint(3, 0)));
            Assert.That(s.Board.HJoint(1, 1), Is.EqualTo(before.HJoint(1, 1)));
            Assert.That(s.Board.HJoint(2, 1), Is.EqualTo(before.HJoint(2, 1)));
        }

        [Test]
        public void PreviewConsumesNeitherRandomnessNorStockAndSeedReplays()
        {
            var a = Session(seed: 71); var b = Session(seed: 71);
            var before = Joints(a.Board); int stock = a.Stock(Booster.Wand);
            for (int i = 0; i < 3; i++) Assert.That(a.CanWandRegion(0), Is.True);
            Assert.That(Joints(a.Board), Is.EqualTo(before)); Assert.That(a.Stock(Booster.Wand), Is.EqualTo(stock));
            a.UseWandRegion(0); b.UseWandRegion(0);
            Assert.That(Joints(a.Board), Is.EqualTo(Joints(b.Board)));
        }

        [Test]
        public void EmptyTinyAndImpossibleRegionsRejectWithoutSpending()
        {
            foreach (var s in new[] { Session(Grid(2, 2)), Session(Grid(sealedBoard: true)), Session() })
            {
                if (s.Level.Width == 4 && !s.Level.Sealed.Any())
                    foreach (int p in s.Board.PresentPieces().ToArray()) s.Board.Remove(p);
                var before = Joints(s.Board); int stock = s.Stock(Booster.Wand);
                Assert.That(s.CanWandRegion(0), Is.False);
                Assert.That(s.UseWandRegion(0), Is.EqualTo(CommandResult.NoArrangement));
                Assert.That(Joints(s.Board), Is.EqualTo(before)); Assert.That(s.Stock(Booster.Wand), Is.EqualTo(stock));
                Assert.That(s.Events, Is.Empty);
            }
        }

        [Test]
        public void ClockFreezesWithoutAddingAndExpiresAcrossFractionalTicks()
        {
            var s = Session(); s.ClockFreezesTime = true;
            s.Tick(100); Assert.That(s.TimeLeft, Is.EqualTo(10), "first move gate");
            Assert.That(s.UseClock(), Is.EqualTo(CommandResult.Ok));
            Assert.That(s.ClockStarted, Is.True); Assert.That(s.TimeLeft, Is.EqualTo(10));
            s.Tick(GameSession.FreezeSeconds - .5); Assert.That(s.TimeLeft, Is.EqualTo(10)); Assert.That(s.FreezeLeft, Is.EqualTo(.5).Within(1e-8));
            s.Tick(.75); Assert.That(s.TimeLeft, Is.EqualTo(10)); Assert.That(s.FreezeLeft, Is.Zero);
            s.Tick(.75); Assert.That(s.TimeLeft, Is.EqualTo(9));
            Assert.That(s.Events.Any(e => e.Kind == EventKind.TimeAdded), Is.False);
        }

        [Test]
        public void FreezeDefersTimeoutAndContinueDoesNotResurrectExpiredFreeze()
        {
            var s = Session(); s.ClockOnFirstMove = false; s.ClockFreezesTime = true;
            s.Tick(9); s.UseClock(); s.Tick(GameSession.FreezeSeconds);
            Assert.That(s.IsOver, Is.False); Assert.That(s.TimeLeft, Is.EqualTo(1));
            s.Tick(1); Assert.That(s.Outcome, Is.EqualTo(Outcome.Time));
            Assert.That(s.UseClock(), Is.EqualTo(CommandResult.GameOver));
            Assert.That(s.Continue(5, 3), Is.True); s.Tick(1);
            Assert.That(s.TimeLeft, Is.EqualTo(4)); Assert.That(s.FreezeLeft, Is.Zero);
        }

        [Test]
        public void TestModeUnlocksAndNeverDepletesStock()
        {
            var level = Grid(); level.Number = 1;
            level.Unlock = BoosterExt.All.ToDictionary(b => b.Id(), b => 99);
            var s = Session(level); s.TestMode = true;
            foreach (var b in BoosterExt.All) { s.SetStock(b, 0); Assert.That(s.IsUnlocked(b), Is.True); Assert.That(s.Stock(b), Is.EqualTo(99)); }
            for (int i = 0; i < 105; i++) Assert.That(s.UseClock(), Is.EqualTo(CommandResult.Ok));
            Assert.That(s.Stock(Booster.Clock), Is.EqualTo(99));
        }

        [Test]
        public void TestCoinsStayFixedWithoutPersistingTheTestBalance()
        {
            var store = new MemoryStore(); var save = new SaveData(store, () => "test");
            save.SetCoins(4321); save.TestMode = true; save.SetCoins(0);
            Assert.That(save.Coins, Is.EqualTo(99999));
            Assert.That(new SaveData(store, () => "test").Coins, Is.EqualTo(4321));
        }

        [TestCase("ic", "1", true)]
        [TestCase("p21", "1", true)]
        [TestCase("pt1", "1", false)]
        [TestCase(null, "1", false)]
        [TestCase("ic", "0", false)]
        [TestCase("p21", null, false)]
        public void TestModeRequiresAnExplicitAllowedCohort(string cohort, string flag, bool expected)
        {
            Assert.That(ReverseSolver.Presentation.LaunchPlan.TestMode(cohort, flag), Is.EqualTo(expected));
        }

        [Test]
        public void ClockLegacyStillAddsTwentySeconds()
        {
            var s = Session(); s.UseClock();
            Assert.That(s.TimeLeft, Is.EqualTo(10 + GameSession.ClockBonusSeconds));
            Assert.That(s.FreezeLeft, Is.Zero); s.Tick(1);
            Assert.That(s.TimeLeft, Is.EqualTo(9 + GameSession.ClockBonusSeconds));
        }

        [Test]
        public void BombContinueRetainsFreezeAndFractionalCarryResumesAfterIt()
        {
            var level = P21Onboarding.Grid("freeze-bomb", 3, 1);
            level.Bombs = new Dictionary<int, int> { [2] = 1 };
            var s = Session(level); s.ClockFreezesTime = true; s.ClockOnFirstMove = false;
            s.Tick(.75); s.UseClock(); s.Tick(2);
            Assert.That(s.TryExit(0, Dir.L), Is.EqualTo(CommandResult.Ok));
            Assert.That(s.Outcome, Is.EqualTo(Outcome.Bomb));
            double freeze = s.FreezeLeft; s.Tick(100);
            Assert.That(s.FreezeLeft, Is.EqualTo(freeze), "loss screen does not drain the freeze");
            Assert.That(s.Continue(5, 3), Is.True);
            int time = s.TimeLeft; s.Tick(freeze + .25);
            Assert.That(s.TimeLeft, Is.EqualTo(time - 1), "carry from before freezing is retained");
        }
    }
}
