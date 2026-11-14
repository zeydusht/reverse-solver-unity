# PM_69: replay_pt1.csv ve replay_pt1_steps.csv özetleri (yalnız standart kütüphane).  python -I pm69_replay_ozet.py
import csv, statistics as st
from collections import Counter, defaultdict
A = list(csv.DictReader(open('replay_pt1.csv', encoding='utf-8')))
S = list(csv.DictReader(open('replay_pt1_steps.csv', encoding='utf-8')))
P = [a for a in A if a['result']]
boost = lambda a: 'wand' in a['inferred'] or 'scissors' in a['inferred']
okp = lambda xs: 100 * sum(a['replay_ok'] == 't' for a in xs) / max(1, len(xs))
print(f"replay: {len(A)} deneme, plays satırı olan {len(P)}, uyum {okp(P):.1f}% (booster'sız {okp([a for a in P if not boost(a)]):.1f}%, booster'lı {okp([a for a in P if boost(a)]):.1f}% / {sum(map(boost, P))})")
print('uyumsuz nedenler:', dict(Counter(a['reason'] for a in A if a['replay_ok'] == 'f')))
ok = [a for a in A if a['replay_ok'] == 't']; okid = {a['row_id'] for a in ok}
B = [a for a in ok if a['result'] == 'bomb']
off = sum(int(a['off_path_exits']) for a in B); on = sum(int(a['on_path_exits']) for a in B)
print(f"bomba ölümü {len(B)}: ölüm başına yol dışı çıkış {off/len(B):.1f}, yolda {on/len(B):.1f}, yol dışı payı {off/(off+on):.2f}")
by = defaultdict(list)
for a in B: by[a['level_id']].append(a)
print('bölüm  ölüm  yol_dışı  yolda')
for lv, xs in sorted(by.items(), key=lambda kv: -len(kv[1]))[:16]:
    print(f"{lv:5} {len(xs):5} {st.mean(int(a['off_path_exits']) for a in xs):8.1f} {st.mean(int(a['on_path_exits']) for a in xs):6.1f}")
J = ['jam_lock', 'jam_chain', 'jam_wall', 'jam_piece', 'jam_tab', 'jam_free', 'jam_nodir']
tot = {j: sum(int(a[j]) for a in ok) for j in J}; T = sum(tot.values())
print('takılma nedeni (pay):', {j[4:]: round(v / T, 3) for j, v in tot.items()}, 'toplam', T)
S = [s for s in S if s['row_id'] in okid]
for s in S:
    s['dt'] = min(max(int(s['dt_ms']), 0), 60000) / 1000; s['j'] = int(s['jams']); s['lim'] = s['limiters']
def g(name, xs):
    if not xs: print(f"  {name:<26} adım      0"); return
    print(f"  {name:<26} adım {len(xs):6}  sn/çıkış {st.mean(x['dt'] for x in xs):5.2f}  medyan {st.median(x['dt'] for x in xs):5.2f}  takılma/çıkış {st.mean(x['j'] for x in xs):.2f}")
print('adım düzeyi (uyumlu denemeler):')
N = [s for s in S if s['narrow'] == 't']; W = [s for s in S if s['narrow'] == 'f']
g('geniş (open>2)', W); g('dar (open<=2)', N)
g('dar + gizli', [s for s in N if s['hidden'] == 't']); g('dar + görünür', [s for s in N if s['hidden'] == 'f'])
for c in ['lock', 'chain', 'wall']:
    g(f'dar + {c} kısıyor', [s for s in N if c in s['lim']]); g(f'geniş + {c} var', [s for s in W if c in s['lim']])
g('dar + engelsiz', [s for s in N if not s['lim']]); g('geniş + engelsiz', [s for s in W if not s['lim']])
for k in [1, 2, 3, 4]: g(f'open_n = {k}', [s for s in S if int(s['open_n']) == k])
g('open_n >= 5', [s for s in S if int(s['open_n']) >= 5])
L = defaultdict(list)
for s in S: L[s['level_id']].append(s)
with open('replay_pt1_level_ozet.csv', 'w', newline='', encoding='utf-8') as f:
    w = csv.writer(f); w.writerow(['level_id', 'steps', 'narrow_share', 'hidden_share', 'sec_per_exit', 'jams_per_exit', 'open_mean'])
    for lv, xs in sorted(L.items()):
        w.writerow([lv, len(xs), round(sum(x['narrow'] == 't' for x in xs) / len(xs), 3), round(sum(x['hidden'] == 't' for x in xs) / len(xs), 3),
                    round(st.mean(x['dt'] for x in xs), 2), round(st.mean(x['j'] for x in xs), 2), round(st.mean(int(x['open_n']) for x in xs), 2)])
