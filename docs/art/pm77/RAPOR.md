# PM_77 RAPOR: eski koridor, 3D diorama yayı, adım adım dalış

**Özet:**
- p23 güncel (gh-pages fc734d1b). Build 38,1 MB Brotli; PM_76'ya göre −0,5 MB, çünkü PM_76'nın çizilmiş açıları kaldırıldı.
- Testler: EditMode 737/739 (2 atlandı, 0 hata), PlayMode 6/6. `verify_build` yerel 4/4, canlı 4/4.
- Ölçümler PC'de, gerçek GPU'lu başsız Chrome'da yapıldı (390×844). iPhone'da ölçülmedi.
- **Yay (2. madde): Zeyd onayı bekliyor.** Yalnız `&gecis=yay` bayrağıyla açılıyor; bayraksız oyunda yay yok.

## 1. Kazanma → sonraki bölüm: eski koridor (`gecis_kazanma_sonraki.gif`)
- **Koridor.** PM_74 öncesi (PM_62/64) kayış birebir geri geldi: aynı dosya sürümü, aynı eğri (smootherstep).
  - Süreler: 0,25 sn belirme, 0,35 sn bekleme, **3 sn kayış**, 0,3 sn sönme.
  - PM_76'nın parallax'ı, izi ve 1 sn'lik hızlı kayışı kaldırıldı.
- **Koridorun sonu.** Kayış tablonun tam karşısında bitiyor. Yay yok; doğrudan dalış başlıyor (aşağıdaki 3. madde).
- **Bölüm → menü** (`gecis_bolum_menu.gif`) aynı eski koridorla gidiyor. Menüye varınca PM_76'nın giriş animasyonu oynamıyor; menü olduğu gibi beliriyor (eski hâl).

## 2. Menü → bölüm: 3D diorama yayı (`yay.gif`, `yay_yavas.gif`) — **Zeyd onayı bekliyor**
- **Katmanlar (Codex, xhigh).** Menünün odası karşıdan çizilmiş düz katmanlar olarak üretildi:
  - duvar: tablo çerçevesiyle; kemerler, pilastrlar, flamalar ve aplikler; tuvali boş;
  - zemin: mermer ve kırmızı halı;
  - önde bağımsız bir sütun.
  - Kaynak ve kontroller: `docs/art/pm77/codex/`.
- **Gerçek derinlik.** Katmanlar 3D'de yerinde duruyor: duvar z=0'da, zemin önünde yatay, sütun duvardan 1 birim önde.
- **Gerçek kamera.** Kamera tablo merkezinin etrafında bir çember üzerinde gidiyor: **35°'den tam karşıya**, her an tabloya bakarak.
  - Her katmanın ızgara noktaları her karede perspektifle yeniden izdüşürülüyor. Bu yüzden perspektif ve parallax her karede doğru; ara çizime gerek yok.
  - Süre: yay **1,8 sn** (ease-in-out, smootherstep), ardından tam karşıda **0,2 sn** durma.
- **Başlangıç karesi = menü.** Bayrakla menünün arka planı dioramanın kendisi, başlangıç açısından görülüyor. Bu yüzden geçişte hiç çapraz geçiş yok; yay tam menünün gösterdiği yerden başlıyor.
  - Tablo menüdekiyle aynı yerde ve aynı boyda; kamera uzaklığı buna göre otomatik ayarlanıyor (3,49 tuval boyu).
  - Bugünkü çizili menüde tablo neredeyse karşıdan (~20°). Ona otursaydı dönüş 20°'nin altında kalıyordu ve gözle zor seçiliyordu (ilk denemede ölçüldü). Bu yüzden başlangıç 35° seçildi; menüdeki tablo biraz daha yandan görünüyor.
  - Bayraksız menü değişmedi.
- **Aynı oda.** Codex'in duvarında pilastrlar tablodan 0,4 birim uzaktaydı ve ekrana girmiyordu. Orta bölme, yalnız pilastr ile çerçeve arasındaki düz duvardan içerik duyarlı dikişle (seam carving) her yandan 110 px daraltıldı (`Tools/import_pm77_diorama.py`).
  - Çerçeve, pilastrlar ve pervazlar bozulmadı.
  - Artık flama ve aplikli pilastrlar tablonun hemen yanında, menüdeki gibi; sonda iki yanda görünüyorlar.
- **Kenar boşluğu.** Her katmandan, yalnız kameranın yay boyunca gördüğü kadarı kuruluyor; ekran kenarlarından atılan ışınlarla ve payla hesaplanıyor. Böylece hiçbir ızgara noktası kameranın yanına ya da arkasına düşmüyor.
  - Otomatik denetim (`&dalisbosluk=1`, magenta zemin, `Tools/gap_check.py`): yay + dalışta **382 karenin 0'ında** boşluk var.
  - İlk build'de sağ kenarda duvar–zemin köşesinde 25 karede küçük bir çentik vardı; zeminin genişliği eksik hesaplanıyordu. Düzeltildi.
- **Akıcılık.** Medyan 59,9 FPS. Bölümün kurulduğu anlarda 4–5 kare 20 ms'yi aşıyor; en kötüsü 50 ms.
- **Boyut.** Dokular crunch'lı DXT:
  - duvar 3908×1364, 530 KB;
  - zemin 4096×1024, 438 KB;
  - sütun 408×2048, 111 KB.
  - Yalnız bayrakla yükleniyor. iPhone DXT desteklemediği için orada açılmış hâlleri ~40 MB tutar; bayraksız oyuncu bunu taşımıyor.
- **Yedek yol (10+ Codex ara açısı) kullanılmadı.** Menü dioramanın kendisi olduğu için "aynı oda" başlangıçta kendiliğinden tutuyor. Zeyd reddederse sıradaki adım o.
- **Toplam (bayrakla, menüden).** Yay 2,0 sn + dalış 3,4 sn = **5,4 sn**.
- **GIF'ler.** `yay.gif` normal hız, `yay_yavas.gif` yarım hız.
  - Kayıt 60 fps. GIF biçimi 1/100 sn adımla çalıştığı ve tarayıcılar 2/100 sn altını yavaşlattığı için normal hız 50 kare/sn gösteriliyor.
  - Yarım hızda her kare tek tek, iki katı süreyle gösteriliyor (toplam 4,6 sn).

## 3. Dalış: tane tane (`gecis_menu_bolum.gif`, `dokunma.gif`)

| Adım | Süre |
|---|---|
| a. Tablonun karşısında yaklaşma, ekran resim olur | 1,1 sn (ease-in-out) |
| nefes | 0,15 sn |
| b. Yalnız resim (bölüm bu sırada kuruluyor) | 0,5 sn |
| nefes | 0,15 sn |
| c. Uzaklaşırken yapboz çizgileri ve çıkıntılar belirir (görünüm aynı, yalnız hız) | 1,0 sn |
| nefes | 0,1 sn |
| d. HUD üstten, booster çubuğu alttan | 0,4 sn |

- **Süre.** Toplam **3,4 sn**. Ölçülen: hareketin başından durmasına 3,5 sn.
  - Adımlar birbirine binmiyor: c bitmeden HUD başlamıyor.
  - Kazanmadan sonra aynı dalış eski koridorun ardından geliyor.
- **Dokunma.** İlk dokunuş 2× hızlandırıyor, ikincisi bitiriyor. Ölçüm: dalışın 0,5. sn'sinde bir dokunuşla geçiş ~2,0 sn'de bitti.
- **Bulunan eski hata (PM_75'ten beri).** Bölüm kurulduğu karede kamera tahtaya yakınlaşıyor, dalış katmanı ise o karede eski ölçekle çiziliyordu. Sonuç: tek karelik "resim 3 kat yakın" sıçraması (PM_76 kayıtlarında menü dalışında 1,6 sn'de, koridorda 2,0 sn'de var).
  - Katman artık bölüm kurulur kurulmaz yeniden yerleştiriliyor.
  - Tek karelik sapma ölçümü 5 geçişte **0**.
- **Kenar boşluğu.** Menü dalışında 274 karenin 0'ında, koridor + dalışta 560 karenin 0'ında.
- **Akıcılık.** Menü → bölüm medyan 59,9 FPS (en kötü 50 ms, 3–4 kare); koridor + dalış medyan 59,9 (en kötü 50 ms, 3 kare).

## Doğrulanmayan
- iPhone Safari'de yay ve dalış hissi, 4096 px dokuların belleği, FPS.
- Zeyd'in yay kararı: onay → bayrak varsayılan olur ve menü arka planı kalıcı olarak diorama olur; ret → yedek yol (Codex ara açıları).

## Açık
- Yay için Zeyd onayı (`&gecis=yay`).
- Sütun yalnız başlangıçta sol kenarda ince görünüyor; yayda hemen çıkıyor. Parallax'ı daha belirgin istenirse sütun içeri alınabilir ya da vazo/perde katmanı eklenebilir.
- Bölüm kurulum karesi hâlâ ~50 ms (3–4 kare); dalışta resim tam ekranken düştüğü için görünmüyor.
