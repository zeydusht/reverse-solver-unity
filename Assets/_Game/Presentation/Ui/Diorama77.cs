using System.Collections.Generic;
using UnityEngine;

namespace ReverseSolver.Presentation
{
    /* PM_77 (Zeyd: "a few drawings in between never felt like turning round"): the menu's room as a 3D diorama of
       flat layers drawn square on (Codex, docs/art/pm77/CODEX_PM77.md): the wall (with the painting's frame, its canvas
       cut out) standing at z = 0, the floor lying at y = 0 in front of it, one free column nearer the camera. The camera
       really travels on a circle round the painting's centre, from the menu's side view (38°) to square on, always
       looking at the painting; every grid point of every layer is projected in perspective each frame, so perspective
       and parallax are right in every frame and no in-between drawing is needed.
       Units: the canvas is 1 high; x right along the wall, y up, z from the camera towards the wall. */
    public sealed class Diorama77
    {
        public sealed class Art
        {
            public Texture Wall, Floor, Column;
            public Vector2 WallPx = new Vector2(4800, 1600), FloorPx = new Vector2(4800, 1200), ColumnPx = new Vector2(480, 2400);
            public Rect CanvasPx = Rect.MinMaxRect(2244, 974, 2556, 1374);   // in the wall, px (y down)
            public float PxPerUnit = 400, FloorPxPerUnit = 400, ColumnPxPerUnit = 400;   // each layer's own scale
            public float ColumnZ = -1.0f, ColumnRightAt = 52;   // the column stands 1 in front of the wall; its right edge at x 52 pt at the start
        }

        public const float StartDeg = 35, Radius = 3.6f, FovDeg = 48;

        readonly Art _a;
        readonly Vector2 _screen, _startMid, _endMid;
        readonly Vector3 _target;
        readonly float _f;
        float _columnX, _startDeg = StartDeg, _radius = Radius;

        /* The start angle and the camera's distance (fitted to the menu's painting), for the log. */
        public float StartAngle => _startDeg;
        public float Distance => _radius;

        public Diorama77(Art art, Vector2 screen, Vector2[] menuQuad)
        {
            _a = art; _screen = screen;
            _target = new Vector3(0, (art.WallPx.y - art.CanvasPx.center.y) / art.PxPerUnit, 0);
            _f = screen.y / 2 / Mathf.Tan(FovDeg * Mathf.Deg2Rad / 2);
            // the painting's centre starts where the menu shows it and ends centred across (same height)
            _startMid = menuQuad != null ? (menuQuad[0] + menuQuad[1] + menuQuad[2] + menuQuad[3]) / 4 : screen / 2;
            _endMid = new Vector2(screen.x / 2, _startMid.y);
            // the menu's room is the diorama's start view (Screens.Menu.UseDiorama): the painting as tall as the menu's
            // opening and in its place, seen from StartDeg (the menu's drawing is nearly square on, ~20 deg: too
            // little turn for an orbit)
            if (menuQuad != null) FitRadius(((menuQuad[3] - menuQuad[0]).magnitude + (menuQuad[2] - menuQuad[1]).magnitude) / 2);
            // the column's place along the wall: its right edge at ColumnRightAt pt in the start view (as the menu's)
            float half = art.ColumnPx.x / 2 / art.ColumnPxPerUnit, lo = -6, hi = 0;
            for (int i = 0; i < 40; i++)
            {
                float mid = (lo + hi) / 2;
                float x = Project(0, new Vector3(mid + half, _target.y, art.ColumnZ)).x;
                if (x < art.ColumnRightAt) lo = mid; else hi = mid;
            }
            _columnX = (lo + hi) / 2;
        }

        Vector3 Wall(Vector2 px) => new Vector3((px.x - _a.CanvasPx.center.x) / _a.PxPerUnit, (_a.WallPx.y - px.y) / _a.PxPerUnit, 0);
        Vector3 Floor(Vector2 px) => new Vector3((px.x - _a.FloorPx.x / 2) / _a.FloorPxPerUnit, 0, -px.y / _a.FloorPxPerUnit);
        Vector3 Column(Vector2 px) => new Vector3(_columnX + (px.x - _a.ColumnPx.x / 2) / _a.ColumnPxPerUnit, (_a.ColumnPx.y - px.y) / _a.ColumnPxPerUnit, _a.ColumnZ);

        /* o: 0 the start (side) .. 1 square on. Layout points (y down). */
        public Vector2 Project(float o, Vector3 p)
        {
            Camera(o, out var cam, out var f, out var r, out var u, out var mid);
            var v = p - cam;
            float z = Mathf.Max(.08f, Vector3.Dot(v, f));
            return mid + new Vector2(_f * Vector3.Dot(v, r) / z, -_f * Vector3.Dot(v, u) / z);
        }

        void Camera(float o, out Vector3 cam, out Vector3 f, out Vector3 r, out Vector3 u, out Vector2 mid)
        {
            float theta = Mathf.Lerp(_startDeg, 0, o) * Mathf.Deg2Rad;
            cam = _target + new Vector3(-Mathf.Sin(theta), 0, -Mathf.Cos(theta)) * _radius;
            f = (_target - cam).normalized;
            r = Vector3.Cross(Vector3.up, f).normalized;
            u = Vector3.Cross(f, r);
            mid = Vector2.Lerp(_startMid, _endMid, o);
        }

        void FitRadius(float mean)
        {
            if (mean < 1) return;
            float lo = 1.6f, hi = 9;
            for (int i = 0; i < 24; i++)
            {
                _radius = (lo + hi) / 2;
                var c = CanvasAt(0);
                if (((c[3] - c[0]).magnitude + (c[2] - c[1]).magnitude) / 2 > mean) lo = _radius; else hi = _radius;
            }
        }

        /* What the camera can see of the wall (x, y) and of the floor (x, z) along the whole orbit: the screen's
           edges cast onto the two planes, with a margin. Only that much is built, so no grid point ever comes near
           or behind the camera and the screen's edges never show past a layer (Tools/gap_check.py). */
        void Seen(out Rect wall, out Rect floor)
        {
            float wx0 = 1e9f, wx1 = -1e9f, wy1 = -1e9f, fx0 = 1e9f, fx1 = -1e9f, fz0 = 1e9f;
            for (int k = 0; k <= 20; k++)
            {
                Camera(k / 20f, out var cam, out var f, out var r, out var u, out var mid);
                for (int e = 0; e < 4; e++)
                    for (int i = 0; i <= 32; i++)
                    {
                        float a = i / 32f;
                        var sp = e == 0 ? new Vector2(a * _screen.x, 0) : e == 1 ? new Vector2(a * _screen.x, _screen.y)
                               : e == 2 ? new Vector2(0, a * _screen.y) : new Vector2(_screen.x, a * _screen.y);
                        var d = sp - mid;
                        var dir = f * _f + r * d.x - u * d.y;
                        if (dir.y < -1e-5f)
                        {
                            var hit = cam + dir * (-cam.y / dir.y);
                            if (hit.z < 0) { fx0 = Mathf.Min(fx0, hit.x); fx1 = Mathf.Max(fx1, hit.x); fz0 = Mathf.Min(fz0, hit.z); continue; }
                        }
                        if (dir.z > 1e-5f)
                        {
                            var hit = cam + dir * (-cam.z / dir.z);
                            wx0 = Mathf.Min(wx0, hit.x); wx1 = Mathf.Max(wx1, hit.x); wy1 = Mathf.Max(wy1, hit.y);
                        }
                    }
            }
            const float margin = .3f;
            // both as wide as either: where the wall meets the floor at a screen edge the two must meet (PM_77 b2: a
            // notch at the right edge where the floor stopped short of the wall's corner)
            float x0 = Mathf.Min(wx0, fx0) - margin, x1 = Mathf.Max(wx1, fx1) + margin;
            wall = Rect.MinMaxRect(x0, 0, x1, wy1 + margin);
            floor = Rect.MinMaxRect(x0, Mathf.Min(fz0 - margin, -.05f), x1, 0);
        }

        public Vector2[] CanvasAt(float o)
        {
            var c = _a.CanvasPx;
            return new[] { Project(o, Wall(new Vector2(c.xMin, c.yMin))), Project(o, Wall(new Vector2(c.xMax, c.yMin))),
                           Project(o, Wall(new Vector2(c.xMax, c.yMax))), Project(o, Wall(new Vector2(c.xMin, c.yMax))) };
        }

        /* The three layers, back to front: wall, floor, column. */
        internal void Build(Transform parent, Material mat, int order, List<Dive75.Layer> layers)
        {
            Seen(out var wallU, out var floorU);
            // the wall in px (y down) from the seen units; above the drawing its top rows repeat (texture clamp)
            float cx = _a.CanvasPx.center.x;
            var wallRect = Rect.MinMaxRect(Mathf.Max(0, cx + wallU.xMin * _a.PxPerUnit), _a.WallPx.y - wallU.yMax * _a.PxPerUnit,
                                           Mathf.Min(_a.WallPx.x, cx + wallU.xMax * _a.PxPerUnit), _a.WallPx.y);
            layers.Add(Dive75.Layer.FromPlane(parent, _a.Wall, mat, wallRect, new Rect(Vector2.zero, _a.WallPx), Wall, 48, 20, order));
            float fc = _a.FloorPx.x / 2;
            var floorRect = Rect.MinMaxRect(Mathf.Max(0, fc + floorU.xMin * _a.FloorPxPerUnit), 0,
                                            Mathf.Min(_a.FloorPx.x, fc + floorU.xMax * _a.FloorPxPerUnit), Mathf.Min(_a.FloorPx.y, -floorU.yMin * _a.FloorPxPerUnit));
            layers.Add(Dive75.Layer.FromPlane(parent, _a.Floor, mat, floorRect, new Rect(Vector2.zero, _a.FloorPx), Floor, 48, 16, order + 1));
            if (_a.Column != null)
                layers.Add(Dive75.Layer.FromPlane(parent, _a.Column, mat, new Rect(Vector2.zero, _a.ColumnPx), new Rect(Vector2.zero, _a.ColumnPx), Column, 4, 24, order + 2));
        }

        internal void Pose(float o, double[] post, List<Dive75.Layer> layers, float alpha)
        {
            foreach (var l in layers) l.Project(p => Project(o, p), post, alpha);
        }

        /* The painting in the canvas at o (a hair larger, under the frame's inner edge): the same in the menu and in
           the dive's first frame. */
        public Vector2[] PictureAt(float o)
        {
            var q = CanvasAt(o);
            var c = (q[0] + q[1] + q[2] + q[3]) / 4;
            for (int i = 0; i < 4; i++) q[i] = c + (q[i] - c) * PictureEnlarge;
            return q;
        }

        public const float PictureEnlarge = 1.015f;
    }
}
