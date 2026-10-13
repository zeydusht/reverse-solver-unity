using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ReverseSolver.Editing;

namespace ReverseSolver.Generation
{
    /* docs/fabrika/RAPOR.md (G6): slot by slot finalists and their numbers, the
       40-slot sequence and the checks on it (PM's sequence control). Rewritten
       on every run from redesign.json plus the run's slot outcomes. */
    public static class FactoryReport
    {
        public const string Path = "docs/fabrika/RAPOR.md";
        static readonly CultureInfo Tr = CultureInfo.GetCultureInfo("tr-TR");

        static string P(double v) => "%" + (v * 100).ToString("0", Tr);
        static string N(double v, string f = "0.00") => v.ToString(f, Tr);
        static double S(RedesignStore.Entry e, string k) => e.Scores.TryGetValue(k, out var v) ? v : double.NaN;

        public sealed class SequenceCheck
        {
            public double FirstTenFirstWin = double.NaN;
            public readonly List<string> Problems = new List<string>();
        }

        /* The 40-slot checks on the best finalist (or the approved level) of each slot. */
        public static SequenceCheck Check(RedesignStore store, IReadOnlyList<SlotSpec> plan)
        {
            var c = new SequenceCheck();
            c.Problems.AddRange(FactoryPlan.Problems(plan));
            var picks = plan.Select(s => Pick(store, s.Slot)).ToList();
            var firstTen = picks.Take(10).Where(e => e != null).Select(e => S(e, "ilkDeneme")).Where(v => !double.IsNaN(v)).ToList();
            if (firstTen.Count > 0) c.FirstTenFirstWin = firstTen.Average();
            if (firstTen.Count == 10 && c.FirstTenFirstWin < .85) c.Problems.Add($"1–10 ilk deneme ortalaması {P(c.FirstTenFirstWin)} (≥ %85)");
            for (int i = 0; i < picks.Count; i++)
                for (int k = i + 1; k < picks.Count; k++)
                {
                    if (picks[i] == null || picks[k] == null) continue;
                    var a = picks[i].Draft.ToLevelData(); var b = picks[k].Draft.ToLevelData();
                    if (Similarity.IsDuplicate(a, b)) c.Problems.Add($"slot {picks[i].Slot} ve {picks[k].Slot} birbirine çok benziyor");
                }
            return c;
        }

        /* The approved level of the slot, otherwise its best finalist. */
        public static RedesignStore.Entry Pick(RedesignStore s, int slot) =>
            s.ForSlot(slot).FirstOrDefault(e => e.Status == RedesignStore.Approved)
            ?? s.ForSlot(slot).Where(e => e.Status == RedesignStore.Finalist).OrderByDescending(e => S(e, "keyif") - 2 * S(e, "hayalKirikligi")).FirstOrDefault();

        public static string Build(RedesignStore store, IReadOnlyList<SlotSpec> plan, IReadOnlyDictionary<int, SlotOutcome> run, string runName, string when,
                                   string runDir = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# Seviye fabrikası raporu");
            sb.AppendLine();
            sb.AppendLine($"Koşu: `{runName}`, {when}. Sayılar modelin çıktısıdır: **keyif tahmini** 1000 sanal oyuncunun (FunSim) sonucudur, gerçek oyuncunun yerini tutmaz; playtest'teki 👍/👎 bunu sınar.");
            sb.AppendLine();

            int ready = plan.Count(s => store.ForSlot(s.Slot).Count(e => e.Status == RedesignStore.Finalist || e.Status == RedesignStore.Approved) >= 2);
            int approved = store.Entries.Count(e => e.Status == RedesignStore.Approved);
            var hard = plan.Where(s => store.ForSlot(s.Slot).Count(e => e.Status == RedesignStore.Finalist || e.Status == RedesignStore.Approved) < 2).Select(s => s.Slot).ToList();
            sb.AppendLine("## Özet");
            sb.AppendLine();
            sb.AppendLine($"- En az 2 finalisti olan slot: **{ready}/{plan.Count}** · onaylı seviye: **{approved}**");
            sb.AppendLine($"- Zor slotlar (2'den az finalist ya da onaylı): {(hard.Count == 0 ? "yok" : string.Join(", ", hard))}");
            var chk = Check(store, plan);
            sb.AppendLine($"- 1–10 tahmini ilk deneme ortalaması: {(double.IsNaN(chk.FirstTenFirstWin) ? "—" : P(chk.FirstTenFirstWin))} (hedef ≥ %85)");
            sb.AppendLine($"- Sıra kontrolü: {(chk.Problems.Count == 0 ? "sorun yok" : string.Join("; ", chk.Problems))}");
            sb.AppendLine();

            sb.AppendLine("## 40'lık sıra");
            sb.AppendLine();
            sb.AppendLine("Her slotun onaylı seviyesi ya da en iyi finalisti. Okuma: hamle başına okuma; ilk: tahmini ilk denemede kazanma.");
            sb.AppendLine();
            sb.AppendLine("| Slot | Tür | Yeni | Seviye | Durum | Okuma | İlk | Keyif | Hayal k. | Sıkıcı | Uyum | Süre |");
            sb.AppendLine("|---|---|---|---|---|---|---|---|---|---|---|---|");
            foreach (var s in plan)
            {
                var e = Pick(store, s.Slot);
                if (e == null) { sb.AppendLine($"| {s.Slot} | {s.Genre.Label()} | {SlotSpec.Label(s.NewThing)} | — | aday yok | | | | | | | |"); continue; }
                sb.AppendLine($"| {s.Slot} | {s.Genre.Label()} | {SlotSpec.Label(s.NewThing)} | {e.Draft.Id} | {e.Status} | {N(S(e, "okuma"))} | {P(S(e, "ilkDeneme"))} | {P(S(e, "keyif"))} | {P(S(e, "hayalKirikligi"))} | {P(S(e, "sikici"))} | {(double.IsNaN(S(e, "uyum")) ? "—" : N(S(e, "uyum")))} | {S(e, "sure")} |");
            }
            sb.AppendLine();
            sb.AppendLine("```");
            sb.AppendLine("okuma  " + Bar(plan.Select(s => Pick(store, s.Slot)).Select(e => e == null ? double.NaN : S(e, "okuma"))));
            sb.AppendLine("ilk    " + Bar(plan.Select(s => Pick(store, s.Slot)).Select(e => e == null ? double.NaN : S(e, "ilkDeneme"))));
            sb.AppendLine("keyif  " + Bar(plan.Select(s => Pick(store, s.Slot)).Select(e => e == null ? double.NaN : S(e, "keyif"))));
            sb.AppendLine("tür    " + string.Concat(plan.Select(s => "TNKOZÇ"[(int)s.Genre])));
            sb.AppendLine("```");
            sb.AppendLine();

            sb.AppendLine("## Slotlar");
            foreach (var s in plan)
            {
                sb.AppendLine();
                sb.AppendLine($"### Slot {s.Slot} — {s.Genre.Label()}{(s.NewThing != null ? ", yeni: " + SlotSpec.Label(s.NewThing) : "")}");
                if (run != null && run.TryGetValue(s.Slot, out var o))
                {
                    sb.AppendLine();
                    sb.AppendLine($"Bu koşu: {o.Search.Evaluated} değerlendirme, {o.Search.BandPassed} bantta, {o.Search.Passed.Count} finalist adayı, {o.Chosen.Count} psikolog eşiğini geçti ({o.Ms / 1000:0} sn).");
                    var rejected = o.Checked.Where(f => !f.Accepted).ToList();
                    if (rejected.Count > 0)
                        sb.AppendLine("Eşikte kalanlar: " + string.Join(" · ", rejected.Take(4).Select(f => $"{f.Candidate.Id}: {string.Join(", ", f.Psych)}")));
                    foreach (var f in o.Checked.Where(f => f.ByMemory != null))
                        sb.AppendLine($"- {f.Candidate.Id} ezber m = 0 / 0,5 / 1,0: keyif {string.Join(" / ", f.ByMemory.Select(r => P(r.EnjoyRate)))}, " +
                                      $"hk {string.Join(" / ", f.ByMemory.Select(r => P(r.FrustrationRate)))}, 3 denemede {string.Join(" / ", f.ByMemory.Select(r => P(r.WinBy3Rate)))}");
                    if (o.Search.Passed.Count == 0 && o.Search.Best != null)
                        sb.AppendLine($"En yakın aday: {string.Join(", ", o.Search.Best.OutOfBand.Concat(o.Search.Best.Unfair))}");
                }
                string log = runDir == null ? null : System.IO.Path.Combine(runDir, $"slot_{s.Slot:00}.txt");
                if (log != null && System.IO.File.Exists(log))
                {
                    var lines = System.IO.File.ReadAllLines(log);
                    sb.AppendLine();
                    sb.AppendLine("Bu koşu: " + (lines.Length > 1 ? lines[1] : ""));
                    var near = lines.FirstOrDefault(l => l.StartsWith("en yakın: "));
                    if (near != null && store.ForSlot(s.Slot).All(e => e.Status != RedesignStore.Finalist)) sb.AppendLine(near);
                    var rejected = lines.Where(l => l.TrimStart().StartsWith("eşik: ")).Take(3).Select(l => l.Trim()).ToList();
                    if (rejected.Count > 0) sb.AppendLine("Eşikte kalanlar: " + string.Join(" · ", rejected));
                }
                var entries = store.ForSlot(s.Slot).ToList();
                if (entries.Count == 0) { sb.AppendLine(); sb.AppendLine("Finalist yok."); continue; }
                sb.AppendLine();
                sb.AppendLine("| Seviye | Durum | Tahta | Gövde | Açık | k₀ | Okuma | Darboğaz | Süre | Keyif | Hayal k. | Sıkıcı | İlk | 3 dn. | Near-miss | Yıldız | Uyum · zirve |");
                sb.AppendLine("|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|");
                foreach (var e in entries)
                    sb.AppendLine($"| {e.Draft.Id} | {e.Status} | {e.Draft.Width}×{e.Draft.Height} | {S(e, "govde")} | %{S(e, "acikDikis")} | {S(e, "k0")} | {N(S(e, "okuma"))} | {S(e, "darbogaz")} | {S(e, "sure")} sn | {P(S(e, "keyif"))} | {P(S(e, "hayalKirikligi"))} | {P(S(e, "sikici"))} | {P(S(e, "ilkDeneme"))} | {P(S(e, "ucDeneme"))} | {P(S(e, "nearMiss"))} | {S(e, "medyanYildiz")} | {(double.IsNaN(S(e, "uyum")) ? "—" : N(S(e, "uyum")) + " · " + P(S(e, "zirve")))} |");
                foreach (var e in entries.Where(e => e.Aha != null || e.Psych != null || e.Notes.Count > 0))
                    sb.AppendLine($"- **{e.Draft.Id}**{(e.Aha != null ? " — fikir: " + e.Aha : "")}{(e.Psych != null ? " — psikolog: " + e.Psych : "")}{(e.Notes.Count > 0 ? " — notlar: " + string.Join("; ", e.Notes) : "")}");
            }
            return sb.ToString();
        }

        static string Bar(IEnumerable<double> values)
        {
            const string bars = "▁▂▃▄▅▆▇█";
            var v = values.ToList();
            double max = v.Where(x => !double.IsNaN(x)).DefaultIfEmpty(1).Max();
            return string.Concat(v.Select(x => double.IsNaN(x) ? ' ' : bars[Math.Min(7, Math.Max(0, (int)(x / max * 7.999)))]));
        }
    }
}
