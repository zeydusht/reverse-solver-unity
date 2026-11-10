# PM_MESAJ_61 — ACİL: iOS build (TestFlight 2) bu gece, PM_58 hâlinden

Zeyd'in Mac'li arkadaşı **yarın 10 günlüğüne gidiyor**. Yarın sabah Mac'te yüklenecek bir iOS build bu gece hazır olmalı. PM_60 bekleyebilir.

## 0. PM_60'ı güvenli noktada durdur
- Çalışıyorsan bulunduğun alt işi bitir ya da yarıda bırak; değişiklikleri ayrı bir dalda **WIP commit** et (`pm60-wip`), push et. PM_60'a bu işten sonra kaldığın yerden devam edeceksin.

## 1. Kaynak: PM_58'in yayınlanmış hâli
- `art` dalında PM_58'in son commit'i (p21 gh-pages `db178a29` ile aynı içerik). İçinde: yeni HUD (M1), Jigo hâlleri, Durdur, kilit (Kont mührü, düşüş), Verlet zincir, bomba nabzı, fiske + kenar payı, 5000 altın + devam (900/1900/2900), intro video + ses C, kazanma Jigo'su.
- **PM_59 tutorial'ı girmesin** (Zeyd beğenmedi). Eğer PM_59 değişiklikleri `art`'ta PM_58'in üstündeyse PM_58 commit'ine dön.

## 2. iOS'a özel ayarlar
- Sürüm **0.6.0 (7)**, bundle `com.zeydtekin.reversesolver`, Team ID 9X55V64NRR.
- Seviyeler ve tablolar: iOS build p21'e **bağlanmasın** (p21 her gün değişiyor). Bugünkü p21 seviye/tablo dosyalarını Pages'te **dondurulmuş** bir klasöre kopyala (ör. `/ios7/`) ve iOS build oradan okusun. Kök ve p20'ye dokunma.
- Intro videoları: iOS'ta nasıl oynuyor? (Web'de HTML `<video>`.) iOS'ta çalışmayacaksa build'e gömülü oynat ya da bu sürümde intro'yu kapat ve raporla. Çökme riski alma.
- Telemetri: `platform='ios'`, cohort `tf2`. 60 FPS ve titreşim haritası (PM_45/46) korunsun.
- Fiske ve kenar payı iOS dokunuşunda da çalışsın (tarayıcı kodu değil, Unity girişi).

## 3. Build
- Unity iOS build → Xcode projesi → `Builds/ReverseSolver-iOS-0.6.0.zip`.
- EditMode yeşil (Perf hariç). Editörde iOS simülasyon çözünürlüğü (390×844 ve 375×667) ekran görüntüsü: HUD, kazanma, devam paneli.

## 4. Mac adımları (Zeyd yazamaz, kopyala-yapıştır olmalı)
- `docs/MAC_YUKLEME.md`'yi bu sürüme göre güncelle ve **tek bir metin dosyası** üret: `Builds/MAC_ADIMLAR_0.6.0.txt`. Zeyd onu Drive'a koyup Mac'te açacak. İçinde sırayla: zip'i aç, Xcode'da aç, Archive, Export, "İ" imza düzeltme betikleri (`imza-duzelt.sh`, `imza-duzelt-2.sh`), `xcrun altool --upload-app` komutu (uygulamaya özel parola yerini `PAROLA` diye bırak), en sonda parolayı silme ve Xcode'dan çıkış yapma hatırlatması.
- Betikler gerekiyorsa `Builds/` altına koy, zip'e de ekle.

## 5. Sonra
PROJE_DURUMU'na tek satır, commit + push. Ardından `pm60-wip` dalına dön ve **PM_60'a devam et**.
