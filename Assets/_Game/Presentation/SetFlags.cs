namespace ReverseSolver.Presentation
{
    /* Faz 4 G8 switches (PRODUCT.md "Faz 4"). Both stay off until Zeyd says so:
         RedesignDefault  the redesigned set (redesign.json, R.. levels) becomes the
                          game's default set; only on Zeyd's command "yeni seti canlıya al".
         AskFun           the 👍/👎 question after a won level; only after
                          docs/sql/004_keyif.sql has been run and checked, with Zeyd's approval.
       The redesigned set can always be played with ?debug=1&set=redesign (nothing is sent). */
    public static class SetFlags
    {
        public static readonly bool RedesignDefault = false;
        public static readonly bool AskFun = false;
    }

    /* Which level file the game plays. */
    public static class SetChoice
    {
        public enum Kind { Levels, Designs, Redesign, Zeydbot }

        /* set=zeydbot (ZeydBot sessions, Zeyd 2026-10-19) works without debug: its rows and moves are sent
           under the player's name (TEST_ZEYD), every level open; nothing else changes for the live set. */
        public static Kind For(bool debug, string setParam, bool hasDesigns, bool hasRedesign, bool redesignDefault, bool hasZeydbot = false)
        {
            if (setParam == "zeydbot" && hasZeydbot) return Kind.Zeydbot;
            if (debug && setParam == "designs" && hasDesigns) return Kind.Designs;
            if (debug && setParam == "redesign" && hasRedesign) return Kind.Redesign;
            if (debug && setParam == "levels") return Kind.Levels;
            if (redesignDefault && hasRedesign) return Kind.Redesign;
            return Kind.Levels;
        }
    }
}
