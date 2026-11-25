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
        static int _decodeFrame = -1;

        public static LevelImages Create() => new GameObject("LevelImages").AddComponent<LevelImages>();

        /* PM_MESAJ_18: the SET40 v3 paintings live on GitHub Pages, never in the build; the iOS app keeps a copy. */
        public const string PaintingsUrl = "https://zeydusht.github.io/reverse-solver-unity/paintings-v2/";   // PM_MESAJ_39: Task25 paintings (root and p19 keep /paintings/)
        static bool Remote(string image) => image.StartsWith("painting-") || image.StartsWith(Thumb);
        /* PM_107 §D2: a painting's thumbnail for the collection's grid (210x280, ~16 KB: Tools/make_kucuk107.py) instead
           of the full 1024x1024 one; its tones are never needed (no board is cut from it). */
        public const string Thumb = "kucuk-";
        public static string ThumbOf(string painting) => Thumb + painting;
        static string CachePath(string image) => Path.Combine(Application.persistentDataPath, "paintings-v2", image + ".jpg");
        static bool DiskCache => Application.platform == RuntimePlatform.IPhonePlayer;

        public static string Url(string image)
        {
            if (image.StartsWith(Thumb)) return PaintingsUrl + "kucuk/" + image.Substring(Thumb.Length) + ".jpg";
            if (Remote(image)) return PaintingsUrl + image + ".jpg";
            string path = $"{Application.streamingAssetsPath}/{Folder}/{image}";
            return path.Contains("://") ? path : new Uri(path).AbsoluteUri;   // editor and desktop: a file path (spaces escaped)
        }

        /* The picture if it is already here, else null; `arrived` runs once it is (never on failure). */
        public Texture2D Get(string image, Action<Texture2D> arrived = null)
        {
            if (string.IsNullOrEmpty(image)) return null;
            if (image.StartsWith(Thumb) && (Live.Active || _failed.Contains(image))) image = image.Substring(Thumb.Length);   // the store build, or no thumbnail: the painting itself
            if (_failed.Contains(image)) return null;
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
                if (!keep.Contains(name)) { Tones.Remove(_ready[name]); Destroy(_ready[name]); _ready.Remove(name); }
        }

        /* PM_73 (ExitTrail): the picture's tone on a coarse grid, read once while the pixels are still on the CPU. */
        public const int ToneSize = 16;
        static readonly Dictionary<Texture, Color[]> Tones = new Dictionary<Texture, Color[]>();
        static Color[] ToneGrid(Texture2D tex)
        {
            var px = tex.GetPixels32();
            int w = tex.width, h = tex.height;
            var sum = new Vector4[ToneSize * ToneSize]; var n = new int[ToneSize * ToneSize];
            int step = Mathf.Max(1, Mathf.Min(w, h) / 96);
            for (int y = 0; y < h; y += step)
                for (int x = 0; x < w; x += step)
                {
                    var c = px[y * w + x];
                    int k = Mathf.Min(ToneSize - 1, y * ToneSize / h) * ToneSize + Mathf.Min(ToneSize - 1, x * ToneSize / w);
                    sum[k] += new Vector4(c.r, c.g, c.b, 0); n[k]++;
                }
            var grid = new Color[ToneSize * ToneSize];
            for (int k = 0; k < grid.Length; k++)
                grid[k] = n[k] == 0 ? Color.gray : new Color(sum[k].x / n[k] / 255f, sum[k].y / n[k] / 255f, sum[k].z / n[k] / 255f, 1);
            return grid;
        }
        /* uv 0..1 from the picture's top left. */
        /* PM_89 §1 (Zeyd: "at the end the picture grows short and long"): the paintings are square; a board of
           another shape shows the middle of it at its own proportions (the same scale across and down), never the
           whole picture squeezed. The frames in the menu and the corridor open at the board's proportions and show
           exactly this crop, so the picture is one and the same from the menu to the game. uv rect, y up. */
        public static Rect Crop(Texture tex, float aspect)
        {
            if (tex == null || aspect <= 0) return new Rect(0, 0, 1, 1);
            float ta = (float)tex.width / Mathf.Max(1, tex.height);
            return ta > aspect ? new Rect((1 - aspect / ta) / 2, 0, aspect / ta, 1) : new Rect(0, (1 - ta / aspect) / 2, 1, ta / aspect);
        }
        public static Vector4 CropST(Texture tex, float aspect) { var c = Crop(tex, aspect); return new Vector4(c.width, c.height, c.x, c.y); }

        public static bool Tone(Texture tex, Vector2 uv, out Color tone)
        {
            tone = Color.gray;
            if (tex == null || !Tones.TryGetValue(tex, out var grid)) return false;
            int x = Mathf.Clamp((int)(uv.x * ToneSize), 0, ToneSize - 1), y = Mathf.Clamp((int)((1 - uv.y) * ToneSize), 0, ToneSize - 1);
            tone = grid[y * ToneSize + x];
            return true;
        }

        IEnumerator Fetch(string image)
        {
            byte[] data = null;
            bool live = Live.Active;   // PM_81: the store build's paintings: the device's or the app's copy, else live/
            bool disk = !live && DiskCache && Remote(image);
            if (live) data = Live.LocalPainting(image);
            if (disk) try { if (File.Exists(CachePath(image))) data = File.ReadAllBytes(CachePath(image)); } catch (Exception) { data = null; }
            if (data == null)
                using (var req = UnityWebRequest.Get(live ? Live.PaintingUrl(image) : Url(image)))
                {
                    yield return req.SendWebRequest();
                    if (req.result == UnityWebRequest.Result.Success) data = req.downloadHandler.data;
                    else if (Host.Debug) Debug.Log($"[image] {image}: {req.error}");
                    if (data != null && live) Live.KeepPainting(image, data);
                    if (data != null && disk)
                        try { Directory.CreateDirectory(Path.GetDirectoryName(CachePath(image))); File.WriteAllBytes(CachePath(image), data); }
                        catch (Exception) { }
                }
            // PM_81 §6: a painting not published yet (61-100 may come later): one of 1-40, the same one each time
            if (data == null && !live && image.StartsWith("painting-") && int.TryParse(image.Substring(9), out int n) && n > 40)
                using (var alt = UnityWebRequest.Get(Url($"painting-{(n - 1) % 40 + 1:00}")))
                {
                    yield return alt.SendWebRequest();
                    if (alt.result == UnityWebRequest.Result.Success) data = alt.downloadHandler.data;
                }
            // PM_107 §D2: one picture decoded per frame (a row of cards arriving together froze the scroll)
            while (_decodeFrame == Time.frameCount) yield return null;
            _decodeFrame = Time.frameCount;
            var callbacks = _waiting[image];
            _waiting.Remove(image);
            bool thumb = image.StartsWith(Thumb);
            if (data == null && thumb)
            {
                // no thumbnail: the painting itself, for whoever waited
                _failed.Add(image);
                var full = image.Substring(Thumb.Length);
                foreach (var cb in callbacks) { var have = Get(full, cb); if (have != null) cb?.Invoke(have); }
                yield break;
            }
            var tex = data == null ? null : new Texture2D(2, 2, TextureFormat.RGB24, false);
            var sw = System.Diagnostics.Stopwatch.StartNew();   // PM_74 &perf=1
            if (tex == null || !tex.LoadImage(data, false))
            {
                if (tex != null) Destroy(tex);
                _failed.Add(image);
                if (Host.Debug && data != null) Debug.Log($"[image] {image}: not a picture");
                yield break;
            }
            double decode = sw.Elapsed.TotalMilliseconds; sw.Restart();
            if (!thumb) Tones[tex] = ToneGrid(tex);
            double tone = sw.Elapsed.TotalMilliseconds; sw.Restart();
            tex.Apply(false, true);                                             // then no CPU copy kept
            if (Host.Current != null && Host.Current.Query("perf") == "1")
                Debug.Log($"[perf] image {image} {tex.width}x{tex.height}: decode {decode:0} ms, tone {tone:0} ms, upload {sw.Elapsed.TotalMilliseconds:0} ms");
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.name = image;
            _ready[image] = tex;
            foreach (var cb in callbacks) cb(tex);
        }

        void OnDestroy()
        {
            foreach (var t in _ready.Values) if (t != null) { Tones.Remove(t); Destroy(t); }
            _ready.Clear();
        }
    }
}
