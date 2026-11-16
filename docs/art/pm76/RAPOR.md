# PM_76 RAPOR: geçişler pürüzsüz (yay çizen kamera, akıcılık turu, değnek kararı)

(Taslak — ölçümler build'den sonra doldurulacak.)

## 1. Tablonun karşısına yay çizen kamera
- Codex (xhigh, `CODEX_PM76.md`) aynı odayı 4 açıdan katmanlı çizdi: a0 = bugünkü menü görünümü (değişmeden, ön plan ve duvar olarak ayrıldı), a1 25°, a2 12°, a3 tam karşı. İki takım üretildi (`acilar.png`, `acilar_b.png`); ikisi de tutarlı, **A takımı** kullanılıyor (tablo, çerçeve, sütunlar, flamalar, halı aynı; Codex'in kendi denetimi: a0 ve koridor birleşimleri tuval dışında 0 piksel fark).
- Oyunda: duvar katmanları (tuval alanı şeffaf), altında bölümün resmi, üstte ön plan. Kamera yay boyunca iki komşu açıyı aynı anda gösterir; ikisi de çerçevesinin köşeleri aynı ara dörtgende buluşacak şekilde bükülür, yeni açı eskisinin üstünde belirir: çerçeve hiçbir anda çift görünmez. Ön plan duvardan hızlı kayar (parallax). Yay boyunca kamera %10 içeri girer (kenar boşluğu olmasın), yaklaşma yayın son %28'inde başlar: tek kesintisiz hareket.
- Süreler: yay 0,85 sn (ease-in-out), yaklaşma 0,61 sn'de başlar ve 0,7 sn sürer, yalnız resim 0,45 sn, çizgiler ve tahta 0,6 sn, HUD'un son 0,2 sn'si → toplam **2,56 sn** (≤ 2,6). Bir dokunuş 3 kat hızlandırır, ikinci bitirir.

## 2. Koridor
## 3. Akıcılık turu
## 4. Değnek
