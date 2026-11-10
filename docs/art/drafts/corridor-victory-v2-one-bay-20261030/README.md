# Tek komşu bölüm — düzeltilmiş onay taslağı

Bu çalışma önceki üç tabloluk kadraj önerisinin yerini alan **ayrı taslaktır**. Oyun animasyonuna uygulanmadı. Önceki taslak da korundu.

## Açılacak dosyalar

- `review-framing.png`: tek bölüm mesafesinin işaretlendiği sahne ve üç temiz kadraj.
- `frame-start.png`: 1. tablo merkezde, 780×1688.
- `frame-middle.png`: iki komşu tablo arasındaki duvar, sütun ve vazo, 780×1688.
- `frame-end.png`: hemen komşu 2. tablo merkezde; başlangıç ile piksel piksel aynı, 780×1688.
- `one-bay-loop.mp4`: bir bölümün 3 saniyelik hareketi, 60 FPS; son tam kare dâhil dosya süresi 181/60 saniye. Tekrar oynatmak için video oynatıcının döngü seçeneği kullanılabilir.
- `one-bay-loop.webp`: aynı tek bölüm hareketinin 3 saniyelik, 30 FPS, sürekli tekrar eden inceleme görseli.
- `index.html`: yerel HTTP sunucusundan açıldığında oynat/durdur ve inceleme kaydırıcısı ile canlı taslak. Buradaki tekrar yalnızca görsel onay içindir; oyundaki tek oynatım davranışı değiştirilmedi.
- `panorama-one-bay.png`: başlangıç penceresi ile bir tam komşu bölüm mesafesini kapsayan işaretsiz sahne.

## Kompozisyon ve tekrarın temeli

Bir mimari bölüm = **992 kaynak piksel**. Hareket boyunca yalnızca bu mesafe katedilir. Ortada başka bir tabloya ulaşılmaz; komşu tablolar arasındaki sütun görülür. Görüntü ekranda sağdan sola gider. Ekran oranı 390:844; çıktı 780×1688.

Önceki panoramanın farklı bölgeleri başa sarılmadı. Beğenilen görsel dil referans alınarak **tek bir mimari tekrar modülü** üretildi. Duvar kapalı, sütun ve vazo onun önündedir. Duvar ve uzun zemin/halı çizgileri sabit, önden ve sığ bir bakışla kurulmuştur. Yatayda giderek küçülen perspektif kullanılmaz; böylece komşu modülde süpürgelik, zemin ve ışık yükseklikleri değişmez. Halı ve mermer düzlemi, vazo kaidesi ile beraber aynı sabit resmin parçasıdır; hareket sırasında esnetilmez veya ayrı kaydırılmaz.

Kaynak `module-source.png` (1024×1536), yerleşik image_gen ile tek bir sabit sahne olarak üretildi. İlk üretim promptu ve referans `generation.json` içindedir. İncelemede kenarda kesilen çapraz güneş lekesi görüldü; aynı sahnede yalnızca bu doğrudan ışık desenleri kaldırıldı, sıcak ortam ışığı, altın parlamalar ve vazonun temas gölgesi korundu. Bu yerleşik image_gen düzenlemesinin promptu `lighting-edit.json` içindedir; önceki sabit sahne de `module-source-before-light-fix.png` adıyla saklandı. Animasyon kareleri bağımsız üretilmedi.

Kaynaktaki nesnesiz 32 piksellik kenar alanları renderer içinde sabit bir uzamsal geçişle eşlendi; kalan 992 piksel ortak tekrar malzemesidir (`module-repeat.png`). Duvar, dado, mermer ve halı **aynı periyodu birlikte** kullanır. Bu geçiş zamana bağlı bir erime, kare üretimi veya uç kareye solma değildir. Başlangıç ve bitiş, aynı periyodik sahnenin komşu bölümleridir. Renderer'ın periyodik koordinatları, aynı piksel örneklemesinde uç eşitliğini de sağlar.

Hareket 3 saniyelik quintic yumuşatma ile başlar ve biter; kadraj/perspektif değişmez. Döngünün sonundan başına aynı sahne, aynı yerleşim ve sıfır hızla geçilir. Bu yalnızca tek görselle kaydırma uygunluğunu değerlendiren onay taslağıdır; yeni oyun kamera uygulaması değildir.

## Doğrulama

`export-report.json`: tarayıcıdaki gerçek taslak çiziminden 181 kare, 60 FPS H.264 MP4; boyut ve süre tarayıcıda tekrar çözülerek kontrol edildi. Başlangıç/bitiş ham piksel farkı sıfırdır.

`qa-report.json`: 181 ardışık ham karenin tamamının fark ölçümleri, döngü birleşimi, başlangıç/bitiş eşitliği ve korunan çalışma zamanı dosyalarının SHA256 kontrolü. `qa/whole-move.png`, `qa/middle-consecutive.png`, `qa/loop-seam-consecutive.png` görsel inceleme paftalarıdır. MP4'ün çözülmüş örnek kareleri de `qa/decoded-*.png` içindedir.

Yeni işlerin tamamı bu klasöre yazıldı. Çalışan CorridorVictoryV2 PNG/MP4 dosyaları, çalışma zamanı shader ve texture dosyaları, zafer kuyruğu, Screens.cs ve GameRoot.cs korunmuştur. Oyun entegrasyonu ve oyun içi kazanma testi bu aşamanın kapsamına alınmadı. Sanatsal kadraj ve döngü onayı bekleniyor.
