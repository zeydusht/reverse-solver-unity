using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_64 combo ladder (T47, docs/art/task47/LISTE.md). Tiers x2 Nice/Güzel, x3 Great/Harika, x4 Super/Süper,
       x5+ Amazing/Muhteşem. Rhythm: contact light and burst at the exit, the badge ~300 ms later (pops to ~1.15x and
       settles, ~650 ms visible), x4+ adds the board wave, a shock ring, screen edge and flash. A 1.2 s meter under the
       board shows the combo window; a jam breaks it. No score numbers (the game has none).
       Board-bound effects (burst, ring, wave) live under the board and shake with it; badge, meter, edge and flash
       live on a screen root that stays still. Pools are prewarmed: no loads or allocations while playing. */
    public sealed class ComboLadderView : MonoBehaviour
    {
        public const float Window = 1.2f, BadgeDelay = .3f, Fps = 24;
        static readonly string[] TierKeys = { "x2", "x3", "x4", "x5-plus" };
        static readonly float[] TierScale = { .77f, .87f, .96f, 1.04f };
        static readonly float[] HoldSeconds = { .18f, .21f, .235f, .27f };
        static readonly float[] BurstPx = { 280, 300, 312, 320 };
        static readonly float[] ShakePx = { .75f, 1.25f, 2, 3 }, ShakeSeconds = { .075f, .095f, .125f, .15f };
        static readonly float[] NumberPt = { 30.5f, 31, 32, 27 }, PraisePt = { 26.5f, 28.5f, 30, 31.5f };
        static readonly string[] NumberRims = { "#784213", "#883111", "#811F4C", "#512B78" };
        static readonly float[] InScale = { .46f, .91f, 1.15f, 1.04f, 1 }, InAngle = { -10, -6, 3, -1, 0 }, InY = { 10, 3, -7, -3, 0 };
        static readonly float[] OutScale = { 1, 1.07f, .76f, .24f }, OutAngle = { 0, 2, -4, -11 }, OutY = { 0, -5, -15, -23 };

        struct Anim
        {
            public SpriteRenderer View;
            public Sprite[] Frames;
            public float At, Life, Size, Alpha;
        }

        BoardView _board;
        Transform _screenRoot, _badgeRoot;
        float _k;                      // screen points per 390-wide design pixel
        Vector2 _screen;
        Sprite[][] _badgeIn = new Sprite[4][], _badgeOut = new Sprite[4][], _fill = new Sprite[4][];
        Sprite[] _small, _medium, _large, _ring, _wave, _edge, _break;
        Sprite _flash, _track, _ember;
        readonly Anim[] _anims = new Anim[8];
        int _nextAnim;
        SpriteRenderer _badge, _edgeView, _flashView, _trackView, _fillView, _emberView, _breakView;
        TextView _number, _praise;
        int _tier = -1, _armedTier = -1, _meterTier;
        float _badgeAt = -10, _hitAt = -10, _meterAt = -10, _breakAt = -10, _shakeAt = -10, _armedAt;
        float _shakePx, _shakeLife, _edgeAlpha, _flashAlpha;
        Vector2 _armedPoint, _badgeCenter;
        int _language = -1;

        public static ComboLadderView Create(BoardView board)
        {
            var v = Draw.Child<ComboLadderView>(board.transform, "Combo ladder");
            v._board = board;
            var cam = Camera.main;
            float h = cam != null ? cam.orthographicSize * 2 : 844;
            v._screen = new Vector2(cam != null ? h * cam.aspect : 390, h);
            v._k = v._screen.x / 390f;
            v._screenRoot = new GameObject("Combo ladder (screen)").transform;
            v._screenRoot.SetParent(board.transform.parent, false);
            v.Load();
            for (int i = 0; i < v._anims.Length; i++) v._anims[i] = new Anim { View = Layer(v.transform, "combo fx", 2195 + i), At = -1 };
            v._edgeView = Layer(v._screenRoot, "combo edge", 2230);
            v._flashView = Layer(v._screenRoot, "combo flash", 2231);
            v._trackView = Layer(v._screenRoot, "combo meter", 2240);
            v._fillView = Layer(v._screenRoot, "combo meter fill", 2241);
            v._emberView = Layer(v._screenRoot, "combo meter ember", 2242);
            v._breakView = Layer(v._screenRoot, "combo meter break", 2243);
            v._badgeRoot = new GameObject("combo badge").transform;
            v._badgeRoot.SetParent(v._screenRoot, false);
            v._badge = Layer(v._badgeRoot, "plate", 2260);
            v._number = TextView.Create(v._badgeRoot, "number", "", 30, Draw.Hex("#FFF5D6"), order: 2262);
            if (Art.Lilita != null) v._number.Font(Art.Lilita);
            v._praise = TextView.Create(v._badgeRoot, "praise", "", 28, Draw.Hex("#FFF5D6"), order: 2262);
            if (TextView.HeadingFont != null) v._praise.Font(TextView.HeadingFont);
            v._praise.Outline(Draw.Hex("#234670"), .22f);
            v._number.gameObject.SetActive(false); v._praise.gameObject.SetActive(false);
            return v;
        }

        static Sprite Get(string key) => Art.Get("T47/" + key);
        static Sprite[] Frames(string key, int count)
        {
            var frames = new Sprite[count];
            for (int i = 0; i < count; i++) frames[i] = Get(key + "/" + (i + 1).ToString("00"));
            return frames;
        }
        static SpriteRenderer Layer(Transform parent, string name, int order)
        {
            var r = Draw.Child<SpriteRenderer>(parent, name);
            r.sortingOrder = order; r.enabled = false; return r;
        }

        void Load()
        {
            for (int t = 0; t < 4; t++)
            {
                _badgeIn[t] = Frames("Badge/" + TierKeys[t] + "/in", 5);
                _badgeOut[t] = Frames("Badge/" + TierKeys[t] + "/out", 4);
            }
            _small = Frames("Burst/small", 8); _medium = Frames("Burst/medium", 10); _large = Frames("Burst/large", 12);
            _ring = Frames("Burst/shock-ring", 8); _wave = Frames("Board/wave", 8); _edge = Frames("Screen/edge", 6);
            _break = Frames("Meter/break", 3);
            _flash = Get("Screen/flash"); _track = Get("Meter/track"); _ember = Get("Meter/ember");
        }

        public static int TierOf(int combo) => combo < 2 ? -1 : Mathf.Min(combo, 5) - 2;
        public static string Praise(int tier) => tier switch
        {
            0 => Lang.T("Nice!", "Güzel!"),
            1 => Lang.T("Great!", "Harika!"),
            2 => Lang.T("Super!", "Süper!"),
            _ => Lang.T("Amazing!", "Muhteşem!")
        };

        /* A successful exit with this combo count; the burst waits for the piece to reach the edge (Hit). */
        public void Arm(int combo, Vector2 boardPoint)
        {
            _armedTier = TierOf(combo);
            _armedAt = Time.unscaledTime; _armedPoint = boardPoint;
            if (_armedTier < 0) return;
            _meterAt = Time.unscaledTime; _meterTier = _armedTier; _breakAt = -10;
        }

        /* The piece reaches the board edge: contact light and burst now, the badge BadgeDelay later. */
        public void Hit(Vector2 boardPoint)
        {
            if (_armedTier < 0) return;
            int tier = _armedTier; _armedTier = -1;
            float now = Time.unscaledTime;
            var burst = tier == 0 ? _small : tier == 1 ? _medium : _large;
            Play(burst, boardPoint, BurstPx[tier] * _k, burst.Length / Fps, 0);
            if (tier == 3) Play(_small, boardPoint + new Vector2(50, 0) * _k, BurstPx[0] * .6f * _k, _small.Length / Fps, .065f);
            if (tier >= 2)
            {
                Play(_ring, boardPoint, (tier == 2 ? 288 : 320) * _k, _ring.Length / Fps, .042f);
                // Wave canvas: the light trail runs 20 px inside, along the tray's inner border.
                Play(_wave, _board.Size / 2, Mathf.Max(_board.Size.x, _board.Size.y) + 56, _wave.Length / Fps, 0);
                _edgeAlpha = tier == 2 ? .65f : 1; _flashAlpha = tier == 2 ? .55f : 1;
            }
            else _edgeAlpha = _flashAlpha = 0;
            _hitAt = now;
            _tier = tier;
            _badgeAt = now + BadgeDelay;
            var exit = _board.Origin + boardPoint;
            float top = _board.Origin.y + 52 * _k, low = _board.Origin.y + _board.Size.y * .42f;
            _badgeCenter = new Vector2(_board.Origin.x + _board.Size.x / 2, Mathf.Clamp(exit.y - 140 * _k, top, Mathf.Max(top, low)));
            _shakeAt = now; _shakePx = ShakePx[tier]; _shakeLife = ShakeSeconds[tier];
            _language = -1;
        }

        /* A wrong move: the chain ends at once and the meter cracks. */
        public void Break()
        {
            bool meterOn = Time.unscaledTime - _meterAt < Window;
            _armedTier = -1; _meterAt = -10; _badgeAt = -10; _tier = -1;
            HideBadge();
            if (meterOn) _breakAt = Time.unscaledTime;
        }

        void Play(Sprite[] frames, Vector2 point, float size, float life, float delay)
        {
            ref var a = ref _anims[_nextAnim++ % _anims.Length];
            a.Frames = frames; a.At = Time.unscaledTime + delay; a.Life = life; a.Size = size; a.Alpha = 1;
            a.View.transform.localPosition = Draw.W(point);
        }

        void HideBadge()
        {
            _badge.enabled = false;
            _number.gameObject.SetActive(false); _praise.gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            foreach (var tier in _fill)
                if (tier != null) foreach (var s in tier) if (s != null) Destroy(s);
            if (_screenRoot != null) Destroy(_screenRoot.gameObject);
            if (_board != null) _board.transform.localPosition = Draw.W(_board.Origin);
        }

        void Update()
        {
            float now = Time.unscaledTime;
            if (_armedTier >= 0 && now - _armedAt > .45f) Hit(_armedPoint);   // the exit never reached an edge (boosters)
            for (int i = 0; i < _anims.Length; i++)
            {
                ref var a = ref _anims[i];
                if (a.At < 0) continue;
                float t = (now - a.At) / a.Life;
                if (t < 0) { a.View.enabled = false; continue; }
                if (t >= 1) { a.View.enabled = false; a.At = -1; continue; }
                var sprite = a.Frames[Mathf.Min((int)(t * a.Frames.Length), a.Frames.Length - 1)];
                a.View.sprite = sprite; a.View.enabled = sprite != null;
                if (sprite != null) a.View.transform.localScale = Vector3.one * (a.Size / sprite.bounds.size.x);
            }
            Shake(now);
            ScreenFx(now);
            Badge(now);
            Meter(now);
        }

        void Shake(float now)
        {
            float t = (now - _shakeAt) / Mathf.Max(.001f, _shakeLife);
            if (t < 0 || t > 1.5f) return;
            var home = Draw.W(_board.Origin);
            if (t >= 1) { _board.transform.localPosition = home; _shakeAt = -10; return; }
            float amp = _shakePx * _k * (1 - t);
            _board.transform.localPosition = home + new Vector3(Mathf.Sin(t * Mathf.PI * 6) * amp, Mathf.Sin(t * Mathf.PI * 4) * amp * .6f, 0);
        }

        void ScreenFx(float now)
        {
            float age = now - _hitAt;
            var center = Draw.W(_screen / 2);
            if (_edgeAlpha > 0 && age < .25f)
            {
                var sprite = _edge[Mathf.Min((int)(age / .25f * _edge.Length), _edge.Length - 1)];
                _edgeView.sprite = sprite; _edgeView.enabled = sprite != null;
                if (sprite != null)
                {
                    _edgeView.transform.localPosition = center;
                    _edgeView.transform.localScale = new Vector3(_screen.x / sprite.bounds.size.x, _screen.y / sprite.bounds.size.y, 1);
                    _edgeView.color = new Color(1, 1, 1, _edgeAlpha);
                }
            }
            else _edgeView.enabled = false;
            if (_flashAlpha > 0 && age < .1f && _flash != null)
            {
                _flashView.sprite = _flash; _flashView.enabled = true;
                _flashView.transform.localPosition = center;
                _flashView.transform.localScale = new Vector3(_screen.x / _flash.bounds.size.x, _screen.y / _flash.bounds.size.y, 1);
                float k = age < .025f ? age / .025f : 1 - (age - .025f) / .075f;
                _flashView.color = new Color(1, 1, 1, _flashAlpha * Mathf.Clamp01(k));
            }
            else _flashView.enabled = false;
        }

        void Badge(float now)
        {
            if (_tier < 0 || now < _badgeAt) { if (_tier < 0) HideBadge(); return; }
            float age = now - _badgeAt, inEnd = 5 / Fps, holdEnd = inEnd + HoldSeconds[_tier], outEnd = holdEnd + 4 / Fps;
            if (age >= outEnd) { _tier = -1; HideBadge(); return; }
            Sprite sprite; float scale, angle, y;
            if (age < inEnd)
            {
                int f = Mathf.Min(4, (int)(age * Fps));
                sprite = _badgeIn[_tier][f]; scale = InScale[f]; angle = InAngle[f]; y = InY[f];
            }
            else if (age < holdEnd) { sprite = _badgeIn[_tier][4]; scale = 1; angle = 0; y = 0; }
            else
            {
                int f = Mathf.Min(3, (int)((age - holdEnd) * Fps));
                sprite = _badgeOut[_tier][f]; scale = OutScale[f]; angle = OutAngle[f]; y = OutY[f];
            }
            _badgeRoot.localPosition = Draw.W(_badgeCenter);
            // The PNGs carry the tier scale and motion; the live text follows the table.
            _badge.sprite = sprite; _badge.enabled = sprite != null;
            if (sprite != null) _badge.transform.localScale = Vector3.one * (390 * _k / sprite.bounds.size.x);
            if (_language != Lang.Revision)
            {
                _language = Lang.Revision;
                _number.Text = "x" + Mathf.Min(99, _tier + 2) + (_tier == 3 ? "+" : "");
                _number.Outline(Draw.Hex(NumberRims[_tier]), .2f);
                _number.SizePt = NumberPt[_tier] * _k;
                _praise.Text = Praise(_tier);
                _praise.SizePt = PraisePt[_tier] * _k;
            }
            float s = TierScale[_tier] * scale;
            var group = Quaternion.Euler(0, 0, angle);
            _number.gameObject.SetActive(true); _praise.gameObject.SetActive(true);
            _number.transform.localPosition = group * (Draw.W(new Vector2(-104, -1.5f + y) * (_k * s)));
            _praise.transform.localPosition = group * (Draw.W(new Vector2(35, 1 + y) * (_k * s)));
            _number.transform.localRotation = _praise.transform.localRotation = group;
            _number.transform.localScale = _praise.transform.localScale = Vector3.one * s;
        }

        void Meter(float now)
        {
            float age = now - _meterAt, broken = now - _breakAt;
            var center = new Vector2(_board.Origin.x + _board.Size.x / 2, _board.Origin.y + _board.Size.y + 18 + 20 * _k);
            var size = new Vector2(136, 20) * _k;
            bool on = age < Window, fading = !on && age < Window + .125f;
            if (broken < .125f)
            {
                var sprite = _break[Mathf.Min(2, (int)(broken / .125f * 3))];
                _breakView.sprite = sprite; _breakView.enabled = sprite != null;
                Place(_breakView, center, size);
                _trackView.enabled = _fillView.enabled = _emberView.enabled = false;
                return;
            }
            _breakView.enabled = false;
            if (!on && !fading) { _trackView.enabled = _fillView.enabled = _emberView.enabled = false; return; }
            float alpha = on ? 1 : 1 - (age - Window) / .125f;
            _trackView.sprite = _track; _trackView.enabled = _track != null;
            Place(_trackView, center, size); _trackView.color = new Color(1, 1, 1, alpha);
            float left = on ? Mathf.Clamp01(1 - age / Window) : 0;
            var fill = Fill(_meterTier, left);
            _fillView.sprite = fill; _fillView.enabled = fill != null && left > 0;
            if (fill != null)
            {
                // The cropped sprite keeps the source's pixels per unit: scale by the full width, anchor the left edge.
                float full = size.x / (_track != null ? _track.bounds.size.x : fill.bounds.size.x);
                _fillView.transform.localScale = Vector3.one * full;
                float width = fill.bounds.size.x * full;
                _fillView.transform.localPosition = Draw.W(new Vector2(center.x - size.x / 2 + width / 2, center.y));
                _fillView.color = new Color(1, 1, 1, alpha);
            }
            _emberView.sprite = _ember; _emberView.enabled = _ember != null && on && left > 0;
            if (_emberView.enabled)
            {
                float x = center.x - size.x / 2 + (8 + left * 255) / 272f * size.x;
                Place(_emberView, new Vector2(x, center.y), Vector2.one * (24 * _k));
            }
        }

        static void Place(SpriteRenderer r, Vector2 center, Vector2 size)
        {
            if (r.sprite == null) return;
            r.transform.localPosition = Draw.W(center);
            r.transform.localScale = new Vector3(size.x / r.sprite.bounds.size.x, size.y / r.sprite.bounds.size.y, 1);
        }

        /* Fill cropped from the right, in 5 % steps prepared once per tier (x 8..263 of the 272 px source). */
        Sprite Fill(int tier, float left)
        {
            if (_fill[tier] == null)
            {
                _fill[tier] = new Sprite[21];
                var source = Get("Meter/fill-" + TierKeys[tier]);
                if (source != null)
                    for (int i = 1; i <= 20; i++)
                    {
                        var rect = source.rect;
                        float px = rect.width / 272f;
                        rect.width = Mathf.Round((8 + 255 * i / 20f) * px);
                        _fill[tier][i] = Sprite.Create(source.texture, rect, new Vector2(.5f, .5f), source.pixelsPerUnit);
                    }
            }
            return _fill[tier][Mathf.Clamp(Mathf.CeilToInt(left * 20), 0, 20)];
        }
    }
}
