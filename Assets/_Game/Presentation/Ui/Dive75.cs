using System;
using System.Collections.Generic;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* The dive into a painting (PM_75, re-timed in PM_77: Zeyd "why so fast, so hurried? slowly, step by step").
       Each step is its own move, with a short breath between them:
         a  approach   1.1 s  in to the painting until the screen is only the picture (ease in-out)
         b  hold        .5 s  only the picture; the level is built behind it
         c  back       1.0 s  out to the board: the jigsaw cuts draw on the picture piece by piece, the pieces take over
         d  HUD         .4 s  the HUD from the top, the booster bar from the bottom
       breaths .15 / .15 / .1 s — 3.4 s. From the menu with &gecis=yay (waiting for Zeyd's approval) the camera first
       flies Zeyd's path through the menu picture itself as a 3D diorama (PM_78, Diorama78): 3.6 s without a stop, and
       the approach sets off .8 s before the flight ends, so the camera never stands still between them.
       A tap makes it twice as fast; a second tap ends it. The screen-space layers ride on the camera, so the camera
       can already sit on the board while the picture still covers the screen. */
    public sealed class Dive75 : MonoBehaviour
    {
        public const float Approach = 1.1f, Breath1 = .15f, HoldSec = .5f, Breath2 = .15f, BackSec = 1.0f, Breath3 = .1f, HudSec = .4f;
        public const float Overlap = .8f, CrossIn = .35f;   // PM_78: the flight is at 60 % of its top speed when the approach sets off
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

        public static Dive75 Run(Setup s)
        {
            var d = new GameObject("PM75 dive").AddComponent<Dive75>();
            d._s = s;
            d._homePos = s.Cam.transform.position; d._homeSize = s.Cam.orthographicSize;
            d.Prepare();
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
            if (_s.Diorama != null)
            {
                // the wall and the floor; over the wall the painting in its canvas; in front the column, its banner and
                // its sconce
                _dio = new Diorama78(_s.Diorama, screen, _s.ImageCentre, _s.ImageK);
                _dio.Build(_overlay, baseMat, _s.Order + 1, _dioLayers);
                _paint = Layer.Grid(_overlay, _s.Picture, baseMat, 12, _s.Order + 2);
                _dio.BuildFronts(_overlay, baseMat, _s.Order + 3, _dioLayers);
                Debug.Log($"[yay] start {_dio.StartAngle:0.0} deg {_dio.Describe(0)}, end {_dio.Describe(1)}, Zeyd's path x{_dio.Scale:0.000} W per metre");
                var front = _dio.CanvasAt(1);
                _r = Rect.MinMaxRect(Mathf.Min(front[0].x, front[3].x), Mathf.Min(front[0].y, front[1].y), Mathf.Max(front[1].x, front[2].x), Mathf.Max(front[2].y, front[3].y));
                float ch2 = Mathf.Max(screen.y, screen.x / Mathf.Max(.1f, _s.BoardAspect)), cw2 = ch2 * _s.BoardAspect;
                _c = new Rect(screen.x / 2 - cw2 / 2, screen.y / 2 - ch2 / 2, cw2, ch2);
                float ta = _s.Picture != null ? (float)_s.Picture.width / _s.Picture.height : 1, ca = _r.width / _r.height;
                _cropFront = ta > ca ? new Rect((1 - ca / ta) / 2, 0, ca / ta, 1) : new Rect(0, (1 - ta / ca) / 2, 1, ta / ca);
                return;
            }
            _paint = Layer.Grid(_overlay, _s.Picture, baseMat, 12, _s.Order + 2);
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
        }

        /* The sprites' own material (a shader found by name may be stripped from the build). */
        static Material SpriteMaterial()
        {
            var any = FindAnyObjectByType<SpriteRenderer>();
            return any != null ? any.sharedMaterial : new Material(Shader.Find("Sprites/Default"));
        }

        static Vector2[] Corners(Rect r) => new[] { new Vector2(r.xMin, r.yMin), new Vector2(r.xMax, r.yMin), new Vector2(r.xMax, r.yMax), new Vector2(r.xMin, r.yMax) };

        int _lastFrame = -1;
        void LateUpdate()
        {
            if (_finished || _s.Cam == null || Time.frameCount == _lastFrame) return;
            _lastFrame = Time.frameCount;
            // a long frame (the level being built) must not eat the hold: at most 1/30 s a frame
            _clock += Mathf.Min(Time.unscaledDeltaTime, 1 / 30f) * _speed;
            Step();
        }

        void Step()
        {
            if (_finished || _s.Cam == null) return;
            float t = _clock;
            PlaceOverlay();
            if (!_raised && t >= (_dio != null && !_s.Seamless ? CrossIn : .22f)) Raise();
            // PM_78: the approach sets off before the flight ends (Overlap): the camera never stands still in between
            float flight = _dio != null ? _dio.Seconds : 0, approachAt = _dio != null ? flight - Overlap : 0;
            float a1 = approachAt + Approach, b0 = a1 + Breath1, b1 = b0 + HoldSec, c0 = b1 + Breath2, c1 = c0 + BackSec, d0 = c1 + Breath3, d1 = d0 + HudSec;
            if (t < c0)
            {
                _s.FadeUi?.Invoke(1 - Mathf.Clamp01(t / .22f));
                float e = BoosterFx74.InOutCubic(Mathf.Clamp01((t - approachAt) / Approach));
                if (_dio != null)
                {
                    // Zeyd's studio: one global smootherstep over the whole flight (a soft start, a soft end, no stop)
                    DioramaPose(SmootherStep(t / flight), e, _s.Seamless ? 1 : Mathf.Clamp01(t / CrossIn));
                    if (!_hidden && !_s.Seamless && t >= CrossIn) { _hidden = true; _s.Hide?.Invoke(); }
                }
                else FacePose(e);
                if (t >= b0 && !_built) BuildLevel();   // the level is built in the hold, after the breath
                return;
            }
            if (!_built) BuildLevel();
            if (!_revealed) StartReveal();
            if (t < c1) { BackPhase((t - c0) / BackSec); return; }
            BackPhase(1);
            if (t < d0) return;
            _s.Slide?.Invoke(BoosterFx74.OutCubic(Mathf.Clamp01((t - d0) / HudSec)));
            if (t >= d1) Finish();
        }

        bool _hidden;
        static float SmootherStep(float t) { t = Mathf.Clamp01(t); return t * t * t * (t * (t * 6 - 15) + 10); }

        /* The approach without an orbit (the corridor, or the menu without &gecis=yay): the painting's slight
           perspective eases out while the camera moves in, all in one move. */
        void FacePose(float e)
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
            PaintInto(zq, e, _s.Crop, 1f, e <= 0 ? q : null, wall);
            _backdrop.Alpha(1);
        }

        /* &gecis=yay: o the flight 0 (the menu's view) .. 1 (Zeyd's last key), e the approach, fade the diorama's
           fade-in over the menu at the start (none when the menu shows the diorama itself). */
        void DioramaPose(float o, float e, float fade)
        {
            var rect = LogLerp(_r, _c, e);
            var zoom = Similarity(_r.center, rect.center, rect.height / _r.height);
            _dio.Pose(o, zoom, _dioLayers, fade);
            var cq = _dio.CanvasAt(o);
            var zq = new Vector2[4];
            for (int c = 0; c < 4; c++) zq[c] = Apply(zoom, cq[c]);
            var crop0 = new Rect(Vector2.Lerp(_s.Crop.position, _cropFront.position, o), Vector2.Lerp(_s.Crop.size, _cropFront.size, o));
            // exactly the menu's opening (PM_78: the frame is the menu's own); at the start the picture lies in it as the
            // menu draws it (flat, cut to the opening), easing into the canvas's true perspective along the flight
            PaintInto(zq, e, crop0, 1, null, null, 1 - o);
            _paint.Fade(fade);
            _backdrop.Alpha(fade);
        }

        /* The painting: in the canvas quad zq (perspective-correct) and, with the approach e, the board's framing;
           its crop eases from crop0 to the whole picture. While a wall still turns (start != null) the picture
           follows the same homography as the wall. flat (PM_78): how much of the menu's own way of drawing it is
           left (Screens.Menu.SetPainting: the crop laid flat over the opening's bounding box, cut to the opening). */
        void PaintInto(Vector2[] zq, float e, Rect crop0, float enlarge, Vector2[] start, double[] wall, float flat = 0)
        {
            var crop = new Rect(Vector2.Lerp(crop0.position, Vector2.zero, e), Vector2.Lerp(crop0.size, Vector2.one, e));
            var zr = Rect.MinMaxRect(Mathf.Min(zq[0].x, zq[3].x), Mathf.Min(zq[0].y, zq[1].y), Mathf.Max(zq[1].x, zq[2].x), Mathf.Max(zq[2].y, zq[3].y));
            float boardW = zr.height * Mathf.Lerp(zr.width / Mathf.Max(1, zr.height), _s.BoardAspect, e);
            float w = Mathf.Max(boardW, zr.width);
            var target = new Rect(zr.center.x - w / 2, zr.yMin, w, zr.height);
            var tq = Corners(target);
            var pq = new Vector2[4];
            for (int c = 0; c < 4; c++) pq[c] = Vector2.Lerp(zq[c], tq[c], e);
            var pc = (pq[0] + pq[1] + pq[2] + pq[3]) / 4;
            for (int c = 0; c < 4; c++) pq[c] = pc + (pq[c] - pc) * enlarge;
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

        void BuildLevel()
        {
            _built = true;
            _board = _s.Build?.Invoke();
            _s.Slide?.Invoke(0);
            // the level's own home view (as built), where the pull back ends
            _homePos = _s.Cam.transform.position; _homeSize = _s.Cam.orthographicSize;
            if (_board == null) return;
            var rect = new Rect(_board.Origin, _board.Size);
            _s.Cam.transform.position = new Vector3(rect.center.x, -rect.center.y, _homePos.z);
            _s.Cam.orthographicSize = Dive74.CoverSize(rect.size, _s.Screen);
            PlaceOverlay();   // this frame already draws with the new camera (PM_77: one frame 3x too close since PM_75)
        }

        void StartReveal()
        {
            _revealed = true;
            if (_board != null) _reveal = Reveal.Create(_board, _board.Image != null ? _board.Image : _s.Picture);
            if (_overlay != null) Destroy(_overlay.gameObject);
            _overlay = null;
            foreach (var w in _walls) w.Dispose();
            _walls.Clear();
            foreach (var w in _dioLayers) w.Dispose();
            _dioLayers.Clear();
            if (!_hidden) { _hidden = true; _s.Hide?.Invoke(); }
            _paint?.Dispose(); _paint = null;
        }

        void BackPhase(float t)
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
            _s.Slide?.Invoke(0);   // PM_77: the HUD comes in its own step, after a breath
        }

        /* Ends now, whatever the phase: the level is built, the camera home, the overlays gone. */
        public void Finish()
        {
            if (_finished) return;
            if (!_built) BuildLevel();
            if (!_revealed) StartReveal();
            _finished = true;
            if (_s.Cam != null) { _s.Cam.transform.position = _homePos; _s.Cam.orthographicSize = _homeSize; }
            if (_board != null) { _board.transform.localScale = Vector3.one; _board.transform.localPosition = Draw.W(_board.Origin); }
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
                                         Func<float, float, Vector2> pxAt, Func<Vector2, Vector3> at, int nx, int ny, int order)
            {
                var l = new Layer { _n = nx };
                var uv = new List<Vector2>(); var tris = new List<int>(); var p3 = new List<Vector3>();
                for (int j = 0; j <= ny; j++)
                    for (int i = 0; i <= nx; i++)
                    {
                        var px = pxAt((float)i / nx, (float)j / ny);
                        uv.Add(new Vector2(px.x / texSize.x, 1 - px.y / texSize.y));
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

            public static Reveal Create(BoardView board, Texture picture)
            {
                var r = new Reveal();
                r._root = new GameObject("PM75 reveal").transform;
                r._root.SetParent(board.transform, false);
                var size = board.Size; float cell = board.Cell;
                var baseMat = SpriteMaterial();
                // the picture over the board rect, stretched like the pieces' image (uv 0..1 over the board)
                r._picMat = new Material(baseMat) { mainTexture = picture != null ? picture : Texture2D.whiteTexture };
                r._picMesh = new Mesh { name = "reveal picture" };
                r._picMesh.vertices = new[] { Draw.W(0, 0), Draw.W(size.x, 0), Draw.W(size.x, size.y), Draw.W(0, size.y) };
                r._picMesh.uv = new[] { new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0) };
                r._picMesh.triangles = new[] { 0, 1, 2, 0, 2, 3 };
                r._picMesh.colors = new[] { Color.white, Color.white, Color.white, Color.white };
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
                for (int y = 0; y < l.Height; y++)
                    for (int x = 1; x < l.Width; x++)
                        r.AddEdge(new Vector2(x, y), new Vector2(0, 1), b.VJoint(x, y), cell, (x + y + .5f) / diag, rng);
                for (int y = 1; y < l.Height; y++)
                    for (int x = 0; x < l.Width; x++)
                        r.AddEdge(new Vector2(x, y), new Vector2(1, 0), b.HJoint(x, y), cell, (x + y + .5f) / diag, rng);
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

            void AddEdge(Vector2 at, Vector2 along, int joint, float cell, float sweep, System.Random rng)
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
                _edges.Add((pts, length, sweep * Spread + (float)rng.NextDouble() * .05f));
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

            /* t: 0..1 over the pull back. */
            public void Show(float t)
            {
                float time = t * BackSec;
                float pictureAlpha = 1 - Mathf.Clamp01((t - .62f) / .28f);
                float lineAlpha = 1 - Mathf.Clamp01((t - .78f) / .22f);
                var c = new Color(1, 1, 1, pictureAlpha);
                _picMesh.colors = new[] { c, c, c, c };
                var verts = new List<Vector3>(); var cols = new List<Color>(); var tris = new List<int>();
                int tip = 0;
                foreach (var (pts, length, start) in _edges)
                {
                    float f = Mathf.Clamp01((time - start) / EdgeSeconds);
                    if (f <= 0) continue;
                    float drawn = BoosterFx74.OutCubic(f) * length;
                    Vector2 end = Polyline(pts, drawn, verts, cols, tris, lineAlpha);
                    if (f < 1 && tip < _tips.Count)
                    {
                        var s = _tips[tip++]; s.enabled = true;
                        s.transform.localPosition = Draw.W(end);
                        s.color = new Color(1, 1, 1, lineAlpha);
                    }
                }
                for (int i = tip; i < _tips.Count; i++) _tips[i].enabled = false;
                _lineMesh.Clear();
                _lineMesh.SetVertices(verts); _lineMesh.SetColors(cols); _lineMesh.SetTriangles(tris, 0);
                _lineMesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1e5f);
            }

            /* The first `length` points of the polyline as a shadowed cream stroke; returns where it ends. */
            static Vector2 Polyline(List<Vector2> pts, float length, List<Vector3> verts, List<Color> cols, List<int> tris, float alpha)
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
                    Quad(a, b, Shadow, shade, verts, cols, tris);
                    Quad(a, b, Width, cream, verts, cols, tris);
                    last = b;
                }
                return last;
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
                if (_root != null) Destroy(_root.gameObject);
                if (_picMesh != null) Destroy(_picMesh);
                if (_lineMesh != null) Destroy(_lineMesh);
                if (_picMat != null) Destroy(_picMat);
                if (_lineMat != null) Destroy(_lineMat);
            }
        }
    }
}
