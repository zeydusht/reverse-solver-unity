# PM_MESAJ_101: Reklam sağlayıcısı AppLovin MAX → Google AdMob

Kodu kendin yaz. Önce `docs/` commit. Bitiş mesajı CLAUDE.md biçiminde, en altta, "Oyna" linkleriyle. `live/`'a yayınlama.

**PM_100 bitmediyse önce onu bitir.**

## Neden
AppLovin, Zeyd'in başvurusunu anında reddetti: "We're currently not accepting new publisher accounts." MAX kullanılamıyor.

**Yeni sağlayıcı: Google AdMob** (Google Mobile Ads Unity eklentisi). Bireysel hesapla açılabiliyor. Mediation ile ileride Unity Ads, Mintegral ve AppLovin (bidding) eklenebilir. Şimdi yalnız AdMob.

## 1. Değişiklik
- AppLovin MAX eklentisini ve bağımlılıklarını **kaldır** (SDK, SKAdNetwork listesinin MAX'e özgü kısmı, MAX'in gizlilik/terms akışı).
- **Google Mobile Ads Unity eklentisi** ekle (güncel kararlı sürüm, External Dependency Manager ile).
- **Reklam katmanı:** oyun kodu sağlayıcıdan habersiz olsun (`IAdProvider` gibi tek arayüz). Yerleşimler, kurallar ve telemetri **PM_81 / PM_82 ile birebir aynı**:
  - R1: süre doldu +30 sn;
  - R2: 2× altın;
  - R3: booster +1;
  - R4: menü hediyesi;
  - I1: 12. bölümden sonra, 3 bölümde ve 120 sn'de bir.

  `live/config.json` aynen.
- **Anahtarlar:** `Assets/Config/ads.json` → AdMob **App ID (iOS)**, **Rewarded ID**, **Interstitial ID**. Zeyd'in gerçek ID'leri gelene kadar **Google'ın resmî test ID'leri** kullanılsın (test reklamı gösterir; mağaza build'inde test ID kalırsa build uyarı versin ve dursun).
- `Info.plist`: `GADApplicationIdentifier`, Google'ın `SKAdNetworkItems` listesi, `NSUserTrackingUsageDescription` (aynı metin).
- **İzin akışı:**
  - ATT 2. bölümden sonra (aynı).
  - **Google UMP (User Messaging Platform)** onay akışı: AB/İngiltere kullanıcıları için gerekli. Türkiye'de görünmeyebilir ama kod hazır olsun.
  - UMP'den sonra ATT, Google'ın önerdiği sırada.
- **Gizli test menüsü** (sürüm yazısına 5 dokunuş): Google **Ad Inspector**.
- `PrivacyInfo.xcprivacy`: Google Mobile Ads'in kendi manifesti + bizimki.
- **Gizlilik sayfası** (`/privacy/`) TR + EN: "AppLovin" geçen yerler "Google AdMob" olsun. Google'ın veri kullanımı linki eklensin.
- **app-ads.txt:** AdMob, geliştirici sitesinde `app-ads.txt` ister. `zeydusht.github.io` kökü için içerik hazırla. Satır Zeyd'in yayıncı kimliğiyle (`pub-…`) doldurulacak, şimdilik yer tutucu.
  - **Repo oluşturma**, yalnız dosyayı ve adımları hazırla: `docs/APP_ADS_TXT.md`.
- **Telemetri:** `ad_shown` gelir alanı AdMob'un `OnAdPaid` değerinden (mikro birim → dolar).
- **Web (WebGL):** reklam yok (aynı).

## 2. Test
- EditMode + PlayMode yeşil. Reklam katmanı için sahte sağlayıcıyla mevcut testler (sıklık, hazır değilken tuş gizli) aynen geçsin.
- iOS build ayarları derlenebilir. Xcode projesi üretilsin ama zip/yükleme yok.
- `docs/SOFT_LAUNCH_KONTROL.md` güncel: AppLovin satırları → AdMob.

## Teslim
- `verify_build` p23 4/4 (web etkilenmemeli).
- `PROJE_DURUMU`'na tek satır, commit + push.
- Son mesaj kısa: ne değişti, Zeyd'den beklenen 3 bilgi (App ID, Rewarded ID, Interstitial ID) ve `pub-` kimliği.
