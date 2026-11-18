using System.IO;
using System.Linq;
using NUnit.Framework;

namespace ReverseSolver.Core.Tests
{
    /* PM_83 §2: the chest economy of sl100 (ChestEconomy, SaveData.ClaimChest83, the live config's numbers). */
    public class ChestEconomyTests
    {
        static LevelSet Set => LevelParser.Parse(File.ReadAllText("Assets/_Game/Levels/sl100.json"));
        static LevelData Lv(int n) => Set.Levels[n - 1];

        [Test]
        public void ChestsEveryFifthLevelAndTheCountsOnVeryHardOnes()
        {
            var c = LiveConfig.Default;
            var set = Set;
            foreach (var l in set.Levels)
            {
                var kind = ChestEconomy.KindOf(l, c);
                if (l.Role == "veryhard") Assert.AreEqual(ChestKind.Count, kind, $"{l.Number}");
                else Assert.AreEqual(l.Number % 5 == 0 ? ChestKind.Normal : ChestKind.None, kind, $"{l.Number}");
            }
            // no chest at 3 any more (the old rule gave a wand there)
            Assert.AreEqual(ChestKind.None, ChestEconomy.KindOf(set.Levels[2], c));
            Assert.AreEqual(29, set.Levels.Count(l => ChestEconomy.KindOf(l, c) != ChestKind.None));
        }

        [Test]
        public void GoldByTierBoostersFromTenTheCountDoubles()
        {
            var c = LiveConfig.Default;
            var stock = new int[4];
            ChestReward Plan(int n) => ChestEconomy.Plan(Lv(n), c, stock, () => .3);
            var five = Plan(5);
            Assert.AreEqual(100, five.Coins); Assert.IsEmpty(five.Boosters, "level 5: gold only");
            Assert.AreEqual(100, Plan(10).Coins); Assert.AreEqual(1, Plan(10).Boosters.Length);
            Assert.AreEqual(100, Plan(20).Coins);
            Assert.AreEqual(150, Plan(25).Coins); Assert.AreEqual(150, Plan(50).Coins);
            Assert.AreEqual(200, Plan(55).Coins); Assert.AreEqual(200, Plan(100).Coins);
            var count19 = Plan(19);
            Assert.AreEqual(ChestKind.Count, count19.Kind);
            Assert.AreEqual(200, count19.Coins); Assert.AreEqual(2, count19.Boosters.Length);
            Assert.AreEqual(300, Plan(29).Coins); Assert.AreEqual(400, Plan(99).Coins);
            // the hard level bonus, no chest
            Assert.AreEqual(100, ChestEconomy.HardBonus(Lv(14), c));
            Assert.AreEqual(ChestKind.None, ChestEconomy.KindOf(Lv(14), c));
            Assert.AreEqual(0, ChestEconomy.HardBonus(Lv(19), c));
            Assert.AreEqual(0, ChestEconomy.HardBonus(Lv(15), c));
        }

        [Test]
        public void OnlyUnlockedBoostersTheLeastHeldMostOften()
        {
            var c = LiveConfig.Default;
            // level 10: only the scissors (6) are open; 19: scissors, hammer (13), clock (18); the wand opens at 25
            CollectionAssert.AreEqual(new[] { Booster.Scissors }, ChestEconomy.Unlocked(Lv(10)));
            CollectionAssert.AreEquivalent(new[] { Booster.Scissors, Booster.Hammer, Booster.Clock }, ChestEconomy.Unlocked(Lv(19)));
            Assert.IsFalse(ChestEconomy.Unlocked(Lv(24)).Contains(Booster.Wand));
            Assert.IsTrue(ChestEconomy.Unlocked(Lv(25)).Contains(Booster.Wand));
            var rnd = new System.Random(1);
            for (int i = 0; i < 200; i++)
                foreach (var b in ChestEconomy.Plan(Lv(19), c, new int[4], rnd.NextDouble).Boosters)
                    Assert.AreNotEqual(Booster.Wand, b, "never a booster not yet unlocked");
            // weighted: with 5 scissors, 5 hammers and no clock held, the clock comes most
            var counts = new int[4];
            for (int i = 0; i < 2000; i++)
                foreach (var b in ChestEconomy.Pick(1, new[] { Booster.Scissors, Booster.Hammer, Booster.Clock }, new[] { 5, 0, 5, 0 }, rnd.NextDouble)) counts[(int)b]++;
            Assert.Greater(counts[(int)Booster.Clock], counts[(int)Booster.Scissors] * 10);
            // two boosters spread: the first one counts as held for the second
            int distinct = 0;
            for (int i = 0; i < 2000; i++) { var two = ChestEconomy.Pick(2, new[] { Booster.Scissors, Booster.Hammer }, new int[4], rnd.NextDouble); if (two[0] != two[1]) distinct++; }
            Assert.Greater(distinct, 1400, "about 80 % two different ones");
        }

        [Test]
        public void TheNumbersComeFromTheLiveConfig()
        {
            var c = LiveConfig.Parse("{\"sandik_her_n\":4,\"sandik_altin\":[10,20],\"sandik_altin_sinir\":[30],\"sandik_booster_bolum\":2,\"sandik_booster\":2,\"kont_altin_kat\":3,\"kont_booster\":1,\"zor_bonus\":7}");
            Assert.AreEqual(4, c.ChestEvery); CollectionAssert.AreEqual(new[] { 10, 20 }, c.ChestCoins);
            Assert.AreEqual(ChestKind.Normal, ChestEconomy.KindOf(Lv(8), c));
            Assert.AreEqual(10, ChestEconomy.Plan(Lv(8), c, new int[4], () => 0).Coins);
            Assert.AreEqual(20, ChestEconomy.Plan(Lv(32), c, new int[4], () => 0).Coins);
            Assert.AreEqual(2, ChestEconomy.Plan(Lv(8), c, new int[4], () => 0).Boosters.Length);
            var k = ChestEconomy.Plan(Lv(19), c, new int[4], () => 0);
            Assert.AreEqual(30, k.Coins); Assert.AreEqual(1, k.Boosters.Length);
            Assert.AreEqual(7, ChestEconomy.HardBonus(Lv(14), c));
            // the round trip, and bad values keep the defaults
            var back = LiveConfig.Parse(c.ToJson());
            Assert.AreEqual(7, back.HardBonus); CollectionAssert.AreEqual(new[] { 30 }, back.ChestTierUpTo);
            var bad = LiveConfig.Parse("{\"sandik_altin\":[\"x\"],\"zor_bonus\":-5}");
            CollectionAssert.AreEqual(new[] { 100, 150, 200 }, bad.ChestCoins); Assert.AreEqual(100, bad.HardBonus);
            // the app's embedded config carries them
            var embedded = LiveConfig.Parse(File.ReadAllText("Assets/Resources/Live/config.json"));
            CollectionAssert.AreEqual(new[] { 100, 150, 200 }, embedded.ChestCoins);
            Assert.AreEqual(100, embedded.HardBonus);
        }

        [Test]
        public void TheClaimGivesGoldAndBoostersOnce()
        {
            var store = new MemoryStore();
            var save = new SaveData(store, () => "p");
            int before = save.Coins;
            var reward = new ChestReward { Kind = ChestKind.Count, Coins = 200, Boosters = new[] { Booster.Hammer, Booster.Clock } };
            Assert.IsTrue(save.ClaimChest83(19, reward));
            Assert.AreEqual(before + 200, save.Coins);
            var stock = save.BoosterStock();
            Assert.AreEqual(1, stock[(int)Booster.Hammer]); Assert.AreEqual(1, stock[(int)Booster.Clock]);
            Assert.IsTrue(save.Chest83Given(19));
            Assert.IsFalse(save.ClaimChest83(19, reward), "once");
            Assert.AreEqual(before + 200, save.Coins);
            // it survives a reload
            Assert.IsTrue(new SaveData(store, () => "p").Chest83Given(19));
            Assert.IsFalse(save.ClaimChest83(20, new ChestReward()), "no chest, nothing");
        }
    }
}
