# ZeydBot score with M13 (PM_MESAJ_3 step 2: "ZeydBot puanına M13 özellik olarak girer"). Explainable and small:
# Zeyd's 1-10 on the hard levels = a + b * M13 key moments + c * misleading pieces per step (ridge), fitted on the
# 14 hard levels of Logs/zdesign/kalibrasyon (design only) and his answers (ham_plays.csv); leave-one-out first.
# Remaining seconds = M15 share x timer (Zeyd's Z12-type pace, 1.90 s a move).
#   python Tools/zeydbot_m13_score.py <taslak_ozellik.csv> ID...
import csv, os, sys
sys.path.insert(0, os.path.dirname(__file__)); import zeydbot_model as zm
score = {}
for p in sorted(csv.DictReader(open(os.path.join(zm.D, 'ham_plays.csv'), encoding='utf-8')), key=lambda p: int(p['attempt'])):
    if p['zorluk_puani'] and p['level_id'] not in score: score[p['level_id']] = int(p['zorluk_puani'])
K = {r['level_id']: r for r in csv.DictReader(open('Logs/zdesign/kalibrasyon/taslak_ozellik.csv', encoding='utf-8'))}
x = lambda r: [1.0, float(r['m13_anahtar']), float(r['yaniltici_adim'])]
L = [l for l in sorted(K) if l in score]
fit = lambda ls: zm.ols([x(K[l]) for l in ls], [score[l] for l in ls], l2=1.0)
pred = {t: zm.clamp_score(sum(a * b for a, b in zip(fit([l for l in L if l != t]), x(K[t])))) for t in L}
print(f"LOO ({len(L)} zor bölüm): Spearman {zm.spearman([pred[l] for l in L], [score[l] for l in L]):.2f}, ortalama mutlak hata {sum(abs(pred[l] - score[l]) for l in L) / len(L):.2f}")
c = fit(L)
print(f"katsayılar (tümü): sabit {c[0]:.2f}, M13 {c[1]:+.3f}, yanıltıcı/adım {c[2]:+.3f}")
D = {r['level_id']: r for r in csv.DictReader(open(sys.argv[1], encoding='utf-8'))}
print('| Taslak | M13-puan | kalan s (M15) |')
print('|---|---|---|')
for t in sys.argv[2:]:
    r = D[t]
    print(f"| {t} | {zm.clamp_score(sum(a * b for a, b in zip(c, x(r)))):.1f} | {float(r['m15_kalan_pay']) * float(r['sure_limiti']):.0f} |")
