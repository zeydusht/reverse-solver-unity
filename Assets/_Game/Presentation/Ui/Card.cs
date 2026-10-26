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

        public const float Pad = 22f;
        public static float ButtonH => Art.T14 ? 52f : 46f;   // Task 14 buttons have 28 pt rounded ends
        public float InnerWidth => _w - Pad * 2;

        /* Codex's popup card is cream: texts drawn for the dark card switch to dark ink there. */
        public static Vector4 Ink(Vector4 onDark, string onCream) => Art.Get("popup-card") != null ? Draw.Hex(onCream) : onDark;

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
                var tile = ShapeView.Create(_content, "icon", new Vector2(52, 52), 6, _order + 2).Radius(14).Skin("icon-tile", false)
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

        string _banner;
        readonly List<(int index, SpriteRenderer sr, string pressed)> _pressArt = new List<(int, SpriteRenderer, string)>();

        /* The pressed sprite of the Task 15 button under pt (true when there is one); the caller acts a moment later. */
        public bool Press(Vector2 pt)
        {
            foreach (var (i, sr, key) in _pressArt)
                if (i < Buttons.Count && Buttons[i].rect.Contains(pt))
                {
                    var s = Art.Get(key);
                    if (s != null) sr.sprite = s;
                    return true;
                }
            return false;
        }   // Task 14: the title sits on the panel's teal banner, not in the rows

        public Card Title(string text)
        {
            if (Art.T14) { _banner = text; return this; }
            _rows.Add(y =>
            {
                TextView.Create(_content, "title", text, 20, Ink(Draw.Hex("#eaf3f2"), "#0B2A36"), order: _order + 2)
                    .transform.localPosition = Draw.W(0, y + 12);
                return 24 + 6;
            });
            return this;
        }

        public Card Body(string text, Vector4? color = null, float sizePt = 13, float gapAfter = 18)
        {
            _rows.Add(y =>
            {
                var t = TextView.Create(_content, "body", text, sizePt, color ?? Ink(Draw.Hex("#8fb2b3"), "#0F4F5A"), false, order: _order + 2);
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
                if (Art.T15)
                {
                    // Task 15 (as Codex's preview-pause.png): the button at its own 684x336 aspect, never stretched, ~60 % of
                    // the card, centred; cream label, dark rim (green #1E5A0E / teal #0B2A36), a 2 px shadow, 1.3x size
                    string key = primary ? "btn2-primary" : "btn2-secondary";
                    float bw = Mathf.Min(InnerWidth, _w * .6f), bh = bw * 336f / 684f, by = y + bh / 2;
                    var art = Art.Place(_content, key, new Vector2(bw, bh), _order + 2);
                    if (art != null) { art.transform.localPosition = Draw.W(0, by); _pressArt.Add((Buttons.Count, art, key + "-pressed")); }
                    TextView.Create(_content, "label " + id, label, 22, Palette.Cream, true, order: _order + 3)
                        .Outline(primary ? Draw.Hex("#1E5A0E") : Palette.Navy, .3f).Shadow(Draw.Rgba(0, 0, 0, .55f), 2)
                        .transform.localPosition = Draw.W(0, by - 3);
                    Buttons.Add((new Rect(-bw / 2, y + bh * .1f, bw, bh * .8f), id));
                    return bh + 4;
                }
                float cy = y + ButtonH / 2;
                var b = ShapeView.Create(_content, "btn " + id, new Vector2(InnerWidth, ButtonH), 2, _order + 2).Radius(11);
                if (primary) b.Fill(Draw.Hex("#f0a742"));
                else b.Fill(new Vector4(0, 0, 0, 0)).Stroke(Draw.Rgba(255, 255, 255, .22f), 1);
                b.Skin(primary ? "primary-normal" : "secondary-normal");
                b.transform.localPosition = Draw.W(0, cy);
                TextView.Create(_content, "label " + id, label, 15, Art.T14 ? Palette.Cream : primary ? Palette.Or(Draw.Hex("#33220a"), Palette.Navy) : Ink(Draw.Hex("#8fb2b3"), "#0B2A36"),
                                order: _order + 3).transform.localPosition = Draw.W(0, cy);
                Buttons.Add((new Rect(-InnerWidth / 2, y, InnerWidth, ButtonH), id));
                return ButtonH + 8;
            });
            return this;
        }

        /* Buttons side by side, equal widths (the ZeydBot 1-10 question). */
        public Card ButtonRow(params (string id, string label)[] items)
        {
            _rows.Add(y =>
            {
                const float gap = 6;
                int n = items.Length;
                float bw = (InnerWidth - gap * (n - 1)) / n, cy = y + ButtonH / 2;
                for (int i = 0; i < n; i++)
                {
                    float x = -InnerWidth / 2 + i * (bw + gap);
                    var b = ShapeView.Create(_content, "btn " + items[i].id, new Vector2(bw, ButtonH), 2, _order + 2).Radius(11);
                    b.Fill(new Vector4(0, 0, 0, 0)).Stroke(Draw.Rgba(255, 255, 255, .22f), 1).Skin("secondary-normal");
                    b.transform.localPosition = Draw.W(x + bw / 2, cy);
                    TextView.Create(_content, "label " + items[i].id, items[i].label, 15, Art.T14 ? Palette.Cream : Ink(Draw.Hex("#8fb2b3"), "#0B2A36"), order: _order + 3)
                        .transform.localPosition = Draw.W(x + bw / 2, cy);
                    Buttons.Add((new Rect(x, y, bw, ButtonH), items[i].id));
                }
                return ButtonH + 8;
            });
            return this;
        }

        /* Lays the rows out, centres the card on screen and converts button rects to screen points. */
        public Card Finish()
        {
            float y = Art.T14 ? 96 : 24;                                        // Task 14: rows start under the 80 pt banner
            foreach (var row in _rows) y += row(y);
            float h = y + 18 - 8 + (Art.T14 ? 30 : Art.Get("popup-card") != null ? 18 : 0);   // the panel's rim needs room under the last button
            float top = (_screen.y - h) / 2;
            _content.localPosition = Draw.W(_screen.x / 2, top);
            // .card: #14484c, 1px rgba(255,255,255,.16), radius 18
            ShapeView.Create(Root, "card", new Vector2(_w, h), 4, _order + 1)
                .Radius(18).Fill(Draw.Hex("#14484c")).Stroke(Draw.Rgba(255, 255, 255, .16f), 1).Skin("popup-card")
                .transform.localPosition = Draw.W(_screen.x / 2, top + h / 2);
            // preview art (#7): 9-sliced frame over the panel's edge, under the content
            var frame = Art.Get("popup-card") != null ? null : Art.Place(Root, "frame", new Vector2(_w + 8, h + 8), _order + 1, sliced: true);
            if (frame != null) frame.transform.localPosition = Draw.W(_screen.x / 2, top + h / 2, -1);
            for (int i = 0; i < Buttons.Count; i++)
            {
                var (r, id) = Buttons[i];
                Buttons[i] = (new Rect(r.x + _screen.x / 2, r.y + top, r.width, r.height), id);
            }
            if (_banner != null)
                TextView.Create(Root, "title", _banner, 21, Palette.Cream, true, order: _order + 3).Outline(Palette.Navy, .2f)
                    .transform.localPosition = Draw.W(_screen.x / 2, top + 40);
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
