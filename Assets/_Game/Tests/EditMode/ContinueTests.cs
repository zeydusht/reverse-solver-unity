using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ReverseSolver.Core.Json;

namespace ReverseSolver.Core.Tests
{
    public class ContinueTests
    {
        static GameSession Session(int timer = 60, bool bombs = false)
        {
            var level = LevelParser.Parse("{\"format\":1,\"levels\":[{\"id\":\"C\",\"version\":1,\"lv\":50,\"w\":3,\"h\":1,\"timer\":" + timer +
                ",\"pieces\":[[[0,0]],[[1,0]],[[2,0]]],\"vEdge\":[[null,0,0,null]],\"hEdge\":[[null,null,null],[null,null,null]]" +
                (bombs ? ",\"bombs\":{\"1\":1,\"2\":1}" : "") + "}]}").Levels[0];
            return TestData.WebClock(new GameSession(level, 1, "test", new Mulberry32(1)));
        }

        [Test]
        public void LadderStopsAfterThreeAndNewAttemptStartsAtNineHundred()
        {
            var rules = new ContinueRules();
            var s = Session();
            foreach (int price in new[] { 900, 1900, 2900 })
            {
                Assert.That(rules.Offer(ContinueKind.Time, s.Continues, 60, 5000).Price, Is.EqualTo(price));
                s.Tick(s.TimeLeft);
                Assert.That(s.Continue(30, 5), Is.True);
            }
            Assert.That(rules.Offer(ContinueKind.Time, s.Continues, 60, 5000), Is.Null);
            Assert.That(rules.Offer(ContinueKind.Bomb, 3, 60, 5000), Is.Null);
            Assert.That(Session().Continues, Is.Zero);
            Assert.That(rules.Offer(ContinueKind.Time, Session().Continues, 60, 5000).Price, Is.EqualTo(900));
        }

        [TestCase(59, 20)]
        [TestCase(60, 30)]
        public void SecondsUseOriginalTimerBoundary(int timer, int seconds)
        {
            var rules = new ContinueRules();
            Assert.That(rules.Offer(ContinueKind.Time, 0, timer, 900).Seconds, Is.EqualTo(seconds));
            Assert.That(rules.Offer(ContinueKind.Bomb, 0, timer, 900).Seconds, Is.Zero);
            Assert.That(rules.Offer(ContinueKind.Time, 0, timer, 899).Affordable, Is.False);
            Assert.That(rules.Offer(ContinueKind.Time, 0, timer, 900).Affordable, Is.True);
        }

        [Test]
        public void ParametersCanBeChangedWithoutPresentation()
        {
            var prices = new[] { 1, 2, 3 };
            var rules = new ContinueRules(prices, 40, 15, 70, 7, 60, 2000);
            prices[0] = 999;
            Assert.That(rules.Offer(ContinueKind.Time, 0, 69, 1).Price, Is.EqualTo(1));
            Assert.That(rules.Offer(ContinueKind.Time, 0, 69, 1).Seconds, Is.EqualTo(15));
            Assert.That(rules.Offer(ContinueKind.Time, 0, 70, 1).Seconds, Is.EqualTo(40));
            Assert.That(rules.BombFuse, Is.EqualTo(7));
            Assert.That(rules.WinCoins, Is.EqualTo(60));
            Assert.That(rules.StartCoins, Is.EqualTo(2000));
        }

        [Test]
        public void TimeContinueClearsRecordAndEmitsOnlyContinued()
        {
            var s = Session();
            s.Tick(60);
            var loss = s.Record;
            int count = s.Events.Count;
            Assert.That(s.Continue(30, 5), Is.True);
            Assert.That(s.TimeLeft, Is.EqualTo(30));
            Assert.That(s.Outcome, Is.EqualTo(Outcome.None));
            Assert.That(s.Record, Is.Null);
            Assert.That(s.Events.Skip(count).Select(e => e.Kind), Is.EqualTo(new[] { EventKind.Continued }));
            s.Tick(30);
            Assert.That(s.Record, Is.Not.SameAs(loss));
            Assert.That(s.Record.Continues, Is.EqualTo(1));
            Assert.That(s.Record.Result, Is.EqualTo("time"));
            Assert.That(loss.Continues, Is.Zero, "the deferred record was never mutated");
        }

        [Test]
        public void BombContinueRevivesBothExpiredPresentBombsWithoutAddingTime()
        {
            var s = Session(bombs: true);
            s.TryExit(0, Dir.U);
            Assert.That(s.Outcome, Is.EqualTo(Outcome.Bomb));
            Assert.That(s.FuseAt(1), Is.Zero);
            Assert.That(s.FuseAt(2), Is.Zero);
            int time = s.TimeLeft;
            Assert.That(s.Continue(30, 5), Is.True);
            Assert.That(s.FuseAt(1), Is.EqualTo(5));
            Assert.That(s.FuseAt(2), Is.EqualTo(5));
            Assert.That(s.TimeLeft, Is.EqualTo(time));
            Assert.That(s.Record, Is.Null);
            Assert.That(s.Continues, Is.EqualTo(1));
        }

        [Test]
        public void BombReviveLeavesHealthyFusesAndRemovedBombsAlone()
        {
            var s = Session(bombs: true);
            s.Level.Bombs = new Dictionary<int, int> { [0] = 1, [1] = 1, [2] = 4 };
            s = TestData.WebClock(new GameSession(s.Level, 1, "test", new Mulberry32(1)));
            s.TryExit(0, Dir.U);
            Assert.That(s.Continue(0, 5), Is.True);
            Assert.That(s.Bombs.ContainsKey(0), Is.False);
            Assert.That(s.FuseAt(1), Is.EqualTo(5));
            Assert.That(s.FuseAt(2), Is.EqualTo(3));
        }

        [Test]
        public void PlayingWinAndQuitRefuseContinueWithoutChangingAnything()
        {
            var s = Session();
            Assert.That(s.Continue(30, 5), Is.False);
            Assert.That(s.Events, Is.Empty);
            for (int i = 0; i < 3; i++) s.TryExit(i, Dir.U);
            Assert.That(s.Outcome, Is.EqualTo(Outcome.Win));
            var record = s.Record;
            int count = s.Events.Count;
            Assert.That(s.Continue(30, 5), Is.False);
            Assert.That(s.Record, Is.SameAs(record));
            Assert.That(s.Events.Count, Is.EqualTo(count));
            Assert.That(s.Continues, Is.Zero);
            var quit = Session(); quit.Quit();
            Assert.That(quit.Continue(30, 5), Is.False);
        }

        [Test]
        public void AddedSecondsNeverInflateStarsAndContinuedWinStillProgresses()
        {
            var s = Session();
            s.Tick(60); s.Continue(30, 5);
            s.Tick(30); s.Continue(30, 5);
            for (int i = 0; i < 3; i++) s.TryExit(i, Dir.U);
            Assert.That(s.Stars, Is.EqualTo(1));
            Assert.That(s.Record.Continues, Is.EqualTo(2));
            Assert.That(GameSession.StarsFor(s.TimeLeft, s.Level.TimerSeconds), Is.EqualTo(3));
            var log = new AttemptLog(); log.Add(s.Record);
            Assert.That(log.BestStars(s.Level.Id), Is.EqualTo(1));
            var stats = new PlayStats(); stats.Add(s.Record, 60);
            Assert.That(stats.WinCount, Is.EqualTo(1));
            Assert.That(stats.FirstTry(s.Level.Id, true), Is.False);
            var store = new MemoryStore(); stats.Save(store);
            Assert.That(PlayStats.Load(store).FirstTry(s.Level.Id, true), Is.False);
        }

        [Test]
        public void PlaysUsesExistingResultAndIdenticalColumnSet()
        {
            var r = new AttemptRecord { LevelId = "C", Result = "win", Attempt = 1 };
            string before = PlayRow.Json(r, "player", "session", "row");
            r.Continues = 1;
            var continued = JsonReader.Parse(PlayRow.Json(r, "player", "session", "row"));
            Assert.That(continued.Get("result").AsString, Is.EqualTo("won_after_continue"));
            Assert.That(continued.Members.Select(m => m.Key), Is.EqualTo(JsonReader.Parse(before).Members.Select(m => m.Key)));
            Assert.That(continued.Members.Any(m => m.Key.ToLowerInvariant().Contains("continue")), Is.False);
            r.Continues = 0;
            Assert.That(PlayRow.Json(r, "player", "session", "row"), Is.EqualTo(before));
        }

        [TestCase("bomb")]
        [TestCase("time")]
        [TestCase("quit")]
        public void LossAfterContinueKeepsExistingResult(string result)
        {
            var s = Session(bombs: true);
            s.TryExit(0, Dir.U); s.Continue(0, 1);
            if (result == "bomb") s.TryExit(1, Dir.U);
            else if (result == "time") s.Tick(60);
            else s.Quit();
            Assert.That(s.Record.Result, Is.EqualTo(result));
            Assert.That(JsonReader.Parse(PlayRow.Json(s.Record, "p", "s", "r")).Get("result").AsString, Is.EqualTo(result));
        }

        [Test]
        public void LowBalanceIsToppedUpOnce()
        {
            var store = new MemoryStore();
            store.Set(SaveData.Key("coins"), "100");
            var save = new SaveData(store, () => "device");
            Assert.That(save.Coins, Is.EqualTo(5000));
            save.SetCoins(200);
            Assert.That(new SaveData(store, () => "unused").Coins, Is.EqualTo(200));
        }

        [Test]
        public void CoinDefaultsAndBalanceSurviveReload()
        {
            var store = new MemoryStore();
            var save = new SaveData(store, () => "device");
            Assert.That(save.Coins, Is.EqualTo(5000));
            save.SetCoins(5000);
            Assert.That(new SaveData(store, () => "unused").Coins, Is.EqualTo(5000));
            save.SetCoins(save.Coins - 900 + new ContinueRules().WinCoins);
            Assert.That(new SaveData(store, () => "unused").Coins, Is.EqualTo(4150));
        }

        [Test]
        public void ContinueRowsPersistSeparatelyAndParkOnMissingTable()
        {
            var store = new MemoryStore();
            var queue = new TelemetryQueue(store, "continue");
            var r = new AttemptRecord { LevelId = "C", LevelVersion = 2, Attempt = 1 };
            string row = ContinueRow.Json("decision", "play-row", "p", r, 1, "time", 900, 30, 1000, "accepted");
            queue.Add("decision", row);
            Assert.That(new TelemetryQueue(store).Rows, Is.Empty);
            var restored = new TelemetryQueue(store, "continue");
            Assert.That(restored.Rows.Single().Row, Is.EqualTo(row));
            Assert.That(JsonReader.Parse(row).Get("attempt_row_id").AsString, Is.EqualTo("play-row"));
            var transport = new MissingTable();
            var sender = new TelemetrySender(restored, transport, "https://example.invalid", "test", true, "continue_events?on_conflict=row_id");
            sender.Flush();
            Assert.That(restored.Rows, Is.Empty);
            Assert.That(restored.Parked, Is.EqualTo(1));
            Assert.That(new TelemetryQueue(store, "continue").Parked, Is.EqualTo(1));
        }

        sealed class MissingTable : ITelemetryTransport
        {
            public void Post(string url, IReadOnlyDictionary<string, string> headers, string body, System.Action<int> done) => done(404);
        }
    }
}
