# M16 (PM_MESAJ_4): similarity of two levels independent of rotation and mirroring. For each of the 8 symmetries
# of the square (rotations by 90 degrees swap width and height) the second board is mapped onto the first and two
# scores are taken, the best symmetry kept:
#   board    Jaccard over the elements: holes (empty cells), locked joints (as unordered cell pairs), sealed edge lanes,
#            bomb cells, nail cells, chain links (cell pairs)
#   solution longest common subsequence of the exit cells in solution order / the longer solution
#   score    mean of the two (0 = nothing alike, 1 = the same level turned)
#
#   python Tools/level_similarity.py <levels.json> [ID ...]      -> matrix (percent) of the given levels
import json, sys, itertools
import csv
import math
from pathlib import Path


def curve_similarity(a, b):
    """1 - normalized Euclidean distance of two ten-point signatures.

    Each curve is divided by its own maximum; a zero curve stays zero.
    Accepts either numeric sequences or semicolon-separated CSV signatures.
    """
    def normalized(values):
        values = [float(v) for v in (values.split(';') if isinstance(values, str) else values)]
        if len(values) != 10 or any(not math.isfinite(v) or v < 0 for v in values):
            raise ValueError('Curve signature must contain ten finite nonnegative values')
        peak = max(values) or 1
        return [v / peak for v in values]
    return 1 - math.sqrt(sum((x - y) ** 2 for x, y in zip(normalized(a), normalized(b)))) / math.sqrt(10)


def occupancy_mask(level, size=16):
    """Fixed canvas, one cell per cell, centred; seals occupy border walls."""
    w, h = level['w'], level['h']
    if min(w, h) <= 0 or max(w, h) + 2 > size:
        raise ValueError('Board must fit fixed canvas including border walls')
    dx, dy = (size - w) // 2, (size - h) // 2
    mask = {('cell', x + dx, y + dy) for piece in cells_of(level) for x, y in piece}
    for side, lanes in (level.get('sealed') or {}).items():
        for lane in lanes:
            x, y = {'U': (lane, -1), 'D': (lane, h), 'L': (-1, lane), 'R': (w, lane)}[side]
            mask.add(('wall', x + dx, y + dy))
    return mask


def mask_similarity(a, b):
    """Jaccard preserving physical dimensions and border walls."""
    ma, mb = occupancy_mask(a), occupancy_mask(b)
    return len(ma & mb) / len(ma | mb) if ma | mb else 1.0


def curves_main():
    out = Path(__file__).resolve().parents[1] / 'docs/analiz'
    with (out / 'hikaye_egrisi_set40.csv').open(encoding='utf-8-sig', newline='') as stream:
        rows = list(csv.DictReader(stream))
    pairs = [(int(a['bolum']), int(b['bolum']), curve_similarity(a['egri_imzasi'], b['egri_imzasi']))
             for a, b in itertools.combinations(rows, 2)]
    pairs.sort(key=lambda pair: (-pair[2], pair[0], pair[1]))
    with (out / 'hikaye_benzer10.csv').open('w', encoding='utf-8', newline='') as stream:
        writer = csv.writer(stream)
        writer.writerow(['bolum_a', 'bolum_b', 'benzerlik'])
        for a, b, score in pairs[:10]:
            writer.writerow([a, b, f'{score:.6f}'])
            print(f'{a} {b} {score:.6f}')


def cells_of(l):
    p = l['pieces'] if isinstance(l['pieces'], list) else json.loads(l['pieces'])
    return [[tuple(c) for c in cs] for cs in p]


def elements(l):
    w, h = l['w'], l['h']
    pieces = cells_of(l)
    E = set()
    filled = {c for cs in pieces for c in cs}
    # holes, not filled cells: every full board has the same cells, which would lift every pair (v2, 2026-10-21)
    for x in range(w):
        for y in range(h):
            if (x, y) not in filled: E.add(('hole', (x, y)))
    for cs in pieces:
        if len(cs) == 2: E.add(('chain', frozenset(cs)))
    for y, row in enumerate(l['vEdge']):
        for x, v in enumerate(row):
            if v not in (None, 0): E.add(('joint', frozenset({(x - 1, y), (x, y)})))
    for y, row in enumerate(l['hEdge']):
        for x, v in enumerate(row):
            if v not in (None, 0): E.add(('joint', frozenset({(x, y - 1), (x, y)})))
    seal = l.get('sealed') or {}
    for side, lanes in seal.items():
        for k in lanes:
            # the outside edge of the lane's border cell on that side
            c = {'U': (k, 0), 'D': (k, h - 1), 'L': (0, k), 'R': (w - 1, k)}[side]
            o = {'U': (k, -1), 'D': (k, h), 'L': (-1, k), 'R': (w, k)}[side]
            E.add(('seal', frozenset({c, o})))
    for k in l.get('bombs') or {}: E.add(('bomb', pieces[int(k)][0]))
    for k in l.get('nails') or {}: E.add(('nail', pieces[int(k)][0]))
    sol = [pieces[m['piece']][0] for m in l.get('solution') or []]
    return E, sol, w, h


def sym(k, w, h):
    """The 8 maps of a w x h board; returns (f, new_w, new_h)."""
    fs = [lambda x, y: (x, y), lambda x, y: (w - 1 - x, y), lambda x, y: (x, h - 1 - y), lambda x, y: (w - 1 - x, h - 1 - y),
          lambda x, y: (y, x), lambda x, y: (h - 1 - y, x), lambda x, y: (y, w - 1 - x), lambda x, y: (h - 1 - y, w - 1 - x)]
    return fs[k], (w, h) if k < 4 else (h, w)


def mapped(E, f):
    out = set()
    for kind, v in E:
        if isinstance(v, frozenset): out.add((kind, frozenset(f(*c) for c in v)))
        else: out.add((kind, f(*v)))
    return out


def lcs(a, b):
    prev = [0] * (len(b) + 1)
    for x in a:
        cur = [0]
        for j, y in enumerate(b): cur.append(prev[j] + 1 if x == y else max(prev[j + 1], cur[j]))
        prev = cur
    return prev[-1]


def similarity(a, b):
    Ea, sa, wa, ha = elements(a)
    Eb, sb, wb, hb = elements(b)
    best = (0, 0, 0, -1)
    for k in range(8):
        f0, (w2, h2) = sym(k, wb, hb)
        # different sizes: the turned board is also slid to every offset over the other (a level grown by a row
        # or cut by a column is still the same level)
        for dx in range(min(0, wa - w2), max(0, wa - w2) + 1):
          for dy in range(min(0, ha - h2), max(0, ha - h2) + 1):
            f = (lambda f0, dx, dy: lambda x, y: (f0(x, y)[0] + dx, f0(x, y)[1] + dy))(f0, dx, dy)
            Em = mapped(Eb, f)
            board = len(Ea & Em) / max(1, len(Ea | Em))
            sol = lcs(sa, [f(*c) for c in sb]) / max(1, len(sa), len(sb))
            s = (board + sol) / 2
            if s > best[0]: best = (s, board, sol, k)
    return best


if __name__ == '__main__':
    if '--curves' in sys.argv[1:]:
        curves_main()
        sys.exit(0)
    L = {l['id']: l for l in json.load(open(sys.argv[1], encoding='utf-8'))['levels']}
    ids = sys.argv[2:] or list(L)
    print('| | ' + ' | '.join(ids) + ' |'); print('|---' * (len(ids) + 1) + '|')
    for a in ids:
        print(f'| {a} ({L[a]["w"]}×{L[a]["h"]}, {len(cells_of(L[a]))}) | ' + ' | '.join('—' if a == b else f'{similarity(L[a], L[b])[0] * 100:.0f}' for b in ids) + ' |')
