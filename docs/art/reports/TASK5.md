# Görev 5 ve 5b teslimi

89 ayrı @3x PNG (4.95 MB / 4.72 MiB), tek seçenek. Önizleme: [review.html](../../../Assets/Art/UI/Task5/review.html). Toplu kontrol: [contact sheet](task5-contact-sheet.png).

- #26–48: `Assets/Art/Gameplay/Task5/` ve `Assets/Art/UI/Task5/`. #36 karartma için envantere uygun olarak yalnız `#19252D`, opaklık 0.90 verilir. Tam ekran #26 opaktır; sprite'lar şeffaftır.
- #49: mevcut `PieceSkin/skin.json` ve `body-256.png` korunur. Parçaya bağlı gölge, seçili parlama ve sürükleme aynı prosedürel skin sözleşmesinden uygulanır; dikdörtgen gölge sprite'ı üretilmez.
- #20–25: ortak kart, birincil/ikincil düğmeler, dört bölüm hücresi, isim girişi ve ilerleme iz/dolgusu ayrı dosyalardır. Sayılar/yazılar oyunda eklenir; puan 1–10 için ortak yazısız düğmenin üç durumu verilir.
- 5b: profil iki durum, avatar, ayarlar dişlisi iki durum, satır/anahtar ve ses açık/kapalı, başlık normal/zor, hedef, altı konfeti ve yıldız ışık halkası; kalan ikonlar, dört zincir disk rengi, resim çerçevesi ve puan düğmeleri tamamlandı. Dolu/boş/dolum yıldızı, bomba normal/sıcak parlaması da dahildir.
- Dikey tutorial eli önceki şeffaf kaynaklardan kayıpsız dışa aktarıldı. Parmak ucu/pivot: tap (151,16), swipe (136,15) px; PNG üst sol koordinat başlangıcı. Unity normalize pivotları tap (0.5243,0.9444), swipe (0.4722,0.9479).

Dosya başına ölçü, tekrar ekseni ve Unity sol/alt/sağ/üst piksel sınırları [manifest.json](../../../Assets/Art/UI/Task5/manifest.json) ve [HANDOFF.md](../HANDOFF.md) içindedir. Profil kutusunda köşe 36 px; avatar yuvası yatay esnemesin diye sol sabit sınır 156 px'tir. HUD pill yüksekliği 138 px sabit, yatay uç sınırları 69 px'tir. İlerleme dolgusu soldan maskelenir. Zincir kablosu ve duvar yatay tile edilir, halka ve diskler ayrı katmandır.

Nunito önerisi: `Nunito[wght].ttf`, normal 400 / kalın 700; SIL Open Font License 1.1. Türkçe Latin Extended kapsamı ve dosya adı [Google Fonts metadata](https://github.com/google/fonts/blob/main/ofl/nunito/METADATA.pb), lisans [OFL.txt](https://github.com/googlefonts/nunito/blob/main/OFL.txt) üzerinden doğrulandı. İstendiği gibi font dosyası indirilmedi.

Üretim: `node docs/art/build-task5.cjs --avatar Assets/Art/UI/Task5/avatar-source.png`. SVG kaynakları aynı klasörlerde; avatar onaylı ikon referansı ile üretilmiştir. Oyna düğmesi, tutorial elleri ve oyun sahnesi Görev 4 kaynaklarından gelir. Skin, onaylı ikon ve menü kaynakları değiştirilmedi.

Doğrulama: `powershell -NoProfile -ExecutionPolicy Bypass -File docs/art/verify-task5.ps1`. 89 PNG açıldı; tüm ölçüler manifest ile eşleşti; şeffaf kenarlar, sıfırdan büyük 9-slice merkezleri ve tile dokularının birebir karşı kenar pikselleri kontrol edildi. [Sonuç](task5-validation.json): hata yok. Toplu görsel kontrol yapıldı. Unity entegrasyonu/oyun içi test bu teslimin dışında; kod değişikliği, push veya deploy yapılmadı.
