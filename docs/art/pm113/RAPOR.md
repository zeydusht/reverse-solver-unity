# PM_113 RAPOR: dalışta U biçimli dönüş (önizleme)

Önizleme, yayın yok. Yerel dal `pm108-onizleme`, push yok.

- **Ölçüm:** yerel build, Chrome, gerçek GPU, 390×844, p23 bölüm 1. iPhone'da denenmedi.
- **Araçlar:** kayıt `Tools/pm108_dalis.js`, grafik ve sayılar `Tools/pm113_plot.py`, video `Tools/pm108_video.py`.
- **Bayrak:** `&u=1 / 2 / 3`. Bayraksız oyun vol 2 olarak kalıyor.

## Süreler

| | Vol 2 | U1 | U2 | U3 |
|---|---|---|---|---|
| Oyna → tablonun kadrajı (ilk varış) | 0,72 sn | 0,66 sn | 0,64 sn | 0,63 sn |
| Oyna → dip | 0,72–0,79 sn arası düz (ortası 0,75) | 0,75 sn | 0,75 sn | 0,75 sn |
| Dip derinliği (%) | %0,8 (değme) | %4,0 | %4,0 | %4,0 |
| Hız +0,3 → −0,3 süresi | 0,27 sn (arada düzlük var) | **0,20 sn** | **0,31 sn** | **0,41 sn** |
| ↳ dipten önce / sonra | 0,18 / 0,10 | 0,100 / 0,102 | 0,151 / 0,154 | 0,200 / 0,208 |
| Toplam (ölçülen) | 1,60 sn | 1,60 sn | 1,59 sn | 1,60 sn |

- **Toplam süre:** Dördünde de oyunun saatiyle bitiş aynı, 1,587 sn. 1,59 ile 1,60 farkı, son karenin hangi ana denk geldiğinden.
- **Dibin konumu:** Dip, vol 2'nin beklemesinin ortasında (0,753 sn; istenen 0,745 ±0,02).

## Ne yapıldı
- **Dibin çevresi parabol:**
  - Hız sıfırdan sabit eğimle, düz bir çizgi olarak geçiyor. Çizgi ±0,33'e kadar uzanıyor, yani ±0,3 aralığının tamamı düz.
  - Sıfırdan tek geçiş var. Düz taban (vol 2) ya da sivri dönüş (bungee) yok.
- **Dibin iki yanı eşit:**
  - Geri çekilmede kamera hem uzaklaşıyor hem de tahtanın yerine kayıyor. Bu yüzden aynı uzaklaşma ekranda ~%26 daha hızlı görünüyor.
  - İlk denemede çıkış tarafı bu yüzden kısa kaldı (0,08 sn'ye karşı 0,10 sn). Çıkıştaki ivme bu kaymaya göre düzeltildi; iki yan artık eşit.
- **Gidiş:**
  - U'nun genişliği dibin çevresinden alınıyor. Gidiş aynı yolu daha erken bitirmek zorunda.
  - B'nin şekli korunuyor, yalnızca biraz hızlanıyor: U1 ×1,005, U2 ×1,10, U3 ×1,29.
  - Ardından yumuşak bir yavaşlama düz çizgiye bağlanıyor. Bu yavaşlama B'nin kendi frenlemesinden sert değil.
  - Gidişin en yüksek hızı vol 2'de 0,85, U3'te 1,02 ekran boyu/sn.
- **Geri çekilme:**
  - Düz çizgiden sonra vol 2'ninki gibi yumuşak bir eğriyle tahtaya oturuyor.
  - En yüksek hız: vol 2 0,99 / U1 0,93 / U2 0,99 / U3 1,07 ekran boyu/sn.
- **Çizgiler:** Tablonun kadrajına ilk varışta başlıyor, geri çekilmenin %80'inde bitiyor. Tahtaya devir kuralı aynı.
- **Ses:** PM_112 kuralı aynı. Çizgi sesi kadrajda çalıyor, dalış whoosh'u dipte bitiyor, geri çekilme whoosh'u dipte başlıyor.
  - Dönüşte sessiz boşluk yok. En sessiz 10 ms: −41 / −43 / −42 dB. Vol 2'de −50 dB'lik bir boşluk var.
  - Üst üste binme yok. En yüksek 10 ms dördünde aynı (−16,7 … −17,2 dB).
- **Görüntü:**
  - Kenar boşluğu üç U'da da 0 kare.
  - Dipte resim ekranı tam dolduruyor. Derinlik bungee 2 ile aynı olduğu için keskinlik de aynı.

## Grafik (`yakinlik.png`)
- **Sol üst:** Tabloya uzaklık, aşağısı daha yakın. Kesik çizgi tablonun kadrajı. Dört eğri aynı anda bitiyor.
- **Sağ üst:** Dip yakın planda, kadraja göre %; her nokta bir kare. Vol 2'de düz taban var, U'larda yuvarlak dip.
- **Alt:** Hız. Sağ altta dip bölgesi, noktalı çizgiler ±0,3. U'larda hız sıfırdan düz bir çizgiyle geçiyor; vol 2'de sıfır çevresinde düzlük var.

## 20 ms'yi aşan kareler (bu PC, Chrome)

| Koşu | Uzun kare | En uzun |
|---|---|---|
| Vol 2 | 4 | 40 ms |
| U1 | 2 | 48 ms |
| U2 | 9 | 39 ms |
| U3 | 3 | 34 ms |

- **Nerede:** Basışın ilk karesi ve kadraja varış, yani bölümün kurulduğu an.
  - Bölüm kuruluşu U'larda da kamera hareket halindeyken oluyor, bungee'de olduğu gibi.
  - U1'in ilk kaydında bölüm kuruluşu 43 ms sürdü (normalde 20–26 ms; makine yükü). Kayıt yinelendi.
- **U2'deki 9 uzun kare:** Çoğu geri çekilmenin başında (26–30 ms). PM_111 ve PM_112'de de vardı.

## Önizleme
- **`u_yan_yana.mp4`:** vol 2 | U1 | U2 | U3, panel başına 390×844, 60 fps, sesli, 10 sn. Dört kez oynuyor; her seferinde bir panelin sesi duyuluyor ("SES" işareti).
- **`u_yan_yana_yarim.mp4`:** Aynısı yarım hızda, yalnız varış ve dönüş bölgesi (0,3–1,3 sn). Ses de yarım hızda, perdesi korunuyor.
- **`u1.mp4`, `u2.mp4`, `u3.mp4`:** Tek tek, 2,5 sn, sesli.
- **Zeyd'in çizimi:** `ref/zeyd_cizim_u.jpg`.

## Doğrulanmayan
- **iPhone:** Telefonda bakılmadı. Hareket halindeyken bölüm kuruluşu karesi telefonda daha uzun sürebilir.
