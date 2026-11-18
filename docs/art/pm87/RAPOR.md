# PM_87 RAPOR

## 1. Sekme geçişi: sayfalar yan yana kayıyor
**Referans ölçüldü** (`ref_alt_menu.mp4`, 30 fps, kareler arası kayma):
- kayış 15, 27, 24, 21, 19, 16, 14, 12, 10, 8 … px;
- hızlı kalkış, uzun yumuşak oturuş: yolun %88'i 0,3 sn'de, tamamı ~0,6 sn'de;
- bu bir ease-out cubic eğrisi; bizde de aynısı.

**Yapılan:**
- Sayfalar alt çubuktaki sırayla yan yana: Ev, Koleksiyon, Jiggle'lar, Başarılar.
- Sekmeye basınca eski sayfa dışarı, yeni sayfa aynı hızla içeri; her sayfa kendi arka planıyla geliyor. Sağdaki sekmeye basınca içerik sola akıyor, soldakine basınca sağa. Atlamalı geçişte (1 → 4) tek kayış.
- Eski sayfa basış anındaki resmiyle kayıyor (kamera bir kez çiziyor); yeni sayfa canlı. Ayrı bir solma ya da sonradan gelen arka plan yok.
- **Parmakla kaydırma:**
  - sayfa parmağı izliyor;
  - yarıdan fazla ya da hızlı fiske (≥ 300 pt/sn) → yeni sekmeye oturuyor; değilse geri yaylanıyor;
  - eksen kilidi: parmak ilk 10 pt'de hangi yöne giderse o; yanlara giderse sayfalar, aşağı/yukarı giderse liste kayıyor;
  - en uçtaki sayfada dışarı doğru kaydırma bir şey yapmıyor.
- **Alt çubuk sabit**, yalnız seçim değişiyor: 0,2 sn, sade ease-out (PM_86'daki geri tepme kalktı). Codex A görünüşü aynı.
- Basışta Select titreşimi ve tık sesi (PM_86 haritası); kaydırmayla geçişte de aynısı.
- Ana menüye (Ev) dönüş de aynı kayışla.
- Arka plandaki menü resmi açma işi (büyük JPEG, web'de 0,1–0,4 sn) kayış bitene kadar bekliyor. Bir koşuda kayışın ortasına denk gelip 427 ms'lik takılma yaptığı görüldü, düzeltildi.

**Ölçüm** (yerel build, PC GPU, Chrome, 390×844):

| | Ölçülen | Kayış boyunca kare | 20 ms'yi aşan |
|---|---|---|---|
| Sekmeye basma | 3 tur × 3 kayış = 9 kayış (son build) + canlı p23'te 3 | 34–35 / kayış | 0 (en uzun 19 ms) |
| Kaydırıp bırakma | 3 tur × 3 (oturma, geri yaylanma, fiske) + canlıda 3 | 43–147 | her birinde 1: parmağın kalktığı kare, 25–31 ms |

- Parmak kalkınca: 3 ms'lik bir kareyi 30 ms'lik bir kare izliyor. Toplam iki kare süresi, bir kare kaçmış. Başsız Chrome'da dokunma olayının geldiği anda oluyor; o karede oyun ağır bir iş yapmıyor. iPhone'da doğrulanmadı.
- Basış karesi (yeni sayfanın kurulduğu kare, kayış başlamadan): 6–83 ms. Bu kare kayışın dışında; sayfa o karede henüz yerinde duruyor.
- Arka planı eksik ya da boş kare: 0. PlayMode testi her karede eski sayfa resmiyle yeni sayfanın ekranı boşluksuz örttüğünü ve çubuğun kıpırdamadığını kontrol ediyor; 3 sekme × 5 geçiş testi de geçti.
- GIF'ler:
  - `sekme_karsilastirma.gif` (referans | bizim) ve `sekme_karsilastirma_yarim_hiz.gif`;
  - `sekme_kaydirma.gif`: kaydırıp oturma, kısa kaydırmada geri yaylanma, hızlı fiske.

## 2. Dalış: resim önce kayıyordu (HATA)
**Sebep** (bulundu): PM_86'daki "resim çerçeveye sabit" düzeltmesi resmin çizimini ilk 0,25 sn'de iki yönde değiştiriyordu:
- menünün kırpımından (`PaintingCrop`) dioramanın ön kırpımına (`_cropFront`);
- menünün düz çiziminden çerçevenin perspektifine.

Kamera bu sürede neredeyse hiç kıpırdamıyordu: smootherstep çan eğrisi 0,3 sn'de yolun yalnız %2'sinde. Bu yüzden resim tek başına önden kayıyordu (deniz feneri sağa, solda çam), ardından kamera geç yürüyordu. Doku aynıydı; fark kırpım ve çizim biçimindeydi.

**Düzeltme:**
- Resim baştan sona menünün kendi çizimi: kırpımı ve düz çizimi. Çerçeve açıklığının her noktası resmin aynı noktasını taşıyor, yani resim çerçeveye yapışık. Tahtanın çerçevesine dönüşüm yalnız çerçeve ekrandan çıkınca başlıyor (PM_86 kuralı).
- **Kamera basış anında kalkıyor.** Yeni eğride hız (u + 0,04)(1 − u)²: ilk karede hareket var, en yüksek hız yolun üçte birinde, iniş yumuşak. Uçuş + yaklaşma yine 2,2 sn; toplam 3,41 sn (PM_86 ile aynı).
- Oyna'nın 0,12 sn'lik "basılı görünüm" beklemesi kalktı; dalış basışla aynı karede başlıyor.
- Düğme, HUD ve alt çubuk ilk 0,3 sn'de birlikte soluyor.

**Ölçüm** (yerel build, ekran kaydı, oyunun kendi günlüğü):
- Dalışın ilk karesi menünün son karesiyle aynı:
  - açıklığın köşeleri menüdekinden 0,00 pt farklı;
  - kayıtta basış karesi ile önceki kare arasında resim bölgesinde fark 0/255.
- Basış → ilk kamera hareketi: 1 kare. İkinci karede açıklık 0,8 pt kaymış.
- **Kamera ile resim senkronu:** oyunun günlüğündeki açıklık köşelerinden her kare çifti için homografi kuruldu; çerçeve üzerindeki ve resim içindeki noktalar kayıtta izlendi (98 kare çifti, 1,6 sn):
  - çerçeve noktaları: tahminden sapma ortanca 0,06 px, en çok 0,25 px;
  - resim noktaları: ortanca 0,15 px, %95'i ≤ 0,81 px, en çok 1,71 px.
  - Resim noktalarının 7 kare çiftinde 1 px'i aşması, en hızlı anda (kare başına 5+ px kayma), resmin yumuşak bölgelerinde izlemenin kendi gürültüsü; hareketsiz karelerde bile 0,12 px ölçüyor.
- 20 ms'yi aşan kare: 2 (basış karesi 28 ms, bölüm kurulumu 36 ms).
- Önce/sonra: `dalis_once_sonra.png` (10 fps; önce = Zeyd'in iPhone kaydı, sonra = yerel build).

**Kazanma → sonraki bölüm:**
- **Devir karesi:** koridordan dalışa geçerken yalnız tablo resmi bir kare hafifçe değişiyordu (resim bölgesi %99,9 dilim 12/255; çerçeve ve duvar sabit). Sebep: dalış resmi 12×12'lik, koridor 8×8'lik ızgarayla çiziyordu. Artık aynı ızgara; devir karesinde fark resimde de duvarda da 2/255 (JPEG gürültüsü).
- **Yaklaşma:** resim baştan tahtanın düz çerçevesine doğru kaymaya başlıyordu. Artık yaklaşmanın %70'ine kadar çerçeveye yapışık, dönüşüm son %30'da. Senkron (27 kare çifti): çerçeve noktaları en çok 0,30 px, resim noktaları ortanca 0,14 px, en çok 1,34 px.
- **Ölü bekleme yok:** koridor durduktan sonraki ilk karede yaklaşma başlıyor.

## Doğrulanmayan
- iPhone'da his: kayış hızı, parmakla kaydırma, eksen kilidi, titreşim.
- Kanal başına ≤ 2/255 kesin ölçümü: JPEG kaydın kendi gürültüsü 2–8/255.
