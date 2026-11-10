using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    sealed class Hud54
    {
        public const float PulseAmount = .045f;
        readonly TextView _left, _clock, _piecesTitle, _timeTitle;
        readonly float _scale;
        readonly JigoPortrait54 _portrait;
        int _lastLeft = -1, _lastTime = -1;
        int _language;

        public static float Scale(float width) => width / 390f;
        static float TopOffset(float width, float safeTop) => Mathf.Max(0, safeTop - 37 * Scale(width));
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
            var center = new Vector2(195, 103) * _scale + new Vector2(0, offset);
            var windowSize = new Vector2(127, 104) * _scale;
            Art54.Stretch(root, Art54.Window, center, windowSize, order + 2);
            _portrait = new JigoPortrait54(root, center, windowSize, order + 3, session);
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
            _lastLeft = n; _left.Text = n.ToString();
        }

        public void SetTime(int seconds)
        {
            int t = Mathf.Max(0, seconds);
            if (t != _lastTime)
            {
                _lastTime = t;
                _clock.Text = $"{t / 60}:{t % 60:00}";
                _clock.Color = t <= JigoMood.WorriedSeconds ? Palette.Coral : Palette.Cream;
            }
            float pulse = t <= JigoMood.WorriedSeconds ? 1 + PulseAmount * Mathf.Sin(Time.unscaledTime * Mathf.PI * 2) : 1;
            _clock.transform.localScale = Vector3.one * pulse;
            if (_language != Lang.Revision)
            {
                _language = Lang.Revision; _piecesTitle.Text = Lang.T("Pieces", "Parça"); _timeTitle.Text = Lang.T("Time", "Süre");
            }
            _portrait.Update();
        }

        public void OnEvent(GameEvent e, GameSession session) => _portrait.OnEvent(e, session);
        public Vector3 ClockPosition => _clock.transform.localPosition;
    }
}
