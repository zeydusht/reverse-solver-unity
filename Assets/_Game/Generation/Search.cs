using System;
using System.Collections.Generic;
using System.Linq;
using ReverseSolver.Core;
using ReverseSolver.Editing;

namespace ReverseSolver.Generation
{
    /* The factory's search for one slot (G1 "arama", G2): random knobs first, then
       hill climbing around the knobs that came closest to the genre's bands.
       Closeness is BandDistance: 0 inside every band, otherwise the sum of how far
       each row is outside, scaled per row, plus 10 per unfair reason.
       Deterministic for a given slot and seed. Passing boards that look like an
       earlier one (Similarity) are not kept. */
    public sealed class SearchResult
    {
        public SlotSpec Spec;
        public readonly List<Assessment> Passed = new List<Assessment>();
        public int Evaluated, Generated, LookAlikes;
        public int BandPassed;                                 // inside the bands and fair (curve aside)
        public double BestDistance = double.MaxValue;
        public Assessment Best;
        public double Ms;

        /* Share of the generated candidates inside every band and fair (G2's rate). */
        public double PassRate => Generated == 0 ? 0 : (double)BandPassed / Generated;
        public override string ToString() =>
            $"slot {Spec.Slot} {Spec.Genre.Label()}: {Passed.Count} finalist ({BandPassed} bantta, {LookAlikes} benzer atıldı), {Generated}/{Evaluated} üretildi, " +
            $"en yakın {BestDistance:0.00}, {Ms / 1000:0.0} sn";
    }

    public static class Search
    {
        /* Scale per band row: how far outside counts as "1". */
        static readonly Dictionary<string, double> Scale = new Dictionary<string, double>
        {
            ["Gövde"] = 4, ["Açık dikiş %"] = 6, ["İlk hamlede serbest (k₀)"] = 2, ["Hamle başına okuma"] = .5,
            ["Darboğaz"] = 1, ["Bomba payı"] = 2, ["Süre payı"] = .1, ["Medyan oyuncunun kalan süresi"] = .05
        };

        public static double BandDistance(Assessment a)
        {
            double d = 10 * a.Unfair.Count;
            d += Calibration.CurveTerm(a.Candidate.Spec.Genre, a.Curve);
            foreach (var m in a.Report.Measures)
            {
                if (!m.HasBand || m.InBand || Calibration.InfoOnly.Contains(m.Name)) continue;
                double v = ValueOf(a, m.Name);
                var band = BandOf(a.Candidate.Spec.Genre, m.Name);
                double off = band.Min.HasValue && v < band.Min.Value ? band.Min.Value - v : band.Max.HasValue && v > band.Max.Value ? v - band.Max.Value : 0;
                d += off / (Scale.TryGetValue(m.Name, out var s) ? s : 1);
            }
            return d;
        }

        static double ValueOf(Assessment a, string name)
        {
            var l = a.Candidate.Level;
            var m = a.Structure;
            switch (name)
            {
                case "Gövde": return l.Pieces.Count;
                case "Açık dikiş %": return LevelStats.Of(l).OpenPercent;
                case "İlk hamlede serbest (k₀)": return m.FreeAtStart;
                case "Hamle başına okuma": return m.ReadingPerMove;
                case "Darboğaz": return m.Bottlenecks10;
                case "Bomba payı": return m.FuseMargin;
                case "Süre payı": return a.Report.TimeShareNow;
                default: return a.Report.MedianLeftNow;
            }
        }

        static Band BandOf(Genre g, string name)
        {
            var b = DifficultyModel.BandsOf(g);
            switch (name)
            {
                case "Gövde": return b.Pieces;
                case "Açık dikiş %": return b.OpenPercent;
                case "İlk hamlede serbest (k₀)": return b.FreeAtStart;
                case "Hamle başına okuma": return b.Reading;
                case "Darboğaz": return b.Bottleneck;
                case "Bomba payı": return b.BombMargin;
                case "Süre payı": return b.TimeShare;
                default: return b.MedianLeft;
            }
        }

        /* The knobs' starting distribution, by how hard the slot is meant to be. */
        static GenParams Draw(IRandom r, Genre g)
        {
            int rank = g == Genre.Tanitim ? 0 : g == Genre.Nefes || g == Genre.Kolay ? 1 : g == Genre.Orta ? 2 : g == Genre.Zor ? 3 : 4;
            return new GenParams
            {
                Interlock = r.NextDouble() * (2 + 2.5 * rank),
                Sweep = r.NextDouble() * (rank <= 1 ? 1 : 2),
                EdgeFirst = r.NextDouble() * 2 - 1,
                NailLate = r.NextDouble(),
                Knot = r.NextDouble() * 3,
                Ease = r.NextDouble(),
            };
        }

        static GenParams Nudge(GenParams p, IRandom r)
        {
            double N() => (r.NextDouble() + r.NextDouble() + r.NextDouble() - 1.5) * 2 / 3;   // about ±1
            var q = p.Clone();
            q.Interlock = Math.Max(0, q.Interlock + N() * 1.5);
            q.Sweep = Math.Max(0, q.Sweep + N() * .4);
            q.EdgeFirst += N() * .4;
            q.NailLate = Math.Min(1, Math.Max(0, q.NailLate + N() * .2));
            q.Knot = Math.Max(0, q.Knot + N() * .5);
            q.Ease = Math.Min(1, Math.Max(0, q.Ease + N() * .2));
            return q;
        }

        /* Up to `budget` candidates; stops early once `want` distinct boards passed. */
        /* funPlayers > 0: band finalists also get a quick fun estimate, and their first-try
           win outside the genre's band (FunThresholds) adds (distance / 0.1) and keeps them
           out of the finalists. 0 leaves the fun estimate to the factory. */
        public static SearchResult Run(SlotSpec spec, uint seed, int budget = 120, int want = 6, Catalog catalog = null, int funPlayers = 0)
        {
            var res = new SearchResult { Spec = spec };
            var players = funPlayers > 0 ? FunSim.Population(funPlayers) : null;
            var t = FunThresholds.For(spec.Genre);
            var funOptions = FunSim.OptionsFor(spec);
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var rng = new Mulberry32(seed * 2654435761u + (uint)spec.Slot);
            var elite = new List<(double d, GenParams p)>();
            int randomPhase = budget / 3;

            for (int i = 0; i < budget && res.Passed.Count < want; i++)
            {
                GenParams p = i < randomPhase || elite.Count == 0 ? Draw(rng, spec.Genre) : Nudge(elite[(int)(rng.NextDouble() * Math.Min(4, elite.Count))].p, rng);
                uint s = (uint)(rng.NextDouble() * uint.MaxValue) | 1u;
                res.Evaluated++;
                var c = Generator.TryMake(spec, p, s, catalog, out _);
                if (c == null) continue;
                res.Generated++;
                var a = Calibration.Assess(c);
                double d = BandDistance(a);
                bool firstOk = true;
                if (players != null && a.Finalist)
                {
                    var f = FunSim.Run(a.Candidate.Level, spec.Genre, players, options: funOptions);
                    double fw = f.FirstWinRate, lo = t.FirstWin.Min ?? 0, hi = t.FirstWin.Max ?? 1;
                    double off = fw < lo ? lo - fw : fw > hi ? fw - hi : 0;
                    d += off / .1 + (f.UnfairRate > t.MaxUnfair ? (f.UnfairRate - t.MaxUnfair) / .05 : 0);
                    firstOk = off == 0 && f.UnfairRate <= t.MaxUnfair;
                }
                if (d < res.BestDistance) { res.BestDistance = d; res.Best = a; }
                elite.Add((d, p));
                elite.Sort((x, y) => x.d.CompareTo(y.d));
                if (elite.Count > 8) elite.RemoveAt(elite.Count - 1);
                if (a.Passed) res.BandPassed++;
                if (!a.Finalist || !firstOk) continue;
                if (res.Passed.Any(k => Similarity.IsDuplicate(k.Candidate.Level, a.Candidate.Level))) { res.LookAlikes++; continue; }
                res.Passed.Add(a);
            }
            res.Ms = sw.Elapsed.TotalMilliseconds;
            return res;
        }
    }
}
