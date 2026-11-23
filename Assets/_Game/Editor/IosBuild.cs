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

         bash Tools/unity_batch.sh method ReverseSolver.EditorTools.IosBuild.Build -buildTarget iOS       (the store)
         bash Tools/unity_batch.sh method ReverseSolver.EditorTools.IosBuild.BuildTest -buildTarget iOS   (test ads) */
    public static class IosBuild
    {
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
            // background music: the Vorbis clip only in this build (the web streams the MP3 from Pages)
            const string music = "Assets/_Game/Presentation/Resources/Music/music.ogg";
            Directory.CreateDirectory(Path.GetDirectoryName(music));
            AssetDatabase.CopyAsset("Assets/Audio/Music/music.ogg", music);
            var imp = (AudioImporter)AssetImporter.GetAtPath(music);
            var ios = imp.GetOverrideSampleSettings(BuildTargetGroup.iOS);
            ios.compressionFormat = AudioCompressionFormat.Vorbis; ios.quality = .5f; ios.loadType = AudioClipLoadType.Streaming;
            imp.SetOverrideSampleSettings(BuildTargetGroup.iOS, ios);
            imp.SaveAndReimport();
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
                AssetDatabase.DeleteAsset(music); AssetDatabase.DeleteAsset(Path.GetDirectoryName(music).Replace('\\', '/'));
                AssetDatabase.DeleteAsset(AdsInBuild);
                AssetDatabase.DeleteAsset("Assets/StreamingAssets/live");
                AssetDatabase.Refresh();
            }
            Debug.Log($"[ios] build {(store ? "mağaza" : "test")} {r.summary.result}: {Out}, {r.summary.totalErrors} hata, {r.summary.totalTime}, {copied.Count} gömülü tablo");
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
