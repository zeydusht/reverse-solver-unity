"""PM_92: the dive's speed as the eye sees it, from the game's own log (pm72_tour.js with TOUR_LOG='[' TOUR_LOG_TS=1,
&dalislog=1): the wall's motion on the screen between two frames (the frame's opening, [cerceve] q, is the wall plane:
its homography from one frame to the next), averaged over a grid on the whole screen, in screen heights per second.
Its growth and its slide along the arc together. Checks that it only rises up to the last --tail of the way (the
screen distance covered), allowing --noise, and how much it drops over that tail.
usage: python Tools/pm92_flow_check.py <tour log> [--csv out.csv] [--tail .08] [--noise .02] [--smooth 5]"""
import re, sys
import numpy as np
import cv2

args = sys.argv[1:]
logf = args[0]
opt = lambda n, d: type(d)(args[args.index('--' + n) + 1]) if '--' + n in args else d
csv, tail, noise, k = opt('csv', ''), opt('tail', .08), opt('noise', .02), opt('smooth', 5)
W, H = 390.0, 844.0
num = lambda s: float(s.replace(',', '.'))
fr = []
for line in open(logf, encoding='utf8', errors='replace'):
    m = re.search(r'\[cerceve\] ([\d.,-]+) e [\d.,-]+ off [\d.,-]+ q ((?:[\d.,-]+ ?){8})', line)
    if m:
        t = num(m.group(1))
        if fr and t <= fr[-1][0] + 1e-4: continue
        fr.append((t, np.float32([num(x) for x in m.group(2).split()]).reshape(4, 2)))
if len(fr) < 8: sys.exit('no [cerceve] lines')
gx, gy = np.meshgrid(np.linspace(0, W, 9), np.linspace(0, H, 19))
grid = np.stack([gx.ravel(), gy.ravel(), np.ones(gx.size)])
rows = []
for (t0, q0), (t1, q1) in zip(fr, fr[1:]):
    M = cv2.getPerspectiveTransform(q0, q1)
    p = M @ grid; p = p[:2] / p[2]
    d = np.hypot(p[0] - grid[0], p[1] - grid[1]).mean()
    h0 = (np.linalg.norm(q0[0] - q0[3]) + np.linalg.norm(q0[1] - q0[2])) / 2
    h1 = (np.linalg.norm(q1[0] - q1[3]) + np.linalg.norm(q1[1] - q1[2])) / 2
    rows.append(((t0 + t1) / 2, d / H / (t1 - t0), np.log(h1 / h0) / (t1 - t0), d / H))
t = np.array([r[0] for r in rows]); v = np.array([r[1] for r in rows]); z = np.array([r[2] for r in rows])
dist = np.cumsum([r[3] for r in rows])
vs = np.convolve(np.pad(v, k // 2, mode='edge'), np.ones(k) / k, 'valid')
share = dist / dist[-1]
body = share <= 1 - tail
run = np.maximum.accumulate(vs[body])
dip = (run - vs[body]) / run
i = int(dip.argmax())
peak = vs.max(); ip = int(vs.argmax())
print(f'{len(rows)} frames, {t[-1]:.3f} s, {dist[-1]:.2f} screen heights; speed (screen heights / s) {vs[0]:.2f} at the start, '
      f'peak {peak:.2f} at {t[ip]:.3f} s ({share[ip]:.0%} of the way), {vs[-1]:.2f} on arrival')
print(f'  rising up to {1 - tail:.0%} of the way: worst dip {dip[i]:.1%} at {t[body][i]:.3f} s '
      f'({"yes" if dip[i] <= noise else "NO"}, noise {noise:.0%}); over the last {tail:.0%}: -{(peak - vs[-1]) / peak:.1%}')
if csv:
    with open(csv, 'w') as f:
        f.write('t,flow,flow_smooth,zoom,share\n')
        for a in zip(t, v, vs, z, share): f.write(','.join(f'{x:.5f}' for x in a) + '\n')
