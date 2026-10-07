using System.Globalization;
using System.Text;

namespace ReverseSolver.Core.Json
{
    /* Minimal JSON writer for flat objects and arrays of them: the telemetry
       rows and the saved data. No reflection, so stripping cannot touch it. */
    public sealed class JsonWriter
    {
        readonly StringBuilder _sb = new StringBuilder();
        bool _first = true;

        public JsonWriter BeginObject() { Sep(); _sb.Append('{'); _first = true; return this; }
        public JsonWriter EndObject() { _sb.Append('}'); _first = false; return this; }
        public JsonWriter BeginArray() { Sep(); _sb.Append('['); _first = true; return this; }
        public JsonWriter EndArray() { _sb.Append(']'); _first = false; return this; }

        public JsonWriter Key(string k) { Sep(); Str(k); _sb.Append(':'); _first = true; return this; }

        public JsonWriter Value(string v) { Sep(); if (v == null) _sb.Append("null"); else Str(v); _first = false; return this; }
        public JsonWriter Value(int v) { Sep(); _sb.Append(v.ToString(CultureInfo.InvariantCulture)); _first = false; return this; }
        public JsonWriter Value(bool v) { Sep(); _sb.Append(v ? "true" : "false"); _first = false; return this; }
        /* Already-serialized JSON, inserted as is. */
        public JsonWriter Raw(string json) { Sep(); _sb.Append(json); _first = false; return this; }

        public JsonWriter Field(string k, string v) => Key(k).Value(v);
        public JsonWriter Field(string k, int v) => Key(k).Value(v);

        public override string ToString() => _sb.ToString();

        void Sep() { if (!_first) _sb.Append(','); _first = false; }

        void Str(string s)
        {
            _sb.Append('"');
            foreach (char c in s)
            {
                switch (c)
                {
                    case '"': _sb.Append("\\\""); break;
                    case '\\': _sb.Append("\\\\"); break;
                    case '\n': _sb.Append("\\n"); break;
                    case '\r': _sb.Append("\\r"); break;
                    case '\t': _sb.Append("\\t"); break;
                    default:
                        if (c < 0x20) _sb.Append("\\u").Append(((int)c).ToString("x4"));
                        else _sb.Append(c);
                        break;
                }
            }
            _sb.Append('"');
        }
    }
}
