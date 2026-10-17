# Fabrika ajan kararları

Seviye tasarımcısı (`seviye-tasarimci`) ve oyuncu psikoloğunun (`oyuncu-psikologu`) kararları burada tutulur: tarih, slot, karar ve gerekçe. Ajanlar yalnızca okur ve karar verir; dosyaları ana Claude yazar.

Ajan bir ürün kuralıyla çeliştiğinde PM karar verir; o karar da burada anılır. Sayılar modelin keyif tahminidir, gerçek oyuncunun yerini tutmaz.

## Tur 1 — 2026-10-14 gece, koşu `kolay-20261014` (Tanıtım / Nefes / Kolay, 20 slot × 3 finalist)

### Psikolog
- **Sonuç:** 57 kabul, 3 ret: C02-20e5c1ef (near-miss %5,2), C04-515e463f (%6,3), C21-a425a46f (%6,3).
- **PM kararı (2026-10-14 gece (2)):** Near-miss eleme değil, yalnızca sıralama. Bu üç aday finalist kaldı, psikolog notuyla sıranın sonuna kondu.
- **Tercihleri** (bağlayıcı değil, kuyrukta "psikolog tercihi" olarak görünür):
  - slot 1: C01-a52dd807
  - slot 11: C11-1a93134d
  - slot 20: C20-43bbb687
- **Ölçülmeyen noktalar:**
  - Tanıtım ve Nefes'te sıkıcılık ölçülmüyor; bu türler sıkıcı kuralından muaf.
  - Booster tanıtımlarında booster'ın gerekli ya da okunur olup olmadığı ölçülmüyor; sim booster kullanmıyor.
  - T/N/K türleri sim'de birbirinden ayrışmıyor. Bu, playtest analizi için backlog'a yazıldı.

### Tasarımcı
- 60 FİKİR cümlesi yazdı.
- **Bulguları ve sonuçları** (PM 2026-10-14 gece (2), `544d51f`):
  1. Slot 3 çivi kartı "9. bölümde çekiç" diyordu → "23. bölümde çekiç" oldu.
  2. Slot 20 bomba kartındaki çekiç ipucu kaldırıldı (çekiç 23'te açılıyor).
  3. Booster kartlarında Türkçe karakter yoktu (Degnek, parca …) → yeniden yazıldı. Değnek kartındaki bozuk "karar" kelimesi "karıştırır" oldu.
  4. Nefes'teki bombaların hepsi ilk hamlede çıkıyordu (kazma 1) → yeni kural: Nefes'te en kısa kazma ≥ 2.
  5. 7 finalistte çivi süstü: sayaç parçanın sırasından çok önce bitiyordu → "çivi önemli" kuralı. Slot 3–4'te koşul, diğer kolay slotlarda arama cezası, O/Z/ÇZ'de rapor sütunu.
  6. Slot 4'te çivi yoktu → slot 4'e tam 1 çivi kondu.
- Bu tur sonrası T/N/K yeniden koşuldu (`kolay2-20261014`). Finalistlerin bir kısmı değişti; tur 2 bütün seti yeniden değerlendiriyor.

## Tur 2 — 2026-10-14, koşular `kolay2-20261014` + `ozc2-20261014` (40 slot)

Bu bölüm ajanların dönüşüyle doldurulur.
