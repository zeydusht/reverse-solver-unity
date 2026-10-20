using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ReverseSolver.Core;
using ReverseSolver.Editing;
using UnityEngine;

namespace ReverseSolver.LevelEditor
{
    /* ZeydBot v1, the engine half (overnight 2026-10-20). Per level, `runs` playthroughs where every move is a
       RIGHT option picked at random (SmartPlayer still wins clean after it); each step records what the
       player faces: pieces left, options, right ones, tightest bomb slack. Design only: no play result is
       read, so the test levels can go through it too. The person half (when Zeyd picks a trap, how long he
       thinks) is Tools/zeydbot_model.py, which plays these steps as a Monte Carlo.

         bash Tools/unity_batch.sh method ReverseSolver.LevelEditor.ZeydBot.Paths [-rsRuns 12] [-rsDig 0.95] [-rsFile path] [-rsOut csv] */
    public static class ZeydBot
    {
        static void Log(string s) => Debug.Log("[zeydbot] " + s);

        public static void Paths()
        {
            int runs = int.Parse(ZeydSet.Arg("-rsRuns", "12"), CultureInfo.InvariantCulture);
            double dig = double.Parse(ZeydSet.Arg("-rsDig", "0.95"), CultureInfo.InvariantCulture);
            string file = ZeydSet.Arg("-rsFile", ZeydSet.Path);
            string outCsv = ZeydSet.Arg("-rsOut", ZeydAnalysis.Out + "/yollar.csv");
            var set = LevelParser.Parse(File.ReadAllText(file));
            var sb = new StringBuilder("level_id,run,adim,kalan_parca,secenek,dogru,en_dar_pay\n");
            foreach (var l in set.Levels)
            {
                var t0 = DateTime.Now;
                for (int run = 0; run < runs; run++)
                {
                    var rng = new System.Random(1000 * run + l.Id.Sum(ch => ch) * 7);
                    var s = new GameSession(l, 1, "bot", new Mulberry32(1)) { ClockOnFirstMove = false };
                    var lng = ZeydAnalysis.Long(l);
                    for (int step = 0; !s.IsOver && step < 200; step++)
                    {
                        var o = ZeydAnalysis.Options(s, 30_000);
                        var sl = ZeydAnalysis.Slacks(s, lng);
                        var right = o.Where(x => x.right).Select(x => x.piece).ToList();
                        sb.Append($"{l.Id},{run},{step},{s.Board.PresentCount},{o.Count},{right.Count},{(sl.Count == 0 ? "" : sl.Min(x => x.slack).ToString())}\n");
                        if (right.Count == 0) break;   // budget miss: SmartPlayer could not confirm a right move
                        // Zeyd digs for the bomb: with a bomb on the board he kept the tightest slack on 95 % of his
                        // first-attempt moves (hamleler.csv, 359 / 377); the bot does the same with -rsDig
                        if (sl.Count > 0 && rng.NextDouble() < dig)
                        {
                            int now = sl.Min(x => x.slack);
                            var keep = right.Where(p0 =>
                            {
                                var c = StructureMetrics.Copy(s); var cl = StructureMetrics.Copy(lng);
                                var d0 = TravelRule.ExitDirs(c.Board, p0)[0];
                                c.TryExit(p0, d0); cl.TryExit(p0, d0);
                                if (c.IsOver) return c.Outcome == Outcome.Win;
                                var after = ZeydAnalysis.Slacks(c, cl);
                                return after.Count == 0 || after.Min(x => x.slack) >= now;
                            }).ToList();
                            if (keep.Count > 0) right = keep;
                        }
                        int p = right[rng.Next(right.Count)];
                        var dir = TravelRule.ExitDirs(s.Board, p).First(d => { var r = SmartPlayer.PlayAfter(s, new Move(p, d), 30_000); return r != null && r.Verdict == SmartVerdict.Won && r.Valves == 0; });
                        s.TryExit(p, dir); lng.TryExit(p, dir);
                    }
                }
                Log($"{l.Id}: {runs} yol, {(DateTime.Now - t0).TotalSeconds:0} s");
                File.WriteAllText(outCsv, sb.ToString(), new UTF8Encoding(false));
            }
            Log("yazıldı: " + outCsv);
        }
    }
}
