# PM_MESAJ_61 EKİ — iOS yüklemesi: geçmiş hatalar + tek betik (PM_61 ile birlikte uygula)

## EK (ZORUNLU): 0.5.1 yüklemesinde yaşanan BÜTÜN hatalar, bu sefer baştan çözülecek
Geçen sefer Mac başında ~3 saat kaybettik. Zeyd yarın **tek seferde** yüklemek istiyor. Kaynak: o günkü PM sohbeti + `docs/MAC_YUKLEME.md`. Mac: arkadaşın Mac'i, kullanıcı klasörü `/Users/keremengin`; Apple Distribution sertifikası ve anahtarı o Mac'in Keychain'inde; `RS_AppStore` App Store profili oluşturulmuştu (gerekirse yeniden indirilir).

Bilinen hatalar ve çözümleri:
1. **`usymtoolarm64` / `process_symbols.sh`: Permission denied.** Windows'ta oluşturulan zip çalıştırma izinlerini kaybediyor. Çözüm: `chmod +x process_symbols.sh usymtool*` ve `xattr -cr .`.
2. **`UnityRuntime.framework`'te eski `_CodeSignature` ve çalıştırma izni.** Çözüm: `rm -rf Frameworks/UnityRuntime.framework/_CodeSignature` ve `chmod +x Frameworks/UnityRuntime.framework/UnityRuntime`. Mümkünse Windows tarafında hiç oluşmasın.
3. **ITMS-90035 Invalid Signature: "TEKİN"deki İ harfi.** Sertifika adı `TEK\C4\B0N` (tek parça İ), ama codesign'ın yazdığı designated requirement `I + U+0307` (`49cc87`). Çözüm `imza-duzelt.sh` mantığı: her framework ve uygulama için requirement'ta `49cc87` → `c4b0` değiştirip `codesign -f --requirements` ile yeniden imzala.
4. **"Uygulama, profildeki sertifikayla imzalanmalı".** Xcode'un otomatik profili Mac'teki sertifikayı içermiyordu. Çözüm `imza-duzelt-2.sh`: Downloads'taki `RS_AppStore.mobileprovision`'ı `embedded.mobileprovision` yap, entitlements'ı profilden al, profildeki ve Mac'te anahtarı olan Apple Distribution ile imzala.
5. **Yükleme:** `xcrun altool --upload-app` + uygulamaya özel parola (adı "Transporter"). İş bitince parola silinir.
6. Keychain şifre kutuları: Mac giriş şifresi + **Always Allow**.
7. **Şifreleme (export compliance) sorusu:** Info.plist'e `ITSAppUsesNonExemptEncryption = false` yaz, ASC'de soru çıkmasın.
8. Build numarası: 7 (0.5.2 (6) hiç yüklenmedi; 7 sorun değil).

İstenen çıktı:
- **Tek betik `Builds/rs-yukle.sh`** (zip'in içinde de olsun). Zeyd Mac'te yalnızca şunu yapsın: zip'i Downloads'a indir → Terminal'e tek satır (`bash ~/Downloads/rs-yukle.sh`) → betik parolayı **gizli girişle sorsun** (`read -s`; Apple ID e-postası betikte hazır olsun, Zeyd yazmasın).
- Betik sırayla, her adımda "ADIM n/…: …" yazarak ve hata olursa açık Türkçe mesajla dursun: unzip → 1 ve 2 düzeltmeleri → `xcodebuild archive` (Any iOS Device, Release, `-allowProvisioningUpdates`) → `xcodebuild -exportArchive` (ExportOptions.plist hazır, method app-store-connect) → .ipa'yı aç → 4 (profil + entitlements) ve 3 (İ düzeltmesi) → `codesign --verify --deep --strict` kontrolü ("satisfies its Designated Requirement" görmeden yüklemesin) → zip → `xcrun altool --upload-app` → "BITTI".
- Bir adım başarısız olursa betik **o adımdan devam** edebilsin (`bash rs-yukle.sh --from 5`).
- `MAC_ADIMLAR_0.6.0.txt`: yalnız 5–6 satır: (1) Xcode'da hesap ekli mi kontrol (Settings → Accounts), (2) RS_AppStore profili Downloads'ta yoksa nasıl indirilir, (3) uygulamaya özel parola nasıl alınır, (4) tek komut, (5) bitince parolayı sil + Xcode'dan çıkış yap. Yedek olarak eski elle yöntem en altta.
- Betiği Windows'ta yazdığın için satır sonları **LF** olsun (CRLF bash'i bozar), kontrol et.
