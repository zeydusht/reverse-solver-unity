using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReverseSolver.Core;

namespace ReverseSolver.Editing
{
    /* Zeyd's five hardness factors (2026-10-18, after playing D04; reference: the web L40), measured
       on the strongest player's path (the stored solution if it wins, else SmartPlayer's):

         1 bomb and move budget  shortest dig (moves, the bomb's own last), fuse − dig, the share of
                                 first-move free pieces that are bait (not on any bomb's way out)
         2 nail on the critical  a nailed piece on a bomb's way out, and the dig with every nail taken
           path                  off: shorter = the nail counts, equal = the nail is decoration
         3 cross remote chains   chains whose cells differ in row and column, cells apart, and where in
                                 the solution they leave (share of the moves done)
         4 no runs               the longest run of moves leaving the same way from the same lane;
                                 the most pieces one move frees at once (cascade)
         5 bottleneck            in the middle half, the longest stretch with at most 2 free pieces;
                                 the most fake-free pieces at once along the path (they slide but stop half way) */
    public sealed class HardFactors
    {
        public int Dig = -1, DigWithoutNails = -1, FuseSlack = int.MinValue, FreeAtStart, Bait;
        public bool DigExact = true;
        public double BaitShare => FreeAtStart == 0 ? 0 : (double)Bait / FreeAtStart;
        public int NailsOnWay;
        public bool NailCritical => NailsOnWay > 0 && DigWithoutNails >= 0 && Dig > DigWithoutNails;
        public readonly List<(int gapRows, int gapCols, double leaveAt)> Chains = new List<(int, int, double)>();
        public int CrossChains => Chains.Count(c => c.gapRows > 0 && c.gapCols > 0 && c.gapRows + c.gapCols >= 4 && c.leaveAt >= .5);
        public bool ChainsCross;
        public int LaneRun, Cascade;
        public int NarrowStretch, FakeFree;
        public int Moves;
        public string Source = "";

        public static HardFactors Of(LevelData level, int digBudget = 60_000)
        {
            var f = new HardFactors();
            var moves = PathOf(level, out f.Source);
            f.Moves = moves.Count;
            var start = new GameSession(level, 1, "factors", new Mulberry32(1));

            // 1 + 2: the deepest bomb's dig, with and without the nails
            var m = StructureMetrics.Of(level, digBudget);
            if (m.BombDigs.Count > 0)
            {
                var deep = m.BombDigs.OrderByDescending(kv => kv.Value.dig).First();
                f.Dig = deep.Value.dig; f.DigExact = deep.Value.exact;
                f.FuseSlack = m.BombDigs.Min(kv => level.Bombs[kv.Key] - kv.Value.dig);
                var d = LevelDraft.FromLevel(level);
                foreach (var n in level.Nails) d.SetNail(level.Pieces[n.Key][0], 0);
                var bare = StructureMetrics.Of(d.ToLevelData(), digBudget);
                f.DigWithoutNails = bare.BombDigs.TryGetValue(deep.Key, out var bd) ? bd.dig : -1;
            }
            var way = new HashSet<int>();
            foreach (var b in level.Bombs.Keys) foreach (var p in Cone(start.Board, b)) way.Add(p);
            var free = FreeNow(start);
            f.FreeAtStart = free.Count;
            f.Bait = free.Count(p => !way.Contains(p) && !level.Bombs.ContainsKey(p));
            f.NailsOnWay = level.Nails.Keys.Count(way.Contains);

            // 3: remote chains
            var chainIds = Enumerable.Range(0, level.Pieces.Count).Where(i => level.Pieces[i].Length == 2).ToList();
            var leaveAt = new Dictionary<int, int>();
            for (int i = 0; i < moves.Count; i++) leaveAt[moves[i].Piece] = i;
            foreach (var c in chainIds)
            {
                var a = level.Pieces[c][0]; var b = level.Pieces[c][1];
                int dr = Math.Abs(a.Y - b.Y), dc = Math.Abs(a.X - b.X);
                if (dr + dc <= 1) continue;
                f.Chains.Add((dr, dc, moves.Count == 0 || !leaveAt.ContainsKey(c) ? 0 : (double)leaveAt[c] / moves.Count));
            }
            for (int i = 0; i < chainIds.Count; i++)
                for (int j = i + 1; j < chainIds.Count; j++)
                    if (Boxes(level.Pieces[chainIds[i]], level.Pieces[chainIds[j]])) f.ChainsCross = true;

            // 4 + 5 along the path
            var s = start.Clone();
            Dir? lastDir = null; int lastLane = -1, run = 0, narrow = 0;
            int lo = moves.Count / 4, hi = moves.Count * 3 / 4;
            for (int i = 0; i < moves.Count && !s.IsOver; i++)
            {
                var before = FreeNow(s);
                f.FakeFree = Math.Max(f.FakeFree, FakeFreeNow(s));
                if (i >= lo && i < hi) { narrow = before.Count <= 2 ? narrow + 1 : 0; f.NarrowStretch = Math.Max(f.NarrowStretch, narrow); }
                var mv = moves[i];
                var cell = level.Pieces[mv.Piece][0];
                int lane = mv.Dir.IsVertical() ? cell.X : cell.Y;
                run = lastDir == mv.Dir && lastLane == lane ? run + 1 : 1;
                f.LaneRun = Math.Max(f.LaneRun, run);
                lastDir = mv.Dir; lastLane = lane;
                s.TryExit(mv.Piece, mv.Dir);
                var after = FreeNow(s);
                f.Cascade = Math.Max(f.Cascade, after.Count(p => !before.Contains(p)));
            }
            return f;
        }

        /* The stored solution when it wins, else SmartPlayer's order. */
        static List<Move> PathOf(LevelData level, out string source)
        {
            if (level.Solution.Count > 0)
            {
                var g = new GameSession(level, 1, "factors", new Mulberry32(1));
                foreach (var mv in level.Solution) g.TryExit(mv.Piece, mv.Dir);
                if (g.Outcome == Outcome.Win) { source = "kayıtlı çözüm"; return level.Solution.ToList(); }
            }
            var r = SmartPlayer.Play(level);
            source = "SmartPlayer";
            return r.Verdict == SmartVerdict.Won ? r.Moves.ToList() : new List<Move>();
        }

        internal static GameSession CloneOf(GameSession s) => s.Clone();

        static List<int> FreeNow(GameSession s) =>
            TravelRule.FreePieces(s.Board).Where(p => s.NailAt(p) <= 0).ToList();

        /* Pieces that slide at least a cell some way but cannot leave any way (nails aside). */
        static int FakeFreeNow(GameSession s)
        {
            int n = 0;
            foreach (var p in s.Board.PresentPieces())
            {
                if (TravelRule.CanExit(s.Board, p)) continue;
                if (DirExt.All.Any(d => { var l = TravelRule.Limit(s.Board, p, d); return !l.Exit && l.Cells > 0; })) n++;
            }
            return n;
        }

        static IEnumerable<int> Cone(Board b, int target)
        {
            var seen = new HashSet<int> { target };
            var q = new Queue<int>();
            q.Enqueue(target);
            while (q.Count > 0)
            {
                int x = q.Dequeue();
                foreach (var d in DirExt.All)
                {
                    int bl = TravelRule.Limit(b, x, d).Blocker;
                    if (bl >= 0 && seen.Add(bl)) { q.Enqueue(bl); yield return bl; }
                }
            }
        }

        static bool Boxes(Cell[] a, Cell[] b)
        {
            int ax0 = Math.Min(a[0].X, a[1].X), ax1 = Math.Max(a[0].X, a[1].X), ay0 = Math.Min(a[0].Y, a[1].Y), ay1 = Math.Max(a[0].Y, a[1].Y);
            int bx0 = Math.Min(b[0].X, b[1].X), bx1 = Math.Max(b[0].X, b[1].X), by0 = Math.Min(b[0].Y, b[1].Y), by1 = Math.Max(b[0].Y, b[1].Y);
            return ax0 <= bx1 && bx0 <= ax1 && ay0 <= by1 && by0 <= ay1;
        }

        /* Zeyd's targets; each line: name, value, target, held. */
        public List<(string name, string value, string target, bool ok)> Rows()
        {
            var r = new List<(string, string, string, bool)>
            {
                ("1 Kazma yolu", Dig < 0 ? "bomba yok" : $"{Dig}{(DigExact ? "" : " (yaklaşık)")}", "≥ 8", Dig >= 8),
                ("1 Fitil − yol", FuseSlack == int.MinValue ? "—" : $"{FuseSlack}", "1–2", FuseSlack >= 1 && FuseSlack <= 2),
                ("1 Yem (ilk hamle)", $"{Bait}/{FreeAtStart} (%{BaitShare * 100:0})", "≥ %50", FreeAtStart > 0 && BaitShare >= .5),
                ("2 Çivi kritik yolda", $"yolda {NailsOnWay}, çivisiz yol {(DigWithoutNails < 0 ? "—" : DigWithoutNails.ToString())}", "yolda ≥ 1 ve çivisiz yol kısa", NailCritical),
                ("3 Çapraz uzak zincir", $"{CrossChains} (toplam uzak {Chains.Count}{(ChainsCross ? ", kesişiyor" : "")})", "≥ 1, ≥ 4 hücre, ikinci yarıda", CrossChains >= 1),
                ("4 Aynı şeritten art arda", $"{LaneRun}", "≤ 2", LaneRun <= 2),
                ("4 Kendiliğinden açılan", $"{Cascade}", "≤ 2", Cascade <= 2),
                ("5 Dar geçit (orta yarı)", $"{NarrowStretch} adım", "≥ 4", NarrowStretch >= 4),
                ("5 Sahte serbest (en çok)", $"{FakeFree}", "> 0", FakeFree > 0),
            };
            return r;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var r in Rows()) sb.AppendLine($"{r.name}: {r.value} (hedef {r.target}) {(r.ok ? "✓" : "✗")}");
            return sb.ToString();
        }
    }
}
