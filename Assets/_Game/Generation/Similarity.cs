using System;
using System.Collections.Generic;
using ReverseSolver.Core;

namespace ReverseSolver.Generation
{
    /* How alike two boards look to a player (G1 "tekrar yok"). Boards of different
       sizes never match. Otherwise the distance is the smallest, over the four
       size-preserving symmetries (as is, mirrored left-right, mirrored top-bottom,
       turned half way), of
         0.6 x share of inner seams that differ in open/locked
       + 0.3 x share of cells whose obstacle differs (nail, bomb, chain)
       + 0.1 x share of wall segments that differ.
       Which side owns a tab is ignored: the player reads "locked or open".
       0 = same board; independent random boards sit around 0.4–0.5. */
    public static class Similarity
    {
        public const double DuplicateBelow = 0.15;

        public static double Distance(LevelData a, LevelData b)
        {
            if (a.Width != b.Width || a.Height != b.Height) return 1;
            double best = 1;
            for (int s = 0; s < 4; s++) best = Math.Min(best, Distance(a, b, s % 2 == 1, s >= 2));
            return best;
        }

        public static bool IsDuplicate(LevelData a, LevelData b) => Distance(a, b) < DuplicateBelow;

        /* b is read through the mirror(s); mirroring both ways is the half turn. */
        static double Distance(LevelData a, LevelData b, bool flipX, bool flipY)
        {
            int w = a.Width, h = a.Height;
            int Mx(int x) => flipX ? w - 1 - x : x;
            int My(int y) => flipY ? h - 1 - y : y;

            int seams = 0, seamDiff = 0;
            for (int y = 0; y < h; y++)
                for (int x = 1; x < w; x++)
                {
                    // the joint left of (x, y) maps to the joint left of the mirrored cell pair
                    int bx = flipX ? w - x : x;
                    seams++;
                    if ((a.VJoint(x, y) != 0) != (b.VJoint(bx, My(y)) != 0)) seamDiff++;
                }
            for (int y = 1; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    int by = flipY ? h - y : y;
                    seams++;
                    if ((a.HJoint(x, y) != 0) != (b.HJoint(Mx(x), by) != 0)) seamDiff++;
                }

            var oa = Obstacles(a);
            var ob = Obstacles(b);
            int cellDiff = 0;
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    if (oa[y * w + x] != ob[My(y) * w + Mx(x)]) cellDiff++;

            var wa = Walls(a, false, false);
            var wb = Walls(b, flipX, flipY);
            int lanes = 2 * (w + h), wallDiff = 0;
            foreach (var k in wa) if (!wb.Contains(k)) wallDiff++;
            foreach (var k in wb) if (!wa.Contains(k)) wallDiff++;

            return .6 * seamDiff / Math.Max(1, seams) + .3 * cellDiff / (double)(w * h) + .1 * Math.Min(1.0, wallDiff / (double)lanes);
        }

        /* Per cell: 0 nothing, 1 nail, 2 bomb, 3 chain. */
        static int[] Obstacles(LevelData l)
        {
            var o = new int[l.Width * l.Height];
            for (int p = 0; p < l.Pieces.Count; p++)
            {
                int kind = l.Bombs.ContainsKey(p) ? 2 : l.Nails.ContainsKey(p) ? 1 : l.Pieces[p].Length > 1 ? 3 : 0;
                foreach (var c in l.Pieces[p]) o[c.Y * l.Width + c.X] = kind;
            }
            return o;
        }

        static HashSet<(Dir, int)> Walls(LevelData l, bool flipX, bool flipY)
        {
            var set = new HashSet<(Dir, int)>();
            foreach (var kv in l.Sealed)
                foreach (int lane in kv.Value)
                {
                    var side = kv.Key;
                    int ln = lane;
                    if (side.IsVertical()) { if (flipX) ln = l.Width - 1 - lane; if (flipY) side = side.Opposite(); }
                    else { if (flipY) ln = l.Height - 1 - lane; if (flipX) side = side.Opposite(); }
                    set.Add((side, ln));
                }
            return set;
        }

        /* Keeps the first of every group of look-alikes; returns how many were dropped. */
        public static int Dedupe(List<Candidate> list)
        {
            var kept = new List<Candidate>();
            int dropped = 0;
            foreach (var c in list)
            {
                bool dup = false;
                foreach (var k in kept) if (IsDuplicate(k.Level, c.Level)) { dup = true; break; }
                if (dup) dropped++; else kept.Add(c);
            }
            list.Clear();
            list.AddRange(kept);
            return dropped;
        }
    }
}
