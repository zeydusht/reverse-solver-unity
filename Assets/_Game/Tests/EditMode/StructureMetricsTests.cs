using System.Linq;
using NUnit.Framework;
using ReverseSolver.Editing;

namespace ReverseSolver.Core.Tests
{
    /* Faz 2 structural measures (PRODUCT.md 2026-10-08 22:37): exact values on
       boards small enough to count by hand. */
    public class StructureMetricsTests
    {
        /* 2x2, all flat. Pieces 0 (0,0), 1 (1,0), 2 (0,1), 3 (1,1); the solver takes them in that order.
             step 1: 4 pieces, 2 exits each = 8 valid of 16 -> 16/8 - 1 = 1
             step 2: (1,0) 3, (0,1) 3, (1,1) 2 = 8 of 12   -> 0.5
             step 3: (0,1) 3, (1,1) 3 = 6 of 8              -> 1/3
             step 4: (1,1) 4 of 4                           -> 0 */
        [Test]
        public void BlindTriesOnAnOpenTwoByTwo()
        {
            var m = StructureMetrics.Of(LevelDraft.New(2, 2).ToLevelData());
            Assert.That(m.Steps, Is.EqualTo(4));
            Assert.That(m.BlindTriesTotal, Is.EqualTo(1 + .5 + 1.0 / 3).Within(1e-9));
            Assert.That(m.BlindTriesPerExit, Is.EqualTo((1 + .5 + 1.0 / 3) / 4).Within(1e-9));
            Assert.That(m.FreedomStart, Is.EqualTo(1.0));
            Assert.That(m.FreedomMean, Is.EqualTo(1.0));
            Assert.That(m.BottleneckSteps, Is.EqualTo(1), "only the last piece is a single choice");
            Assert.That(m.FewestChoices, Is.EqualTo(1));
            Assert.That(m.NailedShare, Is.EqualTo(0));
            Assert.That(double.IsNaN(m.FuseMargin), Is.True, "no bomb");
        }

        /* Same board, nail 2 on (0,0). The solver takes 1, 2, then 0 (freed), then 3.
             step 1: (1,0) 2, (0,1) 2, (1,1) 2 = 6 of 16    -> 16/6 - 1; freedom 3/4
             step 2: nail 1; (0,1) 2, (1,1) 3 = 5 of 12     -> 12/5 - 1; freedom 2/3
             step 3: nail 0; (0,0) 4, (1,1) 4 = 8 of 8      -> 0;        freedom 1
             step 4: (1,1) 4 of 4                           -> 0;        freedom 1 */
        [Test]
        public void ANailTakesAPieceOutOfTheChoices()
        {
            var d = LevelDraft.New(2, 2);
            d.SetNail(new Cell(0, 0), 2);
            var m = StructureMetrics.Of(d.ToLevelData());
            Assert.That(m.NailedShare, Is.EqualTo(.25));
            Assert.That(m.FreedomStart, Is.EqualTo(.75));
            Assert.That(m.BlindTriesTotal, Is.EqualTo(16.0 / 6 - 1 + 12.0 / 5 - 1).Within(1e-9));
            Assert.That(m.FreedomMean, Is.EqualTo((.75 + 2.0 / 3 + 1 + 1) / 4).Within(1e-9));
            Assert.That(m.BottleneckSteps, Is.EqualTo(1));
        }

        /* 3x3 flat, bomb fuse 5 in the middle: one neighbour out, then the bomb: 2 moves, margin 3. */
        [Test]
        public void FuseMarginIsFuseMinusTheShortestWayOut()
        {
            var d = LevelDraft.New(3, 3);
            d.SetBomb(new Cell(1, 1), 5);
            Assert.That(StructureMetrics.Of(d.ToLevelData()).FuseMargin, Is.EqualTo(3));
            d.SetBomb(new Cell(1, 1), 2);
            Assert.That(StructureMetrics.Of(d.ToLevelData()).FuseMargin, Is.EqualTo(0), "exactly enough");
            d.SetBomb(new Cell(1, 1), 1);
            Assert.That(StructureMetrics.Of(d.ToLevelData()).FuseMargin, Is.EqualTo(-1), "one move short: the dig is found with a long fuse");
            d.SetBomb(new Cell(1, 1), 5);
            Assert.That(StructureMetrics.Of(d.ToLevelData()).FuseMarginExact, Is.True, "small board: the exact search finishes");
        }

        [Test]
        public void WebLevelsGetEveryMeasureAndTheyRepeat()
        {
            foreach (var l in TestData.Levels.Levels)
            {
                var a = StructureMetrics.Of(l);
                var b = StructureMetrics.Of(l);
                Assert.That(a.Steps, Is.EqualTo(l.Pieces.Count), l.Id + ": the solver clears every web level");
                Assert.That((a.BlindTriesTotal, a.FreedomMean, a.BottleneckSteps, a.FuseMargin),
                            Is.EqualTo((b.BlindTriesTotal, b.FreedomMean, b.BottleneckSteps, b.FuseMargin)), l.Id);
                Assert.That(l.Bombs.Count == 0 ? double.IsNaN(a.FuseMargin) : a.FuseMargin >= 0, Is.True, l.Id + " fuse margin");
                if (l.Bombs.Count == 0) Assert.That(a.FuseMarginExact, Is.True, l.Id);
            }
        }

        /* L30's bomb sits behind nine pieces: the exact search runs out and SmartPlayer's own dig
           (10 moves, fuse 15) gives a cautious margin of at least 5. */
        [Test]
        public void DeepBombsFallBackToTheSolversDig()
        {
            var m = StructureMetrics.Of(TestData.Levels["L30"]);
            Assert.That(m.FuseMarginExact, Is.False);
            Assert.That(m.FuseMargin, Is.EqualTo(5));
        }

        /* The PM's question: do the new measures tell the 26 bomb-free levels apart? (report) */
        [Test]
        public void SpreadOnBombFreeLevels()
        {
            var ms = TestData.Levels.Levels.Where(l => l.Bombs.Count == 0).Select(StructureMetrics.Of).ToList();
            string Span(string name, System.Func<StructureMetrics, double> f) =>
                $"{name}: {ms.Min(f):0.###} .. {ms.Max(f):0.###} (median {ms.Select(f).OrderBy(x => x).ElementAt(ms.Count / 2):0.###})";
            TestContext.WriteLine($"{ms.Count} bomb-free levels");
            TestContext.WriteLine(Span("blind tries total", m => m.BlindTriesTotal));
            TestContext.WriteLine(Span("blind tries per exit", m => m.BlindTriesPerExit));
            TestContext.WriteLine(Span("freedom at start", m => m.FreedomStart));
            TestContext.WriteLine(Span("freedom mean", m => m.FreedomMean));
            TestContext.WriteLine(Span("bottleneck steps", m => m.BottleneckSteps));
            TestContext.WriteLine(Span("fewest choices", m => m.FewestChoices));
            TestContext.WriteLine(Span("nailed share", m => m.NailedShare));
            foreach (var m in ms)
                TestContext.WriteLine($"  {m.LevelId} blind {m.BlindTriesTotal:0.0} ({m.BlindTriesPerExit:0.00}/exit) freedom {m.FreedomStart:0.00}/{m.FreedomMean:0.00} bottleneck {m.BottleneckSteps} fewest {m.FewestChoices} nailed {m.NailedShare:0.00}");
            Assert.That(ms.Count, Is.EqualTo(24), "L01-L20, L31, L32, L33, L36");
        }
    }
}
