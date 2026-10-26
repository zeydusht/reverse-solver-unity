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
