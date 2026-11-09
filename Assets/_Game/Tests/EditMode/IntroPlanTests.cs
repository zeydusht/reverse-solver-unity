using NUnit.Framework;

namespace ReverseSolver.Core.Tests
{
    public class IntroPlanTests
    {
        static bool Opening(string param, bool[] flags) => IntroPlan.ShowOpening(param,
            flags[0], flags[1], flags[2], flags[3], flags[4], flags[5], flags[6], flags[7], flags[8]);
        static bool Ending(string param, bool[] flags) => IntroPlan.EndingEligible(param,
            flags[0], flags[1], flags[2], flags[3], flags[4], flags[5], flags[6], flags[7], flags[8]);

        [TestCase(null)]
        [TestCase("")]
        [TestCase("other")]
        public void FreshRedesignVisitShowsBoth(string param)
        {
            var flags = new[] { false, false, false, false, false, false, true, false, false };
            Assert.That(Opening(param, flags), Is.True);
            Assert.That(Ending(param, flags), Is.True);
        }

        [Test]
        public void EveryFilterBlocksTheDefault()
        {
            for (int i = 0; i < 9; i++)
            {
                var flags = new[] { false, false, false, false, false, false, true, false, false };
                flags[i] = !flags[i];
                Assert.That(Opening(null, flags), Is.False, "opening filter " + i);
                Assert.That(Ending(null, flags), Is.False, "ending filter " + i);
            }
        }

        [Test]
        public void ExplicitParametersOverrideEveryCombination()
        {
            for (int mask = 0; mask < 512; mask++)
            {
                var flags = new bool[9];
                for (int i = 0; i < flags.Length; i++) flags[i] = (mask & (1 << i)) != 0;
                Assert.That(Opening("0", flags), Is.False);
                Assert.That(Ending("0", flags), Is.False);
                Assert.That(Opening("1", flags), Is.True);
                Assert.That(Ending("1", flags), Is.True);
            }
        }

        [Test]
        public void EndingDependsOnFirstWinRatherThanAnyProgress()
        {
            Assert.That(IntroPlan.ShowOpening(null, false, false, false, false, false, false, true, false, true), Is.False);
            Assert.That(IntroPlan.EndingEligible(null, false, false, false, false, false, false, true, false, false), Is.True);
            Assert.That(IntroPlan.EndingEligible(null, false, false, false, false, false, false, true, false, true), Is.False);
        }
    }
}
