using System;
using System.Collections.Generic;
using System.Linq;
using ReverseSolver.Core;
using ReverseSolver.Editing;

namespace ReverseSolver.Generation
{
    /* D06 (Zeyd 2026-10-18, docs/ZORLUK_METRIKLERI.md): the hardest fair level by M1-M9, fun ignored.
       M1 and M3 first: every seam SmartPlayer can still solve around is locked (few free openers, most
       pieces held by several neighbours); then the bomb on an inner single piece with the deepest dig
       and fuse = dig + 1 (M5), a nail on its way with the counter equal to the moves made before it
       leaves (M6), diagonal chains kept off the edges (M7, M9), and walls on the pieces' easy sides as
       long as the board stays solvable (M8). Time share 1.15; fairness never traded. */
    public static class HardSix
    {
        public sealed class Result
        {
            public LevelDraft Draft;
            public LevelData Level;
            public HardFactors Factors;
            public int Held;          // M rows held
            public double Score;
            public uint Seed;
            public string Params;
        }

        public static SlotSpec Spec()
        {
            var s = SlotSpec.For(40, Genre.CokZor);
            s.Boards = new[] { (6, 7) };
            s.MinBodies = 36; s.MaxBodies = 40;
            s.MinNails = s.MaxNails = 0; s.MinBombs = s.MaxBombs = 0;
            s.MinSeals = s.MaxSeals = 0; s.MinChains = 2; s.MaxChains = 2; s.MinRemote = 2; s.MaxRemote = 2;
            return s;
        }

        public static bool Better(Result a, Result b) => a.Held != b.Held ? a.Held > b.Held : a.Score > b.Score;

        static bool Clean(LevelData l, out SmartPlayResult r) { r = SmartPlayer.Play(l); return r.Verdict == SmartVerdict.Won && r.Valves == 0; }

        public static Result Try(uint seed, Catalog catalog, out string why)
        {
            var rng = new Mulberry32(seed * 2654435761u + 13u);
            var p = new GenParams
            {
                DiagonalChains = true, ChainsInside = true, Interlock = 3 + 5 * rng.NextDouble(), Knot = 1 + 2 * rng.NextDouble(),
                Sweep = -1.5 + rng.NextDouble(), EdgeFirst = -1.5 * rng.NextDouble()
            };
            var spec = Spec();
            var c = Generator.TryMake(spec, p, seed, catalog, out why);
            if (c == null) return null;
            var d = c.Draft.Clone();

            // M1 + M3: lock every seam the board stays clean-solvable with, openers' seams first
            LockAll(d, rng);
            var l = d.ToLevelData();
            int bodies = l.Pieces.Count;
            int Edge(Cell x) => Math.Min(Math.Min(x.X, l.Width - 1 - x.X), Math.Min(x.Y, l.Height - 1 - x.Y));

            // M5: the inner single piece with the deepest dig; fuse = dig + 1
            var inner = Enumerable.Range(0, bodies).Where(i => l.Pieces[i].Length == 1 && Edge(l.Pieces[i][0]) >= 2).ToList();
            int bomb = -1, bestDig = -1;
            foreach (var i in inner.OrderBy(_ => rng.NextDouble()).Take(6))
            {
                var t = d.Clone(); t.SetBomb(l.Pieces[i][0], 99);
                var m = StructureMetrics.Of(t.ToLevelData(), 40_000);
                if (m.BombDigs.TryGetValue(i, out var dg) && dg.exact && dg.dig > bestDig) { bestDig = dg.dig; bomb = i; }
            }
            if (bomb < 0) { why = "bomba yeri yok"; return null; }

            // M6: a nail on the bomb's way out, inside; counter first 30 % of the bodies, then the moves before it leaves
            var cone = Cone(new Board(l), bomb).Where(x => l.Pieces[x].Length == 1 && Edge(l.Pieces[x][0]) >= 2).ToList();
            if (cone.Count == 0) { why = "bomba yolunda içeride parça yok"; return null; }
            int nail = cone[0];
            d.SetNail(l.Pieces[nail][0], (int)Math.Round(.30 * bodies));
            d.SetBomb(l.Pieces[bomb][0], 99);
            var m1 = StructureMetrics.Of(d.ToLevelData(), 60_000);
            if (!m1.BombDigs.TryGetValue(bomb, out var dig) || !dig.exact) { why = "kazma yolu ölçülemedi"; return null; }
            d.SetBomb(l.Pieces[bomb][0], dig.dig + 1);
            if (!Clean(d.ToLevelData(), out var sp0)) { why = "fitil +1 ile çözülmüyor"; return null; }
            int leave = sp0.Moves.FindIndex(mv => mv.Piece == nail);
            if (leave > 0)
            {
                var t = d.Clone(); t.SetNail(l.Pieces[nail][0], leave);
                if (Clean(t.ToLevelData(), out _)) d = t;
            }

            // M8: walls on easy sides while it stays solvable
            Walls(d, rng);

            // time share 1.15, the solution with sides turned
            d.SetTimer(60);
            var ms = StructureMetrics.Of(d.ToLevelData());
            double s = DifficultyModel.SecondsPerPiece(ms.ReadingPerMove, true, false);
            int timer = DifficultyModel.Time(bodies, s, HardDesign.TimeShare);
            d.SetTimer(timer);
            var level = d.ToLevelData();
            if (!Clean(level, out var sp)) { why = "son tahta çözülmüyor"; return null; }
            var moves = new List<Move>();
            var g = new GameSession(level, 1, "hard6", new Mulberry32(1));
            Dir? last = null; int lastLane = -1;
            foreach (var mv in sp.Moves)
            {
                var dirs = TravelRule.ExitDirs(g.Board, mv.Piece);
                var cell = level.Pieces[mv.Piece][0];
                Dir pick = mv.Dir;
                foreach (var dd in dirs.OrderBy(x => x == last && (x.IsVertical() ? cell.X : cell.Y) == lastLane ? 1 : 0)) { pick = dd; break; }
                if (g.TryExit(mv.Piece, pick) != CommandResult.Ok) { why = "çözüm oynanmadı"; return null; }
                moves.Add(new Move(mv.Piece, pick)); last = pick; lastLane = pick.IsVertical() ? cell.X : cell.Y;
            }
            if (g.Outcome != Outcome.Win || g.ValveCount > 0) { why = "çözüm kazanmadı"; return null; }
            d.Solution = moves;
            level = d.ToLevelData();
            var check = DesignCheck.Run(level);
            if (!check.FollowsRule || !check.Fits) { why = "tasarım kontrolü: " + check.Verdict; return null; }
            if (timer / (bodies * s) < HardDesign.MinTimeShare) { why = "süre payı"; return null; }

            var f = HardFactors.Of(level);
            var rows = f.Metrics();
            why = null;
            return new Result
            {
                Draft = d, Level = level, Factors = f, Seed = seed, Params = p.ToString(), Held = rows.Count(r => r.ok),
                // M1 and M3 weigh most
                Score = -10 * Math.Max(0, f.FreeAtStart - 2) + 20 * f.Tied3Share - 2 * f.MaxSingleTie - 2 * f.MaxFreeEarly + f.Dig + 2 * f.EasyWalled
            };
        }

        /* Zeyd's fast path (2026-10-18): D06 = D05 edited by hand rules. Returns the draft and a log. */
        public static LevelDraft FromD05(LevelDraft d05, List<string> log)
        {
            var rng = new Mulberry32(20261018);
            var d = d05.Clone();
            var l = d.ToLevelData();
            int bodies = l.Pieces.Count;
            int Edge(Cell x) => Math.Min(Math.Min(x.X, l.Width - 1 - x.X), Math.Min(x.Y, l.Height - 1 - x.Y));
            // clear the old bomb and nails (re-placed below), long timer while editing
            foreach (var b in l.Bombs.Keys) d.SetBomb(l.Pieces[b][0], 0);
            foreach (var n in l.Nails.Keys) d.SetNail(l.Pieces[n][0], 0);
            d.SetTimer(300);
            log.Add($"D05: ilk hamlede serbest {TravelRule.FreePieces(new Board(d.ToLevelData())).Count}");

            // 1. lock seams (openers first) while SmartPlayer still solves clean
            for (int pass = 0; pass < 4 && TravelRule.FreePieces(new Board(d.ToLevelData())).Count > 2; pass++) LockAll(d, rng);
            l = d.ToLevelData();
            log.Add($"kilitten sonra: ilk hamlede serbest {TravelRule.FreePieces(new Board(l)).Count}, açık dikiş %{LevelStats.Of(l).OpenPercent}");

            // 2. bomb on the inner single piece with the deepest dig; fuse = dig + 1
            int bomb = -1, bestDig = -1;
            foreach (var i in Enumerable.Range(0, bodies).Where(i => l.Pieces[i].Length == 1 && Edge(l.Pieces[i][0]) >= 2))
            {
                var t = d.Clone(); t.SetBomb(l.Pieces[i][0], 99);
                var m = StructureMetrics.Of(t.ToLevelData(), 40_000);
                if (m.BombDigs.TryGetValue(i, out var dg) && dg.exact && dg.dig > bestDig) { bestDig = dg.dig; bomb = i; }
            }
            if (bomb < 0) { log.Add("bomba yeri bulunamadı"); return null; }
            // 3. the nail on the bomb's nearest blocker, inside if possible
            var cone = Cone(new Board(l), bomb).Where(x => l.Pieces[x].Length == 1).ToList();
            int nail = cone.Where(x => Edge(l.Pieces[x][0]) >= 2).DefaultIfEmpty(cone.Count > 0 ? cone[0] : -1).First();
            if (nail >= 0) d.SetNail(l.Pieces[nail][0], (int)Math.Round(.30 * bodies));
            d.SetBomb(l.Pieces[bomb][0], 99);
            var m1 = StructureMetrics.Of(d.ToLevelData(), 60_000);
            int dig = m1.BombDigs.TryGetValue(bomb, out var dd1) ? dd1.dig : bestDig;
            int fuse = dig + 1;
            d.SetBomb(l.Pieces[bomb][0], fuse);
            while (!Clean(d.ToLevelData(), out _) && fuse < dig + 4) d.SetBomb(l.Pieces[bomb][0], ++fuse);
            log.Add($"bomba ({l.Pieces[bomb][0].X},{l.Pieces[bomb][0].Y}) kenar {Edge(l.Pieces[bomb][0])}, kazma yolu {dig}, fitil {fuse} (+{fuse - dig})");
            if (nail >= 0 && Clean(d.ToLevelData(), out var sp0))
            {
                int leave = sp0.Moves.FindIndex(mv => mv.Piece == nail);
                if (leave > 0) { var t = d.Clone(); t.SetNail(l.Pieces[nail][0], leave); if (Clean(t.ToLevelData(), out _)) d = t; }
                log.Add($"çivi ({l.Pieces[nail][0].X},{l.Pieces[nail][0].Y}) kenar {Edge(l.Pieces[nail][0])}, sayaç {d.ToLevelData().Nails[nail]}");
            }
            // 4. walls on easy sides
            Walls(d, rng);
            // timer share 1.15 and the solution
            d.SetTimer(60);
            var ms = StructureMetrics.Of(d.ToLevelData());
            double sec = DifficultyModel.SecondsPerPiece(ms.ReadingPerMove, true, false);
            d.SetTimer(DifficultyModel.Time(bodies, sec, HardDesign.TimeShare));
            var level = d.ToLevelData();
            if (!Clean(level, out var sp)) { log.Add("son tahta temiz çözülmüyor"); return null; }
            d.Solution = sp.Moves.ToList();
            level = d.ToLevelData();
            var g = new GameSession(level, 1, "d06", new Mulberry32(1));
            foreach (var mv in level.Solution) g.TryExit(mv.Piece, mv.Dir);
            log.Add($"kayıtlı çözüm: {g.Outcome}, supap {g.ValveCount}; süre {level.TimerSeconds} sn");
            return g.Outcome == Outcome.Win && g.ValveCount == 0 ? d : null;
        }

        static void LockAll(LevelDraft d, IRandom rng)
        {
            var l = d.ToLevelData();
            var seams = new List<(int x, int y, bool v)>();
            for (int y = 0; y < l.Height; y++) for (int x = 1; x < l.Width; x++) if (l.VJoint(x, y) == 0) seams.Add((x, y, true));
            for (int y = 1; y < l.Height; y++) for (int x = 0; x < l.Width; x++) if (l.HJoint(x, y) == 0) seams.Add((x, y, false));
            for (int i = seams.Count - 1; i > 0; i--) { int k = (int)(rng.NextDouble() * (i + 1)); (seams[i], seams[k]) = (seams[k], seams[i]); }
            var free = new HashSet<Cell>();
            foreach (var pc in TravelRule.FreePieces(new Board(l))) foreach (var c in l.Pieces[pc]) free.Add(c);
            seams = seams.OrderBy(j => free.Contains(new Cell(j.x, j.y)) || free.Contains(j.v ? new Cell(j.x - 1, j.y) : new Cell(j.x, j.y - 1)) ? 0 : 1).ToList();
            foreach (var j in seams)
            {
                int first = rng.NextDouble() < .5 ? 1 : -1;
                foreach (int o in new[] { first, -first })
                {
                    d.SetJoint(j.x, j.y, j.v, o);
                    if (Clean(d.ToLevelData(), out _)) goto next;
                    d.SetJoint(j.x, j.y, j.v, 0);
                }
                next:;
            }
        }

        static void Walls(LevelDraft d, IRandom rng)
        {
            var l = d.ToLevelData();
            var tried = new HashSet<(Dir, int)>();
            int added = 0;
            foreach (var p in Enumerable.Range(0, l.Pieces.Count).OrderBy(_ => rng.NextDouble()))
            {
                if (added >= 5) break;
                var c = l.Pieces[p][0];
                var easy = DirExt.All.OrderBy(dd => Run(l, c, dd)).First();
                int lane = easy.IsVertical() ? c.X : c.Y;
                if (!tried.Add((easy, lane))) continue;
                d.SetSealed(easy, lane, true);
                if (Clean(d.ToLevelData(), out _)) added++; else d.SetSealed(easy, lane, false);
            }
        }

        static int Run(LevelData l, Cell c, Dir d)
        {
            int k = 0;
            for (var x = c.Step(d); l.OnBoard(x); x = x.Step(d)) k++;
            return k;
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
