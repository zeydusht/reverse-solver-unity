"""PM_110: docs/art/pm110/hiz.png, the menu dive's speed on the screen (pm92_flow_check.py --csv: Flow92, screen
heights per second) against the time from the press, for PM_109 and the two variants of PM_110's real aim (the way
in at least as quick as the way back): A, Zeyd's third drawing, and B, the pull back backwards, both at the pull
back's mean speed. The pull back's own speed (its camera, [geri] kamera lines, measured the same way) drawn too.
  top     the targets (Dive75.Speed110 / SpeedBack110 scaled to the measured way) and the measured A and B, the pull
          back backwards beside B
  bottom  measured: PM_109, A, B and the pull back (on its own clock), its mean as a line
usage: python -I Tools/pm110_plot.py <out.png> <pm109 capture> <A capture> <B capture> [--peak110 2]"""
import os, re, sys
import numpy as np
import matplotlib; matplotlib.use('Agg')
import matplotlib.pyplot as plt

argv = sys.argv[1:]
opt = lambda n, d: float(argv[argv.index('--' + n) + 1]) if '--' + n in argv else d
out, c109, ca, cb = argv[:4]
LOW = 1 / opt('peak110', 2); EASE, LAND = .03, .85
BACK = [.07, .10, .30, .52, .68, .81, .91, .97, 1.00, .96, .92, .92, .90, .87, .80, .72, .63, .53, .43, .31, .16]   # Dive75.BackSpeed110
num = lambda s: float(s.replace(',', '.'))


def h3(p0, m0, p1, m1, u):
    u = np.clip(u, 0, 1)
    return (2 * u ** 3 - 3 * u ** 2 + 1) * p0 + (u ** 3 - 2 * u ** 2 + u) * m0 + (-2 * u ** 3 + 3 * u ** 2) * p1 + (u ** 3 - u ** 2) * m1


def speed110(x, T):   # Dive75.Speed110
    te = 1 - EASE / T; lift = LOW * .1 / .18
    last = lambda u: LOW + lift * (u - .82)
    return np.where(x < .08, h3(.90, 1.5 * .07, .97, 0, x / .08), np.where(x < .55, .97,
           np.where(x < .62, h3(.97, 0, 1, 0, (x - .55) / .07), np.where(x < .82, h3(1, 0, LOW, lift * .2, (x - .62) / .2),
           np.where(x < te, last(x), h3(last(te), lift * (1 - te), last(te) * LAND, 0, (x - te) / (1 - te)))))))


def speed_back(x):   # Dive75.SpeedBack110 (Catmull-Rom over the table, read from the end)
    n = len(BACK) - 1; f = np.clip(1 - x, 0, 1) * n; i = np.minimum(n - 1, f.astype(int)); u = f - i
    P = lambda k: np.array(BACK)[np.clip(k, 0, n)]
    p0, p1, p2, p3 = P(i - 1), P(i), P(i + 1), P(i + 2)
    return np.maximum(.02, .5 * (2 * p1 + (p2 - p0) * u + (2 * p0 - 5 * p1 + 4 * p2 - p3) * u * u + (3 * p1 - p0 - 3 * p2 + p3) * u ** 3))


def measured(cap):
    a = np.genfromtxt(os.path.join(cap, 'hiz.csv'), delimiter=',', names=True)
    return a['t'], a['flow_smooth']


def pull_back(cap):
    """the pull back's speed from its camera, Flow92's grid (as Tools/pm110_ekran_hizi.py), on its own clock"""
    cam, gw = [], []
    for line in open(os.path.join(cap, 'log.txt'), encoding='utf8', errors='replace'):
        m = re.search(r'\[geri\] kamera ([\d.,-]+) ([\d.,-]+) ([\d.,-]+) ([\d.,-]+) (\d+) (\d+)', line)
        if m: cam.append([num(m.group(i)) for i in range(1, 7)])
        m = re.search(r'\[geri\] ([\d.,]+) w ([\d.,]+)', line)
        if m: gw.append((num(m.group(1)), num(m.group(2))))
    cam = np.array(cam); W, H = cam[0, 4], cam[0, 5]
    c0 = min(t for t, x in gw if x > 0); c1 = min(t for t, x in gw if x >= 1)
    gx, gy = np.meshgrid(np.linspace(-W / 2, W / 2, 9), np.linspace(-H / 2, H / 2, 19)); P = np.stack([gx.ravel(), gy.ravel()], 1)
    ts, vs = [], []
    for (t0, s0, x0, y0, _, _), (t1, s1, x1, y1, _, _) in zip(cam, cam[1:]):
        if t1 <= t0 or t0 < c0 - 1e-3 or t1 > c1 + 1e-3: continue
        Lx, Ly = P[:, 0] / s0 + x0, P[:, 1] / s0 - y0
        ts.append((t0 + t1) / 2 - c0); vs.append(np.hypot(s1 * (Lx - x1) - P[:, 0], s1 * (Ly + y1) - P[:, 1]).mean() / H / (t1 - t0))
    v = np.convolve(np.pad(vs, 1, mode='edge'), np.ones(3) / 3, 'valid')
    return np.array(ts), v, c1 - c0


t9, v9 = measured(c109); ta, va = measured(ca); tb, vb = measured(cb)
tp, vp, Tp = pull_back(ca)
mean_back = np.trapezoid(vp, tp) / Tp
INK, MUTED, GRID = '#0b0b0b', '#52514e', '#e6e5e0'
GRAY, BLUE, ORANGE, AQUA = '#8f8d87', '#2a78d6', '#eb6834', '#1baf7a'
plt.rcParams.update({'font.family': 'Arial', 'font.size': 11, 'axes.edgecolor': MUTED, 'axes.labelcolor': MUTED,
                     'xtick.color': MUTED, 'ytick.color': MUTED, 'axes.spines.top': False, 'axes.spines.right': False})
fig, (ax1, ax2) = plt.subplots(2, 1, figsize=(9, 8.8), dpi=130, sharex=True, facecolor='#fcfcfb')
for ax in (ax1, ax2):
    ax.set_facecolor('#fcfcfb'); ax.grid(axis='y', color=GRID, lw=.8); ax.set_axisbelow(True)
    ax.set_ylabel('ekran boyu / sn')
lines = []
for tt, vv, design, color, name in ((ta, va, lambda x, T: speed110(x, T), BLUE, 'A'), (tb, vb, lambda x, T: speed_back(x), ORANGE, 'B')):
    T = tt[-1]; x = np.linspace(0, 1, 601); D = np.trapezoid(vv, tt)
    lam = D / np.trapezoid(design(x, T), x * T)
    ax1.plot(x * T, lam * design(x, T), color=color, lw=6, alpha=.22, solid_capstyle='round', label=f'{name}: hedef')
    ax1.plot(tt, vv, color=color, lw=2, label=f'{name}: ölçülen')
    lines.append((name, T, D, vv))
Tb, Db = lines[1][1], lines[1][2]; Dp = np.trapezoid(vp, tp)   # the pull back backwards, at B's time and way
ax1.plot((Tp - tp[::-1]) * Tb / Tp, vp[::-1] * Db / Dp * Tp / Tb, color=GRAY, lw=1.4, ls=(0, (4, 3)), label="geri çekilme tersten, B'nin süresinde")
ymax = max(v9.max(), va.max(), vb.max(), vp.max()) * 1.2
ax1.set_title("Hedef ve ölçülen: A (Zeyd'in 3. çizimi) ve B (geri çekilmenin tersi)", loc='left', color=INK, fontsize=12.5, fontweight='bold')
ax1.set_ylim(0, ymax); ax1.legend(loc='upper right', frameon=False, ncol=1, fontsize=9.5)
ax2.set_title('Ölçülen: PM_109, A, B ve geri çekilme', loc='left', color=INK, fontsize=12.5, fontweight='bold')
ax2.plot(tp, vp, color=GRAY, lw=2, label=f'geri çekilme: {Tp:.2f} sn')
ax2.axhline(mean_back, color=GRAY, lw=1, ls=':', zorder=1)
ax2.text(.005, mean_back + .012, f'geri çekilmenin ortalaması {mean_back:.2f}', color=MUTED, fontsize=9.5, transform=ax2.get_yaxis_transform())
for tt, vv, color, name in ((t9, v9, AQUA, 'PM_109'), (ta, va, BLUE, 'A'), (tb, vb, ORANGE, 'B')):
    m = np.trapezoid(vv, tt) / tt[-1]
    ax2.plot(tt, vv, color=color, lw=2, label=f'{name}: {tt[-1]:.2f} sn, ort. {m:.2f}')
    ax2.plot([tt[-1]], [vv[-1]], 'o', ms=7, color=color, mec='#fcfcfb', mew=2)
ax2.set_ylim(0, ymax); ax2.legend(loc='upper right', frameon=False, ncol=1, fontsize=9.5)
ax2.set_xlabel("Oyna'ya basıştan sonra (sn): yay + yaklaşma, tabloya değene kadar (geri çekilme kendi başlangıcından)")
ax2.set_xlim(0, max(t9[-1], Tp) * 1.03)
fig.tight_layout()
fig.savefig(out, facecolor=fig.get_facecolor())
print(f'[plot] pull back {Tp:.2f} s, mean {mean_back:.3f}, peak {vp.max():.3f}')
for tt, vv, name in ((t9, v9, 'PM_109'), (ta, va, 'A'), (tb, vb, 'B')):
    print(f'[plot] {name}: {tt[-1]:.3f} s, way {np.trapezoid(vv, tt):.3f}, mean {np.trapezoid(vv, tt) / tt[-1]:.3f}, start {vv[0]:.3f}, peak {vv.max():.3f}, end {vv[-1]:.3f}')
