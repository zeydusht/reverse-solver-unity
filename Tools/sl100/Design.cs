using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ReverseSolver.Core;
using ReverseSolver.Core.Json;
using ReverseSolver.Editing;
using ReverseSolver.Generation;

/* PM_82: the first 100 levels (sl100) designed to the PM's plan with difficulty model v2.
   Pool: every level the project has (p22 as played, the redesign sets v2-v4, web, ZeydBot, the factory's finalists,
   the agent sets), each also with obstacle kinds taken away (a nail, a wall or a bomb gone never breaks the recorded
   solution), and the generator's new boards for slots short of candidates. For each slot: the candidates that fit
   its rules (allowed obstacles, the lesson / combination it shows, its size), the timer that brings model v2's first
   try closest to the slot's target (within the duration and tempo limits), then the best by distance to the target,
   squareness and variety (no base twice, no two neighbours of one size, no two neighbours with the same bomb pattern). */
static class Design
{
    public enum Role { Learn, Lesson, Normal, Combo, Hard, VeryHard, Breather, Guide }

    public sealed class Cand
    {
        public string Key, Source, BaseId;
        public LevelData Level;
        public DifficultyV2.Features F;
        public HashSet<string> Types;
        public int N => Level.Pieces.Count;
        public bool Square => Level.Width == Level.Height;
        public bool Played;         // p22 as played (pt1 data blends in)
        public string BombSig;
        public bool Generated;
    }

    public sealed class Pick
    {
        public int Slot; public Role Role; public Cand C; public int Timer; public double P, Lo, Hi, Target; public int Considered;
        public string Note = "";
    }

    static readonly string[] Lessons = { null, null, null, "lock", null, null, null, "chain", null, null, null, "bomb" };
    static string LessonOf(int slot) => slot == 3 ? "lock" : slot == 7 ? "chain" : slot == 11 ? "bomb" : slot == 21 ? "wall" : null;

    static HashSet<string> ComboOf(int slot) => slot switch
    {
        31 => new HashSet<string> { "bomb", "chain" },
        41 => new HashSet<string> { "lock", "bomb" },
        51 => new HashSet<string> { "wall", "bomb" },
        61 => new HashSet<string> { "chain", "wall" },
        71 => new HashSet<string> { "lock", "chain", "bomb" },
        81 => new HashSet<string> { "wall", "lock" },
        91 => new HashSet<string> { "lock", "chain", "bomb", "wall" },
        _ => null,
    };

    /* The decade's theme: what the slots after a lesson or a combination develop and bend. */
    static HashSet<string> ThemeOf(int slot)
    {
        if (slot >= 31) return ComboOf((slot - 1) / 10 * 10 + 1);
        if (slot >= 21) return new HashSet<string> { "wall" };
        if (slot >= 11) return new HashSet<string> { "bomb" };
        if (slot >= 7) return new HashSet<string> { "chain" };
        if (slot >= 3) return new HashSet<string> { "lock" };
        return null;
    }

    static HashSet<string> Allowed(int slot)
    {
        var a = new HashSet<string>();
        if (slot >= 3) a.Add("lock");
        if (slot >= 7) a.Add("chain");
        if (slot >= 11) a.Add("bomb");
        if (slot >= 21) a.Add("wall");
        return a;
    }

    public static Role RoleOf(int slot)
    {
        if (slot == 1) return Role.Guide;
        if (LessonOf(slot) != null) return Role.Lesson;
        if (ComboOf(slot) != null) return Role.Combo;
        if (slot >= 14 && slot % 10 == 4) return Role.Hard;
        if (slot >= 19 && slot % 10 == 9) return Role.VeryHard;
        if (slot >= 20 && slot % 10 == 0) return Role.Breather;
        if (slot <= 13) return Role.Learn;
        return Role.Normal;
    }

    /* The first-try band (model v2, average player) and its target. */
    public static (double lo, double hi, double target) Band(int slot, Role r)
    {
        double k = Math.Clamp((slot - 20) / 80.0, 0, 1);
        double nlo = .75 - .10 * k, nhi = .85 - .10 * k;
        return r switch
        {
            Role.Guide or Role.Lesson => (.90, 1, .96),
            Role.Learn => slot == 2 ? (.90, 1, .985) : (.90, 1, .93),
            Role.Hard => (.35, .50, .42),
            Role.VeryHard => (.20, .30, .25),
            Role.Breather => (.85, 1, .90),
            Role.Combo => (nhi - .02, 1, Math.Min(.93, nhi + .04)),
            _ => (nlo, nhi, (nlo + nhi) / 2),
        };
    }

    /* Pieces by slot and role (boards grow over the game; breathers stay short). */
    static (int min, int max) Size(int slot, Role r)
    {
        if (slot == 2) return (9, 16);
        if (slot <= 6) return (16, 25);
        if (slot <= 13) return (20, 36);
        if (r == Role.Breather) return slot <= 50 ? (20, 33) : (25, 36);
        if (slot <= 30) return r == Role.Hard || r == Role.VeryHard ? (30, 56) : (25, 42);
        if (slot <= 60) return r == Role.Hard || r == Role.VeryHard ? (36, 64) : (30, 49);
        return r == Role.Hard || r == Role.VeryHard ? (40, 64) : (33, 56);
    }

    /* Duration limits (s) and tempo limits (s per piece). */
    static (int tmin, int tmax, double smin, double smax) Time(Role r) => r switch
    {
        Role.Hard or Role.VeryHard => (60, 150, 1.6, 2.6),
        Role.Breather => (45, 100, 1.8, 3.2),
        Role.Learn or Role.Lesson or Role.Guide => (45, 120, 2.0, 4.5),
        _ => (60, 120, 1.7, 2.8),
    };

    static HashSet<string> TypesOf(LevelData l)
    {
        var t = new HashSet<string>();
        if (l.Nails.Count > 0) t.Add("lock");
        if (l.Pieces.Any(p => p.Length > 1)) t.Add("chain");
        if (l.Sealed.Values.Sum(v => v.Length) > 0) t.Add("wall");
        if (l.Bombs.Count > 0) t.Add("bomb");
        return t;
    }

    static string Geometry(LevelData l)
    {
        var sb = new StringBuilder($"{l.Width}x{l.Height}:");
        foreach (var p in l.Pieces) { foreach (var c in p) sb.Append(c.X).Append(',').Append(c.Y).Append(';'); sb.Append('|'); }
        for (int y = 0; y < l.Height; y++) for (int x = 0; x <= l.Width; x++) sb.Append(l.VJoint(Math.Min(x, l.Width), y));
        return sb.ToString();
    }

    /* The board itself (the pieces' cells), whatever its obstacles: one base is used once in the set. */
    static string Board(LevelData l)
    {
        var sb = new StringBuilder($"{l.Width}x{l.Height}:");
        foreach (var p in l.Pieces.Select(p => string.Join(";", p.Select(c => c.X + "," + c.Y))).OrderBy(x => x, StringComparer.Ordinal)) sb.Append(p).Append('|');
        for (int y = 0; y < l.Height; y++) for (int x = 1; x < l.Width; x++) sb.Append(l.VJoint(x, y) + 1);
        for (int y = 1; y < l.Height; y++) for (int x = 0; x < l.Width; x++) sb.Append(l.HJoint(x, y) + 1);
        return sb.ToString();
    }

    static string BombSig(LevelData l)
    {
        if (l.Bombs.Count == 0) return "";
        var parts = new List<string>();
        foreach (var kv in l.Bombs.OrderBy(k => k.Key))
        {
            var c = l.Pieces[kv.Key][0];
            int gx = c.X * 3 / l.Width, gy = c.Y * 3 / l.Height;
            parts.Add($"{gx}{gy}f{Math.Min(9, kv.Value / 4)}");
        }
        parts.Sort();
        return l.Bombs.Count + ":" + string.Join(",", parts);
    }

    static LevelData Strip(LevelData l, string type)
    {
        var v = l.CopyNextVersion();
        if (type == "lock") v.Nails = new Dictionary<int, int>();
        if (type == "bomb") v.Bombs = new Dictionary<int, int>();
        if (type == "wall") v.Sealed = new Dictionary<Dir, int[]>();
        return v;
    }

    static readonly List<Cand> Pool = new List<Cand>();
    static readonly HashSet<string> Seen = new HashSet<string>();

    static void Add(LevelData l, string source, bool played, bool generated = false)
    {
        if (l.Pieces.Count < 9) return;
        foreach (var subset in Subsets(TypesOf(l).Where(t => t != "chain").ToList()))
        {
            var v = l;
            foreach (var t in subset) v = Strip(v, t);
            string key = Geometry(v) + "#" + string.Join(",", TypesOf(v).OrderBy(x => x)) + "#" + v.Nails.Count + v.Bombs.Count + v.Sealed.Values.Sum(s => s.Length);
            if (!Seen.Add(key)) continue;
            Pool.Add(new Cand
            {
                Key = key, Source = source + (subset.Count > 0 ? " −" + string.Join("−", subset) : ""), BaseId = Board(l),
                Level = v, Types = TypesOf(v), Played = played && subset.Count == 0, BombSig = BombSig(v), Generated = generated,
            });
        }
    }

    static IEnumerable<List<string>> Subsets(List<string> items)
    {
        int n = items.Count;
        for (int m = 0; m < 1 << n; m++)
        {
            var s = new List<string>();
            for (int i = 0; i < n; i++) if ((m & 1 << i) != 0) s.Add(items[i]);
            yield return s;
        }
    }

    static IEnumerable<LevelData> Read(string path)
    {
        LevelSet set;
        try { set = LevelParser.Parse(File.ReadAllText(path)); }
        catch (Exception e) { Console.WriteLine($"skip {path}: {e.Message}"); yield break; }
        foreach (var l in set.Levels) yield return l;
    }

    public static void Run(string[] args)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var p22 = P21Onboarding.P22(LevelParser.Parse(File.ReadAllText("Assets/_Game/Levels/redesign.json")));
        foreach (var l in p22.Levels) Add(l, "p22", true);
        foreach (var (f, name) in new[] { ("Assets/_Game/Levels/redesign.json", "v4"), ("Assets/_Game/Levels/redesign_v3.json", "v3"), ("Assets/_Game/Levels/redesign_v2.json", "v2"),
                     ("Assets/_Game/Levels/levels.json", "web"), ("Assets/_Game/Levels/zeydbot.json", "zb"), ("Assets/_Game/Levels/designs.json", "ds") })
            foreach (var l in Read(f)) Add(l, name, false);
        foreach (var f in Directory.GetFiles("Assets/Resources/AjanSets", "*.json")) foreach (var l in Read(f)) Add(l, "ajan", false);
        foreach (var f in Directory.GetFiles("Builds/fabrika", "*.json", SearchOption.AllDirectories)) foreach (var l in Read(f)) Add(l, "fab", false);
        int gen = args.Length > 1 ? int.Parse(args[1]) : 0;
        if (gen > 0) Generate(gen);
        Console.WriteLine($"pool {Pool.Count} candidates ({sw.Elapsed.TotalSeconds:0} s)");
        // features once per candidate (the timer only moves the tempo term)
        System.Threading.Tasks.Parallel.ForEach(Pool, new System.Threading.Tasks.ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 2) }, c =>
        {
            try { c.F = DifficultyV2.Compute(c.Level, 600, 150); } catch (Exception) { c.F = null; }
        });
        Pool.RemoveAll(c => c.F == null);
        Console.WriteLine($"features {Pool.Count} ({sw.Elapsed.TotalSeconds:0} s)");
        var picks = Assign(p22);
        Write(picks, p22);
        Console.WriteLine($"done ({sw.Elapsed.TotalSeconds:0} s)");
    }

    /* New boards for the late slots (squares, 6x6-8x8, the decade's mix) from the generator. */
    static void Generate(int perKind)
    {
        var kinds = new List<(int w, int h, HashSet<string> types, Genre g)>();
        foreach (var size in new[] { (6, 6), (7, 7), (5, 5) })
            foreach (var combo in new[] { "bomb,chain", "lock,bomb", "wall,bomb", "chain,wall", "lock,chain,bomb", "wall,lock", "lock,chain,bomb,wall", "lock,chain" })
                foreach (var g in new[] { Genre.Orta, Genre.Zor, Genre.CokZor })
                    kinds.Add((size.Item1, size.Item2, new HashSet<string>(combo.Split(',')), g));
        int made = 0;
        var lockObj = new object();
        System.Threading.Tasks.Parallel.ForEach(kinds, new System.Threading.Tasks.ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 2) }, k =>
        {
            for (int i = 0; i < perKind; i++)
            {
                var spec = SlotSpec.For(40, k.g);
                spec.Boards = new[] { (k.w, k.h) };
                int cells = k.w * k.h;
                if (!k.types.Contains("lock")) spec.MinNails = spec.MaxNails = 0; else { spec.MinNails = 1; spec.MaxNails = Math.Max(1, spec.MaxNails); }
                if (!k.types.Contains("wall")) spec.MinSeals = spec.MaxSeals = 0; else { spec.MinSeals = 1; spec.MaxSeals = Math.Max(1, spec.MaxSeals); }
                if (!k.types.Contains("chain")) { spec.MinChains = spec.MaxChains = 0; spec.MinRemote = spec.MaxRemote = 0; } else { spec.MinChains = Math.Max(1, spec.MinChains); spec.MaxChains = Math.Max(spec.MinChains, spec.MaxChains); }
                if (!k.types.Contains("bomb")) spec.MinBombs = spec.MaxBombs = 0; else { spec.MinBombs = 1; spec.MaxBombs = Math.Max(1, spec.MaxBombs); }
                spec.MinBodies = cells - spec.MaxChains; spec.MaxBodies = cells - spec.MinChains;   // a chain joins two cells
                var p = new GenParams { Interlock = 1 + i % 3 * .5, EdgeFirst = i % 2 * .3, Knot = .3, Ease = .3 };
                Candidate c = null;
                try { c = Generator.Make(spec, p, (uint)(1000 + i * 7919 + k.w * 31 + (int)k.g * 101 + k.types.Count * 13), 20); } catch (Exception) { }
                if (c?.Level == null) continue;
                lock (lockObj) { Add(c.Level, $"gen{k.w}x{k.h}", false, true); made++; }
            }
        });
        Console.WriteLine($"generated {made}");
        foreach (var g in Pool.Where(c => c.Generated).GroupBy(c => $"{c.Level.Width}x{c.Level.Height} [{string.Join(",", c.Types.OrderBy(x => x))}]").OrderBy(g => g.Key))
            Console.WriteLine($"  gen {g.Key}: {g.Count()} (n {g.Min(c => c.N)}-{g.Max(c => c.N)})");
    }

    static List<Pick> Assign(LevelSet p22)
    {
        var picks = new Pick[101];
        var used = new HashSet<string>();
        // 1: the guided level (p20's level 1, set in by the game data step); 3, 7, 11, 21: p22's lessons as they are
        {
            var s41 = p22.Levels[0];
            var c1 = Pool.First(x => x.Played && x.Level.Id == s41.Id);
            picks[1] = new Pick { Slot = 1, Role = Role.Guide, C = c1, Timer = s41.TimerSeconds, P = DifficultyModelV2.PWin1(c1.F, (double)s41.TimerSeconds / c1.N), Lo = .9, Hi = 1, Target = .96, Considered = 1, Note = "p20'nin 1. bölümü (S41), rehberli" };
            used.Add(c1.BaseId);
        }
        var lessonFrom = new Dictionary<int, int> { [3] = 3, [7] = 7, [11] = 10, [21] = 14 };   // sl100 slot -> p22 slot (lock, bomb, chain, wall)
        // p22: 3 lock, 7 bomb, 10 chain, 14 wall; sl100: 3 lock, 7 chain, 11 bomb, 21 wall
        lessonFrom[7] = 10; lessonFrom[11] = 7;
        foreach (var kv in lessonFrom)
        {
            var l = p22.Levels[kv.Value - 1];
            var c = Pool.First(x => x.Played && x.Level.Id == l.Id);
            var band = Band(kv.Key, Role.Lesson);
            int timer = Math.Min(120, Math.Max(90, l.TimerSeconds));   // the clock stops in a lesson anyway
            picks[kv.Key] = new Pick { Slot = kv.Key, Role = Role.Lesson, C = c, Timer = timer, P = DifficultyModelV2.PWin1(c.F, (double)timer / c.N), Lo = band.lo, Hi = band.hi, Target = band.target, Considered = 1, Note = "p22 dersi aynen" };
            used.Add(c.BaseId);
        }
        // the order: the tightest slots first (very hard, hard, combos, breathers), then the rest
        var order = Enumerable.Range(1, 100).Where(s => picks[s] == null)
            .OrderBy(s => RoleOf(s) switch { Role.VeryHard => 0, Role.Hard => 1, Role.Combo => 2, Role.Guide => 3, Role.Breather => 4, Role.Learn => 5, _ => 6 })
            .ThenBy(s => s).ToList();
        foreach (int slot in order)
        {
            var role = RoleOf(slot);
            var (lo, hi, target) = Band(slot, role);
            var (nmin, nmax) = Size(slot, role);
            var (tmin, tmax, smin, smax) = Time(role);
            var allowed = Allowed(slot);
            var combo = ComboOf(slot);
            var theme = ThemeOf(slot);
            var options = new List<(Cand c, int timer, double p, double cost)>();
            var why = new Dictionary<string, int>();
            void W(string k) => why[k] = why.GetValueOrDefault(k) + 1;
            foreach (var c in Pool)
            {
                if (combo != null && !c.Types.SetEquals(combo)) { continue; }
                if (used.Contains(c.BaseId)) { W("used"); continue; }
                if (c.N < nmin || c.N > nmax) { W("size"); continue; }
                if (!c.Types.IsSubsetOf(allowed)) continue;
                if (slot == 2 && c.Types.Count > 0) continue;
                if (role == Role.Hard || role == Role.VeryHard) { if (slot >= 21 && c.Types.Count < 2) continue; }
                // the timer: model v2's first try closest to the target within the limits
                int t0 = (int)Math.Ceiling(Math.Max(tmin, smin * c.N)), t1 = (int)Math.Floor(Math.Min(tmax, smax * c.N));
                if (t0 > t1) continue;
                int best = -1; double bp = 0, bd = 9;
                for (int t = t0; t <= t1; t += 1)
                {
                    double p = c.Played && t == c.Level.TimerSeconds ? DifficultyModelV2.PWin1Blended(c.F, PlayData.Pt1) : DifficultyModelV2.PWin1(c.F, (double)t / c.N);
                    double d = Math.Abs(p - target);
                    if (d < bd - 1e-9) { bd = d; bp = p; best = t; }
                }
                if (best < 0) continue;
                bool inBand = bp >= lo - 1e-9 && bp <= hi + 1e-9;
                double cost = bd * 10 + (inBand ? 0 : 5);
                if (role == Role.VeryHard && bp < .15) continue;
                if (!c.Square) cost += slot <= 6 ? .1 : .6;
                if (theme != null && !c.Types.Overlaps(theme)) cost += role == Role.Breather ? .2 : 1.0;
                if (theme != null && c.Types.IsSupersetOf(theme)) cost -= .3;
                if (c.Played) cost -= .4;                              // pt1 data: keep what was played
                // variety with the neighbours already placed
                foreach (int n in new[] { slot - 1, slot + 1 })
                {
                    if (n < 1 || n > 100 || picks[n] == null) continue;
                    var o = picks[n].C;
                    if (o.Level.Width == c.Level.Width && o.Level.Height == c.Level.Height) cost += .8;
                    if (c.BombSig.Length > 0 && o.BombSig == c.BombSig) cost += 50;
                }
                // DifficultyModelV2's design rules (tempo is ours here; choke and jams count)
                var broken = DifficultyModelV2.Rules.Check(c.F, Math.Min(slot, 40), role == Role.Hard || role == Role.VeryHard ? DifficultyModelV2.Role.Hard : DifficultyModelV2.Role.Normal)
                    .Where(b => !b.StartsWith("sn/parça")).ToList();
                if (role != Role.Learn && role != Role.Breather && role != Role.Guide) cost += broken.Count * .7;
                options.Add((c, best, bp, cost));
            }
            options.Sort((a, b) => a.cost.CompareTo(b.cost));
            if (options.Count < 8) Console.WriteLine($"slot {slot} {role}: {options.Count} candidates; out: {string.Join(", ", why.Select(kv => kv.Key + " " + kv.Value))}");
            if (options.Count == 0) continue;
            var pick = options[0];
            used.Add(pick.c.BaseId);
            int considered = options.Count;
            picks[slot] = new Pick { Slot = slot, Role = role, C = pick.c, Timer = pick.timer, P = pick.p, Lo = lo, Hi = hi, Target = target, Considered = considered,
                Note = pick.p < lo - 1e-9 || pick.p > hi + 1e-9 ? "bant dışı (en yakın)" : "" };
        }
        return picks.Skip(1).Where(p => p != null).ToList();
    }

    static void Write(List<Pick> picks, LevelSet p22)
    {
        var csv = new StringBuilder("slot,role,level_id,source,w,h,pieces,locks,chains,walls,bombs,timer_s,sec_per_move,p_win1,band_lo,band_hi,candidates,note\n");
        var rows = new StringBuilder();
        foreach (var p in picks)
        {
            var l = p.C.Level; var t = p.C.Types;
            int chains = l.Pieces.Count(x => x.Length > 1), walls = l.Sealed.Values.Sum(v => v.Length);
            csv.AppendLine(string.Join(",", p.Slot, p.Role, l.Id, p.C.Source.Replace(",", ";"), l.Width, l.Height, p.C.N, l.Nails.Count, chains, walls, l.Bombs.Count,
                p.Timer, ((double)p.Timer / p.C.N).ToString("0.00", CultureInfo.InvariantCulture), p.P.ToString("0.000", CultureInfo.InvariantCulture),
                p.Lo.ToString("0.00", CultureInfo.InvariantCulture), p.Hi.ToString("0.00", CultureInfo.InvariantCulture), p.Considered, p.Note));
            Console.WriteLine($"{p.Slot,3} {p.Role,-8} {l.Id,-8} {p.C.Source,-22} {l.Width}x{l.Height} n{p.C.N,2} [{string.Join(",", t.OrderBy(x => x))}] T{p.Timer,3} p={p.P:0.00} ({p.Lo:0.00}-{p.Hi:0.00}) c{p.Considered} {p.Note}");
        }
        Directory.CreateDirectory("docs/analiz/v2");
        File.WriteAllText("docs/analiz/v2/pred_sl100_draft.csv", csv.ToString());
        // the picks as a level file (the game data step adds ids, images, unlocks, intros)
        var w = new JsonWriter();
        var sb = new StringBuilder("{\"format\":1,\"levels\":[\n");
        for (int i = 0; i < picks.Count; i++)
        {
            var p = picks[i];
            var lw = new JsonWriter();
            var l = p.C.Level.CopyNextVersion();
            l.TimerSeconds = p.Timer;
            LevelDraft.FromLevel(l).WriteJson(lw);
            string json = lw.ToString();
            json = json.Substring(0, json.Length - 1) + $",\"_orig_version\":{p.C.Level.Version},\"_orig_timer\":{p.C.Level.TimerSeconds},\"_played\":{(p.C.Played ? "true" : "false")},\"_slot\":{p.Slot},\"_role\":\"{p.Role}\",\"_source\":{new JsonWriter().Value(p.C.Source)},\"_p\":{p.P.ToString("0.000", CultureInfo.InvariantCulture)},\"_hard\":{(l.Hard ? "true" : "false")},\"_tutorial\":{l.Tutorial},\"_candidates\":{p.Considered}}}";
            sb.Append(json).Append(i + 1 < picks.Count ? ",\n" : "\n");
        }
        sb.Append("]}\n");
        File.WriteAllText("Builds/sl100_picks.json", sb.ToString());
    }
}
