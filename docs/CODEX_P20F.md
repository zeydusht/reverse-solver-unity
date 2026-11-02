# Codex görevi P20-F — Menü sahneleri sonradan insin (PM_MESAJ_29 4.3)

Amaç: ilk indirme (data.unityweb) küçülsün. Menü sahne resimleri build'e (Resources) girmesin, tablolar gibi oyun açıldıktan sonra arka planda insin. İnene kadar düz renk zemin.

## Dokunma
`Assets/_Game/Levels/*`, `Core/`, `Platform/`, telemetri (`GameRoot.Rate`, `_rated`), `Tools/verify_build.js`, gh-pages, `docs/PRODUCT.md`, `Tools/art_choice.py`'nin t20/t21 dışı kısımları. Commit atma. `Library/ Logs/ Builds/` okuma. Geçici dosyalar `%TEMP%`'e.

## Kapsam
`Resources/Art/t20-scene-{achievements,collection,jiggles,levels,home}.png` ve `t20-levels-path-repeat.png`.
- `t20-scene-home` kodda kullanılmıyorsa (Ev `menu_bg` kullanıyor) yalnız build'den çıkar.
- Kullanım yerleri: `MenuUi.Scene`, `MenuUi.Viewport`/`SceneSlice` (`Ui/MenuTabs.cs`), `LevelsMap.cs` (yol şeridi, `levels-path-repeat`, dikey tekrar).

## Yapılacaklar
1. Dosyalar `Assets/StreamingAssets/MenuArt/` altına: sahneler **JPG** (opak; saydam pikseller varsa sahnenin koyu zeminine düzleştir, kalite 82, boyut aynı 856×1844), yol şeridi **PNG** (saydamlık var). `Resources/Art/`'tan ve `.meta`'larından sil. `Tools/art_choice.py` t20 kopyalamasında bu 6 dosyayı Resources yerine oraya yazsın (sahneleri JPG'ye çevirerek), tekrar çalıştırınca Resources'a geri gelmesin.
2. İndirme: mevcut `LevelImages` (Board/LevelImages.cs) mantığıyla ama ayrı bir örnek (menü resimleri `KeepOnly` ile atılmasın; bölüm resimleri gibi bırakılmasın). Adres `StreamingAssets/MenuArt/<dosya>`. Oyun açılır açılmaz (ilk menü kurulduktan sonra) 6 dosyanın hepsi sırayla arka planda istensin; biri hata verirse sessizce düz renk kalır.
3. Çizim:
   - `Scene`: resim gelene kadar ekranı dolduran düz renk (sahnenin ortalama rengine yakın koyu teal, ör. `#0B3A44`; her sahne için resmin ortalama rengini sabit olarak yaz). Resim gelince aynı yere 0,2 sn solarak çizilsin; ekran o arada kapanmışsa (root yok) hiçbir şey yapma.
   - `Viewport`/`SceneSlice`: aynı resimden kırpma; resim yoksa aynı düz renk dilim, gelince kırpılmış sprite ile değiştir. Kırpma sprite'ının sahibi bugünkü gibi `MenuPainting.Owned` ile temizlensin.
   - `LevelsMap` yol şeridi: Texture2D'den `wrapModeV = Repeat` ile; gelene kadar çizilmez. Döşeme bugünkü görünüşle aynı.
   - Doku ayarları: `filterMode Bilinear`, `wrapMode Clamp` (yol V Repeat), mipmap yok; sprite PPU bugünkü gibi (sahne 1, yol 3), pivot bugünkü gibi (yol alt-orta).
4. iOS ve editör: StreamingAssets yerel dosya yoluyla çalışır (LevelImages.Url'deki gibi `file://`).

## Kontrol
Derlenmesi; menü sekmelerinde (Bölümler, Koleksiyon, Jiggles, Başarılar) önce düz renk, sonra resim. Unity'yi çalıştıramıyorsan derlemeyi bana bırak (C# 9, Unity 6).

## Bitince
KISA rapor: değişen dosyalar, StreamingAssets'e giden dosyalar ve boyutları, varsayımlar.
