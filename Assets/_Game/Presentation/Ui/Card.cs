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
        readonly string _t23;
        public bool T23 => _t23 != null;

        TextView CardText(Transform parent, string name, string text, float sizePt, Vector4 color,
                          bool bold = true, TextAlignmentOptions align = TextAlignmentOptions.Center, int order = 0)
        {
            var view = TextView.Create(parent, name, text, sizePt, color, bold, align, order);
            return T23 ? view.Font(TextView.TitleFont) : view;
        }

        public const float Pad = 22f;
        public static float ButtonH => Art.T14 ? 52f : 46f;   // Task 14 buttons have 28 pt rounded ends
        public float InnerWidth => _w - Pad * 2;

        /* Codex's popup card is cream: texts drawn for the dark card switch to dark ink there. */
        public static Vector4 Ink(Vector4 onDark, string onCream) => Art.Get("popup-card") != null ? Draw.Hex(onCream) : onDark;

        public Card(Transform parent, Vector2 screen, int order, string name = "Card", string artFamily = null)
        {
            _screen = screen;
            _order = order;
            string family = artFamily ?? (name == "Pause" ? Art.T23Pause : name == "Profile" ? Art.T23Profile : null);
            _t23 = family != null && Art.Get(Art.T23Panel(family)) != null ? family : null;
            _w = T23 ? Mathf.Min(335f, screen.x - 40f) : Mathf.Min(340f, screen.x - 52f);
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
        /* Cards with questions (👍/👎, 1-10) use the compact Task 14 buttons so they fit a 375x667 phone. */
        public bool Compact;
        // Both feedback questions can be visible together; retain every action at phone height.
        public bool Dense;
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
            if (T23 || Art.T14) { _banner = text; return this; }
            _rows.Add(y =>
            {
                CardText(_content, "title", text, 20, Ink(Draw.Hex("#eaf3f2"), "#0B2A36"), order: _order + 2)
                    .transform.localPosition = Draw.W(0, y + 12);
                return 24 + 6;
            });
            return this;
        }

        public Card Body(string text, Vector4? color = null, float sizePt = 13, float gapAfter = 18, int maxLines = 0)
        {
            _rows.Add(y =>
            {
                if (T23 && Compact) gapAfter = Mathf.Min(gapAfter, 2);
                sizePt = Mathf.Max(13, sizePt);
                var t = CardText(_content, "body", text, sizePt, color ?? (T23 ? Draw.Hex("#0F4F5A") : Ink(Draw.Hex("#8fb2b3"), "#0F4F5A")), false, order: _order + 2);
                var tmp = t.GetComponent<TextMeshPro>();
                tmp.textWrappingMode = TextWrappingModes.Normal;
                tmp.alignment = TextAlignmentOptions.Top;
                tmp.rectTransform.sizeDelta = new Vector2(InnerWidth, 10);
                tmp.ForceMeshUpdate();
                while (maxLines > 0 && tmp.textInfo.lineCount > maxLines && sizePt > 12)
                {
                    sizePt -= .5f;
                    t.SizePt = sizePt;
                    tmp.ForceMeshUpdate();
                }
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
                if (T23)
                {
                    float width = Dense ? 128 : _t23 == Art.T23Profile ? 190 : _t23 == Art.T23Pause ? 198 : primary ? 204 : 160;
                    width = Mathf.Min(width, InnerWidth);
                    float spriteHeight = width * 112f / 228f;
                    string spriteKey = primary ? Art.T23Primary : Art.T23Secondary;
                    var sprite = Art.Place(_content, spriteKey, new Vector2(width, spriteHeight), _order + 2);
                    if (sprite != null)
                    {
                        sprite.transform.localPosition = Draw.W(0, y + spriteHeight / 2);
                        _pressArt.Add((Buttons.Count, sprite, spriteKey + "-pressed"));
                    }
                    // PM_MESAJ_41: the title's style (>= 20 pt, thin rim, drop shadow); a .3 rim ate the glyphs on iPhone Safari
                    // PM_MESAJ_42: Baloo heading style, ~50 % of the button's inner height, fitted down to 70 %
                    float labelPt = spriteHeight * .3f;
                    CardText(_content, "label " + id, label, labelPt, Palette.Cream, order: _order + 3)
                        .Heading(primary ? TextView.RimGreen : TextView.RimTeal).FitWidth(width * .78f, labelPt * .7f)
                        .transform.localPosition = Draw.W(0, y + spriteHeight / 2 - 3);
                    float hitHeight = Mathf.Max(44, spriteHeight * .8f);
                    Buttons.Add((new Rect(-width / 2, y + (spriteHeight - hitHeight) / 2, width, hitHeight), id));
                    return spriteHeight + (_t23 == Art.T23Pause ? 13 : Compact ? 2 : 4);
                }
                if (Art.T15)   // PM_MESAJ_29 3.1: the compact (1-10) card too; its main button stays the big green one
                {
                    // Task 15 (as Codex's preview-pause.png): the button at its own 684x336 aspect, never stretched, ~60 % of
                    // the card, centred; cream label, dark rim (green #1E5A0E / teal #0B2A36), a 2 px shadow, 1.3x size
                    string key = primary ? "btn2-primary" : "btn2-secondary";
                    // PM_MESAJ_19: the compact (feedback) card keeps the same art, smaller
                    float bw = Mathf.Min(InnerWidth, _w * (Compact ? .5f : .6f)), bh = bw * 336f / 684f, by = y + bh / 2;
                    var art = Art.Place(_content, key, new Vector2(bw, bh), _order + 2);
                    if (art != null) { art.transform.localPosition = Draw.W(0, by); _pressArt.Add((Buttons.Count, art, key + "-pressed")); }
                    CardText(_content, "label " + id, label, Compact ? 18 : 22, Palette.Cream, true, order: _order + 3)
                        .Outline(primary ? Draw.Hex("#1E5A0E") : Palette.Navy, .3f).Shadow(Draw.Rgba(0, 0, 0, .55f), 2)
                        .transform.localPosition = Draw.W(0, by - 3);
                    Buttons.Add((new Rect(-bw / 2, y + bh * .1f, bw, bh * .8f), id));
                    return bh + 4;
                }
                float height = Compact ? 44 : ButtonH;
                float cy = y + height / 2;
                var b = ShapeView.Create(_content, "btn " + id, new Vector2(InnerWidth, height), 2, _order + 2).Radius(11);
                if (primary) b.Fill(Draw.Hex("#f0a742"));
                else b.Fill(new Vector4(0, 0, 0, 0)).Stroke(Draw.Rgba(255, 255, 255, .22f), 1);
                b.Skin(primary ? "primary-normal" : "secondary-normal");
                b.transform.localPosition = Draw.W(0, cy);
                CardText(_content, "label " + id, label, 15, Art.T14 ? Palette.Cream : primary ? Palette.Or(Draw.Hex("#33220a"), Palette.Navy) : Ink(Draw.Hex("#8fb2b3"), "#0B2A36"),
                                order: _order + 3).transform.localPosition = Draw.W(0, cy);
                Buttons.Add((new Rect(-InnerWidth / 2, y, InnerWidth, height), id));
                return height + 8;
            });
            return this;
        }

        /* PM_MESAJ_19: two small teal Task 15 buttons side by side (Play again · Menu). */
        public Card ButtonPair(params (string id, string label)[] items)
        {
            _rows.Add(y =>
            {
                const float gap = 8;
                int n = items.Length;
                float bw = (InnerWidth - gap * (n - 1)) / n, bh = bw * 336f / 684f, by = y + bh / 2;
                for (int i = 0; i < n; i++)
                {
                    float x = -InnerWidth / 2 + i * (bw + gap) + bw / 2;
                    string key = T23 ? Art.T23Secondary : "btn2-secondary";
                    var art = Art.Place(_content, key, new Vector2(bw, bh), _order + 2);
                    if (art != null) { art.transform.localPosition = Draw.W(x, by); _pressArt.Add((Buttons.Count, art, key + "-pressed")); }
                    var pairLabel = CardText(_content, "label " + items[i].id, items[i].label, T23 ? bh * .3f : 16, Palette.Cream, true, order: _order + 3);
                    if (T23) pairLabel.Heading(TextView.RimTeal).FitWidth(bw * .78f, bh * .21f); else pairLabel.Outline(Palette.Navy, .3f);
                    pairLabel.transform.localPosition = Draw.W(x, by - 2);
                    float hitH = T23 ? Mathf.Max(44, bh * .8f) : bh * .8f;
                    Buttons.Add((new Rect(x - bw / 2, y + (bh - hitH) / 2, bw, hitH), items[i].id));
                }
                return bh + 4;
            });
            return this;
        }

        /* PM_MESAJ_19: like / dislike as two small icon buttons (cream, teal rim) with a drawn thumb, no emoji font. */
        public Card ThumbRow(string upId, string downId)
        {
            _rows.Add(y =>
            {
                const float bw = 72, bh = 46, gap = 16;
                for (int i = 0; i < 2; i++)
                {
                    float x = (i == 0 ? -1 : 1) * (bw + gap) / 2, cy = y + bh / 2;
                    var b = ShapeView.Create(_content, "thumb " + i, new Vector2(bw, bh), 4, _order + 2).Radius(bh / 2)
                        .Fill(Palette.Cream).Stroke(Palette.Teal, 2.5f).Shadow(Draw.Rgba(11, 42, 54, .25f), 0, 2, 4);
                    b.transform.localPosition = Draw.W(x, cy);
                    var icon = new GameObject("icon").transform;
                    icon.SetParent(_content, false);
                    icon.localPosition = Draw.W(x, cy);
                    if (i == 1) icon.localRotation = Quaternion.Euler(0, 0, 180);
                    void Part(string n, Vector2 size, Vector2 at, float rad, float rot)
                    {
                        var p = ShapeView.Create(icon, n, size, 1, _order + 3).Radius(rad).Fill(Palette.TealDark);
                        p.transform.localPosition = Draw.W(at);
                        p.transform.localRotation = Quaternion.Euler(0, 0, rot);
                    }
                    Part("cuff", new Vector2(5, 13), new Vector2(-10, 4), 1.5f, 0);
                    Part("fist", new Vector2(14, 13), new Vector2(0, 4), 3.5f, 0);
                    Part("thumb", new Vector2(6, 12), new Vector2(-1, -6), 3f, -18);
                    Buttons.Add((new Rect(x - bw / 2, y, bw, bh), i == 0 ? upId : downId));
                }
                return bh + (T23 && Compact ? 2 : 10);
            });
            return this;
        }

        /* P20-E: two rows of five, 44 pt targets; the pending selection is filled. */
        public Card ChipRow(string idPrefix, int count, int selected = -1)
        {
            _rows.Add(y =>
            {
                float gap = T23 ? 7 : 6;
                const float h = 44;
                int columns = Mathf.Min(5, count);
                float cw = T23 ? 50 : (InnerWidth - gap * (columns - 1)) / columns;
                float width = columns * cw + (columns - 1) * gap;
                float step = T23 ? 50 : h + gap;
                for (int i = 1; i <= count; i++)
                {
                    float rowY = y + (i - 1) / columns * step, cy = rowY + h / 2;
                    float x = -width / 2 + (i - 1) % columns * (cw + gap) + cw / 2;
                    bool on = i == selected;
                    var c = ShapeView.Create(_content, "chip " + i, new Vector2(cw, h), 2, _order + 2).Radius(8)
                        .Fill(on ? Palette.Teal : Palette.Cream).Stroke(Palette.Teal, 1.5f);
                    c.transform.localPosition = Draw.W(x, cy);
                    if (T23) c.Skin(on ? Art.T23RatingSelected : Art.T23RatingNormal, false);
                    CardText(_content, "chip label " + i, i.ToString(), T23 ? 18 : 14, on ? Palette.Cream : Palette.Navy, true, order: _order + 3)
                        .transform.localPosition = Draw.W(x, cy);
                    Buttons.Add((new Rect(x - cw / 2, rowY, cw, h), idPrefix + i));
                }
                return (Mathf.CeilToInt((float)count / columns) - 1) * step + h + (T23 ? 4 : gap + 4);
            });
            return this;
        }

        public Card TextLink(string id, string label)
        {
            _rows.Add(y =>
            {
                CardText(_content, "link " + id, label, 13, T23 ? Palette.Navy : Ink(Palette.Cream, "#0B2A36"), false, order: _order + 3)
                    .transform.localPosition = Draw.W(0, y + 22);
                Buttons.Add((new Rect(-InnerWidth / 2, y, InnerWidth, 44), id));
                return 44;
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
                    CardText(_content, "label " + items[i].id, items[i].label, 15, Art.T14 ? Palette.Cream : Ink(Draw.Hex("#8fb2b3"), "#0B2A36"), order: _order + 3)
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
            float y = T23 ? (Compact ? 72 : 82) : Art.T14 ? (Compact ? 88 : 96) : 24;                                        // Task 14: rows start under the 80 pt banner
            foreach (var row in _rows) y += row(y);
            float h = T23 ? y + 12 : y + 18 - 8 + (Art.T14 ? (Compact ? 14 : 30) : Art.Get("popup-card") != null ? 18 : 0);   // the panel's rim needs room under the last button
            float top = (_screen.y - h) / 2;
            _content.localPosition = Draw.W(_screen.x / 2, top);
            // .card: #14484c, 1px rgba(255,255,255,.16), radius 18
            ShapeView.Create(Root, "card", new Vector2(_w, h), 4, _order + 1)
                .Radius(18).Fill(Draw.Hex("#14484c")).Stroke(Draw.Rgba(255, 255, 255, .16f), 1).Skin(T23 ? Art.T23Panel(_t23) : "popup-card")
                .transform.localPosition = Draw.W(_screen.x / 2, top + h / 2);
            // preview art (#7): 9-sliced frame over the panel's edge, under the content
            var frame = T23 || Art.Get("popup-card") != null ? null : Art.Place(Root, "frame", new Vector2(_w + 8, h + 8), _order + 1, sliced: true);
            if (frame != null) frame.transform.localPosition = Draw.W(_screen.x / 2, top + h / 2, -1);
            for (int i = 0; i < Buttons.Count; i++)
            {
                var (r, id) = Buttons[i];
                Buttons[i] = (new Rect(r.x + _screen.x / 2, r.y + top, r.width, r.height), id);
            }
            if (_banner != null)
            {
                var titleView = CardText(Root, "title", _banner, T23 ? 24 : 21, Palette.Cream, true, order: _order + 3);
                if (T23) titleView.Heading(TextView.RimTeal).FitWidth(_w - 70, 17); else titleView.Outline(Palette.Navy, .2f);
                titleView.transform.localPosition = Draw.W(_screen.x / 2, top + (T23 ? 35 : 40));
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
