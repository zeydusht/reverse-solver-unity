using System;
using System.Collections.Generic;
using ReverseSolver.Core.Json;

namespace ReverseSolver.Core
{
    public sealed class LevelFormatException : Exception
    {
        public LevelFormatException(string message) : base(message) { }
    }

    /* Reads levels.json into LevelData. Anything that would make the rules
       misbehave is rejected with the path of the offending value, so a bad
       edit fails loudly at load instead of producing an unplayable board. */
    public static class LevelParser
    {
        public const int SupportedFormat = 1;

        public static LevelSet Parse(string json)
        {
            JsonNode root;
            try { root = JsonReader.Parse(json); }
            catch (JsonException e) { throw new LevelFormatException(e.Message); }

            if (root.Kind != JsonKind.Object) throw Fail("", "the file must be a JSON object");
            int format = Int(Required(root, "format", ""), "format");
            if (format != SupportedFormat)
                throw Fail("format", $"unsupported format {format}; this build reads format {SupportedFormat}");

            var list = Required(root, "levels", "");
            if (list.Kind != JsonKind.Array || list.Count == 0) throw Fail("levels", "must be a non-empty array");

            var levels = new List<LevelData>(list.Count);
            var seen = new Dictionary<string, int>();
            for (int i = 0; i < list.Count; i++)
            {
                var level = ParseLevel(list[i], $"levels[{i}]");
                if (seen.TryGetValue(level.Id, out int first))
                    throw Fail($"levels[{i}].id", $"duplicate id '{level.Id}' (also levels[{first}])");
                seen.Add(level.Id, i);
                levels.Add(level);
            }
            return new LevelSet(format, levels);
        }

        static LevelData ParseLevel(JsonNode n, string at)
        {
            if (n.Kind != JsonKind.Object) throw Fail(at, "must be an object");

            if (!n.TryGet("id", out var idNode) || idNode.Kind != JsonKind.String || idNode.AsString.Trim().Length == 0)
                throw Fail(at + ".id", "missing or empty id");
            var l = new LevelData { Id = idNode.AsString };
            at = l.Id;                                   // later errors name the level

            l.Version = Int(Required(n, "version", at), at + ".version");
            if (l.Version < 1) throw Fail(at + ".version", "must be >= 1");
            l.Number = OptInt(n, "lv", at, 0);
            l.Width = Int(Required(n, "w", at), at + ".w");
            l.Height = Int(Required(n, "h", at), at + ".h");
            if (l.Width < 1 || l.Height < 1 || l.Width > 32 || l.Height > 32)
                throw Fail(at, $"board size {l.Width}x{l.Height} out of range");
            l.TimerSeconds = OptInt(n, "timer", at, 0);

            l.Pieces = ParsePieces(Required(n, "pieces", at), l, at + ".pieces");
            l.V = ParseJoints(Required(n, "vEdge", at), l.Height, l.Width + 1, at + ".vEdge");
            l.H = ParseJoints(Required(n, "hEdge", at), l.Height + 1, l.Width, at + ".hEdge");
            l.Sealed = ParseSealed(n, l, at);
            l.Nails = ParseCounters(n, "nails", l, at);
            l.Bombs = ParseCounters(n, "bombs", l, at);
            l.Solution = ParseSolution(n, l, at);

            l.Colors = OptIntArray(n, "colors", at);
            if (l.Colors.Length != 0 && l.Colors.Length != l.CellCount)
                throw Fail(at + ".colors", $"has {l.Colors.Length} entries, board has {l.CellCount} cells");
            l.Palette = OptStringArray(n, "palette", at);
            l.Art = OptString(n, "art");
            l.Image = OptString(n, "image");
            if (l.Image != null && (l.Image.Length == 0 || l.Image.IndexOfAny(new[] { '/', '\\', ':' }) >= 0))
                throw Fail(at + ".image", "must be a plain file name");
            l.Intro = ParseIntro(n, "intro", at);
            l.BoosterIntro = ParseIntro(n, "bintro", at);
            l.Boosters = ParseNamedInts(n, "boosters", at);
            l.Unlock = ParseNamedInts(n, "unlock", at);
            l.Load = OptInt(n, "load", at, 0);
            l.Open = OptInt(n, "open", at, 0);
            return l;
        }

        static List<Cell[]> ParsePieces(JsonNode n, LevelData l, string at)
        {
            if (n.Kind != JsonKind.Array || n.Count == 0) throw Fail(at, "must be a non-empty array");
            var owner = new int[l.CellCount];
            for (int k = 0; k < owner.Length; k++) owner[k] = -1;
            var pieces = new List<Cell[]>(n.Count);
            for (int p = 0; p < n.Count; p++)
            {
                var cellsNode = n[p];
                string pat = $"{at}[{p}]";
                if (cellsNode.Kind != JsonKind.Array || cellsNode.Count == 0) throw Fail(pat, "a piece needs at least one cell");
                var cells = new Cell[cellsNode.Count];
                for (int c = 0; c < cells.Length; c++)
                {
                    var xy = cellsNode[c];
                    if (xy.Kind != JsonKind.Array || xy.Count != 2) throw Fail($"{pat}[{c}]", "a cell is [x, y]");
                    var cell = new Cell(Int(xy[0], $"{pat}[{c}][0]"), Int(xy[1], $"{pat}[{c}][1]"));
                    if (!l.OnBoard(cell)) throw Fail($"{pat}[{c}]", $"cell {cell} is off the {l.Width}x{l.Height} board");
                    int idx = cell.Y * l.Width + cell.X;
                    if (owner[idx] >= 0) throw Fail($"{pat}[{c}]", $"cell {cell} already belongs to piece {owner[idx]}");
                    owner[idx] = p;
                    cells[c] = cell;
                }
                pieces.Add(cells);
            }
            return pieces;
        }

        static int[,] ParseJoints(JsonNode n, int rows, int cols, string at)
        {
            if (n.Kind != JsonKind.Array || n.Count != rows) throw Fail(at, $"must have {rows} rows");
            var grid = new int[rows, cols];
            for (int y = 0; y < rows; y++)
            {
                var row = n[y];
                if (row.Kind != JsonKind.Array || row.Count != cols) throw Fail($"{at}[{y}]", $"must have {cols} entries");
                for (int x = 0; x < cols; x++)
                {
                    var v = row[x];
                    if (v.IsNull) continue;                   // outer edge
                    int j = Int(v, $"{at}[{y}][{x}]");
                    if (j < -1 || j > 1) throw Fail($"{at}[{y}][{x}]", $"joint must be -1, 0 or 1, got {j}");
                    grid[y, x] = j;
                }
            }
            return grid;
        }

        static Dictionary<Dir, int[]> ParseSealed(JsonNode n, LevelData l, string at)
        {
            var result = new Dictionary<Dir, int[]>();
            if (!n.TryGet("sealed", out var s) || s.IsNull) return result;
            if (s.Kind != JsonKind.Object) throw Fail(at + ".sealed", "must be an object");
            foreach (var m in s.Members)
            {
                if (!DirExt.TryParse(m.Key, out var dir)) throw Fail($"{at}.sealed", $"unknown side '{m.Key}'");
                var lanes = IntArray(m.Value, $"{at}.sealed.{m.Key}");
                int max = dir.IsVertical() ? l.Width : l.Height;
                foreach (var lane in lanes)
                    if (lane < 0 || lane >= max) throw Fail($"{at}.sealed.{m.Key}", $"lane {lane} out of range 0..{max - 1}");
                result[dir] = lanes;
            }
            return result;
        }

        static Dictionary<int, int> ParseCounters(JsonNode n, string key, LevelData l, string at)
        {
            var result = new Dictionary<int, int>();
            if (!n.TryGet(key, out var o) || o.IsNull) return result;
            if (o.Kind != JsonKind.Object) throw Fail($"{at}.{key}", "must be an object of piece index -> count");
            foreach (var m in o.Members)
            {
                if (!int.TryParse(m.Key, out int piece) || piece < 0 || piece >= l.Pieces.Count)
                    throw Fail($"{at}.{key}", $"'{m.Key}' is not a piece index");
                int count = Int(m.Value, $"{at}.{key}.{m.Key}");
                if (count < 1) throw Fail($"{at}.{key}.{m.Key}", "count must be >= 1");
                result[piece] = count;
            }
            return result;
        }

        static List<Move> ParseSolution(JsonNode n, LevelData l, string at)
        {
            var moves = new List<Move>();
            if (!n.TryGet("solution", out var s) || s.IsNull) return moves;
            if (s.Kind != JsonKind.Array) throw Fail(at + ".solution", "must be an array");
            for (int i = 0; i < s.Count; i++)
            {
                string mat = $"{at}.solution[{i}]";
                int piece = Int(Required(s[i], "piece", mat), mat + ".piece");
                if (piece < 0 || piece >= l.Pieces.Count) throw Fail(mat, $"piece {piece} does not exist");
                var dirNode = Required(s[i], "dir", mat);
                if (dirNode.Kind != JsonKind.String || !DirExt.TryParse(dirNode.AsString, out var dir))
                    throw Fail(mat + ".dir", "must be U, D, L or R");
                moves.Add(new Move(piece, dir));
            }
            return moves;
        }

        static LevelIntro ParseIntro(JsonNode n, string key, string at)
        {
            if (!n.TryGet(key, out var o) || o.IsNull) return null;
            if (o.Kind != JsonKind.Object) throw Fail($"{at}.{key}", "must be an object or null");
            return new LevelIntro
            {
                Id = OptString(o, "id"), Icon = OptString(o, "icon"), Title = OptString(o, "title"),
                Body = OptString(o, "body"), Tip = OptString(o, "tip")
            };
        }

        static Dictionary<string, int> ParseNamedInts(JsonNode n, string key, string at)
        {
            var result = new Dictionary<string, int>();
            if (!n.TryGet(key, out var o) || o.IsNull) return result;
            if (o.Kind != JsonKind.Object) throw Fail($"{at}.{key}", "must be an object");
            foreach (var m in o.Members) result[m.Key] = Int(m.Value, $"{at}.{key}.{m.Key}");
            return result;
        }

        // ---- helpers -------------------------------------------------------------

        static LevelFormatException Fail(string at, string what) =>
            new LevelFormatException(at.Length == 0 ? what : $"{at}: {what}");

        static JsonNode Required(JsonNode n, string key, string at)
        {
            if (n.Kind != JsonKind.Object || !n.TryGet(key, out var v) || v.IsNull)
                throw Fail(at.Length == 0 ? key : $"{at}.{key}", "is missing");
            return v;
        }

        static int Int(JsonNode n, string at)
        {
            if (n.Kind != JsonKind.Number) throw Fail(at, "must be a number");
            double d = n.AsNumber;
            if (d != Math.Floor(d) || d < int.MinValue || d > int.MaxValue) throw Fail(at, $"must be an integer, got {d}");
            return (int)d;
        }

        static int OptInt(JsonNode n, string key, string at, int fallback) =>
            n.TryGet(key, out var v) && !v.IsNull ? Int(v, $"{at}.{key}") : fallback;

        static string OptString(JsonNode n, string key) =>
            n.TryGet(key, out var v) && v.Kind == JsonKind.String ? v.AsString : null;

        static int[] IntArray(JsonNode n, string at)
        {
            if (n.Kind != JsonKind.Array) throw Fail(at, "must be an array");
            var a = new int[n.Count];
            for (int i = 0; i < a.Length; i++) a[i] = Int(n[i], $"{at}[{i}]");
            return a;
        }

        static int[] OptIntArray(JsonNode n, string key, string at) =>
            n.TryGet(key, out var v) && !v.IsNull ? IntArray(v, $"{at}.{key}") : Array.Empty<int>();

        static string[] OptStringArray(JsonNode n, string key, string at)
        {
            if (!n.TryGet(key, out var v) || v.IsNull) return Array.Empty<string>();
            if (v.Kind != JsonKind.Array) throw Fail($"{at}.{key}", "must be an array");
            var a = new string[v.Count];
            for (int i = 0; i < a.Length; i++)
            {
                if (v[i].Kind != JsonKind.String) throw Fail($"{at}.{key}[{i}]", "must be a string");
                a[i] = v[i].AsString;
            }
            return a;
        }
    }
}
