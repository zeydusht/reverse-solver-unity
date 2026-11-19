# PM_91 RAPOR: dalış hızı (önizleme)

Önizleme, yayın yok. Yerel dal `pm91-onizleme`; push yok, p23 aynı (PM_90).

Ölçümler yerel build'de alındı: Chrome, gerçek GPU, 390×844, 60 fps. Sayılar oyunun kendi günlüğünden okundu (`Tools/pm91_timing_check.py`). iPhone'da denenmedi.

## Süreler

| | Menü → bölüm (5 ve 13) | Kazanma → koridor → bölüm (2→3) |
|---|---|---|
| Koridor | – | 1,92 sn → 1,92 sn (dokunulmadı) |
| Yaklaşma | **2,21 sn → 1,61 sn** | **0,61 sn → 0,56 sn** |
| Resimde kalış | **0,56 sn → 0,27 sn** | **0,43 sn → 0,23 sn** |
| Geri çekilme | 0,80 sn → 0,80 sn | 0,80 sn → 0,80 sn |
| Toplam (dokunuştan bölüme) | 3,40 sn → 2,68 sn | 1,80 sn → 1,58 sn (koridordan sonra) |

- **Resimde kalış:** resmin ekranı tamamen doldurduğu ilk kareden geri çekilmenin başladığı kareye kadar.
  - Eski: resim ekranı yaklaşmanın sonundan 0,16 sn önce dolduruyordu; kamera o sırada hâlâ yavaşça süzülüyordu.
  - Yeni: resim ekranı ancak varış karesinde dolduruyor.
- **Ölçüm tekrarları:**
  - Menü, 5 ve 13. bölüm: eski 0,559 ve 0,556 sn; yeni 0,269 ve 0,274 sn.
  - Kazanma, 2→3: eski 0,432 sn, yeni 0,227 sn. Bir kare (17 ms) oynuyor.

## Eğri (`hiz.png`)

- **Kalkış:** eskisiyle aynı. Menüde ilk 0,15 sn, koridordan sonra ilk 0,12 sn eski eğrinin kendisi; hız ve ivme oradan sıçramadan devam ediyor.
- **Hız tepesi:**
  - Menü: yolun %77'sinde, sürenin %78'inde.
  - Kazanma: yolun %77'sinde, sürenin %82'sinde.
- **Varış:**
  - Hız tepenin %75'i.
  - Son %10'luk yolda hız %21 düşüyor (sınır %30).
  - Eski eğriler varışta duruyordu (hız 0).
- **Ekranda ölçülen (resmin büyüme hızı):**
  - Menü: 0,5. saniyeden varışa kadar 0,82–0,89 arasında, neredeyse düz. Eskisi 1,2. saniyeden sonra sıfıra iniyordu.
  - Kazanma: tepe 2,7, varışta 2,5. Eskisinin tepesi 3,4, varışta 0,2.
- **Yol aynı.** Yalnız zamanlama değişti; menüde resmin ortaya kayması da yola bağlandı.
- **Değme:** varıştan sonra kamera aynı hızla birkaç kare daha içeri giriyor, sonra duruyor. Toplam %2,5, hep eşit ölçek. Menüde yaklaşık 40 ms'de, kazanmada yaklaşık 13 ms'de bitiyor. `&degme=0` ile kapatılabilir.
- **Tahtanın kurulması:** artık yaklaşmanın son karesinde. Bu kare yerelde 42 ms sürdü. Sonraki kare en çok 1/60 sn ilerliyor, böylece resim sıçramıyor; bir anlık duraksama değmeyle aynı ana denk geliyor. iPhone'da bu süre daha uzun olabilir.

## Dokunulmayanlar

- Yol, çerçeve, çizgiler, geri çekilme biçimi, koridor ve HUD.
- PM_90'ın oran ölçümü yeni hâlde de geçiyor:
  - Kare başına X/Y en kötü 0,997 (menü → 5) ve 1,001 (kazanma → 3).
  - Bekleme sırasında 1,000.

## Videolar

`dalis_eski.mp4`, `dalis_yeni.mp4`, `dalis_yan_yana.mp4`, `kazanma_yeni.mp4`:

- Hepsi 60 fps, 390×844 (yan yana 780×844), H.264 `-preset veryfast`, ses yok, hepsi 2 MB'ın altında.
- Kayıt Chrome'dan; kareler 60 fps sabit hıza oturtuldu (`Tools/pm91_video.py`).
- Eski hâl PM_90 build'inden (p23'teki).
- `&dalis91=0` aynı build'de eski zamanlamayı açar.
