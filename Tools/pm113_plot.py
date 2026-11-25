"""PM_113: docs/art/pm113/yakinlik.png, the menu dive's closeness to the picture over time as in Zeyd's drawing (down:
closer; the board's framing a dashed line) and its screen speed under it, vol 2 against the U (&u=1 / 2 / 3), with
the numbers for RAPOR.md.
  closeness  the picture's size against the framing's (ln, shown as %): the way in from the [cerceve] quads, the
             bottom and the pull back from the camera ([geri] kamera), joined at the framing (the log's own time
             for it: vol 2 B's T, the U's "the framing at")
  speed      as pm112_plot.py (Flow92 on the way in, the camera's grid after), signed: + in, - out
  left       the whole of it, from the press to the board settled; right: the bottom up close (each dot a frame)
usage: python -I Tools/pm113_plot.py <out.png> <capture>:<label> ...   (each capture with &dalislog=1)"""
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
    fr, cam, text = [], [], open(os.path.join(cap, 'log.txt'), encoding='utf8', errors='replace').read()
    for line in text.splitlines():
        m = re.search(r'\[cerceve\] ([\d.,-]+) e [\d.,-]+ off [\d.,-]+ q ((?:[\d.,-]+ ?){8})', line)
        if m:
            t = num(m.group(1))
            if not fr or t > fr[-1][0] + 1e-4: fr.append((t, np.float32([num(x) for x in m.group(2).split()]).reshape(4, 2)))
        m = re.search(r'\[geri\] kamera ([\d.,-]+) ([\d.,-]+) ([\d.,-]+) ([\d.,-]+) (\d+) (\d+)', line)
        if m: cam.append([num(m.group(i)) for i in range(1, 7)])
    m = re.search(r'the framing at ([\d,]+) s', text)
    if m: ta = num(m.group(1))
    else: ta = num(re.search(r'PM_92: ([\d,]+) screen heights in', text).group(1)) / .56   # vol 2: B's T (PM_110)
    end = num(re.search(r'ended at ([\d,]+) s', text).group(1))
    lf = re.search(r'over 20 ms (\d+) \(max (\d+) ms\)', text)
    # the speed
    ts, vs = [], []
    for (t0, q0), (t1, q1) in zip(fr, fr[1:]):
        M = cv2.getPerspectiveTransform(q0, q1); p = M @ GRID; p = p[:2] / p[2]
        ts.append((t0 + t1) / 2); vs.append(np.hypot(p[0] - GRID[0], p[1] - GRID[1]).mean() / H0 / (t1 - t0))
    cam = np.array(cam); W, H = cam[0, 4], cam[0, 5]
    P = np.stack([(gx - W0 / 2).ravel() * W / W0, (gy - H0 / 2).ravel() * H / H0], 1)
    prev = None
    for row in cam:
        t1, s1, x1, y1 = row[:4]
        if prev is not None and t1 > prev[0] + 1e-4:
            t0, s0, x0, y0 = prev
            Lx, Ly = P[:, 0] / s0 + x0, P[:, 1] / s0 - y0
            d = np.hypot(s1 * (Lx - x1) - P[:, 0], s1 * (Ly + y1) - P[:, 1]).mean() / H / (t1 - t0)
            ts.append((t0 + t1) / 2); vs.append(d if s1 >= s0 else -d)
        prev = (t1, s1, x1, y1)
    ts, vs = np.array(ts), np.array(vs); o = np.argsort(ts); ts, vs = ts[o], vs[o]
    # the closeness (ln of the size over the framing's; + closer): the quads' heights, the camera's scale
    hq = np.array([np.log(((q[3] - q[0]) ** 2).sum() ** .5 / 2 + ((q[2] - q[1]) ** 2).sum() ** .5 / 2) for _, q in fr])
    tq = np.array([t for t, _ in fr])
    lq_a = hq[-1] + (hq[-1] - hq[-2]) / (tq[-1] - tq[-2]) * (ta - tq[-1])   # the quads at the framing
    ls = np.log(cam[:, 1]); ls_a = ls[0] - (ls[1] - ls[0]) / (cam[1, 0] - cam[0, 0]) * (cam[0, 0] - ta)
    tc = np.concatenate([tq, cam[:, 0]]); lc = np.concatenate([hq - lq_a, ls - ls_a])
    # the bottom: a parabola through the deepest camera frame and its neighbours
    i = int(np.argmax(ls)); i = min(max(i, 1), len(ls) - 2)
    a, b, c = np.polyfit(cam[i - 1:i + 2, 0], ls[i - 1:i + 2], 2)
    tb = -b / (2 * a) if a < 0 else cam[i, 0]; deep = (np.polyval([a, b, c], tb) if a < 0 else ls[i]) - ls_a
    # +.3 -> -.3 (on the frames' speed, between frames on a straight line)
    def cross(level, after, down=True):
        for k in range(1, len(ts)):
            if ts[k] > after and (vs[k - 1] > level >= vs[k] if down else vs[k - 1] < level <= vs[k]):
                return ts[k - 1] + (vs[k - 1] - level) / (vs[k - 1] - vs[k]) * (ts[k] - ts[k - 1])
        return float('nan')
    up = cross(.3, ts[np.argmax(vs)]); down = cross(-.3, up)
    zeros = int((np.sign(vs[(ts > ta - .2) & (ts < end - .05)])[1:] != np.sign(vs[(ts > ta - .2) & (ts < end - .05)])[:-1]).sum())
    return dict(ts=ts, vs=vs, tc=tc, lc=lc, ta=ta, tb=tb, deep=np.exp(deep) - 1, end=end, up=up, down=down, zeros=zeros,
                long=f'{lf.group(1)} ({lf.group(2)} ms)' if lf else '?')


INK, MUTED, GRIDC, BG = '#0b0b0b', '#52514e', '#e6e5e0', '#fcfcfb'
COLORS = ['#8f8d87', '#2a78d6', '#1baf7a', '#eb6834']
plt.rcParams.update({'font.family': 'Arial', 'font.size': 11, 'axes.edgecolor': MUTED, 'axes.labelcolor': MUTED,
                     'xtick.color': MUTED, 'ytick.color': MUTED, 'axes.spines.top': False, 'axes.spines.right': False})
fig, ax = plt.subplots(2, 2, figsize=(14, 9.5), dpi=120, facecolor=BG, gridspec_kw=dict(width_ratios=[1.25, 1]))
for a in ax.ravel(): a.set_facecolor(BG); a.grid(axis='y', color=GRIDC, lw=.8); a.set_axisbelow(True)
Z0, Z1 = .45, 1.05   # the close-up
rows = []
for k, spec in enumerate(specs):
    cap, label = spec.rsplit(':', 1)
    s = series(cap); c = COLORS[k % len(COLORS)]
    # closeness, down closer: plotted as -(ln size), the framing at 0 (the close-up in % of the size)
    ax[0, 0].plot(s['tc'], -s['lc'], color=c, lw=2, label=label)
    m = (s['tc'] > Z0) & (s['tc'] < Z1)
    ax[0, 1].plot(s['tc'][m], -(np.exp(s['lc'][m]) - 1) * 100, color=c, lw=2, marker='o', ms=3, label=label)
    ax[1, 0].plot(s['ts'], s['vs'], color=c, lw=2, label=label)
    m = (s['ts'] > Z0) & (s['ts'] < Z1)
    ax[1, 1].plot(s['ts'][m], s['vs'][m], color=c, lw=2, marker='o', ms=3, label=label)
    for a in (ax[0, 0], ax[1, 0]): a.plot([s['end']], [a.lines[-1].get_ydata()[-1]], 'o', ms=6, color=c, mec=BG, mew=2)
    rows.append((label, s))
    print(f"[plot] {label}: framing {s['ta']:.3f} s, bottom {s['tb']:.3f} s, {s['deep'] * 100:.1f} % in, +.3 at {s['up']:.3f} "
          f"-.3 at {s['down']:.3f}: {s['down'] - s['up']:.3f} s (bottom - up {s['tb'] - s['up']:.3f}, down - bottom {s['down'] - s['tb']:.3f}), "
          f"zero crossings {s['zeros']}, end {s['end']:.2f} s, long frames {s['long']}")
for a in (ax[0, 0], ax[0, 1]):
    a.axhline(0, color=MUTED, lw=1, ls='--')
    a.set_ylabel('tabloya uzaklık  (aşağısı daha yakın)')
ax[0, 0].text(.01, 0, ' tablonun kadrajı', color=MUTED, fontsize=9.5, va='bottom', transform=ax[0, 0].get_yaxis_transform())
ax[0, 0].set_yticks([])
ax[0, 1].set_ylabel('kadraja göre (%)  (aşağısı daha yakın)')
ax[0, 1].text(.01, 0, ' kadraj', color=MUTED, fontsize=9.5, va='bottom', transform=ax[0, 1].get_yaxis_transform())
ax[0, 1].set_ylim(-5.5, 12)
for a in (ax[1, 0], ax[1, 1]):
    a.axhline(0, color=MUTED, lw=.8)
    a.set_ylabel('hız, ekran boyu / sn  (+ içeri, − dışarı)')
for y in (.3, -.3): ax[1, 1].axhline(y, color=MUTED, lw=.8, ls=':')
ax[1, 1].set_ylim(-.65, .75)
ax[0, 0].set_title('Yakınlık: gidiş → dip → geri çekilme', loc='left', color=INK, fontsize=13, fontweight='bold')
ax[0, 1].set_title('Dip, yakın plan (her nokta bir kare)', loc='left', color=INK, fontsize=13, fontweight='bold')
ax[1, 0].set_title('Hız', loc='left', color=INK, fontsize=13, fontweight='bold')
ax[1, 1].set_title('Hız dipte: +0,3 → −0,3 (noktalı çizgiler)', loc='left', color=INK, fontsize=13, fontweight='bold')
for a in ax[:, 0]: a.set_xlim(0, 1.65); a.set_xlabel("Oyna'dan sonra (sn)")
for a in ax[:, 1]: a.set_xlim(Z0, Z1); a.set_xlabel("Oyna'dan sonra (sn)")
ax[0, 0].legend(loc='upper center', frameon=False, fontsize=10, ncol=4)
ax[1, 1].legend(loc='upper right', frameon=False, fontsize=9.5)
fig.tight_layout()
fig.savefig(out, facecolor=fig.get_facecolor())
