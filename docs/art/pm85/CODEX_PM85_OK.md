# Codex PM_85: Rehber oku (3 seçenek)

## Sorun
1. bölümün rehberinde parçanın gideceği yolu gösteren ok, şu anda ince ve noktalı (kesik çizgili altın ok, `Assets/_Game/Presentation/Resources/Art/yeni/tutorial/Path/straight-down.png`). Zeyd noktalı ve ince ok istemiyor.

## İstenen
Düz, **kalın, şerit hâlinde tek bir ok**:
- Rengi krem.
- Kenarı ve gölgesi rehber eldiveniyle aynı dilde ve onunla uyumlu: `Assets/_Game/Presentation/Resources/Art/hand-glove-back.png`. Eldiven yumuşak 3B, krem-beyaz, yumuşak gölgeli, siyah kontur yok; manşeti turkuaz, üstünde altın halka.
- Ok, oyunda parçanın gideceği yol boyunca uzanacak; ucu gidiş yönünde.
- Oyun oku uzatarak çizecek (önce gövde uzar, uç ilerler). Bu yüzden ok **iki ayrı parça** olsun:
  - **gövde:** boyuna her yerde aynı (desen, doku, renk geçişi yok; yalnız enine gölge). Dikey olarak gerilince bozulmamalı.
  - **uç:** gövdenin genişliğiyle kusursuz birleşir.
- Koyu mavi tablo arka planında ve %60 karartılmış ekranda da okunmalı.
- Yazı, noktalı çizgi, ince çizgi olmayacak.

## Seçenekler
- **A:** düz krem şerit, yuvarlak kuyruk, geniş üçgen uç; yumuşak iç gölge ve hafif alt gölge (eldivenin hacmi).
- **B:** krem şerit, kenarında ince altın şerit (eldivenin manşetindeki altın halka gibi); uç biraz yuvarlatılmış.
- **C:** krem şerit, ince turkuaz dış kenar (manşet rengi); uç keskin ve geniş; gövdede çok hafif parlama çizgisi (boyuna düz, gerilince bozulmaz).

## Dosyalar (yalnız bunlara yaz)
Her seçenek X ∈ {A, B, C} için `docs/art/pm85/ok/X/` altına:
- `body.png`: 128×256 şeffaf PNG, ok aşağı bakarken gövdenin bir dilimi. Şerit yatayda ortalı, genişliği ~84 px; üst ve alt kenarı düz kesik (boydan boya aynı; dikeyde gerilecek).
- `head.png`: 192×160 şeffaf PNG, aşağı bakan uç. Üst kenarında gövdeyle birleşen kısım aynı genişlikte (~84 px) ve yatayda ortalı; uç en altta.
- `tail.png`: 128×64 şeffaf PNG, gövdenin yuvarlak başlangıcı (aşağıda gövdeyle birleşir).
- `onizleme.png`: 390×844 maket. Koyu lacivert arka plan, üstünde 3×3 yapboz tahta; ekran %60 karartılmış, yalnız bir parça ve onun aşağı yolu açık. Ok bu parçadan aşağı tahta dışına uzanıyor, eldiven (`hand-glove-back.png`) parçanın üstünde.

Ayrıca:
- `docs/art/pm85/ok/karsilastirma.png`: üç seçenek yan yana, ok 300 px boyunda, hem açık hem koyu zeminde.
- `docs/art/pm85/ok/LOG.md`: 3 satır.

Mevcut dosyalara dokunma, oyuna bağlama yok, soru yok.
