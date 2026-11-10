# CODEX_TASK31 — HUD v3: renk, doygunluk ve katmanları Royal Match ile BİREBİR yap

Zeyd TASK30'u gördü: **yerleşim doğru, yuvalar doğru, ortak paydadayız.** Ama renkler soluk, katman geçişleri yok, rakamlar fazla büyük ve ortalanmamış. Bu sefer referanstan **ölçerek kopyala**. Zeyd'in kararı: booster tuşu, rozet ve panel stili için **birebir aynısı alınır**. Karakter (Jigo) ve oyun ikonları bizim kalır.

Referans: `docs/art/Reference/rm_oyun_ekrani.png` (1170×2532 px; pt = px÷3). Yakın kırpmalar: `docs/art/task31/ref_alt.png`, `ref_moves.png`, `ref_rozet.png`.

## 0. Kalite kuralı (ZORUNLU, bu yüzden 3. tur oluyor)
Teslimden önce kendi çıktını referansla **aynı ölçekte yan yana** kırp: (1) booster tuşu, (2) rozet, (3) Ayarlar tuşu, (4) sağ panel, (5) orta pencere, (6) şerit bordürü. Her halkanın rengini PIL ile ölç; referansla farkı her kanalda ≤12 olsun. Tablosunu `docs/art/task31/denetim.md`'ye yaz. Tutmayanı düzelt, sonra teslim et. Gözle "benziyor" yetmez.

## 1. Booster tuşu — birebir (çap 65 pt)
Merkezden dışa ölçülen katmanlar (px, referans):
| Katman | Renk | Kalınlık |
|---|---|---|
| Yeşil gövde | **dikey gradyan**: üst `#A9DD3E` → `#70D700` → orta `#37A400` → `#008C00` → alt `#005C00` / `#004B00` | gövde |
| İç disk (2. daire) | gövdeden bir ton açık, ince koyu kenarlı; üst solda parlak leke | çap ~%80 |
| Koyu yeşil kenar | `#024F00` | ~2 px |
| Altın halka | `#FFC000` (üstünde ince ışık `#FFCF00`) | ~5 px |
| Turuncu halka | `#FF6A00` / `#FF6400` | ~5 px |
| Yuva (bar üstünde) | koyu lacivert `#003D85` ~3 px + dışında açık mavi ışık çizgisi `#0074C7` ~2 px | tuşu saran oluk |

- **Kalın koyu mavi dış çizgi YOK** (TASK30'daki en büyük hata). En dıştaki renk **turuncu**; dışındaki ince lacivert + açık mavi, bardaki **oyuk**tur, tuşun parçası değil.
- Renkler **doygun**: sarı sarı, turuncu turuncu. Soluk krem-sarı kullanma.
- Basılı / kilitli / tükenmiş hâlleri aynı katmanlarla.

## 2. Adet rozeti — birebir (çap ~25 pt)
- Kırmızı küre, dikey gradyan: üst `#EB4314` → `#EA280C` → `#D31C0A` → alt `#A01305` / `#811401`.
- İçinde bir ton açık ince iç halka, kenarında koyu kırmızı çizgi. **Beyaz kontur yok.**
- Rakam: krem `#FFF5C8`, koyu bordo-kahve kontur + alt gölge. Rozet çapının ~%55'i.
- Yer: tuşun sağ altı, merkezden ~0,35 çap aşağı-sağa; halkanın üstüne biner.

## 3. Ayarlar tuşu — birebir
Halkalar booster ile aynı. Gövde mavi dikey gradyan: üst `#46AAE2` → `#007FE4` → `#0068C7` → `#0056B0` → alt `#003F81`. Dişli krem `#E7DBBB`/`#EACE87`, kalın koyu kontur `#2C2652`.

## 4. Üst paneller (Parça / Süre)
`ref_moves.png`'ye bak:
- **Kurdele:** mavi bant, **yalnız üst ve alt kenarında** ince altın + turuncu çift çizgi. Başlık krem-sarı `#FFF2B0`, koyu lacivert kontur, **küçük** (kurdele yüksekliğinin ~%40'ı) ve **tam ortalı**.
- **Gövde:** mavi kalın çerçeve (2 ton) → iç kenarda gri-bej **çukur bevel** `#A4997F` (sol ve altta kalın, ~2–3 pt) → krem gövde dikey gradyan: üst `#E9DDB8` (kurdele gölgesi) → alt `#FDF6CC`. **Gövdenin etrafında altın veya turuncu katman YOK.** TASK30'da yanlış eklenmiş.
- **Rakam:** krem-sarı dolgu (`#FFF5C8` → `#F6E7A8`), kalın koyu lacivert kontur, sol alta doğru lacivert 3B kalınlık. Yükseklik gövdenin **~%42**'si (TASK30'da ~%70, çok büyük). **Yatayda ve dikeyde tam ortalı.**
- Sol panel: küçük parça ikonu + sayı, ikisi birlikte ortalı. Sağ panel: `0:42`.
- **LV rozeti kalkıyor.** Seviye yalnız Ayarlar menüsünde görünür.

## 5. Renk: "old money" mavisi + krem
Zeyd'in seçimi: B (mavimsi) en yakın, ama turkuaz değil **mavi**. RM mavisinin bir alt tonu: biraz daha lacivert, biraz daha az doygun. Kremle yan yana çok iyi durmalı. **İki aday**, aynı ekranda:
- **M1 Old money:** şerit üst `#14325E` → alt `#2A5A94`; kurdele ve çerçeve `#2E64A3` → `#1D4A82`; koyu kontur `#0E2547`; açık ışık çizgisi `#5B8CC8`; krem `#F7EDCF` / `#FDF6DC`.
- **M2 RM birebir:** şerit `#134383` → `#1A5EA5`, çerçeve `#0059AE` → `#004A8A`, ışık `#0075C5`, kontur `#003D87`.
Altın `#FFBD00`/`#FFC000`, turuncu `#FF7101`/`#FF6A00`, ışık sarısı `#FFCE57`. Bunlar her iki adayda **aynı ve doygun**.

## 6. Jigo büstleri (düzeltme)
- **Normal:** kaşları çatma. Rahat, sıcak, hafif gülümseyen bir karakter; kaşlar nötr ve yuvarlak.
- **Tedirgin:** iyi, aynen kalsın.
- **Korku:** daha az abartı; göz biraz büyük, ağız küçük "o", hafif titreme. Gözleri yuvalarından fırlatma.
- **Üzgün:** iyi ama **yüksük düz dursun**, eğilmesin. Üzüntü yalnız yüzde (kaş, dudak, gözde hafif ıslaklık).
- **Mutlu:** kaş çatma yok. Gerçek, içten gülüş; gözler hilal, yanaklar kalkık.

## 7. Kilit: B (Kont'un balmumu mührü) seçildi
- B tasarımını final yap (64/96 pt). Kırmızı mühür detayı korunur.
- Açılma: 4 kare, kulp hızlı fırlar ("pop"). **Düşüşü kare ile çizme**: yalnız açık kilidin tek temiz PNG'sini ver; düşüşü Claude Code kodla yapacak (yerçekimi + dönme + solma).
- Mühür kırılması için 3 kare küçük parça (kırmızı mum kırıntısı) ver.

## 8. Zincir
Zeyd'in istediği: **p20'deki zincir** (PM_52'den önce oyunda olan, `&art=eski` ile görünen). Task17 değil, Task28F değil. O zincirden tek halka sprite'ı çıkar, sarkma/yaylanma kodu aynı kalacak. 5 renk çifti.

## 9. Süre doldu paneli (yeniden)
RM'nin "out of moves" mantığı:
- Üstte kurdele başlık **"Süre doldu"**.
- **Ortada büyük, üzgün Jigo** (saatin olduğu yerde, panelin merkezinde, büst).
- Altında **"+30 sn"** ve küçük saat ikonu.
- Büyük yeşil **satın al tuşu**: tuşun içinde altın ikonu + fiyat (`900`). Fiyat tuşun üstünde, ayrı bir hapta değil.
- Sağ üst köşede yuvarlak kırmızı **X** (vazgeç). Ayrı "Vazgeç" tuşu yok.
- Panel katmanları §4'teki gibi (mavi çerçeve, bevel, krem gövde, kurdele).

## Teslim
- `docs/art/task31/onizleme.png`: RM ve bizim ekran yan yana (M1 ve M2 ayrı), booster + rozet + Ayarlar + panel **yakın kırpma karşılaştırmaları**, Jigo'nun 5 hâli, kilit B final, zincir, süre doldu paneli.
- `docs/art/task31/denetim.md` (renk ölçüm tablosu).
- Dosyalar `Assets/Art/_yeni/HUD31/`, `Jigo_v3/`, `Obstacles_v3/`, `Devam_v2/`. Eskilerin üstüne yazma.
- 3 satır özet, **dur**.
