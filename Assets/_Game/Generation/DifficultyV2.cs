using System;
using System.Collections.Generic;
using System.Linq;
using ReverseSolver.Core;

namespace ReverseSolver.Generation
{
    /* PM_69: difficulty features v2, measured with the engine's own rules (TravelRule, nails, valve, bomb fuses).
       Removing a piece never blocks another, so for each piece and direction the set of pieces in its corridor
       (occupying a cell it passes or catching a side) is fixed by the board: the piece leaves that way exactly when
       that set is gone and the lane is not walled. Obstacles holds those sets; FastState plays on them (tested
       against GameSession in DifficultyV2Tests). Boosters change joints and are not modelled here. */
    public sealed class Obstacles
    {
        public readonly LevelData Level;
        public readonly int N;
        public readonly int[][][] Obs;          // [piece][dir] corridor pieces
        public readonly bool[,] Walled;          // [piece, dir] a wall segment stops it that way
        public readonly int[,] Dist;             // [piece, dir] cells to travel before it is off (when not walled)
        public readonly List<(int piece, int dir)>[] Rev;
        public readonly bool[] Chain;
        public readonly bool[] TouchesWall;      // a cell on a rim side whose lane is walled
        public readonly int[] Fuse0, Nail0;      // 0 when none

        public Obstacles(LevelData l) : this(new Board(l)) { }

        /* From a board in play (joints may have been cut or reshuffled): removed pieces get empty sets. */
        public Obstacles(Board current)
        {
            var l = current.Level;
            Level = l; N = l.Pieces.Count;
            Obs = new int[N][][]; Walled = new bool[N, 4]; Dist = new int[N, 4];
            Rev = new List<(int, int)>[N]; Chain = new bool[N]; TouchesWall = new bool[N];
            Fuse0 = new int[N]; Nail0 = new int[N];
            for (int i = 0; i < N; i++) Rev[i] = new List<(int, int)>();
            for (int p = 0; p < N; p++)
            {
                Obs[p] = new int[4][];
                Chain[p] = l.Pieces[p].Length > 1;
                for (int d = 0; d < 4; d++)
                {
                    if (!current.IsPresent(p)) { Obs[p][d] = new int[0]; continue; }
                    var b = current.Clone();
                    var list = new List<int>();
                    while (true)
                    {
                        var lim = TravelRule.Limit(b, p, (Dir)d);
                        if (lim.Exit) { Dist[p, d] = lim.Cells + 1; break; }
                        if (lim.Blocker < 0) { Walled[p, d] = true; break; }
                        list.Add(lim.Blocker); b.Remove(lim.Blocker);
                    }
                    Obs[p][d] = list.ToArray();
                    foreach (int q in list) Rev[q].Add((p, d));
                }
                foreach (var c in l.Pieces[p])
                    foreach (var side in DirExt.All)
                    {
                        if (l.OnBoard(c.Step(side))) continue;
                        if (l.Sealed.TryGetValue(side, out var lanes) && Array.IndexOf(lanes, side.IsVertical() ? c.X : c.Y) >= 0) TouchesWall[p] = true;
                    }
            }
            foreach (var kv in l.Bombs) Fuse0[kv.Key] = kv.Value;
            foreach (var kv in l.Nails) Nail0[kv.Key] = kv.Value;
        }

        public bool LaneSealed(Cell c, Dir side) =>
            Level.Sealed.TryGetValue(side, out var lanes) && Array.IndexOf(lanes, side.IsVertical() ? c.X : c.Y) >= 0;
    }

    public enum Hold { None, Lock, Chain, Wall, Piece, Tab }

    /* Session state on the obstacle sets: presence, nail and fuse counters, the release valve (GameSession order:
       remove, tick nails, valve, tick bombs). Explosions are recorded, not enforced: a natural player keeps going so
       we also learn when it would have taken the bomb piece. */
    public sealed class FastState
    {
        public readonly Obstacles O;
        public readonly bool[] Present;
        readonly int[,] _rem;
        public readonly int[] Nail, Fuse;
        public Board Board { get; private set; }  // kept in sync for the reason queries
        public int Exits, PresentCount, FirstBlow = -1;
        public readonly int[] RemovedAt;        // exit index (1-based), 0 while present

        public FastState(Obstacles o)
        {
            O = o; int n = o.N;
            Present = Enumerable.Repeat(true, n).ToArray();
            _rem = new int[n, 4];
            for (int p = 0; p < n; p++) for (int d = 0; d < 4; d++) _rem[p, d] = o.Obs[p][d].Length;
            Nail = (int[])o.Nail0.Clone(); Fuse = (int[])o.Fuse0.Clone();
            Board = new Board(o.Level);
            PresentCount = n; RemovedAt = new int[n];
        }

        /* A session's current state: presence and joints from its board, nail and fuse counters as given. */
        public FastState(Obstacles o, Board current, Func<int, int> nail, Func<int, int> fuse) : this(o)
        {
            Board = current.Clone();
            for (int p = 0; p < o.N; p++)
            {
                Nail[p] = nail(p); Fuse[p] = current.IsPresent(p) ? fuse(p) : 0;
                if (current.IsPresent(p)) continue;
                Present[p] = false; PresentCount--;
                foreach (var (q, d) in o.Rev[p]) _rem[q, d]--;
            }
        }

        public int Rem(int p, int d) => _rem[p, d];
        public bool GeomFree(int p)
        {
            for (int d = 0; d < 4; d++) if (!O.Walled[p, d] && _rem[p, d] == 0) return true;
            return false;
        }
        public bool ExitsWay(int p, int d) => !O.Walled[p, d] && _rem[p, d] == 0;
        public bool Available(int p) => Present[p] && Nail[p] <= 0 && GeomFree(p);
        public bool IsBomb(int p) => O.Fuse0[p] > 0 || Fuse[p] > 0;
        public bool BombAlive(int p) => IsBomb(p) && Present[p];

        public List<int> AvailableList()
        {
            var a = new List<int>();
            for (int p = 0; p < O.N; p++) if (Available(p)) a.Add(p);
            return a;
        }

        public void Remove(int q)
        {
            Present[q] = false; PresentCount--; Exits++; RemovedAt[q] = Exits;
            Board.Remove(q);
            foreach (var (p, d) in O.Rev[q]) _rem[p, d]--;
            for (int p = 0; p < O.N; p++) if (Nail[p] > 0) Nail[p]--;
            // release valve: every geometrically free piece is nailed -> the closest to release (lowest index) pops
            int pick = -1, low = int.MaxValue; bool anyFree = false, unNailedFree = false;
            for (int p = 0; p < O.N && !unNailedFree; p++)
            {
                if (!Present[p] || !GeomFree(p)) continue;
                anyFree = true;
                if (Nail[p] <= 0) unNailedFree = true;
                else if (Nail[p] < low) { low = Nail[p]; pick = p; }
            }
            if (anyFree && !unNailedFree) Nail[pick] = 0;
            for (int p = 0; p < O.N; p++)
            {
                if (!BombAlive(p)) continue;
                Fuse[p]--;
                if (Fuse[p] == 0 && FirstBlow < 0) FirstBlow = Exits;
            }
        }

        /* Why a present, unavailable piece cannot be taken now (one reason, most visible first). */
        public Hold Reason(int p)
        {
            if (Available(p)) return Hold.None;
            if (GeomFree(p)) return Hold.Lock;
            for (int d = 0; d < 4; d++) if (O.Walled[p, d] && _rem[p, d] == 0) return Hold.Wall;
            if (O.Chain[p] && SingleCellCorridor(p)) return Hold.Chain;
            int best = -1;
            for (int d = 0; d < 4; d++) if (!O.Walled[p, d] && (best < 0 || _rem[p, d] < _rem[p, best])) best = d;
            if (best < 0) return Hold.Wall;
            return TravelRule.Limit(Board, p, (Dir)best).Hard ? Hold.Piece : Hold.Tab;
        }

        /* A chained piece one of whose cells alone would have an empty straight corridor (catches ignored). */
        bool SingleCellCorridor(int p)
        {
            var l = O.Level;
            foreach (var c in l.Pieces[p])
                foreach (var d in DirExt.All)
                {
                    bool clear = true;
                    for (int k = 1; ; k++)
                    {
                        var n = c.Step(d, k);
                        if (!l.OnBoard(n)) { if (O.LaneSealed(c, d)) clear = false; break; }
                        int o = Board.PieceAt(n);
                        if (o >= 0 && o != p) { clear = false; break; }
                    }
                    if (clear) return true;
                }
            return false;
        }

        /* Looks free (an open side, no lock, chain or wall). */
        public bool VisuallyFree(int p) => Present[p] && Nail[p] <= 0 && !O.Chain[p] && !O.TouchesWall[p] && OpenSides(p) > 0;

        /* Sides where every body cell faces an empty cell or an unwalled rim. */
        public int OpenSides(int p) { int n = 0; for (int d = 0; d < 4; d++) if (OpenSide(p, d)) n++; return n; }
        public bool OpenSide(int p, int d)
        {
            var l = O.Level;
            var cells = l.Pieces[p];
            foreach (var c in cells)
            {
                var n = c.Step((Dir)d);
                if (!l.OnBoard(n)) { if (O.LaneSealed(c, (Dir)d)) return false; continue; }
                int o = Board.PieceAt(n);
                if (o >= 0 && o != p) return false;
            }
            return true;
        }
        public int ExitDirCount(int p) { int n = 0; for (int d = 0; d < 4; d++) if (ExitsWay(p, d)) n++; return n; }
    }

    public static class DifficultyV2
    {
        public const int Unreachable = 9999;

        /* Fewest further exits until the bomb piece leaves, counting the bomb's own exit, from the given state.
           Branch and bound over the choice of exit direction per needed piece; nails delay a needed piece until
           its counter is 0 (filler exits elsewhere count). Exact unless the node budget runs out (approx = true). */
        public static int MinExits(FastState s, int bomb, out List<int> set, out bool approx, int budget = 300000,
            int upper = Unreachable, List<int> upperSet = null)
        {
            var o = s.O; int n = o.N;
            var inSet = new bool[n]; var chosen = new int[n]; var pending = new List<int> { bomb };
            inSet[bomb] = true;
            // a known plan of cost `upper` (the previous step's): only cheaper ones are searched
            int best = upper < Unreachable ? upper + 1 : Unreachable, nodes = 0, valid = 0; List<int> bestSet = null;
            var resolved = new bool[n]; var seen = new int[n]; int stamp = 0; var stack = new Stack<int>();

            // does `from` (transitively, through the directions chosen so far) wait for `target`?
            bool Reaches(int from, int target)
            {
                stamp++; stack.Clear(); stack.Push(from);
                while (stack.Count > 0)
                {
                    int x = stack.Pop();
                    if (x == target) return true;
                    if (seen[x] == stamp || !resolved[x]) continue;
                    seen[x] = stamp;
                    foreach (int y in o.Obs[x][chosen[x]]) if (s.Present[y] && inSet[y]) stack.Push(y);
                }
                return false;
            }
            var scratch = new List<int>[64];

            void Rec(int i)
            {
                if (++nodes > budget) return;
                if (i == pending.Count)
                {
                    int cost = Schedule(s, pending, chosen, bomb);
                    if (cost < Unreachable) valid++;
                    if (cost < best) { best = cost; bestSet = new List<int>(pending); }
                    return;
                }
                int p = pending[i];
                var dirs = new List<(int d, int add)>(4);
                for (int d = 0; d < 4; d++)
                {
                    if (o.Walled[p, d]) continue;
                    int add = 0; bool cycle = false;
                    foreach (int q in o.Obs[p][d])
                    {
                        if (!s.Present[q]) continue;
                        if (!inSet[q]) add++;
                        else if (Reaches(q, p)) { cycle = true; break; }   // q already waits for p
                    }
                    if (!cycle) dirs.Add((d, add));
                }
                dirs.Sort((a, b) => a.add.CompareTo(b.add));
                foreach (var (d, add) in dirs)
                {
                    if (pending.Count + add >= best) continue;
                    int mark = pending.Count;
                    foreach (int q in o.Obs[p][d]) if (s.Present[q] && !inSet[q]) { inSet[q] = true; pending.Add(q); }
                    chosen[p] = d; resolved[p] = true;
                    Rec(i + 1);
                    resolved[p] = false;
                    for (int k = mark; k < pending.Count; k++) inSet[pending[k]] = false;
                    pending.RemoveRange(mark, pending.Count - mark);
                    if (nodes > budget) return;
                }
            }

            Rec(0);
            approx = nodes > budget;
            if (bestSet == null && upper < Unreachable)
            {
                set = upperSet == null ? new List<int>() : upperSet.Where(q => s.Present[q]).ToList();
                return upper;
            }
            set = bestSet ?? new List<int>();
            return best;
        }

        /* One exit per step: take any needed piece whose corridor pieces are gone and whose nail is 0, else a
           filler exit. Unit tasks with release times and precedence: greedy without idling is optimal. */
        static int Schedule(FastState s, List<int> set, int[] chosen, int bomb)
        {
            var o = s.O;
            var done = new HashSet<int>();
            var nail = new Dictionary<int, int>();
            foreach (int p in set) nail[p] = s.Nail[p];
            for (int c = 1; c < 400; c++)
            {
                int pick = -1;
                foreach (int p in set)
                {
                    if (done.Contains(p) || nail[p] > 0) continue;
                    bool ready = true;
                    foreach (int q in o.Obs[p][chosen[p]]) if (s.Present[q] && !done.Contains(q)) { ready = false; break; }
                    if (!ready) continue;
                    if (p == bomb && done.Count < set.Count - 1) continue;
                    pick = p; break;
                }
                if (pick == bomb) return c;
                if (pick >= 0) done.Add(pick);
                else
                {
                    bool nailWait = false;
                    foreach (int p in set) if (!done.Contains(p) && nail[p] > 0) nailWait = true;
                    if (!nailWait) return Unreachable;          // a cycle in the chosen directions
                }
                foreach (int p in set) if (nail[p] > 0) nail[p]--;
            }
            return Unreachable;
        }

        public enum Policy { Rand, GreedyEdge, BombAware }

        public sealed class PolicyStats
        {
            public double PSurvive;
            public double? NatSlackMed, FirstChokeEx;
            public double?[] NatSlackMedPerBomb;
        }

        public sealed class Profile
        {
            public double OpenMean, OpenMin, BottleneckN, NarrowN, NarrowHiddenN, DistractorRatio = double.NaN;
            public double ShareLock, ShareChain, ShareWall, SharePiece, ShareTab;
            public double NarrowByLock, NarrowByChain, NarrowByWall;
            public double JamBaitStart, JamBaitMid, DirAmbiguity;
            public double[] DistractPerStep;        // per bomb (bomb order), NaN when no window
        }

        public sealed class Features
        {
            public LevelData Level;
            public int[] BombPieces = new int[0];
            public int[] SlackTrue = new int[0];
            public int[] MinExitsAtStart = new int[0];
            public bool SlackApprox;
            public int ChokeN;
            public PolicyStats Rand, Greedy, Aware;
            public Profile Prof;
            public double Seconds;
        }

        public static Features Compute(LevelData level, int games = 2000, int profileGames = 300, uint seed = 69)
        {
            var t0 = DateTime.Now;
            var o = new Obstacles(level);
            var f = new Features { Level = level };
            var start = new FastState(o);
            f.BombPieces = level.Bombs.Keys.OrderBy(k => k).ToArray();
            int nb = f.BombPieces.Length;
            f.SlackTrue = new int[nb]; f.MinExitsAtStart = new int[nb];
            var paths = new List<int>[nb];
            for (int i = 0; i < nb; i++)
            {
                int b = f.BombPieces[i];
                f.MinExitsAtStart[i] = MinExits(start, b, out paths[i], out bool approx);
                f.SlackApprox |= approx;
                f.SlackTrue[i] = level.Bombs[b] - f.MinExitsAtStart[i];
            }
            f.ChokeN = f.SlackTrue.Count(x => x <= 1);
            var pathSet = paths.Select(p => new HashSet<int>(p)).ToArray();
            f.Rand = Run(o, f, pathSet, Policy.Rand, games, seed, 0, null);
            f.Greedy = Run(o, f, pathSet, Policy.GreedyEdge, games, seed + 1, 0, null);
            f.Prof = new Profile();
            f.Aware = Run(o, f, pathSet, Policy.BombAware, games, seed + 2, profileGames, f.Prof);
            f.Seconds = (DateTime.Now - t0).TotalSeconds;
            return f;
        }

        static double? Median(List<double> v)
        {
            if (v.Count == 0) return null;
            v.Sort();
            return v.Count % 2 == 1 ? v[v.Count / 2] : (v[v.Count / 2 - 1] + v[v.Count / 2]) / 2;
        }

        static PolicyStats Run(Obstacles o, Features f, HashSet<int>[] paths, Policy policy, int games, uint seed, int profileGames, Profile prof)
        {
            var rng = new Mulberry32(seed);
            int nb = f.BombPieces.Length, survive = 0;
            var natMin = new List<double>(); var choke = new List<double>();
            var perBomb = Enumerable.Range(0, nb).Select(_ => new List<double>()).ToArray();
            // profile accumulators
            double openSum = 0; int openSteps = 0; var openMins = new List<double>();
            double bottle = 0, narrow = 0, hidden = 0, nLock = 0, nChain = 0, nWall = 0;
            var held = new double[6];
            double drSum = 0; int drSteps = 0;
            var dSum = new double[nb]; var dSteps = new int[nb];
            double baitStart = 0, baitMid = 0, ambNum = 0, ambDen = 0;

            for (int g = 0; g < games; g++)
            {
                var s = new FastState(o);
                bool prof1 = g < profileGames && prof != null;
                int gameMin = int.MaxValue; bool midDone = false;
                if (prof1) { baitStart += JamBait(s); Ambiguity(s, ref ambNum, ref ambDen); }
                bool firstBombGone = nb == 0;
                while (s.PresentCount > 0)
                {
                    var avail = s.AvailableList();
                    if (avail.Count == 0) break;
                    int nearest = -1;
                    for (int i = 0; i < nb; i++)
                        if (s.Present[f.BombPieces[i]] && (nearest < 0 || s.Fuse[f.BombPieces[i]] < s.Fuse[f.BombPieces[nearest]])) nearest = i;
                    if (prof1)
                    {
                        int open = avail.Count;
                        openSum += open; openSteps++; gameMin = Math.Min(gameMin, open);
                        if (open == 1) bottle++;
                        bool lockH = false, chainH = false, wallH = false;
                        for (int p = 0; p < o.N; p++)
                        {
                            if (!s.Present[p] || s.Available(p)) continue;
                            var r = s.Reason(p);
                            held[(int)r]++;
                            if (r == Hold.Lock) lockH = true; else if (r == Hold.Chain) chainH = true; else if (r == Hold.Wall) wallH = true;
                        }
                        if (open <= 2)
                        {
                            narrow++;
                            if (!avail.Any(s.VisuallyFree)) hidden++;
                            if (lockH) nLock++; if (chainH) nChain++; if (wallH) nWall++;
                        }
                        if (nearest >= 0 && !firstBombGone)
                        {
                            int on = avail.Count(paths[nearest].Contains);
                            drSum += (double)(open - on) / open; drSteps++;
                        }
                        for (int i = 0; i < nb; i++)
                        {
                            int b = f.BombPieces[i];
                            if (!s.Present[b] || s.Exits + 1 > o.Fuse0[b]) continue;
                            dSum[i] += avail.Count(p => !paths[i].Contains(p)); dSteps[i]++;
                        }
                        if (!midDone && s.PresentCount * 2 <= o.N) { midDone = true; baitMid += JamBait(s); Ambiguity(s, ref ambNum, ref ambDen); }
                    }
                    int pick;
                    if (policy == Policy.GreedyEdge)
                    {
                        int bestD = int.MaxValue; var ties = new List<int>();
                        foreach (int p in avail)
                        {
                            int dmin = int.MaxValue;
                            for (int d = 0; d < 4; d++) if (s.ExitsWay(p, d)) dmin = Math.Min(dmin, o.Dist[p, d]);
                            if (dmin < bestD) { bestD = dmin; ties.Clear(); }
                            if (dmin == bestD) ties.Add(p);
                        }
                        pick = ties[(int)(rng.NextDouble() * ties.Count)];
                    }
                    else if (policy == Policy.BombAware && nearest >= 0 && rng.NextDouble() < .5)
                    {
                        var on = avail.Where(paths[nearest].Contains).ToList();
                        pick = on.Count > 0 ? on[(int)(rng.NextDouble() * on.Count)] : avail[(int)(rng.NextDouble() * avail.Count)];
                    }
                    else pick = avail[(int)(rng.NextDouble() * avail.Count)];
                    s.Remove(pick);
                    if (!firstBombGone && f.BombPieces.Any(b => !s.Present[b])) firstBombGone = true;
                }
                if (prof1) openMins.Add(gameMin == int.MaxValue ? 0 : gameMin);
                if (s.FirstBlow < 0) survive++; else choke.Add(s.FirstBlow);
                double mn = double.MaxValue;
                for (int i = 0; i < nb; i++)
                {
                    int b = f.BombPieces[i];
                    double slack = s.RemovedAt[b] > 0 ? o.Fuse0[b] - s.RemovedAt[b] : -999;
                    perBomb[i].Add(slack); mn = Math.Min(mn, slack);
                }
                if (nb > 0) natMin.Add(mn);
            }

            if (prof != null && profileGames > 0)
            {
                int pg = Math.Min(games, profileGames);
                prof.OpenMean = openSteps == 0 ? 0 : openSum / openSteps;
                prof.OpenMin = Median(openMins) ?? 0;
                prof.BottleneckN = bottle / pg; prof.NarrowN = narrow / pg; prof.NarrowHiddenN = hidden / pg;
                prof.NarrowByLock = nLock / pg; prof.NarrowByChain = nChain / pg; prof.NarrowByWall = nWall / pg;
                double tot = held.Sum();
                if (tot > 0)
                {
                    prof.ShareLock = held[(int)Hold.Lock] / tot; prof.ShareChain = held[(int)Hold.Chain] / tot;
                    prof.ShareWall = held[(int)Hold.Wall] / tot; prof.SharePiece = held[(int)Hold.Piece] / tot;
                    prof.ShareTab = held[(int)Hold.Tab] / tot;
                }
                prof.DistractorRatio = drSteps == 0 ? double.NaN : drSum / drSteps;
                prof.DistractPerStep = Enumerable.Range(0, nb).Select(i => dSteps[i] == 0 ? double.NaN : dSum[i] / dSteps[i]).ToArray();
                prof.JamBaitStart = baitStart / pg; prof.JamBaitMid = baitMid / pg;
                prof.DirAmbiguity = ambDen == 0 ? 0 : ambNum / ambDen;
            }
            return new PolicyStats
            {
                PSurvive = (double)survive / games,
                NatSlackMed = Median(natMin), FirstChokeEx = Median(choke),
                NatSlackMedPerBomb = perBomb.Select(Median).ToArray(),
            };
        }

        /* Pieces with an open-looking side they cannot actually leave by. */
        public static int JamBait(FastState s)
        {
            int n = 0;
            for (int p = 0; p < s.O.N; p++)
            {
                if (!s.Present[p] || s.Nail[p] > 0) continue;
                for (int d = 0; d < 4; d++)
                    if (s.OpenSide(p, d) && !s.ExitsWay(p, d)) { n++; break; }
            }
            return n;
        }

        static void Ambiguity(FastState s, ref double num, ref double den)
        {
            for (int p = 0; p < s.O.N; p++)
            {
                if (!s.Present[p] || s.Nail[p] > 0 || s.OpenSides(p) < 2) continue;
                den++;
                if (s.ExitDirCount(p) == 1) num++;
            }
        }
    }
}
