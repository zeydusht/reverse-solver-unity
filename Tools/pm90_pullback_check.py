"""PM_90 §3: the dive's end from the game's own log (pm72_tour.js with TOUR_LOG='[' TOUR_LOG_TS=1, &dalislog=1).
Per frame from the hand-over ([devir]) to the game: the board's corners on the screen ([dl] ... b), the picture's
warp over the board ([geri]: u per board width 'across', v per board height 'down'). From them:
- the visible crop: the share of the picture's width (u) and height (v) on the screen; it must only grow over the
  pull back (one way, no narrowing then widening);
- the picture's scale across over down on the screen, frame to frame (1.00 +- .01) and carried;
- the picture's scale frame to frame (the camera's pull back and the warp together).
usage: python Tools/pm90_pullback_check.py <tour log> [--w 390 --h 844] [--csv out.csv]"""
import re, sys

args = sys.argv[1:]
logf = args[0]
W = float(args[args.index('--w') + 1]) if '--w' in args else 390
H = float(args[args.index('--h') + 1]) if '--h' in args else 844
csv = args[args.index('--csv') + 1] if '--csv' in args else None
num = lambda s: float(s.replace(',', '.'))
devir, board, warp = None, {}, {}
for line in open(logf, encoding='utf8', errors='replace'):
    m = re.search(r'\[devir\] ([\d.,-]+)', line)
    if m and devir is None: devir = num(m.group(1))
    m = re.search(r'\[dl\] ([\d.,-]+) [\d.,-]+ b ((?:[\d.,-]+ ?){8})', line)
    if m:
        v = [num(x) for x in m.group(2).split()]
        board[round(num(m.group(1)), 4)] = (v[0], v[1], v[2], v[5])   # left, top, right, bottom (pt)
    m = re.search(r'\[geri\] ([\d.,-]+) w ([\d.,-]+) across ([\d.,-]+) down ([\d.,-]+)', line)
    if m: warp[round(num(m.group(1)), 4)] = (num(m.group(2)), num(m.group(3)), num(m.group(4)))
if devir is None: sys.exit('no [devir] line: run the tour with &dalislog=1')
rows = []
for t in sorted(board):
    if t < devir - 1e-4 or t not in warp: continue
    l, tp, r, b = board[t]; w, across, down = warp[t]
    bw, bh = r - l, b - tp
    # the screen's share of the board, then of the picture (the board shows u across, v down of it)
    sw = (min(r, W) - max(l, 0)) / bw; sh = (min(b, H) - max(tp, 0)) / bh
    rows.append(dict(t=t - devir, w=w, bw=bw, bh=bh, u=across * sw, v=down * sh,
                     px_u=bw / across, px_v=bh / down))   # screen pt per picture uv across / down
if len(rows) < 3: sys.exit('too few frames after the hand-over')
back = [r for r in rows if r['w'] > 0]
hold = [r for r in rows if r['w'] <= 0]
start = back[0]['t'] if back else rows[-1]['t']
steps = []
for a, b in zip(rows, rows[1:]):
    xy = (b['px_u'] / a['px_u']) / (b['px_v'] / a['px_v'])
    sc = b['px_v'] / a['px_v']
    steps.append(dict(t=b['t'], xy=xy, sc=sc, du=b['u'] - a['u'], dv=b['v'] - a['v'], back=b['w'] > 0 or a['w'] > 0))
bs = [s for s in steps if s['back']]
carried = rows[-1]['px_u'] / rows[-1]['px_v'] / (rows[0]['px_u'] / rows[0]['px_v'])
print(f'hand-over at {devir:.3f} s (game clock); {len(rows)} frames to the game, the pull back from {start:+.3f} s ({len(back)} frames)')
print(f'X/Y per frame: worst {max((s["xy"] for s in steps), key=lambda x: abs(x - 1)):.4f} (1.00 +- .01: {"yes" if all(abs(s["xy"] - 1) <= .01 for s in steps) else "NO"}); carried {carried:.4f}')
print(f'  in the hold ({len(hold)} frames): X/Y per frame worst {max((s["xy"] for s in steps if not s["back"]), key=lambda x: abs(x - 1), default=1):.5f}')
sc_hold = [s['sc'] for s in steps if not s['back']]
print(f'scale per frame: the hold {min(sc_hold, default=1):.4f}..{max(sc_hold, default=1):.4f}; the pull back {min(s["sc"] for s in bs):.4f}..{max(s["sc"] for s in bs):.4f} (the camera zooming out)')
mono_u = all(s['du'] >= -1e-4 for s in bs); mono_v = all(s['dv'] >= -1e-4 for s in bs)
print(f'visible crop over the pull back: u {back[0]["u"]:.3f} -> {back[-1]["u"]:.3f} ({"one way" if mono_u else "NOT one way: min step " + format(min(s["du"] for s in bs), ".4f")}), '
      f'v {back[0]["v"]:.3f} -> {back[-1]["v"]:.3f} ({"one way" if mono_v else "NOT one way: min step " + format(min(s["dv"] for s in bs), ".4f")})')
print(f'  biggest step of the visible width: {max(s["du"] for s in bs) * 100:.2f} % of the picture in a frame')
print(f'  in the hold: u {hold[0]["u"]:.3f} -> {hold[-1]["u"]:.3f} (the hold\'s slow push in)' if hold else '')
for r in rows[::max(1, len(rows) // 16)]:
    print(f'  {r["t"]:+.3f} s  w {r["w"]:.3f}  board {r["bw"]:.0f}x{r["bh"]:.0f} pt  visible u {r["u"]:.3f} v {r["v"]:.3f}')
if csv:
    with open(csv, 'w') as f:
        f.write('t,w,bw,bh,u,v,px_u,px_v\n')
        for r in rows: f.write(','.join(f'{r[k]:.5f}' for k in ('t', 'w', 'bw', 'bh', 'u', 'v', 'px_u', 'px_v')) + '\n')
