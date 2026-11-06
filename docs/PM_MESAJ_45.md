# PM_MESAJ_45 — iOS: 60 FPS + titreşim (TestFlight 0.5.1 (3) bulguları)

Zeyd, 0.5.1 (3)'ü TestFlight'tan iPhone'una kurdu (5 Kasım). İki sorun var. Kod değişikliği küçük; yeni iOS build bunlarla çıkacak.

## 1. Oyun iPhone'da 30 FPS (en kritik)
- Kanıt: 3 ekran kaydı (1170x2532, 120 Hz kayıt). Kare kare farka baktım: oyun içinde, menüde, sürüklerken **her 4 kayıt karesinde 1 yeni görüntü = sabit 30 FPS**. Web'de (tarayıcı rAF) 60, bu yüzden web'de sorun yok.
- Sebep (büyük olasılıkla): projede `Application.targetFrameRate` hiç ayarlanmamış (GameRoot/NativeHost'ta yok). Unity iOS'ta varsayılan **30**.
- Yapılacak:
  1. Önce tüm projede `targetFrameRate` / `vSyncCount` ara; başka yerde 30'a çeken bir şey varsa bana bildir.
  2. `NativeHost.Install()` (BeforeSceneLoad, yalnız iOS) içine: `QualitySettings.vSyncCount = 0; Application.targetFrameRate = 60;`
  3. Uygulama arka plandan dönünce de (OnApplicationPause(false)) aynı değeri tekrar ver.
  4. 120 Hz (ProMotion) şimdilik YOK: pil ve ısınma. Info.plist'e `CADisableMinimumFrameDurationOnPhone` ekleme.
  5. Web build'e dokunma (web'de targetFrameRate ayarlama, tarayıcıya bırak).

## 2. Titreşim hissedilmiyor
- Durum: `RSHaptics.mm` build'de var (Builds/iOS/Libraries/Plugins/iOS), ayar "Vibration: On". Ama `Haptics.Play` yalnızca **jam, bomba, kazanma**'da çağrılıyor. Normal parça çıkarmada, parçayı tutmada, düğmelerde hiç titreşim yok → oyuncu "titreşim yok" diye algılıyor.
- Yapılacak (hepsi mevcut `Haptics.Play` üzerinden, ayardan kapatılabilir kalsın):
  1. Parça başarıyla çıkınca: hafif dokunuş (Light). Bu oyunun ana hissi.
  2. Parçayı sürüklemeye başlayınca: çok hafif "seçim" tıkı → `RSHaptics.mm`'e `kind == 3` için `UISelectionFeedbackGenerator selectionChanged` ekle, `Haptics.Kind.Select = 3`.
  3. Jam: mevcut Light yerine **Medium** (hata hissi farklı olsun) → `kind == 4` Medium ekle.
  4. Bomba Heavy, kazanma Success: aynen kalsın.
  5. Menü düğmeleri: Select (çok hafif). Aynı karede üst üste titreşim olmasın (en az 50 ms aralık).
  6. Jeneratörleri her seferinde yeniden yaratma: statik tut, `prepare` çağır (gecikme olmasın).

## 3. Build
- Sürüm **0.5.2 (5)**. p20 değişiklikleri (yeni menüler, eğitim, puanlama kaldırıldı) dahil olsun; seviyeler uzaktan (kök `levels/redesign.json`) — kökü ben söylemeden güncelleme.
- iOS zip'i `Builds/ReverseSolver-iOS-0.5.2.zip` olarak hazırla. Mac'te yükleme İ imza betikleriyle yapılacak (MAC_YUKLEME.md).
- Bitince `PROJE_DURUMU.md` iOS bölümünü güncelle.

## Kabul
- iPhone kaydında oyun içi ve menüde 60 yeni kare/sn.
- Parça çıkarınca, sürüklemeye başlayınca, jam/bomba/kazanmada titreşim hissediliyor; ayardan kapatınca hiçbiri yok.
