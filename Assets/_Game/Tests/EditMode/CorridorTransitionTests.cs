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

        /* PM_85: the 1.5 s glide starts and stops at rest, never goes back (no bounce), and its speed never jumps. */
        [Test]
        public void GlideStartsAndStopsSoftlyWithoutABounce()
        {
            Assert.AreEqual(0, CorridorTransition.Glide(0), 1e-6);
            Assert.AreEqual(1, CorridorTransition.Glide(1), 1e-5);
            float last = 0, lastV = 0;
            const int n = 400;
            for (int i = 1; i <= n; i++)
            {
                float p = CorridorTransition.Glide(i / (float)n), v = (p - last) * n;
                Assert.That(p, Is.GreaterThanOrEqualTo(last - 1e-6f), $"no step back at {i}");
                Assert.That(Mathf.Abs(v - lastV), Is.LessThan(.05f), $"no speed jump at {i}");
                last = p; lastV = v;
            }
            Assert.That(CorridorTransition.Glide(.01f), Is.LessThan(.001f), "a soft start");
            Assert.That(1 - CorridorTransition.Glide(.99f), Is.LessThan(.001f), "a soft stop");
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
