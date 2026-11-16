# PM_75 RAPOR: Zeyd'in p23 testi, düzeltmeler (16 Kasım)

(Taslak: build ve çekimlerden sonra sayılar ve GIF'ler eklenecek.)

## 1. Değnek
**Neden "parça açamıyor" diyordu (ölçüm):** 40 bölümün 445 ara durumunda (bölüm başı, %10 … %90 parça çıkmış) değneğin tırnak değişikliği ile **en az bir parçayı serbest bırakabildiği** durumlar:

| Çıkan parça | Durum | PM_74 (örnekleme) | PM_75 (hedefli arama) | Hiç mümkün |
|---|---|---|---|---|
| %0 (bölüm başı) | 80 | 0 | 0 | 0 |
| %10 | 80 | 12 | 12 | 12 |
| %20 | 71 | 18 | 19 | 19 |
| %30 | 67 | 29 | 29 | 29 |
| %45 | 59 | 26 | 26 | 26 |
| %60 | 34 | 14 | 14 | 14 |
| %75 | 29 | 8 | 8 | 8 |
| %90 | 25 | 4 | 4 | 4 |
| **Toplam** | **445** | **111 (%24,9)** | **112 (%25,2)** | **112 (%25,2)** |

- Sorun arama bütçesi değil, **değneğin kuralı**: yan yana duran iki parça arasındaki tırnak hangi parçaya ait olursa olsun ikisi birbirine takılır (tırnak ya bir yandan ya öbür yandan çıkar). Değnek yalnız tırnağın **sahibini** değiştirir, tırnağı kaldırmaz. Tahta doluyken (bölüm başı) hiçbir değişiklik parça açamaz; sonlara doğru da kalan parçaların çoğu zaten serbesttir.
- Hedefli arama (yeni): kilitli her parçanın çıkış yolundaki tırnakları doğrudan çevirir; örneklemenin kaçırdığı durumları bulur (mümkün olan 112 durumun 112'sinde değnek seçenek sunuyor, "mümkün ama sunulmadı" 0).
- **Gerçekten imkânsızsa** (tahta dolu, ya da her parça zaten çıkabiliyor, ya da 1 parça kaldı) değnek düğmesi **önceden soluk** görünür; basınca uyarı yerine kısa açıklama balonu: "Değnek boşluk açılınca işe yarar. Önce birkaç parça çıkar." / "Her parça zaten çıkabiliyor."
- PlayMode testi (9, 15, 23 + rastgele 10 bölüm, test linki): bölüm başında 13/13'te değnek soluk ve basınca açıklama çıkıyor, seçim yok, girdi serbest. Çözüm yolunda parça çıktıkça 12/13'te değnek yanıyor ve bastığında bir bölge seçip en az 1 parçayı serbest bırakıyor. Bölüm 9'da çözüm yolu boyunca hiç gerekmedi (her parça ya serbest ya sıkışık).
- **Ürün kararı (PM/Zeyd):** "≥ 2 parça varken her zaman bir şey yapsın" bugünkü kuralla mümkün değil. İstenirse değnek son çare olarak bir parçanın kilitli tırnaklarını **düzleştirebilir** (makasa yaklaşır). Bu sürümde yok; karar bekliyor.

## 2. Takılma
- Yeni `Contact.Distance` (Core): parça takıldığında o yönde **gerçekten değdiği yere** kadar kayar. Kural (`TravelRule`, web portu) değişmedi; yalnız çizilen şekil ölçülür: tırnak = kenarın .08 dışında yarıçapı .18 daire.
  - Yandaki komşunun tırnağı yolu kesiyorsa: (boş hücre sayısı) + 0,32 hücre (videodaki durum: 0,32 hücre aşağı iner, çıkıntıya değer).
  - Önde parça varsa: boş hücre sayısı, uç uca gelen tırnaklar kadar eksik (0,26 / 0,52).
  - Yanındaki parçayla kilitliyse: 0 → bugünkü küçük sarsıntı.
- Temas anında %6 sıkışma, "tok" sesi (`pm75-tok`, `Tools/make_pm75_tok.py`), hafif titreşim, 80 ms bekleme, yaylı geri dönüş. Kırmızı yalnız temas anında kısa bir parıltı (önceden sürekli kırmızıydı).
- Parmakla yavaş çekişte de parça parmağı temas noktasına kadar izler.
- EditMode: 3 elle kurulmuş durum (video, uç uca/yuvalanan tırnak, kilitli) geçti.

## 3. Geçişler
- Koridor geri geldi: kazanma → "Bölüm N+1": koridorda bir sonraki tabloya kayış; kazanma → Menü: koridor + menü.
- Dalış yalnız tablonun önünde: menüden "Bölüm N" ve koridorun sonunda.
- Yeni dalış (≈ 2,5 sn): karşıya geçiş (0,5) → yaklaşma (0,6) → yalnız resim (0,5) → uzaklaşırken yapboz çizgileri parça parça çizilir (0,6) → HUD ve çubuk en son (0,3). Bir dokunuş 3 kat hızlandırır, ikinci dokunuş bitirir. `&gecis=yok`: dalış yok.

## 4. Booster'lar
- Makas (~2,1 sn): gelir, tırnağın üstüne yerleşir, ağız yavaşça açılır, tırnağı arasına alır, ~0,5 sn'de görünür şekilde keser, tırnak kopar, makas yuvasına uçup oturur.
- Çekiç (~1,9 sn): parçanın üstüne başı aşağı gelir, kalkar, balyoz gibi iner; çatlak vurduğu noktadan yayılır, kırılma + kıvılcım + sarsıntı + titreşim; yukarı kalkıp yuvasına döner, dönme yok.
- Değnek (~2,0 sn): aynı döngü daha yavaş, sonunda yuvasına döner.
- Saat: Süre kutusunun altında ince süre çubuğu (yeşil → sarı → kırmızı); saat kullanılınca buz mavisi olur, üzerinde ışık gezer ve ucunda kar tanesi döner (duraklama).
- Eski PM_74 fikirleri: `&makasfx=d`, `&cekicfx=d`, `&degnekfx=d` (B ve C de duruyor).

## 5. Ana menü üstü
Codex (xhigh, `CODEX_PM75.md`) üç takım çizdi: isim plakası (solda yuvarlak profil halkası, krem gövde, opak), altın sayacı, yuvarlak mavi + beyaz dişli ayarlar düğmesi (basılı hâliyle). Üst alanda yalnız 3 öğe: isim, altın, ayarlar (Jiggle sayacı kalktı, altın geldi). Önizlemeler: `codex/menu_ust_A.png`, `_B.png`, `_C.png`.

| Seçenek | Görünüş | Puan (opaklık / okunurluk / onaylı dile uyum / sadelik, 10 üzerinden) |
|---|---|---|
| **B (varsayılan)** | kalın lacivert çerçeve + altın iç çizgi (kazanma ekranındaki rozet gibi) | 9 |
| A (`&ust=a`) | sade krem, ince lacivert kenar, altın halka | 8 |
| C (`&ust=c`) | altın kenar + lacivert alt şerit | 7,5 (altın, tablonun altın çerçevesiyle yarışıyor) |

Profil resmi halkanın içinde tam ortalı (halka ölçüldü: 192 px'lik plakada 14–177 px). Plaka 9 dilimli: genişlik değişse de halka yuvarlak kalıyor.

## 6. Düğmeler
- Ana menü: yazı yalnız "Bölüm N", nefes animasyonu yok, basınca %4 küçülür.
- Kazanma: "Bölüm N+1" menüdeki düğmeyle aynı (yeşil); "Menü" aynı ailenin küçük lacivert eşi (`Tools/make_pm75_buttons.py`). Kaybetme ve devam ekranları da aynı aile.
