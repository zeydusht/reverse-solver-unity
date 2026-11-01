using System;
using System.Collections.Generic;

namespace ReverseSolver.Core
{
    public readonly struct AchievementProgress
    {
        public readonly int Value, Target;
        public bool Done => Value >= Target;
        public AchievementProgress(int value, int target) { Value = Math.Min(value, target); Target = target; }
    }

    public static class Achievements
    {
        public static AchievementProgress[] Compute(IReadOnlyList<LevelData> levels, Func<int, bool> won, PlayStats stats, SaveData legacy = null)
        {
            int wins = 0, bombs = 0, cool = 0, flawless = 0, fast = 0, chains = 0, hard = 0;
            for (int i = 0; i < levels.Count; i++)
            {
                if (!won(i)) continue;
                wins++;
                var l = levels[i]; var s = stats.Of(l.Id);
                if (l.Bombs.Count > 0 && stats.FirstTry(l.Id, true, legacy)) bombs++;
                if (s?.MinLeftOnWin < 3) cool = 1;
                if (s?.MinJamsOnWin == 0) flawless = 1;
                if (s != null && s.HalfLeftWin) fast++;
                foreach (var piece in l.Pieces) if (piece.Length > 1) chains++;
                if (l.Hard) hard++;
            }
            return new[] { new AchievementProgress(wins, 1), new AchievementProgress(wins, 10), new AchievementProgress(wins, 40),
                new AchievementProgress(bombs, 10), new AchievementProgress(cool, 1), new AchievementProgress(flawless, 1),
                new AchievementProgress(fast, 10), new AchievementProgress(chains, 25), new AchievementProgress(hard, 7) };
        }
        public static int Completed(AchievementProgress[] all) { int n = 0; foreach (var a in all) if (a.Done) n++; return n; }
    }
}
