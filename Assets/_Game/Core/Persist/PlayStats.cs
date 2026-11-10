using System;
using System.Collections.Generic;
using ReverseSolver.Core.Json;

namespace ReverseSolver.Core
{
    public sealed class LevelPlayStats
    {
        public int Wins { get; internal set; }
        public bool FirstTryWin { get; internal set; }
        internal bool HasFirstTryWin;
        public int? BestWinSeconds { get; internal set; }
        public int? MinJamsOnWin { get; internal set; }
        public int? MinLeftOnWin { get; internal set; }
        public bool HalfLeftWin { get; internal set; }
    }

    // Local aggregates only; never changes the telemetry record or upload queue.
    public sealed class PlayStats
    {
        readonly Dictionary<string, LevelPlayStats> _levels = new Dictionary<string, LevelPlayStats>();
        public int TotalSeconds { get; private set; }
        public int SumLeftOnWins { get; private set; }
        public int WinCount { get; private set; }
        public LevelPlayStats Of(string id) => _levels.TryGetValue(id, out var s) ? s : null;
        public void ImportLegacy(SaveData save)
        {
            foreach (var kv in save.Best)
            {
                if (kv.Value <= 0) continue;
                if (!_levels.TryGetValue(kv.Key, out var s)) _levels[kv.Key] = s = new LevelPlayStats();
                if (s.HasFirstTryWin) continue;
                s.FirstTryWin = save.LossesOf(kv.Key) == 0 && save.Attempts.TryGetValue(kv.Key, out int n) && n == 1;
                s.HasFirstTryWin = true;
            }
        }
        public bool FirstTry(string id, bool won, SaveData legacy = null)
        {
            var s = Of(id);
            return s != null && s.HasFirstTryWin ? s.FirstTryWin
                : won && legacy != null && legacy.LossesOf(id) == 0
                    && legacy.Attempts.TryGetValue(id, out int n) && n == 1;
        }

        public void Add(AttemptRecord r, int timerSeconds)
        {
            if (r == null || string.IsNullOrEmpty(r.LevelId)) return;
            if (r.Result != "win" && r.Result != "bomb" && r.Result != "time" && r.Result != "quit") return;
            TotalSeconds += Math.Max(0, r.DurationSeconds);
            if (!_levels.TryGetValue(r.LevelId, out var s)) _levels[r.LevelId] = s = new LevelPlayStats();
            if (r.Attempt == 1) { s.FirstTryWin = r.Result == "win" && r.Continues == 0; s.HasFirstTryWin = true; }
            if (r.Result != "win") return;
            s.Wins++; WinCount++;
            int left = Math.Max(0, r.LeftSeconds);
            SumLeftOnWins += left;
            s.BestWinSeconds = Min(s.BestWinSeconds, Math.Max(0, r.DurationSeconds));
            s.MinJamsOnWin = Min(s.MinJamsOnWin, Math.Max(0, r.Jams));
            s.MinLeftOnWin = Min(s.MinLeftOnWin, left);
            s.HalfLeftWin |= timerSeconds > 0 && (long)left * 2 >= timerSeconds;
        }
        static int Min(int? old, int value) => old.HasValue ? Math.Min(old.Value, value) : value;

        public void Save(IKeyValueStore store)
        {
            var w = new JsonWriter().BeginObject().Field("total", TotalSeconds).Field("left", SumLeftOnWins).Field("wins", WinCount);
            w.Key("levels").BeginObject();
            foreach (var kv in _levels)
            {
                var s = kv.Value;
                w.Key(kv.Key).BeginObject().Field("wins", s.Wins);
                if (s.HasFirstTryWin) w.Key("first").Value(s.FirstTryWin);
                Write(w, "best", s.BestWinSeconds); Write(w, "jams", s.MinJamsOnWin); Write(w, "left", s.MinLeftOnWin);
                w.Key("half").Value(s.HalfLeftWin).EndObject();
            }
            store.Set(SaveData.Key("stats"), w.EndObject().EndObject().ToString()); store.Save();
        }
        static void Write(JsonWriter w, string key, int? n) { w.Key(key); if (n.HasValue) w.Value(n.Value); else w.Null(); }
        static int? Number(JsonNode n, string key) => n.TryGet(key, out var v) && v.Kind == JsonKind.Number && v.AsNumber >= 0 && v.AsNumber <= int.MaxValue ? (int?)v.AsNumber : null;
        static bool Flag(JsonNode n, string key) => n.TryGet(key, out var v) && v.Kind == JsonKind.Bool && v.AsBool;
        public static PlayStats Load(IKeyValueStore store)
        {
            var s = new PlayStats();
            string raw = store.Get(SaveData.Key("stats"));
            if (string.IsNullOrEmpty(raw)) return s;
            try
            {
                var n = JsonReader.Parse(raw);
                s.TotalSeconds = Number(n, "total") ?? 0; s.SumLeftOnWins = Number(n, "left") ?? 0; s.WinCount = Number(n, "wins") ?? 0;
                if (n.TryGet("levels", out var levels) && levels.Kind == JsonKind.Object)
                    foreach (var kv in levels.Members)
                    {
                        var v = kv.Value; if (v.Kind != JsonKind.Object) continue;
                        s._levels[kv.Key] = new LevelPlayStats { Wins = Number(v, "wins") ?? 0,
                            FirstTryWin = Flag(v, "first"), HasFirstTryWin = v.TryGet("first", out var first) && first.Kind == JsonKind.Bool,
                            BestWinSeconds = Number(v, "best"), MinJamsOnWin = Number(v, "jams"), MinLeftOnWin = Number(v, "left"), HalfLeftWin = Flag(v, "half") };
                    }
            }
            catch (JsonException) { return new PlayStats(); }
            return s;
        }
    }
}
