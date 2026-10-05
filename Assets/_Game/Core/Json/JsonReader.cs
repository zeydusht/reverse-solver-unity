using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ReverseSolver.Core.Json
{
    public sealed class JsonException : Exception
    {
        public JsonException(string message) : base(message) { }
    }

    /* Recursive-descent JSON reader with no reflection, so managed stripping
       cannot remove anything it relies on. Strict RFC 8259: no comments, no
       trailing commas. Errors carry line and column. */
    public static class JsonReader
    {
        public static JsonNode Parse(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            var p = new Parser(text);
            p.SkipWhitespace();
            var root = p.ReadValue(0);
            p.SkipWhitespace();
            if (!p.AtEnd) throw p.Error("unexpected text after the JSON value");
            return root;
        }

        sealed class Parser
        {
            const int MaxDepth = 64;
            readonly string _s;
            int _i;

            public Parser(string s)
            {
                _s = s;
                if (_s.Length > 0 && _s[0] == '﻿') _i = 1;   // tolerate a UTF-8 BOM
            }

            public bool AtEnd => _i >= _s.Length;

            public JsonException Error(string what)
            {
                int line = 1, col = 1;
                for (int k = 0; k < _i && k < _s.Length; k++)
                {
                    if (_s[k] == '\n') { line++; col = 1; } else col++;
                }
                return new JsonException($"JSON error at line {line}, column {col}: {what}");
            }

            public void SkipWhitespace()
            {
                while (_i < _s.Length)
                {
                    char c = _s[_i];
                    if (c == ' ' || c == '\t' || c == '\n' || c == '\r') _i++;
                    else break;
                }
            }

            public JsonNode ReadValue(int depth)
            {
                if (depth > MaxDepth) throw Error("nesting too deep");
                if (AtEnd) throw Error("unexpected end of input");
                char c = _s[_i];
                switch (c)
                {
                    case '{': return ReadObject(depth);
                    case '[': return ReadArray(depth);
                    case '"': return new JsonNode(ReadString());
                    case 't': Expect("true"); return new JsonNode(true);
                    case 'f': Expect("false"); return new JsonNode(false);
                    case 'n': Expect("null"); return JsonNode.Null;
                    default:
                        if (c == '-' || (c >= '0' && c <= '9')) return new JsonNode(ReadNumber());
                        throw Error($"unexpected character '{c}'");
                }
            }

            void Expect(string word)
            {
                if (string.CompareOrdinal(_s, _i, word, 0, word.Length) != 0) throw Error($"expected '{word}'");
                _i += word.Length;
            }

            JsonNode ReadObject(int depth)
            {
                _i++;                                             // {
                var members = new List<KeyValuePair<string, JsonNode>>();
                SkipWhitespace();
                if (!AtEnd && _s[_i] == '}') { _i++; return new JsonNode(members); }
                while (true)
                {
                    SkipWhitespace();
                    if (AtEnd || _s[_i] != '"') throw Error("expected a string key");
                    string key = ReadString();
                    SkipWhitespace();
                    if (AtEnd || _s[_i] != ':') throw Error("expected ':'");
                    _i++;
                    SkipWhitespace();
                    members.Add(new KeyValuePair<string, JsonNode>(key, ReadValue(depth + 1)));
                    SkipWhitespace();
                    if (AtEnd) throw Error("unterminated object");
                    if (_s[_i] == ',') { _i++; continue; }
                    if (_s[_i] == '}') { _i++; return new JsonNode(members); }
                    throw Error("expected ',' or '}'");
                }
            }

            JsonNode ReadArray(int depth)
            {
                _i++;                                             // [
                var items = new List<JsonNode>();
                SkipWhitespace();
                if (!AtEnd && _s[_i] == ']') { _i++; return new JsonNode(items); }
                while (true)
                {
                    SkipWhitespace();
                    items.Add(ReadValue(depth + 1));
                    SkipWhitespace();
                    if (AtEnd) throw Error("unterminated array");
                    if (_s[_i] == ',') { _i++; continue; }
                    if (_s[_i] == ']') { _i++; return new JsonNode(items); }
                    throw Error("expected ',' or ']'");
                }
            }

            string ReadString()
            {
                _i++;                                             // opening quote
                StringBuilder sb = null;
                int runStart = _i;
                while (true)
                {
                    if (AtEnd) throw Error("unterminated string");
                    char c = _s[_i];
                    if (c == '"')
                    {
                        string tail = _s.Substring(runStart, _i - runStart);
                        _i++;
                        return sb == null ? tail : sb.Append(tail).ToString();
                    }
                    if (c < 0x20) throw Error("control character in string");
                    if (c != '\\') { _i++; continue; }

                    sb ??= new StringBuilder();
                    sb.Append(_s, runStart, _i - runStart);
                    _i++;
                    if (AtEnd) throw Error("unterminated escape");
                    char e = _s[_i++];
                    switch (e)
                    {
                        case '"': sb.Append('"'); break;
                        case '\\': sb.Append('\\'); break;
                        case '/': sb.Append('/'); break;
                        case 'b': sb.Append('\b'); break;
                        case 'f': sb.Append('\f'); break;
                        case 'n': sb.Append('\n'); break;
                        case 'r': sb.Append('\r'); break;
                        case 't': sb.Append('\t'); break;
                        case 'u':
                            if (_i + 4 > _s.Length) throw Error("truncated \\u escape");
                            if (!int.TryParse(_s.Substring(_i, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int code))
                                throw Error("bad \\u escape");
                            sb.Append((char)code);
                            _i += 4;
                            break;
                        default: throw Error($"bad escape '\\{e}'");
                    }
                    runStart = _i;
                }
            }

            // ASCII only: char.IsDigit also accepts other scripts' digits.
            static bool IsDigit(char c) => c >= '0' && c <= '9';

            double ReadNumber()
            {
                int start = _i;
                if (_s[_i] == '-') _i++;
                if (AtEnd) throw Error("bad number");
                if (_s[_i] == '0') _i++;
                else if (_s[_i] >= '1' && _s[_i] <= '9') { while (!AtEnd && IsDigit(_s[_i])) _i++; }
                else throw Error("bad number");
                if (!AtEnd && _s[_i] == '.')
                {
                    _i++;
                    if (AtEnd || !IsDigit(_s[_i])) throw Error("bad number");
                    while (!AtEnd && IsDigit(_s[_i])) _i++;
                }
                if (!AtEnd && (_s[_i] == 'e' || _s[_i] == 'E'))
                {
                    _i++;
                    if (!AtEnd && (_s[_i] == '+' || _s[_i] == '-')) _i++;
                    if (AtEnd || !IsDigit(_s[_i])) throw Error("bad number");
                    while (!AtEnd && IsDigit(_s[_i])) _i++;
                }
                return double.Parse(_s.Substring(start, _i - start), NumberStyles.Float, CultureInfo.InvariantCulture);
            }
        }
    }
}
