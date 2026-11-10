# Task19C — Güncel oyun, az altın ve yeni alt sekme seti

Teslim klasörü: `Assets/Art/Menu/Task19C/`. Görev başlığındaki “4 dosya” ile çelişen numaralı teslim listesi esas alındı: **dört ekran + karşılaştırma, toplam beş PNG**.

| Dosya | Ölçü | İçerik |
|---|---|---|
| [01-ev.png](../../Assets/Art/Menu/Task19C/01-ev.png) | 1125×2436 | Güncel oyun menüsü, yeni Ev sekmesi ve küçük Bölümler tuşu |
| [02-koleksiyon.png](../../Assets/Art/Menu/Task19C/02-koleksiyon.png) | 1125×2436 | Sade turkuaz çerçeveler, bölüm numaraları, 11/40 |
| [04-basarilar.png](../../Assets/Art/Menu/Task19C/04-basarilar.png) | 1125×2436 | Krem-turkuaz karolar, yeşil tikler, Hızlı Parmaklar |
| [08-sekme-cubugu.png](../../Assets/Art/Menu/Task19C/08-sekme-cubugu.png) | 1125×2436 | Dört seçili durum ve seçili olmayan dört ikon |
| [pilot-karsilastirma.png](../../Assets/Art/Menu/Task19C/pilot-karsilastirma.png) | 2100×1020 | Solda güncel QA menüsü; ortada 19B Koleksiyon/Başarılar; sağda aynı 19C ekranları |

## Güncel ekranın korunması

`docs/qa/2026-10-29-p19/tr_390x844_2_menu.png`, aynı klasördeki ayarlar ekranı, `2026-10-31` tahta/kazanma ekranları ve Royal Match inceleme kartı okundu. Yeni üretimde Task13 eski menüsü güncel oyun referansı sayılmadı.

Ev ekranında profil/avatar/oyuncu adı, tek Jiggle sayacı, dişli, tablo ve yeşil Level 1 tuşu **güncel QA görüntüsünün özgün piksel parçalarından** korunur. Güncel isim kırpılması ve İngilizce “Level 1” etiketi de referansla aynıdır; bunlar yeniden tasarlanmadı. Kahraman tablo ve koridorun orta bölümü özgün konumlarında tutulur. İsim altında Jiggles satırı, yıldız sayacı veya 128/500 çubuğu yoktur.

Mevcut ekranın üst kontrolleri çentik alanında, yeşil tuşu da yeni çubuk alanındaydı. Zeyd'in açık kararı uygulandı: **“Görünüşlerini koru; üst kontrolleri güvenli alana indir, yeşil tuşu çubuğun gerektirdiği kadar yukarı al.”** Üst gruba @3x yaklaşık 118 px aşağı kaydırma, yeşil tuşa 233 px yukarı kaydırma uygulanır. En üst kontrol 132 px'de başlar; yeşil tuş 2121 px'de biter; alt çubuk 2334 px'de biter ve altta 102 px kalır. Ayrıntı: [home-layout.json](task19c/home-layout.json).

## Görsel kararlar

- Koleksiyon: süslü altın resim çerçeveleri kaldırıldı. Turkuaz kenar ve tek küçük altın köşe noktası kullanıldı. Kazanılan 11 gerçek tabloya yıldız yerine bölüm numarası yazıldı; 12 sıradaki, 13–15 lacivert ve kilitli. Galerinin turuncu/altın mimarisi krem taş ve koyu ahşap/turkuaza çevrildi; arka mimari sakinleştirildi.
- Başarılar: satırlarda altın çerçeve yok; krem yüz ve turkuaz hacimli kenar var. Altın yalnız başlık vurgusu ve ince madalyon halkasında. “Yıldız Avcısı” yerine **“Hızlı Parmaklar — 10 bölümü sürenin yarısı dolmadan bitir”**, yıldız yerine hız çizgili kronometre var. Tamamlanan iki görünür satırın tiki yeşil. 5/16, tüm listenin toplam durumunu temsil eder; görünür satırlar örnektir.
- Yeni alt çubuk: koyu turkuaz, dört eşit bölme ve ince ayırıcı; yalnız üstte ince altın çizgi. Yeni Ev/Koleksiyon/Jiggles/Kupa ikonları aynı üretim setinden. Seçili ikon **54,6 pt**, normal ikon **42 pt**: tam **%30 büyüme**. Açık turkuaz kabarcık altın değildir. Etiketler kabarcığın dışında, ikonun altında ve tüm görsel katmanların önündedir. Başarılar'da tek kırmızı rozet ve krem **1** vardır.
- Altın renk alanı kontrolünde aynı HSV eşiğiyle Koleksiyon 19B yaklaşık **%32,1 → %1,3**, Başarılar **%27,8 → %5,3** çıktı. Koleksiyonda resim hücrelerinin içleri ölçümden çıkarılır. Bu bir renk vekil ölçümüdür; malzemeyi tanımaz, sıcak resim/taş tonlarını da altın sanabilir. Gözle inceleme ayrıca yapıldı. Ev'in mevcut büyük altın kahraman çerçevesi, güncel görünüşü koruma talebi nedeniyle değiştirilmedi.

## Üretim, kaynaklar ve doğrulama

Görsel üretim **built-in image_gen** ile yapıldı; yeni sahne/panel/ikon yüzeyleri kodla düz çizilmedi. Kod yalnız üretilmiş parçaları, mevcut küçük Bölümler tuşunu, kilidi, kırmızı rozeti, gerçek tabloları ve yazıları birleştirir.

- [Üretim promptları](task19c/prompts.json); [galeri malzeme düzeltmesi](task19c/collection-material-correction-prompt.txt).
- `docs/art/task19c/sources/`: yeni yazısız sahneler, çubuk/ikon atlası, temiz koridor ve inceleme küçük görselleri. Seçilenler `koleksiyon-art-v2.png`, `basarilar-art-v2.png`, `nav-atlas.png`, `nav-support.png`, `home-clean.png`.
- `docs/art/task19c/layers/`: her ekranın metinsiz kompoziti, yeni metinler için ayrı şeffaf PNG ve özgün Unicode konum JSON'u. Ev'deki mevcut QA kontrollerinin zaten pişmiş yazıları, görünüşlerinin korunması için özgün piksel parçalarının içinde kalır; bu tur oyun sprite export'u değildir.
- Türkçe yazılar Fredoka ile, en az 15 pt/45 px çizilir. Depodaki SemiBold'a 1 px optik kalınlaştırma uygulanır. Eksik `ş/Ş/ğ/Ğ/İ`, fontun kendi çengel/yay/nokta glifleriyle birleştirilir; aksan şekilleri kodla çizilmez. Oyun entegrasyonunda tam Türkçe glifli Fredoka Bold gerekir.
- Yeniden üretim: `python docs/art/build-task19c.py` (Pillow, NumPy). Yalnız Task19C teslim klasörüne ve `docs/art/task19c/` çalışma alanına yazar. Task19/19B arşivlerine, C#'a, `Assets/_Game` dosyalarına ve Unity sahnelerine yazılmaz; mevcut font yalnız okunur.
- Dört ekranın ölçüleri, yeni metinlerin 45 px alt sınırı, üst 132/alt 102 px güvenli alanları, yıldız metni bulunmaması, yeni ikon oranı ve iki pilotta altın renk vekilinin %10 altında olması kontrol edilir. [validation.json](task19c/validation.json) çıktı SHA-256 değerlerini kaydeder. Beş son PNG görsel olarak da incelendi; yazı/çubuk çakışmaları ve rozet hizası düzeltildi.

Onay sonrasındaki kapsam: **02b, 03, 05, 06, 07**. Profilde Yıldız yerine Kalan süre ortalaması, ayarlarda güncel oyundaki dikey Ses/Müzik/Titreşim/Dil tuşları + yeşil Kapat düzeni esas alınacak. Bu ekranlar bu turda üretilmedi.
