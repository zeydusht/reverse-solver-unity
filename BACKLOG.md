# Reverse Solver — BACKLOG

Son güncelleme: 12 Kasım 2026 (Codex)

Kurallar:
- PM yalnız "PM" bölümünü, Claude Code yalnız "Mühendislik" bölümünü günceller.
- Her güncellemede en üstteki "Son güncelleme" satırı da değişir.
- Durum etiketleri: ⏳ sürüyor · 🔜 sırada · ✅ bitti · ⛔ beklemede

---

## ŞİMDİ

| İş | Kim | Durum | Tahmini süre |
|---|---|---|---|
| SET40 v3 (öğretici eğri + tutorial + Build 5 hazırlığı) — PM_MESAJ_6 | Claude Code | 🔜 gece | ~1 gece |

## Codex devir sırası

| İş | Branch | Durum ve kontrol |
|---|---|---|
| 1 · Onboarding | `codex/01-onboarding` | ✅ 2×2 rehber, engelsiz 3×3 ve ilk 10’a süre payı. EditMode 699 geçti. Play Mode’da Seviye 1’in iki takılmasını ve Seviye 2’nin serbest sırasını dene. Telefon turu açık. |
| 2 · Fiske ve güvenli kenar | `codex/02-controls` | ✅ Yön kilidi çapraz kaymada korunuyor, yatay cihaz girintileri hesaba katılıyor. Unity 18/18. Telefonda kısa fiske ve Safari kenar hareketi kontrolü açık. |
| 3 · Engel tanıtımı | `codex/03-obstacle-lessons` | ✅ 3/10/14/20 tek engelli, kayıpsız dersler. Gerçek sayaç metinleri. Unity 22/22. FunSim 3–10, 96 sanal oyuncuda %100 ilk deneme. Telefon turu açık. |
| 4 · Çıkış hissi | `codex/04-piece-feedback` | ✅ Kenarda pop, yarı yol övgüsü ve mevcut özgün «weee». Derleme ve 18 hareket testi geçti. Play Mode’da 3×3’ü yarıla, sesli ve sessiz dene. Telefon miksajı açık. |
| 5 · Booster gösterimleri | `codex/05-booster-animations` | ✅ Yerel Redesign’da dört booster, açık değnek bölgeleri, Jigo’da fiziksel saat ve 10 sn bar. Unity 17/17. 5/9/15/23 cihaz turu açık. |
| 6 · HUD | | 🔜 Ölçü, çerçeve, beş yuva, Jigo ve devam fiyatları. |
| 7 · Jigo çizimi | | 🔜 Ön, yan ve arka, üç HUD ifadesi. |
| 8 · Kont çizimi | | 🔜 Karakter tasarımı. |
| 9 · Açılış storyboard’u | | 🔜 Kareler ve çekim açıklamaları. |
| 10 · HUD ve booster çizimleri | | 🔜 İkon şekilleri, yeniden renklendirme ve mühür. |

## ZEYD — senin işlerin

| İş | Ne zaman | Süre |
|---|---|---|
| 40'lık setten 5 zirve + 5 rastgele bölümü oyna; her bölüm için 1–10 puan + "aynı bölüm gibi mi?" | Set hazır olunca | ~25 dk |
| 40'lık seti onay kuyruğunda onayla | Yukarıdaki maddeden sonra | ~1 sa |
| CSV Excel kontrolü (36 sütun) | Boş vaktinde | 5 dk |
| iPhone telefon testi (docs/TELEFON_TESTI.md) | Build 5'ten önce | ~25 dk |
| Seviye editörü kabul turu + Zorluk paneli turu | Boş vaktinde | ~30 dk |
| GitHub Actions'taki eski re-run'ı iptal et (0552040) | Boş vaktinde | 2 dk |
| Desktop'taki saatlik gece görevini durdur | Boş vaktinde | 1 dk |
| Playtest daveti: her davetli 1–2 kişiye iletsin mi, ek bir grup var mı? (cevap bekliyor) | Playtest'ten önce | 1 dk |
| 4G kontrolü | Playtest'ten önce | 10 dk |

## PM

| İş | Durum |
|---|---|
| Oturum 5 kararı: yöntem kabul, 40'lık set onaylandı (M16 çeşitlilik eklendi) | ✅ |
| SET40 v1 → v2: zirveler yenilendi; v2 ONAYLANDI (zirveler 8–10, "en keyifli tur"); SET40 v3 öğretici eğri (Zeyd'in slot planı) + tutorial/uyarı ekranları + Build 5 hazırlığı (PM_MESAJ_6) | ✅ 21 Ekim 23:00 |
| Build 5 yayını: Zeyd'in "Build 5'i yayınla" komutunu bekliyor; sonra playtest (~20 kişi) | 🔜 |
| Görsel/ses üretimi Art Director'e (ChatGPT/Codex) devredildi: ART_DIRECTOR_BRIEF.md | ✅ 23 Ekim |
| App Store / iOS | ⛔ ertelendi |

## Mühendislik (Claude Code doldurur)

| İş | Durum | Tahmini süre |
|---|---|---|
| Codex yorumları kontrol + commit (e640763; kod aynı, yalnızca yorum + yeni testler) | ✅ | |
| SET40 v3: 12 yeni bölüm + v2 havuzu yeni eğriye dizildi (574a9ce) | ✅ | |
| Tutorial eli (1–2), tanıtım kartında N, zor bölüm uyarısı, metinler Texts.cs | ✅ kod, build bekliyor | |
| İkon + menü koridor arka planı | ✅ kod, görünüm doğrulanacak | |
| Önizleme ?set=redesign yayında (gh-pages dd11b14, canlı verify 7/7); Build 5 bayrakları kapalı, kök yayını Zeyd komutuyla | ✅ | |
| ASSET_IHTIYAC doğrulandı; CODEX_GOREV.md (3 görev) | ✅ | |
| HardestLevelStillUnderTwoSeconds: 3,4 sn (sınır 2) — makine yükü, kodla ilgisiz; izlenecek | ⛔ | |
| Önizleme 2 yayında (gh-pages 76ae1dc). Codex asset'leri bağlandı (yalnızca ?set=redesign): #10 engel + #11 booster ikonları, A seçenekleri #3 #4 #5 #6 #7 #8 #9 #13 #14; seçim tek yerden `Tools/art_choice.py` | ✅ | |
| Kayıp "kilit" görseli bağlanmadı: oyunda bu kayıp türü yok (yalnızca bomba / süre) | ⛔ | |
| Zincir ve duvar tahtada hâlâ çizim (ikonlar kartlarda); #12, #15–19 playtest sonrası | ⛔ | |
| verify fps: makinede 30 FPS sınırı (24 Ekim akşam, build bağımsız) — tekrar ölçülecek | ⛔ | |
| Önizleme 4 (ecc6541): Codex Görev 4–5 bağlandı; profil, ayarlar, başlangıç kartı, yıldız/konfeti (PM_MESAJ_8–9) | ✅ | |
| Kalan Claude çizimi ~8 (zincir, duvar, booster halleri, bölüm listesi, parça dokusu) → sıradaki Codex/bağlama turu | 🔜 | ~2 sa |
| Codex CLI çalışıyor (`codex exec -s workspace-write`); görevleri ben veriyorum | ✅ | |
| Önizleme 5 düzeltme: parçalar hâlâ prosedürel; zincir/duvar beklenen gibi değil → Codex parça seti (81, `piece_O-F-I-F`) ve yeni engel/pause görselleri gelince bağlanacak | 🔜 | |
| Önizleme 6: engeller hücrenin %80'i, sayaç görselin üstünde (büyük, kalın, dış çizgili), katman parça < zincir < engel < sayaç, pause kartı (Devam / Yeniden başla / Ana menü → quit), duvar geçici kırmızı-beyaz | ✅ | |
| PM_22: zincir kilidi hücre ortasında (%45), p19 yayında; iOS 0.5.1 (4) zip 31 Ekim 19:30 | ✅ | |
| Codex dosya adları bekleniyor: `pause-normal`, `wall-stripe`, `piece_*` (HANDOFF'a düşünce art_choice.py + bağlama) | 🔜 | |
| Önizleme 7: parçalar Kit81 ile (KitAtlas → 3 atlas, Piece.shader kit dalı; resimli bölümde mask+overlay, resimsizde flat) | ✅ | |
| Önizleme 8: Task6 tahta görselleri bağlandı (çivi/bomba %80 + sayaç ortada, zincir şeridi + uç kilidi, duvar şeridi + köşe); HANDOFF'a oyun adlarıyla eklendi. Pause görseli yok, çizim tuş kaldı | ✅ | |
| Önizleme 9: bomba gövdesi ortada %85, sayaçlar Fredoka SemiBold (OFL) lacivert, çift basamak küçülür; zincir kilidi %45, duvar %18, çerçeve altın tint | ✅ | |
| Önizleme 10: Codex Task7 bağlandı (bomba/çivi v2, duvar v2 + köşe, altın tahta çerçevesi, HUD rozet/kutu/urgent/ikonlar/pause); geçici tint ve ofset kalktı | ✅ | |
| Gece QA (PM_MESAJ_14): 40/40 çözüm, slotlar, 3 boyut ekran, telemetri uçtan uca ✓; 6/7 sorun düzeldi; Önizleme 11 yayında | ✅ | |
| Build 5 adayı (bayraklar açık) Builds/build5-candidate; yayın Zeyd komutuyla; iPhone FPS ölçülmedi | 🔜 | |
| Önizleme 12: PALETTE.md zorunlu palet (Ui/Palette.cs: rakam, kontur, urgent, tint, rozet, duvar), booster ikonu iç dairenin ~%75'i; önizlemeler `Tools/preview_mode.sh on/off` ile kök değişmeden; Build 5 şablonundaki ikon kontrolü düzeltildi. Task12 bekleniyor | ✅ | |
| Ana menü Task13: profil kutusu (avatar, ad, Jiggles), yıldız sayacı, ayarlar, "Jiggles rescued X / 100·500·1000", Level tuşu v2 (LilitaOne) + Levels; basılı hâller; alt gezinme gizli | ✅ | |
| Menü QA 3 boyut (docs/qa/2026-10-26-menu): taşma yok; profil yazısı krem kutuda okunmuyordu → lacivert | ✅ | |
| Önizleme 12 (PM_MESAJ_15): Task13 menü + Task14 oyun içi (HUD, birleşik booster tuşları, çerçeve, bomba/çivi/zincir/duvar, kartlar); Lilita rakam/İngilizce, Fredoka Türkçe (ğ/ş/İ RS Sans yedeği) | ✅ | |
| QA 3 boyut (docs/qa/2026-10-26-t14) 8/8; düzeltilen: çift kilit, 4'e bölünmeyen dokular (kart 3,4 MB → 167 KB), kullanılmayan görseller; build 10,9 MB | ✅ | |
| Önizleme 13 (PM_MESAJ_16 A): sürüklerken zincir kilidi kayboluyordu (sprite'lar parçayla yükselmiyordu) + engelleyen parçanın parlaması girintileri boyuyordu ("kırmızı 7") → düzeldi; LV rozeti ortalandı | ✅ | |
| Menü: "Jiggles rescued" çubuğu kalktı, yıldız sayacı → Jiggles sayacı, profilde yalnız ad (ortalı), avatar seçimi (5 Jiggle, cihazda); B kısmı Task15 bekliyor | ✅ | |
| Önizleme 14 (PM_MESAJ_16 B): Task15 bağlandı (rozet v2, duvar v3, HUD hap v2 + ikonlar, kart tuşları v2 ≤ %65, avatarlar, Jiggles sayacı); kilitliyken boş rozet gizlendi | ✅ | |
| Resimli test: ?set=redesign&painting=1, Level 7'de painting-test-01 Kit81 maskesiyle; normal set düz mavi | ✅ | |
| Önizleme 15: resimli level kenarları (lacivert kontur, krem şerit, bevel, sürükleme gölgesi), yalnız painting=1 | ✅ | |
| Önizleme 17: resimli level kenarı = kalın koyu lacivert (#06161D, 1,5×, krem şerit %80, dış gölge); karşılaştırma parametresi kaldırıldı | ✅ | |
| Menü: Levels kısayolu kalktı, Level tuşu ortada; avatar kutusu turkuaz radyal zemin, Jiggle %85 ortalı (seçim kartında da) | ✅ | |
| Kart tuşları: Task15 v2 tuşları 684×336 oranında (9-slice germe yok), basılı hâl, yazı krem 22 pt + koyu kontur + gölge; başlangıç kartında engel ikonları 80 pt, zor uyarısı mercan | ✅ | |
| Web Build 5 kökte yayında (5fc3142); gizlilik sayfası + levels/redesign.json gh-pages'te | ✅ | |
| iOS TestFlight 0.5.0: Xcode projesi + Builds/ReverseSolver-iOS-0.5.0.zip hazır; Mac'te yükleme Zeyd'de (docs/MAC_YUKLEME.md) | ✅ | |
| SHOW1 vitrin bölümü yayında (lv=SHOW1): resimli 6×6, 4 engel, ilk deneme %89; demo kartları, isim girişi yok, SHOW etiketi | ✅ | |
| SHOW1 v2: menü önce + "Demo" tuşu, 4 booster ×1, kazanma kartında Tekrar oyna + Ana menü | ✅ yayında | |
| Sesler: C seçildi → parça başına kısa C, son parça uzun C, kazanınca dı-dı-dııı fanfarı (/sounds/) | ✅ yayında | |
| PM_MESAJ_18: 40 tablo her bölümde (Pages'ten, önbellek, menü tablosu, kazanınca 1 sn resim) — önizleme /p18/ | ✅ önizleme · ⏳ kök için Zeyd onayı · ⏳ iOS 0.5.1 | |
| Kazanma anındaki tam resmin sağ/alt kenar çizgilenmesi | 🔜 | |
| PM_MESAJ_19: sesler, ayarlar (ses/titreşim/dil), English varsayılan, kısa kartlar, isim girişi, duvar=çerçeve, titreşim — önizleme /p19/ | ✅ | |
| iOS 0.5.1 (2) zip: Builds/ReverseSolver-iOS-0.5.1.zip | ✅ · ⏳ Mac'ten TestFlight (Zeyd) | |
| Arka plan müziği (music-c, akışla, Ayarlar → Müzik) | ✅ önizleme · iOS zip bekliyor | |
| Dil ayarını telemetride not etme | 🔜 | |
| PM_MESAJ_20 zincir görünürlüğü (geçici, kodla): 2x kalın, lacivert, krem kenar, gölge; katman parça < zincir < engel < sayaç | ✅ yayında | |
| Veri sistemi Faz 1 (kohort, cihaz, start/hide/quit, 1–10, profil) yayında /p19/; Faz 2 (yeniden oynatıcı, botlar, Monte Carlo, geri test, dongu.py) hazır (PM_MESAJ_25) | ✅ | v1 tabanları geçemedi; PM yükleyip dondurur |
| Yeni eğitim (PM_MESAJ_28 A) /p20/ + FTUE/performans raporu (B, docs/FTUE_PERF_RAPOR.md) | ✅ yayında | veri sorunu PM_MESAJ_29 ile çözüldü |
| Davet öncesi düzeltmeler (PM_MESAJ_29: veri set_rating, Ev halı, FTUE kararları, ilk indirme 13,8 MB) /p20/ | ✅ yayında | canlı (a)(b)(c) geçti |
| Booster eğitimi 5/9/15/23 + envanter (BoosterStart 3) + çoklu spotlight (PM_MESAJ_30) /p20/ | ✅ yayında | PM kararı bekleyenler OVERNIGHT'ta |
| Eğitim karartması tek maske (shader, yumuşak yuvarlak delikler) + bölüm 10 tek zincir çifti (PM_MESAJ_31) /p20/ | ✅ yayında | QA docs/qa/2026-11-02-spot |
| Tanıtım 10 (Z24) ve 14 (Z21) sadeleştirme v2 (PM_MESAJ_32) + bölüm 1 hatalı adım, dik el, unlock=1, koleksiyon sıradaki, Jiggles çubuğu, harita işaretleri (PM_MESAJ_33) /p20/ | ✅ yayında | QA docs/qa/2026-11-02-son |
| Bölüm 1 eğitimi: elin sırtı (Codex hand-back-v1 + basılı kare) + adım 2–3 sağ alt parça (PM_MESAJ_34) /p20/ | ✅ yayında | QA docs/qa/2026-11-03-el |
| Eldivenli el (sırt, ~1,3 hücre), zor bölümde tek kart, Ev profil hizası, harita düğüm hazırlığı (PM_MESAJ_36) /p20/ | ✅ yayında (Task23 düğümleri bekleniyor) | QA docs/qa/2026-11-03-dort |
| Task23 harita düğümleri + zor kartı bombasız (PM_MESAJ_37) | ✅ yayında 848fa47 | QA docs/qa/2026-11-03-sabah |
| Kart tıklama kilidi, LevelList sığıyor, avatar Baron'a binmiyor (PM_MESAJ_38) | ✅ yayında 8ad50da | davet testi GEÇTİ; (b) tanıtım ayrı ölçülemedi; S45'te tanıtım yerine profil sorusu (açık) |
| p20 menü revizyonu A+B (PM_MESAJ_27): alt çubuk v3, üst bar, Nunito Black banner stili, ataletli kaydırma, Task21 parçaları /p20/ | ✅ yayında | Ayarlar anahtar zemini + fırlatma mesafesi düzeltildi, sonraki build'de yayına girecek |
| Koridor Ana menü yolu (PM_MESAJ_24B) /p19/ + menü ekranları A+B (PM_MESAJ_26) /p20/ | ✅ | iPhone'da bakılmadı; süre istatistikleri bu sürümden itibaren |
| Koridor geçişi (PM_MESAJ_24): kazan → 1–10/👍👎 → Devam → N'nin tablosundan N+1'inkine kayma (tek şerit, video yok, +242 KB), dokununca atla; /p19/ | ✅ | iPhone'da bakılmadı; 15→16 betikte kararsız (oyun değil) |
| SET40 v4 (docs/SET40_V4.md): 9 bölüm ayarlandı, /p19/?set=redesign önizlemede (PM_MESAJ_23) | ✅ önizlemede | kök ve /levels/redesign.json (iOS) Zeyd'in "kökü güncelle"siyle |
| PM_MESAJ_20: Codex Task17 renkli zincirler | ✅ yayında | |
| Önizleme geri bildirimi: tek tutorial eli, duvar köşe dirseği, ilerleme yedeği (plays'ten geri getirme) — /p19/ | ✅ | |
| iOS 0.5.1 (3) zip, 29 Ekim 15:31 | ✅ · ⏳ Zeyd Mac'ten TestFlight | |
| ZeydBot v2 | ⛔ setten sonra | |

## Art (ChatGPT / Codex — Art Director doldurur)

Koridor zafer geçişi V2 süreklilik düzeltmesi (2026-10-30): dış cephe kaldırıldı, tek kapalı duvar + derinlikli sütun/kaide yüzeyleri; halı/bordür/mermer tek ortonormal zemin düzleminde, fotoğraf/atlas üçgen sınırı yok. Aynı kamera yolu ve 3 sn süre; orta hız tepesi %32 azaltıldı. Statik kare aynı sahneden; tablo kırpması statik/hareketli aynı shader yolunda (oyun içi OverrideGeometry hatası giderildi). 181 kare/43 kritik ardışık kare + Unity GPU + gerçek Core kazanımı/normal menü dönüşü/tek oynatım/durma: 6/6 test. MP4 aynı klasörde 1080p60 güncellendi, önceki sürüm/video tam yedeklendi. Rapor: `docs/art/corridor-victory/CONTINUITY.md`. Cihaz/yayımlanmış player manuel kontrolü yapılmadı.

| İş | Durum | Tahmini süre |
|---|---|---|
| App ikonu: onaylı mavi sürüm, PWA/favicon/Apple touch dışa aktarımları (1024/512/256/192/180/32) | ✅ | |
| Ana menü koridoru v04: kaynak korunuyor; iki güvenli LEVEL X yerleşimi hazır (`Menu/safe-area-options-v01.html`) | ✅ | |
| LEVEL X düğmesi: turkuaz/turuncu iki @3x 9-slice adayı (`Menu/Buttons/review-options-v01.html`) | 🔜 Zeyd seçimi | |
| Tutorial eli: dokun/kaydır için eldivenli ve çıplak A/B setleri (`TutorialHand/Options/review-options-v01.html`) | 🔜 Zeyd seçimi | |
| Engel tanıtımları: çivi, bomba, zincir, duvar için A/B kadrajları (`ObstacleIntros/review-options-v01.html`) | 🔜 Zeyd seçimi | |
| Zor bölüm uyarısı: Baron/Jiggle odaklı A/B (`HardWarning/review-options-v01.html`) | 🔜 Zeyd seçimi | |
| Popup çerçevesi: turkuaz-altın / krem-teal 9-slice A/B (`UI/PopupFrame/review-options-v01.html`) | 🔜 Zeyd seçimi | |
| Zafer görseli: tek Jiggle / üç Jiggle A/B (`Victory/review-options-v01.html`) | 🔜 Zeyd seçimi | |
| Kaybetme görseli: bomba/süre/hamlesiz sebepleri için A/B (`Defeat/review-options-v01.html`) | 🔜 Zeyd seçimi | |
| P1 #10: oyun içi engel ikonları, şeffaf 128 px exportlar (`Gameplay/ObstacleIcons/index.html`) | ✅ | |
| P1 #11: booster ikonları, şeffaf 128 px exportlar (`Gameplay/BoosterIcons/index.html`) | ✅ | |
| P1 #12: tablo yönü A/B örnekleri (`Paintings/review-directions-v01.html`) | 🔜 Zeyd yön seçimi; seri bekliyor | |
| P1 #13: tahta altlığı A/B, şeffaf merkezli kare çerçeve (`Gameplay/BoardBase/review-options-v01.html`) | 🔜 Zeyd seçimi | |
| P1 #14: dört oyun sesi için iki özgün WAV tını yönü (`Audio/review-options-v01.html`) | 🔜 Zeyd dinleme/seçimi | |
| #20: Bölüm listesi, 5×8 LevelList SVG @3x (`MissingScreens/review-v01.html`) | 🔜 Zeyd incelemesi | |
| #21: Duraklatma/çıkış onayı SVG @3x | 🔜 Zeyd incelemesi | |
| #22: Booster onay kartı SVG @3x | 🔜 Zeyd incelemesi | |
| #23: 5/9/15/23 booster tanıtımı için ortak SVG @3x | 🔜 Zeyd incelemesi | |
| #24: İsim sorma ekranı SVG @3x | 🔜 Zeyd incelemesi | |
| #25: WebGL yükleme ekranı SVG @3x | 🔜 Zeyd incelemesi | |
| 4c jigsaw tek onay parçası O-I-F-F: üst çıkıntı + sağ girinti, iki kenarda da merkezli (`PieceSkin/Task6/piece_O-I-F-F-review.html`) | 🔜 Zeyd incelemesi; 81 kombinasyon örnek onayından sonra | |
| Oyun içi v2: bomba, çivi, duvar, altın tahta çerçevesi ve HUD 12 PNG (Gameplay/Task7/); HANDOFF tamam | ✅ | |
| Booster çubuğu ve üst bar v3: koridor stilinde buton, 4 ikon, sayı rozeti, sade level rozeti ve simetrik hap (Gameplay/Task8/); önizleme HANDOFF'ta | 🔜 Zeyd değerlendirmesi | |
| Booster çubuğu ve ikonları Task10: referansın parlak oyuncak stilinde, 4 şeffaf ikon ve yan yana önizleme; Task9 iptal edilip kaldırıldı | 🔜 Zeyd değerlendirmesi | |
| Task11: oyun içi HUD, menü başlat tuşu, kart tuşları/paneli ve kazanma yıldızları; 5 grup önizlemesi hazır, HANDOFF'a işlendi | ✅ | |
| Task12: koridor ana menüsü için Level X tuşu, profil/avatar, sayaçlar, ayarlar ve alt gezinme; 390×844 önizleme hazır, HANDOFF eklendi | 🔜 Zeyd değerlendirmesi | |
| Task12 Gameplay: booster seti, HUD, menü/kart UI ve tahta çerçevesi PALETTE.md zorunlu tonlarına eşlendi; preview ve HANDOFF hazır | 🔜 Zeyd değerlendirmesi | |
| PlayButton v2 REV: referans restorasyonu ve krem yazı Zeyd tarafından onaylandı; Task13 ana sanat dayanağı olarak korundu | ✅ | |
| Task13 ana menü: 28 şeffaf PNG; profil/sayaç/ayarlar/ilerleme/gezinme, normal-basılı durumlar, koridor üstünde menü ve tek sayfa inceleme hazır; HANDOFF/manifest tamam | 🔜 Zeyd değerlendirmesi | |
| Task14 revizyon 2: beğenilen panel/çerçeve korundu; yeni 4 booster, güçlü bariyer, bomba/çivi/zincir, iki haneli sayaç ve 64 px testleri hazır; ayrı sprite/kartlar onaydan sonra | 🔜 Zeyd revizyon onayı | |
| Tüm A/B seçimlerini tek yerde göster (`docs/art/SECIM.html`) | ✅ | |

## Bitenler (son 3 gün)
- ✅ PM_45 videolar → `Level Design/kareler/` · PM_47 hikâye eğrisi (`docs/analiz/HIKAYE_EGRISI_RAPOR.md`) · PM_48/48B ajan döngüsü + p21 `?set=ajan1` (bomba yayı 6–7, `docs/analiz/AJAN_DILIM1_RAPOR.md`) — 8 Kasım
- ✅ PM_49 ajan2: kare tahta, cömert süre, R20/R21; p21 `?set=ajan2` (06 GEÇTİ, 07 aday — PM kararı) — 8 Kasım
- ✅ PM_50 açılış hikâye videosu: kurgu + ses + oyuna bağlama, p21 `?intro=1` (`docs/art/task26/video/KURGU_RAPOR.md`); `intro_events` SQL'i onay bekliyor — 9 Kasım
- ✅ PM_51 açılış videosu düzeltmeleri: CC0 ses + Kling ses seçenekleri, 1→2 geçişi, eski 1. bölüm resmi; ses seçimi Zeyd'de — 9 Kasım
- ✅ PM_52 kit envanteri: 13 sistem, arayüz taslağı, eski dosya listesi, ilk 5 parça (`docs/kit/KIT_ENVANTER.md`); yalnız rapor, kod değişmedi — 9 Kasım
- ✅ PM_52/53 yeni UI ve engel görselleri p21'de: vitrin `?lv=SHOW_ART`, `&art=eski`, `&fps=1`; canlı verify 4/4 (`docs/art/task28/ENTEGRASYON_RAPOR.md`); telefonda FPS Zeyd'de — 9 Kasım
- ✅ PM_55 vitrin geri bildirimi p21'de: bomba nabzı ve ateş, zincir sarkması ve kalınlığı, zincirli parçaların düzlemi; canlı verify 4/4 — 9 Kasım
- ✅ PM_54 p21'de: HUD şeritleri, Jigo, kilit, kazanma, devam + altın; canlı verify 4/4 (`docs/art/task29/ENTEGRASYON_RAPOR.md`); 015 SQL PM'de — 10 Kasım
- ✅ PM_56/57 p21'de: Verlet zincir, kilit düşüşü, bağlantı noktaları, bomba nabzı, 5000 altın, intro ses C; canlı verify 4/4 — 10 Kasım
- ✅ PM_58 p21'de: fiske ile hamle, kenar payı 24 pt, tarayıcı kenar hareketi engeli; canlı verify 4/4 — 10 Kasım
- ✅ PM_59 p21'de: 2×2 tutorial + kolay 3×3 (yalnız yeni p21 oyuncusu), hedef bandı, kartlar, kilit uyarısı, `&reset=1`; canlı verify 4/4 — 10 Kasım
- ✅ PM_60 p21'de: tutorial geri alındı + hikâye kutusu, booster gösterimleri, haz, kazanma + sandık; canlı verify 4/4 — 11 Kasım
- ✅ PM_62 p21'de: test modu `&test=1`, yeni saat, makas yavaş, değnekte süre akar, kilit okunur, kombo takılmada sıfır, sade kazanma, Jigo geçişleri, harita hapı; canlı verify 4/4 — 11 Kasım
- ⏳ Build +6,9 MB (efekt kareleri sıkıştırılmamış) — doku sıkıştırma kararı PM
- ⏳ PM_60 doğrulanmadı: kombo rozeti, `drag` telemetrisi, sesler, iPhone
- ⏳ `017_tutorial_events.sql` — PM uygular
- ⏳ Tutorial adım 3'te yanlış yön yok (A her yönden çıkıyor) — PM kararı
- ✅ `HardestLevelStillUnderTwoSeconds` → Perf; normal batch dışında, sınır aynı
- ⏳ Fiske/sürükleme telemetrisi `MoveRow.INPUT_COL=false`; sütun hazır olunca açılacak. `017_tutorial_events.sql` hazır, uygulanmadı
- ⏳ `docs/sql/016_continue_select.sql` (continue_events 401) — Zeyd "çalıştır" deyince

- ✅ ZeydBot v1: 30 bölümle kuruldu, oturum 3 sınavını geçti (sıra 0,99)
- ✅ Tasarımcı ajan yöntemi: Z31–Z40, 10 bölümün 8'i 8+ puan
- ✅ Yeni metrikler: M13 anahtar parça anı, M14 bomba çeşitliliği, M15 süre gerilimi
- ✅ Supabase: moves tablosu, 007 (touch/pause/puan); kayıptan sonra puan sorusu düzeltildi
- ✅ Telefondan Claude Code'a mesaj (Remote Control) çalışıyor
