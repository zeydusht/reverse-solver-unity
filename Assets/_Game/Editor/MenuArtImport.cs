using UnityEditor;

namespace ReverseSolver.EditorTools
{
    /* Approved menu art (PM_MESAJ_7): the corridor background is a sprite at 1 pixel per point (the
       game's world unit), at most 2048 px on the long side, compressed, no mipmaps. */
    public sealed class MenuArtImport : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            // Codex art for the v3 preview (Art.cs): @3x sprites, 3 px per point; frame and button 9-sliced
            if (assetPath.Contains("Presentation/Resources/Art/"))
            {
                var a = (TextureImporter)assetImporter;
                a.textureType = TextureImporterType.Sprite;
                a.spriteImportMode = SpriteImportMode.Single;
                a.spritePixelsPerUnit = 3;
                a.mipmapEnabled = false;
                a.maxTextureSize = 1024;
                a.textureCompression = TextureImporterCompression.Compressed;
                a.crunchedCompression = true;
                a.compressionQuality = 60;
                var st = new TextureImporterSettings();
                a.ReadTextureSettings(st);
                st.spriteMeshType = UnityEngine.SpriteMeshType.FullRect;    // needed for sliced drawing
                a.SetTextureSettings(st);
                if (assetPath.EndsWith("/frame.png")) a.spriteBorder = new UnityEngine.Vector4(54, 54, 54, 54);
                if (assetPath.EndsWith("/button.png")) a.spriteBorder = new UnityEngine.Vector4(126, 42, 126, 42);
                return;
            }
            if (!assetPath.EndsWith("Presentation/Resources/menu_bg.png")) return;
            var t = (TextureImporter)assetImporter;
            t.textureType = TextureImporterType.Sprite;
            t.spriteImportMode = SpriteImportMode.Single;
            t.spritePixelsPerUnit = 1;
            t.mipmapEnabled = false;
            t.maxTextureSize = 2048;
            t.textureCompression = TextureImporterCompression.Compressed;
            t.crunchedCompression = true;
            t.compressionQuality = 60;
        }
    }
}
