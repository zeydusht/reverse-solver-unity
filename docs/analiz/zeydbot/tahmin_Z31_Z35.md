# Z31–Z35 tahminleri (DONDURULDU, 2026-10-20, Zeyd oynamadan önce)

İki tahmin var, ikisi de sınanacak.

## 1. ZeydBot v1 (dondurulmuş `fc4ef7c`)

`python Tools/zeydbot_score_drafts.py Logs/zdesign/puan4` ve `.../puan5` ile hesaplandı. Model kodu ve eğitim CSV'leri `fc4ef7c`'den okunuyor; taslak başına 3 bot yolu kullanıldı.

| Bölüm | v1 puan | İlk deneme kazanma | Kalan s (kazanırsa) |
|---|---|---|---|
| Z31 | 8,1 | %0 | — |
| Z32 | 6,1 | %38 | 66 |
| Z33 | 8,1 | %0 | — |
| Z34 | 8,2 | %0 | — |
| Z35 | 8,1 | %0 | 53 |
| Z12 (referans, gerçek: 9, süre bitti) | 4,4 | %67 | 68 |

Not: v1 Z12'ye 4,4 puan verdiği için bu tip bölümlerde zayıf bir ölçüt. Tahmin önceden kayıt olsun diye donduruldu.

## 2. Tasarımcı ajanın tahmini (kör, fabrika ölçmeden önce yazıldı)

| Bölüm | Zeyd kaç verir | Z12 gibi hisseder mi |
|---|---|---|
| Z31 | 8 | belki (evet'e yakın) |
| Z32 | 6 | hayır |
| Z33 | 8 | belki |
| Z34 | 7 | belki |
| Z35 | 7–8 | belki |

## Önceden kayıtlı başarı ölçütü (PM_MESAJ_2)

- Zeyd 5 bölümün en az 3'üne "Z12 gibi" derse yöntem kabul edilir.
- Okuma notu: Z31, Z33 ve Z35 Z12 türevi. Tanınma ihtimalinden dolayı Z32 ve Z34'ün cevabı ayrıca raporlanır (`Z31_Z35_tasarim.md`).

<details><summary>Bölüm türleri (Zeyd: açma)</summary>

| Bölüm | Taslak | Taban |
|---|---|---|
| Z31 | T32b | Z12 180° |
| Z32 | T37c | Z23 |
| Z33 | T31b | Z12 dikey ayna |
| Z34 | T41d | Z23 ayna |
| Z35 | T33b | Z12 yatay ayna |

</details>
