using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ReverseSolver.Core;
using ReverseSolver.Core.Json;
using ReverseSolver.Editing;
using ReverseSolver.Generation;

namespace ReverseSolver.LevelEditor
{
    public static class AjanOlcum
    {
        public static void Run()
        {
            string src = AjanJson.Arg("-rsFile"), dst = AjanJson.Arg("-rsOut");
            var root = JsonReader.Parse(File.ReadAllText(src));
            var levels = LevelParser.Parse(File.ReadAllText(src)).Levels;
            var json = new JsonWriter().BeginObject().Key("levels").BeginArray();
            Directory.CreateDirectory("Logs/ajan");
            int index = 0;
            var cl = Environment.GetCommandLineArgs(); int zi = Array.IndexOf(cl, "-rsZbRuns"); int zbRuns = zi >= 0 && zi + 1 < cl.Length && int.TryParse(cl[zi + 1], out int zr) ? zr : 12;
            foreach (var level in levels)
            {
                var raw = AjanJson.Get(root, "levels")[index];
                var genre = raw.TryGet("genre", out var g) ? AjanJson.GenreOf(g.AsString) : Genre.Orta;
                var sw = System.Diagnostics.Stopwatch.StartNew(); UnityEngine.Debug.Log("[ajan-olcum] basla " + level.Id); var mc = MonteCarlo.Run(level, 1000); double tMc = sw.Elapsed.TotalSeconds; UnityEngine.Debug.Log($"[ajan-olcum] mc bitti {tMc:0.0}s");
                // an obstacle card that introduces the bomb draws attention (FunSim V3, as the factory does for the bomb slot)
                bool bombIntro = level.Intro != null && level.Intro.Icon == "bomb";
                var fun = FunSim.Run(level, genre, options: new FunSim.Options { BombIntro = bombIntro }); double tFun = sw.Elapsed.TotalSeconds; UnityEngine.Debug.Log($"[ajan-olcum] fun bitti {tFun:0.0}s");
                var sp = SmartPlayer.Play(level);
                var earliest = level.Pieces.Count <= 30 ? AjanUret.Earliest(level, level.Bombs.Keys.ToList()) : Enumerable.Repeat(-1, level.Pieces.Count).ToArray();   // exact search is too slow on big boards
                var s = new GameSession(level, 1, "aware", new Mulberry32(1));
                var bombExits = new JsonWriter().BeginArray();
                foreach (var move in sp.Moves)
                {
                    if (level.Bombs.ContainsKey(move.Piece))
                        bombExits.BeginObject().Field("p", move.Piece).Field("step", s.Moves + 1)
                            .Field("earliest", earliest[move.Piece]).Field("fuse_margin", s.FuseAt(move.Piece)).EndObject();
                    s.TryExit(move.Piece, move.Dir);
                }
                bombExits.EndArray();
                var forget = new GameSession(level, 1, "forget", new Mulberry32(1));
                while (!forget.IsOver)
                {
                    var options = TravelRule.FreePieces(forget.Board).Where(p => forget.NailAt(p) <= 0)
                        .OrderBy(p => level.Bombs.ContainsKey(p) ? 1 : 0).ThenBy(p => p).ToList();
                    if (options.Count == 0) break;
                    int p = options[0];
                    if (forget.TryExit(p, TravelRule.ExitDirs(forget.Board, p)[0]) != CommandResult.Ok) break;
                }
                var bots = new JsonWriter().BeginObject();
                bots.Key("bot_rastgele_bombasiz").BeginObject().Field("runs", 1000)
                    .Key("win_rate").Raw(AjanJson.Number(mc.WinRate)).Key("bomb_rate").Raw(AjanJson.Number(mc.BombRate))
                    .Key("frozen_rate").Raw(AjanJson.Number(mc.FrozenRate)).EndObject();
                bots.Key("bot_bomba_unutan").BeginObject().Field("outcome", forget.Outcome.ToString())
                    .Field("moves", forget.Moves).Key("win_rate").Raw(forget.Outcome == Outcome.Win ? "1" : "0").EndObject();
                bots.Key("bot_bilincli").BeginObject().Key("won").Value(sp.Verdict == SmartVerdict.Won)
                    .Field("outcome", sp.Verdict.ToString()).Field("moves", sp.Moves.Count).Key("bomb_exits").Raw(bombExits.ToString()).EndObject();
                bots.Key("funsim").BeginObject().Field("runs", fun.Players).Key("win_rate").Raw(AjanJson.Number(fun.FirstWinRate))
                    .Key("near_miss").Raw(AjanJson.Number(fun.NearMissRate));
                Distribution(bots, "time_left", fun.FirstWinTimeLeft);
                Distribution(bots, "loss_piece_share", fun.FirstLossPieceShare);
                bots.EndObject().EndObject();
                HikayeEgrisi.WriteLevel(json, level, ++index, bots.ToString());
                var csv = new StringBuilder("level_id,run,adim,kalan_parca,secenek,dogru,en_dar_pay,koruyan,koruyan_secildi,yaniltici,dig\n");
                double tPre = sw.Elapsed.TotalSeconds;
                ZeydBot.PathRows(level, zbRuns, .95, csv);
                UnityEngine.Debug.Log($"[ajan-olcum] {level.Id} mc {tMc:0.0}s fun {tFun - tMc:0.0}s rest {tPre - tFun:0.0}s zb {sw.Elapsed.TotalSeconds - tPre:0.0}s");
                string safeId = string.Concat(level.Id.Select(ch => char.IsLetterOrDigit(ch) || ch == '_' || ch == '-' ? ch : '_'));
                File.WriteAllText("Logs/ajan/paths_" + safeId + ".csv", csv.ToString(), new UTF8Encoding(false));
            }
            json.EndArray().EndObject();
            AjanJson.Save(dst, json.ToString());
            UnityEngine.Debug.Log($"[ajan-olcum] {index} bölüm → {dst}");
        }

        static void Distribution(JsonWriter w, string key, List<double> values)
        {
            values.Sort();
            double Q(double p)
            {
                if (values.Count == 0) return double.NaN;
                double x = (values.Count - 1) * p; int i = (int)x;
                return values[i] + (values[Math.Min(i + 1, values.Count - 1)] - values[i]) * (x - i);
            }
            w.Key(key).BeginObject().Field("n", values.Count).Key("median").Raw(AjanJson.Number(Q(.5)))
                .Key("p25").Raw(AjanJson.Number(Q(.25))).Key("p75").Raw(AjanJson.Number(Q(.75))).EndObject();
        }
    }
}
