using System;

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
        bool _hidden;

        public bool Enabled => _sender.Enabled;
        public TelemetryQueue Queue => _queue;
        public TelemetrySender Sender => _sender;

        public TelemetryLog(SaveData save, TelemetryQueue queue, TelemetrySender sender, Func<string> newId)
        {
            _save = save; _queue = queue; _sender = sender; _newId = newId;
        }

        public string NewRowId() => _newId();

        public void Finished(AttemptRecord record, string rowId)
        {
            if (!Enabled) return;
            _queue.Remove(rowId);        // a provisional quit from an earlier hide, if any
            _queue.Add(rowId, PlayRow.Json(record, _save.Player, _save.Install, rowId));
            Flush();
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
            if (!_hidden) _sender.Flush();
        }
    }
}
