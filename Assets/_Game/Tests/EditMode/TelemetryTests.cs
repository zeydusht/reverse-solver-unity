using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ReverseSolver.Core.Json;

namespace ReverseSolver.Core.Tests
{
    /* M5-pre: saving between visits and the send path, with a fake store and
       a fake HTTP. Nothing here talks to Supabase. */
    public class TelemetryTests
    {
        sealed class FakeHttp : ITelemetryTransport
        {
            public readonly List<(string url, IReadOnlyDictionary<string, string> headers, string body)> Calls =
                new List<(string, IReadOnlyDictionary<string, string>, string)>();
            public Queue<int> Statuses = new Queue<int>();
            public void Post(string url, IReadOnlyDictionary<string, string> headers, string body, Action<int> done)
            {
                Calls.Add((url, headers, body));
                done(Statuses.Count > 0 ? Statuses.Dequeue() : 201);
            }
        }

        static int _ids;
        static string NewId() => "id" + (++_ids);

        static (TelemetryLog log, FakeHttp http, MemoryStore store, SaveData save) Setup(bool enabled, MemoryStore store = null)
        {
            store ??= new MemoryStore();
            var save = new SaveData(store, NewId);
            var queue = new TelemetryQueue(store);
            var http = new FakeHttp();
            var sender = new TelemetrySender(queue, http, "https://example.supabase.co", "sb_publishable_test", enabled);
            return (new TelemetryLog(save, queue, sender, NewId), http, store, save);
        }

        static AttemptRecord Finish(AttemptLog log, string level, Action<GameSession> play)
        {
            var s = log.Start(TestData.Levels[level], "unity-web", new Mulberry32(1));
            play(s);
            log.Add(s.Record);
            return s.Record;
        }

        // ---- 1, 2: saved between visits -------------------------------------------

        [Test]
        public void CountersSurviveAReload()
        {
            var store = new MemoryStore();
            var save = new SaveData(store, NewId);
            save.SetPlayer("Zeyd");
            var log = save.Restore();
            Finish(log, "L01", s => s.Quit());
            Finish(log, "L01", s => { foreach (var m in s.Level.Solution) s.TryExit(m.Piece, m.Dir); });
            save.Store(log);

            var again = new SaveData(store, NewId);           // a new visit
            var log2 = again.Restore();
            Assert.That(again.Player, Is.EqualTo("Zeyd"));
            Assert.That(again.Install, Is.EqualTo(save.Install), "install id is stable");
            Assert.That(log2.BestStars("L01"), Is.EqualTo(3));
            var third = log2.Start(TestData.Levels["L01"], "unity-web", new Mulberry32(1));
            Assert.That(third.Attempt, Is.EqualTo(3), "attempt numbers continue from the saved counter");
        }

        [TestCase("{not json")]
        [TestCase("[1,2,3]")]
        [TestCase("{\"L01\":\"x\",\"L02\":-4}")]
        public void CorruptSavesFallBackToDefaults(string junk)
        {
            var store = new MemoryStore();
            store.Set(SaveData.Key("attempts"), junk);
            store.Set(SaveData.Key("best"), junk);
            store.Set(SaveData.Key("queue"), junk);
            Assert.DoesNotThrow(() =>
            {
                var save = new SaveData(store, NewId);
                var log = save.Restore();
                Assert.That(log.Start(TestData.Levels["L01"], "c", new Mulberry32(1)).Attempt, Is.EqualTo(1));
                Assert.That(new TelemetryQueue(store).Rows, Is.Empty);
            });
        }

        // ---- 4: the row is the plays table, column for column ------------------------

        [Test]
        public void RowMatchesThePlaysColumns()
        {
            var r = new AttemptRecord
            {
                LevelId = "L07", LevelVersion = 1, LevelNumber = 7, Client = "unity-web", Attempt = 2, Result = "win",
                DurationSeconds = 41, LeftSeconds = 59, Moves = 30, Jams = 3, Stars = 3, Load = 91,
                Scissors = 1, Wand = 0, Hammer = 0, Clock = 2
            };
            string json = PlayRow.Json(r, "Zeyd \"Z\"", "inst-1", "row-9");
            Assert.That(json, Is.EqualTo(
                "{\"player\":\"Zeyd \\\"Z\\\"\",\"session\":\"inst-1\",\"lv\":7,\"attempt\":2,\"result\":\"win\"," +
                "\"dur\":41,\"left_s\":59,\"moves\":30,\"jams\":3,\"stars\":3,\"load\":91," +
                "\"b_scissors\":1,\"b_wand\":0,\"b_hammer\":0,\"b_clock\":2," +
                "\"client\":\"unity-web\",\"level_version\":1,\"level_id\":\"L07\",\"row_id\":\"row-9\"}"));
            Assert.That(JsonReader.Parse(json).Members.Count, Is.EqualTo(19));
        }

        // ---- 3: sending -------------------------------------------------------------

        [Test]
        public void SuccessEmptiesTheQueueInOneRequestPerBatch()
        {
            var (t, http, _, _) = Setup(enabled: true);
            var log = new AttemptLog();
            var r = Finish(log, "L01", s => s.Quit());
            t.Finished(r, "row-a");
            Assert.That(http.Calls.Count, Is.EqualTo(1));
            Assert.That(t.Queue.Rows, Is.Empty);
            var (url, headers, body) = http.Calls[0];
            Assert.That(url, Is.EqualTo("https://example.supabase.co/rest/v1/plays?on_conflict=row_id"));
            Assert.That(headers["Prefer"], Is.EqualTo("resolution=ignore-duplicates,return=minimal"));
            Assert.That(headers["apikey"], Is.EqualTo("sb_publishable_test"));
            var rows = JsonReader.Parse(body);
            Assert.That(rows.Count, Is.EqualTo(1));
            Assert.That(rows[0].Get("row_id").AsString, Is.EqualTo("row-a"));
            Assert.That(rows[0].Get("client").AsString, Is.EqualTo("unity-web"));
            Assert.That(rows[0].Get("level_id").AsString, Is.EqualTo("L01"));
            Assert.That(rows[0].Get("level_version").Int(), Is.EqualTo(1));
        }

        [TestCase(0)]
        [TestCase(500)]
        [TestCase(503)]
        [TestCase(429)]
        public void TransientFailuresKeepTheRowAndRetryWithTheSameId(int status)
        {
            var (t, http, store, _) = Setup(enabled: true);
            http.Statuses.Enqueue(status);
            t.Finished(Finish(new AttemptLog(), "L01", s => s.Quit()), "row-b");
            Assert.That(t.Queue.Rows.Select(e => e.Id), Is.EqualTo(new[] { "row-b" }));
            Assert.That(new TelemetryQueue(store).Rows.Count, Is.EqualTo(1), "kept in the store across a reload");
            t.Flush();                                         // later: works
            Assert.That(http.Calls.Count, Is.EqualTo(2));
            Assert.That(JsonReader.Parse(http.Calls[1].body)[0].Get("row_id").AsString, Is.EqualTo("row-b"));
            Assert.That(t.Queue.Rows, Is.Empty);
        }

        [TestCase(400)]
        [TestCase(401)]
        [TestCase(404)]
        public void RejectedRowsAreSetAsideSoTheQueueKeepsMoving(int status)
        {
            var (t, http, _, _) = Setup(enabled: true);
            http.Statuses.Enqueue(status);
            t.Finished(Finish(new AttemptLog(), "L01", s => s.Quit()), "row-c");
            Assert.That(t.Queue.Rows, Is.Empty);
            Assert.That(t.Queue.Parked, Is.EqualTo(1));
            t.Finished(Finish(new AttemptLog(), "L02", s => s.Quit()), "row-d");
            Assert.That(http.Calls.Count, Is.EqualTo(2), "the next row is not blocked");
            Assert.That(t.Queue.Rows, Is.Empty);
        }

        [Test]
        public void QueueIsBounded()
        {
            var store = new MemoryStore();
            var q = new TelemetryQueue(store);
            for (int i = 0; i < TelemetryQueue.Max + 7; i++) q.Add("r" + i, "{}");
            Assert.That(q.Rows.Count, Is.EqualTo(TelemetryQueue.Max));
            Assert.That(q.Rows[0].Id, Is.EqualTo("r7"), "oldest rows go first");
            Assert.That(q.Dropped, Is.EqualTo(7));
        }

        [Test]
        public void BatchesAreTwentyFive()
        {
            var (t, http, _, _) = Setup(enabled: true);
            http.Statuses.Enqueue(503);                        // hold the first attempt so rows pile up
            for (int i = 0; i < 30; i++) t.Finished(Finish(new AttemptLog(), "L01", s => s.Quit()), "row" + i);
            // first Finished failed (1 row queued), each later one sent its batch
            Assert.That(http.Calls.All(c => JsonReader.Parse(c.body).Count <= TelemetrySender.Batch));
            Assert.That(t.Queue.Rows, Is.Empty);
        }

        // ---- 5: off means off -----------------------------------------------------------

        [Test]
        public void DisabledSendsNothingAndQueuesNothing()
        {
            var (t, http, store, _) = Setup(enabled: false);
            var log = new AttemptLog();
            var s = log.Start(TestData.Levels["L01"], "unity-web", new Mulberry32(1));
            t.PageHidden(s, "row-e");
            t.PageVisible(s, "row-e");
            s.Quit();
            t.Finished(s.Record, "row-e");
            t.Flush();
            Assert.That(http.Calls, Is.Empty);
            Assert.That(t.Queue.Rows, Is.Empty, "rows made while off are never sent later");
            Assert.That(store.Get(SaveData.Key("queue")), Is.Null);
        }

        [Test]
        public void NoTransportMeansDisabled()
        {
            var store = new MemoryStore();
            var sender = new TelemetrySender(new TelemetryQueue(store), null, "https://x.supabase.co", "k", enabled: true);
            Assert.That(sender.Enabled, Is.False);
        }

        // ---- page closed mid-attempt ------------------------------------------------------

        [Test]
        public void HidingQueuesAQuitThatComingBackTakesAway()
        {
            var (t, http, _, _) = Setup(enabled: true);
            var log = new AttemptLog();
            var s = log.Start(TestData.Levels["L05"], "unity-web", new Mulberry32(1));
            s.Tick(4);
            t.PageHidden(s, "row-f");
            Assert.That(t.Queue.Rows.Count, Is.EqualTo(1));
            Assert.That(http.Calls, Is.Empty, "nothing is sent while hidden");
            Assert.That(JsonReader.Parse(t.Queue.Rows[0].Row).Get("result").AsString, Is.EqualTo("quit"));
            Assert.That(s.IsOver, Is.False, "the attempt itself goes on");

            t.PageVisible(s, "row-f");
            Assert.That(t.Queue.Rows, Is.Empty);
            foreach (var m in SmartPlayer.Play(s.Level).Moves) s.TryExit(m.Piece, m.Dir);
            t.Finished(s.Record, "row-f");
            Assert.That(http.Calls.Count, Is.EqualTo(1));
            Assert.That(JsonReader.Parse(http.Calls[0].body)[0].Get("result").AsString, Is.EqualTo("win"));
        }

        [Test]
        public void AClosedTabLeavesItsQuitRowForTheNextVisit()
        {
            var store = new MemoryStore();
            var (t, http, _, _) = Setup(enabled: true, store);
            var s = new AttemptLog().Start(TestData.Levels["L05"], "unity-web", new Mulberry32(1));
            t.PageHidden(s, "row-g");                       // ...and the page never comes back

            var (next, http2, _, _) = Setup(enabled: true, store);
            next.Flush();
            Assert.That(http2.Calls.Count, Is.EqualTo(1));
            Assert.That(JsonReader.Parse(http2.Calls[0].body)[0].Get("row_id").AsString, Is.EqualTo("row-g"));
        }
    }
}
