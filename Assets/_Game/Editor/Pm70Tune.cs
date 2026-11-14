using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReverseSolver.Core;
using ReverseSolver.Editing;
using ReverseSolver.Generation;
using UnityEngine;
using Role = ReverseSolver.Generation.DifficultyModelV2.Role;

namespace ReverseSolver.EditorTools
{
    /* PM_70: p22 11-40 on difficulty model v2.
         bash Tools/unity_batch.sh method ReverseSolver.EditorTools.Pm70Tune.Report        current p22 table ([pm70] lines)
         bash Tools/unity_batch.sh method ReverseSolver.EditorTools.Pm70Tune.Search 15,16  fuse / bomb-count / timer options
         bash Tools/unity_batch.sh method ReverseSolver.EditorTools.Pm70Tune.Nails 23       lock candidates for the jam rule
       Search works on the PM_68 stage (before PM_70's table) with the slot's PM_70 role. */
    public static class Pm70Tune
    {
        static string Arg(int i, string def)
        {
            var a = Environment.GetCommandLineArgs();
            int k = Array.IndexOf(a, "-executeMethod");
            return k >= 0 && a.Length > k + 2 + i && !a[k + 2 + i].StartsWith("-") ? a[k + 2 + i] : def;
        }

        static LevelSet Original() => LevelParser.Parse(File.ReadAllText("Assets/_Game/Levels/redesign.json"));
        public static LevelSet Before() => P21Onboarding.TuneSecondTen(P21Onboarding.SpaceObstacleLessons(Original()));
        public static LevelSet After() => P21Onboarding.P22(Original());

        public static Role RoleOf(int slot) =>
            slot <= 10 || P21Onboarding.IsObstacleLesson(slot) ? Role.Lesson :
            P21Onboarding.HardSlots22.Contains(slot) ? Role.Hard :
            P21Onboarding.BreatherSlots22.Contains(slot) ? Role.Breather : Role.Normal;

        public static double Target(int slot, Role role) =>
            slot == 40 ? .35 : role == Role.Breather ? .82 : (DifficultyModelV2.Band(slot, role).lo + DifficultyModelV2.Band(slot, role).hi) / 2;

        static string Map(IEnumerable<KeyValuePair<int, int>> m) => string.Join(",", m.OrderBy(kv => kv.Key).Select(kv => $"{kv.Key}:{kv.Value}"));

        public static void Report()
        {
            var before = Before().Levels; var after = After().Levels;
            var rows = new List<(int, Role, double)>();
            for (int slot = 11; slot <= 40; slot++)
            {
                var b = before[slot - 1]; var a = after[slot - 1];
                var fb = DifficultyV2.Compute(b); var fa = ReferenceEquals(a, b) ? fb : DifficultyV2.Compute(a);
                var role = RoleOf(slot);
                double p = DifficultyModelV2.PWin1(fa);
                rows.Add((slot, role, p));
                var broken = DifficultyModelV2.Rules.Check(fa, slot, role);
                Debug.Log($"[pm70] {slot} {a.Id} v{b.Version}->v{a.Version} {role} hard={a.Hard} p {DifficultyModelV2.PWin1(fb):0.00}->{p:0.00} " +
                          $"jam {DifficultyModelV2.JamPerExit(fb):0.00}->{DifficultyModelV2.JamPerExit(fa):0.00} choke {fb.ChokeN}->{fa.ChokeN} " +
                          $"timer {b.TimerSeconds}->{a.TimerSeconds} bombs [{Map(b.Bombs)}]->[{Map(a.Bombs)}] nails [{Map(b.Nails)}]->[{Map(a.Nails)}] " +
                          $"slack [{string.Join(",", fa.SlackTrue)}] pR {fa.Rand.PSurvive:0.00} pA {fa.Aware.PSurvive:0.00} " +
                          $"fun {FunSim.Run(a, Genre.Kolay, FunSim.Calibrated(FunSim.Population(160))).FirstWinRate:0.00}" +
                          (broken.Count > 0 ? " RULES: " + string.Join("; ", broken) : ""));
            }
            foreach (var u in DifficultyModelV2.CheckCurve(rows)) Debug.Log("[pm70] CURVE: " + u);
        }

        /* Fuse slack per bomb over the shortest dig; subsets of the bombs; one bomb on a bombless board. */
        public static void Search()
        {
            var slots = Arg(0, "13").Split(',').Select(int.Parse).ToList();
            var before = Before().Levels;
            foreach (int slot in slots)
            {
                var l = before[slot - 1];
                var role = RoleOf(slot);
                double target = Target(slot, role);
                var (lo, hi) = DifficultyModelV2.Band(slot, role);
                var start = new FastState(new Obstacles(l));
                var cands = new List<(double cost, string desc)>();
                void Try(Dictionary<int, int> bombs, string how)
                {
                    var t = P21Onboarding.Retuned(l, new P21Onboarding.Retune(l.Id, 0, Map(bombs)));
                    var f = DifficultyV2.Compute(t, 600, 80);
                    int timer = DifficultyModelV2.TimerFor(f, target);
                    t = P21Onboarding.Retuned(l, new P21Onboarding.Retune(l.Id, timer, Map(bombs)));
                    f.Level = t;
                    double p = DifficultyModelV2.PWin1(f);
                    var broken = DifficultyModelV2.Rules.Check(f, slot, role);
                    double cost = Math.Abs(p - target) + (p < lo || p > hi ? 1 : 0) + broken.Count;
                    cands.Add((cost, $"p {p:0.00} jam {DifficultyModelV2.JamPerExit(f):0.00} choke {f.ChokeN} timer {timer} bombs \"{Map(bombs)}\" ({how}) " +
                                     $"slack [{string.Join(",", f.SlackTrue)}] pR {f.Rand.PSurvive:0.00} pA {f.Aware.PSurvive:0.00}" + (broken.Count > 0 ? " ! " + string.Join("; ", broken) : "")));
                }
                var keys = l.Bombs.Keys.OrderBy(k => k).ToList();
                var dig = keys.ToDictionary(k => k, k => DifficultyV2.MinExits(start, k, out _, out _));
                var slacks = new[] { 0, 1, 2, 3, 4, 5, 6, 8, 10, 13, 16 };
                if (keys.Count == 0)
                {
                    if (role == Role.Normal || role == Role.Hard)
                        for (int p = 0; p < l.Pieces.Count; p++)
                        {
                            if (l.Nails.ContainsKey(p)) continue;
                            int m = DifficultyV2.MinExits(start, p, out _, out _);
                            foreach (int s in new[] { 1, 3, 5, 8 }) Try(new Dictionary<int, int> { [p] = m + s }, $"yeni bomba {p}, pay {s}");
                        }
                    else Try(new Dictionary<int, int>(), "bombasız");
                }
                else
                {
                    int n = keys.Count;
                    for (int mask = 1; mask < 1 << n; mask++)
                    {
                        var keep = keys.Where((k, i) => (mask >> i & 1) == 1).ToList();
                        if (keep.Count < Math.Max(1, n - 2)) continue;
                        foreach (int s in slacks)
                        {
                            Try(keep.ToDictionary(k => k, k => dig[k] + s), $"{keep.Count}/{n} bomba, pay {s}");
                            // one or two tight bombs (the closest digs) and the rest at s
                            int tight = role == Role.Hard ? 2 : 1;
                            for (int k = 1; k <= Math.Min(tight, keep.Count - 1); k++)
                            {
                                var tightSet = keep.OrderBy(b => dig[b]).Take(k).ToList();
                                if (s <= 1) continue;
                                Try(keep.ToDictionary(b => b, b => dig[b] + (tightSet.Contains(b) ? 1 : s)), $"{keep.Count}/{n} bomba, {k} sıkı (pay 1), diğerleri pay {s}");
                            }
                        }
                    }
                }
                Debug.Log($"[pm70] search {slot} {l.Id} v{l.Version} {role} target {target:0.00} band {lo:0.00}-{hi:0.00} bombs now [{Map(l.Bombs)}] digs [{Map(dig)}] timer {l.TimerSeconds} pieces {l.Pieces.Count}");
                foreach (var c in cands.OrderBy(c => c.cost).Take(6)) Debug.Log($"[pm70]   {c.cost:0.000} {c.desc}");
            }
        }

        /* Locks for the jam rule: up to three, added greedily on the PM_70 level of the slot, keeping p_win1 within .06
           and not adding a choke. */
        public static void Nails()
        {
            var slots = Arg(0, "23").Split(',').Select(int.Parse).ToList();
            var after = After().Levels;
            foreach (int slot in slots)
            {
                var l = after[slot - 1];
                var f0 = DifficultyV2.Compute(l, 400, 150);
                double p0 = DifficultyModelV2.PWin1(f0), jam = DifficultyModelV2.JamPerExit(f0);
                Debug.Log($"[pm70] nails {slot} {l.Id} jam now {jam:0.00} p {p0:0.00} choke {f0.ChokeN} nails [{Map(l.Nails)}]");
                var nails = l.Nails.ToDictionary(kv => kv.Key, kv => kv.Value);
                for (int round = 0; round < 3 && jam < .56; round++)
                {
                    (double jam, int piece, int count, double p, int choke) best = (jam, -1, 0, 0, 0);
                    for (int p = 0; p < l.Pieces.Count; p++)
                    {
                        if (nails.ContainsKey(p) || l.Bombs.ContainsKey(p)) continue;
                        foreach (int c in new[] { 6, 10, 16, 24 })
                        {
                            var trial = new Dictionary<int, int>(nails) { [p] = c };
                            var t = P21Onboarding.Retuned(l, new P21Onboarding.Retune(l.Id, 0, null, Map(trial)));
                            var f = DifficultyV2.Compute(t, 400, 150);
                            double pw = DifficultyModelV2.PWin1(f), j = DifficultyModelV2.JamPerExit(f);
                            if (Math.Abs(pw - p0) > .06 || f.ChokeN > f0.ChokeN) continue;
                            // past the rule: the gentlest lock that clears .56; below it: the strongest
                            bool clears = j >= .56, bestClears = best.jam >= .56 && best.piece >= 0;
                            if (clears ? !bestClears || j < best.jam : !bestClears && j > best.jam + 1e-9) best = (j, p, c, pw, f.ChokeN);
                        }
                    }
                    if (best.piece < 0) break;
                    nails[best.piece] = best.count; jam = best.jam;
                    Debug.Log($"[pm70]   + {best.piece}:{best.count} -> jam {best.jam:0.00} p {best.p:0.00} choke {best.choke} nails \"{Map(nails)}\"");
                }
            }
        }

        public static void FactoryProbe()
        {
            var catalog = Catalog.From(LevelParser.Parse(File.ReadAllText("Assets/_Game/Levels/levels.json")));
            var o = Factory.RunSlot(FactoryPlan.Slot(int.Parse(Arg(0, "1"))), new FactoryOptions { BudgetEasy = 40, BandWant = 4, Players = 200, Finalists = 3 }, catalog);
            foreach (var f in o.Checked)
                Debug.Log($"[pm70] probe {f.Candidate.Level.Pieces.Count} pcs timer {f.Candidate.Level.TimerSeconds} v2 {f.V2?.PWin1:0.00} fun {f.Fun.FirstWinRate:0.00} psych: {string.Join("; ", f.Psych)}");
        }
    }
}
