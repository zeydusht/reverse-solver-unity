using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ReverseSolver.Core;
using ReverseSolver.Core.Json;
using ReverseSolver.Editing;
using UnityEngine;

namespace ReverseSolver.LevelEditor
{
    /* ZeydBot step 4 (Zeyd 2026-10-19): every recorded move of Zeyd's Z attempts replayed on the level;
       at each exit the options at that moment (pieces that can leave), how many are right (SmartPlayer
       still wins clean after them) and how many traps, and whether the chosen one was right. The level's
       expected values (shortest solution, timer, random player's win rate) are joined in the same table.
       Input: Logs/zeydbot/plays.json and moves.json (node Tools/zeydbot_fetch.js). Output:
       Logs/zeydbot/moves_analiz.csv (one row per move) and Logs/zeydbot/bolumler.csv (one row per attempt).
       Only the first attempt is for the analysis; later attempts are kept and flagged (ilk_deneme = 0).

         bash Tools/unity_batch.sh method ReverseSolver.LevelEditor.ZeydAnalysis.Run */
    public static class ZeydAnalysis
    {
        const int Budget = 100_000;
        static void Log(string s) => Debug.Log("[zeydanaliz] " + s);

        public static void Run()
        {
            var set = LevelParser.Parse(File.ReadAllText(ZeydSet.Path));
            var levels = set.Levels.ToDictionary(l => l.Id);
            var plays = JsonReader.Parse(File.ReadAllText("Logs/zeydbot/plays.json"));
            var moves = JsonReader.Parse(File.ReadAllText("Logs/zeydbot/moves.json"));
            var byRow = new Dictionary<string, List<JsonNode>>();
            foreach (var m in moves.Items) { var id = S(m, "row_id"); if (!byRow.ContainsKey(id)) byRow[id] = new List<JsonNode>(); byRow[id].Add(m); }

            var expected = new Dictionary<string, (int shortest, int timer, double random)>();
            foreach (var l in set.Levels)
            {
                var sp = SmartPlayer.Play(l);
                expected[l.Id] = (sp.Moves.Count, l.TimerSeconds, MonteCarlo.Run(l, 300).WinRate);
            }

            var mv = new StringBuilder("level_id,attempt,ilk_deneme,move_no,kind,piece,dir,t_ms,think_ms,hold_ms,time_left,secenek,dogru,tuzak,secilen,en_kisa_cozum,sure_limiti,rastgele_kazanma\n");
            var at = new StringBuilder("level_id,attempt,ilk_deneme,result,left_seconds,duration,hamle,touch,pause,dogru_secim,tuzak_secim,zorluk_puani,en_kisa_cozum,sure_limiti,rastgele_kazanma\n");
            foreach (var p in plays.Items)
            {
                string levelId = S(p, "level_id"), rowId = S(p, "row_id");
                if (!levels.TryGetValue(levelId, out var l)) continue;
                int attempt = I(p, "attempt");
                var ex = expected[levelId];
                var s = new GameSession(l, 1, "analiz", new Mulberry32(1)) { ClockOnFirstMove = false };
                int right = 0, trap = 0, touches = 0, pauses = 0, exits = 0;
                foreach (var m in byRow.TryGetValue(rowId, out var list) ? list.OrderBy(x => I(x, "move_no")) : Enumerable.Empty<JsonNode>())
                {
                    string kind = S(m, "kind"); int piece = I(m, "piece"); string dir = S(m, "dir");
                    int options = -1, r = -1, t = -1; string chosen = "";
                    if (kind == "exit" && !s.IsOver && Enum.TryParse(dir, out Dir d))
                    {
                        var opts = TravelRule.FreePieces(s.Board).Where(x => s.NailAt(x) <= 0).ToList();
                        options = opts.Count; r = 0; t = 0; bool chosenRight = false;
                        foreach (var o in opts)
                        {
                            bool ok = TravelRule.ExitDirs(s.Board, o).Any(od => { var res = SmartPlayer.PlayAfter(s, new Move(o, od), Budget); return res != null && res.Verdict == SmartVerdict.Won && res.Valves == 0; });
                            if (ok) r++; else t++;
                            if (o == piece) chosenRight = ok;
                        }
                        chosen = chosenRight ? "dogru" : "tuzak";
                        if (chosenRight) right++; else trap++;
                        s.TryExit(piece, d);
                        exits++;
                    }
                    else if (kind == "hammer" && !s.IsOver) s.UseHammer(piece);
                    else if (kind == "clock" && !s.IsOver) s.UseClock();
                    else if (kind == "jam" && !s.IsOver) s.RecordJam(piece);
                    if (kind == "touch") touches++;
                    if (kind == "pause") pauses++;
                    mv.Append($"{levelId},{attempt},{(attempt == 1 ? 1 : 0)},{I(m, "move_no")},{kind},{piece},{dir},{I(m, "t_ms")},{I(m, "think_ms")},{IOpt(m, "hold_ms")},{I(m, "time_left")},{N(options)},{N(r)},{N(t)},{chosen},{ex.shortest},{ex.timer},{ex.random.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture)}\n");
                }
                at.Append($"{levelId},{attempt},{(attempt == 1 ? 1 : 0)},{S(p, "result")},{I(p, "left_s")},{I(p, "dur")},{exits},{touches},{pauses},{right},{trap},{IOpt(p, "zorluk_puani")},{ex.shortest},{ex.timer},{ex.random.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture)}\n");
                Log($"{levelId} deneme {attempt}: {S(p, "result")}, {exits} çıkış, doğru {right} / tuzak {trap}");
            }
            File.WriteAllText("Logs/zeydbot/moves_analiz.csv", mv.ToString(), new UTF8Encoding(true));
            File.WriteAllText("Logs/zeydbot/bolumler.csv", at.ToString(), new UTF8Encoding(true));
            Log("yazıldı: Logs/zeydbot/moves_analiz.csv, Logs/zeydbot/bolumler.csv");
        }

        static string N(int v) => v < 0 ? "" : v.ToString();
        static string S(JsonNode n, string k) => n.TryGet(k, out var v) && v.Kind == JsonKind.String ? v.AsString : "";
        static int I(JsonNode n, string k) => n.TryGet(k, out var v) && v.Kind == JsonKind.Number ? (int)v.AsNumber : -1;
        static string IOpt(JsonNode n, string k) => n.TryGet(k, out var v) && v.Kind == JsonKind.Number ? ((int)v.AsNumber).ToString() : "";
    }
}
