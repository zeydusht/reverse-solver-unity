using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ReverseSolver.Editing;
using ReverseSolver.Core.Json;

namespace ReverseSolver.Core.Tests
{
    /* The level editor's model (PRODUCT.md, "Seviye editörü tanımı",
       criteria 2-5). */
    public class LevelEditorTests
    {
        static LevelData Reparse(LevelDraft d)
        {
            var w = new JsonWriter();
            d.WriteJson(w);
            return LevelParser.Parse("{\"format\":1,\"levels\":[" + w + "]}").Levels[0];
        }

        static void AssertSame(LevelData a, LevelData b)
        {
            string at = a.Id;
            Assert.That(b.Id, Is.EqualTo(a.Id), at);
            Assert.That(b.Version, Is.EqualTo(a.Version), at);
            Assert.That(b.Number, Is.EqualTo(a.Number), at);
            Assert.That((b.Width, b.Height, b.TimerSeconds), Is.EqualTo((a.Width, a.Height, a.TimerSeconds)), at);
            Assert.That(b.Pieces.Count, Is.EqualTo(a.Pieces.Count), at);
            for (int p = 0; p < a.Pieces.Count; p++) Assert.That(b.Pieces[p], Is.EqualTo(a.Pieces[p]), $"{at} piece {p}");
            for (int y = 0; y < a.Height; y++)
                for (int x = 1; x < a.Width; x++) Assert.That(b.VJoint(x, y), Is.EqualTo(a.VJoint(x, y)), $"{at} v{x},{y}");
            for (int y = 1; y < a.Height; y++)
                for (int x = 0; x < a.Width; x++) Assert.That(b.HJoint(x, y), Is.EqualTo(a.HJoint(x, y)), $"{at} h{x},{y}");
            Assert.That(Sealed(b), Is.EqualTo(Sealed(a)), at);
            Assert.That(b.Nails, Is.EquivalentTo(a.Nails), at);
            Assert.That(b.Bombs, Is.EquivalentTo(a.Bombs), at);
            Assert.That(b.Solution.Select(m => m.ToString()), Is.EqualTo(a.Solution.Select(m => m.ToString())), at);
            Assert.That(b.Colors, Is.EqualTo(a.Colors), at);
            Assert.That(b.Palette, Is.EqualTo(a.Palette), at);
            Assert.That(b.Art, Is.EqualTo(a.Art), at);
            Assert.That(b.Image, Is.EqualTo(a.Image), at);
            Assert.That(Intro(b.Intro), Is.EqualTo(Intro(a.Intro)), at);
            Assert.That(Intro(b.BoosterIntro), Is.EqualTo(Intro(a.BoosterIntro)), at);
            Assert.That(b.Boosters, Is.EquivalentTo(a.Boosters), at);
            Assert.That(b.Unlock, Is.EquivalentTo(a.Unlock), at);
            Assert.That((b.Load, b.Open), Is.EqualTo((a.Load, a.Open)), at);
        }

        static string Sealed(LevelData l) => string.Join(" ",
            DirExt.All.Where(d => l.Sealed.TryGetValue(d, out var s) && s.Length > 0)
                      .Select(d => d + ":" + string.Join(",", l.Sealed[d].OrderBy(x => x))));

        static string Intro(LevelIntro i) => i == null ? "null" : $"{i.Id}|{i.Icon}|{i.Title}|{i.Body}|{i.Tip}";

        // ---- criterion 2: round trip ---------------------------------------------------

        [Test]
        public void EveryWebLevelRoundTripsUnchanged()
        {
            foreach (var l in TestData.Levels.Levels)
                AssertSame(l, Reparse(LevelDraft.FromLevel(l)));
        }

        [Test]
        public void DraftToLevelDataMatchesTheSourceLevel()
        {
            foreach (var l in TestData.Levels.Levels)
                AssertSame(l, LevelDraft.FromLevel(l).ToLevelData());
        }

        [Test]
        public void DesignFileRoundTrips()
        {
            var store = new DesignStore();
            foreach (var d in SampleDesigns()) store.Save(d);
            var again = DesignStore.Parse(store.ToJson());
            Assert.That(again.NextId, Is.EqualTo(store.NextId));
            Assert.That(again.Designs.Select(d => d.Id), Is.EqualTo(store.Designs.Select(d => d.Id)));
            for (int i = 0; i < store.Designs.Count; i++)
                AssertSame(store.Designs[i].ToLevelData(), again.Designs[i].ToLevelData());
            Assert.That(again.ToJson(), Is.EqualTo(store.ToJson()));
        }

        [Test]
        public void EmptyDesignFileLoads()
        {
            var empty = DesignStore.Parse(new DesignStore().ToJson());
            Assert.That(empty.Designs, Is.Empty);
            Assert.That(empty.ToLevelSet(), Is.Null);
            Assert.That(DesignStore.Parse("").Designs, Is.Empty);
        }

        [Test]
        public void SavingNeverTouchesWebLevelIds()
        {
            var store = new DesignStore();
            var copy = LevelDraft.FromLevel(TestData.Levels["L40"]);
            copy.Id = null;                                   // "open as a copy"
            store.Save(copy);
            Assert.That(copy.Id, Is.EqualTo("D01"));
            Assert.That(copy.Version, Is.EqualTo(1));
        }

        // ---- criterion 3: the editor agrees with GameSession + SmartPlayer ---------

        [Test]
        public void VerdictMatchesSmartPlayerOnWebLevels()
        {
            foreach (var l in TestData.Levels.Levels)
            {
                var check = DesignCheck.Run(l);
                Assert.That(check.Verdict, Is.EqualTo(Expected(l)), l.Id);
                Assert.That(DesignCheck.Run(l, DesignCheck.QuickBudget).Verdict, Is.EqualTo(check.Verdict), l.Id + " quick budget");
                Assert.That(check.FollowsRule, Is.True, l.Id + ": " + check.Message);   // README: 40/40 by the rule
            }
        }

        [Test]
        public void VerdictMatchesSmartPlayerOnDesigns()
        {
            var designs = SampleDesigns();
            var verdicts = designs.Select(d => DesignCheck.Run(d.ToLevelData()).Verdict).ToList();
            for (int i = 0; i < designs.Count; i++)
                Assert.That(verdicts[i], Is.EqualTo(Expected(designs[i].ToLevelData())), designs[i].Art);
            Assert.That(verdicts, Is.EqualTo(new[] { DesignVerdict.Solvable, DesignVerdict.Frozen, DesignVerdict.Bomb, DesignVerdict.Solvable }));
        }

        [Test]
        public void SolutionPlaysToAWin()
        {
            foreach (var d in SampleDesigns())
            {
                var l = d.ToLevelData();
                var r = DesignCheck.Run(l);
                if (!r.FollowsRule) continue;
                var s = new GameSession(l, 1, "test", new Mulberry32(1));
                foreach (var m in r.Solution) Assert.That(s.TryExit(m.Piece, m.Dir), Is.EqualTo(CommandResult.Ok));
                Assert.That(s.Outcome, Is.EqualTo(Outcome.Win));
            }
        }

        [Test]
        public void FrozenReportNamesTheStuckPieces()
        {
            var r = DesignCheck.Run(SampleDesigns()[1].ToLevelData());
            Assert.That(r.Pieces, Is.Not.Empty);
            Assert.That(r.Message, Does.StartWith("Çözülemez"));
        }

        [Test]
        public void FrozenStructureIsReportedAsFrozenEvenWithABomb()
        {
            var d = LevelDraft.New(3, 3);
            foreach (var side in DirExt.All) for (int lane = 0; lane < 3; lane++) d.SetSealed(side, lane, true);
            d.SetBomb(new Cell(1, 1), 3);
            var r = DesignCheck.Run(d.ToLevelData());
            Assert.That(r.Verdict, Is.EqualTo(DesignVerdict.Frozen));
            Assert.That(r.Pieces.Count, Is.EqualTo(9));
        }

        /* Criterion 8: the live panel updates within a second on the largest board. */
        [Test]
        public void QuickCheckIsFastOnTheLargestWebLevel()
        {
            var l = TestData.Levels["L40"];
            DesignCheck.Run(l, DesignCheck.QuickBudget);
            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 5; i++) DesignCheck.Run(LevelDraft.FromLevel(l).ToLevelData(), DesignCheck.QuickBudget);
            double ms = sw.Elapsed.TotalMilliseconds / 5;
            TestContext.WriteLine($"L40 draft -> level -> check: {ms:0.0} ms");
            Assert.That(ms, Is.LessThan(1000));
        }

        [Test]
        public void BombReportNamesTheBombPiece()
        {
            var d = SampleDesigns()[2];
            var l = d.ToLevelData();
            var r = DesignCheck.Run(l);
            Assert.That(r.Pieces, Is.EqualTo(new[] { l.Bombs.Keys.Single() }));
        }

        /* GameSession + SmartPlayer, after the web game's own solvable() for a frozen structure. */
        static DesignVerdict Expected(LevelData l) =>
            !GreedySolver.Solve(l).Solved ? DesignVerdict.Frozen : Expected(SmartPlayer.Play(l));

        static DesignVerdict Expected(SmartPlayResult r) => r.Verdict switch
        {
            SmartVerdict.Won => r.Valves == 0 ? DesignVerdict.Solvable : DesignVerdict.NeedsValve,
            SmartVerdict.Frozen => DesignVerdict.Frozen,
            SmartVerdict.LosesBomb => DesignVerdict.Bomb,
            _ => DesignVerdict.Unverified
        };

        /* A playable 5x6, a walled-in board (frozen), a bomb that cannot be
           reached in time, and a 5x6 with every obstacle type. */
        static List<LevelDraft> SampleDesigns()
        {
            var plain = LevelDraft.New(5, 6);
            plain.Art = "plain";
            plain.ApplyTemplate(new Cell(2, 2), PieceTemplate.All[5]);

            var walled = LevelDraft.New(3, 3);
            walled.Art = "walled";
            foreach (var d in DirExt.All)
                for (int lane = 0; lane < 3; lane++) walled.SetSealed(d, lane, true);

            var bomb = LevelDraft.New(3, 3);
            bomb.Art = "bomb";
            bomb.SetBomb(new Cell(1, 1), 1);

            var mixed = LevelDraft.New(5, 6);
            mixed.Art = "mixed";
            mixed.SetNail(new Cell(0, 0), 3);
            mixed.SetBomb(new Cell(4, 5), 12);
            mixed.SetSealed(Dir.U, 2, true);
            mixed.AddChain(new Cell(1, 1), new Cell(3, 4));
            mixed.ApplyTemplate(new Cell(2, 2), PieceTemplate.All[3]);
            return new List<LevelDraft> { plain, walled, bomb, mixed };
        }

        // ---- criterion 4: no inconsistent board ----------------------------------------

        [Test]
        public void TemplateSidesMatchTheirNeighbours()
        {
            var d = LevelDraft.New(4, 4);
            var c = new Cell(1, 1);
            foreach (var t in PieceTemplate.All)
            {
                d.ApplyTemplate(c, t);
                foreach (var dir in DirExt.All)
                {
                    Assert.That(d.SideOf(c, dir), Is.EqualTo(t.Get(dir)), $"{t.Name} {dir}");
                    var n = c.Step(dir);
                    Assert.That((int)d.SideOf(n, dir.Opposite()), Is.EqualTo(-(int)t.Get(dir)), $"{t.Name} neighbour {dir}");
                }
            }
        }

        [Test]
        public void OuterSidesStayFlat()
        {
            var d = LevelDraft.New(3, 3);
            Assert.That(d.ApplyTemplate(new Cell(0, 0), PieceTemplate.All[5]), Is.EqualTo(2));
            Assert.That(d.SideOf(new Cell(0, 0), Dir.L), Is.EqualTo(Side.Flat));
            Assert.That(d.SideOf(new Cell(0, 0), Dir.U), Is.EqualTo(Side.Flat));
            Assert.That(d.SetSide(new Cell(0, 0), Dir.L, Side.Tab).Ok, Is.False);
            Assert.That(d.CycleJoint(0, 0, true).Ok, Is.False);
        }

        [Test]
        public void SidesReadTheWayTheRuleReadsThem()
        {
            foreach (var l in TestData.Levels.Levels)
            {
                var d = LevelDraft.FromLevel(l);
                var b = new Board(l);
                for (int y = 0; y < l.Height; y++)
                    for (int x = 0; x < l.Width; x++)
                        foreach (var dir in DirExt.All)
                            Assert.That((int)d.SideOf(new Cell(x, y), dir), Is.EqualTo(TravelRule.EdgeOf(b, x, y, dir) == 1 ? 1 : TravelRule.EdgeOf(b, x, y, dir) == -1 ? -1 : 0), $"{l.Id} {x},{y} {dir}");
            }
        }

        [Test]
        public void CycleGoesFlatTabTabFlat()
        {
            var d = LevelDraft.New(3, 3);
            var seen = new List<int>();
            for (int i = 0; i < 3; i++) { d.CycleJoint(1, 0, true); seen.Add(d.VJoint(1, 0)); }
            Assert.That(seen, Is.EqualTo(new[] { 1, -1, 0 }));
        }

        [Test]
        public void ChainRules()
        {
            var d = LevelDraft.New(4, 4);
            Cell a = new Cell(0, 0), b = new Cell(3, 3), c = new Cell(1, 2);
            Assert.That(d.AddChain(a, a).Ok, Is.False, "one cell");
            Assert.That(d.AddChain(a, b).Ok, Is.True, "distant cells are fine (web levels do it)");
            Assert.That(d.AddChain(b, c).Ok, Is.False, "cell already chained");
            Assert.That(d.SetNail(b, 3).Ok, Is.False, "no nail on a chain");
            Assert.That(d.SetBomb(b, 5).Ok, Is.True, "bomb on a chain is fine");
            Assert.That(d.BombAt(a), Is.EqualTo(5), "the pair carries one bomb");
            var l = d.ToLevelData();
            Assert.That(l.Pieces[0], Is.EqualTo(new[] { a, b }), "chains come first");
            Assert.That(l.Bombs[0], Is.EqualTo(5));
            Assert.That(d.RemoveChain(b).Ok, Is.True);
            Assert.That(d.BombAt(a), Is.EqualTo(5), "the bomb stays on the first cell");
            Assert.That(d.BombAt(b), Is.EqualTo(0));
        }

        [Test]
        public void ChainingMovesASingleBombToTheAnchor()
        {
            var d = LevelDraft.New(4, 4);
            d.SetBomb(new Cell(2, 2), 7);
            Assert.That(d.AddChain(new Cell(0, 1), new Cell(2, 2)).Ok, Is.True);
            Assert.That(d.ToLevelData().Bombs, Is.EquivalentTo(new Dictionary<int, int> { [0] = 7 }));
            d.SetBomb(new Cell(3, 3), 4);
            Assert.That(d.AddChain(new Cell(3, 3), new Cell(1, 1)).Ok, Is.True);
            d.SetBomb(new Cell(0, 0), 2);
            Assert.That(d.AddChain(new Cell(0, 0), new Cell(3, 0)).Ok, Is.True);
            var e = LevelDraft.New(3, 3);
            e.SetBomb(new Cell(0, 0), 2);
            e.SetBomb(new Cell(2, 2), 2);
            Assert.That(e.AddChain(new Cell(0, 0), new Cell(2, 2)).Ok, Is.False, "two bombs on one pair");
            e.SetNail(new Cell(1, 1), 2);
            Assert.That(e.AddChain(new Cell(1, 1), new Cell(0, 1)).Ok, Is.False, "nailed piece");
        }

        [Test]
        public void NailAndBombNeverShareAPiece()
        {
            var d = LevelDraft.New(3, 3);
            var c = new Cell(1, 1);
            Assert.That(d.SetNail(c, 4).Ok, Is.True);
            Assert.That(d.SetBomb(c, 4).Ok, Is.False);
            Assert.That(d.SetNail(c, 0).Ok, Is.True);
            Assert.That(d.SetBomb(c, 4).Ok, Is.True);
            Assert.That(d.SetNail(c, 4).Ok, Is.False);
            Assert.That(d.ClearCell(c), Is.True);
            Assert.That(d.BombAt(c), Is.EqualTo(0));
        }

        [Test]
        public void CountsAreClamped()
        {
            var d = LevelDraft.New(3, 3);
            d.SetNail(new Cell(0, 0), 1000);
            Assert.That(d.NailAt(new Cell(0, 0)), Is.EqualTo(LevelDraft.MaxCount));
            d.SetTimer(1);
            Assert.That(d.Timer, Is.EqualTo(LevelDraft.MinTimer));
        }

        [Test]
        public void ResizeKeepsWhatFits()
        {
            var d = LevelDraft.FromLevel(TestData.Levels["L40"]);       // 6x7, chains, nails, bombs, walls
            d.Resize(4, 4);
            Assert.That(d.Problems(), Is.Empty);
            Assert.That((d.Width, d.Height), Is.EqualTo((4, 4)));
            Assert.That(d.Chains.All(c => d.OnBoard(c.A) && d.OnBoard(c.B)));
            var l = Reparse(d);                                          // the parser accepts it
            Assert.That(l.Width, Is.EqualTo(4));
            var src = TestData.Levels["L40"];
            for (int y = 0; y < 4; y++)
                for (int x = 1; x < 4; x++) Assert.That(d.VJoint(x, y), Is.EqualTo(src.VJoint(x, y)));
            d.Resize(6, 7);
            Assert.That(d.Problems(), Is.Empty);
            for (int y = 0; y < 7; y++) Assert.That(d.VJoint(4, y), Is.EqualTo(0), "the old outer edge is a flat inner joint now");
        }

        [Test]
        public void ThousandRandomEditsNeverBreakTheBoard()
        {
            var rnd = new Mulberry32(20261008);
            int R(int n) => (int)(rnd.NextDouble() * n);
            var d = LevelDraft.New(5, 6);
            int refused = 0;
            for (int i = 0; i < 1000; i++)
            {
                Cell C() => new Cell(R(d.Width + 1) - (R(8) == 0 ? 1 : 0), R(d.Height + 1));   // sometimes off the board
                EditResult r;
                switch (R(12))
                {
                    case 0: r = d.CycleJoint(R(d.Width + 1), R(d.Height + 1), R(2) == 0); break;
                    case 1: d.ApplyTemplate(C(), PieceTemplate.All[R(PieceTemplate.All.Length)]); r = EditResult.Done; break;
                    case 2: r = d.SetNail(C(), R(20) - 2); break;
                    case 3: r = d.SetBomb(C(), R(20) - 2); break;
                    case 4: r = d.AddChain(C(), C()); break;
                    case 5: r = d.RemoveChain(C()); break;
                    case 6: d.ClearCell(C()); r = EditResult.Done; break;
                    case 7: r = d.ToggleSealed(DirExt.All[R(4)], R(9)); break;
                    case 8: r = d.SetColor(C(), R(3)); break;
                    case 9: r = d.Resize(R(9) + 2, R(9) + 2); break;
                    case 10: r = d.SetTimer(R(200)); break;
                    default: r = d.SetSide(C(), DirExt.All[R(4)], (Side)(R(3) - 1)); break;
                }
                if (!r.Ok) refused++;
                Assert.That(d.Problems(), Is.Empty, $"step {i}");
                if (i % 50 == 0) Assert.DoesNotThrow(() => Reparse(d), $"step {i}");
            }
            Assert.That(refused, Is.GreaterThan(0), "some edits should have been refused");
            TestContext.WriteLine($"1000 random edits, {refused} refused, final board {d.Width}x{d.Height}");
        }

        [Test]
        public void FitWarning()
        {
            Assert.That(Layout.CellSize(6, 7), Is.EqualTo(51));        // L40 on a 375x667 phone with the K1 side gap
            Assert.That(Layout.CellSize(4, 5), Is.EqualTo(76));
            var big = LevelDraft.New(8, 10);
            Assert.That(DesignCheck.Run(big.ToLevelData()).Fits, Is.False);
            Assert.That(DesignCheck.Run(LevelDraft.New(5, 6).ToLevelData()).Fits, Is.True);
        }

        // ---- criterion 6: editor code stays out of the web build ---------------------

        [Test]
        public void EditingModelIsEditorOnly()
        {
            var asmdef = JsonReader.Parse(System.IO.File.ReadAllText("Assets/_Game/Editing/ReverseSolver.Editing.asmdef"));
            var platforms = asmdef.Get("includePlatforms").Items.Select(n => n.AsString).ToArray();
            Assert.That(platforms, Is.EqualTo(new[] { "Editor" }));
            Assert.That(asmdef.Get("noEngineReferences").AsBool, Is.True);
            foreach (var runtime in new[] { "Core/ReverseSolver.Core.asmdef", "Presentation/ReverseSolver.Presentation.asmdef" })
                Assert.That(System.IO.File.ReadAllText("Assets/_Game/" + runtime), Does.Not.Contain("ReverseSolver.Editing"), runtime);
        }

        // ---- criterion 10: CSV for a Turkish Excel -----------------------------------

        [Test]
        public void CsvIsReadyForTurkishExcel()
        {
            var rows = TestData.Levels.Levels.Select(l => ("web", l)).ToList();
            var store = new DesignStore();
            foreach (var d in SampleDesigns()) store.Save(d);
            rows.AddRange(store.Designs.Select(d => ("tasarim", d.ToLevelData())));
            string csv = LevelCsv.Build(rows);
            var bytes = LevelCsv.ToBytes(csv);
            Assert.That(bytes.Take(3), Is.EqualTo(new byte[] { 0xEF, 0xBB, 0xBF }), "UTF-8 BOM");

            var lines = csv.Split(new[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);
            Assert.That(lines[0], Is.EqualTo(string.Join(";", LevelCsv.Columns)));
            Assert.That(lines.Length, Is.EqualTo(1 + 40 + 4));
            int open = System.Array.IndexOf(LevelCsv.Columns, "acik_kenar_orani");
            foreach (var line in lines.Skip(1))
            {
                var f = line.Split(';');
                Assert.That(f.Length, Is.EqualTo(LevelCsv.Columns.Length), line);
                Assert.That(f[open], Does.Match(@"^\d,\d\d$"), "decimal comma: " + line);
            }
            Assert.That(lines[1], Does.StartWith("web;L01;1;1;4;5;20;0;0;0;0;0;0;0;"));
            Assert.That(lines.Skip(1).Take(40).All(l => l.Contains(";cozulebilir;")), "every web level follows the rule");
            Assert.That(lines.Last(), Does.StartWith("tasarim;D04;"));
        }

        // ---- G1: the image field -------------------------------------------------------

        [Test]
        public void ImageFieldRoundTripsAndKeepsTheVersion()
        {
            var store = new DesignStore();
            var d = LevelDraft.New(5, 6);
            store.Save(d);
            d.Image = d.Id + ".jpg";
            store.Save(d);
            Assert.That(d.Version, Is.EqualTo(1), "an image is look only");
            var again = DesignStore.Parse(store.ToJson());
            Assert.That(again.Find(d.Id).Image, Is.EqualTo("D01.jpg"));
            Assert.That(again.Find(d.Id).ToLevelData().Image, Is.EqualTo("D01.jpg"));
            StringAssert.Contains("\"image\":\"D01.jpg\"", store.ToJson());
            d.Image = null;
            store.Save(d);
            StringAssert.DoesNotContain("\"image\"", store.ToJson(), "no field when there is no image");
        }

        [Test]
        public void WebLevelsHaveNoImage()
        {
            foreach (var l in TestData.Levels.Levels) Assert.That(l.Image, Is.Null, l.Id);
            foreach (var l in TestData.Levels.Levels)
            {
                var w = new JsonWriter();
                LevelDraft.FromLevel(l).WriteJson(w);
                StringAssert.DoesNotContain("\"image\"", w.ToString(), l.Id);
            }
        }

        [Test]
        public void ImageMustBeAPlainFileName()
        {
            string Level(string image) => "{\"format\":1,\"levels\":[{\"id\":\"X\",\"version\":1,\"w\":1,\"h\":1," +
                "\"pieces\":[[[0,0]]],\"vEdge\":[[null,null]],\"hEdge\":[[null],[null]],\"image\":" + image + "}]}";
            Assert.That(LevelParser.Parse(Level("\"D01.jpg\"")).Levels[0].Image, Is.EqualTo("D01.jpg"));
            Assert.That(LevelParser.Parse(Level("null")).Levels[0].Image, Is.Null);
            Assert.Throws<LevelFormatException>(() => LevelParser.Parse(Level("\"../x.jpg\"")));
            Assert.Throws<LevelFormatException>(() => LevelParser.Parse(Level("\"\"")));
        }

        // ---- tier 2: level settings ---------------------------------------------------

        [Test]
        public void PaletteEditsKeepEveryCellOnAColour()
        {
            var d = LevelDraft.New(3, 3);
            Assert.That(d.AddPaletteColour("#12AB34").Ok, Is.True);
            Assert.That(d.Palette[2], Is.EqualTo("#12ab34"));
            d.SetColor(new Cell(0, 0), 2);
            d.SetColor(new Cell(1, 0), 1);
            Assert.That(d.RemovePaletteColour(1).Ok, Is.True);
            Assert.That(d.ColorAt(new Cell(1, 0)), Is.EqualTo(0), "cells of the removed colour take the first");
            Assert.That(d.ColorAt(new Cell(0, 0)), Is.EqualTo(1), "later colours shift down");
            Assert.That(d.Problems(), Is.Empty);
            Assert.That(d.SetPaletteColour(0, "red").Ok, Is.False);
            Assert.That(d.SetPaletteColour(0, "#00ff0").Ok, Is.False);
            d.RemovePaletteColour(1);
            Assert.That(d.RemovePaletteColour(0).Ok, Is.False, "at least one colour stays");
            for (int i = 0; i < LevelDraft.MaxPalette; i++) d.AddPaletteColour("#000000");
            Assert.That(d.Palette.Count, Is.EqualTo(LevelDraft.MaxPalette));
        }

        [Test]
        public void IntroCardAndArtRoundTrip()
        {
            var store = new DesignStore();
            var d = LevelDraft.New(4, 4);
            store.Save(d);
            Assert.That(d.SetIntro("Zincirli çift", "İki parça birlikte hareket eder.", "Zinciri takip et.", "chain").Ok, Is.True);
            Assert.That(d.SetArt("Kalp").Ok, Is.True);
            store.Save(d);
            Assert.That(d.Version, Is.EqualTo(1), "card text and art are look only");
            var l = DesignStore.Parse(store.ToJson()).Find(d.Id).ToLevelData();
            Assert.That((l.Intro.Title, l.Intro.Body, l.Intro.Tip, l.Intro.Icon), Is.EqualTo(("Zincirli çift", "İki parça birlikte hareket eder.", "Zinciri takip et.", "chain")));
            Assert.That(l.Art, Is.EqualTo("Kalp"));
            Assert.That(d.SetIntro("", "", "", null).Ok, Is.True);
            Assert.That(d.Intro, Is.Null, "empty card removes it");
            Assert.That(d.SetIntro("", "metin", null, "chain").Ok, Is.False, "a card needs a title");
            Assert.That(d.SetIntro("Başlık", "metin", null, "nope").Ok, Is.False);
            Assert.That(d.SetArt("  ").Ok, Is.False);
        }

        [Test]
        public void BoosterSettingsAreGameplay()
        {
            var store = new DesignStore();
            var d = LevelDraft.New(4, 4);
            store.Save(d);
            Assert.That(d.SetUnlock("wand", 2).Ok, Is.True);
            store.Save(d);
            Assert.That(d.Version, Is.EqualTo(2));
            Assert.That(d.SetUnlock("nope", 2).Ok, Is.False);
            Assert.That(d.SetBooster("hammer", 3).Ok, Is.True);
            var s = new GameSession(d.ToLevelData(), 1, "test", new Mulberry32(1));
            Assert.That(s.Stock(Booster.Hammer), Is.EqualTo(3));
        }

        // ---- criterion 5: version rule ----------------------------------------------

        [Test]
        public void GameplayEditsRaiseTheVersionByOne()
        {
            var edits = new (string what, System.Action<LevelDraft> edit)[]
            {
                ("size", d => d.Resize(6, 6)),
                ("joint", d => d.CycleJoint(1, 0, true)),
                ("chain", d => d.AddChain(new Cell(0, 0), new Cell(4, 4))),
                ("nail", d => d.SetNail(new Cell(1, 1), 3)),
                ("nail count", d => d.SetNail(new Cell(2, 2), 5)),
                ("bomb", d => d.SetBomb(new Cell(3, 3), 9)),
                ("wall", d => d.ToggleSealed(Dir.L, 1)),
                ("timer", d => d.SetTimer(75)),
                ("booster stock", d => d.SetBooster("hammer", 2)),
                ("booster unlock", d => d.Unlock["clock"] = 1),
                ("level number (unlocks)", d => d.Number = 12),
            };
            foreach (var (what, edit) in edits)
            {
                var store = new DesignStore();
                var d = LevelDraft.New(5, 6);
                d.SetNail(new Cell(2, 2), 4);
                store.Save(d);
                Assert.That(d.Version, Is.EqualTo(1));
                edit(d);
                store.Save(d);
                Assert.That(d.Version, Is.EqualTo(2), what);
                Assert.That(store.Find(d.Id).Version, Is.EqualTo(2), what);
            }
        }

        [Test]
        public void LookAndTextEditsKeepTheVersion()
        {
            var edits = new (string what, System.Action<LevelDraft> edit)[]
            {
                ("colour", d => d.SetColor(new Cell(1, 1), 1)),
                ("palette", d => d.Palette[0] = "#123456"),
                ("art", d => d.Art = "Yeni resim"),
                ("intro", d => d.Intro = new LevelIntro { Title = "Başlık", Body = "Metin", Tip = "İpucu", Icon = "chain" }),
                ("solution", d => d.Solution = new List<Move> { new Move(0, Dir.U) }),
                ("image", d => d.Image = "D01.jpg"),
            };
            foreach (var (what, edit) in edits)
            {
                var store = new DesignStore();
                var d = LevelDraft.New(5, 6);
                store.Save(d);
                edit(d);
                store.Save(d);
                Assert.That(d.Version, Is.EqualTo(1), what);
            }
        }

        [Test]
        public void ManyEditsBeforeOneSaveRaiseTheVersionOnce()
        {
            var store = new DesignStore();
            var d = LevelDraft.New(5, 6);
            store.Save(d);
            d.SetTimer(30);
            d.CycleJoint(1, 1, false);
            d.SetBomb(new Cell(4, 4), 8);
            store.Save(d);
            Assert.That(d.Version, Is.EqualTo(2));
            store.Save(d);
            Assert.That(d.Version, Is.EqualTo(2), "saving again without changes");
            d.SetTimer(31);
            d.SetTimer(30);
            store.Save(d);
            Assert.That(d.Version, Is.EqualTo(2), "changed and changed back");
        }

        [Test]
        public void DeletedIdsAreNotReused()
        {
            var store = new DesignStore();
            var a = LevelDraft.New(3, 3); store.Save(a);
            var b = LevelDraft.New(3, 3); store.Save(b);
            Assert.That((a.Id, b.Id), Is.EqualTo(("D01", "D02")));
            Assert.That(store.Delete("D02"), Is.True);
            var reloaded = DesignStore.Parse(store.ToJson());
            var c = LevelDraft.New(3, 3); reloaded.Save(c);
            Assert.That(c.Id, Is.EqualTo("D03"));
        }

        [Test]
        public void ANewDesignStartsAtVersionOne()
        {
            var store = new DesignStore();
            var copy = LevelDraft.FromLevel(TestData.Levels["L13"]);
            copy.Id = null;
            copy.Version = 7;
            store.Save(copy);
            Assert.That(copy.Version, Is.EqualTo(1));
        }
    }
}
