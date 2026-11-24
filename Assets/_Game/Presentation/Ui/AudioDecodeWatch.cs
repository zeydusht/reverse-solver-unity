using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_104_EK: a sound the browser could not decode (iPhone Safari: "EncodingError: Decoding failed"; on WebGL the
       clip's load state turns Failed) stays silent and the game goes on: the page catches the error before Unity's
       alert (index.html), and the file is reported once: "[audio] decode fail <file>" in the log, audio_decode_fail
       (value: the file) in the soft launch's telemetry. Every clip SesSecim.Load or Art.Sound loads is watched until
       it has loaded or failed; on the web Unity's clip says Loaded even when the browser failed, so the page's guard
       names the failures (Loading: the file Unity is loading; RS_AudioFailsTake: the new failures) and they are
       reported the same way. */
    public sealed class AudioDecodeWatch : MonoBehaviour
    {
        static readonly Dictionary<string, AudioClip> _pending = new Dictionary<string, AudioClip>();
        static readonly HashSet<string> _seen = new HashSet<string>();
        static readonly List<string> _failed = new List<string>();
        static AudioDecodeWatch _self;
        /* The files that failed this run (Resources paths). */
        public static IReadOnlyList<string> Failed => _failed;
        public static int Pending => _pending.Count;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset() { _pending.Clear(); _seen.Clear(); _failed.Clear(); _self = null; }

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] static extern void RS_AudioName(string name);
        [DllImport("__Internal")] static extern string RS_AudioFailsTake();
#endif

        /* The file Unity is about to load (null: done), for the page's guard. */
        public static void Loading(string path)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            RS_AudioName(path ?? "");
            Ensure();
#endif
        }

        public static void Watch(string path, AudioClip clip)
        {
            if (clip == null || !Application.isPlaying || !_seen.Add(path)) return;
            _pending[path] = clip;
            Ensure();
        }

        static void Ensure()
        {
            if (_self == null && Application.isPlaying)
            {
                var go = new GameObject("AudioDecodeWatch") { hideFlags = HideFlags.HideInHierarchy };
                DontDestroyOnLoad(go);
                _self = go.AddComponent<AudioDecodeWatch>();
            }
        }

        float _next;
        readonly List<string> _done = new List<string>();

        void Update()
        {
            if (Time.unscaledTime < _next) return;
            _next = Time.unscaledTime + .5f;
#if UNITY_WEBGL && !UNITY_EDITOR
            var names = RS_AudioFailsTake();
            if (!string.IsNullOrEmpty(names))
                foreach (var n in names.Split('\n'))
                    if (n.Length > 0 && !_failed.Contains(n)) Report(n);
#endif
            if (_pending.Count == 0) return;
            _done.Clear();
            foreach (var kv in _pending)
            {
                var state = kv.Value != null ? kv.Value.loadState : AudioDataLoadState.Failed;
                if (state == AudioDataLoadState.Loading || state == AudioDataLoadState.Unloaded) continue;
                _done.Add(kv.Key);
                if (state == AudioDataLoadState.Failed) Report(kv.Key);
            }
            foreach (var k in _done) _pending.Remove(k);
        }

        static void Report(string path)
        {
            _failed.Add(path);
            Debug.LogWarning($"[audio] decode fail {path}");
            SlTelemetry.Emit("audio_decode_fail", value: path);
        }
    }
}
