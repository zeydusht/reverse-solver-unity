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
            Assert.That(Music.DefaultLevel, Is.EqualTo('c'), "the level: c for now");
            Assert.That(Music.DefaultFanfare, Is.EqualTo('b'));
            Assert.That(Music.LevelVariant, Is.EqualTo('c'));
            Assert.That(Sfx95.Letter(Sfx95.Kind.Exit), Is.EqualTo('c'));
            Assert.That(Sfx95.Letter(Sfx95.Kind.Knock), Is.EqualTo('a'));
            Assert.That(Sfx95.Letter(Sfx95.Kind.Last), Is.EqualTo('a'));
            Assert.That(Sfx95.Letter(Sfx95.Kind.Tick), Is.EqualTo('b'));
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
            Assert.That(UiSounds.Take(), Is.EqualTo(new[] { UiSounds.Kind.Tick, UiSounds.Kind.Open }));
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
