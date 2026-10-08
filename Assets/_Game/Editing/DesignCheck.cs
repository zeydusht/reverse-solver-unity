using System;
using System.Collections.Generic;
using ReverseSolver.Core;

namespace ReverseSolver.Editing
{
    public enum DesignVerdict
    {
        Solvable,       // cleared with 0 bombs, 0 freezes, 0 safety-valve pulls
        NeedsValve,     // cleared, but only because the safety valve popped a nail
        Frozen,         // pieces left that can never leave
        Bomb,           // no order found that takes a bomb out in time
        Unverified      // the bomb search ran out of budget: neither proved nor refuted
    }

    public sealed class DesignReport
    {
        public DesignVerdict Verdict;
        public string Message;                          // Turkish, for the editor panel
        public readonly List<int> Pieces = new List<int>();    // where it got stuck / what went wrong
        public readonly List<Move> Solution = new List<Move>();
        public int Nodes;
        public LevelStats Stats;
        public int CellSize;                            // on a 375x667 phone
        public bool Fits => CellSize >= Layout.MinTouchCell;

        /* The design rule: solvable with 0 bombs, 0 freezes and 0 valve pulls. */
        public bool FollowsRule => Verdict == DesignVerdict.Solvable;
    }

    /* The phone check: the game's own board size rule (Core BoardLayout). */
    public static class Layout
    {
        public const int MinTouchCell = (int)BoardLayout.MinCell;

        public static int CellSize(int width, int height, float screenW = 375, float screenH = 667) =>
            (int)BoardLayout.CellSize(screenW, screenH, width, height);
    }

    /* The editor's solvability check, in two steps:
         1. the web game's own solvable() (GreedySolver): if the pieces cannot
            all leave in any order, the board is frozen whatever the nails and
            bombs do, and the stuck pieces are the answer;
         2. SmartPlayer (the player the 40/40 guarantee is checked with) on a
            GameSession for nails, bombs and the safety valve.
       So the editor and the game cannot disagree about a level.

       The bomb search costs ~16k nodes a second here. The web levels need at
       most 213 nodes, so the live panel uses QuickBudget (well under a second);
       when that runs out the verdict is Unverified and a deep check with the
       full budget can settle it. */
    public static class DesignCheck
    {
        public const int QuickBudget = 10_000;

        public static DesignReport Run(LevelData level, int nodeBudget = SmartPlayer.DefaultNodeBudget)
        {
            var report = new DesignReport
            {
                Stats = LevelStats.Of(level),
                CellSize = Layout.CellSize(level.Width, level.Height)
            };

            var greedy = GreedySolver.Solve(level);
            if (!greedy.Solved)
            {
                report.Verdict = DesignVerdict.Frozen;
                report.Pieces.AddRange(greedy.Stuck);
                report.Message = $"Çözülemez: {greedy.Stuck.Count} parça hiçbir sırayla çıkamıyor (kilitli kenarlar ya da duvarlar).";
                return report;
            }

            var r = SmartPlayer.Play(level, nodeBudget);
            report.Nodes = r.Nodes;

            // Replay the moves to see where it ended and which nails the valve popped.
            var s = new GameSession(level, 1, "editor", new Mulberry32(1));
            var valved = new List<int>();
            s.EventRaised += e => { if (e.Kind == EventKind.ValveReleased) valved.Add(e.Piece); };
            foreach (var m in r.Moves) s.TryExit(m.Piece, m.Dir);
            var left = new List<int>(s.Board.PresentPieces());

            switch (r.Verdict)
            {
                case SmartVerdict.Won when r.Valves == 0:
                    report.Verdict = DesignVerdict.Solvable;
                    report.Solution.AddRange(r.Moves);
                    report.Message = $"Çözülebilir: {r.Moves.Count} hamle; patlama, donma ve supap yok.";
                    break;
                case SmartVerdict.Won:
                    report.Verdict = DesignVerdict.NeedsValve;
                    report.Solution.AddRange(r.Moves);
                    report.Pieces.AddRange(valved);
                    report.Message = $"Kural dışı: çözüm {r.Valves} kez supap gerektiriyor (oynanabilir parçaların hepsi aynı anda çivili kalıyor).";
                    break;
                case SmartVerdict.Frozen:
                    report.Verdict = DesignVerdict.Frozen;
                    report.Pieces.AddRange(left);
                    report.Message = $"Çözülemez: {left.Count} parça sıkıştı, hiçbiri hiçbir yöne çıkamıyor.";
                    break;
                case SmartVerdict.LosesBomb:
                    report.Verdict = DesignVerdict.Bomb;
                    if (r.Piece >= 0) report.Pieces.Add(r.Piece);
                    report.Message = r.Piece >= 0
                        ? $"Çözülemez: bombanın fitili yetmiyor (fitil {r.Fuse}); bomba zamanında çıkarılamıyor."
                        : "Çözülemez: bomba patlıyor.";
                    break;
                default:
                    report.Verdict = DesignVerdict.Unverified;
                    if (r.Piece >= 0) report.Pieces.Add(r.Piece);
                    report.Message = $"Doğrulanamadı: bomba araması sınıra ulaştı ({r.Nodes:N0} adım). Çözülemez demek değil.";
                    break;
            }
            return report;
        }
    }
}
