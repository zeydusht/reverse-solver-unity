using System;
using System.Collections.Generic;
using ReverseSolver.Core.Json;

namespace ReverseSolver.Core
{
    public static class P21ChestRules
    {
        public const int Coins = 100, BoosterCount = 1;
        public static bool Due(int level) => level == 3 || level == 5 || level >= 10 && level % 5 == 0;
        public static int Next(int level) => level < 3 ? 3 : level < 5 ? 5 : (level / 5 + 1) * 5;
        public static int Previous(int level) => level < 3 ? 0 : level < 5 ? 3 : level < 10 ? 5 : level / 5 * 5;
    }

    public sealed partial class SaveData
    {
        readonly Dictionary<string, int> _chests21 = new Dictionary<string, int>();
        public int ChestProgress21 => _chests21.TryGetValue("progress", out int n) ? n : 0;
        public bool ChestGiven21(int level) => _chests21.ContainsKey("given" + level);
        public int PendingChest21
        {
            get
            {
                for (int n = 3; n <= ChestProgress21; n = P21ChestRules.Next(n))
                    if (!ChestGiven21(n)) return n;
                return 0;
            }
        }
        public void RecordChestWin21(int level)
        {
            if (level <= ChestProgress21) return;
            _chests21["progress"] = level;
            _store.Set(Key("chests21"), Write(_chests21)); _store.Save();
        }

        // All balances and the receipt are staged before a single persistent flush.
        // Uses the existing inventory format, retaining training and locked booster stock.
        public bool ClaimChest21(int level, Booster booster)
        {
            if (!P21ChestRules.Due(level) || level > ChestProgress21 || ChestGiven21(level) || (int)booster < 0 || (int)booster >= 4) return false;
            Grant21(new[] { booster }, P21ChestRules.Coins);
            _chests21["given" + level] = (int)booster + 1;
            _store.Set(Key("chests21"), Write(_chests21)); _store.Save();
            return true;
        }

        /* PM_83: the sl100 chest (ChestEconomy) of a won level, once: its gold and boosters in one save. */
        public bool Chest83Given(int level) => _chests21.ContainsKey("c83_" + level);
        public bool ClaimChest83(int level, ChestReward reward)
        {
            if (reward == null || reward.Kind == ChestKind.None || Chest83Given(level)) return false;
            foreach (var b in reward.Boosters) if ((int)b < 0 || (int)b >= 4) return false;
            Grant21(reward.Boosters, reward.Coins);
            _chests21["c83_" + level] = 1;
            _store.Set(Key("chests21"), Write(_chests21)); _store.Save();
            return true;
        }

        /* The current booster stock (rs_boosters), 0 where unknown. */
        public int[] BoosterStock()
        {
            var r = new int[4];
            JsonNode inventory = JsonNode.Null;
            try { inventory = JsonReader.Parse(_store.Get("rs_boosters") ?? "null"); }
            catch (JsonException) { }
            if (inventory.TryGet("stock", out var stock) && stock.Kind == JsonKind.Array && stock.Count == 4)
                for (int i = 0; i < 4; i++) r[i] = stock[i].Kind == JsonKind.Number ? (int)Math.Max(0, Math.Min(int.MaxValue, stock[i].AsNumber)) : 0;
            return r;
        }

        // Balances staged (stock, or the chest bonus kept for a booster not yet learned, and the gold); the caller saves.
        void Grant21(Booster[] boosters, int coins)
        {
            var add = new int[4];
            foreach (var b in boosters) add[(int)b]++;
            JsonNode inventory = JsonNode.Null;
            try { inventory = JsonReader.Parse(_store.Get("rs_boosters") ?? "null"); }
            catch (JsonException) { }
            inventory.TryGet("stock", out var stock); inventory.TryGet("trained", out var trained);
            inventory.TryGet("chestBonus", out var bonus);
            var w = new JsonWriter().BeginObject().Key("stock").BeginArray();
            for (int i = 0; i < 4; i++)
            {
                int count = stock?.Kind == JsonKind.Array && stock.Count == 4 && stock[i].Kind == JsonKind.Number
                    ? (int)Math.Max(0, Math.Min(int.MaxValue, stock[i].AsNumber)) : 0;
                w.Value((int)Math.Min(int.MaxValue, (long)count + add[i]));
            }
            w.EndArray().Key("trained").BeginArray();
            for (int i = 0; i < 4; i++) w.Value(trained?.Kind == JsonKind.Array && trained.Count == 4 && trained[i].AsBool);
            w.EndArray().Key("chestBonus").BeginArray();
            for (int i = 0; i < 4; i++)
            {
                int count = bonus?.Kind == JsonKind.Array && bonus.Count == 4 && bonus[i].Kind == JsonKind.Number
                    ? (int)Math.Max(0, Math.Min(int.MaxValue, bonus[i].AsNumber)) : 0;
                bool learned = trained?.Kind == JsonKind.Array && trained.Count == 4 && trained[i].AsBool;
                w.Value((int)Math.Min(int.MaxValue, (long)count + (learned ? 0 : add[i])));
            }
            w.EndArray().EndObject();
            Coins = (int)Math.Min(int.MaxValue, (long)Coins + coins);
            _store.Set("rs_boosters", w.ToString());
            if (!TestMode) _store.Set(Key("coins"), Coins.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
    }
}
