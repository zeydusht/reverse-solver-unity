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

## Soft launch dönemi dersleri (17–20 Kasım, PM_81–96)
- Canlı oyunda zorluk ritmi: 4 ile biten bölüm zor, 9 ile biten çok zor, x0 nefes; finale zor yığma, zor bölümler aralıklı gelir (PM_82).
- Engel tanıtımları ve booster açılışları ilk zor bölümden hemen önce; tanıtım asla zor slota denk gelmez (PM_82).
- Rehber ipucu (parlama + ok) yalnız ilk 3 bölümde; sonrasında kopya yok, hangi parçanın boşta olduğunu bulmak oyunun kendisi (PM_84).
- Reklam: ödüllü (süre +30 sn, 2× altın, booster bitince +1, menü hediyesi) + 12. bölümden sonra 3 bölümde bir geçiş; banner yok; sıklık uzaktan ayarda (PM_81/82).
- Sandık yalnız açılmış booster verir; ekranın her yerine dokunmak açar; ödül kendiliğinden sayaca uçar, ayrı Topla düğmesi yok (PM_83/86).
- Geçişlerde linear tween yok: kalkış yumuşak, sonda fren yok; hız grafiğini ekranda ölçerek doğrula (tepe/çukur/plato aranır) (PM_86/91/92).
- Kamera dalışında resim çerçeveye katı bağlı; kırpım/oran farkı yalnız çerçeve görünmezken, eşit ölçekle ve tek yönde ayarlanır (PM_87/90).
- Çizilen çizgiler ayrı katman değil, gerçek nesnenin kendi kenarı; devirde piksel farkı ölçülür (PM_84/86).
- Sekme/menü: sayfalar önceden kurulu, basış → hareket ≤16 ms, animasyon kesilebilir; referans oyun kaydından süre ölçülür (PM_87/88).
- Takılma hissi: lastik squash & stretch (çarpmada büyük basılma, geri sekme + küçük aşma), itilince anında tam hız (PM_93/94).
- Ses: sentetik değil kayıtlı ve ticari lisanslı sesler; tekrarlanan seste rastgele perde; lobi ve bölüm müziği ayrı; kazanma animasyonu fanfarın vuruş haritasına oturur (PM_95/96).
- Seçimi kullanıcıya dinleme/önizleme sayfası ya da video ile yaptır; görsel düzeltmede kapsamı dar tut, istenmeyeni değiştirme (PM_90/91).
