# PM_MESAJ_114: U1 varsayılan + geri çekilmede kenar çizgileri çizilerek gelsin + sıçrama temizliği (önizleme)

Önizleme. **Push yok, yayın yok.** `pm108-onizleme` dalında devam et.

## Zeyd'in kararı ve geri bildirimi
- **U1 seçildi.** Dalış eğrisi U1 olarak kalacak; süresine ve şekline dokunma.
- **Kenar çizgileri:** Geri çekilirken tahtanın çevresindeki çizgiler (kenarlar) bir anda beliriyor. İstenen: bunlar da geri çekilme sırasında **çizilerek** gelsin, iç çizgiler gibi.
- **Sıçrama:** Anlık kaçma, kayma, zıplama olmasın. "Kusursuza yakınız, eksikleri düzenleyin."

## 1. U1 varsayılan
- Bayraksız oyunda dalış U1 olsun. `&u=` ve `&resimbekle=` test için çalışmaya devam etsin.

## 2. Bir anda beliren her şeyi bul
Önce teşhis yap, sonra düzelt.
- Dipten bitişe kadar U1'i kare kare kaydet.
- Bir karede birden beliren ya da kaybolan her öğeyi listele. Örnek: tahtanın dış kenarı, çerçeve hattı, duvar/engel çizgileri, hücre ızgarası, köşeler, gölge, üst arayüz (süre, butonlar).
- Ölçüt: öğenin görünürlüğü ya da opaklığı tek karede %30'dan fazla değişiyorsa "bir anda" sayılır.
- Raporda her öğe için yaz: ne olduğu, hangi karede belirdiği, düzeltme sonrası ne olduğu.

## 3. Kenarları çizerek getir
- Bulunan kenar ve çevre çizgilerinin hepsi geri çekilme boyunca çizgi gibi uzayarak gelsin, iç çizgilerle **aynı stil** ve aynı kalınlıkta.
  - Dıştaki çerçeve ise köşelerden ya da üst ortadan başlayıp çevre boyunca ilerlesin.
- **Zamanlama:**
  - Kenarlar dipten sonra başlasın.
  - İç çizgilerle birlikte, geri çekilmenin **%80'inde** tamamlansın. Hepsi aynı anda bitsin.
  - Çizim hızı yumuşak olsun: başta ve sonda hafif yavaşlasın, ortada sabit aksın.
- **Gerçek tahtaya devir:**
  - Çizilen kenarlar gerçek tahtadaki kenarla **piksel piksel aynı** yerde ve aynı renkte bitsin.
  - Devirde renk çakması, kalınlık farkı ya da kayma olmasın (PM_108'den beri geçerli kural).
- Çizgilerden **olmayan** öğeler (süre çubuğu, butonlar, Jiggle'lar gibi) bu PM'in konusu değil. Ama bunlar da bir anda beliriyorsa raporda listele, dokunma.

## 4. Sıçrama ve kayma taraması
Tüm dalışı (Oyna'ya basıştan oyun başlayana kadar) kare kare tara.
- **Hareketli öğeler:** Kamera, resim, çerçeve, tahta, çizgiler.
  - Her biri için konum, boyut ve dönme açısını kare kare çıkar.
  - Bir karedeki değişim komşu karelerin ortalamasının **2 katından** fazlaysa sıçrama say.
- **Kritik anlar:** Özellikle bunlara bak:
  - basış anı
  - kadraja ilk varış
  - dip
  - çizgiden tahtaya devir
  - animasyon bitişi ve oyunun başladığı ilk kare
- **Görüntü farkı:** Ardışık iki kare arasındaki piksel farkını çiz. Ani tepe veren her kareyi işaretle ve sebebini yaz.
- **Düzeltme:** Bulduğun her sıçramayı düzelt. Ama kapsam dar kalsın:
  - U1'in süresi ve eğrisi değişmesin.
  - Çerçeve, menü, renkler ve sesler değişmesin.
- **Uzun kareler:** Bölüm kuruluşundan gelen uzun kareler (20 ms üstü) bu taramada sıçrama gibi görünebilir.
  - Bunları ayrı listele.
  - Kurulumu birkaç kareye yayarak ya da dalıştan önce, basış anında hazırlayarak azaltabiliyorsan azalt. Bunu yaparken görüntü değişmesin.

## Çıktılar: `docs/art/pm114/`
- **`once_sonra.mp4`:** U1 şimdiki | U1 düzeltilmiş. Panel başına 390×844, 60 fps, sesli.
- **`once_sonra_yarim.mp4`:** Aynısı yarım hızda, yalnız dip → oyun başlangıcı bölgesi.
- **`u1_son.mp4`:** Düzeltilmiş hali tek başına.
- **`kenar_kareler.png`:** Geri çekilmeden eşit aralıklı 12 kare, şimdiki ve düzeltilmiş alt alta. Kenarların çizilerek geldiği görünsün.
- **`sicrama.png`:** Kare kare değişim ve piksel farkı grafikleri, önce/sonra; kritik anlar işaretli.
- **`RAPOR.md`:** Kısa:
  - bir anda beliren öğeler tablosu
  - bulunan sıçramalar ve düzeltmeleri
  - uzun kareler (önce / sonra)
  - toplam süre (1,60 sn kalmalı)

## Bitiş
- Push ve yayın yok. Mesajının en altına kısa özet yaz.
