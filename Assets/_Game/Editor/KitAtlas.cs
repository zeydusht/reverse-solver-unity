using System.IO;
using UnityEngine;

namespace ReverseSolver.EditorTools
{
    /* Codex piece kit (Assets/Art/Pieces/Kit81, piece_TOP-RIGHT-BOTTOM-LEFT, O/F/I) packed into three 9x9 atlases
       for Piece.shader: Resources/Art/kit-mask.png, kit-overlay.png, kit-flat.png. Tile order matches the shader's
       kitUV: index = a(U)*27 + a(R)*9 + a(D)*3 + a(L), O=0 F=1 I=2, row 0 at the top. Each 640 px canvas is
       area-sampled down to Tile px (the cell keeps its centred 60 % share).

         bash Tools/unity_batch.sh method ReverseSolver.EditorTools.KitAtlas.Build */
    public static class KitAtlas
    {
        const int Tile = 288;
        const string Kit = "Assets/Art/Pieces/Kit81/", Out = "Assets/_Game/Presentation/Resources/Art/";
        static readonly string[] Kind = { "O", "F", "I" };

        public static void Build()
        {
            foreach (var layer in new[] { "mask", "overlay", "flat" })
            {
                var atlas = new Texture2D(Tile * 9, Tile * 9, TextureFormat.RGBA32, false);
                atlas.SetPixels32(new Color32[Tile * 9 * Tile * 9]);
                for (int idx = 0; idx < 81; idx++)
                {
                    string name = $"piece_{Kind[idx / 27]}-{Kind[idx / 9 % 3]}-{Kind[idx / 3 % 3]}-{Kind[idx % 3]}.png";
                    var src = new Texture2D(2, 2);
                    src.LoadImage(File.ReadAllBytes(Kit + layer + "/" + name));
                    var px = src.GetPixels();
                    int sw = src.width, col = idx % 9, row = idx / 9;
                    var dst = new Color[Tile * Tile];
                    float k = sw / (float)Tile;
                    for (int y = 0; y < Tile; y++)
                        for (int x = 0; x < Tile; x++)
                        {
                            // 3x3 samples per destination pixel, premultiplied so transparent edges do not darken
                            Color acc = default; float a = 0;
                            for (int sy = 0; sy < 3; sy++)
                                for (int sx = 0; sx < 3; sx++)
                                {
                                    var c = px[Mathf.Min(sw - 1, (int)((y + (sy + .5f) / 3f) * k)) * sw + Mathf.Min(sw - 1, (int)((x + (sx + .5f) / 3f) * k))];
                                    acc += new Color(c.r * c.a, c.g * c.a, c.b * c.a, 0); a += c.a;
                                }
                            dst[y * Tile + x] = a > 0 ? new Color(acc.r / a, acc.g / a, acc.b / a, a / 9f) : new Color(0, 0, 0, 0);
                        }
                    // texture rows go up; tile row 0 is the top row of the atlas
                    atlas.SetPixels(col * Tile, (8 - row) * Tile, Tile, Tile, dst);
                    Object.DestroyImmediate(src);
                }
                File.WriteAllBytes(Out + "kit-" + layer + ".png", atlas.EncodeToPNG());
                Object.DestroyImmediate(atlas);
                Debug.Log("[kit] " + layer + " yazıldı");
            }
            UnityEditor.AssetDatabase.Refresh();
        }
    }
}
