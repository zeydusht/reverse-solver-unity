using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ReverseSolver.Core;
using UnityEngine;
using UnityEngine.Networking;

namespace ReverseSolver.Presentation
{
    /* PM_81 §1-2: the store build's content from GitHub Pages' live/ folder (Core.ContentLoader decides; this is the
       Unity side). The app's own copy: Resources/Live (manifest, levels, settings) and, on iOS, paintings 1-20 in
       StreamingAssets/live/paintings (IosBuild copies them in). The device's copy: persistentDataPath/live.
       On the web the folder is read only with &kaynak=live (the test of the store build's path); then nothing is kept
       between visits, the levels and settings are fetched before the game starts. */
    public static class Live
    {
        public const string BaseUrl = "https://zeydusht.github.io/reverse-solver-unity/live/";
        public static bool Active { get; private set; }        // the game reads live/ (iOS store build, or &kaynak=live)
        public static LiveConfig Config { get; private set; } = LiveConfig.Default;
        public static LocalContent Content { get; private set; }
        public static ContentManifest Manifest { get; private set; }   // the manifest of what is in use (paintings check against it)
        public static string Version => Content?.Version ?? "";
        public static string Report = "";                      // for the debug panel and the log
        static ContentLoader _loader;
        static UnityFiles _files;

        /* The settings without live/ (p23 default): the app's own copy of config.json, else the defaults. */
        public static void UseEmbeddedConfig()
        {
            var t = Resources.Load<TextAsset>("Live/config");
            Config = t != null ? LiveConfig.Parse(t.text, "embedded") : LiveConfig.Default;
        }

        /* Start: the device's or the app's copy at once; then (yield) at most `wait` seconds for a newer version from
           the network, used right away when it arrives in time, otherwise kept for the next start. */
        public static IEnumerator Open(MonoBehaviour host, bool keepOnDevice, float wait)
        {
            Active = true;
            _files = new UnityFiles(keepOnDevice);
            _loader = new ContentLoader(_files);
            Content = _loader.LoadLocal();
            Manifest = Content.Origin == ContentOrigin.Cache ? _loader.CachedManifest() : _loader.EmbeddedManifest();
            Config = LiveConfig.Parse(Content.ConfigJson, Content.Origin == ContentOrigin.Cache ? "cache" : "embedded");
            Report = $"live {Content.Origin} {Content.Version}";
            bool done = false;
            var sync = host.StartCoroutine(Sync(() => done = true));
            float until = Time.realtimeSinceStartup + wait;
            while (!done && Time.realtimeSinceStartup < until) yield return null;
            if (done)
            {
                var fresh = _loader.LoadLocal();
                if (fresh.Origin == ContentOrigin.Cache && fresh.Version != Content.Version || _files.MemoryOnly && _files.Has(ContentLoader.ManifestFile))
                {
                    Content = fresh;
                    Manifest = _loader.CachedManifest();
                    Config = LiveConfig.Parse(Content.ConfigJson, "remote");
                }
            }
            Report = $"live {Content.Origin} {Content.Version}" + (done ? "" : " (sync later)");
            SlEventRow.ContentVersion = Content.Version;
            Debug.Log("[live] " + Report);
        }

        /* Fetches the manifest and what changed, commits it to the device's copy. */
        static IEnumerator Sync(Action finished)
        {
            ContentManifest remote = null;
            using (var req = UnityWebRequest.Get(BaseUrl + ContentLoader.ManifestFile + "?t=" + DateTime.UtcNow.Ticks))
            {
                req.timeout = 8;
                yield return req.SendWebRequest();
                if (req.result == UnityWebRequest.Result.Success) remote = ContentManifest.Parse(req.downloadHandler.text);
            }
            if (remote == null) { finished(); yield break; }
            var fetched = new Dictionary<string, byte[]>();
            foreach (var e in _loader.ToFetch(remote))
                using (var req = UnityWebRequest.Get(BaseUrl + e.Path + "?v=" + e.Hash.Substring(0, 8)))
                {
                    req.timeout = 15;
                    yield return req.SendWebRequest();
                    if (req.result != UnityWebRequest.Result.Success) { finished(); yield break; }
                    fetched[e.Path] = req.downloadHandler.data;
                }
            bool ok = _loader.Commit(remote, fetched);
            if (ok && Manifest != null && Manifest.Version == remote.Version) Manifest = remote;
            Debug.Log($"[live] sync {remote.Version}: {fetched.Count} file(s), {(ok ? "kept" : "refused")}");
            finished();
        }

        // ---- paintings (LevelImages' hooks) ----

        /* The painting's name as live/ has it (61-100 not there yet: one of 1-40). */
        public static string MapImage(string image) => ContentLoader.FallbackImage(image, Manifest);
        public static byte[] LocalPainting(string image) => _loader?.LocalPainting(MapImage(image), Manifest);
        public static string PaintingUrl(string image)
        {
            string mapped = MapImage(image);
            var e = Manifest?.Find(ContentLoader.PaintingPath(mapped));
            return BaseUrl + ContentLoader.PaintingPath(mapped) + (e != null ? "?v=" + e.Hash.Substring(0, 8) : "");
        }
        public static void KeepPainting(string image, byte[] data) => _loader?.StorePainting(MapImage(image), data, Manifest);

        /* The app's copy (Resources/Live, StreamingAssets/live) and the device's (persistentDataPath/live). */
        sealed class UnityFiles : IContentFiles
        {
            readonly bool _disk;
            readonly Dictionary<string, byte[]> _memory = new Dictionary<string, byte[]>();
            public bool MemoryOnly => !_disk;
            public UnityFiles(bool disk) { _disk = disk; }
            public bool Has(string path) => _memory.ContainsKey(path);
            static string CacheRoot => Path.Combine(Application.persistentDataPath, "live");

            public byte[] ReadEmbedded(string path)
            {
                if (path.EndsWith(".json"))
                {
                    var t = Resources.Load<TextAsset>("Live/" + path.Substring(0, path.Length - 5));
                    return t != null ? t.bytes : null;
                }
                if (Application.platform == RuntimePlatform.WebGLPlayer) return null;   // a URL there, not a file
                try
                {
                    string f = Path.Combine(Application.streamingAssetsPath, "live", path);
                    return File.Exists(f) ? File.ReadAllBytes(f) : null;
                }
                catch (Exception) { return null; }
            }

            public byte[] ReadCache(string path)
            {
                if (!_disk) return _memory.TryGetValue(path, out var b) ? b : null;
                try
                {
                    string f = Path.Combine(CacheRoot, path);
                    return File.Exists(f) ? File.ReadAllBytes(f) : null;
                }
                catch (Exception) { return null; }
            }

            public void WriteCache(string path, byte[] data)
            {
                if (!_disk) { _memory[path] = data; return; }
                try
                {
                    string f = Path.Combine(CacheRoot, path);
                    Directory.CreateDirectory(Path.GetDirectoryName(f));
                    string tmp = f + ".tmp";
                    File.WriteAllBytes(tmp, data);
                    if (File.Exists(f)) File.Delete(f);
                    File.Move(tmp, f);
                }
                catch (Exception e) { Debug.Log("[live] cache write " + path + ": " + e.Message); }
            }
        }
    }
}
