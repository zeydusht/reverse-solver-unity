using System;
using System.Collections.Generic;
using System.Linq;
using ReverseSolver.Core;
using ReverseSolver.Editing;

namespace ReverseSolver.Generation
{
    /* D07 (Zeyd 2026-10-19): the hardest fair level, built on D06. M1 first: every row walled on one
       side and every column at one end (each piece keeps two ways out, so the board stays solvable),
       then the openers' seams locked until 1-3 pieces are free at the start. Then three inner bombs
       (fuse = dig + 1, + 2 only if the bombs cannot all be saved otherwise), three or four nails on the
       bombs' ways out with counters equal to the moves made before they leave. Fairness: SmartPlayer
       solves it with no valve, the stored solution wins. Psychology and the fun model are ignored. */
    public static class HardSeven
    {
        public sealed class Result
        {
            public LevelDraft Draft;
            public HardFactors Factors;
            public int Bombs, Nails, Chains, WallLanes, Lanes;
            public List<int> FuseSlack = new List<int>();
            public double Score;
            public uint Seed;
        }

        static bool Clean(LevelData l, out SmartPlayResult r) { r = SmartPlayer.Play(l); return r.Verdict == SmartVerdict.Won && r.Valves == 0; }
        static int Free(LevelDraft d) => TravelRule.FreePieces(new Board(d.ToLevelData())).Count;

        public static Result Try(LevelDraft d06, uint seed, out string why)
        {
            why = null;
            var rng = new Mulberry32(seed * 2246822519u + 101u);
            // M9 on a 6×7 board leaves 6 inner cells for 3 bombs, 2 chains (4 cells) and the nails: a 7×8 board has 12
            const int w = 7, h = 8;
            var d = LevelDraft.New(w, h);
            d.SetNumber(40);
            d.Genre = Genre.CokZor;
            if (d06 != null) { d.Art = d06.Art; for (int i = 0; i < 2; i++) d.SetPaletteColour(i, d06.Palette[i]); }
            var innerCells = new List<Cell>();
            for (int y = 2; y <= h - 3; y++) for (int x = 2; x <= w - 3; x++) innerCells.Add(new Cell(x, y));
            innerCells = innerCells.OrderBy(_ => rng.NextDouble()).ToList();
            var usedCells = new HashSet<Cell>();
            for (int k = 0; k < 2; k++)
            {
                var a = innerCells.First(c => !usedCells.Contains(c));
                var b = innerCells.FirstOrDefault(c => !usedCells.Contains(c) && !c.Equals(a) && c.X != a.X && c.Y != a.Y);
                if (!innerCells.Contains(b) || usedCells.Contains(b) || b.Equals(a)) { why = "zincir yeri yok"; return null; }
                if (!d.AddChain(a, b).Ok) { why = "zincir kurulamadı"; return null; }
                usedCells.Add(a); usedCells.Add(b);
            }
            d.SetTimer(400);
            LevelData l;
            // start open: every inner seam flat; the locks come back after the walls
            for (int y = 0; y < h; y++) for (int x = 1; x < w; x++) d.SetJoint(x, y, true, 0);
            for (int y = 1; y < h; y++) for (int x = 0; x < w; x++) d.SetJoint(x, y, false, 0);

            // walls: one side of every row, one end of every column, 1-2 lanes left fully open as doors
            int doors = 1 + (int)(rng.NextDouble() * 2), lanes = w + h;
            var doorLanes = new HashSet<int>();
            while (doorLanes.Count < doors) doorLanes.Add((int)(rng.NextDouble() * lanes));
            for (int y = 0; y < h; y++) if (!doorLanes.Contains(y)) d.SetSealed(rng.NextDouble() < .5 ? Dir.L : Dir.R, y, true);
            for (int x = 0; x < w; x++) if (!doorLanes.Contains(h + x)) d.SetSealed(rng.NextDouble() < .5 ? Dir.U : Dir.D, x, true);
            if (!Clean(d.ToLevelData(), out _)) { why = "duvarlarla çözülmüyor"; return null; }

            // M1: lock seams (openers first) until 1-3 free at the start
            for (int pass = 0; pass < 6 && Free(d) > 3; pass++) LockOpeners(d, rng);
            LockRest(d, rng);
            if (Free(d) > 3) { why = $"açılış {Free(d)}"; }
            l = d.ToLevelData();
            int bodies = l.Pieces.Count;
            int Edge(Cell c) => Math.Min(Math.Min(c.X, w - 1 - c.X), Math.Min(c.Y, h - 1 - c.Y));

            // three inner bombs with the deepest digs, fuse = dig + 1
            var inner = Enumerable.Range(0, bodies).Where(i => l.Pieces[i].Length == 1 && Edge(l.Pieces[i][0]) >= 2).OrderBy(_ => rng.NextDouble()).ToList();
            var digs = new List<(int piece, int dig)>();
            foreach (var i in inner)
            {
                var t = d.Clone(); t.SetBomb(l.Pieces[i][0], 99);
                var m = StructureMetrics.Of(t.ToLevelData(), 20_000);
                if (m.BombDigs.TryGetValue(i, out var dg) && dg.exact) digs.Add((i, dg.dig));
            }
            var bombs = digs.OrderByDescending(x => x.dig).Take(3).ToList();
            if (bombs.Count == 0) { why = "bomba yeri yok"; return null; }
            foreach (var b in bombs) d.SetBomb(l.Pieces[b.piece][0], b.dig + 1);
            var slack = bombs.Select(_ => 1).ToList();
            for (int k = 0; k < bombs.Count && !Clean(d.ToLevelData(), out _); k++) { d.SetBomb(l.Pieces[bombs[k].piece][0], bombs[k].dig + 2); slack[k] = 2; }
            if (!Clean(d.ToLevelData(), out var sp))
            {
                // the hardest bomb that still lets the others be saved goes
                for (int k = bombs.Count - 1; k >= 1 && !Clean(d.ToLevelData(), out sp); k--) { d.SetBomb(l.Pieces[bombs[k].piece][0], 0); bombs.RemoveAt(k); slack.RemoveAt(k); }
                if (!Clean(d.ToLevelData(), out sp)) { why = "bombalarla çözülmüyor"; return null; }
            }

            // nails on the bombs' ways out, counters = moves made before they leave
            var nailed = new List<int>();
            var board = new Board(d.ToLevelData());
            foreach (var b in bombs)
                foreach (var x in Cone(board, b.piece))
                {
                    if (nailed.Count >= 4) break;
                    var lv = d.ToLevelData();
                    if (lv.Pieces[x].Length != 1 || lv.Bombs.ContainsKey(x) || nailed.Contains(x) || Edge(lv.Pieces[x][0]) < 2) continue;
                    if (!Critical(d, lv, x, b.piece)) continue;
                    int leave = sp.Moves.FindIndex(mv => mv.Piece == x);
                    if (leave < 2) continue;
                    var t = d.Clone(); t.SetNail(lv.Pieces[x][0], leave);
                    if (Clean(t.ToLevelData(), out var sp2)) { d = t; sp = sp2; nailed.Add(x); break; }
                }
            for (int extra = 0; extra < 6 && nailed.Count < 3; extra++)
            {
                var lv = d.ToLevelData();
                var way = bombs.SelectMany(b => Cone(new Board(lv), b.piece)).Distinct().Where(x => lv.Pieces[x].Length == 1 && !lv.Bombs.ContainsKey(x) && !nailed.Contains(x) && Edge(lv.Pieces[x][0]) >= 2 && bombs.Any(b => Critical(d, lv, x, b.piece))).ToList();
                if (way.Count == 0) break;
                int x = way[(int)(rng.NextDouble() * way.Count)];
                int leave = sp.Moves.FindIndex(mv => mv.Piece == x);
                if (leave < 2) continue;
                var t = d.Clone(); t.SetNail(lv.Pieces[x][0], leave);
                if (Clean(t.ToLevelData(), out var sp2)) { d = t; sp = sp2; nailed.Add(x); }
            }

            // M6: each nail outlasts its bomb's free dig, so the bomb cannot be reached before the nail opens;
            // the fuses are then measured again with the nails (fuse = dig + 1, + 2 only if needed)
            foreach (var x in nailed.ToList())
            {
                var lv = d.ToLevelData();
                var target = bombs.FirstOrDefault(b => Critical(d, lv, x, b.piece));
                if (target.piece == 0 && target.dig == 0) continue;
                var t = d.Clone();
                t.SetNail(lv.Pieces[x][0], Math.Max(lv.Nails[x], target.dig + 1));
                var mt = StructureMetrics.Of(t.ToLevelData(), 20_000);
                var tl = t.ToLevelData();
                foreach (var b in bombs) if (mt.BombDigs.TryGetValue(b.piece, out var dg)) t.SetBomb(tl.Pieces[b.piece][0], dg.dig + 1);
                if (!Clean(t.ToLevelData(), out _))
                    foreach (var b in bombs) if (mt.BombDigs.TryGetValue(b.piece, out var dg)) t.SetBomb(tl.Pieces[b.piece][0], dg.dig + 2);
                if (Clean(t.ToLevelData(), out var sp3)) { d = t; sp = sp3; }
            }
            {
                var lv = d.ToLevelData();
                var mf = StructureMetrics.Of(lv, 20_000);
                slack = bombs.Select(b => mf.BombDigs.TryGetValue(b.piece, out var dg) ? lv.Bombs[b.piece] - dg.dig : 0).ToList();
            }

            // time share 1.15, the stored solution
            d.SetTimer(60);
            var ms = StructureMetrics.Of(d.ToLevelData());
            double sec = DifficultyModel.SecondsPerPiece(ms.ReadingPerMove, true, false);
            d.SetTimer(DifficultyModel.Time(bodies, sec, HardDesign.TimeShare));
            var level = d.ToLevelData();
            if (!Clean(level, out sp)) { why = "son tahta çözülmüyor"; return null; }
            var sol = new List<Move>();
            var gs = new GameSession(level, 1, "d07", new Mulberry32(1));
            Dir? lastDir = null; int lastLane = -1;
            foreach (var mv in sp.Moves)
            {
                var cell = level.Pieces[mv.Piece][0];
                var dirs = TravelRule.ExitDirs(gs.Board, mv.Piece);
                var pick = dirs.OrderBy(x => x == lastDir && (x.IsVertical() ? cell.X : cell.Y) == lastLane ? 1 : 0).FirstOrDefault();
                if (!dirs.Contains(pick)) pick = mv.Dir;
                gs.TryExit(mv.Piece, pick);
                sol.Add(new Move(mv.Piece, pick)); lastDir = pick; lastLane = pick.IsVertical() ? cell.X : cell.Y;
            }
            d.Solution = sol;
            level = d.ToLevelData();
            var g = new GameSession(level, 1, "d07", new Mulberry32(1));
            foreach (var mv in level.Solution) g.TryExit(mv.Piece, mv.Dir);
            if (g.Outcome != Outcome.Win || g.ValveCount > 0) { why = "kayıtlı çözüm kazanmadı"; return null; }

            var f = HardFactors.Of(level);
            int walled = level.Sealed.Sum(kv => kv.Value.Length);
            return new Result
            {
                Draft = d, Factors = f, Bombs = level.Bombs.Count, Nails = level.Nails.Count, Chains = level.Pieces.Count(p => p.Length == 2),
                WallLanes = walled, Lanes = 2 * (w + h), FuseSlack = slack, Seed = seed,
                Score = -20 * Math.Max(0, f.FreeAtStart - 3) - 6 * Math.Max(0, f.MaxFreeEarly - 3) - 3 * Math.Max(0, f.Cascade - 2)
                        + 6 * level.Bombs.Count + 3 * level.Nails.Count + (f.Dig > 0 ? f.Dig : 0)
            };
        }

        /* The nail on x counts for the bomb: with x pinned for good the bomb's dig grows (or the bomb cannot be reached). */
        static bool Critical(LevelDraft d, LevelData lv, int x, int bomb)
        {
            var free = StructureMetrics.Of(lv, 20_000);
            if (!free.BombDigs.TryGetValue(bomb, out var a)) return false;
            var t = d.Clone();
            t.SetNail(lv.Pieces[x][0], 99);
            var pinned = StructureMetrics.Of(t.ToLevelData(), 20_000);
            return !pinned.BombDigs.TryGetValue(bomb, out var b) || b.dig > a.dig;
        }

        /* Lock the seams around the pieces that can leave now, while the board stays clean-solvable. */
        static void LockOpeners(LevelDraft d, IRandom rng)
        {
            var l = d.ToLevelData();
            var board = new Board(l);
            var free = TravelRule.FreePieces(board);
            var seams = new List<(int x, int y, bool v)>();
            foreach (var p in free)
                foreach (var c in l.Pieces[p])
                {
                    if (c.X > 0 && l.VJoint(c.X, c.Y) == 0) seams.Add((c.X, c.Y, true));
                    if (c.X < l.Width - 1 && l.VJoint(c.X + 1, c.Y) == 0) seams.Add((c.X + 1, c.Y, true));
                    if (c.Y > 0 && l.HJoint(c.X, c.Y) == 0) seams.Add((c.X, c.Y, false));
                    if (c.Y < l.Height - 1 && l.HJoint(c.X, c.Y + 1) == 0) seams.Add((c.X, c.Y + 1, false));
                }
            seams = seams.Distinct().OrderBy(_ => rng.NextDouble()).ToList();
            foreach (var j in seams)
            {
                if (Free(d) <= 2) return;
                foreach (int o in rng.NextDouble() < .5 ? new[] { 1, -1 } : new[] { -1, 1 })
                {
                    d.SetJoint(j.x, j.y, j.v, o);
                    if (Clean(d.ToLevelData(), out _)) goto next;
                    d.SetJoint(j.x, j.y, j.v, 0);
                }
                next:;
            }
        }

        /* M3: lock the other flat seams too (more neighbours hold each piece) while it stays solvable and M1 holds. */
        static void LockRest(LevelDraft d, IRandom rng)
        {
            var l = d.ToLevelData();
            var seams = new List<(int x, int y, bool v)>();
            for (int y = 0; y < l.Height; y++) for (int x = 1; x < l.Width; x++) if (l.VJoint(x, y) == 0) seams.Add((x, y, true));
            for (int y = 1; y < l.Height; y++) for (int x = 0; x < l.Width; x++) if (l.HJoint(x, y) == 0) seams.Add((x, y, false));
            int k0 = Free(d);
            foreach (var j in seams.OrderBy(_ => rng.NextDouble()))
                foreach (int o in rng.NextDouble() < .5 ? new[] { 1, -1 } : new[] { -1, 1 })
                {
                    d.SetJoint(j.x, j.y, j.v, o);
                    if (Free(d) <= Math.Max(k0, 1) && Clean(d.ToLevelData(), out _)) break;
                    d.SetJoint(j.x, j.y, j.v, 0);
                }
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
