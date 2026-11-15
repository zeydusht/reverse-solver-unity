using System.IO;
using System.Linq;
using NUnit.Framework;
using ReverseSolver.Generation;

namespace ReverseSolver.Core.Tests
{
    /* PM_71: p_win1 blended with pt1's real results. */
    public class Pm71Tests
    {
        static LevelSet Original() => LevelParser.Parse(File.ReadAllText("Assets/_Game/Levels/redesign.json"));
        static LevelSet Pm70() => P21Onboarding.TuneLate(P21Onboarding.TuneSecondTen(P21Onboarding.SpaceObstacleLessons(Original())));

        [Test]
        public void Pt1DataCoversTheForty()
        {
            Assert.That(PlayData.Pt1.Count, Is.EqualTo(40), "every pt1 row matches its version in redesign.json");
            Assert.That(PlayData.Pt1.Find("S38").N, Is.EqualTo(7));
        }

        /* The PM's table: S38 (PM_70's version) model ,56, data ,19, blended ,39. */
        [Test]
        public void S38BlendsToThePmValue()
        {
            var s38 = Pm70().Levels.First(l => l.Id == "S38");
            var b = DifficultyModelV2.PWin1Parts(DifficultyV2.Compute(s38));
            TestContext.WriteLine($"model {b.Model:0.000} data {b.Data:0.000} blended {b.Blended:0.000} w {b.Weight:0.000}");
            Assert.That(b.Blended, Is.EqualTo(.39).Within(.02));
        }

        [Test]
        public void ALevelNobodyPlayedKeepsTheModel()
        {
            var level = Original().Levels[12].CopyNextVersion();
            level.Id = "X_NEW";
            var f = DifficultyV2.Compute(level, 600, 80);
            Assert.That(DifficultyModelV2.PWin1Blended(f), Is.EqualTo(DifficultyModelV2.PWin1(f)));
            Assert.That(DifficultyModelV2.PWin1Parts(f).HasData, Is.False);
        }

        [Test]
        public void TheCohortIsAParameter()
        {
            var played = Original();
            var csv = "level_id,level_version,pt1_slot,n,d_obs\nS38,1,38,10,0\n";
            var data = PlayData.Parse("p22", csv, played);
            Assert.That(data.Cohort, Is.EqualTo("p22"));
            Assert.That(data.Count, Is.EqualTo(1));
            // the version played, d_obs 0 (an average player wins half), n 10 (w = .5): the blend is halfway in logits
            var s38 = played.Levels.First(l => l.Id == "S38");
            var b = DifficultyModelV2.PWin1Parts(DifficultyV2.Compute(s38), data);
            Assert.That(b.Data, Is.EqualTo(.5).Within(1e-9));
            Assert.That(b.Weight, Is.EqualTo(.5).Within(1e-9));
            Assert.That(DifficultyModelV2.Logit0(b.Blended), Is.EqualTo(.5 * DifficultyModelV2.Logit0(b.Model)).Within(1e-9));
        }
    }
}
