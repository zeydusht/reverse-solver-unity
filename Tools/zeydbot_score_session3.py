# Scores the frozen Z21-Z30 predictions (docs/analiz/zeydbot/tahmin_Z21_Z30.md, commit fc4ef7c) against
# Zeyd's first attempts (docs/analiz/zeydbot/ham_plays.csv, from node Tools/zeydbot_fetch.js --all).
import csv, os, re, sys, subprocess
sys.path.insert(0, os.path.dirname(__file__)); import zeydbot_model as zm
D = zm.D
frozen = subprocess.run(['git', 'show', 'fc4ef7c:docs/analiz/zeydbot/tahmin_Z21_Z30.md'], capture_output=True, text=True, encoding='utf-8').stdout
pred = {}
for line in frozen.splitlines():
    m = re.match(r'\| (Z\d\d) \| \w+ \| (.*) \|$', line)
    if m: pred[m.group(1)] = [tuple(float(x.strip().lstrip('%')) for x in c.split('/')) for c in m.group(2).split(' | ')]
real = {p['level_id']: p for p in csv.DictReader(open(os.path.join(D, 'ham_plays.csv'), encoding='utf-8')) if p['attempt'] == '1' and p['level_id'] in pred}
L = sorted(pred)
print('| Bölüm | Gerçek: sonuç / kalan s / puan | ZeydBot | Rastgele | Bomba payı |'); print('|---|---|---|---|---|')
for l in L:
    r = real[l]; print(f"| {l} | {r['result']} / {r['left_s']} / {r['zorluk_puani']} | " + ' | '.join(f"%{w:.0f} / {s:.0f} / {p}" for w, s, p in pred[l]) + ' |')
print(); print('| Ölçüt | ZeydBot | Rastgele | Bomba payı |'); print('|---|---|---|---|')
names = range(3)
sp = [zm.spearman([pred[l][i][2] for l in L], [int(real[l]['zorluk_puani']) for l in L]) for i in names]
acc = [sum((pred[l][i][0] >= 50) == (real[l]['result'] == 'win') for l in L) for i in names]
won = [l for l in L if real[l]['result'] == 'win']
w25 = [sum(abs(pred[l][i][1] - float(real[l]['left_s'])) <= .25 * float(real[l]['left_s']) for l in won) for i in names]
print('| Puan sırası (Spearman, hedef ≥ 0,7) | ' + ' | '.join('%.2f' % v for v in sp) + ' |')
print('| Kazandı/kaybetti (eşik %50) | ' + ' | '.join(f'{v}/{len(L)}' for v in acc) + ' |')
print(f'| Kalan s ±%25 ({len(won)} kazanma) | ' + ' | '.join(f'{v}/{len(won)}' for v in w25) + ' |')
