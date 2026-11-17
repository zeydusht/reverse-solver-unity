using ReverseSolver.Core.Json;

namespace ReverseSolver.Core
{
    /* PM_81 §5: one row of `sl_events` (docs/sql/019_soft_launch.sql), the soft launch's own events:
         first_open, session_start, session_end (seconds), att_status (value), ad_shown (placement, network, revenue),
         ad_reward, ad_failed (value: the error), ad_skipped_not_ready, crash (value: message and 5 lines of stack).
       Cohort, platform, device id (IDFV on iOS), app version from TelemetryMeta; the content version from live/. */
    public static class SlEventRow
    {
        public static string ContentVersion = "";

        public static string Json(string rowId, string evt, string player, string session, string placement = null, string network = null,
                                  double? revenue = null, string value = null, int? seconds = null, int? level = null, string levelId = null)
        {
            var w = new JsonWriter().BeginObject().Field("row_id", rowId).Field("event", evt).Field("player", player).Field("session", session);
            var m = TelemetryMeta.Current;
            w.Field("cohort", m?.Cohort).Field("platform", m?.Platform).Field("device_id", m?.DeviceId).Field("app_version", m?.AppVersion);
            w.Field("content_version", ContentVersion);
            // every column every time: PostgREST sends a batch only of rows with the same keys
            w.Field("placement", placement).Field("network", network);
            if (revenue.HasValue) w.Field("revenue", revenue.Value); else w.Key("revenue").Null();
            w.Field("value", value != null && value.Length > 1000 ? value.Substring(0, 1000) : value);
            if (seconds.HasValue) w.Field("seconds", seconds.Value); else w.Key("seconds").Null();
            if (level.HasValue) w.Field("lv", level.Value); else w.Key("lv").Null();
            w.Field("level_id", levelId);
            return w.EndObject().ToString();
        }

        /* A crash's text: the message and the first 5 lines of the stack. */
        public static string Crash(string message, string stack)
        {
            var lines = (stack ?? "").Replace("\r", "").Split('\n');
            var sb = new System.Text.StringBuilder(message ?? "");
            for (int i = 0, n = 0; i < lines.Length && n < 5; i++)
                if (lines[i].Trim().Length > 0) { sb.Append('\n').Append(lines[i].Trim()); n++; }
            return sb.ToString();
        }
    }
}
