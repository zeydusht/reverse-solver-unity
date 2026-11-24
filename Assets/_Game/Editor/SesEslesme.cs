using System.IO;
using System.Linq;
using ReverseSolver.Presentation;
using UnityEngine;

namespace ReverseSolver.EditorTools
{
    /* PM_104 §1: what the game plays with no flag (SesSecim.Collect: every loader with Zeyd's choices), as the files
       under Resources, for Tools/ses_eslesme.js to compare with the listening page's files (docs/art/pm104/ses_eslesme.md).
         bash Tools/unity_batch.sh method ReverseSolver.EditorTools.SesEslesme.Write */
    public static class SesEslesme
    {
        public const string Out = "docs/art/pm104/oyun_sesleri.json";
        const string ResourcesRoot = "Assets/_Game/Presentation/Resources/";

        public static void Write()
        {
            var used = SesSecim.Collect().OrderBy(x => x).ToList();
            var files = used.Select(u => new[] { ".wav", ".ogg", ".mp3", ".json" }.Select(e => ResourcesRoot + u + e).FirstOrDefault(File.Exists))
                            .Where(f => f != null).ToList();
            var choices = string.Join(",", SesSecim.Keys.Select(k => $"\"{k}\":\"{SesSecim.Of(k)}\""));
            File.WriteAllText(Out, "{\n \"secim\": {" + choices + "},\n \"files\": [\n  " + string.Join(",\n  ", files.Select(f => $"\"{f}\"")) + "\n ]\n}\n");
            Debug.Log($"[eslesme] {files.Count} dosya -> {Out}");
        }
    }
}
