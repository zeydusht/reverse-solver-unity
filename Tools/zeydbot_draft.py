# Designer loop (PM_MESAJ_2): turns the designer agent's written operations into draft levels (levels.json
# format) for ZeydDesign.Check. Base boards come from Assets/_Game/Levels/zeydbot.json.
#
# Input (JSON list): [{"id": "T31a", "base": "Z12", "ops": [[op, args...], ...], "note": "why"}]
#   mirror_x | mirror_y                       flip the board (pieces keep their numbers)
#   bomb_at x y fuse | unbomb_at x y          bomb on the piece covering cell (x, y)
#   nail_at x y count | unnail_at x y
#   joint v|h x y value                       v: boundary left of (x, y); h: above (x, y); -1 / 0 / 1
#   seal U|D|L|R lane | unseal U|D|L|R lane   lane = column for U/D, row for L/R
#   timer seconds
#   hole_at x y                               take a single-cell piece off (corner gap / hole)
#   bomb_piece piece fuse                     (the factory's tightened fuse, by piece number)
#
#   python Tools/zeydbot_draft.py <ops.json> <out levels.json>
import json, sys, copy

BASE = 'Assets/_Game/Levels/zeydbot.json'


def piece_at(lv, x, y):
    for i, cells in enumerate(lv['pieces']):
        if [x, y] in [list(c) for c in cells]: return i
    raise ValueError(f'({x}, {y}) üzerinde parça yok')


def mirror_x(lv):
    w = lv['w']
    lv['pieces'] = [[[w - 1 - c[0], c[1]] for c in cells] for cells in lv['pieces']]
    lv['vEdge'] = [[None] + [-row[w - x] if row[w - x] is not None else None for x in range(1, w)] + [None] for row in lv['vEdge']]
    lv['hEdge'] = [row if all(v is None for v in row) else [row[w - 1 - x] for x in range(w)] for row in lv['hEdge']]
    s = lv.get('sealed') or {}
    lv['sealed'] = {'U': sorted(w - 1 - c for c in s.get('U', [])), 'D': sorted(w - 1 - c for c in s.get('D', [])), 'L': s.get('R', []), 'R': s.get('L', [])}
    lv['solution'] = [{**m, 'dir': {'L': 'R', 'R': 'L'}.get(m['dir'], m['dir'])} for m in lv.get('solution') or []]


def mirror_y(lv):
    h = lv['h']
    lv['pieces'] = [[[c[0], h - 1 - c[1]] for c in cells] for cells in lv['pieces']]
    lv['hEdge'] = [[None] * lv['w']] + [[-v if v is not None else None for v in lv['hEdge'][h - y]] for y in range(1, h)] + [[None] * lv['w']]
    lv['vEdge'] = [lv['vEdge'][h - 1 - y] for y in range(h)]
    s = lv.get('sealed') or {}
    lv['sealed'] = {'L': sorted(h - 1 - c for c in s.get('L', [])), 'R': sorted(h - 1 - c for c in s.get('R', [])), 'U': s.get('D', []), 'D': s.get('U', [])}
    lv['solution'] = [{**m, 'dir': {'U': 'D', 'D': 'U'}.get(m['dir'], m['dir'])} for m in lv.get('solution') or []]


def apply(lv, op):
    k, a = op[0], op[1:]
    if k == 'mirror_x': mirror_x(lv)
    elif k == 'mirror_y': mirror_y(lv)
    elif k == 'bomb_at': lv['bombs'][str(piece_at(lv, a[0], a[1]))] = int(a[2])
    elif k == 'bomb_piece': lv['bombs'][str(int(a[0]))] = int(a[1])     # factory's fuse setting (ZeydDesign -rsTightFuse)
    elif k == 'unbomb_at': lv['bombs'].pop(str(piece_at(lv, a[0], a[1])))
    elif k == 'nail_at': lv['nails'][str(piece_at(lv, a[0], a[1]))] = int(a[2])
    elif k == 'unnail_at': lv['nails'].pop(str(piece_at(lv, a[0], a[1])))
    elif k == 'joint':
        tab = lv['vEdge'] if a[0] == 'v' else lv['hEdge']
        x, y, v = int(a[1]), int(a[2]), int(a[3])
        if tab[y][x] is None: raise ValueError(f'{a[0]} ({x}, {y}) dış kenar')
        tab[y][x] = v
    elif k == 'seal':
        lane = lv.setdefault('sealed', {}).setdefault(a[0], [])
        if int(a[1]) not in lane: lane.append(int(a[1])); lane.sort()
    elif k == 'unseal':
        lane = lv.setdefault('sealed', {}).setdefault(a[0], [])
        if int(a[1]) in lane: lane.remove(int(a[1]))
    elif k == 'timer': lv['timer'] = int(a[0])
    elif k == 'hole_at':
        # non-rectangular boards (PM_MESAJ_5): a single-cell piece is taken off, its seams go flat, the numbers after
        # it shift down; the stored solution is dropped (the factory solves again)
        x, y = int(a[0]), int(a[1]); i = piece_at(lv, x, y)
        if len(lv['pieces'][i]) != 1: raise ValueError(f'({x}, {y}) zincirli parça, delik olamaz')
        lv['pieces'].pop(i)
        for key in ('bombs', 'nails'):
            d = {}
            for kk, v in (lv.get(key) or {}).items():
                j = int(kk)
                if j == i: continue
                d[str(j - 1 if j > i else j)] = v
            lv[key] = d
        for (xx, yy) in ((x, y), (x + 1, y)):
            if 0 < xx < lv['w']: lv['vEdge'][yy][xx] = 0
        for (xx, yy) in ((x, y), (x, y + 1)):
            if 0 < yy < lv['h']: lv['hEdge'][yy][xx] = 0
        lv['solution'] = []
    else: raise ValueError('bilinmeyen işlem ' + k)


def main(ops_file, out_file):
    base = {l['id']: l for l in json.load(open(BASE, encoding='utf-8'))['levels']}
    import glob                                                   # SET40 v2 peak candidates (ZeydSet.Peaks)
    for f in glob.glob('Logs/zdesign/zirve_[0-9]*x[0-9]*.json'):
        base.update({l['id']: l for l in json.load(open(f, encoding='utf-8'))['levels']})
    drafts = json.load(open(ops_file, encoding='utf-8'))
    out = []
    for d in drafts:
        lv = copy.deepcopy(base[d['base']])
        if isinstance(lv['pieces'], str): lv['pieces'] = json.loads(lv['pieces'])
        for op in d['ops']: apply(lv, op)
        lv['id'] = d['id']; lv.pop('zeydbot', None)
        out.append(lv)
        print(d['id'], 'tamam:', len(d['ops']), 'işlem, bomba', lv['bombs'], 'çivi', lv['nails'], 'süre', lv['timer'])
    json.dump({'format': 1, 'note': 'ZeydDesign taslakları (geçici)', 'levels': out}, open(out_file, 'w', encoding='utf-8'), ensure_ascii=False)


if __name__ == '__main__':
    main(sys.argv[1], sys.argv[2])
