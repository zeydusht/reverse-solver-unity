"""PM_91 §2: the dive's new approach curves (speed over time), printed as the constants Dive75.cs uses, and the speed
graph old against new (docs/art/pm91/hiz.png) with the speeds measured on the screen (pm91_timing_check.py --csv).

Each curve: the old one's own start up to t1 (the same lift), then a quartic in the time that keeps the speed and the
acceleration where they were at t1, peaks at the share `peak` of the time and lands at `land` x the peak (no slowing
tail: the old ones come to rest). Times in units of the old curve's length (the menu's BellSeconds, the corridor's
CorridorApproach), so &dalishiz= scales them alike.
usage: python Tools/pm91_curve.py [--plot out.png menu_old.csv menu_new.csv win_old.csv win_new.csv]"""
import sys
import numpy as np


def old_menu(x):   # Dive75.Launch's speed, x = t / T0
    return (x + .04) * (1 - x) ** 2 / (1 / 12 + .04 / 3)


def old_menu_s(x):
    c = .04
    return (x * x / 2 - 2 * x ** 3 / 3 + x ** 4 / 4 + c * (x - x * x + x ** 3 / 3)) / (1 / 12 + c / 3)


def old_win(x):    # the corridor's Hermite5 (its end speed, the hold's slow push, is ~0)
    return 30 * x * x * (1 - x) ** 2


def old_win_s(x):
    return 6 * x ** 5 - 15 * x ** 4 + 10 * x ** 3


def design(v, s, t1, T, land, path_peak=.77):
    """the quartic after t1 (in x = t - t1) for which the peak falls at path_peak of the way"""
    def solve(tp):
        v1 = v(t1); a1 = (v(t1 + 1e-6) - v(t1 - 1e-6)) / 2e-6; rest = 1 - s(t1)
        L, xp = T - t1, tp - t1
        A = np.array([[1, 0, 0, 0, 0], [0, 1, 0, 0, 0], [0, 1, 2 * xp, 3 * xp ** 2, 4 * xp ** 3],
                      [1 - land, L - land * xp, L ** 2 - land * xp ** 2, L ** 3 - land * xp ** 3, L ** 4 - land * xp ** 4],
                      [L, L ** 2 / 2, L ** 3 / 3, L ** 4 / 4, L ** 5 / 5]])
        return np.linalg.solve(A, [v1, a1, 0, 0, rest])
    best = None
    for tp in np.linspace(t1 + .02 * T, .95 * T, 400):
        c = solve(tp); t, vv, ss = curve(v, s, t1, T, c)
        ip = vv.argmax()
        if np.any(np.diff(vv[:ip]) < -1e-7) or np.any(np.diff(vv[ip:]) > 1e-7): continue
        if best is None or abs(ss[ip] - path_peak) < abs(best[1] - path_peak): best = (c, ss[ip])
    return best[0]


def curve(v, s, t1, T, c, n=4001):
    t = np.linspace(0, T, n)
    vv = np.where(t < t1, v(t), np.polyval(c[::-1], t - t1))
    x = np.clip(t - t1, 0, None)
    ss = np.where(t < t1, s(t), s(t1) + sum(c[k] * x ** (k + 1) / (k + 1) for k in range(5)))
    return t, vv, ss


MENU = dict(v=old_menu, s=old_menu_s, t1=.15 / 2.2, T=1.6 / 2.2, land=.75)
WIN = dict(v=old_win, s=old_win_s, t1=.12 / .6, T=.55 / .6, land=.75)

if __name__ == '__main__':
    out = {}
    for name, d in (('menu', MENU), ('win', WIN)):
        c = design(**d); out[name] = c
        t, v, s = curve(d['v'], d['s'], d['t1'], d['T'], c)
        ip = v.argmax(); i90 = np.searchsorted(s, .9)
        print(f'{name}: T {d["T"]:.4f} t1 {d["t1"]:.4f}; peak {v[ip]:.3f} at t {t[ip] / d["T"]:.0%} of the time, {s[ip]:.0%} of the way; '
              f'lands at {v[-1] / v[ip]:.2f} x the peak, last 10 % of the way -{1 - v[-1] / v[i90]:.0%}; s(T) {s[-1]:.5f}')
        print('  ' + ', '.join(f'{x:.6f}f' for x in c))
    if '--plot' in sys.argv:
        import matplotlib; matplotlib.use('Agg'); import matplotlib.pyplot as plt
        a = sys.argv[sys.argv.index('--plot') + 1:]
        png, csvs = a[0], a[1:5]
        fig, ax = plt.subplots(2, 2, figsize=(11, 7.5))
        for col, (name, d, T0) in enumerate((('Menü → bölüm', MENU, 2.2), ('Kazanma → koridor → bölüm', WIN, .6))):
            told = np.linspace(0, T0, 800); c = out['menu' if col == 0 else 'win']
            t, v, s = curve(d['v'], d['s'], d['t1'], d['T'], c)
            pk = d['v'](told / T0).max() / T0
            g = ax[0][col]
            g.plot(told, d['v'](told / T0) / T0 / pk, color='#8a8f98', lw=2, label=f'eski ({T0:.2f} sn)')
            g.plot(t * T0, v / T0 / pk, color='#d1495b', lw=2.4, label=f'yeni ({d["T"] * T0:.2f} sn)')
            g.set_title(f'{name}: kameranın hızı (tasarım)', fontsize=11)
            g.set_xlabel('dokunuştan saniye' if col == 0 else 'koridordan sonra saniye'); g.set_ylabel('hız (eskinin tepesi = 1)')
            g.set_ylim(0, 1.15); g.grid(alpha=.3); g.legend(loc='upper right')
            g = ax[1][col]
            for f, color, lab in ((csvs[2 * col], '#8a8f98', 'eski'), (csvs[2 * col + 1], '#d1495b', 'yeni')):
                m = np.genfromtxt(f, delimiter=',', names=True)
                k = np.ones(5) / 5
                g.plot(m['t'][2:-2], np.convolve(m['zoom'], k, 'valid'), color=color, lw=2, label=lab)
            g.set_title('ölçülen: resmin ekranda büyüme hızı', fontsize=11)
            g.set_xlabel('saniye'); g.set_ylabel('ln(boy) / sn'); g.grid(alpha=.3); g.legend(loc='upper left')
        fig.tight_layout(); fig.savefig(png, dpi=110)
        print('wrote', png)
