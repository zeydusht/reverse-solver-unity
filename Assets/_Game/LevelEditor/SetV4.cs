using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ReverseSolver.Core;
using ReverseSolver.Core.Json;
using ReverseSolver.Editing;
using ReverseSolver.Generation;
using UnityEngine;

namespace ReverseSolver.LevelEditor
{
    /* SET40 v4 (PM_MESAJ_23): measures the fuse / extra-bomb variants of the 9 retuned levels. For every level in
       -rsFile (levels.json format, Tools/set40_v4.py writes it): SmartPlayer verdict and valves, FunSim first-try
       win (Genre.Orta, as SHOW1) and the random player's win rate. Winners are written again with SmartPlayer's
       solution to -rsOut (.json) beside a .csv of the numbers.

         bash Tools/unity_batch.sh method ReverseSolver.LevelEditor.SetV4.Measure -rsFile Logs/setv4/var.json -rsOut Logs/setv4/olcum */
    public static class SetV4
    {
        static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
        static void Log(string s) => Debug.Log("[setv4] " + s);

        public static void Measure()
        {
            string file = ZeydSet.Arg("-rsFile", "Logs/setv4/var.json");
            string outp = ZeydSet.Arg("-rsOut", "Logs/setv4/olcum");
            var set = LevelParser.Parse(File.ReadAllText(file));
            var rows = new Dictionary<string, string>();
            var json = new Dictionary<string, string>();
            var gate = new object();
            System.Threading.Tasks.Parallel.ForEach(set.Levels, new System.Threading.Tasks.ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 1) }, l =>
            {
                string row, js = null;
                try
                {
                    var sp = SmartPlayer.Play(l);
                    if (sp.Verdict != SmartVerdict.Won) row = $"{l.Id},{sp.Verdict},{sp.Valves},,";
                    else
                    {
                        var fun = FunSim.Run(l, Genre.Orta);
                        var mc = MonteCarlo.Run(l, 200);
                        row = string.Format(Inv, "{0},Won,{1},{2:0.000},{3:0.000}", l.Id, sp.Valves, fun.FirstWinRate, mc.WinRate);
                        var d = LevelDraft.FromLevel(l);
                        d.Solution = sp.Moves.ToList();
                        var jw = new JsonWriter();
                        d.WriteJson(jw);
                        js = jw.ToString();
                    }
                }
                catch (Exception e) { row = $"{l.Id},HATA {e.Message.Replace(',', ' ')},,,"; }
                lock (gate) { rows[l.Id] = row; if (js != null) json[l.Id] = js; }
            });
            File.WriteAllText(outp + ".csv", "id,verdict,valves,first,mc\n" + string.Concat(set.Levels.Select(l => rows[l.Id] + "\n")), new UTF8Encoding(false));
            File.WriteAllText(outp + ".json", "{\n  \"format\": 1,\n  \"levels\": [\n    " +
                string.Join(",\n    ", set.Levels.Where(l => json.ContainsKey(l.Id)).Select(l => json[l.Id])) + "\n  ]\n}\n", new UTF8Encoding(false));
            Log($"ölçüldü: {set.Levels.Count} bölüm, kazanılan {json.Count} → {outp}.csv");
        }

        /* PM_MESAJ_25 check (?c=ic): a move list that loses -rsLevel (default S45) to its bomb: free pieces other than
           the bombs, taken in board order, until the fuse runs out. Written to Logs/veri/bomb_loss.json for verify_build.

             bash Tools/unity_batch.sh method ReverseSolver.LevelEditor.SetV4.BombLoss [-rsLevel S45] */
        public static void BombLoss()
        {
            string id = ZeydSet.Arg("-rsLevel", "S45");
            var l = LevelParser.Parse(File.ReadAllText("Assets/_Game/Levels/redesign.json")).Levels.First(x => x.Id == id);
            var s = new GameSession(l, 1, "bombloss", new Mulberry32(1));
            var moves = new List<string>();
            while (!s.IsOver)
            {
                bool moved = false;
                for (int p = 0; p < l.Pieces.Count && !moved; p++)
                {
                    if (l.Bombs.ContainsKey(p)) continue;
                    foreach (Dir d in new[] { Dir.U, Dir.D, Dir.L, Dir.R })
                        if (s.TryExit(p, d) == CommandResult.Ok) { moves.Add($"{{\"piece\":{p},\"dir\":\"{d}\"}}"); moved = true; break; }
                }
                if (!moved) break;
            }
            Directory.CreateDirectory("Logs/veri");
            File.WriteAllText("Logs/veri/bomb_loss.json", $"{{\"id\":\"{id}\",\"outcome\":\"{s.Outcome}\",\"solution\":[{string.Join(",", moves)}]}}\n", new UTF8Encoding(false));
            Log($"{id}: {moves.Count} hamle → {s.Outcome}");
        }
    }
}
