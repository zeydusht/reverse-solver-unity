using System.IO;
using NUnit.Framework;

namespace ReverseSolver.Core.Tests
{
    /* PM_84 §1: the hint shows a piece that leaves now and never spoils the rest (with a bomb: the bomb first); the
       live config carries ipucu_son_bolum (3). */
    public class HintPickerTests
    {
        static LevelSet Set => LevelParser.Parse(File.ReadAllText("Assets/_Game/Levels/sl100.json"));

        [Test]
        public void FollowingOnlyTheHintClearsEveryLevel()
        {
            foreach (var l in Set.Levels)
            {
                var s = new GameSession(l, 1, "hint", new Mulberry32(1));
                int guard = 0;
                while (!s.IsOver && guard++ < 400)
                {
                    var m = HintPicker.Next(s);
                    Assert.That(m.HasValue, $"{l.Number}: a hint with {s.Board.PresentCount} pieces left");
                    Assert.AreEqual(CommandResult.Ok, s.TryExit(m.Value.Piece, m.Value.Dir), $"{l.Number}: the hinted {m} leaves");
                }
                Assert.AreEqual(Outcome.Win, s.Outcome, $"{l.Number}: the hints alone win");
            }
        }

        [Test]
        public void NoHintWhenOver()
        {
            var l = Set.Levels[0];
            var s = new GameSession(l, 1, "hint", new Mulberry32(1));
            foreach (var m in l.Solution) s.TryExit(m.Piece, m.Dir);
            Assert.AreEqual(Outcome.Win, s.Outcome);
            Assert.IsNull(HintPicker.Next(s));
        }

        [Test]
        public void TheLastHintLevelIsInTheLiveConfig()
        {
            Assert.AreEqual(3, LiveConfig.Default.HintLastLevel);
            StringAssert.Contains("\"ipucu_son_bolum\":3", LiveConfig.Default.ToJson().Replace(" ", ""));
            Assert.AreEqual(5, LiveConfig.Parse("{\"ipucu_son_bolum\":5}").HintLastLevel);
        }
    }
}
