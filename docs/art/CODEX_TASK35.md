# CODEX_TASK35 — İkon boyutu, saat kadranı, yeni Parça/Süre başlık kutuları

Zeyd TASK34'ü **beğendi** ("çok iyi oldu"). Boyama doğru, ona dokunma. Üç değişiklik var.

## 1. İkonlar tuşun içinde büyüsün (ölçüme göre)
PM ölçümü (`docs/art/task31/ref_alt.png`, tuşun dış çapı ≈192 px): RM ikonları **genişlikte 124–144 px, yükseklikte 111–137 px**, yani dış çapın **%65–75'i**. Yeşil diskin ~%90'ını doldururlar, kenarda çok az boşluk kalır. Bazı uçlar (sap, fitil) altın halkaya hafifçe biner. Optik merkez tam ortada (±%3).
- Kendin de ölç (her RM ikonunun sınır kutusu ve 4 kenardaki boşluk). Sonuçları `docs/art/task35/olcum.md`'ye yaz.
- Bizim 4 ikonu da **aynı orana** getir: en uzun kenar dış çapın ~%72'si. **Optik olarak ortala** (görsel ağırlık merkezi tuş merkezinde).
- Önizlemede RM tuşu ile bizim tuşu aynı boyda ve yan yana göster, ikonun etrafına sınır kutusu çiz.

## 2. Saat
- Ziller **sarı** kalsın (şimdiki gibi).
- Kadranın dışındaki halka/kasa (eski saatte **yeşil** olan bölüm) **siyah** olsun: hacimli siyah, `#3A3A48` → `#1C1C26` → `#0C0C12`, üst solda parlama. Kendi rengiyle kenar, dış kontur yok.
- Bugünkü **sarı/altın gövde kalkıyor**.
- Kadranın içi krem, akrep ve yelkovan koyu. Ayaklar sarı.

## 3. Parça / Süre başlıkları: sıfırdan, kutu
Düz şerit de olmadı. Yeni fikir (Zeyd):
- Başlık, altındaki **krem gövde kutusuna benzeyen küçük bir kutucuk** olsun. Aynı malzeme: krem dolgu, mavi çerçeve, iç bevel. Gövdenin üstünde, ortalı ve gövdeden daha dar/kısa.
- "Parça" ve "Süre" yazısı bu kutunun içinde: koyu lacivert ya da mavi, kalın yuvarlak font, okunur.
- İki kutu (başlık + gövde) bir set gibi dursun: aralarında küçük boşluk ya da hafif üst üste binme. 2–3 varyant göster (ör. başlık gövdeye yapışık, ayrık, gövdeye yarı gömülü).
- Aynı başlık dili **"Süre doldu" panelinde** de kullanılsın.

## Teslim
`docs/art/task35/onizleme.png`: ikon boyutu RM ile karşılaştırma, yeni saat (önce/sonra), başlık kutusu 2–3 varyant oyun ekranında, Süre doldu paneli. Dosyalar `Assets/Art/_yeni/HUD35/`. 3 satır özet, dur.
