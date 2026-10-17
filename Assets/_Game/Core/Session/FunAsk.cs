namespace ReverseSolver.Core
{
    /* When the 👍/👎 question comes (PM 2026-10-14 gece (3), P1 karar 4): once per player and
       level, on the won attempt or after the third lost one, whichever comes first. A quit is
       not a loss. The row of that attempt carries the answer, so its result tells whether it
       was given after a win or after the losses. */
    public static class FunAsk
    {
        public const int AfterLosses = 3;

        /* lossesBefore: lost attempts on this level before this one. */
        public static bool ShouldAsk(Outcome outcome, int lossesBefore, bool alreadyAsked)
        {
            if (alreadyAsked) return false;
            if (outcome == Outcome.Win) return true;
            if (outcome == Outcome.Time || outcome == Outcome.Bomb) return lossesBefore + 1 >= AfterLosses;
            return false;
        }
    }
}
