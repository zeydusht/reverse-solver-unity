# T44 · Kazanma ekranı ve sandık

[Önizleme](onizleme.png) · [Görev](../CODEX_TASK44.md)

**134 RGBA PNG**. Paftada kazanma ekranı, üç sandık dokunuşu ve iki ödül uçuşu 390 × 844 px gösteriliyor. Yakın planlarda beş ödül, yürüyüş kareleri ve 12 onaylı Jigo portresi var.

## Teslim öncesi görsel kontrol

| Zeyd’in notu | Paftadaki kontrol |
|---|---|
| Normal ve mutlu Jigo’nun kaşları | Alt sıradaki 12 PNG onaylı Jigo_v3 kaynaklarıyla bayt düzeyinde eşleşti. Büyük Jigo mutlu 03. Yüzü yeniden çizilmedi. |
| Kurdele ve afiş | Harika! ve Sandık! doğrudan mavi konturlu 3B yazı ve ayrı ışık üstünde. |
| Sıralı yürüyüş | Jigo en sağda önde. Aynı zeminde arkasında dört Jiggle var. Yakın planda altı adım karesi. |
| Ödül boyutu ve tek tek görünme | Üçüncü dokunuş maketinde tek altın ödülü ortada. Beş ödül aynı 1:3 ölçekte ayrı gösteriliyor. |
| Sandık dokunuşları | İlk sahne hafif sallanma ve el. İkinci sahne daha sert sallanma ve kapak sızıntısı. Üçüncü sahne onaylı açık kasa ve tek ödül. |

## Onaylı kaynaklar

- T43 güçlü parça ve engel efektleri kendi klasörlerinde. Sandığın kapalı PNG’si, sekiz açılış karesi, açılış huzmeleri ve Sonraki katmanları burada kaynaklarıyla birebir. [T43 efektleri](../../../Assets/Art/_yeni/T43/Juice/) · [T43 sandığı](../../../Assets/Art/_yeni/T43/Chest/)
- Oyun Durdur için mevcut ortadaki kartı kullanır. Bu görsel pakette dikey Durdur sütunu, Sıradaki tablo ve Kont kartı veya kalabalık sürü katmanı bulunmaz. [Durdur kartı](../../../Assets/_Game/Presentation/GameRoot.cs) · [T23 panel](../../../Assets/_Game/Presentation/Resources/Art/t23-pause-panel.png)

## Kazanma yerleşimi

- Harika! ve Sandık! görselleri canlı başlık için örnek. Baloo 2 ExtraBold, kremden sarıya dolgu, 1:3 ölçekte yaklaşık 5 px koyu mavi kontur ve 8–9 px altın kalınlık. Ortadaki harfler uçlara göre yaklaşık 8 px yukarıda. Titles/light arka katmanı ayrıca çizilir.
- İlerleme örneğinde üç yuva var. Biri yıldızlı, ikisi boş. Sonunda sandık ikonu, altında 2 bölüm sonra sandık! canlı satırı. Boş ve dolu yuvalar tekrar kullanılabilir.
- Tek sıra 376 × 428 px ortak tuvalde, yaklaşık (188, 383) px ayak hizasına göre yerleşir. Altı kareyi 10 FPS’de oynat ve bütün sırayı sağa kaydır. Liderin yürüyüş gövdesi, mutlu Jigo büstünün tamamına uygulanan perspektif ile ayrı ayaklardan oluşur. Büyük kazanma portresi doğrudan onaylı PNG. Takipçiler beş renk.
- Sonraki Seviye ve 4 Jiggle kurtarıldı! canlı. Düğme gövdesi T43 ile aynı.

## Sandık ve ödül katmanları

| Adım | Yerleşim ve süre |
|---|---|
| 1. dokunuş | Shake1 dizisi. 4 kare, 14 FPS. T41 elinin işaret ucu (160, 30) px. Sandığa dokun! yazısına hafif alfa döngüsü uygula. |
| 2. dokunuş | Shake2 ve Leak aynı numarada ve aynı konumda. 4 kare, 14 FPS. Kasa alt pivotu (480, 756) px. |
| 3. dokunuş | Open ve OpenLight, 8 kare, 12 FPS. Açılma huzmesini 03’ten itibaren üstüne çiz. |
| Tek ödül | Bir öğeyi ağızdan ekran merkezine taşı. Pop dizisi 4 kare ve 12 FPS. Son pozda ayrı ışık ve canlı +100 / x1, yaklaşık 0,55 sn bekleme örneği. |
| Hedefe uçuş | İz soldan sağa çizildi. Uçuş doğrultusuna döndür. Altın üst sayaca, booster kendi alt tuşuna. Hedefte Arrival oynar. Paftada çekiç örneklendi. |

| Merkezdeki ödül | Görünür uzun kenar (pt) |
|---|---:|
| Altın | 99,0 |
| Makas | 102,7 |
| Değnek | 103,3 |
| Çekiç | 102,7 |
| Saat | 103,3 |

Görünür boyut 350 × 350 px kaynakta alfa ≥ 100 sınırından ölçüldü ve 1:3 ekrana çevrildi.

## Dosya listesi

— tek katmanı gösterir. Kare dizisinin bağlantısı ilk kareyi açar.

### Başlıklar · 3 PNG

| Dosya | Boyut (px) | Kare | FPS | Katman ve kullanım |
|---|---:|---:|---:|---|
| [light.png](../../../Assets/Art/_yeni/T44/Titles/light.png) | 1032 × 432 | 1 | — | Başlık arkasında ayrı yumuşak ışık ve beş yıldız. |
| [harika-sample.png](../../../Assets/Art/_yeni/T44/Titles/harika-sample.png) | 960 × 330 | 1 | — | Canlı başlık için 3B görünüm örneği. Kavis, mavi kontur ve altın taban. |
| [sandik-sample.png](../../../Assets/Art/_yeni/T44/Titles/sandik-sample.png) | 960 × 330 | 1 | — | Canlı başlık için 3B görünüm örneği. Kavis, mavi kontur ve altın taban. |

### Kazanma · 47 PNG

| Dosya | Boyut (px) | Kare | FPS | Katman ve kullanım |
|---|---:|---:|---:|---|
| [panel.png](../../../Assets/Art/_yeni/T44/Victory/panel.png) | 1080 × 2310 | 1 | — | Kazanma ve ödül maketleri için metinsiz lacivert-altın gövde. |
| [portrait-glow.png](../../../Assets/Art/_yeni/T44/Victory/portrait-glow.png) | 682 × 644 | 1 | — | Büyük Jigo’nun arkasındaki hafif ışık ve yıldızlar. |
| [Progress/panel.png](../../../Assets/Art/_yeni/T44/Victory/Progress/panel.png) | 971 × 328 | 1 | — | Ayrı ilerleme yuvalarını ve canlı satırı taşıyan boş yüzey. |
| [Progress/slot-empty.png](../../../Assets/Art/_yeni/T44/Victory/Progress/slot-empty.png) | 162 × 158 | 1 | — | Kazanılacak bölüm için boş mavi yuva. |
| [Progress/slot-complete.png](../../../Assets/Art/_yeni/T44/Victory/Progress/slot-complete.png) | 162 × 158 | 1 | — | Kazanılan yuva. Altın yıldız ve krem tik. |
| [Progress/dots.png](../../../Assets/Art/_yeni/T44/Victory/Progress/dots.png) | 147 × 76 | 1 | — | Yuva dizisinin sonu ile sandık arasındaki üç nokta. |
| [Progress/chest.png](../../../Assets/Art/_yeni/T44/Victory/Progress/chest.png) | 217 × 202 | 1 | — | Onaylı mavi-altın sandığın ilerleme ikonu. |
| [Procession/floor.png](../../../Assets/Art/_yeni/T44/Victory/Procession/floor.png) | 981 × 157 | 1 | — | Yürüyüş dizisinin altına ayrı yumuşak zemin. |
| [Next/normal.png](../../../Assets/Art/_yeni/T44/Victory/Next/normal.png) | 684 × 336 | 1 | — | Onaylı Sonraki gövdesi veya oku. Sonraki Seviye metni canlı. |
| [Next/pressed.png](../../../Assets/Art/_yeni/T44/Victory/Next/pressed.png) | 684 × 336 | 1 | — | Onaylı Sonraki gövdesi veya oku. Sonraki Seviye metni canlı. |
| [Next/arrow.png](../../../Assets/Art/_yeni/T44/Victory/Next/arrow.png) | 144 × 144 | 1 | — | Onaylı Sonraki gövdesi veya oku. Sonraki Seviye metni canlı. |
| [Procession/Walk/jigo/01–06.png](../../../Assets/Art/_yeni/T44/Victory/Procession/Walk/jigo/01.png) | 376 × 428 | 6 | 10 | Öndeki Jigo. Onaylı mutlu büstün tamamı, küçük yürüyüş rig’i üstünde. |
| [Procession/Walk/turquoise/01–06.png](../../../Assets/Art/_yeni/T44/Victory/Procession/Walk/turquoise/01.png) | 376 × 428 | 6 | 10 | Üç çeyrek sağa bakan Jiggle. Altı kare yürüyüş. |
| [Procession/Walk/green/01–06.png](../../../Assets/Art/_yeni/T44/Victory/Procession/Walk/green/01.png) | 376 × 428 | 6 | 10 | Sağa yürüyüşün renk varyantı, zamanlama ortak. |
| [Procession/Walk/pink/01–06.png](../../../Assets/Art/_yeni/T44/Victory/Procession/Walk/pink/01.png) | 376 × 428 | 6 | 10 | Sağa yürüyüşün renk varyantı, zamanlama ortak. |
| [Procession/Walk/purple/01–06.png](../../../Assets/Art/_yeni/T44/Victory/Procession/Walk/purple/01.png) | 376 × 428 | 6 | 10 | Sağa yürüyüşün renk varyantı, zamanlama ortak. |
| [Procession/Walk/red/01–06.png](../../../Assets/Art/_yeni/T44/Victory/Procession/Walk/red/01.png) | 376 × 428 | 6 | 10 | Sağa yürüyüşün renk varyantı, zamanlama ortak. |

### Onaylı portreler · 12 PNG

| Dosya | Boyut (px) | Kare | FPS | Katman ve kullanım |
|---|---:|---:|---:|---|
| [mutlu/01–06.png](../../../Assets/Art/_yeni/T44/Jigo/mutlu/01.png) | 256 × 256 | 6 | 10 | Onaylı Jigo_v3 kaş ve yüz çizimleri. Birebir kaynak PNG. |
| [normal/01–06.png](../../../Assets/Art/_yeni/T44/Jigo/normal/01.png) | 256 × 256 | 6 | 10 | Onaylı Jigo_v3 kaş ve yüz çizimleri. Birebir kaynak PNG. |

### Sandık ve ödül · 72 PNG

| Dosya | Boyut (px) | Kare | FPS | Katman ve kullanım |
|---|---:|---:|---:|---|
| [Chest/closed.png](../../../Assets/Art/_yeni/T44/Reward/Chest/closed.png) | 960 × 828 | 1 | — | Onaylı kapalı sandık, aynı 960 × 828 tuval. |
| [Chest/Open/01–08.png](../../../Assets/Art/_yeni/T44/Reward/Chest/Open/01.png) | 960 × 828 | 8 | 12 | T43 onaylı sandık açılma kareleri. Üçüncü dokunuşta. |
| [Chest/OpenLight/01–08.png](../../../Assets/Art/_yeni/T44/Reward/Chest/OpenLight/01.png) | 960 × 828 | 8 | 12 | Onaylı açılma huzmesi. Open ile aynı numara. |
| [Touch/Hand/01–05.png](../../../Assets/Art/_yeni/T44/Reward/Touch/Hand/01.png) | 352 × 412 | 5 | 10 | T41 yönlendiren el. İşaret ucu (160, 30) px. |
| [Chest/Shake1/01–04.png](../../../Assets/Art/_yeni/T44/Reward/Chest/Shake1/01.png) | 960 × 828 | 4 | 14 | İlk dokunuş. Hafif sallanma, ışık sızıntısı yok. |
| [Chest/Shake2/01–04.png](../../../Assets/Art/_yeni/T44/Reward/Chest/Shake2/01.png) | 960 × 828 | 4 | 14 | İkinci dokunuş. Daha sert sallanma, hafif aralanan kapak. |
| [Chest/Leak/01–04.png](../../../Assets/Art/_yeni/T44/Reward/Chest/Leak/01.png) | 960 × 828 | 4 | 14 | İkinci dokunuş. Shake2 ile birebir hizalı altın sızıntı katmanı. |
| [Reveal/Glow/01–04.png](../../../Assets/Art/_yeni/T44/Reward/Reveal/Glow/01.png) | 534 × 534 | 4 | 8 | Her tek ödülün arkasında dönen sıcak ışık. |
| [Reveal/Items/gold.png](../../../Assets/Art/_yeni/T44/Reward/Reveal/Items/gold.png) | 350 × 350 | 1 | — | Tek ödül. Ekranda yaklaşık 104 pt görünür boy. |
| [Reveal/Pop/gold/01–04.png](../../../Assets/Art/_yeni/T44/Reward/Reveal/Pop/gold/01.png) | 410 × 410 | 4 | 12 | Fırlama, büyüme, hafif sekme ve duruş. Konum kodla. |
| [Reveal/Items/scissors.png](../../../Assets/Art/_yeni/T44/Reward/Reveal/Items/scissors.png) | 350 × 350 | 1 | — | Tek ödül. Ekranda yaklaşık 104 pt görünür boy. |
| [Reveal/Pop/scissors/01–04.png](../../../Assets/Art/_yeni/T44/Reward/Reveal/Pop/scissors/01.png) | 410 × 410 | 4 | 12 | Fırlama, büyüme, hafif sekme ve duruş. Konum kodla. |
| [Reveal/Items/wand.png](../../../Assets/Art/_yeni/T44/Reward/Reveal/Items/wand.png) | 350 × 350 | 1 | — | Tek ödül. Ekranda yaklaşık 104 pt görünür boy. |
| [Reveal/Pop/wand/01–04.png](../../../Assets/Art/_yeni/T44/Reward/Reveal/Pop/wand/01.png) | 410 × 410 | 4 | 12 | Fırlama, büyüme, hafif sekme ve duruş. Konum kodla. |
| [Reveal/Items/hammer.png](../../../Assets/Art/_yeni/T44/Reward/Reveal/Items/hammer.png) | 350 × 350 | 1 | — | Tek ödül. Ekranda yaklaşık 104 pt görünür boy. |
| [Reveal/Pop/hammer/01–04.png](../../../Assets/Art/_yeni/T44/Reward/Reveal/Pop/hammer/01.png) | 410 × 410 | 4 | 12 | Fırlama, büyüme, hafif sekme ve duruş. Konum kodla. |
| [Reveal/Items/clock.png](../../../Assets/Art/_yeni/T44/Reward/Reveal/Items/clock.png) | 350 × 350 | 1 | — | Tek ödül. Ekranda yaklaşık 104 pt görünür boy. |
| [Reveal/Pop/clock/01–04.png](../../../Assets/Art/_yeni/T44/Reward/Reveal/Pop/clock/01.png) | 410 × 410 | 4 | 12 | Fırlama, büyüme, hafif sekme ve duruş. Konum kodla. |
| [Flight/trail.png](../../../Assets/Art/_yeni/T44/Reward/Flight/trail.png) | 640 × 200 | 1 | — | Uçuş ekseni soldan sağa. Baş tarafını hedefe çevir. |
| [Flight/arrival/01–04.png](../../../Assets/Art/_yeni/T44/Reward/Flight/arrival/01.png) | 284 × 284 | 4 | 12 | Hedef sayaç veya booster tuşu üzerinde kısa ışık. |
| [Flight/coin-counter.png](../../../Assets/Art/_yeni/T44/Reward/Flight/coin-counter.png) | 348 × 109 | 1 | — | Üst altın sayacı. İkon ve sayı ayrı. |
| [Flight/coin.png](../../../Assets/Art/_yeni/T44/Reward/Flight/coin.png) | 120 × 120 | 1 | — | Altın sayacı için mevcut altın ikonu. |
| [Collect/normal.png](../../../Assets/Art/_yeni/T44/Reward/Collect/normal.png) | 684 × 336 | 1 | — | Yeşil Topla gövdesi. Metin canlı. |
| [Collect/pressed.png](../../../Assets/Art/_yeni/T44/Reward/Collect/pressed.png) | 684 × 336 | 1 | — | Yeşil Topla gövdesi. Metin canlı. |

## Yürüyüş kaynağı

Sağa bakan takipçi Jiggle’ın tabanı yerleşik imagegen ile üretildi. Ayaklar katmanlandı ve diğer renkler aynı döngüden üretildi. [Jiggle referansı](../../../Assets/Art/Characters/Jiggles-reference-v02.png) · [Jigo v3](../../../Assets/Art/_yeni/Jigo_v3/)

<details>
<summary>Imagegen istemi</summary>

Use case: a genuinely transparent three-quarter-side game sprite for a rightward walking cycle. Generate exactly ONE turquoise Jiggle from input 1 and 2, the small plush jigsaw puzzle person from Reverse Solver. This is a follower, NO hat, NO brush, NO shield. Preserve the recognizable square jigsaw body with one circular interlock tab and inset, same puffy teal felt, stitching, rounded hands and feet, and friendly big brown eyes. Pose: turned clearly toward SCREEN RIGHT in a 50-degree three-quarter side view with the right side as the front-facing direction. Nose/eyes and happy smile unmistakably look to SCREEN RIGHT. Full body centered, short arms relaxed at side in an easy walk, clearly see both short legs and both rounded feet, feet spaced left-right on ground with both legs visible, no overlapping feet. Tiny jaunty step, near foot at screen left, far foot at screen right with small gap and no cast shadow. Arms stay attached and do not overlap the legs. Both eyes can remain visible, far eye a little narrower. Cozy premium 3D casual puzzle toy art, exact plush material and upper-left light as reference, no outlines, no dust, no extra characters, no ground, no text, genuinely transparent RGBA. Generous transparent margins all around.

</details>
