using System;
using System.Collections.Generic;
using System.Linq;
using ReverseSolver.Core;
using ReverseSolver.Editing;

namespace ReverseSolver.Generation
{
    /* PM_70: difficulty model v2 (docs/analiz/ZORLUK_MODELI_v2.md §5, coefficients docs/analiz/v2/model_v2.json), fitted on
       pt1 with the PM_69 measures. Leave-one-out error ±10.7 points.
         p_win1        average player's first-try win
         jam_per_exit  jams per exit ("düşündürme", axis B) */
    public static class DifficultyModelV2
    {
        public const double MixAware = .85;
        static readonly double[] B = { 1.610541695955902, 0.5333202613860115, -1.7059882743187815, -1.8425707216470628 };

        public static double Logit(double p) => Math.Log((p + .01) / (1.01 - p));

        /* Bomb risk: logit of not surviving, players being 85 % bomb-aware and 15 % random. 1 without bombs. */
        public static double BombRisk(bool bombs, double pSurviveAware, double pSurviveRand) =>
            -Logit(bombs ? MixAware * pSurviveAware + (1 - MixAware) * pSurviveRand : 1.0);

        public static double D(bool bombs, double pSurviveAware, double pSurviveRand, double secPerMove, double narrowByChain, int pieces) =>
            B[0] + B[1] * BombRisk(bombs, pSurviveAware, pSurviveRand) + B[2] * Math.Log(secPerMove) + B[3] * (narrowByChain / pieces);

        public static double PWin1(bool bombs, double pSurviveAware, double pSurviveRand, double secPerMove, double narrowByChain, int pieces) =>
            1 / (1 + Math.Exp(D(bombs, pSurviveAware, pSurviveRand, secPerMove, narrowByChain, pieces)));

        public static double JamPerExit(double narrowByLock, double narrowByWall, double narrowByChain, double dirAmbiguity, int pieces) =>
            .18 + 2.07 * narrowByLock / pieces + .57 * narrowByWall / pieces + .56 * narrowByChain / pieces + .40 * dirAmbiguity;

        /* An untimed level (timer 0) counts as 10 s per piece: plenty (pt1's most generous was 13.3, S41). */
        public const double UntimedSecPerMove = 10;
        public static double SecPerMove(LevelData l) => l.TimerSeconds <= 0 ? UntimedSecPerMove : (double)l.TimerSeconds / l.Pieces.Count;

        public static double PWin1(DifficultyV2.Features f) => PWin1(f, SecPerMove(f.Level));
        public static double PWin1(DifficultyV2.Features f, double secPerMove) => secPerMove <= 0 ? PWin1(f, UntimedSecPerMove) :
            PWin1(f.BombPieces.Length > 0, f.Aware.PSurvive, f.Rand.PSurvive, secPerMove, f.Prof.NarrowByChain, f.Level.Pieces.Count);
        /* PM_71: the model blended with what a cohort really did (PlayData; pt1 by default).
             veriyle  = sigmoid(-d_obs + logit p_model(now) - logit p_model(as played))   the data, moved by the model's change
             birlesik = sigmoid((1 - w) logit p_model + w logit veriyle),  w = n / (n + 10)
           A level the cohort never played (a factory candidate, a new id) gets the model alone. */
        public readonly struct Blend
        {
            public readonly double Model, Data, Blended, Weight; public readonly int N;
            public Blend(double model, double data, double blended, double weight, int n) { Model = model; Data = data; Blended = blended; Weight = weight; N = n; }
            public bool HasData => N > 0;
        }
        public static double Logit0(double p) { p = Math.Min(1 - 1e-4, Math.Max(1e-4, p)); return Math.Log(p / (1 - p)); }
        public static double Sigmoid(double x) => 1 / (1 + Math.Exp(-x));
        public static Blend PWin1Parts(DifficultyV2.Features f, PlayData data = null)
        {
            double m = PWin1(f);
            data = data ?? PlayData.Pt1;
            var row = data?.Find(f.Level.Id);
            if (row == null) return new Blend(m, double.NaN, m, 0, 0);
            double data1 = Sigmoid(-row.D + Logit0(m) - Logit0(data.ModelAtPlay(row)));
            double w = row.N / (row.N + PlayData.Prior);
            return new Blend(m, data1, Sigmoid((1 - w) * Logit0(m) + w * Logit0(data1)), w, row.N);
        }
        public static double PWin1Blended(DifficultyV2.Features f, PlayData data = null) => PWin1Parts(f, data).Blended;
        public static double PWin1Blended(LevelData level, PlayData data = null) => PWin1Blended(DifficultyV2.Compute(level), data);

        public static double JamPerExit(DifficultyV2.Features f) =>
            JamPerExit(f.Prof.NarrowByLock, f.Prof.NarrowByWall, f.Prof.NarrowByChain, f.Prof.DirAmbiguity, f.Level.Pieces.Count);

        /* The timer (whole seconds) that puts p_win1 nearest the target inside the allowed sec/piece band. */
        public static int TimerFor(DifficultyV2.Features f, double target, double minSpm = Rules.MinSecPerMove, double maxSpm = Rules.MaxSecPerMove)
        {
            int n = f.Level.Pieces.Count, best = (int)Math.Ceiling(minSpm * n);
            double bestOff = double.MaxValue;
            for (int t = (int)Math.Ceiling(minSpm * n); t <= (int)Math.Floor(maxSpm * n); t++)
            {
                double off = Math.Abs(PWin1(f, (double)t / n) - target);
                if (off < bestOff - 1e-12) { bestOff = off; best = t; }
            }
            return best;
        }

        public enum Role { Lesson, Normal, Hard, Breather }

        /* The factory's slot role: Zor / Çok zor are hard, Tanıtım a lesson, Nefes a breather. */
        public static Role RoleOf(Genre g) =>
            g == Genre.Zor || g == Genre.CokZor ? Role.Hard : g == Genre.Tanitim ? Role.Lesson : g == Genre.Nefes ? Role.Breather : Role.Normal;

        public sealed class Estimate
        {
            public DifficultyV2.Features Features;
            public double PWin1, JamPerExit;   // PWin1: blended with the data (PM_71) when the level was played
            public double PWin1Model, PWin1Data = double.NaN;
            public List<string> Broken;
        }

        public static Estimate Of(LevelData level, int slot, Role role, int games = 2000)
        {
            var f = DifficultyV2.Compute(level, games);
            var b = PWin1Parts(f);
            return new Estimate { Features = f, PWin1 = b.Blended, PWin1Model = b.Model, PWin1Data = b.Data, JamPerExit = JamPerExit(f), Broken = Rules.Check(f, slot, role) };
        }

        /* PM_70 target curve (p22, model v2). Breather: >= .75. */
        public static (double lo, double hi) Band(int slot, Role role)
        {
            if (role == Role.Lesson) return (.85, 1);
            if (role == Role.Breather) return (.75, 1);
            if (slot <= 20) return role == Role.Hard ? (.35, .45) : (.55, .75);
            if (slot <= 30) return role == Role.Hard ? (.35, .45) : (.50, .70);
            return role == Role.Hard ? (.30, .45) : (.45, .65);
        }

        /* PM_70 design rules (docs/kit/KIT_NOTLARI.md). Returns the broken ones, empty when all hold. */
        public static class Rules
        {
            public const double MinSecPerMove = 1.7, MaxSecPerMove = 2.3, MinJam = .5, DistractOpen = 5, RandSafe = .25;
            public const int SlackOpenBoard = 8, ChokeNormal = 1, ChokeHard = 2;

            public static List<string> Check(DifficultyV2.Features f, int slot, Role role)
            {
                var u = new List<string>();
                var l = f.Level;
                if (role == Role.Lesson) return u;
                int chokeMax = role == Role.Hard ? ChokeHard : ChokeNormal;
                if (f.ChokeN > chokeMax) u.Add($"boğaz {f.ChokeN} (≤ {chokeMax})");
                double spm = SecPerMove(l);
                if (spm < MinSecPerMove - 1e-9 || spm > MaxSecPerMove + 1e-9) u.Add($"sn/parça {spm:0.00} ({MinSecPerMove}–{MaxSecPerMove})");
                // an open board (many distractors) needs a loose fuse or a board a random player survives
                for (int i = 0; i < f.BombPieces.Length; i++)
                {
                    double d = f.Prof.DistractPerStep[i];
                    if (double.IsNaN(d) || d < DistractOpen) continue;
                    if (f.Rand.PSurvive < RandSafe && f.SlackTrue[i] < SlackOpenBoard)
                        u.Add($"açık tahtada sıkı bomba {f.BombPieces[i]}: çeldirici {d:0.0}, pay {f.SlackTrue[i]}, rastgele %{f.Rand.PSurvive * 100:0}");
                }
                if (slot >= 21 && JamPerExit(f) < MinJam - 1e-9) u.Add($"düşündürme {JamPerExit(f):0.00} (≥ {MinJam})");
                return u;
            }
        }

        /* Curve rules over a whole set (slots 11-40): bands, hard counts, no hard next to a hard, breathers after hards. */
        public static List<string> CheckCurve(IReadOnlyList<(int slot, Role role, double p)> rows)
        {
            var u = new List<string>();
            var by = rows.ToDictionary(r => r.slot);
            foreach (var r in rows)
            {
                var (lo, hi) = Band(r.slot, r.role);
                // PM_71: the bands are read to two decimals (the PM's tables); .754 counts as .75
                double read = Math.Round(r.p, 2, MidpointRounding.AwayFromZero);
                if (read < lo - 1e-9 || read > hi + 1e-9) u.Add($"{r.slot} {r.role} {r.p:0.000} ({lo:0.00}–{hi:0.00})");
                if (r.role == Role.Hard && by.TryGetValue(r.slot + 1, out var next) && next.role == Role.Hard) u.Add($"{r.slot}–{r.slot + 1} art arda zor");
                if (r.role == Role.Hard && r.slot >= 21 && r.slot <= 30 && (!by.TryGetValue(r.slot + 1, out var b) || b.p < .75 - 1e-9))
                    u.Add($"{r.slot} zorun ardından nefes yok");
            }
            int Hard(int a, int z) => rows.Count(r => r.slot >= a && r.slot <= z && r.role == Role.Hard);
            if (Hard(11, 20) > 2) u.Add($"11–20 zor {Hard(11, 20)} (≤ 2)");
            if (Hard(21, 30) > 2) u.Add($"21–30 zor {Hard(21, 30)} (≤ 2)");
            if (Hard(31, 40) > 3) u.Add($"31–40 zor {Hard(31, 40)} (≤ 3)");
            if (by.TryGetValue(40, out var last) && (last.role != Role.Hard || Math.Abs(last.p - .35) > .05 + 1e-9)) u.Add($"40 {last.role} {last.p:0.00} (zor, ≈ .35)");
            int breath = rows.Count(r => r.slot >= 31 && r.slot <= 40 && r.p >= .75 - 1e-9);
            if (breath < 1 || breath > 2) u.Add($"31–40 nefes {breath} (1–2)");
            return u;
        }
    }
}
