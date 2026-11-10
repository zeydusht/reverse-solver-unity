# T42 · Booster gösterimleri ve efektler

[Tek pafta](onizleme.png) · [TASK42](../CODEX_TASK42.md) · [Royal Match eki](../CODEX_TASK42_EK.md)

**178 RGBA PNG**. Paftada 390 × 844 px boyutunda 16 oyun maketi var. İlk sıra efekt ve seçim modlarını, ikinci sıra saatin altı adımını, üçüncü sıra makasın dört adımını gösterir.

## Referans ölçüleri

- Çekiçte 1,07–1,20 sn karelerine bakıldı. 1,13 sn karesindeki beyaz-sarı bölge yaklaşık 61 px, hücre adımı yaklaşık 19–20 px. T42 darbe tepesindeki açık renkli ışık 604 px, yani **2,96 hücre**. 54 px hücreyle paftada yaklaşık 160 px. Şok halkası ve seyrek dış kıvılcımlar bu çekirdeğin dışına yayılır. [Çekiç referansı](../booster%20ref/kareler/hammer.png)
- Jester karesinde Shuffle! yaklaşık 92 px / 180 px ekran genişliği. T42 Değiştir! görünür alanı yaklaşık **198 × 90 px** / 390 px. [Onay referansı](../booster%20ref/kareler/jester.png)
- Seçim karartması %72–73 alfa. Çekiç ve makasta tahta penceresi açık. Değnekte dört bölge için T41 çerçeveleri kullanılır. Seçili alt tuşu karartmanın üzerine çizip 6 karelik halkayı üstüne bindir. [Referans analizi](../booster%20ref/kareler/BOOSTER_REF_ANALIZ.md)

En geniş animasyon karesindeki alfa ≥ 80 piksel aralığı ölçüldü. T41 ve T42 aynı piksel ölçeğinde karşılaştırıldı.

| Efekt | T41 genişlik | T42 genişlik | Oran |
|---|---:|---:|---:|
| Parça çıkışı | 214 px | 608 px | 2,84× |
| Engel dağılması | 169 px | 552 px | 3,27× |
| Değnek patlaması | 234 px | 666 px | 2,85× |
| Çekiç darbesi | 208 px | 691 px | 3,32× |
| Makas kırpıntısı | 202 px | 530 px | 2,62× |
| Değnek yıldız izi | 379 px | 782 px | 2,06× |

## Oynatma ve katmanlama

| Akış | Zamanlama ve yerleşim |
|---|---|
| Seçim | Üstte büyük ikon, boş başlık rozeti ve bir satır canlı talimat. Seçili tuş halkası 12 FPS döngü. Hedef/onaydan sonra karartmayı yaklaşık 0,10 sn’de kaldır. Saat doğrudan çalışır. |
| Çekiç | Tuşun merkezinden kavisle çık. 0,00–0,40 sn arasında görünür çapı yaklaşık 2× yap ve ayrı gölgeyi hedefe taşı. 0,27–0,40 sn kalkış, 0,40–0,47 sn iniş, 0,47 sn darbe. 03’te 33–50 ms duraklama ve kısa sarsıntı. 0,67–1,00 sn sekme ve dönerek sağ alta düşüş. |
| Makas | Tuştan seçilen bağın yakın kenarına geç. 0,30 sn’de 4 çıt çıt karesi. İzi gidiş doğrultusuna çevir. Cut-line dizisinin 01’i sağlam başlangıç, 02–06 kareleri soldan sağa kesim. Kırpıntıyı hareket eden kesim ucuna bağla. Kesim yaklaşık 0,20 sn, aşağı kayıp sönme yaklaşık 0,35 sn. |
| Değnek | T41 çerçevesini her çeyreğe ayrı yerleştir. Bölge seçildikten sonra canlı Değiştir! düğmesini göster. Onaydan yaklaşık 0,4 sn sonra değnek varır. Bölge merkezine dokunma patlamasını ve yıldız izini bindir. 0,7–1,0 sn içinde bölgedeki parçalar yer değiştirir, 1,2–1,5 sn’de değnek çıkar. |
| Saat | Düğme bırakılır. Serbest saat tuştan ele yükselir ve tutuş katmanına geçer. Uzanma sonrasında süre donar, iki tutma karesi döner. Bar boşaldığında üç geri çekilme karesinden sonra saat bırakılır, düğme geri alınır ve idle’a dönülür. |
| Patlamalar | 768 × 768 px tuvalde (384, 384) efekt merkezi, 204 px bir hedef hücre. Parça, kırılma, değnek ve makas efektleri 9 kare. İlk iki kare hızlı büyür. Birlikte oynatılan nötr konfeti ve altın ışıkta yalnız konfetiyi boyayabilirsin. |

Saat anahtar kareleri 852 × 630 px, 390 × 844 px M1 maketinde sol üstü (106, 35) px. Anahtar kareler 1:3 ölçekte hazırlandı. Önce arka omuz ve üst kolu, sonra pencere maskesindeki gövdeyi, ardından ön kol ve eli çiz. Tam uzanma maketindeki omuz, dirsek ve bilek sırasıyla (231, 126), (254, 151) ve (282, 171) ekran px.

Saat barı makette (271, 186) px, görünür genişlik yaklaşık 112 px. Kaynak dolguyu barın üstüne (12, 12) px ofsetle çiz. Dolguyu genişliğinden kırp. Krem kanal çerçevenin içinde.

| Saat kolu katmanı | Giriş pivotu (yerel px) | Çıkış pivotu (yerel px) |
|---|---:|---:|
| shoulder | (38.71, 52.92) | (139.16, 162.19) |
| upper-arm | (33.81, 30.87) | (134.26, 140.14) |
| forearm | (25.48, 83.79) | (132.3, 68.11) |
| hand-clock | (25.97, 337.61) | — |
| free-grip | (25.97, 90.16) | — |

## Dosya listesi

Kare numaraları oynatma sırasıdır. — işareti tek katman veya olayla seçilen poz anlamına gelir. Kare dizisi bağlantısı ilk kareye gider.

### Seçim modu · 13 PNG

| Dosya | Boyut (px) | Kare | FPS | Katman ve kullanım |
|---|---:|---:|---:|---|
| [title-badge.png](../../../Assets/Art/_yeni/T42/Selection/title-badge.png) | 704 × 164 | 1 | — | Boş mavi/altın başlık. Booster adı ve tek satır talimat canlı. |
| [Icons/scissors.png](../../../Assets/Art/_yeni/T42/Selection/Icons/scissors.png) | 440 × 440 | 1 | — | Üst seçim modu için büyük ikon. Saat rozeti tanıtım amaçlı kullanılabilir. |
| [Icons/wand.png](../../../Assets/Art/_yeni/T42/Selection/Icons/wand.png) | 440 × 440 | 1 | — | Üst seçim modu için büyük ikon. Saat rozeti tanıtım amaçlı kullanılabilir. |
| [Icons/hammer.png](../../../Assets/Art/_yeni/T42/Selection/Icons/hammer.png) | 440 × 440 | 1 | — | Üst seçim modu için büyük ikon. Saat rozeti tanıtım amaçlı kullanılabilir. |
| [Icons/clock.png](../../../Assets/Art/_yeni/T42/Selection/Icons/clock.png) | 440 × 440 | 1 | — | Üst seçim modu için büyük ikon. Saat rozeti tanıtım amaçlı kullanılabilir. |
| [selected-ring/01–06.png](../../../Assets/Art/_yeni/T42/Selection/selected-ring/01.png) | 384 × 384 | 6 | 12 | Seçili tuşun üstüne merkezle. Parlama ve dönen kıvılcımlar, döngü. |
| [dim-board-clear.png](../../../Assets/Art/_yeni/T42/Selection/dim-board-clear.png) | 1170 × 2532 | 1 | — | 1170 × 2532 M1 örneği. %72 alfa, 6×6 tahta için saydam pencere. |
| [dim-all.png](../../../Assets/Art/_yeni/T42/Selection/dim-all.png) | 1170 × 2532 | 1 | — | %73 karartma, saydam tahta penceresi olmadan. Değnek ilk aşamasında. |

### Boosterlar · 69 PNG

| Dosya | Boyut (px) | Kare | FPS | Katman ve kullanım |
|---|---:|---:|---:|---|
| [Wand/zone-idle.png](../../../Assets/Art/_yeni/T42/Boosters/Wand/zone-idle.png) | 384 × 384 | 1 | — | Onaylı T41 dört bölge seçimi katmanı. |
| [Wand/zone-selected.png](../../../Assets/Art/_yeni/T42/Boosters/Wand/zone-selected.png) | 384 × 384 | 1 | — | Onaylı T41 dört bölge seçimi katmanı. |
| [Wand/scrim-nw.png](../../../Assets/Art/_yeni/T42/Boosters/Wand/scrim-nw.png) | 512 × 512 | 1 | — | Onaylı T41 dört bölge seçimi katmanı. |
| [Wand/scrim-ne.png](../../../Assets/Art/_yeni/T42/Boosters/Wand/scrim-ne.png) | 512 × 512 | 1 | — | Onaylı T41 dört bölge seçimi katmanı. |
| [Wand/scrim-sw.png](../../../Assets/Art/_yeni/T42/Boosters/Wand/scrim-sw.png) | 512 × 512 | 1 | — | Onaylı T41 dört bölge seçimi katmanı. |
| [Wand/scrim-se.png](../../../Assets/Art/_yeni/T42/Boosters/Wand/scrim-se.png) | 512 × 512 | 1 | — | Onaylı T41 dört bölge seçimi katmanı. |
| [Wand/scrim-all.png](../../../Assets/Art/_yeni/T42/Boosters/Wand/scrim-all.png) | 512 × 512 | 1 | — | Onaylı T41 dört bölge seçimi katmanı. |
| [Wand/confirm-normal.png](../../../Assets/Art/_yeni/T42/Boosters/Wand/confirm-normal.png) | 684 × 336 | 1 | — | Onaylı yeşil tuş yüzü. Canlı etiket Değiştir! |
| [Wand/confirm-pressed.png](../../../Assets/Art/_yeni/T42/Boosters/Wand/confirm-pressed.png) | 684 × 336 | 1 | — | Onaylı yeşil tuş yüzü. Canlı etiket Değiştir! |
| [Wand/tool/01.png](../../../Assets/Art/_yeni/T42/Boosters/Wand/tool/01.png) | 512 × 512 | 1 | — | T41 değnek pozu. Hareket ve yörünge kodda. |
| [Wand/tool/02.png](../../../Assets/Art/_yeni/T42/Boosters/Wand/tool/02.png) | 512 × 512 | 1 | — | T41 değnek pozu. Hareket ve yörünge kodda. |
| [Wand/tool/03.png](../../../Assets/Art/_yeni/T42/Boosters/Wand/tool/03.png) | 512 × 512 | 1 | — | T41 değnek pozu. Hareket ve yörünge kodda. |
| [Hammer/flight/01–04.png](../../../Assets/Art/_yeni/T42/Boosters/Hammer/flight/01.png) | 512 × 512 | 4 | 15 | Tuş merkezinden hedefe kavis. Tuş boyutundan 2× büyüt; ayrı gölge. |
| [Hammer/strike/01–03.png](../../../Assets/Art/_yeni/T42/Boosters/Hammer/strike/01.png) | 512 × 512 | 3 | 30 | Kaldır, indir, vur. Sap pivotu (256, 256) px. |
| [Hammer/rebound-fall/01–04.png](../../../Assets/Art/_yeni/T42/Boosters/Hammer/rebound-fall/01.png) | 512 × 512 | 4 | 12 | Sekme ve dönerek sağ alta düşüş. Konum, açı ve alfa geçişi kodda. |
| [Hammer/soft-shadow.png](../../../Assets/Art/_yeni/T42/Boosters/Hammer/soft-shadow.png) | 384 × 176 | 1 | — | Uçuşta çekicin altına. Yerden yükseldikçe genişlet, alfayı azalt. |
| [Hammer/crack/01.png](../../../Assets/Art/_yeni/T42/Boosters/Hammer/crack/01.png) | 320 × 320 | 1 | — | T41 çatlak katmanı, darbe durumuna göre. |
| [Hammer/crack/02.png](../../../Assets/Art/_yeni/T42/Boosters/Hammer/crack/02.png) | 320 × 320 | 1 | — | T41 çatlak katmanı, darbe durumuna göre. |
| [Scissors/walk/01–04.png](../../../Assets/Art/_yeni/T42/Boosters/Scissors/walk/01.png) | 512 × 512 | 4 | 14 | 4 kare çıt çıt döngüsü. Ekran kenarından seçili bağa 0,3 sn. |
| [Scissors/exit/01–03.png](../../../Assets/Art/_yeni/T42/Boosters/Scissors/exit/01.png) | 512 × 512 | 3 | 12 | Kesimden sonra aşağı kayış. Yörünge ve alfa oyun tarafından sürülür. |
| [Scissors/cut-line/01–06.png](../../../Assets/Art/_yeni/T42/Boosters/Scissors/cut-line/01.png) | 768 × 184 | 6 | 25 | Tek bağ. Kesik çizgi soldan sağa söner. Sağ uç parlamasını son karede soldur. |
| [Scissors/speed-trail.png](../../../Assets/Art/_yeni/T42/Boosters/Scissors/speed-trail.png) | 608 × 208 | 1 | — | Hız çizgileri ve mavi hareket izi. X ekseni hareket yönü. |
| [Wand/touch-burst/01–09.png](../../../Assets/Art/_yeni/T42/Boosters/Wand/touch-burst/01.png) | 768 × 768 | 9 | 16 | Bölge merkezine parlak dokunma patlaması. İki karede büyür, sonra dağılır. |
| [Wand/star-trail.png](../../../Assets/Art/_yeni/T42/Boosters/Wand/star-trail.png) | 845 × 475 | 1 | — | Renkli kıvılcım ve altın yıldız izi. Taşı / döndür / söndür. |
| [Hammer/impact/01–09.png](../../../Assets/Art/_yeni/T42/Boosters/Hammer/impact/01.png) | 768 × 768 | 9 | 30 | Beyaz-sarı darbe ve şok dalgası. 02–04 ana tepe. 03 kare darbe anı için. |
| [Hammer/impact-freeze.png](../../../Assets/Art/_yeni/T42/Boosters/Hammer/impact-freeze.png) | 768 × 768 | 1 | — | Darbe karesi. 33–50 ms tut; hedef parça ve tahtayı kısa sars. |
| [Scissors/cut-bits/01–09.png](../../../Assets/Art/_yeni/T42/Boosters/Scissors/cut-bits/01.png) | 768 × 768 | 9 | 16 | Bağ kesildiğinde kumaş / kâğıt kırpıntısı, parlak kıvılcım ve hafif toz. |

### Jigo ve saat · 24 PNG

| Dosya | Boyut (px) | Kare | FPS | Katman ve kullanım |
|---|---:|---:|---:|---|
| [Layers/body-lean.png](../../../Assets/Art/_yeni/T42/Clock/Layers/body-lean.png) | 351 × 365 | 1 | — | Sağa bakan, fırçalı ve kalkansız yeni Jigo büstü. |
| [Layers/button-shield.png](../../../Assets/Art/_yeni/T42/Clock/Layers/button-shield.png) | 294 × 300 | 1 | — | Bırakılıp geri alınan krem dört delikli düğme. |
| [Layers/sweat.png](../../../Assets/Art/_yeni/T42/Clock/Layers/sweat.png) | 126 × 151 | 1 | — | Tutma sırasında iki ter damlası. Alfa / ofsetle canlandır. |
| [Layers/shoulder.png](../../../Assets/Art/_yeni/T42/Clock/Layers/shoulder.png) | 151 × 167 | 1 | — | Rig parçası. Giriş pivotu (38.71, 52.92) px. Çıkış pivotu (139.16, 162.19) px. |
| [Layers/upper-arm.png](../../../Assets/Art/_yeni/T42/Clock/Layers/upper-arm.png) | 198 × 202 | 1 | — | Rig parçası. Giriş pivotu (33.81, 30.87) px. Çıkış pivotu (134.26, 140.14) px. |
| [Layers/forearm.png](../../../Assets/Art/_yeni/T42/Clock/Layers/forearm.png) | 161 × 146 | 1 | — | Rig parçası. Giriş pivotu (25.48, 83.79) px. Çıkış pivotu (132.3, 68.11) px. |
| [Layers/hand-clock.png](../../../Assets/Art/_yeni/T42/Clock/Layers/hand-clock.png) | 330 × 400 | 1 | — | Rig parçası. Giriş pivotu (25.97, 337.61) px. |
| [Layers/free-grip.png](../../../Assets/Art/_yeni/T42/Clock/Layers/free-grip.png) | 220 × 152 | 1 | — | Rig parçası. Giriş pivotu (25.97, 90.16) px. |
| [Layers/flying-clock.png](../../../Assets/Art/_yeni/T42/Clock/Layers/flying-clock.png) | 216 × 250 | 1 | — | Alt şeritten serbest uçan sıcak ahşap saat. |
| [Layers/rise-trail.png](../../../Assets/Art/_yeni/T42/Clock/Layers/rise-trail.png) | 404 × 556 | 1 | — | Saatin yükselişine altın kıvılcım. İsteğe göre saat yörüngesine döndür. |
| [Layers/bar-frame.png](../../../Assets/Art/_yeni/T42/Clock/Layers/bar-frame.png) | 456 × 100 | 1 | — | T41 altın çerçeve ve krem süre kanalı. |
| [Layers/bar-fill.png](../../../Assets/Art/_yeni/T42/Clock/Layers/bar-fill.png) | 432 × 74 | 1 | — | T41 mavi dolgu. 12,12 px ofset, sağdan kırp. |
| [Keys/idle.png](../../../Assets/Art/_yeni/T42/Clock/Keys/idle.png) | 852 × 630 | 1 | — | Onaylı Jigo, fırça ve düğme ile başlar ve burada biter. |
| [Keys/release-button.png](../../../Assets/Art/_yeni/T42/Clock/Keys/release-button.png) | 852 × 630 | 1 | — | Düğme sağ alt köşeye bırakılır. Düğmeyi aşağı çek, sonra saydamlaştır. |
| [Keys/take-clock.png](../../../Assets/Art/_yeni/T42/Clock/Keys/take-clock.png) | 852 × 630 | 1 | — | Tuşundan yükselen saat burada ele geçer. Uçan saat katmanını gizle. |
| [Keys/reach/01–03.png](../../../Assets/Art/_yeni/T42/Clock/Keys/reach/01.png) | 852 × 630 | 3 | 12 | Gövdeden kol uzanır, saat Süre panelinin yanına gelir. |
| [Keys/hold/01–02.png](../../../Assets/Art/_yeni/T42/Clock/Keys/hold/01.png) | 852 × 630 | 2 | 8 | İki karelik hafif zorlanma ve ter döngüsü. Süre barı ayrı. |
| [Keys/retract/01–03.png](../../../Assets/Art/_yeni/T42/Clock/Keys/retract/01.png) | 852 × 630 | 3 | 12 | Saat aşağı inerken kol madalyona döner. Üçüncü karede saat bırakılır. |
| [Keys/recover-button.png](../../../Assets/Art/_yeni/T42/Clock/Keys/recover-button.png) | 852 × 630 | 1 | — | Düğme geri alınır, idle ile tamamlanır. |

### Haz efektleri · 72 PNG

| Dosya | Boyut (px) | Kare | FPS | Katman ve kullanım |
|---|---:|---:|---:|---|
| [PiecePop/light/01–09.png](../../../Assets/Art/_yeni/T42/Juice/PiecePop/light/01.png) | 768 × 768 | 9 | 16 | Beyaz-altın yıldız, sıcak hale ve şok halkası. Sabit ışık katmanı. |
| [PiecePop/tint-neutral/01–09.png](../../../Assets/Art/_yeni/T42/Juice/PiecePop/tint-neutral/01.png) | 768 × 768 | 9 | 16 | RGB nötr konfeti. Parça rengiyle boya ve light ile üst üste oynat. |
| [PiecePop/turquoise/01–09.png](../../../Assets/Art/_yeni/T42/Juice/PiecePop/turquoise/01.png) | 768 × 768 | 9 | 16 | Işıkla birleşik, parça rengine göre konfeti. |
| [PiecePop/green/01–09.png](../../../Assets/Art/_yeni/T42/Juice/PiecePop/green/01.png) | 768 × 768 | 9 | 16 | Işıkla birleşik, parça rengine göre konfeti. |
| [PiecePop/pink/01–09.png](../../../Assets/Art/_yeni/T42/Juice/PiecePop/pink/01.png) | 768 × 768 | 9 | 16 | Işıkla birleşik, parça rengine göre konfeti. |
| [PiecePop/purple/01–09.png](../../../Assets/Art/_yeni/T42/Juice/PiecePop/purple/01.png) | 768 × 768 | 9 | 16 | Işıkla birleşik, parça rengine göre konfeti. |
| [PiecePop/red/01–09.png](../../../Assets/Art/_yeni/T42/Juice/PiecePop/red/01.png) | 768 × 768 | 9 | 16 | Işıkla birleşik, parça rengine göre konfeti. |
| [Fracture/01–09.png](../../../Assets/Art/_yeni/T42/Juice/Fracture/01.png) | 768 × 768 | 9 | 16 | Kilit / zincir kırılması. Altın ve mor metal, kıvılcım ve sıcak toz. |

## Çizim kaynakları

- Üst ve alt HUD M1 pafta bileşiminde mevcut tasarımlarla kuruldu. Dört bölgenin çerçevesi, saat barı ve çekiç çatlakları T41’den alındı. Değnek onayında mevcut yeşil tuş gövdesi kullanıldı. [T41 teslimi](../task41/LISTE.md) · [HUD40 başlıkları](../../../Assets/Art/_yeni/HUD40/Headers/)
- Jigo büstü, eklemli saatli kol ve düğme kalkanı yerleşik imagegen ile üretildi. Çizimler katmanlara ayrıldı ve anahtar pozlar bu katmanlardan birleştirildi. [Jigo v3](../../../Assets/Art/_yeni/Jigo_v3/) · [Saat referansı](../../../Assets/Art/_yeni/HUD36/Boosters/clock-a.png)

<details>
<summary>Imagegen istemleri</summary>

**Jigo büstü**

Use case: identity-preserving game sprite. Generate a single genuinely transparent square PNG of the EXACT turquoise puzzle painter Jigo in input 1, for a masked in-game portrait rig. Preserve the same original silhouette, front-facing puzzle tab on right side, very short soft turquoise fabric limbs, gold-brown eyes, dimensional dimpled bronze tailor's thimble, and painter's wooden gold/blue paintbrush held in his own right hand on the VIEWER'S LEFT. Change the pose to him leaning a little toward viewer-right and glancing toward that side with gently furrowed eyebrows, a small determined closed smile, holding the brush near his left edge. Remove the entire cream button shield and the hand holding that shield. His other arm is a separately rigged element, so show only a modest right-hand side of the turquoise torso, clean and fully drawn down to the bottom, with no hand or protruding arm on viewer right (the jigsaw tab on viewer right is part of his body and remains). Do not change toy proportions, color or identity. Bust crop just below chest as in reference, entire thimble and brush upper tip in frame. Soft clean upper-left light, game sprite polished shaded felt, no outlines. No other prop, no clock, no button, no text, no painted backdrop, no white background, actual RGBA transparency. Center the bust with generous side margins and slightly more canvas underneath.

**Saatli kol**

Use case: stylized game asset. Generate one genuinely transparent PNG of ONLY Jigo's own left arm holding his alarm clock, a short anatomically readable soft toy arm with a visible flexed elbow and a distinct short forearm, for rigging from an upper HUD portrait. Input 1 is exact style/teal fabric/hand reference. Input 2 is clock reference: preserve the clearly readable warm walnut clock casing, gold twin bells, cream face, chocolate hands. Horizontal sprite, upper arm starts on the LEFT, short thick upper arm descends at a 30-degree diagonal toward the elbow, and short forearm bends up to the right about 30 degrees, to a round bare turquoise hand supporting the clock from its bottom left. Wrist is visible just below the thumb. Hand has four short soft rounded fingers, all genuine same turquoise felt and small, with a thumb visible gripping the lower left of the walnut case. Clock on right half faces front, roughly upright, about as tall as the whole arm horizontally. Upper arm and forearm must each have short compact volumes, a subtly evident elbow, matching toy body; avoid long hose, tentacle, exaggerated curve, noodle or floating disconnected hand. Cozy bright upper-left studio lighting, rich saturated teal matching the reference, clean edges. The arm silhouette from cut shoulder end to wrist remains distinct and isolated. All shapes within central 85% with fully transparent margins. Exactly one arm and one clock, no body, no stand, no other elements, no lettering. Actual RGBA transparency.

**Düğme kalkanı**

Use case: transparent game prop for a rig. Generate ONE isolated perfectly round cream/biscuit button shield matching the button held by the turquoise painter Jigo in reference 1 and reference 2. These images are visual references only. The button is a very simple softly beveled pale butter and cream sewing button with 4 holes in a square 2 by 2 pattern, one concentric slightly raised broad rim and softly molded grain, cozy 3D toy style. Front view, at most subtle tilt, pale warm cookie texture, light from top left. Exactly four true cut-through transparent small holes with shaded warm edges. Round button complete with clean silhouette and generous canvas margins. No character, no hands, no other objects, no text, no black rim, no cast shadow, no aura, no backdrop, genuinely transparent RGBA background.

</details>
