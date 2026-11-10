# CorridorVictoryV2 - kapalı duvar ve zemin düzeltmesi

Güncel önizleme: `Assets/Art/Menu/CorridorVictoryV2/corridor_victory_v2_preview.mp4` (1920×1080, 60 FPS). Dikey oyun kadrajı siyah yan boşluklarla korunur. Kamera hareketi 3 saniyedir; video son pozu da içermek için bir ek sabit kareyle 3,0167 saniyedir.

## Yapılan hedefli düzeltmeler

- Eski sütun katmanı, uzak koridoru ve dış manzarayı da içeriyordu. Bu dış cephe kaynak maskesinden çıkarıldı. Arka yüzey artık tek kapalı duvardır: `z = 5.32 + .585x`. Sütunların yan/gövde/başlık/kaide yüzeyleri aynı perspektif ışınlarıyla kesiştirilir; en yakın yüzey örtüşmeyi belirler. Üstten kırpılmış mimari üyelerin devamı tamamlandı.
- Duvar ve süpürgelik aynı yüzeydedir. Orijinal ana tablo/altın çerçeve, bayraklar, mumlar ve renk/ışık dili korunur. Yeni bir dış açıklık ya da içinden dış dünya görünen geçit yoktur. Tablo içeriği doğal olarak manzara içermeye devam eder.
- Bitki/vazo, arkasındaki sütun parçasını taşımayan tek şeffaf varlıktır. Mevcut V2 referansından yerleşik **image_gen** ile ayrıştırıldı; `plant-extracted.png` kaynak dosyası ve `docs/art/corridor-continuity-prompts.json` tam promptu saklar. Aynı sabit varlık bütün karelerde kullanılır; bağımsız kare üretimi yoktur. Bitki kamera yönüne bakan ön katmanda doğru dünya konumu/ölçek/örtüşmeyle çizilir; temas gölgesi zemin üzerindedir.
- Eski zemin, kaynak fotoğraf ile yansıtılarak/katlanarak doldurulmuş atlas arasında geçiş yapıyordu. Bu yol kaldırıldı. Halı, özgün altın bordür ve mermer malzemeleri kaynak görselden bir kez düzleştirildi. Hepsi `y=-1.5` düzlemindeki aynı ortonormal S/N koordinatlarını kullanır. Mermer derzleri dünya düzleminde çizilir; tekrar sınırının kenar texelleri eşittir. Üçgen fotoğraf sınırı ve kareye göre değişen zemin örneklemesi yoktur.
- Statik menü artık aynı sahnenin başlangıç kamerasından üretilen `Static.png` dosyasını kullanır. Onaylı `menu_bg.png` ve `corridor-v04-source.png` değiştirilmedi. Tablo kırpması statik/hareketli durumda aynı shader açıklığını ve merkez kırpmasını kullanır; oyun içi testte hata veren `Sprite.OverrideGeometry` yolu kaldırıldı.
- Kamera yolu, ilk/son poz ve 3 saniyelik süre korunur. Yalnız zaman dağılımı değişti: %22 yumuşak hızlanma, %56 sabit orta hız, %22 yumuşak yavaşlama. Normalize hız tepesi 1,875'ten 1,282'ye indi (yaklaşık %32). Bekleme/ek kutlama/UI eklenmedi. Kazanma, Next ve menüye dönüş mantığı `GameRoot`/`CorridorVictoryQueue` içinde değişmedi.

## Görsel ve teknik doğrulama

**Tarayıcı / MP4**

- Aynı shader ile bütün hareketin 181 karesi alındı. 1,3–2,0 saniye arasındaki 43 ardışık kare ayrıca incelendi.
- `qa/continuity-checkpoints.png`: 0 / 0,75 / 1,3 / 1,5 / 1,75 / 2 / 2,25 / 3 saniye.
- `qa/continuity-middle-consecutive.png`: kritik bölümdeki ardışık kareler.
- `qa/continuity-floor-critical.png`: halı/bordür/mermerin kritik zamanlardaki yakın görünümü.
- `qa/continuity-whole-move.png`: hareketin tamamının 0,05 saniye aralıklı taraması.
- `qa/continuity-before-after-2s.png`: verilen MP4 ile düzeltilmiş sahnenin 2. saniye karşılaştırması.
- Başlangıç/son PNG'leri aynıdır; görüntü geçişi/fade yoktur. Statik görüntü filtrelemesi ile hemen bitişik canlı kare arasındaki ortalama RGB farkı yaklaşık 0,33/255'tir. Kadraj değişimi değildir.
- Sayısal taramada izole zamansal sıçrama bulunmadı. Bu ölçüm görsel incelemeye destek verir; tek başına sanatsal kalite kanıtı sayılmaz.
- Güncel MP4 doğrudan mevcut tarayıcı renderer'ından 1/60 saniyelik sabit örneklerle kodlanır. H.264 yazılım kodlayıcı kullanılır. MP4 tekrar açılıp 0 / 1,3 / 1,5 / 1,75 / 2 / 3 saniyeleri decode edilerek boyut ve süre doğrulanır. `export/mp4-export-report.json` ve `export/decoded-*.png` kayıtlarıdır.

**Unity**

- Beş birim testi: tetikleme/tek tüketim/iptal, kamera yolu ve yeni hız eğrisi.
- Bir Editor PlayMode entegrasyon testi: gerçek oyun sahnesi yüklenir, gerçek bölüm Core hamleleriyle kazanılır, normal `ShowMenu` eylemi çağrılır, tek oynatım ve 3 saniye sonunda statik duruma dönüş doğrulanır. Tekrar normal menüye dönüş animasyonu başlatmaz. Kayıt bellektedir; telemetri kapalıdır.
- Toplam **6/6 test** geçer: `unity-tests.xml`. Oyun akışı raporu: `qa/unity/win-menu-integration.json`.
- Gerçek oyun kamerasından `qa/unity/game-menu-initial.png`, `game-win-card.png`, `game-menu-travel-start.png`, `game-menu-travel-middle.png`, `game-menu-static-end.png` kaydedilir. Editor GameView çözünürlüğü kullanılır; debug yazısı testin mevcut debug modudur, yeni oyun UI'si değildir. Batch testinde ilk shader ısınmasının ölçüm saatini yaşlandırmaması için ilk menü önceden çizilir.
- Ayrı GPU kontrolü, gerçek Unity materyalini Direct3D12'de 0/25/50/75/100% pozlarında render eder: `qa/unity/result.json` ve `frame-*.png`.

Gerçek iPhone, yayımlanmış Web player ya da TestFlight üzerinde manuel test yapılmadı. Tarayıcı performansı masaüstü GPU ölçümüdür; mobil FPS sonucu değildir. Editor PlayMode testi gerçek kazanma/menü kodunu kullanır; insan tarafından fareyle oynanmış cihaz testi değildir.

## Dosyalar ve yeniden üretim

- İşlenen varlıklar ve MP4: `Assets/Art/Menu/CorridorVictoryV2/`.
- Çalışan kaynaklar: `Assets/_Game/Presentation/Resources/CorridorVictory/`.
- Aynı görünümü kullanan paylaşılan shader: `CorridorProjection.cginc`.
- Güncel atlas üreticisi: `docs/art/build-corridor-continuity.py`; eski build komutları bu üreticiye yönlendirilir.
- Maskeler: `columns-source-mask.png`, `wall-source-validity.png`. Dış cephe bölgesi etkin kaynak maskelerinde yoktur; yakın sütun kaidesinin dört piksellik antialias payı ayrı tutulur.
- Tam varlık/model manifesti: `Assets/Art/Menu/CorridorVictoryV2/manifest.json`.
- Denetleyici: `docs/art/check-corridor-continuity.py`; rapor: `qa/continuity-report.json`.

Yeniden üretim sırası: atlasları üret → `node Tools/corridor_victory_preview.js` ile yerel önizlemeyi başlat → `node docs/art/corridor-victory/export-preview-mp4.cjs --capture-static` → aynı dışa aktarıcıyı `--review-frames` ve `--software` seçenekleriyle çalıştır. `node Tools/corridor_victory_preview.js --verify` tam taraması kendi sunucusunu açar; bunun için normal önizleme sunucusunu önce durdurup taramadan sonra yeniden başlat. Ardından `docs/art/check-corridor-continuity.py` raporları üretir. Önizleme adresi: <http://127.0.0.1:8840/docs/art/corridor-victory/index.html>.

Önceki çalışan V2, verilen MP4 dahil, **`docs/art/backups/corridor-victory-v2-before-continuity-20261030/`** altında tam yedektir. Önceki `join-repair-source.png` tarihî referans olarak saklanır; açık koridoru kapatmak için canlı sahneye yapıştırılmaz.
