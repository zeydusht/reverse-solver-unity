# PM_80 RAPOR: tek zaman çizelgesi, bekleyiş, çizgiler bekleyişte başlıyor

**Özet**
- p23 güncel; varsayılan A. B: `&dalis=b`, C: `&dalis=c`. PM_77'nin eski hâli `&gecis=eski` ile açılıyor.
- Ölçümler PC'de, gerçek GPU'lu başsız Chrome'da yapıldı (390×844). iPhone'da ölçülmedi.

## 1. Koreografi (A)

| Zaman | Kamera | Tablo / çizgiler | HUD |
|---|---|---|---|
| 0,0–1,8 | **Yay.** Zeyd'in yolu yay uzunluğuna göre çiziliyor: ilk üçte birde yumuşak kalkış, sonra sabit hız. | — | Menü öğeleri ilk 0,3 sn'de soluyor. |
| 1,8–2,6 | **Yaklaşma.** Kamera yayın bittiği yerden, onun hızı ve yönüyle tabloya ilerliyor (aşağıda). | — | — |
| 2,6–3,4 | **Bekleyiş.** Yavaş %1,5 itme sürüyor. | Yapboz çizgileri merkezden dışa doğru, kalemle çizilir gibi uzuyor (her kenar düzgün hızda, ucunda parıltı); bekleyiş sonunda ~%40'ı çizilmiş oluyor. | — |
| 3,4–4,4 | **Geri çekilme**, ease-in-out. İtmenin hızını devralıyor, tahta ölçeğine yumuşakça oturuyor. | Çizgiler devam ediyor, geri çekilmenin %80'inde tamam. Resim %60'tan itibaren parçalara bırakıyor. Çizgilerin gölgesi sonda ~1,5 px genişliyor (parçalar ayrışıyor), sonra çizgiler soluyor. | HUD üstten, booster çubuğu alttan, son 0,3 sn'de. |

- **Yaklaşmanın yapısı:**
  - Kamera gerçekten 3B dioramada tabloya yürüyor; ekranda yakınlaştırma yok.
  - Yan kayma beşinci dereceden Hermite. Konum, hız ve ivme yaydan sıçramasız devralınıyor.
  - Mesafe (ln z) için hız profili plato: yayın hızından yükseliyor (ilk %30), sabit gidiyor, son %25'te yumuşakça bekleyişin itme hızına iniyor. Sert duruş ya da çarpma yok.
  - Tablonun tahtanın oranına oturması yalnızca kenarda daha fazla resim gösteriyor; içerik kaymıyor.
- **Bekleyişe geçiş:** yaklaşma bittiği karede tablo tam olarak tahtanın çerçevesinde (aradaki 1,3 pt'lik fark yaklaşma boyunca kapatılıyor). Bu yüzden çizgilerin başladığı karede sıçrama yok.
- **İz:** tablonun hızlı kısımda (köşeler karede 5 pt'den hızlıyken) bir önceki karesi en çok %20 saydamlıkla görünüyor. Yalnız yaklaşmanın ortasında devreye giriyor.
- **Bölüm kurulumu:** yaklaşma neredeyse durduğunda yapılıyor (u ≥ 0,94, ekran ~10 pt/sn). Sonraki bölümün resmi bekleyişte indirilip çözülüyordu (140 ms'lik kare); artık geçiş bitince yükleniyor.
- **Diğer kurallar:**
  - Dokunma kuralı aynı: 1. dokunuş 2×, 2. dokunuş bitirir.
  - Kazanma → sonraki bölüm de aynı zaman çizelgesiyle: eski koridor, yaklaşma 0,8, bekleyiş 0,8, geri çekilme 1,0.

## 2. Varyantlar ve bayraklar

| | Yay | Yaklaşma | Bekleyiş | Geri | Bekleyiş sonunda çizgi | Toplam |
|---|---|---|---|---|---|---|
| A (varsayılan) | 1,8 | 0,8 | 0,8 | 1,0 | %40 | 4,4 sn |
| B `&dalis=b` | 1,5 | 0,6 | 0,5 | 0,8 | %40 | 3,4 sn |
| C `&dalis=c` | 2,0 | 0,9 | 1,1 | 1,2 | %50 | 5,2 sn |

Her varyantta geçerli bayraklar:
- `&yayhiz=`: bütün geçişin hızı.
- `&resimbekle=`: bekleyiş (sn). İtme hızı aynı kalıyor.
- `&cizgihiz=`: çizgilerin hızı (çarpan).

## 3. Hız grafikleri (`hiz_A.png`, `hiz_B.png`, `hiz_C.png`)

Grafikteki hız, ekranda görünen hız: resmin (sonra tahtanın) kareden kareye dönüşümünün ekranın 4 köşesi ve merkezindeki ortalama yer değiştirmesi (pt/sn). Kaynak `&dalislog=1` ile alınan kare günlüğü; grafiği `Tools/dive_speed_plot.py` çiziyor.

| | Yay (sabit akış) | Yay→yaklaşma sınırı | Yaklaşma platosu | Bekleyiş (itme) | Geri çekilme tepesi |
|---|---|---|---|---|---|
| A | ~110 pt/sn | 101 | ~540 | ~5–9 | ~740 |
| B | ~130 | 123 | ~730 | ~8 | ~910 |
| C | ~100 | 93 | ~480 | ~5 | ~620 |

- Bekleyiş dışında sıfıra düşüş yok.
- **Kırık:** hiçbir sınırda hız sıçramıyor. Yay→yaklaşma sınırında hız yayın sonundaki ~%10'luk hafif düşüşten (merkeze kayma yavaşlıyor) yumuşakça yükseliyor.
- Hız yalnız bilerek yerlerde sıfıra yaklaşıyor: başlangıçta (menüden kalkış) ve geri çekilmenin sonunda (oturma).

## 4. Ölçümler
- **Kenar boşluğu** (`&dalisbosluk=1`), her karede 0:
  - A: 242 karede 0;
  - B: 267 karede 0;
  - C: 321 karede 0;
  - kazanma → sonraki: 619 karede 0.
- **Tek karelik sıçrama:** 0 (A, B, C, kazanma → sonraki).
- **Akıcılık** (günlüksüz koşu, medyan 59,9 FPS). Geçiş boyunca 20 ms'yi aşan kare sayısı:
  - A: 1 (bölüm kurulumu, 33 ms, yaklaşma dururken);
  - B: 0;
  - C: 0;
  - `&gecis=eski`: 2;
  - kazanma → sonraki: 2.
- **Dokunma anı:** sayfaya ilk dokunuşta 67–100 ms'lik bir kare var. Önce boş bir yere dokununca 33 ms'ye iniyor, yani tarayıcının sesi açması bunu yapıyor. Dalışın kendi kurulumu 7–9 ms. Kare hareket başlamadan, ekran dururken.
- **Bölüm kurulumu** 27–45 ms. Hepsi oyunun bölüm açma kodu (arayüz, booster'lar, eğitim). Bunu birden çok kareye bölmek bu işin dışında kaldı.

## 5. Kendi kalite kontrolüm (`ABC.gif`, `ABC_yavas.gif`)
- **Yay → yaklaşma fark ediliyor mu:** hayır. Çerçeve aynı yönde, durmadan büyümeye başlıyor.
- **Bekleyiş donuk mu:** değil. Yavaş itme ve merkezden çizilen çizgiler devam ediyor.
- **Çizgiler okunuyor mu:** evet; krem çizgi, koyu gölge, ucunda parıltı. Tırnakların yayı tek tek çiziliyor.
- **Geri çekilmede tahta yumuşak oturuyor mu:** evet. PM_76'nın küçük yaylanması (tahtanın %2,5 batıp geri gelmesi) kaldırıldı, çünkü ease-in-out ile çekişiyordu. `&gecis=eski`'de duruyor.
- **Düzeltilenler:**
  - İlk denemede tablo, kamera ilerlemesinden bağımsız olarak kendi kendine büyüyordu. Hız grafiğinde ~1200 pt/sn'lik bir tepe yapıyordu; düzeltildi, artık plato.
  - Sonraki bölümün resmi bekleyişte çözülüyordu (140 ms'lik kare); geçiş sonrasına alındı.

## Doğrulanmayan
- iPhone Safari'de his ve FPS.
- Yay artık herkese açık olduğu için plaka dokusu (1408×1728) herkese yükleniyor. iPhone'da açılmış hâli ~10 MB bellek; ölçülmedi.
