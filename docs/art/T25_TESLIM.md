# Task25 — Teslim ve kalite kaydı

A, B, C sırasıyla tamamlandı. Oyuna bağlama ve silme yapılmadı.

## A — 1–10

Mevcut10 konu sade ve tek odaklı stilde yeniden üretildi; Source1536×1536 PNG,1024×1024 JPG≤300000bayt. Manifest40 kaydı içeriyor; inceleme40 eski/yeni çift ve once-sonra-01-10.png hazır. Konular T24_KONULAR.mdye eklendi.60 rastgele6×7 örnek incelendi.1–3 için3×3/4×4 toplam75 parça tek tek incelendi.3ün iki mor alt parçası benzer bulundu; ikinci denemede geniş patika/gölge eklendi.

## B — Boş alanlar ve gece

-21: geniş bulut kümeleri, kar sırtları, küçük çam kümesi ve ayak izi.
-23: iri don desenleri, çam yansımaları, farklı buz/kar ton sınırları.
-24: bulut kümeleri, kar tümsekleri, farklı kıvrımlı kızak izleri, az çam silueti.
-30: bulutlar, kıyı kar tümsekleri, geniş dallı gölge, yansıma sınırları.
-40: kaya yosun lekeleri, çam gölgeleri, sis ve bulut katmanları; aynı zamanda gerçek gece.
-36/37/40 en pembe kalan üç sahne olarak seçildi.36da sıcak ev/soğuk bulut,37de ay ışığı ve küçük sıcak zemin yansıması,40ta ay ve sıcak pencere/fener. Konu ve kompozisyon korundu.
-Beş düzeltmede210 parçanın tamamı incelendi;36/37 için12 ek rastgele örnek incelendi. Tam parça levhaları docs/art/task25/qa/B altında. Eski v2PNG ve JPGler Denemeler/T25-B-oncesi altında önceki manifest SHA-256 değerleriyle doğrulandı. once-sonra-T25-duzeltmeler.png Task24→25 farkını gösterir.

## C — İki kart

-17-isim: Adın placeholder #92999E, yazılmış isim #0B2A36; isim-bos.png ve isim-yazildi.png. Input gerçek metin alanı olarak kalır, runtime PNGye yazı gömülmez.
-15-zor-bolum: aynı tek Baron, kolları kavuşturulmuş, kendinden emin, arkada karmaşık tahta. Yeni character-scene.png864×576 RGBA,288×192pt;9-slice yok. Yeni sahne çiviye işaret etmez.
-Eski kart dosyaları her ailenin T25-oncesi altında. NIHAI/T25-inceleme.html iki durumu ve yeni sahneyi gösterir. NOT/deneme/secim JSONları ve T23_KOD_LISTESI güncellendi.

## Üretim, limit ve sınırlar

Yerleşik image_gen kullanıldı:10 yeni tablo +3ün bir düzeltmesi +7 B düzenlemesi +1 Baron =19 çağrı. Tam istemler docs/art/task25/generations.json ve baron-generation.json içinde. Haftalık kalan kota araçlarda gösterilmiyor; sayı uydurulmadı. Kota hatası olmadı, yarım kalan iş yok. Araçların ürettiği kareler1536ya LANCZOS ile normalize edildi; doğal1536 ayrıntı iddia edilmiyor. İşlenmemiş üretimler korundu.

Küçük ekran/Unity/oynanış testi yapılmadı. Sakin kar, bulut ve gölge parçaları benzer ailelerde kalsa da biçim veya ton sınırları görsel kontrol edildi; bütün parçaların eşit kolaylıkta olduğu iddia edilmiyor. Kontrol renk/biçim okunurluğudur, otomatik bir çözülebilirlik garantisi değildir.

Mevcut Paintings klasöründeki 199 dosyanın tümü başlangıç SHA-256 değerleriyle aynı. İnceleme bağlantıları,40PNG/JPG boyutları, JPEG sınırı, manifest ve sprite hashleri doğrulandı.
