using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using ReverseSolver.EditorTools;
using ReverseSolver.Presentation;
using UnityEditor;
using UnityEngine;

namespace ReverseSolver.Core.Tests
{
    /* PM_104_EK: iPhone Safari's "EncodingError: Decoding failed". The sounds the game plays go into the web build as
       raw samples (nothing for Safari to decode); a sound that still fails is silent, never Unity's alert. */
    public class Pm104SafariTests
    {
        const string Template = "Assets/WebGLTemplates/ReverseSolver/index.html";

        [Test]
        public void EverySoundTheGamePlaysHasItsRawSamplesForTheWeb()
        {
            var pcm = WebAudioFormat.PcmFiles();
            var used = SesSecim.Collect();
            // every chosen clip (not the beat map's json) and the old Art/audio ones
            foreach (var u in used.Where(u => new[] { ".wav", ".ogg", ".mp3" }.Any(e => File.Exists("Assets/_Game/Presentation/Resources/" + u + e))))
                Assert.That(pcm.Any(f => WebAudioFormat.ResourcePath(f) == u), u);
            foreach (var f in pcm)
            {
                var asset = AssetDatabase.LoadAssetAtPath<AudioClip>(f);
                var clip = WebPcm.Clip(Path.GetFileNameWithoutExtension(f), WebAudioFormat.Encode(f));
                Assert.That(clip, Is.Not.Null, f);
                Assert.That(clip.channels, Is.EqualTo(asset.channels), f + " channels");
                // its own rate (Unity's clip of a 22.05 kHz file says 44.1 kHz): the same length in seconds
                Assert.That(clip.length, Is.EqualTo(asset.length).Within(.005f), f + " length");
                // the same level as the clip the game plays elsewhere (the editor's is decoded Vorbis: close, not exact)
                float a = Rms(asset), r = Rms(clip);
                Assert.That(r, Is.EqualTo(a).Within(System.Math.Max(.01f, a * .1f)), f + " level");
                Object.DestroyImmediate(clip);
            }
            // the shortest effects above all: the lift (0.025 / 0.045 s) and the ticks
            Assert.That(pcm.Count(f => f.Contains("/kalkisk-") || f.Contains("/kalkis-")), Is.GreaterThanOrEqualTo(8));
        }

        static float Rms(AudioClip c)
        {
            var d = new float[c.samples * c.channels];
            if (!c.GetData(d, 0)) return -1;
            double s = 0; foreach (var v in d) s += v * v;
            return (float)System.Math.Sqrt(s / System.Math.Max(1, d.Length));
        }

        [Test]
        public void TheCandidatesStayAacAndTheFolderIsForTheBuildOnly()
        {
            var pcm = WebAudioFormat.PcmFiles();
            var unused = IosBuild.Unused(SesSecim.Collect());
            Assert.That(unused.Count, Is.GreaterThan(100));
            Assert.That(pcm.Intersect(unused), Is.Empty);
            Assert.That(Directory.Exists(WebAudioFormat.OutRoot), Is.False, "written only during the web build");
            var (files, bytes) = WebAudioFormat.Write();
            try
            {
                Assert.That(files, Is.EqualTo(pcm.Count));
                Assert.That(Resources.Load<TextAsset>(WebPcm.Folder + "Art/audio/pm100/kalkisk-f1"), Is.Not.Null);
                Assert.That(bytes, Is.LessThan(3 * 1024 * 1024));
            }
            finally { WebAudioFormat.Remove(); }
            Assert.That(Directory.Exists(WebAudioFormat.OutRoot), Is.False);
        }

        [Test]
        public void ThePageCatchesEveryDecodeError()
        {
            string html = File.ReadAllText(Template);
            // every decodeAudioData promise caught, before Unity's loader
            int guard = html.IndexOf("C.prototype.decodeAudioData = safe");
            Assert.That(guard, Is.GreaterThan(0));
            Assert.That(guard, Is.LessThan(html.IndexOf("{{{ LOADER_FILENAME }}}")));
            Assert.That(html, Does.Contain("p.catch(function (e) { fail(e); })"));
            Assert.That(html, Does.Contain("addEventListener('unhandledrejection'"));
            Assert.That(html, Does.Contain("e.stopImmediatePropagation()"));
            // Unity's direct audio alert (no Web Audio) goes to the console
            Assert.That(html, Does.Contain("window.alert = function (m)"));
            // and the Unity config's errorHandler drops an audio error (true: handled, no alert)
            Assert.That(Regex.IsMatch(html, @"errorHandler:\s*function \(message\) \{ return !!\(window\.rsAudioError"), Is.True);
            foreach (var jslib in new[] { "Assets/_Game/Platform/Web/Music.jslib", "Assets/_Game/Platform/Web/Intro.jslib" })
                Assert.That(File.ReadAllText(jslib), Does.Contain(".catch("), jslib);
        }

        [Test]
        public void TheTelemetryHasTheEvent()
        {
            Assert.That(File.ReadAllText("docs/sql/019_soft_launch.sql"), Does.Contain("'audio_decode_fail'"));
            Assert.That(File.ReadAllText("Assets/_Game/Presentation/Ui/AudioDecodeWatch.cs"), Does.Contain("SlTelemetry.Emit(\"audio_decode_fail\", value: path)"));
        }
    }
}
