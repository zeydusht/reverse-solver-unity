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
PK = {l['id']: l for l in json.load(open('Logs/zdesign/zirve_secilen.json', encoding='utf-8'))['levels']}
# peak metrics: the factory's check of the drafts (PK3 = P77_4 without the hole, measured in Logs/zdesign/zirve)
PM = {r['level_id']: r for f in ('Logs/zdesign/zirve2/taslak_ozellik.csv', 'Logs/zdesign/zirve/taslak_ozellik.csv') for r in csv.DictReader(open(f, encoding='utf-8'))}
PM['PK3'] = PM['P77_4']
# peak prediction: the M13 score (Tools/zeydbot_m13_score.py: 4.92 + 0.158 x M13 + 0.234 x misleading per step)
m13pred = lambda r: min(10, max(1, 4.92 + .158 * float(r['m13_anahtar']) + .234 * float(r['yaniltici_adim'])))
rows, dev, peaks = [], [], []
for s in plan:
    num = None
    if s['src'] == 'P':
        r = PM[s['id']]; num = m13pred(r)
        pred = f"{num:.1f} (M13 modeli) / rastgele %{float(r['rastgele_kazanma']) * 100:.0f}"
        peaks.append(f"| {s['slot']} | {s['id']} | {s['size']}{' delikli' if s['id'] in ('PK2', 'PK4') else ''} | {r['m17_derinlik']} (üst sınır) / %{float(r['m17_sira_basari']) * 100:.0f} | {r['engel_konum'].replace(';', ', ')} | {r['m13_anahtar']} | %{s['z_max'] * 100:.0f} / %{s['peak_max'] * 100:.0f} |")
    elif s['src'] == 'Z':
        f = first[s['id']]
        pred = f"{score[s['id']]} (Zeyd) / {'kazandı' if f['result'] == 'win' else f['result']}"; num = score[s['id']]
    elif s['bombs'] == 0:
        pred = f"{bl_score:.1f} / %{bl_win * 100:.0f}"; num = bl_score
    else:
        pred = 'taban: bomba payı (ölçülmedi)'
    if num is not None and abs(num - s['target']) >= 3: dev.append(f"slot {s['slot']} (hedef {s['target']}, tahmin {num:.1f})")
    rows.append(f"| {s['slot']} | {s['target']} | {pred} | {s['idea']} | {s['size']}, {s['pieces']} parça, {s['bombs']} bomba | {s['id']} |")

lv = [(R if s['src'] == 'R' else PK if s['src'] == 'P' else Z)[s['id']] for s in plan]
sims = sorted(((similarity(a, b)[0], plan[i]['slot'], plan[j]['slot']) for i, a in enumerate(lv) for j, b in enumerate(lv) if i < j), reverse=True)
over50 = sum(1 for x in sims if x[0] >= .5)
sizes = {}
for s in plan: sizes[s['size']] = sizes.get(s['size'], 0) + 1

doc = f"""# SET40 v2 (PM_MESAJ_5 + EK, 2026-10-21 gecesi)

`?set=redesign` altında (Build 5z7, kayıt açık: plays, moves, 1–10); bayraklar kapalı, canlı 40 değişmedi. Slotlar `Tools/set40_plan.py`, belge `Tools/set40_doc.py`.

**Tahmin sütunu:** Zeyd'in oynadığı bölümlerde kendi puanı ve ilk denemesi (ölçüm). Yeni bombasız bölümlerde, Zeyd'in bombasız 10 bölümdeki ortalaması {bl_score:.1f} ve ilk deneme kazanma oranı %{bl_win * 100:.0f}. ZeydBot v1 bu sette kullanılmadı: zirve tipinde geçersiz, bombasızlarda bu ortalamadan farksız.

| Slot | Hedef | Tahmin (puan / ilk deneme) | Fikir | Tahta | Kaynak |
|---|---|---|---|---|---|
""" + "\n".join(rows) + f"""

## Zirveler (yeni, tasarımcı ajan + fabrika; fikirler slot tablosunda)

| Slot | Bölüm | Tahta | M17 derinlik / rastgele sıra başarısı | Engel konumu | M13 | Benzerlik: en yakın Z / en yakın zirve (eşik %50) |
|---|---|---|---|---|---|---|
""" + "\n".join(peaks) + f"""

**Hedef düzeltmesi (PM_MESAJ_5 madde 4):** M13 modeli PK1–PK3'e ~5,5, PK4'e 6,0, PK5'e 6,6 verdi (hedef 9). Yeni tahtalar M17'de çok zor (rastgele sıra başarısı %0–6) ama M13 anahtar anları az (0–2; PK5'te 7). Hedefler buna göre 29/33/36 → 7, 38 → 8, 40 → 9 yapıldı. M13 modeli zayıf (LOO Spearman 0,23) ve oturum 5'i ~1 puan düşük tahmin etti; asıl ölçü Zeyd'in oynayışı.

M17 derinliği kazı aramasının bütçesi aşıldığında yedek yöntemle sayılıyor, gerçeğinden uzun olabilir (üst sınır). Okunur kenar bombası (fitil ≥ yol + 2, derinlik < 3) hiçbirinde yok.

## Hedeften ≥ 3 sapan slotlar

{', '.join(dev) if dev else 'Yok.'}

## M16 benzerlik (döndürme/aynadan bağımsız; zirveler %50, diğerleri %65)

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
