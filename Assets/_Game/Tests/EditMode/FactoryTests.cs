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

        [Test]
        public void ThresholdsNameWhatFails()
        {
            var t = FunThresholds.For(Genre.Zor);
            // Zor: keyif >= 85, total frustration <= 15, unfair <= 5, first try 35-65
            var ok = new FunSim.Result { Players = 100, Enjoyed = 86, Frustrated = 14, HardFrustrated = 14, WinFirst = 50, WinBy3 = 86, NearMiss = 10 };
            Assert.That(t.Check(ok), Is.Empty, "14 % frustration passes on Zor; near-miss never filters");
            var bad = new FunSim.Result { Players = 100, Enjoyed = 80, Frustrated = 16, HardFrustrated = 16, WinFirst = 76, WinBy3 = 84, NearMiss = 10 };
            var why = t.Check(bad);
            Assert.That(why, Has.Some.StartsWith("keyif %80"));
            Assert.That(why, Has.Some.StartsWith("hayal kırıklığı %16"));
            Assert.That(why, Has.Some.StartsWith("ilk deneme %76"));
            foreach (var g in GenreExt.All)
            {
                var unfair = new FunSim.Result { Players = 100, Enjoyed = 94, Frustrated = 6, UnfairFrustrated = 6, WinFirst = 97, WinBy3 = 98 };
                Assert.That(FunThresholds.For(g).Check(unfair), Has.Some.StartsWith("haksız hayal kırıklığı %6"), g.ToString());
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
