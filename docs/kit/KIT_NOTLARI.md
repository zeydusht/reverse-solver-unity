# Kit notları

Her PM işinde öğrenilen, başka oyunda da geçerli ders buraya tek satır olarak eklenir.

- Tahminleri playtest başlamadan dondur; sonra veriyle karşılaştır.
- Test hesaplarını ve kendi cihazlarını veriden en baştan ayır (kohort + hariç tutma tablosu).
- Tutorial bölümleri kaybettirmez; kayıp yalnız yeni öğeden gelir.
- Mac imzası: kullanıcı adında Türkçe karakter (İ) sorun çıkarır.
- Bomba payını tek çözümden değil, gerçek en kısa yoldan ölç; zorluğu "pay" değil, doğal oyuncunun bombayı zamanında alıp almadığı belirler (PM_69).
- Zorluğu iki eksende ölç: kaçırma (bomba + çok boş hamle) ve bulma (kilit/zincir/duvar seçenekleri 1–2'ye indirir). İkincisi kaybettirmez ama düşündürür (PM_69).

## Bölüm tasarım kuralları (PM_70, model v2)
Kod: `DifficultyModelV2.Rules` (üretici ve p22 testi bunları uygular). Ölçüler `DifficultyV2` (`docs/analiz/v2/PM69_RAPOR.md`).
- **Boğaz:** normal bölümde pay_true ≤ 1 olan bomba en çok 1, zor bölümde en çok 2. 3+ boğaz ("ezber merdiveni") yok.
- **Açık tahtada bomba:** bir bombanın fitil penceresinde adım başına çeldirici ≥ 5 ise ya rastgele oyuncu ≥ %25 hayatta kalsın ya da o bombanın payı ≥ 8 olsun. Sıkı bomba (pay ≤ 2) daraltılmış tahtaya (dar an çok, çeldirici az).
- **Süre:** sn/parça 1,7–2,3. Zor bölümü süreyi kısarak değil, engel (dar an) ekleyerek zorlaştır. Bu aralıkta bombasız bölüm modelde ≥ %85: bombasızlar nefes bölümüdür.
- **Düşündürme:** 21–40'ta her bölümde takılma/çıkış tahmini ≥ 0,5 (kilit, zincir veya duvar dar anı). Kilit en çok düşündüren engel (katsayı zincir ve duvarın ~4 katı).
- **Ayar sırası:** fitil → süre → bombalı parçayı taşı → engel ekle/çıkar → havuzdan değiştir. Tahta düzenine gerekmedikçe dokunma; her değişiklikte yeni sürüm.
