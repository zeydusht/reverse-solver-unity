"""SET40 v4 (PM_MESAJ_23, docs/SET40_V4.md): redesign.json from v3 + the v2 pool + zeydbot.json, the 9 retuned levels
taking their bombs and solution from the measured variants (Logs/setv4/olcum*.json, picks in TUNED).
Keeps v3 as Assets/_Game/Levels/redesign_v3.json. Run: python Tools/make_set40_v4.py"""
import json, os, shutil

LV = 'Assets/_Game/Levels/'
v3_path = LV + 'redesign_v3.json'
if not os.path.exists(v3_path):
    shutil.copy(LV + 'redesign.json', v3_path)
v3 = json.load(open(v3_path, encoding='utf-8'))
pool = {l['id']: l for l in json.load(open(LV + 'redesign_v2.json', encoding='utf-8'))['levels']}
pool.update({l['id']: l for l in json.load(open(LV + 'zeydbot.json', encoding='utf-8'))['levels']})
pool.update({l['id']: l for l in v3['levels']})
measured = {}
for n in ('olcum', 'olcum2', 'olcum3'):
    measured.update({l['id']: l for l in json.load(open(f'Logs/setv4/{n}.json', encoding='utf-8'))['levels']})

ORDER = ['S41', 'S42', 'S43', 'S06', 'S44', 'S45', 'S46', 'S47', 'S20', 'Z24', 'S21', 'S51', 'S34', 'Z21', 'S13', 'Z04',
         'S18', 'S22', 'S28', 'S24', 'Z23', 'S19', 'S27', 'S14', 'S35', 'S17', 'S10', 'S50', 'S52', 'S37', 'S08', 'S31',
         'S39', 'S32', 'S36', 'S29', 'Z25', 'S38', 'S40', 'S33']
TUNED = json.load(open('Logs/setv4/secim.json', encoding='utf-8'))      # base id -> measured variant id
HARD = {19, 23, 25, 30, 35, 36, 38}
UNLOCK = {'scissors': 5, 'wand': 9, 'clock': 15, 'hammer': 23}
INTRO = {10: 'S48', 14: 'S52'}                                           # chain and wall cards, from their v3 slots
KEEP = {1, 2, 3, 5, 6, 9, 15, 23}                                        # tutorial / intro / booster cards stay in their slot
assert len(ORDER) == 40 == len(set(ORDER))

old = {l['lv']: l for l in v3['levels']}
out = []
for slot, i in enumerate(ORDER, 1):
    l = json.loads(json.dumps(pool[i]))
    if 'zeydbot' in l:                                                   # Z levels: the audit reads the slot from here
        l['fabrika'] = {'slot': slot, 'candidate': i, 'status': 'finalist', 'genre': l.get('genre') or 'orta',
                        'psych': 'SET40 v4 (zeydbot.json)', 'scores': {}, 'notes': []}
    l.pop('zeydbot', None)
    if i in TUNED:
        m = measured[TUNED[i]]
        l['bombs'] = m['bombs']
        l['solution'] = m['solution']
        l['version'] = l.get('version', 1) + 1
        l.setdefault('fabrika', {})
        if l['fabrika'] is not None:
            l['fabrika'] = dict(l['fabrika'], notes=(l['fabrika'].get('notes') or []) + [f'SET40 v4: {TUNED[i]}'])
    l['lv'] = slot
    l['unlock'] = dict(UNLOCK)
    l['boosters'] = {k: 1 if slot >= at else 0 for k, at in UNLOCK.items()}
    if l.get('fabrika'): l['fabrika'] = dict(l['fabrika'], slot=slot)
    o = old[slot]
    l['intro'] = o['intro'] if slot in KEEP else None
    l['bintro'] = o['bintro'] if slot in KEEP else None
    if slot in INTRO: l['intro'] = pool[INTRO[slot]]['intro']
    l.pop('hard', None); l.pop('tutorial', None)
    if 'tutorial' in o: l['tutorial'] = o['tutorial']; l['timer'] = max(l['timer'], 120)
    if slot in HARD: l['hard'] = True
    out.append(l)

# chains not before the chain card (10), walls not before the wall card (14)
for l in out:
    if l['lv'] < 10: assert all(len(p) == 1 for p in l['pieces']), l['id']
    if l['lv'] < 14: assert not l['sealed'], l['id']

head = json.dumps({k: v for k, v in dict(v3, note='SET40 v4 (PM_MESAJ_23, docs/SET40_V4.md). v3: redesign_v3.json').items() if k != 'levels'},
                  ensure_ascii=False, indent=2)[:-2]
body = ',\n'.join('    ' + json.dumps(l, ensure_ascii=False, separators=(',', ':')) for l in out)
open(LV + 'redesign.json', 'w', encoding='utf-8', newline='\n').write(head + ',\n  "levels": [\n' + body + '\n  ]\n}\n')
print('yazıldı:', ' '.join(f"{l['lv']}:{l['id']}v{l['version']}" for l in out))
