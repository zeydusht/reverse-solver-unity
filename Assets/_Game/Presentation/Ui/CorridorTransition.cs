using System;
using System.Collections.Generic;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_MESAJ_24: after a won level, the camera slides along the gallery from this level's painting (bay 1) to
       the next one's (bay 2). The Task 18 video is a pure horizontal pan, so one still strip
       (Resources/corridor_strip, Tools/make_corridor_strip.py) slides instead of a video playing; the paintings
       are meshes on the strip's apertures (homography, 1.2 % larger so the frame covers their edge).
       A tap skips; `done` runs once, when the slide ends or is skipped, and the overlay then fades away. */
    public sealed class CorridorTransition : MonoBehaviour
    {
        public const float FadeOut = .3f, SkipAfter = .3f;
        /* PM_85 (Zeyd: "very good, but after 3 levels I wondered why so long"): the whole slide 1.5 s (&koridor=): the
           painting just won for .2 s (the fade-in, then a beat), then the slide; the dive goes on at once.
           PM_86 §1 ("it feels like a constant speed"): the slide is a bell of speed (Ease, smootherstep): slow off,
           fastest in the middle with a faint trail, gliding in to rest. Glide (PM_85's plateau) is kept for its test. */
        public static float Seconds = 1.5f;
        /* PM_88 §4 (Zeyd: "a little beat, tiny"): win -> next only, the painting just won stays .2 s more before the
           slide (&koridorbas=) and the next one .2 s before the dive (&koridorson=); the slide itself is the same.
           In both the camera breathes: a .5 % push in and back (sin^2, at rest on both ends). */
        public static float StartBeat = .2f, EndBeat = .2f;
        const float Breath = .005f;
        float _beatIn, _beatOut;
        const float Lead = .2f;
        static float FadeIn => Mathf.Min(.12f, Seconds * .08f);
        static float Hold => Mathf.Max(0, Lead - FadeIn);
        static float Slide => Mathf.Max(.3f, Seconds - Lead);
        const float ViewW = 780, ViewH = 1688, Period = 862.25f, Grow = 1.012f, Late = .25f, MaxCrop = 1.3f;
        // bay 1's inner corners at the first frame (strip px, y down): top-left, top-right, bottom-right, bottom-left
        static readonly Vector2[] Bay = { new Vector2(208.3f, 291.2f), new Vector2(585.6f, 332.9f), new Vector2(576.9f, 882.2f), new Vector2(196.2f, 865.7f) };

        Transform _slide;
        SpriteRenderer _strip;
        readonly SpriteRenderer[] _trail = new SpriteRenderer[2];
        ShapeView _backdrop;
        Material _baseMat;
        readonly List<(Mesh mesh, Material mat, float bornAt)> _paintings = new List<(Mesh, Material, float)>();
        int _order;
        float _start, _endAt = -1;
        Action _done;

        public bool Finished => _endAt >= 0;

        /* Smootherstep, the Task 18 easing (equal to the track to 4 decimals). */
        public static float Ease(float t) { t = Mathf.Clamp01(t); return t * t * t * (t * (t * 6 - 15) + 10); }

        /* PM_85: the slide's place at u (0..1): the speed rises softly over the first 30 %, holds, and falls softly to
           nought over the last 30 % (smoothstep ramps; the place is their integral, so the speed never jumps). */
        public static float Glide(float u, float rise = .3f, float fall = .3f)
        {
            u = Mathf.Clamp01(u);
            static float I(float x) => x * x * x - x * x * x * x / 2;   // the integral of smoothstep from 0 to x
            float total = 1 - rise / 2 - fall / 2, p;
            if (u < rise) p = rise * I(u / rise);
            else if (u < 1 - fall) p = rise / 2 + (u - rise);
            else p = rise / 2 + (1 - rise - fall) + fall * (.5f - I((1 - u) / fall));
            return p / total;
        }

        /* null when the strip is missing: the caller goes straight on. `next` may be null and come later (SetNext). */
        /* PM_89 §1: wonAspect / nextAspect (board width / height): each painting opens at its board's proportions in a
           frame the game draws (Frame89; the strip's own frames were taken off it, Tools/pm89_clear_frames.py) and
           shows the board's crop. 0: the old way (the picture centre-cropped to the bay's opening). */
        public static CorridorTransition Play(Vector2 screen, int order, Texture2D won, Texture2D next, Action done, bool handoff = false,
                                              float wonAspect = 0, float nextAspect = 0)
        {
            var sprite = Resources.Load<Sprite>("corridor_strip");
            if (sprite == null) return null;
            var c = new GameObject("Corridor").AddComponent<CorridorTransition>();
            c._framed = Frame89.On && wonAspect > 0 && nextAspect > 0;
            c._aspect[0] = wonAspect; c._aspect[1] = nextAspect;
            c._order = order; c._done = done; c._start = Time.unscaledTime; c._handoff = handoff;
            c._beatIn = handoff ? StartBeat : 0; c._beatOut = handoff ? EndBeat : 0;
            // cover-fit the 9:19.5 view; a wide window crops at most 23 % of its height (the painting stays whole)
            // and the strip, only one bay wide, gets dark sides
            float k = Mathf.Min(Mathf.Max(screen.x / ViewW, screen.y / ViewH), MaxCrop * screen.y / ViewH);
            c._backdrop = ShapeView.Create(c.transform, "backdrop", screen, 0, order - 1).Radius(0).Fill(new Vector4(.09f, .07f, .06f, 1));
            c._backdrop.transform.localPosition = Draw.W(screen / 2);
            c._slide = new GameObject("slide").transform;
            c._slide.SetParent(c.transform, false);
            c._slide.localScale = new Vector3(k, k, 1);
            c.Place(screen, k, 0);
            c._strip = new GameObject("strip").AddComponent<SpriteRenderer>();
            c._strip.transform.SetParent(c._slide, false);
            c._strip.sprite = sprite;
            c._strip.sortingOrder = order;
            c._strip.transform.localPosition = Draw.W(sprite.rect.width / 2, sprite.rect.height / 2);
            c._baseMat = c._strip.sharedMaterial;
            // PM_86 §1: at the top speed a faint trail (two copies of the strip a part of a frame behind, the paintings
            // stay sharp); same order as the strip, a hair nearer the camera
            for (int i = 0; i < c._trail.Length; i++)
            {
                var g = new GameObject("trail " + i).transform;
                g.SetParent(c.transform, false); g.localScale = new Vector3(k, k, 1);
                var r = new GameObject("strip").AddComponent<SpriteRenderer>();
                r.transform.SetParent(g, false); r.sprite = sprite; r.sortingOrder = order;
                r.transform.localPosition = Draw.W(sprite.rect.width / 2, sprite.rect.height / 2) + new Vector3(0, 0, -.01f * (i + 1));
                r.color = new Color(1, 1, 1, 0);
                c._trail[i] = r;
            }
            if (c._framed) { c.AddFrame(0); c.AddFrame(1); }
            c.AddPainting(won, 0, false);
            c.SetNext(next);
            c._screen = screen; c._k = k;
            c.Update();
            return c;
        }

        Vector2 _screen;
        float _k;
        bool _handoff;
        Texture2D _next;
        bool _framed;
        readonly float[] _aspect = new float[2];
        readonly MeshRenderer[] _frame = new MeshRenderer[2];
        /* PM_89: the bay's opening as a flat rect (its width 1, its height 1.45, as the strip was drawn) and the frames
           the game draws there: OpeningHeight units tall at the board's proportions, the border FrameBorder wide. */
        const float FlatH = 1.45f, OpeningHeight = 1.1f, FrameBorder = .15f;
        public MeshRenderer NextFrame => _frame[1];

        /* The flat bay (0..1 x 0..FlatH) onto bay k's opening in strip px (the strip's own perspective). */
        static double[] BayH(int k)
        {
            var dst = new Vector2[4];
            for (int i = 0; i < 4; i++) dst[i] = Bay[i] + new Vector2(k * Period, 0);
            return Dive75.Homography(new[] { Vector2.zero, new Vector2(1, 0), new Vector2(1, FlatH), new Vector2(0, FlatH) }, dst);
        }

        /* Bay k's opening (strip px: TL, TR, BR, BL) at its board's proportions. */
        Vector2[] Opening89(int k)
        {
            var h = BayH(k); float w = _aspect[k] * OpeningHeight, x0 = .5f - w / 2, y0 = FlatH / 2 - OpeningHeight / 2;
            return new[] { Dive75.Apply(h, new Vector2(x0, y0)), Dive75.Apply(h, new Vector2(x0 + w, y0)),
                           Dive75.Apply(h, new Vector2(x0 + w, y0 + OpeningHeight)), Dive75.Apply(h, new Vector2(x0, y0 + OpeningHeight)) };
        }

        void AddFrame(int k)
        {
            if (Frame89.Texture == null || _baseMat == null) return;
            var open = Frame89.Opening(_aspect[k], OpeningHeight, FrameBorder, out float ppu);
            var pos = new List<Vector2>(); var uv = new List<Vector2>(); var tris = new List<int>();
            Frame89.Build(open, pos, uv, tris);
            var h = BayH(k); float x0 = .5f - _aspect[k] * OpeningHeight / 2, y0 = FlatH / 2 - OpeningHeight / 2;
            var verts = new List<Vector3>();
            foreach (var p in pos) { var s = Dive75.Apply(h, new Vector2(x0 + p.x / ppu, y0 + p.y / ppu)); verts.Add(Draw.W(s.x, s.y)); }
            var m = new Mesh { name = "corridor frame" };
            m.SetVertices(verts); m.SetUVs(0, uv); m.SetTriangles(tris, 0);
            var cols = new Color[verts.Count]; for (int i = 0; i < cols.Length; i++) cols[i] = Color.white;
            m.colors = cols;
            m.RecalculateBounds();
            var go = new GameObject("frame " + k);
            go.transform.SetParent(_slide, false);
            go.AddComponent<MeshFilter>().sharedMesh = m;
            var r = go.AddComponent<MeshRenderer>();
            var mat = new Material(_baseMat) { mainTexture = Frame89.Texture };
            r.sharedMaterial = mat; r.sortingOrder = _order + 2;
            _frame[k] = r;
            _paintings.Add((m, mat, -1));   // faded with the paintings
        }

        /* PM_75: at the end of the slide the dive takes over: the strip, the next painting and where it is now. */
        public SpriteRenderer Strip => _strip;
        public Texture2D NextTexture => _next;
        /* The next painting's crop (uv), as Mesh() centre-crops it to its aperture. */
        public Rect NextCrop
        {
            get
            {
                if (_next == null) return new Rect(0, 0, 1, 1);
                if (_framed) return LevelImages.Crop(_next, _aspect[1]);
                var q = BayQuad(Period);
                float aw = ((q[1] - q[0]).magnitude + (q[2] - q[3]).magnitude) / 2, ah = ((q[3] - q[0]).magnitude + (q[2] - q[1]).magnitude) / 2;
                float ta = (float)_next.width / _next.height, a = aw / ah;
                float uw = ta > a ? a / ta : 1, vh = ta > a ? 1 : ta / a;
                return new Rect((1 - uw) / 2, (1 - vh) / 2, uw, vh);
            }
        }
        /* The next painting's aperture on the screen now, layout points (TL, TR, BR, BL). */
        public Vector2[] NextQuad()
        {
            var q = _framed ? Opening89(1) : BayQuad(Period);
            var r = new Vector2[4];
            for (int i = 0; i < 4; i++)
            {
                var w = _slide.TransformPoint(Draw.W(q[i]));
                r[i] = new Vector2(w.x, -w.y);
            }
            return r;
        }
        static Vector2[] BayQuad(float dx)
        {
            var q = new Vector2[4];
            Vector2 mid = Vector2.zero;
            foreach (var p in Bay) mid += p / 4;
            for (int i = 0; i < 4; i++) q[i] = mid + (Bay[i] - mid) * Grow + new Vector2(dx, 0);
            return q;
        }
        public void DestroyNow() { if (this != null) Destroy(gameObject); }

        /* The view's left edge at strip x = offset, centred on the screen. */
        void Place(Vector2 screen, float k, float offset) =>
            _slide.localPosition = Draw.W(screen.x / 2 - (offset + ViewW / 2) * k, screen.y / 2 - ViewH / 2 * k);

        public void SetNext(Texture2D next)
        {
            if (next == null || this == null || _next != null) return;
            _next = next;
            AddPainting(next, Period, Time.unscaledTime - _start > .05f);   // late: a short fade over the dark placeholder
        }

        /* Skip: the next screen comes now; only a new touch, not the one that pressed Next or Menu (PM_MESAJ_24B). */
        public void Skip() { if (!Finished && Time.unscaledTime - _start >= SkipAfter) End(); }

        int _frames, _slow;
        float _maxDt;

        void End()
        {
            Debug.Log($"[koridor] {Time.unscaledTime - _start:0.00} s (beats {_beatIn:0.00} + {_beatOut:0.00} s): frames {_frames}, over 20 ms {_slow} (max {_maxDt * 1000:0} ms)");   // PM_85
            // PM_75: a skipped slide jumps to its end, so the dive starts from the next painting
            if (_handoff) { _slide.localScale = new Vector3(_k, _k, 1); Place(_screen, _k, Period); }
            _endAt = Time.unscaledTime;
            var d = _done; _done = null;
            d?.Invoke();
        }

        void AddPainting(Texture2D tex, float dx, bool fade)
        {
            if (tex == null || _baseMat == null) return;
            var go = new GameObject("painting");
            go.transform.SetParent(_slide, false);
            var mat = new Material(_baseMat) { mainTexture = tex };
            var r = go.AddComponent<MeshRenderer>();
            r.sharedMaterial = mat;
            r.sortingOrder = _order + 1;
            var mesh = _framed ? Mesh89(tex, dx > 0 ? 1 : 0) : Mesh(tex, dx);
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            _paintings.Add((mesh, mat, fade ? Time.unscaledTime : -1));
        }

        /* An 8x8 grid through the homography of the unit square onto the aperture, the picture centre-cropped to it. */
        static Mesh Mesh(Texture2D tex, float dx)
        {
            var q = new Vector2[4];
            Vector2 mid = Vector2.zero;
            foreach (var p in Bay) mid += p / 4;
            for (int i = 0; i < 4; i++) q[i] = mid + (Bay[i] - mid) * Grow + new Vector2(dx, 0);
            float aw = ((q[1] - q[0]).magnitude + (q[2] - q[3]).magnitude) / 2, ah = ((q[3] - q[0]).magnitude + (q[2] - q[1]).magnitude) / 2;
            float ta = (float)tex.width / tex.height, a = aw / ah;
            float uw = ta > a ? a / ta : 1, vh = ta > a ? 1 : ta / a;
            const int N = 8;
            var verts = new List<Vector3>(); var uv = new List<Vector2>(); var tris = new List<int>();
            for (int j = 0; j <= N; j++)
                for (int i = 0; i <= N; i++)
                {
                    float u = (float)i / N, v = (float)j / N;
                    var p = Project(q, u, v);
                    verts.Add(Draw.W(p.x, p.y));
                    uv.Add(new Vector2((1 - uw) / 2 + u * uw, (1 - vh) / 2 + (1 - v) * vh));
                    if (i < N && j < N) { int s = j * (N + 1) + i; tris.AddRange(new[] { s, s + 1, s + N + 2, s, s + N + 2, s + N + 1 }); }
                }
            var m = new Mesh { name = "corridor painting" };
            m.SetVertices(verts); m.SetUVs(0, uv); m.SetTriangles(tris, 0);
            m.SetColors(new List<Color>(new Color[verts.Count]));
            m.RecalculateBounds();
            return m;
        }

        /* PM_89: the painting in bay k's opening at its board's proportions, the board's crop (the same 8 x 8 grid). */
        Mesh Mesh89(Texture2D tex, int k)
        {
            var q = Opening89(k); var cr = LevelImages.Crop(tex, _aspect[k]);
            const int N = 8;
            var verts = new List<Vector3>(); var uv = new List<Vector2>(); var tris = new List<int>();
            for (int j = 0; j <= N; j++)
                for (int i = 0; i <= N; i++)
                {
                    float u = (float)i / N, v = (float)j / N;
                    var p = Project(q, u, v);
                    verts.Add(Draw.W(p.x, p.y));
                    uv.Add(new Vector2(cr.x + u * cr.width, cr.y + (1 - v) * cr.height));
                    if (i < N && j < N) { int s = j * (N + 1) + i; tris.AddRange(new[] { s, s + 1, s + N + 2, s, s + N + 2, s + N + 1 }); }
                }
            var m = new Mesh { name = "corridor painting" };
            m.SetVertices(verts); m.SetUVs(0, uv); m.SetTriangles(tris, 0);
            m.SetColors(new List<Color>(new Color[verts.Count]));
            m.RecalculateBounds();
            return m;
        }

        /* Unit square -> quad (Heckbert): (0,0) top-left, (1,0) top-right, (1,1) bottom-right, (0,1) bottom-left. */
        public static Vector2 Project(Vector2[] q, float u, float v)
        {
            float dx1 = q[1].x - q[2].x, dx2 = q[3].x - q[2].x, dx3 = q[0].x - q[1].x + q[2].x - q[3].x;
            float dy1 = q[1].y - q[2].y, dy2 = q[3].y - q[2].y, dy3 = q[0].y - q[1].y + q[2].y - q[3].y;
            float den = dx1 * dy2 - dx2 * dy1;
            float g = (dx3 * dy2 - dx2 * dy3) / den, h = (dx1 * dy3 - dx3 * dy1) / den;
            float a = q[1].x - q[0].x + g * q[1].x, b = q[3].x - q[0].x + h * q[3].x;
            float d = q[1].y - q[0].y + g * q[1].y, e = q[3].y - q[0].y + h * q[3].y;
            float w = g * u + h * v + 1;
            return new Vector2((a * u + b * v + q[0].x) / w, (d * u + e * v + q[0].y) / w);
        }

        void Update()
        {
            float now = Time.unscaledTime, t = now - _start;
            if (!Finished && t > 0) { _frames++; if (Time.unscaledDeltaTime > .02f) _slow++; _maxDt = Mathf.Max(_maxDt, Time.unscaledDeltaTime); }
            float alpha = Finished ? 1 - (now - _endAt) / FadeOut : Mathf.Clamp01(t / FadeIn);
            if (Finished && alpha <= 0) { Destroy(gameObject); return; }
            if (!Finished)
            {
                // PM_86 §1: a bell of speed (smootherstep: slow off, fastest in the middle, gliding in to rest)
                float lead = FadeIn + Hold + _beatIn, u = (t - lead) / Slide;
                // PM_88 §4: the beats' breath (a scale about the screen's middle, 1 again at each beat's end)
                float tail = t - lead - Slide, breath = 0;
                if (_beatIn > 0 && t > FadeIn + Hold && u < 0) breath = Pulse((t - FadeIn - Hold) / _beatIn);
                else if (_beatOut > 0 && tail > 0) breath = Pulse(tail / _beatOut);
                float kk = _k * (1 + Breath * breath);
                _slide.localScale = new Vector3(kk, kk, 1);
                Place(_screen, kk, Period * Ease(u));
                float speed = u > 0 && u < 1 ? 30 * u * u * (1 - u) * (1 - u) / 1.875f : 0;   // 0..1, 1 at the top
                float frame = Period * 1.875f / Slide / 60f;                                  // strip px per 60 Hz frame at the top
                for (int i = 0; i < _trail.Length; i++)
                {
                    float lag = frame * speed * (i + 1) / (_trail.Length + 1);
                    _trail[i].transform.parent.localPosition = Draw.W(_screen.x / 2 - (Period * Ease(u) - lag + ViewW / 2) * _k, _screen.y / 2 - ViewH / 2 * _k);
                    _trail[i].color = new Color(1, 1, 1, .22f * Mathf.Clamp01((speed - .5f) / .5f) * alpha);
                }
            }
            else foreach (var r in _trail) r.color = new Color(1, 1, 1, 0);
            _strip.color = new Color(1, 1, 1, alpha);
            _backdrop.Alpha(alpha);
            foreach (var (mesh, _, bornAt) in _paintings)
            {
                float a = alpha * (bornAt < 0 ? 1 : Mathf.Clamp01((now - bornAt) / Late));
                var cols = new Color[mesh.vertexCount];
                for (int i = 0; i < cols.Length; i++) cols[i] = new Color(1, 1, 1, a);
                mesh.colors = cols;
            }
            if (!Finished && t >= FadeIn + Hold + _beatIn + Slide + _beatOut) End();
        }

        static float Pulse(float x) { x = Mathf.Clamp01(x); float s = Mathf.Sin(Mathf.PI * x); return s * s; }

        void OnDestroy()
        {
            foreach (var (mesh, mat, _) in _paintings) { Destroy(mesh); Destroy(mat); }
        }
    }
}
