"""PM_78: the menu picture (menu_bg.png, 1024x1536) as a 3D diorama seen from Zeyd's camera path.

The menu's own camera is solved from its painting: the canvas is a rectangle on the wall, its top and bottom edges
meet at the wall's vanishing point, the verticals are upright (no pitch, a shifted lens: the principal point sits on
the horizon). The scale along the view is free (one family of cameras fits the drawing); the start angle is fixed to
Zeyd's first key (30.2 deg), which fixes the focal length. Units: the canvas width is 1 (W).

World: x right along the wall, y up (floor 0), z out of the wall towards the camera (wall z = 0), as Zeyd's studio.
Every layer is stored in the menu picture's own pixel grid (extended past its edges where the path sees more), so the
first frame redraws menu_bg pixel for pixel; each pixel's 3D point is where the start camera's ray through it meets
the layer's plane.
    python -I Tools/pm78_geom.py            # prints the solved camera and the path's needs"""
import json, math, os, sys

IMG_W, IMG_H = 1024, 1536
# the canvas opening in menu_bg (Screens.Menu.Opening, PM_18): TL, TR, BR, BL
CANVAS = [(349, 296), (662, 359), (663, 832), (348, 815)]

ZEYD = os.path.join(os.path.dirname(__file__), "..", "docs", "art", "pm78", "zeyd_yol.json")


def line(p, q):
    a = q[1] - p[1]; b = p[0] - q[0]
    return a, b, a * p[0] + b * p[1]


def meet(l1, l2):
    d = l1[0] * l2[1] - l2[0] * l1[1]
    return ((l1[2] * l2[1] - l2[2] * l1[1]) / d, (l1[0] * l2[2] - l2[0] * l1[2]) / d)


class Menu:
    """The start camera (image pixels) and the room's planes."""

    def __init__(self, start_deg, wall_base_y=None):
        tl, tr, br, bl = CANVAS
        vp = meet(line(tl, tr), line(bl, br))
        c = meet(line(tl, br), line(tr, bl))           # the canvas centre, projected
        self.vp = vp
        self.cx = c[0]                                 # the camera looks at the canvas centre: it projects on the axis
        self.cy = vp[1]                                # no pitch: the principal point on the horizon
        self.theta = math.radians(start_deg)
        self.f = (vp[0] - self.cx) * math.tan(self.theta)
        # canvas width from its left and right edges (at the centre's height the edges are x = +-W/2 on the wall)
        xl = (tl[0] + bl[0]) / 2 - self.cx; xr = (tr[0] + br[0]) / 2 - self.cx
        # px = f x cos / (D + x sin): with W = 1, solve D from the right edge (x = .5)
        s, co = math.sin(self.theta), math.cos(self.theta)
        self.D = .5 * (self.f * co / xr - s)
        self.D_left = -.5 * (self.f * co / xl) + .5 * s     # the same from the left edge (a check)
        self.target_img = c
        # heights: the canvas centre and the wall's foot below the eye
        self.yc_minus_h = (self.cy - c[1]) * self.D / self.f
        base = wall_base_y if wall_base_y is not None else self._base_at(self.cx)
        self.h = (base - self.cy) * self.D / self.f
        self.yc = self.h + self.yc_minus_h
        self.cam = (-self.D * s, self.h, self.D * co)

    @staticmethod
    def _base_at(x):
        # the wall's foot as drawn (where the wainscot meets the floor): (160, 1173) .. (783, 1123)
        return 1173 + (x - 160) * (1123 - 1173) / (783 - 160)

    # ---- the start camera ----
    def basis(self, theta=None, cam=None):
        th = self.theta if theta is None else theta
        d = (math.sin(th), 0.0, -math.cos(th)); r = (math.cos(th), 0.0, math.sin(th)); u = (0.0, 1.0, 0.0)
        return (cam or self.cam), d, r, u

    def project(self, p, cam=None, theta=None, f=None, mid=None):
        c, d, r, u = self.basis(theta, cam)
        v = (p[0] - c[0], p[1] - c[1], p[2] - c[2])
        z = v[0] * d[0] + v[1] * d[1] + v[2] * d[2]
        x = v[0] * r[0] + v[2] * r[2]; y = v[1]
        f = self.f if f is None else f
        mx, my = (self.cx, self.cy) if mid is None else mid
        return mx + f * x / z, my - f * y / z, z

    def ray(self, px, py, cam=None, theta=None, f=None, mid=None):
        c, d, r, u = self.basis(theta, cam)
        f = self.f if f is None else f
        mx, my = (self.cx, self.cy) if mid is None else mid
        a, b = (px - mx) / f, -(py - my) / f
        return c, (d[0] + a * r[0], b, d[2] + a * r[2])

    def on_wall(self, px, py, **k):
        c, v = self.ray(px, py, **k)
        t = -c[2] / v[2]
        return (c[0] + t * v[0], c[1] + t * v[1], 0.0)

    def on_floor(self, px, py, **k):
        c, v = self.ray(px, py, **k)
        if v[1] >= 0: return None
        t = -c[1] / v[1]
        return (c[0] + t * v[0], 0.0, c[2] + t * v[2])

    def wall_canvas(self):
        return [self.on_wall(*p) for p in CANVAS]


# ---- Zeyd's path ----

def smootherstep(t):
    t = min(1.0, max(0.0, t)); return t * t * t * (t * (t * 6 - 15) + 10)


def catmull_centripetal(p0, p1, p2, p3, u, alpha=.5):
    def tj(ti, a, b): return ti + max(1e-9, math.dist(a, b)) ** alpha
    t0 = 0.0; t1 = tj(t0, p0, p1); t2 = tj(t1, p1, p2); t3 = tj(t2, p2, p3)
    t = t1 + (t2 - t1) * u
    def lerp(a, b, ta, tb): return tuple(a[i] * (tb - t) / (tb - ta) + b[i] * (t - ta) / (tb - ta) for i in range(len(a)))
    a1 = lerp(p0, p1, t0, t1); a2 = lerp(p1, p2, t1, t2); a3 = lerp(p2, p3, t2, t3)
    b1 = lerp(a1, a2, t0, t2); b2 = lerp(a2, a3, t1, t3)
    return lerp(b1, b2, t1, t2)


def catmull_uniform(p0, p1, p2, p3, u):
    u2, u3 = u * u, u * u * u
    return tuple(.5 * (2 * p1[i] + (-p0[i] + p2[i]) * u + (2 * p0[i] - 5 * p1[i] + 4 * p2[i] - p3[i]) * u2 + (-p0[i] + 3 * p1[i] - 3 * p2[i] + p3[i]) * u3) for i in range(2))


def zeyd_keys():
    z = json.load(open(ZEYD, encoding="utf8"))
    return [(k["x"], k["z"]) for k in z["kareler"]], [k["dur"] for k in z["kareler"]], z


def path_at(keys, durs, s, centripetal=True):
    """s: 0..1 of the flight AFTER the global ease (tau / total). Returns (x, z) in Zeyd's metres."""
    total = sum(durs[1:]); tau = s * total; i = 1
    while i < len(keys) - 1 and tau > durs[i]: tau -= durs[i]; i += 1
    u = min(1.0, tau / durs[i])
    a, b = keys[i - 1], keys[i]
    if centripetal:
        p0 = keys[i - 2] if i >= 2 else (2 * a[0] - b[0], 2 * a[1] - b[1])
        p3 = keys[i + 1] if i + 1 < len(keys) else (2 * b[0] - a[0], 2 * b[1] - a[1])
        return catmull_centripetal(p0, a, b, p3, u)
    p0 = keys[max(0, i - 2)]; p3 = keys[min(len(keys) - 1, i + 1)]
    return catmull_uniform(p0, a, b, p3, u)


def main():
    keys, durs, z = zeyd_keys()
    k1 = keys[0]
    start_deg = math.degrees(math.atan2(-k1[0], k1[1]))
    m = Menu(start_deg)
    r1 = math.hypot(*k1)
    scale = m.D / r1                                   # W per Zeyd metre: frame 1 sits on the menu's camera
    print(f"vp {m.vp[0]:.1f},{m.vp[1]:.1f}  centre {m.target_img[0]:.1f},{m.target_img[1]:.1f}  f {m.f:.1f}  start {start_deg:.2f} deg")
    print(f"D {m.D:.3f} W (left edge check {m.D_left:.3f})  eye h {m.h:.3f} W  canvas centre y {m.yc:.3f} W  cam {tuple(round(v, 3) for v in m.cam)}")
    wc = m.wall_canvas()
    print("canvas on wall:", [tuple(round(v, 3) for v in p) for p in wc])
    print(f"scale {scale:.3f} W per Zeyd metre; Zeyd's frame (1.25 m) would be {1.25 * scale:.2f} W wide")
    dev = 0
    for i in range(201):
        s = i / 200
        a = path_at(keys, durs, s, True); b = path_at(keys, durs, s, False)
        dev = max(dev, math.dist(a, b))
    print(f"centripetal vs the studio's uniform Catmull-Rom: at most {dev * 100:.1f} cm apart (Zeyd metres)")
    for k in keys:
        print(f"  key {k}  -> W ({k[0] * scale:.3f}, {k[1] * scale:.3f})  angle {math.degrees(math.atan2(-k[0], k[1])):.1f}  dist {math.hypot(*k) * scale:.3f} W")


if __name__ == "__main__":
    main()


# ---- the flight, in screen points ----

class Flight:
    """The camera along Zeyd's path for one screen size. The menu picture is cover-fitted (Screens.Menu), so the
    start camera in screen points is the image camera scaled by k about the picture's centre. The lens shift (the
    principal point) moves so the painting's centre goes from where the menu shows it to the screen's centre."""

    def __init__(self, menu, screen, centripetal=True):
        self.m = menu; self.sw, self.sh = screen
        self.k = max(self.sw / IMG_W, self.sh / IMG_H)
        self.f = menu.f * self.k
        self.keys, self.durs, _ = zeyd_keys()
        self.scale = menu.D / math.hypot(*self.keys[0])
        self.centripetal = centripetal
        self.start_mid = self.to_screen(menu.cx, menu.cy)
        self.start_paint = self.to_screen(*menu.target_img)

    def to_screen(self, px, py):
        return (self.sw / 2 + self.k * (px - IMG_W / 2), self.sh / 2 + self.k * (py - IMG_H / 2))

    def to_image(self, sx, sy):
        return ((sx - self.sw / 2) / self.k + IMG_W / 2, (sy - self.sh / 2) / self.k + IMG_H / 2)

    def pose(self, o):
        """o: 0..1 along the path (after the global ease). Camera (W), heading, principal point (screen)."""
        x, z = path_at(self.keys, self.durs, o, self.centripetal)
        x *= self.scale; z *= self.scale
        theta = math.atan2(-x, z)                      # the head on the painting's centre (x = 0)
        cam = (x, self.m.h, z)
        dist = math.hypot(x, z)
        paint_y = self.start_paint[1] + (self.sh / 2 - self.start_paint[1]) * o
        mid_x = self.start_mid[0] + (self.sw / 2 - self.start_mid[0]) * o
        mid_y = paint_y + self.f * (self.m.yc - self.m.h) / dist
        return cam, theta, (mid_x, mid_y)

    def hit(self, o, sx, sy):
        cam, th, mid = self.pose(o)
        c, v = self.m.ray(sx, sy, cam=cam, theta=th, f=self.f, mid=mid)
        if v[1] < 0:
            t = -c[1] / v[1]; p = (c[0] + t * v[0], 0.0, c[2] + t * v[2])
            if p[2] > 0: return "floor", p
        t = -c[2] / v[2]
        return "wall", (c[0] + t * v[0], c[1] + t * v[1], 0.0)


def needs(screens, steps=40, n=48):
    m = Menu(math.degrees(math.atan2(-zeyd_keys()[0][0][0], zeyd_keys()[0][0][1])))
    out = {}
    for scr in screens:
        fl = Flight(m, scr)
        box = {"wall": [1e9, 1e9, -1e9, -1e9], "floor": [1e9, 1e9, -1e9, -1e9]}
        for i in range(steps + 1):
            o = i / steps
            for e in range(4):
                for j in range(n + 1):
                    a = j / n
                    sx, sy = [(a * fl.sw, 0), (a * fl.sw, fl.sh), (0, a * fl.sh), (fl.sw, a * fl.sh)][e]
                    kind, p = fl.hit(o, sx, sy)
                    px, py, _ = m.project(p)
                    b = box[kind]
                    b[0] = min(b[0], px); b[1] = min(b[1], py); b[2] = max(b[2], px); b[3] = max(b[3], py)
        out[scr] = box
    return m, out


if __name__ == "__main__" and len(sys.argv) > 1 and sys.argv[1] == "needs":
    m, out = needs([(390, 844), (375, 667), (430, 932), (360, 780), (414, 896)])
    for scr, box in out.items():
        print(scr, {k: [round(v) for v in b] for k, b in box.items()})
