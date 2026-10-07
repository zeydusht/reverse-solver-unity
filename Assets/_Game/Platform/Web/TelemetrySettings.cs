namespace ReverseSolver.Platform
{
    /* Where playtest rows go. Baked into the build; there is no URL switch.

       SendEnabled stays false until docs/sql/002_plays_level_id_row_id.sql
       has been run in Supabase (the rows carry level_id and row_id, which the
       table rejects before that). Turning it on: Zeyd runs the SQL -> set true
       -> build -> check Zeyd's own test row -> PM approves (PRODUCT.md).
       Debug sessions (?debug=1) and the editor never send either way.

       The key is the project's publishable (anon) key, the same one the web
       game ships in config.js; it can only do what the table's row-level
       security allows. Never put a service_role key here. */
    public static class TelemetrySettings
    {
        public const bool SendEnabled = false;
        public const string SupabaseUrl = "https://vwneqdhzfevgbuwoadod.supabase.co";
        public const string AnonKey = "sb_publishable_S-DzN5GrUyMR2izo8vHc3Q_0kaGiVDK";
    }
}
