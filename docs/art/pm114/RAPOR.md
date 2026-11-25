# PM_114 RAPOR: U1 varsayılan, kenarlar çizilerek geliyor, sıçrama taraması (önizleme)

Önizleme, yayın yok. Yerel dal `pm108-onizleme`, push yok.

- **Ölçüm:** yerel build, başsız Chrome, gerçek GPU, 390×844 (2x), p23 bölüm 1. iPhone'da denenmedi.
- **Araçlar:** kayıt `Tools/pm108_dalis.js`, tarama ve grafik `Tools/pm114_scan.py`, kareler `Tools/pm114_frames.py`, video `Tools/pm108_video.py`.
- **Bayraklar:** Bayraksız oyun artık U1. `&u=0` vol 2'yi, `&resimbekle=` ve `&bungee=` eskisi gibi kendi hâllerini açıyor. `&kenar=0`, bu PM'deki bütün düzeltmeleri kapatıp PM_113'ü gösteriyor.

## Toplam süre
Oyunun saatiyle iki hâlde de 1,587 sn, ölçülen 1,59–1,60 sn. U1'in eğrisi ve süresi değişmedi.

## Bir anda beliren öğeler (geri çekilme, dipten bitişe)
"Bir anda" = tek karede %30'dan fazla.

| Öğe | Şimdiki | Düzeltilmiş |
|---|---|---|
| **Tahtanın dış kenarı** (parçaların çevre çizgisi) | 1,43 sn'de **tek karede %87** geliyor | Dipten sonra (0,78 sn) başlıyor. İki kalem üst ortadan çıkıp iki yandan alt ortada buluşuyor. Karede en çok **%3,4**. İç çizgilerle aynı anda bitiyor (1,42 sn, geri çekilmenin %80'i). |
| **Kalem uçları** (çizgi ucundaki parlak nokta) | Her biri tek karede açılıp kapanıyor (%100) | Çizginin ilk ve son %30'unda yumuşakça açılıp kapanıyor. Karede en çok %28. |
| İç çizgiler | Karede en çok %4 | Aynı |
| Parça görünümü (gölge, kabartma, rozetler) | 1,42–1,59 sn arası, karede en çok %15 | Aynı |
| Tahtanın çerçevesi (altın-lacivert) | Bir anda belirmiyor; kamera geri çekildikçe ekrana giriyor | Aynı |
| Üst ve alt çubuk (süre, parça sayısı, butonlar) | 1,29–1,59 sn arası kayarak geliyor, ilk karede yolun %14'ü. **Dokunulmadı.** | Aynı |
| Jigo'nun konuşma balonu | Kamerayla büyüyor; yazı harf harf yazılıyor. İkinci satırın ilk harfi ~1,31 sn'de beliriyor (piksel farkındaki tek tepe). **Dokunulmadı.** | Aynı |

- **Kenarın sebebi:** Dış kenar, iç çizgilerin zamanlamasına hiç girmemişti. Çizgiler bitince çizim saati 0,85'ten 1,30'a tek karede atlıyordu, kenarın kalanı da o karede geliyordu.
- **Yapılanlar:**
  - Kenar artık kendi kalemleriyle çiziliyor. Hız başta ve sonda yavaş, ortada sabit.
  - Çizim saati son çizgiyle bitiyor.
- **Stil ve devir:** Kenar, iç çizgilerle aynı çizim yoluyla çiziliyor (aynı krem iç şerit, aynı lacivert dış hat, aynı kalınlık). Bittiği an oyunun kendi kenarıyla aynı. Oyunun ilk karesine geçişte piksel farkı sıfıra iniyor; renk çakması ya da kayma yok.
- **Ekran dışında çizilen kısım:** Tahta dipte ekrandan büyük olduğu için kenarın ilk kısmı (~1,2 sn'ye kadar) ekranın dışında çiziliyor. Ekrana girdiğinde bir kısmı çoktan çizilmiş oluyor (`kenar_kareler.png`).

## Sıçramalar
Kural: bir karedeki değişim komşu karelerin ortalamasının 2 katından fazla. Ayrıca ardışık karelerin piksel farkı. Ayrıntılar `sicrama.png`'de.

| An | Ne oluyordu | Düzeltme |
|---|---|---|
| **Basış** | Basış karesi uzun (30–48 ms), saat tek adımda 33 ms ilerliyordu. İlk hareket karesi sonrakilerin 1,5–2 katıydı (piksel farkında 0,03 sn'de tepe). | İlk adım en çok 1/60 sn. Tepe yok. |
| **Dip** (0,75 sn) | Kalem uçları tek karede beliriyordu (piksel farkında tepe) | Uçlar yumuşak açılıyor. Tepe yok. |
| **Çizgilerin devri** (0,67 sn, sonraki kare) | Devir karesinden sonra saat 33 ms atlıyordu (boyut değişimi 2,3 %/kare, sonra 0,4) | Bu kare de en çok 1/60 sn (bölüm kuruluşundan sonraki karede PM_91'den beri böyleydi) |
| **Çizgilerin bitişi** (1,43 sn) | Çizim saati atlıyordu (dış kenar %87) | Giderildi (yukarıda) |
| Kadraja varış, devir | Piksel farkı komşularından düşük | Sıçrama yok |
| Animasyon bitişi ve oyunun ilk karesi | Fark sıfıra iniyor; yalnız Jigo'nun yazısı değişiyor | Sıçrama yok |

### Bulundu, düzeltilmedi (PM kararı gerekiyor)
- **0,27 sn, uçuştan yaklaşmaya geçiş:** Tablonun dönmesi (~1,2°/kare) ve yana kayması (~0,7 pt/kare) tek karede duruyor. Her iki hâlde de var.
  - **Sebep:** Zeyd'in yolu tablonun önüne yandan, hızla geliyor; yaklaşma ise tam karşıdan başlıyor.
  - **Neden düzeltilmedi:** Akıcı yapmak için ya kamera tablonun karşısını biraz geçip geri dönmeli ya da uçuş sonunda yavaşlamalı. İkisi de yolun görünüşünü değiştirir. Denedim ve geri aldım.
- **Dipte takılma, 0,72–0,85 sn:** Art arda 5 kare 25–30 ms (60 FPS'ten 30'a düşüş). Her iki hâlde de var.
  - **Ne kadar sık:** Kayıtsız koşularda şimdikinde 3 koşunun 2'sinde, düzeltilmişte 3'ünde çıktı. Ekran kaydıyla şimdikinde 6 koşunun 5'inde, düzeltilmişte 6'sının 1'inde.
  - **Ses ya da günlük değil:** Sesler kapalıyken ve günlük kapalıyken de çıkıyor.
  - **Sebep:** Ekran kartı yükü. Aynı koşu 1x ekran yoğunluğunda 4 koşunun 4'ünde temiz. Dipte, en yakın zoomda parçalar bütün ekranı kaplıyor ve parça çizimi ağır.
  - **Çözüm:** Parça shader'ını hafifletmek, oyunun tamamını etkiliyor; ayrı bir iş. iPhone'da (3x) daha belirgin olabilir.

## Uzun kareler (20 ms üstü)
Ekran kaydı olmadan, her hâlde 3 koşu.

| Kare | Şimdiki | Düzeltilmiş |
|---|---|---|
| Basış (hazırlık 12 ms) | 30–31 ms. Ardından saat 33 ms atlıyor. | 31 ms. Ardından saat 17 ms ilerliyor: sıçrama yok, kısa bir bekleme. |
| Bölüm kuruluşu (varışta) | iş 19–22 ms, kare 33–37 ms | iş 18–22 ms, kare 29–36 ms |
| Çizgilerin devri | iş 7–8 ms, kare 22–29 ms | iş 5–7 ms, kare 22 ms |
| Dip (0,72–0,85 sn) | 2/3 koşuda 5 kare 25–30 ms | 3/3 koşuda aynı (yukarıda) |
| Oyun başladıktan ~0,1 sn sonra | Sonraki bölümün resmi açılıyor: 48 ms (ekran durgun) | Aynı |

- **Kuruluş neden azaltılmadı:**
  - Basışa taşınamıyor: bölümü kurmak menüyü kaldırıyor, menünün kontrolleri o anda kaybolurdu.
  - Birkaç kareye bölünemiyor: kuruluş tek bir işlem.
  - Bunun yerine kuruluştan ve çizgilerin devrinden sonraki adımlar 1/60 sn ile sınırlı. Uzun kare sıçrama değil, kısa bir bekleme olarak görünüyor.

## Çıktılar
- **`once_sonra.mp4`:** şimdiki | düzeltilmiş. Panel başına 390×844, 60 fps, sesli, 5 sn. İki kez oynuyor, her seferinde bir panelin sesi.
  - İki kayıt da dipteki takılmaya denk gelmeyen koşulardan seçildi (adil karşılaştırma).
- **`once_sonra_yarim.mp4`:** yarım hızda, dip → oyunun başı (0,70–1,85 sn).
- **`u1_son.mp4`:** düzeltilmiş hâl tek başına, 2,5 sn, sesli.
- **`kenar_kareler.png`:** geri çekilmeden eşit aralıklı 12 kare, şimdiki üstte, düzeltilmiş altta. Kenar 1,21 sn'den itibaren yandan çizilerek geliyor; şimdikinde 1,37'de yok, 1,45'te tamam.
- **`sicrama.png`:** iki sütun (şimdiki, düzeltilmiş), üç satır:
  - resim ve tahta: kare kare merkez, boyut, dönme
  - çizgiler: karede ne kadarı geliyor, %30 çizgisi
  - piksel farkı: ani tepeler kırmızı halka
  - Kritik anlar kesik çizgiyle işaretli.

## Doğrulanmayan
- **iPhone:** Telefonda bakılmadı. Dipteki takılma (ekran kartı) ve kuruluş karesi telefonda daha belirgin olabilir.
