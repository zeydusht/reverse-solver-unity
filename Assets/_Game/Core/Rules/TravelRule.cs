using System;
using System.Collections.Generic;

namespace ReverseSolver.Core
{
    /* How far a piece can slide in a direction, and why it stops.
         Exit     the piece leaves the board; Cells is the travel inside it
         Blocker  the piece in the way, -1 for a wall segment
         Hard     a solid stop (occupied cell or wall) rather than a catch */
    public readonly struct TravelLimit : IEquatable<TravelLimit>
    {
        public readonly int Cells;
        public readonly bool Exit;
        public readonly int Blocker;
        public readonly bool Hard;

        public TravelLimit(int cells, bool exit, int blocker, bool hard)
        {
            Cells = cells; Exit = exit; Blocker = blocker; Hard = hard;
        }

        public bool Equals(TravelLimit o) => Cells == o.Cells && Exit == o.Exit && Blocker == o.Blocker && Hard == o.Hard;
        public override bool Equals(object obj) => obj is TravelLimit t && Equals(t);
        public override int GetHashCode() => (Cells * 31 + Blocker) * 4 + (Exit ? 2 : 0) + (Hard ? 1 : 0);
        public override string ToString() =>
            Exit ? $"exit after {Cells}" : $"stop at {Cells} ({(Blocker < 0 ? "wall" : "piece " + Blocker)}, {(Hard ? "hard" : "catch")})";
    }

    /* Port of the web game's edgeOf / catches / travelLimit (index.html).
       Keep it line-for-line equivalent: golden tests generated from the web
       code check every branch.

       A piece is blocked when, at any position along the travel axis (home
       included):
         - a cell it moves into is occupied, or
         - a cell beside the travel axis is occupied and the two facing edges
           catch. Only hollow/flat against hollow/flat slides past; a tab
           always catches.
       A chained pair is rigid: both cells need a clear corridor at once. */
    public static class TravelRule
    {
        const int Tab = 1, Socket = -1, Flat = 0;

        /* Edge profile of a cell side from the board's joints. The outer rim is flat. */
        public static int EdgeOf(Board b, int x, int y, Dir side)
        {
            var l = b.Level;
            int v;
            switch (side)
            {
                case Dir.L: if (x == 0) return Flat; v = b.VJoint(x, y); break;
                case Dir.R: if (x == l.Width - 1) return Flat; v = b.VJoint(x + 1, y); break;
                case Dir.U: if (y == 0) return Flat; v = b.HJoint(x, y); break;
                default: if (y == l.Height - 1) return Flat; v = b.HJoint(x, y + 1); break;
            }
            if (v == 0) return Flat;
            if (side == Dir.L || side == Dir.U) return v == -1 ? Tab : Socket;
            return v == 1 ? Tab : Socket;
        }

        /* Two facing edges catch unless both are flat or hollow. Note the moving
           side is read at the piece's HOME cell, as in the web game. */
        static bool Catches(Board b, Cell home, Dir side, Cell n) =>
            !(EdgeOf(b, home.X, home.Y, side) <= 0 && EdgeOf(b, n.X, n.Y, side.Opposite()) <= 0);

        /* A sealed side only walls the listed lanes. A body is stopped if a wall
           segment sits in front of ANY of its cells. */
        static bool LaneBlocked(LevelData l, Cell[] cells, Dir dir)
        {
            if (!l.Sealed.TryGetValue(dir, out var lanes) || lanes.Length == 0) return false;
            foreach (var c in cells)
            {
                int lane = dir.IsVertical() ? c.X : c.Y;
                if (Array.IndexOf(lanes, lane) >= 0) return true;
            }
            return false;
        }

        public static TravelLimit Limit(Board b, int piece, Dir dir)
        {
            var l = b.Level;
            var cells = l.Pieces[piece];
            Dir perpA = dir.IsVertical() ? Dir.L : Dir.U;
            Dir perpB = dir.IsVertical() ? Dir.R : Dir.D;
            var at = new Cell[cells.Length];
            int k = 0, lastFull = 0;

            while (true)
            {
                bool allOff = true, allOn = true;
                for (int i = 0; i < cells.Length; i++)
                {
                    at[i] = cells[i].Step(dir, k);
                    bool on = l.OnBoard(at[i]);
                    allOff &= !on;
                    allOn &= on;
                }

                if (allOff)
                {
                    // Wall segment in the way: the body slides up against it and stops.
                    if (LaneBlocked(l, cells, dir)) return new TravelLimit(lastFull, false, -1, true);
                    return new TravelLimit(k - 1, true, -1, false);
                }
                if (allOn) lastFull = k;

                for (int i = 0; i < at.Length; i++)            // cells it moves into
                {
                    if (k == 0 || !l.OnBoard(at[i])) continue;
                    int o = b.PieceAt(at[i]);
                    if (o >= 0 && o != piece) return new TravelLimit(Math.Max(0, k - 1), false, o, true);
                }

                for (int i = 0; i < at.Length; i++)            // nothing may shear past
                {
                    if (!l.OnBoard(at[i])) continue;
                    if (Shears(b, piece, cells[i], at[i], perpA, at, out int blocker) ||
                        Shears(b, piece, cells[i], at[i], perpB, at, out blocker))
                        return new TravelLimit(Math.Max(0, k - 1), false, blocker, false);
                }
                k++;
            }
        }

        static bool Shears(Board b, int piece, Cell home, Cell p, Dir side, Cell[] here, out int blocker)
        {
            blocker = -1;
            var n = p.Step(side);
            if (Array.IndexOf(here, n) >= 0) return false;    // its own other half
            int o = b.PieceAt(n);
            if (o < 0 || o == piece || !Catches(b, home, side, n)) return false;
            blocker = o;
            return true;
        }

        public static bool CanExit(Board b, int piece)
        {
            foreach (var d in DirExt.All)
                if (Limit(b, piece, d).Exit) return true;
            return false;
        }

        public static List<Dir> ExitDirs(Board b, int piece)
        {
            var dirs = new List<Dir>(4);
            foreach (var d in DirExt.All)
                if (Limit(b, piece, d).Exit) dirs.Add(d);
            return dirs;
        }

        /* Pieces with at least one exit right now, ascending. */
        public static List<int> FreePieces(Board b)
        {
            var free = new List<int>();
            foreach (var p in b.PresentPieces())
                if (CanExit(b, p)) free.Add(p);
            return free;
        }
    }
}
