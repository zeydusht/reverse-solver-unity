using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ReverseSolver.EditorTools
{
    /* One-click web build into Builds/WebGL, ready to push to GitHub Pages.
       Pages serves files as-is without a Content-Encoding header, so the build
       ships gzip with the JS decompression fallback rather than relying on the
       server. Those settings are applied here too, so a build can never go out
       with a config that will not load on Pages. */
    public static class WebBuild
    {
        public const string OutputDir = "Builds/WebGL";

        [MenuItem("Reverse Solver/Build WebGL")]
        public static void Build()
        {
            ApplySettings();

            var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
            if (scenes.Length == 0)
            {
                Debug.LogError("WebBuild: no enabled scenes in Build Settings.");
                return;
            }

            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = OutputDir,
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            });

            var s = report.summary;
            if (s.result == BuildResult.Succeeded)
                Debug.Log($"WebBuild: {Path.GetFullPath(OutputDir)} ({s.totalSize / (1024f * 1024f):0.0} MB, {s.totalTime.TotalSeconds:0}s)");
            else
                Debug.LogError($"WebBuild: {s.result}, {s.totalErrors} error(s)");
        }

        [MenuItem("Reverse Solver/Apply Web Settings")]
        public static void ApplySettings()
        {
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
            PlayerSettings.WebGL.decompressionFallback = true;
            PlayerSettings.WebGL.template = "PROJECT:ReverseSolver";
            PlayerSettings.WebGL.dataCaching = true;
            PlayerSettings.WebGL.nameFilesAsHashes = true;
            PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.ExplicitlyThrownExceptionsOnly;
            PlayerSettings.SetManagedStrippingLevel(UnityEditor.Build.NamedBuildTarget.WebGL, ManagedStrippingLevel.Medium);
            PlayerSettings.runInBackground = false;
        }
    }
}
