# ZeydBot v1 — Z21–Z30 tahminleri (DONDURULDU, 2026-10-20 gece)

Bu dosya Zeyd oturum 3'ü oynamadan **önce** commitlendi. Bu commit'ten sonra model değişmez; iyileştirmeler `zeydbot-v2` dalında.
Model: `Tools/zeydbot_model.py` (bu commit'teki hali) + `docs/analiz/zeydbot/yollar.csv` (ZeydBot.Paths, 12 yol/bölüm, bombaya yönelme 0,95).
Eğitim: yalnız Z01–Z20 ilk denemeleri. Z21–Z30'un hiçbir oyun sonucu indirilmedi ve okunmadı (`Tools/zeydbot_fetch.js` Z20 üstünü atar).
Yeniden üretmek: `python Tools/zeydbot_model.py predict Z21 Z22 Z23 Z24 Z25 Z26 Z27 Z28 Z29 Z30`.

Hücre: ilk denemede kazanma olasılığı / kazanırsa kalan saniye / 1–10 zorluk puanı.

| Bölüm | Tür | ZeydBot: ilk deneme kazanma / kalan s / puan | Taban rastgele oyuncu | Taban bomba payı |
|---|---|---|---|---|
| Z21 | orta | %100 / 35 / 2.6 | %65 / 30 / 5.6 | %69 / 31 / 5.5 |
| Z22 | orta | %100 / 38 / 2.5 | %67 / 30 / 5.6 | %69 / 31 / 5.5 |
| Z23 | zor | %82 / 87 / 3.6 | %53 / 66 / 5.9 | %56 / 66 / 6.1 |
| Z24 | orta | %100 / 35 / 2.6 | %76 / 31 / 5.3 | %69 / 31 / 5.5 |
| Z25 | zor | %92 / 83 / 3.0 | %61 / 65 / 5.7 | %56 / 63 / 6.1 |
| Z26 | zor | %80 / 102 / 3.7 | %93 / 88 / 4.5 | %56 / 74 / 6.1 |
| Z27 | kolay | %100 / 24 / 2.3 | %100 / 25 / 1.0 | %99 / 25 / 1.0 |
| Z28 | kolay | %100 / 40 / 2.4 | %100 / 47 / 1.0 | %99 / 47 / 1.0 |
| Z29 | zor | %0 / 0 / 8.7 | %53 / 50 / 5.9 | %56 / 50 / 6.1 |
| Z30 | kolay | %100 / 33 / 2.4 | %100 / 40 / 1.0 | %99 / 40 / 1.0 |

İlk satır parametreler (tüm Z01–Z20 ile): yukarıdaki tablonun ilk satırı değil, şu: 
parametreler: tuzak a=-2.039 b=2.631; düşünme c=['5.916', '-0.282', '0.208', '0.311'] tau=0.177 sigma=0.637; puan s=['1.768', '6.350', '1.714']

## Önceden kaydedilen başarı ölçütleri (PM)

- Zorluk sırası: tahmin edilen puan ile Zeyd'in 1–10 puanı arasında Spearman ≥ 0,7.
- Kalan saniye: ilk denemede kazanılan bölümlerde hata ±%25 içinde.
- Kazandı/kaybetti doğruluğu (eşik %50) raporlanır.
- Aynı ölçütler iki taban için de hesaplanır (aynı tablo); ZeydBot tabanları geçemezse açıkça yazılır.

## Dürüst not (dondurmadan önce bilinen)

Leave-one-out'ta (Z01–Z20, `loo.md`) ZeydBot bomba payı tabanını **geçmedi**: puan sırası 0,73 (taban 0,79), kalan s ±%25 içinde %73 (taban %93), kazan/kaybet doğruluğu %70 (taban %70). Rastgele oyuncu tabanını puan sırasında geçti (0,73 / 0,61).
