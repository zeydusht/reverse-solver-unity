# Task19 — Tam ekran tasarımlar ve Task20 parça planı

İnceleme: [inceleme.html](../../Assets/Art/Menu/Task19/inceleme.html). Dokuz ana PNG 1125×2436 px (@3x). Bu teslim tasarım önerisidir; yeni parçalar onaylı üretim sprite'ı sayılmaz. Unity sahnesi, C# veya oyun verisi değiştirilmedi.

## Katmanlar ve yeniden üretim

- `Assets/Art/Menu/Task19/layers/<ekran>-art.png`: yazısız kompozit tasarım katmanı.
- `layers/<ekran>-text.png`: bağımsız şeffaf metin katmanı.
- `layers/<ekran>-text.json`: özgün Türkçe metin, @3x konum, punto ve renk. İsim, sayılar ve bölüm numaraları burada ayrı kayıtlardır.
- `source/sources.json`: kullanılan mevcut dosyaların tam yolları.
- `source/jiggles-shelf.png`: onaylı Jiggles kartı ve Task15 avatarları referansıyla image_gen tarafından üretilen yeni raf sahnesi. Henüz Zeyd onayı yok. Task20'de karakterler ve raf ayrı çıkarılmalı.
- `source/koleksiyon-16.png`, `koleksiyon-31.png`: 16–40 hücrelerin devam görünümü. Ana ekranla birlikte 40 hücrenin tamamı gösterilir.
- `source/bolumler-17.png`, `22`, `28`, `34`: 19, 23, 25, 30, 35, 36, 38 zor düğümleri ve 20/30/40 tablo duraklarının inceleme görünümü.
- Yeniden üretim: `python docs/art/build-task19.py` (Pillow ve NumPy gerekir; yeni görsel çağrısı yapmaz).

Türkçe metin Fredoka-SemiBold ile çizilir. Depodaki fontun `ş/Ş/ğ/Ğ/İ` glifleri eksik çıktığından, bu tasarımda Fredoka taban harfi ve ayrı çizilen çengel/kısa yay/nokta birleştirildi; JSON gerçek Unicode metni korur. Tam fontu indirme girişimi ağ kısıtı nedeniyle başarısız oldu. Task20/oyun entegrasyonunda tam Latin Extended Fredoka dosyası kullanılmalıdır. Ses ikonları ve tamamlanma tikleri font glifi yerine çizimdir.

Üst 132 px ve alt 102 px alana kontrol/yazı yerleştirilmedi; arka plan bu alanlarda devam eder. Ev, Koleksiyon, Jiggles ve Başarılar şeritleri aynı y konumundadır (ana şerit 2172 px, etiket merkezi 2307 px). Krem kartlarda lacivert metin; turkuaz/karanlık zeminlerde okunurluk için krem etiket kullanılır. Ayarlar açık anahtarı, yeşil Jiggle ve görevde açıkça istenen karakter rengi, ana eylem yeşili kuralının belirtilmiş istisnalarıdır. Bölüm yolu tamamlanan düğümleri turkuaz-altın olarak gösterir; ana eylem dışındaki yeşili sınırlama kuralı esas alındı.

## Mevcut onaylı parçalar

Ölçüler tasarımdaki hedef @3x boyuttur; Unity import/9-slice değerleri bu turda değiştirilmedi.

| Parça | Kaynak / durum | Hedef ölçü @3x | 9-slice | Tekrar kullanıldığı ekran |
|---|---|---|---|---|
| Koridor | mevcut: `Review/Koridor/2-ortali-yan-blok-v04.png` | 1125×2436 | Hayır | Tüm ekranlar |
| Ana bölüm tuşu | mevcut: `UI/PlayButton/menu-play-button-v2.png` | 765×345 | Hayır, oran korunmalı | 01 |
| Profil kutusu | mevcut: `Menu/Task13/profile-box.png` | 516×168 | Mevcut kenarlar korunacak | 01 |
| Ayarlar dişlisi | mevcut: `Menu/Task13/settings-button.png` | 147×147 | Hayır | 01 |
| Sayaç hapı | mevcut: `Menu/Task13/counter-pill.png` | 330×102 | Evet, yatay | 01 |
| Yıldız / Jiggle sayaç ikonu | mevcut: `Menu/Task13/token-star.png`, `token-jiggle.png` | 111×111 | Hayır | 01, 03, 05 |
| Kurtarılan Jiggle | mevcut: `Gameplay/Task15/jiggle-counter-icon.png` | 171×171 / 69×69 | Hayır | 03, 05 |
| Bölümler tuşu | mevcut: `Menu/Task13/levels-button.png` | 153×153 | Hayır | 01 |
| İlerleme izi / dolgu | mevcut: `Menu/Task13/progress-track.png`, `progress-fill.png` | 705×84 | Evet; dolgu kırpılır | 01; ilerleme dili 04 |
| Alt şerit | mevcut: `Menu/Task13/bottom-nav.png` | 1125×162 | Evet, yatay | 01, 02, 03, 04, 08 |
| Seçili sekme | mevcut: `Menu/Task13/nav-active.png` | 246×216 | Hayır | 01, 02, 03, 04, 08 |
| Dört sekme ikonu | mevcut: `Menu/Task13/nav-home.png`, `nav-collection.png`, `nav-jiggles.png`, `nav-trophy.png` | seçili 177×177, normal 144×144 | Hayır | Sekmeler, 08; madalyon içleri |
| Bildirim noktası | mevcut: `Menu/Task13/notification-badge.png` | 45×45 | Hayır | Sekmeler, 08 |
| Kart kabuğu | mevcut: `Gameplay/Task14/card-panel.png` | 993×1449 / 993×1638 / 993×1443 | Task20: gövde ve başlık ayrılarak 9-slice | 02b, 05, 06 |
| İkincil tuş | mevcut: `Gameplay/Task15/btn-secondary-v2.png` | 723×174 / 729×177 / 399×138 | Evet, yatay | 02b, 05, 06 |
| Beş avatar | mevcut: `Gameplay/Task15/avatar-{blue,pink,red,green,purple}.png` | 177×177; profil 258×258 | Hayır | 01, 03, 05, 07 |
| Tablo çerçevesi | mevcut: `Menu/InfiniteCorridor/frame-empty-v01.png` (şeffaf tuvalin içerik alanı) | 300×228; detay 783×591 | Hayır | 02, 02b, 07 |
| Gerçek tablolar | mevcut: `Paintings/painting-01.jpg` … `painting-40.jpg` | hücre içi 240×156; detay 723×519 | Hayır | 01, 02, 02b, 07 |
| Yıldız | mevcut: `Gameplay/Task14/star-full.png` | 36×36 / 63×63 | Hayır | 02, 02b, 04, 05, 07 |
| Kilit | mevcut: `Gameplay/Task14/booster-lock.png` | 60×72 | Hayır | 02, 04, 07 |
| Bomba, saat, zincir | mevcut: `Gameplay/Task14/bomb-384.png`, `booster-clock.png`, `chain-link-repeat-96x48.png` | 90×90 | Hayır | 04, 05 |

## Task20'de yeni çizilecek / ayrı hazırlanacak parçalar

Kompozit içindeki küçük geometrik işaretler yerleşim önerisidir. Nihai kabartma işçiliği onaylı Level v2 ailesiyle eşleştirilecek; bu turda küçük sprite export'u yapılmadı.

| Yeni parça adı | Ölçü @3x | 9-slice | Ekran / tekrar kullanımı |
|---|---|---|---|
| collection-next-tag | 228×54 | Evet | 02; ileride 07 |
| collection-next-glow | 324×252 | Hayır | 02; sıradaki çerçevede hafif dış ışık |
| hard-corner-red | 66×66 | Hayır | 02; 19, 23, 25, 30, 35, 36, 38 |
| close-button | 90×90 | Hayır | 02b, 05, 06 |
| jiggles-display-shelf | 1023×786 kompozit alan | Hayır | 03; mevcut yeni sahneden raf/arka plan ayrılacak |
| jiggle-standing-{blue,pink,red,green,purple} | her biri yaklaşık 210×300 | Hayır | 03; beş ayrı karakter, iki kol ve iki bacak silueti korunur |
| jiggle-color-count-pill | 144×60 | Evet | 03, beş tekrar |
| avatar-selected-ring | 195×195 | Hayır | 03; 05 avatar seçimine de taşınabilir |
| milestone-rail | 783×9 | Evet, yatay | 03 |
| milestone-{frame,badge,portrait} | 78×78, üç farklı sembol | Hayır | 03; mockup'taki çerçeve/yıldız/portre parçaları temel alınır |
| achievement-row | 1017×171 | Evet | 04; dokuz örnek, toplam 16 satıra ölçeklenir |
| achievement-medal-base | 120×120 | Hayır | 04, 05 |
| achievement-symbol-{first,collector,museum,bomb,hourglass,flawless,stars,chain,baron-hat} | 90×90 | Hayır | 04; mevcut ikonlar ve çizim önerileri temel alınır. İlk Tablo/Koleksiyoncu ayrı tek/çoklu tablo; Soğukkanlı kum saati |
| achievement-medal-locked | 120×120 | Hayır | 04; gri-lacivert |
| achievement-check | 60×54 | Hayır | 04, 05 |
| slim-progress-track/fill | 588×15 | Evet, yatay | 04; sayılar ayrı |
| profile-stat-cell | 414×219 | Evet | 05; altı tekrar |
| profile-edit-pencil | 48×48 | Hayır | 05 |
| setting-{sound,music,vibration} | 96×96 | Hayır | 06 |
| toggle-on/off + knob | 192×90; düğme 66×66 | Evet izde, düğmede hayır | 06; iki durum görünür |
| language-segment + selected | 585×135 / 276×111 | Evet | 06 |
| path-carpet-repeat | 81×240 başlangıç önerisi | Evet, boyuna | 07; kıvrımlı yol için parçalı mesh/spline gerekebilir |
| level-node-{complete,current,locked,hard} | normal 162×162, current 198×198 | Hayır | 07; tamamlanan için görevde yeşil-altın / turkuaz-altın kararı bekleniyor |
| current-node-glow | 240×240 | Hayır | 07; mevcut küçük avatar yeniden kullanılır |
| hard-baron-face | 102×102 | Hayır | 07; mockup'ta onaylı `Characters/Baron-reference-v01.png` öfkeli portresi dairesel kırpılarak yeniden kullanılır; nihai şeffaf portre ayrı hazırlanır |
| back-arrow | 72×72 | Hayır | 07; diğer tam ekran alt sayfalarda tekrar |

## İçerik ve onay kararları

- Gerçek tablo başlıkları dört `Paintings/manifest-batch-*.json` dosyasından okunur. Detay ekranı 7. tablonun gerçek adı **Kemerli su yolu** ile gösterilir. 1.248 renk toplamı: 328 mavi + 277 pembe + 242 kırmızı + 211 yeşil + 190 mor.
- Başarılar 5/16 olarak, istenen dokuz örnek satırla gösterilir; diğer yedi başarıya uydurma ad eklenmez. Bomba/saat ve mevcut sekme ikonları mockup'ta yeniden kullanılır; nihai özgün semboller yukarıdaki Task20 planındadır.
- Görev hem “Bölüm 12 sıradaki” hem “12/40 koleksiyon” istediği için aynı sayılar korunmuştur; bu bir oyun kayıt modeli tanımı değildir. Ekranda 1–11 tamamlanmış, 12 sıradaki görünür. Entegrasyonda koleksiyon sayısının tamamlanan tabloyu mu, görülen tabloyu mu sayacağı netleşmelidir.
- Karar 1: Genel yerleşim ve Jiggles raf sahnesi onaylanıyor mu?
- Karar 2: `12/40` koleksiyon sayacı yalnız kazanılan tabloları sayacaksa bu örnekte `11/40` yapılsın mı?
- Karar 3: Bölüm yolu tamamlanan düğümlerde ana eylem dışındaki yeşile izin verilsin mi, yoksa tasarımdaki turkuaz-altın korunsun mu?

## Kontrol

Üretim betiği dokuz PNG'nin 1125×2436 boyutunu ve tüm metin katmanlarının üst/alt güvenli alanını doğrular. Dokuz ekranın ortak kontak sayfası `source/contact-sheet.jpg` üzerinden gözle kontrol edildi; Türkçe eksik glif kutuları, raf sayaç konumları ve başlık çakışmaları düzeltildi. Oyuna bağlanmadığı için çalışma zamanı testi kapsam dışıdır.
