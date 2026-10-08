using System.IO;
using UnityEngine;

namespace ReverseSolver.LevelEditor
{
    /* Turning a picture Zeyd picks into a level image (PRODUCT.md G1):
         - a source copy, longest side at most 2048 px, kept out of the build
           (Assets/_Game/Levels/ImageSources/<id>.<ext>) so the crop can be
           redone when the board changes shape;
         - the board image: centre-cropped to the board's w:h, about 150 px per
           cell, longest side at most 1024, JPG quality 80
           (Assets/StreamingAssets/LevelImages/<id>.jpg, fetched by the game
           when the level opens).
       The file Zeyd picked is only read. */
    public static class LevelImageImport
    {
        public const string ImageDir = "Assets/StreamingAssets/LevelImages";
        public const string SourceDir = "Assets/_Game/Levels/ImageSources";
        public const int SourceMax = 2048, BoardMax = 1024, PixelsPerCell = 150, Quality = 80;

        public static string ImagePath(string image) => $"{ImageDir}/{image}";

        public static Texture2D Load(string path)
        {
            var t = new Texture2D(2, 2, TextureFormat.RGBA32, true);
            if (!t.LoadImage(File.ReadAllBytes(path))) { Object.DestroyImmediate(t); return null; }
            t.filterMode = FilterMode.Trilinear;
            t.wrapMode = TextureWrapMode.Clamp;
            return t;
        }

        /* Writes the source copy for `id` from the picked file; returns its path. */
        public static string StoreSource(string picked, string id)
        {
            Directory.CreateDirectory(SourceDir);
            foreach (var old in Directory.GetFiles(SourceDir, id + ".*"))
                if (!old.EndsWith(".meta")) { File.Delete(old); if (File.Exists(old + ".meta")) File.Delete(old + ".meta"); }
            var src = Load(picked);
            if (src == null) return null;
            float k = Mathf.Min(1f, (float)SourceMax / Mathf.Max(src.width, src.height));
            var copy = Resample(src, new Rect(0, 0, 1, 1), Mathf.RoundToInt(src.width * k), Mathf.RoundToInt(src.height * k));
            bool png = Path.GetExtension(picked).ToLowerInvariant() == ".png";
            string path = $"{SourceDir}/{id}{(png ? ".png" : ".jpg")}";
            File.WriteAllBytes(path, png ? copy.EncodeToPNG() : copy.EncodeToJPG(92));
            Object.DestroyImmediate(src);
            Object.DestroyImmediate(copy);
            return path;
        }

        public static string SourceOf(string id)
        {
            if (id == null || !Directory.Exists(SourceDir)) return null;
            foreach (var f in Directory.GetFiles(SourceDir, id + ".*")) if (!f.EndsWith(".meta")) return f.Replace('\\', '/');
            return null;
        }

        /* Cuts the board image for a w x h board from the source copy; returns its size in bytes. */
        public static long WriteBoardImage(string source, string image, int width, int height)
        {
            var src = Load(source);
            if (src == null) return -1;
            float srcAspect = (float)src.width / src.height, aspect = (float)width / height;
            var uv = srcAspect > aspect
                ? new Rect((1 - aspect / srcAspect) / 2, 0, aspect / srcAspect, 1)      // wider: trim the sides
                : new Rect(0, (1 - srcAspect / aspect) / 2, 1, srcAspect / aspect);    // taller: trim top and bottom
            float w = width * PixelsPerCell, h = height * PixelsPerCell;
            float k = Mathf.Min(1f, BoardMax / Mathf.Max(w, h));
            var cut = Resample(src, uv, Mathf.RoundToInt(w * k), Mathf.RoundToInt(h * k));
            Directory.CreateDirectory(ImageDir);
            var bytes = cut.EncodeToJPG(Quality);
            File.WriteAllBytes(ImagePath(image), bytes);
            Object.DestroyImmediate(src);
            Object.DestroyImmediate(cut);
            return bytes.Length;
        }

        /* Scales the uv rectangle of src to w x h through the GPU (mipmapped, so large reductions stay smooth). */
        static Texture2D Resample(Texture2D src, Rect uv, int w, int h)
        {
            var rt = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGB32);
            Graphics.Blit(src, rt, uv.size, uv.position);
            var prev = RenderTexture.active;
            RenderTexture.active = rt;
            var t = new Texture2D(w, h, TextureFormat.RGB24, false);
            t.ReadPixels(new Rect(0, 0, w, h), 0, 0);
            t.Apply();
            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(rt);
            return t;
        }
    }
}
