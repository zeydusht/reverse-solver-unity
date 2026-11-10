# T43 · Durdur, ödül ve sonraki tablo

[Önizleme](onizleme.png) · [Görev](../CODEX_TASK43.md)

**160 RGBA PNG**. Paftanın üst sırasındaki altı 390 × 844 px maket efektlerin önce ve sonra hâlini, Durdur sütununu ve kazanma ekranını gösterir. Alt sırada üç sandık ekranı, 8 açılma karesi, 6 kumaş karesi, 2 Kont pozu ve 3 sürü yoğunluğu var.

## Patlama ölçüsü

T42 ve T43 için aynı M1 tahta, aynı hedef hücre ve üçüncü kare kullanıldı. 768 px efekt tuvali için 204 px bir hedef hücreye karşılık gelir. Paftada hücre genişliği 54 px.

| Kare 03 | Opak piksel (alfa ≥ 245) | Beyaz-sarı opak piksel |
|---|---:|---:|
| Çekiç darbesi · T42 | 57.160 | 52.512 |
| Parça çıkışı · T42 | 48.877 | 35.315 |
| Parça çıkışı · T43 | 140.909 | 63.205 |
| Engel dağılması · T42 | 39.661 | 11.391 |
| Engel dağılması · T43 | 136.236 | 58.664 |

Beyaz-sarı sütununda RGB eşikleri sırasıyla 246, 216 ve 126. T43’te ilk üç kare dolu bir merkez, kalın halka ve doygun parçacıklar taşır. 18 FPS’de bu vuruş yaklaşık 0,17 sn, tüm geçiş 0,50 sn. Renk değiştirmek için yalnız nötr konfeti katmanını boya ve ışıkla senkron oynat.

## Durdur sütunu

Royal Match’in 3,5 sn karesinde tuşlar arası adım yaklaşık 46 px / 234 px ekran genişliği. M1’de mevcut Durdur merkezi yaklaşık (348, 786) px. Üstünde Titreşim, Ses, Müzik ve Çık merkezleri y=710, 634, 558 ve 481 px. Sütun ve boş seviye rozeti 390 × 1122 kaynakta, 1:3 ölçekte (260, 390) px ekran başlangıcına yerleşir. [Ayarlar referansı](../booster%20ref/kareler/ayarlar_genel.png)

- Arka planı %74 alfa ile karart, Durdur tuşunu üstte çiz. Açılmanın dört karesini 14 FPS’de oynat. Son karede Seviye 19 örneği için boş krem rozet var. Metin merkezi yaklaşık (321, 424) ekran px.
- Yeşil ve kırmızı tuş gövdeleri, krem ikonlar ve kapalı çizgisi ayrı. Açık ve kapalı birleşik tuşlar da hazır. Açılma karesi düzenine göre sonrasında ilgili tuşun durumunu çiz.

## Sandık ve ilerleme

- 3., 5., 10. bölüm ve sonraki sandıklar aynı katmanlarla gösterilebilir. Paftada 5. bölüm ödülü ve Seviye 8’den sonraki ilerleme örnekleniyor. Ödül değerleri ve bölüm metinleri canlı.
- Kapalı, sallanma, açılma, huzme ve saçılma aynı 960 × 828 px tuvalde. Kasa alt pivotu (480, 756) px. Sallanma 8 FPS. 12 FPS açılışta kasa ve kapak altta, huzme ve ödüller üstte. 01–02’de ışık ve saçılmayı gizle. 03–08’de aynı numarayı eşle. Sekizinci kareden sonra ödül slotlarına ve yeşil Topla tuşuna geç.
- İlerleme dolgusunu 680 × 70 px kanalın içinde (14, 13) px ofsetten sola hizala, ilerlemeye göre sağdan kırp. Boş mavi kanal, dolgu, panel ve sandık ucu ayrı. Paftadaki dolgu %60.

## Sonraki tablo ve sürü

- Sonraki tabloyu 942 × 456 px maske ile kırp. Arkaya Kont gölgesi, öne resim, altın çerçeve, kumaş ve mühür sırasıyla gelir. Kumaş 12 FPS’de yukarı kalkar. Mühür kapalı kartın sağ üst bölgesinde. Paftadaki tablo bir yerleşim örneği. [Örnek resim](../../../Assets/Art/Paintings_v2/painting-06.jpg)
- Az, orta ve çok arka sürüde 8, 16 ve 26 Jiggle var. Öndeki tam boy Jigo ayrı. T41 koşu kareleriyle yeni Jiggle’ları yanlardan getir ve +4 örneğini boş rozete canlı yaz. [T41 koşu kareleri](../../../Assets/Art/_yeni/T41/Juice/Rescued/Run/)

## Dosya listesi

— tek görsel veya kodla sürülen katman anlamına gelir. Kare dizisi bağlantısı ilk kareyi açar.

### Parça çıkışı ve engel · 72 PNG

| Dosya | Boyut (px) | Kare | FPS | Kullanım |
|---|---:|---:|---:|---|
| [PiecePop/light/01–09.png](../../../Assets/Art/_yeni/T43/Juice/PiecePop/light/01.png) | 768 × 768 | 9 | 18 | Beyaz-sarı opak merkez ve kalın şok halkası. İlk 3 kare pop. |
| [PiecePop/tint-neutral/01–09.png](../../../Assets/Art/_yeni/T43/Juice/PiecePop/tint-neutral/01.png) | 768 × 768 | 9 | 18 | Ayrı boyanabilir gri konfeti ve yıldız. light ile senkron. |
| [PiecePop/turquoise/01–09.png](../../../Assets/Art/_yeni/T43/Juice/PiecePop/turquoise/01.png) | 768 × 768 | 9 | 18 | Opak ışık ve parçanın renk ailesindeki dolu yıldız/konfeti. |
| [PiecePop/green/01–09.png](../../../Assets/Art/_yeni/T43/Juice/PiecePop/green/01.png) | 768 × 768 | 9 | 18 | Opak ışık ve parçanın renk ailesindeki dolu yıldız/konfeti. |
| [PiecePop/pink/01–09.png](../../../Assets/Art/_yeni/T43/Juice/PiecePop/pink/01.png) | 768 × 768 | 9 | 18 | Opak ışık ve parçanın renk ailesindeki dolu yıldız/konfeti. |
| [PiecePop/purple/01–09.png](../../../Assets/Art/_yeni/T43/Juice/PiecePop/purple/01.png) | 768 × 768 | 9 | 18 | Opak ışık ve parçanın renk ailesindeki dolu yıldız/konfeti. |
| [PiecePop/red/01–09.png](../../../Assets/Art/_yeni/T43/Juice/PiecePop/red/01.png) | 768 × 768 | 9 | 18 | Opak ışık ve parçanın renk ailesindeki dolu yıldız/konfeti. |
| [Fracture/01–09.png](../../../Assets/Art/_yeni/T43/Juice/Fracture/01.png) | 768 × 768 | 9 | 18 | Büyük metal parça, kırık halka ve keskin kıvılcım. İlk 3 kare pop. |

### Durdur · 22 PNG

| Dosya | Boyut (px) | Kare | FPS | Kullanım |
|---|---:|---:|---:|---|
| [dim.png](../../../Assets/Art/_yeni/T43/Pause/dim.png) | 1170 × 2532 | 1 | — | Arka plan için yaklaşık %74 koyu lacivert katman. |
| [level-badge.png](../../../Assets/Art/_yeni/T43/Pause/level-badge.png) | 360 × 101 | 1 | — | Küçük krem rozet. Seviye 19 canlı metin örneği. |
| [Layers/green-normal.png](../../../Assets/Art/_yeni/T43/Pause/Layers/green-normal.png) | 232 × 232 | 1 | — | Onaylı booster gövdesi ve altın halkalar. |
| [Layers/green-pressed.png](../../../Assets/Art/_yeni/T43/Pause/Layers/green-pressed.png) | 232 × 232 | 1 | — | Basılı yeşil gövde. |
| [Layers/red-normal.png](../../../Assets/Art/_yeni/T43/Pause/Layers/red-normal.png) | 232 × 232 | 1 | — | Çık düğmesi için kırmızı disk, altın halkalar korunur. |
| [Layers/red-pressed.png](../../../Assets/Art/_yeni/T43/Pause/Layers/red-pressed.png) | 232 × 232 | 1 | — | Basılı kırmızı gövde. |
| [Layers/icon-vibration.png](../../../Assets/Art/_yeni/T43/Pause/Layers/icon-vibration.png) | 152 × 152 | 1 | — | Krem ve altın sembol. Ortak merkez (76, 76) px. |
| [Layers/icon-sound.png](../../../Assets/Art/_yeni/T43/Pause/Layers/icon-sound.png) | 152 × 152 | 1 | — | Krem ve altın sembol. Ortak merkez (76, 76) px. |
| [Layers/icon-music.png](../../../Assets/Art/_yeni/T43/Pause/Layers/icon-music.png) | 152 × 152 | 1 | — | Krem ve altın sembol. Ortak merkez (76, 76) px. |
| [Layers/icon-exit.png](../../../Assets/Art/_yeni/T43/Pause/Layers/icon-exit.png) | 152 × 152 | 1 | — | Krem ve altın sembol. Ortak merkez (76, 76) px. |
| [Layers/off-slash.png](../../../Assets/Art/_yeni/T43/Pause/Layers/off-slash.png) | 152 × 152 | 1 | — | Kapalı hâl için mercan çapraz çizgisi. |
| [Buttons/vibration-on.png](../../../Assets/Art/_yeni/T43/Pause/Buttons/vibration-on.png) | 232 × 232 | 1 | — | Altın halkalı yuvarlak tuş. |
| [Buttons/vibration-off.png](../../../Assets/Art/_yeni/T43/Pause/Buttons/vibration-off.png) | 232 × 232 | 1 | — | Yeşil gövde, kapalı hâli gösteren çizgi. |
| [Buttons/sound-on.png](../../../Assets/Art/_yeni/T43/Pause/Buttons/sound-on.png) | 232 × 232 | 1 | — | Altın halkalı yuvarlak tuş. |
| [Buttons/sound-off.png](../../../Assets/Art/_yeni/T43/Pause/Buttons/sound-off.png) | 232 × 232 | 1 | — | Yeşil gövde, kapalı hâli gösteren çizgi. |
| [Buttons/music-on.png](../../../Assets/Art/_yeni/T43/Pause/Buttons/music-on.png) | 232 × 232 | 1 | — | Altın halkalı yuvarlak tuş. |
| [Buttons/music-off.png](../../../Assets/Art/_yeni/T43/Pause/Buttons/music-off.png) | 232 × 232 | 1 | — | Yeşil gövde, kapalı hâli gösteren çizgi. |
| [Buttons/exit.png](../../../Assets/Art/_yeni/T43/Pause/Buttons/exit.png) | 232 × 232 | 1 | — | Altın halkalı yuvarlak tuş. |
| [opening/01–04.png](../../../Assets/Art/_yeni/T43/Pause/opening/01.png) | 390 × 1122 | 4 | 14 | Alttan üste Titreşim, Ses, Müzik, Çık. Son karede boş seviye rozeti. |

### Ödül sandığı · 45 PNG

| Dosya | Boyut (px) | Kare | FPS | Kullanım |
|---|---:|---:|---:|---|
| [Layers/body.png](../../../Assets/Art/_yeni/T43/Chest/Layers/body.png) | 576 × 359 | 1 | — | Alt gövde, iç hazne görünür. |
| [Layers/lid.png](../../../Assets/Art/_yeni/T43/Chest/Layers/lid.png) | 577 × 270 | 1 | — | Kapak ayrı. Alt orta nokta mandal pivotu. |
| [closed.png](../../../Assets/Art/_yeni/T43/Chest/closed.png) | 960 × 828 | 1 | — | Kapalı sandık. Ortak kasa tuvali 960 × 828 px. |
| [shake/01–04.png](../../../Assets/Art/_yeni/T43/Chest/shake/01.png) | 960 × 828 | 4 | 8 | Beklerken kısa sallanma döngüsü. Alt pivot (480, 756) px. |
| [open/01–08.png](../../../Assets/Art/_yeni/T43/Chest/open/01.png) | 960 × 828 | 8 | 12 | Mandal açılır, kapak yukarı fırlar, arka kenara oturur. |
| [Beams/01–08.png](../../../Assets/Art/_yeni/T43/Chest/Beams/01.png) | 960 × 828 | 8 | 12 | Kapak açılışıyla eşlenen sıcak ışık huzmeleri. Ortak tuval ve iç ağız hizası. |
| [Rewards/title-badge.png](../../../Assets/Art/_yeni/T43/Chest/Rewards/title-badge.png) | 696 × 292 | 1 | — | Boş altın Ödül! rozeti. Metin canlı. |
| [Rewards/collect-normal.png](../../../Assets/Art/_yeni/T43/Chest/Rewards/collect-normal.png) | 684 × 336 | 1 | — | Yeşil Topla düğmesi. Metin canlı. |
| [Rewards/collect-pressed.png](../../../Assets/Art/_yeni/T43/Chest/Rewards/collect-pressed.png) | 684 × 336 | 1 | — | Basılı Topla gövdesi. |
| [Rewards/coin-pile.png](../../../Assets/Art/_yeni/T43/Chest/Rewards/coin-pile.png) | 420 × 302 | 1 | — | Hacimli altın yığını. +100 canlı metin. |
| [Rewards/count-badge.png](../../../Assets/Art/_yeni/T43/Chest/Rewards/count-badge.png) | 192 × 95 | 1 | — | Booster x1 ve +100 altın için boş rozet. |
| [Rewards/Icons/scissors.png](../../../Assets/Art/_yeni/T43/Chest/Rewards/Icons/scissors.png) | 240 × 240 | 1 | — | Mevcut booster ikonu, ödül slotu için. |
| [Rewards/Icons/wand.png](../../../Assets/Art/_yeni/T43/Chest/Rewards/Icons/wand.png) | 240 × 240 | 1 | — | Mevcut booster ikonu, ödül slotu için. |
| [Rewards/Icons/hammer.png](../../../Assets/Art/_yeni/T43/Chest/Rewards/Icons/hammer.png) | 240 × 240 | 1 | — | Mevcut booster ikonu, ödül slotu için. |
| [Rewards/Icons/clock.png](../../../Assets/Art/_yeni/T43/Chest/Rewards/Icons/clock.png) | 240 × 240 | 1 | — | Mevcut booster ikonu, ödül slotu için. |
| [Rewards/coin.png](../../../Assets/Art/_yeni/T43/Chest/Rewards/coin.png) | 120 × 120 | 1 | — | Saçılma için tek altın. |
| [Loot/01–08.png](../../../Assets/Art/_yeni/T43/Chest/Loot/01.png) | 960 × 828 | 8 | 12 | Saçılan altın ve 4 booster. Açılmayla eşle, sonunda ödül slotlarına geç. |
| [Progress/panel.png](../../../Assets/Art/_yeni/T43/Chest/Progress/panel.png) | 960 × 282 | 1 | — | Kazanma panelindeki ilerleme modülü. Üst satır canlı. |
| [Progress/track.png](../../../Assets/Art/_yeni/T43/Chest/Progress/track.png) | 680 × 70 | 1 | — | Altın kenar ve mavi boş kanal. |
| [Progress/fill.png](../../../Assets/Art/_yeni/T43/Chest/Progress/fill.png) | 652 × 44 | 1 | — | Ayrı altın dolgu. Soldan sağa açılır. |
| [Progress/chest-icon.png](../../../Assets/Art/_yeni/T43/Chest/Progress/chest-icon.png) | 217 × 202 | 1 | — | İlerleme çubuğunun sağ ucundaki küçük kapalı sandık. |

### Kazanma · 4 PNG

| Dosya | Boyut (px) | Kare | FPS | Kullanım |
|---|---:|---:|---:|---|
| [panel.png](../../../Assets/Art/_yeni/T43/Victory/panel.png) | 1080 × 2280 | 1 | — | Kazanma ekranı için lacivert-altın boş panel. |
| [next-normal.png](../../../Assets/Art/_yeni/T43/Victory/next-normal.png) | 684 × 336 | 1 | — | Onaylı yeşil Sonraki gövdesi. Metin canlı. |
| [next-pressed.png](../../../Assets/Art/_yeni/T43/Victory/next-pressed.png) | 684 × 336 | 1 | — | Basılı Sonraki gövdesi. |
| [next-arrow.png](../../../Assets/Art/_yeni/T43/Victory/next-arrow.png) | 144 × 144 | 1 | — | Sonraki sağ oku, ayrı katman. |

### Sıradaki tablo ve Kont · 12 PNG

| Dosya | Boyut (px) | Kare | FPS | Kullanım |
|---|---:|---:|---:|---|
| [frame.png](../../../Assets/Art/_yeni/T43/Painting/frame.png) | 942 × 456 | 1 | — | Altın oyma çerçeve. Orta açıklık şeffaf. |
| [aperture-mask.png](../../../Assets/Art/_yeni/T43/Painting/aperture-mask.png) | 942 × 456 | 1 | — | Oyun tablosunu çerçevenin içine kırpan alfa maskesi. |
| [title-badge.png](../../../Assets/Art/_yeni/T43/Painting/title-badge.png) | 564 × 107 | 1 | — | Sıradaki tablo başlığı için boş krem rozet. |
| [cloth/01–06.png](../../../Assets/Art/_yeni/T43/Painting/cloth/01.png) | 942 × 456 | 6 | 12 | Kumaş sağ alt köşeyi açık bırakır. Altı karede yukarı toplanır. |
| [count-seal.png](../../../Assets/Art/_yeni/T43/Painting/count-seal.png) | 192 × 192 | 1 | — | Kont’un K monogramlı sıcak balmumu mührü. |
| [Count/peek.png](../../../Assets/Art/_yeni/T43/Painting/Count/peek.png) | 560 × 654 | 1 | — | Kont’un çerçeve arkasından eğilen gölgesi. |
| [Count/chin-up.png](../../../Assets/Art/_yeni/T43/Painting/Count/chin-up.png) | 560 × 654 | 1 | — | Kont’un çenesi yukarıda, bir parmak kaldırdığı ikinci gölge. |

### Jiggle sürüsü · 5 PNG

| Dosya | Boyut (px) | Kare | FPS | Kullanım |
|---|---:|---:|---:|---|
| [low.png](../../../Assets/Art/_yeni/T43/Flock/low.png) | 1044 × 400 | 1 | — | Beş renkli sürü arka katmanı. 8 Jiggle. |
| [medium.png](../../../Assets/Art/_yeni/T43/Flock/medium.png) | 1044 × 400 | 1 | — | Beş renkli sürü arka katmanı. 16 Jiggle. |
| [high.png](../../../Assets/Art/_yeni/T43/Flock/high.png) | 1044 × 400 | 1 | — | Beş renkli sürü arka katmanı. 26 Jiggle. |
| [jigo-lead.png](../../../Assets/Art/_yeni/T43/Flock/jigo-lead.png) | 407 × 480 | 1 | — | Öndeki tam boy Jigo. Fırça ve dört delikli düğme kalkanı. |
| [plus-badge.png](../../../Assets/Art/_yeni/T43/Flock/plus-badge.png) | 271 × 133 | 1 | — | Yeni katılan sayısı için boş altın rozet. +4 canlı metin. |

## Görsel kaynaklar

- Sandığın iki parçası, iki Kont pozu ve tam boy Jigo yerleşik imagegen ile üretildi. Kont pozları renklendirilip gölgeye dönüştürüldü. [Kont referansı](../task26/kont-pozlar.png) · [Jigo v3](../../../Assets/Art/_yeni/Jigo_v3/)
- Sürüde mevcut beş ayakta Jiggle, ödül slotlarında mevcut dört booster kullanıldı. [Sürü kaynakları](../../../Assets/Art/Menu/Task20/) · [Booster kaynakları](../../../Assets/Art/_yeni/HUD36/Boosters/)

<details>
<summary>Imagegen istemleri</summary>

**Sandık parçaları**

Use case: game asset production, physically separated lid and body of ONE small reward chest on true RGBA alpha. Match the input 1 warm wood/gold shaded toy and input 2 royal gallery blue/cream gold UI; use them solely as style references. Render one rich old money deep cobalt blue lacquered wooden treasure chest with broad rounded antique saturated gold bands, ivory highlight bevels, and one tiny warm copper latch. Symmetrical front three-quarter from slightly above, camera yaw 0 degrees (no left or right side turned toward camera) so horizontal base and lid match width and align. Provide TWO cutout pieces in this single tall canvas, no other contents. Bottom half: only the chest's lower box, clean rounded-square front panel, two gold upright belts, low feet, visible narrow open empty inner well at top with dark indigo felt, precisely front centered. Top half: only the matching gently domed lid from exact same view, separate by a very generous transparent gap, showing its royal blue lacquer, corresponding gold belts aligned with the lower box belts, tiny matching clasp. Lid width exactly equal to box width and centered on same X; when moved down its flat bottom edge mates with front lip of box. Each piece fully visible, comfortable empty outer margins. Friendly premium casual puzzle game paint with subtle material texture, strong warm highlights, NO cast shadows or ambient glow, no hinge connecting the pieces, no coins, no text, no grid, no labels, true transparent background.

**Kont eğilme**

Use case: transparent game cutout used as a silhouette teaser. Subject: exactly the young aristocratic Count in reference, with unmistakable swooped wavy hair and one large front curl, narrow long nose, raised chin, very tall pointed coat collar, elegant angular shoulders. New single waist-up pose: 3/4 LEFT facing profile, shoulders leaning toward the left, bending slightly to peek around an invisible picture frame, eyes looking left, smug small smirk. One hand with softly curved theatrical fingers near his own left-facing cheek, fingers clearly separated from chin outline. No actual frame, no props, no scenery, no hat, no text. Graphic readable near-silhouette, dark indigo coat and deep blue-purple dark features, small restrained periwinkle edge lighting from upper left, no glowing eyes. Full head, hair and both shoulders within the frame. Match reference identity, human, charming arrogant comic villain, studio toy-render look. Single centered image, ample true transparent outer margins, no shadows on the transparent background.

**Kont çene yukarı**

Use case: transparent game cutout used as a silhouette teaser. Subject: exactly the young Count character in input 1, with very distinct high wavy swept pompadour, narrow arched nose, extravagant tall pointed coat collar and slim aristocratic body. Single waist-up shot of a different peek pose: face turned to the LEFT in clean 3/4 profile, chin proudly raised, angled head glancing toward a painting offscreen on viewer left; he holds up one expressive index finger just below shoulder height on the LEFT side of the silhouette, the finger is clearly separated from the head, humorous haughty posture. Readable rich dark navy/indigo almost silhouette, restrained violet light just on the left rim, gentle warm purple bounce in coat folds, elegant toy proportions. Full head and shoulders in frame, hands in silhouette, leave generous actual transparent margins. No other elements, no light streaks or cast shadow, no visible picture frame, no lettering. Actual RGBA transparency.

**Tam boy Jigo**

Use case: transparent full-body game sprite for a victory flock. Generate one genuinely transparent PNG of the exact tiny turquoise puzzle painter Jigo in input 1 (official Jigo_v3 bust identity reference) and input 2 (body proportion and equipment reference). Preserve the same saturated turquoise fabric puzzle-shaped face and body, amber eyes, round teal bare hands, squat turquoise feet, dimpled antique silver-bronze thimble hat, the blue-tipped golden wooden painter's brush in his own right hand on the viewer's left, and his cream four-hole button shield in his left hand on viewer right. Pose: full figure, standing proudly on two feet, smiling, facing nearly forward, brush held aloft just a little, shield at his side, ready to lead a crowd. Material texture and lit bevels match the toy casual puzzle assets. Single isolated character, centered, clean readable silhouette at tiny phone size, all equipment and both feet fully visible with generous empty margins. No other characters, no scenery, no cast shadow, no text, no surrounding glow, actual transparent RGBA.

</details>
