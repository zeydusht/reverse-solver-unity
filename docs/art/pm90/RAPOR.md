# PM_90 RAPOR

Ölçümler yerel build'de yapıldı (Chrome, gerçek GPU, 390×844, 60 fps). iPhone'da doğrulanmadı.

## 1. PM_89 §1 geri alındı

- Dalış, çerçeve, menü, koridor, menü ve koridor resimleri (`oda.png`, `corridor_strip.png`) PM_88'in son hâline döndü (git'ten, `741df153`). Oyunun çizdiği çerçeve (`Frame89`, `frame89.png`) silindi.
- Kalanlar: Jiggles ekranı, buzlu bar, yeşil sürenin kalkması. Tahtanın resmi eşit ölçekli kırpımda kaldı (PM_89'dan). 3. madde bunu gerektiriyor: tahta resmi çekiştirirse geri çekilmede ezilme kaçınılmaz.
- Doğrulama `geri_alma.png`:
  - Menü PM_88 build'iyle piksel piksel aynı (ortalama fark 0,0).
  - Koridor da aynı (fark 0,02–0,18 / 255; JPEG ve zamanlama).

## 2. Buzlu bar yönü

- Bar sağdan sola azalıyor: sol uç sabit, sağ uç sola çekiliyor.
- PlayMode testi iki şeyi kontrol ediyor: sol uç yerinde mi, sağ uç geri çekiliyor mu.

## 3. Dalış sonundaki anlık dalgalanma

**Sebep:** PM_88'de resim yaklaşmanın sonunda çerçevedeki hâlinden tahtanın hâline çevriliyordu. Bu çeviri eşit ölçekli değildi:

- Menüdeki çerçeve resmi duvar düzleştikçe ~%7 yanlara açılıyor.
- Tahta ise kare resmi kendi oranına çekiştiriyordu.

Bu, varıştan hemen önce kare başına %8'lik oran sıçraması yapıyordu.

**Şimdi:**

- **Yaklaşma ve varış PM_88'deki gibi.** Resim çerçevede nasıl görünüyorsa öyle kalıyor; son anda hiçbir dönüşüm yok.
- **Tahta, varan resmin tam altında kuruluyor.** Tahtanın boyu çerçeve açıklığının boyu, ekranı kaplıyor. Tahtanın parçaları resmi çerçevedekiyle birebir aynı gösteriyor, kenarlardan taşan kısım dahil. Devir anında ekranda hiçbir şey değişmiyor.
- **Geri çekilirken** resim, varıştaki hâlinden tahtanın kendi kırpımına geçiyor:
  - geçiş geri çekilmenin tamamına yayılıyor (0,8 sn);
  - hız eşit, başta ve sonda yumuşuyor (ilk ve son %15);
  - oran değişimi her karede eşit.
  - Çizgiler tahtanın gerçek kenarları, kaymıyor. Parçalarla altlarındaki resim aynı dönüşümü kullanıyor.

### Ölçüm

İki bağımsız ölçüm yapıldı:

- **Oyunun kendi sayıları** (`Tools/pm90_pullback_check.py`): tahtanın ekrandaki köşeleri ve resmin dönüşümü.
- **Karelerden** (`Tools/pm90_feature_check.py`): ardışık karelerde SIFT noktaları eşleniyor ve RANSAC ile afin uydurma yapılıyor. Çizilen kesikler aykırı nokta sayılıyor.

Eski alan uydurması (`pm89_aspect_check.py`) bu süreçte güvenilir değil: çizilen kesikleri ve gölgeleri oran değişimi sanıyor.

| Dalış | Tahta | X/Y kare başına en kötü (kareler, varıştan −1 ile +1,2 sn arası) | X/Y kare başına (oyunun sayısı) | Görünen kırpım, geri çekilme | Resmin kendi ölçek adımı | Kenar boşluğu, geri çekilme |
|---|---|---|---|---|---|---|
| **Önce:** PM_88 kazan → 3 | 4×5 | **0,916** (varıştan 0,35 sn önce) | – | – | – | – |
| Menü → 5 | 4×5 | 0,997 | 0,998 | u 0,42 → 0,80, v 0,97 → 1, tek yönde | %0,24/kare | 0 |
| Menü → 13 | 5×6 | 0,996 | 0,998 | u 0,42 → 0,83, v 0,97 → 1, tek yönde | %0,23/kare | 0 |
| Menü → 8 | 6×6 kare | 0,998 | 0,998 | u 0,42 → 1,00, v 0,98 → 1, tek yönde | %0,27/kare | 0 |
| Kazan → 3 | 4×5 | 1,001 | 0,999 | u 0,46 → 0,80, v 0,99 → 1, tek yönde | 0 | 0 |
| Kazan → 13 | 5×6 | 1,001 | 1,001 | u 0,46 → 0,83, v 0,99 → 1, tek yönde | 0 | 0 |

- **Koridor yolu:** çerçevedeki resim zaten tahtanın oranında varıyor. Geri çekilmede yalnız kamera uzaklaşıyor, oran hiç değişmiyor.
- **Menü yolu:** varan resimle tahta arasında toplam %6,5'lik oran farkı var. Bu fark 48 kareye yayıldı: kare başına en çok %0,2–0,4. PM_88'de aynı fark yaklaşmanın son birkaç karesine sıkışıyordu.
- **"Ölçek değişimi < %1/kare":**
  - Resmin kendi yeniden dizilmesi kare başına en çok %0,27.
  - Kameranın geri çekilmesi PM_85'ten beri aynı ve dokunulmadı; o, kare başına %2,5–3,2 küçültüyor. Bu eşit ölçek, oranı değiştirmiyor.
- **Kenar:**
  - Varışta tahta ekranı tam kaplıyor (kaplama çerçevesinin ×1,000'ı).
  - Geri çekilmede bütün karelerde kenar boşluğu 0. Ölçüm `&dalisbosluk=1` ile yapıldı: açıkta kalan yer eflatun görünür.
- **Önce/sonra kareleri:** `varis_once_sonra.png`, 20 fps, varıştan −0,05 ile +1,1 sn arası. Önce: PM_88 menü → 1 ve kazan → 3. Sonra: yukarıdaki beş dalış.

## Kapsam dışında görülen

- Menü dalışında uçuşun başında, dokunuştan 0,2 sn sonra 0,18 sn boyunca ekranın sağ alt köşesinde küçük bir açık kalıyor (en çok ~130 px).
  - Normal oyunda lacivert görünür.
  - PM_88'de de aynısı var.
  - Uçuş yoluna dokunulmadığı için düzeltilmedi.

## Doğrulanmayan

- iPhone Safari'de his.
