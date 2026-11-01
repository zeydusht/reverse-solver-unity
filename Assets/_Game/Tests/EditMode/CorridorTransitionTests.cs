using NUnit.Framework;
using ReverseSolver.Presentation;
using UnityEngine;

namespace ReverseSolver.Core.Tests
{
    /* PM_MESAJ_24: the slide's easing matches Task 18's track and the painting mesh lands on the aperture's corners. */
    public class CorridorTransitionTests
    {
        [Test]
        public void EaseMatchesTheVideoTrack()
        {
            Assert.AreEqual(0, CorridorTransition.Ease(0), 1e-6);
            Assert.AreEqual(.1035f, CorridorTransition.Ease(45 / 180f), 1e-4);   // frame-track.json, frame 45 of 180
            Assert.AreEqual(.5f, CorridorTransition.Ease(.5f), 1e-6);
            Assert.AreEqual(1, CorridorTransition.Ease(2), 1e-6);
        }

        [Test]
        public void ProjectionHitsTheCorners()
        {
            var q = new[] { new Vector2(208.3f, 291.2f), new Vector2(585.6f, 332.9f), new Vector2(576.9f, 882.2f), new Vector2(196.2f, 865.7f) };
            var uv = new[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
            for (int i = 0; i < 4; i++)
                Assert.That((CorridorTransition.Project(q, uv[i].x, uv[i].y) - q[i]).magnitude, Is.LessThan(.01f));
        }

        [Test]
        public void StripIsInResources() => Assert.IsNotNull(Resources.Load<Sprite>("corridor_strip"));
    }
}
