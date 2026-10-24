using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ReverseSolver.Core.Json;
using ReverseSolver.Core;

namespace ReverseSolver.Editing
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
        /// <summary>Live view of the stored drafts; elements are mutable, not snapshots.</summary>
        /// <remarks>Clone an element before editing so Save can compare against the stored baseline.</remarks>
        public IReadOnlyList<LevelDraft> Designs => _designs;

        /// <summary>Loads a design set; blank input creates an empty store.</summary>
        /// <remarks>
        /// Restores nextId and raises it past all recognized D-number ids in the file.
        /// This recovery happens on parse, not when Save accepts a caller-supplied id.
        /// </remarks>
        public static DesignStore Parse(string json)
        {
            var store = new DesignStore();
            if (string.IsNullOrWhiteSpace(json)) return store;
            JsonNode root;
            try { root = JsonReader.Parse(json); }
            catch (JsonException e) { throw new LevelFormatException(e.Message); }
            if (root.TryGet("nextId", out var n) && n.Kind == JsonKind.Number) store.NextId = (int)n.AsNumber;
            if (root.TryGet("levels", out var list) && list.Kind == JsonKind.Array && list.Count > 0)
            {
                var parsed = LevelParser.Parse(json).Levels;
                for (int i = 0; i < parsed.Count; i++)
                {
                    var d = LevelDraft.FromLevel(parsed[i]);
                    // look-only fields the game does not read
                    if (list[i].TryGet("genre", out var gn) && gn.Kind == JsonKind.String && GenreExt.TryParse(gn.AsString, out var genre))
                        d.Genre = genre;
                    if (list[i].TryGet("imageCrop", out var c) && c.Kind == JsonKind.Object)
                        d.Crop = new ImageCrop(Num(c, "zoom", 1), Num(c, "x", 0), Num(c, "y", 0));
                    store._designs.Add(d);
                }
            }
            foreach (var d in store._designs) store.NextId = Math.Max(store.NextId, IdNumber(d.Id) + 1);
            return store;
        }

        static float Num(JsonNode o, string key, float fallback) =>
            o.TryGet(key, out var v) && v.Kind == JsonKind.Number ? (float)v.AsNumber : fallback;

        /// <summary>Returns the stored mutable draft, or null when the exact id is absent.</summary>
        /// <remarks>Use Clone before editing; in-place edits also change Save's version baseline.</remarks>
        public LevelDraft Find(string id)
        {
            foreach (var d in _designs) if (d.Id == id) return d;
            return null;
        }

        /// <summary>Stores a clone and updates the caller's id and version.</summary>
        /// <remarks>
        /// Pass a separately edited clone of the stored draft: version comparison uses
        /// the current stored GameplayKey, not a historical snapshot. The returned
        /// object is the stored clone itself, so clone it again before further edits.
        /// New ids start at version 1; existing ids retain or increment the stored
        /// version according to GameplayKey, regardless of the caller's version.
        /// A new valid D-number id supplied by the caller is retained without advancing
        /// NextId. Callers importing such ids must manage allocation to avoid collisions.
        /// This is an in-memory operation; persistence requires writing ToJson separately.
        /// </remarks>
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

        /// <summary>Removes an exact id in memory without reducing NextId; returns whether found.</summary>
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
