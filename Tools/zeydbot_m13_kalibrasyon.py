# Recipe v2 calibration (PM_MESAJ_3): M13-M15 of the hard levels (ZeydDesign.Check -> Logs/zdesign/kalibrasyon)
# against Zeyd's 1-10, and in his own first real attempts (hamleler.csv, yaniltici column) whether thinking time
# and touches jump at key moments (visible = options + misleading >= 4, right = 1).
#   python Tools/zeydbot_m13_kalibrasyon.py  -> stdout (markdown)
import csv, os, sys
sys.path.insert(0, os.path.dirname(__file__)); import zeydbot_model as zm
D = zm.D
score = {}
for p in sorted(csv.DictReader(open(os.path.join(D, 'ham_plays.csv'), encoding='utf-8')), key=lambda p: int(p['attempt'])):
    if p['zorluk_puani'] and p['level_id'] not in score: score[p['level_id']] = int(p['zorluk_puani'])
F = {r['level_id']: r for r in csv.DictReader(open('Logs/zdesign/kalibrasyon/taslak_ozellik.csv', encoding='utf-8'))}

# Zeyd's moves: first real attempt per level (a 0-move quit does not count)
att = {}
for r in csv.DictReader(open(os.path.join(D, 'denemeler.csv'), encoding='utf-8')):
    if r['level_id'] in F and r['level_id'] not in att and r['hamle'] != '0': att[r['level_id']] = r['attempt']
moves = [m for m in csv.DictReader(open(os.path.join(D, 'hamleler.csv'), encoding='utf-8')) if m['level_id'] in att and m['attempt'] == att[m['level_id']]]
stat = {}
for l in F:
    key_t, other_t, key_touch, other_touch, nk, no = [], [], 0, 0, 0, 0
    pending_touch = 0
    for m in [m for m in moves if m['level_id'] == l]:
        if m['kind'] == 'touch': pending_touch += 1; continue
        if m['kind'] != 'exit' or not m['secenek'] or m['move_no'] == '1': continue
        key = int(m['secenek']) + int(m['yaniltici'] or 0) >= 4 and m['dogru'] == '1'
        (key_t if key else other_t).append(int(m['think_ms']))
        if key: key_touch += pending_touch; nk += 1
        else: other_touch += pending_touch; no += 1
        pending_touch = 0
    med = lambda v: sorted(v)[len(v) // 2] if v else 0
    stat[l] = (nk, med(key_t), med(other_t), key_touch / max(nk, 1), other_touch / max(no, 1))

print('| Bölüm | Puan | M13 anahtar (2. yarı, dizi) | M14 süpürme / yön / düz şerit | M14 imza | M15 kalan pay | Zeyd: anahtar an sayısı | düşünme medyan anahtar / diğer (ms) | dokunma/hamle anahtar / diğer |')
print('|---|---|---|---|---|---|---|---|---|')
for l in sorted(F):
    r, s = F[l], stat[l]
    print(f"| {l} | {score.get(l, '—')} | {r['m13_anahtar']} ({r['m13_ikinci_yari']}, {r['m13_art_arda']}) | {r['m14_supurme']} / {r['m14_yon']} / {'var' if r['m14_duz_seritten'] == '1' else 'yok'} | {r['m14_imza']} | %{float(r['m15_kalan_pay']) * 100:.0f} | {s[0]} | {s[1]} / {s[2]} | {s[3]:.2f} / {s[4]:.2f} |")
L = [l for l in sorted(F) if l in score]
for name, f in (('M13 anahtar', lambda l: float(F[l]['m13_anahtar'])), ('M13 ikinci yarı', lambda l: float(F[l]['m13_ikinci_yari'])),
                ('yanıltıcı/adım', lambda l: float(F[l]['yaniltici_adim'])), ('M14 süpürme', lambda l: float(F[l]['m14_supurme'])),
                ('Zeyd anahtar an sayısı', lambda l: stat[l][0])):
    print(f"\nPuanla Spearman ({len(L)} zor bölüm), {name}: {zm.spearman([f(l) for l in L], [score[l] for l in L]):+.2f}", end='')
k = [t for l in L for t in [stat[l][1]] if stat[l][0]]; o = [stat[l][2] for l in L if stat[l][0]]
print(f"\n\nAnahtar anda düşünme medyanı diğer hamlelerden uzun: {sum(stat[l][1] > stat[l][2] for l in L if stat[l][0])}/{sum(1 for l in L if stat[l][0])} bölüm")
