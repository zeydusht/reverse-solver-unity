using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using ReverseSolver.Core;
using ReverseSolver.Core.Json;

namespace ReverseSolver.LevelEditor
{
    // Editor-only via ReverseSolver.LevelEditor.asmdef. No authored data is changed.
    public static class HikayeEgrisi
    {
        public static void Run() => Measure("Assets/_Game/Levels/redesign.json", "Logs/hikaye/steps.json");

        // ZeydBot set (Z01..Z40, contains Zeyd's favourite Z12).
        public static void RunZeyd() => Measure("Assets/_Game/Levels/zeydbot.json", "Logs/hikaye/steps_zeyd.json");

        static void Measure(string src, string dst)
        {
            var watch = Stopwatch.StartNew();
            var levels = LevelParser.Parse(File.ReadAllText(src)).Levels;
            var json = new JsonWriter().BeginObject().Key("levels").BeginArray();
            int index = 0, total = 0;
            foreach (var level in levels)
            {
                var path = SmartPlayer.Play(level);
                var state = new GameSession(level, 1, "hikaye", new Mulberry32(1));
                json.BeginObject().Field("id", level.Id).Field("index", ++index)
                    .Field("pieces", level.Pieces.Count).Key("solved").Value(path.Verdict == SmartVerdict.Won)
                    .Key("steps").BeginArray();
                foreach (var move in path.Moves)
                {
                    var removable = new List<int>();
                    var ok = new List<int>();
                    var unk = new List<int>();
                    int nails = 0, chains = 0;
                    foreach (int p in state.Board.PresentPieces())
                    {
                        if (state.NailAt(p) > 0) nails++;
                        if (level.Pieces[p].Length == 2) chains++;
                        // PlayAfter clones state and uses GameSession.TryExit, including nails
                        // and travel locks. Exit direction does not affect the remaining board.
                        foreach (var dir in TravelRule.ExitDirs(state.Board, p))
                        {
                            var result = SmartPlayer.PlayAfter(state, new Move(p, dir), 60000);
                            if (result == null) continue;
                            removable.Add(p);
                            if (result.Verdict == SmartVerdict.Won || result.Verdict == SmartVerdict.Unverified)
                                ok.Add(p);
                            if (result.Verdict == SmartVerdict.Unverified) unk.Add(p);
                            break;
                        }
                    }
                    json.BeginObject();
                    Indices(json, "removable", removable);
                    Indices(json, "ok", ok);
                    Indices(json, "unk", unk);
                    json.Key("bombs").BeginArray();
                    foreach (var bomb in state.Bombs)
                        if (state.Board.IsPresent(bomb.Key))
                            json.BeginObject().Field("p", bomb.Key).Field("f", state.FuseAt(bomb.Key)).EndObject();
                    json.EndArray().Field("nails", nails).Field("chains", chains).Field("move", move.Piece).EndObject();
                    if (state.TryExit(move.Piece, move.Dir) != CommandResult.Ok)
                        throw new InvalidOperationException(level.Id + ": SmartPlayer path refused");
                    total++;
                }
                if ((path.Verdict == SmartVerdict.Won) != (state.Outcome == Outcome.Win))
                    throw new InvalidOperationException(level.Id + ": replay verdict mismatch");
                json.EndArray().EndObject();
            }
            json.EndArray().EndObject();
            Directory.CreateDirectory("Logs/hikaye");
            File.WriteAllText(dst, json + "\n", new UTF8Encoding(false));
            UnityEngine.Debug.Log(string.Format(CultureInfo.InvariantCulture,
                "[hikaye] {0} seviye, {1} adım, sure {2:0.00}s", index, total, watch.Elapsed.TotalSeconds));
        }

        static void Indices(JsonWriter json, string key, List<int> values)
        {
            json.Key(key).BeginArray();
            foreach (int value in values) json.Value(value);
            json.EndArray();
        }
    }
}
