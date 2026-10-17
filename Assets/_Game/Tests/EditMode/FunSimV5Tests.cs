using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using ReverseSolver.Core;
using ReverseSolver.Editing;
using ReverseSolver.Generation;

namespace ReverseSolver.Core.Tests
{
    /* V5 (PRODUCT.md "V5 çıkış kapısı", 2026-10-17) and Zeyd's fun targets
       ("Keyif eşikleri (Zeyd hedefleri, 2026-10-17)"). */
    public class FunSimV5Tests
    {
        const string GoldenPath = "Assets/_Game/Tests/EditMode/Golden/funsim_v4_golden.json";
        static RedesignStore _golden;
        static RedesignStore Golden => _golden ??= RedesignStore.Load(File.ReadAllText(GoldenPath));
        static List<FunSim.Player> _players;
        static List<FunSim.Player> Players => _players ??= FunSim.Population();

        static RedesignStore.Entry Entry(string id) => Golden.Entries.Single(e => e.Candidate == id);

        /* The Çok zor finalist with two bombs and a nail, with the stock and unlocks given. */
        static LevelData Hard(int number, int stock, int clockAt = 15, int hammerAt = 23)
        {
            var d = Entry("C30-37066ecb").Draft.Clone();
            d.Number = number;
            foreach (var id in LevelDraft.BoosterIds) d.SetBooster(id, stock);
            d.SetUnlock("clock", clockAt);
            d.SetUnlock("hammer", hammerAt);
            return d.ToLevelData();
        }

        static FunSim.Result V5(LevelData l, List<FunSim.Player> players = null) =>
            FunSim.Run(l, Genre.CokZor, players ?? Players, options: new FunSim.Options { Boosters = true });

        [Test]
        public void OffReproducesThePreV5Scores()
        {
            foreach (var e in Golden.Entries)
            {
                var spec = FactoryPlan.Slot(e.Slot);
                var r = FunSim.Run(e.Draft.ToLevelData(), spec.Genre, Players, options: FunSim.OptionsFor(spec));
                string at = $"{e.Candidate}: {r}";
                Assert.That(System.Math.Round(r.EnjoyRate, 3), Is.EqualTo(e.Scores["keyif"]), at);
                Assert.That(System.Math.Round(r.FrustrationRate, 3), Is.EqualTo(e.Scores["hayalKirikligi"]), at);
                Assert.That(System.Math.Round(r.BoredRate, 3), Is.EqualTo(e.Scores["sikici"]), at);
                Assert.That(System.Math.Round(r.FirstWinRate, 3), Is.EqualTo(e.Scores["ilkDeneme"]), at);
                Assert.That(System.Math.Round(r.WinBy3Rate, 3), Is.EqualTo(e.Scores["ucDeneme"]), at);
                Assert.That(System.Math.Round(r.NearMissRate, 3), Is.EqualTo(e.Scores["nearMiss"]), at);
                Assert.That(System.Math.Round(r.EarlyTimeoutRate, 3), Is.EqualTo(e.Scores["erkenSure"]), at);
                Assert.That(r.HammerPlayers + r.ClockPlayers, Is.Zero, at);
            }
        }

        [Test]
        public void NoStockNoUse()
        {
            var r = V5(Hard(30, 0));
            Assert.That(r.HammerPlayers, Is.Zero);
            Assert.That(r.ClockPlayers, Is.Zero);
            Assert.That(r.ToString(), Does.Contain("booster'lı"));
        }

        [Test]
        public void UnlocksFollowTheLevelNumber()
        {
            var before = V5(Hard(14, 1));
            Assert.That(before.ClockPlayers + before.HammerPlayers, Is.Zero, "slot 14: no clock, no hammer");
            var mid = V5(Hard(22, 1));
            Assert.That(mid.ClockPlayers, Is.GreaterThan(0), "slot 22: clock open");
            Assert.That(mid.HammerPlayers, Is.Zero, "slot 22: hammer still locked");
            var late = V5(Hard(23, 1));
            Assert.That(late.HammerPlayers, Is.GreaterThan(0), "slot 23: hammer open");
            TestContext.WriteLine($"22: {mid}\n23: {late}");
        }

        [Test]
        public void StockIsReadFromTheLevelPerAttempt()
        {
            var one = V5(Hard(30, 1));
            Assert.That(one.MostHammers, Is.EqualTo(1));
            Assert.That(one.MostClocks, Is.EqualTo(1));
            var two = V5(Hard(30, 2));
            Assert.That(two.MostHammers, Is.EqualTo(2), "stock 2 in the data: two hammers in one attempt");
            Assert.That(two.MostClocks, Is.LessThanOrEqualTo(2));
            TestContext.WriteLine($"1: {one}\n2: {two}");
        }

        /* a is the bomb attention at that moment; V2 lifts it to 0.8 after a bomb went off,
           so a player who stays at a = 0 is measured with V2 off. */
        [Test]
        public void BlindPlayersNeverHammerABomb()
        {
            var noV2 = new FunSim.Options { Boosters = true, AttentionAfterBomb = 0 };
            var blind = FunSim.Population().Select(p => new FunSim.Player { Speed = p.Speed, Skill = p.Skill, BombAttention = 0, Patience = p.Patience, Retry = p.Retry }).ToList();
            Assert.That(FunSim.Run(Hard(30, 1), Genre.CokZor, blind, options: noV2).BombHammers, Is.Zero, "a = 0");
            var keen = blind.Select(p => new FunSim.Player { Speed = p.Speed, Skill = p.Skill, BombAttention = 1, Patience = p.Patience, Retry = p.Retry }).ToList();
            Assert.That(FunSim.Run(Hard(30, 1), Genre.CokZor, keen, options: noV2).BombHammers, Is.GreaterThan(0), "a = 1");
        }

        [Test]
        public void SameSeedSameResultAndBoostersHelp()
        {
            var a = V5(Hard(30, 1));
            var b = V5(Hard(30, 1));
            Assert.That(b.ToString().Split('·').Where(x => !x.Contains("ms")), Is.EqualTo(a.ToString().Split('·').Where(x => !x.Contains("ms"))));
            var plain = FunSim.Run(Hard(30, 1), Genre.CokZor, Players);
            Assert.That(a.WinBy3Rate, Is.GreaterThanOrEqualTo(plain.WinBy3Rate), $"V5 {a}\nplain {plain}");
        }

        [Test]
        public void HardestLevelStillUnderTwoSeconds()
        {
            // the largest board with the redesign's stock (1 of each, all open at 40); the web L40 has no
            // booster table and would fall back to the web game's 5/3/7/10
            var d = LevelDraft.FromLevel(TestData.Levels.Levels[39]);
            foreach (var id in LevelDraft.BoosterIds) d.SetBooster(id, 1);
            var l40 = d.ToLevelData();
            FunSim.Run(l40, Genre.CokZor, Players, options: new FunSim.Options { Boosters = true });   // warm
            var sw = Stopwatch.StartNew();
            var r = FunSim.Run(l40, Genre.CokZor, Players, options: new FunSim.Options { Boosters = true });
            Assert.That(sw.Elapsed.TotalSeconds, Is.LessThan(2.0), r.ToString());
        }

        // ---- Zeyd's targets ------------------------------------------------------------------------

        static FunSim.Result R(int enjoyed, int frustrated, int firstWin, int early = 0) => new FunSim.Result
        { Players = 100, Enjoyed = enjoyed, Frustrated = frustrated, HardFrustrated = frustrated, WinFirst = firstWin, WinBy3 = enjoyed, EarlyTimeout = early };

        [Test]
        public void ZeydTargetsBoundaries()
        {
            var z = FunThresholds.For(Genre.Zor);
            Assert.That(z.Check(R(80, 20, 50)), Is.Empty, "Zor: keyif 80, hk 20 pass");
            Assert.That(z.Check(R(79, 20, 50)), Has.Some.StartsWith("keyif %79"));
            Assert.That(z.Check(R(80, 21, 50)), Has.Some.StartsWith("hayal kırıklığı %21"));
            var cz = FunThresholds.For(Genre.CokZor);
            Assert.That(cz.Check(R(70, 30, 30)), Is.Empty, "Çok zor: keyif 70, hk 30 pass");
            Assert.That(cz.Check(R(69, 30, 30)), Has.Some.StartsWith("keyif %69"));
            Assert.That(cz.Check(R(70, 31, 30)), Has.Some.StartsWith("hayal kırıklığı %31"));
            var o = FunThresholds.For(Genre.Orta);
            Assert.That(o.Check(R(90, 10, 75)), Is.Empty, "Orta: keyif 90, hk 10 pass");
            Assert.That(o.Check(R(89, 10, 75)), Has.Some.StartsWith("keyif %89"));
            Assert.That(o.Check(R(90, 11, 75)), Has.Some.StartsWith("hayal kırıklığı %11"));
            foreach (var g in new[] { Genre.Tanitim, Genre.Nefes, Genre.Kolay })
            {
                var t = FunThresholds.For(g);
                Assert.That((t.MinEnjoy, t.MaxFrustration, t.MaxUnfair), Is.EqualTo((.90, .05, .05)), g.ToString());
            }
        }

        [Test]
        public void FunFromBoostersFirstTryWithout()
        {
            var z = FunThresholds.For(Genre.Zor);
            var plain = R(60, 40, 50, early: 20);
            var boosted = R(85, 15, 70);
            Assert.That(z.Check(plain, boosted), Is.Empty, "keyif and hk from V5, first try and early time-out without boosters");
            Assert.That(z.Check(R(60, 40, 70), boosted), Has.Some.StartsWith("ilk deneme %70"));
            Assert.That(z.Check(R(60, 40, 50, early: 26), boosted), Has.Some.StartsWith("erken süre bitmesi %26"));
        }

        [Test]
        public void ZorBandRelaxesOnlyAfterThreeRoundsAndShort()
        {
            Assert.That(FunThresholds.RelaxAllowed(Genre.Zor, 1, afterThreeRounds: false), Is.False);
            Assert.That(FunThresholds.RelaxAllowed(Genre.Zor, 1, afterThreeRounds: true), Is.True);
            Assert.That(FunThresholds.RelaxAllowed(Genre.Zor, 2, afterThreeRounds: true), Is.False);
            Assert.That(FunThresholds.RelaxAllowed(Genre.Kolay, 0, afterThreeRounds: true), Is.False);
            var z = FunThresholds.For(Genre.Zor);
            Assert.That(z.Check(R(85, 10, 68)), Has.Some.StartsWith("ilk deneme %68"));
            Assert.That(z.Relaxed().Check(R(85, 10, 68)), Is.Empty, "Zor's band top 70 when relaxed");
            Assert.That(z.Relaxed().Check(R(85, 10, 71)), Has.Some.StartsWith("ilk deneme %71"));
            Assert.That(FunThresholds.For(Genre.CokZor).Relaxed().FirstWin.Max, Is.EqualTo(.45));
            Assert.That(z.FirstWin.Max, Is.EqualTo(.65), "the relaxed copy leaves the original alone");
        }

        [Test]
        public void SetUnderNinetyRanksFunFirstOnZor()
        {
            var t = FunThresholds.For(Genre.Zor);
            Finalist F(int enjoy, int first) => new Finalist { Thresholds = t, Fun = R(enjoy, 100 - enjoy, first), Boosted = R(enjoy, 100 - enjoy, first) };
            var centred = F(81, 50);
            var funnier = F(95, 60);
            Assert.That(centred.RankKey.CompareTo(funnier.RankKey), Is.LessThan(0), "default: band middle first");
            centred.FunFirst = funnier.FunFirst = true;
            Assert.That(funnier.RankKey.CompareTo(centred.RankKey), Is.LessThan(0), "set under 90 %: fun first");
        }

        [Test]
        public void RedesignStockIsOneOfEach()
        {
            foreach (var id in LevelDraft.BoosterIds) Assert.That(Catalog.Stock[id], Is.EqualTo(1), id);
            foreach (var kv in Catalog.RedesignTexts)
                Assert.That(kv.Value.body + " " + kv.Value.tip, Does.Not.Match(@"\b(5|3|7|10) (makas|değnek|çekiç|saat|tane|hak)"), "a redesign card names no stock: " + kv.Key);
        }
    }
}
