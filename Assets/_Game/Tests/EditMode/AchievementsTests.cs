using System.Collections.Generic;
using NUnit.Framework;

namespace ReverseSolver.Core.Tests
{
    public class AchievementsTests
    {
        static List<LevelData> Levels()
        {
            var result = new List<LevelData>();
            var hard = new HashSet<int> { 19, 23, 25, 30, 35, 36, 38 };
            for (int slot = 1; slot <= 40; slot++) result.Add(new LevelData {
                Id = "P" + slot, Number = 100 + slot, Hard = hard.Contains(slot),
                Pieces = new[] { new[] { new Cell(0, 0), new Cell(2, 0) } },
                Bombs = new Dictionary<int, int> { [0] = 3 } });
            return result;
        }

        [Test]
        public void ZeroWinsHaveNoProgress()
        {
            var a = Achievements.Compute(Levels(), i => false, new PlayStats());
            Assert.That(a.Length, Is.EqualTo(9)); Assert.That(Achievements.Completed(a), Is.EqualTo(0));
            foreach (var item in a) Assert.That(item.Value, Is.EqualTo(0));
        }

        [Test]
        public void TwelveWinsCountDistinctLevelsAndDoNotAwardUnknownPerformance()
        {
            var levels = Levels(); var save = new SaveData(new MemoryStore(), () => "test");
            for (int i = 0; i < 12; i++) { save.Best[levels[i].Id] = 3; save.Attempts[levels[i].Id] = 1; }
            var stats = new PlayStats(); stats.ImportLegacy(save);
            var a = Achievements.Compute(levels, i => i < 12, stats, save);
            Assert.That(Achievements.Completed(a), Is.EqualTo(3));
            Assert.That(a[2].Value, Is.EqualTo(12)); Assert.That(a[3].Done, Is.True);
            Assert.That(a[4].Value, Is.EqualTo(0)); Assert.That(a[5].Value, Is.EqualTo(0));
            Assert.That(a[6].Value, Is.EqualTo(0)); Assert.That(a[7].Value, Is.EqualTo(12)); Assert.That(a[8].Value, Is.EqualTo(0));
        }

        [Test]
        public void FortyWinsCompleteAllNineWithoutReplayInflation()
        {
            var levels = Levels(); var stats = new PlayStats();
            foreach (var l in levels)
            {
                stats.Add(new AttemptRecord { LevelId = l.Id, Attempt = 1, Result = "win", LeftSeconds = 30, DurationSeconds = 30 }, 60);
                stats.Add(new AttemptRecord { LevelId = l.Id, Attempt = 2, Result = "win", LeftSeconds = 2, DurationSeconds = 58 }, 60);
            }
            var a = Achievements.Compute(levels, i => true, stats);
            Assert.That(Achievements.Completed(a), Is.EqualTo(9));
            Assert.That(a[2].Value, Is.EqualTo(40)); Assert.That(a[7].Value, Is.EqualTo(25)); Assert.That(a[8].Value, Is.EqualTo(7));
            var partial = Achievements.Compute(levels, i => i != 37, stats);
            Assert.That(partial[8].Value, Is.EqualTo(6)); Assert.That(partial[8].Done, Is.False);
        }

        [Test]
        public void ReplayingOneBombLevelCannotUnlockTenLevelAchievements()
        {
            var levels = Levels(); var stats = new PlayStats();
            for (int n = 1; n <= 12; n++) stats.Add(new AttemptRecord { LevelId = levels[0].Id, Attempt = n, Result = "win", LeftSeconds = 50 }, 60);
            var a = Achievements.Compute(levels, i => i == 0, stats);
            Assert.That(a[3].Value, Is.EqualTo(1)); Assert.That(a[6].Value, Is.EqualTo(1)); Assert.That(a[7].Value, Is.EqualTo(1));
        }
    }
}
