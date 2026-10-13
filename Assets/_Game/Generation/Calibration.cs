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
        /* A fuse of 5 or less can go off within the first five moves (PM, option 5). */
        public const int MinFuse = 6;
        /* Çok zor in the factory: bomb margin 3-5 (PM 2026-10-14 gece (2), A5; the model table keeps 1-3). */
        public const int CokZorMarginMin = 3, CokZorMarginMax = 5;
        /* Nefes: a bomb is fine but not one that leaves on the first move (shortest dig >= 2). */
        public const int NefesMinDig = 2;
        /* "Çivi önemli": the nailed piece leaves at most 3 moves after its counter ran out, in the
           recorded order (PM 2026-10-14 gece (2), the designer's definition). */
        public const int NailSlackMax = 3;

        /* Moves between a nail's counter running out and its piece leaving, in the level's solution. */
        public static List<(int piece, int counter, int slack)> NailSlack(LevelData level)
        {
            var list = new List<(int, int, int)>();
            var sol = level.Solution;
            foreach (var n in level.Nails)
            {
                int step = -1;
                for (int i = 0; i < sol.Count; i++) if (sol[i].Piece == n.Key) { step = i; break; }
                if (step >= 0) list.Add((n.Key, n.Value, step - n.Value));
            }
            return list;
        }

        /* Easy slots with nails (other than 3 and 4, where it is a condition): search penalty per idle nail. */
        public static double NailPenalty(Assessment a)
        {
            var spec = a.Candidate.Spec;
            bool easy = spec.Genre == Genre.Tanitim || spec.Genre == Genre.Nefes || spec.Genre == Genre.Kolay;
            if (!easy || spec.Slot == 3 || spec.Slot == 4) return 0;
            return NailSlack(a.Candidate.Level).Count(x => x.slack > NailSlackMax) * .5;
        }

        static int MarginFor(Candidate c) =>
            c.Spec.Genre == Genre.CokZor ? Math.Min(CokZorMarginMax, Math.Max(CokZorMarginMin, c.Params?.BombMargin ?? 0))
                                          : DifficultyModel.Of(c.Spec.Genre).BombMargin;
        /* Checkpoint 4: Orta, Zor and Çok zor get their timer from the fun estimate
           (first-try win nearest the band's middle) with the time share floor 1.0. */
        public const double SearchedTimeShareFloor = 1.0;
        public static bool TimeSearched(Genre g) => g == Genre.Orta || g == Genre.Zor || g == Genre.CokZor;
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
                if (spec.Genre == Genre.Nefes && dig.dig < NefesMinDig)
                    a.Unfair.Add($"nefeste bomba ilk hamlede çıkıyor (kazma {dig.dig}, en az {NefesMinDig})");
                d.SetBomb(level.Pieces[b.Key][0], dig.dig + MarginFor(c));
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
            a.OutOfBand.AddRange(Filter(a.Report, spec.Genre));
            if (spec.Genre == Genre.CokZor && level.Bombs.Count > 0 && !double.IsNaN(m.FuseMargin)
                && (m.FuseMargin < CokZorMarginMin || m.FuseMargin > CokZorMarginMax))
                a.OutOfBand.Add($"Bomba payı {m.FuseMargin:0} (fabrika hedefi {CokZorMarginMin}–{CokZorMarginMax})");
            if (spec.Slot == 3 || spec.Slot == 4)
                foreach (var n in NailSlack(level).Where(x => x.slack > NailSlackMax))
                    a.Unfair.Add($"çivi süs: sayaç parçanın sırasından {n.slack} hamle önce bitiyor (en fazla {NailSlackMax})");
            if (CurveMatters(spec.Genre)) a.Curve = Curve.Of(level, CurveRuns);

            a.Unfair.AddRange(UnfairReasons(level, spec, m, a.FormulaTime, s));
            return a;
        }

        /* Rows that follow from the timer the factory itself sets (T from P): on a factory
           candidate they measure the rounding to 5 s, not the board, so they are shown
           but never filter (PM 2026-10-13 ~21:00, A1). The unfair list keeps 1.1 / 75 s / 2. */
        public static readonly string[] InfoOnly = { "Süre payı", "Medyan oyuncunun kalan süresi" };

        /* Band rows a candidate is outside of, as text; empty when inside every band. */
        public static List<string> Filter(DifficultyModel.Report report, Genre? genre = null) =>
            report.Measures.Where(r => r.HasBand && !r.InBand && !InfoOnly.Contains(r.Name)
                                       && !(genre == Genre.CokZor && r.Name == "Bomba payı"))   // the factory's 3-5, checked apart
                           .Select(r => $"{r.Name} {r.Value} (hedef {r.BandText})").ToList();

        /* Checkpoint 4: the timer (5 s steps, from P = 1.0 up to 75 s) whose estimated first-try
           win is nearest the band's middle among the ones whose early time-outs stay under the
           cap. First-try win only grows with the timer, so a bisection finds the band's middle.
           Returns null when no timer keeps early time-outs under the cap. */
        public static FunSim.Result FitTimer(Assessment a, List<FunSim.Player> players, FunThresholds t, FunSim.Options options)
        {
            var d = a.Candidate.Draft;
            var l = a.Candidate.Level;
            double gs = l.Pieces.Count * DifficultyModel.SecondsPerPiece(a.Structure.ReadingPerMove, l.Bombs.Count > 0, a.Candidate.Spec.Slot == 1);
            int lo = DifficultyModel.Ceil5(SearchedTimeShareFloor * gs), hi = DifficultyModel.MaxTime;
            if (lo > hi) return null;
            double target = ((t.FirstWin.Min ?? 0) + (t.FirstWin.Max ?? 1)) / 2;
            var tried = new Dictionary<int, FunSim.Result>();
            FunSim.Result At(int seconds)
            {
                if (tried.TryGetValue(seconds, out var r)) return r;
                d.SetTimer(seconds);
                return tried[seconds] = FunSim.Run(d.ToLevelData(), a.Candidate.Spec.Genre, players, options: options);
            }
            int a5 = lo / 5, b5 = hi / 5;
            while (a5 < b5)                                              // smallest timer reaching the target
            {
                int m = (a5 + b5) / 2;
                if (At(m * 5).FirstWinRate >= target) b5 = m; else a5 = m + 1;
            }
            int best = -1;
            double bestOff = double.MaxValue;
            foreach (int sec in new[] { a5 * 5 - 5, a5 * 5 }.Where(x => x >= lo && x <= hi))
            {
                var r = At(sec);
                if (r.EarlyTimeoutRate > t.MaxEarlyTimeout + 1e-9) continue;
                double off = Math.Abs(r.FirstWinRate - target);
                if (off < bestOff) { bestOff = off; best = sec; }
            }
            if (best < 0)                                                // longer timers until early time-outs fit
                for (int sec = a5 * 5 + 5; sec <= hi && best < 0; sec += 5)
                    if (At(sec).EarlyTimeoutRate <= t.MaxEarlyTimeout + 1e-9) best = sec;
            if (best < 0) { d.SetTimer(a.Candidate.Level.TimerSeconds); return null; }
            d.SetTimer(best);
            a.Candidate.Level = d.ToLevelData();
            return tried[best];
        }

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
            foreach (var b in level.Bombs)
                if (b.Value < MinFuse) u.Add($"fitil {b.Value} ({MinFuse}'dan kısa: ilk 5 hamlede patlayabilir)");
            double share = level.TimerSeconds / (level.Pieces.Count * s);
            double floor = TimeSearched(spec.Genre) ? SearchedTimeShareFloor : MinTimeShare;
            if (share < floor - 1e-9) u.Add($"süre payı {share:0.00} ({floor:0.0#}'in altı)");
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
