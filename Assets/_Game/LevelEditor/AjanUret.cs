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

namespace ReverseSolver.LevelEditor
{
    public static class AjanUret
    {
        public static void Run()
        {
            var r = JsonReader.Parse(File.ReadAllText(AjanJson.Arg("-rsRecipe")));
            string id = AjanJson.Get(r, "id").AsString;
            var shapes = r.TryGet("shapes", out var shapeList)
                ? shapeList.Items.Select(v => ((int)v[0].AsNumber, (int)v[1].AsNumber)).ToArray()
                : new[] { (AjanJson.Int(r, "w"), AjanJson.Int(r, "h")) };
            if (shapes.Length == 0 || shapes.Any(v => v.Item1 < 2 || v.Item1 > 12 || v.Item2 < 2 || v.Item2 > 12))
                throw new FormatException("Geçersiz shapes boyutları");
            int w = shapes[0].Item1, h = shapes[0].Item2;
            if (string.IsNullOrWhiteSpace(id) || w < 2 || w > 12 || h < 2 || h > 12 ||
                AjanJson.Int(r, "timer") < 5 || AjanJson.Int(r, "timer") > 600 ||
                AjanJson.Int(r, "seed_from") < 1 || AjanJson.Int(r, "seed_to") < AjanJson.Int(r, "seed_from") ||
                AjanJson.Int(r, "seed_to") == int.MaxValue || AjanJson.Int(r, "count") < 1)
                throw new FormatException("Geçersiz tarif boyut/süre/tohum/sayı");
            var spec = new SlotSpec { Slot = 40, Genre = AjanJson.GenreOf(AjanJson.Get(r, "genre").AsString),
                Boards = new[] { (w, h) }, MinBodies = 1, MaxBodies = w * h, MinBombs = 0, MaxBombs = 0 };
            (spec.MinSeals, spec.MaxSeals) = AjanJson.Range(r, "seals");
            (spec.MinChains, spec.MaxChains) = AjanJson.Range(r, "chains");
            (spec.MinNails, spec.MaxNails) = AjanJson.Range(r, "nails");
            var output = new List<string>();
            int tried = 0, count = AjanJson.Int(r, "count"), last = AjanJson.Int(r, "seed_to");
            for (int seed = AjanJson.Int(r, "seed_from"); seed <= last && output.Count < count; seed++)
            {
                tried++;
                // Failed seeds keep trying the current slot's shape. Accepted
                // candidates advance to the next shape deterministically.
                (w, h) = shapes[output.Count % shapes.Length];
                spec.Boards = new[] { (w, h) };
                spec.MaxBodies = w * h;
                // Factory's default GenParams; exactly one generator attempt per recipe seed.
                var c = Generator.TryMake(spec, new GenParams(), (uint)seed, null, out _);
                if (c == null) continue;
                var start = new GameSession(c.Level, 1, "ajan", new Mulberry32(1));
                if (TravelRule.FreePieces(start.Board).Count(p => start.NailAt(p) <= 0) < AjanJson.Int(r, "min_open_first")) continue;
                var earliest = Earliest(c.Level);
                var requests = AjanJson.Get(r, "bombs").Items;
                var chosen = new Dictionary<int, int>();
                var usedK = new HashSet<int>();
                bool Pick(int i)
                {
                    if (i == requests.Count) return true;
                    var range = AjanJson.Get(requests[i], "earliest");
                    string region = requests[i].TryGet("region", out var regionNode) ? regionNode.AsString : "any";
                    if (!new[] { "any", "top", "bottom", "left", "right", "center" }.Contains(region))
                        throw new FormatException("Geçersiz bomba bölgesi: " + region);
                    bool Preferred(int piece)
                    {
                        var cells = c.Level.Pieces[piece];
                        double x = cells.Average(cell => cell.X), y = cells.Average(cell => cell.Y);
                        switch (region)
                        {
                            case "top": return y < h / 2.0;
                            case "bottom": return y >= h / 2.0;
                            case "left": return x < w / 2.0;
                            case "right": return x >= w / 2.0;
                            case "center": return Math.Abs(x - (w - 1) / 2.0) <= w / 4.0 && Math.Abs(y - (h - 1) / 2.0) <= h / 4.0;
                            default: return true;
                        }
                    }
                    // Prefer region matches, then fall back to any legal piece.
                    foreach (int p in Enumerable.Range(0, earliest.Length).OrderBy(p => Preferred(p) ? 0 : 1).ThenBy(p => p))
                    {
                        int k = earliest[p];
                        int fuse = k + AjanJson.Int(requests[i], "margin");
                        if (chosen.ContainsKey(p) || usedK.Contains(k) || k < range[0].AsNumber || k > range[1].AsNumber ||
                            c.Level.Nails.ContainsKey(p) || fuse < 1 || fuse > LevelDraft.MaxCount || chosen.Values.Contains(fuse)) continue;
                        chosen[p] = fuse; usedK.Add(k);
                        if (Pick(i + 1))
                        {
                            // Try every assignment, not just the first geometric match.
                            var bd = c.Draft.Clone();
                            foreach (var bomb in chosen) bd.SetBomb(c.Level.Pieces[bomb.Key][0], bomb.Value);
                            var l = bd.ToLevelData();
                            if (SmartPlayer.Play(l).Verdict == SmartVerdict.Won) return true;
                        }
                        chosen.Remove(p); usedK.Remove(k);
                    }
                    return false;
                }
                if (!Pick(0)) continue;
                var bombDraft = c.Draft.Clone();
                foreach (var bomb in chosen) bombDraft.SetBomb(c.Level.Pieces[bomb.Key][0], bomb.Value);
                var level = bombDraft.ToLevelData();
                var path = SmartPlayer.Play(level);
                if (path.Verdict != SmartVerdict.Won) continue;
                var draft = LevelDraft.FromLevel(level);
                draft.Id = id + "_" + Suffix(output.Count); draft.Version = 1;
                draft.SetTimer(AjanJson.Int(r, "timer")); draft.Genre = spec.Genre;
                draft.Solution = path.Moves.ToList();
                var jw = new JsonWriter(); draft.WriteJson(jw);
                var metadata = new JsonWriter().BeginObject().Field("seed", seed).Key("bomb_k").BeginObject();
                foreach (int p in chosen.Keys) metadata.Field(p.ToString(CultureInfo.InvariantCulture), earliest[p]);
                metadata.EndObject().EndObject();
                var extra = new Dictionary<string, string> { ["ajan"] = metadata.ToString() };
                string fuse = string.Join(" / ", chosen.Values);
                foreach (string key in new[] { "intro", "bintro" })
                    if (r.TryGet(key, out var intro)) extra[key] = AjanJson.Serialize(intro, fuse);
                output.Add(AjanJson.Overlay(JsonReader.Parse(jw.ToString()), extra));
            }
            string result = "{\"format\":1,\"levels\":[" + string.Join(",", output) + "]}";
            if (output.Count > 0) LevelParser.Parse(result); // unknown ajan metadata is ignored by the parser
            AjanJson.Save(AjanJson.Arg("-rsOut"), result);
            UnityEngine.Debug.Log($"[ajan-uret] {id} denenen {tried}, geçen {output.Count}");
        }

        static string Suffix(int n) => n < 26 ? ((char)('a' + n)).ToString() : Suffix(n / 26 - 1) + Suffix(n % 26);

        // Exact minimum number of legal exits including the target, with bombs removed.
        // Iterative deepening + blocker closure: before a blocked target can leave, at
        // least one current blocker on an exit route must leave. Recursing through all
        // four routes includes every possible prerequisite. Nails can require unrelated
        // exits, so a nailed prerequisite expands to every present piece. No search cap
        // or SmartPlayer path index is mistaken for an exact minimum.
        public static int[] Earliest(LevelData source, ICollection<int> only = null)
        {
            var draft = LevelDraft.FromLevel(source);
            foreach (int p in source.Bombs.Keys) draft.SetBomb(source.Pieces[p][0], 0);
            var level = draft.ToLevelData();
            var result = Enumerable.Repeat(-1, level.Pieces.Count).ToArray();
            for (int target = 0; target < result.Length; target++)
            {
                if (only != null && !only.Contains(target)) continue;
                var initial = new GameSession(level, 1, "earliest", new Mulberry32(1));
                bool Search(GameSession s, int depth, Dictionary<string, int> seen)
                {
                    if (!s.Board.IsPresent(target)) return true;
                    if (depth == 0 || s.IsOver) return false;
                    string key = string.Join(",", s.Board.PresentPieces());
                    if (seen.TryGetValue(key, out int previous) && previous >= depth) return false;
                    seen[key] = depth;
                    var needed = new HashSet<int>();
                    void Visit(int p)
                    {
                        if (!needed.Add(p)) return;
                        // A free prerequisite can be removed immediately. Clearing a
                        // different route to it cannot shorten a monotone exit plan.
                        if (s.NailAt(p) <= 0 && TravelRule.CanExit(s.Board, p)) return;
                        if (s.NailAt(p) > 0)
                        { foreach (int other in s.Board.PresentPieces().ToArray()) if (other != p) Visit(other); }
                        foreach (Dir d in DirExt.All)
                        {
                            if (level.Sealed.TryGetValue(d, out var lanes) && level.Pieces[p].Any(cell =>
                                Array.IndexOf(lanes, d.IsVertical() ? cell.X : cell.Y) >= 0)) continue;
                            var limit = TravelRule.Limit(s.Board, p, d);
                            if (!limit.Exit && limit.Blocker >= 0) Visit(limit.Blocker);
                        }
                    }
                    Visit(target);
                    foreach (int p in needed.OrderBy(p => p == target ? 0 : 1).ThenBy(p => p))
                    {
                        if (s.NailAt(p) > 0) continue;
                        var dirs = TravelRule.ExitDirs(s.Board, p);
                        if (dirs.Count == 0) continue;
                        var copy = StructureMetrics.Copy(s);
                        if (copy.TryExit(p, dirs[0]) == CommandResult.Ok && copy.ValveCount == 0 && Search(copy, depth - 1, seen)) return true;
                    }
                    return false;
                }
                for (int depth = 1; depth <= result.Length; depth++)
                    if (Search(initial, depth, new Dictionary<string, int>())) { result[target] = depth; break; }
            }
            return result;
        }
    }

    internal static class AjanJson
    {
        public static string Arg(string key)
        {
            var args = Environment.GetCommandLineArgs(); int i = Array.IndexOf(args, key);
            if (i < 0 || i + 1 == args.Length) throw new ArgumentException("Eksik argüman: " + key);
            return args[i + 1];
        }
        public static JsonNode Get(JsonNode n, string key) => n.TryGet(key, out var v) ? v : throw new FormatException("Eksik alan: " + key);
        public static int Int(JsonNode n, string key) => (int)Get(n, key).AsNumber;
        public static (int, int) Range(JsonNode n, string key)
        { var v = Get(n, key); return v.Kind == JsonKind.Array ? ((int)v[0].AsNumber, (int)v[1].AsNumber) : ((int)v.AsNumber, (int)v.AsNumber); }
        public static Genre GenreOf(string g) => g == "normal" ? Genre.Orta : g == "super-zor" ? Genre.CokZor :
            GenreExt.TryParse(g, out var genre) ? genre : throw new FormatException("Bilinmeyen tip: " + g);
        public static void Save(string path, string json)
        { Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))); File.WriteAllText(path, json + "\n", new UTF8Encoding(false)); }
        public static string Number(double v) => double.IsNaN(v) || double.IsInfinity(v) ? "null" : v.ToString("R", CultureInfo.InvariantCulture);
        public static string Serialize(JsonNode n, string fuse = null)
        {
            var w = new JsonWriter();
            switch (n.Kind)
            {
                case JsonKind.Null: w.Null(); break;
                case JsonKind.Bool: w.Value(n.AsBool); break;
                case JsonKind.Number: w.Raw(Number(n.AsNumber)); break;
                case JsonKind.String: w.Value(fuse == null ? n.AsString : n.AsString.Replace("{F}", fuse)); break;
                case JsonKind.Array: w.BeginArray(); foreach (var v in n.Items) w.Raw(Serialize(v, fuse)); w.EndArray(); break;
                case JsonKind.Object: w.BeginObject(); foreach (var kv in n.Members) w.Key(kv.Key).Raw(Serialize(kv.Value, fuse)); w.EndObject(); break;
            }
            return w.ToString();
        }
        public static string Overlay(JsonNode n, Dictionary<string, string> extra)
        {
            var w = new JsonWriter().BeginObject();
            foreach (var kv in n.Members) if (!extra.ContainsKey(kv.Key)) w.Key(kv.Key).Raw(Serialize(kv.Value));
            foreach (var kv in extra) w.Key(kv.Key).Raw(kv.Value);
            return w.EndObject().ToString();
        }
    }
}
