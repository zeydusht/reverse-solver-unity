"""PM_112: docs/art/pm112/hiz.png, the screen speed of the menu dive through the turn into the pull back, vol 2
(PM_111's .07 s hold) against the bungee (&bungee= 2 / 4 / 6), signed: + into the picture, - out of it. The way in
from the [cerceve] lines (Flow92, as pm92_flow_check.py), the hold and the pull back from the camera ([geri] kamera,
the same 9 x 19 grid). Both on the dive's clock.
  top     the whole of it, from the press to the board settled
  bottom  the arrival and the turn up close (time from the arrival)
usage: python -I Tools/pm112_plot.py <out.png> <capture>:<label> ...   (each capture with &dalislog=1)"""
import os, re, sys
import numpy as np
import cv2
import matplotlib; matplotlib.use('Agg')
import matplotlib.pyplot as plt

out, specs = sys.argv[1], sys.argv[2:]
num = lambda s: float(s.replace(',', '.'))
W0, H0 = 390.0, 844.0
gx, gy = np.meshgrid(np.linspace(0, W0, 9), np.linspace(0, H0, 19))
GRID = np.stack([gx.ravel(), gy.ravel(), np.ones(gx.size)])


def series(cap):
    fr, cam = [], []
    for line in open(os.path.join(cap, 'log.txt'), encoding='utf8', errors='replace'):
        m = re.search(r'\[cerceve\] ([\d.,-]+) e [\d.,-]+ off [\d.,-]+ q ((?:[\d.,-]+ ?){8})', line)
        if m:
            t = num(m.group(1))
            if not fr or t > fr[-1][0] + 1e-4: fr.append((t, np.float32([num(x) for x in m.group(2).split()]).reshape(4, 2)))
        m = re.search(r'\[geri\] kamera ([\d.,-]+) ([\d.,-]+) ([\d.,-]+) ([\d.,-]+) (\d+) (\d+)', line)
        if m: cam.append([num(m.group(i)) for i in range(1, 7)])
    ts, vs = [], []
    for (t0, q0), (t1, q1) in zip(fr, fr[1:]):   # the way in (Flow92)
        M = cv2.getPerspectiveTransform(q0, q1); p = M @ GRID; p = p[:2] / p[2]
        ts.append((t0 + t1) / 2); vs.append(np.hypot(p[0] - GRID[0], p[1] - GRID[1]).mean() / H0 / (t1 - t0))
    arrive = fr[-1][0]
    cam = np.array(cam); W, H = cam[0, 4], cam[0, 5]
    P = np.stack([(gx - W0 / 2).ravel() * W / W0, (gy - H0 / 2).ravel() * H / H0], 1)
    prev = None
    for row in cam:   # the hold and the pull back (the camera)
        t1, s1, x1, y1 = row[:4]
        if prev is not None and t1 > prev[0] + 1e-4:
            t0, s0, x0, y0 = prev
            Lx, Ly = P[:, 0] / s0 + x0, P[:, 1] / s0 - y0
            d = np.hypot(s1 * (Lx - x1) - P[:, 0], s1 * (Ly + y1) - P[:, 1]).mean() / H / (t1 - t0)
            ts.append((t0 + t1) / 2); vs.append(d if s1 >= s0 else -d)
        prev = (t1, s1, x1, y1)
    ts, vs = np.array(ts), np.array(vs)
    o = np.argsort(ts)
    # the turn: from the arrival until the camera is back at the arrival's size (s: the picture's scale)
    # (the first camera frame is already past the arrival: its size taken back to the arrival along the first two)
    r = np.log(cam[1, 1] / cam[0, 1]) / max(1e-4, cam[1, 0] - cam[0, 0])
    s_arr = cam[0, 1] * np.exp(-r * (cam[0, 0] - arrive)); back = cam[(cam[:, 1] < s_arr) & (cam[:, 0] > cam[0, 0] + .02), 0]
    turn = (back[0] - arrive) if len(back) else float('nan')
    return ts[o], vs[o], arrive, cam[-1, 0], turn, cam[:, 1].max() / s_arr - 1


INK, MUTED, GRIDC = '#0b0b0b', '#52514e', '#e6e5e0'
COLORS = ['#8f8d87', '#2a78d6', '#1baf7a', '#eb6834']
plt.rcParams.update({'font.family': 'Arial', 'font.size': 11, 'axes.edgecolor': MUTED, 'axes.labelcolor': MUTED,
                     'xtick.color': MUTED, 'ytick.color': MUTED, 'axes.spines.top': False, 'axes.spines.right': False})
fig, (ax1, ax2) = plt.subplots(2, 1, figsize=(9, 9), dpi=130, facecolor='#fcfcfb')
for ax in (ax1, ax2):
    ax.set_facecolor('#fcfcfb'); ax.grid(axis='y', color=GRIDC, lw=.8); ax.set_axisbelow(True)
    ax.axhline(0, color=MUTED, lw=.8)
    ax.set_ylabel('ekran boyu / sn  (+ içeri, − dışarı)')
for k, spec in enumerate(specs):
    cap, label = spec.rsplit(':', 1)
    t, v, arrive, end, turn, deep = series(cap)
    c = COLORS[k % len(COLORS)]
    ax1.plot(t, v, color=c, lw=2, label=f'{label}: bitiş {end:.2f} sn')
    ax1.plot([end], [v[-1]], 'o', ms=6, color=c, mec='#fcfcfb', mew=2)
    m = (t > arrive - .15) & (t < arrive + .35)
    ax2.plot(t[m] - arrive, v[m], color=c, lw=2, marker='o', ms=3, label=label)
    # the speed's largest change from one frame to the next around the turn (a kink shows as a jump)
    mm = (t > arrive - .1) & (t < arrive + .35)
    jump = np.abs(np.diff(v[mm])).max() if mm.sum() > 1 else 0
    w = (t > arrive) & (t < arrive + .3); sg = np.sign(v[w]); flat = int((np.abs(v[w]) < .005).sum())
    cross = int((sg[1:] != sg[:-1]).sum())
    print(f'[plot] {label}: arrival {arrive:.3f} s, end {end:.3f} s, turn (back at the arrival size) {turn:.3f} s, deepest +{deep * 100:.1f} %, '
          f'zero crossings {cross}, frames under .005 {flat}, fastest in after arrival {v[w].max():.3f}, fastest out {v.min():.3f}, '
          f'largest step near the turn {jump:.3f} screen heights/s')
ax1.set_title('Gidiş → dönüş → geri çekilme', loc='left', color=INK, fontsize=13, fontweight='bold')
ax1.set_xlabel("Oyna'ya basıştan sonra (sn)")
ax1.legend(loc='lower left', frameon=False, fontsize=9.5)
ax2.axvline(0, color=MUTED, lw=.8, ls=':')
ax2.text(.003, .97, 'varış', color=MUTED, fontsize=9.5, transform=ax2.get_xaxis_transform(), va='top')
ax2.set_title('Varış ve dönüş, yakın plan (her nokta bir kare)', loc='left', color=INK, fontsize=13, fontweight='bold')
ax2.set_xlabel('varıştan sonra (sn)')
ax2.legend(loc='lower left', frameon=False, fontsize=9.5)
fig.tight_layout()
fig.savefig(out, facecolor=fig.get_facecolor())
