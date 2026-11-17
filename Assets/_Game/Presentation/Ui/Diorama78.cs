using System.Collections.Generic;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_78 (Zeyd: "the arc is right — first right, then forward — but not in that picture: in our own menu picture"):
       the ORIGINAL menu picture (menu_bg.png) as a 3D diorama, flown along the path Zeyd drew in his camera path
       studio (docs/art/pm78/zeyd_yol.json): 3.6 s through his four keys without stopping, the head always on the
       painting's centre.
       The menu's own camera is solved from its painting (Tools/pm78_geom.py): the canvas is a rectangle on the wall,
       its top and bottom edges meet at the wall's vanishing point, the verticals stay upright (a shifted lens, the
       principal point on the horizon). The start angle is Zeyd's first key; his path is scaled about the painting
       so that key sits on the menu's camera (the angles are his).
       Every layer keeps menu_bg's own pixel grid (the "plate", extended past the picture's edges by Codex where the
       path sees more; Tools/pm78_import.py): each pixel's 3D point is where the start camera's ray through it meets
       the layer's plane. So the first frame redraws the menu picture pixel for pixel, and every later frame is that
       same room from Zeyd's path. Layers: the wall (z 0) and the floor (y 0) meet on the wall's foot; the column,
       its banner and its sconce stand a little in front of the wall.
       Units: the canvas is 1 wide. x right along the wall, y up (floor 0), z out of the wall towards the camera. */
    public sealed class Diorama78
    {
        public sealed class Front
        {
            public string Name;
            public Texture Tex;
            public Rect Px;          // its rect in the picture (px, y down)
            public float Depth;      // in front of the wall
        }

        public sealed class Art
        {
            public Texture Room;                                   // the plate: the wall and the floor
            public readonly List<Front> Fronts = new List<Front>();
            public Vector2 PlateSize = new Vector2(1408, 1728), ImageOrigin = new Vector2(0, 128), ImageSize = new Vector2(1024, 1536);
            public float F = 1954.8f, Cx = 512.8f, Cy = 1005.2f;   // the start camera (picture px)
            public float StartDeg = 30.19f, D = 5.393f, H = .385f, Yc = 1.57f;
            public Vector2[] CanvasPx = { new Vector2(349, 296), new Vector2(662, 359), new Vector2(663, 832), new Vector2(348, 815) };
            public Vector2[] Keys = { new Vector2(-1.454f, 2.499f), new Vector2(-1.004f, 2.517f), new Vector2(-.474f, 2.405f), new Vector2(-.024f, 2.122f) };
            public float[] Durs = { 0, 1.2f, 1.2f, 1.2f };
            public float Seconds = 3.6f;
        }

        readonly Art _a;
        readonly Vector2 _screen, _imgCentre, _startMid, _startPaint;
        readonly float _k, _f, _scale;
        readonly Vector3 _cam0, _d0, _r0;
        readonly Vector3[] _canvas = new Vector3[4];

        /* imgCentre: where the picture's centre is (layout points); k: layout points per picture px (the menu's
           cover fit). */
        public Diorama78(Art art, Vector2 screen, Vector2 imgCentre, float k)
        {
            _a = art; _screen = screen; _imgCentre = imgCentre; _k = k;
            _f = art.F * k;
            float th = art.StartDeg * Mathf.Deg2Rad;
            _d0 = new Vector3(Mathf.Sin(th), 0, -Mathf.Cos(th)); _r0 = new Vector3(Mathf.Cos(th), 0, Mathf.Sin(th));
            _cam0 = new Vector3(-art.D * Mathf.Sin(th), art.H, art.D * Mathf.Cos(th));
            _scale = art.D / art.Keys[0].magnitude;   // W per metre: Zeyd's first key on the menu's camera
            _startMid = ImageToLayout(new Vector2(art.Cx, art.Cy));
            for (int i = 0; i < 4; i++) _canvas[i] = OnPlaneZ(art.CanvasPx[i], 0);
            _startPaint = Start(new Vector3(0, art.Yc, 0));
        }

        public float Seconds => _a.Seconds;
        /* For the log: the start angle, and how far Zeyd's path was scaled. */
        public float StartAngle => _a.StartDeg;
        public float Scale => _scale;

        public Vector2 ImageToLayout(Vector2 px) => _imgCentre + (px - _a.ImageSize / 2) * _k;

        // ---- the start camera (picture px) ----

        Vector3 Dir0(Vector2 px) => _d0 + _r0 * ((px.x - _a.Cx) / _a.F) + Vector3.up * (-(px.y - _a.Cy) / _a.F);

        Vector3 OnPlaneZ(Vector2 px, float z)
        {
            var v = Dir0(px);
            return _cam0 + v * ((z - _cam0.z) / v.z);
        }

        Vector3 OnFloor(Vector2 px)
        {
            var v = Dir0(px);
            return _cam0 + v * (-_cam0.y / Mathf.Min(-1e-5f, v.y));
        }

        /* A 3D point in the start view, in picture px. */
        Vector2 ToImage(Vector3 p)
        {
            var v = p - _cam0;
            float z = Mathf.Max(1e-4f, Vector3.Dot(v, _d0));
            return new Vector2(_a.Cx + _a.F * Vector3.Dot(v, _r0) / z, _a.Cy - _a.F * v.y / z);
        }

        Vector2 Start(Vector3 p) => ImageToLayout(ToImage(p));

        /* The wall's foot (y 0, z 0) in the picture: a straight line. */
        float FootY(float x)
        {
            var a = ToImage(new Vector3(-2, 0, 0)); var b = ToImage(new Vector3(2, 0, 0));
            return a.y + (x - a.x) * (b.y - a.y) / (b.x - a.x);
        }

        // ---- Zeyd's path ----

        /* o: 0..1 along the path after the global ease (the studio's tau / total): the camera's (x, z) in metres.
           Centripetal Catmull-Rom through his keys, each segment its own share of the time. */
        Vector2 PathAt(float o)
        {
            var k = _a.Keys; var dur = _a.Durs;
            float total = 0;
            for (int j = 1; j < k.Length; j++) total += dur[j];
            float tau = Mathf.Clamp01(o) * total; int i = 1;
            while (i < k.Length - 1 && tau > dur[i]) { tau -= dur[i]; i++; }
            float u = Mathf.Clamp01(tau / Mathf.Max(1e-4f, dur[i]));
            var a = k[i - 1]; var b = k[i];
            var p0 = i >= 2 ? k[i - 2] : 2 * a - b;
            var p3 = i + 1 < k.Length ? k[i + 1] : 2 * b - a;
            return Centripetal(p0, a, b, p3, u);
        }

        static Vector2 Centripetal(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float u)
        {
            float T(float ti, Vector2 a, Vector2 b) => ti + Mathf.Sqrt(Mathf.Max(1e-6f, (b - a).magnitude));
            float t0 = 0, t1 = T(t0, p0, p1), t2 = T(t1, p1, p2), t3 = T(t2, p2, p3);
            float t = Mathf.Lerp(t1, t2, u);
            Vector2 L(Vector2 a, Vector2 b, float ta, float tb) => a * ((tb - t) / (tb - ta)) + b * ((t - ta) / (tb - ta));
            var a1 = L(p0, p1, t0, t1); var a2 = L(p1, p2, t1, t2); var a3 = L(p2, p3, t2, t3);
            return L(L(a1, a2, t0, t2), L(a2, a3, t1, t3), t1, t2);
        }

        struct Cam { public Vector3 Pos, D, R; public Vector2 Mid; }

        /* The camera at o (0 the menu's view .. 1 Zeyd's last key): the head on the painting's centre (from the
           position, no angle in-betweening); the lens shift takes the painting's centre from where the menu shows it
           to the screen's centre. */
        Cam CameraAt(float o)
        {
            var p = PathAt(o) * _scale;
            float th = Mathf.Atan2(-p.x, p.y), dist = p.magnitude;
            float paintY = Mathf.Lerp(_startPaint.y, _screen.y / 2, o);
            return new Cam
            {
                Pos = new Vector3(p.x, _a.H, p.y), D = new Vector3(Mathf.Sin(th), 0, -Mathf.Cos(th)), R = new Vector3(Mathf.Cos(th), 0, Mathf.Sin(th)),
                Mid = new Vector2(Mathf.Lerp(_startMid.x, _screen.x / 2, o), paintY + _f * (_a.Yc - _a.H) / dist)
            };
        }

        Vector2 Project(in Cam c, Vector3 p)
        {
            var v = p - c.Pos;
            float z = Mathf.Max(.05f, Vector3.Dot(v, c.D));
            return c.Mid + new Vector2(_f * Vector3.Dot(v, c.R) / z, -_f * v.y / z);
        }

        public Vector2 Project(float o, Vector3 p) => Project(CameraAt(o), p);

        /* The camera's position (W) and heading (deg) at o, for the log. */
        public string Describe(float o)
        {
            var c = CameraAt(o);
            return $"({c.Pos.x:0.000}, {c.Pos.z:0.000}) {Mathf.Atan2(c.D.x, -c.D.z) * Mathf.Rad2Deg:0.0} deg";
        }

        public Vector2[] CanvasAt(float o)
        {
            var c = CameraAt(o);
            return new[] { Project(c, _canvas[0]), Project(c, _canvas[1]), Project(c, _canvas[2]), Project(c, _canvas[3]) };
        }

        /* The painting in the canvas: exactly the menu's opening at the start (the menu draws it there too). */
        public Vector2[] PictureAt(float o) => CanvasAt(o);

        // ---- what to build ----

        /* What the camera sees of the wall and the floor along the whole path, in picture px: the screen's edges
           cast onto the two planes, with a margin, inside the plate. Only that much is built, so no grid point comes
           near the camera and no screen edge ever shows past a layer (Tools/gap_check.py). */
        void Seen(out Rect wall, out float floorBottom)
        {
            float x0 = 1e9f, x1 = -1e9f, y0 = 1e9f, y1 = -1e9f;
            for (int s = 0; s <= 24; s++)
            {
                var c = CameraAt(s / 24f);
                for (int e = 0; e < 4; e++)
                    for (int i = 0; i <= 32; i++)
                    {
                        float a = i / 32f;
                        var sp = e == 0 ? new Vector2(a * _screen.x, 0) : e == 1 ? new Vector2(a * _screen.x, _screen.y)
                               : e == 2 ? new Vector2(0, a * _screen.y) : new Vector2(_screen.x, a * _screen.y);
                        var d = sp - c.Mid;
                        var dir = c.D * _f + c.R * d.x - Vector3.up * d.y;
                        Vector3 hit;
                        if (dir.y < -1e-5f && (hit = c.Pos + dir * (-c.Pos.y / dir.y)).z > 0) { }
                        else if (dir.z < -1e-5f) hit = c.Pos + dir * (-c.Pos.z / dir.z);
                        else continue;
                        var px = ToImage(hit);
                        x0 = Mathf.Min(x0, px.x); x1 = Mathf.Max(x1, px.x); y0 = Mathf.Min(y0, px.y); y1 = Mathf.Max(y1, px.y);
                    }
            }
            const float margin = 24;
            var lo = -_a.ImageOrigin; var hi = _a.PlateSize - _a.ImageOrigin;
            wall = Rect.MinMaxRect(Mathf.Max(lo.x, x0 - margin), Mathf.Max(lo.y, y0 - margin), Mathf.Min(hi.x, x1 + margin), 0);
            floorBottom = Mathf.Min(hi.y, y1 + margin);
        }

        /* The wall and the floor (one plate, meeting on the wall's foot), back to front, at `order`. */
        internal void Build(Transform parent, Material mat, int order, List<Dive75.Layer> layers)
        {
            Seen(out var wall, out float bottom);
            float x0 = wall.xMin, x1 = wall.xMax, top = wall.yMin;
            Vector2 Plate(Vector2 px) => px + _a.ImageOrigin;
            layers.Add(Dive75.Layer.FromGrid(parent, "wall", _a.Room, mat, _a.PlateSize,
                (u, v) => { float x = Mathf.Lerp(x0, x1, u); return Plate(new Vector2(x, Mathf.Lerp(top, FootY(x), v))); },
                px => OnPlaneZ(px - _a.ImageOrigin, 0), 40, 48, order));
            layers.Add(Dive75.Layer.FromGrid(parent, "floor", _a.Room, mat, _a.PlateSize,
                (u, v) => { float x = Mathf.Lerp(x0, x1, u); return Plate(new Vector2(x, Mathf.Lerp(FootY(x), bottom, v))); },
                px => OnFloor(px - _a.ImageOrigin), 40, 20, order));
        }

        /* The column, its banner and its sconce: each a cut-out on its own plane in front of the wall. */
        internal void BuildFronts(Transform parent, Material mat, int order, List<Dive75.Layer> layers)
        {
            foreach (var fr in _a.Fronts)
            {
                if (fr.Tex == null) continue;
                var r = fr.Px; float depth = fr.Depth;
                int ny = Mathf.Clamp(Mathf.RoundToInt(r.height / 40), 2, 24);
                layers.Add(Dive75.Layer.FromGrid(parent, fr.Name, fr.Tex, mat, r.size,
                    (u, v) => new Vector2(u * r.width, v * r.height),
                    px => OnPlaneZ(px + r.min, depth), 4, ny, order++));
            }
        }

        internal void Pose(float o, double[] post, List<Dive75.Layer> layers, float alpha)
        {
            var c = CameraAt(o);
            foreach (var l in layers) l.Project(p => Project(c, p), post, alpha);
        }
    }
}
