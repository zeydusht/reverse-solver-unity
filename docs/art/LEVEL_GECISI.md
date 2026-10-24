# Zaferden koridora dönüş — Zeyd'in yönlendirmesi

24 Ekim 2026 güncellemesi: Zeyd'in akışına göre 2,8 sn'lik tarayıcı animatiği `Assets/Art/AnimationDemo/index.html` içinde hazır. Bu, v04 koridoru üstünde koşu kareleri ve basit kamera tween'idir; gerçek katmanlı geçiş veya oyun entegrasyonu değildir. Önceki karar ve uygulama notları aşağıdadır.

## Üretilen kısa konsept demo

- Oyuncuyu temsil eden Jiggle önde kameraya doğru koşar; Baron küçük ölçekte arkada takip eder.
- Kamera çekiliyormuş hissi için v04 koridor hafif ölçeklenip yana kayar.
- Ayrı Jiggles/Baron koşu pozları sırayla oynatılır. Örnek seviye rozeti tamamlanan 12'den sıradaki 13'e geçer.
- Önizleme: `Assets/Art/AnimationDemo/index.html`; poster: `Assets/Art/AnimationDemo/poster-v01.png`.
- Bu demo boyalı tabloyu kendi katmanı olarak kaydırmaz veya yeni level tablosuyla değiştirmez. Bunun için duvar arka planı, ana/yan çerçeveler, resim içerikleri ve karakterler ayrı sahne katmanları olmalıdır.

1. Bölüm kazanılır; kurtarılan Jiggles oyuncuya/kameraya doğru koşar.
2. Arkalarında çatık kaşlı Baron onları kovalar. Jiggles'ın oyuncuya ulaşması kurtuluş hissini vermeli; kazanılmış bölüm yeniden tehlikeye girmiş gibi görünmemeli.
3. Kamera geri çekilirken tamamlanan bölümün tablosu perspektifte sağa kayar ve sağdaki sonraki duvar bloğunda küçük görünür. Koridorun blok devamlılığı korunur; tablo aniden silinip farklı bir yerde belirmez.
4. Ana duvar bloğunda ortalanmış tek büyük çerçevede sıradaki bölüm görünür; sağdaki komşu blokta önceki tablo küçük kalır. Tamamlanmış tablonun içeriğinin boşalması bu mesajda ayrıca kararlaştırılmadı.
5. Oyuncu hazır olduğunda sıradaki bölümü başlatır. Bu belge otomatik bölüm başlatmayı kararlaştırmaz.

## Statik sanat yönü

Seçilen tasarım dayanağı: Assets/Art/Review/Koridor/2.png. Güncel kullanıcı açıklaması: **tek çerçeve yalnız ana duvar bloğu içindir**. Ana petrol duvarı çevreleyen iki sütun arasında büyük tablo ortalanır; bu bloktaki iki küçük çerçeve olmaz. Sağ arkadaki komşu duvar bloğunun küçük tablosu kalır. Böylece toplam iki çerçeve vardır: bir büyük ana, bir küçük uzak yan. Işık, perspektif, mimari ve bordo halı korunur. Sonraki alternatif: Assets/Art/Review/Koridor/2-ortali-yan-blok-v05.png. Zeyd son olarak v04'ü seçti: Assets/Art/Review/Koridor/2-ortali-yan-blok-v04.png; v04 ilk yerleşim denemesi değil, kullanıcının seçtiği çalışma tabanıdır. Önceki v03 uzak yan tabloyu da kaldırdığı için bu açıklamayı karşılamaz; geçmiş sürüm olarak korunur.

## Mühendislik ve sonraki sanat işi

- Tek katmanlı koridor PNG'si gerçek kamera geri çekilmesini veya tablo içeriğinin bağımsız değişmesini tek başına sağlamaz. İleride çevre / boş çerçeve / tablo içi ve karakter katmanları hazırlanmalı.
- Geri çekilme sırasında ana tabloyu sağdaki küçük tabloya dönüştüren mekânsal devamlılık için kamera yolu ve duvar bloklarının perspektifi birlikte kurulmalı; statik resimdeki iki tabloyu yalnız ölçekleyip üst üste getirmek yeterli kabul edilmez. Yeni ana bölüm tablosunun girişi animatikte doğrulanmalı.
- Mevcut statik revizyonda tablo resmi kalır; şu anda her level için dinamik resim veya numara eklenmez.
- iPhone WebGL hedefi için hafif sprite/tween veya sınırlı parallax yaklaşımı değerlendirilmeli; ağır video kullanılmamalı.
- Animasyonun süreleri, katman sınırları ve sonraki level sunumu uygulama öncesi kısa animatikle somutlaştırılabilir. Bu aşamada süre veya etkileşim ayrıntısı nihai karar değildir.
