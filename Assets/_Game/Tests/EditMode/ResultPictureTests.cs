using NUnit.Framework;
using ReverseSolver.Editing;
using ReverseSolver.Presentation;
using UnityEngine;

namespace ReverseSolver.Core.Tests
{
    /* G1-b (PRODUCT.md 2026-10-13): "Resmi tamamladın" only for a won design
       whose picture arrived; everything else keeps the old result card. */
    public class ResultPictureTests
    {
        static GameSession Won(string image)
        {
            var d = LevelDraft.New(3, 3);
            d.Image = image;
            var l = d.ToLevelData();
            var s = TestData.WebClock(new GameSession(l, 1, "test", new Mulberry32(1)));
            foreach (var m in DesignCheck.Run(l).Solution) s.TryExit(m.Piece, m.Dir);
            Assert.That(s.Outcome, Is.EqualTo(Outcome.Win));
            return s;
        }

        [Test]
        public void WonDesignWithItsPictureShowsIt()
        {
            var pic = new Texture2D(4, 4);
            try { Assert.That(Screens.ShowsPicture(Won("D01.jpg"), pic), Is.True); }
            finally { Object.DestroyImmediate(pic); }
        }

        [Test]
        public void NoPictureOrNotLoadedKeepsTheOldCard()
        {
            var pic = new Texture2D(4, 4);
            try
            {
                Assert.That(Screens.ShowsPicture(Won("D01.jpg"), null), Is.False, "picture did not arrive (404, offline)");
                Assert.That(Screens.ShowsPicture(Won(null), pic), Is.False, "level without a picture");
                var lostDraft = LevelDraft.New(3, 3);
                lostDraft.Image = "D01.jpg";
                var lost = TestData.WebClock(new GameSession(lostDraft.ToLevelData(), 1, "test", new Mulberry32(1)));
                lost.Tick(999);
                Assert.That(lost.Outcome, Is.EqualTo(Outcome.Time));
                Assert.That(Screens.ShowsPicture(lost, pic), Is.False, "time ran out");
                foreach (var l in TestData.Levels.Levels) Assert.That(l.Image, Is.Null, l.Id + ": live levels have no picture");
            }
            finally { Object.DestroyImmediate(pic); }
        }
    }
}
