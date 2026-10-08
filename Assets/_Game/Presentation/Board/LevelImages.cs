using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ReverseSolver.Presentation
{
    /* Level pictures (PRODUCT.md G1) from StreamingAssets/LevelImages, fetched
       when a level opens so they never weigh on the first download. Until a
       picture arrives, and if it never does (offline, 404), the level plays
       with its colours; nothing is shown about the failure. The next level's
       picture is fetched in the background; pictures of other levels are let
       go to keep memory down on a phone. */
    public sealed class LevelImages : MonoBehaviour
    {
        public const string Folder = "LevelImages";

        readonly Dictionary<string, Texture2D> _ready = new Dictionary<string, Texture2D>();
        readonly Dictionary<string, List<Action<Texture2D>>> _waiting = new Dictionary<string, List<Action<Texture2D>>>();
        readonly HashSet<string> _failed = new HashSet<string>();

        public static LevelImages Create() => new GameObject("LevelImages").AddComponent<LevelImages>();

        public static string Url(string image)
        {
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
            using (var req = UnityWebRequestTexture.GetTexture(Url(image), true))
            {
                yield return req.SendWebRequest();
                var callbacks = _waiting[image];
                _waiting.Remove(image);
                if (req.result != UnityWebRequest.Result.Success)
                {
                    _failed.Add(image);
                    if (Host.Debug) Debug.Log($"[image] {image}: {req.error}");
                    yield break;
                }
                var tex = DownloadHandlerTexture.GetContent(req);
                tex.wrapMode = TextureWrapMode.Clamp;
                tex.name = image;
                _ready[image] = tex;
                foreach (var cb in callbacks) cb(tex);
            }
        }

        void OnDestroy()
        {
            foreach (var t in _ready.Values) if (t != null) Destroy(t);
            _ready.Clear();
        }
    }
}
