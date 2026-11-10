# Task36 · Yeşil disk boşluğu

[Önizleme paftası](onizleme.png) · [HUD36 dosyaları](../../../Assets/Art/_yeni/HUD36/) · [Task35 yerleşimi](../task35/olcum.md)

195 × 195 px ikon katmanının merkezi (97, 97) px. Tuş dış çapı 195 px. Yeşil alanın koyu kenarıyla birlikte altın halkaya ulaştığı sınır, HUD31 tuşunda r=83,30 px. Mavi ölçüm çemberi bu sınırı gösterir. Yarı saydam kenar dahil alfa değeri sıfırdan büyük her pikselin en uzak köşesi, tuş merkezine göre ölçüldü. En küçük boşluk, yeşil sınır yarıçapından bu uzaklığın çıkarılmasıyla bulundu.

Kontrolde %4 alt sınırı olarak **7,80 px** kullanıldı. Bu, 195 px dış çapın %4’üdür. Yeşil alanın kendi çapına göre %4, 6,66 px olur. Negatif değer taşmayı gösterir.

| İkon | Önceki en dar boşluk | Yeni uzun kenar | Yeni merkez x / y | Yeni en büyük yarıçap | Yeni en dar boşluk | Dış çapa oran | Sonuç |
|---|---:|---:|---:|---:|---:|---:|---|
| Makas | -7,60 px | 124 px | 91,58 / 96,54 px | 75,33 px | 7,97 px | %4,09 | Geçti |
| Değnek | -8,20 px | 126 px | 102,25 / 91,58 px | 74,92 px | 8,38 px | %4,30 | Geçti |
| Mavi çekiç | -6,45 px | 127 px | 102,14 / 91,89 px | 75,08 px | 8,22 px | %4,22 | Geçti |
| Saat | 0,46 px | 141 px | 96,60 / 97,28 px | 82,84 px | 0,46 px | %0,24 | Saat ölçüsü korundu |

Makas sola 5 px, değnek ve çekiç sağa 5 px ve yukarı 5 px kaydırıldı. Üçü de Task34 kaynağından tek geçişte ölçeklendi. Alfa ağırlık merkezlerinin en büyük eksen sapması 5,5 px içinde.

Saatin iki kahverengi sürümü Task35’in alfa maskesi, boyutu ve konumuyla eşleşiyor. Her piksel yeşil sınır içinde. En dar boşluk 0,46 px, dolayısıyla saat %4 hedefine ulaşmıyor. Saatin yerini ve ölçüsünü koruma talimatı bu örnekte önceliklendirildi.

## Yerleşim ve kaynaklar

- Beş booster PNG’si 195 × 195 px. Sıcak ahşap kasa (clock-a) ana ekranda örnek olarak kullanıldı. Kırmızımsı kahve (clock-b) aynı boyutta.
- Başlığın krem kutusu Task35 C kaynağıyla aynı. 336 × 107 px oyun başlığı tuval üstü y=150 px, x merkezleri 201 ve 972 px. Süsler kutunun arkasında. [Task35 kutuları](../../../Assets/Art/_yeni/HUD35/Headers/)
- Süre doldu başlıkları 698 × 137 px. x merkezi 585, tuval üstü y=505 px. Kutunun alt yarısı panelin üstüne oturur. Yazı canlı metindir ve başlık PNG’sinden ayrıdır.
- Oyun ekranında turuncu dış halka, altın halka, yeşil gövde ve rozet mevcut HUD kaynaklarıyla bileştirildi. [Tuş kaynakları](../../../Assets/Art/_yeni/HUD31/shared/) · [Booster boya kaynakları](../../../Assets/Art/_yeni/HUD34/Boosters/)
