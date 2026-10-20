# Z36–Z40 tahminleri (DONDURULDU, 2026-10-21 gecesi, Zeyd oynamadan önce)

## 1. ZeydBot M13 puanı (`python Tools/zeydbot_m13_score.py Logs/zdesign/hizli5/taslak_ozellik.csv ...`)

Puan = 4,92 + 0,158 × M13 anahtar an + 0,234 × yanıltıcı/adım. Model 14 zor bölümün tasarım ölçümleri ve Zeyd'in puanlarıyla eğitildi. Kalan saniye = M15 payı × süre; M15 Zeyd'in Z12 tipi hızını (1,90 s/hamle) kullanıyor.
**Zayıf model:** 14 bölümde leave-one-out Spearman 0,23, ortalama mutlak hata 1,5 puan.

| Bölüm | M13-puan | Kalan s (kazanırsa) |
|---|---|---|
| Z36 | 8,1 | 17 |
| Z37 | 7,5 | 17 |
| Z38 | 8,2 | 17 |
| Z39 | 6,7 | 17 |
| Z40 | 8,1 | 17 |

ZeydBot v1 bu tipte geçersiz (Z12'ye 4,4 veriyor; PM 2026-10-20 kararı), bu yüzden kazanma olasılığı tahmini yok.

## 2. Tasarımcı ajanın tahmini (kör, fabrika ölçmeden önce)

| Bölüm | Zeyd kaç verir | Z12 gibi mi |
|---|---|---|
| Z36 | 8–9 | evet |
| Z37 | 6–7 | hayır-belki |
| Z38 | 8–9 | belki-evet |
| Z39 | 6–7 | hayır-belki |
| Z40 | 8 | belki |

## Önceden kayıtlı ölçüt (PM_MESAJ_3)

- En az 3 bölüm puan ≥ 8 alırsa ve Zeyd "şablon yok" derse, 40 bölümlük yeni set için PM onayı istenir.

<details><summary>Bölüm türleri (Zeyd: açma)</summary>

| Bölüm | Taslak | Aile | M13 (2. yarı) | M14 süpürme / yön / düz şerit | İmza |
|---|---|---|---|---|---|
| Z36 | T42e | Z12 dikey ayna | 15 (12) | 2 / 2 / yok | U2>U D2>R R2>U R2>R |
| Z37 | T47e | Z23 180° | 13 (9) | 3 / 1 / yok | R2>R R3>D L2>L U2>U |
| Z38 | T43e | Z12 yatay ayna | 15 (12) | 2 / 2 / yok | D2>R U2>D U2>L L2>D |
| Z39 | T45e | Z23 dikey ayna | 8 (4) | 3 / 1 / yok | L2>L L3>D R2>U R2>L |
| Z40 | T44e | Z12 180° | 15 (12) | 2 / 1 / yok | U2>R D2>L L2>L L2>U |

Ayrıntılar: `Z31_Z35_tasarim.md`'deki yöntem, tur 5 işlemleri `Logs/zdesign/tur5.json`, raporlar `Logs/zdesign/hizli5/`.

</details>
