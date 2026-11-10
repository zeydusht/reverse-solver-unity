using System.Linq;
using NUnit.Framework;
using ReverseSolver.Core.Json;

namespace ReverseSolver.Core.Tests
{
    public class P21ChestTests
    {
        static SaveData Load(MemoryStore store) => new SaveData(store, () => "test");
        [Test]
        public void ScheduleAndProgressSegments()
        {
            Assert.That(Enumerable.Range(1, 31).Where(P21ChestRules.Due), Is.EqualTo(new[] { 3, 5, 10, 15, 20, 25, 30 }));
            Assert.That(new[] { 0, 1, 2, 3, 4, 5, 9, 10, 14, 15 }.Select(P21ChestRules.Next), Is.EqualTo(new[] { 3, 3, 3, 5, 5, 10, 10, 15, 15, 20 }));
            Assert.That(P21ChestRules.Next(1) - 1, Is.EqualTo(2));
        }
        [Test]
        public void ProgressAndReceiptsSurviveReloadAndReplays()
        {
            var store = new MemoryStore(); var save = Load(store);
            save.RecordChestWin21(3); save.RecordChestWin21(1);
            save = Load(store);
            Assert.That(save.ChestProgress21, Is.EqualTo(3));
            Assert.That(save.PendingChest21, Is.EqualTo(3));
            Assert.That(save.ClaimChest21(3, Booster.Hammer), Is.True);
            save = Load(store); int coins = save.Coins;
            Assert.That(save.ChestGiven21(3), Is.True);
            Assert.That(save.ClaimChest21(3, Booster.Clock), Is.False);
            Assert.That(save.Coins, Is.EqualTo(coins));
            Assert.That(save.PendingChest21, Is.Zero);
            save.RecordChestWin21(15);
            foreach (int n in new[] { 5, 10, 15 })
            {
                Assert.That(save.PendingChest21, Is.EqualTo(n));
                Assert.That(save.ClaimChest21(n, Booster.Clock), Is.True);
            }
            Assert.That(Load(store).PendingChest21, Is.Zero);
        }
        [TestCase(Booster.Scissors)] [TestCase(Booster.Wand)] [TestCase(Booster.Hammer)] [TestCase(Booster.Clock)]
        public void RewardAddsExactly100AndOneSelectedBooster(Booster booster)
        {
            var store = new MemoryStore(); var save = Load(store);
            store.Set("rs_boosters", "{\"stock\":[2,3,4,5],\"trained\":[true,false,true,false]}");
            save.RecordChestWin21(3); int before = save.Coins, saves = store.Saves;
            Assert.That(save.ClaimChest21(3, booster), Is.True);
            Assert.That(store.Saves, Is.EqualTo(saves + 1));
            Assert.That(Load(store).Coins, Is.EqualTo(before + 100));
            var inventory = JsonReader.Parse(store.Get("rs_boosters"));
            inventory.TryGet("stock", out var stock); inventory.TryGet("trained", out var trained);
            inventory.TryGet("chestBonus", out var bonus);
            for (int i = 0; i < 4; i++)
            {
                Assert.That(stock[i].AsNumber, Is.EqualTo(i + 2 + (i == (int)booster ? 1 : 0)));
                Assert.That(trained[i].AsBool, Is.EqualTo(i % 2 == 0));
                Assert.That(bonus[i].AsNumber, Is.EqualTo(i == (int)booster && i % 2 != 0 ? 1 : 0));
            }
        }
        [Test]
        public void InvalidClaimsHaveNoEffectAndCorruptStateRecovers()
        {
            var store = new MemoryStore(); store.Set(SaveData.Key("chests21"), "broken");
            var save = Load(store); int coins = save.Coins, saves = store.Saves;
            Assert.That(save.ClaimChest21(3, Booster.Hammer), Is.False);
            Assert.That(save.Coins, Is.EqualTo(coins)); Assert.That(store.Saves, Is.EqualTo(saves));
            save.RecordChestWin21(5);
            Assert.That(save.ClaimChest21(4, Booster.Hammer), Is.False);
            Assert.That(save.ClaimChest21(3, (Booster)4), Is.False);
            store.Set("rs_boosters", "broken");
            Assert.That(save.ClaimChest21(3, Booster.Hammer), Is.True);
            Assert.That(Load(store).Coins, Is.EqualTo(coins + 100));
        }
        [Test]
        public void LockedBoosterBonusesAccumulateForLaterTraining()
        {
            var store = new MemoryStore(); var save = Load(store); save.RecordChestWin21(5);
            save.ClaimChest21(3, Booster.Clock); save = Load(store); save.ClaimChest21(5, Booster.Clock);
            var inventory = JsonReader.Parse(store.Get("rs_boosters"));
            inventory.TryGet("stock", out var stock); inventory.TryGet("chestBonus", out var bonus);
            Assert.That(stock[(int)Booster.Clock].AsNumber, Is.EqualTo(2));
            Assert.That(bonus[(int)Booster.Clock].AsNumber, Is.EqualTo(2));
            Assert.That(save.ChestGiven21(3) && save.ChestGiven21(5), Is.True);
        }
        [Test]
        public void OnlyP21ResetClearsChestState()
        {
            var store = new MemoryStore(); var save = Load(store); save.RecordChestWin21(3); save.ClaimChest21(3, Booster.Wand);
            Assert.That(SaveData.ResetForQuery(store, "1", "p20", new LevelData[0]), Is.False);
            Assert.That(Load(store).ChestGiven21(3), Is.True);
            SaveData.ResetForQuery(store, "1", "p21", new LevelData[0]);
            Assert.That(Load(store).ChestProgress21, Is.Zero);
            Assert.That(Load(store).ChestGiven21(3), Is.False);
        }
    }
}
