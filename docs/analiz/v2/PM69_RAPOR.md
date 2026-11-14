# PM_69 raporu: zorluk özellikleri v2 + pt1 replay (14 Kasım)

Yalnız ölçüm. Bölüm, p22 ve yayın değişmedi.

## 0. Özet
- **Replay uyumu %97,7.** Ölçüm, `plays` satırı olan 1279 pt1 denemesi üzerinden yapıldı (hedef ≥ %95). Booster'sız denemelerde %97,8, makas/değnek kullanılanlarda %95,8 (96 deneme).
- **Bomba ölümleri iki türe ayrılıyor** (420 ölüm, ölüm başına 8,0 yol dışı ve 11,3 yolda çıkış, yol dışı payı %41):
  - **Çeldirici (eksen A):** oyuncu bombanın yolunda olmayan parçaları alıp ölüyor. Yol dışı çıkış yoldakinden çok daha fazla: S47, S21, S34, Z04, S08, S24, S20.
  - **Sıkı fitil / ezber merdiveni:** oyuncu bombanın yolunu kazıyor ama yetişemiyor. Yoldaki çıkış yol dışının 3–10 katı: S28, S35, S38, S33, S39, Z23.
- **Eksen B'nin insan maliyeti ölçüldü.** Kilit seçenekleri 1–2'ye düşürdüğünde oyuncu çıkış başına 2,57 sn harcıyor ve 1,92 kez takılıyor. Engelsiz geniş anlarda bu değerler 1,34 sn ve 0,35. Zincir 2,07 sn / 1,45, duvar 2,04 sn / 1,38.
- **Gerçek pay** oyuncu davranışından ölçülenle aynı çıktı (S50 12, S17 7, S18 5); eski kayıtlı pay 1–2 idi. 250 bölüm için `level_features_v2` ve kontrol tablosu §4'te.
- **Supabase tabloları açılmadı.** Veritabanı değişikliği Zeyd'in "çalıştır" onayına bağlı (kalıcı kural); otomatik izin de bunu reddetti. SQL ve yükleme hazır, bkz. §5. **Zeyd'e sor.**

## 1. Yöntem
Kod: `Assets/_Game/Generation/DifficultyV2.cs` (ölçüm), `Assets/_Game/Editor/Pm69Run.cs` (havuz, replay, CSV). Testler: `Tests/EditMode/DifficultyV2Tests.cs`.

**Engel kümeleri.** Parça çıkarmak hiçbir parçayı bloke etmiyor (GreedySolver'daki gerçek). Bu yüzden her parça ve her yön için yol üstündeki parça kümesi tahtadan bir kez çıkarılabiliyor: yoldaki hücreyi dolduranlar ve yanda tırnakla tutanlar. Küme `TravelRule.Limit` ile tek tek kaldırılarak bulunuyor. Parça o yönden ancak bu küme tamamen gittiğinde ve şerit duvarlı değilse çıkabiliyor.

`FastState` bu kümeler üstünde oynuyor. Sıra GameSession'la aynı: parça çıkar, kilit sayaçları azalır, supap, bomba sayaçları azalır. Test (`FastStateMatchesTheSession`), 40 bölümde rastgele oyunlarda her adımda açık parça listesinin ve patlama anının GameSession'la birebir aynı olduğunu doğruluyor.

**Gerçek pay (`slack_true`).** Her bomba için, bombalı parçanın çıkması için gereken en az çıkış sayısı (bombanın kendi çıkışı dahil) hesaplanıyor. Yöntem dal-sınır araması: gereken her parça için bir çıkış yönü seçiliyor ve gereken küme en küçük tutuluyor. Kilitli bir parça sayacı 0 olana kadar bekliyor; arada başka parça çıkarılabilir ve bu çıkışlar da sayılıyor. Pay = fitil − bu sayı. Pay 0 olduğunda bomba tam son hamlede alınıyor; negatif payda bomba kesin patlıyor. Elle kurulmuş iki tahtada test edildi (`SlackTrueIsFuseMinusTheShortestDigToTheBomb`, `ANailOnTheShortDigMakesTheOtherSideCheaper`).

Arama tam: döngü yaratan yön seçimi anında eleniyor. Düğüm bütçesi aşılırsa sonuç bir üst sınırdır (`slack_approx` = t). Test `TrueDigIsNeverLongerThanTheRecordedSolution`, gerçek yolun kayıtlı çözümden hiç uzun olmadığını doğruluyor.

**Doğal oyuncu Monte Carlo.** Her bölüm, her politika için 2000 oyun oynandı (tohum 69, 70, 71). Patlama oyunu durdurmuyor: oyuncu oynamaya devam ediyor, böylece bombayı "ne zaman alırdı" da ölçülüyor.
- `rand`: o an çıkabilen parçalardan rastgele biri.
- `greedy_edge`: kenara en kısa yoldan çıkan parça (eşitlikte rastgele).
- `bomb_aware`: %50 olasılıkla en acil bombanın (en az fitil) yolundaki açık bir parça, yoksa rastgele. Bombanın yolu başlangıçtaki en kısa kazı kümesi.

## 2. Özellik tanımları (`level_features_v2`)
| Sütun | Tanım |
|---|---|
| `fuse`, `slack_true` | bomba başına fitil ve gerçek pay (parça sırasına göre; CSV'de `;` ayraçlı, tabloda jsonb) |
| `slack_true_min`, `slack_true_mean` | en küçük / ortalama gerçek pay |
| `choke_n` | pay_true ≤ 1 olan bomba sayısı (boğaz) |
| `p_survive_<pol>` | hiç patlamadan bitiren oyun oranı |
| `nat_slack_med_<pol>` | oyun başına en sıkı bombanın doğal payı (fitil − bombalı parçanın çıktığı çıkış), oyunlar üstünden medyan. Negatif = patlar. |
| `nat_slack_per_bomb_rand` | `rand` için bomba başına doğal pay medyanı |
| `first_choke_ex_<pol>` | patlayan oyunlarda ilk patlamanın kaçıncı çıkışta olduğu, medyan |
| `open_min`, `open_mean` | çıkabilen parça sayısı: oyun başına en küçüğün medyanı / bütün adımların ortalaması |
| `bottleneck_n` | oyun başına open_n = 1 olan adım sayısı |
| `distractor_ratio` | ilk bomba alınana kadar ortalama (open − en acil bombanın yolundaki açık parça) / open |
| `distract_per_step` | **Eksen A.** Bomba başına, fitil penceresi boyunca (bomba alınana ya da fitil bitene kadar) adım başına bombanın yolunda olmayan açık parça sayısı |
| `distract_vs_slack` | çeldirici / (pay_true + 1), bomba başına; `_max` en kötü bomba |
| `narrow_n`, `narrow_hidden_n` | **Eksen B.** Oyun başına open_n ≤ 2 olan adım sayısı, ve bunlardan açık parçaların hiçbirinin "görsel serbest" olmadığı adımlar |
| `blocked_share_<neden>` | çıkamayan parçaların neden payı (adım × parça) |
| `narrow_by_<neden>` | oyun başına, en az bir parçayı o engelin tuttuğu dar adım sayısı |
| `jam_bait_start`, `jam_bait_mid` | açık görünen bir yönü olup o yönden çıkamayan parça sayısı: başta ve parçaların yarısı gittiğinde |
| `dir_ambiguity` | açık görünen 2+ yönü olan parçalar içinde yalnız 1 geçerli çıkış yönü olanların oranı (başta + ortada) |
| `pieces`, `locks`, `chains`, `walls`, `timer_s`, `sec_per_move`, `open_edge_pct` | var olanlar. `walls` = duvarlı şerit sayısı, `sec_per_move` = süre / parça |

Kesin tanımlar:
- **Açık görünen yön:** parçanın o yöndeki bütün hücre komşuları boş ya da duvarsız kenar.
- **Görsel serbest:** kilit yok, zincir değil, duvarlı kenara değmiyor, en az bir açık görünen yönü var.
- **Engel nedeni** (çıkamayan parça için tek neden, görünürlüğe göre sırayla):
  1. Kilit: geometrik olarak serbest ama sayaç > 0.
  2. Duvar: bir yönün yolu boş ama şerit duvarlı.
  3. Zincir: zincirli parçanın hücrelerinden biri tek başına olsa düz yolu boş (tırnaklar sayılmıyor).
  4. Başka parça veya tırnak: en az engelli yöndeki ilk durma sert (doluluk) mu, tırnak tutması mı.
- **Profil ölçüleri** (open, narrow, blocked, jam_bait, dir_ambiguity, distract) `bomb_aware` politikasının ilk 300 oyunundan; hayatta kalma ölçüleri 2000 oyunun tamamından.

## 3. pt1 replay (`replay_pt1`, `replay_pt1_steps`)
`moves` tablosundaki 82 129 pt1 satırı (1308 deneme) GameSession'da baştan oynatıldı.
- Her çıkıştan önce kaydın `fuses` alanı motorun fitil sayaçlarıyla karşılaştırıldı; hiç uyuşmazlık çıkmadı.
- Çekiç kaydındaki parçayla oynatıldı.
- **Makas:** kayıtta hangi eklemin kesildiği yok. Sonraki çıkışları en uzun süre geçerli tutan eklem seçildi.
- **Değnek:** rastgele; sonraki çıkışları en uzun geçerli tutan tohum seçildi (80 tohum içinden).
- Bombadan sonra "devam" varsa sonraki satırın fitil değerleriyle sürdürüldü.

| | Deneme |
|---|---|
| Toplam (moves) | 1308 |
| `plays` satırı var | 1279 |
| **Uyumlu** (aynı sonuç, aynı çıkış sayısı) | **1249 (%97,7)** |
| Bunlardan son çıkışı kayda düşmemiş kazanma (tek parça kalmış, çıkış = moves − 1) | 38 (`inferred` = `last_exit_unlogged`; uyumlu sayıldı) |
| Uyumsuz: kayıttaki çıkış motorda bloke (`exit_Blocked`) | 18 (çoğu S41 v1, 1. bölüm: son hamle) |
| Uyumsuz: sonuç farklı (`outcome_*`) | 9 |
| Uyumsuz: çıkış sayısı farklı (`exits_*`) | 3 |
| `plays` satırı yok (yarım kalan deneme) | 29 |

Replay süresi 11 sn.

Adım başına `on_path_n`, en acil bombanın o andaki en kısa kazı kümesinden hesaplandı. Aynı tam arama kullanıldı; önceki adımın planı üst sınır olarak verildi, bütçe 20 000 düğüm.

### 3a. Bomba ölümleri: yol dışı çıkış
420 uyumlu bomba ölümü. Ölüm başına ortalama **8,0 yol dışı, 11,3 yolda çıkış**. Yol dışı payı **0,41**.

"Yolda" = o anda patlayan bombanın en kısa kazı kümesinde olan parça.

| bölüm | ölüm | yol dışı (ort) | yolda (ort) | tür |
|---|---|---|---|---|
| S28 | 64 | 3,2 | 12,4 | sıkı fitil |
| S35 | 42 | 3,0 | 26,0 | sıkı fitil |
| S38 | 38 | 2,0 | 19,8 | sıkı fitil |
| S47 | 27 | 18,7 | 2,5 | çeldirici |
| S32 | 24 | 7,4 | 9,8 | karışık |
| S18 | 24 | 7,4 | 9,6 | karışık |
| S33 | 23 | 3,9 | 14,4 | sıkı fitil |
| S21 | 21 | 15,8 | 0,2 | çeldirici |
| Z23 | 21 | 2,9 | 7,6 | sıkı fitil |
| S34 | 21 | 20,6 | 0,4 | çeldirici |
| S20 | 20 | 6,9 | 0,1 | çeldirici (ders: kuralı bilmeme) |
| S24 | 12 | 14,4 | 6,6 | çeldirici |
| Z04 | 11 | 21,2 | 0,8 | çeldirici |
| S08 | 11 | 14,7 | 3,3 | çeldirici |
| S39 | 11 | 6,8 | 23,5 | sıkı fitil |

İlk koşuda arama hatası nedeniyle yol kümeleri boş çıkmış ve yol dışı payı yanlışlıkla 0,88 görünmüştü. Düzeltildi: döngü kontrolü seçim anında yapılıyor, arama tam. Yeni test: gerçek yol kayıtlı çözümden hiç uzun değil, 69 bombanın 22'sinde daha kısa.

### 3b. Takılma nedenleri (22 943 takılma, uyumlu denemeler)
| Neden | Pay |
|---|---|
| tırnak | %74,1 |
| zincir | %9,1 |
| başka parça | %8,5 |
| kilit | %6,0 |
| duvar | %2,3 |

Bütün takılmalar gerçekten çıkamayan parçada oldu: yolu açıkken "takılma" olarak kaydedilen hamle yok.

### 3c. Eksen B'nin insan maliyeti (adım düzeyi, 32 611 çıkış)
`dt` = önceki çıkıştan bu çıkışa geçen süre (60 sn'de kırpıldı). Takılma, aynı durumdaki takılma sayısı.

| Durum | Adım | sn/çıkış (ort) | medyan | takılma/çıkış |
|---|---|---|---|---|
| geniş (open > 2), engelsiz | 10 879 | 1,34 | 0,70 | 0,35 |
| geniş (open > 2), hepsi | 23 540 | 1,39 | 0,77 | 0,47 |
| dar (open ≤ 2), hepsi | 9 071 | 1,82 | 0,85 | 1,15 |
| dar, engelsiz | 2 762 | 1,33 | 0,67 | 0,63 |
| **dar, kilit kısıyor** | 960 | **2,57** | 1,08 | **1,92** |
| **dar, zincir kısıyor** | 3 843 | **2,07** | 1,03 | **1,45** |
| **dar, duvar kısıyor** | 3 838 | **2,04** | 0,92 | **1,38** |
| dar + gizli | 1 182 | 1,97 | 0,78 | 1,33 |
| dar + görünür | 7 889 | 1,79 | 0,87 | 1,12 |
| open_n = 1 | 3 476 | 1,99 | 0,82 | 1,50 |
| open_n = 2 | 5 595 | 1,71 | 0,87 | 0,93 |
| open_n = 3 | 5 362 | 1,47 | 0,82 | 0,64 |
| open_n ≥ 5 | 14 040 | 1,40 | 0,75 | 0,38 |

Yorum:
- Dar anın kendisi tek başına pahalı değil: engelsiz dar an 1,33 sn / 0,63 takılma.
- Pahalı olan, dar anı bir engelin yaratması: kilit, zincir ya da duvar seçenekleri kıstığında takılma 2–3 katına, düşünme süresi 1,5–1,9 katına çıkıyor.
- "Gizli" tanımı (yalnız zincir ve duvar görsel serbestliği bozuyor) bu farkı az yakalıyor. Engel nedenini doğrudan kullanmak (`narrow_by_*`) daha güçlü.

Bölüm başına özet: `replay_pt1_level_ozet.csv`.

## 4. Kontrol tablosu
Set `redesign-v4` (pt1'in oynadığı sürümler). Sütunlar:
- `eski pay`: `level_features.fuse_slack_min`.
- `pay_true`: gerçek pay, bomba başına.
- `p_*`: hayatta kalma oranı (`rand`, `greedy_edge`, `bomb_aware`).
- `nat_pay_*`: en sıkı bombanın doğal payı, medyan.
- `ilk_patlama`: ilk patlamanın kaçıncı çıkışta olduğu, medyan.
- `çeldirici`: `distract_max`.
- `dar*`: oyun başına adım sayısı.
- `tuzak_baş`: `jam_bait_start`.

**Bomba bölümleri**

| bölüm | slot | parça | fitil | eski pay | pay_true | boğaz | p_rand | p_greedy | p_aware | nat_pay_rand | nat_pay_aware | ilk_patlama | çeldirici | çeld/(pay+1) | open_ort | dar | dar_gizli | dar_kilit | dar_zincir | dar_duvar | tuzak_baş | yön_belirsiz |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| S31 | 32 | 54 | 30/28/33/32 | 0 | 1/1/1/1 | 4 | 0.262 | 0 | 0.565 | -1 | 0 | 33 | 0.613 | 0.306 | 1.586 | 50.06 | 11.51 | 5.023 | 12.053 | 45.447 | 14 | 0.25 |
| S17 | 26 | 33 | 19 | 2 | 7 | 0 | 0.075 | 0 | 0.766 | -3 | 1 | 19 | 3.378 | 0.422 | 3.541 | 13.55 | 1.033 | 0 | 11.033 | 0 | 12 | 0.593 |
| S32 | 34 | 54 | 18/27/17/18 | 0 | 2/5/2/2 | 0 | 0 | 0 | 0 | -10 | -8 | 17 | 2.655 | 0.885 | 2.628 | 25.983 | 3.627 | 0 | 2.23 | 9.92 | 13 | 0.25 |
| S18 | 17 | 33 | 17 | 2 | 5 | 0 | 0.002 | 0 | 0.424 | -7 | -1 | 17 | 3.451 | 0.575 | 3.624 | 11.237 | 1.043 | 0 | 5.483 | 5.887 | 14 | 0.578 |
| S47 | 8 | 30 | 24/20 | 12 | 18/16 | 0 | 0.182 | 0 | 0.991 | -4 | 8 | 20 | 8.376 | 0.493 | 5.993 | 4.41 | 0 | 0 | 0 | 0 | 11 | 0.505 |
| S08 | 31 | 36 | 18 | 11 | 12 | 0 | 0.172 | 0 | 0.992 | -4 | 8 | 18 | 5.806 | 0.447 | 4.285 | 9.81 | 0 | 0 | 0 | 7.893 | 13 | 0.432 |
| S50 | 28 | 28 | 18 | 1 | 12 | 0 | 0.076 | 0.006 | 0.99 | -7 | 8 | 18 | 8.608 | 0.662 | 8.03 | 2.24 | 0.113 | 0 | 0.017 | 0 | 6 | 0.595 |

**Çok duvar/kilit + karşılaştırma**

| bölüm | slot | parça | fitil | eski pay | pay_true | boğaz | p_rand | p_greedy | p_aware | nat_pay_rand | nat_pay_aware | ilk_patlama | çeldirici | çeld/(pay+1) | open_ort | dar | dar_gizli | dar_kilit | dar_zincir | dar_duvar | tuzak_baş | yön_belirsiz |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Z25 | 37 | 54 | 28/31/30/29 | 1 | 1/1/1/1 | 4 | 0.034 | 0 | 0.368 | -6 | -1 | 28 | 1.571 | 0.786 | 1.754 | 46.097 | 7.48 | 22.16 | 17.817 | 2.16 | 14 | 0.416 |
| S29 | 36 | 62 | 7/21/17/43 | 1 | 1/1/1/11 | 3 | 0 | 0 | 0.128 | -11 | -3 | 17 | 1.733 | 0.656 | 2.706 | 30.817 | 5.46 | 11.19 | 13.117 | 20.443 | 12 | 0.426 |
| Z23 | 21 | 54 | 9/18/11/22 | 1 | 1/1/1/1 | 4 | 0 | 0 | 0.064 | -3 | -2 | 9 | 1.458 | 0.729 | 1.731 | 46.79 | 8.44 | 4.453 | 4.613 | 45.79 | 15 | 0.222 |
| S06 | 4 | 30 |  |  |  | 0 | 1 | 1 | 1 |  |  |  |  |  | 3.684 | 6.983 | 0 | 0.01 | 0 | 0 | 14 | 0.315 |
| S36 | 35 | 47 | 35/15/23 | 1 | 1/1/1 | 3 | 0.051 | 0 | 0.632 | -4 | 0 | 23 | 1 | 0.5 | 2.058 | 32.027 | 7.157 | 6.567 | 6.63 | 31.02 | 9 | 0.485 |
| Z24 | 10 | 39 | 22 | 2 | 2 | 0 | 0.122 | 0.694 | 0.712 | -4 | 0 | 22 | 5.005 | 1.668 | 4.105 | 12.853 | 1.837 | 1.967 | 10.84 | 0 | 12 | 0.879 |

**Beklentiyle karşılaştırma**

1. **Gerçek pay, oyuncu davranışından ölçülenle aynı çıktı.** S50 12 (PM: ≥ 12), S17 7 (PM: 7), S18 5 (PM: 5), S47 18/16, S08 12. Eski kayıtlı pay bu bölümlerde 1–2 kadar düşüktü. Havuzda 69 bombanın 22'sinde gerçek pay kayıtlıdan büyük.
2. **S31 ve S32: beklendiği gibi.**
   - S31: `p_survive` rand %26, aware %57; `nat_slack_med` −1 / 0.
   - S32: `p_survive` 0 / 0; `nat_slack_med` −10 / −8.
   - Aynı boyut ve fitil düzeyinde S31'in bombaları doğal sırada zamanında çıkıyor, S32'ninkiler 8–10 çıkış geç kalıyor.
   - Bir fark: S31'in gerçek payı 1/1/1/1 (4 boğaz); S32'nin 2/5/2/2. Yani S31 kâğıt üstünde daha sıkı, ama doğal oyuncu bombaları zaten sırasında alıyor. "Pay tek başına yetmez" bulgusu motorda da doğrulandı.
3. **S17 ve S18: beklendiği gibi, ama yalnız bombaya bakan oyuncuda.**
   - S17 aware %77, doğal pay +1; S18 aware %42, doğal pay −1.
   - Rastgele oyuncu ikisinde de ölüyor (%7,5 ve %0,2). Fark, bombaya bakan oyuncuda ortaya çıkıyor.
4. **S47, S08 ve S50: kısmen.**
   - Rastgele oyuncuda `p_survive` düşük (%18, %17, %8; doğal pay −4, −4, −7).
   - Bombaya bakan oyuncuda %99. Bu bölümlerde zorluk tamamen dikkat: bombaya bakan oyuncu kesin kurtuluyor, bakmayan kesin ölüyor.
   - **Çeldirici beklendiği gibi yüksek.** `distract_per_step` 8,4 / 5,8 / 8,6. Karşılaştırma: S31 0,6, S32 2,7, Z25 1,6.
   - Replay de aynı şeyi söylüyor: S47 ve S08 ölümlerinde çıkışların çoğu yol dışında (18,7 ve 14,7'ye karşı 2,5 ve 3,3).
5. **Z25, S29, Z23 `narrow_hidden_n`: kısmen.**
   - Değerler 7,5 / 5,5 / 8,4. Engelsiz açık tahtalarda bu değer 0 (S06, S47, S08). Yani yüksek.
   - Ama en yüksek değer bunlarda değil: S31'de 11,5, S36'da 7,2. Bunlar da duvarlı/zincirli.
   - Dar anların kaynağı farklı:
     - Z23 ve S36'da neredeyse hep duvar (`narrow_by_wall` 45,8 ve 31,0).
     - Z25'te kilit (22,2) ve zincir (17,8).
     - S29'da üçü birden.
   - "Gizli" tanımı yalnız zincir ve duvarı görsel serbestlik dışı sayıyor. Kilidin yarattığı dar an, açılan parça görünür serbest olduğu için gizli sayılmıyor. Eksen B için `narrow_by_*` daha doğrudan bir ölçü.
6. **`greedy_edge` neredeyse her bomba bölümünde ölüyor** (p ≈ 0; tek istisna Z24 %69). Kenardan alan oyuncu bombaya hiç gitmiyor. Bu politika "düşünmeden görünen parçayı alan" oyuncunun alt sınırı olarak kullanılabilir.
7. **`jam_bait_start` bölümler arasında az ayırt ediyor** (6–15). Takılmanın %74'ü tırnaktan, ve çoğu tahtada açık görünüp tırnakla tutulan 10+ parça var. `dir_ambiguity` daha ayırt edici: Z24 0,88, S36 0,49, Z23 0,22.

## 5. Supabase (bekliyor: Zeyd'in onayı)
- `docs/sql/018_pm69_features_v2.sql`: 3 yeni tablo, anon okuma politikası. Mevcut tablolara dokunmuyor.
- Yükleme `node Tools/pm69_upload.js` ile yapılacak. Yükleme süresince her tabloya geçici bir anon insert politikası konup hemen ardından kaldırılacak.
- Satır sayıları: `level_features_v2` = 250, `replay_pt1` = 1308, `replay_pt1_steps` = 33 028.
- **Zeyd "çalıştır" derse:** migration, geçici politika, yükleme, politikayı kaldırma, sayım kontrolü (~2 dk).

## 6. Dosyalar ve süreler
- EditMode 718 geçti / 0 kaldı (2 atlandı), yeni testler `DifficultyV2Tests` (4).
- `level_features_v2.csv` (havuz: redesign-v4 40 + Z21 v1, p22 40, redesign-v3 40, redesign-v2 40, zeydbot 40, web-v1 40, designs 8), hesap süresi 113 sn (250 bölüm × 3 politika × 2000 oyun).
- `replay_pt1.csv` (1308 deneme; repoda `player` sütunu takma ad p001–p057, gerçek ad `row_id` ile `moves`'tan bulunur ve Supabase'e adla yüklenir), `replay_pt1_steps.csv` (adım başına), `replay_pt1_level_ozet.csv`.
- `pm69_replay_ozet.py`, `pm69_kontrol.py`.
- Veri indirme ve yeniden üretme:
  1. `node Tools/pm69_fetch.js`, sonra `node Tools/pm69_prep.js`.
  2. `bash Tools/unity_batch.sh method ReverseSolver.EditorTools.Pm69Run.Replay`.
  3. `bash Tools/unity_batch.sh method ReverseSolver.EditorTools.Pm69Run.Features 2000`.
