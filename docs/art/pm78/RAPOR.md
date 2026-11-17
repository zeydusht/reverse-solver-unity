# PM_78 RAPOR: Zeyd'in yolu, orijinal menü resminin içinde

**Özet**
- p23 güncel (gh-pages 89dbf868). Build 37,3 MB Brotli; PM_77'den 0,8 MB küçük, çünkü PM_77'nin Codex odası kaldırıldı.
- Testler: EditMode 737/739 (2 atlandı, 0 hata), PlayMode 6/6. `verify_build` yerelde 4/4, canlıda 4/4.
- Ölçümler PC'de, gerçek GPU'lu başsız Chrome'da yapıldı (390×844). iPhone'da ölçülmedi.
- **Yay yalnız `&gecis=yay` ile açık. Zeyd'in onayı bekleniyor.**

## 1. PM_77 odası gitti; diorama artık orijinal menü resminin kendisi
- **Codex'in PM_77 odası kaldırıldı.** Duvar, zemin ve sütun dokuları build'den çıktı, `Diorama77` ve içe aktarma betiği silindi.
- **Menü resminin kamerası resmin kendisinden çözüldü** (`Tools/pm78_geom.py`).
  - Tuvalin üst ve alt kenarları duvarın kaçış noktasında buluşuyor: (3873, 1005). Dikeyler dik; ufuk lambri çıtasında.
  - Tuval duvarda temiz bir dikdörtgene oturuyor: 1 × 1,365 (genişlik × yükseklik).
  - Bu çözüm resmin herhangi bir yerini zorlamadan tutarlı çıktı.
- **Her katman menü resminin kendi piksel ızgarasında duruyor.** Her pikselin 3B yeri, başlangıç kamerasından o piksele giden ışının katmanın düzlemine değdiği nokta. Bu yüzden 1. kare menü resmini piksel piksel yeniden çiziyor; sonraki her kare aynı odanın başka bir yerden görünüşü.
- **Katmanlar** (`katmanlar.png`):

  | Katman | Konum |
  |---|---|
  | duvar + tablo | duvar düzlemi |
  | zemin + halı | zemin düzlemi; ikisi duvarın dibinde birleşiyor |
  | sağdaki sütun | duvarın 0,05 önünde |
  | flama | duvarın 0,11 önünde |
  | aplik | duvarın 0,17 önünde |

  Ölçek: tuval genişliği 1.
  - Ön katmanlar göründükleri sürece duvara göre kayıyor: sütun ~4, flama ~6, aplik ~9 pt.
  - Kemerler ve tonoz duvar katmanında. Resimde sütun duvara gömülü çizilmiş (ayağı duvarın dibinde), kemerler de sütunun başlığından çıkıyor. Ayrı bir katmana alınsalardı ayakları havada kalırdı. Yolun ilk yarısında ekrandan çıkıyorlar.
- **Codex'in işi: yeni oda çizmek değil, orijinali tamamlamak.** Tek tur yetti; prompt `CODEX_PM78.md`, kontroller `codex/`.
  - Kamera yol boyunca resmin dışını da görüyor. Codex şu kenarları aynı oda, aynı renk ve tarzla doldurdu:
    - üstte 128 px (duvar ve kemerler);
    - sağda 384 px (galeri devam ediyor, halı ve mermer zemin aynı çizgilerle sürüyor);
    - altta 64 px.
  - Sütun, flama ve aplik silinmiş iki plaka da çizdi. Bunlar ön katmanların arkasında, kayınca açılan birkaç piksellik şerit için.
  - Orijinal pikseller her dosyada birebir korunuyor (`dogrulama.json`: fark 0). Codex'in sildiği pikseller oyunda yalnız ön katmanın tam örttüğü yerde kullanılıyor.
  - Codex 30 dakikalık süre sınırına takıldı ama dosyaları ve kontrolleri bitirmişti. Üretim notunu ben yazdım.
- **Kabul testi** (`baslangic_karsilastirma.png`: yan yana, %50 üst üste, fark):
  - **Diorama başlangıç pozundan çizildi** (Python, oyundan bağımsız ışın izleme, aynı örnekleme): menü resmiyle en büyük fark 2/255. Tablo, çerçeve, renkler ve kemer birebir üst üste.
  - **Oyunun kendi kareleri** (bayraksız menü ile `&gecis=yay` menüsü):
    - oda ortalama 1,7/255 (iki dokunun sıkıştırma izi ve JPEG);
    - tuvaldeki bölüm resmi 0,3/255.
    - Üst üste bindirmede çift görüntü yok.
  - Codex'e yeniden çizdirmek gerekmedi.
- **Çapraz geçiş yok.** Bayrakla menünün arka planı bu dioramanın başlangıç görüntüsü, tablodaki resim de menünün çizdiği gibi. Dokununca hiçbir şey değişmeden kamera kalkıyor.
  - Tablodaki resim ilk karede menüdeki gibi (düz kesilmiş) başlıyor, yol boyunca tuvalin gerçek perspektifine yumuşakça geçiyor.
- **Bayraksız menü değişmedi.**
- **Karşıdan görünüş:** `karsidan.png` (duvar tam karşıdan, zemin tepeden). Zeyd'in dört karesi: `zeyd_kareleri.png`.

## 2. Yol: Zeyd'in kareleri, durmadan tek akış
- **Kareler:** stüdyodaki dört kare, açıları aynen: 30,2° → 21,7° → 11,1° → 0,6°.
  - Zaman: tek bir global smootherstep, 3,6 sn. Yalnız başta yumuşak kalkış, sonda yumuşak duruş; aradaki karelerde hız sıfıra düşmüyor.
  - Yol: centripetal Catmull-Rom. Stüdyonun önizlemesi uniform Catmull-Rom kullanıyor; iki yol en çok 4 cm ayrılıyor (stüdyo ölçüsüyle).
  - Kafa her an tablo merkezine bakıyor; açı konumdan hesaplanıyor, kareler arasında açı ara değerlemesi yapılmıyor.
- **Ölçek: yol 1,87 kat büyütüldü.**
  - Stüdyonun 1. karesi tabloya menüdekinden yakın: stüdyoda tablo ekran yüksekliğinin ~%71'i, menü resminde ~%45'i.
  - Tablo genişliğini ölçek alıp kareleri olduğu gibi koysaydım 1. kare menü olamazdı. O yüzden yolun şekli ve açıları aynen kaldı, yol tablo merkezine göre 1,87 kat büyütüldü; 1. kare tam menünün kamerası oldu.
  - Görüş açısı menü resminden geliyor (~43° dikey, stüdyoda 52°) ve yol boyunca sabit.
- **Kafa tablo merkezinde.** Yatayda baştan beri merkezde. Dikeyde tablonun merkezi menüde ekranın üst üçte birinde (%37); yol boyunca ekranın ortasına iniyor, dikeyler dik kalıyor.
- **4. kare ile dalış arasında durma yok.** Yaklaşma, yay bitmeden 0,8 sn önce başlıyor.
  - Tuval köşelerinin ekran hızı yayın tepesinde 86 pt/sn. Geçiş boyunca 52 pt/sn'nin altına inmiyor; tepenin %60'ı, hiç durma yok.
  - 0,6 sn erken başlasaydı 31'e iniyordu; aynı anda başlasaydı 0'a düşüyordu.
- **Dalış PM_77'deki adımlarla:** yaklaşma 1,1, sadece resim 0,5, yapboz çizgileri 1,0, HUD 0,4 sn; aralarda 0,15 / 0,15 / 0,1 sn nefes.
  - Toplam 6,2 sn; ölçülen 6,3 sn.
  - İlk dokunuş 2× hızlandırıyor (tuvale 2,2 sn'de varıyor), ikincisi bitiriyor.
- **Kenar boşluğu denetimi** (`&dalisbosluk=1`):
  - 390×844'te 465 karenin 0'ında boşluk var;
  - 375×667'de 457 karenin 0'ında (`Tools/pm72_tour.js --size`).
  - Kamera yolun hangi noktasında hangi alanı görüyorsa, her ekran boyu için yalnız o kadarı kuruluyor.
- **Akıcılık:** medyan 59,9 FPS, en kötü kare 50 ms (4 kare 20 ms'yi aşıyor; bölümün kurulduğu anda, resim ekranı kaplarken). Tek karelik sıçrama yok.

## 3. Yayın
- Yeni yay `&gecis=yay` bayrağında. Bayraksız oyunda menü → bölüm geçişi PM_77'deki gibi.
- **Kazanma → sonraki bölüm:** PM_77'nin eski koridoru, ardından dalış; dokunma aynı. 561 karede boşluk 0, en kötü kare 33 ms.
- **Boyut:** plaka 1408×1728 (DXT1 crunch, menü resmiyle aynı ayar) ve üç küçük kesit. Yalnız bayrakla yükleniyor.
- **GIF'ler:**
  - `yay.gif`: normal hız, 50 kare/sn.
  - `yay_yavas.gif`: yarım hız, her kare tek tek.
  - İkisi de yay ve yaklaşmayı kapsıyor (4,4 sn), 200 px genişlik.

## Doğrulanmayan
- iPhone Safari'de his, FPS ve bellek.
- Zeyd'in kararı: onaylarsa bayrak varsayılan olur; beğenmezse ne değişsin.

## Açık
- Yay için Zeyd onayı (`&gecis=yay`).
- Resim çizildiği gibi bir yerde tutarsız: halı duvara tam paralel değil, sağa doğru duvara yaklaşıyor. Yolda gözle seçilmiyor, tepeden bakışta (`karsidan.png`) görülüyor.
