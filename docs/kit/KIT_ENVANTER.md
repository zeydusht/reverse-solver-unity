# Oyun üretim kiti envanteri

9 Kasım 2026. Saatler kaba **tahminlerdir**; tek geliştiricinin ayırma, uyarlama ve ilk doğrulama emeğini anlatır. Envanter dosya başları, `rg` ile başlıklar, sınıf/fonksiyon imzaları ve seçili tanımlayıcı eşleşmelerinden çıkarıldı; tam kod okuması yapılmadı.
“Güncel”, mevcut kaynakta kullanım/bağlantı bulunduğunu belirtir; çalıştırılarak doğrulandığı anlamına gelmez. PM_52'de verilen başlangıç yollarının tümü mevcut. p21 kartlarının yalnız adları görüldü, içerikleri incelenmedi. Hiçbir araç çalıştırılmadı; canlı servis ve ikili varlık doğrulaması yapılmadı.

| Sistem | Dosyalar | Tür | Mekaniğe bağımlılık | Kite almak için ne gerekir | Durum | Tahmini saat |
|---|---|---|---|---|---|---|
| Kural/çözücü çekirdeği | `Assets/_Game/Core/{Model,Rules,Session,Solve}/*.cs`; `Tools/web_harness.js`; `Tools/lib/png.js` | Oyuna özel | `TravelRule.CanExit`, `VJoint/HJoint`: yönlü çıkış ve birleşimler; `GameSession.NailAt/FuseAt`, `SmartPlayer.ShortestFuse`: çivi ve bomba sırası; `UseHammer/UseScissors/UseWand/UseClock`: güçlendiriciler. | Kuralları oyun eklentisinde tut; aşağıdaki arayüzü uygula. Web referansını isteğe bağlı doğrulama sağlayıcısı yap. | Güncel: C# çekirdeği; eski port referansı: harness (`make_session_golden.js` çağırıyor). Güncel: `png.js` QA yardımcısıdır, çözücü değildir; `Tools/lib/` içinde başka kaynak yok. | 24–40 |
| Bölüm fabrikası + Monte Carlo + zorluk | `Assets/_Game/Generation/*.cs`; `Assets/_Game/Editing/{MonteCarlo,StructureMetrics}.cs`; `Assets/_Game/Editing/Difficulty/DifficultyModel.cs`; `Tools/set40_*.py`; `Tools/level_similarity.py`; `docs/{FAZ4_SEVIYE_FABRIKASI,ZORLUK_METRIKLERI}.md`; `docs/fabrika/*.md` | Karışık | `MaxNailShare`, `BombMargin`, `Fuse`, `NailCounter`, `MonteCarlo.BombRate`; benzerlikte `bomb/nail/chain` etiketleri. `set40_plan.py` engel tanıtım slotlarını sabit biliyor. | Arama, tohum ve finalist sıralamasını ayır; üretici, ölçüler ve kabul eşiklerini oyun sağlayıcısına taşı. SET40 içeriklerini örnek veri yap. | Güncel: fabrika/ölçüm çekirdeği ve v4 araçları; Eski deneme: SET40 v1/v3 belge betikleri ve tarihli fabrika koşu raporları. | 32–56 |
| Tasarımcı ajan + eleştirmen | `Tools/ajan/{dongu.py,test_dongu.py,BOLUM_AJANI.md,ELESTIRMEN.md}`; `Assets/_Game/LevelEditor/Ajan*.cs`; `docs/BOLUM_TASARIM_KITABI.md`; `docs/bolum_kartlari/`; `Tools/hikaye_egrisi.py` | Karışık | `recipe['bombs']`, `seals/chains/nails`, `bot_bilincli`, `bot_bomba_unutan`; `bomba_D` ilk fitil ile çıkış adımını kıyaslıyor. | Kart/eleştiri döngüsünü koru; tarif şeması, bot rolleri ve kural kontrollerini oyuna göre yükle. Mevcut p21 kartlarını kite taşıma. | Güncel: döngü ve ajan metinleri; kartların içerik durumu kapsam dışında. | 20–36 |
| ZeydBot | `Tools/zeydbot_*.py`; `Tools/zeydbot_fetch.js`; `Assets/_Game/LevelEditor/{ZeydBot,ZeydAnalysis,ZeydDesign}.cs` | Karışık | `MOVE_FIRST_FREE`, `slack_x`, `ZeydBot.PathRows`; tuzak payı, düşünme süresi, bomba payı; v2'de `dig_rate`, `played_before` ve Z01… sırası. | Kişi/veri yollarını yapılandır; hamle özelliklerini oyun adaptöründen al. Donmuş deneyleri genel modelden ayır. | Güncel: `model.py` ajan tarafından kullanılıyor; eski v2/oturum deneyleri ve yardımcıların belirsizlikleri aşağıda tek tek. | 16–28 |
| Oyuncu botları / FunSim / nüfus | `Assets/_Game/Generation/{FunSim,Calibration}.cs`; `Assets/_Game/LevelEditor/Veri.cs`; `Tools/veri/*.{py,js}`; `docs/VERI_SISTEMI.md` | Karışık | `BombAttention`, `EarlyBombMoves`, `HammerFuse`, `NailAt`, `Booster.Hammer`; Python özelliklerinde `fuse_slack_min`, `nail_max`, `chains`. | Nüfus örnekleme ve kalibrasyonu ayır; karar politikası, özellik üretimi ve keyif eşiklerini eklenti yap. Tahminleri insan verisiyle yeniden kalibre et. | Güncel: FunSim ve veri döngüsü; `Options.Legacy` eski karşılaştırma seçeneği; `v0_tahmin.py` dondurulmuş taban modeldir. | 24–40 |
| Telemetri + playtest | `Assets/_Game/Core/Telemetry/*.cs`; `Assets/_Game/Platform/Web/{WebTransport,WebBridge,TelemetrySettings}.cs`; `docs/sql/*.sql`; `docs/PLAYTEST_TASLAK.md` | Karışık | `MoveRow` içindeki `fuses`; SQL olaylarında `hammer/scissors/wand/clock`, özelliklerde `bombs/nails/fuse_min`. `v_attempts` test adlarını eliyor; sonraki görünümler `cohort` kullanıyor. | Olay zarfı ve kohort/kalite filtrelerini ayır; oyun özellik tablosunu değiştir. `level_id/version`, `row_id`, `device_id` ayrımlarını koru. | Güncel kaynak: 009 görünümleri; Eski deneme/taslak: 008 SET40 v3 sorgusu (2099 başlangıcı). Canlı Supabase durumu doğrulanmadı. | 12–24 |
| Sanat hattı (Codex) | `docs/ART_DIRECTOR_BRIEF.md`; `docs/art/`; `docs/CODEX_*.md`; `Tools/art_choice.py`; `Tools/t23_art_sync.cjs`; `Assets/_Game/Editor/MenuArtImport.cs` | Karışık | `CHOICE` grupları ve `intro` eşlemesinde `nail/bomb/chain/wall`; T23 klasör aileleri ve sabit kaynak/hedef yolları. | Brief, seçim, manifest ve teslim ölçülerini şablonlaştır; tema/engel adlarını ve kopyalama yollarını yapılandır. | Güncel: seçim/senkronizasyon araçları ve teslim belgeleri; Eski deneme: tarihli görev taslakları; T25 teslim başı oyuna bağlanmadığını söylüyor. | 8–16 |
| Hikâye + açılış videosu | `docs/HIKAYE.md`; `Tools/intro/kurgu.py`; `Assets/_Game/Core/IntroPlan.cs` | Karışık | Hikâye karakterleri ve tablo teması oyuna özel; kurguda `CLIP_TRIMS`, `sack_rustle`, `brush_dab`, `frame_thud`; `IntroPlan.EndingEligible` oyun sonu seçimi. | Zaman çizelgesi/ses miksini ayrı araç yap; klip planı ve hikâye olaylarını veri olarak ver. | Güncel: hikâye karar kaydı ve kurgu aracı; eski öneriler aynı belgede duruyor, video çıktıları incelenmedi. | 6–12 |
| Ses | `Tools/make_music.py`; `Tools/make_win_sounds.py`; `docs/sounds/`; `Assets/_Game/Presentation/Ui/Music.cs` | Karışık | Ses üreticilerinde bomba/çivi kuralı görülmedi; `music-c.wav` seçimi, sabit çıktı yolları ve kazanma melodileri oyuna özel. | Girdi/çıktı, seviye ve format ayarlarını parametreleştir; dinleme/seçim sayfasını şablonlaştır. | Güncel: seçilmiş müzik dönüştürücü; Eski deneme: kazanma sesi/müzik aday üretimi ve inceleme sayfaları. | 3–6 |
| Menü / meta / booster / i18n | `Assets/Scenes/Game.unity` (yalnız yol); `Assets/_Game/Presentation/{GameRoot,BoosterInventory}.cs`; `Assets/_Game/Presentation/Ui/*.cs`; `Assets/_Game/Core/Persist/*.cs`; `Assets/_Game/Core/Session/BoosterControls.cs`; `docs/i18n/en.md` | Karışık | `Booster.Wand/Clock/Scissors/Hammer`, `Level.BoosterIntro`, `Achievements.Compute` içinde `l.Bombs`; `Lang.T(en,tr)` iki dil seçiyor. | Menü bileşenlerini olay/servis arayüzlerine bağla; booster, başarı, ilerleme ve metin kataloglarını oyun verisine taşı. | Güncel kaynak; sahne içeriği ve görsel bağları incelenmedi. | 24–40 |
| QA / doğrulama | `Tools/verify_build.js`; `Tools/lib/png.js`; `Assets/_Game/Tests/EditMode/*.cs`; `docs/qa/`; `docs/UX_KOD_DENETIM.md` | Karışık | `L01/L40`, `Z01`, `S41/S50`, `solution`, `result === 'bomb'`, sabit sürükleme koordinatları ve sonuç kartı ölçüleri. | Tarayıcı/ekran ölçümü altyapısını ayır; senaryoları ve kontrol noktalarını oyun sözleşmesinden üret. | Güncel: doğrulayıcı ve 3 Kasım UX denetimi; eski sete bağlı senaryolar mevcut, bu görevde koşulmadı. | 12–24 |
| Yayın (web + iOS) | `Assets/_Game/Editor/{WebBuild,IosBuild}.cs`; `.github/workflows/webgl-build.yml`; `docs/{M7_IOS,MAC_YUKLEME,CLOUD_BUILD}.md`; `Tools/{zip_ios.py,unity_batch.sh,preview_mode.sh}` | Karışık | Kural bağımlılığı az; `preview_mode.sh` doğrudan `SetFlags.cs` içindeki `RedesignDefault/AskFun` ve HTML set seçimini değiştiriyor. Unity sürümü, ürün/çıktı adları sabit. | Ürün kimliği, Unity yolu ve dağıtım hedeflerini parametreleştir; önizleme için açık yapılandırma kullan. | Güncel: build/zip/batch kaynakları; Eski deneme: Build 5'e bağlı `preview_mode.sh`; yayın/imzalama çalışması doğrulanmadı. | 8–16 |
| PM ↔ Claude Code ↔ Codex iş akışı | `docs/PM_MESAJ_*.md`; `docs/PROJE_DURUMU.md`; `docs/CODEX_PLAN.md` | Genel | Oyun kuralları yerine görev kapsamı, kabul ve devir bilgisi taşır; SET40/p20 gibi proje adları içerikte geçer. | Görev, durum ve teslim şablonlarını örnek içerikten ayır; izin verilen dosya sınırını her görevde açık yaz. | Güncel: PM_52 ve durum dosyası; Eski deneme: önceki mesajlar ve CODEX_PLAN'daki tarihsel gece turları. PM_51 kapsam dışı. | 2–4 |

## Arayüz taslağı

Aşağıdaki 19 satır yalnız sözleşme taslağıdır; uygulama değildir. Hamle listesi yasal hamleleri döndürür; başarısız oyuncu girişleri ayrıca olay olarak ölçülebilir.

```text
OyunSözleşmesi:
  kimlikVeKuralSürümü() -> Kimlik
  tarifŞeması() -> Şema
  engelTanımları() -> [EngelTürü, Açıklama]
  bölümDoğrula(bölüm) -> [Hata]
  bölümÜret(tarif, tohum) -> Bölüm
  başlat(bölüm, tohum) -> Durum
  kopyala(durum) -> Durum
  durumAnahtarı(durum) -> Anahtar
  tahta(durum) -> GeometriVeNesneler
  engeller(durum) -> [Tür, Konum, Sayaç, Etki]
  kaynaklar(durum) -> SüreVeHamleBütçesi
  hamleler(durum) -> [Hamle]
  hamleAçıklaması(durum, hamle) -> MetinVeHedef
  uygula(durum, hamle) -> YeniDurumVeOlaylar
  zamanİlerlet(durum, saniye) -> YeniDurumVeOlaylar
  sonuç(durum) -> Sürüyor / Kazandı / Kaybetti(neden)
  özellikler(durum) -> AdlandırılmışÖlçüler
  tasarımÖzeti(bölüm) -> AmaçVeÖğretilecekKurallar
  olayŞeması() -> HamleVeSonuçAlanları
```

## Ölü/eski dosyalar

- Kanıt yöntemi: aşağıdaki tarihler her dosya için `git log -1 --format=%cs -- <dosya>` çıktısıdır; çağrı kanıtı `rg` ile Tools kaynakları ve docs Markdown dosyalarında arandı (p21, PM_51, art arşivleri ve bu envanter hariç). Referans yokluğu tek başına ölü kod kanıtı değildir; kesin **Ölü kod** hükmü verilmedi.
- `Tools/zeydbot_model.py` — **Güncel, dışlanmaz** (2026-11-08): `Tools/ajan/dongu.py:135` doğrudan import ediyor; v1 başlığına rağmen etkin bağımlılık.
- `Tools/zeydbot_model_v2.py` — **Eski deneme; emin değil** (2026-10-20): v2 deney başlığı ve `played_before` sabit Z sırası; kendisi dışında Tools/doc referansı bulunmadı, başlıkta vaat edilen `docs/analiz/zeydbot/loo_v2.md` mevcut değil.
- `Tools/zeydbot_score_session3.py` — **Eski deneme** (2026-10-20): `docs/analiz/zeydbot/RAPOR.md:51` oturum 3 değerlendirmesi; donmuş `fc4ef7c` tahminleri.
- `Tools/zeydbot_score_session4.py` — **Eski deneme** (2026-10-21): aynı raporun 80. satırı ve dosya başı, donmuş `6b602ba` Z31–Z35 değerlendirmesi.
- `Tools/zeydbot_score_drafts.py` — **Eski deneme** (2026-10-20): başlıkta `fc4ef7c` model/veri okuması; `tahmin_Z31_Z35.md:7` eski taslak puanlama çağrısı.
- `Tools/zeydbot_korelasyon.py` — **Eski analiz aracı; emin değil** (2026-10-20): `zorluk_analizi.md:3` aynı hesabın `zeydbot_model.py` içindeki `spearman` ile yapıldığını belirtiyor; genel kite ayrı araç olarak alınmamalı.
- `Tools/zeydbot_boards.py` — **Eski tasarım yardımcısı; emin değil** (2026-10-20): sabit `zeydbot.json` okur; kendisi dışında Tools/doc referansı bulunmadı. Çalışmaz/ölü olduğu kanıtlanmadı.
- `Tools/zeydbot_draft.py` — **Güncel yardımcı adayı; emin değil** (2026-10-21): `Z31_Z35_tasarim.md:3` ve dosya başı `ZeydDesign.Check` akışını bağlıyor; yeni ajan döngüsü tarafından çağrıldığı görülmedi. Genel kite doğrudan alınmaz.
- `Tools/zeydbot_fetch.js` — **Güncel veri yardımcısı adayı; emin değil** (2026-10-20): `RAPOR.md:51,80` çağrıları var; kanıt tarihsel, bugünkü servis çalışması doğrulanmadı. Yapılandırmadan kite alınmaz.
- `Tools/zeydbot_m13_kalibrasyon.py` — **Eski deney kalibrasyonu** (2026-10-21): `Z12_TARIFI.md:85` ve dosya başı eski M13 çalışmasına bağlıyor; çekirdeğe alınmaz.
- `Tools/zeydbot_m13_score.py` — **Eski deney puanlayıcısı** (2026-10-21): `tahmin_Z36_Z40.md:3` çağrısı; başlığı 14 zor bölüm ve sabit Z12 temposunu kullanıyor. Yeni ajan kullanım kanıtı yok.
- `Tools/set40_plan.py`, `Tools/set40_doc.py` — **Eski deneme** (ikisi 2026-10-21): başlıkları SET40 v1; `Tools/set40_v3_doc.py` (2026-10-24) v3 belgesi üretir. Güncel v4 için `Tools/set40_v4.py` (2026-10-31) ve `docs/SET40_V4.md` var; sürüme bağlı bu betikler genel kite alınmaz.
- `Tools/preview_mode.sh` — **Eski deneme; emin değil** (2026-10-26): başlık Build 5 bekleme durumuna özel; Tools/docs referans aramasında PM_52 envanter girdisi dışında çağrı bulunmadı. `git checkout` içeren geri alma kolu nedeniyle mevcut haliyle kit şablonu olmaz.

## İlk çıkarılacak 5 parça

1. **PM görev/devir şablonları (2–4 saat):** Çok az kod bağımlılığıyla kapsam, kabul ölçütü ve dosya sınırı disiplinini her oyuna taşır.
2. **Ses dönüştürme ve dinleme sayfası (3–6 saat):** Kurallardan bağımsız format/seviye ayarları; yolları parametreleştirmek yeterli başlangıç sağlar.
3. **Sanat brief–seçim–teslim şablonları (8–16 saat):** Manifest ve boyut kontrolleri tekrar kullanılır; tema ve engel eşlemesi ayrı veriye taşınabilir.
4. **Telemetri olay zarfı ve kohort filtreleri (12–24 saat):** Kimlik/sürüm, ilk deneme ve test verisi ayrımı sonraki oyunlarda aynı ölçüm hatalarını önler; oyun özellikleri ayrıca tanımlanır.
5. **QA tarayıcı/ekran ölçüm altyapısı (12–24 saat):** `verify_build.js` ve `png.js` ortak cihaz boyutu, hata ve görsel ölçüm altyapısı sağlar; sabit bölüm senaryoları adaptör gerektirir.
