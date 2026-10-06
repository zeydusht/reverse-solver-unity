using System.Collections.Generic;

namespace ReverseSolver.Core
{
    public enum SmartVerdict
    {
        Won,            // cleared the board
        LosesBomb,      // the (restricted) search found no order that defuses a bomb in time
                        // or a bomb went off; not a proof over every move order
        Unverified,     // search budget ran out before a bomb could be settled
        Frozen          // pieces left, none can leave and the valve did not help
    }

    public sealed class SmartPlayResult
    {
        public string LevelId;
        public SmartVerdict Verdict;
        public Outcome Outcome;
        public readonly List<Move> Moves = new List<Move>();
        public int Valves;
        public int Nodes;           // search nodes used
        public string Note;

        public override string ToString() =>
            $"{LevelId} {Verdict} moves={Moves.Count} valves={Valves} nodes={Nodes}{(Note != null ? " " + Note : "")}";
    }

    /* Plays a level the way a careful player would, to check the web README's
       guarantee (every level clearable with 0 bombs going off, 0 freezes, 0
       safety-valve pulls). It never uses boosters and obeys nails.

       While a bomb is on the board it targets the one with the shortest fuse:
       iterative deepening over moves of the pieces that block the bomb
       (transitively), plus one filler move to let nail counters run down, for
       the shortest sequence that takes the bomb piece out before its fuse ends.
       Without bombs it removes the lowest-index free, un-nailed piece; removal
       never blocks anything, so that cannot lose. */
    public static class SmartPlayer
    {
        public const int DefaultNodeBudget = 300_000;

        public static SmartPlayResult Play(LevelData level, int nodeBudget = DefaultNodeBudget)
        {
            var r = new SmartPlayResult { LevelId = level.Id };
            var s = new GameSession(level, 1, "solver", new Mulberry32(1));
            int budget = nodeBudget;

            while (!s.IsOver)
            {
                int target = ShortestFuse(s);
                if (target >= 0)
                {
                    // Cheap rule first; search only when it would let the bomb go off.
                    var plan = Direct(s, target) ?? Defuse(s, target, ref budget, out bool exhausted);
                    exhausted = budget <= 0;
                    r.Nodes = nodeBudget - budget;
                    if (plan == null)
                    {
                        r.Verdict = exhausted ? SmartVerdict.Unverified : SmartVerdict.LosesBomb;
                        r.Note = $"bomb on piece {target}, fuse {s.FuseAt(target)}";
                        Finish(s, r);
                        return r;
                    }
                    foreach (var m in plan) Apply(s, m, r);
                    continue;
                }

                int next = FirstMovable(s);
                if (next < 0)
                {
                    r.Verdict = SmartVerdict.Frozen;
                    Finish(s, r);
                    return r;
                }
                Apply(s, new Move(next, TravelRule.ExitDirs(s.Board, next)[0]), r);
            }
            r.Nodes = nodeBudget - budget;
            r.Verdict = s.Outcome == Outcome.Win ? SmartVerdict.Won : SmartVerdict.LosesBomb;
            Finish(s, r);
            return r;
        }

        static void Finish(GameSession s, SmartPlayResult r)
        {
            r.Outcome = s.Outcome;
            r.Valves = s.ValveCount;
        }

        static void Apply(GameSession s, Move m, SmartPlayResult r)
        {
            var res = s.TryExit(m.Piece, m.Dir);
            if (res != CommandResult.Ok) throw new System.InvalidOperationException($"{s.Level.Id}: planned {m} refused: {res}");
            r.Moves.Add(m);
        }

        static int ShortestFuse(GameSession s)
        {
            int best = -1, fuse = int.MaxValue;
            foreach (var b in s.Bombs)
                if (s.Board.IsPresent(b.Key) && b.Value < fuse) { fuse = b.Value; best = b.Key; }
            return best;
        }

        static bool Movable(GameSession s, int p) => s.Board.IsPresent(p) && s.NailAt(p) <= 0 && TravelRule.CanExit(s.Board, p);

        static int FirstMovable(GameSession s)
        {
            foreach (var p in s.Board.PresentPieces())
                if (Movable(s, p)) return p;
            return -1;
        }

        // ---- bomb search ----------------------------------------------------------

        /* The obvious way in: take the bomb piece if it can leave, else a movable
           piece that blocks it, else the lowest movable piece. Returns the moves if
           that removes the bomb before it goes off, null otherwise. */
        static List<Move> Direct(GameSession start, int target)
        {
            var s = start.Clone();
            var moves = new List<Move>();
            while (!s.IsOver && s.Board.IsPresent(target))
            {
                int pick = -1;
                if (Movable(s, target)) pick = target;
                else
                {
                    foreach (var d in DirExt.All)
                    {
                        int b = TravelRule.Limit(s.Board, target, d).Blocker;
                        if (b >= 0 && Movable(s, b)) { pick = b; break; }
                    }
                    if (pick < 0) pick = FirstMovable(s);
                }
                if (pick < 0) return null;
                var m = new Move(pick, TravelRule.ExitDirs(s.Board, pick)[0]);
                s.TryExit(m.Piece, m.Dir);
                moves.Add(m);
            }
            return s.Board.IsPresent(target) || s.Outcome == Outcome.Bomb ? null : moves;
        }

        /* Shortest move list ending with the target's removal, at most `fuse`
           moves long (the target may leave on the move its fuse would hit 0:
           a bomb whose piece is gone is dropped before fuses tick). */
        static List<Move> Defuse(GameSession start, int target, ref int budget, out bool exhausted)
        {
            exhausted = false;
            int fuse = start.FuseAt(target);
            for (int depth = 1; depth <= fuse; depth++)
            {
                var path = new List<Move>();
                var seen = new HashSet<(ulong, int)>();
                var found = Search(start, target, depth, path, seen, ref budget);
                if (found) return path;
                if (budget <= 0) { exhausted = true; return null; }
            }
            return null;
        }

        static bool Search(GameSession s, int target, int left, List<Move> path, HashSet<(ulong, int)> seen, ref int budget)
        {
            if (--budget <= 0) return false;
            if (Movable(s, target))
            {
                var m = new Move(target, TravelRule.ExitDirs(s.Board, target)[0]);
                var c = s.Clone();
                c.TryExit(m.Piece, m.Dir);
                if (c.Outcome != Outcome.Bomb) { path.Add(m); return true; }
            }
            if (left <= 1) return false;
            if (!seen.Add((Mask(s), left))) return false;

            foreach (var p in Candidates(s, target))
            {
                var m = new Move(p, TravelRule.ExitDirs(s.Board, p)[0]);
                var c = s.Clone();
                c.TryExit(m.Piece, m.Dir);
                if (c.IsOver) continue;                       // another bomb went off
                path.Add(m);
                if (Search(c, target, left - 1, path, seen, ref budget)) return true;
                path.RemoveAt(path.Count - 1);
                if (budget <= 0) return false;
            }
            return false;
        }

        /* Movable pieces in the target's blocking cone first (nearest first),
           then one filler move so nail counters can run down. */
        static List<int> Candidates(GameSession s, int target)
        {
            var cone = new List<int>();
            var inCone = new HashSet<int> { target };
            var frontier = new Queue<int>();
            frontier.Enqueue(target);
            while (frontier.Count > 0)
            {
                int q = frontier.Dequeue();
                foreach (var d in DirExt.All)
                {
                    int b = TravelRule.Limit(s.Board, q, d).Blocker;
                    if (b >= 0 && inCone.Add(b)) { cone.Add(b); frontier.Enqueue(b); }
                }
            }
            var result = new List<int>();
            foreach (var p in cone) if (Movable(s, p)) result.Add(p);
            foreach (var p in s.Board.PresentPieces())
                if (!inCone.Contains(p) && Movable(s, p)) { result.Add(p); break; }
            return result;
        }

        static ulong Mask(GameSession s)
        {
            ulong m = 0;
            foreach (var p in s.Board.PresentPieces()) m |= 1UL << (p & 63);
            return m;
        }
    }
}
