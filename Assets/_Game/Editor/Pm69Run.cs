using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ReverseSolver.Core;
using ReverseSolver.Generation;
using UnityEngine;

namespace ReverseSolver.EditorTools
{
    /* PM_69: difficulty features v2 for the whole level pool + pt1 replay. Measurement only.
         bash Tools/unity_batch.sh method ReverseSolver.EditorTools.Pm69Run.Features [games]
         bash Tools/unity_batch.sh method ReverseSolver.EditorTools.Pm69Run.Replay
       Inputs: Logs/pm69/moves.tsv, plays.tsv (node Tools/pm69_fetch.js + Tools/pm69_prep.js).
       Outputs: docs/analiz/v2/level_features_v2.csv, replay_pt1.csv, replay_pt1_steps.csv. */
    public static class Pm69Run
    {
        const string Out = "docs/analiz/v2/";
        static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

        static string F(double v) => double.IsNaN(v) || double.IsInfinity(v) ? "" : Math.Round(v, 3).ToString(Inv);
        static string F(double? v) => v.HasValue ? F(v.Value) : "";
        static string L(IEnumerable<string> v) => string.Join(";", v);

        static string Arg(int i, string def)
        {
            var a = Environment.GetCommandLineArgs();
            int k = Array.IndexOf(a, "-executeMethod");
            return k >= 0 && a.Length > k + 2 + i && !a[k + 2 + i].StartsWith("-") ? a[k + 2 + i] : def;
        }

        public static List<(string set, LevelData level)> Pool()
        {
            var pool = new List<(string, LevelData)>();
            LevelSet Load(string p) => LevelParser.Parse(File.ReadAllText(p));
            var v4 = Load("Assets/_Game/Levels/redesign.json");
            foreach (var l in v4.Levels) pool.Add(("redesign-v4", l));
            // pt1 played Z21 v1 once: the file before ac50c359's successor
            const string old = "Logs/pm69/redesign_ac50c359.json";
            if (File.Exists(old))
                foreach (var l in Load(old).Levels)
                    if (!v4.Levels.Any(x => x.Id == l.Id && x.Version == l.Version)) pool.Add(("redesign-v4", l));
            foreach (var l in P21Onboarding.TuneSecondTen(P21Onboarding.SpaceObstacleLessons(v4)).Levels) pool.Add(("p22", l));
            foreach (var (file, set) in new[] { ("redesign_v3.json", "redesign-v3"), ("redesign_v2.json", "redesign-v2"),
                         ("zeydbot.json", "zeydbot"), ("levels.json", "web-v1"), ("designs.json", "designs") })
                foreach (var l in Load("Assets/_Game/Levels/" + file).Levels) pool.Add((set, l));
            return pool;
        }

        // ------------------------------------------------------------------ 1. features

        public static void Features()
        {
            int games = int.Parse(Arg(0, "2000"));
            var pool = Pool();
            var t0 = DateTime.Now;
            var sb = new StringBuilder();
            sb.AppendLine("set_name,level_id,level_version,slot,w,h,pieces,locks,chains,walls,bombs,timer_s,sec_per_move,open_edge_pct," +
                          "fuse,slack_true,slack_true_min,slack_true_mean,slack_approx,choke_n," +
                          "p_survive_rand,nat_slack_med_rand,first_choke_ex_rand,p_survive_greedy,nat_slack_med_greedy,first_choke_ex_greedy," +
                          "p_survive_aware,nat_slack_med_aware,first_choke_ex_aware,nat_slack_per_bomb_rand," +
                          "open_min,open_mean,bottleneck_n,distractor_ratio,distract_per_step,distract_max,distract_vs_slack,distract_vs_slack_max," +
                          "narrow_n,narrow_hidden_n,blocked_share_lock,blocked_share_chain,blocked_share_wall,blocked_share_piece,blocked_share_tab," +
                          "narrow_by_lock,narrow_by_chain,narrow_by_wall,jam_bait_start,jam_bait_mid,dir_ambiguity,calc_s");
            int done = 0;
            foreach (var (set, l) in pool)
            {
                var f = DifficultyV2.Compute(l, games);
                var p = f.Prof;
                int walls = l.Sealed.Values.Sum(v => v.Length);
                var dvs = f.BombPieces.Select((b, i) => p.DistractPerStep[i] / (f.SlackTrue[i] + 1.0)).ToArray();
                bool hasB = f.BombPieces.Length > 0;
                sb.Append(string.Join(",", new[]
                {
                    set, l.Id, l.Version.ToString(), l.Number.ToString(), l.Width.ToString(), l.Height.ToString(), l.Pieces.Count.ToString(),
                    l.Nails.Count.ToString(), l.Pieces.Count(c => c.Length > 1).ToString(), walls.ToString(), l.Bombs.Count.ToString(),
                    l.TimerSeconds.ToString(), F((double)l.TimerSeconds / l.Pieces.Count), l.Open.ToString(),
                    L(f.BombPieces.Select(b => l.Bombs[b].ToString())), L(f.SlackTrue.Select(x => x.ToString())),
                    hasB ? f.SlackTrue.Min().ToString() : "", hasB ? F(f.SlackTrue.Average()) : "", f.SlackApprox ? "t" : "f", f.ChokeN.ToString(),
                    F(f.Rand.PSurvive), F(f.Rand.NatSlackMed), F(f.Rand.FirstChokeEx),
                    F(f.Greedy.PSurvive), F(f.Greedy.NatSlackMed), F(f.Greedy.FirstChokeEx),
                    F(f.Aware.PSurvive), F(f.Aware.NatSlackMed), F(f.Aware.FirstChokeEx),
                    L(f.Rand.NatSlackMedPerBomb.Select(x => F(x))),
                    F(p.OpenMin), F(p.OpenMean), F(p.BottleneckN), F(p.DistractorRatio),
                    L(p.DistractPerStep.Select(x => F(x))), hasB ? F(p.DistractPerStep.Where(x => !double.IsNaN(x)).DefaultIfEmpty(double.NaN).Max()) : "",
                    L(dvs.Select(x => F(x))), hasB ? F(dvs.Where(x => !double.IsNaN(x)).DefaultIfEmpty(double.NaN).Max()) : "",
                    F(p.NarrowN), F(p.NarrowHiddenN), F(p.ShareLock), F(p.ShareChain), F(p.ShareWall), F(p.SharePiece), F(p.ShareTab),
                    F(p.NarrowByLock), F(p.NarrowByChain), F(p.NarrowByWall), F(p.JamBaitStart), F(p.JamBaitMid), F(p.DirAmbiguity), F(f.Seconds),
                }));
                sb.AppendLine();
                if (++done % 20 == 0) Debug.Log($"[pm69] features {done}/{pool.Count} {(DateTime.Now - t0).TotalSeconds:0}s");
            }
            File.WriteAllText(Out + "level_features_v2.csv", sb.ToString());
            Debug.Log($"[pm69] features done: {pool.Count} levels, {games} games/policy, {(DateTime.Now - t0).TotalSeconds:0}s");
        }

        // ------------------------------------------------------------------ 2. replay

        sealed class Row
        {
            public string RowId, Player, Level, Kind, Dir, Fuses; public int Version, Attempt, MoveNo, Piece, T, Think;
        }

        sealed class Step
        {
            public int Index, Piece, Open, OnPath, Think, Dt, Jams; public bool Narrow, Hidden; public string Limiters;
            public int[] JamBy = new int[7];
        }

        sealed class Result
        {
            public int FailAt = -1; public string Reason = ""; public GameSession Session; public List<Step> Steps = new List<Step>();
            public int[] JamBy = new int[7]; public int OnPath, OffPath; public double OpenSum; public int Exits;
        }

        // jam reasons: kilit, zincir, duvar, parça, tırnak, serbest (exit was open), yön yok + serbest
        static readonly System.Diagnostics.Stopwatch swDig = new System.Diagnostics.Stopwatch(), swInfer = new System.Diagnostics.Stopwatch(),
            swReason = new System.Diagnostics.Stopwatch();
        static readonly string[] JamNames = { "lock", "chain", "wall", "piece", "tab", "free", "nodir" };

        static string Fuses(GameSession s) =>
            string.Join(";", s.Bombs.Where(kv => s.Board.IsPresent(kv.Key)).OrderBy(kv => kv.Key).Select(kv => $"{kv.Key}:{kv.Value}"));

        static string Norm(string f) => string.IsNullOrEmpty(f) ? "" :
            string.Join(";", f.Split(';').Where(x => x.Length > 0).OrderBy(x => int.Parse(x.Split(':')[0])));

        static int JamReason(GameSession s, FastState fs, int piece, string dir)
        {
            if (s.NailAt(piece) > 0) return 0;
            if (string.IsNullOrEmpty(dir) || !DirExt.TryParse(dir, out var d))
            {
                if (TravelRule.CanExit(s.Board, piece)) return 6;
                var r = fs.Reason(piece);
                return r == Hold.Chain ? 1 : r == Hold.Wall ? 2 : r == Hold.Piece ? 3 : r == Hold.Lock ? 0 : 4;
            }
            var lim = TravelRule.Limit(s.Board, piece, d);
            if (lim.Exit) return 5;
            if (lim.Blocker < 0) return 2;
            if (fs.O.Chain[piece] && fs.Reason(piece) == Hold.Chain) return 1;
            return lim.Hard ? 3 : 4;
        }

        /* Plays one attempt. cuts: scissors row index -> joint; seed: the session's random (wand). */
        static Result Play(LevelData level, List<Row> rows, Dictionary<int, (int x, int y, bool v)> cuts, uint seed, bool stats)
        {
            var s = new GameSession(level, rows[0].Attempt, "replay", new Mulberry32(seed)) { TestMode = true };
            var r = new Result { Session = s };
            Obstacles obs = stats ? new Obstacles(level) : null;
            bool jointsChanged = false;
            int lastT = 0; var jamBuf = new int[7]; int jamN = 0;
            var onPathMarks = new List<(int piece, HashSet<int> bombsOnPath)>();
            var prevDig = new Dictionary<int, (int cost, List<int> set)>();
            for (int i = 0; i < rows.Count; i++)
            {
                var m = rows[i];
                if (m.Kind == "start") { lastT = m.T; continue; }
                if (m.Kind == "exit")
                {
                    if (s.IsOver && s.Outcome == Outcome.Bomb && !string.IsNullOrEmpty(m.Fuses))
                    {
                        // continued after the bomb: the row's fuses are the reset values
                        int reset = m.Fuses.Split(';').Select(x => int.Parse(x.Split(':')[1])).Max();
                        s.Continue(30, reset);
                    }
                    if (!s.IsOver && Norm(m.Fuses) != Fuses(s)) { r.FailAt = i; r.Reason = "fuse_mismatch"; return r; }
                    if (stats && !s.IsOver)
                    {
                        if (jointsChanged) { obs = new Obstacles(s.Board); jointsChanged = false; }
                        var fs = new FastState(obs, s.Board, s.NailAt, s.FuseAt);
                        var avail = fs.AvailableList();
                        var st = new Step { Index = s.Moves + 1, Piece = m.Piece, Open = avail.Count, Think = m.Think, Dt = m.T - lastT, Jams = jamN, JamBy = jamBuf };
                        st.Narrow = st.Open <= 2;
                        st.Hidden = st.Narrow && !avail.Any(fs.VisuallyFree);
                        swReason.Start();
                        var lim = new SortedSet<string>();
                        for (int p = 0; p < obs.N; p++)
                        {
                            if (!fs.Present[p] || fs.Available(p)) continue;
                            var why = fs.Reason(p);
                            if (why == Hold.Lock) lim.Add("lock"); else if (why == Hold.Chain) lim.Add("chain"); else if (why == Hold.Wall) lim.Add("wall");
                        }
                        st.Limiters = string.Join("+", lim);
                        swReason.Stop();
                        // bombs' current shortest digs: on-path counts for the nearest one, marks for every one
                        var onBombs = new HashSet<int>(); int nearest = -1; HashSet<int> nearestSet = null;
                        foreach (var kv in s.Bombs.Where(kv => s.Board.IsPresent(kv.Key)).OrderBy(kv => kv.Value).ThenBy(kv => kv.Key))
                        {
                            bool known = prevDig.TryGetValue(kv.Key, out var pd);
                            swDig.Start();
                            int cost = DifficultyV2.MinExits(fs, kv.Key, out var set, out _, 20000,
                                known ? pd.cost : DifficultyV2.Unreachable, known ? pd.set : null);
                            swDig.Stop();
                            prevDig[kv.Key] = (cost, set);
                            var hs = new HashSet<int>(set);
                            if (nearest < 0) { nearest = kv.Key; nearestSet = hs; }
                            if (hs.Contains(m.Piece)) onBombs.Add(kv.Key);
                        }
                        st.OnPath = nearestSet == null ? -1 : avail.Count(nearestSet.Contains);
                        onPathMarks.Add((m.Piece, onBombs));
                        r.Steps.Add(st); r.OpenSum += st.Open;
                    }
                    var res = s.TryExit(m.Piece, DirExt.TryParse(m.Dir ?? "", out var d) ? d : Dir.U);
                    if (res != CommandResult.Ok) { r.FailAt = i; r.Reason = "exit_" + res; return r; }
                    lastT = m.T; jamBuf = new int[7]; jamN = 0;
                    continue;
                }
                if (m.Kind == "jam")
                {
                    if (s.IsOver || m.Piece < 0 || m.Piece >= level.Pieces.Count || !s.Board.IsPresent(m.Piece)) continue;
                    if (stats)
                    {
                        if (jointsChanged) { obs = new Obstacles(s.Board); jointsChanged = false; }
                        int why = JamReason(s, new FastState(obs, s.Board, s.NailAt, s.FuseAt), m.Piece, m.Dir);
                        jamBuf[why]++; r.JamBy[why]++;
                    }
                    jamN++;
                    continue;
                }
                if (m.Kind == "hammer")
                {
                    if (s.UseHammer(m.Piece) != CommandResult.Ok) { r.FailAt = i; r.Reason = "hammer"; return r; }
                    lastT = m.T;
                    continue;
                }
                if (m.Kind == "clock") { s.UseClock(); continue; }
                if (m.Kind == "scissors")
                {
                    if (cuts != null && cuts.TryGetValue(i, out var c))
                    {
                        if (s.UseScissors(c.x, c.y, c.v) != CommandResult.Ok) { r.FailAt = i; r.Reason = "scissors"; return r; }
                        jointsChanged = true; prevDig.Clear();
                    }
                    continue;
                }
                if (m.Kind == "wand")
                {
                    if (s.UseWand() != CommandResult.Ok) { r.FailAt = i; r.Reason = "wand"; return r; }
                    jointsChanged = true; prevDig.Clear();
                    continue;
                }
            }
            r.Exits = s.Moves;
            // bomb deaths: exits on / off the shortest dig of the bomb that blew
            if (stats && s.Outcome == Outcome.Bomb)
            {
                var blown = s.Bombs.Where(kv => s.Board.IsPresent(kv.Key) && kv.Value <= 0).Select(kv => kv.Key).DefaultIfEmpty(-1).First();
                foreach (var (piece, on) in onPathMarks) if (on.Contains(blown)) r.OnPath++; else r.OffPath++;
            }
            return r;
        }

        static IEnumerable<(int x, int y, bool v)> Joints(LevelData l)
        {
            for (int y = 0; y < l.Height; y++) for (int x = 1; x < l.Width; x++) yield return (x, y, true);
            for (int y = 1; y < l.Height; y++) for (int x = 0; x < l.Width; x++) yield return (x, y, false);
        }

        /* Fills unknown scissors cuts and the wand's seed so the recorded exits stay legal as long as possible. */
        static (Dictionary<int, (int, int, bool)> cuts, uint seed, string note) Infer(LevelData level, List<Row> rows)
        {
            var cuts = new Dictionary<int, (int, int, bool)>();
            uint seed = 1;
            string note = "";
            var first = Play(level, rows, cuts, seed, false);
            if (rows.Any(m => m.Kind == "wand"))
            {
                note += "wand";
                int bestAt = first.FailAt < 0 ? int.MaxValue : first.FailAt;
                for (uint sd = 2; sd <= 80 && bestAt != int.MaxValue; sd++)
                {
                    int at = Play(level, rows, cuts, sd, false).FailAt;
                    if (at < 0) at = int.MaxValue;
                    if (at > bestAt) { bestAt = at; seed = sd; }
                }
            }
            var scissorRows = Enumerable.Range(0, rows.Count).Where(i => rows[i].Kind == "scissors").ToList();
            if (scissorRows.Count > 0) note += (note.Length > 0 ? "+" : "") + "scissors";
            foreach (int si in scissorRows)
            {
                int bestAt = Play(level, rows, cuts, seed, false).FailAt;
                if (bestAt < 0) break;
                (int, int, bool)? best = null;
                foreach (var j in Joints(level))
                {
                    cuts[si] = j;
                    int at = Play(level, rows, cuts, seed, false).FailAt;
                    if (at < 0) at = int.MaxValue;
                    if (at > bestAt) { bestAt = at; best = j; }
                    cuts.Remove(si);
                }
                if (best.HasValue) cuts[si] = best.Value;
            }
            return (cuts, seed, note);
        }

        public static void Replay()
        {
            var t0 = DateTime.Now;
            var levels = new Dictionary<(string, int), LevelData>();
            foreach (var (set, l) in Pool()) if (set == "redesign-v4") levels[(l.Id, l.Version)] = l;
            var plays = File.ReadAllLines("Logs/pm69/plays.tsv").Select(x => x.Split('\t')).ToDictionary(x => x[0], x => x);
            var rows = File.ReadAllLines("Logs/pm69/moves.tsv").Select(x => x.Split('\t')).Select(x => new Row
            {
                RowId = x[0], Player = x[1], Level = x[2], Version = int.Parse(x[3]), Attempt = int.Parse(x[4]), MoveNo = int.Parse(x[5]),
                Kind = x[6], Piece = x[7] == "" ? -1 : int.Parse(x[7]), Dir = x[8], T = x[9] == "" ? 0 : int.Parse(x[9]),
                Think = x[10] == "" ? 0 : int.Parse(x[10]), Fuses = x[12],
            }).GroupBy(r => r.RowId).ToList();

            var att = new StringBuilder("row_id,player,level_id,level_version,attempt,result,moves,replay_exits,replay_ok,reason,inferred," +
                "off_path_exits,on_path_exits,jams,jam_lock,jam_chain,jam_wall,jam_piece,jam_tab,jam_free,jam_nodir,open_mean\n");
            var steps = new StringBuilder("row_id,player,level_id,level_version,attempt,step,piece,open_n,on_path_n,narrow,hidden,limiters,think_ms,dt_ms," +
                "jams,jam_lock,jam_chain,jam_wall,jam_piece,jam_tab,jam_free,jam_nodir\n");
            int ok = 0, withPlay = 0, n = 0, okNoBooster = 0, noBooster = 0;
            var reasons = new Dictionary<string, int>();
            int limit = int.Parse(Arg(0, "100000")), failLogs = 0;
            foreach (var g in rows.Take(limit))
            {
                var ta = DateTime.Now;
                var list = g.ToList();
                var m0 = list[0];
                n++;
                string reason; bool good = false; Result res = null; string note = "";
                plays.TryGetValue(g.Key, out var play);
                if (!levels.TryGetValue((m0.Level, m0.Version), out var level)) reason = "level_missing";
                else
                {
                    swInfer.Start();
                    var (cuts, seed, nt) = Infer(level, list);
                    swInfer.Stop();
                    note = nt;
                    res = Play(level, list, cuts, seed, true);
                    var s = res.Session;
                    if (res.FailAt >= 0) reason = res.Reason;
                    else if (play == null) reason = "no_plays_row";
                    else
                    {
                        string result = play[1]; int moves = int.Parse(play[2]);
                        bool outcome = result == "win" ? s.Outcome == Outcome.Win : result == "bomb" ? s.Outcome == Outcome.Bomb : !s.IsOver;
                        if (!outcome && result == "win" && !s.IsOver && s.Board.PresentCount == 1 && s.Moves == moves - 1)
                        { reason = ""; good = true; note += (note.Length > 0 ? "+" : "") + "last_exit_unlogged"; }
                        else if (!outcome) reason = $"outcome_{s.Outcome}_vs_{result}";
                        else if (s.Moves != moves) reason = $"exits_{s.Moves}_vs_{moves}";
                        else { reason = ""; good = true; }
                    }
                }
                if (!good && failLogs++ < 12)
                    Debug.Log($"[pm69] fail {g.Key.Substring(0, 8)} {m0.Level} v{m0.Version} {reason} {note} rows {list.Count} at {res?.FailAt} kinds " +
                              string.Join("", list.Select(x => x.Kind[0])) + $" play {play?[1]} {play?[2]}");
                if (play != null) withPlay++;
                if (good) ok++;
                bool booster = note.Contains("wand") || note.Contains("scissors");
                if (play != null && !booster) { noBooster++; if (good) okNoBooster++; }
                reasons[good ? "ok" : reason] = reasons.TryGetValue(good ? "ok" : reason, out int c) ? c + 1 : 1;
                var jb = res?.JamBy ?? new int[7];
                att.AppendLine(string.Join(",", new[]
                {
                    g.Key, Csv(m0.Player), m0.Level, m0.Version.ToString(), m0.Attempt.ToString(), play?[1] ?? "", play?[2] ?? "",
                    res?.Session.Moves.ToString() ?? "", good ? "t" : "f", reason, note,
                    res != null && res.Session.Outcome == Outcome.Bomb ? res.OffPath.ToString() : "",
                    res != null && res.Session.Outcome == Outcome.Bomb ? res.OnPath.ToString() : "",
                    jb.Sum().ToString(), jb[0].ToString(), jb[1].ToString(), jb[2].ToString(), jb[3].ToString(), jb[4].ToString(), jb[5].ToString(), jb[6].ToString(),
                    res != null && res.Steps.Count > 0 ? F(res.OpenSum / res.Steps.Count) : "",
                }));
                if (res != null)
                    foreach (var st in res.Steps)
                        steps.AppendLine(string.Join(",", new[]
                        {
                            g.Key, Csv(m0.Player), m0.Level, m0.Version.ToString(), m0.Attempt.ToString(), st.Index.ToString(), st.Piece.ToString(),
                            st.Open.ToString(), st.OnPath < 0 ? "" : st.OnPath.ToString(), st.Narrow ? "t" : "f", st.Hidden ? "t" : "f", st.Limiters,
                            st.Think.ToString(), st.Dt.ToString(), st.Jams.ToString(),
                            st.JamBy[0].ToString(), st.JamBy[1].ToString(), st.JamBy[2].ToString(), st.JamBy[3].ToString(), st.JamBy[4].ToString(), st.JamBy[5].ToString(), st.JamBy[6].ToString(),
                        }));
                double secs = (DateTime.Now - ta).TotalSeconds;
                if (secs > 3) Debug.Log($"[pm69] slow {m0.Level} {note} {list.Count} rows {secs:0.0}s dig {swDig.Elapsed.TotalSeconds:0} infer {swInfer.Elapsed.TotalSeconds:0} reason {swReason.Elapsed.TotalSeconds:0}");
                if (n % 100 == 0) Debug.Log($"[pm69] replay {n}/{rows.Count} {(DateTime.Now - t0).TotalSeconds:0}s");
            }
            File.WriteAllText(Out + "replay_pt1.csv", att.ToString());
            File.WriteAllText(Out + "replay_pt1_steps.csv", steps.ToString());
            Debug.Log($"[pm69] replay done: attempts {n}, with plays row {withPlay}, ok {ok} ({100.0 * ok / Math.Max(1, withPlay):0.0}% of those), " +
                      $"no-booster ok {okNoBooster}/{noBooster}, {(DateTime.Now - t0).TotalSeconds:0}s");
            foreach (var kv in reasons.OrderByDescending(kv => kv.Value)) Debug.Log($"[pm69] reason {kv.Key}: {kv.Value}");
        }

        static string Csv(string s) => s == null ? "" : s.Contains(",") || s.Contains("\"") ? "\"" + s.Replace("\"", "\"\"") + "\"" : s;
    }
}
