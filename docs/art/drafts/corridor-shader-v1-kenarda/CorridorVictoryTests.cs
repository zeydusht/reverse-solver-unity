using NUnit.Framework;
using ReverseSolver.Presentation;
using UnityEngine;

namespace ReverseSolver.Tests
{
    [Category("CorridorVictory")]
    public sealed class CorridorVictoryTests
    {
        [Test] public void StartupAndOrdinaryReturnsStayStatic()
        {
            var q = new CorridorVictoryQueue(); q.EnterMenu(10);
            Assert.IsFalse(q.Playing(10)); Assert.IsFalse(q.Pending);
        }
        [Test] public void WinIsConsumedOnceAndLayoutDoesNotRestartIt()
        {
            var q = new CorridorVictoryQueue(); q.Won(); q.EnterMenu(10);
            Assert.IsTrue(q.Playing(10)); Assert.IsTrue(q.Playing(12.9f));
            q.EnterMenu(11); Assert.AreEqual(10, q.StartedAt);
            Assert.IsFalse(q.Playing(13)); q.EnterMenu(20); Assert.IsFalse(q.Playing(20));
            q.Won(); q.EnterMenu(21); Assert.IsTrue(q.Playing(21));
        }
        [Test] public void LeavingMenuCancelsVisibleTravel()
        {
            var q = new CorridorVictoryQueue(); q.Won(); q.EnterMenu(5); q.LeaveMenu();
            Assert.IsFalse(q.Playing(6));
        }
        [Test] public void CameraMovesLeftWithOneBayAndAdditionalMidpointPullback()
        {
            Assert.AreEqual(Vector3.zero, CorridorVictory.CameraAt(0));
            var end = CorridorVictory.CameraAt(1); var mid = CorridorVictory.CameraAt(.5f);
            Assert.Less(end.x, 0); Assert.AreEqual(-3.42f, end.x, .0001f);
            Assert.Less(mid.z, end.z / 2); Assert.AreEqual(0, mid.y);
            Assert.AreEqual(0, CorridorVictory.Ease(-1)); Assert.AreEqual(1, CorridorVictory.Ease(2));
        }
        [Test] public void TravelHasSmoothEndsAndAConstantSlowerMiddle()
        {
            Assert.AreEqual(.5f, CorridorVictory.Ease(.5f), .00001f);
            float maxVelocity=0, previous=0;
            for(int frame=1;frame<=1000;frame++) {
                float next=CorridorVictory.Ease(frame/1000f);
                Assert.GreaterOrEqual(next,previous);
                maxVelocity=Mathf.Max(maxVelocity,(next-previous)*1000);
                previous=next;
            }
            Assert.Less(maxVelocity,1.284f); // old quintic peak: 1.875
            Assert.Less(CorridorVictory.Ease(.001f)/.001f,.001f);
            Assert.AreEqual(CorridorVictory.Ease(.4f)-CorridorVictory.Ease(.3f),CorridorVictory.Ease(.7f)-CorridorVictory.Ease(.6f),.00001f);
        }
    }
}
