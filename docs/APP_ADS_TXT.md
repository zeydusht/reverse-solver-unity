# app-ads.txt (AdMob)

AdMob, uygulamanın App Store sayfasındaki **geliştirici sitesinin kökünde** bir `app-ads.txt` dosyası ister. Yoksa reklam geliri kısılabilir.

Bizim site: `https://zeydusht.github.io/`. Dosya şu adreste açılmalı:

```
https://zeydusht.github.io/app-ads.txt
```

## Dosyanın içeriği

Tek satır (yayıncı kimliği: `pub-4171335473466896`, `docs/ADMOB_KIMLIKLER.md`):

```
google.com, pub-4171335473466896, DIRECT, f08c47fec0942fa0
```

## Neden ayrı bir depo

Oyun `zeydusht.github.io/reverse-solver-unity/` altında yayında. Bu bir alt klasör, kök değil. Kökü yalnız **`zeydusht.github.io`** adlı depo yayınlar; o depo henüz yok. Ben depo açmadım.

## Zeyd'in adımları (bir kez, ~3 dk)

1. GitHub'da **New repository**.
   - Adı tam olarak `zeydusht.github.io` olsun.
   - Public seç, "Add a README" işaretli olsun.
2. Depoda **Add file → Create new file**.
   - Ad: `app-ads.txt`.
   - İçerik: yukarıdaki tek satır.
   - **Commit**.
3. Settings → Pages → Branch: `main`, klasör `/ (root)` → Save. Birkaç dakika sonra `https://zeydusht.github.io/app-ads.txt` açılır.
4. App Store Connect'te uygulamanın **Pazarlama URL'si / Geliştirici web sitesi** `https://zeydusht.github.io` olsun.
5. AdMob → Uygulamalar → Reverse Solver → **app-ads.txt** sekmesi → "Güncellemeleri kontrol et". Doğrulama 24 saate kadar sürebilir.

## Kontrol

```
curl -s https://zeydusht.github.io/app-ads.txt
```

Çıktı yukarıdaki satırla birebir aynı olmalı. Satırın başında ya da sonunda boşluk olmasın.
