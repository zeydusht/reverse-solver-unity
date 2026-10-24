using System;

namespace ReverseSolver.Core
{
    /* Obstacle and density counts derived from a level, never stored in the
       file. These are the inputs the difficulty model will be fitted on, so
       they must come from the level itself rather than from hand-kept fields. */
    /// <summary>
    /// Snapshot of authored geometry, not remaining Board state or a solveability score.
    /// Counts collection entries as supplied; does not validate piece indices, wall
    /// lanes or counter values, and does not deduplicate wall lanes.
    /// </summary>
    public readonly struct LevelStats
    {
        public readonly int Width, Height, Pieces;
        public readonly int Chains;        // pieces with more than one cell
        public readonly int Nails;         // dictionary entries, not sum of nail counters
        public readonly int Bombs;         // dictionary entries, not sum of bomb counters
        public readonly int SealedSides;   // board sides with any wall segment
        public readonly int SealedLanes;   // wall segments in total
        public readonly int Joints;        // all internal cell boundaries, even within one piece
        public readonly int OpenJoints;    // of those, flat seams (0)

        LevelStats(LevelData l)
        {
            Width = l.Width;
            Height = l.Height;
            Pieces = l.Pieces.Count;
            int chains = 0;
            foreach (var p in l.Pieces) if (p.Length > 1) chains++;
            Chains = chains;
            Nails = l.Nails.Count;
            Bombs = l.Bombs.Count;

            int sides = 0, lanes = 0;
            foreach (var s in l.Sealed)
                if (s.Value.Length > 0) { sides++; lanes += s.Value.Length; }
            SealedSides = sides;
            SealedLanes = lanes;

            int joints = 0, open = 0;
            for (int y = 0; y < l.Height; y++)
                for (int x = 1; x < l.Width; x++) { joints++; if (l.VJoint(x, y) == 0) open++; }
            for (int y = 1; y < l.Height; y++)
                for (int x = 0; x < l.Width; x++) { joints++; if (l.HJoint(x, y) == 0) open++; }
            Joints = joints;
            OpenJoints = open;
        }

        /// <summary>
        /// Recomputes counts from a non-null, fully populated level with joint arrays
        /// matching its dimensions. Later edits to the source do not update this value.
        /// LevelData.Open and Load are not used.
        /// </summary>
        public static LevelStats Of(LevelData level) => new LevelStats(level);

        /* Open joints as a whole percent, computed the way the level generator
           did (divide first), so it reproduces the `open` field of the web data.
           Includes every internal grid seam, regardless of piece occupancy; excludes
           outer edges and walls. Math.Round uses midpoint-to-even rounding (e.g.
           12.5 -> 12, 37.5 -> 38); a board with no internal seams reports 0. */
        public int OpenPercent => Joints == 0 ? 0 : (int)Math.Round((double)OpenJoints / Joints * 100);

        public int Cells => Width * Height;

        public override string ToString() =>
            $"{Width}x{Height} pieces={Pieces} chains={Chains} nails={Nails} bombs={Bombs} " +
            $"sealed={SealedLanes} lanes/{SealedSides} sides open={OpenPercent}%";
    }
}
