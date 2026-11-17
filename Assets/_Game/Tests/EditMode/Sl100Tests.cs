using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using ReverseSolver.Presentation;

namespace ReverseSolver.Core.Tests
{
    /* PM_81 §6 / PM_82 §1: the 100-level set (sl100) as the plan wants it, and 100 levels carried through. */
    public class Sl100Tests
    {
        static LevelSet Set => LevelParser.Parse(File.ReadAllText("Assets/_Game/Levels/sl100.json"));

        static HashSet<string> Kinds(LevelData l)
        {
            var t = new HashSet<string>();
            if (l.Nails.Count > 0) t.Add("lock");
            if (l.Pieces.Any(p => p.Length > 1)) t.Add("chain");
            if (l.Bombs.Count > 0) t.Add("bomb");
            if (l.Sealed.Values.Sum(v => v.Length) > 0) t.Add("wall");
            return t;
        }

        [Test]
        public void HundredLevelsWithTheirPaintingsAndIds()
        {
            var set = Set;
            Assert.AreEqual(100, set.Levels.Count);
            Assert.AreEqual(100, set.Levels.Select(l => l.Id).Distinct().Count());
            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual(i + 1, set.Levels[i].Number, $"slot {i + 1}");
                Assert.AreEqual($"painting-{i + 1:00}", set.Levels[i].Image);
                Assert.IsFalse(set.Levels[i].Placeholder, $"slot {i + 1} designed");
            }
            // the app's own copy of live/ is this set
            var embedded = LevelParser.Parse(File.ReadAllText("Assets/Resources/Live/levels.json"));
            CollectionAssert.AreEqual(set.Levels.Select(l => l.Id + "v" + l.Version), embedded.Levels.Select(l => l.Id + "v" + l.Version));
        }

        [Test]
        public void RhythmLessonsCombinationsAndBoosters()
        {
            var set = Set;
            string Role(int slot) => set.Levels[slot - 1].Role;
            for (int s = 14; s <= 94; s += 10) Assert.AreEqual("hard", Role(s), $"{s}");
            for (int s = 19; s <= 99; s += 10) Assert.AreEqual("veryhard", Role(s), $"{s}");
            for (int s = 20; s <= 100; s += 10) Assert.AreEqual("breather", Role(s), $"{s}");
            foreach (var (slot, kind) in new[] { (3, "lock"), (7, "chain"), (11, "bomb"), (21, "wall") })
            {
                var l = set.Levels[slot - 1];
                Assert.AreEqual(kind, l.Lesson, $"lesson {slot}");
                CollectionAssert.AreEquivalent(new[] { kind }, Kinds(l), $"lesson {slot} shows its obstacle alone");
                Assert.IsNotNull(l.Intro, $"lesson {slot} card");
            }
            // nothing appears before its lesson
            for (int s = 1; s <= 100; s++)
            {
                var k = Kinds(set.Levels[s - 1]);
                if (s < 3) Assert.IsEmpty(k, $"{s}");
                if (s < 7) Assert.IsFalse(k.Contains("chain"), $"{s} chain");
                if (s < 11) Assert.IsFalse(k.Contains("bomb"), $"{s} bomb");
                if (s < 21) Assert.IsFalse(k.Contains("wall"), $"{s} wall");
            }
            var combos = new Dictionary<int, string[]>
            {
                [31] = new[] { "bomb", "chain" }, [41] = new[] { "lock", "bomb" }, [51] = new[] { "wall", "bomb" }, [61] = new[] { "chain", "wall" },
                [71] = new[] { "lock", "chain", "bomb" }, [81] = new[] { "wall", "lock" }, [91] = new[] { "lock", "chain", "bomb", "wall" },
            };
            foreach (var kv in combos) CollectionAssert.AreEquivalent(kv.Value, Kinds(set.Levels[kv.Key - 1]), $"combination {kv.Key}");
            foreach (var (slot, booster) in new[] { (6, "scissors"), (13, "hammer"), (18, "clock"), (25, "wand") })
            {
                Assert.AreEqual(booster, set.Levels[slot - 1].BoosterIntro?.Id, $"booster {slot}");
                Assert.AreEqual(slot, set.Levels[0].Unlock[booster]);
            }
            // durations: at most 120 s, hard and very hard at most 150 s
            foreach (var l in set.Levels)
                Assert.LessOrEqual(l.TimerSeconds, l.Hard ? 150 : 120, l.Id);
        }

        [Test]
        public void ProgressCarriesAHundredLevels()
        {
            var set = Set;
            var log = new AttemptLog();
            var progress = new Progress(log, set.Levels);
            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual(i, progress.Next(), $"next after {i} wins");
                if (i + 1 < 100) Assert.IsFalse(progress.Open(i + 1), $"{i + 2} opens only after {i + 1} is won");
                log.Add(new AttemptRecord { LevelId = set.Levels[i].Id, Result = "win", Stars = 1 });
            }
            Assert.IsTrue(progress.Won(99));
            Assert.AreEqual(99, progress.Next());
            Assert.IsTrue(P21ChestRules.Due(100));
            Assert.AreEqual(100, P21ChestRules.Next(95));
        }
    }
}
