using UnityEditor;
using UnityEngine;

namespace ReverseSolver.Editor
{
    public sealed class CorridorVictoryImport : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith("Assets/_Game/Presentation/Resources/CorridorVictory/")) return;
            var t = (TextureImporter)assetImporter;
            t.textureType = TextureImporterType.Default;
            t.sRGBTexture = true;
            t.alphaIsTransparency = true;
            t.mipmapEnabled = false;
            t.npotScale = TextureImporterNPOTScale.None;
            t.maxTextureSize = 2048;
            t.wrapMode = TextureWrapMode.Clamp;
            t.filterMode = FilterMode.Bilinear;
            t.textureCompression = TextureImporterCompression.Compressed;
            if (assetPath.EndsWith("/Static.png")) {
                t.textureType = TextureImporterType.Sprite;
                t.spriteImportMode = SpriteImportMode.Single;
                t.spritePixelsPerUnit = 1;
                t.textureCompression = TextureImporterCompression.Uncompressed;
                var settings = new TextureImporterSettings();
                t.ReadTextureSettings(settings);
                settings.spriteMeshType = SpriteMeshType.FullRect;
                t.SetTextureSettings(settings);
            }
            if (assetPath.EndsWith("/Source.png")) {
                t.textureCompression = TextureImporterCompression.Uncompressed;
                t.alphaIsTransparency = false;
            }
            if (assetPath.EndsWith("/arches.png")) {
                // Packed source-validity channels are data, not visible artwork.
                t.sRGBTexture = false;
                t.textureCompression = TextureImporterCompression.Uncompressed;
                t.alphaIsTransparency = false;
            }
        }
    }
}
