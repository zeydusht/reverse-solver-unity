# Task35 · Booster boyutu ve başlık ölçümü

[Önizleme paftası](onizleme.png) · [Royal Match alt şerit kırpması](../task31/ref_alt.png) · [HUD35 kaynakları](../../../Assets/Art/_yeni/HUD35/)

## Royal Match ölçümü

1170 × 302 px kırpmayı Pillow ile ölçtüm. Kırpma ana ekranın y=2230 satırından başlıyor. Koyu renk dış oluk, adet rozeti ve tuşun gölgesi dış çap hesabına girmedi. Turuncu kenar yatay ve dikey merkez kesitinden okundu. İkon silüeti, yeşil veya mavi tuş zemininden renk maskesiyle ayrıldı. Ana ikonun bağlı piksel bölgesi ölçüldü. Sayılar yaklaşık birer piksel tolerans taşır. Koordinatlar yarı açık, başlangıç dahil ve bitiş hariç.

| Royal Match ikonu | Merkez (x, y) | Dış çap en × boy | Sınır kutusu (x₀, y₀, x₁, y₁) | İkon en × boy | Sol | Üst | Sağ | Alt |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| Çekiç | 126, 128 | 194 × 192 px | 72, 63, 184, 197 | 112 × 134 px | 42 px | 31 px | 40 px | 27 px |
| Arbalet | 355, 128 | 194 × 192 px | 293, 58, 422, 196 | 129 × 138 px | 34 px | 26 px | 31 px | 28 px |
| Top | 585, 128 | 194 × 192 px | 530, 66, 643, 176 | 113 × 110 px | 42 px | 34 px | 39 px | 48 px |
| Palyaço şapkası | 814, 128 | 194 × 192 px | 743, 76, 886, 172 | 143 × 96 px | 26 px | 44 px | 25 px | 52 px |
| Ayarlar dişlisi | 1043, 128 | 194 × 192 px | 989, 71, 1097, 184 | 108 × 113 px | 43 px | 39 px | 43 px | 40 px |

Dört boosterda ölçtüğüm en aralığı **112–143 px**, boy aralığı **96–138 px**. PM tablosu 124–144 ve 111–137 px veriyordu. Özellikle topun eni ile şapkanın boyu, rozeti ve gölgeyi ayırınca daha küçük çıktı. En uzun kenarın dış çapa oranı dört ikon için yaklaşık %58–74. Üretimde brifin ortak **%72** hedefini uyguladım. Ayarlar dişlisi ek referans olarak ölçüldü.

Paftada iki taraf aynı 205 px kırpma ve aynı büyütme oranıyla gösterildi. Camgöbeği çizgi sınır kutusu, krem artı tuş merkezi, pembe nokta silüet ağırlık merkezi. Royal Match noktasında düzleştirilmiş görselden çıkarılan ikili maske kullanıldı.

## Task35 yerleşimi

Task34 boyası kullanılarak ikonlar tek oranla ölçeklendi. Saatte yalnızca önceki yeşil alanın karşılığı olan dış kasa siyaha geçti. Boyut ve merkez için alfa kanalı ağırlıklı ortalama alındı. Aşağıdaki koordinatlar 195 × 195 px saydam tuvalin sol üstüne göre. Her sprite tuşun merkezine (97, 97) px pivotla yerleştirilir. Rozet üst katmanda.

| İkon | Sınır kutusu | En × boy | Uzun kenar / dış çap | Sol / üst / sağ / alt boşluk | Ağırlık merkezi |
|---|---:|---:|---:|---:|---:|
| Makas | 33, 32, 174, 161 | 141 × 129 px | %72,3 | 33 / 32 / 21 / 34 px | 97,44 / 97,18 px |
| Değnek | 36, 31, 161, 172 | 125 × 141 px | %72,3 | 36 / 31 / 34 / 23 px | 96,61 / 96,62 px |
| Mavi çekiç | 35, 33, 169, 174 | 134 × 141 px | %72,3 | 35 / 33 / 26 / 21 px | 96,67 / 97,40 px |
| Saat | 39, 25, 159, 166 | 120 × 141 px | %72,3 | 39 / 25 / 36 / 29 px | 96,60 / 97,28 px |

Merkez sapması her eksende 0,5 px altında. Üç ikonun boyası Task34 PNG’lerinden doğrudan örneklendi. Saatin siyah kasasında #3A3A48, #1C1C26 ve #0C0C12 geçişleri ve mevcut üst sol parlamanın konumu kullanıldı. Kadran, kollar, ziller ve ayaklar eski boyayı taşıyor. [Task34 kaynak ikonları](../../../Assets/Art/_yeni/HUD34/Boosters/)

## Başlık kutuları

Kutular krem dolgu, mavi çerçeve ve iç bevel taşır. Baloo 2 ExtraBold başlıklar canlı metindir. Alt panel, sayaç ve diğer HUD parçaları mevcut kaynaklardan bileştirildi.

| Dosya | Tuval | Üst ekran konumu |
|---|---:|---|
| `Headers/a.png` · Bitişik | 286 × 78 px | x merkezi 201 / 972, tuval üstü y=121 px. Boyalı yüz gövdeye 6 px biner. |
| `Headers/b.png` · Aralıklı | 260 × 76 px | Aynı x merkezleri, y=107 px. Boyalı yüzler arasında 10 px. |
| `Headers/c.png` · Yarı gömülü | 274 × 78 px | Aynı x merkezleri, y=150 px. Boyalı yüzün 35 px bölümü gövdeye biner. |
| `Headers/continue.png` · Süre doldu | 614 × 102 px | x merkezi 585, y=464 px. Bitişik düzenin geniş başlığı. |

Süre doldu provasında Bitişik başlık ve yalnız kırmızı X gösterildi. Bu teslim görsel prova ve sekiz saydam PNG’den oluşur. Oyun koduna bağlanmadı.
