using System.Collections.Generic;

namespace ReverseSolver.Core.Json
{
    public enum JsonKind { Null, Bool, Number, String, Array, Object }

    /* A parsed JSON value. Deliberately small: the level file is the only
       input, so this needs just enough to walk a tree and report where a value
       came from when it is the wrong shape. Objects keep their key order. */
    public sealed class JsonNode
    {
        public static readonly JsonNode Null = new JsonNode(JsonKind.Null);

        public JsonKind Kind { get; }
        readonly bool _bool;
        readonly double _number;
        readonly string _string;
        readonly List<JsonNode> _items;
        readonly List<KeyValuePair<string, JsonNode>> _members;
        readonly Dictionary<string, JsonNode> _lookup;

        JsonNode(JsonKind kind) { Kind = kind; }
        public JsonNode(bool value) : this(JsonKind.Bool) { _bool = value; }
        public JsonNode(double value) : this(JsonKind.Number) { _number = value; }
        public JsonNode(string value) : this(JsonKind.String) { _string = value; }
        public JsonNode(List<JsonNode> items) : this(JsonKind.Array) { _items = items; }
        public JsonNode(List<KeyValuePair<string, JsonNode>> members) : this(JsonKind.Object)
        {
            _members = members;
            _lookup = new Dictionary<string, JsonNode>(members.Count);
            foreach (var m in members) _lookup[m.Key] = m.Value;   // last duplicate wins, as in JS
        }

        public bool IsNull => Kind == JsonKind.Null;
        public bool AsBool => _bool;
        public double AsNumber => _number;
        public string AsString => _string;
        public IReadOnlyList<JsonNode> Items => _items;
        public IReadOnlyList<KeyValuePair<string, JsonNode>> Members => _members;
        public int Count => Kind == JsonKind.Array ? _items.Count : Kind == JsonKind.Object ? _members.Count : 0;

        public JsonNode this[int index] => _items[index];

        public bool TryGet(string key, out JsonNode value)
        {
            value = null;
            return Kind == JsonKind.Object && _lookup.TryGetValue(key, out value);
        }
    }
}
