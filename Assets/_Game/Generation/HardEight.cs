using System;
using System.Collections.Generic;
using System.Linq;
using ReverseSolver.Core;
using ReverseSolver.Editing;

namespace ReverseSolver.Generation
{
    /* D07, solution first (PM 2026-10-19, B): on an open 7×8 board with one wall per row and column
       and two inner chains, a removal order is built that never leaves the same lane the same way
       twice running (M4) and digs the bombs out first; then every seam is locked that the order still
       replays through (M1, M3: no solver's search limit, the stored solution proves the board solvable).
       Fuses and nail counters come from the order: fuse = the moves up to the bomb (dig + slack),
       counter = the moves before the nailed piece leaves (no spare move). Fairness: the stored
       solution wins with no valve; the unfair list is clean; cells fit 44 pt. */
    public static class HardEight
    {
        public sealed class Result
        {
            public LevelDraft Draft;
            public HardFactors Factors;
            public int Held;
            public double Score;
            public uint Seed;
            public List<int> FuseSlack = new List<int>();
            public string Diag;
        }

        const int W = 7, H = 8;

        public static Result Try(LevelDraft look, uint seed, out string why)
        {
            why = null;
            var rng = new Mulberry32(seed * 2654435761u + 77u);
            var d = LevelDraft.New(W, H);
            d.SetNumber(40);
            d.Genre = Genre.CokZor;
            if (look != null) { d.Art = look.Art; for (int i = 0; i < 2; i++) d.SetPaletteColour(i, look.Palette[i]); }
            int Edge(Cell c) => Math.Min(Math.Min(c.X, W - 1 - c.X), Math.Min(c.Y, H - 1 - c.Y));

            // two inner chains in another row and column, not touching (M7, M9)
            var inner = new List<Cell>();
            for (int y = 2; y <= H - 3; y++) for (int x = 2; x <= W - 3; x++) inner.Add(new Cell(x, y));
            inner = inner.OrderBy(_ => rng.NextDouble()).ToList();
            var used = new HashSet<Cell>();
            for (int k = 0; k < 2; k++)
            {
                var a = inner.FirstOrDefault(c => !used.Contains(c));
                var b = inner.FirstOrDefault(c => !used.Contains(c) && c.X != a.X && c.Y != a.Y);
                if (!inner.Contains(b) || used.Contains(b) || b.Equals(a) || !d.AddChain(a, b).Ok) { why = "zincir yeri yok"; return null; }
                used.Add(a); used.Add(b);
            }
            // walls: one side of every row, one end of every column (M8)
            for (int y = 0; y < H; y++) d.SetSealed(rng.NextDouble() < .5 ? Dir.L : Dir.R, y, true);
            for (int x = 0; x < W; x++) d.SetSealed(rng.NextDouble() < .5 ? Dir.U : Dir.D, x, true);
            var l = d.ToLevelData();
            int n = l.Pieces.Count;

            // three inner single bombs (M5, M9)
            var bombs = Enumerable.Range(0, n).Where(i => l.Pieces[i].Length == 1 && Edge(l.Pieces[i][0]) >= 2).OrderBy(_ => rng.NextDouble()).Take(3).ToList();
            if (bombs.Count < 3) { why = "bomba yeri yok"; return null; }

            // the order on the open board: the bombs' ways out first, never the same lane the same way twice running
            var order = BuildOrder(l, bombs, rng, out why);
            if (order == null) return null;

            // M1 + M3: lock every seam the order still replays through (openers first)
            bool baseOk = Replays(d.ToLevelData(), order);
            int locked0 = LockFromOrder(d, order, rng);
            int wallsAdded = 0;
            // M1: more walls while the order still goes through (each one closes openers), then lock again
            var lanes = new List<(Dir side, int lane)>();
            foreach (var side in DirExt.All) for (int ln = 0; ln < (side.IsVertical() ? W : H); ln++) lanes.Add((side, ln));
            foreach (var (side, ln) in lanes.OrderBy(_ => rng.NextDouble()))
            {
                if (TravelRule.FreePieces(new Board(d.ToLevelData())).Count <= 2) break;
                var cur = d.ToLevelData();
                if (cur.Sealed.TryGetValue(side, out var sl) && sl.Contains(ln)) continue;
                d.SetSealed(side, ln, true);
                if (!Replays(d.ToLevelData(), order)) d.SetSealed(side, ln, false); else wallsAdded++;
            }
            int locked1 = LockFromOrder(d, order, rng);
            Diag = $"açık tahtada sıra oynanıyor {baseOk}, kilit {locked0}+{locked1}, ek duvar {wallsAdded}";
            l = d.ToLevelData();

            return Finish(d, order, rng, seed, out why, bombs);
        }

        /* The peel way (Generator): every seam locked at first, each step the move that flattens the fewest
           seams, so few pieces open at the start and most stay held; then bombs, nails and sides from the order. */
        public static Result TryPeel(LevelDraft look, uint seed, out string why)
        {
            var rng = new Mulberry32(seed * 40503u + 9u);
            var spec = SlotSpec.For(40, Genre.CokZor);
            spec.Boards = new[] { (W, H) };
            spec.MinBodies = 50; spec.MaxBodies = 56;
            spec.MinNails = spec.MaxNails = 0; spec.MinBombs = spec.MaxBombs = 0;
            spec.MinSeals = 13; spec.MaxSeals = 18; spec.MinChains = spec.MaxChains = 2; spec.MinRemote = spec.MaxRemote = 2;
            var p = new GenParams { ChainsInside = true, Interlock = 8 + 6 * rng.NextDouble(), Knot = 2 * rng.NextDouble(), Sweep = -2 * rng.NextDouble(), EdgeFirst = -2 * rng.NextDouble() };
            var c = Generator.TryMake(spec, p, seed, null, out why);
            if (c == null) return null;
            var d = c.Draft.Clone();
            d.Genre = Genre.CokZor;
            if (look != null) { d.Art = look.Art; for (int i = 0; i < 2; i++) d.SetPaletteColour(i, look.Palette[i]); }
            return Finish(d, c.Level.Solution.ToList(), rng, seed, out why);
        }


        /* Bombs (given, or the three inner singles whose order position is closest to their dig), nails on
           their ways out with counters = moves before they leave, fuses = moves up to the bomb, the stored solution. */
        static Result Finish(LevelDraft d, List<Move> order, IRandom rng, uint seed, out string why, List<int> bombs = null)
        {
            why = null;
            var l = d.ToLevelData();
            int n = l.Pieces.Count;
            int Edge(Cell c) => Math.Min(Math.Min(c.X, W - 1 - c.X), Math.Min(c.Y, H - 1 - c.Y));
            var at = new Dictionary<int, int>();
            for (int i = 0; i < order.Count; i++) at[order[i].Piece] = i;
            if (bombs == null)
            {
                var cand = new List<(int p, int slack)>();
                foreach (var i in Enumerable.Range(0, n).Where(i => l.Pieces[i].Length == 1 && Edge(l.Pieces[i][0]) >= 2 && at[i] >= 8))
                {
                    var t = d.Clone(); t.SetBomb(l.Pieces[i][0], 99);
                    var m = StructureMetrics.Of(t.ToLevelData(), 20_000);
                    if (m.BombDigs.TryGetValue(i, out var dg) && dg.exact && dg.dig >= 8) cand.Add((i, at[i] + 1 - dg.dig));
                }
                bombs = cand.OrderBy(x => x.slack).ThenBy(_ => rng.NextDouble()).Take(3).Select(x => x.p).ToList();
                if (bombs.Count == 0) { why = "bomba yeri yok"; return null; }
            }
            var cones = bombs.ToDictionary(b => b, b => Cone(new Board(l), b));
            var nailable = bombs.SelectMany(b => cones[b]).Distinct()
                .Where(x => l.Pieces[x].Length == 1 && !bombs.Contains(x) && Edge(l.Pieces[x][0]) >= 2 && at[x] >= 3)
                .OrderBy(x => at[x]).ToList();
            foreach (var x in nailable.Take(4)) d.SetNail(l.Pieces[x][0], at[x]);
            foreach (var b in bombs) d.SetBomb(l.Pieces[b][0], at[b] + 1);
            l = d.ToLevelData();
            if (!Replays(l, order)) { why = "çiviler / fitillerle sıra oynanmıyor"; return null; }

            // time share 1.15 and the stored solution
            d.SetTimer(60);
            var ms = StructureMetrics.Of(d.ToLevelData(), 20_000);
            double s = DifficultyModel.SecondsPerPiece(ms.ReadingPerMove, true, false);
            int timer = DifficultyModel.Time(n, s, HardDesign.TimeShare);
            d.SetTimer(timer);
            var sides = Sides(d.ToLevelData(), order);
            if (sides == null) { why = "sıra son tahtada oynanmıyor"; return null; }
            d.Solution = sides;
            var level = d.ToLevelData();
            var g = new GameSession(level, 1, "d07", new Mulberry32(1));
            g.ClockOnFirstMove = false;
            foreach (var mv in level.Solution) if (g.TryExit(mv.Piece, mv.Dir) != CommandResult.Ok) { why = "kayıtlı çözüm takıldı"; return null; }
            if (g.Outcome != Outcome.Win || g.ValveCount > 0) { why = "kayıtlı çözüm kazanmadı"; return null; }
            var check = DesignCheck.Run(level, 1);
            if (!check.Fits) { why = "hücre 44 pt altı"; return null; }
            // the 75 s cap is left out: a 7×8 board cannot keep it with a 1.15 share (reported)
            var unfair = Calibration.UnfairReasons(level, HardSix.Spec(), ms, timer, s).Where(u => !u.StartsWith("süre")).ToList();
            if (unfair.Count > 0) { why = "haksız: " + string.Join(", ", unfair); return null; }

            var f = HardFactors.Of(level);
            var slack = bombs.Select(b => ms.BombDigs.TryGetValue(b, out var dg) ? level.Bombs[b] - dg.dig : 99).ToList();
            var rows = f.Metrics();
            return new Result
            {
                Draft = d, Factors = f, Seed = seed, Held = rows.Count(r => r.ok), FuseSlack = slack, Diag = Diag,
                Score = 20 * f.Tied3Share - 2 * f.MaxSingleTie - 5 * Math.Max(0, f.FreeAtStart - 2) - 3 * Math.Max(0, f.LaneRun - 2) - slack.Sum()
            };
        }

        static List<Move> BuildOrder(LevelData l, List<int> bombs, IRandom rng, out string why)
        {
            why = null;
            var board = new Board(l);
            var order = new List<Move>();
            Dir? lastDir = null; int lastLane = -1;
            var bombsLeft = new List<int>(bombs);
            while (board.PresentCount > 0)
            {
                var free = TravelRule.FreePieces(board);
                if (free.Count == 0) { why = "açık tahtada sıra kurulamadı"; return null; }
                // the next bomb's way out first
                var target = bombsLeft.Where(b => board.IsPresent(b)).DefaultIfEmpty(-1).First();
                var cone = target >= 0 ? new HashSet<int>(Cone(board, target)) { target } : new HashSet<int>();
                var pool = free.Where(cone.Contains).ToList();
                if (pool.Count == 0) pool = free;
                var options = new List<(int p, Dir d, int bad)>();
                foreach (var p in pool)
                    foreach (var dd in TravelRule.ExitDirs(board, p))
                    {
                        var c = l.Pieces[p][0];
                        int lane = dd.IsVertical() ? c.X : c.Y;
                        options.Add((p, dd, dd == lastDir && lane == lastLane ? 1 : 0));
                    }
                if (options.Count == 0) { why = "açık tahtada sıra kurulamadı"; return null; }
                var best = options.Where(o => o.bad == 0).ToList();
                if (best.Count == 0) best = options;
                var pick = best[(int)(rng.NextDouble() * best.Count)];
                board.Remove(pick.p);
                order.Add(new Move(pick.p, pick.d));
                lastDir = pick.d; lastLane = pick.d.IsVertical() ? l.Pieces[pick.p][0].X : l.Pieces[pick.p][0].Y;
                bombsLeft.Remove(pick.p);
            }
            return order;
        }

        static bool Replays(LevelData l, List<Move> order) => Sides(l, order) != null;

        /* The same piece order with any legal side, avoiding the last move's lane and side; null if a piece cannot go. */
        static List<Move> Sides(LevelData l, List<Move> order)
        {
            var g = new GameSession(l, 1, "order", new Mulberry32(1));
            g.ClockOnFirstMove = false;
            var outMoves = new List<Move>(order.Count);
            Dir? lastDir = null; int lastLane = -1;
            foreach (var mv in order)
            {
                if (g.NailAt(mv.Piece) > 0) return null;
                var dirs = TravelRule.ExitDirs(g.Board, mv.Piece);
                if (dirs.Count == 0) return null;
                var c = l.Pieces[mv.Piece][0];
                var pick = dirs.OrderBy(x => x == lastDir && (x.IsVertical() ? c.X : c.Y) == lastLane ? 1 : 0).First();
                if (g.TryExit(mv.Piece, pick) != CommandResult.Ok) return null;
                outMoves.Add(new Move(mv.Piece, pick)); lastDir = pick; lastLane = pick.IsVertical() ? c.X : c.Y;
            }
            return g.Outcome == Outcome.Win && g.ValveCount == 0 ? outMoves : null;
        }

        [ThreadStatic] public static string Diag;

        static int LockFromOrder(LevelDraft d, List<Move> order, IRandom rng)
        {
            int locked = 0;
            var l = d.ToLevelData();
            var seams = new List<(int x, int y, bool v)>();
            for (int y = 0; y < l.Height; y++) for (int x = 1; x < l.Width; x++) if (l.VJoint(x, y) == 0) seams.Add((x, y, true));
            for (int y = 1; y < l.Height; y++) for (int x = 0; x < l.Width; x++) if (l.HJoint(x, y) == 0) seams.Add((x, y, false));
            var free = new HashSet<Cell>();
            foreach (var p in TravelRule.FreePieces(new Board(l))) foreach (var c in l.Pieces[p]) free.Add(c);
            seams = seams.OrderBy(j => (free.Contains(new Cell(j.x, j.y)) || free.Contains(j.v ? new Cell(j.x - 1, j.y) : new Cell(j.x, j.y - 1)) ? 0 : 1) + rng.NextDouble() * .5).ToList();
            foreach (var j in seams)
                foreach (int o in rng.NextDouble() < .5 ? new[] { 1, -1 } : new[] { -1, 1 })
                {
                    d.SetJoint(j.x, j.y, j.v, o);
                    if (Replays(d.ToLevelData(), order)) { locked++; break; }
                    d.SetJoint(j.x, j.y, j.v, 0);
                }
            return locked;
        }

        static List<int> Cone(Board b, int target)
        {
            var order = new List<int>();
            var seen = new HashSet<int> { target };
            var q = new Queue<int>();
            q.Enqueue(target);
            while (q.Count > 0)
            {
                int x = q.Dequeue();
                foreach (var dd in DirExt.All)
                {
                    int bl = TravelRule.Limit(b, x, dd).Blocker;
                    if (bl >= 0 && seen.Add(bl)) { q.Enqueue(bl); order.Add(bl); }
                }
            }
            return order;
        }
    }
}
