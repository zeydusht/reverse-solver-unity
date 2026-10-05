using System.Runtime.InteropServices;
using UnityEngine;

namespace ReverseSolver.Platform
{
    /* C# side of WebBridge.jslib. Outside a web player the calls fall back to
       the Unity log, so callers never need their own platform checks. */
    public static class WebBridge
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] static extern void RS_BootReport(string message);
#else
        static void RS_BootReport(string message) => Debug.Log("[boot] " + message);
#endif

        public static void BootReport(string message) => RS_BootReport(message);
    }
}
