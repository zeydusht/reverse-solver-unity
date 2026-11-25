# PM_110 RAPOR: dalış hızı 3. tur (önizleme)

Önizleme, yayın yok. Yerel dal `pm108-onizleme`, push yok.

- **Ölçüm:** yerel build, Chrome, gerçek GPU, 390×844. Hız `Tools/pm92_flow_check.py` ile ölçüldü (ekran boyu/sn). iPhone'da denenmedi.
- **Araçlar:** kayıt `Tools/pm108_dalis.js`, video `Tools/pm108_video.py`, grafik `Tools/pm110_plot.py`.

## Ne değişti
`Dive75.Speed110`, Zeyd'in 3. çizimi (PM_92/108 yöntemi, yol aynı).

| Süre | Hız (tepeye göre) | Ölçülen |
|---|---|---|
| t = 0 | %90 | ilk kare %93 |
| 0 → %8 | %97'ye kısa tırmanış | %8'de 0,97 |
| %8 → %55 | %97 düz plato | 0,973, ±%0,1 |
| %55 → %62 | %100 tepe | %62'de 1,00 |
| %62 → %82 | %50'ye dik, yumuşak iniş (ivme sıçramasız; inişin sonu son bölümün eğimine bağlanıyor) | %70: 0,87 · %78: 0,59 · %82: 0,51 |
| %82 → son | %50 → %55 hafif artış, son 0,03 sn'de %15 iniş ve değme | %90: 0,52 · %96: 0,53 |

- **Süre kendiliğinden:** Tepe hızı PM_109'la aynı (0,44 ekran boyu/sn), yol aynı. Süre bu ikisinden hesaplanıyor (`Seconds110`, 0,9–1,1 sn bandına sınırlı). Tasarım 1,10 sn, ölçülen 1,07 sn: PM_109'la aynı.
  - Yüksek hız daha erken başlıyor, son ise daha yavaş; ikisi birbirini götürüyor.
  - Daha kısa isterse `&dalishiz=` ile ayarlanabilir: 1,1 → ~0,97 sn.
- **Son hız:** 0,22 ekran boyu/sn; PM_109'da 0,265'ti. `&dalistepe=` (varsayılan 2) tepe ile bu düşük hız arasındaki oran.
- **Dalış havası:** Ses yeni hızı izliyor; whoosh yine tabloya değerken bitiyor.
- **Değişmeyenler:** yol, çerçeve, resimde kalış, geri çekilme, kazanma → koridor → dalış ve sesler.

## Süreler (PM_109 → PM_110)

| | PM_109 | PM_110 |
|---|---|---|
| Yay + yaklaşma | 1,07 sn | **1,07 sn** (tasarım 1,10) |
| Resimde kalış | 0,29 sn | 0,29 sn |
| Geri çekilme | 0,80 sn | 0,80 sn |
| Toplam (bölüm başlayana kadar) | 2,18 sn | **2,18 sn** |

## Kenar boşluğu
Şu dört koşunun hepsinde 0 kare, ilk kareler dahil:
- PM_110;
- PM_109 (`&dalis110=0`);
- `&dalistepe=2.5&dalishiz=1.25`;
- `&dalishiz=0.8`.

İlk karelere ayrıca gözle bakıldı: kenarda açıklık yok.

## 20 ms'yi aşan kareler (bu PC, Chrome)

| Koşu | Ekran kaydı açık | Ekran kaydı yok |
|---|---|---|
| PM_109 | 8 kare, en uzun 38 ms (değme) | 3 kare, en uzun 30 ms |
| PM_110 | 2–4 kare, en uzun 33–36 ms | 3 kare, en uzun 31 ms |

- **Nerede:** Basışın ilk karesi (~30 ms) ve tabloya değme (bölüm kuruluyor, 19–25 ms). Bazı koşularda geri çekilmede 21–30 ms'lik kareler var.
- **Yay ve yaklaşmanın içinde:** uzun kare yok.
- **Kayıt kesintisi:** İlk PM_110 kaydında ekran kaydı ilk 0,13 sn'yi kaçırdı. Oyunun kendi ölçümünde o anda takılma yoktu (ilk kare 36 ms). Video için yeniden kaydedildi; yeni kayıtta en büyük boşluk 34 ms.

## Bayraklar
| Bayrak | Ne yapar |
|---|---|
| `&dalis110=0` | PM_109 hâli |
| `&dalis109=0` | PM_108 hâli |
| `&dalis108=0` | PM_92 hâli |
| `&dalistepe=` | Tepe ile son hız oranı. PM_110'da varsayılan 2, PM_108/109'da 1,65. |
| `&dalishiz=` | Hız çarpanı; büyüdükçe hızlı. |

## Önizleme
- **`hiz.png`:**
  - Üstte Zeyd'in 2. ve 3. çizimi (kalın açık çizgiler) ve tasarımlar (kesik).
  - Altta PM_109 ve PM_110'un ölçülen hızı.
- **`dalis_yan_yana.mp4`:** PM_109 | PM_110, 780×844, 60 fps, 6,2 sn, 2,45 MB. Önce PM_109'un, sonra PM_110'un sesiyle oynuyor; sesi duyulan taraf "SES" ile işaretli.
- **`dalis_yeni.mp4`:** PM_110, 390×844, 60 fps, sesli, 3,1 sn, 0,65 MB.

## Doğrulanmayan
iPhone'da bakılmadı.
