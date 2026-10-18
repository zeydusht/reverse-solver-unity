# D07 — TESLİM DEĞİL, M3 ve M4 kaldı (19 Ekim 2026)

**Link:** [D07 oyna](https://zeydusht.github.io/reverse-solver-unity/br/?debug=1&set=designs&lv=D07) — `https://zeydusht.github.io/reverse-solver-unity/br/?debug=1&set=designs&lv=D07`

Build 4e yayında (gh-pages `bfa7f4e`; bayraklar kapalı; **süre artık ilk hamleyle başlıyor**). Doğrulama: yerel 6/6, canlı 6/6; testler 536/537 (1 atlandı).

PM kararı (2026-10-19, B): 2 saatlik güçlü yöntem denemesinde (çözüm önce + kilitleri çözümden türetme, soyma yöntemi) M3/M4'ü tutan aday çıkmadı; en iyi aday bu etiketle yayınlandı. Zeyd oynayıp M3/M4'ün gerçekten gerekli olup olmadığına karar verir.

**D07 özet:** 7×8 (6×7'de M9 imkânsız: iç bölgede 6 hücre, 3 bomba + 2 zincir sığmıyor), 54 parça · açılışta serbest **2** · oyun boyunca en çok serbest **3** · bir hamlede açılan en çok 2 · **3 bomba**, fitil payları +1/+1/+1, en derin kazma yolu 30 · **2 çivi** (bombanın yolunu çivili 28 / çivisiz 22 hamleye uzatıyor) · **2 uzak zincir** (yön çatışmalı) · duvarlı şerit **13/30** · süre 145 sn (pay 1,16; 75 sn tavanı 7×8'de 1,15 ile tutmuyor, atlandı).

**Kalan:** M3 ≥ 3 komşuya bağlı %15 (hedef çoğu) · M3 aynı anda tek bağlı 7 (≤ 2) · M4 aynı şeritten art arda 4 (≤ 2). Neden: daha çok kilit ya da şerit kısıtı tahtayı supapsız çözülmez yapıyor (adillik).

| Metrik | L40 | D04 | D05 | D06 | D07 |
|---|---|---|---|---|---|
| M1 Açılış hamlesi (k₀) | 7 ✗ | 9 ✗ | 11 ✗ | 11 ✗ | 2 ✓ |
| M2 Serbest tavanı (ilk %70) | 8 ✗ | 10 ✗ | 11 ✗ | 11 ✗ | 3 ✓ |
| M3 ≥ 3 komşuya bağlı | %0 ✗ | %30 ✗ | %25 ✗ | %25 ✗ | %15 ✗ |
| M3 Tek bağlı (aynı anda en çok) | 14 ✗ | 6 ✗ | 4 ✗ | 3 ✗ | 7 ✗ |
| M3 Kenarı düz kare | 6 ✗ | 0 ✓ | 1 ✗ | 0 ✓ | 0 ✓ |
| M4 Aynı şeritten art arda | 3 ✗ | 3 ✗ | 2 ✓ | 2 ✓ | 4 ✗ |
| M5 Bomba kenara uzaklık | 0 ✗ | 2 ✓ | 0 ✗ | 2 ✓ | 2 ✓ |
| M5 Kazma yolu / fitil payı | 4 / +11 ✗ | 10 / +3 ✗ | 14 / +2 ✗ | 15 / +1 ✓ | 30 / +1 ✓ |
| M6 Çivi bomba yolunda | yolda 2, bombanın yolu çivili 4 / çivisiz 4 ✗ | yolda 1, bombanın yolu çivili 10 / çivisiz 10 ✗ | yolda 2, bombanın yolu çivili 14 / çivisiz 2 ✓ | yolda 1, bombanın yolu çivili 15 / çivisiz 9 ✓ | yolda 2, bombanın yolu çivili 28 / çivisiz 22 ✓ |
| M6 Çivi boş hamle payı | 1 ✗ | 1 ✗ | 0 ✓ | 0 ✓ | 0 ✓ |
| M7 Zincir yön çatışması | var ✓ | yok, uçlar yan yana / aynı şeritte ✗ | yok ✗ | yok ✗ | var ✓ |
| M8 Kolay yönü duvarlı parça / şerit | 21 / 12 ✓ | 3 / 2 ✗ | 4 / 2 ✗ | 7 / 5 ✓ | 25 / 13 ✓ |
| M9 Kenara uzaklık bomba / çivi / zincir | 0 / 0 / 0 ✗ | 2 / 0 / 0 ✗ | 0 / 0 / 0 ✗ | 2 / 1 / 0 ✗ | 2 / 2 / 2 ✓ |
| yol | SmartPlayer, 39 hamle, 6×7, 39 parça, süre 65 sn | kayıtlı çözüm, 40 hamle, 6×7, 40 parça, süre 65 sn | kayıtlı çözüm, 40 hamle, 6×7, 40 parça, süre 65 sn | kayıtlı çözüm, 40 hamle, 6×7, 40 parça, süre 65 sn | kayıtlı çözüm, 54 hamle, 7×8, 54 parça, süre 145 sn |
| Monte Carlo | 1000 rastgele oyun: kazanma %15,7, bomba kaybı %84,3, donma 0, supaplı oyun 0 | 1000 rastgele oyun: kazanma %0,0, bomba kaybı %100,0, donma 0, supaplı oyun 0 | 1000 rastgele oyun: kazanma %8,9, bomba kaybı %91,1, donma 0, supaplı oyun 0 | 1000 rastgele oyun: kazanma %0,2, bomba kaybı %99,8, donma 0, supaplı oyun 0 | 1000 rastgele oyun: kazanma %3,3, bomba kaybı %96,7, donma 0, supaplı oyun 0 |
| En kısa çözüm | 39 hamle (SmartPlayer: Won, supap 0); tasarım kontrolü: Solvable | 40 hamle (SmartPlayer: Won, supap 0); tasarım kontrolü: Solvable | 40 hamle (SmartPlayer: Won, supap 0); tasarım kontrolü: Solvable | 40 hamle (SmartPlayer: Won, supap 0); tasarım kontrolü: Solvable | 54 hamle (SmartPlayer: Won, supap 0); tasarım kontrolü: Solvable |
| M10 Süre payı | 1,23 (hedef 1,15; süre ilk hamleyle: build'de yok) | 1,24 (hedef 1,15; süre ilk hamleyle: build'de yok) | 1,21 (hedef 1,15; süre ilk hamleyle: build'de yok) | 1,21 (hedef 1,15; süre ilk hamleyle: build'de yok) | 1,16 (hedef 1,15; süre ilk hamleyle: build'de yok) |

M10: süre payı yukarıda; "süre ilk hamleyle" Build 4e'de var (tablodaki "build'de yok" notu ölçüm anındaki eski etiket).

**Getir:** kazandın mı, kaç saniye, nerede takıldın; M3/M4 eksikliği oyunda hissedildi mi (kolay mı, haksız mı).

---

Eski raporlar (D06, D05 tabloları): `docs/arsiv/OVERNIGHT_ARSIV.md`.
