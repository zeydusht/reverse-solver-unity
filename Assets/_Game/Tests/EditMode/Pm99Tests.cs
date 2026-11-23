using NUnit.Framework;
using ReverseSolver.Presentation;
using UnityEngine;

namespace ReverseSolver.Core.Tests
{
    /* PM_99: the level music E, the soft exit, the louder knock, no last-piece sound, the win screen's beats (7 / 5). */
    public class Pm99Tests
    {
        [Test]
        public void TheNewDefaults()
        {
            Assert.That(Music.DefaultLevel, Is.EqualTo('E'), "the level: Cat in Space");
            Assert.That(Music.LevelVariant, Is.EqualTo('E'));
            Assert.That(Sfx95.Letter(Sfx95.Kind.Exit), Is.EqualTo('f'), "the exit: the soft swish");
            Assert.That(Sfx95.Letter(Sfx95.Kind.Knock), Is.EqualTo('a'), "the knock stays a (louder)");
            Assert.That(Music.DefaultWinBeats, Is.EqualTo(5));
            Assert.That(Music.FanfareKey, Is.EqualTo("g"), "PM_102: g by default (5 beats of its own)");
        }

        [Test]
        public void SoftExitsAreShortTakes()
        {
            foreach (char letter in "fghij")
                for (int n = 1; n <= 3; n++)
                {
                    var clip = Resources.Load<AudioClip>($"Art/audio/pm99/cikis-{letter}{n}");
                    Assert.That(clip, Is.Not.Null, $"cikis-{letter}{n}");
                    Assert.That(clip.length, Is.LessThanOrEqualTo(.26f), $"cikis-{letter}{n}: at most 0.25 s");
                }
            for (int n = 1; n <= 4; n++) Assert.That(Resources.Load<AudioClip>($"Art/audio/pm99/carpma-a{n}"), Is.Not.Null, "the louder knock");
        }

        [Test]
        public void TheShortFanfareKeepsTheLastChord()
        {
            var full = BeatMap.Of("b"); var cut = BeatMap.Of("b5");
            Assert.That(full.beats.Length, Is.EqualTo(7));
            Assert.That(cut.beats.Length, Is.EqualTo(5));
            float d = full.beats[4] - cut.beats[2];   // the cut: beats 3 and 4 left out
            Assert.That(cut.beats[1], Is.EqualTo(full.beats[1]).Within(.002f));
            Assert.That(cut.beats[4], Is.EqualTo(full.beats[6] - d).Within(.002f), "the last chord, earlier by the cut");
            Assert.That(cut.length, Is.EqualTo(full.length - d).Within(.03f));
        }

        [Test]
        public void EveryBeatOneElement()
        {
            // light, title, Jigo, gold, Next, Menu, confetti
            Assert.That(P21WinView.Plan(7), Is.EqualTo(new[] { 0, 1, 2, 3, 4, 5, 6 }));
            Assert.That(P21WinView.Plan(5), Is.EqualTo(new[] { 0, 0, 1, 2, 3, 4, 4 }));
            Assert.That(P21WinView.Plan(6), Is.EqualTo(new[] { 0, 1, 2, 3, 4, 5, 5 }));
        }
    }
}
