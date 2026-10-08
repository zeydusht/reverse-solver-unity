namespace ReverseSolver.Core
{
    /* The web game's first-level hand (index.html drawTutorial / clearTutorial).

       The hand points at the first move of the level's stored solution, on the
       first level of the set, while the board is still untouched. It goes for
       good, for the rest of the page visit, once any piece leaves the board on
       any level (the web clears it in flyOut and in the hammer, and its flag is
       not saved: a reload shows it again on level 1).

       A pure query: it never sends a command, so time, attempts and jams are
       exactly what they are without it. One guard the web lacks: it never
       points at a move the session would refuse (on level 1 no booster is
       unlocked, so the board cannot change before the first move anyway). */
    public static class Tutorial
    {
        public static bool Hint(GameSession s, bool firstLevel, bool done, out Move move)
        {
            move = default;
            if (done || !firstLevel || s.IsOver) return false;
            var l = s.Level;
            if (s.Board.PresentCount != l.Pieces.Count || l.Solution.Count == 0) return false;
            move = l.Solution[0];
            if (!s.Board.IsPresent(move.Piece) || s.NailAt(move.Piece) > 0) return false;
            return s.Probe(move.Piece, move.Dir).Exit;
        }
    }
}
