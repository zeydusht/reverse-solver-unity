# PM77 diorama teslimi

Üç katman, yerleşim verileri ve önizlemeler `CODEX_PM77.md` ölçülerine göre hazırlandı. Görseller yerleşik `image_gen.imagegen` aracıyla, yerel oda referansları görüntü girdisi olarak verilerek üretildi. Ana barok çerçevenin kimliğini korumak için `PM76/aci/a3_bg.png` içindeki çerçeve alfa ayrımı ve ölçekleme ile yeni duvara yerleştirildi; dört küçük manzara yeni görsel üretiminden alındı.

Üretim kaynakları `kaynaklar/` içinde, ilk üç istem `uretim_promptlari.json` içinde, çerçevelerin yerleştirilmesinden önce boş duvarı hazırlayan son istem `duvar_temizleme_promptu.txt` içinde saklandı. Üretilen duvar/zemin kaynakları 2172×724, sütun kaynağı 819×1920 pikseldir; teslim dokuları PIL ile istenen boyutlara ölçeklendi. PIL ayrıca çerçeve hizalaması, alfa ayrımı, halı sınırlarının yerleşimi ve düzlemlerin perspektif önizlemelerinde kullanıldı.

Teslim dosyaları: `Assets/Art/_yeni/PM77/diorama/duvar.png` (4800×1600), `zemin.png` (4800×1200), `on_sutun.png` (480×2400), `diorama.json`. PNG dosyaları RGBA'dır. Duvarda yalnız `[2244,974,2556,1374)` dikdörtgeni tamamen şeffaftır; kalan duvar ve zemin tamamen opaktır. Piksel koordinatlarının başlangıcı sol üsttür, dikdörtgenlerin sağ/alt sınırları hariçtir. Ölçek 400 piksel/birimdir. Sütunun taban orta noktası `(240,2400)` olarak tanımlıdır; son görüntü satırında taban pikselleri vardır.

Zemin halısı `[0,140,4800,580)` aralığındadır: duvardan 0,35 birim sonra başlar, 1,1 birim genişliğindedir. Duvarın zemin çizgisi y=1600; lambri üst sınırı y=1380'dir. Dünya eksenleri: X duvar boyunca sağa, Y yukarı, Z duvardan izleyiciye doğrudur. Sütunun iki kontroldeki değişmeyen taban konumu `(-1.8,0,1.2)`; düzlemi duvara paraleldir.

`diorama_katmanlar.png` üç katmanı dama tahtası üzerinde ayrı gösterir. `diorama_kontrol.png`, 38° ve tam karşıdan görünümleri oda referansları ve alfa yakın planıyla birlikte gösterir. İki görünüm ayrıca `diorama_38derece.png` ve `diorama_karsidan.png` dosyalarında bulunur. Önizlemelerde aynı dokular gerçek dünya düzlemlerinden perspektif homografileriyle örneklenir; tuvale `painting-01.jpg` merkezden kırpılarak yerleştirilir. Kamera yarıçapı, konumları ve görüş açısı JSON'dadır. Bunlar çevrimdışı kontrollerdir; oyun kodu/sahnesi değiştirilmedi, Unity içinde çalışma testi yapılmadı.

`dogrulama.json` boyut, RGBA, tam alfa deliği, halı sınırları, sütun tabanı, kamera açıları ve 10 MB dosya sınırı kontrollerinin sonucunu ve teslim PNG'lerinin SHA-256 özetlerini içerir. Yalnız izin verilen iki klasöre yazıldı; commit yapılmadı.
