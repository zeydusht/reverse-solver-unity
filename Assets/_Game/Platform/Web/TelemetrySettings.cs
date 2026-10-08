namespace ReverseSolver.Platform
{
    /* Where playtest rows go. Baked into the build; there is no URL switch.

       On since 2026-10-08: docs/sql/002_plays_level_id_row_id.sql was run in
       Supabase (checked with an anon read) and Zeyd approved going live. Rows
       named TEST_* are left out of analysis (PRODUCT.md). If the live check
       of the first build with sending on fails, set false and republish.
       Debug sessions (?debug=1) and the editor never send either way.

       The key is the project's publishable (anon) key, the same one the web
       game ships in config.js; it can only do what the table's row-level
       security allows. Never put a service_role key here. */
    public static class TelemetrySettings
    {
        public const bool SendEnabled = true;
        public const string SupabaseUrl = "https://vwneqdhzfevgbuwoadod.supabase.co";
        public const string AnonKey = "sb_publishable_S-DzN5GrUyMR2izo8vHc3Q_0kaGiVDK";
    }
}
