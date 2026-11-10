# Codex görevi P20-H — Eğitim karartması: tek maske (PM_MESAJ_31)

Kaynak: `docs/PM_MESAJ_31.md` (oku). Dosya: `Assets/_Game/Presentation/Board/TutorialSpotlight.cs` (+ yeni shader).

## Dokunma
`Assets/_Game/Levels/*`, `Core/`, `Platform/`, telemetri, `Tools/verify_build.js`, gh-pages, `docs/PRODUCT.md`. Eğitim akışı (adımlar, girdi kuralları, envanter) aynı kalır; yalnız örtü çizimi ve delik şekilleri değişir. Commit atma. `Library/ Logs/ Builds/` okuma. Geçici dosyalar `%TEMP%`'e.

## 1. Tek örtü + shader delikleri
- Yeni shader `Assets/_Game/Presentation/Resources/Shaders/Veil.shader` (`Shader "ReverseSolver/Veil"`, URP 2D'de çalışan basit unlit, şeffaf blend, ZWrite Off; mevcut `Presentation/Shaders/Shape.shader`'ın yapısını örnek al). Resources'ta olduğu için build'e girer; yükleme `Resources.Load<Shader>("Shaders/Veil")` + `new Material(...)`.
- Ekranı tamamen kaplayan tek bir quad (MeshRenderer, sorting order bugünkü örtüyle aynı ya da HUD ve booster çubuğunun üstünde, banner yazısı ve el örtünün üstünde). Örtü HUD'u, booster çubuğunu ve alt alanı da kaplar.
- Delikler shader'a dizi olarak: en çok 16 şekil, her biri `float4 _Shape[i] = (merkez.x, merkez.y, yarıGenişlik, yarıYükseklik)` ve `float4 _ShapeP[i] = (köşeYarıçapı, cosθ, sinθ, 0)` dünya biriminde (1 birim = 1 pt, mevcut `Draw.W` dönüşümü). `_Count`, `_Color` (#000, α .6), `_Feather` (7 pt).
- Fragment: her şekil için döndürülmüş yuvarlatılmış kutu SDF'i; `d = min(...)`; `alpha = _Color.a * smoothstep(-_Feather/2, _Feather/2, d)` (içeride 0, dışarıda .6). Daire = yarıçapı yarı-boyuta eşit kare. Şerit (zincir) = döndürülmüş ince kutu.
- Eski dikdörtgen parçalı örtüyü (`VeilRects`, `Rounded` yaklaşımları, `Box` ile örtü) kaldır. Delik listesi artık `List<Hole>` (merkez, boyut, yarıçap, açı).
- Delik dışında hiçbir yerde ton farkı olmamalı (tek quad, tek renk). Kenar ve güvenli alanlarda dikiş olmaz.

## 2. Delik şekilleri (yalnız gereken şekiller)
- **Sürükleme (bölüm 1):** hedef parçanın her hücresi (hücre boyu, köşe yarıçapı = hücrenin yuvarlaklığı ya da hücre × .18) + çıkış yolu: parçadan tahta kenarına (bugünkü .9 hücre taşma dahil) **parçanın o eksendeki genişliğinde** tek şerit. Bitişik satır/sütun açılmaz.
- **Zincir (bölüm 10):** yalnız **bir** zincirli çift: iki parçanın hücreleri + iki kilit arasında ince şerit (genişlik ~14 pt, iki kilit merkezini birleştiren döndürülmüş kutu). Çıkış yolu yok. Çift seçimi: zincir doğru parçası diğer zincirlerin doğru parçalarıyla en az kesişen; eşitse ilk.
- **Çivi:** çivili parça hücreleri + sayaç rozeti (daire, rozet boyu + 4 pt).
- **Bomba:** bomba parçası hücreleri + fitil rozeti (daire).
- **Duvar:** duvar şeridi (şeridin kendi dikdörtgeni, yuvarlak köşe 4 pt).
- **Booster:** tuş adımında yalnız tuş (daire, tuş çapı + 8 pt). Hedef adımında yalnız hedef hücre(ler) (makas: eklemin iki hücresi; çekiç: hedef parçanın hücreleri).
- Kural kartı adımı (bölüm 1) mevcut hafif örtüsünü kullanıyorsa onu da aynı tek quad ile (α .35, delik yok) çiz.

## Test
`TutorialScriptTests` / var olan testler bozulmasın; eski `VeilRects` testleri varsa delik listesi testine çevir (ör. zincir adımında delik sayısı = iki parçanın hücre sayısı + 1 şerit; sürükleme adımında hücreler + 1 şerit). Unity'yi çalıştıramıyorsan derlemeyi bana bırak (C# 9, Unity 6).

## Bitince
KISA rapor: değişen dosyalar, shader'ın sorting/order'ı, zincir çifti seçimi, eksikler.
