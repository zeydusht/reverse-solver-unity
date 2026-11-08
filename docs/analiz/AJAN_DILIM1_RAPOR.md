PM KARARI GEREKİYOR: Tanıtımda "rastgele bot %15–50 kazanır" kuralı (R12) "fitil = en erken çıkış + 2" (R10) ile birlikte sağlanamıyor. 45 tanıtım adayının en yükseği %9,8; R12 kalsın mı, UYARI mı olsun, yoksa eşik %1–10'a mı insin?

# Ajan dilim 1 — bomba yayı 6–7 (PM_MESAJ_48 + 48B, 8 Kasım)

**Oynanacak link:** https://zeydusht.github.io/reverse-solver-unity/p21/?set=ajan1&lang=tr
Sırayla: 1 = 06-A (3×5, 23 sn) · 2 = 06-B (3×6, 30 sn) · 3 = 07-A (5×7, 2 bomba + duvar, 33 sn) · 4 = 07-B (4×6, kolay karşılaştırma). Hamleler set=zeydbot gibi kaydedilir.

**Zeyd ne hissetmeli:**
- **06:** "Bomba gerçekten patlar, önce onun yolunu açmalıyım." Bombayı unutan 6. hamlede kaybeder; hatırlayan rahat bitirir.
- **07:** "İki bombanın sırasını ben seçiyorum, yanlış sıra kaybettirir." Ortada 4 adımlık tek parça zinciri var, sonra tahta açılıyor.

## Döngü nasıl çalıştı
`python Tools/ajan/dongu.py --kart docs/bolum_kartlari/<kart>.json --uret`. Akış: kart → tarif → üretici → ölçüm (hikâye eğrisi, 4 bot, FunSim, ZeydBot modeli, eğri/siluet benzerliği) → kural tabanlı eleştirmen → en fazla 3 tur. Yeni kartla aynı komut çalışıyor.

| Kart | Koşu | Aday | GEÇTİ | Ana ret sebepleri |
|---|---|---|---|---|
| p21_06 | eski kurallar (PM_48) | 9 | 0 | final temposu (K5), kalan süre |
| p21_06 | 48B kuralları | 9 | 0 | rastgele bot < %15, FunSim < %65, tehdit adımı |
| p21_06 | 48B + küçük tahta (3×5, 3×6, 6×3) | 9 | 0 | yalnız rastgele bot (%0–3) ve süre |
| p21_06 | süre ayarı (23 / 30 sn) | 2 | 0 | 06-A: yalnız R12 rastgele bot · 06-B: R12 + FunSim %64,9 |
| p21_07 | 3 tarif × 3 tur | 27 | 1 | ilk tuzak ↔ tuzaklı adım, boğaz uzunluğu, FunSim çok düşük |

Döngüde düzeltilen ölçüm hataları:
- MonteCarlo ve FunSim, zincirli parça reddedilince sonsuza kadar dönüyordu (saat ilk hamlede başladığı için). Editör koduna döngü koruması eklendi.
- FunSim bomba tanıtım kartını hesaba katmıyordu. Artık kart varsa oyuncunun bombaya dikkati en az 0,8.
- R11 tehdidi bombaya giden oyuncunun yolunda ölçüyordu. Bu yolda pay hiç 2'ye inmediği için R11 hiç geçemiyordu. Artık tehdit, bombayı unutan oyuncunun payının 2'ye indiği adım (fitil − 2).

## Finalistler ve eski 6–7
| | Adım | Eğri (seçenek) | Fitil (en erken) | Unutan bot | FunSim ilk deneme | Kalan süre (FunSim medyanı) |
|---|---|---|---|---|---|---|
| eski 6 (S45) | 20 | 9→11→1, boğaz yok | 23 | **kazanır** | %100 | — |
| eski 7 (S46) | 25 | 10→1 düz | 33 | **kazanır** | %100 | — |
| 06-A | 15 | 8,7,7,7,7,6,5,5,5,4,3,3,3,2,1 | 6 (4) | kaybeder (6. hamle) | %65 | %48 |
| 06-B | 18 | 7,6,6,6,6,7,7,7,6,6,5,5,4,4,3,2,2,1 | 6 (4) | kaybeder | %64,9 | %50 |
| 07-A | 35 | 13,13,13,12,12,**1,1,1,1**,13,…,1 | 8 ve 9 | kaybeder | %4,7 | %21 (ZeydBot %98 kazanır, %38 kalır) |

Kitap kontrolleri:
- 06-A: R12 rastgele bot dışında hepsi GEÇTİ.
- 07-A: hepsi GEÇTİ. UYARI'lar: fitil/en erken çıkış ölçülemedi, çünkü 30 parçanın üstünde kesin arama çok yavaş. Final temposu da UYARI.
- Bombayı unutan bot dört bölümde de kaybediyor.

## Açık sorunlar
1. **R12 çelişkisi** (en üstte): rastgele oyuncu, fitil + 2 hamle içinde tam o 2–3 engelleyiciyi seçemiyor.
2. **07'de tuzak ile kazanılabilirlik çatışıyor:** tuzak çıkması için pay dar olmalı, dar pay da FunSim'in kazanma oranını %2–12'ye düşürüyor. SET40'ta 7. bölüm %100, 8. bölüm %47. ZeydBot modeli (Zeyd'in oyunu) 07-A'yı %98 kazanıyor. Kart "normal" tip için FunSim alt sınırı koymuyor. Öneri: normal tipte FunSim ≥ %30, yoksa 07 tipi "zor" olsun.
3. **R16 / K5:** SET40'ta son %30 parçanın süre payı medyanı 0,29 (tanıtımda 0,29, normalde 0,29, süper zorda 0,26). Oyuncu modelinde hamleler eşit sürdüğü için %20'ye hiçbir bölüm inmiyor. Şimdilik UYARI.
4. **verify_build** canlı p20'de de 5/7 veriyor (`card` başlık–çerçeve 7 pt; `redesign` S41'de sonuç kartı yok). p21 bunu değiştirmedi, kontroller p20 arayüzünün gerisinde kalmış.
