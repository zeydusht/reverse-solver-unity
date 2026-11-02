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
        public void BoosterTrainingUsesUnlockSlotsAndApplicableSolutionTargets()
        {
            var levels = Levels();
            var buttons = Script().steps.Where(s => s.tur == "booster").ToArray();
            Assert.That(buttons.Select(s => s.slot), Is.EqualTo(new[] { 5, 9, 15, 23 }));
            foreach (var step in buttons)
            {
                Assert.That(TutorialScript.BoosterOf(step.booster, out var b), Is.True);
                var level = levels.Levels[step.slot - 1];
                Assert.That(level.Unlock[b.Id()], Is.EqualTo(step.slot));
                var session = new GameSession(level, 1, "test", new Mulberry32(1));
                bool target = TutorialScript.BoosterTarget(session, b, out int piece, out var joint);
                var targetSteps = Script().ForSlot(step.slot, true).Where(s => s.tur == "booster_hedef").ToArray();
                if (b == Booster.Wand || b == Booster.Clock) { Assert.That(targetSteps, Is.Empty); continue; }
                Assert.That(targetSteps.Length, Is.EqualTo(1));
                if (!target) { Assert.That(piece, Is.EqualTo(-1), "missing targets are explicitly skipped"); continue; }
                session.SetStock(b, 0);
                session.FreeUse = b;
                var result = b == Booster.Scissors ? session.UseScissors(joint.x, joint.y, joint.vertical) : session.UseHammer(piece);
                Assert.That(result, Is.EqualTo(CommandResult.Ok));
                Assert.That(session.Used(b), Is.Zero);
                Assert.That(session.Stock(b), Is.Zero);
                if (b == Booster.Scissors) Assert.That(session.IsCuttable(joint.x, joint.y, joint.vertical), Is.False);
                else Assert.That(session.Board.IsPresent(piece), Is.False);
            }
        }

        [Test]
        public void InventoryPersistsConsumptionAndGrantsTrainingStockOnlyOnce()
        {
            var store = new MemoryStore();
            var level = Levels().Levels[22];
            var inventory = new BoosterInventory(store, level, _ => false);
            foreach (var b in BoosterExt.All) Assert.That(inventory.Stock(b), Is.Zero);
            inventory.Complete(Booster.Scissors);
            inventory.Spend(Booster.Scissors);
            inventory.Complete(Booster.Scissors);
            var restored = new BoosterInventory(store, level, _ => true);
            Assert.That(restored.Trained(Booster.Scissors), Is.True);
            Assert.That(restored.Stock(Booster.Scissors), Is.EqualTo(BoosterInventory.BoosterStart - 1));
            Assert.That(restored.Stock(Booster.Hammer), Is.Zero, "existing save is never refilled by progress");
            var migrated = new BoosterInventory(new MemoryStore(), level, i => i < 14);
            Assert.That(migrated.Stock(Booster.Scissors), Is.EqualTo(BoosterInventory.BoosterStart));
            Assert.That(migrated.Stock(Booster.Clock), Is.Zero);
        }

        [Test]
        public void MissingBoosterTargetIsExplicit()
        {
            var session = new GameSession(Levels().Levels[0], 1, "test", new Mulberry32(1));
            foreach (var m in session.Level.Solution) session.Board.Remove(m.Piece);
            Assert.That(TutorialScript.BoosterTarget(session, Booster.Scissors, out _, out _), Is.False);
            Assert.That(TutorialScript.BoosterTarget(session, Booster.Hammer, out _, out _), Is.False);
        }

        [TestCase(Dir.L)]
        [TestCase(Dir.R)]
        [TestCase(Dir.U)]
        [TestCase(Dir.D)]
        public void DragMaskOpensCellsAndOnePathWithoutAdjacentLanes(Dir direction)
        {
            var origin = new UnityEngine.Vector2(30, 100);
            var cells = new[] { new Cell(1, 1), new Cell(2, 1), new Cell(1, 2) };
            var end = direction switch
            {
                Dir.L => new UnityEngine.Vector2(3, 220),
                Dir.R => new UnityEngine.Vector2(384, 220),
                Dir.U => new UnityEngine.Vector2(150, 46),
                _ => new UnityEngine.Vector2(150, 514)
            };
            var holes = TutorialSpotlight.DragHoles(origin, 60, cells, direction, end);
            Assert.That(holes.Count, Is.EqualTo(cells.Length + 1));
            for (int i = 0; i < cells.Length; i++)
            {
                Assert.That(holes[i].Center, Is.EqualTo(origin + new UnityEngine.Vector2((cells[i].X + .5f) * 60, (cells[i].Y + .5f) * 60)));
                Assert.That(holes[i].Size, Is.EqualTo(UnityEngine.Vector2.one * 60));
                Assert.That(holes[i].Radius, Is.EqualTo(10.8f).Within(.001f));
            }
            var path = holes.Last();
            var rect = new UnityEngine.Rect(path.Center - path.Size / 2, path.Size);
            if (direction.IsVertical())
            {
                Assert.That(rect.xMin, Is.EqualTo(90));
                Assert.That(rect.xMax, Is.EqualTo(210));
                Assert.That(direction == Dir.U ? rect.yMin : rect.yMax, Is.EqualTo(end.y));
                Assert.That(direction == Dir.U ? rect.yMax : rect.yMin, Is.EqualTo(direction == Dir.U ? 160 : 280));
            }
            else
            {
                Assert.That(rect.yMin, Is.EqualTo(160));
                Assert.That(rect.yMax, Is.EqualTo(280));
                Assert.That(direction == Dir.L ? rect.xMin : rect.xMax, Is.EqualTo(end.x));
                Assert.That(direction == Dir.L ? rect.xMax : rect.xMin, Is.EqualTo(direction == Dir.L ? 90 : 210));
            }
        }

        [Test]
        public void ChainSelectionMinimizesCrossingsAndBreaksTiesByFirstPiece()
        {
            var level = Levels().Levels[9];
            var pieces = new System.Collections.Generic.List<Cell[]>();
            level.Pieces = pieces;
            pieces.Add(new[] { new Cell(0, 0), new Cell(2, 2) });
            pieces.Add(new[] { new Cell(2, 0), new Cell(0, 2) });
            pieces.Add(new[] { new Cell(3, 0), new Cell(3, 2) });
            pieces.Add(new[] { new Cell(4, 0), new Cell(4, 2) });
            Assert.That(TutorialSpotlight.ChainPiece(level), Is.EqualTo(2));
            pieces.RemoveRange(2, 2);
            Assert.That(TutorialSpotlight.ChainPiece(level), Is.Zero);
        }

        [Test]
        public void ChainMaskOpensOnlySelectedCellsAndOneFourteenPointStrip()
        {
            var level = Levels().Levels[9];
            int piece = TutorialSpotlight.ChainPiece(level);
            Assert.That(piece, Is.GreaterThanOrEqualTo(0));
            var cells = level.Pieces[piece];
            var origin = new UnityEngine.Vector2(30, 100);
            var holes = TutorialSpotlight.ChainHoles(origin, 60, level);
            Assert.That(holes.Count, Is.EqualTo(cells.Length + 1));
            for (int i = 0; i < cells.Length; i++)
                Assert.That(holes[i].Center, Is.EqualTo(origin + new UnityEngine.Vector2((cells[i].X + .5f) * 60, (cells[i].Y + .5f) * 60)));
            var strip = holes.Last();
            var delta = holes[1].Center - holes[0].Center;
            Assert.That(strip.Center, Is.EqualTo((holes[0].Center + holes[1].Center) / 2));
            Assert.That(strip.Size.x, Is.EqualTo(delta.magnitude).Within(.001f));
            Assert.That(strip.Size.y, Is.EqualTo(14));
            Assert.That(strip.Angle, Is.EqualTo(UnityEngine.Mathf.Atan2(delta.y, delta.x)).Within(.001f));
            Assert.That(holes.Count, Is.LessThanOrEqualTo(16));
            Assert.That(TutorialSpotlight.ChainHoles(origin, 60, Levels().Levels[0]), Is.Empty);
        }

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
            Assert.That(first.Select(s => s.tur), Is.EqualTo(new[] { "surukle", "hata", "surukle" }));
            Assert.That((first[1].parca, levels.Levels[0].Solution[1].Dir), Is.EqualTo((6, Dir.D)));
            Assert.That(first[2].parca, Is.EqualTo(first[1].parca));
        }

        [Test]
        public void S41ErrorDirectionReallyBlocksAndDoesNotCountAsAJam()
        {
            var level = Levels().Levels.First(l => l.Id == "S41");
            var session = new GameSession(level, 1, "test", new Mulberry32(1));
            var first = level.Solution[0];
            Assert.That(session.TryExit(first.Piece, first.Dir), Is.EqualTo(CommandResult.Ok));
            var step = Script().ForSlot(1, true)[1];
            Assert.That(TutorialScript.ErrorMove(session, step, out var error), Is.True);
            Assert.That(error.Piece, Is.EqualTo(level.Solution[1].Piece));
            Assert.That(error.Dir, Is.EqualTo(Dir.R), "first blocked direction in R, L, U, D order");
            Assert.That(error.Dir, Is.Not.EqualTo(level.Solution[1].Dir));
            Assert.That(session.Probe(error.Piece, error.Dir).Exit, Is.False);

            session.TutorialMove = true;
            var drag = new DragModel(new SessionDragTarget(session), 60);
            Assert.That(drag.Press(error.Piece, 0, 0), Is.True);
            drag.Move(error.Dir.Dx() * 1000, error.Dir.Dy() * 1000);
            Assert.That(session.Events.Last().Kind, Is.EqualTo(EventKind.Jammed));
            Assert.That((session.Moves, session.Jams), Is.EqualTo((1, 0)));
            Assert.That(drag.Release(out _, out _), Is.EqualTo(DragRelease.SnappedBack));
            session.TutorialMove = false;
            Assert.That(session.TryExit(error.Piece, level.Solution[1].Dir), Is.EqualTo(CommandResult.Ok));
            session.RecordJam(level.Solution[2].Piece);
            Assert.That(session.Jams, Is.EqualTo(1), "ordinary jams still count after training");
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
        public void RetryRepeatsDragsAndErrorButDoesNotRepeatObstacleIntro()
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
