using System;
using System.Collections.Generic;

namespace ReverseSolver.Core
{
    /* One attempt at one level: the web game's session rules (index.html,
       flyOut / tickNails / releaseValve / tickBombs / finish / boosters /
       startClock) as plain C#.

       State changes only through the commands below. Time arrives through
       Tick(dt) and randomness through IRandom, so a session replays exactly
       from (level, seed, commands, ticks).

       A successful removal runs, in the web game's order:
         remove -> nails tick -> safety valve -> fuses tick -> win check

       Deliberate differences from the web game (decided in PRODUCT.md):
         - DurationSeconds counts the clock seconds this session actually ran.
           The web game used timer - timeLeft, which the clock booster skewed.
         - Every attempt ends in exactly one record, quitting included. The
           web game never wrote its quit rows.

       Jams. The web game counts a jam when the player presses a nailed piece,
       or drags a piece beyond the point where it stops (once per drag and
       direction). Both are input decisions, so Presentation reports them
       through RecordJam; the session only counts them. */
    public sealed class GameSession
    {
        public const int ClockBonusSeconds = 20;
        const int WandAttempts = 200;

        public LevelData Level { get; }
        public Board Board { get; }
        public int Attempt { get; }
        public string Client { get; }

        public int TimeLeft { get; private set; }
        public int ElapsedSeconds { get; private set; }
        public int Moves { get; private set; }
        public int Jams { get; private set; }
        public int ValveCount { get; private set; }
        public Outcome Outcome { get; private set; }
        public bool IsOver => Outcome != Outcome.None;
        public AttemptRecord Record { get; private set; }

        public IReadOnlyList<GameEvent> Events => _events;
        public event Action<GameEvent> EventRaised;

        readonly IRandom _random;
        readonly Dictionary<int, int> _nails;
        readonly SortedDictionary<int, int> _bombs;   // ascending piece order, like JS integer keys
        readonly int[] _stock = new int[4];
        readonly int[] _used = new int[4];
        readonly List<GameEvent> _events = new List<GameEvent>();
        double _clockCarry;

        public GameSession(LevelData level, int attempt, string client, IRandom random)
        {
            Level = level ?? throw new ArgumentNullException(nameof(level));
            _random = random ?? throw new ArgumentNullException(nameof(random));
            Attempt = attempt;
            Client = client;
            Board = new Board(level);
            TimeLeft = level.TimerSeconds;
            _nails = new Dictionary<int, int>(level.Nails);
            _bombs = new SortedDictionary<int, int>();
            foreach (var b in level.Bombs) _bombs[b.Key] = b.Value;

            // The web game falls back to these when a level has no booster table.
            bool table = level.Boosters != null && level.Boosters.Count > 0;
            int[] fallback = { 5, 3, 7, 10 };
            foreach (var b in BoosterExt.All)
                _stock[(int)b] = table ? (level.Boosters.TryGetValue(b.Id(), out int n) ? n : 0) : fallback[(int)b];
        }

        GameSession(GameSession o)
        {
            Level = o.Level; Attempt = o.Attempt; Client = o.Client; _random = o._random;
            Board = o.Board.Clone();
            TimeLeft = o.TimeLeft; ElapsedSeconds = o.ElapsedSeconds; Moves = o.Moves; Jams = o.Jams;
            ValveCount = o.ValveCount; Outcome = o.Outcome; Record = o.Record;
            _nails = new Dictionary<int, int>(o._nails);
            _bombs = new SortedDictionary<int, int>(o._bombs);
            Array.Copy(o._stock, _stock, 4);
            Array.Copy(o._used, _used, 4);
            _events.AddRange(o._events);
            _clockCarry = o._clockCarry;
        }

        /* A branch for search (solvers, Monte Carlo). Shares the random source,
           so only branch where no further draws matter, or pass a fresh one. */
        internal GameSession Clone() => new GameSession(this);

        // ---- queries ----------------------------------------------------------------

        public int NailAt(int piece) => _nails.TryGetValue(piece, out int n) ? n : 0;
        public int FuseAt(int piece) => _bombs.TryGetValue(piece, out int n) ? n : 0;
        public IReadOnlyDictionary<int, int> Nails => _nails;
        public IReadOnlyDictionary<int, int> Bombs => _bombs;
        public int Stock(Booster b) => _stock[(int)b];
        public int Used(Booster b) => _used[(int)b];

        /* Locked while the level number is below the booster's unlock level. */
        public bool IsUnlocked(Booster b) =>
            Level.Unlock == null || !Level.Unlock.TryGetValue(b.Id(), out int at) || Level.Number >= at;

        public TravelLimit Probe(int piece, Dir dir) => TravelRule.Limit(Board, piece, dir);

        /* 3 stars with >= 45% of the timer left, 2 with >= 20%, else 1 (web starsFor). */
        public int Stars => Outcome != Outcome.Win ? 0 : StarsFor(TimeLeft, Level.TimerSeconds);

        public static int StarsFor(int timeLeft, int timer)
        {
            if (timer <= 0) return 3;
            double frac = (double)timeLeft / timer;
            return frac >= .45 ? 3 : frac >= .2 ? 2 : 1;
        }

        // ---- commands ---------------------------------------------------------------

        /* A drag-out that the player released past the exit point. */
        public CommandResult TryExit(int piece, Dir dir)
        {
            if (IsOver) return CommandResult.GameOver;
            if (!IsPiece(piece)) return CommandResult.NoSuchPiece;
            if (NailAt(piece) > 0) return CommandResult.Pinned;
            if (!TravelRule.Limit(Board, piece, dir).Exit) return CommandResult.Blocked;
            Remove(piece, dir, byHammer: false);
            return CommandResult.Ok;
        }

        public CommandResult RecordJam(int piece)
        {
            if (IsOver) return CommandResult.GameOver;
            if (!IsPiece(piece)) return CommandResult.NoSuchPiece;
            Jams++;
            Raise(GameEvent.Jam(piece));
            return CommandResult.Ok;
        }

        /* Removes any present piece, nailed or carrying a bomb, and counts as a move. */
        public CommandResult UseHammer(int piece)
        {
            var check = CanUse(Booster.Hammer);
            if (check != CommandResult.Ok) return check;
            if (!IsPiece(piece)) return CommandResult.NoSuchPiece;
            Spend(Booster.Hammer);
            Remove(piece, Dir.U, byHammer: true);
            return CommandResult.Ok;
        }

        /* Flattens one tabbed joint between two different present pieces.
           Vertical: the joint left of cell (x, y). Horizontal: the joint above it. */
        public CommandResult UseScissors(int x, int y, bool vertical)
        {
            var check = CanUse(Booster.Scissors);
            if (check != CommandResult.Ok) return check;
            if (!IsCuttable(x, y, vertical)) return CommandResult.NotAJoint;
            if (vertical) Board.SetVJoint(x, y, 0); else Board.SetHJoint(x, y, 0);
            Spend(Booster.Scissors);
            Raise(GameEvent.Cut(x, y, vertical));
            return CommandResult.Ok;
        }

        /* The joints the scissors can cut right now (web showJoints). */
        public bool IsCuttable(int x, int y, bool vertical)
        {
            var l = Level;
            if (vertical)
            {
                if (x < 1 || x >= l.Width || y < 0 || y >= l.Height || Board.VJoint(x, y) == 0) return false;
                int a = Board.PieceAt(new Cell(x - 1, y)), b = Board.PieceAt(new Cell(x, y));
                return a >= 0 && b >= 0 && a != b;
            }
            if (y < 1 || y >= l.Height || x < 0 || x >= l.Width || Board.HJoint(x, y) == 0) return false;
            int above = Board.PieceAt(new Cell(x, y - 1)), below = Board.PieceAt(new Cell(x, y));
            return above >= 0 && below >= 0 && above != below;
        }

        /* Re-decides which side owns every tab, keeping the board solvable. Flat
           joints stay flat. Up to 200 tries; if none is solvable the joints are
           restored and nothing is spent (web useWand). */
        public CommandResult UseWand()
        {
            var check = CanUse(Booster.Wand);
            if (check != CommandResult.Ok) return check;
            var l = Level;
            var v0 = new int[l.Height, l.Width + 1];
            var h0 = new int[l.Height + 1, l.Width];
            for (int y = 0; y < l.Height; y++) for (int x = 1; x < l.Width; x++) v0[y, x] = Board.VJoint(x, y);
            for (int y = 1; y < l.Height; y++) for (int x = 0; x < l.Width; x++) h0[y, x] = Board.HJoint(x, y);

            for (int t = 0; t < WandAttempts; t++)
            {
                // Same draw order as the web game: vertical joints row by row, then horizontal.
                for (int y = 0; y < l.Height; y++)
                    for (int x = 1; x < l.Width; x++)
                        if (v0[y, x] != 0) Board.SetVJoint(x, y, _random.NextDouble() < .5 ? 1 : -1);
                for (int y = 1; y < l.Height; y++)
                    for (int x = 0; x < l.Width; x++)
                        if (h0[y, x] != 0) Board.SetHJoint(x, y, _random.NextDouble() < .5 ? 1 : -1);

                if (GreedySolver.Solve(Board).Solved)
                {
                    Spend(Booster.Wand);
                    Raise(GameEvent.Simple(EventKind.JointsReshuffled));
                    return CommandResult.Ok;
                }
            }
            for (int y = 0; y < l.Height; y++) for (int x = 1; x < l.Width; x++) Board.SetVJoint(x, y, v0[y, x]);
            for (int y = 1; y < l.Height; y++) for (int x = 0; x < l.Width; x++) Board.SetHJoint(x, y, h0[y, x]);
            return CommandResult.NoArrangement;
        }

        public CommandResult UseClock()
        {
            var check = CanUse(Booster.Clock);
            if (check != CommandResult.Ok) return check;
            Spend(Booster.Clock);
            TimeLeft += ClockBonusSeconds;
            Raise(GameEvent.Time(EventKind.TimeAdded, ClockBonusSeconds));
            return CommandResult.Ok;
        }

        /* Advances the countdown. Only call while the level is actually being
           played (not during an intro or with the app hidden): the record's
           duration is the seconds counted here. */
        public void Tick(double dt)
        {
            if (IsOver || dt <= 0) return;
            _clockCarry += dt;
            while (_clockCarry >= 1 - 1e-9 && !IsOver)
            {
                _clockCarry -= 1;
                TimeLeft--;
                ElapsedSeconds++;
                Raise(GameEvent.Time(EventKind.TimeTick, TimeLeft));
                if (TimeLeft <= 0) Finish(Outcome.Time);
            }
        }

        /* Restart, back to the menu, or another level: the attempt ends without a result. */
        public CommandResult Quit()
        {
            if (IsOver) return CommandResult.GameOver;
            Finish(Outcome.Quit);
            return CommandResult.Ok;
        }

        // ---- rules --------------------------------------------------------------------

        void Remove(int piece, Dir dir, bool byHammer)
        {
            Board.Remove(piece);
            Moves++;
            Raise(GameEvent.Removed(piece, dir, byHammer));
            TickNails();
            ReleaseValve();
            TickBombs();
            if (!IsOver && Board.PresentCount == 0) Finish(Outcome.Win);
        }

        void TickNails()
        {
            if (_nails.Count == 0) return;
            var keys = new List<int>(_nails.Keys);
            keys.Sort();
            bool any = false;
            foreach (var k in keys)
            {
                if (_nails[k] <= 0) continue;
                _nails[k]--;
                any = true;
                if (_nails[k] == 0 && Board.IsPresent(k)) Raise(GameEvent.ForPiece(EventKind.NailFreed, k));
            }
            if (any) Raise(GameEvent.Simple(EventKind.NailsTicked));
        }

        /* Counters only tick on a move, so if every free piece is still nailed
           the board would be dead. Pop the one closest to release; the lowest
           index wins a tie (web releaseValve). */
        void ReleaseValve()
        {
            var free = TravelRule.FreePieces(Board);
            if (free.Count == 0) return;
            int pick = -1, low = int.MaxValue;
            foreach (var p in free)
            {
                int n = NailAt(p);
                if (n <= 0) return;
                if (n < low) { low = n; pick = p; }
            }
            _nails[pick] = 0;
            ValveCount++;
            Raise(GameEvent.ForPiece(EventKind.ValveReleased, pick));
        }

        /* A fuse burns one step per move; a bomb whose piece has left is gone.
           Any fuse at zero ends the level (web tickBombs). */
        void TickBombs()
        {
            if (_bombs.Count == 0) return;
            bool blown = false, ticked = false;
            foreach (var k in new List<int>(_bombs.Keys))
            {
                if (!Board.IsPresent(k))
                {
                    _bombs.Remove(k);
                    Raise(GameEvent.ForPiece(EventKind.BombDefused, k));
                    continue;
                }
                _bombs[k]--;
                ticked = true;
                if (_bombs[k] <= 0) blown = true;
            }
            if (ticked) Raise(GameEvent.Simple(EventKind.BombsTicked));
            if (blown) Finish(Outcome.Bomb);
        }

        CommandResult CanUse(Booster b)
        {
            if (IsOver) return CommandResult.GameOver;
            if (!IsUnlocked(b)) return CommandResult.Locked;
            if (_stock[(int)b] <= 0) return CommandResult.OutOfStock;
            return CommandResult.Ok;
        }

        void Spend(Booster b)
        {
            _stock[(int)b]--;
            _used[(int)b]++;
            Raise(GameEvent.Used(b));
        }

        bool IsPiece(int piece) => piece >= 0 && piece < Level.Pieces.Count && Board.IsPresent(piece);

        void Finish(Outcome outcome)
        {
            if (IsOver) return;
            Outcome = outcome;
            Record = new AttemptRecord
            {
                LevelId = Level.Id,
                LevelVersion = Level.Version,
                LevelNumber = Level.Number,
                Client = Client,
                Attempt = Attempt,
                Result = outcome.Id(),
                DurationSeconds = ElapsedSeconds,
                LeftSeconds = Math.Max(0, TimeLeft),
                Moves = Moves,
                Jams = Jams,
                Stars = Stars,
                Load = Level.Load,
                Scissors = _used[(int)Booster.Scissors],
                Wand = _used[(int)Booster.Wand],
                Hammer = _used[(int)Booster.Hammer],
                Clock = _used[(int)Booster.Clock]
            };
            Raise(GameEvent.End(outcome));
        }

        void Raise(GameEvent e)
        {
            _events.Add(e);
            EventRaised?.Invoke(e);
        }
    }
}
