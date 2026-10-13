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

        /* The opening sequence GameRoot runs, with a fake host: the gate stays open
           while 20 s pass, then the name arrives. */
        sealed class Opening
        {
            public readonly AttemptLog Log = new AttemptLog();
            public readonly System.Collections.Generic.List<GameSession> Started = new System.Collections.Generic.List<GameSession>();
            public int Menus;
            public string Player;
            public System.Action<string> Gate;

            public Opening(int requested, bool askName)
            {
                LaunchPlan.Run(requested, askName,
                    i => Started.Add(Log.Start(TestData.Levels.Levels[i], "test", new Mulberry32(1))),
                    () => Menus++,
                    done => Gate = done,
                    name => Player = name);
            }

            public void Wait(double seconds) { foreach (var s in Started) s.Tick(seconds); }
        }

        [Test]
        public void BehindTheGateNoAttemptAndNoClockUntilTheName()
        {
            var o = new Opening(1, askName: true);                 // ?lv=2, new player
            var l02 = TestData.Levels.Levels[1];
            Assert.That(o.Gate, Is.Not.Null, "the gate is asked for");
            Assert.That(o.Menus, Is.EqualTo(1), "the menu sits under the gate");
            o.Wait(20);
            Assert.That(o.Started, Is.Empty, "no session while the gate is open");
            Assert.That(o.Log.AttemptsOf(l02.Id), Is.EqualTo(0), "no attempt counted");

            o.Gate("TEST_X");
            Assert.That(o.Player, Is.EqualTo("TEST_X"), "the name is stored before the level opens");
            Assert.That(o.Started.Count, Is.EqualTo(1));
            Assert.That(o.Started[0].Level.Id, Is.EqualTo(l02.Id));
            Assert.That(o.Started[0].Attempt, Is.EqualTo(1));
            Assert.That(o.Started[0].TimeLeft, Is.EqualTo(l02.TimerSeconds), "full time after the gate");
        }

        [Test]
        public void RootAddressShowsTheMenuAndTheNameOpensNothing()
        {
            var o = new Opening(-1, askName: true);
            Assert.That(o.Menus, Is.EqualTo(1));
            o.Gate("TEST_X");
            Assert.That(o.Started, Is.Empty, "root flow: gate, then the menu");
        }

        [Test]
        public void KnownPlayerOpensTheLinkedLevelAtOnce()
        {
            var o = new Opening(1, askName: false);
            Assert.That(o.Gate, Is.Null, "no gate");
            Assert.That((o.Started.Count, o.Menus), Is.EqualTo((1, 0)));
            Assert.That(o.Log.AttemptsOf(TestData.Levels.Levels[1].Id), Is.EqualTo(1));
        }
    }
}
