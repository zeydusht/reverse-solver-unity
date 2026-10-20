# Scores the frozen Z31-Z35 predictions (docs/analiz/zeydbot/tahmin_Z31_Z35.md, commit 6b602ba: frozen v1 + the
# designer agent's guess) against Zeyd's first real attempt (a 0-move quit does not count) in ham_plays.csv.
import csv, os, re, sys, subprocess
sys.path.insert(0, os.path.dirname(__file__)); import zeydbot_model as zm
frozen = subprocess.run(['git', 'show', '6b602ba:docs/analiz/zeydbot/tahmin_Z31_Z35.md'], capture_output=True, cwd=os.path.join(os.path.dirname(__file__), '..')).stdout.decode('utf-8')
bot, agent = {}, {}
for line in frozen.splitlines():
    m = re.match(r'\| (Z3[1-5]) \| ([\d,]+) \| %(\d+) \| ([^|]+) \|$', line)
    if m: bot[m.group(1)] = (float(m.group(2).replace(',', '.')), int(m.group(3)) / 100, m.group(4).strip())
    m = re.match(r'\| (Z3[1-5]) \| ([\d–]+) \| ([^|]+) \|$', line)
    if m: agent[m.group(1)] = (sum(map(float, m.group(2).split('–'))) / len(m.group(2).split('–')), m.group(3).strip())
plays = [p for p in csv.DictReader(open(os.path.join(zm.D, 'ham_plays.csv'), encoding='utf-8')) if p['level_id'] in bot]
first, score = {}, {}
for p in sorted(plays, key=lambda p: int(p['attempt'])):
    l = p['level_id']
    if l not in first and not (p['result'] == 'quit' and p['moves'] == '0'): first[l] = p
    if p['zorluk_puani'] and l not in score: score[l] = int(p['zorluk_puani'])
L = sorted(bot)
print('| Bölüm | Gerçek: ilk gerçek deneme / kalan s / puan | v1: puan / kazanma / kalan s | Ajan: puan / Z12 gibi mi |')
print('|---|---|---|---|')
for l in L:
    f = first[l]
    print(f"| {l} | {f['result']} (deneme {f['attempt']}) / {f['left_s'] if f['result'] == 'win' else '—'} / {score.get(l, '—')} | {bot[l][0]:.1f} / %{bot[l][1] * 100:.0f} / {bot[l][2]} | {agent[l][0]:g} / {agent[l][1]} |")
rated = [l for l in L if l in score]
print()
print(f"Puan sırası (Spearman, {len(rated)} bölüm): v1 {zm.spearman([bot[l][0] for l in rated], [score[l] for l in rated]):.2f}, ajan {zm.spearman([agent[l][0] for l in rated], [score[l] for l in rated]):.2f}")
print(f"Kazandı/kaybetti (eşik %50): v1 {sum((bot[l][1] >= .5) == (first[l]['result'] == 'win') for l in L)}/{len(L)}")
print(f"Ortalama mutlak puan hatası: v1 {sum(abs(bot[l][0] - score[l]) for l in rated) / len(rated):.2f}, ajan {sum(abs(agent[l][0] - score[l]) for l in rated) / len(rated):.2f}")
