using System;
using System.Collections.Generic;
using ReverseSolver.Core;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* The dive into a painting (PM_75; PM_80, Zeyd: "the camera's move a bit more continuous; once there let it wait
       a little, coming straight back was not good; when we get there the lines start to come out slowly, and we keep
       seeing them on the way back. The best, smoothest version").
       One timeline, one camera curve; no step changes the speed, except the wait put there on purpose (A, the default):
         flight    1.8 s  Zeyd's path through the menu picture (PM_78, Diorama78) by its length: a soft start, then
                          an even flow
         approach   .8 s  on into the painting, taking over the flight's speed and direction (quintic Hermite: the
                          position, the speed and the acceleration carry over); it slows softly as the picture fills
                          the screen and settles into
         hold       .8 s  only the picture, a slow 1.5 % push (never frozen); the jigsaw cuts start drawing from the
                          centre outwards, ~40 % by the end of it. The level is built as the approach comes to rest.
         back      1.0 s  out to the board, ease in-out; the cuts keep drawing, done at 80 %, the pieces take over;
                          the HUD and the booster bar over its last .3 s
       about 4.4 s. &dalis=b quicker (1.5 / .6 / .5 / .8), &dalis=c more cinematic (2.0 / .9 / 1.1 / 1.2, cuts 50 %
       at the end of the hold). &yayhiz= scales all of it, &resimbekle= sets the hold (s), &cizgihiz= the cuts' speed.
       &gecis=eski: PM_77's way (no flight, 1.1 / .5 / 1.0 / .4 s with breaths). A tap makes it twice as fast; a
       second tap ends it. The screen-space layers ride on the camera, so the camera can already sit on the board
       while the picture still covers the screen. */
    public sealed class Dive75 : MonoBehaviour
    {
        public const float CrossIn = .35f;
        public static bool Legacy;            // &gecis=eski: PM_77's dive (no flight, the old steps)
        public static float SpeedK = 1;       // &yayhiz=
        public static float HoldSec = -1;     // &resimbekle= (s; below 0: the variant's)
        public static float LineK = 1;        // &cizgihiz=
        public static char Variant = 'a';     // &dalis=
        public static float BellSeconds = 2.2f;   // PM_86 §10: the menu's flight and approach, one smootherstep (s)
        public static float DiveSpeed = 1;        // &dalishiz= (a factor on it)
        public static float FaceGate = .7f;   // PM_87: the corridor's approach share after which the picture eases to the board
        public static float LinesDone = .7f;  // PM_84/85: the share of the pull back by which the cuts are all drawn
        public static float CorridorApproach = .6f;   // PM_85: after the corridor (no flight) the approach is .6 s
        public static bool Log;               // &dalislog=1: every frame's painting / board corners on screen, for the speed graphs
        /* PM_91 (Zeyd: "it goes well at first but slows down as it nears the painting"): the approach keeps its speed
           into the painting (PM_92: rising all the way, see MenuSeconds92; the old curves came to rest), and the
           picture holds half as long (PM_90 measured .55 s from the menu, .43 s after the corridor).
           &dalis91=0 the PM_90 timing. */
        public static bool Arrive91 = true;
        public static float TouchMax = .025f;   // &degme=: on arrival the camera runs on into the picture by at most this
        const float TouchSeconds = .04f;        // (its speed falling off over about this), then holds
        const float MenuHold91 = .285f, CorridorHold91 = .227f;   // the picture covers the screen as the approach arrives
        /* PM_92 (Zeyd on PM_91: "somewhere in the middle it feels as if it slows down, a struggle, then it flows; go
           straight into the wall with the acceleration it set off with"): the approach's speed is set as the eye sees
           it, the wall's motion on the screen (its growth and its slide along the arc together, Flow92), and only
           rises: from the old curve's own start, through RiseFrom92 of the time at 1, to Rise92 x that at Peak92 of the
           time, then lands at Land92 x it. A cubic in each stretch (the speed and the acceleration have no step); the
           way is laid out by that screen distance, so the arc handing over to the approach has no step either. */
        const float MenuSeconds92 = 1.5f, CorridorSeconds92 = .5f;
        const float RiseFrom92 = .35f, Rise92 = 1.5f, Peak92 = .95f, Land92 = .87f;
        /* after the corridor the dive sets off from rest: its first stretch ends with the acceleration it has then
           (the line on from there meets 0 at half that time: Rise = 1 + 2 (Peak - RiseFrom) / RiseFrom), so it keeps
           accelerating evenly into the wall, no elbow (PM_92's first try eased off at .17 s, like PM_91's at .2 s) */
        const float CorridorRiseFrom92 = .2f, CorridorRise92 = 1 + 2 * (Peak92 - CorridorRiseFrom92) / CorridorRiseFrom92;
        /* PM_108 (Zeyd's drawing, 25 Nov: "it seemed slow at the start"): the menu's dive (the arc and the approach) in
           1.2 s, its speed on the screen (Flow92, the way as PM_92 lays it out) off at once at Start108 of the peak, up
           to the peak by Rise108 of the time (a cubic, the acceleration with no step), the peak held to Hold108, down
           to 1 / Peak108 of it by Fall108, then even to the arrival; the last EndEase108 s ease to EndLand108 of it (the
           touch, PM_91, then takes over at that speed). &dalis108=0 PM_92's; &dalistepe= the peak over the even speed;
           &dalishiz= still a factor on the speed (1.25: 0.96 s). The corridor's dive is not changed. */
        public static bool Dive108 = true;
        public static float Peak108 = 1.65f;
        const float MenuSeconds108 = 1.2f, Start108 = .55f, Rise108 = .25f, Hold108 = .45f, Fall108 = .60f;
        const float EndEase108 = .03f, EndLand108 = .85f;
        bool _plan108;
        float[] _speed108;   // the design's speed over the time (1 at the peak), for the dive's air
        /* PM_109 (Zeyd's second drawing, 25 Nov 10:47): off lower (Start109 of the peak) but up steeply, to Climb109
           of it by Rise109 of the time; then a long high plateau rising slowly to the peak by Top109, the peak held to
           Hold109, down to 1 / Peak108 of it by Fall109 (later than PM_108's), then even, and the same last ease and
           touch. Its time is what that shape takes at PM_108's even speed (the same way, the same low approach):
           Seconds109, kept within .9..1.1 s. &dalis109=0 PM_108's; &dalistepe= and &dalishiz= as there. */
        public static bool Dive109 = true;
        const float Start109 = .40f, Climb109 = .85f, Rise109 = .10f, Top109 = .65f, Hold109 = .72f, Fall109 = .82f;
        bool _plan109;

        struct Plan { public float Flight, Approach, Hold, Back, Lines, Push; }
        static Plan Current
        {
            get
            {
                // PM_85: A holds .4 s (the cuts start as soon as the picture fills the screen), pulls back in .8 s
                var p = Variant == 'b' ? new Plan { Flight = 1.5f, Approach = .6f, Hold = .4f, Back = .8f, Lines = .25f }
                      : Variant == 'c' ? new Plan { Flight = 2.0f, Approach = .9f, Hold = 1.1f, Back = 1.2f, Lines = .5f }
                      : new Plan { Flight = 1.8f, Approach = .8f, Hold = .4f, Back = .8f, Lines = .25f };
                if (HoldSec >= 0) p.Hold = HoldSec;
                p.Push = p.Hold > .01f ? .015f * p.Hold / .8f : 0;   // 1.5 % over A's .8 s hold, the same slow rate in all
                return p;
            }
        }
        public static bool Gap;   // &dalisbosluk=1: a magenta backdrop, to count uncovered edge pixels in captures
        public static bool On = true;
        public static bool Orbit;   // PM_77 &gecis=yay

        public sealed class Setup
        {
            public Camera Cam;
            public Vector2 Screen;
            public Vector2[] Quad;               // the painting's opening at the start, layout points: TL, TR, BR, BL
            public Rect Crop = new Rect(0, 0, 1, 1);   // the picture's uv shown in the opening
            public Texture Picture;
            public float BoardAspect = 1;        // width / height of the next level's board
            public List<SpriteRenderer> Walls = new List<SpriteRenderer>();   // copied and warped with the wall plane
            public int Order;                    // at the start: just over the wall it copies (the menu's controls fade above it)
            public int TopOrder;                 // once those have faded: over everything
            public Color Backdrop = new Color(.05f, .09f, .16f, 1);
            public Action<float> FadeUi;         // the start screen's own controls, 1 -> 0 at the start
            public Action Hide;                  // the originals of the copied walls (and their painting) go away
            public Func<BoardView> Build;        // builds the level (during the hold); its board, or null
            public Action<float> Slide;          // the HUD and bar, 0 (out) .. 1 (home)
            public Action Done;
            public Diorama78.Art Diorama;        // PM_78 &gecis=yay: Zeyd's path through the menu picture before the dive
            public bool Seamless;                // the menu shows the diorama's start view itself: no cross-fade
            public Vector2 ImageCentre;          // the menu picture's centre (layout points) and
            public float ImageK = 1;             // its scale (layout points per picture px): the diorama's start camera
        }

        Setup _s;
        Transform _overlay;
        readonly List<Layer> _walls = new List<Layer>();
        Layer _paint;
        ShapeView _backdrop;
        float _clock, _speed = 1, _homeSize;
        Vector3 _homePos;
        int _taps;
        bool _built, _revealed, _finished, _raised;
        BoardView _board;
        Reveal _reveal;
        Rect _r, _c;
        double[] _faceH;
        Diorama78 _dio;
        readonly List<Layer> _dioLayers = new List<Layer>();
        Rect _cropFront;
        Plan _p;
        float _bellT, _bellSplit, _morphFrom;   // PM_86: the bell's length, the flight's share of it; where the morph may start
        float _flight, _v, _ramp, _zEnd, _rate;   // PM_80: the flight's length speed (W/s) after its soft start (s); the
                                                  // approach's end distance; the hold's push (ln of the size per s)
        Layer _ghost;                             // PM_80: last frame's painting, faint, while it moves fast
        Rect _endFix;                             // PM_80: the board's framing less the canvas's at the approach's end
        bool _ghostOn;
        /* PM_88 §1 (Zeyd: "at the end the picture grows for a moment, then turns"): the board is built right under the
           picture as it is on the screen when the camera arrives: the same place, size and crop (the camera is put
           there; _fitA/_fitX/_fitY: the board's whole picture on the screen, x = a u + bx, y = a / aspect (1 - v) + by).
           What a camera cannot match (the menu's flat drawing straightened by the wall: a few % of slant) is taken up
           over the whole approach, never squeezed into its end. */
        float _fitA, _fitX, _fitY;
        bool _new91, _calm;                // PM_91: the new timing; the frame after the level is built does not jump
        float _lensT, _step, _touch, _touchTau;   // the old lens shift's length; the last frame's step; the arrival's touch
        bool _fit;
        Func<float, float, Vector2> _endAt, _gluedUv;

        public static Dive75 Run(Setup s)
        {
            var d = new GameObject("PM75 dive").AddComponent<Dive75>();
            d._s = s;
            d._homePos = s.Cam.transform.position; d._homeSize = s.Cam.orthographicSize;
            var clock = System.Diagnostics.Stopwatch.StartNew();
            d.Prepare();
            d.Note($"prepare {clock.Elapsed.TotalMilliseconds:0} ms");
            if (s.Diorama == null || s.Seamless) s.Hide?.Invoke();   // otherwise the diorama fades in over the menu first (CrossIn)
            d._lastFrame = Time.frameCount;      // the clock starts on the next frame
            d.Step();
            return d;
        }

        /* A tap: twice as fast (a first visit is not in a hurry; a returning player may be); the second one ends it. */
        public void Tap()
        {
            _taps++;
            if (_taps == 1) _speed = 2; else Finish();
        }

        void Prepare()
        {
            var screen = _s.Screen;
            _overlay = new GameObject("overlay").transform;
            _overlay.SetParent(_s.Cam.transform, false);
            _backdrop = ShapeView.Create(_overlay, "backdrop", screen * 1.6f, 0, _s.Order).Radius(0).Fill(_s.Backdrop);
            _backdrop.transform.localPosition = Draw.W(screen / 2);
            Material baseMat = null;
            foreach (var w in _s.Walls)
            {
                if (w == null || w.sprite == null) continue;
                baseMat = baseMat ?? w.sharedMaterial;
                _walls.Add(Layer.FromSprite(_overlay, w, _s.Order + 1, screen));
            }
            baseMat = baseMat ?? SpriteMaterial();
            if (Gap) _backdrop.Fill(new Color(1, 0, 1, 1));
            _p = Current;
            if (_s.Diorama == null && Variant == 'a') _p.Approach = CorridorApproach;
            _new91 = Arrive91 && !Legacy && Variant == 'a';
            if (_new91)
            {
                if (HoldSec < 0) { _p.Hold = _s.Diorama != null ? MenuHold91 : CorridorHold91; _p.Push = .015f * _p.Hold / .8f; }
                if (_s.Diorama == null) _p.Approach = CorridorSeconds92;
            }
            _rate = _p.Hold > .01f ? Mathf.Log(1 + _p.Push) / _p.Hold : 0;
            if (_s.Diorama != null)
            {
                float ch2 = Mathf.Max(screen.y, screen.x / Mathf.Max(.1f, _s.BoardAspect)), cw2 = ch2 * _s.BoardAspect;
                _c = new Rect(screen.x / 2 - cw2 / 2, screen.y / 2 - ch2 / 2, cw2, ch2);
                // the wall and the floor; over the wall the painting in its canvas; in front the column, its banner and
                // its sconce
                _dio = new Diorama78(_s.Diorama, screen, _s.ImageCentre, _s.ImageK);
                if (!Legacy)
                {
                    // PM_80: the flight by length, a soft start over its first third, then even; the approach ends
                    // straight in front of the painting, its canvas the board's framing
                    _flight = _p.Flight; _ramp = _flight / 3; _v = _dio.Length / (_flight - _ramp / 2);
                    _zEnd = _dio.DistanceFor(_c.height);
                    // PM_86 §10: the flight and the approach as one smootherstep over BellSeconds; the flight's share is
                    // where the zoom's pace (d ln z per share) at the path's end meets the approach's even one
                    _bellT = BellSeconds / Mathf.Max(.25f, DiveSpeed);
                    float a = Mathf.Abs(_dio.EndTangent.y) * _dio.Length / Mathf.Max(1e-3f, _dio.EndPoint.y);
                    float dz = Mathf.Abs(Mathf.Log(_zEnd) - Mathf.Log(_dio.EndPoint.y));
                    _bellSplit = Mathf.Clamp(a / Mathf.Max(1e-4f, a + dz), .45f, .85f);
                    _flight = _bellT * _bellSplit; _p.Approach = _bellT - _flight;
                    for (int i = 0; i <= 10; i++) _dio.Extra.Add(ApproachAt(i / 10f));
                    var eq = _dio.CanvasAt(new Vector2(0, _zEnd), 1);
                    var zr = Rect.MinMaxRect(Mathf.Min(eq[0].x, eq[3].x), Mathf.Min(eq[0].y, eq[1].y), Mathf.Max(eq[1].x, eq[2].x), Mathf.Max(eq[2].y, eq[3].y));
                    float bw = zr.height * _s.BoardAspect;
                    _endFix = new Rect(_c.position - new Vector2(zr.center.x - bw / 2, zr.yMin), _c.size - new Vector2(bw, zr.height));
                    Note($"end framing off by {_endFix.position} {_endFix.size}");
                }
                _dio.Build(_overlay, baseMat, _s.Order + 1, _dioLayers);
                if (!Legacy)
                {
                    // PM_86: the split where the canvas's speed on the screen carries over from the flight to the approach
                    // (per share of each), so the bell has no step in the middle
                    float L = _dio.Length, dl = L * .01f;
                    float vf = Move(_dio.CanvasAt(_dio.AtLength(L - dl), 1), _dio.CanvasAt(_dio.AtLength(L), 1)) / .01f;
                    float va = Move(_dio.CanvasAt(ApproachEven(0), 1), _dio.CanvasAt(ApproachEven(.01f), 1)) / .01f;
                    if (vf > 1e-3f && va > 1e-3f) _bellSplit = Mathf.Clamp(vf / (vf + va), .3f, .85f);
                    _flight = _bellT * _bellSplit; _p.Approach = _bellT - _flight;
                    _lensT = _flight + .375f * _p.Approach;
                    Note($"bell split {_bellSplit:0.00}");
                    if (_new91)
                    {
                        _plan108 = Dive108; _plan109 = Dive108 && Dive109;
                        float T = (_plan109 ? Seconds109() : _plan108 ? MenuSeconds108 : MenuSeconds92) / Mathf.Max(.25f, DiveSpeed);
                        if (_plan108) _flight = T * _bellSplit;   // (the clock's split only; the camera follows Way92)
                        _p.Approach = T - _flight;
                        Plan92(MenuQuad92, T, true);
                    }
                    _morphFrom = MorphGate(screen);
                    // PM_87 §2: the opening's first view against the menu's own (the first frame must be the menu's)
                    var first = _dio.CanvasAt(_dio.AtLength(0), 0); float startOff = 0;
                    for (int i = 0; i < 4; i++) startOff = Mathf.Max(startOff, (first[i] - _s.Quad[i]).magnitude);
                    Note($"start opening off the menu's by {startOff:0.00} pt");
                }
                _paint = Layer.Grid(_overlay, _s.Picture, baseMat, 12, _s.Order + 2);
                if (!Legacy) { _ghost = Layer.Grid(_overlay, _s.Picture, baseMat, 12, _s.Order + 3); _ghost.SetActive(false); }
                _dio.BuildFronts(_overlay, baseMat, _s.Order + 4, _dioLayers);
                Debug.Log($"[yay] start {_dio.StartAngle:0.0} deg {_dio.Describe(0)}, end {_dio.Describe(1)}, Zeyd's path x{_dio.Scale:0.000} W per metre, {_dio.Length:0.00} W long; approach to {_zEnd:0.00} W");
                var front = _dio.CanvasAt(1);
                _r = Rect.MinMaxRect(Mathf.Min(front[0].x, front[3].x), Mathf.Min(front[0].y, front[1].y), Mathf.Max(front[1].x, front[2].x), Mathf.Max(front[2].y, front[3].y));
                float ta = _s.Picture != null ? (float)_s.Picture.width / _s.Picture.height : 1, ca = _r.width / _r.height;
                _cropFront = ta > ca ? new Rect((1 - ca / ta) / 2, 0, ca / ta, 1) : new Rect(0, (1 - ta / ca) / 2, 1, ta / ca);
                if (!Legacy)
                {
                    var endQ = _dio.CanvasAt(new Vector2(0, _zEnd), 1);
                    _endAt = (x, y) => CorridorTransition.Project(endQ, x, y);
                    _gluedUv = MenuUv;
                    FitBoard(screen);
                }
                return;
            }
            // PM_87 §2.6: the corridor's own 8 x 8 grid (CorridorTransition.Mesh): the same triangles on the first frame, so
            // the painting does not shimmer as the dive takes it over (a 12 x 12 grid interpolates it a hair differently)
            _paint = Layer.Grid(_overlay, _s.Picture, baseMat, 8, _s.Order + 2);
            if (!Legacy) { _ghost = Layer.Grid(_overlay, _s.Picture, baseMat, 8, _s.Order + 3); _ghost.SetActive(false); }
            // 1: upright, centred, the opening's near side tall, the crop's proportions
            var q = _s.Quad;
            float h = Mathf.Max((q[3] - q[0]).magnitude, (q[2] - q[1]).magnitude) * 1.04f;
            float aspect = _s.Crop.width * (_s.Picture != null ? _s.Picture.width : 1) / Mathf.Max(1e-3f, _s.Crop.height * (_s.Picture != null ? _s.Picture.height : 1));
            var mid = (q[0] + q[1] + q[2] + q[3]) / 4;
            _r = new Rect(screen.x / 2 - h * aspect / 2, mid.y - h / 2, h * aspect, h);
            // 2: the board's framing, covering the screen
            float ch = Mathf.Max(screen.y, screen.x / Mathf.Max(.1f, _s.BoardAspect)), cw = ch * _s.BoardAspect;
            _c = new Rect(screen.x / 2 - cw / 2, screen.y / 2 - ch / 2, cw, ch);
            _faceH = Homography(q, Corners(_r));
            if (!Legacy)
            {
                var wallEnd = Mul(Similarity(_r.center, _c.center, _c.height / _r.height), Homography(q, Corners(_r)));
                var crop = _s.Crop;
                _endAt = (x, y) => Apply(wallEnd, CorridorTransition.Project(q, x, y));
                _gluedUv = (x, y) => new Vector2(crop.x + x * crop.width, crop.y + (1 - y) * crop.height);
                FitBoard(screen);
            }
        }

        /* The menu's own drawing of the picture (Screens.Menu.SetPainting): its crop laid flat over the opening's
           bounding box; the uv at the opening's point (x, y). */
        Vector2 MenuUv(float x, float y)
        {
            var q = _s.Quad; var c0 = _s.Crop;
            var box = Rect.MinMaxRect(Mathf.Min(q[0].x, q[3].x), Mathf.Min(q[0].y, q[1].y), Mathf.Max(q[1].x, q[2].x), Mathf.Max(q[2].y, q[3].y));
            var pt = CorridorTransition.Project(q, x, y);   // PM_90: not clamped (the board carries the picture on past the opening)
            var n = new Vector2((pt.x - box.xMin) / Mathf.Max(1e-4f, box.width), (pt.y - box.yMin) / Mathf.Max(1e-4f, box.height));
            return new Vector2(c0.x + n.x * c0.width, c0.y + (1 - n.y) * c0.height);
        }

        /* PM_90 §3: the board under the picture as it arrives: its height the opening's (the cover framing: the board
           covers the screen), centred on it. _arriveH: the picture the board must show there (board uv, v up -> picture
           uv: the frame's own drawing of it, carried on past the opening's edges); _boardH: the board's own crop
           (LevelImages.Crop). Over the pull back the board eases from the one to the other (PullBack90). */
        void FitBoard(Vector2 screen)
        {
            float ba = Mathf.Max(.1f, _s.BoardAspect);
            var e = new[] { _endAt(0, 0), _endAt(1, 0), _endAt(1, 1), _endAt(0, 1) };
            var zr = Rect.MinMaxRect(Mathf.Min(e[0].x, e[3].x), Mathf.Min(e[0].y, e[1].y), Mathf.Max(e[1].x, e[2].x), Mathf.Max(e[2].y, e[3].y));
            float hgt = Mathf.Max(zr.height, screen.y, screen.x / ba), a = hgt * ba;
            float bx = Mathf.Clamp(zr.center.x - a / 2, screen.x - a, 0), by = Mathf.Clamp(zr.center.y - hgt / 2, screen.y - hgt, 0);
            _fitA = a; _fitX = bx; _fitY = by; _fit = true;
            var toOpening = Inverse(Homography(new[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) }, e));
            Vector2 UvOf(Vector2 pt) { var xy = Apply(toOpening, pt); return _gluedUv(xy.x, xy.y); }
            var uv = new[] { UvOf(new Vector2(bx, by)), UvOf(new Vector2(bx + a, by)), UvOf(new Vector2(bx + a, by + hgt)), UvOf(new Vector2(bx, by + hgt)) };
            _arriveH = Homography(new[] { new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0) }, uv);
            for (int i = 0; i < 9; i++) _arriveH[i] /= _arriveH[8];
            var cr = LevelImages.Crop(_s.Picture, ba);
            _boardH = new double[] { cr.width, 0, cr.x, 0, cr.height, cr.y, 0, 0, 1 };
            // how far apart the two are: the arriving picture's uv over the board, and its scale across over down
            // against the board's own (1: the same proportions)
            float sx = (uv[1] - uv[0]).magnitude, sy = (uv[0] - uv[3]).magnitude;
            float own = cr.height / Mathf.Max(1e-5f, cr.width);
            Note($"board under the arriving picture: x{a / Mathf.Max(1, CoverA(screen, ba)):0.000} of the cover framing, arriving u {Mathf.Min(uv[0].x, uv[3].x):0.000}..{Mathf.Max(uv[1].x, uv[2].x):0.000} v {Mathf.Min(uv[2].y, uv[3].y):0.000}..{Mathf.Max(uv[0].y, uv[1].y):0.000}, across/down x{sy / Mathf.Max(1e-5f, sx) / own:0.000} of the board's");
        }

        double[] _arriveH, _boardH;

        /* PM_90 §3: 0..1 over the pull back at an even pace, easing in and out over its first and last 15 % (the most
           change in any one frame 1.18 x the average; a smoothstep's is 1.5 x). */
        static float Even90(float u)
        {
            const float a = .15f, vmax = 1 / (1 - a);
            u = Mathf.Clamp01(u);
            static float I(float x) => x * x * x - x * x * x * x / 2;   // the integral of smoothstep from 0 to x
            if (u < a) return vmax * a * I(u / a);
            if (u > 1 - a) return 1 - vmax * a * I((1 - u) / a);
            return vmax * (a / 2 + u - a);
        }

        /* PM_90 §3: the board's picture w of the way (0: as it arrived, 1: the board's own); the pieces and the picture
           under their hairlines alike. */
        void PullBack90(float w)
        {
            if (_board == null || _arriveH == null || Legacy) return;
            // the picture's proportions (across over down) change at an even rate (in ln) with w, not faster at one end
            double ax = _arriveH[0], ay = _arriveH[4], bx = _boardH[0], by = _boardH[4];
            double wa = w;
            if (w > 0 && w < 1 && ax > 0 && ay > 0 && bx > 0 && by > 0 && Math.Abs(Math.Log(bx / by) - Math.Log(ax / ay)) > 1e-4)
            {
                double target = Math.Exp(Math.Log(ax / ay) + (Math.Log(bx / by) - Math.Log(ax / ay)) * w);
                double den = (bx - ax) - target * (by - ay);
                if (Math.Abs(den) > 1e-9) wa = Math.Max(0, Math.Min(1, (target * ay - ax) / den));
            }
            var h = new double[9];
            for (int i = 0; i < 9; i++) h[i] = _arriveH[i] + (_boardH[i] - _arriveH[i]) * wa;
            var l = _board.Session.Level;
            for (int i = 0; i < l.Pieces.Count; i++) _board.PieceViewOf(i)?.SetImageWarp(w >= 1 ? null : h);
            _reveal?.Warp(h);
            if (Log) { var c = Apply(h, new Vector2(.5f, .5f)); Debug.Log($"[geri] {_clock:0.0000} w {w:0.0000} across {h[0]:0.00000} down {h[4]:0.00000} at {c.x:0.0000} {c.y:0.0000}"); }
        }

        static float CoverA(Vector2 screen, float ba) => Mathf.Max(screen.y, screen.x / ba) * ba;

        /* The board's uv at a screen point, once fitted. */
        Vector2 BoardUv(Vector2 pt) => new Vector2((pt.x - _fitX) / _fitA, 1 - (pt.y - _fitY) * Mathf.Max(.1f, _s.BoardAspect) / _fitA);

        /* PM_88 §1: the picture at the opening's point (x, y), m of the way (0: glued to the frame as it was drawn,
           1: the fitted board's). */
        Vector2 UvAt(float x, float y, float m)
        {
            var g = _gluedUv(x, y);
            if (m <= 0 || !_fit) return g;
            var b = BoardUv(_endAt(x, y));
            var uv = Vector2.LerpUnclamped(g, b, m);
            return new Vector2(Mathf.Clamp01(uv.x), Mathf.Clamp01(uv.y));
        }

        /* The camera on the fitted board (where the board's picture lies as fitted). */
        void FittedView(out Vector3 pos, out float size)
        {
            var o = _board.Origin; var z = _board.Size;
            float s = _fitA / Mathf.Max(1e-3f, z.x);
            pos = new Vector3(o.x - (_fitX - _s.Screen.x / 2) / s, (_fitY - _s.Screen.y / 2) / s - o.y, _homePos.z);
            size = _s.Screen.y / (2 * s);
        }

        /* PM_88 §1: the opening on the screen now (cq), the picture in it m of the way to the board's. */
        void PaintMorph(Vector2[] cq, Func<float, float, Vector2> at, float m)
        {
            if (_ghost != null && _ghostOn && _lastPq != null)
            {
                float move = 0;
                for (int c = 0; c < 4; c++) move += (cq[c] - _lastPq[c]).magnitude / 4;
                move *= 1 / 60f / Mathf.Max(1 / 240f, Time.unscaledDeltaTime);
                float a = .2f * Mathf.Clamp01((move - 5) / 10);
                _paint.CopyTo(_ghost, a);
                _ghost.SetActive(a > .005f);
            }
            if (_lastPq == null) _lastPq = new Vector2[4];
            for (int c = 0; c < 4; c++) _lastPq[c] = cq[c];
            _ghostOn = true;
            _paint.Positions(at, (x, y) => UvAt(x, y, m), 1);
            if (Log) LogPicture(at, m, cq);
        }

        /* &dalislog=1: [varis] t what scale angle u v: the picture at the screen's middle (screen points per picture
           pixel, its turn in degrees, the uv there), drawn by the overlay or by the board through the camera. */
        void LogPicture(Func<float, float, Vector2> at, float m, Vector2[] cq)
        {
            var mid = _s.Screen / 2;
            var p = Apply(Inverse(SquareToQuad(cq)), mid);
            const float d = .01f;
            Vector2 U(float x, float y) => UvAt(x, y, m);
            var px = (at(p.x + d, p.y) - at(p.x - d, p.y)) / (2 * d); var py = (at(p.x, p.y + d) - at(p.x, p.y - d)) / (2 * d);
            var ux = (U(p.x + d, p.y) - U(p.x - d, p.y)) / (2 * d); var uy = (U(p.x, p.y + d) - U(p.x, p.y - d)) / (2 * d);
            float det = ux.x * uy.y - uy.x * ux.y;
            if (Mathf.Abs(det) < 1e-9f) return;
            // screen per uv: [px py] * inverse([ux uy])
            float i00 = uy.y / det, i01 = -uy.x / det, i10 = -ux.y / det, i11 = ux.x / det;
            float j00 = px.x * i00 + py.x * i10, j01 = px.x * i01 + py.x * i11, j10 = px.y * i00 + py.y * i10, j11 = px.y * i01 + py.y * i11;
            LogVaris("overlay", j00, j01, j10, j11, U(p.x, p.y));
        }

        void LogVaris(string what, float j00, float j01, float j10, float j11, Vector2 uv)
        {
            float tw = _s.Picture != null ? _s.Picture.width : 1, th = _s.Picture != null ? _s.Picture.height : 1;
            // per picture pixel, v flipped to the screen's y down
            float a = j00 / tw, b = -j01 / th, c = j10 / tw, e = -j11 / th;
            float scale = Mathf.Sqrt(Mathf.Abs(a * e - b * c)), angle = Mathf.Atan2(c - b, a + e) * Mathf.Rad2Deg;
            Debug.Log($"[varis] {_clock:0.0000} {what} {scale:0.00000} {angle:0.000} {uv.x:0.0000} {uv.y:0.0000}");
        }

        void LogBoardPicture()
        {
            if (!Log || _board == null) return;
            var cam = _s.Cam; float s = _s.Screen.y / (2 * cam.orthographicSize); var c = cam.transform.position;
            var o = _board.Origin; var z = _board.Size;
            // the uv at the screen's middle (layout x = c.x, y = -c.y there)
            float u = (c.x - o.x) / z.x, v = 1 - (-c.y - o.y) / z.y;
            LogVaris("board", z.x * s, 0, 0, -z.y * s, new Vector2(u, v));
        }

        /* The sprites' own material (a shader found by name may be stripped from the build). */
        static Material SpriteMaterial()
        {
            var any = FindAnyObjectByType<SpriteRenderer>();
            return any != null ? any.sharedMaterial : new Material(Shader.Find("Sprites/Default"));
        }

        static Vector2[] Corners(Rect r) => new[] { new Vector2(r.xMin, r.yMin), new Vector2(r.xMax, r.yMin), new Vector2(r.xMax, r.yMax), new Vector2(r.xMin, r.yMax) };

        int _lastFrame = -1, _frames, _slow;
        float _maxDt;
        void LateUpdate()
        {
            if (_finished || _s.Cam == null || Time.frameCount == _lastFrame) return;
            _lastFrame = Time.frameCount;
            _frames++; if (Time.unscaledDeltaTime > .02f) { _slow++; if (Log) Note($"long frame {Time.unscaledDeltaTime * 1000:0} ms at {_clock:0.00} s"); } _maxDt = Mathf.Max(_maxDt, Time.unscaledDeltaTime);   // PM_84
            // a long frame (the level being built) must not eat the hold: at most 1/30 s a frame
            // PM_91: the level is built on the approach's last frame, at speed: the next frame steps on at most 1/60 s
            // (a beat on the touch, not a jump)
            _step = Mathf.Min(Time.unscaledDeltaTime, _calm ? 1 / 60f : 1 / 30f) * _speed * Mathf.Max(.1f, SpeedK);
            _calm = false;
            _clock += _step;
            Step();
        }

        void Step()
        {
            if (_finished || _s.Cam == null) return;
            if (Legacy) StepLegacy(); else StepFlow();
        }

        /* &gecis=eski: PM_77's steps (approach 1.1, breath .15, hold .5, breath .15, back 1.0, HUD .4 after a .1 s
           breath), the corridor's way (no flight). */
        void StepLegacy()
        {
            float t = _clock;
            PlaceOverlay();
            if (!_raised && t >= .22f) Raise();
            const float approach = 1.1f, breath = .15f, hold = .5f, back = 1.0f, hud = .4f;
            float b0 = approach + breath, c0 = b0 + hold + breath, c1 = c0 + back, d0 = c0 + back * 1.1f, d1 = d0 + hud;
            if (t < c0)
            {
                _s.FadeUi?.Invoke(1 - Mathf.Clamp01(t / .22f));
                FacePose(BoosterFx74.InOutCubic(Mathf.Clamp01(t / approach)));
                if (t >= b0 && !_built) BuildLevel();   // the level is built in the hold
                return;
            }
            if (!_built) BuildLevel();
            if (!_revealed) StartReveal();
            BackLegacy(Mathf.Clamp01((t - c0) / back));
            _s.Slide?.Invoke(t < d0 ? 0 : BoosterFx74.OutCubic(Mathf.Clamp01((t - d0) / hud)));
            if (t >= Mathf.Max(c1, d1)) Finish();
        }

        /* PM_80: the one timeline (see the top). */
        void StepFlow()
        {
            float t = _clock;
            PlaceOverlay();
            if (!_raised && t >= (_dio != null && !_s.Seamless ? CrossIn : .22f)) Raise();
            float h0 = _flight + _p.Approach, h1 = h0 + _p.Hold, h2 = h1 + _p.Back;
            _s.FadeUi?.Invoke(1 - SmoothStep(t / .3f));   // the menu's controls, over the first .3 s
            Cues100(t, h0, h1, h2);
            // PM_107 §C6: the air follows the motion: the flight and the approach are one smootherstep over [0, h0]
            // (PM_86), its speed 30 u^2 (1-u)^2, 1 at the middle
            // PM_108: the menu's dive its own speed (off at once, the peak, then even), the air the same
            Gecis100.Dive(t >= h0 ? 0 : _speed108 != null ? Interp(_speed108, t / Mathf.Max(.05f, h0)) : Bell107(t / Mathf.Max(.05f, h0)));
            if (t < h0)
            {
                float u = Mathf.Clamp01((t - _flight) / _p.Approach);
                if (_dio != null)
                {
                    FlowDiorama(t, _s.Seamless ? 1 : Mathf.Clamp01(t / CrossIn));
                    if (!_hidden && !_s.Seamless && t >= CrossIn) { _hidden = true; _s.Hide?.Invoke(); }
                }
                else FacePose(ApproachE(u), 1, 0);   // PM_90 §3: glued, no morph (see FlowDiorama)
                // the level is built as the approach comes to rest (the screen barely moves: a long frame is not seen);
                // the cuts take over on the next frame
                if (!_built && (_new91 ? t + _step >= h0 : u >= .94f)) BuildLevel();
                LogOverlay(t);
                return;
            }
            if (!_built) BuildLevel();
            if (!_revealed) StartReveal();
            if (_board != null)
            {
                // the hold: a slow push in; the back: ease in-out to the level's own view, taking over the push
                var rect = new Rect(_board.Origin, _board.Size);
                var from = new Vector3(rect.center.x, -rect.center.y, _homePos.z);
                float lnCover = Mathf.Log(Dive74.CoverSize(rect.size, _s.Screen)), lnPush = Push91(_p.Hold);
                if (_fit) { FittedView(out from, out float fs); lnCover = Mathf.Log(fs); }   // PM_88 §1: from where the picture arrived
                if (t < h1) { _s.Cam.orthographicSize = Mathf.Exp(lnCover - Push91(t - h0)); _s.Cam.transform.position = from; }
                else
                {
                    float u = Mathf.Clamp01((t - h1) / Mathf.Max(.01f, _p.Back));
                    float l0 = lnCover - lnPush, l1 = Mathf.Log(_homeSize);
                    float l = Hermite3(l0, -Push91Rate(_p.Hold) * _p.Back, l1, 0, u);
                    _s.Cam.orthographicSize = Mathf.Exp(l);
                    _s.Cam.transform.position = Vector3.LerpUnclamped(from, _homePos, Mathf.Abs(l1 - l0) > 1e-5f ? (l - l0) / (l1 - l0) : u);
                }
            }
            // PM_90 §3: the arriving picture held through the hold, then eased to the board's own over the whole pull back
            // (the one difference spread out; the camera alone scales it, the same across and down; the cuts are the board's)
            PullBack90(Even90((t - h1) / Mathf.Max(.01f, _p.Back)));
            _reveal?.Flow(LineFrac(t, h0, h1), Mathf.Clamp01((t - h1) / Mathf.Max(.01f, _p.Back)));
            LogBoardPicture();
            float hud = Mathf.Min(.3f, _p.Back);
            _s.Slide?.Invoke(BoosterFx74.OutCubic(Mathf.Clamp01((t - (h2 - hud)) / hud)));
            LogBoard(t);
            if (t >= h2) Finish();
        }

        /* PM_100 §4: the dive's sounds on its own clock: the rising whoosh so that it ends as the picture fills the screen
           (DiveLead before it, at the dive's present pace), the ting and the cuts' paper there, the falling whoosh as it
           pulls back, the HUD's tick as it settles. Each once. */
        bool _cueIn, _cueTouch, _cueBack, _cueHud;
        void Cues100(float t, float h0, float h1, float h2)
        {
            float rate = Mathf.Max(.01f, _speed * Mathf.Max(.1f, SpeedK));   // the clock's s per real s
            if (!_cueIn && t < h0 && (h0 - t) / rate <= Gecis100.DiveLead)
            {
                // PM_102 §5: the whoosh ends on the touch: after the corridor the approach is shorter than it, so it joins
                // part way (its end still on the touch)
                float left = (h0 - t) / rate;
                _cueIn = true; Gecis100.Event("dalış (değmeye " + left.ToString("0.00") + " s)");
                Gecis100.Play(Gecis100.Kind.Dalis, Gecis100.DiveWhooshVolume, Gecis100.DiveLead - left);
            }
            if (!_cueTouch && t >= h0) { _cueTouch = _cueIn = true; Gecis100.Event("değme"); Gecis100.Play(Gecis100.Kind.Cizgi); }   // PM_106 §1: no ting at the touch (Zeyd: the "klink" goes; the whoosh stays)
            if (!_cueBack && t >= h1) { _cueBack = true; Gecis100.Event("geri çekilme"); Gecis100.Play(Gecis100.Kind.Geri); }
            if (!_cueHud && t >= h2) { _cueHud = true; Gecis100.Event("HUD oturdu"); Gecis100.Play(Gecis100.Kind.Hud); }
        }

        /* The hold's push in (ln of the camera's size, from the hand-over): the slow push, and with PM_91 the touch:
           the camera runs on into the picture at the approach's arrival speed, that speed falling off at once (over
           _touchTau), by at most TouchMax in all; then the pull back takes it from there. */
        float Push91(float x) => _rate * x + (_touchTau > 0 ? _touch * (1 - Mathf.Exp(-Mathf.Max(0, x) / _touchTau)) : 0);
        float Push91Rate(float x) => _rate + (_touchTau > 0 ? _touch / _touchTau * Mathf.Exp(-Mathf.Max(0, x) / _touchTau) : 0);

        /* PM_91: the picture's growth on the screen (ln of its size per s) as the approach arrives. */
        float ArriveRate91()
        {
            if (_wayD == null) return 0;
            const float dt = .02f;
            float a = Way92(_T92 - dt), b = Way92(_T92);
            return (LnHeight(_quad92(b)) - LnHeight(_quad92(a))) / dt;
        }
        static float LnHeight(Vector2[] q) => Mathf.Log(Mathf.Max(1e-3f, ((q[3] - q[0]).magnitude + (q[2] - q[1]).magnitude) / 2));

        float[] _wayD, _timeD;            // PM_92: the screen distance covered at each step of the way / of the time
        float _T92;
        System.Func<float, Vector2[]> _quad92;
        const int WaySteps92 = 240, TimeSteps92 = 600;

        /* PM_92: the wall's motion on the screen from the opening a to b (the same plane: one homography), the mean over
           a grid on the whole screen, in screen heights. */
        float Flow92(Vector2[] a, Vector2[] b)
        {
            var h = Homography(a, b); var sc = _s.Screen; float sum = 0; int n = 0;
            for (int i = 0; i <= 8; i++)
                for (int j = 0; j <= 18; j++)
                {
                    var p = new Vector2(sc.x * i / 8f, sc.y * j / 18f);
                    sum += (Apply(h, p) - p).magnitude; n++;
                }
            return sum / n / Mathf.Max(1, sc.y);
        }

        /* PM_92: the way (0..1) laid out by its screen distance, and the time by the speed (see MenuSeconds92); the
           start's speed and acceleration the old curve's (the menu: Launch over its first .2 s; the corridor: from
           rest, as its Hermite). */
        void Plan92(System.Func<float, Vector2[]> quad, float T, bool menu)
        {
            _quad92 = quad; _T92 = T;
            _wayD = new float[WaySteps92 + 1];
            var prev = quad(0);
            for (int i = 1; i <= WaySteps92; i++) { var q = quad(i / (float)WaySteps92); _wayD[i] = _wayD[i - 1] + Flow92(prev, q); prev = q; }
            float D = _wayD[WaySteps92], v0 = 0, a0 = 0;
            bool p108 = menu && _plan108;
            if (menu && _bellT > 0 && !p108)
            {
                float At(float t) => Interp(_wayD, Launch(t / _bellT));
                v0 = (At(.02f) - At(0)) / .02f;
                float v2 = (At(.21f) - At(.19f)) / .02f;
                a0 = Mathf.Max(0, (v2 - v0) / .2f);
            }
            float[] Speeds(float lam, float sv0, float sa0)
            {
                float riseFrom = menu ? RiseFrom92 : CorridorRiseFrom92, rise = menu ? Rise92 : CorridorRise92;
                float t1 = riseFrom * T, t2 = Peak92 * T, m = (rise - 1) / Mathf.Max(1e-3f, t2 - t1);
                var v = new float[TimeSteps92 + 1];
                if (p108) { for (int k = 0; k <= TimeSteps92; k++) v[k] = lam * MenuSpeed(k / (float)TimeSteps92, T); return v; }
                for (int k = 0; k <= TimeSteps92; k++)
                {
                    float t = T * k / TimeSteps92;
                    v[k] = t < t1 ? Hermite3(sv0, sa0 * t1, lam, lam * m * t1, t / t1)
                         : t < t2 ? Hermite3(lam, lam * m * (t2 - t1), lam * rise, 0, (t - t1) / (t2 - t1))
                         : Hermite3(lam * rise, 0, lam * rise * Land92, 0, (t - t2) / Mathf.Max(1e-4f, T - t2));
                }
                return v;
            }
            float Area(float[] v) { float a = 0; for (int k = 1; k < v.Length; k++) a += (v[k] + v[k - 1]) / 2 * T / TimeSteps92; return a; }
            float s0 = Area(Speeds(0, v0, a0)), s1 = Area(Speeds(1, 0, 0));
            float lambda = Mathf.Max(1e-4f, (D - s0) / Mathf.Max(1e-6f, s1));
            var vs = Speeds(lambda, v0, a0);
            if (p108) { _speed108 = new float[TimeSteps92 + 1]; for (int k = 0; k <= TimeSteps92; k++) _speed108[k] = MenuSpeed(k / (float)TimeSteps92, T); }
            _timeD = new float[TimeSteps92 + 1];
            for (int k = 1; k <= TimeSteps92; k++) _timeD[k] = _timeD[k - 1] + (vs[k] + vs[k - 1]) / 2 * T / TimeSteps92;
            float scale = D / Mathf.Max(1e-6f, _timeD[TimeSteps92]);
            for (int k = 0; k <= TimeSteps92; k++) _timeD[k] *= scale;
            Note($"PM_92: {D:0.000} screen heights in {T:0.00} s, from {v0:0.000} (+{a0:0.00}/s) through {lambda:0.000}");
        }

        /* PM_108: the menu dive's speed at the share x of its time T, 1 at the peak (see Dive108). */
        public static float Speed108(float x, float T)
        {
            float even = 1 / Mathf.Max(1.01f, Peak108), te = T > 0 ? Mathf.Clamp01(1 - EndEase108 / T) : 1;
            if (x < Rise108) return Hermite3(Start108, 1.5f * (1 - Start108), 1, 0, x / Rise108);
            if (x < Hold108) return 1;
            if (x < Fall108) return Hermite3(1, 0, even, 0, (x - Hold108) / (Fall108 - Hold108));
            if (x < te) return even;
            return Hermite3(even, 0, even * EndLand108, 0, Mathf.Clamp01((x - te) / Mathf.Max(1e-4f, 1 - te)));
        }

        float MenuSpeed(float x, float T) => _plan109 ? Speed109(x, T) : Speed108(x, T);

        /* PM_109: the menu dive's speed at the share x of its time T, 1 at the peak (see Dive109). The climb meets the
           plateau's slope, so the acceleration has no step there. */
        public static float Speed109(float x, float T)
        {
            float even = 1 / Mathf.Max(1.01f, Peak108), te = T > 0 ? Mathf.Clamp01(1 - EndEase108 / T) : 1;
            const float lean = .2f;   // the plateau's slope at its start (over its own span), easing to 0 at the top
            if (x < Rise109) return Hermite3(Start109, 1.5f * (Climb109 - Start109), Climb109, lean * Rise109 / (Top109 - Rise109), x / Rise109);
            if (x < Top109) return Hermite3(Climb109, lean, 1, 0, (x - Rise109) / (Top109 - Rise109));
            if (x < Hold109) return 1;
            if (x < Fall109) return Hermite3(1, 0, even, 0, (x - Hold109) / (Fall109 - Hold109));
            if (x < te) return even;
            return Hermite3(even, 0, even * EndLand108, 0, Mathf.Clamp01((x - te) / Mathf.Max(1e-4f, 1 - te)));
        }

        /* PM_109: the time at which its shape keeps PM_108's even speed over the same way (PM_108's area over its own,
           at DiveSpeed 1), within .9..1.1 s. */
        public static float Seconds109()
        {
            static float Area(System.Func<float, float> f) { float a = 0; for (int k = 1; k <= 600; k++) a += (f(k / 600f) + f((k - 1) / 600f)) / 2 / 600; return a; }
            float a108 = MenuSeconds108 * Area(x => Speed108(x, MenuSeconds108)), T = 1;
            for (int i = 0; i < 3; i++) { float t = T; T = a108 / Mathf.Max(1e-4f, Area(x => Speed109(x, t))); }
            return Mathf.Clamp(T, .9f, 1.1f);
        }

        /* PM_92: the way (0..1) reached at t. */
        float Way92(float t)
        {
            float d = Interp(_timeD, Mathf.Clamp01(t / Mathf.Max(1e-3f, _T92)));
            int lo = 0, hi = WaySteps92;
            if (d <= 0) return 0;
            if (d >= _wayD[hi]) return 1;
            while (hi - lo > 1) { int m = (lo + hi) / 2; if (_wayD[m] <= d) lo = m; else hi = m; }
            float f = (d - _wayD[lo]) / Mathf.Max(1e-9f, _wayD[hi] - _wayD[lo]);
            return (lo + f) / WaySteps92;
        }
        static float Interp(float[] a, float x)
        {
            float f = Mathf.Clamp01(x) * (a.Length - 1); int i = Mathf.Min(a.Length - 2, (int)f);
            return Mathf.Lerp(a[i], a[i + 1], f - i);
        }

        /* PM_92: the frame's opening on the screen at the way s, through the menu's diorama (as FlowDiorama poses it). */
        Vector2[] MenuQuad92(float s)
        {
            var p = s <= _bellSplit ? _dio.AtLength(_dio.Length * s / _bellSplit) : ApproachEven((s - _bellSplit) / (1 - _bellSplit));
            float w = SmoothStep(_bellT * LaunchInverse(s) / Mathf.Max(.01f, _lensT));
            return _dio.CanvasAt(p, w);
        }

        /* PM_92: the opening after the corridor at the approach's e (as FacePose poses it). */
        Vector2[] FaceQuad92(float e)
        {
            var q = _s.Quad; var r = Corners(_r);
            var rect = LogLerp(_r, _c, e);
            var zoom = Similarity(_r.center, rect.center, rect.height / _r.height);
            var zq = new Vector2[4];
            for (int i = 0; i < 4; i++) zq[i] = Apply(zoom, Vector2.Lerp(q[i], r[i], e));
            return zq;
        }
        /* The old curve's time (share of it) at the progress s: the lens shift keeps to the way, not the clock. */
        static float LaunchInverse(float s)
        {
            float lo = 0, hi = 1;
            for (int i = 0; i < 24; i++) { float m = (lo + hi) / 2; if (Launch(m) < s) lo = m; else hi = m; }
            return (lo + hi) / 2;
        }

        /* How much of the cuts is drawn: 0 at the hold's start, Lines at its end, all at 80 % of the back; slow at
           first (a pen), even through the hold's end. &cizgihiz= runs it faster or slower. */
        float LineFrac(float t, float h0, float h1)
        {
            float a = h0, b = h1, c = h1 + LinesDone * _p.Back, tt = a + (t - a) * Mathf.Max(.1f, LineK);
            if (tt <= a) return 0;
            if (tt >= c) return 1;
            float at = b - a > .01f ? _p.Lines : 0;
            float d1 = b - a > .01f ? at / (b - a) : 0, d2 = (1 - at) / Mathf.Max(.01f, c - b);
            float s = d1 > 0 && d2 > 0 ? 2 / (1 / d1 + 1 / d2) : 0;   // the slope at the hold's end (a harmonic mean: no overshoot)
            return tt < b ? Hermite3(0, 0, at, s * (b - a), (tt - a) / (b - a)) : Hermite3(at, s * (c - b), 1, 0, (tt - b) / (c - b));
        }

        /* The flight: the length along Zeyd's path at t, a soft start (the speed eases up over _ramp) then even. */
        float FlightLength(float t)
        {
            if (t < _ramp) { float x = t / _ramp; return _v * _ramp * (x * x * x - x * x * x * x / 2); }
            return _v * _ramp / 2 + _v * (t - _ramp);
        }

        /* The approach at u (0..1): from the path's end, with its speed and direction (W/s), to straight in front of
           the painting at _zEnd, arriving at the hold's push rate. The distance in ln z (the picture grows evenly on
           the screen): its rate rises from the flight's over the first 30 %, holds, and eases out over the last 25 %
           (each change a smoothstep: no jump in the speed or the acceleration); a plateau, not a peak, so the way in
           is an even flow. The sideways in x: a quintic Hermite (no jump in the acceleration either). */
        Vector2 ApproachAt(float u)
        {
            var p0 = _dio.EndPoint; var v0 = _dio.EndTangent * (_v * _p.Approach);
            float x = Hermite5(p0.x, v0.x, 0, 0, u);
            float lz0 = Mathf.Log(p0.y), d = Mathf.Log(_zEnd) - lz0;
            float lz = lz0 + Plateau(u, v0.y / p0.y, d, -_rate * _p.Approach, .3f, .75f);
            return new Vector2(x, Mathf.Exp(lz));
        }

        /* PM_86: the approach at an even pace in ln z (the bell's smootherstep gives the speed). */
        Vector2 ApproachEven(float u)
        {
            u = Mathf.Clamp01(u);
            var p0 = _dio.EndPoint;
            float lz = Mathf.Lerp(Mathf.Log(p0.y), Mathf.Log(_zEnd), u);
            return new Vector2(Mathf.Lerp(p0.x, 0, SmoothStep(u)), Mathf.Exp(lz));
        }

        /* PM_86 §11: the approach's progress (in ln z, 0..1) from which the frame's opening covers the whole screen. */
        float MorphGate(Vector2 screen)
        {
            float z0 = Mathf.Log(_dio.EndPoint.y), z1 = Mathf.Log(_zEnd);
            var corners = new[] { new Vector2(0, 0), new Vector2(screen.x, 0), new Vector2(screen.x, screen.y), new Vector2(0, screen.y) };
            for (int i = 0; i <= 40; i++)
            {
                var p = _bellT > 0 ? ApproachEven(i / 40f) : ApproachAt(i / 40f);
                var q = _dio.CanvasAt(p, 1);
                bool covers = true;
                foreach (var c in corners) if (!Inside(q, c)) { covers = false; break; }
                if (covers) return Mathf.Min(.9f, (z0 - Mathf.Log(p.y)) / (z0 - z1));
            }
            return .9f;
        }

        static float Move(Vector2[] a, Vector2[] b)
        {
            float m = 0;
            for (int i = 0; i < 4; i++) m += (b[i] - a[i]).magnitude / 4;
            return m;
        }

        static bool Inside(Vector2[] q, Vector2 pt)
        {
            float sign = 0;
            for (int i = 0; i < 4; i++)
            {
                var a = q[i]; var b = q[(i + 1) % 4];
                float c = (b.x - a.x) * (pt.y - a.y) - (b.y - a.y) * (pt.x - a.x);
                if (Mathf.Abs(c) < 1e-6f) continue;
                if (sign == 0) sign = Mathf.Sign(c); else if (Mathf.Sign(c) != sign) return false;
            }
            return true;
        }

        /* The distance covered by u (0..1) of a move of `total` that starts at speed g0 and ends at g1 (per unit u):
           a smoothstep up to a constant speed over 0..ua, then a smoothstep down to g1 over ub..1. */
        static float Plateau(float u, float g0, float total, float g1, float ua, float ub)
        {
            float gm = (total - g0 * ua / 2 - g1 * (1 - ub) / 2) / (ua / 2 + ub - ua + (1 - ub) / 2);
            static float I(float x) => x * x * x - x * x * x * x / 2;   // the integral of smoothstep from 0 to x
            u = Mathf.Clamp01(u);
            if (u < ua) return g0 * u + (gm - g0) * ua * I(u / ua);
            float s = g0 * ua + (gm - g0) * ua / 2;
            if (u < ub) return s + gm * (u - ua);
            s += gm * (ub - ua);
            return s + gm * (u - ub) + (g1 - gm) * (1 - ub) * I((u - ub) / (1 - ub));
        }

        /* The approach without the flight (the corridor): the painting's zoom eases in from rest, out to the push rate. */
        float ApproachE(float u)
        {
            float lnRatio = Mathf.Log(Mathf.Max(1.0001f, _c.height / Mathf.Max(1, _r.height)));
            if (_new91)
            {
                if (_wayD == null) Plan92(FaceQuad92, _p.Approach, false);
                return Way92(Mathf.Clamp01(u) * _p.Approach);
            }
            return Hermite5(0, 0, 1, _rate * _p.Approach / lnRatio, Mathf.Clamp01(u));
        }

        static readonly double[] Identity = { 1, 0, 0, 0, 1, 0, 0, 0, 1 };

        /* The flight and the approach through the diorama, one camera curve. */
        void FlowDiorama(float t, float fade)
        {
            Vector2 p; float e = 0;
            bool bell = _bellT > 0;
            float s = bell ? (_new91 && _wayD != null ? Way92(t) : Launch(t / _bellT)) : 0;   // PM_86 §10: one move over the flight and the approach (PM_87: off at once)
            if (bell ? s <= _bellSplit : t <= _flight) p = bell ? _dio.AtLength(_dio.Length * s / _bellSplit) : _dio.AtLength(FlightLength(t));
            else
            {
                p = bell ? ApproachEven((s - _bellSplit) / (1 - _bellSplit)) : ApproachAt((t - _flight) / _p.Approach);
                float z0 = Mathf.Log(_dio.EndPoint.y), z1 = Mathf.Log(_zEnd);
                float k = (z0 - Mathf.Log(p.y)) / (z0 - z1);
                // PM_86 §11: the painting stays in its frame (the opening's own projection) until the frame has left the
                // screen; only then it eases to the board's framing
                e = SmoothStep((k - _morphFrom) / Mathf.Max(.05f, 1 - _morphFrom));
            }
            // the lens shift takes the painting from the menu's place to the centre, easing out early in the approach
            float w = bell && _new91 ? SmoothStep(_bellT * LaunchInverse(s) / Mathf.Max(.01f, _lensT)) : SmoothStep(t / (_flight + .375f * _p.Approach));
            _dio.Pose(p, w, Identity, _dioLayers, fade);
            var cq = _dio.CanvasAt(p, w);
            // PM_87 §2: the picture is the menu's own drawing of it (its crop laid flat over the opening, cut to it), carried
            // by the opening point for point: on the first frame exactly the menu's, then glued to the frame as the camera
            // moves (PM_86 handed the menu's crop over to another in the first .25 s while the camera stood: the picture
            // slid in its frame). Only once the frame has left the screen does it ease to the whole picture (e).
            if (_fit)
            {
                // PM_90 §3 (Zeyd: "at the very end the picture shortens and grows for a moment"): the picture stays
                // exactly as the frame shows it through the approach and the arrival (PM_88 morphed it to the board's
                // here); the board takes it over as it is and eases it to its own over the pull back (PullBack90)
                float m = 0;
                PaintMorph(cq, (x, y) => CorridorTransition.Project(cq, x, y), m);
                e = 0;
            }
            else PaintInto(cq, e, _cropFront, 1, null, null, 1 - e, true);
            if (Log)
            {
                // the drawn picture's quad against the frame's opening while the picture is still in its frame
                float off = 0;
                if (e < 1e-4f && _lastPq != null)
                    for (int i = 0; i < 4; i++) off = Mathf.Max(off, (_lastPq[i] - cq[i]).magnitude);
                Debug.Log($"[cerceve] {t:0.000} e {e:0.000} off {off:0.00} q {cq[0].x:0.00} {cq[0].y:0.00} {cq[1].x:0.00} {cq[1].y:0.00} {cq[2].x:0.00} {cq[2].y:0.00} {cq[3].x:0.00} {cq[3].y:0.00}");
            }
            _paint.Fade(fade);
            _backdrop.Alpha(fade);
        }

        static float SmoothStep(float t) { t = Mathf.Clamp01(t); return t * t * (3 - 2 * t); }
        /* PM_87 §2: the camera's progress (0..1): off at once (its speed rises from the first frame, a soft lift, no
           standing still), the most speed a third of the way, a long soft landing (no speed and no push at the end).
           The speed is (u + .04)(1 - u)^2, normalised. */
        public static float Launch(float u)
        {
            if (u <= 0) return 0;
            if (u >= 1) return 1;
            const float c = .04f;
            float u2 = u * u, u3 = u2 * u;
            return Mathf.Min(1, (u2 / 2 - 2 * u3 / 3 + u2 * u2 / 4 + c * (u - u2 + u3 / 3)) / (1f / 12 + c / 3));   // float rounding: never past 1
        }
        static float Hermite3(float p0, float m0, float p1, float m1, float u)
        {
            float u2 = u * u, u3 = u2 * u;
            return (2 * u3 - 3 * u2 + 1) * p0 + (u3 - 2 * u2 + u) * m0 + (-2 * u3 + 3 * u2) * p1 + (u3 - u2) * m1;
        }
        /* Quintic Hermite with no second derivative at either end. */
        static float Hermite5(float p0, float m0, float p1, float m1, float u)
        {
            float u3 = u * u * u, u4 = u3 * u, u5 = u4 * u;
            return (1 - 10 * u3 + 15 * u4 - 6 * u5) * p0 + (u - 6 * u3 + 8 * u4 - 3 * u5) * m0
                 + (-4 * u3 + 7 * u4 - 3 * u5) * m1 + (10 * u3 - 15 * u4 + 6 * u5) * p1;
        }

        // ---- &dalislog=1 ----
        Vector2[] _logQuad;
        void LogOverlay(float t)
        {
            if (!Log || _logQuad == null) return;
            var q = _logQuad;
            Debug.Log($"[dl] {t:0.0000} {Time.unscaledDeltaTime * 1000:0.0} o {q[0].x:0.0} {q[0].y:0.0} {q[1].x:0.0} {q[1].y:0.0} {q[2].x:0.0} {q[2].y:0.0} {q[3].x:0.0} {q[3].y:0.0}");
        }
        void LogBoard(float t)
        {
            if (!Log || _board == null) return;
            var cam = _s.Cam; float s = _s.Screen.y / (2 * cam.orthographicSize); var c = cam.transform.position;
            Vector2 S(Vector2 l) => new Vector2(_s.Screen.x / 2 + (l.x - c.x) * s, _s.Screen.y / 2 + (l.y + c.y) * s);
            var o = _board.Origin; var z = _board.Size;
            var a = S(o); var b = S(o + z);
            Debug.Log($"[dl] {t:0.0000} {Time.unscaledDeltaTime * 1000:0.0} b {a.x:0.0} {a.y:0.0} {b.x:0.0} {a.y:0.0} {b.x:0.0} {b.y:0.0} {a.x:0.0} {b.y:0.0}");
        }

        bool _hidden;
        Vector2[] _lastPq;

        /* The approach without an orbit (the corridor, or the menu without &gecis=yay): the painting's slight
           perspective eases out while the camera moves in, all in one move. */
        void FacePose(float e, float enlarge = 1, float m = -1)
        {
            var q = _s.Quad; var r = Corners(_r);
            var to = new Vector2[4];
            for (int i = 0; i < 4; i++) to[i] = Vector2.Lerp(q[i], r[i], e);
            var rect = LogLerp(_r, _c, e);
            var zoom = Similarity(_r.center, rect.center, rect.height / _r.height);
            var wall = Mul(zoom, Homography(q, to));
            foreach (var w in _walls) w.Warp(wall, 1);   // the wall never fades: the picture covers the screen by the end
            var zq = new Vector2[4];
            for (int c = 0; c < 4; c++) zq[c] = Apply(zoom, to[c]);
            // PM_87 §2.6: the picture stays the frame's opening (the wall's own homography) until the frame's slight
            // turn has mostly eased out and the frame is at the screen's edges; only then it eases to the board's
            // framing (PM_86 eased it from the start: it slid in its frame on the way in)
            float ep = Legacy ? e : SmoothStep((e - FaceGate) / (1 - FaceGate));
            if (m >= 0 && _fit)
            {
                // PM_88 §1: glued to the frame all the way; the board is put where the picture arrives
                PaintMorph(zq, (x, y) => Apply(wall, CorridorTransition.Project(q, x, y)), m);
                ep = 0;
            }
            else PaintInto(zq, ep, _s.Crop, enlarge, ep <= 0 ? q : null, wall);
            if (Log && !Legacy)
                Debug.Log($"[cerceve] {_clock:0.000} e {ep:0.000} off 0 q {zq[0].x:0.00} {zq[0].y:0.00} {zq[1].x:0.00} {zq[1].y:0.00} {zq[2].x:0.00} {zq[2].y:0.00} {zq[3].x:0.00} {zq[3].y:0.00}");
            _backdrop.Alpha(1);
        }

        /* The painting: in the canvas quad zq (perspective-correct) and, with the approach e, the board's framing;
           its crop eases from crop0 to the whole picture. While a wall still turns (start != null) the picture
           follows the same homography as the wall. flat (PM_78): how much of the menu's own way of drawing it is
           left (Screens.Menu.SetPainting: the crop laid flat over the opening's bounding box, cut to the opening). */
        void PaintInto(Vector2[] zq, float e, Rect crop0, float enlarge, Vector2[] start, double[] wall, float flat = 0, bool exact = false)
        {
            var crop = new Rect(Vector2.Lerp(crop0.position, Vector2.zero, e), Vector2.Lerp(crop0.size, Vector2.one, e));
            var zr = Rect.MinMaxRect(Mathf.Min(zq[0].x, zq[3].x), Mathf.Min(zq[0].y, zq[1].y), Mathf.Max(zq[1].x, zq[2].x), Mathf.Max(zq[2].y, zq[3].y));
            float boardW = zr.height * Mathf.Lerp(zr.width / Mathf.Max(1, zr.height), _s.BoardAspect, e);
            float w = Mathf.Max(boardW, zr.width);
            var target = new Rect(zr.center.x - w / 2, zr.yMin, w, zr.height);
            if (exact)
            {
                // PM_80: the camera alone makes it grow (the approach ends with the canvas the board's framing); only
                // what is left between the two (a point or so) is taken up with e: exactly the board's framing at the end
                var aspect = new Rect(zr.center.x - boardW / 2, zr.yMin, boardW, zr.height);
                target = new Rect(aspect.position + _endFix.position * e, aspect.size + _endFix.size * e);
            }
            var tq = Corners(target);
            var pq = new Vector2[4];
            for (int c = 0; c < 4; c++) pq[c] = Vector2.Lerp(zq[c], tq[c], e);
            var pc = (pq[0] + pq[1] + pq[2] + pq[3]) / 4;
            for (int c = 0; c < 4; c++) pq[c] = pc + (pq[c] - pc) * enlarge;
            if (Log)
            {
                // the whole picture's corners (where the crop's edges extend to): what the eye follows, not the quad
                var cq = pq; var cr = crop;
                Vector2 At(float u, float v) => CorridorTransition.Project(cq, (u - cr.x) / cr.width, 1 - (v - cr.y) / cr.height);
                _logQuad = new[] { At(0, 1), At(1, 1), At(1, 0), At(0, 0) };
            }
            if (_ghost != null && _ghostOn)
            {
                // PM_80: a faint trace of the last frame's painting while it moves fast (the corners > 5 pt a frame at 60 fps)
                float move = 0;
                for (int c = 0; c < 4; c++) move += (pq[c] - _lastPq[c]).magnitude / 4;
                move *= 1 / 60f / Mathf.Max(1 / 240f, Time.unscaledDeltaTime);
                float a = .2f * Mathf.Clamp01((move - 5) / 10);
                _paint.CopyTo(_ghost, a);
                _ghost.SetActive(a > .005f);
            }
            if (_lastPq == null) _lastPq = new Vector2[4];
            for (int c = 0; c < 4; c++) _lastPq[c] = pq[c];
            _ghostOn = true;
            if (start != null) _paint.Positions((x, y) => Apply(wall, CorridorTransition.Project(start, x, y)), crop, 1);
            else if (flat > 0)
            {
                var q = _s.Quad; var c0 = _s.Crop;
                var box = Rect.MinMaxRect(Mathf.Min(q[0].x, q[3].x), Mathf.Min(q[0].y, q[1].y), Mathf.Max(q[1].x, q[2].x), Mathf.Max(q[2].y, q[3].y));
                _paint.Positions((x, y) => CorridorTransition.Project(pq, x, y), (x, y) =>
                {
                    var s = Rect.PointToNormalized(box, CorridorTransition.Project(q, x, y));
                    var menu = new Vector2(c0.x + s.x * c0.width, c0.y + (1 - s.y) * c0.height);
                    return Vector2.Lerp(new Vector2(crop.x + x * crop.width, crop.y + (1 - y) * crop.height), menu, flat);
                }, 1);
            }
            else _paint.Positions((x, y) => CorridorTransition.Project(pq, x, y), crop, 1);
        }

        void Raise()
        {
            _raised = true;
            int d = _s.TopOrder - _s.Order;
            if (d == 0 || _overlay == null) return;
            foreach (var r in _overlay.GetComponentsInChildren<Renderer>(true)) r.sortingOrder += d;   // the views not shown yet too
        }

        static Rect LogLerp(Rect a, Rect b, float e)
        {
            float w = Mathf.Exp(Mathf.Lerp(Mathf.Log(a.width), Mathf.Log(b.width), e));
            float h = Mathf.Exp(Mathf.Lerp(Mathf.Log(a.height), Mathf.Log(b.height), e));
            var c = Vector2.Lerp(a.center, b.center, e);
            return new Rect(c.x - w / 2, c.y - h / 2, w, h);
        }

        /* The overlay rides on the camera at the scale of the home view. */
        void PlaceOverlay()
        {
            if (_overlay == null) return;
            float k = _s.Cam.orthographicSize / _homeSize;
            _overlay.localScale = new Vector3(k, k, 1);
            _overlay.localPosition = new Vector3(-_s.Screen.x / 2 * k, _s.Screen.y / 2 * k, 10);
        }

        /* PM_80: how long the dive's own steps take (one line at its end: [dalis] ...). */
        readonly System.Text.StringBuilder _notes = new System.Text.StringBuilder();
        void Note(string what) => _notes.Append(what).Append(", ");

        void BuildLevel()
        {
            _built = true;
            var clock = System.Diagnostics.Stopwatch.StartNew();
            _calm = _new91;
            _board = _s.Build?.Invoke();
            Note($"build {clock.Elapsed.TotalMilliseconds:0} ms at {_clock:0.00} s");
            _s.Slide?.Invoke(0);
            // the level's own home view (as built), where the pull back ends
            _homePos = _s.Cam.transform.position; _homeSize = _s.Cam.orthographicSize;
            if (_board == null) return;
            var rect = new Rect(_board.Origin, _board.Size);
            _s.Cam.transform.position = new Vector3(rect.center.x, -rect.center.y, _homePos.z);
            _s.Cam.orthographicSize = Dive74.CoverSize(rect.size, _s.Screen);
            if (_fit && !Legacy) { FittedView(out var fp, out float fs); _s.Cam.transform.position = fp; _s.Cam.orthographicSize = fs; }
            PullBack90(0);   // PM_90 §3: the picture exactly as it arrives
            PlaceOverlay();   // this frame already draws with the new camera (PM_77: one frame 3x too close since PM_75)
        }

        void StartReveal()
        {
            _revealed = true;
            if (_new91 && TouchMax > 0)
            {
                float v = ArriveRate91();
                _touch = Mathf.Min(TouchMax, v * TouchSeconds); _touchTau = v > 1e-3f ? _touch / v : 0;
                Note($"arrives growing {v:0.00} ln/s, touch {_touch:0.000} over {_touchTau * 1000:0} ms, hold {_p.Hold:0.00} s");
            }
            var clock = System.Diagnostics.Stopwatch.StartNew();
            if (_board != null) _reveal = Reveal.Create(_board, _board.Image != null ? _board.Image : _s.Picture, !Legacy);
            PullBack90(0);
            if (_overlay != null) Destroy(_overlay.gameObject);
            _overlay = null;
            foreach (var w in _walls) w.Dispose();
            _walls.Clear();
            foreach (var w in _dioLayers) w.Dispose();
            _dioLayers.Clear();
            if (!_hidden) { _hidden = true; _s.Hide?.Invoke(); }
            _paint?.Dispose(); _paint = null;
            _ghost?.Dispose(); _ghost = null;
            Note($"reveal {clock.Elapsed.TotalMilliseconds:0} ms at {_clock:0.00} s");
            if (Log) Debug.Log($"[devir] {_clock:0.0000}");   // PM_86: the frame the dive's painting hands over to the board
        }

        void BackLegacy(float t)
        {
            float e = CorridorTransition.Ease(Mathf.Clamp01(t));
            if (_board != null)
            {
                var rect = new Rect(_board.Origin, _board.Size);
                var from = new Vector3(rect.center.x, -rect.center.y, _homePos.z);
                float fromSize = Dive74.CoverSize(rect.size, _s.Screen);
                _s.Cam.orthographicSize = Mathf.Exp(Mathf.Lerp(Mathf.Log(fromSize), Mathf.Log(_homeSize), e));
                _s.Cam.transform.position = Vector3.Lerp(from, _homePos, e);
                // PM_76: the board settles with a small spring (it dips 2.5 % below its size and comes back); only the
                // board moves, so no screen edge is ever uncovered
                float k = Mathf.Clamp01((t - .7f) / .3f);
                float scale = 1 - .025f * Mathf.Sin(k * Mathf.PI);
                _board.transform.localScale = new Vector3(scale, scale, 1);
                _board.transform.localPosition = Draw.W(_board.Origin + _board.Size / 2 * (1 - scale));
            }
            _reveal?.Show(t);
        }

        /* Ends now, whatever the phase: the level is built, the camera home, the overlays gone. */
        /* PM_107 §C6: the smootherstep's speed at u (0..1), 1 at its peak. */
        public static float Bell107(float u) { u = Mathf.Clamp01(u); return 16 * u * u * (1 - u) * (1 - u); }

        public void Finish()
        {
            Gecis100.Dive(0);
            if (_finished) return;
            if (!_built) BuildLevel();
            if (!_revealed) StartReveal();
            _finished = true;
            Debug.Log($"[dalis] {(Legacy ? "eski" : Variant.ToString())}: {_notes}ended at {_clock:0.00} s; frames {_frames}, over 20 ms {_slow} (max {_maxDt * 1000:0} ms)");
            if (_s.Cam != null) { _s.Cam.transform.position = _homePos; _s.Cam.orthographicSize = _homeSize; }
            if (_board != null) { _board.transform.localScale = Vector3.one; _board.transform.localPosition = Draw.W(_board.Origin); }
            PullBack90(1);   // PM_90: the board's own crop
            _reveal?.Dispose(); _reveal = null;
            _s.FadeUi?.Invoke(0);
            _s.Slide?.Invoke(1);
            var done = _s.Done; _s.Done = null;
            Destroy(gameObject);
            done?.Invoke();
        }

        void OnDestroy()
        {
            if (_overlay != null) Destroy(_overlay.gameObject);
            foreach (var w in _walls) w.Dispose();
            foreach (var w in _dioLayers) w.Dispose();
            _paint?.Dispose();
            _ghost?.Dispose();
            _reveal?.Dispose();
        }

        // ---- homographies (3x3, row-major, [x y w] = H [x y 1]) ----------------------------------------------

        static double[] SquareToQuad(Vector2[] q)
        {
            double dx1 = q[1].x - q[2].x, dx2 = q[3].x - q[2].x, dx3 = q[0].x - q[1].x + q[2].x - q[3].x;
            double dy1 = q[1].y - q[2].y, dy2 = q[3].y - q[2].y, dy3 = q[0].y - q[1].y + q[2].y - q[3].y;
            double den = dx1 * dy2 - dx2 * dy1;
            double g = (dx3 * dy2 - dx2 * dy3) / den, h = (dx1 * dy3 - dx3 * dy1) / den;
            double a = q[1].x - q[0].x + g * q[1].x, b = q[3].x - q[0].x + h * q[3].x;
            double d = q[1].y - q[0].y + g * q[1].y, e = q[3].y - q[0].y + h * q[3].y;
            return new[] { a, b, q[0].x, d, e, q[0].y, g, h, 1 };
        }

        static double[] Inverse(double[] m)
        {
            double a = m[0], b = m[1], c = m[2], d = m[3], e = m[4], f = m[5], g = m[6], h = m[7], i = m[8];
            double A = e * i - f * h, B = -(d * i - f * g), C = d * h - e * g;
            double det = a * A + b * B + c * C;
            return new[]
            {
                A / det, -(b * i - c * h) / det, (b * f - c * e) / det,
                B / det, (a * i - c * g) / det, -(a * f - c * d) / det,
                C / det, -(a * h - b * g) / det, (a * e - b * d) / det
            };
        }

        static double[] Mul(double[] p, double[] q)
        {
            var r = new double[9];
            for (int row = 0; row < 3; row++)
                for (int col = 0; col < 3; col++)
                    r[row * 3 + col] = p[row * 3] * q[col] + p[row * 3 + 1] * q[3 + col] + p[row * 3 + 2] * q[6 + col];
            return r;
        }

        static double[] Similarity(Vector2 from, Vector2 to, float s) =>
            new double[] { s, 0, to.x - s * from.x, 0, s, to.y - s * from.y, 0, 0, 1 };

        public static double[] Homography(Vector2[] from, Vector2[] to) => Mul(SquareToQuad(to), Inverse(SquareToQuad(from)));

        public static Vector2 Apply(double[] h, Vector2 p)
        {
            double w = h[6] * p.x + h[7] * p.y + h[8];
            return new Vector2((float)((h[0] * p.x + h[1] * p.y + h[2]) / w), (float)((h[3] * p.x + h[4] * p.y + h[5]) / w));
        }

        // ---- layers --------------------------------------------------------------------------------------------

        internal sealed class Layer
        {
            MeshRenderer _r;
            Mesh _m;
            Material _mat;
            Vector2[] _src;          // wall: layout points; grid: (u, v)
            Vector3[] _v;
            Vector2[] _uv;
            Color[] _c;
            int _n;

            public static Layer FromSprite(Transform parent, SpriteRenderer sr, int order, Vector2 screen)
            {
                var sp = sr.sprite; var tex = sp.texture;
                if (tex != null) tex.wrapMode = TextureWrapMode.Clamp;   // the 8 % past top and bottom repeats the edge rows
                var lb = sp.bounds; var tr = sp.textureRect;
                // only the part near the screen (the corridor strip is many bays wide)
                Vector2 Layout(float u, float v)
                {
                    var w = sr.transform.TransformPoint(new Vector3(Mathf.LerpUnclamped(lb.min.x, lb.max.x, u), Mathf.LerpUnclamped(lb.max.y, lb.min.y, v), 0));   // past the edges too
                    return new Vector2(w.x, -w.y);
                }
                float x0 = Layout(0, 0).x, x1 = Layout(1, 0).x;
                float u0 = Mathf.Clamp01(Mathf.InverseLerp(x0, x1, -screen.x * .6f)), u1 = Mathf.Clamp01(Mathf.InverseLerp(x0, x1, screen.x * 1.6f));
                const int nx = 24, ny = 40;
                var l = new Layer { _n = nx };
                var src = new List<Vector2>(); var uv = new List<Vector2>(); var tris = new List<int>();
                for (int j = 0; j <= ny; j++)
                    for (int i = 0; i <= nx; i++)
                    {
                        float u = Mathf.Lerp(u0, u1, (float)i / nx), v = Mathf.Lerp(-.08f, 1.08f, (float)j / ny);   // PM_76: 8 % past top and bottom
                        src.Add(Layout(u, v));
                        uv.Add(new Vector2((tr.x + u * tr.width) / tex.width, (tr.y + (1 - v) * tr.height) / tex.height));
                        if (i < nx && j < ny) { int s = j * (nx + 1) + i; tris.AddRange(new[] { s, s + 1, s + nx + 2, s, s + nx + 2, s + nx + 1 }); }
                    }
                l.Make(parent, "wall", new Material(sr.sharedMaterial) { mainTexture = tex }, src.ToArray(), uv.ToArray(), tris, order);
                return l;
            }

            /* A texture's pixel rect `px` (working pixels, y down) laid out by `toLayout`, as an nx x ny grid. */
            public static Layer FromTexture(Transform parent, Texture tex, Material baseMat, Rect px, Rect texPx, Func<Vector2, Vector2> toLayout, int nx, int ny, int order)
            {
                var l = new Layer { _n = nx };
                var src = new List<Vector2>(); var uv = new List<Vector2>(); var tris = new List<int>();
                for (int j = 0; j <= ny; j++)
                    for (int i = 0; i <= nx; i++)
                    {
                        float u = (float)i / nx, v = (float)j / ny;
                        var at = new Vector2(px.x + u * px.width, px.y + v * px.height);
                        src.Add(toLayout(at));
                        // outside the texture's own rect the uv runs past 0..1: the clamped edge pixels stretch
                        uv.Add(new Vector2((at.x - texPx.x) / texPx.width, 1 - (at.y - texPx.y) / texPx.height));
                        if (i < nx && j < ny) { int s = j * (nx + 1) + i; tris.AddRange(new[] { s, s + 1, s + nx + 2, s, s + nx + 2, s + nx + 1 }); }
                    }
                l.Make(parent, "view", new Material(baseMat) { mainTexture = tex != null ? tex : Texture2D.whiteTexture }, src.ToArray(), uv.ToArray(), tris, order);
                return l;
            }

            public void Fade(float a) => SetAlpha(a);

            /* PM_77: a plane in the diorama: a texture's pixel rect `px` with each pixel's 3D point `at`, drawn by
               projecting every grid point each frame (perspective-correct to the grid's resolution). */
            public static Layer FromPlane(Transform parent, Texture tex, Material baseMat, Rect px, Rect texPx, Func<Vector2, Vector3> at, int nx, int ny, int order)
            {
                var l = FromTexture(parent, tex, baseMat, px, texPx, _ => Vector2.zero, nx, ny, order);
                l._p3 = new Vector3[l._src.Length];
                for (int j = 0, k = 0; j <= ny; j++)
                    for (int i = 0; i <= nx; i++, k++)
                        l._p3[k] = at(new Vector2(px.x + (float)i / nx * px.width, px.y + (float)j / ny * px.height));
                return l;
            }
            Vector3[] _p3;

            /* PM_78: a texture of size texSize laid out on an nx x ny grid by pxAt(u, v) (texture px, y down), each
               grid point's 3D point at(px) (the grid may follow a slanted edge, e.g. the wall's foot). */
            public static Layer FromGrid(Transform parent, string name, Texture tex, Material baseMat, Vector2 texSize,
                                         Func<float, float, Vector2> pxAt, Func<Vector2, Vector3> at, int nx, int ny, int order, bool clampUv = false)
            {
                var l = new Layer { _n = nx };
                var uv = new List<Vector2>(); var tris = new List<int>(); var p3 = new List<Vector3>();
                for (int j = 0; j <= ny; j++)
                    for (int i = 0; i <= nx; i++)
                    {
                        var px = pxAt((float)i / nx, (float)j / ny);
                        var t = new Vector2(px.x / texSize.x, 1 - px.y / texSize.y);
                        uv.Add(clampUv ? new Vector2(Mathf.Clamp01(t.x), Mathf.Clamp01(t.y)) : t);   // PM_108: past the plate, its edge
                        p3.Add(at(px));
                        if (i < nx && j < ny) { int s = j * (nx + 1) + i; tris.AddRange(new[] { s, s + 1, s + nx + 2, s, s + nx + 2, s + nx + 1 }); }
                    }
                l.Make(parent, name, new Material(baseMat) { mainTexture = tex != null ? tex : Texture2D.whiteTexture }, new Vector2[uv.Count], uv.ToArray(), tris, order);
                l._p3 = p3.ToArray();
                return l;
            }

            public void Project(Func<Vector3, Vector2> project, double[] post, float alpha)
            {
                for (int i = 0; i < _p3.Length; i++) _v[i] = Draw.W(Apply(post, project(_p3[i])));
                SetAlpha(alpha);
                _m.vertices = _v;
                _m.bounds = new Bounds(Vector3.zero, Vector3.one * 1e5f);
            }

            /* PM_80: this layer's current mesh into `to` (the same grid), at alpha a. */
            public void CopyTo(Layer to, float a)
            {
                if (to._v.Length != _v.Length) return;
                Array.Copy(_v, to._v, _v.Length); Array.Copy(_uv, to._uv, _uv.Length);
                to.SetAlpha(a);
                to._m.vertices = to._v; to._m.uv = to._uv;
                to._m.bounds = new Bounds(Vector3.zero, Vector3.one * 1e5f);
            }

            public void SetActive(bool on) { if (_r != null && _r.gameObject.activeSelf != on) _r.gameObject.SetActive(on); }

            public static Layer Grid(Transform parent, Texture tex, Material baseMat, int n, int order)
            {
                var l = new Layer { _n = n };
                var src = new List<Vector2>(); var tris = new List<int>();
                for (int j = 0; j <= n; j++)
                    for (int i = 0; i <= n; i++)
                    {
                        src.Add(new Vector2((float)i / n, (float)j / n));
                        if (i < n && j < n) { int s = j * (n + 1) + i; tris.AddRange(new[] { s, s + 1, s + n + 2, s, s + n + 2, s + n + 1 }); }
                    }
                var mat = new Material(baseMat) { mainTexture = tex != null ? tex : Texture2D.whiteTexture };
                l.Make(parent, "painting", mat, src.ToArray(), new Vector2[src.Count], tris, order);
                return l;
            }

            void Make(Transform parent, string name, Material mat, Vector2[] src, Vector2[] uv, List<int> tris, int order)
            {
                var go = new GameObject(name);
                go.transform.SetParent(parent, false);
                _mat = mat; _src = src; _uv = uv;
                _v = new Vector3[src.Length]; _c = new Color[src.Length];
                _m = new Mesh { name = name };
                _m.MarkDynamic();
                for (int i = 0; i < src.Length; i++) _v[i] = Draw.W(src[i]);
                _m.vertices = _v; _m.uv = _uv; _m.SetTriangles(tris, 0);
                for (int i = 0; i < _c.Length; i++) _c[i] = Color.white;
                _m.colors = _c;
                _m.bounds = new Bounds(Vector3.zero, Vector3.one * 1e5f);
                go.AddComponent<MeshFilter>().sharedMesh = _m;
                _r = go.AddComponent<MeshRenderer>();
                _r.sharedMaterial = mat;
                _r.sortingOrder = order;
            }

            public void Warp(double[] h, float alpha)
            {
                for (int i = 0; i < _src.Length; i++) _v[i] = Draw.W(Apply(h, _src[i]));
                SetAlpha(alpha);
                _m.vertices = _v;
                _m.bounds = new Bounds(Vector3.zero, Vector3.one * 1e5f);
            }

            /* PM_78: the positions and the uv each from the grid point (0..1, y down). */
            public void Positions(Func<float, float, Vector2> at, Func<float, float, Vector2> uv, float alpha)
            {
                for (int i = 0; i < _src.Length; i++)
                {
                    var p = _src[i];
                    _v[i] = Draw.W(at(p.x, p.y));
                    _uv[i] = uv(p.x, p.y);
                }
                SetAlpha(alpha);
                _m.vertices = _v; _m.uv = _uv;
                _m.bounds = new Bounds(Vector3.zero, Vector3.one * 1e5f);
            }

            public void Positions(Func<float, float, Vector2> at, Rect crop, float alpha)
            {
                for (int i = 0; i < _src.Length; i++)
                {
                    var p = _src[i];
                    _v[i] = Draw.W(at(p.x, p.y));
                    _uv[i] = new Vector2(crop.x + p.x * crop.width, crop.y + (1 - p.y) * crop.height);
                }
                SetAlpha(alpha);
                _m.vertices = _v; _m.uv = _uv;
                _m.bounds = new Bounds(Vector3.zero, Vector3.one * 1e5f);
            }

            void SetAlpha(float a)
            {
                if (_c.Length > 0 && Mathf.Approximately(_c[0].a, a)) return;
                for (int i = 0; i < _c.Length; i++) _c[i] = new Color(1, 1, 1, a);
                _m.colors = _c;
            }

            public void Dispose()
            {
                if (_r != null) Destroy(_r.gameObject);
                if (_m != null) Destroy(_m);
                if (_mat != null) Destroy(_mat);
            }
        }

        /* Over the board: the whole picture, the jigsaw cuts drawn on it edge by edge (a sweep from the top left,
           each edge .2 s, a bright tip while it draws), then the picture and the lines fade and the pieces remain. */
        sealed class Reveal
        {
            const float Width = 1.7f, Shadow = 3.4f, EdgeSeconds = .55f, Spread = .7f;   // PM_77: the same drawing over the 1 s pull back
            Transform _root;
            MeshRenderer _picture;
            Material _picMat, _lineMat;
            Mesh _picMesh, _lineMesh;
            readonly List<(List<Vector2> pts, float length, float start)> _edges = new List<(List<Vector2>, float, float)>();
            readonly List<SpriteRenderer> _tips = new List<SpriteRenderer>();
            static Sprite _dot;

            public static Reveal Create(BoardView board, Texture picture, bool flow = false)
            {
                var r = new Reveal { _flow = flow };
                r._root = new GameObject("PM75 reveal").transform;
                r._root.SetParent(board.transform, false);
                var size = board.Size; float cell = board.Cell;
                var baseMat = SpriteMaterial();
                // the picture over the board rect, stretched like the pieces' image (uv 0..1 over the board)
                r._picMat = new Material(baseMat) { mainTexture = picture != null ? picture : Texture2D.whiteTexture };
                r._picMesh = new Mesh { name = "reveal picture" };
                // PM_90: a grid, its uv through the dive's warp (Warp); the board's own crop until then
                const int G = PicGrid;
                var pv3 = new Vector3[(G + 1) * (G + 1)]; var ptris = new List<int>();
                for (int j = 0; j <= G; j++)
                    for (int i = 0; i <= G; i++)
                    {
                        pv3[j * (G + 1) + i] = Draw.W(size.x * i / G, size.y * j / G);
                        if (i < G && j < G) { int k = j * (G + 1) + i; ptris.AddRange(new[] { k, k + 1, k + G + 2, k, k + G + 2, k + G + 1 }); }
                    }
                r._picMesh.vertices = pv3;
                r._picMesh.triangles = ptris.ToArray();
                r._picColors = new Color[pv3.Length];
                r.SetPicAlpha(1);
                var bcr = LevelImages.Crop(picture, size.x / Mathf.Max(1e-3f, size.y));
                r.Warp(new double[] { bcr.width, 0, bcr.x, 0, bcr.height, bcr.y, 0, 0, 1 });
                var pg = new GameObject("picture"); pg.transform.SetParent(r._root, false);
                pg.AddComponent<MeshFilter>().sharedMesh = r._picMesh;
                r._picture = pg.AddComponent<MeshRenderer>(); r._picture.sharedMaterial = r._picMat; r._picture.sortingOrder = Order;
                // the cuts, from the board's joints (tabs: circles of radius .18 centred .08 past the edge)
                r._lineMat = new Material(baseMat) { mainTexture = Texture2D.whiteTexture };
                r._lineMesh = new Mesh { name = "reveal lines" }; r._lineMesh.MarkDynamic();
                var lg = new GameObject("lines"); lg.transform.SetParent(r._root, false);
                lg.AddComponent<MeshFilter>().sharedMesh = r._lineMesh;
                var lr = lg.AddComponent<MeshRenderer>(); lr.sharedMaterial = r._lineMat; lr.sortingOrder = Order + 1;
                var b = board.Session.Board; var l = board.Session.Level;
                float diag = l.Width + l.Height;
                var rng = new System.Random(l.Width * 31 + l.Height);
                // PM_77: a sweep from the top left; PM_80: from the centre outwards
                var mid = new Vector2(l.Width, l.Height) / 2; float far = Mathf.Max(.5f, mid.magnitude - .5f);
                float Sweep(Vector2 m) => flow ? Mathf.Clamp01(((m - mid).magnitude - .5f) / far) : (m.x + m.y) / diag;
                for (int y = 0; y < l.Height; y++)
                    for (int x = 1; x < l.Width; x++)
                        r._vStart[(x, y)] = r.AddEdge(new Vector2(x, y), new Vector2(0, 1), b.VJoint(x, y), cell, Sweep(new Vector2(x, y + .5f)), rng);
                for (int y = 1; y < l.Height; y++)
                    for (int x = 0; x < l.Width; x++)
                        r._hStart[(x, y)] = r.AddEdge(new Vector2(x, y), new Vector2(1, 0), b.HJoint(x, y), cell, Sweep(new Vector2(x + .5f, y)), rng);
                if (flow) r.MeasureDrawing();
                // PM_84/86: the cuts are the board's own: the real pieces show the plain picture with their own cut
                // drawn in pen by pen (the piece shader's mask, the same material, place and scale); no layer comes or
                // goes. After the cuts they take their own look, the shadow and the parting (look, PieceView.SetReveal);
                // the picture lies under them only to fill the hairlines between them, and the badges fade in with the look
                if (flow && Art.Texture("kit-mask") != null)
                {
                    float outer = Spread + .05f;
                    float StartOf(Cell c, Dir d) => d switch
                    {
                        Dir.L => c.X > 0 && r._vStart.TryGetValue((c.X, c.Y), out var a) ? a : outer,
                        Dir.R => c.X + 1 < l.Width && r._vStart.TryGetValue((c.X + 1, c.Y), out var b2) ? b2 : outer,
                        Dir.U => c.Y > 0 && r._hStart.TryGetValue((c.X, c.Y), out var u) ? u : outer,
                        _ => c.Y + 1 < l.Height && r._hStart.TryGetValue((c.X, c.Y + 1), out var dd) ? dd : outer,
                    };
                    for (int i = 0; i < l.Pieces.Count; i++)
                    {
                        var pv = b.IsPresent(i) ? board.PieceViewOf(i) : null;
                        if (pv == null) continue;
                        pv.SetSeamStarts(StartOf);
                        r._seams.Add(pv);
                        foreach (var rr in pv.GetComponentsInChildren<Renderer>(true))
                            if (rr.gameObject != pv.gameObject) r._badges.Add(rr);
                    }
                    foreach (var rr in board.GetComponentsInChildren<Renderer>(true))
                        if (rr.sortingOrder >= 1290 && rr.GetComponentInParent<PieceView>() == null && !rr.transform.IsChildOf(r._root)) r._badges.Add(rr);
                    r._picture.sortingOrder = 99;   // under the pieces (100+), over the tray
                    lr.enabled = false;
                }
                if (_dot == null) _dot = MakeDot();
                for (int i = 0; i < 24; i++)
                {
                    var tip = new GameObject("tip").AddComponent<SpriteRenderer>();
                    tip.transform.SetParent(r._root, false);
                    tip.sprite = _dot; tip.sortingOrder = Order + 2; tip.enabled = false;
                    tip.transform.localScale = Vector3.one * (cell * .16f / (_dot.bounds.size.x));
                    r._tips.Add(tip);
                }
                return r;
            }
            const int Order = 2000;   // over the pieces, chains, walls and badges (1800); under the HUD (3000)
            const int PicGrid = 8;
            Color[] _picColors;
            Vector2[] _picUv;
            void SetPicAlpha(float a)
            {
                var c = new Color(1, 1, 1, a);
                for (int i = 0; i < _picColors.Length; i++) _picColors[i] = c;
                _picMesh.colors = _picColors;
            }
            /* PM_90 §3: the picture under the cuts through h (board uv, v up -> picture uv), as the pieces. */
            public void Warp(double[] h)
            {
                const int G = PicGrid;
                if (_picUv == null) _picUv = new Vector2[(G + 1) * (G + 1)];
                for (int j = 0; j <= G; j++)
                    for (int i = 0; i <= G; i++)
                    {
                        var uv = Apply(h, new Vector2(i / (float)G, 1 - j / (float)G));
                        _picUv[j * (G + 1) + i] = new Vector2(Mathf.Clamp01(uv.x), Mathf.Clamp01(uv.y));
                    }
                _picMesh.uv = _picUv;
            }

            readonly Dictionary<(int, int), float> _vStart = new Dictionary<(int, int), float>(), _hStart = new Dictionary<(int, int), float>();
            readonly List<PieceView> _seams = new List<PieceView>();
            readonly List<Renderer> _badges = new List<Renderer>();
            public bool Seams => _seams.Count > 0;
            float _look = -1;

            void Badges(float a)
            {
                foreach (var b in _badges)
                {
                    if (b == null) continue;
                    if (b.TryGetComponent<ShapeView>(out var sv)) sv.Alpha(a);
                    else if (b.TryGetComponent<SolidView>(out var so)) so.Alpha(a);
                    else if (b.TryGetComponent<TextView>(out var tv)) tv.Alpha = a;
                    else if (b is SpriteRenderer sr) { var c = sr.color; c.a = a; sr.color = c; }
                    else b.enabled = a > .01f;
                }
            }

            float AddEdge(Vector2 at, Vector2 along, int joint, float cell, float sweep, System.Random rng)
            {
                var n = new Vector2(along.y, along.x);             // the bulge side: +x for a vertical edge, +y for a horizontal one
                var pts = new List<Vector2>();
                void P(Vector2 c) => pts.Add(c * cell);
                const float half = .161f, off = .08f, rad = .18f;
                P(at);
                if (joint != 0)
                {
                    // the left / upper cell owns the tab: it bulges +n. In the edge's frame (along, out) the circle meets
                    // the edge line at angles a0 and a1; the cut goes the long way round, through the far point.
                    var outward = n * (joint > 0 ? 1 : -1);
                    var centre = at + along * .5f + outward * off;
                    float a0 = Mathf.Atan2(-off, -half), a1 = Mathf.Atan2(-off, half);
                    float arc = -(2 * Mathf.PI - (a1 - a0));
                    const int steps = 14;
                    for (int k = 0; k <= steps; k++)
                    {
                        float a = a0 + arc * k / steps;
                        P(centre + along * (Mathf.Cos(a) * rad) + outward * (Mathf.Sin(a) * rad));
                    }
                }
                P(at + along);
                float length = 0;
                for (int i = 1; i < pts.Count; i++) length += (pts[i] - pts[i - 1]).magnitude;
                float start = sweep * Spread + (float)rng.NextDouble() * .05f;
                _edges.Add((pts, length, start));
                return start;
            }

            static Sprite MakeDot()
            {
                const int n = 32;
                var tex = new Texture2D(n, n, TextureFormat.RGBA32, false) { hideFlags = HideFlags.HideAndDontSave, wrapMode = TextureWrapMode.Clamp };
                var px = new Color[n * n];
                for (int y = 0; y < n; y++)
                    for (int x = 0; x < n; x++)
                    {
                        float d = new Vector2(x + .5f - n / 2f, y + .5f - n / 2f).magnitude / (n / 2f);
                        float a = Mathf.Clamp01(1 - d); a *= a;
                        px[y * n + x] = new Color(1f, .97f, .85f, a);
                    }
                tex.SetPixels(px); tex.Apply();
                return Sprite.Create(tex, new Rect(0, 0, n, n), new Vector2(.5f, .5f), n);
            }

            bool _flow;
            float[] _drawn;          // PM_80: the share of all the cuts drawn at each step of the drawing's own clock
            const int DrawSteps = 64;
            float DrawEnd => Spread + .05f + EdgeSeconds;
            float EdgeEase(float f) => _flow ? f * f * (3 - 2 * f) : BoosterFx74.OutCubic(f);   // PM_80: a pen's even stroke

            void MeasureDrawing()
            {
                _drawn = new float[DrawSteps + 1];
                float total = 0;
                foreach (var e in _edges) total += e.length;
                for (int i = 0; i <= DrawSteps; i++)
                {
                    float time = DrawEnd * i / DrawSteps, sum = 0;
                    foreach (var (_, length, start) in _edges) sum += EdgeEase(Mathf.Clamp01((time - start) / EdgeSeconds)) * length;
                    _drawn[i] = total > 0 ? sum / total : 1;
                }
            }

            /* PM_80: share 0..1 of the cuts drawn (from the centre out); back 0..1 the pull back: the picture gives way
               to the pieces from 60 %, the cuts' shadow widens a little (the pieces part, 1-2 px), then they fade. */
            public void Flow(float share, float back)
            {
                float time = DrawEnd;
                if (_drawn != null && share < 1)
                {
                    int i = 0;
                    while (i < DrawSteps && _drawn[i + 1] < share) i++;
                    float d = _drawn[Mathf.Min(DrawSteps, i + 1)] - _drawn[i];
                    time = DrawEnd * (i + (d > 1e-6f ? Mathf.Clamp01((share - _drawn[i]) / d) : 0)) / DrawSteps;
                }
                float sm(float x) { x = Mathf.Clamp01(x); return x * x * (3 - 2 * x); }
                if (Seams)
                {
                    // PM_86: every cut is drawn by LinesDone of the pull back; from there to its end the pieces take their
                    // own look (at least .2 s), the shadow and the parting with it
                    _look = sm((back - Dive75.LinesDone) / Mathf.Max(.05f, 1 - Dive75.LinesDone));
                    Render(time, 1, 1, Shadow);
                    return;
                }
                Render(time, 1 - sm((back - .6f) / .32f), 1 - sm((back - .84f) / .16f), Shadow + 1.6f * sm((back - .7f) / .25f));
            }

            /* t: 0..1 over the pull back. */
            public void Show(float t)
            {
                float time = t * 1.0f;   // the drawing's own timeline (PM_77's 1 s pull back), scaled to the back's length
                Render(time, 1 - Mathf.Clamp01((t - .62f) / .28f), 1 - Mathf.Clamp01((t - .78f) / .22f), Shadow);
            }

            void Render(float time, float pictureAlpha, float lineAlpha, float shadow)
            {
                SetPicAlpha(pictureAlpha);
                var verts = new List<Vector3>(); var cols = new List<Color>(); var tris = new List<int>();
                int tip = 0;
                foreach (var (pts, length, start) in _edges)
                {
                    float f = Mathf.Clamp01((time - start) / EdgeSeconds);
                    if (f <= 0) continue;
                    float drawn = EdgeEase(f) * length;
                    Vector2 end = Seams ? PointAt(pts, drawn) : Polyline(pts, drawn, verts, cols, tris, lineAlpha, shadow);
                    if (f < 1 && tip < _tips.Count)
                    {
                        var s = _tips[tip++]; s.enabled = true;
                        s.transform.localPosition = Draw.W(end);
                        s.color = new Color(1, 1, 1, lineAlpha);
                    }
                }
                for (int i = tip; i < _tips.Count; i++) _tips[i].enabled = false;
                if (Seams)
                {
                    float look = Mathf.Max(0, _look);
                    foreach (var pv in _seams) if (pv != null) pv.SetReveal(true, time, EdgeSeconds, look);
                    SetPicAlpha(1 - look);
                    Badges(look);
                }
                _lineMesh.Clear();
                _lineMesh.SetVertices(verts); _lineMesh.SetColors(cols); _lineMesh.SetTriangles(tris, 0);
                _lineMesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1e5f);
            }

            /* The first `length` points of the polyline as a shadowed cream stroke; returns where it ends. */
            static Vector2 Polyline(List<Vector2> pts, float length, List<Vector3> verts, List<Color> cols, List<int> tris, float alpha, float shadow)
            {
                var shade = new Color(.07f, .2f, .23f, .55f * alpha); var cream = new Color(1f, .96f, .86f, .95f * alpha);
                float left = length; Vector2 last = pts[0];
                for (int i = 1; i < pts.Count && left > 0; i++)
                {
                    var a = pts[i - 1]; var b = pts[i];
                    float seg = (b - a).magnitude;
                    if (seg <= 1e-4f) continue;
                    if (seg > left) { b = a + (b - a) * (left / seg); seg = left; }
                    left -= seg;
                    Quad(a, b, shadow, shade, verts, cols, tris);
                    Quad(a, b, Width, cream, verts, cols, tris);
                    last = b;
                }
                return last;
            }

            /* The point `length` along the polyline (the pen's tip). */
            static Vector2 PointAt(List<Vector2> pts, float length)
            {
                for (int i = 1; i < pts.Count; i++)
                {
                    float seg = (pts[i] - pts[i - 1]).magnitude;
                    if (seg >= length) return pts[i - 1] + (pts[i] - pts[i - 1]) * (seg > 1e-5f ? length / seg : 0);
                    length -= seg;
                }
                return pts[pts.Count - 1];
            }

            static void Quad(Vector2 a, Vector2 b, float width, Color c, List<Vector3> verts, List<Color> cols, List<int> tris)
            {
                var d = (b - a).normalized; var n = new Vector2(-d.y, d.x) * width / 2;
                a -= d * width * .35f; b += d * width * .35f;           // overlap a little: no gaps at the joints
                int s = verts.Count;
                verts.Add(Draw.W(a + n)); verts.Add(Draw.W(b + n)); verts.Add(Draw.W(b - n)); verts.Add(Draw.W(a - n));
                cols.Add(c); cols.Add(c); cols.Add(c); cols.Add(c);
                tris.AddRange(new[] { s, s + 1, s + 2, s, s + 2, s + 3 });
            }

            public void Dispose()
            {
                // PM_86: the pieces as always (by now the look is whole and every cut drawn: nothing changes on screen)
                foreach (var pv in _seams) if (pv != null) pv.SetReveal(false, 0, EdgeSeconds, 1);
                if (Seams) Badges(1);
                _seams.Clear();
                if (_root != null) Destroy(_root.gameObject);
                if (_picMesh != null) Destroy(_picMesh);
                if (_lineMesh != null) Destroy(_lineMesh);
                if (_picMat != null) Destroy(_picMat);
                if (_lineMat != null) Destroy(_lineMat);
            }
        }
    }
}
