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
            // Codex piece kit atlases (KitAtlas.cs): plain textures sampled by Piece.shader, 2592 px
            if (assetPath.Contains("Presentation/Resources/Art/kit-"))
            {
                var k = (TextureImporter)assetImporter;
                k.textureType = TextureImporterType.Default;
                k.alphaIsTransparency = true;
                k.mipmapEnabled = false;
                k.maxTextureSize = 2048;   // PM_MESAJ_14: no texture above 2048 px
                k.wrapMode = UnityEngine.TextureWrapMode.Clamp;
                k.textureCompression = TextureImporterCompression.Compressed;
                k.crunchedCompression = true;
                k.compressionQuality = 70;
                return;
            }
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
                if (assetPath.EndsWith("skin-body.png")) { a.wrapMode = UnityEngine.TextureWrapMode.Repeat; a.crunchedCompression = false; }
                if (assetPath.EndsWith("game-background.png")) a.maxTextureSize = 1024;   // 576x1024: compressible (1125 px is not a multiple of 4)
                if (assetPath.EndsWith("/frame.png")) a.spriteBorder = new UnityEngine.Vector4(54, 54, 54, 54);
                // Codex Task 5 borders (left, bottom, right, top px), written by Tools/art_choice.py from Codex's manifest
                var m = System.Text.RegularExpressions.Regex.Match(System.IO.File.ReadAllText("Assets/_Game/Editor/art_borders.json"),
                    "\"" + System.IO.Path.GetFileName(assetPath) + "\":" + @"\s*\[\s*(\d+),\s*(\d+),\s*(\d+),\s*(\d+)");
                if (m.Success)
                    a.spriteBorder = new UnityEngine.Vector4(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value),
                                                             int.Parse(m.Groups[3].Value), int.Parse(m.Groups[4].Value));
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
