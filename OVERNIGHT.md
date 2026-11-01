# Gece raporu — PM_MESAJ_28 (2 Kasım): https://zeydusht.github.io/reverse-solver-unity/p20/?set=redesign — p19, kök, redesign.json, bölümler, iOS dokunulmadı.

- **0 (dosyada "0" bölümü yoktu; PM_27'nin kalanı olarak alındı):** Ayarlar anahtarları artık görünüyor (satır karosunun altında kalıyordu). Kaydırma yavaşlaması PM'in 0,135'i: fırlatma testi 1.712 pt/s → 807 pt / 2,2 sn (hedef ≥ 844 pt, sınırın hemen altı; betik CDP ile daha hızlı fırlatamıyor).
- **A eğitim — çalışıyor (yayında):** Bölüm 1: adım 1 parça 3 **sola**, adım 2 parça 6 **aşağı** (yön çözümden, test ediliyor: 8 yeni test), ~%60 örtü + aydınlık şerit (kenardaki parça için tahtanın dışına uzanır), el parçadan kenara sürükler, alt yazı "Parçayı sola sürükle" / "Drag the piece to the left"; yalnız hedef parça oynar, yanlış yön = sallanma, session'a gitmez (jam yok); sonra kural kartı (dokununca kapanır); süre eğitim bitene kadar 2:00'de durur. Engel spotlight'ları 3 çivi, 6 bomba, 10 zincir, 14 duvar (`Resources/tutorial.json`, metinler `Texts.cs` + `docs/i18n/en.md`). Çivi metni oyun koduna göre değişti: "Diğer parçalar çıktıkça çivi sayacı azalır." (PM'in "birkaç hamle sonra açılır" yerine; tanıtım kartı ise "her hamlende" diyor → PM karar). Ekran: `docs/qa/2026-11-02-ftue/05–08, 15, 38`. **Bulunan ve düzeltilen:** Codex bölüm 2'de eski (hep yukarı bakan) eli geri getirmişti — Zeyd'in "ikinci el yukarıyı gösteriyor" şikâyetinin kaynağı muhtemelen bu eski el; kaldırıldı (28 Ekim kararı: el yalnız bölüm 1).
- **B raporu:** `docs/FTUE_PERF_RAPOR.md`, 67 görüntü `docs/qa/2026-11-02-ftue/`. İlk 5 tıkanma: (1) kazanma kartında en üstteki "Geç" tuşu "Sonraki bölüm"ün üstünde, bölümü geçmek sanılır; (2) 1–10 kutuları ≈ 24 pt; (3) çivi kuralı iki farklı cümle; (4) Bölüm 4 "Level 4 · Jiggle'ları kurtar!" kartı amaçsız, İngilizce başlık; (5) kural kartı çiziminde kırmızı çarpı yok, oklar soluk.
- **Yükleme (CPU ×4, 15,4 MB):** Hızlı 4G 17,9 sn · Yavaş 4G 83,8 sn · 3G 175,6 sn; menüden bölüm 1 0,7–0,9 sn. FPS (PC) hepsi 60, koleksiyon kaydırmada tek 83 ms takılma. Bellek tepe JS 7 MB + wasm 114 MB. Uygulanan: başlık font atlası yarıya (−79 KB indirme). Öneriler raporda (menü sahnelerini sonradan indirme ~1 MB).
- **Veri — AÇIK SORUN (yüksek):** canlı `?c=ic`: Bölüm 1 ilk deneme kazanıldı, hamleler geldi ama `plays` satırı gelmedi (1–10 cevaplanmadan sayfa kapandı); deneme 2 geldi (`attempt 2`, `web-p20-2026-11-01b`, kohort/cihaz dolu). 27-B build'inde aynı test geçiyordu. Neden bulunamadı → pt1 davetlerinden önce çözülmeli.
- **Atlanan / 5 dk sınırı:** Unity build'leri ~6,5–7 dk (bölünemiyor; arka planda koşturuldu, 3 kez). İlk açılış turu 3 parçaya bölündü; 2. parça kayıptan sonra kesildi (kayıp sonrası 1–10 cevabı çekilmedi); "Bölümler'den tekrar oyna" betikte düğüm bulunamadı (doğrulanmadı). Sürüm etiketi hâlâ `p20-2026-11-01b` (değiştirilmedi).

# p20 menü revizyonu A+B yayında (PM_MESAJ_27, 2 Kasım): https://zeydusht.github.io/reverse-solver-unity/p20/?set=redesign — p19 (pt1), kök Build 5, iOS dokunulmadı. Sürüm `web-p20-2026-11-01b`.

- **A (yapıldı):** alt çubuk ekranın altına yapışık, ikonlar alt güvenli alanın üstünde; üst bar güvenli alan + 8 pt'den; Level tuşu çubuğa yakın; Ev tablosu boşalan alanla büyüyor (oran korunur). Font: **Nunito Black** (Türkçe tam; Baloo 2 ExtraBold de tam, Titan One'da İ yok, Fredoka'da ğ ş İ Ğ Ş yok → `docs/qa/font-karsilastirma.png`); başlık, Level, seçili sekme, sayaç, "X Jiggle kurtarıldı", kart tuşları krem→sarı degrade + koyu kontur + gölge. Jiggles: renk sayaçları kalktı, toplam büyük + ikon. Hata ayıklama yazısı yalnız `?debug=1` (QA çekimleri `&hud=0`). Kaydırma: atalet (son 80 ms hızı), uçlarda esneme (0,1), 10 pt altı dokunuş = tık, hareketli listeye dokunmak yalnız durdurur, fare tekerleği.
- **B (Task21, yapıldı):** alt çubuk v3 (yazısız ikonlar, yalnız seçili etiket, seçili yuva 2 kat geniş, 0,15 sn tween), sol üst kare avatar + isim plaketi + yeni sayaç hapı, Avatarım yeni halkalar + tik, kilometre taşları 3 madalyon + noktalı yol, Ayarlar krem satırlar + yan yana Gizlilik / Bize yaz.
- **Kontrol:** 64 görüntü `docs/qa/2026-11-01-p20b/` (3 boy, TR/EN, istisna 0): alt çubuk en altta, üst bar en üstte, hata ayıklama yazısı yok. Canlı `?c=ic` veri (IC_P20B, TEST_P20B): S41 deneme 1 ve 2, `web-p20-2026-11-01b` GEÇTİ. Testler 568/570.
- **Sorunlar (bir sonraki build'de, kod düzeltildi, yayında değil):** (1) Ayarlar'da anahtarların yeşil/gri zemini satır karosunun altında kalıyor, yalnız düğme görünüyor. (2) Fırlatma testi KALDI: 1.712 pt/s → 539 pt (1,5 sn), hedef ≥ 844 pt; yavaşlama PM'in 0,135'ine çekildi (bu hızda ~850 pt). İkisi için build 5 dk sınırını aştığı için durduruldu. İlk fırlatma koşusu da takıldı (süreç öldürüldü).
- Not: HANDOFF "sayaç sayıları lacivert konturSUZ" diyor (krem hap); sayaç bu yüzden lacivert düz yazı, A3'teki "sayaç = banner stili" yerine.

# p20 menü ekranları yayında (PM_MESAJ_26, 1 Kasım): https://zeydusht.github.io/reverse-solver-unity/p20/?set=redesign — p19 (pt1) dondurulmuş, dokunulmadı; kök Build 5, iOS, redesign.json dokunulmadı.
- Yapıldı (A+B birlikte): alt sekme çubuğu (Ev/Koleksiyon/Jiggles/Başarılar, rozet), Ev + Bölümler tuşu + uzun isim küçülür, Koleksiyon 3 sütun + tablo detayı (ad, "Bölüm N'de kazanıldı · süre", Tekrar oyna), Ayarlar (anahtarlar, dil, Gizlilik, Bize yaz → privacy sayfasındaki zeydusht@gmail.com, sürüm), Bölümler haritası (40 düğüm, zor = Baron, şu anki + avatar), Jiggles (renk rafı (slot−1) mod 5, Avatarım, 500/1.000/2.000), Başarılar 9 satır (canlı hesap), Profil kartı 6 istatistik. Yeni yerel istatistik deposu (PlayStats): süre/ilk deneme/sıkışma bu sürümden itibaren tutulur, eskisi için ilk deneme sayaçtan çıkarılır, süre geçmişi yok ("—"). Task20 görselleri sıkıştırıldı (data 6,1 → 9,4 MB Brotli).
- Doğrulama: 64 ekran görüntüsü docs/qa/2026-11-01-p20 (375/390/430, TR/EN, "Zeynep Yılmazoğlu", 0/12/40 bölüm; ?won=N ve ?player= yalnız debug) — istisna 0; 12 bölümde Jiggles 313 (60+64+69+70+50), başarı 2/9, koleksiyon 12/40; 0 → 0/40, 0/9; 40 → 40/40, 5/9 elle hesapla aynı. Canlı p20 ?c=ic (IC_REPLAY1): S41 deneme 1 ve 2 kayıtlı, app_version web-p20-2026-11-01. Testler 568/570 (2 atlanan), yerel 6/7 (card 7 pt eski). Doğrulanmadı: yavaş ağda koleksiyon küçük resim fade'i, iPhone, haritada dokunuşla tekrar oynama (kod yolu Koleksiyon'la aynı StartLevel).

# Koridor "Ana menü" yolunda da (PM_MESAJ_24B, 1 Kasım, p19): kazan → (1–10, 👍/👎) → Menü → aynı koridor (N → N+1) → 0,3 sn fade ile menü (N+1 tablosu, "Level N+1"); basan dokunuş atlatmaz (ilk 0,3 sn dokunuş yok sayılır); kayıp kartı ve 40 değişmedi. Canlı: 1→menü 3,62 sn, ikinci dokunuş 0,1 sn sonra atlatmadı, yeşil 0; ?c=ic (IC_KONTROL3) veri GEÇTİ; 15→menü betiğin 15'teki otomatik çözüm sorunu yüzünden denenemedi. Testler 558/560 (2 atlanan; FunSim süre testi bu koşuda geçti).

# Koridor geçişi p19'da (PM_MESAJ_24, 1 Kasım): https://zeydusht.github.io/reverse-solver-unity/p19/?set=redesign&c=pt1 — kazan → (1–10, 👍/👎) → Devam → koridor (N'nin tablosu → N+1'in tablosu, 3,6 sn, dokununca atlanır) → sonraki bölüm. Kök Build 5, iOS, bölümler ve redesign.json dokunulmadı.
- Seçim: video değil tek görsel şerit. Task 18 videosu saf yatay kayma (90. kare = 0. kare 431 px kaymış, fark ~2/255; kolaylaştırma smootherstep, takip dosyasıyla 4 basamak aynı), şerit `Resources/corridor_strip.png` (`Tools/make_corridor_strip.py`, data +242 KB; web videosu 0,70 MB ek indirme olurdu, WebGL/iOS video riski yok). Tablolar 4 köşeye homografiyle (8×8 ağ), %1,2 büyük; yeşil şeritte koyuya boyandı (geç kalan tablo yerine koyu yer tutucu, inince 0,25 sn fade). Yatay pencerede dikey kırpma ≤ %23, yanlar koyu. 40'ta Devam yok → bitiş kartı aynı (koridor oynamaz). Eski shader işi (menüde, dokuları eksikti, batch testi kalıyordu) build dışına `docs/art/drafts/corridor-shader-v1-kenarda/`'ya alındı, silinmedi.
- Doğrulama: `verify_build.js --only corridor` 375×667/390×844/900×640/320×720: 1→2 ve 39→40 yeşil 0, son karede N+1 tablosu, sonraki bölüm açıldı, istisna 0; 15→16 390×844'te aynı (betiğin otomatik çözümü 15'te çoğu koşuda tutmuyor, --rlv'de tutuyor — oyunla ilgisiz); 40 bitiş kartı; 1,5 MB/s yavaş ağda sonraki tablo kazanmadan önce inmiş ("hazır"). Yerel standart 6/7 (card 7 pt, eskiden beri). Canlı ?c=ic (IC_KONTROL2): S41 win/S50 bomb/S42 quit, cohort/platform/device_id/app_version dolu, GEÇTİ. Testler 557/560 (FunSim 2,02 sn eskiden beri). iPhone'da denenmedi; geç tablo yer tutucusu yalnız kodla, otomatik test edilmedi.

# Veri sistemi Faz 2 hazır (PM_MESAJ_25, 1 Kasım): docs/veri/tahmin_v1.csv, botlar_v1.json, level_sim_v4.csv (PM Supabase'e yükleyecek); tek komut `python Tools/veri/dongu.py --cohort pt1 --run v2` (VERI_SISTEMI 11).
- Veri: 164 deneme yeniden oynatıldı (4151 seçim anı), hepsi Zeyd'in (ddd, deneme1, Zeyd Testt, Zeyd Denimi tek oyuncu). Bot: seçimde en güçlü etken yakınlık (son parçaya uzak aday −9,2), sonra doğru hamle +1,65, bomba +1,07, zincir +1,17; süre medyanı ~0,7 sn/hamle, jam 0,16/hamle. Nüfus yayılımı VARSAYIM (tek oyuncu). Oyuncu-dışarıda-bırak yapılamadı (tek oyuncu).
- Geri test (bölüm dışarıda, 91 ilk deneme, 80 kazanma): v1 Brier 0,208 — sabit ortalama 0,109 ve fitil tabanı 0,095'i GEÇEMİYOR (botlar fazla kötümser: Zeyd uzman, nüfus varsayımı daha zayıf oyuncular da çekiyor). Sıralamada iyi: Zeyd'in 1–10 puanıyla ρ 0,86 (v0 0,88). Öneri: pt1'e v0 ile girilsin, v1 yanında dondurulursa yalnız sıralama için. Not: FunSimV5 süre testi 2,02 sn (sınır 2,0) ile kalıyor, FunSim değişmedi.

# Veri sistemi Faz 1 yayında (PM_MESAJ_25, 1 Kasım): pt1 linki https://zeydusht.github.io/reverse-solver-unity/p19/?set=redesign&c=pt1 (Türkçe: &lang=tr). Kök Build 5 ve iOS dokunulmadı.
- Yapıldı: ?c= kohortu cihazda kalıyor; plays'e cohort/platform/device_id/app_version (web-p19-2026-11-01), moves'a cohort/device_id; moves'ta start (move_no 0), hide/show (eski pause/resume yerine), quit (menüye dönüş), her 10 satırda + gizlenince gönderim; jam satırında denenen yön; kohortlu SET40'ta 1–10 bölüm başına bir kez (kazanç ve kayıp, "Geç" tuşu), kohortsuz SET40'ta artık sorulmuyor (ZeydBot seti eskisi gibi); kohort başına bir kez profil sorusu (Hiç/Ara sıra/Sık → player_profiles). Testler 555/555, yerel verify 9/10 (kart 7 pt, eskiden beri).
- Canlı ?c=ic kontrolü (IC_KONTROL, masaüstü Chrome): v_attempts S41 win, S50 bomb, S42 quit — hepsi cohort=ic, platform=desktop, device_id dolu; moves: S41#0 start … exit/L/D/U/R, S42#13 quit, S43#0 start, #1–2 exit, #3 hide (sekme kapandı, plays satırı yok); player_profiles ic/IC_KONTROL/desktop/puzzle_exp 0; v_level_actual cohort=ic: S41 win1 1 (n 1), S42 other1 1, S50 v2 fail_bomb1 1. zorluk_puani boş: otomatik dokunuş "Geç"e geldi (1–10 çip yolu Z03'te zorluk_puani 1 ile geçti). v_abandoned (02:33): ic / IC_KONTROL / S43 v1 / deneme 1 / last_move_no 3 — bırakılan deneme görünüyor.

# SET40 v4 önizlemede (PM_MESAJ_23, 31 Ekim): https://zeydusht.github.io/reverse-solver-unity/p19/?set=redesign (aynı link; kök Build 5, /levels/redesign.json ve iOS zip'e dokunulmadı)
- Ayarlanan 9 bölüm, tahmini Zeyd puanı (FunSim ilk deneme): 8 S47 3,5 (%47) · 9 S20 3,1 (%65) · 12 S51 4,2 (%37) · 17 S18 4,7 (%24) · 26 S17 3,9 (%41) · 27 S10 3,8 (%38) · 28 S50 3,6 (%48) · 29 S52 3,5 (%51) · 31 S08 3,8 (%39). Hepsi SmartPlayer temiz, benzerlik en yüksek %57; açılış 40/40 yerel + canlı; 8, 17, 31 canlıda kayıtlı çözümle kazanıldı. Tablo: docs/SET40_V4.md.
- Sorunlar: ZeydBot v1 bu bölümleri ayırt edemedi, tahmin FunSim kalibrasyonundan (±1,4). S51'de fitil tek başına işlemedi, +1 bomba eklendi; S20 fitili 7 (PM 10 demişti, 10'da 2 puan kalıyordu). Zor uyarısı yalnız 19/23/25/30/35/36/38'de (39 ve 40'tan kalktı). verify `card` KALDI (D01 kartında başlık–çerçeve 7 pt < 8) — önceki p19'da da KALDI (0 pt), set işiyle ilgisiz. Koridor testi batchmode'da kalıyor (commitlenmemiş koridor işi; build'e girmedi). iOS uygulaması v4'ü ancak /levels/redesign.json güncellenince alır — Zeyd'in kararı.

# Zincir kilidi hücre ortasında + iOS 0.5.1 (4) (PM_MESAJ_22, 31 Ekim): önizleme https://zeydusht.github.io/reverse-solver-unity/p19/?set=redesign
- Kilidin yuvarlak gövdesi hücre merkezinde, hücrenin %45'i genişliğinde; zincir merkezden merkeze gövdeye giriyor. 40, 24, SHOW1 üç boyutta kontrol edildi (yatay + çapraz; bu bölümlerde altın ve turkuaz çift var, mor/mercan aynı kodla çiziliyor). Açılış 40/40, canlı adreste de geçti. Kök Build 5'e dokunulmadı.
- iOS: Builds/ReverseSolver-iOS-0.5.1.zip — 31 Ekim 2026 19:30, 305 MB, 0.5.1 (4), Xcode build 0 hata. Commitlenmemiş koridor kazanma animasyonu işi build'e girmedi (build süresince kenara alındı, sonra geri kondu).

# Masaüstü 9:16 alan yayında (PM_MESAJ_21, 31 Ekim): https://zeydusht.github.io/reverse-solver-unity/p19/?set=redesign (ayrıca /br/, /gz/, /p18/; yalnız index.html, kök yönlendirme ve oyun dosyaları aynı).
- Sonuç: masaüstü Chrome'da (gerçek GPU, fare) 1280×800, 1000×1000, 700×900 ve açıkken 1280×800→900×700 yeniden boyutlandırmada oyun ortada 9:16, yanlar #0B2A36, çizim çözünürlüğü = alan (bulanık değil); Level 1 her boyutta fareyle sürüklenerek çözüldü, tıklama kayması yok. 420 px dar pencerede ve telefonda blok devreye girmiyor (SHOW1 telefon kontrolü geçti).

# iOS 0.5.1 (3) HAZIR — 29 Ekim 15:31: Builds/ReverseSolver-iOS-0.5.1.zip (305 MB). Zeyd sabah: Mac'e götür, docs/MAC_YUKLEME.md → "Sonraki yüklemeler" (3, 5, 6. adımlar).
- İçinde: 40 tablo, PM_19'un tamamı (English varsayılan + Türkçe; Ayarlar: ses/müzik/titreşim/dil; kart tuşları; isim girişi; duvar), tek tutorial eli, ilerleme kaydı + Supabase'ten geri getirme, arka plan müziği C (Vorbis, yalnız iOS'ta), renkli zincirler (Task17), titreşim (UIImpactFeedbackGenerator).
- Xcode projesi kontrol edildi: com.zeydtekin.reversesolver, 0.5.1 (3), iOS 15+, ITSAppUsesNonExemptEncryption=false, 0 hata. Bölümler Pages'teki onaylı SET40 v3'ten iner (Pages dosyası yereldeki v3 ile birebir aynı; v4 yayında değil).
- Renkli zincirler önizlemede de yayında (/p19/?set=redesign): her bölümde çiftler sırayla altın, turkuaz, mor, mercan; canlıda doğrulandı.

# Zincir görünürlüğü (PM_MESAJ_20) yayında: /p19/?set=redesign. Zincir 2 kat kalın, lacivert, krem kenarlı, gölgeli; parça < zincir < engel < sayaç; canlıda doğrulandı.
# SET40 v4 önerisi hazır (yayında DEĞİL): docs/SET40_V4.md. 7 👎 bölüm çıkıyor, 8–18 3→5'e çıkıyor, 23 uyarılı, 37 → Z25, 39↔40. Zeyd onayı bekleniyor.

# Arka plan müziği (music-c) önizlemede: /p19/?set=redesign. Web'de build'e girmiyor; Pages /music/music.mp3 (0,92 MB, 128 kbps) ilk dokunuşta akışla başlıyor, 57,6 sn boşluksuz döngü (MP3 baş sessizliği atlanıyor), %35 seviye, kazanınca 2 sn kısılıyor, Ayarlar → Müzik aç/kapat. Canlıda doğrulandı (verify music). iOS: Vorbis ogg yalnız iOS build'ine giriyor; zip üretilmedi, cihazda denenmedi.

# Önizleme 19 güncellendi (28 Ekim akşam, aynı link /p19/?set=redesign; kök ve iOS dokunulmadı): tek el (Codex eldiveni) yalnız Level 1'in ilk 2 hamlesinde, Level 2'de el yok; duvar köşeleri tek dirsek parçasıyla birleşiyor (19/25/35/40, SHOW1 kontrol; sağ alt köşe hiçbir bölümde yok).
- İlerleme: her bitişte hemen yazılıyor, bölüm kimliğiyle tutuluyor (sürüm/set değişince silinmiyor), kök ile /br/?set=redesign aynı kaydı kullanıyor; önizleme klasörleri (/p19/) ayrı kayıt. Yedek: tarayıcı kaydı boşsa ve aynı isim girilirse kazanılan bölümler Supabase plays'ten okunup geri geliyor (şema aynı; TEST_B5 ile canlıda doğrulandı → Level 2).
- Tarayıcılar: Safari, Chrome (iOS) ve WhatsApp içi tarayıcının kayıtları birbirinden ayrı; Safari 7 gün girilmeyen sitenin kaydını silebilir. Üçünde de aynı ismi yazınca ilerleme Supabase'ten gelir. Not: isim parolasız; başkasının ismini yazan onun ilerlemesini görür.

# Önizleme 19 (PM_MESAJ_19, 28 Ekim): https://zeydusht.github.io/reverse-solver-unity/p19/?set=redesign (Türkçe: &lang=tr) — kök Build 5 değişmedi.
- iOS 0.5.1 (2) hazır: Builds/ReverseSolver-iOS-0.5.1.zip (304 MB, Xcode 0 hata); 40 tablo + bu tur + titreşim. Mac adımları: docs/MAC_YUKLEME.md → "Sonraki yüklemeler".
- Yapıldı: parça sesi kaldırıldı (takılma + son parça + fanfar kaldı); Ayarlar: Ses, Titreşim, Dil; varsayılan English; kısa kazanma kartı (👍/👎 çizim, 1–10 çip, yeşil Next level, Play again · Menu); isim girişi krem/lacivert, 12 karakter, kutuya sığar; duvar çerçevenin yerini alıyor.
- QA: yerel 5/5 + canlı 2/2; 2 dil × 3 boyut ekran görüntüleri docs/qa/2026-10-28-p19 (menü, isim, oyun, kazanma, ayarlar).
- Kalan: Müzik ayarı yok (oyunda müzik yok); dil ayarı Supabase'e yazılmıyor (şemaya dokunulmadı); titreşim ve iOS isim klavyesi gerçek iPhone'da denenmedi; kazanma resmi kenar çizgilenmesi.
- 1–10 çip satırı yalnız oyuncu modunda (debug'sız) çıkıyor; ekran görüntüsünde yok, telefonda bak.

# Önizleme 18 (PM_MESAJ_18, 28 Ekim): her bölüm kendi tablosuyla — https://zeydusht.github.io/reverse-solver-unity/p18/?set=redesign
- Bölüm N → painting-NN (40 tablo). Resimler build'de değil, GitHub Pages /paintings/'ten iniyor; sıradaki bölümün resmi önceden yükleniyor, web'de tarayıcı önbelleği, iOS'ta diske kopya. Resim gelmezse tahta düz mavi açılıyor, oyun beklemiyor.
- Ana menüdeki büyük altın çerçevede sıradaki bölümün tablosu var (çerçeve aynı). Kazanınca kart açılmadan önce tamamlanmış resim 1 sn görünüyor.
- Build boyutu değişmedi (Brotli 11,43 MB → 11,44 MB, kod farkı). Kök adres (Build 5) DEĞİŞMEDİ; önizleme ayrı klasörde (/p18/). Kök için Zeyd'in "kökü güncelle" onayı bekleniyor.
- QA (3 boyut: 375×667, 390×844, 430×932; yerel + canlı, istisna 0): menü, bölüm 1, 2, 19, 30, 40 ve kazanma anı → docs/qa/2026-10-28-paint40(-live). Kenarlar, bomba/çivi sayaçları, zincirler ve kırmızı-beyaz duvar okunuyor; güçlendirme gerekmedi.
- Bilinen küçük kusur: kazanma anındaki tam resmin sağ ve alt kenarında ince çizgilenme var (kenar şeridinde). Sonraki işte düzeltilecek.
- iOS 0.5.1'e girecek (kod ortak, titreşim işiyle birlikte).

# Yeni sesler yayında (Zeyd C'yi seçti): her parçada C'nin kısa hali (0,4 sn), son parçada uzun hali, ardından kazanma "dı-dı-dııı" fanfarı. Dinle: https://zeydusht.github.io/reverse-solver-unity/sounds/ (üstteki 3 tuş). Testler 552/552, canlı kontrol 2/2.

# SHOW1 vitrin bölümü (Görkem Bey için): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign&painting=1&lv=SHOW1
- 6×6 Akdeniz resimli tahta; 1 bomba (fitil 24), 1 çivi (11), 1 uzak zincir (2 kilit), 2 duvar şeridi; süre 2:30. Link önce ana menüyü açar (isim girişi yok, isim "SHOW"), büyük tuş "Demo" yazar ve SHOW1'i başlatır. 4 booster açık, her biri 1 adet. Kazanınca "Tebrikler! / Bu bir demo bölüm." kartında "Tekrar oyna" ve "Ana menü", kayıtlar level_id SHOW1 + oyuncu SHOW.
- Doğrulama (fabrika üreticisi + bot): SmartPlayer kazanır, supap 0; FunSim ilk deneme %89, Monte Carlo (rastgele) %63; fitil 10→24 taraması: %20/%32/%43/%54/%64/%74/%89.
- Metrikler: kazma 4, fitilPay 20, yem 0,3, sahteSerbest 1, m1Açılış 10, m2Tavan 11, m3Bağlı3 0,11, m3TekBağlı 7, m6ÇiviPay 22, m7Çatışma 0, m8DuvarKolay 1 (tam liste Logs/batch/ShowLevel.Tune-*.log).
- Anahtar parça: SmartPlayer çözümünde zincirli çift bombadan önce çıkıyor. QA: 3 telefon boyutu (yeni akış: docs/qa/2026-10-27-show2 — menü, booster'lar, iki tuşlu kart doğrulandı; build verify 6/7, kalan card bilinen ölçüm hatası).

# iOS 0.5.0 HAZIR (27 Ekim gecesi): Builds/ReverseSolver-iOS-0.5.0.zip (304 MB)
1. Zip'i Mac'e aktar (USB / Google Drive / AirDrop), docs/MAC_YUKLEME.md'yi adım adım izle (Xcode en güncel, Unity-iPhone.xcodeproj, Signing → Team, Product → Archive → Distribute → App Store Connect).
2. Kontrol edildi: bundle com.zeydtekin.reversesolver, 0.5.0 (1), iOS 15+, yalnız iPhone, dikey, otomatik imza, şifreleme beyanı (Missing Compliance çıkmaz), Mac IL2CPP araçları zip'te çalıştırılabilir işaretli.
3. Gizlilik sayfası (iletişim zeydusht@gmail.com): https://zeydusht.github.io/reverse-solver-unity/privacy/ — App Store Connect'te "Privacy Policy URL" olarak gir.
4. Web Build 5 kökte yayında; iOS kayıtları Supabase'e "unity-ios" olarak düşer (008 sorgusu ikisini de sayar).
5. Gerçek iPhone'da denenmeyenler (ilk TestFlight'ta bak): isim girişi (iOS klavyesi), çentik/alt çubuk boşlukları, ses.
6. Not: Unity projesinin aktif hedefi şimdi iOS; bir sonraki web build'i ilk seferde biraz uzun sürer.

# Build 5 YAYINDA (27 Ekim): https://zeydusht.github.io/reverse-solver-unity/ — kök SET40 v3, eski 40 ?set=classic, 👍/👎 + kayıt açık. Canlı verify 6/7 (card = bilinen debug ölçümü). Gizlilik: /privacy/ (iletişim adresi yer tutucu).
- iOS 0.5.0 hazırlığı: NativeHost (güvenli alan, iOS klavyesiyle isim, unity-ios kaydı), bölümler GitHub Pages'ten önbelleğe, IosBuild (bundle com.zeydtekin.reversesolver, iOS 15+, dikey, otomatik imza). iOS build iOS Build Support kurulunca alınacak.

# Zeyd: Unity Hub'dan iOS Build Support'u elle kur (Installs → 6000.6.4f1 → ⚙ → Add modules → iOS Build Support). Komut satırından denendi, yönetici izni penceresi onaylanmadığı için kurulamadı; iOS build (madde 5) bunu bekliyor.

# Önizleme (kart tuşları): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign — Task15 tuşları orijinal oranında (germe yok), krem yazı + koyu kontur + gölge, basılı hâl; başlangıç kartında ikonlar 2×

# Önizleme (menü düzeltmesi): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign — Levels tuşu yok, Level tuşu ortada, avatar kutusunda turkuaz radyal zemin + ortalı Jiggle (seçim kartında da)

# Önizleme 17: resimli level kalın koyu lacivert kenarla (Zeyd seçti) https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign&painting=1&lv=S46

# Kenar karşılaştırması (önizleme 16): şimdiki https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign&painting=1&lv=S46 · koyu lacivert kalın https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign&painting=1&lv=S46&edge=navy · siyah kalın https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign&painting=1&lv=S46&edge=dark

# Önizleme 15 (26 Ekim 20:55): resimli level https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign&painting=1&lv=S46
- Parça kenarı: lacivert dış kontur, içte krem ışık şeridi (%60), hafif bevel; sürüklenen parçanın altında aşağı-sağa yumuşak gölge. Yalnız painting=1; düz mavi set aynı. 3 telefon boyutunda kontrol edildi (docs/qa/2026-10-26-p15).

# Önizleme 14 (26 Ekim 19:45): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign
- Resimli yapboz testi (Level 7): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign&painting=1&lv=S46
- Codex Task15: kırmızı rozet, duvar v3, uzun HUD hapları + ikonlar, kısa/uzun kart tuşları, 5 avatar, Jiggles sayacı. Çerçeve ve booster çubuğu eski hâliyle kaldı (Zeyd'in notu).

# Önizleme 12 (26 Ekim 16:40): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign
- Yeni menü (Task13: profil, yıldız, Jiggles rescued, Level + Levels) ve yeni oyun içi set (Task14: HUD, booster tuşları, çerçeve, engeller, duvar, kartlar).
- QA 3 telefon boyutu 8/8, taşma yok; Lilita'da ğ/ş/İ olmadığı için Türkçe metinler Fredoka (+RS Sans yedeği).
- Build 5 yayınlanmadı; kodda Build 5 hâli duruyor, yayın öncesi yeniden alınacak.

# Gece turu (PM_MESAJ_14, 26 Ekim sabahı)
1. Son önizleme 11: https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign (Task10 booster, Task11 HUD/kart/menü, kodla kırmızı-beyaz duvar). Kök değişmedi.
2. QA: 40 bölüm kayıtlı çözümle kazanılıyor, slotlar doğru (tutorial 1–2, tanıtım 3/6/10/14, zor 19/25/30/35–40, booster 5/9/15/23). 3 boyutta 24 ekran: docs/qa/2026-10-26/.
3. Bulunan 7 sorun, 6'sı düzeldi: kart tuşları alt kenara değiyordu; kayıpta çift "tekrar" tuşu; listede sıradaki bölüm okunmuyordu; kazanmada 1–10 kayda girmiyordu (👍 satırı erken gönderiyordu); 008 sorgusu yanlış kimlik (R→S); 2048 üstü atlas + kullanılmayan 64 görsel. Kalan: kazanma görselinde ince çizgi (Codex görev 7).
4. Telemetri (TEST_NIGHT, Build 5 adayı): plays + moves 9/9, 👍 ve 1–10 kazanmada, 1–10 kayıpta, pause→Ana menü = quit ✓. Eski a6b testi kökte L02 beklediği için artık uymuyor (ürün hatası değil).
5. Build 5 adayı hazır, YAYINLANMADI: Builds/build5-candidate (kök yeni set, ?set=classic eski, 👍/👎 açık). Yerel verify 6/7 (card = bilinen debug ölçüm hatası). Boyut 9,7 MB (br), PC'de 60 FPS; iPhone FPS ölçülemedi.
6. Not: kodda bayraklar artık AÇIK; bir sonraki gh-pages yüklemesi kökü değiştirir → yalnız "Build 5'i yayınla" ile.
7. Zeyd sabah: 1) önizleme 11'i oyna, 2) "Build 5'i yayınla" kararı, 3) iPhone + 4G testi, 4) CODEX_GOREV 7'yi Codex'e ilet.

# Önizleme 10 (26 Ekim): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign&lv=S40 — Codex Task7 (v2 bomba/çivi/duvar, altın çerçeve, HUD seti); verify 7/7, kök değişmedi.

# Önizleme 9 (25 Ekim): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign&lv=S40 — ortalanmış büyük bomba, Fredoka sayaçlar, kalın duvar, altın çerçeve; verify 7/7, kök değişmedi.

# Önizleme 8 (25 Ekim): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign · &lv=S28 — Codex Task6 çivi, bomba, zincir, duvar (+köşe) tahtada; verify 7/7, kök değişmedi.

# Önizleme 7 (25 Ekim): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign — parçalar Codex Kit81 ile (mask/overlay/flat, 81 kenar kombinasyonu); prosedürel çizim yedek. Verify 7/7, kök değişmedi.

# Önizleme 6 (25 Ekim): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign · zincir+duvar: &lv=S28
- Büyük engel + üstünde sayaç, zincir engelin altında, pause kartı, kırmızı-beyaz duvar. Verify 7/7; kök değişmedi.

# Önizleme 5 (25 Ekim 13:10): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign · ilk zor: &lv=S28
- Codex Görev 6 + kalan bağlantılar: zincir, duvar, booster halleri (kilitli/aktif/tükendi/rozet), bölüm hücreleri, makas noktası, parça gövde dokusu, yükleme ve isim ekranı.
- Verify yerel 7/7; kök ve canlı 40 değişmedi. Duvar şeridi ince/gri, okunurluğu Zeyd baksın.

# Önizleme 4 (gh-pages ecc6541, 25 Ekim 02:30) — yalnızca ?set=redesign; kök ve canlı 40 değişmedi
- Bitti: PM_MESAJ_8 (sade menü + "Level X" tuşu, tek el, yükleme/isim ekranı) ve PM_MESAJ_9 (profil kutusu "Kurtarılan Jiggles", ayarlar/ses, bölüm başlangıç kartı, yıldız + konfeti).
- Codex CLI ile Görev 4 (parça skin'i) ve 5/5b (89 PNG) yaptırıldı, hepsi bağlandı: üst bar, booster yuvaları, kartlar, düğmeler, tahta, sayaçlar, oyun arka planı.
- Link: https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign (menü) · ilk zor + başlangıç kartı: https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign&lv=S28
- Zeyd onaylasın: Codex'in yapboz parça stili (koyu petrol kenar; inceleme: Assets/Art/PieceSkin/review.html), krem kart zemini, Level X tuşu.
- Kalan Claude çizimi ~8: zincir ve duvarın tahtadaki çizimi, booster kilitli/tükendi halleri, bölüm listesi hücreleri, bomba sıcak parlaması, parça dokusu (yalnız kenar rengi uygulandı).
- Verify yerel 7/7; boyut 8,94 MB (oyun arka planı 9 MB → 98 KB küçültüldü). Build 5 kök yayını Zeyd'in "tamam"ını bekliyor.

# Önizleme 2 — Codex görselleri (gh-pages 76ae1dc, 24 Ekim 20:30), yalnızca ?set=redesign; kök ve canlı 40 değişmedi
- Engel + booster ikonları, A seçenekleri (düğme, el, tanıtım/zor/kazanma/kaybetme görselleri, kart çerçevesi, tahta altlığı, sesler). A/B tek yerden: `Tools/art_choice.py` → build.
- Aynı linkler (aşağıda). Telefon boyutunda (375×667) bomba sayacı ve booster ikonları okunuyor; çivi kartında artık çekiç değil çivi ikonu.
- Verify: yerel 7/7 (ilk koşu); sonra makinede FPS 30'a sabitlendi (yerel ve canlı aynı, eski set de) → makine durumu, build değil. Boyut 7,38 → 8,46 MB.
- Bağlanmayan: kayıp "kilit" görseli (oyunda o kayıp türü yok), zincir/duvar tahtada çizim kaldı.

# Önizleme — Build 5-önizleme (gh-pages dd11b14, 24 Ekim 18:25): SET40 v3 yalnızca ?set=redesign; kök ve canlı 40 değişmedi
- Ana menü (koridor + mavi ikon): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign
- Tutorial 1: https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign&lv=S41 · Çivi (3): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign&lv=S43 · Bomba (6): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign&lv=S45
- Zincir (10): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign&lv=S48 · Duvar (14): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign&lv=S52
- İlk zor (19, uyarı ekranı): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign&lv=S28 · Final (40): https://zeydusht.github.io/reverse-solver-unity/br/?set=redesign&lv=S40
- Verify: yerel ve canlı 7/7. Not: "card" ölçümü 👍/👎 açıkken (Build 5 bayrakları) yanlış KALDI veriyor; debug yazısı resim çerçevesine biniyor, kart sığıyor. Ölçüm düzeltilmeli.

# Gece raporu — 24 Ekim 2026 (PM_MESAJ_7)

1. **SET40 v3 hazır, Build 5 yerelde (yayınlanmadı).** Varsayılan set v3, eski 40 `?set=classic`, 👍/👎 açık. Zeyd "Build 5'i yayınla" derse kök yayınlanır.
2. **Zeyd'in test listesi:** 1–2 (tutorial eli), 3, 6, 10, 14 (tanıtım kartları), 19 (zor uyarısı + ilk zor), 40 (final), ana menü arka planı ve ikon. Telefonda oynamak için önce yayın kararı gerekiyor (gh-pages'e gönderilmedi).
3. Yeni 12 bölüm (1–3, 5–8, 10–14) fabrikanın üreticisiyle yapıldı, ölçülmedi; 4, 9, 15–40 v2 havuzundan. S39 çıktı, 37 nefes. Tablo: `docs/analiz/zeydbot/SET40.md`.
4. Booster açılışları değişmedi (5/9/15/23); tanıtımlarla (3/6/10/14) çakışma yok.
5. Testler 551/554; kalan 1 hız testi (L40 FunSim 3,4 sn > 2 sn) Codex'in yalnızca yorum içeren hâlinde de kalıyor, kodla ilgisiz.
6. verify_build 6/7: k1 ve fps artık `?set=classic` ile ölçülüyor. "card" kalan ölçüm hatası (debug yazısı resim çerçevesine biniyor, kart 660 ≤ 667 pt sığıyor).
7. Codex: yorumlar commit'lendi (e640763); yeni görevler `docs/CODEX_GOREV.md`; ASSET_IHTIYAC koda göre dolduruldu.
