# Menü "Level X" tuşu (v1, 26 Ekim 2026)

Zeyd'in referansındaki tuş tarzı (yeşil gövde, kalın altın kenar, parlak üst yarı) kodla üretildi. Script: `make_button.py`.

- `menu-play-button.png`: normal hali, 600×216 px (@3x, 200×72 pt).
- `menu-play-button-pressed.png`: basılı hali. Gövde 6 px aşağı iner, renk koyulaşır.
- **9-slice:** sol ve sağdan 80 px, üstten ve alttan 70 px.
- **Kenar renkleri:** `docs/art/PALETTE.md`'deki altın skalasından (#FBBA0E / #F38A03 / #CE5D01).
- **Yazı görselin içinde değil.** Unity'de `LilitaOne-Regular.ttf` ile yazılır (OFL lisansı, `LilitaOne-OFL.txt`):
  - Boyut: tuş yüksekliğinin yaklaşık %43'ü.
  - Dolgu: #FFFFF0.
  - Kontur: 7 px (@3x), #1E5C0E.
  - Gölge: 5 px aşağıda, #14460A, %80 opak.
  - **Not:** Lilita One'da ğ, ş ve İ harfleri yok. Bu font yalnız "Level X" gibi İngilizce yazılarda kullanılmalı; Türkçe metinler Fredoka ile kalır.
- `preview.png`: normal ve basılı hal, "Level 7" yazısıyla.

## v2 — referans restorasyonu (güncel inceleme sürümü)

`menu-play-button-v2.png` ve `menu-play-button-v2-pressed.png`: 684×336, görünen tuş 660×300, şeffaf. 9-slice: dört kenardan 92 px. `preview-v2.png`: referansın özgün en-boy oranı korunarak karşılaştırma; krem #F9F1BA ile Level 7 yazısı yalnız önizlemededir. Üretim kaynakları Source/ altında; yeniden dışa aktarım docs/art/create-play-button-v2.ps1 ile yapılır. Önceki incelemedeki 660×220 oranı iptal edildi.