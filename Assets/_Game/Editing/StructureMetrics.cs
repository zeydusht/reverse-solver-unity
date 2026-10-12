using System;
using System.Collections.Generic;
using ReverseSolver.Core;

namespace ReverseSolver.Editing
{
    /* Faz 2 candidate structural measures (PRODUCT.md, 2026-10-08 22:37): what a
       level asks of a human beyond bomb pressure, which the random player
       cannot see (it never tries a wrong piece). Exact, not sampled: computed
       along the solver's path (SmartPlayer, the 40/40 player), on the state
       before each move. Editor only.

         blind tries   if a player tried remaining pieces x 4 directions at
                       random, the expected wrong tries before a valid exit is
                       options / valid - 1 (sampling with replacement); summed
                       over the path and averaged per exit. Valid = un-nailed
                       piece, direction that leaves.
         freedom       leavable un-nailed pieces / pieces left: at the start,
                       and averaged over the path.
         bottleneck    steps with 10+ pieces left where exactly one could leave
                       (Bottlenecks10); the fewest leavable pieces seen on the path.
         nailed share  nailed pieces / pieces at the start.
         fuse margin   for each bomb: its fuse minus the fewest moves that take
                       its piece out from the start (search over the pieces
                       that block it, as SmartPlayer does); the smallest over
                       the bombs. NaN without bombs. When the exact search
                       runs out of budget (a bomb behind many pieces: L26, L30,
                       L34), the dig SmartPlayer itself plays is used instead:
                       an upper bound on the shortest dig, so the margin is a
                       cautious lower bound and FuseMarginExact is false. */
    public sealed class StructureMetrics
    {
        public string LevelId;
        public int Steps;
        public double BlindTriesTotal, BlindTriesPerExit;
        public double FreedomStart, FreedomMean;
        public int FewestChoices;
        public double NailedShare;
        public double FuseMargin = double.NaN;
        public bool FuseMarginExact = true;

        /* Difficulty model inputs (PRODUCT.md "Zorluk modeli"), along the same path:
             FreeAtStart   k0, pieces that can leave on the first move
             ReadingLoad   sum over moves of (n + 1) / (k + 1), n pieces left, k of them leavable
             ReadingPerMove ReadingLoad / pieces ("hamle başına okuma")
             Bottlenecks10 moves with 10 or more pieces left and only one that can leave
             BombDigs      per bomb piece: fewest moves that take it out (exact, or SmartPlayer's dig) */
        public int FreeAtStart;
        public double ReadingLoad, ReadingPerMove;
        public int Bottlenecks10;
        public readonly Dictionary<int, (int dig, bool exact)> BombDigs = new Dictionary<int, (int, bool)>();

        public const int FuseSearchBudget = 20_000;

        public static StructureMetrics Of(LevelData level)
        {
            var m = new StructureMetrics { LevelId = level.Id, FewestChoices = int.MaxValue };
            var path = SmartPlayer.Play(level).Moves;
            var s = new GameSession(level, 1, "structure", new Mulberry32(1));
            m.NailedShare = level.Pieces.Count == 0 ? 0 : (double)level.Nails.Count / level.Pieces.Count;
            // the dig is searched with every fuse long, so it is found even when the level's fuse is too short
            var longFuses = LevelDraft.FromLevel(level);
            foreach (var b in level.Bombs) longFuses.SetBomb(level.Pieces[b.Key][0], LevelDraft.MaxCount);
            var digStart = new GameSession(longFuses.ToLevelData(), 1, "structure", new Mulberry32(1));
            m.FuseMargin = FuseMarginOf(digStart, level.Bombs, m.BombDigs, out m.FuseMarginExact);

            double freedomSum = 0;
            foreach (var move in path)
            {
                if (s.IsOver) break;
                int left = s.Board.PresentCount, validPieces = 0, validPairs = 0;
                foreach (var p in s.Board.PresentPieces())
                {
                    if (s.NailAt(p) > 0) continue;
                    int dirs = TravelRule.ExitDirs(s.Board, p).Count;
                    if (dirs > 0) { validPieces++; validPairs += dirs; }
                }
                if (validPairs > 0) m.BlindTriesTotal += 4.0 * left / validPairs - 1;
                double freedom = left == 0 ? 0 : (double)validPieces / left;
                if (m.Steps == 0) { m.FreedomStart = freedom; m.FreeAtStart = validPieces; }
                m.ReadingLoad += (left + 1.0) / (validPieces + 1.0);
                if (left >= 10 && validPieces == 1) m.Bottlenecks10++;
                freedomSum += freedom;
                m.FewestChoices = Math.Min(m.FewestChoices, validPieces);
                m.Steps++;
                s.TryExit(move.Piece, move.Dir);
            }
            if (m.Steps > 0)
            {
                m.BlindTriesPerExit = m.BlindTriesTotal / m.Steps;
                m.FreedomMean = freedomSum / m.Steps;
            }
            if (m.FewestChoices == int.MaxValue) m.FewestChoices = 0;
            m.ReadingPerMove = level.Pieces.Count == 0 ? 0 : m.ReadingLoad / level.Pieces.Count;
            return m;
        }

        /* start: the level with every fuse long; fuses: the level's own fuses. */
        static double FuseMarginOf(GameSession start, IReadOnlyDictionary<int, int> fuses, Dictionary<int, (int, bool)> digs, out bool exact)
        {
            double best = double.NaN;
            exact = true;
            foreach (var b in fuses)
            {
                int budget = FuseSearchBudget;
                bool thisExact = true;
                int moves = FewestMovesToRemove(start, b.Key, start.Board.PresentCount, ref budget);
                if (moves < 0 && budget <= 0)
                {
                    moves = DirectDig(start, b.Key);
                    if (moves >= 0) { exact = false; thisExact = false; }
                }
                if (moves < 0) continue;
                digs[b.Key] = (moves, thisExact);
                double margin = b.Value - moves;
                if (double.IsNaN(best) || margin < best) best = margin;
            }
            return best;
        }

        /* SmartPlayer's direct dig: the bomb piece if it can go, else a movable
           piece that blocks it, else the lowest movable piece. The moves it
           takes to get the bomb piece out, or -1 if a bomb goes off first. */
        static int DirectDig(GameSession start, int target)
        {
            var s = start.Clone();
            int moves = 0;
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
                    if (pick < 0)
                        foreach (var p in s.Board.PresentPieces())
                            if (Movable(s, p)) { pick = p; break; }
                }
                if (pick < 0) return -1;
                s.TryExit(pick, TravelRule.ExitDirs(s.Board, pick)[0]);
                moves++;
            }
            return s.Board.IsPresent(target) || s.Outcome == Outcome.Bomb ? -1 : moves;
        }

        /* Iterative deepening up to maxMoves: the moves (the target's own last)
           that take `target` out; -1 if none found within the fuse or budget. */
        static int FewestMovesToRemove(GameSession start, int target, int maxMoves, ref int budget)
        {
            for (int depth = 1; depth <= maxMoves; depth++)
            {
                var seen = new HashSet<(ulong, int)>();
                if (Reach(start, target, depth, seen, ref budget)) return depth;
                if (budget <= 0) return -1;
            }
            return -1;
        }

        /* seen: (pieces left, moves left) states already shown to fail at this depth. */
        static bool Reach(GameSession s, int target, int left, HashSet<(ulong, int)> seen, ref int budget)
        {
            if (--budget <= 0) return false;
            if (Movable(s, target)) return true;                    // its own move is the last one
            if (left <= 1) return false;
            if (!seen.Add((Mask(s), left))) return false;
            foreach (var p in Blockers(s, target))
            {
                var c = s.Clone();
                c.TryExit(p, TravelRule.ExitDirs(c.Board, p)[0]);
                if (!c.Board.IsPresent(target) && !c.IsOver) return true;
                if (c.IsOver) continue;
                if (Reach(c, target, left - 1, seen, ref budget)) return true;
                if (budget <= 0) return false;
            }
            return false;
        }

        static ulong Mask(GameSession s)
        {
            ulong m = 0;
            foreach (var p in s.Board.PresentPieces()) m |= 1UL << (p & 63);
            return m;
        }

        static bool Movable(GameSession s, int p) => s.Board.IsPresent(p) && s.NailAt(p) <= 0 && TravelRule.CanExit(s.Board, p);

        /* Movable pieces in the target's blocking cone, then one filler move
           so nail counters can run down (SmartPlayer.Candidates). */
        static List<int> Blockers(GameSession s, int target)
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
    }
}
