# PM_79 RAPOR: yay varsayılan, 2× hızlı, tek akış

**Özet**
- p23 güncel (gh-pages 91d8485a). Yay (PM_78, orijinal menü dioraması) artık varsayılan menü → bölüm geçişi.
- Testler: EditMode 737/739 (2 atlandı, 0 hata), PlayMode 6/6. `verify_build` yerelde 4/4, canlıda 4/4.
- Ölçümler PC'de, gerçek GPU'lu başsız Chrome'da yapıldı (390×844). iPhone'da ölçülmedi.

## 1. Varsayılan
- Bayraksız adres yayı açıyor; `&gecis=yay` gerekmiyor.
- `&gecis=eski`: PM_77'nin geçişi. Yay yok; adımlar eski süreleriyle ve nefesleriyle (1,1 / 0,5 / 1,0 / 0,4 sn).
- `&gecis=yok`: geçiş yok (değişmedi).

## 2. Yeni zamanlama (`gecis.gif`, `gecis_yavas.gif`)

| Adım | Önce | Şimdi |
|---|---|---|
| Yay (Zeyd'in 4 karesi) | 3,6 sn | **1,8 sn**, yumuşak kalkış |
| Tablonun karşısında duruş | vardı | **yok**: yaklaşma yay bitmeden 0,35 sn önce başlıyor |
| Yaklaşma | 1,1 sn | **0,55 sn**, hızlanarak (dalış) |
| Sadece resim | 0,5 sn + nefesler | **0,12 sn**, nefes yok |
| Uzaklaşırken yapboz çizgileri | 1,0 sn | **0,6 sn**, ease-out |
| HUD + booster çubuğu | 0,4 sn | **0,3 sn**, çizgilerin son %30'uyla üst üste |

- **Tek eğri.**
  - Yay yumuşak başlıyor (smoothstep). Yaklaşma yay bitmeden başlıyor ve ivmelenerek resme giriyor; hız resme değene kadar artıyor.
  - Konum ve hız sürekli; hız hiçbir yerde sıfıra inmiyor.
  - Simülasyonda (tuval köşelerinin ekran hızı): yayın tepesi 140 pt/sn. Geçişte hız bunun altına inmiyor, resme değerken ~1000 pt/sn'ye çıkıyor.
- **Resme değme anı:** kısa beyaz parlama (0,18 sn) ve hafif ölçek darbesi (%3), sonra 0,12 sn bekleme.
- **Toplam:** ≈ 2,85 sn. PM'in tablosundaki ≈3,3 sn'den kısa, çünkü yay ile yaklaşma 0,35 sn üst üste biniyor.
- **Dokunma kuralı aynı:** 1. dokunuş 2×, 2. dokunuş bitirir.
- **Ayar bayrakları:**
  - `&yayhiz=` bütün geçişin hızı (varsayılan 1, 0,25–4 arası; 1,2 = %20 hızlı, ≈ 2,4 sn);
  - `&resimbekle=` resimde bekleme (sn, varsayılan 0,12).
- **Kazanma → sonraki bölüm de etkileniyor.** Eski koridor (3 sn) aynı. Ardından gelen dalış da yeni adımlarla (0,55 / 0,12 / 0,6 / 0,3) çünkü Zeyd'in "resimde 1–2 sn bekliyor" dediği bekleme o da. `&gecis=eski` ikisini de eski hâline döndürür.

## Ölçümler
- **Kenar boşluğu** (`&dalisbosluk=1`): menü → bölüm 270 karenin 0'ında, kazanma → sonraki 562 karenin 0'ında.
- **Tek karelik sıçrama:** 0.
- **Akıcılık:**
  - Varsayılan geçiş medyan 59,9 FPS.
  - 5 koşunun 4'ünde geçiş boyunca 20 ms'yi aşan 3 kare var (bölümün kurulduğu anda, resim ekranı kaplarken); en kötüsü 50–67 ms.
  - Bir koşuda geçiş bittikten hemen sonra (bölüm başlarken) tek bir 333 ms kare çıktı; diğer koşularda tekrarlanmadı.
  - `&yayhiz=1.2`: 3 kare, en kötü 50 ms. `&gecis=eski`: 2 kare, en kötü 50 ms.

## Doğrulanmayan
- iPhone Safari'de his ve FPS.
- Yay artık varsayılan olduğu için plaka dokusu (1408×1728) herkese yükleniyor. iPhone'da açılmış hâli ~10 MB bellek; ölçülmedi.
