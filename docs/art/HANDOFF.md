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

## Task8 — booster çubuğu ve üst bar v3

| Dosya | Oyunda nereye | Boyut |
|---|---|---:|
| `Assets/Art/Gameplay/Task8/booster-button.png` | Booster çubuğundaki boş yuvarlak tuş zemini | 264×264 şeffaf PNG |
| `Assets/Art/Gameplay/Task8/booster-scissors.png` | Makas booster tuşunun büyük merkez ikonu | 240×240 şeffaf PNG |
| `Assets/Art/Gameplay/Task8/booster-wand.png` | Asa booster tuşunun büyük merkez ikonu | 240×240 şeffaf PNG |
| `Assets/Art/Gameplay/Task8/booster-hammer.png` | Çekiç booster tuşunun büyük merkez ikonu | 240×240 şeffaf PNG |
| `Assets/Art/Gameplay/Task8/booster-clock.png` | Saat booster tuşunun büyük merkez ikonu | 240×240 şeffaf PNG |
| `Assets/Art/Gameplay/Task8/booster-badge.png` | Booster tuşunun sağ altındaki sayı rozeti zemini | 96×96 şeffaf PNG |
| `Assets/Art/Gameplay/Task8/hud-level-badge-v3.png` | Üst barda LV etiketi ve bölüm numarası rozeti | 216×216 şeffaf PNG |
| `Assets/Art/Gameplay/Task8/hud-pill-v3.png` | Üst bar süre/hamle alanı; ikon yüzeye doğrudan yerleşir | 600×168 şeffaf PNG; 9-slice köşe ~48 px |
| `docs/art/Task8-review.png` | Booster ve üst bar birleşik görsel incelemesi | 1420×760 PNG |

Önizleme: `docs/art/Task8-review.png`. Booster butonu, ikon ve sayı rozeti ayrı katmanlardır; üst bar hapında sol ikon yuvası yoktur.
## Task10 — referans stili booster seti

| Dosya | Oyunda nereye | Boyut |
|---|---|---:|
| `Assets/Art/Gameplay/Task10/booster-button.png` | Booster tuşlarının boş, parlak turuncu-altın halkalı turkuaz zemini | 264×264 şeffaf PNG |
| `Assets/Art/Gameplay/Task10/booster-scissors.png` | Makas tuşunun iri mercan saplı, açık duran makas ikonu | 240×240 şeffaf PNG |
| `Assets/Art/Gameplay/Task10/booster-wand.png` | Değnek tuşunun mor saplı, yıldız uçlu ikonu | 240×240 şeffaf PNG |
| `Assets/Art/Gameplay/Task10/booster-hammer.png` | Çekiç tuşunun kırmızı tombul başlı, kısa ahşap saplı ikonu | 240×240 şeffaf PNG |
| `Assets/Art/Gameplay/Task10/booster-clock.png` | Saat tuşunun turuncu çift zilli, beyaz kadranlı ikonu | 240×240 şeffaf PNG |
| `Assets/Art/Gameplay/Task10/booster-badge.png` | Booster tuşunun sağ altındaki sayı rozeti zemini | 96×96 şeffaf PNG |
| `Assets/Art/Gameplay/Task10/preview.png` | Dört tamamlanmış booster tuşunun yan yana önizlemesi | 1240×340 PNG |

Task9 iptal edildi; klasör ve HANDOFF kaydı kaldırıldı. Task10 görünümü, `Assets/Art/Reference/booster-bar-layout-reference.jpg` ile kompozisyon ve parlak oyuncak malzeme dili açısından karşılaştırıldı; ikon biçimleri ve renkleri özgün tutuldu.
## Task11 — oyun arayüzü oyuncak stili

| Dosya | Oyunda nereye | Boyut |
|---|---|---:|
| `Assets/Art/Gameplay/Task11/hud-level-badge.png` | Üst bar bölüm rozeti; üst LV ve alt sayı yazı alanları ayrı katman | 216×216 şeffaf PNG |
| `Assets/Art/Gameplay/Task11/hud-pill.png` | Üst bar hamle/süre zemini; ikon yüzeye doğrudan konur | 600×168 şeffaf PNG; 9-slice köşe 72 px |
| `Assets/Art/Gameplay/Task11/hud-pill-urgent.png` | Süre azalınca kırmızımsı üst bar zemini | 600×168 şeffaf PNG; 9-slice köşe 72 px |
| `Assets/Art/Gameplay/Task11/hud-icon-piece.png` | Üst barda hamle göstergesi yapboz ikonu | 120×120 şeffaf PNG |
| `Assets/Art/Gameplay/Task11/hud-icon-clock.png` | Üst barda süre göstergesi saat ikonu | 120×120 şeffaf PNG |
| `Assets/Art/Gameplay/Task11/menu-play-button.png` | Menüde Level X yazısı eklenecek ana başlat tuşu | 600×216 şeffaf PNG; 9-slice köşe 72 px |
| `Assets/Art/Gameplay/Task11/menu-play-button-pressed.png` | Menü başlat tuşunun basılı hali | 600×216 şeffaf PNG; 9-slice köşe 72 px |
| `Assets/Art/Gameplay/Task11/btn-primary.png` | Pause/kazan/kaybet kartlarında ana işlem tuşu | 600×168 şeffaf PNG; 9-slice köşe 72 px |
| `Assets/Art/Gameplay/Task11/btn-primary-pressed.png` | Ana işlem tuşunun basılı hali | 600×168 şeffaf PNG; 9-slice köşe 72 px |
| `Assets/Art/Gameplay/Task11/btn-secondary.png` | Kartlarda ikincil işlem tuşu | 600×168 şeffaf PNG; 9-slice köşe 72 px |
| `Assets/Art/Gameplay/Task11/btn-secondary-pressed.png` | İkincil işlem tuşunun basılı hali | 600×168 şeffaf PNG; 9-slice köşe 72 px |
| `Assets/Art/Gameplay/Task11/btn-danger.png` | Kartlarda çıkış/tekrar gibi tehlikeli işlem tuşu | 600×168 şeffaf PNG; 9-slice köşe 72 px |
| `Assets/Art/Gameplay/Task11/btn-danger-pressed.png` | Tehlikeli işlem tuşunun basılı hali | 600×168 şeffaf PNG; 9-slice köşe 72 px |
| `Assets/Art/Gameplay/Task11/card-panel.png` | Pause/kazan/kaybet kartlarının krem zemin paneli | 969×1200 şeffaf PNG; 9-slice köşe 96 px |
| `Assets/Art/Gameplay/Task11/star-full.png` | Kazanma kartında kazanılmış yıldız | 180×180 şeffaf PNG |
| `Assets/Art/Gameplay/Task11/star-empty.png` | Kazanma kartında boş yıldız | 180×180 şeffaf PNG |
| `Assets/Art/Gameplay/Task11/preview-hud.png` | HUD rozeti, haplar ve ikonlar grup önizlemesi | 1420×390 PNG |
| `Assets/Art/Gameplay/Task11/preview-menu.png` | Menü başlat tuşu normal/basılı önizlemesi | 1240×290 PNG |
| `Assets/Art/Gameplay/Task11/preview-card-buttons.png` | Kart tuşları ve basılı hallerinin önizlemesi | 1500×570 PNG |
| `Assets/Art/Gameplay/Task11/preview-card-panel.png` | Kart zemini önizlemesi | 600×690 PNG |
| `Assets/Art/Gameplay/Task11/preview-stars.png` | Dolu/boş yıldız önizlemesi | 600×270 PNG |

Dokuz dilim kenar payı: hap ve kart tuşlarında 72 px; kart panelinde 96 px. Tüm yazı alanları boş bırakılmıştır.
## Task12 — koridor ana menü UI

| Dosya | Oyunda nereye | Boyut |
|---|---|---:|
| `Assets/Art/Menu/Task12/menu-level-button-v01.png` | Koridor ana menüsündeki Level X başlat tuşu | 600×216 şeffaf PNG |
| `Assets/Art/Menu/Task12/menu-level-button-pressed-v01.png` | Level X tuşunun basılı durumu | 600×216 şeffaf PNG |
| `Assets/Art/Menu/Task12/menu-profile-button-v01.png` | Sol üst oyuncu profili; Jiggles portresi | 216×216 şeffaf PNG |
| `Assets/Art/Menu/Task12/menu-resource-pill-v01.png` | Üst sıradaki para/enerji/yıldız sayaç zemini; tekrarlanabilir | 360×108 şeffaf PNG; 9-slice |
| `Assets/Art/Menu/Task12/menu-token-coin-v01.png` | Para göstergesinin ayrı altın token ikonu | 112×112 şeffaf PNG |
| `Assets/Art/Menu/Task12/menu-token-heart-v01.png` | Enerji göstergesinin ayrı kalp ikonu | 112×112 şeffaf PNG |
| `Assets/Art/Menu/Task12/menu-token-star-v01.png` | Yıldız/puan göstergesinin ayrı ikonu | 112×112 şeffaf PNG |
| `Assets/Art/Menu/Task12/menu-settings-button-v01.png` | Sağ üst ayarlar tuşu | 168×168 şeffaf PNG |
| `Assets/Art/Menu/Task12/menu-bottom-nav-v01.png` | Ana menü alt gezinme şeridi | 1170×240 şeffaf PNG; 9-slice |
| `Assets/Art/Menu/Task12/menu-nav-home-v01.png` | Alt gezinme ana sayfa ikonu | 120×120 şeffaf PNG |
| `Assets/Art/Menu/Task12/menu-nav-album-v01.png` | Alt gezinme tablo/album ikonu | 120×120 şeffaf PNG |
| `Assets/Art/Menu/Task12/menu-nav-friends-v01.png` | Alt gezinme Jiggles/kurtarma ikonu | 120×120 şeffaf PNG |
| `Assets/Art/Menu/Task12/menu-nav-trophy-v01.png` | Alt gezinme kupa/başarı ikonu | 120×120 şeffaf PNG |
| `Assets/Art/Menu/Task12/menu-preview-v01.png` | Koridor üstüne yerleştirilmiş ana menü düzeni; örnek sayaç değerleri | 390×844 PNG |

Menü önizlemesinde seçili corridor-v04 arka planı korundu; profil portresinde onaylı Jiggles referansından yararlanıldı. Referans ekranlardaki varlıklar kopyalanmadı. 9-slice kaynaklarında resource pill için 42 px, alt nav için 76 px kenar payı kullanılabilir.

## Task12 — oyun UI zorunlu palet uyumu

| Dosya | Oyunda nereye | Boyut |
|---|---|---:|
| `Assets/Art/Gameplay/Task12/booster-button.png` | Booster tuşlarının ortak altın-teal yuvarlak zemini | 264×264 şeffaf PNG |
| `Assets/Art/Gameplay/Task12/booster-scissors.png` | Makas booster ikonu; ortak altın-krem-lacivert takım stili | 240×240 şeffaf PNG |
| `Assets/Art/Gameplay/Task12/booster-wand.png` | Değnek booster ikonu; ortak altın-krem-lacivert takım stili | 240×240 şeffaf PNG |
| `Assets/Art/Gameplay/Task12/booster-hammer.png` | Çekiç booster ikonu; ortak altın-krem-lacivert takım stili | 240×240 şeffaf PNG |
| `Assets/Art/Gameplay/Task12/booster-clock.png` | Saat booster ikonu; ortak altın-krem-lacivert takım stili | 240×240 şeffaf PNG |
| `Assets/Art/Gameplay/Task12/booster-badge.png` | Booster adet rozeti zemini; sayı ayrı katman | 96×96 şeffaf PNG |
| `Assets/Art/Gameplay/Task12/hud-level-badge.png` | Oyun üst barı bölüm rozeti | 216×216 şeffaf PNG |
| `Assets/Art/Gameplay/Task12/hud-pill.png` | Oyun üst barı hamle/süre zemini | 600×168 şeffaf PNG; 9-slice |
| `Assets/Art/Gameplay/Task12/hud-pill-urgent.png` | Süre azaldığında üst bar zemini | 600×168 şeffaf PNG; 9-slice |
| `Assets/Art/Gameplay/Task12/hud-icon-piece.png` | Üst barda hamle simgesi | 120×120 şeffaf PNG |
| `Assets/Art/Gameplay/Task12/hud-icon-clock.png` | Üst barda süre simgesi | 120×120 şeffaf PNG |
| `Assets/Art/Gameplay/Task12/menu-play-button.png` | Menü Level X başlat tuşu | 600×216 şeffaf PNG; 9-slice |
| `Assets/Art/Gameplay/Task12/menu-play-button-pressed.png` | Menü Level X başlat tuşunun basılı hali | 600×216 şeffaf PNG; 9-slice |
| `Assets/Art/Gameplay/Task12/btn-primary.png` | Kartlardaki ana işlem tuşu | 600×168 şeffaf PNG; 9-slice |
| `Assets/Art/Gameplay/Task12/btn-primary-pressed.png` | Ana işlem tuşunun basılı hali | 600×168 şeffaf PNG; 9-slice |
| `Assets/Art/Gameplay/Task12/btn-secondary.png` | Kartlardaki ikincil işlem tuşu | 600×168 şeffaf PNG; 9-slice |
| `Assets/Art/Gameplay/Task12/btn-secondary-pressed.png` | İkincil işlem tuşunun basılı hali | 600×168 şeffaf PNG; 9-slice |
| `Assets/Art/Gameplay/Task12/btn-danger.png` | Kartlardaki çıkış/tekrar tuşu | 600×168 şeffaf PNG; 9-slice |
| `Assets/Art/Gameplay/Task12/btn-danger-pressed.png` | Tehlikeli işlem tuşunun basılı hali | 600×168 şeffaf PNG; 9-slice |
| `Assets/Art/Gameplay/Task12/card-panel.png` | Pause/kazan/kaybet kart paneli | 969×1200 şeffaf PNG; 9-slice |
| `Assets/Art/Gameplay/Task12/star-full.png` | Kazanma kartı dolu yıldız | 180×180 şeffaf PNG |
| `Assets/Art/Gameplay/Task12/star-empty.png` | Kazanma kartı boş yıldız | 180×180 şeffaf PNG |
| `Assets/Art/Gameplay/Task12/board-frame-gold-v2.png` | Oyun tahtası çerçevesi; Task7 çerçevesinin palet uyumlu sürümü | 1125×1125 şeffaf PNG |
| `Assets/Art/Gameplay/Task12/preview.png` | Çerçeve, booster çubuğu ve HUD palet karşılaştırma sayfası | 1800×580 PNG |

Renk eşlemesi `docs/art/PALETTE.md` zorunlu altın, teal, krem, lacivert ve mercan tonlarına göre yapıldı. Booster ikonlarının vektör/çizim karakteri Task10 oyuncak stilinden korunup ortak altın-krem paletine toplandı.
## PlayButton v2 — referans uyumlu Level tuşu

| Dosya | Oyunda nereye | Boyut |
|---|---|---:|
| `Assets/Art/UI/PlayButton/menu-play-button-v2.png` | Menüde Level X başlat tuşu; yazısız kaynak, LilitaOne metni Unity'de eklenir | 684×336 şeffaf PNG (@3x; görünür gövde 660×300) |
| `Assets/Art/UI/PlayButton/menu-play-button-v2-pressed.png` | Başlat tuşunun basılı hali; gövde 4 px aşağıda ve koyulaştırılmış | 684×336 şeffaf PNG (@3x) |
| `Assets/Art/UI/PlayButton/preview-v2.png` | Referans ve “Level 7” yazılı örnek tuşun yan yana karşılaştırması | 1560×590 PNG |
| `Assets/Art/UI/PlayButton/Source/normal-v2.png` | Yeniden dışa aktarım için normal durum üretim kaynağı; oyuna doğrudan bağlanmaz | 1860×846 şeffaf PNG |
| `Assets/Art/UI/PlayButton/Source/pressed-v2.png` | Yeniden dışa aktarım için basılı durum üretim kaynağı; oyuna doğrudan bağlanmaz | 1859×846 şeffaf PNG |

9-slice: sol/sağ 92 px, üst/alt 92 px. Köşeleri sabit tutup yalnızca orta gövdeyi esnetin. Revizyon: önceki uzun/ince prosedürel çizim reddedildi; mevcut v2 referans görselinden image_gen ile yeniden oluşturuldu. Görünen oran yaklaşık 2,2:1. Önizleme metni referanstaki harflerden ölçülen krem (#F9F1BA), koyu yeşil konturlu ve gölgelidir; metin PNG kaynaklarına gömülü değildir. Referans önizlemede özgün 247×155 oranında gösterilir.
<!-- MENU_TASK13_START -->
## Task13 — onaylı Level tuşuyla aynı dilde ana menü

Onaylı Level normal/basılı dosyaları SHA-256 ile doğrulanan birebir kopyadır. Yeni çizimler Zeyd incelemesindedir. Tüm sprite PNG dosyalari ayrı @3x şeffaf PNG; dinamik yazı ve sayılar ayrı kalır. `manifest.json` ölçü, kaynak ve 9-slice için esas kayıttır. Source klasörü runtime atlasına alınmaz.

| Dosya | Oyunda nereye | Boyut / 9-slice |
|---|---|---|
| `Assets/Art/Menu/Task13/profile-frame.png` | Avatarın içi şeffaf altın-turkuaz çerçevesi | 216×216 |
| `Assets/Art/Menu/Task13/profile-avatar.png` | Onaylı Jiggle kimliğinde avatar portresi | 132×132 |
| `Assets/Art/Menu/Task13/counter-pill.png` | Ortak kaynak sayacı zemini | 360×108; L/R/T/B 44/44/44/44 |
| `Assets/Art/Menu/Task13/settings-button.png` | Ana menü ayarlar dişlisi | 168×168 |
| `Assets/Art/Menu/Task13/profile-box.png` | Profil adı ve kurtarılan sayısı zemini | 480×156; L/R/T/B 156/48/48/48 |
| `Assets/Art/Menu/Task13/token-coin.png` | Para göstergesi simgesi | 120×120 |
| `Assets/Art/Menu/Task13/token-heart.png` | Can göstergesi simgesi | 120×120 |
| `Assets/Art/Menu/Task13/token-star.png` | Yıldız göstergesi simgesi | 120×120 |
| `Assets/Art/Menu/Task13/token-jiggle.png` | Kurtarılan Jiggles göstergesi simgesi | 120×120 |
| `Assets/Art/Menu/Task13/nav-home.png` | Alt gezinmede şato/ana ekran | 180×180 |
| `Assets/Art/Menu/Task13/nav-collection.png` | Alt gezinmede tablo koleksiyonu | 180×180 |
| `Assets/Art/Menu/Task13/nav-jiggles.png` | Alt gezinmede kurtarılan Jiggles | 180×180 |
| `Assets/Art/Menu/Task13/nav-trophy.png` | Alt gezinmede başarılar | 180×180 |
| `Assets/Art/Menu/Task13/progress-track.png` | Kurtarılan hedefi ilerleme çubuğu boş izi | 1020×120; L/R/T/B 60/60/48/48 |
| `Assets/Art/Menu/Task13/progress-fill.png` | İlerleme dolgusu; oyun tarafından kırpılır | 996×72; L/R/T/B 36/36/30/30 |
| `Assets/Art/Menu/Task13/bottom-nav.png` | Alt gezinme şeridi; ikon ve etiketler ayrı | 1170×246; L/R/T/B 66/66/48/48 |
| `Assets/Art/Menu/Task13/nav-active.png` | Seçili alt sekme zemini | 252×222 |
| `Assets/Art/Menu/Task13/plus-button.png` | Küçük ekleme/artı kontrolü | 84×84 |
| `Assets/Art/Menu/Task13/notification-badge.png` | Bildirim sayısı rozeti; sayı ayrı | 72×72 |
| `Assets/Art/Menu/Task13/levels-button.png` | Bölüm listesi kısayolu | 192×192 |
| `Assets/Art/Menu/Task13/profile-box-pressed.png` | Profil kutusu basılı | 480×156; L/R/T/B 156/48/48/48 |
| `Assets/Art/Menu/Task13/settings-button-pressed.png` | Ayarlar dişlisi basılı | 168×168 |
| `Assets/Art/Menu/Task13/plus-button-pressed.png` | Artı kontrolü basılı | 84×84 |
| `Assets/Art/Menu/Task13/profile-frame-pressed.png` | Avatar çerçevesi basılı | 216×216 |
| `Assets/Art/Menu/Task13/profile-button.png` | Profil avatar düğmesi | 216×216 |
| `Assets/Art/Menu/Task13/profile-button-pressed.png` | Profil avatar düğmesi basılı | 216×216 |
| `Assets/Art/Menu/Task13/menu-play-button.png` | Onaylı Level tuşu | 684×336; L/R/T/B 92/92/92/92 |
| `Assets/Art/Menu/Task13/menu-play-button-pressed.png` | Onaylı Level tuşu basılı | 684×336; L/R/T/B 92/92/92/92 |
| `Assets/Art/Menu/Task13/preview-menu@3x.png` | Sanat inceleme önizlemesi; oyun sprite dosyası değildir | 1170×2532 |
| `Assets/Art/Menu/Task13/preview-menu.png` | Sanat inceleme önizlemesi; oyun sprite dosyası değildir | 390×844 |
| `Assets/Art/Menu/Task13/preview-header.png` | Sanat inceleme önizlemesi; oyun sprite dosyası değildir | 1500×490 |
| `Assets/Art/Menu/Task13/preview-nav.png` | Sanat inceleme önizlemesi; oyun sprite dosyası değildir | 1500×530 |
| `Assets/Art/Menu/Task13/preview-controls.png` | Sanat inceleme önizlemesi; oyun sprite dosyası değildir | 1500×820 |
| `Assets/Art/Menu/Task13/preview.png` | Sanat inceleme önizlemesi; oyun sprite dosyası değildir | 1800×1240 |
| `Assets/Art/Menu/Task13/index.html` | 390×844 telefon menüsü ve tüm sprite dosyalarının tek sayfalık incelemesi | HTML; yerel açılabilir |

İlerleme dolgusu maske/kırpma ile gösterilir; iki PNG üst üste yerleşir. Gösterge ikonları sayaç zemininin sol kenarına bindirilir. Profil portresi/çerçeve/kutu ayrı parçalar ve birleşik avatar düğmesi olarak teslim edilir. Önizlemedeki kullanıcı adı, sayaçlar ve alt gezinme etiketleri örnektir.
<!-- MENU_TASK13_END -->
## Task14 — onaylanan taslak (geçmiş kayıt)

| Dosya | Oyunda nereye | Boyut | 9-slice |
|---|---|---|---|
| `Assets/Art/Gameplay/Task14/preview-ingame.png` | Revizyon 2 tam ekran sanat taslağı: yeni booster/engel/bariyer; Zeyd onayı için, oyuna bağlanmaz | 390×844 PNG | Uygulanmaz; birleşik önizleme |

Zeyd revizyon 2'yi onayladı. Önceki ekranlar ve ham çalışma kaynakları Source altında korundu; inceleme görsellerindeki rakamlar üretim PNG'lerine gömülmedi. Ayrı final sprite teslimi aşağıdaki Task14 export tablosundadır. Oyun koduna dokunulmadı.

<!-- TASK14_FINAL_EXPORT_BEGIN -->
## Task14 — final export tamamlandı (Zeyd onaylı)

61 üretim PNG: @3x, gerçek alpha, yazısız/rakamsız. Border sırası sol/sağ/üst/alt; birim PNG pikseli. Pivot normalize Unity sol-alt başlangıçlıdır. Kontrol geçti; oyun entegrasyonu yapılmadı. Ayrıntı: `docs/art/TASK14_EXPORT.md`.

| Dosya | Oyunda nereye | Boyut | 9-slice L/R/T/B | Pivot x/y |
|---|---|---|---|---|
| `Assets/Art/Gameplay/Task14/hud-level-badge.png` | HUD level rozeti; LV ve numara ayrı | 216×216 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/hud-level-badge-pressed.png` | HUD level rozeti basılı | 216×216 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/hud-moves-pill.png` | HUD hamle kutusu; parça ikonu içerir, sayı boş | 360×168 px | 120/72/72/72 | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/hud-moves-pill-pressed.png` | HUD hamle kutusu basılı | 360×168 px | 120/72/72/72 | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/hud-time-pill.png` | HUD süre kutusu; saat ikonu içerir, sayı boş | 408×168 px | 126/72/72/72 | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/hud-time-pill-pressed.png` | HUD süre kutusu basılı | 408×168 px | 126/72/72/72 | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/hud-time-pill-urgent.png` | HUD azalan süre; mercan zemin, sayı boş | 408×168 px | 126/72/72/72 | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/hud-time-pill-urgent-pressed.png` | HUD acil süre basılı | 408×168 px | 126/72/72/72 | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/hud-pause-normal.png` | HUD durdur tuşu | 168×168 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/hud-pause-pressed.png` | HUD durdur tuşu basılı | 168×168 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/booster-button.png` | Booster boş yuvarlak tuş zemini | 264×264 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/booster-button-pressed.png` | Booster boş zemin basılı | 264×264 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/booster-button-zero.png` | Booster 0-adet zemin durumu; rakam ayrı | 264×264 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/booster-lock.png` | Booster kilitli durum kaplama simgesi | 96×96 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/booster-button-locked.png` | Booster kilitli zemin; kilit içerir | 264×264 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/booster-scissors.png` | Booster bağımsız scissors ikonu; görünür ölçü 150 px | 240×240 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/booster-wand.png` | Booster bağımsız wand ikonu; görünür ölçü 150 px | 240×240 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/booster-hammer.png` | Booster bağımsız hammer ikonu; görünür ölçü 150 px | 240×240 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/booster-clock.png` | Booster bağımsız clock ikonu; görünür ölçü 150 px | 240×240 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/booster-badge.png` | Booster adet rozeti; tüm rakamlar boş | 96×96 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/booster-scissors-normal.png` | Birleşik scissors booster / normal; sayı ayrı | 264×264 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/booster-scissors-pressed.png` | Birleşik scissors booster / pressed; sayı ayrı | 264×264 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/booster-scissors-locked.png` | Birleşik scissors booster / locked; sayı ayrı | 264×264 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/booster-scissors-zero.png` | Birleşik scissors booster / zero; sayı ayrı | 264×264 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/booster-wand-normal.png` | Birleşik wand booster / normal; sayı ayrı | 264×264 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/booster-wand-pressed.png` | Birleşik wand booster / pressed; sayı ayrı | 264×264 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/booster-wand-locked.png` | Birleşik wand booster / locked; sayı ayrı | 264×264 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/booster-wand-zero.png` | Birleşik wand booster / zero; sayı ayrı | 264×264 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/booster-hammer-normal.png` | Birleşik hammer booster / normal; sayı ayrı | 264×264 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/booster-hammer-pressed.png` | Birleşik hammer booster / pressed; sayı ayrı | 264×264 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/booster-hammer-locked.png` | Birleşik hammer booster / locked; sayı ayrı | 264×264 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/booster-hammer-zero.png` | Birleşik hammer booster / zero; sayı ayrı | 264×264 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/booster-clock-normal.png` | Birleşik clock booster / normal; sayı ayrı | 264×264 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/booster-clock-pressed.png` | Birleşik clock booster / pressed; sayı ayrı | 264×264 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/booster-clock-locked.png` | Birleşik clock booster / locked; sayı ayrı | 264×264 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/booster-clock-zero.png` | Birleşik clock booster / zero; sayı ayrı | 264×264 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/booster-bar.png` | Booster çubuğu boş paneli; tuşlar ayrı | 1170×360 px | 108/108/108/72 | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/board-frame-gold.png` | Tahta altın çerçevesi; ortası şeffaf, bariyer ayrı | 1125×1125 px | 300/300/300/300 | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/bomb-384.png` | Hücre bomba engeli; gövde merkezi (192,192), boş krem sayaç | 384×384 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/nail-head-384.png` | Hücre çivi engeli; üstten altın baş, boş geniş sayı yüzü | 384×384 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/chain-link-repeat-96x48.png` | İki hücre arası zincir; X tekrar | 96×48 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/chain-endpoint-144.png` | Zincirin hücrelerdeki belirgin uç kilidi | 144×144 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/wall-repeat-96x48.png` | Kapalı kenar bariyeri; X tekrar, dikey kullanımda 90 derece döndür | 96×48 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/wall-corner-96x96.png` | Bariyer L köşe; yukarı ve sağ açık bağlantı | 96×96 px | Yok | 0.25/0.25 |
| `Assets/Art/Gameplay/Task14/card-panel.png` | Ortak kart paneli; başlık ve gövde metni boş | 969×1200 px | 192/192/300/132 | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/btn-primary.png` | Kart primary tuşu; yazısız | 600×216 px | 84/84/84/84 | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/btn-primary-pressed.png` | Kart primary tuşu basılı; yazısız | 600×216 px | 84/84/84/84 | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/btn-secondary.png` | Kart secondary tuşu; yazısız | 600×216 px | 84/84/84/84 | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/btn-secondary-pressed.png` | Kart secondary tuşu basılı; yazısız | 600×216 px | 84/84/84/84 | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/btn-danger.png` | Kart danger tuşu; yazısız | 600×216 px | 84/84/84/84 | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/btn-danger-pressed.png` | Kart danger tuşu basılı; yazısız | 600×216 px | 84/84/84/84 | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/star-full.png` | Kazanma dolu yıldız | 180×180 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/star-empty.png` | Kazanma boş yıldız durumu | 180×180 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/card-pause.png` | Kart pause; tüm metinler ve eylem tuşları ayrı | 969×1320 px | 192/192/552/132 | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/card-win.png` | Kart win; tüm metinler ve eylem tuşları ayrı | 969×1200 px | 192/192/552/132 | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/card-lose.png` | Kart lose; tüm metinler ve eylem tuşları ayrı | 969×1200 px | 192/192/552/132 | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/card-start.png` | Kart start; tüm metinler ve eylem tuşları ayrı | 969×1200 px | 192/192/552/132 | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/loss-reason-bomb.png` | Kaybetme kartı bomba sebebi; boş etiket | 240×240 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/loss-reason-time.png` | Kaybetme kartı süre sebebi | 240×240 px | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/card-lose-bomb.png` | Kaybetme kartı bomba sebebiyle; metin ve tuş ayrı | 969×1200 px | 192/192/552/132 | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/card-lose-time.png` | Kaybetme kartı süre sebebiyle; metin ve tuş ayrı | 969×1200 px | 192/192/552/132 | 0.50/0.50 |
| `Assets/Art/Gameplay/Task14/preview-export.png` | İnceleme; runtime sprite değildir | 1200×1032 px | Uygulanmaz | Uygulanmaz |
| `Assets/Art/Gameplay/Task14/preview-booster-states.png` | İnceleme; runtime sprite değildir | 1200×1032 px | Uygulanmaz | Uygulanmaz |
| `Assets/Art/Gameplay/Task14/preview-cards.png` | İnceleme; runtime sprite değildir | 1200×774 px | Uygulanmaz | Uygulanmaz |
| `Assets/Art/Gameplay/Task14/preview-seams.png` | İnceleme; runtime sprite değildir | 768×240 px | Uygulanmaz | Uygulanmaz |
| `Assets/Art/Gameplay/Task14/preview-ingame.png` | İnceleme; runtime sprite değildir | 390×844 px | Uygulanmaz | Uygulanmaz |
| `Assets/Art/Gameplay/Task14/preview-details.png` | İnceleme; runtime sprite değildir | 720×304 px | Uygulanmaz | Uygulanmaz |
| `Assets/Art/Gameplay/Task14/LilitaOne-Regular.ttf` | TMP kaynak font; ı/ç/ö/ü var, ğ/ş/İ yok | TTF | Uygulanmaz | Uygulanmaz |
| `Assets/Art/Gameplay/Task14/LilitaOne-OFL.txt` | Font ile birlikte korunacak OFL 1.1 lisansı | Metin | Uygulanmaz | Uygulanmaz |
| `Assets/Art/Gameplay/Task14/manifest.json` | Dosya başına kullanım, ölçü, slice, pivot, @3x ve SHA-256 | 61 sprite kaydı | Metadata | Metadata |
| `Assets/Art/Gameplay/Task14/README.md` | Paket özeti ve entegrasyon bağlantıları | Markdown | Uygulanmaz | Uygulanmaz |

Bomba gövde merkezi (192,192); etiket ve çivi yüzü iki hane için boştur. `zero` durumları 0 rakamı içermez. Birleşik booster ile ayrı zemin+ikon yolu alternatiflerdir; iki yol üst üste kullanılmaz. Kart tuşları ayrı `btn-*` dosyalarıdır. Kazanma yıldızlarını dinamik göstermek için ortak panel ve ayrı yıldızlar kullanılır.

Zincir/duvar X tekrar kenarlarında ve duvar köşe bağlantılarında RGBA farkı: 0. Frame merkezi alpha=0. 252 korunan kaynakta hash farkı: 0. Font TTF kopyası birebir. `Source/` ham/önceki çalışma arşividir, final atlas teslimine dahil değildir. C# dosyası ve Assets/_Game değiştirilmedi. Font ayrıntısı: `docs/art/TASK14_FONT.md`; rapor: `docs/art/reports/task14-export-validation.json`.
<!-- TASK14_FINAL_EXPORT_END -->

### Task13 + Task14 oyundaki adları (Claude Code, 26 Ekim) — `Tools/art_choice.py`
| Dosya | Oyunda | Not |
|---|---|---|
| `Menu/Task13/*.png` | `m13-<ad>` | Menü: profil kutusu/avatar/çerçeve, sayaç hapı, yıldız/jiggle simgesi, ayarlar, Levels, ilerleme izi/dolgusu; alt gezinme build'e alınmadı (gizli) |
| `UI/PlayButton/menu-play-button-v2(-pressed).png` | `level-button(-pressed)` | 9-slice 92 px, yazı Lilita One |
| `Task14/hud-level-badge.png` | `level-badge` | LV + numara Lilita |
| `Task14/hud-moves-pill.png`, `hud-time-pill.png`, `hud-time-pill-urgent.png` | aynı adlar | İkon görselde; rakam Lilita lacivert (acil hâlde krem); süre ≤ 20 sn urgent |
| `Task14/hud-pause-normal.png` | `pause-normal` | |
| `Task14/booster-{scissors,wand,hammer,clock}-{normal,pressed,locked,zero}.png` | aynı adlar | Birleşik tuş: hazır / seçili / kilitli / adet 0; ayrı ikon ve kilit çizilmez |
| `Task14/booster-badge.png` | `booster-badge` | Rakam Lilita krem |
| `Task14/board-frame-gold.png` | `board-frame` | 4'ün katına dolduruldu (1128), 9-slice 300 px, bant 26 pt |
| `Task14/bomb-384.png`, `nail-head-384.png` | `bomb-v2`, `board-nail` | Bomba gövdesi hücrenin %76'sı (ortada), çivi başı ~%75; sayaç Lilita lacivert, kontursuz |
| `Task14/chain-link-repeat-96x48.png`, `chain-endpoint-144.png` | `chain-repeat`, `chain-end` | |
| `Task14/wall-repeat-96x48.png`, `wall-corner-96x96.png` | `wall-stripe`, `wall-corner` | Şerit hücrenin %20'si; köşe sol-alt çizildiği gibi, diğerleri döndürülür |
| `Task14/card-panel.png` | `popup-card` | Bütün kartlar; 972'ye dolduruldu, başlık turkuaz şeritte (Fredoka, krem) |
| `Task14/btn-primary.png`, `btn-secondary.png`, `btn-danger.png` | `primary-normal`, `secondary-normal`, `danger-normal` | 9-slice 84 px |
| `Task14/star-full.png`, `star-empty.png`, `loss-reason-bomb.png`, `loss-reason-time.png` | aynı adlar | Kazanma 3 yıldız; kaybetme sebebi ikonu |
| Kullanılmadı | — | `card-pause/win/lose*/start` (gömülü ikonlar içeriğimizle çakışır; ortak `card-panel`), `booster-bar` (panel yerleşimi yok), basılı HUD hâlleri |

<!-- TASK15_BEGIN -->
## Task15 — export tamamlandı

Hazır Source çizimlerinden Python/Pillow ile export edildi; yeniden çizim yapılmadı. Border L/R/T/B kaynak pikseli, pivot Unity sol-alt başlangıçlı normalize koordinattır. UI @3x; avatarlar özel 256×256 ölçü, tablo özel 1536×1536 ölçüdedir.

| Dosya | Oyunda nereye | Boyut | 9-slice L/R/T/B | Pivot |
|---|---|---|---|---|
| `Assets/Art/Gameplay/Task15/booster-badge-v2.png` | Yeni kırmızı adet rozeti; rakam ayrı | 96×96 | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task15/wall-repeat-v3.png` | Kalın kırmızı-beyaz duvar; X tekrar, native kalınlık 24 pt | 144×72 | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task15/wall-corner-v3.png` | Duvar köşesi; üstten giriş, sağdan çıkış | 144×144 | Yok | 0.25/0.25 |
| `Assets/Art/Gameplay/Task15/hud-moves-pill-v2.png` | Hamle hapı boş zemin; ikon ve sayı ayrı | 330×210 | 84/84/84/84 | 0.50/0.50 |
| `Assets/Art/Gameplay/Task15/hud-time-pill-v2.png` | Süre hapı boş zemin; kare kronometre ve sayı ayrı | 366×210 | 84/84/84/84 | 0.50/0.50 |
| `Assets/Art/Gameplay/Task15/hud-time-pill-urgent-v2.png` | Acil süre hapı; mercan yalnız burada, sayı ayrı | 366×210 | 84/84/84/84 | 0.50/0.50 |
| `Assets/Art/Gameplay/Task15/hud-icon-piece-v2.png` | HUD bağımsız piece ikonu; kare Simple/PreserveAspect | 120×120 | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task15/hud-icon-stopwatch-v2.png` | HUD bağımsız stopwatch ikonu; kare Simple/PreserveAspect | 120×120 | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task15/btn-primary-v2.png` | Başla/Devam yeşil ana tuş | 684×336 | 92/92/92/92 | 0.50/0.50 |
| `Assets/Art/Gameplay/Task15/btn-primary-v2-pressed.png` | Başla/Devam yeşil ana tuş basılı | 684×336 | 92/92/92/92 | 0.50/0.50 |
| `Assets/Art/Gameplay/Task15/btn-secondary-v2.png` | Yeniden başla/Ana menü turkuaz tuş; yazı ayrı | 684×336 | 92/92/92/92 | 0.50/0.50 |
| `Assets/Art/Gameplay/Task15/btn-secondary-v2-pressed.png` | Turkuaz ikincil tuş basılı | 684×336 | 92/92/92/92 | 0.50/0.50 |
| `Assets/Art/Gameplay/Task15/avatar-blue.png` | blue Jiggle profil avatarı; 256 px özel ölçü | 256×256 | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task15/avatar-pink.png` | pink Jiggle profil avatarı; 256 px özel ölçü | 256×256 | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task15/avatar-red.png` | red Jiggle profil avatarı; 256 px özel ölçü | 256×256 | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task15/avatar-green.png` | green Jiggle profil avatarı; 256 px özel ölçü | 256×256 | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task15/avatar-purple.png` | purple Jiggle profil avatarı; 256 px özel ölçü | 256×256 | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task15/jiggle-counter-icon.png` | Menü sağ üst kurtarılan Jiggle sayacı; 64 pt | 192×192 | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task15/painting-test-01.png` | Parçalara board UV ile kaplanacak test tablosu; çizgisiz tam resim | 1536×1536 | Yok | 0.50/0.50 |
| `Assets/Art/Gameplay/Task15/preview.png` | İnceleme; runtime değildir | 1296×1008 | Uygulanmaz | Uygulanmaz |
| `Assets/Art/Gameplay/Task15/preview-ingame.png` | İnceleme; runtime değildir | 390×844 | Uygulanmaz | Uygulanmaz |
| `Assets/Art/Gameplay/Task15/preview-ingame-blue.png` | İnceleme; runtime değildir | 390×844 | Uygulanmaz | Uygulanmaz |
| `Assets/Art/Gameplay/Task15/preview-pause.png` | İnceleme; runtime değildir | 390×844 | Uygulanmaz | Uygulanmaz |
| `Assets/Art/Gameplay/Task15/preview-avatars.png` | İnceleme; runtime değildir | 390×844 | Uygulanmaz | Uygulanmaz |
| `Assets/Art/Gameplay/Task15/preview-painted-board.png` | İnceleme; runtime değildir | 1536×1536 | Uygulanmaz | Uygulanmaz |
| `Assets/Art/Gameplay/Task15/preview-components.png` | İnceleme; runtime değildir | 1200×1270 | Uygulanmaz | Uygulanmaz |
| `Assets/Art/Gameplay/Task15/preview-seams.png` | İnceleme; runtime değildir | 900×300 | Uygulanmaz | Uygulanmaz |
| `Assets/Art/Gameplay/Task15/manifest.json` | Dosya ve entegrasyon metadata listesi | 19 PNG kaydı | Metadata | Metadata |
| `Assets/Art/Gameplay/Task15/README.md` | Paket ve katman kullanım notları | Markdown | Uygulanmaz | Uygulanmaz |

Haplar boş zemindir; ayrı 120×120 ikonlar Simple + PreserveAspect ile kullanılır. Yeni rozet için Task14 ayrı zemin+ikon yolu kullanılmalı; eski rozet içeren birleşik booster PNG üstüne ikinci rozet eklenmemeli. Detaylar: `docs/art/TASK15_EXPORT.md`.

Kontrol: 19 üretim PNG, 14 Source PNG değişmeden korundu, 109 eski onaylı sanat kaynağında fark 0; duvar ve köşe RGBA farkı 0. Rapor: `docs/art/reports/task15-export-validation.json`. C# ve Assets/_Game yazımı yapılmadı.
<!-- TASK15_END -->

### Task15 oyundaki adları (Claude Code, 26 Ekim)
| Dosya | Oyunda | Not |
|---|---|---|
| `booster-badge-v2.png` | `booster-badge` | Kilitliyken gizli (kilit tuş görselinde) |
| `wall-repeat-v3.png`, `wall-corner-v3.png` | `wall-stripe`, `wall-corner` | Şerit (tile'ın %90'ı) hücrenin %20'si |
| `hud-moves-pill-v2.png`, `hud-time-pill-v2.png`, `hud-time-pill-urgent-v2.png` | `hud-moves-pill`, `hud-time-pill`, `hud-time-pill-urgent` | 9-slice 84 px, 56 pt yükseklik |
| `hud-icon-piece-v2.png`, `hud-icon-stopwatch-v2.png` | `icon-piece`, `hud-icon-clock` | Hapın solunda 30 pt |
| `btn-primary-v2(-pressed).png`, `btn-secondary-v2(-pressed).png` | `btn2-primary`, `btn2-secondary` | Kartlarda tek tuşlar: 60 pt, kartın en çok %65'i; 1–10 ve 👍/👎 satırları Task14 tuşlarıyla |
| `avatar-{blue,pink,red,green,purple}.png` | aynı adlar | Avatar seçimi |
| `jiggle-counter-icon.png` | `jiggle-counter` | Menü sağ üst |
| `painting-test-01.png` | `painting-test-01` | Yalnız `?set=redesign&painting=1`, Level 7 |
| Kullanılmadı | — | Önizlemedeki süslü tahta çerçevesi ve kutulu booster çubuğu (Zeyd) |

<!-- TASK16_BATCH_1_BEGIN -->
## Task16 — Parti 1 tamamlandı

Üretim: built-in `image_gen`; stil referansı `Gameplay/Task15/painting-test-01.png`. Özgün sahneler; tabloların içinde yazı, logo, insan yüzü, çerçeve ve engel yok.

| Dosya | Bölüm | Tema | Boyut |
|---|---|---|---|
| `Assets/Art/Paintings/Source/painting-01.png` / `Assets/Art/Paintings/painting-01.jpg` | 1 | Çiçekli sahil köyü | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 290.413 bayt |
| `Assets/Art/Paintings/Source/painting-02.png` / `Assets/Art/Paintings/painting-02.jpg` | 2 | Renkli balıkçı limanı | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 290.274 bayt |
| `Assets/Art/Paintings/Source/painting-03.png` / `Assets/Art/Paintings/painting-03.jpg` | 3 | Teraslı zeytinlik | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 295.022 bayt |
| `Assets/Art/Paintings/Source/painting-04.png` / `Assets/Art/Paintings/painting-04.jpg` | 4 | Fener koyu | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 298.636 bayt |
| `Assets/Art/Paintings/Source/painting-05.png` / `Assets/Art/Paintings/painting-05.jpg` | 5 | Begonvilli sokak | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 291.545 bayt |
| `Assets/Art/Paintings/Source/painting-06.png` / `Assets/Art/Paintings/painting-06.jpg` | 6 | Limon bahçesi | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 297.837 bayt |
| `Assets/Art/Paintings/Source/painting-07.png` / `Assets/Art/Paintings/painting-07.jpg` | 7 | Kemerli su yolu | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 285.390 bayt |
| `Assets/Art/Paintings/Source/painting-08.png` / `Assets/Art/Paintings/painting-08.jpg` | 8 | Seramik avlusu | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 290.911 bayt |
| `Assets/Art/Paintings/Source/painting-09.png` / `Assets/Art/Paintings/painting-09.jpg` | 9 | Üzüm terasları | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 298.697 bayt |
| `Assets/Art/Paintings/Source/painting-10.png` / `Assets/Art/Paintings/painting-10.jpg` | 10 | Çamlıklı saklı koy | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 290.488 bayt |
| `Assets/Art/Paintings/preview-batch-1.png` | Parti 1 | 10 resimlik inceleme ızgarası | 1580×760 PNG |
| `Assets/Art/Paintings/preview-board-1.png` | Parti 1 | 6×6 tahta, bir bomba ve bir çivi | 1104×1104 PNG |

Üretim aracının verdiği 1254×1254 kareler kırpılmadan bicubic ile 1536×1536 kaynak ölçüsüne getirildi; bu teslim boyutu yerel üretim çözünürlüğü değildir. Oyun JPG dosyaları özgün üretimden Lanczos ile 1024×1024 küçültüldü, kalite 85 / 4:2:0 / optimize / progressive olarak kodlandı. Yoğun bitki ayrıntılarında 300.000 bayt sınırını sağlamak için yalnız JPG dosyalarında dosya başına kaydedilmiş hafif Gaussian ön filtre kullanıldı; kaynak PNG dosyalarına uygulanmadı.

Promptlar: `docs/art/task16-batch-1-prompts.json`. Dosya ölçüleri, yerel üretim ölçüsü, ön filtre yarıçapı, SHA-256 ve byte ölçümleri: `Assets/Art/Paintings/manifest-batch-1.json` ve `docs/art/reports/task16-batch-1-validation.json`.

Tahta önizlemesi gerçek Task14 bomba ve çivi görsellerini kullanır; bu engeller kaynak/oyun resimlerine gömülmedi. Önizleme kare dilimlidir, canlı oyun testi değildir. C# ve izinli iki klasörün dışındaki proje dosyalarına yazılmadı.
<!-- TASK16_BATCH_1_END -->

<!-- TASK16_BATCH_2_BEGIN -->
## Task16 — Parti 2 tamamlandı

Üretim: built-in `image_gen`; stil referansı `Gameplay/Task15/painting-test-01.png`. Özgün sahneler; tabloların içinde yazı, logo, insan yüzü, çerçeve ve engel yok.

| Dosya | Bölüm | Tema | Boyut |
|---|---|---|---|
| `Assets/Art/Paintings/Source/painting-11.png` / `Assets/Art/Paintings/painting-11.jpg` | 11 | Çiçekli orman şelalesi | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 292.306 bayt |
| `Assets/Art/Paintings/Source/painting-12.png` / `Assets/Art/Paintings/painting-12.jpg` | 12 | Dere kenarında değirmen | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 295.166 bayt |
| `Assets/Art/Paintings/Source/painting-13.png` / `Assets/Art/Paintings/painting-13.jpg` | 13 | Mantar ormanı | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 286.563 bayt |
| `Assets/Art/Paintings/Source/painting-14.png` / `Assets/Art/Paintings/painting-14.jpg` | 14 | Lavanta tarlası | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 285.822 bayt |
| `Assets/Art/Paintings/Source/painting-15.png` / `Assets/Art/Paintings/painting-15.jpg` | 15 | Nilüferli göl evi | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 294.858 bayt |
| `Assets/Art/Paintings/Source/painting-16.png` / `Assets/Art/Paintings/painting-16.jpg` | 16 | Elma bahçesi | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 294.433 bayt |
| `Assets/Art/Paintings/Source/painting-17.png` / `Assets/Art/Paintings/painting-17.jpg` | 17 | Dere üzerindeki ahşap köprü | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 284.391 bayt |
| `Assets/Art/Paintings/Source/painting-18.png` / `Assets/Art/Paintings/painting-18.jpg` | 18 | Kır çiçekli çiftlik | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 285.418 bayt |
| `Assets/Art/Paintings/Source/painting-19.png` / `Assets/Art/Paintings/painting-19.jpg` | 19 | Salkım söğütlü gölet | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 295.301 bayt |
| `Assets/Art/Paintings/Source/painting-20.png` / `Assets/Art/Paintings/painting-20.jpg` | 20 | Çiçekli orman serası | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 290.659 bayt |
| `Assets/Art/Paintings/preview-batch-2.png` | Parti 2 | 10 resimlik inceleme ızgarası | 1580×760 PNG |
| `Assets/Art/Paintings/preview-board-2.png` | Parti 2 | 6×6 tahta, bir bomba ve bir çivi | 1104×1104 PNG |

Üretim aracının verdiği 1254×1254 kareler kırpılmadan bicubic ile 1536×1536 kaynak ölçüsüne getirildi; bu teslim boyutu yerel üretim çözünürlüğü değildir. Oyun JPG dosyaları özgün üretimden Lanczos ile 1024×1024 küçültüldü, kalite 85 / 4:2:0 / optimize / progressive olarak kodlandı. Yoğun bitki ayrıntılarında 300.000 bayt sınırını sağlamak için yalnız JPG dosyalarında dosya başına kaydedilmiş hafif Gaussian ön filtre kullanıldı; kaynak PNG dosyalarına uygulanmadı.

Promptlar: `docs/art/task16-batch-2-prompts.json`. Dosya ölçüleri, yerel üretim ölçüsü, ön filtre yarıçapı, SHA-256 ve byte ölçümleri: `Assets/Art/Paintings/manifest-batch-2.json` ve `docs/art/reports/task16-batch-2-validation.json`.

Tahta önizlemesi gerçek Task14 bomba ve çivi görsellerini kullanır; bu engeller kaynak/oyun resimlerine gömülmedi. Önizleme kare dilimlidir, canlı oyun testi değildir. C# ve izinli iki klasörün dışındaki proje dosyalarına yazılmadı.
<!-- TASK16_BATCH_2_END -->

<!-- TASK16_BATCH_3_BEGIN -->
## Task16 — Parti 3 tamamlandı

Üretim: built-in `image_gen`; stil referansı `Gameplay/Task15/painting-test-01.png`. Özgün sahneler; tabloların içinde yazı, logo, insan yüzü, çerçeve ve engel yok.

| Dosya | Bölüm | Tema | Boyut |
|---|---|---|---|
| `Assets/Art/Paintings/Source/painting-21.png` / `Assets/Art/Paintings/painting-21.jpg` | 21 | Karlı köy meydanı | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 285.380 bayt |
| `Assets/Art/Paintings/Source/painting-22.png` / `Assets/Art/Paintings/painting-22.jpg` | 22 | Dağ yamacında kulübe | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 295.908 bayt |
| `Assets/Art/Paintings/Source/painting-23.png` / `Assets/Art/Paintings/painting-23.jpg` | 23 | Buz pateni göleti | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 298.649 bayt |
| `Assets/Art/Paintings/Source/painting-24.png` / `Assets/Art/Paintings/painting-24.jpg` | 24 | Kuzey ışıklı vadi | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 298.466 bayt |
| `Assets/Art/Paintings/Source/painting-25.png` / `Assets/Art/Paintings/painting-25.jpg` | 25 | Karlı vadide tren köprüsü | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 292.654 bayt |
| `Assets/Art/Paintings/Source/painting-26.png` / `Assets/Art/Paintings/painting-26.jpg` | 26 | Donmuş şelale bahçesi | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 296.348 bayt |
| `Assets/Art/Paintings/Source/painting-27.png` / `Assets/Art/Paintings/painting-27.jpg` | 27 | Dağ teleferik durağı | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 287.764 bayt |
| `Assets/Art/Paintings/Source/painting-28.png` / `Assets/Art/Paintings/painting-28.jpg` | 28 | Karlı teras bahçesi | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 291.881 bayt |
| `Assets/Art/Paintings/Source/painting-29.png` / `Assets/Art/Paintings/painting-29.jpg` | 29 | Işıklı buz mağarası | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 285.428 bayt |
| `Assets/Art/Paintings/Source/painting-30.png` / `Assets/Art/Paintings/painting-30.jpg` | 30 | Zirvedeki gözlemevi | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 289.200 bayt |
| `Assets/Art/Paintings/preview-batch-3.png` | Parti 3 | 10 resimlik inceleme ızgarası | 1580×760 PNG |
| `Assets/Art/Paintings/preview-board-3.png` | Parti 3 | 6×6 tahta, bir bomba ve bir çivi | 1104×1104 PNG |

Üretim aracının verdiği 1254×1254 kareler kırpılmadan bicubic ile 1536×1536 kaynak ölçüsüne getirildi; bu teslim boyutu yerel üretim çözünürlüğü değildir. Oyun JPG dosyaları özgün üretimden Lanczos ile 1024×1024 küçültüldü, kalite 85 / 4:2:0 / optimize / progressive olarak kodlandı. Yoğun bitki ayrıntılarında 300.000 bayt sınırını sağlamak için yalnız JPG dosyalarında dosya başına kaydedilmiş hafif Gaussian ön filtre kullanıldı; kaynak PNG dosyalarına uygulanmadı.

Promptlar: `docs/art/task16-batch-3-prompts.json`. Dosya ölçüleri, yerel üretim ölçüsü, ön filtre yarıçapı, SHA-256 ve byte ölçümleri: `Assets/Art/Paintings/manifest-batch-3.json` ve `docs/art/reports/task16-batch-3-validation.json`.

Tahta önizlemesi gerçek Task14 bomba ve çivi görsellerini kullanır; bu engeller kaynak/oyun resimlerine gömülmedi. Önizleme kare dilimlidir, canlı oyun testi değildir. C# ve izinli iki klasörün dışındaki proje dosyalarına yazılmadı.
<!-- TASK16_BATCH_3_END -->

<!-- TASK16_BATCH_4_BEGIN -->
## Task16 — Parti 4 tamamlandı

Üretim: built-in `image_gen`; stil referansı `Gameplay/Task15/painting-test-01.png`. Özgün sahneler; tabloların içinde yazı, logo, insan yüzü, çerçeve ve engel yok.

| Dosya | Bölüm | Tema | Boyut |
|---|---|---|---|
| `Assets/Art/Paintings/Source/painting-31.png` / `Assets/Art/Paintings/painting-31.jpg` | 31 | Fener festivali | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 288.892 bayt |
| `Assets/Art/Paintings/Source/painting-32.png` / `Assets/Art/Paintings/painting-32.jpg` | 32 | Yıldızlı küçük hisar | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 296.099 bayt |
| `Assets/Art/Paintings/Source/painting-33.png` / `Assets/Art/Paintings/painting-33.jpg` | 33 | Sihirli kütüphane | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 293.002 bayt |
| `Assets/Art/Paintings/Source/painting-34.png` / `Assets/Art/Paintings/painting-34.jpg` | 34 | Balon şehri | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 299.954 bayt |
| `Assets/Art/Paintings/Source/painting-35.png` / `Assets/Art/Paintings/painting-35.jpg` | 35 | Ay ışıklı nilüfer köşkü | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 294.295 bayt |
| `Assets/Art/Paintings/Source/painting-36.png` / `Assets/Art/Paintings/painting-36.jpg` | 36 | Büyülü gök küresi salonu | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 287.754 bayt |
| `Assets/Art/Paintings/Source/painting-37.png` / `Assets/Art/Paintings/painting-37.jpg` | 37 | Uçan adalar şelalesi | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 299.288 bayt |
| `Assets/Art/Paintings/Source/painting-38.png` / `Assets/Art/Paintings/painting-38.jpg` | 38 | Işıklı iksir atölyesi | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 298.678 bayt |
| `Assets/Art/Paintings/Source/painting-39.png` / `Assets/Art/Paintings/painting-39.jpg` | 39 | Kont'un büyülü bahçesi | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 298.684 bayt |
| `Assets/Art/Paintings/Source/painting-40.png` / `Assets/Art/Paintings/painting-40.jpg` | 40 | Kont'un görkemli şatosu | Kaynak PNG 1536×1536; oyun JPG 1024×1024, kalite 85, 291.020 bayt |
| `Assets/Art/Paintings/preview-batch-4.png` | Parti 4 | 10 resimlik inceleme ızgarası | 1580×760 PNG |
| `Assets/Art/Paintings/preview-board-4.png` | Parti 4 | 6×6 tahta, bir bomba ve bir çivi | 1104×1104 PNG |

Üretim aracının verdiği 1254×1254 kareler kırpılmadan bicubic ile 1536×1536 kaynak ölçüsüne getirildi; bu teslim boyutu yerel üretim çözünürlüğü değildir. Oyun JPG dosyaları özgün üretimden Lanczos ile 1024×1024 küçültüldü, kalite 85 / 4:2:0 / optimize / progressive olarak kodlandı. Yoğun bitki ayrıntılarında 300.000 bayt sınırını sağlamak için yalnız JPG dosyalarında dosya başına kaydedilmiş hafif Gaussian ön filtre kullanıldı; kaynak PNG dosyalarına uygulanmadı.

Promptlar: `docs/art/task16-batch-4-prompts.json`. Dosya ölçüleri, yerel üretim ölçüsü, ön filtre yarıçapı, SHA-256 ve byte ölçümleri: `Assets/Art/Paintings/manifest-batch-4.json` ve `docs/art/reports/task16-batch-4-validation.json`.

Tahta önizlemesi gerçek Task14 bomba ve çivi görsellerini kullanır; bu engeller kaynak/oyun resimlerine gömülmedi. Önizleme kare dilimlidir, canlı oyun testi değildir. C# ve izinli iki klasörün dışındaki proje dosyalarına yazılmadı.
<!-- TASK16_BATCH_4_END -->

<!-- TASK16_COLLECTION_COMPLETE -->

## Task16 — 40 tablo tamamlandı

4 oda × 10 tablo: 40 adet 1536×1536 PNG kaynak, 40 adet 1024×1024 kalite 85 JPG (her biri 300.000 bayt altında), 4 toplu ızgara ve 4 adet 6×6 bomba/çivi önizlemesi teslim edildi. 40. tablo Kont'un görkemli şatosudur; Kont, insan veya yüz görünmez.

Tek katalog: `Assets/Art/Paintings/manifest-collection.json`. 80 kaynak/oyun dosyasının SHA-256 değeri kontrol edildi; fark 0. Son parti öncesindeki 66 sanat dosyası değişmedi. Genel rapor: `docs/art/reports/task16-collection-validation.json`. Üretim promptları dört ayrı `docs/art/task16-batch-N-prompts.json` dosyasındadır. Yerel üretimden 1536 px boyutlandırma ve yalnız JPG ön filtresi notları yukarıdaki parti kayıtlarında ve README içinde korunmuştur.

<!-- TASK17_BEGIN -->
## Task17 — Resimli tahtada görünür zincir v3

| Dosya | Oyunda nereye | Boyut | Pivot |
|---|---|---|---|
| `Assets/Art/Gameplay/Task17/chain-link-v3.png` | Zincir kilitleri arasında tekrarlanan iri demir halka bandı; U Repeat / V Clamp | 128×64 RGBA | 0.50 / 0.50 |
| `Assets/Art/Gameplay/Task17/chain-lock-v3.png` | Zincir uçlarındaki altın kilit; kalın demir bağlantı halkası | 160×160 RGBA | 0.50 / 0.50 |
| `Assets/Art/Gameplay/Task17/preview-chain.png` | Tablo 40 ve 24 üzerinde eski/yeni çapraz zincir karşılaştırması | 1120×1320 RGB | Uygulanmaz |
| `Assets/Art/Gameplay/Task17/preview-repeat.png` | Açık/koyu zeminde tekrar ve kilit yakın görünümü | 1024×290 RGBA | Uygulanmaz |

PALETTE.md renkleri: lacivert demir #0B2A36, altın üst ışığı #FBBA0E, 4 px krem dış hale #F8EBD0. 56 px alpha bandı için sprite tuvali yüksekliği hücrenin %21.7143'ü, görünen kalınlık yaklaşık %19; yatay tekrar adımı %43.4286. Kilit üst halka bağlantısı kaynakta (80,27), Unity normalize (0.50,0.83125). Pivot merkezde kalır. Önizlemede eski/yeni tuval yüksekliği aynıdır.

Kontrol: 128×64 ve 160×160, şeffaf halka açıklığı ve dış alan; X tekrar kenarlarında RGBA farkı 0. Eski iki zincir/kilit kaynağı ve iki tablo hash farkı 0. İki resimde görsel okunurluk incelendi. Kaynak: `docs/art/build-task17.py`; rapor: `docs/art/reports/task17-validation.json`; kullanım: `Assets/Art/Gameplay/Task17/README.md`. Yalnız Task17 ve docs/art yazımı; C#, Unity veya dotnet çalıştırılmadı. Entegrasyon yapılmadı.
<!-- TASK17_END -->

<!-- TASK17_COLORS_BEGIN -->
## Task17 — Renkli zincir çiftleri

| Dosya | Oyunda nereye | Boyut | Pivot |
|---|---|---|---|
| `Assets/Art/Gameplay/Task17/chain-link-v3-altin.png` | Altın çiftin tekrarlanan zinciri | 128×64 RGBA | 0.50 / 0.50 |
| `Assets/Art/Gameplay/Task17/chain-lock-v3-altin.png` | Altın çiftin iki uç kilidi; yıldız | 160×160 RGBA | 0.50 / 0.50 |
| `Assets/Art/Gameplay/Task17/chain-link-v3-turkuaz.png` | Turkuaz çiftin tekrarlanan zinciri | 128×64 RGBA | 0.50 / 0.50 |
| `Assets/Art/Gameplay/Task17/chain-lock-v3-turkuaz.png` | Turkuaz çiftin iki uç kilidi; damla | 160×160 RGBA | 0.50 / 0.50 |
| `Assets/Art/Gameplay/Task17/chain-link-v3-mor.png` | Mor çiftin tekrarlanan zinciri | 128×64 RGBA | 0.50 / 0.50 |
| `Assets/Art/Gameplay/Task17/chain-lock-v3-mor.png` | Mor çiftin iki uç kilidi; hilal ay | 160×160 RGBA | 0.50 / 0.50 |
| `Assets/Art/Gameplay/Task17/chain-link-v3-mercan.png` | Mercan çiftin tekrarlanan zinciri | 128×64 RGBA | 0.50 / 0.50 |
| `Assets/Art/Gameplay/Task17/chain-lock-v3-mercan.png` | Mercan çiftin iki uç kilidi; kalp | 160×160 RGBA | 0.50 / 0.50 |
| `Assets/Art/Gameplay/Task17/preview-chain-colors.png` | Aynı tahtada iki renkli çift; tablo 40 ve 24 | 1120×1060 RGB | Uygulanmaz |

Onaylı v3 geometri ve dosyaları korunur. Halka U Repeat/V Clamp; 4 px krem hale; tuval yüksekliği hücrenin %21.7143'ü, görünür kalınlık yaklaşık %19. Kilit bağlantısı (0.50,0.83125). Dört renk ailesinin mor skalası ve mercan kullanım izni PALETTE.md Task17 ekine yazıldı. Renk körlüğü için her eşleşme hem renk hem ayrı şekille kodlanır.

Üretim: `docs/art/build-task17-colors.py`. Hash/boyut/pivot kayıtları: `Assets/Art/Gameplay/Task17/manifest-colors.json`. Kontrol: `docs/art/reports/task17-colors-validation.json`; dört X tekrar kenarında RGBA farkı 0, onaylı alpha geometri farkı 0, korunan kaynak hash farkı 0. Önizleme görsel olarak incelendi. C#, Unity, dotnet çalıştırılmadı; oyun entegrasyonu yapılmadı.
<!-- TASK17_COLORS_END -->
