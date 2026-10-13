using System.Diagnostics;
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
