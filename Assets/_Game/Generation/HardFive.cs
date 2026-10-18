using System;
using System.Collections.Generic;
using System.Linq;
using ReverseSolver.Core;
using ReverseSolver.Editing;

namespace ReverseSolver.Generation
{
    /* Zeyd's second hard Çok zor level (2026-10-18, after D04): every one of the five hardness
       factors (HardFactors) at least as good as the web L40 and on Zeyd's target. The generator
       builds a 6×7 board with diagonal remote chains and one bomb; the nails go onto the bomb's
       way out (counters 30 / 40 % of the bodies), the fuse is the proved dig + 2, the timer
       share 1.15, the solution SmartPlayer's with exit sides turned. Fairness is never traded:
       solvable, no valve, the stored solution wins, DesignCheck and the unfair list clean. */
    public static class HardFive
    {
        public sealed class Result
        {
            public LevelDraft Draft;
            public LevelData Level;
            public HardFactors Factors;
            public int Missed;                // factor rows not held
            public double Score;
            public uint Seed;
            public string Params;
        }

        public const int L40CrossChains = 2, L40FakeFree = 5;       // measured with HardFactors, 2026-10-18

        public static SlotSpec Spec()
        {
            var s = SlotSpec.For(40, Genre.CokZor);
            s.Boards = new[] { (6, 7) };
            s.MinBodies = 36; s.MaxBodies = 40;
            s.MinNails = s.MaxNails = 2; s.MinBombs = s.MaxBombs = 1;
            s.MinSeals = 0; s.MaxSeals = 2; s.MinChains = 2; s.MaxChains = 3; s.MinRemote = 2; s.MaxRemote = 3;
            return s;
        }

        public static bool Better(Result a, Result b) => a.Missed != b.Missed ? a.Missed < b.Missed : a.Score > b.Score;

        public static Result Try(uint seed, Catalog catalog, out string why)
        {
            var rng = new Mulberry32(seed * 3266489917u + 11u);
            var p = new GenParams
            {
                DiagonalChains = true, Interlock = 2 + 6 * rng.NextDouble(), Knot = 1 + 2 * rng.NextDouble(), Ease = .4 * rng.NextDouble(),
                Sweep = -1.5 + 1.5 * rng.NextDouble(), EdgeFirst = -1.5 * rng.NextDouble(), OpenTarget = .46, FreeMax = 6, NailLate = .5
            };
            var spec = Spec();
            var c = Generator.TryMake(spec, p, seed, catalog, out why);
            if (c == null) return null;
            var d = c.Draft.Clone();
            var l0 = d.ToLevelData();
            if (l0.Bombs.Count != 1) { why = "bomba yok"; return null; }
            int bomb = l0.Bombs.Keys.First();
            int bodies = l0.Pieces.Count;

            // nails onto the bomb's way out: the cone pieces nearest the bomb first
            foreach (var n in l0.Nails) d.SetNail(l0.Pieces[n.Key][0], 0);
            var board = new Board(l0);
            var cone = ConeByDepth(board, bomb).Where(x => l0.Pieces[x].Length == 1 && x != bomb).ToList();
            if (cone.Count < 1) { why = "bomba yolunda çivilik parça yok"; return null; }
            int[] counters = { (int)Math.Round(.30 * bodies), (int)Math.Round(.40 * bodies) };
            var nailed = new List<int> { cone[0] };
            nailed.Add(cone.Count > 1 ? cone[Math.Min(cone.Count - 1, 1 + (int)(rng.NextDouble() * Math.Min(3, cone.Count - 1)))] : -1);
            for (int i = 0; i < 2; i++) if (nailed[i] >= 0) d.SetNail(l0.Pieces[nailed[i]][0], counters[i]);

            // the fuse: proved dig with the nails + 2
            d.SetBomb(l0.Pieces[bomb][0], 99);
            var m0 = StructureMetrics.Of(d.ToLevelData(), 60_000);
            if (!m0.BombDigs.TryGetValue(bomb, out var dig) || !dig.exact) { why = "kazma yolu ölçülemedi"; return null; }
            d.SetBomb(l0.Pieces[bomb][0], dig.dig + 2);

            // timer share 1.15
            d.SetTimer(60);
            var m = StructureMetrics.Of(d.ToLevelData());
            double s = DifficultyModel.SecondsPerPiece(m.ReadingPerMove, true, false);
            int timer = DifficultyModel.Time(bodies, s, HardDesign.TimeShare);
            d.SetTimer(timer);
            var level = d.ToLevelData();

            // solvable clean; the solution with exit sides turned whenever the board allows
            var sp = SmartPlayer.Play(level);
            if (sp.Verdict != SmartVerdict.Won || sp.Valves > 0) { why = "SmartPlayer: " + sp.Verdict; return null; }
            var moves = new List<Move>();
            var g = new GameSession(level, 1, "hard5", new Mulberry32(1));
            Dir? last = null;
            foreach (var mv in sp.Moves)
            {
                var dirs = TravelRule.ExitDirs(g.Board, mv.Piece);
                var dir = dirs.Contains(mv.Dir) && mv.Dir != last ? mv.Dir : dirs.FirstOrDefault(x => x != last);
                if (!dirs.Contains(dir)) dir = mv.Dir;
                if (g.TryExit(mv.Piece, dir) != CommandResult.Ok) { why = "çözüm oynanmadı"; return null; }
                moves.Add(new Move(mv.Piece, dir)); last = dir;
            }
            if (g.Outcome != Outcome.Win || g.ValveCount > 0) { why = "çözüm kazanmadı"; return null; }
            d.Solution = moves;
            level = d.ToLevelData();
            var check = DesignCheck.Run(level);
            if (!check.FollowsRule || !check.Fits) { why = "tasarım kontrolü: " + check.Verdict; return null; }
            var unfair = Calibration.UnfairReasons(level, spec, m, timer, s);
            if (unfair.Count > 0) { why = "haksız: " + string.Join(", ", unfair); return null; }
            if (timer / (bodies * s) < HardDesign.MinTimeShare) { why = "süre payı"; return null; }

            var f = HardFactors.Of(level);
            var rows = f.Rows();
            why = null;
            return new Result
            {
                Draft = d, Level = level, Factors = f, Seed = seed, Params = p.ToString(),
                // Zeyd: every factor at least as good as the web L40 (2 cross chains, 5 fake-free at once)
                Missed = rows.Count(r => !r.ok) + (f.CrossChains < L40CrossChains ? 1 : 0) + (f.FakeFree < L40FakeFree ? 1 : 0),
                Score = 3 * Math.Max(0, f.Dig) + 4 * f.NarrowStretch - 3 * f.LaneRun - 2 * f.Cascade + 10 * f.BaitShare + 2 * f.FakeFree + 3 * f.CrossChains
            };
        }

        /* The bomb's blockers, nearest first (breadth-first from the bomb). */
        static List<int> ConeByDepth(Board b, int target)
        {
            var order = new List<int>();
            var seen = new HashSet<int> { target };
            var q = new Queue<int>();
            q.Enqueue(target);
            while (q.Count > 0)
            {
                int x = q.Dequeue();
                foreach (var d in DirExt.All)
                {
                    int bl = TravelRule.Limit(b, x, d).Blocker;
                    if (bl >= 0 && seen.Add(bl)) { q.Enqueue(bl); order.Add(bl); }
                }
            }
            return order;
        }
    }
}
