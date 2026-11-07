# PM → Claude Code (8 Kasım): Level Design videolarını PM'in okuyabileceği hale getir (kod/oyun yok)

**İlk iş:** commit et. PM dosyalarını asla geri alma. Unity, build ve p20'ye dokunma. Bu iş yalnız video işleme.

## Durum
Zeyd `Level Design/` klasörüne iki ekran kaydı koydu, her biri ~10 dk:
- `ScreenRecording_11-08-2026 01-42-26_1.MP4` (~1,07 GB)
- `ScreenRecording_11-08-2026 01-52-13_1.MP4` (~0,9 GB)

Biri Royal Match, biri Block Out. Hangisinin hangisi olduğunu ilk karelerden anla. PM dosyaları en fazla 400 MB taşıyabildiği için kareleri burada çıkar.

## 1. Git'e girmesin (önce bunu yap)
- `.gitignore`'a `Level Design/*.MP4`, `Level Design/*.mp4` ve `Level Design/**/*.mp4` ekle.
- Videolar asla commit edilmesin. Yanlışlıkla eklendiyse çıkar.

## 2. Kareler
ffmpeg kullan. Yoksa `pip install imageio-ffmpeg`, ya da `winget install ffmpeg`.

Her video için `Level Design/kareler/<oyun>/` (`rm` ya da `blockout`) altına şunları koy:
- **Sahne değişimi kareleri:** `select='gt(scene,0.06)'`, en az 1 sn arayla, genişlik 540 px, JPG kalite ~4. Dosya adı saniyeyi içersin (`t0123.jpg` gibi). Video başına en fazla ~400 kare. Fazlaysa eşiği yükselt.
- **Temas sayfaları:**
  - 4×4 ızgara (16 kare), her karenin köşesinde mm:ss zaman damgası, sayfa genişliği ~1600 px.
  - Kaynak: her 2 saniyeden bir kare.
  - Dosya adı `sayfa_01.jpg`, `sayfa_02.jpg`… (her sayfa ~32 sn kapsar, video başına ~20 sayfa).
- **Bölüm sınırları** (`bolumler.csv`): bölüm başlangıç ekranı ("Level X" / başla) ve bitiş ekranı (kazanma/kaybetme) zamanları. Sahne değişiminden ve parlaklık sıçramasından otomatik tahmin et. Emin olmadıklarını "?" ile işaretle. Sütunlar: `oyun, bolum_no(okunabildiyse), baslangic_sn, bitis_sn, sonuc(kazandi/kaybetti/?)`.
- **Küçük video kopyası:** 480 px genişlik, 15 fps, H.264 CRF 30, `-preset veryfast`, sessiz. `kareler/<oyun>/kucuk.mp4`, hedef < 150 MB.

## Bitiş
- `Level Design/kareler/LISTE.md`: her oyun için kare sayısı, sayfa sayısı, bölüm sayısı, küçük video boyutu.
- OVERNIGHT başına 1 satır. Zeyd'e "bitti" de.
- 30 dakikayı geçmesin. Takılırsan dur ve yaz.
