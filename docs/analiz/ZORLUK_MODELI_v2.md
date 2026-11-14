# Zorluk modeli v2: bölüm bölüm kök neden + birleşik regresyon (PM, 14 Kasım)

Veri: pt1, 51 oyuncu, 1259 deneme, 82 bin hamle kaydı (`v_attempts`, `moves`, `level_features`). Kod ve ara çıktılar: `docs/analiz/v2/`.

## 0. Yöntem (kısaca)
1. **Oyuncu becerisi + bölüm zorluğu ayrı ayrı ölçüldü** (IRT / Rasch modeli). Böylece "geç bölümlere yalnız iyi oyuncular geldi" yanılgısı düzeltildi. Sonuç: her bölüm için "ortalama oyuncunun ilk denemede kazanma şansı".
2. Bu zorluk, bölümün tasarım özellikleriyle açıklanmaya çalışıldı (birleşik regresyon). Biri dışarıda bırakılarak sınandı (leave-one-out).
3. Açıklanamayan bölümlerde hamle kayıtlarına inildi: hangi bomba patladı, kaçıncı çıkışta, kazananlar bombalı parçayı kaçıncı çıkışta aldı, takılma oranı.

## 1. Ana bulgular

**1. Beceri farkı küçük, öğrenme büyük.** Oyuncu becerisi sapması 0,57 logit. Aynı bölümü tekrar oynamanın etkisi ise **+0,67 × log(deneme)**: 2. denemede +0,46, 4. denemede +0,93. Zor bölümler çoğunlukla "beceri" değil **"deneyip ezberleme"** ile geçiliyor.

**2. Zorluğun %73'ünü üç şey açıklıyor** (sınama hatası 2,18 → 1,13):

| Özellik | Katsayı (logit) | Anlamı |
|---|---|---|
| Bomba sıkılığı `exp(−pay/2)` | **+2,02** | pay 0 → +2,0; pay 1 → +1,2; pay 2 → +0,74; pay 4 → +0,27 |
| Bombanın erken gitmesi gerekmesi `1 − fitil/parça` | **+2,88** | fitil 8 / 54 parça → +2,45; fitil yarısı → +1,44 |
| Süre bolluğu `log(sn/hamle)` | **−1,17** | 2 sn → 1,5 sn: +0,34 |
| Kilit, zincir, duvar sayısı, parça sayısı | ≈ 0 | modeli iyileştirmiyor |

Ortalama oyuncunun ilk denemede kazanma şansı (2 sn/hamle):

| Fitil/parça | pay 0 | pay 1 | pay 2 | pay 4 | pay 8 |
|---|---|---|---|---|---|
| bombasız | | | %93 | | |
| 0,8 | %49 | %68 | %77 | %85 | %87 |
| 0,5 | %29 | %47 | %59 | %70 | %75 |
| 0,25 | %16 | %30 | %41 | %53 | %59 |
| 0,15 | %13 | %25 | %35 | %46 | %52 |

**3. Generator'daki "fitil payı" yanlış ölçülüyor.** Pay, çözücünün tek bir çözümünden hesaplanıyor, gerçek en kısa yoldan değil. Kanıt (oyuncuların bombalı parçayı en erken aldığı an):
- **S50:** kayıtlı pay 1, gerçek pay ≥ 12.
- **S17:** kayıtlı 2, gerçek 7.
- **S18:** kayıtlı 2, gerçek 5.

Oyuncu davranışından ölçülen pay zorlukla daha güçlü ilişkili: r = −0,78; kayıtlı payda r = −0,54.

**4. Asıl eksik özellik: bomba "doğal sırayla" uyumlu mu?** Aynı pay, aynı boyut, çok farklı sonuçlar:
- **S31 ve S32** (ikisi de 4 bomba, pay 0, 54 parça): ortalama oyuncu %72'ye %11. S31'de bombalı parçalar insanların zaten alacağı sırada çıkıyor.
- **S17 ve S18** (ikisi de pay 2, 33 parça): %95'e %19.
- **S08** (pay 11) yine de %32. Oyuncular bombaya giden yolu bırakıp 18 başka parça alıyor.

Yani zorluğu "pay" değil, **"oyuncunun doğal oynayışında bomba zamanında çıkıyor mu"** belirliyor. Bu, tahta geometrisinden hesaplanmalı (aşağıda PM_69).

**5. Süre kaybı = takılma.** Süreden kaybedenlerde takılma/çıkış oranı kazananların 3–5 katı:
- S06: 1,35'e 0,30.
- S43: 1,8'e 0,43.
- S36: 1,9'a 0,9.

Süreden kaybeden yavaş değil, parçanın yönünü anlamamış.

**6. Ölüm hep belli bir "boğaz" bombada.**
- **S35:** 42 ölümün 42'si 25 numaralı bombada, 29. çıkışta.
- **S28:** üç boğaz var: 8., 19. ve 33. çıkış. Oyuncu her seferinde bir sonrakine kadar ilerliyor (ortalama 5,6 deneme).

Bu "ezber merdiveni" eğlenceli zorluk değil, deneme-yanılma.

## 2. 40 bölüm: beklenen / olan / kök neden
Sütunlar:
- **v0.1:** asıl tahmin.
- **v1:** bot tahmini.
- **Gerçek:** pt1 ham ilk deneme.
- **Ort.:** ortalama oyuncu, beceriye göre düzeltilmiş.
- **Model:** yeni regresyon.

| # | Bölüm | Rol | v0.1 | v1 | Gerçek | Ort. | Model | Kök neden |
|---|---|---|---|---|---|---|---|---|
| 1 | S41 | öğretici | ,97 | 1,0 | ,98 | ,99 | ,99 | Tuttu. 9 parça, 120 sn: süre önemsiz. |
| 2 | S42 | öğretici | ,97 | 1,0 | ,94 | ,98 | ,98 | Tuttu. |
| 3 | S43 | kilit dersi | ,96 | 1,0 | ,95 | ,97 | ,95 | Tuttu. Kaybedenlerde takılma 4× (yön anlaşılmamış). |
| 4 | S06 | ısınma | ,93 | ,99 | **,75** | ,77 | ,92 | **Takılma → süre.** Kaybedenlerde takılma/çıkış 1,35 (kazanan 0,30), 30 parçanın 14'ü kalmış. 55 sn dar. |
| 5 | S44 | ısınma | ,96 | 1,0 | 1,0 | ,99 | ,94 | Tuttu. Kazananların %22'si booster denedi (ilk booster). |
| 6 | S45 | ısınma | ,96 | 1,0 | ,97 | ,99 | ,97 | Tuttu. İlk bomba, pay 10. |
| 7 | S46 | ısınma | ,95 | 1,0 | ,97 | ,99 | ,97 | Tuttu. |
| 8 | S47 | yükseliş | ,82 | ,80 | **,62** | ,42 | ,83 | **Doğal sırada geç bomba.** Pay 12 görünüyor, ama 18 numaralı bomba ölümlerin 21/26'sı. Oyuncular onu ortalama 12. çıkışta alıyor (fitil 20). 2 bomba ilk kez. |
| 9 | S20 | yükseliş | ,81 | ,57 | **,38** | ,47 | ,57 | **Kuralı anlamama.** Fitil 7: bombalı parçayı almadan 7 parça çıkarıp ölüyorlar, 2. denemede %69. p22'de 7. bölüm dersi bunu çözüyor. |
| 10 | Z24 | zincir dersi | ,91 | ,67 | ,74 | ,85 | ,93 | Takılma (0,54/çıkış) → süre %17. |
| 11 | S21 | yükseliş | ,70 | ,35 | **,29** | ,26 | ,44 | **Gerçek pay 2, boğaz bomba** (3 numara, fitil 16, 21/21 ölüm). Kazananlar da 14. çıkışta alıyor. 60 sn dar (kalan süre 10 sn). |
| 12 | S51 | yükseliş | ,74 | ,58 | ,71 | ,64 | ,63 | Tuttu. |
| 13 | S34 | yükseliş | ,66 | ,38 | ,60 | ,34 | ,57 | Pay 2. Gerçek %60 görünüyor ama becerisi yüksek grup oynadı; ortalama oyuncu %34. 4+ denemede %12. |
| 14 | Z21 | duvar dersi | ,91 | ,44 | ,94 | ,98 | ,92 | Tuttu. Duvar zorlaştırmıyor. |
| 15 | S13 | yükseliş | ,84 | ,96 | ,94 | ,93 | ,90 | Tuttu. Bombasız. |
| 16 | Z04 | yükseliş | ,68 | ,43 | **,43** | ,37 | ,56 | Pay 2, boğaz bomba 13 (11/11 ölüm). %29 booster. |
| 17 | S18 | yükseliş | ,65 | ,31 | **,14** | ,19 | ,52 | **Doğal sırada geç bomba.** Kayıtlı pay 2. Kazananların yarısı bombayı fitilin son 1–2 çıkışında alıyor. Aynı paydaki S17 %95. 3,5 deneme. |
| 18 | S22 | yükseliş | ,63 | ,57 | ,69 | ,67 | ,59 | Tuttu. |
| 19 | S28 | zirve | ,42 | ,02 | **,00** | ,08 | ,15 | **Ezber merdiveni:** 4 bomba, pay 0, boğazlar 8., 19. ve 33. çıkış. 5,6 deneme. p22'de 39'a taşındı. |
| 20 | S24 | orta | ,56 | ,17 | **,33** | ,34 | ,40 | Pay 1, boğaz bomba 1 (12/12). 55 sn dar: kazananlarda 7 sn kalıyor. |
| 21 | Z23 | orta | ,48 | ,22 | **,27** | ,23 | ,33 | Fitil 9 / 54 parça (erken bomba). 19 numara 16/21 ölüm. |
| 22 | S19 | orta | ,79 | ,96 | ,90 | ,91 | ,90 | Tuttu (bombasız). |
| 23 | S27 | zirve | ,68 | ,70 | ,40 | ,44 | ,38 | Fitil 6 / 39. Model doğru buldu; v0.1 kolay sanmıştı. |
| 24 | S14 | orta | ,84 | ,95 | ,90 | ,89 | ,90 | Tuttu. |
| 25 | S35 | zirve | ,44 | ,01 | **,00** | ,06 | ,30 | **Tek boğaz:** 42/42 ölüm 25 numarada (29. çıkış). 7,1 deneme. En kötü tasarım. |
| 26 | S17 | nefes | ,69 | ,52 | **1,0** | ,95 | ,59 | **Pay yanlış ölçülmüş** (kayıtlı 2, gerçek 7) + bomba doğal sırada. |
| 27 | S10 | nefes | ,79 | ,76 | **1,0** | ,95 | ,67 | Pay 10, bomba doğal sırada. Model biraz kötümser. |
| 28 | S50 | nefes | ,73 | ,60 | ,88 | ,85 | ,59 | **Pay yanlış:** kayıtlı 1, gerçek ≥12. |
| 29 | S52 | nefes | ,81 | ,64 | ,88 | ,85 | ,81 | Tuttu. |
| 30 | S37 | zirve | ,41 | ,03 | ,38 | ,30 | ,26 | Tuttu (pay 0, 4 bomba). Takılma 1,2. |
| 31 | S08 | nefes | ,78 | ,79 | **,50** | ,32 | ,74 | **Doğal sırada geç bomba.** Pay 11, ama yarısı bombayı almadan 18 parça alıyor. Nefes bölümü olmamış. |
| 32 | S31 | zor | ,54 | ,70 | **,75** | ,72 | ,42 | **Kolay.** Pay 0 ama bombalar doğal sırada çıkıyor; 170 sn çok bol (kalan 100 sn). |
| 33 | S39 | zor | ,43 | ,05 | ,25 | ,25 | ,25 | Tuttu. Boğaz bomba 37. |
| 34 | S32 | zor | ,43 | ,02 | **,00** | ,11 | ,19 | **Ezber:** 4 bomba, pay 0, boğaz 36 (fitil 17), 19/24 ölüm. |
| 35 | S36 | zor | ,51 | ,41 | ,43 | ,45 | ,30 | **Takılma → süre** (%57). Kaybedende takılma/çıkış 1,9. Bomba değil. |
| 36 | S29 | zor | ,44 | ,01 | ,29 | ,18 | ,20 | Fitil 7 / 62 parça, 3 boğaz. 3,6 deneme. |
| 37 | Z25 | nefes | ,55 | ,35 | **,86** | ,83 | ,57 | Kolay. 4 bomba ama doğal sırada, 145 sn bol. |
| 38 | S38 | final | ,45 | ,09 | **,00** | ,04 | ,13 | **Ezber + süre.** 3 boğaz (8., 17., 29. çıkış), 8,1 deneme. En zoru. |
| 39 | S40 | final | ,44 | ,56 | ,57 | ,41 | ,32 | Tuttu. Süre %29. |
| 40 | S33 | final | ,43 | ,06 | **,00** | ,12 | ,20 | **Ezber:** fitil 8 / 62, iki boğaz (8. ve 27. çıkış). |

Not: 25'ten sonra 7–10 oyuncu var, düzeltilmiş değerler yine de belirsiz (±0,15).

## 3. Kök neden türleri (generator için kural)
| Tür | Bölümler | Ne yapmalı |
|---|---|---|
| A. Bomba doğal sırada geç çıkıyor | S47, S18, S08 | Yeni özellik: "doğal oyuncu bombayı zamanında alıyor mu" (simülasyon). Pay tek başına yetmez. |
| B. Ezber merdiveni (çok boğaz, pay 0) | S28, S35, S32, S38, S33, S29 | Bir bölümde en çok 1 boğaz bomba (pay ≤1). Pay 0 yalnız tek bombada, zor bölümde. |
| C. Kuralı anlamama | S20 | Ders bölümü (p22'de 7) + ilk bombalı bölümlerde pay ≥8. |
| D. Takılma → süre | S06, Z24, S36, (S43) | Takılma tuzağı ölçüsü: açık kenarlı görünüp takılan parça sayısı. Isınma bölümlerinde sınırlı tut, sürede +10 sn. |
| E. Pay yanlış ölçülmüş | S17, S50 (ve büyük olasılıkla diğerleri) | Payı gerçek en kısa yoldan yeniden hesapla. |
| F. Süre çok bol | S31, Z25, S28, 1–2 | Süreyi sn/hamle ≈ 2,0'ye çek (bomba bölümlerinde süre ikincil). |

## 4. Henüz ölçemediğimiz, motor gerektiren özellikler (PM_69)
1. **Gerçek pay:** her bomba için en kısa yol (çözücü bombayı önceliklendirerek), fitil eksi bu yol.
2. **Doğal oyuncu hayatta kalma olasılığı:** rastgele/açgözlü oynayan bir politikayla Monte Carlo. Bombaların ne kadar sıklıkla zamanında çıktığı ve ortalama "doğal pay".
3. **Açık hamle sayısı:** her adımda kaç parça çıkabilir, bunların kaçı bombaya giden yolda. Zeyd'in "açık hamle 1 ise x kadar zor" fikri. Hem çeldirici sayısı hem darboğaz.
4. **Takılma tuzağı:** çıkabilecekmiş gibi görünüp takılan parça sayısı (yön belirsizliği).
5. **Boğaz sayısı:** fitili sıkı (pay ≤1) bomba sayısı.
6. **pt1 replay:** 82 bin hamleyi motorda yeniden oynat. Her bomba ölümünde kaç hamle "yol dışı" gitmiş, her takılmanın nedeni ne.

Bunlar gelince aynı modeli bu özelliklerle yeniden kuracağım (hedef: hata 1,13 → ~0,7). Generator bu modelle puanlayacak: her bölümün "ortalama oyuncu ilk deneme şansı" tahmin edilip hedef aralığa uyanlar seçilecek. Zeyd'in kendi oyunları da beceri ekseninde en üste yerleştirilecek; botun neden iyimser olduğu sayısal görünecek.
