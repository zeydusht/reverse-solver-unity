"""SET40 v3 (PM_MESAJ_6/7): assemble redesign.json from the new levels (Builds/setv3/gen.json, SetV3.Gen)
and the v2 pool. Slot plan: tutorial 1-2, intros 3/6/10/14, hard 19/25/30, peaks 35-40, final 40.
Keeps v2 as Assets/_Game/Levels/redesign_v2.json. Run: python Tools/make_set40_v3.py"""
import json, os, shutil

LV = 'Assets/_Game/Levels/'
v2_path = LV + 'redesign_v2.json'
if not os.path.exists(v2_path):
    shutil.copy(LV + 'redesign.json', v2_path)
v2 = json.load(open(v2_path, encoding='utf-8'))
pool = {l['id']: l for l in v2['levels']}
gen = {l['v3slot']: l for l in json.load(open('Builds/setv3/gen.json', encoding='utf-8'))['levels']}

# slot -> v2 id (reused) or 'new'
PLAN = {4: 'S06', 9: 'S20', 15: 'S13', 16: 'S17', 17: 'S10', 18: 'S18', 19: 'S28', 20: 'S24',
        21: 'S22', 22: 'S19', 23: 'S27', 24: 'S14', 25: 'S35', 26: 'S26', 27: 'S25', 28: 'S30',
        29: 'S11', 30: 'S37', 31: 'S23', 32: 'S31', 33: 'S34', 34: 'S32',
        35: 'S36', 36: 'S29', 37: 'S08', 38: 'S38', 39: 'S33', 40: 'S40'}
HARD = {19, 25, 30, 35, 36, 38, 39, 40}
TUTORIAL = {1: 1, 2: 2}
UNLOCK = {'scissors': 5, 'wand': 9, 'clock': 15, 'hammer': 23}
BINTRO = {5: 'S05', 9: 'S09', 15: 'S15', 23: 'S23'}          # v2 levels holding each booster card

def intro(icon, title, body, tip):
    return {'icon': icon, 'title': title, 'body': body, 'tip': tip}

def first(d):
    return next(iter(d.values()))

next_id = max(v2['nextId'], 1 + max(int(i[1:]) for i in pool))
out = []
for slot in range(1, 41):
    if slot in PLAN:
        l = dict(pool[PLAN[slot]])
    else:
        l = dict(gen[slot]); del l['v3slot']
        l['id'] = 'S%02d' % next_id; next_id += 1
        l['version'] = 1
        l['fabrika'] = {'slot': slot, 'candidate': l['id'], 'status': 'finalist', 'genre': l.get('genre') or 'tanitim',
                        'psych': 'SET40 v3 (SetV3.Gen)', 'scores': {}, 'notes': []}
    l['lv'] = slot
    l['unlock'] = dict(UNLOCK)
    l['boosters'] = {k: 1 if slot >= at else 0 for k, at in UNLOCK.items()}   # redesign stock: 1 of each once open
    if l.get('fabrika'): l['fabrika'] = dict(l['fabrika'], slot=slot)
    l['intro'] = None
    l['bintro'] = pool[BINTRO[slot]]['bintro'] if slot in BINTRO else None
    l.pop('hard', None); l.pop('tutorial', None)
    if slot in HARD: l['hard'] = True
    if slot in TUTORIAL:
        l['tutorial'] = TUTORIAL[slot]; l['timer'] = max(l['timer'], 120)
    if slot == 3:
        n = first(l['nails'])
        l['intro'] = intro('hammer', 'Çivili parça', f'Çiviyi sökmek için {n} hamle yapman gerekiyor. Sayı her hamlende bir azalır, sıfırda parça serbest kalır.',
                           'Önce diğer parçaları çıkar, çivi kendiliğinden sökülür.')
    if slot == 6:
        n = first(l['bombs'])
        l['intro'] = intro('bomb', 'Bombalı parça', f'Bu bombayı {n} hamle içinde çıkarmalısın. Sayı her hamlende azalır; sıfıra inerse bölüm biter.',
                           'Bombanın yolunu açan parçaları bul ve önce onları çıkar.')
    if slot == 10:
        l['intro'] = intro('chain', 'Zincirli çift', 'Aynı renkte halka taşıyan iki parça zincirle bağlıdır ve tek parça gibi hareket eder. İkisinin de önü aynı yöne açık olmalı.',
                           'Zinciri takip et, iki ucunu da serbest bırak.')
    if slot == 14:
        l['intro'] = intro('wall', 'Mühürlü kenar', 'Kırmızı şeritli kenar bir duvardır, o yönden hiçbir parça çıkamaz.',
                           'Çıkış yönleri azaldı, parçaları başka yöne sürükle.')
    out.append(l)

v3 = {'format': v2['format'], 'nextId': next_id,
      'note': 'SET40 v3 (PM_MESAJ_6/7): öğretici eğri. 1-2 tutorial, 3/6/10/14 tanıtım, 19/25/30 zor, 35-40 zirve. v2: redesign_v2.json',
      'levels': out}
with open(LV + 'redesign.json', 'w', encoding='utf-8', newline='\n') as f:
    f.write('{\n  "format": %d,\n  "nextId": %d,\n  "note": %s,\n  "levels": [\n' % (v3['format'], next_id, json.dumps(v3['note'], ensure_ascii=False)))
    f.write(',\n'.join('    ' + json.dumps(l, ensure_ascii=False, separators=(',', ':')) for l in out))
    f.write('\n  ]\n}\n')

for l in out:
    print(l['lv'], l['id'], '%dx%d' % (l['w'], l['h']), len(l['pieces']), 'n%d b%d' % (len(l.get('nails') or {}), len(l.get('bombs') or {})),
          'H' if l.get('hard') else '', 'T%d' % l['tutorial'] if l.get('tutorial') else '', 'intro' if l['intro'] else '', 'bintro' if l['bintro'] else '')
