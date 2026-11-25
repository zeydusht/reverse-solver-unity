using System.IO;
using NUnit.Framework;
using ReverseSolver.Presentation;

namespace ReverseSolver.Core.Tests
{
    /* PM_110: the menu dive's speed as Zeyd drew it the third time: off near the peak, a short climb to a long flat
       plateau, the peak at 55..62 %, a steep smooth fall to half of it by 82 %, then a slight rise to the touch; its
       time what that takes at PM_109's peak speed, .9..1.1 s; &dalis110=0 PM_109's. */
    public class Pm110Tests
    {
        [TearDown] public void Restore() { Dive75.Peak108 = 1.65f; Dive75.Peak110 = 2f; Dive75.Dive108 = Dive75.Dive109 = Dive75.Dive110 = true; Dive75.Variant110 = 'b'; Dive75.BackSpeed = .56f; }

        [Test]
        public void TheSpeedHasTheThirdDrawingsShape()
        {
            float T = Dive75.Seconds110();
            Assert.That(Dive75.Speed110(0, T), Is.EqualTo(.90f).Within(1e-4f), "off near the peak");
            float prev = 0;
            for (int i = 0; i <= 8; i++) { float v = Dive75.Speed110(i / 100f, T); Assert.That(v, Is.GreaterThanOrEqualTo(prev - 1e-5f), "a short climb"); prev = v; }
            for (int i = 8; i <= 55; i++) Assert.That(Dive75.Speed110(i / 100f, T), Is.EqualTo(.97f).Within(1e-4f), "the flat plateau");
            Assert.That(Dive75.Speed110(.62f, T), Is.EqualTo(1).Within(1e-4f), "the peak at 62 %");
            prev = 2;
            for (int i = 62; i <= 82; i++) { float v = Dive75.Speed110(i / 100f, T); Assert.That(v, Is.LessThanOrEqualTo(prev + 1e-5f), "the fall"); Assert.That(v, Is.GreaterThanOrEqualTo(.5f - 1e-3f), "no dip under the low"); prev = v; }
            Assert.That(Dive75.Speed110(.72f, T), Is.LessThan(.8f), "a steep fall");
            Assert.That(Dive75.Speed110(.82f, T), Is.EqualTo(.5f).Within(1e-4f), "half the peak by 82 %");
            prev = 0;
            for (float x = .82f; x < 1 - .03f / T - .001f; x += .01f) { float v = Dive75.Speed110(x, T); Assert.That(v, Is.GreaterThanOrEqualTo(prev - 1e-5f), "rising slightly to the end"); prev = v; }
            Assert.That(prev, Is.InRange(.53f, .55f), "about 55 % before the last ease");
            Assert.That(Dive75.Speed110(1, T), Is.LessThan(prev), "the same short ease");
            for (int i = 1; i <= 1000; i++) Assert.That(System.Math.Abs(Dive75.Speed110(i / 1000f, T) - Dive75.Speed110((i - 1) / 1000f, T)), Is.LessThan(.03f), "no step");
        }

        [Test]
        public void ItsTimeComesOutNearOneSecond() => Assert.That(Dive75.Seconds110(), Is.InRange(.9f, 1.1f));

        [Test]
        public void ThePeakFlagSetsTheLowSpeed()
        {
            Dive75.Peak110 = 2.5f;
            Assert.That(Dive75.Speed110(.82f, 1), Is.EqualTo(.4f).Within(1e-4f));
        }

        /* the real aim: the way in at least as quick as the way back (its mean speed on the screen) */
        [Test]
        public void TheDiveIsAsQuickAsThePullBack()
        {
            // the pull back measured as PM_92 measures the dive: 0.438 screen heights in 0.78 s
            Assert.That(Dive75.BackSpeed, Is.GreaterThanOrEqualTo(.438f / .783f - .005f));
            string d = File.ReadAllText("Assets/_Game/Presentation/Ui/Dive75.cs");
            StringAssert.Contains("MenuWay92(MenuQuad92) / Mathf.Max(.05f, BackSpeed)", d);
        }

        [Test]
        public void TheBVariantIsThePullBackBackwards()
        {
            Assert.That(Dive75.SpeedBack110(0), Is.EqualTo(.16f).Within(1e-4f), "the pull back's end first");
            Assert.That(Dive75.SpeedBack110(1), Is.EqualTo(.07f).Within(1e-4f), "its start last");
            Assert.That(Dive75.SpeedBack110(.6f), Is.EqualTo(1).Within(1e-4f), "its peak (at 40 % of it) at 60 %");
            for (int i = 1; i <= 1000; i++) Assert.That(System.Math.Abs(Dive75.SpeedBack110(i / 1000f) - Dive75.SpeedBack110((i - 1) / 1000f)), Is.LessThan(.03f), "smooth");
        }

        [Test]
        public void TheFlagsAreWired()
        {
            string root = File.ReadAllText("Assets/_Game/Presentation/GameRoot.cs"), d = File.ReadAllText("Assets/_Game/Presentation/Ui/Dive75.cs");
            StringAssert.Contains("Dive75.Dive110 = Host.Current.Query(\"dalis110\") != \"0\";", root);
            StringAssert.Contains("Dive75.Peak110 = QueryFloat(\"dalistepe\", 2f, 1.05f, 3);", root);
            StringAssert.Contains("_plan110 = _plan109 && Dive110;", d);
            StringAssert.Contains("Dive75.BackSpeed = QueryFloat(\"dalisgeri\", .56f, .2f, 2);", root);
            StringAssert.Contains("Dive75.Variant110 = v110 == \"a\" || v110 == \"c\" ? v110[0] : 'b';", root);
        }
    }
}
