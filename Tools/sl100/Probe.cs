using System;
using System.Collections.Generic;
using System.Linq;
using ReverseSolver.Core;
using ReverseSolver.Generation;
using ReverseSolver.Editing;
static class Probe
{
    public static void Run()
    {
        var why = new Dictionary<string, int>();
        int ok = 0;
        foreach (var (w, h) in new[] { (7, 7), (8, 8), (6, 6) })
            for (int i = 0; i < 30; i++)
            {
                var spec = SlotSpec.For(40, i % 2 == 0 ? Genre.Zor : Genre.Orta);
                spec.Boards = new[] { (w, h) };
                int cells = w * h;
                spec.MinBodies = cells - spec.MaxChains; spec.MaxBodies = cells - spec.MinChains;
                var p = new GenParams { Interlock = 1 + i % 3 * .5 };
                var c = Generator.TryMake(spec, p, (uint)(77 + i * 131 + w), null, out string reason);
                if (c != null) ok++; else { string k = $"{w}x{h}: " + (reason ?? "?").Split(':')[0]; why[k] = why.GetValueOrDefault(k) + 1; }
            }
        Console.WriteLine("ok " + ok);
        foreach (var kv in why.OrderByDescending(k => k.Value)) Console.WriteLine($"{kv.Value} {kv.Key}");
        Console.WriteLine(SlotSpec.For(40, Genre.Zor));
    }
}
