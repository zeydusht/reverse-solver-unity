# PM_MESAJ_52 — Yeni UI ve engel görsellerini oyuna yerleştir (p21)

Kaynak: Codex TASK28 serisi, dosyalar `Assets/Art/_yeni/` (Boosters Z1, HUD, Obstacles). Önce/sonra paftaları `docs/art/task28/`. Zeyd onayladı. p20/kök/pt1'e dokunma, her şey **p21**. Eski görseller silinmez, bayrakla geri dönülebilsin (`&art=eski`).

## 1. Booster çubuğu + HUD
Yeni PNG'leri mevcut yerlerine bağla (dosya adı ve boyut birebir olmalı, değilse raporla). Durumlar: normal, basılı, kilitli, tükenmiş. Rozet ve rakam canlı metin.

## 2. Bomba
- Yeni siyah bomba gövdesi + fitil kıvılcımı **kare döngüsü** (6–8 kare, ~10–12 fps).
- **Kodla nabız:** sayaç 4 ve üstündeyken yavaş, hafif ölçek nefesi (~%3, 1,2 sn). **3-2-1**'de acil PNG'ye geç, nabız belirgin (~%8, her hamlede bir "pump") + kısa titreme. Mevcut titreşim haritası (Medium) korunur.
- Sayaç etiketi ayrı katman, okunurluk bozulmasın.

## 3. Çivi → ASMA KİLİT (yalnız görünüm, kural aynı)
- Oyun mantığı ve bölüm verisi aynen kalır (parça N hamle kilitli). Yalnız görsel ve metinler değişir.
- Kilit gövdesinde numara (canlı metin). 0'da: açılma kareleri (kulp fırlar) → düşüp kaybolma.
- Bütün metinler (TR/EN, tanıtım kartı, i18n): "çivi" → "kilit", "nail" → "lock". Kod içindeki isimleri değiştirme, yalnız görünen metin.

## 4. Zincir — B (sarkık), fizik eğrisiyle
- Parça ortalarında yuvarlak bağlantı noktası sprite'ı (renk + işaret: yıldız, damla, hilal, kalp, yaprak; 5 renk).
- Zincir = tek **halka sprite'ı**, iki bağlantı noktası arasında **katener (ya da yaklaşık parabol) eğrisi** boyunca dizilir. Yerçekimi her zaman ekranın aşağısına:
  - yatayda ortası sarkar,
  - çaprazda (ör. sol alttan sağ üste) eğri düz çizginin altında kalır, alttan yatık çıkıp üste doğru dikleşir,
  - dikeyde neredeyse düz.
- Sarkma: zincir boyunun ~%8–12'si, en fazla ~0,4 hücre. Başka parçaların üstüne binmesin (gerekirse sarkmayı azalt). Halkalar eğrinin teğetine göre döndürülsün, eşit aralıklı.
- Parça hareket edince/zincir gerilince kısa yaylanma: 0,3–0,5 sn sönümlü salınım.
- Kopma: Codex'in kopma kareleri.

## 5. Duvar
Değişmez.

## Teslim
p21 güncel. Link: bomba, kilit ve zincirin (yatay, çapraz, dikey, uzun) aynı anda görüldüğü bir **vitrin bölümü** (`&lv=SHOW_ART`). `docs/art/task28/ENTEGRASYON_RAPOR.md`: değişen dosyalar, eksikler, telefonda FPS. PROJE_DURUMU'na tek satır. EditMode testleri geçsin.
