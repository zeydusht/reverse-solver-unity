using System;

namespace ReverseSolver.Editing
{
    /* G1 picture placement (PRODUCT.md, A7): which part of the source picture
       a w x h board shows. Without adjustment it is the largest centred
       rectangle of the board's shape (cover). Zoom (1..MaxZoom) shrinks that
       window around its centre; pan (-1..1 on each axis) slides it within the
       room the zoom leaves, -1 = left/top edge, +1 = right/bottom edge.
       Coordinates are texture UVs: (0, 0) bottom-left, y up. Look only. */
    public readonly struct ImageCrop : IEquatable<ImageCrop>
    {
        public const float MaxZoom = 4f;
        public static readonly ImageCrop None = new ImageCrop(1, 0, 0);

        public readonly float Zoom, PanX, PanY;

        public ImageCrop(float zoom, float panX, float panY)
        {
            Zoom = Math.Max(1f, Math.Min(MaxZoom, zoom));
            PanX = Math.Max(-1f, Math.Min(1f, panX));
            PanY = Math.Max(-1f, Math.Min(1f, panY));
        }

        public bool IsNone => Zoom == 1f && PanX == 0f && PanY == 0f;

        /* (x, y, width, height) in UVs of a srcW x srcH picture for a board of w x h cells. */
        public (float x, float y, float w, float h) Window(int srcW, int srcH, int boardW, int boardH)
        {
            float srcAspect = (float)srcW / srcH, aspect = (float)boardW / boardH;
            float cw = srcAspect > aspect ? aspect / srcAspect : 1f;      // cover: trim the longer side
            float ch = srcAspect > aspect ? 1f : srcAspect / aspect;
            float w = cw / Zoom, h = ch / Zoom;
            // room left inside the cover window, then inside the whole picture for the pan
            float cx = .5f + PanX * (1f - w) / 2f;
            float cy = .5f - PanY * (1f - h) / 2f;                         // pan down = lower in the picture = smaller v
            return (cx - w / 2f, cy - h / 2f, w, h);
        }

        public bool Equals(ImageCrop o) => Zoom == o.Zoom && PanX == o.PanX && PanY == o.PanY;
        public override bool Equals(object obj) => obj is ImageCrop c && Equals(c);
        public override int GetHashCode() => Zoom.GetHashCode() * 31 + PanX.GetHashCode() * 7 + PanY.GetHashCode();
    }

    /* G1 readability (PRODUCT.md, A7): the piece seams are drawn as a white rim
       over a dark rim taken from the picture. Where the picture is light
       around a seam, the white rim blends in and tabs are hard to read. The
       score is 1 - the mean lightness of the picture in a band around every
       inner seam (lightness eased toward grey like the shader does); low
       scores warn. The threshold is the PM's. */
    public static class SeamReadability
    {
        /* lum: lightness 0..1 per pixel, row 0 at the top, imgW x imgH; the picture covers the board. */
        public static float Score(float[] lum, int imgW, int imgH, int boardW, int boardH, float bandCells = .08f)
        {
            if (lum == null || lum.Length != imgW * imgH || boardW < 1 || boardH < 1) return 1f;
            double sum = 0;
            long n = 0;
            float cellW = (float)imgW / boardW, cellH = (float)imgH / boardH;
            float bx = bandCells * cellW, by = bandCells * cellH;
            for (int py = 0; py < imgH; py++)
                for (int px = 0; px < imgW; px++)
                {
                    float fx = px / cellW, fy = py / cellH;
                    float dx = Math.Abs(fx - (float)Math.Round(fx)) * cellW;
                    float dy = Math.Abs(fy - (float)Math.Round(fy)) * cellH;
                    bool nearV = dx <= bx && Math.Round(fx) > 0 && Math.Round(fx) < boardW;
                    bool nearH = dy <= by && Math.Round(fy) > 0 && Math.Round(fy) < boardH;
                    if (!nearV && !nearH) continue;
                    float l = lum[py * imgW + px];
                    sum += l + (.5f - l) * .12f;                             // the shader eases contrast by 12%
                    n++;
                }
            return n == 0 ? 1f : (float)(1 - sum / n);
        }
    }
}
