# T47 · Kombo görselleri

98 RGBA PNG. Dosya yolları [Assets/Art/_yeni/T47](../../../Assets/Art/_yeni/T47/) köküne göre. Numaralı diziler artan sırayla bir kez oynar. Konum ve ölçekler 390 × 844 referans ekranındaki tasarım pikselleridir. Paftadaki her oyun ekranı, o kademenin farklı anlarındaki efektleri birlikte gösterir. [Önizleme](onizleme.png)

| Dosya | Tuval px | Kare | FPS | Kullanım |
|---|---:|---:|---:|---|
| `Badge/{x2,x3,x4,x5-plus}/in/01..05.png` | 780 × 264 | 5 × 4 | 24 | Rozet ve övgü plakası. 05 duruş pozu. |
| `Badge/{x2,x3,x4,x5-plus}/out/01..04.png` | 780 × 264 | 4 × 4 | 24 | Kısa ikinci sıçrama, sonra yukarı çekilerek kapanış. 04 sonunda kaldır. |
| `Burst/small/01..08.png` | 640 × 640 | 8 | 24 | ×2. Altın temas ışığı ve en çok 10 ana parçacık. |
| `Burst/medium/01..10.png` | 640 × 640 | 10 | 24 | ×3. Turuncu temas ışığı ve en çok 22 ana parçacık. |
| `Burst/large/01..12.png` | 640 × 640 | 12 | 24 | ×4 ve ×5+. Mercan-pembe temas ışığı ve en çok 36 ana parçacık. |
| `Burst/shock-ring/01..08.png` | 640 × 640 | 8 | 24 | Yüksek komboda genişleyen, kesik renkli şok halkası. |
| `Board/wave/01..08.png` | 688 × 688 | 8 | 24 | Tahta kenarında saat yönünde bir tur. İki karşılıklı ışık ucu. |
| `Screen/edge/01..06.png` | 390 × 844 | 6 | 24 | Ekran kenarı renk vuruşu. Katmanı tüm oyun alanına ger. |
| `Screen/flash.png` | 390 × 844 | 1 | — | Krem ekran flaşı. PNG alfa değeri 34/255. |
| `Meter/track.png` | 272 × 40 | 1 | — | Altın çerçeve ve mavi kanal. |
| `Meter/fill-{x2,x3,x4,x5-plus}.png` | 272 × 40 | 4 | — | Kademeye göre değişen dolgu. Yanan uç sağdan sola ilerler. |
| `Meter/ember.png` | 48 × 48 | 1 | — | Dolgunun hareketli ucuna bağlanan krem-altın kıvılcım. |
| `Meter/break/01..03.png` | 272 × 40 | 3 | 24 | Çatla, ayrıl, sön. 125 ms sonra kaldır. |

## Kademe ve canlı yazı

| Kombo | TR | EN | Rozet taban ölçeği¹ | Rozet başlat / duruş (ms)² | Patlama çapı³ | Tahta sarsıntısı genlik / süre |
|---|---|---|---:|---|---:|---|
| ×2 | Güzel! | Nice! | 0,77 | 100 / 180 | 280 px | 0,75 px / 75 ms |
| ×3 | Harika! | Great! | 0,87 | 115 / 210 | 300 px | 1,25 px / 95 ms |
| ×4 | Süper! | Super! | 0,96 | 130 / 235 | 312 px | 2 px / 125 ms |
| ×5+ | Muhteşem! | Amazing! | 1,04 | 140 / 270 | 320 px | 3 px / 150 ms |

¹ Kademe ölçeği ve giriş/çıkış hareketi PNG’lere işlenmiştir. Her PNG’yi 390 × 132 boyutunda, merkez pivotla çiz. Başlangıç merkezi çıkış konumunun 100–175 px üstünde, tahtanın üst bölgesinde. En geniş pozu hesaba katarak görünür alanı ekran içinde tut. Önizlemede merkez (195, 276).  
² İlk sayı başarılı çıkıştan rozete kadar gecikme. İkinci sayı beş giriş karesinden sonra duruş pozunu tutma süresi. Ardından dört çıkış karesi.  
³ Patlama PNG tuvalinin ekran boyutu. Parçacıkların görünür alanı buna göre daha küçük. Patlamayı parçanın çıkış noktasına ortala.

Metinler canlıdır. 390 × 132 tuvalin ölçeklenmemiş sol üstünden rakam merkezi (91, 64,5), övgü merkezi (230, 67). Rakam **Lilita One**, boyutlar 30,5 / 31 / 32 / 27 px. Övgü **Baloo 2 ExtraBold**, boyutlar 26,5 / 28,5 / 30 / 31,5 px. Krem renk `#FFF5D6`. Rakam için sıcak ince kenar `#784213` / `#883111` / `#811F4C` / `#512B78`, övgü için mavi `#234670`. Kenar 1 px. Unity font varlığında Türkçe glifleri ve `×` işaretini dahil et. Övgüye tek satırda yaklaşık 164 px ayır ve optik olarak ortala.

Metin grubu aynı merkez pivotu (195, 66) çevresinde hareket eder. Tablodaki kademe ölçeğini kare ölçeğiyle çarp. Açı pozitifse saat yönünün tersine. Y ofseti ekran koordinatında aşağı yönde pozitif.

| Dizi | Kare ölçekleri | Açı (°) | Y ofseti (px) |
|---|---|---|---|
| Giriş 01–05 | 0,46 · 0,91 · 1,15 · 1,04 · 1 | −10 · −6 · 3 · −1 · 0 | 10 · 3 · −7 · −3 · 0 |
| Çıkış 01–04 | 1 · 1,07 · 0,76 · 0,24 | 0 · 2 · −4 · −11 | 0 · −5 · −15 · −23 |

## Oynatma ve katmanlar

- **0 ms** · Başarılı çıkışın merkezinde patlamayı başlat. Temas ışığı 01–02’de zirve yapar, sonra resimli parçalar ve metalik kırıklar açılır. Küçük 333 ms, orta 417 ms, büyük 500 ms. ×5+ için bir küçük patlamayı 65 ms gecikmeyle ikinci bir çıkış noktası varsa orada, tek çıkış varsa ana merkezin 45–55 px yanında %60 çapta oynat.
- **×4 / ×5+** · Halkayı 42 ms gecikmeyle aç. Çap ×4 için 288 px, ×5+ için 320 px. Tahta dalgasını 0 ms’de başlat ve 333 ms oynat. Dalgayı 344 × 344 referans çerçeveyle aynı merkeze bağla. Işık izi tuval kenarından 20 px içeride. Farklı tahta boyutlarında izi çerçevenin iç bordürüne hizala. Kenar parlama süresi 250 ms. ×4’te kenar opaklığını %65, flaş sprite opaklığını %55 kullan. ×5+’ta ikisini %100 kullan. Flaş 25 ms’de tepe yapıp 100 ms’de sıfıra iner.
- **Katman sırası** · Tahta ve parçalar → çerçeve dalgası → şok halkası → patlama → ekran flaşı ve kenar → rozet, canlı yazı, sayaç ve HUD. Tahta ve ona bağlı efektleri aynı sarsıntı grubuna al. Genliği sıfıra doğru söndür. HUD’u ve ekran kenarını sabit tut. Mevcut azaltılmış hareket tercihi etkinse sarsıntı ve tam ekran flaşı kapat.
- **Puan geri bildirimi** · Çıkış puanı mevcut olaydan geliyorsa, rozet başlangıcından 150 ms sonra `+N` yazısını çıkışın 24 px üstünde aç. 300 ms’de 22 px yukarı taşı, son 100 ms’de soldur. Önizlemedeki puanlar yerleşim örneğidir.
- **Sayaç** · Her başarılı çıkışla 1.200 ms’ye kur. 136 × 20 gösterim, tahta altında ortalı. Önizleme merkezi (195, 648). Dolguyu ve yanan ucu `clamp01(kalanMs / 1200)` ile sür. Dolgu kaynak pikselinde x=8 ile x=263 arasında sağdan kırpılır. Yanlış hamlede zinciri kapat ve kalan dolgu ile ucu kaldır, ardından kırılma dizisini başlat. Süre dolduğunda boş sayaç 125 ms’de sönebilir. Yeni başarılı çıkış, yeni kademe ve dolguya hemen geçer.
- **Unity içe aktarım** · Sprite (2D and UI), alpha is transparency, mipmap kapalı, sabit merkez pivot. Ekran katmanlarını viewport’a sabitle. Kareleri boyuta göre kırpmadan yerleştir. PNG dizileri ve canlı metin eşleşmesi için motor entegrasyonunda tablodaki değerleri kullan.
