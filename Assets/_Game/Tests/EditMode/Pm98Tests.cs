using NUnit.Framework;
using ReverseSolver.Presentation;
using UnityEngine;

namespace ReverseSolver.Core.Tests
{
    /* PM_98: Zeyd's picks, the new level candidates, the pause's music level, the UI sounds. */
    public class Pm98Tests
    {
        [Test]
        public void ZeydsPicksAreTheDefaults()
        {
            Assert.That(Music.DefaultLobby, Is.EqualTo('n'), "the lobby: Urban Shop");
            Assert.That(Music.DefaultFanfare, Is.EqualTo('g'));   // PM_102   // PM_99 moved the level to E and the exit to f (Pm99Tests)
            Assert.That(Sfx95.Letter(Sfx95.Kind.Knock), Is.EqualTo('a'));
            Assert.That(Sfx95.Letter(Sfx95.Kind.Last), Is.EqualTo('a'));
            Assert.That(Sfx95.Letter(Sfx95.Kind.Tick), Is.EqualTo('0'), "PM_105: the tick is gone (every button: the tok)");
        }

        [Test]
        public void PauseKeepsTheMusicLow()
        {
            Assert.That(Music.PanelLevel, Is.EqualTo(.35f).Within(.001f));
            Assert.That(Music.PanelBack, Is.EqualTo(.3f).Within(.001f));
        }

        [Test]
        public void ASwitchOrAPassiveButtonOutranksTheTick()
        {
            UiSounds.Take();
            UiSounds.Request(UiSounds.Kind.Tick); UiSounds.Request(UiSounds.Kind.Toggle);
            Assert.That(UiSounds.Take(), Is.EqualTo(new[] { UiSounds.Kind.Toggle }));
            UiSounds.Request(UiSounds.Kind.Dull); UiSounds.Request(UiSounds.Kind.Tick);
            Assert.That(UiSounds.Take(), Is.EqualTo(new[] { UiSounds.Kind.Dull }));
            UiSounds.Request(UiSounds.Kind.Tick); UiSounds.Request(UiSounds.Kind.Open);
            Assert.That(UiSounds.Take(), Is.EqualTo(new[] { UiSounds.Kind.Tick }), "PM_106: one sound per press");
            UiSounds.Request(UiSounds.Kind.Close); UiSounds.Request(UiSounds.Kind.Open);   // a panel rebuilt in place
            Assert.That(UiSounds.Take(), Is.Empty);
        }

        [Test]
        public void EveryUiSoundHasRecordedTakes()
        {
            foreach (UiSounds.Kind k in System.Enum.GetValues(typeof(UiSounds.Kind)))
            {
                var clip = UiSounds.Pick(k, out float pitch);
                Assert.That(clip, Is.Not.Null, k.ToString());
                Assert.That(pitch, Is.InRange(.95f, 1.05f));
            }
        }
    }
}
