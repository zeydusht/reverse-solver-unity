using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using ReverseSolver.Presentation;

namespace ReverseSolver.Core.Tests
{
    /* PM_113: &u=1/2/3 the U instead of vol 2's hold: the same time to the board, the bottom (4 % in) at the middle of
       vol 2's hold, the speed through zero on a straight line (+.33 -> -.33 screen heights/s in .2 / .3 / .4 s), the
       same both sides, one zero crossing, no frozen frame; laid over B's own speed (its shape, PM_110 'b'). */
    public class Pm113Tests
    {
        [TearDown] public void Restore() { Dive75.U = 0; Dive75.Bungee = 0; }

        const BindingFlags Any = BindingFlags.NonPublic | BindingFlags.Static;
        static readonly Type PlanT = typeof(Dive75).GetNestedType("UPlan", BindingFlags.NonPublic);
        static readonly MethodInfo PlanM = typeof(Dive75).GetMethods(Any).First(m => m.Name == "PlanU113" && m.GetParameters().Length == 7);
        static readonly MethodInfo SpeedM = typeof(Dive75).GetMethod("SpeedU113", Any);
        static readonly MethodInfo BackM = typeof(Dive75).GetMethod("UBack113", Any);
        static readonly MethodInfo WidthM = typeof(Dive75).GetMethod("U113Width", Any);
        static float F(object plan, string field) => (float)PlanT.GetField(field).GetValue(plan);

        // B as the game lays it out (vol 2, p23 level 1 at 390 x 844: 0.402 screen heights at 0.56 a second)
        const float D = .402f, PerScreen = 3.26f, Hold = .07f;
        static readonly float T = D / .56f;
        static float[] BSpeeds()
        {
            const int N = 600;
            var v = new float[N + 1]; float area = 0;
            for (int k = 0; k <= N; k++) v[k] = Dive75.SpeedBack110(k / (float)N);
            for (int k = 1; k <= N; k++) area += (v[k] + v[k - 1]) / 2 * T / N;
            for (int k = 0; k <= N; k++) v[k] *= D / area;
            return v;
        }

        [Test]
        public void TheUIsAFlagAndVolTwoStaysTheDefault()
        {
            string d = File.ReadAllText("Assets/_Game/Presentation/Ui/Dive75.cs"), root = File.ReadAllText("Assets/_Game/Presentation/GameRoot.cs");
            Assert.That(Dive75.U, Is.EqualTo(0f));
            StringAssert.Contains("Dive75.U = QueryFloat(\"u\", 0, 0, 5);", root);
            StringAssert.Contains("if (U > 0) PlanU113(); else if (Bungee > 0) PlanBungee112();", d);
            StringAssert.Contains("const float Depth113 = .04f, Edge113 = .33f, MinFall113 = .08f;", d);
            StringAssert.Contains("_bungee = _u = true;", d);   // the bungee's sounds, air, cuts and no touch
            StringAssert.Contains("if (_u && !_turned) TurnU113(from, lnCover - _bungeeDepth, Mathf.Log(_homeSize));", d);   // the way out over the slide
            StringAssert.Contains("float l = _u ? UBack113(l0, l1, _uOut, _uEdge, _p.Back, t - h1)", d);
            Assert.That((float)WidthM.Invoke(null, new object[] { 1f }), Is.EqualTo(.2f).Within(1e-5f));
            Assert.That((float)WidthM.Invoke(null, new object[] { 3f }), Is.EqualTo(.4f).Within(1e-5f));
        }

        [TestCase(1f)]
        [TestCase(2f)]
        [TestCase(3f)]
        public void TheSpeedPassesZeroOnAStraightLine(float uFlag)
        {
            var v = BSpeeds();
            float width = (float)WidthM.Invoke(null, new object[] { uFlag }), depth = (float)Math.Log(1.04), tb = T + Hold / 2, end = T + .882f;
            object plan = PlanM.Invoke(null, new object[] { v, T, D, PerScreen, tb, width, depth });
            float A = F(plan, "A"), Ta = F(plan, "Ta"), Mu = F(plan, "Mu"), brake = F(plan, "Brake");
            float edge = F(plan, "VL") / A, lnOut = 1.283f, back = end - tb;
            float Ln(float x) => (float)BackM.Invoke(null, new object[] { 0f, lnOut + depth, A * PerScreen, edge, back, x });
            // the signed speed on the screen (screen heights/s, + in)
            float Speed(float t)
            {
                if (t < Ta) return (float)SpeedM.Invoke(null, new object[] { plan, v, T, t });
                if (t < tb) return A * (tb - t);
                const float h = 1e-3f; float x = t - tb;
                return -(Ln(x + h) - Ln(Math.Max(0, x - h))) / (x + h - Math.Max(0, x - h)) / PerScreen;
            }
            // the way in covered at the framing, the bottom where asked, 4 % in
            float way = 0; const int n = 2000;
            for (int k = 1; k <= n; k++) way += (Speed(Ta * k / n) + Speed(Ta * (k - 1) / n)) / 2 * Ta / n;
            Assert.That(way, Is.EqualTo(D).Within(D * .01f), "the way in");
            Assert.That(tb, Is.EqualTo(.745f).Within(.02f));
            Assert.That(PerScreen * A / 2 * (tb - Ta) * (tb - Ta), Is.EqualTo(depth).Within(1e-4f), "4 % in at the bottom");
            Assert.That(Mu, Is.InRange(.95f, 1.6f), "B's shape lifted, not replaced");
            Assert.That(brake, Is.GreaterThanOrEqualTo(-5.5f - 1e-3f), "the fall no harder than B's own");
            // the times through +.3 and -.3: the width, the same both sides of the bottom
            float up = float.NaN, down = float.NaN; int zeros = 0, still = 0; float prev = Speed(.3f);
            for (float t = .3f; t < end - .05f; t += 1e-3f)
            {
                float s = Speed(t);
                if (float.IsNaN(up) && prev >= .3f && s < .3f) up = t;
                if (float.IsNaN(down) && prev > -.3f && s <= -.3f) down = t;
                if (Math.Sign(s) != Math.Sign(prev)) zeros++;
                prev = s;
            }
            Assert.That(down - up, Is.EqualTo(width).Within(.01f), "+.3 -> -.3");
            Assert.That(tb - up, Is.EqualTo(down - tb).Within(.01f), "both sides alike");
            Assert.That(zeros, Is.EqualTo(1), "one zero crossing");
            // straight: the speed's slope the same all through +-.3
            for (float t = up; t <= down; t += .01f)
                Assert.That((Speed(t + .002f) - Speed(t - .002f)) / .004f, Is.EqualTo(-A).Within(A * .05f), $"the slope at {t:0.000} s");
            // no frozen frame (60 fps): the screen moves on every frame but the bottom's
            for (float t = 1 / 60f; t < end - .05f; t += 1 / 60f)
                if (Math.Abs(Speed(t)) < .02f && Math.Abs(t - tb) > 1 / 60f) still++;
            Assert.That(still, Is.EqualTo(0), "no frame standing still");
            Assert.That(Ln(back), Is.EqualTo(lnOut + depth).Within(1e-4f), "the board's view at the end");
        }

        [Test]
        public void TheWayOutIsOneHump()
        {
            float a = .6f / .3f * PerScreen, edge = .33f / (.6f / .3f), back = .85f, l1 = 1.322f;
            float Ln(float x) => (float)BackM.Invoke(null, new object[] { 0f, l1, a, edge, back, x });
            int humps = 0, prevDir = 0; float prev = 0;
            for (float x = .005f; x < back - .005f; x += .005f)
            {
                float s = (Ln(x + .001f) - Ln(x - .001f)) / .002f;
                int dir = Math.Sign(s - prev);
                if (prevDir == 1 && dir == -1) humps++;
                if (dir != 0) prevDir = dir;
                prev = s;
            }
            Assert.That(humps, Is.EqualTo(1));
            Assert.That(Ln(edge), Is.EqualTo(a / 2 * edge * edge).Within(1e-5f));
            Assert.That(Ln(back), Is.EqualTo(l1).Within(1e-5f));
        }
    }
}
