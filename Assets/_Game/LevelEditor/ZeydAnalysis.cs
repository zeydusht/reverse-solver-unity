using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ReverseSolver.Core;
using ReverseSolver.Core.Json;
using ReverseSolver.Editing;
using UnityEngine;

namespace ReverseSolver.LevelEditor
{
    /* ZeydBot (Zeyd 2026-10-19/20): every recorded move of Zeyd's Z attempts replayed on the level.
       At each exit: the options at that moment (pieces that can leave), how many are right (SmartPlayer
       still wins clean after them) and how many traps, whether the chosen one was right, and per bomb the
       slack left (fuse - fewest moves that take it out from here). Per level (design only, no play result,
       so also fine for Z21-Z30): expected values and the path features the model may use.

       Input: Logs/zeydbot/plays.json, moves.json (node Tools/zeydbot_fetch.js [--all]; v2: every Z level with frozen predictions).
       Output in docs/analiz/zeydbot/: hamleler.csv (one row per move), denemeler.csv (one per attempt),
       bolum_ozellik.csv (one per level Z01-Z30, design only).

         bash Tools/unity_batch.sh method ReverseSolver.LevelEditor.ZeydAnalysis.Run */
    public static class ZeydAnalysis
    {
        const int Budget = 100_000;
        public const string Out = "docs/analiz/zeydbot";
        static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
        static void Log(string s) => Debug.Log("[zeydanaliz] " + s);

        /* Options at this state: (piece, right). Right = some exit of it still lets SmartPlayer win with no valve. */
        public static List<(int piece, bool right)> Options(GameSession s, int budget = Budget)
        {
            var list = new List<(int, bool)>();
            foreach (var o in TravelRule.FreePieces(s.Board).Where(x => s.NailAt(x) <= 0))
            {
                bool ok = TravelRule.ExitDirs(s.Board, o).Any(od => { var r = SmartPlayer.PlayAfter(s, new Move(o, od), budget); return r != null && r.Verdict == SmartVerdict.Won && r.Valves == 0; });
                list.Add((o, ok));
            }
            return list;
        }

        /* Per bomb still on the board: fuse left - fewest moves to take it out (-99 = no dig before a bomb goes). */
        /* lng: the same state with every fuse long (Long), so a dig is not cut short by another bomb's fuse;
           the same definition as StructureMetrics' fuse margin, from the current state. */
        public static List<(int piece, int slack)> Slacks(GameSession s, GameSession lng) =>
            s.Bombs.Where(b => s.Board.IsPresent(b.Key)).Select(b => { int d = StructureMetrics.DigNow(lng, b.Key); return (b.Key, d < 0 ? -99 : b.Value - d); }).ToList();

        /* v2 (PM 2026-10-20, "düşündüren zor"): pieces that can slide at least one cell but not leave — they look
           free and are not (Z12: Zeyd touched / jammed on 0.46 of his moves, the other hard levels 0.06-0.21). */
        public static int Misleading(GameSession s) =>
            s.Board.PresentPieces().Count(p => s.NailAt(p) <= 0 && !TravelRule.CanExit(s.Board, p) &&
                DirExt.All.Any(d => TravelRule.Limit(s.Board, p, d).Cells > 0));

        public static GameSession Long(LevelData l)
        {
            var d = LevelDraft.FromLevel(l);
            foreach (var b in l.Bombs) d.SetBomb(l.Pieces[b.Key][0], LevelDraft.MaxCount);
            return new GameSession(d.ToLevelData(), 1, "uzun", new Mulberry32(1)) { ClockOnFirstMove = false };
        }

        public static void Run()
        {
            Directory.CreateDirectory(Out);
            var set = LevelParser.Parse(File.ReadAllText(ZeydSet.Path));
            var levels = set.Levels.ToDictionary(l => l.Id);
            Features(set);

            var plays = JsonReader.Parse(File.ReadAllText("Logs/zeydbot/plays.json"));
            var moves = JsonReader.Parse(File.ReadAllText("Logs/zeydbot/moves.json"));
            var byRow = new Dictionary<string, List<JsonNode>>();
            foreach (var m in moves.Items) { var id = S(m, "row_id"); if (!byRow.ContainsKey(id)) byRow[id] = new List<JsonNode>(); byRow[id].Add(m); }

            var mv = new StringBuilder("level_id,attempt,move_no,kind,piece,dir,t_ms,think_ms,hold_ms,time_left,kalan_parca,secenek,dogru,tuzak,secilen,bomba_pay,en_dar_pay,secim_sonrasi_en_dar_pay,yaniltici\n");
            var at = new StringBuilder("level_id,attempt,result,left_s,dur,hamle,touch,pause,dogru_secim,tuzak_secim,zorluk_puani,think_ort_ms,think_medyan_ms,think_max_ms,kayit\n");
            foreach (var p in plays.Items)
            {
                string levelId = S(p, "level_id"), rowId = S(p, "row_id");
                if (!levels.TryGetValue(levelId, out var l)) continue;
                int attempt = I(p, "attempt");
                var s = new GameSession(l, 1, "analiz", new Mulberry32(1)) { ClockOnFirstMove = false };
                var lng = Long(l);
                int right = 0, trap = 0, touches = 0, pauses = 0, exits = 0;
                var thinks = new List<int>();
                var list = byRow.TryGetValue(rowId, out var ls) ? ls.OrderBy(x => I(x, "move_no")).ToList() : new List<JsonNode>();
                foreach (var m in list)
                {
                    string kind = S(m, "kind"); int piece = I(m, "piece"); string dir = S(m, "dir");
                    string opt = "", r = "", t = "", chosen = "", slack = "", minSlack = "", after = "", mis = "";
                    int left = s.Board.PresentCount;
                    if (kind == "exit" && !s.IsOver && Enum.TryParse(dir, out Dir d))
                    {
                        var opts = Options(s);
                        var sl = Slacks(s, lng);
                        mis = Misleading(s).ToString();
                        opt = opts.Count.ToString(); r = opts.Count(o => o.right).ToString(); t = opts.Count(o => !o.right).ToString();
                        bool ok = opts.Any(o => o.piece == piece && o.right);
                        chosen = ok ? "dogru" : "tuzak";
                        if (ok) right++; else trap++;
                        slack = string.Join(";", sl.Select(x => x.piece + ":" + x.slack));
                        minSlack = sl.Count == 0 ? "" : sl.Min(x => x.slack).ToString();
                        s.TryExit(piece, d); lng.TryExit(piece, d);
                        var sa = s.IsOver ? new List<(int, int)>() : Slacks(s, lng);
                        after = sa.Count == 0 ? "" : sa.Min(x => x.Item2).ToString();
                        exits++;
                        thinks.Add(I(m, "think_ms"));
                    }
                    else if (kind == "hammer" && !s.IsOver) { s.UseHammer(piece); lng.UseHammer(piece); }
                    else if (kind == "clock" && !s.IsOver) s.UseClock();
                    else if (kind == "jam" && !s.IsOver) s.RecordJam(piece);
                    if (kind == "touch") touches++;
                    if (kind == "pause") pauses++;
                    mv.Append($"{levelId},{attempt},{I(m, "move_no")},{kind},{piece},{dir},{I(m, "t_ms")},{I(m, "think_ms")},{IOpt(m, "hold_ms")},{I(m, "time_left")},{left},{opt},{r},{t},{chosen},{slack},{minSlack},{after},{mis}\n");
                }
                thinks.Sort();
                string kayit = list.Count == 0 ? "hamle_yok" : levelId == "Z03" && attempt == 2 ? "atildi_quit" : "";
                at.Append($"{levelId},{attempt},{S(p, "result")},{I(p, "left_s")},{I(p, "dur")},{exits},{touches},{pauses},{right},{trap},{IOpt(p, "zorluk_puani")}," +
                          $"{(thinks.Count == 0 ? "" : thinks.Average().ToString("0", Inv))},{(thinks.Count == 0 ? "" : thinks[thinks.Count / 2].ToString())},{(thinks.Count == 0 ? "" : thinks.Max().ToString())},{kayit}\n");
                Log($"{levelId} deneme {attempt}: {S(p, "result")}, {exits} çıkış, doğru {right} / tuzak {trap}");
            }
            File.WriteAllText(Out + "/hamleler.csv", mv.ToString(), new UTF8Encoding(false));
            File.WriteAllText(Out + "/denemeler.csv", at.ToString(), new UTF8Encoding(false));
            Log("yazıldı: hamleler.csv, denemeler.csv");
        }

        /* Design-only features of every level (Z01-Z30): no play result enters here. Along SmartPlayer's path:
           options / right per move, moves with exactly one right option ("tek_dogru"), the right share,
           the tightest bomb slack on the path. */
        static void Features(LevelSet set)
        {
            var root = JsonReader.Parse(File.ReadAllText(ZeydSet.Path));
            root.TryGet("levels", out var jl);
            var sb = new StringBuilder("level_id,tur,oturum,parca,sure_limiti,en_kisa_cozum,rastgele_kazanma,bomba,civi,zincir,baslangic_en_dar_pay,baslangic_ort_pay,en_derin_kazi," +
                                       "m1_dogru,m1_tuzak,m2_en_cok_serbest,m4,m12_tek_eksen,yol_ort_secenek,yol_ort_dogru_orani,yol_tek_dogru,yol_tuzakli_hamle,yol_tuzak_orani_ort,yol_en_dar_pay\n");
            var yol = new StringBuilder("level_id,adim,kalan_parca,secenek,dogru,en_dar_pay,yaniltici\n");
            for (int i = 0; i < set.Levels.Count; i++)
            {
                var l = set.Levels[i];
                jl[i].TryGet("zeydbot", out var z); z.TryGet("kind", out var k); z.TryGet("session", out var se);
                var path = SmartPlayer.Play(l).Moves;
                var s = new GameSession(l, 1, "oz", new Mulberry32(1)) { ClockOnFirstMove = false };
                var lng = Long(l);
                var start = Slacks(s, lng);
                var digs = s.Bombs.Select(b => StructureMetrics.DigNow(lng, b.Key)).ToList();
                var first = Options(s);
                int single = 0, trapMoves = 0, n = 0, pathMin = start.Count == 0 ? 99 : start.Min(x => x.slack);
                double optSum = 0, rightShare = 0, trapShare = 0;
                foreach (var mv in path)
                {
                    if (s.IsOver) break;
                    var o = Options(s);
                    int rr = o.Count(x => x.right);
                    var sn = Slacks(s, lng);
                    yol.Append($"{l.Id},{n},{s.Board.PresentCount},{o.Count},{rr},{(sn.Count == 0 ? "" : sn.Min(x => x.slack).ToString())},{Misleading(s)}\n");
                    if (o.Count > 0)
                    {
                        n++; optSum += o.Count; rightShare += (double)rr / o.Count; trapShare += (double)(o.Count - rr) / o.Count;
                        if (rr == 1) single++;
                        if (rr < o.Count) trapMoves++;
                    }
                    s.TryExit(mv.Piece, mv.Dir); lng.TryExit(mv.Piece, mv.Dir);
                    if (!s.IsOver) foreach (var x in Slacks(s, lng)) pathMin = Math.Min(pathMin, x.slack);
                }
                var f = HardFactors.Of(l);
                int vertical = path.Count(m => m.Dir.IsVertical());
                double axis = path.Count == 0 ? 0 : Math.Max(vertical, path.Count - vertical) / (double)path.Count;
                var mc = MonteCarlo.Run(l, 300);
                string F(double v) => v.ToString("0.000", Inv);
                sb.Append($"{l.Id},{k.AsString},{se.AsNumber},{l.Pieces.Count},{l.TimerSeconds},{path.Count},{F(mc.WinRate)},{l.Bombs.Count},{l.Nails.Count},{l.Pieces.Count(p => p.Length == 2)}," +
                          $"{(start.Count == 0 ? "" : start.Min(x => x.slack).ToString())},{(start.Count == 0 ? "" : F(start.Average(x => x.slack)))},{(digs.Count == 0 ? "" : digs.Max().ToString())}," +
                          $"{first.Count(x => x.right)},{first.Count(x => !x.right)},{f.MaxFreeAtAll},{f.LaneRun},{F(axis)},{F(n == 0 ? 0 : optSum / n)},{F(n == 0 ? 0 : rightShare / n)},{single},{trapMoves},{F(n == 0 ? 0 : trapShare / n)},{(pathMin == 99 ? "" : pathMin.ToString())}\n");
                Log($"{l.Id} özellik: tek doğru {single}, tuzaklı {trapMoves}, en dar pay {pathMin}");
            }
            File.WriteAllText(Out + "/bolum_ozellik.csv", sb.ToString(), new UTF8Encoding(false));
            File.WriteAllText(Out + "/bolum_yol.csv", yol.ToString(), new UTF8Encoding(false));
        }

        static string S(JsonNode n, string k) => n.TryGet(k, out var v) && v.Kind == JsonKind.String ? v.AsString : "";
        static int I(JsonNode n, string k) => n.TryGet(k, out var v) && v.Kind == JsonKind.Number ? (int)v.AsNumber : -1;
        static string IOpt(JsonNode n, string k) => n.TryGet(k, out var v) && v.Kind == JsonKind.Number ? ((int)v.AsNumber).ToString() : "";
    }
}
