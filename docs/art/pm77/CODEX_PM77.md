# CODEX PM77 · Menü koridoru 3B diorama katmanları

## İstek
Menüden bölüme geçerken kamera, duvardaki tablonun çevresinde **gerçek bir 3B yay** çizip yandan tam karşıya gelecek. Bunu düzlemlerden kurulu bir diorama ile yapacağız: duvar bir düzlem, zemin bir düzlem, öndeki sütun bir düzlem. Unity her karede perspektifle çizecek. Bunun için katmanların **perspektifsiz (dik/ortografik)** çizilmesi gerekiyor: duvar tam karşıdan, zemin tam tepeden.

Oda bugünkü menü ile **aynı** olmalı. Referanslar:
- `Assets/_Game/Presentation/Resources/menu_bg.png` (bugünkü açılı görünüm)
- `Assets/Art/_yeni/PM76/aci/a3_bg.png` (aynı odanın tam karşıdan görünümü, PM76'da çizildi)
- `Assets/Art/_yeni/PM76/koridor/wall.png` ve `fg.png` (aynı galerinin karşıdan duvarı ve sütunları)

Oda: petrol yeşili/turkuaz duvar (#1E6B6B civarı), barok **altın çerçeve** (tepesinde zambak süsü), krem taş pilastrlar ve altın başlıklar, kırmızı zambaklı flamalar, pirinç duvar lambaları, alçak krem lambri, cilalı krem mermer zemin, altın kenarlı **kırmızı halı**. Işık soldan, sıcak. Tarz parlak, doygun, yumuşak 3B oyun illüstrasyonu.

## Katmanlar (`Assets/Art/_yeni/PM77/diorama/`, şeffaf RGBA PNG)
Ölçek: **1 birim = 400 piksel** (tablonun tuval yüksekliği 1 birim).

1. `duvar.png` **4800×1600**: duvarın **tam karşıdan görünüşü** (elevation), hiç perspektif yok, kaçış noktası yok.
   - Alt kenar (y=1600) zemin çizgisi.
   - **Ortada büyük barok altın çerçeve**, menüdekiyle aynı. İç tuval alanı **tam şu dikdörtgende ve ŞEFFAF**: sol üst (2244, 974), sağ alt (2556, 1374), yani 312×400.
   - Çerçevenin iki yanında, aralıklı **pilastrlar**: krem taş, altın başlık, üstünde kırmızı zambaklı flama, yanında pirinç lamba.
   - Komşu bölmelerde her yanda 2'şer **daha küçük altın çerçeveli tablo** (manzara resimleri).
   - Altta **lambri**: krem, altın çıtalı, yaklaşık 0,55 birim yüksek.
   - Üstte korniş ve kör kemerler. Duvar resmin üst kenarına kadar dolu.
   - Hiçbir yerde boşluk olmasın (tuval hariç).
2. `zemin.png` **4800×1200**: zeminin **tam tepeden** görünüşü. Üst kenar (y=0) duvarın dibi; aşağı doğru 3 birim izleyiciye yaklaşır. Cilalı krem mermer üstünde, duvara paralel uzanan **kırmızı halı**: duvardan 0,35 birim uzakta başlar, 1,1 birim enindedir, kenarlarında altın şerit ve zambak deseni vardır. Tam genişlik, kenara kadar dolu, perspektif yok.
3. `on_sutun.png` **480×2400**: kameraya yakın duran **serbest sütun**: krem taş gövde, altın başlık, kırmızı zambaklı flama, pirinç lamba ve dibinde mavi-altın saksıda beyaz çiçekli bitki. Tam karşıdan, gerisi şeffaf. Alt kenar zemine basar.
4. `diorama.json`: tuval köşeleri (piksel), zemin çizgisi, ölçek, sütunun taban orta noktası.

## Önizleme (zorunlu)
- `docs/art/pm77/codex/diorama_katmanlar.png`: üç katman ayrı ayrı (dama tahtası zeminde).
- `docs/art/pm77/codex/diorama_kontrol.png`: duvar ve zemini basit bir perspektif dönüşümle birleştirip **38° yandan** ve **tam karşıdan** iki görünüm üret. Tuvale `Assets/Art/Paintings/painting-01.jpg` koy, sütunu duvarın 1,2 birim önüne ve tablonun 1,8 birim soluna yerleştir. Bu iki görüntü `menu_bg.png` ve `a3_bg.png` ile aynı odayı göstermeli; göstermiyorsa katmanı yeniden çiz.

## Kurallar
- Yalnız `Assets/Art/_yeni/PM77/` ve `docs/art/pm77/codex/` altına yaz. Kod ve sahneye dokunma. Git commit yapma.
- Bitince en fazla 6 satır kısa özet.
