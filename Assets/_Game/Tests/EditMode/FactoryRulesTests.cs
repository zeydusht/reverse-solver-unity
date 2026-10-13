using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using ReverseSolver.Core;
using ReverseSolver.Editing;
using ReverseSolver.Generation;

namespace ReverseSolver.Core.Tests
{
    /* PM 2026-10-14 gece (2) ("4-b"): early time-out caps, Orta's relaxation, Çok zor bomb margin,
       Nefes bomb depth, "çivi önemli", slot 4's nail, the redesign set's cards. */
    public class FactoryRulesTests
    {
        static Catalog _catalog;
        static Catalog Catalog => _catalog ??= Catalog.From(TestData.Levels);

        static FunSim.Result Early(int earlyOutOf100) => new FunSim.Result
        { Players = 100, Enjoyed = 80, Frustrated = 20, HardFrustrated = 20, WinFirst = 50, WinBy3 = 80, EarlyTimeout = earlyOutOf100 };

        [Test]
        public void EarlyTimeoutCapsZorAndCokZor()
        {
            var z = FunThresholds.For(Genre.Zor);
            Assert.That(z.Check(Early(25)), Has.None.StartsWith("erken süre"));
            Assert.That(z.Check(Early(26)), Has.Some.StartsWith("erken süre bitmesi %26"));
            var cz = FunThresholds.For(Genre.CokZor);
            var at35 = Early(35); at35.WinFirst = 30;
            var at36 = Early(36); at36.WinFirst = 30;
            Assert.That(cz.Check(at35), Has.None.StartsWith("erken süre"));
            Assert.That(cz.Check(at36), Has.Some.StartsWith("erken süre bitmesi %36"));
        }

        [Test]
        public void OrtaRelaxesOnlyWhenShortAndLabelled()
        {
            Assert.That(FunThresholds.RelaxAllowed(Genre.Orta, 1), Is.True);
            Assert.That(FunThresholds.RelaxAllowed(Genre.Orta, 2), Is.False);
            Assert.That(FunThresholds.RelaxAllowed(Genre.Zor, 0), Is.False);
            var t = FunThresholds.For(Genre.Orta);
            var r = Early(8); r.WinFirst = 75; r.Enjoyed = 90; r.Frustrated = 8; r.HardFrustrated = 8;
            Assert.That(t.Check(r), Has.Some.StartsWith("erken süre bitmesi %8"), "strict Orta cap is 5 %");
            Assert.That(t.Relaxed().Check(r), Has.None.StartsWith("erken süre"), "relaxed cap 10 %");
            Assert.That(t.MaxEarlyTimeout, Is.EqualTo(.05), "the relaxed copy leaves the original alone");

            var o = Factory.RunSlot(FactoryPlan.Slot(6), new FactoryOptions { BudgetOrta = 80, BandWant = 3, Players = 200, SearchPlayers = 100, MemoryTable = false }, Catalog);
            var strict = o.Checked.Count(f => f.Accepted && !f.EarlyRelaxed);
            foreach (var f in o.Checked.Where(f => f.EarlyRelaxed))
            {
                Assert.That(strict, Is.LessThan(2), "relaxed candidates only when the slot is short");
                var s = new RedesignStore();
                s.PutFinalists(o, 3);
                foreach (var e in s.Entries.Where(e => e.Candidate == f.Candidate.Id)) Assert.That(e.Notes, Has.Some.StartsWith("erken süre gevşetildi"));
            }
            TestContext.WriteLine($"{o.Search} → kabul {strict}, gevşetilmiş {o.Checked.Count(f => f.EarlyRelaxed && f.Accepted)}");
        }

        /* Çok zor in the factory: margin 3-5, never 2. */
        [Test]
        public void CokZorBombMarginThreeToFive()
        {
            int bombs = 0;
            foreach (int margin in new[] { 3, 5, 2 })
                for (uint seed = 1; seed <= 12; seed++)
                {
                    var c = Generator.Make(FactoryPlan.Slot(30), new GenParams { Interlock = 4, Sweep = 1, BombMargin = margin }, seed, 20, Catalog);
                    if (c == null || c.Level.Bombs.Count == 0) continue;
                    var a = Calibration.Assess(c);
                    var l = a.Candidate.Level;
                    var m = StructureMetrics.Of(l);
                    foreach (var b in l.Bombs)
                    {
                        int got = b.Value - m.BombDigs[b.Key].dig;
                        Assert.That(got, Is.InRange(3, 5), $"{c.Id}: margin {got}");
                        if (margin != 2) Assert.That(got, Is.EqualTo(margin), c.Id);
                        bombs++;
                    }
                }
            Assert.That(bombs, Is.GreaterThan(0));
        }

        /* Nefes: a bomb that leaves on the first move (dig 1) is out; dig 2 is fine. */
        [Test]
        public void NefesBombNeedsADigOfTwo()
        {
            Candidate Hand(Cell bomb)
            {
                var d = LevelDraft.New(4, 5);
                d.SetTimer(60);
                d.SetNumber(26);
                d.SetBomb(bomb, 12);
                d.Solution = DesignCheck.Run(d.ToLevelData()).Solution;
                return new Candidate { Id = "HAND", Spec = FactoryPlan.Slot(26), Draft = d, Level = d.ToLevelData(), Params = new GenParams() };
            }
            Assert.That(Calibration.Assess(Hand(new Cell(0, 0))).Unfair, Has.Some.StartsWith("nefeste bomba ilk hamlede"));
            Assert.That(Calibration.Assess(Hand(new Cell(1, 1))).Unfair, Has.None.StartsWith("nefeste bomba"));
        }

        /* "Çivi önemli": the nailed piece leaves at most 3 moves after its counter ran out. */
        [Test]
        public void NailMattersWithinThreeMoves()
        {
            var d = LevelDraft.New(4, 5);
            d.SetTimer(60);
            d.SetNail(new Cell(0, 0), 4);
            var l = d.ToLevelData();
            int nailed = l.Nails.Keys.Single();
            // a recorded order where the nailed piece is the (4 + slack)-th move
            List<Move> OrderWith(int slack)
            {
                var others = Enumerable.Range(0, l.Pieces.Count).Where(p => p != nailed).Select(p => new Move(p, Dir.U)).ToList();
                others.Insert(4 + slack, new Move(nailed, Dir.U));
                return others;
            }
            d.Solution = OrderWith(3);
            Assert.That(Calibration.NailSlack(d.ToLevelData()).Single().slack, Is.EqualTo(3));
            d.Solution = OrderWith(4);
            Assert.That(Calibration.NailSlack(d.ToLevelData()).Single().slack, Is.EqualTo(4));

            // as a condition on slots 3 and 4
            var spec = FactoryPlan.Slot(3);
            int checkedNails = 0;
            for (uint seed = 1; seed <= 15; seed++)
            {
                var c = Generator.Make(spec, new GenParams(), seed, 20, Catalog);
                var a = Calibration.Assess(c);
                var slack = Calibration.NailSlack(a.Candidate.Level);
                bool idle = slack.Any(x => x.slack > Calibration.NailSlackMax);
                Assert.That(a.Unfair.Any(u => u.StartsWith("çivi süs")), Is.EqualTo(idle), c.Id);
                checkedNails += slack.Count;
            }
            Assert.That(checkedNails, Is.GreaterThan(0));
        }

        [Test]
        public void SlotFourAlwaysHasItsNail()
        {
            var spec = FactoryPlan.Slot(4);
            Assert.That((spec.MinNails, spec.MaxNails), Is.EqualTo((1, 1)));
            for (uint seed = 1; seed <= 10; seed++)
                Assert.That(LevelStats.Of(Generator.Make(spec, new GenParams(), seed, 20, Catalog).Level).Nails, Is.EqualTo(1));
        }

        /* The redesign set's cards: Turkish letters, the font has every glyph, the calendar is right. */
        [Test]
        public void RedesignCardsReadInTurkish()
        {
            var atlas = new HashSet<int>();
            foreach (var line in File.ReadAllLines("Assets/_Game/Presentation/Fonts/RS Sans SDF.asset"))
            {
                var t = line.Trim();
                if (t.StartsWith("m_Unicode: ")) atlas.Add(int.Parse(t.Substring(11)));
            }
            foreach (var thing in new[] { SlotSpec.Scissors, SlotSpec.Wand, SlotSpec.Hammer, SlotSpec.Clock, SlotSpec.Nail, SlotSpec.Bomb, SlotSpec.Seal, SlotSpec.Chain })
            {
                var card = Catalog.IntroFor(thing);
                Assert.That(card, Is.Not.Null, thing);
                foreach (var text in new[] { card.Title, card.Body, card.Tip }.Where(x => x != null))
                    foreach (char ch in text)
                        Assert.That(atlas.Contains(ch), Is.True, $"{thing}: '{ch}' (U+{(int)ch:X4}) is not in the font atlas");
            }
            Assert.That(Catalog.IntroFor(SlotSpec.Wand).Title, Is.EqualTo("Değnek"));
            Assert.That(Catalog.IntroFor(SlotSpec.Wand).Body, Does.Contain("karıştırır"));
            Assert.That(Catalog.IntroFor(SlotSpec.Hammer).Title, Is.EqualTo("Çekiç"));
            Assert.That(Catalog.IntroFor(SlotSpec.Clock).Tip, Is.EqualTo("Süre daralınca en hızlı çözüm budur."));
            Assert.That(Catalog.IntroFor(SlotSpec.Nail).Tip, Does.Contain("23. bölümde çekiç"));
            Assert.That(Catalog.IntroFor(SlotSpec.Bomb).Tip, Is.Null, "the hammer opens at 23");
            Assert.That(TestData.Levels["L06"].BoosterIntro.Title, Is.EqualTo("Degnek"), "the live 40 keep their cards");
        }
    }
}
