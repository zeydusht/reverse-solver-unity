using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ReverseSolver.Core;
using ReverseSolver.Presentation;
using UnityEngine;

namespace ReverseSolver.Core.Tests
{
    public sealed class T28ArtTests
    {
        [Test]
        public void SagAndPointsHangDown()
        {
            Assert.That(ChainCurve.Sag(Vector2.zero, new Vector2(0, 5), 1), Is.Zero);
            Assert.That(ChainCurve.Sag(Vector2.zero, new Vector2(2, 0), 1), Is.EqualTo(.2f).Within(.0001f));
            Assert.That(ChainCurve.Sag(Vector2.zero, new Vector2(8, 0), 1), Is.EqualTo(.4f).Within(.0001f));
            Assert.That(ChainCurve.Point(Vector2.zero, new Vector2(2, 0), .2f, .5f).y, Is.GreaterThan(0));
            var a = new Vector2(.5f, 3.5f); var b = new Vector2(2.5f, .5f);
            for (int i = 0; i <= 100; i++)
            {
                float t = i / 100f;
                Assert.That(ChainCurve.Point(a, b, ChainCurve.Sag(a, b, 1), t).y,
                            Is.GreaterThanOrEqualTo(Vector2.Lerp(a, b, t).y));
            }
        }

        [Test]
        public void SamplesHaveEqualSpacingAndExcludeEndpoints()
        {
            var a = Vector2.zero; var b = new Vector2(4, -2);
            var samples = ChainCurve.Samples(a, b, .35f, .22f);
            var points = new[] { a }.Concat(samples.Select(s => s.Position)).Concat(new[] { b }).ToArray();
            var gaps = Enumerable.Range(1, points.Length - 1).Select(i => Vector2.Distance(points[i - 1], points[i])).ToArray();
            Assert.That(gaps.Min(), Is.GreaterThan(0));
            Assert.That(gaps.Max() / gaps.Min(), Is.LessThan(1.01f));
            Assert.That(samples[0].Position, Is.Not.EqualTo(a));
            Assert.That(samples[samples.Length - 1].Position, Is.Not.EqualTo(b));
        }

        [Test]
        public void FitSagAvoidsOnlyNewOccupiedCells()
        {
            var a = new Vector2(.5f, .8f); var b = new Vector2(3.5f, .8f);
            float sag = ChainCurve.Sag(a, b, 1);
            Assert.That(ChainCurve.FitSag(a, b, sag, 1, new HashSet<Cell>()), Is.EqualTo(sag));
            Assert.That(ChainCurve.FitSag(a, b, sag, 1, new HashSet<Cell> { new Cell(1, 1) }), Is.LessThan(sag));
            Assert.That(ChainCurve.FitSag(a, b, sag, 1, new HashSet<Cell> { new Cell(1, 0) }), Is.EqualTo(sag));
        }

        [Test]
        public void ShowcaseParsesWithAllFourChainOrientationsAndCounters()
        {
            var asset = Resources.Load<TextAsset>("show_art");
            Assert.That(asset, Is.Not.Null);
            var set = LevelParser.Parse(asset.text);
            Assert.That(set.Levels.Count, Is.EqualTo(1));
            var level = set.Levels[0];
            Assert.That(level.Id, Is.EqualTo("SHOW_ART"));
            Assert.That((level.Width, level.Height, level.Version), Is.EqualTo((6, 8, 1)));
            var chains = level.Pieces.Where(p => p.Length == 2).Select(p =>
                (dx: System.Math.Abs(p[1].X - p[0].X), dy: System.Math.Abs(p[1].Y - p[0].Y))).ToArray();
            Assert.That(chains, Does.Contain((2, 0)));
            Assert.That(chains, Does.Contain((2, 2)));
            Assert.That(chains, Does.Contain((0, 3)));
            Assert.That(chains, Does.Contain((5, 0)));
            Assert.That(level.Pieces.Any(p => p.Length == 2 && (p[1].X - p[0].X) * (p[1].Y - p[0].Y) < 0), Is.True);
            Assert.That(level.Bombs.Count, Is.EqualTo(2));
            Assert.That(level.Nails.Count, Is.EqualTo(2));
            Assert.That(level.Bombs.Values, Is.EquivalentTo(new[] { 7, 3 }));
            Assert.That(level.Nails.Values, Is.EquivalentTo(new[] { 3, 1 }));
        }
    }
}
