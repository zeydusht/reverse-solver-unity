# PM → Claude Code (31 Ekim 2026) — Veri sistemi Faz 1 + Faz 2

Önce oku: `docs/VERI_SISTEMI.md` (bütün plan; özellikle bölüm 4, 6, 7). Supabase şeması kuruldu: `docs/sql/009_veri_sistemi.sql`.
PM_MESAJ_24 (koridor) bekliyorsa bu mesaj ondan ÖNCE gelir: oyun testinin davetleri buna bağlı.

Genel kurallar:
- Kök Build 5'e dokunma; iOS zip üretme. Tüm oyun değişiklikleri önizlemede (`p19/?set=redesign`).
- Model tablolarına (level_features, level_targets, model_runs, level_predictions, player_bots) anon ile yazamazsın, bu bilerek böyle. Sen **dosya üret** (`docs/veri/*.csv|json`), PM Supabase'e yükler. Okuma anon REST ile serbest.
- Her adım sonunda OVERNIGHT.md başına 1–2 satır.

---

## FAZ 1 — Kayıt eksikleri (oyun testi bunlarsız başlamaz)

1. **Kohort:** URL `?c=<kod>` (ör. `?set=redesign&c=pt1`). Değer cihazda saklansın (sonraki açılışta link parametresi yoksa da kalsın). Tüm `plays`, `moves`, `player_profiles` satırlarına `cohort` yaz. Etiket yoksa `null`.
2. **device_id:** ilk açılışta rastgele uuid üret, sakla (web: localStorage, iOS: PlayerPrefs); `plays.device_id` ve `moves.device_id`.
3. **platform:** `ios | android | desktop | mobile-web` → `plays.platform`. **app_version** → `plays.app_version` (ör. `web-p19-2026-10-31`).
4. **Yarıda bırakma ve kayıp hamle:**
   - Bölüm açılınca `moves` satırı `kind='start'` (move_no 0, aynı row_id).
   - Hamleleri artık her 10 hamlede ve sekme/uygulama arka plana geçerken (`visibilitychange` / `OnApplicationPause`) gönder; `kind='hide'` / `'show'` satırları ekle. Menüye dönüşte `kind='quit'`.
   - `(row_id, move_no)` benzersiz: tekrar gönderimde `Prefer: resolution=ignore-duplicates` kullan.
5. **jam satırında denenen yön:** `dir` doldurulsun (şu an null).
6. **Zorluk sorusu (yalnız kohort varken):** her bölüm sonunda (kazanınca VE kayıptan sonra, bölüm başına en fazla bir kez) "Bu bölüm ne kadar zordu? 1–10" + "Geç". `plays.zorluk_puani`. 👍/👎 sorusu olduğu gibi kalır. Kart tek dokunuş, oyunu yavaşlatmasın.
7. **Profil sorusu (yalnız kohort varken, ilk açılışta bir kez):** isim girişinden hemen sonra "Bu tür bulmaca oyunlarını oynar mısın? Hiç / Ara sıra / Sık" → `player_profiles (cohort, player, device_id, platform, puzzle_exp 0/1/2)`.
8. **Kontrol:** `?c=ic` ile 3 bölüm oyna (bir kazanma, bir bomba, bir yarıda bırakma). Supabase'de şunlar görünmeli:
   - `v_attempts` satırlarında cohort/platform/device_id dolu
   - `moves`'ta start / hide / quit
   - `v_abandoned`'da bırakılan deneme (30 dk sonra)
   - `v_level_actual`'da `cohort='ic'`
   Sorgu çıktılarını OVERNIGHT.md'ye yapıştır.
9. **pt1 linki:** `https://zeydusht.github.io/reverse-solver-unity/p19/?set=redesign&c=pt1` (+ Türkçe için `&lang=tr`). Linki OVERNIGHT.md'ye yaz.

## FAZ 2 — Botlar ve nüfus Monte Carlo

10. **Özellik tablosuna simülasyon metrikleri:** Her SET40 v4 bölümü için SmartPlayer/FunSim'den şunları üret: açılış hamlesi k₀ (M1 metriği), serbest parça tavanı (M2), en dar fitil payı (gerçek en kısa yol ile), tuzak sayısı (çözülemez duruma götüren ilk hamle sayısı/adım), rastgele kazanma, FunSim ilk deneme. Çıktı: `docs/veri/level_sim_v4.csv` (level_id, level_version, sütunlar).
11. **Yeniden oynatıcı → hamle tablosu:** Supabase'deki tüm `moves` (eski oyuncular + Zeyd + pt1 geldikçe) Unity'de tahta durumuyla yeniden oynatılsın. Her **seçim anı** için satır: oyuncu, bölüm, deneme, adım, o anki yasal hamleler (aday listesi) ve her aday için özellikler (doğru mu = kazanılabilir durumda kalıyor mu, en dar fitil payına etkisi, kaç parçayı serbest bırakıyor, çivili mi, zincir ortağı, son dokunulan parçaya mesafe), seçilen aday, think_ms, kalan süre. Çıktı: `docs/veri/secimler.csv`. (ZeydBot adım 1'deki yeniden oynatma kodunu temel al: `docs/analiz/zeydbot/RAPOR.md`.)
12. **Bot eğitimi (M2):** `Tools/veri/bot_fit.py`
    - Seçim modeli: koşullu logit (aday özellikleri → seçim olasılığı), parametreler `VERI_SISTEMI.md` 6/M2 tablosundaki gibi.
    - Düşünme süresi: log-normal, seçenek sayısı ve fitil baskısı ile.
    - jam_rate, learn, platform etkisi.
    - **Kısmi havuzlama:** her oyuncunun parametresi = nüfus ortalaması + kendine özel sapma (hiyerarşik/empirik Bayes; az veri → ortalamaya yakın).
    - Çıktı: `docs/veri/botlar_<run>.json` (oyuncu başına params + nüfus ortalama/kovaryans).
13. **Nüfus Monte Carlo (M3):** Unity'de bot oyuncu (BotPlayer) bu parametrelerle oynasın. Nüfus dağılımından 1000 sanal oyuncu çek, her SET40 v4 bölümünü oynat (ilk deneme + kaybedince 2. deneme, `learn` ile). Bölüm başına: p_win1 (+%90 aralık), p_fail_bomb, p_fail_time, mean_attempts, mean_dur_s, mean_left_frac, mean_jams. Çıktı: `docs/veri/tahmin_v1.csv` — sütun adları `level_predictions` tablosuyla aynı.
14. **Geri test (pt1'den önce, mevcut veriyle):** Oyuncu-dışarıda-bırak ve bölüm-dışarıda-bırak; v1'i v0 ve sabit tabanlarla karşılaştır (Brier, MAE, Spearman). Çıktı: `docs/veri/geri_test_v1.md`. **Dürüst ol:** v1 tabanları geçemiyorsa açıkça yaz; o zaman pt1'e v0 ile gireriz.
    Not: şu anki veride neredeyse yalnız Zeyd var; bot nüfusu pt1 gelmeden tek oyuncudan kurulur. Bu yüzden v1'in değeri asıl pt1'den sonra (Faz 4) ölçülecek. Faz 2'nin amacı **boru hattının hazır olması**: pt1 verisi gelince tek komutla yeniden eğit + yeniden tahmin.
15. **Tek komut:** `python Tools/veri/dongu.py --cohort pt1 --run v2` → Supabase'den çek → yeniden oynat → bot eğit → Monte Carlo → `tahmin_v2.csv` + `botlar_v2.json` + kısa rapor. Bunu `docs/VERI_SISTEMI.md` bölüm 11'e ekle.

## Bitince
- OVERNIGHT.md: Faz 1 kontrol çıktıları, pt1 linki, v1 geri test özeti (3 satır).
- PM'e: `tahmin_v1.csv`, `botlar_v1.json`, `level_sim_v4.csv` hazır de; PM Supabase'e yükleyip v1'i dondurur, sonra davetler gider.
