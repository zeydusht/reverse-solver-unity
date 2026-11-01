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
        public const float FadeIn = .25f, Hold = .35f, Slide = 3f, FadeOut = .3f;
        const float ViewW = 780, ViewH = 1688, Period = 862.25f, Grow = 1.012f, Late = .25f, MaxCrop = 1.3f;
        // bay 1's inner corners at the first frame (strip px, y down): top-left, top-right, bottom-right, bottom-left
        static readonly Vector2[] Bay = { new Vector2(208.3f, 291.2f), new Vector2(585.6f, 332.9f), new Vector2(576.9f, 882.2f), new Vector2(196.2f, 865.7f) };

        Transform _slide;
        SpriteRenderer _strip;
        ShapeView _backdrop;
        Material _baseMat;
        readonly List<(Mesh mesh, Material mat, float bornAt)> _paintings = new List<(Mesh, Material, float)>();
        int _order;
        float _start, _endAt = -1;
        Action _done;

        public bool Finished => _endAt >= 0;

        /* Smootherstep, the Task 18 easing (equal to the track to 4 decimals). */
        public static float Ease(float t) { t = Mathf.Clamp01(t); return t * t * t * (t * (t * 6 - 15) + 10); }

        /* null when the strip is missing: the caller goes straight on. `next` may be null and come later (SetNext). */
        public static CorridorTransition Play(Vector2 screen, int order, Texture2D won, Texture2D next, Action done)
        {
            var sprite = Resources.Load<Sprite>("corridor_strip");
            if (sprite == null) return null;
            var c = new GameObject("Corridor").AddComponent<CorridorTransition>();
            c._order = order; c._done = done; c._start = Time.unscaledTime;
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
            c.AddPainting(won, 0, false);
            c.SetNext(next);
            c._screen = screen; c._k = k;
            c.Update();
            return c;
        }

        Vector2 _screen;
        float _k;

        /* The view's left edge at strip x = offset, centred on the screen. */
        void Place(Vector2 screen, float k, float offset) =>
            _slide.localPosition = Draw.W(screen.x / 2 - (offset + ViewW / 2) * k, screen.y / 2 - ViewH / 2 * k);

        public void SetNext(Texture2D next)
        {
            if (next == null || this == null || _paintings.Count > 1) return;
            AddPainting(next, Period, Time.unscaledTime - _start > .05f);   // late: a short fade over the dark placeholder
        }

        /* Skip: the next screen comes now. */
        public void Skip() { if (!Finished) End(); }

        void End()
        {
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
            var mesh = Mesh(tex, dx);
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
            float alpha = Finished ? 1 - (now - _endAt) / FadeOut : Mathf.Clamp01(t / FadeIn);
            if (Finished && alpha <= 0) { Destroy(gameObject); return; }
            if (!Finished) Place(_screen, _k, Period * Ease((t - FadeIn - Hold) / Slide));
            _strip.color = new Color(1, 1, 1, alpha);
            _backdrop.Alpha(alpha);
            foreach (var (mesh, _, bornAt) in _paintings)
            {
                float a = alpha * (bornAt < 0 ? 1 : Mathf.Clamp01((now - bornAt) / Late));
                var cols = new Color[mesh.vertexCount];
                for (int i = 0; i < cols.Length; i++) cols[i] = new Color(1, 1, 1, a);
                mesh.colors = cols;
            }
            if (!Finished && t >= FadeIn + Hold + Slide) End();
        }

        void OnDestroy()
        {
            foreach (var (mesh, mat, _) in _paintings) { Destroy(mesh); Destroy(mat); }
        }
    }
}
