using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReverseSolver.Core.Json;

namespace ReverseSolver.Core.Tests
{
    /* Shared fixtures. Paths are relative to the project root, which is the
       working directory when the Unity test runner executes. */
    static class TestData
    {
        public const string LevelsPath = "Assets/_Game/Levels/levels.json";
        public const string GoldenPath = "Assets/_Game/Tests/EditMode/Golden/travel_golden.json";

        static LevelSet _levels;
        static JsonNode _raw, _golden;

        public static LevelSet Levels => _levels ??= LevelParser.Parse(File.ReadAllText(LevelsPath));

        /* The level file as a plain JSON tree, for checks that must not go through LevelParser. */
        public static JsonNode Raw => _raw ??= JsonReader.Parse(File.ReadAllText(LevelsPath));

        public static JsonNode Golden => _golden ??= JsonReader.Parse(File.ReadAllText(GoldenPath));

        public static IEnumerable<string> LevelIds() =>
            JsonReader.Parse(File.ReadAllText(LevelsPath)).TryGet("levels", out var l)
                ? l.Items.Select(n => { n.TryGet("id", out var id); return id.AsString; })
                : Enumerable.Empty<string>();

        public static JsonNode RawLevel(string id) =>
            Raw.TryGet("levels", out var l) ? l.Items.First(n => n.TryGet("id", out var i) && i.AsString == id) : null;

        public static JsonNode GoldenLevel(string id) =>
            Golden.TryGet("levels", out var l) ? l.Items.First(n => n.TryGet("id", out var i) && i.AsString == id) : null;

        public static JsonNode Get(this JsonNode n, string key) => n.TryGet(key, out var v) ? v : null;
        public static int Int(this JsonNode n) => (int)n.AsNumber;
    }
}
