# Task19B — İki ekranlık stil pilotu

[İnceleme](../../Assets/Art/Menu/Task19B/inceleme.html) · [Dörtlü karşılaştırma](../../Assets/Art/Menu/Task19B/pilot-karsilastirma.png)

Hazır: `Assets/Art/Menu/Task19B/02-koleksiyon.png`, `04-basarilar.png` (ikisi de 1125×2436 @3x) ve `pilot-karsilastirma.png` (1692×1100). Karşılaştırmada sol iki telefon mevcut onaylı Task13 menüsü ve Task15 Durduruldu kartıdır; sağ iki telefon yeni pilotlardır. Telefonlar aynı görünen ölçekte gösterilir.

## Referanslardan uygulanan kararlar

İlk olarak `docs/art/references-menu-contact-sheet.png` incelendi; ardından `Assets/Art/References/WhatsApp Image 2026-10-23 at 18.34.52 (7).jpeg` galeri sahnesi, `(8).jpeg` dolu ana ekranı ve `(3).jpeg` kabartmalı etkinlik kartı tam boy incelendi. Kullanılan dersler: sıcak ve dolu mekân, geniş başlık plaketleri, kalın hacimli kenarlar, büyük ikonlar, güçlü görsel hiyerarşi. Royal Match karakteri, logosu ve arması kullanılmadı. Görsel kimlik için onaylı Task13 menüsü, Task15 Durduruldu kartı ve Level v2 tuşu üretime referans verildi.

- Koleksiyon: güneş alan petrol duvarlı galeri, kırmızı halı, altın oymalar, büyük turkuaz başlık; gerçek `Paintings/painting-01.jpg`–`painting-11.jpg` örnekleri. 11 tablo kazanılmış; 12 sıradaki; 13–15 lacivert zeminli ve iri kilitli. Dokuz üç-yıldız + iki iki-yıldız toplam **31** eder. Hücreler kaydırılacak 40 tabloluk listenin ilk görünümüdür.
- Başarılar: ayrı, sıcak ışıklı kupa salonu; beş kabartmalı krem-altın satır; tablo, çoklu tablo, bomba, yıldız ve kilitli şapka için hacimli madalyonlar. Beş satır, 16 başarı listesinin görünür örneğidir; üstteki 5/16 toplam durumdur. İki tamamlanan, iki ilerleyen ve bir kilitli durum gösterilir.
- Task13 `counter-pill`, `bottom-nav`, `nav-active`, dört sekme ikonu, yıldız ve bildirim rozeti yeniden kullanıldı. Etiketler tüm görsellerin üstünde, ikonların altında çizilir; seçili çerçeve etiketi örtemez. Her iki ekranda navigasyon aynı konumdadır.
- Kabul edilen veri kararları korundu: koleksiyon yalnız kazanılanı sayar (**11/40**); kalan ekranlarda Ev ilerlemesi 11/40, profil Bölüm 12/40 olacaktır. Bölüm yolu turkuaz-altın tamamlanan düğüm, iri altın güncel düğüm, gri-lacivert kilit ve kırmızı kenarlı Baron işareti kararıyla sonraki tura aittir. Task19 arşivi değiştirilmedi.

## Üretim ve katmanlar

Sahneler, başlık plaketleri, çerçeveler, karo yüzeyleri ve madalyonlar **built-in image_gen** ile üretildi. Kodla düz UI yüzeyi çizilmedi; `docs/art/build-task19b.py` mevcut sprite'ları, gerçek tablo örneklerini ve gerçek yazıları birleştirir. Görsel üretim komutları: [TASK19B_PROMPTS.json](TASK19B_PROMPTS.json).

- `source/koleksiyon-art-v2.png`: seçilen yazısız galeri; ilk üretim `koleksiyon-art.png` korunur. Revizyonda yalnız son sıranın lacivert kilitli içleri ve istenmeyen küçük kalp tepeliğinin kaldırılması istendi.
- `source/basarilar-art.png`: yazısız kupa salonu ve hacimli satırlar.
- `layers/*-art.png`: tablo ve onaylı sprite yerleşimleri dahil, metinsiz kompozit.
- `layers/*-text.png`: şeffaf ve ayrı gerçek Türkçe metin katmanı.
- `layers/*-text.json`: özgün Unicode metinler, boyutlar ve konumlar; isim/sayaç/ilerleme sayıları ayrı kayıtlardır.
- `source/sources.json`: kullanılan mevcut kaynaklar; `source/validation.json`: ölçü, font alt sınırı, güvenli alan ve çıktı SHA-256 kayıtları.

Metinler en az **15 pt / 45 px**; başlıklar krem, lacivert kontur ve alt gölgeli. Depodaki Fredoka-SemiBold kullanıldı; 1 px optik kalınlaştırma Bold ağırlığına yaklaştırır. Bu ortamda RAQM bulunmadığı ve depodaki font `ş/Ş/ğ/Ğ/İ` birleşik gliflerini içermediği için, fontun **kendi** çengel/kısa yay/nokta glifleri taban harfe doğru konumda yerleştirildi. `ö/ü/ç/ı` mevcut glifleri korunur; kodla aksan şekli çizilmez. Oyun entegrasyonunda tam Türkçe glifli Fredoka Bold kullanılması gerekir. Tam fontu ağdan edinme denemesi sistemin bağlantı kısıtıyla engellendi.

Üretim betiği iki ekranın ölçüsünü ve metin katmanlarının üst **132 px**, alt **102 px** güvenli alanlarını doğrular. Alt sekme görseli de alt güvenli alanın üzerinde biter. Ekranlar gözle kontrol edildi; açıklama/ilerleme çubuğu çakışmaları, Türkçe işaretlerin konumları ve sekme okunurluğu düzeltildi. HTML bağlantıları ayrıca doğrulanır.

Bu teslim yalnız iki ekranlık pilottur. Genel stil Zeyd tarafından onaylandıktan sonra 01, 02b, 03, 05, 06, 07 ve 08 ekranları sonraki turda yapılacak; bu turda C#, oyun verisi ve Unity sahnesine yazılmadı.
