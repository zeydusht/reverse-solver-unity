using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* The mandatory palette (docs/art/PALETTE.md, Zeyd 2026-10-26). Every colour the code puts on the preview art
       (digits, rims, urgent, tints) comes from here; nothing outside this table is used with the art on. */
    public static class Palette
    {
        public static readonly Vector4 GoldLight = Draw.Hex("#FBBA0E"), Gold = Draw.Hex("#F38A03"), GoldShade = Draw.Hex("#CE5D01"),
            GoldGlow = Draw.Hex("#FFF1C2"), Teal = Draw.Hex("#1C7F8C"), TealDark = Draw.Hex("#0F4F5A"), Cream = Draw.Hex("#F8EBD0"),
            Navy = Draw.Hex("#0B2A36"), Coral = Draw.Hex("#E8574A");
        public static readonly Vector4 White = new Vector4(1, 1, 1, 1);
        public static Color C(Vector4 v, float a = 1) => new Color(v.x, v.y, v.z, a);
        /* Old drawn colour without the art, palette colour with it. */
        public static Vector4 Or(Vector4 drawn, Vector4 palette) => Art.On ? palette : drawn;
    }
}
