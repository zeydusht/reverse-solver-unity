using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReverseSolver.Presentation;
using UnityEditor;
using UnityEngine;

namespace ReverseSolver.EditorTools
{
    /* PM_104_EK: iPhone Safari's "EncodingError: Decoding failed". On the web Unity turns every clip into AAC (an .m4a
       in the data file, decoded by the browser's WebAudio) and its importer has no WebGL override (PCM, MP3 ... all
       refused: measured), so the sounds the game plays (SesSecim.Collect: Zeyd's choices with no flag) and the old
       ones Art.Sound loads (Art/audio/*.wav) also go into the web build as raw samples: Resources/WebPcm/<path>.bytes
       (WebPcm reads them; nothing for Safari to decode). Written from each clip's source file as its importer would
       (its rate; mono when it forces mono, the channels' mean), for the web build only: WebBuild writes them before
       the build and removes them after; the iOS build never sees them. The candidates (only with a flag) stay AAC, but for
       the request being tried (pm106). */
    public static class WebAudioFormat
    {
        public const string AudioRoot = "Assets/_Game/Presentation/Resources/Art/audio";
        const string ResourcesRoot = "Assets/_Game/Presentation/Resources/";
        public const string OutRoot = ResourcesRoot + "WebPcm";

        /* The clips (Assets paths) the web build carries as raw samples. */
        public static List<string> PcmFiles()
        {
            var used = SesSecim.Collect();
            var files = new List<string>();
            foreach (var f in Directory.GetFiles(AudioRoot, "*.*", SearchOption.AllDirectories).Select(x => x.Replace('\\', '/')))
            {
                if (f.EndsWith(".meta") || !(AssetImporter.GetAtPath(f) is AudioImporter)) continue;
                bool root = Path.GetDirectoryName(f).Replace('\\', '/') == AudioRoot;
                // PM_106: this request's candidates too (pm106: the exits q-u, the chain d-f), so Zeyd can try each one
                // with its flag on the iPhone (an AAC candidate may not decode there)
                bool trying = f.Contains("/audio/pm106/");
                if (root || trying || used.Contains(ResourcePath(f))) files.Add(f);
            }
            return files.OrderBy(x => x).ToList();
        }

        /* "Assets/.../Resources/Art/audio/pm95/tik-a3.wav" -> "Art/audio/pm95/tik-a3" */
        public static string ResourcePath(string assetPath)
        {
            string res = assetPath.Substring(ResourcesRoot.Length);
            return res.Substring(0, res.Length - Path.GetExtension(res).Length);
        }

        /* Writes every PcmFile's raw samples into OutRoot. Returns how many and their size. */
        public static (int files, long bytes) Write()
        {
            Remove();
            int n = 0; long size = 0;
            AssetDatabase.StartAssetEditing();
            try
            {
                foreach (var f in PcmFiles())
                {
                    var b = Encode(f);
                    if (b == null) { Debug.LogWarning($"[web] ham örnek yazılamadı (AAC kalır): {f}"); continue; }
                    string dst = $"{OutRoot}/{ResourcePath(f)}.bytes";
                    Directory.CreateDirectory(Path.GetDirectoryName(dst));
                    File.WriteAllBytes(dst, b);
                    n++; size += b.Length;
                }
            }
            finally { AssetDatabase.StopAssetEditing(); AssetDatabase.Refresh(); }
            Debug.Log($"[web] sesler: {n} dosya ham örnek (seçilenler ve eski sesler), {size / 1024} KB");
            return (n, size);
        }

        /* OutRoot gone (after the build, before the iOS build, after a build that died half way). */
        public static void Remove()
        {
            if (!Directory.Exists(OutRoot)) return;
            AssetDatabase.DeleteAsset(OutRoot);
            if (Directory.Exists(OutRoot)) Directory.Delete(OutRoot, true);
            if (File.Exists(OutRoot + ".meta")) File.Delete(OutRoot + ".meta");
            AssetDatabase.Refresh();
        }

        [MenuItem("Reverse Solver/Web Sesleri (ham örnek) yaz")]
        public static void WriteMenu() => Write();

        /* A clip's source (a WAV: 8/16/24/32-bit PCM or 32-bit float) as WebPcm's file, the way its importer reads it:
           its rate, its channels or their mean when it forces mono. null: not a WAV we read, or the importer resamples. */
        public static byte[] Encode(string assetPath)
        {
            var imp = AssetImporter.GetAtPath(assetPath) as AudioImporter;
            if (imp == null || !assetPath.EndsWith(".wav", StringComparison.OrdinalIgnoreCase)) return null;
            if (imp.defaultSampleSettings.sampleRateSetting != AudioSampleRateSetting.PreserveSampleRate) return null;
            if (!ReadWav(File.ReadAllBytes(assetPath), out int channels, out int rate, out float[] s)) return null;
            int frames = s.Length / channels, outCh = imp.forceToMono ? 1 : channels;
            var o = new byte[13 + frames * outCh * 2];
            o[0] = (byte)'R'; o[1] = (byte)'S'; o[2] = (byte)'P'; o[3] = (byte)'1'; o[4] = (byte)outCh;
            BitConverter.GetBytes(rate).CopyTo(o, 5); BitConverter.GetBytes(frames).CopyTo(o, 9);
            int w = 13;
            for (int i = 0; i < frames; i++)
                for (int c = 0; c < outCh; c++)
                {
                    float v;
                    if (outCh == channels) v = s[i * channels + c];
                    else { v = 0; for (int k = 0; k < channels; k++) v += s[i * channels + k]; v /= channels; }
                    int q = Mathf.Clamp(Mathf.RoundToInt(v * 32768f), -32768, 32767);
                    o[w++] = (byte)(q & 0xff); o[w++] = (byte)((q >> 8) & 0xff);
                }
            return o;
        }

        static bool ReadWav(byte[] b, out int channels, out int rate, out float[] samples)
        {
            channels = rate = 0; samples = null;
            if (b.Length < 12 || b[0] != 'R' || b[1] != 'I' || b[2] != 'F' || b[3] != 'F') return false;
            int fmt = 0, bits = 0, i = 12, dataAt = -1, dataLen = 0;
            while (i + 8 <= b.Length)
            {
                string id = System.Text.Encoding.ASCII.GetString(b, i, 4);
                int len = BitConverter.ToInt32(b, i + 4);
                if (id == "fmt ")
                {
                    fmt = BitConverter.ToUInt16(b, i + 8); channels = BitConverter.ToUInt16(b, i + 10);
                    rate = BitConverter.ToInt32(b, i + 12); bits = BitConverter.ToUInt16(b, i + 22);
                    if (fmt == 0xFFFE && len >= 40) fmt = BitConverter.ToUInt16(b, i + 32);   // WAVE_FORMAT_EXTENSIBLE: its sub format
                }
                else if (id == "data") { dataAt = i + 8; dataLen = Math.Min(len, b.Length - dataAt); }
                i += 8 + len + (len & 1);
            }
            if (dataAt < 0 || channels < 1 || rate <= 0) return false;
            int step = bits / 8;
            if (step < 1) return false;
            int n = dataLen / step;
            n -= n % channels;
            samples = new float[n];
            for (int k = 0, o = dataAt; k < n; k++, o += step)
            {
                if (fmt == 3 && bits == 32) samples[k] = BitConverter.ToSingle(b, o);
                else if (fmt != 1) return false;
                else if (bits == 16) samples[k] = (short)(b[o] | (b[o + 1] << 8)) / 32768f;
                else if (bits == 24) samples[k] = ((b[o] << 8 | b[o + 1] << 16 | b[o + 2] << 24) >> 8) / 8388608f;
                else if (bits == 32) samples[k] = BitConverter.ToInt32(b, o) / 2147483648f;
                else if (bits == 8) samples[k] = (b[o] - 128) / 128f;
                else return false;
            }
            return true;
        }
    }
}
