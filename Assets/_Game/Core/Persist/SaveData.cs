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
    public sealed class SaveData
    {
        public const int Version = 1;
        public static readonly string Prefix = $"rs.v{Version}.";
        public static string Key(string name) => Prefix + name;

        readonly IKeyValueStore _store;

        public string Player { get; private set; }
        public string Install { get; private set; }
        public Dictionary<string, int> Attempts { get; } = new Dictionary<string, int>();
        public Dictionary<string, int> Best { get; } = new Dictionary<string, int>();

        public SaveData(IKeyValueStore store, Func<string> newId)
        {
            _store = store;
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
