# PM_92 RAPOR: dalış hızı, 2. tur (önizleme)

Önizleme, yayın yok. Yerel dal `pm91-onizleme`; push yok, p23 aynı (PM_90).

- **Ölçüm ortamı:** yerel build, Chrome, gerçek GPU, 390×844, 60 fps. iPhone'da denenmedi.
- **Süre ölçümü:** `Tools/pm91_timing_check.py`.
- **Hız ölçümü:** `Tools/pm92_flow_check.py`.

## Hissedilen hız nasıl ölçüldü

Çerçevenin açıklığı her karede duvar düzleminin ekrandaki yerini veriyor. İki kare arasında duvarın ekrandaki hareketi tek bir dönüşüm. Bu hareket bütün ekrana yayılmış 9×19 noktada ortalandı ve ekran boyu/sn olarak yazıldı. Böylece resmin büyümesi ve yay boyunca kayma birlikte ölçülüyor.

## Sorun neydi (PM_91)

- **Menü:** hız 0,53. saniyede tepe yapıp düşüyordu (0,33 → 0,29). Ardından varışa kadar yavaşça azalıyordu.
  - Yay bitip düz yaklaşma başlarken iki hareketin toplamı bir an fazlaydı.
  - Sonra eğri ilerlemeye göre kurulduğu için ekrandaki hız sabit kalamıyordu.
  - Tepeden sonraki en büyük düşüş %20.
- **Kazanma:** 0,2. saniyede ivme birden azalıyordu (dirsek). Tepeden sonraki en büyük düşüş %5.

## Ne değişti

- **Ölçüye göre kurulan eğri:** zaman eğrisi artık doğrudan bu ölçüyle kuruluyor.
  - Oyun açılışta yolu (yay + yaklaşma) 240 adımda örnekliyor ve her adımda ekranın ne kadar kaydığını hesaplıyor.
  - Zaman bu "ekran yoluna" göre dağıtılıyor.
  - Yay → yaklaşma geçişi tek hareket gibi akıyor; birleşmede sıçrama yok.
- **Menü:**
  - Kalkış eski eğrinin kendi başlangıç hızı ve ivmesi.
  - Hız sonra hiç düşmeden 1,5 katına çıkıyor; tepe yolun %93'ünde.
  - Sonra kısa bir iniş (tasarımda %13).
- **Kazanma:**
  - Duruştan kalkıyor ve ivme kırılmadan tabloya kadar artıyor.
  - Bunun için ilk %20'lik sürenin sonundaki ivme sonraya taşınıyor; ilk denemedeki 0,17. saniye dirseği gitti.
  - Kalkış PM_91'den biraz daha yumuşak: 0,2. saniyede hız 0,59 (PM_91'de 0,82).
- **Değişmeyenler:**
  - Resimde kalış, geri çekilme, yol, çerçeve ve çizgiler.
  - Varıştaki %2,5'lik değme (artık yeni varış hızından hesaplanıyor).

## Süreler (PM_91 → PM_92)

| | Menü → bölüm (5 ve 13) | Kazanma → koridor → bölüm (2→3) |
|---|---|---|
| Yaklaşma | 1,61 sn → **1,50 sn** | 0,57 sn → **0,52 sn** (tasarım 0,50) |
| Resimde kalış | 0,27 sn → 0,27 sn | 0,22 sn → 0,23 sn |
| Geri çekilme | 0,80 sn → 0,80 sn | 0,80 sn → 0,80 sn |
| Toplam | 2,68 sn → **2,58 sn** | 1,58 sn → **1,53 sn** (koridordan sonra) |

## Hız (ölçülen, `hiz.png`)

| | PM_91 | PM_92 |
|---|---|---|
| **Menü:** yolun %92'sine kadar en büyük geri düşüş | %20 (0,53. sn tepeden sonra) | **%0** (5 ve 13) |
| **Menü:** tepe | 0,33 (yolun %27'si) | 0,37 (yolun %93–94'ü) |
| **Menü:** son kısımda düşüş | %33 | **%9–11** |
| **Menü:** varış hızı | 0,22 | 0,34 |
| **Kazanma:** yolun %92'sine kadar en büyük geri düşüş | %5 | **%0** |
| **Kazanma:** son kısımda düşüş | %18 | **%6** |
| **Kazanma:** varış hızı | 0,77 | 1,41 |

- Hızlar ekran boyu/sn.
- Ölçülen kareler tasarım eğrisinin tam üstünde (grafiğin üst sırası).
- **Dikkat:** sürekli artan hız ve aynı ya da daha kısa süre, varışın daha hızlı olması demek. Menüde %55, kazanmada %83 daha hızlı. Zeyd PM_91'in son kısmını çok iyi bulmuştu; varışın bu hızda nasıl hissettirdiği videoda görülmeli.
- **PM_90'ın oran ölçümü** hâlâ geçiyor: kare başına X/Y en kötü 0,998 (menü), 0,999 (kazanma).

## Videolar

`dalis_yan_yana.mp4` (PM_91 | PM_92, aynı anda başlıyor), `dalis_yeni.mp4`, `kazanma_yeni.mp4`:

- 60 fps, 390×844 (yan yana 780×844), H.264 `-preset veryfast`, ses yok, hepsi 2 MB'ın altında.
- PM_91 hâli PM_91 build'inden alındı.
