using System.IO;
using NUnit.Framework;
using ReverseSolver.Presentation;

namespace ReverseSolver.Core.Tests
{
    /* PM_108: the menu dive's speed as Zeyd drew it: off at once, up to a peak held over the arc, down to an even
       approach before 60 % of the time, 1.2 s in all; &dalis108=0 PM_92's. */
    public class Pm108Tests
    {
        [TearDown] public void Restore() { Dive75.Peak108 = 1.65f; Dive75.Dive108 = true; }

        [Test]
        public void TheSpeedHasZeydsShape()
        {
            const float T = 1.2f;
            Assert.That(Dive75.Speed108(0, T), Is.EqualTo(.55f).Within(1e-4f), "off at once, 55 % of the peak");
            float prev = 0;
            for (int i = 0; i <= 25; i++) { float v = Dive75.Speed108(i / 100f, T); Assert.That(v, Is.GreaterThanOrEqualTo(prev - 1e-5f), "rising to the peak"); prev = v; }
            for (int i = 25; i <= 45; i++) Assert.That(Dive75.Speed108(i / 100f, T), Is.EqualTo(1).Within(1e-4f), "the plateau");
            prev = 2;
            for (int i = 45; i <= 60; i++) { float v = Dive75.Speed108(i / 100f, T); Assert.That(v, Is.LessThanOrEqualTo(prev + 1e-5f), "easing down"); prev = v; }
            float even = 1 / 1.65f;
            for (float x = .6f; x < 1 - .03f / T - .001f; x += .01f) Assert.That(Dive75.Speed108(x, T), Is.EqualTo(even).Within(1e-4f), "even to the arrival");
            Assert.That(Dive75.Speed108(1, T), Is.EqualTo(even * .85f).Within(1e-4f), "a short ease on the last frames");
            // no step in the speed (the acceleration has none either: each stretch meets the next with its slope)
            for (int i = 1; i <= 1000; i++) Assert.That(System.Math.Abs(Dive75.Speed108(i / 1000f, T) - Dive75.Speed108((i - 1) / 1000f, T)), Is.LessThan(.03f));
        }

        [Test]
        public void ThePeakFlagSetsTheEvenSpeed()
        {
            Dive75.Peak108 = 2;
            Assert.That(Dive75.Speed108(.8f, 1.2f), Is.EqualTo(.5f).Within(1e-4f));
        }

        [Test]
        public void FlagsAndTimingAreWired()
        {
            string root = File.ReadAllText("Assets/_Game/Presentation/GameRoot.cs"), d = File.ReadAllText("Assets/_Game/Presentation/Ui/Dive75.cs");
            StringAssert.Contains("Dive75.Dive108 = Host.Current.Query(\"dalis108\") != \"0\";", root);
            StringAssert.Contains("Dive75.Peak108 = QueryFloat(\"dalistepe\", 1.65f", root);
            StringAssert.Contains("const float MenuSeconds108 = 1.2f", d);
            StringAssert.Contains("float T = (_plan108 ? MenuSeconds108 : MenuSeconds92) / Mathf.Max(.25f, DiveSpeed);", d);
            StringAssert.Contains("if (_s.Diorama == null) _p.Approach = CorridorSeconds92;", d);   // the corridor's dive as it was
        }

        /* the bottom-right corner seen past the floor early in the arc (PM_88): the wall and the floor go on past the
           plate, its edge pixels drawn there */
        [Test]
        public void TheFloorGoesOnPastThePlate()
        {
            string dio = File.ReadAllText("Assets/_Game/Presentation/Ui/Diorama78.cs"), d = File.ReadAllText("Assets/_Game/Presentation/Ui/Dive75.cs");
            StringAssert.Contains("var hi = _a.PlateSize * 1.25f - _a.ImageOrigin;", dio);
            StringAssert.Contains("px => OnFloor(px - _a.ImageOrigin), 40, 20, order, clampUv: true));", dio);
            StringAssert.Contains("uv.Add(clampUv ? new Vector2(Mathf.Clamp01(t.x), Mathf.Clamp01(t.y)) : t);", d);
        }
    }
}
