# Task40 · Final görsel entegrasyon listesi

[Başlık önizlemesi](onizleme.png) · [Task39 #4 seçimi](../task39/flama_10.png)

Bu liste HUD, beş Jigo hâli, engeller, Süre doldu ve Kazanma için kullanılacak **117 PNG’yi** ve iki mevcut yazı tipini dosya bazında verir. Dosya yolları depo köküne göredir.

## Oyun HUD’u

390 × 844 pt yerleşim için 3 px = 1 pt. Oyun başlık sprite’ı 364 × 166 px. Parça ve Süre x merkezleri 201 ve 972 px, tuval üstü y=123 px. Canlı başlık metninin merkezi y=186 px, görünür harf yüksekliği 38 px. Krem sayaç gövdelerinin sol üstü (31, 181) ve (802, 181) px. Başlık kutusu ve flamalar tek saydam sprite. Booster ikonları 195 × 195 px tuvalde, (97, 97) px pivotla merkezlenir. Rozet üst katmana gelir. Sıcak kahverengi saat, Task36’dan beri ana provada kullanılan A tonudur.

| Dosya yolu | Kullanım |
|---|---|
| [Assets/Art/_yeni/HUD40/Headers/game.png](../../../Assets/Art/_yeni/HUD40/Headers/game.png) | Parça ve Süre için ortak krem başlık ve dışa bakan flama çifti. Yazı canlı. |
| [Assets/Art/_yeni/HUD32/top-strip.png](../../../Assets/Art/_yeni/HUD32/top-strip.png) | M1 üst şerit ve altın bordür. |
| [Assets/Art/_yeni/HUD32/bottom-strip-a.png](../../../Assets/Art/_yeni/HUD32/bottom-strip-a.png) | A yönündeki alt şerit gradyanı. |
| [Assets/Art/_yeni/HUD32/counter-panel.png](../../../Assets/Art/_yeni/HUD32/counter-panel.png) | Parça ve Süre sayaçlarının krem gövdesi. |
| [Assets/Art/_yeni/HUD31/icons/icon-piece.png](../../../Assets/Art/_yeni/HUD31/icons/icon-piece.png) | Parça sayacının yapboz işareti. |
| [Assets/Art/_yeni/HUD31/m1/portrait-window.png](../../../Assets/Art/_yeni/HUD31/m1/portrait-window.png) | Jigo’nun kemerli altın penceresi. |
| [Assets/Art/_yeni/HUD31/shared/portrait-mask.png](../../../Assets/Art/_yeni/HUD31/shared/portrait-mask.png) | Portreyi pencerenin içine kırpan alfa maskesi. |
| [Assets/Art/_yeni/HUD31/m1/socket.png](../../../Assets/Art/_yeni/HUD31/m1/socket.png) | Alt sıradaki tuşların mavi yuvası. |
| [Assets/Art/_yeni/HUD31/shared/booster-normal.png](../../../Assets/Art/_yeni/HUD31/shared/booster-normal.png) | Yeşil booster gövdesi · normal. |
| [Assets/Art/_yeni/HUD31/shared/booster-pressed.png](../../../Assets/Art/_yeni/HUD31/shared/booster-pressed.png) | Yeşil booster gövdesi · basılı. |
| [Assets/Art/_yeni/HUD31/shared/booster-locked.png](../../../Assets/Art/_yeni/HUD31/shared/booster-locked.png) | Yeşil booster gövdesi · kilitli. |
| [Assets/Art/_yeni/HUD31/shared/booster-zero.png](../../../Assets/Art/_yeni/HUD31/shared/booster-zero.png) | Yeşil booster gövdesi · tükenmiş. |
| [Assets/Art/_yeni/HUD31/shared/badge.png](../../../Assets/Art/_yeni/HUD31/shared/badge.png) | Canlı booster adedi için boş kırmızı rozet. |
| [Assets/Art/_yeni/HUD31/shared/locked-icon.png](../../../Assets/Art/_yeni/HUD31/shared/locked-icon.png) | Kilitli booster durumunun küçük kilit işareti. |
| [Assets/Art/_yeni/HUD36/Boosters/scissors.png](../../../Assets/Art/_yeni/HUD36/Boosters/scissors.png) | Makas · final boyut ve disk içi yerleşim. |
| [Assets/Art/_yeni/HUD36/Boosters/wand.png](../../../Assets/Art/_yeni/HUD36/Boosters/wand.png) | Yıldızlı değnek · final boyut ve disk içi yerleşim. |
| [Assets/Art/_yeni/HUD36/Boosters/hammer.png](../../../Assets/Art/_yeni/HUD36/Boosters/hammer.png) | Mavi çekiç · final boyut ve disk içi yerleşim. |
| [Assets/Art/_yeni/HUD36/Boosters/clock-a.png](../../../Assets/Art/_yeni/HUD36/Boosters/clock-a.png) | Sıcak kahverengi kasalı saat · Task36 ana prova seçeneği. |
| [Assets/Art/_yeni/HUD31/shared/settings-normal.png](../../../Assets/Art/_yeni/HUD31/shared/settings-normal.png) | Mavi Durdur gövdesi · normal. |
| [Assets/Art/_yeni/HUD31/shared/settings-pressed.png](../../../Assets/Art/_yeni/HUD31/shared/settings-pressed.png) | Mavi Durdur gövdesi · basılı. |
| [Assets/Art/_yeni/HUD31/shared/settings-locked.png](../../../Assets/Art/_yeni/HUD31/shared/settings-locked.png) | Mavi Durdur gövdesi · kilitli. |
| [Assets/Art/_yeni/HUD31/shared/settings-zero.png](../../../Assets/Art/_yeni/HUD31/shared/settings-zero.png) | Mavi Durdur gövdesi · tükenmiş. |
| [Assets/Art/_yeni/HUD32/pause-icon.png](../../../Assets/Art/_yeni/HUD32/pause-icon.png) | Mavi Durdur tuşunun iki krem çizgisi. |

## Jigo portre kareleri

Portreler 256 × 256 px. Her dizi 01’den 06’ya oynar. Normal dizi Task32’nin kapalı gülüşünü kullanır. Tedirgin hâl süre ≤10 sn, korku süre ≤3 sn veya bomba 1, üzgün hâl kayıp, mutlu hâl parça çıkarma tepkisidir.

| Dosya yolu | Kullanım |
|---|---|
| [Assets/Art/_yeni/HUD32/jigo/normal/01.png](../../../Assets/Art/_yeni/HUD32/jigo/normal/01.png) | Normal portre · kare 01. |
| [Assets/Art/_yeni/HUD32/jigo/normal/02.png](../../../Assets/Art/_yeni/HUD32/jigo/normal/02.png) | Normal portre · kare 02. |
| [Assets/Art/_yeni/HUD32/jigo/normal/03.png](../../../Assets/Art/_yeni/HUD32/jigo/normal/03.png) | Normal portre · kare 03. |
| [Assets/Art/_yeni/HUD32/jigo/normal/04.png](../../../Assets/Art/_yeni/HUD32/jigo/normal/04.png) | Normal portre · kare 04 · göz kırpma. |
| [Assets/Art/_yeni/HUD32/jigo/normal/05.png](../../../Assets/Art/_yeni/HUD32/jigo/normal/05.png) | Normal portre · kare 05. |
| [Assets/Art/_yeni/HUD32/jigo/normal/06.png](../../../Assets/Art/_yeni/HUD32/jigo/normal/06.png) | Normal portre · kare 06. |
| [Assets/Art/_yeni/Jigo_v3/tedirgin/01.png](../../../Assets/Art/_yeni/Jigo_v3/tedirgin/01.png) | Tedirgin portre · kare 01. |
| [Assets/Art/_yeni/Jigo_v3/tedirgin/02.png](../../../Assets/Art/_yeni/Jigo_v3/tedirgin/02.png) | Tedirgin portre · kare 02. |
| [Assets/Art/_yeni/Jigo_v3/tedirgin/03.png](../../../Assets/Art/_yeni/Jigo_v3/tedirgin/03.png) | Tedirgin portre · kare 03. |
| [Assets/Art/_yeni/Jigo_v3/tedirgin/04.png](../../../Assets/Art/_yeni/Jigo_v3/tedirgin/04.png) | Tedirgin portre · kare 04. |
| [Assets/Art/_yeni/Jigo_v3/tedirgin/05.png](../../../Assets/Art/_yeni/Jigo_v3/tedirgin/05.png) | Tedirgin portre · kare 05. |
| [Assets/Art/_yeni/Jigo_v3/tedirgin/06.png](../../../Assets/Art/_yeni/Jigo_v3/tedirgin/06.png) | Tedirgin portre · kare 06. |
| [Assets/Art/_yeni/Jigo_v3/korku/01.png](../../../Assets/Art/_yeni/Jigo_v3/korku/01.png) | Korku portre · kare 01. |
| [Assets/Art/_yeni/Jigo_v3/korku/02.png](../../../Assets/Art/_yeni/Jigo_v3/korku/02.png) | Korku portre · kare 02. |
| [Assets/Art/_yeni/Jigo_v3/korku/03.png](../../../Assets/Art/_yeni/Jigo_v3/korku/03.png) | Korku portre · kare 03. |
| [Assets/Art/_yeni/Jigo_v3/korku/04.png](../../../Assets/Art/_yeni/Jigo_v3/korku/04.png) | Korku portre · kare 04. |
| [Assets/Art/_yeni/Jigo_v3/korku/05.png](../../../Assets/Art/_yeni/Jigo_v3/korku/05.png) | Korku portre · kare 05. |
| [Assets/Art/_yeni/Jigo_v3/korku/06.png](../../../Assets/Art/_yeni/Jigo_v3/korku/06.png) | Korku portre · kare 06. |
| [Assets/Art/_yeni/Jigo_v3/uzgun/01.png](../../../Assets/Art/_yeni/Jigo_v3/uzgun/01.png) | Üzgün portre · kare 01. |
| [Assets/Art/_yeni/Jigo_v3/uzgun/02.png](../../../Assets/Art/_yeni/Jigo_v3/uzgun/02.png) | Üzgün portre · kare 02 · Süre doldu panelinde kullanılan kare. |
| [Assets/Art/_yeni/Jigo_v3/uzgun/03.png](../../../Assets/Art/_yeni/Jigo_v3/uzgun/03.png) | Üzgün portre · kare 03. |
| [Assets/Art/_yeni/Jigo_v3/uzgun/04.png](../../../Assets/Art/_yeni/Jigo_v3/uzgun/04.png) | Üzgün portre · kare 04. |
| [Assets/Art/_yeni/Jigo_v3/uzgun/05.png](../../../Assets/Art/_yeni/Jigo_v3/uzgun/05.png) | Üzgün portre · kare 05. |
| [Assets/Art/_yeni/Jigo_v3/uzgun/06.png](../../../Assets/Art/_yeni/Jigo_v3/uzgun/06.png) | Üzgün portre · kare 06. |
| [Assets/Art/_yeni/Jigo_v3/mutlu/01.png](../../../Assets/Art/_yeni/Jigo_v3/mutlu/01.png) | Mutlu portre · kare 01. |
| [Assets/Art/_yeni/Jigo_v3/mutlu/02.png](../../../Assets/Art/_yeni/Jigo_v3/mutlu/02.png) | Mutlu portre · kare 02. |
| [Assets/Art/_yeni/Jigo_v3/mutlu/03.png](../../../Assets/Art/_yeni/Jigo_v3/mutlu/03.png) | Mutlu portre · kare 03. |
| [Assets/Art/_yeni/Jigo_v3/mutlu/04.png](../../../Assets/Art/_yeni/Jigo_v3/mutlu/04.png) | Mutlu portre · kare 04. |
| [Assets/Art/_yeni/Jigo_v3/mutlu/05.png](../../../Assets/Art/_yeni/Jigo_v3/mutlu/05.png) | Mutlu portre · kare 05. |
| [Assets/Art/_yeni/Jigo_v3/mutlu/06.png](../../../Assets/Art/_yeni/Jigo_v3/mutlu/06.png) | Mutlu portre · kare 06. |

## Engeller

Kilit B’nin görünür uzun kenarı hücre boyunun yaklaşık %55’i. Açık 04 karesi düşüş animasyonunun görsel kaynağı. Zincir ve ankraj renk eşleri aynı grup adıyla kullanılır. Bomba gövdesi 384 × 384 px, kıvılcım 576 × 576 px. Gövde, kıvılcım tuvaline (96, 96) px konumundan oturur. Etiket ve sayı ayrı katmandır.

| Dosya yolu | Kullanım |
|---|---|
| [Assets/Art/_yeni/Obstacles_v3/padlock-b/closed.png](../../../Assets/Art/_yeni/Obstacles_v3/padlock-b/closed.png) | Kont’un B kilidi · kapalı gövde ve canlı sayı zemini. |
| [Assets/Art/_yeni/Obstacles_v3/padlock-b/open-01.png](../../../Assets/Art/_yeni/Obstacles_v3/padlock-b/open-01.png) | B kilidi açılışı · kare 01. |
| [Assets/Art/_yeni/Obstacles_v3/padlock-b/open-02.png](../../../Assets/Art/_yeni/Obstacles_v3/padlock-b/open-02.png) | B kilidi açılışı · kare 02. |
| [Assets/Art/_yeni/Obstacles_v3/padlock-b/open-03.png](../../../Assets/Art/_yeni/Obstacles_v3/padlock-b/open-03.png) | B kilidi açılışı · kare 03. |
| [Assets/Art/_yeni/Obstacles_v3/padlock-b/open-04.png](../../../Assets/Art/_yeni/Obstacles_v3/padlock-b/open-04.png) | B kilidi açılışı · kare 04 · düşüş için açık sprite. |
| [Assets/Art/_yeni/Obstacles_v3/padlock-b/wax-01.png](../../../Assets/Art/_yeni/Obstacles_v3/padlock-b/wax-01.png) | Mum kırıntısı efekti · kare 01. |
| [Assets/Art/_yeni/Obstacles_v3/padlock-b/wax-02.png](../../../Assets/Art/_yeni/Obstacles_v3/padlock-b/wax-02.png) | Mum kırıntısı efekti · kare 02. |
| [Assets/Art/_yeni/Obstacles_v3/padlock-b/wax-03.png](../../../Assets/Art/_yeni/Obstacles_v3/padlock-b/wax-03.png) | Mum kırıntısı efekti · kare 03. |
| [Assets/Art/_yeni/Obstacles_v3/chain/chain-link-altin.png](../../../Assets/Art/_yeni/Obstacles_v3/chain/chain-link-altin.png) | Zincir halkası · altın. |
| [Assets/Art/_yeni/Obstacles_v3/chain/chain-anchor-altin.png](../../../Assets/Art/_yeni/Obstacles_v3/chain/chain-anchor-altin.png) | Zincir ankrajı · altın · yıldız. |
| [Assets/Art/_yeni/Obstacles_v3/chain/chain-link-turkuaz.png](../../../Assets/Art/_yeni/Obstacles_v3/chain/chain-link-turkuaz.png) | Zincir halkası · turkuaz. |
| [Assets/Art/_yeni/Obstacles_v3/chain/chain-anchor-turkuaz.png](../../../Assets/Art/_yeni/Obstacles_v3/chain/chain-anchor-turkuaz.png) | Zincir ankrajı · turkuaz · damla. |
| [Assets/Art/_yeni/Obstacles_v3/chain/chain-link-mor.png](../../../Assets/Art/_yeni/Obstacles_v3/chain/chain-link-mor.png) | Zincir halkası · mor. |
| [Assets/Art/_yeni/Obstacles_v3/chain/chain-anchor-mor.png](../../../Assets/Art/_yeni/Obstacles_v3/chain/chain-anchor-mor.png) | Zincir ankrajı · mor · hilal. |
| [Assets/Art/_yeni/Obstacles_v3/chain/chain-link-mercan.png](../../../Assets/Art/_yeni/Obstacles_v3/chain/chain-link-mercan.png) | Zincir halkası · mercan. |
| [Assets/Art/_yeni/Obstacles_v3/chain/chain-anchor-mercan.png](../../../Assets/Art/_yeni/Obstacles_v3/chain/chain-anchor-mercan.png) | Zincir ankrajı · mercan · kalp. |
| [Assets/Art/_yeni/Obstacles_v3/chain/chain-link-yesil.png](../../../Assets/Art/_yeni/Obstacles_v3/chain/chain-link-yesil.png) | Zincir halkası · yeşil. |
| [Assets/Art/_yeni/Obstacles_v3/chain/chain-anchor-yesil.png](../../../Assets/Art/_yeni/Obstacles_v3/chain/chain-anchor-yesil.png) | Zincir ankrajı · yeşil · yaprak. |
| [Assets/Art/_yeni/Obstacles/chain-break-01.png](../../../Assets/Art/_yeni/Obstacles/chain-break-01.png) | Zincir kopuşu · mevcut efekt karesi 01. |
| [Assets/Art/_yeni/Obstacles/chain-break-02.png](../../../Assets/Art/_yeni/Obstacles/chain-break-02.png) | Zincir kopuşu · mevcut efekt karesi 02. |
| [Assets/Art/_yeni/Obstacles/chain-break-03.png](../../../Assets/Art/_yeni/Obstacles/chain-break-03.png) | Zincir kopuşu · mevcut efekt karesi 03. |
| [Assets/Art/_yeni/Obstacles/chain-break-04.png](../../../Assets/Art/_yeni/Obstacles/chain-break-04.png) | Zincir kopuşu · mevcut efekt karesi 04. |
| [Assets/Art/_yeni/Obstacles/bomb-v2.png](../../../Assets/Art/_yeni/Obstacles/bomb-v2.png) | Normal siyah bomba gövdesi. |
| [Assets/Art/_yeni/Obstacles/bomb-v2-urgent.png](../../../Assets/Art/_yeni/Obstacles/bomb-v2-urgent.png) | 3–2–1 için sıcak parlamalı bomba gövdesi. |
| [Assets/Art/_yeni/Obstacles/bomb-counter-label.png](../../../Assets/Art/_yeni/Obstacles/bomb-counter-label.png) | Bombanın canlı sayısı için boş krem etiket. |
| [Assets/Art/_yeni/Obstacles_v2/bomb/normal/spark-01.png](../../../Assets/Art/_yeni/Obstacles_v2/bomb/normal/spark-01.png) | Normal kıvılcım döngüsü · kare 01. |
| [Assets/Art/_yeni/Obstacles_v2/bomb/normal/spark-02.png](../../../Assets/Art/_yeni/Obstacles_v2/bomb/normal/spark-02.png) | Normal kıvılcım döngüsü · kare 02. |
| [Assets/Art/_yeni/Obstacles_v2/bomb/normal/spark-03.png](../../../Assets/Art/_yeni/Obstacles_v2/bomb/normal/spark-03.png) | Normal kıvılcım döngüsü · kare 03. |
| [Assets/Art/_yeni/Obstacles_v2/bomb/normal/spark-04.png](../../../Assets/Art/_yeni/Obstacles_v2/bomb/normal/spark-04.png) | Normal kıvılcım döngüsü · kare 04. |
| [Assets/Art/_yeni/Obstacles_v2/bomb/normal/spark-05.png](../../../Assets/Art/_yeni/Obstacles_v2/bomb/normal/spark-05.png) | Normal kıvılcım döngüsü · kare 05. |
| [Assets/Art/_yeni/Obstacles_v2/bomb/normal/spark-06.png](../../../Assets/Art/_yeni/Obstacles_v2/bomb/normal/spark-06.png) | Normal kıvılcım döngüsü · kare 06. |
| [Assets/Art/_yeni/Obstacles_v2/bomb/normal/spark-07.png](../../../Assets/Art/_yeni/Obstacles_v2/bomb/normal/spark-07.png) | Normal kıvılcım döngüsü · kare 07. |
| [Assets/Art/_yeni/Obstacles_v2/bomb/normal/spark-08.png](../../../Assets/Art/_yeni/Obstacles_v2/bomb/normal/spark-08.png) | Normal kıvılcım döngüsü · kare 08. |
| [Assets/Art/_yeni/Obstacles_v2/bomb/urgent/spark-01.png](../../../Assets/Art/_yeni/Obstacles_v2/bomb/urgent/spark-01.png) | 3–2–1 için acil kıvılcım döngüsü · kare 01. |
| [Assets/Art/_yeni/Obstacles_v2/bomb/urgent/spark-02.png](../../../Assets/Art/_yeni/Obstacles_v2/bomb/urgent/spark-02.png) | 3–2–1 için acil kıvılcım döngüsü · kare 02. |
| [Assets/Art/_yeni/Obstacles_v2/bomb/urgent/spark-03.png](../../../Assets/Art/_yeni/Obstacles_v2/bomb/urgent/spark-03.png) | 3–2–1 için acil kıvılcım döngüsü · kare 03. |
| [Assets/Art/_yeni/Obstacles_v2/bomb/urgent/spark-04.png](../../../Assets/Art/_yeni/Obstacles_v2/bomb/urgent/spark-04.png) | 3–2–1 için acil kıvılcım döngüsü · kare 04. |
| [Assets/Art/_yeni/Obstacles_v2/bomb/urgent/spark-05.png](../../../Assets/Art/_yeni/Obstacles_v2/bomb/urgent/spark-05.png) | 3–2–1 için acil kıvılcım döngüsü · kare 05. |
| [Assets/Art/_yeni/Obstacles_v2/bomb/urgent/spark-06.png](../../../Assets/Art/_yeni/Obstacles_v2/bomb/urgent/spark-06.png) | 3–2–1 için acil kıvılcım döngüsü · kare 06. |
| [Assets/Art/_yeni/Obstacles_v2/bomb/urgent/spark-07.png](../../../Assets/Art/_yeni/Obstacles_v2/bomb/urgent/spark-07.png) | 3–2–1 için acil kıvılcım döngüsü · kare 07. |
| [Assets/Art/_yeni/Obstacles_v2/bomb/urgent/spark-08.png](../../../Assets/Art/_yeni/Obstacles_v2/bomb/urgent/spark-08.png) | 3–2–1 için acil kıvılcım döngüsü · kare 08. |

## Süre doldu paneli

Başlık sprite’ı 704 × 166 px, x merkezi 585 px ve tuval üstü y=478 px. Canlı başlığın merkezi (585, 554) px, görünür harf yüksekliği 47 px. Krem panel tuvali (87, 542) px’te. Üzgün portre yukarıdaki diziden kullanılır. +30 sn, 900 ve başlık canlı metindir.

| Dosya yolu | Kullanım |
|---|---|
| [Assets/Art/_yeni/HUD40/Headers/continue.png](../../../Assets/Art/_yeni/HUD40/Headers/continue.png) | Süre doldu başlığı ve Parça/Süre ile aynı flama çifti. Yazı canlı. |
| [Assets/Art/_yeni/Devam_v2/panel.png](../../../Assets/Art/_yeni/Devam_v2/panel.png) | Krem modal gövdesi. |
| [Assets/Art/_yeni/Devam_v2/close-button.png](../../../Assets/Art/_yeni/Devam_v2/close-button.png) | Sağ üstteki kırmızı X tuşunun gövdesi. |
| [Assets/Art/_yeni/Devam_v2/icon-close.png](../../../Assets/Art/_yeni/Devam_v2/icon-close.png) | Kırmızı tuş üzerindeki krem X. |
| [Assets/Art/_yeni/Devam_v2/continue-clock.png](../../../Assets/Art/_yeni/Devam_v2/continue-clock.png) | +30 sn metninin yanındaki küçük saat. |
| [Assets/Art/_yeni/Devam_v2/continue-coin.png](../../../Assets/Art/_yeni/Devam_v2/continue-coin.png) | Yeşil satın al tuşundaki altın para. |
| [Assets/Art/_yeni/Devam_v2/continue-primary.png](../../../Assets/Art/_yeni/Devam_v2/continue-primary.png) | Süre satın alma ve Kazanma provasında kullanılan yeşil Devam tuşu. |

## Kazanma ve Jigo sevinç

Tam boy sevinç 640 × 640 px, 01–08 sıralı. Işık huzmesi ve konfeti, karakterden ayrı katmanlardır. Galeri arka planı ve yeşil Devam tuşu ilgili satırlarda listelenmiştir.

| Dosya yolu | Kullanım |
|---|---|
| [Assets/Art/_yeni/Jigo/sevinc/01.png](../../../Assets/Art/_yeni/Jigo/sevinc/01.png) | Tam boy sevinç animasyonu · kare 01. |
| [Assets/Art/_yeni/Jigo/sevinc/02.png](../../../Assets/Art/_yeni/Jigo/sevinc/02.png) | Tam boy sevinç animasyonu · kare 02. |
| [Assets/Art/_yeni/Jigo/sevinc/03.png](../../../Assets/Art/_yeni/Jigo/sevinc/03.png) | Tam boy sevinç animasyonu · kare 03. |
| [Assets/Art/_yeni/Jigo/sevinc/04.png](../../../Assets/Art/_yeni/Jigo/sevinc/04.png) | Tam boy sevinç animasyonu · kare 04. |
| [Assets/Art/_yeni/Jigo/sevinc/05.png](../../../Assets/Art/_yeni/Jigo/sevinc/05.png) | Tam boy sevinç animasyonu · kare 05. |
| [Assets/Art/_yeni/Jigo/sevinc/06.png](../../../Assets/Art/_yeni/Jigo/sevinc/06.png) | Tam boy sevinç animasyonu · kare 06. |
| [Assets/Art/_yeni/Jigo/sevinc/07.png](../../../Assets/Art/_yeni/Jigo/sevinc/07.png) | Tam boy sevinç animasyonu · kare 07. |
| [Assets/Art/_yeni/Jigo/sevinc/08.png](../../../Assets/Art/_yeni/Jigo/sevinc/08.png) | Tam boy sevinç animasyonu · kare 08. |
| [Assets/Art/_yeni/Jigo/sevinc/fx/light-rays.png](../../../Assets/Art/_yeni/Jigo/sevinc/fx/light-rays.png) | Jigo’nun arkasındaki bağımsız altın ışık huzmesi. |
| [Assets/Art/_yeni/Jigo/sevinc/fx/confetti-coral.png](../../../Assets/Art/_yeni/Jigo/sevinc/fx/confetti-coral.png) | Mercan konfeti parçacığı. |
| [Assets/Art/_yeni/Jigo/sevinc/fx/confetti-gold.png](../../../Assets/Art/_yeni/Jigo/sevinc/fx/confetti-gold.png) | Altın konfeti parçacığı. |
| [Assets/Art/_yeni/Jigo/sevinc/fx/confetti-purple.png](../../../Assets/Art/_yeni/Jigo/sevinc/fx/confetti-purple.png) | Mor konfeti parçacığı. |
| [Assets/Art/_yeni/Jigo/sevinc/fx/confetti-teal.png](../../../Assets/Art/_yeni/Jigo/sevinc/fx/confetti-teal.png) | Turkuaz konfeti parçacığı. |

## Mevcut ortak kaynaklar

Aşağıdakiler yeniden çizim gerektirmeden mevcut dosya yollarıyla kullanılır.

| Dosya yolu | Kullanım |
|---|---|
| [Assets/_Game/Presentation/Resources/Art/game-background.png](../../../Assets/_Game/Presentation/Resources/Art/game-background.png) | Oyun ve Kazanma için mevcut galeri arka planı. |
| [Assets/_Game/Presentation/Resources/Art/board-frame.png](../../../Assets/_Game/Presentation/Resources/Art/board-frame.png) | Mevcut oyun tahtası çerçevesi. |
| [Assets/_Game/Presentation/Resources/Art/wall-stripe.png](../../../Assets/_Game/Presentation/Resources/Art/wall-stripe.png) | Mevcut duvar şeridi. |
| [Assets/_Game/Presentation/Resources/Fonts/Baloo2-ExtraBold.ttf](../../../Assets/_Game/Presentation/Resources/Fonts/Baloo2-ExtraBold.ttf) | Başlıkların ve modal etiketlerinin canlı yazısı. |
| [Assets/_Game/Presentation/Resources/Fonts/LilitaOne-Regular.ttf](../../../Assets/_Game/Presentation/Resources/Fonts/LilitaOne-Regular.ttf) | Sayaç ve adet rakamlarının canlı yazısı. |

## Final sürüm eşlemesi

- HUD31 ve HUD32 üstteki satırlarda adı geçen gövde, şerit, sayaç, maske ve ikonları sağlar.
- HUD33, HUD34 ve HUD35 üretim aşamalarıdır. Güncel booster boya ve yerleşimi HUD36 sprite’larında, HUD35 krem kutuları HUD40 başlık sprite’larının içinde.
- HUD37’nin dış köşe flaması ile Task38–39 seçenek paftaları arşiv/prova malzemesidir. Final başlık dosyaları HUD40’taki iki satırdır.
- Task40 görsel teslimdir. Yukarıdaki yollar entegrasyonda kullanılacak kaynakları gösterir.
