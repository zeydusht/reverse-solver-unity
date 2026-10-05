using System;

namespace ReverseSolver.Core
{
    /* Order matches the web game's `Object.keys(D)` (U, D, L, R), which the
       greedy solver iterates; keeping it makes the port's choices identical. */
    public enum Dir { U, D, L, R }

    public static class DirExt
    {
        public static readonly Dir[] All = { Dir.U, Dir.D, Dir.L, Dir.R };

        public static int Dx(this Dir d) => d == Dir.L ? -1 : d == Dir.R ? 1 : 0;
        public static int Dy(this Dir d) => d == Dir.U ? -1 : d == Dir.D ? 1 : 0;
        public static bool IsVertical(this Dir d) => d == Dir.U || d == Dir.D;

        public static Dir Opposite(this Dir d) => d switch
        {
            Dir.U => Dir.D,
            Dir.D => Dir.U,
            Dir.L => Dir.R,
            _ => Dir.L
        };

        public static bool TryParse(string s, out Dir dir)
        {
            switch (s)
            {
                case "U": dir = Dir.U; return true;
                case "D": dir = Dir.D; return true;
                case "L": dir = Dir.L; return true;
                case "R": dir = Dir.R; return true;
                default: dir = Dir.U; return false;
            }
        }
    }

    /* Board coordinates: x to the right, y downward, (0,0) top-left. */
    public readonly struct Cell : IEquatable<Cell>
    {
        public readonly int X, Y;
        public Cell(int x, int y) { X = x; Y = y; }

        public Cell Step(Dir d, int k = 1) => new Cell(X + d.Dx() * k, Y + d.Dy() * k);

        public bool Equals(Cell o) => X == o.X && Y == o.Y;
        public override bool Equals(object obj) => obj is Cell c && Equals(c);
        public override int GetHashCode() => (X * 397) ^ Y;
        public override string ToString() => $"({X},{Y})";
    }

    public readonly struct Move
    {
        public readonly int Piece;
        public readonly Dir Dir;
        public Move(int piece, Dir dir) { Piece = piece; Dir = dir; }
        public override string ToString() => $"{Piece}{Dir}";
    }
}
