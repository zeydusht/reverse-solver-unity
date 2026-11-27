"""PM_116: docs/art/pm116/renk.png: the board's mean saturation and brightness frame by frame over the pull back (the
board's quad on the screen from the log's [dl] b, its inner 80 %, captured frames of Tools/pm108_dalis.js), the
captures one over the other; and each frame's change per 1/60 s against its neighbours' mean (a jump: over twice it).
usage: python -I Tools/pm116_color.py <renk.png> <capture>:<label> [<capture>:<label> ...]"""
import glob, json, os, re, sys
import numpy as np
import cv2
import matplotlib; matplotlib.use('Agg')
import matplotlib.pyplot as plt

out, specs = sys.argv[1], sys.argv[2:]
num = lambda s: float(s.replace(',', '.'))
COLS = ['#2a6fdb', '#e0702a', '#2f9e5a', '#9b51e0']


def load(cap):
    text = open(os.path.join(cap, 'log.txt'), encoding='utf8', errors='replace').read()
    start = None; rects = []
    for line in text.splitlines():
        m = re.match(r'^([\d.]+) \[cerceve\] 0[.,]000 ', line)
        if m and start is None: start = float(m.group(1)) / 1000
        m = re.match(r'^([\d.]+) \[dl\] ([\d.,-]+) [\d.,]+ b ((?:[\d.,-]+ ?){8})', line)
        if m:
            q = np.array([num(x) for x in m.group(3).split()]).reshape(4, 2)
            rects.append((float(m.group(1)) / 1000, num(m.group(2)), q))
    tb = num(re.search(r'the bottom at ([\d,]+) s', text).group(1)); end = num(re.search(r'ended at ([\d,]+) s', text).group(1))
    times = np.array(json.load(open(os.path.join(cap, 'times.json')))); files = sorted(glob.glob(os.path.join(cap, 'f*.jpg')))
    return start, rects, tb, end, times, files


def series(cap):
    start, rects, tb, end, times, files = load(cap)
    rows = []; last = rects[-1]
    # the dive's frames from the bottom, then .25 s of the game (the board where the dive left it)
    picks = [r for r in rects if r[1] >= tb - .02] + [(last[0] + k / 60, last[1] + k / 60, last[2]) for k in range(1, 16)]
    seen = set()
    for wall, clock, q in picks:
        i = min(int(np.searchsorted(times, wall + .004)), len(files) - 1)
        if i in seen: continue
        seen.add(i)
        im = cv2.imread(files[i]); h, w = im.shape[:2]
        sc = w / 390
        x0, y0 = q[:, 0].min() * sc, q[:, 1].min() * sc; x1, y1 = q[:, 0].max() * sc, q[:, 1].max() * sc
        cx, cy, hw, hh = (x0 + x1) / 2, (y0 + y1) / 2, (x1 - x0) * .4, (y1 - y0) * .4
        a, b, c, d = int(max(0, cx - hw)), int(min(w, cx + hw)), int(max(0, cy - hh)), int(min(h, cy + hh))
        if b - a < 20 or d - c < 20: continue
        hsv = cv2.cvtColor(im[c:d, a:b], cv2.COLOR_BGR2HSV).reshape(-1, 3).astype(float)
        rows.append((clock, hsv[:, 1].mean() / 255 * 100, hsv[:, 2].mean() / 255 * 100))
    return np.array(rows), tb, end


def jumps(r):
    out = []
    for col, name in ((1, 'saturation'), (2, 'brightness')):
        dq = np.diff(r[:, col]) / np.maximum(np.diff(r[:, 0]) * 60, .5)   # per 1/60 s: a frame the capture missed is not a jump
        for i in range(1, len(dq) - 1):
            nb = (abs(dq[i - 1]) + abs(dq[i + 1])) / 2
            if abs(dq[i]) > 2 * nb and abs(dq[i]) > .15: out.append((name, r[i + 1, 0], dq[i], nb))
    return out


fig, axs = plt.subplots(2, 1, figsize=(10, 7), sharex=True)
for k, s in enumerate(specs):
    cap, label = s.rsplit(':', 1)
    r, tb, end = series(cap)
    for ax, col, name in ((axs[0], 1, 'doygunluk (%)'), (axs[1], 2, 'parlaklık (%)')):
        ax.plot(r[:, 0], r[:, col], '-o', ms=2.5, lw=1.6, color=COLS[k % 4], label=label); ax.set_ylabel(name)
    js = jumps(r); dmax = np.abs(np.diff(r[:, 1:], axis=0)).max(0)
    print(f'[renk] {label}: saturation {r[0, 1]:.1f} -> {r[-1, 1]:.1f}, brightness {r[0, 2]:.1f} -> {r[-1, 2]:.1f}; the most in a frame {dmax[0]:.2f} / {dmax[1]:.2f}; jumps {[(n, round(t, 3), round(d, 2), round(nb, 2)) for n, t, d, nb in js]}')
for ax in axs:
    ax.axvline(tb, color='#888', ls='--', lw=1); ax.axvline(end, color='#888', ls='--', lw=1)
    ax.grid(alpha=.25); ax.spines[['top', 'right']].set_visible(False)
axs[0].text(tb, axs[0].get_ylim()[1], ' dip', va='top', color='#666', fontsize=9); axs[0].text(end, axs[0].get_ylim()[1], ' oyun', va='top', color='#666', fontsize=9)
axs[0].legend(frameon=False, loc='lower left'); axs[1].set_xlabel('dalışın saati (sn)')
axs[0].set_title('Geri çekilme: tahtanın ortalama doygunluğu ve parlaklığı, kare kare', loc='left', fontsize=11)
fig.tight_layout(); fig.savefig(out, dpi=130); print(f'[renk] {out}')
