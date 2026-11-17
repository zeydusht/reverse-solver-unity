# CODEX PM78 · Menü resmini tamamla (yeni oda ÇİZME)

## Bağlam
Menüden bölüme geçerken kamera, menü resminin içinde 3B olarak gezecek: önce sağa, sonra tabloya doğru ileri. Başlangıç karesi menü resminin **kendisi**. Oyun bu resmi düzlemlere ayırıp perspektifle yeniden çiziyor: duvar, zemin, sağdaki sütun, flaması ve apliği.

Kamera yol boyunca resmin kenarlarından biraz dışarısını da görüyor:
- üstte en çok ~80 px (duvar ve kemerler),
- sağ altta zemin ve halı (resmin sağ kenarından ~300 px dışarı).

Ayrıca sütun, flama ve aplik duvardan biraz önde. Kamera kayınca arkalarında birkaç piksellik şerit açılıyor.

**PM_77'de yeni bir oda çizilmişti; beğenilmedi. Bu iş yeni oda çizmek değil, orijinal resmi tamamlamak.** Orijinal pikseller oyunda birebir kullanılacak; senin çizdiğin yalnız resimde olmayan yerlerde görünecek.

## Girdiler: `Assets/Art/_yeni/PM78/girdi/`
- `oda_girdi.png` (1408×1728 RGB): orijinal menü resmi (`menu_bg.png`, 1024×1536) **(0,128)** konumunda. Çevresi magenta (#FF00FF):
  - üstte 128 px,
  - sağda 384 px,
  - altta 64 px.
- `maske_sutun.png`, `maske_flama.png`, `maske_aplik.png` (1408×1728, beyaz = nesne): tablonun sağındaki büyük sütun (başlıktan ayağa kadar), sütunun kırmızı zambaklı flaması ve sütundaki pirinç mumlu aplik.
- `maskeler_onizleme.png`: üç maske resmin üstünde (mavi sütun, sarı flama, yeşil aplik).
- Referans: `Assets/_Game/Presentation/Resources/menu_bg.png`.

## İstenen: `Assets/Art/_yeni/PM78/oda/` (hepsi 1408×1728, RGB PNG)

### 1. `oda_genis.png`: magenta alanlar dolu, resim kesintisiz devam ediyor
- Aynı oda, aynı renkler, aynı çizim tarzı (parlak, doygun, yumuşak 3B oyun illüstrasyonu), aynı sıcak ışık.
- **Perspektif aynı:**
  - duvar sağa doğru uzaklaşıyor;
  - halının kenarları ve altın şeritleri düz çizgiler; aynı doğrultuda devam etsin;
  - dikeyler dik.
- **Üst (128 px):**
  - sol ve ortada petrol yeşili duvar yukarı devam eder;
  - sağda krem kemerler ve tonozlar yukarı doğru doğal devam eder.
- **Sağ (384 px):**
  - galeri sağa doğru devam eder: aynı tarzda kemerler, sütunlar, pencereler;
  - altta krem cilalı mermer zemin;
  - kırmızı, altın kenarlı **halı aynı çizgilerle sağa devam eder**. Bu bölge yol boyunca en çok görülen yer: zemin ve halı temiz ve inandırıcı olmalı.
- **Alt (64 px):** halı ve mermer aşağı devam eder.
- **Orijinal 1024×1536 bölge değişmeyecek.** Üretimden sonra orijinali (0,128)'e piksel piksel geri yapıştır. Ek yerinde görünür bir çizgi, renk ya da ışık sıçraması olmasın. Gerekirse yalnız **eklenen tarafta** ≤24 px yumuşak geçiş yap.

### 2. `oda_flamasiz.png`: flama ve aplik yok
`oda_genis.png`'nin aynısı, şu farkla:
- `maske_flama` ve `maske_aplik` içinde (3 px genişletilmiş) flama ve aplik silinmiş;
- yerlerinde sütunun krem taş yüzeyi devam ediyor;
- maske sütunun dışına taşıyorsa o kısımda arkadaki duvar ya da küçük tablonun çerçevesi devam ediyor.

Maske dışı birebir aynı.

### 3. `oda_temiz.png`: sütun da yok
`oda_flamasiz.png`'nin aynısı, şu farkla: `maske_sutun` içinde (3 px genişletilmiş) sütun silinmiş, sütun hiç yokmuş gibi iki yandaki yüzeyler maskenin içine devam ediyor:
- **Solda:** tablonun bölmesindeki petrol yeşili duvar, altında krem lambri.
- **Sağda:** sonraki bölmenin gölgedeki duvarı ve lambrisi.
- **Üstte:** kemerler ve tonoz.

Oyunda bu alanın yalnız **sağ kenarındaki birkaç piksel** görünür; ama yine de temiz ve inandırıcı olsun. Maske dışı birebir aynı.

## Kontrol (zorunlu)
- `docs/art/pm78/codex/kontrol_genis.png`: `oda_genis.png` küçültülmüş, orijinalin sınırı ince bir çizgiyle işaretli.
- `docs/art/pm78/codex/kontrol_temiz.png`: `oda_flamasiz` ve `oda_temiz`'in sütun çevresi (x 700–1000, y 300–1300), yan yana.
- `docs/art/pm78/codex/dogrulama.json`:
  - üç dosyanın boyutu;
  - orijinal bölgede `menu_bg.png` ile en büyük piksel farkı (0 olmalı);
  - maske dışında dosyalar arası en büyük fark (0 olmalı);
  - magenta (#FF00FF ±40) piksel sayısı (0 olmalı).
- `docs/art/pm78/codex/URETIM_NOTLARI.md`: kısa üretim notu; kullandığın istemler `uretim_promptlari.json` içinde.

## Kurallar
- Yalnız `Assets/Art/_yeni/PM78/oda/` ve `docs/art/pm78/codex/` altına yaz. Kod ve sahneye dokunma. Git commit yapma.
- Görselleri yerleşik görüntü üretimiyle yap, `oda_girdi.png`'yi ve `menu_bg.png`'yi görüntü girdisi olarak ver. Birleştirme, maskeleme ve kontroller için PIL kullan.
- Bitince en fazla 6 satır kısa özet.
