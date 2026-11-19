# Codex PM_89: saat booster'ının buzlu barı ve kırılması, 3 seçenek

## Sorun
Süre panelinin altındaki yeşil süre barı kalkıyor. Yerine yalnız **saat booster'ı** kullanılınca (Jigo saati 10 sn dondurur) panelin altında buzlu bir bar çıkacak:
- donma boyunca soldan sağa boşalacak;
- bitince çatlayıp kırılarak kaybolacak.

Süre paneli de donarken hafif buz kenarı alacak.

Referans: `docs/art/pm89/buzlu_bar/referans_hud.png`. Görüntü 2× ölçekte; üstüne bir kart geldiği için kararmış. Sağdaki "Süre" panelinin altındaki yeşil ince çizgi, kalkacak bar.

Ölçüler (oyun noktası; 1 pt = 3 px çiz):
- panel gövdesi 113×97 pt;
- buzlu bar panelin hemen altında, yaklaşık 100×14 pt.

Stil: old money mavi, krem, altın; opak ve doygun; buz mavisi (#CFF3FF → #6FC6EE), beyaz kırağı.

## Parçalar (her seçenek)
1. `track.png`, 600×84 şeffaf: barın yuvası. Koyu lacivert oluk, ince altın ya da buzlu kenar. 9-dilim: uçlardaki 42 px sabit, orta düz (her sütun aynı).
2. `fill.png`, 600×60 şeffaf: buz dolgu. Buz mavisi, hafif kırağı dokusu, üstte ince parlama. 9-dilim: uçlardaki 30 px sabit, orta düz (her sütun aynı; doku dikey yönde olabilir).
3. `crystal.png`, 128×128 şeffaf: küçük buz kristali parıltısı (6 kollu yıldız, merkezi parlak). Barın üstünde ara ara parlayacak.
4. `shard_1.png` … `shard_6.png`, her biri 96×96 şeffaf: kırılınca saçılacak küçük buz parçaları. Farklı şekiller, keskin ama sevimli; yarı saydam açık mavi, beyaz kenar.
5. `crack_1.png`, `crack_2.png`, `crack_3.png`, 600×84 şeffaf: barın üstüne binen çatlak katmanları (1 ince, 2 dallanmış, 3 tamamen yarılmış). Beyaz-mavi çizgiler, `track` ile aynı yerleşim.
6. `panel_frost.png`, 339×291 şeffaf: süre panelinin üstüne binecek buz kenarı. Kenarlarda ve köşelerde kırağı ve küçük buz sarkıtları. Ortası tamamen şeffaf, sayı okunmalı (ortadaki 200×150 px alfa 0).
7. `onizleme.gif`, 390×200, ~2 sn, 30 fps (PIL ile):
   - koyu lacivert zeminde süre paneli yer tutucusu (krem kutu, "0:42");
   - altında bar doludan boşa iner (hızlandırılmış);
   - kristaller parlar;
   - sonda crack_1 → 3, parçalar saçılıp düşer, bar söner, panel kırağısı erir.

## Seçenekler
- **A:** Lacivert oluk + altın kenar; dolgu buz mavisi kalın, içinde hafif kabarcık/kırağı; parçalar küçük ve çok.
- **B:** Oluğun kendisi de buz (yarı saydam açık mavi, beyaz kırağı kenar), dolgu daha doygun mavi; parçalar az ve büyük.
- **C:** Krem oluk + mavi kenar (oyunun panel dili), dolgu parlak buz; panelde kırağı daha belirgin, sarkıtlı.

## Teslim
Her seçenek X ∈ {A, B, C} için `docs/art/pm89/buzlu_bar/X/` altına yukarıdaki dosyalar. Ayrıca:
- `docs/art/pm89/buzlu_bar/karsilastirma.png`: üç seçeneğin birer karesi yan yana;
- `docs/art/pm89/buzlu_bar/LOG.md`: seçenek başına 1 satır.

Kontroller: boyutlar, RGBA, 9-dilim orta bölgelerinin düzlüğü, panel_frost'un ortasının şeffaflığı.

Mevcut dosyalara dokunma, oyuna bağlama yok, soru yok.
