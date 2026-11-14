using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace ReverseSolver.Core.Tests
{
    /* PM_66: what the region wand actually does. Random mid-game states of the 40 redesign levels (a random number
       of free pieces removed), every quadrant tried on a replayed copy: how many pieces can leave afterwards that
       could not before. The wand must free at least one piece, or the quadrant cannot be chosen. */
    public class WandFreedTests
    {
        const string RedesignPath = "Assets/_Game/Levels/redesign.json";

        static HashSet<int> Free(GameSession s)
        {
            var set = new HashSet<int>();
            foreach (int p in TravelRule.FreePieces(s.Board)) if (s.NailAt(p) <= 0) set.Add(p);
            return set;
        }

        static GameSession Replay(LevelData level, List<Move> moves)
        {
            var s = new GameSession(level, 1, "test", new Mulberry32(7)) { TestMode = true };
            foreach (var m in moves) s.TryExit(m.Piece, m.Dir);
            return s;
        }

        [Test]
        public void RegionWandFreesAtLeastOnePiece()
        {
            var levels = LevelParser.Parse(File.ReadAllText(RedesignPath)).Levels;
            var rng = new Mulberry32(66);
            int states = 0, eligible = 0, used = 0, withFreed = 0, freedTotal = 0, blocked = 0;
            var histogram = new int[6];
            int wholeUsed = 0, wholeFreed = 0;
            int statesWithWand = 0; var byStage = new int[3, 2];   // removed <20%, 20-40%, 40%+: states, states with an eligible quadrant
            foreach (var level in levels)
            {
                for (int state = 0; state < 6; state++)
                {
                    var moves = new List<Move>();
                    var s = Replay(level, moves);
                    int removals = (int)(rng.NextDouble() * level.Pieces.Count * .6f);
                    for (int i = 0; i < removals && !s.IsOver; i++)
                    {
                        var free = Free(s).ToList();
                        if (free.Count == 0) break;
                        int p = free[(int)(rng.NextDouble() * free.Count)];
                        var m = new Move(p, TravelRule.ExitDirs(s.Board, p)[0]);
                        if (s.TryExit(m.Piece, m.Dir) == CommandResult.Ok) moves.Add(m);
                    }
                    if (s.IsOver) continue;
                    states++;
                    int stage = System.Math.Min(2, (int)(moves.Count * 5.0 / level.Pieces.Count));
                    byStage[stage, 0]++;
                    bool any = false;
                    var before = Free(s);
                    for (int q = 0; q < 4; q++)
                    {
                        var trial = Replay(level, moves);
                        if (!trial.CanWandRegion(q)) { blocked++; continue; }
                        eligible++; any = true;
                        if (trial.UseWandRegion(q) != CommandResult.Ok) continue;
                        used++;
                        int freed = Free(trial).Count(p => !before.Contains(p));
                        Assert.That(trial.WandFreed.Count, Is.EqualTo(freed), "the session reports the pieces it freed");
                        histogram[System.Math.Min(5, freed)]++;
                        freedTotal += freed;
                        if (freed > 0) withFreed++;
                        if (freed > 0 && moves.Count <= 4 && level.Width <= 5)
                            TestContext.WriteLine($"example {level.Id} lv{level.Number} {level.Width}x{level.Height} q{q} moves " +
                                string.Join(" ", moves.Select(m => $"{m.Piece}{m.Dir}@{level.Pieces[m.Piece][0].X},{level.Pieces[m.Piece][0].Y}")) +
                                " freed " + string.Join(",", trial.WandFreed));
                    }
                    if (!any)
                    {
                        // PM_67 (b): the whole board is offered when no quadrant frees a piece
                        var trial = Replay(level, moves);
                        if (trial.CanWandRegion(WandRegion.WholeBoard) && trial.UseWandRegion(WandRegion.WholeBoard) == CommandResult.Ok)
                        {
                            int freed = Free(trial).Count(p => !before.Contains(p));
                            wholeUsed++; wholeFreed += freed; any = true;
                            Assert.That(freed, Is.GreaterThan(0), "the whole-board wand frees a piece too");
                        }
                    }
                    if (any) { statesWithWand++; byStage[stage, 1]++; }
                }
            }
            TestContext.WriteLine($"states {states}, quadrant tries {states * 4}, eligible {eligible}, not eligible {blocked}, used {used}");
            TestContext.WriteLine($"uses freeing >= 1 piece: {withFreed}/{used} ({100.0 * withFreed / System.Math.Max(1, used):0.0}%), mean freed {(double)freedTotal / System.Math.Max(1, used):0.00}");
            TestContext.WriteLine($"freed 0/1/2/3/4/5+: {string.Join("/", histogram)}");
            TestContext.WriteLine($"whole board used {wholeUsed}, mean freed {(double)wholeFreed / System.Math.Max(1, wholeUsed):0.00}");
            TestContext.WriteLine($"states with a usable wand (quadrant or whole board): {statesWithWand}/{states}; by removed share <20% {byStage[0, 1]}/{byStage[0, 0]}, 20-40% {byStage[1, 1]}/{byStage[1, 0]}, 40%+ {byStage[2, 1]}/{byStage[2, 0]}");
            Assert.That(used, Is.GreaterThan(0));
            Assert.That(withFreed, Is.EqualTo(used), "every wand use frees at least one piece");
        }
    }
}
