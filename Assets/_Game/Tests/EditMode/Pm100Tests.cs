using NUnit.Framework;
using ReverseSolver.Presentation;
using UnityEngine;

namespace ReverseSolver.Core.Tests
{
    /* PM_100: the fanfare's other instruments (same beats), the jam and the exit apart, the new mix, the transitions. */
    public class Pm100Tests
    {
        [Test]
        public void TheFanfareTonesKeepItsBeats()
        {
            Assert.That(Music.DefaultTone, Is.EqualTo("piyano"), "today's fanfare stays the default");
            Assert.That(Music.Tone, Is.EqualTo("piyano"));
            Assert.That(Music.FanfareClipKey, Is.EqualTo(Music.FanfareKey), "the piano plays the beats' own clip");
            Assert.That(Music.Tones, Is.EquivalentTo(new[] { "piyano", "marimba", "pizz", "ksilofon", "kutu", "bakir" }));
#if UNITY_EDITOR
            foreach (var tone in Music.Tones)
            {
                if (tone == Music.DefaultTone) continue;
                foreach (var key in new[] { "b", "b5" })
                {
                    var clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>($"Assets/Audio/Fanfare96/kazanma-{key}-{tone}.wav");
                    Assert.That(clip, Is.Not.Null, $"kazanma-{key}-{tone}");
                    // the same length as the piano's: the beat map (b, b5) is its map too
                    Assert.That(clip.length, Is.EqualTo(BeatMap.Of(key).length).Within(.01f), $"kazanma-{key}-{tone}");
                }
            }
#endif
        }

        [Test]
        public void TheJamGetsOnlyAShortLiftOff()
        {
            for (int n = 1; n <= 4; n++)
            {
                var lift = Resources.Load<AudioClip>($"Art/audio/pm100/kalkis-f{n}");
                Assert.That(lift, Is.Not.Null, $"kalkis-f{n}");
                Assert.That(lift.length, Is.LessThanOrEqualTo(.06f), "at most 60 ms");
                Assert.That(lift.length + .01f, Is.LessThanOrEqualTo(GameRoot.LiftRoom), "over before the knock at the contact");
                var shortLift = Resources.Load<AudioClip>($"Art/audio/pm100/kalkisk-f{n}");
                Assert.That(shortLift, Is.Not.Null, $"kalkisk-f{n}");
                Assert.That(shortLift.length + .01f, Is.LessThanOrEqualTo(GameRoot.LiftShortRoom), "the short one before a short slide's contact");
                Assert.That(Resources.Load<AudioClip>($"Art/audio/pm100/cikis-f{n}"), Is.Not.Null, "the exit, high-passed");
                Assert.That(Resources.Load<AudioClip>($"Art/audio/pm100/carpma-a{n}"), Is.Not.Null, "the knock, softened on top");
            }
            Assert.That(Resources.Load<AudioClip>("Art/audio/pm100/bomba-a1"), Is.Not.Null);
            Assert.That(Resources.Load<AudioClip>("Art/audio/pm100/kilit-a1"), Is.Not.Null);
            Sfx95.Load();
            Assert.That(Sfx95.Letter(Sfx95.Kind.Lift), Is.EqualTo(Sfx95.Letter(Sfx95.Kind.Exit)), "the lift-off is the exit's own");
            Assert.That(Sfx95.On(Sfx95.Kind.Bomb) && Sfx95.On(Sfx95.Kind.Unlock), Is.True);
        }

        [Test]
        public void TheMusicIsThreeDbDown()
        {
            Assert.That(Music.BaseVolume, Is.EqualTo(.35f * Mathf.Pow(10, -3 / 20f)).Within(.002f));
            Assert.That(Music.Gain, Is.EqualTo(1f), "&muzikses= absent: 1");
            Assert.That(Sfx95.Gain, Is.EqualTo(1f), "&sfx= absent: 1");
        }

        [Test]
        public void EveryTransitionHasThreeCandidates()
        {
            foreach (var kind in new[] { "koridor", "dalis", "ting", "cizgi", "geri", "hud", "sekme" })
                foreach (char l in "abc")
                    Assert.That(Resources.Load<AudioClip>($"Art/audio/pm100/gecis-{kind}-{l}"), Is.Not.Null, $"gecis-{kind}-{l}");
            Gecis100.Load();
            foreach (Gecis100.Kind k in System.Enum.GetValues(typeof(Gecis100.Kind)))
            {
                Assert.That(Gecis100.Letter(k), Is.EqualTo(Gecis100.Default));
                Assert.That(Gecis100.Clip(k), Is.Not.Null, k.ToString());
            }
            Assert.That(Gecis100.Clip(Gecis100.Kind.Dalis).length, Is.EqualTo(Gecis100.DiveLead).Within(.03f), "the rising whoosh ends at the touch");
            Assert.That(Gecis100.Clip(Gecis100.Kind.Koridor).length, Is.GreaterThanOrEqualTo(1.5f), "the corridor's texture outlasts its slide");
            Assert.That(Gecis100.Off, Is.False);
        }
    }
}
