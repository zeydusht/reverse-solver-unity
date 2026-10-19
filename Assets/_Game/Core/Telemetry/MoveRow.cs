using ReverseSolver.Core.Json;

namespace ReverseSolver.Core
{
    /* One row of the Supabase `moves` table (ZeydBot, Zeyd 2026-10-19; docs/sql/006_moves.sql): every
       action of an attempt, tied to its `plays` row by row_id. The options at that moment (right /
       trap), the fuses and the solver's view are computed afterwards by replaying the moves in the
       editor, so the phone does no search. */
    public static class MoveRow
    {
        public static string Json(string rowId, string player, string levelId, int levelVersion, int attempt, int moveNo,
                                  string kind, int piece, string dir, int tMs, int thinkMs, int timeLeft, string fuses)
        {
            var w = new JsonWriter().BeginObject()
                .Field("row_id", rowId).Field("player", player).Field("level_id", levelId).Field("level_version", levelVersion)
                .Field("attempt", attempt).Field("move_no", moveNo).Field("kind", kind).Field("piece", piece).Field("dir", dir)
                .Field("t_ms", tMs).Field("think_ms", thinkMs).Field("time_left", timeLeft).Field("fuses", fuses);
            return w.EndObject().ToString();
        }
    }
}
