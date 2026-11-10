using NUnit.Framework;
using ReverseSolver.Presentation;
using UnityEngine;

namespace ReverseSolver.Core.Tests
{
    public sealed class RopeSimTests
    {
        static readonly Vector2 Min = new Vector2(-20, -20), Max = new Vector2(20, 20);
        static RopeSim Make(Vector2 end) => new RopeSim(Vector2.zero, end, 1, Min, Max);
        static void Settle(RopeSim sim, Vector2 end)
        {
            for (int i = 0; i < 600; i++) sim.Advance(ChainTuning.Step, Vector2.zero, end);
        }

        static float Sag(RopeSim sim, Vector2 end)
        {
            var normal = new Vector2(end.y, -end.x).normalized;
            float sag = 0;
            for (int i = 1; i < sim.Count - 1; i++)
                sag = Mathf.Max(sag, Mathf.Abs(Vector2.Dot(sim[i] - end * ((float)i / (sim.Count - 1)), normal)));
            return sag;
        }

        [TestCase(1, 0, .30f)]
        [TestCase(3, 0, .30f)]
        [TestCase(5, 0, .30f)]
        [TestCase(8, 0, .30f)]
        [TestCase(1, -1, .35f)]
        [TestCase(2, -2, .35f)]
        [TestCase(2, 2, .35f)]
        public void RestSagMatchesTarget(float x, float y, float target)
        {
            var end = new Vector2(x, y); var sim = Make(end);
            Assert.That(Sag(sim, end), Is.EqualTo(target).Within(.05f), "pre-settled initial shape");
            Settle(sim, end);
            Assert.That(Sag(sim, end), Is.EqualTo(target).Within(.05f));
        }

        [TestCase(1, 0)]
        [TestCase(3, 0)]
        [TestCase(8, 0)]
        [TestCase(2, -2)]
        [TestCase(0, -3)]
        public void SettledLengthIsPreserved(float x, float y)
        {
            var end = new Vector2(x, y); var sim = Make(end);
            Settle(sim, end);
            float length = 0;
            for (int i = 1; i < sim.Count; i++)
            {
                float segment = Vector2.Distance(sim[i - 1], sim[i]);
                Assert.That(segment, Is.EqualTo(sim.Length / (sim.Count - 1)).Within(sim.Length / (sim.Count - 1) * .02f));
                length += segment;
            }
            Assert.That(length, Is.EqualTo(sim.Length).Within(sim.Length * .02f));
            Assert.That(sim[0], Is.EqualTo(Vector2.zero));
            Assert.That(sim[sim.Count - 1], Is.EqualTo(end));
        }

        [Test]
        public void TautRopeStraightensThenSagsAgain()
        {
            var sim = Make(new Vector2(2, 0));
            var taut = new Vector2(sim.Length + .5f, 0);
            Settle(sim, taut);
            Assert.That(Sag(sim, taut), Is.LessThan(.01f));
            Settle(sim, new Vector2(2, 0));
            Assert.That(Sag(sim, new Vector2(2, 0)), Is.EqualTo(.30f).Within(.05f));
        }

        [Test]
        public void FixedStepsAgreeAt30And144Hz()
        {
            var end = new Vector2(2, -2); var slow = Make(end); var fast = Make(end);
            slow.Kick(new Vector2(1.7f, 2.4f)); fast.Kick(new Vector2(1.7f, 2.4f));
            for (int i = 0; i < 90; i++) slow.Advance(1f / 30, Vector2.zero, end);
            for (int i = 0; i < 432; i++) fast.Advance(1f / 144, Vector2.zero, end);
            for (int i = 0; i < slow.Count; i++)
                Assert.That(Vector2.Distance(slow[i], fast[i]), Is.LessThan(.0001f));
        }

        [Test]
        public void MovingAnchorsLeavesParticlesInWorldSpaceAndBoundsHold()
        {
            var end = new Vector2(3, 0); var sim = Make(end);
            var before = sim[sim.Count / 2]; var shift = new Vector2(.5f, .5f);
            sim.Advance(ChainTuning.Step, shift, end + shift);
            Assert.That(Vector2.Distance(sim[sim.Count / 2], before + shift), Is.GreaterThan(.05f));
            sim.Kick(Vector2.down * 100);
            for (int frame = 0; frame < 120; frame++)
            {
                sim.Advance(1f / 30, shift, end + shift);
                for (int i = 0; i < sim.Count; i++)
                {
                    Assert.That(sim[i].x, Is.InRange(Min.x, Max.x));
                    Assert.That(sim[i].y, Is.InRange(Min.y, Max.y));
                }
            }
        }

        [Test]
        public void SplitHalvesHangThenFallAfterRelease()
        {
            var end = new Vector2(3, 0); var sim = Make(end);
            sim.Split();
            for (int i = 0; i < 48; i++) sim.Advance(ChainTuning.Step, Vector2.zero, end);
            Assert.That(sim[0], Is.EqualTo(Vector2.zero));
            Assert.That(sim[sim.Count - 1], Is.EqualTo(end));
            int cut = (sim.Count - 1) / 2;
            Assert.That(Vector2.Distance(sim[cut], sim[cut + 1]), Is.GreaterThan(sim.Length / (sim.Count - 1)));
            sim.Release(); Settle(sim, end);
            Assert.That(sim[0].y, Is.LessThan(-.1f));
            Assert.That(sim[sim.Count - 1].y, Is.LessThan(-.1f));
        }
    }
}
