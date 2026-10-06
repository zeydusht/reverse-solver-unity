using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* The web game's .veil + .card (index.html): a dark veil over the screen
       and a centred card with an optional icon tile, title, body, tip and
       full-width buttons (primary amber, the rest ghost). Every button is at
       least 46pt tall. Builders call Add* in order; Finish() sizes and places it. */
    public sealed class Card
    {
        public readonly Transform Root;
        public readonly List<(Rect rect, string id)> Buttons = new List<(Rect, string)>();

        readonly Vector2 _screen;
        readonly int _order;
        readonly float _w;
        readonly List<System.Func<float, float>> _rows = new List<System.Func<float, float>>();
        readonly Transform _content;

        public const float Pad = 22f, ButtonH = 46f;
        public float InnerWidth => _w - Pad * 2;

        public Card(Transform parent, Vector2 screen, int order, string name = "Card")
        {
            _screen = screen;
            _order = order;
            _w = Mathf.Min(340f, screen.x - 52f);
            Root = new GameObject(name).transform;
            Root.SetParent(parent, false);
            ShapeView.Create(Root, "veil", screen, 0, order).Radius(0).Fill(Draw.Rgba(8, 32, 34, .9f))
                .transform.localPosition = Draw.W(screen / 2);
            _content = new GameObject("content").transform;
            _content.SetParent(Root, false);
        }

        /* Rows are laid out top-down from y; each returns its height. */
        public Card Icon(System.Action<Transform, float, int> draw)
        {
            _rows.Add(y =>
            {
                // .card .ic: 52px, radius 14, gradient #63c2ef -> #2f8fd6, 3px white ring
                var tile = ShapeView.Create(_content, "icon", new Vector2(52, 52), 6, _order + 2).Radius(14)
                    .Gradient(Draw.Hex("#63c2ef"), Draw.Hex("#2f8fd6")).Stroke(Draw.Rgba(255, 255, 255, .9f), 3);
                tile.transform.localPosition = Draw.W(0, y + 26);
                var holder = new GameObject("glyph").transform;
                holder.SetParent(_content, false);
                holder.localPosition = Draw.W(0, y + 26);
                draw(holder, 30, _order + 3);
                return 52 + 12;
            });
            return this;
        }

        public Card Title(string text)
        {
            _rows.Add(y =>
            {
                TextView.Create(_content, "title", text, 20, Draw.Hex("#eaf3f2"), order: _order + 2)
                    .transform.localPosition = Draw.W(0, y + 12);
                return 24 + 6;
            });
            return this;
        }

        public Card Body(string text, Vector4? color = null, float sizePt = 13, float gapAfter = 18)
        {
            _rows.Add(y =>
            {
                var t = TextView.Create(_content, "body", text, sizePt, color ?? Draw.Hex("#8fb2b3"), false, order: _order + 2);
                var tmp = t.GetComponent<TextMeshPro>();
                tmp.textWrappingMode = TextWrappingModes.Normal;
                tmp.alignment = TextAlignmentOptions.Top;
                tmp.rectTransform.sizeDelta = new Vector2(InnerWidth, 10);
                tmp.ForceMeshUpdate();
                float h = Mathf.Max(sizePt * 1.4f, tmp.preferredHeight);
                tmp.rectTransform.sizeDelta = new Vector2(InnerWidth, h);
                t.transform.localPosition = Draw.W(0, y + h / 2);
                return h + gapAfter;
            });
            return this;
        }

        public Card Custom(float height, System.Action<Transform, float, float, int> draw)
        {
            _rows.Add(y => { draw(_content, y, InnerWidth, _order + 2); return height; });
            return this;
        }

        public Card Button(string id, string label, bool primary)
        {
            _rows.Add(y =>
            {
                float cy = y + ButtonH / 2;
                var b = ShapeView.Create(_content, "btn " + id, new Vector2(InnerWidth, ButtonH), 2, _order + 2).Radius(11);
                if (primary) b.Fill(Draw.Hex("#f0a742"));
                else b.Fill(new Vector4(0, 0, 0, 0)).Stroke(Draw.Rgba(255, 255, 255, .22f), 1);
                b.transform.localPosition = Draw.W(0, cy);
                TextView.Create(_content, "label " + id, label, 15, primary ? Draw.Hex("#33220a") : Draw.Hex("#8fb2b3"),
                                order: _order + 3).transform.localPosition = Draw.W(0, cy);
                Buttons.Add((new Rect(-InnerWidth / 2, y, InnerWidth, ButtonH), id));
                return ButtonH + 8;
            });
            return this;
        }

        /* Lays the rows out, centres the card on screen and converts button rects to screen points. */
        public Card Finish()
        {
            float y = 24;
            foreach (var row in _rows) y += row(y);
            float h = y + 18 - 8;
            float top = (_screen.y - h) / 2;
            _content.localPosition = Draw.W(_screen.x / 2, top);
            // .card: #14484c, 1px rgba(255,255,255,.16), radius 18
            ShapeView.Create(Root, "card", new Vector2(_w, h), 4, _order + 1)
                .Radius(18).Fill(Draw.Hex("#14484c")).Stroke(Draw.Rgba(255, 255, 255, .16f), 1)
                .transform.localPosition = Draw.W(_screen.x / 2, top + h / 2);
            for (int i = 0; i < Buttons.Count; i++)
            {
                var (r, id) = Buttons[i];
                Buttons[i] = (new Rect(r.x + _screen.x / 2, r.y + top, r.width, r.height), id);
            }
            Height = h;
            Top = top;
            return this;
        }

        public float Height { get; private set; }
        public float Top { get; private set; }
        public float Left => (_screen.x - _w) / 2;

        public string HitButton(Vector2 pt)
        {
            foreach (var (r, id) in Buttons) if (r.Contains(pt)) return id;
            return null;
        }

        public void Destroy() { if (Root != null) Object.Destroy(Root.gameObject); }
    }
}
