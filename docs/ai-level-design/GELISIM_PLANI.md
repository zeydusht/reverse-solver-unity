# AI Level Design: gelişim planı (PM_97, 23 Kasım 2026)

**Dayanak:** `DURUM_HARITASI.md`. Sıra:

- **(a)** soft launch'tan önce, riski sıfır, yalnız ölçüm ve rapor;
- **(b)** `sl1` verisi gelince;
- **(c)** daha fazla veri gerekince.

**Kurallar:**

- Hiçbir öneri soft launch'ı geciktirmez.
- (a) maddeleri oyun koduna ve yayına dokunmaz. Yalnız editör ve araç tarafında ölçüm ve rapordur.
- Her karmaşık yöntem için "neden daha basiti yetmez" yazıldı. Yetiyorsa basiti seçildi.

**Telemetri:** yayından önce yeni alan **gerekmiyor**.

- Yakın kaçış: `plays.left_s` + `level_features.timer_s`.
- Son hamlede kazanma: `left_s` + son çıkış zamanı (`moves.t_ms`).
- Kayıpta kalan parça: parça sayısı − `moves` çıkış sayısı.
- Bombayı patlatan parça: `moves.fuses`.

Hepsi bugünkü kayıtlardan türetilebiliyor (pt1'de bu denetimde türetildi).

---

## (a) Soft launch'tan önce: riski sıfır (yalnız ölçüm ve rapor)

### a1. Deneyim metrikleri: "son hamlede kazanma" ve "yakın kaçış" gerçek veride ve simde (≈ 5 saat)

1. **Matematik:** bölüm başına oranlar, Wilson %95 aralığıyla. Sim ile gerçek arasında Spearman sıra ilişkisi ve ortalama mutlak fark.
2. **Algoritma:**
   - Tanımlar sabitlenir:
     - **son hamlede kazanma:** son çıkış, sürenin son %10'unda;
     - **yakın kaçış:** kayıpta ≤ 3 parça kaldı.
   - pt1 için SQL / Python ile hesaplanır.
   - Aynı bölümlerde FunSim'in `NearMissRate`'i hesaplanıp karşılaştırılır.
3. **Veri:** var (pt1: 698 kazanç, 536 kayıp, 82 bin hamle).
4. **Fayda:** vizyonun sevdiği deneyimin bugünkü sıklığı ölçülür. Gerçekte nadir: kazançların %4,6'sı, kayıpların %2,8'i. Simin bunu doğru tahmin edip etmediği görülür. Bu sayı olmadan "son hamlede kazanma ihtimali yüksek bölüm" hedeflenemez.
5. **Test:** pt1 bölümleri iki yarıya ayrılır; tanım eşikleri ilk yarıda seçilip ikinci yarıda raporlanır.
- **Overfitting riski:** eşikleri (%10, 3 parça) veriye bakarak ayarlamak. Eşikler baştan sabitlenir.
- **Reward hacking riski:** yok (yalnız ölçüm). İleride hedef olunca: süre kısılarak yapay "son saniye" üretmek. a4'teki hüsran terimi bunu dengeler.

### a2. Model v2'nin dürüst yeniden sınanması ve tekrarlanabilirliği (≈ 6 saat)

1. **Matematik:**
   - İç içe biri-dışarıda (nested LOO): özellik ve %85/%15 karışım ağırlığı her katın içinde seçilir.
   - Bölüm ailesine göre gruplu katlar: aynı bölümün v2/v3 sürümleri aynı katta.
   - Kalibrasyon eğrisi; bootstrap ile tahmin aralığı.
2. **Algoritma:** bugünkü ağırlıklı ridge (`final.py`) aynen. Yalnız sınama şekli değişir.
3. **Veri:** var. `level_features_v2.csv`, `irt_level_d.csv` ve girdi tablosu repoya konur (bugün asıl ara dosya `P.pkl` repoda değil).
4. **Fayda:**
   - Modelin gerçek hatası öğrenilir. Bu denetimde basit LOO bile 12,3 puan çıktı, belgedeki 10,7 değil.
   - sl100 hedef bantlarının (±5–10 puan) model hatasından dar olduğu görünür hâle gelir.
   - Her tahmin aralıklı verilir.
5. **Test:** iç içe LOO MAE ve hep-ortalama tabanı. Aralık kapsaması: %80'lik aralıkların gerçeği ≈ %80 tutması.
- **Overfitting riski:** iç içe olmayan LOO'nun iyimserliği; aile sızıntısı. Bu iş bunları ölçmek için var.
- **Reward hacking riski:** yok.

### a3. Zeyd'in geri bildirimi: etiketli veri seti (≈ 4 saat)

1. **Matematik:** yok, veri düzenleme. Sonra yalnız betimleme: etiket ile ölçülen özellik arasında Spearman.
2. **Algoritma:**
   - Şu kaynaklardan bölüm kimliğine bağlanan `docs/ai-level-design/geri_bildirim.csv` çıkarılır:
     - `plays.zorluk_puani` (48 puan);
     - `PRODUCT.md`, PM mesajları ve `PT1_GERIBILDIRIM.md`'deki sözlü yorumlar.
   - Sütunlar: bölüm, sürüm, tarih, puan, sabit sözlükten etiketler.
   - Etiket sözlüğü: kolay, zor, sıkıcı, kısa, düşündürdü, son-hamle, tatmin, etiket-yanlış, strateji-yok.
   - Belirsiz eşleşmeler "?" ile bırakılır.
3. **Veri:** var ama dağınık; tek değerlendirici.
4. **Fayda:** reward modelinin ilk ölçütü. Hangi ölçülen özelliğin "sıkıcı" ya da "tatmin" ile birlikte gittiği görülür. Ör. ZeydBot'ta bomba payı tek başına puan sırasını 0,79 ile yakalıyor.
5. **Test:** etiketlerin %20'si ikinci kez, kör olarak etiketlenir. Tutarlılık (Cohen κ) raporlanır.
- **Overfitting riski:** tek kişinin zevki. Bu set **eğitim için değil, ölçüt** olarak kullanılır.
- **Reward hacking riski:** yok (ölçüm).

### a4. Tek, sürümlü kalite fonksiyonu (Loss v1) ve sl100'ün geriye dönük puanı (≈ 4 saat)

1. **Matematik:**

   ```
   L = w1·|p̂ − hedef|/σ_model          zorluk hatası, model hatasıyla ölçekli
     + w2·max(0, s − s0)               tekrar: komşu bölümlere benzerlik (Similarity)
     + w3·max(0, hüsran − h0)          FunSim hüsran oranı ve haksız nedenler
     + w4·ihlal sayısı                 DifficultyModelV2.Rules
     + ∞·[geçersiz]                    çözülemez bölüm
   ```

   - Ağırlıklar sl100'ün bugünkü maliyetinden alınır; yazılı ve sürümlüdür.
   - Hata payına bölmek, "model emin değilken hedefe yakınlık" puanını şişirmez.
2. **Algoritma:** sl100'ün 100 bölümü L ile puanlanır; en kötü 10 slot ve hangi terimden kötü oldukları listelenir. Kod değişmez.
3. **Veri:** var (model tahminleri, FunSim, Similarity).
4. **Fayda:**
   - Üç dağınık puanın (sl100 maliyeti, BandDistance, FunSim eşikleri) yerine tek bir dil.
   - b4'te ağırlıklar gerçek veriyle ayarlanabilir.
   - Vizyondaki Loss formülünün somut ilk sürümü.
5. **Test:**
   - L ile pt1'deki gerçek bırakma (bölümden sonra çıkış) ve Zeyd'in puanı (a3) arasında Spearman.
   - Ağırlıklar bu teste bakılarak değiştirilmez (yalnız raporlanır).
- **Overfitting riski:** ağırlıkları sonuca bakıp oynamak. v1 dondurulur.
- **Reward hacking riski:**
  - Hüsran terimi FunSim'in varsayımlarına dayandığı için arama FunSim'in kör noktalarına kayabilir. Ör. FunSim'in "bomba dikkati"ni yanıltan ama insanı yoran bölümler.
  - Önlem: FunSim, gerçek veriyle kalibre edilene kadar (b2) yalnız ceza olarak kullanılır, ödül olarak değil.

### a5. Çözüm yolu çeşitliliği metriği (≈ 5 saat, yalnız editör aracı)

1. **Matematik:** rastgele oyuncunun ilk k çıkış dizilerinin Shannon entropisi. Farklı tam yol sayısının Chao1 (gözlenmeyen tür) tahmini.
2. **Algoritma:** mevcut `MonteCarlo` koşularında yol önekleri sayılır. Ek bir motor gerekmez (`FastState`).
3. **Veri:** gerekmez (simülasyon).
4. **Fayda:** vizyondaki "kaç farklı çözüm yolu var" sorusu yanıtlanır. "Strateji gerektirmiyor" yorumunun ölçülebilir karşılığı olabilir. Doğrulaması a3 etiketleriyle yapılır.
5. **Test:** tohum değişince metrik ±%5 içinde kalmalı (kararlılık). a3'teki "strateji-yok" etiketli bölümlerle ilişkisi raporlanır.
- **Overfitting riski:** düşük (parametresiz).
- **Reward hacking riski:** ileride hedef olunca "çok yol = iyi" sanılabilir. Oysa yol çokluğu düşündürmeyi azaltabilir. Hedef olarak değil, bantlı kullanılmalı.

---

## (b) Soft launch verisi (`sl1`) gelince

### b1. Model v2'nin ilk gerçek dış sınavı + kalibrasyon (≈ 6 saat; sl1'de bölüm başına ≥ 30 oyuncu olunca)

1. **Matematik:** Rasch / IRT sl1'de yeniden kurulur. Model v2 tahminine **Platt kalibrasyonu** uygulanır: p' = σ(α + β·logit p̂), yalnız iki parametre.
2. **Algoritma:** model v2 **değiştirilmeden** sl100'ün oynanan bölümlerinde sınanır. Sonra yalnız α ve β ayarlanır.
3. **Veri:** soft launch ile gelecek.
4. **Fayda:** pt1 dışında hiç sınanmamış modelin gerçek hatası öğrenilir. Hedef MAE ≤ 12; şu an bilinmiyor. Bantlar buna göre genişletilir ya da daraltılır.
5. **Test:** sl1 oyuncuları ikiye bölünür (oyuncu bazında): kalibrasyon ilk yarıda, ölçüm ikinci yarıda.
- **Neden daha karmaşığı değil:** 100 bölüm ve iki parametre. Fazlası aşırı uyum.
- **Overfitting riski:** α ve β'yı aynı oyuncularda ayarlayıp ölçmek. Bölünerek önlenir.
- **Reward hacking riski:** yok.

### b2. Oyuncu profilleri (Beginner / Average / Expert) ve FunSim kalibrasyonu (≈ 12 saat)

1. **Matematik:**
   - IRT beceri tahmininin üçte birlikleri profil olur.
   - Her profil için bomba dikkati ve düşünme süresi dağılımı, hiyerarşik (kısmi havuzlama) modelle tahmin edilir. Bugünkü `bot_fit.py` ile aynı empirik Bayes.
   - FunSim sabitleri (hız, sabır, dikkat) bu tahminlerle değiştirilir.
2. **Algoritma:** koşullu logit (hamle seçimi) + log-normal süre; profil başına ortalama, oyuncu başına sapma.
3. **Veri:** pt1 (53 oyuncu, 82 bin hamle) + sl1 hamleleri.
4. **Fayda:**
   - Bölüm başına "yeni oyuncu için" ve "deneyimli için" ayrı zorluk.
   - FunSim'in yakın kaçış oranı gerçekle uyuşur hâle gelir (a1 karşılaştırmasıyla ölçülür).
   - PM_25'te botlar tabanı geçemedi (Brier 0,208'e karşı 0,095). Hedef: tabanı geçmek.
5. **Test:** oyuncu-dışarıda ve bölüm-dışarıda katlar (PM_25'teki `geri_test.py` aynen). Brier ve MAE, fitil tabanı ve sabit taban ile karşılaştırılır.
- **Neden basit değil:** profil olmadan model tek bir "ortalama insan". Vizyonun Beginner/Expert sorusu yanıtlanamaz. Yine de profil sayısı 3'te tutulur.
- **Overfitting riski:** Expert sınıfında az oyuncu (pt1'de ≥ 30 deneme yapan 12 kişi). Kısmi havuzlama bunun için.
- **Reward hacking riski:** üretici, botların zayıf noktasına uyan bölümler üretebilir. Ör. botun hiç takılmadığı ama insanın takıldığı yön belirsizliği. Takılma/çıkış gerçek veriyle ayrıca izlenir.

### b3. "Bomba fark ediliyor mu" özelliği (≈ 7 saat)

1. **Matematik:** model v2'ye 2–3 yeni özellik:
   - bombalı parçanın kenara uzaklığı;
   - ilk ekranda bombaya giden yolun açık mı olduğu;
   - açgözlü oyuncunun bombadan önce aldığı parça sayısı.

   Aynı ağırlıklı ridge.
2. **Algoritma:** yeni özellikler motorla hesaplanır (`DifficultyV2`); iç içe CV ile eklenip eklenmemesine karar verilir.
3. **Veri:** pt1 + sl1.
4. **Fayda:** modelin en büyük hatası bombalı ve "kolay ama model zor sanıyor" bölümlerde: S10 95/51, S17 95/59, Z21 98/55. Hedef: bombalı bölüm MAE'si 14 → ≤ 10.
5. **Test:** pt1'de iç içe LOO; sl1'de dış sınav (b1 düzeni).
- **Overfitting riski:** 6 aykırı bölüme bakarak özellik icat etmek. Özellikler önce yazılı tanımlanır, sonra bakılır; sl1'de sınanmadan kabul edilmez.
- **Reward hacking riski:** üretici bombayı "görünür ama yine de ölümcül" konumlara itebilir. Hüsran terimi (a4) ve gerçek bomba ölüm yerleri izlenir.

### b4. Gerçek oyunculardan "keyif" sinyali: devam etme modeli (≈ 9 saat)

1. **Matematik:** lojistik regresyon. Hedef: oyuncu bölümden sonra aynı oturumda bir sonrakini oynadı mı (ya da ertesi gün döndü mü).

   ```
   P(devam) = σ(β0 + β1·p̂ + β2·takılma/çıkış + β3·[yakın kaçış] + β4·[son hamlede kazanma] + β5·kalan süre payı + oyuncu etkisi)
   ```

2. **Algoritma:** karışık etkili lojistik (oyuncu rastgele etki); L2 ceza.
3. **Veri:** sl1 (binlerce deneme). Zeyd'in tek kişilik puanından çok daha büyük ve çok kişili bir ödül sinyali.
4. **Fayda:**
   - a4'teki Loss ağırlıkları (w1–w3) veriyle ayarlanır.
   - "Son hamlede kazanma oyuncuyu tutuyor mu" sorusu ölçülür; vizyonun varsayımı sınanır.
   - pt1'de 10. bölümden önce bırakanların çoğu son bölümünü kazanarak çıkmıştı. Bu model bunu açıklamaya çalışır.
5. **Test:** zamana göre bölme (ilk haftalar eğitim, son hafta test). AUC ve kalibrasyon; yalnız p̂ kullanan modele karşı.
- **Neden basit:** sinyal gürültülü. Yüzlerce bölümde 5–6 katsayı güvenilir, daha fazlası değil.
- **Overfitting riski:** oturum uzunluğu gibi bölümle ilgisiz etkenler; oyuncu etkisiyle ayrılır.
- **Reward hacking riski: yüksek.** "Devam" en kolay bölümlerle artar ve tutunma kısa vadede yükselir ama oyun sıkıcılaşır. Önlem:
  - devam modeli yalnız zorluk bantları **içinde** sıralama için kullanılır;
  - bantları değiştirme yetkisi yok;
  - Zeyd'in etiketleri (a3) ayrı ölçüt olarak izlenir.

### b5. Set düzeyinde seçim: açgözlü yerine bütün eğri (≈ 7 saat; yalnız 101+ bölümler için)

1. **Matematik:** Σ L(slot) + λ·Σ benzerlik(komşu) toplamını en aza indiren atama.
2. **Algoritma:** bugünkü açgözlü atamadan başlayan benzetimli tavlama (simulated annealing) ya da yerel takas. Slot sayısı ≤ 100, aday havuzu birkaç bin; dakikalar içinde biter.
3. **Veri:** gerekmez (a4'teki L).
4. **Fayda:** bugün slotlar sırayla doluyor; erken slot iyi adayı "harcayıp" sonrakini kötü bırakabiliyor. Fayda toplam L'deki düşüşle ölçülür. sl100 aynen kalır; yalnız yeni bölümlere uygulanır.
5. **Test:** aynı havuzda açgözlü ile tavlama toplam L'si; rastgele tohumlarla kararlılık.
- **Overfitting riski:** yok (veri öğrenmesi yok); L'nin kendi kusurlarını büyütme riski var.
- **Reward hacking riski:** L'nin zayıf terimini sömürme. Ör. Similarity'nin görmediği "aynı fikir, farklı boy" tekrarları. Örneklem Zeyd'e gösterilir.

---

## (c) Daha fazla veri gerektirenler

| # | Öneri | Matematik / algoritma | Veri koşulu | Beklenen fayda | Test | Neden daha basiti yetmez / risk | Saat |
|---|---|---|---|---|---|---|---|
| c1 | Zorluk modeli için gradient boosting | Ağaç toplulukları (monotonluk kısıtıyla: fitil ↑ → zorluk ↓) | ≥ 150 bölüm, her biri ≥ 30 oyuncu | Doğrusal modelin kaçırdığı etkileşimler (bomba × duvar), MAE ≤ 8 | Gruplu CV; ridge'e karşı. Fark < 1 puansa ridge kalır | 40–100 bölümde ağaçlar ezberler. **Risk:** aşırı uyum; monotonluk kısıtı ve erken durdurma | 8 |
| c2 | Vekil model ile Bayesçi optimizasyon (üretici düğmeleri) | Gauss süreci vekili + beklenen iyileşme (expected improvement) | Doğrulanmış L (a4 + b4) ve MAE ≤ 10'luk zorluk modeli | Aynı kalitede bölümü daha az adayla bulmak | Aynı süre bütçesinde tepe tırmanmaya karşı en iyi L | Bugünkü arama yeterince hızlı; kazanç ancak pahalı FunSim değerlendirmesinde. **Risk:** vekilin hatasını sömürme; her finalist gerçek simle yeniden ölçülür | 14 |
| c3 | Tercih öğrenme (hangisi daha iyi) | Bradley–Terry: P(A > B) = σ(r_A − r_B), r = özelliklerin doğrusal birleşimi | ≥ 200 ikili karşılaştırma (Zeyd + birkaç tasarımcı ya da oyuncu) | Puan ölçeği sorunu olmadan "kalite" sıralaması; reward modelin çekirdeği | Görülmemiş bölüm çiftlerinde isabet; değerlendiriciler arası tutarlılık | Mutlak 1–10 puanı kişiden kişiye kayıyor; ikili karşılaştırma daha az veriyle kararlı. **Risk:** az değerlendirici = tek zevk; çok değerlendirici şartı | 6 + toplama |
| c4 | Sinir ağı / pekiştirmeli öğrenme oyuncusu | Politika ağı (hamle seçimi); dropout + L2 | ≥ 1000 oyuncu, ≥ 1 milyon hamle; c1 ve b2 tavana dayanmış olmalı | Daha insan-benzeri hamle seçimi | b2 ile aynı geri test; Brier ile koşullu logite karşı | **Bugün önerilmiyor:** koşullu logit (birkaç parametre) PM_25'te bile tabanı geçemedi; sorun model gücü değil veri ve özellik. RL ajanı "en iyi oyuncu"yu öğrenir, "ortalama insan"ı değil. Dropout yalnız bu aşamada anlamlı | 30+ |

---

## Önerilen sıra ve toplam

| Aşama | İş | Saat |
|---|---|---|
| (a) şimdi | a1 deneyim metrikleri → a2 model v2 dürüst sınama → a3 geri bildirim seti → a4 Loss v1 → a5 yol çeşitliliği | ≈ 24 |
| (b) sl1 ile | b1 dış sınav + kalibrasyon → b3 bomba fark edilirliği → b2 profiller → b4 devam modeli → b5 set eniyileme | ≈ 41 |
| (c) | c1 → c3 → c2; c4 yalnız koşulları oluşursa | ≈ 28+ |

**Ölçülebilir başarı tanımı** (vizyondaki "%80 → %100" yerine):

- sl1'de model MAE ≤ 10 ve %80'lik aralıkların kapsaması %75–85;
- botların Brier skoru fitil tabanından iyi;
- Loss v1 ile gerçek devam oranı arasında Spearman ≥ 0,4;
- yeni bölümlerin Zeyd etiketlerinde "sıkıcı" ve "kolay" payının azalması (a3 ile ölçülür).
