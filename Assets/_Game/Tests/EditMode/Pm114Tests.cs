using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using ReverseSolver.Presentation;
using UnityEngine;

namespace ReverseSolver.Core.Tests
{
    /* PM_114: U1 the default; the board's rim drawn by two pens from the top's middle round both sides to the bottom's
       middle, from the dive's bottom to the cuts' end, eased in and out; the pens' tips fade (&kenar=0: PM_113's). */
    public class Pm114Tests
    {
        [Test]
        public void UOneIsTheDefaultAndTheOldStaysAFlag()
        {
            string d = File.ReadAllText("Assets/_Game/Presentation/Ui/Dive75.cs"), root = File.ReadAllText("Assets/_Game/Presentation/GameRoot.cs");
            string shader = File.ReadAllText("Assets/_Game/Presentation/Shaders/Piece.shader");
            StringAssert.Contains("Dive75.U = QueryFloat(\"u\", Dive75.HoldSec >= 0 || Dive75.Bungee > 0 ? 0 : 1, 0, 5);", root);
            StringAssert.Contains("Dive75.Fix114 = Host.Current.Query(\"kenar\") != \"0\";", root);
            StringAssert.Contains("float Outer(Cell c, Dir d) => Fix114 ? RimStart114(c.X, c.Y, d, l.Width, l.Height) : outer;", d);
            StringAssert.Contains("float share = LineFrac(t, h0, h1), rim = Fix114 ? RimEase114(RimShare114(t, h0, h1)) : -1;", d);
            StringAssert.Contains("float c = h1 + LinesEnd * _p.Back, tt = h0 + (t - h0) * Mathf.Max(.1f, LineK);", d);   // the cuts' own end
            StringAssert.Contains("return saturate((_RvRim - k0 - sr) / 0.015 + 0.5);", shader);
            // the jumps: the press and the frame after the cuts take over step at most 1/60 s; the drawing ends with its last cut
            StringAssert.Contains("d._calm = Fix114;", d);
            StringAssert.Contains("if (Fix114) _calm = true;", d);
            StringAssert.Contains("float DrawEnd => _drawEnd > 0 ? _drawEnd : Spread + .05f + EdgeSeconds;", d);
            StringAssert.Contains("_drawEnd = last + EdgeSeconds;", d);
        }

        [Test]
        public void ThePensEaseInAndOutAndRunEvenInTheMiddle()
        {
            Assert.That(Dive75.RimEase114(0), Is.EqualTo(0).Within(1e-6f));
            Assert.That(Dive75.RimEase114(1), Is.EqualTo(1).Within(1e-5f));
            float prev = 0, h = 1e-3f;
            for (float x = h; x <= 1; x += h) { float e = Dive75.RimEase114(x); Assert.That(e, Is.GreaterThanOrEqualTo(prev - 1e-6f)); prev = e; }
            float V(float x) => (Dive75.RimEase114(x + h) - Dive75.RimEase114(x - h)) / (2 * h);
            Assert.That(V(.002f), Is.LessThan(.05f), "slow at the start");
            Assert.That(V(.998f), Is.LessThan(.05f), "slow at the end");
            Assert.That(V(.3f), Is.EqualTo(V(.7f)).Within(1e-2f), "even in the middle");
            Assert.That(V(.5f), Is.EqualTo(1 / (1 - .25f)).Within(1e-2f));
            // no step in the speed anywhere
            for (float x = .01f; x < .99f; x += .01f) Assert.That(Math.Abs(V(x + .01f) - V(x)), Is.LessThan(.1f), $"at {x:0.00}");
        }

        /* The rim's sides as the shader reads them: each pen's way from 0 to w + h covered once, no gap, no overlap, and
           each side drawn from where the pen reaches it. */
        [TestCase(3, 3)]
        [TestCase(4, 3)]
        [TestCase(5, 4)]
        [TestCase(2, 5)]
        public void TheTwoPensGoRoundTheRimOnce(int w, int h)
        {
            var spans = new List<(float a, float b, bool right)>();
            void Side(int x, int y, Dir d)
            {
                float st = Dive75.RimStart114(x, y, d, w, h);
                Assert.That(st, Is.LessThan(-.5f));
                float code = Mathf.Floor(-st - 1 + .5f), mode = Mathf.Floor(code / 1000), k0 = (code - mode * 1000) * .5f;
                // the side's ends in cells (y down), from its start along the shader's 'along' (x for U / D, y for L / R)
                Vector2 p0 = d == Dir.U ? new Vector2(x, 0) : d == Dir.D ? new Vector2(x, h) : d == Dir.L ? new Vector2(0, y) : new Vector2(w, y);
                Vector2 p1 = p0 + (d == Dir.U || d == Dir.D ? Vector2.right : Vector2.up);
                if (mode < 2)
                {
                    var first = mode < .5f ? p0 : p1;   // where the pen comes in
                    bool r = (p0 + p1).x / 2 > w / 2f;
                    Assert.That(Vector2.Distance(Dive75.RimPoint114(k0, r, w, h), first), Is.LessThan(1e-4f), $"{d} {x},{y} starts where its pen is");
                    Assert.That(Vector2.Distance(Dive75.RimPoint114(k0 + 1, r, w, h), mode < .5f ? p1 : p0), Is.LessThan(1e-4f), $"{d} {x},{y} ends where its pen is");
                    spans.Add((k0, k0 + 1, r));
                }
                else
                {
                    // the middle cell of an odd width: half for each pen
                    spans.Add((k0, k0 + .5f, true)); spans.Add((k0, k0 + .5f, false));
                    var mid = (p0 + p1) / 2;
                    Assert.That(Vector2.Distance(Dive75.RimPoint114(mode == 2 ? k0 : k0 + .5f, true, w, h), mid), Is.LessThan(1e-4f));
                }
            }
            for (int x = 0; x < w; x++) { Side(x, 0, Dir.U); Side(x, h - 1, Dir.D); }
            for (int y = 0; y < h; y++) { Side(0, y, Dir.L); Side(w - 1, y, Dir.R); }
            foreach (bool r in new[] { true, false })
            {
                var mine = spans.FindAll(s => s.right == r); mine.Sort((p, q) => p.a.CompareTo(q.a));
                float at = 0;
                foreach (var s in mine) { Assert.That(s.a, Is.EqualTo(at).Within(1e-4f), "no gap, no overlap"); at = s.b; }
                Assert.That(at, Is.EqualTo(w + h).Within(1e-4f), "round to the bottom's middle");
            }
        }
    }
}
