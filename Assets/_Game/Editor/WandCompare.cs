using System.IO;
using System.Collections.Generic;
using ReverseSolver.Core;
using ReverseSolver.Generation;
using ReverseSolver.Editing;
using UnityEditor;
using UnityEngine;

namespace ReverseSolver.EditorTools
{
    public static class WandCompare
    {
        [MenuItem("Reverse Solver/Compare whole-board and region wand")]
        public static void Run()
        {
            // Controlled intervention: one wand at the start of every attempt, same players/seeds.
            // No automatic runtime policy is claimed; rejected region uses are included in the rates.
            string path = "Assets/_Game/Levels/redesign.json";
            foreach (string arg in System.Environment.GetCommandLineArgs())
                if (arg.StartsWith("-wandLevels=")) path = arg.Substring(12);
            var levels = LevelParser.Parse(File.ReadAllText(path)).Levels;
            var plan = FactoryPlan.All();
            var population = FunSim.Population();
            Debug.Log("[wand-compare] Start-of-attempt use; 1000 paired players, 5 seeds, 3 attempts; random eligible quadrant. Geometric solvability only, as legacy wand.");
            for (int index = 0; index < levels.Count; index++)
            {
                var level = levels[index];
                var genre = index < plan.Count ? plan[index].Genre : Genre.Orta;
                for (int variant = 0; variant < 2; variant++)
                {
                    double first = 0, fun = 0, frustration = 0, by3 = 0;
                    int uses = 0, offered = 0;
                    for (uint seed = 1; seed <= 5; seed++)
                    {
                        var pick = new Mulberry32(seed);
                        var options = index < plan.Count ? FunSim.OptionsFor(plan[index]) : new FunSim.Options();
                        options.BoosterSeed = seed;
                        options.PrepareAttempt = session =>
                        {
                            offered++; session.TestBoosterUnlock = Booster.Wand; session.SetStock(Booster.Wand, 1);
                            CommandResult result = CommandResult.NoArrangement;
                            if (variant == 0) result = session.UseWand();
                            else
                            {
                                var eligible = new List<int>();
                                for (int i = 0; i < 4; i++)
                                    if (session.CanWandRegion(i)) eligible.Add(i);
                                if (eligible.Count > 0) result = session.UseWandRegion(eligible[(int)(pick.NextDouble() * eligible.Count)]);
                            }
                            if (result == CommandResult.Ok) uses++;
                        };
                        var r = FunSim.Run(level, genre, population, seed: seed, options: options);
                        first += r.FirstWinRate; fun += r.EnjoyRate; frustration += r.FrustrationRate; by3 += r.WinBy3Rate;
                    }
                    Debug.Log($"[wand-compare] {level.Id} {(variant == 0 ? "whole" : "region")} first-win={first * 20:0.0}% fun={fun * 20:0.0}% frustrated={frustration * 20:0.0}% win-by3={by3 * 20:0.0}% applied={uses}/{offered}");
                }
            }
        }
    }
}
