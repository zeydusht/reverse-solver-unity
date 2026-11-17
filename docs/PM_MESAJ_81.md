# PM_MESAJ_81: Soft launch altyapısı: mağaza sürümü, AppLovin MAX, telemetri, 100 bölüm

Kodu kendin yaz. Önce `docs/` commit. Yeni dal `pm81` (p23'ten). Bitiş mesajı CLAUDE.md biçiminde, en altta, "Oyna" linkleriyle.

Bağlam: `docs/YOL_HARITASI_SOFT_LAUNCH.md` dosyasını oku. Kısaca 24 Kasım'da iOS App Store'da yalnız Türkiye'de yayın, hedef 100–150 indirme, para kazanma AppLovin MAX reklamları, IAP yok, 100 bölüm. **Bu iş bitince özellik dondurulur.** Bu mesajda yeni görsel ya da animasyon işi yok.

PM_80 bitmediyse önce onu bitir. Zeyd dalış seçimini (A/B/C) yazınca varsayılan o olur; yazmazsa A kalır. Dalış cilası burada biter.

## 1. Mağaza sürümü ayrı yoldan okusun: `live/`
- Pages'te yeni klasör `live/`: p23'ün **bölüm, tablo ve ayar** verisinin dondurulmuş kopyası. Oyun kodu (WebGL) gerekmez, yalnız veri.
- iOS build yalnız `live/`'dan okur. p23, p22 ve kök ona dokunmaz. `live/`'a yayın tek komutla olsun (`Tools/yayin/live_yayinla.py`). **Yalnız PM onayıyla çalıştırılır**; bu iş içinde ilk kopyayı sen oluştur.
- **Çevrimdışı güvence:**
  - build içine gömülü yedek: bütün bölüm JSON'u + 1–20 tabloları;
  - indirilen her şey cihazda önbelleğe yazılır;
  - internet yoksa önbellek, o da yoksa gömülü yedek kullanılır. Oyun hiçbir durumda boş ekranda kalmasın.
- `live/manifest.json`: içerik sürümü + dosya listesi + özetler (hash). Oyun açılışta sürümü karşılaştırır ve yalnız değişeni indirir.
- Test: uçak modunda ilk açılış (gömülü yedek), sonra internetli açılış (indirir), sonra tekrar uçak modu (önbellek).

## 2. Uzaktan ayar: `live/config.json`
Build'siz değişecekler: reklam sıklıkları (aşağıdaki değerler varsayılan), devam fiyatları (900/1900/2900), başlangıç altını (5000), +30 sn, `reklam_acik` (true/false). Dosya inemezse gömülü varsayılanlar kullanılır.

## 3. AppLovin MAX
- Unity için **AppLovin MAX eklentisi** (Integration Manager). Yalnız AppLovin ağı; başka ağ ekleme.
- Anahtarlar `Assets/Config/ads.json`: SDK Key, iOS Rewarded ID, iOS Interstitial ID. Zeyd henüz göndermediyse yer tutucu bırak, PM sonra doldurur.
- **Yerleşimler:**
  - **R1 (ödüllü), Süre doldu paneli:** yeşil altın tuşunun yanında "Reklam izle +30 sn" tuşu. Deneme başına 1 kez. `won_after_continue` kuralı aynen geçerli; sonuçta hangi yolla devam edildiği (`continue_type`: gold/ad) yazılsın.
  - **R2 (ödüllü), kazanma ekranı:** "Reklam izle, 2× altın" (+50 → +100). Sonraki Seviye her zaman bir dokunuşla geçilebilsin; reklam zorunlu değil.
  - **I1 (geçiş reklamı):** yalnız kazanmadan sonra, Sonraki Seviye'ye basınca, koridor geçişinden önce. Kurallar (config):
    - `gecis_baslangic_bolum` = 12
    - `gecis_her_n_bolum` = 3
    - `gecis_min_sn` = 120
    - ilk oturumun ilk 10 dakikasında yok
    - kayıptan sonra yok
  - **Banner yok.**
- **Davranış:**
  - Reklamlar önceden yüklensin.
  - Reklam hazır değilse ödüllü tuş **görünmesin** (ölü tuş yok).
  - Reklam sırasında süre durur, oyun sesi kısılır, reklam bitince geri gelir.
  - Ödül yalnız "ödül kazanıldı" geri çağrısında verilsin.
  - Hata olursa oyun aynen devam etsin.
- **Gizli test menüsü:** Ayarlar'da sürüm yazısına 5 dokunuş → MAX Mediation Debugger + test reklamı göster.
- Web (WebGL) sürümünde reklam yok; tuşlar görünmesin, kod derlensin.

## 4. İzin, gizlilik, Apple kuralları
- MAX'in **Terms & Privacy Policy akışı** açık: gizlilik linki + ATT izni.
- ATT izni ilk açılışta değil, **2. bölüm kazanıldıktan sonra** sorulsun (profil sorusundan sonra, aynı anda değil). Mümkün değilse ilk açılış kalsın, raporla.
- `NSUserTrackingUsageDescription`:
  - TR: "Sana daha uygun reklamlar göstermek ve oyunu ücretsiz tutmak için kullanılır."
  - EN: "Used to show you more relevant ads and keep the game free."
- `SKAdNetworkItems` MAX listesiyle; `PrivacyInfo.xcprivacy` var olsun (MAX'inki + bizim: Supabase'e giden oyun verisi).
- `ITSAppUsesNonExemptEncryption = false`.
- **Gizlilik sayfası** (`/privacy/`) TR + EN güncel olsun: AppLovin reklamları, reklam kimliği (IDFA, izin verilirse), cihaz kimliği (IDFV), oyun verisi (Supabase), KVKK başlığı, veri silme talebi için e-posta. Değişiklikleri raporda listele; PM okuyacak.

## 5. Telemetri: `sl1` kohortu
- Mağaza build'i otomatik `cohort = 'sl1'`, `platform = 'ios'`, `app_version`, `content_version` göndersin. Cihaz kimliği IDFV.
- **Yeni olaylar:**
  - `first_open`
  - `session_start` / `session_end` (süre)
  - `att_status`
  - `ad_shown` (yerleşim, ağ, gelir: MAX `OnAdRevenuePaidEvent`)
  - `ad_reward`
  - `ad_failed`
  - `ad_skipped_not_ready`
  - `crash` (yakalanmamış istisna: mesaj + ilk 5 satır yığın)
- Kuyruk: internet yoksa olaylar cihazda bekler, sonra gönderilir. Uygulama kapanırken kaybolmasın.
- SQL `docs/sql/019_soft_launch.sql`: tablolar/sütunlar + görünümler:
  - `v_sl_tutma`: kurulum gününe göre D1/D3/D7;
  - `v_sl_huni`: bölüm bazında ulaşan, kazanan, bırakan;
  - `v_sl_reklam`: gün bazında gösterim, DAU başına gösterim, ödüllü izleme oranı, gelir;
  - `v_sl_gunluk`: kurulum, DAU, oturum, dk/DAU.
- Zeyd'in cihazları ve test isimleri dışlansın (migration 013 kuralı). **SQL'i uygulama**; PM Zeyd onayıyla uygular.

## 6. 100 bölüm desteği
- Harita, sandık eşikleri (3/5/10 düzeni 100'e kadar sürsün), Jiggle sayacı, Profil ve tablo listesi **100 bölümü** taşısın.
- 41–100 şimdilik **yer tutucu**: var olan bölümlerden kopya, bayrakla işaretli (`placeholder: true`). Asıl bölümler PM_82'de gelir.
- 100'ü bitirince "Yeni tablolar yolda!" ekranı: Jigo + toplam kurtarılan Jiggle + Menü. Oyun kilitlenmesin; bitmiş bölümler tekrar oynanabilsin.
- Tablo yoksa (61–100 gecikirse) geçici olarak 1–40'tan bir tablo kullanılsın, çökme olmasın.

## 7. iOS build ayarları (zip alma, yükleme yok)
- Sürüm **1.0.0 (8)**, yalnız iPhone, yalnız dikey, 60 fps kalsın.
- İkon ve açılış ekranı yerinde. Unity açılış logosu lisans izin veriyorsa kapalı.
- Build'in Xcode projesi derlenebilir olsun. Zip'i PM_84'te (Cuma) alacağız. Bugün yalnız ayarların doğru olduğunu doğrula.

## 8. Kontrol listesi dosyası
`docs/SOFT_LAUNCH_KONTROL.md`: her maddeyi ✅/❌ ile yaz (bu mesajdaki 1–7 + Apple'ın istedikleri). Zeyd'in App Store Connect'te yapacakları ayrı bölümde, basit adımlarla.

## Teslim
- EditMode + PlayMode yeşil; yeni testler:
  - reklam sıklığı kuralları;
  - reklam hazır değilken tuş gizli;
  - çevrimdışı açılış 3 durumu;
  - 100 bölüm haritası;
  - olay kuyruğu.
- `verify_build` p23 4/4. `live/` yayında ve oyun ondan okuyor (web test bayrağı `&kaynak=live`).
- `PROJE_DURUMU`'na tek satır, commit + push.
- Son mesaj kısa, en altta, "Oyna" linkleri:
  - `p23/?c=p23&test=1&lang=tr`
  - `p23/?c=p23&test=1&lang=tr&kaynak=live`
  - `p23/?c=p23&test=1&lang=tr&lv=100` (bitiş ekranı)
