"""PM_108: docs/art/pm108/hiz.png, the menu dive's speed on the screen (pm92_flow_check.py --csv: the wall's motion,
screen heights per second) against time from the press.
  top     the new: Zeyd's drawing (the shape he asked for, PM_108's points, scaled to the measured way), the design
          (Dive75.Speed108 over the time, the same way) and what was measured
  bottom  measured, now (PM_92: &dalis108=0) against the new
usage: python -I Tools/pm108_plot.py <out.png> <old.csv> <new.csv> [--T 1.2] [--peak 1.65]"""
import sys
import numpy as np
import matplotlib; matplotlib.use('Agg')
import matplotlib.pyplot as plt

argv = sys.argv[1:]
opt = lambda n, d: float(argv[argv.index('--' + n) + 1]) if '--' + n in argv else d
out, old_csv, new_csv = argv[:3]
T, PEAK = opt('T', 1.2), opt('peak', 1.65)
START, RISE, HOLD, FALL, EASE, LAND = .55, .25, .45, .60, .03, .85   # Dive75.cs Start108 … EndLand108


def h3(p0, m0, p1, m1, u):
    return (2 * u ** 3 - 3 * u ** 2 + 1) * p0 + (u ** 3 - 2 * u ** 2 + u) * m0 + (-2 * u ** 3 + 3 * u ** 2) * p1 + (u ** 3 - u ** 2) * m1


def speed108(x):
    even, te = 1 / PEAK, 1 - EASE / T
    return np.where(x < RISE, h3(START, 1.5 * (1 - START), 1, 0, x / RISE), np.where(x < HOLD, 1,
           np.where(x < FALL, h3(1, 0, even, 0, np.clip((x - HOLD) / (FALL - HOLD), 0, 1)),
           np.where(x < te, even, h3(even, 0, even * LAND, 0, np.clip((x - te) / (1 - te), 0, 1))))))


def zeyd(x):
    """PM_108's points: 55 % at once, the peak from 25 % to 45 %, 60 % of it by 60 %, even to the end (straight lines)"""
    return np.interp(x, [0, RISE, HOLD, FALL, 1], [START, 1, 1, 1 / PEAK, 1 / PEAK])


def read(f):
    a = np.genfromtxt(f, delimiter=',', names=True)
    return a['t'], a['flow_smooth']


to, vo = read(old_csv); tn, vn = read(new_csv)
D = np.trapezoid(vn, tn)   # the measured way, screen heights
x = np.linspace(0, 1, 601); t = x * T
lam = D / np.trapezoid(speed108(x), t)

INK, MUTED, GRID = '#0b0b0b', '#52514e', '#e6e5e0'
BLUE, ORANGE, AQUA = '#2a78d6', '#eb6834', '#1baf7a'
plt.rcParams.update({'font.family': 'Arial', 'font.size': 11, 'axes.edgecolor': MUTED, 'axes.labelcolor': MUTED,
                     'xtick.color': MUTED, 'ytick.color': MUTED, 'axes.spines.top': False, 'axes.spines.right': False})
fig, (ax1, ax2) = plt.subplots(2, 1, figsize=(9, 8.6), dpi=130, sharex=True, facecolor='#fcfcfb')
for ax in (ax1, ax2):
    ax.set_facecolor('#fcfcfb'); ax.grid(axis='y', color=GRID, lw=.8); ax.set_axisbelow(True)
    ax.set_ylabel('ekran boyu / sn')
ymax = max(vn.max(), vo.max(), lam) * 1.18

ax1.set_title('Yeni (PM_108): istenen, tasarım ve ölçülen', loc='left', color=INK, fontsize=13, fontweight='bold')
ax1.axvspan(RISE * T, HOLD * T, color=BLUE, alpha=.06, lw=0)
ax1.text((RISE + HOLD) / 2 * T, ymax * .965, 'tepe', ha='center', va='top', color=MUTED, fontsize=10)
ax1.axvspan(FALL * T, T, color=AQUA, alpha=.06, lw=0)
ax1.text((FALL + 1) / 2 * T, ymax * .965, 'sabit yaklaşma', ha='center', va='top', color=MUTED, fontsize=10)
ax1.plot(t, lam * zeyd(x), color=AQUA, lw=6, alpha=.28, solid_capstyle='round', label="Zeyd'in istediği şekil")
ax1.plot(t, lam * speed108(x), color=MUTED, lw=1.6, ls=(0, (4, 3)), label='tasarım (Dive75.Speed108)')
ax1.plot(tn, vn, color=BLUE, lw=2, label='ölçülen (yeni)')
ax1.set_ylim(0, ymax); ax1.legend(loc='lower center', frameon=False, ncol=3, fontsize=10)

ax2.set_title('Ölçülen: şimdiki (PM_92) ve yeni', loc='left', color=INK, fontsize=13, fontweight='bold')
ax2.plot(to, vo, color=ORANGE, lw=2, label=f'şimdiki: {to[-1]:.2f} sn')
ax2.plot(tn, vn, color=BLUE, lw=2, label=f'yeni: {tn[-1]:.2f} sn')
for tt, vv, c in ((to, vo, ORANGE), (tn, vn, BLUE)):
    ax2.plot([tt[-1]], [vv[-1]], 'o', ms=7, color=c, mec='#fcfcfb', mew=2)
ax2.set_ylim(0, ymax); ax2.legend(loc='lower center', frameon=False, ncol=2, fontsize=10)
ax2.set_xlabel("Oyna'ya basıştan sonra (sn): yay + yaklaşma, tabloya değene kadar")
ax2.set_xlim(0, max(to[-1], tn[-1]) * 1.03)
fig.tight_layout()
fig.savefig(out, facecolor=fig.get_facecolor())
print(f'[plot] {out}: way {D:.2f} screen heights; design peak {lam:.2f}, even {lam / PEAK:.2f}; measured new peak {vn.max():.2f}, start {vn[0]:.2f}, end {vn[-1]:.2f}')
