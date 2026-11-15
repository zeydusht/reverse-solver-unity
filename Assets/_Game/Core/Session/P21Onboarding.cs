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
        // PM_67: the bomb lesson moved from 20 to 7; 8 and 9 are its soft follow-ups.
        public const int LockLesson = 3, ChainLesson = 10, WallLesson = 14, BombLesson = 7;
        public static bool IsSoftBomb(int slot) => slot == BombLesson + 1 || slot == BombLesson + 2;
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
            int last = Math.Max(Math.Max(ChainLesson, WallLesson), BombLesson + 2);
            for (int i = 0; i < Math.Min(last, levels.Count); i++)
            {
                int slot = i + 1;
                var level = levels[i];
                bool deferBomb = slot < BombLesson && (level.Bombs.Count > 0 || level.Intro?.Icon == "bomb");
                bool isolateLesson = slot == ChainLesson || slot == WallLesson;
                if (IsSoftBomb(slot) && level.Bombs.Count > 0)
                {
                    // One bomb, a fuse longer than the board: the second meeting with a bomb cannot be lost to it.
                    var soft = level.CopyNextVersion();
                    int keep = -1, fuse = -1;
                    foreach (var b in level.Bombs) if (b.Value > fuse) { keep = b.Key; fuse = b.Value; }
                    soft.Bombs = new Dictionary<int, int> { [keep] = Math.Max(fuse, level.Pieces.Count + 4) };
                    levels[i] = soft;
                    continue;
                }
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
                    lesson.Bombs = new Dictionary<int, int>();
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

        /* PM_68: the 11-20 wall. Calibrated to pt1 (FunSim Kolay, speed x1.2, bomb attention +.4, skill +.1; weighted
           MAE .112), estimate = pt1 real + (sim tuned - sim as played). Bomb fuses and timers only, boards untouched.
           S28 (4 bombs, 0/13 in pt1) leaves slot 19 for slot 39 as a hard level; S50 (one bomb) takes 19. */
        public static readonly (string Id, int Fuse, int Timer)[] SecondTenTuning =
            { ("S21", 3, 10), ("S13", 0, -15), ("Z04", 0, 20), ("S18", 3, 20), ("S24", 6, 0), ("S28", 4, 0) };
        public const string WallLevel = "S28", WallReplacement = "S50";
        public const int WallSlot = 19, WallNewSlot = 39;

        public static LevelSet TuneSecondTen(LevelSet original)
        {
            var levels = new List<LevelData>(original.Levels);
            int wall = levels.FindIndex(l => l.Id == WallLevel), swap = levels.FindIndex(l => l.Id == WallReplacement);
            if (wall != WallSlot - 1 || swap < 0 || levels.Count < WallNewSlot) return original;   // only the authored order
            var moved = levels[wall]; var incoming = levels[swap];
            levels.RemoveAt(Math.Max(wall, swap)); levels.RemoveAt(Math.Min(wall, swap));
            levels.Insert(WallSlot - 1, incoming.CopyNextVersion());
            levels.Insert(WallNewSlot - 1, moved);
            for (int i = 0; i < levels.Count; i++)
            {
                var l = levels[i];
                foreach (var t in SecondTenTuning)
                    if (l.Id == t.Id) { l = Tuned(l, t.Fuse, t.Timer); break; }
                if (l.Number != i + 1) { l = l.Clone(); l.Number = i + 1; }
                levels[i] = l;
            }
            return new LevelSet(original.Format, levels);
        }

        /* PM_70: slots 11-40 tuned on difficulty model v2 (docs/art/task29/ENTEGRASYON_RAPOR.md "PM_70"). Each entry
           replaces what it names on the PM_68 set: Timer (0 keeps), Bombs / Nails as "piece:count,..." (null keeps,
           "" clears), Hard (the "zor bölüm" card; null keeps). Boards are untouched. Next version for each. */
        public readonly struct Retune
        {
            public readonly string Id, Bombs, Nails; public readonly int Timer; public readonly bool? Hard;
            public Retune(string id, int timer = 0, string bombs = null, string nails = null, bool? hard = null)
            { Id = id; Timer = timer; Bombs = bombs; Nails = nails; Hard = hard; }
        }

        public static readonly Retune[] LateTuning =
        {
            new Retune("S34", 67, "6:24"),                                   // 13 normal (not hard: a tight bomb on an open board)
            new Retune("S13", 70, "10:15"),                                  // 15 one bomb, sec/piece 2.06 instead of 1.03
            new Retune("Z04", 67, "13:26"),                                  // 16
            new Retune("S18", 74),                                           // 17 sec/piece 2.24
            new Retune("S22", 67, "19:24"),                                  // 18 open board: fuse +3
            new Retune("S24", 81),                                           // 20 sec/piece 2.08
            new Retune("Z23", 115, "19:10,31:19", hard: true),               // 21 hard, 2 of 4 bombs
            new Retune("S19", 57),                                           // 22 breather
            new Retune("S27", 77, "3:3,23:5", "8:24,28:11", true),          // 23 hard, a lock (jams)
            new Retune("S14", 60),                                           // 24 breather
            new Retune("S35", 124, "18:36,19:36,24:35,25:32", hard: false),  // 25
            new Retune("S17", 57, "21:20"),                                  // 26
            new Retune("S10", 68),                                           // 27
            new Retune("S08", 0, null, "16:24,20:11,21:11"),                 // 30 a lock (jams)
            new Retune("S52", 46, "17:15", "19:24"),                         // 28 a lock (jams)
            new Retune("S37", 101, "20:21,25:53,35:48,37:45", hard: false),  // 29
            new Retune("S31", 92, "23:31,30:34,35:33"),                      // 31 3 of 4 bombs
            new Retune("S39", 98, "19:48,25:48,32:22,37:44"),                // 32
            new Retune("S32", 100, "26:26,31:32,36:25,37:26", "25:14,50:24", true), // 33 hard, a lock (jams)
            new Retune("S36", 85, "7:36,29:16,46:24", hard: false),          // 34
            new Retune("S29", 133, "7:8,24:22,47:18", hard: true),           // 35 hard, 3 of 4 bombs
            new Retune("Z25", 93, "25:29,31:31,32:30"),                      // 36 3 of 4 bombs
            new Retune("S38", 90, "8:34,29:22,30:8", hard: false),           // 37
            new Retune("S40", 106, "14:42,15:26"),                           // 38 2 of 3 bombs
            new Retune("S28", 119, "18:38,24:39,30:24", hard: false),        // 39 breather, 3 of 4 bombs
            new Retune("S33", 115, "8:10,10:33,38:29,47:46", hard: true),    // 40 hard, ~.35
        };

        /* PM_70 roles on 11-40 (the rest of 11-40 is normal; 1-10 and 14 are lessons / soft). */
        public static readonly int[] HardSlots22 = { 21, 23, 33, 35, 40 };
        public static readonly int[] BreatherSlots22 = { 22, 24, 39 };

        public static LevelSet TuneLate(LevelSet original)
        {
            var levels = new List<LevelData>(original.Levels);
            for (int i = 0; i < levels.Count; i++)
                foreach (var r in LateTuning)
                    if (levels[i].Id == r.Id) { levels[i] = Retuned(levels[i], r); break; }
            return new LevelSet(original.Format, levels);
        }

        /* PM_71: eight levels tuned again on the data-blended p_win1 (pt1's real results; ENTEGRASYON_RAPOR "PM_71").
           Same form as LateTuning, applied on PM_70's levels; boards untouched, next version. */
        public static readonly Retune[] DataTuning =
        {
            new Retune("S21", 0, "3:22"),                                    // 11 fuse +3: .53 -> .59
            new Retune("S17", 0, "21:16"),                                   // 26 fuse -4: .87 -> .61
            new Retune("S10", 0, "15:19"),                                   // 27 the bomb from 8 to 15 (a fuse cut alone breaks the open-board rule): .85 -> .63
            new Retune("S08", 63, "14:20"),                                  // 30 fuse +2, 70 -> 63 s: .52 -> .58
            new Retune("S39", 100, "19:41,25:48,32:22,37:44"),               // 32 bomb 19 -7, 98 -> 100 s: .66 -> .57
            new Retune("S32", 0, "26:26,31:32,36:25,37:24"),                 // 33 hard, bomb 37 -2: .47 -> .40
            new Retune("Z25", 0, "25:29,31:30,32:30"),                       // 36 bomb 31 -1: .67 -> .57
            new Retune("S38", 0, "8:32,29:17,30:9"),                         // 37 bomb 30 +1 (pt1's deaths), 29 -5, 8 -2: .39 -> .54
        };

        public static LevelSet TuneData(LevelSet original)
        {
            var levels = new List<LevelData>(original.Levels);
            for (int i = 0; i < levels.Count; i++)
                foreach (var r in DataTuning)
                    if (levels[i].Id == r.Id) { levels[i] = Retuned(levels[i], r); break; }
            return new LevelSet(original.Format, levels);
        }

        /* The p22 set: lessons, PM_68's 11-20, PM_70's 11-40, PM_71's data tuning. */
        public static LevelSet P22(LevelSet original) => TuneData(TuneLate(TuneSecondTen(SpaceObstacleLessons(original))));

        public static LevelData Retuned(LevelData level, Retune r)
        {
            var t = level.CopyNextVersion();
            if (r.Timer > 0) t.TimerSeconds = r.Timer;
            if (r.Bombs != null) t.Bombs = Counters(r.Bombs);
            if (r.Nails != null) t.Nails = Counters(r.Nails);
            if (r.Hard.HasValue) t.Hard = r.Hard.Value;
            return t;
        }

        public static Dictionary<int, int> Counters(string s)
        {
            var d = new Dictionary<int, int>();
            foreach (var kv in s.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var p = kv.Split(':');
                d[int.Parse(p[0].Trim())] = int.Parse(p[1].Trim());
            }
            return d;
        }

        /* PM_68: a gentler copy (next version): every bomb fuse +fuse, the timer +timer seconds (may be negative). */
        public static LevelData Tuned(LevelData level, int fuse, int timer)
        {
            var t = level.CopyNextVersion();
            if (fuse != 0)
            {
                var bombs = new Dictionary<int, int>();
                foreach (var b in level.Bombs) bombs[b.Key] = b.Value + fuse;
                t.Bombs = bombs;
            }
            t.TimerSeconds = Math.Max(20, level.TimerSeconds + timer);
            return t;
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
