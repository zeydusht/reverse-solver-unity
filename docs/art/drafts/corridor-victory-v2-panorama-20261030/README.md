# CorridorVictoryV2 — geniş sahne görsel onayı

Durum: yalnızca sanatsal onay taslağı. Animasyona uygulanmadı.

## Teslimler

- `panorama-clean.png`: tek, kesintisiz çizim; işaretsiz, doğal üretim boyutu 1619×971. Ayrı karelerin birleştirilmesi değildir.
- `review-framing.png`: aynı panorama üzerindeki başlangıç / orta / bitiş pencereleri, görüntünün **ekranda sağdan sola** hareketini gösteren ok ve üç kadrajın ayrı gösterimi. Üzerindeki grafikler yalnızca inceleme içindir.
- `frame-start.png`, `frame-middle.png`, `frame-end.png`: 390×844, tam oyun önizlemesi oranında, aynı sahneden kırpılmış temiz kadrajlar. Ayrı üretilmiş kareler değildir.
- `generation.json`: yerleşik image_gen ile kullanılan tam prompt, kaynak referans ve üretilen dosyanın yolu.
- `review-manifest.json`: kırpım koordinatları, boyutlar, SHA256 değerleri ve korunmuş dosyaların değişmediği kontrolü.

## Görsel yaklaşım ve kontrol

Ana referans `Assets/Art/Menu/CorridorVictoryV2/Source.png`; mevcut MP4'ün dikey içerik oranı 390:844. Teal duvar, sıcak taş, altın oyma çerçeveler, kırmızı/altın halı, mavi/altın vazolar ve gün ışığı korundu. Duvar bütün genişlik boyunca kapalıdır; dış manzaralar yalnızca tablolardadır. Vazolar mermer şerit üzerindeki kaidelerde durur; halı, taş zeminin önünde tek sürekli yüzey olarak resmedilmiştir. Temiz panorama ve inceleme paftası görsel olarak incelendi.

Bu taslak, tek sabit ve sığ açılı perspektifle çizilmiş geniş bir sahnedir. Kadrajlar görüntünün sağdan sola hareketini önerir; kaynak panorama üzerinde örnekleme penceresi soldan sağa ilerler. İşaretli uçlar arasında yaklaşık **2,20 dikey ekran genişliği** vardır. Dış kenarlarda yaklaşık 91 piksellik kompozisyon payı bırakılmıştır. Perspektif değiştiren bir kamera dönüşü veya panoramanın esnetilmesi önerilmemektedir. Görsel onayından sonra tek görsel hareketinin yeterliliği değerlendirilecektir; gerekirse aynı onaylı sahneden katmanlar çıkarılacaktır.

Kırpımlar sabit çizimin kadraj örnekleridir; animasyon, hareket testi, MP4 veya oyun entegrasyonu teslimi değildir. Başlangıç ve bitiş aynı dosyanın kopyası değildir; tekrarın bitişte nasıl duracağı sonraki uygulama aşamasında ayrıca doğrulanmalıdır.

`Assets/Art/Menu/CorridorVictoryV2`, çalışma zamanı CorridorVictory kaynakları, Screens.cs ve GameRoot.cs dosyaları öncesi/sonrası SHA256 ile kontrol edildi: değişiklik yok. Çalışan MP4 değiştirilmedi. Bütün yeni dosyalar bu taslak klasöründedir.

Sonraki işlem için yalnızca bu kompozisyonun ve kadrajların sanatsal onayı beklenmektedir.
