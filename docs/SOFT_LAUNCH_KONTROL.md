# Soft launch kontrol listesi (PM_81 / PM_82)

Son güncelleme: 18 Kasım 2026, gece. ✅ yapıldı ve doğrulandı · 🟡 yapıldı, telefonda ya da Mac'te doğrulanmadı · ❌ yapılmadı / bekliyor.

## 1. Mağaza sürümü `live/`'dan okur
- ✅ Pages'te `live/` var: `levels.json`, `config.json`, `paintings/`, `manifest.json` (içerik sürümü + her dosyanın SHA-256'sı).
  - Şu an yayındaki: `sl100-ph-…` (PM_81'in yer tutucu 100'ü: p23'ün 40'ı + 60 işaretli kopya).
  - PM_82'nin sl100'ü **hazır ama yayınlanmadı** (PM onayı bekliyor).
- ✅ Yayın tek komut: `python Tools/yayin/live_yayinla.py --onay "<PM onayı>"`. Onay yazılmazsa çalışmaz. p23, p22 ve köke dokunmaz.
- ✅ iOS yalnız `live/`'ı okur (NativeHost: kohort `sl1`). Web'de test bayrağı `&kaynak=live`.
- ✅ Çevrimdışı güvence:
  - build'e gömülü yedek: bütün bölümler + ayarlar (Resources/Live), iOS build'inde 1–20 tabloları (StreamingAssets);
  - indirilen her şey cihazda önbelleğe yazılıyor;
  - açılış sırası: önbellek, yoksa gömülü yedek. Ağ en çok 2,5 sn beklenir, oyun hiç boş ekranda kalmaz.
  - Tablo yoksa 1–40'tan biri kullanılır (61–100 gecikirse).
- ✅ Açılışta sürüm karşılaştırılır, yalnız değişen dosya iner. Hash tutmayan dosya reddedilir; manifest en son yazılır.
- ✅ 3 durum testi (birim test `SoftLaunchTests`):
  1. uçak modunda ilk açılış → gömülü;
  2. internetli açılış → indirir;
  3. tekrar uçak modu → önbellek.
- 🟡 Aynı 3 durum iPhone'da denenmedi (TestFlight'ta denenecek).

## 2. Uzaktan ayar `live/config.json`
- ✅ Reklam sıklıkları, devam fiyatları (900 / 1900 / 2900), başlangıç altını (5000), +30 sn, `reklam_acik`, R1–R4 aç/kapa, R3/R4 günlük sınır, R4 altını. Dosya inmezse gömülü varsayılanlar.

## 3. Google AdMob (PM_101; AppLovin yeni yayıncı almadığı için MAX çıktı)
- ✅ Eklenti: Google Mobile Ads Unity Plugin 11.7.0 (iOS SDK 13.12.0). Bağımlılıklar: projede zaten olan Google External Dependency Manager 1.2.186 (eklentinin Unity kayıt defterinden EDM 2.1.1 kurması kapalı: o sunucuya bu makineden erişilemedi; eklenti eski EDM'yi destekliyor). Yalnız AdMob; mediation yok (ileride Unity Ads, Mintegral, AppLovin bidding eklenebilir). MAX ve MAX adaptörü silindi.
- ✅ Kimlikler `Assets/Config/ads.json` (`docs/ADMOB_KIMLIKLER.md`): mağaza build'i gerçek kimlikler; test build'i (`IosBuild.BuildTest`), editör ve test cihazı Google'ın test kimlikleri. Mağaza build'inde test ya da yer tutucu kimlik varsa build durur.
- ✅ Zeyd'in iPhone'u test cihazı: gizli test menüsünde "Bu cihazı test cihazı yap" (o cihazda yalnız test reklamı). AdMob panelinde de eklenebilir: Ayarlar → Test cihazları → menüde görünen IDFA.
- ❌ Anahtarlar: `Assets/Config/ads.json`'da yer tutucu. Zeyd SDK Key + iOS Rewarded ID + iOS Interstitial ID'yi gönderince doldurulacak. Yer tutucuyla build çalışır, reklam açılmaz, tuşlar görünmez.
- ✅ Yerleşimler:
  - R1: "Süre doldu"da "Reklam izle +30 sn", deneme başına 1; `continue_type` gold/ad.
  - R2: kazanmada "Reklam izle, 2× altın".
  - I1: kazandıktan sonra Sonraki'de, koridordan önce: 12'den sonra, 3 bölümde bir, 120 sn; ilk oturumun ilk 10 dk'sında ve kayıptan sonra yok.
  - Banner yok.
- ✅ PM_82: R3 (booster 0 → video işareti → +1, günde 5) ve R4 (menüde "Hediye" → 100 altın, günde 3).
- ✅ Davranış:
  - reklamlar önceden yüklenir, hazır değilse tuş görünmez;
  - reklam sırasında süre durur ve ses kısılır;
  - ödül yalnız "ödül kazanıldı" geri çağrısında verilir;
  - hata olursa oyun aynen devam eder.
- ✅ Gizli test menüsü: Ayarlar'da sürüm yazısına 5 dokunuş → "Reklam testi": Google Ad Inspector, test cihazı aç/kapa, gizlilik seçenekleri (UMP), IDFA.
- ✅ Web'de reklam yok; `&reklam=sahte` ile tarayıcıda sahte reklamla denenebilir.
- 🟡 Gerçek reklam gösterimi iPhone'da denenmedi (anahtar gelince TestFlight).

## 4. İzin, gizlilik, Apple kuralları
- ✅ Onay: Google UMP formu (AB/İngiltere'de; Türkiye'de görünmeyebilir), sonra ATT (Google'ın önerdiği sıra). UMP reklama izin vermezse reklam istenmez. **AdMob panelinde Gizlilik ve mesajlar → GDPR mesajı oluşturulmalı** (yoksa AB'de form çıkmaz).
- ✅ ATT sorusu ilk açılışta değil: reklam katmanı 2. bölüm kazanıldıktan ve profil sorusu kapandıktan sonra başlıyor (UMP → ATT → SDK). Bu yüzden 1–2. bölümde reklam yok.
- ✅ `NSUserTrackingUsageDescription`: Info.plist'te EN, `tr.lproj/InfoPlist.strings` içinde TR. `CFBundleLocalizations` tr, en.
- ✅ `GADApplicationIdentifier` ve Google'ın `SKAdNetworkItems` listesi: eklentinin iOS işlemi yazıyor; `IosBuild` build sırasında kontrol ediyor (yoksa durur).
- ✅ `PrivacyInfo.xcprivacy` bizim (cihaz kimliği, ürün etkileşimi, çökme; izleme yok) + Google Mobile Ads SDK'nın kendi manifesti.
- ✅ `ITSAppUsesNonExemptEncryption = false`.
- ✅ Gizlilik sayfası TR + EN güncellendi: Google AdMob (Google'ın veri kullanımı linki, AB onay formu), IDFA (izinle), IDFV, Supabase, KVKK, silme e-postası, kullanım koşulları (`#terms`).
- 🟡 `app-ads.txt`: `zeydusht.github.io` köküne konacak (`docs/APP_ADS_TXT.md`); Zeyd'in kök deposu açması gerekiyor.

## 5. Telemetri `sl1`
- ✅ Mağaza build'i: `cohort = sl1`, `platform = ios`, `app_version`, `content_version`; cihaz kimliği IDFV.
- ✅ Olaylar: `first_open`, `session_start` / `session_end` (süre), `att_status`, `ad_shown` (yerleşim, ağ, gelir), `ad_reward`, `ad_failed`, `ad_skipped_not_ready`, `crash` (mesaj + 5 satır yığın).
- ✅ Kuyruk: çevrimdışı bekler, her satırda cihaza kaydedilir, kapanışta kaybolmaz (`SoftLaunchTests`).
- ✅ SQL `docs/sql/019_soft_launch.sql` yazıldı, **uygulanmadı**:
  - tablo `sl_events`, `continue_events.continue_type`;
  - görünümler `v_sl_tutma`, `v_sl_huni`, `v_sl_reklam`, `v_sl_gunluk`;
  - Zeyd'in cihazları ve test adları dışarıda (013 kuralı).
- ❌ PM, Zeyd'in onayıyla SQL'i uygulayacak. Uygulanmadan `sl_events` satırları kuyrukta bekler.

## 6. 100 bölüm
- ✅ Menü, sandık eşikleri (3, 5, 10, 15 … 100), Jiggle sayacı, profil, tablo listesi 100 bölümü taşıyor (`Sl100Tests`).
- ✅ 100 bitince "Yeni tablolar yolda!" ekranı: Jigo, toplam kurtarılan Jiggle, Menü.
- ✅ PM_82: harita oyuncuya kapalı, düz ilerleme; bitmiş bölüme dönülmez (test linki hariç).
- ✅ sl100 tasarlandı: model v2 ile, 100 bölüm hedef bantta (`docs/analiz/PM82_100_BOLUM.md`).
- 🟡 Tablolar: 1–80 hazır; 81–100 Codex'te. Eksik olanlar 1–40'tan birini gösterir.

## 7. iOS build ayarları
- ✅ 1.0.0 (8), yalnız iPhone, yalnız dikey, 60 fps, onaylı ikon ve açılış ekranı, Unity açılış logosu kapalı (Unity 6 lisansı izin veriyor).
- 🟡 Ayarlar `IosBuild` içinde: 1.0.0 (8), yalnız iPhone, dikey, logo kapalı, Info.plist ve gizlilik manifesti. Testler geçti.
- ❌ Bu işte iOS dışa aktarımı (Xcode projesi) çalıştırılmadı; PM_84'te zip ile birlikte üretilecek.
- 🟡 Mac'te Google Mobile Ads pod'ları için `pod install` gerekecek, sonra `.xcworkspace` açılacak (`rs-yukle.sh` buna göre güncellenmeli).

## Apple'ın istedikleri (inceleme)
- ✅ ATT metni ve izin akışı; reddedilirse de oyun ve reklam çalışıyor.
- ✅ Gizlilik politikası linki (uygulamada Ayarlar → Gizlilik; mağazada aynı link).
- ✅ Şifreleme beyanı (yok).
- ❌ App Store Connect'te gizlilik etiketleri, yaş sınırı, ülke (aşağıda Zeyd'in adımları).

## Zeyd'in App Store Connect'te yapacakları (basit adımlar)
1. **Uygulama Gizliliği → Başla**
   - "Veri topluyor musunuz?" → **Evet**.
   - **Tanımlayıcılar:**
     - Cihaz Kimliği: analiz + uygulama işlevi, kimliğinize bağlı değil, izleme yok.
     - Reklam Verileri / Reklam Kimliği: üçüncü taraf reklam, **izleme: evet** (Google AdMob).
   - **Kullanım Verileri:** Ürün Etkileşimi (analiz); Reklam Verileri (üçüncü taraf reklam).
   - **Tanılama:** Çökme Verileri (uygulama işlevi).
   - Gizlilik politikası adresi: `https://zeydusht.github.io/reverse-solver-unity/privacy/`.
2. **Yaş Derecelendirmesi:** bütün sorulara "Yok". Reklam içerdiği sorulursa "Evet". Sonuç 4+ olmalı.
3. **Fiyatlandırma ve Kullanılabilirlik:** Ücretsiz; ülkeler: **yalnız Türkiye**.
4. **Sürüm 1.0.0:** "Elle yayınla" seçili. Ekran görüntüleri ve açıklama PM_84'te gelecek.
5. ✅ **AdMob:** App ID, Rewarded ID, Interstitial ID ve yayıncı kimliği geldi (`docs/ADMOB_KIMLIKLER.md`). AdMob'un ödeme incelemesi ve uygulamanın mağaza linkiyle eşlenmesi bekleniyor.
