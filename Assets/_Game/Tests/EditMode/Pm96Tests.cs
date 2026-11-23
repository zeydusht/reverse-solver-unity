using NUnit.Framework;
using ReverseSolver.Presentation;

namespace ReverseSolver.Core.Tests
{
    /* PM_96: Zeyd's sound choices as defaults, the lobby and level music, the fanfares' beat map. */
    public class Pm96Tests
    {
        [Test]
        public void ZeydsChoicesAreTheDefaults()
        {
            // PM_98 §1 moved the lobby to n and the level to c (Pm98Tests); the win stays b
            Assert.That(Music.DefaultFanfare, Is.EqualTo('b'), "the win: Zeyd's b");
            Assert.That(Music.LobbyVariant, Is.EqualTo(Music.DefaultLobby));
            Assert.That(Music.LevelVariant, Is.EqualTo(Music.DefaultLevel));
            Assert.That(Music.FanfareVariant, Is.EqualTo('b'));
        }

        [Test]
        public void EveryFanfareHasCountableBeats()
        {
            foreach (char letter in "0abcdefg")
            {
                Assert.That(BeatMap.Has(letter.ToString()), Is.True, $"fanfare {letter} in vurus_haritasi.json");
                var f = BeatMap.Of(letter.ToString());
                Assert.That(f, Is.Not.SameAs(BeatMap.Fallback), $"fanfare {letter}");
                Assert.That(f.beats.Length, Is.InRange(5, 7), $"fanfare {letter}: 5-7 beats");
                Assert.That(f.beats[0], Is.LessThan(.05f), $"fanfare {letter} starts on a beat");
                for (int i = 1; i < f.beats.Length; i++)
                    Assert.That(f.beats[i] - f.beats[i - 1], Is.GreaterThanOrEqualTo(.13f), $"fanfare {letter}: beat {i + 1} apart from the last one");
                Assert.That(f.beats[f.beats.Length - 1], Is.LessThan(f.length), $"fanfare {letter}: its beats inside it");
                Assert.That(f.strong, Is.InRange(2, f.beats.Length - 1), $"fanfare {letter}: the strongest beat after the title");
                if ("defg".IndexOf(letter) >= 0) Assert.That(f.length, Is.InRange(1.5f, 2.55f), $"new fanfare {letter}: 1.5-2.5 s");
            }
        }

        [Test]
        public void TheEntranceLeadsTheBeat()
        {
            Assert.That(P21WinView.Lead, Is.InRange(.01f, .02f), "an event starts 10-20 ms before its beat");
            Assert.That(Music.CrossFade, Is.EqualTo(.6f).Within(.05f), "the dive's crossfade");
        }
    }
}
