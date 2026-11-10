# Task15 — Claude Code teslimi

Hazır Source çizimleri yeniden üretilmeden Python/Pillow ile dışa aktarıldı. 19 üretim PNG'si `Assets/Art/Gameplay/Task15/` kökünde. Dosya başına boyut, 9-slice L/R/T/B, pivot ve hash `manifest.json` ve HANDOFF.md içinde. Oyun entegrasyonu yapılmadı; C# dosyalarına dokunulmadı.

## Yerleşim: eski ince dikdörtgenleri kullanmayın

| Öğe | Kaynak boyutu | Önerilen telefon ölçüsü | Davranış |
|---|---|---|---|
| Hamle hapı | 330×210 | 110×70 pt | Zemin Sliced, 84/84/84/84 px |
| Süre / urgent hapı | 366×210 | 122×70 pt | Zemin Sliced, 84/84/84/84 px |
| HUD ikonları | 120×120 | 36–40×36–40 pt | Simple, PreserveAspect; ayrı obje |
| Yeşil / turkuaz kart tuşları | 684×336 | 228×112 pt; önizlemede 220×108 | Sliced, 92/92/92/92 px; yüksekliği koruyun |
| Duvar tekrar | 144×72 | 48×24 pt | X Tiled; ince eski duvar slotuna sıkıştırmayın |
| Duvar köşe | 144×144 | 48×48 pt | Pivot 0.25/0.25; yukarıdan girer, sağa çıkar |
| Avatarlar | 256×256 | Kare 64–88 pt | Simple + PreserveAspect |
| Jiggle sayaç ikonu | 192×192 | 64×64 pt | Kare; eski yıldız yerine tek ikon |

Tüm ölçüler kaynak PNG pikselidir; nominal UI @3x ölçüsü PNG / 3'tür. Avatarlar talep edilen özel 256 px boyuttadır; tablo özel 1536 px boyuttadır. Pivotlar normalize Unity sol-alt başlangıçlıdır, köşe dışında 0.5/0.5.

HUD haplarında ikon yuvası, ikon veya sayı gömülü değildir. İkonu kare Image olarak ayrı yerleştirin; hap genişliği değişirken kronometre/parça esnemesin. Önizleme hamlede ikon kutusu yerel `(18,48,108,108)`, sayı alanı yaklaşık `(150,45,150,120)`; sürede ikon `(18,48,108,108)`, sayı alanı `(144,45,204,120)` piksel olarak örneklenmiştir. Hap yüksekliği 210 px kalmalıdır. LV rozeti ve durdur tuşu onaylı Task14 dosyalarıdır.

## Tuş ve yazı eşlemesi

Başla/Devam → `btn-primary-v2.png`; Yeniden başla/Ana menü → `btn-secondary-v2.png`. Her birinin `-pressed` dosyası aynı canvas/pivotu kullanır. Yeşil ana tuş onaylı `UI/PlayButton/menu-play-button-v2` dosyalarıyla byte düzeyinde aynıdır; yeniden çizilmedi.

Tuş yazısı PNG'ye gömülü değildir. Krem `#F8EBD0`, yaklaşık 1–1.5 pt lacivert kontur/gölge; kısa başlıklar 30–34 pt, uzun Yeniden başla metni 24–27 pt ve mevcut yüzeyin içine auto-fit. Sarı yüzey üstünde beyaz metin kullanılmaz. Pressed sırasında metin de yaklaşık 2 pt aşağı hareket edebilir.

Lilita One'ın ğ/ş/İ eksiği TASK14_FONT.md'de doğrulanmıştı. Önizlemede bu harfleri içeren metinler Windows Segoe UI Bold ile gösterildi; Windows fontu oyuna kopyalanmadı veya dağıtılmadı. Unity'de Türkçe kapsamı doğrulanmış TMP fallback gerekir. Sayı ve diğer kısa başlıklar mevcut Lilita One'dır.

## Kırmızı ve rozet katmanı

PALETTE.md v2 kırmızı skalası `#F04438 / #D62822 / #94160F`. Beyaz şerit/rozet konturu `#FFF9F1`; mercan `#E8574A` yalnız urgent süredir. Eski Task14 PNG'leri yerinde korundu.

Yeni rozet için önerilen bileşim: `Task14/booster-button.png` + ayrı `Task14/booster-<nesne>.png` + `Task15/booster-badge-v2.png` + ayrı adet TMP. Task14 `booster-<nesne>-normal/pressed/zero.png` birleşikleri eski rozeti zaten içerir. Bunların üzerine yeni rozet bindirilirse ekran görüntüsündeki çift kontur/üst üste rozet görünümü oluşur. Tek katman yolu seçilmelidir; onaylı booster nesneleri yeniden boyanmaz.

Duvar tekrarının ilk ve son RGBA sütunu aynıdır; köşenin üst/sağ bağlantıları düz parçaya eşleştirildi. Slice ile uzun şeridi germeyin, X Tiled kullanın. Dikey kenarda sprite'ı 90° döndürün. Atlas için padding/extrusion mühendislikte ayarlanır. Önizleme 24 pt kalınlığın altın ve mavi karşısında okunmasını gösterir.

## Resimli yapboz

`painting-test-01.png` tek 1536×1536 resimdir. Mevcut Kit81 maskeleriyle board UV üzerinden örneklenmeli; her parçaya tüm resmi ayrı ayrı basmayın. `preview-painted-board.png`, tabloyu mevcut 640 px Kit81 maskelerine (384 px gövde + 128 px marj) göre kesintisiz haritalayıp mevcut şeffaf overlay kenarlarını üzerine ekleyen 5×5 testi gösterir. Kit81 dosyaları değiştirilmedi. Önizleme seviye verisini veya shader'ı değiştirmez.

## Kontrol ve yeniden export

`docs/art/build-task15.py` yalnız Task15 ve docs/art içine yazar; Source PNG'lerinin önce/sonra hashlerini kontrol eder. 14 Source PNG ve 109 eski onaylı dosyada fark sıfır; duvar/köşe RGBA farkı sıfır. 19 dosyanın boyutu, alpha kanalı ve 9-slice kenarları doğrulandı. Rapor: `docs/art/reports/task15-export-validation.json`.

Python bağımlılıkları Pillow ve NumPy'dir; bu oturumda yerel `docs/art/.task15-python/` içine kuruldu ve sürüm kontrolünden hariç tutuldu. Export PowerShell betiği kullanılmadı. Önceki image_gen Source istemleri `docs/art/templates/task15-prompts.md` içinde; bu devam turunda yeni image_gen çağrısı yapılmadı.
