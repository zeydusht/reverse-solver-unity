"""PM_89 §1: does the picture stay one picture (the same scale across and down, the same crop) through the dive's end?
From a tour's frames (pm72_tour.js, TOUR_LOG='[' TOUR_LOG_TS=1, &dalislog=1): between every two frames the picture's
move is fitted as an affine map (OpenCV ECC on the screen's middle); its X and Y scale (the columns' lengths) and
their ratio per frame, and the ratio carried from the arrival (the product of the steps). The arrival is the
approach's end from the game's log ([cerceve] last line, or the [devir] hand-over).
After the arrival the crop must not change at all: the steps' scale and shift are reported there too.
usage: python Tools/pm89_aspect_check.py <frames dir> <tour log> [--from -1.0] [--to 1.4] [--csv out.csv]"""
import json, glob, re, sys, math
import numpy as np
import cv2

args = sys.argv[1:]
d, logf = args[0], args[1]
t0 = float(args[args.index('--from') + 1]) if '--from' in args else -1.0
t1 = float(args[args.index('--to') + 1]) if '--to' in args else 1.4
csv = args[args.index('--csv') + 1] if '--csv' in args else None
num = lambda s: float(s.replace(',', '.'))
cer, devir = [], []
for line in open(logf, encoding='utf8', errors='replace'):
    line = line.strip()
    m = re.match(r'^([\d.,]+) \[cerceve\] ', line)
    if m: cer.append(num(m.group(1)) / 1000)
    m = re.match(r'^([\d.,]+) \[devir\] ', line)
    if m: devir.append(num(m.group(1)) / 1000)
if not cer and not devir: sys.exit('no [cerceve] / [devir] lines: run the tour with &dalislog=1')
arrival = devir[0] if devir else cer[-1]
times = json.load(open(d + '/times.json'))
files = sorted(glob.glob(d + '/f*.jpg'))


def load(i):
    g = cv2.cvtColor(cv2.imread(files[i]), cv2.COLOR_BGR2GRAY).astype(np.float32) / 255
    h, w = g.shape
    g = g[int(h * .3):int(h * .7), int(w * .2):int(w * .8)]   # the picture's middle (inside the board in both builds)
    return cv2.resize(g, (g.shape[1] // 2, g.shape[0] // 2), interpolation=cv2.INTER_AREA)


rows = []
idx = [i for i in range(1, min(len(files), len(times))) if arrival + t0 <= times[i] <= arrival + t1]
prev = load(idx[0] - 1) if idx else None
for i in idx:
    cur = load(i)
    dt = times[i] - times[i - 1]
    if dt <= 1e-4: prev = cur; continue
    warp = np.eye(2, 3, dtype=np.float32)
    try:
        _, warp = cv2.findTransformECC(prev, cur, warp, cv2.MOTION_AFFINE,
                                       (cv2.TERM_CRITERIA_EPS | cv2.TERM_CRITERIA_COUNT, 200, 1e-6), None, 5)
    except cv2.error:
        prev = cur; continue
    sx = math.hypot(warp[0, 0], warp[1, 0]); sy = math.hypot(warp[0, 1], warp[1, 1])
    rows.append((times[i] - arrival, sx, sy, float(warp[0, 2]) * 2, float(warp[1, 2]) * 2))
    prev = cur
if not rows: sys.exit('no frames in the window')
# the ratio carried through the window (the product of the steps), and from the arrival on
carry = 1.0; out = []
for t, sx, sy, tx, ty in rows:
    carry *= sx / sy
    out.append((t, sx, sy, sx / sy, carry, tx, ty))
before = [r for r in out if r[0] < 0]; after = [r for r in out if r[0] >= 0]
step = max(abs(r[3] - 1) for r in out)
print(f'arrival at {arrival:.3f} s; {len(out)} frame pairs from {t0:+.1f} to {t1:+.1f} s')
print(f'X/Y ratio per frame: worst {1 + step if max(out, key=lambda r: abs(r[3] - 1))[3] > 1 else 1 - step:.4f} (1.00 +- .01: {"yes" if step <= .01 else "NO"})')
cs = [r[4] for r in out]
print(f'X/Y ratio carried through the window: {min(cs):.4f} .. {max(cs):.4f}')
if after:
    sc = max(max(abs(r[1] - 1), abs(r[2] - 1)) for r in after); sh = max(math.hypot(r[5], r[6]) for r in after)
    carry_a = 1.0; sxa = 1.0
    for r in after: sxa *= r[1]
    print(f'after the arrival ({len(after)} pairs): scale step at most {sc * 100:.2f} %, shift at most {sh:.2f} px a frame; scale carried {sxa:.4f} (the crop {"unchanged" if abs(sxa - 1) < .005 and sc < .003 else "CHANGES"})')
for r in out[::max(1, len(out) // 24)]:
    print(f'  {r[0]:+.3f} s  sx {r[1]:.4f}  sy {r[2]:.4f}  x/y {r[3]:.4f}  carried {r[4]:.4f}')
if csv:
    with open(csv, 'w') as f:
        f.write('t,sx,sy,ratio,carried,tx,ty\n')
        for r in out: f.write(','.join(f'{v:.5f}' for v in r) + '\n')
