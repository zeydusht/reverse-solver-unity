using UnityEditor;
using UnityEngine;

namespace ReverseSolver.EditorTools
{
    /* PM_76: the dive's views and the corridor's layers as small crunched textures (they came in uncompressed and
       capped at 1024: +7.5 MB to the download). Sets the importer through Unity (a hand-written .meta was rewritten)
       and prints what each texture really is.
         bash Tools/unity_batch.sh method ReverseSolver.EditorTools.Pm76TexInfo.Fix
         bash Tools/unity_batch.sh method ReverseSolver.EditorTools.Pm76TexInfo.Run */
    public static class Pm76TexInfo
    {
        const string Folder = "Assets/_Game/Presentation/Resources/Art/yeni/pm78";   // PM_78: the diorama

        public static void Fix()
        {
            foreach (var guid in AssetDatabase.FindAssets("t:Texture2D", new[] { Folder }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var imp = (TextureImporter)AssetImporter.GetAtPath(path);
                imp.textureType = TextureImporterType.Sprite;
                imp.spriteImportMode = SpriteImportMode.Single;
                imp.mipmapEnabled = false;
                imp.npotScale = TextureImporterNPOTScale.None;
                imp.alphaIsTransparency = true;
                imp.maxTextureSize = 2048;
                imp.textureCompression = TextureImporterCompression.Compressed;
                imp.crunchedCompression = true;
                imp.compressionQuality = 50;
                foreach (var platform in new[] { "WebGL", "iPhone", "Standalone" }) imp.ClearPlatformTextureSettings(platform);
                imp.SaveAndReimport();
            }
            Run();
        }

        public static void Run()
        {
            foreach (var guid in AssetDatabase.FindAssets("t:Texture2D", new[] { Folder }))
                Log(AssetDatabase.GUIDToAssetPath(guid));
            Log("Assets/_Game/Presentation/Resources/menu_bg.png");
        }

        static void Log(string path)
        {
            var t = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            long mem = t != null ? UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(t) : -1;
            Debug.Log($"[tex76] {path.Replace(Folder + "/", "")} {(t != null ? $"{t.width}x{t.height} {t.format}" : "missing")} mem {mem}");
        }
    }
}
