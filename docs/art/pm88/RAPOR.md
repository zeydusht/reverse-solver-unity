# PM_88 RAPOR

Ölçümler yerel build'de (Chrome, gerçek GPU, 390×844); iPhone'da doğrulanmadı.

## 1. Dalışın sonunda resim büyüyüp dönüyordu (HATA)

**Sebep (PM'in tahmini doğru):** Resim önce menünün düz çizimiyle çerçeveye yapışık gidiyordu. Tahtanın resmine geçiş iki farkı birden kapatıyordu:
- menünün kırpımından resmin tamamına geçiş: resim küçülüyor;
- düz çizimden düzgün perspektife geçiş: resim dönüyor gibi görünüyor.

Bu geçiş, menü dalışında çerçeve ekrandan çıktıktan sonraki kısa aralığa, kazanma dalışında yaklaşmanın son %30'una sıkışmıştı. Kayıtta şöyle görünüyor:
- **Menü dalışı:** varıştan ~0,55 sn önce resim büyürken (%+0,6/kare) birden küçülmeye dönüyor (%−0,9/kare) ve 0,29°/kare dönüyor.
- **Kazanma dalışı:** büyüme hızı kare başına 2 puana kadar sıçrıyor, 0,19°/kare dönüyor.

**Düzeltme (Zeyd'in istediği gibi):**
- Tahta, kameranın vardığı anda ekranda görünen resmin tam üstüne kuruluyor: aynı yer, aynı ölçek, aynı kırpım. Kamera tahtayı o görüntüye göre konumluyor; düz tahtaya geri çekilme tahtanın kendisiyle yapılıyor.
- Kameranın karşılayamadığı küçük fark (menünün düz çiziminin eğikliği) yaklaşmanın tamamına yayıldı:
  - menüde en çok 47,6 pt, kazanmada 33,7 pt;
  - hız, kameranın en hızlı olduğu ortada en yüksek; uçlarda sıfır.
- Devir karesi (resimden tahtaya): ölçek farkı %0,04 (menü) / %0,03 (kazanma), açı farkı 0°. Ekran ortasındaki nokta resimde de tahtada da aynı.

**Ölçüm:** `Tools/arrival_check.py`. Kayıttaki ardışık kareler arasında resmin hareketi benzerlik dönüşümü olarak bulunuyor (OpenCV ECC; ölçek ve dönüş, 1/60 sn başına).

| | Önce (p23, PM_87) | Sonra |
|---|---|---|
| Menü, varış ±0,3 sn: ölçek / dönüş | %0,06 / 0,05° | %0,22 / 0,06° (hedef < %1, < 0,3°) |
| Menü, yaklaşma + bekleme (−0,9…+0,35 sn): ölçek aralığı | %−0,89 … +1,09 (küçülmeye dönüyor) | %−0,02 … +1,01 (yön değişimi yok) |
| Menü, yaklaşma + bekleme: en çok dönüş | 0,29°/kare | 0,06°/kare |
| Menü, kareden kareye hız sıçraması | 0,43 puan | 0,10 puan |
| Kazanma, yaklaşma + bekleme: en çok dönüş | 0,19°/kare | 0,12°/kare |
| Kazanma, kareden kareye hız sıçraması | 2,02 puan | 0,68 puan |
| Kazanma, yön değişimi | yok | yok |

- **Not (kazanma → sonraki):** yaklaşma 0,6 sn'lik bir zoom (PM_85). Varıştan önceki 0,3 sn içinde kameranın kendi büyütmesi %5,6/kare'ye çıkıyor. Bu bir sıçrama değil, kameranın yumuşak eğrisi; yukarıdaki satırlarda sıçrama ölçüsü ayrıca var. "< %1/kare" hedefi bu pencerede ancak yaklaşma uzatılırsa tutar (ör. 1,0 sn). PM kararı.
- Varış ±0,5 sn sonrası geri çekilme (küçülme) tasarım gereği; ölçüm penceresinin dışında.
- Kareler 10 fps: `varis_once_sonra.png` (menü → bölüm ve kazan → sonraki; önce ve sonra).

## 2. Açılış yükleme ekranı

**Akış:**
- Linke basınca açılış videosu `index.html`'in kendisinde oynuyor (`TemplateData/pm88-film.js`); Unity arkada yükleniyor.
- Oyun hazır olunca videoyu devralıyor (`Intro.jslib`). "Atla" ancak o zaman çıkıyor.
- **Hafif kopya:** A + B toplam 2,90 MB (eskisi 4,23 MB), 720×1280, `faststart`. Ses videonun içinde: AAC; bu sayede iOS'ta "Sesi aç"a tek dokunuş yetiyor.
- Video sessiz başlıyor, sol üstte "🔇 Sesi aç" düğmesi var (yeniden basınca "Sesi kapat").
- Kimin gördüğü:
  - ilk ziyaret (oyun hiç açılmamış), `lv`/`set`/`debug`/`c=pt1` olmayan bir link;
  - `&intro=1` her zaman gösterir, `&intro=0` hiç göstermez.
  - Sayfa yeni sandığı ama oyunun tanıdığı bir oyuncu için oyun videoyu hemen kapatıyor; video o zaman yükleyici gibi davranmış oluyor.
- Video bitince ya da "Atla"ya basılınca doğrudan oyuna geçiliyor:
  - oyun ilk ekranını aynı karede kuruyor, video 0,25 sn sonra 0,3 sn'de sönüyor;
  - ilk ziyarette isim kartı artık videodan **sonra** çıkıyor, sonra 1. bölüm tablodan çıkıyor.
- Video oyun hazır olmadan biterse son karesinde bekliyor ve altında ince bir altın çizgi yükleme durumunu gösteriyor.
- Video bozulur ya da 8 sn takılırsa yükleme ekranına geçiliyor. iOS Düşük Güç modunda otomatik oynatma engellenirse ortada "▶ Oynat" çıkıyor.
- **Sonraki açılışlar:** turuncu çubuk kalktı. Yerine menünün galeri resmi, zıplayan Jigo, "Yükleniyor…" yazısı ve ince altın çizgi var (`yukleme_ekrani.png`). Resim ve Jigo sayfanın başında öncelikli yükleniyor.

**Ölçüm** (`Tools/film_check.js`, yeni profil = ilk ziyaret; zamanlar linke basıştan itibaren):

| | Sınırsız | 4G (10 Mbit/sn, 60 ms) |
|---|---|---|
| İlk video karesi | 0,44 sn | 0,92–1,00 sn |
| Oyun yüklendi / hazır ("Atla" çıkar) | 3,85 / 3,93 sn | 37,2 / 37,4 sn |
| Video bitti mi? | hayır, Atla 1,5 sn sonra basıldı | evet (~30 sn), 7 sn altın çizgiyle bekledi, oyun gelince kendiliğinden geçti |
| Atla (ya da oyun hazır) → video gitti | 0,87 sn | 1,03 sn |
| Bu aralıkta boş kare | 0 / 63 kare | 0 / 66 kare |

- Yavaş 4G'de (4 Mbit/sn, 150 ms): ilk kare 1,96 sn; oyun 60 sn'de gelmedi. Oyun verisi 32,8 MB (Brotli); bu 4G'de tek başına ~30 sn indirme. Video bu sürenin ~30 sn'sini kapatıyor, ama veri boyutu açık bir iş.

## 3. Alt menü: anında tepki

- **Dört sayfa da kurulu ve canlı**, yan yana duruyor (ekran dışında, gizli değil). Basınca sayfalar yalnız kayıyor: kurulum yok, resim çekme yok.
- Ekranda olmayan sayfalar menüden 0,6 sn sonra, her karede biri kuruluyor (72 / 46 / 29 ms), kayış sırasında hiç kurulmuyor. Arka planda resim açma işi de kayış sırasında bekliyor.
- **Basış dokunmanın kendisinde işleniyor** (bırakmayı beklemiyor). Sayfalar ve alt çubuktaki seçim aynı karede hareket ediyor:
  - basış karesindeki iş 0,3–4,8 ms (ilk basışta ilk ses yüzünden 6,5 ms);
  - dokunma o kareye en çok 14 ms önce geliyor, yani basıştan ilk harekete ≤ 1 kare.
- **Süre:** 0,35 sn, ease-out quint; yolun %97'si ilk 0,18 sn'de. Alt çubuktaki seçim 0,15 sn.
- **Kesilebilir:** kayış sürerken başka sekmeye basınca sayfalar o anki yerinden yeni hedefe dönüyor (sıçrama yok, testte kontrol ediliyor).
- Parmakla kaydırma, eksen kilidi ve geri yaylanma aynı; artık sayfanın resmi değil, sayfanın kendisi kayıyor.

**Ölçüm:**

| | Sonuç |
|---|---|
| Basış → ilk hareket | aynı kare (dokunma ≤ 14 ms bekliyor) |
| 4 hızlı basış (~0,26 sn içinde), 3 tur: hareket karesi / 20 ms'yi aşan / en uzun | 100 / **0** / 17 ms |
| Tek tek basış, 5 kayış (fareyle) | 0 kare 20 ms'yi aşıyor, en uzun 17 ms |
| Dokunma taklidiyle (başsız Chrome) | her kayışta 1 kare 28–32 ms |

- Dokunma taklidindeki 30 ms'lik kare dokunmanın hemen ardından geliyor ve 8 ms + 25 ms çifti şeklinde. Oyunun o karedeki işi 0,3–2 ms; aynı kayışlar fareyle 0. Yani başsız Chrome'un dokunmayı iletme biçiminden geliyor; iPhone'da doğrulanmadı.
- GIF'ler:
  - `sekme_karsilastirma_yarim_hiz.gif` (referans | bizim, yarım hız);
  - `sekme_tak_tak_tak_yarim_hiz.gif` (4 hızlı basış: Ev → Koleksiyon → Jiggles → Başarılar → Ev).

## 4. Koridorda kısa duruşlar

- Yalnız kazan → sonraki bölümde:
  - kayıştan önce bulunulan tablonun önünde +0,20 sn (`&koridorbas=`);
  - yeni tablonun önünde dalıştan önce +0,20 sn (`&koridorson=`).
- Duruşlarda kamera %0,5 itip geri geliyor (sin², iki ucunda hareketsiz).
- Kayış aynı (çan eğrisi). Toplam koridor 1,92 sn; 20 ms'yi aşan kare 0.

## Doğrulanmayan

- iPhone Safari'de his: sekmelere basış ve kaydırma, video sesi (Sesi aç), Düşük Güç modu.
- Gerçek 4G'de açılış süresi (ölçüm Chrome'un ağ kısıtlamasıyla).
