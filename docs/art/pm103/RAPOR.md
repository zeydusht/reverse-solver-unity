# PM_103 RAPOR: sesler kesinleşti

## 1. Seçimler tek yerde

`Assets/Config/Resources/ses.json`: Zeyd'in 24 Kasım 06:51 tablosu.

| Ses | Seçim |
|---|---|
| Lobi | n |
| Bölüm | E |
| Kazanma | g |
| Kazanma tınısı | **marimba** |
| Çıkış | f |
| Çarpma | a |
| Tık | b |
| Makas | **c** |
| Değnek | a |
| Çekiç | a |
| Saat | a |
| Koridor | a |
| Dalış | a |
| Çizgi | a |
| Geri çekilme | a |
| Sekme | a |

- Bütün ses kodu varsayılanını bu dosyadan okuyor. Kodda başka bir varsayılan kalmadı.
- Bayraklar (`&lobi=`, `&kazanmaton=`, `&sesmakas=` …) test için duruyor. Web'de (p23) çalışıyor.
- Bir seçimi değiştirmek için yalnız bu dosyadaki harfi değiştirmek yeter: web, iOS ve testler onu izler.

## 2. iOS build

- **Müzik build'in içinde.** Lobi (Urban Shop), bölüm (Cat in Space) ve kazanma fanfarı (g, marimba) uygulamanın kendi dosyası. İnternetten indirilmiyor, çevrimdışı çalışıyor.
- **Geçişler web'deki gibi:**
  - menüde lobi müziği;
  - dalışta 0,6 sn çapraz geçiş;
  - panelde müzik %35;
  - kayıpta %40;
  - kazanmada müzik 0,15 sn'de kesilir, fanfardan sonra lobi kaldığı yerden 1 sn'de gelir.

  Eskiden iOS'ta tek bir eski müzik çalıyordu.
- **Seçilmeyen adaylar build'de yok.** Build sırasında 153 aday ses dosyası dışarıda kalıyor, build biter bitmez yerine dönüyor. Build'in kendi listesine göre:
  - build'de 64 ses dosyası var;
  - aday klasörlerinden yalnız seçilen 50'si girdi;
  - seçilmemiş aday 0;
  - seçilmemiş bir aday girerse build durur.
- **Boyut** (mağaza build'i, Xcode projesinin `Data` klasörü):

  | | Önce | Sonra | Fark |
  |---|---|---|---|
  | Data (bütün oyun verisi) | 105 761 KB | 105 653 KB | **−108 KB** |
  | Ses dosyası (`resources.resource`) | 1 784 KB | 1 697 KB | −87 KB |

  - Seçilmeyen ~150 aday çıktı. Yerine iki müzik girdi: lobi 790 KB, bölüm 619 KB (Vorbis). Fanfar 63 KB.
  - Eski tek müzik de çıktı.
  - Sonuçta build biraz küçüldü.

## 3. Web

- Oyun açılırken yalnız seçilen sesler yükleniyor (51 dosya: efektler, arayüz, booster, geçiş sesleri ve fanfarın vuruş haritası).
- Müzikler eskisi gibi GitHub Pages'ten akıyor: önce lobi, sonra bölüm, sonra fanfar (g-marimba).
- Adaylar web build'inde duruyor, ama yalnız bayrakla açılıyor. Bayraksız oyun onları yüklemiyor. Dinleme sayfasından dinlenip seçiliyor.
- Dinleme sayfası `p23/onizleme/sesler.html`: "varsayılan" etiketi artık marimba ve makas c'de.

## 4. Lisanslar

`docs/SES_LISANSLARI.md`: yalnız oyunda çalan sesler; her birinin kaynağı, yazarı, lisansı (CC0 ya da bizim üretimimiz) ve linki. Açılış filminin sesleri de içinde.

## Doğrulama

- EditMode testleri: seçimler dosyada; her ses sınıfının varsayılanı seçim; açılışta yalnız seçilen dosyalar yükleniyor; iOS'un dışarıda bıraktığı liste; gömülecek müzik dosyaları ve uzunlukları; build yarıda kalırsa adaylar dışarıda unutulmuş mu.
- PlayMode testi: iOS müziği (editörde aynı dosyalar):
  - menüde lobi;
  - dalışta iki müzik birlikte, sonra yalnız bölüm;
  - panelde %35;
  - fanfar marimba, müzik susar;
  - fanfardan sonra lobi geri gelir.

## Doğrulanmayan

- Gerçek iPhone'da müzik ve geçişler. Xcode projesi üretildi, Mac'te derlenip telefonda dinlenmedi.
