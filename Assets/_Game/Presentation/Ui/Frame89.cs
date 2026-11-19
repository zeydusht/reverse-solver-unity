using System.Collections.Generic;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_89 §1 (Zeyd: "at the end the picture grows short and long"): the gallery's frames are drawn by the game, each
       opening at its level's board proportions, so what the frame shows is exactly the board's picture (the same
       crop, the same scale across and down) from the menu to the game. The art (Art/yeni/pm89/frame89, Codex's flat
       frame with its shadow, Tools/pm89_frame_import.py) is sliced 5 x 5: the corners and the middle ornaments keep
       their size, the plain mouldings between them stretch. Positions are frame px from the opening's top left
       (y down); the caller lays them on the wall (the menu's diorama: 3D, the corridor: its strip's perspective). */
    public static class Frame89
    {
        public const string Key = "Art/yeni/pm89/frame89";
        public const float Size = 1320, Lip = 210;   // the art's side; the opening starts Lip px in (210 .. 1110)
        static readonly float[] Cut = { 0, 390, 540, 780, 930, 1320 };
        static readonly int[] Steps = { 3, 3, 3, 3, 3 };   // grid lines per slice (the perspective's resolution)
        public const float MinOpening = 620;              // below this the stretches would turn over

        static Texture2D _tex;
        static bool _tried;
        public static Texture2D Texture
        {
            get
            {
                if (!_tried) { _tried = true; _tex = Resources.Load<Texture2D>(Key); if (_tex != null) _tex.wrapMode = TextureWrapMode.Clamp; }
                return _tex;
            }
        }
        public static bool On => Texture != null;

        /* The target's breaks along one side for an opening `open` px long. */
        static float[] Breaks(float open)
        {
            float s = (open + 2 * Lip - (Cut[1] + (Cut[3] - Cut[2]) + (Cut[5] - Cut[4]))) / 2;   // each stretch's new length
            return new[] { 0, Cut[1], Cut[1] + s, Cut[1] + s + (Cut[3] - Cut[2]), Cut[1] + 2 * s + (Cut[3] - Cut[2]), 2 * Cut[1] + 2 * s + (Cut[3] - Cut[2]) };
        }

        /* The frame round an opening of `open` frame px: positions (from the opening's top left, y down), uv, tris. */
        public static void Build(Vector2 open, List<Vector2> pos, List<Vector2> uv, List<int> tris)
        {
            open = Vector2.Max(open, Vector2.one * MinOpening);
            float[] bx = Breaks(open.x), by = Breaks(open.y);
            var xs = new List<(float t, float s)>(); var ys = new List<(float t, float s)>();
            Lines(bx, xs); Lines(by, ys);
            int nx = xs.Count, b0 = pos.Count;
            foreach (var (ty, sy) in ys)
                foreach (var (tx, sx) in xs)
                {
                    pos.Add(new Vector2(tx - Lip, ty - Lip));
                    uv.Add(new Vector2(sx / Size, 1 - sy / Size));
                }
            for (int j = 0; j < ys.Count - 1; j++)
                for (int i = 0; i < nx - 1; i++)
                {
                    // the middle (inside the opening on every slice) is empty
                    float cx = (xs[i].s + xs[i + 1].s) / 2, cy = (ys[j].s + ys[j + 1].s) / 2;
                    if (cx > Cut[1] && cx < Cut[4] && cy > Cut[1] && cy < Cut[4]) continue;
                    int s = b0 + j * nx + i;
                    tris.AddRange(new[] { s, s + 1, s + nx + 1, s, s + nx + 1, s + nx });
                }
        }

        static void Lines(float[] target, List<(float, float)> o)
        {
            for (int k = 0; k < 5; k++)
                for (int i = k == 0 ? 0 : 1; i <= Steps[k]; i++)
                {
                    float f = (float)i / Steps[k];
                    o.Add((Mathf.Lerp(target[k], target[k + 1], f), Mathf.Lerp(Cut[k], Cut[k + 1], f)));
                }
        }

        /* The opening's size and the frame px per unit for a picture of `aspect` (width / height) on a wall whose
           opening is `height` units tall: the border is `border` units wide whatever the size. */
        public static Vector2 Opening(float aspect, float height, float border, out float pxPerUnit)
        {
            pxPerUnit = (Lip - 60) / border;   // the art's border is 150 px (Lip less its 60 px of shadow room)
            return new Vector2(aspect * height, height) * pxPerUnit;
        }
    }
}
