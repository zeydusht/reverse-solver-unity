# PM_MESAJ_90: Çerçeve değişikliğini GERİ AL; yalnız dalış sonundaki anlık dalgalanmayı düzelt; buzlu bar yönü

Kodu kendin yaz. p23 üzerinde. Önce `docs/` commit. Bitiş mesajı CLAUDE.md biçiminde, en altta, "Oyna" linkleriyle. `live/`'a yayınlama.

Zeyd (19 Kasım 16:30): "Çerçeve niye değişti, çok kötü. Bir öncekine alın hemen. Benim tek isteğim en sondaki anlık dalgalanmayı çözmekti. Başka hiçbir şeyi değiştirmeyin."

**Bu işin kapsamı dar. Aşağıdakiler dışında hiçbir şeye dokunma:** yaklaşma, yol, kamera eğrisi, koridor, süreler, menü, çerçeve görünüşü.

## 1. PM_89 §1'i GERİ AL
- Çerçeve 9-dilim / tahta oranına uyma değişikliğini tamamen geri al: kod, çerçeve görselleri, menü ve koridordaki tablo görünüşü.
- **Dalış, çerçeve, menü ve koridor PM_88 sonundaki hâline birebir dönsün** (git'ten; PM_88'in son commit'i).
- PM_89'dan **kalanlar:**
  - Jiggles ekranı (Zeyd: ok);
  - buzlu bar (2. maddedeki yön düzeltmesiyle);
  - süre altındaki yeşil barın kalkması.
- Geri dönüşü doğrula: menü ve koridor ekran görüntüsü PM_88'deki hâliyle yan yana, `docs/art/pm90/geri_alma.png`.

## 2. Buzlu bar yönü
Şu an ters gidiyor. Bar **sağdan sola doğru azalsın**: dolgunun sağ ucu sola çekilir, sol uç sabit. Başka bir şeyi değişmez.

## 3. Tek iş: dalış sonundaki anlık dalgalanma
- **Sorun:** Kamera çerçeveye varana kadar her şey çok iyi. Varış anında (resim ekranı doldururken) ve geri çekilmenin başında resim bir an kısalıp uzuyor. Sebebi çerçevedeki resim oranı ile tahta oranının farklı olması (PM_89 analizi: `docs/art/pm89/geri_cekilme_20fps.png`).
- **Zeyd'in çözümü (PM onaylı):**
  1. **Yaklaşma ve varış:** resim hangi oranda ve kırpımda görünüyorsa öyle kalsın. Son saniyede hiçbir yeniden boyutlandırma ya da oran değişimi yok.
  2. **Geri çekilirken çerçeve ekranda yok, yalnız resim ve çizgiler var.** Oran ve kırpım farkını bu sırada sessizce ayarla (yeniden diz), göz fark etmesin:
     - **yalnız eşit ölçek:** X/Y ölçek oranı her karede 1,00 ± 0,01, resim hiçbir karede yatayda ya da dikeyde ayrı ezilip uzamasın;
     - fazladan görünecek kenarlar (tahta daha genişse yanlar) geri çekilme boyunca **tek yönde ve yumuşakça** açılsın: önce daralıp sonra genişleme, geri gidip gelme yok;
     - değişim çekilmenin tamamına yayılsın, bir iki kareye sıkışmasın;
     - çizgiler bu sırada tahtanın gerçek kenarları olarak kalır (PM_84/86 kuralı), kaymaz.
- **Kenarları da kontrol et:** geri çekilmede ekran kenarında boşluk ya da yanlış renk kalmasın (kenar boşluğu 0).
- **Ölçüm:**
  - varıştan oyuna kadar her karede X/Y ölçek oranı;
  - görünen kırpımın genişliği tek yönde (monoton) değişiyor mu;
  - ardışık karelerde ölçek değişimi < %1/kare.
  - Önce/sonra 20 fps kareler `docs/art/pm90/varis_once_sonra.png`.
  - Kare olmayan 2 tahtada ve kare bir tahtada dene.
  - Menü → bölüm ve kazanma → sonraki bölüm, ikisi de.

## Teslim
- EditMode + PlayMode yeşil, `verify_build` p23 4/4.
- `PROJE_DURUMU`'na tek satır, commit + push.
- Son mesaj kısa, en altta, "Oyna" linkleri:
  - `p23/?c=p23&lang=tr` (Oyna → dalış)
  - `p23/?c=p23&test=1&lang=tr&lv=3` (kazan → dalış)
  - `p23/?c=p23&test=1&lang=tr&lv=18&booster=saat` (buzlu bar)
