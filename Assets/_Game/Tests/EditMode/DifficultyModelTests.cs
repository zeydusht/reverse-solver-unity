using System.Linq;
using NUnit.Framework;
using ReverseSolver.Editing;

namespace ReverseSolver.Core.Tests
{
    /* A8 difficulty model (PRODUCT.md "Zorluk modeli (başlangıç değerleri)"),
       hand-computed values. */
    public class DifficultyModelTests
    {
        [Test]
        public void TimeFormulaRoundsUpToFive()
        {
            Assert.That(DifficultyModel.Time(34, 1.2, 1.3), Is.EqualTo(55), "34 x 1.2 x 1.3 = 53.04 -> 55");
            Assert.That(DifficultyModel.Time(25, 1.0, 2.0), Is.EqualTo(50), "exactly 50 stays 50");
            Assert.That(DifficultyModel.Ceil5(50.01), Is.EqualTo(55));
        }

        [Test]
        public void SecondsPerPieceFromStructure()
        {
            Assert.That(DifficultyModel.SecondsPerPiece(2.5, false, false), Is.EqualTo(1.0).Within(1e-9), "0.6 + 0.16 x 2.5");
            Assert.That(DifficultyModel.SecondsPerPiece(2.5, true, false), Is.EqualTo(1.1).Within(1e-9), "+0.1 with a bomb");
            Assert.That(DifficultyModel.SecondsPerPiece(4.0, true, true), Is.EqualTo(1.35), "the game's first level");
        }

        [Test]
        public void StarsAndMedianTimeLeft()
        {
            Assert.That(DifficultyModel.Stars(.45), Is.EqualTo(3));
            Assert.That(DifficultyModel.Stars(.449), Is.EqualTo(2));
            Assert.That(DifficultyModel.Stars(.20), Is.EqualTo(2));
            Assert.That(DifficultyModel.Stars(.199), Is.EqualTo(1));
            // T = g * s * P exactly: the median player keeps 1 - 1/P
            Assert.That(DifficultyModel.MedianTimeLeft(25, 1.0, 50), Is.EqualTo(1 - 1 / 2.0).Within(1e-9));
            Assert.That(DifficultyModel.MedianTimeLeft(30, 1.1, 66), Is.EqualTo(.5).Within(1e-9));
        }

        [Test]
        public void FuseIsShortestDigPlusTheKindsMargin()
        {
            Assert.That(DifficultyModel.Fuse(6, Genre.Zor), Is.EqualTo(11));
            Assert.That(DifficultyModel.Fuse(6, Genre.CokZor), Is.EqualTo(8));
            Assert.That(DifficultyModel.Fuse(6, Genre.Tanitim), Is.EqualTo(16));
        }

        [Test]
        public void NailCountersStayUnderFortyPercent()
        {
            Assert.That(DifficultyModel.NailCounter(30, Genre.Kolay, 0), Is.EqualTo(7), "22.5 % of 30 = 6.75");
            Assert.That(DifficultyModel.NailCounter(30, Genre.Orta, 0), Is.EqualTo(9), "30 %");
            Assert.That(DifficultyModel.NailCounter(36, Genre.Zor, 0), Is.EqualTo(10), "27.5 % of 36 = 9.9");
            Assert.That(DifficultyModel.NailCounter(36, Genre.Zor, 1), Is.EqualTo(14), "37.5 % of 36 = 13.5");
            foreach (var g in GenreExt.All)
                for (int i = 0; i < 3; i++)
                    Assert.That(DifficultyModel.NailCounter(40, g, i), Is.LessThanOrEqualTo(40 * DifficultyModel.MaxNailShare), $"{g} nail {i}");
        }

        /* 3x3, bomb fuse 3 in the middle: shortest dig 2 (one neighbour, then the bomb). */
        [Test]
        public void BombBoardSuggestsDigPlusMargin()
        {
            var d = LevelDraft.New(3, 3);
            d.SetBomb(new Cell(1, 1), 3);
            var l = d.ToLevelData();
            var m = StructureMetrics.Of(l);
            Assert.That(m.BombDigs.Values.Single().dig, Is.EqualTo(2));
            var r = DifficultyModel.Analyze(l, Genre.Zor, m, 0);
            Assert.That(r.Fuses.Single().suggested, Is.EqualTo(2 + 5));
            Assert.That(r.Measures.Single(x => x.Name == "Bomba payı").Value, Is.EqualTo("1"));
        }

        /* 2x2 with a nail: reading per move = (5/4 + 4/3 + 1 + 1) / 4, one nail suggestion. */
        [Test]
        public void NailedBoardReadingAndNailSuggestion()
        {
            var d = LevelDraft.New(2, 2);
            d.SetNail(new Cell(0, 0), 2);
            var l = d.ToLevelData();
            var m = StructureMetrics.Of(l);
            Assert.That(m.ReadingPerMove, Is.EqualTo((1.25 + 4.0 / 3 + 1 + 1) / 4).Within(1e-9));
            Assert.That(m.FreeAtStart, Is.EqualTo(3));
            var r = DifficultyModel.Analyze(l, Genre.Kolay, m, 1);
            Assert.That(r.Nails.Single(), Is.EqualTo((0, 2, 1)), "22.5 % of 4 pieces rounds to 1");
            Assert.That(r.S, Is.EqualTo(.6 + .16 * m.ReadingPerMove).Within(1e-9));
        }

        [Test]
        public void WarningsForLongTimeAndHeavyNails()
        {
            var d = LevelDraft.New(4, 4);
            d.SetTimer(90);
            d.SetNail(new Cell(0, 0), 8);           // 8 of 16 pieces = 50 %
            var l = d.ToLevelData();
            var r = DifficultyModel.Analyze(l, Genre.Orta, StructureMetrics.Of(l), 1);
            Assert.That(r.Warnings.Any(w => w.Contains("75 sn")), Is.True);
            Assert.That(r.Warnings.Any(w => w.Contains("%40")), Is.True);
            Assert.That(r.SuggestedTime, Is.LessThanOrEqualTo(DifficultyModel.MaxTime));
        }

        [Test]
        public void GenreIsLookOnlyAndStored()
        {
            var store = new DesignStore();
            var d = LevelDraft.New(5, 6);
            store.Save(d);
            d.Genre = Genre.Zor;
            store.Save(d);
            Assert.That(d.Version, Is.EqualTo(1), "the kind never raises the version");
            Assert.That(DesignStore.Parse(store.ToJson()).Find(d.Id).Genre, Is.EqualTo(Genre.Zor));
            d.SetTimer(d.Timer + 5);                  // what "Uygula" does for the time
            store.Save(d);
            Assert.That(d.Version, Is.EqualTo(2), "applying a suggestion changes gameplay");
        }

        [Test]
        public void LargestWebLevelIsQuick()
        {
            var l = TestData.Levels["L40"];
            MonteCarlo.Run(l, 5);
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var mc = MonteCarlo.Run(l, MonteCarlo.DefaultRuns);
            var r = DifficultyModel.Analyze(l, Genre.CokZor, mc.Structure, mc.WinRate);
            double ms = sw.Elapsed.TotalMilliseconds;
            TestContext.WriteLine($"L40 structure + 200 games + model: {ms:0} ms; suggested {r.SuggestedTime} s, s {r.S:0.00}");
            Assert.That(ms, Is.LessThan(1000));
        }
    }
}
