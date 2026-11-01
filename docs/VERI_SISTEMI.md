# Veri Sistemi: tasarla → tahmin et → oynat → karşılaştır → öğren

Hazırlayan: PM, 31 Ekim 2026. Durum: **Faz 0 bitti** (şema, v0 tahmin, hedefler). Faz 1–2 Claude Code'da (`docs/PM_MESAJ_25.md`).

---

## 0. Sade özet (Zeyd için)

Her bölüm için üç soruyu aynı tabloda yan yana görmek istiyoruz:

1. **Ne ayarladık?** Bölümün sayıları: boyut, zincir, bomba, fitil, çivi, duvar, süre.
2. **Ne bekliyoruz?** İki şey var:
   - **Hedef:** tasarım niyetimiz. Örnek: "33. bölümde oyuncuların %52'si ilk denemede geçsin."
   - **Tahmin:** modelin hesabı. Örnek: "%43 geçer, kaybedenlerin çoğu bombadan kaybeder."
3. **Veri ne diyor?** 20 kişi oynadıktan sonra gerçek oranlar.

Bu üçü karşılaştırılınca her bölüme otomatik bir hüküm düşer: "tahmin tuttu", "beklenenden ZOR" ya da "hedeften KOLAY" gibi. Tahmin tutmadığında modeli düzeltiriz. Bölüm hedefi tutmadığında bölümü düzeltiriz.

Bir sonraki bölüm hazırlanırken model, oyuncular oynamadan önce sonucu söyler.

Tahminler oyun testinden **önce dondurulur**. Veritabanı, dondurulmuş bir tahminin sonradan değiştirilmesine izin vermez. Böylece "aslında böyle olacağını biliyorduk" diyemeyiz, model dürüstçe sınanır.

## 1. Döngü

```
 Tasarla (bölüm JSON)
   → Özellik çıkar        level_features      (Tools/veri/level_features.py + FunSim metrikleri)
   → Hedef koy            level_targets       (PM, SET40 tablosundan)
   → Tahmin et            level_predictions   (bot nüfusu + Monte Carlo; şimdilik v0 regresyon)
   → Dondur               model_runs.frozen_at + git commit
   → Oynat (kohort pt1)   plays + moves + player_profiles
   → Karşılaştır          v_pred_vs_actual, v_model_score, v_funnel
   → Öğren                botları yeniden eğit → yeni koşu (v2) → bir sonraki set
```

Her tur bir **kohort** (oyuncu grubu): `pt1` = ilk 20–30 kişilik test, `ic` = ekip içi deneme.

## 2. Ölçüler (tanımlar sabit, değişmez)

| Ölçü | Tanım | Neden |
|---|---|---|
| **win1** | Oyuncunun o bölümdeki 1. denemesinin kazanma oranı | Zorluğun ana ölçüsü |
| fail_bomb1 / fail_time1 | 1. denemenin bomba / süre ile kaybedilme oranı | Zorluğun *türü*: tuzak mı, hız mı |
| attempts_to_win | Kazanana kadar ortalama deneme | Kayıp sonrası öğrenme |
| left_frac | Kazanınca kalan süre / toplam süre | "Son saniyede geçtim" gerilimi |
| jams | Yanlış (sıkışık) parçaya dokunma | Yanıltıcılık |
| rating | Oyuncunun 1–10 zorluk puanı | Hissedilen zorluk |
| keyif | 👍/👎 | Zorluk iyi mi kötü mü |
| funnel | O bölüme ulaşan oyuncu sayısı | Bırakma = en pahalı başarısızlık |

Booster kullanılan denemeler ayrıca işaretlenir (`used_booster`). Ana zorluk ölçüsü booster'sız ilk denemedir (PLAYTEST_TASLAK karar 6).

## 3. Puan ↔ oran eşlemesi (varsayım, veriyle düzeltilecek)

Elimizdeki puanlar Zeyd'in. Zeyd tasarımcı ve uzman bir oyuncu, yeni bir oyuncu daha çok zorlanır. Bu yüzden puanı nüfus oranına çeviren bir **başlangıç varsayımı** koyduk:

**A1:** `win1 = 0,97 − 0,075 × (puan − 1)`

| Puan | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10 |
|---|---|---|---|---|---|---|---|---|---|---|
| İlk denemede geçen | %97 | %90 | %82 | %75 | %67 | %60 | %52 | %45 | %37 | %30 |

**A2:** Bombalı bölümde kayıpların %75'i bombadan, bombasız bölümde %100'ü süreden.

Gerekçe: Eski web testinde 8 oyuncu vardı. Bombalı bölümlerde kayıpların hepsi bombadan, bombasızlarda süreden geldi. A1 tipik casual bulmaca eğrisine yakın. pt1 verisi gelince A1 ve A2 ilk düzeltilecek şeyler olacak (bkz. 7.3).

## 4. Hangi veri tutuluyor

### Şu an tutulanlar
- `plays`: denemenin sonucu, süre, kalan saniye, hamle, sıkışma, yıldız, booster, zorluk puanı, 👍/👎.
- `moves`: **her hamle ayrı satır**. Satırda şunlar var:
  - Hangi parça, hangi yöne.
  - Bölüm başından beri geçen süre (ms) ve bir önceki hamleden beri düşünme süresi (ms).
  - Kalan süre.
  - O anki tüm fitiller (`"8:7;10:30"` biçiminde).
  - Dokunma kayıtlarında basılı tutma süresi.

Yani "ilk hangi parçayı çekmiş, sonra hangisini, hangi saniyede" sorusunun cevabı zaten kaydediliyor (bugün 6.233 hamle satırı var).

### Eksikler (Claude Code, Faz 1)

| Eksik | Neden önemli | Çözüm |
|---|---|---|
| Kohort etiketi | Test oyuncularını ekip denemelerinden ayırmak için | Linke `?c=pt1` eklenir, tüm satırlara `cohort` yazılır (sütunlar hazır) |
| Cihaz kimliği | Aynı ismi kullanan iki kişiyi ayırmak, kişiye özel bot kurmak için | `device_id` (anonim uuid, cihazda saklanır) |
| Platform | Telefonda dokunmak ile fareyle oynamak arasında hız farkı var | `platform` |
| Yarıda bırakma | Hamleler bölüm sonunda toplu gönderiliyor. Sekme kapanırsa o denemenin hiçbir kaydı kalmıyor | Bölüm açılınca `start` satırı gelir. Ayrıca her 10 hamlede bir ve sekme gizlenince (`hide`) gönderim yapılır. `(row_id, move_no)` benzersiz olduğu için tekrar gönderim zarar vermez |
| Sıkışmada denenen yön | Botu klonlarken oyuncunun hangi yöne denediği gerekiyor | `jam` satırına `dir` yazılır |
| 1–10 zorluk sorusu | Şu an yalnız ZeydBot setinde soruluyor | pt1'de her bölümden sonra tek dokunuşla sorulur, "geç" seçeneği olur. Kayıptan sonra da sorulur (RAPOR.md "Önemli 1") |
| Oyuncu profili | Botları oyuncu tipine göre gruplamak için | Oyunun başında tek soru: "Bu tür bulmaca oyunları oynar mısın? Hiç / Ara sıra / Sık" (`player_profiles`) |

İsim dışında kişisel veri tutulmuyor. Gizlilik sayfası buna göre güncel kalmalı.

## 5. Supabase katmanı (kuruldu: `docs/sql/009_veri_sistemi.sql`)

| Nesne | Ne | Kim yazar |
|---|---|---|
| `plays`, `moves`, `player_profiles` | Oyuncu verisi | Oyun (anon) |
| `level_features` | Bölüm başına sayılar + `sim` (FunSim metrikleri, JSON) | PM (Claude Code CSV üretir) |
| `level_targets` | Hedef puan bandı → hedef win1 bandı, beklenen kayıp tipi, bölümün rolü | PM |
| `model_runs` | Her tahmin koşusu: model, varsayımlar, geri test sonucu, git commit, `frozen_at` | PM |
| `level_predictions` | Koşu × bölüm: p_win1 (+ aralık), kayıp tipi, süre, sıkışma, puan | PM |
| `player_bots` | Oyuncu başına bot parametreleri + nüfus dağılımı | PM (Claude Code JSON üretir) |
| `v_attempts` | Temiz deneme satırları (test isimleri hariç) | görünüm |
| `v_level_actual` | Kohort × bölüm: win1 + Wilson %95 aralığı, kayıp tipleri, puan, keyif | görünüm |
| `v_funnel` | Hangi bölüme kaç kişi ulaştı | görünüm |
| `v_abandoned` | Başlayıp bitmeyen denemeler | görünüm |
| **`v_pred_vs_actual`** | **Ne ayarladık / ne bekledik / veri ne dedi** + hüküm | görünüm |
| `v_model_score` | Koşu başına karne: Brier, MAE, sıra korelasyonu, sabit tabana karşı | görünüm |

Model tablolarına web'den (anon) yazılamaz, yalnız okunabilir. Web build'indeki anahtar herkese açık, tahminlerle kimse oynayamamalı. Dondurulmuş bir koşunun tahminini veritabanı tetikleyicisi reddeder.

## 6. Modeller (basitten karmaşığa; karmaşık olan basiti geçemezse kullanılmaz)

> Eski ZeydBot dersi (RAPOR.md): ZeydBot v1, tek sayılık "bomba payı" tabanını **geçemedi**. Bu yüzden her yeni modelin önce sabit tabanları yenmesi gerekir. Az veriyle karmaşık model kolayca kendini kandırır.

### M0: Tabanlar
- Herkese ortalama win1.
- Yalnız fitil payından tahmin.

### M1: Özellik regresyonu (v0, bugün donduruldu)
Zeyd'in 40 bölüme verdiği puanlarla eğitildi. Ayarlanan 9 bölümde eski sürümün sayıları kullanıldı.

`puan ≈ 2,39 + 0,22·bomba − 0,13·fitil_payı + 0,24·duvar_şeridi + 0,56·zincir + 0,26·çivi`

Her ek öğenin puana yaklaşık etkisi:

| Ek öğe | Puan etkisi |
|---|---|
| 1 zincir | +0,56 |
| 1 çivi | +0,26 |
| 1 duvar şeridi | +0,24 |
| 1 bomba | +0,22 |
| 1 hamle fitil payı | −0,13 |

Bu regresyon ile FunSim kalibrasyonunun (`puan ~ logit(bot ilk deneme) + logit(rastgele)`) ortalaması alınır.

| Model (birini dışarıda bırak, n = 40) | Ortalama hata (puan) | Sıra korelasyonu |
|---|---|---|
| Sabit ortalama | 2,86 | — |
| Yalnız regresyon | ~1,3 | ~0,78 |
| Yalnız FunSim kalibrasyonu (n = 37) | 1,41 | 0,82 |
| **v0 = ikisinin ortalaması** | **1,25** | **0,85** |

Not: Özellikleri aynı veride seçtik, bu yüzden bu sayılar biraz iyimser.

**v0'ın bildiğimiz zayıflıkları:**
- **Tuzakları görmüyor.** S27'yi 4,8 tahmin ediyor, Zeyd 9 verdi (8. hamlede bombayla kaybetti). Sayılar aynı kalsa bile parçaların dizilişi tuzak yaratabilir. Bunu ancak hamle simülasyonu görür → M2.
- **Uçları ortaya çekiyor.** 10'luk bölümler 8 civarı, 1'lik bölümler 1 civarı çıkıyor (regresyonun doğası). Bu yüzden 38–40'ın "hedeften kolay" görünmesi kısmen modelden.
- **Z bölümleri.** Model Z23, Z24 ve Z25'i Zeyd'in puanından 1,5–2,5 puan zor buluyor. Zeyd bunları daha önce oynamıştı (SET40_V4 notu). Gerçek oyuncu verisi hangisinin haklı olduğunu gösterecek.
- **Tek oyuncu.** Model yalnızca Zeyd'i biliyor. Nüfusa geçiş A1 varsayımına dayanıyor.

### M2: Oyuncu botları (Faz 2, Claude Code)
Her oyuncu için hamle hamle bir "klon" kurulur. Kayıtlı her deneme, Unity'deki yeniden oynatıcıda tahtanın o anki haliyle oynatılır. Her hamlede şu sorulur: "Bu durumda olası hamleler arasından neden bunu seçti, ne kadar düşündü?"

| Parametre | Ne | Nasıl ölçülür |
|---|---|---|
| `softmax_beta` | Doğru (kazanmaya götüren) hamleye yönelme gücü. 0 = rastgele, yüksek = usta | Seçilen hamle ile diğer adaylar karşılaştırılır (koşullu logit) |
| `bomb_priority` | Fitil kısaldıkça bombaya yönelme | Aday hamlenin fitil payına etkisi |
| `locality` | Son dokunduğu yere yakın parçayı seçme eğilimi | Aday ile son parça arası mesafe |
| `jam_rate` | Kilitli parçaya dokunma oranı | `jam` satırları / dokunuşlar |
| `think_mu`, `think_sigma` | Düşünme süresi (log-normal). Seçenek sayısı ve fitil baskısı ile değişir | `think_ms` |
| `learn` | Aynı bölümde her yeni denemede iyileşme | 1. ve 2. deneme karşılaştırması |
| `platform` | Fare / dokunma hız farkı | `platform` |

**Az veriyle kişiye özel bot nasıl olur:** 20 kişinin her biri 40 bölüm oynasa bile tek kişinin verisi gürültülüdür. Bu yüzden **kısmi havuzlama** (hiyerarşik model) kullanılır. Her oyuncunun parametresi, nüfus ortalamasından başlar ve kendi verisi kadar oradan uzaklaşır. Az oynayan oyuncu ortalamaya yakın kalır, çok oynayan kendine benzer.

### M3: Nüfus Monte Carlo
Botlar yalnızca 20 kişiyle sınırlı kalmaz. Parametrelerin nüfus dağılımından (ortalama + yayılım + aralarındaki ilişki) **1000 sanal oyuncu** çekilir, yani nüfus "büyütülür". Her yeni bölüm bu 1000 sanal oyuncuya oynatılır. Bölüm başına çıktılar:
- p_win1 ve %90 aralığı
- p_fail_bomb ve p_fail_time
- kazanana kadar deneme sayısı
- süre ve kalan süre
- sıkışma sayısı
- tahmini 1–10 puan

İstersek nüfusu hedef kitleye göre ağırlıklandırırız (ör. %70 telefon, %50 "ara sıra" oyuncu).

### M4: Kalibrasyon
Sanal oyuncuların oranı ile gerçek oran arasında sistematik bir fark kalırsa (ör. botlar hep %10 iyimser), son bir düzeltme eğrisi bu farkı kapatır. Puan tahmini de simülasyon çıktılarından gerçek 1–10 puanlara regresyonla öğrenilir.

## 7. Doğrulama: ön-kayıt ve karne

### 7.1 Ön-kayıt
- pt1 davetleri gitmeden önce koşu dondurulur (`frozen_at` + git commit).
- Dondurulan koşu: v0 (bugün) + v1 (botlar hazırsa, davetten önce).
- İki modeli aynı veriyle sınarız. Karmaşık olan basiti geçemezse basit kalır.

### 7.2 Başarı ölçütleri (önceden yazıldı)
- Bölüm sırası: Spearman ρ ≥ 0,70. Değer, PLAYTEST_TASLAK karar 6'dan alındı.
- Brier skoru sabit ortalamadan iyi olmalı (`v_model_score.brier < brier_baseline_mean`).
- n ≥ 8 olan bölümlerde win1 ortalama hatası ≤ 0,15.
- Hüküm yalnız n ≥ 8 olan bölümde verilir. Daha az veri olan bölüm "veri az" sayılır.

### 7.3 Veri gelince sıra
1. A1 eşlemesi gerçek win1 ile yeniden kurulur.
2. Botlar pt1 hamleleriyle eğitilir. Biri dışarıda bırakılarak test edilir: oyuncu ve bölüm.
3. Yeni koşu (v2) açılır.
4. "Beklenenden ZOR/KOLAY" çıkan bölümler için neden analizi yapılır: hangi özellik ya da tuzak kaçırıldı?
5. Gerekirse yeni özellik eklenir (ör. tuzak sayısı, açılış hamlesi k₀).

## 8. 20 kişinin istatistik gerçeği (önemli)

- **Belirsizlik geniş.** 20 kişiyle bir bölümde ölçülen %60, gerçekte %39–78 arası olabilir (Wilson %95). Tek bölüm hükmü kaba olur. Eğrinin şekli ve büyük ıskalar güvenilir, tek bölümde %5'lik fark güvenilir değil.
- **Bırakma.** Eski web testinde 8 kişi başladı, 25. bölümden sonra 1–2 kişi kaldı. 20 kişi başlarsa 40. bölüme 6–10 kişi ulaşabilir.
  - Önlem 1: 25–30 kişi davet et.
  - Önlem 2: Davette "40 bölümü bitirmeni rica ediyoruz, ~45–60 dk, 2 oturuma bölebilirsin" yaz.
  - Önlem 3: Bırakma eğrisini (`v_funnel`) her gün izle.
- **Botlar bu yüzden var.** Bot hamle düzeyinde öğrenir, yani bir oyuncunun 3. bölümdeki davranışı 33. bölümün tahminini de besler. Bölüm başına 20 oyun gerekmez.
- **Sıra etkisi.** Herkes 1'den 40'a sırayla oynuyor. Oyuncu ilerledikçe öğrenir, sona doğru bölümler biraz kolay görünür. Botta "o ana kadar oynanan bölüm sayısı" değişkeni bunu ayırır.
- **Arkadaş yanlılığı.** Arkadaşlar sabırlı ve iyi niyetli olur, keyif puanı şişebilir. Zorluk ölçüleri (kazanma, kayıp tipi) bundan daha az etkilenir.

## 9. Fazlar

| Faz | Kim | Ne | Bitti sayılır |
|---|---|---|---|
| **0** | PM | Şema (009), özellik tablosu, hedefler, v0 tahmin ve dondurma | ✅ 31 Ekim |
| **1** | Claude Code | Kayıt eksikleri (bölüm 4), `?c=pt1` linki, profil sorusu, her bölümde puan sorusu, `ic` kohortuyla deneme | Zeyd'in `ic` oyunu Supabase'de tüm yeni alanlarla görünür, `v_pred_vs_actual` dolu |
| **2** | Claude Code | Yeniden oynatıcı → hamle tablosu, bot eğitimi (M2), nüfus Monte Carlo (M3), v1 tahmini + geri test | v1, eski verideki tabanları geçiyor ya da geçemediği yazılı. Davetten önce donduruldu |
| **3** | Zeyd | 25–30 kişiyi davet et (pt1 linki), 1 hafta | 40. bölüme ≥ 8 kişi ya da süre doldu |
| **4** | PM + Claude Code | Karşılaştırma raporu, A1/A2 düzeltmesi, botların yeniden eğitimi (v2), SET40 v5 önerisi | Rapor + v2 koşusu + değişecek bölümler listesi |
| **5+** | herkes | Her yeni bölüm, çıkmadan önce v2 ile tahmin edilir ve hedef bandına girmeden yayınlanmaz | — |

## 10. Bugünkü tablo (SET40 v4, v0 tahmini, dondurulmuş)

"Hedef" sütunu SET40_V4.md'deki puan bandının A1 ile çevrilmiş hâli. "Durum" tahmin ile hedef arasında 1 puandan büyük fark olup olmadığını gösterir. Bu henüz veri değil, hipotez.

| # | Bölüm | Ne ayarladık | Hedef puan → ilk deneme kazanma | v0 tahmin puan → ilk deneme kazanma (aralık) | Kayıp tipi | Durum |
|---|---|---|---|---|---|---|
| 1 | S41 | 3×3, 120 sn | 1 → %97 | 1.0 → %97 (%80–97) | süre | hedefte |
| 2 | S42 | 4×4, 120 sn | 1 → %97 | 1.0 → %97 (%80–97) | süre | hedefte |
| 3 | S43 | 4×5, 1 çivi, 60 sn | 1–2 → %90–97 | 1.1 → %96 (%80–97) | süre | hedefte |
| 4 | S06 | 5×6, 1 çivi, 55 sn | 2 → %90 | 1.6 → %93 (%76–97) | süre | hedefte |
| 5 | S44 | 5×5, 2 çivi, 60 sn | 2 → %90 | 1.2 → %96 (%79–97) | süre | hedefte |
| 6 | S45 | 4×5, 1 bomba (fitil 23, pay 10), 60 sn | 2 → %90 | 1.1 → %96 (%79–97) | bomba | hedefte |
| 7 | S46 | 5×5, 1 bomba (fitil 33, pay 8), 60 sn | 2–3 → %82–90 | 1.3 → %95 (%78–97) | bomba | hedefte |
| 8 | S47 (v2) | 5×6, 2 bomba (fitil 20, pay 12), 60 sn | 3 → %82 | 3.0 → %82 (%66–97) | bomba | hedefte |
| 9 | S20 (v2) | 5×6, 1 bomba (fitil 7, pay 5), 65 sn | 3 → %82 | 3.1 → %81 (%65–97) | bomba | hedefte |
| 10 | Z24 | 6×7, 3 zincir, 1 bomba (fitil 22, pay 2), 2 çivi, 65 sn | 3 → %82 | 4.7 → %69 (%53–86) | bomba | hedeften zor olabilir |
| 11 | S21 | 6×7, 2 zincir, 1 bomba (fitil 16, pay 2), 2 çivi, 60 sn | 3–4 → %74–82 | 4.6 → %70 (%53–86) | bomba | hedefte |
| 12 | S51 (v2) | 5×6, 1 zincir, 2 bomba (fitil 10, pay 5), 60 sn | 3–4 → %74–82 | 4.0 → %74 (%58–91) | bomba | hedefte |
| 13 | S34 | 6×7, 3 zincir, 1 bomba (fitil 21, pay 2), 2 çivi, 65 sn | 4 → %74 | 5.1 → %66 (%50–83) | bomba | hedeften zor olabilir |
| 14 | Z21 | 6×7, 3 zincir, 1 bomba (fitil 20, pay 2), 2 çivi, 1 duvar, 65 sn | 3–4 → %74–82 | 5.6 → %63 (%46–79) | bomba | hedeften zor olabilir |
| 15 | S13 | 6×6, 2 zincir, 1 çivi, 1 duvar, 50 sn | 4 → %74 | 2.8 → %84 (%67–97) | süre | hedeften kolay olabilir |
| 16 | Z04 | 6×7, 3 zincir, 1 bomba (fitil 22, pay 2), 2 çivi, 1 duvar, 60 sn | 4 → %74 | 4.8 → %68 (%52–85) | bomba | hedefte |
| 17 | S18 (v2) | 6×6, 3 zincir, 1 bomba (fitil 17, pay 2), 1 çivi, 2 duvar, 50 sn | 4–5 → %67–74 | 5.3 → %65 (%48–81) | bomba | hedefte |
| 18 | S22 | 6×7, 3 zincir, 1 bomba (fitil 21, pay 2), 2 çivi, 2 duvar, 70 sn | 5 → %67 | 5.5 → %63 (%47–80) | bomba | hedefte |
| 19 | S28 | 7×8, 2 zincir, 4 bomba (fitil 8, pay 0), 3 çivi, 13 duvar, 130 sn | 7–9 → %37–52 | 8.3 → %42 (%30–59) | bomba | hedefte |
| 20 | S24 | 6×7, 3 zincir, 1 bomba (fitil 21, pay 1), 2 çivi, 55 sn | 6 → %60 | 6.5 → %56 (%39–72) | bomba | hedefte |
| 21 | Z23 | 7×8, 2 zincir, 4 bomba (fitil 9, pay 1), 4 çivi, 13 duvar, 150 sn | 5 → %67 | 7.6 → %48 (%31–64) | bomba | hedeften zor olabilir |
| 22 | S19 | 6×6, 3 zincir, 2 çivi, 2 duvar, 50 sn | 4–5 → %67–74 | 3.4 → %79 (%63–96) | süre | hedefte |
| 23 | S27 | 6×7, 3 zincir, 2 bomba (fitil 6, pay 4), 1 çivi, 1 duvar, 60 sn | 7–8 → %44–52 | 4.8 → %68 (%52–85) | bomba | hedeften kolay olabilir |
| 24 | S14 | 6×6, 1 zincir, 2 çivi, 2 duvar, 50 sn | 4–5 → %67–74 | 2.8 → %84 (%67–97) | süre | hedeften kolay olabilir |
| 25 | S35 | 7×8, 2 zincir, 4 bomba (fitil 29, pay 0), 14 duvar, 105 sn | 8 → %44 | 8.1 → %44 (%30–61) | bomba | hedefte |
| 26 | S17 (v2) | 6×6, 3 zincir, 1 bomba (fitil 19, pay 2), 1 çivi, 1 duvar, 55 sn | 3–4 → %74–82 | 4.7 → %69 (%52–86) | bomba | hedefte |
| 27 | S10 (v2) | 6×6, 1 bomba (fitil 17, pay 10), 1 çivi, 1 duvar, 55 sn | 3–4 → %74–82 | 3.4 → %79 (%63–96) | bomba | hedefte |
| 28 | S50 (v2) | 5×6, 2 zincir, 1 bomba (fitil 18, pay 1), 60 sn | 3 → %82 | 4.2 → %73 (%57–90) | bomba | hedeften zor olabilir |
| 29 | S52 (v2) | 5×5, 1 bomba (fitil 14, pay 9), 1 duvar, 60 sn | 3 → %82 | 3.1 → %81 (%64–97) | bomba | hedefte |
| 30 | S37 | 7×8, 2 zincir, 4 bomba (fitil 20, pay 0), 4 çivi, 13 duvar, 135 sn | 8–9 → %37–44 | 8.4 → %41 (%30–58) | bomba | hedefte |
| 31 | S08 (v2) | 6×6, 1 bomba (fitil 18, pay 11), 2 çivi, 2 duvar, 70 sn | 3–4 → %74–82 | 3.5 → %78 (%62–95) | bomba | hedefte |
| 32 | S31 | 7×8, 2 zincir, 4 bomba (fitil 28, pay 0), 2 çivi, 14 duvar, 170 sn | 7–8 → %44–52 | 6.7 → %54 (%38–70) | bomba | hedefte |
| 33 | S39 | 7×8, 2 zincir, 4 bomba (fitil 22, pay 0), 2 çivi, 13 duvar, 115 sn | 7 → %52 | 8.2 → %43 (%30–60) | bomba | hedeften zor olabilir |
| 34 | S32 | 7×8, 2 zincir, 4 bomba (fitil 17, pay 0), 1 çivi, 14 duvar, 110 sn | 7–8 → %44–52 | 8.2 → %43 (%30–60) | bomba | hedefte |
| 35 | S36 | 7×7, 2 zincir, 3 bomba (fitil 15, pay 1), 3 çivi, 12 duvar, 80 sn | 8 → %44 | 7.2 → %50 (%34–67) | bomba | hedefte |
| 36 | S29 | 8×8, 2 zincir, 4 bomba (fitil 7, pay 1), 4 çivi, 14 duvar, 110 sn | 8–9 → %37–44 | 8.1 → %44 (%30–60) | bomba | hedefte |
| 37 | Z25 | 7×8, 2 zincir, 4 bomba (fitil 28, pay 1), 1 çivi, 13 duvar, 145 sn | 4–5 → %67–74 | 6.6 → %55 (%39–72) | bomba | hedeften zor olabilir |
| 38 | S38 | 6×7, 2 zincir, 3 bomba (fitil 8, pay 0), 3 çivi, 12 duvar, 70 sn | 9 → %37 | 7.9 → %45 (%30–62) | bomba | hedeften kolay olabilir |
| 39 | S40 | 7×7, 2 zincir, 3 bomba (fitil 16, pay 1), 3 çivi, 13 duvar, 80 sn | 8–9 → %37–44 | 8.1 → %44 (%30–60) | bomba | hedefte |
| 40 | S33 | 8×8, 2 zincir, 4 bomba (fitil 8, pay 1), 4 çivi, 15 duvar, 105 sn | 10 → %30 | 8.2 → %43 (%30–60) | bomba | hedeften kolay olabilir |

**Bu tablodan çıkan sorular (pt1 cevaplayacak):**
- 10, 13, 14, 21 ve 37 (Z bölümleri ve S34): Model bunları hedeften zor buluyor, Zeyd'in puanı kolay diyor. Gerçek oyuncu hangisini doğrulayacak?
- 15 ve 24 (S13, S14; bombasız, 50 sn): Model kolay buluyor. Zorluğu süre baskısından geliyor, bu da regresyonun zayıf olduğu yer.
- 23 (S27): Model kolay diyor, ama içinde tuzak var. Bot (M2) burada farkı göstermeli.
- 38 ve 40 (S38, S33): "Kolay" görünmeleri kısmen modelin uçları ortaya çekmesinden.

## 11. Dosyalar

| Dosya | Ne |
|---|---|
| `docs/sql/009_veri_sistemi.sql` | Supabase şeması (uygulandı) |
| `Tools/veri/level_features.py` | Bölüm JSON → özellik CSV |
| `Tools/veri/v0_tahmin.py` | v0 modeli + birini dışarıda bırak karnesi → `docs/veri/tahmin_v0.csv` |
| `docs/veri/level_features_v4.csv` | SET40 v4 özellikleri |
| `docs/veri/tahmin_v0.csv` | v0 tahmini (Supabase'deki dondurulmuş koşuyla aynı) |
| `docs/PM_MESAJ_25.md` | Claude Code için Faz 1–2 görevi |
| `Tools/veri/fetch.js` | Supabase'den bütün plays + moves (anon okuma) → `Logs/veri/` |
| `Tools/veri/hazirla.py` | Oyuncu eşleme (Zeyd'in isimleri tek oyuncu; test/IC_KONTROL/SHOW hariç; kohortlu satır device_id ile), bütün bölüm sürümleri (`S47@2`), hamle girdisi |
| `Veri.Replay` (Unity) | Her denemeyi tahtada yeniden oynatır → `docs/veri/secimler.csv` (seçim anı × aday: doğru mu, fitil payı etkisi, serbest bıraktığı, bomba, zincir, son parçaya uzaklık, seçildi mi, süre) |
| `Veri.Sim` (Unity) | `docs/veri/level_sim_v4.csv`: k₀, M2 tavanı, en dar fitil payı, tuzaklar, rastgele kazanma, FunSim ilk deneme |
| `Tools/veri/bot_fit.py` | M2: koşullu logit + log-normal süre + jam oranı, kısmi havuzlama → `docs/veri/botlar_<run>.json` |
| `Veri.Bots` (Unity) | M3: nüfustan sanal oyuncular, her bölüm (+ kaybedene 2. deneme) → `docs/veri/tahmin_<run>.csv` (level_predictions sütunları) |
| `Tools/veri/geri_test.py` | Bölüm/oyuncu dışarıda bırak; v1 ↔ v0 ↔ sabit ↔ fitil tabanı (Brier, MAE, Spearman) → `docs/veri/geri_test_<run>.md` |
| `Tools/veri/dongu.py` | Hepsi tek komutta (aşağıda) |

**Tek komut (pt1 verisi gelince, Unity kapalıyken):**

```
python Tools/veri/dongu.py --cohort pt1 --run v2          # yalnız pt1 oyuncularından öğrenir
python Tools/veri/dongu.py --cohort pt1 --run v2 --hepsi  # pt1 + eski gerçek oyuncular (Zeyd)
```
Çıktı: `botlar_v2.json`, `tahmin_v2.csv`, `geri_test_v2.md`, `rapor_v2.md` (docs/veri). Süre: yeniden oynatma ~5 dk, Monte Carlo (1000 sanal oyuncu) ~30 dk, geri test ~15 dk.

**Bot tanımındaki kısaltmalar (v1):** "doğru hamle" = hamleden sonra tahtadaki her bombanın fitil payı ≥ 0 (pay = fitil − bombayı çıkarmanın en kısa yolu; yol SmartPlayer'ın doğrudan kazma yolu, tam arama değil — tam çözücü aday başına Monte Carlo için çok yavaştı). İki bomba aynı yolu paylaşırsa birlikte denetlenmez. Güçlendirme (makas/değnek) kullanılan denemeler o hamlede durdurulur.

### Hazır sorgular (Supabase SQL editörü)

```sql
-- Ne ayarladık / ne bekledik / veri ne dedi (pt1)
select slot, level_id, target_win1_lo, target_win1_hi, p_win1, n_first, win1, win1_lo, win1_hi, tahmin_sonucu, hedef_sonucu
from v_pred_vs_actual where run_id = 'v0-2026-10-31' and (cohort = 'pt1' or cohort is null) order by slot;

-- Bırakma eğrisi
select slot, players_reached, players_won from v_funnel where cohort = 'pt1' order by slot;

-- Model karnesi
select * from v_model_score where cohort = 'pt1';
```
