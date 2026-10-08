using System.IO;
using System.Linq;
using NUnit.Framework;
using ReverseSolver.Editing;

namespace ReverseSolver.Core.Tests
{
    /* Faz 2 Monte Carlo (PRODUCT.md, 2026-10-08 plan iii). */
    public class MonteCarloTests
    {
        [Test]
        public void SameSeedsSameResult()
        {
            var l = TestData.Levels["L24"];
            var a = MonteCarlo.Run(l, 50, 7);
            var b = MonteCarlo.Run(l, 50, 7);
            Assert.That(b.ToString(), Is.EqualTo(a.ToString()));
            Assert.That((b.Wins, b.Bombs, b.Frozen, b.RunsWithValve), Is.EqualTo((a.Wins, a.Bombs, a.Frozen, a.RunsWithValve)));
            var c = MonteCarlo.Run(l, 50, 8);
            Assert.That(c.Runs, Is.EqualTo(50));
        }

        [Test]
        public void OutcomesAddUp()
        {
            foreach (var l in TestData.Levels.Levels.Take(12))
            {
                var r = MonteCarlo.Run(l, 30);
                Assert.That(r.Wins + r.Bombs + r.Frozen, Is.EqualTo(30), l.Id);
                Assert.That(r.MeanBranching, Is.GreaterThan(0), l.Id);
            }
        }

        [Test]
        public void NoObstaclesMeansEveryRandomGameIsWon()
        {
            // removing a piece never blocks another, so without nails and bombs a random player cannot lose
            var l = TestData.Levels["L01"];
            var r = MonteCarlo.Run(l, 100);
            Assert.That(r.WinRate, Is.EqualTo(1.0));
            Assert.That(r.MeanMoves, Is.EqualTo(l.Pieces.Count));
        }

        [Test]
        public void AWalledInBoardIsAlwaysFrozen()
        {
            var d = LevelDraft.New(3, 3);
            foreach (var side in DirExt.All) for (int lane = 0; lane < 3; lane++) d.SetSealed(side, lane, true);
            var r = MonteCarlo.Run(d.ToLevelData(), 20);
            Assert.That(r.FrozenRate, Is.EqualTo(1.0));
            Assert.That(r.MeanMoves, Is.EqualTo(0));
        }

        [Test]
        public void TwoHundredRunsOnTheLargestLevelInASecond()
        {
            var l = TestData.Levels["L40"];
            MonteCarlo.Run(l, 5);
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var r = MonteCarlo.Run(l, MonteCarlo.DefaultRuns);
            double ms = sw.Elapsed.TotalMilliseconds;
            TestContext.WriteLine($"L40 x {MonteCarlo.DefaultRuns}: {ms:0} ms  {r}");
            Assert.That(ms, Is.LessThan(1000));
        }

        /* A report, not a gate: all 40 levels x 200 seeds to Builds/monte_carlo.csv. */
        [Test]
        public void ReportAllLevels()
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var rows = TestData.Levels.Levels.Select(l => ("web", MonteCarlo.Run(l))).ToList();
            Directory.CreateDirectory("Builds");
            File.WriteAllBytes("Builds/monte_carlo.csv", LevelCsv.ToBytes(MonteCarlo.Csv(rows)));
            foreach (var (_, r) in rows) TestContext.WriteLine(r.ToString());
            TestContext.WriteLine($"40 levels x {MonteCarlo.DefaultRuns}: {sw.ElapsedMilliseconds} ms -> Builds/monte_carlo.csv");
            Assert.That(rows.Count, Is.EqualTo(40));
        }
    }
}
