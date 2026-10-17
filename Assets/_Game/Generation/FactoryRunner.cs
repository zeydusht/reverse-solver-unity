using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ReverseSolver.Core;
using ReverseSolver.Editing;

namespace ReverseSolver.Generation
{
    /* G6's driver, shared by the batchmode command, the editor window and the
       headless runner: run the given slots, keep the chosen finalists in
       redesign.json, write every slot's candidates under Builds/fabrika/<run>/
       (outside git) and rewrite docs/fabrika/RAPOR.md. Paths are relative to the
       project root. */
    public static class FactoryRunner
    {
        public const string RunsDir = "Builds/fabrika";

        /* "1-40", "12", "3,5,20-22". */
        public static List<int> ParseSlots(string text)
        {
            var list = new List<int>();
            foreach (var part in (text ?? "1-40").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var ends = part.Split('-');
                int a = int.Parse(ends[0].Trim()), b = ends.Length > 1 ? int.Parse(ends[1].Trim()) : a;
                for (int i = Math.Min(a, b); i <= Math.Max(a, b); i++) if (i >= 1 && i <= FactoryPlan.Slots && !list.Contains(i)) list.Add(i);
            }
            return list;
        }

        /* progress(done, total, line): return false to stop after the current slot. */
        /* part: null for a normal run (redesign.json + RAPOR.md); a name for one of several
           parallel runs: finalists go to Builds/fabrika/<run>/<part>.json and Merge joins them. */
        public static Dictionary<int, SlotOutcome> Run(IEnumerable<int> slots, FactoryOptions o, string runName,
                                                       Func<int, int, string, bool> progress = null, string levelsJson = "Assets/_Game/Levels/levels.json",
                                                       string part = null)
        {
            var plan = FactoryPlan.All();
            var catalog = Catalog.From(LevelParser.Parse(File.ReadAllText(levelsJson)));
            var players = FunSim.Population(o.Players);
            string dir = Path.Combine(RunsDir, runName);
            Directory.CreateDirectory(dir);
            string storePath = part == null ? RedesignStore.Path : Path.Combine(dir, part + ".json");
            var store = part == null ? RedesignStore.LoadOrNew() : new RedesignStore();
            var outcomes = new Dictionary<int, SlotOutcome>();
            var todo = slots.ToList();
            for (int i = 0; i < todo.Count; i++)
            {
                var spec = plan[todo[i] - 1];
                var outc = Factory.RunSlot(spec, o, catalog, players);
                outcomes[spec.Slot] = outc;
                store.PutFinalists(outc, o.Finalists);
                store.Save(storePath);                                // after every slot: a stopped run keeps what it found
                File.WriteAllText(Path.Combine(dir, $"slot_{spec.Slot:00}.txt"), SlotLog(outc), new UTF8Encoding(false));
                string line = $"{outc.Search} → {outc.Chosen.Count} kabul";
                if (progress != null && !progress(i + 1, todo.Count, line)) break;
            }
            if (part == null) WriteReport(store, plan, runName);
            return outcomes;
        }

        static void WriteReport(RedesignStore store, List<SlotSpec> plan, string runName)
        {
            // slot logs of this run feed the report's per-slot lines
            Directory.CreateDirectory(Path.GetDirectoryName(FactoryReport.Path));
            File.WriteAllText(FactoryReport.Path, FactoryReport.Build(store, plan, null, runName, DateTime.Now.ToString("yyyy-MM-dd HH:mm"), Path.Combine(RunsDir, runName)),
                              new UTF8Encoding(false));
        }

        /* Joins the parts of a parallel run into redesign.json and rewrites the report. */
        public static RedesignStore Merge(string runName)
        {
            string dir = Path.Combine(RunsDir, runName);
            var store = RedesignStore.LoadOrNew();
            foreach (var f in Directory.GetFiles(dir, "*.json").OrderBy(f => f))
                store.MergeFinalists(RedesignStore.LoadOrNew(f));
            store.Save();
            WriteReport(store, FactoryPlan.All(), runName);
            return store;
        }

        /* The set's fun average (PRODUCT.md "Keyif eşikleri (Zeyd hedefleri)"): each slot's approved
           level or first finalist (the report's pick); slots without one are left out. */
        public static (double average, int slots) SetFun(RedesignStore store)
        {
            var picks = Enumerable.Range(1, FactoryPlan.Slots).Select(s => FactoryReport.Pick(store, s)).Where(e => e != null && e.Scores.ContainsKey("keyif")).ToList();
            return (picks.Count == 0 ? 0 : picks.Average(e => e.Scores["keyif"]), picks.Count);
        }

        /* V5 re-scoring of the saved pool (redesign.json): every unapproved finalist gets the
           redesign stock (1 of each once open), the runs without and with boosters, and the
           current thresholds (relaxed only where it was let in relaxed). Those that fail leave
           the pool; the log lists them and the per-slot counts before and after. */
        public static string Rescore(string runName, int players = 1000)
        {
            var plan = FactoryPlan.All();
            var pop = FunSim.Population(players);
            var store = RedesignStore.LoadOrNew();
            string dir = Path.Combine(RunsDir, runName);
            Directory.CreateDirectory(dir);
            var before = Enumerable.Range(1, FactoryPlan.Slots).ToDictionary(s => s, s => store.ForSlot(s).Count(e => e.Status == RedesignStore.Finalist));
            var sb = new StringBuilder();
            sb.AppendLine($"V5 yeniden puanlama {runName}: {store.Entries.Count(e => e.Status == RedesignStore.Finalist)} finalist");
            sb.AppendLine();
            sb.AppendLine("| Slot | Tür | Aday | Keyif booster'sız → booster'lı | Hk booster'sız → booster'lı | İlk | Erken | Çekiç | Saat | Sonuç |");
            sb.AppendLine("|---|---|---|---|---|---|---|---|---|---|");
            var gone = new List<RedesignStore.Entry>();
            foreach (var e in store.Entries.Where(e => e.Status == RedesignStore.Finalist).ToList())
            {
                var spec = plan[e.Slot - 1];
                foreach (var id in LevelDraft.BoosterIds)
                    e.Draft.SetBooster(id, e.Slot >= SlotSpec.IntroducedAt[id] ? Catalog.Stock[id] : 0);
                var level = e.Draft.ToLevelData();
                var plain = FunSim.Run(level, spec.Genre, pop, options: FunSim.OptionsFor(spec));
                var boosted = FunSim.Run(level, spec.Genre, pop, options: FunSim.OptionsFor(spec, boosters: true));
                var t = FunThresholds.For(spec.Genre);
                if (e.Scores.ContainsKey("erkenGevsetildi") || e.Scores.ContainsKey("bantGevsetildi")) t = t.Relaxed();
                var why = t.Check(plain, boosted);
                RedesignStore.FunScores(e, plain, boosted);
                e.Notes.RemoveAll(n => n.StartsWith("booster'a dayalı"));
                if (e.Scores.ContainsKey("boosteraDayali")) e.Notes.Add($"booster'a dayalı: booster'lı keyif booster'sızdan {(boosted.EnjoyRate - plain.EnjoyRate) * 100:0} puan yüksek");
                if (why.Count > 0) gone.Add(e);
                sb.AppendLine($"| {e.Slot} | {spec.Genre.Label()} | {e.Candidate} | %{plain.EnjoyRate * 100:0.0} → %{boosted.EnjoyRate * 100:0.0} | %{plain.FrustrationRate * 100:0.0} → %{boosted.FrustrationRate * 100:0.0} | " +
                              $"%{plain.FirstWinRate * 100:0} | %{plain.EarlyTimeoutRate * 100:0.0} | %{boosted.HammerRate * 100:0} | %{boosted.ClockRate * 100:0} | {(why.Count == 0 ? "kalır" : "çıkar: " + string.Join(", ", why))} |");
            }
            store.Entries.RemoveAll(gone.Contains);
            store.Save();
            sb.AppendLine();
            sb.AppendLine("Slot başına finalist (önce → sonra): " + string.Join(" · ", Enumerable.Range(1, FactoryPlan.Slots)
                .Select(s => $"{s}: {before[s]}→{store.ForSlot(s).Count(e => e.Status == RedesignStore.Finalist)}")));
            var (avg, n) = SetFun(store);
            sb.AppendLine($"Set ortalaması keyif (booster'lı, {n} slot): %{avg * 100:0.0}");
            File.WriteAllText(Path.Combine(dir, "rescore.md"), sb.ToString(), new UTF8Encoding(false));
            WriteReport(store, plan, runName);
            return sb.ToString();
        }

        /* Slots with fewer than two finalists or approved levels. */
        public static List<int> ShortSlots(RedesignStore store) =>
            Enumerable.Range(1, FactoryPlan.Slots).Where(s => Kept(store, s) < 2).ToList();

        static int Kept(RedesignStore store, int slot) => store.ForSlot(slot).Count(e => e.Status != RedesignStore.Rejected);

        /* Rounds for short slots (PRODUCT.md "2026-10-17 SIRASI"): up to `rounds` searches with new
           seeds (firstSeed, firstSeed + 1, …) while a slot has fewer than two finalists, the slots of a
           round in parallel; the finalists found fill the slot (earlier ones stay). Then, with relax,
           one more round with the relaxed thresholds for the slots still short. Fun first on Zor /
           Çok zor when the set's fun average is under 90 %. */
        /* roundMinutes / totalMinutes: time limits (a round's searches stop at its limit and keep what
           they found; no new round starts after the total); a progress line every 20 minutes. */
        public static string RunRounds(IEnumerable<int> slots, FactoryOptions o, string runName, int rounds, uint firstSeed, bool relax,
                                       Action<string> log = null, string levelsJson = "Assets/_Game/Levels/levels.json",
                                       double roundMinutes = 45, double totalMinutes = 240)
        {
            var started = DateTime.UtcNow;
            var plan = FactoryPlan.All();
            var catalog = Catalog.From(LevelParser.Parse(File.ReadAllText(levelsJson)));
            var players = FunSim.Population(o.Players);
            string dir = Path.Combine(RunsDir, runName);
            Directory.CreateDirectory(dir);
            var store = RedesignStore.LoadOrNew();
            var (avg, _) = SetFun(store);
            bool funFirst = avg < .90;
            var todo = slots.ToList();
            var sb = new StringBuilder();
            sb.AppendLine($"koşu {runName}: slotlar {string.Join(",", todo)}, {rounds} tur + {(relax ? "gevşetme turu" : "gevşetme yok")}, set keyif %{avg * 100:0.0} → keyif önce: {(funFirst ? "evet (Z/ÇZ)" : "hayır")}");
            log?.Invoke(sb.ToString().TrimEnd());
            for (int r = 0; r < rounds + (relax ? 1 : 0); r++)
            {
                bool relaxRound = r == rounds;
                var shortNow = r == 0 ? todo : todo.Where(s => Kept(store, s) < 2).ToList();   // the first round runs every slot asked for
                if (shortNow.Count == 0) break;
                if ((DateTime.UtcNow - started).TotalMinutes >= totalMinutes) { log?.Invoke($"zaman sınırı {totalMinutes:0} dk: tur {r + 1} başlatılmadı"); break; }
                var opt = Clone(o);
                opt.Seed = firstSeed + (uint)r;
                opt.Relax = relaxRound;
                opt.FunFirst = funFirst;
                opt.Deadline = DateTime.UtcNow.AddMinutes(Math.Min(roundMinutes, totalMinutes - (DateTime.UtcNow - started).TotalMinutes));
                int doneSlots = 0;
                using var beat = new System.Threading.Timer(_ => log?.Invoke($"ilerleme: tur {r + 1}, {(DateTime.UtcNow - started).TotalMinutes:0} dk, {System.Threading.Volatile.Read(ref doneSlots)}/{shortNow.Count} slot bitti"),
                                                            null, TimeSpan.FromMinutes(20), TimeSpan.FromMinutes(20));
                var sw = System.Diagnostics.Stopwatch.StartNew();
                var outcomes = new System.Collections.Concurrent.ConcurrentDictionary<int, SlotOutcome>();
                System.Threading.Tasks.Parallel.ForEach(shortNow, new System.Threading.Tasks.ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 1) },
                    s => { outcomes[s] = Factory.RunSlot(plan[s - 1], opt, catalog, players); System.Threading.Interlocked.Increment(ref doneSlots); });
                foreach (var s in shortNow)
                {
                    var outc = outcomes[s];
                    int had = store.ForSlot(s).Count(e => e.Status == RedesignStore.Finalist);
                    store.PutFinalists(outc, o.Finalists, keep: true);
                    int now = store.ForSlot(s).Count(e => e.Status == RedesignStore.Finalist);
                    File.WriteAllText(Path.Combine(dir, $"slot_{s:00}_tur{r + 1}.txt"), SlotLog(outc), new UTF8Encoding(false));
                    string line = $"tur {r + 1}{(relaxRound ? " (gevşetme)" : "")} tohum {opt.Seed}: {outc.Search} → {outc.Chosen.Count} kabul, slot {had}→{now} finalist" + (outc.Search.TimedOut ? " (zaman sınırı)" : "");
                    sb.AppendLine(line);
                    log?.Invoke(line);
                }
                store.Save();
                log?.Invoke($"tur {r + 1} bitti: {shortNow.Count} slot, {sw.Elapsed.TotalMinutes:0} dk");
            }
            var left = todo.Where(s => Kept(store, s) < 2).ToList();
            sb.AppendLine("2'den az finalistle kalan: " + (left.Count == 0 ? "yok" : string.Join(", ", left)));
            File.WriteAllText(Path.Combine(dir, "turlar.md"), sb.ToString(), new UTF8Encoding(false));
            WriteReport(store, plan, runName);
            return sb.ToString();
        }

        static FactoryOptions Clone(FactoryOptions o) => new FactoryOptions
        {
            BudgetEasy = o.BudgetEasy, BudgetOrta = o.BudgetOrta, BudgetZor = o.BudgetZor, BudgetCokZor = o.BudgetCokZor, BandWant = o.BandWant,
            Finalists = o.Finalists, Players = o.Players, SearchPlayers = o.SearchPlayers, MemoryTable = o.MemoryTable, Seed = o.Seed, Relax = o.Relax, FunFirst = o.FunFirst, Deadline = o.Deadline
        };

        /* Every candidate the fun estimate saw, for the agents and later analysis. */
        static string SlotLog(SlotOutcome o)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{o.Spec}");
            sb.AppendLine(o.Search.ToString());
            if (o.Search.Best != null) sb.AppendLine("en yakın: " + o.Search.Best);
            foreach (var f in o.Checked)
            {
                sb.AppendLine($"{f.Candidate.Id} {(f.Accepted ? "KABUL" : "RET")} {f.Candidate.Params} t{f.Candidate.Level.TimerSeconds} {f.Candidate.Level.Width}x{f.Candidate.Level.Height}");
                sb.AppendLine($"  {f.Fun}");
                if (f.Boosted != null) sb.AppendLine($"  V5: {f.Boosted}");
                if (f.Assessment.Curve != null) sb.AppendLine($"  {f.Assessment.Curve.Spark()} {f.Assessment.Curve}");
                if (!f.Accepted) sb.AppendLine("  eşik: " + string.Join(", ", f.Psych));
                if (f.ByMemory != null) sb.AppendLine("  m 0 / 0,5 / 1,0: " + string.Join(" | ", f.ByMemory.Select(r => r.ToString())));
                if (f.Fun.FrustrationWhy.Count > 0) sb.AppendLine("  hayal kırıklığı: " + string.Join(", ", f.Fun.FrustrationWhy.OrderByDescending(k => k.Value).Select(k => $"{k.Key} {k.Value}")));
            }
            return sb.ToString();
        }
    }
}
