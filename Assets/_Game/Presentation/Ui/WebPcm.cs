using System.Collections.Generic;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_104_EK: iPhone Safari's "EncodingError: Decoding failed". On the web Unity turns every clip into AAC (its
       importer has no WebGL override: PCM is refused) and the browser decodes it with WebAudio; Safari fails on some.
       So the web build carries the sounds the game plays as raw samples too (Resources/WebPcm/<path>.bytes, written
       for the build only by WebAudioFormat), and the game makes its clips from them (AudioClip.Create + SetData: Unity
       hands WebAudio the samples, nothing is decoded). Same samples, rate and channels as the clip's source: the
       levels and the loops unchanged. Everywhere else (the editor, iOS) this returns null and the clip asset plays.
       File: "RSP1", channels (1 byte), rate (int32), frames (int32), then int16 samples, interleaved, little endian. */
    public static class WebPcm
    {
        public const string Folder = "WebPcm/";
        static readonly Dictionary<string, AudioClip> _cache = new Dictionary<string, AudioClip>();
        /* How many clips came from raw samples this run. */
        public static int Count => _cache.Count;
        static bool _warned;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset() => _cache.Clear();

        /* The clip of a Resources path (no extension) from its raw samples; null: none (not the web, or not chosen). */
        public static AudioClip Load(string path)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (_cache.TryGetValue(path, out var c)) return c;
            var t = Resources.Load<TextAsset>(Folder + path);
            if (t == null) return null;
            c = Clip(path.Substring(path.LastIndexOf('/') + 1), t.bytes);
            Resources.UnloadAsset(t);
            if (c != null) _cache[path] = c;
            return c;
#else
            return null;
#endif
        }

        public static AudioClip Clip(string name, byte[] b)
        {
            if (b == null || b.Length < 13 || b[0] != 'R' || b[1] != 'S' || b[2] != 'P' || b[3] != '1') return null;
            int channels = b[4], rate = System.BitConverter.ToInt32(b, 5), frames = System.BitConverter.ToInt32(b, 9);
            int n = frames * channels;
            if (channels < 1 || rate <= 0 || frames <= 0 || b.Length < 13 + n * 2) return null;
            var data = new float[n];
            for (int i = 0, o = 13; i < n; i++, o += 2) data[i] = (short)(b[o] | (b[o + 1] << 8)) / 32768f;
            // PM_105: a browser with no Web Audio at all (Playwright's WebKit on Windows) makes an empty clip and SetData
            // throws: that stopped the game's start. Then no clip: the loader goes on to the asset (silent there).
            AudioClip clip = null;
            try
            {
                clip = AudioClip.Create(name, frames, channels, rate, false);
                clip.SetData(data, 0);
                return clip;
            }
            catch (System.Exception e)
            {
                if (clip != null) Object.Destroy(clip);
                if (!_warned) { _warned = true; Debug.LogWarning($"[audio] ham örnek kurulamadı ({name}): {e.Message}"); }
                return null;
            }
        }
    }
}
