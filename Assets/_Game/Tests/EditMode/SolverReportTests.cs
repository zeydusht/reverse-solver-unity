using System.Linq;
using System.Text;
using NUnit.Framework;

namespace ReverseSolver.Core.Tests
{
    /* Reports, not gates (PRODUCT.md, M2 criteria 1b and 8). They always pass
       and print their findings; the numbers go to the PM. */
    public class SolverReportTests
    {
        [Test]
        public void SmartPlayerOnAllLevels()
        {
            var sb = new StringBuilder("Smart player (nail-aware, shortest path to bombs, no boosters):\n");
            int won = 0, valves = 0;
            foreach (var l in TestData.Levels.Levels)
            {
                var r = SmartPlayer.Play(l);
                if (r.Verdict == SmartVerdict.Won) won++;
                valves += r.Valves;
                sb.AppendLine("  " + r);

                // Whatever the verdict, every planned move was legal (Play throws otherwise)
                // and a win really cleared the board.
                if (r.Verdict == SmartVerdict.Won) Assert.That(r.Outcome, Is.EqualTo(Outcome.Win), l.Id);
            }
            sb.AppendLine($"  won {won}/40, safety valve pulls {valves}");
            TestContext.WriteLine(sb.ToString());
            UnityEngine.Debug.Log(sb.ToString());
            Assert.Pass($"smart player won {won}/40, valves {valves}");
        }

        [Test]
        public void WandKeepsBoardsSolvable()
        {
            var sb = new StringBuilder("Wand on every level where it is unlocked and stocked, seeds 1-5:\n");
            int tries = 0, applied = 0, solvable = 0;
            foreach (var l in TestData.Levels.Levels)
            {
                var probe = new GameSession(l, 1, "report", new Mulberry32(1));
                if (!probe.IsUnlocked(Booster.Wand) || probe.Stock(Booster.Wand) == 0) continue;
                int ok = 0;
                for (uint seed = 1; seed <= 5; seed++)
                {
                    var s = new GameSession(l, 1, "report", new Mulberry32(seed));
                    tries++;
                    if (s.UseWand() != CommandResult.Ok) continue;
                    applied++; ok++;
                    if (GreedySolver.Solve(s.Board).Solved) solvable++;
                }
                sb.AppendLine($"  {l.Id}: wand applied {ok}/5");
            }
            sb.AppendLine($"  applied {applied}/{tries}; greedy solves {solvable}/{applied} of the reshuffled boards");
            TestContext.WriteLine(sb.ToString());
            UnityEngine.Debug.Log(sb.ToString());
            Assert.Pass($"wand applied {applied}/{tries}, solvable after {solvable}/{applied}");
        }
    }
}
