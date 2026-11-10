# Panorama — eşleşen komşu bölüm revizyonu

Yalnızca ayrı inceleme taslağıdır. Oyun dosyalarına uygulanmadı.

## Teslimler

- `panorama-matched-step.mp4`: tek komşu bölüm, 3 saniyelik hareket, 60 FPS, 780×1688. Son tam kare dâhil dosya süresi 3,016667 saniye.
- `review-three-cycles.mp4`: aynı hareketin kesintisiz üç tekrarı, 9,016667 saniye. Her tekrar bir bölüm ilerler; fazladan tablo atlanmaz.
- `start-end-comparison.png`: ham başlangıç, ham bitiş ve siyah piksel farkı görüntüsü.
- `review-frames.png`: başlangıç, orta sütun/duvar ve komşu tablo kadrajı.
- `before-after-start.png`: önceki beğenilen taslak ile bu revizyonun başlangıcını yan yana gösterir; yerel değişiklikleri değerlendirmek içindir.
- `matched-scene.png`, `matched-bay.png`: tüm zamanlarda kullanılan sabit sahne ve tek mimari modül.
- `index.html`: HTTP üzerinden açılabilen bir kez oynat / üç tekrar incele sayfası. Oyun entegrasyonu değildir.

## Mevcut çizim nasıl korundu?

Asıl kaynak `docs/art/drafts/corridor-victory-v2-panorama-20261030/panorama-clean.png` dosyasıdır. Önceki `panorama-single-step.mp4` ve onu üreten kodlar/teslimler **backup/** klasörüne kopyalandı. Eski taslak klasörü de değiştirilmedi. Reddedilen önden görünümlü one-bay çizimi kullanılmadı.

İlk tablonun çevresindeki **496 piksellik** bölüm kopyalandı. Tablo ve tam altın çerçeve, sütun/başlık, banner, mumluk ve vazo orijinal piksellerinden alındı; tablo veya mimari öğe yeniden çizilmedi. Tam tablo/çerçeve bölgesinin piksel eşitliği ayrıca kontrol edilir. Eğik çerçeve ve mevcut güneş/gölge desenleri korunur.

Ham yatay kopya, sütunlar ve eğimli zemin arasında kesikler oluşturuyordu. Sabit sahne şu yerel işlemlerle hazırlandı:

1. Duvar/süpürgelik tekrar sınırı, **orijinal sütunun arkasına** alındı. Sütun/vazo aynı orijinal piksel maskesiyle tek parça yerleştirildi. Arkadaki dar düz duvar/taş uzantısında mevcut satır renkleri örneklenerek süreklilik sağlandı. Bu bölge özgün tuvaldeki kırpılmış yan sütunun yerini tamamlar.
2. Zemin birleşimi için yalnızca **192×221 kaynak piksel** büyüklüğündeki küçük kırpım yerleşik image_gen ile düzenlendi. Altın halı bordürü, iki mevcut ucun yüksekliğine bağlanan sığ yerel bir geçişle tamamlandı; ana halı alanı ve duvarın tamamı yeniden üretilmedi. Üretilen kırpımın dışında hiçbir üretilmiş mimari kullanılmadı.
3. Görüntünün en altında kesilmiş altın bordür parçaları tekrarlandığında kama oluşturuyordu. En alt **36 piksellik** bölüm aynı sahnenin mevcut kırmızı dokusundan, boyut değiştirmeden kopyalanarak uzatıldı.

Bu yüzden revizyon, kaynak panoramanın bütünüyle değişmeden başa sarılması değildir; komşu bölümün eşleşmesi için izin verilen yerel birleştirme çalışmasıdır. Düzenlenmiş zemin birleşimi ve yan bölüm tamamlama, `before-after-start.png` üzerinden görülebilir. Kaynak nesne pikselleri esnetilmez; yerel üretim kırpımı yerleştirme boyutuna örneklenir. Sabit mat kenarları yalnızca sahne hazırlanırken kullanılır.

## Hareket ve tekrar

Son beğenilen videodaki yön, 496 kaynak piksel mesafe, 390:844 kadraj oranı, sabit ölçek, 3 saniye ve quintic yumuşatma korundu. Hareket sırasında yalnızca aynı sabit sahne üzerindeki kırpım penceresi yer değiştirir. Ara kareler üretilmez; zamana bağlı alpha/crossfade, morph, zoom veya perspektif dönüşümü yoktur.

Komşu bölümler aynı sabit malzemenin piksel kopyalarıdır. Kamera bir bölüm ilerlediğinde periyodik koordinat aynı malzemeyi örnekler; bu, Canvas'ın farklı büyük x koordinatlarında oluşturabildiği 1 seviyelik yuvarlama farkını da önler. Son kare ayrı bir resimle değiştirilmez ve bitişe yaklaşırken hiçbir görüntü erimesi uygulanmaz. Ortada aradaki tek sütun geçer.

Üç tekrarlı videoda 180 karelik aynı H.264 örnek dizisi üç kez kullanılır; her başlangıç aynı IDR kareyle başlar, son tam kare ayrıca eklenir. Böylece inceleme videoları arasında farklı kodlama sonuçlarıyla sahte ışık değişimi oluşturulmaz.

## Doğrulama kapsamı

- 181 ham kare; başlangıç–bitiş farkı sıfır.
- Tüm hareket, orta bölümün ardışık kareleri ve son/ilk ardışık kareler için inceleme paftaları: `qa/whole-move.png`, `qa/middle-consecutive.png`, `qa/seam-consecutive.png`.
- Her iki MP4 tarayıcıda tekrar çözülür; üç tekrarlı videonun 3 ve 6 saniye birleşimleri ile 9 saniye bitişinden örnekler alınır: `qa/three-cycle-decoded-joins.png`.
- Kodlanmış ikinci/üçüncü tekrar başlangıçları ilk başlangıçla ayrıca karşılaştırılır.
- Asıl kaynak, önceki taslak, mevcut oyun PNG/MP4, shader/texture ve ilgili C# dosyaları SHA256 ile kontrol edilir. Eski MP4 yedeği birebir doğrulanır.

Sonuçlar `export-report.json`, `composition-report.json` ve `qa-report.json` içindedir. Bu teslim için Unity/gameplay testi çalıştırılmadı; çalışan oyunun zafer/menü davranışına dokunulmadı.

## Yeniden üretim ve yerel üretim kaydı

`prepare-copy.py` → `compose-scene.py` → `node export.cjs` → `prepare-review.py`.

Yerel üretim: yerleşik image_gen. Kullanılan son prompt, kaynak ve dosya yolu `prompts.json` içindedir. Yalnızca `floor-join-generated.png` içindeki yerel zemin düzenlemesi kullanıldı. Önceki geniş birleşim denemesinden kalan `seam-repair-generated.png` ve o denemelerin promptları inceleme izi olarak tutuldu; final sahnede kullanılmadı.
