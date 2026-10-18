using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using ReverseSolver.Editing;
using ReverseSolver.Generation;

namespace ReverseSolver.Core.Tests
{
    /* The redesign set as it stands (PRODUCT.md "2026-10-17 SIRASI", independent review):
       every finalist is solvable without a bomb, freeze or valve, carries the redesign stock,
       and five drawn finalists reproduce their stored numbers and pass today's thresholds. */
    public class RedesignAuditTests
    {
        static RedesignStore _store;
        static RedesignStore Store => _store ??= RedesignStore.LoadOrNew();

        static RedesignStore.Entry[] Finalists => Store.Entries.Where(e => e.Status != RedesignStore.Rejected).ToArray();

        [Test]
        public void EveryFinalistIsSolvableCleanAndStocked()
        {
            Assert.That(Finalists, Is.Not.Empty);
            foreach (var e in Finalists)
            {
                var l = e.Draft.ToLevelData();
                var r = SmartPlayer.Play(l);
                Assert.That(r.Verdict, Is.EqualTo(SmartVerdict.Won), $"{e.Candidate}: {r}");
                Assert.That(r.Valves, Is.Zero, $"{e.Candidate}: valve");
                Assert.That(l.Solution, Is.Not.Empty, $"{e.Candidate}: stored solution");
                var g = new GameSession(l, 1, "audit", new Mulberry32(1));
                foreach (var m in l.Solution) g.TryExit(m.Piece, m.Dir);
                Assert.That(g.Outcome, Is.EqualTo(Outcome.Win), $"{e.Candidate}: stored solution wins");
                Assert.That(l.Number, Is.EqualTo(e.Slot), e.Candidate);
                foreach (var id in LevelDraft.BoosterIds)
                {
                    int at = SlotSpec.IntroducedAt[id];
                    Assert.That(l.Unlock[id], Is.EqualTo(at), $"{e.Candidate} {id} unlock");
                    Assert.That(l.Boosters[id], Is.EqualTo(e.Slot >= at ? 1 : 0), $"{e.Candidate} {id} stock");
                }
            }
        }

        /* Five finalists drawn with a fixed seed: the stored V5 numbers come back and pass. */
        [Test]
        public void FiveDrawnFinalistsReproduceAndPass()
        {
            var pop = FunSim.Population();
            var rng = new Mulberry32(20261017);
            var pool = Finalists.ToList();
            for (int k = 0; k < 5 && pool.Count > 0; k++)
            {
                var e = pool[(int)(rng.NextDouble() * pool.Count)];
                pool.Remove(e);
                var spec = FactoryPlan.Slot(e.Slot);
                var l = e.Draft.ToLevelData();
                var plain = FunSim.Run(l, spec.Genre, pop, options: FunSim.OptionsFor(spec));
                var boosted = FunSim.Run(l, spec.Genre, pop, options: FunSim.OptionsFor(spec, boosters: true));
                string at = $"{e.Candidate} (slot {e.Slot}): {boosted}";
                Assert.That(Math.Round(boosted.EnjoyRate, 3), Is.EqualTo(e.Scores["keyif"]), at);
                Assert.That(Math.Round(boosted.FrustrationRate, 3), Is.EqualTo(e.Scores["hayalKirikligi"]), at);
                Assert.That(Math.Round(plain.FirstWinRate, 3), Is.EqualTo(e.Scores["ilkDeneme"]), at);
                Assert.That(Math.Round(plain.EarlyTimeoutRate, 3), Is.EqualTo(e.Scores["erkenSure"]), at);
                var t = FunThresholds.For(spec.Genre);
                if (e.Scores.ContainsKey("erkenGevsetildi") || e.Scores.ContainsKey("bantGevsetildi")) t = t.Relaxed();
                Assert.That(t.Check(plain, boosted), Is.Empty, at);
                TestContext.WriteLine(at);
            }
        }
    }
}
