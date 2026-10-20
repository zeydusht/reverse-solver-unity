# 40 bölüm zorluk eğrisi önerisi (ZeydBot, gece 2026-10-20) — fabrikaya bağlanmadı

Kaynak: Zeyd'in 18 puanı (`denemeler.csv`), bölüm özellikleri (`bolum_ozellik.csv`), `zorluk_analizi.md`. Bu bir öneri; PM onayı ve oturum 3 sonucu gelmeden fabrikaya girmez.

## Ayar → beklenen puan (Zeyd'in verisinden)

| Ayar | Veride örnek | Zeyd'in puanı | Güven |
|---|---|---|---|
| Bomba yok (kolay) | Z03, Z05, Z06, Z09, Z11, Z14, Z20 | 1–2 | yüksek (7/7) |
| 1 bomba, pay +2, tuzak yok (orta) | Z04, Z10, Z15, Z19 | 2–3 | yüksek (4/4) |
| 1 bomba, pay +2, ama bir kez yandı | Z17 | 4 | düşük (n = 1) |
| 1 bomba, pay +1 (orta) | Z16 | 6 | düşük (n = 1) |
| 4 bomba, pay 0–1, çok tuzak | Z07, Z13 | 8–9 | orta |
| 4 bomba, pay 0, en derin kazı 51, 4 çivi | Z18 | 9 | düşük |
| **Z12 tipi:** 4 bomba, pay 1, ~20 "tek doğru" hamle, tuzak 0, dar süre | Z12 | 9 + "çok keyifli" | düşük (n = 1) |

**Orta boşluk (4–7) için öneri:** "Payı 2 olan orta" ile "zor" arasında veri yok. Bu boşluğu doldurmanın en ucuz yolu **pay +1'li orta** (Z16 gibi). Önerilen merdiven:
- puan 4 ≈ 1 bomba, pay +1, tuzaklı hamle 0–2
- puan 5–6 ≈ 2 bomba, pay +1/+2, tek doğru hamle 8–12
- puan 7 ≈ 3 bomba, pay +1/+1/+2, tuzaklı hamle ≤ 5

Pay +3 Zeyd'i hiç zorlamaz (ortaların hepsi pay 2'de 2–3 puan aldı), o yüzden yalnızca giriş bölümlerinde kullanılır. Bu merdivenin 4–7 kısmı veriyle değil, **iki uçtan aradeğerle** önerildi; Z31–Z38 gibi bir set ile ölçülmeli.

## 40 bölüm eğrisi (hedef puan)

Kurallar: yeni engel tanıtımı (3, 7, 12, 20) kolay olur (puan 2–3). Diğerlerinde her 4–5 bölümde bir yükseliş gelir. Dört "Z12 tipi" zirve var (10, 19, 29, 38). Her zirveden sonra bir nefes bölümü gelir.

| Bölüm | Hedef | Ne | | Bölüm | Hedef | Ne |
|---|---|---|---|---|---|---|
| 1 | 1 | bombasız | | 21 | 4 | 1 bomba pay +1 |
| 2 | 1 | bombasız | | 22 | 5 | 2 bomba |
| 3 | 2 | **yeni engel** tanıtım, pay +3 | | 23 | 5 | 2 bomba, tek doğru ~10 |
| 4 | 2 | bombasız | | 24 | 6 | 2 bomba pay +1/+1 |
| 5 | 3 | 1 bomba pay +2 | | 25 | 7 | 3 bomba |
| 6 | 3 | 1 bomba pay +2 | | 26 | 3 | nefes |
| 7 | 2 | **yeni engel** tanıtım | | 27 | 5 | 2 bomba |
| 8 | 3 | 1 bomba pay +2 | | 28 | 7 | 3 bomba |
| 9 | 4 | 1 bomba pay +1 | | 29 | **9** | **Z12 tipi** |
| 10 | **7** | **Z12 tipi (hafif):** 2 bomba, pay 1, tuzak 0 | | 30 | 4 | nefes |
| 11 | 3 | nefes | | 31 | 6 | 2–3 bomba |
| 12 | 2 | **yeni engel** tanıtım | | 32 | 7 | 3 bomba |
| 13 | 4 | 1 bomba pay +1 | | 33 | 5 | nefes |
| 14 | 4 | 1 bomba pay +1 | | 34 | 7 | 3 bomba |
| 15 | 5 | 2 bomba pay +2 | | 35 | 8 | 4 bomba pay 1 |
| 16 | 3 | nefes | | 36 | 6 | nefes |
| 17 | 5 | 2 bomba | | 37 | 8 | 4 bomba |
| 18 | 6 | 2 bomba pay +1/+2 | | 38 | **9** | **Z12 tipi** |
| 19 | **8** | **Z12 tipi** | | 39 | 6 | nefes |
| 20 | 2 | **yeni engel** tanıtım | | 40 | **9** | final (Z12 tipi ya da Z18 tipi) |

Gerekçeler:
- Zeyd'in puanları iki tepeli (1–3 ve 7–9). Eğri bu yüzden ortayı bilinçli olarak dolduruyor.
- Z12 tipi zirvelerde **tuzak az, tek doğru yol çok, süre dar**. Z13 gibi tuzak dolu zor bölümler yalnızca seyrek kullanılır (35, 37); ceza hissi keyif hipotezine ters düşüyor.
- Puan 8–9 hedefleri yalnızca 19. bölümden sonra geliyor. 40/40 README garantisi (SmartPlayer çözebilir) her bölümde aynen geçerli kalır.
