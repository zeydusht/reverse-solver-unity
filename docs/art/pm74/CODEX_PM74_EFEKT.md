# CODEX PM74 · Makas, çekiç, değnek: vuruş anı parçaları

## Oyun ve sorun
Reverse Solver: yapboz parçası çıkarma bulmacası, dikey telefon (390×844 pt, görseller @3x). Üç booster'ın vuruş anı zayıf:
- **Makas** zincirli iki parçanın arasındaki zinciri keser. Hedef: makas zincire gelir, **ağzı geniş açılır**, keskin bir "şak"la kapanır, **zincir halkaları kopup saçılır**, makas dönerek çıkar.
- **Çekiç** bir parçayı kırar. Hedef: vuruş anında ekran sarsıntısı + kıvılcım + **parça çatlar**, sonra kırılıp dağılır.
- **Değnek** bir bölgedeki parçaların bağlantılarını değiştirir. Hedef: **sihir izi** ve parçaların yer değiştirmesi okunur olsun.

Bakılacak dosyalar:
- Mevcut çizimler (bu tasarımları koru): `Assets/_Game/Presentation/Resources/Art/yeni/boosters21/Boosters/Scissors/walk/01.png` (kırmızı saplı, beyaz ağızlı makas), `…/Hammer/flight/01.png` (mavi çekiç), `…/Wand/tool/01.png` (mor saplı yıldız değnek).
- Zincirli tahta: `docs/art/pm74/once/08_bolum10.jpg` ve `11_bolum20.jpg` (zincirler altın, mor, turkuaz halkalar; uçlarında yuvarlak madalyonlar).
- Onaylı tarz: old money mavi, krem + mavi çerçeve, altın, opak ve doygun renkler, Royal Match kalitesi (`docs/art/booster ref/kareler/BOOSTER_REF_ANALIZ.md`).

## Parçalar (şeffaf RGBA PNG, @3x, kenarları temiz, gölge yok)
Klasör: `Assets/Art/_yeni/PM74/efekt/`

### Makas (`makas/`)
- `open.png`, `half.png`, `closed.png` 512×512: aynı makas, ağız **geniş açık (~70°)**, yarı açık (~30°), **kapalı**. Üçünde de **vida (pivot) görselin tam ortasında**, ağız sağa bakıyor, saplar solda. Üst üste konunca vida kımıldamasın.
- `link_a.png`, `link_b.png` 128×128: kırık (açılmış) tek zincir halkası, iki farklı açı. Renk **açık gri metal** (oyun içinde altın/mor/turkuaza boyanacak), parlak kenar.
- `snip_flash.png` 384×384: beyaz-altın X biçimli keskin parlama (kesme anı).

### Çekiç (`cekic/`)
- `crack_01.png`, `crack_02.png`, `crack_03.png` 384×384: kare bir yapboz parçasının üstüne binecek çatlak çizgileri (koyu çizgi + açık kenar), ortadan dışa büyüyor; 03 parçayı 5–6 dilime ayırıyor. Zemin şeffaf.
- `shard_a.png`, `shard_b.png`, `shard_c.png` 128×128: üçgenimsi kırık parça kıymıkları, **açık gri** (oyunda resmin rengine boyanacak), hafif kalınlık.
- `spark.png` 160×160: altın-beyaz dört kollu yıldız kıvılcım.
- `dust.png` 256×256: krem-bej yumuşak toz bulutu.

### Değnek (`degnek/`)
- `sparkle.png` 96×96: küçük altın-beyaz yıldız parıltısı (iz parçacığı).
- `swirl_01.png` … `swirl_06.png` 512×512: mor-altın sihir girdabı halkası (ortadan dışa açılıp sönen), 06 neredeyse kaybolmuş.
- `trail.png` 512×128: soldan sağa incelen mor-altın sihir izi şeridi (sağ ucu kalın ve parlak).

## Önizleme
`docs/art/pm74/booster/efekt_parcalar.png`: bütün parçaların koyu mavi zemin üstünde pafta görüntüsü; makasın üç pozu yan yana ve üst üste (vida hizası görünsün).

## Kurallar
- Kod, sahne, `Resources/` ya da başka dosyaya DOKUNMA. Git commit yapma.
- Bitince en fazla 5 satır kısa özet yaz.
