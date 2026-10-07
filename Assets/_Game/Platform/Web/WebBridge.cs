using System;
using System.Runtime.InteropServices;
using ReverseSolver.Core;
using ReverseSolver.Presentation;
using UnityEngine;

namespace ReverseSolver.Platform
{
    /* C# side of WebBridge.jslib. Outside a web player the calls fall back to
       harmless defaults, so callers never need their own platform checks. */
    public static class WebBridge
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] static extern void RS_BootReport(string message);
        [DllImport("__Internal")] static extern string RS_Query(string name);
        [DllImport("__Internal")] static extern float RS_PixelRatio();
        [DllImport("__Internal")] static extern float RS_SafeInset(int side);
        [DllImport("__Internal")] static extern void RS_AskName(string obj, string method);
        [DllImport("__Internal")] static extern void RS_WatchVisibility(string obj, string method);
        public const bool InBrowser = true;
#else
        static void RS_BootReport(string message) => Debug.Log("[boot] " + message);
        static string RS_Query(string name) => null;
        static float RS_PixelRatio() => 1f;
        static float RS_SafeInset(int side) => 0f;
        static void RS_AskName(string obj, string method) { }
        static void RS_WatchVisibility(string obj, string method) { }
        public const bool InBrowser = false;
#endif

        public static void BootReport(string message) => RS_BootReport(message);
        public static string Query(string name) => RS_Query(name);
        public static float PixelRatio => RS_PixelRatio();
        public static float SafeInset(int side) => RS_SafeInset(side);
        public static void AskName(string obj, string method) => RS_AskName(obj, method);
        public static void WatchVisibility(string obj, string method) => RS_WatchVisibility(obj, method);
    }

    /* The page as Presentation sees it, installed before the first scene loads. */
    public sealed class WebHost : IHost
    {
        readonly IKeyValueStore _store = new PlayerPrefsStore();
        Action<string> _nameDone;

        public string Query(string name) => WebBridge.Query(name);
        public float PixelRatio => WebBridge.PixelRatio;
        public Vector4 SafeInsets => new Vector4(WebBridge.SafeInset(0), WebBridge.SafeInset(1),
                                                 WebBridge.SafeInset(2), WebBridge.SafeInset(3));
        public void ReportBoot(string message) => WebBridge.BootReport(message);

        public IKeyValueStore Store => _store;
        public ITelemetryTransport Transport => WebTransport.Instance;
        public string SupabaseUrl => TelemetrySettings.SupabaseUrl;
        public string AnonKey => TelemetrySettings.AnonKey;
        public bool SendEnabled => TelemetrySettings.SendEnabled;

        public void AskName(Action<string> done)
        {
            _nameDone = done;
            WebBridge.AskName(Receiver.Name, nameof(Receiver.OnPlayerName));
        }

        public event Action<bool> VisibilityChanged;

        internal void NameAnswered(string name) { var d = _nameDone; _nameDone = null; d?.Invoke(name); }
        internal void Visibility(bool visible) => VisibilityChanged?.Invoke(visible);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Install()
        {
            if (!WebBridge.InBrowser) return;
            var host = new WebHost();
            Host.Current = host;
            Receiver.Create(host);
            WebBridge.WatchVisibility(Receiver.Name, nameof(Receiver.OnVisibility));
        }

        /* Target of the page's SendMessage calls. */
        sealed class Receiver : MonoBehaviour
        {
            public const string Name = "RSWebHost";
            WebHost _host;

            public static void Create(WebHost host)
            {
                var go = new GameObject(Name);
                DontDestroyOnLoad(go);
                go.AddComponent<Receiver>()._host = host;
            }

            public void OnPlayerName(string name) => _host.NameAnswered(name);
            public void OnVisibility(string state) => _host.Visibility(state != "hidden");
        }
    }
}
