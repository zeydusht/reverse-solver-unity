using System;

namespace ReverseSolver.Core
{
    /* PM_75: how far a piece that cannot leave really slides before it touches what stops it, in cells. The rule
       stays TravelRule.Limit (the web port); this only measures its stop on the drawn shape. Tabs are circles of
       radius .18 centred .08 outside an edge (Piece.shader), so a tab reaches .26 past its edge and the side of a
       tab head starts .32 from the cell's corner along that edge.
         a cell in the way at offset k        k-1 cells, less the tab(s) meeting a flat edge or each other (.26 each)
         a side catch at offset k >= 1        k-1 + .32: a tab beside the lane meets the body (or the body's own
                                              side tab meets the neighbour) a third of the way into that cell
         a side catch at home (k = 0)         0: the piece is interlocked with a neighbour; it only jiggles
         a sealed wall                        the last offset fully on the board, less a tab facing the wall
       A chained pair moves as one: its first contact stops both. */
    public static class Contact
    {
        public const float TabReach = .26f, TabClear = .32f;

        public static float Distance(Board b, int piece, Dir dir) => Distance(b, piece, dir, TravelRule.Limit(b, piece, dir));

        public static float Distance(Board b, int piece, Dir dir, TravelLimit limit)
        {
            if (limit.Exit) return limit.Cells;
            var cells = b.Level.Pieces[piece];
            if (limit.Hard && limit.Blocker < 0)
            {
                foreach (var c in cells)
                    if (!IsOwn(cells, c.Step(dir)) && TravelRule.EdgeOf(b, c.X, c.Y, dir) > 0)
                        return Math.Max(0, limit.Cells - TabReach);
                return limit.Cells;
            }
            if (limit.Hard)
            {
                float overlap = 0;
                foreach (var c in cells)
                {
                    var at = c.Step(dir, limit.Cells + 1);
                    if (b.PieceAt(at) != limit.Blocker) continue;
                    int lead = TravelRule.EdgeOf(b, c.X, c.Y, dir), face = TravelRule.EdgeOf(b, at.X, at.Y, dir.Opposite());
                    float o = (lead > 0 && face >= 0 ? TabReach : 0) + (face > 0 && lead >= 0 ? TabReach : 0);
                    overlap = Math.Max(overlap, o);
                }
                return Math.Max(0, limit.Cells - overlap);
            }
            if (limit.Cells == 0 && CatchesAtHome(b, piece, dir)) return 0;
            return limit.Cells + TabClear;
        }

        /* The k = 0 test of TravelRule.Limit: a present neighbour beside the piece whose facing edges catch. */
        static bool CatchesAtHome(Board b, int piece, Dir dir)
        {
            var cells = b.Level.Pieces[piece];
            Dir perpA = dir.IsVertical() ? Dir.L : Dir.U, perpB = dir.IsVertical() ? Dir.R : Dir.D;
            foreach (var c in cells)
                foreach (var side in new[] { perpA, perpB })
                {
                    var n = c.Step(side);
                    if (IsOwn(cells, n)) continue;
                    int o = b.PieceAt(n);
                    if (o < 0 || o == piece) continue;
                    if (!(TravelRule.EdgeOf(b, c.X, c.Y, side) <= 0 && TravelRule.EdgeOf(b, n.X, n.Y, side.Opposite()) <= 0)) return true;
                }
            return false;
        }

        static bool IsOwn(Cell[] cells, Cell c) => Array.IndexOf(cells, c) >= 0;
    }
}
