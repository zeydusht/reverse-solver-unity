using System;
using System.Collections.Generic;
using System.Linq;
using ReverseSolver.Core;
using ReverseSolver.Editing;

namespace ReverseSolver.Generation
{
    /* What G2 decided about one candidate. */
    public sealed class Assessment
    {
        public Candidate Candidate;                       // calibrated: timer, fuses, solution
        public StructureMetrics Structure;
        public DifficultyModel.Report Report;
        public int FormulaTime;                           // ⌈g·s·P⌉₅ before the 75 s cap
        public readonly List<string> OutOfBand = new List<string>();
        public readonly List<string> Unfair = new List<string>();
        public bool Passed => OutOfBand.Count == 0 && Unfair.Count == 0;
        /* G3 curve (Orta, Zor, Çok zor only; null elsewhere) and whether it meets the finalist bar. */
        public LevelCurve Curve;
        public bool CurveOk => !Calibration.CurveMatters(Candidate.Spec.Genre) || Curve == null || !Curve.Defined || Curve.Fit >= Calibration.MinCurveFit;
        public bool Finalist => Passed && CurveOk;

        public override string ToString() =>
            $"{Candidate.Id}: {(Passed ? "geçti" : "elendi")}" +
            (OutOfBand.Count > 0 ? " bant dışı: " + string.Join(", ", OutOfBand) : "") +
            (Unfair.Count > 0 ? " haksız: " + string.Join(", ", Unfair) : "");
    }

    /* G2 (PRODUCT.md "Faz 4"): set the timer, fuses and nail counters with the
       difficulty model, then drop what is outside the slot genre's bands or on
       the "haksız zorluk: yapma listesi".

         timer   T = ⌈g · s · P⌉₅, s from the structure (0.6 + 0.16 × reading,
                 +0.1 with a bomb, 1.35 on the game's first level)
         fuse    shortest dig (searched with long fuses) + the genre's margin;
                 the bomb introduction wants a dig of 2-3 and margin 10
         nails   the genre's share of the bodies (already set by G1)

       Bands: every row DifficultyModel.Analyze checks. A row tied to an
       obstacle (bomb margin) only exists when the level has that obstacle, so a
       bomb-free Zor candidate is never dropped for its bomb margin.

       Unfair list (each a separate reason):
         bomb margin 0 or less · time share under 1.1 · timer over 75 s ·
         cells under 44 pt on a 375 x 667 phone (K1 layout) · more than one new
         thing, or a thing before its introduction slot · an introduction with
         a time share under 2 · the bomb introduction's dig outside 2-3. */
    public static class Calibration
    {
        public const double MinTimeShare = 1.1, IntroTimeShare = 2.0;
        public const int IntroBombDigMin = 2, IntroBombDigMax = 3;

        public static Assessment Assess(Candidate c)
        {
            var a = new Assessment { Candidate = c };
            var spec = c.Spec;
            var d = c.Draft;
            var level = c.Level;
            bool first = spec.Slot == 1;

            var m = StructureMetrics.Of(level);
            int g = level.Pieces.Count;
            bool hasBomb = level.Bombs.Count > 0;

            // fuses from the shortest dig
            foreach (var b in level.Bombs)
            {
                if (!m.BombDigs.TryGetValue(b.Key, out var dig)) { a.Unfair.Add("bomba hiç kazılamıyor"); continue; }
                if (spec.Genre == Genre.Tanitim && spec.NewThing == SlotSpec.Bomb && (dig.dig < IntroBombDigMin || dig.dig > IntroBombDigMax))
                    a.Unfair.Add($"bomba tanıtımında bomba {dig.dig} hamle uzakta (2–3 olmalı)");
                d.SetBomb(level.Pieces[b.Key][0], DifficultyModel.Fuse(dig.dig, spec.Genre));
            }

            // timer
            double s = DifficultyModel.SecondsPerPiece(m.ReadingPerMove, hasBomb, first);
            a.FormulaTime = DifficultyModel.Time(g, s, DifficultyModel.Of(spec.Genre).P);
            d.SetTimer(Math.Min(a.FormulaTime, DifficultyModel.MaxTime));

            // the calibrated level, solved again (shorter fuses may change what wins)
            level = d.ToLevelData();
            var check = DesignCheck.Run(level, DesignCheck.QuickBudget * 5);
            if (!check.FollowsRule) { a.Unfair.Add("ayardan sonra çözülemiyor: " + check.Verdict); }
            else
            {
                d.Solution = check.Solution;
                level = d.ToLevelData();
            }
            c.Level = level;

            m = StructureMetrics.Of(level);
            a.Structure = m;
            a.Report = DifficultyModel.Analyze(level, spec.Genre, m, double.NaN, first);
            a.OutOfBand.AddRange(Filter(a.Report));
            if (CurveMatters(spec.Genre)) a.Curve = Curve.Of(level, CurveRuns);

            a.Unfair.AddRange(UnfairReasons(level, spec, m, a.FormulaTime, s));
            return a;
        }

        /* Rows that follow from the timer the factory itself sets (T from P): on a factory
           candidate they measure the rounding to 5 s, not the board, so they are shown
           but never filter (PM 2026-10-13 ~21:00, A1). The unfair list keeps 1.1 / 75 s / 2. */
        public static readonly string[] InfoOnly = { "Süre payı", "Medyan oyuncunun kalan süresi" };

        /* Band rows a candidate is outside of, as text; empty when inside every band. */
        public static List<string> Filter(DifficultyModel.Report report) =>
            report.Measures.Where(r => r.HasBand && !r.InBand && !InfoOnly.Contains(r.Name))
                           .Select(r => $"{r.Name} {r.Value} (hedef {r.BandText})").ToList();

        // ---- G3: the curve as a finalist condition ----------------------------------------------

        public const double MinCurveFit = .6;
        public const int CurveRuns = 60;

        public static bool CurveMatters(Genre g) => g == Genre.Orta || g == Genre.Zor || g == Genre.CokZor;

        /* What the curve adds to the search's band distance: max(0, 0.6 - fit) / 0.2 for
           Orta, Zor, Çok zor; nothing for the easy kinds or an undefined curve. */
        public static double CurveTerm(Genre g, LevelCurve c) =>
            !CurveMatters(g) || c == null || !c.Defined ? 0 : Math.Max(0, MinCurveFit - c.Fit) / .2;

        /* The "haksız zorluk" list on a calibrated level; empty when clean. */
        public static List<string> UnfairReasons(LevelData level, SlotSpec spec, StructureMetrics m, int formulaTime, double s)
        {
            var u = new List<string>();
            if (level.Bombs.Count > 0 && !double.IsNaN(m.FuseMargin) && m.FuseMargin <= 0) u.Add($"bomba payı {m.FuseMargin:0} (0 ya da altı)");
            double share = level.TimerSeconds / (level.Pieces.Count * s);
            if (share < MinTimeShare) u.Add($"süre payı {share:0.00} (1,1'in altı)");
            if (formulaTime > DifficultyModel.MaxTime || level.TimerSeconds > DifficultyModel.MaxTime) u.Add($"süre {formulaTime} sn (75'ten uzun)");
            int cell = Layout.CellSize(level.Width, level.Height);
            if (cell < Layout.MinTouchCell) u.Add($"hücre {cell} pt (44'ün altı)");
            foreach (var thing in NewThingsIn(level, spec)) u.Add(thing);
            if (spec.Genre == Genre.Tanitim && share < IntroTimeShare) u.Add($"tanıtımda süre payı {share:0.00} (2'nin altı)");
            return u;
        }

        /* "Tek yenilik": what the level shows against the introduction calendar. */
        static IEnumerable<string> NewThingsIn(LevelData level, SlotSpec spec)
        {
            var st = LevelStats.Of(level);
            var present = new List<string>();
            if (st.Nails > 0) present.Add(SlotSpec.Nail);
            if (st.SealedLanes > 0) present.Add(SlotSpec.Seal);
            if (st.Chains > 0) present.Add(SlotSpec.Chain);
            if (st.Bombs > 0) present.Add(SlotSpec.Bomb);
            foreach (var t in present)
                if (SlotSpec.IntroducedAt[t] > spec.Slot) yield return $"{t} slot {SlotSpec.IntroducedAt[t]}'den önce";
            int news = present.Count(t => SlotSpec.IntroducedAt[t] == spec.Slot)
                     + (spec.NewThing != null && !SlotSpec.IsObstacle(spec.NewThing) ? 1 : 0);
            if (news > 1) yield return $"{news} yeni şey (en fazla 1)";
        }

        // ---- the look: palette x pattern readability -----------------------------------------------

        public const float MinReadability = .30f;

        /* Seam readability of a two-colour board (SeamReadability on the cell colours). */
        public static float Readability(LevelData level) => Readability(level.Width, level.Height, level.Colors, level.Palette);

        public static float Readability(int boardW, int boardH, int[] colors, IReadOnlyList<string> palette, int px = 24)
        {
            int w = boardW * px, h = boardH * px;
            var lum = new float[w * h];
            var pal = palette.Select(Lightness).ToArray();
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    lum[y * w + x] = pal[colors[(y / px) * boardW + x / px]];
            return SeamReadability.Score(lum, w, h, boardW, boardH);
        }

        public static float Lightness(string hex)
        {
            int v = Convert.ToInt32(hex.TrimStart('#'), 16);
            return (.299f * ((v >> 16) & 255) + .587f * ((v >> 8) & 255) + .114f * (v & 255)) / 255f;
        }
    }
}
