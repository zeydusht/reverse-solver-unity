# T45 · Durdur, kayıp ve bölüm anlatımı

[Tek pafta](onizleme.png) · [Görev](../CODEX_TASK45.md)

**70 RGBA PNG**. Paftada yedi adet 390 × 844 px önce/sonra çifti ve animasyon yakın planları var. Önce ekranları yerel kod ve kaynak PNG’lerden aynı oyun arka planında yeniden kuruldu. Bomba döngüsü ayrıca PM_59 kaydından kontrol edildi. [Bomba kaydı](../task29/pm59/bomba_karti.gif)

## Önce kaynakları

| Ekran | Kullanılan yerel kaynak |
|---|---|
| Durdur | OpenPause54, ortalanan Card, T23 durdur kartı ve düğmeleri. |
| Kaybetme | Screens.Result, T23 kayıp kartı ve süre simgesi. |
| Bomba, kilit ve zincir | Screens.ObstacleCard. Bomba için mevcut 3 → 2 → 1 döngüsü, diğer ikisi için T23 sahne resimleri. |
| 1. bölüm | P21TutorialView.Story. 86 px krem kutu ve eski avatar. Önce vurgusunda Piece.shader iç renk karışımı kullanıldı. |
| Başlangıç bandı | P21TutorialView.Banner. 40 px lacivert gövde ve krem yazı. |

[GameRoot.cs](../../../Assets/_Game/Presentation/GameRoot.cs) · [Screens.cs](../../../Assets/_Game/Presentation/Ui/Screens.cs) · [P21TutorialView.cs](../../../Assets/_Game/Presentation/Board/P21TutorialView.cs)

## Sonra yerleşimi

- **Durdur.** 1170 × 2532 px karartma tamamen opak. 390 × 844 örneğinde kart 356 × 464 px ve sol üstü (17, 191). Tahta alanı kartın içinde kalır. Seviye N canlı. Yeşil Devam, üç yuvarlak ayar ve küçük kırmızı Çık ayrı. Tuşların açık ve kapalı PNG’leri var.
- **Kaybetme.** Devam teklifi kapandıktan sonra Olmadı! ve canlı neden satırı kullanılır. Paftadaki örnek Süre bitti. Bomba patladı aynı yuvaya gelir. Üzgün Jigo’nun altı karesi onaylı kaynağıyla birebir. Büyük Tekrar Dene ve küçük Menü ayrı.
- **Başlıklar.** 3B Türkçe PNG’ler canlı tipografi örneği. Baloo 2 ExtraBold, krem-sarı yüz, mavi kontur, altın taban ve hafif yukarı kavis. Yıldızlı arka ışık ayrı. Gövde krem, çerçeve mavi lake.

## Engel kartları

Üç kart aynı 1044 × 1548 px paneli kullanır. Büyük engel görseli ve 840 × 268 px mini demo ayrı. Sayaç rakamları demo karelerinin sabit örnekleridir. Kart açıklaması ve Anladım! canlıdır.

| Engel | Mini demo |
|---|---|
| Bomba | 8 kare, 8 FPS. 01–02 = 3, 03–04 = 2, 05–06 = 1. 07’de parça sağa gider ve fitil söner. 08’de tahta boş, küçük duman ve tik. |
| Kilit | 8 kare, 8 FPS. 01–02 = 2, 03–04 = 1, 05 = 0. 06–08’de kilit açıkken aşağı düşer. Parça yerinde. |
| Zincir | 8 kare, 12 FPS. İki parça ve bağlantı birlikte sağa gider. Başlangıç ve bitiş kareleri kısa süre tutulabilir. |

| Kart | TR canlı açıklama | EN canlı açıklama |
|---|---|---|
| Bomba | Bombalı parçayı sayı bitmeden çıkar. Her hamlede sayı 1 azalır. 0 olursa patlar! | Clear the bomb piece before its number runs out. It drops by 1 each move. At 0 it blows! |
| Kilit | Kilitli parça N hamle boyunca hareket etmez. Başka parçaları oyna. Sayı 0 olunca kilit açılır. | A locked piece can’t move for N moves. Play other pieces. At 0 the lock opens. |
| Zincir | Zincirli iki parça birlikte hareket eder. | Two chained pieces move together. |

Kilit kartında N gerçek değerdir. Paftada 2 kullanıldı. [PM_59](../../PM_MESAJ_59.md) · [PM_60](../../PM_MESAJ_60.md) · [Texts.cs](../../../Assets/_Game/Presentation/Ui/Texts.cs)

## 1. bölüm ve hedef bandı

- Sohbet kutusu 1086 × 198 px. Makette 362 × 66 px, ekran yüksekliğinin %7,8’i. Alt kenarı, güvenli alan hesaba katılarak ButtonsTop’tan 10 px yukarıda. Metin canlı ve iki satıra bölünebilir. Normal ve konuşan avatar ayrı. Konuşma için onaylı mutlu kareden ağız alındı. Normal kaşlar ve gözlerin yerleşimi korunuyor.
- Altın ışık 6 kare, 8 FPS. 288 × 288 px tuvalde ortak merkez (144, 144). Yuvarlatılmış 202 × 202 px parça alanı tamamen saydam. Farklı parça biçimlerinde aynı nabzı gerçek parçanın dış siluetine uygula. Resim alanını doldurma.
- Hedef bandı 1050 × 144 px. Makette 350 × 48 px. Parça ikonu ve canlı yazı ayrı. Örnek hareket merkezinin y=−26 px’den güvenli üst + 37 px’e inişi, bekleme ve aynı yönde geri çıkış. Toplam 1,50 sn için giriş 0,20 sn, duruş 1,05 sn, çıkış 0,25 sn.

## Dosya listesi

— tek katman. Kare dizisi bağlantısı ilk PNG’yi açar.

### Başlıklar · 6 PNG

| Dosya | Boyut (px) | Kare | FPS | Kullanım |
|---|---:|---:|---:|---|
| [pause-sample.png](../../../Assets/Art/_yeni/T45/Titles/pause-sample.png) | 960 × 330 | 1 | — | Canlı başlık için T44 tarzı 3B örnek. |
| [loss-sample.png](../../../Assets/Art/_yeni/T45/Titles/loss-sample.png) | 960 × 330 | 1 | — | Canlı başlık için T44 tarzı 3B örnek. |
| [bomb-sample.png](../../../Assets/Art/_yeni/T45/Titles/bomb-sample.png) | 960 × 330 | 1 | — | Canlı başlık için T44 tarzı 3B örnek. |
| [lock-sample.png](../../../Assets/Art/_yeni/T45/Titles/lock-sample.png) | 960 × 330 | 1 | — | Canlı başlık için T44 tarzı 3B örnek. |
| [chain-sample.png](../../../Assets/Art/_yeni/T45/Titles/chain-sample.png) | 960 × 330 | 1 | — | Canlı başlık için T44 tarzı 3B örnek. |
| [light.png](../../../Assets/Art/_yeni/T45/Titles/light.png) | 1032 × 432 | 1 | — | Bağımsız yumuşak ışık ve yıldızlar. Başlık yazısı ayrı. |

### Durdur · 9 PNG

| Dosya | Boyut (px) | Kare | FPS | Kullanım |
|---|---:|---:|---:|---|
| [dim-full.png](../../../Assets/Art/_yeni/T45/Pause/dim-full.png) | 1170 × 2532 | 1 | — | Opak tam ekran lacivert karartma. Tahta görünmez. |
| [panel.png](../../../Assets/Art/_yeni/T45/Pause/panel.png) | 1068 × 1392 | 1 | — | Ortadaki büyük krem kart ve mavi lake çerçeve. |
| [level-badge.png](../../../Assets/Art/_yeni/T45/Pause/level-badge.png) | 362 × 99 | 1 | — | Seviye N için boş küçük krem rozet. |
| [Toggle/sound-on.png](../../../Assets/Art/_yeni/T45/Pause/Toggle/sound-on.png) | 232 × 232 | 1 | — | Mavi çerçeve ile kullanılan yeşil yuvarlak tuş. Kapalı durumda mercan çizgi. |
| [Toggle/sound-off.png](../../../Assets/Art/_yeni/T45/Pause/Toggle/sound-off.png) | 232 × 232 | 1 | — | Mavi çerçeve ile kullanılan yeşil yuvarlak tuş. Kapalı durumda mercan çizgi. |
| [Toggle/music-on.png](../../../Assets/Art/_yeni/T45/Pause/Toggle/music-on.png) | 232 × 232 | 1 | — | Mavi çerçeve ile kullanılan yeşil yuvarlak tuş. Kapalı durumda mercan çizgi. |
| [Toggle/music-off.png](../../../Assets/Art/_yeni/T45/Pause/Toggle/music-off.png) | 232 × 232 | 1 | — | Mavi çerçeve ile kullanılan yeşil yuvarlak tuş. Kapalı durumda mercan çizgi. |
| [Toggle/vibration-on.png](../../../Assets/Art/_yeni/T45/Pause/Toggle/vibration-on.png) | 232 × 232 | 1 | — | Mavi çerçeve ile kullanılan yeşil yuvarlak tuş. Kapalı durumda mercan çizgi. |
| [Toggle/vibration-off.png](../../../Assets/Art/_yeni/T45/Pause/Toggle/vibration-off.png) | 232 × 232 | 1 | — | Mavi çerçeve ile kullanılan yeşil yuvarlak tuş. Kapalı durumda mercan çizgi. |

### Düğmeler · 6 PNG

| Dosya | Boyut (px) | Kare | FPS | Kullanım |
|---|---:|---:|---:|---|
| [primary-normal.png](../../../Assets/Art/_yeni/T45/Buttons/primary-normal.png) | 684 × 336 | 1 | — | Devam, Tekrar Dene ve Anladım! için onaylı yeşil gövde. Canlı metin. |
| [exit-normal.png](../../../Assets/Art/_yeni/T45/Buttons/exit-normal.png) | 684 × 336 | 1 | — | Çık düğmesi. Kırmızı lake yüz, altın kenarlar. |
| [secondary-normal.png](../../../Assets/Art/_yeni/T45/Buttons/secondary-normal.png) | 392 × 150 | 1 | — | Küçük mavi Menü düğmesi. Canlı metin. |
| [primary-pressed.png](../../../Assets/Art/_yeni/T45/Buttons/primary-pressed.png) | 684 × 336 | 1 | — | Devam, Tekrar Dene ve Anladım! için onaylı yeşil gövde. Canlı metin. |
| [exit-pressed.png](../../../Assets/Art/_yeni/T45/Buttons/exit-pressed.png) | 684 × 336 | 1 | — | Çık düğmesi. Kırmızı lake yüz, altın kenarlar. |
| [secondary-pressed.png](../../../Assets/Art/_yeni/T45/Buttons/secondary-pressed.png) | 392 × 150 | 1 | — | Küçük mavi Menü düğmesi. Canlı metin. |

### Kaybetme · 8 PNG

| Dosya | Boyut (px) | Kare | FPS | Kullanım |
|---|---:|---:|---:|---|
| [panel.png](../../../Assets/Art/_yeni/T45/Loss/panel.png) | 1038 × 1506 | 1 | — | Krem kart. Olmadı! ve kayıp nedeni ayrı. |
| [portrait-light.png](../../../Assets/Art/_yeni/T45/Loss/portrait-light.png) | 686 × 659 | 1 | — | Üzgün Jigo arkasında sıcak kremle uyumlu mavi ışık. |
| [Jigo/01–06.png](../../../Assets/Art/_yeni/T45/Loss/Jigo/01.png) | 256 × 256 | 6 | 9 | Onaylı üzgün Jigo_v3 kareleri, bayt düzeyinde aynı. |

### Engel kartları · 29 PNG

| Dosya | Boyut (px) | Kare | FPS | Kullanım |
|---|---:|---:|---:|---|
| [panel.png](../../../Assets/Art/_yeni/T45/Intro/panel.png) | 1044 × 1548 | 1 | — | Bomba, kilit ve zincir için ortak boş tanıtım kartı. |
| [Demo/frame.png](../../../Assets/Art/_yeni/T45/Intro/Demo/frame.png) | 840 × 268 | 1 | — | Ortak mini tahta. Hareket ve sayaç PNG’leri üstte. |
| [Icons/bomb.png](../../../Assets/Art/_yeni/T45/Intro/Icons/bomb.png) | 366 × 411 | 1 | — | Onaylı fitilli bomba. Büyük tanıtım resmi, örnek 3. |
| [Icons/lock.png](../../../Assets/Art/_yeni/T45/Intro/Icons/lock.png) | 384 × 384 | 1 | — | Onaylı B kilidi ve Kont’un kırmızı mührü. Örnek 2. |
| [Icons/chain.png](../../../Assets/Art/_yeni/T45/Intro/Icons/chain.png) | 692 × 395 | 1 | — | İki örnek parça ve onaylı altın zincir halkaları. |
| [Demo/Bomb/01–08.png](../../../Assets/Art/_yeni/T45/Intro/Demo/Bomb/01.png) | 840 × 268 | 8 | 8 | 3, 3, 2, 2, 1, 1, çıkışta fitil söner, tahta boş. Patlama yok. |
| [Demo/Lock/01–08.png](../../../Assets/Art/_yeni/T45/Intro/Demo/Lock/01.png) | 840 × 268 | 8 | 8 | 2, 2, 1, 1, 0, açılış ve düşme. Parça yerinde kalır. |
| [Demo/Chain/01–08.png](../../../Assets/Art/_yeni/T45/Intro/Demo/Chain/01.png) | 840 × 268 | 8 | 12 | İki parça, bağlantıyı koruyarak aynı miktarda sağa kayar. |

### 1. bölüm · 9 PNG

| Dosya | Boyut (px) | Kare | FPS | Kullanım |
|---|---:|---:|---:|---|
| [Chat/panel.png](../../../Assets/Art/_yeni/T45/Tutorial/Chat/panel.png) | 1086 × 198 | 1 | — | İnce krem sohbet kutusu. 390 × 844 maketinde 362 × 66 px. |
| [Chat/jigo-normal.png](../../../Assets/Art/_yeni/T45/Tutorial/Chat/jigo-normal.png) | 176 × 176 | 1 | — | Onaylı normal Jigo, küçük madalyon. |
| [Chat/jigo-talk.png](../../../Assets/Art/_yeni/T45/Tutorial/Chat/jigo-talk.png) | 176 × 176 | 1 | — | Normal gözler ve kaşlar korunur. Onaylı mutlu kareden alınan ağız konuşma pozu. |
| [Highlight/pulse/01–06.png](../../../Assets/Art/_yeni/T45/Tutorial/Highlight/pulse/01.png) | 288 × 288 | 6 | 8 | Altın kenar ışığı. 202 × 202 px merkez saydam. Altı kare yumuşak nabız. |

### Hedef bandı · 3 PNG

| Dosya | Boyut (px) | Kare | FPS | Kullanım |
|---|---:|---:|---:|---|
| [banner.png](../../../Assets/Art/_yeni/T45/Goal/banner.png) | 1050 × 144 | 1 | — | İnce mavi-altın başlangıç bandı. Üstten girip çıkar. |
| [piece-icon.png](../../../Assets/Art/_yeni/T45/Goal/piece-icon.png) | 120 × 120 | 1 | — | Onaylı HUD parça ikonu. |
| [text-sample.png](../../../Assets/Art/_yeni/T45/Goal/text-sample.png) | 852 × 109 | 1 | — | 3B küçük canlı yazı için Türkçe örnek. |
