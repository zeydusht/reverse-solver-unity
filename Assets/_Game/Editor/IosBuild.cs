using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReverseSolver.Presentation;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace ReverseSolver.EditorTools
{
    /* iOS store build (PM_81: the soft launch, 1.0.0 (8)): player settings and an Xcode project in Builds/iOS, which is
       archived on a Mac (docs/MAC_YUKLEME.md; the Google Mobile Ads pods: `pod install`, then the .xcworkspace). Needs
       the iOS Build Support module.
         only iPhone, only portrait, 60 fps (NativeHost), the approved icon and launch screen, no Unity splash
         the content: the levels and settings from Resources/Live; paintings 1-20 copied into StreamingAssets/live for
                      this build only (the offline start); the rest from GitHub Pages' live/
         ads (PM_101: Google AdMob; AppLovin MAX before): Assets/Config/ads.json into Resources for this build only, with
              "build": "store" (the real ids; the build stops if one is a placeholder or Google's test id) or "test"
              (BuildTest: a development build with Google's test ids); the App ID into the plugin's settings (it writes
              GADApplicationIdentifier and Google's SKAdNetworkItems into Info.plist); the ATT text (TR + EN); our
              PrivacyInfo.xcprivacy (the Google Mobile Ads SDK brings its own)
         sounds (PM_103): Zeyd's choices (Assets/Config/Resources/ses.json) in the build, offline: the lobby's and the
              level's loop and the win fanfare copied into Resources/Music for this build only (Vorbis; the web streams
              them from Pages); every sound candidate no loader picks with the defaults (SesSecim.Collect) moved out of
              Resources for the build (Assets/_IosDisi) and back after it; the build's audio listed in its log

         bash Tools/unity_batch.sh method ReverseSolver.EditorTools.IosBuild.Build -buildTarget iOS       (the store)
         bash Tools/unity_batch.sh method ReverseSolver.EditorTools.IosBuild.BuildTest -buildTarget iOS   (test ads) */
    public static class IosBuild
    {
        public const string MusicInBuild = "Assets/_Game/Presentation/Resources/Music", Stash = "Assets/_IosDisi";
        const string AudioRoot = "Assets/_Game/Presentation/Resources/Art/audio";
        /* The candidates' folders: every file in them is a sound some flag can pick. */
        public static readonly string[] CandidateFolders = { "pm95", "pm96", "pm98", "pm99", "pm100", "pm102", "pm104", "pm105", "pm106", "pm107" };

        /* PM_103: the chosen lobby and level loops (their own Ogg files: no lead-in, the exact loop) and the fanfare (its
           chosen tone) into Resources/Music as lobi, bolum, kazanma: Music reads them there. */
        public static void EmbedMusic()
        {
            AssetDatabase.DeleteAsset(MusicInBuild);
            Directory.CreateDirectory(MusicInBuild);
            foreach (var (src, name, loop) in new[]
            {
                (Music.SourceOf(Music.DefaultLobby), "lobi", true), (Music.SourceOf(Music.DefaultLevel), "bolum", true),
                (Music.FanfareSource(Music.FanfareClipKey), "kazanma", false),
            })
            {
                if (!File.Exists(src)) throw new BuildFailedException($"seçilen müzik yok: {src}");
                string dst = $"{MusicInBuild}/{name}{Path.GetExtension(src)}";
                AssetDatabase.CopyAsset(src, dst);
                var imp = (AudioImporter)AssetImporter.GetAtPath(dst);
                var ios = imp.GetOverrideSampleSettings(BuildTargetGroup.iOS);
                ios.compressionFormat = AudioCompressionFormat.Vorbis;
                // the loops streamed from the app's own files; the fanfare in memory (its beats follow its clock)
                ios.quality = loop ? .5f : .7f; ios.loadType = loop ? AudioClipLoadType.Streaming : AudioClipLoadType.DecompressOnLoad;
                ios.preloadAudioData = !loop;
                imp.SetOverrideSampleSettings(BuildTargetGroup.iOS, ios);
                imp.SaveAndReimport();
                Debug.Log($"[ios] müzik {name}: {src}");
            }
        }

        /* The candidates no loader picks with Zeyd's choices: moved out of Resources (into Stash, their .meta with them),
           so this build leaves them out. Returns how many. */
        public static int StashCandidates()
        {
            var used = SesSecim.Collect();
            var move = Unused(used);
            AssetDatabase.StartAssetEditing();
            try
            {
                foreach (var path in move)
                {
                    string dst = path.Replace(AudioRoot, Stash + "/audio");
                    Directory.CreateDirectory(Path.GetDirectoryName(dst));
                    File.Move(path, dst); File.Move(path + ".meta", dst + ".meta");
                }
            }
            finally { AssetDatabase.StopAssetEditing(); AssetDatabase.Refresh(); }
            Debug.Log($"[ios] sesler: {used.Count} seçili dosya yüklenecek, {move.Count} aday build dışında");
            return move.Count;
        }

        /* The candidate files (Assets paths) not in `used` (Resources paths, no extension). */
        public static List<string> Unused(ICollection<string> used)
        {
            var list = new List<string>();
            foreach (var folder in CandidateFolders)
            {
                string dir = $"{AudioRoot}/{folder}";
                if (!Directory.Exists(dir)) continue;
                foreach (var f in Directory.GetFiles(dir).Select(x => x.Replace('\\', '/')).Where(x => !x.EndsWith(".meta")).OrderBy(x => x))
                    if (!used.Contains($"Art/audio/{folder}/{Path.GetFileNameWithoutExtension(f)}")) list.Add(f);
            }
            return list;
        }

        /* Everything Stash holds back where it was. */
        public static void RestoreCandidates()
        {
            if (!Directory.Exists(Stash)) return;
            int n = 0;
            foreach (var f in Directory.GetFiles(Stash, "*", SearchOption.AllDirectories).Select(x => x.Replace('\\', '/')).Where(x => !x.EndsWith(".meta")))
            {
                string dst = f.Replace(Stash + "/audio", AudioRoot);
                Directory.CreateDirectory(Path.GetDirectoryName(dst));
                File.Move(f, dst);
                if (File.Exists(f + ".meta")) File.Move(f + ".meta", dst + ".meta");
                n++;
            }
            Directory.Delete(Stash, true);
            if (File.Exists(Stash + ".meta")) File.Delete(Stash + ".meta");
            AssetDatabase.Refresh();
            Debug.Log($"[ios] {n} aday yerine döndü");
        }

        /* The build's sound files (the report's packed assets; a clip is two entries, its header and its data): how many,
           their size, and that no unchosen candidate slipped in. */
        static void LogAudio(UnityEditor.Build.Reporting.BuildReport r, int stashed)
        {
            var used = SesSecim.Used;
            long bytes = 0;
            var files = new HashSet<string>();
            var strays = new HashSet<string>();
            var lines = new List<string>();
            foreach (var pack in r.packedAssets)
                foreach (var a in pack.contents)
                {
                    if (a.type != typeof(AudioClip)) continue;
                    files.Add(a.sourceAssetPath); bytes += (long)a.packedSize;
                    string folder = CandidateFolders.FirstOrDefault(f => a.sourceAssetPath.Contains($"/audio/{f}/"));
                    if (folder != null && !used.Contains($"Art/audio/{folder}/{Path.GetFileNameWithoutExtension(a.sourceAssetPath)}")) strays.Add(a.sourceAssetPath);
                    lines.Add($"{a.packedSize,9} {a.sourceAssetPath}");
                }
            File.WriteAllLines("Logs/ios_audio.txt", lines.OrderBy(x => x));
            Debug.Log($"[ios] build'deki sesler: {files.Count} dosya, {bytes / 1024} KB; seçilmemiş aday {strays.Count} (dışarıda bırakılan {stashed}); liste Logs/ios_audio.txt");
            if (strays.Count > 0) throw new BuildFailedException("seçilmemiş aday build'de: " + string.Join(", ", strays.Take(5)));
        }

        public const string BundleId = "com.zeydtekin.reversesolver", Version = "1.0.0", BuildNumber = "8", Out = "Builds/iOS";
        const string Icon = "Assets/Art/Icon/Exports/2-market-mavi-1024.png";   // the approved app icon
        public const string AttEn = "Used to show you more relevant ads and keep the game free.";
        public const string AttTr = "Sana daha uygun reklamlar göstermek ve oyunu ücretsiz tutmak için kullanılır.";
        const string AdsSource = "Assets/Config/ads.json", AdsInBuild = "Assets/Resources/ads.json";
        const string EmbeddedPaintings = "Assets/StreamingAssets/live/paintings";

        public static void ApplySettings()
        {
            var t = NamedBuildTarget.iOS;
            PlayerSettings.SetApplicationIdentifier(t, BundleId);
            PlayerSettings.bundleVersion = Version;
            PlayerSettings.iOS.buildNumber = BuildNumber;
            PlayerSettings.productName = "Reverse Solver";
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;
            PlayerSettings.iOS.appleEnableAutomaticSigning = true;
            PlayerSettings.iOS.targetOSVersionString = "15.0";
            PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneOnly;
            PlayerSettings.iOS.requiresFullScreen = true;
            PlayerSettings.iOS.hideHomeButton = false;
            PlayerSettings.SetScriptingBackend(t, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetArchitecture(t, 1);                               // ARM64
            PlayerSettings.statusBarHidden = true;
            // PM_81: no "Made with Unity" (Unity 6 lets every licence turn it off)
            PlayerSettings.SplashScreen.show = false;
            PlayerSettings.SplashScreen.showUnityLogo = false;

            // icon: the approved 1024 image for every size iOS asks for (Unity scales it)
            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(Icon);
            foreach (var kind in new[] { IconKind.Application, IconKind.Spotlight, IconKind.Settings, IconKind.Notification, IconKind.Store })
            {
                int n = PlayerSettings.GetIconSizes(t, kind).Length;
                if (n > 0) PlayerSettings.SetIcons(t, Enumerable.Repeat(icon, n).ToArray(), kind);
            }

            // launch screen: the icon centred on the game's dark teal (#0E2A2E, the page background)
            PlayerSettings.SplashScreen.backgroundColor = new Color32(0x0E, 0x2A, 0x2E, 0xFF);
            PlayerSettings.iOS.SetiPhoneLaunchScreenType(iOSLaunchScreenType.ImageAndBackgroundRelative);
            PlayerSettings.iOS.SetLaunchScreenImage(icon, iOSLaunchScreenImageType.iPhonePortraitImage);
            AssetDatabase.SaveAssets();
            Debug.Log($"[ios] ayarlar: {BundleId} {Version} ({BuildNumber}), iOS 15+, yalnız iPhone, dikey, açılış logosu kapalı");
        }

        /* PM_101: Google AdMob from Assets/Config/ads.json. store: the real ids; else Google's test ids. Returns why the
           store build cannot go on (a placeholder, or Google's test id), or null. The App ID goes into the plugin's
           settings asset (its post-process writes GADApplicationIdentifier and SKAdNetworkItems); json: the file for the
           build, with "build". */
        public static string ApplyAds(bool store, out string json)
        {
            string text = File.ReadAllText(AdsSource);
            json = null;
            if (store)
            {
                string problem = AdKeys.StoreProblem(text);
                if (problem != null) return problem;
            }
            int at = text.IndexOf('{');
            json = text.Substring(0, at + 1) + "\n  \"build\": \"" + (store ? "store" : "test") + "\"," + text.Substring(at + 1);
            var keys = AdKeys.Choose(json, !store, false);
            SetGmaSettings(keys.AppId, AttEn);
            Debug.Log($"[ios] AdMob: {(store ? "mağaza, gerçek kimlikler" : "test, Google'ın test kimlikleri")}, App ID {keys.AppId}");
            return null;
        }

        /* The Google Mobile Ads plugin's settings (an internal class: through its serialized fields). */
        static void SetGmaSettings(string iosAppId, string att)
        {
            var type = System.Type.GetType("GoogleMobileAds.Editor.GoogleMobileAdsSettings, GoogleMobileAds.Editor");
            var load = type?.GetMethod("LoadInstance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
            var settings = load?.Invoke(null, null) as ScriptableObject;
            if (settings == null) throw new BuildFailedException("Google Mobile Ads ayarları bulunamadı (eklenti yüklü mü?)");
            var so = new SerializedObject(settings);
            so.FindProperty("adMobIOSAppId").stringValue = iosAppId;
            so.FindProperty("userTrackingUsageDescription").stringValue = att;
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
        }

        /* The store build: the real AdMob ids (it stops on a placeholder or a test id). */
        public static void Build() => Build(true);

        /* A development build with Google's test ids (test ads only), for trying the ads on a phone. */
        public static void BuildTest() => Build(false);

        static void Build(bool store)
        {
            ApplySettings();
            string problem = ApplyAds(store, out string adsJson);
            if (problem != null)
            {
                Debug.LogError($"[ios] MAĞAZA BUILD'İ DURDU: {problem}. Gerçek AdMob kimliklerini Assets/Config/ads.json'a yaz (docs/ADMOB_KIMLIKLER.md).");
                if (Application.isBatchMode) EditorApplication.Exit(1);
                return;
            }
            var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
            if (Directory.Exists(Out)) Directory.Delete(Out, true);
            RestoreCandidates();   // a build that died half way
            WebAudioFormat.Remove();   // PM_104_EK: the web's raw samples never in the app
            // PM_103: the chosen music in this build (the web streams it from Pages), and only the chosen sounds
            EmbedMusic();
            var stashed = StashCandidates();
            // the ad ids (with "build") and paintings 1-20 (the offline start), for this build only
            File.WriteAllText(AdsInBuild, adsJson);
            Directory.CreateDirectory(EmbeddedPaintings);
            var copied = new List<string>();
            for (int n = 1; n <= 20; n++)
            {
                string src = $"{LiveExport.Paintings}/painting-{n:00}.jpg", dst = $"{EmbeddedPaintings}/painting-{n:00}.jpg";
                if (!File.Exists(src)) continue;
                File.Copy(src, dst, true); copied.Add(dst);
            }
            AssetDatabase.Refresh();
            UnityEditor.Build.Reporting.BuildReport r;
            try
            {
                r = BuildPipeline.BuildPlayer(new BuildPlayerOptions
                {
                    scenes = scenes, locationPathName = Out, target = BuildTarget.iOS, options = store ? BuildOptions.None : BuildOptions.Development
                });
            }
            finally
            {
                AssetDatabase.DeleteAsset(MusicInBuild);
                RestoreCandidates();
                AssetDatabase.DeleteAsset(AdsInBuild);
                AssetDatabase.DeleteAsset("Assets/StreamingAssets/live");
                AssetDatabase.Refresh();
            }
            Debug.Log($"[ios] build {(store ? "mağaza" : "test")} {r.summary.result}: {Out}, {r.summary.totalErrors} hata, {r.summary.totalTime}, {copied.Count} gömülü tablo");
            LogAudio(r, stashed);
            if (r.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded) EditorApplication.Exit(1);
        }
    }

#if UNITY_IOS
    /* Info.plist and the Xcode project:
         ITSAppUsesNonExemptEncryption = false (only HTTPS), the display name
         NSUserTrackingUsageDescription in English (Info.plist) and Turkish (tr.lproj/InfoPlist.strings)
         PrivacyInfo.xcprivacy: the game's own data (device id, gameplay, crashes to our Supabase; no tracking); the
         Google Mobile Ads SDK brings its own manifest. Its plugin's post-process writes GADApplicationIdentifier and
         Google's SKAdNetworkItems (checked here). AppTrackingTransparency and AdSupport for Plugins/iOS/RSAtt.mm. */
    public static class IosPlist
    {
        [UnityEditor.Callbacks.PostProcessBuild(900)]
        static void After(BuildTarget target, string path)
        {
            if (target != BuildTarget.iOS) return;
            string plistPath = Path.Combine(path, "Info.plist");
            var plist = new UnityEditor.iOS.Xcode.PlistDocument();
            plist.ReadFromFile(plistPath);
            plist.root.SetBoolean("ITSAppUsesNonExemptEncryption", false);
            plist.root.SetString("CFBundleDisplayName", "Reverse Solver");
            plist.root.SetString("NSUserTrackingUsageDescription", IosBuild.AttEn);
            plist.root.SetString("CFBundleDevelopmentRegion", "tr");
            // PM_101: the plugin's post-process (earlier) must have written the App ID and Google's SKAdNetwork list
            plist.root.values.TryGetValue("GADApplicationIdentifier", out var appId);
            plist.root.values.TryGetValue("SKAdNetworkItems", out var sk);
            if (appId == null || string.IsNullOrEmpty(appId.AsString())) throw new BuildFailedException("Info.plist: GADApplicationIdentifier yok");
            Debug.Log($"[ios] Info.plist: GADApplicationIdentifier {appId.AsString()}, SKAdNetworkItems {(sk is UnityEditor.iOS.Xcode.PlistElementArray arr ? arr.values.Count : 0)}");
            var langs = plist.root.CreateArray("CFBundleLocalizations");
            langs.AddString("tr"); langs.AddString("en");
            plist.WriteToFile(plistPath);

            var projPath = UnityEditor.iOS.Xcode.PBXProject.GetPBXProjectPath(path);
            var proj = new UnityEditor.iOS.Xcode.PBXProject();
            proj.ReadFromFile(projPath);
            string main = proj.GetUnityMainTargetGuid();
            string framework = proj.GetUnityFrameworkTargetGuid();
            proj.AddFrameworkToProject(framework, "AppTrackingTransparency.framework", true);
            proj.AddFrameworkToProject(framework, "AdSupport.framework", false);
            foreach (var (lang, text) in new[] { ("tr", IosBuild.AttTr), ("en", IosBuild.AttEn) })
            {
                string dir = Path.Combine(path, lang + ".lproj");
                Directory.CreateDirectory(dir);
                File.WriteAllText(Path.Combine(dir, "InfoPlist.strings"),
                    $"\"NSUserTrackingUsageDescription\" = \"{text}\";\n\"CFBundleDisplayName\" = \"Reverse Solver\";\n");
                string guid = proj.AddFile(lang + ".lproj/InfoPlist.strings", lang + ".lproj/InfoPlist.strings");
                proj.AddFileToBuild(main, guid);
            }
            File.Copy("Assets/_Game/Editor/PrivacyInfo.xcprivacy.txt", Path.Combine(path, "PrivacyInfo.xcprivacy"), true);
            proj.AddFileToBuild(main, proj.AddFile("PrivacyInfo.xcprivacy", "PrivacyInfo.xcprivacy"));
            proj.WriteToFile(projPath);
        }
    }
#endif
}
