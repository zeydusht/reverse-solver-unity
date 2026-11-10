# PM_61 düzeltme — rs-yukle.sh'de 2 hata (PM betiği satır satır okudu)

Betik genel olarak iyi (LF satır sonu, `bash -n` temiz, adımlar ve `--from` doğru). Ama iki kritik hata var; ikisi de 0.5.1 yüklemesinde **çalıştığı kanıtlanmış** yöntemden sapıyor. O gün kullanılan ve çalışan betikler konuşma kaydında: `imza-duzelt.sh` ve `imza-duzelt-2.sh`. Bunları birebir izle.

## Hata 1: ADIM 7, İ düzeltmesi hiçbir şey yapmıyor
`codesign -d -r-` çıktısında sertifika adı **hex metin** olarak yazıyor (0.5.1'de ekranda `TEK…49cc87…N` görüldü). Çalışan betik **metin** olarak değiştiriyordu:
```
R=$(codesign -d -r- "$1" 2>&1 | sed -n 's/^designated => //p' | sed 's/49cc87/c4b0/g')
```
Yeni betik ise **ham baytları** (`\x49\xcc\x87`) arıyor. Hex metinde bu baytlar yok, değişiklik olmaz; ADIM 8 doğrulaması da düşer.
**Düzelt:** hex metin değişimini (büyük/küçük harf duyarsız: `49cc87`/`49CC87` → `c4b0`/`C4B0`) yap; ham bayt değişimini de yedek olarak bırak. `--requirements` ile imzalarken framework'lerde `--preserve-metadata=identifier,flags`, uygulamada `--entitlements` kullan (0.5.1'deki gibi).

## Hata 2: ADIM 6, yalnız profildeki ilk sertifikaya bakıyor
`RS_AppStore` profili oluşturulurken **bütün sertifikalar işaretlendi**; Mac'te anahtarı olan sertifika 0. sırada olmayabilir. Ayrıca `PlistBuddy Print` veri alanını ham bayt + satır sonu olarak basıyor, `openssl -inform der` bunu reddedebilir.
**Düzelt:** 0.5.1'deki gibi 0–9 arası her sertifikayı dene:
```
D=$(plutil -extract DeveloperCertificates.$N raw -o - "$OUT/profile.plist") ; S=$(echo "$D" | base64 -D | shasum | awk '{print toupper($1)}')
```
ve Mac'teki Apple Distribution kimlikleriyle eşleşeni seç.

## Kontrol et ve raporla
- Zip'in içinde gerçekten `iOS/Unity-iPhone.xcodeproj` var mı (betik bunu bekliyor).
- `Info.plist`'te `ITSAppUsesNonExemptEncryption = false` var mı; sürüm 0.6.0, build 7.
- Düzeltilmiş `rs-yukle.sh`'i `Builds/`'e ve zip'in yanına koy (zip'i yeniden üretmene gerek yoksa üretme). LF ve `bash -n` kontrolü.
- Yaptığın değişikliği 3 satırda yaz. PM tekrar okuyacak.
