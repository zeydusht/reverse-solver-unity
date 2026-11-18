using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ReverseSolver.Core;
using ReverseSolver.Generation;
static class Program
{
    static void Main(string[] a)
    {
        Directory.SetCurrentDirectory("C:/Users/zeydt/Downloads/Reverse Solver/ReverseSolver");
        if (a.Length > 0 && a[0] == "design") { Design.Run(a); return; }
        if (a.Length > 0 && a[0] == "probe") { Probe.Run(); return; }
        if (a.Length > 0 && a[0] == "economy") { Economy.Run(a); return; }
        if (a.Length > 0 && a[0] == "probe2") { Probe2.Run(); return; }
        var set = P21Onboarding.P22(LevelParser.Parse(File.ReadAllText("Assets/_Game/Levels/redesign.json")));
        var sw = Stopwatch.StartNew();
        foreach (var l in set.Levels.Take(40))
        {
            sw.Restart();
            var f = DifficultyV2.Compute(l, 600, 150);
            double ms = sw.Elapsed.TotalMilliseconds;
            Console.WriteLine($"{l.Id} {l.Width}x{l.Height} n{l.Pieces.Count} T{l.TimerSeconds} p={DifficultyModelV2.PWin1(f):0.00} blend={DifficultyModelV2.PWin1Blended(f, PlayData.Pt1):0.00} t(.8)={DifficultyModelV2.TimerFor(f, .8)} {ms:0}ms");
        }
    }
}
