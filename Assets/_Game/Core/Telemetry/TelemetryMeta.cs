using ReverseSolver.Core.Json;

namespace ReverseSolver.Core
{
    /* PM_MESAJ_25 (docs/VERI_SISTEMI.md 4, docs/sql/009): who sent a row. Cohort from the link (?c=pt1, kept on the
       device), the device's anonymous id, platform and app version. Set once at start by the game; null (tests,
       tools) writes nothing, so the rows stay as before. */
    public sealed class TelemetryMeta
    {
        public string Cohort, DeviceId, Platform, AppVersion;

        public static TelemetryMeta Current;

        /* A cohort code as it may come from a link: 1-16 of a-z, 0-9, _ and -; anything else is no cohort. */
        public static string CleanCohort(string c)
        {
            if (string.IsNullOrEmpty(c) || c.Length > 16) return null;
            foreach (char ch in c)
                if (!(ch >= 'a' && ch <= 'z' || ch >= '0' && ch <= '9' || ch == '_' || ch == '-')) return null;
            return c;
        }

        /* PM_64: p22 is the next preview of the p21 game; both play the p21 flow, rows keep their own cohort.
           PM_65: the internal cohort (ic, test links) plays the same game, so no old tutorial shows up there. */
        public static bool P21Line(string c) => c == "p21" || c == "p22" || c == "ic";

        internal static void Plays(JsonWriter w)
        {
            var m = Current;
            if (m == null) return;
            w.Field("cohort", m.Cohort).Field("platform", m.Platform).Field("device_id", m.DeviceId).Field("app_version", m.AppVersion);
        }

        internal static void Moves(JsonWriter w)
        {
            var m = Current;
            if (m == null) return;
            w.Field("cohort", m.Cohort).Field("device_id", m.DeviceId);
        }
    }

    /* One row of `player_profiles` (docs/sql/009): the one question asked once per cohort. exp: 0 never, 1 sometimes, 2 often. */
    public static class ProfileRow
    {
        public static string Json(string cohort, string player, string deviceId, string platform, int exp) =>
            new JsonWriter().BeginObject().Field("cohort", cohort).Field("player", player).Field("device_id", deviceId)
                .Field("platform", platform).Field("puzzle_exp", exp).EndObject().ToString();
    }
}
