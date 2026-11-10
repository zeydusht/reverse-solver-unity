using System;
using System.Collections.Generic;

namespace ReverseSolver.Core
{
    public readonly struct WandRegion
    {
        public readonly int X, Y, Width, Height;
        public WandRegion(int x, int y, int width, int height) { X = x; Y = y; Width = width; Height = height; }
        public bool Contains(Cell c) => c.X >= X && c.Y >= Y && c.X < X + Width && c.Y < Y + Height;
        public static WandRegion Quadrant(LevelData level, int index)
        {
            if (index < 0 || index > 3) throw new ArgumentOutOfRangeException(nameof(index));
            int w = (level.Width + 1) / 2, h = (level.Height + 1) / 2;
            return new WandRegion(index % 2 == 0 ? 0 : w, index < 2 ? 0 : h,
                index % 2 == 0 ? w : level.Width - w, index < 2 ? h : level.Height - h);
        }
    }

    public sealed partial class GameSession
    {
        // Opt in only for p21/new art; legacy clock and whole-board wand stay unchanged.
        public bool ClockFreezesTime { get; set; }
        public double FreezeLeft { get; private set; }
        public bool CanWandRegion(int quadrant) => !IsOver && RegionArrangement(quadrant, new Mulberry32(21)) != null;

        public CommandResult UseWandRegion(int quadrant)
        {
            var check = CanUse(Booster.Wand);
            if (check != CommandResult.Ok) return check;
            var arrangement = RegionArrangement(quadrant, _random) ?? RegionArrangement(quadrant, new Mulberry32(21));
            if (arrangement == null) return CommandResult.NoArrangement;
            foreach (var j in arrangement)
                if (j.vertical) Board.SetVJoint(j.x, j.y, j.value); else Board.SetHJoint(j.x, j.y, j.value);
            Spend(Booster.Wand);
            Raise(GameEvent.Simple(EventKind.JointsReshuffled));
            return CommandResult.Ok;
        }

        List<(int x, int y, bool vertical, int value)> RegionArrangement(int quadrant, IRandom random)
        {
            var region = WandRegion.Quadrant(Level, quadrant);
            var inside = new HashSet<int>();
            foreach (int p in Board.PresentPieces())
            {
                bool full = true;
                foreach (var c in Level.Pieces[p]) if (!region.Contains(c)) { full = false; break; }
                if (full) inside.Add(p);
            }
            var joints = new List<(int x, int y, bool vertical, int value)>();
            bool Pair(int ax, int ay, int bx, int by)
            {
                int a = Board.PieceAt(new Cell(ax, ay)), b = Board.PieceAt(new Cell(bx, by));
                return a != b && inside.Contains(a) && inside.Contains(b);
            }
            for (int y = region.Y; y < region.Y + region.Height; y++)
                for (int x = region.X + 1; x < region.X + region.Width; x++)
                    if (Board.VJoint(x, y) != 0 && Pair(x - 1, y, x, y)) joints.Add((x, y, true, Board.VJoint(x, y)));
            for (int y = region.Y + 1; y < region.Y + region.Height; y++)
                for (int x = region.X; x < region.X + region.Width; x++)
                    if (Board.HJoint(x, y) != 0 && Pair(x, y - 1, x, y)) joints.Add((x, y, false, Board.HJoint(x, y)));
            if (joints.Count == 0) return null;
            var candidate = Board.Clone();
            // Same ownership randomization and geometric solvability guard as UseWand.
            // Enumerate small regions exactly, so impossible/unchanged layouts never spend stock.
            int attempts = joints.Count <= 8 ? 1 << joints.Count : WandAttempts;
            int offset = joints.Count <= 8 ? (int)(random.NextDouble() * attempts) : 0;
            for (int t = 0; t < attempts; t++)
            {
                bool changed = false;
                for (int i = 0; i < joints.Count; i++)
                {
                    var j = joints[i];
                    int value = joints.Count <= 8 ? (((t + offset) & (1 << i)) == 0 ? 1 : -1) : (random.NextDouble() < .5 ? 1 : -1);
                    changed |= value != j.value;
                    if (j.vertical) candidate.SetVJoint(j.x, j.y, value); else candidate.SetHJoint(j.x, j.y, value);
                }
                if (!changed || !GreedySolver.Solve(candidate).Solved) continue;
                var result = new List<(int, int, bool, int)>();
                foreach (var j in joints) result.Add((j.x, j.y, j.vertical, j.vertical ? candidate.VJoint(j.x, j.y) : candidate.HJoint(j.x, j.y)));
                return result;
            }
            return null;
        }
    }
}
