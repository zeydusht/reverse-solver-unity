# Codex Task51 — Tablolar 41–100 (60 yeni tablo, mevcut stil)

Bu görev tek seferde bir parti içindir. Komut hangi partiyi istiyorsa (ör. "41–50") yalnız onu yap.

## Stil
- Mevcut `Assets/Art/Paintings_v2/painting-01..40.jpg` ile aynı stil: sade, tek odaklı, animasyon filmi konsepti, sıcak ışık, okunaklı orta ölçekli biçimler.
- Referans: önce `painting-05.jpg`, `painting-17.jpg`, `painting-26.jpg`, `painting-35.jpg` dosyalarına bak. Görsel üretim aracını bunlardan biriyle (stil referansı) kullan.
- Çizgi, çerçeve, yazı, logo, insan yüzü yok. 1–2 küçük Jiggle olabilir (mevcut tablolardaki gibi küçük, sevimli, yuvarlak yaratık).
- Tablolar yapboz olarak 5×5 … 8×8 kare parçalara bölünecek. **Her parçada ayırt edilebilir bir biçim ya da renk olsun.** Büyük düz gökyüzü, düz su, düz kar alanı yok; boş alanlara doku, renk geçişi ve ikincil biçimler (bulut kümeleri, taşlar, yapraklar, fenerler) koy.
- Aynı salondaki 10 tablo birbirinden farklı konu, farklı bakış açısı ve farklı baskın renk taşısın.

## Dosyalar (yalnız bunlara yaz)
- `Assets/Art/Paintings_v2/painting-NN.jpg`: 1024×1024 RGB JPG, en çok 300.000 bayt (gerekirse kaliteyi düşür ya da çok hafif ön filtre uygula).
- `Assets/Art/Paintings_v2/Source/painting-NN.png`: kaynak (en az 1024×1024).
- `Assets/Art/Paintings_v2/manifest-v2.json`: yeni tabloları ekle (mevcut kayıtlara dokunma; her kayıtta no, TR/EN ad, boyut, bayt, SHA-256).
- `Assets/Art/Paintings_v2/once-sonra-NN-MM.png`: partinin 10 tablosu yan yana küçük önizleme; altında her birinin 6×6 kesilmiş hâli.
- `docs/art/T51_LOG.md`: partinin sonuna 3–5 satır (ne bitti, sorun).
- Mevcut dosyaları silme, mevcut tabloları değiştirme, oyuna bağlama yok, soru yok.

## Salonlar ve konular

### 41–50 · Çöl ve vaha salonu (kum, turkuaz su, terakota, sıcak öğle ışığı)
41 Vaha kuyusu (palmiye altında taş kuyu) · 42 Kervan çadırı (desenli çadır, minderler) · 43 Kum tepesi feneri (dünlerin arasında tek fener kulesi) · 44 Baharat pazarı tezgâhı (renkli baharat tepeleri) · 45 Kaktüs bahçesi (çiçek açmış kaktüsler, saksılar) · 46 Mozaik çeşme (mavi çinili avlu çeşmesi) · 47 Uçan halı dükkânı (asılı halılar, biri hafifçe süzülüyor) · 48 Kayalık kemer (kızıl kaya kemeri, uzakta vaha) · 49 Hurma hasadı (salkımlar, sepetler) · 50 Çöl gözlemevi (kubbeli küçük gözlemevi, akşamüstü).

### 51–60 · Deniz altı ve mercan salonu (turkuaz, mercan pembe, altın ışık hüzmeleri)
51 Mercan bahçesi · 52 Batık gemi pruvası (dost, renkli, yosunlu) · 53 Deniz feneri balığı mağarası (ışıklı küçük balıklar) · 54 İstiridye incisi (açık istiridyede inci) · 55 Deniz kaplumbağası yolu · 56 Deniz yosunu ormanı · 57 Denizaltı (sarı, yuvarlak lombozlu) · 58 Kum kalesi resifi (deniz altında kum şatosu) · 59 Denizanası balosu (pastel denizanaları) · 60 Hazine sandığı (yarı açık, altın paralar, mercanlar).

### 61–70 · Sonbahar ve hasat salonu (turuncu, bordo, altın sarısı)
61 Balkabağı tarlası · 62 Elma şırası değirmeni · 63 Yaprak yağmuru altında bank · 64 Korkuluk ve kargalar (sevimli) · 65 Ambar kapısı (açık, içinde saman) · 66 Mantar toplama sepeti · 67 Sonbahar gölü kayıkhanesi · 68 Ceviz ağacı salıncağı · 69 Hasat sofrası (dışarıda uzun masa) · 70 Sisli bağ evi.

### 71–80 · Gökyüzü ve uçan gemi salonu (açık mavi, krem bulutlar, pirinç)
71 Uçan gemi iskelesi · 72 Bulut deniz feneri · 73 Sıcak hava balonu pazarı · 74 Rüzgâr gülleri kulesi · 75 Gökkuşağı köprüsü · 76 Kanatlı posta güvercinliği · 77 Bulutların üstünde çay bahçesi · 78 Pervaneli kütüphane · 79 Fırtına gözcüsü kulesi (dost, mor-mavi) · 80 Gün doğumu güvertesi.

### 81–90 · Orman tapınağı ve harabe salonu (zümrüt yeşil, taş grisi, altın)
81 Sarmaşıklı taş kapı · 82 Şelaleli tapınak · 83 Dev kök köprüsü · 84 Ateşböceği vadisi · 85 Taş kaplumbağa heykeli · 86 Gizli bahçe havuzu · 87 Ağaç ev köyü · 88 Kristal mağara ağzı · 89 Antik gözlem kulesi · 90 Orkide merdiveni.

### 91–100 · Kont'un şatosu salonu (derin mor, mum ışığı, altın; korkutucu değil, büyülü)
91 Şato kapısı ve köprü · 92 Mum ışıklı koridor · 93 Saat kulesi içi (dişliler) · 94 Kont'un kütüphanesi · 95 Kubbeli ziyafet salonu (boş, hazır sofra) · 96 Ayna salonu (yansımalar, figür yok) · 97 Kule bahçesi ve gül çitleri · 98 Simya laboratuvarı (renkli şişeler) · 99 Taht salonu (boş taht) · 100 Kurtuluş: şatonun üstünde gün doğumu, havai fişekler, uçan küçük Jiggle'lar.

## Kontrol (her tablo için)
- 6×6 ve 8×8 ızgarayla kes: hiçbir parça tek düz renk olmasın.
- 300.000 bayt sınırı, 1024×1024, RGB.
- Konu listedekiyle uyuşsun; salon renk paleti tutsun.
