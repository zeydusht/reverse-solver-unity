# SET40 v4 — önerilen sıralama (29 Ekim 2026, YAYINDA DEĞİL)

Kaynak: Zeyd'in `deneme1` oyunu (Supabase plays, 40 bölüm, 1–10 zorluk puanı ve 👍/👎). Zeyd onaylayınca uygulanır ve yayınlanır.

## Okuma

- 1–18 düz: 15. bölüm dışında hepsi 1–2.
- 👎 verilen 7 bölüm (10, 11, 26, 27, 28, 29, 31): biri 3, diğerleri 1 puan. 27 dışındakiler bombasız.
- Zeyd'in puanını belirleyen asıl şey bomba payı (ve tahta boyu). Bombasız bölümler 1 puan alıyor. Mevcut ZeydBot modeli de bombasız her bölüme 1,1 diyor, yani bu iki veri örtüşüyor.
- 23 (9 puan, bombayla bir kayıp) uyarısız bir sıçrama. 37 (1 puan) zirvenin ortasında boşluk. 40 (8 puan) 39'dan (10) zayıf.

## Tablo

"Zeyd puanı" o slottaki **eski** bölümün `deneme1` puanıdır. "ZeydBot tahmini" **yeni** bölüm içindir:
- **ölçüldü (deneme1):** Zeyd bu bölümü bu oyunda oynadı.
- **ölçüldü (Z oturumu):** Zeyd aynı bölümü ZeydBot setinde `ddd` adıyla oynadı.
- **ölçülmedi:** Bölüm yeniden ayarlanacak (`*`), tahmin ayardan sonra bot ve FunSim ile çıkar.

| Bölüm | Eski id | Yeni id | Zeyd puanı | ZeydBot tahmini | Hedef | Not |
|---|---|---|---|---|---|---|
| 1 | S41 | S41 | 1 | 1 (deneme1) | 1 | öğretici |
| 2 | S42 | S42 | 1 | 1 (deneme1) | 1 | öğretici |
| 3 | S43 | S43 | 1 | 1 (deneme1) | 1–2 | çivi tanıtımı |
| 4 | S06 | S06 | 2 | 2 (deneme1) | 2 | |
| 5 | S44 | S44 | 1 | 1 (deneme1) | 2 | makas |
| 6 | S45 | S45 | 2 | 2 (deneme1) | 2 | bomba tanıtımı |
| 7 | S46 | S46 | 1 | 1 (deneme1) | 2–3 | |
| 8 | S47 | S47* | 1 | ölçülmedi | 3 | 2 bombanın fitili kısalır (29/35 → ~16/20) |
| 9 | S20 | S20* | 2 | ölçülmedi (~3) | 3 | değnek; fitil 12 → 10 |
| 10 | S48 👎 | Z24 | 1 | 3 (Z oturumu) | 3 | zincir tanıtım kartı Z24'e taşınır (duvarsız) |
| 11 | S49 👎 | S21 (=Z19) | 1 | 3 (Z oturumu) | 3–4 | duvarsız |
| 12 | S50 | S51* | 1 | ölçülmedi | 3–4 | fitil 15 → ~10 |
| 13 | S51 | S34 (=Z22) | 1 | 5 (deneme1) | 4 | 33'ten öne alınır (duvarsız) |
| 14 | S52 | Z21 | 1 | 3 (Z oturumu) | 3–4 | duvar tanıtım kartı Z21'e taşınır |
| 15 | S13 | S13 | 4 | 4 (deneme1) | 4 | saat |
| 16 | S17 | Z04 | 2 | 3 (Z oturumu) | 4 | |
| 17 | S10 | S18* | 1 | ölçülmedi | 4–5 | +1 bomba |
| 18 | S18 | S22 (=Z15) | 2 | 5 (deneme1) | 5 | 21'den öne alınır |
| 19 | S28 | S28 | 9 | 9 (deneme1) | 7–9 | zor uyarısı |
| 20 | S24 | S24 | 7 | 7 (deneme1) | 6 | |
| 21 | S22 | Z23 | 5 | 5 (Z oturumu) | 5 | |
| 22 | S19 | S19 | 4 | 4 (deneme1) | 4–5 | |
| 23 | S27 | S27 + zor uyarısı | 9 | 9 (deneme1) | 7–8 | çekiç tanıtımı yerinde kalır, uyarı kartı eklenir |
| 24 | S14 | S14 | 4 | 4 (deneme1) | 4–5 | |
| 25 | S35 | S35 | 10 | 10 (deneme1) | 8 | zor uyarısı |
| 26 | S26 👎 | S17* | 1 | ölçülmedi | 3–4 | nefes; +1 bomba |
| 27 | S25 👎 | S10* | 3 | ölçülmedi | 3–4 | nefes; +1 bomba |
| 28 | S30 👎 | S50* | 1 | ölçülmedi | 3 | nefes; +1 bomba |
| 29 | S11 👎 | S52* | 1 | ölçülmedi | 3 | nefes; +1 bomba |
| 30 | S37 | S37 | 10 | 10 (deneme1) | 8–9 | zor uyarısı |
| 31 | S23 👎 | S08* | 1 | ölçülmedi | 3–4 | nefes; +1 bomba |
| 32 | S31 | S31 | 9 | 9 (deneme1) | 7–8 | |
| 33 | S34 | S39 (=Z29) | 5 | 6 (Z oturumu) / 8,5 (v2) | 7 | |
| 34 | S32 | S32 | 8 | 8 (deneme1) | 7–8 | |
| 35 | S36 | S36 | 8 | 8 (deneme1) | 8 | zor uyarısı |
| 36 | S29 | S29 | 9 | 9 (deneme1) | 8–9 | zor uyarısı |
| 37 | S08 | Z25 | 1 | 4 (Z oturumu) | 4–5 | zirvedeki kısa nefes |
| 38 | S38 | S38 | 10 | 10 (deneme1) | 9 | zor uyarısı |
| 39 | S33 | S40 | 10 | 8 (deneme1) | 8–9 | 40 ile yer değiştirir |
| 40 | S40 | S33 | 8 | 10 (deneme1) | 10 | final |

## Ne değişiyor

- **Yeni gelen 8 bölüm, Zeyd'in ölçtüğü puanlarla:** Z04, Z21, Z23, Z24, Z25, S21 (=Z19), S39 (=Z29); ayrıca S34 ve S22 öne alınıyor.
- **Ayarlanacak 9 bölüm (`*`):** S47, S20, S51, S18, S17, S10, S50, S52, S08. Hepsine bomba eklenecek ya da fitili kısaltılacak. Oynanışı değiştiği için her birinin `version`'ı artar. Fitiller, ilk denemede kazanma oranı hedefe uyana kadar FunSim ile taranır (SHOW1'de yaptığımız gibi). Her birinin bot sonucu bu tabloya eklenir.
- **Setten çıkan 7 bölüm:** S48, S49, S26, S25, S30, S11, S23. Hepsi 👎 aldı.
- **Tanıtım kartları:** Zincir kartı S48'den Z24'e, duvar kartı S52'den Z21'e taşınıyor. Booster tanıtımları (5, 9, 15, 23) aynı slotta kalıyor.
- **23:** Bölüm yerinde kalıyor, zor uyarısı ekleniyor. Bunun sebebi kilit açılma kuralı: çekiç 23'te açılıyor ve tanıtımı da bu bölümde.
- **Benzerlik (M16, Tools/level_similarity.py):** Yeni ve kalan bölümler arasındaki en yüksek benzerlik %54 (Z23–S35), sonra %47 (S28–S35, zaten sette). Kopya yok. Z23 istenmezse 21 için Z34 kullanılabilir; ama Z34'ün fitilleri Z23 ile aynı.
- **Z bölümlerinin puanları:** Zeyd bunları ZeydBot setinde oynadı ve hatırlıyor olabilir. Yeni bir oyuncu için biraz daha zor gelebilirler.

## Eğri (hedef)

1–7: 1–2 · 8–18: 3 → 5 (yavaş artış) · 19 zor · 20–24: 4–7, 23 uyarılı · 25 zor · 26–29 ve 31: nefes, 3–4 · 30 zor · 32–36: 7–9 · 37 kısa nefes (4–5) · 38–40: 9–10.

## Onaydan sonra

1. Ayarlanacak 9 bölüm üretilir; fitiller taranır, SmartPlayer ve 40/40 açılış kontrolü yapılır.
2. `redesign.json` v4 yazılır, id'ler korunur, değişen bölümlerin `version`'ı artar. 40 tablonun sırası slotlara göre kalır.
3. Önizleme yayınlanır. Kök adres, Zeyd "kökü güncelle" demeden değişmez.
