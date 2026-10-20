using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReverseSolver.Core;
using ReverseSolver.Core.Json;
using ReverseSolver.Editing;
using ReverseSolver.Generation;
using UnityEngine;

namespace ReverseSolver.LevelEditor
{
    /* ZeydBot phase, step 1 (Zeyd 2026-10-19): a 30-level set — 10 kolay, 10 orta, 10 zor (zor = D08's
       settings) — in three mixed sessions of 10, every session holding each kind. Written to
       Assets/_Game/Levels/zeydbot.json (levels.json format, ids Z01-Z30 in play order, "zeydbot" block
       with kind and session). The factory and the live set are not touched.

         bash Tools/unity_batch.sh method ReverseSolver.LevelEditor.ZeydSet.Build [-rsZorMin 30] [-rsOrtaMin 12] */
    public static class ZeydSet
    {
        public const string Path = "Assets/_Game/Levels/zeydbot.json";
        static readonly string[] Args = Environment.GetCommandLineArgs();
        internal static string Arg(string k, string d) { int i = Array.IndexOf(Args, k); return i >= 0 && i + 1 < Args.Length ? Args[i + 1] : d; }
        static void Log(string s) => Debug.Log("[zeydset] " + s);

        public static void Build()
        {
            var designs = DesignStore.Parse(File.ReadAllText("Assets/_Game/Levels/designs.json"));
            var look = designs.Find("D06");
            var zor = Collect(double.Parse(Arg("-rsZorMin", "30")), 10, seed =>
            {
                HardSeven.BombCount = 4;
                var r = HardSeven.Try(look, seed, out _);
                return r != null && r.Bombs >= 4 ? (r.Draft, r.Score) : (null, 0);
            }, "zor");
            var catalog = Catalog.From(LevelParser.Parse(File.ReadAllText("Assets/_Game/Levels/levels.json")));
            var orta = Collect(double.Parse(Arg("-rsOrtaMin", "12")), 10, seed =>
            {
                var r = HardFive.Try(seed + 9_000_000u, catalog, out _);
                return r != null ? (r.Draft, -r.Missed * 10 + r.Score) : (null, 0);
            }, "orta");
            // kolay: the redesign finalists Zeyd found too easy (Kolay / Orta slots), ten different slots
            var redesign = RedesignStore.LoadOrNew();
            var kolay = redesign.Entries.Where(e => e.Genre == "kolay" || e.Genre == "orta")
                .GroupBy(e => e.Slot).Select(g => g.First()).OrderBy(e => e.Slot).Take(10)
                .Select(e => e.Draft.Clone()).ToList();
            Write(kolay, orta, zor);
        }

        /* Per level: fairness (the stored solution wins, no valve), new M1 (right / trap first moves: a move
           is right when SmartPlayer still wins after it), M11 (obstacles over the four quarters; bomb >= 2 and
           nails / chains >= 1 from the edge; the edge ring not bare), M12 (one axis <= 60 % of the exits),
           M4 and Monte Carlo. A markdown table to Logs/batch/zeydbot_olcum.md. */
        public static void Report()
        {
            var set = LevelParser.Parse(File.ReadAllText(Path));
            var root = JsonReader.Parse(File.ReadAllText(Path));
            root.TryGet("levels", out var list);
            var rows = new List<string> { "| Bölüm | Oturum | Tür | Tahta / parça | Adil (çözüm, supap) | M1 doğru / tuzak | M2 en çok serbest | M4 | M11 çeyrek / kenar | M12 tek eksen | Bomba / çivi / zincir | Rastgele kazanma |", "|---|---|---|---|---|---|---|---|---|---|---|---|" };
            for (int i = 0; i < set.Levels.Count; i++)
            {
                var l = set.Levels[i];
                list[i].TryGet("zeydbot", out var z); z.TryGet("kind", out var k); z.TryGet("session", out var se);
                var g = new GameSession(l, 1, "rep", new Mulberry32(1)) { ClockOnFirstMove = false };
                foreach (var mv in l.Solution) g.TryExit(mv.Piece, mv.Dir);
                bool fair = g.Outcome == Outcome.Win && g.ValveCount == 0;
                // M1: right / trap among the first moves
                int right = 0, trap = 0;
                var start = new GameSession(l, 1, "rep", new Mulberry32(1)) { ClockOnFirstMove = false };
                foreach (var p in TravelRule.FreePieces(start.Board).Where(p => start.NailAt(p) <= 0))
                {
                    bool ok = false;
                    foreach (var dir in TravelRule.ExitDirs(start.Board, p))
                    {
                        var r = SmartPlayer.PlayAfter(l, new Move(p, dir), 100_000);
                        if (r != null && r.Verdict == SmartVerdict.Won && r.Valves == 0) { ok = true; break; }
                    }
                    if (ok) right++; else trap++;
                }
                var f = HardFactors.Of(l);
                int Edge(Cell c) => Math.Min(Math.Min(c.X, l.Width - 1 - c.X), Math.Min(c.Y, l.Height - 1 - c.Y));
                var obstacles = l.Bombs.Keys.Concat(l.Nails.Keys).Concat(Enumerable.Range(0, l.Pieces.Count).Where(p => l.Pieces[p].Length == 2)).Distinct().ToList();
                var quarters = new HashSet<int>(obstacles.SelectMany(p => l.Pieces[p]).Select(c => (c.X * 2 >= l.Width ? 1 : 0) + (c.Y * 2 >= l.Height ? 2 : 0)));
                bool ringUsed = obstacles.SelectMany(p => l.Pieces[p]).Any(c => Edge(c) <= 1) || l.Sealed.Values.Any(v => v.Length > 0);
                int vertical = l.Solution.Count(m => m.Dir.IsVertical());
                double axis = l.Solution.Count == 0 ? 0 : Math.Max(vertical, l.Solution.Count - vertical) / (double)l.Solution.Count;
                var mc = MonteCarlo.Run(l, 300);
                rows.Add($"| {l.Id} | {se.AsNumber} | {k.AsString} | {l.Width}×{l.Height} / {l.Pieces.Count} | {(fair ? "✓" : "✗")} | {right} / {trap} | {f.MaxFreeAtAll} | {f.LaneRun} | {quarters.Count}/4 / {(ringUsed ? "dolu" : "boş")} | %{axis * 100:0} | {l.Bombs.Count} / {l.Nails.Count} / {l.Pieces.Count(p => p.Length == 2)} | %{mc.WinRate * 100:0} |");
                Log($"{l.Id} ölçüldü");
            }
            Directory.CreateDirectory("Logs/batch");
            File.WriteAllText("Logs/batch/zeydbot_olcum.md", string.Join(Environment.NewLine, rows) + Environment.NewLine, new System.Text.UTF8Encoding(false));
            Log("tablo: Logs/batch/zeydbot_olcum.md");
        }

        /* Fills the zor kind up to ten (keeps the levels already in zeydbot.json) and deals the sessions again. */
        public static void MoreZor()
        {
            var set = LevelParser.Parse(File.ReadAllText(Path));
            var root = JsonReader.Parse(File.ReadAllText(Path));
            root.TryGet("levels", out var list);
            var kinds = new Dictionary<string, List<LevelDraft>> { ["kolay"] = new List<LevelDraft>(), ["orta"] = new List<LevelDraft>(), ["zor"] = new List<LevelDraft>() };
            for (int i = 0; i < set.Levels.Count; i++)
            {
                list[i].TryGet("zeydbot", out var z); z.TryGet("kind", out var k);
                var d = LevelDraft.FromLevel(set.Levels[i]); d.Id = null;
                kinds[k.AsString].Add(d);
            }
            int need = 10 - kinds["zor"].Count;
            Log($"mevcut: kolay {kinds["kolay"].Count}, orta {kinds["orta"].Count}, zor {kinds["zor"].Count}; eksik zor {need}");
            if (need > 0)
            {
                var look = DesignStore.Parse(File.ReadAllText("Assets/_Game/Levels/designs.json")).Find("D06");
                kinds["zor"].AddRange(Collect(double.Parse(Arg("-rsZorMin", "30")), need, seed =>
                {
                    HardSeven.BombCount = 4;
                    var r = HardSeven.Try(look, seed + 50_000_000u, out _);
                    return r != null && r.Bombs >= 4 ? (r.Draft, r.Score) : (null, 0);
                }, "zor+"));
            }
            Write(kinds["kolay"], kinds["orta"], kinds["zor"]);
        }

        static void Write(List<LevelDraft> kolay, List<LevelDraft> orta, List<LevelDraft> zor)
        {
            Log($"kolay {kolay.Count}, orta {orta.Count}, zor {zor.Count}");

            // three sessions of ten, mixed: kinds dealt 4/3/3 in turn, order shuffled with a fixed seed
            var rng = new Mulberry32(20261019);
            var all = kolay.Select(d => (d, kind: "kolay")).Concat(orta.Select(d => (d, kind: "orta"))).Concat(zor.Select(d => (d, kind: "zor"))).ToList();
            var sessions = new List<List<(LevelDraft d, string kind)>> { new List<(LevelDraft, string)>(), new List<(LevelDraft, string)>(), new List<(LevelDraft, string)>() };
            var kindsInOrder = new[] { "kolay", "orta", "zor" };
            for (int k = 0; k < kindsInOrder.Length; k++)
            {
                // each kind starts its deal one session later, so the three sessions get ten each (4/3/3 rotated)
                var of = all.Where(x => x.kind == kindsInOrder[k]).ToList();
                for (int i = 0; i < of.Count; i++) sessions[(i + k) % 3].Add(of[i]);
            }
            var lines = new List<string>();
            int n = 0;
            for (int s = 0; s < 3; s++)
            {
                var order = sessions[s].OrderBy(_ => rng.NextDouble()).ToList();
                foreach (var (d, kind) in order)
                {
                    n++;
                    var c = d.Clone();
                    c.Id = $"Z{n:00}";
                    c.SetNumber(n);
                    c.Genre = kind == "kolay" ? Genre.Kolay : kind == "orta" ? Genre.Orta : Genre.CokZor;
                    // the number is part of the gameplay key, so the stored solution is found again and checked
                    var sp = SmartPlayer.Play(c.ToLevelData());
                    if (sp.Verdict != SmartVerdict.Won || sp.Valves > 0) Log($"Z{n:00}: SmartPlayer {sp.Verdict}, supap {sp.Valves} — çözüm yazılamadı");
                    else c.Solution = sp.Moves.ToList();
                    var w = new JsonWriter();
                    c.WriteJson(w);
                    string level = w.ToString();
                    lines.Add("    " + level.Substring(0, level.Length - 1) + $",\"zeydbot\":{{\"kind\":\"{kind}\",\"session\":{s + 1}}}}}");
                    Log($"Z{n:00} oturum {s + 1} {kind} {c.Width}×{c.Height} {c.ToLevelData().Pieces.Count} parça");
                }
            }
            File.WriteAllText(Path, "{\n  \"format\": 1,\n  \"note\": \"ZeydBot seti (Zeyd 2026-10-19): 10 kolay, 10 orta, 10 zor; 3 oturum x 10, karışık. Canlıya girmez.\",\n  \"levels\": [\n" +
                              string.Join(",\n", lines) + "\n  ]\n}\n", new System.Text.UTF8Encoding(false));
            Log($"yazıldı: {Path} ({n} bölüm)");
        }

        /* v2 (PM 2026-10-20): five "Z12-type" candidates, appended as Z31-Z35 (session 4; Z01-Z30 untouched).
           Z12 against the session-3 hard levels (docs/analiz/zeydbot/RAPOR.md, v2): no traps on the path, start
           slack 1, a timer of about 2.3x the path (Z23-Z26 had 2.7-3.2x) and many misleading pieces (slide but
           cannot leave; Zeyd touched / jammed on 0.46 of his Z12 moves). D08's generator (HardSeven, 4 bombs);
           a board passes with <= 2 trap moves on SmartPlayer's path and start slack <= 1; ranked by misleading
           pieces per step. Timer set to 2.3x the path, rounded to 5 s.

             bash Tools/unity_batch.sh method ReverseSolver.LevelEditor.ZeydSet.Z12Type [-rsZ12Min 60] */
        public static void Z12Type()
        {
            var look = DesignStore.Parse(File.ReadAllText("Assets/_Game/Levels/designs.json")).Find("D06");
            var picks = Collect(double.Parse(Arg("-rsZ12Min", "60")), 5, seed =>
            {
                HardSeven.BombCount = 4;
                var r = HardSeven.Try(look, seed + 70_000_000u, out _);
                if (r == null || r.Bombs < 4) return (null, 0);
                var l = r.Draft.ToLevelData();
                var path = SmartPlayer.Play(l);
                if (path.Verdict != SmartVerdict.Won || path.Valves > 0) return (null, 0);
                var s = new GameSession(l, 1, "z12", new Mulberry32(1)) { ClockOnFirstMove = false };
                var lng = ZeydAnalysis.Long(l);
                var start = ZeydAnalysis.Slacks(s, lng);
                if (start.Count == 0 || start.Min(x => x.slack) > 1) return (null, 0);
                int traps = 0; double mis = 0; int steps = 0;
                foreach (var mv in path.Moves)
                {
                    if (s.IsOver) break;
                    var o = ZeydAnalysis.Options(s, 30_000);
                    if (o.Any(x => !x.right) && ++traps > 2) return (null, 0);
                    mis += ZeydAnalysis.Misleading(s); steps++;
                    s.TryExit(mv.Piece, mv.Dir); lng.TryExit(mv.Piece, mv.Dir);
                }
                var d = r.Draft.Clone();
                d.SetTimer((int)Math.Round(path.Moves.Count * 2.3 / 5) * 5);
                Log($"aday {seed}: tuzaklı {traps}, yanıltıcı/adım {mis / steps:0.00}, süre {d.Timer}");
                return (d, mis / steps);
            }, "z12tipi");

            var text = File.ReadAllText(Path);
            int at = text.LastIndexOf("\n  ]", StringComparison.Ordinal);
            int n = LevelParser.Parse(text).Levels.Count;
            var lines = new List<string>();
            foreach (var d in picks)
            {
                n++;
                var c = d.Clone();
                c.Id = $"Z{n:00}";
                c.SetNumber(n);
                c.Genre = Genre.CokZor;
                var sp = SmartPlayer.Play(c.ToLevelData());
                if (sp.Verdict != SmartVerdict.Won || sp.Valves > 0) { Log($"Z{n:00}: SmartPlayer {sp.Verdict} — atlandı"); n--; continue; }
                c.Solution = sp.Moves.ToList();
                var w = new JsonWriter();
                c.WriteJson(w);
                string level = w.ToString();
                lines.Add("    " + level.Substring(0, level.Length - 1) + ",\"zeydbot\":{\"kind\":\"z12tipi\",\"session\":4}}");
                Log($"Z{n:00} oturum 4 z12tipi, süre {c.Timer}");
            }
            File.WriteAllText(Path, text.Substring(0, at) + ",\n" + string.Join(",\n", lines) + text.Substring(at), new System.Text.UTF8Encoding(false));
            Log($"yazıldı: {Path} (+{lines.Count})");
        }

        /* Seeds in parallel for `minutes`; the `want` best distinct boards. */
        static List<LevelDraft> Collect(double minutes, int want, Func<uint, (LevelDraft d, double score)> make, string name)
        {
            var found = new List<(LevelDraft d, double score)>();
            var gate = new object();
            var end = DateTime.UtcNow.AddMinutes(minutes);
            var beat = DateTime.UtcNow;
            int tried = 0;
            System.Threading.Tasks.Parallel.For(1, 10_000_000, new System.Threading.Tasks.ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 1) }, (i, state) =>
            {
                if (DateTime.UtcNow >= end) { state.Stop(); return; }
                (LevelDraft d, double score) r;
                try { r = make((uint)i); } catch (Exception) { r = (null, 0); }
                lock (gate)
                {
                    tried++;
                    if (r.d != null) found.Add(r);
                    if ((DateTime.UtcNow - beat).TotalMinutes >= 5) { beat = DateTime.UtcNow; Log($"{name}: {tried} aday, {found.Count} uygun"); }
                }
            });
            Log($"{name} bitti: {tried} aday, {found.Count} uygun");
            return found.OrderByDescending(x => x.score).Take(want).Select(x => x.d).ToList();
        }
    }
}
