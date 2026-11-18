"""PM_87 §2: does the picture move with its frame? From the game's own log ([cerceve]: the frame opening's 4 corners
each frame) the opening's homography between two captured frames predicts where every point of the wall plane
goes. Patches on the gold frame and inside the picture are tracked (SSD, sub-pixel) between consecutive captured
frames; each patch's error = measured move - predicted move. The capture's lag behind the log is fitted on the frame
patches; the picture patches' errors then say whether the picture is glued to the frame (and its frame to the
camera). usage: sync2.py <frames dir> <log> <seconds after the press>"""
import json, glob, re, sys
import numpy as np
from PIL import Image
d, logf, secs = sys.argv[1], sys.argv[2], float(sys.argv[3])
log = []
for line in open(logf, encoding='utf8', errors='replace'):
    m = re.match(r'^([\d.,]+) \[cerceve\] ([\d,.]+) e ([\d,.]+) off [\d,.]+ q (.*)$', line.strip())
    if not m: continue
    num = lambda s: float(s.replace(',', '.'))
    vals = [num(v) for v in m.group(4).split()]
    log.append((num(m.group(1)) / 1000, num(m.group(3)), np.array(vals).reshape(4, 2)))
t = json.load(open(d + '/times.json')); fs = sorted(glob.glob(d + '/f*.jpg'))
L = lambda i: np.asarray(Image.open(fs[i]).convert('L'), dtype=float)
def H(src, dst):
    A = []
    for (x, y), (u, v) in zip(src, dst):
        A += [[x, y, 1, 0, 0, 0, -u * x, -u * y, -u], [0, 0, 0, x, y, 1, -v * x, -v * y, -v]]
    h = np.linalg.svd(np.array(A))[2][-1]; return (h / h[-1]).reshape(3, 3)
def ap(h, p):
    v = h @ np.array([p[0], p[1], 1.0]); return v[:2] / v[2]
R, S = 10, 8
def track(a, b, p):
    x, y = int(round(p[0])), int(round(p[1]))
    tpl = a[y - R:y + R, x - R:x + R]; ssd = np.full((2 * S + 1, 2 * S + 1), np.inf)
    for dy in range(-S, S + 1):
        for dx in range(-S, S + 1):
            w = b[y + dy - R:y + dy + R, x + dx - R:x + dx + R]
            if w.shape == tpl.shape and tpl.size: ssd[dy + S, dx + S] = ((w - tpl) ** 2).mean()
    iy, ix = np.unravel_index(ssd.argmin(), ssd.shape)
    def sub(c, l, r): return 0 if not np.isfinite(l + r) or l + r - 2 * c == 0 else .5 * (l - r) / (l + r - 2 * c)
    fx = ix - S + (sub(ssd[iy, ix], ssd[iy, ix - 1], ssd[iy, ix + 1]) if 0 < ix < 2 * S else 0)
    fy = iy - S + (sub(ssd[iy, ix], ssd[iy - 1, ix], ssd[iy + 1, ix]) if 0 < iy < 2 * S else 0)
    return np.array([p[0] + fx - x + x, p[1] + fy - y + y]) + 0, np.array([fx + (x - p[0]) * 0, fy])
def quad_at(time):
    k = max([i for i, e in enumerate(log) if e[0] <= time] or [0]); return log[k][2], log[k][1]
press = log[0][0]
# patches in opening coordinates (u, v in 0..1): inside the picture, and just outside it (the gold frame)
pic_uv = [(.15, .2), (.85, .2), (.5, .5), (.15, .8), (.85, .8)]
frame_uv = [(-.06, .5), (1.06, .5), (.5, -.04), (.5, 1.04)]
def at_uv(q, u, v):
    h = H(np.array([[0, 0], [1, 0], [1, 1], [0, 1]], float), q); return ap(h, (u, v))
frames = [i for i in range(1, len(fs)) if press - .05 <= t[i] <= press + secs]
def run(lag, report=False):
    errs_f, errs_p, rows = [], [], []
    for i in frames:
        qa, ea = quad_at(t[i - 1] - lag); qb, eb = quad_at(t[i] - lag)
        if eb > 1e-4: break   # the morph to the board has begun (the frame is off screen)
        h = H(qa, qb); a, b = L(i - 1), L(i)
        row_f, row_p = [], []
        for uvs, out in ((frame_uv, row_f), (pic_uv, row_p)):
            for u, v in uvs:
                p = at_uv(qa, u, v)
                if not (25 < p[0] < a.shape[1] - 25 and 25 < p[1] < a.shape[0] - 25): continue
                pred = ap(h, p) - p
                if np.abs(pred).max() > S - 1: continue
                _, meas = track(a, b, p)
                out.append(float(np.abs(meas - pred).max()))
        errs_f += row_f; errs_p += row_p
        rows.append((t[i] - press, max(row_f, default=0), max(row_p, default=0)))
    if report:
        for dt, f, p in rows: print(f'{dt * 1000:+5.0f} ms  frame {f:.2f} px  picture {p:.2f} px')
    return (np.median(errs_f) if errs_f else 9), errs_f, errs_p, rows
best = min((run(l / 1000)[0], l) for l in range(0, 80, 2))
lag = best[1] / 1000
_, ef, ep, rows = run(lag, True)
print(f'lag {lag * 1000:.0f} ms; {len(rows)} frame pairs; frame patches: median {np.median(ef):.2f}, p95 {np.percentile(ef, 95):.2f}, max {max(ef):.2f} px; '
      f'picture patches: median {np.median(ep):.2f}, p95 {np.percentile(ep, 95):.2f}, max {max(ep):.2f} px')
