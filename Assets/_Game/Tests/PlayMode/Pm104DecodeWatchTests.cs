using System.Collections;
using NUnit.Framework;
using ReverseSolver.Presentation;
using UnityEngine;
using UnityEngine.TestTools;

namespace ReverseSolver.PlayMode.Tests
{
    /* PM_104_EK: a clip that never loads (on WebGL: Safari could not decode it) is reported once, by its file, and the
       game goes on; a clip that loads is not. */
    public sealed class Pm104DecodeWatchTests
    {
        static IEnumerator Wait(float s) { float end = Time.realtimeSinceStartup + s; while (Time.realtimeSinceStartup < end) yield return null; }

        [UnityTest]
        public IEnumerator AFailedClipIsReportedOnceAndTheGameGoesOn()
        {
            var good = AudioClip.Create("pm104-good", 441, 1, 44100, false);
            var bad = AudioClip.Create("pm104-bad", 441, 1, 44100, false);
            AudioDecodeWatch.Watch("test/pm104-good", good);
            AudioDecodeWatch.Watch("test/pm104-bad", bad);
            Object.Destroy(bad);   // gone: as Failed as a clip Safari could not decode
            LogAssert.Expect(LogType.Warning, "[audio] decode fail test/pm104-bad");
            yield return Wait(1.2f);
            Assert.That(AudioDecodeWatch.Failed, Does.Contain("test/pm104-bad"));
            Assert.That(AudioDecodeWatch.Failed, Does.Not.Contain("test/pm104-good"));
            AudioDecodeWatch.Watch("test/pm104-bad", bad);   // once
            yield return Wait(.7f);
            Assert.That(System.Linq.Enumerable.Count(AudioDecodeWatch.Failed, f => f == "test/pm104-bad"), Is.EqualTo(1));
            Object.Destroy(good);
        }
    }
}
