# CODEX PM74 · Saat booster'ı: 3 fikir + parçalar

## Oyun ve sorun
Reverse Solver: yapboz parçası çıkarma bulmacası, dikey telefon (390×844 pt, görseller @3x). Saat booster'ı **süreyi 10 saniye durdurur** (kural bu, değişmiyor). Şu anki animasyonda Jigo'nun kolu düğmesini Süre kutusuna uzatıyor; oyuncu ne olduğunu anlamıyor ("inanılmaz kötü").

Hedef: tek bakışta "süreme 10 saniye eklendi / süre durdu" okunsun, sevimli olsun, toplam ≤ 1,5 sn, sonda yumuşak yavaşlama.

Bakılacak dosyalar:
- Oyun ekranı: `docs/art/pm74/once/11_bolum20.jpg` (üstte Parça kutusu, ortada kemerli pencerede Jigo, sağda **Süre** kutusu; altta booster çubuğu: makas, değnek, çekiç, **saat**, durdur).
- Saat ikonu (bu tasarımı koru, daha ayrıntılı çiz): `Assets/_Game/Presentation/Resources/Art/yeni/boosters21/Clock/Layers/flying-clock.png`.
- Onaylı tarz: old money mavi, krem panel + mavi çerçeve, altın, kalın 3B yazı, **kurdele yok**, opak ve doygun renkler. Royal Match kalitesi (`docs/art/booster ref/kareler/BOOSTER_REF_ANALIZ.md`).

## 1. Üç fikir (storyboard)
Her fikir için **tek PNG**, 5 kare yan yana (her kare 390×844'ün üst yarısı: 390×420, toplam 1950×420 px; @1x yeterli), karelerin altına süre damgası (0,0 / 0,3 / 0,6 / 0,9 / 1,3 sn). Fikirler birbirinden **gerçekten farklı** olsun. Öneriler (daha iyisini bulursan değiştir):
- **A · Uçan çalar saat:** saat booster tuşundan kavisle Süre kutusuna uçar (büyüyerek), kutunun üstünde zıplayıp çalar (çanlar titrer), kutu buzlu maviye döner, "+10 sn" rozeti kutunun altına oturur ve geri sayar.
- **B · Geri saran kadran:** büyük saat ekranın üst ortasında belirir, akrep ve yelkovan hızla geri döner (ışıltılı iz), saat küçülerek Süre kutusuna dalar, kutuda "+10" altın sayı belirir.
- **C · Kum saati / donma dalgası:** saat kutuya uçar, değdiği anda buz kristali dalgası kutudan dışa yayılır, rakamlar buzla kaplanır, küçük kar taneleri dökülür.
Kaydet: `docs/art/pm74/booster/saat/fikir_A.png`, `fikir_B.png`, `fikir_C.png`.

## 2. Parçalar (şeffaf RGBA PNG, @3x, kenarları temiz, gölge yok)
Klasör: `Assets/Art/_yeni/PM74/saat/`
- `clock_big.png` 600×600: çalar saat önden, `flying-clock.png` ile aynı tasarım (kahverengi gövde, altın çanlar, krem kadran) ama **akrep ve yelkovan YOK** (boş kadran, ortada altın göbek).
- `hand_long.png` 60×300 ve `hand_short.png` 60×220: koyu kahve ibreler, dönme noktası görselin **alt ortasında** (alt uçta 30 px'lik yuvarlak göbek), dikey duruyor.
- `bells_ring_01.png` … `03.png` 600×600: yalnız iki çan + çekiç, çalarken üç poz (sola, orta, sağa eğik); `clock_big` ile üst üste konunca tam otursun.
- `frost_burst_01.png` … `06.png` 512×512: buz mavisi kristal patlaması (ortadan dışa büyüyen halka + kristal çizgiler), 06 neredeyse kaybolmuş.
- `snowflake.png` 96×96: beyaz-buz mavisi kar tanesi.
- `ice_glaze.png` 340×290: Süre kutusunun (krem panel, mavi çerçeve, 113×97 pt) üstüne binecek yarı saydam buz camı: kenarlarda kristal, ortası açık (rakam okunmalı).
- `glow_ring.png` 512×512: altın-sarı yumuşak ışık halkası (ortası boş).

## 3. Önizleme
`docs/art/pm74/booster/saat/parcalar.png`: bütün parçaların koyu mavi zemin üstünde pafta görüntüsü.

## Kurallar
- Kod, sahne, `Resources/` ya da başka dosyaya DOKUNMA. Git commit yapma.
- Bitince en fazla 5 satır kısa özet yaz.
