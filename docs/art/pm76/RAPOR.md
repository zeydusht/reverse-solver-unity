# PM_76 RAPOR: geçişler pürüzsüz (yay çizen kamera, akıcılık turu, değnek kararı)

**Özet:** p23 güncel (gh-pages 8713f0de; build 38,6 MB Brotli, PM_75'e göre +1,6 MB). EditMode 737/739 (2 atlandı, 0 hata), PlayMode 6/6. `verify_build` yerel 4/4, canlı 4/4. Ölçümler PC'de, gerçek GPU'lu başsız Chrome'da; iPhone'da ölçülmedi.

## 1. Tablonun karşısına yay çizen kamera (`dalis_yay.gif`, `acilar.png`, `gecis_menu_bolum.gif`)
- **Açılar.** Codex (xhigh, `CODEX_PM76.md`) aynı odayı 4 açıdan, katmanlı olarak çizdi:
  - a0: bugünkü menü görünümü, değişmeden; yalnız ön plan ve duvar olarak ayrıldı.
  - a1 25°, a2 12°, a3 tam karşıdan.
- **Takım seçimi.** Codex iki takım üretti (`acilar.png`, `acilar_b.png`). İkisi de tutarlı: aynı tablo, çerçeve, sütunlar, flamalar, halı ve ışık. **A takımı** kullanılıyor. Codex'in denetimine göre a0 ve koridor katmanları birleşince, tuval alanı dışında özgün görüntüden 0 piksel fark var.
- **Yöntem: düz resim dönmüyor.**
  - Kamera yay boyunca iki komşu açıyı aynı anda gösteriyor. İkisi de çerçevelerinin köşeleri aynı ara dörtgende buluşacak şekilde bükülüyor, yenisi eskisinin üstünde beliriyor. Böylece çerçeve hiçbir anda çift görünmüyor.
  - Bölümün resmi duvarın kesik tuvalinin altında; ön plan duvardan hızlı kayıyor.
- **Hareket.** Yay 0,85 sn (ease-in-out). Yaklaşma yayın son %28'inde, durmadan başlıyor ve 0,7 sn sürüyor. Ardından:
  1. yalnız resim 0,45 sn,
  2. çizgiler ve tahta 0,6 sn; sonunda tahta %2,5 yaylanarak oturuyor,
  3. HUD'un son 0,2 sn'si.
  - Toplam **2,56 sn** (sınır 2,6).
  - Bir dokunuş 3 kat hızlandırıyor, ikincisi bitiriyor.
- **Kenar boşluğu.** Katmanlar her yönde %8 büyük çiziliyor; taşan kısımlar kenar piksellerinin uzatılmasıyla dolduruluyor. Otomatik denetim (`Tools/gap_check.py`, `&dalisbosluk=1` ile magenta zemin), geçişin her karesinde ekran kenarındaki boşluk piksellerini sayıyor:
  - menü dalışı: 128 karenin **0**'ında,
  - koridor + dalış: 162 karenin **0**'ında.
  - (İlk denemede 15 karede vardı; sebebi bir `Lerp` kırpmasıydı, düzeltildi.)
- **Boyut.** Açılar build'e önce 7,5 MB ekliyordu. Proje içe aktarma kuralı (`MenuArtImport`) dokuları 1024'e kırpıp sıkıştırmasız bırakıyordu. pm76 klasörüne ayrı kural yazıldı (crunch'lı DXT): her açı yaklaşık 250 KB, toplam artış **+1,6 MB**.
- **Ön yükleme.** Dalışta yükleme takılması olmasın diye açılar, koridor katmanları, panel görselleri ve yazı tipleri menü boştayken (0,8 sn sonra) birkaç kareye yayılarak yükleniyor. Oyuncu bu bitmeden dokunursa dalış eski karşıya-dönme yolunu kullanıyor.

## 2. Koridor (`gecis_kazanma_sonraki.gif`, `gecis_bolum_menu.gif`)
- Kayış 3 sn'den **1 sn**'ye indi. Eğri aynı aile (smootherstep).
- Ön plan katmanı: sütunlar, flamalar, lambalar ve saksılar (Codex `koridor/fg`, `koridor/wall`). Her bölmeye bir sütun düşecek şekilde tekrarlanıyor ve duvarın iki katı hızla kayıyor (parallax). Başta ve sonda sütunlar özgün yerlerinde.
- Hızlı anda şeridin arkasında hafif bir iz var (hareket bulanıklığı).
- Bir sonraki bölüme dalış, kayışın %80'inde, şerit hâlâ hareket ederken devralıyor: arada durma yok.
- Kazanmadan menüye: 1 sn kayış, menü altında belirirken giriş animasyonu.

## 3. Akıcılık turu
Her geçiş önce/sonra GIF'iyle kaydedildi. GIF 50 fps; GIF biçimi 60 fps'i göstermiyor, kayıt 60 fps.

| Geçiş | Önce: süre / en kötü kare | Sonra: süre / en kötü kare | Eğri |
|---|---|---|---|
| Açılış → menü (`gecis_acilis.gif`) | anında / **217 ms** (isim girilince menü baştan kuruluyordu) | 0,6 sn / **17 ms** | üst çubuk iner, düğme ve sekmeler çıkar; %4 yaylanma, üst üste binmeli |
| Menü → bölüm (`gecis_menu_bolum.gif`) | 2,5 sn (düz homografi) / 50 ms | 2,56 sn yay + dalış / 33 ms × 3 (*) | ease-in-out; yaklaşma yayla %28 çakışık |
| Kazanma ekranının gelişi (`kazanma_gelis.gif`) | 1,15 sn / 17 ms | 1,0 sn / 17 ms | ease-out, başlık ve düğmeler %5 yaylanma |
| Kazanma → sonraki (`gecis_kazanma_sonraki.gif`) | 3,9 sn koridor + 2,5 sn dalış / **100 ms** | 1 sn koridor + dalış (≈ 3,1 sn) / 34 ms × 2 (*) | smootherstep; %80'de devir |
| Bölüm → menü (`gecis_bolum_menu.gif`) | anında / 67 ms | 0,6 sn (lacivert örtü erir, menü gelir) / 33 ms × 1 (**) | ease-out, %4 yaylanma |
| Durdur (`panel_durdur.gif`) | anında aç-kapa / 33 ms | açılış 0,34 / kapanış 0,24 sn / 33 ms × 1 (**) | büyüme %88 → %100, %5 yaylanma; kapanış ease-in-out |
| Ayarlar (`panel_ayarlar.gif`) | anında / 33 ms | 0,34 / 0,24 sn / 33 ms × 1 (**) | aynı |
| Devam (`panel_devam.gif`) | 0,7 sn / 17 ms | 0,55 sn / 17 ms | ease-out, başlık ve düğme yaylanması |
| Sandık (`panel_sandik.gif`) | — / 33 ms | — / **17 ms** | (ön yükleme) |
| Booster seçimi (`booster_secim.gif`) | anında giriş, 0,1 sn doğrusal çıkış / 33 ms | giriş 0,3 sn, başlık %5 yaylanarak iner; çıkış 0,24 sn ease-in-out / 33 ms × 1 (**) | ease-out / ease-in-out |

(*) Bu kareler dalışın "yalnız resim" beklemesinde oluyor: ekran durgunken arkada bölüm kuruluyor, göze görünmüyor. Ölçüm zamanları: dokunuştan 1,71–1,81 sn ve 1,93–1,98 sn sonra.
(**) Tek kare, dokunulan karede panel kurulurken oluşuyor (dokunuştan 0,13–0,18 sn sonra). Panelin solarak açılması o kareden sonra başlıyor.

**Hedef karşılanmadı.** "Hiçbir kare 20 ms'yi aşmasın" hedefine bu iki durumda ulaşılamadı: hareket sırasında 20 ms üstü kare yok, ama bu kurulum anlarında var. Durdur kartının kurulumu PC'de 21 ms sürüyor. Bunu düşürmek için yapılanlar:
- ortak yazı malzemesi,
- tek adımlık yazı sığdırma,
- görsellerin önceden yüklenmesi.

Kalanı için panelleri önceden kurup saklamak gerekir; bu sürümde yok.

Diğer ayrıntılar:
- Doğrusal hareket kalmadı.
- Ard arda gelen adımlar üst üste biniyor. Tek istisna dalıştaki bilinçli 0,45 sn "yalnız resim" beklemesi.

## 4. Değnek (PM kararı, varsayılan) — `degnek_duzlestirme.gif`
- Değnek önce bugünkü gibi tırnakları yeniden dizmeyi deniyor.
- Olmazsa seçilen bölgede, en çok parçayı serbest bırakan parçanın (eşitlikte en çok komşuyu tutanın) bütün tırnaklarını ve oyuklarını **düzleştiriyor**. Parça dikdörtgen oluyor ve en az 1 parça serbest kalıyor.
- Düz kenar hiçbir yere takılmadığı için tahta çözülebilir kalıyor.
- **Ölçüm (445 ara durum).** Sıkışık parça bulunan 430 durumun **429**'unda değnek bir şey yapabiliyor (PM_75'te durumların %25'i). Soluk görünüş ile sunulan seçenek hiçbir durumda çelişmiyor.
- **PlayMode:** 13 bölümün **13**'ünde bölüm başında bile değnek bir parçanın tırnaklarını düzleştirip 1–2 parçayı serbest bırakıyor.
- **Animasyon:** düzleşen eklemlerde ışık parlıyor. Eski çerçeve altın renginde ısınıp 0,95 sn'de soluyor; geriye düz kenar kalıyor. Serbest kalan parçalar iki kez altın renginde yanıp sönüyor.
- `&degnek=eski` bayrağıyla PM_75 davranışı (bölüm başında soluk düğme ve açıklama) geri geliyor; bu da PlayMode'da denetleniyor.

## Bulunan ve düzeltilen hata
Yeni panel animasyonunda Durdur kartı "Devam"dan sonra ekranda kalıyordu: kart mantık olarak kapanıyor, görüntü kalıyordu. Ara build'de yakalandı ve düzeltildi. PlayMode'a "Devam'dan sonra Durdur paneli 1 sn içinde ekrandan gider" denetimi eklendi.

## Açık
- iPhone'da his ve kare süreleri ölçülmedi.
- Panel kurulum karesi (tek 33 ms) için paneller önceden kurulabilir (bir sonraki iş).
