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
            // PM_75: the sampling can miss the few tabs that matter on a big region; aim at them directly
            return RequireFree && TargetedSearch ? TargetedArrangement(region, joints, before, random) : null;
        }

        /* PM_75: a cheap geometric test, no solver: can re-deciding the tabs free any piece on this board at all?
           False when the pieces are still packed (a piece blocked by a present neighbour stays blocked whatever
           side owns the tab) or one piece is left: the wand button dims and explains instead. */
        public bool WandCanHelp()
        {
            if (IsOver || Board.PresentCount < 2) return false;
            var region = WandRegion.Quadrant(Level, WandRegion.WholeBoard);
            var inside = new HashSet<int>(Board.PresentPieces());
            var before = FreeNow(Board);
            var work = Board.Clone();
            foreach (int p in inside)
            {
                if (before.Contains(p) || NailAt(p) > 0) continue;
                foreach (var d in DirExt.All)
                    if (FreeingFlips(work, region, inside, p, d) != null) return true;
            }
            return false;
        }

        /* For each piece that cannot leave, the tabs along one of its ways out are turned hollow (only tabs the wand
           may change: inside the region, flat joints stay flat); the first such change that frees a piece and keeps
           the board solvable wins. The other tabs of the region are kept, then re-drawn a few times if the board
           would not be solvable. */
        List<(int x, int y, bool vertical, int value)> TargetedArrangement(WandRegion region,
            List<(int x, int y, bool vertical, int value)> joints, HashSet<int> before, IRandom random)
        {
            var inside = new HashSet<int>();
            foreach (int p in Board.PresentPieces())
            {
                bool full = true;
                foreach (var c in Level.Pieces[p]) if (!region.Contains(c)) { full = false; break; }
                if (full) inside.Add(p);
            }
            var pieces = new List<int>(inside);
            int start = pieces.Count == 0 ? 0 : (int)(random.NextDouble() * pieces.Count);
            var work = Board.Clone();
            for (int n = 0; n < pieces.Count; n++)
            {
                int p = pieces[(start + n) % pieces.Count];
                if (before.Contains(p) || NailAt(p) > 0) continue;
                foreach (var d in DirExt.All)
                {
                    var flips = FreeingFlips(work, region, inside, p, d);
                    if (flips == null) continue;
                    var fixedAt = new HashSet<(int, int, bool)>();
                    foreach (var f in flips) fixedAt.Add((f.x, f.y, f.vertical));
                    for (int tryNo = 0; tryNo < TargetedRedraws; tryNo++)
                    {
                        var values = new int[joints.Count];
                        bool changed = false;
                        var candidate = Board.Clone();
                        for (int i = 0; i < joints.Count; i++)
                        {
                            var j = joints[i];
                            int value = j.value;
                            if (fixedAt.Contains((j.x, j.y, j.vertical)))
                                foreach (var f in flips) { if (f.x == j.x && f.y == j.y && f.vertical == j.vertical) value = f.value; }
                            else if (tryNo > 0 && random.NextDouble() < .5) value = -value;
                            values[i] = value;
                            changed |= value != j.value;
                            if (j.vertical) candidate.SetVJoint(j.x, j.y, value); else candidate.SetHJoint(j.x, j.y, value);
                        }
                        if (!changed) continue;
                        bool frees = false;
                        foreach (int q in FreeNow(candidate)) if (!before.Contains(q)) { frees = true; break; }
                        if (!frees || !GreedySolver.Solve(candidate).Solved) continue;
                        var result = new List<(int, int, bool, int)>();
                        for (int i = 0; i < joints.Count; i++) result.Add((joints[i].x, joints[i].y, joints[i].vertical, values[i]));
                        return result;
                    }
                }
            }
            return null;
        }
        const int TargetedRedraws = 24;
        internal static bool TargetedSearch = true;   // tests measure the PM_66 sampling alone with false

        /* The tab changes that let piece p slide out towards d, or null if no change the wand may make is enough:
           p's own sides beside an empty cell turn hollow, and every present neighbour along the way turns its
           facing side hollow. `work` is a scratch copy of the board; it is restored before returning. */
        List<(int x, int y, bool vertical, int value)> FreeingFlips(Board work, WandRegion region, HashSet<int> inside, int p, Dir d)
        {
            var cells = Level.Pieces[p];
            Dir perpA = d.IsVertical() ? Dir.L : Dir.U, perpB = d.IsVertical() ? Dir.R : Dir.D;
            var flips = new List<(int x, int y, bool vertical, int value)>();
            var original = new List<(int x, int y, bool vertical, int value)>();
            void Hollow(Cell c, Dir side)
            {
                if (!JointOf(c, side, out int x, out int y, out bool vertical)) return;
                int now = vertical ? work.VJoint(x, y) : work.HJoint(x, y);
                if (now == 0) return;                                            // flat stays flat
                Cell a = vertical ? new Cell(x - 1, y) : new Cell(x, y - 1), b = new Cell(x, y);
                if (!region.Contains(a) || !region.Contains(b)) return;
                int pa = work.PieceAt(a), pb = work.PieceAt(b);
                if (pa == pb || !(pa < 0 || inside.Contains(pa)) || !(pb < 0 || inside.Contains(pb))) return;
                if (TravelRule.EdgeOf(work, c.X, c.Y, side) <= 0) return;      // already hollow
                foreach (var f in flips) if (f.x == x && f.y == y && f.vertical == vertical) return;   // claimed the other way
                original.Add((x, y, vertical, now));
                flips.Add((x, y, vertical, -now));
                if (vertical) work.SetVJoint(x, y, -now); else work.SetHJoint(x, y, -now);
            }
            foreach (var h in cells)
                foreach (var side in new[] { perpA, perpB })
                {
                    var n = h.Step(side);
                    if (Level.OnBoard(n) && work.PieceAt(n) < 0) Hollow(h, side);
                }
            for (int k = 1; k <= Math.Max(Level.Width, Level.Height) + 2; k++)
            {
                bool anyOn = false;
                foreach (var h in cells)
                {
                    var at = h.Step(d, k);
                    if (!Level.OnBoard(at)) continue;
                    anyOn = true;
                    foreach (var side in new[] { perpA, perpB })
                    {
                        var n = at.Step(side);
                        int o = work.PieceAt(n);
                        if (o >= 0 && o != p) Hollow(n, side.Opposite());
                    }
                }
                if (!anyOn) break;
            }
            bool exits = flips.Count > 0 && TravelRule.Limit(work, p, d).Exit;
            for (int i = original.Count - 1; i >= 0; i--)
            {
                var o = original[i];
                if (o.vertical) work.SetVJoint(o.x, o.y, o.value); else work.SetHJoint(o.x, o.y, o.value);
            }
            return exits ? flips : null;
        }

        bool JointOf(Cell c, Dir side, out int x, out int y, out bool vertical)
        {
            x = c.X; y = c.Y; vertical = !side.IsVertical();
            switch (side)
            {
                case Dir.L: return c.X >= 1 && c.X < Level.Width && Level.OnBoard(c);
                case Dir.R: x = c.X + 1; return c.X + 1 < Level.Width && Level.OnBoard(c);
                case Dir.U: return c.Y >= 1 && Level.OnBoard(c);
                default: y = c.Y + 1; return c.Y + 1 < Level.Height && Level.OnBoard(c);
            }
        }
    }
}
