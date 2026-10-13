using System.Linq;
using NUnit.Framework;
using ReverseSolver.Core;
using ReverseSolver.Generation;

namespace ReverseSolver.Core.Tests
{
    /* G3: the curve inside a level, on web levels whose shape is known. */
    public class CurveTests
    {
        [Test]
        public void PeakInTheKnotScoresHigh()
        {
            var c = Curve.Of(TestData.Levels["L15"]);
            TestContext.WriteLine("L15 " + c.Spark() + " " + c);
            Assert.That(c.PeakAt, Is.InRange(.3, .7));
            Assert.That(c.Fit, Is.GreaterThanOrEqualTo(.9));
        }

        [Test]
        public void PeakAtTheStartScoresLow()
        {
            var c = Curve.Of(TestData.Levels["L05"]);
            TestContext.WriteLine("L05 " + c.Spark() + " " + c);
            Assert.That(c.PeakAt, Is.LessThan(.2));
            Assert.That(c.PeakScore, Is.EqualTo(1 - 3 / 3.0).Within(1e-9), "highest bin 0: three bins before the knot");
            Assert.That(c.Fit, Is.LessThanOrEqualTo(.4));
        }

        [Test]
        public void BombLevelsGiveTheWholeCurveAndSeedsRepeat()
        {
            var a = Curve.Of(TestData.Levels["L40"], 60);
            var b = Curve.Of(TestData.Levels["L40"], 60);
            Assert.That(a.Reading.All(r => !double.IsNaN(r)), Is.True, "long fuses for the curve: no bin left empty");
            Assert.That(b.Reading, Is.EqualTo(a.Reading));
            Assert.That(a.Spark().Length, Is.EqualTo(LevelCurve.Bins));
        }

        [Test]
        public void KnotKnobRaisesTheFit()
        {
            var cat = Catalog.From(TestData.Levels);
            double Fit(double knot, double ease) => Enumerable.Range(1, 8)
                .Select(s => Generator.Make(FactoryPlan.Slot(14), new GenParams { Interlock = 5, Sweep = 1, Knot = knot, Ease = ease }, (uint)s, 20, cat))
                .Where(c => c != null).Average(c => Curve.Of(c.Level, 80).Fit);
            double flat = Fit(0, 0), shaped = Fit(3, 1);
            TestContext.WriteLine($"slot 14 fit: no shape {flat:0.00}, knot 3 / ease 1 {shaped:0.00}");
            Assert.That(shaped, Is.GreaterThan(flat));
        }
    }
}
