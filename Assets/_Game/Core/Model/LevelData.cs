using System.Collections.Generic;

namespace ReverseSolver.Core
{
    /* One level as authored. Immutable: play state lives in Board.

       Joints. A boundary between two cells holds +1, -1 or 0:
         +1 / -1  one side owns the tab, the pair is interlocked
          0       flat seam, the two slide past each other
       VJoint(x, y) is the boundary on the left of cell (x, y), x in 0..W;
       HJoint(x, y) is the boundary above cell (x, y), y in 0..H. Entries on
       the board's outer edge are null in the source and stored as 0; the rule
       code never reads them. */
    public sealed class LevelData
    {
        public string Id { get; internal set; }
        public int Version { get; internal set; }
        public int Number { get; internal set; }          // `lv`: position in the web order
        public int Width { get; internal set; }
        public int Height { get; internal set; }
        public int TimerSeconds { get; internal set; }

        /* Each piece is a list of cells; a chained pair is one piece with two. */
        public IReadOnlyList<Cell[]> Pieces { get; internal set; }

        internal int[,] V;                                // [y, x], x in 0..W
        internal int[,] H;                                // [y, x], y in 0..H
        public int VJoint(int x, int y) => V[y, x];
        public int HJoint(int x, int y) => H[y, x];

        /* Wall segments: for each side, the lanes (column for U/D, row for L/R) that are sealed. */
        public IReadOnlyDictionary<Dir, int[]> Sealed { get; internal set; }
        /* piece index -> counter */
        public IReadOnlyDictionary<int, int> Nails { get; internal set; }
        public IReadOnlyDictionary<int, int> Bombs { get; internal set; }

        public IReadOnlyList<Move> Solution { get; internal set; }

        // Presentation data, carried through untouched.
        public int[] Colors { get; internal set; }        // per cell, y * W + x -> palette index
        public string[] Palette { get; internal set; }
        public string Art { get; internal set; }
        public LevelIntro Intro { get; internal set; }    // obstacle introduction, may be null
        public LevelIntro BoosterIntro { get; internal set; }
        public IReadOnlyDictionary<string, int> Boosters { get; internal set; }
        public IReadOnlyDictionary<string, int> Unlock { get; internal set; }

        // Generator outputs, kept for comparing prediction with play data.
        public int Load { get; internal set; }
        public int Open { get; internal set; }

        public int CellCount => Width * Height;
        public bool OnBoard(Cell c) => c.X >= 0 && c.X < Width && c.Y >= 0 && c.Y < Height;

        public override string ToString() => $"{Id} v{Version}";
    }

    public sealed class LevelIntro
    {
        public string Id;      // booster id for a booster intro, otherwise null
        public string Icon;
        public string Title;
        public string Body;
        public string Tip;
    }

    public sealed class LevelSet
    {
        public int Format { get; }
        public IReadOnlyList<LevelData> Levels { get; }

        readonly Dictionary<string, LevelData> _byId;

        internal LevelSet(int format, List<LevelData> levels)
        {
            Format = format;
            Levels = levels;
            _byId = new Dictionary<string, LevelData>(levels.Count);
            foreach (var l in levels) _byId.Add(l.Id, l);
        }

        public LevelData this[string id] => _byId[id];
        public bool TryGet(string id, out LevelData level) => _byId.TryGetValue(id, out level);
    }
}
