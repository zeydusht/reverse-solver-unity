# PM_ACIL — GitHub'da sızan anahtar (11 Kasım 11:06) — PM_62'den ÖNCE yap

GitHub secret scanning ve Google Cloud uyarısı: `zeydusht/reverse-solver-unity` (herkese açık) deposunda **commit 39812477** içinde:
- `...-46cf-a82f-1a73581cd18f.dmp` (Chrome/Crashpad çökme dosyası) → GCP API key (service account bound, proje "Default Gemini Project", gen-lang-client-0244907128)
- `.../shared_proto_db/000003.log` → Google API Key
Sebep büyük ihtimalle: doğrulama/test için açılan başsız Chrome'un **profil klasörü** (ör. `.chrome-task6/` benzeri, `user-data-dir`) depoya commit edilmiş. Google anahtarı siliyor.

Hiçbir yerde anahtarın değerini **yazdırma, rapora koyma**.

## 1. Bul
- `git show --stat 39812477`: hangi dal, hangi klasörler. Aynı türden dosya başka commit'lerde ya da `gh-pages`'te var mı (`git log --all -- '*shared_proto_db*' '*.dmp' '*Crashpad*' '*Cookies*' '*Login Data*'`).
- Profil klasöründe **Cookies, Login Data, Local State, Web Data** gibi dosyalar var mı? Varsa raporla (içeriğini açma). Bu profil Zeyd'in gerçek Chrome'u mu, yoksa test için yaratılmış boş bir profil mi?

## 2. Temizle
- Bu klasörleri çalışma ağacından sil, `.gitignore`'a ekle: tarayıcı profilleri (`.chrome*/`, `*chrome-profile*/`, `user-data*/`), `*.dmp`, `Crashpad/`, `*shared_proto_db*`, `.task*-python/`, `.task*-pip-cache/` ve benzeri araç önbellekleri.
- Geçmişten sil: `git filter-repo` (yoksa kur) ile bu yolları **bütün dallardan** temizle. Sonra etkilenen dalları **force push** et. Force push'tan önce `pm60-wip`, `art`, `main`, `gh-pages` dallarının yedeğini yerelde etiketle (`yedek-11kasim-*`).
- `gh-pages`'e dokunman gerekiyorsa yalnız sızan dosyaları kaldır; p20, p21, kök, pt1 içerikleri aynı kalsın.

## 3. Bir daha olmasın
- `verify_build` ve Codex'in tarayıcı denemeleri profil klasörünü **depo dışında** (ör. `%TEMP%`) oluştursun.
- Commit öncesi kontrol: büyük/ikili dosya ve bilinen gizli desenleri (`AIza`, `-----BEGIN`, `sk-`) engelleyen basit bir pre-commit kancası.

## 4. Rapor
3–5 satır: ne bulundu (dosya türleri, profil gerçek mi test mi), ne silindi, hangi dallar force-push edildi, gitignore ve kanca eklendi mi. Sonra PM_62'ye dön (codex exec'i `< /dev/null` ile çalıştır).
