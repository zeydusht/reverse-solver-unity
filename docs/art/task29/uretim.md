# Task29 · Oyun şeritleri, Jigo, Devam ve Kazanma

[Önizleme paftası](onizleme.png) · [HUD29](../../../Assets/Art/_yeni/HUD29/) · [Jigo](../../../Assets/Art/_yeni/Jigo/) · [Devam](../../../Assets/Art/_yeni/Devam/)

## Kaynaklar ve üretim

- Jigo'nun altı ana pozu için iki görsel birlikte kullanıldı. Turkuaz yapboz gövde, gümüş yüksük, ahşap saplı mavi fırça ve dört delikli krem düğme kalkan bu referanslardan alındı. [Task26 model paftası](../task26/kahraman-final.png) · [v06 film karesi](../task26/cekim-kareleri-v06/11-kahraman-bakis.png)
- Şerit, altın bordür, madalyon, krem haplar, kart, fiyat alanı ve efektler katmanlı alfa maskeleriyle çizildi. Turkuaz `#1C7F8C` ve gölge `#0F4F5A`, altın `#FBBA0E`, `#F38A03`, `#CE5D01` ve krem `#F8EBD0` kullanıldı. [Oyun paleti](../PALETTE.md)
- Parça ve saat ikonları, seviye rozeti ve pause gövdesi Task28 HUD paketinden aynen alındı. [Task28 HUD](../../../Assets/Art/_yeni/HUD/)
- Devam kartındaki para, onaylı yeşil ana tuş ve turkuaz ikincil tuş mevcut oyun görsellerinden kopyalandı. [Altın para](../../../Assets/_Game/Presentation/Resources/Art/m13-token-coin.png) · [Yeşil tuş](../../../Assets/_Game/Presentation/Resources/Art/btn2-primary.png) · [Turkuaz tuş](../../../Assets/_Game/Presentation/Resources/Art/btn2-secondary.png)
- Oyun provasında galeri arka planı ve oyun çerçevesiyle bir Task26 tablo karesi kullanıldı. Z2 arşivindeki mavi çekiç, Task28 boosterlarıyla birlikte yalnızca paftada yer alıyor. Bomba görseli ve sayaç etiketi Task28F paketinden. [Galeri](../../../Assets/_Game/Presentation/Resources/Art/game-background.png) · [Oyun çerçevesi](../../../Assets/_Game/Presentation/Resources/Art/board-frame.png) · [Tablo](../task26/cekim-kareleri-v06/tablo-01.png) · [Mavi çekiç](../task28/_arsiv/Z2-v3-mavi-cekic/booster-hammer-normal.png) · [Task28F engelleri](../../../Assets/Art/_yeni/Obstacles/)

## Yerleşim ve katmanlar

390 × 844 px oyun provasında üst şerit y=0, alt şerit y=724 konumunda. İkisi de 390 × 120 px. Alt şeritte y=810–843 aralığı home çubuğu için ayrıldı. Madalyon (143, 27) konumunda 104 × 104 px, maskeli portre 96 × 96 px. Katman sırası arkadan öne `jigo-medallion-base`, dairesel maske içindeki Jigo ve `jigo-medallion-rim`.

Kalan parça hapı 98 × 45 px, süre hapı 96 × 45 px. 390 px genişlikte mevcut oyun HUD'u sırasıyla 121 × 56 ve 118 × 56 px gösteriyor. Yeni örnekte genişlik ve yükseklikler yaklaşık %20 küçüldü. `LV`, seviye, parça sayısı, süre, booster ve bomba sayaçları ayrı metin katmanları. [Mevcut HUD ölçüleri](../../../Assets/_Game/Presentation/Ui/Hud.cs)

Devam provasında kart 326 × 450 px, saat 94 px ve fiyat alanı 128 × 42 px. Üzgün Jigo doğrudan `uzgun` karelerini kullanır. `+30`, 900 / 1900 / 2900 ve tuş etiketleri boş gövdeler üzerine yerleşir. Paftanın ana panelinde 900, alt satırında üç fiyat örneği bulunur. Kazanma provasında ışık, konfeti, Jigo ve metin bağımsız katmanlardan birleştirildi. Raster pafta bu katmanların sabit bileşimi. PNG kaynaklar oyuna bağlanmadı.

## Çıktı ölçüleri

| Klasör | PNG ve boyut |
|---|---|
| `HUD29` | `hud-top-strip`, `booster-bottom-strip` · 1170 × 360 px |
| `HUD29` | `jigo-medallion-base`, `jigo-medallion-rim` · 312 × 312 px |
| `HUD29` | `hud-moves-pill` · 294 × 135 px, `hud-time-pill` · 288 × 135 px |
| `HUD29` | `level-badge` · 216 × 216 px, `pause-normal` · 168 × 168 px, `icon-piece` ve `hud-icon-clock` · 120 × 120 px |
| `Devam` | `continue-panel` · 978 × 1350 px, `continue-price-field` · 384 × 126 px |
| `Devam` | `continue-clock`, `continue-coin` · 120 × 120 px |
| `Devam` | `continue-primary`, `decline-secondary` · 684 × 336 px |
| `Jigo/normal`, `tedirgin`, `korku`, `uzgun` | Her klasörde `01–06` · 256 × 256 px RGBA |
| `Jigo/mutlu` | `01–03` · 256 × 256 px RGBA |
| `Jigo/sevinc` | `01–08` · 640 × 640 px RGBA |
| `Jigo/sevinc/fx` | `light-rays` · 840 × 840 px, `confetti-coral`, `confetti-teal`, `confetti-gold`, `confetti-purple` · 72 × 72 px |
| `docs/art/task29` | `onizleme.png` · 1860 × 1826 px, üç ekran provasının her biri 390 × 844 px |

## Kareler

Ana pozlardan ara kareler aynı tuvalde bölgesel deformasyon ve alfa korunarak üretildi. Dört portre döngüsü için son kareden ilk kareye geçiş komşu kare geçişleriyle karşılaştırıldı. Karelerin şeffaflığı, boyutu ve benzersizliği denetlendi. Portreler 96 px, ikonlar 64 px paftada görsel olarak kontrol edildi.

| Hâl | Sıra ve hareket |
|---|---|
| Normal | `01–06 → 01`. Kameraya bakış, nefes ve 04'te en kapalı göz. Göz kırpmasını seyrek göstermek için açık göz karesi uzatılabilir. |
| Tedirgin | `01–06 → 01`, süre ≤10 sn. Kalkık kaş, kayan bakış, beliren ter ve fırçayı sıkma. |
| Korku | `01–06 → 01`, süre ≤3 sn veya bomba sayacı 1. Kalkan ağzın önünde yükselir, miğfer titrer. |
| Üzgün | `01–06 → 01`, kayıpta. Miğfer göze doğru iner ve dudak titrer. |
| Mutlu | `01–03`, bir parça çıkınca kısa tepki. Fırça 02'de en yüksek konumunda. |
| Sevinç | `01–08`. Yükselme, 04–05'te sıçrama tepesi ve iniş. Fırça ve kalkan farklı yönlerde hareket eder. Konfeti ve huzme ayrı oynatılabilir. |

## ImageGen promptları

Her çağrıda ortak giriş, aşağıdaki hâl metni ve ortak son ek bu sırayla kullanıldı. Görsel 1 Task26 model paftası, Görsel 2 v06 film karesi.

**Ortak giriş**

> Produce one game animation key sprite of JIGO. Image 1 is the official Task26 model sheet: copy its exact front character design, proportions and all props. Image 2 is the same hero in the final movie, a second appearance reference only. Preserve vivid turquoise square jigsaw body, exact connection at right and notch at left, wide brown eyes in familiar face, short distinct rounded arms, the same dimpled polished SILVER THIMBLE helmet with turned rim, the same small wooden BRUSH with blue paint and gold ferrule in his right hand (viewer LEFT), the same round ivory four-hole sewing-button SHIELD in his left (viewer RIGHT). No other costume or features.

| Hâl | Prompt bölümü |
|---|---|
| `normal` | Bust from helmet to natural bottom of the upper body, shoulders, and both hands. Calm friendly small smile and open brown eyes looking straight at the viewer. The blue paint brush peeks up at image LEFT at cheek height, with its handle visible briefly. The four-hole button shield stays low at image RIGHT. This is a clean base for an eye blink animation; eye pupils, upper and lower lids are separate, sharply readable, clearly spaced and face camera directly. |
| `tedirgin` | Bust from helmet to natural bottom of the upper body, with shoulders, hands, brush and low shield. Worried but friendly: eyebrows raised symmetrically, large clear open white eyes and centered brown irises that leave visible white space left and right for a later shifting gaze, mouth slightly tense in a small wavy frown. His visible hand tightly grips the brush handle, white space at upper right of face for one separate sweat-drop sprite. No existing sweat droplet, keep that patch unobstructed. |
| `korku` | Bust from helmet to shoulders, with hands and props. Startled pose: two very wide, round brown eyes peeking over the TOP of his own golden-cream four-hole button shield raised at image RIGHT to partially cover his lower face. Show both eyes and helmet clearly. The rim of the same button near mouth level, never hide the eyes. Brush tip at image LEFT. His helmet stays on his centered head tab, visibly identifiable and suitable for a small shiver. No extraneous effects. |
| `uzgun` | Bust from helmet to shoulders, brush on image LEFT angled slightly downward, button shield on image RIGHT held low. Sad, gentle expression, lowered gaze, inner eyebrows high with eyes still visible. The exact same silver thimble has slipped down a bit, its bottom rim close to and partly shading the top of one eye. A clearly visible small downturned lower lip. No tears or drops. Keep the blue brush tip in frame, plus silver thimble and four-hole button. |
| `mutlu` | Bust from helmet to shoulders and raised hands. Friendly delighted open smile, bright clear brown eyes, lifting the same blue paint-tipped brush vertically at image LEFT. Show the blue brush tip rising to about the helmet brim height, the ferrule and hand visible; the same cream four-hole button shield low at image RIGHT. Fit the entire upper body and props inside the image. Ready for a short lift reaction animation. |
| `sevinc` | FULL BODY celebration key pose. Both distinct short legs and feet shown in mid-jump, one foot tucked and the other extended downward. Triumphant joyful open smile and bright brown eyes. Jigo raises his blue paint brush high at image LEFT like a victory flag, with full tip, ferrule and grip visible; at image RIGHT, his other hand raises his own four-hole cream button shield away from body, slight angle, four holes still readable. Silver thimble firmly on the top puzzle tab, exact costume; generous empty padding on all sides, all props and feet included. |

**Ortak son**

> Square clean production framing, front view, cinematic warm upper-left toy lighting and softly shaded turquoise, body and face sculpted like source, original gear colors, simplified crisp forms at 96 px. One isolated cutout on genuine alpha. Transparent throughout outside the character, no stage, no floor or drop shadow, no scene, extra props, circular surround, panel, captions, text, logo, signatures or particles.

**UI ve efekt çizim tarifi**

> Use the Reverse Solver toy palette and clean bevels. Full-width teal ribbons have subtle surface shading and matching narrow cream and gold borders facing the board. Put a circular gold frame around a light teal well, keep its rim separate, and make the empty cream HUD pills narrower than the current layout. Make the continue card cream with a gold rim and a separate blank price field. Keep all type out of the source PNGs. Export the radial gold light and the four confetti shapes as separate transparent files.
