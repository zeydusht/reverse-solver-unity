using NUnit.Framework;

namespace ReverseSolver.Core.Tests
{
    public class PlayStatsTests
    {
        static AttemptRecord Record(string result, int attempt, int seconds = 20, int left = 40, int jams = 0) =>
            new AttemptRecord { LevelId = "P20", Result = result, Attempt = attempt, DurationSeconds = seconds, LeftSeconds = left, Jams = jams };

        [Test]
        public void LossesAndQuitOnlyAddTimeAndUnknownResultsAreIgnored()
        {
            var s = new PlayStats();
            s.Add(Record("bomb", 1), 60); s.Add(Record("time", 2), 60); s.Add(Record("quit", 3), 60);
            s.Add(Record("other", 4), 60);
            Assert.That(s.TotalSeconds, Is.EqualTo(60));
            Assert.That(s.WinCount, Is.EqualTo(0));
            Assert.That(s.Of("P20").BestWinSeconds, Is.Null);
            Assert.That(s.Of("P20").MinJamsOnWin, Is.Null);
            Assert.That(s.Of("P20").FirstTryWin, Is.False);
        }

        [Test]
        public void ReplayedWinsKeepFirstTryAndBestValues()
        {
            var s = new PlayStats();
            s.Add(Record("win", 1, 30, 30, 2), 60);
            s.Add(Record("win", 2, 12, 48, 0), 60);
            s.Add(Record("win", 3, 59, 1, 3), 60);
            var l = s.Of("P20");
            Assert.That(l.Wins, Is.EqualTo(3)); Assert.That(l.FirstTryWin, Is.True);
            Assert.That(l.BestWinSeconds, Is.EqualTo(12)); Assert.That(l.MinJamsOnWin, Is.EqualTo(0));
            Assert.That(l.MinLeftOnWin, Is.EqualTo(1)); Assert.That(l.HalfLeftWin, Is.True);
            Assert.That(s.TotalSeconds, Is.EqualTo(101)); Assert.That(s.SumLeftOnWins, Is.EqualTo(79));
        }

        [Test]
        public void WinningAfterFailureIsNotFirstTryAndHalfTimerUsesExactBoundary()
        {
            var s = new PlayStats(); s.Add(Record("time", 1), 61); s.Add(Record("win", 2, 30, 30), 61);
            Assert.That(s.Of("P20").FirstTryWin, Is.False); Assert.That(s.Of("P20").HalfLeftWin, Is.False);
            s.Add(Record("win", 3, 30, 31), 61);
            Assert.That(s.Of("P20").HalfLeftWin, Is.True);
        }

        [Test]
        public void JsonRoundTripPreservesAllAggregatesAndUnknowns()
        {
            var store = new MemoryStore(); var s = new PlayStats();
            s.Add(Record("win", 1, 18, 42, 1), 60);
            s.Add(new AttemptRecord { LevelId = "quit-only", Result = "quit", Attempt = 1, DurationSeconds = 7 }, 60);
            s.Save(store); var loaded = PlayStats.Load(store);
            Assert.That(store.Get(SaveData.Key("stats")), Is.Not.Null);
            Assert.That(loaded.TotalSeconds, Is.EqualTo(25)); Assert.That(loaded.WinCount, Is.EqualTo(1));
            Assert.That(loaded.SumLeftOnWins, Is.EqualTo(42)); Assert.That(loaded.Of("P20").Wins, Is.EqualTo(1));
            Assert.That(loaded.Of("P20").FirstTryWin, Is.True); Assert.That(loaded.Of("P20").BestWinSeconds, Is.EqualTo(18));
            Assert.That(loaded.Of("P20").MinJamsOnWin, Is.EqualTo(1)); Assert.That(loaded.Of("P20").MinLeftOnWin, Is.EqualTo(42));
            Assert.That(loaded.Of("P20").HalfLeftWin, Is.True); Assert.That(loaded.Of("quit-only").BestWinSeconds, Is.Null);
        }

        [Test]
        public void LegacyFirstTrySurvivesReplayWithoutInventingTimeOrJams()
        {
            var store = new MemoryStore(); var save = new SaveData(store, () => "test");
            save.Best["P20"] = 3; save.Attempts["P20"] = 1;
            var s = new PlayStats(); s.ImportLegacy(save);
            Assert.That(s.FirstTry("P20", true, save), Is.True);
            Assert.That(s.Of("P20").BestWinSeconds, Is.Null); Assert.That(s.Of("P20").MinJamsOnWin, Is.Null);
            save.Attempts["P20"] = 2; s.Add(Record("quit", 2), 60); s.Save(store);
            Assert.That(PlayStats.Load(store).FirstTry("P20", true, save), Is.True);
            Assert.That(s.WinCount, Is.EqualTo(0));
        }

        [Test]
        public void MissingFirstFlagUsesLegacyButExplicitFalseDoesNot()
        {
            var store = new MemoryStore(); var save = new SaveData(store, () => "test");
            save.Best["P20"] = 1; save.Attempts["P20"] = 1;
            store.Set(SaveData.Key("stats"), "{\"levels\":{\"P20\":{\"wins\":1}}}");
            Assert.That(PlayStats.Load(store).FirstTry("P20", true, save), Is.True);
            store.Set(SaveData.Key("stats"), "{\"levels\":{\"P20\":{\"first\":false}}}");
            Assert.That(PlayStats.Load(store).FirstTry("P20", true, save), Is.False);
            store.Set(SaveData.Key("stats"), "broken");
            Assert.That(PlayStats.Load(store).TotalSeconds, Is.EqualTo(0));
        }
    }
}
