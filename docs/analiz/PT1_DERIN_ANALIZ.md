# pt1 derin analiz: tahmin vs gerçek (PM, 14 Kasım)

Kaynak: Supabase `v_pred_vs_actual`, `v_model_score`, `v_attempts`, `level_features` (cohort pt1, 51 oyuncu, 4–10 Kasım). Tahminler pt1'den ÖNCE donduruldu: v0 (31 Ekim, özellik regresyonu + FunSim), v0.1 (2 Kasım, birincil), v1 (1 Kasım, oyuncu botları + nüfus Monte Carlo, "ikincil / fazla kötümser" denmişti).

## 1. Modellerin karnesi
| Model | Brier (düşük iyi) | Sabit tahmin tabanı | MAE ilk deneme | Sıralama uyumu (Spearman) |
|---|---|---|---|---|
| v0 | 0,161 | 0,214 | 0,186 | 0,80 |
| v0.1 (birincil) | 0,160 | 0,211 | 0,181 | 0,80 |
| **v1 (botlar)** | **0,139** | 0,214 | **0,135** | **0,86** |

**Ders 1:** "fazla kötümser" diye ikinciye düşürdüğümüz **bot modeli (v1) en iyisi çıktı.** v0, bomba bölümlerini sistematik olarak kolay sandı.

## 2. Bölüm bölüm: kaç tahmin tuttu?
Yeterli veri olan 34 bölümde (n ≥ 8): **24 tuttu, 10 "beklenenden ZOR", 0 "beklenenden kolay"** (v0.1). Yanılgı hep aynı yönde: oyun düşündüğümüzden zor.

Beklenenden zor çıkanlar (v0.1 tahmin → gerçek ilk deneme):
| Bölüm | Kimlik | Tahmin | Gerçek | Ana kayıp | Not |
|---|---|---|---|---|---|
| 4 | S06 | 0,93 | 0,75 | süre %25 | bombasız; 30 parça / 55 sn sıkı |
| 8 | S47 | 0,82 | 0,63 | bomba %34 | ilk 2 bombalı bölüm |
| 9 | S20 | 0,81 | **0,38** | bomba %52 | fitil payı 5; kazananların %38'i booster kullandı |
| 10 | Z24 | 0,91 | 0,74 | süre %17 | zincir dersi; ortalama 30 takılma |
| 11 | S21 | 0,70 | **0,29** | bomba %57 | fitil payı 2; kalan süre medyanı 10 sn |
| 16 | Z04 | 0,68 | 0,43 | bomba %43 | %29 booster |
| 17 | S18 | 0,65 | **0,14** | bomba %64 | 3,5 deneme; %31 booster |
| 19 | S28 | 0,42 | **0,00** | bomba %92 | 4 bomba, fitil payı 0; 5,6 deneme |
| 25 | S35 | 0,44 | **0,00** | bomba %80 | 7,1 deneme; %38 booster |
| 34 | S32 | 0,43 | **0,00** | bomba %100 | 4,3 deneme |
(n 7 ile "veri az": 38 S38 0/7 ve 8,1 deneme; 40 S33 0/7 ve 4,4 deneme; ikisi de bomba.)

Beklenenden **kolay** (istatistikte "tuttu" ama belirgin): 26 S17 (0,69 → 1,00), 27 S10 (0,79 → 1,00), 37 Z25 (0,55 → 0,86), 15 S13 (0,84 → 0,94), 22 S19 (0,79 → 0,90). Hepsi bombasız ya da fitil payı geniş.

## 3. Hangi engel beklenenden zor?
**Bomba, açık ara.** Kayıpların büyük çoğunluğu bomba. Belirleyici olan bomba sayısından çok **fitil payı** (en dar bombada, gereken hamleden fazla kalan sayı):
| En dar fitil payı | Bölümler | Ortalama "bomba ile kayıp" (ilk deneme) |
|---|---|---|
| 0 | 19, 25, 30, 32, 33, 34, 38 | ~%65 |
| 1 | 20, 21, 28, 35–37, 39, 40 | ~%40 |
| 2 | 11, 13, 16, 17, 18, 26 | ~%35 |
| 4–5 | 9, 12, 23 | ~%45 (9'da yeni öğreniliyordu) |
| 8–12 | 6, 7, 8, 27, 29, 31 | ~%15 |
- **Kilit, duvar, zincir kaybettirmiyor**, düşündürüyor: 14 Z21 (duvar dersi) %94, 15 S13 %94, 22 S19 %90, 24 S14 %90. Ama takılma sayısını büyütüyorlar (30+ bölümlerde ortalama 30–77 takılma): deneme-yanılma çok.
- **Süre** ancak birkaç bölümde belirleyici: 4 S06 (%25), 10 Z24 (%17), 35 S36 (%57), 38 S38 (%43).

## 4. "Zor dedik, herkes rahat geçti" / "kolay dedik, zorlandı"
- Zor dedik, rahat geçildi: **32 S31** (zor rolü, hedef 0,45–0,52 → 0,75; 170 sn süre çok cömert, kalan süre medyanı 101 sn), **39 S40** (0,57), **23 S27** (zirve, 0,40 hedefte).
- Kolay/orta dedik, zorlandı: **11 S21** (yükseliş, hedef 0,75–0,82 → 0,29), **17 S18** (0,67–0,75 → 0,14), **21 Z23** (orta, 0,67 → 0,27), **20 S24** (orta 0,60 → 0,33), **4 S06** (ısınma 0,90 → 0,75).

## 5. Booster ve tekrar davranışı ("saniye arttırıp geçmek")
pt1'de devam/+süre satın alma yoktu; tek "kurtarıcı" booster'lardı.
- Kazanılan oyunlarda booster kullanımı: 9 (%38), 25 (%38), 17 (%31), 16 (%29), 15 (%27), 5 (%22), 23 (%20). Booster en çok bombalı duvarlarda kullanılıyor.
- Ortalama deneme (kazananlar): 38 S38 8,1 · 25 S35 7,1 · 19 S28 5,6 · 40 S33 4,4 · 34 S32 4,3 · 36 S29 3,6 · 17 S18 3,5. 35+'a gelen 7 kişi çok ısrarcı (hepsi bitirdi), ama bu oyuncular seçilmiş en sabırlılar.
- Süre çok cömert olan yerler (kazananların kalan süre medyanı): 1–2 (~110 sn), 32 (101 sn), 19 ve 30 (59–65 sn). Süre baskısı yok.

## 6. Nerede bırakıyorlar?
- Kaybedip bırakma: 8 ve 9 (3'er kişi), 25 (2). Bomba duvarları.
- Kazanıp bırakma (asıl kayıp): 1–5 arası 15 kişi (PM_59–68'deki onboarding ve "bir tane daha" işleri bunun için).

## 7. Sonuç ve öneriler
1. **Bomba fitil payı kuralı** (kitaba): 1–20 arası en dar fitil payı **≥ 4**; 21–30 **≥ 2**; **0–1 yalnız "zor bölüm"de**, art arda değil.
2. **Birincil tahmin modeli v1 (botlar)** olsun ya da v0 + v1 ortalaması; v0'da bomba ağırlıkları yükseltilsin. `dongu.py --cohort pt1 --run v2` ile **v2'yi dondur**, p22 verisiyle sına.
3. **21–40'ı da PM_68 yöntemiyle ayarla**: özellikle 21 Z23, 25 S35, 34 S32, 38 S38, 40 S33 (hepsi %0–27 ve fitil payı 0–1). 32 S31'in süresini kıs (çok kolay).
4. 4 S06 ve 10 Z24'e +10 sn süre.
5. Kilit/duvar/zincir bölümleri iyi; takılma sayısı yüksek ama kayıp yok. Bunlar "düşündüren" engel olarak kalsın.
6. Veri sınırı: 20'den sonra 7–13 oyuncu; sonuçlar yönü gösteriyor ama kesin değil. p22 ile yeni test (pt2) şart.
