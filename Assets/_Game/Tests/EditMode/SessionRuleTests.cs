using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ReverseSolver.Core.Tests
{
    /* M2 criteria 4-7 beyond the golden replays: refusals, wand determinism,
       endings, the attempt record and replayability. Where a rule is the web
       game's, the comment names the line it comes from. */
    public class SessionRuleTests
    {
        static LevelData L(string id) => TestData.Levels[id];
        static GameSession New(string id, uint seed = 1, int attempt = 1) => new GameSession(L(id), attempt, "unity-web", new Mulberry32(seed));

        static LevelData Row(int n, string extra = "")
        {
            string pieces = string.Join(",", Enumerable.Range(0, n).Select(x => $"[[{x},0]]"));
            string v = "[null," + string.Join(",", Enumerable.Repeat("0", n - 1)) + (n > 1 ? "," : "") + "null]";
            string hRow = "[" + string.Join(",", Enumerable.Repeat("null", n)) + "]";
            string json = $"{{\"format\":1,\"levels\":[{{\"id\":\"R\",\"version\":1,\"lv\":50,\"w\":{n},\"h\":1,\"timer\":10," +
                          $"\"pieces\":[{pieces}],\"vEdge\":[{v}],\"hEdge\":[{hRow},{hRow}]{extra}}}]}}";
            return LevelParser.Parse(json).Levels[0];
        }

        // ---- criterion 4: boosters ----------------------------------------------

        [Test]
        public void BoostersAreLockedBelowTheirUnlockLevel()
        {
            // web: drawBar/bar click, locked = LEVEL.lv < unlock[id]; L01 unlocks at 3/6/9/12.
            var s = New("L01");
            foreach (var b in BoosterExt.All) Assert.That(s.IsUnlocked(b), Is.False, b.Id());
            Assert.That(s.UseHammer(0), Is.EqualTo(CommandResult.Locked));
            Assert.That(s.UseClock(), Is.EqualTo(CommandResult.Locked));
            Assert.That(s.Events, Is.Empty);
            Assert.That(New("L09").IsUnlocked(Booster.Hammer), Is.True);
            Assert.That(New("L08").IsUnlocked(Booster.Hammer), Is.False);
        }

        [Test]
        public void StockComesFromTheLevelAndRunsOut()
        {
            var s = New("L06");                         // wand 1, scissors 5
            Assert.That(s.Stock(Booster.Wand), Is.EqualTo(1));
            Assert.That(s.UseWand(), Is.EqualTo(CommandResult.Ok));
            Assert.That(s.Stock(Booster.Wand), Is.Zero);
            Assert.That(s.Used(Booster.Wand), Is.EqualTo(1));
            int events = s.Events.Count;
            Assert.That(s.UseWand(), Is.EqualTo(CommandResult.OutOfStock));
            Assert.That(s.Events.Count, Is.EqualTo(events), "a refused command raises nothing");
        }

        [Test]
        public void ScissorsRefusesWhatShowJointsWouldNotOffer()
        {
            var s = New("L03");                         // scissors 2, unlocked at 3
            var l = s.Level;
            Assert.That(s.UseScissors(0, 0, true), Is.EqualTo(CommandResult.NotAJoint), "outer edge");
            // find a flat internal joint and a tabbed one
            var flat = Enumerable.Range(1, l.Width - 1).SelectMany(x => Enumerable.Range(0, l.Height).Select(y => (x, y)))
                                 .First(p => l.VJoint(p.x, p.y) == 0);
            var tab = Enumerable.Range(1, l.Width - 1).SelectMany(x => Enumerable.Range(0, l.Height).Select(y => (x, y)))
                                .First(p => l.VJoint(p.x, p.y) != 0);
            Assert.That(s.UseScissors(flat.x, flat.y, true), Is.EqualTo(CommandResult.NotAJoint), "already flat");
            Assert.That(s.UseScissors(tab.x, tab.y, true), Is.EqualTo(CommandResult.Ok));
            Assert.That(s.Board.VJoint(tab.x, tab.y), Is.Zero);
            Assert.That(s.Moves, Is.Zero, "cutting is not a move");
        }

        [Test]
        public void ScissorsNeedsTwoDifferentPresentPieces()
        {
            var s = New("L03");
            var l = s.Level;
            var tab = Enumerable.Range(1, l.Width - 1).SelectMany(x => Enumerable.Range(0, l.Height).Select(y => (x, y)))
                                .First(p => l.VJoint(p.x, p.y) != 0);
            Assert.That(s.IsCuttable(tab.x, tab.y, true), Is.True);
            s.Board.Remove(s.Board.PieceAt(new Cell(tab.x - 1, tab.y)));   // one side now empty
            Assert.That(s.IsCuttable(tab.x, tab.y, true), Is.False);
        }

        [Test]
        public void WandIsDeterministicPerSeedAndLeavesFlatJointsAlone()
        {
            string Joints(GameSession s)
            {
                var l = s.Level;
                var v = Enumerable.Range(0, l.Height).SelectMany(y => Enumerable.Range(0, l.Width + 1).Select(x => s.Board.VJoint(x, y)));
                var h = Enumerable.Range(0, l.Height + 1).SelectMany(y => Enumerable.Range(0, l.Width).Select(x => s.Board.HJoint(x, y)));
                return string.Join("", v.Concat(h).Select(j => j < 0 ? "-" : j > 0 ? "+" : "0"));
            }
            var original = Joints(New("L10"));
            var boards = new List<string>();
            foreach (uint seed in new uint[] { 1, 2, 3 })
            {
                var a = New("L10", seed); var b = New("L10", seed);
                Assert.That(a.UseWand(), Is.EqualTo(CommandResult.Ok));
                Assert.That(b.UseWand(), Is.EqualTo(CommandResult.Ok));
                Assert.That(Joints(a), Is.EqualTo(Joints(b)), $"seed {seed}: same seed, same board");
                for (int i = 0; i < original.Length; i++)
                    if (original[i] == '0') Assert.That(Joints(a)[i], Is.EqualTo('0'), $"seed {seed}: flat joint {i} changed");
                Assert.That(GreedySolver.Solve(a.Board).Solved, "wand keeps the board solvable");
                boards.Add(Joints(a));
            }
            Assert.That(boards.Distinct().Count(), Is.EqualTo(3), "different seeds, different boards");
        }

        [Test]
        public void ClockAddsExactlyTwentySeconds()
        {
            var s = New("L12");                         // clock 2, unlocked at 12
            int before = s.TimeLeft;
            Assert.That(s.UseClock(), Is.EqualTo(CommandResult.Ok));
            Assert.That(s.TimeLeft, Is.EqualTo(before + 20));
            Assert.That(s.ElapsedSeconds, Is.Zero, "the clock booster does not count as played time");
        }

        [Test]
        public void HammerWorksOnNailedPieces()
        {
            var s = New("L09");                         // hammer 2, nails on 31..33
            int nailed = s.Nails.First(n => n.Value > 0).Key;
            Assert.That(s.TryExit(nailed, Dir.D), Is.EqualTo(CommandResult.Pinned));
            Assert.That(s.UseHammer(nailed), Is.EqualTo(CommandResult.Ok));
            Assert.That(s.Board.IsPresent(nailed), Is.False);
            Assert.That(s.Moves, Is.EqualTo(1), "a hammer blow is a move");
        }

        // ---- criterion 5: endings -----------------------------------------------

        [Test]
        public void TimeRunsOutExactlyAtZeroWithFractionalTicks()
        {
            var s = new GameSession(Row(2), 1, "t", new Mulberry32(1));
            for (int i = 0; i < 39; i++) s.Tick(0.25);
            Assert.That(s.IsOver, Is.False, "9.75 s of 10");
            Assert.That(s.TimeLeft, Is.EqualTo(1));
            s.Tick(0.25);
            Assert.That(s.Outcome, Is.EqualTo(Outcome.Time));
            Assert.That(s.TimeLeft, Is.Zero);
            Assert.That(s.Record.DurationSeconds, Is.EqualTo(10));
        }

        [Test]
        public void NothingIsAcceptedAfterTheEnd()
        {
            var s = new GameSession(Row(2, ",\"boosters\":{\"hammer\":1,\"clock\":1,\"scissors\":1,\"wand\":1}"), 1, "t", new Mulberry32(1));
            Assert.That(s.Quit(), Is.EqualTo(CommandResult.Ok));
            int events = s.Events.Count;
            Assert.That(s.TryExit(0, Dir.L), Is.EqualTo(CommandResult.GameOver));
            Assert.That(s.UseHammer(0), Is.EqualTo(CommandResult.GameOver));
            Assert.That(s.UseClock(), Is.EqualTo(CommandResult.GameOver));
            Assert.That(s.UseWand(), Is.EqualTo(CommandResult.GameOver));
            Assert.That(s.UseScissors(1, 0, true), Is.EqualTo(CommandResult.GameOver));
            Assert.That(s.RecordJam(0), Is.EqualTo(CommandResult.GameOver));
            Assert.That(s.Quit(), Is.EqualTo(CommandResult.GameOver));
            s.Tick(100);
            Assert.That(s.Events.Count, Is.EqualTo(events));
            Assert.That(s.TimeLeft, Is.EqualTo(10));
        }

        [TestCase(Outcome.Win)]
        [TestCase(Outcome.Time)]
        [TestCase(Outcome.Bomb)]
        [TestCase(Outcome.Quit)]
        public void EveryAttemptEndsInExactlyOneRecord(Outcome how)
        {
            var s = new GameSession(Row(3, ",\"bombs\":{\"2\":1}"), 1, "t", new Mulberry32(1));
            switch (how)
            {
                case Outcome.Win: s.TryExit(2, Dir.R); s.TryExit(0, Dir.L); s.TryExit(1, Dir.U); break;
                case Outcome.Time: s.Tick(10); break;
                case Outcome.Bomb: s.TryExit(0, Dir.L); break;
                case Outcome.Quit: s.Quit(); s.Quit(); break;
            }
            Assert.That(s.Outcome, Is.EqualTo(how));
            Assert.That(s.Events.Count(e => e.Kind == EventKind.Finished), Is.EqualTo(1));
            Assert.That(s.Record, Is.Not.Null);
            Assert.That(s.Record.Result, Is.EqualTo(how.Id()));
        }

        // ---- criterion 6: the attempt record --------------------------------------

        [Test]
        public void RecordFieldsAreFilled()
        {
            var level = L("L12");
            var s = new GameSession(level, 3, "unity-web", new Mulberry32(1));
            s.Tick(5);
            s.UseClock();
            int free = TravelRule.FreePieces(s.Board).First(p => s.NailAt(p) <= 0);
            s.RecordJam(free);
            s.TryExit(free, TravelRule.ExitDirs(s.Board, free)[0]);
            s.Quit();
            var r = s.Record;
            Assert.That(r.LevelId, Is.EqualTo("L12"));
            Assert.That(r.LevelVersion, Is.EqualTo(1));
            Assert.That(r.LevelNumber, Is.EqualTo(12));
            Assert.That(r.Client, Is.EqualTo("unity-web"));
            Assert.That(r.Attempt, Is.EqualTo(3));
            Assert.That(r.Result, Is.EqualTo("quit"));
            Assert.That(r.DurationSeconds, Is.EqualTo(5));
            Assert.That(r.LeftSeconds, Is.EqualTo(level.TimerSeconds - 5 + 20));
            Assert.That(r.Moves, Is.EqualTo(1));
            Assert.That(r.Jams, Is.EqualTo(1));
            Assert.That(r.Stars, Is.Zero, "no stars without a win");
            Assert.That(r.Load, Is.EqualTo(level.Load));
            Assert.That((r.Scissors, r.Wand, r.Hammer, r.Clock), Is.EqualTo((0, 0, 0, 1)));
        }

        [TestCase(10, 10, 3)] [TestCase(5, 10, 3)] [TestCase(45, 100, 3)] [TestCase(44, 100, 2)]
        [TestCase(20, 100, 2)] [TestCase(19, 100, 1)] [TestCase(1, 100, 1)] [TestCase(30, 10, 3)]
        public void StarsFollowTheWebFormula(int left, int timer, int stars) =>
            // web starsFor: frac >= .45 ? 3 : frac >= .2 ? 2 : 1
            Assert.That(GameSession.StarsFor(left, timer), Is.EqualTo(stars));

        [Test]
        public void AttemptNumbersCountEveryStartAndQuitsAreRecorded()
        {
            var log = new AttemptLog();
            var level = L("L01");
            var first = log.Start(level, "unity-web", new Mulberry32(1));
            first.Quit();                               // restart
            log.Add(first.Record);
            var second = log.Start(level, "unity-web", new Mulberry32(1));
            foreach (var m in level.Solution) second.TryExit(m.Piece, m.Dir);
            log.Add(second.Record);

            Assert.That(first.Attempt, Is.EqualTo(1));
            Assert.That(second.Attempt, Is.EqualTo(2));
            Assert.That(log.Records.Select(r => r.Result), Is.EqualTo(new[] { "quit", "win" }));
            Assert.That(log.BestStars("L01"), Is.EqualTo(3));
            Assert.That(log.AttemptsOf("L02"), Is.Zero);
        }

        // ---- criterion 7: determinism ----------------------------------------------

        [Test]
        public void SameSeedAndCommandsGiveTheSameEventsAndRecord()
        {
            List<string> Play(out AttemptRecord rec)
            {
                var s = New("L31", seed: 7);
                s.Tick(3);
                s.UseWand();
                s.Tick(1.5);
                while (!s.IsOver)
                {
                    var p = TravelRule.FreePieces(s.Board).FirstOrDefault(x => s.NailAt(x) <= 0);
                    if (TravelRule.FreePieces(s.Board).All(x => s.NailAt(x) > 0)) { s.Quit(); break; }
                    s.TryExit(p, TravelRule.ExitDirs(s.Board, p)[0]);
                    s.Tick(0.7);
                }
                rec = s.Record;
                return s.Events.Select(e => e.ToString()).ToList();
            }
            var a = Play(out var ra);
            var b = Play(out var rb);
            Assert.That(a, Is.EqualTo(b));
            Assert.That(ra.ToString(), Is.EqualTo(rb.ToString()));
            Assert.That(a.Count, Is.GreaterThan(30));
        }

        [Test]
        public void MulberryMatchesTheJavaScriptGenerator()
        {
            // First draws of mulberry32(1) in JavaScript (Tools/web_harness.js).
            var r = new Mulberry32(1);
            var got = Enumerable.Range(0, 3).Select(_ => r.NextDouble()).ToArray();
            Assert.That(got, Is.EqualTo(new[] { 0.62707394058816135, 0.0027357211802154779, 0.52744703995995224 }));
        }
    }
}
