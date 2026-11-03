using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace ReverseSolver.Presentation
{
    /* Level pictures (PRODUCT.md G1) from StreamingAssets/LevelImages, fetched
       when a level opens so they never weigh on the first download. Until a
       picture arrives, and if it never does (offline, 404), the level plays
       with its colours; nothing is shown about the failure. The next level's
       picture is fetched in the background; pictures of other levels are let
       go to keep memory down on a phone.

       Plain UnityWebRequest + ImageConversion.LoadImage, not
       UnityWebRequestTexture: that module was taken out in the size work
       (Packages/manifest.json) and LoadImage's module is already in. */
    public sealed class LevelImages : MonoBehaviour
    {
        public const string Folder = "LevelImages";

        readonly Dictionary<string, Texture2D> _ready = new Dictionary<string, Texture2D>();
        readonly Dictionary<string, List<Action<Texture2D>>> _waiting = new Dictionary<string, List<Action<Texture2D>>>();
        readonly HashSet<string> _failed = new HashSet<string>();

        public static LevelImages Create() => new GameObject("LevelImages").AddComponent<LevelImages>();

        /* PM_MESAJ_18: the SET40 v3 paintings live on GitHub Pages, never in the build; the iOS app keeps a copy. */
        public const string PaintingsUrl = "https://zeydusht.github.io/reverse-solver-unity/paintings-v2/";   // PM_MESAJ_39: Task25 paintings (root and p19 keep /paintings/)
        static bool Remote(string image) => image.StartsWith("painting-");
        static string CachePath(string image) => Path.Combine(Application.persistentDataPath, "paintings-v2", image + ".jpg");
        static bool DiskCache => Application.platform == RuntimePlatform.IPhonePlayer;

        public static string Url(string image)
        {
            if (Remote(image)) return PaintingsUrl + image + ".jpg";
            string path = $"{Application.streamingAssetsPath}/{Folder}/{image}";
            return path.Contains("://") ? path : new Uri(path).AbsoluteUri;   // editor and desktop: a file path (spaces escaped)
        }

        /* The picture if it is already here, else null; `arrived` runs once it is (never on failure). */
        public Texture2D Get(string image, Action<Texture2D> arrived = null)
        {
            if (string.IsNullOrEmpty(image) || _failed.Contains(image)) return null;
            if (_ready.TryGetValue(image, out var tex)) return tex;
            if (_waiting.TryGetValue(image, out var list)) { if (arrived != null) list.Add(arrived); return null; }
            _waiting[image] = new List<Action<Texture2D>>();
            if (arrived != null) _waiting[image].Add(arrived);
            StartCoroutine(Fetch(image));
            return null;
        }

        /* Lets go of every picture not named (the current and the next level's). */
        public void KeepOnly(params string[] images)
        {
            var keep = new HashSet<string>();
            foreach (var i in images) if (!string.IsNullOrEmpty(i)) keep.Add(i);
            foreach (var name in new List<string>(_ready.Keys))
                if (!keep.Contains(name)) { Destroy(_ready[name]); _ready.Remove(name); }
        }

        IEnumerator Fetch(string image)
        {
            byte[] data = null;
            bool disk = DiskCache && Remote(image);
            if (disk) try { if (File.Exists(CachePath(image))) data = File.ReadAllBytes(CachePath(image)); } catch (Exception) { data = null; }
            if (data == null)
                using (var req = UnityWebRequest.Get(Url(image)))
                {
                    yield return req.SendWebRequest();
                    if (req.result == UnityWebRequest.Result.Success) data = req.downloadHandler.data;
                    else if (Host.Debug) Debug.Log($"[image] {image}: {req.error}");
                    if (data != null && disk)
                        try { Directory.CreateDirectory(Path.GetDirectoryName(CachePath(image))); File.WriteAllBytes(CachePath(image), data); }
                        catch (Exception) { }
                }
            var callbacks = _waiting[image];
            _waiting.Remove(image);
            var tex = data == null ? null : new Texture2D(2, 2, TextureFormat.RGB24, false);
            if (tex == null || !tex.LoadImage(data, true))                      // true: no CPU copy kept
            {
                if (tex != null) Destroy(tex);
                _failed.Add(image);
                if (Host.Debug && data != null) Debug.Log($"[image] {image}: not a picture");
                yield break;
            }
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.name = image;
            _ready[image] = tex;
            foreach (var cb in callbacks) cb(tex);
        }

        void OnDestroy()
        {
            foreach (var t in _ready.Values) if (t != null) Destroy(t);
            _ready.Clear();
        }
    }
}
