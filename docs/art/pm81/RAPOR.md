# PM_81 RAPOR: soft launch altyapısı

Madde madde durum: `docs/SOFT_LAUNCH_KONTROL.md`.

## Yapılanlar
- **`live/` (GitHub Pages):** `levels.json`, `config.json`, `paintings/`, `manifest.json` (sürüm + SHA-256).
  - Mağaza build'i yalnız bunu okur. Web'de test için `&kaynak=live`.
  - İlk kopya yayında: PM_81'in yer tutucusu (p23'ün 40'ı + 60 işaretli kopya, o anki tablolar).
  - Yayın komutu: `python Tools/yayin/live_yayinla.py --onay "<PM onayı>"`. Onaysız çalışmaz.
- **Çevrimdışı:**
  - gömülü bölümler ve ayarlar (Resources/Live), iOS'ta 1–20 tabloları;
  - indirilen her şey cihazda önbellekte;
  - açılışta en çok 2,5 sn ağ beklenir;
  - bozuk ya da eksik dosya reddedilir.
  - 3 durum (uçak modu → internet → uçak modu) birim testte.
- **Uzaktan ayar:** reklam sıklıkları, devam fiyatları, başlangıç altını, +30 sn, `reklam_acik`, R1–R4, günlük sınırlar.
- **AppLovin MAX 8.6.6:** yalnız AppLovin ağı.
  - Yerleşimler: R1 (süre doldu +30 sn), R2 (2× altın), I1 (geçiş: 12'den sonra, 3 bölümde bir, 120 sn; ilk oturumun ilk 10 dk'sı ve kayıp sonrası yok). Banner yok.
  - Hazır değilse tuş yok. Reklamda süre durur ve ses kısılır. Ödül yalnız "ödül kazanıldı"da verilir.
  - Gizli test menüsü: Ayarlar'da sürüm yazısına 5 dokunuş.
  - **Anahtarlar yer tutucu** (`Assets/Config/ads.json`).
- **ATT 2. bölümden sonra:** MAX'in şartlar ve gizlilik akışı soruyu SDK başlarken soruyor. SDK bu yüzden 2. bölüm kazanıldıktan ve profil sorusu kapandıktan sonra başlatılıyor. 1–2. bölümde reklam yok.
  - Metin: EN Info.plist'te, TR `tr.lproj`'da.
- **Telemetri `sl1`:** `sl_events` (ilk açılış, oturum, ATT, reklam olayları + gelir, hata). IDFV cihaz kimliği. Çevrimdışı kuyruk.
  - SQL `docs/sql/019_soft_launch.sql` yazıldı, **uygulanmadı**.
- **100 bölüm:** sandık eşikleri, Jiggle sayacı, profil, tablo listesi 100'ü taşıyor; 100'den sonra "Yeni tablolar yolda!".
- **iOS 1.0.0 (8):**
  - yalnız iPhone, dikey, 60 fps;
  - Unity açılış logosu kapalı;
  - `PrivacyInfo.xcprivacy`, `ITSAppUsesNonExemptEncryption = false`.

## Gizlilik sayfası değişiklikleri (`/privacy/`, TR + EN; PM okuyacak)
1. Tarih: 18 Kasım 2026.
2. **Yeni bölüm "Reklamlar (AppLovin)":**
   - ödüllü ve geçiş reklamları;
   - AppLovin'in işlediği bilgiler ve gizlilik linki;
   - IDFA yalnız ATT izniyle; izin Ayarlar'dan değişir; izin yoksa reklam kişiselleştirilmez;
   - web'de reklam yok.
3. **Toplanan bilgiler:**
   - "reklam kimliği / cihaz kimliği toplanmaz" cümlesi kaldırıldı;
   - **IDFV** (uygulamaya özel cihaz kimliği), **hata kayıtları**, oturum süreleri, içerik sürümü eklendi;
   - 👍/👎 ve 1–10 puanları çıkarıldı (artık sorulmuyor).
4. **Paylaşım:** Supabase (oyun verisi + reklam olayları), AppLovin, GitHub Pages (indirme kayıtları).
5. **Yeni bölüm "KVKK":**
   - veri sorumlusu Zeyd Tekin;
   - amaçlar;
   - hukuki sebepler (hizmet, meşru menfaat, IDFA için açık rıza);
   - yurt dışına aktarım (Supabase, AppLovin, GitHub);
   - 11. madde hakları.
6. **Silme isteği:** e-posta + oyuncu adı, 30 gün içinde.
7. **Yeni "Kullanım koşulları"** (`#terms`): ücretsiz, "olduğu gibi", oyun içi altının para değeri yok.

## Doğrulanmayan
- iPhone'da gerçek reklam, ATT penceresi, önbellek davranışı.
- Xcode projesinin Mac'te derlenmesi: MAX CocoaPods ister, `pod install` gerekir.
- `sl_events` satırları: SQL uygulanana kadar kuyrukta bekler.
