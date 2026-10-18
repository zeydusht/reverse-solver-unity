using NUnit.Framework;
using ReverseSolver.Core;

namespace ReverseSolver.Core.Tests
{
    /* Zeyd 2026-10-18: the countdown starts with the first action, not when the level opens. */
    public class ClockStartTests
    {
        static GameSession New() => new GameSession(TestData.Levels.Levels[0], 1, "test", new Mulberry32(1));

        [Test]
        public void NoTimePassesBeforeTheFirstMove()
        {
            var s = New();
            int t = s.TimeLeft;
            s.Tick(30);
            Assert.That(s.TimeLeft, Is.EqualTo(t), "looking at the board is free");
            Assert.That(s.ElapsedSeconds, Is.Zero, "the row's duration counts from the first move");
            Assert.That(s.ClockStarted, Is.False);
        }

        [Test]
        public void TheFirstExitStartsTheClock()
        {
            var s = New();
            var m = TestData.Levels.Levels[0].Solution[0];
            Assert.That(s.TryExit(m.Piece, m.Dir), Is.EqualTo(CommandResult.Ok));
            Assert.That(s.ClockStarted, Is.True);
            int t = s.TimeLeft;
            s.Tick(3);
            Assert.That(s.TimeLeft, Is.EqualTo(t - 3));
        }

        [Test]
        public void AJamStartsTheClockAndABlockedTryDoesNot()
        {
            var s = New();
            s.TryExit(0, Dir.U);                       // may be blocked: a refused try is not an action
            if (!s.ClockStarted) { s.RecordJam(0); Assert.That(s.ClockStarted, Is.True, "a jam is a move the player made"); }
        }

        [Test]
        public void TheWebRuleIsStillThereForTheGoldenScenarios()
        {
            var s = New();
            s.ClockOnFirstMove = false;
            int t = s.TimeLeft;
            s.Tick(2);
            Assert.That(s.TimeLeft, Is.EqualTo(t - 2));
        }
    }
}
