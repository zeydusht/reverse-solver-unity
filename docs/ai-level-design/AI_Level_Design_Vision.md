# REVERSE SOLVER — AI LEVEL DESIGN SYSTEM

_Vizyon, Mevcut Durum ve Matematiksel Geliştirme Planı_

> Zeyd, 20 Kasım 2026. Uzun vadeli vizyon belgesi; içerik korunarak kaydedildi.


## 1. ANA VİZYONUMUZ

Amacımız yalnızca otomatik level üreten bir bot geliştirmek değil.

Hedefimiz, profesyonel bir AI Level Designer + AI Product Specialist gibi çalışan, kendi ürettiği levelleri oynayan, değerlendiren, eleştiren ve matematiksel yöntemlerle sürekli iyileştiren bir yapay zekâ sistemi oluşturmak.

Sistem şu sorulara cevap verebilmeli:

* Bir level neden kolay veya zor?
* Oyuncular hangi hamlelerde zorlanıyor?
* Bir level’ın kaç farklı çözüm yolu var?
* Hangi engel kombinasyonları zorluk yaratıyor?
* Oyuncunun son hamlede kazanma olasılığı nedir?
* Bir level neden sıkıcı veya eğlenceli?
* Hedeflenen zorluğa ulaşmak için neyi değiştirmeliyiz?
* Oyuncuların oyundan aldığı tatmini nasıl artırabiliriz?
* Ardışık levellerde zorluk ve çeşitlilik nasıl dengelenmeli?

Uzun vadede sistem, yalnızca Reverse Solver veya Royal Match için değil, farklı puzzle oyunlarına uyarlanabilecek modüler bir mimariye sahip olmalı.

## 2. MEVCUT DURUMUMUZ

Reverse Solver isimli çalışan bir puzzle oyunumuz bulunuyor.

Claude yardımıyla ilk level generator sistemimizi geliştirdik.

İlk aşamada yaklaşık 40 level üretildi. Bunlar kolay, orta ve zor gibi kategorilere ayrıldı.

Ardından bu levelleri bizzat oynayarak değerlendirdim.

Verdiğim geri bildirimlerden bazıları:

* Çok sıkıcı.
* Çok kolay.
* Çok kısa sürdü.
* Hiç düşündürmedi.
* Son hamlede çözebildim.
* Çözümü bulunca çok tatmin oldum.
* Zorluk etiketi yanlış.
* Bu level gerçekten çok iyi.
* Bu level’ın çözümünde yeterli strateji gerekmiyor.

Claude bu geri bildirimlere göre yeni leveller oluşturdu. Bu süreç birkaç kez tekrarlandı.

Yani başlangıçta Human Evaluator olarak ben çalıştım.

Daha sonra kendi oynama davranışımı taklit eden bir bot geliştirilmesini istedim.

Yaklaşık 40 level’ı yeniden oynadım ve şu bilgiler kaydedildi:

* Her hamlem.
* Hamle zamanları.
* İlk hangi hamleleri tercih ettiğim.
* Önce hangi engellere odaklandığım.
* Hangi çözüm stratejilerini izlediğim.

Bu bilgiler kullanılarak oyuncu davranışımı taklit etmeyi amaçlayan bir bot geliştirildi.

Ancak botun gerçekten eğitilmiş bir machine learning modeli mi yoksa kurallarla çalışan bir algoritma mı olduğu kod üzerinden doğrulanmalı.

Şu anda Generator ve Evaluator sistemlerinin çalışan bir ilk versiyonu var.

Evaluator, Monte Carlo simülasyonları yapabiliyor ve level’ların başarı oranı, zorluk ve benzeri metriklerini raporlayabiliyor.

Oyun ayrıca yaklaşık 20–30 gerçek oyuncu tarafından test edildi.

Elimizde:

* Level bazında first-try win verileri.
* Başarısız deneme sayıları.
* Oyuncuların hangi level’larda kaldığı.
* Level tamamlama verileri.

bulunuyor.

Ancak diğer oyuncuların hamle bazlı davranış kayıtları henüz yok.

## 3. ASIL HEDEF: %80’DEN %100’E YAKLAŞMAK

Mevcut sistem ilk versiyonuna göre önemli ölçüde gelişti.

Öznel değerlendirmemle başlangıçtaki yaklaşık %0 seviyesinden %80 seviyesine geldiğimizi düşünüyorum.

Bu yüzdeler ölçülmüş bilimsel performans değerleri değil, sistemin gelişimini anlatan bir benzetme.

Şimdi amacımız kalan kalite farkını azaltmak.

Bunu yalnızca daha iyi promptlar yazarak değil, AI’ın matematiksel temellerini kullanarak yapmak istiyoruz.

Özellikle:

* Linear Algebra
* Matrices and Vectors
* Probability and Statistics
* Machine Learning
* Neural Networks
* Gradient Descent
* Backpropagation
* Optimization
* Monte Carlo Simulation
* Reward Modeling
* Reinforcement Learning
* Overfitting Prevention
* Regularization
* Dropout

gibi kavramları gerçekten ihtiyaç duyulan yerlerde kullanmak istiyoruz.

Amaç AI terimlerini projeye süs olarak eklemek değil, bu tekniklerle ölçülebilir iyileşmeler sağlamak.

## 4. HEDEF SİSTEM MİMARİSİ

Sistem beş ana parçadan oluşacak.

### A. LEVEL GENERATOR

Belirlenen hedeflere göre level üretecek.

Örneğin:

“Bana zor ama çözülebilir, son hamlede kazanma ihtimali yüksek bir level oluştur.”

Generator engelleri, hamle limitini, başlangıç durumunu ve diğer mekanikleri belirleyecek.

### B. PLAYER BEHAVIOR MODEL

İnsan oyuncuların davranışlarını taklit edecek.

Başlangıçta mevcut oyuncu botumuz kullanılacak.

İleride farklı oyuncu profilleri geliştirilecek:

* Beginner Player
* Average Player
* Expert Player

Böylece level’ın yalnızca genel zorluğunu değil, farklı oyuncular için ne kadar zor olduğunu değerlendirebileceğiz.

### C. MONTE CARLO SIMULATOR

Oluşturulan level’ı binlerce kez oynayacak.

Ölçülecek metrikler:

* Win rate
* First-try win rate
* Average moves to win
* Failure rate
* Remaining moves
* Solution path diversity
* Near-miss frequency
* Last-move win frequency
* Dead-end frequency

Monte Carlo sonuçlarının doğruluğu, kullanılan oyuncu modelinin gerçek insan davranışını ne kadar iyi temsil ettiğine bağlı olacak.

### D. AI LEVEL EVALUATOR

Simülasyon sonuçlarını analiz edecek.

Örneğin:

“Bu level zor olarak tasarlanmış ancak simülasyonlarda başarı oranı %85. Hedef %35. Hamle sayısı fazla ve engeller yeterince kısıtlayıcı değil.”

Sadece puan vermeyecek, nedenlerini de açıklayacak.

### E. OPTIMIZER

Evaluator’ın geri bildirimini alacak ve level’ı iyileştirecek.

Örneğin:

* Hamle limitini azalt.
* Engel sayısını artır.
* Engellerin yerini değiştir.
* Çözüm yollarını azalt.
* Daha fazla stratejik karar gerektiren bir yapı oluştur.

Sonra level yeniden simüle edilecek.

Bu döngü hedeflenen kaliteye ulaşana kadar devam edecek.

## 5. MATEMATİKSEL OPTİMİZASYON

Bu projenin en önemli hedeflerinden biri, level tasarımını matematiksel bir optimizasyon problemine dönüştürmek.

Örneğin:

Bir level’da 30 hamle var.

Monte Carlo simülasyonlarında başarı oranı %85.

Hedefimiz %35.

Optimizer şu soruyu soracak:

“Başarı oranını hedefe yaklaştırmak için level’ın hangi parametrelerini ne kadar değiştirmeliyim?”

Burada türev ve gradyan mantığından yararlanmak istiyoruz.

Ancak level parametrelerinin çoğu kesikli değişkenlerdir.

Örneğin engel sayısı 3 veya 4 olabilir, 3,5 olamaz.

Bu nedenle klasik türev her zaman doğrudan uygulanamaz.

Değerlendirilmesi gereken yöntemler:

* Finite Difference
* Local Search
* Evolutionary Algorithms
* Bayesian Optimization
* Gradient-Based Optimization
* Surrogate Models

Özellikle türevlenebilir bir kalite tahmin modeli geliştirilebilirse gradient descent ve backpropagation kullanılabilir.

Amaç, parametre değişikliklerinin kalite üzerindeki etkisini sistematik olarak öğrenmek.

## 6. LOSS FUNCTION VE REWARD FUNCTION

Evaluator’ın yalnızca sezgisel değerlendirmeler yapmasını istemiyoruz.

Mümkün olduğunca matematiksel bir kalite fonksiyonu oluşturmalıyız.

Örnek:

Loss = w1 × Difficulty Error + w2 × Repetition Penalty + w3 × Frustration Penalty + w4 × Invalid Level Penalty

Buradaki w değerleri farklı hedeflerin önemini belirleyen ağırlıklardır.

Bu fonksiyon başlangıç taslağıdır.

Eğlenceyi kusursuz ölçen hazır bir matematiksel formül değildir.

Bu yüzden gerçek oyuncu geri bildirimleriyle doğrulanmalı ve geliştirilmelidir.

Amaç yalnızca zor level üretmek değil, hedeflenen oyuncu deneyimini üretmek olmalı.

## 7. HUMAN FEEDBACK VE REWARD MODELING

Geçmişte leveller için verdiğim değerlendirmeler çok önemli.

Bu geri bildirimleri düzenli bir veri setine dönüştürmek istiyoruz.

Örneğin:

Level A: Çok sıkıcı.

Level B: Çok kolay.

Level C: Çok iyi, son hamlede çözüldü.

Level D: Zor ama eğlenceli değil.

Level E: Çok düşündürdü ve tatmin ediciydi.

Bu verilerle bir Reward Model eğitme ihtimalini değerlendirmeliyiz.

Model yalnızca başarı oranına bakmak yerine insanın kalite değerlendirmesini tahmin etmeyi öğrenebilir.

Bunun için supervised learning, preference learning ve reward modeling yaklaşımları araştırılmalı.

Veri miktarı başlangıçta küçük olduğu için basit modellerle başlamak ve daha fazla insan geri bildirimi toplamak gerekebilir.

## 8. OVERFITTING ÖNLENMELİ

Bu projede overfitting önemli bir risk.

Örneğin sistem yalnızca benim 40 level’lık oynama davranışıma göre optimize edilirse benim için güzel görünen ama diğer oyuncular için kötü olan leveller üretebilir.

Benzer şekilde Evaluator, Generator’ın tekrar tekrar ürettiği belirli level yapılarına aşırı uyum sağlayabilir.

Bu nedenle:

* Training, validation ve test verileri ayrılmalı.
* Daha önce görülmemiş level’larda performans ölçülmeli.
* Farklı oyuncu profilleri kullanılmalı.
* Yeni oyuncularla testler yapılmalı.
* Aynı level ailesinin çok benzer varyasyonları farklı veri kümelerine dağıtılarak yanıltıcı sonuçlar oluşturulmamalı.
* Simülasyon sonuçları gerçek oyuncu verileriyle karşılaştırılmalı.

Ayrıca Reward Hacking riski incelenmeli.

Sistem kalite puanını yükseltmeyi öğrenip gerçekten eğlenceli olmayan leveller üretmemeli.

## 9. DROPOUT VE REGULARIZATION

Overfitting’i azaltmak için uygun machine learning modellerinde regularization yöntemlerini değerlendirmek istiyoruz.

Bunlar:

* Dropout
* L2 Regularization
* Weight Decay
* Early Stopping
* Data Augmentation
* Cross Validation

Dropout özellikle neural network eğitimi sırasında bazı nöronların çıktılarını geçici olarak devre dışı bırakarak modelin belirli bağlantılara aşırı bağımlı olmasını azaltmaya yardımcı olabilir.

Ancak dropout, her tür Generator veya Evaluator için uygun değildir.

Kurallı algoritmalarda rastgele nöron kapatmak gibi bir işlem bulunmayabilir.

Bu nedenle dropout yalnızca uygun bir neural network mimarisi kullanıldığında ve deneysel olarak faydalı olduğu görüldüğünde uygulanmalıdır.

Amacımız genelleme yeteneği güçlü modeller geliştirmek.

## 10. GERÇEK OYUNCU VERİLERİYLE MODEL CALIBRATION

Şu anda yaklaşık 20–30 oyuncunun level bazında başarı verileri mevcut.

Bu veriler, simülasyon sonuçlarının doğruluğunu kontrol etmek için kullanılabilir.

Örneğin:

Simülatör first-try win oranını %75 tahmin etti.

Gerçek oyuncularda oran %40 çıktı.

Bu durumda Player Behavior Model veya tahmin sistemimiz yeterince iyi kalibre edilmemiş olabilir.

Model Calibration uygulayarak simülasyon tahminlerini gerçek oyuncu sonuçlarıyla daha uyumlu hâle getirmek istiyoruz.

Ancak mevcut veri miktarı küçük.

Bu yüzden istatistiksel belirsizlikler, örneklem büyüklüğü ve overfitting riski dikkate alınmalı.

Monte Carlo simülasyonu gerçek insan verisinin yerine geçmez.

## 11. LEVEL DIFFICULTY VE FUN FACTOR

Difficulty ve Fun aynı şey değildir.

Bir level zor ama sıkıcı olabilir.

Kolay ama eğlenceli olabilir.

Özellikle sevdiğimiz deneyimler:

* Near-miss
* Last-move win
* Stratejik düşünme
* Çözümü keşfetme
* Rahatlama ve başarı hissi
* Beklenmedik ama adil çözümler

Bunları ayrı metrikler olarak değerlendirmeliyiz.

Ancak sürekli son hamlede kazanılan leveller üretmek de tekrarlayıcı olabilir.

Bu nedenle level çeşitliliği ve progression önemli.

Sistem gerektiğinde beş veya altı level’da bir zorluk artışı gibi farklı progression stratejilerini deneyebilmeli.

## 12. SCALABILITY VE PORTABILITY

Sistem yalnızca Reverse Solver’a bağımlı kalmamalı.

Uzun vadede farklı puzzle oyunlarına uyarlanabilir olmalı.

Bu nedenle:

* Game Rules
* Level Representation
* Player Model
* Simulator
* Evaluator
* Optimizer

mümkün olduğunca birbirinden ayrılmalı.

Yeni bir oyuna geçildiğinde oyun kuralları ve simülasyon bileşenleri değişebilir.

Ancak değerlendirme ve optimizasyon altyapısının mümkün olduğunca yeniden kullanılabilmesini istiyoruz.

## 13. GELİŞTİRME YOL HARİTASI

### PHASE 1 — AUDIT

Mevcut Reverse Solver kod tabanını incele.

Generator, Evaluator, Player Bot ve Monte Carlo bileşenlerinin nasıl çalıştığını belirle.

Hangi parçaların gerçekten machine learning kullandığını doğrula.

Mevcut metrikleri ve eksikleri raporla.

### PHASE 2 — METRICS

Level kalitesi için ölçülebilir metrikler oluştur.

Simülasyonları tekrar üretilebilir hâle getir.

Level seed, parametreler ve sonuçları kaydet.

### PHASE 3 — CALIBRATION

Mevcut gerçek oyuncu verileriyle simülasyon tahminlerini karşılaştır.

Oyuncu davranış modelinin eksiklerini belirle.

### PHASE 4 — MATHEMATICAL OPTIMIZATION

Loss function oluştur.

Level parametrelerini sistematik biçimde optimize et.

Finite difference, evolutionary search veya diğer uygun optimizasyon yöntemlerini dene.

### PHASE 5 — MACHINE LEARNING

Gerekli ve yeterli veri oluştuğunda oyuncu davranışı ve level kalitesi için öğrenilmiş modeller geliştir.

Neural network kullanımı anlamlıysa backpropagation, gradient descent ve regularization yöntemlerini uygula.

### PHASE 6 — AUTOMATED FEEDBACK LOOP

Generator → Simulator → Evaluator → Optimizer döngüsünü otomatikleştir.

Her yeni level versiyonunun önceki versiyonlara göre gerçekten daha iyi olup olmadığını ölç.

### PHASE 7 — HUMAN VALIDATION

Yeni oyuncularla testler yap.

Simülasyon sonuçlarıyla insan deneyimini karşılaştır.

Modeli gerçek kullanıcı geri bildirimleriyle geliştirmeye devam et.

## 14. CLAUDE / CODEX İÇİN TALİMATLAR

Bu belge, Reverse Solver projesinin uzun vadeli AI vizyonunu anlatıyor.

Bu aşamada senden bütün sistemi sıfırdan yeniden yazmanı istemiyorum.

Öncelikle mevcut kod tabanını incele.

Mevcut Generator, Evaluator, Player Bot ve Monte Carlo sistemlerinin gerçekten nasıl çalıştığını belirle.

Varsayımlarla hareket etme.

Matematiksel optimizasyonun nerede uygulanabileceğini araştır.

Hangi bileşenlerde machine learning gerekli, hangilerinde klasik algoritmalar daha uygun, açıkla.

Overfitting, dropout, regularization ve reward hacking gibi riskleri değerlendir.

Mevcut çalışan sistemi bozmadan, aşamalı ve ölçülebilir bir gelişim planı öner.

Her önerinin:

1. Matematiksel temelini,
2. Kullanılacak algoritmayı,
3. Gereken verileri,
4. Beklenen faydasını,
5. Nasıl test edileceğini

açıkla.

Sırf AI kullanmış olmak için gereksiz neural network veya reinforcement learning ekleme.

Temel hedefimiz daha fazla level üretmek değil, daha kaliteli level üretmeyi öğrenen bir sistem oluşturmak.

## 15. NİHAİ VİZYON

Uzun vadede istediğim sistem şu:

Ben yalnızca şöyle bir komut vereceğim:

“Bana 20 tane zor ama eğlenceli level oluştur. Oyuncuların düşünmesini sağla. Bazılarında son hamlede kazanma deneyimi olsun. Birbirine çok benzeyen leveller oluşturma.”

AI Level Designer levelleri üretecek.

AI Player Bot bu levelleri farklı stratejilerle binlerce kez oynayacak.

AI Evaluator sonuçları değerlendirecek.

Optimizer matematiksel olarak iyileştirme seçeneklerini araştıracak.

Generator yeni versiyonlar oluşturacak.

Bu döngü, yeterli kalite seviyesine ulaşana kadar tekrarlanacak.

İlk aşamada insan kontrolü olacak.

Uzun vadede sistem mümkün olduğunca otonom, taşınabilir, ölçeklenebilir ve sürekli gelişen bir AI Level Design Factory hâline gelecek.

Nihai hedefimiz, matematik ve makine öğrenmesiyle desteklenen, oyuncuların gerçekten keyif aldığı oyun bölümlerini sistematik biçimde tasarlayabilen bir yapay zekâ sistemi geliştirmek.
