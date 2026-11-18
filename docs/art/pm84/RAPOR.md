# PM_84 RAPOR: ipucu (bölüm 1–3), son parça mesajı, dalışta çizgiden tahtaya geçiş

## 1. İpucu
**1. bölüm, serbest kısım:** "Süpersin! Kalan Jiggle'ları sen kurtar!" yazısından 1,6 sn sonra (serbest kısmın ilk hamlesi, bölümün 3. hamlesi) ipucu kendiliğinden çıkar:
- bir parça altın parlar ve nabız gibi atar;
- gideceği yönde krem ok;
- karartma yok;
- Jigo üst kutuda "Bu parçayı oynatabilirsin!" der.

Oyuncu başka bir doğru parçayı çıkarırsa da kabul edilir; ipucu kalkar, kızma yok.

**Hareketsizlik ipucu (bölüm 1, 2, 3):**
- 5 sn hareketsizlikten sonra çıkar.
- Parlama nabız gibi: ölçek 1,0 ↔ 1,06, 0,8 sn.
- Ok gidiş yönünde 2 kez ileri-geri sallanır, kısa durur, tekrarlar.
- Bölümdeki ilk gösterimde Jigo kutuda 2,6 sn "Bu parçayı oynatabilirsin!" der.
- Dokununca hemen kalkar; sonraki 5 sn hareketsizlikte yine çıkar.

**Seçilen parça:**
- Bomba varsa önce bombayı çözen hamle seçilir (SmartPlayer'ın buradan ilk hamlesi).
- Yoksa bölümün kendi çözümündeki, şu an çıkabilen ilk parça.
- O da yoksa çıkabilen herhangi bir parça.

Test: 100 bölümün her biri yalnız ipucu izlenerek kazanılıyor.

**4. bölüm ve sonrası:** ipucu yok; zor ve çok zor bölümlerde de yok. Sınır `live/config.json` → `ipucu_son_bolum = 3`. PM_82'deki "2. bölümde 6 sn" kuralı bununla değişti. Booster dersleri ve engel kartları aynen kalıyor.

## 2. HATA: "Bütün Jiggle'lar kurtuldu!" erken çıkıyordu
**Sebep:** Mesaj, tahtada **1 parça kaldığında** tetikleniyordu (`PresentCount <= 1`). PM_82'de "son parçadan önceki satır" olarak yazılmıştı, ama metin "hepsi kurtuldu" diyordu. PlayMode testi de bu yanlış anı doğruluyordu (1 parça kalınca mesajı bekliyordu). Sayaçta eksik yok: 9 parça sayılıyor.

**Düzeltme:**
- Mesaj son parça tahtadan çıkıp uçuşu bitince gelir.
- 1,5 sn ekranda kalır, sonra kazanma ekranı ("Harika!") açılır.
- Son parçadan önce kutu kapanır.

**Diğer bölümler:** Kazanma ekranı her bölümde zaten son parçanın uçuşu bitmeden açılmıyor (`FlyingUntil` bekleniyor). Erken tetik yalnız 1. bölümün rehber mesajındaydı.

**PlayMode testleri:**
- 1. bölüm 9/9: 1 parça kalınca mesaj yok; son parçadan sonra mesaj; kazanma ekranı mesajı bekliyor.
- 2. bölüm: 1 parça kalınca kazanma yok, ekran yok; son parçadan sonra kazanma ekranı.
- 4. bölüm: 7 sn boyunca ipucu yok.

## 3. Dalış: çizgiler = tahtanın kendi parça kenarları
**Yöntem:**
- Gerçek tahta, dalışta resim tam ekran olduğunda resmin tam altında, aynı yer ve ölçekte kuruluyor (eskiden de öyleydi).
- Yeni olan: çizgileri artık ayrı bir krem çizgi katmanı çizmiyor. Her parçanın bir kopyası, **aynı ağ, aynı shader ve aynı yer** ile yalnız kendi kenarını çiziyor: lacivert kenar, krem bant ve hafif gölge. Kenar kalemle merkezden dışa, tırnakların etrafından dolaşarak çiziliyor.
- Geri çekilirken resim, altındaki aynı kenarlı gerçek parçalara bırakıyor. Kopyalar, gerçek kenarlar göründükten sonra kalkıyor.
- Kamera tek nesneyi (tahtayı) ölçekleyerek oyun konumuna getiriyor. Ayrı katmanın kalkıp tahtanın gelmesi yok.
- Kazanma → sonraki bölüm dalışı da aynı kodu kullanıyor.

**Ölçüm:**
- **Çizgi ile parça kenarı arasındaki fark: 0 px.** Çizgi ve kenar aynı ağ ve shader'dan çiziliyor.
- **Boş kare:** yeni kayıtta 0. Kayıt 385 kare; geri çekilme kare kare incelendi. Çizgiler çizilmeye başladığı andan oyuna kadar her karede ya çizgi ya tahtanın kenarı var.
- Eski kayıtta krem çizgi solarken altındaki lacivert kenarla birbirini götürüyordu; Zeyd'in gördüğü boşluk bu.
- Otomatik görüntü ölçümü denendi. Resmin kendi ayrıntısı ve kameranın kare içinde kayması yüzünden güvenilir sonuç vermedi, rapora konmadı.
- **20 ms'yi aşan kare** (oyunun kendi sayacı, PC, gerçek GPU):

  | Geçiş | Kare | 20 ms'yi aşan | Sebep |
  |---|---|---|---|
  | Menü → bölüm dalışı | 229 | 1 (31 ms) | bölüm kurulumu, ekran dururken |
  | Kazanma → sonraki dalış | 108 | 1 (44 ms) | aynı |

**GIF'ler (önce | sonra):**
- `docs/art/pm84/dalis_once_sonra.gif`
- `docs/art/pm84/dalis_once_sonra_yarim_hiz.gif`
- `docs/art/pm84/ipucu_bolum2.gif`

## Doğrulanmayan
- iPhone'da his: ipucu parlamasının ve okun dikkat çekiciliği.
- Kenarın büyütülmüş hâli: dalışta kenar resim büyükken kalın görünüyor, oyundaki kenarın büyütülmüşü.
