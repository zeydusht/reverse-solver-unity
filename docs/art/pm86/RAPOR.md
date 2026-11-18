# PM_86 RAPOR

## 1. Koridor kayışı: çan eğrisi
- 1,5 sn aynı. Konum smootherstep (6t⁵ − 15t⁴ + 10t³):
  - hız başta ve sonda 0'a teğet, ortada tepe;
  - ivmede sıçrama yok.
- Tepe hızda hafif iz: şeridin iki kopyası, bir karenin bir parçası geride, en çok %22 saydamlıkla. Yalnız hızın üst yarısında görünür. Tablolar keskin kalıyor.
- **Parallax yapılamadı.** Koridor tek bir şerit resmi (`corridor_strip`); ayrı ön plan katmanı yok. Parallax için koridorun katmanlara ayrılmış çizimi gerekir, Codex işi.
- Ölçüm: koridor 1,50 sn, 90 kare, 20 ms'yi aşan kare 0.
- Grafik: `koridor_hiz.png` (önce plato, sonra çan).
- GIF'ler: `koridor_once_sonra.gif`, `koridor_once_sonra_yarim_hiz.gif`.

**Doğrusal tween denetimi** (54 tween tarandı; yumuşak eğriye çevrilenler):

| Yer | Ne | Önce | Şimdi |
|---|---|---|---|
| Badges (bomba patlaması) | büyüme | doğrusal | OutCubic |
| BoardView.SetImage | resim karışımı | doğrusal | smoothstep |
| BoardView çınlama | sallanma | parçalı doğrusal | her salınım smoothstep |
| BoardView çekiç parçası | büyüme/küçülme ve solma | doğrusal | OutCubic + smoothstep |
| Hud (+puan yazısı) | yükselme ve solma | doğrusal | OutCubic, karesel solma |
| Hud54 parça sayacı | küçülme | doğrusal | OutCubic |
| MenuTabs | iki sahne solması | doğrusal | smoothstep |
| GameRoot.Flash | solma | doğrusal | karesel |
| Screens konfeti | uçuş | doğrusal | yavaşlayan (OutQuad) |
| Alt çubuk | geçiş | doğrusal 0,15 sn | 0,25 sn ease-out-back |

**Bilerek sabit kalanlar:**
- çekiçle uçan parçanın dönüşü ve fırlatılışı (fizik: atış);
- ışıltının dönüşü;
- sürüklemede takılmaya hızlanma (t², bilerek).

Geri kalanların hepsi zaten eğriliydi: OutCubic, OutBack, Settle, smoothstep, sinüs.

## 2. Çizgiden tahtaya: ayrı katman yok
**Yöntem:**
- Dalışta tahta kurulunca gerçek parçalar düz resim olarak görünüyor. Kesikler, parçaların kendi kenarı (aynı malzeme, shader, yer ve ölçek); kalemle çizilen bir görünürlük maskesiyle açılıyor (`PieceView.SetReveal`, shader'da `_RvOn`).
- Çizgiler bitince (çekilmenin %70'i) parçalar kendi görünümünü 0,24 sn'de yumuşakça alıyor: kontrast, renk geçişi, kabartma, gölge, ayrışma.
- Kilit, bomba ve zincir rozetleri de bu görünümle birlikte geliyor.
- Resim parçaların altında yalnız aradaki kıl payı boşlukları dolduruyor. Kopya çizgi katmanı (PM_84) kaldırıldı.

**Ölçüm** (ekran kaydı, PC GPU, JPEG):

| Geçiş | Devir karesi farkı (%99,9 dilim) | En büyük | Parlaklık adımı | Komşu kare çiftleri |
|---|---|---|---|---|
| Kazanma → sonraki | 7/255 | 15/255 | +0,01 | 8–53/255 |
| Menü → bölüm | 6/255 | 14/255 | 0,00 | — |

- Fark görüntüsünde (x8) yalnız JPEG blok gürültüsü var; çizgi, ton ya da konum farkı yok (`devir_fark_kazanma.png`, `devir_fark_menu.png`).
- Kanal başına ≤ 2/255 ölçütü JPEG kayıtla kanıtlanamıyor: kaydın kendi gürültüsü 6–8/255.
- Dalış boyunca kareler arası en büyük ortalama parlaklık adımı 0,59/255 (ortalama 0,10).
- Ölçek ve konum: kamera tek eğri, tahta tek nesne.

## 3. Sandık
- Ekranın her yerine dokunmak sayılıyor: sallan, sallan, açıl.
- Ödüller uçarken dokunmak animasyonu 2,5× hızlandırıyor; ödüller kaybolmuyor, hepsi yerine iniyor.
- İlk sandıkta altta nabız gibi atan "Dokun!" yazısı (`sandik_dokun.png`).

## 4. Titreşim
- Harita: `docs/TITRESIM_HARITASI.md`.
- Eklenenler:
  - her düğmede Select ve tık sesi;
  - sandıkta artan sıra: Light, Medium, Heavy, ardından Success;
  - booster seçimi Select;
  - çekiç ve makas Medium (çekicin son vuruşu Heavy'den Medium'a indi);
  - değnek 3 hafif dalga;
  - saat Medium;
  - kayıp/süre Error (iOS'a yeni tür);
  - devam ve reklam ödülü Success;
  - Kont mührü Heavy.
- 50 ms kuralı ve Titreşim anahtarı aynı. Web'de iPhone sessizce yok sayıyor.

## 5. Müzik
- 5 CC0 aday, `&muzik=a…e`. Lisanslar ve döngü notları: `MUZIK_LISANS.md`.
- Bayraksız açılışta eski müzik çalıyor; Zeyd seçince varsayılan yapılır.
- Döngüler tıklamasız. Ses eski müzikle aynı seviyede, efektlerin altında.
- Canlıda doğrulandı: `&muzik=c` 128 sn'lik döngüyü çalıyor.
- Pixabay otomatik indirmeyi engelliyor. Mixkit'in lisansı oyunları dışlıyor.
- Ayarlar'da Müzik ve Ses zaten ayrı anahtar.
- Reklam sırasında ses duruyor; sayfa ya da uygulama arka plana gidince müzik duruyor (zaten vardı).

## 6. Küçük eksikler
**Yapılanlar:**
- Bölüm ortasında uygulama arka plana gidince süre duruyor; dönünce Durdur paneli açık geliyor.
- Altın ve booster sayaçları değişince kısa sayma animasyonu (menü altını, booster sayıları).
- Reklam düğmesine hızlı çift dokunuş ikinci reklamı açmıyor.
- Zor / çok zor şeridi dokununca hemen (0,12 sn) kapanıyor.
- Her düğmede tık sesi ve titreşim.

**Zaten vardı:**
- Kazanma, sandık ve kayıp düğmelerinde çift tetik koruması.
- İnternet yokken reklam düğmesi görünmüyor: reklam hazır değilse düğme hiç çıkmıyor, hata mesajı da yok.
- Düğmelerin basılı görünümü.

**Sessiz mod:**
- iOS ses oturumu "Ambient": iPhone'un yan anahtarına uyuyor, müzik de efektler de susuyor.
- Web'de Safari'nin WebAudio'su da aynı.
- Telefonda doğrulanmadı.

## 7. Alt menü sekmeleri: arka plan geç geliyordu (HATA)
**Sebep:**
- Sekme açılınca üst ve alt şeritler (`viewport-cover`) hemen çiziliyordu.
- Ortadaki sahne resmi ise hafızada hazır olsa bile 0,2 sn'de saydamdan beliriyordu.
- Ayrıca sahneler menüden 10 sn sonra yükleniyordu.

**Düzeltme:**
- Hazır resim aynı karede, tam görünür çiziliyor.
- Ön yükleme menüden 2 sn sonra başlıyor; resimler bellekte kalıyor.

**Test:** PlayMode, 3 sekme × 5 geçiş. Arka planı eksik kare 0.

## 8. Alt çubuk
- Codex 3 seçenek çizdi: A 9, B 8, C 7 (C `alt_menu/elenen/`). **A bağlandı:** lacivert şerit, parlak kraliyet mavisi seçili yüz, kalın altın kenar.
- Davranış:
  - krem ayrım çizgileri;
  - seçili sekme ortası sabit kalarak iki yana genişliyor; diğerleri kalan yeri paylaşıp daralıyor;
  - ikon kalkıp büyüyor, etiket beliriyor; arkasında sıcak ışık; diğerleri hafif sönük;
  - geçiş 0,25 sn, hafif geri tepmeli;
  - menünün içeriği basışla aynı anda değişiyor.
- GIF'ler: `alt_menu/alt_menu.gif`, `alt_menu/alt_menu_yarim_hiz.gif`.

## 9. Başarılar
- Codex 3 seçenek: A 9, B 8, C 6. **A bağlandı:** krem satır, mavi çerçeve, lacivert-altın madalyonlar, krem oluk içinde altın dolgu. Dokuz rozetin hepsi aynı dilde.
- Önce/sonra: `basarilar/once.png`, `basarilar/sonra.png`. Yalnız görsel.

## 10. Menü → bölüm dalışı
- Uçuş ve yaklaşma tek çan eğrisi (smootherstep), 2,2 sn. Önce 2,6 sn sürüyordu.
- Uçuşla yaklaşmanın bölündüğü yer, ekrandaki hızın kesintisiz kalacağı noktadan seçiliyor (0,33).
- Resimde kalış 0,4 sn, geri çekilme 0,8 sn. Toplam 3,41 sn (önce 3,81 sn).
- `&dalishiz=` çarpanı eklendi.
- Grafikler: `dalis_hiz.png`, `dalis_hiz_once.png`. 20 ms'yi aşan kare 1 (27 ms, bölüm kurulumu).

## 11. Tablo resmi çerçeveden bağımsız dönüyordu (HATA)
**Sebep:**
- Resim üç karışımla çiziliyordu:
  - menüdeki düz çizimle çerçevenin perspektifi arasında, bütün uçuş boyunca (`flat = 1 − w`);
  - kırpma, menünün kırpmasından ön kırpmaya kayıyordu;
  - yaklaşma başlar başlamaz, çerçeve henüz ekrandayken, tahta çerçevesine dönüşüyordu.
- Bu yüzden resim çerçevenin içinde bükülüp kayıyordu.

**Düzeltme:**
- Düz çizim ve kırpma geçişi ilk 0,25 sn'de bitiyor (kamera henüz kıpırdamamışken).
- Ondan sonra resim her karede çerçeve açıklığının aynı izdüşümüyle çiziliyor.
- Tahtaya dönüşüm ancak çerçeve ekrandan çıkınca başlıyor (1,78 sn).

**Ölçüm** (oyunun kendi günlüğü):
- Resim çerçevedeyken 92 karede resmin dörtgeni çerçeve açıklığıyla aynı: fark 0,0 px.
- Bu, aynı dörtgenden çizildiğinin kontrolü. Çerçeve çiziminin açıklığı ile bu dörtgenin örtüşmesi GIF'te gözle kontrol edildi.
- GIF'ler: `dalis_once_sonra.gif`, `dalis_once_sonra_yarim_hiz.gif`.

## Doğrulanmayan
- iPhone'da his: kayış, titreşimler, sessiz mod.
- Müziklerin kulağa nasıl geldiği.
- Kanal başına ≤ 2/255 kesin ölçümü (kayıpsız kayıt gerekir).
