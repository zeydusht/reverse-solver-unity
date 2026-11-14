using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using NUnit.Framework;
using ReverseSolver.Generation;
using Role = ReverseSolver.Generation.DifficultyModelV2.Role;

namespace ReverseSolver.Core.Tests
{
    /* PM_70: difficulty model v2 and the p22 curve on it. */
    public class P22CurveTests
    {
        static LevelSet Original() => LevelParser.Parse(File.ReadAllText("Assets/_Game/Levels/redesign.json"));

        /* The C# model gives the PM's p_pred (docs/analiz/v2/pool_scored.csv, final.py) for all 250 levels. */
        [Test]
        public void ModelV2ReproducesThePoolScores()
        {
            var lines = File.ReadAllLines("docs/analiz/v2/pool_scored.csv");
            var head = lines[0].Split(',').ToList();
            int Col(string name) => head.IndexOf(name);
            double D(string[] v, string name) => v[Col(name)] == "" ? 1 : double.Parse(v[Col(name)], CultureInfo.InvariantCulture);
            int n = 0;
            foreach (var line in lines.Skip(1))
            {
                var v = line.Split(',');
                bool bombs = int.Parse(v[Col("bombs")]) > 0;
                double p = DifficultyModelV2.PWin1(bombs, D(v, "p_survive_aware"), D(v, "p_survive_rand"), D(v, "sec_per_move"),
                    D(v, "narrow_by_chain"), int.Parse(v[Col("pieces")]));
                Assert.That(p, Is.EqualTo(D(v, "p_pred")).Within(.01), $"{v[0]} {v[1]} v{v[2]}");
                n++;
            }
            Assert.That(n, Is.EqualTo(250));
        }

        static DifficultyModelV2.Role RoleOf(int slot) =>
            slot <= 10 || P21Onboarding.IsObstacleLesson(slot) ? Role.Lesson :
            P21Onboarding.HardSlots22.Contains(slot) ? Role.Hard :
            P21Onboarding.BreatherSlots22.Contains(slot) ? Role.Breather : Role.Normal;

        /* p22 11-40 on model v2: the target bands, hard counts and breathers, and every design rule from 13 on. */
        [Test]
        public void P22ElevenToFortyHitsTheCurveAndTheRules()
        {
            var original = Original().Levels;
            var p22 = P21Onboarding.P22(Original()).Levels;
            Assert.That(p22.Select(l => l.Id).OrderBy(x => x), Is.EqualTo(original.Select(l => l.Id).OrderBy(x => x)), "same 40 levels");
            var rows = new List<(int, Role, double)>();
            var broken = new List<string>();
            for (int slot = 11; slot <= 40; slot++)
            {
                var l = p22[slot - 1];
                Assert.That(l.Number, Is.EqualTo(slot));
                var role = RoleOf(slot);
                Assert.That(l.Hard, Is.EqualTo(role == Role.Hard), $"{slot} {l.Id}: the hard card matches the role");
                var f = DifficultyV2.Compute(l);
                double p = DifficultyModelV2.PWin1(f);
                rows.Add((slot, role, p));
                if (slot >= 13) broken.AddRange(DifficultyModelV2.Rules.Check(f, slot, role).Select(u => $"{slot} {l.Id}: {u}"));
                TestContext.WriteLine($"{slot} {l.Id} v{l.Version} {role} p {p:0.00} jam {DifficultyModelV2.JamPerExit(f):0.00} choke {f.ChokeN}");
            }
            broken.AddRange(DifficultyModelV2.CheckCurve(rows));
            Assert.That(broken, Is.Empty);
        }

        /* Every PM_70 change is a new version of the same level; untouched levels keep theirs. */
        [Test]
        public void RetunedLevelsGetANewVersion()
        {
            var before = P21Onboarding.TuneSecondTen(P21Onboarding.SpaceObstacleLessons(Original())).Levels.ToDictionary(l => l.Id);
            var after = P21Onboarding.P22(Original()).Levels;
            var tuned = new HashSet<string>(P21Onboarding.LateTuning.Select(r => r.Id));
            Assert.That(tuned.Count, Is.EqualTo(P21Onboarding.LateTuning.Length), "one entry per level");
            foreach (var l in after)
                Assert.That(l.Version, Is.EqualTo(before[l.Id].Version + (tuned.Contains(l.Id) ? 1 : 0)), l.Id);
        }
    }
}
