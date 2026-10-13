using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using ReverseSolver.Core;
using ReverseSolver.Core.Json;
using ReverseSolver.Editing;
using ReverseSolver.Generation;

namespace ReverseSolver.Core.Tests
{
    /* G1 (PRODUCT.md "Faz 4 tanımı"): the peel generator. */
    public class GeneratorTests
    {
        static Catalog _catalog;
        static Catalog Catalog => _catalog ??= Catalog.From(TestData.Levels);

        static string Json(LevelDraft d)
        {
            var w = new JsonWriter();
            d.WriteJson(w);
            return w.ToString();
        }

        /* 1000 candidates spread over the 40 slots (25 seeds each). Every one the
           generator returns is solvable with 0 bombs / 0 freezes / 0 valves, keeps
           the slot's limits and survives a levels.json round trip. */
        [Test]
        public void ThousandCandidatesAreAllSolvableAndKeepTheirLimits()
        {
            var made = new List<Candidate>();
            var perKind = new Dictionary<Genre, (int n, double ms)>();
            int missing = 0;
            foreach (var spec in FactoryPlan.All())
                for (uint s = 1; s <= 25; s++)
                {
                    var sw = Stopwatch.StartNew();
                    var c = Generator.Make(spec, new GenParams { Interlock = (s % 5) * .5 }, s * 1000 + (uint)spec.Slot, 20, Catalog);
                    double ms = sw.Elapsed.TotalMilliseconds;
                    perKind.TryGetValue(spec.Genre, out var k);
                    perKind[spec.Genre] = (k.n + 1, k.ms + ms);
                    if (c == null) { missing++; continue; }
                    made.Add(c);
                }

            foreach (var c in made)
            {
                var l = c.Level;
                var spec = c.Spec;
                var smart = SmartPlayer.Play(l);
                Assert.That((smart.Verdict, smart.Valves), Is.EqualTo((SmartVerdict.Won, 0)), c.ToString());
                Assert.That(DesignCheck.Run(l).FollowsRule, Is.True, c.ToString());

                Assert.That(spec.Boards, Does.Contain((l.Width, l.Height)), c.ToString());
                Assert.That(l.Pieces.Count, Is.InRange(spec.MinBodies, spec.MaxBodies), c + " bodies");
                var st = LevelStats.Of(l);
                Assert.That(st.Nails, Is.InRange(spec.MinNails, spec.MaxNails), c + " nails");
                Assert.That(st.Bombs, Is.InRange(spec.MinBombs, spec.MaxBombs), c + " bombs");
                Assert.That(st.Chains, Is.InRange(spec.MinChains, spec.MaxChains), c + " chains");
                Assert.That(st.SealedLanes, Is.InRange(spec.MinSeals, spec.MaxSeals), c + " walls");
                if (spec.Slot < 12) Assert.That(st.Chains, Is.Zero, c + ": single-cell pieces before slot 12");
                if (spec.Slot < 20) Assert.That(st.Bombs, Is.Zero, c + ": no bomb before slot 20");
                foreach (var n in l.Nails) Assert.That(n.Value, Is.LessThanOrEqualTo(l.Pieces.Count * DifficultyModel.MaxNailShare), c + " nail share");
                Assert.That(Layout.CellSize(l.Width, l.Height), Is.GreaterThanOrEqualTo(44), c + " cell size");

                // levels.json round trip: same gameplay, same solution
                var parsed = LevelParser.Parse("{\"format\":1,\"levels\":[" + Json(c.Draft) + "]}").Levels[0];
                Assert.That(LevelDraft.FromLevel(parsed).GameplayKey(), Is.EqualTo(c.Draft.GameplayKey()), c + " round trip");
                Assert.That(parsed.Solution.Select(m => (m.Piece, m.Dir)), Is.EqualTo(l.Solution.Select(m => (m.Piece, m.Dir))), c + " solution");
            }

            var copy = new List<Candidate>(made);
            int dropped = 0;
            foreach (var g in copy.GroupBy(c => c.Spec.Slot)) { var list = g.ToList(); dropped += Similarity.Dedupe(list); }
            TestContext.WriteLine($"G1: {made.Count}/1000 made ({missing} slots-seeds failed 20 tries); look-alikes inside a slot: {dropped} " +
                                  $"({100.0 * dropped / made.Count:0.0} %, distance < {Similarity.DuplicateBelow})");
            foreach (var kv in perKind)
                TestContext.WriteLine($"  {kv.Key.Label(),-8} {kv.Value.n,4} tries  {1000.0 * kv.Value.n / kv.Value.ms:0.0} candidates/s");
            Assert.That(missing, Is.LessThanOrEqualTo(10), "the generator should almost always find a board");
        }

        [Test]
        public void SameSeedSameBoardByteForByte()
        {
            foreach (int slot in new[] { 1, 12, 30, 40 })
            {
                var spec = FactoryPlan.Slot(slot);
                var a = Generator.Make(spec, new GenParams(), 77, 20, Catalog);
                var b = Generator.Make(spec, new GenParams(), 77, 20, Catalog);
                Assert.That(a, Is.Not.Null, $"slot {slot}");
                Assert.That(Json(b.Draft), Is.EqualTo(Json(a.Draft)), $"slot {slot}");
                var other = Generator.Make(spec, new GenParams(), 78, 20, Catalog);
                Assert.That(Json(other.Draft), Is.Not.EqualTo(Json(a.Draft)), $"slot {slot}: another seed, another board");
            }
        }

        [Test]
        public void IntroductionSlotsShowTheirNewThingAlone()
        {
            var nail = Generator.Make(FactoryPlan.Slot(3), new GenParams(), 5, 20, Catalog);
            var st = LevelStats.Of(nail.Level);
            Assert.That((st.Nails, st.Bombs, st.Chains, st.SealedLanes), Is.EqualTo((1, 0, 0, 0)));
            Assert.That(nail.Level.Intro?.Title, Is.EqualTo("Çivili parça"));
            var bomb = Generator.Make(FactoryPlan.Slot(20), new GenParams(), 5, 20, Catalog);
            st = LevelStats.Of(bomb.Level);
            Assert.That((st.Nails, st.Bombs, st.Chains, st.SealedLanes), Is.EqualTo((0, 1, 0, 0)));
            var scissors = Generator.Make(FactoryPlan.Slot(5), new GenParams(), 5, 20, Catalog);
            Assert.That(scissors.Level.BoosterIntro?.Id, Is.EqualTo("scissors"));
            Assert.That(scissors.Level.Unlock["scissors"], Is.EqualTo(5), "the redesign opens the scissors at 5");
        }

        /* More interlock preference keeps more seams locked (less open). */
        [Test]
        public void InterlockKnobClosesSeams()
        {
            var spec = FactoryPlan.Slot(14);
            double Open(double k) => Enumerable.Range(1, 15).Select(s => Generator.Make(spec, new GenParams { Interlock = k }, (uint)s, 20, Catalog))
                                               .Where(c => c != null).Average(c => c.Level.Open);
            double loose = Open(0), tight = Open(3);
            TestContext.WriteLine($"slot 14 open seams: interlock 0 -> {loose:0.0} %, interlock 3 -> {tight:0.0} %");
            Assert.That(tight, Is.LessThan(loose));
        }

        [Test]
        public void SimilarityIgnoresMirrorsAndSeesDifferences()
        {
            var a = Generator.Make(FactoryPlan.Slot(14), new GenParams(), 9, 20, Catalog).Level;
            Assert.That(Similarity.Distance(a, a), Is.EqualTo(0));
            var mirrored = Mirror(a);
            Assert.That(Similarity.Distance(a, mirrored), Is.EqualTo(0).Within(1e-9), "a mirrored copy is the same board");
            var b = Generator.Make(FactoryPlan.Slot(14), new GenParams(), 10, 20, Catalog).Level;
            Assert.That(Similarity.Distance(a, b), Is.GreaterThan(Similarity.DuplicateBelow), "two seeds, two boards");
        }

        /* Left-right mirror through the editor model. */
        static LevelData Mirror(LevelData l)
        {
            var src = LevelDraft.FromLevel(l);
            var d = LevelDraft.New(l.Width, l.Height);
            int w = l.Width;
            for (int y = 0; y < l.Height; y++) for (int x = 1; x < w; x++) d.SetJoint(w - x, y, true, -src.VJoint(x, y));
            for (int y = 1; y < l.Height; y++) for (int x = 0; x < w; x++) d.SetJoint(w - 1 - x, y, false, src.HJoint(x, y));
            foreach (var p in l.Pieces)
            {
                var m = p.Select(c => new Cell(w - 1 - c.X, c.Y)).ToArray();
                if (m.Length == 2) d.AddChain(m[0], m[1]);
            }
            for (int p = 0; p < l.Pieces.Count; p++)
            {
                var c = new Cell(w - 1 - l.Pieces[p][0].X, l.Pieces[p][0].Y);
                if (l.Nails.TryGetValue(p, out int n)) d.SetNail(c, n);
                if (l.Bombs.TryGetValue(p, out int f)) d.SetBomb(c, f);
            }
            foreach (var kv in l.Sealed)
                foreach (int lane in kv.Value)
                {
                    var side = kv.Key.IsVertical() ? kv.Key : kv.Key.Opposite();
                    d.SetSealed(side, kv.Key.IsVertical() ? w - 1 - lane : lane, true);
                }
            return d.ToLevelData();
        }

        [Test]
        public void GenerationIsEditorOnly()
        {
            var asmdef = File.ReadAllText("Assets/_Game/Generation/ReverseSolver.Generation.asmdef");
            Assert.That(asmdef, Does.Contain("\"includePlatforms\": [\n        \"Editor\"\n    ]").Or.Contain("\"includePlatforms\": [\r\n        \"Editor\"\r\n    ]"));
            Assert.That(File.ReadAllText("Assets/_Game/Presentation/ReverseSolver.Presentation.asmdef"), Does.Not.Contain("Generation"));
            Assert.That(File.ReadAllText("Assets/_Game/Platform/ReverseSolver.Platform.asmdef"), Does.Not.Contain("Generation"));
        }
    }
}
