using System.Collections.Generic;

namespace ReverseSolver.Core
{
    // One budget for the play visit; layout rebuilds and level retries do not replenish it.
    public sealed class P21LockHints
    {
        int _shown;
        public bool TryShow(int remaining)
        {
            if (remaining <= 0 || _shown >= 2) return false;
            _shown++;
            return true;
        }
    }

    // Runtime prelude only: the authored 40-level sets and their permanent ids stay intact.
    public static class P21Onboarding
    {
        public const string TutorialId = "P21_T01", EasyId = "P21_E01";
        // PM60 only changes new art; the legacy art path keeps its PM59 behaviour.
        public static bool Enabled(string cohort, bool redesign, bool playOverride, bool newArt = true, string tut2x2 = null) =>
            cohort == "p21" && redesign && !playOverride && (!newArt || tut2x2 == "1");
        public static bool IsNewFlow(SaveData save) => save.Best.Count == 0 ||
            save.Attempts.ContainsKey(TutorialId) || save.Best.ContainsKey(TutorialId);

        public static LevelSet Prepend(LevelSet original)
        {
            var levels = new List<LevelData> { Grid(TutorialId, 2, 1), Grid(EasyId, 3, 2) };
            levels.AddRange(original.Levels);
            return new LevelSet(original.Format, levels);
        }

        public static LevelData Grid(string id, int size, int number)
        {
            var pieces = new List<Cell[]>();
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++) pieces.Add(new[] { new Cell(x, y) });
            return new LevelData { Id = id, Version = 1, Number = number, Width = size, Height = size,
                TimerSeconds = 180, Pieces = pieces, V = new int[size, size + 1], H = new int[size + 1, size],
                Sealed = new Dictionary<Dir, int[]>(), Nails = new Dictionary<int, int>(), Bombs = new Dictionary<int, int>(),
                Solution = new List<Move>(), Colors = new int[0], Palette = new string[0],
                Boosters = new Dictionary<string, int> { ["scissors"] = 0, ["wand"] = 0, ["hammer"] = 0, ["clock"] = 0 },
                Unlock = new Dictionary<string, int> { ["scissors"] = 999, ["wand"] = 999, ["hammer"] = 999, ["clock"] = 999 } };
        }
    }

    public sealed class P21Story
    {
        public int Step { get; private set; }
        public int Target { get; private set; } = -1;

        public void Refresh(GameSession session, int removed = -1)
        {
            if (Target >= 0 && removed == Target) Target = -1;
            if (Target >= 0 && session.Board.IsPresent(Target) && session.NailAt(Target) <= 0 &&
                TravelRule.CanExit(session.Board, Target)) return;
            Target = -1;
            if (Step >= 3 || session.IsOver) return;
            foreach (int piece in TravelRule.FreePieces(session.Board))
                if (session.NailAt(piece) <= 0) { Target = piece; Step++; break; }
        }
    }
}
