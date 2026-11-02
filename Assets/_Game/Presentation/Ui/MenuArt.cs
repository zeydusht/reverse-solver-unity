using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ReverseSolver.Presentation
{
    // Separate from LevelImages: menu textures live for the whole visit, without KeepOnly eviction.
    public sealed class MenuArt : MonoBehaviour
    {
        static MenuArt _instance;
        public static MenuArt Instance => _instance != null ? _instance :
            _instance = new GameObject("MenuArt").AddComponent<MenuArt>();

        static readonly string[] Files = {
            "t20-scene-levels.jpg", "t20-scene-collection.jpg", "t20-scene-jiggles.jpg",
            "t20-scene-achievements.jpg", "t20-levels-path-repeat.png"
        };
        readonly Dictionary<string, Sprite> _ready = new Dictionary<string, Sprite>();
        readonly Dictionary<string, List<Action<Sprite>>> _waiting = new Dictionary<string, List<Action<Sprite>>>();
        readonly HashSet<string> _failed = new HashSet<string>(), _fetching = new HashSet<string>();
        bool _started;
        /* True while a level is being played: the background preload waits. */
        public static Func<bool> Busy;

        // Source RGB means at 30% brightness: a dark tone specific to each scene.
        public static Vector4 Background(string key) => Draw.Hex(key switch {
            "achievements" => "#381B15", "collection" => "#1C2525", "home" => "#1C1917",
            "jiggles" => "#302D26", "levels" => "#3B3125", _ => "#0B3A44"
        });

        public Sprite Get(string file, Action<Sprite> arrived)
        {
            if (_ready.TryGetValue(file, out var sprite)) return sprite;
            if (_failed.Contains(file)) return null;
            if (!_waiting.TryGetValue(file, out var callbacks))
                _waiting[file] = callbacks = new List<Action<Sprite>>();
            callbacks.Add(arrived);
            if (!_fetching.Contains(file)) StartCoroutine(Fetch(file));   // a tab opened before the preload got there
            return null;
        }

        public void Preload()
        {
            if (_started) return;
            _started = true;
            StartCoroutine(FetchAll());
        }

        IEnumerator FetchAll()
        {
            // PM_MESAJ_29 4.3: not during the first start (the CPU and the line belong to the menu and level 1's picture)
            yield return new WaitForSecondsRealtime(10f);
            foreach (var file in Files)
            {
                while (Busy != null && Busy()) yield return new WaitForSecondsRealtime(1f);   // no decoding mid-level
                if (!_fetching.Contains(file)) yield return Fetch(file);
            }
        }

        IEnumerator Fetch(string file)
        {
            _fetching.Add(file);
            {
                string path = $"{Application.streamingAssetsPath}/MenuArt/{file}";
                string url = path.Contains("://") ? path : new Uri(path).AbsoluteUri;
                using (var request = UnityWebRequest.Get(url))
                {
                    yield return request.SendWebRequest();
                    if (request.result != UnityWebRequest.Result.Success) { _failed.Add(file); _waiting.Remove(file); yield break; }
                    var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    if (!texture.LoadImage(request.downloadHandler.data, true))
                    {
                        Destroy(texture); _failed.Add(file); _waiting.Remove(file); yield break;
                    }
                    bool road = file == "t20-levels-path-repeat.png";
                    texture.name = file;
                    texture.filterMode = FilterMode.Bilinear;
                    texture.wrapMode = TextureWrapMode.Clamp;
                    if (road) texture.wrapModeV = TextureWrapMode.Repeat;
                    var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                        new Vector2(.5f, road ? 0 : .5f), road ? 3 : 1, 0, SpriteMeshType.FullRect);
                    _ready[file] = sprite;
                    if (_waiting.TryGetValue(file, out var callbacks))
                    {
                        _waiting.Remove(file);
                        foreach (var callback in callbacks) callback(sprite);
                    }
                }
            }
        }

        void OnDestroy()
        {
            foreach (var sprite in _ready.Values)
            {
                if (sprite == null) continue;
                Destroy(sprite.texture); Destroy(sprite);
            }
            _ready.Clear(); _waiting.Clear();
            if (_instance == this) _instance = null;
        }
    }
}
