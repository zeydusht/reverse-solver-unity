using System;
using System.Collections.Generic;
using System.Linq;
using ReverseSolver.Core;

namespace ReverseSolver.Generation
{
    /* G3: the difficulty curve inside one level (PRODUCT.md "Faz 4", "Seviye içi
       eğri"). Along random-player paths (the Monte Carlo player: nails obeyed,
       moves drawn uniformly among the pieces that can leave), every move's
       reading (n + 1) / (k + 1) — n pieces left, k of them can leave — is put in
       the 10 % progress bin it happens in (progress = pieces already out / all).

       Three acts (eğitim seti, modül 4):
         Açılış  0–20 %   easy
         Düğüm   30–70 %  the peak
         Final   70–100 % reading falls; pressure comes from the clock and deep bombs

       Fit (eğri uyumu, 0..1) = mean of
         peak     1 when the highest bin is in the knot, minus 1/3 per bin outside
         opening  how much easier the opening is than the knot: 1 at 20 % below
         final    how much easier the final is than the knot: 1 at 30 % below
       Peak moment (zirve anı) = progress at the centre of the highest bin. */
    public sealed class LevelCurve
    {
        public const int Bins = 10;
        public readonly double[] Reading = new double[Bins];    // mean reading per bin; NaN when no move fell in it
        public readonly int[] Moves = new int[Bins];
        public double Opening, Knot, Final, PeakAt, Fit;
        public double PeakScore, OpeningScore, FinalScore;
        /* No knot to compare with (no move in 30–70 %, or a knot of 0): "eğri yok", never filters. */
        public bool Defined = true;

        public override string ToString() =>
            !Defined ? "eğri yok" :
            $"eğri {string.Join(" ", Reading.Select(r => double.IsNaN(r) ? "–" : r.ToString("0.0")))} · zirve %{PeakAt * 100:0} · uyum {Fit:0.00}";

        /* A tiny text chart for reports: one column per bin, eight levels high. */
        public string Spark()
        {
            const string bars = "▁▂▃▄▅▆▇█";
            double max = Reading.Where(r => !double.IsNaN(r)).DefaultIfEmpty(1).Max();
            return new string(Reading.Select(r => double.IsNaN(r) ? ' ' : bars[Math.Min(7, (int)(r / max * 7.999))]).ToArray());
        }
    }

    public static class Curve
    {
        public const double OpeningEase = .20, FinalEase = .30;

        /* Bombs get long fuses for the curve (as for the dig): the curve is about
           reading; a random player blowing up early would cut it short. */
        public static LevelCurve Of(LevelData level, int runs = 200, uint firstSeed = 1)
        {
            if (level.Bombs.Count > 0)
            {
                var d = ReverseSolver.Editing.LevelDraft.FromLevel(level);
                foreach (var bk in level.Bombs) d.SetBomb(level.Pieces[bk.Key][0], ReverseSolver.Editing.LevelDraft.MaxCount);
                level = d.ToLevelData();
            }
            var c = new LevelCurve();
            var sum = new double[LevelCurve.Bins];
            int total = level.Pieces.Count;
            var free = new List<int>();
            for (int k = 0; k < runs; k++)
            {
                var pick = new Mulberry32(firstSeed + (uint)k);
                var s = new GameSession(level, 1, "curve", new Mulberry32(firstSeed + (uint)k));
                while (!s.IsOver)
                {
                    free.Clear();
                    foreach (var p in TravelRule.FreePieces(s.Board))
                        if (s.NailAt(p) <= 0) free.Add(p);
                    if (free.Count == 0) break;
                    int left = s.Board.PresentCount;
                    int bin = Math.Min(LevelCurve.Bins - 1, (total - left) * LevelCurve.Bins / total);
                    sum[bin] += (left + 1.0) / (free.Count + 1.0);
                    c.Moves[bin]++;
                    int piece = free[(int)(pick.NextDouble() * free.Count)];
                    var dirs = TravelRule.ExitDirs(s.Board, piece);
                    s.TryExit(piece, dirs[(int)(pick.NextDouble() * dirs.Count)]);
                }
            }
            for (int b = 0; b < LevelCurve.Bins; b++) c.Reading[b] = c.Moves[b] == 0 ? double.NaN : sum[b] / c.Moves[b];
            Score(c);
            return c;
        }

        static double MeanOf(LevelCurve c, int from, int to)
        {
            var v = Enumerable.Range(from, to - from + 1).Select(b => c.Reading[b]).Where(r => !double.IsNaN(r)).ToList();
            return v.Count == 0 ? double.NaN : v.Average();
        }

        static void Score(LevelCurve c)
        {
            int peak = -1;
            for (int b = 0; b < LevelCurve.Bins; b++)
                if (!double.IsNaN(c.Reading[b]) && (peak < 0 || c.Reading[b] > c.Reading[peak])) peak = b;
            if (peak < 0) { c.Defined = false; return; }
            c.PeakAt = (peak + .5) / LevelCurve.Bins;
            c.Opening = MeanOf(c, 0, 1);
            c.Knot = Enumerable.Range(3, 4).Select(b => c.Reading[b]).Where(r => !double.IsNaN(r)).DefaultIfEmpty(double.NaN).Max();
            c.Final = MeanOf(c, 7, 9);
            if (double.IsNaN(c.Knot) || c.Knot <= 0) { c.Defined = false; return; }
            int outside = peak < 3 ? 3 - peak : peak > 6 ? peak - 6 : 0;
            c.PeakScore = Math.Max(0, 1 - outside / 3.0);
            c.OpeningScore = double.IsNaN(c.Knot) || double.IsNaN(c.Opening) ? 0 : Clamp01((c.Knot - c.Opening) / c.Knot / OpeningEase);
            c.FinalScore = double.IsNaN(c.Knot) || double.IsNaN(c.Final) ? 0 : Clamp01((c.Knot - c.Final) / c.Knot / FinalEase);
            c.Fit = (c.PeakScore + c.OpeningScore + c.FinalScore) / 3;
        }

        static double Clamp01(double v) => v < 0 ? 0 : v > 1 ? 1 : v;
    }
}
