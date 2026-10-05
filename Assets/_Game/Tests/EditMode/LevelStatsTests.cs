using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ReverseSolver.Core.Tests
{
    /* M1 acceptance 3: obstacle counts derived by LevelStats agree with the web
       data, counted independently from the raw JSON, and with the chapter plan
       in the web README. */
    public class LevelStatsTests
    {
        static IEnumerable<string> Ids => TestData.LevelIds();

        [TestCaseSource(nameof(Ids))]
        public void CountsMatchRawWebData(string id)
        {
            var s = LevelStats.Of(TestData.Levels[id]);
            var raw = TestData.RawLevel(id);

            Assert.That(s.Width, Is.EqualTo(raw.Get("w").Int()));
            Assert.That(s.Height, Is.EqualTo(raw.Get("h").Int()));
            Assert.That(s.Pieces, Is.EqualTo(raw.Get("pieces").Count));
            Assert.That(s.Chains, Is.EqualTo(raw.Get("pieces").Items.Count(p => p.Count > 1)), "chains");
            Assert.That(s.Nails, Is.EqualTo(raw.Get("nails").Count), "nails");
            Assert.That(s.Bombs, Is.EqualTo(raw.Get("bombs").Count), "bombs");
            Assert.That(s.SealedLanes, Is.EqualTo(raw.Get("sealed").Members.Sum(m => m.Value.Count)), "sealed lanes");
            Assert.That(s.OpenPercent, Is.EqualTo(raw.Get("open").Int()), "open joint % reproduces the generator's field");
        }

        /* web README, "Bölümler": nails from 5, distant chains from 11, bombs from 21, sealed edges from 31. */
        [Test]
        public void ObstaclesFirstAppearWhereTheChaptersIntroduceThem()
        {
            var levels = TestData.Levels.Levels;
            int First(System.Func<LevelStats, bool> has) =>
                levels.First(l => has(LevelStats.Of(l))).Number;

            Assert.That(First(s => s.Nails > 0), Is.EqualTo(5), "nails");
            Assert.That(First(s => s.Chains > 0), Is.EqualTo(11), "chains");
            Assert.That(First(s => s.Bombs > 0), Is.EqualTo(21), "bombs");
            Assert.That(First(s => s.SealedLanes > 0), Is.EqualTo(31), "sealed edges");
        }

        [Test]
        public void ChainsAreDistantNotAdjacent()
        {
            // README: only distant chains are used; adjacent ones made levels easier.
            foreach (var l in TestData.Levels.Levels)
                foreach (var p in l.Pieces.Where(p => p.Length > 1))
                {
                    int dist = System.Math.Abs(p[0].X - p[1].X) + System.Math.Abs(p[0].Y - p[1].Y);
                    Assert.That(dist, Is.GreaterThan(1), $"{l.Id}: chain {p[0]}-{p[1]} is adjacent");
                }
        }
    }
}
