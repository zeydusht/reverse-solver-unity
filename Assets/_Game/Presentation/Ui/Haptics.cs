using UnityEngine;
#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace ReverseSolver.Presentation
{
    /* PM_MESAJ_19 §6: iOS taps (UIImpactFeedbackGenerator, Plugins/iOS/RSHaptics.mm): light on a jam, heavy on a
       bomb, the success notification on a win.
       PM_MESAJ_45: light on every removed piece, medium on a jam, a selection tick on drag start and button taps;
       at most one tap per 50 ms. Off in the settings; the web and the editor skip it silently. */
    public static class Haptics
    {
        public enum Kind { Light = 0, Heavy = 1, Success = 2, Select = 3, Medium = 4 }

        public static bool On
        {
            get => PlayerPrefs.GetInt("rs_haptics", 1) == 1;
            set { PlayerPrefs.SetInt("rs_haptics", value ? 1 : 0); PlayerPrefs.Save(); }
        }

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")] static extern void RS_Haptic(int kind);
        static float _last = -1f;
        public static void Play(Kind kind)
        {
            if (!On) return;
            float now = Time.unscaledTime;
            if (_last >= 0f && now - _last < .05f) return;
            _last = now;
            RS_Haptic((int)kind);
        }
#else
        public static void Play(Kind kind) { }
#endif
    }
}
