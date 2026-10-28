using UnityEngine;
#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace ReverseSolver.Presentation
{
    /* PM_MESAJ_19 §6: iOS taps (UIImpactFeedbackGenerator, Plugins/iOS/RSHaptics.mm): light on a jam, heavy on a
       bomb, the success notification on a win. Off in the settings; the web and the editor skip it silently. */
    public static class Haptics
    {
        public enum Kind { Light = 0, Heavy = 1, Success = 2 }

        public static bool On
        {
            get => PlayerPrefs.GetInt("rs_haptics", 1) == 1;
            set { PlayerPrefs.SetInt("rs_haptics", value ? 1 : 0); PlayerPrefs.Save(); }
        }

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")] static extern void RS_Haptic(int kind);
        public static void Play(Kind kind) { if (On) RS_Haptic((int)kind); }
#else
        public static void Play(Kind kind) { }
#endif
    }
}
