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
        // PM_74: ribbonless titles (approved style); &hud=kurdele brings the PM_54 ribboned header back
        public static bool Ribbons, NavyTitles;
        readonly JigoPortrait54 _portrait;
        readonly JigoPortraitLive54 _live;
        // PM_66: with the live portrait the shield clock borrows the fixed (shieldless) face while the shield is away.
        public void ShowClockShield(bool away)
        {
            _portrait?.ShieldAway(away);
            if (_live == null || _portrait == null) return;
            _live.Visible(!away); _portrait.Visible(away);
        }
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
            // PM_74: the Pieces box shows only the count, big and centred (no puzzle icon)
            _left = Panel(root, 201f / 3, offset, order, out _piecesTitle, Lang.T("Pieces", "Parça"), .44f);
            _clock = Panel(root, 972f / 3, offset, order, out _timeTitle, Lang.T("Time", "Süre"));
            TimeBar(root, 972f / 3, offset, order, session);
            var center = JigoCenter(screen.x, safeTop);
            var windowSize = new Vector2(127, 104) * _scale;
            Art54.Stretch(root, Art54.Window, center, windowSize, order + 2);
            if (FixedPortrait || P21BoosterView.ShieldClock) _portrait = new JigoPortrait54(root, center, windowSize, order + 3, session);
            if (!FixedPortrait) _live = new JigoPortraitLive54(root, center, windowSize, order + 3, session);
            if (_live != null) _portrait?.Visible(false);
        }

        TextView Panel(Transform root, float x, float offset, int order, out TextView title, string label, float numberShare = .34f)
        {
            var shift = new Vector2(0, offset);
            var body = new Vector2(113, 290f / 3) * _scale;
            var center = new Vector2(x, (181 + 145) / 3f) * _scale + shift;
            Art54.Stretch(root, Art54.Panel, center, body, order + 1);
            if (Ribbons)
                Art54.Place(root, Art54.Header, new Vector2(x, (123 + 83) / 3f) * _scale + shift,
                    new Vector2(364f / 3, 166f / 3) * _scale, order + 2);
            else
            {
                // PM_74: the approved style has no ribbons: the title sits on a cream pill with the blue rim
                // (&hud=lacivert: a navy tab instead of the cream pill; both kept for Zeyd)
                var at = new Vector2(x, 62) * _scale + shift;
                ShapeView.Create(root, "title shadow", new Vector2(86, 26) * _scale, 2, order + 2).Radius(13 * _scale)
                    .Fill(Draw.Hex("#0D2C5E")).transform.localPosition = Draw.W(at + new Vector2(0, 2.5f * _scale));
                var pill = ShapeView.Create(root, "title pill", new Vector2(86, 26) * _scale, 2, order + 3).Radius(13 * _scale);
                if (NavyTitles) pill.Gradient(Draw.Hex("#1A3F7E"), Draw.Hex("#0D2C5E")).Stroke(Draw.Hex("#E3B54E"), 1.5f * _scale);
                else pill.Gradient(Draw.Hex("#FFF9E9"), Draw.Hex("#F3E2BC")).Stroke(Draw.Hex("#1D4C92"), 2.5f * _scale);
                pill.transform.localPosition = Draw.W(at);
            }
            title = TextView.Create(root, "title", label, 16 * _scale, Palette.Cream, order: order + 4).Heading(TextView.RimTeal);
            title.transform.localPosition = Draw.W(new Vector2(x, 62) * _scale + shift);
            var number = TextView.Create(root, "number", "", body.y * numberShare, Palette.Cream, order: order + 4)
                .Heading(TextView.RimTeal);   // same thick rim as the titles: cream fill alone vanishes on the cream body
            number.transform.localPosition = Draw.W(center);
            return number;
        }

        /* PM_75: a thin bar under the Time box, the share of the time left: green, yellow under half, red under a
           quarter. While the clock booster holds the time it turns ice blue, a light runs along it and a snowflake
           sits at its end (the pause reads on the bar too). */
        ShapeView _barTrack, _barFill, _barShine;
        SpriteRenderer _barFlake;
        Vector2 _barLeft;
        float _barW, _barH, _barShown = -1;
        int _timer;
        public const float BarWidth = 96, BarHeight = 6;

        void TimeBar(Transform root, float x, float offset, int order, GameSession session)
        {
            _timer = session != null ? session.Level.TimerSeconds : 0;
            if (_timer <= 0) return;
            var center = new Vector2(x, (181 + 145) / 3f) * _scale + new Vector2(0, offset);
            float boxBottom = center.y + 290f / 6 * _scale;
            _barW = BarWidth * _scale; _barH = BarHeight * _scale;
            var at = new Vector2(center.x, boxBottom - 2 * _scale);   // the box art ends ~8 pt above its frame: between the box and the band edge
            _barLeft = at - new Vector2(_barW / 2, 0);
            _barTrack = ShapeView.Create(root, "time bar", new Vector2(_barW + 2.4f * _scale, _barH + 2.4f * _scale), 2, order + 4)
                .Radius((_barH + 2.4f * _scale) / 2).Fill(Draw.Hex("#0A1E3F")).Stroke(Draw.Hex("#E3B54E"), 1.2f * _scale);
            _barTrack.transform.localPosition = Draw.W(at);
            _barFill = ShapeView.Create(root, "time bar fill", new Vector2(_barW, _barH), 1, order + 5).Radius(_barH / 2);
            _barShine = ShapeView.Create(root, "time bar shine", new Vector2(_barH * 2.4f, _barH), 1, order + 6).Radius(_barH / 2)
                .Fill(new Vector4(1, 1, 1, .55f));
            _barShine.gameObject.SetActive(false);
            _barFlake = Art.Place(root, "yeni/pm74/saat/snowflake", Vector2.one * 15 * _scale, order + 7);
            if (_barFlake == null) { var r = Resources.Load<Sprite>("Art/yeni/pm74/saat/snowflake"); if (r != null) { _barFlake = new GameObject("time bar flake").AddComponent<SpriteRenderer>(); _barFlake.transform.SetParent(root, false); _barFlake.sprite = r; _barFlake.sortingOrder = order + 7; _barFlake.transform.localScale = Vector3.one * (15 * _scale / r.bounds.size.x); } }
            if (_barFlake != null) _barFlake.enabled = false;
        }

        void UpdateTimeBar(int seconds)
        {
            if (_barFill == null) return;
            if (_untimed) { _barTrack.gameObject.SetActive(false); _barFill.gameObject.SetActive(false); _barShine.gameObject.SetActive(false); if (_barFlake != null) _barFlake.enabled = false; return; }
            float share = Mathf.Clamp01(seconds / (float)_timer);
            // ease the bar towards the time (whole seconds would make it step)
            _barShown = _barShown < 0 ? share : Mathf.MoveTowards(_barShown, share, Time.unscaledDeltaTime * Mathf.Max(.25f, 2f / _timer));
            float w = Mathf.Max(_barH, _barW * _barShown);
            _barFill.Resize(new Vector2(w, _barH), 1).Radius(_barH / 2);
            _barFill.transform.localPosition = Draw.W(_barLeft + new Vector2(w / 2, 0));
            Vector4 top, bottom;
            if (_frozen) { top = Draw.Hex("#CFF3FF"); bottom = Draw.Hex("#6FC6EE"); }
            else if (_barShown > .5f) { top = Draw.Hex("#8BE04E"); bottom = Draw.Hex("#3E9E26"); }
            else if (_barShown > .25f) { top = Draw.Hex("#FFE066"); bottom = Draw.Hex("#E3A21A"); }
            else { top = Draw.Hex("#FF7A66"); bottom = Draw.Hex("#D7392B"); }
            _barFill.Gradient(top, bottom);
            _barFill.gameObject.SetActive(_barShown > 0);
            _barShine.gameObject.SetActive(_frozen);
            if (_frozen)
            {
                float k = Mathf.Repeat(Time.unscaledTime / 1.1f, 1);
                _barShine.transform.localPosition = Draw.W(_barLeft + new Vector2(Mathf.Lerp(_barH, w - _barH, k), 0));
                _barShine.Alpha(Mathf.Sin(k * Mathf.PI));
            }
            if (_barFlake != null)
            {
                _barFlake.enabled = _frozen;
                _barFlake.transform.localPosition = Draw.W(_barLeft + new Vector2(w, 0));
                _barFlake.transform.localRotation = Quaternion.Euler(0, 0, Time.unscaledTime * 40);
            }
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
            UpdateTimeBar(0);
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
            UpdateTimeBar(t);
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
