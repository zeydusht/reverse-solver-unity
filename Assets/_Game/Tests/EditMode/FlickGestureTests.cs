using NUnit.Framework;
using ReverseSolver.Presentation;
using UnityEngine;

namespace ReverseSolver.Core.Tests
{
    public class FlickGestureTests
    {
        static FlickResult Read(float x, float y, float speed = 0, bool released = true, float cell = 60) =>
            new FlickGesture().Classify(Vector2.zero, new Vector2(x, y), new Vector2(speed, 0), released, cell);

        [Test]
        public void DeadZoneIsATapEvenAtHighSpeed()
        {
            Assert.That(Read(7.99f, 0, 900).State, Is.EqualTo(FlickState.None));
            Assert.That(Read(7.99f, 0, 900, false).State, Is.EqualTo(FlickState.Pending));
            Assert.That(Read(8, 0, 900, true, 20).State, Is.EqualTo(FlickState.Move));
        }

        [TestCase(20, 1, 1, 0)]
        [TestCase(-20, 1, -1, 0)]
        [TestCase(1, 20, 0, 1)]
        [TestCase(1, -20, 0, -1)]
        public void UsesFourDominantDirections(float x, float y, float dx, float dy)
        {
            var r = Read(x, y);
            Assert.That(r.State, Is.EqualTo(FlickState.Move));
            Assert.That(r.Direction, Is.EqualTo(new Vector2(dx, dy)));
        }

        [Test]
        public void AxisStaysLockedAfterFirstUnambiguousMotion()
        {
            var g = new FlickGesture();
            g.Classify(Vector2.zero, new Vector2(10, 0), Vector2.zero, false, 60);
            var r = g.Classify(Vector2.zero, new Vector2(20, 40), Vector2.zero, true, 60);
            Assert.That(r.Direction, Is.EqualTo(Vector2.right));
        }

        [TestCase(20, 20)]
        [TestCase(20, 16)]
        [TestCase(-16, 20)]
        public void AmbiguousDiagonalCancelsWithoutMoving(float x, float y)
        {
            Assert.That(Read(x, y, 1000).State, Is.EqualTo(FlickState.Cancel));
            Assert.That(Read(x, y, 1000, false).State, Is.EqualTo(FlickState.Pending));
        }

        [Test]
        public void DistanceUsesTheSmallerOfThirtyPercentAndEighteenPoints()
        {
            foreach (float cell in new[] { 40f, 60f, 100f })
            {
                float threshold = Mathf.Min(.3f * cell, 18f);
                Assert.That(Read(threshold - .01f, 0, 1000, false, cell).State, Is.EqualTo(FlickState.Pending));
                Assert.That(Read(threshold - .01f, 0, 0, true, cell).State, Is.EqualTo(FlickState.Cancel));
                Assert.That(Read(threshold, 0, 0, true, cell).State, Is.EqualTo(FlickState.Move));
                Assert.That(Read(threshold + .01f, 0, 0, true, cell).State, Is.EqualTo(FlickState.Move));
            }
        }

        [Test]
        public void SpeedTriggersWhileHeldOrReleaseTriggersAtAnySpeed()
        {
            Assert.That(Read(18, 0, 599, false).State, Is.EqualTo(FlickState.Pending));
            Assert.That(Read(18, 0, 600, false).State, Is.EqualTo(FlickState.Move));
            Assert.That(Read(18, 0, 0, true).State, Is.EqualTo(FlickState.Move));
            Assert.That(Read(18, 0, -900, false).State, Is.EqualTo(FlickState.Pending));
        }

        [Test]
        public void CommittedTouchCannotMoveTwice()
        {
            var g = new FlickGesture();
            Assert.That(g.Classify(Vector2.zero, new Vector2(18, 0), new Vector2(600, 0), false, 60).State, Is.EqualTo(FlickState.Move));
            Assert.That(g.Classify(Vector2.zero, new Vector2(80, 0), Vector2.zero, true, 60).State, Is.EqualTo(FlickState.None));
        }

        [TestCase(320, 7)]
        [TestCase(375, 8)]
        [TestCase(393, 6)]
        public void PresentationLayoutKeepsTwentyFourPointsAtBothEdges(float width, int columns)
        {
            float cell = BoardSizing.CellSize(width, 852, columns, 6);
            Assert.That((width - columns * cell) / 2, Is.GreaterThanOrEqualTo(24));
            Assert.That(BoardSizing.SideGap(width, columns), Is.InRange(24f, 34f));
        }
    }
}
