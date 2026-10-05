using System.Collections.Generic;

namespace ReverseSolver.Core
{
    /* The mutable side of a level: which pieces are still on the board and the
       current joints (boosters will rewrite those in M2). Removing a piece is
       the only way the board changes shape, and it only ever empties cells. */
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

        public Board Clone() => new Board(this);

        public bool IsPresent(int piece) => _present[piece];

        /* Present pieces in ascending index order, the order the web game's Set iterates in. */
        public IEnumerable<int> PresentPieces()
        {
            for (int p = 0; p < _present.Length; p++)
                if (_present[p]) yield return p;
        }

        public int PieceAt(Cell c) => Level.OnBoard(c) ? _owner[c.Y * Level.Width + c.X] : -1;

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
