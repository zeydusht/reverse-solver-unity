# 40 web seviyesi — zorluk modeliyle değerlendirme (13 Ekim 2026)

**Ne:** Canlı 40 seviyenin hepsi Zorluk panelinin modelinden geçirildi (`DifficultyModel`, PRODUCT.md "Zorluk modeli (başlangıç değerleri)"). `levels.json` değişmedi. Bu tablo Zeyd'in yeniden tasarımı için girdi, karar değil.

**Tür eşlemesi.** Seviyelerin türü Zeyd'in etiketi değil, PM'in açık kuralıyla verildi:
- L01, L05, L11, L21 ve L31 **Tanıtım**: yeni engelin öğretildiği seviyeler. L01 oyunun ilk bölümü olduğu için s = 1,35.
- Diğerleri hamle başına okumaya göre:

  | Okuma | Tür |
  |---|---|
  | < 2,7 | Kolay |
  | 2,7–3,3 | Orta |
  | 3,3–4,0 | Zor |
  | 4,0 ve üstü | Çok zor |

Rastgele oyuncu sayıları 200 oyunluk Monte Carlo'dan geliyor. Web sütunu `web_verisi_2026-10-13.md`'den (bugünkü süreyle oynanmış satırlar).

## Özet

- **Önerilen süre mevcut süreden en az %20 sapan 19 seviye var.**
  - İlk 10 seviyenin süresi formülün iki katına yakın: L02–L10'da öneri 45–65 sn, mevcut 70–115 sn. Web oyuncuları bu seviyelerde sürenin %62–76'sını kullanmadan bitiriyor. Model ile veri aynı şeyi söylüyor: süre baskısı yok.
  - L11 ve sonrasında öneri genelde mevcut süreden uzun, çünkü web süreleri 40–60 sn'de sıkışık. Öneriler 75 sn sınırının altında kalıyor.
- **Kazanma oranı türle aynı yönde** (yalnızca yön göstergesi):

  | Tür | Seviye | Web satırı | Web kazanma |
  |---|---|---|---|
  | Tanıtım | 5 | 25 | %84 |
  | Kolay | 3 | 14 | %100 |
  | Orta | 13 | 44 | %91 |
  | Zor | 14 | 36 | %72 |
  | Çok zor | 5 | 14 | %50 |

- **En sık bant dışı ölçüler:** "ilk hamlede serbest (k₀)", "açık dikiş %" ve "süre payı". Web seviyeleri modelin bantlarına göre yapılmadığı için bu beklenen bir sonuç. Bant dışı sayısı bir yeniden tasarım listesi değil, tablodaki son sütun.
- **Bomba:** L21 bir tanıtım seviyesi ve bomba içeriyor; model tanıtım türünde bombaya izin vermediği için "Bomba payı" orada bant dışı. Bomba tanıtımı için bu bilerek yapılmış bir seçim. Model bunu ayırt etmiyor (bulgu, aşağıda).

## Önerilen süreyle mevcut süre arasında %20 ya da daha fazla fark olanlar

- **L02** (Orta): mevcut 85 sn → öneri 45 sn (-47 %)
- **L03** (Kolay): mevcut 70 sn → öneri 50 sn (-29 %)
- **L04** (Orta): mevcut 80 sn → öneri 45 sn (-44 %)
- **L05** (Tanıtım): mevcut 95 sn → öneri 70 sn (-26 %)
- **L06** (Kolay): mevcut 90 sn → öneri 65 sn (-28 %)
- **L07** (Orta): mevcut 100 sn → öneri 55 sn (-45 %)
- **L08** (Orta): mevcut 115 sn → öneri 60 sn (-48 %)
- **L09** (Orta): mevcut 115 sn → öneri 65 sn (-43 %)
- **L10** (Orta): mevcut 115 sn → öneri 65 sn (-43 %)
- **L11** (Tanıtım): mevcut 45 sn → öneri 70 sn (+56 %)
- **L12** (Kolay): mevcut 40 sn → öneri 60 sn (+50 %)
- **L13** (Orta): mevcut 50 sn → öneri 60 sn (+20 %)
- **L19** (Orta): mevcut 50 sn → öneri 65 sn (+30 %)
- **L20** (Orta): mevcut 55 sn → öneri 75 sn (+36 %)
- **L21** (Tanıtım): mevcut 45 sn → öneri 75 sn (+67 %)
- **L23** (Orta): mevcut 50 sn → öneri 70 sn (+40 %)
- **L26** (Orta): mevcut 50 sn → öneri 70 sn (+40 %)
- **L31** (Tanıtım): mevcut 45 sn → öneri 75 sn (+67 %)
- **L37** (Orta): mevcut 55 sn → öneri 70 sn (+27 %)

## Tablo

Sütunlar:
- **engel:** zincir / çivi / bomba sayısı.
- **okuma:** hamle başına okuma.
- **darboğaz:** 10+ parça kalmışken tek seçenek kalan hamleler.
- **bomba payı:** fitil − en kısa kazma yolu; "≥" yaklaşık demek.
- **web:** satır / kazanma / kazanan medyan sn / s_veri.
- **bant dışı:** türün bandı dışında kalan ölçü sayısı.

| Seviye | Tahta | Gövde | Engel | Okuma | Darboğaz | Bomba payı | Tür | Süre | Öneri | s | Web | Bant dışı |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| L01 | 4×5 | 20 | 0/0/0 | 2,43 | 0 | — | Tanıtım | 60 | **60** | 1,35 | 10 / %90 / 27 / 1,35 | 2 |
| L02 | 5×5 | 25 | 0/0/0 | 2,92 | 0 | — | Orta | 85 | **45** | 1,07 | 6 / %100 / 25,5 / 1,02 | 4 |
| L03 | 5×5 | 25 | 0/0/0 | 2,30 | 0 | — | Kolay | 70 | **50** | 0,97 | 6 / %100 / 24,5 / 0,98 | 1 |
| L04 | 5×5 | 25 | 0/0/0 | 2,86 | 0 | — | Orta | 80 | **45** | 1,06 | 6 / %100 / 23,5 / 0,94 | 3 |
| L05 | 5×6 | 30 | 0/2/0 | 2,66 | 0 | — | Tanıtım | 95 | **70** | 1,03 | 5 / %100 / 27 / 0,90 | 1 |
| L06 | 5×6 | 30 | 0/2/0 | 2,54 | 0 | — | Kolay | 90 | **65** | 1,01 | 5 / %100 / 30 / 1,00 | 1 |
| L07 | 5×6 | 30 | 0/3/0 | 3,04 | 0 | — | Orta | 100 | **55** | 1,09 | 5 / %100 / 27 / 0,90 | 2 |
| L08 | 6×6 | 36 | 0/3/0 | 2,75 | 0 | — | Orta | 115 | **60** | 1,04 | 4 / %100 / 36,5 / 1,01 | 4 |
| L09 | 6×6 | 36 | 0/3/0 | 2,83 | 0 | — | Orta | 115 | **65** | 1,05 | 3 / %100 / 44 / 1,22 | 3 |
| L10 | 6×6 | 36 | 0/4/0 | 2,80 | 0 | — | Orta | 115 | **65** | 1,05 | 3 / %100 / 28 / 0,78 | 2 |
| L11 | 5×6 | 29 | 1/0/0 | 2,85 | 0 | — | Tanıtım | 45 | **70** | 1,06 | 4 / %75 / 36 / 1,24 | 3 |
| L12 | 5×6 | 29 | 1/0/0 | 2,65 | 0 | — | Kolay | 40 | **60** | 1,02 | 3 / %100 / 29 / 1,00 | 4 |
| L13 | 6×6 | 35 | 1/1/0 | 2,78 | 0 | — | Orta | 50 | **60** | 1,04 | 3 / %100 / 35 / 1,00 | 3 |
| L14 | 6×6 | 34 | 2/1/0 | 3,30 | 0 | — | Zor | 50 | **50** | 1,13 | 3 / %100 / 32 / 0,94 | 3 |
| L15 | 6×6 | 34 | 2/2/0 | 3,70 | 0 | — | Zor | 55 | **55** | 1,19 | 5 / %60 / 43 / 1,26 | 2 |
| L16 | 6×6 | 34 | 2/2/0 | 3,93 | 0 | — | Zor | 55 | **55** | 1,23 | 4 / %100 / 41,5 / 1,22 | 2 |
| L17 | 6×6 | 33 | 3/2/0 | 4,20 | 6 | — | Çok zor | 55 | **50** | 1,27 | 3 / %100 / 43 / 1,30 | 4 |
| L18 | 5×6 | 29 | 1/1/0 | 2,92 | 0 | — | Orta | 45 | **50** | 1,07 | 2 / %100 / 29 / 1,00 | 4 |
| L19 | 6×6 | 34 | 2/2/0 | 3,15 | 0 | — | Orta | 50 | **65** | 1,10 | 2 / %100 / 33,5 / 0,99 | 4 |
| L20 | 6×7 | 40 | 2/2/0 | 3,23 | 0 | — | Orta | 55 | **75** | 1,12 | 4 / %50 / 49 / 1,23 | 4 |
| L21 | 5×6 | 30 | 0/0/1 | 2,89 | 0 | 14 | Tanıtım | 45 | **75** (75 sınırı) | 1,16 | 4 / %50 / 31,5 / 1,05 | 6 |
| L22 | 6×6 | 36 | 0/0/1 | 3,66 | 0 | 8 | Zor | 55 | **65** | 1,28 | 3 / %67 / 33,5 / 0,93 | 5 |
| L23 | 6×6 | 35 | 1/1/1 | 3,03 | 0 | 12 | Orta | 50 | **70** | 1,19 | 2 / %100 / 32,5 / 0,93 | 3 |
| L24 | 6×6 | 34 | 2/2/1 | 3,71 | 2 | 10 | Zor | 55 | **60** | 1,29 | 5 / %20 / 33 / 0,97 | 3 |
| L25 | 5×6 | 29 | 1/1/1 | 3,58 | 1 | 10 | Zor | 45 | **50** | 1,27 | 1 / %100 / 32 / 1,10 | 5 |
| L26 | 6×6 | 35 | 1/1/1 | 3,17 | 0 | >=1 | Orta | 50 | **70** | 1,21 | 2 / %50 / 36 / 1,03 | 5 |
| L27 | 6×6 | 34 | 2/2/1 | 3,37 | 0 | 6 | Zor | 50 | **55** | 1,24 | 2 / %50 / 46 / 1,35 | 4 |
| L28 | 6×6 | 34 | 2/2/2 | 4,00 | 3 | 8 | Zor | 55 | **60** | 1,34 | 2 / %50 / 45 / 1,32 | 5 |
| L29 | 6×7 | 39 | 3/2/1 | 3,60 | 0 | 10 | Zor | 60 | **65** | 1,28 | 2 / %50 / 42 / 1,08 | 6 |
| L30 | 6×6 | 35 | 1/1/1 | 3,42 | 0 | >=5 | Zor | 55 | **60** | 1,25 | 1 / %100 / 33 / 0,94 | 2 |
| L31 | 5×6 | 30 | 0/0/0 | 2,91 | 0 | — | Tanıtım | 45 | **75** | 1,06 | 2 / %100 / 21 / 0,70 | 3 |
| L32 | 6×6 | 36 | 0/1/0 | 3,74 | 0 | — | Zor | 55 | **60** | 1,20 | 2 / %100 / 26 / 0,72 | 2 |
| L33 | 6×6 | 35 | 1/1/0 | 3,34 | 0 | — | Zor | 50 | **55** | 1,13 | 3 / %100 / 28 / 0,80 | 2 |
| L34 | 6×6 | 35 | 1/1/1 | 4,01 | 2 | >=1 | Çok zor | 55 | **55** | 1,34 | 4 / %25 / 38 / 1,09 | 2 |
| L35 | 6×6 | 34 | 2/2/1 | 4,74 | 1 | 10 | Çok zor | 60 | **60** | 1,46 | 2 / %50 / 54 / 1,59 | 5 |
| L36 | 5×6 | 29 | 1/1/0 | 3,64 | 2 | — | Zor | 50 | **45** | 1,18 | 2 / %100 / 32 / 1,10 | 5 |
| L37 | 6×6 | 35 | 1/1/1 | 3,27 | 0 | 13 | Orta | 55 | **70** | 1,22 | 2 / %50 / 24 / 0,69 | 2 |
| L38 | 6×6 | 35 | 1/2/1 | 3,70 | 0 | 11 | Zor | 60 | **60** | 1,29 | 1 / %100 / 32 / 0,91 | 4 |
| L39 | 6×7 | 40 | 2/2/1 | 4,51 | 1 | 12 | Çok zor | 70 | **70** | 1,42 | 4 / %25 / 63 / 1,57 | 6 |
| L40 | 6×7 | 39 | 3/2/2 | 4,08 | 1 | 11 | Çok zor | 65 | **65** | 1,35 | 1 / %100 / 64 / 1,64 | 6 |

## Modelle ilgili bulgular (PM'e)

1. **Tanıtım türü, öğretilen engeli tanımıyor.** Tanıtım türü bombaya izin vermiyor, oysa L21 bombanın tanıtıldığı seviye. "Tanıtım" ya engelin türünü bilmeli ya da tanıtılan engel bant dışı sayılmamalı.
2. **Tanıtımın süre önerisi web oyuncu verisinden uzun.** P = 2,2 L11/L21/L31'de 70–75 sn öneriyor. Web'de 45 sn ile kazananlar sürenin %20–53'ünü bırakıyor.
3. **İlk 10 seviyenin süresi formülün iki katı.** Yeniden tasarımda bu, süreleri kısaltma ya da tahtayı büyütme kararıdır. Web verisi süre baskısı olmadığını doğruluyor (kazananlar %62–76 süre bırakıyor).
