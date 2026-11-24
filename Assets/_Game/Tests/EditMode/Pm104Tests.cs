using System.IO;
using System.Linq;
using NUnit.Framework;
using ReverseSolver.EditorTools;
using ReverseSolver.Presentation;
using UnityEngine;

namespace ReverseSolver.Core.Tests
{
    /* PM_104: the listening page plays what the game plays; one sound per booster event; the slide's swoosh candidates
       (k-p) and the buttons' tok (a-g); the page's "Bugünkü seçim". */
    public class Pm104Tests
    {
        const string Audio = "Assets/_Game/Presentation/Resources/Art/audio/";

        static float Rms(AudioClip c)
        {
            var d = new float[c.samples * c.channels];
            c.GetData(d, 0);
            double s = 0; foreach (var v in d) s += v * v;
            return (float)System.Math.Sqrt(s / System.Math.Max(1, d.Length));
        }

        [Test]
        public void ABoosterEventPlaysOnlyItsOwnSound()
        {
            SesSecim.Collect();
            Assert.That(BoosterSounds102.On(BoosterSounds102.Part.MakasKesim), Is.True);
            Assert.That(BoosterSounds102.On(BoosterSounds102.Part.CekicVurus), Is.True);
            string root = File.ReadAllText("Assets/_Game/Presentation/GameRoot.cs");
            // the scissors' cut: the old "piece" tok only without the snip; the hammer: no slide beside its blow
            StringAssert.Contains("e.Kind == EventKind.JointCut) { if (!BoosterSounds102.On(BoosterSounds102.Part.MakasKesim)) PlaySound(\"piece\"); }", root);
            StringAssert.Contains("else if (e.ByHammer && !bomb && BoosterSounds102.On(BoosterSounds102.Part.CekicVurus))", root);
        }

        [Test]
        public void TheSwooshCandidatesAreShortSoftAndAtTodaysLevel()
        {
            foreach (char l in "klmnop")
                for (int n = 1; n <= 4; n++)
                {
                    var c = Resources.Load<AudioClip>($"Art/audio/pm104/cikis-{l}{n}");
                    Assert.That(c, Is.Not.Null, $"cikis-{l}{n}");
                    Assert.That(c.length, Is.InRange(.12f, .26f), $"cikis-{l}{n} length");
                    Assert.That(Resources.Load<AudioClip>($"Art/audio/pm104/kalkis-{l}{n}"), Is.Not.Null);
                    Assert.That(Resources.Load<AudioClip>($"Art/audio/pm104/kalkisk-{l}{n}"), Is.Not.Null);
                    // a soft start: the first 10 ms well under the loudest part
                    var d = new float[c.samples * c.channels]; c.GetData(d, 0);
                    int w = (int)(c.frequency * .01f) * c.channels;
                    float head = d.Take(w).Max(System.Math.Abs), max = d.Max(System.Math.Abs);
                    Assert.That(head, Is.LessThan(max * .6f), $"cikis-{l}{n} soft start");
                }
            // as loud as today's f (the loudest 400 ms, LUFS, measured by Tools/make_sfx104.py: -15.0)
            var lufs = System.Text.RegularExpressions.Regex.Matches(File.ReadAllText("docs/art/pm104/onizleme/sesler104.json"), @"""file"": ""cikis-[k-p]\d\.wav"",\s*""seconds"": [\d.]+,\s*""lufs"": (-?[\d.]+)");
            Assert.That(lufs.Count, Is.EqualTo(24));
            foreach (System.Text.RegularExpressions.Match m in lufs)
                Assert.That(float.Parse(m.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture), Is.InRange(-16.5f, -13.5f));
        }

        [Test]
        public void TheSlideFollowsTheSpeed()
        {
            Assert.That(GameRoot.ExitBySpeed(-1), Is.EqualTo((1f, 1f)), "not known: as before");
            var slow = GameRoot.ExitBySpeed(2); var fast = GameRoot.ExitBySpeed(60);
            Assert.That(slow.pitch, Is.LessThan(1)); Assert.That(slow.volume, Is.LessThan(1));
            Assert.That(fast.pitch, Is.GreaterThan(1)); Assert.That(fast.volume, Is.GreaterThan(1));
            Assert.That(fast.pitch, Is.LessThanOrEqualTo(1.05f));
        }

        [Test]
        public void TheButtonsTok()
        {
            Assert.That(SesSecim.Of("dugme"), Is.EqualTo("a"));
            Assert.That(UiSounds.DugmeLetter, Is.EqualTo('a'));
            foreach (char l in "abcdefg")
            {
                var c = Resources.Load<AudioClip>($"Art/audio/pm104/ui-tok-{l}1");
                Assert.That(c, Is.Not.Null, $"ui-tok-{l}1");
                Assert.That(c.length, Is.LessThanOrEqualTo(.125f));
            }
            // a plain tick turns into the tok; a switch still sounds a switch; a panel's swish goes under the tok
            UiSounds.Take();
            UiSounds.Request(UiSounds.Kind.Tick); UiSounds.AsTok(); UiSounds.Request(UiSounds.Kind.Open);
            var t = UiSounds.Take();
            Assert.That(t, Is.EquivalentTo(new[] { UiSounds.Kind.Tok, UiSounds.Kind.Open }));
            Assert.That(UiSounds.SwishUnderTok, Is.True);
            UiSounds.Request(UiSounds.Kind.Toggle); UiSounds.AsTok();
            Assert.That(UiSounds.Take(), Is.EquivalentTo(new[] { UiSounds.Kind.Toggle }));
            Assert.That(UiSounds.SwishUnderTok, Is.False);
            Assert.That(UiSounds.Pick(UiSounds.Kind.Tok, out _).name, Does.StartWith("ui-tok-a"));
        }

        [Test]
        public void OnlyTheChosenOnesAreLoadedAndTheRestStaysOutOfIos()
        {
            var used = SesSecim.Collect();
            Assert.That(used, Does.Contain("Art/audio/pm104/ui-tok-a1"));
            Assert.That(used.Any(u => u.Contains("/pm104/cikis-") || u.Contains("/pm104/ui-tok-b")), Is.False);
            var unused = IosBuild.Unused(used);
            Assert.That(unused, Does.Contain(Audio + "pm104/cikis-k1.wav"));
            Assert.That(unused, Does.Contain(Audio + "pm104/ui-tok-g1.wav"));
            Assert.That(unused, Does.Not.Contain(Audio + "pm104/ui-tok-a1.wav"));
        }

        [Test]
        public void TheListeningPagePlaysWhatTheGamePlays()
        {
            string html = File.ReadAllText("docs/art/pm102/onizleme/sesler.html");
            foreach (var s in new[] { "Bugünkü seçim", "Kaydırma (swoosh)", "Düğme (tok)", "10 art arda", "bugun.json", "sesler104.json", "dugme:'a'" })
                StringAssert.Contains(s, html);
            string today = File.ReadAllText("docs/art/pm104/onizleme/bugun.json");
            foreach (var f in new[] { "booster-makas-kesim-c.wav", "cikis-f1.wav", "carpma-a1.wav", "ui-tok-a1.wav", "gecis-sekme-a.wav" })
                StringAssert.Contains(f, today);
            // the page's copies are the game's files, byte for byte
            foreach (var (page, game) in new[] { ("cikis-f1.wav", "pm100/cikis-f1.wav"), ("carpma-a1.wav", "pm102/carpma-a1.wav"), ("booster-makas-kesim-c.wav", "pm102/booster-makas-kesim-c.wav") })
                Assert.That(File.ReadAllBytes("docs/art/pm104/onizleme/sesler/" + page), Is.EqualTo(File.ReadAllBytes(Audio + game)), page);
        }
    }
}
