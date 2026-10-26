# Ana menü — onaylı Level v2 sanat ailesi

İnceleme: `index.html`. Telefon ölçeğinde menü: `preview-menu.png`; @3x: `preview-menu@3x.png`. Tek sayfa toplu çizimler: `preview.png`. Grup görselleri: `preview-header.png`, `preview-nav.png`, `preview-controls.png`.

28 ayrı şeffaf PNG teslim edilir. Profil/avatar, kaynak sayaçları, ayarlar, kurtarılan ilerleme çubuğu, bölüm listesi kısayolu ve alt gezinme bileşenleri ayrı sprite'lardır. Dinamik yazı ve sayılar yalnız inceleme kompozisyonlarına eklenir. İki Level sprite'ı onaylı canonical v2 ile SHA-256 bakımından aynıdır.

`manifest.json`: her sprite'ın adı, kullanım yeri, @3x boyutu, L/R/T/B 9-slice ve kaynak dosyası. `Source/` yüksek çözünürlüklü üretim kaynağıdır; runtime atlasına yalnız manifest.assets listesi alınır. Para/kalp token'ları ortak sayaç setinin görsel parçalarıdır; ana menü incelemesi mevcut kurtarılan Jiggles ve yıldız göstergeleriyle hazırlanmıştır.

9-slice: profil kutusu L156/R48/T48/B48; sayaç hapı 44 px dört kenar; ilerleme izi L60/R60/T48/B48; dolgu L36/R36/T30/B30; alt gezinme L66/R66/T48/B48; onaylı Level tuşları 92 px dört kenar. İlerleme dolgusu oyun tarafından kırpılır/maskelenir. Önizleme metin dolgusu #F9F1BA; Level konturu koyu yeşil. Yazı tipi mevcut LilitaOne; Türkçe arayüz metinlerinde projedeki Türkçe destekli yazı tipi kullanılabilir.

Kaynak üretimi yerleşik image_gen ile yapıldı. Prompt seti: `docs/art/templates/menu-task13-prompts.md`. Dışa aktarım: `docs/art/build-menu-task13.ps1`; kompozisyon: `docs/art/render-menu-task13.ps1`; manifest ve teslim tabloları: `docs/art/sync-menu-task13.ps1`.

Yeni menü çizimleri kullanıcı değerlendirmesine sunulur; oyun entegrasyonu mühendislik aşamasıdır.
