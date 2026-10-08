using NUnit.Framework;

namespace ReverseSolver.Core.Tests
{
    /* The level-1 hand (PRODUCT.md, M6 criteria 1-3): the web drawTutorial rule. */
    public class TutorialTests
    {
        static GameSession L01() => new GameSession(TestData.Levels["L01"], 1, "test", new Mulberry32(1));

        [Test]
        public void PointsAtTheWebMoveAndThatMoveLeaves()
        {
            var s = L01();
            Assert.That(Tutorial.Hint(s, true, false, out var m), Is.True);
            var first = TestData.Levels["L01"].Solution[0];
            Assert.That((m.Piece, m.Dir), Is.EqualTo((first.Piece, first.Dir)));       // web: LEVEL.solution[0]
            Assert.That((m.Piece, m.Dir), Is.EqualTo((16, Dir.L)));
            Assert.That(s.Probe(m.Piece, m.Dir).Exit, Is.True);
            Assert.That(TravelRule.ExitDirs(s.Board, m.Piece), Does.Contain(m.Dir));
            Assert.That(s.TryExit(m.Piece, m.Dir), Is.EqualTo(CommandResult.Ok));
        }

        [Test]
        public void GoneAfterAnyRemoval()
        {
            var s = L01();
            var free = TravelRule.FreePieces(s.Board);
            int other = free.Find(p => p != 16);
            s.TryExit(other, TravelRule.ExitDirs(s.Board, other)[0]);
            Assert.That(Tutorial.Hint(s, true, false, out _), Is.False, "board no longer untouched");
        }

        [Test]
        public void OnlyOnTheFirstLevelAndUntilDone()
        {
            var s = L01();
            Assert.That(Tutorial.Hint(s, false, false, out _), Is.False, "not the first level");
            Assert.That(Tutorial.Hint(s, true, true, out _), Is.False, "already done this visit");
            var l2 = new GameSession(TestData.Levels["L02"], 1, "test", new Mulberry32(1));
            Assert.That(Tutorial.Hint(l2, true, false, out var m2), Is.True, "the rule is the set's first level, whatever it is");
            Assert.That(l2.Probe(m2.Piece, m2.Dir).Exit, Is.True);
        }

        [Test]
        public void TimeAndJamsDoNotHideIt()
        {
            var s = L01();
            s.Tick(10);
            s.RecordJam(3);
            Assert.That(Tutorial.Hint(s, true, false, out _), Is.True);
        }

        [Test]
        public void AskingChangesNothing()
        {
            var s = L01();
            s.Tick(2.5);
            int events = s.Events.Count, time = s.TimeLeft, moves = s.Moves, jams = s.Jams;
            for (int i = 0; i < 10; i++) Tutorial.Hint(s, true, false, out _);
            Assert.That((s.Events.Count, s.TimeLeft, s.Moves, s.Jams, s.Board.PresentCount),
                        Is.EqualTo((events, time, moves, jams, 20)));
        }

        [Test]
        public void NotAfterTheLevelEnded()
        {
            var s = L01();
            s.Tick(61);
            Assert.That(s.Outcome, Is.EqualTo(Outcome.Time));
            Assert.That(Tutorial.Hint(s, true, false, out _), Is.False);
        }
    }
}
