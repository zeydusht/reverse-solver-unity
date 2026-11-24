using NUnit.Framework;
using ReverseSolver.Presentation;
using UnityEngine;

namespace ReverseSolver.Core.Tests
{
    /* PM_102: the win fanfare g and its tones, the knock steady, the boosters' sounds, the corridor's whoosh. */
    public class Pm102Tests
    {
        [Test]
        public void TheWinIsGWithItsFiveBeats()
        {
            Assert.That(Music.DefaultFanfare, Is.EqualTo('g'));
            Assert.That(Music.FanfareKey, Is.EqualTo("g"));
            var g = BeatMap.Of("g");
            Assert.That(g.beats.Length, Is.EqualTo(5), "light + Great!, Jigo, +50, Next, Menu + confetti");
            Assert.That(P21WinView.Plan(5), Is.EqualTo(new[] { 0, 0, 1, 2, 3, 4, 4 }));
            Assert.That(Music.TonesOf('g'), Is.EqualTo(new[] { "piyano", "marimba", "pizz", "ksilofon", "kutu" }));
            Assert.That(Music.FanfareClipKey, Is.EqualTo("g-pizz"), "PM_107: g in its pizzicato tone");
#if UNITY_EDITOR
            foreach (var tone in Music.TonesOf('g'))
            {
                if (tone == Music.Piano) continue;
                var clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>($"Assets/Audio/Fanfare96/kazanma-g-{tone}.wav");
                Assert.That(clip, Is.Not.Null, tone);
                Assert.That(clip.length, Is.EqualTo(g.length).Within(.01f), $"{tone}: g's length (its beat map)");
            }
#endif
        }

        static float Peak(AudioClip c)
        {
            var d = new float[c.samples * c.channels];
            c.GetData(d, 0);
            float p = 0;
            foreach (var v in d) p = Mathf.Max(p, Mathf.Abs(v));
            return 20 * Mathf.Log10(p + 1e-9f);
        }

        [Test]
        public void TheKnocksTakesAreAlike()
        {
            float p0 = 0, l0 = 0;
            for (int n = 1; n <= 4; n++)
            {
                var c = Resources.Load<AudioClip>($"Art/audio/pm102/carpma-a{n}");
                Assert.That(c, Is.Not.Null, $"carpma-a{n}");
                if (n == 1) { p0 = Peak(c); l0 = c.length; continue; }
                Assert.That(Peak(c), Is.EqualTo(p0).Within(.5f), $"carpma-a{n}: the same peak");
                Assert.That(c.length, Is.EqualTo(l0).Within(.005f), $"carpma-a{n}: the same length");
            }
            Assert.That(20 * Mathf.Log10(GameRoot.ExitLevel), Is.EqualTo(-1f).Within(.05f), "the slide 1 dB down");
            Assert.That(GameRoot.KnockGap, Is.InRange(.1f, .25f));
        }

        [Test]
        public void EveryBoosterHasThreeCandidates()
        {
            var parts = new[] { ("makas", "kesim"), ("degnek", "donusum"), ("cekic", "vurus"), ("cekic", "kirinti"), ("saat", "tiktak"), ("saat", "donma"), ("saat", "kirilma") };
            foreach (var (b, part) in parts)
                foreach (char l in "abc")
                    Assert.That(Resources.Load<AudioClip>($"Art/audio/pm102/booster-{b}-{part}-{l}"), Is.Not.Null, $"booster-{b}-{part}-{l}");
            BoosterSounds102.Load();
            foreach (BoosterSounds102.Part p in System.Enum.GetValues(typeof(BoosterSounds102.Part)))
                Assert.That(BoosterSounds102.On(p), Is.True, p.ToString());
            Assert.That(BoosterSounds102.Clip(BoosterSounds102.Part.DegnekDonusum).length, Is.GreaterThan(BoosterSounds102.WandLead), "the poof inside the clip");
        }

        [Test]
        public void TheCorridorsWhooshIsTheLouderOne()
        {
            Gecis100.Load();
            // PM_107: its mastered copy (pm107), made from pm102's (docs/art/pm107/miks/hedefler.json)
            Assert.That(Gecis100.Clip(Gecis100.Kind.Koridor), Is.SameAs(Resources.Load<AudioClip>("Art/audio/pm107/gecis-koridor-a")));
            StringAssert.Contains("\"source\": \"Assets/_Game/Presentation/Resources/Art/audio/pm102/gecis-koridor-a.wav\"", System.IO.File.ReadAllText("docs/art/pm107/miks/hedefler.json"));
        }
    }
}
