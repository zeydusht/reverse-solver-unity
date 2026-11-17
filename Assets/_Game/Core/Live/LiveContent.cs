using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using ReverseSolver.Core.Json;

namespace ReverseSolver.Core
{
    /* PM_81 §1: the store build reads only GitHub Pages' live/ folder: a frozen copy of the levels, the paintings and
       the settings. live/manifest.json names the content version and every file with its SHA-256; the game fetches
       only what changed, keeps it on the device, and never waits on the network to start:
         the device's copy (cache)  when every file it needs is there and matches its hash
         the copy inside the app    otherwise (embedded: all the levels, the settings, paintings 1-20)
       A new remote version is downloaded into the cache in the background and used from the next start; the manifest
       is written last, so a half-finished download is never taken for a version. */
    public sealed class ContentManifest
    {
        public sealed class Entry
        {
            public string Path, Hash;
            public long Size;
        }

        public string Version = "";
        public readonly List<Entry> Files = new List<Entry>();

        public Entry Find(string path) => Files.Find(f => f.Path == path);

        public static ContentManifest Parse(string json)
        {
            if (string.IsNullOrEmpty(json)) return null;
            try
            {
                var root = JsonReader.Parse(json);
                if (root.Kind != JsonKind.Object || !root.TryGet("version", out var v) || v.Kind != JsonKind.String) return null;
                var m = new ContentManifest { Version = v.AsString };
                if (root.TryGet("files", out var files) && files.Kind == JsonKind.Array)
                    foreach (var f in files.Items)
                        if (f.TryGet("path", out var p) && p.Kind == JsonKind.String && f.TryGet("sha256", out var h) && h.Kind == JsonKind.String)
                            m.Files.Add(new Entry { Path = p.AsString, Hash = h.AsString.ToLowerInvariant(),
                                Size = f.TryGet("size", out var s) && s.Kind == JsonKind.Number ? (long)s.AsNumber : 0 });
                return m;
            }
            catch (JsonException) { return null; }
        }

        public string ToJson()
        {
            var w = new JsonWriter().BeginObject().Field("format", 1).Field("version", Version);
            w.Key("files").BeginArray();
            foreach (var f in Files) w.BeginObject().Field("path", f.Path).Field("sha256", f.Hash).Field("size", (int)f.Size).EndObject();
            return w.EndArray().EndObject().ToString();
        }

        public static string Sha256(byte[] data)
        {
            using var sha = SHA256.Create();
            var h = sha.ComputeHash(data);
            var sb = new StringBuilder(h.Length * 2);
            foreach (byte b in h) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }

    /* Where the content's files live. Paths are relative to live/ ("levels.json", "paintings/painting-07.jpg"). */
    public interface IContentFiles
    {
        byte[] ReadEmbedded(string path);       // inside the app; null when not there
        byte[] ReadCache(string path);          // on the device; null when not there
        void WriteCache(string path, byte[] data);
    }

    public enum ContentOrigin { Embedded, Cache }

    public sealed class LocalContent
    {
        public string LevelsJson, ConfigJson, Version;
        public ContentOrigin Origin;
    }

    public sealed class ContentLoader
    {
        public const string ManifestFile = "manifest.json", LevelsFile = "levels.json", ConfigFile = "config.json";
        static readonly string[] Required = { LevelsFile, ConfigFile };
        readonly IContentFiles _files;

        public ContentLoader(IContentFiles files) { _files = files; }

        /* The manifest the device has (or null). */
        public ContentManifest CachedManifest() => ContentManifest.Parse(Text(_files.ReadCache(ManifestFile)));
        public ContentManifest EmbeddedManifest() => ContentManifest.Parse(Text(_files.ReadEmbedded(ManifestFile)));

        /* At start: the device's copy when complete and intact, otherwise the app's own. Never touches the network. */
        public LocalContent LoadLocal()
        {
            var cached = CachedManifest();
            if (cached != null)
            {
                var data = new Dictionary<string, byte[]>();
                bool ok = true;
                foreach (var path in Required)
                {
                    var e = cached.Find(path); var bytes = e == null ? null : _files.ReadCache(path);
                    if (bytes == null || ContentManifest.Sha256(bytes) != e.Hash) { ok = false; break; }
                    data[path] = bytes;
                }
                if (ok)
                    return new LocalContent { LevelsJson = Text(data[LevelsFile]), ConfigJson = Text(data[ConfigFile]), Version = cached.Version, Origin = ContentOrigin.Cache };
            }
            var em = EmbeddedManifest();
            return new LocalContent
            {
                LevelsJson = Text(_files.ReadEmbedded(LevelsFile)), ConfigJson = Text(_files.ReadEmbedded(ConfigFile)),
                Version = em?.Version ?? "embedded", Origin = ContentOrigin.Embedded,
            };
        }

        /* The files of a remote manifest that the device must fetch now: the levels and the settings whose hash differs
           from the device's copy (paintings come one by one when needed, see PaintingPath / NeedsPainting). */
        public List<ContentManifest.Entry> ToFetch(ContentManifest remote)
        {
            var list = new List<ContentManifest.Entry>();
            if (remote == null) return list;
            foreach (var path in Required)
            {
                var e = remote.Find(path);
                if (e == null) continue;
                var have = _files.ReadCache(path);
                if (have == null || ContentManifest.Sha256(have) != e.Hash) list.Add(e);
            }
            return list;
        }

        /* Stores what was fetched (each checked against the remote hash) and then the remote manifest, so the next start
           uses it. False (nothing committed) when a file is missing or does not match. */
        public bool Commit(ContentManifest remote, IDictionary<string, byte[]> fetched)
        {
            if (remote == null) return false;
            foreach (var path in Required)
                if (remote.Find(path) == null) return false;
            foreach (var e in ToFetch(remote))
                if (!fetched.TryGetValue(e.Path, out var b) || b == null || ContentManifest.Sha256(b) != e.Hash) return false;
            foreach (var kv in fetched)
            {
                var e = remote.Find(kv.Key);
                if (e != null && ContentManifest.Sha256(kv.Value) == e.Hash) _files.WriteCache(kv.Key, kv.Value);
            }
            _files.WriteCache(ManifestFile, Encoding.UTF8.GetBytes(remote.ToJson()));
            return true;
        }

        public static string PaintingPath(string image) => "paintings/" + image + ".jpg";

        /* A painting from the device or the app, when it is there and (if the manifest names it) intact; else null. */
        public byte[] LocalPainting(string image, ContentManifest manifest)
        {
            string path = PaintingPath(image);
            var e = manifest?.Find(path);
            var cached = _files.ReadCache(path);
            if (cached != null && (e == null || ContentManifest.Sha256(cached) == e.Hash)) return cached;
            return _files.ReadEmbedded(path);
        }

        /* Keeps a downloaded painting when it matches the manifest. */
        public bool StorePainting(string image, byte[] data, ContentManifest manifest)
        {
            string path = PaintingPath(image);
            var e = manifest?.Find(path);
            if (data == null || e != null && ContentManifest.Sha256(data) != e.Hash) return false;
            _files.WriteCache(path, data);
            return true;
        }

        /* PM_81 §6: a level whose painting the live folder does not have yet uses one of 1-40 (the same one each time). */
        public static string FallbackImage(string image, ContentManifest manifest)
        {
            if (manifest == null || manifest.Find(PaintingPath(image)) != null) return image;
            int n = Number(image);
            if (n <= 0) return image;
            string alt = $"painting-{(n - 1) % 40 + 1:00}";
            return manifest.Find(PaintingPath(alt)) != null ? alt : image;
        }

        static int Number(string image)
        {
            if (image == null || !image.StartsWith("painting-")) return 0;
            return int.TryParse(image.Substring(9), out int n) ? n : 0;
        }

        static string Text(byte[] b) => b == null ? null : Encoding.UTF8.GetString(b);
    }
}
