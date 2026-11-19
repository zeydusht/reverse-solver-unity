# Codex PM_89: dilimlenebilir altın tablo çerçevesi, 3 seçenek

## Sorun
Menüdeki ve koridordaki tablo çerçevesi arka plan resmine gömülü ve hep dar-dikey (açıklık ~0,6). Bölümlerin tahtası ise kare ya da kareye yakın (en-boy 0,8–1,0). Oyun çerçeveyi her bölümde tahtanın oranında kendisi çizecek. Bunun için düz (önden, perspektifsiz) bir çerçeve görseli gerekiyor; köşeleri ve süsleri sabit kalacak, aradaki düz pervaz uzayıp kısalacak.

Referanslar:
- Bugünkü çerçeve (koridordan, hafif perspektifli): `docs/art/pm89/cerceve/referans_cerceve.png`.
- Menü: `docs/art/pm89/cerceve/referans_menu.png`.
- Stil: barok altın, sıcak ışık, parlak ve doygun. Old money mavi duvarın (#0E4A55 civarı teal) üstünde durur.

## Parça (her seçenek)
`frame.png`, **1200×1200 şeffaf PNG**, önden düz görünüm.
- Açıklık (iç kenar) tam olarak **x 150–1050, y 150–1050** (900×900). Açıklığın içi tamamen şeffaf (alfa 0), çerçevenin iç dudağı bu çizgiye kadar gelir.
- Çerçevenin dışı da şeffaf; gölge **çizme** (gölgeyi oyun ekliyor).
- **Dilim düzeni** (oyun bu çizgilerden keser):
  - x ve y için: 0–330 **köşe** (sabit), 330–480 **düz pervaz** (uzar/kısalır), 480–720 **orta süs** (sabit), 720–870 **düz pervaz**, 870–1200 **köşe**.
  - 330–480 ve 720–870 aralıklarında pervaz kesiti **her satırda/sütunda birebir aynı** olmalı. Bu aralıkta süs, kıvrım, yaprak ya da ışık değişimi olmasın; yalnız boyuna düz bir profil. Böylece uzatılınca fark edilmez.
  - Üst ortada küçük bir taç/arma: 480–720 içinde, açıklığın üstüne taşabilir ama 0–150 bandında kalsın. Alt ortada daha küçük bir süs. Yan ortalarda küçük süs ya da düz (seçeneğe göre).
- Işık: sol üstten sıcak; her kenar kendi içinde tutarlı.
- Yazı yok, perspektif yok, arka plan yok.

## Seçenekler
- **A:** Bugünkü çerçevenin dili: kıvrımlı akantus köşeler, üstte zambak (fleur-de-lis) taç, altta küçük zambak; pervaz yuvarlak altın profil.
- **B:** Daha sade klasik: köşelerde rozet + küçük kıvrım, üstte istiridye/deniz kabuğu arma, pervaz çok katlı ince altın profil, en içte ince krem astar.
- **C:** Kraliyet: altın çerçevenin içinde ince **lacivert emaye** astar (açıklığa bitişik 18–24 px), köşelerde dolgun altın kıvrımlar, üstte küçük taç.

## Teslim
Her seçenek X ∈ {A, B, C} için:
- `docs/art/pm89/cerceve/X/frame.png`.
- `docs/art/pm89/cerceve/X/onizleme.png`, 1500×600: Python/PIL ile çerçeveyi dilimleyip üç açıklıkta (900×900, 720×900, 788×900) yan yana kur. Açıklığa `Assets/Art/Paintings/painting-05.jpg` resmini koy, zemin teal (#0E4A55).
- Kontroller: açıklık alfası 0; 330–480 ve 720–870 aralıklarında satır/sütun farkı (ortalama mutlak fark < 3/255); dış kenar temiz (kaba kenar yok).

Ayrıca:
- `docs/art/pm89/cerceve/karsilastirma.png`: üç önizleme alt alta.
- `docs/art/pm89/cerceve/LOG.md`: seçenek başına 1 satır (araç, istem özeti, kontrol sonuçları).

Mevcut dosyalara dokunma, oyuna bağlama yok, soru yok.
