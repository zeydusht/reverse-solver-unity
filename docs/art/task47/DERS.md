# T47 · Block Blast kombo incelemesi

5:17’lik kaydı önce FFmpeg ile 1 FPS’de baştan sona taradım, sonra kombo anlarını 20 FPS’de inceledim. Zamanlar en yakın 50 ms’lik örneği, boyutlar 150 px’e küçültülmüş tahta üzerinde yaklaşık görünür alanı gösteriyor. [Genel paftalar](ref/)

| Kademe | Yazı ve konum | Patlama, renk ve ışık |
|---|---|---|
| 1 · 01:01,800 | Mavi “Combo”, rakam yok. Sol taraftaki çıkış sütununun orta hizasında. Yazı zirvede ≈100 × 29 px, oturunca ≈80 × 23 px. | Bir mavi sütun ≈19 px genişliğinde aydınlanıyor. Kırıklar yaklaşık 1–1,5 hücre genişliğine yayılıyor. |
| 2 · 01:04,250 | Önce sözcük ve altın ışık, rakam ≈50–100 ms sonra belirgin. Sözcük oturunca ≈80 × 23 px, rakam ≈22 × 30 px. Yine çıkan sütun hizasında. | Sarı sütun ve aynı renkte kırıklar. Sütun 01:03,950’de parlıyor, kırıklar 01:04,200–01:04,650 arasında okunuyor. Rakamın çevresinde ayrı bir altın ışık var. |
| 3 · 01:06,150 | Sözcük ≈80 × 23 px, rakam ≈24 × 31 px. Rakam girişte ≈40–45 px yüksekliğe sıçrıyor. | Camgöbeği sütun ve birkaç renkli nokta. Kırıklar 01:06,050–01:06,550 arasında okunuyor. Tek sütun örneklerinde yaklaşık 20–35 seçilebilir iri kırık var, 1’den 3’e düzenli adet artışı seçilmiyor. |
| 4+ · 01:15,100 ve 01:51,400 | “Combo 5” üstte temizlenen sıralara taşınıyor. 5 oturunca ≈25 × 33 px, tepe ≈45 px. “Combo 10”un iki haneli rakam grubu tepe anında ≈60 × 44 px. | 5’te iki kırmızı sıra ≈38 px’lik bir şerit olarak başlıyor ve kırıklar ≈4 hücre derinliğine saçılıyor. Görünür kırıklar tek çizgiden belirgin fazla. Alan farkına ikinci sıra da katkı veriyor. 10’da tek camgöbeği sütunda bile tahta çerçevesi renkleniyor. |

Ölçülen düz seri 3’ten 5’e iki sıralı temizlemeyle geçti. Kaydın ayrıca tam tahta ödülüne ait X1–X5 katmanı var. 00:04,250’de X1 ≈38 × 24 px, 03:11,050’de X4 ve 03:17,550’de X5 ≈40 × 25 px. İkisi de tahta ortasında ≈750 ms duruyor ve ≈100–150 ms’de sönüyor. Büyük balon ve tema geçişleri bu ödüle eşlik ediyor. Düzenli kombo merdiveninde her kademede yazı boyu veya parçacık adedi artışı bu kayıttan çıkarılamıyor. [1](ref/seri-01-01.jpg) · [2](ref/seri-02-01.jpg) · [3](ref/seri-03-01.jpg) · [5](ref/seri-05-01.jpg) · [10](ref/seri-10-01.jpg) · [X4](ref/temiz-tahta-x4-02.jpg) · [X5](ref/temiz-tahta-x5-02.jpg)

**Yazı ritmi.** Çizgi önce yanıyor, yazı yaklaşık 300–350 ms sonra çıkıyor. Giriş küçük pozdan yaklaşık 1,25–1,5× tepeye 50–100 ms’de ulaşıyor, 100–150 ms’de 1×’e oturuyor. 2 ve 3 toplam ≈600–650 ms görünür. Son 50–100 ms’de kısa bir yeniden büyüme ve merkeze büzülme var. Harflerde sabit yatık çizim var, örneklerde belirgin animasyonlu dönüş seçilmiyor. [2 çıkışı](ref/seri-02-cikis-01.jpg)

**Sarsıntı ve flaş.** Sabit hücreler ve dış HUD karşılaştırıldığında 20 FPS’de genel ekran sarsıntısı seçilmiyor. Parlaklık çıkış çizgisi ile skor çevresinde yoğun. 5’te tahta kenarı 01:14,750–01:15,550 boyunca önce camgöbeği/pembe, sonra altın. Altın noktalar kenar boyunca farklı yerlerde yanıyor. 10’daki renkli çerçeve ≈450–500 ms. Tam tahta ödüllerindeki tüm ekran renk değişimi ayrıca tema geçişiyle birlikte görülüyor. [5](ref/seri-05-01.jpg) · [10](ref/seri-10-01.jpg)

**Skor.** Altın +105, +140 ve +627 etiketleri temizlenen bölgeye yakın, kombo yazısı kaybolduktan sonra beliriyor. +627 01:16,300–01:16,550 arasında görünür, sabit noktaya yakın kalıp solar. Bu örneklerde HUD’a kadar çizilmiş bir uçuş görünmüyor. Üst toplam ayrı bir animasyonla artıyor ve çevresinde camgöbeği parıltı yanıyor. [Skor](ref/skor-05-01.jpg)

## Oyuna aktarılacak üç ders

1. **Parçayı önce aydınlat, sonra dağıt.** Çıkışın nerede olduğunu ilk karede doygun ışıkla göster. Ardından resimli yapboz kırıkları ve metalik yıldızlar açığa çıksın.
2. **Vuruşları zamana yay.** Temas, kırılma, rozet sıçraması ve puan geri bildirimi ayrı vuruşlar olsun. Rozet çıkış konumuna yakın belirsin ve çabuk yer açsın.
3. **Büyük anı çerçeveye taşı.** Yüksek kademeye şok halkası ve gezici çerçeve ışığı ekle. Tahta görünür kalsın.

Blok maskotları, mavi eğik “Combo” yazısı, küp kırıkları, balonlar, kalpler, tema adları ve neon çerçeve tasarımı referansa ait. Reverse Solver’ın krem canlı yazısını, altın işçiliğini, resimli yapboz kırıklarını ve mavi galeri HUD’unu kullanacağız. Renk merdiveni, hafif sarsıntı ve 1,2 sn’lik sayaç T47 tasarım kararlarıdır.
