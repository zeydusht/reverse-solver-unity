# PM_108 RAPOR: menü → bölüm dalışının hızı (önizleme)

Önizleme, yayın yok. Yerel dal `pm108-onizleme`, push yok; p23 PM_107'deki gibi.

- **Ölçüm ortamı:** yerel build, Chrome, gerçek GPU, 390×844. iPhone'da denenmedi.
- **Hız ölçümü:** `Tools/pm92_flow_check.py`. Duvarın ekrandaki hareketi ekran boyu/sn olarak ölçülüyor (PM_92'deki ölçü).
- **Kayıt:** `Tools/pm108_dalis.js`, görüntü + ses + günlük tek koşuda.
- **Videolar:** `Tools/pm108_video.py`.
- **Grafik:** `Tools/pm108_plot.py`.

## Ne değişti
- **Yol aynı, zaman dağılımı yeni:** PM_92'nin "ekran hızına göre" kurulan eğrisi kullanılıyor (`Dive75.Speed108`). Ekran hızı Zeyd'in çizimindeki şekle göre kuruluyor:

  | Süre | Hız |
  |---|---|
  | Başlangıç | tepenin %55'i, ilk karede belirgin hareket |
  | 0 → %25 | tepeye yumuşak yükseliş (ivmede sıçrama yok) |
  | %25 → %45 | tepe platosu |
  | %45 → %60 | tepenin 1/1,65'ine (%61) yumuşak iniş |
  | %60 → son | sabit |
  | son 0,03 sn | %15'lik kısa iniş, sonra PM_91'deki değme |

- **Süre:** yay + yaklaşma 1,5 sn'den 1,2 sn'ye indi.
- **Değişmeyenler:**
  - yol, çerçeve, resimde kalış (0,29 sn), geri çekilme (0,8 sn);
  - kazanma → koridor → dalış;
  - sesler.
- **Dalış havası:** Ses yeni hızı izliyor: başta orta, tepede en yüksek, yaklaşmada sabit. Sondaki whoosh yine tabloya değerken bitiyor (PM_107 C6).
- **Kenar boşluğu (PM_88'den beri):**
  - **Önce:** Uçuşun başında sağ alt köşede ~0,2 sn açık kalıyordu (yeni hızda 8 kare, eskisinde 15).
  - **Sebep:** Zemin, kameranın merceği ortalanırken baktığı yere göre kuruluyordu. Mercek kendi saatiyle kaydığı için kamera başta biraz daha sağ-alta bakıyordu.
  - **Düzeltme:** Zemin ve duvar merceğin iki ucu için de kuruluyor. Resmin dışına taşan kısım resmin kenar pikselleriyle dolduruluyor.
  - **Sonuç:** Yeni, şimdiki, `&dalistepe=2.2&dalishiz=1.25` ve `&dalishiz=0.8` koşularının hepsinde 0.

## Süreler (şimdiki → yeni)

| | Şimdiki (PM_92) | Yeni (PM_108) |
|---|---|---|
| Yay + yaklaşma (Oyna'dan tabloya değene) | 1,49 sn | **1,19 sn** (tasarım 1,20) |
| Resimde kalış | 0,29 sn | 0,29 sn |
| Geri çekilme | 0,80 sn | 0,80 sn |
| Toplam (bölüm başlayana kadar) | 2,60 sn | **2,29 sn** |

## Hız (ölçülen, `hiz.png`)

| ekran boyu/sn | Şimdiki | Yeni |
|---|---|---|
| İlk kare | 0,08 | **0,26** (3 kat) |
| Tepe | 0,37 (1,40. sn, yolun %93'ü) | **0,44** (0,30–0,54. sn) |
| Tepeden sonra | –%11 | sabit yaklaşmada 0,265, **±%1** (0,72 → 1,15 sn) |
| Varış | 0,33 | 0,24 |

Ölçülen eğri tasarımın üstünde: tepe 0,44 (tasarım 0,43), sabit hız 0,265 (tasarım 0,26).

## 20 ms'yi aşan kareler (bu PC, Chrome)
Oyunun kendi sayımı, `&dalislog=1` ile her birinin anı `[dalis]` satırında.

| Koşu | Toplam | En uzun | Nerede |
|---|---|---|---|
| Şimdiki | 3–9 | 39–44 ms | Basışın ilk karesi, tabloya değme (bölüm kuruluyor), sonra resimde kalış ve geri çekilmede 20–33 ms'lik birkaç kare |
| Yeni | 3–9 | 34–48 ms | Aynı yerler |

- Yay ve yaklaşmanın içinde ek uzun kare yok. Sayılar koşudan koşuya değişiyor (makinenin yükü). Yeni hızın getirdiği bir fark görülmedi.
- `&dalishiz=1.25` koşusunda değme karesi 69 ms. Daha hızlı varışta bölümün kuruluşu aynı kareye denk geliyor.

## Bayraklar (Zeyd telefonda denesin)
- `&dalis108=0`: şimdiki (PM_92) eğri.
- `&dalistepe=` (1,05–3; varsayılan 1,65): tepe ile son sabit hız oranı. Büyüdükçe ortası daha atılgan, sonu daha sakin.
- `&dalishiz=` (0,25–4; varsayılan 1): bu bayrak PM_86'dan beri **hız** çarpanı, eski anlamı korundu:
  - 1,25 → 0,96 sn;
  - 0,8 → 1,5 sn.

## Önizleme
- **`hiz.png`:**
  - Üstte Zeyd'in istediği şekil (açık yeşil kalın çizgi), tasarım (kesik çizgi) ve ölçülen (mavi).
  - Altta şimdiki ve yeni, ölçülen.
- **`dalis_yan_yana.mp4`:**
  - Şimdiki | yeni, 780×844 (her yan 390×844), 60 fps, 6,4 sn, 2,5 MB.
  - İki kez oynuyor: önce şimdikinin sesiyle, sonra yeninin sesiyle. Sesi duyulan taraf "SES" ile işaretli.
- **`dalis_yeni.mp4`:** Yeni, 390×844, 60 fps, sesli (dalış havası + whoosh + müzik), 3,2 sn, 0,7 MB.

## Doğrulanmayan
- **iPhone:** Telefonda bakılmadı.
- **Uzun kareler:** Telefonda uzun kareler daha uzun olabilir (bölüm kuruluşu değme anında).
