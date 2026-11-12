using System.Collections.Generic;
using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    // Passive T42 layers. GameRoot owns input, timing and the delayed Core command.
    sealed class P21BoosterView
    {
        const int Order = 3500;
        const float ScissorsTravelSeconds = .72f, ScissorsCutSeconds = .64f, ScissorsReturnSeconds = .72f;
        public const float ScissorsHitSeconds = ScissorsTravelSeconds + ScissorsCutSeconds / 2;
        public const float ScissorsDuration = ScissorsTravelSeconds + ScissorsCutSeconds + ScissorsReturnSeconds;
        public const float ClockTouchSeconds = .62f;
        const float IceBreakSeconds = 7f / 18, ClockRetractSeconds = .55f;
        const string ClockShield = "Clock/Layers/button-shield";
        MenuPainting _fillOwner;
        int _fillStep = -1;
        readonly Transform _root;
        readonly Vector2 _screen;
        readonly float _safeTop;
        readonly BoardView _board;
        readonly BoosterBar _bar;
        readonly Dictionary<string, Sprite> _sprites = new Dictionary<string, Sprite>();
        Transform _selection;
        SpriteRenderer _ring, _tool, _shadow, _burst, _trail, _cut, _clockShield, _clockGrip, _ice, _spark, _barFrame, _barFill;
        readonly ShapeView _clockArm, _clockBacking;
        readonly TextView _freezeSeconds;
        int _shownFreezeSeconds = -1;
        Booster? _selected;
        int _zone = -2, _language = -1;
        float _thawAge;
        bool _clockWasActive;
        public Rect ConfirmRect { get; private set; }

        public P21BoosterView(Transform parent, Vector2 screen, float safeTop, BoardView board, BoosterBar bar)
        {
            _root = new GameObject("P21 boosters").transform; _root.SetParent(parent, false);
            _screen = screen; _safeTop = safeTop; _board = board; _bar = bar;
            _tool = Layer(_root, "tool", Order + 10); _shadow = Layer(_root, "shadow", Order + 8);
            _burst = Layer(_root, "burst", Order + 11); _trail = Layer(_root, "trail", Order + 9);
            _cut = Layer(_root, "cut", Order + 10);
            _clockArm = ShapeView.Create(_root, "Jigo left arm", new Vector2(32, 13), 1, Order - 98)
                .Radius(6.5f).Gradient(Draw.Hex("#38C8BD"), Draw.Hex("#126963"));
            _clockArm.gameObject.SetActive(false);
            _clockBacking = ShapeView.Create(_root, "clock shield backing", Vector2.one * 32, 1, Order - 97)
                .Radius(16).Gradient(Draw.Hex("#694728"), Draw.Hex("#412918"));
            _clockBacking.gameObject.SetActive(false);
            _clockShield = Layer(_root, "Jigo clock shield", Order - 96);
            _clockGrip = Layer(_root, "Jigo left grip", Order - 95);
            _ice = Layer(_root, "time panel ice", Order - 94);
            _spark = Layer(_root, "shield contact", Order - 93);
            _barFrame = Layer(_root, "freeze channel", Order - 94); _barFill = Layer(_root, "freeze remaining", Order - 93);
            _fillOwner = _barFill.gameObject.AddComponent<MenuPainting>();
            _freezeSeconds = TextView.Create(_root, "Freeze seconds", "", 10 * Hud54.Scale(screen.x), Palette.Cream, order: Order - 92)
                .Heading(TextView.RimTeal);
            _freezeSeconds.gameObject.SetActive(false);
        }

        static SpriteRenderer Layer(Transform parent, string name, int order)
        {
            var r = Draw.Child<SpriteRenderer>(parent, name); r.sortingOrder = order; r.enabled = false; return r;
        }

        void Paint(SpriteRenderer r, string key, Vector2 point, Vector2 size, float alpha = 1, float angle = 0)
        {
            if (key == null || alpha <= 0) { r.enabled = false; return; }
            if (!_sprites.TryGetValue(key, out var sprite))
                _sprites[key] = sprite = key.StartsWith("T46/") || key.StartsWith("hud/")
                    ? Art.Get(key) : Resources.Load<Sprite>("Art/yeni/boosters21/" + key);
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
            float below = area.yMax + 32 * scale;
            bool confirmAbove = b == Booster.Wand && zone >= 0 && below + 24 * scale > _bar.RectOf(b).yMin;
            Paint(Layer(_selection, "icon", Order + 2), "Selection/Icons/" + b.Id(), new Vector2(_screen.x / 2, top), Vector2.one * (78 * scale));
            if (!confirmAbove)
            {
                Paint(Layer(_selection, "title badge", Order + 1), "Selection/title-badge", new Vector2(_screen.x / 2, top + 57 * scale), new Vector2(260, 48) * scale);
                var title = TextView.Create(_selection, "title", Texts.BoosterTitle21(b), 22 * scale, Palette.Cream, order: Order + 3).Heading(TextView.RimTeal);
                title.transform.localPosition = Draw.W(_screen.x / 2, top + 57 * scale);
                var hint = TextView.Create(_selection, "instruction", Texts.BoosterHint21(b), 13 * scale, Palette.Cream, order: Order + 3);
                hint.transform.localPosition = Draw.W(_screen.x / 2, top + 94 * scale);
            }
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
            var center = new Vector2(_screen.x / 2, confirmAbove ? area.yMin - 29 * scale : below);
            ConfirmRect = new Rect(center - new Vector2(82, 24) * scale, new Vector2(164, 48) * scale);
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
            var b = booster.Value;
            if (b == Booster.Scissors)
            {
                Scissors(age, target, vertical);
                return;
            }
            var start = _bar.RectOf(b).center;
            float travel = Mathf.Clamp01(age / .4f);
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

        void Scissors(float age, Vector2 target, bool vertical)
        {
            const float cutEnd = ScissorsTravelSeconds + ScissorsCutSeconds;
            float approach = Mathf.Clamp01(age / ScissorsTravelSeconds);
            float retreat = Mathf.Clamp01((age - cutEnd) / ScissorsReturnSeconds);
            float travel = Mathf.SmoothStep(0, 1, approach) * Mathf.SmoothStep(1, 0, retreat);
            float angle = vertical ? -90 : 0;
            var slot = _bar.RectOf(Booster.Scissors);
            float cutSize = Mathf.Clamp(_board.Cell * 1.22f, 78 * Hud54.Scale(_screen.x), 112 * Hud54.Scale(_screen.x));
            var seam = vertical ? Vector2.up : Vector2.right;
            var point = Vector2.Lerp(slot.center, target - seam * (cutSize * .24f), travel);
            float cut = (age - ScissorsTravelSeconds) / ScissorsCutSeconds;
            int pose = cut < .20f || cut >= 1 ? 1 : cut < .50f ? 2 : cut < .80f ? 3 : 4;
            float slotMix = Mathf.SmoothStep(0, 1, Mathf.InverseLerp(.01f, .2f, travel));
            if (slotMix < 1)
                Paint(_shadow, Art54.Boosters[(int)Booster.Scissors], point, slot.size * .84f, 1 - slotMix, angle * travel);
            Paint(_tool, Frame("Boosters/Scissors/walk", pose), point,
                Vector2.Lerp(slot.size * .84f, Vector2.one * cutSize, travel), slotMix, angle * travel);
            if (age >= ScissorsHitSeconds && age < cutEnd)
            {
                float snip = Mathf.InverseLerp(ScissorsHitSeconds, cutEnd, age);
                Paint(_cut, Frame("Boosters/Scissors/cut-line", 1 + Mathf.Min(5, (int)(snip * 6))),
                    target, new Vector2(_board.Cell, _board.Cell * .24f), 1 - snip, angle);
            }
        }

        public bool Clock(float age, double freezeLeft, bool active, bool stopped)
        {
            _clockShield.enabled = _clockGrip.enabled = _ice.enabled = _spark.enabled = _barFrame.enabled = _barFill.enabled = false;
            bool frozen = age >= ClockTouchSeconds && freezeLeft > 0 && !stopped;
            _freezeSeconds.gameObject.SetActive(frozen);
            _clockArm.gameObject.SetActive(false);
            _clockBacking.gameObject.SetActive(false);
            if (age < 0 || stopped)
            {
                _clockWasActive = false;
                _thawAge = 0;
                return false;
            }
            float s = Hud54.Scale(_screen.x);
            var center = Hud54.JigoCenter(_screen.x, _safeTop);
            var panel = Hud54.TimePanelCenter(_screen.x, _safeTop);
            bool reaching = age < ClockTouchSeconds;
            if (reaching || frozen)
            {
                _clockWasActive = frozen;
                _thawAge = 0;
            }
            else
            {
                if (!_clockWasActive) return false;
                if (active) _thawAge += Time.unscaledDeltaTime;
                if (_thawAge > IceBreakSeconds + ClockRetractSeconds) { _clockWasActive = false; return false; }
            }
            float reach = reaching
                ? Mathf.SmoothStep(0, 1, age / ClockTouchSeconds)
                : Mathf.SmoothStep(1, 0, Mathf.InverseLerp(IceBreakSeconds, IceBreakSeconds + ClockRetractSeconds, _thawAge));
            DrawClockShield(center, panel, s, reach);
            if (frozen)
            {
                Paint(_ice, "T46/clock/ice_overlay", panel, new Vector2(113, 96.67f) * s, .78f);
                if (age < ClockTouchSeconds + .25f)
                    Paint(_spark, Frame("T46/clock/spark", 1 + Mathf.Min(5, (int)((age - ClockTouchSeconds) * 24))),
                        panel - new Vector2(32, 15) * s, Vector2.one * 32 * s);
                var p = panel + new Vector2(0, 57.33f) * s;
                string bg = Art.Get("T46/clock/freeze_bar_bg") != null ? "T46/clock/freeze_bar_bg" : "Clock/Layers/bar-frame";
                string fill = Art.Get("T46/clock/freeze_bar_fill") != null ? "T46/clock/freeze_bar_fill" : "Clock/Layers/bar-fill";
                Paint(_barFrame, bg, p, new Vector2(100, 14) * s);
                float fraction = Mathf.Clamp01((float)(freezeLeft / GameSession.FreezeSeconds));
                int seconds = Mathf.CeilToInt((float)freezeLeft);
                if (seconds != _shownFreezeSeconds)
                {
                    _shownFreezeSeconds = seconds;
                    _freezeSeconds.Text = seconds.ToString();
                }
                _freezeSeconds.transform.localPosition = Draw.W(p);
                var fillSource = Art.Get("T46/clock/freeze_bar_fill");
                if (fillSource == null) Paint(_barFill, fill, p - new Vector2((1 - fraction) * 48 * s, 0), new Vector2(96 * fraction, 10) * s);
                else
                {
                    int step = Mathf.CeilToInt(fraction * 100);
                    if (step != _fillStep)
                    {
                        _fillStep = step;
                        if (_fillOwner.Owned != null) Object.Destroy(_fillOwner.Owned);
                        var rect = fillSource.rect; rect.width *= step / 100f;
                        _fillOwner.Owned = step > 0 ? Sprite.Create(fillSource.texture, rect, new Vector2(.5f, .5f), fillSource.pixelsPerUnit) : null;
                    }
                    _barFill.sprite = _fillOwner.Owned; _barFill.enabled = _barFill.sprite != null;
                    _barFill.transform.localPosition = Draw.W(p - new Vector2((1 - step / 100f) * 48 * s, 0));
                    _barFill.transform.localScale = new Vector3(96 * s / fillSource.bounds.size.x, 10 * s / fillSource.bounds.size.y, 1);
                }
            }
            else if (!reaching && _thawAge < IceBreakSeconds)
                Paint(_ice, Frame("T46/clock/ice_break", 1 + Mathf.Min(6, (int)(_thawAge * 18))), panel, new Vector2(113, 434f / 3) * s);
            return true;
        }

        void DrawClockShield(Vector2 center, Vector2 panel, float scale, float reach)
        {
            float turn = Mathf.Sin(reach * Mathf.PI);
            float diameter = Mathf.Lerp(Hud54.RestingShieldSize, 92, reach) * scale;
            var point = Vector2.Lerp(Hud54.ShieldRest(center, scale), panel, reach) - Vector2.up * (12 * scale * turn);
            var shoulder = center + new Vector2(25, 27) * scale;
            var hand = Hud54.ShieldGrip(point, diameter, turn);
            var arm = hand - shoulder;
            _clockArm.gameObject.SetActive(reach > .12f);
            _clockArm.transform.localPosition = Draw.W((shoulder + hand) / 2);
            _clockArm.transform.localRotation = Quaternion.Euler(0, 0, -Mathf.Atan2(arm.y, arm.x) * Mathf.Rad2Deg);
            _clockArm.transform.localScale = new Vector3((arm.magnitude + 8 * scale) / 32, scale, 1);
            float width = diameter * (1 - .48f * turn);
            _clockBacking.gameObject.SetActive(reach > .5f);
            _clockBacking.transform.localPosition = Draw.W(point);
            _clockBacking.transform.localScale = new Vector3(width, diameter, 32) * (.64f / 32);
            Paint(_clockShield, ClockShield, point, new Vector2(width, diameter), 1, -18 * reach);
            Paint(_clockGrip, "hud/jigo-shield-grip", hand, new Vector2(23, 16) * scale, 1, -48 * reach);
        }
    }
}
