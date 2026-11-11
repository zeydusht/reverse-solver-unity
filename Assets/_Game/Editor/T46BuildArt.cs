using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace ReverseSolver.EditorTools
{
    // Copy only at build time. Never import or write source art metadata here.
    public sealed class T46BuildArt : IPreprocessBuildWithReport
    {
        public int callbackOrder => -100;
        public void OnPreprocessBuild(BuildReport report)
        {
            const string source = "Assets/Art/_yeni/T46";
            const string target = "Assets/_Game/Presentation/Resources/Art/yeni/T46";
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
                importer.SaveAndReimport();
            }
        }
    }
}
