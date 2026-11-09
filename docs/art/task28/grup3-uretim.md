# Task28E · Grup 3 üretim kaydı

[Önce/sonra levhası](grup3-once-sonra.png) · [13 şeffaf PNG](../../../Assets/Art/_yeni/Engeller)

## Dosyalar ve katmanlar

| Dosya | Piksel ölçüsü | Kullanım |
|---|---|---|
| bomb-v2.png | 384 × 384 | Siyah-lacivert gövde ve kırmızı kor. |
| bomb-counter-label.png | 240 × 240 | Boş krem sayaç etiketi, ayrı sprite. |
| board-nail.png | 384 × 384 | Eğik gümüş baş ve görünen gövde. |
| nail-counter-badge.png | 128 × 128 | Boş kırmızı sayaç rozeti, ayrı sprite. |
| chain-link-v3-{altin,turkuaz,mor,mercan}.png | 128 × 64 | Dört tekrar karosu, mevcut alfa ve dikiş hizası. |
| chain-lock-v3-{altin,turkuaz,mor,mercan}.png | 160 × 160 | Yıldız, damla, hilal ve kalp, mevcut gövde ve askı hizası. |
| wall-stripe.png | 144 × 72 | Kırmızı ve uyarı beyazı bariyer karosu. |

PNG’lere rakam işlenmedi. Yerel provada her sayaç üç katmanla çizildi. İlk katman engel gövdesi, ikincisi boş etiket veya rozet, üçüncüsü canlı sayı. Aşağıdaki ankrajlar gövde sprite tuvali içinde sol üstten normalize edilmiştir.

| Sayaç | Rozet merkezi (x, y) | Rozet tuvali / gövde tuvali | Rakam boyutu / gövde tuvali (1 / 2 hane) |
|---|---|---|---|
| Bomba | (0,50, 0,50) | 0,505 | 0,47 / 0,36, lacivert |
| Çivi | (0,252, 0,76) | 0,37 | 0,25 / 0,185, krem |

Gövde PNG’lerinin mevcut anahtarları korundu. Oyuna bağlarken boş etiket/rozet ayrı sprite olarak eklenmeli ve canlı rakam onun merkezine taşınmalı. Bomba etiketi ve rakamı, kritik nabız sırasında gövdeyle birlikte ölçeklenir. Duvar köşesi mevcut oyundaki yatay ve dikey şerit birleşimiyle gösterildi.

## Kontrol

- Mevcut 11 PNG ile ad ve boyutlar eşleşti. İki boş sayaç yüzü yeni dosya.
- Sekiz zincir/kilit PNG’sinin alfa kanalları mevcut karşılıklarıyla birebir. Dört zincir ve duvar karosunda sol ve sağ uç RGBA sütunları birebir. Zincir açıklıkları şeffaf.
- Levhada 264, 96 ve 64 px örnekleri, 64 px hücreli resimli renkli/gri ton prova, çivide tek ve iki hane, bombada normal ve kritik nabız tepesi var.
- Oyun kaynakları, onaylı Booster ve HUD PNG’leri ve palet dahil 41 dosyanın SHA-256 değerleri değişmedi. Kontrol yerel PNG provasıdır. Canlı Unity testi ve entegrasyon yapılmadı.

## Üretim istemleri

İki engel gövdesi yerleşik imagegen ile üretildi. Girdi olarak mevcut bomba ve çivi silüeti ile onaylı pembe çekiç kullanıldı. Zincir, duvar ve boş sayaç yüzleri mevcut geometriler üzerinden pürüzsüz gradyan ve kabartmayla oluşturuldu.

### Bomba

Kaynak render · exec-02ef07ec-ff41-46b5-a135-26f842276e68.png

> Mobile game production sprite edit. Input 1 is the current bomb reference. Input 2 is the approved toy UI surface reference. Create the bomb BODY ONLY, without its round beige label and without its big gold ring; these will be a separate sprite layer. Preserve the orientation: a perfectly plump front-facing round bomb, simple blue-black navy body, a small navy collar and short curved wick coming from the top center, arcing to upper right. Black-navy smooth molded body with cool navy edge catchlight and a broad cream reflection on the upper-left hemisphere. Use deep ink #0B2A36 and almost-black shadows, not teal. The wick must be bold and large enough to read at 64 px, with a visible saturated red rounded ember, subtle red bounce near the tip, no separate sparks. Optional very small royal-gold accent at the collar only. Clean center of sphere, ready for a separate cream circular counter label. Friendly tactile animation toy material like reference, broad gradients, plump molded volume, no scratches, grain or wear, no scene, floor, cast shadow, text, numbers or watermark. One single centered asset on genuine transparent alpha, with margin all around.

### Çivi

Kaynak render · exec-bf10c24a-3b53-4003-8048-cc9f0b9172d6.png

> Mobile game production sprite edit. Input 1 is the existing proper nail silhouette reference. Input 2 is the approved toy UI surface reference. Make one shiny SILVER headed toy nail viewed in a friendly 3/4 side view, driven diagonally at a slight slant into an unseen surface, cap toward upper left and clearly visible substantial silver-gray straight shaft towards lower right, with one short visible tapered point. Show the large bright silver disk head as an angled ellipse with a raised thickness and visible rim. The head takes about half of the icon, the attached exposed shaft extends at least one head radius beyond it so the silhouette reads immediately as a NAIL even in grayscale and at 64px. Gold should be absent. No slot, screw threads, surrounding board, glyph, badge or number. Soft round toy-grade volume, large clean silver specular and bright silver head, medium silver shaft with a navy gray edge, warm upper-left reflected light, no realistic scratches or noise. Short chunky proportions. Single object isolated and fully visible, transparent alpha with clean margin, no external shadow.
