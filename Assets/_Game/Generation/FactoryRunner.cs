using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ReverseSolver.Core;

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
                if (f.Assessment.Curve != null) sb.AppendLine($"  {f.Assessment.Curve.Spark()} {f.Assessment.Curve}");
                if (!f.Accepted) sb.AppendLine("  eşik: " + string.Join(", ", f.Psych));
                if (f.ByMemory != null) sb.AppendLine("  m 0 / 0,5 / 1,0: " + string.Join(" | ", f.ByMemory.Select(r => r.ToString())));
                if (f.Fun.FrustrationWhy.Count > 0) sb.AppendLine("  hayal kırıklığı: " + string.Join(", ", f.Fun.FrustrationWhy.OrderByDescending(k => k.Value).Select(k => $"{k.Key} {k.Value}")));
            }
            return sb.ToString();
        }
    }
}
