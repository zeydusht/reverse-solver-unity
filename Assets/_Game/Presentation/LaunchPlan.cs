namespace ReverseSolver.Presentation
{
    /* What the game opens with (PRODUCT.md A6b, 2026-10-13). A level asked for
       in the link (?lv=N) waits behind the name gate: until the player has
       given a name the menu sits under the gate, so no attempt starts and no
       clock runs while they type; then the level opens at full time. Without
       the gate (name known, debug, the level editor) it opens straight away. */
    public readonly struct LaunchPlan
    {
        /* Start this level now (StartNow), or this one once the name is in (AfterName), -1 for none. */
        public readonly bool StartNow;
        public readonly int Level, AfterName;

        LaunchPlan(bool now, int level, int after) { StartNow = now; Level = level; AfterName = after; }

        /* requested: level index from the link or the editor, -1 for the menu. */
        public static LaunchPlan For(int requested, bool askName)
        {
            if (requested < 0) return new LaunchPlan(false, -1, -1);
            return askName ? new LaunchPlan(false, -1, requested) : new LaunchPlan(true, requested, -1);
        }
    }
}
