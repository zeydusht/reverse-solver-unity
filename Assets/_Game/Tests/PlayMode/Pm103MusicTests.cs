using System.Collections;
using NUnit.Framework;
using ReverseSolver.Presentation;
using UnityEngine;
using UnityEngine.TestTools;

namespace ReverseSolver.PlayMode.Tests
{
    /* PM_103: the iOS build's music (the editor plays the same files): the lobby's loop in the menu, the dive's crossfade
       to the level's, the panel's 35 %, the win fanfare (g in marimba) cutting the music, the lobby back after it where
       it stopped. Offline: nothing is downloaded. */
    public sealed class Pm103MusicTests
    {
        GameObject _go;

        [TearDown]
        public void TearDown() { if (_go != null) Object.Destroy(_go); }

        static IEnumerator Wait(float s) { float end = Time.realtimeSinceStartup + s; while (Time.realtimeSinceStartup < end) yield return null; }

        [UnityTest]
        public IEnumerator TheMusicPlaysOfflineWithItsCrossfades()
        {
            bool wasOn = Music.On, wasMuted = Art.Muted;
            Music.On = true; Art.Muted = false;   // PlayerPrefs: a test before may have left the sound off
            _go = new GameObject("Pm103Music");
            Music.Start(_go.AddComponent<MusicDriver>());
            yield return Wait(.5f);
            var now = Music.Now;
            Assert.That(now.lobbyClip, Is.EqualTo("music-n"), "the lobby: Urban Shop");
            Assert.That(now.levelClip, Is.EqualTo("music-E"), "the level: Cat in Space");
            Assert.That(now.lobby && !now.level, Is.True, "the menu: the lobby's loop only");
            Assert.That(now.lobbyVolume, Is.EqualTo(Music.BaseVolume).Within(.01f));

            Music.Play(Music.Scene.Level);
            yield return Wait(.3f);
            now = Music.Now;
            Assert.That(now.lobby && now.level, Is.True, "the crossfade: both");
            Assert.That(now.levelVolume, Is.InRange(.01f, Music.BaseVolume - .01f));
            yield return Wait(.5f);
            now = Music.Now;
            Assert.That(!now.lobby && now.level, Is.True, "the level's loop only");
            Assert.That(now.levelVolume, Is.EqualTo(Music.BaseVolume).Within(.01f));

            Music.Panel(true);
            yield return Wait(.3f);
            Assert.That(Music.Now.levelVolume, Is.EqualTo(Music.BaseVolume * Music.PanelLevel).Within(.01f), "a panel: 35 %");
            Music.Panel(false);
            yield return Wait(.4f);
            Assert.That(Music.Now.levelVolume, Is.EqualTo(Music.BaseVolume).Within(.01f));

            Assert.That(Music.Fanfare(), Is.True);
            Assert.That(Music.FanfareClipName, Is.EqualTo("kazanma-g-pizz"), "PM_107: Zeyd's pizzicato");
            yield return Wait(.3f);
            now = Music.Now;
            Assert.That(!now.lobby && !now.level, Is.True, "the fanfare alone");
            Assert.That(Music.FanfareTime, Is.InRange(.1f, .6f));
            yield return Wait(Music.FanfareLength + .2f);
            now = Music.Now;
            Assert.That(now.lobby && !now.level, Is.True, "after the fanfare the lobby comes back");
            Assert.That(now.lobbyVolume, Is.InRange(.005f, Music.BaseVolume), "fading in over 1 s");
            Music.On = wasOn; Art.Muted = wasMuted;
        }
    }
}
