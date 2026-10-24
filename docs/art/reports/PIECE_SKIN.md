# Yapboz parçası skin teslimi · 25 Ekim 2026

Tek üretim skin'i: **Petrol / sıcak porselen**. Mevcut `PieceSkin/Review/piece-direction-*.svg` eskizleri tarihsel yön çalışmalarıdır; teslim edilen skin bunlardan bağımsız tek seçenektir.

## Dosyalar ve sözleşme

- `Assets/Art/PieceSkin/skin.json`: stil değerlerinin ana kaynağı. Tüm `Pt` alanları CSS pt/oyun birimi; @3x çıktıda 3 ile çarpılır. Renkler sRGB `#RRGGBB`, alfa gereken renkler `#RRGGBBAA`; ayrı opacity değerleri 0–1. Blur/glow değerleri Canvas shadowBlur görsel referansıdır; Unity shader karşılığı aynı görünüm için ayarlanmalıdır.
- `Assets/Art/PieceSkin/body-256.png`: 256×256 opak, düz krem #F4E8CF; kontrast %0, tüm karşı kenarlar eşit ve kesintisiz tekrar eder. Hafif gövde için görevde izin verilen düz dolgu seçildi. Sabit parça sprite'ı değildir.
- `Assets/Art/PieceSkin/review.html`: yerelden açılabilen inceleme; iki mevcut tablo ve özgün vektör ton tablosu üzerinde 3×4 karşılaştırma, her tahtada 4×4 prosedürel parça. Hücre 40–76 pt; `?size=40` ve `?size=76` doğrudan sınır boyutlarını açar.
- `Assets/Art/PieceSkin/review-data.js`: file:// üzerinde fetch gerektirmeyen skin.json kopyası. JSON değişirse `window.PIECE_SKIN = <skin.json içeriği>;` olarak eşzamanlı güncellenir.
- `Assets/Art/PieceSkin/Review/tonal-study.svg`: açık krem, koyu petrol ve yoğun desen içeren üçüncü örnek tablo; oyun için yeni bölüm resmi değildir.
- `Assets/Art/PieceSkin/Review/review-{40,56,76}pt.png`: tarayıcıdan alınan görsel doğrulama kayıtları.

## Uygulama sırası

1. Prosedürel şekli gövde dokusuyla doldur; tabloyu tahta genelindeki sabit UV koordinatlarıyla %75 opaklıkta üzerine bindir. Doku Repeat; tablo Clamp. Her parçaya tabloyu yeniden sığdırma.
2. Çivili parçanın birleşik yüzeyine saturation=0,25 uygula, ardından siyahı %22 bindir. Kontur ve bevel bu dönüşümden sonra gelir.
3. Şekle kırpılmış 1,8 pt iç bevel: üst/sol sıcak highlight, alt/sağ mürekkep gölge. Kesintisiz 2,2 pt petrol konturu en üstte çiz. Ortak kenarlarda kalınlığı iki katına çıkarma; merkezlenmiş konturlar aynı sınırı paylaşır.
4. Seçili parçada altın 3 pt dış glow; koyu kontur korunur. Sürüklenen parça merkezinden ×1,06 büyür ve en son çizilir; gölge y-ofseti 5 pt olur. Normal gölge y=2 pt, blur=2,5 pt.
5. Çivili görünüm renk dışında ayrıca mevcut çivi ikonu/sayacıyla belirtilmelidir. İnceleme sayfasındaki küçük metal başlık yalnız temsildir; yeni çivi sprite'ı teslim edilmez.

İnceleme canvas'ı durumları aynı merkez parçada gösterir. Komşu kenarlar ters yönde aynı eğriyi izler; dış çerçeve düzdür. Skin, tırnak yönü veya parça geometrisi tanımlamaz. Oyun kodu, Unity import/entegrasyonu ve gerçek telefon testi bu teslimde yoktur.
