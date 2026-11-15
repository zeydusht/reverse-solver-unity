using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReverseSolver.Core;
using ReverseSolver.Generation;
using UnityEngine;
using Role = ReverseSolver.Generation.DifficultyModelV2.Role;

namespace ReverseSolver.EditorTools
{
    /* PM_71: p22 on the data-blended p_win1 (DifficultyModelV2.PWin1Blended, pt1).
         bash Tools/unity_batch.sh method ReverseSolver.EditorTools.Pm71Tune.Report          the 11-40 table ([pm71] lines)
         bash Tools/unity_batch.sh method ReverseSolver.EditorTools.Pm71Tune.Search 26:.62:move+lock,37:.55
                                                         options for a slot and a target (fuse and timer; moves, locks)
       The blend is the model's logit shifted by a constant per level, c = w (d_obs + logit p_model(pt1 version)), so a
       blended target T is the model target sigmoid(logit T + c): Search aims the timer at that. */
    public static class Pm71Tune
    {
        static string Arg(int i, string def)
        {
            var a = Environment.GetCommandLineArgs();
            int k = Array.IndexOf(a, "-executeMethod");
            return k >= 0 && a.Length > k + 2 + i && !a[k + 2 + i].StartsWith("-") ? a[k + 2 + i] : def;
        }
        static LevelSet Original() => LevelParser.Parse(File.ReadAllText("Assets/_Game/Levels/redesign.json"));
        /* PM_70's p22 (live before PM_71) and the p22 with PM_71. */
        public static LevelSet Before() => P21Onboarding.TuneLate(P21Onboarding.TuneSecondTen(P21Onboarding.SpaceObstacleLessons(Original())));
        public static LevelSet After() => P21Onboarding.P22(Original());
        static string Map(IEnumerable<KeyValuePair<int, int>> m) => string.Join(",", m.OrderBy(kv => kv.Key).Select(kv => $"{kv.Key}:{kv.Value}"));

        public static void Report()
        {
            var before = Before().Levels; var after = After().Levels;
            var rowsB = new List<(int, Role, double)>(); var rowsA = new List<(int, Role, double)>();
            for (int slot = 11; slot <= 40; slot++)
            {
                var b = before[slot - 1]; var a = after[slot - 1];
                var fb = DifficultyV2.Compute(b); var fa = ReferenceEquals(a, b) ? fb : DifficultyV2.Compute(a);
                var role = Pm70Tune.RoleOf(slot);
                var pb = DifficultyModelV2.PWin1Parts(fb); var pa = DifficultyModelV2.PWin1Parts(fa);
                rowsB.Add((slot, role, pb.Blended)); rowsA.Add((slot, role, pa.Blended));
                var (lo, hi) = DifficultyModelV2.Band(slot, role);
                var broken = DifficultyModelV2.Rules.Check(fa, slot, role);
                string Veri(DifficultyModelV2.Blend x) => x.HasData ? x.Data.ToString("0.00") : "-";
                Debug.Log($"[pm71] {slot} {a.Id} v{b.Version}->v{a.Version} {role} n {pa.N} model {pb.Model:0.00}->{pa.Model:0.00} " +
                          $"veriyle {Veri(pb)}->{Veri(pa)} birlesik {pb.Blended:0.000}->{pa.Blended:0.000} band {lo:0.00}-{hi:0.00}" +
                          (pa.Blended < lo - 1e-9 || pa.Blended > hi + 1e-9 ? " DISARIDA" : "") +
                          $" timer {b.TimerSeconds}->{a.TimerSeconds} bombs [{Map(b.Bombs)}]->[{Map(a.Bombs)}] nails [{Map(b.Nails)}]->[{Map(a.Nails)}]" +
                          (broken.Count > 0 ? " RULES: " + string.Join("; ", broken) : ""));
            }
            foreach (var u in DifficultyModelV2.CheckCurve(rowsB)) Debug.Log("[pm71] CURVE before: " + u);
            foreach (var u in DifficultyModelV2.CheckCurve(rowsA)) Debug.Log("[pm71] CURVE after: " + u);
            Debug.Log($"[pm71] curve after: {DifficultyModelV2.CheckCurve(rowsA).Count} broken");
        }

        /* Exact options: "slot|timer|bombs|nails;..." on PM_70's level (timer 0 keeps; bombs/nails empty keep). */
        public static void Eval()
        {
            foreach (var item in Arg(0, "37|0|8:34,29:22,30:9|").Split(';'))
            {
                var v = item.Split('|');
                int slot = int.Parse(v[0]);
                var l = Before().Levels[slot - 1];
                var t = P21Onboarding.Retuned(l, new P21Onboarding.Retune(l.Id, int.Parse(v[1]), v.Length > 2 && v[2] != "" ? v[2] : null, v.Length > 3 && v[3] != "" ? v[3] : null));
                var f = DifficultyV2.Compute(t);
                var p = DifficultyModelV2.PWin1Parts(f);
                var broken = DifficultyModelV2.Rules.Check(f, slot, Pm70Tune.RoleOf(slot));
                Debug.Log($"[pm71] eval {slot} {l.Id} timer {t.TimerSeconds} bombs [{Map(t.Bombs)}] nails [{Map(t.Nails)}] model {p.Model:0.000} veriyle {p.Data:0.000} " +
                          $"blended {p.Blended:0.000} jam {DifficultyModelV2.JamPerExit(f):0.00} choke {f.ChokeN} slack [{string.Join(",", f.SlackTrue)}]" +
                          (broken.Count > 0 ? " ! " + string.Join("; ", broken) : ""));
            }
        }

        /* Options on PM_70's level: the fuse of every bomb, one bomb at a time (all -8..+8), a bomb moved to another
           piece at its dig + slack, one lock added; each with the timer that best meets the target. */
        public static void Search()
        {
            foreach (var item in Arg(0, "26:.62").Split(','))
            {
                var parts = item.Split(':');
                int slot = int.Parse(parts[0]);
                double target = double.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
                string extra = parts.Length > 2 ? parts[2] : "";   // "move" and/or "lock": also those options
                var l = Before().Levels[slot - 1];
                var role = Pm70Tune.RoleOf(slot);
                var f0 = DifficultyV2.Compute(l, 600, 80);
                var parts0 = DifficultyModelV2.PWin1Parts(f0);
                double c = DifficultyModelV2.Logit0(parts0.Model) - DifficultyModelV2.Logit0(parts0.Blended);
                double modelTarget = DifficultyModelV2.Sigmoid(DifficultyModelV2.Logit0(target) + c);
                Debug.Log($"[pm71] search {slot} {l.Id} v{l.Version} {role} now model {parts0.Model:0.000} blended {parts0.Blended:0.000} " +
                          $"target {target:0.00} -> model target {modelTarget:0.00}; bombs [{Map(l.Bombs)}] nails [{Map(l.Nails)}] timer {l.TimerSeconds} pieces {l.Pieces.Count}");
                var cands = new List<(double cost, string desc)>();
                var seen = new HashSet<string>();
                void Try(Dictionary<int, int> bombs, Dictionary<int, int> nails, string how)
                {
                    string key = Map(bombs) + "|" + Map(nails);
                    if (!seen.Add(key)) return;
                    var t = P21Onboarding.Retuned(l, new P21Onboarding.Retune(l.Id, 0, Map(bombs), Map(nails)));
                    var f = DifficultyV2.Compute(t, 600, 80);
                    int timer = DifficultyModelV2.TimerFor(f, modelTarget);
                    t = P21Onboarding.Retuned(l, new P21Onboarding.Retune(l.Id, timer, Map(bombs), Map(nails)));
                    f.Level = t;
                    var p = DifficultyModelV2.PWin1Parts(f);
                    var broken = DifficultyModelV2.Rules.Check(f, slot, role);
                    double cost = Math.Abs(p.Blended - target) + broken.Count;
                    cands.Add((cost, $"blended {p.Blended:0.000} model {p.Model:0.00} jam {DifficultyModelV2.JamPerExit(f):0.00} choke {f.ChokeN} timer {timer} " +
                                     $"bombs \"{Map(bombs)}\" nails \"{Map(nails)}\" ({how}) slack [{string.Join(",", f.SlackTrue)}]" +
                                     (broken.Count > 0 ? " ! " + string.Join("; ", broken) : "")));
                }
                var bombs0 = parts.Length > 4 && parts[4] != "" ? P21Onboarding.Counters(parts[4]) : l.Bombs.ToDictionary(kv => kv.Key, kv => kv.Value);
                var nails0 = l.Nails.ToDictionary(kv => kv.Key, kv => kv.Value);
                Try(bombs0, nails0, "yalnız süre");
                for (int s = -8; s <= 8; s++)
                {
                    if (s == 0 || bombs0.Count == 0) continue;
                    Try(bombs0.ToDictionary(kv => kv.Key, kv => Math.Max(1, kv.Value + s)), nails0, $"tüm fitiller {s:+0;-0}");
                    foreach (var b in bombs0.Keys)
                        Try(bombs0.ToDictionary(kv => kv.Key, kv => kv.Key == b ? Math.Max(1, kv.Value + s) : kv.Value), nails0, $"bomba {b} fitil {s:+0;-0}");
                }
                // a bomb moved to another piece (fuse = its shortest dig + slack)
                var start = new FastState(new Obstacles(l));
                foreach (var b in extra.Contains("move") ? bombs0.Keys.ToList() : new List<int>())
                    for (int p = 0; p < l.Pieces.Count; p++)
                    {
                        if (bombs0.ContainsKey(p) || nails0.ContainsKey(p)) continue;
                        int dig = DifficultyV2.MinExits(start, p, out _, out _);
                        foreach (int slack in new[] { 2, 4, 6, 9 })
                        {
                            var moved = bombs0.Where(kv => kv.Key != b).ToDictionary(kv => kv.Key, kv => kv.Value);
                            moved[p] = dig + slack;
                            Try(moved, nails0, $"bomba {b} -> {p}, pay {slack}");
                        }
                    }
                // one lock added
                for (int p = 0; p < (extra.Contains("lock") ? l.Pieces.Count : 0); p++)
                {
                    if (bombs0.ContainsKey(p) || nails0.ContainsKey(p)) continue;
                    foreach (int count in new[] { 6, 10, 16 })
                    {
                        var n = new Dictionary<int, int>(nails0) { [p] = count };
                        Try(bombs0, n, $"kilit {p}:{count}");
                    }
                }
                string only = parts.Length > 3 ? parts[3] : null;   // e.g. "bomba 30": every option of that kind
                foreach (var cand in cands.Where(x => only == null || x.desc.Contains(only)).OrderBy(x => x.cost).Take(only == null ? 8 : 40))
                    Debug.Log($"[pm71]   {cand.cost:0.000} {cand.desc}");
            }
        }
    }
}
