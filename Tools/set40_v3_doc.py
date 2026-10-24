"""SET40 v3 slot table at the top of docs/analiz/zeydbot/SET40.md (v2 section kept below)."""
import json, re
p = 'docs/analiz/zeydbot/SET40.md'
old = open(p, encoding='utf-8').read()
if old.startswith('# SET40 v3'): old = old[old.index('# SET40 v2'):]
est = {}
for line in old.split('\n'):
    m = re.match(r'\| (\d+) \| [^|]+ \| ([^|]+) \|', line)
    if m and int(m.group(1)) not in est: est[int(m.group(1))] = m.group(2).strip()
L = json.load(open('Assets/_Game/Levels/redesign.json', encoding='utf-8'))['levels']
target = {1:'1',2:'1',3:'2',4:'2',5:'3',6:'2',7:'3',8:'4',9:'5',10:'2',11:'3',12:'3',13:'4',14:'3',15:'4',16:'3–4',17:'3–4',18:'3–4',19:'7',20:'5–6',25:'7–8',30:'7–8',37:'nefes',40:'10'}
rows = ['| Slot | Hedef (yeni oyuncu) | Zeyd tahmini (v2) | Engel (çivi/bomba/zincir/duvar) | Boyut | Bayrak | Kaynak |', '|---|---|---|---|---|---|---|']
for l in L:
    s = l['lv']; i = int(l['id'][1:]); new = i > 40
    chains = l['w'] * l['h'] - len(l['pieces']); walls = sum(len(v) for v in (l.get('sealed') or {}).values())
    flag = ' '.join(x for x in ['tutorial %d' % l['tutorial'] if l.get('tutorial') else '', 'zor uyarısı' if l.get('hard') else '',
                                'tanıtım: ' + l['intro']['title'] if l.get('intro') else '', 'booster: ' + l['bintro']['title'] if l.get('bintro') else ''] if x)
    t = target.get(s, '8–9' if s >= 35 else '4–6')
    rows.append(f"| {s} | {t} | {'yeni, ölçülmedi' if new else est.get(i, '—')} | {len(l.get('nails') or {})}/{len(l.get('bombs') or {})}/{chains}/{walls} | {l['w']}x{l['h']}, {len(l['pieces'])} parça | {flag} | {l['id']} {'(yeni, SetV3.Gen)' if new else '(v2 slot %d)' % i} |")
head = '''# SET40 v3 (PM_MESAJ_6/7, 2026-10-24): öğretici eğri

Build 5'in varsayılan seti (yayınlanmadı). Üretim: `SetV3.Gen` (12 yeni bölüm) + `Tools/make_set40_v3.py` (sıra, kartlar, bayraklar); tablo `Tools/set40_v3_doc.py`. v2: `Levels/redesign_v2.json`. Booster açılışları değişmedi (makas 5, değnek 9, saat 15, çekiç 23), tanıtımlar 3/6/10/14 ile çakışmıyor. S39 çıkarıldı ("eski usul"), 37 nefes. 15 üretilemedi → S13 (duvar+zincir+çivi). Hiçbir engel tanıtımından önce görünmüyor (betik kontrolü).

'''
open(p, 'w', encoding='utf-8', newline='\n').write(head + '\n'.join(rows) + '\n\n---\n\n' + old)
