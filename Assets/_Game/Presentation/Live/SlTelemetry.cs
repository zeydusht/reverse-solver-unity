using System;
using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_81 §5: the soft launch's events into `sl_events` (docs/sql/019_soft_launch.sql). Only the store build's
       cohort (sl1) sends; everyone else writes nothing. Rows wait in the device's queue without a network and leave
       with the next flush (start, return to the app, every few events); the queue is saved on every row, so closing
       the app loses nothing. */
    public static class SlTelemetry
    {
        public const string FirstOpenKey = "rs_sl_first_open", FirstSessionKey = "rs_sl_first_session";
        static TelemetryQueue _queue;
        static TelemetrySender _sender;
        static Func<string> _newId;
        static string _player, _session;
        static double _sessionStart = -1;
        static bool _crashHooked;
        public static bool On { get; private set; }
        public static double SessionStart => _sessionStart;
        /* The first session (the one with first_open): the interstitial keeps out of its first 10 minutes. */
        public static bool FirstSession { get; private set; }

        public static void Start(IHost host, bool send, string player, Func<string> newId)
        {
            On = true;
            _newId = newId;
            _player = player;
            _queue = new TelemetryQueue(host.Store, "sl");
            _sender = new TelemetrySender(_queue, send ? host.Transport : null, host.SupabaseUrl, host.AnonKey, send, "sl_events?on_conflict=row_id");
            Ads.Event = (evt, placement, network, revenue, value) => Emit(evt, placement, network, revenue, value);
            if (!_crashHooked) { _crashHooked = true; Application.logMessageReceived += OnLog; }
            bool first = host.Store.Get(FirstOpenKey) == null;
            NewSession();
            if (first)
            {
                host.Store.Set(FirstOpenKey, Ads.Now.ToString(System.Globalization.CultureInfo.InvariantCulture));
                host.Store.Set(FirstSessionKey, _session);
                host.Store.Save();
                Emit("first_open");
            }
            FirstSession = host.Store.Get(FirstSessionKey) == _session;
            Emit("session_start");
            Flush();
        }

        static void NewSession()
        {
            _session = _newId();
            _sessionStart = Ads.Now;
        }

        /* App to the background: the session ends; back after more than a minute: a new one. */
        static double _hiddenAt = -1;
        public static void Visibility(bool visible)
        {
            if (!On) return;
            if (!visible)
            {
                _hiddenAt = Ads.Now;
                Emit("session_end", seconds: (int)Math.Max(0, Ads.Now - _sessionStart));
                Flush();
                return;
            }
            if (_hiddenAt >= 0 && Ads.Now - _hiddenAt > 60)
            {
                NewSession();
                FirstSession = false;
                Emit("session_start");
            }
            _hiddenAt = -1;
            Flush();
        }

        public static void Emit(string evt, string placement = null, string network = null, double? revenue = null, string value = null,
                                int? seconds = null, int? level = null, string levelId = null)
        {
            if (!On || _queue == null) return;
            string id = _newId();
            _queue.Add(id, SlEventRow.Json(id, evt, _player, _session, placement, network, revenue, value, seconds, level, levelId));
            if (_queue.Rows.Count >= 5) Flush();
        }

        public static void Flush() => _sender?.Flush();

        /* An uncaught exception: its message and the first 5 lines of the stack, once per distinct message a session. */
        static string _lastCrash;
        static void OnLog(string message, string stack, LogType type)
        {
            if (type != LogType.Exception || !On || message == _lastCrash) return;
            _lastCrash = message;
            Emit("crash", value: SlEventRow.Crash(message, stack));
            Flush();
        }
    }
}
