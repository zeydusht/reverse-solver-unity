using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReverseSolver.Core;
using ReverseSolver.Editing;
using ReverseSolver.Generation;
using UnityEngine;

namespace ReverseSolver.EditorTools
{
    /* PM_68: calibrate FunSim to pt1's real first-try wins (slots 1-20 of the redesign set as pt1 played it), then
       search the smallest fuse/timer change that brings 11-20 into the target band, and pick a slot-19 replacement.
       Run: bash Tools/unity_batch.sh method ReverseSolver.EditorTools.Pm68Tune.Run   (prints [pm68] lines) */
    public static class Pm68Tune
    {
        // pt1, attempt 1, v_attempts (14 Nov): slot -> first-try win rate, players
        static readonly double[] Pt1 = { .98, .94, .95, .75, 1.00, .97, .97, .63, .38, .74, .29, .71, .60, .94, .94, .43, .14, .69, .00, .33 };
        static readonly int[] Pt1N = { 51, 49, 44, 40, 36, 33, 33, 32, 29, 23, 21, 21, 20, 17, 16, 14, 14, 13, 13, 12 };

        public sealed class Calibration { public double Speed = 1, Attention, Skill; public Genre Genre = Genre.Orta; public double Mae = 1; }

        static List<FunSim.Player> Shift(List<FunSim.Player> b, Calibration c) => b.Select(p => new FunSim.Player
        {
            Speed = p.Speed * c.Speed, Skill = Math.Min(1, Math.Max(0, p.Skill + c.Skill)),
            BombAttention = Math.Min(1, Math.Max(0, p.BombAttention + c.Attention)), Patience = p.Patience, Retry = p.Retry
        }).ToList();

        public static double Rate(LevelData l, Calibration c, List<FunSim.Player> players, bool bombIntro = false) =>
            FunSim.Run(l, c.Genre, players, options: new FunSim.Options { BombIntro = bombIntro }).FirstWinRate;

        public static IReadOnlyList<LevelData> Original() => LevelParser.Parse(File.ReadAllText("Assets/_Game/Levels/redesign.json")).Levels;

        public static Calibration Calibrate(IReadOnlyList<LevelData> levels, List<FunSim.Player> base0, out double[] fitted)
        {
            var best = new Calibration();
            fitted = null;
            foreach (var genre in new[] { Genre.Kolay, Genre.Orta })
                foreach (double speed in new[] { 1.0, 1.1, 1.2, 1.3, 1.45 })
                    foreach (double attention in new[] { .2, .4, .6, .8 })
                        foreach (double skill in new[] { -.1, 0, .1, .2 })
                        {
                            var c = new Calibration { Speed = speed, Attention = attention, Skill = skill, Genre = genre };
                            var players = Shift(base0, c);
                            var rates = new double[20];
                            double err = 0, weight = 0;
                            for (int i = 0; i < 20; i++)
                            {
                                rates[i] = Rate(levels[i], c, players, levels[i].Intro?.Icon == "bomb");
                                err += Pt1N[i] * Math.Abs(rates[i] - Pt1[i]); weight += Pt1N[i];
                            }
                            c.Mae = err / weight;
                            if (c.Mae < best.Mae) { best = c; fitted = rates; }
                        }
            return best;
        }

        public static void Run()
        {
            var original = Original();
            var base0 = FunSim.Population(160);
            var cal = Calibrate(original, base0, out var fitted);
            var players = Shift(base0, cal);
            Debug.Log($"[pm68] calibration genre={cal.Genre} speed={cal.Speed} attention={cal.Attention} skill={cal.Skill} weighted MAE={cal.Mae:0.000}");
            double plain = 0;
            for (int i = 0; i < 20; i++)
            {
                plain += Math.Abs(fitted[i] - Pt1[i]);
                Debug.Log($"[pm68] cal {i + 1} {original[i].Id} pt1={Pt1[i]:0.00} sim={fitted[i]:0.00} diff={fitted[i] - Pt1[i]:+0.00;-0.00}");
            }
            Debug.Log($"[pm68] plain MAE={plain / 20:0.000}");

            var p22 = P21Onboarding.SpaceObstacleLessons(new LevelSet(1, original.ToList())).Levels;
            for (int i = 0; i < 20; i++)
                Debug.Log($"[pm68] before {i + 1} {p22[i].Id} v{p22[i].Version} bombs={p22[i].Bombs.Count} sim={Rate(p22[i], cal, players):0.00}");

            // fuse/timer search for 11-20 normal levels. Estimate = pt1 real + (sim tuned - sim as pt1 played it).
            for (int slot = 11; slot <= 20; slot++)
            {
                if (P21Onboarding.IsObstacleLesson(slot) || slot == 19) continue;
                var l = p22[slot - 1];
                double sim0 = fitted[slot - 1], real = Pt1[slot - 1];
                double Est(LevelData t) => Math.Min(1, Math.Max(0, real + Rate(t, cal, players) - sim0));
                double e0 = Est(l);
                string bestKey = "none"; double bestE = e0, bestCost = e0 >= .55 && e0 <= .75 ? 0 : double.MaxValue;
                if (bestCost > 0)
                    foreach (int fuse in l.Bombs.Count > 0 ? new[] { 0, 2, 3, 4, 6, 8 } : new[] { 0 })
                        foreach (int timer in new[] { -20, -15, -10, -5, 0, 10, 20, 30, 45 })
                        {
                            if (fuse == 0 && timer == 0) continue;
                            if (e0 > .75 && (fuse > 0 || timer > 0)) continue;
                            if (e0 < .55 && timer < 0) continue;
                            double e = Est(P21Onboarding.Tuned(l, fuse, timer));
                            double cost = fuse + Math.Abs(timer) / 5.0 + (e < .55 || e > .75 ? 100 : 0) + Math.Abs(e - .65);
                            if (cost < bestCost) { bestCost = cost; bestKey = $"fuse+{fuse} timer{timer:+0;-0}"; bestE = e; }
                        }
                Debug.Log($"[pm68] tune {slot} {l.Id} real={real:0.00} sim0={sim0:0.00} est {e0:0.00} -> {bestE:0.00} with {bestKey}");
            }

            foreach (var l in original.Skip(20))
            {
                if (l.Bombs.Count > 1) continue;
                Debug.Log($"[pm68] candidate {l.Number} {l.Id} {l.Width}x{l.Height} bombs={l.Bombs.Count} hard={l.Hard} sim={Rate(l, cal, players):0.00}");
            }
            var s28 = original.First(l => l.Id == "S28");
            foreach (int fuse in new[] { 0, 4, 8 })
                Debug.Log($"[pm68] S28 hard fuse+{fuse} sim={Rate(P21Onboarding.Tuned(s28, fuse, 0), cal, players):0.00}");
        }
    }
}
