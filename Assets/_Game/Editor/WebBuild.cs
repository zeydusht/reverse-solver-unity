using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ReverseSolver.EditorTools
{
    /* One-click web build into Builds/WebGL, ready to push to GitHub Pages.
       Pages serves files as-is without a Content-Encoding header, so the build
       ships compressed files with the JS decompression fallback rather than
       relying on the server. Those settings are applied here too, so a build
       can never go out with a config that will not load on Pages.

       Every build appends its output sizes to Builds/size-log.csv and writes
       Builds/<dir>-report.txt (what went into the wasm and the data file), so
       the effect of a size change is measured rather than guessed. */
    public static class WebBuild
    {
        public const string OutputDir = "Builds/WebGL";
        const string SizeLog = "Builds/size-log.csv";
        const string StrippedDir = "Library/Bee/artifacts/WebGL/ManagedStripped";

        [MenuItem("Reverse Solver/Build WebGL")]
        public static void Build() => Build("manual", OutputDir, WebGLCompressionFormat.Gzip);

        /* applySettings: false builds with the project settings as they are, so a
           size experiment can change one thing at a time. */
        public static bool Build(string label, string outputDir, WebGLCompressionFormat compression,
                                 bool applySettings = true)
        {
            if (applySettings) ApplySettings();
            PlayerSettings.WebGL.compressionFormat = compression;

            var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
            if (scenes.Length == 0)
            {
                Debug.LogError("WebBuild: no enabled scenes in Build Settings.");
                return false;
            }

            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outputDir,
                target = BuildTarget.WebGL,
                options = BuildOptions.DetailedBuildReport
            });

            // Ship setting stays gzip; a comparison build must not leave brotli behind.
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;

            var s = report.summary;
            if (s.result != BuildResult.Succeeded)
            {
                Debug.LogError($"WebBuild [{label}]: {s.result}, {s.totalErrors} error(s)");
                return false;
            }

            var files = OutputSizes(outputDir);
            LogSizes(label, compression, files, s.totalTime);
            WriteReport(outputDir, label, files, report);
            Debug.Log($"WebBuild [{label}]: {Path.GetFullPath(outputDir)} " +
                      $"({files.Values.Sum() / 1048576f:0.00} MB, {s.totalTime.TotalSeconds:0}s)");
            return true;
        }

        [MenuItem("Reverse Solver/Apply Web Settings")]
        public static void ApplySettings()
        {
            var webgl = UnityEditor.Build.NamedBuildTarget.WebGL;
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
            PlayerSettings.WebGL.decompressionFallback = true;
            PlayerSettings.WebGL.template = "PROJECT:ReverseSolver";
            PlayerSettings.WebGL.dataCaching = true;
            PlayerSettings.WebGL.nameFilesAsHashes = true;
            PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.ExplicitlyThrownExceptionsOnly;
            PlayerSettings.SetManagedStrippingLevel(webgl, ManagedStrippingLevel.High);
            PlayerSettings.SetIl2CppCodeGeneration(webgl, UnityEditor.Build.Il2CppCodeGeneration.OptimizeSize);
            // The wasm-side counterpart of OptimizeSize. Unity's default is
            // BuildTimes, which barely optimizes; DiskSizeLTO is the smallest
            // download at the cost of a slower build.
            EditorUserBuildSettings.SetPlatformSettings("WebGL", "CodeOptimization", "DiskSizeLTO");
            PlayerSettings.stripEngineCode = true;
            PlayerSettings.runInBackground = false;
            SyncPageBackground();
        }

        /* iOS home-screen mode leaves a strip under the canvas at the home
           indicator. Rather than fight the viewport, the page background is made
           the camera's clear colour so the strip cannot be seen. The template
           reads it as BACKGROUND_COLOR, so the camera in the first build scene
           stays the single source of that colour. */
        static void SyncPageBackground()
        {
            var first = EditorBuildSettings.scenes.FirstOrDefault(s => s.enabled);
            if (first == null || !File.Exists(first.path)) return;
            var m = System.Text.RegularExpressions.Regex.Match(File.ReadAllText(first.path),
                @"m_BackGroundColor: \{r: ([\d.eE-]+), g: ([\d.eE-]+), b: ([\d.eE-]+)");
            if (!m.Success)
            {
                Debug.LogWarning($"WebBuild: no camera background colour found in {first.path}");
                return;
            }
            float F(int i) => float.Parse(m.Groups[i].Value, System.Globalization.CultureInfo.InvariantCulture);
            PlayerSettings.SplashScreen.backgroundColor = new Color(F(1), F(2), F(3), 1f);
        }

        // ---- measurement -------------------------------------------------------

        // Only what the browser downloads: Build/* (loader, framework, wasm, data).
        static Dictionary<string, long> OutputSizes(string outputDir)
        {
            var result = new Dictionary<string, long>();
            foreach (var f in Directory.GetFiles(Path.Combine(outputDir, "Build")))
            {
                string name = Path.GetFileName(f);
                string kind = name.Contains(".loader.js") ? "loader"
                            : name.Contains(".framework.js") ? "framework"
                            : name.Contains(".wasm") ? "wasm"
                            : name.Contains(".data") ? "data" : name;
                result[kind] = new FileInfo(f).Length;
            }
            return result;
        }

        static void LogSizes(string label, WebGLCompressionFormat compression,
                             Dictionary<string, long> files, TimeSpan buildTime)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SizeLog));
            if (!File.Exists(SizeLog))
                File.WriteAllText(SizeLog, "time,label,compression,loader,framework,wasm,data,total,build_s\n");
            long Get(string k) => files.TryGetValue(k, out var v) ? v : 0;
            File.AppendAllText(SizeLog, string.Join(",",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm"), label, compression,
                Get("loader"), Get("framework"), Get("wasm"), Get("data"), files.Values.Sum(),
                (int)buildTime.TotalSeconds) + "\n");
        }

        static void WriteReport(string outputDir, string label, Dictionary<string, long> files, BuildReport report)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"WebBuild report [{label}] {DateTime.Now:yyyy-MM-dd HH:mm}");
            sb.AppendLine();
            sb.AppendLine("Downloaded files (compressed):");
            foreach (var kv in files.OrderByDescending(k => k.Value))
                sb.AppendLine($"  {kv.Value,10:N0}  {kv.Key}");
            sb.AppendLine($"  {files.Values.Sum(),10:N0}  total");

            sb.AppendLine();
            sb.AppendLine("Managed assemblies after stripping (uncompressed IL, a proxy for wasm share):");
            if (Directory.Exists(StrippedDir))
                foreach (var f in Directory.GetFiles(StrippedDir, "*.dll")
                                           .Select(p => new FileInfo(p)).OrderByDescending(f => f.Length))
                    sb.AppendLine($"  {f.Length,10:N0}  {f.Name}");

            sb.AppendLine();
            sb.AppendLine("Largest assets in the data file (uncompressed):");
            var assets = report.packedAssets
                .SelectMany(p => p.contents)
                .GroupBy(c => $"{c.sourceAssetPath} [{c.type.Name}]")
                .Select(g => (name: g.Key, size: g.Sum(c => (long)c.packedSize)))
                .OrderByDescending(a => a.size).ToList();
            if (assets.Count == 0) sb.AppendLine("  (data file reused from cache; no breakdown)");
            foreach (var a in assets.Take(40)) sb.AppendLine($"  {a.size,10:N0}  {a.name}");

            File.WriteAllText(outputDir.TrimEnd('/') + "-report.txt", sb.ToString());
        }
    }
}
