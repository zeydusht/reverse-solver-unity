#!/bin/bash
# Reverse Solver 0.6.0 (7) -> TestFlight. Tek komut:  bash ~/Downloads/rs-yukle.sh
# Bir adim hata verirse duzeltip o adimdan devam:  bash ~/Downloads/rs-yukle.sh --from 5
# PM_MESAJ_61 EK: 0.5.1 yuklemesindeki 8 hatanin hepsi burada sirayla cozulur.

APPLE_ID="zeydtekin12@gmail.com"
TEAM_ID="9X55V64NRR"
BUNDLE_ID="com.zeydtekin.reversesolver"
VERSION="0.6.0"
ZIP="$HOME/Downloads/ReverseSolver-iOS-$VERSION.zip"
PROFILE="$HOME/Downloads/RS_AppStore.mobileprovision"
WORK="$HOME/rs-$VERSION"          # zip buraya acilir
PROJ="$WORK/iOS"                  # Xcode projesi
OUT="$WORK/out"                   # arsiv, ipa, gecici dosyalar
TOTAL=10

FROM=1
if [ "$1" = "--from" ] && [ -n "$2" ]; then FROM="$2"; fi

step() { echo; echo "==== ADIM $1/$TOTAL: $2 ===="; }
fail() { echo; echo "!!!! HATA (ADIM $1): $2"; echo "Duzeltince devam etmek icin:  bash ~/Downloads/rs-yukle.sh --from $1"; exit 1; }
want() { [ "$FROM" -le "$1" ]; }

app_dir() { ls -d "$OUT/ipa/Payload/"*.app 2>/dev/null | head -1; }

# ---------------------------------------------------------------- 1
if want 1; then
  step 1 "Zip aciliyor ($ZIP)"
  [ -f "$ZIP" ] || fail 1 "Zip bulunamadi. ReverseSolver-iOS-$VERSION.zip dosyasini Drive'dan Indirilenler (Downloads) klasorune indir."
  rm -rf "$WORK" && mkdir -p "$WORK" || fail 1 "$WORK klasoru olusturulamadi."
  unzip -q "$ZIP" -d "$WORK" || fail 1 "Zip acilamadi (dosya yarim inmis olabilir, tekrar indir)."
  [ -d "$PROJ/Unity-iPhone.xcodeproj" ] || fail 1 "Zip'in icinde iOS/Unity-iPhone.xcodeproj yok."
  echo "Tamam: $PROJ"
fi

# ---------------------------------------------------------------- 2  (hata 1 ve 2)
if want 2; then
  step 2 "Izinler ve eski imzalar temizleniyor"
  cd "$PROJ" || fail 2 "$PROJ yok. Bastan calistir: bash ~/Downloads/rs-yukle.sh"
  xattr -cr . 2>/dev/null
  chmod +x process_symbols.sh usymtool* 2>/dev/null
  find . -name "*.sh" -exec chmod +x {} \; 2>/dev/null
  find . -name "usymtool*" -exec chmod +x {} \; 2>/dev/null
  rm -rf Frameworks/UnityRuntime.framework/_CodeSignature 2>/dev/null
  [ -f Frameworks/UnityRuntime.framework/UnityRuntime ] && chmod +x Frameworks/UnityRuntime.framework/UnityRuntime
  find . -path "*.framework/_CodeSignature" -prune -exec rm -rf {} \; 2>/dev/null
  echo "Tamam."
fi

# ---------------------------------------------------------------- 3
if want 3; then
  step 3 "Arsiv aliniyor (xcodebuild archive, birkac dakika surer)"
  cd "$PROJ" || fail 3 "$PROJ yok. Bastan calistir."
  rm -rf "$OUT/RS.xcarchive"; mkdir -p "$OUT"
  xcodebuild archive -project Unity-iPhone.xcodeproj -scheme Unity-iPhone -configuration Release \
    -destination "generic/platform=iOS" -archivePath "$OUT/RS.xcarchive" -allowProvisioningUpdates \
    DEVELOPMENT_TEAM="$TEAM_ID" CODE_SIGN_STYLE=Automatic > "$OUT/archive.log" 2>&1
  [ -d "$OUT/RS.xcarchive" ] || { tail -n 25 "$OUT/archive.log"; fail 3 "Arsiv alinamadi. Xcode > Settings > Accounts'ta Apple hesabi ekli mi? Tam gunluk: $OUT/archive.log"; }
  echo "Tamam: $OUT/RS.xcarchive"
fi

# ---------------------------------------------------------------- 4
if want 4; then
  step 4 "Disa aktariliyor (exportArchive, app-store-connect)"
  cat > "$OUT/ExportOptions.plist" <<PLIST
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0"><dict>
  <key>method</key><string>app-store-connect</string>
  <key>teamID</key><string>$TEAM_ID</string>
  <key>signingStyle</key><string>automatic</string>
  <key>uploadSymbols</key><false/>
  <key>destination</key><string>export</string>
</dict></plist>
PLIST
  rm -rf "$OUT/export"
  xcodebuild -exportArchive -archivePath "$OUT/RS.xcarchive" -exportPath "$OUT/export" \
    -exportOptionsPlist "$OUT/ExportOptions.plist" -allowProvisioningUpdates > "$OUT/export.log" 2>&1
  ls "$OUT/export/"*.ipa >/dev/null 2>&1 || { tail -n 25 "$OUT/export.log"; fail 4 "Disa aktarma basarisiz. Tam gunluk: $OUT/export.log"; }
  echo "Tamam: $(ls "$OUT/export/"*.ipa)"
fi

# ---------------------------------------------------------------- 5
if want 5; then
  step 5 ".ipa aciliyor"
  rm -rf "$OUT/ipa"; mkdir -p "$OUT/ipa"
  unzip -q "$(ls "$OUT/export/"*.ipa | head -1)" -d "$OUT/ipa" || fail 5 ".ipa acilamadi. Adim 4'ten tekrar dene: --from 4"
  APP="$(app_dir)"; [ -n "$APP" ] || fail 5 "Payload icinde .app yok."
  echo "Tamam: $APP"
fi

# ---------------------------------------------------------------- 6  (hata 4: profil + entitlements + dogru sertifika)
if want 6; then
  step 6 "RS_AppStore profili ve entitlements ile imzalaniyor"
  APP="$(app_dir)"; [ -n "$APP" ] || fail 6 ".app yok. --from 5 ile tekrar dene."
  [ -f "$PROFILE" ] || fail 6 "RS_AppStore.mobileprovision Indirilenler'de yok. MAC_ADIMLAR dosyasindaki 2. maddeye bak, indirip --from 6 ile devam et."
  security cms -D -i "$PROFILE" > "$OUT/profile.plist" 2>/dev/null || fail 6 "Profil okunamadi."
  /usr/libexec/PlistBuddy -x -c "Print :Entitlements" "$OUT/profile.plist" > "$OUT/entitlements.plist" || fail 6 "Profilden entitlements alinamadi."
  # profildeki her sertifikanin (0-9) SHA-1'i; Mac'te anahtari olan Apple Distribution kimligiyle eslesen secilir
  IDS=$(security find-identity -v -p codesigning | grep "Apple Distribution" | awk '{print $2}')
  SIGN=""; SHAS=""
  for N in 0 1 2 3 4 5 6 7 8 9; do
    D=$(plutil -extract DeveloperCertificates.$N raw -o - "$OUT/profile.plist" 2>/dev/null) || break
    S=$(echo "$D" | base64 -D | shasum | awk '{print toupper($1)}')
    SHAS="$SHAS $S"
    for h in $IDS; do [ "$h" = "$S" ] && SIGN="$h"; done
    [ -n "$SIGN" ] && break
  done
  if [ -z "$SIGN" ]; then
    echo "Profildeki sertifikalar:$SHAS"; security find-identity -v -p codesigning
    fail 6 "Profildeki Apple Distribution sertifikasinin anahtari bu Mac'te yok. RS_AppStore profilini bu Mac'teki sertifikayla yeniden olusturup indir."
  fi
  echo "$SIGN" > "$OUT/sign_id.txt"
  cp "$PROFILE" "$APP/embedded.mobileprovision"
  rm -rf "$APP/_CodeSignature"
  for fw in "$APP/Frameworks/"*.framework "$APP/Frameworks/"*.dylib; do
    [ -e "$fw" ] || continue
    rm -rf "$fw/_CodeSignature"
    codesign -f -s "$SIGN" "$fw" || fail 6 "Framework imzalanamadi: $fw (Keychain sorarsa Mac giris sifresi + Always Allow)"
  done
  codesign -f -s "$SIGN" --entitlements "$OUT/entitlements.plist" "$APP" || fail 6 "Uygulama imzalanamadi (Keychain sorarsa Mac giris sifresi + Always Allow)."
  echo "Tamam: $SIGN ile imzalandi."
fi

# ---------------------------------------------------------------- 7  (hata 3: TEKIN'deki I)
if want 7; then
  step 7 "'TEKIN' adindaki I harfi icin imza gereksinimi duzeltiliyor"
  APP="$(app_dir)"; SIGN="$(cat "$OUT/sign_id.txt" 2>/dev/null)"
  [ -n "$APP" ] && [ -n "$SIGN" ] || fail 7 "Once adim 6 bitmeli: --from 6"
  fix() {   # $1 = framework ya da app, $2 = ek codesign secenekleri
    local req
    req=$(codesign -d -r- "$1" 2>&1 | sed -n 's/^designated => //p')
    [ -n "$req" ] || return 1
    # I + birlesik nokta -> tek parca I. codesign adi hex metin yazar (0.5.1: TEK...49cc87...N): metin degisimi;
    # ham bayt degisimi yedek.
    req=$(printf '%s' "$req" | sed 's/49cc87/c4b0/g; s/49CC87/C4B0/g' | perl -pe 's/\x49\xcc\x87/\xc4\xb0/g')
    codesign -f -s "$SIGN" $2 --requirements "=designated => $req" "$1"
  }
  for fw in "$APP/Frameworks/"*.framework "$APP/Frameworks/"*.dylib; do
    [ -e "$fw" ] || continue
    fix "$fw" "--preserve-metadata=identifier,flags" || fail 7 "Gereksinim duzeltilemedi: $fw"
  done
  fix "$APP" "--entitlements $OUT/entitlements.plist" || fail 7 "Uygulamanin gereksinimi duzeltilemedi."
  echo "Tamam."
fi

# ---------------------------------------------------------------- 8
if want 8; then
  step 8 "Imza kontrolu (codesign --verify --deep --strict)"
  APP="$(app_dir)"; [ -n "$APP" ] || fail 8 ".app yok. --from 5"
  codesign --verify --deep --strict --verbose=2 "$APP" > "$OUT/verify.log" 2>&1
  cat "$OUT/verify.log"
  grep -q "satisfies its Designated Requirement" "$OUT/verify.log" || fail 8 "Imza dogrulanmadi ('satisfies its Designated Requirement' yok). Yukleme YAPILMADI. --from 6 ile tekrar dene."
  echo "Tamam: imza gecerli."
fi

# ---------------------------------------------------------------- 9
if want 9; then
  step 9 "Yeni .ipa paketleniyor"
  [ -d "$OUT/ipa/Payload" ] || fail 9 "Payload yok. --from 5"
  rm -f "$OUT/ReverseSolver-$VERSION.ipa"
  (cd "$OUT/ipa" && zip -qry "$OUT/ReverseSolver-$VERSION.ipa" Payload) || fail 9 ".ipa olusturulamadi."
  echo "Tamam: $OUT/ReverseSolver-$VERSION.ipa"
fi

# ---------------------------------------------------------------- 10
if want 10; then
  step 10 "TestFlight'a yukleniyor (xcrun altool)"
  [ -f "$OUT/ReverseSolver-$VERSION.ipa" ] || fail 10 ".ipa yok. --from 9"
  echo "Apple ID: $APPLE_ID"
  printf "Uygulamaya ozel parolayi yapistir (ekranda gorunmez) ve Enter'a bas: "
  read -s PASS; echo
  [ -n "$PASS" ] || fail 10 "Parola bos."
  xcrun altool --upload-app -f "$OUT/ReverseSolver-$VERSION.ipa" -t ios -u "$APPLE_ID" -p "$PASS" 2>&1 | tee "$OUT/upload.log"
  unset PASS
  if grep -qi "No errors uploading\|UPLOAD SUCCEEDED" "$OUT/upload.log"; then
    echo; echo "==== BITTI: 0.6.0 (7) yuklendi. 10-30 dakika icinde App Store Connect > TestFlight'ta gorunur. ===="
    echo "Simdi: appleid.apple.com'da 'Transporter' parolasini sil ve Xcode > Settings > Accounts'tan hesabi kaldir."
  else
    fail 10 "Yukleme basarisiz. Gunluk: $OUT/upload.log  (parola yanlissa yeni parola alip --from 10)"
  fi
fi
