using System.IO;
using NUnit.Framework;
using ReverseSolver.Presentation;

namespace ReverseSolver.Core.Tests
{
    /* PM_111: B the default (Zeyd's pick); the menu's hold shorter by &resimbekle= (0 / .07 / .145), the corridor's
       its own; on a short hold the cuts start at the arrival and are all drawn by 80 % of the pull back, and the pull
       back's whoosh comes in under the touch's paper. */
    public class Pm111Tests
    {
        [TearDown] public void Restore() { Dive75.Variant110 = 'b'; Dive75.HoldSec = -1; Dive75.LinesEnd = .7f; }

        [Test]
        public void BIsTheDefault() => Assert.That(Dive75.Variant110, Is.EqualTo('b'));

        [Test]
        public void TheHoldFlagIsTheMenusOnly()
        {
            string d = File.ReadAllText("Assets/_Game/Presentation/Ui/Dive75.cs"), root = File.ReadAllText("Assets/_Game/Presentation/GameRoot.cs");
            StringAssert.Contains("Dive75.HoldSec = QueryFloat(\"resimbekle\", -1, 0, 3);", root);
            StringAssert.Contains("if (HoldSec < 0 || _s.Diorama == null) { _p.Hold = _s.Diorama != null ? MenuHold112 : CorridorHold91;", d);   // PM_112: vol 2 the default
        }

        [Test]
        public void AShortHoldDrawsTheCutsByEightyPercent()
        {
            string d = File.ReadAllText("Assets/_Game/Presentation/Ui/Dive75.cs");
            StringAssert.Contains("const float LinesDone111 = .8f", d);
            StringAssert.Contains("LinesEnd = _shortHold ? LinesDone111 : LinesDone;", d);
            StringAssert.Contains("c = h1 + LinesEnd * _p.Back", d);
            StringAssert.Contains(": sm((back - Dive75.LinesEnd) / Mathf.Max(.05f, 1 - Dive75.LinesEnd));", d);   // the hand-over (PM_86) follows (PM_116: unless &renk=)
        }

        [Test]
        public void ThePullBacksWhooshComesInUnderThePaper()
        {
            string d = File.ReadAllText("Assets/_Game/Presentation/Ui/Dive75.cs");
            StringAssert.Contains("Gecis100.Play(Gecis100.Kind.Geri, Mathf.Lerp(GeriDuck111, 1, near));", d);
            StringAssert.Contains("float near = _shortHold ? Mathf.Clamp01((h1 - h0) / GeriNear111) : 1;", d);
        }

        /* the speed runs on through the hold: the pull back sets off with the camera's rate as the hold ends */
        [Test]
        public void ThePullBackSetsOffWithTheHoldsRate()
        {
            string d = File.ReadAllText("Assets/_Game/Presentation/Ui/Dive75.cs");
            StringAssert.Contains(": _shortHold ? Hermite5(l0, -Push91Rate(_p.Hold) * _p.Back, l1, u) : Hermite3(l0, -Push91Rate(_p.Hold) * _p.Back, l1, 0, u);", d);
        }
    }
}
