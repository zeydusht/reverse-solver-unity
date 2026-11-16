PM76 görsel teslimi

Çıktılar: `Assets/Art/_yeni/PM76/aci/`, `aci_b/`, `koridor/`.
İki takımda da a0 doğal 1024×1536 ölçüsündedir; ayrıca açıkça adlandırılmış
1180×1770 kopyaları bulunur. a1–a3 katmanları 1180×1770 RGBA PNG'dir.
Koridor katmanları 1644×1688 RGBA PNG'dir. Her takımın `aci.json` dosyası
tuval köşelerini, gerçek dış çerçeve kutularını ve a0 için merkezlenmiş
%15 büyütmenin matrisini içerir. Köşe sırası: sol üst, sağ üst, sağ alt, sol alt.

Odalar ve kapalı alanların arka planları, yerleşik imagegen aracında gerçek
referans PNG'ler verilerek üretildi. Bütün fotoğraf döndürülmedi. Özgün altın
çerçevenin süsleri, katmanı ayrılarak yeni oda açılarındaki duvar düzlemine
perspektif olarak hizalandı. Ön planın ışık ve kenar matları Pillow/NumPy ile
ayrıldı. a0 ve koridorun birleşimleri, a0'ın boşaltılan tuvali dışında,
kaynaklarla piksel piksel aynıdır; sayısal sonuçlar `validation.json` içindedir.
Üretim istemleri ve elenen ilk a1 denemesi bilgisi `prompts.json` içindedir.

Önizlemeler: `acilar.png`, `acilar_b.png`, `katmanlar.png`.
İki açı önizlemesinde de aynı `painting-01.jpg`, oranı korunup ortadan
kırpılarak perspektif tuvale yerleştirildi. Önizleme resmi teslim edilen
arka planlara gömülmez; tuval delikleri gerçekten saydamdır.

Tercih: A / `aci`. Ön sütunun kademeli daralması ve karşıdan görülen son
odanın geometrisi kamera yayı hissini daha açık verir. B / `aci_b` aynı
çerçeveyi korur; sağdaki koridoru son açıda daha fazla gösteren alternatiftir.

`build_pm76.py`, saklanan üretim kaynakları ve `angle_config.json` ile
katmanları, JSON dosyalarını ve önizlemeleri yeniden oluşturur. Yalnızca
PM76 çıktı klasörlerine yazar. Oyun kodu, sahneler ve kaynak görseller bu
işlem tarafından değiştirilmedi; commit yapılmadı.
