# PM107 altın teknik kontrolü

25 Kasım 2026. Üretim: yerleşik `image_gen`; dışa aktarma, saydamlık temizliği, küçük boy rötuşu ve montaj: Pillow / NumPy. Çalışma yaklaşık 20 dakika içinde tamamlandı. İstemler [uretim_promptlari.json](uretim_promptlari.json), sayısal sonuçlar [teknik_olcumler.json](teknik_olcumler.json) içinde.

## Boyut, merkez ve saydamlık

Sınır kutuları, alfa > 0 olan alan için `(sol, üst, sağ, alt)` biçimindedir; sağ ve alt sınırlar hariçtir. Piksel merkezine göre hesap `(sol + sağ − 1) / 2` şeklindedir. 512 tuvalin merkezi `(255.5, 255.5)`, 64 tuvalin merkezi `(31.5, 31.5)` olur.

| Dosya | Boyut / kip | Alfa sınır kutusu | Alan merkezi | Tuval merkezinden sapma | Bağlı şekil |
|---|---|---|---|---|---:|
| A_coin.png | 512×512 RGBA | (35,32,477,480) | (255.5,255.5) | (0,0) px | 1 |
| B_coin.png | 512×512 RGBA | (39,32,472,480) | (255,255.5) | (−0.5,0) px | 1 |
| C_coin.png | 512×512 RGBA | (49,32,462,480) | (255,255.5) | (−0.5,0) px | 1 |
| final_coin.png | 512×512 RGBA | (41,32,471,480) | (255.5,255.5) | **(0,0) px** | 1 |
| final_coin_64.png | 64×64 RGBA | (5,3,59,61) | (31.5,31.5) | **(0,0) px** | 1 |

B ve C'deki yarım piksel yatay sapma, tek sayılı görünür genişliğin çift sayılı tuvalde tam piksele yerleştirilmesinden gelir. Finalin iki boyutu tam merkezlidir. Görünen şekiller kenarlara değmez; bütün sprite köşeleri saydamdır.

**Gerçek alfa ile üretildi; #FF00FF zemin gerekmedi.** Beş sprite dosyasının her birinde görünür tam magenta `(255,0,255)` pikseli **0**, yakın magenta (`R>200, G<100, B>200, A>0`) pikseli **0**. Tam saydam piksellerde sıfırdan farklı RGB sayısı da her dosyada **0**; renkli saydam kenar yükü yok.

| Dosya | Tam saydam piksel | Kısmi alfa pikseli |
|---|---:|---:|
| A_coin.png | 108008 | 1560 |
| B_coin.png | 110789 | 1547 |
| C_coin.png | 124689 | 1520 |
| final_coin.png | 111230 | 1942 |
| final_coin_64.png | 1657 | 361 |

Düşük opaklık kalıntıları temizlendi; ana şekle bağlı olmayan bileşenler çıkarıldı. Nihai dosyalarda 8 komşulukla sayılan bağlı bileşen sayısı **1**. Kenardaki kısmi alfa, yumuşak örnekleme içindir.

## İyileştirme turu ve 64 px rötuş

B'den ayrı bir üretim turu yapıldı: yüzey gürültüsü azaltıldı, oyuk / kabartma ayrımı artırıldı, parıltı daha geniş ve kontrollü hale getirildi. Final dış konturu yaklaşık 4 px sıcak koyu `#8A6420` desteği aldı. Ana renk ailesi `#E3B54E` / `#F6D36B`; ışık ve gölge doğal ton geçişleri içerir.

64 px dosya yalnız otomatik küçültme değildir: Lanczos küçültmesinden sonra RGB üzerinde elle ayarlanmış keskinlik (`radius=0.65`, `percent=145`, `threshold=3`) ve bir piksellik iç kenarda %22 umbra karışımı uygulandı. Alfa keskinleştirilmedi; düşük alfa saçakları ayıklandı. [24/32/64 px karşılaştırması](okunurluk_kontrol.png), üç zeminde gözle incelendi. 24 pt değerlendirmesinde 96 dpi karşılığı 32 px kullanıldı; ayrıca 24 px daha zorlayıcı örnek olarak kontrol edildi.

## Ekran montajları

`A_onizleme.png`, `B_onizleme.png`, `C_onizleme.png` ve `final_onizleme.png`: **2340×1688 RGB**, soldan sağa menü / kazanma / süre doldu. Her panel özgün ekranın **780×1688** ölçüsündedir. Dört konumda teslim edilen ilgili PNG'nin kendisi yer alır: menü, +50 ödülü, devam düğmesi ve alttaki bakiye.

Menüde eski krem sayaç kapsülü kaldırıldı; düz `999999`, mevcut Baloo2 ExtraBold yazı tipiyle krem `#FFF0C3` ve koyu `#123743` konturla çizildi. Sayaç yanında `+` yok. Ödülün `+50` ve sürenin `+30 sn` ifadeleri korundu.

Yapay zekâ ile temizlenen zeminlerden yalnız para bölgeleri alındı; menü zemin geçişi komşu duvar renklerine uyarlandı. Özgün ekranlarla piksel karşılaştırmasında aşağıdaki bölgeler dışında değişen piksel sayısı, **dört önizlemenin üç panelinin tamamında 0**:

- Menü: (424,48,645,149).
- Kazanma: (414,1076,499,1161).
- Süre doldu: düğme (136,1141,644,1291), bakiye altını (252,1308,309,1365).

Önizlemeler görsel montajdır; oyuna entegrasyon yapılmadı. [Yerleşim detayı](yerlesim_kontrol.png) ve [final montaj](final_onizleme.png) gözle incelendi. Bütün teslim dosyaları 10 MB altındadır.

## Kapsam

Yalnız `docs/art/pm107/codex/altin/` altında görsel, Markdown rapor ve JSON kayıtları yazıldı. `GOREV.md` korunmuştur. Oyun varlıkları ve kodu değiştirilmedi; kod dosyası, tarayıcı profili, önbellek veya sanal ortam oluşturulmadı.
