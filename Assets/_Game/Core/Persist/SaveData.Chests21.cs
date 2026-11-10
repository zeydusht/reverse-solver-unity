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
                w.Value((int)Math.Min(int.MaxValue, (long)count + (i == (int)booster ? P21ChestRules.BoosterCount : 0)));
            }
            w.EndArray().Key("trained").BeginArray();
            for (int i = 0; i < 4; i++) w.Value(trained?.Kind == JsonKind.Array && trained.Count == 4 && trained[i].AsBool);
            w.EndArray().Key("chestBonus").BeginArray();
            for (int i = 0; i < 4; i++)
            {
                int count = bonus?.Kind == JsonKind.Array && bonus.Count == 4 && bonus[i].Kind == JsonKind.Number
                    ? (int)Math.Max(0, Math.Min(int.MaxValue, bonus[i].AsNumber)) : 0;
                bool learned = trained?.Kind == JsonKind.Array && trained.Count == 4 && trained[i].AsBool;
                w.Value((int)Math.Min(int.MaxValue, (long)count + (i == (int)booster && !learned ? P21ChestRules.BoosterCount : 0)));
            }
            w.EndArray().EndObject();
            Coins = (int)Math.Min(int.MaxValue, (long)Coins + P21ChestRules.Coins);
            _chests21["given" + level] = (int)booster + 1;
            _store.Set("rs_boosters", w.ToString());
            _store.Set(Key("coins"), Coins.ToString(System.Globalization.CultureInfo.InvariantCulture));
            _store.Set(Key("chests21"), Write(_chests21)); _store.Save();
            return true;
        }
    }
}
