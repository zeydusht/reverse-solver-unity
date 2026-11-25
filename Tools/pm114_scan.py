"""PM_114: the menu's dive frame by frame, before / after (Tools/pm108_dalis.js captures with &dalislog=1):
  moves    the picture / board on the screen each game frame (its corners: [cerceve] q on the way in, [dl] b from the
           hand over; the camera [geri] kamera): centre (pt), size (ln), turn (deg); a jump: a frame's change over twice
           its neighbours' mean (and over a floor), told apart from a long frame (the same change per clock second)
  lines    [cizgi]: the cuts' share, the rim's, the drawing's clock, the pieces' look, the HUD's slide: the most any of
           them changes in one frame (over 30 % 'at once'); the rim as PM_113 drew it (&kenar=0: every side at once
           from .75 s on the drawing's clock, over .55 s, smoothstep) from the logged clock; the pens' tips (PM_113: on
           and off in a frame; PM_114: over 30 % of their stroke)
  pixels   consecutive captured frames: the mean difference, and in the pull back with the camera's move taken out (the
           previous frame moved by the camera: what is left is what came, went or slipped), its strongest 26 x 26 pt
           patch; a spike: over twice its neighbours' mean
usage: python -I Tools/pm114_scan.py <sicrama.png> <capture>:<label> [<capture>:<label>]"""
import glob, json, os, re, sys
import numpy as np
import cv2
import matplotlib; matplotlib.use('Agg')
import matplotlib.pyplot as plt

out, specs = sys.argv[1], sys.argv[2:]
num = lambda s: float(s.replace(',', '.'))
W0, H0 = 390, 844
EDGE_SEC, OUTER = .55, .75   # Reveal's EdgeSeconds; PM_113's rim start on the drawing's clock (Spread + .05)


def smooth(x):
    x = np.clip(x, 0, 1); return x * x * (3 - 2 * x)


def load(cap):
    text = open(os.path.join(cap, 'log.txt'), encoding='utf8', errors='replace').read()
    start = None; quads = {}; cam = []; cz = []; devir = None
    for line in text.splitlines():
        m = re.match(r'^([\d.]+) \[cerceve\] ([\d.,-]+) e [\d.,-]+ off [\d.,-]+ q ((?:[\d.,-]+ ?){8})', line)
        if m:
            t = num(m.group(2))
            if start is None and abs(t) < 1e-6: start = float(m.group(1)) / 1000
            quads.setdefault(round(t, 4), ('in', np.array([num(x) for x in m.group(3).split()]).reshape(4, 2)))
            continue
        m = re.match(r'^([\d.]+) \[dl\] ([\d.,-]+) ([\d.,]+) b ((?:[\d.,-]+ ?){8})', line)
        if m: quads[round(num(m.group(2)), 4)] = ('board', np.array([num(x) for x in m.group(4).split()]).reshape(4, 2)); continue
        m = re.match(r'^([\d.]+) \[geri\] kamera ([\d.,-]+) ([\d.,-]+) ([\d.,-]+) ([\d.,-]+)', line)
        if m: cam.append((float(m.group(1)) / 1000, num(m.group(2)), num(m.group(3)), num(m.group(4)), num(m.group(5)))); continue
        m = re.match(r'^[\d.]+ \[cizgi\] ([\d.,-]+) share ([\d.,-]+) rim ([\d.,-]+) time ([\d.,-]+) look ([\d.,-]+) hud ([\d.,-]+)', line)
        if m: cz.append([num(m.group(i)) for i in range(1, 7)]); continue
        m = re.match(r'^[\d.]+ \[devir\] ([\d.,]+)', line)
        if m: devir = num(m.group(1))
    g = lambda p, d=float('nan'): (lambda m: num(m.group(1)) if m else d)(re.search(p, text))
    info = dict(ta=g(r'the framing at ([\d,]+) s'), tb=g(r'the bottom at ([\d,]+) s'), end=g(r'ended at ([\d,]+) s'),
                devir=devir, longs=[(int(a), num(b)) for a, b in re.findall(r'long frame (\d+) ms at ([\d,]+) s', text)],
                build=g(r'build (\d+) ms'), reveal=g(r'reveal (\d+) ms'), prepare=g(r'prepare (\d+) ms'))
    if np.isnan(info['tb']): info['tb'] = info['ta']
    ts = sorted(quads); Q = np.array([quads[t][1] for t in ts]); seg = np.array([quads[t][0] for t in ts]); ts = np.array(ts)
    cz = np.array(cz) if cz else np.zeros((0, 6))
    times = np.array(json.load(open(os.path.join(cap, 'times.json')))); files = sorted(glob.glob(os.path.join(cap, 'f*.jpg')))
    return dict(start=start, ts=ts, Q=Q, seg=seg, cam=cam, cz=cz, times=times, files=files, **info)


def moves(d):
    """the picture / board on the screen per game frame: centre x, y (pt), size (ln of sqrt area), turn (deg)"""
    Q, ts = d['Q'], d['ts']
    c = Q.mean(1)
    x, y = Q[:, :, 0], Q[:, :, 1]
    area = .5 * np.abs((x * np.roll(y, -1, 1) - np.roll(x, -1, 1) * y).sum(1))
    size = np.log(np.sqrt(area))
    top = Q[:, 1] - Q[:, 0]; turn = np.degrees(np.arctan2(top[:, 1], top[:, 0]))
    return ts, dict(x=c[:, 0], y=c[:, 1], size=size, turn=turn)


def jumps(ts, q, floor):
    """frames whose change is over twice the mean of the frames' either side (and over floor); 'clock' when the change
    per clock second is not (a longer step of the clock, not a jump on the way)"""
    dq = np.diff(q); dt = np.diff(ts); out = []
    for i in range(1, len(dq) - 1):
        nb = (abs(dq[i - 1]) + abs(dq[i + 1])) / 2
        if abs(dq[i]) > 2 * nb and abs(dq[i]) > floor:
            rate = abs(dq[i]) / max(dt[i], 1e-4); nr = (abs(dq[i - 1]) / max(dt[i - 1], 1e-4) + abs(dq[i + 1]) / max(dt[i + 1], 1e-4)) / 2
            out.append((ts[i + 1], dq[i], nb, 'clock' if rate <= 2 * nr else 'jump'))
    return out


def lines(d, fixed):
    """the most each drawn thing changes in a frame, and where"""
    cz = d['cz']
    if not len(cz): return {}
    t, share, rim, time, look, hud = cz.T
    res = {}
    def most(name, v):
        dv = np.abs(np.diff(v)); i = int(np.argmax(dv)) if len(dv) else 0
        res[name] = (float(dv[i]) if len(dv) else 0, float(t[i + 1]) if len(dv) else 0)
    most('cuts', share)
    if fixed: most('rim', rim)
    else: most('rim', smooth((time - OUTER) / EDGE_SEC))   # every side of it together, PM_113
    most('look', look)
    most('hud', hud)
    # the tips: PM_113 on / off in a frame; PM_114 over 12 % of a stroke (.55 s on the drawing's clock)
    ok = np.diff(time) < .2   # PM_113's clock jumping to its end (the cuts all drawn by then)
    res['tips'] = (min(1.0, float(np.max(np.abs(np.diff(time))[ok])) / (EDGE_SEC * .3)) if fixed else 1.0, None)
    if fixed:
        rimTip = np.clip(np.minimum(rim, 1 - rim) / .3, 0, 1)
        res['rim tips'] = (float(np.max(np.abs(np.diff(rimTip)))), float(t[1 + int(np.argmax(np.abs(np.diff(rimTip))))]))
    res['rim from'] = float(t[np.argmax(rim > 1e-4)]) if fixed else float(t[np.argmax(time > OUTER)])
    res['rim to'] = float(t[np.argmax(rim >= 1 - 1e-4)]) if fixed else float(t[np.argmax(time >= OUTER + EDGE_SEC - 1e-4)])
    res['cuts to'] = float(t[np.argmax(share >= 1 - 1e-4)])
    return res


def pixels(d):
    """per captured frame (dive time): the mean difference to the frame before (per 1/60 s), and in the pull back
    with the camera's move taken out, its strongest patch and where"""
    start, cam = d['start'], d['cam']
    ts = d['times'] - start; rows = []; prev = None
    for t, f in zip(ts, d['files']):
        if t < -.1 or t > d['end'] + .4: prev = None; continue
        im = cv2.imread(f).astype(np.float32)
        c = [x for x in cam if x[0] <= t + start]
        c = c[-1] if c and t < d['end'] + .02 else None
        if prev is not None:
            dt = max(t - prev[0], 1e-3); plain = np.abs(im - prev[1]).mean() * (1 / 60) / dt
            comp, where = np.nan, None
            if c is not None and prev[2] is not None and c is not prev[2]:
                _, _, s0, x0, y0 = prev[2]; _, _, s1, x1, y1 = c; k = s1 / s0
                M = np.float32([[k, 0, W0 / 2 - k * W0 / 2 + s1 * (x0 - x1)], [0, k, H0 / 2 - k * H0 / 2 + s1 * (y1 - y0)]])
                w = cv2.warpAffine(prev[1], M, (W0, H0), flags=cv2.INTER_LINEAR, borderValue=(-1, -1, -1))
                ok = w[..., 0] >= 0; dd = np.abs(im - w).mean(2) * ok
                b = cv2.resize(dd, (W0 // 26, H0 // 26), interpolation=cv2.INTER_AREA)
                j, i = np.unravel_index(np.argmax(b), b.shape); comp, where = float(b.max()), (i * 26 + 13, j * 26 + 13)
            elif c is None and t > d['end']:
                b = cv2.resize(np.abs(im - prev[1]).mean(2), (W0 // 26, H0 // 26), interpolation=cv2.INTER_AREA)
                j, i = np.unravel_index(np.argmax(b), b.shape); comp, where = float(b.max()), (i * 26 + 13, j * 26 + 13)
            rows.append((t, plain, comp, where, dt))
        prev = (t, im, c)
    return rows


def spikes(rows, col, floor):
    v = np.array([r[col] for r in rows]); out = []
    for i in range(1, len(v) - 1):
        if np.isnan(v[i]): continue
        nb = np.nanmean([v[i - 1], v[i + 1]])
        if v[i] > 2 * nb and v[i] > floor: out.append(i)
    return out


INK, MUTED, GRIDC, BG = '#0b0b0b', '#52514e', '#e6e5e0', '#fcfcfb'
COL = {'x': '#2a78d6', 'y': '#1baf7a', 'size': '#eb6834', 'turn': '#8f8d87'}
plt.rcParams.update({'font.family': 'Arial', 'font.size': 10.5, 'axes.edgecolor': MUTED, 'axes.labelcolor': MUTED,
                     'xtick.color': MUTED, 'ytick.color': MUTED, 'axes.spines.top': False, 'axes.spines.right': False})
fig, ax = plt.subplots(3, len(specs), figsize=(8 * len(specs), 13.5), dpi=110, facecolor=BG, squeeze=False, sharex=True)
for k, spec in enumerate(specs):
    cap, label = spec.rsplit(':', 1); fixed = 'kenar=0' not in json.load(open(os.path.join(cap, 'meta.json')))['query']
    d = load(cap); ts, mv = moves(d)
    print(f'[scan] {label}: framing {d["ta"]:.3f} s, bottom {d["tb"]:.3f} s, hand over {d["devir"]}, end {d["end"]:.3f} s; '
          f'prepare {d["prepare"]:.0f} ms, build {d["build"]:.0f} ms, reveal {d["reveal"]:.0f} ms; long frames {d["longs"]}')
    floors = dict(x=.5, y=.5, size=.004, turn=.05)
    for name, q in mv.items():
        # the way in (the picture's quad) and from the hand over (the board's): each on its own (two different quads;
        # the hand over itself in the pixels)
        found = []
        for sg in ('in', 'board'):
            m = d['seg'] == sg; found += jumps(ts[m], q[m], floors[name])
        for (t, dq, nb, why) in found:
            print(f'[scan]   {name} at {t:.3f} s: {dq:+.4f} a frame against {nb:.4f} either side ({why})')
    for name, v in lines(d, fixed).items(): print(f'[scan]   {name}: {v}')
    rows = pixels(d)
    for i in spikes(rows, 1, 2.0): print(f'[scan]   pixels at {rows[i][0]:.3f} s: {rows[i][1]:.2f} a 1/60 s (dt {rows[i][4] * 1000:.0f} ms)')
    for i in spikes(rows, 2, 25): print(f'[scan]   left over at {rows[i][0]:.3f} s: {rows[i][2]:.1f} at {tuple(int(v) for v in rows[i][3])}')
    for r in rows:
        if r[0] > d['end'] - .05: print(f'[scan]   after the end {r[0]:.3f} s: {r[1]:.2f} a 1/60 s, patch {r[2]:.1f}, dt {r[4] * 1000:.0f} ms')
    # the moves: change a frame (centre pt, size % , turn deg x 10)
    a = ax[0, k]
    dts = ts[1:]
    same = d['seg'][1:] == d['seg'][:-1]   # not across the hand over (two different quads)
    def P(v, c, lb):
        v = np.where(same, v, np.nan); a.plot(dts, v, color=c, lw=1.6, marker='o', ms=2.5, label=lb)
    P(np.hypot(np.diff(mv['x']), np.diff(mv['y'])), COL['x'], 'merkez (pt / kare)')
    P(np.abs(np.diff(mv['size'])) * 100, COL['size'], 'boyut (% / kare)')
    P(np.abs(np.diff(mv['turn'])) * 10, COL['turn'], 'dönme (0,1° / kare)')
    a.set_title(f'{label}: resim / tahta, kare kare değişim', loc='left', color=INK, fontsize=12.5, fontweight='bold')
    a.legend(loc='upper left', bbox_to_anchor=(0, -.06), ncol=3, frameon=False, fontsize=9)
    # the lines: the share of each a frame
    a = ax[1, k]; cz = d['cz']
    if len(cz):
        t, share, rim, time, look, hud = cz.T
        rimv = rim if fixed else smooth((time - OUTER) / EDGE_SEC)
        for v, c, lb in ((share, '#2a78d6', 'iç çizgiler'), (rimv, '#eb6834', 'dış kenar'), (look, '#1baf7a', 'parça görünümü'), (hud, '#8f8d87', 'üst/alt çubuk')):
            a.plot(t[1:], np.abs(np.diff(v)) * 100, color=c, lw=1.6, marker='o', ms=2.5, label=lb)
        a.axhline(30, color='#d03b3b', lw=1, ls='--'); a.text(t[0], 31, ' %30: "bir anda"', color='#d03b3b', fontsize=9)
    a.set_title(f'{label}: çizgiler, bir karede ne kadarı geliyor (%)', loc='left', color=INK, fontsize=12.5, fontweight='bold')
    a.legend(loc='upper left', bbox_to_anchor=(0, -.06), ncol=4, frameon=False, fontsize=9); a.set_ylim(0, 105)
    # the pixels
    a = ax[2, k]
    tt = np.array([r[0] for r in rows]); pl = np.array([r[1] for r in rows]); cp = np.array([r[2] for r in rows])
    a.plot(tt, pl, color='#2a78d6', lw=1.6, marker='o', ms=2.5, label='ardışık iki kare (ortalama)')
    a.plot(tt, cp / 4, color='#eb6834', lw=1.6, marker='o', ms=2.5, label='kamera çıkarılınca en güçlü yama (÷4)')
    for i in spikes(rows, 1, 2.0): a.plot(tt[i], pl[i], 'o', ms=10, mfc='none', mec='#d03b3b', mew=2)
    for i in spikes(rows, 2, 25): a.plot(tt[i], cp[i] / 4, 'o', ms=10, mfc='none', mec='#d03b3b', mew=2)
    a.set_title(f'{label}: piksel farkı (kırmızı halka: ani tepe)', loc='left', color=INK, fontsize=12.5, fontweight='bold')
    a.legend(loc='upper left', bbox_to_anchor=(0, -.12), ncol=2, frameon=False, fontsize=9); a.set_xlabel("Oyna'dan sonra (sn)")
    marks = [(0, 'basış'), (d['ta'], 'kadraj'), (d['tb'], 'dip'), (d['devir'], 'devir'),
             (d['tb'] + .8 * (d['end'] - d['tb']), 'çizgiler tamam'), (d['end'], 'bitiş')]
    for a in ax[:, k]:
        a.set_facecolor(BG); a.grid(axis='y', color=GRIDC, lw=.8); a.set_axisbelow(True); a.set_xlim(-.05, d['end'] + .3)
        for x, lb in marks:
            if x is None or np.isnan(x): continue
            a.axvline(x, color=MUTED, lw=.8, ls=':')
            a.text(x, 1, ' ' + lb, color=MUTED, fontsize=8.5, rotation=90, va='top', ha='right', transform=a.get_xaxis_transform())
fig.tight_layout(); fig.savefig(out, facecolor=fig.get_facecolor())
