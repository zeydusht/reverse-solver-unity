using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_103: Zeyd's final sound choices (24 Nov 2026), in one place: Assets/Config/Resources/ses.json. Every sound's
       default comes from here (Music, Sfx95, UiSounds, BoosterSounds102, Gecis100); the flags (&lobi=, &sescikis=,
       &sesmakas= ...) still override them, for tests.
       Every sound file those read from Resources goes through Load, so what the game plays is known (Used): the files
       loaded when the game starts, and the iOS build leaves every other candidate out (IosBuild). */
    public static class SesSecim
    {
        public const string Asset = "Assets/Config/Resources/ses.json";
        /* The keys of ses.json, Zeyd's table. */
        public static readonly string[] Keys =
            { "lobi", "bolum", "kazanma", "kazanmaton", "cikis", "carpma", "makas", "degnek", "cekic", "saat", "koridor", "dalis", "cizgi", "geri", "sekme", "dugme",
              "zincir", "zincirger", "kilitdokun", "kilitac", "patlama", "duvar" };   // PM_105: "tik" gone (every button: the tok)

        static Dictionary<string, string> _map;
        static Dictionary<string, string> Map
        {
            get
            {
                if (_map != null) return _map;
                _map = new Dictionary<string, string>();
                var text = Resources.Load<TextAsset>("ses");
                if (text == null) { Debug.LogError("[ses] ses.json yok (Assets/Config/Resources)"); return _map; }
                foreach (Match m in Regex.Matches(text.text, "\"(\\w+)\"\\s*:\\s*\"([^\"]*)\"")) _map[m.Groups[1].Value] = m.Groups[2].Value;
                return _map;
            }
        }

        /* The choice for a key ("marimba", "n" ...); null: none in the file. */
        public static string Of(string key) => Map.TryGetValue(key, out var v) && !string.IsNullOrEmpty(v) ? v : null;
        /* Its letter; `fallback` only when the file has none. */
        public static char Letter(string key, char fallback = 'a') { var v = Of(key); return v != null ? v[0] : fallback; }

        /* PM_107: the mastered copies' folder. */
        public const string Mastered = "Art/audio/pm107/";
        static readonly HashSet<string> _used = new HashSet<string>();
        static readonly HashSet<string> _replaced = new HashSet<string>();
        /* PM_107: the original files a mastered copy (pm107) played in place of. */
        public static IReadOnlyCollection<string> Replaced => _replaced;
        /* What the game chose: the files it loaded, the originals of the mastered copies among them. */
        public static HashSet<string> Chosen() { var c = new HashSet<string>(_used); c.UnionWith(_replaced); return c; }
        /* The Resources paths (no extension) of every sound file loaded so far. */
        public static IReadOnlyCollection<string> Used => _used;

        /* Resources.Load for the sound loaders: remembers what was found. */
        public static T Load<T>(string path) where T : Object
        {
            // PM_107 §C: the mastered copy of a chosen sound (Tools/make_miks107.py: its level in the mix, its highs and its
            // attack softened) plays in its place; &miks=0 the original
            if (typeof(T) == typeof(AudioClip) && Mix107.On && !path.StartsWith(Mastered))
            {
                var m = Load<T>(Mastered + path.Substring(path.LastIndexOf('/') + 1));
                if (m != null)
                {
#if UNITY_EDITOR
                    // the original that is really there (a loader asks pm102 first, then pm100 ...: the newest folder holding
                    // the name, as Tools/make_miks107.py took it)
                    var original = OriginalOf(path);
                    if (original != null) _replaced.Add(original);
#endif
                    return m;
                }
            }
            // PM_104_EK: on the web the chosen sounds come from their raw samples (WebPcm: Safari decodes nothing)
            var a = typeof(T) == typeof(AudioClip) ? WebPcm.Load(path) as T : null;
            if (a != null) { _used.Add(path); return a; }
            bool clip = typeof(T) == typeof(AudioClip);
            if (clip) AudioDecodeWatch.Loading(path);
            a = Resources.Load<T>(path);
            if (clip) AudioDecodeWatch.Loading(null);
            if (a != null) _used.Add(path);
            if (a is AudioClip c) AudioDecodeWatch.Watch(path, c);   // PM_104_EK: a file the browser cannot decode is reported
            return a;
        }

#if UNITY_EDITOR
        static string OriginalOf(string path)
        {
            const string Root = "Assets/_Game/Presentation/Resources/";
            if (!string.IsNullOrEmpty(UnityEditor.AssetDatabase.AssetPathToGUID(Root + path + ".wav", UnityEditor.AssetPathToGUIDOptions.OnlyExistingAssets))) return path;
            string name = path.Substring(path.LastIndexOf('/') + 1), best = null; int bestN = -1;
            foreach (var g in UnityEditor.AssetDatabase.FindAssets(name, new[] { Root + "Art/audio" }))
            {
                string a = UnityEditor.AssetDatabase.GUIDToAssetPath(g);
                if (System.IO.Path.GetFileNameWithoutExtension(a) != name || a.Contains("/pm107/")) continue;
                var m = Regex.Match(a, "/pm(\\d+)/");
                int n = m.Success ? int.Parse(m.Groups[1].Value) : 0;
                if (n > bestN) { bestN = n; best = a.Substring(Root.Length, a.Length - Root.Length - System.IO.Path.GetExtension(a).Length); }
            }
            return best;
        }
#endif

        /* What the game plays with no flags: every loader run with the defaults (the editor, the iOS build). */
        public static HashSet<string> Collect()
        {
            _used.Clear(); _replaced.Clear(); _map = null;
            UiSounds.Clear();
            BeatMap.Clear();
            Sfx95.Load();
            UiSounds.Preload();
            Gecis100.Load();
            BoosterSounds102.Load();
            Sfx105.Load();
            BeatMap.Has(Music.FanfareKey);
            return new HashSet<string>(_used);
        }
    }
}
