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

        /* §D2: the collection's detail and its cards in the new palette; the grid's cards from thumbnails, one picture
           decoded per frame (the first scroll froze for 1 s at a phone's CPU pace: the "jumping background"). */
        [Test]
        public void TheCollectionIsInTheNewPaletteAndLight()
        {
            string c = File.ReadAllText("Assets/_Game/Presentation/Ui/CollectionScreen.cs");
            StringAssert.Contains("if (MenuUi.T48) { BuildT48(screen, c, w, h, index, images, order, bestSeconds); return; }", c);
            string t48 = c.Substring(c.IndexOf("void BuildT48("));
            t48 = t48.Substring(0, t48.IndexOf("static SpriteRenderer Place("));
            foreach (var old in new[] { "modal-panel", "title-plaque", "name-band", "button-secondary", "portrait-frame" })
                Assert.That(t48, Does.Not.Contain(old), "no old turquoise part in the detail: " + old);
            StringAssert.Contains("\"level-plate\"", c);
            StringAssert.Contains("thumb: true", c);
            Assert.That(LevelImages.Url(LevelImages.ThumbOf("painting-07")), Is.EqualTo(LevelImages.PaintingsUrl + "kucuk/painting-07.jpg"));
            string li = File.ReadAllText("Assets/_Game/Presentation/Board/LevelImages.cs");
            StringAssert.Contains("while (_decodeFrame == Time.frameCount) yield return null;", li);
            StringAssert.Contains("if (!thumb) Tones[tex] = ToneGrid(tex);", li);
        }

        /* §D3, §D4, §D5: the tab bar from Codex's chosen option (the files MenuTabs draws, their sizes and slicing kept),
           one gold coin everywhere, the badge with the level. */
        [Test]
        public void TheBarTheCoinAndTheBadge()
        {
            const string Art = "Assets/_Game/Presentation/Resources/Art/";
            var sizes = new (string f, int w, int h)[] { ("yeni/pm86/menu/pm86-tab-strip.png", 1170, 240), ("yeni/pm86/menu/pm86-tab-selected.png", 420, 270),
                ("yeni/pm86/menu/pm86-tab-divider.png", 24, 200), ("t21-tab-icon-home-normal.png", 168, 148), ("t21-tab-icon-trophy-normal.png", 168, 148) };
            foreach (var (f, w, h) in sizes)
            {
                var tex = new Texture2D(2, 2); tex.LoadImage(File.ReadAllBytes(Art + f));
                Assert.That((tex.width, tex.height), Is.EqualTo((w, h)), f);
                Object.DestroyImmediate(tex);
            }
            StringAssert.Contains("spriteBorder: {x: 70, y: 40, z: 70, w: 70}", File.ReadAllText(Art + "yeni/pm86/menu/pm86-tab-selected.png.meta"));
            foreach (var d in new[] { "PUANLAR.md", "SECENEKLER.md", "KONTROL.md" })
            {
                Assert.That(File.Exists("docs/art/pm107/codex/alt_cubuk/" + d), "alt_cubuk " + d);
                Assert.That(File.Exists("docs/art/pm107/codex/altin/" + d), "altin " + d);
            }
            // one coin: every coin file the game draws is the same picture (at its own size)
            Assert.That(Resources.Load<Sprite>("Art/yeni/pm107/altin"), Is.Not.Null);
            float[] Mean(string f) { var t = new Texture2D(2, 2); t.LoadImage(File.ReadAllBytes(Art + f)); var p = t.GetPixels32(); double r = 0, g = 0, b = 0, n = 0; foreach (var c in p) if (c.a > 200) { r += c.r; g += c.g; b += c.b; n++; } Object.DestroyImmediate(t); return new[] { (float)(r / n), (float)(g / n), (float)(b / n) }; }
            var coin = Mean("yeni/pm107/altin.png");
            foreach (var f in new[] { "yeni/T50/win/coin.png", "yeni/devam_v2/continue-coin.png", "yeni/win21/Reward/Flight/coin.png", "m13-token-coin.png" })
            {
                var m = Mean(f);
                for (int k = 0; k < 3; k++) Assert.That(m[k], Is.EqualTo(coin[k]).Within(6f), f);
            }
            string screens = File.ReadAllText("Assets/_Game/Presentation/Ui/Screens.cs");
            StringAssert.Contains("Sprite(Coin107, new Vector2(coinLeft + 20, hy), Vector2.one * 40, order + 3);", screens);
            StringAssert.Contains("string.Format(Texts.CollectionLevel, level)", screens);
            StringAssert.Contains("Top75(screen, safe, order, player, next + 1)", screens);
        }

        /* §D6: the lesson card's line readable, an achievement's condition on one line. */
        [Test]
        public void TheSmallTextsAreReadable()
        {
            StringAssert.Contains(".Title(title).Body(body, sizePt: 16, gapAfter: 18, maxLines: 3)", File.ReadAllText("Assets/_Game/Presentation/Ui/Screens.cs"));
            StringAssert.Contains("tmp.textWrappingMode = TMPro.TextWrappingModes.NoWrap;", File.ReadAllText("Assets/_Game/Presentation/Ui/AchievementsScreen.cs"));
            Assert.That(File.Exists("docs/art/pm107/DENETIM.md"));
            // the avatars not chosen: a navy disc with a gold ring (no old turquoise); the gold counters without a box
            StringAssert.Contains(".Fill(Draw.Hex(\"#234670\")).Stroke(Draw.Hex(\"#E3B54E\"), 3)", File.ReadAllText("Assets/_Game/Presentation/Ui/JigglesScreen.cs"));
            string win = File.ReadAllText("Assets/_Game/Presentation/Ui/P21WinView.cs");
            Assert.That(win, Does.Not.Contain("Reward/Flight/coin-counter"), "no box under the gold count");
            StringAssert.Contains("v.At(195, 704)", File.ReadAllText("Assets/_Game/Presentation/Ui/LossView74.cs"));
        }

        /* §C: the scenarios before and after on the listening page (effects and music together, mp3: Safari plays it). */
        [Test]
        public void TheListeningPageHasTheMixBeforeAndAfter()
        {
            string html = File.ReadAllText("docs/art/pm102/onizleme/sesler.html");
            StringAssert.Contains("<script data-sec=\"Miksaj: önce / sonra (PM_107)\">", html);
            foreach (var sc in new[] { "bolum", "saat", "zincir", "bomba", "kazanma", "menu", "dalis" })
                foreach (var t in new[] { "once", "sonra" })
                    Assert.That(File.Exists($"docs/art/pm107/onizleme/miks/{sc}-{t}.mp3"), sc + " " + t);
        }
    }
}
