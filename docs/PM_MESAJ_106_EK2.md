# PM_MESAJ_106_EK2: Dinleme sayfası iPhone'da boş görünüyor

**Sorun:** Zeyd iPhone Safari'de `p23/onizleme/sesler.html` sayfasını açıyor (24 Kasım 23:14). Başlık, açıklama ve sarı not kutusu görünüyor; alttaki "Seçimlerle oyunda dene" ve "Seçimi kopyala" düğmeleri de var. Ama **hiçbir ses listesi yok**: "Bugünkü seçim" dahil bütün bölümler boş. Sayfa bozuk.

**Muhtemel sebepler** (kontrol et):
- Listeyi kuran JavaScript Safari'de hata veriyor (Safari'nin desteklemediği bir sözdizimi ya da API);
- liste JSON'u (manifest) yanlış yolda ya da PM_104_EK dönüştürmesinden sonra dosya adları değişti (.ogg → .m4a) ve eşleşmiyor;
- `canPlayType` gibi bir biçim kontrolü bütün adayları eliyor;
- yeni bölümler (sürükleme, zincir, PM_106) eklenirken bir yazım hatası bütün sayfayı durduruyor.

**Düzeltme:**
- Playwright **WebKit** (iPhone 13 boyutu) ile sayfayı aç, konsol hatalarını oku, düzelt.
- Bir bölüm hata verse bile diğerleri görünsün: her bölüm ayrı kurulsun, hata o bölümde "yüklenemedi" yazsın.
- Her bölümde en az bir sesin WebKit'te gerçekten çaldığını test et (decode hatası 0).
- Sayfanın 390×844 ekran görüntüsü `docs/art/pm106/sesler_webkit.png`.

**Teslim:** canlı p23'te düzeltilmiş sayfa, commit + push. Son mesajda link: `p23/onizleme/sesler.html`. Sonra PM_106'ya devam.
