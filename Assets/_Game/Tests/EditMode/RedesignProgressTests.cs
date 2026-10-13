using NUnit.Framework;
using ReverseSolver.Core;
using ReverseSolver.Presentation;

namespace ReverseSolver.Core.Tests
{
    /* Faz 4 G8 (P1 karar 2): progress per set. Unity-bound (Progress lives with the screens). */
    public class RedesignProgressTests
    {
        /* Progress and locks are kept per level id, so the redesigned set (R..) and the live 40 (L..) never mix. */
        [Test]
        public void ProgressIsSeparatePerSet()
        {
            var log = new AttemptLog();
            var live = TestData.Levels.Levels;
            var s = log.Start(live[0], "test", new Mulberry32(1));
            foreach (var m in live[0].Solution) s.TryExit(m.Piece, m.Dir);
            log.Add(s.Record);
            var liveProgress = new Progress(log, live);
            Assert.That(liveProgress.Won(0), Is.True);
            var redesign = LevelParser.Parse(System.IO.File.ReadAllText("Assets/_Game/Levels/redesign.json")).Levels;
            var newProgress = new Progress(log, redesign);
            Assert.That(newProgress.Won(0), Is.False, "winning L01 does not count for the new set");
            Assert.That(newProgress.Open(1), Is.False, "the new set keeps its own lock");
            Assert.That(newProgress.Next(), Is.EqualTo(0));
        }
    }
}
