# Task32 · HUD final denetimi

[Önizleme](onizleme.png) · [390 × 844 pt referans](../Reference/rm_oyun_ekrani.png) · [Booster renk kaynağı](../task31/ref_alt.png) · [Task31 denetimi](../task31/denetim.md) · [HUD32](../../../Assets/Art/_yeni/HUD32/)

Ölçülen 17 ikon örneğinin tamamı kanal başına ≤12 sınırında. En büyük fark 6. HUD malzemesinde korunması gereken 42 örneğin en büyük farkı 10. Tam HUD tablosundaki 20 fark seçilen M1 paletinden veya düz başlığa geçişten geliyor. Bu satırlar ayrıca işaretlendi.

Pillow ile önce 1170 × 2532 px referans ve birebir alt şerit kırpması okundu. İkon tablosu, referans ve sprite üzerinde belirtilen 3 × 3 yamaların RGB medyanını verir. HUD tablosu Task31’in sabit koordinatlarını ve bağımsız malzeme katmanlarının 1:1 RGB değerlerini kullanır. Paftada altı kırpmanın her çifti aynı bölge ve ölçektedir. Δ mutlak R/G/B farkı, 3 px = 1 pt.

## Yeni mavi gradyan

| Şerit | (x, y) tam ekran px | İstenen | Ölçülen |
|---|---:|---|---|
| Üst | 6, 0 | #112C55 | #112C55 |
| Üst | 6, 238 | #173F78 | #173F78 |
| Üst | 6, 476 | #1D5499 | #1D5499 |
| Alt A | 6, 2322 | #245593 | #245593 |
| Alt A | 6, 2531 | #112C55 | #112C55 |
| Alt B | 6, 2322 | #12345F | #12345F |
| Alt B | 6, 2531 | #1D5499 | #1D5499 |

## Booster boyası

Kırmızı palyaço şapkasından, mor şapka ve çekiçten, sarı metal şapka bandından, koyu ton topun gövdesinden ölçüldü. Açık sarı parlamada kırpmanın #FDD32C örneği esas alındı. Mavi çekiç B için mavi ve krem, özgün gövdeye uygulanan alternatif palettir.

| Malzeme | Referans kırpma px | Sprite px | Referans | Ölçülen | Δ R/G/B |
|---|---:|---:|---|---|---|
| Makas · kulp ışığı | 754, 106 | 46, 86 | #D02130 | #CF2130 | 1/0/0 |
| Makas · kulp orta | 765, 109 | 51, 96 | #C1202D | #C0202D | 1/0/0 |
| Makas · kulp gölge | 777, 131 | 77, 195 | #AE1520 | #AD1521 | 1/0/1 |
| Asa · mor ışık | 840, 93 | 83, 151 | #A133F1 | #A033F0 | 1/0/1 |
| Asa · mor orta | 842, 137 | 98, 132 | #7C19BE | #7B19BD | 1/0/1 |
| Asa · mor gölge | 844, 137 | 113, 137 | #7417AD | #7317AC | 1/0/1 |
| Asa · yıldız açık | 834, 148 | 131, 83 | #F6BA16 | #F5B916 | 1/1/0 |
| Asa · yıldız orta | 790, 146 | 153, 113 | #F1AB02 | #EFAA02 | 2/1/0 |
| Asa · yıldız gölge | 781, 162 | 165, 67 | #E28F00 | #E08E01 | 2/1/1 |
| Asa · yıldız parlama | 830, 148 | 134, 60 | #FDD32C | #FCD22D | 1/1/1 |
| Çekiç A · mor ışık | 105, 74 | 143, 89 | #6F41EF | #6F41EE | 0/0/1 |
| Çekiç A · mor gövde | 106, 96 | 126, 122 | #5031CF | #5031CE | 0/0/1 |
| Çekiç A · kırmızı kapak | 765, 109 | 41, 91 | #C1202D | #C0202D | 1/0/0 |
| Saat · kasa orta | 577, 152 | 55, 133 | #302E60 | #302E60 | 0/0/0 |
| Saat · kasa gölge | 619, 139 | 162, 173 | #211F3F | #211F3F | 0/0/0 |
| Saat · sarı ayak | 790, 146 | 145, 197 | #F1AB02 | #EFA902 | 2/2/0 |
| Saat · sarı zil | 790, 146 | 176, 65 | #F1AB02 | #F1AF08 | 0/4/6 |

## HUD · sabit referans koordinatları

| Grup | Örnek sayısı | ≤12 | Tasarım farkı |
|---|---:|---:|---:|
| Booster | 16 | 14 | 2 |
| Rozet | 7 | 7 | 0 |
| Ayarlar | 13 | 11 | 2 |
| Sağ panel | 14 | 4 | 10 |
| Orta pencere | 7 | 5 | 2 |
| Şerit bordürü | 10 | 6 | 4 |

| Katman | (x, y) px | Referans | Task32 | Δ R/G/B | Sonuç |
|---|---:|---|---|---|---|
| Booster · Turuncu · üst | 126, 2265 | #FF6A00 | #FF6B00 | 0/1/0 | Geçti |
| Booster · Turuncu · yan | 33, 2359 | #FF6400 | #FF6500 | 0/1/0 | Geçti |
| Booster · Turuncu · alt | 126, 2451 | #FF4C00 | #FF4D01 | 0/1/1 | Geçti |
| Booster · Altın · üst | 126, 2271 | #FFC000 | #FFC100 | 0/1/0 | Geçti |
| Booster · Altın ışığı · üst | 126, 2268 | #FFCF00 | #FFCB00 | 0/4/0 | Geçti |
| Booster · Altın · yan | 41, 2359 | #FFAC00 | #FFAC00 | 0/0/0 | Geçti |
| Booster · Altın ışığı · yan | 37, 2359 | #FFC128 | #FFC128 | 0/0/0 | Geçti |
| Booster · Koyu yeşil kenar | 46, 2359 | #034F00 | #034F00 | 0/0/0 | Geçti |
| Booster · Yeşil gövde · üst | 126, 2281 | #A9DD3E | #A9DD3E | 0/0/0 | Geçti |
| Booster · Yeşil gövde · orta | 51, 2359 | #008C00 | #008C00 | 0/0/0 | Geçti |
| Booster · Yeşil gövde · alt | 126, 2434 | #005C00 | #005C00 | 0/0/0 | Geçti |
| Booster · İç disk · üst | 126, 2290 | #70D600 | #70D600 | 0/0/0 | Geçti |
| Booster · İç disk · orta | 63, 2359 | #38A400 | #38A400 | 0/0/0 | Geçti |
| Booster · İç disk · kenar ışığı | 57, 2359 | #4DAB26 | #4BAA22 | 2/1/4 | Geçti |
| Booster · Yuva · lacivert | 27, 2359 | #003D85 | #112343 | 17/26/66 | M1 paleti |
| Booster · Yuva · açık mavi | 23, 2359 | #0075C6 | #5A8BC6 | 90/22/0 | M1 paleti |
| Rozet · Dış koyu kırmızı | 177, 2423 | #880007 | #880007 | 0/0/0 | Geçti |
| Rozet · Dış gövde · orta | 181, 2423 | #CD0000 | #CD0000 | 0/0/0 | Geçti |
| Rozet · İç halka | 186, 2423 | #FE4051 | #FE3F50 | 0/1/1 | Geçti |
| Rozet · Üst parlama | 214, 2391 | #FE5557 | #FE5557 | 0/0/0 | Geçti |
| Rozet · İç gövde · orta | 193, 2423 | #E90000 | #E90000 | 0/0/0 | Geçti |
| Rozet · İç gövde · alt | 214, 2445 | #DC0000 | #DC0000 | 0/0/0 | Geçti |
| Rozet · Dış gövde · alt | 214, 2455 | #A20000 | #A20000 | 0/0/0 | Geçti |
| Ayarlar · Turuncu halka | 949, 2359 | #FF6400 | #FF6400 | 0/0/0 | Geçti |
| Ayarlar · Altın halka | 957, 2359 | #FFAD00 | #FFAC00 | 0/1/0 | Geçti |
| Ayarlar · Altın ışığı | 953, 2359 | #FFC128 | #FFBC1E | 0/5/10 | Geçti |
| Ayarlar · Koyu mavi gövde kenarı | 962, 2359 | #002C53 | #002C53 | 0/0/0 | Geçti |
| Ayarlar · Mavi gövde · üst | 1043, 2281 | #46AAE2 | #46AAE2 | 0/0/0 | Geçti |
| Ayarlar · Mavi gövde · orta | 968, 2359 | #0055AA | #0055AA | 0/0/0 | Geçti |
| Ayarlar · Mavi gövde · alt | 1043, 2434 | #003F81 | #003F81 | 0/0/0 | Geçti |
| Ayarlar · İç disk · üst | 1043, 2291 | #0080E5 | #0080E5 | 0/0/0 | Geçti |
| Ayarlar · İç disk · orta | 978, 2359 | #006AC9 | #006AC9 | 0/0/0 | Geçti |
| Ayarlar · İç disk · alt | 1043, 2421 | #0056B0 | #0056B0 | 0/0/0 | Geçti |
| Ayarlar · İç disk · kenar ışığı | 973, 2359 | #2B87C6 | #2C88C6 | 1/1/0 | Geçti |
| Ayarlar · Yuva · lacivert | 943, 2359 | #003D85 | #102444 | 16/25/65 | M1 paleti |
| Ayarlar · Yuva · açık mavi | 939, 2359 | #0075C6 | #5282BD | 82/13/9 | M1 paleti |
| Sağ panel · Gövde · dış mavi | 805, 330 | #0075C5 | #4376B6 | 67/1/15 | M1 paleti |
| Sağ panel · Gövde · koyu mavi | 811, 330 | #003D87 | #112F59 | 17/14/46 | M1 paleti |
| Sağ panel · Bevel · sol üst | 819, 242 | #AEA384 | #AEA384 | 0/0/0 | Geçti |
| Sağ panel · Bevel · sol orta | 819, 330 | #A0957D | #A0957D | 0/0/0 | Geçti |
| Sağ panel · Bevel · sol alt | 819, 402 | #988E78 | #988E78 | 0/0/0 | Geçti |
| Sağ panel · Bevel · alt | 971, 449 | #958B77 | #958B77 | 0/0/0 | Geçti |
| Sağ panel · Krem · üst | 875, 222 | #E8DCB8 | #F6ECCE | 14/16/22 | M1 paleti |
| Sağ panel · Krem · orta | 875, 326 | #F7ECBC | #FAF1D5 | 3/5/25 | M1 paleti |
| Sağ panel · Krem · alt | 875, 414 | #FEF4BE | #FCF5DB | 2/1/29 | M1 paleti |
| Sağ panel · Kurdele · üst altın | 976, 122 | #FFC400 | #EEB324 | 17/17/36 | Düz başlık |
| Sağ panel · Kurdele · üst turuncu | 976, 127 | #FF7500 | #3163A1 | 206/18/161 | Düz başlık |
| Sağ panel · Kurdele · mavi | 876, 155 | #005AAE | #285997 | 40/1/23 | Düz başlık |
| Sağ panel · Kurdele · alt altın | 969, 182 | #FFC300 | #194072 | 230/131/114 | Düz başlık |
| Sağ panel · Kurdele · alt turuncu | 969, 186 | #FF7500 | #D99124 | 38/28/36 | Düz başlık |
| Orta pencere · Dış açık mavi | 396, 302 | #0075C5 | #5B8CC8 | 91/23/3 | M1 paleti |
| Orta pencere · Koyu mavi | 401, 302 | #003D86 | #0E2547 | 14/24/63 | M1 paleti |
| Orta pencere · Turuncu bant | 408, 302 | #FC6E00 | #FC6E00 | 0/0/0 | Geçti |
| Orta pencere · Açık sarı bant | 412, 302 | #FFC762 | #FFC762 | 0/0/0 | Geçti |
| Orta pencere · Altın bant | 415, 302 | #FFAF00 | #FFAF00 | 0/0/0 | Geçti |
| Orta pencere · İç kenar gölgesi | 422, 302 | #242E44 | #233046 | 1/2/2 | Geçti |
| Orta pencere · İç camgöbeği geçişi | 429, 302 | #2F516F | #30516D | 1/0/2 | Geçti |
| Şerit bordürü · Altın başlangıç | 6, 479 | #FFBB00 | #FFBB00 | 0/0/0 | Geçti |
| Şerit bordürü · Altın gölge | 6, 481 | #FFB000 | #FFB000 | 0/0/0 | Geçti |
| Şerit bordürü · Sarı ışık | 6, 484 | #FFD65A | #FFD65A | 0/0/0 | Geçti |
| Şerit bordürü · Turuncu | 6, 487 | #FF7101 | #FF7101 | 0/0/0 | Geçti |
| Şerit bordürü · Turuncu alt | 6, 490 | #E2640A | #E2640A | 0/0/0 | Geçti |
| Şerit bordürü · Üst şerit · tepe | 4, 1 | #124382 | #112C55 | 1/23/45 | M1 paleti |
| Şerit bordürü · Üst şerit · alt | 4, 475 | #1A5EA5 | #1D5499 | 3/10/12 | Geçti |
| Şerit bordürü · Alt şerit · koyu kenar | 4, 2314 | #003984 | #163766 | 22/2/30 | M1 paleti |
| Şerit bordürü · Alt şerit · ışık çizgisi | 4, 2320 | #0075C5 | #4877B2 | 72/2/19 | M1 paleti |
| Şerit bordürü · Alt şerit · dolgu | 4, 2365 | #0056AB | #204C88 | 32/10/35 | M1 paleti |

## Yerleşim ve onaylı varlıklar

- Ana 390 × 844 pt provada açık üstten koyu alta inen **alt şerit A** kullanıldı. Bu yönde yeşil tuşların altında daha koyu bir zemin var. Çekiç A ana ekranda, iki çekiç de seçenek satırında. Bu iki seçim prova içindir.
- Parça ve Süre şeritlerinin boyalı yüksekliği 67 px, Baloo 2 ExtraBold başlık yüzü 39 px (%58). Devam başlığında 80 px üstünde 48 px (%60). Şeritler düz, üst ve alt çizgi ince. [Baloo 2 ExtraBold](../../../Assets/_Game/Presentation/Resources/Fonts/Baloo2-ExtraBold.ttf)
- Krem sayaç alanı yaklaşık y=207–438 px. Rakam yüzü 79 px (%34,2), düşey merkez y=323 px. Sağ sayı x=972 px, sol ikon ve sayı grubunun merkezi x=201 px. Parça ikonunun görünen tuvali 78 × 79 px. Başlık, rakam ve etiketler paftaya ayrı metin katmanından işlendi.
- Kilit B için oyun provasında görünür uzun kenar 106 px / 192 px, yani 35,3 / 64 pt (%55,2). Ölçüm alfa ≥128 sınırından alındı. Kilit PNG'leri, açılma ve mum kareleri olduğu gibi kullanılır. Zincir aynı p20 paketinden. [Engeller v3](../../../Assets/Art/_yeni/Obstacles_v3/)
- Durdur simgesi 120 × 132 px, aynı mavi tuş gövdesine takılır. İki krem çubuk ve koyu kontur ayrı ikon. Süre doldu provasında yeşil satın al tuşu, para, 900, +30 sn, saat ve kırmızı X aynı. Ek metin tuşu isteğe bağlı; etiketi PNG'ye gömülmedi.

### Yeniden kullanılacak dosyalar

| Kaynak | Yeniden kullanılan dosyalar |
|---|---|
| [HUD31](../../../Assets/Art/_yeni/HUD31/) | `m1/socket.png`, `m1/portrait-window.png`; `shared/booster-{normal,pressed,locked,zero}.png`, `shared/settings-{normal,pressed,locked,zero}.png`, `shared/badge.png`, `shared/portrait-mask.png`, `shared/locked-icon.png`; `icons/icon-piece.png`. |
| [Jigo_v3](../../../Assets/Art/_yeni/Jigo_v3/) | `tedirgin`, `korku`, `uzgun`, `mutlu` klasörlerinin altışar karesi. |
| [Devam_v2](../../../Assets/Art/_yeni/Devam_v2/) | `panel.png`, `close-button.png`, `icon-close.png`, `continue-clock.png`, `continue-coin.png`, `continue-primary.png`. |
| [Obstacles_v3](../../../Assets/Art/_yeni/Obstacles_v3/) | `padlock-b` ve `chain` klasörlerinin tamamı. |

### HUD32'deki 19 yeni PNG

| Dosya | Boyut |
|---|---|
| `top-strip.png` | 1170 × 504 px |
| `bottom-strip-a.png`, `bottom-strip-b.png` | 1170 × 219 px |
| `counter-panel.png`, `header-strip.png` | 340 × 290 ve 355 × 78 px |
| `pause-icon.png` | 120 × 132 px |
| `icons/{scissors,wand,hammer-a,hammer-b,clock}.png` | 240 × 240 px. Beş görselin de alfa kanalı kaynakla birebir aynı. |
| `jigo/normal/01…06.png` | 256 × 256 px RGBA. Altı karede de kaynak alfa ve ağız bölgesi dışındaki pikseller birebir aynı. |
| `devam/title-strip.png`, `devam/decline-text-button.png` | 1008 × 91 ve 490 × 70 px |

### Jigo normal düzenlemesi

Yerleşik ImageGen, Task31 normal büstünün ilk karesi üzerinde kullanıldı. Üretim çıktısı `exec-090dc511-4b5a-4a35-bace-fbbe7618b4ad.png`. Yeni ağız alanı kaynak büste hizalanıp yalnızca yerel renk maskesi olarak mevcut altı kareye uygulandı. 04 göz kırpma karesi kaynak göz piksellerini korur. [Task31 normal büstü](../../../Assets/Art/_yeni/Jigo_v3/normal/01.png)

> Precise local image edit for a game production sprite. The provided exact image is the asset to edit. Change ONLY the small mouth area. Paint the current visibly open white-toothed mouth into a very tiny softly upturned CLOSED lip seam on turquoise (a small U curve, with corners softened, width about 13 percent of the square frame, shallow curve, absolutely zero teeth, zero tongue and zero open dark cavity). Think gentle relaxed warmth, not a grin, not a frown. No eyebrow movement, no changes to the position, shape, size or look of eyes, no blush change. Preserve every other pixel or match as closely as possible: same hat and face positions, torso, silhouette, props, warm lighting, tight bust crop, transparent outside, no new elements, no text. Keep the same square framing and do not zoom or recompose.

Bu teslim sabit PNG provasıdır. Oyun koduna veya mevcut kaynak PNG'lere yazılmadı.
