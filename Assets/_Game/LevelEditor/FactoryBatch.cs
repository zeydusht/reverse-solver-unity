using System;
using System.IO;
using System.Linq;
using ReverseSolver.Core;
using ReverseSolver.Editing;
using ReverseSolver.Generation;
using UnityEngine;

namespace ReverseSolver.LevelEditor
{
    /* Batchmode twin of the Fabrika tab (lost in G7, back 2026-10-17). Lines tagged [fabrika]
       are what Tools/unity_batch.sh shows; the full log stays in Logs/batch.

         bash Tools/unity_batch.sh method ReverseSolver.LevelEditor.FactoryBatch.Run -rsSlots 1-40 -rsDepth 1 [-rsRun name]
         bash Tools/unity_batch.sh method ReverseSolver.LevelEditor.FactoryBatch.Rescore [-rsRun name]
         bash Tools/unity_batch.sh method ReverseSolver.LevelEditor.FactoryBatch.Rounds -rsSlots 10,28,34 [-rsShort]
              [-rsDepth 2] [-rsRounds 3] [-rsSeed 2] [-rsRelax] [-rsRun name]

       Rounds: -rsShort adds every slot left with fewer than two finalists; -rsRelax adds the
       relaxed round after the others (PRODUCT.md: only after three rounds). */
    public static class FactoryBatch
    {
        static readonly string[] Args = Environment.GetCommandLineArgs();
        static string Arg(string k, string d) { int i = Array.IndexOf(Args, k); return i >= 0 && i + 1 < Args.Length ? Args[i + 1] : d; }
        static bool Flag(string k) => Array.IndexOf(Args, k) >= 0;
        static string RunName(string prefix) => Arg("-rsRun", prefix + DateTime.Now.ToString("yyyyMMdd-HHmm"));
        static void Log(string line) { foreach (var l in line.Split('\n')) Debug.Log("[fabrika] " + l.TrimEnd()); }

        public static void Run()
        {
            var slots = FactoryRunner.ParseSlots(Arg("-rsSlots", "1-40"));
            int depth = int.Parse(Arg("-rsDepth", "1"));
            string run = RunName("");
            Log($"koşu {run}: slotlar {string.Join(",", slots)}, derinlik {depth}");
            FactoryRunner.Run(slots, FactoryWindow.OptionsFor(depth), run, (done, total, line) => { Log($"{done}/{total} {line}"); return true; });
            Log("bitti, rapor " + FactoryReport.Path);
        }

        public static void Rescore()
        {
            string run = RunName("rescore-");
            var text = FactoryRunner.Rescore(run);
            foreach (var l in text.Split('\n').Where(l => !l.StartsWith("|"))) Log(l);
            Log($"tablo: {FactoryRunner.RunsDir}/{run}/rescore.md");
        }

        /* Finalists whose stored solution was dropped (a timer or stock change invalidates it) get
           SmartPlayer's winning order back; a level it cannot win clean is only reported. */
        public static void FillSolutions()
        {
            var store = RedesignStore.LoadOrNew();
            int filled = 0, failed = 0;
            foreach (var e in store.Entries.Where(e => e.Draft.Solution.Count == 0))
            {
                var r = ReverseSolver.Core.SmartPlayer.Play(e.Draft.ToLevelData());
                if (r.Verdict == ReverseSolver.Core.SmartVerdict.Won && r.Valves == 0) { e.Draft.Solution = new System.Collections.Generic.List<ReverseSolver.Core.Move>(r.Moves); filled++; }
                else { failed++; Log($"çözüm yok: {e.Candidate} {r}"); }
            }
            store.Save();
            Log($"çözüm dolduruldu: {filled}, başarısız {failed}, toplam {store.Entries.Count}");
        }

        /* Zeyd's single hard Çok zor level (HardDesign): seeds in parallel until -rsMin minutes,
           a progress line every 5 minutes, the best one saved to designs.json with a D id. */
        public static void HardOne()
        {
            double minutes = double.Parse(Arg("-rsMin", "20"));
            var catalog = Catalog.From(LevelParser.Parse(File.ReadAllText("Assets/_Game/Levels/levels.json")));
            var start = DateTime.UtcNow; var end = start.AddMinutes(minutes); var beat = start;
            HardDesign.Result best = null; int tried = 0, passed = 0;
            var why = new System.Collections.Generic.Dictionary<string, int>();
            var gate = new object();
            System.Threading.Tasks.Parallel.For(1, 10_000_000, new System.Threading.Tasks.ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 1) }, (i, state) =>
            {
                if (DateTime.UtcNow >= end) { state.Stop(); return; }
                var r = HardDesign.Try((uint)i, catalog, out string w);
                lock (gate)
                {
                    tried++;
                    if (r != null) { if (r.Miss == 0) passed++; if (best == null || HardDesign.Better(r, best)) best = r; }
                    else { if (tried <= 3) Log("  örnek: " + w); string k = (w ?? "?").StartsWith("açık dikiş") || (w ?? "").StartsWith("kazma yolu") || (w ?? "").StartsWith("ilk hamlede") ? w : System.Text.RegularExpressions.Regex.Replace(w ?? "?", @"[0-9,.]+", "#"); why.TryGetValue(k, out int n); why[k] = n + 1; }
                    if ((DateTime.UtcNow - beat).TotalMinutes >= 4) { beat = DateTime.UtcNow; Log($"ilerleme: {(beat - start).TotalMinutes:0} dk, {tried} aday, {passed} şartları geçti, en iyi puan {(best == null ? "—" : best.Score.ToString("0.0"))}, en az kaçırma {(best == null ? "—" : best.Miss.ToString("0.0"))}"); }
                }
            });
            Log($"bitti: {tried} aday, {passed} şartları geçti, {(DateTime.UtcNow - start).TotalMinutes:0} dk");
            foreach (var kv in why.OrderByDescending(k => k.Value).Take(20)) Log($"  elenme: {kv.Key} × {kv.Value}");
            if (best == null) { Log("adil aday bulunamadı"); return; }
            Log(best.Miss == 0 ? "bütün şartları tutan aday bulundu" : $"bütün şartları tutan yok; en yakın aday (kaçırma {best.Miss:0.0}) kaydediliyor");
            const string path = "Assets/_Game/Levels/designs.json";
            var store = DesignStore.Parse(File.Exists(path) ? File.ReadAllText(path) : "");
            best.Draft.Id = null;
            var saved = store.Save(best.Draft);
            File.WriteAllText(path, store.ToJson(), new System.Text.UTF8Encoding(false));
            Log("kaydedildi: " + saved.Id);
            Log(best.Report());
        }

        /* Zeyd's second hard level (HardFive): seeds in parallel until -rsMin minutes, a progress line
           every 4 minutes; the one holding the most of the five factors saved to designs.json. */
        public static void HardFiveRun()
        {
            double minutes = double.Parse(Arg("-rsMin", "30"));
            var catalog = Catalog.From(LevelParser.Parse(File.ReadAllText("Assets/_Game/Levels/levels.json")));
            var start = DateTime.UtcNow; var end = start.AddMinutes(minutes); var beat = start;
            HardFive.Result best = null; int tried = 0, full = 0;
            var why = new System.Collections.Generic.Dictionary<string, int>();
            var gate = new object();
            System.Threading.Tasks.Parallel.For(1, 10_000_000, new System.Threading.Tasks.ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 1) }, (i, state) =>
            {
                if (DateTime.UtcNow >= end) { state.Stop(); return; }
                var r = HardFive.Try((uint)i, catalog, out string w);
                lock (gate)
                {
                    tried++;
                    if (r != null) { if (r.Missed == 0) full++; if (best == null || HardFive.Better(r, best)) best = r; }
                    else { string k = System.Text.RegularExpressions.Regex.Replace(w ?? "?", @"[0-9,.]+", "#"); why.TryGetValue(k, out int n); why[k] = n + 1; }
                    if ((DateTime.UtcNow - beat).TotalMinutes >= 4) { beat = DateTime.UtcNow; Log($"ilerleme: {(beat - start).TotalMinutes:0} dk, {tried} aday, {full} hepsini tuttu, en az kaçırılan {(best == null ? "—" : best.Missed.ToString())}"); }
                }
            });
            Log($"bitti: {tried} aday, {full} hepsini tuttu, {(DateTime.UtcNow - start).TotalMinutes:0} dk");
            foreach (var kv in why.OrderByDescending(k => k.Value).Take(8)) Log($"  elenme: {kv.Key} × {kv.Value}");
            if (best == null) { Log("adil aday bulunamadı"); return; }
            const string path = "Assets/_Game/Levels/designs.json";
            var store = DesignStore.Parse(File.Exists(path) ? File.ReadAllText(path) : "");
            best.Draft.Id = null;
            var saved = store.Save(best.Draft);
            File.WriteAllText(path, store.ToJson(), new System.Text.UTF8Encoding(false));
            Log($"kaydedildi: {saved.Id} (tohum {best.Seed}, kaçırılan {best.Missed}; {best.Params})");
            foreach (var row in best.Factors.Rows()) Log($"  {row.name}: {row.value} (hedef {row.target}) {(row.ok ? "✓" : "✗")}");
            Log($"  yol: {best.Factors.Source}, {best.Factors.Moves} hamle, süre {best.Level.TimerSeconds} sn, {best.Level.Pieces.Count} parça");
        }

        /* D06: the hardest fair level by M1-M9 (HardSix). */
        public static void HardSixRun()
        {
            double minutes = double.Parse(Arg("-rsMin", "30"));
            var catalog = Catalog.From(LevelParser.Parse(File.ReadAllText("Assets/_Game/Levels/levels.json")));
            var start = DateTime.UtcNow; var end = start.AddMinutes(minutes); var beat = start;
            HardSix.Result best = null; int tried = 0, full = 0;
            var why = new System.Collections.Generic.Dictionary<string, int>();
            var gate = new object();
            System.Threading.Tasks.Parallel.For(1, 10_000_000, new System.Threading.Tasks.ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 1) }, (i, state) =>
            {
                if (DateTime.UtcNow >= end) { state.Stop(); return; }
                var r = HardSix.Try((uint)i, catalog, out string w);
                lock (gate)
                {
                    tried++;
                    if (r != null) { if (r.Held == 13) full++; if (best == null || HardSix.Better(r, best)) best = r; }
                    else { string k = System.Text.RegularExpressions.Regex.Replace(w ?? "?", @"[0-9,.]+", "#"); why.TryGetValue(k, out int n); why[k] = n + 1; }
                    if ((DateTime.UtcNow - beat).TotalMinutes >= 4) { beat = DateTime.UtcNow; Log($"ilerleme: {(beat - start).TotalMinutes:0} dk, {tried} aday, {full} hepsini tuttu, en çok tutan {(best == null ? "—" : best.Held.ToString())}"); }
                }
            });
            Log($"bitti: {tried} aday, {full} hepsini tuttu, {(DateTime.UtcNow - start).TotalMinutes:0} dk");
            foreach (var kv in why.OrderByDescending(k => k.Value).Take(8)) Log($"  elenme: {kv.Key} × {kv.Value}");
            if (best == null) { Log("adil aday bulunamadı"); return; }
            const string path = "Assets/_Game/Levels/designs.json";
            var store = DesignStore.Parse(File.Exists(path) ? File.ReadAllText(path) : "");
            best.Draft.Id = null;
            var saved = store.Save(best.Draft);
            File.WriteAllText(path, store.ToJson(), new System.Text.UTF8Encoding(false));
            Log($"kaydedildi: {saved.Id} (tohum {best.Seed}, tutan {best.Held}/13; {best.Params})");
            foreach (var row in best.Factors.Metrics()) Log($"  {row.name}: {row.value} (hedef {row.target}) {(row.ok ? "✓" : "✗")}");
            Log($"  yol: {best.Factors.Source}, {best.Factors.Moves} hamle, süre {best.Level.TimerSeconds} sn, {best.Level.Pieces.Count} parça");
        }

        /* D06 from D05 by Zeyd's hand rules (HardSix.FromD05), saved as the next design. */
        public static void D06()
        {
            const string path = "Assets/_Game/Levels/designs.json";
            var store = DesignStore.Parse(File.ReadAllText(path));
            var log = new System.Collections.Generic.List<string>();
            var d = HardSix.FromD05(store.Find("D05"), log);
            foreach (var l in log) Log(l);
            if (d == null) { Log("D06 çıkmadı"); return; }
            d.Id = store.Find("D06") == null ? "D06" : null;
            var saved = store.Save(d);
            File.WriteAllText(path, store.ToJson(), new System.Text.UTF8Encoding(false));
            Log("kaydedildi: " + saved.Id);
        }

        /* D07 (HardSeven) on D06: seeds in parallel for -rsMin minutes, the best saved as D07. */
        public static void D07()
        {
            const string path = "Assets/_Game/Levels/designs.json";
            double minutes = double.Parse(Arg("-rsMin", "20"));
            var store = DesignStore.Parse(File.ReadAllText(path));
            var d06 = store.Find("D06");
            HardSeven.BombCount = int.Parse(Arg("-rsBombs", "3"));
            string newId = Arg("-rsId", "D07");
            var start = DateTime.UtcNow; var end = start.AddMinutes(minutes); var beat = start;
            HardSeven.Result best = null; int tried = 0, ok = 0;
            var why = new System.Collections.Generic.Dictionary<string, int>();
            var gate = new object();
            System.Threading.Tasks.Parallel.For(1, 10_000_000, new System.Threading.Tasks.ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 1) }, (i, state) =>
            {
                if (DateTime.UtcNow >= end) { state.Stop(); return; }
                var r = HardSeven.Try(d06, (uint)i, out string w);
                lock (gate)
                {
                    tried++;
                    if (r != null && r.Bombs >= HardSeven.BombCount) { ok++; if (best == null || r.Score > best.Score) best = r; }
                    else { string k = System.Text.RegularExpressions.Regex.Replace(w ?? "?", @"[0-9]+", "#"); why.TryGetValue(k, out int n); why[k] = n + 1; }
                    if ((DateTime.UtcNow - beat).TotalMinutes >= 4) { beat = DateTime.UtcNow; Log($"ilerleme: {(beat - start).TotalMinutes:0} dk, {tried} aday, {ok} adil, en iyi açılış {(best == null ? "—" : best.Factors.FreeAtStart.ToString())}"); }
                }
            });
            Log($"bitti: {tried} aday, {ok} adil");
            foreach (var kv in why.OrderByDescending(k => k.Value).Take(6)) Log($"  elenme: {kv.Key} × {kv.Value}");
            if (best == null) { Log("D07 çıkmadı"); return; }
            best.Draft.Id = store.Find(newId) == null ? newId : null;
            var saved = store.Save(best.Draft);
            File.WriteAllText(path, store.ToJson(), new System.Text.UTF8Encoding(false));
            var f = best.Factors;
            Log($"kaydedildi: {saved.Id} (tohum {best.Seed})");
            Log($"  açılışta serbest {f.FreeAtStart}; oyun boyunca en çok serbest {f.MaxFreeAtAll}; ilk %70'te en çok {f.MaxFreeEarly}; bir hamlede açılan en çok {f.Cascade}");
            Log($"  bomba {best.Bombs}, fitil payları {string.Join("/", best.FuseSlack.Select(x => "+" + x))}; çivi {best.Nails}; zincir {best.Chains}; duvarlı şerit {best.WallLanes}/{best.Lanes}");
            Log($"  kazma yolu (en derin) {f.Dig}; çivisiz {f.DigWithoutNails}; süre {best.Draft.Timer} sn");
        }

        /* D07, solution first (HardEight): seeds in parallel for -rsMin minutes; the most M rows held saved as D07. */
        public static void D07b()
        {
            const string path = "Assets/_Game/Levels/designs.json";
            double minutes = double.Parse(Arg("-rsMin", "20"));
            var store = DesignStore.Parse(File.ReadAllText(path));
            var look = store.Find("D06");
            var start = DateTime.UtcNow; var end = start.AddMinutes(minutes); var beat = start;
            HardEight.Result best = null; int tried = 0, ok = 0, full = 0;
            var why = new System.Collections.Generic.Dictionary<string, int>();
            var gate = new object();
            System.Threading.Tasks.Parallel.For(1, 10_000_000, new System.Threading.Tasks.ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 1) }, (i, state) =>
            {
                if (DateTime.UtcNow >= end) { state.Stop(); return; }
                var r = Flag("-rsPeel") ? HardEight.TryPeel(look, (uint)i, out string w) : HardEight.Try(look, (uint)i, out w);
                lock (gate)
                {
                    tried++;
                    if (r != null) { ok++; if (r.Held == 13) full++; if (best == null || r.Held > best.Held || (r.Held == best.Held && r.Score > best.Score)) best = r; }
                    else { string k = System.Text.RegularExpressions.Regex.Replace(w ?? "?", @"[0-9]+", "#"); why.TryGetValue(k, out int n); why[k] = n + 1; }
                    if ((DateTime.UtcNow - beat).TotalMinutes >= 4) { beat = DateTime.UtcNow; Log($"ilerleme: {(beat - start).TotalMinutes:0} dk, {tried} aday, {ok} adil, {full} hepsini tuttu, en çok tutan {(best == null ? "—" : best.Held + "/13")}"); }
                }
            });
            Log($"bitti: {tried} aday, {ok} adil, {full} hepsini tuttu");
            foreach (var kv in why.OrderByDescending(k => k.Value).Take(6)) Log($"  elenme: {kv.Key} × {kv.Value}");
            if (best == null) { Log("aday yok"); return; }
            var old = store.Find("D07");
            best.Draft.Id = "D07";
            var saved = store.Save(best.Draft);
            File.WriteAllText(path, store.ToJson(), new System.Text.UTF8Encoding(false));
            Log($"kaydedildi: {saved.Id} (tohum {best.Seed}, {best.Held}/13, fitil payları {string.Join("/", best.FuseSlack.Select(x => "+" + x))})");
            foreach (var row in best.Factors.Metrics()) Log($"  {row.name}: {row.value} (hedef {row.target}) {(row.ok ? "✓" : "✗")}");
            Log("  tanı: " + best.Diag);
        }

        public static void Rounds()
        {
            var slots = FactoryRunner.ParseSlots(Arg("-rsSlots", "10,28,34"));
            if (Flag("-rsShort"))
                foreach (var s in FactoryRunner.ShortSlots(RedesignStore.LoadOrNew())) if (!slots.Contains(s)) slots.Add(s);
            slots.Sort();
            var o = FactoryWindow.OptionsFor(int.Parse(Arg("-rsDepth", "2")));
            o.Finalists = int.Parse(Arg("-rsFinalists", o.Finalists.ToString()));   // > 3: new finalists sit beside the old ones for a later pick
            FactoryRunner.RunRounds(slots, o, RunName("tur-"), int.Parse(Arg("-rsRounds", "3")), uint.Parse(Arg("-rsSeed", "2")), Flag("-rsRelax"), Log,
                roundMinutes: double.Parse(Arg("-rsRoundMin", "45")), totalMinutes: double.Parse(Arg("-rsTotalMin", "240")));
            Log("bitti, rapor " + FactoryReport.Path);
        }
    }
}
