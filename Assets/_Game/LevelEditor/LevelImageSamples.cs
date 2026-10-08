using System.IO;
using ReverseSolver.Editing;
using UnityEditor;
using UnityEngine;

namespace ReverseSolver.LevelEditor
{
    /* Puts the generated test pictures (Builds/test-images, drawn from maths:
       no copyright) on the first designs, for checking G1 before Zeyd brings
       real pictures. Works outside Play mode through the same import as the
       editor's "Görsel seç…" button. */
    public static class LevelImageSamples
    {
        public const string Folder = "Builds/test-images";
        static readonly string[] Pictures = { "test_gun_batimi.png", "test_halkalar.png", "test_yildiz.png" };

        [MenuItem("Reverse Solver/Test görsellerini D01–D03'e koy")]
        public static string Apply()
        {
            if (EditorApplication.isPlaying) return "Play modunda çalışmaz.";
            var store = DesignStore.Parse(File.Exists(LevelEditorSession.DesignsPath) ? File.ReadAllText(LevelEditorSession.DesignsPath) : "");
            var log = new System.Text.StringBuilder();
            for (int i = 0; i < Pictures.Length && i < store.Designs.Count; i++)
            {
                var d = store.Designs[i].Clone();
                string picked = Path.Combine(Folder, Pictures[i]);
                if (!File.Exists(picked)) { log.AppendLine($"{picked} yok"); continue; }
                string source = LevelImageImport.StoreSource(picked, d.Id);
                string image = d.Id + ".jpg";
                long bytes = LevelImageImport.WriteBoardImage(source, image, d.Width, d.Height);
                d.Image = image;
                store.Save(d);
                log.AppendLine($"{d.Id} {d.Width}x{d.Height}: {image} {bytes / 1024} KB (v{d.Version})");
            }
            File.WriteAllText(LevelEditorSession.DesignsPath, store.ToJson());
            AssetDatabase.Refresh();
            Debug.Log("[images] " + log);
            return log.ToString();
        }
    }
}
