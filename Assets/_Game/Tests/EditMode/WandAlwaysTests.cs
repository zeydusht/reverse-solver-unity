using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace ReverseSolver.Core.Tests
{
    /* PM_75: "the wand can't free a piece yet" on the test link. Mid-game states of the 40 redesign levels (0-90 %
       of the pieces removed): is some wand option offered (a quadrant, else the whole board), before PM_75 (the
       PM_66 sampling alone) and now (plus the targeted search)? Where nothing is offered, is it really impossible
       (WandCanHelp false: the pieces are still packed, so no tab change can free one)? */
    public class WandAlwaysTests
    {
        const string RedesignPath = "Assets/_Game/Levels/redesign.json";
        static readonly double[] Stages = { 0, .1, .2, .3, .45, .6, .75, .9 };

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

        static bool Offered(LevelData level, List<Move> moves)
        {
            var s = Replay(level, moves);
            for (int q = 0; q < 4; q++) if (s.CanWandRegion(q)) return true;
            return s.CanWandRegion(WandRegion.WholeBoard);
        }

        [Test]
        public void WandIsOfferedWheneverATabChangeCanFreeAPiece()
        {
            var levels = LevelParser.Parse(File.ReadAllText(RedesignPath)).Levels;
            int states = 0, before = 0, now = 0, possible = 0, missed = 0;
            var stageStates = new int[Stages.Length]; var stageBefore = new int[Stages.Length]; var stageNow = new int[Stages.Length];
            var stagePossible = new int[Stages.Length];
            var swHelp = new Stopwatch(); var swOffer = new Stopwatch(); int helpCalls = 0, offerCalls = 0;
            var lines = new List<string>();
            try
            {
                foreach (var level in levels)
                {
                    var firstNow = new List<string>();
                    for (int si = 0; si < Stages.Length; si++)
                        for (uint seed = 1; seed <= 2; seed++)
                        {
                            var rng = new Mulberry32(seed * 977 + (uint)level.Number);
                            var moves = new List<Move>();
                            var s = Replay(level, moves);
                            int removals = (int)(Stages[si] * level.Pieces.Count);
                            for (int i = 0; i < removals && !s.IsOver; i++)
                            {
                                var free = Free(s).ToList();
                                if (free.Count == 0) break;
                                int p = free[(int)(rng.NextDouble() * free.Count)];
                                var m = new Move(p, TravelRule.ExitDirs(s.Board, p)[0]);
                                if (s.TryExit(m.Piece, m.Dir) == CommandResult.Ok) moves.Add(m);
                            }
                            if (s.IsOver || s.Board.PresentCount < 2) continue;
                            states++; stageStates[si]++;
                            GameSession.TargetedSearch = false;
                            bool old = Offered(level, moves);
                            GameSession.TargetedSearch = true;
                            swOffer.Start(); bool offered = Offered(level, moves); swOffer.Stop(); offerCalls++;
                            swHelp.Start(); bool canHelp = Replay(level, moves).WandCanHelp(); swHelp.Stop(); helpCalls++;
                            if (old) { before++; stageBefore[si]++; }
                            if (offered) { now++; stageNow[si]++; }
                            if (canHelp) { possible++; stagePossible[si]++; }
                            if (canHelp && !offered) { missed++; lines.Add($"missed {level.Id} lv{level.Number} stage {Stages[si]:0.00} seed {seed} present {s.Board.PresentCount}"); }
                            Assert.That(!old || offered, $"{level.Id}: the targeted search never takes an option away");
                            Assert.That(!offered || canHelp, $"{level.Id}: an offered wand implies a tab change can free a piece");
                        }
                }
            }
            finally { GameSession.TargetedSearch = true; }
            TestContext.WriteLine($"[wand75] states {states}: offered before {before} ({100.0 * before / states:0.0}%), now {now} ({100.0 * now / states:0.0}%), " +
                                  $"possible at all {possible} ({100.0 * possible / states:0.0}%), possible but not offered {missed}");
            for (int si = 0; si < Stages.Length; si++)
                TestContext.WriteLine($"[wand75] removed {Stages[si] * 100:0}%: states {stageStates[si]}, before {stageBefore[si]}, now {stageNow[si]}, possible {stagePossible[si]}");
            TestContext.WriteLine($"[wand75] WandCanHelp {swHelp.Elapsed.TotalMilliseconds / helpCalls:0.00} ms/call, offer check (5 regions) {swOffer.Elapsed.TotalMilliseconds / offerCalls:0.0} ms/state");
            foreach (var l in lines.Take(20)) TestContext.WriteLine("[wand75] " + l);
            Assert.That(missed, Is.LessThanOrEqualTo(states / 50), "a possible wand is offered (≥ 98 %)");
        }
    }
}
