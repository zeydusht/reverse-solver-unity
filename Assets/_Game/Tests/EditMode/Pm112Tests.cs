using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using ReverseSolver.Presentation;

namespace ReverseSolver.Core.Tests
{
    /* PM_112: vol 2's hold (.07 s) the menu's default; &bungee= (per cent) turns it into a bungee: the camera meets
       the picture at speed, brakes on into it at B's own fall's braking, its speed passes zero once at the bottom,
       and the picture is back at the arrival's size as long after the bottom as before it, the acceleration unbroken. */
    public class Pm112Tests
    {
        [TearDown] public void Restore() { Dive75.Bungee = 0; Dive75.HoldSec = -1; Dive75.LinesEnd = .7f; }

        static readonly MethodInfo QuinticM = typeof(Dive75).GetMethod("Quintic", BindingFlags.NonPublic | BindingFlags.Static);
        static readonly MethodInfo BackAtM = typeof(Dive75).GetMethod("BackAt112", BindingFlags.NonPublic | BindingFlags.Static);
        static float Q(float p0, float v0, float a0, float p1, float v1, float a1, float T, float x)
            => (float)QuinticM.Invoke(null, new object[] { p0, v0, a0, p1, v1, a1, T, x });
        static float BackAt(float d, float turn, float lnOut, float T) => (float)BackAtM.Invoke(null, new object[] { d, turn, lnOut, T });

        [Test]
        public void VolTwoIsTheDefaultAndTheBungeeIsAFlag()
        {
            string d = File.ReadAllText("Assets/_Game/Presentation/Ui/Dive75.cs"), root = File.ReadAllText("Assets/_Game/Presentation/GameRoot.cs");
            Assert.That(Dive75.Bungee, Is.EqualTo(0f));
            StringAssert.Contains("const float MenuHold112 = .07f, Brake112 = 5.5f;", d);
            StringAssert.Contains("Dive75.Bungee = QueryFloat(\"bungee\", 0, 0, 20);", root);
            StringAssert.Contains("if (Bungee > 0) PlanBungee112();", d);
        }

        [Test]
        public void TheQuinticMeetsItsEnds()
        {
            const float T = 1, h = .01f;
            float p0 = .1f, v0 = .5f, a0 = -2, p1 = -.4f, v1 = .2f, a1 = 3;
            float F(float x) => Q(p0, v0, a0, p1, v1, a1, T, x);
            float G(float x) => F(T - x);   // the end, backwards
            Assert.That(F(0), Is.EqualTo(p0).Within(1e-5f));
            Assert.That(F(T), Is.EqualTo(p1).Within(1e-5f));
            // one-sided differences, second order
            Assert.That((-3 * F(0) + 4 * F(h) - F(2 * h)) / (2 * h), Is.EqualTo(v0).Within(.01f));
            Assert.That((-3 * G(0) + 4 * G(h) - G(2 * h)) / (2 * h), Is.EqualTo(-v1).Within(.01f));
            Assert.That((2 * F(0) - 5 * F(h) + 4 * F(2 * h) - F(3 * h)) / (h * h), Is.EqualTo(a0).Within(.1f));
            Assert.That((2 * G(0) - 5 * G(h) + 4 * G(2 * h) - G(3 * h)) / (h * h), Is.EqualTo(a1).Within(.1f));
        }

        /* the menu's picture at the arrival (2.9 ln a screen height, as measured), braking at 5.5 screen heights/s^2,
           the pull back's way out (ln 3.64): at each depth one zero crossing of the speed, back at the arrival's size
           twice the bottom's time after the arrival (.10 .. .18 s), no frame (60 fps) standing still but the one at the
           bottom, and the pull back's speed one hump */
        [TestCase(2f)]
        [TestCase(4f)]
        [TestCase(6f)]
        public void TheBungeeTurnsOnceAndComesBackOnTime(float percent)
        {
            const float perScreen = 2.9f, brake = 5.5f;
            float depth = (float)Math.Log(1 + percent / 100), hold = (float)Math.Sqrt(2 * depth / (perScreen * brake));
            float v = perScreen * brake * hold, a0 = -perScreen * brake, lnOut = (float)Math.Log(3.64f), back = .85f - hold;
            Assert.That(2 * hold, Is.InRange(.09f, .19f));
            float lo = .5f, hi = 400;
            for (int i = 0; i < 40; i++) { float m = (lo + hi) / 2; if (BackAt(depth, m, lnOut, back) > hold) lo = m; else hi = m; }
            float turn = (lo + hi) / 2;
            float Z(float t) => t < hold ? Q(0, v, a0, depth, 0, -turn, hold, t) : Q(depth, 0, -turn, -lnOut, 0, 0, back, t - hold);
            Assert.That(hold + BackAt(depth, turn, lnOut, back), Is.EqualTo(2 * hold).Within(.002f));
            Assert.That(turn, Is.EqualTo(perScreen * brake).Within(perScreen * brake * .25f), "about the arrival's braking: no kink at the bottom");
            int changes = 0, still = 0, humps = 0; float prev = 0, prevOut = 0; int prevDir = 0;
            const float dt = 1 / 600f;
            for (float t = dt; t < hold + back - dt; t += dt)
            {
                float s = (Z(t + dt) - Z(t - dt)) / (2 * dt);
                if (prev != 0 && t < hold + back - .05f && Math.Sign(s) != Math.Sign(prev)) changes++;   // (at rest at the end)
                prev = s;
                if (t > hold + dt && t < hold + back - .05f)
                {
                    int dir = Math.Sign(s - prevOut);   // the out speed's magnitude grows (-1) then falls (+1), once
                    if (prevDir == -1 && dir == 1) humps++;
                    if (dir != 0) prevDir = dir;
                }
                prevOut = s;
            }
            for (float t = 1 / 60f; t < hold + back - .05f; t += 1 / 60f)
                if (Math.Abs(Z(t) - Z(t - 1 / 60f)) < 1e-4f) still++;
            Assert.That(changes, Is.EqualTo(1), "the speed passes zero once");
            Assert.That(still, Is.LessThanOrEqualTo(1), "no frozen frame (but at most the bottom's)");
            Assert.That(humps, Is.EqualTo(1), "the pull back's speed one hump");
            Assert.That(Z(hold), Is.EqualTo(depth).Within(1e-5f));
        }

        [Test]
        public void TheSoundsFollowTheBottom()
        {
            string d = File.ReadAllText("Assets/_Game/Presentation/Ui/Dive75.cs");
            StringAssert.Contains("float end = _bungee ? h1 : h0;", d);   // the dive's whoosh ends at the bottom
            StringAssert.Contains("_bungee && t < h1 ? air * (1 - SmoothStep((t - h0) / Mathf.Max(.01f, h1 - h0))) : 0", d);   // the air too
            StringAssert.Contains("if (_new91 && TouchMax > 0 && !_bungee)", d);   // no touch
            StringAssert.Contains("float l = _bungee ? Quintic(l0, 0, _bungeeTurn, l1, 0, 0, _p.Back, t - h1)", d);
        }
    }
}
