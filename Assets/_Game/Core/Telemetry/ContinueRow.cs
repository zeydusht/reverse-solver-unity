using ReverseSolver.Core.Json;

namespace ReverseSolver.Core
{
    // A separate table: never add continue fields to the plays payload.
    public static class ContinueRow
    {
        public static string Json(string id, string attemptId, string player, AttemptRecord attempt,
            int continueNo, string kind, int price, int seconds, int coinsBefore, string result)
        {
            var m = TelemetryMeta.Current;
            return new JsonWriter().BeginObject().Field("row_id", id).Field("attempt_row_id", attemptId)
                .Field("player", player).Field("cohort", m?.Cohort).Field("device_id", m?.DeviceId)
                .Field("app_version", m?.AppVersion).Field("level_id", attempt.LevelId)
                .Field("level_version", attempt.LevelVersion).Field("attempt", attempt.Attempt)
                .Field("continue_no", continueNo).Field("kind", kind).Field("price", price)
                .Field("seconds", seconds).Field("coins_before", coinsBefore).Field("result", result)
                .EndObject().ToString();
        }
    }
}
