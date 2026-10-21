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
            File.WriteAllText($"{dir}/taslak_ozellik.csv", "level_id,tur,sure_limiti,en_kisa_cozum,adil,rastgele_kazanma,yol_tuzakli_hamle,yol_zorunlu,yol_ort_secenek,yaniltici_adim,baslangic_en_dar_pay,m13_anahtar,m13_ikinci_yari,m13_art_arda,m14_supurme,m14_yon,m14_duz_seritten,m14_imza,m15_kalan_pay\n" +
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

        /* SET40 v1 (PM_MESAJ_4): Tools/set40_plan.py's slots written as redesign.json, one finalist per slot, for
           Zeyd's approval queue (?set=redesign). Sources: redesign.json (R) and zeydbot.json (Z). The number is part
           of the gameplay key, so each solution is found again after SetNumber. The old 103 drafts stay in git.

             bash Tools/unity_batch.sh method ReverseSolver.LevelEditor.ZeydDesign.WriteSet40 */
        public static void WriteSet40()
        {
            var plan = ReverseSolver.Core.Json.JsonReader.Parse(File.ReadAllText(ZeydSet.Arg("-rsFile", "Logs/zdesign/set40_plan.json")));
            var r = LevelParser.Parse(File.ReadAllText(Generation.RedesignStore.Path)).Levels.ToDictionary(x => x.Id);
            var z = LevelParser.Parse(File.ReadAllText(ZeydSet.Path)).Levels.ToDictionary(x => x.Id);
            var rawR = ReverseSolver.Core.Json.JsonReader.Parse(File.ReadAllText(Generation.RedesignStore.Path));
            rawR.TryGet("levels", out var rl);
            var genreOf = new Dictionary<string, string>();
            foreach (var n in rl.Items) { n.TryGet("id", out var id); n.TryGet("genre", out var g); if (g != null && g.Kind == ReverseSolver.Core.Json.JsonKind.String) genreOf[id.AsString] = g.AsString; }
            var store = new Generation.RedesignStore();
            foreach (var p in plan.Items)
            {
                p.TryGet("slot", out var slotN); p.TryGet("src", out var src); p.TryGet("id", out var idN); p.TryGet("idea", out var idea); p.TryGet("genre", out var gN); p.TryGet("target", out var tN);
                int slot = (int)slotN.AsNumber;
                var l = (src.AsString == "R" ? r : z)[idN.AsString];
                var d = LevelDraft.FromLevel(l);
                d.Id = $"S{slot:00}";
                d.SetNumber(slot);
                // boosters follow the new slot (unlock at their introduction, one in stock from then on)
                foreach (var b in LevelDraft.BoosterIds)
                {
                    int at = Generation.SlotSpec.IntroducedAt[b];
                    d.SetUnlock(b, at); d.SetBooster(b, slot >= at ? 1 : 0);
                }
                // obstacles not before their introduction slot
                var ld = d.ToLevelData();
                var need = new List<string>();
                if (ld.Nails.Count > 0 && slot < Generation.SlotSpec.IntroducedAt[Generation.SlotSpec.Nail]) need.Add("çivi");
                if (ld.Sealed.Values.Any(v => v.Length > 0) && slot < Generation.SlotSpec.IntroducedAt[Generation.SlotSpec.Seal]) need.Add("mühür");
                if (ld.Pieces.Any(pc => pc.Length == 2) && slot < Generation.SlotSpec.IntroducedAt[Generation.SlotSpec.Chain]) need.Add("zincir");
                if (ld.Bombs.Count > 0 && slot < Generation.SlotSpec.IntroducedAt[Generation.SlotSpec.Bomb]) need.Add("bomba");
                if (need.Count > 0) throw new Exception($"slot {slot} {idN.AsString}: tanıtımından önce {string.Join(", ", need)}");
                string gs = genreOf.TryGetValue(idN.AsString, out var g0) ? g0 : gN.AsString;
                if (GenreExt.TryParse(gs, out var genre)) d.Genre = genre;
                var sp = SmartPlayer.Play(d.ToLevelData());
                if (sp.Verdict != SmartVerdict.Won || sp.Valves > 0) throw new Exception($"slot {slot} {idN.AsString}: SmartPlayer {sp.Verdict}, supap {sp.Valves}");
                d.Solution = sp.Moves.ToList();
                var e = new Generation.RedesignStore.Entry { Draft = d, Slot = slot, Candidate = idN.AsString, Genre = gs, Aha = idea.AsString, Psych = $"SET40 v1, hedef puan {tN.AsNumber}" };
                store.Entries.Add(e);
                Log($"slot {slot}: {idN.AsString} ({gs}), çözüm {sp.Moves.Count}");
            }
            store.Save();
            Log($"yazıldı: {Generation.RedesignStore.Path} ({store.Entries.Count} slot)");
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
            string m13 = ",,,,,,,";
            var line = new StringBuilder();
            var key = new List<int>();                                   // M13: steps that are key moments
            var exits = new List<(int piece, Dir dir, int lane, bool bomb, bool chain)>();
            foreach (var mv in sp.Moves)
            {
                if (s.IsOver) break;
                var c0 = l.Pieces[mv.Piece][0];
                exits.Add((mv.Piece, mv.Dir, mv.Dir.IsVertical() ? c0.X : c0.Y, l.Bombs.ContainsKey(mv.Piece), l.Pieces[mv.Piece].Length == 2));
                var o = ZeydAnalysis.Options(s, 30_000);
                int r = o.Count(x => x.right), mi = ZeydAnalysis.Misleading(s);
                var sl = ZeydAnalysis.Slacks(s, lng);
                if (r < o.Count) traps++;
                if (o.Count == 1) forced++;
                // M13 (PM 2026-10-21): many pieces free or looking free (>= 4), one move that moves the solution on
                if (o.Count + mi >= 4 && r == 1) key.Add(n + 1);
                n++; opt += o.Count; mis += mi;
                // one cell per step: options/right, misleading, slack; "T" = a trap is on offer, "!" = forced
                line.Append($"{n}:{o.Count}/{r}{(r < o.Count ? "T" : o.Count == 1 ? "!" : "")}{(o.Count + mi >= 4 && r == 1 ? "A" : "")} y{mi}{(sl.Count == 0 ? "" : " p" + sl.Min(x => x.slack))} · ");
                s.TryExit(mv.Piece, mv.Dir); lng.TryExit(mv.Piece, mv.Dir);
            }
            md.Append($"Yol: tuzaklı hamle {traps}, zorunlu (tek seçenek) {forced}, ortalama seçenek {opt / Math.Max(1, n):0.00}, yanıltıcı / adım {mis / Math.Max(1, n):0.00}, başlangıç en dar pay {(start.Count == 0 ? "—" : start.Min(x => x.slack) < 0 ? "ölçülemedi (kazı araması bütçeyi aştı)" : start.Min(x => x.slack).ToString())}\n\n");
            md.Append("Adım adım (adım:seçenek/doğru, T tuzak var, ! zorunlu, A anahtar an, y yanıltıcı, p en dar pay):\n\n" + line + "\n\n");

            // M13: key moments, how many in the second half, the longest run of them with <= 3 easy moves between
            int half = key.Count(k => k > n / 2), runs = 0, best = 0;
            for (int i = 0; i < key.Count; i++) { runs = i > 0 && key[i] - key[i - 1] <= 4 ? runs + 1 : 1; best = Math.Max(best, runs); }
            md.Append("| Ölçüt | Değer | Hedef | Tuttu |\n|---|---|---|---|\n");
            md.Append($"| M13 Anahtar an (görünür ≥ 4, ilerleten 1) | {key.Count} (ikinci yarıda {half}, art arda en uzun {best}) | orta-son yarıda art arda 3–6 | {(best >= 3 && half >= 3 ? "✓" : "✗")} |\n");
            md.Append($"| M13 adımları | {string.Join(", ", key)} | | |\n");
            // M14: per bomb, the 8 exits before it: longest same lane + same direction run, directions used;
            // bombs / chains leaving one after another along one straight lane
            var sig = new List<string>(); int sweepMax = 0, dirsMin = 4; bool straight = false;
            for (int i = 0; i < exits.Count; i++)
            {
                if (!exits[i].bomb) continue;
                var win = exits.Skip(Math.Max(0, i - 8)).Take(Math.Min(8, i)).ToList();
                int run = 0, bestRun = 0;
                for (int j = 0; j < win.Count; j++) { run = j > 0 && win[j].dir == win[j - 1].dir && win[j].lane == win[j - 1].lane ? run + 1 : 1; bestRun = Math.Max(bestRun, run); }
                int dirs = win.Select(w => w.dir).Distinct().Count();
                sweepMax = Math.Max(sweepMax, bestRun); dirsMin = Math.Min(dirsMin, dirs);
                if (i > 0 && (exits[i - 1].bomb || exits[i - 1].chain) && exits[i - 1].dir == exits[i].dir && exits[i - 1].lane == exits[i].lane) straight = true;
                var main = win.GroupBy(w => w.dir).OrderByDescending(g => g.Count()).FirstOrDefault();
                sig.Add($"{(main == null ? "-" : main.Key.ToString())}{bestRun}>{exits[i].dir}");
            }
            if (l.Bombs.Count > 0)
            {
                md.Append($"| M14 Bombadan önce aynı şerit + yön art arda (en çok) | {sweepMax} | ≤ 2 | {(sweepMax <= 2 ? "✓" : "✗")} |\n");
                md.Append($"| M14 Bomba yolunu açan yön sayısı (en az) | {dirsMin} | ≥ 2 | {(dirsMin >= 2 ? "✓" : "✗")} |\n");
                md.Append($"| M14 Bomba/zincir aynı düz şeritten art arda | {(straight ? "var" : "yok")} | yok | {(straight ? "✗" : "✓")} |\n");
                md.Append($"| M14 Desen imzası (bomba başına: baskın süpürme yönü + uzunluk > bombanın çıkışı) | {string.Join(" ", sig)} | setteki zorlarda farklı | |\n");
            }
            // M15: Zeyd's pace on the Z12-type levels he won (Z31, Z33, Z35: 101 + 96 + 111 s over 3 x 54 moves = 1.90 s
            // a move, ham_plays.csv); the share of the timer he would have left, target 10-20 %
            double pace = (101 + 96 + 111) / (3.0 * 54), leftShare = 1 - pace * sp.Moves.Count / Math.Max(1, l.TimerSeconds);
            md.Append($"| M15 Kalan süre payı (Zeyd'in Z12 tipi hızı 1,90 s/hamle) | %{leftShare * 100:0} | %10–20 | {(leftShare >= .10 && leftShare <= .20 ? "✓" : "✗")} |\n");
            m13 = $"{key.Count},{half},{best},{sweepMax},{dirsMin},{(straight ? 1 : 0)},{string.Join(" ", sig)},{leftShare.ToString("0.000", Inv)}";
            string feat = $"{l.Id},taslak,{l.TimerSeconds},{sp.Moves.Count},1,{mc.WinRate.ToString("0.000", Inv)},{traps},{forced},{(opt / Math.Max(1, n)).ToString("0.000", Inv)},{(mis / Math.Max(1, n)).ToString("0.000", Inv)},{(start.Count == 0 ? "" : start.Min(x => x.slack).ToString())},{m13}\n";
            return (md.ToString(), feat);
        }
    }
}
