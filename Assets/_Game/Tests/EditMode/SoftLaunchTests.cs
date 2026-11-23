using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace ReverseSolver.Core.Tests
{
    /* PM_81: the soft launch's rules: ad frequency, no dead reward button, the offline start, the event queue. */
    public class SoftLaunchTests
    {
        // ---- ads ----

        static double Min(double m) => m * 60;

        [Test]
        public void InterstitialFollowsTheConfig()
        {
            var c = LiveConfig.Default;
            var s = new AdState { WinsSinceInterstitial = 3 };
            // a returning player (not the first session), level 12 won, never shown: due
            Assert.IsTrue(AdRules.InterstitialDue(c, s, 12, false, Min(100), false, 0));
            // before level 12, after a loss, too few wins: not due
            Assert.IsFalse(AdRules.InterstitialDue(c, s, 11, false, Min(100), false, 0));
            Assert.IsFalse(AdRules.InterstitialDue(c, s, 15, true, Min(100), false, 0));
            Assert.IsFalse(AdRules.InterstitialDue(c, new AdState { WinsSinceInterstitial = 2 }, 15, false, Min(100), false, 0));
            // the first 10 minutes of the first session: never
            Assert.IsFalse(AdRules.InterstitialDue(c, s, 15, false, Min(9.9), true, 0));
            Assert.IsTrue(AdRules.InterstitialDue(c, s, 15, false, Min(10.1), true, 0));
            // shown: the counter restarts, and 120 s must pass as well as 3 wins
            AdRules.InterstitialShown(s, 1000);
            for (int i = 0; i < 3; i++) AdRules.Won(s);
            Assert.IsFalse(AdRules.InterstitialDue(c, s, 18, false, 1100, false, 0));
            Assert.IsTrue(AdRules.InterstitialDue(c, s, 18, false, 1120, false, 0));
            // switched off remotely
            var off = LiveConfig.Parse("{\"reklam_acik\": false}");
            Assert.IsFalse(AdRules.InterstitialDue(off, s, 18, false, 5000, false, 0));
            var later = LiveConfig.Parse("{\"gecis_baslangic_bolum\": 20, \"gecis_her_n_bolum\": 5, \"gecis_min_sn\": 300}");
            Assert.AreEqual(20, later.InterstitialFromLevel);
            Assert.IsFalse(AdRules.InterstitialDue(later, s, 18, false, 5000, false, 0));
        }

        [Test]
        public void RewardButtonHiddenWhenNoAdIsReady()
        {
            var c = LiveConfig.Default; var s = new AdState();
            foreach (var p in new[] { AdPlacement.R1Continue, AdPlacement.R2DoubleGold, AdPlacement.R3Booster, AdPlacement.R4Gift })
            {
                Assert.IsFalse(AdRules.ShowRewardButton(c, s, p, "2026-11-18", false), p + ": not loaded");
                Assert.IsTrue(AdRules.ShowRewardButton(c, s, p, "2026-11-18", true), p + ": loaded");
            }
            Assert.IsFalse(AdRules.ShowRewardButton(LiveConfig.Parse("{\"r4_acik\": false}"), s, AdPlacement.R4Gift, "2026-11-18", true));
        }

        [Test]
        public void DailyCapsResetEachDay()
        {
            var c = LiveConfig.Default; var s = new AdState();
            for (int i = 0; i < 3; i++) AdRules.Rewarded(s, AdPlacement.R4Gift, "2026-11-18");
            Assert.IsFalse(AdRules.ShowRewardButton(c, s, AdPlacement.R4Gift, "2026-11-18", true));
            Assert.IsTrue(AdRules.ShowRewardButton(c, s, AdPlacement.R3Booster, "2026-11-18", true));
            Assert.IsTrue(AdRules.ShowRewardButton(c, s, AdPlacement.R4Gift, "2026-11-19", true));
            var again = AdState.Parse(s.ToJson());
            Assert.AreEqual("2026-11-19", again.Day);
        }

        [Test]
        public void ConfigKeepsDefaultsOnBadInput()
        {
            Assert.AreEqual(5000, LiveConfig.Parse("not json").StartCoins);
            var c = LiveConfig.Parse("{\"devam_fiyatlari\": [500, 1000], \"baslangic_altin\": 3000, \"ek_sure_sn\": -4}");
            CollectionAssert.AreEqual(new[] { 500, 1000 }, c.ContinuePrices);
            Assert.AreEqual(3000, c.StartCoins);
            Assert.AreEqual(30, c.ExtraSeconds);
            var round = LiveConfig.Parse(c.ToJson());
            CollectionAssert.AreEqual(c.ContinuePrices, round.ContinuePrices);
        }

        // ---- the offline start (live/) ----

        sealed class Files : IContentFiles
        {
            public readonly Dictionary<string, byte[]> Embedded = new Dictionary<string, byte[]>(), Cache = new Dictionary<string, byte[]>();
            public byte[] ReadEmbedded(string path) => Embedded.TryGetValue(path, out var b) ? b : null;
            public byte[] ReadCache(string path) => Cache.TryGetValue(path, out var b) ? b : null;
            public void WriteCache(string path, byte[] data) => Cache[path] = data;
        }

        static byte[] B(string s) => Encoding.UTF8.GetBytes(s);

        static ContentManifest Manifest(string version, Dictionary<string, byte[]> files)
        {
            var m = new ContentManifest { Version = version };
            foreach (var kv in files) m.Files.Add(new ContentManifest.Entry { Path = kv.Key, Hash = ContentManifest.Sha256(kv.Value), Size = kv.Value.Length });
            return m;
        }

        [Test]
        public void OfflineFirstThenOnlineThenOfflineAgain()
        {
            var files = new Files();
            var embedded = new Dictionary<string, byte[]> { ["levels.json"] = B("{\"v\":1}"), ["config.json"] = B("{}") };
            foreach (var kv in embedded) files.Embedded[kv.Key] = kv.Value;
            files.Embedded["manifest.json"] = B(Manifest("v1", embedded).ToJson());
            var loader = new ContentLoader(files);

            // 1. airplane mode, first start: the app's own copy
            var first = loader.LoadLocal();
            Assert.AreEqual(ContentOrigin.Embedded, first.Origin);
            Assert.AreEqual("{\"v\":1}", first.LevelsJson);
            Assert.AreEqual("v1", first.Version);

            // 2. online: a new version; only the changed file is fetched, then committed
            var remoteFiles = new Dictionary<string, byte[]> { ["levels.json"] = B("{\"v\":2}"), ["config.json"] = B("{}") };
            files.Cache["config.json"] = B("{}");          // already the same on the device
            var remote = Manifest("v2", remoteFiles);
            var fetch = loader.ToFetch(remote);
            Assert.AreEqual(1, fetch.Count);
            Assert.AreEqual("levels.json", fetch[0].Path);
            Assert.IsFalse(loader.Commit(remote, new Dictionary<string, byte[]> { ["levels.json"] = B("{\"v\":3}") }), "a file that does not match its hash is refused");
            Assert.IsNull(files.ReadCache("manifest.json"));
            Assert.IsTrue(loader.Commit(remote, new Dictionary<string, byte[]> { ["levels.json"] = B("{\"v\":2}") }));

            // 3. airplane mode again: the device's copy
            var third = loader.LoadLocal();
            Assert.AreEqual(ContentOrigin.Cache, third.Origin);
            Assert.AreEqual("{\"v\":2}", third.LevelsJson);
            Assert.AreEqual("v2", third.Version);

            // a damaged copy on the device: back to the app's own
            files.Cache["levels.json"] = B("{\"v\":9}");
            Assert.AreEqual(ContentOrigin.Embedded, loader.LoadLocal().Origin);
        }

        [Test]
        public void MissingPaintingFallsBackToOneOfTheFirstForty()
        {
            var m = new ContentManifest { Version = "v" };
            for (int i = 1; i <= 60; i++) m.Files.Add(new ContentManifest.Entry { Path = $"paintings/painting-{i:00}.jpg", Hash = "x" });
            Assert.AreEqual("painting-55", ContentLoader.FallbackImage("painting-55", m));
            Assert.AreEqual("painting-21", ContentLoader.FallbackImage("painting-61", m));
            Assert.AreEqual("painting-20", ContentLoader.FallbackImage("painting-100", m));
        }

        // ---- the event queue ----

        [Test]
        public void EventsWaitOfflineAndSurviveARestart()
        {
            var store = new MemoryStore();
            var q = new TelemetryQueue(store, "sl");
            q.Add("e1", SlEventRow.Json("e1", "first_open", "p", "s"));
            q.Add("e2", SlEventRow.Json("e2", "ad_shown", "p", "s", "r1_sure", "AdMob", 0.0123));
            q.Add("e1", SlEventRow.Json("e1", "first_open", "p", "s"));      // the same row once only
            var offline = new TelemetrySender(q, new NoNet(), "https://x", "k", true, "sl_events?on_conflict=row_id");
            offline.Flush();
            Assert.AreEqual(2, q.Rows.Count, "no network: the rows wait");
            var again = new TelemetryQueue(store, "sl");                   // the app was closed and opened again
            Assert.AreEqual(2, again.Rows.Count);
            StringAssert.Contains("\"revenue\":0.0123", again.Rows[1].Row);
            var online = new TelemetrySender(again, new Ok(), "https://x", "k", true, "sl_events?on_conflict=row_id");
            online.Flush();
            Assert.AreEqual(0, again.Rows.Count);
        }

        [Test]
        public void CrashTextKeepsFiveStackLines()
        {
            string text = SlEventRow.Crash("NullReferenceException: x", "a\nb\n\nc\nd\ne\nf\ng");
            Assert.AreEqual("NullReferenceException: x\na\nb\nc\nd\ne", text);
        }

        sealed class NoNet : ITelemetryTransport
        {
            public void Post(string url, IReadOnlyDictionary<string, string> headers, string body, System.Action<int> done) => done(0);
        }

        sealed class Ok : ITelemetryTransport
        {
            public void Post(string url, IReadOnlyDictionary<string, string> headers, string body, System.Action<int> done) => done(201);
        }
    }
}
