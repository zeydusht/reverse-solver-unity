"""PM_90 §3: the picture's X and Y scale frame to frame from the frames themselves (an independent check of
pm90_pullback_check.py, which reads the game's own numbers). SIFT points matched between each two frames and an
affine map fitted with RANSAC: the cuts being drawn, the shadows and the pieces' look are outliers, not a stretch
(an area fit, pm89_aspect_check.py, follows them). Only points on the picture: inside the board ([dl] ... b, inset
4 %) and the screen, under the HUD's top band and above its bottom one.
usage: python Tools/pm90_feature_check.py <frames dir> <tour log> [--from -1.0] [--to 1.2] [--csv out.csv]
  (pm72_tour.js with TOUR_LOG='[' TOUR_LOG_TS=1, &dalislog=1; before the hand-over the whole screen is used)"""
import json, glob, re, sys, math
import numpy as np
import cv2

args = sys.argv[1:]
d, logf = args[0], args[1]
t0 = float(args[args.index('--from') + 1]) if '--from' in args else -1.0
t1 = float(args[args.index('--to') + 1]) if '--to' in args else 1.2
csv = args[args.index('--csv') + 1] if '--csv' in args else None
num = lambda s: float(s.replace(',', '.'))
devir_page = devir_game = None; boards = []
for line in open(logf, encoding='utf8', errors='replace'):
    m = re.match(r'^([\d.,]+) \[devir\] ([\d.,]+)', line)
    if m and devir_page is None: devir_page, devir_game = num(m.group(1)) / 1000, num(m.group(2))
    m = re.match(r'^([\d.,]+) \[dl\] ([\d.,-]+) [\d.,-]+ b ((?:[\d.,-]+ ?){8})', line)
    if m:
        v = [num(x) for x in m.group(3).split()]
        boards.append((num(m.group(1)) / 1000, v[0], v[1], v[2], v[5]))
if devir_page is None: sys.exit('no [devir] line')
times = json.load(open(d + '/times.json'))
files = sorted(glob.glob(d + '/f*.jpg'))
sift = cv2.SIFT_create(nfeatures=1500)
bf = cv2.BFMatcher()


def board_at(t):
    past = [b for b in boards if b[0] <= t + .002]
    return past[-1] if past else None


def feats(i):
    im = cv2.cvtColor(cv2.imread(files[i]), cv2.COLOR_BGR2GRAY)
    h, w = im.shape; k = w / 390.0
    mask = np.zeros_like(im)
    x0, y0, x1, y1 = 0, int(110 * k), w, int(h - 120 * k)   # under the top HUD band, over the bottom bar
    b = board_at(times[i]) if times[i] >= devir_page else None
    if b:
        _, l, tp, r, bt = b
        ix, iy = (r - l) * .04, (bt - tp) * .04
        x0, y0 = max(x0, int((l + ix) * k)), max(y0, int((tp + iy) * k))
        x1, y1 = min(x1, int((r - ix) * k)), min(y1, int((bt - iy) * k))
    if x1 - x0 < 40 or y1 - y0 < 40: return None, None
    mask[y0:y1, x0:x1] = 255
    return sift.detectAndCompute(im, mask)


rows = []
idx = [i for i in range(1, min(len(files), len(times))) if devir_page + t0 <= times[i] <= devir_page + t1]
prev = feats(idx[0] - 1) if idx else None
for i in idx:
    cur = feats(i)
    if prev[1] is None or cur[1] is None or len(prev[0]) < 8 or len(cur[0]) < 8: prev = cur; continue
    good = [m for m, n in (p for p in bf.knnMatch(prev[1], cur[1], k=2) if len(p) == 2) if m.distance < .7 * n.distance]
    if len(good) < 12: prev = cur; continue
    a = np.float32([prev[0][m.queryIdx].pt for m in good]); b = np.float32([cur[0][m.trainIdx].pt for m in good])
    M, inl = cv2.estimateAffine2D(a, b, method=cv2.RANSAC, ransacReprojThreshold=1.5, maxIters=4000, confidence=.999)
    if M is None or inl.sum() < 12: prev = cur; continue
    sx = math.hypot(M[0, 0], M[1, 0]); sy = math.hypot(M[0, 1], M[1, 1])
    rows.append((times[i] - devir_page, sx, sy, sx / sy, int(inl.sum())))
    prev = cur
if not rows: sys.exit('no frame pairs')
carry = 1.0; out = []
for t, sx, sy, r, n in rows:
    carry *= r; out.append((t, sx, sy, r, carry, n))
worst = max(out, key=lambda r: abs(r[3] - 1))
print(f'{len(out)} frame pairs {t0:+.2f}..{t1:+.2f} s from the hand-over; X/Y per frame worst {worst[3]:.4f} at {worst[0]:+.3f} s '
      f'(1.00 +- .01: {"yes" if abs(worst[3] - 1) <= .01 else "NO"}); carried {min(r[4] for r in out):.4f} .. {max(r[4] for r in out):.4f}; '
      f'points per pair {min(r[5] for r in out)}..{max(r[5] for r in out)}')
for seg, lo, hi in (('approach', -9, 0), ('hold', 0, .4), ('pull back', .4, 9)):
    s = [r for r in out if lo <= r[0] < hi]
    if s:
        w = max(s, key=lambda r: abs(r[3] - 1))
        print(f'  {seg}: {len(s)} pairs, X/Y per frame worst {w[3]:.4f}; scale per frame {min(r[2] for r in s):.4f}..{max(r[2] for r in s):.4f}')
if csv:
    with open(csv, 'w') as f:
        f.write('t,sx,sy,ratio,carried,inliers\n')
        for r in out: f.write(','.join(f'{v:.5f}' for v in r) + '\n')
