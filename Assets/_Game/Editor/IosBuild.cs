using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace ReverseSolver.EditorTools
{
    /* iOS TestFlight build (0.5.0): player settings and an Xcode project in Builds/iOS, which Zeyd's friend archives on a
       Mac (docs/MAC_YUKLEME.md). Needs the iOS Build Support module.

         bash Tools/unity_batch.sh method ReverseSolver.EditorTools.IosBuild.Build -buildTarget iOS */
    public static class IosBuild
    {
        public const string BundleId = "com.zeydtekin.reversesolver", Version = "0.5.1", BuildNumber = "2", Out = "Builds/iOS";
        const string Icon = "Assets/Art/Icon/Exports/2-market-mavi-1024.png";   // the approved app icon

        public static void ApplySettings()
        {
            var t = NamedBuildTarget.iOS;
            PlayerSettings.SetApplicationIdentifier(t, BundleId);
            PlayerSettings.bundleVersion = Version;
            PlayerSettings.iOS.buildNumber = BuildNumber;
            PlayerSettings.productName = "Reverse Solver";
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.iOS.appleEnableAutomaticSigning = true;
            PlayerSettings.iOS.targetOSVersionString = "15.0";
            PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneOnly;
            PlayerSettings.iOS.requiresFullScreen = true;
            PlayerSettings.iOS.hideHomeButton = false;
            PlayerSettings.SetScriptingBackend(t, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetArchitecture(t, 1);                               // ARM64
            PlayerSettings.statusBarHidden = true;

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
            Debug.Log($"[ios] ayarlar: {BundleId} {Version} ({BuildNumber}), iOS 15+, dikey, otomatik imza");
        }

        public static void Build()
        {
            ApplySettings();
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
            UnityEditor.Build.Reporting.BuildReport r;
            try
            {
                r = BuildPipeline.BuildPlayer(new BuildPlayerOptions
                {
                    scenes = scenes, locationPathName = Out, target = BuildTarget.iOS, options = BuildOptions.None
                });
            }
            finally { AssetDatabase.DeleteAsset(music); AssetDatabase.DeleteAsset(Path.GetDirectoryName(music).Replace('\\', '/')); }
            Debug.Log($"[ios] build {r.summary.result}: {Out}, {r.summary.totalErrors} hata, {r.summary.totalTime}");
            if (r.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded) EditorApplication.Exit(1);
        }
    }

#if UNITY_IOS
    /* Info.plist: no non-exempt encryption (only HTTPS), so TestFlight does not stop at "Missing Compliance". */
    public static class IosPlist
    {
        [UnityEditor.Callbacks.PostProcessBuild]
        static void After(BuildTarget target, string path)
        {
            if (target != BuildTarget.iOS) return;
            string plistPath = Path.Combine(path, "Info.plist");
            var plist = new UnityEditor.iOS.Xcode.PlistDocument();
            plist.ReadFromFile(plistPath);
            plist.root.SetBoolean("ITSAppUsesNonExemptEncryption", false);
            plist.root.SetString("CFBundleDisplayName", "Reverse Solver");
            plist.WriteToFile(plistPath);
        }
    }
#endif
}
