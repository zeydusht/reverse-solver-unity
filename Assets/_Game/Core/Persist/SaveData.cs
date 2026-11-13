using System;
using System.Collections.Generic;
using ReverseSolver.Core.Json;

namespace ReverseSolver.Core
{
    /* Where saved strings live: PlayerPrefs in the game (IndexedDB on the web),
       a dictionary in tests. */
    public interface IKeyValueStore
    {
        string Get(string key);          // null when missing
        void Set(string key, string value);
        void Delete(string key);
        void Save();
    }

    public sealed class MemoryStore : IKeyValueStore
    {
        public readonly Dictionary<string, string> Data = new Dictionary<string, string>();
        public int Saves { get; private set; }
        public string Get(string key) => Data.TryGetValue(key, out var v) ? v : null;
        public void Set(string key, string value) => Data[key] = value;
        public void Delete(string key) => Data.Remove(key);
        public void Save() => Saves++;
    }

    /* Everything the game keeps between visits, under versioned keys. A key
       that is missing or does not parse falls back to its default; a bad save
       never stops the game. Bump Version if the layout changes; old keys are
       then ignored rather than misread.

         rs.v1.player    player name (web: up_player)
         rs.v1.install   random id of this browser (web: up_sid)
         rs.v1.attempts  {"L01": 3, ...}  starts per level id
         rs.v1.best      {"L01": 2, ...}  best stars per level id
         rs.v1.queue     telemetry queue (TelemetryQueue) */
    public sealed partial class SaveData
    {
        public const int Version = 1;
        public static readonly string Prefix = $"rs.v{Version}.";
        public static string Key(string name) => Prefix + name;

        public static bool ResetForQuery(IKeyValueStore store, string reset, string cohort, IEnumerable<LevelData> levels)
        {
            if (reset != "1" || !TelemetryMeta.P21Line(cohort)) return false;
            foreach (string name in new[] { "attempts", "best", "losses", "fun_asked", "stats", "chests21" }) store.Delete(Key(name));
            foreach (string key in new[] { "rs_intro_shown", "rs_intro_end_shown", "rs_boosters", "rs_profile_p21", "rs_p21_tutorial_seen", "rs_ach_seen" }) store.Delete(key);
            foreach (var level in levels) store.Delete("rs_hard_p21_" + level.Id);
            store.Set(Key("coins"), new ContinueRules().StartCoins.ToString(System.Globalization.CultureInfo.InvariantCulture));
            store.Save();
            return true;
        }

        readonly IKeyValueStore _store;

        int _coins;
        public bool TestMode { get; set; }
        public int Coins { get => TestMode ? 99999 : _coins; private set => _coins = value; }
        public string Player { get; private set; }
        public string Install { get; private set; }
        public Dictionary<string, int> Attempts { get; } = new Dictionary<string, int>();
        public Dictionary<string, int> Best { get; } = new Dictionary<string, int>();
        /* Faz 4 G8 (👍/👎, SetFlags.AskFun): lost attempts per level and whether the question was asked. */
        public Dictionary<string, int> Losses { get; } = new Dictionary<string, int>();
        public Dictionary<string, int> FunAsked { get; } = new Dictionary<string, int>();

        public SaveData(IKeyValueStore store, Func<string> newId)
        {
            _store = store;
            Coins = int.TryParse(store.Get(Key("coins")), out int coins) && coins >= 0 ? coins : new ContinueRules().StartCoins;
            // PM_MESAJ_57: start balance rose to 5000; lift existing lower balances once.
            if (store.Get(Key("coins_topup57")) == null)
            {
                if (Coins < 5000) SetCoins(5000);
                store.Set(Key("coins_topup57"), "1");
                store.Save();
            }
            Player = store.Get(Key("player")) ?? "";
            Install = store.Get(Key("install"));
            if (string.IsNullOrEmpty(Install))
            {
                Install = newId();
                store.Set(Key("install"), Install);
                store.Save();
            }
            ReadCounts("attempts", Attempts);
            ReadCounts("best", Best);
            ReadCounts("losses", Losses);
            ReadCounts("fun_asked", FunAsked);
            ReadCounts("chests21", _chests21);
        }

        public void SetCoins(int coins)
        {
            if (TestMode) return;
            Coins = Math.Max(0, coins);
            _store.Set(Key("coins"), Coins.ToString(System.Globalization.CultureInfo.InvariantCulture));
            _store.Save();
        }

        public int LossesOf(string levelId) => Losses.TryGetValue(levelId, out int n) ? n : 0;
        public bool WasFunAsked(string levelId) => FunAsked.ContainsKey(levelId);

        public void RecordLoss(string levelId)
        {
            Losses[levelId] = LossesOf(levelId) + 1;
            _store.Set(Key("losses"), Write(Losses));
            _store.Save();
        }

        public void MarkFunAsked(string levelId)
        {
            FunAsked[levelId] = 1;
            _store.Set(Key("fun_asked"), Write(FunAsked));
            _store.Save();
        }

        void ReadCounts(string name, Dictionary<string, int> into)
        {
            string raw = _store.Get(Key(name));
            if (string.IsNullOrEmpty(raw)) return;
            try
            {
                var o = JsonReader.Parse(raw);
                if (o.Kind != JsonKind.Object) return;
                foreach (var m in o.Members)
                    if (m.Value.Kind == JsonKind.Number && m.Value.AsNumber >= 0) into[m.Key] = (int)m.Value.AsNumber;
            }
            catch (JsonException) { into.Clear(); }     // corrupt: start over for this key
        }

        public void SetPlayer(string name)
        {
            Player = (name ?? "").Trim();
            _store.Set(Key("player"), Player);
            _store.Save();
        }

        /* Writes the log's counters (attempt numbers, best stars). */
        public void Store(AttemptLog log)
        {
            foreach (var kv in log.AttemptCounts) Attempts[kv.Key] = kv.Value;
            foreach (var kv in log.BestStarsByLevel) Best[kv.Key] = Math.Max(kv.Value, Best.TryGetValue(kv.Key, out int b) ? b : 0);
            _store.Set(Key("attempts"), Write(Attempts));
            _store.Set(Key("best"), Write(Best));
            _store.Save();
        }

        /* Zeyd 2026-10-28: best stars recovered from the player's won rows on the server (the browser's own save was
           empty: another browser, an in-app browser, cleared storage). Only raises; returns how many levels changed. */
        public int MergeBest(IEnumerable<KeyValuePair<string, int>> best)
        {
            int changed = 0;
            foreach (var kv in best)
            {
                if (string.IsNullOrEmpty(kv.Key) || kv.Value <= 0) continue;
                if (Best.TryGetValue(kv.Key, out int b) && b >= kv.Value) continue;
                Best[kv.Key] = kv.Value;
                changed++;
            }
            if (changed == 0) return 0;
            _store.Set(Key("best"), Write(Best));
            _store.Save();
            return changed;
        }

        /* An AttemptLog that continues the saved counters. */
        public AttemptLog Restore()
        {
            var log = new AttemptLog();
            log.Restore(Attempts, Best);
            return log;
        }

        static string Write(Dictionary<string, int> d)
        {
            var w = new JsonWriter().BeginObject();
            foreach (var kv in d) w.Field(kv.Key, kv.Value);
            return w.EndObject().ToString();
        }
    }
}
