using System.IO;
using NUnit.Framework;
using ReverseSolver.Presentation;

namespace ReverseSolver.Core.Tests
{
    /* PM_115: the flight's sideways eased to a stop before the approach (the turn no longer stops in a frame), and the
       dive's plain picture skips the piece shader's samples it does not show (&pm115=0: PM_114's). */
    public class Pm115Tests
    {
        [Test]
        public void TheFixesAreOnAndTheOldStaysAFlag()
        {
            string d = File.ReadAllText("Assets/_Game/Presentation/Ui/Dive75.cs"), root = File.ReadAllText("Assets/_Game/Presentation/GameRoot.cs");
            string shader = File.ReadAllText("Assets/_Game/Presentation/Shaders/Piece.shader"), pv = File.ReadAllText("Assets/_Game/Presentation/Board/PieceView.cs");
            StringAssert.Contains("Dive75.Fix115 = Host.Current.Query(\"pm115\") != \"0\";", root);
            StringAssert.Contains("if (bell && Fix115) p.x += TurnEase115(s);", d);
            StringAssert.Contains("pv.SetReveal(true, time, EdgeSeconds, look, rimD, Fix115);", d);
            StringAssert.Contains("_b.SetFloat(RvFastId, on && fast ? 1 : 0);", pv);
            // the fast path only in the dive (_RvOn), only in its plain picture (look 0), the ring only where no cut is drawn
            StringAssert.Contains("bool plain = _RvOn > 0.5 && _RvFast > 0.5 && look <= 0.0;", shader);
            StringAssert.Contains("bool ring = !(_RvOn > 0.5 && _RvFast > 0.5) || seam > 0.0;", shader);
            StringAssert.Contains("if (plain && imageAmt > 0.001) shA = 0.0;", shader);
        }

        [Test]
        public void TheEaseStartsStillEndsAtZeroAndTakesTheSpeedAway()
        {
            const float h = 1e-3f;
            float Q(float u) => Dive75.TurnQ115(u);
            float V(float u) => (Q(u + h) - Q(u - h)) / (2 * h);
            float A(float u) => (Q(u + h) - 2 * Q(u) + Q(u - h)) / (h * h);
            Assert.That(Q(0), Is.EqualTo(0).Within(1e-6f));
            Assert.That(Q(1), Is.EqualTo(0).Within(1e-6f), "square on where the approach starts");
            Assert.That(V(h), Is.EqualTo(0).Within(1e-3f));
            Assert.That(A(2 * h), Is.EqualTo(0).Within(.1f), "no push at the start");
            Assert.That(V(1 - h), Is.EqualTo(1).Within(1e-2f), "the path's sideways speed taken away at the split");
            Assert.That(A(1 - 2 * h), Is.EqualTo(0).Within(.2f), "no push at the end");
            // the sideways speed (1 - V, the path's own taken as even) never turns back: no overshoot
            for (float u = .01f; u < 1; u += .01f) Assert.That(1 - V(u), Is.GreaterThanOrEqualTo(-1e-3f), $"at {u:0.00}");
        }
    }
}
