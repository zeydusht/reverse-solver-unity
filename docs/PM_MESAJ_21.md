# PM → Claude Code (30 Ekim 2026) — Masaüstünde 9:16 oyun alanı (yalnız yayın)

Kod yazma, Unity derleme YOK. Düzeltme hazır, sadece yayınla.

## Ne değişti (PM yaptı)

Yalnız web sayfası CSS'i: `index.html` içindeki `#unity-canvas` satırının altına bir `@media (min-aspect-ratio:9/16) and (hover:hover) and (pointer:fine)` bloğu eklendi. Masaüstünde (fare, geniş pencere) oyun ortada 9:16 telefon alanında açılır, yanlar #0B2A36 olur. Telefon ve tabletlere (dokunmatik) dokunmaz.

Güncellenen dosyalar:
- `Assets/WebGLTemplates/ReverseSolver/index.html` (şablon, bir sonraki derlemede kalıcı olsun)
- `Builds/publish/br/index.html`
- `Builds/publish/gz/index.html`

## Yap

1. Üç dosyayı git'e commit et. Mesaj: "web: desktop 9:16 play area".
2. Aynı CSS bloğunu Pages'te yayınlanan şu sayfalara da uygula (yalnız index.html, oyun dosyalarına dokunma): `p19/` önizlemesi, `br/` ve diğer önizleme klasörleri (`?set=redesign` kullananlar). Kök adresteki Build 5'e DOKUNMA.
3. Yayınla ve masaüstü Chrome'da aç: oyun ortada dikey bir alanda görünmeli, tıklama/sürükleme doğru noktada çalışmalı (kayma olursa söyle). Tarayıcı penceresini daralt/genişlet, yeniden boyutlandırmayı dene.
4. Bir şey bozulursa (tıklama kayması, bulanıklık) bloğu geri al ve OVERNIGHT.md'ye yaz.
5. OVERNIGHT.md başına 2 satır: yayın linki + sonuç. iOS zip'ini üretme.
