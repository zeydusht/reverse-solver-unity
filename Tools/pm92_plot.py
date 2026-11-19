"""PM_92: docs/art/pm92/hiz.png, the dive's speed as the eye sees it (pm92_flow_check.py --csv: the wall's motion on the
screen, screen heights per second). Top: PM_92's design (Dive75.Plan92's speed, rebuilt here from the measured way
and start) against what was measured. Bottom: measured, PM_91 against PM_92.
usage: python Tools/pm92_plot.py <out.png> <menu_pm91.csv> <menu_pm92.csv> <win_pm91.csv> <win_pm92.csv>"""
import sys
import numpy as np
import matplotlib; matplotlib.use('Agg')
import matplotlib.pyplot as plt

PEAK, LAND = .95, .87   # Dive75.cs Peak92, Land92; RiseFrom92 / Rise92 (menu), CorridorRiseFrom92 / CorridorRise92
SHAPE = {True: (.35, 1.5), False: (.2, 1 + 2 * (.95 - .2) / .2)}


def h3(p0, m0, p1, m1, u):
    return (2 * u ** 3 - 3 * u ** 2 + 1) * p0 + (u ** 3 - 2 * u ** 2 + u) * m0 + (-2 * u ** 3 + 3 * u ** 2) * p1 + (u ** 3 - u ** 2) * m1


def design(D, T, v0, a0, menu, n=600):
    RISE_FROM, RISE = SHAPE[menu]
    t = np.linspace(0, T, n + 1); t1, t2 = RISE_FROM * T, PEAK * T; m = (RISE - 1) / (t2 - t1)

    def speeds(lam, sv0, sa0):
        return np.where(t < t1, h3(sv0, sa0 * t1, lam, lam * m * t1, t / t1),
               np.where(t < t2, h3(lam, lam * m * (t2 - t1), lam * RISE, 0, (t - t1) / (t2 - t1)),
                        h3(lam * RISE, 0, lam * RISE * LAND, 0, (t - t2) / (T - t2))))
    lam = (D - np.trapezoid(speeds(0, v0, a0), t)) / np.trapezoid(speeds(1, 0, 0), t)
    return t, speeds(lam, v0, a0)


def load(f):
    return np.genfromtxt(f, delimiter=',', names=True)


out, m91, m92, w91, w92 = sys.argv[1:6]
fig, ax = plt.subplots(2, 2, figsize=(11, 7.5))
for col, (name, f91, f92, T, menu) in enumerate((('Menü → bölüm', m91, m92, 1.5, True), ('Kazanma → koridor → bölüm', w91, w92, .5, False))):
    a, b = load(f91), load(f92)
    D = float(np.sum(b['flow'] * np.gradient(b['t'])))   # the way's screen distance, as measured
    v0 = float(b['flow'][0]) if menu else 0.0
    a0 = float(max(0, (np.interp(.2, b['t'], b['flow']) - v0) / .2)) if menu else 0.0
    t, v = design(D, T, v0, a0, menu)
    g = ax[0][col]
    g.plot(t, v, color='#d1495b', lw=2.4, label='PM_92 tasarım')
    g.plot(b['t'], b['flow'], 'o', ms=3.5, color='#1d3557', label='PM_92 ölçülen (kare kare)')
    g.set_title(f'{name}: ekranda hissedilen hız', fontsize=11)
    g.set_xlabel('saniye'); g.set_ylabel('ekran boyu / sn'); g.grid(alpha=.3); g.legend(loc='upper left')
    g.set_ylim(0, max(v.max(), b['flow'].max()) * 1.15)
    g = ax[1][col]
    k = 3
    sm = lambda x: np.convolve(np.pad(x, k // 2, mode='edge'), np.ones(k) / k, 'valid')
    g.plot(a['t'], sm(a['flow']), color='#8a8f98', lw=2, label=f'PM_91 ({a["t"][-1]:.2f} sn)')
    g.plot(b['t'], sm(b['flow']), color='#d1495b', lw=2.4, label=f'PM_92 ({b["t"][-1]:.2f} sn)')
    ip = int(np.argmax(sm(a['flow'])))
    if ip < len(a['t']) * .7:
        g.annotate('PM_91: tepe, sonra düşüş', (a['t'][ip], sm(a['flow'])[ip]), xytext=(a['t'][ip] + .05 * T, sm(a['flow'])[ip] * 1.12),
                   arrowprops=dict(arrowstyle='->', color='#555'), fontsize=9, color='#555')
    g.set_title('ölçülen: PM_91 ve PM_92', fontsize=11)
    g.set_xlabel('saniye'); g.set_ylabel('ekran boyu / sn'); g.grid(alpha=.3); g.legend(loc='upper left')
    g.set_ylim(0, max(sm(a['flow']).max(), sm(b['flow']).max()) * 1.25)
fig.suptitle('Hissedilen hız = duvarın ekrandaki ortalama kayması (büyüme + yay boyunca kayma birlikte)', fontsize=10, y=.995)
fig.tight_layout(); fig.savefig(out, dpi=110)
print('wrote', out)
