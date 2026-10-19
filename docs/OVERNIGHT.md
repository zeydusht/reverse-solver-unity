# D08 — D07'den daha zor, 4 dağınık bomba (19 Ekim 2026)

**Link:** [D08 oyna](https://zeydusht.github.io/reverse-solver-unity/br/?debug=1&set=designs&lv=D08) — `https://zeydusht.github.io/reverse-solver-unity/br/?debug=1&set=designs&lv=D08`

Build 4f yayında (gh-pages `0bdecdf`, bayraklar kapalı, süre ilk hamleyle başlıyor). Doğrulama: yerel 6/6, canlı 6/6.

Zeyd'in isteği: D07 iyi ama daha zor, bombalar homojen. D08: **4 bomba** tahtaya yayılmış (en yakın iki bomba arası en büyük olacak şekilde seçildi), **4 çivi** bombaların yolunda, en derin kazma yolu **51** (D07: 30), rastgele oyuncu 1000 oyunda **hiç kazanamıyor** (D07: %3,3), süre 135 sn.

**Kaçırılanlar:** M1 açılış 3 (hedef 1–2) · M3 (%15; tek bağlı 7) · M4 art arda 6 · bir bombanın fitil payı **+0** (kayıtlı çözüm kazanıyor, ama hiç boş hamle yok — en sıkı bomba). Adillik: çözülebilir, supap 0.

| Metrik | D07 | D08 |
|---|---|---|
| M1 Açılış hamlesi (k₀) | 2 ✓ | 3 ✗ |
| M2 Serbest tavanı (ilk %70) | 3 ✓ | 3 ✓ |
| M3 ≥ 3 komşuya bağlı | %15 ✗ | %15 ✗ |
| M3 Tek bağlı (aynı anda en çok) | 7 ✗ | 7 ✗ |
| M3 Kenarı düz kare | 0 ✓ | 0 ✓ |
| M4 Aynı şeritten art arda | 4 ✗ | 6 ✗ |
| M5 Bomba kenara uzaklık | 2 ✓ | 2 ✓ |
| M5 Kazma yolu / fitil payı | 30 / +1 ✓ | 51 / +0 ✗ |
| M6 Çivi bomba yolunda | yolda 2, bombanın yolu çivili 28 / çivisiz 22 ✓ | yolda 4, bombanın yolu çivili 46 / çivisiz 45 ✓ |
| M6 Çivi boş hamle payı | 0 ✓ | 0 ✓ |
| M7 Zincir yön çatışması | var ✓ | var ✓ |
| M8 Kolay yönü duvarlı parça / şerit | 25 / 13 ✓ | 25 / 13 ✓ |
| M9 Kenara uzaklık bomba / çivi / zincir | 2 / 2 / 2 ✓ | 2 / 2 / 2 ✓ |
| yol | kayıtlı çözüm, 54 hamle, 7×8, 54 parça, süre 145 sn | kayıtlı çözüm, 54 hamle, 7×8, 54 parça, süre 135 sn |
| Monte Carlo | 1000 rastgele oyun: kazanma %3,3, bomba kaybı %96,7, donma 0, supaplı oyun 0 | 1000 rastgele oyun: kazanma %0,0, bomba kaybı %100,0, donma 0, supaplı oyun 0 |
| En kısa çözüm | 54 hamle (SmartPlayer: Won, supap 0); tasarım kontrolü: Solvable | 54 hamle (SmartPlayer: Won, supap 0); tasarım kontrolü: Solvable |
| M10 Süre payı | 1,16 (hedef 1,15; süre ilk hamleyle: build'de yok) | 1,16 (hedef 1,15; süre ilk hamleyle: build'de yok) |

**Getir:** kazandın mı, kaç saniye, kaçıncı denemede; D07'ye göre ne kadar zor (yüzde olarak).

---

Eski raporlar (D07 tablosu dahil): `docs/arsiv/OVERNIGHT_ARSIV.md`.
