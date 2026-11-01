using System.IO;
using System.Linq;
using NUnit.Framework;
using ReverseSolver.Presentation;

namespace ReverseSolver.Core.Tests
{
    public class TutorialScriptTests
    {
        static TutorialScript Script() => TutorialScript.Parse(File.ReadAllText("Assets/_Game/Presentation/Resources/tutorial.json"));
        static LevelSet Levels() => LevelParser.Parse(File.ReadAllText("Assets/_Game/Levels/redesign.json"));

        [Test]
        public void EveryDragUsesTheSolutionAndReallyExitsAfterPreviousSteps()
        {
            var levels = Levels();
            var steps = Script().steps.Where(s => s.tur == "surukle").GroupBy(s => s.slot);
            foreach (var slot in steps)
            {
                var level = levels.Levels[slot.Key - 1];
                var session = new GameSession(level, 1, "test", new Mulberry32(1));
                foreach (var step in slot.OrderBy(s => s.adim))
                {
                    Assert.That(TutorialScript.MoveOf(level, step, out var move), Is.True);
                    Assert.That(move.Dir, Is.EqualTo(level.Solution.First(m => m.Piece == step.parca).Dir));
                    Assert.That(session.NailAt(move.Piece), Is.Zero);
                    Assert.That(session.Probe(move.Piece, move.Dir).Exit, Is.True, $"slot {slot.Key}, step {step.adim}");
                    Assert.That(session.TryExit(move.Piece, move.Dir), Is.EqualTo(CommandResult.Ok));
                }
                Assert.That(session.Moves, Is.EqualTo(slot.Count()), "tutorial moves are ordinary moves");
                Assert.That(session.Jams, Is.Zero);
            }
            var first = Script().ForSlot(1, true);
            Assert.That(first.Select(s => s.tur), Is.EqualTo(new[] { "surukle", "surukle", "kural" }));
            Assert.That((first[1].parca, levels.Levels[0].Solution[1].Dir), Is.EqualTo((6, Dir.D)));
        }

        [Test]
        public void EveryObstacleHasARealCellIncludingTheSealedWallLane()
        {
            var levels = Levels();
            foreach (var step in Script().steps.Where(s => s.tur == "goster"))
            {
                var level = levels.Levels[step.slot - 1];
                Assert.That(TutorialScript.ObstacleOf(level, step.engel, out var cell, out var side), Is.True, step.engel);
                Assert.That(level.OnBoard(cell), Is.True);
                if (step.engel == "wall")
                {
                    Assert.That(side.HasValue, Is.True);
                    Assert.That(level.Sealed[side.Value], Does.Contain(side.Value.IsVertical() ? cell.X : cell.Y));
                }
                else
                {
                    int piece = new Board(level).PieceAt(cell);
                    Assert.That(piece, Is.GreaterThanOrEqualTo(0));
                    if (step.engel == "nail") Assert.That(level.Nails.ContainsKey(piece), Is.True);
                    if (step.engel == "bomb") Assert.That(level.Bombs.ContainsKey(piece), Is.True);
                    if (step.engel == "chain") Assert.That(level.Pieces[piece].Length, Is.GreaterThan(1));
                }
            }
            Assert.That(TutorialScript.ObstacleOf(levels.Levels[0], "nail", out _, out _), Is.False);
        }

        [Test]
        public void RetryRepeatsDragsAndRuleButDoesNotRepeatObstacleIntro()
        {
            var script = Script();
            Assert.That(script.ForSlot(1, false).Count, Is.EqualTo(3));
            foreach (int slot in new[] { 3, 6, 10, 14 })
            {
                Assert.That(script.ForSlot(slot, true).Count, Is.EqualTo(1));
                Assert.That(script.ForSlot(slot, false), Is.Empty);
            }
        }

        [TestCase(Dir.L, 40, 0)]
        [TestCase(Dir.L, 0, -40)]
        [TestCase(Dir.D, 0, -40)]
        [TestCase(Dir.D, -40, 0)]
        public void WrongDirectionIsRejectedBeforeDragModelCanJam(Dir expected, float dx, float dy)
        {
            var session = new GameSession(Levels().Levels[0], 1, "test", new Mulberry32(1));
            var drag = new DragModel(new SessionDragTarget(session), 60);
            drag.Press(3, 0, 0);
            Assert.That(TutorialScript.WrongDirection(expected, drag.Direction, dx, dy, out _), Is.True);
            drag.Cancel(); // same early rejection path as GameRoot: no Move/Release/Abort
            Assert.That((session.Moves, session.Jams, session.Board.PresentCount), Is.EqualTo((0, 0, 9)));
        }

        [Test]
        public void DirectionGuardPreservesAxisAndAcceptsTheRecordedDirection()
        {
            Assert.That(TutorialScript.WrongDirection(Dir.L, null, -40, 0, out _), Is.False);
            Assert.That(TutorialScript.WrongDirection(Dir.D, null, 0, 40, out _), Is.False);
            Assert.That(TutorialScript.WrongDirection(Dir.D, null, 1, -1, out _), Is.False, "below swipe threshold");
            Assert.That(TutorialScript.WrongDirection(Dir.L, Dir.L, -10, 90, out _), Is.False, "axis already locked");
            Assert.That(TutorialScript.WrongDirection(Dir.L, Dir.L, 10, 90, out _), Is.True, "direction reversed");
        }
    }
}
