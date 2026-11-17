using System;
using System.IO;
using System.Linq;
using ReverseSolver.Core;
static class Probe2
{
    public static void Run()
    {
        var set = LevelParser.Parse(File.ReadAllText("Assets/_Game/Levels/sl100.json"));
        var l = set.Levels[0];
        Console.WriteLine($"{l.Id} {l.Width}x{l.Height}");
        for (int p = 0; p < l.Pieces.Count; p++) Console.WriteLine($"piece {p}: {string.Join(" ", l.Pieces[p].Select(c => $"({c.X},{c.Y})"))}");
        var s = new GameSession(l, 1, "probe", new Mulberry32(1));
        foreach (var (piece, dir) in new[] { (6, Dir.D), (8, Dir.R), (8, Dir.D) })
        {
            var pr = s.Probe(piece, dir);
            Console.WriteLine($"{piece} {dir}: exit={pr.Exit}");
            if (pr.Exit) { var b = s.Board; Console.WriteLine("  exits " + string.Join(",", TravelRule.ExitDirs(b, piece))); }
            if (pr.Exit) s.Board.Remove(piece);
        }
    }
}
