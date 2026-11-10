# Task22 — son görsel ve teknik inceleme

Bu çalışma bir art aday teslimidir. Unity, build, canlı telefon/klavye veya etkileşim testi yapılmadı. Önce/sonra görüntüleri kaynak ekranın aynı kırpımına yerleştirilmiş önerilerdir; gerçek yeni oyun ekranı diye sunulmaz.

## Denemeler ve son kontrol

İlk taslakta ayar hizası, bulanık harita bağlamı, alt çubuğa yakın ilerleme kapsülü ve fazla yataylaşan ana tuşlar yeterince iyi değildi. İkinci taslakta özgün ana tuş en/boyu ve mevcut soru/yıldız akışı korundu. Üçüncü tasarım yönünde boyalı başlık/anahtar malzemeleri, sembol fontuna bağımlı olmayan tik/çarpı, tam kaynak kırpımı ve kesin yerleşim düzeltildi. İlk iki denemenin önce/sonra kopyaları `deneme1/` ve `deneme2/` içinde saklandı; hiçbir dosya silinmedi/taşınmadı. Aynı yön içindeki alfa/başlık etiketi/dosya kaynağı export düzeltmeleri yeni bir tasarım yönü sayılmadı.

01–22'nin her biri, ilk kaynak kontakları ve son telefon kontakları ile tekrar incelendi. Ayrıca dar kırpımlı 18/19, başlık ve etiket taşması için gerçek piksel boyutunda; 01/06 eski-yeni tam kart karşılaştırması için ayrı açıldı. 101–113 ek ailesi toplu kontakta, 101 ve109 ayrı büyük kırpımda incelendi. Ek adayların PNG'leri değiştirilmedi.

| No | Son görsel inceleme sonucu |
|---|---|
| 01 | Kalın altın kabuk yerine boyalı turkuaz/krem; üç yıldız ve iki sıra soru korunuyor, tek yeşil ana eylem. İstatistikler örnek dinamik metinlerdir. |
| 02 | İki sıra mevcut akış korunuyor; normal/seçili/basılı kutu ailesi ve alt dudak, 50×44 pt hedef. 7 yalnız seçili durum örneği. |
| 03 | Compact sonuçta bomba nedeni ve 0 örnek sayısı tekrar görünür; yeniden dene ilk eylem, puan sorusu ikincil. |
| 04 | Yeni 3 Kasım süre kaybı kaynağı; saat nedeni büyük, soru olmayan kaynak durumuna gereksiz soru eklenmedi. |
| 05 | Yeni 3 Kasım pause kaynağı; Devam yeşil, yeniden başla/ana menü turkuaz. |
| 06 | Çivi sayacı 5 örneği başrolde, iki kısa kural, mevcut onaylı nesne kimliği. Eski karakter illüstrasyonunun farklı yönde sadeleştirilmiş adayıdır. |
| 07 | Bomba sayacı 23 örneği, fitil önceliği, nesne ve tek eylem hiyerarşisi. |
| 08 | İki uç ve zincir bağlantısı; mevcut spot bağlamı açıkça etiketli. Ayrı modal varmış gibi gösterilmez. |
| 09 | Kırmızı-beyaz kenar nesnesi ve açık yol kuralı; spot bağlamı etiketli. |
| 10 | Makas nesnesi büyüdü; birleşim noktası talimatı kısa, el grafiğine müdahale yok. |
| 11 | Çekiç nesnesi büyük; tek parça hedefi. Kaynak booster öğretim bağlamı. |
| 12 | Saat nesnesi büyük; +20 saniye mevcut kuralı korunuyor. |
| 13 | Değnek nesnesi ve resim değişmez kuralı; yeni bir mekanik icat edilmedi. |
| 14 | Yeni 3 Kasım zor giriş bağlamı; ayrı uyarı yönü. Kaynak “zor başlangıç” ile öneri “zor uyarı” farkı NOT.md'de. |
| 15 | Boş beklemeye oyun kimliği ve üç noktalı belirsiz bekleme; sahte yüzde çizilmedi. |
| 16 | Form paneli ortak aileye alındı; boş isim alanı ve büyük yeşil Başla. Klavye davranışı hâlâ kod/cihaz işi. |
| 17 | Üç yanıt ortak ikincil tuş ailesinde; metinler en az13 pt. Anketin atlanması ayrıca ürün kararıdır. |
| 18 | Boyalı mevcut anahtar ailesinde daha büyük düğme + çizilmiş tik/çarpı. Fonttaki eksik tik glifi kullanılmıyor. Üç açık durum kaynak tercihiyle aynı. |
| 19 | Boyalı segmentler, krem/lacivert yazı ve ayrı seçim tiki. Kaynaktaki English seçimi korunuyor. |
| 20 | Düğümler aynı mevcut merkezlerde; yol, avatar ve onaylı parlama korunuyor. Bulanık taslak kullanılmıyor. |
| 21 | Sıradaki gerçek 5. hücre üstünde; kilitli6 aynı yerinde. Çizim gerçek tablo resmini değiştirmiyor. |
| 22 | Kısa krem/turkuaz kapsül, 315/2.000 ve 500/1.000 eşikleri; alt çubuğa taşmıyor, mevcut ilerleme işlevi korunuyor. |

## Teknik kapılar

- Ana aile: 22 iş /130 şeffaf RGBA üretim adayı. Her dosyada gerçek alfa, @3x tam pt ölçü, geçerli slice sınırları ve manifest SHA-256 kontrolü. Belirtilen dokunma hedefleri en az44 pt. Metin katmanlarında içerik metni en az13 pt.
- Ana yüksek/orta çizim bulguları ile teslim klasörleri birebir eşleşir. Her klasörde önce/sonra, tam ekran, iki telefon boyutu, metin yerleşim JSON'u ve NOT.md var.
- Ana taslak katmanlarının HSV altın maskeleri %10 altında. Yöntem `gold-measurements.json` içinde; bu sezgisel yerel katman ölçüsüdür, resim sarılarını dışlayarak tüm oyunun algısal altın oranı ölçülmüş sayılmaz.
- Ek aile: 13 alternatif /20 şeffaf @3x PNG, bağımsız boyut/alfa/hash kontrolü geçti. Ek kaynağın kendi `ai-validation.json` raporu da korunur. 110 tek düğümde %13,875 altın maskesi taşır; toplam ekran oranı ayrıca değerlendirilir.
- 35 aile ve bütün mevcut önce/sonralar tek `inceleme.html` içinde. Harici ağ/font bağımlılığı yok. HTML hedefleri ve dosyaları statik olarak doğrulandı; browser etkileşim testi yapılmış sayılmaz.
- Başlangıçtaki 3.082 okunabilir Assets/Art dosyasının SHA-256'sı aynı; eksilen dosya yok. Kontrol yalnız bu başlangıç sanat havuzunu kanıtlar. Eşzamanlı build'in kendi geçici Resources dosyalarındaki değişiklikleri bu art exportuna atfedilmez.
- İlk denemeden kalan `01-kazanma/nesne-alani.png` artık manifestte kullanılmıyor; silme yasağı nedeniyle yerinde tutuldu ve `unusedAttemptFiles` alanında işaretlendi. 101–113 ek ailesi “kullanılmayan deneme artığı” sayılmaz.

## Karar bekleyen sınırlar

40. bölümün gerçek terminal kazanma kartı yeni tam turda da yok; lv40 görüntüsü oyun tahtasıdır. Ana rapor bunu yüksek kod/QA işi olarak kaydeder. Eksik kareye dayanarak sahte önce/sonra üretilmedi. Güncel zincir/duvar/üç booster için ayrı modal kaydı olmayan yerlerde en yeni öğretim bağlamı kullanıldı ve etiketlendi.

112 alternatifi üç madalyona döner; PM33 ilerleme gereğiyle ürün kararı gerekir. 22 numaralı ana aday aynı ilerleme işlevini koruyan öneridir. Hiçbir aday onaylı/oyuna entegre sayılmaz.

Makine sonuçları: [validation.json](validation.json), [consolidation.json](consolidation.json), [original-art-check.json](original-art-check.json), [gold-measurements.json](gold-measurements.json).
