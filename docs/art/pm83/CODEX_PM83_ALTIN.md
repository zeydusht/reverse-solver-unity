# Codex PM_83 — Altın para ikonu (3 seçenek)

## Sorun
Oyundaki altın ikonu eski ve sönük: yassı, kenarı ince, ortada soluk bir yapboz ya da yıldız. Zeyd bilinen mobil bulmaca oyunlarındaki gibi **gerçek oyun parası** istiyor.

## İstenen
Parlak, kalın kenarlı, yuvarlak altın para:
- hafif 3/4 açı değil, **önden**; kalın, kabartmalı bir dış halka (kenar) ve içte bir düzlem;
- ortada **kabartma** motif;
- üst solda belirgin **parlama** (beyaz-sarı ışık lekesi + küçük yıldız parıltısı);
- doygun altın sarısı → turuncu gölgeler, koyu altın-kahve alt kenar (kalınlık hissi);
- siyah kontur yok; oyunun HUD stiliyle uyumlu (onaylı old money mavi + krem, doygun altın; hacimli boyama). Referans: `Assets/_Game/Presentation/Resources/Art/yeni/T50/win/coin.png` (şimdiki, daha iyisi isteniyor) ve `Assets/_Game/Presentation/Resources/Art/yeni/pm75/menu/A_coin_pill.png` (şimdiki sayaç).
- 24 px'te bile para olarak okunmalı.

## Seçenekler
- **A:** ortada kabartma beş köşeli yıldız.
- **B:** ortada kabartma Jigo yüzü (yapboz parçası biçimli sevimli yüz, `Assets/Art/Characters/Jiggles-reference-v02.png`; tek renk altın kabartma, gözler ve gülümseme okunur).
- **C:** ortada kabartma taç ya da yapboz parçası (hangisi daha iyi okunuyorsa).

## Dosyalar (yalnız bunlara yaz)
Her seçenek X ∈ {A, B, C} için `docs/art/pm83/altin/X/` altına:
- `coin.png`: 512×512 şeffaf PNG, para kare içinde ortalı, kenardan ~16 px boşluk, alt gölge yok.
- `coin_pill.png`: 360×132 şeffaf PNG, üst sayaç: solda bu para (yüksekliği ~124 px), sağda krem dolgu ve ince lacivert-altın çerçeveli hap. **"+" düğmesi yok.** Şimdiki `A_coin_pill.png` ile aynı yerleşim (para hapın sol ucuna biner).
- `onizleme.png`: 390×844 küçük maket: üstte sayaç ("5000"), ortada "+50" ve parası, altta yeşil düğme üstünde "900" ve parası, ayrıca 24, 36, 64, 128 px boylarında para.

Ayrıca `docs/art/pm83/altin/karsilastirma.png`: üç seçenek ve şimdiki para yan yana (her biri 256 px ve 36 px).

`docs/art/pm83/altin/LOG.md`: 3 satır. Mevcut dosyalara dokunma, oyuna bağlama yok, soru yok.
