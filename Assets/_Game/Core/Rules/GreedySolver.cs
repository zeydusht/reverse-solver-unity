using System.Collections.Generic;

namespace ReverseSolver.Core
{
    public sealed class GreedyResult
    {
        public bool Solved;
        public List<int> Order = new List<int>();     // pieces in removal order
        public List<int> Stuck = new List<int>();     // pieces left when no exit remained
    }

    /* Port of the web game's solvable(): repeatedly remove the lowest-index
       piece that has any exit. Removing a piece never blocks another, so on
       this ruleset greedy decides solvability. Ignores nails and bombs, as the
       web version does; those are session rules (M2). */
    public static class GreedySolver
    {
        public static GreedyResult Solve(LevelData level) => Solve(new Board(level));

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
