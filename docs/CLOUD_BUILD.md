# Bulut build (GitHub Actions)

Bilgisayarda WebGL build alınamadığında kullanılır (ör. Windows Akıllı Uygulama Denetimi Unity'nin dosyasını engellediğinde). Workflow: `.github/workflows/webgl-build.yml`. Yalnızca elle başlar; çıktıyı indirilebilir bir artifact olarak verir, gh-pages'e kendisi yayınlamaz.

Kullanılan imaj: `unityci/editor:ubuntu-6000.6.4f1-webgl-3` (game-ci; projedeki Unity sürümüyle aynı, `ProjectSettings/ProjectVersion.txt`'den otomatik okunur).

## Bir kerelik kurulum (senin yapacakların)

1. **Lisans:** game-ci'nin etkinleştirme sayfasındaki güncel adımları izle: https://game.ci/docs/github/activation
   - Unity Personal için gereken secret'lar: `UNITY_EMAIL`, `UNITY_PASSWORD` ve sayfa nasıl tarif ediyorsa `UNITY_LICENSE`.
   - **Dikkat:** Bu bilgisayarda klasik lisans dosyası (`C:\ProgramData\Unity\Unity_lic.ulf`) yok; Unity'nin yeni lisans sistemi kullanılıyor. game-ci'nin Personal lisansı şu an hangi yolla kabul ettiğini o sayfadan doğrula. Bu yolun çalışacağı garanti değil; çalışmazsa bulut build seçeneği düşer.
2. **Secret'ları ekle:** github.com/zeydusht/reverse-solver-unity → Settings → Secrets and variables → Actions → *New repository secret*. İsimler birebir: `UNITY_EMAIL`, `UNITY_PASSWORD`, `UNITY_LICENSE`. Bu değerler repoya asla yazılmaz.

## Çalıştırma

1. Repo → Actions → *WebGL build* → *Run workflow* (isteğe bağlı bir etiket yaz) → *Run workflow*.
2. İlk çalıştırma ~20–40 dk sürebilir (imaj indirme + IL2CPP); sonrakiler `Library` önbelleğiyle daha kısa.
3. Bitince çalıştırmanın sayfasında *Artifacts → webgl-builds* indir. İçinde `Builds/WebGL` (Brotli, `/br/`), `Builds/WebGL-gz` (gzip, `/gz/`), size-log ve raporlar var.
4. Yayını mühendislik (Claude) yapar: artifact'i indirip gh-pages'e koyar. İstersen bana "artifact hazır" demen yeterli.

## Secret'lar yokken

Workflow Unity'yi etkinleştirme adımında lisans hatasıyla durur; bu beklenen davranış.
