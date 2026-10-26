# Task15 — final export

19 final PNG hazır Source çizimlerinden Python/Pillow ile dışa aktarıldı. Yeşil ana tuş, onaylı Level v2 normal/basılı dosyalarının birebir kopyasıdır. Source çizimleri ve Task14 onaylı seti değiştirilmedi.

- Yeni sıcak kırmızı: booster-badge-v2; wall-repeat-v3 ve wall-corner-v3.
- Dolgun HUD: hud-moves-pill-v2, hud-time-pill-v2 ve mercan urgent sürümü; ayrı kare parça/kronometre ikonları. Zeminlerde ikon veya sayı yoktur.
- Kart tuşları: yeşil btn-primary-v2 ve turkuaz btn-secondary-v2, normal/basılı. 684×336 tuval, kalın 660×300 görünen tuş; 9-slice 92 px.
- Beş avatar 256×256; jiggle-counter-icon 192×192, 64 pt gösterim.
- painting-test-01 1536×1536; tamamı resim, parça çizgileri veya çerçeve yoktur.

preview.png resimli tahta/HUD, pause ve avatar seçimini birlikte gösterir. preview-ingame-blue.png yeni duvarı mavi parçalar üstünde gösterir. preview-painted-board.png mevcut Kit81 maskesi ve overlay ile resim testi; preview-components.png tüm parçalar; preview-seams.png altın ve mavi üstünde duvar tekrarıdır. Önizlemeler runtime atlasına alınmaz.

manifest.json her dosyanın kullanım, boyut, border, pivot ve hash bilgilerini içerir. Pivot Unity sol-alt başlangıçlıdır. Kullanım ve minimum ekran ölçüleri: docs/art/TASK15_EXPORT.md. Teslim: docs/art/HANDOFF.md. Kontrol: docs/art/reports/task15-export-validation.json.

Hap zeminini esnetirken ikonları ayrı Simple/PreserveAspect olarak kullanın. Eski rozeti içeren Task14 birleşik boosterına ikinci rozet bindirmeyin; ayrı zemin+ikon+yeni rozet yolunu kullanın. Sayı ve tuş metinleri oyun tarafından yazılır. C# veya Assets/_Game değişikliği yoktur.
