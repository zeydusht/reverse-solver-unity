using System.IO;
using System.Linq;
using NUnit.Framework;
using ReverseSolver.EditorTools;
using ReverseSolver.Presentation;
using UnityEngine;

namespace ReverseSolver.Core.Tests
{
    /* PM_103: Zeyd's final sound choices in one place (ses.json) are every sound's default; the iOS build carries only
       those (the chosen music embedded, the other candidates left out). */
    public class Pm103Tests
    {
        static readonly (string key, string choice)[] Zeyd =
        {
            // PM_107: Zeyd's table of 24 Nov 23:59
            ("lobi", "n"), ("bolum", "E"), ("kazanma", "g"), ("kazanmaton", "pizz"), ("cikis", "u"), ("carpma", "a"),
            ("makas", "c"), ("degnek", "a"), ("cekic", "c"), ("saat", "b"), ("koridor", "a"), ("dalis", "a"), ("cizgi", "c"), ("geri", "a"), ("sekme", "a"), ("dugme", "c"),
            ("zincir", "f"), ("zincirger", "f"), ("kilitdokun", "a"), ("kilitac", "a"), ("patlama", "a"), ("duvar", "a"),
        };

        [Test]
        public void ZeydsChoicesAreInOneFile()
        {
            Assert.That(File.Exists(SesSecim.Asset), SesSecim.Asset);
            Assert.That(SesSecim.Keys, Is.EquivalentTo(Zeyd.Select(z => z.key)));
            foreach (var (key, choice) in Zeyd) Assert.That(SesSecim.Of(key), Is.EqualTo(choice), key);
        }

        [Test]
        public void EverySoundsDefaultIsTheChoice()
        {
            SesSecim.Collect();
            Assert.That(Music.LobbyVariant, Is.EqualTo('n'));
            Assert.That(Music.LevelVariant, Is.EqualTo('E'));
            Assert.That(Music.FanfareKey, Is.EqualTo("g"));
            Assert.That(Music.Tone, Is.EqualTo("pizz"));
            Assert.That(Music.FanfareClipKey, Is.EqualTo("g-pizz"));
            Assert.That(Sfx95.Letter(Sfx95.Kind.Exit), Is.EqualTo('u'));
            Assert.That(Sfx95.Letter(Sfx95.Kind.Lift), Is.EqualTo('0'), "PM_106: no lift-off");
            Assert.That(Sfx95.Letter(Sfx95.Kind.Knock), Is.EqualTo('a'));
            Assert.That(Sfx95.Letter(Sfx95.Kind.Tick), Is.EqualTo('0'), "PM_105: no tick");
            foreach (var (b, l) in new[] { ("makas", 'c'), ("degnek", 'a'), ("cekic", 'c'), ("saat", 'b') })
                Assert.That(BoosterSounds102.Letter(b), Is.EqualTo(l), b);
            Assert.That(BoosterSounds102.Clip(BoosterSounds102.Part.MakasKesim).name, Is.EqualTo("booster-makas-kesim-c"), "the garden shears");
            foreach (Gecis100.Kind k in System.Enum.GetValues(typeof(Gecis100.Kind)))
                Assert.That(Gecis100.Letter(k), Is.EqualTo(k == Gecis100.Kind.Cizgi ? 'c' : 'a'), k.ToString());
        }

        [Test]
        public void TheGameLoadsOnlyTheChosenFiles()
        {
            SesSecim.Collect();
            var used = SesSecim.Chosen();   // PM_107: the originals of the mastered copies (pm107) count as chosen
            foreach (var p in used) Assert.That(Resources.Load(p), Is.Not.Null, p);
            // the chosen ones
            foreach (var p in new[]
            {
                "Art/audio/pm106/cikis-u1", "Art/audio/pm102/carpma-a1", "Art/audio/pm102/carpma-a4",
                "Art/audio/pm104/ui-tok-c1", "Art/audio/pm98/ui-toggle1", "Art/audio/pm106/zincir-f1", "Art/audio/pm106/zincir-f3", "Art/audio/pm105/duvar-a1", "Art/audio/pm102/booster-makas-kesim-c",
                "Art/audio/pm102/booster-degnek-donusum-a", "Art/audio/pm102/booster-saat-tiktak-b", "Art/audio/pm102/gecis-koridor-a",
                "Art/audio/pm100/gecis-dalis-a", "Art/audio/pm100/gecis-sekme-a", "Art/audio/pm96/vurus_haritasi",
            })
                Assert.That(used, Does.Contain(p));
            // the others: never loaded
            foreach (var p in used)
            {
                string name = p.Substring(p.LastIndexOf('/') + 1);
                Assert.That(name, Does.Not.Match("^booster-makas-.*-[ab]$|^booster-degnek-.*-[bc]$|^booster-cekic-.*-[ab]$|^booster-saat-.*-[ac]$|^gecis-(koridor|dalis|ting|geri|hud|sekme)-[bc]$|^gecis-cizgi-[ab]$|^cikis-[a-t]|^kalkisk?-|^carpma-[b-d]|^tik-|^ui-tick-|^ui-tok-[ab]|^ui-tok-c[23]|^(zincir|zincirger)-[a-e]|^(kilitdokun|kilitac|patlama|duvar)-[bc]"), p);
            }
            Assert.That(used.Count(p => p.Contains("pm102/carpma-a")), Is.EqualTo(4), "the knock's own four takes (pm102), not the older copies");
            Assert.That(used.Count(p => p.Contains("carpma-a") && !p.Contains("pm102/") && !p.Contains("pm107/")), Is.EqualTo(0));
            // the iOS build leaves the rest out
            var unused = IosBuild.Unused(new System.Collections.Generic.HashSet<string>(SesSecim.Used));
            Assert.That(unused, Does.Contain("Assets/_Game/Presentation/Resources/Art/audio/pm102/booster-makas-kesim-a.wav"));
            Assert.That(unused, Does.Contain("Assets/_Game/Presentation/Resources/Art/audio/pm100/carpma-a1.wav"));
            Assert.That(unused, Does.Contain("Assets/_Game/Presentation/Resources/Art/audio/pm96/kazanma-g.wav"), "the fanfare is embedded in its tone (Resources/Music)");
            Assert.That(unused.Any(u => u.Contains("vurus_haritasi")), Is.False, "the beats stay");
            Assert.That(unused.Any(u => u.EndsWith(".meta")), Is.False);
        }

        [Test]
        public void TheChosenMusicIsThereForTheIosBuild()
        {
            var lobby = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(Music.SourceOf(Music.DefaultLobby));
            var level = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(Music.SourceOf(Music.DefaultLevel));
            var win = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(Music.FanfareSource(Music.FanfareClipKey));
            Assert.That(lobby, Is.Not.Null, Music.SourceOf(Music.DefaultLobby));
            Assert.That(level, Is.Not.Null, Music.SourceOf(Music.DefaultLevel));
            Assert.That(win, Is.Not.Null, Music.FanfareSource(Music.FanfareClipKey));
            Assert.That(lobby.length, Is.EqualTo(83.478f).Within(.01f), "Urban Shop: its exact loop");
            Assert.That(level.length, Is.EqualTo(64f).Within(.01f), "Cat in Space: its two halves");
            Assert.That(win.length, Is.EqualTo(BeatMap.Of("g").length).Within(.01f), "the pizzicato (PM_107) on g's beats");
        }

        [Test]
        public void NoCandidateIsLeftOutsideResources()
        {
            Assert.That(Directory.Exists(IosBuild.Stash), Is.False, "an iOS build stopped half way: IosBuild.RestoreCandidates");
            Assert.That(Directory.Exists(IosBuild.MusicInBuild), Is.False, "the iOS build's music copies are for that build only");
        }
    }
}
