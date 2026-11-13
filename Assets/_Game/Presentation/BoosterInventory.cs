using System;
using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    public sealed class BoosterInventory
    {
        public const int BoosterStart = 3;
        public const string Key = "rs_boosters";
        [Serializable] public sealed class Data
        {
            public int[] stock = new int[4];
            public bool[] trained = new bool[4];
            public int[] chestBonus;
        }
        readonly IKeyValueStore _store;
        readonly Data _data;
        public BoosterInventory(IKeyValueStore store, LevelData level, Func<int, bool> won)
        {
            _store = store;
            try { _data = JsonUtility.FromJson<Data>(store.Get(Key) ?? "null"); }
            catch (Exception) { _data = null; }
            bool missing = _data == null || _data.stock == null || _data.stock.Length != 4;
            var trained = _data?.trained;
            if (missing) _data = new Data();
            if (trained != null && trained.Length == 4) _data.trained = trained;
            else _data.trained = new bool[4];
            if (missing)
                foreach (var b in BoosterExt.All)
                    if (Trained(b) || (level.Unlock != null && level.Unlock.TryGetValue(b.Id(), out int at) && won(at - 1)))
                        _data.stock[(int)b] = BoosterStart;
            if (_data.chestBonus != null && _data.chestBonus.Length != 4) _data.chestBonus = null;
            foreach (var b in BoosterExt.All) _data.stock[(int)b] = Math.Max(0, _data.stock[(int)b]);
            Save();
        }
        public bool Trained(Booster b) => _data.trained[(int)b];
        public int Stock(Booster b) => _data.stock[(int)b];
        public void Apply(GameSession session)
        {
            foreach (var b in BoosterExt.All) if (session.IsUnlocked(b)) session.SetStock(b, Stock(b));
        }
        public void Complete(Booster b)
        {
            if (Trained(b)) return;
            _data.trained[(int)b] = true;
            int bonus = Art.T54 && TelemetryMeta.P21Line(Art.GameplayCohort) && _data.chestBonus != null ? Math.Max(0, _data.chestBonus[(int)b]) : 0;
            _data.stock[(int)b] = (int)Math.Min(int.MaxValue, (long)BoosterStart + bonus);
            if (_data.chestBonus != null) _data.chestBonus[(int)b] = 0;
            Save();
        }
        public void Spend(Booster b)
        {
            _data.stock[(int)b] = Math.Max(0, Stock(b) - 1);
            Save();
        }
        void Save() { _store.Set(Key, JsonUtility.ToJson(_data)); _store.Save(); }
    }
}
