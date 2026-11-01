using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ReverseSolver.Core;
using ReverseSolver.Core.Json;
using ReverseSolver.Editing;
using ReverseSolver.Generation;
using UnityEngine;

namespace ReverseSolver.LevelEditor
{
    /* Veri sistemi Faz 2 (PM_MESAJ_25, docs/VERI_SISTEMI.md 6): the engine half. Tools/veri/*.py do the rest.

       Replay   every recorded attempt (Logs/veri/hamle_girdi.csv, Tools/veri/hazirla.py) on its level version
                (Logs/veri/seviyeler.json, ids "S47@2"); at each exit one row per candidate piece with its features
                -> docs/veri/secimler.csv. Stops an attempt at a booster that changes the board in a way the row
                does not record (scissors, wand) or at a move the board refuses.
       Sim      SET40 v4 design metrics from SmartPlayer / FunSim -> docs/veri/level_sim_v4.csv.
       Bots     the population Monte Carlo: virtual players drawn from botlar_<run>.json play every SET40 v4 level,
                a second attempt after a loss -> docs/veri/tahmin_<run>.csv (level_predictions columns). With
                -rsFolds <dir>: one bots file per held-out level or player (Tools/veri/geri_test.py), each played
                only on its levels -> <dir>/tahmin.csv.

         bash Tools/unity_batch.sh method ReverseSolver.LevelEditor.Veri.Replay
         bash Tools/unity_batch.sh method ReverseSolver.LevelEditor.Veri.Sim
         bash Tools/unity_batch.sh method ReverseSolver.LevelEditor.Veri.Bots -rsBots docs/veri/botlar_v1.json -rsRun v1 [-rsN 1000] */
    public static class Veri
    {
        static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
        static void Log(string s) => Debug.Log("[veri] " + s);
        const string SetPath = "Assets/_Game/Levels/redesign.json";

        // ---- candidate features (shared by the replay and the bots) ------------------------------------

        public sealed class Cand
        {
            public int Piece; public Dir Dir;     // Dir: an exit that keeps the level winnable when there is one
            public bool Right, Bomb, Chain;
            public int SlackDelta, Frees;
            public double Dist;
        }

        public sealed class State
        {
            public List<Cand> Cands; public int MinSlack;   // MinSlack: tightest bomb slack now (99: no bomb on the board)
            public double Pressure => MinSlack >= 99 ? 0 : 1.0 / (1 + Math.Max(0, MinSlack));
        }

        /* Fewest moves to dig a bomb out, on the long-fuse board: depends on the board only, so it is kept per
           (level, present pieces, bomb). A small budget; past it SmartPlayer's direct dig (StructureMetrics). */
        const int DigBudget = 0;   // 0: SmartPlayer's direct dig only (the search was far too slow for the Monte Carlo)
        static readonly ConcurrentDictionary<string, int> Digs = new ConcurrentDictionary<string, int>();

        static string Mask(GameSession s)
        {
            var k = new StringBuilder(s.Level.Id, s.Level.Pieces.Count + 8).Append(':');
            for (int p = 0; p < s.Level.Pieces.Count; p++) k.Append(s.Board.IsPresent(p) ? '1' : '0');
            return k.ToString();
        }

        static int MinSlack(GameSession s, GameSession lng)
        {
            int min = 99; string mask = null;
            foreach (var b in s.Bombs)
            {
                if (!s.Board.IsPresent(b.Key)) continue;
                mask ??= Mask(lng);
                int d = Digs.GetOrAdd(mask + "#" + b.Key, _ => StructureMetrics.DigNow(lng, b.Key, DigBudget));
                min = Math.Min(min, d < 0 ? -99 : b.Value - d);
            }
            return min >= 99 ? 99 : Math.Max(-5, min);
        }

        /* The candidates at this state: every free, unnailed piece. lng: the same state with long fuses (dig lengths).
           Right = after it every bomb still on the board keeps a slack >= 0 (fuse - fewest moves to dig it out). */
        public static State Features(GameSession s, GameSession lng)
        {
            var b = s.Board;
            bool bombs = s.Bombs.Any(x => b.IsPresent(x.Key));
            int before = bombs ? MinSlack(s, lng) : 99;
            var free = TravelRule.FreePieces(b).Where(p => s.NailAt(p) <= 0).ToList();
            var list = new List<Cand>();
            foreach (int p in free)
            {
                var dirs = TravelRule.ExitDirs(b, p);
                if (dirs.Count == 0) continue;
                // the board after a removal does not depend on the way out, so any exit stands for the piece
                var c = new Cand { Piece = p, Dir = dirs[0], Bomb = s.Bombs.ContainsKey(p), Chain = s.Level.Pieces[p].Length > 1, Right = true };
                var a = s.Clone(); a.TryExit(p, c.Dir);
                if (bombs)
                {
                    var la = lng.Clone(); la.TryExit(p, c.Dir);
                    int after = a.IsOver && a.Outcome == Outcome.Bomb ? -5 : MinSlack(a, la);
                    c.SlackDelta = Math.Max(-5, Math.Min(5, Math.Min(after, before + 5) - before));
                    // right: every bomb left can still be dug out in time (each on its own: two bombs sharing a way
                    // are not checked together; SmartPlayer per candidate was far too slow for the Monte Carlo)
                    c.Right = after >= 0;
                }
                c.Frees = TravelRule.FreePieces(a.Board).Count(q => a.NailAt(q) <= 0) - (free.Count - 1);
                list.Add(c);
            }
            return new State { Cands = list, MinSlack = before };
        }

        static double Dist(LevelData l, int a, int b)
        {
            if (a < 0 || b < 0) return 0;
            var x = l.Pieces[a][0]; var y = l.Pieces[b][0];
            return (Math.Abs(x.X - y.X) + Math.Abs(x.Y - y.Y)) / (double)(l.Width + l.Height);
        }

        static Dictionary<string, LevelData> Book(string path) =>
            LevelParser.Parse(File.ReadAllText(path)).Levels.ToDictionary(l => l.Id);

        static GameSession Fresh(LevelData l, int attempt = 1) => new GameSession(l, attempt, "veri", new Mulberry32(1)) { ClockOnFirstMove = false };

        // ---- Replay -----------------------------------------------------------------------------------

        public static void Replay()
        {
            string input = ZeydSet.Arg("-rsIn", "Logs/veri/hamle_girdi.csv");
            string outp = ZeydSet.Arg("-rsOut", "docs/veri/secimler.csv");
            var book = Book(ZeydSet.Arg("-rsLevels", "Logs/veri/seviyeler.json"));
            var lines = File.ReadAllLines(input);
            var head = lines[0].Split(',');
            int Col(string n) => Array.IndexOf(head, n);
            int cRow = Col("row_id"), cKey = Col("player_key"), cLv = Col("level_key"), cAtt = Col("attempt"), cNo = Col("move_no"), cKind = Col("kind"),
                cPiece = Col("piece"), cDir = Col("dir"), cT = Col("t_ms"), cThink = Col("think_ms"), cLeft = Col("time_left"), cPlat = Col("platform");
            var attempts = lines.Skip(1).Select(x => x.Split(',')).GroupBy(f => f[cRow]).ToList();
            var outRows = new ConcurrentDictionary<string, string>();
            int stopped = 0, missing = 0, finished = 0;
            System.Threading.Tasks.Parallel.ForEach(attempts, new System.Threading.Tasks.ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 1) }, g =>
            {
                var rows = g.OrderBy(f => int.Parse(f[cNo], Inv)).ToList();
                var f0 = rows[0];
                if (!book.TryGetValue(f0[cLv], out var l)) { System.Threading.Interlocked.Increment(ref missing); return; }
                var s = Fresh(l); var lng = Fresh(ZeydAnalysis.Long(l).Level);
                var sb = new StringBuilder();
                int last = -1, step = 0, prevT = 0;
                foreach (var f in rows)
                {
                    string kind = f[cKind];
                    int piece = int.Parse(f[cPiece], Inv);
                    int t = int.Parse(f[cT], Inv);
                    if (kind == "touch" || kind == "jam") { if (piece >= 0) last = piece; continue; }
                    if (kind == "scissors" || kind == "wand") { System.Threading.Interlocked.Increment(ref stopped); break; }
                    if (kind == "hammer") { s.UseHammer(piece); lng.UseHammer(piece); last = piece; prevT = t; if (s.IsOver) break; continue; }
                    if (kind != "exit") continue;
                    if (!Enum.TryParse<Dir>(f[cDir], out var dir)) break;
                    var st = Features(s, lng);
                    if (!st.Cands.Any(c => c.Piece == piece)) { System.Threading.Interlocked.Increment(ref stopped); break; }
                    int nRight = st.Cands.Count(c => c.Right);
                    foreach (var c in st.Cands)
                        sb.Append(string.Join(",", f[cKey], g.Key, f0[cLv], f[cAtt], step.ToString(Inv), st.Cands.Count.ToString(Inv), nRight.ToString(Inv),
                            st.MinSlack.ToString(Inv), st.Pressure.ToString("0.###", Inv), c.Piece.ToString(Inv), c.Right ? "1" : "0", c.SlackDelta.ToString(Inv),
                            c.Frees.ToString(Inv), c.Bomb ? "1" : "0", c.Chain ? "1" : "0", Dist(l, c.Piece, last).ToString("0.###", Inv),
                            c.Piece == piece ? "1" : "0", f[cThink], (t - prevT).ToString(Inv), f[cLeft], f[cPlat])).Append('\n');
                    if (s.TryExit(piece, dir) != CommandResult.Ok) { System.Threading.Interlocked.Increment(ref stopped); break; }
                    lng.TryExit(piece, dir);
                    last = piece; prevT = t; step++;
                    if (s.IsOver) break;
                }
                outRows[g.Key] = sb.ToString();
                int done = System.Threading.Interlocked.Increment(ref finished);
                if (done % 25 == 0) Log($"{done}/{attempts.Count} deneme");
            });
            Directory.CreateDirectory(Path.GetDirectoryName(outp));
            File.WriteAllText(outp, "player_key,row_id,level_key,attempt,step,n_opts,n_right,min_slack,pressure,cand,right,slack_delta,frees,bomb,chain,dist,chosen,think_ms,dt_ms,time_left,platform\n" +
                string.Concat(attempts.Select(g => outRows.TryGetValue(g.Key, out var r) ? r : "")), new UTF8Encoding(false));
            Log($"yeniden oynatÄ±ldÄ±: {attempts.Count} deneme, durdurulan {stopped}, bÃ¶lÃ¼mÃ¼ bulunmayan {missing} â†’ {outp}");
        }

        // ---- Sim: design metrics of SET40 v4 ------------------------------------------------------------

        public static void Sim()
        {
            string outp = ZeydSet.Arg("-rsOut", "docs/veri/level_sim_v4.csv");
            var set = LevelParser.Parse(File.ReadAllText(SetPath)).Levels;
            var rows = new ConcurrentDictionary<string, string>();
            System.Threading.Tasks.Parallel.ForEach(set, new System.Threading.Tasks.ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 1) }, l =>
            {
                var e = new RedesignStore.Entry { Draft = LevelDraft.FromLevel(l), Slot = l.Number };
                RedesignStore.HardScores(e, l);
                string Sc(string k) => e.Scores.TryGetValue(k, out var v) && !double.IsNaN(v) ? v.ToString("0.##", Inv) : "";   // empty: no bomb
                // traps along SmartPlayer's path: candidates that leave the level unwinnable
                var sp = SmartPlayer.Play(l);
                var s = Fresh(l); var lng = Fresh(ZeydAnalysis.Long(l).Level);
                int traps0 = -1, trapSteps = 0, trapTotal = 0, steps = 0;
                foreach (var m in sp.Moves)
                {
                    var st = Features(s, lng);
                    int wrong = st.Cands.Count(c => !c.Right);
                    if (traps0 < 0) traps0 = wrong;
                    trapTotal += wrong; if (wrong > 0) trapSteps++; steps++;
                    s.TryExit(m.Piece, m.Dir); lng.TryExit(m.Piece, m.Dir);
                    if (s.IsOver) break;
                }
                var mc = MonteCarlo.Run(l, 200);
                var fun = FunSim.Run(l, Genre.Orta);
                rows[l.Id] = string.Join(",", l.Id, l.Version.ToString(Inv), l.Number.ToString(Inv), Sc("m1Acilis"), Sc("m2Tavan"), Sc("fitilPay"), Math.Max(0, traps0).ToString(Inv), trapSteps.ToString(Inv), (steps == 0 ? 0 : (double)trapTotal / steps).ToString("0.##", Inv),
                    sp.Moves.Count.ToString(Inv), mc.WinRate.ToString("0.###", Inv), fun.FirstWinRate.ToString("0.###", Inv));
            });
            File.WriteAllText(outp, "level_id,level_version,slot,k0_acilis,m2_tavan,fitil_pay_min,tuzak_ilk_adim,tuzakli_adim,tuzak_adim_basi,cozum_hamle,rastgele_kazanma,funsim_ilk\n" +
                string.Concat(set.Select(l => rows[l.Id] + "\n")), new UTF8Encoding(false));
            Log($"yazÄ±ldÄ±: {outp} ({set.Count} bÃ¶lÃ¼m)");
        }

        // ---- Bots: population Monte Carlo ------------------------------------------------------------------

        public sealed class BotParams
        {
            public double Right, Bomb, Slack, Free, Dist, Chain, Learn;      // choice utilities (conditional logit)
            public double Mu, MuOpts, MuPress, MuLater, Sigma;               // log seconds per exit
            public double JamRate;
        }

        static readonly string[] Names = { "right", "bomb", "slack", "free", "dist", "chain", "learn", "mu", "mu_opts", "mu_press", "mu_later", "sigma", "jam_rate" };

        static BotParams FromVector(double[] v) => new BotParams
        {
            Right = v[0], Bomb = v[1], Slack = v[2], Free = v[3], Dist = v[4], Chain = v[5], Learn = v[6],
            Mu = v[7], MuOpts = v[8], MuPress = v[9], MuLater = v[10], Sigma = Math.Max(.05, v[11]), JamRate = Math.Max(0, v[12]),
        };

        /* Reads the population block of a botlar json: mean and the Cholesky factor of the covariance, in Names order. */
        static (double[] mean, double[,] chol) Population(string path)
        {
            var root = JsonReader.Parse(File.ReadAllText(path));
            root.TryGet("population", out var pop);
            pop.TryGet("mean", out var mean); pop.TryGet("cov", out var cov);
            int n = Names.Length;
            var m = new double[n]; var c = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                mean.TryGet(Names[i], out var mv); m[i] = mv.AsNumber;
                for (int j = 0; j < n; j++) c[i, j] = cov[i][j].AsNumber;
            }
            var L = new double[n, n];   // Cholesky with a small ridge
            for (int i = 0; i < n; i++)
                for (int j = 0; j <= i; j++)
                {
                    double sum = c[i, j] + (i == j ? 1e-9 : 0);
                    for (int k = 0; k < j; k++) sum -= L[i, k] * L[j, k];
                    L[i, j] = i == j ? Math.Sqrt(Math.Max(sum, 1e-12)) : sum / L[j, j];
                }
            return (m, L);
        }

        static double Normal(Mulberry32 r) { double u = Math.Max(1e-12, r.NextDouble()), v = r.NextDouble(); return Math.Sqrt(-2 * Math.Log(u)) * Math.Cos(2 * Math.PI * v); }

        static BotParams Draw((double[] mean, double[,] chol) pop, Mulberry32 r)
        {
            int n = pop.mean.Length; var z = new double[n]; var v = new double[n];
            for (int i = 0; i < n; i++) z[i] = Normal(r);
            for (int i = 0; i < n; i++) { v[i] = pop.mean[i]; for (int k = 0; k <= i; k++) v[i] += pop.chol[i, k] * z[k]; }
            return FromVector(v);
        }

        /* Features per board (present-piece set) and level: within one attempt the fuses and nails follow from the
           number of moves, so the set is the whole state (no boosters in the simulation). */
        sealed class Cache
        {
            readonly ConcurrentDictionary<string, State> _d = new ConcurrentDictionary<string, State>();
            public State Get(GameSession s, GameSession lng)
            {
                return _d.GetOrAdd(Mask(s), _ => Features(s, lng));
            }
        }

        sealed class Outcome1 { public Outcome Result; public double Dur, LeftFrac; public int Jams; }

        static Outcome1 PlayOnce(LevelData l, LevelData lngLevel, BotParams b, int attempt, Cache cache, Mulberry32 r)
        {
            var s = Fresh(l, attempt); var lng = Fresh(lngLevel, attempt);
            int last = -1, jams = 0; double dur = 0;
            bool later = attempt > 1, first = true;
            while (!s.IsOver)
            {
                var st = cache.Get(s, lng);
                if (st.Cands.Count == 0) { s.Tick(s.TimeLeft + 1); break; }   // nothing can leave: the clock runs out
                var u = st.Cands.Select(c => b.Right * (c.Right ? 1 : 0) + b.Learn * (later && c.Right ? 1 : 0) + b.Bomb * (c.Bomb ? 1 : 0) + b.Slack * c.SlackDelta
                                             + b.Free * c.Frees + b.Dist * Dist(l, c.Piece, last) + b.Chain * (c.Chain ? 1 : 0)).ToArray();
                double max = u.Max(), sum = 0;
                for (int i = 0; i < u.Length; i++) { u[i] = Math.Exp(u[i] - max); sum += u[i]; }
                double x = r.NextDouble() * sum; int pick = 0;
                while (pick < u.Length - 1 && (x -= u[pick]) > 0) pick++;
                var cand = st.Cands[pick];
                double secs = Math.Exp(b.Mu + b.MuOpts * Math.Log(st.Cands.Count) + b.MuPress * st.Pressure + b.MuLater * (later ? 1 : 0) + b.Sigma * Normal(r));
                while (r.NextDouble() < b.JamRate / (1 + b.JamRate)) jams++;   // geometric, mean = JamRate per exit
                if (!first) { s.Tick(secs); dur += secs; }                     // the clock starts with the first move
                first = false;
                if (s.IsOver) break;
                s.TryExit(cand.Piece, cand.Dir); lng.TryExit(cand.Piece, cand.Dir);
                last = cand.Piece;
            }
            return new Outcome1 { Result = s.Outcome, Dur = dur, LeftFrac = (double)s.TimeLeft / Math.Max(1, l.TimerSeconds), Jams = jams };
        }

        static string Predict(LevelData l, (double[] mean, double[,] chol) pop, int n, int groups, uint seed, Cache cache)
        {
            var lngLevel = ZeydAnalysis.Long(l).Level;
            var r = new Mulberry32(seed);
            int win1 = 0, bomb1 = 0, time1 = 0, attemptsSum = 0, jams = 0; double dur = 0, left = 0; int leftN = 0;
            var groupWin = new double[groups];
            for (int i = 0; i < n; i++)
            {
                var b = Draw(pop, r);
                var o = PlayOnce(l, lngLevel, b, 1, cache, r);
                bool won = o.Result == Outcome.Win;
                if (won) { win1++; groupWin[i % groups]++; left += o.LeftFrac; leftN++; }
                else if (o.Result == Outcome.Bomb) bomb1++; else time1++;
                dur += o.Dur; jams += o.Jams;
                int att = 1;
                if (!won) { att = 2; PlayOnce(l, lngLevel, b, 2, cache, r); }
                attemptsSum += att;
            }
            var g = groupWin.Select(w => w / (n / (double)groups)).OrderBy(x => x).ToArray();
            double Q(double q) => g[Math.Min(g.Length - 1, (int)Math.Floor(q * (g.Length - 1) + .5))];
            double p = (double)win1 / n;
            double Rating(double w) => Math.Max(1, Math.Min(10, 1 + (.97 - w) / .075));   // A1 turned round
            return string.Join(",", l.Id, l.Version.ToString(Inv), l.Number.ToString(Inv), n.ToString(Inv), p.ToString("0.###", Inv), Q(.05).ToString("0.###", Inv), Q(.95).ToString("0.###", Inv),
                ((double)bomb1 / n).ToString("0.###", Inv), ((double)time1 / n).ToString("0.###", Inv), ((double)attemptsSum / n).ToString("0.###", Inv),
                (dur / n).ToString("0.#", Inv), (leftN == 0 ? 0 : left / leftN).ToString("0.###", Inv), ((double)jams / n).ToString("0.##", Inv),
                Rating(p).ToString("0.#", Inv), Rating(Q(.95)).ToString("0.#", Inv), Rating(Q(.05)).ToString("0.#", Inv));
        }

        const string PredHead = "level_id,level_version,slot,n_sims,p_win1,p_win1_lo,p_win1_hi,p_fail_bomb,p_fail_time,mean_attempts,mean_dur_s,mean_left_frac,mean_jams,pred_rating,pred_rating_lo,pred_rating_hi";

        public static void Bots()
        {
            string run = ZeydSet.Arg("-rsRun", "v1");
            int n = int.Parse(ZeydSet.Arg("-rsN", "1000"), Inv);
            string folds = ZeydSet.Arg("-rsFolds", null);
            var t0 = DateTime.Now;
            var po = new System.Threading.Tasks.ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 1) };
            if (folds == null)
            {
                var set = LevelParser.Parse(File.ReadAllText(ZeydSet.Arg("-rsSet", SetPath))).Levels;
                var pop = Population(ZeydSet.Arg("-rsBots", $"docs/veri/botlar_{run}.json"));
                var rows = new ConcurrentDictionary<string, string>();
                System.Threading.Tasks.Parallel.ForEach(set, po, l => { var t1 = DateTime.Now; rows[l.Id] = Predict(l, pop, n, 20, 20261101u + (uint)l.Number, new Cache()); Log($"{l.Number} {l.Id}: {(DateTime.Now - t1).TotalSeconds:0} s"); });
                string outp = ZeydSet.Arg("-rsOut", $"docs/veri/tahmin_{run}.csv");
                File.WriteAllText(outp, PredHead + "\n" + string.Concat(set.Select(l => rows[l.Id] + "\n")), new UTF8Encoding(false));
                Log($"yazÄ±ldÄ±: {outp} ({set.Count} bÃ¶lÃ¼m Ã— {n} sanal oyuncu, {(DateTime.Now - t0).TotalSeconds:0} s)");
                return;
            }
            // folds: <dir>/fold_*.json, each {"levels": ["S47@2", ...], "population": ...}; levels from Logs/veri/seviyeler.json
            var book = Book(ZeydSet.Arg("-rsLevels", "Logs/veri/seviyeler.json"));
            var jobs = new List<(string fold, LevelData l, (double[], double[,]) pop)>();
            foreach (var f in Directory.GetFiles(folds, "fold_*.json").OrderBy(x => x))
            {
                var root = JsonReader.Parse(File.ReadAllText(f));
                root.TryGet("levels", out var lv);
                var pop = Population(f);
                foreach (var x in lv.Items) if (book.TryGetValue(x.AsString, out var l)) jobs.Add((Path.GetFileNameWithoutExtension(f), l, pop));
            }
            var caches = new ConcurrentDictionary<string, Cache>();
            var res = new ConcurrentDictionary<int, string>();
            System.Threading.Tasks.Parallel.ForEach(Enumerable.Range(0, jobs.Count), po, i =>
            {
                var j = jobs[i];
                res[i] = j.fold + "," + j.l.Id + "," + Predict(j.l, j.pop, n, 10, 777u + (uint)i, caches.GetOrAdd(j.l.Id, _ => new Cache()));
            });
            File.WriteAllText(Path.Combine(folds, "tahmin.csv"), "fold,level_key," + PredHead + "\n" + string.Concat(Enumerable.Range(0, jobs.Count).Select(i => res[i] + "\n")), new UTF8Encoding(false));
            Log($"katlar: {jobs.Count} iÅŸ, {(DateTime.Now - t0).TotalSeconds:0} s â†’ {folds}/tahmin.csv");
        }
    }
}
