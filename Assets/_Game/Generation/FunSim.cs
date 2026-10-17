using System;
using System.Collections.Generic;
using System.Linq;
using ReverseSolver.Core;
using ReverseSolver.Editing;

namespace ReverseSolver.Generation
{
    /* G4: the fun estimate ("keyif tahmini", PRODUCT.md "Faz 4"). 1000 seeded
       virtual players with different habits play a level up to three times; each
       is classed as enjoyed, frustrated or bored by named rules. This is a model's
       output, not a measurement of real players: the playtest's 👍/👎 checks it.

       A player's move: they look at pieces in random order until they find one
       that can leave (looks = position of the first free, unnailed piece; for a
       random looker the mean is (n + 1) / (k + 1), the model's "reading"), their
       skill shortens the search, and the move takes
           seconds = speed × (MoveBase + MovePerLook × looks)
       which for a median player is the difficulty model's s = 0.6 + 0.16 × reading.
       With a bomb on the board, bomb attention is the chance they work on the
       bomb's way out (the pieces blocking it) instead of any free piece.

       Every constant below is an assumption, named here and printed in the report. */
    public static class FunSim
    {
        // ---- the population (assumptions) ---------------------------------------------------------
        public const double MoveBase = 0.6, MovePerLook = 0.16;      // eğitim seti, modül 8
        public const double SpeedSigma = 0.35;                       // log-normal spread of speed between players
        public const double PatienceMedian = 12, PatienceSigma = 0.4; // seconds one move may take before it annoys
        public const double RetryMin = .55, RetryMax = 1.0;          // chance to try again after a loss
        public const double RetryNearMiss = .30, RetryEarlyBomb = -.30;
        public const double Learning = .85;                          // looks on later attempts (öğrenmeli); 1 = öğrenmesiz

        // ---- the rules (eğitim seti, modüller 4–8) ---------------------------------------------------
        public const double NearMissWinShare = .10;                  // won with < 10 % of the time left
        public const int NearMissLossPieces = 3;                     // lost with <= 3 pieces left
        public const int EarlyBombMoves = 5;                         // a bomb in the first 5 moves
        public const double ManyPiecesLeft = .25;                    // time ran out with >= 25 % of the pieces left
        public const double BoredTimeLeft = .60;                     // won with > 60 % of the time left …
        public const int KnotLooks = 4;                              // … and never needed 4+ looks for a move
        public const int Attempts = 3;

        // ---- V5: when a stuck player reaches for a booster (assumptions) ---------------------------
        public const double ClockTimeShare = .30;                    // clock: time left <= 30 % of the timer …
                                                                     // … and pieces left × this attempt's mean move > time left
        public const int HammerFuse = 2;                             // hammer on a bomb: its fuse <= 2 and the piece not free
        public const int HammerMaxLooks = 8;                         // hammer when stuck: looks reach min(8, patience − 1)

        /* The model assumptions PM froze at checkpoint 3 (PRODUCT.md "Keyif eşikleri
           (kontrol noktası 3)"). Legacy turns them all off: G4's first version, kept
           so its numbers stay reproducible. */
        public sealed class Options
        {
            public double Memory = .5;            // V1: a piece removed in an earlier attempt is found with 1 look, with this chance
            public double AttentionAfterBomb = .8; // V2: after a bomb went off, attention is at least this (0 = off)
            public bool BombIntro;                 // V3: the bomb introduction slot: attention drawn from [IntroAttention, 1]
            public double IntroAttention = .8;
            public int KolayKnotLooks = 3;         // V4: on Kolay a knot is 3+ looks (others KnotLooks)
            /* V5 (PRODUCT.md "V5 çıkış kapısı", 2026-10-17): the player uses the hammer and the
               clock from the level's own stock and unlock (GameSession's rule) when stuck. Off:
               the run is the same as before V5, draw for draw. */
            public bool Boosters;

            public static Options Legacy => new Options { Memory = 0, AttentionAfterBomb = 0, BombIntro = false, KolayKnotLooks = KnotLooks };
            public Options With(double memory) { var o = (Options)MemberwiseClone(); o.Memory = memory; return o; }
        }

        /* The options for a slot: V3 only on the bomb introduction. */
        public static Options OptionsFor(SlotSpec spec, bool boosters = false) => new Options { BombIntro = spec.NewThing == SlotSpec.Bomb, Boosters = boosters };

        /* The reasons that make frustration "unfair" (haksız) rather than difficulty. */
        public static readonly string[] UnfairReasons = { "tanıtımda kayıp", "ilk 5 hamlede patlama", "sabır aşıldı" };

        public sealed class Player
        {
            public double Speed, Skill, BombAttention, Patience, Retry;
        }

        /* The same 1000 players for every level, so levels compare. */
        public static List<Player> Population(int n = 1000, uint seed = 20261013)
        {
            var r = new Mulberry32(seed);
            double Normal() { double u = Math.Max(1e-12, r.NextDouble()), v = r.NextDouble(); return Math.Sqrt(-2 * Math.Log(u)) * Math.Cos(2 * Math.PI * v); }
            var list = new List<Player>(n);
            for (int i = 0; i < n; i++)
                list.Add(new Player
                {
                    Speed = Math.Exp(SpeedSigma * Normal()),
                    Skill = r.NextDouble(),
                    BombAttention = r.NextDouble(),
                    Patience = PatienceMedian * Math.Exp(PatienceSigma * Normal()),
                    Retry = RetryMin + (RetryMax - RetryMin) * r.NextDouble(),
                });
            return list;
        }

        public enum Verdict { Enjoyed, Frustrated, Bored }

        public sealed class Result
        {
            public int Players;
            public int Enjoyed, Frustrated, Bored;
            public int WinFirst, WinBy3, NearMiss;
            public int AhaPlayers;
            public double MedianStars;
            public double Ms;
            public bool Learning;
            public double Memory;
            public int UnfairFrustrated, HardFrustrated, FrustratedAfterNearMiss;
            /* Players whose clock ran out with >= 25 % of the pieces left in some attempt. */
            public int EarlyTimeout;
            /* V5: players who used the hammer / the clock at least once, and wins in an attempt that used either. */
            public bool Boosters;
            public int HammerPlayers, ClockPlayers, Wins, BoostedWins;
            /* the most hammers / clocks one attempt used, and hammers on a bomb about to go off (V5 (a)) */
            public int MostHammers, MostClocks, BombHammers;
            public double HammerRate => (double)HammerPlayers / Players;
            public double ClockRate => (double)ClockPlayers / Players;
            public double BoostedWinShare => Wins == 0 ? 0 : (double)BoostedWins / Wins;
            public double EarlyTimeoutRate => (double)EarlyTimeout / Players;
            public double UnfairRate => (double)UnfairFrustrated / Players;
            public double HardRate => (double)HardFrustrated / Players;
            public readonly Dictionary<string, int> FrustrationWhy = new Dictionary<string, int>();
            public readonly Dictionary<string, int> EnjoyedHow = new Dictionary<string, int>();

            public double EnjoyRate => (double)Enjoyed / Players;
            public double FrustrationRate => (double)Frustrated / Players;
            public double BoredRate => (double)Bored / Players;
            public double FirstWinRate => (double)WinFirst / Players;
            public double WinBy3Rate => (double)WinBy3 / Players;
            public double NearMissRate => (double)NearMiss / Players;

            public override string ToString() =>
                $"keyif %{EnjoyRate * 100:0.0} · hayal kırıklığı %{FrustrationRate * 100:0.0} · sıkıcı %{BoredRate * 100:0.0} · " +
                $"ilk deneme %{FirstWinRate * 100:0} · 3 denemede %{WinBy3Rate * 100:0} · near-miss %{NearMissRate * 100:0} · " +
                $"medyan yıldız {MedianStars:0} · haksız hk %{UnfairRate * 100:0.0} · {Ms:0} ms" + (Learning ? "" : " (öğrenmesiz)") + $" · m {Memory:0.0}" +
                (Boosters ? $" · booster'lı (çekiç %{HammerRate * 100:0}, saat %{ClockRate * 100:0}, kazançların %{BoostedWinShare * 100:0}'i booster'lı)" : "");
        }

        // ---- one level --------------------------------------------------------------------------------

        public static Result Run(LevelData level, Genre genre, List<Player> players = null, bool learning = true, uint seed = 1, Options options = null)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            players ??= Population();
            options ??= new Options();
            var cache = new Cache(level);
            var res = new Result { Players = players.Count, Learning = learning, Memory = options.Memory, Boosters = options.Boosters };
            int knotLooks = genre == Genre.Kolay ? options.KolayKnotLooks : KnotLooks;
            var stars = new List<int>(players.Count);
            bool intro = genre == Genre.Tanitim;
            bool boringApplies = genre != Genre.Tanitim && genre != Genre.Nefes;
            for (int i = 0; i < players.Count; i++)
            {
                var p = players[i];
                var rng = new Mulberry32(seed * 7919u + (uint)i * 104729u + 1u);
                var why = new List<string>();
                bool won = false, nearMiss = false, lostNearMiss = false, aha = false, knot = false, lastLossNearMiss = false;
                int winAttempt = 0, winStars = 0;
                double winLeftShare = 0;
                double retry = p.Retry;
                double attention = options.BombIntro ? options.IntroAttention + (1 - options.IntroAttention) * p.BombAttention : p.BombAttention;
                var remembered = new List<int>();
                var recall = new Mulberry32(seed * 31337u + (uint)i * 7u + 3u);     // V1's own draws: the main stream stays as it was
                var reach = options.Boosters ? new Mulberry32(seed * 65537u + (uint)i * 11u + 5u) : null;   // V5's own draws
                bool hammered = false, clocked = false;
                for (int a = 1; a <= Attempts && !won; a++)
                {
                    var at = Play(level, p, a > 1 && learning ? Learning : 1.0, cache, rng, attention, a > 1 ? options.Memory : 0, remembered, knotLooks, recall, reach);
                    hammered |= at.Hammers > 0;
                    res.MostHammers = Math.Max(res.MostHammers, at.Hammers);
                    res.MostClocks = Math.Max(res.MostClocks, at.Clocks);
                    res.BombHammers += at.BombHammers;
                    clocked |= at.Clocks > 0;
                    foreach (var piece in at.Removed) if (!remembered.Contains(piece)) remembered.Add(piece);
                    if (at.Outcome == Outcome.Bomb && options.AttentionAfterBomb > 0) attention = Math.Max(attention, options.AttentionAfterBomb);
                    knot |= at.Knot;
                    aha |= at.Aha;
                    if (at.LongestMove > p.Patience) why.Add("sabır aşıldı");
                    if (at.Outcome == Outcome.Win)
                    {
                        won = true; winAttempt = a;
                        res.Wins++;
                        if (at.Hammers + at.Clocks > 0) res.BoostedWins++;
                        winLeftShare = (double)at.TimeLeft / level.TimerSeconds;
                        winStars = GameSession.StarsFor(at.TimeLeft, level.TimerSeconds);
                        if (winLeftShare < NearMissWinShare) nearMiss = true;
                        break;
                    }
                    if (intro) why.Add("tanıtımda kayıp");
                    if (at.Outcome == Outcome.Bomb && at.Moves <= EarlyBombMoves) { why.Add("ilk 5 hamlede patlama"); retry += RetryEarlyBomb; }
                    if (at.Outcome == Outcome.Time && at.PiecesLeft >= ManyPiecesLeft * level.Pieces.Count) why.Add("çok parça kala süre bitti");
                    lastLossNearMiss = at.PiecesLeft <= NearMissLossPieces;
                    if (at.PiecesLeft <= NearMissLossPieces) { nearMiss = true; lostNearMiss = true; retry += RetryNearMiss; }
                    if (a < Attempts && rng.NextDouble() > Math.Min(1, Math.Max(0, retry))) break;   // gives up
                }
                if (!won) why.Add("3 denemede kazanamadı");
                if (won && winAttempt == 1) res.WinFirst++;
                if (won) res.WinBy3++;
                if (nearMiss) res.NearMiss++;
                if (aha) res.AhaPlayers++;
                if (hammered) res.HammerPlayers++;
                if (clocked) res.ClockPlayers++;
                stars.Add(won ? winStars : 0);

                if (why.Contains("çok parça kala süre bitti")) res.EarlyTimeout++;
                if (why.Count > 0)
                {
                    res.Frustrated++;
                    if (UnfairReasons.Contains(why[0])) res.UnfairFrustrated++; else res.HardFrustrated++;
                    if (lastLossNearMiss) res.FrustratedAfterNearMiss++;
                    foreach (var w in why.Distinct()) { res.FrustrationWhy.TryGetValue(w, out int n); res.FrustrationWhy[w] = n + 1; }
                }
                else if (boringApplies && won && !knot && winLeftShare > BoredTimeLeft) res.Bored++;
                else
                {
                    res.Enjoyed++;
                    string how = lostNearMiss ? "near-miss kayıp sonra kazanç" : winLeftShare < NearMissWinShare ? "near-miss kazanç" : "bantta kazanç";
                    res.EnjoyedHow.TryGetValue(how, out int n); res.EnjoyedHow[how] = n + 1;
                }
            }
            stars.Sort();
            res.MedianStars = stars[stars.Count / 2];
            res.Ms = sw.Elapsed.TotalMilliseconds;
            return res;
        }

        sealed class Attempt
        {
            public Outcome Outcome;
            public int Moves, TimeLeft, PiecesLeft;
            public double LongestMove;
            public bool Knot, Aha;
            public int Hammers, Clocks, BombHammers;
            public readonly List<int> Removed = new List<int>();
        }

        /* attention: the bomb attention for this attempt (V2, V3); memory: V1's chance that
           a piece already taken out in an earlier attempt is found at once; reach: V5's draws,
           null when the player never uses a booster. */
        static Attempt Play(LevelData level, Player p, double learn, Cache cache, IRandom rng,
                            double attention, double memory, List<int> remembered, int knotLooks, IRandom recall, IRandom reach)
        {
            var s = new GameSession(level, 1, "funsim", new Mulberry32(1));
            var at = new Attempt();
            var moveTimes = new List<double>();
            double moveSum = 0;
            var isFree = new bool[level.Pieces.Count];                 // membership of `free`, same answers as free.Contains
            int prevFree = -1;
            bool stalled = false;
            var order = new List<int>();
            while (!s.IsOver)
            {
                ulong mask = cache.Mask(s.Board);
                var free = cache.Free(s.Board, mask).Where(x => s.NailAt(x) <= 0).ToList();
                if (free.Count == 0) { at.Outcome = Outcome.Quit; break; }              // frozen: counts as not won
                Array.Clear(isFree, 0, isFree.Length);
                foreach (var q in free) isFree[q] = true;
                int n = s.Board.PresentCount;

                if (reach != null)
                {
                    // V5 clock: the time left will not cover the pieces left at this attempt's pace
                    if (moveTimes.Count > 0 && Usable(s, Booster.Clock) && s.TimeLeft <= ClockTimeShare * level.TimerSeconds
                        && n * (moveSum / moveTimes.Count) > s.TimeLeft)
                    {
                        s.UseClock();
                        at.Clocks++;
                    }
                    // V5 hammer (a): a bomb about to go off whose piece cannot leave, if the player sees it
                    int lit = Target(s);
                    if (lit >= 0 && s.FuseAt(lit) <= HammerFuse && !isFree[lit] && Usable(s, Booster.Hammer)
                        && reach.NextDouble() < attention)
                    {
                        at.BombHammers++;
                        moveSum += p.Speed * (MoveBase + MovePerLook);
                        if (Hammer(s, at, lit, p.Speed * (MoveBase + MovePerLook), moveTimes)) break;
                        continue;
                    }
                }

                // V1: remembers a piece from an earlier attempt that can leave now
                int recalled = -1;
                if (memory > 0 && recall.NextDouble() < memory)
                    foreach (var q in remembered) if (isFree[q]) { recalled = q; break; }

                // looks: position of the first free piece in a random order, shortened by skill
                order.Clear();
                foreach (var q in s.Board.PresentPieces()) order.Add(q);
                int looks = 1;
                for (int i = order.Count - 1; i >= 0; i--)
                {
                    int k = (int)(rng.NextDouble() * (i + 1));
                    int q = order[k]; order[k] = order[i]; order[i] = q;
                    if (isFree[q]) break;
                    looks++;
                }
                if (recalled >= 0) looks = 1;
                // V5 hammer (b): stuck for min(8, patience − 1) looks, they break a piece that cannot leave
                int breakable = reach != null && looks >= Math.Min(HammerMaxLooks, p.Patience - 1) && Usable(s, Booster.Hammer) ? HammerTarget(s, cache, isFree) : -1;
                if (breakable >= 0)
                {
                    int stuckLooks = Math.Max(1, (int)Math.Ceiling(Math.Min(HammerMaxLooks, p.Patience - 1)));
                    double spent = p.Speed * (MoveBase + MovePerLook * (1 + (stuckLooks - 1) * (1.3 - .6 * p.Skill) * learn));
                    at.Knot = true;
                    moveSum += spent;
                    if (Hammer(s, at, breakable, spent, moveTimes)) break;
                    continue;
                }
                double eff = 1 + (looks - 1) * (1.3 - .6 * p.Skill) * learn;
                double seconds = p.Speed * (MoveBase + MovePerLook * eff);
                moveTimes.Add(seconds);
                moveSum += seconds;

                if (looks >= knotLooks) { at.Knot = true; stalled = true; }
                // aha: right after a stalled move, two or more pieces more can leave
                if (prevFree >= 0 && stalled && free.Count >= prevFree + 2) { at.Aha = true; stalled = false; }
                prevFree = free.Count;

                at.LongestMove = Math.Max(at.LongestMove, seconds);
                s.Tick(seconds);
                if (s.IsOver) break;

                int pick = free[(int)(rng.NextDouble() * free.Count)];
                if (recalled >= 0) pick = recalled;
                int bomb = Target(s);
                if (bomb >= 0 && rng.NextDouble() < attention)
                {
                    var cone = cache.Cone(s.Board, mask, bomb).Where(x => isFree[x]).ToList();
                    if (isFree[bomb]) pick = bomb;
                    else if (cone.Count > 0) pick = cone[(int)(rng.NextDouble() * cone.Count)];
                }
                var dirs = TravelRule.ExitDirs(s.Board, pick);
                s.TryExit(pick, dirs[(int)(rng.NextDouble() * dirs.Count)]);
                at.Removed.Add(pick);
            }
            if (at.Outcome != Outcome.Quit) at.Outcome = s.Outcome;
            at.Moves = s.Moves;
            at.TimeLeft = s.TimeLeft;
            at.PiecesLeft = s.Board.PresentCount;
            return at;
        }

        static bool Usable(GameSession s, Booster b) => !s.IsOver && s.IsUnlocked(b) && s.Stock(b) > 0;

        /* One hammer move: the time it takes, then GameSession's own hammer rule. True when the attempt is over. */
        static bool Hammer(GameSession s, Attempt at, int piece, double seconds, List<double> moveTimes)
        {
            at.LongestMove = Math.Max(at.LongestMove, seconds);
            moveTimes.Add(seconds);
            s.Tick(seconds);
            if (s.IsOver) return true;
            s.UseHammer(piece);
            at.Hammers++;
            at.Removed.Add(piece);
            return s.IsOver;
        }

        /* The stuck player's hammer target among the pieces that cannot leave: one on the
           most urgent bomb's way out, else a nailed piece still counting, else any; within
           each, the one whose removal frees the most pieces (then the lowest index). */
        static int HammerTarget(GameSession s, Cache cache, bool[] isFree)
        {
            var stuck = s.Board.PresentPieces().Where(x => !isFree[x]).ToList();
            int bomb = Target(s);
            var tier = bomb >= 0 ? cache.Cone(s.Board, cache.Mask(s.Board), bomb).Where(stuck.Contains).ToList() : new List<int>();
            if (bomb >= 0 && stuck.Contains(bomb)) tier.Add(bomb);
            if (tier.Count == 0) tier = stuck.Where(x => s.NailAt(x) > 0).ToList();
            if (tier.Count == 0) tier = stuck;
            int best = -1, most = -1;
            foreach (var x in tier.OrderBy(x => x))
            {
                int freed = cache.FreeWithout(s.Board, x);
                if (freed > most) { most = freed; best = x; }
            }
            return best;
        }

        static int Target(GameSession s)
        {
            int best = -1, fuse = int.MaxValue;
            foreach (var b in s.Bombs)
                if (s.Board.IsPresent(b.Key) && b.Value < fuse) { fuse = b.Value; best = b.Key; }
            return best;
        }

        /* Free pieces and bomb cones by the set of pieces still on the board (the board's
           joints never change without boosters, so the set decides both). */
        sealed class Cache
        {
            readonly Dictionary<(ulong, ulong), int[]> _free = new Dictionary<(ulong, ulong), int[]>();
            readonly Dictionary<(ulong, ulong, int), int[]> _cone = new Dictionary<(ulong, ulong, int), int[]>();
            readonly int _n;
            public Cache(LevelData l) { _n = l.Pieces.Count; }

            public ulong Mask(Board b)
            {
                ulong m = 0;
                foreach (var p in b.PresentPieces()) if (p < 64) m |= 1UL << p;
                return m;
            }

            ulong High(Board b)
            {
                ulong m = 0;
                foreach (var p in b.PresentPieces()) if (p >= 64) m |= 1UL << (p - 64);
                return m;
            }

            public int[] Free(Board b, ulong mask)
            {
                var key = (mask, _n > 64 ? High(b) : 0UL);
                if (!_free.TryGetValue(key, out var f)) _free[key] = f = TravelRule.FreePieces(b).ToArray();
                return f;
            }

            /* How many pieces could leave once piece x is gone (V5's hammer target), by the set of pieces left. */
            readonly Dictionary<(ulong, ulong, int), int> _freeWithout = new Dictionary<(ulong, ulong, int), int>();
            public int FreeWithout(Board b, int x)
            {
                var key = (Mask(b), _n > 64 ? High(b) : 0UL, x);
                if (_freeWithout.TryGetValue(key, out int n)) return n;
                var c = b.Clone();
                c.Remove(x);
                return _freeWithout[key] = TravelRule.FreePieces(c).Count;
            }

            public int[] Cone(Board b, ulong mask, int target)
            {
                var key = (mask, _n > 64 ? High(b) : 0UL, target);
                if (_cone.TryGetValue(key, out var c)) return c;
                var cone = new List<int>();
                var seen = new HashSet<int> { target };
                var q = new Queue<int>();
                q.Enqueue(target);
                while (q.Count > 0)
                {
                    int x = q.Dequeue();
                    foreach (var d in DirExt.All)
                    {
                        int bl = TravelRule.Limit(b, x, d).Blocker;
                        if (bl >= 0 && seen.Add(bl)) { cone.Add(bl); q.Enqueue(bl); }
                    }
                }
                return _cone[key] = cone.ToArray();
            }
        }
    }
}
