using UnityEditor;

namespace ReverseSolver.EditorTools
{
    /* Approved menu art (PM_MESAJ_7): the corridor background is a sprite at 1 pixel per point (the
       game's world unit), at most 2048 px on the long side, compressed, no mipmaps. */
    public sealed class MenuArtImport : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            if (assetPath.Contains("Presentation/Resources/Art/yeni/T46/"))
            {
                var t46 = (TextureImporter)assetImporter;
                t46.textureType = TextureImporterType.Sprite; t46.spriteImportMode = SpriteImportMode.Single;
                t46.spritePixelsPerUnit = 100; t46.alphaIsTransparency = true; t46.mipmapEnabled = false;
                t46.wrapMode = UnityEngine.TextureWrapMode.Clamp; t46.filterMode = UnityEngine.FilterMode.Bilinear;
                t46.maxTextureSize = 2048; t46.textureCompression = TextureImporterCompression.Uncompressed;
                var settings = new TextureImporterSettings(); t46.ReadTextureSettings(settings);
                settings.spriteMeshType = UnityEngine.SpriteMeshType.FullRect;
                settings.spriteAlignment = (int)UnityEngine.SpriteAlignment.Custom;
                settings.spritePivot = assetPath.Contains("/clock/extend/") || assetPath.Contains("/clock/hold/") || assetPath.Contains("/clock/retract/")
                    ? new UnityEngine.Vector2(128f / 352, .5f) : assetPath.Contains("/clock/ice_break/")
                    ? new UnityEngine.Vector2(.5f, 1 - 145f / 434) : new UnityEngine.Vector2(.5f, .5f);
                t46.SetTextureSettings(settings);
                t46.spriteBorder = assetPath.EndsWith("freeze_bar_bg.png") ? UnityEngine.Vector4.one * 14
                    : assetPath.EndsWith("freeze_bar_fill.png") ? UnityEngine.Vector4.one * 10 : UnityEngine.Vector4.zero;
                return;
            }
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
                k.compressionQuality = 50;   // PM_MESAJ_29 4.2: 2048 px is already the drawn size (cell ~137 px); lighter crunch
                return;
            }
            if (assetPath.Contains("Presentation/Resources/Art/"))
            {
                var a = (TextureImporter)assetImporter;
                a.textureType = TextureImporterType.Sprite;
                a.spriteImportMode = SpriteImportMode.Single;
                if (assetPath.Contains("/yeni/"))
                {
                    a.alphaIsTransparency = true;
                    a.wrapMode = UnityEngine.TextureWrapMode.Clamp;
                    a.spritePivot = new UnityEngine.Vector2(.5f, .5f);
                    var old = AssetImporter.GetAtPath("Assets/_Game/Presentation/Resources/Art/" + System.IO.Path.GetFileName(assetPath)) as TextureImporter;
                    if (old != null) a.spriteBorder = old.spriteBorder;
                }
                a.spritePixelsPerUnit = 3;
                a.mipmapEnabled = false;
                a.maxTextureSize = assetPath.Contains("/yeni/hud/") || assetPath.Contains("/yeni/devam_v2/") ? 2048 : 1024;
                if (assetPath.Contains("/t20-") || assetPath.Contains("/t21-") || assetPath.Contains("/t23-"))
                {
                    a.wrapMode = UnityEngine.TextureWrapMode.Clamp;
                    a.alphaIsTransparency = true;
                    a.maxTextureSize = 2048;
                    a.spritePixelsPerUnit = assetPath.Contains("/t20-scene-") ? 1 : 3;
                    a.filterMode = UnityEngine.FilterMode.Bilinear;
                    a.spritePivot = assetPath.Contains("-standing.png") || assetPath.EndsWith("/t20-levels-path-repeat.png")
                        ? new UnityEngine.Vector2(.5f, 0) : new UnityEngine.Vector2(.5f, .5f);
                    if (assetPath.EndsWith("/t20-levels-path-repeat.png"))
                        a.wrapModeV = UnityEngine.TextureWrapMode.Repeat;
                }
                a.textureCompression = TextureImporterCompression.Compressed;
                a.crunchedCompression = true;
                a.compressionQuality = 60;
                if (assetPath.Contains("/t23-"))
                {
                    // PM_MESAJ_39: the PNGs are padded to multiples of 4, so at their own size they stay block-compressible
                    const int limit = 2048;   // a smaller cap rescales to odd sizes and the texture ships uncompressed
                    a.maxTextureSize = limit;
                    a.SetPlatformTextureSettings(new TextureImporterPlatformSettings
                    {
                        name = "WebGL", overridden = true, maxTextureSize = limit,
                        format = TextureImporterFormat.DXT5Crunched,
                        textureCompression = TextureImporterCompression.Compressed,
                        crunchedCompression = true, compressionQuality = assetPath.EndsWith("-panel.png") ? 30 : 50   // panels: flat colour, 30 holds up
                    });
                }
                var st = new TextureImporterSettings();
                a.ReadTextureSettings(st);
                st.spriteMeshType = UnityEngine.SpriteMeshType.FullRect;    // needed for sliced drawing
                if (assetPath.Contains("/t20-") || assetPath.Contains("/t21-") || assetPath.Contains("/t23-")) st.spriteAlignment = (int)UnityEngine.SpriteAlignment.Custom;
                a.SetTextureSettings(st);
                if (assetPath.EndsWith("skin-body.png")) { a.wrapMode = UnityEngine.TextureWrapMode.Repeat; a.crunchedCompression = false; }
                if (assetPath.EndsWith("/popup-card.png")) a.maxTextureSize = 2048;   // 972x1200 stays block-aligned (1024 would rescale it to 830 px)
                if (assetPath.EndsWith("game-background.png")) a.maxTextureSize = 1024;   // 576x1024: compressible (1125 px is not a multiple of 4)
                if (assetPath.EndsWith("/frame.png")) a.spriteBorder = new UnityEngine.Vector4(54, 54, 54, 54);
                // Codex Task 5 borders (left, bottom, right, top px), written by Tools/art_choice.py from Codex's manifest
                var m = System.Text.RegularExpressions.Regex.Match(System.IO.File.ReadAllText("Assets/_Game/Editor/art_borders.json"),
                    "\"" + System.IO.Path.GetFileName(assetPath) + "\":" + @"\s*\[\s*(\d+),\s*(\d+),\s*(\d+),\s*(\d+)");
                if (m.Success)
                    a.spriteBorder = new UnityEngine.Vector4(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value),
                                                             int.Parse(m.Groups[3].Value), int.Parse(m.Groups[4].Value));
                if (assetPath.EndsWith("/hud-moves-pill.png") || assetPath.EndsWith("/hud-time-pill.png") || assetPath.EndsWith("/hud-time-pill-urgent.png"))
                    a.spritePixelsPerUnit = 2;   // PM_MESAJ_29: art_choice.py keeps them at 2x their drawn size
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
