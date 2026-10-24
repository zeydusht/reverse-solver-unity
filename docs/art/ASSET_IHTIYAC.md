# Eksik asset listesi (v1 — PM taslağı, 24 Ekim 2026)

Bu dosyayı iki ekip birlikte kullanır:

- **Codex (Art Director):** Listedeki işleri öncelik sırasıyla üretir. Biten satırın "Durum" sütununu günceller ve dosyayı `HANDOFF.md`'ye ekler.
- **Claude Code:** Listeyi koda göre doğrular. Gerçek boyutları, eksik ekranları ve kodda kullanılmayan satırları düzeltir. Bu iş için yalnız "Ölçü / not" sütununu ve en alttaki "Claude Code notları" bölümünü düzenler.

Durum etiketleri: ✅ hazır/onaylı · 🎨 Codex üretiyor · 🔜 sırada · ❓ Claude Code doğrulayacak

## P0: Playtest build'i için (önce bunlar)

| # | Asset | Nerede | Ölçü / not | Durum |
|---|---|---|---|---|
| 1 | Uygulama ikonu | PWA, favicon, mağaza | 1024 / 512 / 256 / 192 / 180 / 32 px. **Kod:** 180 (apple-touch), 192 + 512 (manifest), 512 maskable (kenar boşluğu gerekir), 32 (favicon) bağlandı; 1024/256 yalnızca mağaza | ✅ altı boyut hazır; dosyalar Icon/Exports içinde |
| 2 | Menü koridor arka planı | Ana menü | v04 hazır. **Kod:** 1024×1536 (2:3), ekranı kaplayacak şekilde ölçeklenir (iPhone 375×667 pt ~%13 yanlardan kırpılır). Başlık + düğmeler ekran ortasında, yarı saydam koyu panel üstünde; güvenli alan korunur | ✅ v04 kaynak + iki UI güvenli alanı yerleşim seçeneği: `Menu/safe-area-options-v01.html` |
| 3 | "Oyna" / "Level X" düğmesi | Ana menü | **Kod:** düğme 296×46 pt (dar ekranda ekran−80), @3x → 888×138 px, 9-slice köşe ≥ 14 pt (42 px) | 🎨 İki renk seçeneği 888×138; 9-slice metadata hazır, Zeyd seçimi bekleniyor (`Menu/Buttons/review-options-v01.html`) |
| 4 | Tutorial eli | Bölüm 1–2 ve tanıtımlar | **Kod:** şu an vektör ok + parlama (TutorialHand.cs), hücre boyutunda (~40–60 pt). PNG gelirse ~96 pt → 288 px @3x | 🎨 A/B iki şeffaf poz seti hazır; stil seçimi bekliyor (`TutorialHand/Options/review-options-v01.html`) |
| 5 | Engel tanıtım pencere görselleri: çivi, bomba, zincir, duvar | Bölüm 3, 6, 10, 14 | **Kod:** kart 340 pt genişlik, iç 296 pt; ikon karesi 52 pt (156 px @3x). Sahne görseli eklenirse 296×198 pt → 888×592 px | 🎨 Baron ve dört engel için A/B sahne adayları 888×592; inceleme: `ObstacleIntros/review-options-v01.html` |
| 6 | "Zor bölüm" uyarı ekranı | Bölüm 19, 25, 30, 35–40 | **Kod:** şimdilik tanıtım kartıyla aynı kart (Screens.HardWarning, 52 pt ikon + başlık + metin + "Başla"). Görsel 888×592 px @3x | 🎨 Baron uyarısı/Jiggle endişesi A/B hazır (`HardWarning/review-options-v01.html`); seçim bekliyor |
| 7 | Pencere ve kart çerçevesi (UI) | Tanıtım ve uyarı pencereleri | **Kod:** kart vektörle çiziliyor (Card.cs, 340 pt, köşe 18 pt); 9-slice PNG köşe ≥ 54 px @3x | 🎨 İki 888×592 şeffaf merkezli A/B çerçeve hazır; sınır 54 px, seçim bekliyor (`UI/PopupFrame/review-options-v01.html`) |
| 8 | Bölüm sonu kutlaması | Kazanma ekranı | **Kod:** sonuç kartı Screens.Result (aynı 340 pt kart); poz başına ~120 pt → 360 px @3x | 🎨 Tek Jiggle ve üç Jiggle şeffaf A/B pozu 768 px hazır (`Victory/review-options-v01.html`); seçim bekliyor |
| 9 | Kaybetme ekranı | Bomba patlaması / süre bitti | **Kod:** aynı sonuç kartı; kayıp sebebi kodda ayrı (bomba / süre / kilit) → 3 görsel gerekebilir, ~120 pt | 🎨 Bomba/süre/hamlesiz için A/B altı kare görseli 512 px hazır (`Defeat/review-options-v01.html`); seçim bekliyor |

## P1: Oyun içi (playtest'ten sonra; okunabilirlik testi şart)

| # | Asset | Nerede | Ölçü / not | Durum |
|---|---|---|---|---|
| 10 | Engel oyun içi grafikleri: çivi, bomba, zincir, duvar | Tahta | Hücre boyutunda okunmalı (64–128 px). Sayaç rakamı dinamik kalır | ✅ Dört şeffaf 128 px export hazır (`Gameplay/ObstacleIcons/index.html`); sayaç dinamik kalır |
| 11 | Booster ikonları: makas, değnek, çekiç, saat | Alt bar | 128 px'te okunmalı | ✅ Dört şeffaf 128 px export hazır (`Gameplay/BoosterIcons/index.html`) |
| 12 | Tablo resimleri | Her bölümün yapbozu | Önce yön kararı (Baron'un sevdikleri mi, dış dünya mı?). Parça kenarı belirgin, sakin, orta kontrast | 🎨 İki 1080×728 yön örneği hazır (`Paintings/review-directions-v01.html`); seri üretimi seçim bekliyor |
| 13 | Tahta altlığı / çerçeve | Oyun ekranı | Tablo çerçevesi gibi; hard bölümlerde altın | 🎨 Şeffaf merkezli 888 px A/B: ceviz-teal ve altın (`Gameplay/BoardBase/review-options-v01.html`); seçim bekliyor |
| 14 | "Weee" sesi + parça çıkış, kazanma, kaybetme sesleri | Ses | Kısa, bize ait. Minion sesi yok | 🎨 Dört efekt için A/B özgün kısa WAV tınıları hazır (`Audio/review-options-v01.html`); dinleme/seçim bekliyor |

## P2: Hikâye ve meta

| # | Asset | Nerede | Ölçü / not | Durum |
|---|---|---|---|---|
| 15 | Koridor katmanları: duvar, çerçeve, tablo içi, karakter | Zoom ve geri dönüş geçişi | `LEVEL_GECISI.md`'ye göre | 🔜 |
| 16 | Jiggle ve Baron koşu döngüleri | Geçiş animasyonu | 2 kare hazır; 4–6 kareye çıkarılacak | 🎨 |
| 17 | Profil ekranı + "Kurtarılan Jiggles" sayacı | Profil | — | 🔜 |
| 18 | 100 / 500 / 1000 hediye sahnesi | Kilometre taşı | Jiggles birleşip hediye verir | 🔜 |
| 19 | Mağaza ekran görüntüleri + kısa tanıtım videosu | App Store | Yayından önce | 🔜 |

## Claude Code notları

- Birim: oyun 1 birim = 1 CSS pt; iPhone @3x. Tüm görseller pt × 3 px üretilmeli.
- Eksik ekranlar (listede yok): **Bölümler listesi** (LevelList), **duraklatma/çıkış onayı**, **booster onay kartı** (Screens.Ask), **booster tanıtım kartı** (bintro, 5/9/15/23), **isim sorma ekranı** (oyuncu adı), **yükleme ekranı** (WebGL şablonu, ikon 192 kullanılıyor).
- Kayıp türü 3: bomba, süre, kilitlenme (hamle kalmadı). #9 iki değil üç görsel olabilir.
- Kullanılmayan: #13'teki "hard bölümlerde altın" için kodda bayrak artık var (`hard`); #6 ile aynı kaynak kullanılabilir.
