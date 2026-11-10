using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ReverseSolver.Editor
{
    /// <summary>GPU smoke check for the actual Unity shader, independent of the browser preview.</summary>
    public static class CorridorVictoryRenderCheck
    {
        public static void Run()
        {
            try {
                AssetDatabase.Refresh();
                var shader = Resources.Load<Shader>("CorridorVictory/Projection");
                if (shader == null || !shader.isSupported) throw new Exception("Corridor shader unavailable on this GPU");
                var material = new Material(shader);
                material.SetTexture("_Source", Resources.Load<Texture2D>("CorridorVictory/Source"));
                foreach (string name in new[] { "Wall", "Floor", "Frame", "Columns", "Arches" }) {
                    var texture = Resources.Load<Texture2D>("CorridorVictory/" + name.ToLowerInvariant());
                    if (texture == null) throw new Exception("Missing corridor texture: " + name);
                    material.SetTexture("_" + name, texture);
                }
                var original = Resources.Load<Texture2D>("CorridorVictory/Static");
                var rt = new RenderTexture(512, 768, 0, RenderTextureFormat.ARGB32);
                rt.Create();
                var pixels = new Texture2D(512, 768, TextureFormat.RGBA32, false);
                string directory = "docs/art/corridor-victory/qa/unity";
                Directory.CreateDirectory(directory);
                byte[] start = null;
                foreach (float progress in new[] { 0f, .25f, .5f, .75f, 1f }) {
                    material.SetFloat("_Progress", progress);
                    material.SetVector("_CameraOffset", Presentation.CorridorVictory.CameraAt(progress));
                    Graphics.Blit(original, rt, material);
                    RenderTexture.active = rt;
                    pixels.ReadPixels(new Rect(0, 0, 512, 768), 0, 0);
                    pixels.Apply();
                    byte[] png = pixels.EncodeToPNG();
                    if (progress == 0) start = png;
                    if (progress == 1 && Convert.ToBase64String(start) != Convert.ToBase64String(png))
                        throw new Exception("Unity start/end pixels differ");
                    File.WriteAllBytes(Path.Combine(directory, "frame-" + progress.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + ".png"), png);
                }
                foreach (var message in ShaderUtil.GetShaderMessages(shader))
                    if (message.severity.ToString() == "Error") throw new Exception(message.message);
                File.WriteAllText(Path.Combine(directory, "result.json"), "{\"status\":\"passed\",\"endpointPixelsIdentical\":true,\"graphicsDevice\":\"" + SystemInfo.graphicsDeviceType + "\"}");
                RenderTexture.active = null;
                UnityEngine.Object.DestroyImmediate(pixels);
                rt.Release(); UnityEngine.Object.DestroyImmediate(rt);
                UnityEngine.Object.DestroyImmediate(material);
                Debug.Log("CORRIDOR_GPU_CHECK_PASSED");
                EditorApplication.Exit(0);
            } catch (Exception error) {
                Debug.LogException(error); EditorApplication.Exit(1);
            }
        }
    }
}
