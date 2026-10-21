# SET40 v1 document (docs/analiz/zeydbot/SET40.md) from Logs/zdesign/set40_plan.json.
# Prediction per slot, with its source:
#   Z levels   Zeyd's own 1-10 and his first real attempt (ham_plays.csv) — measured, not modelled
#   R levels   bombless: Zeyd's mean 1-10 on the bombless levels he rated (Z03 Z05 Z06 Z09 Z11 Z14 Z20 Z27 Z28 Z30)
#              and his first-try win rate there; with bombs (C30): the bomb-slack baseline (zeydbot_model.baseline)
import csv, json, os, sys
sys.path.insert(0, os.path.dirname(__file__))
import zeydbot_model as zm
from level_similarity import similarity

plan = json.load(open('Logs/zdesign/set40_plan.json', encoding='utf-8'))
plays = sorted(csv.DictReader(open(os.path.join(zm.D, 'ham_plays.csv'), encoding='utf-8')), key=lambda p: int(p['attempt']))
score, first = {}, {}
for p in plays:
    l = p['level_id']
    if p['zorluk_puani'] and l not in score: score[l] = int(p['zorluk_puani'])
    if l not in first and not (p['result'] == 'quit' and p['moves'] == '0'): first[l] = p
bombless = ['Z03', 'Z05', 'Z06', 'Z09', 'Z11', 'Z14', 'Z20', 'Z27', 'Z28', 'Z30']
bl_score = sum(score[l] for l in bombless if l in score) / sum(1 for l in bombless if l in score)
bl_win = sum(first[l]['result'] == 'win' for l in bombless) / len(bombless)

R = {l['id']: l for l in json.load(open('Logs/zdesign/redesign_eski.json', encoding='utf-8'))['levels']}
Z = {l['id']: l for l in json.load(open('Assets/_Game/Levels/zeydbot.json', encoding='utf-8'))['levels']}
rows = []
for s in plan:
    if s['src'] == 'Z':
        f = first[s['id']]
        pred = f"{score[s['id']]} (Zeyd) / {'kazandı' if f['result'] == 'win' else f['result']}"
    elif s['bombs'] == 0:
        pred = f"{bl_score:.1f} / %{bl_win * 100:.0f}"
    else:
        pred = 'taban: bomba payı (ölçülmedi)'
    rows.append(f"| {s['slot']} | {s['target']} | {pred} | {s['idea']} | {s['size']}, {s['pieces']} parça, {s['bombs']} bomba | {s['id']} |")

lv = [(R if s['src'] == 'R' else Z)[s['id']] for s in plan]
sims = sorted(((similarity(a, b)[0], plan[i]['slot'], plan[j]['slot']) for i, a in enumerate(lv) for j, b in enumerate(lv) if i < j), reverse=True)
over50 = sum(1 for x in sims if x[0] >= .5)
sizes = {}
for s in plan: sizes[s['size']] = sizes.get(s['size'], 0) + 1

doc = f"""# SET40 v1 (PM_MESAJ_4, PM kararı 2026-10-21 (2) = A)

`?set=redesign` altında (Build 5z6); bayraklar kapalı, canlı 40 değişmedi. Slotlar `Tools/set40_plan.py`, belge `Tools/set40_doc.py`.

**Tahmin sütunu:** Zeyd'in oynadığı bölümlerde kendi puanı ve ilk denemesi (ölçüm). Yeni bombasız bölümlerde, Zeyd'in bombasız 10 bölümdeki ortalaması {bl_score:.1f} ve ilk deneme kazanma oranı %{bl_win * 100:.0f}. ZeydBot v1 bu sette kullanılmadı: zirve tipinde geçersiz, bombasızlarda bu ortalamadan farksız.

| Slot | Hedef | Tahmin (puan / ilk deneme) | Fikir | Tahta | Kaynak |
|---|---|---|---|---|---|
""" + "\n".join(rows) + f"""

## M16 benzerlik (döndürme/aynadan bağımsız, eşik %65)

- 780 çiftin en yükseği %{sims[0][0] * 100:.0f} (slot {sims[0][1]} – {sims[0][2]}). %50 ve üstü {over50} çift.
- Tahta boyutları: {', '.join(f'{k} ({v})' for k, v in sorted(sizes.items()))}.

## Etiketli eksikler (PM kararı)

- **Zor slotların hepsi 7×8.** Farklı boyutta zor malzeme yok. Farklı boyutta zor üretimi ayrı iş (en çok 1 gece).
- **Zirveleri (29, 35, 37, 40) ve bütün Z bölümlerini Zeyd daha önce oynadı.** Onay oturumunda tanıyacak; o oturum yalnızca akış/sıra/kalibrasyon onayı.
- **Tanıtım sırası korundu** (çivi 3, makas 5, mühür 7, değnek 9, zincir 12, saat 15, bomba 20, çekiç 23). Bu yüzden 1–19'da bomba yok. Eğri önerisindeki 10. ve 19. slottaki "Z12 tipi" yerine, bombasızlar arasında en çok yanıltıcı parçası olanlar kondu (hedef 4–5).
- Eğri önerisindeki 4–7 basamağı veriyle değil, iki uç arasında tahminle kuruldu (`zorluk_egrisi_onerisi.md`).
"""
open('docs/analiz/zeydbot/SET40.md', 'w', encoding='utf-8').write(doc)
print('yazıldı; en yüksek benzerlik', sims[0])
