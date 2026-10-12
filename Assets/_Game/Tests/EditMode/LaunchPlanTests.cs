using NUnit.Framework;
using ReverseSolver.Presentation;

namespace ReverseSolver.Core.Tests
{
    /* A6b (PRODUCT.md 2026-10-13): behind the name gate no attempt starts and no clock runs. */
    public class LaunchPlanTests
    {
        [Test]
        public void ALinkedLevelWaitsBehindTheNameGate()
        {
            var p = LaunchPlan.For(2, askName: true);
            Assert.That(p.StartNow, Is.False, "nothing starts while the gate is open: no attempt, no clock");
            Assert.That(p.AfterName, Is.EqualTo(2), "the linked level opens once the name is in");
        }

        [Test]
        public void WithoutTheGateTheLinkedLevelOpensAtOnce()
        {
            var p = LaunchPlan.For(2, askName: false);
            Assert.That((p.StartNow, p.Level, p.AfterName), Is.EqualTo((true, 2, -1)));
        }

        [Test]
        public void NoLinkMeansTheMenuGateOrNot()
        {
            foreach (bool gate in new[] { true, false })
            {
                var p = LaunchPlan.For(-1, gate);
                Assert.That((p.StartNow, p.AfterName), Is.EqualTo((false, -1)), "root address: menu, unchanged");
            }
        }

        /* The attempt counter only moves when a session starts: deferring the start keeps it at zero. */
        [Test]
        public void NoAttemptIsCountedBeforeTheLevelStarts()
        {
            var log = new AttemptLog();
            var l = TestData.Levels["L03"];
            var s = log.Start(l, "test", new Mulberry32(1));   // what StartLevel does after the gate
            Assert.That(s.Attempt, Is.EqualTo(1));
            Assert.That(s.TimeLeft, Is.EqualTo(l.TimerSeconds), "full time when it starts");
        }
    }
}
