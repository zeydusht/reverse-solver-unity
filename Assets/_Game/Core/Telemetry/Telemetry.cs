using System;
using System.Collections.Generic;
using ReverseSolver.Core.Json;

namespace ReverseSolver.Core
{
    /* One row of the Supabase `plays` table, column for column. The web game
       wrote the first fifteen columns; client, level_version (docs/sql via
       Supabase/001), level_id and row_id (docs/sql/002) are the Unity additions. */
    public static class PlayRow
    {
        public static string Json(AttemptRecord r, string player, string session, string rowId) =>
            new JsonWriter().BeginObject()
                .Field("player", player)
                .Field("session", session)
                .Field("lv", r.LevelNumber)
                .Field("attempt", r.Attempt)
                .Field("result", r.Result)
                .Field("dur", r.DurationSeconds)
                .Field("left_s", r.LeftSeconds)
                .Field("moves", r.Moves)
                .Field("jams", r.Jams)
                .Field("stars", r.Stars)
                .Field("load", r.Load)
                .Field("b_scissors", r.Scissors)
                .Field("b_wand", r.Wand)
                .Field("b_hammer", r.Hammer)
                .Field("b_clock", r.Clock)
                .Field("client", r.Client)
                .Field("level_version", r.LevelVersion)
                .Field("level_id", r.LevelId)
                .Field("row_id", rowId)
                .EndObject().ToString();
    }

    /* Rows waiting to be sent, kept in the store so a reload or a dropped
       connection loses nothing (as the web game's queue). Each row carries
       its row_id for life, so a retry can never become a second row.
       Bounded: past Max the oldest rows go. A row the server rejects (4xx)
       is set aside so it cannot block the rows behind it. */
    public sealed class TelemetryQueue
    {
        public const int Max = 500, MaxParked = 100;
        static readonly string Key = SaveData.Key("queue");
        static readonly string ParkedKey = SaveData.Key("queue.parked");

        public sealed class Entry { public string Id; public string Row; }

        readonly IKeyValueStore _store;
        readonly List<Entry> _rows = new List<Entry>();
        readonly List<Entry> _parked = new List<Entry>();

        public IReadOnlyList<Entry> Rows => _rows;
        public int Parked => _parked.Count;
        public int Dropped { get; private set; }

        public TelemetryQueue(IKeyValueStore store)
        {
            _store = store;
            Read(Key, _rows);
            Read(ParkedKey, _parked);
        }

        public void Add(string id, string rowJson)
        {
            if (_rows.Exists(e => e.Id == id)) return;
            _rows.Add(new Entry { Id = id, Row = rowJson });
            while (_rows.Count > Max) { _rows.RemoveAt(0); Dropped++; }
            Persist();
        }

        public bool Remove(string id)
        {
            int n = _rows.RemoveAll(e => e.Id == id);
            if (n > 0) Persist();
            return n > 0;
        }

        public List<Entry> Take(int count) => _rows.GetRange(0, Math.Min(count, _rows.Count));

        public void Accept(IEnumerable<Entry> sent)
        {
            foreach (var e in sent) _rows.RemoveAll(r => r.Id == e.Id);
            Persist();
        }

        public void Park(IEnumerable<Entry> rejected)
        {
            foreach (var e in rejected)
            {
                _rows.RemoveAll(r => r.Id == e.Id);
                _parked.Add(e);
            }
            while (_parked.Count > MaxParked) _parked.RemoveAt(0);
            Persist();
        }

        void Persist()
        {
            _store.Set(Key, Write(_rows));
            _store.Set(ParkedKey, Write(_parked));
            _store.Save();
        }

        static string Write(List<Entry> list)
        {
            var w = new JsonWriter().BeginArray();
            foreach (var e in list) w.BeginObject().Field("id", e.Id).Field("row", e.Row).EndObject();
            return w.EndArray().ToString();
        }

        void Read(string key, List<Entry> into)
        {
            string raw = _store.Get(key);
            if (string.IsNullOrEmpty(raw)) return;
            try
            {
                var a = JsonReader.Parse(raw);
                if (a.Kind != JsonKind.Array) return;
                foreach (var n in a.Items)
                    if (n.TryGet("id", out var id) && id.Kind == JsonKind.String &&
                        n.TryGet("row", out var row) && row.Kind == JsonKind.String)
                        into.Add(new Entry { Id = id.AsString, Row = row.AsString });
            }
            catch (JsonException) { into.Clear(); }
        }
    }

    /* HTTP as the sender sees it. Status 0 means no response (offline, CORS, timeout). */
    public interface ITelemetryTransport
    {
        void Post(string url, IReadOnlyDictionary<string, string> headers, string body, Action<int> done);
    }

    /* Sends queued rows to Supabase's REST endpoint in batches of 25, one
       request at a time, like the web game. It does nothing at all unless it
       is enabled with a transport: the build ships with sending off until the
       SQL for the new columns has been run (PRODUCT.md, M5-pre).

         2xx           rows leave the queue
         0 / 5xx / 429 rows stay, sent again later with the same row_id
         other 4xx     rows are set aside (Parked) so the queue keeps moving

       Duplicates: POST ...?on_conflict=row_id with
       Prefer: resolution=ignore-duplicates, so a row that did reach the
       server before a retry is ignored there. */
    public sealed class TelemetrySender
    {
        public const int Batch = 25;

        readonly TelemetryQueue _queue;
        readonly ITelemetryTransport _transport;
        readonly string _url;
        readonly Dictionary<string, string> _headers;

        public bool Enabled { get; }
        public bool Busy { get; private set; }
        public int Calls { get; private set; }
        public int Sent { get; private set; }
        public int LastStatus { get; private set; } = -1;

        public TelemetrySender(TelemetryQueue queue, ITelemetryTransport transport, string supabaseUrl, string anonKey, bool enabled)
        {
            _queue = queue;
            _transport = transport;
            Enabled = enabled && transport != null && !string.IsNullOrEmpty(supabaseUrl) && !string.IsNullOrEmpty(anonKey);
            // accept either the project URL or the full REST endpoint (web LOG.flush)
            string baseUrl = (supabaseUrl ?? "").Trim().TrimEnd('/');
            if (baseUrl.EndsWith("/rest/v1")) baseUrl = baseUrl.Substring(0, baseUrl.Length - "/rest/v1".Length);
            _url = baseUrl + "/rest/v1/plays?on_conflict=row_id";
            _headers = new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json",
                ["apikey"] = anonKey ?? "",
                ["Authorization"] = "Bearer " + anonKey,
                ["Prefer"] = "resolution=ignore-duplicates,return=minimal"
            };
        }

        public string Url => _url;

        /* Sends the next batch if there is one; continues until the queue is
           empty or a request fails. Safe to call often. */
        public void Flush()
        {
            if (!Enabled || Busy || _queue.Rows.Count == 0) return;
            var batch = _queue.Take(Batch);
            var w = new JsonWriter().BeginArray();
            foreach (var e in batch) w.Raw(e.Row);
            string body = w.EndArray().ToString();
            Busy = true;
            Calls++;
            _transport.Post(_url, _headers, body, status =>
            {
                Busy = false;
                LastStatus = status;
                if (status >= 200 && status < 300)
                {
                    _queue.Accept(batch);
                    Sent += batch.Count;
                    Flush();
                }
                else if (status >= 400 && status < 500 && status != 408 && status != 429)
                {
                    _queue.Park(batch);
                    Flush();
                }
                // 0, 408, 429, 5xx: keep the rows; the next Flush retries them
            });
        }
    }
}
