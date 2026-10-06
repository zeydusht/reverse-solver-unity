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
    }

    public static class Host
    {
        public static IHost Current { get; set; } = new EditorHost();

        public static bool Debug => Current.Query("debug") == "1";

        /* Editor and standalone: no page. Debug is on so the overlay shows while developing. */
        sealed class EditorHost : IHost
        {
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
        }
    }
}
