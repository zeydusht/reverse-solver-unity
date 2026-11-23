# AI Level Design: durum haritası (PM_97, 23 Kasım 2026)

**Kapsam:** yalnız okuma. Kod, belgeler ve Supabase'te salt okuma sorguları (SELECT). Hiçbir şey değiştirilmedi.

**Kaynak:** vizyon `AI_Level_Design_Vision.md`. "Kanıt" sütunundaki sayılar ya bir dosyadan ya da bu denetimde yeniden ölçüldü; hangisi olduğu yazıyor.

## Kısa cevaplar (PM_97'nin 6 sorusu)

### 1. ZeydBot öğrenilmiş bir model mi?

Sinir ağı ya da derin öğrenme değil. Ama tamamen kurallı da değil: **Zeyd'in hamlelerinden parametreleri tahmin edilmiş küçük istatistik modeller ile kurallı bir yol motorunun birleşimi.**

- **Motor yarısı:** `Assets/_Game/LevelEditor/ZeydBot.cs` (`ZeydBot.Paths`).
  - Kurallı. Her bölümü 12 kez yalnız "doğru" hamlelerle rastgele oynar.
  - Her adımda kaydettikleri: kalan parça, seçenek sayısı, doğru seçenek sayısı, en dar bomba payı.
  - Hiçbir oyun sonucunu okumaz.
- **İnsan yarısı:** `Tools/zeydbot_model.py` (v1) ve `Tools/zeydbot_model_v2.py` (v2, `zeydbot-v2` dalı).
  - **Tuzağa düşme:** P = sigmoid(a + b · logit(tuzak payı)). a ve b, Zeyd'in ilk denemelerindeki çıkış hamlelerinden **Newton yöntemiyle, L2 cezalı lojistik regresyon** ile bulunuyor (`fit_logistic`, l2 = 0,5, b 1'e çekiliyor).
  - **Düşünme süresi:** log(süre) ~ seçenek sayısı, "tek doğru var mı", kalan parça; **en küçük kareler** (`ols`).
  - **1–10 puanı:** kayıp şansı ve kullanılan süre payı üzerinden doğrusal model.
  - Bu parçalar yolları Monte Carlo ile 400 kez oynar.
  - v2 eklemeleri: "bombaya önce gitme" oranı (önem ağırlıkları), öğrenme (önceden oynanan bölüm sayısı), puan modeline "yanıltıcı parça".
- **Eğitim adımı var**, ama tek bir kişinin (Zeyd'in) yaklaşık 40 bölümlük verisinde; her katman 2–4 parametreli. Biri dışarıda doğrulama (LOO) ile sınanmış.
- **Sonuç** (`docs/analiz/zeydbot/loo.md`, Z01–Z20):

  | | ZeydBot | rastgele oyuncu | bomba payı tabanı |
  |---|---|---|---|
  | Puan sırası (Spearman) | 0,73 | 0,61 | **0,79** |
  | Kazandı/kaybetti doğruluğu | %70 | %60 | %70 |
  | Brier (düşük iyi) | 0,233 | **0,162** | 0,203 |

  Yani ZeydBot basit bir tek-özellik tabanını geçemiyor.

### 2. Monte Carlo metrikleri: hangileri ölçülüyor?

Tablo aşağıda (C bölümü). Kısaca:

- **Simülasyonda var:**
  - kazanma, ilk denemede kazanma, 3 denemede kazanma;
  - bomba ve donma (ölü uç) oranı;
  - ortalama hamle, dallanma, ilk deneme kayıpları;
  - yakın kaçış (near-miss: sürenin son %10'unda kazanma ya da ≤ 3 parça kala kayıp);
  - keyif / hüsran / sıkılma, "aha" oyuncusu, booster kullanımı.
- **Yok:** çözüm yolu çeşitliliği (gerçek yol sayımı), "son hamlede kazanma" ayrı bir metrik olarak.
- **Uygulanmaz:** kalan hamle (oyunda hamle sınırı yok; karşılığı kalan süre).

### 3. Model v2'nin gerçek isabeti

**Belgelenen** (`ZORLUK_MODELI_v2.md` §5): pt1'in 40 bölümünde LOO ortalama mutlak hata **±10,7 puan**.

**Bu denetimde yeniden hesaplandı** (repodaki `level_features_v2.csv` + `irt_level_d.csv`; aynı formül ve ağırlıklar):

- LOO MAE **12,3 puan**, RMSE 17,2, en büyük hata 44 puan.
- Bölümlerin %60'ı ±10,7 içinde.
- Hep-ortalama tabanı 30,7 puan.
- 10,7 ile 12,3 arasındaki fark girdi tablosundan: asıl çalışmanın ara dosyası (`P.pkl`) repoda yok, birebir yeniden üretilemiyor.

| Bölüm tipi | n | MAE | Yön |
|---|---|---|---|
| Bombasız | 8 | **4,2** | sapma yok |
| Bombalı | 32 | **14,4** | |
| Gerçekte kolay (ilk deneme ≥ %80) | 16 | 15,6 | **model 15 puan fazla kötümser** |
| Gerçekte zor (< %50) | 20 | 11,0 | model 8 puan fazla iyimser |
| 21–40. slot | 20 | 13,0 | |

**En çok yanıldığı bölümler** (gerçek → tahmin):

- S10 95 → 51;
- Z21 98 → 55;
- S17 95 → 59;
- Z24 85 → 53;
- S08 32 → 64;
- S21 26 → 54.

Hepsi bombalı, "bombaya bakan oyuncu kurtulur, rastgele oyuncu ölür" tipinde. Model, oyuncunun bombayı fark edip etmediğini (konum, görünürlük) bilmiyor.

**Uyarılar:**

- Özellikler ve %85/%15 karışım ağırlığı **aynı 40 bölümde** seçildi. LOO bu seçimi kapsamıyor, yani gerçek hata büyük olasılıkla daha yüksek.
- Model pt1 dışında **hiç sınanmadı**: p22 ve p23 kohortlarında birer oyuncu var.

### 4. Hamle verisi ne kadar?

Bu denetimde Supabase'ten sayıldı; test oyuncuları ve dışlananlar hariç.

| Kohort | Oyuncu | Deneme | Hamle satırı | `input` dolu |
|---|---|---|---|---|
| pt1 (1–17 Kasım) | 53 | 1277 | **82 171** (33 bin çıkış, 23 bin takılma) | 0 |
| ic | 30 | 55 | 1 335 | 109 |
| eski ve diğer (p21–p23, tf2) | 21 | 472 | 3 028 | 396 |
| **Zeyd** (`ddd`, `Zeyd`) | 1 | 167 | 4 084 | — |

- `input` (fiske/sürükleme) alanı 10 Kasım'dan beri yazılıyor; pt1 satırlarında boş. Bu yüzden yalnız 509 satırda dolu.
- **pt1:** 53 oyuncunun 31'i ≥ 10 deneme, 12'si ≥ 30 deneme oynadı. 52'sinin bulmaca deneyimi profili var.
- IRT oyuncu becerisi sapması 0,57 logit. Öğrenme etkisi bundan büyük: +0,67 · log(deneme).

**Beginner / Average / Expert için yeterli mi?**

- **Kaba bir 3 sınıf için evet:** beceri üçte birlikleri, 10–18 oyuncu × binlerce hamle.
- **Sınıf başına ayrı bir hamle seçim modeli için sınırda.** Expert sınıfında ≥ 30 deneme oynayan yalnız 12 kişi var.
- **Asıl veri soft launch'la gelecek:** `sl1` kohortu henüz yok.

### 5. Zeyd'in öznel geri bildirimleri kayıtlı mı?

**Sayısal kısmı kayıtlı:**

- `plays.zorluk_puani`: Zeyd'in 1–10 puanı, **48 deneme**.
- ZeydBot dosyaları: `docs/analiz/zeydbot/denemeler.csv`, `hamleler.csv`.
- `keyif` (👍/👎) yalnız 1 satır; oyunda kapalı.

**Sözlü kısmı dağınık:**

- "çok kolay", "sıkıcı", "son hamlede çözdüm" gibi yorumlar `PRODUCT.md` karar kaydında, PM mesajlarında ve `PT1_GERIBILDIRIM.md`'de serbest metin olarak duruyor.
- Bölüm kimliğine bağlı, etiketli bir veri seti (bölüm, sürüm, etiket, puan) **yok**.

**Reward model için:** 48 puan + birkaç düzine etiketlenebilir yorum, tek bir değerlendiriciden. Ancak çok basit bir sıralama modeli için yeterli (2–3 özellik); "eğitim" için değil, **ölçüt** olarak kullanılmalı.

### 6. Bugünkü seçimin kalite fonksiyonu (sl100, PM_82)

`Tools/sl100/Design.cs` (`Assign`) her slot için adaylara elle ağırlıklandırılmış bir **maliyet** veriyor ve en düşüğünü seçiyor:

```
maliyet = 10·|p̂ − hedef| + 5·[bant dışında]
        + 0,7·(kırılan tasarım kuralı sayısı)          (boğaz bomba, takılma vb.; DifficultyModelV2.Rules)
        + 0,8·[komşu slotla aynı tahta boyu] + 50·[komşuyla aynı bomba imzası]
        + tema / kare tahta / "pt1'de oynandı" düzeltmeleri (±0,1–1,0)
```

- p̂: model v2'nin "ortalama oyuncu ilk deneme" tahmini. Süre, p̂'yi hedefe en yaklaştıran saniye seçilerek ayarlanıyor.

**Vizyondaki Loss'a yakınlığı.** Dört terimin üçü var:

- Difficulty Error: 10·|p̂ − hedef|.
- Repetition: boyut / bomba imzası. Fabrikada ayrıca `Similarity`.
- Invalid: yalnız çözülebilir adaylar, tip ve boyut filtreleri.

**Eksikler:**

- **Frustration** yalnız tasarım kuralları kadar temsil ediliyor; FunSim'in hüsran oranı seçime girmiyor.
- **Ağırlıklar elle konmuş**, hiçbir veriyle ayarlanmamış.
- **Seçim açgözlü:** slotlar sırayla doldurulur; bütün setin toplam maliyeti en aza indirilmez.

## Ayrıntılı harita

| Vizyon parçası | Bugün | Nerede | Nasıl çalışıyor | Kanıt | Eksik |
|---|---|---|---|---|---|
| **A. Level Generator** | **var** | `Assets/_Game/Generation/Generator.cs`, `Search.cs`, `SlotSpec.cs`, `Factory*.cs`; `Tools/sl100/Design.cs` | Slot tarifine göre tahta üretir (soyma sırası düğmeleri: Interlock, EdgeFirst, NailLate). Arama: rastgele düğmeler, sonra tepe tırmanma; hedef bantlara uzaklığı en aza indirir. | sl100'ün 100 bölümü bu havuzdan (`PM82_100_BOLUM.md`); fabrika raporu `docs/fabrika/RAPOR.md` | Hedefi doğal dille almaz; "son hamlede kazanma" gibi deneyim hedefi yok. |
| **B. Player Behavior Model** | **kısmen** | `Core/Solve/SmartPlayer.cs`; `Generation/FunSim.cs` (1000 sanal oyuncu); `Tools/veri/bot_fit.py` (PM_25 oyuncu botları); ZeydBot | FunSim: hız, beceri, bomba dikkati, sabır, yeniden deneme dağılımları; değerleri **varsayım** (adlı sabitler). bot_fit: oyuncu başına koşullu logit + kısmi havuzlama (empirik Bayes). | PM_25 geri testi: botlar (v1) tabanları geçemedi (Brier 0,208; fitil tabanı 0,095; `model_runs` v1). Model v2'de insan, %85 "bombaya bakan" + %15 rastgele karışımı. | Beginner/Average/Expert profilleri yok; tek karışım. FunSim sabitleri gerçek veriye oturtulmadı. |
| **C. Monte Carlo** | **var** | `Editing/MonteCarlo.cs` (200 koşu), `StructureMetrics.cs` (kesin yol ölçüleri), `Generation/Curve.cs`, `FunSim.cs`, `DifficultyV2.cs` (`FastState`) | Tohumlu, tekrarlanabilir (seed = ilk tohum + koşu no). | `DifficultyV2Tests`, `CoreIsolationTests`; FunSim L40 × 1000 × 3 = 1,3 sn | — |
| · Win rate / first-try / 3 denemede | var | MonteCarlo.WinRate, FunSim.FirstWinRate / WinBy3Rate | | | |
| · Ortalama hamle | var | MeanMoves | | | |
| · Failure rate | var | BombRate, FrozenRate, FunSim kayıp nedenleri | | | |
| · Remaining moves | uygulanmaz | (oyunda hamle sınırı yok) | Karşılığı kalan süre: FunSim'de "sürenin %60'ından fazlası kaldı = sıkıcı". | | |
| · Solution path diversity | **kısmen** | MeanBranching, FreedomMean, FewestChoices, Bottlenecks10 | Dallanma ve darboğaz ölçülüyor; farklı çözüm yolu sayısı değil. | | Yol sayımı ya da yol entropisi yok. |
| · Near-miss | var (simde) | FunSim.NearMissRate | Son %10 sürede kazanma ya da ≤ 3 parça kala kayıp. | **Gerçek pt1 (bu denetim):** kazançların yalnız **%4,6**'sı son %10 sürede; kayıpların **%2,8**'i ≤ 3 parça kala. | Simdeki oran gerçekle karşılaştırılmadı. |
| · Last-move win | **yok** (ayrı metrik) | — | Near-miss içinde. | Gerçekte nadir (yukarıda). | Ayrı metrik ve hedef yok. |
| · Dead-end | var | FrozenRate (hamle kalmayan durum) | | | |
| **D. AI Level Evaluator** | **kısmen** | `DifficultyModelV2.cs` (p_win1, takılma/çıkış), `DifficultyModelV2.Rules` (tasarım kuralları), `FactoryReport.cs`, `Tools/ajan/` (bölüm ajanı + eleştirmen döngüsü, PM_47/48) | Sayı + neden: kural ihlalleri adlı ("boğaz bomba", "sn/parça"). Ajan döngüsü kart → tarif → ölçüm → kural eleştirisi (en çok 3 tur). | Model v2 LOO MAE 10,7 (belge) / 12,3 (bu denetim). Düşündürme modeli R² 0,71. | Açıklamalar kural tabanlı; gerçek oyuncuya karşı sınanmış bir "neden" modeli yok. |
| **E. Optimizer** | **kısmen** | `Search.cs` (tepe tırmanma), sl100 süre ayarı, `KIT_NOTLARI` ayar sırası, `Tools/ajan/dongu.py` | Düğmeler ve süre üzerinden yerel arama; bant uzaklığı + 10 × haksız neden. | sl100 tahminleri bantlarda (`pred_sl100.csv`). | Ortak bir kayıp fonksiyonu yok; bayesçi ya da evrimsel arama yok; set düzeyinde eniyileme yok. |
| Matematiksel optimizasyon (§5) | kısmen | yukarıdaki | Ayrık parametrelerde yerel arama; süre tek boyutlu tarama. | | Vekil (surrogate) model ile arama yok. |
| Loss / reward (§6) | kısmen | sl100 maliyeti, `Search.BandDistance`, FunSim eşikleri (`docs/fabrika/KARARLAR.md`) | Üç ayrı, elle ağırlıklandırılmış puan. | — | Tek, sürümlenmiş, veriyle ayarlanmış bir Loss yok. |
| Human feedback / reward model (§7) | kısmen | `plays.zorluk_puani` (48), `keyif` (1), ZeydBot puan modeli | ZeydBot 1–10 puanını kayıp şansı ve süreyle doğrusal tahmin ediyor. | Spearman 0,73 (bomba payı tabanı 0,79). | Etiketli yorum veri seti yok; 👍/👎 kapalı. |
| Overfitting önleme (§8) | kısmen | LOO bütün modellerde; ZeydBot'ta Z21–Z30 hiç okunmadı (gerçek test); `excluded_players` | | `model_runs` notu: "özellik seçimi aynı veride yapıldı, hafif iyimser". | Ayrı test kohortu yok; bölüm aileleri kat (fold) dışına ayrılmıyor (S47 v2/v3 aynı aile). |
| Dropout / regularization (§9) | uygulanabilir olduğu kadar | Ridge (model v2: R = 0,3·I; ZeydBot l2 = 0,5; bot_fit empirik Bayes) | Sinir ağı yok, dropout yok (gereksiz). | | — |
| Gerçek veriyle kalibrasyon (§10) | **var** (pt1) | `docs/analiz/v2/irt.py` (Rasch / IRT), `PlayData.cs`, `DifficultyModelV2.PWin1Blended` | Oyuncu becerisi ve bölüm zorluğu ayrı; oynanmış bölümde gerçek veri ile model harmanlanır. | 51 oyuncu, 1259 deneme; beceri sapması 0,57; öğrenme +0,67 · log(deneme). | Yalnız tek kohort; belirsizlik aralıkları raporda yok. |
| Difficulty ≠ Fun (§11) | kısmen | İki eksen: p_win1 (kaybettirme) ve takılma/çıkış (düşündürme); FunSim keyif / sıkıcı; `Curve.cs` (bölüm içi eğri) | | Düşündürme modeli R² 0,71. | Keyif tahmini gerçek oyuncuyla hiç doğrulanmadı (👍/👎 1 satır). |
| Progression | var | sl100 rolleri (4 zor, 9 çok zor, x0 nefes), `KIT_NOTLARI` | Elle konmuş ritim + model bantları. | `pm82_egri.png` | Ritmin tutunmaya etkisi ölçülmedi. |
| Taşınabilirlik (§12) | **kısmen** | Assembly ayrımı: Core (kurallar) ← Editing / Generation (ölçü, üretici) ← araçlar | Kurallar Core'da saf C#. | `CoreIsolationTests` | Simulator / Evaluator / Optimizer arayüzleri Reverse Solver'a özgü; ortak bir "oyun" arayüzü yok. |
| Tekrarlanabilirlik (Phase 2) | kısmen | Tohumlu sim; `model_runs` (3 koşu: v0, v0.1, v1), `level_predictions` (120 satır), `level_features` (42) | | | Model v2 ve sl100 koşuları `model_runs`'a yazılmadı; v2'nin ara dosyası repoda değil. |

## Gerçek oyuncu verisinden üç ek bulgu (bu denetimde ölçüldü)

1. **Son hamlede kazanma deneyimi gerçekte nadir.** pt1'deki 698 kazancın %4,6'sı sürenin son %10'unda geldi; 536 kaybın yalnız %2,8'i ≤ 3 parça kala oldu. Kayıplar çoğunlukla erken bir "boğaz" bombada (`ZORLUK_MODELI_v2.md` bulgu 6). Vizyonun sevdiği deneyim bugün tasarımda hedeflenmiyor ve oyuncu verisinde neredeyse yok.
2. **Model v2'nin hatası bombalı bölümlerde.** Bombasız bölümlerde 4 puan, bombalılarda 14 puan. Kolay bombalı bölümleri fazla zor sanıyor.
3. **İnsan geri bildirimi tek kişiden.** Sayısal 48 puanın hepsi Zeyd'in. Gerçek oyunculardan keyif verisi yok.
