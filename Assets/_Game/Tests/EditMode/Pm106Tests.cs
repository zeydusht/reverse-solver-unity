using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using ReverseSolver.EditorTools;
using ReverseSolver.Presentation;
using UnityEngine;

namespace ReverseSolver.Core.Tests
{
    /* PM_106: one sound per press (Ayarlar sounded twice); no ting at the dive's touch; the exit in Block Out's manner
       (docs/art/pm106/BLOCKOUT_SES_ANALIZ.md: q-u, one take, one pitch, no lift-off; &cikisperde=1 a ladder); a real
       chain (d-f: loops played as a bed that follows the chain's speed); the rubber on every jam (PlayMode:
       Pm106RubberTests). */
    public class Pm106Tests
    {
        const string Audio = "Assets/_Game/Presentation/Resources/Art/audio/";

        [Test]
        public void APressIsOneSound()
        {
            UiSounds.Take();
            UiSounds.Request(UiSounds.Kind.Tick); UiSounds.AsTok(); UiSounds.Request(UiSounds.Kind.Open);   // Ayarlar
            Assert.That(UiSounds.Take(), Is.EquivalentTo(new[] { UiSounds.Kind.Tok }), "the tok alone, no swish beside it");
            UiSounds.Request(UiSounds.Kind.Tok); UiSounds.Request(UiSounds.Kind.Close);   // the X
            Assert.That(UiSounds.Take(), Is.EquivalentTo(new[] { UiSounds.Kind.Tok }));
            UiSounds.Request(UiSounds.Kind.Open);   // a panel on its own keeps its swish
            Assert.That(UiSounds.Take(), Is.EquivalentTo(new[] { UiSounds.Kind.Open }));
        }

        [Test]
        public void TheDiveEndsWithoutATing()
        {
            Gecis100.Load();
            Assert.That(Gecis100.Clip(Gecis100.Kind.Ting), Is.Null);
            Assert.That(Gecis100.Clip(Gecis100.Kind.Dalis), Is.Not.Null, "the whoosh stays");
            Assert.That(File.ReadAllText("Assets/_Game/Presentation/Ui/Dive75.cs"), Does.Not.Contain("Play(Gecis100.Kind.Ting)"));
        }

        [Test]
        public void TheAnalysisCameFirstAndAnswersEveryHypothesis()
        {
            string md = File.ReadAllText("docs/art/pm106/BLOCKOUT_SES_ANALIZ.md");
            foreach (var s in new[] { "Neden iyi hissettiriyor", "| 1 |", "| 2 |", "| 3 |", "| 4 |", "| 5 |", "**Doğru**", "**Yanlış**", "Ham sayılar" })
                StringAssert.Contains(s, md);
            foreach (var f in new[] { "blockout_birakma.wav", "blockout_cikis.wav", "blockout_seri.wav" })
                Assert.That(File.Exists("docs/art/pm106/referans/" + f), f);
        }

        [Test]
        public void TheExitIsBlockOutsOneTake()
        {
            SesSecim.Collect();
            Assert.That(SesSecim.Letter("cikis"), Is.EqualTo('u'), "PM_107: Zeyd's u");
            Assert.That(Sfx95.ExitFixed, Is.True);
            foreach (char l in "qrstu")
            {
                var c = Resources.Load<AudioClip>($"Art/audio/pm106/cikis-{l}1");
                Assert.That(c, Is.Not.Null, $"cikis-{l}1");
                Assert.That(c.length, Is.LessThanOrEqualTo(.6f), $"cikis-{l}1 short");
                Assert.That(Resources.Load<AudioClip>($"Art/audio/pm106/cikis-{l}2"), Is.Null, "one take");
            }
            for (int i = 0; i < 8; i++)
            {
                var clip = Sfx95.Pick(Sfx95.Kind.Exit, out float pitch);
                Assert.That(clip.name, Is.EqualTo("cikis-u1"));
                Assert.That(pitch, Is.EqualTo(1f), "no random pitch");
            }
            Assert.That(Sfx95.On(Sfx95.Kind.Lift), Is.False, "no lift-off: silent until it lands");
            Assert.That(Sfx95.On(Sfx95.Kind.LiftShort), Is.False);
            // the levels: the exits at the knock (+1, played 1 dB down), the tok's energy low (no hiss)
            var json = File.ReadAllText("docs/art/pm106/onizleme/sesler106.json");
            foreach (Match m in Regex.Matches(json, "\"file\": \"cikis-[q-u]1\\.wav\",\\s*\"seconds\": [\\d.]+,\\s*\"lufs\": (-?[\\d.]+),\\s*\"peak\": (-?[\\d.]+)"))
            {
                float lufs = float.Parse(m.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);
                Assert.That(lufs, Is.InRange(-15f, -11.5f), m.Value);
                Assert.That(float.Parse(m.Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture), Is.LessThanOrEqualTo(-1.9f), m.Value);
            }
            Assert.That(Regex.Matches(json, "\"file\": \"cikis-").Count, Is.EqualTo(5), "five candidates");
        }

        [Test]
        public void TheLadderIsOnlyAFlag()
        {
            Assert.That(GameRoot.LadderPitch(0), Is.EqualTo(1f));
            Assert.That(GameRoot.LadderPitch(1), Is.EqualTo(Mathf.Pow(2, 1 / 12f)).Within(1e-5f), "a semitone up");
            Assert.That(GameRoot.LadderPitch(GameRoot.LadderSteps), Is.EqualTo(1f), "back to the start after five");
            string root = File.ReadAllText("Assets/_Game/Presentation/GameRoot.cs");
            StringAssert.Contains("Query(\"cikisperde\") == \"1\"", root);
            StringAssert.Contains("if (e.Kind == EventKind.Jammed || (e.Kind == EventKind.Finished && e.Outcome != Outcome.Win)) ResetLadder106();", root);
        }

        [Test]
        public void TheChainIsARealChainPlayedAsABed()
        {
            SesSecim.Collect();
            Assert.That(Sfx105.Letter(Sfx105.Kind.Zincir), Is.EqualTo('f'), "PM_107: Zeyd's f");
            Assert.That(Sfx105.Letter(Sfx105.Kind.ZincirGer), Is.EqualTo('f'));
            Assert.That(Sfx105.Continuous, Is.True);
            Assert.That(Sfx105.Takes(Sfx105.Kind.Zincir).Length, Is.EqualTo(3), "three loops");
            foreach (char l in "def")
                for (int n = 1; n <= 3; n++)
                {
                    var c = Resources.Load<AudioClip>($"{Sfx105.Folder106}zincir-{l}{n}");
                    Assert.That(c, Is.Not.Null, $"zincir-{l}{n}");
                    Assert.That(c.length, Is.InRange(1f, 1.5f), "a loop, not a grain");
                    // the links' body: energy under 4 kHz (PM_105's grains were all 8.8 kHz hiss)
                    var d = new float[c.samples * c.channels]; c.GetData(d, 0);
                    double low = 0, all = 0, y = 0, a = System.Math.Exp(-2 * System.Math.PI * 4000 / c.frequency);
                    for (int i = 0; i < d.Length; i += c.channels) { y = (1 - a) * d[i] + a * y; low += y * y; all += d[i] * d[i]; }
                    Assert.That(low / all, Is.GreaterThan(.1), $"zincir-{l}{n} under 4 kHz");
                }
            // the bed: silent when still; a loop when it moves; another (never the same) at the loop's end; louder faster
            var bed = new Sfx105.Bed();
            for (int f = 0; f < 30; f++) Assert.That(bed.Tick(.5f, 1 / 60f, 3, -1), Is.EqualTo(-1));
            Assert.That(bed.Volume, Is.EqualTo(0));
            int first = bed.Tick(4, 1 / 60f, 3, -1);
            Assert.That(first, Is.InRange(0, 2));
            for (int f = 0; f < 20; f++) Assert.That(bed.Tick(4, 1 / 60f, 3, 1f), Is.EqualTo(-1), "plays on");
            float slow = bed.Volume;
            int last = first;
            for (int k = 0; k < 20; k++)
            {
                int next = bed.Tick(4, 1 / 60f, 3, .02f);   // the loop ending
                Assert.That(next, Is.Not.EqualTo(last), "never the same loop twice");
                last = next;
            }
            for (int f = 0; f < 30; f++) bed.Tick(12, 1 / 60f, 3, 1f);
            Assert.That(bed.Volume, Is.GreaterThan(slow), "louder the faster");
            for (int f = 0; f < 30; f++) bed.Tick(0, 1 / 60f, 3, 1f);
            Assert.That(bed.Volume, Is.EqualTo(0), "it dies away when the chain stops");
            StringAssert.Contains("if (Sfx105.Continuous) { Bed106(Time.unscaledTime < _bedQuietUntil ? 0 : _board.ChainMotion); return; }", File.ReadAllText("Assets/_Game/Presentation/GameRoot.cs"));
            // the taut ting: the same chain
            foreach (char l in "def")
                Assert.That(Resources.Load<AudioClip>($"{Sfx105.Folder106}zincirger-{l}1"), Is.Not.Null);
        }

        [Test]
        public void TheWebCarriesTheCandidatesBeingTried()
        {
            // PM_107: Zeyd chose (u, f); the web carries the chosen ones (their mastered copies), not every candidate
            var files = WebAudioFormat.PcmFiles();
            Assert.That(files, Does.Contain(Audio + "pm107/cikis-u1.wav"));
            Assert.That(files, Does.Contain(Audio + "pm107/zincir-f3.wav"));
            Assert.That(files, Does.Not.Contain(Audio + "pm106/cikis-q1.wav"));
            Assert.That(IosBuild.CandidateFolders, Does.Contain("pm106"));
            var unused = IosBuild.Unused(SesSecim.Collect());
            Assert.That(unused, Does.Contain(Audio + "pm106/cikis-q1.wav"), "iOS: only the chosen");
            Assert.That(unused, Does.Not.Contain(Audio + "pm107/cikis-u1.wav"));
        }

        [Test]
        public void EveryJamSquashesAlike()
        {
            string b = File.ReadAllText("Assets/_Game/Presentation/Board/BoardView.cs");
            StringAssert.Contains("float squash1 = RubberSquashAt(Rubber), squash = squash1;", b);
            Assert.That(b, Does.Not.Contain("RubberNudgeSquash"));
            StringAssert.Contains("(Session.NailAt(piece) > 0 || (!TravelRule.Limit(Session.Board, piece, dir).Exit && Contact.Distance(Session.Board, piece, dir) < SlamMin))", b);
            Assert.That(BoardView.RubberSquashAt(2), Is.EqualTo(.38f).Within(1e-4f));
        }

        [Test]
        public void TheListeningPageHasTheNewSounds()
        {
            string html = File.ReadAllText("docs/art/pm102/onizleme/sesler.html");
            foreach (var s in new[] { "sesler106.json", "Sürükleme / çıkış (Block Out tarzı)", "Block Out ile", "10 art arda", "Zincir (gerçek zincir kaydı)", "dragBed", "cikisperde" })
                StringAssert.Contains(s, html);
            Assert.That(html, Does.Not.Contain("with:'ting'"));
            StringAssert.Contains("cikis:'u'", html);
        }

        /* PM_106_EK2: the page was blank on the iPhone (and everywhere): one apostrophe ("Durdur'daki") in a quoted hint
           stopped the one script that built every section. Now each section is its own <script> (a syntax error or a
           throw leaves the others and says "yüklenemedi" in its place), and Tools/webkit_sesler.js opened it in WebKit at
           390 x 844 and played every section's first sound. */
        [Test]
        public void TheListeningPageStandsSectionBySection()
        {
            string html = File.ReadAllText("docs/art/pm102/onizleme/sesler.html");
            Assert.That(Regex.Matches(html, "<script data-sec=\"[^\"]+\">\\s*block\\('").Count, Is.GreaterThanOrEqualTo(10), "a script per section");
            StringAssert.Contains("function block(title, fn)", html);
            StringAssert.Contains("Bu bölüm yüklenemedi", html);
            StringAssert.Contains("if (!RAN[s.dataset.sec]) failed(", html);
            Assert.That(html, Does.Not.Contain("(Durdur'daki"));
            string run = File.ReadAllText("docs/art/pm106/sesler_webkit.json");
            StringAssert.Contains("\"pass\": true", run);
            StringAssert.Contains("\"viewport\": \"390x844\"", run);
            Assert.That(Regex.IsMatch(run, "\"broken\": true"), Is.False, "no section yüklenemedi");
            Assert.That(Regex.IsMatch(run, "\"decodeErrors\": \\[\\s*\""), Is.False, "no decode failed");
            Assert.That(File.Exists("docs/art/pm106/sesler_webkit.png"));
        }
    }
}
