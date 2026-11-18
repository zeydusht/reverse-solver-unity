using UnityEngine;
#if (UNITY_IOS || UNITY_WEBGL) && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace ReverseSolver.Presentation
{
    /* PM_MESAJ_19 §6 / 45, PM_72: taps on game moments.
         iOS app   real haptics (UIImpactFeedbackGenerator / UINotificationFeedbackGenerator, Plugins/iOS/RSHaptics.mm)
         web       navigator.vibrate (Android Chrome; 8-35 ms). iPhone Safari has no web vibration: there only the
                   picture and the sound remain.
       Light on every exit and a lock breaking, medium on a bomb piece out, success on the last piece, a very short
       weak tap on a jam (its own switch, default on: &takilmatitresim=0/1 sets it), selection on drag start and
       buttons (iOS only). The settings' "Titreşim" switch turns all of it off. At most one tap per 50 ms. */
    public static class Haptics
    {
        public enum Kind { Light = 0, Heavy = 1, Success = 2, Select = 3, Medium = 4, Jam = 5, Error = 6 }

        /* PM_86 §4: every tap the game asks for, before the switches (tests and the map in docs/TITRESIM_HARITASI.md). */
        public static System.Action<Kind> Asked;

        /* PM_86 §4/§6: a button: the select tap and a soft click (GameRoot sets the click). */
        public static System.Action Click;
        public static void Button() { Play(Kind.Select); Click?.Invoke(); }

        /* A tap a little later (the chest's success .1 s after its heavy opening). */
        public static void PlayLater(Kind kind, float seconds)
        {
            var host = Object.FindAnyObjectByType<GameRoot>();
            if (host == null) { Play(kind); return; }
            host.StartCoroutine(Later(kind, seconds));
        }
        static System.Collections.IEnumerator Later(Kind kind, float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
            Play(kind);
        }

        public static bool On
        {
            get => PlayerPrefs.GetInt("rs_haptics", 1) == 1;
            set { PlayerPrefs.SetInt("rs_haptics", value ? 1 : 0); PlayerPrefs.Save(); }
        }

        public static bool JamOn
        {
            get => PlayerPrefs.GetInt("rs_haptics_jam", 1) == 1;
            set { PlayerPrefs.SetInt("rs_haptics_jam", value ? 1 : 0); PlayerPrefs.Save(); }
        }

        /* Web vibration length per kind, ms (0: none). Select is left out: Android would buzz on every button. */
        public static int WebMs(Kind kind) => kind switch
        {
            Kind.Light => 12, Kind.Medium => 22, Kind.Heavy => 35, Kind.Success => 30, Kind.Jam => 8, Kind.Error => 40, _ => 0
        };

        /* Whether a kind plays at all under the switches. */
        public static bool Allowed(Kind kind) => On && (kind != Kind.Jam || JamOn);

#if (UNITY_IOS || UNITY_WEBGL) && !UNITY_EDITOR
#if UNITY_IOS
        [DllImport("__Internal")] static extern void RS_Haptic(int kind);
#else
        [DllImport("__Internal")] static extern void RS_Vibrate(int ms);
#endif
        static float _last = -1f;
        public static void Play(Kind kind)
        {
            Asked?.Invoke(kind);
            if (!Allowed(kind)) return;
            float now = Time.unscaledTime;
            bool strong = kind == Kind.Heavy || kind == Kind.Success || kind == Kind.Error;   // PM_MESAJ_46: never swallowed
            if (!strong && _last >= 0f && now - _last < .05f) return;
            _last = now;
#if UNITY_IOS
            RS_Haptic((int)kind);
#else
            int ms = WebMs(kind);
            if (ms > 0) RS_Vibrate(ms);
#endif
        }
#else
        public static void Play(Kind kind) => Asked?.Invoke(kind);
#endif
    }
}
