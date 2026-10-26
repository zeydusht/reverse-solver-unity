# Reverse Solver — BACKLOG

Son güncelleme: 26 Ekim 2026 (Codex — Art)

Kurallar:
- PM yalnız "PM" bölümünü, Claude Code yalnız "Mühendislik" bölümünü günceller.
- Her güncellemede en üstteki "Son güncelleme" satırı da değişir.
- Durum etiketleri: ⏳ sürüyor · 🔜 sırada · ✅ bitti · ⛔ beklemede

---

## ŞİMDİ

| İş | Kim | Durum | Tahmini süre |
|---|---|---|---|
| SET40 v3 (öğretici eğri + tutorial + Build 5 hazırlığı) — PM_MESAJ_6 | Claude Code | 🔜 gece | ~1 gece |

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
| iOS TestFlight 0.5.0: kod + ayar hazır; build/zip iOS Build Support kurulumunu bekliyor (yönetici izni) | 🔜 | ~30 dk |
| ZeydBot v2 | ⛔ setten sonra | |

## Art (ChatGPT / Codex — Art Director doldurur)

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

- ✅ ZeydBot v1: 30 bölümle kuruldu, oturum 3 sınavını geçti (sıra 0,99)
- ✅ Tasarımcı ajan yöntemi: Z31–Z40, 10 bölümün 8'i 8+ puan
- ✅ Yeni metrikler: M13 anahtar parça anı, M14 bomba çeşitliliği, M15 süre gerilimi
- ✅ Supabase: moves tablosu, 007 (touch/pause/puan); kayıptan sonra puan sorusu düzeltildi
- ✅ Telefondan Claude Code'a mesaj (Remote Control) çalışıyor
