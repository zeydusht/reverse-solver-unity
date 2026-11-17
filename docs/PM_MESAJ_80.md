# PM_MESAJ_80: Dalışın en akıcı hâli: kesintisiz kamera, resimde bekleyiş, çizgiler beklerken başlasın

Kodu kendin yaz. p23 üzerinde. Önce `docs/` commit. Bitiş mesajı CLAUDE.md biçiminde, en altta, "Oyna" linkleriyle.

Zeyd (17 Kasım 08:21): "Fena değil ama tam istediğimi anlatamamışım. Kameranın gidişi biraz daha continuous olsun. Gidince orada biraz beklesin; direkt geri gelmek iyi olmadı. Oraya gittiğimizde çizgiler yavaş yavaş çıkmaya başlasa, geri gelirken de görmeye devam etsek. En iyi ve en smooth versiyonu istiyorum."

## Koreografi (varsayılan = A)
Tek bir ana zaman çizelgesi, tek kamera eğrisi. Hiçbir adım arasında hız sıçraması ya da sıfırlanması yok; bilerek konan bekleyiş hariç.

| Zaman | Kamera | Tablo / çizgiler | HUD |
|---|---|---|---|
| 0,0–1,8 sn | **Yay**: Zeyd'in 4 karesi (PM_78). Başta yumuşak kalkış, sonra **sabit akış**. | — | menü öğeleri ilk 0,3 sn'de solar |
| 1,8–2,6 sn | **Yaklaşma**: yayın hızını ve yönünü devralarak tabloya ilerler (teğet ve hız eşleşir, Hermite birleştirme). Ekran resimle dolarken **yumuşakça yavaşlar** ve oturur. Sert duruş ya da "çarpma" yok. | — | — |
| 2,6–3,4 sn | **Bekleyiş (0,8 sn)**: kamera durmaz, çok yavaş %1,5 itme devam eder (nefes; donmuş görünmesin). | **Yapboz çizgileri burada başlar**: merkezden dışa doğru, kesik çizgiler kalemle çizilir gibi yavaşça uzar. Çıkıntılar hafif belirir. Bekleyişin sonunda çizgilerin ~%40'ı görünür. | — |
| 3,4–4,4 sn | **Geri çekilme (1,0 sn)**: ease-in-out, oyun tahtası ölçeğine oturur. | Çizgiler çizilmeye **devam eder**, geri çekilmenin %80'inde tamamlanır. En sonda parçalar çok hafif ayrışır (gölge, 1–2 px). | Son 0,3 sn'de HUD üstten, booster çubuğu alttan, çekilmeyle üst üste |

- **Toplam ≈ 4,4 sn.** Dokunma: 1. dokunuş 2×, 2. dokunuş bitirir.
- **Akıcılık teknik şartları:**
  - Konum, FOV ve dönüş tek eğriden; adım sınırlarında konum + hız sürekli.
  - Kamera yolu için yay uzunluğuna göre yeniden parametreleme: yol üstünde hız, nokta aralığından bağımsız düzgün olsun.
  - Yaklaşmanın son %20'sinde hafif ease-out.
  - Hareket bulanıklığı ya da hafif iz yalnız hızlı kısımlarda, abartısız.
  - 60 fps. Geçiş boyunca 20 ms'yi aşan kare sayısını raporla, hedef 0. Bölüm kurulumu bekleyişe ya da daha erkene alınsın; ön yükleme şart.
- Kenar boşluğu denetimi her karede 0.

## Karşılaştırma için iki varyant (Zeyd seçecek)
- **B (`&dalis=b`)**: daha çevik. Yay 1,5 · yaklaşma 0,6 · bekleyiş 0,5 · geri çekilme 0,8 (toplam ≈ 3,4).
- **C (`&dalis=c`)**: daha sinematik. Yay 2,0 · yaklaşma 0,9 · bekleyiş 1,1 · geri çekilme 1,2 (toplam ≈ 5,2). Çizgiler bekleyişte %50'ye ulaşır.

İnce ayar bayrakları her varyantta geçerli: `&yayhiz=`, `&resimbekle=` (sn), `&cizgihiz=` (çarpan).

## Kendi kalite kontrolün (teslimden önce)
- A, B ve C'yi 60 fps kaydet. Her birinin hız grafiğini çiz (kamera hızı – zaman; `docs/art/pm80/hiz_A.png` vb.). Grafikte bekleyiş dışında sıfıra düşüş ya da kırık olmamalı.
- Yan yana GIF: `docs/art/pm80/ABC.gif` (normal) ve `ABC_yavas.gif` (yarım hız).
- Kendin izle ve şunları kontrol et:
  - yay → yaklaşma geçişi fark edilmiyor mu,
  - bekleyiş donuk değil mi,
  - çizgiler okunuyor mu,
  - geri çekilmede tahta yerine yumuşak oturuyor mu.
  
  Takılan yeri düzeltmeden teslim etme.

## Teslim
- p23 güncel (varsayılan A), testler yeşil, `verify_build` p23 4/4. `PROJE_DURUMU`'na tek satır, commit + push.
- Son mesaj kısa, en altta, "Oyna" linkleri:
  - `p23/?c=p23&test=1&lang=tr` (A)
  - `p23/?c=p23&test=1&lang=tr&dalis=b`
  - `p23/?c=p23&test=1&lang=tr&dalis=c`
