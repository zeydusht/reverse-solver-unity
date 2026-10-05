using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace ReverseSolver.Core.Tests
{
    /* M1 acceptance: the 40 web levels load, their recorded solutions play out
       under the ported rule, and the port agrees with the web code. */
    public class LevelRuleTests
    {
        static IEnumerable<string> Ids => TestData.LevelIds();

        [Test]
        public void LevelFileLoads()
        {
            var set = TestData.Levels;
            Assert.That(set.Format, Is.EqualTo(1));
            Assert.That(set.Levels.Count, Is.EqualTo(40));
            Assert.That(set.Levels.Select(l => l.Id),
                Is.EqualTo(Enumerable.Range(1, 40).Select(i => $"L{i:00}")), "ids follow the web order on first import");
            Assert.That(set.Levels.All(l => l.Version == 1));
            Assert.That(set.Levels.Select(l => l.Number), Is.EqualTo(Enumerable.Range(1, 40)));
        }

        // ---- acceptance 1: recorded solution is valid move by move ------------------

        [TestCaseSource(nameof(Ids))]
        public void RecordedSolutionEmptiesTheBoard(string id)
        {
            var level = TestData.Levels[id];
            var b = new Board(level);
            Assert.That(level.Solution.Count, Is.EqualTo(level.Pieces.Count), "one move per piece");
            for (int i = 0; i < level.Solution.Count; i++)
            {
                var m = level.Solution[i];
                Assert.That(b.IsPresent(m.Piece), $"step {i}: piece {m.Piece} already removed");
                var lim = TravelRule.Limit(b, m.Piece, m.Dir);
                Assert.That(lim.Exit, $"step {i}: {m} cannot exit, {lim}");
                b.Remove(m.Piece);
            }
            Assert.That(b.PresentCount, Is.Zero);
        }

        // ---- acceptance 2: travel rule matches the web code ---------------------

        [TestCaseSource(nameof(Ids))]
        public void TravelLimitMatchesWebGolden(string id)
        {
            var level = TestData.Levels[id];
            var golden = TestData.GoldenLevel(id);
            int probes = 0;
            foreach (var state in golden.Get("states").Items)
            {
                int step = state.Get("step").Int();
                var b = new Board(level);
                for (int i = 0; i < step; i++) b.Remove(level.Solution[i].Piece);

                foreach (var row in state.Get("limits").Items)
                {
                    int piece = row[0].Int();
                    var dir = DirExt.All[row[1].Int()];
                    var expected = new TravelLimit(row[2].Int(), row[3].Int() == 1, row[4].Int(), row[5].Int() == 1);
                    var actual = TravelRule.Limit(b, piece, dir);
                    Assert.That(actual, Is.EqualTo(expected), $"step {step}, piece {piece} {dir}");
                    probes++;
                }
            }
            Assert.That(probes, Is.GreaterThan(0));
        }

        [TestCaseSource(nameof(Ids))]
        public void FreePiecesMatchWebAtStart(string id)
        {
            var expected = TestData.GoldenLevel(id).Get("free0").Items.Select(n => n.Int());
            Assert.That(TravelRule.FreePieces(new Board(TestData.Levels[id])), Is.EqualTo(expected));
        }

        // ---- acceptance 4: greedy solver equals the web solver --------------------

        [TestCaseSource(nameof(Ids))]
        public void GreedySolverMatchesWeb(string id)
        {
            var golden = TestData.GoldenLevel(id);
            var r = GreedySolver.Solve(TestData.Levels[id]);
            Assert.That(r.Solved, Is.EqualTo(golden.Get("solvable").AsBool));
            Assert.That(r.Order, Is.EqualTo(golden.Get("greedyOrder").Items.Select(n => n.Int())), "same removal order");
        }

        /* Not an assertion on solvability: a level greedy cannot clear is data
           for the report, not a failure. Equality with the web is checked above. */
        [Test]
        public void GreedySolverReport()
        {
            var sb = new StringBuilder("Greedy solver over 40 levels:\n");
            int solved = 0;
            foreach (var l in TestData.Levels.Levels)
            {
                var r = GreedySolver.Solve(l);
                if (r.Solved) solved++;
                sb.AppendLine($"  {l.Id} {(r.Solved ? "solved" : "STUCK " + string.Join(",", r.Stuck))} " +
                              $"free at start {TravelRule.FreePieces(new Board(l)).Count}/{l.Pieces.Count}");
            }
            sb.AppendLine($"  solved {solved}/40");
            TestContext.WriteLine(sb.ToString());
            Assert.Pass($"greedy solved {solved}/40");
        }
    }
}
