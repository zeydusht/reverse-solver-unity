using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ReverseSolver.Core;
using ReverseSolver.Editing;
using UnityEngine;

namespace ReverseSolver.LevelEditor
{
    /* Designer loop (PM 2026-10-20, PM_MESAJ_2): the factory's check of the designer agent's drafts. For every
       level in -rsFile (levels.json format; Tools/zeydbot_draft.py writes it from the agent's operations), in
       parallel: solvable by SmartPlayer and with no valve, M1-M9 (HardFactors), M11 / M12, the random player's
       win rate, and step by step along SmartPlayer's path the options, right ones, misleading pieces and the
       tightest slack (where the "aha" sits). Also the bot's right-only paths, so the frozen v1 can score it
       (Tools/zeydbot_score_drafts.py).

       Out, in -rsOut (default Logs/zdesign): <id>.md per draft, taslak_yollar.csv, taslak_ozellik.csv.

         bash Tools/unity_batch.sh method ReverseSolver.LevelEditor.ZeydDesign.Check -rsFile Logs/zdesign/taslak.json [-rsRuns 6] */
    public static class ZeydDesign
    {
        static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
        static void Log(string s) => Debug.Log("[zdesign] " + s);

        public static void Check()
        {
            string file = ZeydSet.Arg("-rsFile", "Logs/zdesign/taslak.json");
            string dir = ZeydSet.Arg("-rsOut", "Logs/zdesign");
            int runs = int.Parse(ZeydSet.Arg("-rsRuns", "6"), Inv);
            Directory.CreateDirectory(dir);
            var set = LevelParser.Parse(File.ReadAllText(file));
            var paths = new Dictionary<string, string>();
            var feats = new Dictionary<string, string>();
            var gate = new object();
            System.Threading.Tasks.Parallel.ForEach(set.Levels, new System.Threading.Tasks.ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 1) }, l =>
            {
                var t0 = DateTime.Now;
                string md, feat;
                var sb = new StringBuilder();
                try
                {
                    (md, feat) = Report(l);
                    if (runs > 0 && feat != null) ZeydBot.PathRows(l, runs, 0.95, sb);   // 0.95: the frozen v1's rate; -rsRuns 0 = quick filter only
                }
                catch (Exception e) { md = $"# {l.Id}\n\nHATA: {e.Message}\n"; feat = null; }
                File.WriteAllText($"{dir}/{l.Id}.md", md, new UTF8Encoding(false));
                lock (gate) { paths[l.Id] = sb.ToString(); if (feat != null) feats[l.Id] = feat; }
                Log($"{l.Id}: {(DateTime.Now - t0).TotalSeconds:0} s");
            });
            File.WriteAllText($"{dir}/taslak_yollar.csv", "level_id,run,adim,kalan_parca,secenek,dogru,en_dar_pay,koruyan,koruyan_secildi,yaniltici,dig\n" +
                string.Concat(set.Levels.Select(l => paths.TryGetValue(l.Id, out var p) ? p : "")), new UTF8Encoding(false));
            File.WriteAllText($"{dir}/taslak_ozellik.csv", "level_id,tur,sure_limiti,en_kisa_cozum,adil,rastgele_kazanma,yol_tuzakli_hamle,yol_zorunlu,yol_ort_secenek,yaniltici_adim,baslangic_en_dar_pay\n" +
                string.Concat(set.Levels.Select(l => feats.TryGetValue(l.Id, out var f) ? f : "")), new UTF8Encoding(false));
            Log($"yazıldı: {dir} ({set.Levels.Count} taslak)");
        }

        /* The chosen drafts (-rsFile, in order) appended to zeydbot.json as the next Z ids, session 4; the number is
           part of the gameplay key, so the solution is found again after SetNumber. Z01-Z30 are not touched.

             bash Tools/unity_batch.sh method ReverseSolver.LevelEditor.ZeydDesign.Publish -rsFile Logs/zdesign/secilen.json */
        public static void Publish()
        {
            var drafts = LevelParser.Parse(File.ReadAllText(ZeydSet.Arg("-rsFile", "Logs/zdesign/secilen.json")));
            var text = File.ReadAllText(ZeydSet.Path);
            int at = text.LastIndexOf("\n  ]", StringComparison.Ordinal);
            int n = LevelParser.Parse(text).Levels.Count;
            var lines = new List<string>();
            foreach (var l in drafts.Levels)
            {
                var c = LevelDraft.FromLevel(l);
                string from = l.Id;
                c.Id = $"Z{++n:00}";
                c.SetNumber(n);
                c.Genre = Genre.CokZor;
                var sp = SmartPlayer.Play(c.ToLevelData());
                if (sp.Verdict != SmartVerdict.Won || sp.Valves > 0) throw new Exception($"{from} -> {c.Id}: SmartPlayer {sp.Verdict}, supap {sp.Valves}");
                c.Solution = sp.Moves.ToList();
                var w = new ReverseSolver.Core.Json.JsonWriter();
                c.WriteJson(w);
                string level = w.ToString();
                lines.Add("    " + level.Substring(0, level.Length - 1) + $",\"zeydbot\":{{\"kind\":\"tasarim\",\"session\":4,\"taslak\":\"{from}\"}}}}");
                Log($"{from} -> {c.Id}, çözüm {sp.Moves.Count} hamle");
            }
            File.WriteAllText(ZeydSet.Path, text.Substring(0, at) + ",\n" + string.Join(",\n", lines) + text.Substring(at), new UTF8Encoding(false));
            Log($"yazıldı: {ZeydSet.Path} (+{lines.Count})");
        }

        /* -rsTightFuse 1: a draft that loses to a bomb is solved with every fuse long; when that wins, each bomb's
           fuse becomes its exit step on that solution + 1 (Z12's rule, docs/analiz/zeydbot/Z12_TARIFI.md 3). */
        static LevelData TightFuses(LevelData l, StringBuilder md)
        {
            var d = LevelDraft.FromLevel(l);
            foreach (var b in l.Bombs) d.SetBomb(l.Pieces[b.Key][0], LevelDraft.MaxCount);
            var sp = SmartPlayer.Play(d.ToLevelData());
            if (sp.Verdict != SmartVerdict.Won || sp.Valves > 0) { md.Append("Fitil ayarı: uzun fitille de çözülmedi.\n\n"); return l; }
            foreach (var b in l.Bombs)
            {
                int step = sp.Moves.FindIndex(m => m.Piece == b.Key) + 1;
                d.SetBomb(l.Pieces[b.Key][0], step + 1);
                md.Append($"Fitil ayarı: bomba parça {b.Key} {b.Value} → {step + 1} (çözümde {step}. hamle)\n");
            }
            md.Append("\n");
            return d.ToLevelData();
        }

        static (string md, string feat) Report(LevelData l)
        {
            var sp = SmartPlayer.Play(l);
            bool fair = sp.Verdict == SmartVerdict.Won && sp.Valves == 0;
            var pre = new StringBuilder();
            if (!fair && ZeydSet.Arg("-rsTightFuse", "0") == "1" && l.Bombs.Count > 0)
            {
                l = TightFuses(l, pre);
                sp = SmartPlayer.Play(l);
                fair = sp.Verdict == SmartVerdict.Won && sp.Valves == 0;
                if (fair) File.WriteAllText($"{ZeydSet.Arg("-rsOut", "Logs/zdesign")}/{l.Id}.fitil.json", string.Join(",", l.Bombs.Select(b => $"{b.Key}:{b.Value}")));
            }
            var md = new StringBuilder(pre + $"# {l.Id}\n\n{l.Width}×{l.Height}, {l.Pieces.Count} parça, süre {l.TimerSeconds} s, bomba {l.Bombs.Count}, çivi {l.Nails.Count}, zincir {l.Pieces.Count(p => p.Length == 2)}\n\n");
            md.Append($"**Çözülebilir (SmartPlayer, supap 0): {(fair ? "EVET" : $"HAYIR ({sp.Verdict}, supap {sp.Valves})")}**, çözüm {sp.Moves.Count} hamle, süre / çözüm = {(double)l.TimerSeconds / Math.Max(1, sp.Moves.Count):0.00}\n\n");
            if (!fair) return (md.ToString(), null);

            var mc = MonteCarlo.Run(l, 300);
            md.Append($"Rastgele oyuncu kazanma: %{mc.WinRate * 100:0} (300 koşu)\n\n");
            var f = HardFactors.Of(l);
            md.Append("| Ölçüt | Değer | Hedef (Çok zor) | Tuttu |\n|---|---|---|---|\n");
            foreach (var m in f.Metrics()) md.Append($"| {m.name} | {m.value} | {m.target} | {(m.ok ? "✓" : "✗")} |\n");
            int Edge(Cell c) => Math.Min(Math.Min(c.X, l.Width - 1 - c.X), Math.Min(c.Y, l.Height - 1 - c.Y));
            var obstacles = l.Bombs.Keys.Concat(l.Nails.Keys).Concat(Enumerable.Range(0, l.Pieces.Count).Where(p => l.Pieces[p].Length == 2)).Distinct().ToList();
            var quarters = new HashSet<int>(obstacles.SelectMany(p => l.Pieces[p]).Select(c => (c.X * 2 >= l.Width ? 1 : 0) + (c.Y * 2 >= l.Height ? 2 : 0)));
            int vertical = sp.Moves.Count(m => m.Dir.IsVertical());
            double axis = sp.Moves.Count == 0 ? 0 : Math.Max(vertical, sp.Moves.Count - vertical) / (double)sp.Moves.Count;
            md.Append($"| M11 Engel çeyrekleri | {quarters.Count}/4 | 4/4 | {(quarters.Count == 4 ? "✓" : "✗")} |\n");
            md.Append($"| M12 Tek eksen payı | %{axis * 100:0} | ≤ %60 | {(axis <= .6 ? "✓" : "✗")} |\n\n");

            // step by step along SmartPlayer's path: where the choices, traps and misleading pieces are
            var s = new GameSession(l, 1, "taslak", new Mulberry32(1)) { ClockOnFirstMove = false };
            var lng = ZeydAnalysis.Long(l);
            var start = ZeydAnalysis.Slacks(s, lng);
            int traps = 0, forced = 0, n = 0; double opt = 0, mis = 0;
            var line = new StringBuilder();
            foreach (var mv in sp.Moves)
            {
                if (s.IsOver) break;
                var o = ZeydAnalysis.Options(s, 30_000);
                int r = o.Count(x => x.right), mi = ZeydAnalysis.Misleading(s);
                var sl = ZeydAnalysis.Slacks(s, lng);
                if (r < o.Count) traps++;
                if (o.Count == 1) forced++;
                n++; opt += o.Count; mis += mi;
                // one cell per step: options/right, misleading, slack; "T" = a trap is on offer, "!" = forced
                line.Append($"{n}:{o.Count}/{r}{(r < o.Count ? "T" : o.Count == 1 ? "!" : "")} y{mi}{(sl.Count == 0 ? "" : " p" + sl.Min(x => x.slack))} · ");
                s.TryExit(mv.Piece, mv.Dir); lng.TryExit(mv.Piece, mv.Dir);
            }
            md.Append($"Yol: tuzaklı hamle {traps}, zorunlu (tek seçenek) {forced}, ortalama seçenek {opt / Math.Max(1, n):0.00}, yanıltıcı / adım {mis / Math.Max(1, n):0.00}, başlangıç en dar pay {(start.Count == 0 ? "—" : start.Min(x => x.slack) < 0 ? "ölçülemedi (kazı araması bütçeyi aştı)" : start.Min(x => x.slack).ToString())}\n\n");
            md.Append("Adım adım (adım:seçenek/doğru, T tuzak var, ! zorunlu, y yanıltıcı, p en dar pay):\n\n" + line + "\n");
            string feat = $"{l.Id},taslak,{l.TimerSeconds},{sp.Moves.Count},1,{mc.WinRate.ToString("0.000", Inv)},{traps},{forced},{(opt / Math.Max(1, n)).ToString("0.000", Inv)},{(mis / Math.Max(1, n)).ToString("0.000", Inv)},{(start.Count == 0 ? "" : start.Min(x => x.slack).ToString())}\n";
            return (md.ToString(), feat);
        }
    }
}
