# PM_112 RAPOR: resimde durma yerine bungee dönüşü (önizleme)

Önizleme, yayın yok. Yerel dal `pm108-onizleme`, push yok.

- **Ölçüm:** yerel build, Chrome, gerçek GPU, 390×844, p23 bölüm 1. iPhone'da denenmedi.
- **Araçlar:**
  - kayıt: `Tools/pm108_dalis.js`
  - hız grafiği: `Tools/pm112_plot.py`
  - video: `Tools/pm108_video.py` (yeni `--slow 2`: yarım hız, ses perdesi korunarak uzatılıyor)

## Ne değişti
- **Varsayılan vol 2.** Menü dalışında resimde kalış artık 0,07 sn (B + vol 2, toplam 1,60 sn). `&resimbekle=` hâlâ çalışıyor.
- **`&bungee=2 / 4 / 6`:** Kalışın ve değmenin yerine bungee dönüşü geliyor:
  - Kamera tabloya varınca durmuyor. İçeri girmeye devam ediyor, yavaşlıyor.
  - Hız dipte bir kez sıfırdan geçiyor, sonra geri çekilmeye dönüyor.
  - İvme kesintisiz. Ekranda donmuş kare yok.
- **Neden gidişin sonu da değişti:**
  - B tabloya neredeyse durarak varıyor (0,06 ekran boyu/sn, ~0,19 ln/sn). Bu hızla 0,06 sn'de ancak ~%0,5 içeri girilebiliyor.
  - İlk denemede %2–6'yı bu sürenin içine zorla sığdırdım. Sonuç: varıştan hemen sonra öne doğru ani bir atılma oldu (hızda sivri tepe). İstenen "yavaşlar" değildi.
  - Bu yüzden B'nin frenlemesi tablonun içine taşındı. B tepesine kadar (0,43 sn) aynen kalıyor. Tepeden sonraki düşüş tabloyu hızla karşılıyor ve aynı frenlemeyle (B'nin kendi düşüşü, 5,5 ekran boyu/sn²) dibe kadar sürüyor.
  - Gidiş bu yüzden biraz erken bitiyor. Kazanılan zaman geri çekilmeye ekleniyor, toplam süre vol 2 ile aynı kalıyor.
- **Dönüş süresi derinliğe bağlı:** Fren aynı olduğu için derin olan daha uzun dönüyor (aynı lastik).
  - %2 ve %4 istenen 0,10–0,14 sn aralığında.
  - %6 0,16 sn ile biraz üstünde. %6'yı 0,14 sn'ye sığdırmak, gidişe yeni bir hız tümseği eklemeyi gerektiriyordu; eklemedim.
- **Çizgiler:**
  - Varışta başlıyor, bungee boyunca çiziliyor, geri çekilmenin %80'inde bitiyor.
  - Çizgiden tahtaya devir kuralı aynı.
- **Ses:**
  - Çizgi sesi (c) varışta çalıyor.
  - Dalış whoosh'u ve dalış havası dipte sönüyor. Geri çekilme whoosh'u dipte başlıyor; çizgi sesine yakın düştüğü için biraz kısık (PM_111 kuralı).
  - Dönüşte sessiz boşluk yok. En sessiz 10 ms −38 / −41 dB; vol 2'de −51 dB'lik bir boşluk vardı.
  - En yüksek 10 ms seviye dördünde aynı (−16,7 / −17,2 / −16,8 / −16,9 dB). Üst üste binme yok.

## Süreler

| | Vol 2 | Bungee 1 (%2) | Bungee 2 (%4) | Bungee 3 (%6) |
|---|---|---|---|---|
| Oyna → tabloya varış | 0,71 sn | 0,65 sn | 0,63 sn | 0,61 sn |
| Varış → dip | 0,07 sn bekleme | 0,05 sn | 0,07 sn | 0,08 sn |
| Dönüş (varış → yeniden varış boyuna) | (duruş) | 0,09 sn | 0,13 sn | 0,16 sn |
| Geri çekilme (dipten) | 0,78 sn | 0,89 sn | 0,88 sn | 0,88 sn |
| **Toplam** (ölçülen) | **1,60 sn** | **1,59 sn** | **1,60 sn** | **1,59 sn** |

- Dönüş süresi kameranın kendi değerleri.
- Ekranda ölçülen dönüş biraz daha uzun: 0,11 / 0,17 / 0,19 sn. Sebebi, geri çekilmede resmin tahtanın kendi kadrajına yavaşça oturması (PM_90).

## Hız (`hiz.png`)
İşaretli hız: + içeri, − dışarı. Altta dönüş bölgesi yakın plan, her nokta bir kare.
- **Bungee'lerde:**
  - Hız tabloyu 0,30 / 0,42 / 0,52 ekran boyu/sn ile geçiyor (vol 2'de 0,06).
  - Sıfırdan tek geçiş var. Dönüş S biçiminde: düz bölge, sivri tepe ya da köşe yok.
- **Geri çekilme:** Biraz daha uzun sürdüğü için tepe hızı 0,99'dan 0,87–0,90 ekran boyu/sn'ye indi.
- **Kenar boşluğu:** Üç bungee'de 0 kare.
- **Keskinlik:**
  - Resim 1024×1024. Telefonda zaten ~2,5 kat büyütülüyor; %6 fazlası farkı değiştirmiyor.
  - Dipteki kare varıştaki kare kadar keskin görünüyor. Piksellenme yok (doğrusal süzgeç).

## 20 ms'yi aşan kareler (bu PC, Chrome)

| Koşu | Uzun kare | En uzun |
|---|---|---|
| Vol 2 | 3 | 32 ms |
| Bungee 1 | 7 | 41 ms |
| Bungee 2 | 7 | 31 ms |
| Bungee 3 | 3 | 43 ms |

- **Nerede:** Basışın ilk karesi ve tabloya varış (bölüm kuruluyor).
- **Bungee'lerde fark:** Bölüm kuruluşu artık kamera hızlıyken oluyor. Vol 2'de neredeyse durgun anına düşüyordu; uzun kare hareketin içinde daha çok fark edilebilir. Telefonda bakılmalı.
- **Geri çekilmenin başı:** Bazı koşularda 26–30 ms'lik birkaç kare de var (PM_111'de vol 1 ve 3'te de vardı).

## Önizleme
- **`bungee_yan_yana.mp4`:**
  - Vol 2 | bungee 1 | bungee 2 | bungee 3, 1560×844 (her panel 390×844), 60 fps, 10 sn, 8,7 MB.
  - Dört kez oynuyor. Her seferinde bir panelin sesi duyuluyor ("SES" işareti).
- **`bungee_yan_yana_yarim.mp4`:**
  - Aynısı yarım hızda; yalnız varış ve dönüş bölgesi (0,3–1,3 sn). 8 sn, 5,6 MB.
  - Ses de yarım hızda, perdesi aynı.
- **`bungee1.mp4`, `bungee2.mp4`, `bungee3.mp4`:** Her biri 390×844, 60 fps, sesli, 2,5 sn, 0,6 MB.

## Doğrulanmayan
- **iPhone:** Telefonda bakılmadı. Hareketin içindeki bölüm kuruluşu karesi telefonda daha uzun olabilir.
