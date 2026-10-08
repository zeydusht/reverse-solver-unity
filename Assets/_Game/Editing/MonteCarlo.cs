using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ReverseSolver.Core;

namespace ReverseSolver.Editing
{
    /* Faz 2's structural difficulty (PRODUCT.md, "Zorluk deneyi ilkeleri"):
       how a player who picks moves at random fares. Each run plays one level
       through GameSession's real commands, so nails, the safety valve and
       bombs work exactly as in the game. No boosters, no clock.

       The random player respects nails: at every step it picks uniformly
       among the pieces that can leave right now and are not nailed, then
       uniformly among that piece's exit directions (Mulberry32, seeded, so
       the same seed always plays the same game). The number of such pieces is
       the step's branching. A run ends in a win, a bomb going off, or frozen
       (nothing can leave and the valve did not help). Editor only. */
    public static class MonteCarlo
    {
        public const int DefaultRuns = 200;

        public sealed class Result
        {
            public string LevelId;
            public int Version, Runs, Wins, Bombs, Frozen, RunsWithValve;
            public double MeanMoves, MeanBranching, MeanValves;

            public double WinRate => Runs == 0 ? 0 : (double)Wins / Runs;
            public double BombRate => Runs == 0 ? 0 : (double)Bombs / Runs;
            public double FrozenRate => Runs == 0 ? 0 : (double)Frozen / Runs;
            public double ValveRate => Runs == 0 ? 0 : (double)RunsWithValve / Runs;

            public override string ToString() =>
                $"{LevelId} win {WinRate:P0} bomb {BombRate:P0} frozen {FrozenRate:P0} valve {ValveRate:P0} " +
                $"moves {MeanMoves:0.0} branching {MeanBranching:0.0}";
        }

        /* Seeds firstSeed .. firstSeed + runs - 1. */
        public static Result Run(LevelData level, int runs = DefaultRuns, uint firstSeed = 1)
        {
            var r = new Result { LevelId = level.Id, Version = level.Version, Runs = runs };
            long moves = 0, valves = 0;
            double branchSum = 0;
            long branchSteps = 0;
            var free = new List<int>();
            for (int k = 0; k < runs; k++)
            {
                var pick = new Mulberry32(firstSeed + (uint)k);
                var s = new GameSession(level, 1, "montecarlo", new Mulberry32(firstSeed + (uint)k));
                while (!s.IsOver)
                {
                    free.Clear();
                    foreach (var p in TravelRule.FreePieces(s.Board))
                        if (s.NailAt(p) <= 0) free.Add(p);
                    if (free.Count == 0) break;                         // frozen
                    branchSum += free.Count;
                    branchSteps++;
                    int piece = free[(int)(pick.NextDouble() * free.Count)];
                    var dirs = TravelRule.ExitDirs(s.Board, piece);
                    s.TryExit(piece, dirs[(int)(pick.NextDouble() * dirs.Count)]);
                }
                moves += s.Moves;
                valves += s.ValveCount;
                if (s.ValveCount > 0) r.RunsWithValve++;
                if (s.Outcome == Outcome.Win) r.Wins++;
                else if (s.Outcome == Outcome.Bomb) r.Bombs++;
                else r.Frozen++;
            }
            r.MeanMoves = runs == 0 ? 0 : (double)moves / runs;
            r.MeanValves = runs == 0 ? 0 : (double)valves / runs;
            r.MeanBranching = branchSteps == 0 ? 0 : branchSum / branchSteps;
            return r;
        }

        public static readonly string[] Columns =
        {
            "set", "id", "version", "tohum", "kazanma_orani", "patlama_orani", "donma_orani",
            "supap_orani", "ort_supap", "ort_hamle", "ort_dallanma"
        };

        /* Same Turkish-Excel shape as LevelCsv: ';', decimal comma; write with LevelCsv.ToBytes for the BOM. */
        public static string Csv(IEnumerable<(string set, Result r)> rows)
        {
            var tr = CultureInfo.GetCultureInfo("tr-TR");
            var sb = new StringBuilder(string.Join(";", Columns)).Append("\r\n");
            foreach (var (set, r) in rows)
                sb.Append(string.Join(";", set, r.LevelId, r.Version.ToString(CultureInfo.InvariantCulture),
                    r.Runs.ToString(CultureInfo.InvariantCulture), r.WinRate.ToString("0.000", tr), r.BombRate.ToString("0.000", tr),
                    r.FrozenRate.ToString("0.000", tr), r.ValveRate.ToString("0.000", tr), r.MeanValves.ToString("0.00", tr),
                    r.MeanMoves.ToString("0.0", tr), r.MeanBranching.ToString("0.00", tr))).Append("\r\n");
            return sb.ToString();
        }
    }
}
