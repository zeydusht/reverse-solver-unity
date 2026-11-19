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
        public const float HammerContact = .40f, HammerShatter = .56f, HammerEnd = 1.05f;
        public const float ScissorsArrive = .34f, ScissorsSnap = .50f, ScissorsEnd = 1.15f;
        public const float WandArrive = .40f, WandHit = .72f, WandEnd = 1.45f;
        public const float ClockArrive = .44f, ClockTouch = .62f, ClockEnd = 1.35f;

        readonly Transform _root;
        readonly Vector2 _screen;
        readonly float _safeTop, _s;
        readonly BoardView _board;
        readonly BoosterBar _bar;
        readonly Dictionary<string, Sprite> _sprites = new Dictionary<string, Sprite>();
        readonly SpriteRenderer _tool, _flash, _ring, _crack, _glaze, _shadow, _clock, _bells, _clockRing, _frost, _blade2, _pivot, _impact;
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
            _shadow = Layer("fx shadow", Order); _crack = Layer("fx crack", Order + 9); _glaze = Layer("fx glaze", Order + 1);
            _ring = Layer("fx ring", Order + 2); _tool = Layer("fx tool", Order + 6); _flash = Layer("fx flash", Order + 8);
            _blade2 = Layer("fx tool lower blade", Order + 5); _pivot = Layer("fx tool pivot", Order + 7); _impact = Layer("fx impact ring", Order + 4);
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

        /* PM_75 art (Codex): Art/yeni/pm75/... */
        Sprite Get75(string key)
        {
            if (!_sprites.TryGetValue("75/" + key, out var sprite)) _sprites["75/" + key] = sprite = Resources.Load<Sprite>("Art/yeni/pm75/" + key);
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
        /* A layout vector (y down) turned like a sprite with Euler z = deg (counter-clockwise on screen). */
        static Vector2 RotL(Vector2 v, float deg)
        {
            float r = deg * Mathf.Deg2Rad, c = Mathf.Cos(r), s = Mathf.Sin(r);
            return new Vector2(v.x * c + v.y * s, -v.x * s + v.y * c);
        }

        bool Once(int bit) { if ((_fired & (1 << bit)) != 0) return false; _fired |= 1 << bit; return true; }

        bool ClockOnce(int bit) { if ((_clockFired & (1 << bit)) != 0) return false; _clockFired |= 1 << bit; return true; }

        void Hide() { _tool.enabled = _flash.enabled = _ring.enabled = _crack.enabled = _shadow.enabled = _blade2.enabled = _pivot.enabled = _impact.enabled = false; }
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

        // ---- ideas -----------------------------------------------------------------------------------------------

        /* Three ideas per booster (PM_74 quality loop); A is the default, &cekicfx= / &makasfx= / &degnekfx= b or c.
             hammer   A arc + wind-up + strike (Royal Match)   B drops from above and slams   C two knocks
             scissors A fly, open wide, snap                     B two snips ("çıt çıt")         C dashes across the seam
             wand     A one loop drawing a swirl                 B three taps                    C a diagonal sweep */
        /* PM_75 (Zeyd: "no need to be fast, slowly looks better"): A is now the slow, readable one for the hammer and
           the scissors; D (&cekicfx=d, &makasfx=d) is PM_74's A. */
        public enum Idea { A, B, C, D }
        public static Idea HammerIdea = Idea.A, ScissorsIdea = Idea.A, WandIdea = Idea.A;
        public static Idea ParseIdea(string v) => v == "b" ? Idea.B : v == "c" ? Idea.C : v == "d" ? Idea.D : Idea.A;
        static float[] HammerContacts(Idea i) => i == Idea.B ? new[] { .30f } : i == Idea.C ? new[] { .40f, .66f } : new[] { HammerContact };
        public static float HammerHit(Idea i) => i == Idea.A ? Slam.Shatter : i == Idea.B ? .46f : i == Idea.C ? .80f : HammerShatter;
        public static float HammerDone(Idea i) => i == Idea.A ? Slam.End : i == Idea.B ? .85f : i == Idea.C ? 1.2f : HammerEnd;
        public static float ScissorsHit(Idea i) => i == Idea.A ? Snip.Break : i == Idea.B ? .58f : i == Idea.C ? .30f : ScissorsSnap;
        public static float ScissorsDone(Idea i) => i == Idea.A ? Snip.End : i == Idea.B ? 1.2f : i == Idea.C ? .78f : ScissorsEnd;

        /* The slow hammer: over the piece head down, up (wind-up), down like a sledgehammer, cracks from the struck
           point, shatters, back up and home to its slot. */
        static class Slam { public const float Arrive = .45f, Raise = .78f, Strike = .9f, Shatter = 1.08f, Lift = 1.38f, End = 1.9f; }
        /* The slow scissors: over the tab, the jaws open slowly, close in on it, cut through it (.5 s), the tab breaks
           off, home to the slot. */
        static class Snip { public const float Arrive = .5f, Open = .9f, Close = 1.08f, Break = 1.58f, Leave = 1.66f, End = 2.15f; public const float Wide = 32; }
        public static float WandHitAt(Idea i) => i == Idea.A ? Swirl.Hit : i == Idea.B ? .76f : i == Idea.C ? .66f : WandHit;
        public static float WandDone(Idea i) => i == Idea.A ? Swirl.End : i == Idea.B ? 1.3f : i == Idea.C ? 1.2f : WandEnd;
        /* The slow wand (PM_75): flies over, one slow loop drawing the swirl, the change, home to its slot. D: PM_74's A. */
        static class Swirl { public const float Arrive = .6f, Hit = 1.15f, End = 2.0f; }

        // ---- hammer ---------------------------------------------------------------------------------------------

        void Hammer(float age, Vector2 target)
        {
            var idea = HammerIdea;
            if (idea == Idea.A) { HammerSlow(age, target); return; }
            var start = _bar.RectOf(Booster.Hammer).center;
            float cell = _board.Cell, size = Mathf.Clamp(cell * 1.9f, 84 * _s, 124 * _s);
            // the path is the head's: it lands on the piece's centre (the sprite is placed from it below)
            var above = target;
            var raised = target + new Vector2(cell * .25f, -cell * .6f);
            var contacts = HammerContacts(idea);
            float first = contacts[0], last = contacts[contacts.Length - 1], shatter = HammerHit(idea), end = HammerDone(idea);
            Vector2 point; float angle, scale = 1, alpha = 1;
            if (idea == Idea.B)
            {
                var top = above - new Vector2(0, 260 * _s);
                if (age < .08f) { point = top; angle = 6; scale = .8f; alpha = age / .08f; }
                else if (age < first)
                {
                    float t = InCubic(Span(age, .08f, first));
                    point = Vector2.Lerp(top, above, t); angle = Mathf.Lerp(6, -22, t); scale = Mathf.Lerp(.8f, 1.08f, t);
                }
                else if (age < first + .14f)
                {
                    float t = Span(age, first, first + .14f);
                    point = above - new Vector2(0, Mathf.Sin(t * Mathf.PI) * 24 * _s); angle = Mathf.Lerp(-22, -8, OutCubic(t));
                    scale = 1.08f - .08f * t;
                }
                else
                {
                    float t = Span(age, first + .14f, end);
                    point = above - new Vector2(-40 * _s * t, 140 * _s * OutCubic(t)); angle = -8 + 30 * t; alpha = 1 - InCubic(t);
                }
            }
            else
            {
                if (age < first - .1f)
                {
                    // the flight: an arc from the slot, growing, the head tipping back (wind-up)
                    float t = OutCubic(Span(age, 0, first - .1f));
                    point = Vector2.Lerp(start, raised, t) - Vector2.up * (Mathf.Sin(t * Mathf.PI) * 90 * _s);
                    scale = Mathf.Lerp(.55f, 1, t); angle = Mathf.Lerp(0, 38, t);
                }
                else if (age < first)
                {
                    // the swing: fast, accelerating into the piece
                    float t = InCubic(Span(age, first - .1f, first));
                    point = Vector2.Lerp(raised, above, t); angle = Mathf.Lerp(38, -28, t);
                }
                else if (idea == Idea.C && age < last)
                {
                    // C: lifted again (.16 s), then the second, harder swing
                    float lift = last - .1f;
                    if (age < lift) { float t = OutCubic(Span(age, first, lift)); point = Vector2.Lerp(above, raised, t); angle = Mathf.Lerp(-28, 44, t); }
                    else { float t = InCubic(Span(age, lift, last)); point = Vector2.Lerp(raised, above, t); angle = Mathf.Lerp(44, -30, t); }
                }
                else if (age < last + .14f)
                {
                    // the recoil: a little bounce up
                    float t = Span(age, last, last + .14f);
                    point = above - new Vector2(0, Mathf.Sin(t * Mathf.PI) * 16 * _s);
                    angle = Mathf.Lerp(-28, -6, OutCubic(t));
                }
                else
                {
                    // tumbling out of the screen, lower right
                    float t = Span(age, last + .14f, end);
                    point = above + new Vector2(170 * _s * t, -70 * _s * Mathf.Sin(t * Mathf.PI) + 520 * _s * t * t);
                    angle = -6 - 460 * InCubic(t) - 120 * t; alpha = 1 - InCubic(t);
                }
            }
            var shadowAt = idea == Idea.B ? target + new Vector2(0, cell * .35f) : Vector2.Lerp(start, target, OutCubic(Span(age, 0, first))) + new Vector2(0, cell * .35f);
            float shadowGrow = idea == Idea.B ? Mathf.Lerp(.4f, 1, Span(age, 0, first)) : 1;
            Paint(_shadow, Get("cekic/shadow", "Boosters/Hammer/soft-shadow"), shadowAt, new Vector2(size * .8f, size * .26f) * shadowGrow, .45f * (1 - Span(age, last, last + .2f)));
            // the hammer sprite's striking face sits at (+.077, -.15) of its size from the centre (Hammer/flight/01)
            var centre = point - RotL(new Vector2(.077f, -.15f) * size * scale, angle);
            Paint(_tool, Get("cekic/hammer", "Boosters/Hammer/flight/01"), centre, Vector2.one * size * scale, alpha, angle);

            for (int k = 0; k < contacts.Length; k++)
            {
                if (age < contacts[k] || !Once(k)) continue;
                bool final = k == contacts.Length - 1;
                Shake((final ? 9 : 5) * _s * (idea == Idea.B ? 1.25f : 1), final ? .2f : .12f);
                Haptics.Play(Haptics.Kind.Medium);   // PM_86 §4: a booster's blow is medium
                Sound?.Invoke("piece", final ? 1f : .7f);
                int n = final ? 8 : 4;
                for (int i = 0; i < n; i++)
                {
                    float a = (i / (float)n) * Mathf.PI * 2 + Random.Range(-.3f, .3f), v = Random.Range(260, 420) * _s;
                    Spawn(Get("cekic/spark", "Boosters/Hammer/impact/03"), target, new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * v, Random.Range(-360, 360),
                        Random.Range(16, 26) * _s, 4 * _s, .38f, 0, 5f, new Color(1, .93f, .62f));
                }
            }
            if (age >= first)
            {
                // the flash after each knock: a short white-gold star burst
                float lastHit = first; foreach (var c in contacts) if (age >= c) lastHit = c;
                float f = Span(age, lastHit, lastHit + .15f);
                Paint(_flash, Get("cekic/spark", "Boosters/Hammer/impact/03"), target + new Vector2(cell * .3f, -cell * .3f), Vector2.one * cell * Mathf.Lerp(.8f, 1.3f, OutCubic(f)), 1 - f * f);
                // the crack grows on the piece until it shatters
                if (age < shatter + .04f)
                {
                    float c = Span(age, first, shatter);
                    var crack = Get(c < .34f ? "cekic/crack_01" : c < .7f ? "cekic/crack_02" : "cekic/crack_03");
                    Paint(_crack, crack, target, Vector2.one * cell * 1.2f, 1);
                    if (crack == null) _crack.enabled = false;
                }
            }
            if (age >= shatter && Once(4))
            {
                Shake(5 * _s, .14f);
                Sound?.Invoke("pm72-defuse", .7f);   // the crumble under the knock
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

        void HammerSlow(float age, Vector2 target)
        {
            var slot = _bar.RectOf(Booster.Hammer).center;
            float cell = _board.Cell, size = Mathf.Clamp(cell * 1.9f, 92 * _s, 128 * _s);
            var down = Get75("cekic/hammer_down");
            var squash = Get75("cekic/hammer_down_squash");
            // the striking face's centre in the sprite: (230, 470) of 512 (Codex), else the old hammer turned head down
            var face = down != null ? new Vector2(230 - 256, 470 - 256) / 512f : new Vector2(0, -.28f);
            var sprite = down ?? Get("cekic/hammer", "Boosters/Hammer/flight/01");
            float baseAngle = down != null ? 0 : 180;
            var over = target - new Vector2(0, cell * .55f);           // where the face hovers before the wind-up
            var high = target - new Vector2(0, cell * 1.45f);          // the top of the wind-up
            Vector2 point; float angle = baseAngle, scale = 1, alpha = 1, squashed = 0;
            if (age < Slam.Arrive)
            {
                float t = InOutCubic(Span(age, 0, Slam.Arrive));
                point = Vector2.Lerp(slot, over, t) - Vector2.up * (Mathf.Sin(t * Mathf.PI) * 70 * _s);
                scale = Mathf.Lerp(.5f, 1, t); angle = baseAngle + 8 * (1 - t);
            }
            else if (age < Slam.Raise)
            {
                // the wind-up: up, a touch bigger (nearer), a few degrees back
                float t = OutCubic(Span(age, Slam.Arrive, Slam.Raise));
                point = Vector2.Lerp(over, high, t); scale = 1 + .06f * t; angle = baseAngle + 6 * t;
            }
            else if (age < Slam.Strike)
            {
                // the slam: accelerating straight down onto the point
                float t = InCubic(Span(age, Slam.Raise, Slam.Strike));
                point = Vector2.Lerp(high, target, t); scale = 1.06f - .06f * t; angle = baseAngle + 6 * (1 - t);
            }
            else if (age < Slam.Shatter)
            {
                point = target; squashed = 1 - Span(age, Slam.Strike, Slam.Strike + .12f);
            }
            else if (age < Slam.Lift)
            {
                float t = OutCubic(Span(age, Slam.Shatter, Slam.Lift));
                point = Vector2.Lerp(target, high, t); angle = baseAngle - 4 * t;
            }
            else
            {
                // home: back to its slot, shrinking into the icon
                float t = InOutCubic(Span(age, Slam.Lift, Slam.End));
                point = Vector2.Lerp(high, slot, t) - Vector2.up * (Mathf.Sin(t * Mathf.PI) * 50 * _s);
                scale = Mathf.Lerp(1, .5f, t); angle = baseAngle - 4 * (1 - t);
                alpha = 1 - Span(age, Slam.End - .06f, Slam.End);
            }
            // the shadow under the face: small and pale while it is high, dark on contact
            float height = Mathf.Clamp01((target.y - point.y) / (cell * 1.45f));
            if (age < Slam.Lift + .1f)
                Paint(_shadow, Get("cekic/shadow", "Boosters/Hammer/soft-shadow"), target + new Vector2(0, cell * .12f),
                    new Vector2(size * .55f, size * .18f) * Mathf.Lerp(1, .6f, height), .55f * (1 - .6f * height) * Span(age, .1f, Slam.Arrive));
            var useSprite = squashed > .5f && squash != null ? squash : sprite;
            var sc = new Vector2(1 + .05f * squashed, 1 - .08f * squashed) * size * scale;
            var centre = point - RotL(Vector2.Scale(face, sc), angle);
            Paint(_tool, useSprite, centre, sc, alpha, angle);
            if (age >= Slam.Strike && Once(0))
            {
                Shake(10 * _s, .22f);
                Haptics.Play(Haptics.Kind.Medium);   // PM_86 §4
                Sound?.Invoke("piece", 1f);
                for (int i = 0; i < 10; i++)
                {
                    float a = Random.Range(Mathf.PI * 1.05f, Mathf.PI * 1.95f), v = Random.Range(240, 420) * _s;
                    Spawn(Get("cekic/spark", "Boosters/Hammer/impact/03"), target, new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * v, Random.Range(-360, 360),
                        Random.Range(16, 26) * _s, 4 * _s, .42f, 900 * _s, 3f, new Color(1, .93f, .62f));
                }
            }
            if (age >= Slam.Strike && age < Slam.Strike + .32f)
            {
                // the ring on the struck point
                float f = Span(age, Slam.Strike, Slam.Strike + .32f);
                var ring = Get75("cekic/impact_ring") ?? Get("saat/glow_ring");
                Paint(_impact, ring, target, new Vector2(cell * Mathf.Lerp(.4f, 1.6f, OutCubic(f)), cell * Mathf.Lerp(.25f, .9f, OutCubic(f))), 1 - f);
                Paint(_flash, Get("cekic/spark", "Boosters/Hammer/impact/03"), target, Vector2.one * cell * Mathf.Lerp(.6f, 1.1f, OutCubic(f)), (1 - f) * (1 - f));
            }
            if (age >= Slam.Strike && age < Slam.Shatter + .04f)
            {
                // the crack spreads from the struck point
                float c = Span(age, Slam.Strike, Slam.Shatter);
                var crack = Get(c < .3f ? "cekic/crack_01" : c < .65f ? "cekic/crack_02" : "cekic/crack_03");
                Paint(_crack, crack, target, Vector2.one * cell * Mathf.Lerp(.85f, 1.2f, c), 1);
            }
            if (age >= Slam.Shatter && Once(1))
            {
                Shake(6 * _s, .16f);
                Haptics.Play(Haptics.Kind.Medium);
                Sound?.Invoke("pm72-defuse", .75f);
                var shards = new[] { Get("cekic/shard_a"), Get("cekic/shard_b"), Get("cekic/shard_c") };
                for (int i = 0; i < 12; i++)
                {
                    float a = (i / 12f) * Mathf.PI * 2 + Random.Range(-.25f, .25f), v = Random.Range(160, 340) * _s;
                    var at = target + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * cell * Random.Range(.08f, .3f);
                    var vel = new Vector2(Mathf.Cos(a), Mathf.Sin(a) - .5f) * v;
                    if (shards[i % 3] != null) Spawn(shards[i % 3], at, vel, Random.Range(-540, 540), cell * Random.Range(.24f, .38f), cell * .18f, .66f, 1400 * _s, .6f, _tone);
                    else SpawnSquare(at, vel, cell * Random.Range(.14f, .24f), .62f, _tone);
                }
                for (int i = 0; i < 4; i++)
                    Spawn(Get("cekic/dust", "Boosters/Hammer/impact/07"), target + Random.insideUnitCircle * cell * .25f, Random.insideUnitCircle * 60 * _s, 0,
                        cell * .7f, cell * 1.3f, .55f, -40 * _s, 2f, new Color(1, .96f, .88f, .8f));
            }
        }

        void ScissorsSlow(float age, Vector2 target, bool vertical)
        {
            var slot = _bar.RectOf(Booster.Scissors).center;
            float cell = _board.Cell, size = Mathf.Clamp(cell * 1.6f, 92 * _s, 128 * _s);
            float seamAngle = vertical ? -90 : 0;
            var tip = vertical ? new Vector2(0, 1) : new Vector2(1, 0);       // the blades point along the seam
            // the pivot sits behind the tab so the blades' cutting point (a fifth of the size ahead) is on it
            var cutAt = target - tip * size * .2f;
            var waitAt = cutAt - tip * size * .16f;                             // open, a little back from the tab
            Vector2 point; float angle = seamAngle, scale = 1, alpha = 1, open = 0;
            if (age < Snip.Arrive)
            {
                float t = InOutCubic(Span(age, 0, Snip.Arrive));
                point = Vector2.Lerp(slot, waitAt, t) - Vector2.up * (Mathf.Sin(t * Mathf.PI) * 60 * _s);
                scale = Mathf.Lerp(.5f, 1, t); angle = seamAngle + 30 * (1 - t);
            }
            else if (age < Snip.Open)
            {
                // the jaws open slowly
                point = waitAt; open = InOutCubic(Span(age, Snip.Arrive, Snip.Open));
            }
            else if (age < Snip.Close)
            {
                // in around the tab, wide open
                point = Vector2.Lerp(waitAt, cutAt, InOutCubic(Span(age, Snip.Open, Snip.Close))); open = 1;
            }
            else if (age < Snip.Break)
            {
                // the cut: the blades close through the tab, slow then biting (≈ .5 s)
                float t = Span(age, Snip.Close, Snip.Break);
                open = 1 - (t < .7f ? .55f * (t / .7f) : .55f + .45f * InCubic((t - .7f) / .3f));
                point = cutAt + Random.insideUnitCircle * 1.2f * _s * Mathf.Sin(t * Mathf.PI);
                if (t > .05f && Once(5)) { Haptics.Play(Haptics.Kind.Light); Sound?.Invoke("pm72-tick", .5f); }
            }
            else if (age < Snip.Leave) { point = cutAt; open = 0; }
            else
            {
                // home to its slot (no spin), shrinking into the icon
                float t = InOutCubic(Span(age, Snip.Leave, Snip.End));
                point = Vector2.Lerp(cutAt, slot, t) - Vector2.up * (Mathf.Sin(t * Mathf.PI) * 70 * _s);
                scale = Mathf.Lerp(1, .5f, t); angle = seamAngle + 30 * t;
                alpha = 1 - Span(age, Snip.End - .06f, Snip.End);
            }
            var upper = Get75("makas/blade_upper"); var lower = Get75("makas/blade_lower");
            if (upper != null && lower != null)
            {
                float a = Snip.Wide * open;
                Paint(_tool, upper, point, Vector2.one * size * scale, alpha, angle + a);
                Paint(_blade2, lower, point, Vector2.one * size * scale, alpha, angle - a);
                Paint(_pivot, Get75("makas/pivot"), point, Vector2.one * size * scale * .25f, alpha, angle);
            }
            else
            {
                string pose = open > .66f ? "open" : open > .2f ? "half" : "closed";
                Paint(_tool, Get("makas/" + pose, pose == "open" ? "Boosters/Scissors/walk/04" : pose == "half" ? "Boosters/Scissors/walk/03" : "Boosters/Scissors/walk/01"),
                    point, Vector2.one * size * scale, alpha, angle);
            }
            if (age >= Snip.Break - .02f && Once(0))
            {
                Haptics.Play(Haptics.Kind.Medium);
                Sound?.Invoke("pm72-unlock", 1f);
                Shake(4 * _s, .12f);
                // the tab breaks off: Codex's knobbed shard in the picture's tone, falling away; crumbs and sparks
                var tab = Get("cekic/shard_c");
                var dir = vertical ? new Vector2(.8f, -.7f) : new Vector2(.4f, -1);
                if (tab != null) Spawn(tab, target, dir.normalized * 240 * _s, 360, cell * .5f, cell * .38f, .8f, 1300 * _s, .4f, _tone);
                else SpawnDisc(target, dir.normalized * 220 * _s, cell * .26f, .7f, _tone);
                var bits = new[] { Get("cekic/shard_a"), Get("cekic/shard_b"), Get("cekic/spark") };
                for (int i = 0; i < 8; i++)
                {
                    float r = Random.Range(0, Mathf.PI * 2), v = Random.Range(120, 260) * _s;
                    var bit = bits[i % 3]; bool gold = i % 3 == 2;
                    var vel = new Vector2(Mathf.Cos(r), Mathf.Sin(r) - .6f) * v;
                    if (bit != null) Spawn(bit, target, vel, Random.Range(-600, 600), cell * (gold ? .2f : Random.Range(.14f, .22f)), cell * .08f, .5f, gold ? 0 : 1200 * _s, gold ? 3f : .5f, gold ? Color.white : _tone);
                    else SpawnSquare(target, vel, cell * Random.Range(.07f, .12f), .5f, _tone);
                }
            }
            if (age >= Snip.Break - .02f && age < Snip.Break + .2f)
            {
                float f = Span(age, Snip.Break - .02f, Snip.Break + .2f);
                Paint(_flash, Get("makas/snip_flash", "Boosters/Hammer/impact/03"), target, Vector2.one * cell * Mathf.Lerp(.7f, 1.3f, OutCubic(f)), 1 - f, seamAngle + 45);
            }
        }

        // ---- scissors -------------------------------------------------------------------------------------------

        void Scissors(float age, Vector2 target, bool vertical)
        {
            var idea = ScissorsIdea;
            if (idea == Idea.A) { ScissorsSlow(age, target, vertical); return; }
            var slot = _bar.RectOf(Booster.Scissors);
            float cell = _board.Cell, size = Mathf.Clamp(cell * 1.5f, 84 * _s, 118 * _s);
            // the blades point along the seam they cut across: a vertical joint (left|right pieces) is cut top-down
            float seamAngle = vertical ? -90 : 0;
            var back = vertical ? new Vector2(0, -1) : new Vector2(-1, 0);
            var rest = target + back * size * .18f;
            float snap = ScissorsHit(idea), end = ScissorsDone(idea);
            Vector2 point; float angle = seamAngle, scale = 1, alpha = 1; string pose = "closed";
            if (idea == Idea.C)
            {
                // a dash along the seam: in from beyond the board, open on the way, shut on the joint, out the far side
                var from = rest + back * (cell * 4.5f); var to = rest - back * (cell * 5f);
                if (age < snap)
                {
                    float t = Span(age, 0, snap);
                    point = Vector2.Lerp(from, rest, OutCubic(t)); alpha = Mathf.Clamp01(t * 4);
                    pose = t < .35f ? "closed" : t < .7f ? "half" : "open";
                }
                else if (age < snap + .06f) { point = rest; pose = "closed"; }
                else
                {
                    float t = Span(age, snap + .06f, end);
                    point = Vector2.Lerp(rest, to, InCubic(t)); alpha = 1 - InCubic(t); pose = "closed";
                }
                float trail = age < end ? Mathf.Clamp01(1 - Mathf.Abs(age - snap) / .3f) : 0;
                var dir = -back;
                Paint(_ring, Get("degnek/trail"), point - dir * size * .55f, new Vector2(size * 1.5f, size * .34f), .75f * trail,
                    vertical ? -90 : 0, Color.white);
            }
            else if (age < ScissorsArrive)
            {
                float t = OutCubic(Span(age, 0, ScissorsArrive));
                point = Vector2.Lerp(slot.center, rest + back * size * .14f, t) - Vector2.up * (Mathf.Sin(t * Mathf.PI) * 60 * _s);
                scale = Mathf.Lerp(.55f, 1, t); angle = Mathf.Lerp(seamAngle + 40, seamAngle, t);
            }
            else if (idea == Idea.B && age < snap)
            {
                // two snips: open, shut (a small snip), open, shut (the cut)
                float mid = (ScissorsArrive + snap) / 2;
                float t = age < mid ? Span(age, ScissorsArrive, mid) : Span(age, mid, snap);
                pose = t < .3f ? "half" : t < .75f ? "open" : "closed";
                point = rest + back * size * (age < mid ? .1f : .04f) * (1 - t);
                if (age >= mid - .02f && Once(5)) { Haptics.Play(Haptics.Kind.Light); Sound?.Invoke("pm72-tick", .7f); }
            }
            else if (age < snap - .06f)
            {
                // the jaws open wide, a small pull back (anticipation)
                float t = OutCubic(Span(age, ScissorsArrive, snap - .06f));
                point = rest + back * size * (.14f + .05f * t); pose = t < .45f ? "half" : "open";
            }
            else if (age < snap)
            {
                // snap: forward and shut
                float t = InCubic(Span(age, snap - .06f, snap));
                point = rest + back * size * Mathf.Lerp(.19f, 0, t); pose = t < .5f ? "half" : "closed";
            }
            else if (age < snap + .16f)
            {
                float t = Span(age, snap, snap + .16f);
                point = rest + Random.insideUnitCircle * 2.5f * _s * (1 - t);
            }
            else
            {
                // spin away, up and right, fading
                float t = Span(age, snap + .16f, end);
                float e = InCubic(t);
                point = rest + new Vector2(220 * _s * e, -60 * _s * Mathf.Sin(t * Mathf.PI) - 260 * _s * e);
                angle = seamAngle - 400 * OutCubic(t); scale = 1 - .25f * t; alpha = 1 - e;
            }
            var sprite = Get("makas/" + pose, pose == "open" ? "Boosters/Scissors/walk/04" : pose == "half" ? "Boosters/Scissors/walk/03" : "Boosters/Scissors/walk/01");
            Paint(_tool, sprite, point, Vector2.one * size * scale, alpha, angle);
            if (age >= snap - .02f)
            {
                if (Once(0))
                {
                    Haptics.Play(Haptics.Kind.Medium);
                    Sound?.Invoke("pm72-unlock", 1f);
                    Shake(4 * _s, .12f);
                    // the cut tab flies off, its bits scatter
                    // the cut tab: Codex's knobbed shard in the picture's tone, big enough to read, spinning off
                    var tab = Get("cekic/shard_c");
                    var dir = vertical ? new Vector2(1, -.6f) : new Vector2(.5f, -1);
                    if (tab != null) Spawn(tab, target, dir.normalized * 300 * _s, 540, cell * .55f, cell * .4f, .7f, 1300 * _s, .4f, _tone);
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
                float f = Span(age, snap - .02f, snap + .2f);
                Paint(_flash, Get("makas/snip_flash", "Boosters/Hammer/impact/03"), target, Vector2.one * cell * Mathf.Lerp(.8f, 1.5f, OutCubic(f)), 1 - f, seamAngle + 45);
            }
        }

        // ---- wand -----------------------------------------------------------------------------------------------

        float _trailAt;
        void Wand(float age, Vector2 target)
        {
            var idea = WandIdea;
            var slot = _bar.RectOf(Booster.Wand);
            float cell = _board.Cell, size = Mathf.Clamp(cell * 1.6f, 80 * _s, 110 * _s);
            float radius = cell * .9f, hit = WandHitAt(idea), end = WandDone(idea);
            float arrive = idea == Idea.A ? Swirl.Arrive : WandArrive;   // PM_75: A slower, and it flies home
            Vector2 point; float angle = 20, alpha = 1, scale = 1;
            var tipOffset = new Vector2(-.18f, -.32f);
            if (idea == Idea.C)
            {
                var a0 = target + new Vector2(-1.3f, -1.1f) * cell; var a1 = target + new Vector2(1.3f, 1.1f) * cell;
                if (age < .26f) { float t = OutCubic(Span(age, 0, .26f)); point = Vector2.Lerp(slot.center, a0, t) - Vector2.up * (Mathf.Sin(t * Mathf.PI) * 90 * _s); scale = Mathf.Lerp(.6f, 1, t); }
                else if (age < hit) { float t = InOutCubic(Span(age, .26f, hit)); point = Vector2.Lerp(a0, a1, t); angle = 20 - 40 * t; }
                else { float t = Span(age, hit, end); float e = InCubic(t); point = a1 + new Vector2(_screen.x * .7f * e, -120 * _s * Mathf.Sin(t * Mathf.PI)); angle = -20 - 140 * e; alpha = 1 - e; }
                // the sweep's trail: a ribbon from the start corner to the wand
                if (age >= .26f && age < hit + .3f)
                {
                    var head = point + tipOffset * size; var tail = a0 + tipOffset * size;
                    var d = head - tail; float len = d.magnitude;
                    if (len > 4)
                        Paint(_ring, Get("degnek/trail"), (head + tail) / 2, new Vector2(len, size * .4f), age < hit ? 1 : 1 - (age - hit) / .3f,
                            -Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg);
                }
            }
            else if (age < arrive)
            {
                float t = OutCubic(Span(age, 0, arrive));
                var dest = idea == Idea.B ? target + new Vector2(cell * .35f, -cell * .55f) : target + new Vector2(radius, 0);
                point = Vector2.Lerp(slot.center, dest, t) - Vector2.up * (Mathf.Sin(t * Mathf.PI) * 110 * _s);
                angle = Mathf.Lerp(-20, 20, t); scale = Mathf.Lerp(.6f, 1, t);
            }
            else if (idea == Idea.B && age < hit + .1f)
            {
                // three taps over the region: a dip and a burst each
                var home = target + new Vector2(cell * .35f, -cell * .55f);
                float[] taps = { .46f, .61f, hit };
                float dip = 0;
                for (int k = 0; k < 3; k++)
                {
                    float d = age - taps[k];
                    if (d > -.07f && d < .07f) dip = Mathf.Max(dip, 1 - Mathf.Abs(d) / .07f);
                    if (d >= 0 && Once(6 + k))
                    {
                        Haptics.Play(Haptics.Kind.Light);
                        var at = target + new Vector2(k == 0 ? -.6f : k == 1 ? .6f : 0, k == 2 ? .5f : -.3f) * cell;
                        for (int i = 0; i < 6; i++)
                        {
                            float a = i / 6f * Mathf.PI * 2;
                            Spawn(Get("degnek/sparkle", "Boosters/Hammer/impact/03"), at, new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * 150 * _s, 300, 14 * _s, 3 * _s, .45f, 0, 3f,
                                i % 2 == 0 ? new Color(1, .9f, .5f) : new Color(.86f, .7f, 1f));
                        }
                    }
                }
                point = home + new Vector2(-6, 14) * _s * dip; angle = 20 - 25 * dip;
            }
            else if (idea == Idea.A && age < hit + .18f)
            {
                // one loop around the region's centre, drawing the swirl
                float t = InOutCubic(Span(age, arrive, hit + .18f));
                float a = t * Mathf.PI * 2;
                point = target + new Vector2(Mathf.Cos(a), -Mathf.Sin(a)) * radius;
                angle = 20 + 30 * Mathf.Sin(a * 2);
            }
            else if (idea == Idea.A)
            {
                // PM_75: home to its slot, shrinking into the icon
                float t = InOutCubic(Span(age, hit + .18f, end));
                var leave = target + new Vector2(radius, 0);
                point = Vector2.Lerp(leave, slot.center, t) - Vector2.up * (Mathf.Sin(t * Mathf.PI) * 80 * _s);
                angle = Mathf.Lerp(20, -20, t); scale = Mathf.Lerp(1, .55f, t); alpha = 1 - Span(age, end - .06f, end);
            }
            else
            {
                float from = idea == Idea.B ? hit + .1f : hit + .18f;
                float t = Span(age, from, end);
                float e = InCubic(t);
                var leave = idea == Idea.B ? target + new Vector2(cell * .35f, -cell * .55f) : target + new Vector2(radius, 0);
                point = leave + new Vector2(_screen.x * .9f * e, -80 * _s * Mathf.Sin(t * Mathf.PI));
                angle = 20 - 160 * e; alpha = 1 - e;
            }
            Paint(_tool, Get("degnek/wand", "Boosters/Wand/tool/01"), point, Vector2.one * size * scale, alpha, angle);
            // sparkles fall off the star along the path
            var tip = point + tipOffset * size;
            if (age < hit + .3f && Time.unscaledTime - _trailAt > .025f)
            {
                _trailAt = Time.unscaledTime;
                Spawn(Get("degnek/sparkle", "Boosters/Hammer/impact/03"), tip + Random.insideUnitCircle * 6 * _s, Random.insideUnitCircle * 40 * _s + new Vector2(0, 30 * _s),
                    Random.Range(-200, 200), Random.Range(10, 18) * _s, 2 * _s, .45f, 120 * _s, 1f, Random.value < .5f ? new Color(1, .9f, .5f) : new Color(.86f, .7f, 1f));
            }
            // the swirl: drawn by the loop (A), or bursting at the change (B, C)
            float swirlFrom = idea == Idea.A ? arrive : hit - .1f;
            if (age >= swirlFrom)
            {
                float f = Span(age, swirlFrom, hit + .5f);
                var swirl = Get(f < .2f ? "degnek/swirl_01" : f < .4f ? "degnek/swirl_02" : f < .55f ? "degnek/swirl_03" : f < .7f ? "degnek/swirl_04" : f < .85f ? "degnek/swirl_05" : "degnek/swirl_06",
                    "Boosters/Wand/touch-burst/0" + (1 + Mathf.Min(8, (int)(f * 9))));
                if (idea != Idea.C || age >= hit - .1f)
                    Paint(idea == Idea.C ? _flash : _ring, swirl, target, Vector2.one * radius * 2.6f * Mathf.Lerp(.7f, 1.15f, OutCubic(f)), f < .8f ? 1 : (1 - f) * 5, -360 * f);
            }
            if (age >= hit && Once(0))
            {
                Haptics.Play(Haptics.Kind.Light);   // PM_86 §4: the wand's turn is three light waves (the third here)
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
                Frost89(panel, 0, false, true);
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
                    Haptics.Play(Haptics.Kind.Medium);   // PM_86 §4: the clock freezing
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
                // "+10 sn" pops big over the box (clear of the notch), holds, then drops into the chip under it
                float p = Span(age, touch + .05f, touch + .95f);
                if (p < 1)
                {
                    _plus.gameObject.SetActive(true);
                    _plus.Text = Lang.T("+10 s", "+10 sn");
                    float pop = p < .28f ? OutBack(p / .28f) : 1;
                    float drop = InOutCubic(Span(p, .6f, 1));
                    _plus.transform.localPosition = Draw.W(Vector2.Lerp(panel + new Vector2(0, 4 * _s), ChipCenter(panel), drop));
                    _plus.FitWidth(104 * _s, 18 * _s);   // inside the 113 pt box
                    _plus.transform.localScale = Vector3.one * Mathf.Max(.01f, pop * Mathf.Lerp(1, .6f, drop));
                    _plus.Alpha = p < .85f ? 1 : 1 - (p - .85f) / .15f;
                }
                else _plus.gameObject.SetActive(false);
            }
            else { _frost.enabled = false; _plus.gameObject.SetActive(false); }

            // while the time is frozen: the box glazed, the chip under it counting the bonus seconds down
            _clockOwnsGlaze = frozen && age >= touch;
            if (Frost89On)
            {
                // PM_89 §2: the ice bar under the box and the box's frosted rim (Frost89) instead of the glaze and the chip
                _glaze.enabled = false; Chip(panel, 0, false);
                Frost89(panel, freezeLeft, _clockOwnsGlaze, false);
                return true;
            }
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

        // PM_75: just under the new time bar (Hud54.TimeBar), which shows the pause too
        Vector2 ChipCenter(Vector2 panel) => Frost89On ? FrostBarCenter(panel) : panel + new Vector2(0, 70f) * _s;

        // ---- PM_89 §2: the clock's ice bar ----------------------------------------------------------------------
        /* The green bar under the Time box is gone; only while the clock booster holds the time an ice bar shows there
           (Codex option A, Art/yeni/pm89/frost: a navy groove with a gold rim, an ice fill, frost). It empties from the
           right (PM_90: towards the left) over the freeze, ice crystals glint on it, and when the time runs again it cracks (three stages) and
           breaks into shards with a crackle and a light tap. The box wears a frosted rim while frozen that thaws away. */
        public static bool Frost89Allowed = true;   // &buzbar=0: the PM_74 glaze and chip
        bool Frost89On => Frost89Allowed && Get89("track") != null && Get89("fill") != null;
        const float FrostW = 100, FrostH = 14, FrostCrack = .32f, FrostBreak = .62f, FrostIn = .22f;
        SpriteRenderer _fTrack, _fFill, _fCrack, _fPanel;
        readonly SpriteRenderer[] _fGlint = new SpriteRenderer[3];
        float _fShownAt = -1, _fBreakAt = -1, _fPeak = 1, _fPanelA;
        bool _fWas, _fShattered;

        Sprite Get89(string key)
        {
            if (!_sprites.TryGetValue("89/" + key, out var sprite)) _sprites["89/" + key] = sprite = Resources.Load<Sprite>("Art/yeni/pm89/frost/" + key);
            return sprite;
        }

        public static Vector2 FrostBarCenter(Vector2 panel, float s) => panel + new Vector2(0, 290f / 6 - .5f) * s;
        Vector2 FrostBarCenter(Vector2 panel) => FrostBarCenter(panel, _s);

        static void Sliced(SpriteRenderer r, Sprite sp, Vector2 point, Vector2 size, float alpha)
        {
            if (sp == null || alpha <= 0 || size.x <= 0) { r.enabled = false; return; }
            r.sprite = sp; r.enabled = true; r.drawMode = SpriteDrawMode.Sliced;
            r.transform.localPosition = Draw.W(point); r.transform.localRotation = Quaternion.identity; r.transform.localScale = Vector3.one;
            r.size = size; r.color = new Color(1, 1, 1, Mathf.Clamp01(alpha));
        }

        void Frost89(Vector2 panel, double freezeLeft, bool frozen, bool stopped)
        {
            if (_fTrack == null)
            {
                _fTrack = Layer("fx ice bar", Order + 1); _fFill = Layer("fx ice bar fill", Order + 2); _fCrack = Layer("fx ice bar crack", Order + 3);
                _fPanel = Layer("fx ice rim", Order + 1);
                for (int i = 0; i < _fGlint.Length; i++) _fGlint[i] = Layer("fx ice glint " + i, Order + 3);
            }
            float now = Time.unscaledTime;
            var c = FrostBarCenter(panel); var size = new Vector2(FrostW, FrostH) * _s;
            if (stopped) { HideFrost(); _fWas = false; _fBreakAt = -1; return; }
            if (frozen)
            {
                if (!_fWas) { _fShownAt = now; _fPeak = Mathf.Max(1, (float)freezeLeft); _fBreakAt = -1; }
                _fWas = true;
                _fPeak = Mathf.Max(_fPeak, (float)freezeLeft);
                float k = OutBack(Span(now, _fShownAt, _fShownAt + FrostIn), 1.2f);   // pops open
                Sliced(_fTrack, Get89("track"), c, new Vector2(size.x, size.y * Mathf.Max(.05f, k)), Mathf.Clamp01(k * 2));
                // the fill: the share of the freeze left, from the left (PM_90 §2: its right end draws back to the left)
                float share = Mathf.Clamp01((float)freezeLeft / _fPeak), inner = size.x - 4 * _s, h = size.y * 60 / 84 * Mathf.Max(.05f, k);
                float w = inner * share, minW = 10 * _s;
                Sliced(_fFill, Get89("fill"), new Vector2(c.x - inner / 2 + Mathf.Max(w, minW) / 2, c.y), new Vector2(Mathf.Max(w, minW), h), w >= minW ? 1 : w / minW);
                _fCrack.enabled = false;
                // three glints wandering along what is left of the fill
                for (int i = 0; i < _fGlint.Length; i++)
                {
                    float ph = Mathf.Repeat(now * .9f + i * .37f, 1), a = Mathf.Sin(ph * Mathf.PI);
                    float x = c.x - inner / 2 + w * Mathf.Repeat(.18f + i * .31f + Mathf.Floor(now * .9f + i * .37f) * .53f, 1);
                    Paint(_fGlint[i], Get89("crystal"), new Vector2(x, c.y - 1 * _s), Vector2.one * (9 + 5 * a) * _s, a * a * Mathf.Clamp01(k) * (w > minW ? 1 : 0), now * 60 + i * 40);
                }
                _fPanelA = Mathf.Clamp01((now - _fShownAt) / .25f);
                Paint(_fPanel, Get89("panel_frost"), panel, new Vector2(113.3f, 97.3f) * _s, _fPanelA * .95f);
                return;
            }
            if (_fWas)
            {
                // the time runs again: crack, break, thaw
                _fWas = false; _fBreakAt = now; _fShattered = false;
                Sound?.Invoke("pm89-ice", .9f);
            }
            if (_fBreakAt < 0) { HideFrost(); return; }
            float t = now - _fBreakAt;
            if (t >= FrostBreak + .3f) { HideFrost(); _fBreakAt = -1; return; }
            _fFill.enabled = false;
            foreach (var g in _fGlint) g.enabled = false;
            float gone = Span(t, FrostCrack, FrostBreak);
            Sliced(_fTrack, Get89("track"), c, size * (1 + .04f * Mathf.Sin(Mathf.Clamp01(t / FrostCrack) * Mathf.PI)), 1 - gone);
            int stage = t < .1f ? 1 : t < .2f ? 2 : 3;
            Paint(_fCrack, Get89("crack_" + stage), c, size, (1 - gone) * Mathf.Clamp01(t / .04f));
            if (t >= FrostCrack && !_fShattered)
            {
                _fShattered = true;
                Haptics.Play(Haptics.Kind.Light);
                Shake(1.5f * _s, .1f);
                for (int i = 0; i < 12; i++)
                {
                    float x = Mathf.Lerp(-.45f, .45f, (i + Random.Range(0f, 1f)) / 12f);
                    var p = c + new Vector2(x * size.x, Random.Range(-.3f, .3f) * size.y);
                    var v = new Vector2(x * 260 + Random.Range(-40, 40), Random.Range(-170, -60)) * _s;
                    Spawn(Get89("shard_" + (1 + i % 6)), p, v, Random.Range(-420, 420), Random.Range(7, 12) * _s, 4 * _s, Random.Range(.55f, .8f), 900 * _s, .6f, Color.white);
                }
            }
            // the box's rim thaws: it fades and lifts a hair
            float thaw = Span(t, 0, .45f);
            Paint(_fPanel, Get89("panel_frost"), panel - new Vector2(0, 2 * thaw) * _s, new Vector2(113.3f, 97.3f) * _s * (1 + .02f * thaw), _fPanelA * .95f * (1 - thaw));
        }

        void HideFrost()
        {
            if (_fTrack == null) return;
            _fTrack.enabled = _fFill.enabled = _fCrack.enabled = _fPanel.enabled = false;
            foreach (var g in _fGlint) g.enabled = false;
        }

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
