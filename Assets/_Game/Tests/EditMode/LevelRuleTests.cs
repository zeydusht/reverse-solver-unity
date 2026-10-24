using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace ReverseSolver.Core.Tests
{
    /* M1 acceptance: the 40 web levels load, their recorded solutions play out
       under the ported rule, and the port agrees with the web code. */
    public class LevelRuleTests
    {
        static IEnumerable<string> Ids => TestData.LevelIds();

        [Test]
        public void LevelFileLoads()
        {
            var set = TestData.Levels;
            Assert.That(set.Format, Is.EqualTo(1));
            Assert.That(set.Levels.Count, Is.EqualTo(40));
            Assert.That(set.Levels.Select(l => l.Id),
                Is.EqualTo(Enumerable.Range(1, 40).Select(i => $"L{i:00}")), "ids follow the web order on first import");
            Assert.That(set.Levels.All(l => l.Version == 1));
            Assert.That(set.Levels.Select(l => l.Number), Is.EqualTo(Enumerable.Range(1, 40)));
        }

        // ---- acceptance 1: recorded solution is valid move by move ------------------

        [TestCaseSource(nameof(Ids))]
        public void RecordedSolutionEmptiesTheBoard(string id)
        {
            var level = TestData.Levels[id];
            var b = new Board(level);
            Assert.That(level.Solution.Count, Is.EqualTo(level.Pieces.Count), "one move per piece");
            for (int i = 0; i < level.Solution.Count; i++)
            {
                var m = level.Solution[i];
                Assert.That(b.IsPresent(m.Piece), $"step {i}: piece {m.Piece} already removed");
                var lim = TravelRule.Limit(b, m.Piece, m.Dir);
                Assert.That(lim.Exit, $"step {i}: {m} cannot exit, {lim}");
                b.Remove(m.Piece);
            }
            Assert.That(b.PresentCount, Is.Zero);
        }

        // ---- acceptance 2: travel rule matches the web code ---------------------

        [TestCaseSource(nameof(Ids))]
        public void TravelLimitMatchesWebGolden(string id)
        {
            var level = TestData.Levels[id];
            var golden = TestData.GoldenLevel(id);
            int probes = 0;
            foreach (var state in golden.Get("states").Items)
            {
                int step = state.Get("step").Int();
                var b = new Board(level);
                for (int i = 0; i < step; i++) b.Remove(level.Solution[i].Piece);

                foreach (var row in state.Get("limits").Items)
                {
                    int piece = row[0].Int();
                    var dir = DirExt.All[row[1].Int()];
                    var expected = new TravelLimit(row[2].Int(), row[3].Int() == 1, row[4].Int(), row[5].Int() == 1);
                    var actual = TravelRule.Limit(b, piece, dir);
                    Assert.That(actual, Is.EqualTo(expected), $"step {step}, piece {piece} {dir}");
                    probes++;
                }
            }
            Assert.That(probes, Is.GreaterThan(0));
        }

        [TestCaseSource(nameof(Ids))]
        public void FreePiecesMatchWebAtStart(string id)
        {
            var expected = TestData.GoldenLevel(id).Get("free0").Items.Select(n => n.Int());
            Assert.That(TravelRule.FreePieces(new Board(TestData.Levels[id])), Is.EqualTo(expected));
        }

        // ---- acceptance 4: greedy solver equals the web solver --------------------

        [TestCaseSource(nameof(Ids))]
        public void GreedySolverMatchesWeb(string id)
        {
            var golden = TestData.GoldenLevel(id);
            var r = GreedySolver.Solve(TestData.Levels[id]);
            Assert.That(r.Solved, Is.EqualTo(golden.Get("solvable").AsBool));
            Assert.That(r.Order, Is.EqualTo(golden.Get("greedyOrder").Items.Select(n => n.Int())), "same removal order");
        }

        // Small synthetic boards cover states absent from the complete web levels.
        static Board GreedyBoard(bool trapRightPiece = false)
        {
            string walls = trapRightPiece ? ",\"sealed\":{\"U\":[1],\"D\":[1],\"L\":[0],\"R\":[0]}" : "";
            return new Board(LevelParser.Parse(
                "{\"format\":1,\"levels\":[{\"id\":\"GREEDY\",\"version\":1,\"w\":2,\"h\":1," +
                "\"pieces\":[[[0,0]],[[1,0]]],\"vEdge\":[[null,0,null]]," +
                "\"hEdge\":[[null,null],[null,null]]" + walls + "}]}").Levels[0]);
        }

        [Test]
        public void GreedySolverPreservesAPartiallyClearedBoard()
        {
            var board = GreedyBoard();
            board.Remove(0);

            var result = GreedySolver.Solve(board);

            Assert.That(result.Solved, Is.True);
            Assert.That(result.Order, Is.EqualTo(new[] { 1 }), "already removed pieces stay removed");
            Assert.That(result.Stuck, Is.Empty);
            Assert.That(board.PresentCount, Is.EqualTo(1));
            Assert.That(board.PresentPieces(), Is.EqualTo(new[] { 1 }));
            Assert.That(board.PieceAt(new Cell(0, 0)), Is.EqualTo(-1));
            Assert.That(board.PieceAt(new Cell(1, 0)), Is.EqualTo(1), "solving must not clear the input's cells");
            Assert.That(GreedySolver.Solve(board).Order, Is.EqualTo(result.Order), "the same input can be solved again");
        }

        [Test]
        public void GreedySolverAcceptsAnAlreadyEmptyBoard()
        {
            var board = GreedyBoard();
            board.Remove(0);
            board.Remove(1);

            var result = GreedySolver.Solve(board);

            Assert.That(result.Solved, Is.True);
            Assert.That(result.Order, Is.Empty);
            Assert.That(result.Stuck, Is.Empty);
            Assert.That(board.PresentCount, Is.Zero);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void GreedySolverReportsOnlyTheRemainingTrappedPiece(bool removeFreePieceFirst)
        {
            var board = GreedyBoard(trapRightPiece: true);
            if (removeFreePieceFirst) board.Remove(0);

            var result = GreedySolver.Solve(board);

            Assert.That(result.Solved, Is.False);
            Assert.That(result.Order, Is.EqualTo(removeFreePieceFirst ? new int[0] : new[] { 0 }));
            Assert.That(result.Stuck, Is.EqualTo(new[] { 1 }));
            Assert.That(board.PresentCount, Is.EqualTo(removeFreePieceFirst ? 1 : 2));
            Assert.That(board.PieceAt(new Cell(0, 0)), Is.EqualTo(removeFreePieceFirst ? -1 : 0));
            Assert.That(board.PieceAt(new Cell(1, 0)), Is.EqualTo(1));

            var replay = board.Clone();
            foreach (int piece in result.Order)
            {
                Assert.That(TravelRule.CanExit(replay, piece), Is.True, "every reported removal is legal");
                replay.Remove(piece);
            }
            Assert.That(replay.PresentPieces(), Is.EqualTo(result.Stuck));
            Assert.That(TravelRule.FreePieces(replay), Is.Empty, "the reported remainder really has no exit");
        }

        [Test]
        public void GreedySolverRevisitsALowerIndexAfterUnblockingIt()
        {
            // Piece 0 is between 1 and 2, with its vertical exits sealed.
            // Removing 1 opens 0's left exit: 0 must be reconsidered before 2.
            var level = LevelParser.Parse(
                "{\"format\":1,\"levels\":[{\"id\":\"REVISIT\",\"version\":1,\"w\":3,\"h\":1," +
                "\"pieces\":[[[1,0]],[[0,0]],[[2,0]]],\"vEdge\":[[null,0,0,null]]," +
                "\"hEdge\":[[null,null,null],[null,null,null]],\"sealed\":{\"U\":[1],\"D\":[1]}}]}").Levels[0];
            var board = new Board(level);
            Assert.That(TravelRule.FreePieces(board), Is.EqualTo(new[] { 1, 2 }));

            var result = GreedySolver.Solve(board);

            Assert.That(result.Solved, Is.True);
            Assert.That(result.Order, Is.EqualTo(new[] { 1, 0, 2 }), "restart at the lowest remaining index after each removal");
            Assert.That(result.Stuck, Is.Empty);
            foreach (int piece in result.Order)
            {
                Assert.That(TravelRule.CanExit(board, piece), Is.True, "each removal must be legal when replayed");
                board.Remove(piece);
            }
            Assert.That(board.PresentCount, Is.Zero);
        }

        [TestCase(0)]
        [TestCase(1)]
        public void GreedySolverWaitsForTheWholeChainedPieceCorridor(int sealedLane)
        {
            // Piece 0 spans both rows. One sealed left lane blocks the whole body;
            // both right-hand pieces must leave before it can exit to the right.
            var level = LevelParser.Parse(
                "{\"format\":1,\"levels\":[{\"id\":\"CHAIN\",\"version\":1,\"w\":2,\"h\":2," +
                "\"pieces\":[[[0,0],[0,1]],[[1,0]],[[1,1]]]," +
                "\"vEdge\":[[null,0,null],[null,0,null]]," +
                "\"hEdge\":[[null,null],[0,0],[null,null]]," +
                "\"sealed\":{\"U\":[0],\"D\":[0],\"L\":[" + sealedLane + "]}}]}").Levels[0];
            var board = new Board(level);
            Assert.That(TravelRule.CanExit(board, 0), Is.False);

            var result = GreedySolver.Solve(board);

            Assert.That(result.Solved, Is.True);
            Assert.That(result.Order, Is.EqualTo(new[] { 1, 2, 0 }));
            Assert.That(result.Stuck, Is.Empty);
            foreach (int piece in result.Order)
            {
                Assert.That(TravelRule.CanExit(board, piece), Is.True);
                board.Remove(piece);
                if (piece == 1)
                    Assert.That(TravelRule.CanExit(board, 0), Is.False, "one clear row is insufficient for a chained piece");
            }
            Assert.That(board.PresentCount, Is.Zero);
            Assert.That(board.PieceAt(new Cell(0, 0)), Is.EqualTo(-1));
            Assert.That(board.PieceAt(new Cell(0, 1)), Is.EqualTo(-1), "both cells leave together");
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        public void GreedySolverRespectsSidewaysCatches(int joint)
        {
            // Horizontal exits are sealed. A tab on either facing edge catches
            // during vertical travel; flat edges let both pieces slide out.
            var level = LevelParser.Parse(
                "{\"format\":1,\"levels\":[{\"id\":\"CATCH\",\"version\":1,\"w\":2,\"h\":1," +
                "\"pieces\":[[[0,0]],[[1,0]]],\"vEdge\":[[null," + joint + ",null]]," +
                "\"hEdge\":[[null,null],[null,null]],\"sealed\":{\"L\":[0],\"R\":[0]}}]}").Levels[0];
            var board = new Board(level);
            var result = GreedySolver.Solve(board);

            Assert.That(result.Solved, Is.EqualTo(joint == 0));
            Assert.That(result.Order, Is.EqualTo(joint == 0 ? new[] { 0, 1 } : new int[0]));
            Assert.That(result.Stuck, Is.EqualTo(joint == 0 ? new int[0] : new[] { 0, 1 }));
            foreach (int piece in result.Order)
            {
                Assert.That(TravelRule.CanExit(board, piece), Is.True);
                board.Remove(piece);
            }
            Assert.That(board.PresentPieces(), Is.EqualTo(result.Stuck));
            Assert.That(TravelRule.FreePieces(board), Is.Empty, "no reported stuck piece may have an exit");
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        public void GreedySolverRespectsCatchesDuringHorizontalTravel(int joint)
        {
            // Transposed board: vertical exits are sealed, so the horizontal
            // travel branch must check both upper and lower facing edges.
            var level = LevelParser.Parse(
                "{\"format\":1,\"levels\":[{\"id\":\"HCATCH\",\"version\":1,\"w\":1,\"h\":2," +
                "\"pieces\":[[[0,0]],[[0,1]]],\"vEdge\":[[null,null],[null,null]]," +
                "\"hEdge\":[[null],[" + joint + "],[null]],\"sealed\":{\"U\":[0],\"D\":[0]}}]}").Levels[0];
            var board = new Board(level);
            foreach (int piece in board.PresentPieces())
            {
                foreach (var dir in new[] { Dir.L, Dir.R })
                {
                    var limit = TravelRule.Limit(board, piece, dir);
                    Assert.That(limit.Exit, Is.EqualTo(joint == 0));
                    Assert.That(limit.Hard, Is.False, "a tab catches rather than hitting a solid obstacle");
                    Assert.That(limit.Blocker, Is.EqualTo(joint == 0 ? -1 : 1 - piece));
                }
            }

            var result = GreedySolver.Solve(board);

            Assert.That(result.Solved, Is.EqualTo(joint == 0));
            Assert.That(result.Order, Is.EqualTo(joint == 0 ? new[] { 0, 1 } : new int[0]));
            Assert.That(result.Stuck, Is.EqualTo(joint == 0 ? new int[0] : new[] { 0, 1 }));
            foreach (int piece in result.Order)
            {
                Assert.That(TravelRule.CanExit(board, piece), Is.True);
                board.Remove(piece);
            }
            Assert.That(board.PresentPieces(), Is.EqualTo(result.Stuck));
            Assert.That(TravelRule.FreePieces(board), Is.Empty);
        }

        // Greedy checks geometry only; session counters must not affect its answer.
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        public void GreedySolverIgnoresNailAndBombCounters(bool nail, bool bomb)
        {
            string counters = (nail ? ",\"nails\":{\"0\":9}" : "") +
                              (bomb ? ",\"bombs\":{\"1\":1}" : "");
            var level = LevelParser.Parse(
                "{\"format\":1,\"levels\":[{\"id\":\"COUNTERS\",\"version\":1,\"w\":2,\"h\":1," +
                "\"pieces\":[[[0,0]],[[1,0]]],\"vEdge\":[[null,0,null]]," +
                "\"hEdge\":[[null,null],[null,null]]" + counters + "}]}").Levels[0];

            Assert.That(level.Nails.Count, Is.EqualTo(nail ? 1 : 0), "fixture retains the nail");
            Assert.That(level.Bombs.Count, Is.EqualTo(bomb ? 1 : 0), "fixture retains the bomb");
            var result = GreedySolver.Solve(level);

            Assert.That(result.Solved, Is.True, "session counters do not block geometric exits");
            Assert.That(result.Order, Is.EqualTo(new[] { 0, 1 }), "lowest index first even with a live nail or urgent bomb");
            Assert.That(result.Stuck, Is.Empty);
            if (nail) Assert.That(level.Nails[0], Is.EqualTo(9), "solving must not tick nails");
            if (bomb) Assert.That(level.Bombs[1], Is.EqualTo(1), "solving must not tick fuses");
        }

        /* Not an assertion on solvability: a level greedy cannot clear is data
           for the report, not a failure. Equality with the web is checked above. */
        [Test]
        public void GreedySolverReport()
        {
            var sb = new StringBuilder("Greedy solver over 40 levels:\n");
            int solved = 0;
            foreach (var l in TestData.Levels.Levels)
            {
                var r = GreedySolver.Solve(l);
                if (r.Solved) solved++;
                sb.AppendLine($"  {l.Id} {(r.Solved ? "solved" : "STUCK " + string.Join(",", r.Stuck))} " +
                              $"free at start {TravelRule.FreePieces(new Board(l)).Count}/{l.Pieces.Count}");
            }
            sb.AppendLine($"  solved {solved}/40");
            TestContext.WriteLine(sb.ToString());
            Assert.Pass($"greedy solved {solved}/40");
        }
    }
}
