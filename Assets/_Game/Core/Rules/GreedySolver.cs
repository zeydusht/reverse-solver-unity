using System.Collections.Generic;

namespace ReverseSolver.Core
{
    /// <summary>A geometric removal result; all piece indices refer to the input level.</summary>
    public sealed class GreedyResult
    {
        /// <summary>True when no pieces remain, including an initially empty board.</summary>
        public bool Solved;
        /// <summary>
        /// Legal removal order for pieces present at the start. Directions are not recorded;
        /// use TravelRule.ExitDirs on each successive board state when replaying this order.
        /// This may be a partial order when Solved is false.
        /// </summary>
        public List<int> Order = new List<int>();     // pieces in removal order
        /// <summary>Remaining indices in ascending order; none has a geometric exit.</summary>
        public List<int> Stuck = new List<int>();     // pieces left when no exit remained
    }

    /* Port of the web game's solvable(): repeatedly remove the lowest-index
       piece that has any exit. Removing a piece never blocks another, so on
       this ruleset greedy decides solvability. Ignores nails and bombs, as the
       web version does; those are session rules (M2). */
    public static class GreedySolver
    {
        /// <summary>Checks the complete level with its initial joints and all pieces present.</summary>
        /// <remarks>
        /// This checks geometry only. It does not simulate session counters, timing, or
        /// booster use, so a solved result is not a guarantee of a session win.
        /// </remarks>
        public static GreedyResult Solve(LevelData level) => Solve(new Board(level));

        /// <summary>Checks the remaining pieces using the board's current joints.</summary>
        /// <remarks>
        /// Works on a clone: occupancy, joints, and level data are not changed.
        /// Already removed pieces appear in neither Order nor Stuck. After each removal,
        /// scanning restarts at the lowest remaining index. Removing pieces cannot add
        /// geometric blockers, so no backtracking is needed for this removal-only search.
        /// Session counters, timing, and booster use are not simulated.
        /// </remarks>
        public static GreedyResult Solve(Board start)
        {
            var b = start.Clone();
            var r = new GreedyResult();
            while (b.PresentCount > 0)
            {
                int next = -1;
                foreach (var p in b.PresentPieces())
                    if (TravelRule.CanExit(b, p)) { next = p; break; }
                if (next < 0) break;
                b.Remove(next);
                r.Order.Add(next);
            }
            r.Solved = b.PresentCount == 0;
            r.Stuck.AddRange(b.PresentPieces());
            return r;
        }
    }
}
