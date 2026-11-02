using System;
using ReverseSolver.Core.Json;

namespace ReverseSolver.Core
{
    /* Glue between finished attempts and the send queue.

       Each attempt gets its row_id when it starts. When it ends, its row is
       queued and a send is tried. When the page is hidden mid-attempt, a quit
       row is queued under the same id and sending pauses: if the tab is
       closed, that row goes out on the next visit; if the player comes back,
       it is taken out again and the attempt carries on. Sending stays paused
       while hidden so a provisional quit can never reach the server ahead of
       the attempt's real result (the server keeps the first row per row_id).

       When sending is disabled (the shipped default until the SQL has been
       run, and always with ?debug=1 or in the editor) nothing is queued at
       all: rows made then are never sent later. */
    public sealed class TelemetryLog
    {
        readonly SaveData _save;
        readonly TelemetryQueue _queue;
        readonly TelemetrySender _sender;
        readonly Func<string> _newId;
        readonly TelemetryQueue _ratingQueue;
        readonly TelemetrySender _ratings;
        bool _hidden;

        public bool Enabled => _sender.Enabled;
        public TelemetryQueue Queue => _queue;
        public TelemetrySender Sender => _sender;

        /* ratings: rpc/set_rating (docs/sql/012). The 1-10 and 👍/👎 answers fill an already sent row;
           they go only once the plays queue is empty, so the row is on the server first. */
        public TelemetryLog(SaveData save, TelemetryQueue queue, TelemetrySender sender, Func<string> newId,
                            TelemetryQueue ratingQueue = null, TelemetrySender ratings = null)
        {
            _save = save; _queue = queue; _sender = sender; _newId = newId;
            _ratingQueue = ratingQueue; _ratings = ratings;
            if (_ratings != null) _sender.Drained = FlushRatings;
        }

        public string NewRowId() => _newId();

        public void Finished(AttemptRecord record, string rowId)
        {
            if (!Enabled) return;
            _queue.Remove(rowId);        // a provisional quit from an earlier hide, if any
            _queue.Add(rowId, PlayRow.Json(record, _save.Player, _save.Install, rowId));
            Flush();
        }

        /* PM_MESAJ_29: the row went when the level ended; an answer given later fills its empty column. */
        public void Rate(string rowId, int? hardness, int? fun)
        {
            if (!Enabled || _ratings == null || (hardness == null && fun == null)) return;
            var w = new JsonWriter().BeginObject().Field("p_row_id", rowId);
            if (hardness.HasValue) w.Field("p_zorluk", hardness.Value);
            if (fun.HasValue) w.Field("p_keyif", fun.Value);
            _ratingQueue.Add(rowId + (hardness.HasValue ? ":h" : ":f"), w.EndObject().ToString());
            Flush();
        }

        void FlushRatings()
        {
            if (!_hidden && _ratings != null && _queue.Rows.Count == 0 && !_sender.Busy) _ratings.Flush();
        }

        public void PageHidden(GameSession open, string rowId)
        {
            _hidden = true;
            if (!Enabled || open == null || open.IsOver) return;
            _queue.Add(rowId, PlayRow.Json(open.PreviewQuit(), _save.Player, _save.Install, rowId));
        }

        public void PageVisible(GameSession open, string rowId)
        {
            _hidden = false;
            if (Enabled && open != null && !open.IsOver) _queue.Remove(rowId);
            Flush();
        }

        public void Flush()
        {
            if (_hidden) return;
            _sender.Flush();
            FlushRatings();
        }
    }
}
