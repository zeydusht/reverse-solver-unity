using System;

namespace ReverseSolver.Core
{
    /* Board size on screen and the touch margin around it; the one place the
       game (GameRoot) and the level editor's phone warning both take it from.

       The web game's sizeCell is min((W - 38) / w, (H - 262) / h, 78): 19 pt
       either side, which leaves the tray 1-2 pt from the screen edge, and a
       piece in an outer column has to be pulled past the edge to leave
       (PRODUCT.md K1). The side gap is now
           G = clamp((W - 44 w) / 2, 19, 34)
       so boards keep 44 pt cells where they can and get up to 34 pt of room to
       pull in. A deliberate difference from the web. */
    public static class BoardLayout
    {
        public const float MinCell = 44f, MaxCell = 78f;
        public const float MinGap = 19f, MaxGap = 34f;
        public const float VerticalReserve = 262f;   // strip, prompt, booster bar (web sizeCell)

        public static float SideGap(float screenW, int width) =>
            Math.Max(MinGap, Math.Min(MaxGap, (screenW - MinCell * width) / 2f));

        /* innerH: screen height minus the safe-area insets. */
        public static float CellSize(float screenW, float innerH, int width, int height) =>
            (float)Math.Floor(Math.Min(Math.Min((screenW - 2 * SideGap(screenW, width)) / width,
                                                (innerH - VerticalReserve) / height), MaxCell));
    }

    /* Grabbing a piece from the margin around the board (PRODUCT.md K1): a
       press up to Reach points outside the board, on the tray's rim or on a
       wall, takes the piece in the nearest outer cell within that distance. */
    public static class EdgeGrab
    {
        public const float Reach = 24f;

        /* (px, py): the press in board points, (0, 0) at the board's top-left. */
        public static int PieceAt(Board b, float px, float py, float cell, out bool outside)
        {
            var l = b.Level;
            float w = l.Width * cell, h = l.Height * cell;
            outside = px < 0 || py < 0 || px >= w || py >= h;
            if (!outside) return b.PieceAt(new Cell((int)(px / cell), (int)(py / cell)));

            int best = -1;
            float bestD = Reach + 1e-3f;
            for (int y = 0; y < l.Height; y++)
                for (int x = 0; x < l.Width; x++)
                {
                    if (x != 0 && y != 0 && x != l.Width - 1 && y != l.Height - 1) continue;
                    int p = b.PieceAt(new Cell(x, y));
                    if (p < 0) continue;
                    float dx = Math.Max(Math.Max(x * cell - px, px - (x + 1) * cell), 0);
                    float dy = Math.Max(Math.Max(y * cell - py, py - (y + 1) * cell), 0);
                    float d = (float)Math.Sqrt(dx * dx + dy * dy);
                    if (d < bestD) { bestD = d; best = p; }
                }
            return best;
        }
    }
}
