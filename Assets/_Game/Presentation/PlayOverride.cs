using System;
using ReverseSolver.Core;

namespace ReverseSolver.Presentation
{
    /* Lets the level editor (Unity editor only) open a level in the real game
       scene and take the screen back afterwards. Unset in every build: the
       editor is the only code that sets it. */
    public static class PlayOverride
    {
        /* When set, GameRoot plays Set[Index] instead of its level file. */
        public static LevelSet Set;
        public static int Index;
        /* Called instead of showing the menu (menu button, Esc). */
        public static Action Exit;

        public static bool Active => Set != null;

        public static void Clear()
        {
            Set = null;
            Index = 0;
            Exit = null;
        }
    }
}
