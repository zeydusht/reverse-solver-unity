using System.Collections.Generic;
using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    // Passive T42 layers. GameRoot owns input, timing and the delayed Core command.
    sealed class P21BoosterView
    {
        const int Order = 3500;
        readonly Transform _root;
        readonly Vector2 _screen;
        readonly float _safeTop;
        readonly BoardView _board;
        readonly BoosterBar _bar;
        readonly Dictionary<string, Sprite> _sprites = new Dictionary<string, Sprite>();
        Transform _selection;
        SpriteRenderer _ring, _tool, _shadow, _burst, _trail, _cut, _clock, _flyingClock, _button, _barFrame, _barFill;
        Booster? _selected;
        int _zone = -2, _language = -1;
        float _clockEnded = -1;
        bool _clockWasActive;
        public Rect ConfirmRect { get; private set; }

        public P21BoosterView(Transform parent, Vector2 screen, float safeTop, BoardView board, BoosterBar bar)
        {
            _root = new GameObject("P21 boosters").transform; _root.SetParent(parent, false);
            _screen = screen; _safeTop = safeTop; _board = board; _bar = bar;
            _tool = Layer(_root, "tool", Order + 10); _shadow = Layer(_root, "shadow", Order + 8);
            _burst = Layer(_root, "burst", Order + 11); _trail = Layer(_root, "trail", Order + 9);
            _cut = Layer(_root, "cut", Order + 10);
            _clock = Layer(_root, "Jigo clock", Order - 96); _flyingClock = Layer(_root, "flying clock", Order - 95);
            _button = Layer(_root, "dropped button", Order - 95);
            _barFrame = Layer(_root, "freeze channel", Order - 94); _barFill = Layer(_root, "freeze remaining", Order - 93);
        }

        static SpriteRenderer Layer(Transform parent, string name, int order)
        {
            var r = Draw.Child<SpriteRenderer>(parent, name); r.sortingOrder = order; r.enabled = false; return r;
        }

        void Paint(SpriteRenderer r, string key, Vector2 point, Vector2 size, float alpha = 1, float angle = 0)
        {
            if (key == null || alpha <= 0) { r.enabled = false; return; }
            if (!_sprites.TryGetValue(key, out var sprite)) _sprites[key] = sprite = Resources.Load<Sprite>("Art/yeni/boosters21/" + key);
            r.sprite = sprite; r.enabled = sprite != null;
            if (sprite == null) return;
            r.transform.localPosition = Draw.W(point);
            r.transform.localRotation = Quaternion.Euler(0, 0, angle);
            r.transform.localScale = new Vector3(size.x / sprite.bounds.size.x, size.y / sprite.bounds.size.y, 1);
            r.color = new Color(1, 1, 1, alpha);
        }

        static string Frame(string path, int frame) => path + "/" + frame.ToString("00");
        public Rect ZoneRect(int index)
        {
            var r = WandRegion.Quadrant(_board.Session.Level, index);
            return new Rect(_board.Origin + new Vector2(r.X, r.Y) * _board.Cell, new Vector2(r.Width, r.Height) * _board.Cell);
        }

        public void Selection(Booster? selected, int zone, bool[] valid)
        {
            if (_selected == selected && _zone == zone && _language == Lang.Revision) return;
            _selected = selected; _zone = zone; _language = Lang.Revision;
            if (_selection != null)
            {
                var old = _selection;
                if (selected.HasValue) Object.Destroy(old.gameObject);
                else
                {
                    var shapes = old.GetComponentsInChildren<ShapeView>(); var sprites = old.GetComponentsInChildren<SpriteRenderer>();
                    var labels = old.GetComponentsInChildren<TextView>();
                    Tweens.Run(old, .1f, t =>
                    {
                        foreach (var shape in shapes) if (shape != null) shape.Alpha(1 - t);
                        foreach (var sprite in sprites) if (sprite != null) sprite.color = new Color(1, 1, 1, 1 - t);
                        foreach (var label in labels) if (label != null) label.Alpha = 1 - t;
                    }, () => { if (old != null) Object.Destroy(old.gameObject); });
                }
                _selection = null;
            }
            _ring = null; ConfirmRect = default;
            if (!selected.HasValue) return;
            _selection = new GameObject("selection").transform; _selection.SetParent(_root, false);
            var area = new Rect(_board.Origin, _board.Size);
            void Dim(Rect r)
            {
                if (r.width <= 0 || r.height <= 0) return;
                ShapeView.Create(_selection, "dim", r.size, 0, Order).Fill(Draw.Rgba(0, 0, 0, .72f))
                    .transform.localPosition = Draw.W(r.center);
            }
            Dim(new Rect(0, 0, _screen.x, area.yMin));
            Dim(new Rect(0, area.yMax, _screen.x, _screen.y - area.yMax));
            Dim(new Rect(0, area.yMin, area.xMin, area.height));
            Dim(new Rect(area.xMax, area.yMin, _screen.x - area.xMax, area.height));
            var b = selected.Value;
            float scale = Hud54.Scale(_screen.x), top = Mathf.Max(_safeTop + 40 * scale, area.yMin - 130 * scale);
            Paint(Layer(_selection, "icon", Order + 2), "Selection/Icons/" + b.Id(), new Vector2(_screen.x / 2, top), Vector2.one * (78 * scale));
            Paint(Layer(_selection, "title badge", Order + 1), "Selection/title-badge", new Vector2(_screen.x / 2, top + 57 * scale), new Vector2(260, 48) * scale);
            var title = TextView.Create(_selection, "title", Texts.BoosterTitle21(b), 22 * scale, Palette.Cream, order: Order + 3).Heading(TextView.RimTeal);
            title.transform.localPosition = Draw.W(_screen.x / 2, top + 57 * scale);
            var hint = TextView.Create(_selection, "instruction", Texts.BoosterHint21(b), 13 * scale, Palette.Cream, order: Order + 3);
            hint.transform.localPosition = Draw.W(_screen.x / 2, top + 94 * scale);
            var button = _bar.RectOf(b);
            Art54.Place(_selection, Art54.Bodies[1], button.center, button.size, Order + 1);
            Art54.Place(_selection, Art54.Boosters[(int)b], button.center, button.size * .84f, Order + 2);
            _ring = Layer(_selection, "sparkle ring", Order + 3);
            if (b != Booster.Wand) return;
            for (int i = 0; i < 4; i++)
            {
                var r = ZoneRect(i);
                Paint(Layer(_selection, "region", Order + 1), "Boosters/Wand/zone-" + (zone == i ? "selected" : "idle"), r.center, r.size);
                if (!valid[i]) ShapeView.Create(_selection, "unavailable", r.size, 0, Order + 2)
                    .Fill(Draw.Rgba(65, 65, 65, .65f)).transform.localPosition = Draw.W(r.center);
            }
            if (zone < 0) return;
            var center = new Vector2(_screen.x / 2, Mathf.Min(area.yMax + 40 * scale, _bar.RectOf(Booster.Wand).yMin - 45 * scale));
            ConfirmRect = new Rect(center - new Vector2(90, 35) * scale, new Vector2(180, 70) * scale);
            Paint(Layer(_selection, "confirm", Order + 3), "Boosters/Wand/confirm-normal", center, ConfirmRect.size);
            var confirm = TextView.Create(_selection, "confirm label", Texts.RegionShuffle, 24 * scale, Palette.Cream, order: Order + 4).Heading(TextView.RimTeal);
            confirm.transform.localPosition = Draw.W(center);
        }

        public void TickSelection()
        {
            if (_ring != null && _selected.HasValue)
            {
                var r = _bar.RectOf(_selected.Value);
                Paint(_ring, Frame("Selection/selected-ring", 1 + (int)(Time.unscaledTime * 12) % 6), r.center, r.size * 1.35f);
            }
        }

        public void Effect(Booster? booster, float age, Vector2 target, bool vertical)
        {
            _tool.enabled = _shadow.enabled = _burst.enabled = _trail.enabled = _cut.enabled = false;
            if (!booster.HasValue) return;
            var b = booster.Value; var start = _bar.RectOf(b).center;
            float arrive = b == Booster.Scissors ? .3f : .4f;
            float travel = Mathf.Clamp01(age / arrive);
            var point = Vector2.Lerp(start, target, travel) - Vector2.up * (Mathf.Sin(travel * Mathf.PI) * 100);
            float size = Mathf.Lerp(68, 136, travel);
            string path; float angle = 0, alpha = 1;
            if (b == Booster.Hammer)
            {
                path = age < .4f ? Frame("Boosters/Hammer/flight", 1 + Mathf.Min(3, (int)(age * 15)))
                    : age < .67f ? Frame("Boosters/Hammer/strike", 1 + Mathf.Min(2, (int)((age - .4f) * 30)))
                    : Frame("Boosters/Hammer/rebound-fall", 1 + Mathf.Min(3, (int)((age - .67f) * 12)));
                if (age >= .27f && age < .4f) point.y -= 30 * Mathf.Sin((age - .27f) / .13f * Mathf.PI / 2);
                if (age >= .4f && age < .47f) point.y -= 30 * (1 - (age - .4f) / .07f);
                if (age >= .67f)
                {
                    float t = (age - .67f) / .33f;
                    point += new Vector2(180 * t, -80 * Mathf.Sin(t * Mathf.PI) + 450 * t * t);
                    angle = -500 * t; alpha = 1 - t;
                }
                Paint(_shadow, "Boosters/Hammer/soft-shadow", Vector2.Lerp(start, target, travel) + new Vector2(0, 30), new Vector2(size, size * .35f), .55f * alpha);
                if (age >= .47f) Paint(_burst, Frame("Boosters/Hammer/impact", 1 + Mathf.Min(8, (int)((age - .47f) * 30))), target, Vector2.one * (_board.Cell * 3.8f), Mathf.Clamp01((1 - age) * 5));
            }
            else if (b == Booster.Scissors)
            {
                path = age < .55f ? Frame("Boosters/Scissors/walk", 1 + (int)(age * 14) % 4)
                    : Frame("Boosters/Scissors/exit", 1 + Mathf.Min(2, (int)((age - .55f) * 12)));
                if (age < .3f) Paint(_trail, "Boosters/Scissors/speed-trail", point, new Vector2(size * 1.5f, size * .5f), .7f, Mathf.Atan2(-(target.y - start.y), target.x - start.x) * Mathf.Rad2Deg);
                if (age >= .3f && age < .55f)
                {
                    var tip = target + (vertical ? Vector2.up : Vector2.right) * (_board.Cell * ((age - .3f) / .25f - .5f));
                    point = tip;
                    Paint(_cut, Frame("Boosters/Scissors/cut-line", 1 + Mathf.Min(5, (int)((age - .3f) * 25))), target, new Vector2(_board.Cell, _board.Cell * .24f), 1, vertical ? -90 : 0);
                }
                if (age >= .3f)
                    Paint(_burst, Frame("Boosters/Scissors/cut-bits", 1 + Mathf.Min(8, (int)((age - .3f) * 16))),
                        target + (vertical ? Vector2.up : Vector2.right) * (_board.Cell * (Mathf.Clamp01((age - .3f) / .25f) - .5f)),
                        Vector2.one * (_board.Cell * 3), Mathf.Clamp01((1 - age) * 5));
                if (age >= .55f) { float t = (age - .55f) / .45f; point.y += 260 * t * t; alpha = 1 - t; }
            }
            else
            {
                path = Frame("Boosters/Wand/tool", 1 + (int)(age * 8) % 3);
                if (age >= .4f && age < 1.2f)
                {
                    point += new Vector2(10 * Mathf.Sin(age * 18), -12 * Mathf.Sin(age * 9));
                    Paint(_burst, Frame("Boosters/Wand/touch-burst", 1 + Mathf.Min(8, (int)((age - .4f) * 16))), target, Vector2.one * (_board.Cell * 3.8f), Mathf.Clamp01((1.2f - age) * 3));
                    Paint(_trail, "Boosters/Wand/star-trail", point, new Vector2(180, 100), .8f);
                }
                if (age >= 1.2f) { float t = (age - 1.2f) / .3f; point.x += _screen.x * t; point.y -= 70 * t; angle = -140 * t; alpha = 1 - t; }
            }
            Paint(_tool, path, point, Vector2.one * size, Mathf.Clamp01(alpha), angle);
        }

        // Key canvases share a fixed portrait origin; the arm reaches right to the Time panel.
        public bool Clock(float age, double freezeLeft, bool stopped, float safeTop)
        {
            _clock.enabled = _flyingClock.enabled = _button.enabled = _barFrame.enabled = _barFill.enabled = false;
            if (age < 0) return false;
            float s = Hud54.Scale(_screen.x), offset = Mathf.Max(0, safeTop - 37 * s);
            var center = new Vector2(227, 103) * s + new Vector2(0, offset);
            string key;
            if (age < .18f) key = "release-button";
            else if (age < .4f) key = "take-clock";
            else if (age < .65f) key = Frame("reach", 1 + Mathf.Min(2, (int)((age - .4f) * 12)));
            else if (freezeLeft > 0 && !stopped)
            {
                key = Frame("hold", 1 + (int)(Time.unscaledTime * 8) % 2);
                _clockWasActive = true; _clockEnded = -1;
            }
            else
            {
                if (!_clockWasActive) return false;
                if (_clockEnded < 0) _clockEnded = Time.unscaledTime;
                float t = Time.unscaledTime - _clockEnded;
                if (t > .6f) { _clockWasActive = false; return false; }
                key = t < .25f ? Frame("retract", 1 + Mathf.Min(2, (int)(t * 12))) : t < .45f ? "recover-button" : "idle";
            }
            Paint(_clock, "Clock/Keys/" + key, center, new Vector2(284, 210) * s);
            if (age < .4f)
            {
                float t = Mathf.Clamp01(age / .4f);
                Paint(_flyingClock, "Clock/Layers/flying-clock", Vector2.Lerp(_bar.RectOf(Booster.Clock).center, new Vector2(245, 115) * s + new Vector2(0, offset), t), Vector2.one * (48 * s), 1 - Mathf.Clamp01((t - .8f) * 5));
                Paint(_button, "Clock/Layers/button-shield", new Vector2(225, 125 + t * 70) * s + new Vector2(0, offset), Vector2.one * (36 * s), 1 - t);
            }
            if (freezeLeft > 0 && !stopped)
            {
                var p = new Vector2(324, 150) * s + new Vector2(0, offset);
                Paint(_barFrame, "Clock/Layers/bar-frame", p, new Vector2(100, 22) * s);
                float fraction = Mathf.Clamp01((float)(freezeLeft / GameSession.ClockBonusSeconds));
                Paint(_barFill, "Clock/Layers/bar-fill", p - new Vector2((1 - fraction) * 47 * s, 0), new Vector2(94 * fraction, 16) * s);
            }
            return true;
        }
    }
}
