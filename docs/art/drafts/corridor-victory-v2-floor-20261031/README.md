# Yalnızca zemin revizyonu

Ayrı taslak. Üst sahne, hareket yönü/mesafesi, tablo/mimari, ölçek ve 3 saniyelik yumuşatma korunmuştur. Oyuna uygulanmadı.

## Teslimler

- `floor-revision-step.mp4`: bir komşu bölüm, yaklaşık 3 saniye; 780×1688, 60 FPS.
- `floor-review-three-cycles.mp4`: aynı hareketin üç tekrarı; yaklaşık 9 saniye, 60 FPS.
- `floor-close-start.png`, `floor-close-middle.png`, `floor-close-end.png`: gerçek renderdan alınan temiz zemin yakın görüntüleri, 780×498.
- `floor-closeups.png`: üç yakın görüntünün etiketli inceleme paftası.
- `floor-three-bays.png`: tekrar sınırları dâhil bütün zemin malzemesinin üç bölüm boyunca görünümü.
- `qa/whole-floor-move.png`: tüm hareketten 37 zemin örneği.
- `qa/middle-floor-consecutive.png`: 1,30–2,00 saniye aralığındaki 43 ardışık zemin karesi.
- `qa/loop-floor-consecutive.png`: son/ilk ardışık kareler.
- `qa/three-cycle-floor-joins.png`: gerçek üç tekrarlı MP4'ün çözümlenmiş birleşim kareleri.

## Ne değişti?

Önceki dar zemin yaması, alt halı doku uzantısı ve farklı yükseklikte kesilmiş bordür kaldırıldı. Zemin, **bir bütün olarak tek periyodik malzeme düzeni** ile hazırlandı. Yerel bir yamanın üstü kapatılmadı.

Kırmızı halıdaki mevcut desenin yerel piksel parçaları kendi boyutlarında kullanıldı. Birleştirmeler, örtüşme alanındaki en küçük renk hatasını izleyen kesimlerle yapılır; dokuya blur filtresi veya alpha yama geçişi uygulanmaz. Motifler ölçeklenmez. Böylece dokudaki tekrar komşuları birbirine uyar; bulanık dikdörtgenler oluşmaz.

Altın bordür, orijinal şeritten alınan mevcut motif ve ince altın raylardan hazırlanır. Kaynak şeridin eğimi bir defalık rijit 2,6° dönüşle hizalanır; motifler büyütülmez/küçültülmez. Aynı motif, aradaki boşlukta birer piksellik farklarla 99/99/100/99/99 piksel aralıklara yerleştirilir; toplam 496 piksel tam mimari periyodudur. **Tek bir bordür bandı** vardır; kaynak y794–826 bandında kalır. Tekrar sınırında ikinci bir bordür parçası, seviye sıçraması veya kırık uç yoktur.

Mermer küçük fotoğraf parçalarından döşenmedi. Orijinal sıcak mermer renklerinden örneklenen sürekli damar/yansıma alanı, tek affine düzlemde hazırlandı. İki derz ailesinin ortak temel vektörleri `(124,0)` ve `(32,24)`; bir mimari periyot dört yatay taş aralığıdır. Derzler tekrar kenarında aynı doğrultuda devam eder. Kaynak kaidenin alt sınırı büyütülerek incelendi: nesnenin sonu y725, aşağısındaki alan eski zemin yansımasıdır. Yeni düzlem y726'dan başlar; kaidenin kendi taş/altın pikselleri aynen korunur. Kaide dışındaki duvar süpürgeliği y729'a kadar ayrıca korunur. Eski yansıma şeridi yeni mermerin üzerine kopyalanmaz; temas gölgesi ve sıcak yansıma yeni düzlemle birlikte devam eder.

Bu revizyonda yeni imagegen çizimi kullanılmadı. Kırmızı doku ve altın süsler mevcut kaynaktan; mermerin sürekli yüzeyi aynı sıcak paletten, ortak düzlem koordinatlarında hazırlanmıştır. Üst sahne yeniden üretilmedi.

## Ne korundu?

`preview.js`, önceki `corridor-victory-v2-panorama-matched-20261031/preview.js` ile **bayt bayt aynı**. 496 kaynak piksel ilerleme, aynı yön, 390:844 kadraj oranı, sabit ölçek, aynı quintic easing ve 3 saniye korunur. Hareket yalnızca sabit sahnenin kırpımıdır; morph, zamana bağlı crossfade veya animasyonlu zemin deformasyonu yoktur.

Kaynak y726 üzerindeki bütün üst sahne ve kaide pikselleri önceki modülle eşittir. Kaidenin dışındaki duvar süpürgeliği y729'a kadar da eşittir. 181 renderın üst sahne alanı önceki karelerle ayrıca piksel piksel karşılaştırılır.

Önceki revizyonun üst düzey teslimleri ve MP4'leri `backup/` klasöründedir; önceki taslak klasörü değiştirilmedi. Çalışan oyun assetleri/MP4'ü, kaynak panorama ve ilgili C#/shader dosyaları korunmuştur.

## Kontrol

Başlangıç/bitiş ham piksel eşitliği kontrol edilir, fakat tek başarı ölçütü değildir. Bütün 181 zemin karesi, orta bölümün ardışık kareleri, zemin malzemesinin kendi tekrar kenarı ve üç tekrarlı MP4'ün 3/6 saniye birleşimleri kontrol edilir. Altın bordürün tek sabit y bandında kalması ve mermer derzlerinin ortak düzlemi ayrıca kaydedilir. Sonuçlar `floor-build-report.json`, `export-report.json`, `qa-report.json` içindedir.

Bu aşamada Unity/gameplay testi yapılmadı; oyun kodunda veya oyunun zafer/menü davranışında değişiklik yoktur.

Yeniden üretim: `build-floor.py` → `node export.cjs` → `prepare-review.py`.
