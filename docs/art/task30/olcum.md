# Task30 · Referans HUD ölçümü

[Ölçülen iPhone ekranı](../Reference/rm_oyun_ekrani.png) · [PM ölçümü](../HUD_ANALIZ_RM.md)

Görseli Pillow ile RGB olarak okudum. Kaynak 1170 × 2532 px. Aşağıdaki (x, y) koordinatları 390 × 844 pt ölçeğinde. Renk örnekleri kaynakta her noktanın karşılığı olan 3 × 3 piksel bloğunun RGB medyanı. Sınırları yatay ve dikey kesitlerle, aynı satırdaki dış arka plan pikseline göre ve kenar gölgelerini ayırarak ölçtüm.

| Öğe | Bağımsız ölçüm | PM karşılaştırması ve çizim kararı |
|---|---|---|
| Üst dolgu | y=0–158, toplam 159 pt (%18,84). x=2–4 hattında y=0 '#124382', y=78 '#155093', y=158 '#1A5EA5'. | Geometri aynı. Üst renk PM'den bir ton koyu. Çizimde A/B/C turkuaz gradyanları. |
| Alt bordür | y=159–163, toplam 5 pt. x=2'de sırasıyla '#FEBB00', '#FFB300', '#FFC93D', '#FF7101', '#E2640A'. | Kalınlık aynı. Örneklenen sarı tepe y=161. Çizimde brifin 1 pt ışık + 2 pt altın + 2 pt turuncu sırası. |
| Üst şerit gölgesi | x=2'de y=164 '#2D5160', y=165 '#376375', y=166 '#5191AA', y=167 '#5BA1BC', y=168 '#5BA2BD'. | Gölge ilk 3–4 pt boyunca azalıyor. Çizimde y=164–168, başta %50 alfa ve yumuşak kuyruk. |
| Sol ve sağ panel | Gövdenin düz kesitinde y=100'de sol x=11–122, sağ x=268–379. Kurdelenin kanatları x=8–124 ve x=265–381. Alt gövde/gölge y=154–155. | Tasarım alanı PM'deki 113 × 118 pt, x=9 / 268, y=37. Kurdelenin genişliği gövdeden birkaç pt fazla, alt uçları y≈72. |
| Panel katmanları | y=100 sol kesitinde açık mavi x=11 '#006EC0', lacivert x=12–13 '#003D87', bej x=14–17 '#A4997F' çevresi, krem x=18–24 '#F9F0CC' → '#F4E9BB'. Turuncu ve altın özellikle kurdele x=9–14, y=49'da okunuyor. Kremde y=70 '#E6DAB7', y=136 '#FEF4BE'. | Düz yan kenarda sıcak bantlar kurdeleden daha az belirgin, bej geçiş medyanda 3–4 pt. Brifin sekiz katman sırası ve 1–2 pt bej bevel'i kullanılıyor. |
| Portre | x=131–258 yaklaşık 127 pt. Kemerin dış tepesi y=36–37, üst eğri omuzları y≈51, düz alt kenar y=155. y=100'de sol profilde x=131–132 açık mavi, 133–134 lacivert, 135–136 turuncu, 137 ışık, 138–139 altın, 140 iç zemin. | PM tablosunda y=51 başlangıç verilmiş. Bu nokta kemer omzunda. Çizimde görselin dış tepesi y=37 ve alt hizası y=155, omuz y≈51. İç dolgu gece moru → camgöbeği. |
| Alt şerit | y=771–843, toplam 73 pt (%8,65). x=2'de y=771 '#003984', y=772 '#004284', y=773 '#0073C4', y=774 '#0054AB', y=843 '#0062AD'. | Yükseklik aynı. İki kenar rengi arasındaki geçiş bir ek satıra yayılıyor. Çizimde 1 + 1 pt kenar ve turkuaz gradyan. |
| Tuşlar ve yuvalar | Merkezler x≈42 / 118 / 195 / 272 / 348. Merkez düşey kesitlerinde dış turuncu y=754–755, altın y=756–757, y=814–817 karşı kenar. Dış halka yaklaşık 64 pt, yuva biraz daha geniş. | PM yaklaşık 62 pt. Çizimde nominal çap 62 pt, merkez y=786 ve üst y=755. 16 pt, yani çapın %26'sı dışarıda. Çukur ve gölge çapın dışında. |
| Tahta | Referans dış tahta x≈3–388, y≈230–699. | Bizim kare tahta PM §3'e göre 374 × 374 pt, x=8, y=268. 587 pt kullanılabilir dikey alanın ortasına yakın. |

Açık referans ve yeni ekran paftada birebir aynı 390 × 844 boyutta gösterilecek. Kılavuzlar 37/155 pt panel hizasını, 159/164 pt şerit sınırını, 755/771/786 pt alt hizalarını ve yatay portre/tuş merkezlerini kullanacak.
