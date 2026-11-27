"""PM_115: the painting's turn and sideways slide on the screen per game frame around the end of the flight (Tools/pm108_dalis.js
captures with &dalislog=1, the [cerceve] q corners): each frame's turn (deg) and the centre's x step (pt), per 1/60 s
(so a longer frame is not a jump), and the criterion: no frame's change of that step over twice its neighbours' mean
(changes under a floor, .06 deg / .06 pt, count as none).
usage: python -I Tools/pm115_turn.py <capture> [<capture> ...] [--from .15 --to .40] [--rows]"""
import os, re, sys
import numpy as np

args = sys.argv[1:]
def opt(n, d):
    return float(args[args.index('--' + n) + 1]) if '--' + n in args else d
T0, T1, ROWS = opt('from', .15), opt('to', .40), '--rows' in args
caps = [a for i, a in enumerate(args) if not a.startswith('--') and (i == 0 or args[i - 1] not in ('--from', '--to'))]
num = lambda s: float(s.replace(',', '.'))
FLOOR = .06

for cap in caps:
    quads = {}
    for line in open(os.path.join(cap, 'log.txt'), encoding='utf8', errors='replace'):
        m = re.match(r'^[\d.]+ \[cerceve\] ([\d.,-]+) e [\d.,-]+ off [\d.,-]+ q ((?:[\d.,-]+ ?){8})', line)
        if m: quads.setdefault(round(num(m.group(1)), 4), np.array([num(x) for x in m.group(2).split()]).reshape(4, 2))
    ts = np.array(sorted(quads)); Q = np.array([quads[t] for t in ts])
    top = Q[:, 1] - Q[:, 0]; turn = np.degrees(np.arctan2(top[:, 1], top[:, 0])); x = Q.mean(1)[:, 0]
    dt = np.diff(ts) * 60
    step = {'turn': np.diff(turn) / dt, 'x': np.diff(x) / dt}   # per 1/60 s, at ts[1:]
    name = cap.replace('\\', '/').rstrip('/').split('/')[-1]
    sel = (ts[1:] >= T0) & (ts[1:] <= T1)
    out = []
    for k, v in step.items():
        acc = np.abs(np.diff(v))   # the change of the step, at ts[2:]
        worst = (0, 0, 0)
        for i in range(1, len(acc) - 1):
            if not (T0 <= ts[i + 2] <= T1): continue
            nb = (acc[i - 1] + acc[i + 1]) / 2
            r = acc[i] / max(nb, 1e-6) if acc[i] > FLOOR else 0
            if r > worst[0]: worst = (r, ts[i + 2], acc[i])
        out.append(f'{k}: total {v[sel].sum() if k == "x" else (turn[ts <= T1][-1] - turn[ts >= T0][0]):+.2f}, most step {np.abs(v[sel]).max():.2f}, '
                   f'most change {np.abs(np.diff(v))[sel[1:]].max():.2f}, worst x{worst[0]:.1f} of its neighbours at {worst[1]:.3f} s ({worst[2]:.2f})')
    print(f'[turn] {name}: ' + '; '.join(out))
    if ROWS:
        for i in np.where(sel)[0]:
            print(f'  {ts[i + 1]:.3f}  turn {step["turn"][i]:+.2f}  x {step["x"][i]:+.2f}')
