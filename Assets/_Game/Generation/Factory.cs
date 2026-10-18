using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReverseSolver.Core;
using ReverseSolver.Core.Json;
using ReverseSolver.Editing;

namespace ReverseSolver.Generation
{
    /* G6: the factory for one slot or all 40 (PRODUCT.md "Faz 4"):
         search (G1 + G2 + G3) → fun estimate for the band finalists (G4) →
         the psychologist's thresholds → the best 2-3 distinct finalists.
       The agents (G5) read the report and change the knobs between runs
       through per-slot overrides; the code never decides more than the
       thresholds written here. Finalists go to redesign.json (levels.json
       format, plus a "fabrika" block); RAPOR.md is rewritten each run. */
    public sealed class FactoryOptions
    {
        public int BudgetEasy = 150, BudgetOrta = 400, BudgetZor = 800, BudgetCokZor = 800;
        public int BandWant = 12;                 // band finalists to keep searching for before the fun estimate
        public int Finalists = 3;
        public int Players = 1000;
        public int SearchPlayers = 200;            // fun estimate inside the search (first-try target); finalists get Players
        public bool MemoryTable = true;            // m = 0 / 0.5 / 1.0 side by side for the report
        public uint Seed = 1;
        /* Only after three rounds (PRODUCT.md "Keyif eşikleri (Zeyd hedefleri)"): a slot short of two
           finalists may take Orta's early time-out cap at 10 % or Zor / Çok zor's first-try band top
           at 70 / 45 %, labelled. */
        public bool Relax;
        /* The set's fun average is under 90 %: Zor / Çok zor rank fun first. */
        public bool FunFirst;
        /* A search stops drawing candidates at this time (UTC); the slot keeps what it found. */
        public DateTime? Deadline;

        public int BudgetFor(Genre g) => g == Genre.Orta ? BudgetOrta : g == Genre.Zor ? BudgetZor : g == Genre.CokZor ? BudgetCokZor : BudgetEasy;
    }

    /* The psychologist's numbers (PRODUCT.md "Keyif eşikleri (Zeyd hedefleri, 2026-10-17)"
       over checkpoints 3 and 4): fun, total and unfair frustration from the V5 run (with
       boosters); the first-try band (the genre's band ± 5 points; Tanıtım / Nefes floors
       without tolerance) and the early time-out cap from the run without boosters.
       Near-miss never filters; it only ranks. */
    public sealed class FunThresholds
    {
        public Genre Genre;
        public double MinEnjoy = .90, MaxFrustration = .05, MaxUnfair = .05;
        /* Checkpoint 4: players whose clock ran out with >= 25 % of the pieces left (O/Z/ÇZ); 1 = no cap. */
        public double MaxEarlyTimeout = 1;
        public Band FirstWin, NearMiss;

        public static FunThresholds For(Genre g)
        {
            var t = new FunThresholds { Genre = g };
            switch (g)
            {
                case Genre.Tanitim: t.FirstWin = new Band(.95, null); t.NearMiss = new Band(0, .05); break;
                case Genre.Nefes: t.FirstWin = new Band(.90, null); t.NearMiss = new Band(0, .05); break;
                case Genre.Kolay: t.FirstWin = new Band(.80, 1.0); t.NearMiss = new Band(0, .05); break;
                // fun Zeyd 2026-10-17 (O 90, Z 80, ÇZ 70), frustration PM (O 10, Z 20, ÇZ 30); early time-out
                // caps O 5 %, Z 25 %, ÇZ 35 % and the first-try bands as at checkpoint 4
                case Genre.Orta: t.MinEnjoy = .90; t.MaxFrustration = .10; t.MaxEarlyTimeout = .05; t.FirstWin = new Band(.60, .90); t.NearMiss = new Band(.05, .15); break;
                case Genre.Zor: t.MinEnjoy = .80; t.MaxFrustration = .20; t.MaxEarlyTimeout = .25; t.FirstWin = new Band(.35, .65); t.NearMiss = new Band(.15, .30); break;
                default: t.MinEnjoy = .70; t.MaxFrustration = .30; t.MaxEarlyTimeout = .35; t.FirstWin = new Band(.15, .40); t.NearMiss = new Band(.30, .50); break;
            }
            return t;
        }

        public const double OrtaRelaxedEarlyTimeout = .10, ZorRelaxedTop = .70, CokZorRelaxedTop = .45;
        public const string EarlyRelaxedNote = "erken süre gevşetildi: tavan %10 (Orta, 3 turdan sonra slotta 2'den az finalist)";
        public const string BandRelaxedNote = "bant gevşetildi: ilk deneme üst sınırı {0} (3 turdan sonra slotta 2'den az finalist)";

        /* The relaxed copy: Orta's early time-out cap 10 % ("erken süre gevşetildi"), Zor's and
           Çok zor's first-try band top 70 / 45 % ("bant gevşetildi"); other genres unchanged. */
        public FunThresholds Relaxed()
        {
            var r = (FunThresholds)MemberwiseClone();
            if (Genre == Genre.Orta) r.MaxEarlyTimeout = OrtaRelaxedEarlyTimeout;
            if (Genre == Genre.Zor) r.FirstWin = new Band(FirstWin.Min, ZorRelaxedTop);
            if (Genre == Genre.CokZor) r.FirstWin = new Band(FirstWin.Min, CokZorRelaxedTop);
            return r;
        }

        /* Relaxing applies only after three rounds, to an Orta / Zor / Çok zor slot left with fewer than two finalists. */
        public static bool RelaxAllowed(Genre g, int finalists, bool afterThreeRounds) =>
            afterThreeRounds && (g == Genre.Orta || g == Genre.Zor || g == Genre.CokZor) && finalists < 2;

        /* plain: the run without boosters (first try, early time-out); boosted: the V5 run (fun,
           frustration); null takes plain for both (the search and the old tables). */
        public List<string> Check(FunSim.Result plain, FunSim.Result boosted = null)
        {
            var f = boosted ?? plain;
            var u = new List<string>();
            if (f.EnjoyRate < MinEnjoy - 1e-9) u.Add($"keyif %{f.EnjoyRate * 100:0.0} (≥ %{MinEnjoy * 100:0})");
            if (f.FrustrationRate > MaxFrustration + 1e-9) u.Add($"hayal kırıklığı %{f.FrustrationRate * 100:0.0} (≤ %{MaxFrustration * 100:0})");
            if (f.UnfairRate > MaxUnfair + 1e-9) u.Add($"haksız hayal kırıklığı %{f.UnfairRate * 100:0.0} (≤ %{MaxUnfair * 100:0})");
            if (!FirstWin.Contains(plain.FirstWinRate)) u.Add($"ilk deneme %{plain.FirstWinRate * 100:0} (hedef {Pct(FirstWin)})");
            if (plain.EarlyTimeoutRate > MaxEarlyTimeout + 1e-9) u.Add($"erken süre bitmesi %{plain.EarlyTimeoutRate * 100:0.0} (≤ %{MaxEarlyTimeout * 100:0})");
            return u;
        }

        /* How far the first-try win is from the band's middle (0 inside the middle). */
        public double FirstWinOffCentre(double firstWin)
        {
            double lo = FirstWin.Min ?? 0, hi = FirstWin.Max ?? 1;
            return Math.Abs(firstWin - (lo + hi) / 2);
        }

        public double NearMissOff(double nearMiss) =>
            NearMiss.Contains(nearMiss) ? 0 : Math.Min(Math.Abs(nearMiss - (NearMiss.Min ?? 0)), Math.Abs(nearMiss - (NearMiss.Max ?? 1)));

        static string Pct(Band b) => b.Max.HasValue ? $"%{b.Min * 100:0}–{b.Max * 100:0}" : $"%{b.Min * 100:0}+";
    }

    public sealed class Finalist
    {
        public Assessment Assessment;
        /* Fun: without boosters (first try, early time-out, near-miss); Boosted: V5 (fun, frustration). */
        public FunSim.Result Fun, Boosted;
        public FunSim.Result Feel => Boosted ?? Fun;
        public readonly List<string> Psych = new List<string>();
        public bool Accepted => Psych.Count == 0;
        public Candidate Candidate => Assessment.Candidate;
        public FunThresholds Thresholds;
        public FunSim.Result[] ByMemory;              // m = 0 / 0.5 / 1.0 (the report shows them side by side)
        /* Checkpoint 4: let in although the curve fit is under 0.6 (>= 0.40), one per slot with < 2 finalists. */
        public bool CurveException;
        /* Orta only: accepted with the early time-out cap relaxed to 10 % (slot short of two). */
        public bool EarlyRelaxed;
        /* Zor / Çok zor only: accepted with the first-try band top relaxed to 70 / 45 %. */
        public bool BandRelaxed;
        /* The set's fun average is under 90 %: on Zor / Çok zor fun ranks first. */
        public bool FunFirst;
        /* V5 lifted fun by more than this over the run without boosters: "booster'a dayalı" (information). */
        public const double BoosterLeaning = .15;
        public bool LeansOnBoosters => Boosted != null && Boosted.EnjoyRate - Fun.EnjoyRate > BoosterLeaning + 1e-9;

        /* PM's order: first-try win close to the band's middle, then fun, then near-miss close to its band. */
        public (double, double, double) RankKey => FunFirst
            ? (-Feel.EnjoyRate, Thresholds.FirstWinOffCentre(Fun.FirstWinRate), Thresholds.NearMissOff(Fun.NearMissRate))
            : (Thresholds.FirstWinOffCentre(Fun.FirstWinRate), -Feel.EnjoyRate, Thresholds.NearMissOff(Fun.NearMissRate));
        public double Score => -RankKey.Item1 * 10 + RankKey.Item2 * -1 - RankKey.Item3;
    }

    public sealed class SlotOutcome
    {
        public SlotSpec Spec;
        public SearchResult Search;
        public readonly List<Finalist> Checked = new List<Finalist>();
        public List<Finalist> Chosen => Checked.Where(f => f.Accepted).OrderBy(f => f.RankKey).ToList();
        public bool Hard(int want) => Chosen.Count < Math.Min(2, want);
        public double Ms;
    }

    public static class Factory
    {
        /* knobs: an override from the designer agent (centre of the search), or null. */
        public static SlotOutcome RunSlot(SlotSpec spec, FactoryOptions o, Catalog catalog, List<FunSim.Player> players = null, FunThresholds t = null)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            players ??= FunSim.Population(o.Players);
            t ??= FunThresholds.For(spec.Genre);
            var outc = new SlotOutcome { Spec = spec };
            var fun = FunSim.OptionsFor(spec);
            var boosted = FunSim.OptionsFor(spec, boosters: true);
            bool funFirst = o.FunFirst && (spec.Genre == Genre.Zor || spec.Genre == Genre.CokZor);
            Finalist Estimate(Assessment a, FunThresholds th)
            {
                var f = new Finalist
                {
                    Assessment = a, Thresholds = th, FunFirst = funFirst,
                    Fun = FunSim.Run(a.Candidate.Level, spec.Genre, players, options: fun),
                    Boosted = FunSim.Run(a.Candidate.Level, spec.Genre, players, options: boosted)
                };
                f.Psych.AddRange(th.Check(f.Fun, f.Boosted));
                return f;
            }
            outc.Search = Search.Run(spec, o.Seed, o.BudgetFor(spec.Genre), o.BandWant, catalog, o.SearchPlayers, o.Relax, o.Deadline);
            foreach (var a in outc.Search.Passed)
            {
                var f = Estimate(a, t);
                if (o.MemoryTable)
                    f.ByMemory = new[] { 0, .5, 1.0 }.Select(m => m == .5 ? f.Boosted : FunSim.Run(a.Candidate.Level, spec.Genre, players, options: boosted.With(m))).ToArray();
                outc.Checked.Add(f);
            }
            // curve exception: a slot short of two finalists may take its best near-curve candidate (one)
            if (outc.Chosen.Count < 2)
                foreach (var a in outc.Search.CurveNear.OrderByDescending(x => x.Curve.Fit))
                {
                    var f = Estimate(a, t);
                    f.CurveException = true;
                    outc.Checked.Add(f);
                    if (f.Accepted) break;
                }
            // after three rounds, a slot short of two: Orta's early time-out cap 10 %, Zor / Çok zor's band top 70 / 45 %, labelled
            if (FunThresholds.RelaxAllowed(spec.Genre, outc.Chosen.Count, o.Relax))
            {
                var relaxed = t.Relaxed();
                if (spec.Genre == Genre.Orta)
                    foreach (var a in outc.Search.EarlyNear)
                    {
                        if (!FunThresholds.RelaxAllowed(spec.Genre, outc.Chosen.Count, o.Relax)) break;
                        Calibration.FitTimer(a, FunSim.Population(o.SearchPlayers), relaxed, fun);
                        var f = Estimate(a, relaxed);
                        f.EarlyRelaxed = true;
                        outc.Checked.Add(f);
                    }
                else
                    for (int i = 0; i < outc.Checked.Count && FunThresholds.RelaxAllowed(spec.Genre, outc.Chosen.Count, o.Relax); i++)
                    {
                        var f0 = outc.Checked[i];
                        if (f0.Accepted || f0.CurveException || relaxed.Check(f0.Fun, f0.Boosted).Count > 0) continue;
                        var f = new Finalist { Assessment = f0.Assessment, Thresholds = relaxed, Fun = f0.Fun, Boosted = f0.Boosted, ByMemory = f0.ByMemory, FunFirst = funFirst, BandRelaxed = true };
                        outc.Checked[i] = f;
                    }
            }
            outc.Ms = sw.Elapsed.TotalMilliseconds;
            return outc;
        }
    }

    // ---- redesign.json ------------------------------------------------------------------------------

    /* The redesigned set: finalists and approved levels in levels.json's format, each
       with a "fabrika" block. Approved levels carry their R id (never reused; nextId)
       and survive every factory run; a run replaces only the unapproved finalists of
       the slots it ran. The game reads the file like levels.json (extra keys ignored). */
    public sealed class RedesignStore
    {
        public const string Path = "Assets/_Game/Levels/redesign.json";
        public const string Finalist = "finalist", Approved = "onayli", Rejected = "ret";

        public sealed class Entry
        {
            public LevelDraft Draft;
            public int Slot;
            public string Candidate, Status = Finalist, Genre, Aha, Psych, Params, Curve;
            public uint Seed;
            public readonly Dictionary<string, double> Scores = new Dictionary<string, double>();
            public readonly List<string> Notes = new List<string>();
        }

        public int NextId = 1;
        public readonly List<Entry> Entries = new List<Entry>();

        public IEnumerable<Entry> ForSlot(int slot) => Entries.Where(e => e.Slot == slot);

        /* A run's chosen finalists replace the slot's earlier unapproved finalists; keep: they
           stay and the new ones only fill the slot up to `count` (a later round of a short slot). */
        public void PutFinalists(SlotOutcome o, int count, bool keep = false)
        {
            if (!keep) Entries.RemoveAll(e => e.Slot == o.Spec.Slot && e.Status == Finalist);
            int room = count - (keep ? Entries.Count(e => e.Slot == o.Spec.Slot && e.Status == Finalist) : 0);
            foreach (var f in o.Chosen.Where(f => !Entries.Any(e => e.Candidate == f.Candidate.Id)).Take(Math.Max(0, room)))
            {
                var e = new Entry
                {
                    Draft = f.Candidate.Draft.Clone(), Slot = o.Spec.Slot, Candidate = f.Candidate.Id, Seed = f.Candidate.Seed,
                    Genre = o.Spec.Genre.Id(), Params = f.Candidate.Params.ToString()
                };
                e.Draft.Id = f.Candidate.Id;
                e.Draft.Genre = o.Spec.Genre;
                if (f.CurveException) e.Notes.Add("eğri istisnası: uyum " + f.Assessment.Curve.Fit.ToString("0.00") + " (< 0,6)");
                if (f.EarlyRelaxed) e.Notes.Add(FunThresholds.EarlyRelaxedNote);
                if (f.BandRelaxed) e.Notes.Add(string.Format(FunThresholds.BandRelaxedNote, $"%{f.Thresholds.FirstWin.Max * 100:0}"));
                if (f.LeansOnBoosters) e.Notes.Add($"booster'a dayalı: booster'lı keyif booster'sızdan {(f.Boosted.EnjoyRate - f.Fun.EnjoyRate) * 100:0} puan yüksek");
                e.Curve = f.Assessment.Curve != null && f.Assessment.Curve.Defined ? f.Assessment.Curve.Spark() : null;
                Score(e, f);
                Entries.Add(e);
            }
            Entries.Sort((a, b) => a.Slot != b.Slot ? a.Slot.CompareTo(b.Slot) : string.CompareOrdinal(a.Candidate, b.Candidate));
        }

        static void Score(Entry e, Finalist f)
        {
            var a = f.Assessment;
            var m = a.Structure;
            var l = f.Candidate.Level;
            FunScores(e, f.Fun, f.Boosted);
            e.Scores["okuma"] = Math.Round(m.ReadingPerMove, 2);
            e.Scores["k0"] = m.FreeAtStart;
            e.Scores["acikDikis"] = LevelStats.Of(l).OpenPercent;
            e.Scores["darbogaz"] = m.Bottlenecks10;
            e.Scores["govde"] = l.Pieces.Count;
            e.Scores["sure"] = l.TimerSeconds;
            e.Scores["p"] = Math.Round(l.TimerSeconds / (l.Pieces.Count * DifficultyModel.SecondsPerPiece(m.ReadingPerMove, l.Bombs.Count > 0, e.Slot == 1)), 2);
            if (f.CurveException) e.Scores["egriIstisnasi"] = 1;
            if (f.EarlyRelaxed) e.Scores["erkenGevsetildi"] = 1;
            if (f.BandRelaxed) e.Scores["bantGevsetildi"] = 1;
            var slack = Calibration.NailSlack(l);
            if (slack.Count > 0) e.Scores["civiOnemli"] = slack.Count(x => x.slack <= Calibration.NailSlackMax) / (double)slack.Count;
            if (a.Curve != null && a.Curve.Defined) { e.Scores["uyum"] = Math.Round(a.Curve.Fit, 2); e.Scores["zirve"] = Math.Round(a.Curve.PeakAt, 2); }
            HardScores(e, l);
        }

        /* Zeyd's five hardness factors (HardFactors, 2026-10-18), kept with every finalist. */
        public static void HardScores(Entry e, LevelData l)
        {
            var h = HardFactors.Of(l);
            if (h.Dig >= 0) { e.Scores["kazma"] = h.Dig; e.Scores["fitilPay"] = h.FuseSlack; }
            e.Scores["yem"] = Math.Round(h.BaitShare, 2);
            e.Scores["civiKritik"] = h.NailCritical ? 1 : 0;
            e.Scores["caprazZincir"] = h.CrossChains;
            e.Scores["seritSeri"] = h.LaneRun;
            e.Scores["kendiliginden"] = h.Cascade;
            e.Scores["darGecit"] = h.NarrowStretch;
            e.Scores["sahteSerbest"] = h.FakeFree;
            e.Scores["m1Acilis"] = h.FreeAtStart;
            e.Scores["m2Tavan"] = h.MaxFreeEarly;
            e.Scores["m3Bagli3"] = Math.Round(h.Tied3Share, 2);
            e.Scores["m3TekBagli"] = h.MaxSingleTie;
            e.Scores["m3DuzKare"] = h.FlatSquares;
            e.Scores["m5BombaKenar"] = h.BombEdge;
            if (h.NailSlack != int.MaxValue) e.Scores["m6CiviPay"] = h.NailSlack;
            e.Scores["m7Catisma"] = h.ChainConflict && h.ChainEndsApart ? 1 : 0;
            e.Scores["m8DuvarKolay"] = h.EasyWalled;
            e.Scores["m9Kenar"] = Math.Min(h.BombEdge, Math.Min(h.NailEdge, h.ChainEdge));
            e.Scores["mTutan"] = h.Metrics().Count(r => r.ok);
        }

        /* The fun numbers: keyif, frustration, bored and stars from the V5 run (boosted; null = the
           old run without boosters), first try, 3 tries, near-miss and early time-out without
           boosters; the boosterless fun and V5's use rates beside them (PRODUCT.md "V5 çıkış kapısı"). */
        public static void FunScores(Entry e, FunSim.Result plain, FunSim.Result boosted)
        {
            var feel = boosted ?? plain;
            e.Scores["keyif"] = Math.Round(feel.EnjoyRate, 3);
            e.Scores["hayalKirikligi"] = Math.Round(feel.FrustrationRate, 3);
            e.Scores["haksiz"] = Math.Round(feel.UnfairRate, 3);
            e.Scores["sikici"] = Math.Round(feel.BoredRate, 3);
            e.Scores["medyanYildiz"] = feel.MedianStars;
            e.Scores["ilkDeneme"] = Math.Round(plain.FirstWinRate, 3);
            e.Scores["ucDeneme"] = Math.Round(plain.WinBy3Rate, 3);
            e.Scores["nearMiss"] = Math.Round(plain.NearMissRate, 3);
            e.Scores["erkenSure"] = Math.Round(plain.EarlyTimeoutRate, 3);
            if (boosted == null) return;
            e.Scores["keyifSiz"] = Math.Round(plain.EnjoyRate, 3);
            e.Scores["hkSiz"] = Math.Round(plain.FrustrationRate, 3);
            e.Scores["boosterKazanc"] = Math.Round(boosted.BoostedWinShare, 3);
            e.Scores["cekic"] = Math.Round(boosted.HammerRate, 3);
            e.Scores["saat"] = Math.Round(boosted.ClockRate, 3);
            if (boosted.EnjoyRate - plain.EnjoyRate > ReverseSolver.Generation.Finalist.BoosterLeaning + 1e-9) e.Scores["boosteraDayali"] = 1; else e.Scores.Remove("boosteraDayali");
        }

        /* Zeyd's approval: the finalist becomes R<nn> (never reused). */
        public string Approve(Entry e, string note = null)
        {
            if (e.Status == Approved) return e.Draft.Id;
            e.Status = Approved;
            e.Draft.Id = $"R{NextId++:00}";
            if (!string.IsNullOrWhiteSpace(note)) e.Notes.Add("Zeyd: " + note.Trim());
            return e.Draft.Id;
        }

        public void Reject(Entry e, string note)
        {
            e.Status = Rejected;
            if (!string.IsNullOrWhiteSpace(note)) e.Notes.Add("Zeyd: " + note.Trim());
        }

        public string ToJson()
        {
            var lines = new List<string>();
            foreach (var e in Entries)
            {
                var w = new JsonWriter();
                e.Draft.WriteJson(w);
                string level = w.ToString();
                var f = new JsonWriter();
                f.BeginObject().Field("slot", e.Slot).Field("candidate", e.Candidate).Field("seed", unchecked((int)e.Seed))
                 .Field("status", e.Status).Field("genre", e.Genre).Field("params", e.Params).Field("aha", e.Aha).Field("psych", e.Psych).Field("curve", e.Curve);
                f.Key("scores").BeginObject();
                foreach (var kv in e.Scores) f.Key(kv.Key).Raw(kv.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
                f.EndObject();
                f.Key("notes").BeginArray();
                foreach (var n in e.Notes) f.Value(n);
                f.EndArray().EndObject();
                lines.Add("    " + level.Substring(0, level.Length - 1) + ",\"fabrika\":" + f + "}");
            }
            return "{\n  \"format\": 1,\n  \"nextId\": " + NextId + ",\n  \"note\": \"Faz 4 yeniden tasarım seti: fabrika finalistleri ve Zeyd'in onayladıkları (R..). Canlıya yalnızca Zeyd'in komutuyla.\",\n" +
                   "  \"levels\": [\n" + string.Join(",\n", lines) + "\n  ]\n}\n";
        }

        public static RedesignStore Load(string json)
        {
            var s = new RedesignStore();
            var root = JsonReader.Parse(json);
            if (root.TryGet("nextId", out var nid)) s.NextId = (int)nid.AsNumber;
            if (!root.TryGet("levels", out var any) || any.Count == 0) return s;
            var set = LevelParser.Parse(json);
            root.TryGet("levels", out var list);
            for (int i = 0; i < set.Levels.Count; i++)
            {
                var e = new Entry { Draft = LevelDraft.FromLevel(set.Levels[i]) };
                if (list[i].TryGet("fabrika", out var f))
                {
                    e.Slot = Int(f, "slot"); e.Candidate = Str(f, "candidate"); e.Seed = unchecked((uint)Int(f, "seed"));
                    e.Status = Str(f, "status") ?? Finalist; e.Genre = Str(f, "genre"); e.Params = Str(f, "params");
                    e.Aha = Str(f, "aha"); e.Psych = Str(f, "psych"); e.Curve = Str(f, "curve");
                    if (f.TryGet("scores", out var sc)) foreach (var kv in sc.Members) e.Scores[kv.Key] = kv.Value.AsNumber;
                    if (f.TryGet("notes", out var notes)) foreach (var n in notes.Items) e.Notes.Add(n.AsString);
                }
                if (e.Genre != null && GenreExt.TryParse(e.Genre, out var g)) e.Draft.Genre = g;
                s.Entries.Add(e);
            }
            return s;
        }

        public static RedesignStore LoadOrNew(string path = Path) => File.Exists(path) ? Load(File.ReadAllText(path)) : new RedesignStore();
        public void Save(string path = Path) => File.WriteAllText(path, ToJson(), new System.Text.UTF8Encoding(false));

        /* A part store from a parallel run: its finalists replace this store's unapproved
           finalists of the same slots; approved and rejected entries here are kept. */
        public void MergeFinalists(RedesignStore part)
        {
            var slots = part.Entries.Select(e => e.Slot).Distinct().ToList();
            Entries.RemoveAll(e => slots.Contains(e.Slot) && e.Status == Finalist);
            foreach (var e in part.Entries.Where(e => e.Status == Finalist))
                if (!Entries.Any(x => x.Candidate == e.Candidate)) Entries.Add(e);
            Entries.Sort((a, b) => a.Slot != b.Slot ? a.Slot.CompareTo(b.Slot) : string.CompareOrdinal(a.Candidate, b.Candidate));
        }

        static string Str(JsonNode n, string k) => n.TryGet(k, out var v) && v.Kind == JsonKind.String ? v.AsString : null;
        static int Int(JsonNode n, string k) => n.TryGet(k, out var v) && v.Kind == JsonKind.Number ? (int)v.AsNumber : 0;
    }
}
