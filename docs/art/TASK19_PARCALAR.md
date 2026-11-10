# Task19D — Task20 parça ve katman planı

Bu liste Task19D tasarımını esas alır; eski Task19 listesi `docs/art/task19d/TASK19_PARCALAR-before19d.md` olarak saklandı. Task19C'nin altın oranı, yıldızsız dili ve Ev yerleşimi onaylıdır. **Yeni D parçaları ekran tasarımı adayıdır; bu liste üretim sprite/atlas export'u yapıldığı anlamına gelmez.**

İnceleme: [9 ekran yan yana](../../Assets/Art/Menu/Task19D/inceleme.html) · [kontak](../../Assets/Art/Menu/Task19D/kontak.png). Dokuz ekran 1125×2436 px @3x; kontak 1125×2436 px (3×3 telefon, her biri 375×812).

## Sabitler

- Ekran: 375×812 pt. Üst 132 px ve alt 102 px kontrol/metin alanı değildir. Ortak alt çubuk y=2154–2334 px; bütün ekranlarda aynıdır.
- Altın yalnız küçük vurgu/ince halka/üst şerit; büyük yüzeyler krem-turkuaz-laciverttir. Ev'in onaylı mevcut büyük tablo çerçevesi korunur. Yıldız puanı, yıldız başarısı, para, kalp, mağaza yoktur.
- Türkçe Fredoka. Başlıklar krem, lacivert kontur/gölge; krem kart metni lacivert. Gövde/sekme metni genellikle 15 pt ve üstüdür. D'nin açık küçültme talebine göre uzun isim ve Bomba açıklaması gerektiği kadar küçülür; sürüm yazısı küçüktür.
- Gerçek kare resim merkezi **[128,0,896,1024]** ile 768×1024'e kırpılır; esnetilmez. Koleksiyon dış hücresi **243×324**, iç resmi **207×276 px**, ikisi de **3:4**. Büyük detay resmi **540×720 px**; yol dönüm noktası resmi **90×120 px**.
- Koleksiyon 3 sütun; hücre başlangıçları x=81/441/801 px. İlk sıra y=498 px, sıra adımı 390 px. Dört sıra ve etiketleri tamamı görünür; beşinci sıra y=2088 px'de başlar, görünüm y=2154 px'de kesilir. Yalnız ilk 11 tablo kazanılmış, 12 sıradakidir.
- Normal sekme ikon yüksekliği 126 px; seçili 163,8≈164 px (**%30 büyük**). Seçili yuvanın **tamamı** açık turkuaz/yükseltilmiş, diğerleri gömülü ve koyudur. İkon kabarcığı ayrıca korunur. Etiket kabarcığın dışında, ikonun altında ve en üst katmandadır.

## Ortak yeni / yeniden kullanılacak parçalar

Ölçüler @3x hedef ölçüleridir. Genişletilebilir kabuklar için belirtilen 9-slice Task20 hazırlık önerisidir; Unity import değeri bu tur değiştirilmedi. Aynı raster sahnede birleşmiş parçalar Task20'de ayrı temiz alfa ile hazırlanmalıdır.

| Ad | Durum / kaynak | Ölçü @3x | 9-slice | Ekranlar / tekrar |
|---|---|---:|---|---|
| nav-rail-home/collection/jiggles/trophy | **Yeni D:** `task19d/sources/nav-bays.png`, dört ayrı seçili yuva | her biri 1125×180 | Evet, yuvalar ayrı 281/282 px hücreler; kenar/bevel korunur | 01,02,02b,03,04,05,06,07,08 |
| nav-selection-bubble | mevcut: `task19c/sources/nav-support.png` | 174×174 | Hayır | Tüm sekme durumları |
| nav-icon-home/collection/jiggles/trophy | mevcut: onaylı C, `task19c/sources/nav-atlas.png` alt bağımsız ikon seti | normal yükseklik126, seçili164; oran korunur | Hayır | Tüm ekranlar, profil ikonları |
| notification-badge | mevcut: `Gameplay/Task15/booster-badge-v2.png` | 57×57 | Hayır | Başarılar ikonunda; rakam **1 ayrı** |
| title-plaque | C dilinde **yeni D** kart/sahne başlıkları | dar yaklaşık720×180; geniş yaklaşık960×195 | Evet; kıvrımlı uçlar sabit | 02,02b,03,04,05,06,07 |
| cream-teal-counter-pill | mevcut C/D sahnelerde; ayrı temiz export gerekir | yaklaşık288×108 | Evet, yatay | 02=11/40;03=5 renk;04=5/16;07=40 bölüm |
| modal-cream-shell | **Yeni D:** detay/profil/ayarlar kaynakları; turkuaz rim | yaklaşık990×1650 | Evet; başlık ayrı | 02b,05,06 |
| close-red-circle | **Yeni D:** kırmızı yuvarlak düğme, X ayrı/ikon | yaklaşık84×84 | Hayır | 02b,05,06 |
| button-secondary-teal | **Yeni D:** aynı kabartmalı turkuaz aile | büyük yaklaşık735×177; küçük405×132 | Evet, yatay | 02b,05,06 |
| portrait-frame-teal | **Yeni D:** `koleksiyon-v2.png` ilk çerçevesinden yeniden kullanılan doku | 243×324 | Evet; mockup'ta kaynak34px → hedef26px köşeler | 02; büyük ve küçük varyantlarla02b/07 |
| painting-01…40 | mevcut: `Assets/Art/Paintings/painting-XX.jpg` | merkezi768×1024 kırpım; hedefler yukarıda | Hayır | 02,02b,07 |
| padlock-teal-cream | mevcut: `Gameplay/Task14/booster-lock.png`; D yolunda hacimli yeni kilit | yaklaşık69×87; büyük yol kilidi yaklaşık66×72 | Hayır | 02,07 |

`task19c/` ve `task19d/` kaynak yolları bu tabloda `docs/art/` altındadır; diğer kısa yollar `Assets/Art/` altındadır.

## 01 — Ev

| Parça | Durum | Ölçü @3x | 9-slice / tekrar |
|---|---|---:|---|
| home-corridor/hero | mevcut: güncel `docs/qa/2026-10-29-p19/tr_390x844_2_menu.png`; C temiz koridor katmanı | 1125×2436 | Hayır; mevcut tablo ve konumu korunur |
| profile-avatar/gear/jiggle-counter | mevcut: güncel QA piksel parçaları, onaylı C yerleşimi | C ile aynı | Hayır; final export'ta yazısızlaştırılacak |
| profile-name-box | mevcut: `Menu/Task13/profile-box.png`, üst avatar korunarak yeniden yerleştirildi | 351×120 | Evet; metin alanına otomatik sığdırma |
| profile-name-text | **Yeni D düzeltmesi:** kırpılmış “Zeynep Yılma” yerine tam **Zeynep Yılmaz** | alan282 px genişlik; punto sığdırılır | Metin ayrı; 14+ karakterlerde de fit uygulanır |
| primary-level-button | mevcut: güncel QA/C yeşil Level1 tuşu | yaklaşık645×303 | Görünüş/konum C onayına göre korunur; metin sonradan dinamik |
| levels-button | mevcut: `Menu/Task13/levels-button.png` | 132×132 | Hayır; “Bölümler” ayrı metin |

Profil avatarı/oyuncu adı, tek Jiggle sayacı ve dişli korunur; eski yıldız sayacı veya kurtarma ilerleme çubuğu geri gelmez. Ev güncel QA Level1/0 Jiggle kaydını, koleksiyon/profil ise görevin örnek ilerleme kaydını gösterir; bu mockup seti tek bir kayıt dosyasının anlık görüntüsü değildir.

## 02 — Koleksiyon / 02b — Tablo detayı

| Parça | Durum | Ölçü @3x | 9-slice / tekrar |
|---|---|---:|---|
| gallery-wall-clean | **Yeni D:** `sources/koleksiyon-clean.png`, çerçevesiz sahne | 1125×2436 | Hayır; duvar dokusu kaydırılan içerik için tekrar hazırlanabilir |
| collection-cell / next-state | **Yeni D** dikey hücre; turkuaz kenar, küçük köşe noktası | 243×324 | Evet; 40 hücrede aynı |
| collection-hard-corner | Task20'de küçük kırmızı köşe işareti ayrılacak | yaklaşık45×45 | Hayır;19,23,25,30,35,36,38 |
| collection-label | bölüm numarası / Sıradaki | en fazla312 px genişlik | Metin ayrı; puan/yıldız satırı yok |
| detail-portrait-frame | **Yeni D:** `sources/detay.png` | yaklaşık660×810; gerçek resim540×720 | Çerçeve9-slice; resim hayır |
| detail-info/name | gerçek7. tablo adı: **Kemerli su yolu**; “Bölüm7'de kazanıldı ·0:42” | metin alanı840 px genişlik | Metin ayrı; dinamik süre/bölüm |
| detail-replay / close | ortak turkuaz tuş ve kırmızı X | yaklaşık690×174 /84×84 | Tuş yatay9-slice; X hayır |

40 kayıt ve zor durumları: [collection-model.json](task19d/collection-model.json). Dikey gerçek resimlerin kaynak kırpım/hedef konumları: [portraits.json](task19d/portraits.json).

## 03 — Jiggles

| Parça | Durum | Ölçü @3x | 9-slice / tekrar |
|---|---|---:|---|
| jiggles-family-room/shelf | **Yeni D:** `sources/jiggles.png`; krem/petrol oda, koyu ahşap raf | 1125×2436 sahne; raf alanı yaklaşık1080×900 | Hayır; oda/raf ayrı katmanlar çıkarılacak |
| jiggle-standing-blue/pink/red/green/purple | **Yeni D sahne**, kimlik dayanağı mevcut `Characters/Jiggles-reference-v02.png` ve Task15 avatarları | her biri yaklaşık210×330 | Hayır; iki ayrı kol/el ve iki ayrı bacak/ayak korunur |
| color-count-pill | yeni D beş krem hap | yaklaşık144×60 | Evet; rakamlar ayrı |
| five-avatar-circles | yeni D dairesel portreler; mavi seçili ince altın halka | yaklaşık165×165 | Hayır;03/05 avatar seçiminde tekrar |
| milestone-rail | yeni D kalın turkuaz şerit | yaklaşık900×120 | Evet, yatay |
| milestone-frame/badge/portrait | yeni D üç ayrı sembol, ince altın halka | yaklaşık156×156 | Hayır; yıldız/para ödülü değildir |

Toplam **1.248**: mavi328 + pembe277 + kırmızı242 + yeşil211 + mor190. Eşikler500/1.000/2.000, görsel ödüller Çerçeve/Rozet/Portre. Sayaç/ödül etiketleri ayrı metindir.

## 04 — Başarılar

| Parça | Durum | Ölçü @3x | 9-slice / tekrar |
|---|---|---:|---|
| trophy-room | mevcut: onaylı C `sources/basarilar-art-v2.png` | 1125×2436 | Hayır |
| achievement-tile | mevcut C krem yüz/turkuaz kabartma | yaklaşık945×294 | Evet; 16 kayıt için tekrar |
| medallion-picture/collection/bomb/speed/hat | mevcut C beş hacimli ikon, ince altın halka | yaklaşık210×210 | Hayır; kalan başarıların ikonları Task20 setine eklenir |
| progress-track/fill | mevcut C koyu rim/turkuaz dolgu | yaklaşık570×51 | Evet; dolgu kırpılır |
| completed-green-check / locked | mevcut C yeşil tik ve gri-lacivert kilit | yaklaşık69×69 | Hayır |
| achievement-description | **Yeni D düzeltmesi:** her açıklama tek satır | uzun cümle için630 px genişlik | Bomba cümlesi gerektiği kadar küçültülür |

Tam metinler: **“10 bombalı bölümü ilk denemede geç”**, **“10 bölümü yarı sürede bitir”**. Açıklama ortası satır başlangıcından42 pt, ilerleme ortası68,5 pt; arada açık boşluk vardır. Yıldız Avcısı yerine Hızlı Parmaklar/süratli kronometre korunur.

## 05 — Profil / 06 — Ayarlar

| Parça | Durum | Ölçü @3x | 9-slice / tekrar |
|---|---|---:|---|
| profile-shell/name-band/pencil | **Yeni D:** `sources/profil.png` | kabuk yaklaşık990×1680; isim bandı810×150 | Kabuk/bant9-slice, kalem hayır |
| profile-portrait-ring | yeni D ince altın avatar halkası | yaklaşık330×330 | Hayır |
| blue-profile-avatar | mevcut: `Gameplay/Task15/avatar-blue.png`, dairesel maske | 282×282 | Hayır; yeni karakter çizilmez |
| statistic-cell | yeni D altı krem-turkuaz hücre | yaklaşık411×195 | Evet; ikonlar/sayı/etiket ayrı |
| statistic-icons | mevcut yeni C sekme ikonları + Task15 Jiggle/saat/parça | 66×66 | Hayır |
| setting-shell | **Yeni D:** `sources/ayarlar.png` | yaklaşık990×1500 | Evet |
| setting-sound/music/vibration | yeni D üç hacimli ikon | yaklaşık186×186 | Hayır |
| toggle-on/off / knob | yeni D yeşil açık, gri-lacivert kapalı | yaklaşık195×108 /84×84 | İz yatay9-slice; düğme hayır |
| language-segment / selected | yeni D turkuaz/krem iki bölmeli kontrol | yaklaşık570×150 | Evet |
| privacy/contact buttons | ortak küçük turkuaz tuş ailesi | yaklaşık405×132 | Evet; etiketler ayrı |

Profil altı değeri: **Bölüm12/40**, **Kurtarılan Jiggle1.248**, **Toplam süre18 dk**, **İlk denemede%75**, **Başarı5/16**, **Ortalama kalan süre0:46**. Ayarlarda Ses/Müzik açık, Titreşim kapalı; Türkçe seçili, English alternatif; Gizlilik/Bize yaz, sürümv0.5.1. Task19D'nin açık talebiyle anahtar/segment düzeni, önceki C notundaki dikey metin tuşlarının yerine geçer.

## 07 — Bölümler

| Parça | Durum | Ölçü @3x | 9-slice / tekrar |
|---|---|---:|---|
| level-corridor/stone/carpet | **Yeni D:** `sources/bolumler.png` | 1125×2436 | Sahnede hayır; Task20'de yol/taş modülleri ayrı tekrar hazırlanır |
| node-complete | yeni D turkuaz-altın tok düğme | yaklaşık150×138 | Hayır |
| node-current / glow | yeni D parlak altın büyük düğme | yaklaşık180×168 /225×225 | Hayır |
| node-locked / big-lock | yeni D gri-lacivert düğme / büyük kilit | yaklaşık150×138 /66×72 | Hayır |
| node-hard-red | yeni D kırmızı kenarlı zor düğüm | yaklaşık150×144 | Hayır;19 örneği |
| hard-baron-portrait | mevcut: `Characters/Baron-reference-v01.png` öfkeli portreden dairesel kırpım | 93×93 | Hayır; ayrı temiz portre export'u gerekir |
| current-avatar | mevcut: `Gameplay/Task15/avatar-blue.png` | 102×102 | Hayır |
| milestone-portrait-frame | yeni D küçük dikey turkuaz çerçeve | yaklaşık126×168; iç resim90×120 | Çerçeve9-slice, resim hayır |
| back-arrow / node-numbers | ayrı yazı/ikon katmanı | ok yaklaşık72×72, sayılar ayrı | Hayır |

Görünür pencere9–20;9/10/11 bitmiş,12 güncel,13–18/20 kilitli,19 kırmızı kenar + Baron. 10/20 yanında gerçek dikey tablolar var. 40 düğüm ve10/20/30/40 kilometre taşları: [level-model.json](task19d/level-model.json).

## Katmanlar ve kontrol

- `docs/art/task19d/layers/<ekran>-art.png`: metinsiz kompozit. Yeni metinler `<ekran>-text.png` ve özgün Unicode/konum/punto JSON'undadır. Ev'in mevcut Jiggle sayısı/Level etiketi özgün QA piksellerinde pişmiştir; son Task20 export'unda bunlar da yazısızlaştırılacak. Oyuncu adı bu tur ayrı metne alındı.
- `docs/art/task19d/sources/`: **built-in image_gen** ile üretilmiş raster sahne/panel/çubuklar. [Tam prompt seti](task19d/prompts.json). Kod yeni düz UI yüzeyi çizmez; kaynak parçaları kırpar/9-slice yerleştirir, gerçek resmi ve yazıyı bindirir.
- Yeniden üretim: `python docs/art/build-task19d.py` (Pillow,NumPy). Önceki B/C tipografi fonksiyonları ve onaylı ikon kaynakları yalnız okunur. C#/Unity sahnesi/oyun verisi değiştirilmedi.
- Depodaki Fredoka-SemiBold, 1px optik kalınlaştırma ve fontun kendi Türkçe birleştirici glifleriyle kullanılır. Oyun entegrasyonunda tam Türkçe glifli Fredoka Bold gerekir.
- [validation.json](task19d/validation.json): dokuz ekran ölçüsü/SHA-256, güvenli alan, metin alt sınırları, ortak çubuk konumu; [portraits.json](task19d/portraits.json): 3:4 kırpımlar. HTML bağlantıları ve dokuzlu görsel kontak kontrol edilir. İlk sütundaki Ev görünüşü C onayını korur; alt çubuk ve tam isim günceldir.

Task20'de önce ortak nav/ikon, çerçeve, kabuk/tuş ve metin alanları ayrı temiz alfa/9-slice olarak hazırlanmalı; ardından sahneye bağlı raf/karakter/yol katmanları ayrılmalıdır. Bu aşamada atlas/import ayarı veya sahne entegrasyonu yapılmadı.

## Task20 üretim export'u

Task19D ekranları onaylandı ve Task20 üretim paketine dönüştürüldü. Fiili dosya adları, ölçüler, pivotlar ve 9-slice değerlerinde **[Task20 manifest.json](../../Assets/Art/Menu/Task20/manifest.json)** ve **[HANDOFF.md](../../Assets/Art/Menu/Task20/HANDOFF.md)** esas alınır; yukarıdaki tablolar tasarım aşamasının hedef planıdır. `isRuntime:false / category:preview` kayıtları oyuna import edilmez.

Task19D kaynak ekranlarında tablo10 işareti10 düğümünün yanına taşındı ve altındaki ek10 kaldırıldı;19 kırmızı kenar+Baron+kilitle kilitli gösterildi. Bomba açıklamasında44px sağ boşluk, Avatarım başlığının altında30px boşluk doğrulandı. Kaynak9ekran/kontak yenilendi.

Üretim sprite'ları yazısızdır; çerçevelerin içi şeffaftır. `portrait-placeholder` kazanılmamış resim için çerçeveden ayrı lacivert zemin, `hero-picture-mask` Ev resmi için ayrı maske sağlar. Basılı durumlar ayrı dosyadır. `levels-path-repeat` üst/alt RGBA kenarları eşit olan dikey tekrar parçasıdır. Üretim parçalarından kurulmuş9ekran: [preview-kontak.png](../../Assets/Art/Menu/Task20/preview-kontak.png). Katman/temiz sahne/9-slice nedeniyle oluşan görünüş farkları HANDOFF'ta açıkça belirtilmiştir. C#/prefab/sahne bağlantısı yapılmadı.
