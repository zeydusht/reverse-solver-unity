using System;
using System.Collections;
using ReverseSolver.Core;
using ReverseSolver.Presentation;
using UnityEngine;
using UnityEngine.Networking;

namespace ReverseSolver.Platform
{
    /* The iOS app's host (TestFlight 0.5.0): no page and no URL, so no debug and no ?painting; the SET40 v3 set is
       the default (SetFlags). Safe area in points (notch, home bar), PlayerPrefs for saves, telemetry with
       UnityWebRequest like the web, the player's name through the iOS keyboard, app pause as visibility. */
    public sealed class NativeHost : IHost
    {
        readonly IKeyValueStore _store = new PlayerPrefsStore();
        public string Query(string name) => null;

        /* Pixels per point: the device's UIScreen scale (2 or 3), read back from the dpi Unity reports. */
        public float PixelRatio => Mathf.Clamp(Mathf.Round(Screen.dpi / 160f), 1f, 3f);

        public Vector4 SafeInsets
        {
            get
            {
                var s = Screen.safeArea; float k = PixelRatio;
                return new Vector4(s.xMin, Screen.height - s.yMax, Screen.width - s.xMax, s.yMin) / k;
            }
        }

        public void ReportBoot(string message) => Debug.Log("[boot] " + message);
        public IKeyValueStore Store => _store;
        public ITelemetryTransport Transport => WebTransport.Instance;
        public string SupabaseUrl => TelemetrySettings.SupabaseUrl;
        public string AnonKey => TelemetrySettings.AnonKey;
        public bool SendEnabled => TelemetrySettings.SendEnabled;
        public event Action<bool> VisibilityChanged;

        public void AskName(Action<string> done) => Runner.Instance.AskName(done);
        public void PlayIntro(string parts, string wake, string skip, string ending, bool sound, Action<string> events) => events("done");
        internal void Visibility(bool visible) => VisibilityChanged?.Invoke(visible);

#if UNITY_IOS && !UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Install()
        {
            FrameRate();
            var host = new NativeHost();
            Host.Current = host;
            Runner.Create(host);
            RemoteLevels.Refresh();
        }
#endif

        /* PM_MESAJ_45: Unity's iOS default is 30 FPS; ask for 60 (no ProMotion 120: battery and heat). Web untouched. */
        static void FrameRate()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }

        /* Keyboard for the name, app pause/resume, coroutines for the remote levels. */
        internal sealed class Runner : MonoBehaviour
        {
            public static Runner Instance { get; private set; }
            NativeHost _host;
            TouchScreenKeyboard _kb;
            Action<string> _done;

            public static void Create(NativeHost host)
            {
                var go = new GameObject("RSNativeHost");
                DontDestroyOnLoad(go);
                Instance = go.AddComponent<Runner>();
                Instance._host = host;
            }

            public void AskName(Action<string> done)
            {
                _done = done;
                _kb = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false, Texts.NamePrompt, 12);
            }

            void Update()
            {
                if (_kb == null || _done == null) return;
                if (_kb.status == TouchScreenKeyboard.Status.Done || _kb.status == TouchScreenKeyboard.Status.Canceled ||
                    _kb.status == TouchScreenKeyboard.Status.LostFocus)
                {
                    string name = LaunchPlan.NormalizePlayerName(_kb.text);
                    if (name.Length == 0) { _kb = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false, Texts.NamePrompt, 12); return; }
                    var d = _done; _done = null; _kb = null; d(name);
                }
            }

            void OnApplicationPause(bool paused)
            {
                if (!paused) FrameRate();
                _host?.Visibility(!paused);
            }
        }
    }

    /* PM: "level'ları uzaktan yükle". The game starts at once with the levels inside the app (or the copy saved from
       an earlier launch); meanwhile the published list is fetched from GitHub Pages and, if it parses as a full set,
       saved for the next launch. No network: the bundled set, nothing waits. */
    public static class RemoteLevels
    {
        public const string Url = "https://zeydusht.github.io/reverse-solver-unity/levels/redesign.json";
        public const string Key = "rs_remote_redesign";

        public static void Refresh()
        {
            if (NativeHost.Runner.Instance != null) NativeHost.Runner.Instance.StartCoroutine(Fetch());
        }

        static IEnumerator Fetch()
        {
            using var req = UnityWebRequest.Get(Url);
            req.timeout = 8;
            yield return req.SendWebRequest();
            if (req.result != UnityWebRequest.Result.Success) yield break;
            string json = req.downloadHandler.text;
            try
            {
                var set = LevelParser.Parse(json);
                if (set.Levels.Count < 40) yield break;
            }
            catch { yield break; }
            PlayerPrefs.SetString(Key, json);
            PlayerPrefs.Save();
        }
    }
}
