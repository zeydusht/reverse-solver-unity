using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using ReverseSolver.EditorTools;
using ReverseSolver.Presentation;
using UnityEngine;

namespace ReverseSolver.Core.Tests
{
    /* PM_105: the chained pair's knock (PlayMode: Pm105ChainKnockTests); the obstacles' sounds (zincir, zincirger,
       kilitdokun, kilitac, patlama, duvar; a-c); every button plays Durdur's tok, one take at one pitch, and the old
       "çıt" (the tick b) is gone from the project. */
    public class Pm105Tests
    {
        const string Audio = "Assets/_Game/Presentation/Resources/Art/audio/";

        [Test]
        public void EveryButtonPlaysTheOneTok()
        {
            SesSecim.Collect();
            string first = null;
            for (int i = 0; i < 12; i++)
                foreach (var kind in new[] { UiSounds.Kind.Tick, UiSounds.Kind.Tok })
                {
                    var clip = UiSounds.Pick(kind, out float pitch);
                    Assert.That(clip, Is.Not.Null, kind.ToString());
                    Assert.That(pitch, Is.EqualTo(1f), "no random pitch");
                    first ??= clip.name;
                    Assert.That(clip.name, Is.EqualTo(first), "one take");
                }
            Assert.That(first, Is.EqualTo("ui-tok-a1"), "Durdur's tok (Zeyd: liked)");
            // a plain press is the tok too: a panel's swish goes under it
            UiSounds.Take();
            UiSounds.Request(UiSounds.Kind.Tick); UiSounds.Request(UiSounds.Kind.Open);
            UiSounds.Take();
            Assert.That(UiSounds.SwishUnderTok, Is.True);
        }

        [Test]
        public void TheOldTickIsGoneEverywhere()
        {
            var files = Directory.GetFiles(Audio, "*.wav", SearchOption.AllDirectories).Select(f => f.Replace('\\', '/')).ToArray();
            Assert.That(files.Where(f => Regex.IsMatch(Path.GetFileName(f), "^(ui-tick-|tik-)")), Is.Empty, "the tick's files");
            Assert.That(Resources.Load<AudioClip>("Art/audio/pm98/ui-tick-b1"), Is.Null);
            Assert.That(Resources.Load<AudioClip>("Art/audio/pm95/tik-b1"), Is.Null);
            Assert.That(Sfx95.Letter(Sfx95.Kind.Tick), Is.EqualTo('0'));
            // no code path names it, and the UI's old fallback (PM_72's tick) is gone from the button sounds
            foreach (var f in Directory.GetFiles("Assets/_Game/Presentation", "*.cs", SearchOption.AllDirectories))
                Assert.That(File.ReadAllText(f), Does.Not.Contain("ui-tick-"), f);
            string root = File.ReadAllText("Assets/_Game/Presentation/GameRoot.cs");
            Assert.That(root, Does.Not.Contain("kind == UiSounds.Kind.Tick) clip = Art.Sound(\"pm72-tick\")"));
            Assert.That(SesSecim.Keys, Does.Not.Contain("tik"));
            Assert.That(File.ReadAllText(SesSecim.Asset), Does.Not.Contain("\"tik\""));
        }

        [Test]
        public void TheObstaclesHaveThreeCandidatesEach()
        {
            var json = File.ReadAllText("docs/art/pm105/onizleme/sesler105.json");
            foreach (var key in Sfx105.Keys)
                foreach (char l in "abc")
                {
                    var c = Resources.Load<AudioClip>($"{Sfx105.Folder}{key}-{l}1");
                    Assert.That(c, Is.Not.Null, $"{key}-{l}1");
                    float max = key == "zincir" ? .15f : key == "duvar" ? .25f : key == "kilitdokun" ? .26f : key == "zincirger" ? .3f : .75f;
                    Assert.That(c.length, Is.LessThanOrEqualTo(max), $"{key}-{l}1 length");
                    StringAssert.Contains($"\"file\": \"{key}-{l}1.wav\"", json);
                }
            // the levels against the game's (Tools/make_sfx105.py measures them: the loudest 400 ms)
            var target = Regex.Matches(json, "\"title\": \"[^\"]*\",\\s*\"lufs\": (-?[\\d.]+)");
            Assert.That(target.Count, Is.EqualTo(6));
            foreach (Match m in Regex.Matches(json, "\"file\": \"(\\w+)-[a-c]\\d\\.wav\",\\s*\"seconds\": [\\d.]+,\\s*\"lufs\": (-?[\\d.]+),\\s*\"peak\": (-?[\\d.]+)"))
            {
                float lufs = float.Parse(m.Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture);
                float peak = float.Parse(m.Groups[3].Value, System.Globalization.CultureInfo.InvariantCulture);
                Assert.That(lufs, Is.InRange(-25.5f, -11f), m.Value);
                Assert.That(peak, Is.LessThanOrEqualTo(-1.5f), m.Value);
            }
            // the rattle sits well under the slide, the wall over the knock
            float L(string k) => float.Parse(Regex.Match(json, $"\"{k}\": \\{{\\s*\"title\": \"[^\"]*\",\\s*\"lufs\": (-?[\\d.]+)").Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);
            Assert.That(L("zincir"), Is.LessThan(L("kilitac") - 6));
            Assert.That(L("duvar"), Is.GreaterThan(L("zincirger")));
        }

        [Test]
        public void TheBombIsAToyNotABoom()
        {
            foreach (char l in "abc")
            {
                var c = Resources.Load<AudioClip>($"{Sfx105.Folder}patlama-{l}1");
                var d = new float[c.samples * c.channels]; c.GetData(d, 0);
                // the share of energy under 120 Hz (a one-pole low-pass): small, no deep boom
                double lowE = 0, all = 0, y = 0, a = System.Math.Exp(-2 * System.Math.PI * 120 / c.frequency);
                for (int i = 0; i < d.Length; i += c.channels) { y = (1 - a) * d[i] + a * y; lowE += y * y; all += d[i] * d[i]; }
                Assert.That(lowE / System.Math.Max(1e-9, all), Is.LessThan(.25), $"patlama-{l}1 bass share");
            }
        }

        [Test]
        public void TheRattleFollowsTheChainsSpeed()
        {
            var r = new Sfx105.Rattle();
            int grains = 0;
            for (int f = 0; f < 120; f++) if (r.Tick(.5f, 1 / 60f, out _)) grains++;
            Assert.That(grains, Is.EqualTo(0), "a still chain (its sag's sway) is silent");
            float lastLevel = 0; int slow = 0, fast = 0;
            for (int f = 0; f < 60; f++) if (r.Tick(3, 1 / 60f, out float lv)) { slow++; lastLevel = lv; }
            float slowLevel = lastLevel;
            for (int f = 0; f < 60; f++) if (r.Tick(12, 1 / 60f, out float lv)) { fast++; lastLevel = lv; }
            Assert.That(slow, Is.GreaterThan(0));
            Assert.That(fast, Is.GreaterThan(slow), "more grains the faster it goes");
            Assert.That(lastLevel, Is.GreaterThan(slowLevel), "and louder");
            Assert.That(Sfx105.RattleLevel(Sfx105.Still), Is.EqualTo(0));
            Assert.That(Sfx105.RattleLevel(20), Is.EqualTo(1));
        }

        [Test]
        public void TheGameLoadsTheChosenObstacleSoundsAndIosLeavesTheRest()
        {
            var used = SesSecim.Collect();
            foreach (var key in Sfx105.Keys)
            {
                Assert.That(Sfx105.Letter((Sfx105.Kind)System.Array.IndexOf(Sfx105.Keys, key)), Is.EqualTo('a'), key);
                Assert.That(used, Does.Contain($"{Sfx105.Folder}{key}-a1"));
                Assert.That(used.Any(u => u.Contains($"/pm105/{key}-b") || u.Contains($"/pm105/{key}-c")), Is.False, key);
            }
            var unused = IosBuild.Unused(used);
            Assert.That(unused, Does.Contain(Audio + "pm105/patlama-b1.wav"));
            Assert.That(unused, Does.Not.Contain(Audio + "pm105/patlama-a1.wav"));
            Assert.That(unused, Does.Contain(Audio + "pm104/ui-tok-a2.wav"), "the tok's other takes");
        }

        [Test]
        public void TheSoundsAreWiredToTheirMoments()
        {
            string root = File.ReadAllText("Assets/_Game/Presentation/GameRoot.cs");
            StringAssert.Contains("_board.Clang(piece); PlaySfx105(Sfx105.Kind.KilitDokun", root);
            StringAssert.Contains("e.Outcome == Outcome.Bomb && PlaySfx105(Sfx105.Kind.Patlama, 1f)) { PlaySoundLater(\"loss\", PatlamaLead105)", root);
            StringAssert.Contains("PlaySfx105(Sfx105.Kind.KilitAc, 1f) || PlaySfx(Sfx95.Kind.Unlock, 1f)", root);
            StringAssert.Contains("wall && Sfx105.On(Sfx105.Kind.Duvar)", root);
            StringAssert.Contains("if (chained && fresh) PlaySfx105(Sfx105.Kind.ZincirGer", root);
            StringAssert.Contains("_rattle105.Tick(_board.ChainMotion", root);
        }

        [Test]
        public void TheMapListsEveryButtonsSound()
        {
            string map = File.ReadAllText("docs/TITRESIM_HARITASI.md");
            StringAssert.Contains("ui-tok-a1", map);
            var rows = map.Split('\n').Where(l => l.StartsWith("|")).ToArray();
            Assert.That(rows.Where(r => Regex.IsMatch(r, @"\|\s*tık\b") || r.Contains("ui-tick") || r.Contains("tik-")), Is.Empty, "no row plays the tick");
            foreach (var s in new[] { "zincir-a", "zincirger-a", "kilitdokun-a", "kilitac-a", "patlama-a", "duvar-a" })
                StringAssert.Contains(s, map);
        }

        [Test]
        public void TheListeningPageHasTheNewSounds()
        {
            string html = File.ReadAllText("docs/art/pm102/onizleme/sesler.html");
            foreach (var s in new[] { "sesler105.json", "Sürükle", "seszincir", "sesduvar", "seskilitac" })
                StringAssert.Contains(s, html);
            Assert.That(html, Does.Not.Contain("sestik"));
        }
    }
}
