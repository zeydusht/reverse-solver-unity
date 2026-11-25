"""PM_110: the screen speed as the eye sees it, from a Tools/pm108_dalis.js capture's frames (not the game's log):
dense optical flow (Farneback) between consecutive frames, its mean length over the screen's middle band (the HUD
and the menu's bar slide in and out above and below it), in screen heights per second, against the time from the
dive's start (its first [cerceve] line). The dive (to the touch, the first [gecis] değme line) and the pull back
(the [geri] lines, w > 0) apart: their way, time and mean speed.
usage: python -I Tools/pm110_ekran_hizi.py <capture dir> [--csv out.csv] [--band .14,.80]"""
import glob, json, os, re, sys
import numpy as np
import cv2

argv = sys.argv[1:]
d = argv[0]
opt = lambda n, dflt: argv[argv.index('--' + n) + 1] if '--' + n in argv else dflt
y0, y1 = (float(x) for x in opt('band', '.14,.80').split(','))
num = lambda s: float(s.replace(',', '.'))
start = touch = back0 = back1 = None
for line in open(os.path.join(d, 'log.txt'), encoding='utf8', errors='replace'):
    ms = line.split(' ', 1)[0]
    try: at = float(ms) / 1000
    except ValueError: continue
    if start is None and re.search(r'\[cerceve\] 0[.,]000 ', line): start = at
    if start is not None and touch is None and '[gecis] olay değme' in line: touch = at - start
    m = re.search(r'\[geri\] [\d.,]+ w ([\d.,]+)', line)
    if m and start is not None:
        w = num(m.group(1))
        if w > 0 and back0 is None: back0 = at - start
        if w >= 1 and back1 is None: back1 = at - start
if start is None or touch is None: sys.exit('no dive start / touch in the log')
if back0 is None:   # an older capture without [geri] lines: the board's [varis] scale, from its first fall to its last change
    sc = []
    for line in open(os.path.join(d, 'log.txt'), encoding='utf8', errors='replace'):
        m = re.search(r'^([\d.]+) \[varis\] [\d.,]+ board ([\d.,]+)', line)
        if m: sc.append((float(m.group(1)) / 1000 - start, num(m.group(2))))
    for (ta, a), (tb, b) in zip(sc, sc[1:]):
        if back0 is None and b < a - 1e-4: back0 = ta
        if back0 is not None and abs(b - a) > 1e-4: back1 = tb
times = json.load(open(os.path.join(d, 'times.json'))); files = sorted(glob.glob(os.path.join(d, 'f*.jpg')))
n = min(len(times), len(files))
H = 422
prev = None; rows = []
for i in range(n):
    t = times[i] - start
    if t < -.1 or t > (back1 or touch + 1.2) + .1: continue
    g = cv2.cvtColor(cv2.resize(cv2.imread(files[i]), (195, H), interpolation=cv2.INTER_AREA), cv2.COLOR_BGR2GRAY)
    if prev is not None and t > prev[0] + 1e-3:
        f = cv2.calcOpticalFlowFarneback(prev[1], g, None, .5, 4, 21, 4, 7, 1.5, 0)
        band = f[int(y0 * H):int(y1 * H)]
        rows.append(((prev[0] + t) / 2, np.hypot(band[..., 0], band[..., 1]).mean() / H / (t - prev[0]), t - prev[0]))
    prev = (t, g)
r = np.array(rows)
def part(a, b):
    m = (r[:, 0] > a) & (r[:, 0] < b)
    way = (r[m, 1] * r[m, 2]).sum()
    return way, b - a, way / (b - a), r[m, 1].max() if m.any() else 0
dw, dt, dv, dp = part(0, touch)
print(f'dive      0.000-{touch:.3f} s: {dw:.3f} screen heights, mean {dv:.3f}/s, peak {dp:.3f}/s')
if back0 is not None and back1 is not None:
    bw, bt, bv, bp = part(back0, back1)
    print(f'pull back {back0:.3f}-{back1:.3f} s: {bw:.3f} screen heights, mean {bv:.3f}/s, peak {bp:.3f}/s')
if '--csv' in argv:
    with open(opt('csv', ''), 'w') as f:
        f.write(f'# touch {touch:.4f} back0 {back0 or 0:.4f} back1 {back1 or 0:.4f}\nt,v,dt\n')
        for a in r: f.write(','.join(f'{x:.5f}' for x in a) + '\n')

# the pull back from the game's own camera ([geri] kamera t s cx cy W H: screen points per unit and its centre), the
# way PM_92 measures the dive (Flow92: a 9 x 19 grid on the screen, its mean motion, screen heights): exact for the
# camera; the optical flow above reads little on flat colour (the dive's plain wall), so the two are told apart
cam = []
for line in open(os.path.join(d, 'log.txt'), encoding='utf8', errors='replace'):
    m = re.search(r'\[geri\] kamera ([\d.,-]+) ([\d.,-]+) ([\d.,-]+) ([\d.,-]+) (\d+) (\d+)', line)
    if m: cam.append([num(m.group(i)) for i in range(1, 7)])
gw = []
for line in open(os.path.join(d, 'log.txt'), encoding='utf8', errors='replace'):
    m = re.search(r'\[geri\] ([\d.,]+) w ([\d.,]+)', line)
    if m: gw.append((num(m.group(1)), num(m.group(2))))
if cam and gw:
    cam = np.array(cam); CW, CH = cam[0, 4], cam[0, 5]
    c0 = min(t for t, x in gw if x > 0); c1 = min(t for t, x in gw if x >= 1)
    gx, gy = np.meshgrid(np.linspace(-CW / 2, CW / 2, 9), np.linspace(-CH / 2, CH / 2, 19)); P = np.stack([gx.ravel(), gy.ravel()], 1)
    way, peak = 0, 0
    for (t0, s0, x0, yc0, _, _), (t1, s1, x1, yc1, _, _) in zip(cam, cam[1:]):
        if t1 <= t0 or t0 < c0 - 1e-3 or t1 > c1 + 1e-3: continue
        Lx, Ly = P[:, 0] / s0 + x0, P[:, 1] / s0 - yc0
        step = np.hypot(s1 * (Lx - x1) - P[:, 0], s1 * (Ly + yc1) - P[:, 1]).mean() / CH
        way += step; peak = max(peak, step / (t1 - t0))
    print(f'pull back (camera, as Flow92) {c0:.3f}-{c1:.3f} s: {way:.3f} screen heights, mean {way / (c1 - c0):.3f}/s, peak {peak:.3f}/s')
