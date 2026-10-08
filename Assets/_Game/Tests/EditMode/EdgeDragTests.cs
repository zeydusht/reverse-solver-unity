using NUnit.Framework;

namespace ReverseSolver.Core.Tests
{
    /* PRODUCT.md K1: edge pieces were hard to pull out on a phone. Flick,
       cancel, grabbing from the margin and the wider side gap. */
    public class EdgeDragTests
    {
        const float S = 60f;

        static GameSession Session(string id) => new GameSession(TestData.Levels[id], 1, "test", new Mulberry32(1));

        static (int piece, Dir dir, TravelLimit lim) FreeMove(GameSession s)
        {
            foreach (var p in TravelRule.FreePieces(s.Board))
                if (s.NailAt(p) <= 0)
                {
                    var d = TravelRule.ExitDirs(s.Board, p)[0];
                    return (p, d, s.Probe(p, d));
                }
            throw new System.InvalidOperationException("no free piece");
        }

        /* Drags `dist` points in `dir` over `seconds`, in 10 steps, starting at t = 0. */
        static DragModel Pull(GameSession s, int piece, Dir dir, float dist, double seconds)
        {
            var drag = new DragModel(new SessionDragTarget(s), S);
            Assert.That(drag.Press(piece, 0, 0, 0, false));
            for (int k = 1; k <= 10; k++)
                drag.Move(dir.Dx() * dist * k / 10, dir.Dy() * dist * k / 10, seconds * k / 10);
            return drag;
        }

        static float ExitDistance(TravelLimit lim) => (lim.Cells + DragModel.ExitOvershoot) * S;

        [Test]
        public void AFastShortFlickTakesThePieceOut()
        {
            var s = Session("L01");
            var (p, d, lim) = FreeMove(s);
            float dist = ExitDistance(lim) * .25f;                       // well short of the 42% rule
            var drag = Pull(s, p, d, dist, .01);                           // ~ dist / 0.01 s
            Assert.That(dist / .01, Is.GreaterThan(DragModel.FlickSpeed));
            Assert.That(drag.Release(out _, out _), Is.EqualTo(DragRelease.Exited));
            Assert.That(drag.Last.Flick, Is.True);
            Assert.That(s.Board.IsPresent(p), Is.False);
            Assert.That(s.Jams, Is.EqualTo(0), "a flick is not a jam");
        }

        [Test]
        public void ASlowShortPullSpringsBack()
        {
            var s = Session("L01");
            var (p, d, lim) = FreeMove(s);
            var drag = Pull(s, p, d, ExitDistance(lim) * .25f, 1.0);
            Assert.That(drag.Release(out _, out _), Is.EqualTo(DragRelease.SnappedBack));
            Assert.That(s.Board.IsPresent(p), Is.True);
        }

        [Test]
        public void AFastFlickBelowTwentyPercentSpringsBack()
        {
            var s = Session("L01");
            var (p, d, lim) = FreeMove(s);
            var drag = Pull(s, p, d, ExitDistance(lim) * .15f, .01);
            Assert.That(drag.Release(out _, out _), Is.EqualTo(DragRelease.SnappedBack));
        }

        [Test]
        public void AFlickTowardsABlockedSideDoesNothing()
        {
            int tried = 0;
            foreach (var l in TestData.Levels.Levels)
            {
                var s = Session(l.Id);
                foreach (var p in s.Board.PresentPieces())
                    foreach (var d in DirExt.All)
                    {
                        var lim = s.Probe(p, d);
                        if (lim.Exit || s.NailAt(p) > 0) continue;
                        // as far and as fast as the piece can go: still a blocked side
                        var drag = Pull(s, p, d, 5 * S, .02);
                        Assert.That(drag.Release(out _, out _), Is.EqualTo(DragRelease.SnappedBack), $"{l.Id} piece {p} {d}");
                        Assert.That(s.Board.IsPresent(p), Is.True);
                        if (++tried >= 20) return;
                    }
            }
            Assert.That(tried, Is.GreaterThan(0));
        }

        [Test]
        public void TheOldRuleStillWorksWithoutTimestamps()
        {
            var s = Session("L01");
            var (p, d, lim) = FreeMove(s);
            var drag = new DragModel(new SessionDragTarget(s), S);
            drag.Press(p, 0, 0);
            float dist = ExitDistance(lim) * .45f;
            drag.Move(d.Dx() * dist, d.Dy() * dist);
            Assert.That(drag.Release(out _, out _), Is.EqualTo(DragRelease.Exited));
            Assert.That(drag.Last.Flick, Is.False);
        }

        [Test]
        public void ACancelPastTheExitPointLeaves()
        {
            var s = Session("L01");
            var (p, d, lim) = FreeMove(s);
            var drag = Pull(s, p, d, ExitDistance(lim) * .5f, 1.0);
            Assert.That(drag.Abort(out _, out _), Is.EqualTo(DragRelease.Exited));
            Assert.That(drag.Last.Cancelled, Is.True);
            Assert.That(s.Board.IsPresent(p), Is.False);
            Assert.That(s.Jams, Is.EqualTo(0));
        }

        [Test]
        public void ACancelBeforeTheExitPointSpringsBackEvenIfFast()
        {
            var s = Session("L01");
            var (p, d, lim) = FreeMove(s);
            var drag = Pull(s, p, d, ExitDistance(lim) * .3f, .02);
            Assert.That(drag.Abort(out _, out _), Is.EqualTo(DragRelease.SnappedBack), "no flick on a cancel");
            Assert.That(s.Board.IsPresent(p), Is.True);
            Assert.That(s.Jams, Is.EqualTo(0), "a cancel is not a jam");
        }

        [Test]
        public void JamsAreCountedAsBefore()
        {
            var s = Session("L08");
            foreach (var p in s.Board.PresentPieces())
                foreach (var d in DirExt.All)
                {
                    var lim = s.Probe(p, d);
                    if (lim.Exit || s.NailAt(p) > 0) continue;
                    var drag = Pull(s, p, d, 5 * S, .05);                  // fast and far past the stop
                    drag.Release(out _, out _);
                    Assert.That(s.Jams, Is.EqualTo(1), "one jam per drag and direction, flick or not");
                    return;
                }
        }

        [Test]
        public void TheReportSaysHowItEnded()
        {
            var s = Session("L01");
            var (p, d, lim) = FreeMove(s);
            var drag = new DragModel(new SessionDragTarget(s), S);
            drag.Press(p, 0, 0, 0, true);
            drag.Move(d.Dx() * 10, d.Dy() * 10, .5);
            drag.Release(out _, out _);
            var r = drag.Last;
            Assert.That(r.Result, Is.EqualTo(DragRelease.SnappedBack));
            Assert.That(r.PressedOutside, Is.True);
            Assert.That(r.Offset, Is.EqualTo(10).Within(1e-3));
            Assert.That(r.Needed, Is.EqualTo(ExitDistance(lim) * DragModel.ExitFraction).Within(1e-3));
            StringAssert.Contains("SnappedBack", r.ToString());
        }

        // ---- grabbing from the margin -----------------------------------------------

        [Test]
        public void APressJustOutsideTakesTheNearestEdgePiece()
        {
            var s = Session("L01");                                         // 4x5, all single cells
            var b = s.Board;
            Assert.That(EdgeGrab.PieceAt(b, -10, 2.5f * S, S, out bool o1), Is.EqualTo(b.PieceAt(new Cell(0, 2))));
            Assert.That(o1, Is.True);
            Assert.That(EdgeGrab.PieceAt(b, 4 * S + 20, 0.5f * S, S, out _), Is.EqualTo(b.PieceAt(new Cell(3, 0))));
            Assert.That(EdgeGrab.PieceAt(b, 1.5f * S, -23, S, out _), Is.EqualTo(b.PieceAt(new Cell(1, 0))));
            Assert.That(EdgeGrab.PieceAt(b, 2.5f * S, 5 * S + 5, S, out _), Is.EqualTo(b.PieceAt(new Cell(2, 4))));
            Assert.That(EdgeGrab.PieceAt(b, -30, 2.5f * S, S, out _), Is.EqualTo(-1), "beyond the reach");
            Assert.That(EdgeGrab.PieceAt(b, 1.5f * S, 2.5f * S, S, out bool o2), Is.EqualTo(b.PieceAt(new Cell(1, 2))));
            Assert.That(o2, Is.False);
        }

        [Test]
        public void AnEmptyEdgeCellFallsBackToTheNearestOneWithinReach()
        {
            var s = Session("L01");
            var b = s.Board;
            b.Remove(b.PieceAt(new Cell(0, 0)));
            Assert.That(EdgeGrab.PieceAt(b, -5, 5, S, out _), Is.EqualTo(-1), "the next edge cell is farther than 24 pt");
            Assert.That(EdgeGrab.PieceAt(b, -5, S - 5, S, out _), Is.EqualTo(b.PieceAt(new Cell(0, 1))), "near the corner: the cell below");
        }

        // ---- side gap -------------------------------------------------------------------

        [Test]
        public void SideGapMatchesTheTable()
        {
            Assert.That(BoardLayout.CellSize(393, 852 - 59 - 34, 6, 6), Is.EqualTo(54));
            Assert.That(BoardLayout.CellSize(375, 667, 6, 7), Is.EqualTo(51));
            Assert.That(BoardLayout.CellSize(430, 932 - 59 - 34, 6, 6), Is.EqualTo(60));
            Assert.That(BoardLayout.SideGap(393, 6), Is.EqualTo(34));
            Assert.That(BoardLayout.SideGap(320, 7), Is.EqualTo(19), "never below the web's 19");
        }

        [Test]
        public void NoWebLevelDropsBelowFortyFourPoints()
        {
            foreach (var (w, h) in new[] { (375, 667), (390, 844 - 47 - 34), (430, 932 - 59 - 34), (393, 852 - 59 - 34) })
                foreach (var l in TestData.Levels.Levels)
                    Assert.That(BoardLayout.CellSize(w, h, l.Width, l.Height), Is.GreaterThanOrEqualTo(44), $"{l.Id} on {w}x{h}");
        }
    }
}
