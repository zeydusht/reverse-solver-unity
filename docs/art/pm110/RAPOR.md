# PM_110 RAPOR: dalış hızı 3. tur, asıl amaç (önizleme)

Önizleme, yayın yok. Yerel dal `pm108-onizleme`, push yok.

Not: `docs/PM_MESAJ_110.md` diskte güncellenmemişti; "Asıl amaç" bölümü dosyada yok. İş, Zeyd'in mesajındaki tarifle yapıldı:
- önce geri çekilmenin ekran hızı ölçüldü;
- gidişin ortalama hızı en az onun kadar yapıldı;
- iki varyant hazırlandı: A, Zeyd'in 3. çizimi (bu hıza ölçekli); B, geri çekilmenin tersi.

- **Ölçüm:** yerel build, Chrome, gerçek GPU, 390×844, p23 bölüm 1. iPhone'da denenmedi.
- **Araçlar:**
  - kayıt: `Tools/pm108_dalis.js`;
  - hız: `Tools/pm92_flow_check.py` (gidiş) ve `Tools/pm110_ekran_hizi.py` (geri çekilme);
  - video: `Tools/pm108_video.py`;
  - grafik: `Tools/pm110_plot.py`.

## 1. Geri çekilmenin ekran hızı
Gidiş ve geri çekilme aynı ölçüyle ölçüldü: PM_92'nin ölçüsü (Flow92). Ekrana 9×19'luk bir ızgara konuyor, her karede ızgaranın ortalama kayması ölçülüyor; birim ekran boyu/sn.
- **Gidiş:** duvar düzleminden ölçülüyor.
- **Geri çekilme:** oyunun kamerasından ölçülüyor (yeni `[geri] kamera` satırı).

| | Yol | Süre | Ortalama | Tepe |
|---|---|---|---|---|
| Geri çekilme | 0,438 ekran boyu | 0,78 sn | **0,56/sn** | 0,81 |
| PM_109 gidişi | 0,40 ekran boyu | 1,07 sn | 0,37/sn | 0,44 |

PM_109'da geri çekilme gidişten 1,5 kat hızlıydı.

**Görüntüden ikinci ölçü:** Karelerden optik akışla da ölçüldü (`pm110_ekran_hizi.py`). Bu ölçü düz renkli alanda hareketi göremiyor. Dalışta ekranın çoğu düz turkuaz duvar olduğu için gidişi olduğundan yavaş gösteriyor: gidiş 0,25/sn, geri çekilme 0,46/sn. Bu yüzden esas alınmadı. Bu ölçüyle süre 0,60 sn çıkardı.

## 2. Varyantlar
İki varyantta da gidişin süresi, yolu bu ortalama hızla alacak kadar:

0,40 / 0,56 = **0,71 sn**

Oyun yolu kendisi hesaplıyor; `&dalisgeri=` ile hedef hız değiştirilebilir.

### A: Zeyd'in 3. çizimi
`Dive75.Speed110`, hedef ekran hızı (tepeye göre):

| Süre | Hız |
|---|---|
| Başlangıç | %90 |
| %8 | %97 |
| %55'e kadar | düz |
| %62 | %100, tepe |
| %62 → %82 | %50'ye dik, yumuşak iniş |
| %82 → son | %50'den %55'e hafif artış, sonra kısa iniş ve değme |

- **Ölçülen:** başlangıç 0,64, plato 0,65, tepe 0,67, iniş sonu 0,34, değme 0,34. Ortalama **0,56**.

### B: Geri çekilmenin tersi
- **Kurgu:** Geri çekilmenin ölçülen hız şekli sondan başa okunuyor (`Dive75.SpeedBack110`, 21 noktalık tablo, yumuşak eğri).
  - Yavaş başlıyor.
  - Sürenin %60'ında tepeye çıkıyor.
  - Tabloya çok yavaş değiyor.
- **Ölçülen:** başlangıç 0,21, tepe 0,84, değme 0,06. Ortalama **0,57**.
- **Tutarlılık:** B'nin tepesi geri çekilmenin tepesiyle (0,81) neredeyse aynı. Grafikte geri çekilmenin tersi B'nin üstüne oturuyor.

### Ortak
- **Dalış havası:** Ses hızı izliyor, whoosh yine tabloya değerken bitiyor.
- **Değişmeyenler:**
  - yol, çerçeve;
  - resimde kalış, geri çekilme;
  - kazanma → koridor → dalış;
  - sesler.

## Süreler

| | PM_109 | A | B |
|---|---|---|---|
| Yay + yaklaşma | 1,07 sn | **0,71 sn** | **0,71 sn** |
| Resimde kalış | 0,29 sn | 0,29 sn | 0,29 sn |
| Geri çekilme | 0,78 sn | 0,78 sn | 0,78 sn |
| Toplam (bölüm başlayana kadar) | 2,18 sn | **1,81 sn** | **1,81 sn** |

## Kenar boşluğu
0 kare, ilk kareler dahil. Kontrol edilen koşular:
- A
- B
- A, `&dalishiz=1.25`
- B, `&dalishiz=0.8`

## 20 ms'yi aşan kareler (bu PC, Chrome, ekran kaydı kapalı)

| Koşu | Sayı | En uzun |
|---|---|---|
| PM_109 | 8 | 31 ms |
| A | 8 | 32 ms |
| B | 8 | 31 ms |

- **Nerede:** Hep aynı yerler: basışın ilk karesi (~30 ms), tabloya değme (bölüm kuruluyor, ~30 ms), geri çekilmenin başında 22–30 ms'lik birkaç kare.
- **Yay ve yaklaşma:** İçinde uzun kare yok.

## Bayraklar

| Bayrak | Ne yapar |
|---|---|
| `&dalis110=a` | A (varsayılan) |
| `&dalis110=b` | B |
| `&dalis110=c` | 3. çizim, kendi süresiyle (1,07 sn; bu turun ilk hâli) |
| `&dalis110=0` | PM_109 |
| `&dalis109=0` | PM_108 |
| `&dalisgeri=` | Hedef ortalama hız (varsayılan 0,56 ekran boyu/sn). Büyüdükçe dalış kısalır. |
| `&dalishiz=` | Hız çarpanı |
| `&dalistepe=` | A ve c'de tepe ile son hız oranı (varsayılan 2) |

## Önizleme
- **`hiz.png`:**
  - Üstte A ve B'nin hedefi ve ölçülen hızı, ayrıca B'nin süresine ölçeklenmiş geri çekilmenin tersi.
  - Altta PM_109, A, B ve geri çekilmenin ölçülen hızı. Geri çekilmenin ortalaması çizgiyle gösteriliyor.
- **`dalis_yan_yana.mp4`:** PM_109 | A | B, 1170×844 (her yan 390×844), 60 fps, 9 sn, 5,3 MB. Üç kez oynuyor: önce PM_109'un, sonra A'nın, sonra B'nin sesiyle. Sesi duyulan taraf "SES" ile işaretli.
- **`dalis_a.mp4`, `dalis_b.mp4`:** Her biri 390×844, 60 fps, sesli, 2,8 sn, 0,6 MB.

## Doğrulanmayan
- **iPhone:** Telefonda bakılmadı.
- **Diğer bölümler:** Geri çekilmenin hızı yalnız bölüm 1'de ölçüldü. Başka bölümlerde tahtanın boyu farklıysa geri çekilmenin yolu da değişir; gidişin süresi ise sabit kalır (yolu her bölümde aynı).
