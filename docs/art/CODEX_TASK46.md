# CODEX_TASK46 — Yeni saat booster'ı + Jigo geçiş kareleri (PM_62)

Mevcut dil: T44/T45 (old money mavi + krem, doygun altın/turuncu, hacimli, siyah kontur yok). Mevcut Jigo madalyon kareleri ve saat booster kareleri `Assets/Art/_yeni/` altında (önce bul ve bak). Eski saatteki **kopuk/yamalı kol kalkıyor**.

## 1. Saat: Jigo fırçayı kılıç gibi uzatır (tek parça poz kareleri)
Madalyondaki Jigo, gövdesiyle birlikte sağa eğilip fırçasını kılıç gibi uzatır; fırça madalyondan hafifçe taşar. Kol ayrı parça gibi GÖRÜNMESİN: her kare Jigo + fırça tek parça çizim.
- `Assets/Art/_yeni/T46/clock/extend/01..06.png` — düğmeyi indirir, fırçayı uzatır (son kare = tam uzanmış, fırça ucu sağda).
- `Assets/Art/_yeni/T46/clock/hold/01..04.png` — uzanmış hâlde hafif nefes döngüsü.
- `Assets/Art/_yeni/T46/clock/retract/01..05.png` — fırçayı geri çeker, düğmeyi alır (son kare = normal madalyon pozuna eşit).
- Madalyon karelerinin tuvali ve pivotu mevcut normal Jigo kareleriyle aynı olsun (fırça taşan kısım için tuval sağa genişlerse `LISTE.md`'de pivot/ofseti piksel olarak yaz). Fırça ucunun kare içindeki konumunu (px) `LISTE.md`'ye yaz.

## 2. Dokunma ışıltısı + buz kaplaması
- `T46/clock/spark/01..06.png` — fırça ucunun panele değdiği yerde ışıltı (altın-beyaz yıldız patlaması).
- `T46/clock/ice_overlay.png` — Süre paneline oturan buz/ayaz kaplaması (yarı saydam, kenarları kırağılı). Panelin mevcut boyutuna göre çiz.
- `T46/clock/ice_break/01..07.png` — buz çatlar ve parçalanıp düşer.
- `T46/clock/freeze_bar_bg.png` + `freeze_bar_fill.png` — panelin altındaki 10 sn barı (buz mavisi dolgu, krem/mavi çerçeve, 9-slice kenar payını yaz).

## 3. Jigo normal ↔ mutlu geçiş + daha sık kareler
- `T46/jigo/normal_to_happy/01..05.png` ve `happy_to_normal/01..05.png` — ara kareler, ani atlama olmasın.
- Normal ve mutlu döngüleri şu an kesik görünüyor: varsa 12–24 fps için ara kareler `T46/jigo/normal_loop/`, `T46/jigo/happy_loop/` (mevcut kare sayısını 2 katına çıkar, aynı tuval).

## Teslim
`docs/art/task46/onizleme.png` (saat akışı şeridi + geçişler, 390×844 ölçekte) ve `docs/art/task46/LISTE.md` (dosyalar, kare sayıları, önerilen fps, pivot/ofset, fırça ucu konumu, 9-slice). 3 satır özet, dur. Kurallar: kurdele/afiş yok, Jigo mutlu/normalde kaş çatmaz, soluk efekt yok, RM'yi birebir kopyalama yok.
