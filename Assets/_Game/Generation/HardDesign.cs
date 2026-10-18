using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReverseSolver.Core;
using ReverseSolver.Editing;

namespace ReverseSolver.Generation
{
    /* Zeyd's single hardest-but-fair Çok zor level (PRODUCT.md karar 2026-10-18 (3)): the
       "derin bomba" recipe with his conditions, measured by the strongest player (SmartPlayer's
       order, 0.5 s a piece for a fast expert), not by the fun model.

         6×7, 36-40 bodies, open seams 42-46 %, at most 5 pieces free on the first move;
         the bomb on a core cell (each edge >= 3 cells away), shortest dig >= 8, fuse = dig + 2;
         two remote chains whose ends sit on opposite edges and that can leave only after the bomb;
         two nails each crossing the way out of >= 6 pieces, counters 30 % and 40 % of the bodies;
         at most 3 exits in a row the same way along the solution; time share 1.15 (>= 1.1);
         solvable, no valve, the stored solution wins, the unfair list clean. */
    public static class HardDesign
    {
        public const int FuseOverDig = 3;            // PM 2026-10-18 (4), A: Zeyd's dig + 2 -> dig + 3
        public const double ExpertSecondsPerPiece = 0.5, TimeShare = 1.15, MinTimeShare = 1.1;
        /* Zeyd's range is 42-46 %; SmartPlayer-clean 6×7 boards stop at 48 % (measured 2026-10-18), so 47-48 is let in and reported. */
        public static int MaxOpen = 48;
        public const int MaxRun = 3, MinDig = 8, MinNailCuts = 6, MaxFreeAtStart = 6;   // PM 2026-10-18 (4), A: Zeyd's 4-5 -> 6

        public sealed class Result
        {
            public LevelDraft Draft;
            public LevelData Level;
            public StructureMetrics Metrics;
            public int Dig, Fuse, MaxRunSameWay, Bodies, OpenPercent, ExpertSeconds, Timer;
            public double Share, Score;
            public int[] NailCuts = Array.Empty<int>(), NailCounters = Array.Empty<int>();
            public Cell Core;
            public uint Seed;
            public string Params;
            /* Conditions missed (fairness never is): text "tarif X, gerçek Y" and a cost; 0 = every condition held. */
            public readonly List<(string text, double cost)> Misses = new List<(string, double)>();
            public double Miss => Misses.Sum(m => m.cost);

            public string Report()
            {
                var m = Metrics;
                var sb = new StringBuilder();
                sb.AppendLine($"tohum {Seed}, düğmeler: {Params}");
                sb.AppendLine($"tahta {Level.Width}×{Level.Height}, gövde {Bodies}, açık dikiş %{OpenPercent}, ilk hamlede serbest {m.FreeAtStart}, en az seçenek {m.FewestChoices}, darboğaz {m.Bottlenecks10}, hamle başına okuma {m.ReadingPerMove:0.00}");
                sb.AppendLine($"bomba ({Core.X},{Core.Y}) — kenarlara uzaklık {Core.X + 1}/{Level.Width - Core.X}/{Core.Y + 1}/{Level.Height - Core.Y}; en kısa kazma yolu {Dig}, fitil {Fuse} (tarif yol + 2, PM A kararıyla yol + {FuseOverDig})");
                sb.AppendLine($"zincir: satır ({0},{Core.Y})–({Level.Width - 1},{Core.Y}) bomba satırında, yalnız yatay kayar: bombadan önce çıkamaz (her durumda); ikinci zincir başka satırda, yine karşı kenarlar arası (geç kilit): çözüm yolunda bombadan sonra çıkıyor");
                sb.AppendLine($"çivi: sayaçlar {string.Join(", ", NailCounters)} (gövdenin %30 / %40'ı), kestiği yol {string.Join(", ", NailCuts)} parça");
                sb.AppendLine($"çözüm (SmartPlayer): {Level.Solution.Count} hamle, aynı yöne art arda en fazla {MaxRunSameWay}");
                sb.AppendLine($"süre {Timer} sn (pay {Share:0.00}); hızlı uzman (parça başına {ExpertSecondsPerPiece} sn): ~{ExpertSeconds} sn");
                sb.AppendLine(Misses.Count == 0 ? "kaçırılan şart: yok" : "kaçırılan şartlar: " + string.Join(" · ", Misses.Select(m => m.text)));
                return sb.ToString();
            }
        }

        public static SlotSpec Spec()
        {
            var s = SlotSpec.For(40, Genre.CokZor);
            s.Boards = new[] { (6, 7) };
            s.MinBodies = 36; s.MaxBodies = 40;
            s.MinNails = s.MaxNails = 0; s.MinBombs = s.MaxBombs = 0;
            s.MinSeals = s.MaxSeals = 0; s.MinChains = s.MaxChains = 2; s.MinRemote = s.MaxRemote = 2;
            return s;
        }

        /* One candidate seed: null with the reason when a condition fails. */
        public static Result Try(uint seed, Catalog catalog, out string why)
        {
            var rng = new Mulberry32(seed * 2246822519u + 7u);
            var p = new GenParams
            {
                CrossChains = true, Interlock = 2 + 6 * rng.NextDouble(), Knot = 2 * rng.NextDouble(), Ease = .5 * rng.NextDouble(),
                Sweep = -1 + 2 * rng.NextDouble(), EdgeFirst = -1.5 * rng.NextDouble()
            };
            var spec = Spec();
            var c = Generator.TryMake(spec, p, seed, catalog, out why);
            if (c == null) return null;
            if (c.Core == null) { why = "no core"; return null; }
            var core = c.Core.Value;
            var d = c.Draft.Clone();
            int bodies = c.Level.Pieces.Count;
            Relock(d, rng);
            int open = LevelStats.Of(d.ToLevelData()).OpenPercent;
            var miss = new List<(string, double)>();
            if (open < 42 || open > 46) miss.Add(($"açık dikiş: tarif %42–46, gerçek %{open}", open > MaxOpen ? open - 46 : 0));

            // the bomb on the core: its shortest dig with a long fuse, then fuse = dig + 2
            d.SetBomb(core, 99);
            var m0 = StructureMetrics.Of(d.ToLevelData(), 60_000);    // deep bomb: prove the shortest dig
            int bombPiece = PieceAt(d.ToLevelData(), core);
            if (!m0.BombDigs.TryGetValue(bombPiece, out var dig) || !dig.exact) { why = "kazma yolu ölçülemedi"; return null; }
            if (dig.dig < MinDig) miss.Add(($"en kısa kazma yolu: tarif ≥ {MinDig}, gerçek {dig.dig}", 2.0 * (MinDig - dig.dig)));
            d.SetBomb(core, dig.dig + FuseOverDig);

            // nails over the knot: the single pieces that cross the most ways out
            var lv = d.ToLevelData();
            int[] counters = { (int)Math.Round(.30 * bodies), (int)Math.Round(.40 * bodies) };
            var singles = Enumerable.Range(0, bodies).Where(i => lv.Pieces[i].Length == 1 && i != bombPiece).ToList();
            var cuts = singles.ToDictionary(i => i, i => Cuts(lv, i));
            var picks = singles.OrderByDescending(i => cuts[i]).ThenBy(_ => rng.NextDouble()).Take(4).ToList();
            Result best = null;
            for (int a = 0; a < picks.Count; a++)
                for (int b = a + 1; b < picks.Count; b++)
                {
                    var dd = d.Clone();
                    dd.SetNail(lv.Pieces[picks[a]][0], counters[0]);
                    dd.SetNail(lv.Pieces[picks[b]][0], counters[1]);
                    var r = Finish(dd, core, bodies, open, new[] { cuts[picks[a]], cuts[picks[b]] }, counters, spec, out string w);
                    if (r == null) { why = w; continue; }
                    r.Seed = seed; r.Params = p.ToString();
                    r.Misses.AddRange(miss);
                    foreach (var cut in r.NailCuts) if (cut < MinNailCuts) r.Misses.Add(($"çivinin kestiği yol: tarif ≥ {MinNailCuts}, gerçek {cut}", .5 * (MinNailCuts - cut)));
                    if (best == null || Better(r, best)) best = r;
                }
            if (best != null) why = null;
            return best;
        }

        /* Every condition held beats any miss; then fewer misses; then the harder board. */
        public static bool Better(Result a, Result b) => a.Miss != b.Miss ? a.Miss < b.Miss : a.Score > b.Score;

        static Result Finish(LevelDraft d, Cell core, int bodies, int open, int[] cuts, int[] counters, SlotSpec spec, out string why)
        {
            why = null;
            d.SetTimer(60);
            var level = d.ToLevelData();
            var m = StructureMetrics.Of(level);
            var miss = new List<(string, double)>();
            if (m.FreeAtStart > 5) miss.Add(($"ilk hamlede serbest: tarif ≤ 4–5, gerçek {m.FreeAtStart}", 1.5 * Math.Max(0, m.FreeAtStart - MaxFreeAtStart)));
            double s = DifficultyModel.SecondsPerPiece(m.ReadingPerMove, true, false);
            int timer = DifficultyModel.Time(bodies, s, TimeShare);
            d.SetTimer(timer);
            level = d.ToLevelData();
            double share = timer / (bodies * s);
            if (share < MinTimeShare) { why = $"süre payı {share:0.00}"; return null; }

            var sp = SmartPlayer.Play(level);
            if (sp.Verdict != SmartVerdict.Won || sp.Valves > 0) { why = "SmartPlayer: " + sp.Verdict; return null; }
            int bombPiece = PieceAt(level, core);
            var chains = Enumerable.Range(0, bodies).Where(i => level.Pieces[i].Length == 2).ToList();
            int rowChain = chains.First(i => level.Pieces[i].All(c => c.Y == core.Y));

            // the solution with exit sides turned as often as the board allows; chains only after the bomb
            var moves = new List<Move>();
            var g = new GameSession(level, 1, "hard", new Mulberry32(1));
            Dir? last = null; int run = 0, maxRun = 0; bool bombOut = false, rowEarly = false, chainEarly = false;
            foreach (var mv in sp.Moves)
            {
                if (!bombOut)
                    if (TravelRule.ExitDirs(g.Board, rowChain).Count > 0 && !rowEarly) { rowEarly = true; miss.Add(("satır zinciri: tarif bombadan sonra, gerçek bombadan önce çıkabiliyor", 3)); }
                var dirs = TravelRule.ExitDirs(g.Board, mv.Piece);
                var dir = dirs.Contains(mv.Dir) && mv.Dir != last ? mv.Dir : dirs.FirstOrDefault(x => x != last);
                if (!dirs.Contains(dir)) dir = mv.Dir;
                if (g.TryExit(mv.Piece, dir) != CommandResult.Ok) { why = "çözüm oynanmadı"; return null; }
                run = dir == last ? run + 1 : 1; last = dir; maxRun = Math.Max(maxRun, run);
                moves.Add(new Move(mv.Piece, dir));
                if (mv.Piece == bombPiece) bombOut = true;
                if (chains.Contains(mv.Piece) && !bombOut && !chainEarly) { chainEarly = true; miss.Add(("zincir: tarif bombadan sonra, gerçek çözümde bombadan önce çıkıyor", 2)); }
            }
            if (g.Outcome != Outcome.Win || g.ValveCount > 0) { why = "çözüm kazanmadı"; return null; }
            if (maxRun > MaxRun) miss.Add(($"aynı yöne art arda: tarif ≤ {MaxRun}, gerçek {maxRun}", maxRun - MaxRun));
            d.Solution = moves;
            level = d.ToLevelData();

            var check = DesignCheck.Run(level);
            if (!check.FollowsRule || !check.Fits) { why = "tasarım kontrolü: " + check.Verdict; return null; }
            var unfair = Calibration.UnfairReasons(level, spec, m, timer, s);
            if (unfair.Count > 0) { why = "haksız: " + string.Join(", ", unfair); return null; }
            int dig = level.Bombs[bombPiece] - FuseOverDig;             // fuse = proved shortest dig + 3

            var res = new Result
            {
                Draft = d, Level = level, Metrics = m, Core = core, Dig = dig, Fuse = level.Bombs[bombPiece], MaxRunSameWay = maxRun,
                Bodies = bodies, OpenPercent = open, Timer = timer, Share = share, NailCuts = cuts, NailCounters = counters,
                ExpertSeconds = (int)Math.Ceiling(bodies * ExpertSecondsPerPiece),
                // harder first: a long dig, few choices, more bottlenecks and reading
                Score = 3 * dig - 4 * m.FreeAtStart + 3 * m.Bottlenecks10 + 5 * m.ReadingPerMove - 2 * m.FewestChoices
            };
            res.Misses.AddRange(miss);
            return res;
        }

        /* Lock flat seams again while SmartPlayer still solves the board clean (no bomb or nail yet):
           first the seams of the pieces free at the start, until at most 46 % are open. */
        static void Relock(LevelDraft d, IRandom rng)
        {
            var l = d.ToLevelData();
            var seams = new List<(int x, int y, bool v)>();
            for (int y = 0; y < l.Height; y++) for (int x = 1; x < l.Width; x++) if (l.VJoint(x, y) == 0) seams.Add((x, y, true));
            for (int y = 1; y < l.Height; y++) for (int x = 0; x < l.Width; x++) if (l.HJoint(x, y) == 0) seams.Add((x, y, false));
            for (int i = seams.Count - 1; i > 0; i--) { int k = (int)(rng.NextDouble() * (i + 1)); (seams[i], seams[k]) = (seams[k], seams[i]); }
            var free = new HashSet<Cell>();
            foreach (var p in TravelRule.FreePieces(new Board(l))) foreach (var c in l.Pieces[p]) free.Add(c);
            bool Near((int x, int y, bool v) j) => free.Contains(new Cell(j.x, j.y)) || free.Contains(j.v ? new Cell(j.x - 1, j.y) : new Cell(j.x, j.y - 1));
            seams = seams.OrderBy(j => Near(j) ? 0 : 1).ToList();
            for (int pass = 0, before = -1; pass < 3; pass++)
            {
                int now = LevelStats.Of(d.ToLevelData()).OpenPercent;
                if (now <= 45 || now == before) break;
                before = now;
            foreach (var j in seams)
            {
                if (LevelStats.Of(d.ToLevelData()).OpenPercent <= 45) break;
                var cur = d.ToLevelData();
                if ((j.v ? cur.VJoint(j.x, j.y) : cur.HJoint(j.x, j.y)) != 0) continue;
                int first = rng.NextDouble() < .5 ? 1 : -1;
                foreach (int o in new[] { first, -first })
                {
                    d.SetJoint(j.x, j.y, j.v, o);
                    var r = SmartPlayer.Play(d.ToLevelData());
                    if (r.Verdict == SmartVerdict.Won && r.Valves == 0) goto next;
                    d.SetJoint(j.x, j.y, j.v, 0);
                }
                next:;
            }
            }
        }

        static int PieceAt(LevelData l, Cell c)
        {
            for (int i = 0; i < l.Pieces.Count; i++) if (l.Pieces[i].Contains(c)) return i;
            return -1;
        }

        /* How many other pieces have piece q's cell on a straight way out to some edge. */
        static int Cuts(LevelData l, int q)
        {
            var qc = l.Pieces[q][0];
            int n = 0;
            for (int i = 0; i < l.Pieces.Count; i++)
            {
                if (i == q) continue;
                bool hit = false;
                foreach (var d in DirExt.All)
                {
                    foreach (var c in l.Pieces[i])
                        for (var x = c.Step(d); l.OnBoard(x); x = x.Step(d))
                            if (x.Equals(qc)) { hit = true; break; }
                    if (hit) break;
                }
                if (hit) n++;
            }
            return n;
        }
    }
}
