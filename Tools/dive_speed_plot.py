"""PM_80: the dive's camera speed against time, from a capture made with &dalislog=1 (Tools/pm72_tour.js with
TOUR_LOG='[d'): every frame Dive75 logs the dive clock, the real frame time and the painting's (then the board's)
four corners on the screen. The speed is what the eye sees move: the plane's frame-to-frame homography applied to
the screen's four corners and its centre, their mean speed (pt per second of the dive clock).
    python -I Tools/dive_speed_plot.py <capture log> <out.png> [title] [phase ends, e.g. 1.8,2.6,3.4,4.4] [phase names, comma separated]
Prints the speed at each phase end, the lowest speed outside the hold, the sharpest frame-to-frame change and the
frames over 20 ms as Unity saw them (in a logging capture these run high: count them in a run without &dalislog)."""
import re, sys
import numpy as np
from PIL import Image, ImageDraw, ImageFont

LINE = re.compile(r"\[dl\] ([\d.]+) ([\d.]+) ([ob]) ((?:-?[\d.]+ ?){8})")


def read(path):
    rows = []
    for line in open(path, encoding="utf8", errors="replace"):
        m = LINE.search(line.replace(",", "."))
        if m:
            v = [float(x) for x in m.group(4).split()]
            rows.append((float(m.group(1)), float(m.group(2)), m.group(3), [(v[i], v[i + 1]) for i in range(0, 8, 2)]))
    return rows


SCREEN = [(0, 0), (390, 0), (390, 844), (0, 844), (195, 422)]


def homography(src, dst):
    m = []
    for (x, y), (u, v) in zip(src, dst):
        m.append([x, y, 1, 0, 0, 0, -u * x, -u * y, -u]); m.append([0, 0, 0, x, y, 1, -v * x, -v * y, -v])
    h = np.linalg.svd(np.array(m))[2][-1]
    return h.reshape(3, 3) / h[-1]


def speeds(rows):
    out = []
    for a, b in zip(rows, rows[1:]):
        dt = b[0] - a[0]
        if dt <= 1e-5: continue
        h = homography(a[3], b[3]); d = 0
        for x, y in SCREEN:
            u, v, w = h @ np.array([x, y, 1.0]); d += ((u / w - x) ** 2 + (v / w - y) ** 2) ** .5 / len(SCREEN)
        out.append(((a[0] + b[0]) / 2, d / dt, b[1], b[2]))
    return out


def font(size):
    try: return ImageFont.truetype("arial.ttf", size)
    except OSError: return ImageFont.load_default()


def plot(sp, dst, title, ends):
    W, H, L, R, T, B = 960, 380, 70, 20, 40, 50
    im = Image.new("RGB", (W, H), (255, 255, 255)); d = ImageDraw.Draw(im)
    tmax = max(max(s[0] for s in sp), ends[-1] if ends else 0) * 1.02
    vmax = max(s[1] for s in sp) * 1.1
    X = lambda t: L + (W - L - R) * t / tmax
    Y = lambda v: H - B - (H - T - B) * v / vmax
    names = sys.argv[5].split(",") if len(sys.argv) > 5 else ["yay", "yaklaşma", "bekleyiş", "geri çekilme"]   # PM_86: own phase names
    prev = 0
    for i, e in enumerate(ends):
        if i == 2: d.rectangle((X(prev), T, X(e), H - B), fill=(242, 244, 250))
        d.line((X(e), T, X(e), H - B), fill=(190, 190, 200))
        if i < len(names): d.text(((X(prev) + X(e)) / 2 - 28, T + 4), names[i], fill=(90, 90, 110), font=font(13))
        prev = e
    d.line((L, H - B, W - R, H - B), fill=(0, 0, 0)); d.line((L, T, L, H - B), fill=(0, 0, 0))
    step = 100 if vmax < 1200 else 200
    for v in range(0, int(vmax) + 1, step):
        d.line((L - 4, Y(v), L, Y(v)), fill=(0, 0, 0)); d.text((8, Y(v) - 7), f"{v}", fill=(0, 0, 0), font=font(12))
    for k in range(0, int(tmax * 2) + 1):
        t = k / 2; d.line((X(t), H - B, X(t), H - B + 4), fill=(0, 0, 0)); d.text((X(t) - 8, H - B + 6), f"{t:.1f}", fill=(0, 0, 0), font=font(12))
    pts = [(X(t), Y(v)) for t, v, _, _ in sp]
    d.line(pts, fill=(40, 90, 200), width=2)
    d.text((L, 10), title, fill=(0, 0, 0), font=font(16))
    d.text((W - 330, H - 22), "ekranda görünen hız (pt/sn) — süre (sn)", fill=(80, 80, 80), font=font(12))
    im.save(dst)


def main():
    a = sys.argv[1:]
    rows = read(a[0]); sp = speeds(rows)
    title = a[2] if len(a) > 2 else ""
    ends = [float(x) for x in a[3].split(",")] if len(a) > 3 else []
    plot(sp, a[1], title, ends)
    print(f"{a[1]}: {len(rows)} frames logged, {len(sp)} speeds")
    for e in ends:
        near = min(sp, key=lambda s: abs(s[0] - e)); print(f"  at {e:.2f} s: {near[1]:.0f} pt/s")
    if len(ends) >= 3:
        out = [s for s in sp if not (ends[1] - .05 <= s[0] <= ends[2] + .25) and .1 < s[0] < ends[-1] - .1]
        lo = min(out, key=lambda s: s[1]); print(f"  lowest outside the hold (and the first/last .1 s): {lo[1]:.0f} pt/s at {lo[0]:.2f} s")
    worst = (0, 0)
    for p, q in zip(sp, sp[1:]):
        if max(p[1], q[1]) > 40:
            r = abs(q[1] - p[1]) / max(p[1], q[1])
            if r > worst[0]: worst = (r, q[0])
    print(f"  sharpest change between two frames: {worst[0] * 100:.0f} % at {worst[1]:.2f} s")
    long = [(round(s[0], 2), s[2]) for s in sp if s[2] > 20]
    print(f"  frames over 20 ms (dive clock, ms): {long}")


if __name__ == "__main__":
    main()
