# Orijinal panorama — yalnızca 1. tablodan 2. tabloya

Yalnızca ayrı taslak. Oyun animasyonuna uygulanmadı.

## Teslim

- `panorama-single-step.mp4`: yaklaşık 3 saniye, 60 FPS, 780×1688. Bir kez sağdan sola kayma; bitişte durur. Döngü veya başa sarma birleşimi yoktur.
- `review-frames.png`: başlangıç / orta / bitiş kadrajları.
- `frame-start.png`, `frame-middle.png`, `frame-end.png`: ayrı, temiz 780×1688 kadrajlar.
- `index.html`: yerel HTTP sunucusunda tek oynatımlı taslak ve inceleme kaydırıcısı.
- `export-report.json`, `qa-report.json`, `qa/whole-move.png`: teknik doğrulama ve tam hareket incelemesi.

Tek görsel kaynak: `docs/art/drafts/corridor-victory-v2-panorama-20261030/panorama-clean.png` (1619×971). `original-panorama.png` yalnızca tarayıcıya sunmak için alınan, SHA256 ile birebir eşitliği doğrulanan kopyadır. Yeni çizim, imagegen, yeniden aydınlatma, dikiş eşleme, zemin düzeltme, panorama tekrarı veya reddedilen one-bay görseli kullanılmadı.

1. tablo merkezinin kaynak x koordinatı 313, hemen komşu 2. tablonun merkezi 809; toplam hareket **496 kaynak piksel**. Orta nokta x561: aradaki sütun/duvar. Üçüncü tablonun çerçevesi hareketin hiçbir anında kadraja girmez. Kadraj y=0, yükseklik 971, genişlik 971×390/844; bu boyutlar hareket boyunca değişmez. Tek dönüşüm sabit boyutlu kırpım penceresinin x konumudur. Çıkış görüntüsüne aynı oranda sabit ölçekleme yapılır; animasyonlu zoom, perspektif dönüşümü veya esnetme yoktur. Orijinal ışık, gölge ve çizimin perspektifi olduğu gibi kalır.

Quintic yumuşatma ile 3 saniyede bir kez ilerler. 181 örnek kare ve son tam kare dâhil MP4 dosya süresi 3,016667 saniyedir. İki farklı orijinal kadraj olduğu için uçlar farklıdır; eşleştirmek üzere düzenleme yapılmadı.

Kaynak panorama ve mevcut CorridorVictoryV2 PNG/MP4, runtime shader/texture dosyaları, Screens.cs, GameRoot.cs ve zafer kuyruğu önce/sonra SHA256 kontrolüyle korunmuştur. Tüm yeni dosyalar bu taslak klasöründedir. Sonsuz döngü önerisi bu teslimin kapsamına alınmadı.
