# Task31 · HUD v3 teslim denetimi

[Yan yana pafta](onizleme.png) · [Referans ekranı](../Reference/rm_oyun_ekrani.png) · [Alt şerit kırpması](ref_alt.png) · [Sağ panel kırpması](ref_moves.png) · [Rozet kırpması](ref_rozet.png)

**M2, 67/67 renk örneğinde geçti. En büyük kanal farkı 11.** M1 aynı geometriyi ve sıcak renkleri kullanır. M1 için verilen mavi ve krem paletinde 17 satır referansın ±12 eşiğini aşıyor. Bu satırlar tabloda palet farkı olarak ayrıldı.

Önce 1170 × 2532 px kaynakta temiz kesitler belirlendi ve örnek koordinatları sabitlendi. Sağdaki sayılar (x, y) piksel koordinatıdır. Pillow ile kaynak ve yeni katmanların 1:1 RGB değerleri okundu. Renk örnekleri karakter, ikon ve canlı metin altındaki malzeme bileşiminden alındı. Paftadaki üçlü kırpmalarda her satır için aynı bölge ve ölçek kullanıldı. Δ, mutlak R/G/B farkıdır. Telefon ölçeği 3 px = 1 pt.

Ölçülen referans rozetinin kırmızısı brif tablosundan daha doygun. İç gövde ortada #E90000, üst parlama #FE5557. Üst şerit bordüründeki sarı tepe y=484 ve #FFD65A. Bunlar iki adayda da kaynak ekranın piksel değerlerine göre ayarlandı.

## Renk tablosu

| Grup | Örnek | M2 en büyük Δ | M1 palet farkı |
|---|---:|---:|---:|
| Booster | 16 | 11 | 2 |
| Rozet | 7 | 1 | 0 |
| Ayarlar | 13 | 10 | 2 |
| Sağ panel | 14 | 1 | 6 |
| Orta pencere | 7 | 2 | 2 |
| Şerit bordürü | 10 | 3 | 5 |

| Malzeme katmanı | (x, y) px | Referans | M1 | Δ R/G/B | M2 | Δ R/G/B | M1 sonucu |
|---|---:|---|---|---|---|---|---|
| Booster · Turuncu · üst | 126, 2265 | `#FF6A00` | `#FF6B00` | 0/1/0 | `#FF6B00` | 0/1/0 | Geçti |
| Booster · Turuncu · yan | 33, 2359 | `#FF6400` | `#FF6500` | 0/1/0 | `#FF6500` | 0/1/0 | Geçti |
| Booster · Turuncu · alt | 126, 2451 | `#FF4C00` | `#FF4D01` | 0/1/1 | `#FF4D01` | 0/1/1 | Geçti |
| Booster · Altın · üst | 126, 2271 | `#FFC000` | `#FFC100` | 0/1/0 | `#FFC100` | 0/1/0 | Geçti |
| Booster · Altın ışığı · üst | 126, 2268 | `#FFCF00` | `#FFCB00` | 0/4/0 | `#FFCB00` | 0/4/0 | Geçti |
| Booster · Altın · yan | 41, 2359 | `#FFAC00` | `#FFAC00` | 0/0/0 | `#FFAC00` | 0/0/0 | Geçti |
| Booster · Altın ışığı · yan | 37, 2359 | `#FFC128` | `#FFC128` | 0/0/0 | `#FFC128` | 0/0/0 | Geçti |
| Booster · Koyu yeşil kenar | 46, 2359 | `#034F00` | `#034F00` | 0/0/0 | `#034F00` | 0/0/0 | Geçti |
| Booster · Yeşil gövde · üst | 126, 2281 | `#A9DD3E` | `#A9DD3E` | 0/0/0 | `#A9DD3E` | 0/0/0 | Geçti |
| Booster · Yeşil gövde · orta | 51, 2359 | `#008C00` | `#008C00` | 0/0/0 | `#008C00` | 0/0/0 | Geçti |
| Booster · Yeşil gövde · alt | 126, 2434 | `#005C00` | `#005C00` | 0/0/0 | `#005C00` | 0/0/0 | Geçti |
| Booster · İç disk · üst | 126, 2290 | `#70D600` | `#70D600` | 0/0/0 | `#70D600` | 0/0/0 | Geçti |
| Booster · İç disk · orta | 63, 2359 | `#38A400` | `#38A400` | 0/0/0 | `#38A400` | 0/0/0 | Geçti |
| Booster · İç disk · kenar ışığı | 57, 2359 | `#4DAB26` | `#4BAA22` | 2/1/4 | `#4BAA22` | 2/1/4 | Geçti |
| Booster · Yuva · lacivert | 27, 2359 | `#003D85` | `#112343` | 17/26/66 | `#05387A` | 5/5/11 | Palet farkı |
| Booster · Yuva · açık mavi | 23, 2359 | `#0075C6` | `#5A8BC6` | 90/22/0 | `#0074C3` | 0/1/3 | Palet farkı |
| Rozet · Dış koyu kırmızı | 177, 2423 | `#880007` | `#880007` | 0/0/0 | `#880007` | 0/0/0 | Geçti |
| Rozet · Dış gövde · orta | 181, 2423 | `#CD0000` | `#CD0000` | 0/0/0 | `#CD0000` | 0/0/0 | Geçti |
| Rozet · İç halka | 186, 2423 | `#FE4051` | `#FE3F50` | 0/1/1 | `#FE3F50` | 0/1/1 | Geçti |
| Rozet · Üst parlama | 214, 2391 | `#FE5557` | `#FE5557` | 0/0/0 | `#FE5557` | 0/0/0 | Geçti |
| Rozet · İç gövde · orta | 193, 2423 | `#E90000` | `#E90000` | 0/0/0 | `#E90000` | 0/0/0 | Geçti |
| Rozet · İç gövde · alt | 214, 2445 | `#DC0000` | `#DC0000` | 0/0/0 | `#DC0000` | 0/0/0 | Geçti |
| Rozet · Dış gövde · alt | 214, 2455 | `#A20000` | `#A20000` | 0/0/0 | `#A20000` | 0/0/0 | Geçti |
| Ayarlar · Turuncu halka | 949, 2359 | `#FF6400` | `#FF6400` | 0/0/0 | `#FF6400` | 0/0/0 | Geçti |
| Ayarlar · Altın halka | 957, 2359 | `#FFAD00` | `#FFAC00` | 0/1/0 | `#FFAC00` | 0/1/0 | Geçti |
| Ayarlar · Altın ışığı | 953, 2359 | `#FFC128` | `#FFBC1E` | 0/5/10 | `#FFBC1E` | 0/5/10 | Geçti |
| Ayarlar · Koyu mavi gövde kenarı | 962, 2359 | `#002C53` | `#002C53` | 0/0/0 | `#002C53` | 0/0/0 | Geçti |
| Ayarlar · Mavi gövde · üst | 1043, 2281 | `#46AAE2` | `#46AAE2` | 0/0/0 | `#46AAE2` | 0/0/0 | Geçti |
| Ayarlar · Mavi gövde · orta | 968, 2359 | `#0055AA` | `#0055AA` | 0/0/0 | `#0055AA` | 0/0/0 | Geçti |
| Ayarlar · Mavi gövde · alt | 1043, 2434 | `#003F81` | `#003F81` | 0/0/0 | `#003F81` | 0/0/0 | Geçti |
| Ayarlar · İç disk · üst | 1043, 2291 | `#0080E5` | `#0080E5` | 0/0/0 | `#0080E5` | 0/0/0 | Geçti |
| Ayarlar · İç disk · orta | 978, 2359 | `#006AC9` | `#006AC9` | 0/0/0 | `#006AC9` | 0/0/0 | Geçti |
| Ayarlar · İç disk · alt | 1043, 2421 | `#0056B0` | `#0056B0` | 0/0/0 | `#0056B0` | 0/0/0 | Geçti |
| Ayarlar · İç disk · kenar ışığı | 973, 2359 | `#2B87C6` | `#2C88C6` | 1/1/0 | `#2C88C6` | 1/1/0 | Geçti |
| Ayarlar · Yuva · lacivert | 943, 2359 | `#003D85` | `#102444` | 16/25/65 | `#033A7F` | 3/3/6 | Palet farkı |
| Ayarlar · Yuva · açık mavi | 939, 2359 | `#0075C6` | `#5282BD` | 82/13/9 | `#0070C1` | 0/5/5 | Palet farkı |
| Sağ panel · Gövde · dış mavi | 805, 330 | `#0075C5` | `#5B8CC8` | 91/23/3 | `#0075C5` | 0/0/0 | Palet farkı |
| Sağ panel · Gövde · koyu mavi | 811, 330 | `#003D87` | `#0E2547` | 14/24/64 | `#003D87` | 0/0/0 | Palet farkı |
| Sağ panel · Bevel · sol üst | 819, 242 | `#AEA384` | `#AEA384` | 0/0/0 | `#AEA384` | 0/0/0 | Geçti |
| Sağ panel · Bevel · sol orta | 819, 330 | `#A0957D` | `#A0957D` | 0/0/0 | `#A0957D` | 0/0/0 | Geçti |
| Sağ panel · Bevel · sol alt | 819, 402 | `#988E78` | `#988E78` | 0/0/0 | `#988E78` | 0/0/0 | Geçti |
| Sağ panel · Bevel · alt | 971, 449 | `#958B77` | `#958B77` | 0/0/0 | `#958B77` | 0/0/0 | Geçti |
| Sağ panel · Krem · üst | 875, 222 | `#E8DCB8` | `#F6ECCE` | 14/16/22 | `#E8DCB8` | 0/0/0 | Palet farkı |
| Sağ panel · Krem · orta | 875, 326 | `#F7ECBC` | `#FAF1D5` | 3/5/25 | `#F7ECBC` | 0/0/0 | Palet farkı |
| Sağ panel · Krem · alt | 875, 414 | `#FEF4BE` | `#FCF5DB` | 2/1/29 | `#FEF4BE` | 0/0/0 | Palet farkı |
| Sağ panel · Kurdele · üst altın | 976, 122 | `#FFC400` | `#FFC400` | 0/0/0 | `#FFC400` | 0/0/0 | Geçti |
| Sağ panel · Kurdele · üst turuncu | 976, 127 | `#FF7500` | `#FF7500` | 0/0/0 | `#FF7500` | 0/0/0 | Geçti |
| Sağ panel · Kurdele · mavi | 876, 155 | `#005AAE` | `#295C98` | 41/2/22 | `#0059AE` | 0/1/0 | Palet farkı |
| Sağ panel · Kurdele · alt altın | 969, 182 | `#FFC300` | `#FFC300` | 0/0/0 | `#FFC300` | 0/0/0 | Geçti |
| Sağ panel · Kurdele · alt turuncu | 969, 186 | `#FF7500` | `#FF7500` | 0/0/0 | `#FF7500` | 0/0/0 | Geçti |
| Orta pencere · Dış açık mavi | 396, 302 | `#0075C5` | `#5B8CC8` | 91/23/3 | `#0075C5` | 0/0/0 | Palet farkı |
| Orta pencere · Koyu mavi | 401, 302 | `#003D86` | `#0E2547` | 14/24/63 | `#003D87` | 0/0/1 | Palet farkı |
| Orta pencere · Turuncu bant | 408, 302 | `#FC6E00` | `#FC6E00` | 0/0/0 | `#FC6E00` | 0/0/0 | Geçti |
| Orta pencere · Açık sarı bant | 412, 302 | `#FFC762` | `#FFC762` | 0/0/0 | `#FFC762` | 0/0/0 | Geçti |
| Orta pencere · Altın bant | 415, 302 | `#FFAF00` | `#FFAF00` | 0/0/0 | `#FFAF00` | 0/0/0 | Geçti |
| Orta pencere · İç kenar gölgesi | 422, 302 | `#242E44` | `#233046` | 1/2/2 | `#233046` | 1/2/2 | Geçti |
| Orta pencere · İç camgöbeği geçişi | 429, 302 | `#2F516F` | `#30516D` | 1/0/2 | `#30516D` | 1/0/2 | Geçti |
| Şerit bordürü · Altın başlangıç | 6, 479 | `#FFBB00` | `#FFBB00` | 0/0/0 | `#FFBB00` | 0/0/0 | Geçti |
| Şerit bordürü · Altın gölge | 6, 481 | `#FFB000` | `#FFB000` | 0/0/0 | `#FFB000` | 0/0/0 | Geçti |
| Şerit bordürü · Sarı ışık | 6, 484 | `#FFD65A` | `#FFD65A` | 0/0/0 | `#FFD65A` | 0/0/0 | Geçti |
| Şerit bordürü · Turuncu | 6, 487 | `#FF7101` | `#FF7101` | 0/0/0 | `#FF7101` | 0/0/0 | Geçti |
| Şerit bordürü · Turuncu alt | 6, 490 | `#E2640A` | `#E2640A` | 0/0/0 | `#E2640A` | 0/0/0 | Geçti |
| Şerit bordürü · Üst şerit · tepe | 4, 1 | `#124382` | `#14325E` | 2/17/36 | `#124382` | 0/0/0 | Palet farkı |
| Şerit bordürü · Üst şerit · alt | 4, 475 | `#1A5EA5` | `#2A5A94` | 16/4/17 | `#1A5EA5` | 0/0/0 | Palet farkı |
| Şerit bordürü · Alt şerit · koyu kenar | 4, 2314 | `#003984` | `#0E2547` | 14/20/61 | `#003C84` | 0/3/0 | Palet farkı |
| Şerit bordürü · Alt şerit · ışık çizgisi | 4, 2320 | `#0075C5` | `#5B8CC8` | 91/23/3 | `#0075C5` | 0/0/0 | Palet farkı |
| Şerit bordürü · Alt şerit · dolgu | 4, 2365 | `#0056AB` | `#224F88` | 34/7/35 | `#0056AB` | 0/0/0 | Palet farkı |

## Geometri ve metin

| Kontrol | Teslim ölçüsü |
|---|---|
| Oyun ekranı | 390 × 844 pt. Task30'un panel, pencere, tahta ve tuş merkezleri kullanıldı. |
| Booster / Ayarlar | Nominal 195 px, 65 pt. Turuncu, tuş gövdesinin son katmanı. Koyu lacivert ve açık mavi çizgiler, şerit üstündeki ayrı yuva katmanında. |
| Adet rozeti | 77 px dış gövde, yaklaşık 25,7 pt. Tuş merkezine göre +88 px / +64 px. Beyaz kontur kullanılmadı. |
| Üst paneller | 113 × 118 pt nominal alan. Gövde mavi → gri-bej bevel → krem. Altın ve turuncu şeritler yalnızca kurdelede. |
| Kurdele metni | Başlık krem yüzü 30 px, yaklaşık 10 pt. Kurdele merkezine yerleşti. |
| Panel sayıları | Krem rakam yüzü 100 px, yaklaşık 33,3 pt ve krem alan yüksekliğinin %43'ü. Sağ merkez (972, 325) px. Sol parça ikonu ve sayı birlikte (201, 325) px'e ortalandı. |
| Süre doldu | Büyük üzgün büst ortada, küçük saat ve +30 sn altında, 900 ve para ikonu yeşil tuşun içinde. Sağ üstte kırmızı X. |

## Kaynaklar ve sprite dizileri

- Booster nesneleri ve parça ikonu HUD30'dan bayt düzeyinde aynı alındı. Mavi çekicin kökeni Task28 arşivi. Dişli, bu setin sekiz dişli konturuyla üretildi ve brifteki krem/mor tonlarına boyandı. Yazı ve sayılar kaynak sprite'lara gömülmedi. [HUD30 ikonları](../../../Assets/Art/_yeni/HUD30/icons/) · [Arşiv mavi çekiç](../task28/_arsiv/Z2-v3-mavi-cekic/booster-hammer.png)
- Jigo v3'te beş hâlin her birinde altı 256 × 256 RGBA kare var. Tedirgin dizisi Task30'dan bayt düzeyinde aynı. Dört yeni ana poz yerleşik ImageGen ile Task30 büstleri ve Task26 model paftası kullanılarak hazırlandı. Normalde 04 göz kırpıyor, korkuda yüksük hafif titriyor, üzgünde düz yüksük ve dudak hareketi var, mutluda hilal gözler ve kalkan/fırça okunuyor. [Task26 model paftası](../task26/kahraman-final.png) · [Task30 büstleri](../../../Assets/Art/_yeni/Jigo_v2/)
- Kont'un B kilidinde Task30'un kapalı gövdesi ve mühür detayı aynen alındı. Ardından `open-01 → open-02 → open-03 → open-04`. `open-04.png` düşüş kodunun kullanacağı tek temiz açık kare. Üç `wax-01…03` karesi ayrı 192 × 192 RGBA parçacık katmanı. Kulp açılma PNG'leri 384 × 384. [Task30 B kilidi](../../../Assets/Art/_yeni/Obstacles_v2/padlock-b/)
- p20 kaynağı olarak yayın öncesi `0a356fd4` Git ağacındaki `Assets/_Game/Presentation/Resources/Art/chain-link-v3-*.png` dosyaları kullanıldı. Bu sürümde ChainView ilk olarak bu dosyaları yüklüyor. Dört dosyanın SHA-256 değerleri Task17 arşiv kopyalarıyla eşleşiyor. Dolayısıyla p20 kaynağı ile Task17'nin aynı dosyayı içermesi sürüm kaydında görülen bir çakışma. Bu teslimde tek halka doğrudan p20 runtime karosunun ön halkasından alfa maskesiyle ayırıldı. Task30'da kullanılan üretim geometrisi tekrar çalıştırılmadı. [Eski görünüm entegrasyon kaydı](../task28/ENTEGRASYON_RAPOR.md)
- p20 zincirinde dört renk mevcut. Beşinci yeşil halka p20 altın halkanın renklendirilmiş eşidir. Yaprak işareti mevcut yeşil ankrajdan alındı. Halkalar 128 × 64, ankrajlar 224 × 224 ve parça merkezi pivotu ortak. Sarkma ve yaylanma koduna dokunulmadı. [p20 zincir dosyalarının güncel çalışma kopyası](../../../Assets/_Game/Presentation/Resources/Art/)
- Devam kartı, kurdele, kırmızı X, saat, para ve tuş katmanları bağımsız. Yeşil tuş, saat ve para Task29 paketinden alındı. [Task29 Devam](../../../Assets/Art/_yeni/Devam/)

## ImageGen kayıtları

Ortak düzenleme hedefi Task30'daki ilgili kareydi. Task26 paftasının sol ön pozu kimlik referansıydı. Omuz hizasındaki kadraj, mavi fırça, düğme kalkan, turkuaz gövde ve gümüş yüksük korundu. Çıktı gerçek alfa kanallı tek büst olarak istendi.

| Hâl | İstem farkı | Ana poz |
|---|---|---|
| Normal | Yumuşak, yataya yakın yay kaşlar ve küçük sıcak gülüş. | `exec-18e9c5b3-cee7-44be-8bf1-f5351822514b.png` |
| Korku | Biraz daha büyük göz, doğal iris, küçük o ağız ve hafif kaygı. | `exec-cbfd31b6-b14c-4bd5-9e8f-b33871240f5e.png` |
| Üzgün | Düz ve ortalanmış yüksük, düşük bakış, hafif ıslak alt kapak, kıvrık dudak. | `exec-196650bf-990b-4533-8131-6bedfc1d0b88.png` |
| Mutlu | Yumuşak yuvarlak kaş, hilal göz ve kalkık yanaklar. | `exec-5d361f93-663d-4713-b111-a4e6d187e2fe.png` |

Bu teslim 390 × 844 pt sabit görsel prova ve sürümlü PNG kaynaklarıdır. Unity ve oyun entegrasyonu yapılmadı.
