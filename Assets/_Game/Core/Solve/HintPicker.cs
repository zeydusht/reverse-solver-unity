namespace ReverseSolver.Core
{
    /* PM_84: the piece the idle hint shows (levels 1..ipucu_son_bolum): a move that gets a piece out now and does not
       spoil the rest. With a bomb on the board, SmartPlayer's first move from here (the bomb piece, or what blocks
       it, before its fuse runs out). Otherwise the first move of the level's own solution that leaves now, then any
       piece that leaves now (removing a piece never blocks another, so it cannot spoil the rest). Null when no piece
       can leave. */
    public static class HintPicker
    {
        public static Move? Next(GameSession s, int nodeBudget = 50_000)
        {
            if (s == null || s.IsOver) return null;
            bool bomb = false;
            foreach (var b in s.Bombs) if (s.Board.IsPresent(b.Key)) { bomb = true; break; }
            if (bomb)
            {
                var r = SmartPlayer.PlayFrom(s, nodeBudget);
                if (r.Moves.Count > 0 && Leaves(s, r.Moves[0])) return r.Moves[0];
            }
            foreach (var m in s.Level.Solution)
                if (Leaves(s, m)) return m;
            for (int p = 0; p < s.Level.Pieces.Count; p++)
            {
                if (!s.Board.IsPresent(p)) continue;
                foreach (var d in TravelRule.ExitDirs(s.Board, p))
                    if (Leaves(s, new Move(p, d))) return new Move(p, d);
            }
            return null;
        }

        static bool Leaves(GameSession s, Move m) =>
            s.Board.IsPresent(m.Piece) && s.NailAt(m.Piece) <= 0 && s.Probe(m.Piece, m.Dir).Exit;
    }
}
