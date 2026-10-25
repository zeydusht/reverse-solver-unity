# Mühendisliğe sanat teslimi

## Yapboz skin · 25 Ekim 2026

| Asset path | Nereye gider | Boyut | Hareket / kullanım |
|---|---|---|---|
| `Assets/Art/PieceSkin/skin.json` | Prosedürel parça görünümü #49 | pt birimli stil sözleşmesi | Normal, seçili, sürüklenen, çivili; entegrasyon yapılmadı |
| `Assets/Art/PieceSkin/body-256.png` | Parça gövde dolgusu | 256×256 opak PNG | Repeat; düz krem, kontrast %0; tablo %75 opaklıkla üstüne |
| `Assets/Art/PieceSkin/review.html` | Sanat incelemesi | Üç tablo × dört durum; her biri 4×4 | 40–76 pt ayarlanabilir; yerelden açılır |
| `Assets/Art/PieceSkin/review-data.js` | Yerel inceleme verisi | skin.json eş kopyası | JSON değişirse birlikte güncellenir |
| `Assets/Art/PieceSkin/Review/tonal-study.svg` | Üçüncü tablo / ton testi | 768×768 vektör | Açık, koyu ve yoğun desen; yalnız inceleme |
| `Assets/Art/PieceSkin/Review/review-{40,56,76}pt.png` | Görsel doğrulama | Tarayıcı ekran kayıtları | Hücre boyutu sınırları ve varsayılan değer |
| `docs/art/reports/PIECE_SKIN.md` | Teknik teslim | Renk, alfa, pt ve çizim sırası | Mühendislik uygulama notları; cihaz testi yapılmadı |

## Güncel sanat paketi · 24 Ekim 2026

Gözden geçirme ana sayfası: `Assets/Art/ProductionReview.html`. Kaynak/katmanlı menü örneği: `Assets/Art/Menu/index.html`. Bu dosyalar sanat teslimidir; Unity projesine import veya kod entegrasyonu yapılmadı.

| Asset path | Nereye gider | Boyut | Hareket / kullanım |
|---|---|---:|---|
| `Assets/Art/Icon/Exports/2-market-mavi-1024.png` | Uygulama mağazası / ana ikon kaynağı | 1024×1024 opak | Statik; onaylı ikonun yeniden boyutlandırılmış sürümü |
| `Assets/Art/Icon/Exports/2-market-mavi-512.png`, `...-256.png` | Küçük ikon hedefleri / inceleme | 512×512, 256×256 opak | Statik; otomatik köşe maskesi uygulanmadı |
| `Assets/Art/Icon/Exports/2-market-mavi-192.png`, `...-180.png`, `...-32.png` | PWA, Apple touch ve favicon hedefleri | 192×192, 180×180, 32×32 opak | Aynı onaylı ikonun küçültülmüş kopyaları; kaynak korunur |
| `Assets/Art/Menu/corridor-v04-source.png` | Menü sahne arka planı | 1024×1536 | Seçilmiş v04'ün birebir kopyası, tek katman |
| `Assets/Art/Menu/safe-area-options-v01.html` | Menü UI güvenli alanı incelemesi | v04 oranlı iki mockup | Tuş A alçak, B daha yukarı; kesikli kılavuzlar örnek safe area'dır, cihaz testi değildir |
| `Assets/Art/Menu/Exports/level-button-teal-1024x320.png`, `...orange-1024x320.png` | Menü LEVEL X UI tuşu | 1024×320 şeffaf | Yazı ayrı dinamik UI metni olmalı |
| `Assets/Art/Menu/Buttons/level-button-teal-option-a-888x138.png`, `level-button-orange-option-b-888x138.png` | Menü LEVEL X UI tuşu, 3× kaynak | 888×138 şeffaf; Unity 9-slice sınırı 126/42/126/42 px | A/B renk seçimi bekliyor; Unity `.meta` sınırları hazır. İnceleme: `Buttons/review-options-v01.html` |
| `Assets/Art/Menu/safe-area-options-v01.html` | Menü telefon oranı ve UI güvenli alanı | 375×667 oranlı iki mockup | Aşağı/yukarı tuş yüksekliği seçeneği |
| `Assets/Art/AnimationDemo/sprites/jiggle-run-01.png`, `jiggle-run-02.png` | Animatik / koşu döngüsü | 560×741, 562×741 şeffaf | İki kare, döngü |
| `Assets/Art/AnimationDemo/sprites/baron-run-01.png`, `baron-run-02.png` | Animatik / kovalamaca | 548×795, 501×797 şeffaf | İki kare, daha küçük arka katman |
| `Assets/Art/TutorialHand/Options/option-a-tap-288.png`, `option-a-swipe-288.png` | Tutorial dokun/kaydır işareti | İki adet 288×288 şeffaf PNG | Seçenek A: krem eldiven + teal manşet; 2 ayrı durum |
| `Assets/Art/TutorialHand/Options/option-b-tap-288.png`, `option-b-swipe-288.png` | Tutorial dokun/kaydır işareti | İki adet 288×288 şeffaf PNG | Seçenek B: çıplak ten rengi el; seçimi bekliyor |
| `Assets/Art/TutorialHand/Options/review-options-v01.html` | El işareti A/B incelemesi | Eldivenli ve çıplak el; tap/kaydır pozları | Zeyd'in stil seçimi bekleniyor |
| `Assets/Art/ObstacleIntros/OptionA/{nail,bomb,chain,wall}-888x592.png` | Engel tanıtım kartları | Dört adet 888×592 PNG | Baron orta plan, inceleme adayı |
| `Assets/Art/ObstacleIntros/OptionB/{nail,bomb,chain,wall}-888x592.png` | Engel tanıtım kartları | Dört adet 888×592 PNG | Engeller yakın plan, inceleme adayı |
| `Assets/Art/ObstacleIntros/review-options-v01.html` | Engel tanıtım A/B incelemesi | Dört engelin iki kadrajı | Kart metni dinamik UI'da kalır; Zeyd seçimi bekleniyor |
| `Assets/Art/HardWarning/option-a-baron-warning-888x592.png`, `option-b-jiggle-warning-888x592.png` | Zor bölüm uyarı kartı | İki adet 888×592 PNG | A Baron uyarıyor, B endişeli Jiggle; metin/düğme dinamik UI'da |
| `Assets/Art/HardWarning/review-options-v01.html` | Zor bölüm uyarısı A/B incelemesi | İki kadraj | Zeyd seçimi bekleniyor |
| `Assets/Art/UI/PopupFrame/option-a-teal-gold-888x592.png`, `option-b-cream-teal-888x592.png` | Tanıtım ve uyarı kartı çerçevesi | İki adet 888×592 şeffaf merkezli PNG | A/B; Unity 9-slice sınırı 54 px; seçim bekleniyor |
| `Assets/Art/UI/PopupFrame/option-a-teal-gold-888x592.png.meta`, `option-b-cream-teal-888x592.png.meta` | Unity sprite import ayarları | 9-slice border 54 px | Sprite metadata hazır |
| `Assets/Art/UI/PopupFrame/review-options-v01.html` | Kart çerçevesi A/B incelemesi | Boş merkez örneği | Zeyd seçimi bekleniyor |
| `Assets/Art/Victory/option-a-single-jiggle-768.png`, `option-b-three-jiggles-768.png` | Zafer sonucu illüstrasyonu | İki adet 768×768 şeffaf PNG | Tek Jiggle / üç renkli Jiggle; seçim bekleniyor |
| `Assets/Art/Victory/review-options-v01.html` | Zafer pozu A/B incelemesi | İki grup yoğunluğu | Zeyd seçimi bekleniyor |
| `Assets/Art/Defeat/OptionA/{bomb,time,no-moves}-512.png` | Kaybetme sonucu, üç sebep | Üç adet 512×512 PNG | Baron alaycı, Jiggle üzgün; sıcak orta plan |
| `Assets/Art/Defeat/OptionB/{bomb,time,no-moves}-512.png` | Kaybetme sonucu, üç sebep | Üç adet 512×512 PNG | Teatral yakın plan; üç sebep ayrı gösterilir |
| `Assets/Art/Defeat/OptionA,OptionB/*-888x296.png` | Kaybetme geniş kadraj eskizleri | Altı adet 888×296 PNG | Önceki alternatifler korunuyor; inceleme kare 512 px sürümleri kullanır |
| `Assets/Art/Defeat/review-options-v01.html` | Kaybetme A/B incelemesi | 2 yön × 3 sebep | Zeyd seçimi bekleniyor |
| `Assets/Art/Gameplay/ObstacleIcons/{bomb,nail,chain,wall}-128.png` | Oyun içi engel ikonları | Dört adet 128×128 şeffaf PNG | Mevcut onay öncesi konsept master'larından küçültüldü; sayaç ayrı UI metni |
| `Assets/Art/Gameplay/ObstacleIcons/index.html` | Engel ikonları önizlemesi | Dört ikon | Küçük hücre okunurluğu için inceleme |
| `Assets/Art/Gameplay/BoosterIcons/{scissors,wand,hammer,clock}-128.png` | Alt bar booster ikonları | Dört adet 128×128 şeffaf PNG | Mevcut konsept master'larından küçültüldü |
| `Assets/Art/Gameplay/BoosterIcons/index.html` | Booster ikonları önizlemesi | Dört ikon | Zeyd incelemesi |
| `Assets/Art/Paintings/DirectionA/outside-world-coast-1080x728.png` | Yapboz tablo örneği, dış dünya yönü | 1080×728 PNG | Jigsaw çizgileri eklenmedi; orta kontrast örnek |
| `Assets/Art/Paintings/DirectionB/baron-favorites-1080x728.png` | Yapboz tablo örneği, Baron favorileri yönü | 1080×728 PNG | Jigsaw çizgileri eklenmedi; seri yönü seçimi bekleniyor |
| `Assets/Art/Paintings/review-directions-v01.html` | Tablo yönü A/B | İki aday | Seri üretimine geçmek için Zeyd yön seçimi gerekli |
| `Assets/Art/Gameplay/BoardBase/option-a-walnut-teal-888.png`, `option-b-hard-gold-888.png` | Oyun tahtası altlığı | İki adet 888×888 şeffaf merkezli PNG | Normal sıcak ahşap / altın zor seviye; seçim bekleniyor |
| `Assets/Art/Gameplay/BoardBase/review-options-v01.html` | Tahta altlığı A/B incelemesi | İki frame alternatifi | Zeyd seçimi bekleniyor |
| `Assets/Art/Audio/OptionA/{weee,piece,win,loss}.wav` | Oyun sesleri | Dört kısa mono PCM WAV | Yumuşak çan yönü, konsept; entegrasyon/miksaj yok |
| `Assets/Art/Audio/OptionB/{weee,piece,win,loss}.wav` | Oyun sesleri | Dört kısa mono PCM WAV | Parlak oyun çanı yönü, konsept; seçim bekleniyor |
| `Assets/Art/Audio/review-options-v01.html` | Ses A/B dinleme sayfası | İki yön × dört ses | Zeyd tını seçimi bekleniyor |
| `Assets/Art/MissingScreens/level-list-1125x2001.svg` | Bölüm listesi ekranı | 1125×2001 px SVG artboard @3x; 5×8 hücre | 375×667 pt referansı; Card/cell ölçüleri Screens.LevelList ve Card.cs'ye göre |
| `Assets/Art/MissingScreens/pause-exit-1125x2001.svg` | Duraklatma ve çıkış onayı konsepti | 1125×2001 px SVG artboard @3x | 323 pt kart, 46 pt düğmeler; mevcut kodda ayrı modal yok, tasarım önerisi |
| `Assets/Art/MissingScreens/booster-confirm-1125x2001.svg` | Booster onay kartı | 1125×2001 px SVG artboard @3x | Screens.Ask; saat örneği, stok değeri örnektir/dinamiktir |
| `Assets/Art/MissingScreens/booster-intro-1125x2001.svg` | Booster tanıtım kartı | 1125×2001 px SVG artboard @3x | Seviye 5 Makas örneği; seviye 9 Değnek, 15 Saat, 23 Çekiç aynı kart ölçülerini kullanır |
| `Assets/Art/MissingScreens/name-gate-1125x2001.svg` | Oyuncu adı giriş ekranı | 1125×2001 px SVG artboard @3x | WebBridge HTML ölçülerine göre; sistem klavyesi tarayıcı tarafından açılır |
| `Assets/Art/MissingScreens/loading-1125x2001.svg` | WebGL yükleme ekranı | 1125×2001 px SVG artboard @3x | Mevcut 96 pt ikon, 180×6 pt progress bar ve 13 pt mesaj ölçüleri; animasyon/ilerleme uygulama kodunda |
| `Assets/Art/MissingScreens/review-v01.html` | Yeni ekranlar #20–25 önizlemesi | Altı tek seçenekli ekran | SVG'ler 375×667 pt @3x referansına göre |
| `docs/art/SECIM.html` | Zeyd için tüm sanat A/B seçimleri | Birleşik önizleme sayfası | P0/P1 incelemeleri ve yeni tek seçenekli ekranlar |
| `Assets/Art/AnimationDemo/index.html` | Sanat incelemesi | 480×720 canvas | 2,8 sn tekrar oynatılan sprite/tween animatik; katmanlı final kamera değil |
| `Assets/Art/Obstacles/Exports/{bomb,nail,chain,wall}.png` | Engel ikon/oyun atlası adayları | Her biri 1024×1024 şeffaf | Statik konsept |
| `Assets/Art/Boosters/Exports/{scissors,wand,hammer,clock}.png` | Güçlendirici ikon/oyun atlası adayları | Her biri 1024×1024 şeffaf | Statik konsept |

İkon ve v04 koridor Zeyd'in seçimiyle onaylıdır. Düğme, birleşik menü pozisyonu, koşu kareleri ve engel/güçlendirici tasarımları yeni inceleme adaylarıdır; kart/ikon onaylı referansların üzerine yazılmaz. PNG master'ları `Assets/Art/` içinde korunur.

Gerçek zafer geçişini üretmek için tek parça koridor PNG'si yerine duvar/çerçeve/tablo içi ve karakter katmanları ile kamera hareketi kurulmalıdır. Animatikte yeni level yalnız rozet metniyle temsil edilir; boya görseli/çerçeve bağımsız biçimde değişmez. Bu teslim mühendislikte hangi katmanların gerektiğini gösterir, çalışma zamanı değişikliği içermez.

**Güncel onaylı ikon:** Assets/Art/Review/Ikon/2-market-mavi-v01.png / Zeyd'in seçtiği ikon, turkuaz–mercan–lavanta üç Jiggles ve Baron, mavi hız çizgili fon / 1254×1254 opak PNG; 1024/512/256 dışa aktarımları `Assets/Art/Icon/Exports/` altında hazır. Statik, oyuna entegre edilmedi. Kırmızı/sarı/yeşil revizyon kullanılmaz. Güncel boyutlar ve durum aşağıdaki 24 Ekim tesliminde.

## Tarihsel paralel inceleme notları (sonraki kararlar geçerlidir)

O tarihte en yeni olan koridor revizyonu: Assets/Art/Review/Koridor/2-ortali-yan-blok-v05.png / ana duvar bloğunda ortalanmış büyük tablo + sağ sonraki blokta küçük tablo. Zeyd daha sonra v04'ü seçti; v05 yalnız tarihsel alternatif olarak kalır.

Güncel ikon renk revizyonu: Assets/Art/Review/Ikon/3-market-kirmizi-sari-yesil-v01.png / ortada kırmızı, solda sarı, sağda yeşil; mavi hız çizgili ikon, kullanıcı değerlendirmesi / kare opak PNG / statik; henüz entegre edilmedi

Seçilen koridorun güncel revizyonu: Assets/Art/Review/Koridor/2-tek-cerceve-v03.png / tek büyük tablo içeren statik menü; yeni revizyon değerlendirme bekliyor / 1024×1536 PNG / statik; zaferden koridora geri kamera/kaçış fikri LEVEL_GECISI.md içinde, uygulama yapılmadı

En yeni ikon: Assets/Art/Review/Ikon/2-market-mavi-v01.png / market referansıyla mavi gradyan ve beyaz hız çizgileri, üç Jiggles + Baron; onay bekliyor / 1254×1254 opak PNG / statik, mevcut ikonların yerine henüz entegre edilmedi

Toplu görsel inceleme: docs/art/INCELEME.html. Bu gruptaki yeni varlıklar henüz Zeyd onaylı değildir ve mevcut kaynakların yerine geçirilmedi. Onaylı/güncel dosyalar ve yedek kaydı: REFERANS_DURUMU.md.

Assets/Art/Review/Ikon/1-safe-area.png / üç Jiggles + Baron ikonunun kenar boşluğu revizyonu; inceleme / 1254×1254 opak PNG / statik; Assets/Art/Review/Ikon/preview.html küçük boyut ve maske önizlemesi, cihaz testi değil

Assets/Art/Review/Koridor/1-bordo-v02.png / statik menü koridoru 1, merkezi tablo, stil kartına uyumlu bordo revizyon; inceleme / 1024×1536 PNG / statik tek katman; UI, zoom veya tablo etkileşimi yok

Assets/Art/Review/Koridor/2.png / statik menü koridoru 2, hafif perspektif; inceleme / 1024×1536 PNG / statik tek katman; oyun tahtası altlığı değildir

Assets/Art/Review/Jiggles/1-poz-kontrol.png / aynı Jiggle kimliğinin üç pozunda geometri/uzuv incelemesi / 1536×1024 PNG / referans görseli; animasyon atlası değil

Eski Jiggles v01 ve iptal edilen ekstra tatlı ifade/tulum yalnız docs/art/backups/ ZIP'inde tutulur. Aşağıdaki eski teslim notlarında adlarının geçmesi tarihsel kayıttır. Güncel aktif Jiggles kartı v02'dir.

Baron aday 3 onaylıdır; referans kartları ve dört ikon seçeneği hazır. İkon seçimi yapılmadığı için uygulama ikonu değiştirilmemeli. Playtest öncesi kapsam: ikon ve statik menü koridoru; stil ve karakter referansları bunların hazırlığıdır.

Teslim satırı biçimi:

`asset path / where it goes / size / animation notes`

Yakınlaşma geçişi, oyun içi tablolar ve sayaç entegrasyonu playtest sonrasıdır.

## İnceleme teslimi — 23 Ekim

Güncel ikon revizyonu: Assets/Art/Ornekler/Ikon/4-uc-jiggles-v02.png / ikon 4 yönünün üç Jiggles ve gradyanlı sürümü; nihai onay bekleniyor / kare PNG / statik, entegrasyon yapılmamalı

Assets/Art/Characters/Jiggles-reference-v02.png / görsel üretim ana çalışma referansı; oyuna sprite olarak koyma / 1536×1024 PNG / statik, rig veya animasyon atlası değil

Assets/Art/Characters/Baron-reference-v01.png / onaylı aday 3'ten ana çalışma referansı; öfke ifadesi temel / 1536×1024 PNG / statik, rig veya animasyon atlası değil

Assets/Art/Ornekler/Ikon/1.png–4.png / uygulama ikonu seçenekleri; Zeyd seçiminden sonra teslim boyutları çıkarılacak / her biri 1254×1254 opak PNG / statik; köşe maskesi uygulanmadı

Jiggles-reference-v01.png kullanılmamalı; anatomik hata içerir, v02 ile düzeltildi. Gerçek telefon ikon testi ve format dönüşümleri bekliyor. Menü koridorunun nihai seçimi açık; bu tur kod veya entegrasyon yapılmadı.

## Görev 4 teslimi — 25 Ekim

- 4a `Assets/Art/Menu/Buttons/play-button-normal-600x216.svg` ve `play-button-pressed-600x216.svg` / menü Oyna düğmesi / 600×216 px / yazı ayrı; 9-slice köşe sınırı 72 px.
- 4b `Assets/Art/TutorialHand/Exports/hand-tap-vertical-288.png`, `hand-swipe-vertical-288.png` / tutorial eli / 288×288 px şeffaf / parmak ucu tap (151,16), swipe yaklaşık (136,15) px; iki ayrı poz.
- 4c `Assets/Art/PieceSkin/Review/piece-direction-a-256.svg`, `piece-direction-b-256.svg`, `piece-sample-review-v01.html` / oyun tahtası parça görünümü / yön başına tek 256×256 SVG örnek / seçili, sürüklenen ve çivili durumlar HTML incelemesinde; 81 parça üretimi Zeyd’in görsel onayını ve Claude Code formatını bekliyor.
- 4d `Assets/Art/Gameplay/HUD/hud-strip-1125x180.svg`, `clock-pill-urgent-354x132.svg` / oyun üst barı / 1125×180 ve 354×132 px / level, hamle, süre metinleri runtime’da basılmalı.
- 4e `Assets/Art/Gameplay/Boosters/booster-slots-1125x330.svg` / alt booster barı / 1125×330 px artboard; 60 pt kutu, 13 pt aralık / boş, dolu, kilitli ve sayaçlı örnekler; sayı runtime’da değişir.
- 4f `Assets/Art/Gameplay/Backgrounds/baron-castle-play-v01-1125x2436.png` / oyun sahnesi arka planı / 1125×2436 px / statik; merkezde tahta için sakin alan bırakıldı.
- 4g `Assets/Art/Gameplay/BoardSprites/chain-link-repeat-v01.svg`, `wall-stripe-repeat-v01.svg` / oyun tahtası / ölçeklenebilir tile SVG / zincir uzunluğu ve duvar şeridi çizimde tekrarlanır.
- 4h `Assets/Art/UI/ui-kit-v01.svg` / genel UI kit / 1125×1060 px SVG artboard / sayaç ve metin örnekleri dinamik değerlerle değiştirilmeli.
- Toplu önizleme: `Assets/Art/Task4/review-v01.html`.
- Claude Code UI envanteri #26–49 ASSET_IHTIYAC.md’ye eklendi. #26–48 teslimleri `Assets/Art/Inventory/ui-inventory-review-v01.html` içinde gözden geçirilebilir.

## Claude Code envanteri teslimi — #26–48

- #26 `Assets/Art/Gameplay/Backgrounds/gameplay-backdrop-1125x2001.png` / oyun tam ekran arka planı / 1125×2001 px.
- #27 `Assets/Art/Inventory/level-badge-156.svg` / HUD level rozeti / 156×156 px; numara ayrı.
- #28 `Assets/Art/Gameplay/HUD/hud-strip-1125x180.svg`, `Assets/Art/Inventory/hud-icons-153x51.svg` / hamle-süre pill’leri ve ikon atlası / @3x; dinamik değerler ayrı.
- #29 `Assets/Art/Inventory/reset-button-138.svg` / reset / 138×138 px.
- #30 `Assets/Art/Inventory/booster-states-768x180.svg` / booster slotları / 4×180×180 px kare: normal, kilitli, tükenmiş, seçili.
- #31 `Assets/Art/Inventory/booster-badges-144.svg` / stok ve kilit rozeti / 2×72 px; sayı alanı boş.
- #32–33 `Assets/Art/Inventory/card-buttons-660x276.svg` / birincil ve ikincil düğmeler / her biri 660×138 px, 9-slice.
- #34 `Assets/Art/Inventory/close-108.svg` / kapat / 108×108 px.
- #35 `Assets/Art/UI/ui-kit-v01.svg` / popup zemini / 1020 px geniş modül.
- #36 `Assets/Art/Inventory/veil-1125x2001.svg` / popup karartma / #0e2a2e, %90.
- #37 `Assets/Art/Inventory/lock-tile-156.svg` / kart ikonu karosu / 156×156 px.
- #38 `Assets/Art/Inventory/board-tray-1020.svg` / tahta tepsisi / 1020×1020 px.
- #39 `Assets/Art/Inventory/empty-slot-228.svg` / boş hücre / 228×228 px.
- #40 `Assets/Art/Inventory/wall-edge-tile-228x18.svg` / duvar şeridi / 228×18 px yatay tile.
- #41 `Assets/Art/Inventory/chain-complete-228x62.svg`, `Assets/Art/Gameplay/BoardSprites/chain-link-repeat-v01.svg` / kablo, uç diskleri ve tekrarlanan halka.
- #42–43 `Assets/Art/Inventory/obstacle-badges-153.svg`, `bomb-pulse-144x72.svg` / çivi-bomba rozetleri ve sıcak nabız; sayaç dinamik.
- #44 `Assets/Art/Inventory/stars-filled-empty-168x84.svg` / dolu-boş yıldız / 84 px.
- #45 `Assets/Art/Inventory/progress-540x18.svg` / loading track ve fill / 540×18 px.
- #46 `Assets/Art/Inventory/toast-900x84.svg` / bildirim balonu / 900×84 px, yatay uzatılabilir.
- #47 `Assets/Art/Inventory/level-cells-584x146.svg` / dört bölüm hücresi durumu / her hücre 146×146 px.
- #48 `Assets/Art/Inventory/target-glow-228.svg` / tutorial hedef parlaması / 228×228 px.
- #49’daki mevcut `PieceSkin/skin.json`, `body-256.png` ve procedural review korunuyor; yeni 4c yön örnekleri onay bekliyor, bu turda 81 parça dışa aktarılmadı.

## Ek çizim kalemleri

- `Assets/Art/Inventory/result-star-frames-84x252.svg` / sonuç yıldızı dolu-boş-parlama kareleri / 3×84×84 px.
- `Assets/Art/Inventory/result-picture-frame-888x592.svg` / sonuç kartı bölüm resmi çerçevesi / 888×592 px.
- `Assets/Art/Inventory/score-input-atlas-936x108.svg` / 1–10 ve beğen/beğenme düğme atlası / 12×78×108 px.
- `Assets/Art/Inventory/chain-endpoint-tints-184x46.svg` / zincir uç renkleri / dört 46 px disk.
- `Assets/Art/Inventory/rotate-phone-168.svg` / yatay ekran döndürme ikonu / 168×168 px.
- `Assets/Art/Inventory/name-input-states-888x276.svg` / isim girişi normal ve odak durumu / iki 888×138 px kare.
- Font önerisi: Nunito, Türkçe karakterli, SIL OFL 1.1; lisans ve durum `ASSET_IHTIYAC.md` içinde. Font binary’si bu turda kopyalanmadı.
- Toplu önizleme: `Assets/Art/Inventory/ui-inventory-review-v01.html`.

## Görev 5 ve 5b PNG teslimi

İnceleme: `Assets/Art/UI/Task5/review.html`. Makine listesi: `Assets/Art/UI/Task5/manifest.json`. Sınırlar Unity sırasıyla sol/alt/sağ/üst px; pivot aksi belirtilmedikçe merkez (0.5,0.5). Metin ve sayılar çizilmedi.

| Dosya | Ölçü | # | 9-slice / tekrar / not |
|---|---|---|---|
| `Assets/Art/Gameplay/Task5/level-badge.png` | 156×156 | 27 | yok; şeffaf PNG |
| `Assets/Art/Gameplay/Task5/hud-pill-normal.png` | 354×138 | 28 | L/B/R/T 69/24/69/24 px; şeffaf PNG |
| `Assets/Art/Gameplay/Task5/hud-pill-hot.png` | 354×138 | 28 | L/B/R/T 69/24/69/24 px; şeffaf PNG |
| `Assets/Art/Gameplay/Task5/reset-normal.png` | 138×138 | 29 | yok; şeffaf PNG |
| `Assets/Art/Gameplay/Task5/reset-pressed.png` | 138×138 | 29 | yok; şeffaf PNG |
| `Assets/Art/Gameplay/Task5/booster-normal.png` | 180×180 | 30 | yok; İkon ayrı katman; locked kilit içerir. |
| `Assets/Art/Gameplay/Task5/booster-empty.png` | 180×180 | 30 | yok; İkon ayrı katman; locked kilit içerir. |
| `Assets/Art/Gameplay/Task5/booster-locked.png` | 180×180 | 30 | yok; İkon ayrı katman; locked kilit içerir. |
| `Assets/Art/Gameplay/Task5/booster-out.png` | 180×180 | 30 | yok; İkon ayrı katman; locked kilit içerir. |
| `Assets/Art/Gameplay/Task5/booster-badge-stock.png` | 72×72 | 31 | yok; şeffaf PNG |
| `Assets/Art/Gameplay/Task5/booster-badge-lock.png` | 72×72 | 31 | yok; şeffaf PNG |
| `Assets/Art/UI/Task5/primary-normal.png` | 888×138 | 32 | L/B/R/T 33/33/33/33 px; #20–25 ortak düğme |
| `Assets/Art/UI/Task5/primary-pressed.png` | 888×138 | 32 | L/B/R/T 33/33/33/33 px; #20–25 ortak düğme |
| `Assets/Art/UI/Task5/secondary-normal.png` | 888×138 | 33 | L/B/R/T 33/33/33/33 px; #20–25 ortak düğme |
| `Assets/Art/UI/Task5/secondary-pressed.png` | 888×138 | 33 | L/B/R/T 33/33/33/33 px; #20–25 ortak düğme |
| `Assets/Art/UI/Task5/close-normal.png` | 108×108 | 34 | yok; şeffaf PNG |
| `Assets/Art/UI/Task5/close-pressed.png` | 108×108 | 34 | yok; şeffaf PNG |
| `Assets/Art/UI/Task5/popup-card.png` | 1020×720 | 35 | L/B/R/T 54/54/54/54 px; #20–25 ortak kart; yükseklik dilimlenir |
| `Assets/Art/UI/Task5/icon-tile.png` | 156×156 | 37 | yok; şeffaf PNG |
| `Assets/Art/Gameplay/Task5/board-tray.png` | 1020×1020 | 38 | L/B/R/T 60/60/60/60 px; şeffaf PNG |
| `Assets/Art/Gameplay/Task5/cell-slot.png` | 228×228 | 39 | yok; şeffaf PNG |
| `Assets/Art/Gameplay/Task5/wall-repeat.png` | 96×18 | 40 | yok; tile x; 96 px periyot; karşı kenarlar eşit; uç kapak yok |
| `Assets/Art/Gameplay/Task5/chain-cable.png` | 96×34 | 41 | yok; tile x; şeffaf PNG |
| `Assets/Art/Gameplay/Task5/chain-link.png` | 62×40 | 41 | yok; şeffaf PNG |
| `Assets/Art/Gameplay/Task5/chain-disc-1.png` | 46×46 | 41 | yok; ChainView.Tints #ffcc4d |
| `Assets/Art/Gameplay/Task5/chain-disc-2.png` | 46×46 | 41 | yok; ChainView.Tints #5fe0d0 |
| `Assets/Art/Gameplay/Task5/chain-disc-3.png` | 46×46 | 41 | yok; ChainView.Tints #ff8fb8 |
| `Assets/Art/Gameplay/Task5/chain-disc-4.png` | 46×46 | 41 | yok; ChainView.Tints #9ecbff |
| `Assets/Art/Gameplay/Task5/nail-counter.png` | 51×51 | 42 | yok; şeffaf PNG |
| `Assets/Art/Gameplay/Task5/bomb-counter-normal.png` | 51×51 | 43 | yok; şeffaf PNG |
| `Assets/Art/Gameplay/Task5/bomb-counter-hot.png` | 51×51 | 43 | yok; şeffaf PNG |
| `Assets/Art/UI/Task5/star-full.png` | 84×84 | 44 | yok; şeffaf PNG |
| `Assets/Art/UI/Task5/star-empty.png` | 84×84 | 44 | yok; şeffaf PNG |
| `Assets/Art/UI/Task5/star-filling-glow.png` | 84×84 | 44 | yok; Dolumda alpha/scale ile kullanılır |
| `Assets/Art/UI/Task5/progress-track.png` | 540×18 | 45 | L/B/R/T 9/0/9/0 px; Dolgu maskeyle kırpılır; #25 ile ortak |
| `Assets/Art/UI/Task5/progress-fill.png` | 540×18 | 45 | L/B/R/T 9/0/9/0 px; Dolgu maskeyle kırpılır; #25 ile ortak |
| `Assets/Art/UI/Task5/toast.png` | 600×84 | 46 | L/B/R/T 42/18/42/18 px; şeffaf PNG |
| `Assets/Art/UI/Task5/level-cell-open.png` | 146×146 | 47 | yok; Sayı dinamik; #20 ile ortak |
| `Assets/Art/UI/Task5/level-cell-locked.png` | 146×146 | 47 | yok; Sayı dinamik; #20 ile ortak |
| `Assets/Art/UI/Task5/level-cell-complete.png` | 146×146 | 47 | yok; Sayı dinamik; #20 ile ortak |
| `Assets/Art/UI/Task5/level-cell-next.png` | 146×146 | 47 | yok; Sayı dinamik; #20 ile ortak |
| `Assets/Art/Gameplay/Task5/tutorial-target-glow.png` | 228×228 | 48 | L/B/R/T 42/42/42/42 px; şeffaf PNG |
| `Assets/Art/Gameplay/Task5/icon-piece.png` | 48×48 | 5b | yok; şeffaf PNG |
| `Assets/Art/Gameplay/Task5/icon-clock.png` | 48×48 | 5b | yok; şeffaf PNG |
| `Assets/Art/Gameplay/Task5/icon-reset.png` | 51×51 | 5b | yok; şeffaf PNG |
| `Assets/Art/Gameplay/Task5/icon-lock.png` | 42×42 | 5b | yok; şeffaf PNG |
| `Assets/Art/Gameplay/Task5/icon-reset-pressed.png` | 51×51 | 5b | yok; şeffaf PNG |
| `Assets/Art/UI/Task5/icon-lock-card.png` | 90×90 | 5b | yok; şeffaf PNG |
| `Assets/Art/UI/Task5/result-picture-frame.png` | 888×592 | 5b | L/B/R/T 18/18/18/18 px; Merkez tamamen şeffaf |
| `Assets/Art/UI/Task5/rating-normal.png` | 78×108 | 5b | L/B/R/T 24/24/24/24 px; 1–10 sayıları oyun yazar; tek ortak zemin |
| `Assets/Art/UI/Task5/rating-selected.png` | 78×108 | 5b | L/B/R/T 24/24/24/24 px; 1–10 sayıları oyun yazar; tek ortak zemin |
| `Assets/Art/UI/Task5/rating-pressed.png` | 78×108 | 5b | L/B/R/T 24/24/24/24 px; 1–10 sayıları oyun yazar; tek ortak zemin |
| `Assets/Art/UI/Task5/thumb-up-normal.png` | 138×138 | 5b | yok; İkincil düğme ailesi |
| `Assets/Art/UI/Task5/thumb-up-selected.png` | 138×138 | 5b | yok; İkincil düğme ailesi |
| `Assets/Art/UI/Task5/thumb-up-pressed.png` | 138×138 | 5b | yok; İkincil düğme ailesi |
| `Assets/Art/UI/Task5/thumb-down-normal.png` | 138×138 | 5b | yok; İkincil düğme ailesi |
| `Assets/Art/UI/Task5/thumb-down-selected.png` | 138×138 | 5b | yok; İkincil düğme ailesi |
| `Assets/Art/UI/Task5/thumb-down-pressed.png` | 138×138 | 5b | yok; İkincil düğme ailesi |
| `Assets/Art/Gameplay/Task5/bomb-glow-normal.png` | 72×72 | 5b | yok; Nabız alpha/scale; ikon ayrı katman |
| `Assets/Art/Gameplay/Task5/bomb-glow-hot.png` | 72×72 | 5b | yok; Nabız alpha/scale; ikon ayrı katman |
| `Assets/Art/UI/Task5/rotate-phone.png` | 168×168 | 5b | yok; şeffaf PNG |
| `Assets/Art/UI/Task5/name-input-normal.png` | 888×138 | 5b | L/B/R/T 33/33/33/33 px; şeffaf PNG |
| `Assets/Art/UI/Task5/name-input-focus.png` | 888×138 | 5b | L/B/R/T 33/33/33/33 px; şeffaf PNG |
| `Assets/Art/UI/Task5/profile-normal.png` | 480×156 | 5b | L/B/R/T 156/36/36/36 px; Köşe 36 px; soldaki avatarı korumak için sol sabit bölge 156 px; yazı x≥162 |
| `Assets/Art/UI/Task5/profile-pressed.png` | 480×156 | 5b | L/B/R/T 156/36/36/36 px; Köşe 36 px; soldaki avatarı korumak için sol sabit bölge 156 px; yazı x≥162 |
| `Assets/Art/UI/Task5/settings-normal.png` | 138×138 | 5b | yok; şeffaf PNG |
| `Assets/Art/UI/Task5/settings-pressed.png` | 138×138 | 5b | yok; şeffaf PNG |
| `Assets/Art/UI/Task5/settings-row-on.png` | 888×150 | 5b | L/B/R/T 30/30/30/30 px; İkon ve switch ayrı katman |
| `Assets/Art/UI/Task5/switch-on.png` | 156×90 | 5b | yok; şeffaf PNG |
| `Assets/Art/UI/Task5/sound-on.png` | 90×90 | 5b | yok; şeffaf PNG |
| `Assets/Art/UI/Task5/settings-row-off.png` | 888×150 | 5b | L/B/R/T 30/30/30/30 px; İkon ve switch ayrı katman |
| `Assets/Art/UI/Task5/switch-off.png` | 156×90 | 5b | yok; şeffaf PNG |
| `Assets/Art/UI/Task5/sound-off.png` | 90×90 | 5b | yok; şeffaf PNG |
| `Assets/Art/UI/Task5/level-header-normal.png` | 888×180 | 5b | L/B/R/T 60/60/60/60 px; şeffaf PNG |
| `Assets/Art/UI/Task5/level-header-hard.png` | 888×180 | 5b | L/B/R/T 60/60/60/60 px; şeffaf PNG |
| `Assets/Art/UI/Task5/goal-jiggle.png` | 120×120 | 5b | yok; şeffaf PNG |
| `Assets/Art/UI/Task5/confetti-1.png` | 36×36 | 5b | yok; şeffaf PNG |
| `Assets/Art/UI/Task5/confetti-2.png` | 36×36 | 5b | yok; şeffaf PNG |
| `Assets/Art/UI/Task5/confetti-3.png` | 36×36 | 5b | yok; şeffaf PNG |
| `Assets/Art/UI/Task5/confetti-4.png` | 36×36 | 5b | yok; şeffaf PNG |
| `Assets/Art/UI/Task5/confetti-5.png` | 36×36 | 5b | yok; şeffaf PNG |
| `Assets/Art/UI/Task5/confetti-6.png` | 36×36 | 5b | yok; şeffaf PNG |
| `Assets/Art/UI/Task5/star-burst.png` | 252×252 | 5b | yok; şeffaf PNG |
| `Assets/Art/UI/Task5/play-normal.png` | 600×216 | 5b | L/B/R/T 72/72/72/72 px; Görev 4 onaylı kaynak SVG |
| `Assets/Art/UI/Task5/play-pressed.png` | 600×216 | 5b | L/B/R/T 72/72/72/72 px; Görev 4 onaylı kaynak SVG |
| `Assets/Art/Gameplay/Task5/game-background.png` | 1125×2001 | 26 | yok; Görev 4 sahnesi merkezden 1125×2001 kırpılır; tam ekran opak |
| `Assets/Art/UI/Task5/hand-tap.png` | 288×288 | tutorial | yok; Parmak ucu/pivot üst soldan (151,16) px |
| `Assets/Art/UI/Task5/hand-swipe.png` | 288×288 | tutorial | yok; Parmak ucu/pivot üst soldan (136,15) px |
| `Assets/Art/UI/Task5/avatar.png` | 132×132 | 5b | yok; Onaylı ikon referansından üretilen turkuaz Jiggle yüzü; köşe 27 px |

#36 veil: PNG gerekmez; renk `#19252D`, opaklık `0.90`. #49: `Assets/Art/PieceSkin/skin.json` + `body-256.png` değişmeden korunur; gölge, seçili/parça sürükleme bu skin sözleşmesinden uygulanır.

Yazı tipi önerisi: `Nunito[wght].ttf` — normal 400 / kalın 700 — SIL Open Font License 1.1, Türkçe Latin Extended desteği. Yalnız dosya adı/lisans önerisidir; font yüklenmedi. Kaynak: [Google Fonts dosya/Latin Extended bilgisi](https://github.com/google/fonts/blob/main/ofl/nunito/METADATA.pb), [SIL OFL 1.1](https://github.com/googlefonts/nunito/blob/main/OFL.txt).

## Görev 6 PNG teslimi

İnceleme: Assets/Art/Gameplay/Task6/review.html. Manifest Görev 5 biçimindedir; @3x, merkez pivot (0.5,0.5), tüm slice alanları null. Yazı/sayı yok.

| Dosya | Ölçü | 9-slice / not |
|---|---|---|
| Assets/Art/Gameplay/Task6/booster-active.png | 180×180 | yok; Görev 5 normal yuva geometrisi, altın kenar ve iç ışıma; booster ikonu ayrı katman |
| Assets/Art/Gameplay/Task6/joint-target.png | 66×66 | yok; dolu turuncu/altın hedef, merkez (33,33) px |
| Assets/Art/Gameplay/Task6/joint-target-pulse.png | 66×66 | yok; aynı pivot, daha büyük/parlak gövde; normal ile dönüşümlü kullanılabilir |
| Assets/Art/Gameplay/Task6/loader-bg.png | 750×1334 | yok; opak, Görev 5 şato sahnesinden bulanık/koyu türev; 250 KB altında |

Kontroller: docs/art/reports/task6-validation.json. İlk üç PNG şeffaf; arka plan tam opak. Unity entegrasyonu ve push/deploy yapılmadı.
Yükleme arka planı: 206.176 bayt; renk azaltmadan kayıpsız PNG sıkıştırması. Yeniden üretim: `node docs/art/build-task6.cjs`; doğrulama: `powershell -NoProfile -ExecutionPolicy Bypass -File docs/art/verify-task6.ps1`.

## Oyun içi görsel revizyonu — 25 Ekim

- `Assets/Art/Gameplay/Task6/nail-head-384.png` / tahta üstten görünüş çivi başı / 384×384 şeffaf PNG / düz altın merkez alanı hamle sayısı için boş; rakam çizilmedi.
- `Assets/Art/Gameplay/Task6/bomb-counter-384.png` / iri bomba / 384×384 şeffaf PNG / kısa fitil, açık krem boş etiket alanı; rakam çizilmedi.
- `Assets/Art/Gameplay/Task6/chain-link-repeat-96x48.png` / zincir halkası / 96×48 şeffaf PNG / yatay dizilimde halkalar 48 px adımla bindirilir.
- `Assets/Art/Gameplay/Task6/chain-endpoint-144.png` / zincir uç kilit/halka diski / 144×144 şeffaf PNG / mevcut ölçü yönü korundu.
- `Assets/Art/Gameplay/Task6/wall-repeat-96x48.png` / duvar uyarı şeridi / 96×48 şeffaf PNG / kırmızı-beyaz çapraz; yatay tekrar.
- `Assets/Art/Gameplay/Task6/wall-corner-96x96.png` / şerit köşesi / 96×96 şeffaf PNG / L dönüş.
- `Assets/Art/PieceSkin/Task6/piece-sample-384.png` / yapboz görünüm örneği / 384×384 şeffaf PNG / üst merkezde çıkıntı, diğer kenarlar düz; tablo resmi, koyu kontur, bevel ve gölge.
- `Assets/Art/PieceSkin/Task6/sample-review-v01.html` / aynı örnek parçanın 3×3 materyal önizlemesi / 81 kenar kombinasyonu değildir.
- 4c için şekil maskesi/overlay/düz dolgu üçlüsü ve 81 kombinasyon üretimi Zeyd’in örnek onayına kadar beklemede.
- `Assets/Art/UI/Task6/pause-button-168.png` / oyun içi pause düğmesi / 168×168 şeffaf PNG.
- `Assets/Art/UI/Task6/pause-card-1020x1080.png` / pause kart zemini / 1020×1080 şeffaf PNG / mevcut popup kartından 60 px çerçeve köşeleri korunup 9-bölge yöntemiyle uzatıldı.
- Pause düzeni `Assets/Art/UI/Task6/pause-review-v01.html` içinde: Devam, Yeniden başla, Ana menü metinleri ayrı katman; mevcut genel `UI/Task5/primary-normal.png`, `secondary-normal.png` düğmeleri tekrar kullanılır.
- Toplu inceleme: `Assets/Art/Gameplay/Task6/redraw-review-v01.html`.

## Yapboz parçası 2×2 demo — 25 Ekim

- `Assets/Art/Review/Pieces/demo-v01.png` / turkuaz–mercan–altın yüzeyli 2×2 geçmeli set / 1152×1152 px, şeffaf PNG / referans geometrisine göre kare gövde, yuvarlak merkez tırnakları, koyu kontur ve yumuşak bevel.
- `Assets/Art/Review/Pieces/demo-v01-painted.png` / aynı setin örnek tablo resmi kaplanmış hali / 1152×1152 px, şeffaf PNG / resim tek parça gibi hizalı; kontur, kabartma ve gölgeler görünür.
- 81 kenar kombinasyonu ve mask/overlay/fill çıktıları Zeyd onayı gelene kadar üretilmeyecek.
- `Assets/Art/Review/Pieces/demo-v02-centered.png` / IMG_2392.PNG’de işaretlenen aynı 2×2 demo / 1152×1152 şeffaf PNG / üst dikey, sağ sütundaki yatay ve alt dikey bağlantı kenar ortasına alındı; diğer çizim pikselleri korunur.
- `Assets/Art/Review/Pieces/demo-v02-centered-review.html` / v01-v02 karşılaştırma sayfası / yalnız inceleme; 81 kombinasyon üretilmedi.
- Yeniden üretim: `powershell -NoProfile -ExecutionPolicy Bypass -File Assets/Art/Review/Pieces/recenter-connections.ps1`.
- `Assets/Art/PieceSkin/Task6/piece_O-I-F-F.png` / tek 4c onay parçası / 384×384 şeffaf PNG / üst O çıkıntısı ve sağ I girintisi kendi kenarlarının tam ortasında; alt/sol F düz.
- `Assets/Art/PieceSkin/Task6/piece_O-I-F-F-review.html` / merkez çizgileri açılıp kapanabilen tek-parça önizlemesi / 81 kombinasyon Zeyd bu örneği onaylayana kadar beklemede.
- Yeniden üretim: `powershell -NoProfile -ExecutionPolicy Bypass -File Assets/Art/PieceSkin/Task6/build-one-piece-demo.ps1`.

## Sonsuz koridor geçiş demosu — 25 Ekim

İlk sanat ve tarayıcı prototipi hazır. Bölme frontal/ortografik; 1125×2436 px (@3x) baz alınır. Demo 390×844 telefon çerçevesinde 1 sn bekler, 1,5 sn ease-in-out ile bir tam bölme sola kayar, sonraki tabloyu gösterir ve döngüyü sürdürür. Mod A düz kayış; Mod B silindir kıvrımı kaydırıcılıdır. Oyuna entegrasyon yapılmadı, Zeyd incelemesi bekleniyor.

| Dosya | Ölçü | Yatay tekrar | Paralaks / açıklama |
|---|---:|---|---|
| `Assets/Art/Menu/InfiniteCorridor/wall-tile-v01.png` | 1125×2436 | X; sol/sağ sınır RGB farkı 0 | 1.0×; opak teal duvar alanı |
| `Assets/Art/Menu/InfiniteCorridor/frame-empty-v01.png` | 1125×2436; çerçeve 981×651, (72,890) | Bölme ile X | 1.0×; alfa şeffaf boş çerçeve |
| `Assets/Art/Menu/InfiniteCorridor/painting-level-01-v01.png` | 716×330 | Bölme başına değişir | 1.0×; tablo iç ölçüsü |
| `Assets/Art/Menu/InfiniteCorridor/painting-level-02-v01.png` | 716×330 | Bölme başına değişir | 1.0×; örnek sonraki bölüm resmi |
| `Assets/Art/Menu/InfiniteCorridor/foreground-columns-v01.png` | 1125×2436 | X; alfa kenarları eşlenik | 1.4×; sütun, sancak ve aplikler, şeffaf orta alan |
| `Assets/Art/Menu/InfiniteCorridor/floor-strip-v01.png` | 1125×600 | X; dikiş kontrollü | 1.0×; şeffaf çevreli, UI altındaki bordo şerit |
| `Assets/Art/Menu/InfiniteCorridor/tile-repeat-proof-v01.png` | 3375×2436 | Üç döşeme yan yana | Duvar, sütun, çerçeve, tablo ve zeminle birleşim kanıtı |
| `Assets/Art/Menu/InfiniteCorridor/demo-preview-v01.png` | 780×1688 | yok; 390×844 önizleme @2x | Durağan kompozit ekran görüntüsü |
| `Assets/Art/Menu/InfiniteCorridor/index.html` | 390×844 sahne | canlı döngü | Mod A/B, 1 sn + 1,5 sn, kıvrım kaydırıcısı, tekrar oynat |
| `Assets/Art/Menu/InfiniteCorridor/build-demo.ps1` | kaynak oluşturucu | — | Katman ve kanıt PNG’lerini yeniden üretir |

Üretim kaynakları `Assets/Art/Menu/InfiniteCorridor/source/` içindedir. Önizleme ve katmanlar Zeyd’in değerlendirmesi için; görsel onayı ve oyun bağlantısı beklemede.
| `Assets/Art/Gameplay/Task6/nail-head-384.png` | Tahtadaki çivi (oyunda `board-nail`) | 384×384 şeffaf | Hücrenin %80'i; sayaç çivi başının ortasında |
| `Assets/Art/Gameplay/Task6/bomb-counter-384.png` | Tahtadaki bomba (oyunda `board-bomb`) | 384×384 şeffaf | Hücrenin %80'i; sayaç ortadaki krem dairede |
| `Assets/Art/Gameplay/Task6/chain-link-repeat-96x48.png` | Tahtadaki zincir şeridi (oyunda `chain-repeat`) | 96×48, yatay tekrar | Kalınlık hücrenin 0,24'ü; iki hücre arasında döndürülür |
| `Assets/Art/Gameplay/Task6/chain-endpoint-144.png` | Zincir uç kilidi (oyunda `chain-end`) | 144×144 şeffaf | Her uçta hücrenin 0,5'i |
| `Assets/Art/Gameplay/Task6/wall-repeat-96x48.png` | Duvar şeridi (oyunda `wall-stripe`) | 96×48, yatay tekrar | Yükseklik = duvar kalınlığı; sol/sağ kenarda 90° döner |
| `Assets/Art/Gameplay/Task6/wall-corner-96x96.png` | Duvar köşesi (oyunda `wall-corner`) | 96×96 şeffaf | İki kapalı kenarın buluştuğu tahta köşesinde |

> Not (Claude Code): yeni oyun görselleri `Tools/art_choice.py` içindeki ad eşlemesine eklenmeden oyuna girmez; yeni dosyaya bu tabloya "oyunda `ad`" ile satır yazın.

## Task7 — oyun içi görseller v2

| Dosya | Oyunda nereye | Boyut |
|---|---|---:|
| `Assets/Art/Gameplay/Task7/bomb-v2-384.png` | Tahta bomba gövdesi ve hamle sayacı zemini | 384×384 şeffaf PNG |
| `Assets/Art/Gameplay/Task7/nail-head-v2-384.png` | Tahta çivi başı ve hamle sayacı zemini | 384×384 şeffaf PNG |
| `Assets/Art/Gameplay/Task7/wall-repeat-v2-96x96.png` | Tahta kenar bariyeri, tekrarlanabilir şerit | 96×96 şeffaf PNG; yatay tekrar |
| `Assets/Art/Gameplay/Task7/wall-corner-v2-96x96.png` | Tahta kenar bariyeri köşesi | 96×96 şeffaf PNG |
| `Assets/Art/Gameplay/Task7/board-frame-gold-v1.png` | Oyun tahtasını çevreleyen altın tablo çerçevesi | 1125×1125 şeffaf PNG; 9-slice köşe ≥90 px |
| `Assets/Art/Gameplay/Task7/hud-level-badge.png` | Üst barda bölüm rozeti; LV ve numara yazısı ayrı katman | 216×216 şeffaf PNG |
| `Assets/Art/Gameplay/Task7/hud-pill.png` | Üst bar hamle/süre kutusu zemini | 600×168 şeffaf PNG; 9-slice |
| `Assets/Art/Gameplay/Task7/hud-pill-urgent.png` | Süre azaldığında üst bar kutusu | 600×168 şeffaf PNG; 9-slice |
| `Assets/Art/Gameplay/Task7/hud-icon-piece.png` | Hamle göstergesi yapboz parçası ikonu | 96×96 şeffaf PNG |
| `Assets/Art/Gameplay/Task7/hud-icon-clock.png` | Süre göstergesi saat ikonu | 96×96 şeffaf PNG |
| `Assets/Art/Gameplay/Task7/hud-pause-normal.png` | Normal durumdaki oyun içi durdur tuşu | 168×168 şeffaf PNG |

| `Assets/Art/Gameplay/Task7/hud-pause-pressed.png` | Basılı durumdaki oyun içi durdur tuşu | 168×168 şeffaf PNG || (bekleniyor) `Assets/Art/Gameplay/Task6/bomb-v2-384.png` | Bomba v2 (oyunda `bomb-v2`) | 384×384, gövde tuvalin ortasında | Gövde hücrenin %85'i; sayaç krem dairede |

### Task7 oyundaki adları (Claude Code, 26 Ekim) — `Tools/art_choice.py` bu adlarla kopyalar
| Dosya | Oyunda | Kullanım |
|---|---|---|
| `bomb-v2-384.png` | `bomb-v2` | Gövde (tuvalin %71'i, ortada) hücrenin %85'i; sayaç krem dairede |
| `nail-head-v2-384.png` | `board-nail` | Hücrenin %80'i; sayaç ortada |
| `wall-repeat-v2-96x96.png` | `wall-stripe` | Yatay tekrar; şerit (11–85 px) duvar kalınlığına (hücrenin %18'i) ölçeklenir |
| `wall-corner-v2-96x96.png` | `wall-corner` | İki kapalı kenarın buluştuğu köşe |
| `board-frame-gold-v1.png` | `board-frame` | Tahta tepsisi, 9-slice 90 px; geçici altın tint kalktı |
| `hud-level-badge.png` | `level-badge` | Üst bar bölüm rozeti |
| `hud-pill.png`, `hud-pill-urgent.png` | `hud-pill-normal`, `hud-pill-urgent` | 9-slice 84/40/84/40 px; süre ≤ 20 sn'de urgent |
| `hud-icon-piece.png`, `hud-icon-clock.png` | `icon-piece`, `hud-icon-clock` | Üst bar ikonları |
| `hud-pause-normal.png`, `hud-pause-pressed.png` | `pause-normal`, `pause-pressed` | Durdur tuşu (basılı hal henüz kullanılmıyor) |
