using System;
using System.Collections.Generic;
using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* What the page around the game provides. The web platform layer replaces
       the editor defaults at startup (Platform.WebHost); Presentation never
       depends on Platform directly. */
    public interface IHost
    {
        /* URL query parameter, or null. */
        string Query(string name);
        /* Device pixels per point the canvas renders at (capped at 2 by the template). */
        float PixelRatio { get; }
        /* Safe-area insets in points: x left, y top, z right, w bottom. */
        Vector4 SafeInsets { get; }
        /* Load-time panel (?debug=1). */
        void ReportBoot(string message);

        /* Saved data between visits. */
        IKeyValueStore Store { get; }

        /* Telemetry: transport (null = no network at all), endpoint and the
           build's send switch. */
        ITelemetryTransport Transport { get; }
        string SupabaseUrl { get; }
        string AnonKey { get; }
        bool SendEnabled { get; }

        /* Asks the player's name (web name gate); answer arrives through done. */
        void AskName(Action<string> done);
        void PlayIntro(string parts, string wake, string skip, string ending, bool sound, Action<string> events);

        /* Page hidden / shown (tab switch, app switch, closing). */
        event Action<bool> VisibilityChanged;
    }

    /* PM_72: the &test=1 link. Reads the device's save (name, settings) but keeps every write in memory, so test
       progress never reaches the normal save, and sends nothing (no telemetry rows from test play). */
    public sealed class TestHost : IHost
    {
        public readonly IHost Inner;
        public TestHost(IHost inner) { Inner = inner; Store = new OverlayStore(inner.Store); }
        public string Query(string name) => Inner.Query(name);
        public float PixelRatio => Inner.PixelRatio;
        public Vector4 SafeInsets => Inner.SafeInsets;
        public void ReportBoot(string message) => Inner.ReportBoot(message);
        public IKeyValueStore Store { get; }
        public ITelemetryTransport Transport => null;
        public string SupabaseUrl => null;
        public string AnonKey => null;
        public bool SendEnabled => false;
        public void AskName(Action<string> done) => Inner.AskName(done);
        public void PlayIntro(string parts, string wake, string skip, string ending, bool sound, Action<string> events) =>
            Inner.PlayIntro(parts, wake, skip, ending, sound, events);
        public event Action<bool> VisibilityChanged { add => Inner.VisibilityChanged += value; remove => Inner.VisibilityChanged -= value; }

        public sealed class OverlayStore : IKeyValueStore
        {
            readonly IKeyValueStore _base;
            readonly System.Collections.Generic.Dictionary<string, string> _own = new System.Collections.Generic.Dictionary<string, string>();
            public OverlayStore(IKeyValueStore inner) { _base = inner; }
            public string Get(string key) => _own.TryGetValue(key, out var v) ? v : _base.Get(key);
            public void Set(string key, string value) => _own[key] = value;
            public void Delete(string key) => _own[key] = null;
            public void Save() { }
        }
    }

    public static class Host
    {
        public static IHost Current { get; set; } = new EditorHost();

        public static bool Debug => Current.Query("debug") == "1";

        /* Editor and standalone: no page and no network. Debug is on so the
           overlay shows while developing. */
        sealed class EditorHost : IHost
        {
            readonly IKeyValueStore _store = new PlayerPrefsStore();
            public string Query(string name) => name == "debug" ? "1" : null;
            public float PixelRatio => 1f;
            public Vector4 SafeInsets
            {
                get
                {
                    var s = Screen.safeArea;
                    return new Vector4(s.xMin, Screen.height - s.yMax, Screen.width - s.xMax, s.yMin);
                }
            }
            public void ReportBoot(string message) => UnityEngine.Debug.Log("[boot] " + message);
            public IKeyValueStore Store => _store;
            public ITelemetryTransport Transport => null;
            public string SupabaseUrl => null;
            public string AnonKey => null;
            public bool SendEnabled => false;
            public void AskName(Action<string> done) => done("editor");
            public void PlayIntro(string parts, string wake, string skip, string ending, bool sound, Action<string> events) => events("done");
            public event Action<bool> VisibilityChanged { add { } remove { } }
        }
    }

    /* PlayerPrefs as the key-value store; on the web Unity keeps it in
       IndexedDB, and Save() flushes it there. */
    public sealed class PlayerPrefsStore : IKeyValueStore
    {
        public string Get(string key) => PlayerPrefs.HasKey(key) ? PlayerPrefs.GetString(key) : null;
        public void Set(string key, string value) => PlayerPrefs.SetString(key, value);
        public void Delete(string key) => PlayerPrefs.DeleteKey(key);
        public void Save() => PlayerPrefs.Save();
    }
}
