# T46 — Saat booster ve Jigo geçişleri

**65 RGBA PNG**, tek parça saat pozları, rahat kaşlar, mavi/krem ve doygun altın dili. [390 × 844 önizleme](onizleme.png), [saat animasyonu](clock_review.gif), [Jigo animasyonu](jigo_review.gif), [dosya/ölçü/SHA-256 manifestosu](manifest.json).

## Dosyalar ve oynatma

Tüm yollar `Assets/Art/_yeni/T46/` köküne göredir. PNG'lerin yanında sabit GUID, şeffaflık, bilinear filtre, mipmap kapalı ve özel pivot içeren `.png.meta` dosyaları bulunur. Saat pozlarında ayrı kol katmanı yoktur; eski T42 rig parçaları kullanılmaz.

| Dosya/dizi | Kare | Tuval px | FPS | Oynatma |
|---|---:|---:|---:|---|
| `clock/extend/01..06.png` | 6 | 352 × 256 | 12 | Bir kez, 0,50 sn. Düğme iner, gövde döner ve fırça sağa uzanır. |
| `clock/hold/01..04.png` | 4 | 352 × 256 | 12 | 0,333 sn nefes döngüsü; 10 sn boyunca 30 tur. Temas ucu sabit. |
| `clock/retract/01..05.png` | 5 | 352 × 256 | 12 | Bir kez, 0,417 sn. Fırça geri gelir ve düğme alınır. |
| `clock/spark/01..06.png` | 6 | 96 × 96 | 24 | İlk temasta bir kez, 0,25 sn. Altın-beyaz yıldızlar. |
| `clock/ice_overlay.png` | 1 | 339 × 290 | — | Temastan itibaren panelde 10 sn; sayılar görünür kalır. |
| `clock/ice_break/01..07.png` | 7 | 339 × 434 | 18 | Bir kez, 0,389 sn. 01 sağlam, 02 çatlak, 03–06 düşen parçalar, 07 boş. |
| `clock/freeze_bar_bg.png` | 1 | 300 × 42 | — | Krem çerçeve ve mavi kanal. |
| `clock/freeze_bar_fill.png` | 1 | 288 × 30 | — | Buz mavisi dolgu; sağdan kırpılır. |
| `jigo/normal_to_happy/01..05.png` | 5 | 256 × 256 | 20 | Bir kez, 0,25 sn; göz, yanak, ağız ve fırça yüksekliği kademeli. |
| `jigo/happy_to_normal/01..05.png` | 5 | 256 × 256 | 20 | Aynı pozların tersi, 0,25 sn. |
| `jigo/normal_loop/01..12.png` | 12 | 256 × 256 | 20 | 0,60 sn döngü; altı özgün kare + altı alfa korumalı zamansal ara örnek. |
| `jigo/happy_loop/01..12.png` | 12 | 256 × 256 | 20 | 0,60 sn döngü; son→ilk arası da örneklenmiştir. |

Döngüler 12–24 FPS aralığında kullanılabilir; eski altı karelik 10 FPS zamanlamasını korumak için **20 FPS** önerilir. Kaynak 01..06, yeni 01/03/05/07/09/11 kareleriyle piksel düzeyinde aynıdır. 12. kare, eski 06→01 hareketinin ortasıdır. İfade geçişlerini döngünün 01 fazında başlatıp hedef döngünün 01 fazına bağla; iki farklı yüzü alfa ile üst üste bindirme. GIF zamanları 10 ms adımlara yuvarlanabilir; tabloda motor için kesin hızlar verilmiştir.

## Tuval, pivot ve ofset

Koordinatlar **PNG sol üstünden**, x sağa/y aşağıya; tüm değerler kaynak pikselidir.

| Grup | Pivot px (sol üst) | Unity normalize pivot (sol alt) | Yerleşim/ofset |
|---|---|---|---|
| Jigo döngü/geçiş | (128, 128) | (0,5; 0,5) | T44 ile aynı 256 × 256 tuval, sıfır ofset. |
| Saat extend/hold/retract | (128, 128) | (0,363636364; 0,5) | Eski 256 × 256 alanın sağına **96 px** saydam pay eklenir; eski alanın sol üstü (0,0), içerik ofseti (0,0). |
| Işıltı | (48, 48) | (0,5; 0,5) | Merkezini her karenin fırça ucuna bağla. |
| Buz kaplaması | (169,5; 145) | (0,5; 0,5) | Süre paneli gövdesiyle aynı merkez/ölçek. |
| Buz kırılma | (169,5; 145) | (0,5; 0,665898618) | İlk 290 satır kaplamayla birebir hizalı, aşağıda **144 px** düşüş payı. Kaplamayla aynı dünya pivotu. |
| Bar bg/fill | tuval merkezi | (0,5; 0,5) | Dolgu bg sol üstüne göre (+6,+6) px; merkezleri ortaktır. |

Saat karesini genişliğe sığdırarak küçültme: normal büstün piksel ölçeğini koru. Normal büst 108 px yüksekliğinde gösteriliyorsa yeni saat tuvali 148,5 × 108 px gösterilir. Madalyon arka planı sabit kalır; saat çizimi madalyon maskesinin **üstünde** yer alır, dışarı uzanan fırça kırpılmaz. `retract/05` ve `extend/01` içindeki ilk 256 × 256 piksel normal kaynakla aynıdır; ek 96 px tamamen saydamdır.

## Fırça ucu

Temas/ışıltı yerleşim noktaları, saat tuvali içinde px; başlangıçta fırçanın mavi üst ucu, uzanmış halde sağdaki mavi uç referans alınır. Bütün kareler ayrıca `manifest.json` içindedir.

| Kare | x | y |
|---|---:|---:|
| extend/01 | 23 | 119 |
| extend/02 | 52 | 119 |
| extend/03 | 214 | 123 |
| extend/04 | 248 | 118 |
| extend/05 | 273 | 134 |
| extend/06 | 290 | 140 |
| hold/01..04 | 290 | 140 |
| retract/01 | 273 | 134 |
| retract/02 | 248 | 118 |
| retract/03 | 214 | 123 |
| retract/04 | 52 | 119 |
| retract/05 | 23 | 119 |

Genel yerleşim formülü: `uç_ekran = pivot_ekran + (uç_px − (128,128)) × ölçek`. Tutma nefesi bütün büste temas noktası etrafında ±%0,8 uygulanır; kol veya el ayrı hareket ettirilmez. Buz 10 saniye boyunca sabit kalır; başlangıç ışıltısı hold döngüsünde tekrar tetiklenmez. Buz kırılırken statik kaplamayı kaldır; son boş kareden sonra katmanı kapat ve geri alma dizisini oynat.

## Panel ve 9-slice

Mevcut `Hud54.Panel`: 390 × 844 ekranda **113 × 96,6667 px**, merkez (324;108,6667), sol üst (267,5;60,3333), güvenli üst alan ofseti ayrıca eklenir. 3× dokuda **339 × 290 px** bu ölçüyü tam karşılar. Eski HUD31 panel bitmap'i 340 × 290 olsa da kod gövdeyi 113 px genişliğe gerer. Buz, başlık grafiğine değil bu gövdeye oturur. Önizlemedeki HUD yakın planı akışı göstermek için aşağı taşınmıştır; motor yerleşim koordinatları bu paragraftadır.

| PNG | 9-slice sol/alt/sağ/üst | 390 px ekranda | Kullanım |
|---|---|---|---|
| freeze_bar_bg | 14 / 14 / 14 / 14 px | 100 × 14 px | Sliced, en az 28 × 28 kaynak px. |
| freeze_bar_fill | 10 / 10 / 10 / 10 px | 96 × 10 px | Sabit tam boy dolgu üzerinde soldan ankrajlı kırpma. |

Bar merkezi için panelin altında (324;150) + güvenli üst alan ofseti kullanılabilir. Dolgu oranı `clamp(kalanSüre / 10, 0, 1)`; 0'da görünürlüğü kapat. Uç kapakların ezilmemesi için dolguyu daraltarak ölçeklemek yerine maskeyle sağdan kırp. Buz kaplaması ve kırılma kareleri slice edilmez.

## Üretim ve kontrol

- Kaynaklar: `T44/Jigo/normal/01..06.png`, `T44/Jigo/mutlu/01..06.png`, T44/T45 paftaları, eski T42 saat akışı ve `Hud54.cs` ölçüleri. T44 Jigo kareleri onaylı Jigo_v3 kaynaklarının kopyalarıdır.
- **Yerleşik image_gen** kullanıldı; CLI/API kullanılmadı. [Saat kaynak paftası](clock_source.png), [son ifade kaynak paftası](jigo_refined_source.png). Saydam alfa korundu. Kare kesme, sabit pivot yerleşimi, bütün büstte nefes dönüşümü ve premultiplied-alpha zamansal ara örnekleme [build_task46.py](build_task46.py) içindedir. Döngülerin yeni çift numaralı kareleri iki komşunun %50 zaman örneğidir; ifade geçişlerindeki üç iç poz ise yeniden çizilmiştir. UI buz/ışıltı/bar grafikleri aynı betikle yüksek çözünürlükte çizilip küçültülür.
- Saat üretim istemi: exact T44 Jigo identity; transparent 1536×1024, 3×2 equal cells; six poses normal→lower button→turn torso→bring brush across chest→reach right→full fencing reach; one continuous torso/shoulder/arm/hand/brush drawing, no rig patches, one brush, friendly brows, silver thimble, gold/blue brush, cream four-hole button, rich glossy shading, no text/grid/banner/ribbon/clock prop.
- İfade üretim istemi: exact normal and happy T44 references; transparent 1536×1024, 3×2 equal cells; six progressively redrawn expressions from open amber eyes/small smile to closed smiling eyes/open laughing mouth; fixed scale and body placement, brush left/button right, relaxed eyebrows throughout, no crossfade/double features, no text/grid/banner/ribbon.
- İfade düzeltme istemi: edit only paintbrush and gripping hand of six-cell sheet; gradually raise brush toward exact happy reference, blue tip local y=230/206/182/158/134/110 in 512 px cells; hand near y420; preserve body, thimble, face, button and relaxed eyebrows; no shoulder spike, duplicated brush or detached limb. Son paftadan 02/04/05 hücreleri kullanıldı; geçiş uçları yine özgün normal/mutlu PNG'leridir.
- Doğrulama: **65/65 RGBA PNG**, istenen kare sayıları ve boyutları, 12 özgün döngü anahtarının piksel eşitliği, dönüş sonunun kaynak normalle eşitliği, sağ payın saydamlığı ve **390 × 844** önizleme boyutu betikte assert ile kontrol edildi. SHA-256 manifestosu yeniden üretildi. PNG'ler lacivert zemin üzerinde görsel incelendi; kaşlar rahat, kol tek parça, buz merkezi okunabilir.
- Teslim yalnızca `Assets/Art/_yeni/T46/` ve `docs/art/task46/` içindedir; oyun kodu entegrasyonu bu teslimin parçası değildir. Motor içinde oynatma testi yapılmadı; import ayarları ve animasyon inceleme GIF'leri sağlandı.
