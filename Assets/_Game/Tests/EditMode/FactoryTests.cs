using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ReverseSolver.Core;
using ReverseSolver.Editing;
using ReverseSolver.Generation;

namespace ReverseSolver.Core.Tests
{
    /* G6: the slot run, the psychologist's thresholds and redesign.json. */
    public class FactoryTests
    {
        static Catalog _catalog;
        static Catalog Catalog => _catalog ??= Catalog.From(TestData.Levels);

        static FactoryOptions Quick => new FactoryOptions { BudgetEasy = 40, BandWant = 4, Players = 200, Finalists = 3 };

        /* Checkpoint 4 thresholds (PRODUCT.md "Keyif eşikleri (kontrol noktası 4)"). */
        [Test]
        public void ThresholdsNameWhatFails()
        {
            var t = FunThresholds.For(Genre.Zor);                 // keyif >= 70, total <= 30, unfair <= 5, early time-out <= 25, first try 35-65
            var ok = new FunSim.Result { Players = 100, Enjoyed = 70, Frustrated = 30, HardFrustrated = 30, WinFirst = 50, WinBy3 = 70, NearMiss = 10, EarlyTimeout = 25 };
            Assert.That(t.Check(ok), Is.Empty, "Zor: 30 % frustration and 25 % early time-outs pass; near-miss never filters");
            var bad = new FunSim.Result { Players = 100, Enjoyed = 69, Frustrated = 31, HardFrustrated = 31, WinFirst = 76, WinBy3 = 69, NearMiss = 10, EarlyTimeout = 26 };
            var why = t.Check(bad);
            Assert.That(why, Has.Some.StartsWith("keyif %69"));
            Assert.That(why, Has.Some.StartsWith("hayal kırıklığı %31"));
            Assert.That(why, Has.Some.StartsWith("ilk deneme %76"));
            Assert.That(why, Has.Some.StartsWith("erken süre bitmesi %26"));
            foreach (var g in GenreExt.All)
            {
                var unfair = new FunSim.Result { Players = 100, Enjoyed = 94, Frustrated = 6, UnfairFrustrated = 6, WinFirst = 97, WinBy3 = 98 };
                Assert.That(FunThresholds.For(g).Check(unfair), Has.Some.StartsWith("haksız hayal kırıklığı %6"), g.ToString());
            }
            Assert.That(FunThresholds.For(Genre.Kolay).MaxEarlyTimeout, Is.EqualTo(1), "T/N/K unchanged: no early time-out cap");
        }

        /* Checkpoint 4: a searched timer may go down to P = 1.0 (T/N/K keep 1.1). */
        [Test]
        public void SearchedTimerFloorIsOne()
        {
            var d = LevelDraft.New(5, 5);                        // 25 pieces
            d.SetTimer(49);
            List<string> U(LevelDraft draft, Genre g)
            {
                var l = draft.ToLevelData();
                return Calibration.UnfairReasons(l, SlotSpec.For(g == Genre.Zor ? 14 : 2, g), StructureMetrics.Of(l), l.TimerSeconds, 2.0);
            }
            // s = 2.0 s/piece: 49 s → P 0.98, 50 s → P 1.00
            Assert.That(U(d, Genre.Zor), Has.Some.Contains("süre payı 0,98").Or.Some.Contains("süre payı 0.98"));
            d.SetTimer(50);
            Assert.That(U(d, Genre.Zor), Has.None.Contains("süre payı"));
            Assert.That(U(d, Genre.Kolay), Has.Some.Contains("süre payı"), "Kolay keeps the 1.1 floor");
        }

        /* Checkpoint 4: at most one curve exception, only where the slot is short of two finalists. */
        [Test]
        public void CurveExceptionOnlyWhenShortAndAtMostOne()
        {
            var o = Factory.RunSlot(FactoryPlan.Slot(14), new FactoryOptions { BudgetZor = 120, BandWant = 3, Players = 200, SearchPlayers = 100, MemoryTable = false }, Catalog);
            var accepted = o.Checked.Where(f => f.Accepted).ToList();
            int exceptions = accepted.Count(f => f.CurveException);
            TestContext.WriteLine($"{o.Search} → {accepted.Count} kabul, {exceptions} eğri istisnası; erken bakılan {o.Search.CurveNear.Count}");
            Assert.That(exceptions, Is.LessThanOrEqualTo(1));
            if (exceptions == 1) Assert.That(accepted.Count(f => !f.CurveException), Is.LessThan(2));
            foreach (var f in accepted.Where(f => f.CurveException)) Assert.That(f.Assessment.Curve.Fit, Is.GreaterThanOrEqualTo(.40));
            foreach (var f in o.Checked)
            {
                var l = f.Candidate.Level;
                double gs = l.Pieces.Count * DifficultyModel.SecondsPerPiece(f.Assessment.Structure.ReadingPerMove, l.Bombs.Count > 0, false);
                Assert.That(l.TimerSeconds / gs, Is.GreaterThanOrEqualTo(1.0 - 1e-9), f.Candidate.Id + ": P >= 1.0");
                Assert.That(l.TimerSeconds, Is.LessThanOrEqualTo(75));
            }
        }

        [Test]
        public void SlotRunGivesCheckedDistinctFinalists()
        {
            var o = Factory.RunSlot(FactoryPlan.Slot(1), Quick, Catalog);
            TestContext.WriteLine($"{o.Search} → {o.Chosen.Count} kabul");
            Assert.That(o.Checked, Is.Not.Empty);
            foreach (var f in o.Checked)
            {
                Assert.That(f.Assessment.Finalist, Is.True);
                Assert.That(f.Fun.Players, Is.EqualTo(200));
            }
            var chosen = o.Chosen;
            Assert.That(chosen.Select(f => f.RankKey), Is.EqualTo(chosen.Select(f => f.RankKey).OrderBy(k => k)));
            Assert.That(chosen.All(f => f.Accepted), Is.True);
        }

        [Test]
        public void RedesignStoreRoundTripsApprovesAndKeepsApprovedOnRerun()
        {
            var o = Factory.RunSlot(FactoryPlan.Slot(1), Quick, Catalog);
            Assume.That(o.Chosen.Count, Is.GreaterThanOrEqualTo(2));
            var s = new RedesignStore();
            s.PutFinalists(o, 3);
            int n = s.Entries.Count;
            Assert.That(s.Entries.All(e => e.Status == RedesignStore.Finalist && e.Slot == 1 && e.Scores.ContainsKey("keyif")), Is.True);

            var first = s.Entries[0];
            Assert.That(s.Approve(first, "güzel"), Is.EqualTo("R01"));
            Assert.That(s.Approve(first), Is.EqualTo("R01"), "approving twice keeps the id");
            s.Reject(s.Entries[1], "çok kolay");
            Assert.That(s.NextId, Is.EqualTo(2));

            var json = s.ToJson();
            var back = RedesignStore.Load(json);
            Assert.That(back.NextId, Is.EqualTo(2));
            Assert.That(back.Entries.Select(e => (e.Draft.Id, e.Status, e.Slot, e.Candidate)),
                        Is.EqualTo(s.Entries.Select(e => (e.Draft.Id, e.Status, e.Slot, e.Candidate))));
            Assert.That(back.Entries[0].Notes, Has.Some.EqualTo("Zeyd: güzel"));
            Assert.That(back.Entries[0].Draft.GameplayKey(), Is.EqualTo(first.Draft.GameplayKey()));

            // the game reads it like levels.json: same pieces, the solution plays
            var set = LevelParser.Parse(json);
            foreach (var l in set.Levels)
            {
                var g = new GameSession(l, 1, "test", new Mulberry32(1));
                foreach (var m in l.Solution) g.TryExit(m.Piece, m.Dir);
                Assert.That(g.Outcome, Is.EqualTo(Outcome.Win), l.Id);
            }

            // a new run of the slot replaces unapproved finalists only; R01 never comes back as a new id
            back.PutFinalists(o, 3);
            Assert.That(back.Entries.Count(e => e.Status == RedesignStore.Approved), Is.EqualTo(1));
            Assert.That(back.Entries.Count(e => e.Status == RedesignStore.Rejected), Is.EqualTo(1));
            var again = back.Entries.First(e => e.Status == RedesignStore.Finalist);
            Assert.That(back.Approve(again), Is.EqualTo("R02"));
        }

        /* Parallel parts join without touching approved levels. */
        [Test]
        public void PartsMergeAndKeepApproved()
        {
            var o = Factory.RunSlot(FactoryPlan.Slot(1), Quick, Catalog);
            Assume.That(o.Chosen.Count, Is.GreaterThanOrEqualTo(2));
            var main = new RedesignStore();
            main.PutFinalists(o, 1);
            main.Approve(main.Entries[0]);
            var part = new RedesignStore();
            part.PutFinalists(o, 3);
            main.MergeFinalists(RedesignStore.Load(part.ToJson()));
            Assert.That(main.Entries.Count(e => e.Status == RedesignStore.Approved), Is.EqualTo(1));
            Assert.That(main.Entries.Select(e => e.Candidate).Distinct().Count(), Is.EqualTo(main.Entries.Count), "no candidate twice");
            Assert.That(main.Entries.Count, Is.EqualTo(System.Math.Min(3, o.Chosen.Count)));
        }
    }
}
