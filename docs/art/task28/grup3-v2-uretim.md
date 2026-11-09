# Task28F · Grup 3 v2 üretim kaydı

[İnceleme levhası](grup3-v2.png) · [30 şeffaf PNG](../../../Assets/Art/_yeni/Obstacles)

## Bomba

| Dosya | Boyut | Katman ve sıra |
|---|---|---|
| bomb-v2.png | 384 × 384 | Onaylanan gövdenin birebir kopyası. |
| bomb-fuse-01…08.png | 384 × 384 | Yalnız fitil ucu ve kıvılcım. Gövdeyle merkez pivotu ortak. 08’den 01’e dön. |
| bomb-v2-urgent.png | 384 × 384 | 3, 2 ve 1’de kullanılacak sıcak gövde. Alfa kanalı normal gövdeyle birebir. |
| bomb-fuse-urgent.png | 384 × 384 | Acil gövdenin üzerinde, aynı pivotla daha büyük kıvılcım. |
| bomb-counter-label.png | 240 × 240 | Onaylanan boş krem etiket. Canlı rakam etiketin merkezinde. |

Normal ve acil karelerde etiket merkezde ve tuvali gövde tuvalinin %50,5’i. Onaylı gövde sabit koru içerir. Animasyon PNG’leri korun üstüne gelen parlamayı ve kıvılcımı taşır. Sekiz kare eş aralıklı periyodik fazlardan çıktı. Son kareden ilk kareye alfa değişimi diğer kare geçişlerinin aralığında kaldı.

## Asma kilit

Sekiz kare aynı 384 × 384 tuvali kullanır. Sıra **padlock-closed → padlock-open-01…04 → padlock-fall-01…03 → gizle**. Açılmada gümüş gövde aynı yerde kalır, pirinç kulp yükselip açılır. Düşüş kareleri aynı açık kilidin hareketini ve kaybolmasını gösterir. Krem yüz boştur. Kapalı kilitte sayı merkezi, sol üstten normalize (0,50, 0,644). Levhada tek hane %31,5 ve iki hane %28,5 tuval boyunda, lacivert ve ayrı metin olarak örneklendi.

## Zincir

- **chain-link.png**, 128 × 96 nötr ve içi şeffaf tek halkadır. Uzunluk boyunca aynı PNG çoğaltılır ve hat teğetine döndürülür.
- **chain-anchor-{altin,turkuaz,mor,mercan,yesil}.png**, 160 × 160. Merkez pivotu parçanın tam merkezine oturur. İşaretler sırasıyla yıldız, damla, hilal, kalp ve yaprak.
- **chain-break-01…04.png**, 256 × 160, nötr katman. Kopuşta hattın teğetine hizala. İki yarım halka ayrılır ve söner. Dördüncü kareden sonra katmanı gizle.
- Yerel levhada halka ve kopuş renkleri gri ton üzerinden aile gölge, orta ve ışık renklerine eşlendi. Aynı nötr PNG beş renge temel olur.

| Düzen | Merkez aralığı | En büyük sarkma | Komşu parça örtüşmesi | Alt komşuya en küçük boşluk |
|---|---|---|---|---|
| A kısa | 64 px | 0 px | 0 px² | 26 px |
| B kısa | 64 px | 8 px | 0 px² | 25 px |
| A uzun | 192 px | 0 px | 0 px² | 26 px |
| B uzun | 192 px | 21 px | 0 px² | 10 px |

Ölçüler levhadaki 64 px hücre dizilimine ait. B’nin orta noktasında sarkma en büyük, uçlarda sıfır. A/B seçimi açık.

## Duvar ve doğrulama

Oyunda kullanılan duvar dosyasının SHA-256 değeri korundu. Task28E’de hazırlanan yeni duvar [arşive taşındı](_arsiv/grup3-v1/wall-stripe.png).

30 PNG, saydamlık ve beklenen ölçülerle doğrulandı. Fitil ve asma kilit kareleri tuval sınırına değmiyor. 64, 96 ve 264 px levha görsel olarak incelendi. Bomba şişme/titremesi, tetikleme ve zincir yerleşimi oyun koduna bırakıldı. Kontrol yerel PNG provasıdır.

## Yerleşik imagegen istemleri

### Acil bomba ışığı

Kaynak render · exec-251e3431-7223-4956-b5f6-107ed550476b.png

> Precise localized edit of image 1, the approved black-navy toy bomb. Same exact shape, pose, short bent wick, solid round body, alpha background and absolute relative geometry. Keep the body dark navy-black with original cream specular and dark lower shading. Add a controlled but clearly visible warm red bounce light on the upper-right shoulder directly below its existing red ember, a secondary warm red rim down the upper-right quadrant. Center and lower-left remain navy black. Preserve the existing small red ember exactly; any larger spark will be added as a separate layer. No count, label, new ornament, detached sparks, background, floor, explosion, grain or text. Image 2 is the toy material reference. Single isolated exact asset on genuine transparent alpha.

### Asma kilit gövdesi

Kaynak render · exec-78ee7dd4-1e06-4779-9634-4d4f24b1f36d.png

> Create ONLY the silver BODY of a toy padlock for a puzzle game, WITHOUT the U shackle, which will be composited separately. The approved cream counter label in image 1 is a color and center readout reference; image 2 is the approved toy material reference. Straight-on front orthographic, no side perspective: a chunky rounded-square smooth solid SILVER / steel shell with a soft sculpted bevel, silhouette slightly wider than tall (1.15:1). Bright large upper-left reflected highlight and cool blue-gray shadow. Inset in the front, one LARGE blank warm cream enamel rectangular counter plate with broadly rounded corners, about 70% of the body width and 60% of height, empty flat center with ample room for two navy digits. Thin navy/cool metallic outline between the cream plate and silver body. The body top edge is clean and level, shackle sockets may be two very subtle small dark indications near the shoulders. NO actual shackle, no brass protrusions, no keyhole, no digits, no text, no border beyond the object, no other objects, no surface or external shadows, no brushed grain or scratches. Thick smooth 3D toy game material, isolated one asset on true transparent alpha with padding.
