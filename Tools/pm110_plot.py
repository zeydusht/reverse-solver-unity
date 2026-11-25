"""PM_110: docs/art/pm110/hiz.png, the menu dive's speed on the screen (pm92_flow_check.py --csv: the wall's motion,
screen heights per second) against time from the press, PM_109 against PM_110.
  top     the targets: Zeyd's second and third drawings as PM_109 and PM_110 put them (straight lines through their points) and
          the designs (Dive75.Speed109 / Speed110), each scaled to its measured way
  bottom  measured, PM_109 (&dalis110=0) against PM_110
usage: python -I Tools/pm110_plot.py <out.png> <pm109.csv> <pm110.csv> [--peak 1.65] [--peak110 2]"""
import sys
import numpy as np
import matplotlib; matplotlib.use('Agg')
import matplotlib.pyplot as plt

argv = sys.argv[1:]
opt = lambda n, d: float(argv[argv.index('--' + n) + 1]) if '--' + n in argv else d
out, a_csv, b_csv = argv[:3]
PEAK = opt('peak', 1.65); EVEN = 1 / PEAK; EASE, LAND = .03, .85
PEAK110 = opt('peak110', 2); LOW = 1 / PEAK110


def h3(p0, m0, p1, m1, u):
    u = np.clip(u, 0, 1)
    return (2 * u ** 3 - 3 * u ** 2 + 1) * p0 + (u ** 3 - 2 * u ** 2 + u) * m0 + (-2 * u ** 3 + 3 * u ** 2) * p1 + (u ** 3 - u ** 2) * m1


def tail(x, T, v):
    te = 1 - EASE / T
    return np.where(x < te, v, h3(EVEN, 0, EVEN * LAND, 0, (x - te) / (1 - te)))


def speed108(x, T):   # Dive75.Speed108
    return tail(x, T, np.where(x < .25, h3(.55, 1.5 * .45, 1, 0, x / .25), np.where(x < .45, 1,
                np.where(x < .60, h3(1, 0, EVEN, 0, (x - .45) / .15), EVEN))))


def speed109(x, T):   # Dive75.Speed109
    L = .2
    return tail(x, T, np.where(x < .10, h3(.40, 1.5 * .45, .85, L * .1 / .55, x / .10),
                np.where(x < .65, h3(.85, L, 1, 0, (x - .10) / .55), np.where(x < .72, 1,
                np.where(x < .82, h3(1, 0, EVEN, 0, (x - .72) / .10), EVEN)))))


def speed110(x, T):   # Dive75.Speed110
    te = 1 - EASE / T; lift = LOW * .1 / .18
    last = lambda u: LOW + lift * (u - .82)
    return np.where(x < .08, h3(.90, 1.5 * .07, .97, 0, x / .08), np.where(x < .55, .97,
           np.where(x < .62, h3(.97, 0, 1, 0, (x - .55) / .07), np.where(x < .82, h3(1, 0, LOW, lift * .2, (x - .62) / .2),
           np.where(x < te, last(x), h3(last(te), lift * (1 - te), last(te) * LAND, 0, (x - te) / (1 - te)))))))


zeyd110 = lambda x: np.interp(x, [0, .08, .55, .62, .82, 1], [.90, .97, .97, 1, .50, .55])
zeyd108 = lambda x: np.interp(x, [0, .25, .45, .60, 1], [.55, 1, 1, EVEN, EVEN])
zeyd109 = lambda x: np.interp(x, [0, .10, .65, .72, .82, 1], [.40, .85, 1, 1, .60, .60])


def read(f):
    a = np.genfromtxt(f, delimiter=',', names=True)
    return a['t'], a['flow_smooth']


ta, va = read(a_csv); tb, vb = read(b_csv)
INK, MUTED, GRID = '#0b0b0b', '#52514e', '#e6e5e0'
ORANGE, BLUE = '#eb6834', '#2a78d6'
plt.rcParams.update({'font.family': 'Arial', 'font.size': 11, 'axes.edgecolor': MUTED, 'axes.labelcolor': MUTED,
                     'xtick.color': MUTED, 'ytick.color': MUTED, 'axes.spines.top': False, 'axes.spines.right': False})
fig, (ax1, ax2) = plt.subplots(2, 1, figsize=(9, 8.6), dpi=130, sharex=True, facecolor='#fcfcfb')
for ax in (ax1, ax2):
    ax.set_facecolor('#fcfcfb'); ax.grid(axis='y', color=GRID, lw=.8); ax.set_axisbelow(True)
    ax.set_ylabel('ekran boyu / sn')
lines = []
for (tt, vv, design, drawing, color, name) in ((ta, va, speed109, zeyd109, ORANGE, 'PM_109'), (tb, vb, speed110, zeyd110, BLUE, 'PM_110')):
    T = tt[-1]; x = np.linspace(0, 1, 601); D = np.trapezoid(vv, tt)
    lam = D / np.trapezoid(design(x, T), x * T)
    ax1.plot(x * T, lam * drawing(x), color=color, lw=7, alpha=.22, solid_capstyle='round', label=f'{name}: Zeyd\'in çizimi')
    ax1.plot(x * T, lam * design(x, T), color=color, lw=1.6, ls=(0, (4, 3)), label=f'{name}: tasarım')
    ax2.plot(tt, vv, color=color, lw=2, label=f'{name} ölçülen: {T:.2f} sn')
    ax2.plot([T], [vv[-1]], 'o', ms=7, color=color, mec='#fcfcfb', mew=2)
    lines.append((name, T, lam, vv))
ymax = max(va.max(), vb.max(), *(l[2] for l in lines)) * 1.22
ax1.set_title('Hedef: Zeyd\'in 2. ve 3. çizimi, tasarım', loc='left', color=INK, fontsize=13, fontweight='bold')
ax1.set_ylim(0, ymax); ax1.legend(loc='lower center', frameon=False, ncol=2, fontsize=10)
ax2.set_title('Ölçülen: PM_109 ve PM_110', loc='left', color=INK, fontsize=13, fontweight='bold')
ax2.set_ylim(0, ymax); ax2.legend(loc='lower center', frameon=False, ncol=2, fontsize=10)
ax2.set_xlabel("Oyna'ya basıştan sonra (sn): yay + yaklaşma, tabloya değene kadar")
ax2.set_xlim(0, max(ta[-1], tb[-1]) * 1.03)
fig.tight_layout()
fig.savefig(out, facecolor=fig.get_facecolor())
for name, T, lam, vv in lines:
    print(f'[plot] {name}: {T:.2f} s, design peak {lam:.3f}; measured start {vv[0]:.3f} peak {vv.max():.3f} end {vv[-1]:.3f}')
