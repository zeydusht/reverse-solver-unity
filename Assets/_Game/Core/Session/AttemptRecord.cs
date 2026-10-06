using System;
using System.Collections.Generic;

namespace ReverseSolver.Core
{
    /* One finished attempt, the row a playtest produces. Field meanings
       (PRODUCT.md decision log, 2026-10-05):
         Attempt          every start of a level counts, restarts included
         Result           win / time / bomb / quit; quit = ended without a result
         DurationSeconds  clock seconds the session counted (active play only)
         LeftSeconds      seconds on the clock at the end, never negative
         Jams             presses on a nailed piece + drags pushed past the stop
         Stars            0 unless won; 3/2/1 by share of the timer left
       Player and session ids are added by the platform layer when sending (M5). */
    public sealed class AttemptRecord
    {
        public string LevelId;
        public int LevelVersion;
        public int LevelNumber;
        public string Client;
        public int Attempt;
        public string Result;
        public int DurationSeconds;
        public int LeftSeconds;
        public int Moves;
        public int Jams;
        public int Stars;
        public int Load;
        public int Scissors, Wand, Hammer, Clock;

        public override string ToString() =>
            $"{LevelId} v{LevelVersion} #{Attempt} {Result} dur={DurationSeconds} left={LeftSeconds} " +
            $"moves={Moves} jams={Jams} stars={Stars} b={Scissors}/{Wand}/{Hammer}/{Clock} client={Client}";
    }

    /* In-memory attempt numbering and best stars per level. Persisting it is
       the platform's job (M5); this keeps the counting rule in one place. */
    public sealed class AttemptLog
    {
        readonly Dictionary<string, int> _attempts = new Dictionary<string, int>();
        readonly Dictionary<string, int> _best = new Dictionary<string, int>();
        readonly List<AttemptRecord> _records = new List<AttemptRecord>();

        public IReadOnlyList<AttemptRecord> Records => _records;

        /* Attempt number for the next start of this level, and counts it as started. */
        public int Begin(string levelId)
        {
            _attempts.TryGetValue(levelId, out int n);
            _attempts[levelId] = ++n;
            return n;
        }

        public GameSession Start(LevelData level, string client, IRandom random) =>
            new GameSession(level, Begin(level.Id), client, random);

        public void Add(AttemptRecord record)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));
            _records.Add(record);
            _best.TryGetValue(record.LevelId, out int best);
            if (record.Stars > best) _best[record.LevelId] = record.Stars;
        }

        public int AttemptsOf(string levelId) => _attempts.TryGetValue(levelId, out int n) ? n : 0;
        public int BestStars(string levelId) => _best.TryGetValue(levelId, out int s) ? s : 0;
    }
}
