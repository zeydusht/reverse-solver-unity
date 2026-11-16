using System;
using System.Collections.Generic;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_75: the slow dive into a painting, ≈ 2.5 s (Zeyd: "too fast, straight in, like a teleport").
         1 face  .5 s  the camera comes round to face the painting square on: the wall's perspective eases out
                       (one homography for the whole wall plane, the frame included)
         2 near  .6 s  in to the painting until it fills the screen; the picture settles into the board's framing
         3 hold  .5 s  only the picture; the level is built behind it
         4 back  .6 s  out to the board: the jigsaw cuts draw on the picture piece by piece, then the pieces take over
         5 hud   .3 s  the HUD and the booster bar slide in last
       A tap makes it three times faster; a second tap ends it. The screen-space layers ride on the camera, so the
       camera can already sit on the board while the picture still covers the screen. */
    /* PM_76 (Zeyd: "the wall turns towards us, robotic"): from the menu the camera now travels on an arc round the
       painting to face it, through four drawn views of the same room (Codex, a0 = today's menu view .. a3 = square
       on). Two neighbouring views are shown at once, each warped so their canvases meet on the same in-between quad
       (the frame never doubles) and the newer one fades in over the older; the near foreground slides faster. The
       approach begins before the arc ends (one move, no stop): arc .85 s, approach from .61 s for .7 s, hold .45 s,
       back .6 s, the HUD's last .2 s — 2.56 s. The corridor keeps the facing homography (it arrives nearly square on). */
    public sealed class Dive75 : MonoBehaviour
    {
        public const float Face = .5f, Near = .6f, Hold = .5f, Back = .6f, HudIn = .3f;
        public const float ArcSeconds = .85f, ArcNearAt = .72f, ArcNear = .7f, ArcHold = .45f, ArcHudTail = .2f;
        public static bool Gap;   // &dalisbosluk=1: a magenta backdrop, to count uncovered edge pixels in captures

        /* One drawn view of the menu's room (working pixels, 1180x1770, y down). */
        public sealed class ArcView
        {
            public Texture Bg, Fg;
            public Rect FgPx;
            public Vector2[] Canvas;
        }
        public const float Total = Face + Near + Hold + Back + HudIn;
        public static bool On = true;

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
            public List<ArcView> Arc;            // PM_76: the camera arc (menu); null: the facing homography (corridor)
            public float FaceSeconds = .5f, NearAt = .38f;   // PM_76: the approach overlaps the facing turn
            public bool FaceFromMotion;          // the corridor is still moving at the handover: the turn starts at speed
            public Vector2 ArcSize = new Vector2(1180, 1770);
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
        readonly List<Layer> _arcBg = new List<Layer>(), _arcFg = new List<Layer>();
        readonly List<Vector2[]> _arcQuads = new List<Vector2[]>();
        Rect _cropA3;

        public static Dive75 Run(Setup s)
        {
            var d = new GameObject("PM75 dive").AddComponent<Dive75>();
            d._s = s;
            d._homePos = s.Cam.transform.position; d._homeSize = s.Cam.orthographicSize;
            d.Prepare();
            s.Hide?.Invoke();
            d._lastFrame = Time.frameCount;      // the clock starts on the next frame
            d.Step();
            return d;
        }

        /* A tap: three times faster; the second one ends the dive. */
        public void Tap()
        {
            _taps++;
            if (_taps == 1) _speed = 3; else Finish();
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
            if (_s.Arc != null && _s.Arc.Count >= 2)
            {
                // under the views: the painting shows through their cut-out canvases
                _paint = Layer.Grid(_overlay, _s.Picture, baseMat, 12, _s.Order + 1);
                var size = _s.ArcSize;
                float k = Mathf.Max(screen.x / size.x, screen.y / size.y);
                Vector2 V(Vector2 px) => screen / 2 + (px - size / 2) * k;
                for (int i = 0; i < _s.Arc.Count; i++)
                {
                    var v = _s.Arc[i];
                    _arcBg.Add(Layer.FromTexture(_overlay, v.Bg, baseMat, new Rect(Vector2.zero, size), V, 24, 36, _s.Order + 2 + i * 2));
                    _arcFg.Add(v.Fg != null ? Layer.FromTexture(_overlay, v.Fg, baseMat, v.FgPx, V, 4, 16, _s.Order + 12 + i) : null);
                    var quad = new Vector2[4];
                    for (int c = 0; c < 4; c++) quad[c] = V(v.Canvas[c]);
                    _arcQuads.Add(quad);
                }
                var last = _arcQuads[_arcQuads.Count - 1];
                _r = Rect.MinMaxRect(last[0].x, last[0].y, last[2].x, last[2].y);
                float ch2 = Mathf.Max(screen.y, screen.x / Mathf.Max(.1f, _s.BoardAspect)), cw2 = ch2 * _s.BoardAspect;
                _c = new Rect(screen.x / 2 - cw2 / 2, screen.y / 2 - ch2 / 2, cw2, ch2);
                // the picture's crop at the square-on view: the canvas's proportions, centred
                float ta = _s.Picture != null ? (float)_s.Picture.width / _s.Picture.height : 1, ca = _r.width / _r.height;
                _cropA3 = ta > ca ? new Rect((1 - ca / ta) / 2, 0, ca / ta, 1) : new Rect(0, (1 - ta / ca) / 2, 1, ta / ca);
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
            // the overlay rides on the camera at the scale of the home view
            float k = _s.Cam.orthographicSize / _homeSize;
            if (_overlay != null)
            {
                _overlay.localScale = new Vector3(k, k, 1);
                _overlay.localPosition = new Vector3(-_s.Screen.x / 2 * k, _s.Screen.y / 2 * k, 10);
            }
            if (!_raised && t >= .22f) Raise();
            if (_arcBg.Count > 0) { ArcStep(t); return; }
            FaceStep(t);
        }

        /* PM_76: the facing turn (homography) and the approach overlap: one move, no stop. */
        void FaceStep(float t)
        {
            float nearStart = _s.NearAt;
            if (t < nearStart + ArcNear + ArcHold)
            {
                _s.FadeUi?.Invoke(1 - Mathf.Clamp01(t / .22f));
                float f = Mathf.Clamp01(t / _s.FaceSeconds);
                FacePose(_s.FaceFromMotion ? BoosterFx74.OutCubic(f) : BoosterFx74.InOutCubic(f), BoosterFx74.InOutCubic(Mathf.Clamp01((t - nearStart) / ArcNear)));
                if (t >= nearStart + ArcNear && !_built) BuildLevel();
                return;
            }
            Tail(t, nearStart + ArcNear + ArcHold);
        }

        void FacePose(float ef, float e)
        {
            var q = _s.Quad; var r = Corners(_r);
            var to = new Vector2[4];
            for (int i = 0; i < 4; i++) to[i] = Vector2.Lerp(q[i], r[i], ef);
            var rect = LogLerp(_r, _c, e);
            var zoom = Similarity(_r.center, rect.center, rect.height / _r.height);
            var wall = Mul(zoom, Homography(q, to));
            const float wallAlpha = 1;   // PM_76: the wall never fades: the picture covers the screen by the end (no gap)
            foreach (var w in _walls) w.Warp(wall, wallAlpha);
            var zq = new Vector2[4];
            for (int c = 0; c < 4; c++) zq[c] = Apply(zoom, to[c]);
            PaintInto(zq, e, _s.Crop, 1f, ef < 1 ? q : null, wall);
            _backdrop.Alpha(1);
        }

        /* The painting: in the canvas quad zq (perspective-correct) and, with the approach e, the board's framing;
           its crop eases from crop0 to the whole picture. While a wall still turns (start != null) the picture
           follows the same homography as the wall. */
        void PaintInto(Vector2[] zq, float e, Rect crop0, float enlarge, Vector2[] start, double[] wall)
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
            if (start != null && e <= 0) _paint.Positions((x, y) => Apply(wall, CorridorTransition.Project(start, x, y)), crop, 1);
            else _paint.Positions((x, y) => CorridorTransition.Project(pq, x, y), crop, 1);
        }

        /* The pull back to the board (with its drawn cuts), then the HUD's last part. */
        void Tail(float t, float holdEnd)
        {
            if (!_built) BuildLevel();
            if (!_revealed) StartReveal();
            float b = (t - holdEnd) / Back;
            if (b < 1) { BackPhase(b); return; }
            BackPhase(1);
            float hud = (t - holdEnd - Back) / ArcHudTail;
            _s.Slide?.Invoke(Mathf.Lerp(.6f, 1, BoosterFx74.OutCubic(Mathf.Clamp01(hud))));
            if (hud >= 1) Finish();
        }

        void ArcStep(float t)
        {
            float nearStart = ArcSeconds * ArcNearAt, nearEnd = nearStart + ArcNear, holdEnd = nearEnd + ArcHold;
            if (t < holdEnd)
            {
                _s.FadeUi?.Invoke(1 - Mathf.Clamp01(t / .22f));
                ArcPose(BoosterFx74.InOutCubic(Mathf.Clamp01(t / ArcSeconds)), BoosterFx74.InOutCubic(Mathf.Clamp01((t - nearStart) / ArcNear)));
                if (t >= nearEnd && !_built) BuildLevel();
                return;
            }
            Tail(t, holdEnd);
        }


        /* p: the arc 0..1 (eased); e: the approach 0..1 (eased). */
        void ArcPose(float p, float e)
        {
            int n = _arcQuads.Count;
            float theta = p * (n - 1);
            int i = Mathf.Min(n - 2, Mathf.FloorToInt(theta));
            float u = theta - i;
            var qa = _arcQuads[i]; var qb = _arcQuads[i + 1];
            var qm = new Vector2[4];
            for (int c = 0; c < 4; c++) qm[c] = Vector2.Lerp(qa[c], qb[c], u);
            // the camera also moves in a little along the arc (10 %), about the canvas: no edge ever uncovered
            var mid = (qm[0] + qm[1] + qm[2] + qm[3]) / 4;
            float dolly = 1 + .1f * p * (1 - e);   // handed over to the approach: the cover framing ends exact
            var rect = LogLerp(_r, _c, e);
            float s = rect.height / _r.height;
            var zoom = Mul(Similarity(_r.center, rect.center, s), Similarity(mid, mid, dolly));
            var ha = Mul(zoom, Homography(qa, qm)); var hb = Mul(zoom, Homography(qb, qm));
            float w = Mathf.SmoothStep(0, 1, Mathf.InverseLerp(.15f, .85f, u));
            for (int k = 0; k < n; k++)
            {
                bool shown = k == i || k == i + 1;
                _arcBg[k].SetActive(shown);
                _arcFg[k]?.SetActive(shown);
            }
            const float wallAlpha = 1;   // PM_76: the wall never fades: the picture covers the screen by the end (no gap)
            _arcBg[i].Warp(ha, wallAlpha);
            _arcBg[i + 1].Warp(hb, w * wallAlpha);
            // the near foreground slides further than the wall (parallax) and crossfades
            float slide = -.08f * _s.Screen.x;
            _arcFg[i]?.Warp(Mul(Similarity(Vector2.zero, new Vector2(slide * u, 0), 1), ha), (1 - w) * wallAlpha);
            _arcFg[i + 1]?.Warp(Mul(Similarity(Vector2.zero, new Vector2(slide * (u - 1), 0), 1), hb), w * wallAlpha);
            // the painting: in the canvas (perspective) during the arc, then the board's framing; its crop eases from
            // the menu's to the square-on canvas's, then to the whole picture; a hair larger than the cut-out (no seam)
            var crop0 = new Rect(Vector2.Lerp(_s.Crop.position, _cropA3.position, p), Vector2.Lerp(_s.Crop.size, _cropA3.size, p));
            var zq = new Vector2[4];
            for (int c = 0; c < 4; c++) zq[c] = Apply(zoom, qm[c]);
            PaintInto(zq, e, crop0, 1.015f, null, null);
            _backdrop.Alpha(1);
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
        }

        void StartReveal()
        {
            _revealed = true;
            if (_board != null) _reveal = Reveal.Create(_board, _board.Image != null ? _board.Image : _s.Picture);
            if (_overlay != null) Destroy(_overlay.gameObject);
            _overlay = null;
            foreach (var w in _walls) w.Dispose();
            _walls.Clear();
            foreach (var w in _arcBg) w.Dispose();
            foreach (var w in _arcFg) w?.Dispose();
            _arcBg.Clear(); _arcFg.Clear();
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
            _s.Slide?.Invoke(Mathf.Clamp01((t - .55f) / .45f) * .6f);
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
            foreach (var w in _arcBg) w.Dispose();
            foreach (var w in _arcFg) w?.Dispose();
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

        sealed class Layer
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
                var lb = sp.bounds; var tr = sp.textureRect;
                // only the part near the screen (the corridor strip is many bays wide)
                Vector2 Layout(float u, float v)
                {
                    var w = sr.transform.TransformPoint(new Vector3(Mathf.Lerp(lb.min.x, lb.max.x, u), Mathf.Lerp(lb.max.y, lb.min.y, v), 0));
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
                        float u = Mathf.Lerp(u0, u1, (float)i / nx), v = (float)j / ny;
                        src.Add(Layout(u, v));
                        uv.Add(new Vector2((tr.x + u * tr.width) / tex.width, (tr.y + (1 - v) * tr.height) / tex.height));
                        if (i < nx && j < ny) { int s = j * (nx + 1) + i; tris.AddRange(new[] { s, s + 1, s + nx + 2, s, s + nx + 2, s + nx + 1 }); }
                    }
                l.Make(parent, "wall", new Material(sr.sharedMaterial) { mainTexture = tex }, src.ToArray(), uv.ToArray(), tris, order);
                return l;
            }

            /* A texture's pixel rect `px` (working pixels, y down) laid out by `toLayout`, as an nx x ny grid. */
            public static Layer FromTexture(Transform parent, Texture tex, Material baseMat, Rect px, Func<Vector2, Vector2> toLayout, int nx, int ny, int order)
            {
                var l = new Layer { _n = nx };
                var src = new List<Vector2>(); var uv = new List<Vector2>(); var tris = new List<int>();
                for (int j = 0; j <= ny; j++)
                    for (int i = 0; i <= nx; i++)
                    {
                        float u = (float)i / nx, v = (float)j / ny;
                        src.Add(toLayout(new Vector2(px.x + u * px.width, px.y + v * px.height)));
                        uv.Add(new Vector2(u, 1 - v));
                        if (i < nx && j < ny) { int s = j * (nx + 1) + i; tris.AddRange(new[] { s, s + 1, s + nx + 2, s, s + nx + 2, s + nx + 1 }); }
                    }
                l.Make(parent, "view", new Material(baseMat) { mainTexture = tex != null ? tex : Texture2D.whiteTexture }, src.ToArray(), uv.ToArray(), tris, order);
                return l;
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
            const float Width = 1.7f, Shadow = 3.4f, EdgeSeconds = .34f, Spread = .42f;
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
                float time = t * Back;
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
