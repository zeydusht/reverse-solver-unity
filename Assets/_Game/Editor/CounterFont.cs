using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace ReverseSolver.EditorTools
{
    /* Counter digits on obstacles (PM_MESAJ_12): Fredoka SemiBold (OFL, Resources/Fonts/OFL-Fredoka.txt),
       a static SDF atlas with 0-9 only, so nothing is generated at runtime in WebGL.

         bash Tools/unity_batch.sh method ReverseSolver.EditorTools.CounterFont.Build */
    public static class CounterFont
    {
        const string Dir = "Assets/_Game/Presentation/Resources/Fonts/";

        /* Lilita One (OFL, Resources/Fonts/LilitaOne-OFL.txt) for English menu words ("Level 7", "Levels", "Jiggles
           rescued"): printable ASCII only, the font has no Turkish letters.
             bash Tools/unity_batch.sh method ReverseSolver.EditorTools.CounterFont.BuildLilita */
        public static void BuildLilita()
        {
            var font = AssetDatabase.LoadAssetAtPath<Font>(Dir + "LilitaOne-Regular.ttf");
            var fa = TMP_FontAsset.CreateFontAsset(font, 90, 9, GlyphRenderMode.SDFAA, 1024, 512, AtlasPopulationMode.Dynamic);
            var ascii = new System.Text.StringBuilder();
            for (char c = ' '; c <= '~'; c++) ascii.Append(c);
            fa.TryAddCharacters(ascii.ToString());
            Save(fa, "Lilita");
        }

        /* Fredoka SemiBold (OFL) for Turkish texts with the preview art (Lilita has no ğ, ş, İ): ASCII + Turkish letters.
             bash Tools/unity_batch.sh method ReverseSolver.EditorTools.CounterFont.BuildFredoka */
        public static void BuildFredoka()
        {
            var font = AssetDatabase.LoadAssetAtPath<Font>(Dir + "Fredoka-SemiBold.ttf");
            var fa = TMP_FontAsset.CreateFontAsset(font, 56, 6, GlyphRenderMode.SDFAA, 512, 512, AtlasPopulationMode.Dynamic);
            var chars = new System.Text.StringBuilder("çÇğĞıİöÖşŞüÜâÂîÎûÛ·…’‘“”–—👍");
            for (char c = ' '; c <= '~'; c++) chars.Append(c);
            fa.TryAddCharacters(chars.ToString(), out string missing);
            if (!string.IsNullOrEmpty(missing)) Debug.Log("[font] Fredoka eksik: " + missing);
            // Fredoka has no ğ, ş, İ (checked in the full Google Fonts file too): those come from the game's RS Sans
            var rs = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Game/Presentation/Fonts/RS Sans SDF.asset");
            if (rs != null) fa.fallbackFontAssetTable = new System.Collections.Generic.List<TMP_FontAsset> { rs };
            Save(fa, "Fredoka");
        }

        /* Nunito Black (OFL, Resources/Fonts/OFL-Nunito.txt) for menu banners (PM_MESAJ_27 A3): every Turkish letter.
           Wide padding so the thick outline and the shadow fit in the SDF.
             bash Tools/unity_batch.sh method ReverseSolver.EditorTools.CounterFont.BuildTitle */
        public static void BuildTitle()
        {
            var font = AssetDatabase.LoadAssetAtPath<Font>(Dir + "Nunito-Black.ttf");
            var fa = TMP_FontAsset.CreateFontAsset(font, 56, 11, GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.Dynamic, false);
            var chars = new System.Text.StringBuilder("çÇğĞıİöÖşŞüÜâÂîÎûÛ·…’‘“”–—");
            for (char c = ' '; c <= '~'; c++) chars.Append(c);
            fa.TryAddCharacters(chars.ToString(), out string missing);
            if (!string.IsNullOrEmpty(missing)) Debug.Log("[font] Title eksik: " + missing);
            // the banner's outline + shadow variant must reach the build (shader_feature keywords are stripped otherwise)
            fa.material.EnableKeyword("OUTLINE_ON"); fa.material.EnableKeyword("UNDERLAY_ON");
            fa.material.SetColor("_UnderlayColor", new Color(0, 0, 0, 0));   // plain labels in this font: no shadow
            Save(fa, "Title");
        }

        /* Baloo 2 ExtraBold (OFL, Resources/Fonts/OFL-Baloo2.txt) for card titles and buttons (PM_MESAJ_42):
           ASCII + Turkish letters only, a static atlas kept small (<= 0.3 MB in the download).
             bash Tools/unity_batch.sh method ReverseSolver.EditorTools.CounterFont.BuildBaloo */
        public static void BuildBaloo()
        {
            var font = AssetDatabase.LoadAssetAtPath<Font>(Dir + "Baloo2-ExtraBold.ttf");
            var fa = TMP_FontAsset.CreateFontAsset(font, 48, 9, GlyphRenderMode.SDFAA, 1024, 512, AtlasPopulationMode.Dynamic, false);
            var chars = new System.Text.StringBuilder("çÇğĞıİöÖşŞüÜâÂîÎûÛ·…’‘“”–—");
            for (char c = ' '; c <= '~'; c++) chars.Append(c);
            fa.TryAddCharacters(chars.ToString(), out string missing);
            if (!string.IsNullOrEmpty(missing)) Debug.Log("[font] Baloo eksik: " + missing);
            fa.material.EnableKeyword("OUTLINE_ON"); fa.material.EnableKeyword("UNDERLAY_ON");
            Save(fa, "Baloo");
        }

        static void Save(TMP_FontAsset fa, string name)
        {
            fa.atlasPopulationMode = AtlasPopulationMode.Static;
            fa.name = name;
            AssetDatabase.DeleteAsset(Dir + name + ".asset");
            AssetDatabase.CreateAsset(fa, Dir + name + ".asset");
            fa.atlasTexture.name = name + " Atlas";
            AssetDatabase.AddObjectToAsset(fa.atlasTexture, fa);
            fa.material.name = name + " Material";
            AssetDatabase.AddObjectToAsset(fa.material, fa);
            AssetDatabase.SaveAssets();
            Debug.Log("[font] " + name + ".asset: " + fa.characterTable.Count + " karakter");
        }

        public static void Build()
        {
            var font = AssetDatabase.LoadAssetAtPath<Font>(Dir + "Fredoka-SemiBold.ttf");
            var fa = TMP_FontAsset.CreateFontAsset(font, 90, 9, GlyphRenderMode.SDFAA, 512, 256, AtlasPopulationMode.Dynamic);
            fa.TryAddCharacters("0123456789");
            fa.atlasPopulationMode = AtlasPopulationMode.Static;
            fa.name = "Counter";
            AssetDatabase.DeleteAsset(Dir + "Counter.asset");
            AssetDatabase.CreateAsset(fa, Dir + "Counter.asset");
            fa.atlasTexture.name = "Counter Atlas";
            AssetDatabase.AddObjectToAsset(fa.atlasTexture, fa);
            fa.material.name = "Counter Material";
            AssetDatabase.AddObjectToAsset(fa.material, fa);
            AssetDatabase.SaveAssets();
            Debug.Log("[font] Counter.asset: " + fa.characterTable.Count + " karakter");
        }
    }
}
