using System;
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
        public const int LockLesson = 3, ChainLesson = 10, WallLesson = 14, BombLesson = 20;
        public static bool IsObstacleLesson(int slot) => slot == LockLesson || slot == ChainLesson ||
            slot == WallLesson || slot == BombLesson;
        // PM60 only changes new art; the legacy art path keeps its PM59 behaviour.
        public static bool Enabled(string cohort, bool redesign, bool playOverride, bool newArt = true, string tut2x2 = null) =>
            (cohort == null || TelemetryMeta.P21Line(cohort)) && redesign && !playOverride && (!newArt || tut2x2 == "1");
        public static bool IsNewFlow(SaveData save) => save.Best.Count == 0 ||
            HasPreludeProgress(save);

        public static bool HasPreludeProgress(SaveData save) =>
            save.Attempts.ContainsKey(TutorialId) || save.Best.ContainsKey(TutorialId);

        // PM_64: Codex's 2x2 guided opening is opt-in (&tut=2x2); the default is the PM_60 story level.
        public static bool UseGuided(string cohort, bool redesign, bool playOverride, bool newArt, string flag) =>
            (cohort == null || TelemetryMeta.P21Line(cohort)) && redesign && !playOverride && newArt && flag == "1";
        // PM_64: obstacle lessons at 3/10/14/20 are on by default (&dersler=0 turns them off).
        public static bool UseLessons(string cohort, bool redesign, bool playOverride, bool newArt, string flag) =>
            (cohort == null || TelemetryMeta.P21Line(cohort)) && redesign && !playOverride && newArt && flag != "0";

        public static LevelSet ReplaceOpening(LevelSet original)
        {
            var levels = new List<LevelData>(original.Levels);
            if (levels.Count < 2) return original;
            for (int i = 0; i < 2; i++)
            {
                var previous = levels[i];
                var lesson = Grid(previous.Id, i + 2, previous.Number);
                lesson.Version = previous.Version + 1;
                lesson.Image = previous.Image;
                levels[i] = lesson;
            }
            for (int i = 2; i < Math.Min(10, levels.Count); i++)
            {
                var level = levels[i];
                int minimum = Math.Max(90, 4 * level.Pieces.Count);
                levels[i] = level.WithTimerSeconds(Math.Max(level.TimerSeconds, minimum));
            }
            return new LevelSet(original.Format, levels);
        }

        public static (int Step, int Piece, Dir? Direction, bool Blocked) GuidedSlide(int action) => action switch
        {
            0 => (1, 2, Dir.D, false), // lower left leaves
            1 => (2, 3, Dir.U, true),  // lower right runs into upper right
            2 => (2, 3, Dir.D, false),
            3 => (3, 1, Dir.L, true),  // upper right runs into upper left
            4 => (3, 1, Dir.R, false),
            _ => (4, 0, null, false)
        };

        public static LevelSet SpaceObstacleLessons(LevelSet original)
        {
            var levels = new List<LevelData>(original.Levels);
            for (int i = 0; i < Math.Min(BombLesson, levels.Count); i++)
            {
                int slot = i + 1;
                var level = levels[i];
                bool deferBomb = slot < BombLesson && (level.Bombs.Count > 0 || level.Intro?.Icon == "bomb");
                bool isolateLesson = slot == ChainLesson || slot == WallLesson;
                if (slot == BombLesson)
                {
                    levels[i] = CreateBombLesson(level);
                    continue;
                }
                if (!deferBomb && !isolateLesson) continue;
                var lesson = level.CopyNextVersion();
                if (deferBomb)
                {
                    lesson.Bombs = new Dictionary<int, int>();
                    if (lesson.Intro?.Icon == "bomb") lesson.Intro = null;
                }
                if (isolateLesson)
                {
                    lesson.Nails = new Dictionary<int, int>();
                    var board = new Board(lesson);
                    var path = GreedySolver.Solve(board);
                    if (path.Solved)
                    {
                        var moves = new List<Move>();
                        foreach (int piece in path.Order)
                        {
                            moves.Add(new Move(piece, TravelRule.ExitDirs(board, piece)[0]));
                            board.Remove(piece);
                        }
                        lesson.Solution = moves;
                    }
                }
                levels[i] = lesson;
            }
            return new LevelSet(original.Format, levels);
        }

        static LevelData CreateBombLesson(LevelData previous)
        {
            var lesson = Grid(previous.Id, 4, previous.Number);
            lesson.Version = previous.Version + 1;
            lesson.Image = previous.Image;
            lesson.Unlock = previous.Unlock;
            lesson.Boosters = previous.Boosters;
            // One piece removed per turn leaves enough fuse even if the bomb goes last.
            lesson.Bombs = new Dictionary<int, int> { [10] = lesson.Pieces.Count };
            lesson.Intro = new LevelIntro { Icon = "bomb", Title = "Bombalı parça" };
            var solution = new List<Move>();
            for (int piece = 0; piece < lesson.Pieces.Count; piece++) solution.Add(new Move(piece, Dir.U));
            lesson.Solution = solution;
            return lesson;
        }

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
