# T50 — Harita bölüm düğmeleri (yalnız Öğe 2)

39 şeffaf **RGBA PNG @3x** parça ve üç **390×844 px** oyun içi birleşik önizleme.
Düğme parçaları sayısızdır; numaralar yalnız önizlemelere Baloo 2 ExtraBold ile eklenmiştir. Rozetler ve Jigo ayrı katmandır.

## Seçenek A

Düz mavi madalyon ve kalın, katmanlı 3B altın kenar; en sade ve doğrudan okunan seçenek.
Şu anki bölüm 1,25× büyük, parlak çerçeveli ve hafif haleli; tamamlananlarda ayrı altın tik bulunur.

Önizleme: [harita_A.png](secenekler/harita_A.png).

Parça klasörü: `Assets/Art/_yeni/T50/harita/A/`

- `node_completed.png`
- `node_current.png`
- `node_locked.png`
- `node_hard.png`
- `node_lesson.png`
- `badge_check.png`
- `badge_lock.png`
- `badge_kont.png`
- `badge_bomb.png`
- `badge_chain.png`
- `badge_wall.png`
- `jigo_marker.png`
- `path_dot.png`

## Seçenek B

Müze rozeti gibi oyulmuş altın çerçeve, akantus kıvrımları ve küçük tepe süsü.
Mavi, bordo ve gümüş durumlar aynı oymalı biçimi korur; galerinin mevcut altın tablo çerçeveleriyle uyumludur.

Önizleme: [harita_B.png](secenekler/harita_B.png).

Parça klasörü: `Assets/Art/_yeni/T50/harita/B/`

- `node_completed.png`
- `node_current.png`
- `node_locked.png`
- `node_hard.png`
- `node_lesson.png`
- `badge_check.png`
- `badge_lock.png`
- `badge_kont.png`
- `badge_bomb.png`
- `badge_chain.png`
- `badge_wall.png`
- `jigo_marker.png`
- `path_dot.png`

## Seçenek C

Oyunun konusunu yansıtan, kalın altın kenarlı tek yapboz parçası biçimi.
Büyük boş orta yüzey numarayı taşır; kilitli durumda gümüş, zor bölümde bordo yüzey kullanılır.

Önizleme: [harita_C.png](secenekler/harita_C.png).

Parça klasörü: `Assets/Art/_yeni/T50/harita/C/`

- `node_completed.png`
- `node_current.png`
- `node_locked.png`
- `node_hard.png`
- `node_lesson.png`
- `badge_check.png`
- `badge_lock.png`
- `badge_kont.png`
- `badge_bomb.png`
- `badge_chain.png`
- `badge_wall.png`
- `jigo_marker.png`
- `path_dot.png`

## Ölçüler ve durum eşlemesi

| Dosya | PNG ölçüsü (@3x) | Mantıksal ölçü (@1x) |
| --- | --- | --- |
| `node_completed.png` | 216×216 | 72×72 |
| `node_current.png` | 270×270 | 90×90 (1,25×) |
| `node_locked.png` | 216×216 | 72×72 |
| `node_hard.png` | 216×216 | 72×72 |
| `node_lesson.png` | 216×216 | 72×72 |
| `badge_*.png` | 72×72 | 24×24 |
| `jigo_marker.png` | 90×102 | 30×34 |
| `path_dot.png` | 24×24 | 8×8 |

- Tamamlandı: `node_completed` + `badge_check`.
- Şu anki: `node_current` + üstünde `jigo_marker`.
- Kilitli: `node_locked` + `badge_lock`.
- Zor bölüm: `node_hard` + `badge_kont`; kafatası kullanılmadı.
- Engel dersi: `node_lesson` + `badge_bomb`, `badge_chain`, `badge_wall` veya `badge_lock`.
- Tamamlanan özel bölümlere ayrıca `badge_check` eklenebilir.

## Önizleme yerleşimi ve referanslar

Her önizlemede 1–6 tamamlandı, 7 şu anki, 8–12 kilitlidir. 5 zor bölüm (bordo + Kont mührü + tik), 4 bomba dersi (bomba + tik) olarak gösterildi. Altın yol noktaları düğmeler arasında kalır.

Kaynak ekran: `docs/art/task29/pm72/once/harita.png`. Yalnız eski düğmelerin, numaraların, Oyna balonunun ve küçük Jigo'nun kapladığı yerler, imagegen ile temizlenmiş arka planın ilgili bölgeleriyle dolduruldu; bunun dışındaki koridor/duvar kaynak görüntüden korundu. Üst alan (y=0–91), alt şerit (y=746–843), En üste ve En alta düğmeleri kaynakla piksel piksel aynıdır.

Stil referansı: `docs/art/task29/pm72/once/menu.png`.
Jigo başı: onaylı `Assets/Art/_yeni/Jigo_v3/normal/01.png` referansından imagegen ile baş işareti üretildi; gümüş yüksük, turkuaz yapboz yüzü, kahverengi gözler ve mutlu ifade korundu.

Engel rozetleri aşağıdaki onaylı kaynakların doğrudan kırpılıp ölçeklenmiş pikselleridir; yeni ikon çizilmedi:

| Çıktı | Onaylı kaynak (`Assets/_Game/Presentation/Resources/Art/`) |
| --- | --- |
| `badge_lock.png` | `t20-icon-lock.png` |
| `badge_bomb.png` | `icon-bomb.png` |
| `badge_chain.png` | `icon-chain.png` |
| `badge_wall.png` | `icon-wall.png` |

## Üretim ve doğrulama

Yerleşik **imagegen** kullanıldı. Atlasların parçalara ayrılması, RGBA dışa aktarımı, onaylı ikonların ölçeklenmesi ve numaralı birleşimler geçici klasörde Pillow ile yapıldı. Yeni proje kodu, sahne, Resources veya başka proje dosyası değiştirilmedi; commit yapılmadı.

39 parçanın RGBA modu, gerçek saydamlığı, üçe bölünebilen @3x ölçüleri, 1,25× güncel düğme boyutu ve üç önizlemenin 390×844 ölçüsü doğrulandı. Üst/alt şerit ve iki gezinme düğmesi için kaynakla piksel eşitliği kontrol edildi. Üç önizleme ve parça toplu görünümü görsel olarak incelendi.

## Kullanılan görsel tarifleri

Her tarifin başında görevdeki ortak stil bloğu aynen yer alır. A/B/C tarifleri aynı durumları içeren ayrı saydam atlaslar üretir; atlaslar teslimde tek tek dosyalara ayrılmıştır.

### A — düz madalyon

Referans: menu.png: mevcut oyun stili referansı.

```text
Cozy premium mobile puzzle game UI, "old money" museum-gallery mood.

Palette:
- deep royal blue #1F3F8F → Royal-Match blue #2F6BD8 gradients,
- warm cream #F6EBD3 panels,
- antique gold #D9A441 trims with soft drop shadow,
- green #3DBA4E primary buttons with layered 3D bevel.

Typography: chunky rounded bold (Baloo 2 ExtraBold look), cream fill, blue outline, gold extrusion.

Thick, saturated, opaque shapes, soft inner bevels, no thin lines, no ribbons/banners, no neon, no flat vector look, no text other than specified.

Mascot Jigo (existing approved character: turquoise round body, silver thimble helmet, blue-tipped brush, button shield). Keep its exact look. Never frowning.

Use case: stylized-concept. Production mobile game UI sprite atlas, option A: simple ROUND medallions with thick layered 3D edges. Render a square 1536x1536 TRANSPARENT RGBA atlas, precisely 3 equal columns x 3 equal rows of 512x512 pixels. Each sprite isolated in its own cell, centered at cell center; never touches adjacent cell. NO text, NO numbers, NO letters, NO grid lines or background. Face-on camera, no tilt, consistent upper-left lighting. Fine polish of premium sculpted toy enamel and gold, similar to supplied menu style.

Row 1:
cell 1: completed medallion, blue enamel circular face, thick antique gold bezel, EMPTY center, NO attached check badge.
cell 2: CURRENT medallion, same blue face but medallion 1.25 times bigger than cell1, luminous bright layered gold rim and restrained soft gold halo, EMPTY center, no mascot.
cell 3: locked medallion, muted desaturated gray-blue opaque face, thick SILVER bezel, EMPTY center, no attached lock.
Row 2:
cell 1: HARD medallion, burgundy red enamel face, darker antique gold thick rim, EMPTY center, no attached seal.
cell 2: LESSON medallion, normal blue enamel face, thick gold rim, EMPTY center, no attached obstacle.
cell 3: small separate circular gold CHECK badge with raised gold checkmark and dark blue enamel inset, no text.
Row 3:
cell 1: separate Count's noble wax SEAL badge, burgundy wax edge and gold crown crest embossed, aristocratic museum emblem, NO skull, NO letters.
cell 2: ONE small round thick gold path dot, sculpted bead with highlight.
cell 3: leave totally EMPTY transparent.
Sizing: normal nodes visible width 350px, current 437px incl halo, check badge 180px, seal badge 180px, path dot 76px. Equal 512px cells, generous alpha separation. Actual transparent background, no checkerboard. Deliver atlas only.
```

### B — oymalı müze rozeti

Referans: menu.png: mevcut oyun stili referansı.

```text
Cozy premium mobile puzzle game UI, "old money" museum-gallery mood.

Palette:
- deep royal blue #1F3F8F → Royal-Match blue #2F6BD8 gradients,
- warm cream #F6EBD3 panels,
- antique gold #D9A441 trims with soft drop shadow,
- green #3DBA4E primary buttons with layered 3D bevel.

Typography: chunky rounded bold (Baloo 2 ExtraBold look), cream fill, blue outline, gold extrusion.

Thick, saturated, opaque shapes, soft inner bevels, no thin lines, no ribbons/banners, no neon, no flat vector look, no text other than specified.

Mascot Jigo (existing approved character: turquoise round body, silver thimble helmet, blue-tipped brush, button shield). Keep its exact look. Never frowning.

Use case: stylized-concept. Production mobile game UI sprite atlas, option B: ROUND museum honor medallions surrounded by thick CARVED GOLD frames, elegant bold sculpted acanthus scrolls and small gold crest at the top, enamel recess, a restrained ornamental portrait-gallery look. Clearly more ornamental than a plain ring, still readable and chunky at mobile size. Render a square 1536x1536 TRANSPARENT RGBA atlas, precisely 3 equal columns x 3 equal rows of 512x512 pixels. Each sprite isolated in its own cell, centered at cell center; never touches adjacent cell. NO text, NO numbers, NO letters, NO grid lines or background. Face-on camera, no tilt, consistent upper-left lighting. Fine polish of premium sculpted toy enamel and gold, similar to supplied menu style.

Row 1:
cell 1: completed medallion, blue enamel circular face, thick carved antique gold acanthus bezel, EMPTY center, NO attached check badge.
cell 2: CURRENT medallion, same blue face but medallion 1.25 times bigger than cell1, luminous bright layered gold rim and restrained soft gold halo, EMPTY center, no mascot.
cell 3: locked medallion, muted desaturated gray-blue opaque face, thick carved SILVER acanthus bezel, EMPTY center, no attached lock.
Row 2:
cell 1: HARD medallion, burgundy red enamel face, darker carved antique gold thick rim, EMPTY center, no attached seal.
cell 2: LESSON medallion, normal blue enamel face, thick carved gold rim, EMPTY center, no attached obstacle.
cell 3: small separate circular gold CHECK badge with raised gold checkmark and dark blue enamel inset, no text.
Row 3:
cell 1: separate Count's noble wax SEAL badge, burgundy wax edge and gold crown crest embossed, aristocratic museum emblem, NO skull, NO letters.
cell 2: ONE small round thick gold path dot, sculpted bead with highlight.
cell 3: leave totally EMPTY transparent.
Sizing: normal nodes visible width 330px, current 412px incl halo, check badge 180px, seal badge 180px, path dot 76px. Equal 512px cells, generous alpha separation. Actual transparent background, no checkerboard. Deliver atlas only.
```

### C — yapboz düğme

Referans: menu.png: mevcut oyun stili referansı.

```text
Cozy premium mobile puzzle game UI, "old money" museum-gallery mood.

Palette:
- deep royal blue #1F3F8F → Royal-Match blue #2F6BD8 gradients,
- warm cream #F6EBD3 panels,
- antique gold #D9A441 trims with soft drop shadow,
- green #3DBA4E primary buttons with layered 3D bevel.

Typography: chunky rounded bold (Baloo 2 ExtraBold look), cream fill, blue outline, gold extrusion.

Thick, saturated, opaque shapes, soft inner bevels, no thin lines, no ribbons/banners, no neon, no flat vector look, no text other than specified.

Mascot Jigo (existing approved character: turquoise round body, silver thimble helmet, blue-tipped brush, button shield). Keep its exact look. Never frowning.

Use case: stylized-concept. Production mobile game UI sprite atlas, option C: JIGSAW PUZZLE PIECE SHAPED buttons. A single chunky upright square jigsaw piece silhouette with one large rounded tab at top, one at right, one concave socket at left and one at bottom. The whole edge is layered 3D GOLD; the broad empty central face is blue enamel. MUST be visibly a puzzle PIECE silhouette, not round and not a circle with a puzzle icon. Toy-like rounded corners, premium bevels. All five states have the exact same puzzle silhouette.. Render a square 1536x1536 TRANSPARENT RGBA atlas, precisely 3 equal columns x 3 equal rows of 512x512 pixels. Each sprite isolated in its own cell, centered at cell center; never touches adjacent cell. NO text, NO numbers, NO letters, NO grid lines or background. Face-on camera, no tilt, consistent upper-left lighting. Fine polish of premium sculpted toy enamel and gold, similar to supplied menu style.

Row 1:
cell 1: completed puzzle button, blue enamel puzzle-shaped face, thick antique gold bezel, EMPTY center, NO attached check badge.
cell 2: CURRENT puzzle button, same blue face but puzzle button 1.25 times bigger than cell1, luminous bright layered gold rim and restrained soft gold halo, EMPTY center, no mascot.
cell 3: locked puzzle button, muted desaturated gray-blue opaque face, thick SILVER bezel, EMPTY center, no attached lock.
Row 2:
cell 1: HARD puzzle button, burgundy red enamel face, darker antique gold thick rim, EMPTY center, no attached seal.
cell 2: LESSON puzzle button, normal blue enamel face, thick gold rim, EMPTY center, no attached obstacle.
cell 3: small separate circular gold CHECK badge with raised gold checkmark and dark blue enamel inset, no text.
Row 3:
cell 1: separate Count's noble wax SEAL badge, burgundy wax edge and gold crown crest embossed, aristocratic museum emblem, NO skull, NO letters.
cell 2: ONE small round thick gold path dot, sculpted bead with highlight.
cell 3: leave totally EMPTY transparent.
Sizing: normal nodes visible width 340px, current 425px incl halo, check badge 180px, seal badge 180px, path dot 76px. Equal 512px cells, generous alpha separation. Actual transparent background, no checkerboard. Deliver atlas only.
```

### Jigo başı işareti

Referans: Assets/Art/_yeni/Jigo_v3/normal/01.png: onaylı karakter kimliği.

```text
Cozy premium mobile puzzle game UI, "old money" museum-gallery mood.

Palette:
- deep royal blue #1F3F8F → Royal-Match blue #2F6BD8 gradients,
- warm cream #F6EBD3 panels,
- antique gold #D9A441 trims with soft drop shadow,
- green #3DBA4E primary buttons with layered 3D bevel.

Typography: chunky rounded bold (Baloo 2 ExtraBold look), cream fill, blue outline, gold extrusion.

Thick, saturated, opaque shapes, soft inner bevels, no thin lines, no ribbons/banners, no neon, no flat vector look, no text other than specified.

Mascot Jigo (existing approved character: turquoise round body, silver thimble helmet, blue-tipped brush, button shield). Keep its exact look. Never frowning.

Use case: identity-preserve / background-extraction. Asset: jigo_marker.png, isolated head marker for a mobile game map. Input image 1 is the EXACT approved Jigo character, its identity has priority over the generic round-body shorthand above.
Extract a small HEAD-ONLY icon from the approved character: the exact silver perforated thimble helmet and exact turquoise face with large brown eyes, happy smile and soft peach cheeks. Preserve its characteristic jigsaw-shaped head silhouette and the helmet shape, rendering style, colors and friendly expression. Crop out BOTH arms, brush, button shield and lower body. Keep the helmet, face, cheeks, mouth. The bottom of head should be naturally rounded just below smile, like a cropped mascot portrait. DO NOT reinterpret or redesign the face, no extra badge, no circle, no pin, no sign or frame, no text or number. Only the turquoise head and helmet, centered on actual transparent RGBA, with generous transparent padding. No backing plate. Front view, match the exact reference.

```

### Arka plan temizliği

Referans: docs/art/task29/pm72/once/harita.png: düzenlenen kaynak ekran; teslimde yalnız eski öğelerin izi için kullanıldı.

```text
Cozy premium mobile puzzle game UI, "old money" museum-gallery mood.

Palette:
- deep royal blue #1F3F8F → Royal-Match blue #2F6BD8 gradients,
- warm cream #F6EBD3 panels,
- antique gold #D9A441 trims with soft drop shadow,
- green #3DBA4E primary buttons with layered 3D bevel.

Typography: chunky rounded bold (Baloo 2 ExtraBold look), cream fill, blue outline, gold extrusion.

Thick, saturated, opaque shapes, soft inner bevels, no thin lines, no ribbons/banners, no neon, no flat vector look, no text other than specified.

Mascot Jigo (existing approved character: turquoise round body, silver thimble helmet, blue-tipped brush, button shield). Keep its exact look. Never frowning.

Use case: precise-object-edit. Edit target image: supplied screenshot of the existing level map. This is ONLY background cleanup for subsequent compositing. Preserve image canvas aspect ratio, ALL corridor architecture, paintings, columns, vases, stairs, red carpet, lighting and textures EXACTLY. Preserve the complete top area including title "Bölümler" and back arrow, bottom navigation bar and ALL four tab icons, and the "En üste" and "En alta" buttons with their exact locations and appearance.

REMOVE ONLY every gold circular level node (including the half-visible gold node at the top of the carpet), all their digits 1,2,3,4,5,6,7, the green "Oyna" bubble and its pointer, and the tiny turquoise Jigo at level1. Inpaint their footprints with the SAME underlying continuous red carpet and stair surfaces, keeping gold carpet borders naturally connected. Do not blur, crop, shift, restyle, add elements, move navigation, or add labels. The result must be this original empty museum staircase map, ready for replacement level buttons. Opaque screenshot, same portrait composition.
```
