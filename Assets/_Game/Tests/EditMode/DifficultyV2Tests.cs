using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using ReverseSolver.Generation;

namespace ReverseSolver.Core.Tests
{
    /* PM_69: true fuse slack and the fast obstacle-set state behind the v2 difficulty features. */
    public class DifficultyV2Tests
    {
        /* One row of single cells, every joint tabbed: only the end pieces slide out sideways, a piece in the
           middle needs every piece on one side gone first (a tab catches the neighbours if it goes up or down). */
        static LevelData Row(int w, int bomb, int fuse, string nails = "")
        {
            var pieces = string.Join(",", Enumerable.Range(0, w).Select(x => $"[[{x},0]]"));
            var v = "null," + string.Join(",", Enumerable.Repeat("1", w - 1)) + ",null";
            var h = string.Join(",", Enumerable.Repeat("null", w));
            return LevelParser.Parse("{\"format\":1,\"levels\":[{\"id\":\"R\",\"version\":1,\"lv\":30,\"w\":" + w + ",\"h\":1,\"timer\":60," +
                $"\"pieces\":[{pieces}],\"vEdge\":[[{v}]],\"hEdge\":[[{h}],[{h}]],\"bombs\":{{\"{bomb}\":{fuse}}}" +
                (nails.Length > 0 ? $",\"nails\":{{{nails}}}" : "") + "}]}").Levels[0];
        }

        [Test]
        public void SlackTrueIsFuseMinusTheShortestDigToTheBomb()
        {
            // 5 wide, bomb in the middle: two pieces on either side must go first, then the bomb: 3 exits
            var f = DifficultyV2.Compute(Row(5, 2, 5), games: 50, profileGames: 10);
            Assert.That(f.MinExitsAtStart[0], Is.EqualTo(3));
            Assert.That(f.SlackTrue[0], Is.EqualTo(2));
            // 4 wide, bomb second from the left: piece 0 then the bomb
            f = DifficultyV2.Compute(Row(4, 1, 4), games: 50, profileGames: 10);
            Assert.That(f.MinExitsAtStart[0], Is.EqualTo(2));
            Assert.That(f.SlackTrue[0], Is.EqualTo(2));
            Assert.That(f.ChokeN, Is.EqualTo(0));
        }

        [Test]
        public void ANailOnTheShortDigMakesTheOtherSideCheaper()
        {
            // piece 0 nailed for 3 exits: the left dig would take 5 exits, the right one (3, 2, bomb) takes 3
            var f = DifficultyV2.Compute(Row(4, 1, 4, "\"0\":3"), games: 50, profileGames: 10);
            Assert.That(f.MinExitsAtStart[0], Is.EqualTo(3));
            Assert.That(f.SlackTrue[0], Is.EqualTo(1));
            Assert.That(f.ChokeN, Is.EqualTo(1));
            // a fuse of 2 cannot be met at all
            f = DifficultyV2.Compute(Row(4, 1, 2, "\"0\":3"), games: 50, profileGames: 10);
            Assert.That(f.SlackTrue[0], Is.EqualTo(-1));
            Assert.That(f.Rand.PSurvive, Is.EqualTo(0));
        }

        /* The recorded solution is one plan, so the true dig is never longer than the bomb's place in it. */
        [Test]
        public void TrueDigIsNeverLongerThanTheRecordedSolution()
        {
            var levels = LevelParser.Parse(File.ReadAllText("Assets/_Game/Levels/redesign.json")).Levels;
            int bombs = 0, shorter = 0;
            foreach (var level in levels)
            {
                if (level.Bombs.Count == 0 || level.Solution == null || level.Solution.Count == 0) continue;
                var start = new FastState(new Obstacles(level));
                foreach (var kv in level.Bombs)
                {
                    int at = level.Solution.Select(m => m.Piece).ToList().IndexOf(kv.Key) + 1;
                    int min = DifficultyV2.MinExits(start, kv.Key, out var set, out bool approx);
                    Assert.That(min, Is.LessThan(DifficultyV2.Unreachable), $"{level.Id} bomb {kv.Key}");
                    if (!approx) Assert.That(min, Is.LessThanOrEqualTo(at), $"{level.Id} bomb {kv.Key}");
                    Assert.That(set, Does.Contain(kv.Key));
                    bombs++; if (min < at) shorter++;
                }
            }
            TestContext.WriteLine($"bombs {bombs}, true dig shorter than the recorded solution's {shorter}");
            Assert.That(bombs, Is.GreaterThan(20));
        }

        /* The fast state agrees with GameSession move by move on random plays of the 40 levels. */
        [Test]
        public void FastStateMatchesTheSession()
        {
            var levels = LevelParser.Parse(File.ReadAllText("Assets/_Game/Levels/redesign.json")).Levels;
            var rng = new Mulberry32(69);
            foreach (var level in levels)
            {
                var o = new Obstacles(level);
                for (int game = 0; game < 3; game++)
                {
                    var fast = new FastState(o);
                    var s = new GameSession(level, 1, "test", new Mulberry32(1)) { TestMode = true };
                    while (!s.IsOver)
                    {
                        var avail = fast.AvailableList();
                        var real = s.Board.PresentPieces().Where(p => s.NailAt(p) <= 0 && TravelRule.CanExit(s.Board, p)).ToList();
                        Assert.That(avail, Is.EqualTo(real), $"{level.Id} after {s.Moves} exits");
                        if (avail.Count == 0) break;
                        int pick = avail[(int)(rng.NextDouble() * avail.Count)];
                        Assert.That(s.TryExit(pick, TravelRule.ExitDirs(s.Board, pick)[0]), Is.EqualTo(CommandResult.Ok));
                        fast.Remove(pick);
                        if (s.Outcome == Outcome.Bomb) Assert.That(fast.FirstBlow, Is.EqualTo(fast.Exits), level.Id);
                        else Assert.That(fast.FirstBlow, Is.EqualTo(-1), level.Id);
                    }
                }
            }
        }
    }
}
