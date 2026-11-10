# Task37 · Final HUD dosya listesi

[Önizleme](onizleme.png) · [HUD37 başlık dosyaları](../../../Assets/Art/_yeni/HUD37/Headers/)

Başlık için B’nin üst dış köşeden sarkan açık mavi flaması seçildi. Pafta B1 ve B2 boylarını eşlenmiş oyun ve Süre doldu ekranlarında gösterir. Boy seçildiğinde aynı satırdaki iki PNG birlikte kullanılır.

## Başlık flaması

| Seçenek | Parça / Süre | Süre doldu |
|---|---|---|
| B1 · Büyük | [game-b-large.png](../../../Assets/Art/_yeni/HUD37/Headers/game-b-large.png) | [continue-b-large.png](../../../Assets/Art/_yeni/HUD37/Headers/continue-b-large.png) |
| B2 · Daha büyük | [game-b-xlarge.png](../../../Assets/Art/_yeni/HUD37/Headers/game-b-xlarge.png) | [continue-b-xlarge.png](../../../Assets/Art/_yeni/HUD37/Headers/continue-b-xlarge.png) |

Oyun PNG’leri 374 × 114 px, Süre doldu PNG’leri 754 × 152 px. HUD35’in yarı gömülü krem kutusu oyun PNG’sinde, aynı malzemedeki geniş kutu modal PNG’sinde bulunur. Oyun başlıklarının x merkezleri 201 ve 972, tuvalin üstü y=150 px. Süre doldu başlığının x merkezi 585, tuvalin üstü y=505 px. Başlık, sayaç, booster adedi ve fiyat canlı metin katmanlarıdır.

Oyun için tek flamanın nominal sarkışı B1’de 73 px, B2’de 89 px. Yarı saydam gölge dahil en alt satır B2’de ekranın y=250 satırında, altın bordür y=477’de. Başlık kumaşı sayaç alanının ve portre penceresinin dışında kalır.

## Oyun HUD’u

| Katman | Kullanılacak dosyalar |
|---|---|
| Üst ve alt şerit | [HUD32/top-strip.png](../../../Assets/Art/_yeni/HUD32/top-strip.png), [HUD32/bottom-strip-a.png](../../../Assets/Art/_yeni/HUD32/bottom-strip-a.png) |
| Parça / Süre gövdeleri | [HUD32/counter-panel.png](../../../Assets/Art/_yeni/HUD32/counter-panel.png), [HUD31/icons/icon-piece.png](../../../Assets/Art/_yeni/HUD31/icons/icon-piece.png) |
| Jigo penceresi | [HUD31/m1/portrait-window.png](../../../Assets/Art/_yeni/HUD31/m1/portrait-window.png), [HUD31/shared/portrait-mask.png](../../../Assets/Art/_yeni/HUD31/shared/portrait-mask.png) |
| Jigo normal | [HUD32/jigo/normal/01–06.png](../../../Assets/Art/_yeni/HUD32/jigo/normal/) |
| Jigo diğer dört hâl | [Tedirgin](../../../Assets/Art/_yeni/Jigo_v3/tedirgin/), [korku](../../../Assets/Art/_yeni/Jigo_v3/korku/), [üzgün](../../../Assets/Art/_yeni/Jigo_v3/uzgun/), [mutlu](../../../Assets/Art/_yeni/Jigo_v3/mutlu/) · Her klasörde 01–06 |
| Tuş yuvası | [HUD31/m1/socket.png](../../../Assets/Art/_yeni/HUD31/m1/socket.png) |
| Yeşil booster gövdesi | [Normal](../../../Assets/Art/_yeni/HUD31/shared/booster-normal.png), [basılı](../../../Assets/Art/_yeni/HUD31/shared/booster-pressed.png), [kilitli](../../../Assets/Art/_yeni/HUD31/shared/booster-locked.png), [tükenmiş](../../../Assets/Art/_yeni/HUD31/shared/booster-zero.png) |
| Booster ikonları | [HUD36 makas](../../../Assets/Art/_yeni/HUD36/Boosters/scissors.png), [değnek](../../../Assets/Art/_yeni/HUD36/Boosters/wand.png), [çekiç](../../../Assets/Art/_yeni/HUD36/Boosters/hammer.png), [sıcak kahverengi saat](../../../Assets/Art/_yeni/HUD36/Boosters/clock-a.png) |
| Rozet ve kilit işareti | [HUD31/shared/badge.png](../../../Assets/Art/_yeni/HUD31/shared/badge.png), [HUD31/shared/locked-icon.png](../../../Assets/Art/_yeni/HUD31/shared/locked-icon.png) |
| Mavi Durdur tuşu | [Normal](../../../Assets/Art/_yeni/HUD31/shared/settings-normal.png), [basılı](../../../Assets/Art/_yeni/HUD31/shared/settings-pressed.png), [kilitli](../../../Assets/Art/_yeni/HUD31/shared/settings-locked.png), [tükenmiş](../../../Assets/Art/_yeni/HUD31/shared/settings-zero.png), [HUD32/pause-icon.png](../../../Assets/Art/_yeni/HUD32/pause-icon.png) |

Booster ikonları mevcut 195 × 195 px tuvalde, (97, 97) px pivotla ortalanır. Bu paftada Task36’nın ana oyun provasıyla aynı sıcak kahverengi saat kullanıldı. Kırmızımsı kahve Task36’nın ayrı renk örneği olarak duruyor. [clock-b.png](../../../Assets/Art/_yeni/HUD36/Boosters/clock-b.png)

## Süre doldu paneli

| Katman | Kullanılacak dosyalar |
|---|---|
| Krem panel | [Devam_v2/panel.png](../../../Assets/Art/_yeni/Devam_v2/panel.png) |
| Kırmızı X | [Devam_v2/close-button.png](../../../Assets/Art/_yeni/Devam_v2/close-button.png), [Devam_v2/icon-close.png](../../../Assets/Art/_yeni/Devam_v2/icon-close.png) |
| Üzgün Jigo | [Jigo_v3/uzgun/](../../../Assets/Art/_yeni/Jigo_v3/uzgun/) · Paftada 02.png |
| Süre, para, yeşil tuş | [Devam_v2/continue-clock.png](../../../Assets/Art/_yeni/Devam_v2/continue-clock.png), [Devam_v2/continue-coin.png](../../../Assets/Art/_yeni/Devam_v2/continue-coin.png), [Devam_v2/continue-primary.png](../../../Assets/Art/_yeni/Devam_v2/continue-primary.png) |
| Yazı tipleri | [Baloo 2 ExtraBold](../../../Assets/_Game/Presentation/Resources/Fonts/Baloo2-ExtraBold.ttf), [Lilita One](../../../Assets/_Game/Presentation/Resources/Fonts/LilitaOne-Regular.ttf) |

## Sürüm kaydı

- **HUD31** tuş gövdeleri, yuva, rozet ve portre parçalarının kaynağı.
- **HUD32** şeritler, sayaç kutusu, Durdur ikonu ve normal Jigo dizisinin kaynağı.
- **HUD33** kontur provası, **HUD34** güncel booster boyasının üretim kaynağı.
- **HUD35** krem kutuların üretim kaynağı. Oyun ve panel için kutular HUD37 başlık PNG’lerinin içinde.
- **HUD36** yerleştirilmiş booster ikonlarının kaynağı. Başlık katmanı HUD37’ye geçti.
- **HUD37** yalnızca B1 ve B2 oyun / Süre doldu başlıklarını içerir.

Engeller paftada mevcut kaynaklarla bileştirildi. [Zincir ve B kilidi](../../../Assets/Art/_yeni/Obstacles_v3/) · [Bomba gövdesi](../../../Assets/Art/_yeni/Obstacles/) · [Kıvılcım](../../../Assets/Art/_yeni/Obstacles_v2/bomb/)
