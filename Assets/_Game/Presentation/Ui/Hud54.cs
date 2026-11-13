using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    sealed class Hud54
    {
        public const float PulseAmount = .045f;
        readonly TextView _left, _clock, _piecesTitle, _timeTitle;
        readonly float _scale;
        // PM_64: the PM_62 live portrait is the default; Codex's fixed arch portrait with &portre=sabit or &saat=kalkan.
        public static bool FixedPortrait;
        readonly JigoPortrait54 _portrait;
        readonly JigoPortraitLive54 _live;
        public void ShowClockShield(bool away) => _portrait?.ShieldAway(away);
        public void ShowClockPortrait(bool showing) { _portrait?.Visible(!showing); _live?.Visible(!showing); }
        bool _frozen, _untimed;
        public void SetFrozen(bool frozen) { _frozen = frozen; }
        int _lastLeft = -1, _lastTime = -1;
        int _language;

        public static float Scale(float width) => width / 390f;
        static float TopOffset(float width, float safeTop) => Mathf.Max(0, safeTop - 37 * Scale(width));
        public const float RestingShieldSize = 42;
        public static Vector2 JigoCenter(float width, float safeTop) =>
            new Vector2(195, 103) * Scale(width) + Vector2.up * TopOffset(width, safeTop);
        public static Vector2 TimePanelCenter(float width, float safeTop) =>
            new Vector2(324, 326f / 3) * Scale(width) + Vector2.up * TopOffset(width, safeTop);
        public static Vector2 ShieldRest(Vector2 center, float scale) => center + new Vector2(24, 30) * scale;
        public static Vector2 ShieldGrip(Vector2 center, float diameter, float turn) =>
            center + new Vector2(-.29f + .1f * turn, .20f) * diameter;
        public static float TopBottom(Vector2 screen, float safeTop) =>
            Mathf.Max(screen.y * .19f + 9 * Scale(screen.x), 168 * Scale(screen.x) + TopOffset(screen.x, safeTop));
        public static float BottomTop(Vector2 screen, float safeBottom) => screen.y - screen.y * .086f - safeBottom;
        public static float ButtonsTop(Vector2 screen, float safeBottom) => BottomTop(screen, safeBottom) - 15.5f * Scale(screen.x);

        public Hud54(Transform root, Vector2 screen, float safeTop, int order, GameSession session)
        {
            _scale = Scale(screen.x);
            float offset = TopOffset(screen.x, safeTop), bottom = TopBottom(screen, safeTop);
            Art54.Stretch(root, Art54.Top, new Vector2(screen.x / 2, bottom / 2), new Vector2(screen.x, bottom), order);
            _language = Lang.Revision;
            _left = Panel(root, 201f / 3, offset, order, out _piecesTitle, Lang.T("Pieces", "Parça"));
            _clock = Panel(root, 972f / 3, offset, order, out _timeTitle, Lang.T("Time", "Süre"));
            Art54.Place(root, Art54.Piece, new Vector2(39, 87) * _scale + new Vector2(0, offset), Vector2.one * (19 * _scale), order + 3);
            var center = JigoCenter(screen.x, safeTop);
            var windowSize = new Vector2(127, 104) * _scale;
            Art54.Stretch(root, Art54.Window, center, windowSize, order + 2);
            if (FixedPortrait) _portrait = new JigoPortrait54(root, center, windowSize, order + 3, session);
            else _live = new JigoPortraitLive54(root, center, windowSize, order + 3, session);
        }

        TextView Panel(Transform root, float x, float offset, int order, out TextView title, string label)
        {
            var shift = new Vector2(0, offset);
            var body = new Vector2(113, 290f / 3) * _scale;
            var center = new Vector2(x, (181 + 145) / 3f) * _scale + shift;
            Art54.Stretch(root, Art54.Panel, center, body, order + 1);
            Art54.Place(root, Art54.Header, new Vector2(x, (123 + 83) / 3f) * _scale + shift,
                new Vector2(364f / 3, 166f / 3) * _scale, order + 2);
            title = TextView.Create(root, "title", label, 16 * _scale, Palette.Cream, order: order + 4).Heading(TextView.RimTeal);
            title.transform.localPosition = Draw.W(new Vector2(x, 62) * _scale + shift);
            var number = TextView.Create(root, "number", "", body.y * .34f, Palette.Cream, order: order + 4)
                .Heading(TextView.RimTeal);   // same thick rim as the titles: cream fill alone vanishes on the cream body
            number.transform.localPosition = Draw.W(center);
            return number;
        }

        public void SetLeft(int n)
        {
            if (n == _lastLeft) return;
            if (_lastLeft >= 0 && n < _lastLeft && Art.P21Gameplay)
                Tweens.Run(_left, .2f, t => { if (_left != null) _left.transform.localScale = Vector3.one * Mathf.Lerp(1.15f, 1, t); });
            _lastLeft = n; _left.Text = n.ToString();
        }

        public void PauseTutorialTime()
        {
            _untimed = true;
            _clock.SizePt = 17 * _scale;
            _clock.Text = Lang.T("No limit", "Sınırsız");
            _clock.Color = Palette.Cream;
            _clock.transform.localScale = Vector3.one;
        }

        public void SetTime(int seconds)
        {
            if (_language != Lang.Revision)
            {
                _language = Lang.Revision;
                _piecesTitle.Text = Lang.T("Pieces", "Parça");
                _timeTitle.Text = Lang.T("Time", "Süre");
                if (_untimed) _clock.Text = Lang.T("No limit", "Sınırsız");
            }
            _portrait?.Update(); _live?.Update();
            if (_untimed) return;

            int t = Mathf.Max(0, seconds);
            if (t != _lastTime)
            {
                _lastTime = t;
                _clock.Text = $"{t / 60}:{t % 60:00}";
            }
            _clock.Color = _frozen ? Draw.Hex("#D8F5FF") : t <= JigoMood.WorriedSeconds ? Palette.Coral : Palette.Cream;
            float pulse = !_frozen && t <= JigoMood.WorriedSeconds ? 1 + PulseAmount * Mathf.Sin(Time.unscaledTime * Mathf.PI * 2) : 1;
            _clock.transform.localScale = Vector3.one * pulse;
        }

        public void OnEvent(GameEvent e, GameSession session) { _portrait?.OnEvent(e, session); _live?.OnEvent(e, session); }
        public Vector3 ClockPosition => _clock.transform.localPosition;
    }
}
