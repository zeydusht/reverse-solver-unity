using System.IO;
using System.Linq;
using NUnit.Framework;
using ReverseSolver.Core.Json;

namespace ReverseSolver.Core.Tests
{
    /* M3 criteria 2 and 3: the input model in isolation, then golden sessions
       played through simulated drags instead of commands. */
    public class DragModelTests
    {
        const float S = 60f;   // cell size in points

        static GameSession Session(string id) => new GameSession(TestData.Levels[id], 1, "test", new Mulberry32(1));

        static (int piece, Dir dir) FreeMove(GameSession s)
        {
            foreach (var p in TravelRule.FreePieces(s.Board))
                if (s.NailAt(p) <= 0) return (p, TravelRule.ExitDirs(s.Board, p)[0]);
            throw new System.InvalidOperationException("no free piece");
        }

        static (int piece, Dir dir, TravelLimit lim) BlockedMove(GameSession s, bool hard)
        {
            foreach (var p in s.Board.PresentPieces())
                foreach (var d in DirExt.All)
                {
                    var l = s.Probe(p, d);
                    if (!l.Exit && l.Hard == hard && l.Blocker >= 0 && s.NailAt(p) <= 0) return (p, d, l);
                }
            throw new System.InvalidOperationException("no blocked move");
        }

        static (float x, float y) Along(Dir d, float dist) => (d.Dx() * dist, d.Dy() * dist);

        [Test]
        public void AxisIsChosenAfterMinSwipeAndThenLocked()
        {
            var s = Session("L01");
            var drag = new DragModel(new SessionDragTarget(s), S);
            Assert.That(drag.Press(0, 0, 0));
            drag.Move(4, 3);                                   // 5 pt: below the threshold
            Assert.That(drag.Direction, Is.Null);
            drag.Move(9, -2);                                  // horizontal wins
            Assert.That(drag.Direction, Is.EqualTo(Dir.R));
            drag.Move(-3, -40);                                // mostly vertical now, axis stays horizontal
            Assert.That(drag.Direction, Is.EqualTo(Dir.L));
            Assert.That(drag.Offset, Is.EqualTo(System.Math.Min(3f, drag.Offset)).Within(1e-4));
        }

        [Test]
        public void PieceStopsAtItsTravelLimitWithTheWebGive()
        {
            foreach (bool hard in new[] { true, false })
            {
                var s = Session("L08");
                var (p, d, lim) = BlockedMove(s, hard);
                var drag = new DragModel(new SessionDragTarget(s), S);
                drag.Press(p, 0, 0);
                var (x, y) = Along(d, 10 * S);
                drag.Move(x, y);
                float give = hard ? DragModel.GiveHard : DragModel.GiveSoft;
                Assert.That(drag.Offset, Is.EqualTo((lim.Cells + give) * S).Within(1e-3), hard ? "hard stop" : "catch");
                Assert.That(drag.Blocker, Is.EqualTo(lim.Blocker));
                Assert.That(drag.Release(out _, out _), Is.EqualTo(DragRelease.SnappedBack));
                Assert.That(s.Board.IsPresent(p));
            }
        }

        [Test]
        public void ReleaseBeforeTheExitThresholdSpringsBack()
        {
            var s = Session("L01");
            var (p, d) = FreeMove(s);
            var lim = s.Probe(p, d);
            float threshold = (lim.Cells + DragModel.ExitOvershoot) * S * DragModel.ExitFraction;
            var drag = new DragModel(new SessionDragTarget(s), S);

            drag.Press(p, 0, 0);
            var (x, y) = Along(d, threshold - 1);
            drag.Move(x, y);
            Assert.That(drag.Release(out _, out _), Is.EqualTo(DragRelease.SnappedBack));
            Assert.That(s.Board.IsPresent(p));

            drag.Press(p, 0, 0);
            (x, y) = Along(d, threshold + 0.01f);
            drag.Move(x, y);
            Assert.That(drag.Release(out int got, out var gotDir), Is.EqualTo(DragRelease.Exited));
            Assert.That((got, gotDir), Is.EqualTo((p, d)));
            Assert.That(s.Board.IsPresent(p), Is.False);
            Assert.That(s.Moves, Is.EqualTo(1));
        }

        [Test]
        public void JamCountsOncePerDragAndDirection()
        {
            var s = Session("L08");
            var (p, d, lim) = BlockedMove(s, hard: true);
            var drag = new DragModel(new SessionDragTarget(s), S);
            drag.Press(p, 0, 0);

            var (x, y) = Along(d, (lim.Cells + DragModel.GiveHard) * S + DragModel.JamSlack - 0.5f);
            drag.Move(x, y);
            Assert.That(s.Jams, Is.Zero, "within the slack");
            (x, y) = Along(d, 5 * S);
            drag.Move(x, y);
            drag.Move(x * 1.5f, y * 1.5f);
            Assert.That(s.Jams, Is.EqualTo(1), "once per direction");

            // back through the start into the opposite direction, if that one is blocked too
            var back = d.Opposite();
            if (!s.Probe(p, back).Exit)
            {
                (x, y) = Along(back, 10 * S);
                drag.Move(x, y);
                Assert.That(s.Jams, Is.EqualTo(2), "a new direction counts again");
            }
            int before = s.Jams;
            drag.Release(out _, out _);
            drag.Press(p, 0, 0);
            (x, y) = Along(d, 10 * S);
            drag.Move(x, y);
            Assert.That(s.Jams, Is.EqualTo(before + 1), "a new drag counts again");
        }

        [Test]
        public void PressingANailedPieceIsAJamAndStartsNoDrag()
        {
            var s = Session("L05");
            int nailed = s.Nails.First(n => n.Value > 0).Key;
            var drag = new DragModel(new SessionDragTarget(s), S);
            Assert.That(drag.Press(nailed, 0, 0), Is.False);
            Assert.That(drag.Active, Is.False);
            Assert.That(s.Jams, Is.EqualTo(1));
        }

        [Test]
        public void NoDragAfterTheEnd()
        {
            var s = Session("L01");
            s.Quit();
            var drag = new DragModel(new SessionDragTarget(s), S);
            Assert.That(drag.Press(0, 0, 0), Is.False);
            Assert.That(s.Jams, Is.Zero);
        }

        // ---- criterion 3: golden sessions through the input layer ------------------

        static readonly string[] Replayed = { "L01", "L05", "L11", "L21", "L24", "L31" };

        [TestCaseSource(nameof(Replayed))]
        public void GoldenSessionPlayedByDragging(string id)
        {
            var golden = JsonReader.Parse(File.ReadAllText("Assets/_Game/Tests/EditMode/Golden/session_golden.json"));
            golden.TryGet("levels", out var levels);
            var sc = levels.Items.First(n => n.Get("id").AsString == id);
            var steps = sc.Get("steps");
            var s = Session(id);
            var drag = new DragModel(new SessionDragTarget(s), S);

            foreach (var step in steps.Items)
            {
                var cmd = step.Get("cmd");
                int piece = cmd[1].Int();
                DirExt.TryParse(cmd[2].AsString, out var dir);
                Assert.That(drag.Press(piece, 100, 100), $"{id}: press {piece}");
                // a finger moving in a few steps, slightly off-axis, past the exit point
                float full = (s.Probe(piece, dir).Cells + DragModel.ExitOvershoot) * S;
                for (int k = 1; k <= 4; k++)
                {
                    var (x, y) = Along(dir, full * k / 4f);
                    float wobble = 2f * (k % 2 == 0 ? 1 : -1);
                    drag.Move(100 + x + (dir.IsVertical() ? wobble : 0), 100 + y + (dir.IsVertical() ? 0 : wobble));
                }
                Assert.That(drag.Release(out _, out _), Is.EqualTo(DragRelease.Exited), $"{id}: {piece}{dir}");
            }
            var last = steps.Items.Last().Get("after");
            Assert.That(s.Outcome.Id(), Is.EqualTo(last.Get("res").AsString));
            Assert.That(s.Moves, Is.EqualTo(last.Get("moves").Int()));
            Assert.That(s.Stars, Is.EqualTo(last.Get("stars").Int()));
            Assert.That(s.Jams, Is.Zero, "clean drags count no jams");
        }
    }
}
