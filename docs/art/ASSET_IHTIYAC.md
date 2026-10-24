# Eksik asset listesi (v1 — PM taslağı, 24 Ekim 2026)

Bu dosyayı iki ekip birlikte kullanır:

- **Codex (Art Director):** Listedeki işleri öncelik sırasıyla üretir. Biten satırın "Durum" sütununu günceller ve dosyayı `HANDOFF.md`'ye ekler.
- **Claude Code:** Listeyi koda göre doğrular. Gerçek boyutları, eksik ekranları ve kodda kullanılmayan satırları düzeltir. Bu iş için yalnız "Ölçü / not" sütununu ve en alttaki "Claude Code notları" bölümünü düzenler.

Durum etiketleri: ✅ hazır/onaylı · 🎨 Codex üretiyor · 🔜 sırada · ❓ Claude Code doğrulayacak

## P0: Playtest build'i için (önce bunlar)

| # | Asset | Nerede | Ölçü / not | Durum |
|---|---|---|---|---|
| 1 | Uygulama ikonu | PWA, favicon, mağaza | 1024 / 512 / 256 / 192 / 180 / 32 px. **Kod:** 180 (apple-touch), 192 + 512 (manifest), 512 maskable (kenar boşluğu gerekir), 32 (favicon) bağlandı; 1024/256 yalnızca mağaza  | ✅ altı boyut hazır; dosyalar Icon/Exports içinde |
| 2 | Menü koridor arka planı | Ana menü | v04 hazır. **Kod:** 1024×1536 (2:3), ekranı kaplayacak şekilde ölçeklenir (iPhone 375×667 pt ~%13 yanlardan kırpılır). Başlık + düğmeler ekran ortasında, yarı saydam koyu panel üstünde; güvenli alan korunur  | ✅ v04 kaynak + iki UI güvenli alanı yerleşim seçeneği: `Menu/safe-area-options-v01.html` |
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
| 10 | Engel oyun içi grafikleri: çivi, bomba, zincir, duvar | Tahta | Hücre boyutunda okunmalı (64–128 px). Sayaç rakamı dinamik kalır  | ✅ Dört şeffaf 128 px export hazır (`Gameplay/ObstacleIcons/index.html`); sayaç dinamik kalır |
| 11 | Booster ikonları: makas, değnek, çekiç, saat | Alt bar | 128 px'te okunmalı  | ✅ Dört şeffaf 128 px export hazır (`Gameplay/BoosterIcons/index.html`) |
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

## P0+: Kodla çizilen her görsel (PM_MESAJ_8 B, envanter 25 Ekim)

| # | Öğe | Ekran | Kod dosyası | Ölçü (pt → @3x px) | 9-slice | Durum |
|---|---|---|---|---|---|---|
| 26 | Oyun ekranı arka planı | Oyun | `GameRoot.cs (kamera rengi #0e2a2e)` | 375×667 pt referans, tam ekran → 1125×2001 px; üst/alt güvenli alan dahil, kenarlar kırpılabilir | yok | 🎨 hazır; `UI/Task5/manifest.json` + HANDOFF (#36 renk/alpha, #49 mevcut skin) |
| 27 | Üst bar: level rozeti | Oyun | `Ui/Hud.cs (badge)` | 52×52 pt → 156 px, daire | yok | 🎨 hazır; `UI/Task5/manifest.json` + HANDOFF (#36 renk/alpha, #49 mevcut skin) |
| 28 | Üst bar: parça sayısı ve süre panelleri (pill) | Oyun | `Ui/Hud.cs (pill)` | yükseklik 46 pt, genişlik dinamik (~100–120 pt) → 138 px yükseklik; ikonlar 16 pt (48 px): parça, saat | evet (yatay, köşe 23 pt / 69 px) | 🎨 hazır; `UI/Task5/manifest.json` + HANDOFF (#36 renk/alpha, #49 mevcut skin) |
| 29 | Üst bar: yeniden başlat düğmesi | Oyun | `Ui/Hud.cs (reset)` | 46×46 pt → 138 px, daire + ok ikonu | yok | 🎨 hazır; `UI/Task5/manifest.json` + HANDOFF (#36 renk/alpha, #49 mevcut skin) |
| 30 | Booster yuvası (kutu) normal / kilitli / tükendi | Oyun alt bar | `Ui/BoosterBar.cs (bst, shine)` | 60×60 pt → 180 px, köşe 16 pt; 3 hal | yok | 🎨 hazır; `UI/Task5/manifest.json` + HANDOFF (#36 renk/alpha, #49 mevcut skin) |
| 31 | Booster stok rozeti + kilit rozeti | Oyun alt bar | `Ui/BoosterBar.cs (badge)` | 24×24 pt → 72 px, daire; sayı dinamik | yok | 🎨 hazır; `UI/Task5/manifest.json` + HANDOFF (#36 renk/alpha, #49 mevcut skin) |
| 32 | Birincil düğme (turuncu/altın) | Tüm kartlar | `Ui/Card.cs, Ui/Screens.cs (btn)` | genişlik dinamik × 46 pt → 138 px yükseklik, köşe 11 pt | evet (köşe 33 px) | 🎨 hazır; `UI/Task5/manifest.json` + HANDOFF (#36 renk/alpha, #49 mevcut skin) |
| 33 | İkincil düğme (çerçeveli, saydam) | Tüm kartlar | `Ui/Card.cs, Ui/Screens.cs (btn)` | genişlik dinamik × 46 pt → 138 px | evet (köşe 33 px) | 🎨 hazır; `UI/Task5/manifest.json` + HANDOFF (#36 renk/alpha, #49 mevcut skin) |
| 34 | Kapat düğmesi (X) | Kartlar (yeni) | `— (kodda yok, kartlar düğmeyle kapanıyor)` | 36×36 pt → 108 px | yok | 🎨 hazır; `UI/Task5/manifest.json` + HANDOFF (#36 renk/alpha, #49 mevcut skin) |
| 35 | Kart zemini (popup) | Tüm kartlar | `Ui/Card.cs (card)` | genişlik 340 pt (dar ekranda ekran−52), yükseklik dinamik, köşe 18 pt → 1020 px genişlik | evet (54 px; #7 çerçevesiyle birlikte) | 🎨 hazır; `UI/Task5/manifest.json` + HANDOFF (#36 renk/alpha, #49 mevcut skin) |
| 36 | Kart arkası karartma (veil) | Tüm kartlar | `Ui/Card.cs (veil)` | tam ekran, düz renk %90 → doku gerekmez, yalnızca renk/opaklık | yok | 🎨 hazır; `UI/Task5/manifest.json` + HANDOFF (#36 renk/alpha, #49 mevcut skin) |
| 37 | Kart ikon karosu | Tanıtım, booster kartları | `Ui/Card.cs (icon)` | 52×52 pt → 156 px, köşe 14 pt | yok | 🎨 hazır; `UI/Task5/manifest.json` + HANDOFF (#36 renk/alpha, #49 mevcut skin) |
| 38 | Tahta tepsisi (tray) | Oyun | `Board/BoardView.cs (Tray)` | tahta + 36 pt; 4×4 → ~340 pt, 8×8 → ~340 pt (hücre küçülür) → 1020 px | evet (köşe 20 pt / 60 px) — #13 ile birleşebilir | 🎨 hazır; `UI/Task5/manifest.json` + HANDOFF (#36 renk/alpha, #49 mevcut skin) |
| 39 | Boş hücre yuvası (slot) | Oyun | `Board/BoardView.cs (Slot)` | hücre 40–76 pt → 228 px @3x en büyük | yok | 🎨 hazır; `UI/Task5/manifest.json` + HANDOFF (#36 renk/alpha, #49 mevcut skin) |
| 40 | Duvar (mühürlü kenar) şeridi | Oyun | `Board/BoardView.cs (Wall)` | kalınlık ~6 pt, uzunluk hücre × şerit → 18 px kalınlık, yatay tekrar eden doku | evet (yatay tile) | 🎨 hazır; `UI/Task5/manifest.json` + HANDOFF (#36 renk/alpha, #49 mevcut skin) |
| 41 | Zincir: kablo + halka + uç disk | Oyun | `Board/ChainView.cs (cable, link, disc)` | kablo kalınlık hücre×0,15; halka hücre×0,27×0,18; disk hücre×0,2 → hücre 76 pt için kablo 34 px, halka 62×40 px | kablo yatay tile | 🎨 hazır; `UI/Task5/manifest.json` + HANDOFF (#36 renk/alpha, #49 mevcut skin) |
| 42 | Çivi sayaç rozeti | Oyun | `Board/Badges.cs (chip)` | 17×17 pt → 51 px daire; sayı dinamik | yok | 🎨 hazır; `UI/Task5/manifest.json` + HANDOFF (#36 renk/alpha, #49 mevcut skin) |
| 43 | Bomba sayaç rozeti + "sıcak" (≤4) hali | Oyun | `Board/Badges.cs (chip, fuse)` | 17×17 pt → 51 px; sıcak hal kırmızı | yok | 🎨 hazır; `UI/Task5/manifest.json` + HANDOFF (#36 renk/alpha, #49 mevcut skin) |
| 44 | Yıldızlar (dolu / boş) | Sonuç kartı | `Ui/Screens.cs (Result)` | ~28 pt → 84 px, 2 hal | yok | 🎨 hazır; `UI/Task5/manifest.json` + HANDOFF (#36 renk/alpha, #49 mevcut skin) |
| 45 | İlerleme çubuğu (yükleme) | Yükleme ekranı | `WebGLTemplates/ReverseSolver/index.html (#loader .track/.fill)` | 180×6 pt → 540×18 px, iz + dolgu | evet (yatay) | 🎨 hazır; `UI/Task5/manifest.json` + HANDOFF (#36 renk/alpha, #49 mevcut skin) |
| 46 | Toast / bildirim balonu | Oyun ("Çivi kendiliğinden söküldü", tutorial yazısı) | `Ui/BoosterBar.cs (Notice)` | genişlik dinamik × ~28 pt → 84 px yükseklik | evet (yatay) | 🎨 hazır; `UI/Task5/manifest.json` + HANDOFF (#36 renk/alpha, #49 mevcut skin) |
| 47 | Bölüm listesi hücresi: açık / kilitli / bitmiş / sıradaki | Bölüm listesi | `Ui/Screens.cs (LevelList sel)` | 48,6 pt → 146 px, köşe 11 pt; 4 hal | yok | 🎨 hazır; `UI/Task5/manifest.json` + HANDOFF (#36 renk/alpha, #49 mevcut skin) |
| 48 | Tutorial hedef parlaması | Tutorial | `Board/TutorialHand.cs (TargetGlow)` | hücre boyutu kare çerçeve + parlama → 228 px | evet | 🎨 hazır; `UI/Task5/manifest.json` + HANDOFF (#36 renk/alpha, #49 mevcut skin) |
| 49 | Yapboz parçası skin (bkz. CODEX_GOREV Görev 4) | Oyun | `Board/PieceView.cs (prosedürel mesh)` | doku 256×256 px tileable + stil değerleri | —  | 🎨 hazır; `UI/Task5/manifest.json` + HANDOFF (#36 renk/alpha, #49 mevcut skin) |

## Kalan Claude çizimleri (PM_MESAJ_9, 25 Ekim) — #26–49'a ek

#26–49 ve aşağıdaki ek kalemler teslim edildi. Tek PNG seti: `Assets/Art/UI/Task5/` ve `Assets/Art/Gameplay/Task5/`; dosya bazında ölçü ve sınırlar HANDOFF.md / manifest.json içinde.

| Öğe | Ekran | Kod dosyası | Ölçü (pt → @3x px) | Durumlar | 9-slice | Durum |
|---|---|---|---|---|---|---|
| Üst bar ikonları: parça, saat kadranı, yeniden başlat oku | Oyun üst bar | `Ui/Hud.cs (pieceIcon, dial, hand, arc)` | 16–17 pt → 48–51 px | normal / basılı (reset) | yok | 🎨 hazır (font: öneri; parça efektleri: #49 skin) |
| Kilit ikonu | Booster yuvası, bölüm listesi | `Ui/Icons.cs (Lock)` | ~14 pt → 42 px | normal | yok | 🎨 hazır (font: öneri; parça efektleri: #49 skin) |
| Kart ikon karosundaki çizim ikonlar (yalnız Codex ikonu yoksa: kilit) | Kartlar | `Ui/Icons.cs` | 30 pt → 90 px | normal | yok | 🎨 hazır (font: öneri; parça efektleri: #49 skin) |
| Sonuç kartı yıldızları + doluyor animasyonu | Sonuç kartı | `Ui/Screens.cs (Result)` | ~28 pt → 84 px | dolu / boş / parlama | yok | 🎨 hazır (font: öneri; parça efektleri: #49 skin) |
| Sonuç kartı resim çerçevesi (bölüm resmi) | Kazanma kartı | `Board/PicturePiece.cs` | genişlik 296 pt, çerçeve ~6 pt → 888 px | normal | evet (18 px) | 🎨 hazır (font: öneri; parça efektleri: #49 skin) |
| Puan sorusu 1–10 sayı düğmeleri ve 👍/👎 düğmeleri | Sonuç kartı | `Ui/Screens.cs (Result, askFun/askHard)` | ~26×36 pt → 78×108 px; 👍/👎 = ikincil düğme | normal / seçili / basılı | evet (sayı düğmesi) | 🎨 hazır (font: öneri; parça efektleri: #49 skin) |
| Bomba "sıcak" nabzı ve kırmızı parlama | Oyun | `Board/Badges.cs (Glow, Pulse)` | ikonun %120'si → 2 kare ya da parlama halkası 72 px | normal / sıcak | yok | 🎨 hazır (font: öneri; parça efektleri: #49 skin) |
| Parça gölgesi, seçili parlaması, sürükleme gölgesi | Oyun | `Board/PieceView.cs` | skin (#49) içinde | normal / seçili / sürüklenen / çivili | — | 🎨 hazır (font: öneri; parça efektleri: #49 skin) |
| Zincir uç diski renk tonları | Oyun | `Board/ChainView.cs (Tints)` | disk 0,2 × hücre → 46 px | renk başına 1 | yok | 🎨 hazır (font: öneri; parça efektleri: #49 skin) |
| Yatay ekran uyarısı (telefonu çevir ikonu) | Yatay telefon | `WebGLTemplates/.../index.html (#rotate svg)` | 56 pt → 168 px | normal | yok | 🎨 hazır (font: öneri; parça efektleri: #49 skin) |
| İsim giriş kutusu | İsim ekranı | `Platform/Web/*.jslib (rs-name-in)` | genişlik 296 pt × 46 pt → 888×138 px | normal / odak | evet (33 px) | 🎨 hazır (font: öneri; parça efektleri: #49 skin) |
| Yazı tipi (RS Sans) | Her yer | `Presentation/Fonts/RS Sans SDF` | —: Codex stiline uyan bir yazı tipi önerisi (TTF, Türkçe karakterli, lisansı açık) | normal / kalın | — | 🎨 hazır (font: öneri; parça efektleri: #49 skin) |
| Menü "Level X" düğmesi (geçici: #3 A düğmesi 200×72 pt) | Ana menü | `Ui/Screens.cs (Menu)` | 200×72 pt → 600×216 px | normal / basılı | evet | 🎨 hazır (font: öneri; parça efektleri: #49 skin) |


## Claude Code notları

- Birim: oyun 1 birim = 1 CSS pt; iPhone @3x. Tüm görseller pt × 3 px üretilmeli.
- Eksik ekranlar (listede yok): **Bölümler listesi** (LevelList), **duraklatma/çıkış onayı**, **booster onay kartı** (Screens.Ask), **booster tanıtım kartı** (bintro, 5/9/15/23), **isim sorma ekranı** (oyuncu adı), **yükleme ekranı** (WebGL şablonu, ikon 192 kullanılıyor).
- Kayıp türü 3: bomba, süre, kilitlenme (hamle kalmadı). #9 iki değil üç görsel olabilir.
- Kullanılmayan: #13'teki "hard bölümlerde altın" için kodda bayrak artık var (`hard`); #6 ile aynı kaynak kullanılabilir.

## Ek UI ekranları

| # | Asset | Nerede | Ölçü / not | Durum |
|---|---|---|---|---|
| 20 | Bölüm listesi (LevelList) | Menü | 375×667 pt referans; Card 323×~624 pt → 969×1872 px. 5×8 hücre, 48.6 pt (145.8 px), 9 pt (27 px) aralık | 🎨 Tek SVG artboard 1125×2001 px @3x hazır (`MissingScreens/level-list-1125x2001.svg`); inceleme bekliyor ; ortak yazısız PNG parçalar: `UI/Task5/` (kart, düğmeler, dört hücre, isim kutusu, iz/dolgu) |
| 21 | Duraklatma / bölümden çıkış onayı | Oyun | Ortak Card 323 pt genişlik (969 px @3x); düğmeler 46 pt (138 px) | 🎨 Tek SVG artboard 1125×2001 px @3x hazır (`MissingScreens/pause-exit-1125x2001.svg`); inceleme bekliyor ; ortak yazısız PNG parçalar: `UI/Task5/` (kart, düğmeler, dört hücre, isim kutusu, iz/dolgu) |
| 22 | Booster onay kartı (Screens.Ask) | Oyun | Ortak Card 323 pt; ikon 52 pt (156 px), iç alan 279 pt (837 px), iki düğme 46 pt (138 px) | 🎨 Saat örnekli tek SVG artboard 1125×2001 px @3x hazır (`MissingScreens/booster-confirm-1125x2001.svg`); stok metni dinamik ; ortak yazısız PNG parçalar: `UI/Task5/` (kart, düğmeler, dört hücre, isim kutusu, iz/dolgu) |
| 23 | Booster tanıtım kartı (5/9/15/23) | Oyun | Ortak Card 323 pt; ikon 52 pt (156 px), başlık 20 pt (60 px), gövde/ipucu 13 pt (39 px), düğme 46 pt (138 px) | 🎨 Seviye 5 Makas örnekli tek SVG artboard 1125×2001 px @3x hazır (`MissingScreens/booster-intro-1125x2001.svg`); aynı şablon 9/15/23'te değişken içerikle kullanılır ; ortak yazısız PNG parçalar: `UI/Task5/` (kart, düğmeler, dört hücre, isim kutusu, iz/dolgu) |
| 24 | İsim sorma ekranı | İlk açılış | Web HTML kartı max 340 pt; 375 pt ekranda yatay padding nedeniyle 323 pt; başlık 20 pt, açıklama 13 pt, giriş 16 pt, düğme 15 pt | 🎨 Tek SVG artboard 1125×2001 px @3x hazır (`MissingScreens/name-gate-1125x2001.svg`) ; ortak yazısız PNG parçalar: `UI/Task5/` (kart, düğmeler, dört hücre, isim kutusu, iz/dolgu) |
| 25 | Yükleme ekranı | WebGL açılışı | Tam ekran; ikon 96 pt (288 px), progress 180×6 pt (540×18 px), boşluk 18 pt (54 px), mesaj 13 pt (39 px) | 🎨 Onaylı 192 ikonlu tek SVG artboard 1125×2001 px @3x hazır (`MissingScreens/loading-1125x2001.svg`); progress dinamik ; ortak yazısız PNG parçalar: `UI/Task5/` (kart, düğmeler, dört hücre, isim kutusu, iz/dolgu) |

İnceleme: `Assets/Art/MissingScreens/review-v01.html`. A/B adaylarının toplu sayfası: `SECIM.html`.

## Görev 4 — oyun içi görsel seti (25 Ekim)

İnceleme sayfası: `Assets/Art/Task4/review-v01.html`.

| İş | Teslim | Ölçü / kullanım | Durum |
|---|---|---|---|
| 4a Oyna düğmesi | `Assets/Art/Menu/Buttons/play-button-normal-600x216.svg`, `play-button-pressed-600x216.svg` | 600×216 px; yazı ayrı; 9-slice köşe sınırı 72 px  | ✅ Normal ve basılı durum hazır |
| 4b Dikey tutorial eli | `Assets/Art/TutorialHand/Exports/hand-tap-vertical-288.png`, `hand-swipe-vertical-288.png` | Her biri 288×288 px, şeffaf  | ✅ Tap ve swipe pozları hazır; tap parmak ucu (151, 16) px, swipe yaklaşık (136, 15) px |
| 4c Yapboz parçası skin | `Assets/Art/PieceSkin/Review/piece-direction-a-256.svg`, `piece-direction-b-256.svg` | Yön başına tek 256×256 SVG örnek; durum önizlemesi review sayfasında | 🎨 Örnek Zeyd onayı bekliyor; 81 üretimi ve script onaya kadar beklemede |
| 4d Üst bar / HUD | `Assets/Art/Gameplay/HUD/hud-strip-1125x180.svg`, `clock-pill-urgent-354x132.svg` | HUD 1125×180 px; acil sayaç 354×132 px; sayılar dinamik  | ✅ Normal ve acil durum hazır |
| 4e Booster yuvası | `Assets/Art/Gameplay/Boosters/booster-slots-1125x330.svg` | 60 pt slot, 13 pt aralık; boş/dolu/kilitli/sayaçlı  | ✅ Dört durum hazır; ikon ve sayaç metni runtime’da değiştirilecek |
| 4f Oyun arka planı | `Assets/Art/Gameplay/Backgrounds/baron-castle-play-v01-1125x2436.png` | 1125×2436 px portre; orta alan düşük kontrastlı  | ✅ Tek sahne hazır |
| 4g Zincir ve duvar | `Assets/Art/Gameplay/BoardSprites/chain-link-repeat-v01.svg`, `wall-stripe-repeat-v01.svg` | Ölçeklenebilir tile sprite; zincir halka, duvar şeridi tekrar kullanımı  | ✅ İki tahta sprite’ı hazır |
| 4h Genel UI kiti | `Assets/Art/UI/ui-kit-v01.svg` | Birincil/ikincil/kapat, kart, çivi/bomba rozeti, yıldız, ilerleme, toast/uyarı  | ✅ Tek SVG kit hazır |

Claude Code UI envanteri #26–49 eklendi ve #26–48 üretildi. Toplu önizleme: `Assets/Art/Inventory/ui-inventory-review-v01.html`. #49’daki mevcut procedural skin ve 4×4 inceleme korunuyor; bu turda 81 parça dışa aktarılmadı. Yeni 4c A/B örnekleri için Zeyd’in yön seçimi bekleniyor.

Ek çizim envanteri (aynı önizleme sayfasında):

| Öğe | Teslim | Durum |
|---|---|---|
| HUD ikonları: parça, saat, reset | `Assets/Art/Inventory/hud-icons-153x51.svg`  | ✅ |
| Kilit ikonu ve kart ikonu karosu | `Assets/Art/Inventory/lock-icon-42.svg`, `lock-tile-156.svg`  | ✅ |
| Sonuç yıldızı animasyon kareleri | `Assets/Art/Inventory/result-star-frames-84x252.svg`  | ✅ Dolu/boş/parlama, 3 kare |
| Sonuç resim çerçevesi | `Assets/Art/Inventory/result-picture-frame-888x592.svg`  | ✅ |
| Puan 1–10, beğen/beğenme düğmeleri | `Assets/Art/Inventory/score-input-atlas-936x108.svg`  | ✅ |
| Bomba sıcak nabız/parlama | `Assets/Art/Inventory/bomb-pulse-144x72.svg`  | ✅ İki görünüm |
| Zincir uç disk tonları | `Assets/Art/Inventory/chain-endpoint-tints-184x46.svg`  | ✅ Dört ton |
| Yatay ekran uyarısı | `Assets/Art/Inventory/rotate-phone-168.svg`  | ✅ |
| İsim giriş alanı | `Assets/Art/Inventory/name-input-states-888x276.svg`  | ✅ Normal/odak kareleri |
| RS Sans font önerisi | Nunito; Türkçe Latin Extended ve açık lisans  | ✅ Öneri; TTF entegrasyonu ayrı |
| Menü “Level X” düğmesi | `Assets/Art/Menu/Buttons/play-button-normal-600x216.svg`, `play-button-pressed-600x216.svg`  | ✅ Yazı katmanı ayrı |

Font kaynağı/lisans: [Nunito OFL 1.1](https://github.com/googlefonts/nunito/blob/main/OFL.txt). Var olan `PieceSkin/skin.json` ve procedural 4×4 review korunmuştur; yeni 4c örneğine onay gelene kadar bu mevcut dosyalar değiştirilmez.
- #20–25 SVG'leri tam ekran örnek; parçaları (düğme, hücre, çubuk) #26+ envanterinde ayrı istenir. #24 isim ve #25 yükleme ekranı önizlemede CSS ile SVG'ye uyduruldu. #21 duraklatma: oyunda duraklat/çık akışı yok, ürün kararı gerekir. #20 bölüm listesi: yeni menüde "Bölümler" düğmesi yok, önizlemede ulaşılamaz.
