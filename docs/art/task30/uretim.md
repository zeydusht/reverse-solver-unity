# Task30 · Katmanlı HUD, Jigo büstü ve engeller

[Önizleme](onizleme.png) · [Bağımsız ölçüm](olcum.md) · [HUD30](../../../Assets/Art/_yeni/HUD30/) · [Jigo v2](../../../Assets/Art/_yeni/Jigo_v2/) · [Engeller v2](../../../Assets/Art/_yeni/Obstacles_v2/)

## HUD

Paftanın ilk iki ekranı 390 × 844 pt ve aynı ölçekte. Kılavuzlar panel ve kemer hizasını, bordürü, tuş tepesini, alt şerit sınırını ve tuş merkezlerini işaretler. A yalnızca hizalı ekran ve Devam provasında örnek olarak kullanıldı. Sağdaki üç aday aynı içerikle gösteriliyor. [PM analizi](../HUD_ANALIZ_RM.md) · [Palet](../PALETTE.md)

| Aday | Dikey gradyan |
|---|---|
| A · Petrol turkuaz | `#0A3A4A` → `#138A9C` |
| B · Mavimsi | `#0B3550` → `#1786A8` |
| C · Yeşilimsi | `#0E4A47` → `#1A9A8C` |

- Üst dolgu 159 pt. Alt kenarda 1 pt ışık, 2 pt altın, 2 pt turuncu ve 3–4 pt yumuşak gölge var. `top-strip` bu katmanları 168 pt tuvalde taşır.
- Parça paneli (9, 37), Süre paneli (268, 37). İkisi de 113 × 118 pt. Dıştan içe açık camgöbeği, lacivert, turuncu, ışık, altın, koyu gradyan bant, bej bevel ve krem yüz. Kurdelenin ortası y=39, dış uçları y≈70. Başlıklar, parça ikonu, sayaçlar, seviye yazısı ve olası acil süre rengi ayrı katmanlar.
- Portre penceresi (131,5, 37), nominal 127 pt genişlik ve y=155 alt hiza. Beş bant ve gece moru → camgöbeği dolgu `portrait-window` içinde. Jigo'yu `portrait-clip` alfa kanalı ile kes ve pencerenin üstüne yerleştir. Kemerin tepesi bağımsız ölçümde y≈36–37. Çizim bu hizayı kullanıyor.
- Alt şerit y=771–844, 73 pt. Beş merkez x=42 / 118 / 195 / 272 / 348, y=786. Tuş dış çapı 62 pt, üstü y=755. Çukur ve gölge ayrı. Katman sırası turuncu, altın, gövde ışığı, gövde, iç disk, ikon. Ayarlar gövdesi mavi, dişli bağımsız. Basılı durumda gövde ve ikon 2 pt iner. Kilitli durumda kısık ikon ve küçük kilit kullanılır. Tükenmiş durumda kısık ikon ve canlı 0. Rozet dışarıdan gövdeye biner, rakam boş PNG'nin üstüne çizilir.
- Dört booster ikonu, mavi çekiç dahil, bağımsız kaynak PNG'lerden bayt düzeyinde aynı kopyalar. [Task28 boosterları](../../../Assets/Art/_yeni/Boosters/) · [Arşiv mavi çekiç](../task28/_arsiv/Z2-v3-mavi-cekic/booster-hammer.png)
- 374 × 374 pt oyun tahtası paftada x=8, y=268. Bölüm rozeti y=6'da, üstte pause yok. [Oyun arka planı](../../../Assets/_Game/Presentation/Resources/Art/game-background.png) · [Task26 tablo karesi](../task26/cekim-kareleri-v06/tablo-01.png)
- Devam kartı aynı katman sırası ve alt gölgeyle yeniden çizildi. Üst kurdele, fiyat yuvası, saat, para, tuş gövdeleri ve üzgün portre ayrı. Metin ve örnek fiyat paftada üst katmandan çizildi. Sevinç için Task29'daki sekiz tam boy kare ve efektler kullanıma hazır. [Task29 sevinç](../../../Assets/Art/_yeni/Jigo/sevinc/) · [Task29 Devam kaynağı](../../../Assets/Art/_yeni/Devam/)

## Jigo

Beş yeni büst yerleşik ImageGen ile Task26 model paftasından türetildi. Fırça ucu, yüksük, düğme kalkan ve turkuaz silüet korunarak yüz yakın planda tutuldu. Her hâlde altı farklı kare, 256 × 256 px RGBA. Ana pozlardan ara kareler göz, miğfer, ağız ve fırçaya bölgesel hareket uygulanarak üretildi. Son kareden ilk kareye geçiş piksel farkı üzerinden kontrol edildi. [Task26 model paftası](../task26/kahraman-final.png)

| Hâl | Hareket |
|---|---|
| Normal | Açık göz, hafif nefes, 04'te göz kırpma. |
| Tedirgin | İki gözde yan bakış, ter damlası, fırça eli. |
| Korku | Açık ağız, yuvarlak gözler, miğfer titremesi. |
| Üzgün | Sağ göze inen yüksük ve titreyen dudak. |
| Mutlu | Geniş gülüş, 04'te yükselen fırça. |

**Ortak istem girişi**

> One isolated single close-up toy-character game sprite of JIGO on genuine transparent RGBA alpha. Reference image is the official three-view model sheet: use ONLY its large FRONT Jigo on the LEFT to match identity. Faithfully retain turquoise jigsaw body with top tab, knob at right and recess at left, plump rounded toy sculpt, warm big brown eyes, exact softly dimpled polished silver sewing-thimble hat, wooden paintbrush with vivid BLUE paint at viewer LEFT, round ivory four-hole sewing-button shield at viewer RIGHT. TIGHT PORTRAIT BUST from top of hat to shoulders, visible hands and uppermost edge of shield; no legs, feet, lower body or full shield in frame. The character is framed LARGE and shoulders end at image lower edge as a deliberate bust cut. Dominant enlarged face in the central three quarters of width, eyes and mouth distinct, not shrunk to fit props. Only short blue brush tip, ferrule and upper handle peeking on viewer LEFT, keep close to cheek so face stays dominant. Shield peeks from LOWER RIGHT as a small partial foreground shape with holes, keep all facial features visible unless explicitly specified. Front view, upright, evenly balanced left and right in same geometry as model sheet, warm upper-left toy lighting, clean smooth detailed toy materials, strong clear large shapes readable at 96px.

| Hâl | İsteme eklenen ifade | ImageGen kaynağı |
|---|---|---|
| `normal` | Warm modest smile with two eyes evenly open and looking straight at viewer. Small visible tooth line and relaxed softly curved eyebrows. Clearly awake. Keep both eyes symmetrical for a later blink. | `exec-8a5af5a9-8911-4ee7-8ba5-8d98f8a2b717.png` |
| `tedirgin` | Pronounced anxious side-eye looking all the way toward image RIGHT with both irises offset, a high asymmetric raised eyebrow pose, a little pinched wavy grimace with clenched visible teeth, and one oversized simple readable pale blue sweat drop on the LEFT temple below the thimble (toward brush). Expression must be anxious, brows raised, not frightened or happy. | `exec-05eee302-89ef-4434-b1ff-72586267d5d4.png` |
| `korku` | Very exaggerated startled fright: enormous completely round white eyes with SMALL centered brown pupils and a very conspicuous tall dark open O-shaped screaming mouth, nearly as big as one eye, with tiny lower pink tongue. Both thick eyebrows visibly lifted, silver thimble slightly tilted, shield low enough to reveal the entire screaming mouth and BOTH eyes. Fright, not a smile. | `exec-f3669ba2-6876-421e-bcdc-6528854afb89.png` |
| `uzgun` | Very exaggerated sadness: the same silver thimble physically slipped down on the viewer's RIGHT so its metal lower brim partially covers that eye, visibly tilted and overlapping its top third. Visible eye on left looks DOWN with drooping upper lid, inner eyebrows sad, broad upside-down curve mouth with protruding lower lip. No tear, no sweat. Clearly sad, not frightened or angry. | `exec-ad92988b-b2f7-43ae-acef-11c35f805f51.png` |
| `mutlu` | Joyous big laugh with a wide toothy crescent smile, pink tongue, both eyes squeezing into clearly visible upturned happy arcs. Blue tipped brush raised high close to left side of silver thimble, paint tip beside its crown, brush vertical and close to body. Recognizably delighted, not startled. | `exec-7f3f85fa-5cd0-43fb-87b4-4cc18b7bf1ce.png` |

**Ortak istem sonu**

> Keep helmet, eyes and mouth all fully inside canvas unless helmet is requested to overlap eye. Transparent outside silhouette; no circle, no window, no border, no extra props or background, no labels or typography, no color chips, no other faces. Square 1:1 composition.

## Engeller

- Kilitler altın yüz, balmumu veya ahşap ve altın olarak katmanlı çizildi. B adayında Kont'un kıvrık perçemi yeni bir kabartma mühür işareti olarak yorumlandı. Kapalı karede canlı sayı merkezi A/C için normalize (0,50, 0,66), B için (0,50, 0,68). Sıra `closed` → `open-01…04` → `fall-01…03` → gizle. Açılmada yalnız kulp hareket eder. Düşüşte açık silüet aşağıya ve sağa döner, küçülür ve söner. `body` ile beş `shackle-*` dosyası animasyon katmanlarıdır. [Kont pozları](../task26/kont-pozlar.png)
- Task17 v3 üretim geometrisindeki ön halka, arkadaki bağlantı halkası ve yatay karo tekrarı çıkarılarak tek sprite olarak ayrıldı. Dört onaylı renk korundu, yaprak işaretli beşinci yuva için yeşil halka aynı geometriyle üretildi. Beş merkez ankrajı aynı işaretleri koruyor. 64 pt hücre provasındaki eski ve yeni halka görünür yüksekliği `9 → 18 pt` (alfa ≥128), **2 kat**. Yeni halka 38 × 22 pt tuval, yaklaşık 26 pt adım ve dönüşümlü %70 yükseklikle döşendi. [Task17 kaynakları](../../../Assets/Art/Gameplay/Task17/) · [Task28F eski zincir ve bomba](../../../Assets/Art/_yeni/Obstacles/)
- Bomba için normal ve acil ayrı sekizer kıvılcım karesi. Eski 384 × 384 px gövde, 576 × 576 px efekt alanına (96, 96) konur. İki katmanın merkez pivotu aynı, fitil kökü efekt alanında (328, 115). Yeni sıcak sarı → turuncu hale, çekirdek ve kıvılcımlar efekt PNG'sinde. Acil katman 3–2–1'de daha büyük ve parlak. Alfa ≥180 ile sekiz karenin medyan çekirdek ve parçacık kutusu ölçüldü. Eski 38 × 22 px, normal 90,5 × 50 px, acil 121 × 66,5 px. Normal hâl iki eksende 2,38 ve 2,27 kat büyüdü.

## Dosya boyutları ve doğrulama

| Klasör | Dosyalar ve ölçüler |
|---|---|
| `HUD30/a`, `b`, `c` | Aday başına `top-strip` 1170 × 504, `bottom-strip` 1170 × 219, `counter-panel` 339 × 366 (4 pt gölge payı dahil), `counter-ribbon` 351 × 117 px |
| `HUD30/shared` | Portre pencere ve maske 381 × 363, çukur 222 × 222, rozet 84 × 84, dişli 138 × 138, kilit işareti 72 × 72, sekiz tuş durumu 216 × 216 px |
| `HUD30/icons` | Dört booster 240 × 240, parça ikonu 120 × 120, bölüm rozeti 216 × 216 px |
| `HUD30/devam` | Kart 978 × 1350, kurdele 744 × 117, fiyat yuvası 405 × 126, saat ve para 120 × 120, iki tuş 684 × 336 px |
| `Jigo_v2` | 5 × 6 kare, 256 × 256 px |
| `Obstacles_v2/padlock-{a,b,c}` | Her adayda sekiz animasyon ve altı kaynak katmanı, 384 × 384 px |
| `Obstacles_v2/chain` | Beş halka 128 × 64 ve beş ankraj 160 × 160 px |
| `Obstacles_v2/bomb` | Normal ve acil sekizer kare, 576 × 576 px |
| `onizleme.png` | 2304 × 2300 px. Oyun ekranları 390 × 844 pt, Jigo kareleri 96 pt, engel örnekleri 64 pt. |

39 HUD, 30 portre ve 68 engel PNG'si doğrulandı. Kaynak booster ikonları eş, portre ve kıvılcım dizilerindeki kareler farklı, Task17 halkanın içi şeffaf. Metin ve rakamlar paftanın bileşiminde görünür ve kaynak sprite'lara gömülmedi. Bu teslim yerel görsel prova. Aday seçimi ve oyun entegrasyonu bekliyor.
