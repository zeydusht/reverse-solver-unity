# Sanat üretim paketi · 24 Ekim 2026

Dal: `art`. Üretim yalnız `Assets/Art/` ve `docs/art/` altında yapıldı; BACKLOG.md'de yalnızca Art tablosu güncellendi. Mühendislik değişikliklerine dokunulmadı.

## Onaylı kaynaklar ve koruma

- İkon: `Assets/Art/Review/Ikon/2-market-mavi-v01.png` — Zeyd'in seçtiği görsel. PNG kaynağı korunup 1024/512/256 opak boyutları `Assets/Art/Icon/Exports/` altına verildi. Platform maskelemesi eklenmedi.
- Koridor: `Assets/Art/Review/Koridor/2-ortali-yan-blok-v04.png` — Zeyd'in en son seçtiği düzen. Dosyanın byte kopyası menü klasöründe kaynak olarak duruyor. v05 ve diğer adaylar yerlerinde korunur.
- Jiggles `Characters/Jiggles-reference-v02.png`, Baron `Characters/Baron-reference-v01.png`, seçilen Baron taslağı `Ornekler/Baron/3.png` de yedekte tutuldu.
- Beş korunan dosya için yeni SHA-256 envanteri ve ZIP: `docs/art/backups/current-approved-20261024-v01-manifest.json`, `current-approved-20261024-v01.zip`. Önceki 50 dosyalık ve üç iptal edilmiş dosyayı içeren yedek de yerinde bırakıldı.

## Üretilenler

- Menü: `Assets/Art/Menu/opening-layout-v01.png`, `Menu/index.html`, yazısız teal/turuncu düğme ve 1024×320 şeffaf dışa aktarımlar. LEVEL X yazısı demo dosyasında ayrı metindir.
- Animatik: `Assets/Art/AnimationDemo/index.html` ve `poster-v01.png`. İki Jiggle ve iki Baron koşu pozu ayrı şeffaf PNG'lere ayrıldı. Animasyon 2,8 sn döngülü, sprite/tween tabanlıdır.
- Engeller: `Assets/Art/Obstacles/Exports/` içinde bomba, çivi, zincir ve şerit duvarı, her biri 1024×1024 şeffaf.
- Güçlendiriciler: `Assets/Art/Boosters/Exports/` içinde makas, değnek, çekiç ve saat, her biri 1024×1024 şeffaf.
- Onay ve yeni adaylar aynı önizlemede: `Assets/Art/ProductionReview.html`.

## Sınırlar ve sonraki karar

Yeni menü yerleşimi, düğme rengi/ölçeği, koşu kareleri ve sekiz engel/booster görseli konsept onayı bekler; Zeyd bunlardan birini onaylanmış kaynak diye belirtmedikçe yeni kartlara taşınmaz. Animatikte arka plan tek bir görseldir: resmin ana çerçeveden sağdaki çerçeveye sahne perspektifinde kayması ve yeni resmin bağımsız görünmesi henüz yapılmadı. Bunun için ayrı çevre/çerçeve/tablo katmanları ve kamera animasyonu gerekir. Bu aşamada Unity kodu, oyun kuralları veya entegrasyon değiştirilmedi.
