using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ReverseSolver.Core;
using ReverseSolver.Core.Json;
using ReverseSolver.Editing;
using UnityEditor;
using UnityEngine;

namespace ReverseSolver.EditorTools
{
    /* PM_81: the content of GitHub Pages' live/ folder and the app's own copy of it.
         Assets/Resources/Live/   levels.json, config.json, manifest.json   (inside every build: the offline start)
         Builds/live/             the same plus paintings/painting-NN.jpg   (Tools/yayin/live_yayinla.py publishes it)
       The paintings come from Assets/Art/Paintings_v2 (1-40 and Task51's 41-100 as they arrive). The manifest names
       the content version and every file's SHA-256; the game fetches only what changed.

         bash Tools/unity_batch.sh method ReverseSolver.EditorTools.LiveExport.Placeholder100   (PM_81: 40 + 60 copies)
         bash Tools/unity_batch.sh method ReverseSolver.EditorTools.LiveExport.FromSet -rsSet Assets/_Game/Levels/sl100.json */
    public static class LiveExport
    {
        public const string Res = "Assets/Resources/Live", Stage = "Builds/live", Paintings = "Assets/Art/Paintings_v2";

        /* PM_81 §6: p23's 40 (the p22 set as the game resolves it) and 41-100 as marked copies of them. */
        [MenuItem("Reverse Solver/Live: sl100 yer tutucu (40 + 60 kopya)")]
        public static void Placeholder100()
        {
            var redesign = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/_Game/Levels/redesign.json");
            var p22 = P21Onboarding.P22(LevelParser.Parse(redesign.text));
            var levels = new List<string>();
            for (int i = 0; i < 100; i++)
            {
                var src = p22.Levels[i % p22.Levels.Count];
                bool ph = i >= p22.Levels.Count;
                string id = ph ? $"PH{i + 1:000}" : src.Id;
                levels.Add(LevelJson(src, id, i + 1, $"painting-{i + 1:00}", ph ? "normal" : Role(i + 1), ph, null));
            }
            Write(levels, "sl100-ph");
        }

        /* A designed set file (PM_82's sl100): written as is, with its roles. */
        public static void FromSet()
        {
            string path = Arg("-rsSet") ?? "Assets/_Game/Levels/sl100.json";
            var set = LevelParser.Parse(File.ReadAllText(path));
            var levels = new List<string>();
            for (int i = 0; i < set.Levels.Count; i++)
            {
                var l = set.Levels[i];
                levels.Add(LevelJson(l, l.Id, i + 1, l.Image ?? $"painting-{i + 1:00}", l.Role ?? "normal", l.Placeholder, l.Lesson));
            }
            Write(levels, "sl100");
        }

        /* PM_70's roles on the p22 order (until PM_82 sets its own). */
        static string Role(int slot) =>
            P21Onboarding.IsObstacleLesson(slot) ? "lesson" : P21Onboarding.HardSlots22.Contains(slot) ? "hard"
            : P21Onboarding.BreatherSlots22.Contains(slot) ? "breather" : "normal";

        /* One level object: the level editor's writer, then the set's own fields (id, position, picture, role …). */
        public static string LevelJson(LevelData l, string id, int lv, string image, string role, bool placeholder, string lesson)
        {
            var w = new JsonWriter();
            LevelDraft.FromLevel(l).WriteJson(w);
            string json = w.ToString();
            json = Regex.Replace(json, "^\\{\"id\":\"[^\"]*\"", "{\"id\":" + Quote(id));
            json = Regex.Replace(json, "\"lv\":-?\\d+", "\"lv\":" + lv);
            json = Regex.Replace(json, ",\"image\":\"[^\"]*\"", "");
            var extra = new JsonWriter().BeginObject().Field("image", image).Field("role", role).Field("hard", l.Hard || role == "hard" || role == "veryhard")
                .Field("tutorial", l.Tutorial);
            if (placeholder) extra.Field("placeholder", true);
            if (lesson != null) extra.Field("lesson", lesson);
            string tail = extra.EndObject().ToString();
            return json.Substring(0, json.Length - 1) + "," + tail.Substring(1);
        }

        static string Quote(string s) => new JsonWriter().Value(s).ToString();

        public static void Write(List<string> levels, string versionHead)
        {
            string levelsJson = "{\"format\":1,\"levels\":[\n" + string.Join(",\n", levels) + "\n]}\n";
            LevelParser.Parse(levelsJson);                                       // it must read back
            string configJson = LiveConfig.Default.ToJson() + "\n";
            var files = new Dictionary<string, byte[]>
            {
                [ContentLoader.LevelsFile] = Encoding.UTF8.GetBytes(levelsJson),
                [ContentLoader.ConfigFile] = Encoding.UTF8.GetBytes(configJson),
            };
            int paintings = 0;
            for (int n = 1; n <= 100; n++)
            {
                string src = $"{Paintings}/painting-{n:00}.jpg";
                if (!File.Exists(src)) continue;
                files[$"paintings/painting-{n:00}.jpg"] = File.ReadAllBytes(src);
                paintings++;
            }
            string version = $"{versionHead}-{DateTime.UtcNow:yyyyMMdd-HHmm}-{ContentManifest.Sha256(files[ContentLoader.LevelsFile]).Substring(0, 6)}";
            var m = new ContentManifest { Version = version };
            foreach (var kv in files.OrderBy(k => k.Key, StringComparer.Ordinal))
                m.Files.Add(new ContentManifest.Entry { Path = kv.Key, Hash = ContentManifest.Sha256(kv.Value), Size = kv.Value.Length });
            string manifestJson = m.ToJson() + "\n";

            Directory.CreateDirectory(Res);
            File.WriteAllText($"{Res}/levels.json", levelsJson);
            File.WriteAllText($"{Res}/config.json", configJson);
            File.WriteAllText($"{Res}/manifest.json", manifestJson);
            if (Directory.Exists(Stage)) Directory.Delete(Stage, true);
            foreach (var kv in files)
            {
                string f = Path.Combine(Stage, kv.Key);
                Directory.CreateDirectory(Path.GetDirectoryName(f));
                File.WriteAllBytes(f, kv.Value);
            }
            File.WriteAllText(Path.Combine(Stage, ContentLoader.ManifestFile), manifestJson);
            AssetDatabase.Refresh();
            Debug.Log($"[live] export {version}: {levels.Count} levels, {paintings} paintings -> {Res}, {Stage}");
        }

        static string Arg(string name)
        {
            var a = Environment.GetCommandLineArgs();
            int i = Array.IndexOf(a, name);
            return i >= 0 && i + 1 < a.Length ? a[i + 1] : null;
        }
    }
}
