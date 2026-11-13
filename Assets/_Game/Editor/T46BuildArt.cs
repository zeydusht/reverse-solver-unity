using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace ReverseSolver.EditorTools
{
    // Copy only at build time. Never import or write source art metadata here.
    // T46 (clock, Jigo) and T47 (combo ladder, PM_64) go to Resources/Art/yeni/<set>.
    public sealed class T46BuildArt : IPreprocessBuildWithReport
    {
        public int callbackOrder => -100;
        public void OnPreprocessBuild(BuildReport report)
        {
            Copy("T46", 0);
            // T47 frames are drawn at 2x (bursts 640 px for ~320 pt); 384 px keeps them sharp enough and the build small.
            Copy("T47", 384);
        }

        static void Copy(string set, int maxSize)
        {
            string source = "Assets/Art/_yeni/" + set;
            string target = "Assets/_Game/Presentation/Resources/Art/yeni/" + set;
            if (!Directory.Exists(source)) return;
            foreach (string file in Directory.GetFiles(source, "*.png", SearchOption.AllDirectories))
            {
                string dest = target + file.Substring(source.Length);
                Directory.CreateDirectory(Path.GetDirectoryName(dest));
                File.Copy(file, dest, true);
                dest = dest.Replace('\\', '/');
                AssetDatabase.ImportAsset(dest, ImportAssetOptions.ForceUpdate);
                var importer = (TextureImporter)AssetImporter.GetAtPath(dest);
                var original = AssetImporter.GetAtPath(file.Replace('\\', '/')) as TextureImporter;
                if (original != null)
                {
                    var settings = new TextureImporterSettings(); original.ReadTextureSettings(settings);
                    importer.SetTextureSettings(settings);
                    importer.spritePixelsPerUnit = original.spritePixelsPerUnit;
                    importer.spriteBorder = original.spriteBorder;
                }
                // Resources.Load<Sprite> needs one sprite per file.
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.mipmapEnabled = false;
                importer.alphaIsTransparency = true;
                // The combo plate is drawn full screen width: keep it at 512 px.
                if (maxSize > 0) importer.maxTextureSize = file.Replace('\\', '/').Contains("/Badge/") ? 512 : maxSize;
                importer.SaveAndReimport();
            }
        }
    }
}
