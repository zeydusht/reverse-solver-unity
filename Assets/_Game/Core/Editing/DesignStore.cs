using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ReverseSolver.Core.Json;

namespace ReverseSolver.Core.Editing
{
    /* The design set, Assets/_Game/Levels/designs.json: levels.json's format
       plus `nextId`, so a deleted design's id is never handed out again.

       Ids are D01, D02, ... and never change once given. Version rule
       (PRODUCT.md, level editor): a save that changes gameplay
       (LevelDraft.GameplayKey) raises the version by one, however many edits
       went into it; a save that only changes colours, text or the stored
       solution keeps it. A new design starts at version 1. */
    public sealed class DesignStore
    {
        public const string Prefix = "D";

        public int NextId { get; private set; } = 1;
        readonly List<LevelDraft> _designs = new List<LevelDraft>();
        public IReadOnlyList<LevelDraft> Designs => _designs;

        public static DesignStore Parse(string json)
        {
            var store = new DesignStore();
            if (string.IsNullOrWhiteSpace(json)) return store;
            JsonNode root;
            try { root = JsonReader.Parse(json); }
            catch (JsonException e) { throw new LevelFormatException(e.Message); }
            if (root.TryGet("nextId", out var n) && n.Kind == JsonKind.Number) store.NextId = (int)n.AsNumber;
            if (root.TryGet("levels", out var list) && list.Kind == JsonKind.Array && list.Count > 0)
                foreach (var l in LevelParser.Parse(json).Levels) store._designs.Add(LevelDraft.FromLevel(l));
            foreach (var d in store._designs) store.NextId = Math.Max(store.NextId, IdNumber(d.Id) + 1);
            return store;
        }

        public LevelDraft Find(string id)
        {
            foreach (var d in _designs) if (d.Id == id) return d;
            return null;
        }

        /* Stores a copy of the draft and returns it with its id and version set.
           The caller's draft gets the same id and version. */
        public LevelDraft Save(LevelDraft draft)
        {
            var old = draft.Id == null ? null : Find(draft.Id);
            if (old == null)
            {
                if (draft.Id == null || IdNumber(draft.Id) <= 0)
                {
                    draft.Id = Prefix + NextId.ToString("00", CultureInfo.InvariantCulture);
                    NextId++;
                }
                draft.Version = 1;
                var copy = draft.Clone();
                _designs.Add(copy);
                return copy;
            }
            draft.Version = old.GameplayKey() == draft.GameplayKey() ? old.Version : old.Version + 1;
            var saved = draft.Clone();
            _designs[_designs.IndexOf(old)] = saved;
            return saved;
        }

        public bool Delete(string id)
        {
            var d = Find(id);
            return d != null && _designs.Remove(d);
        }

        /* One design per line, so a change shows up as a one-line diff. */
        public string ToJson()
        {
            var sb = new StringBuilder();
            sb.Append("{\"format\":").Append(LevelParser.SupportedFormat)
              .Append(",\"source\":\"Reverse Solver level editor\"")
              .Append(",\"nextId\":").Append(NextId).Append(",\"levels\":[");
            for (int i = 0; i < _designs.Count; i++)
            {
                var w = new JsonWriter();
                _designs[i].WriteJson(w);
                sb.Append(i == 0 ? "\n" : ",\n").Append(w);
            }
            sb.Append(_designs.Count > 0 ? "\n]}\n" : "]}\n");
            return sb.ToString();
        }

        /* The set as the game reads it; null while there are no designs. */
        public LevelSet ToLevelSet() => _designs.Count == 0 ? null : LevelParser.Parse(ToJson());

        static int IdNumber(string id) =>
            id != null && id.StartsWith(Prefix, StringComparison.Ordinal) &&
            int.TryParse(id.Substring(Prefix.Length), NumberStyles.None, CultureInfo.InvariantCulture, out int n) ? n : 0;
    }
}
