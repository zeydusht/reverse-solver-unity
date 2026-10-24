using System.Collections.Generic;

namespace ReverseSolver.Core
{
    /// <summary>Mutable occupancy and joint state used by geometric rules and solvers.</summary>
    /// <remarks>
    /// Requires valid geometry: distinct, in-bounds piece cells and correctly sized joints.
    /// Construction does not validate these conditions. Level is shared and its geometry
    /// must remain unchanged while boards use it. Session counters are stored elsewhere.
    /// </remarks>
    public sealed class Board
    {
        public LevelData Level { get; }

        readonly bool[] _present;
        readonly int[] _owner;          // cell index -> piece, -1 when empty
        readonly int[,] _v, _h;

        public int PresentCount { get; private set; }

        public Board(LevelData level)
        {
            Level = level;
            _present = new bool[level.Pieces.Count];
            _owner = new int[level.CellCount];
            for (int i = 0; i < _owner.Length; i++) _owner[i] = -1;
            for (int p = 0; p < level.Pieces.Count; p++)
            {
                _present[p] = true;
                foreach (var c in level.Pieces[p]) _owner[c.Y * level.Width + c.X] = p;
            }
            PresentCount = level.Pieces.Count;
            _v = (int[,])level.V.Clone();
            _h = (int[,])level.H.Clone();
        }

        Board(Board other)
        {
            Level = other.Level;
            _present = (bool[])other._present.Clone();
            _owner = (int[])other._owner.Clone();
            _v = (int[,])other._v.Clone();
            _h = (int[,])other._h.Clone();
            PresentCount = other.PresentCount;
        }

        /// <summary>Copies presence, occupancy, current joints, and the remaining count.</summary>
        /// <remarks>
        /// Removing pieces or editing joints on the copy does not change the source.
        /// Both boards share the same Level reference; level data is not deep-copied.
        /// </remarks>
        public Board Clone() => new Board(this);

        public bool IsPresent(int piece) => _present[piece];

        /* Present pieces in ascending index order, the order the web game's Set iterates in. */
        public IEnumerable<int> PresentPieces()
        {
            for (int p = 0; p < _present.Length; p++)
                if (_present[p]) yield return p;
        }

        /// <summary>Original piece index, or -1 for an empty or out-of-bounds cell.</summary>
        public int PieceAt(Cell c) => Level.OnBoard(c) ? _owner[c.Y * Level.Width + c.X] : -1;

        /// <summary>Empties a present piece; removing it again has no effect.</summary>
        /// <remarks>
        /// Requires a valid original piece index. Does not check exit directions, nails,
        /// bombs, or move legality, and does not change joints or renumber other pieces.
        /// </remarks>
        public void Remove(int piece)
        {
            if (!_present[piece]) return;
            _present[piece] = false;
            PresentCount--;
            foreach (var c in Level.Pieces[piece]) _owner[c.Y * Level.Width + c.X] = -1;
        }

        public int VJoint(int x, int y) => _v[y, x];
        public int HJoint(int x, int y) => _h[y, x];
        internal void SetVJoint(int x, int y, int value) => _v[y, x] = value;
        internal void SetHJoint(int x, int y, int value) => _h[y, x] = value;
    }
}
