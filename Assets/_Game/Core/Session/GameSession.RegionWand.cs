using System;
using System.Collections.Generic;

namespace ReverseSolver.Core
{
    public readonly struct WandRegion
    {
        public readonly int X, Y, Width, Height;
        public WandRegion(int x, int y, int width, int height) { X = x; Y = y; Width = width; Height = height; }
        public bool Contains(Cell c) => c.X >= X && c.Y >= Y && c.X < X + Width && c.Y < Y + Height;
        /* PM_67 (option b): index 4 is the whole board, offered only when no quadrant can free a piece. */
        public const int WholeBoard = 4;
        public static WandRegion Quadrant(LevelData level, int index)
        {
            if (index < 0 || index > WholeBoard) throw new ArgumentOutOfRangeException(nameof(index));
            if (index == WholeBoard) return new WandRegion(0, 0, level.Width, level.Height);
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
        /* PM_66: the pieces the last region wand freed (could not leave before, can now). */
        public IReadOnlyList<int> WandFreed { get; private set; } = new List<int>();
        /* PM_66: a wand use must free a piece. Off for the taught free use (the lesson runs on a full board) and
           for tests of the joint mechanics. */
        public bool WandMustFree { get; set; } = true;
        bool RequireFree => WandMustFree && FreeUse != Booster.Wand;

        HashSet<int> FreeNow(Board board)
        {
            var set = new HashSet<int>();
            foreach (int p in TravelRule.FreePieces(board)) if (NailAt(p) <= 0) set.Add(p);
            return set;
        }

        public bool CanWandRegion(int quadrant) => !IsOver && RegionArrangement(quadrant, new Mulberry32(21)) != null;

        public CommandResult UseWandRegion(int quadrant)
        {
            var check = CanUse(Booster.Wand);
            if (check != CommandResult.Ok) return check;
            var arrangement = RegionArrangement(quadrant, _random) ?? RegionArrangement(quadrant, new Mulberry32(21));
            if (arrangement == null) return CommandResult.NoArrangement;
            var before = FreeNow(Board);
            foreach (var j in arrangement)
                if (j.vertical) Board.SetVJoint(j.x, j.y, j.value); else Board.SetHJoint(j.x, j.y, j.value);
            var freed = new List<int>();
            foreach (int p in FreeNow(Board)) if (!before.Contains(p)) freed.Add(p);
            WandFreed = freed;
            Spend(Booster.Wand);
            Raise(GameEvent.Simple(EventKind.JointsReshuffled));
            return CommandResult.Ok;
        }

        /* PM_66: the wand re-decides tab ownership inside the quadrant, now also on the edges between a piece and
           an empty cell there (those decide whether a piece can slide past), and only an arrangement that frees at
           least one piece counts: the one freeing the most pieces among the solvable ones. None: the quadrant is
           not selectable and nothing is spent. Edges on the quadrant's border never change. */
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
                if (a == b) return false;
                return (inside.Contains(a) || a < 0) && (inside.Contains(b) || b < 0) && (a >= 0 || b >= 0);
            }
            for (int y = region.Y; y < region.Y + region.Height; y++)
                for (int x = region.X + 1; x < region.X + region.Width; x++)
                    if (Board.VJoint(x, y) != 0 && Pair(x - 1, y, x, y)) joints.Add((x, y, true, Board.VJoint(x, y)));
            for (int y = region.Y + 1; y < region.Y + region.Height; y++)
                for (int x = region.X; x < region.X + region.Width; x++)
                    if (Board.HJoint(x, y) != 0 && Pair(x, y - 1, x, y)) joints.Add((x, y, false, Board.HJoint(x, y)));
            if (joints.Count == 0) return null;
            var before = FreeNow(Board);
            var candidate = Board.Clone();
            // Small regions are enumerated exactly; larger ones sampled. Rank by pieces freed (cheap), then
            // check solvability from the best down, so the solver runs only as often as needed.
            int attempts = joints.Count <= 8 ? 1 << joints.Count : WandAttempts;
            int offset = joints.Count <= 8 ? (int)(random.NextDouble() * attempts) : 0;
            var ranked = new List<(int freed, int order, int[] values)>();
            for (int t = 0; t < attempts; t++)
            {
                bool changed = false;
                var values = new int[joints.Count];
                for (int i = 0; i < joints.Count; i++)
                {
                    var j = joints[i];
                    int value = joints.Count <= 8 ? (((t + offset) & (1 << i)) == 0 ? 1 : -1) : (random.NextDouble() < .5 ? 1 : -1);
                    values[i] = value;
                    changed |= value != j.value;
                    if (j.vertical) candidate.SetVJoint(j.x, j.y, value); else candidate.SetHJoint(j.x, j.y, value);
                }
                if (!changed) continue;
                int freed = 0;
                foreach (int p in FreeNow(candidate)) if (!before.Contains(p)) freed++;
                if (freed > 0 || !RequireFree) ranked.Add((freed, t, values));
            }
            ranked.Sort((a, b) => a.freed != b.freed ? b.freed.CompareTo(a.freed) : a.order.CompareTo(b.order));
            foreach (var option in ranked)
            {
                for (int i = 0; i < joints.Count; i++)
                {
                    var j = joints[i];
                    if (j.vertical) candidate.SetVJoint(j.x, j.y, option.values[i]); else candidate.SetHJoint(j.x, j.y, option.values[i]);
                }
                if (!GreedySolver.Solve(candidate).Solved) continue;
                var result = new List<(int, int, bool, int)>();
                for (int i = 0; i < joints.Count; i++) result.Add((joints[i].x, joints[i].y, joints[i].vertical, option.values[i]));
                return result;
            }
            return null;
        }
    }
}
