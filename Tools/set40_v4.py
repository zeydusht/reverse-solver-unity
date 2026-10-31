"""SET40 v4 (PM_MESAJ_23), step 1: the fuse / extra-bomb variants of the 9 retuned levels -> Logs/setv4/var.json.
SetV4.Measure then measures them (SmartPlayer, FunSim first try, random player).
Variant id: <base>_<piece>-<fuse>[_<piece>-<fuse>].   Run: python Tools/set40_v4.py"""
import json, os

LV = 'Assets/_Game/Levels/'
cur = {l['id']: l for l in json.load(open(LV + 'redesign.json', encoding='utf-8'))['levels']}
v2 = {l['id']: l for l in json.load(open(LV + 'redesign_v2.json', encoding='utf-8'))['levels']}


def base(i):
    return cur.get(i) or v2[i]


def var(l, bombs):
    v = json.loads(json.dumps(l))
    v['bombs'] = {str(k): f for k, f in bombs.items()}
    v['id'] = l['id'] + '_' + '_'.join(f'{k}-{f}' for k, f in sorted(bombs.items(), key=lambda kv: int(kv[0])))
    return v


out = []
# fuse shortening
l = base('S47')                                   # bombs 7:35, 18:29
for a in range(12, 25, 2):
    for b in (a - 4, a, a + 4):
        out.append(var(l, {'18': a, '7': b}))
for i, fs in (('S20', range(7, 13)), ('S51', range(7, 16))):
    l = base(i); k = next(iter(l['bombs']))
    out += [var(l, {k: f}) for f in fs]
# +1 bomb: single-cell, not nailed, not already a bomb; fuse = exit step + slack
for i in ('S18', 'S17', 'S10', 'S50', 'S52', 'S08'):
    l = base(i)
    order = [m['piece'] for m in l['solution']]
    n = len(order)
    cand = [p for p in order[n // 4: n * 3 // 4]
            if len(l['pieces'][p]) == 1 and str(p) not in l['nails'] and str(p) not in l['bombs']]
    step = max(1, len(cand) // 8)
    for p in cand[::step][:8]:
        k = order.index(p) + 1
        for slack in (1, 2, 4, 7, 11):
            out.append(var(l, dict(l['bombs'], **{str(p): k + slack})))

os.makedirs('Logs/setv4', exist_ok=True)
json.dump({'format': 1, 'levels': out}, open('Logs/setv4/var.json', 'w', encoding='utf-8'), ensure_ascii=False)
print(len(out), 'varyant')
