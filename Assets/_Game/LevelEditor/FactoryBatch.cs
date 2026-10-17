using System;
using System.Linq;
using ReverseSolver.Generation;
using UnityEngine;

namespace ReverseSolver.LevelEditor
{
    /* Batchmode twin of the Fabrika tab (lost in G7, back 2026-10-17). Lines tagged [fabrika]
       are what Tools/unity_batch.sh shows; the full log stays in Logs/batch.

         bash Tools/unity_batch.sh method ReverseSolver.LevelEditor.FactoryBatch.Run -rsSlots 1-40 -rsDepth 1 [-rsRun name]
         bash Tools/unity_batch.sh method ReverseSolver.LevelEditor.FactoryBatch.Rescore [-rsRun name]
         bash Tools/unity_batch.sh method ReverseSolver.LevelEditor.FactoryBatch.Rounds -rsSlots 10,28,34 [-rsShort]
              [-rsDepth 2] [-rsRounds 3] [-rsSeed 2] [-rsRelax] [-rsRun name]

       Rounds: -rsShort adds every slot left with fewer than two finalists; -rsRelax adds the
       relaxed round after the others (PRODUCT.md: only after three rounds). */
    public static class FactoryBatch
    {
        static readonly string[] Args = Environment.GetCommandLineArgs();
        static string Arg(string k, string d) { int i = Array.IndexOf(Args, k); return i >= 0 && i + 1 < Args.Length ? Args[i + 1] : d; }
        static bool Flag(string k) => Array.IndexOf(Args, k) >= 0;
        static string RunName(string prefix) => Arg("-rsRun", prefix + DateTime.Now.ToString("yyyyMMdd-HHmm"));
        static void Log(string line) { foreach (var l in line.Split('\n')) Debug.Log("[fabrika] " + l.TrimEnd()); }

        public static void Run()
        {
            var slots = FactoryRunner.ParseSlots(Arg("-rsSlots", "1-40"));
            int depth = int.Parse(Arg("-rsDepth", "1"));
            string run = RunName("");
            Log($"koşu {run}: slotlar {string.Join(",", slots)}, derinlik {depth}");
            FactoryRunner.Run(slots, FactoryWindow.OptionsFor(depth), run, (done, total, line) => { Log($"{done}/{total} {line}"); return true; });
            Log("bitti, rapor " + FactoryReport.Path);
        }

        public static void Rescore()
        {
            string run = RunName("rescore-");
            var text = FactoryRunner.Rescore(run);
            foreach (var l in text.Split('\n').Where(l => !l.StartsWith("|"))) Log(l);
            Log($"tablo: {FactoryRunner.RunsDir}/{run}/rescore.md");
        }

        public static void Rounds()
        {
            var slots = FactoryRunner.ParseSlots(Arg("-rsSlots", "10,28,34"));
            if (Flag("-rsShort"))
                foreach (var s in FactoryRunner.ShortSlots(RedesignStore.LoadOrNew())) if (!slots.Contains(s)) slots.Add(s);
            slots.Sort();
            var o = FactoryWindow.OptionsFor(int.Parse(Arg("-rsDepth", "2")));
            FactoryRunner.RunRounds(slots, o, RunName("tur-"), int.Parse(Arg("-rsRounds", "3")), uint.Parse(Arg("-rsSeed", "2")), Flag("-rsRelax"), Log);
            Log("bitti, rapor " + FactoryReport.Path);
        }
    }
}
