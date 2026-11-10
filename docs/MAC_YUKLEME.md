# Reverse Solver'ı Mac'ten TestFlight'a yükleme

Bu rehber, teknik bilgisi olmayan biri içindir. Toplam süre: ilk seferde yaklaşık 1–1,5 saat (çoğu indirme ve bekleme).

## Gerekenler

- Bir **Mac** (arkadaşının Mac'i olabilir). macOS güncel olmalı.
- **Xcode**: Mac'teki App Store'dan ücretsiz indirilir. App Store'daki **en güncel sürümü** kur (eski sürümle yapılan yüklemeleri Apple kabul etmeyebilir). İndirme 10–15 GB, 30–60 dakika sürebilir.
- **Apple Developer hesabı** (yıllık ücretli). Hesap Zeyd'in Apple ID'si ile açılmış olmalı.
- Zeyd'in gönderdiği dosya: **ReverseSolver-iOS-0.5.0.zip**

## 1. Uygulamayı App Store Connect'te bir kez oluştur (yalnız ilk sefer)

1. Tarayıcıda **appstoreconnect.apple.com** adresine gir, Zeyd'in Apple ID'si ile oturum aç.
2. **Uygulamalar** → sol üstteki **+** → **Yeni Uygulama**.
3. Şunları doldur:
   - Platform: **iOS**
   - Ad: **Reverse Solver** (alınmışsa "Reverse Solver Puzzle" gibi bir ad dene)
   - Birincil dil: **Türkçe**
   - Paket kimliği (Bundle ID): listede **com.zeydtekin.reversesolver** yoksa önce aşağıdaki "Bundle ID kaydı" adımını yap.
   - SKU: **reversesolver** (kendine özel bir yazı, kimse görmez)
4. **Oluştur**'a bas.

**Bundle ID kaydı** (gerekirse): **developer.apple.com** → Account → **Certificates, IDs & Profiles** → **Identifiers** → **+** → **App IDs** → **App** → Description: *Reverse Solver*, Bundle ID: **Explicit** → `com.zeydtekin.reversesolver` → Continue → Register. Sonra App Store Connect'e dönüp 3. adımı tamamla.

## 2. Xcode'a Apple hesabını ekle (yalnız ilk sefer)

1. Xcode'u aç.
2. Üst menü: **Xcode → Settings… → Accounts**.
3. Sol alttaki **+** → **Apple ID** → Zeyd'in Apple ID'si ve şifresi.
4. Hesap listede görününce pencereyi kapat.

## 3. Projeyi aç

1. **ReverseSolver-iOS-0.5.0.zip** dosyasına çift tıkla; **ReverseSolver-iOS-0.5.0** klasörü çıkar.
2. Klasörün içinde **Unity-iPhone.xcodeproj** dosyasına çift tıkla. Xcode açılır.
   - Yanında *Unity-iPhone.xcworkspace* diye bir dosya varsa **onu** aç.
3. İlk açılışta Xcode birkaç dakika "Indexing / Processing" yazabilir; bitmesini bekle.

## 4. İmzalama (Signing)

1. Sol taraftaki dosya listesinde en üstteki mavi **Unity-iPhone** simgesine tıkla.
2. Ortadaki bölümde **TARGETS** altında **Unity-iPhone**'u seç.
3. Üstteki sekmelerden **Signing & Capabilities**'e tıkla.
4. **Automatically manage signing** kutusu işaretli olsun.
5. **Team**: açılır listeden Zeyd'in ekibini seç (ad soyad ya da şirket adı).
6. **Bundle Identifier** `com.zeydtekin.reversesolver` yazıyor olmalı. Değiştirme.
7. Kırmızı hata yazısı kalmadıysa bu adım tamam. Hata varsa birkaç saniye bekle; Xcode sertifikayı kendisi oluşturur.
   - TARGETS listesinde **UnityFramework** da varsa: ona tıkla, aynı sekmede Team'i yine seç.

## 5. Arşiv al (Archive)

1. Xcode penceresinin en üstündeki cihaz seçiminden **Any iOS Device (arm64)**'ü seç (bir iPhone simülatörü değil).
2. Üst menü: **Product → Archive**.
3. Bekle: 5–20 dakika sürebilir. Bitince **Organizer** penceresi kendiliğinden açılır ve listede bugünün tarihli bir arşiv görürsün.
   - Hata çıkarsa ekranın fotoğrafını çekip Zeyd'e gönder.

## 6. Yükle (Upload)

1. Organizer'da arşivi seç → sağdaki **Distribute App**.
2. **App Store Connect** (ya da **TestFlight & App Store**) → **Distribute** / **Next**.
3. Sorulan ekranlarda varsayılanları bırak, **Next / Upload**'a bas.
4. "Upload Successful" yazınca bitti. Pencereyi kapatabilirsin.

## 7. TestFlight'ta görün (Zeyd yapar)

1. 10–30 dakika sonra App Store Connect → uygulama → **TestFlight** sekmesinde sürüm **0.5.0 (1)** görünür ("Processing" bitince).
2. "Missing Compliance" (şifreleme sorusu) çıkarsa: **Manage** → *Uygulama standart şifreleme dışında şifreleme kullanmıyor* seçeneği (**None of the algorithms mentioned above**) → Kaydet.
3. **Internal Testing** → **+** → test edecek kişileri ekle. Onların iPhone'una **TestFlight** uygulaması üzerinden davet gelir.
4. Dış test (External Testing) için Apple'ın kısa bir incelemesi gerekir; gizlilik politikası adresi istenir:
   `https://zeydusht.github.io/reverse-solver-unity/privacy/`

## 0.6.0 (7) ve sonrası: tek betik (PM_MESAJ_61)

Elle Xcode adımları yerine `Builds/rs-yukle.sh` kullanılır (zip'in içinde de var). Mac'te: zip ve betik Indirilenler'e → `bash ~/Downloads/rs-yukle.sh` → parola gizli girişle sorulur. Betik 10 adımda: zip'i açar, izinleri ve eski `_CodeSignature`'ı düzeltir, archive + export alır, .ipa'yı `RS_AppStore` profili ve profildeki Apple Distribution sertifikasıyla yeniden imzalar, "TEKİN"deki İ için designated requirement'ı düzeltir (`49cc87` → `c4b0`), `codesign --verify --deep --strict` geçmeden yüklemez, `xcrun altool --upload-app` ile yükler. Hata olursa Türkçe mesajla durur; `--from N` ile o adımdan devam eder. Kısa talimat: `Builds/MAC_ADIMLAR_0.6.0.txt`. Zip Windows'ta Unix izinleriyle ve `_CodeSignature` olmadan üretilir (0.5.1'deki 1. ve 2. hata baştan önlenir).

## Sonraki yüklemeler

Zeyd yeni bir zip gönderdiğinde yalnız **3, 5 ve 6. adımlar** tekrarlanır. Her yeni yüklemede build numarası Zeyd'in tarafında artırılmış olur; aynı numarayla iki kez yüklenemez.

**0.5.1 (build 3), 29 Ekim:** `Builds/ReverseSolver-iOS-0.5.1.zip`. Mac'te eski `ReverseSolver-iOS-0.5.0` klasörünü sil, yeni zip'i aç, sonra yalnız **3, 5 ve 6. adımlar**. 1, 2 ve 4. adımlar ilk seferde yapıldıysa tekrar gerekmez; 4'te Team seçili değilse yeniden seç. Sürüm 0.5.1, build 3 projede hazır, Xcode'da değiştirme.
Bu sürümde: 40 tablo (internetten iner, telefona kaydedilir), İngilizce varsayılan + Türkçe, Ayarlar (ses, müzik, titreşim, dil), yeni kart tuşları ve isim girişi, kalın duvarlar, tek tutorial eli, ilerleme kaydı + isimle geri getirme, arka plan müziği, renkli zincirler, titreşim. Bölümler GitHub Pages'teki onaylı SET40 v3'ten gelir.
Bundan sonraki her zip'te sürüm ya da build numarası bir artar (0.5.2 / 3 …); App Store Connect aynı numarayı iki kez kabul etmez.

## Sorun çıkarsa

- "No account for team" / "Signing requires a development team": 2. ve 4. adımları tekrar kontrol et.
- "Bundle identifier is not available": 1. adımdaki Bundle ID kaydı yapılmamış.
- Archive menüsü gri: üstte cihaz olarak **Any iOS Device** seçilmemiş.
- Başka her durumda: ekranın fotoğrafını Zeyd'e gönder.
