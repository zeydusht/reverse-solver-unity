# CODEX PM75 · Makas iskeleti, başı aşağı çekiç, ana menü üst çubuğu

## Oyun
Reverse Solver: yapboz parçası çıkarma bulmacası, dikey telefon (390×844 pt, görseller @3x). Onaylı tarz: **old money mavi** (lacivert #153E75 / #0E2244), **krem gövde + mavi kenar**, **altın**, kalın 3B yazı, **kurdele yok**, **opak ve doygun** (yarı saydam hiçbir şey yok), Royal Match kalitesi. Referans ekranlar: `docs/art/pm74/sonra/menu.jpg`, `docs/art/pm74/sonra/kazanma_en.jpg`, `docs/art/pm74/sonra/bolum20.jpg`.

Bütün çıktılar şeffaf RGBA PNG, kenarları temiz, dış gölge YOK (gölgeyi oyun çizer), resmin içinde yazı YOK.
Klasör: `Assets/Art/_yeni/PM75/`

## 1. Makas iskeleti (`makas/`)
Bugünkü makas: `Assets/_Game/Presentation/Resources/Art/yeni/pm74/makas/open.png` ve `closed.png` (kırmızı halka saplar, beyaz-gümüş ağızlar, altın vida). **Aynı tasarımı** iki ayrı yarıya böl ki oyunda ağız **yavaşça, sürekli** açılıp kapanabilsin:
- `blade_upper.png` 512×512: üstteki sap halkası (sol üstte) + ona bağlı ağız (sağa uzanır). Kapalı makasın üst yarısı.
- `blade_lower.png` 512×512: alttaki sap halkası (sol altta) + ona bağlı ağız (sağa uzanır). Kapalı makasın alt yarısı.
- `pivot.png` 128×128: altın vida başı (iki yarının üstüne biner).
- Kural: iki yarı **0° dönüşte üst üste konunca kapalı makası** verir (ağız sağa bakar, uçlar birleşik), **vida merkezi iki görselde de tam (256, 256)**. Oyun üst yarıyı vida çevresinde saat yönünün tersine, alt yarıyı saat yönünde döndürerek ağzı açar (her biri 0°–35°). Ağızların iç kenarı keskin ve okunur olsun; ağız açıkken arada boşluk temiz kalsın.

## 2. Başı aşağı çekiç (`cekic/`)
Bugünkü çekiç: `Assets/_Game/Presentation/Resources/Art/yeni/boosters21/Boosters/Hammer/flight/01.png` (mavi parlak tokmak başı, krem sap). **Aynı çekiç**, yeni duruş:
- `hammer_down.png` 512×512: 3B, hafif yukarıdan bakış. **Tokmak başı dikey duruyor, yuvarlak vuran yüzü tam aşağıya (tahtaya) bakıyor**; alttaki yuvarlak yüz hafif elips olarak görünsün. Krem sap baştan sağa-yukarı ~35° çıkıyor (oyuncu tutuyormuş gibi). **Vuran yüzün merkezi görselde (230, 470)**, çekiç bunun üstünde. Dönmeden yukarı kalkıp balyoz gibi inecek; vuracağı nokta net okunmalı.
- `hammer_down_squash.png` 512×512: aynı çekiç, vuruş anı: baş %8 basık, vuran yüz aynı noktada (230, 470).
- `impact_ring.png` 384×384: düz zemine yayılan beyaz-altın şok halkası (yatay elips, ince ve parlak, ortası boş).

## 3. Ana menü üst çubuğu (`menu/`) — 3 seçenek: A, B, C
Sorun (Zeyd): profil resmi kaymış, haplar yarı saydam (arka plan içinden görünüyor), ayarlar düğmesi belli olmuyor, Jiggle sayacı karışık. Üst alanda **en çok 3 öğe**: isim plakası, altın sayacı, ayarlar.

Her seçenek için (`menu/A_…`, `menu/B_…`, `menu/C_…`):
- `X_name_plate.png` 720×192 (240×64 pt): **solda yuvarlak profil halkası** (iç boşluk tam daire, çapı plakanın yüksekliğinin ~%85'i, plakanın sol ucunda, dikey olarak tam ortalı; oyun profil/Jigo resmini bu dairenin içine çizer: dairenin içi boş/şeffaf kalsın), sağında isim yazılacak **krem gövde**, **mavi kenar**, opak, 3B (üstte ışık, altta hafif koyulaşma). İsim yazısı yok.
- `X_coin_pill.png` 360×132 (120×44 pt): solda **altın sikke** (plakadan taşabilir), sağında sayı için krem gövde, mavi kenar, opak. Sayı yok.
- `X_settings.png` ve `X_settings_pressed.png` 192×192 (64 pt): **yuvarlak mavi düğme + beyaz dişli**, katmanlı 3B (altın ya da koyu mavi dış halka, parlak üst yarım ay, alt gölge kalınlığı). Basılı hali 4 px aşağıda, üst kalınlığı azalmış.
- Seçenekler birbirinden belirgin farklı olsun: ör. A sade krem + ince lacivert kenar + altın halka; B lacivert kalın çerçeve + altın iç çizgi (kazanma ekranındaki rozet gibi); C krem gövde + altın kenar + lacivert gölge şeridi.

## Önizleme (zorunlu)
- `docs/art/pm75/codex/menu_ust_A.png`, `_B.png`, `_C.png`: 390×844 (ya da @2x 780×1688) menü önizlemesi: arka plan `Assets/_Game/Presentation/Resources/menu_bg.png` (ekranı kaplayacak şekilde ortadan kırp), üstte safe area 47 pt boşluk, sonra solda isim plakası (içinde yuvarlak Jigo: `Assets/_Game/Presentation/Resources/Art/jigo/uzgun/01.png` olmasın, `docs/art/pm74/sonra/menu.jpg`'deki kare avatardaki mavi Jiggle'ı kullanabilirsin), ortada-sağda altın sayacı, sağ üstte ayarlar düğmesi. Yazılar: isim "ZEYD", altın "120".
- `docs/art/pm75/codex/makas_iskelet.png`: makas yarıları 0°, 15°, 35° açılmış üç durum yan yana koyu mavi zeminde (vida kımıldamamalı).
- `docs/art/pm75/codex/cekic_asagi.png`: `hammer_down` ve `hammer_down_squash` koyu mavi zeminde, vuran yüz noktası küçük kırmızı artı ile işaretli.

## Kurallar
- Kod, sahne, `Assets/_Game/` altındaki hiçbir dosya ya da başka dosyaya DOKUNMA (yalnız yukarıdaki iki klasöre yaz). Git commit yapma.
- Bitince en fazla 6 satır kısa özet yaz.
