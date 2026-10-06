using System.Runtime.InteropServices;
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
        public const bool InBrowser = true;
#else
        static void RS_BootReport(string message) => Debug.Log("[boot] " + message);
        static string RS_Query(string name) => null;
        static float RS_PixelRatio() => 1f;
        static float RS_SafeInset(int side) => 0f;
        public const bool InBrowser = false;
#endif

        public static void BootReport(string message) => RS_BootReport(message);
        public static string Query(string name) => RS_Query(name);
        public static float PixelRatio => RS_PixelRatio();
        public static float SafeInset(int side) => RS_SafeInset(side);
    }

    /* The page as Presentation sees it, installed before the first scene loads. */
    public sealed class WebHost : IHost
    {
        public string Query(string name) => WebBridge.Query(name);
        public float PixelRatio => WebBridge.PixelRatio;
        public Vector4 SafeInsets => new Vector4(WebBridge.SafeInset(0), WebBridge.SafeInset(1),
                                                 WebBridge.SafeInset(2), WebBridge.SafeInset(3));
        public void ReportBoot(string message) => WebBridge.BootReport(message);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Install()
        {
            if (WebBridge.InBrowser) Host.Current = new WebHost();
        }
    }
}
