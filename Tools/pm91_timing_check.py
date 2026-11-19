"""PM_91 §1: the dive's durations from the game's own log (pm72_tour.js with TOUR_LOG='[' TOUR_LOG_TS=1, &dalislog=1).
  approach  the dive's clock from 0 (the tap on Oyna, or the end of the corridor) to the hand-over ([devir])
  on the picture  from the first frame the picture (the frame's opening, [cerceve] q, or else the board from the
                  hand-over, [dl] b) covers the whole screen (within --tol pt, default 2) to the start of the pull back (the first [geri] with w > 0, less one frame)
  pull back  from there to the first [geri] with w = 1
and the camera's speed over the approach: the opening's size on the screen (its height, in ln) per second, to --csv.
usage: python Tools/pm91_timing_check.py <tour log> [--tol 2] [--screen 390x844] [--csv out.csv]"""
import re, sys, math

args = sys.argv[1:]
logf = args[0]
tol = float(args[args.index('--tol') + 1]) if '--tol' in args else 2.0
W, H = (float(v) for v in (args[args.index('--screen') + 1] if '--screen' in args else '390x844').split('x'))
csv = args[args.index('--csv') + 1] if '--csv' in args else None
num = lambda s: float(s.replace(',', '.'))
frames, geri, devir, koridor, boards = [], [], None, None, []
for line in open(logf, encoding='utf8', errors='replace'):
    m = re.search(r'\[cerceve\] ([\d.,-]+) e [\d.,-]+ off [\d.,-]+ q ((?:[\d.,-]+ ?){8})', line)
    if m:
        v = [num(x) for x in m.group(2).split()]
        frames.append((num(m.group(1)), [(v[0], v[1]), (v[2], v[3]), (v[4], v[5]), (v[6], v[7])]))
    m = re.search(r'\[geri\] ([\d.,-]+) w ([\d.,-]+)', line)
    if m: geri.append((num(m.group(1)), num(m.group(2))))
    m = re.search(r'\[dl\] ([\d.,-]+) [\d.,-]+ b ((?:[\d.,-]+ ?){8})', line)
    if m:
        v = [num(x) for x in m.group(2).split()]
        boards.append((num(m.group(1)), [(v[0], v[1]), (v[2], v[3]), (v[4], v[5]), (v[6], v[7])]))
    m = re.search(r'\[devir\] ([\d.,-]+)', line)
    if m and devir is None: devir = num(m.group(1))
    m = re.search(r'\[koridor\] ([\d.,-]+) s', line)
    if m and koridor is None: koridor = num(m.group(1))
if not frames or not geri or devir is None: sys.exit('no [cerceve] / [geri] / [devir] lines')


def inside(q, p):
    s = 0
    for i in range(4):
        a, b = q[i], q[(i + 1) % 4]
        c = (b[0] - a[0]) * (p[1] - a[1]) - (b[1] - a[1]) * (p[0] - a[0])
        if abs(c) < 1e-9: continue
        if s == 0: s = 1 if c > 0 else -1
        elif (c > 0) != (s > 0): return False
    return True


corners = [(tol, tol), (W - tol, tol), (W - tol, H - tol), (tol, H - tol)]
cover = next((t for t, q in frames if all(inside(q, c) for c in corners)), None)
if cover is None:   # the frame's opening never quite covers it: the board does, from the hand-over ([dl] ... b)
    cover = next((t for t, q in boards if t >= devir - 1e-4 and all(inside(q, c) for c in corners)), None)
back0 = next((t for t, w in geri if w > 0), None)
before = [t for t, w in geri if w <= 0 and (back0 is None or t < back0)]
h1 = before[-1] if before else None
back1 = next((t for t, w in geri if w >= 1), None)
print(f'approach {devir:.3f} s' + (f' (after the corridor {koridor:.2f} s)' if koridor else '') +
      f'; on the picture {(h1 - cover) if cover is not None and h1 else float("nan"):.3f} s (covers at {cover if cover is not None else float("nan"):.3f}, '
      f'pull back from {h1 if h1 else float("nan"):.3f}); pull back {(back1 - h1) if back1 and h1 else float("nan"):.3f} s')
# the speed: d ln(height of the opening) / dt, per frame
rows = []
for (t0, q0), (t1, q1) in zip(frames, frames[1:]):
    if t1 - t0 < 1e-4: continue
    h0 = (math.dist(q0[0], q0[3]) + math.dist(q0[1], q0[2])) / 2
    h1_ = (math.dist(q1[0], q1[3]) + math.dist(q1[1], q1[2])) / 2
    c0 = [sum(p[k] for p in q0) / 4 for k in (0, 1)]; c1 = [sum(p[k] for p in q1) / 4 for k in (0, 1)]
    rows.append(((t0 + t1) / 2, math.log(h1_ / h0) / (t1 - t0), math.dist(c0, c1) / (t1 - t0)))
if rows:
    pk = max(rows, key=lambda r: r[1])
    tail = [r for r in rows if r[0] >= devir * .9]
    print(f'  zoom speed (ln height / s): peak {pk[1]:.2f} at {pk[0]:.3f} s ({pk[0] / devir:.0%} of the approach); '
          f'last 10 %: {min(r[1] for r in tail) if tail else float("nan"):.2f}..{max(r[1] for r in tail) if tail else float("nan"):.2f}')
if csv:
    with open(csv, 'w') as f:
        f.write('t,zoom,move\n')
        for r in rows: f.write(f'{r[0]:.5f},{r[1]:.5f},{r[2]:.3f}\n')
