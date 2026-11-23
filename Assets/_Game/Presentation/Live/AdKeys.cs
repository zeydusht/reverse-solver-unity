using ReverseSolver.Core.Json;

namespace ReverseSolver.Presentation
{
    /* PM_101: the ad ids (Google AdMob) from Assets/Config/ads.json, and which set a device uses.
         the store build ("build": "store", IosBuild.Build): the real ids; the build stops if one is still a placeholder
                     or Google's test id (StoreProblem)
         a test build ("build": "test", IosBuild.BuildTest, a development build) and the editor: Google's official
                     test ids (test ads only)
         a test device (Zeyd's iPhone; the hidden test menu, or its hashed id in "test_devices"): Google's test ad units
                     even in the store build, and its id in the request configuration: no real ad is ever shown to it
                     (a click on one's own real ad can close the AdMob account) */
    public static class AdKeys
    {
        // Google's official iOS test ids (https://developers.google.com/admob/ios/test-ads)
        public const string TestAppId = "ca-app-pub-3940256099942544~1458002511";
        public const string TestRewarded = "ca-app-pub-3940256099942544/1712485313";
        public const string TestInterstitial = "ca-app-pub-3940256099942544/4411468910";
        const string GooglePublisher = "3940256099942544";
        public const string TestDeviceKey = "rs_ad_test_device";

        public sealed class Set
        {
            public string AppId, Rewarded, Interstitial;
            public bool Test;                 // Google's test ids
            public string Build;              // "store", "test" or "" (the editor's raw file)
            public string[] TestDevices = new string[0];
        }

        public static bool Placeholder(string s) => string.IsNullOrEmpty(s) || s.StartsWith("BURAYA");
        public static bool IsTestId(string s) => !string.IsNullOrEmpty(s) && s.Contains(GooglePublisher);

        static string Str(JsonNode o, string k) => o.TryGet(k, out var n) && n.Kind == JsonKind.String ? n.AsString.Trim() : "";

        /* The ids this device uses; null when the file cannot be read. Test ids unless this is the store build with
           real ids and the device is not a test device. */
        public static Set Choose(string json, bool developmentBuild, bool testDevice)
        {
            JsonNode o;
            try { o = JsonReader.Parse(json ?? ""); }
            catch (JsonException) { return null; }
            var s = new Set { Build = Str(o, "build") };
            if (o.TryGet("test_devices", out var d) && d.Kind == JsonKind.Array)
            {
                var list = new System.Collections.Generic.List<string>();
                foreach (var e in d.Items) if (e.Kind == JsonKind.String && !Placeholder(e.AsString)) list.Add(e.AsString.Trim());
                s.TestDevices = list.ToArray();
            }
            string app = Str(o, "ios_app_id"), rew = Str(o, "ios_rewarded"), inter = Str(o, "ios_interstitial");
            bool real = s.Build == "store" && !developmentBuild && !testDevice && !Placeholder(app) && !Placeholder(rew) && !Placeholder(inter);
            s.Test = !real;
            if (real) { s.AppId = app; s.Rewarded = rew; s.Interstitial = inter; }
            else { s.AppId = TestAppId; s.Rewarded = TestRewarded; s.Interstitial = TestInterstitial; }
            return s;
        }

        /* Why the file cannot go into a store build (null: it can): a real id missing, or a test id in its place. */
        public static string StoreProblem(string json)
        {
            JsonNode o;
            try { o = JsonReader.Parse(json ?? ""); }
            catch (JsonException e) { return "ads.json okunamadı: " + e.Message; }
            foreach (var k in new[] { "ios_app_id", "ios_rewarded", "ios_interstitial" })
            {
                string v = Str(o, k);
                if (Placeholder(v)) return $"{k} yok (yer tutucu)";
                if (IsTestId(v)) return $"{k} Google'ın test kimliği: mağaza build'i gerçek kimlik ister";
                if (!v.StartsWith("ca-app-pub-")) return $"{k} bir AdMob kimliği değil: {v}";
            }
            if (!Str(o, "ios_app_id").Contains("~")) return "ios_app_id bir uygulama kimliği değil (~ yok)";
            return null;
        }

        /* This device is a test device (the hidden test menu; kept on the device). */
        public static bool TestDevice
        {
            get => Host.Current?.Store.Get(TestDeviceKey) == "1";
            set { if (Host.Current == null) return; Host.Current.Store.Set(TestDeviceKey, value ? "1" : "0"); Host.Current.Store.Save(); }
        }
    }
}
