using System.Collections.Generic;
using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_74: the booster moments redone (≤ 1.5 s each, one ease-out family, a soft stop):
         hammer   flies in an arc, winds up, strikes: the piece cracks, the screen shakes, sparks; the piece shatters
         scissors fly to the joint, open wide, snap shut ("şak"): the tab breaks off and its bits scatter; spin away
         wand     flies with a sparkle trail, circles the region drawing a swirl; the region lifts and resettles
         clock    the alarm clock flies to the Time box and rings; frost spreads over the box, "+10 sn" pops
       Sprites: Art/yeni/pm74/... (Codex, PM_74), with the PM_62 booster art as the fallback. GameRoot keeps the
       timing contract: the Core command at Hit seconds, the effect over at End seconds. */
    sealed class BoosterFx74
    {
        public const float HammerContact = .40f, HammerShatter = .50f, HammerEnd = 1.0f;
        public const float ScissorsArrive = .34f, ScissorsSnap = .50f, ScissorsEnd = 1.15f;
        public const float WandArrive = .40f, WandHit = .72f, WandEnd = 1.45f;
        public const float ClockArrive = .44f, ClockTouch = .62f, ClockEnd = 1.35f;

        readonly Transform _root;
        readonly Vector2 _screen;
        readonly float _safeTop, _s;
        readonly BoardView _board;
        readonly BoosterBar _bar;
        readonly Dictionary<string, Sprite> _sprites = new Dictionary<string, Sprite>();
        readonly SpriteRenderer _tool, _flash, _ring, _crack, _glaze, _shadow, _clock, _bells, _clockRing, _frost;
        readonly TextView _plus;
        readonly List<Particle> _particles = new List<Particle>();
        readonly Stack<SpriteRenderer> _pool = new Stack<SpriteRenderer>();
        const int Order = 3510;
        public System.Action<string, float> Sound;

        Booster? _running;
        float _lastAge = -1, _clockLastAge = -1;
        int _fired, _clockFired;
        Color _tone = Color.white;
        Vector2 _shakeBase; float _shakeUntil = -1, _shakeAmp, _shakeStart;
        Transform _shakeTarget;

        struct Particle
        {
            public SpriteRenderer R; public Vector2 P, V; public float Spin, Angle, Size, SizeEnd, Life, Age, Gravity, Drag; public Color C;
        }

        public BoosterFx74(Transform parent, Vector2 screen, float safeTop, BoardView board, BoosterBar bar)
        {
            _root = new GameObject("PM74 booster fx").transform; _root.SetParent(parent, false);
            _screen = screen; _safeTop = safeTop; _board = board; _bar = bar; _s = Hud54.Scale(screen.x);
            _shadow = Layer("fx shadow", Order); _crack = Layer("fx crack", Order + 1); _glaze = Layer("fx glaze", Order + 1);
            _ring = Layer("fx ring", Order + 2); _tool = Layer("fx tool", Order + 6); _flash = Layer("fx flash", Order + 8);
            _frost = Layer("fx clock frost", Order + 2); _clockRing = Layer("fx clock ring", Order + 3);
            _clock = Layer("fx clock", Order + 10); _bells = Layer("fx clock bells", Order + 11);
            _plus = TextView.Create(_root, "fx plus seconds", "", 30 * _s, Palette.Cream, order: Order + 9).Heading(TextView.RimTeal);
            _plus.gameObject.SetActive(false);
            _root.gameObject.AddComponent<FxGuard74>().Gone = RestoreCamera;
        }

        /* A level change mid-shake destroys this view: the camera goes back home. */
        sealed class FxGuard74 : MonoBehaviour { public System.Action Gone; void OnDestroy() => Gone?.Invoke(); }
        void RestoreCamera()
        {
            if (_shakeUntil >= 0 && _shakeTarget != null) _shakeTarget.localPosition = new Vector3(_shakeBase.x, _shakeBase.y, _shakeTarget.localPosition.z);
            _shakeUntil = -1;
        }

        SpriteRenderer Layer(string name, int order)
        {
            var r = Draw.Child<SpriteRenderer>(_root, name); r.sortingOrder = order; r.enabled = false; return r;
        }

        Sprite Get(string key, string fallback = null)
        {
            if (key == null) return null;
            if (!_sprites.TryGetValue(key, out var sprite))
            {
                sprite = Resources.Load<Sprite>("Art/yeni/pm74/" + key);
                if (sprite == null && fallback != null) sprite = fallback.StartsWith("hud/") || fallback.StartsWith("T46/") ? Art.Get(fallback) : Resources.Load<Sprite>("Art/yeni/boosters21/" + fallback);
                _sprites[key] = sprite;
            }
            return sprite;
        }

        void Paint(SpriteRenderer r, Sprite sprite, Vector2 point, Vector2 size, float alpha = 1, float angle = 0, Color? tint = null)
        {
            if (sprite == null || alpha <= 0) { r.enabled = false; return; }
            r.sprite = sprite; r.enabled = true;
            r.transform.localPosition = Draw.W(point);
            r.transform.localRotation = Quaternion.Euler(0, 0, angle);
            r.transform.localScale = new Vector3(size.x / sprite.bounds.size.x, size.y / sprite.bounds.size.y, 1);
            var c = tint ?? Color.white; c.a *= Mathf.Clamp01(alpha); r.color = c;
        }

        public static float OutCubic(float t) { t = Mathf.Clamp01(t); return 1 - (1 - t) * (1 - t) * (1 - t); }
        public static float InCubic(float t) { t = Mathf.Clamp01(t); return t * t * t; }
        public static float InOutCubic(float t) { t = Mathf.Clamp01(t); return t < .5f ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2; }
        public static float OutBack(float t, float k = 1.6f) { t = Mathf.Clamp01(t) - 1; return 1 + (k + 1) * t * t * t + k * t * t; }
        static float Span(float age, float from, float to) => Mathf.Clamp01((age - from) / (to - from));

        bool Once(int bit) { if ((_fired & (1 << bit)) != 0) return false; _fired |= 1 << bit; return true; }

        bool ClockOnce(int bit) { if ((_clockFired & (1 << bit)) != 0) return false; _clockFired |= 1 << bit; return true; }

        void Hide() { _tool.enabled = _flash.enabled = _ring.enabled = _crack.enabled = _shadow.enabled = false; }
        void HideClock() { _clock.enabled = _bells.enabled = _clockRing.enabled = _frost.enabled = false; }

        /* Once per frame, after Effect and Clock. */
        public void Tick() { TickParticles(); TickShake(); }

        /* Every frame while a targeted booster runs (null: none). */
        public void Effect(Booster? booster, float age, Vector2 target, bool vertical)
        {
            if (booster != _running || age < _lastAge)
            {
                _fired = 0; _running = booster;
                if (booster.HasValue) _tone = _board.ToneAt(target);
            }
            _lastAge = booster.HasValue ? age : -1;
            Hide();
            if (booster == Booster.Hammer) Hammer(age, target);
            else if (booster == Booster.Scissors) Scissors(age, target, vertical);
            else if (booster == Booster.Wand) Wand(age, target);
        }

        // ---- hammer ---------------------------------------------------------------------------------------------

        void Hammer(float age, Vector2 target)
        {
            var start = _bar.RectOf(Booster.Hammer).center;
            float cell = _board.Cell, size = Mathf.Clamp(cell * 1.9f, 84 * _s, 124 * _s);
            var above = target + new Vector2(cell * .42f, -cell * .78f);   // the head lands on the piece's centre
            Vector2 point; float angle, scale = 1, alpha = 1;
            if (age < HammerContact - .1f)
            {
                // the flight: an arc from the slot, growing, the head tipping back (wind-up)
                float t = OutCubic(Span(age, 0, HammerContact - .1f));
                point = Vector2.Lerp(start, above + new Vector2(cell * .2f, -cell * .35f), t) - Vector2.up * (Mathf.Sin(t * Mathf.PI) * 90 * _s);
                scale = Mathf.Lerp(.55f, 1, t); angle = Mathf.Lerp(0, 38, t);
            }
            else if (age < HammerContact)
            {
                // the swing: fast, accelerating into the piece
                float t = InCubic(Span(age, HammerContact - .1f, HammerContact));
                point = Vector2.Lerp(above + new Vector2(cell * .2f, -cell * .35f), above, t);
                angle = Mathf.Lerp(38, -28, t);
            }
            else if (age < HammerContact + .14f)
            {
                // the recoil: a little bounce up
                float t = Span(age, HammerContact, HammerContact + .14f);
                point = above - new Vector2(0, Mathf.Sin(t * Mathf.PI) * 16 * _s);
                angle = Mathf.Lerp(-28, -6, OutCubic(t));
            }
            else
            {
                // tumbling out of the screen, lower right
                float t = Span(age, HammerContact + .14f, HammerEnd);
                point = above + new Vector2(170 * _s * t, -70 * _s * Mathf.Sin(t * Mathf.PI) + 520 * _s * t * t);
                angle = -6 - 460 * InCubic(t) - 120 * t; alpha = 1 - InCubic(t);
            }
            Paint(_shadow, Get("cekic/shadow", "Boosters/Hammer/soft-shadow"), Vector2.Lerp(start, target, OutCubic(Span(age, 0, HammerContact))) + new Vector2(0, cell * .35f),
                new Vector2(size * .8f, size * .26f), .45f * (1 - Span(age, HammerContact, HammerContact + .2f)));
            Paint(_tool, Get("cekic/hammer", "Boosters/Hammer/flight/01"), point, Vector2.one * size * scale, alpha, angle);

            if (age >= HammerContact)
            {
                if (Once(0))
                {
                    Shake(9 * _s, .2f);
                    Haptics.Play(Haptics.Kind.Heavy);
                    Sound?.Invoke("piece", 1f);
                    for (int i = 0; i < 7; i++)
                    {
                        float a = (i / 7f) * Mathf.PI * 2 + Random.Range(-.3f, .3f), v = Random.Range(260, 420) * _s;
                        Spawn(Get("cekic/spark", "Boosters/Hammer/impact/03"), target, new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * v, Random.Range(-360, 360),
                            Random.Range(16, 26) * _s, 4 * _s, .38f, 0, 5f, new Color(1, .93f, .62f));
                    }
                }
                // the flash: a short white-gold star burst on the contact
                float f = Span(age, HammerContact, HammerContact + .2f);
                Paint(_flash, Get("cekic/flash", "Boosters/Hammer/impact/03"), target, Vector2.one * cell * Mathf.Lerp(1.4f, 2.3f, OutCubic(f)), 1 - f * f);
                // the crack grows on the piece until it shatters
                if (age < HammerShatter + .04f)
                {
                    float c = Span(age, HammerContact, HammerShatter);
                    var crack = Get(c < .34f ? "cekic/crack_01" : c < .7f ? "cekic/crack_02" : "cekic/crack_03");
                    Paint(_crack, crack, target, Vector2.one * cell * 1.02f, 1);
                    if (crack == null) _crack.enabled = false;
                }
            }
            if (age >= HammerShatter && Once(1))
            {
                Shake(5 * _s, .14f);
                var shards = new[] { Get("cekic/shard_a"), Get("cekic/shard_b"), Get("cekic/shard_c") };
                for (int i = 0; i < 10; i++)
                {
                    float a = Random.Range(-Mathf.PI, 0) * .9f - .15f, v = Random.Range(180, 360) * _s;
                    var sprite = shards[i % 3];
                    var at = target + new Vector2(Random.Range(-.3f, .3f), Random.Range(-.3f, .3f)) * cell;
                    if (sprite != null)
                        Spawn(sprite, at, new Vector2(Mathf.Cos(a) * v, Mathf.Sin(a) * v), Random.Range(-540, 540),
                            cell * Random.Range(.28f, .42f), cell * .2f, .62f, 1500 * _s, .6f, _tone);
                    else SpawnSquare(at, new Vector2(Mathf.Cos(a) * v, Mathf.Sin(a) * v), cell * Random.Range(.16f, .26f), .62f, _tone);
                }
                for (int i = 0; i < 4; i++)
                    Spawn(Get("cekic/dust", "Boosters/Hammer/impact/07"), target + Random.insideUnitCircle * cell * .25f, Random.insideUnitCircle * 60 * _s, 0,
                        cell * .7f, cell * 1.3f, .5f, -40 * _s, 2f, new Color(1, .96f, .88f, .8f));
            }
        }

        // ---- scissors -------------------------------------------------------------------------------------------

        void Scissors(float age, Vector2 target, bool vertical)
        {
            var slot = _bar.RectOf(Booster.Scissors);
            float cell = _board.Cell, size = Mathf.Clamp(cell * 1.5f, 84 * _s, 118 * _s);
            // the blades point along the seam they cut across: a vertical joint (left|right pieces) is cut top-down
            float seamAngle = vertical ? -90 : 0;
            var back = vertical ? new Vector2(0, -1) : new Vector2(-1, 0);
            var rest = target + back * size * .18f;
            Vector2 point; float angle, scale = 1, alpha = 1; string pose;
            if (age < ScissorsArrive)
            {
                float t = OutCubic(Span(age, 0, ScissorsArrive));
                point = Vector2.Lerp(slot.center, rest + back * size * .14f, t) - Vector2.up * (Mathf.Sin(t * Mathf.PI) * 60 * _s);
                scale = Mathf.Lerp(.55f, 1, t); angle = Mathf.Lerp(seamAngle + 40, seamAngle, t); pose = "closed";
            }
            else if (age < ScissorsSnap - .06f)
            {
                // the jaws open wide, a small pull back (anticipation)
                float t = OutCubic(Span(age, ScissorsArrive, ScissorsSnap - .06f));
                point = rest + back * size * (.14f + .05f * t); angle = seamAngle; pose = t < .45f ? "half" : "open";
            }
            else if (age < ScissorsSnap)
            {
                // snap: forward and shut
                float t = InCubic(Span(age, ScissorsSnap - .06f, ScissorsSnap));
                point = rest + back * size * Mathf.Lerp(.19f, 0, t); angle = seamAngle; pose = t < .5f ? "half" : "closed";
            }
            else if (age < ScissorsSnap + .16f)
            {
                float t = Span(age, ScissorsSnap, ScissorsSnap + .16f);
                point = rest + Random.insideUnitCircle * 2.5f * _s * (1 - t); angle = seamAngle; pose = "closed";
            }
            else
            {
                // spin away, up and right, fading
                float t = Span(age, ScissorsSnap + .16f, ScissorsEnd);
                float e = InCubic(t);
                point = rest + new Vector2(220 * _s * e, -60 * _s * Mathf.Sin(t * Mathf.PI) - 260 * _s * e);
                angle = seamAngle - 400 * OutCubic(t); scale = 1 - .25f * t; alpha = 1 - e; pose = "closed";
            }
            var sprite = Get("makas/" + pose, pose == "open" ? "Boosters/Scissors/walk/04" : pose == "half" ? "Boosters/Scissors/walk/03" : "Boosters/Scissors/walk/01");
            Paint(_tool, sprite, point, Vector2.one * size * scale, alpha, angle);
            if (age >= ScissorsSnap - .02f)
            {
                if (Once(0))
                {
                    Haptics.Play(Haptics.Kind.Medium);
                    Sound?.Invoke("pm72-unlock", 1f);
                    Shake(4 * _s, .12f);
                    // the cut tab flies off, its bits scatter
                    var tab = Get("makas/tab");
                    var dir = vertical ? new Vector2(1, -.6f) : new Vector2(.5f, -1);
                    if (tab != null) Spawn(tab, target, dir.normalized * 260 * _s, 520, cell * .34f, cell * .3f, .6f, 1300 * _s, .4f, _tone);
                    else SpawnDisc(target, dir.normalized * 260 * _s, cell * .26f, .6f, _tone);
                    // the scissors cut a tab between two pieces: puzzle crumbs in the picture's tone and gold sparks
                    var bits = new[] { Get("cekic/shard_a"), Get("cekic/shard_b"), Get("cekic/spark") };
                    for (int i = 0; i < 8; i++)
                    {
                        float a = Random.Range(0, Mathf.PI * 2), v = Random.Range(140, 300) * _s;
                        var bit = bits[i % 3];
                        var vel = new Vector2(Mathf.Cos(a), Mathf.Sin(a) - .6f) * v;
                        bool gold = i % 3 == 2;
                        if (bit != null) Spawn(bit, target, vel, Random.Range(-600, 600), cell * (gold ? .22f : Random.Range(.16f, .24f)), cell * .08f, .5f, gold ? 0 : 1200 * _s, gold ? 3f : .5f, gold ? Color.white : _tone);
                        else SpawnSquare(target, vel, cell * Random.Range(.07f, .12f), .5f, i % 2 == 0 ? _tone : new Color(1, .86f, .45f));
                    }
                }
                float f = Span(age, ScissorsSnap - .02f, ScissorsSnap + .2f);
                Paint(_flash, Get("makas/snip_flash", "Boosters/Hammer/impact/03"), target, Vector2.one * cell * Mathf.Lerp(.8f, 1.5f, OutCubic(f)), 1 - f, seamAngle + 45);
            }
        }

        // ---- wand -----------------------------------------------------------------------------------------------

        float _trailAt;
        void Wand(float age, Vector2 target)
        {
            var slot = _bar.RectOf(Booster.Wand);
            float cell = _board.Cell, size = Mathf.Clamp(cell * 1.6f, 80 * _s, 110 * _s);
            float radius = cell * .9f;
            Vector2 point; float angle, alpha = 1, scale = 1;
            if (age < WandArrive)
            {
                float t = OutCubic(Span(age, 0, WandArrive));
                point = Vector2.Lerp(slot.center, target + new Vector2(radius, 0), t) - Vector2.up * (Mathf.Sin(t * Mathf.PI) * 110 * _s);
                angle = Mathf.Lerp(-20, 20, t); scale = Mathf.Lerp(.6f, 1, t);
            }
            else if (age < WandHit + .18f)
            {
                // one loop around the region's centre, drawing the swirl
                float t = InOutCubic(Span(age, WandArrive, WandHit + .18f));
                float a = t * Mathf.PI * 2;
                point = target + new Vector2(Mathf.Cos(a), -Mathf.Sin(a)) * radius;
                angle = 20 + 30 * Mathf.Sin(a * 2);
            }
            else
            {
                float t = Span(age, WandHit + .18f, WandEnd);
                float e = InCubic(t);
                point = target + new Vector2(radius, 0) + new Vector2(_screen.x * .9f * e, -80 * _s * Mathf.Sin(t * Mathf.PI));
                angle = 20 - 160 * e; alpha = 1 - e;
            }
            Paint(_tool, Get("degnek/wand", "Boosters/Wand/tool/01"), point, Vector2.one * size * scale, alpha, angle);
            // sparkles fall off the star along the path
            var tip = point + new Vector2(-.18f, -.32f) * size;
            if (age < WandHit + .3f && Time.unscaledTime - _trailAt > .025f)
            {
                _trailAt = Time.unscaledTime;
                Spawn(Get("degnek/sparkle", "Boosters/Hammer/impact/03"), tip + Random.insideUnitCircle * 6 * _s, Random.insideUnitCircle * 40 * _s + new Vector2(0, 30 * _s),
                    Random.Range(-200, 200), Random.Range(10, 18) * _s, 2 * _s, .45f, 120 * _s, 1f, Random.value < .5f ? new Color(1, .9f, .5f) : new Color(.86f, .7f, 1f));
            }
            if (age >= WandArrive)
            {
                float f = Span(age, WandArrive, WandHit + .5f);
                var swirl = Get(f < .2f ? "degnek/swirl_01" : f < .4f ? "degnek/swirl_02" : f < .55f ? "degnek/swirl_03" : f < .7f ? "degnek/swirl_04" : f < .85f ? "degnek/swirl_05" : "degnek/swirl_06",
                    "Boosters/Wand/touch-burst/0" + (1 + Mathf.Min(8, (int)(f * 9))));
                Paint(_ring, swirl, target, Vector2.one * radius * 2.6f * Mathf.Lerp(.7f, 1.15f, OutCubic(f)), f < .8f ? 1 : (1 - f) * 5, -360 * f);
            }
            if (age >= WandHit && Once(0))
            {
                Haptics.Play(Haptics.Kind.Medium);
                Sound?.Invoke("pm72-unlock", 1f);
                for (int i = 0; i < 12; i++)
                {
                    float a = i / 12f * Mathf.PI * 2;
                    Spawn(Get("degnek/sparkle", "Boosters/Hammer/impact/03"), target + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius * .4f,
                        new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * 220 * _s, 300, 16 * _s, 4 * _s, .55f, 0, 3f, i % 2 == 0 ? new Color(1, .9f, .5f) : new Color(.86f, .7f, 1f));
                }
            }
        }

        // ---- clock ----------------------------------------------------------------------------------------------

        /* Three ideas (Codex PM_74, docs/art/pm74/booster/saat/fikir_*.png); &saatfx=b or c for the others.
             A  the alarm clock flies from its button to the Time box, lands on it and rings; frost; "+10 sn"
             B  a big clock appears under the HUD, its hands rewind, it dives into the Time box; "+10"
             C  the clock flies straight in; on contact an ice wave bursts out of the box
           In all three the box glazes and a "+10 sn" chip under it counts the frozen seconds down. */
        public enum ClockIdea { A, B, C }
        public static ClockIdea ClockStyle = ClockIdea.A;
        public static float ClockTouchFor(ClockIdea idea) => idea == ClockIdea.B ? .86f : idea == ClockIdea.C ? .42f : ClockTouch;
        static float ClockEndFor(ClockIdea idea) => idea == ClockIdea.B ? 1.4f : idea == ClockIdea.C ? 1.15f : ClockEnd;

        bool _clockOwnsGlaze;
        SpriteRenderer _handLong, _handShort;
        ShapeView _chip;
        TextView _chipText;
        int _chipShown = -1;

        void Hands(Vector2 center, float size, float angle, float alpha, float longTurn, float shortTurn)
        {
            if (_handLong == null) { _handLong = Layer("fx clock hand long", Order + 12); _handShort = Layer("fx clock hand short", Order + 12); }
            void Hand(SpriteRenderer r, Sprite sp, float length, float turn)
            {
                if (sp == null || alpha <= 0) { r.enabled = false; return; }
                float a = angle + turn;
                // the sprite's pivot is its centre; the hand turns about its bottom end (the clock's centre)
                var dir = new Vector2(Mathf.Sin(-a * Mathf.Deg2Rad), -Mathf.Cos(-a * Mathf.Deg2Rad));   // layout: y down
                Paint(r, sp, center + dir * length / 2, new Vector2(length * sp.bounds.size.x / sp.bounds.size.y, length), alpha, a);
            }
            Hand(_handLong, Get("saat/hand_long"), size * .2f, longTurn);
            Hand(_handShort, Get("saat/hand_short"), size * .145f, shortTurn);
        }

        /* The dial centre of clock_big (600 px: the cream dial is centred ~22 px under the middle). */
        static Vector2 Dial(Vector2 point, float size, float angle)
        {
            var off = new Vector2(0, size * .0625f);
            float r = -angle * Mathf.Deg2Rad;
            return point + new Vector2(off.x * Mathf.Cos(r) - off.y * Mathf.Sin(r), off.x * Mathf.Sin(r) + off.y * Mathf.Cos(r));
        }

        /* The clock: age < 0 none. Returns false when nothing is drawn. */
        public bool Clock(float age, double freezeLeft, bool stopped)
        {
            var panel = Hud54.TimePanelCenter(_screen.x, _safeTop);
            bool frozen = freezeLeft > 0 && !stopped;
            if (age < 0 || stopped)
            {
                HideClock(); _clockLastAge = -1;
                if (_handLong != null) _handLong.enabled = _handShort.enabled = false;
                _clockOwnsGlaze = false; _glaze.enabled = false; _plus.gameObject.SetActive(false); Chip(panel, 0, false);
                return false;
            }
            if (age < _clockLastAge || _clockLastAge < 0) _clockFired = 0;
            _clockLastAge = age;
            var idea = ClockStyle;
            float touch = ClockTouchFor(idea), end = ClockEndFor(idea);
            var slot = _bar.RectOf(Booster.Clock);
            float big = 92 * _s;
            var over = panel + new Vector2(0, -6 * _s);
            var clockSprite = Get("saat/clock_big", "Clock/Layers/flying-clock");
            bool handsOn = Get("saat/hand_long") != null && clockSprite != null && clockSprite.name.Contains("clock_big");
            if (age < end)
            {
                Vector2 point; float angle = 0, scale, alpha = 1, size = big, rewind = 0;
                if (idea == ClockIdea.B)
                {
                    var stage = new Vector2(_screen.x / 2, Hud54.TopBottom(_screen, _safeTop) + 95 * _s);
                    size = 150 * _s;
                    if (age < .26f) { float t = Span(age, 0, .26f); point = stage; scale = Mathf.Max(.01f, OutBack(t, 1.4f)); angle = -12 * (1 - t); }
                    else if (age < .62f)
                    {
                        float t = Span(age, .26f, .62f);
                        point = stage; scale = 1 + .04f * Mathf.Sin(t * Mathf.PI * 4); rewind = -720 * InOutCubic(t);
                    }
                    else if (age < touch)
                    {
                        float t = InCubic(Span(age, .62f, touch));
                        point = Vector2.Lerp(stage, panel, t) - Vector2.up * (Mathf.Sin(t * Mathf.PI) * 40 * _s);
                        scale = Mathf.Lerp(1, .3f, t); angle = Mathf.Lerp(0, 18, t); rewind = -720;
                    }
                    else { point = panel; scale = .3f; alpha = 0; rewind = -720; }
                }
                else if (idea == ClockIdea.C)
                {
                    if (age < touch)
                    {
                        float t = InCubic(Span(age, 0, touch));
                        point = Vector2.Lerp(slot.center, panel, t) - Vector2.up * (Mathf.Sin(t * Mathf.PI) * 70 * _s);
                        scale = Mathf.Lerp(.6f, .9f, t); angle = Mathf.Lerp(-30, 10, t);
                    }
                    else
                    {
                        float t = Span(age, touch, touch + .16f);
                        point = panel; scale = Mathf.Lerp(.9f, .2f, OutCubic(t)); alpha = 1 - t;
                    }
                }
                else if (age < ClockArrive)
                {
                    float t = OutCubic(Span(age, 0, ClockArrive));
                    point = Vector2.Lerp(slot.center, over, t) - Vector2.up * (Mathf.Sin(t * Mathf.PI) * 120 * _s) + Vector2.right * (Mathf.Sin(t * Mathf.PI) * -40 * _s);
                    scale = Mathf.Lerp(.6f, 1.12f, t); angle = Mathf.Lerp(-25, 8, t);
                }
                else if (age < touch + .22f)
                {
                    // rings: a fast wobble that settles
                    float t = Span(age, ClockArrive, touch + .22f);
                    point = over; scale = Mathf.Lerp(1.12f, 1, OutCubic(t));
                    angle = 12 * Mathf.Sin(t * Mathf.PI * 9) * (1 - t);
                }
                else
                {
                    // shrinks into the chip under the box
                    float t = InOutCubic(Span(age, touch + .22f, end));
                    var chip = ChipCenter(panel);
                    point = Vector2.Lerp(over, chip + new Vector2(-34 * _s, 0), t);
                    scale = Mathf.Lerp(1, .22f, t); alpha = 1 - Span(age, end - .1f, end);
                }
                Paint(_clock, clockSprite, point, Vector2.one * size * scale, alpha, angle);
                if (handsOn)
                {
                    float wiggle = idea == ClockIdea.A && age >= ClockArrive && age < touch + .22f ? 10 * Mathf.Sin(age * 60) : 0;
                    Hands(Dial(point, size * scale, angle), size * scale, angle, alpha, 60 + rewind + wiggle, -60 + rewind / 12);
                }
                else if (_handLong != null) _handLong.enabled = _handShort.enabled = false;
                // ring waves while it rings (A) or rewinds (B)
                bool ringing = idea == ClockIdea.A ? age >= ClockArrive && age < touch + .22f : idea == ClockIdea.B && age >= .26f && age < .62f;
                if (ringing)
                {
                    float t = idea == ClockIdea.A ? Span(age, ClockArrive, touch + .22f) : Span(age, .26f, .62f);
                    if (ClockOnce(0)) { Haptics.Play(Haptics.Kind.Light); Sound?.Invoke("pm72-tick", .8f); }
                    if (t > .5f && ClockOnce(1)) { Haptics.Play(Haptics.Kind.Light); Sound?.Invoke("pm72-tick", .8f); }
                    float w = (t * 2.2f) % 1;
                    Paint(_clockRing, Get("saat/glow_ring", "Boosters/Wand/touch-burst/02"), point, Vector2.one * size * scale * Mathf.Lerp(.9f, 1.6f, w), .6f * (1 - w));
                }
                else _clockRing.enabled = false;
                _bells.enabled = false;
            }
            else
            {
                _clock.enabled = _bells.enabled = _clockRing.enabled = false;
                if (_handLong != null) _handLong.enabled = _handShort.enabled = false;
            }

            // contact: frost over the box (C: a big ice wave), "+10 sn" pops above it
            if (age >= touch)
            {
                bool wave = idea == ClockIdea.C;
                if (ClockOnce(2))
                {
                    Sound?.Invoke("pm72-unlock", .9f);
                    Haptics.Play(wave ? Haptics.Kind.Medium : Haptics.Kind.Light);
                    if (wave) Shake(3 * _s, .12f);
                    int n = wave ? 14 : 9;
                    for (int i = 0; i < n; i++)
                    {
                        float a = i / (float)n * Mathf.PI * 2 + .2f;
                        Spawn(Get("saat/snowflake", "Boosters/Hammer/impact/03"), panel + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * 20 * _s,
                            new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * (wave ? 230 : 150) * _s, Random.Range(-180, 180), Random.Range(10, 16) * _s, 6 * _s,
                            wave ? .9f : .7f, 160 * _s, 1.5f, new Color(.86f, .96f, 1f));
                    }
                }
                float f = Span(age, touch, touch + (wave ? .5f : .35f));
                if (f < 1) Paint(_frost, Get("saat/frost_burst_0" + (1 + Mathf.Min(5, (int)(f * 6))), "Boosters/Wand/touch-burst/0" + (1 + Mathf.Min(8, (int)(f * 9)))),
                    panel, Vector2.one * (wave ? 230 : 150) * _s * Mathf.Lerp(.7f, 1.1f, OutCubic(f)), f < .7f ? 1 : (1 - f) / .3f, 0, Color.white);
                else _frost.enabled = false;
                float p = Span(age, touch + .05f, touch + 1.0f);
                if (p < 1)
                {
                    _plus.gameObject.SetActive(true);
                    _plus.Text = idea == ClockIdea.B ? "+10" : Lang.T("+10 s", "+10 sn");
                    float pop = p < .25f ? OutBack(p / .25f) : 1;
                    _plus.transform.localPosition = Draw.W(panel + new Vector2(idea == ClockIdea.B ? -70 * _s : 0, (idea == ClockIdea.B ? 0 : -58 * _s) - 22 * _s * OutCubic(p)));
                    _plus.transform.localScale = Vector3.one * Mathf.Max(.01f, pop);
                    _plus.Alpha = p < .75f ? 1 : 1 - (p - .75f) / .25f;
                }
                else _plus.gameObject.SetActive(false);
            }
            else { _frost.enabled = false; _plus.gameObject.SetActive(false); }

            // while the time is frozen: the box glazed, the chip under it counting the bonus seconds down
            _clockOwnsGlaze = frozen && age >= touch;
            if (_clockOwnsGlaze)
            {
                float g = Span(age, touch, touch + .25f);
                Paint(_glaze, Get("saat/ice_glaze", "T46/clock/ice_overlay"), panel, new Vector2(118, 102) * _s, g * .95f);
            }
            else _glaze.enabled = false;
            Chip(panel, freezeLeft, _clockOwnsGlaze);
            return true;
        }

        /* The "+10 sn" chip: a navy pill with a gold rim under the Time box, the frozen seconds left. */
        void Chip(Vector2 panel, double freezeLeft, bool on)
        {
            if (_chip == null)
            {
                _chip = ShapeView.Create(_root, "fx freeze chip", new Vector2(92, 28) * _s, 2, Order + 3).Radius(14 * _s)
                    .Fill(Draw.Hex("#0E2B5C")).Stroke(Draw.Hex("#E3B54E"), 2 * _s);
                _chipText = TextView.Create(_root, "fx freeze chip text", "", 17 * _s, Palette.Cream, order: Order + 4).Heading(Draw.Hex("#0B1E40"));
            }
            _chip.gameObject.SetActive(on); _chipText.gameObject.SetActive(on);
            if (!on) { _chipShown = -1; return; }
            var c = ChipCenter(panel);
            _chip.transform.localPosition = Draw.W(c);
            _chipText.transform.localPosition = Draw.W(c);
            int left = Mathf.CeilToInt((float)freezeLeft);
            if (left != _chipShown) { _chipShown = left; _chipText.Text = string.Format(Lang.T("+{0} s", "+{0} sn"), left); }
        }

        Vector2 ChipCenter(Vector2 panel) => panel + new Vector2(0, 57.33f) * _s;

        // ---- particles and shake --------------------------------------------------------------------------------

        void Spawn(Sprite sprite, Vector2 p, Vector2 v, float spin, float size, float sizeEnd, float life, float gravity, float drag, Color c)
        {
            if (sprite == null) return;
            var r = _pool.Count > 0 ? _pool.Pop() : Layer("fx particle", Order + 4);
            r.sprite = sprite; r.enabled = true;
            _particles.Add(new Particle { R = r, P = p, V = v, Spin = spin, Size = size, SizeEnd = sizeEnd, Life = life, Gravity = gravity, Drag = drag, C = c, Angle = Random.Range(0, 360) });
        }

        static Sprite _square, _disc;
        void SpawnSquare(Vector2 p, Vector2 v, float size, float life, Color c)
        {
            if (_square == null)
            {
                var tex = new Texture2D(8, 8) { filterMode = FilterMode.Bilinear, hideFlags = HideFlags.HideAndDontSave };
                var px = new Color[64]; for (int i = 0; i < 64; i++) px[i] = Color.white; tex.SetPixels(px); tex.Apply(false, true);
                _square = Sprite.Create(tex, new Rect(0, 0, 8, 8), new Vector2(.5f, .5f), 8);
            }
            Spawn(_square, p, v, Random.Range(-500, 500), size, size * .5f, life, 1300 * _s, .5f, c);
        }

        void SpawnDisc(Vector2 p, Vector2 v, float size, float life, Color c)
        {
            if (_disc == null)
            {
                const int n = 32;
                var tex = new Texture2D(n, n) { filterMode = FilterMode.Bilinear, hideFlags = HideFlags.HideAndDontSave };
                var px = new Color[n * n];
                for (int y = 0; y < n; y++) for (int x = 0; x < n; x++)
                    {
                        float d = new Vector2(x + .5f - n / 2f, y + .5f - n / 2f).magnitude / (n / 2f);
                        px[y * n + x] = new Color(1, 1, 1, Mathf.Clamp01((1 - d) * 8));
                    }
                tex.SetPixels(px); tex.Apply(false, true);
                _disc = Sprite.Create(tex, new Rect(0, 0, n, n), new Vector2(.5f, .5f), n);
            }
            Spawn(_disc, p, v, 0, size, size * .6f, life, 1300 * _s, .4f, c);
        }

        void TickParticles()
        {
            float dt = Mathf.Min(Time.unscaledDeltaTime, 1 / 20f);
            for (int i = _particles.Count - 1; i >= 0; i--)
            {
                var q = _particles[i];
                q.Age += dt;
                if (q.Age >= q.Life || q.R == null)
                {
                    if (q.R != null) { q.R.enabled = false; _pool.Push(q.R); }
                    _particles.RemoveAt(i); continue;
                }
                q.V *= Mathf.Max(0, 1 - q.Drag * dt);
                q.V.y += q.Gravity * dt;
                q.P += q.V * dt; q.Angle += q.Spin * dt;
                float k = q.Age / q.Life;
                float size = Mathf.Lerp(q.Size, q.SizeEnd, OutCubic(k));
                var b = q.R.sprite.bounds.size;
                q.R.transform.localPosition = Draw.W(q.P);
                q.R.transform.localRotation = Quaternion.Euler(0, 0, q.Angle);
                q.R.transform.localScale = new Vector3(size / b.x, size / b.y, 1);
                var c = q.C; c.a *= k < .6f ? 1 : 1 - (k - .6f) / .4f; q.R.color = c;
                _particles[i] = q;
            }
        }

        void Shake(float amp, float seconds)
        {
            var cam = Camera.main;
            if (cam == null) return;
            if (_shakeUntil < 0) { _shakeTarget = cam.transform; _shakeBase = cam.transform.localPosition; }
            _shakeAmp = amp; _shakeStart = Time.unscaledTime; _shakeUntil = Time.unscaledTime + seconds;
        }

        void TickShake()
        {
            if (_shakeUntil < 0 || _shakeTarget == null) return;
            float now = Time.unscaledTime;
            if (now >= _shakeUntil) { _shakeTarget.localPosition = new Vector3(_shakeBase.x, _shakeBase.y, _shakeTarget.localPosition.z); _shakeUntil = -1; return; }
            float k = 1 - (now - _shakeStart) / (_shakeUntil - _shakeStart);
            float amp = _shakeAmp * k * k;
            var off = new Vector2(Mathf.Sin(now * 97) * amp, Mathf.Sin(now * 71 + 1.3f) * amp * .8f);
            _shakeTarget.localPosition = new Vector3(_shakeBase.x + off.x, _shakeBase.y + off.y, _shakeTarget.localPosition.z);
        }

        /* Level change or teardown: the camera back home, every layer off. */
        public void Clear()
        {
            if (_shakeUntil >= 0 && _shakeTarget != null) _shakeTarget.localPosition = new Vector3(_shakeBase.x, _shakeBase.y, _shakeTarget.localPosition.z);
            _shakeUntil = -1;
            foreach (var q in _particles) if (q.R != null) { q.R.enabled = false; _pool.Push(q.R); }
            _particles.Clear();
            Hide(); HideClock(); _glaze.enabled = false; _plus.gameObject.SetActive(false);
        }
    }
}
