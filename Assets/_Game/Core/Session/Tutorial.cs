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

        /* SET40 v3 tutorial levels (PM_MESAJ_6): the hand walks the stored solution step by step. It points at
           the first solution move whose piece is still on the board and can leave now, for the first `steps`
           removals of the attempt (tutorial 1: every move; tutorial 2: fewer, "el ve yazı daha az"). */
        public static int StepsOf(LevelData l) => l.Tutorial == 1 ? int.MaxValue : l.Tutorial == 2 ? 3 : 0;

        public static bool Step(GameSession s, out Move move)
        {
            move = default;
            var l = s.Level;
            if (s.IsOver || l.Pieces.Count - s.Board.PresentCount >= StepsOf(l)) return false;
            foreach (var m in l.Solution)
            {
                if (!s.Board.IsPresent(m.Piece) || s.NailAt(m.Piece) > 0) continue;
                if (s.Probe(m.Piece, m.Dir).Exit) { move = m; return true; }
            }
            return false;
        }
    }
}
