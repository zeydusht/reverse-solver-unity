using NUnit.Framework;
using ReverseSolver.Presentation;

namespace ReverseSolver.Core.Tests
{
    public sealed class JigoMoodTests
    {
        [TestCase(11, null, false, false, false, JigoState.Normal)]
        [TestCase(11, null, true, false, false, JigoState.Mutlu)]
        [TestCase(10, null, true, false, false, JigoState.Tedirgin)]
        [TestCase(3, null, true, false, false, JigoState.Korku)]
        [TestCase(4, 2, false, false, false, JigoState.Tedirgin)]
        [TestCase(30, 1, true, false, false, JigoState.Korku)]
        [TestCase(30, 2, true, false, false, JigoState.Mutlu)]
        [TestCase(3, 1, true, true, true, JigoState.Uzgun)]
        [TestCase(30, null, false, true, false, JigoState.Uzgun)]
        [TestCase(30, null, false, false, true, JigoState.Mutlu)]
        [TestCase(1, 1, false, false, true, JigoState.Mutlu)]
        public void PriorityAndThresholds(float time, int? fuse, bool happy, bool lost, bool won, JigoState expected)
        {
            Assert.That(JigoMood.Read(time, fuse, happy, lost, won), Is.EqualTo(expected));
        }

        [Test]
        public void FractionalTimeAboveThresholdDoesNotEnterEarly()
        {
            Assert.That(JigoMood.Read(10.01f, null, false, false, false), Is.EqualTo(JigoState.Normal));
            Assert.That(JigoMood.Read(3.01f, null, false, false, false), Is.EqualTo(JigoState.Tedirgin));
            Assert.That(JigoMood.Read(0, null, true, false, false), Is.EqualTo(JigoState.Korku));
        }
    }
}
