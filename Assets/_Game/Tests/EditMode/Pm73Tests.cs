using NUnit.Framework;
using ReverseSolver.Presentation;

namespace ReverseSolver.Core.Tests
{
    /* PM_73: the agreed numbers of the win entrance, the map buttons, the exit trail and the breath. */
    public class Pm73Tests
    {
        [Test]
        public void WinEntranceIsShort()
        {
            // PM_96: the entrance follows the fanfare's beats; the chosen one's last beat ends it
            var beats = BeatMap.Of(Music.FanfareKey).beats;   // PM_99: the default is the 5-beat cut
            Assert.That(beats[beats.Length - 1], Is.LessThanOrEqualTo(2.0f));
            Assert.That(BeatMap.Fallback.beats[BeatMap.Fallback.beats.Length - 1], Is.LessThanOrEqualTo(1.2f));
        }

        [Test]
        public void MapButtonsReadOnAPhone()
        {
            Assert.That(LevelsMap.BadgeSize, Is.GreaterThanOrEqualTo(22));
            Assert.That(LevelsMap.CurrentSize / LevelsMap.NodeSize, Is.EqualTo(1.25f).Within(.02f));
            Assert.That(LevelsMap.MarkerShare, Is.EqualTo(.7f).Within(.05f));
        }

        [Test]
        public void BreathOnlySlowsImportantExits()
        {
            Assert.That(BoardView.BreathSpeed, Is.EqualTo(.2f));
            Assert.That(BoardView.BreathSeconds, Is.EqualTo(.09f).Within(.02f));
            Assert.That(BoardView.LastBreathSeconds, Is.EqualTo(.16f).Within(.02f));
            Assert.That(BoardView.LightBreathSeconds, Is.EqualTo(.05f));
            Assert.That(BoardView.Breath, Is.EqualTo(BoardView.BreathMode.Important), "default: important moments only");
        }

        [Test]
        public void TrailIsShortAndCapped()
        {
            Assert.That(ExitTrail.Life, Is.InRange(.18f, .25f));
            Assert.That(ExitTrail.Max, Is.EqualTo(3));
            Assert.That(ExitTrail.MinCells, Is.LessThan(ExitTrail.LengthCells), "a slow push leaves a shorter trail than a flick");
            Assert.That(ExitTrail.Order, Is.LessThan(100), "under every piece");
            Assert.That(ExitTrail.Style, Is.EqualTo(ExitTrail.Kind.Strip));
        }

        [Test]
        public void BombDemoShowsTheRealFuse()
        {
            var go = new UnityEngine.GameObject("bomb demo");
            try
            {
                var demo = go.AddComponent<BombIntroLoop>();
                demo.Init(UnityEngine.Vector2.zero, 0, 16);
                Assert.That(demo.Fuse, Is.EqualTo(16));
            }
            finally { UnityEngine.Object.DestroyImmediate(go); }
        }
    }
}
