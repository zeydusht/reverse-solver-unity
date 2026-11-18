"""PM_88 §1: does the picture grow or turn at once when the dive arrives? From a tour's screen frames (pm72_tour.js,
TOUR_LOG='[' TOUR_LOG_TS=1): between every two frames the picture's move is fitted as a similarity (ECC, OpenCV, on
the middle of the screen: scale and turn), per 1/60 s. The arrival is the dive's approach end, from the game's own
log: the last [cerceve] line (the picture still drawn over the frame), or [varis] lines when they are there.
usage: python Tools/arrival_check.py <frames dir> <tour log> [--window .3] [--csv out.csv]
prints: per frame pair (time from the arrival, scale change %/frame, turn deg/frame) inside +-window and the
largest of each, and whether the scale changes direction (grows, then shrinks) in it."""
import json, glob, re, sys, math
import numpy as np
import cv2

args = sys.argv[1:]
d, logf = args[0], args[1]
win = float(args[args.index('--window') + 1]) if '--window' in args else .3
csv = args[args.index('--csv') + 1] if '--csv' in args else None

num = lambda s: float(s.replace(',', '.'))
cer, var, devir = [], [], []
for line in open(logf, encoding='utf8', errors='replace'):
    line = line.strip()
    m = re.match(r'^([\d.,]+) \[cerceve\] ([\d,.]+) ', line)
    if m: cer.append((num(m.group(1)) / 1000, num(m.group(2))))
    m = re.match(r'^([\d.,]+) \[varis\] ([\d,.]+) (\w+) ', line)
    if m: var.append((num(m.group(1)) / 1000, num(m.group(2)), m.group(3)))
    m = re.match(r'^([\d.,]+) \[devir\] ', line)
    if m: devir.append(num(m.group(1)) / 1000)
if var:
    over = [v for v in var if v[2] == 'overlay']
    arrival = over[-1][0] if over else var[0][0]
elif cer:
    arrival = cer[-1][0]
else:
    sys.exit('no [cerceve] or [varis] lines: run the tour with &dalislog=1')
times = json.load(open(d + '/times.json'))
files = sorted(glob.glob(d + '/f*.jpg'))
lag = .0   # the capture's frames carry the compositor's time; the log the page's: the same clock (CDP)

def load(i):
    g = cv2.cvtColor(cv2.imread(files[i]), cv2.COLOR_BGR2GRAY).astype(np.float32) / 255
    h, w = g.shape
    g = g[int(h * .15):int(h * .85), int(w * .1):int(w * .9)]   # the middle (no HUD, no bar)
    return cv2.resize(g, (g.shape[1] // 2, g.shape[0] // 2), interpolation=cv2.INTER_AREA)

rows = []
idx = [i for i in range(1, len(files)) if arrival - win - .2 <= times[i] <= arrival + win + .2]
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
    a, b, c, e = warp[0, 0], warp[0, 1], warp[1, 0], warp[1, 1]
    scale = math.sqrt(abs(a * e - b * c)); turn = math.degrees(math.atan2(c - b, a + e))
    k = (1 / 60) / dt   # per 60 Hz frame
    ds = (scale ** k - 1) * 100; dr = turn * k
    rows.append((times[i] - arrival, ds, dr, dt * 1000))
    prev = cur

inside = [r for r in rows if -win <= r[0] <= win]
if csv:
    with open(csv, 'w') as f:
        f.write('t,scale_pct_per_frame,turn_deg_per_frame,dt_ms\n')
        for r in rows: f.write('%.3f,%.4f,%.4f,%.1f\n' % r)
if not inside: sys.exit('no frames around the arrival')
ms = max(inside, key=lambda r: abs(r[1])); mr = max(inside, key=lambda r: abs(r[2]))
# a change of direction: the scale grows (> +.15 %/frame) and also shrinks (< -.15 %/frame) inside the window
grows = any(r[1] > .15 for r in inside); shrinks = any(r[1] < -.15 for r in inside)
print(f'arrival at {arrival:.3f} (log), {len(inside)} frame pairs within +-{win} s')
print(f'scale: largest {ms[1]:+.2f} %/frame at {ms[0]:+.3f} s; turn: largest {mr[2]:+.3f} deg/frame at {mr[0]:+.3f} s')
print(f'direction change: {"yes" if grows and shrinks else "no"} (grows {grows}, shrinks {shrinks})')
for r in inside: print(f'  {r[0]:+.3f}  {r[1]:+.2f} %  {r[2]:+.3f} deg  ({r[3]:.0f} ms)')
