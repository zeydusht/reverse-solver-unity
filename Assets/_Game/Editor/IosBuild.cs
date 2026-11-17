using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppLovinMax.Scripts.IntegrationManager.Editor;
using ReverseSolver.Core.Json;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace ReverseSolver.EditorTools
{
    /* iOS store build (PM_81: the soft launch, 1.0.0 (8)): player settings and an Xcode project in Builds/iOS, which is
       archived on a Mac (docs/MAC_YUKLEME.md; MAX's CocoaPods: `pod install`, then the .xcworkspace). Needs the iOS
       Build Support module.
         only iPhone, only portrait, 60 fps (NativeHost), the approved icon and launch screen, no Unity splash
         the content: the levels and settings from Resources/Live; paintings 1-20 copied into StreamingAssets/live for
                      this build only (the offline start); the rest from GitHub Pages' live/
         ads: Assets/Config/ads.json into Resources for this build only; MAX's terms and privacy flow with our privacy
              page and the ATT text (TR + EN); SKAdNetwork ids by MAX's own post-process; our PrivacyInfo.xcprivacy

         bash Tools/unity_batch.sh method ReverseSolver.EditorTools.IosBuild.Build -buildTarget iOS */
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

        /* PM_81 §3-4: AppLovin MAX from Assets/Config/ads.json: the SDK key, the terms and privacy flow (privacy page,
           terms), our ATT text. False when the keys are still placeholders (the build still works, without ads). */
        public static bool ApplyAds()
        {
            var o = JsonReader.Parse(File.ReadAllText(AdsSource));
            string R(string k) => o.TryGet(k, out var n) && n.Kind == JsonKind.String ? n.AsString : "";
            string sdk = R("sdk_key");
            bool keys = !sdk.StartsWith("BURAYA") && sdk.Length > 0 && !R("ios_rewarded").StartsWith("BURAYA") && !R("ios_interstitial").StartsWith("BURAYA");
            var s = AppLovinSettings.Instance;
            s.SdkKey = keys ? sdk : "";
            s.SaveAsync();
            var i = AppLovinInternalSettings.Instance;
            i.ConsentFlowEnabled = true;
            i.ConsentFlowPrivacyPolicyUrl = R("privacy_url");
            i.ConsentFlowTermsOfServiceUrl = R("terms_url");
            i.OverrideDefaultUserTrackingUsageDescriptions = true;
            i.UserTrackingUsageDescriptionEn = AttEn;
            i.Save();
            AssetDatabase.SaveAssets();
            Debug.Log($"[ios] MAX: {(keys ? "anahtarlar var" : "anahtarlar yer tutucu: reklam yok")}, şartlar ve gizlilik akışı açık");
            return keys;
        }

        public static void Build()
        {
            ApplySettings();
            ApplyAds();
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
            // the ad keys and paintings 1-20 (the offline start), for this build only
            File.Copy(AdsSource, AdsInBuild, true);
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
                    scenes = scenes, locationPathName = Out, target = BuildTarget.iOS, options = BuildOptions.None
                });
            }
            finally
            {
                AssetDatabase.DeleteAsset(music); AssetDatabase.DeleteAsset(Path.GetDirectoryName(music).Replace('\\', '/'));
                AssetDatabase.DeleteAsset(AdsInBuild);
                AssetDatabase.DeleteAsset("Assets/StreamingAssets/live");
                AssetDatabase.Refresh();
            }
            Debug.Log($"[ios] build {r.summary.result}: {Out}, {r.summary.totalErrors} hata, {r.summary.totalTime}, {copied.Count} gömülü tablo");
            if (r.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded) EditorApplication.Exit(1);
        }
    }

#if UNITY_IOS
    /* Info.plist and the Xcode project:
         ITSAppUsesNonExemptEncryption = false (only HTTPS), the display name
         NSUserTrackingUsageDescription in English (Info.plist) and Turkish (tr.lproj/InfoPlist.strings)
         PrivacyInfo.xcprivacy: the game's own data (device id, gameplay, crashes to our Supabase; no tracking);
         AppLovin's SDK brings its own manifest. MAX's post-process adds the SKAdNetwork ids. */
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
            var langs = plist.root.CreateArray("CFBundleLocalizations");
            langs.AddString("tr"); langs.AddString("en");
            plist.WriteToFile(plistPath);

            var projPath = UnityEditor.iOS.Xcode.PBXProject.GetPBXProjectPath(path);
            var proj = new UnityEditor.iOS.Xcode.PBXProject();
            proj.ReadFromFile(projPath);
            string main = proj.GetUnityMainTargetGuid();
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
