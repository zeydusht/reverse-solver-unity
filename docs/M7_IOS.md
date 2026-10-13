# M7 — iOS uygulaması ve TestFlight (App Store'a giden yol)

Yazan: Zeyd'in PM'i (claude.ai), 2026-10-13 18:00. Zeyd'in isteği: "artık publish edelim App Store'a".

## Hedef ve sıra

**Bu milestone'un hedefi:** Oyun, Zeyd'in iPhone'unda **TestFlight** üzerinden gerçek bir uygulama olarak çalışsın. Herkese açık App Store yayını bu milestone'un dışında; gerekçesi aşağıda.

1. **Önce telefon testi.** Build 2'nin telefon testi (`docs/TELEFON_TESTI.md`) ve ardından Build 3 her şeyin önünde gelir; veri kalitesini etkileyen bir hata çıkarsa M7 bekler. M7'nin Zeyd'den bağımsız hazırlık işleri bu arada başlayabilir.
2. **Web sürümü çalışmaya devam eder.** WebGL build ve playtest yolu (GitHub Pages) hiçbir adımda bozulmaz. iOS ikinci bir build hedefi olarak eklenir.
3. **Neden önce yalnızca TestFlight (PM kararı):**
   - Playtest (Faz 3) henüz yapılmadı.
   - 40 seviyenin zorluk eğrisinde bilinen sorunlar var: 1–10 seviyelerinde kazanma %98, 11. seviyede süre 115 sn'den 45 sn'ye düşüyor, 21–30 seviyelerinde ilk denemede kazanma %36.
   - Herkese açık yayın, Faz 4'teki zorluk düzeltmesinden ve mağaza hazırlığından sonra gelir. Önerilen sıra: önce yalnızca Türkiye'de soft launch, sonra dünya geneli. Karar Zeyd'in.
   - TestFlight'ın dahili testi App Review beklemez. Zeyd ve playtest grubu bu sayede oyunu gerçek uygulama olarak oynar.

## Bilinen dış şartlar (2026-10-13 itibarıyla)

| Şart | Ayrıntı |
|---|---|
| Xcode ve SDK | App Store Connect'e yüklenen uygulamalar 28 Nisan 2026'dan beri **Xcode 26 ve iOS 26 SDK** ile derlenmiş olmalı. |
| Mac | Windows'ta Xcode yok. Unity Windows'ta Xcode projesi üretebilir, ama derleme, imzalama ve yükleme macOS'ta yapılır. Bunun için **GitHub Actions macOS runner'ı** kullanılacak; herkese açık repoda standart runner'lar ücretsiz. |
| Apple Developer Program | Yıllık 99 $. Bireysel kayıtta App Store'daki satıcı adı Zeyd'in adıdır. Kaydı yalnızca Zeyd yapabilir. |
| Ekran görüntüleri | Herkese açık yayın için 6,9" dikey set zorunlu: 1320×2868 (ya da 1290×2796 / 1260×2736), 1–10 adet. |
| Yaş derecelendirmesi | Yeni sistemde 4+, 9+, 13+, 16+ ve 18+ var. Anketteki yeni sorular zorunlu. |

## Mühendislik işleri

**I1. iOS hedefi (Unity)**
- Unity'ye **iOS Build Support** modülünü ekle. Unity Hub'ın komut satırı kurulumu yeterliyse sen yap; Zeyd'in onayı gerekirse sabah listesine yaz.
- Player ayarları:
  - yalnızca dikey yön,
  - IL2CPP, aynı `link.xml`,
  - **`Application.targetFrameRate = 60`** (iOS varsayılanı 30 FPS),
  - minimum iOS sürümü: Unity 6.6'nın desteklediği en düşük sürüm,
  - bundle ID: Zeyd'in kararı (öneri `com.zeydtekin.reversesolver`; kayıttan sonra değiştirilemez).
- `ITSAppUsesNonExemptEncryption = false` (yalnızca HTTPS kullanılıyor).
- Geçici bir uygulama ikonu ve açılış ekranı. Asıl ikon mağaza hazırlığında yapılır.

**I2. iOS platform katmanı (`Platform/`)**
- Web'e özel `Host`'un iOS karşılığı:
  - **Güvenli alan:** `Screen.safeArea`'dan nokta cinsinden hesaplanır. Çentikli ve Dynamic Island'lı ekranlarda doğru olmalı.
  - **Piksel oranı:** noktaya doğru çevrilmeli.
  - **Kayıt:** PlayerPrefs.
  - **Görünürlük:** `OnApplicationPause` / `OnApplicationFocus`. Kuyruk uygulama arka plana geçerken gönderilir.
- **İsim kapısı:** HTML kutusunun yerine oyun içinde bir giriş alanı (TMP ve klavye). TestFlight playtest'i için kalır. Herkese açık yayında isim kapısı kalkıp anonim kurulum kimliğine geçilecek mi, bu bir PM kararı; şimdi değil.
- URL parametreleri yok.
  - **Debug:** Development build'de ya da gizli bir hareketle (ör. logoya 5 dokunuş) açılır.
  - **Tasarım seti:** yalnızca debug'da.
- **Telemetri:** `client = 'unity-ios'`. Gönderim, kuyruk ve `TEST_` kuralları web'le aynı. Supabase şemasına dokunulmaz; `client` değeri zaten serbest metin, değilse PM'e sor.
- Core'a hiçbir şey eklenmez. Bağımlılık yönü aynı kalır: Core ← Presentation ← Platform.

**I3. Bulut build ve TestFlight hattı (GitHub Actions, yalnızca elle tetiklenir)**
1. Unity'den iOS Xcode projesi üret: game-ci ya da Windows'ta batchmode ile, sonra artifact olarak aktar.
2. macOS runner'ında Xcode 26 ile archive al.
3. App Store Connect API anahtarıyla imzala ve TestFlight'a yükle (fastlane ya da xcodebuild ile).

- Sırlar yalnızca GitHub Secrets'ta durur: API anahtarı (.p8), Key ID, Issuer ID ve gerekirse sertifika parolası. **Repoya ve sohbete hiçbir sır girmez.**
- Önce repo herkese açık mı kontrol et. Değilse macOS dakikaları ücretli olur; PM'e sor.
- Build numarası her yüklemede otomatik artar.
- `docs/IOS_BUILD.md`'de hattı tek sayfada anlat: Zeyd neyi nereye ekler, nasıl tetikler, hata olursa ne getirir.

**I4. Mağaza ve politika taslakları (Zeyd onaylar, yüklemez)**
- **Gizlilik politikası:** GitHub Pages'te `privacy.html`, Türkçe ve İngilizce.
  - Neler toplanıyor: oyuncunun yazdığı takma ad, bölüm denemeleri, süreler, istemci türü.
  - Nerede tutuluyor ve ne için: Supabase'de, seviye tasarımı için.
  - Ne kadar süre saklanıyor ve nasıl silinir.
  - Reklam ve izleme yok.
  - İletişim e-postasını Zeyd verir; yer tutucu bırak.
- **App Privacy cevap taslağı:** hangi veri türü, kullanıcıya bağlı mı, izleme var mı. Belirsiz kalan yerlerde temkinli beyanı seç ve gerekçesini yaz.
- **Yaş derecelendirme anketi:** cevap taslağı.
- **Mağaza metni taslağı (TR ve EN):** ad adayları (App Store'da ad benzersiz olmalı; 3 yedek ad), alt başlık, açıklama, anahtar kelimeler.
- **Ekran görüntüsü üretimi:** RenderShot ya da başsız Chrome ile 1320×2868 (440×956 pt × 3). L01, bir orta seviye, bir engel tanıtımı ve "Resmi tamamladın" kartı. Bu taslak; asıl set mağaza hazırlığında.

## Kabul ölçütleri (M7)

1. Web build'i değişmeden çalışır: `verify_build.js` aynı sonuçları verir, EditMode testleri yeşil kalır.
2. iOS Xcode projesi üretilir. Bulut hattı archive'ı alır ve TestFlight'a yükler; yükleme App Store Connect'te "işlendi" durumuna geçer.
3. Zeyd'in iPhone'unda TestFlight'tan kurulan uygulamada:
   - 40/40 seviye okunuyor (debug açılış kontrolü),
   - güvenli alan doğru (çentik ya da Dynamic Island ve ana ekran çubuğu),
   - sürüklerken 60 FPS,
   - isim girilebiliyor,
   - uygulama kapatılıp açılınca isim ve ilerleme duruyor,
   - L01'de el görünüyor,
   - kenar parçaları rahat çıkıyor.
4. Supabase'de `client = 'unity-ios'` ve `TEST_ZEYD` olan satır görünür. Debug oturumu satır göndermez.
5. I4'teki taslaklar `docs/store/` altında, Zeyd'in onayını bekler halde durur.

## Zeyd'in yapacakları (M7 için)

1. **Apple Developer Program'a bireysel kayıt** (99 $/yıl). Apple hesabında iki adımlı doğrulama açık olmalı. Onay genelde bir gün içinde gelir.
2. **Bundle ID ve uygulama adı kararı:** öneri `com.zeydtekin.reversesolver`.
3. **Onay gelince:**
   - App Store Connect'te uygulamayı oluştur.
   - App Store Connect API anahtarı oluştur (Kullanıcılar ve Erişim → Entegrasyonlar).
   - .p8 dosyasını, Key ID'yi ve Issuer ID'yi GitHub repo → Settings → Secrets'e ekle. **Sohbete yapıştırma.**
4. **iPhone'a TestFlight uygulamasını kur.** Kendini App Store Connect'te dahili test kullanıcısı olarak ekle.
5. **Gizlilik politikası için bir iletişim e-postası** ver.
