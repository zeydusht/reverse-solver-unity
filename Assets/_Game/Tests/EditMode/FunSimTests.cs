using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using ReverseSolver.Core;
using ReverseSolver.Editing;
using ReverseSolver.Generation;

namespace ReverseSolver.Core.Tests
{
    /* G4: direction tests of the fun estimate on hand-made boards. */
    public class FunSimTests
    {
        /* 4 x 5, every seam flat: any edge piece leaves, reading stays low. */
        static LevelData Open(int timer)
        {
            var d = LevelDraft.New(4, 5);
            d.SetTimer(timer);
            return d.ToLevelData();
        }

        [Test]
        public void ObviouslyEasyBoardInAHardSlotIsBoring()
        {
            var r = FunSim.Run(Open(75), Genre.Zor);
            TestContext.WriteLine("open 4x5, 75 s, Zor: " + r);
            Assert.That(r.BoredRate, Is.GreaterThan(.5));
            Assert.That(r.EnjoyRate, Is.LessThan(.5));
            var breather = FunSim.Run(Open(75), Genre.Nefes);
            Assert.That(breather.BoredRate, Is.EqualTo(0), "a breather is allowed to be easy");
        }

        [Test]
        public void ImpossibleTimerFrustrates()
        {
            var r = FunSim.Run(Open(5), Genre.Orta);
            TestContext.WriteLine("open 4x5, 5 s: " + r);
            Assert.That(r.FrustrationRate, Is.GreaterThan(.9));
            Assert.That(r.WinBy3Rate, Is.LessThan(.05));
        }

        /* Timer at the median player's time: many finish (or fail) close to the limit. */
        [Test]
        public void TimerAtTheMedianGivesNearMisses()
        {
            var tight = FunSim.Run(Open(16), Genre.Zor);      // 20 pieces x ~0.8 s
            var loose = FunSim.Run(Open(60), Genre.Zor);
            TestContext.WriteLine("16 s: " + tight + "\n60 s: " + loose);
            Assert.That(tight.NearMissRate, Is.GreaterThan(.15));
            Assert.That(tight.NearMissRate, Is.GreaterThan(loose.NearMissRate));
        }

        [Test]
        public void SameSeedSameResultAndLearningHelps()
        {
            var l = TestData.Levels["L22"];
            var a = FunSim.Run(l, Genre.Zor);
            var b = FunSim.Run(l, Genre.Zor);
            Assert.That((a.Enjoyed, a.Frustrated, a.Bored, a.WinFirst, a.NearMiss), Is.EqualTo((b.Enjoyed, b.Frustrated, b.Bored, b.WinFirst, b.NearMiss)));
            var noLearning = FunSim.Run(l, Genre.Zor, learning: false);
            TestContext.WriteLine("L22 learning: " + a + "\nL22 no learning: " + noLearning);
            Assert.That(a.WinBy3, Is.GreaterThanOrEqualTo(noLearning.WinBy3));
            Assert.That(a.WinFirst, Is.EqualTo(noLearning.WinFirst), "learning only touches later attempts");
        }

        // ---- G4-ek: the frozen assumptions V1-V4 (PRODUCT.md "Keyif eşikleri (kontrol noktası 3)") ----

        /* Legacy (m = 0, no V2/V3/V4) gives G4's first numbers exactly (recorded before the change). */
        [Test]
        public void LegacyGivesTheFirstVersionsNumbersExactly()
        {
            var expected = new[]
            {
                ("L22", Genre.Zor, 87, 213, 0, 44, 87, 11, 0),
                ("L40", Genre.CokZor, 195, 105, 0, 145, 196, 28, 156),
                ("L15", Genre.Zor, 243, 57, 0, 230, 245, 51, 215),
                ("L03", Genre.Kolay, 300, 0, 0, 299, 300, 2, 1),
            };
            foreach (var (id, g, enj, fr, bo, first, by3, nm, aha) in expected)
            {
                var r = FunSim.Run(TestData.Levels[id], g, FunSim.Population(300), options: FunSim.Options.Legacy);
                Assert.That((r.Enjoyed, r.Frustrated, r.Bored, r.WinFirst, r.WinBy3, r.NearMiss, r.AhaPlayers), Is.EqualTo((enj, fr, bo, first, by3, nm, aha)), id);
            }
        }

        /* V1: remembering earlier attempts does not lower the three-attempt win (m = 0 ≤ 0,5 ≤ 1,0 within noise). */
        [Test]
        public void MemoryRaisesTheThreeAttemptWin()
        {
            foreach (var (id, g) in new[] { ("L22", Genre.Zor), ("L15", Genre.Zor), ("L35", Genre.CokZor) })
            {
                var pop = FunSim.Population(1000);
                var by = new[] { 0, .5, 1.0 }.Select(m => FunSim.Run(TestData.Levels[id], g, pop, options: new FunSim.Options().With(m))).ToArray();
                TestContext.WriteLine($"{id} 3 denemede: m0 {by[0].WinBy3Rate:P0} · m0,5 {by[1].WinBy3Rate:P0} · m1 {by[2].WinBy3Rate:P0}");
                // memory changes the paths players take, so a run can differ by a few players: 1 % tolerance
                int tol = pop.Count / 100;
                Assert.That(by[0].WinBy3, Is.LessThanOrEqualTo(by[1].WinBy3 + tol), id);
                Assert.That(by[1].WinBy3, Is.LessThanOrEqualTo(by[2].WinBy3 + tol), id);
                Assert.That(by[0].WinFirst, Is.EqualTo(by[2].WinFirst), id + ": memory only acts from the second attempt");
            }
        }

        /* V2: after a bomb went off, the next attempt watches the bomb. */
        [Test]
        public void AttentionAfterABombHelpsTheNextAttempt()
        {
            var l = TestData.Levels["L22"];
            var pop = FunSim.Population(400);
            var off = new FunSim.Options { Memory = 0, AttentionAfterBomb = 0 };
            var on = new FunSim.Options { Memory = 0, AttentionAfterBomb = .8 };
            var a = FunSim.Run(l, Genre.Zor, pop, options: off);
            var b = FunSim.Run(l, Genre.Zor, pop, options: on);
            TestContext.WriteLine($"L22 3 denemede: V2 kapalı {a.WinBy3Rate:P0}, açık {b.WinBy3Rate:P0}");
            Assert.That(b.WinBy3, Is.GreaterThan(a.WinBy3));
            Assert.That(b.WinFirst, Is.EqualTo(a.WinFirst), "only attempts after a bomb change");
        }

        /* V3: only the bomb introduction slot gets the card's attention. */
        [Test]
        public void IntroductionAttentionOnlyOnTheBombSlot()
        {
            Assert.That(FunSim.OptionsFor(FactoryPlan.Slot(20)).BombIntro, Is.True);
            foreach (int slot in new[] { 3, 12, 21, 30, 40 }) Assert.That(FunSim.OptionsFor(FactoryPlan.Slot(slot)).BombIntro, Is.False, $"slot {slot}");
            var l = TestData.Levels["L21"];                     // the web game's bomb introduction
            var pop = FunSim.Population(400);
            var plain = FunSim.Run(l, Genre.Tanitim, pop, options: new FunSim.Options());
            var intro = FunSim.Run(l, Genre.Tanitim, pop, options: new FunSim.Options { BombIntro = true });
            TestContext.WriteLine($"L21 ilk deneme: dikkat U[0,1] {plain.FirstWinRate:P0}, U[0,8,1] {intro.FirstWinRate:P0}");
            Assert.That(intro.WinFirst, Is.GreaterThanOrEqualTo(plain.WinFirst));
        }

        /* V4: on Kolay a 3-look move is a knot, so fewer wins count as boring. */
        [Test]
        public void KolayKnotAtThreeLooks()
        {
            var cat = Catalog.From(TestData.Levels);
            var c = Generator.Make(FactoryPlan.Slot(2), new GenParams { Interlock = 1 }, 5, 20, cat);
            var cal = Calibration.Assess(c).Candidate.Level;
            var pop = FunSim.Population(400);
            var before = FunSim.Run(cal, Genre.Kolay, pop, options: new FunSim.Options { KolayKnotLooks = FunSim.KnotLooks });
            var after = FunSim.Run(cal, Genre.Kolay, pop, options: new FunSim.Options());
            TestContext.WriteLine($"slot 2 sıkıcı: düğüm 4 bakış {before.BoredRate:P0}, 3 bakış {after.BoredRate:P0}");
            Assert.That(after.Bored, Is.LessThanOrEqualTo(before.Bored));
        }

        [Test]
        public void FrustrationSplitsIntoUnfairAndDifficulty()
        {
            var r = FunSim.Run(Open(5), Genre.Orta, FunSim.Population(200));
            Assert.That(r.UnfairFrustrated + r.HardFrustrated, Is.EqualTo(r.Frustrated));
            Assert.That(r.HardFrustrated, Is.GreaterThan(0), "time running out with most pieces left is difficulty");
            var intro = FunSim.Run(Open(5), Genre.Tanitim, FunSim.Population(200));
            Assert.That(intro.UnfairFrustrated, Is.EqualTo(intro.Frustrated), "losing an introduction is unfair first");
        }

        /* Target: one candidate x 1000 players x 3 attempts in 2 s or less. */
        [Test]
        public void ThousandPlayersOnTheLargestBoard()
        {
            var l = TestData.Levels["L40"];
            FunSim.Run(l, Genre.CokZor, FunSim.Population(50));
            var sw = Stopwatch.StartNew();
            var r = FunSim.Run(l, Genre.CokZor);
            TestContext.WriteLine($"L40 x 1000 players: {sw.ElapsedMilliseconds} ms — {r}");
            Assert.That(r.Players, Is.EqualTo(1000));
        }
    }
}
