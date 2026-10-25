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
