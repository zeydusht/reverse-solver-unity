# PM_109 RAPOR: dalış hızı 2. tur (önizleme)

Önizleme, yayın yok. Yerel dal `pm108-onizleme`, push yok.

- **Ölçüm:** yerel build, Chrome, gerçek GPU, 390×844, `Tools/pm92_flow_check.py` (duvarın ekrandaki hareketi, ekran boyu/sn). iPhone'da denenmedi.
- **Kayıt:** `Tools/pm108_dalis.js`. **Video:** `Tools/pm108_video.py`. **Grafik:** `Tools/pm109_plot.py`.

## Ne değişti
`Dive75.Speed109`: ekran hızı Zeyd'in ikinci çizimine göre kuruluyor (PM_92/108 yöntemi, yol aynı).

| Süre | Hız (tepeye göre) | Ölçülen |
|---|---|---|
| t = 0 | %40 | ilk kare (0–30 ms ortası) %49, tasarım da %48 |
| 0 → %10 | %85'e dik tırmanış (ivme sıçramasız, plato eğimine bağlanıyor) | %10'da 0,84 |
| %10 → %65 | %85 → %100 yavaş yükselen plato | %30: 0,92 · %50: 0,98 |
| %65 → %72 | tepe | 1,00 |
| %72 → %82 | tepenin 1/1,65'ine (%61) yumuşak iniş | %77: 0,89 · %82: 0,65 |
| %82 → son | sabit, sonra son 0,03 sn'de %15 iniş ve değme (PM_108 ile aynı) | 0,265, **±%0,3** |

- **Süre kendiliğinden:** Son sabit hız PM_108'inkiyle aynı (0,265 ekran boyu/sn), yol aynı. Süre bu ikisinden hesaplanıyor (`Seconds109`, 0,9–1,1 sn bandına sınırlı). Tasarım 1,09 sn, ölçülen 1,07 sn.
- **Tepe:** PM_108'le aynı (0,44 ekran boyu/sn). Fark, yüksek hızın ne kadar uzun sürdüğünde.
- **Dalış havası:** Ses yeni hızı izliyor, whoosh yine değmede bitiyor.
- **Değişmeyenler:** yol, çerçeve, resimde kalış, geri çekilme, kazanma → koridor → dalış, sesler.

## Süreler (PM_108 → PM_109)

| | PM_108 | PM_109 |
|---|---|---|
| Yay + yaklaşma | 1,19 sn | **1,07 sn** (tasarım 1,09) |
| Resimde kalış | 0,29 sn | 0,29 sn |
| Geri çekilme | 0,80 sn | 0,80 sn |
| Toplam (bölüm başlayana kadar) | 2,29 sn | **2,18 sn** |

## Kenar boşluğu
0 kare: PM_109, PM_108 (`&dalis109=0`), `&dalistepe=2.2&dalishiz=1.25` ve `&dalishiz=0.8` koşularının hepsinde.

## 20 ms'yi aşan kareler (bu PC, Chrome)

| Koşu | Ekran kaydı açık | Ekran kaydı yok (2 koşu) |
|---|---|---|
| PM_108 | 3 kare, en uzun 36 ms (değme) | 3 ve 9 kare, en uzun 31 ve 44 ms (değme) |
| PM_109 | 4 kare, en uzun 56 ms (değme) | 3 ve 9 kare, en uzun 30 ve 29 ms (değme) |

- **Nerede:** Basışın ilk karesi (~30 ms) ve tabloya değme (bölüm kuruluyor). Bazı koşularda geri çekilmede 21–32 ms'lik birkaç kare de var.
- **Yay ve yaklaşmanın içinde:** Uzun kare yok.
- **Ekran kaydı açıkken:** Değme karesi, dalış ne kadar erken biterse o kadar uzuyor: 0,82 sn'de 82 ms, 1,08 sn'de 56 ms, 1,20 sn'de 36 ms. Kayıt kapalıyken bu fark yok: PM_109 29–30 ms, PM_108 31–44 ms. Yani PC'de fark kaydın yükünden geliyor.
- **Telefonda:** Daha kısa dalışta bölümün kuruluşu daha erken bir kareye denk gelir. Telefonda ayrıca bakılmalı.

## Bayraklar
- `&dalis109=0`: PM_108 hâli.
- `&dalis108=0`: PM_92 hâli.
- `&dalistepe=` (1,05–3; varsayılan 1,65): Tepe ile son sabit hız oranı.
- `&dalishiz=`: Hız çarpanı (PM_86'dan beri), büyüdükçe hızlı.

## Önizleme
- **`hiz.png`:**
  - Üstte Zeyd'in iki çizimi (kalın açık çizgiler) ve tasarımlar (kesik).
  - Altta PM_108 ve PM_109'un ölçülen hızları.
- **`dalis_yan_yana.mp4`:** PM_108 | PM_109, 780×844 (her yan 390×844), 60 fps, 6,4 sn, 2,5 MB. Önce PM_108'in, sonra PM_109'un sesiyle oynuyor; sesi duyulan taraf "SES" ile işaretli.
- **`dalis_yeni.mp4`:** PM_109, 390×844, 60 fps, sesli, 3,1 sn, 0,65 MB.

## Doğrulanmayan
iPhone'da bakılmadı.
