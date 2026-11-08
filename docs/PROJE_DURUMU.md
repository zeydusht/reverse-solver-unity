# Reverse Solver — PROJE DURUMU (PM devir dosyası)

Son güncelleme: 8 Kasım 2026, 19:35. Yeni sohbette önce bu dosyayı oku, sonra Zeyd'in sorusuna geç. Eski sohbeti tekrar etme.

## Dosya kuralı
- Her iş bitince veya Zeyd "değişiklikleri PROJE_DURUMU'na ekle" deyince bu dosyayı güncelle (kısa tut, bitenleri sil, tarihi yenile). Güncelledikten sonra `/clear` hatırlat.

## Zeyd ve çalışma biçimi
- Zeyd Tekin: Türkçe konuşur, MIS öğrencisi, mobil oyun PM'i olmak istiyor. Yazılımda deneyimli değil: **basit, adım adım** anlat. Sorunu davranışından (tıklıyorum, olmuyor) anla, sebebi sen bul.
- Opus tercih eder. Bitmiş işin sonunda `/clear` hatırlat. Token tasarrufu önemli: kısa konuş, tekrar etme, uzun log okutma.
- Benim rolüm PM: proje dosyalarını cihaz köprüsüyle oku/yaz, Supabase'e bak, Claude Code için `docs/PM_MESAJ_N.md`, Codex için `docs/art/CODEX_TASKNN.md` yaz. Her cevabın sonunda "Sıradaki görev" ve açık işler.
- Zeyd uzun terminal komutlarını elle yazamıyor: komutu **metin dosyası** olarak gönder (SendUserFile), o Drive'a yükleyip Mac'te açıyor.
- Build/yayın durumu için dosya tarihlerine bak (`Builds/`), tahmin etme.

## Proje
- Oyun: yapboz parçası çıkarma bulmacası, Unity 6.6.4f1. Yerel: `C:\Users\zeydt\Downloads\Reverse Solver\ReverseSolver`. Repo: `zeydusht/reverse-solver-unity`. Supabase proje: `vwneqdhzfevgbuwoadod`.
- Web (GitHub Pages): kök https://zeydusht.github.io/reverse-solver-unity/ (Build 5, ELLEME, "kökü güncelle" onayı bekliyor). Önizleme: `/p19/?set=redesign` (Türkçe `&lang=tr`). Demo: `/br/?set=redesign&painting=1&lv=SHOW1`. Gizlilik: `/privacy/`.
- Seviyeler ve 40 tablo çalışma zamanında Pages'ten iner, seviye değişikliği yeni iOS build gerektirmez. **SET40 v4 önizlemede yayında** (p19; 9 bölüm ayarlandı, PM_MESAJ_23). Kökteki `/levels/redesign.json` hâlâ v3: iOS ve kök v4'ü "kökü güncelle" denince alır.
- Önizlemede bitenler (31 Ekim): masaüstü 9:16 alan (PM_21), zincir kilidi hücre ortasında (PM_22), SET40 v4 (PM_23). iOS zip 0.5.1 (4) hazır (kilit düzeltmesi dahil), Apple'a gönderilmedi.
- Sanat: Codex (yalnız `Assets/Art` ve `docs/art`), entegrasyon: Claude Code (`docs/PM_MESAJ_N.md`). Palet `docs/art/PALETTE.md`. Fredoka Türkçe, LilitaOne yalnız İngilizce etiketler.
- Supabase `plays` tablosu: TEST_* ve ddd analizden hariç, `deneme1` = Zeyd'in 40 seviyelik zorluk puanlaması.

## Veri sistemi (1 Kasım 2026, `docs/VERI_SISTEMI.md`)
- Amaç: her bölüm için "ne ayarladık / ne bekledik / veri ne dedi" + tahminleri oyun testinden önce dondurmak, oyuncu botları + nüfus Monte Carlo ile kendini düzelten tahmin.
- Supabase kuruldu (009): `cohort/platform/device_id` sütunları, `player_profiles`, `level_features`, `level_targets`, `model_runs`, `level_predictions` (dondurulunca değişmez, tetikleyici test edildi), `player_bots`, görünümler `v_attempts`, `v_level_actual`, `v_funnel`, `v_abandoned`, **`v_pred_vs_actual`**, `v_model_score`. Model tablolarına yalnız PM (MCP) yazar; anon okur.
- **v0 tahmini donduruldu** (`v0-2026-10-31`): regresyon (bomba, fitil payı, duvar, zincir, çivi) + FunSim kalibrasyonu ortalaması, LOO MAE 1,25 puan (sabit 2,86), Spearman 0,85. Puan→oran varsayımı A1: win1 = 0,97 − 0,075·(puan−1). Zayıf: tuzak görmüyor (S27), uçları ortaya çekiyor.
- Kohort: `pt1` = 25–30 kişilik ilk test (`?c=pt1`), `ic` = ekip içi. **pt1 linki Claude Code Faz 1'i bitirmeden dağıtılmaz.**
- PM_25 bitti (1 Kasım): kayıt eksikleri yayında (`ic` kontrolü geçti), botlar + Monte Carlo → **v1 donduruldu** (`v1-2026-11-01`, İKİNCİL: geri testte sabit tabanı geçemedi, fazla kötümser; tek oyunculu veri). Birincil tahmin v0.
- **pt1 DAVET BEKLİYOR, test p20'den yapılacak** (yeni menüler PM_26/27 + yeni eğitim PM_28). Link `https://zeydusht.github.io/reverse-solver-unity/p20/?set=redesign&c=pt1&lang=tr` (davet metnindeki p19'u p20 yap). p19 yedek, hedef 25–30 kişi. Önce koridor geçişi (PM_24) p19'a eklenecek, sonra **Zeyd ne zaman derse** davet gider (mesaj metni hazır). Davetler gidince p19 dondurulur, yeni işler `p20/`'ye. Zeyd kendisi `&c=ic` ile oynar. Takip: `v_funnel`, `v_pred_vs_actual`, `v_model_score` (cohort pt1). pt1 bitince: `python Tools/veri/dongu.py --cohort pt1 --run v2`.

## iOS / TestFlight (şu an en önemli)
- Bundle `com.zeydtekin.reversesolver`, sürüm **0.5.1 (3)**, Team ID 9X55V64NRR. Zip: `Builds/ReverseSolver-iOS-0.5.1.zip`.
- Apple'a **yüklendi** ve işlendi. App Store Connect → Reverse Solver (app id 6827648724) → TestFlight.
- İç grup **Ekip** (build "Testing"), dış grup **Arkadaslar** (boş). Zeyd'in kendi iPhone'una (Apple ID zeydtekin12@gmail.com) **davet maili gelmiyor**; TestFlight uygulaması boş.
- **Dış gruba build eklerken** "There was an error processing your request" (web) / "Validation Error" (App Store Connect uygulaması). "What to Test" kaydederken de aynı. Test Information dolu. Büyük olasılıkla Apple tarafında/hesap sorunu. Denenecekler: Apple sistem durumu, başka tarayıcı, bekleme, **Apple Developer Support'a yazmak** (davet maili + dış test hatası birlikte). Hâlâ yapılmadı/sonuç yok.
- Arkadaşını Users and Access'ten ekibe davet etti; kabul edince iç gruba eklenebilir (rol düşük olsun).
- Kullanıcı adı "Zeyd TEKİN" içindeki **İ harfi** Mac imzasında sorun çıkardı. Çözüm betikleri `imza-duzelt.sh` ve `imza-duzelt-2.sh` (Drive'da). Gelecek yüklemelerde Xcode → Archive → Export → bu betikler → `xcrun altool --upload-app` (uygulamaya özel parola gerekir, sonra sil) gerekecek.
- Bunun için arkadaşın Mac'i kullanıldı (Xcode'dan çıkış yapılıp yapılmadığı belirsiz). Parola silindi.
- **5 Kasım: 0.5.1 (3) Zeyd'in iPhone'unda çalıştı** (3 ekran kaydı, `Downloads\Mobile Devices`). Bulgular: (1) oyun sabit **30 FPS** (targetFrameRate ayarlı değil, Unity iOS varsayılanı), web 60; (2) titreşim yalnız jam/bomba/kazanmada var, parça çıkarmada yok → "titreşim yok" hissi. İkisi **PM_MESAJ_45**'te (60 FPS + yeni titreşim haritası, sürüm 0.5.2 (5), p20 değişiklikleri dahil). Zeyd ayrıca iPhone Ayarlar → Ses ve Dokunuş → Sistem Dokunuşları açık mı bakacak.
- **7 Kasım: 0.5.2 (6) hazır** (`Builds/ReverseSolver-iOS-0.5.2.zip`, 299 MB, yüklenmedi). PM_45: iOS'ta 60 FPS (açılış + arka plandan dönüş), yeni titreşim haritası (parça Light, sürükleme/düğme Select, jam Medium, bomba Heavy, kazanma Success, 50 ms aralık). PM_46: Success ve Heavy 50 ms kuralına takılmıyor; son parçada Light yok, yalnız Success. iPhone'da doğrulanmadı.

## pt1 YAYINDA (4 Kasım 00:00) — en güncel durum
- Davetler 3–4 Kasım gecesi gönderildi. Link `https://zeydusht.github.io/reverse-solver-unity/p20/?c=pt1` (davetliler 1–2 kişiye iletebilir). İlk gerçek oyuncu: Tolgahan (00:10).
- **p20 DONDU** (sürüm `web-p20-2026-11-03`). Yeni işler `p21/`. Kök ve p19'a dokunma.
- p20'de olanlar (PM_34–44): eldivenli el (arkadan), zor bölümde tek Baron kartı, Ev profil hizası, Task23 harita düğümleri, kart tıklama kilidi (PM_38), 40 yeni tablo (Paintings_v2, Task24/25 stili), Task23 kartları, Baloo 2 ExtraBold başlık/düğme stili (krem-sarı geçiş, koyu kontur, gölge), **bölüm sonu puanlama KALDIRILDI** (zorluk_puani/keyif boş; set_rating kodu duruyor), profil sorusu bölüm 2'yi kazanınca, süre başlamadan dokunuş jam sayılmaz ('tut'), WhatsApp önizleme = oyun ikonu. Boyut 16,33 MB.
- **Veri temizliği (migration 013):** `v_attempts`/`v_abandoned` şunları dışlar: adında "zeyd" geçen, tek harf tekrarı (dd, zz…), `excluded_players` tablosu, `excluded_devices` (Zeyd'in 7 cihazı), pt1'de 4 Kasım 00:00 öncesi. Yeni test ismi çıkarsa `excluded_players`'a ekle.
- **Oyuncu tablosu `v_oyuncular`** (migration 014, 7 Kasım): grup, oyuncu, bitirdiği bölüm, dakika, ilk/son giriş. Zeyd Supabase Table Editor'dan bakar. 7 Kasım: pt1'de 29 oyuncu.
- Takip: `v_funnel`, `v_pred_vs_actual` (run `v0.1-2026-11-02` birincil), `v_model_score`, cohort pt1. Tahminler kazanma oranına dayanıyor (puan artık yok).

## Limitler ve çalışma düzeni
- Claude (Pro) haftalık limit sıkışık, sıfırlanma **cumartesi 15:00**. Codex haftalık ~%20 kalmıştı (Task25 sonrası daha az). Codex CLI 0.166.0.
- Codex artık telefondaki Codex uygulamasından Zeyd'in bilgisayarında (yerel) çalışıyor; test edildi. Komutlar oraya yapıştırılabilir.
- Bilgisayar prizde, uyku kapalı olmalı.
- **Cumartesi yapılacak:** otonom PM döngüsü (PM saatlik uyanır, görev dosyası yazar, Claude Code/Codex işçi, QA ajanı, temizlik ajanı `_silinecek/` klasörüne taşır, akşam özeti). Ayrıca dosya şişmesi: PRODUCT.md (116 KB) ve OVERNIGHT.md (48 KB) arşive bölünecek.

- **Açılış hikâye animasyonu (8 Kasım):** `docs/art/CODEX_TASK26.md` (15–20 sn, 6 sahne: çayır → Baron toplar → çerçeveler → koridor → el tabloyu söker "weee" → "Free them all!" ve koridor zoom'u). Önce storyboard, Zeyd onayı, sonra final sprite + tween (≤2 MB). Hikâye kaynağı `docs/HIKAYE.md`.

## Açık işler (öncelik sırası)
1. pt1 takibi: her akşam `v_funnel` + bırakma noktaları + tahmin karşılaştırması. 8 Kasım sonrası rapor + v2.
2. Cumartesi: otonom sistem kurulumu + temizlik (`docs/art/TEMIZLIK_ONERI.md`: 25,6 MB güvenli cache + 1,57 GB arşiv adayı; Redesign_T22/T23 denemeleri git dışına).
3. p21 adayları: UX_KOD_DENETIM.md'deki orta bulgular, redesign doğrulama aracı (yeni krem kart), ZeydBot, kalan Task23 aileleri (yükleme, HUD cilası, booster çubuğu, 40. bölüm kartı).
4. TestFlight: Apple vaka 103014067408 (4 Kasım telefon görüşmesi, "48 saat bekle"). **5 Kasım: Zeyd kendini internal tester olarak ekleyebildi.** Sıradaki: iPhone'a 0.5.1 (3) kurulumu, dış grup (Arkadaslar) tekrar denemesi; olmazsa aynı maile cevap (taslak verildi). Not: 0.5.1 (3) eski sürüm (p20 değişiklikleri yok, seviyeler kökteki redesign.json v3) → testçilere iOS dağıtılmadan önce yeni iOS build gerekir (İ imza prosedürü).
5. Görkem Türk: 3 Kasım'da hatırlatma taslağı verildi ("abi" hitabı); pt1 ilk sonuçlarıyla dönmek daha güçlü.
6. **Bölüm tasarım okulu (8 Kasım):** "Bölüm bir hikâye." Referans: `Level Design/kareler/` (RM 3473–77, Block Out 273–78). **`docs/BOLUM_TASARIM_KITABI.md` v0.2**: 11 hikâye kartı, K1–K12 kuralları (SET40 eşikleri işlendi), yeni engel tarifi tanıt→geliştir→bük→bitir, bomba sert kuralları, D2 kendi verimiz, ajan planı F1–F6. Bot ile Royal Match oynatmak YOK.
   - PM_47 bitti: hikâye eğrisi ölçümü (`docs/analiz/HIKAYE_EGRISI_RAPOR.md`, `Tools/hikaye_egrisi.py`). Bulgular: tuzak yalnız bombadan geliyor (oyunda karar anı az), 1–9 aynı düz eğri (benzerlik buradan), bomba D kuralını kalanlar 6, 7, 8, 9, 12, 23, 27, 29, 31, son 10 bölümde nefes yok.
   - PM_48/48B bitti (8 Kasım): ajan döngüsü çalışıyor (`Tools/ajan/dongu.py --kart … --uret`). p21 yayında: `p21/?set=ajan1&lang=tr` (06-A/B tanıt, 07-A/B geliştir; eski 6–7'de bomba yok sayılabiliyordu, yenilerde unutan kaybediyor). PM kararı: tanıtımda R12 (rastgele bot %15–50) fitil=k+2 ile çelişiyor; 07-A FunSim %4,7 (normal tip için alt sınır yok). Rapor `docs/analiz/AJAN_DILIM1_RAPOR.md`.
   - **ajan1 Zeyd notu (8 Kasım 19:23):** tahtalar dikey geldi (kare istiyor), hâlâ tutorial, süreden kaybetti → kitaba "Öğrenme dönemi" (1–10'da kayıp yalnız yeni öğeden, süreden kayıp ≤%5/15) + "Tahta oranı" (kare, çeşitlilik siluetten) + R12/R16 UYARI + normal FunSim ≥%30 eklendi. **Sıradaki: PM_MESAJ_49** → `p21/?set=ajan2` → Zeyd oynar.
   - PM_49 bitti (8 Kasım): `p21/?set=ajan2&lang=tr` yayında. 06 kare tahta, 3 bomba, FunSim %92–94, süreden kayıp %3: GEÇTİ. 07 aday: FunSim %53 ama tuzaksız. **PM kararı:** 07'de FunSim ≥%50 ile tuzak birlikte sağlanamıyor. Rapor AJAN_DILIM1_RAPOR.md 'ajan2'.
   - **Sıradaki: PM_MESAJ_48** = ajan altyapısı (F3 talimat + F4 eleştirmen + kart JSON + `Tools/ajan/dongu.py`) ve ilk dikey dilim: bomba yayı 6–7 yeniden (p21'de ayrı set, p20/kök'e dokunma). Sonra: Zeyd oynar/onaylar → 3, 10, 14 tanıtımları → kilitli bölge prototipi (öncelik 1 yeni mekanik) → son 10 bölüme nefes.

