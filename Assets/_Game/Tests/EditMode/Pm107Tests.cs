using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using ReverseSolver.EditorTools;
using ReverseSolver.Presentation;
using UnityEngine;

namespace ReverseSolver.Core.Tests
{
    /* PM_107: Zeyd's final sounds (24 Nov 23:59) as the defaults; the chain heard whenever a chained pair moves (sliding
       out too); the mix: buses with their ducks (the clock's tick-tock in front), the chosen sounds mastered to their
       levels (pm107), the same clip at most twice at once, a limiter on the web's output; the dive's air following
       the motion. */
    public class Pm107Tests
    {
        const string Audio = "Assets/_Game/Presentation/Resources/Art/audio/";

        [Test]
        public void ZeydsTableIsTheDefault()
        {
            foreach (var (k, v) in new[] { ("kazanmaton", "pizz"), ("cikis", "u"), ("cekic", "c"), ("saat", "b"), ("cizgi", "c"), ("dugme", "c"), ("zincir", "f"), ("zincirger", "f") })
                Assert.That(SesSecim.Of(k), Is.EqualTo(v), k);
            SesSecim.Collect();
            Assert.That(Music.Tone, Is.EqualTo("pizz"));
            Assert.That(Sfx95.Letter(Sfx95.Kind.Exit), Is.EqualTo('u'));
            Assert.That(BoosterSounds102.Letter("cekic"), Is.EqualTo('c'));
            Assert.That(BoosterSounds102.Letter("saat"), Is.EqualTo('b'));
            Assert.That(Gecis100.Letter(Gecis100.Kind.Cizgi), Is.EqualTo('c'));
            Assert.That(Sfx105.Letter(Sfx105.Kind.Zincir), Is.EqualTo('f'));
            Assert.That(Sfx105.Letter(Sfx105.Kind.ZincirGer), Is.EqualTo('f'));
            Assert.That(UiSounds.Pick(UiSounds.Kind.Tok, out _).name, Is.EqualTo("ui-tok-c1"));
            StringAssert.Contains("ton:'pizz', cikis:'u'", File.ReadAllText("docs/art/pm102/onizleme/sesler.html"));
        }

        [Test]
        public void TheChosenSoundsPlayMastered()
        {
            var used = SesSecim.Collect();
            var chosen = SesSecim.Chosen();
            Assert.That(SesSecim.Replaced, Is.Not.Empty);
            foreach (var original in SesSecim.Replaced)
            {
                string name = original.Substring(original.LastIndexOf('/') + 1);
                Assert.That(used, Does.Contain(SesSecim.Mastered + name), name);
                Assert.That(used, Does.Not.Contain(original), "the original stays out of the build: " + original);
            }
            foreach (var n in new[] { "cikis-u1", "booster-saat-tiktak-b", "zincir-f1", "ui-tok-c1", "gecis-dalis-a", "gecis-koridor-a" })
                Assert.That(used, Does.Contain(SesSecim.Mastered + n), n);
            Assert.That(chosen, Does.Contain("Art/audio/pm106/cikis-u1"));
            // the levels (docs/art/pm107/miks/hedefler.json, Tools/make_miks107.py): the tick-tock up into the mix, the dive heard
            string json = File.ReadAllText("docs/art/pm107/miks/hedefler.json");
            float After(string clip) => float.Parse(Regex.Match(json, "\"" + Regex.Escape(clip) + "\": \\{[\\s\\S]*?\"after\": \\{\\s*\"lufs\": (-?[\\d.]+)").Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);
            Assert.That(After("booster-saat-tiktak-b"), Is.InRange(-23f, -20f), "the tick-tock (was -29.6: drowned by the slides)");
            Assert.That(After("cikis-u1") - After("booster-saat-tiktak-b") + Mix107.ClockSfxDb, Is.LessThanOrEqualTo(1f), "under the clock the slide is no louder than the tick-tock");
            Assert.That(After("gecis-dalis-a"), Is.GreaterThan(-26f), "the dive (was -33)");
            foreach (Match m in Regex.Matches(json, "\"after\": \\{\\s*\"lufs\": -?[\\d.]+,\\s*\"peak\": (-?[\\d.]+)"))
                Assert.That(float.Parse(m.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture), Is.LessThanOrEqualTo(-1.4f), "every peak under -1.5 dBFS");
            // the web carries the mastered copies as raw samples; iOS only them
            var pcm = WebAudioFormat.PcmFiles();
            Assert.That(pcm, Does.Contain(Audio + "pm107/cikis-u1.wav"));
            Assert.That(pcm, Does.Not.Contain(Audio + "pm106/cikis-q1.wav"), "the candidates no longer tried (Zeyd chose)");
            var unused = IosBuild.Unused(used);
            Assert.That(unused, Does.Contain(Audio + "pm106/cikis-u1.wav"), "iOS: the mastered copy only");
            Assert.That(unused, Does.Not.Contain(Audio + "pm107/cikis-u1.wav"));
        }

        [Test]
        public void TheBusesDuckAsTheMixSays()
        {
            Mix107.Clock(false); Mix107.FanfareEnd();
            Assert.That(Mix107.TargetDb(Mix107.Bus.Sfx), Is.EqualTo(0));
            Mix107.Clock(true);
            Assert.That(Mix107.TargetDb(Mix107.Bus.Sfx), Is.EqualTo(-5f), "the slides step back for the tick-tock");
            Assert.That(Mix107.TargetDb(Mix107.Bus.Music), Is.EqualTo(-3f));
            Assert.That(Mix107.TargetDb(Mix107.Bus.Clock), Is.EqualTo(0), "the tick-tock never ducks");
            Mix107.Clock(false);
            Mix107.Fanfare(3);
            Assert.That(Mix107.TargetDb(Mix107.Bus.Sfx), Is.EqualTo(-6f));
            Assert.That(Mix107.TargetDb(Mix107.Bus.Bed), Is.EqualTo(-6f));
            Assert.That(Mix107.TargetDb(Mix107.Bus.Ui), Is.EqualTo(-3f));
            Mix107.FanfareEnd();
            Mix107.Bang();
            Assert.That(Mix107.TargetDb(Mix107.Bus.Music), Is.LessThan(0));
            // the glide: down fast, up slow
            for (int i = 0; i < 10; i++) Mix107.Tick(.01f);
            Assert.That(Mix107.Db(Mix107.Bus.Music), Is.EqualTo(Mix107.BangMusicDb).Within(.01f));
            string root = File.ReadAllText("Assets/_Game/Presentation/GameRoot.cs");
            StringAssert.Contains("Mix107.Tick(Time.unscaledDeltaTime);", root);
            StringAssert.Contains("Mix107.Bang();", root);
            StringAssert.Contains("Mix107.Clock(on && clip != null);", File.ReadAllText("Assets/_Game/Presentation/Ui/BoosterSounds102.cs"));
        }

        [Test]
        public void TheSameClipAtMostTwiceAtOnce()
        {
            var clip = AudioClip.Create("pm107-voice", 4410, 1, 44100, false);
            try
            {
                Assert.That(Mix107.Admit(clip), Is.True);
                Assert.That(Mix107.Admit(clip), Is.True);
                Assert.That(Mix107.Admit(clip), Is.False, "a third copy on top: a crack");
            }
            finally { Object.DestroyImmediate(clip); }
        }

        [Test]
        public void TheWebOutputIsLimited()
        {
            string page = File.ReadAllText("Assets/WebGLTemplates/ReverseSolver/index.html");
            StringAssert.Contains("createDynamicsCompressor", page);
            StringAssert.Contains("k.threshold.value = -4; k.knee.value = 0; k.ratio.value = 20;", page);
            StringAssert.Contains("RS_MusicMix", File.ReadAllText("Assets/_Game/Platform/Web/Music.jslib"));
        }

        [Test]
        public void TheChainRattlesWheneverAChainedPairMoves()
        {
            string b = File.ReadAllText("Assets/_Game/Presentation/Board/BoardView.cs");
            StringAssert.Contains("foreach (var r in _leavingRopes) m = Mathf.Max(m, r.Motion);", b);
            StringAssert.Contains("_leavingRopes.Add(leaving);", b);
            string root = File.ReadAllText("Assets/_Game/Presentation/GameRoot.cs");
            StringAssert.Contains("_bedQuietUntil = 0;", root);
            Assert.That(root, Does.Not.Contain("if (_board == null || _session == null || _session.IsOver) { Bed106(0); return; }"));
        }

        [Test]
        public void TheDiveIsHeardFromItsStart()
        {
            Assert.That(Dive75.Bell107(0), Is.EqualTo(0));
            Assert.That(Dive75.Bell107(.5f), Is.EqualTo(1).Within(1e-5f));
            Assert.That(Dive75.Bell107(.1f), Is.GreaterThan(.1f), "heard early in the motion");
            Assert.That(Dive75.Bell107(1), Is.EqualTo(0), "gone at the arrival");
            StringAssert.Contains("Gecis100.Dive(t < h0 ? Bell107(t / Mathf.Max(.05f, h0)) : 0);", File.ReadAllText("Assets/_Game/Presentation/Ui/Dive75.cs"));
        }
    }
}
