# Codex Task15 — Önizleme 12 geri bildirimi (Bitti)

Zeyd oyunu oynadı. Booster çubuğu, bomba, çivi ve durdur tuşu onaylandı; onlara dokunma.

Ekran görüntüleri: `docs/qa/feedback-2026-10-26/`.

Kurallar:
- Yalnız `Assets/Art/Gameplay/Task15/` ve `docs/art/` klasörlerine yaz. C# dosyalarına dokunma.
- Renkler `PALETTE.md`'den gelecek.
- Bittiğinde HANDOFF'a her dosya için satır ekle: dosya / oyunda nereye / boyut / 9-slice / pivot.

## İstenenler

### 1. Yeni kırmızı

Şu anki kırmızı yapay duruyor; booster adet rozeti ve duvar şeridinde bu yüzden beğenilmedi.

- Royal Match tarzı, sıcak ve canlı bir kırmızı skala kur: parlak / orta / gölge. Ton referansı: `Assets/Art/Reference/booster-bar-layout-reference.jpg` içindeki rozet kırmızısı. Yalnız tonu al, görseli kopyalama.
- Skalayı `PALETTE.md`'ye "Kırmızı" olarak ekle. Mercan yalnız urgent süre için kalsın.
- Booster adet rozetini (`booster-badge`) bu tonla yeniden çıkar.

### 2. Duvar v3 (kırmızı-beyaz engel)

Şu anki şerit tahtada fark edilmiyor. Oyuncu bakmadan "burası duvar" demeli.

- Parlak kırmızı ve beyaz, kalın ve seyrek çapraz çizgiler.
- Hafif hacim: üstte parlama, altta gölge.
- İnce koyu kontur.
- Dosyalar: `wall-repeat-v3` (yatay tile) ve `wall-corner-v3`.
- Tahtanın altın çerçevesi ve mavi parçalar üstünde net okunmalı.
- Önceki örneklerinden beğenilen bir şerit varsa onu temel al.

### 3. HUD hapları v2 (hamle ve süre)

Şu anki haplar çok yatay ve ince; ikonlar seçilmiyor.

- Dikeyde daha dolgun, yatayda biraz daha kısa yap.
- Yeni minimal ikonlar:
  - **Yapboz parçası:** tek parça, net silüet. Hamle hapı.
  - **Saat:** kronometre ya da kum saati, göz gibi görünmesin. Süre hapı.
- Sayı alanı boş kalsın.
- Durdur tuşu ve LV rozeti aynı kalsın.

### 4. Kart tuşları v2

Şu anki tuşlar çok yatay; ince ve uzunlar. "Başla" yazısı sarı zeminde beyaz kaldığı için okunmuyor.

- Daha kısa ve daha uzun boylu tuşlar çiz. Level tuşu (`menu-play-button-v2`) ailesinden olsun.
- Ana tuş:
  - Biri yeşil, Level tuşu gibi: "Başla" ve "Devam".
  - Biri turkuaz: "Yeniden başla" ve "Ana menü".
- Yazı okunurluğu: koyu zemin + krem yazı ya da açık zemin + lacivert yazı. Sarı üstüne beyaz olmasın.
- Her tuşun normal ve pressed hali olsun. 9-slice destekli olsun.

### 5. 5 Jiggle avatarı

- Renkler: mavi, pembe, kırmızı, yeşil, mor.
- Mevcut avatar stilinde çiz, 256×256, kare kutuya oturacak şekilde.
- Sağ üstteki sayaç için 64 px'te de okunan küçük bir Jiggle ikonu çıkar: `jiggle-counter-icon`.

### 6. Resimli yapboz testi

Parçaların düz mavi yerine bir tablo resmi göstermesini deniyoruz.

- `painting-test-01.png`: 1536×1536, kare.
- Bizim stilimizde parlak, okunur bir sahne çiz; menü koridorundaki tablolar gibi.
- Resim parçalara bölünecek. Bu yüzden:
  - Büyük, net şekiller kullan.
  - Her bölgede ayırt edilebilir renk ve detay olsun.
  - Tek renk büyük gökyüzü alanından kaçın.

## Teslim

- Önizleme: `Task15/preview.png`. Tahtada yeni duvar, yeni HUD hapları, pause kartı ve avatar seçim kartı görünsün.
- Bitince "Task15 bitti" yaz.
