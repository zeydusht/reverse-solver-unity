using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ReverseSolver.Core;
using ReverseSolver.Editing;
using ReverseSolver.Generation;

namespace ReverseSolver.Core.Tests
{
    /* G2 (PRODUCT.md "Faz 4"): calibration, the band filter and the unfair list. */
    public class CalibrationTests
    {
        static Catalog _catalog;
        static Catalog Catalog => _catalog ??= Catalog.From(TestData.Levels);

        static Candidate Hand(LevelDraft d, SlotSpec spec)
        {
            d.SetNumber(spec.Slot);
            var l = d.ToLevelData();
            var check = DesignCheck.Run(l);
            d.Solution = check.Solution;
            return new Candidate { Id = "HAND", Spec = spec, Draft = d, Level = d.ToLevelData() };
        }

        static List<string> Unfair(LevelDraft d, SlotSpec spec, int formulaTime = 0, double s = 1.0)
        {
            d.SetNumber(spec.Slot);
            var l = d.ToLevelData();
            return Calibration.UnfairReasons(l, spec, StructureMetrics.Of(l), formulaTime == 0 ? l.TimerSeconds : formulaTime, s);
        }

        // ---- the unfair list, one item each ------------------------------------------------------

        [Test]
        public void UnfairBombMarginZero()
        {
            var d = LevelDraft.New(4, 5);
            d.SetTimer(60);
            d.SetBomb(new Cell(1, 1), 2);               // two moves to dig it out, fuse 2: margin 0
            var u = Unfair(d, SlotSpec.For(25, Genre.Zor));
            Assert.That(u, Has.Some.Contains("bomba payı 0"));
            d.SetBomb(new Cell(1, 1), 3);
            Assert.That(Unfair(d, SlotSpec.For(25, Genre.Zor)), Has.None.Contains("bomba payı"));
        }

        [Test]
        public void UnfairTimeShareUnderOnePointOne()
        {
            var d = LevelDraft.New(4, 5);
            d.SetTimer(20);                              // 20 pieces x 1 s: share 1.0
            Assert.That(Unfair(d, SlotSpec.For(10, Genre.Zor)), Has.Some.Contains("süre payı 1,00").Or.Some.Contains("süre payı 1.00"));
            d.SetTimer(25);
            Assert.That(Unfair(d, SlotSpec.For(10, Genre.Zor)), Has.None.Contains("1,1'in altı"));
        }

        [Test]
        public void UnfairTimerOverSeventyFive()
        {
            var d = LevelDraft.New(4, 5);
            d.SetTimer(75);
            Assert.That(Unfair(d, SlotSpec.For(10, Genre.Zor), formulaTime: 80), Has.Some.Contains("75'ten uzun"));
            Assert.That(Unfair(d, SlotSpec.For(10, Genre.Zor), formulaTime: 75), Has.None.Contains("75'ten uzun"));
        }

        /* K1 layout on a 375 x 667 phone: 8 columns leave 42 pt cells. */
        [Test]
        public void UnfairCellsUnderFortyFourPoints()
        {
            var d = LevelDraft.New(8, 7);
            d.SetTimer(75);
            Assert.That(Layout.CellSize(8, 7), Is.LessThan(44));
            Assert.That(Unfair(d, SlotSpec.For(10, Genre.Zor)), Has.Some.Contains("44'ün altı"));
            Assert.That(Unfair(LevelDraft.New(6, 7), SlotSpec.For(10, Genre.Zor)), Has.None.Contains("44'ün altı"), "6 x 7 keeps 51 pt");
        }

        [Test]
        public void UnfairThingBeforeItsSlotAndTwoNewThings()
        {
            var d = LevelDraft.New(4, 5);
            d.SetTimer(60);
            d.SetNail(new Cell(1, 1), 3);
            Assert.That(Unfair(d, SlotSpec.For(2, Genre.Kolay)), Has.Some.Contains("nail slot 3'den önce"));
            var two = LevelDraft.New(4, 5);
            two.SetTimer(60);
            two.SetBomb(new Cell(1, 1), 12);
            var spec = SlotSpec.For(20, Genre.Tanitim, SlotSpec.Bomb);
            spec.NewThing = SlotSpec.Hammer;                // a booster introduced next to the bomb: two new things
            Assert.That(Unfair(two, spec), Has.Some.Contains("2 yeni şey"));
        }

        [Test]
        public void UnfairIntroductionUnderTwiceTheTime()
        {
            var d = LevelDraft.New(4, 5);
            d.SetTimer(30);                              // 20 pieces x 1 s: share 1.5
            Assert.That(Unfair(d, SlotSpec.For(1, Genre.Tanitim)), Has.Some.Contains("tanıtımda süre payı"));
            d.SetTimer(40);
            Assert.That(Unfair(d, SlotSpec.For(1, Genre.Tanitim)), Has.None.Contains("tanıtımda"));
        }

        /* The bomb introduction: the bomb 2-3 moves away. A corner bomb (dig 1) is refused. */
        [Test]
        public void BombIntroductionWantsADigOfTwoToThree()
        {
            var corner = LevelDraft.New(4, 5);
            corner.SetTimer(60);
            corner.SetBomb(new Cell(0, 0), 12);
            var a = Calibration.Assess(Hand(corner, SlotSpec.For(20, Genre.Tanitim, SlotSpec.Bomb)));
            Assert.That(a.Unfair, Has.Some.Contains("1 hamle uzakta"));
            var inner = LevelDraft.New(4, 5);
            inner.SetTimer(60);
            inner.SetBomb(new Cell(1, 1), 12);           // one neighbour out, then the bomb: dig 2
            a = Calibration.Assess(Hand(inner, SlotSpec.For(20, Genre.Tanitim, SlotSpec.Bomb)));
            Assert.That(a.Unfair, Has.None.Contains("hamle uzakta"));
            Assert.That(a.Candidate.Level.Bombs.Values.Single(), Is.EqualTo(2 + 10), "fuse = dig 2 + margin 10");
        }

        // ---- calibration ----------------------------------------------------------------------------

        [Test]
        public void FuseIsShortestDigPlusMarginAndTimerIsTheFormula()
        {
            int checkedBombs = 0;
            for (uint seed = 1; seed <= 6; seed++)
            {
                var c = Generator.Make(FactoryPlan.Slot(25), new GenParams(), seed, 20, Catalog);
                var a = Calibration.Assess(c);
                var l = a.Candidate.Level;
                var m = StructureMetrics.Of(l);
                foreach (var b in l.Bombs) { Assert.That(b.Value, Is.EqualTo(m.BombDigs[b.Key].dig + 5), c.Id); checkedBombs++; }
                double s = DifficultyModel.SecondsPerPiece(m.ReadingPerMove, l.Bombs.Count > 0, false);
                Assert.That(a.FormulaTime, Is.EqualTo(DifficultyModel.Time(l.Pieces.Count, s, 1.3)), c.Id);
                Assert.That(l.TimerSeconds, Is.EqualTo(System.Math.Min(a.FormulaTime, 75)), c.Id);
                if (a.Unfair.All(u => !u.StartsWith("ayardan"))) Assert.That(DesignCheck.Run(l).FollowsRule, Is.True, c.Id + " still solvable");
            }
            Assert.That(checkedBombs, Is.GreaterThan(0));
        }

        /* A row tied to an obstacle only applies when the level has it. */
        [Test]
        public void BombFreeZorIsNotDroppedForTheBombMargin()
        {
            var c = Generator.Make(FactoryPlan.Slot(14), new GenParams(), 4, 20, Catalog);
            Assert.That(c.Level.Bombs, Is.Empty);
            var a = Calibration.Assess(c);
            Assert.That(a.OutOfBand, Has.None.Contains("Bomba payı"));
            Assert.That(a.Report.Measures.Select(m => m.Name), Has.None.EqualTo("Bomba payı"));
        }

        [Test]
        public void EveryGeneratedLookIsReadable()
        {
            var excluded = new List<string>();
            foreach (var (w, h) in new[] { (4, 5), (5, 5), (5, 6), (6, 6), (6, 7) })
                for (int p = 0; p < Catalog.Palettes.Length; p++)
                    for (int t = 0; t < Catalog.Patterns; t++)
                    {
                        float r = Calibration.Readability(w, h, Catalog.Paint(w, h, t), Catalog.PaletteOf(p));
                        if (r < Calibration.MinReadability) excluded.Add($"{w}x{h} {Catalog.Palettes[p].a}/{Catalog.Palettes[p].b} {Catalog.PatternNames[t]} {r:0.00}");
                    }
            TestContext.WriteLine($"palette x pattern under {Calibration.MinReadability}: {excluded.Count} of {5 * Catalog.Palettes.Length * Catalog.Patterns}");
            foreach (var e in excluded) TestContext.WriteLine("  " + e);
            foreach (int slot in new[] { 1, 6, 14, 30, 40 })
                for (uint s = 1; s <= 10; s++)
                {
                    var c = Generator.Make(FactoryPlan.Slot(slot), new GenParams(), s, 20, Catalog);
                    Assert.That(Calibration.Readability(c.Level), Is.GreaterThanOrEqualTo(Calibration.MinReadability), c.Id);
                }
        }

        // ---- the band filter over the plan ----------------------------------------------------------------

        /* Pass rate of the band filter per slot: 25 candidates from the slot's search
           (random knobs, then hill climbing). Under 20 % marks a "zor slot adayı"
           (it does not stop G4). */
        [Test]
        public void BandPassRateBySlot()
        {
            var hard = new List<string>();
            int total = 0, passed = 0;
            foreach (var spec in FactoryPlan.All())
            {
                var r = Search.Run(spec, 1, 25, int.MaxValue, Catalog);
                int ok = r.BandPassed;
                total += r.Generated; passed += ok;
                TestContext.WriteLine($"slot {spec.Slot,2} {spec.Genre.Label(),-8} {ok,2}/{r.Generated} geçti ({r.PassRate * 100:0} %){(r.PassRate < .2 ? "  ← zor slot adayı" : "")}" +
                                      (ok == 0 && r.Best != null ? "  en yakın: " + string.Join(", ", r.Best.OutOfBand.Concat(r.Best.Unfair)) : ""));
                if (r.PassRate < .2) hard.Add($"{spec.Slot}");
            }
            TestContext.WriteLine($"toplam {passed}/{total}; zor slot adayları: {(hard.Count == 0 ? "yok" : string.Join(", ", hard))}");
            Assert.That(total, Is.GreaterThan(900));
        }

        /* A1: the time-share and median rows never filter a factory candidate. */
        [Test]
        public void RoundingRowsAreInformationOnly()
        {
            var r = new DifficultyModel.Report();
            r.Measures.Add(new DifficultyModel.Measure { Name = "Süre payı", Value = "1,23", BandText = "1,1–1,2", HasBand = true, InBand = false });
            r.Measures.Add(new DifficultyModel.Measure { Name = "Medyan oyuncunun kalan süresi", Value = "%18", BandText = "%9–17", HasBand = true, InBand = false });
            r.Measures.Add(new DifficultyModel.Measure { Name = "Hamle başına okuma", Value = "4,2", BandText = "4,0+", HasBand = true, InBand = true });
            Assert.That(Calibration.Filter(r), Is.Empty, "a Çok zor candidate outside only on the rounding rows passes");
            r.Measures.Add(new DifficultyModel.Measure { Name = "Darboğaz", Value = "2", BandText = "3+", HasBand = true, InBand = false });
            Assert.That(Calibration.Filter(r), Has.Count.EqualTo(1).And.Some.StartsWith("Darboğaz"));
        }

        static LevelCurve CurveWithFit(double fit) => new LevelCurve { Fit = fit, Defined = true };

        [Test]
        public void CurveAddsToTheDistanceOnlyForHarderKinds()
        {
            Assert.That(Calibration.CurveTerm(Genre.Zor, CurveWithFit(.5)), Is.EqualTo(.5).Within(1e-9), "(0.6 - 0.5) / 0.2");
            Assert.That(Calibration.CurveTerm(Genre.Zor, CurveWithFit(.7)), Is.EqualTo(0));
            Assert.That(Calibration.CurveTerm(Genre.CokZor, CurveWithFit(.2)), Is.EqualTo(2).Within(1e-9));
            foreach (var g in new[] { Genre.Tanitim, Genre.Nefes, Genre.Kolay })
                Assert.That(Calibration.CurveTerm(g, CurveWithFit(0)), Is.EqualTo(0), g.ToString());
            Assert.That(Calibration.CurveTerm(Genre.Zor, new LevelCurve { Defined = false }), Is.EqualTo(0), "eğri yok: no penalty");
        }

        [Test]
        public void SearchIsDeterministicAndKeepsOnlyPassingDistinctBoards()
        {
            var spec = FactoryPlan.Slot(8);
            var a = Search.Run(spec, 3, 60, 4, Catalog);
            var b = Search.Run(spec, 3, 60, 4, Catalog);
            Assert.That(b.Passed.Select(x => x.Candidate.Id), Is.EqualTo(a.Passed.Select(x => x.Candidate.Id)));
            Assert.That(a.Passed, Is.Not.Empty);
            foreach (var x in a.Passed) Assert.That((x.Passed, Search.BandDistance(x)), Is.EqualTo((true, 0.0)), x.ToString());
            for (int i = 0; i < a.Passed.Count; i++)
                for (int k = i + 1; k < a.Passed.Count; k++)
                    Assert.That(Similarity.IsDuplicate(a.Passed[i].Candidate.Level, a.Passed[k].Candidate.Level), Is.False);
        }
    }
}
