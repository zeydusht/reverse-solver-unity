# PM_89 RAPOR

Ölçümler yerel build'de yapıldı (Chrome, gerçek GPU, 390×844). iPhone'da doğrulanmadı.

## 1. Dalışın sonunda resim uzayıp kısalıyordu (HATA)

### Önce ölçülen

| | Değer |
|---|---|
| Bölüm resimleri | hepsi **kare** (1024×1024) |
| Menü çerçevesinin açıklığı | ekranda 315×536 px (0,59); duvarda 1,00×1,37 birim (0,73) |
| Koridor çerçevesinin açıklığı | 377×550 px (0,69) |
| 5. bölümün tahtası | 4×5 (0,80) |
| Menüde görünen kırpım | resmin ortadaki %59'u (u 0,21–0,79, v 0–1), eşit ölçek |
| Tahtada görünen | resmin **tamamı**, 4×5'e çekiştirilmiş: X ölçeği = 0,80 × Y ölçeği |
| Varışta ekranda (PM_88) | tahtanın ortadaki %58'i; geri çekilirken iki yandan %21'er yeni resim açılıyordu |

Sonuç: hem kırpım değişiyordu hem de tahta kare resmi yatayda %20 sıkıştırıyordu. İkisi birden.
- Kapı tahtada çerçevedekinden %20 dar görünüyordu.
- Zeyd'in kaydında X/Y oranı dalış boyunca 1,00'den **0,64**'e iniyor; "kısalıp uzuyor" hissi buradan geliyor.
- Eski kazanma dalışında oran 0,85'e iniyor.

### Seçilen yol: A

Çerçeve her bölümde tahtanın oranında açılıyor. B (sabit çerçeve) olmazdı: hiçbir tahta 0,59–0,69 oranında değil, 100 bölümün oranı 0,80–1,00 arasında.

- **Tahta resmi artık çekiştirilmiyor.** Kare resmin ortası, tahtanın oranında kırpılıyor (X ve Y ölçeği aynı). Aynı kırpım şu yerlerde de kullanılıyor:
  - parçalar;
  - bitmiş resim;
  - dalıştaki resim;
  - kazanma galerisi;
  - iz rengi.
- **Çerçeve artık oyunun kendisi.** Eski çerçeveler menü ve koridor resimlerinden silindi, yerine duvar dolduruldu (`Tools/pm89_clear_frames.py`). Yeni çerçeve Codex'in düz altın çerçevesi; 5×5 dilimli: köşeler ve orta süsler sabit, düz pervaz uzuyor.
  - Menüde 3B odanın duvarına çiziliyor: açıklık 1,2 birim yüksek, genişliği tahtanın oranı.
  - Koridorda şeridin perspektifiyle çiziliyor: kazanılan ve sıradaki tablo, her biri kendi oranında.
  - Menüdeki resim de aynı duvar düzleminde, tahtanın kırpımıyla duruyor (menünün eski düz çizimi kalktı).
- **Varış:** kamera resmi ekranı doldurana kadar değil, **tahtanın oyundaki yerine** kadar götürüyor.
  - Resim tahtanın tam üstüne iniyor (fark 0,05 pt).
  - Geri çekilme yok, dolayısıyla yeni içerik açılmıyor.
  - Resim yerine oturunca duvar ve çerçeve 0,5 sn'de sönüp oyunun arka planına dönüyor; kesikler resmin üstünde çiziliyor, HUD kayarak geliyor.
  - Ayar bayrağı: `&dalissonme=` (sn).

### Ölçüm (`Tools/pm89_aspect_check.py`: ardışık kareler arası afin uydurma, X ve Y ölçeği ayrı)

| Dalış | Tahta | X/Y oranı, kare başına en kötü | X/Y birikmiş (varıştan ±1 sn) | Varış: resim ile tahta arası | Varıştan sonra |
|---|---|---|---|---|---|
| **Önce:** menü → 5 (Zeyd'in iPhone kaydı) | 4×5 | 0,990 | **0,63** … 1,00 | geri çekilme, ölçek 0,45'e iner | iki yandan %21'er yeni resim |
| **Önce:** kazan → 3 (p23, PM_88) | 4×5 | 0,83 | **0,70** … 1,00 | geri çekilme, ölçek 0,44'e iner | yeni resim açılır |
| Sonra: menü → 5 | 4×5 | 0,998 | 0,994 … 1,011 | 0,05 pt | ≤ 0,3 px/kare |
| Sonra: menü → 13 | 5×6 | 1,001 | 1,001 … 1,007 | 0,05 pt | ≤ 0,2 px/kare |
| Sonra: menü → 7 | 6×7 | 1,001 | 1,000 … 1,013 | 0,05 pt | ≤ 0,2 px/kare |
| Sonra: kazan → 3 | 4×5 | 1,003 | 1,000 … 1,044 | 0,02 pt | ≤ 0,5 px/kare |
| Sonra: kazan → 13 | 5×6 | 1,003 | 1,000 … 1,042 | 0,02 pt | ≤ 0,5 px/kare |

- **Varış farkı** oyunun kendi kaydından ölçüldü: dalışın son karesindeki resim köşeleri ile tahtanın ekrandaki köşeleri karşılaştırıldı. Tahta da hedefin tam üstünde kuruluyor (fark 0,00 pt).
- **Varıştan sonra:** kamera hiç kıpırdamıyor. Ölçülen ≤ 0,5 px'lik oynama ve %0,3'lük ölçek, görüntünün kendisinin değişmesinden geliyor (kesikler çiziliyor, parçalar resmi devralıyor); resim yer değiştirmiyor. Görünen kırpım varıştan oyuna kadar aynı.
- **6×7 kazan → sonraki** ölçülemedi: başsız Chrome sandık ekranından sonraki düğmeye basamadı. Aynı tahta menü dalışında ölçüldü.

- Yaklaşma sırasında birikmiş oranın 1,00 → ~1,04 olması duvarın yandan önden görünüşe dönmesinden geliyor (3B perspektif). Kare başına değişim en çok %0,3.
- Kareler 20 fps: `geri_cekilme_once_sonra.png` (menü → bölüm ve kazan → sonraki; önce ve sonra).
- **Tuvalin boyu bölüme göre:** kare tahtalarda sabit bir tuval, kameranın uçuşunun bittiği yerde tam varış ölçüsünü veriyordu; yaklaşma ya hiç ilerlemiyor ya da geri gidiyordu. Bu yüzden tuvalin duvardaki boyu her bölümde, varış uçuş yolunun bittiği uzaklığın %80'ine düşecek şekilde seçiliyor (0,70–1,15 birim). Menü ve dalış aynı tuvali kuruyor.
  - Kare 6×6 (menü → 8): kare başına en kötü 0,9994; birikmiş 0,995–1,003.
  - 4×5 (menü → 5, son build): 1,0015; birikmiş 1,000–1,018.
  - 3×3 (menü → 1): tuval en küçük sınırda (0,70). Kare başına 0,988'lik tek değer varıştan 0,93 sn sonra, kamera dururken çıkıyor: 1. bölümün rehber kutusu görüntüyü değiştiriyor, resim yer değiştirmiyor.
- Eski çerçevenin duvarda bıraktığı yer Laplace dolgusuyla kapatıldı: yalnız kenardaki duvar rengi içeri yayılıyor, menüde duvarın kendi dokusu ekleniyor. Küçük çerçevede leke kalmıyor.

## 2. Süre barı kalktı, saat booster'ında buzlu bar

- Süre panelinin altındaki yeşil bar tamamen kalktı. Eski hâli `&surebar=1` ile geri gelir.
- Saat booster'ı kullanılınca:
  - panelin altında buzlu bar açılıyor (0,22 sn);
  - bar 10 sn'de soldan sağa boşalıyor, üstünde buz kristalleri parlıyor;
  - Süre paneli kırağı kenarı alıyor.
- Donma bitince bar 0,3 sn'de üç aşamada çatlıyor, sonra 12 buz parçasına ayrılıp düşüyor. Bu sırada çatırtı sesi (`pm89-ice`, oyunda üretildi), hafif titreşim (Light) ve küçük bir sarsıntı var; panelin kırağısı 0,45 sn'de çözülüyor.
- Eski "+10 sn" çipi kalktı; "+10 sn" yazısı artık bara düşüyor.
- Codex 3 seçenek çizdi (`buzlu_bar/A|B|C`, `karsilastirma.png`). Puan (PM_74 ölçütü):
  - **A** (lacivert oluk + altın kenar): 9. Stile en uygun olanı; bağlandı.
  - C (krem oluk): 7,5.
  - B (buz oluk): 7.
  - Saklanan en iyi 2: A ve C.
- GIF: `buzlu_bar/oyunda.gif`.

## 3. Jiggles ekranı

- **Raf:** Codex 3 seçenek çizdi (`jiggles/A|B|C`, `karsilastirma.png`). Puan:
  - **B** (lacivert kadife pano, altın raflar, küçük taç): 9. Jiggle'lar lacivert zeminde en net okunuyor; bağlandı.
  - A (kemerli vitrin, krem iç): 8,5.
  - C (açık cam vitrin): 7,5, kalabalık.
  - Saklanan en iyi 2: B ve A.
- **Alt kısım:**
  - Başarılar ekranının bar dili kullanılıyor: krem kart, krem yuva, altın dolgu; sayı kalın 3B.
  - Ara hedefler barın üstünde ödül simgesi: 500 sandık, 1.000 teşekkür hediyesi (daha büyük, sıcak ışık halesiyle), 2.000 madalya. Sayılar simgelerin altında.
  - Ulaşılmamış hedef gri ve küçük kilitli. Hediye ulaşılmamışken de sıcak tonda kalıyor.
- **Avatar sırası:**
  - Beş yer eşit aralıklı.
  - Seçili avatar kalın altın halka + hafif ışık içinde.
  - Oyunda kilitli avatar yok: hepsi seçilebilir. Bu yüzden "sönük + kilit" hâli görünmüyor; avatar kilidi bir ürün kararı.
- Önce/sonra: `jiggles/once.png`, `jiggles/sonra.png` (390×844).

## Doğrulanmayan

- iPhone Safari'de his: dalışın sonu, sönme süresi, buz sesi ve titreşim.
