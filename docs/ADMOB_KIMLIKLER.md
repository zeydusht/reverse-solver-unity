# AdMob kimlikleri (Zeyd, 23 Kasım 2026)

Bu kimlikler gizli değil: uygulamanın içinde zaten görünürler. Repo'da durabilir.

| | Kimlik |
|---|---|
| Yayıncı (Publisher) | `pub-4171335473466896` |
| Uygulama kimliği (iOS App ID, `GADApplicationIdentifier`) | `ca-app-pub-4171335473466896~6469853119` |
| Ödüllü (Rewarded), `RS Rewarded` | `ca-app-pub-4171335473466896/2134575175` |
| Geçiş (Interstitial), `RS Interstitial` | `ca-app-pub-4171335473466896/7814724376` |

## Kullanım
- `Assets/Config/ads.json`'a yaz. Test ID'leri yalnız editörde ve `&test=1` / geliştirme build'inde kalsın; mağaza build'i gerçek ID'leri kullansın.
- **Geliştirme sırasında gerçek reklama tıklama yok.** Zeyd'in iPhone'u AdMob'da test cihazı olarak eklensin (Ad Inspector'dan ya da kodda test cihaz kimliği). Kendi reklamına tıklamak hesabın kapanmasına yol açabilir.
- `app-ads.txt` satırı (`zeydusht.github.io` kökü):

  ```
  google.com, pub-4171335473466896, DIRECT, f08c47fec0942fa0
  ```

## Durum
- AdMob hesabı: ödeme bilgileri girildi, Google inceliyor (~24 sa).
- Uygulama: "İnceleme gerekli". App Store'da yayınlanınca AdMob'da mağaza linkiyle eşlenecek, sonra inceleme tamamlanır.
