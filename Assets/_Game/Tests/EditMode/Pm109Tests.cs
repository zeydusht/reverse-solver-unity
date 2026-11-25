using System.IO;
using NUnit.Framework;
using ReverseSolver.Presentation;

namespace ReverseSolver.Core.Tests
{
    /* PM_109: the menu dive's speed as Zeyd drew it the second time: off lower but up steeply, a long high plateau
       rising slowly to the peak, a late ease down to the even approach; its time what that takes at PM_108's even
       speed, .9..1.1 s; &dalis109=0 PM_108's. */
    public class Pm109Tests
    {
        [TearDown] public void Restore() { Dive75.Peak108 = 1.65f; Dive75.Dive108 = true; Dive75.Dive109 = true; }

        [Test]
        public void TheSpeedHasTheSecondDrawingsShape()
        {
            float T = Dive75.Seconds109(), even = 1 / 1.65f;
            Assert.That(Dive75.Speed109(0, T), Is.EqualTo(.40f).Within(1e-4f), "off lower, 40 % of the peak");
            Assert.That(Dive75.Speed109(.10f, T), Is.EqualTo(.85f).Within(1e-4f), "85 % of the peak by 10 % of the time");
            float prev = 0;
            for (int i = 0; i <= 65; i++) { float v = Dive75.Speed109(i / 100f, T); Assert.That(v, Is.GreaterThanOrEqualTo(prev - 1e-5f), "rising all the way to the peak"); prev = v; }
            for (int i = 10; i <= 72; i++) Assert.That(Dive75.Speed109(i / 100f, T), Is.GreaterThanOrEqualTo(.85f - 1e-4f), "high from 10 % to 72 %");
            Assert.That(Dive75.Speed109(.60f, T), Is.GreaterThan(Dive75.Speed109(.30f, T)), "the plateau climbs slowly");
            for (int i = 65; i <= 72; i++) Assert.That(Dive75.Speed109(i / 100f, T), Is.EqualTo(1).Within(1e-4f), "the peak");
            Assert.That(Dive75.Speed109(.60f, T), Is.GreaterThan(.95f), "PM_108's fall (45..60 %) now inside the plateau");
            prev = 2;
            for (int i = 72; i <= 82; i++) { float v = Dive75.Speed109(i / 100f, T); Assert.That(v, Is.LessThanOrEqualTo(prev + 1e-5f), "easing down late"); prev = v; }
            for (float x = .82f; x < 1 - .03f / T - .001f; x += .01f) Assert.That(Dive75.Speed109(x, T), Is.EqualTo(even).Within(1e-4f), "even to the arrival, PM_108's level");
            Assert.That(Dive75.Speed109(1, T), Is.EqualTo(even * .85f).Within(1e-4f), "the same short ease");
            for (int i = 1; i <= 1000; i++) Assert.That(System.Math.Abs(Dive75.Speed109(i / 1000f, T) - Dive75.Speed109((i - 1) / 1000f, T)), Is.LessThan(.03f), "no step");
        }

        [Test]
        public void ItsTimeComesOutNearOneSecond()
        {
            float T = Dive75.Seconds109();
            Assert.That(T, Is.InRange(.9f, 1.1f));
            Assert.That(T, Is.LessThan(1.2f), "shorter than PM_108's");
        }

        [Test]
        public void TheFlagIsWired()
        {
            string root = File.ReadAllText("Assets/_Game/Presentation/GameRoot.cs"), d = File.ReadAllText("Assets/_Game/Presentation/Ui/Dive75.cs");
            StringAssert.Contains("Dive75.Dive109 = Host.Current.Query(\"dalis109\") != \"0\";", root);
            StringAssert.Contains("_plan108 = Dive108; _plan109 = Dive108 && Dive109;", d);
            StringAssert.Contains("float MenuSpeed(float x, float T) => _plan109 ? Speed109(x, T) : Speed108(x, T);", d);
        }
    }
}
