# CODEX PM76 · Tablonun karşısına yay çizen kamera: aynı odanın açıları ve katmanlar

## Oyun ve istek
Reverse Solver (dikey telefon 390×844 pt). Ana menüde oyuncu bir saray koridorunda, duvardaki altın çerçeveli tablonun önünde. "Bölüm N"e basınca kamera tablonun etrafında **bir yay çizerek** sağa ilerleyip tablonun **tam karşısına** gelecek, sonra tabloya yaklaşacak. Bunu düz bir resmi döndürerek değil, **aynı odanın farklı açılardan çizilmiş katmanlı karelerini** (2,5B parallax) değiştirerek yapacağız. Zeyd: "birebir aynısı olması lazım" — yeni açılar bugünkü odanın aynısı olmalı.

Referans (bugünkü görünüm, **değişmeden** başlangıç karesi a0): `Assets/_Game/Presentation/Resources/menu_bg.png` (1024×1536). Tablonun iç tuval alanı (oyun buraya bölümün resmini çizer) bu görüntüde dört köşe: sol üst (349,296), sağ üst (662,359), sağ alt (663,832), sol alt (348,815).

Oda: turkuaz/petrol yeşili duvar, barok **altın çerçeve** (tepesinde zambak süsü), krem taş sütunlar ve altın başlıklar, kırmızı zambaklı flamalar, pirinç duvar lambaları, mavi-altın saksıda beyaz çiçekli bitki, alçak krem lambri, mermer zemin, altın kenarlı **kırmızı halı**, soldan sıcak güneş ışığı. Tarz: parlak, doygun, yumuşak 3B oyun illüstrasyonu (Royal Match kalitesi).

## 1. Açılar (`Assets/Art/_yeni/PM76/aci/`)
Kamera tablonun ortasından geçen dikey eksen çevresinde bir çember üstünde sağa doğru ilerliyor. Bugünkü kare a0 (tablo soldan, yaklaşık 35–40° açıyla görülüyor). İstenen:
- **a1**: kamera yayda ~1/3 ilerlemiş (tablo hâlâ hafif soldan, açı ~25°)
- **a2**: ~2/3 ilerlemiş (açı ~12°)
- **a3**: **tam karşıdan** (tablo dikdörtgen, ekranın yatay ortasında; tuval alanı tam dikdörtgen)

Her açı için **ayrı katmanlar** (şeffaf RGBA PNG, kenarları temiz):
- `aK_bg.png` **1180×1770** (bugünkünden %15 büyük tuval; bugünkü ölçek: tablo çerçevesi görüntü yüksekliğinin ~%45'i): duvar, tablo çerçevesi, lambri, zemin, halı, uzaktaki sütunlar. **Tablonun iç tuval alanı ŞEFFAF** (oyun resmi oraya çizer). Kenara kadar dolu, hiç boşluk yok.
- `aK_fg.png` 1180×1770: kameraya **en yakın** ön plan öğeleri (ön sütun, flama, lamba, saksı/bitki) yalnız onlar, gerisi şeffaf. Kamera yay çizerken ön plan hızlı, duvar yavaş kayacak; ön plan sütunu bir açıdan ötekine geçişi örtebilsin diye a1 ve a2'de ekranın bir yanında kalın bir ön plan sütunu ya da saksı bulunsun.
- `aci.json`: her açı için tuval alanının dört köşesi (sol üst, sağ üst, sağ alt, sol alt; o açının 1180×1770 piksellerinde), çerçevenin dış kutusu, ve a0 için 1024×1536'dan 1180×1770'e eşleme (a0'ı ortalayarak %15 büyüt).

**a0 (bugünkü kare) de katmanlara ayrılsın, görünümü değişmeden:**
- `a0_fg.png` (1024×1536, menu_bg ile piksel piksel hizalı): sol öndeki sütun + flama + lamba + mavi saksı ve bitki; gerisi şeffaf.
- `a0_bg.png` (1024×1536): menu_bg'nin ön plan çıkarılmış hâli; sütunun ve saksının arkası duvar, lambri ve zeminle doğal olarak tamamlanmış (ön plan kayınca arkası görünecek). Tuval alanı şeffaf. Ön planın kapladığı yer dışında menu_bg ile piksel piksel aynı.

**Tutarlılık (zorunlu):** a1, a2, a3 aynı oda: aynı tablo çerçevesi (süsler, oranlar, altın tonu), aynı duvar rengi, aynı sütunlar, flamalar, lambalar, saksı, halı deseni ve aynı ışık. Görüntü üretiminde bugünkü görüntüyü (`menu_bg.png`) referans ver, "aynı sahne, kamera tablonun çevresinde yay üstünde X derece sağa döndü" diye tarif et. Tutmayan kareyi reddet, yeniden çizdir. **En iyi iki deneme** kalsın: birinci takım `aci/`, ikinci takım `aci_b/` (aynı dosya adları).

## 2. Koridor şeridi katmanları (`Assets/Art/_yeni/PM76/koridor/`)
Bölümden bölüme geçişte bu şerit yana kayıyor: `Assets/_Game/Presentation/Resources/corridor_strip.png` (1644×1688; iki tablo yuvası, aralarında sütun, flama, lamba ve saksılı bitki). Parallax için:
- `fg.png` (1644×1688, şeritle piksel piksel hizalı): yalnız sütunlar, flamalar, lambalar, saksılar ve bitkiler; gerisi şeffaf.
- `wall.png` (1644×1688): şeridin ön plansız hâli; sütunların ve saksıların arkası duvar, lambri, zemin ve halıyla doğal olarak tamamlanmış. Tablo yuvaları (koyu dikdörtgenler) aynı yerde, aynı biçimde.

## Önizleme (zorunlu)
- `docs/art/pm76/acilar.png`: a0, a1, a2, a3 yan yana (her biri katmanlar birleştirilmiş, tuval alanına aynı örnek resim konmuş: `Assets/Art/Paintings/painting-01.jpg`), altlarında açı adı. İkinci takım için `acilar_b.png`.
- `docs/art/pm76/katmanlar.png`: a0_fg ve a0_bg ayrı ayrı (dama tahtası zeminde) ve birleşimleri; koridor fg ve wall için aynısı.

## Kurallar
- Kod, sahne ya da `Assets/_Game/` altındaki hiçbir dosyaya DOKUNMA; yalnız `Assets/Art/_yeni/PM76/` ve `docs/art/pm76/` altına yaz. Git commit yapma.
- Bitince en fazla 6 satır kısa özet yaz (hangi takım daha tutarlı, neden).
