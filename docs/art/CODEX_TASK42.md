# CODEX_TASK42 — TASK41 düzeltmeleri: "wow" efektler, Jigo'nun saat animasyonu, makas akışı

Zeyd TASK41'i gördü. Maketler, tutorial, aferin rozetleri, Sonraki, Jiggle sayacı ve kilit uyarısı **onaylı**. Üç şey değişiyor.

## 1. Efektler: soluk değil, WOW
Parça çıkış patlaması, engel dağılması, değnek ışık patlaması, makas kırpıntısı ve yıldız tozu izi şu an grimsi toz gibi; telefonda görünmüyor. Oyuncu "haz veren bir şey eksik" dedi; bu efektler o hissi verecek.
- En az **2 kat büyük**, **doygun renkli**, parlak: merkezde beyaz-sarı parlama halesi, dışa savrulan yıldızlar, renkli konfeti, kısa ışık halkası (shockwave). Renkli sürüm parçanın rengini alır.
- Zamanlama "pop" hissi versin: ilk 2 kare hızlı büyüme, sonra yavaş dağılma. 8–10 kare.
- Engel kırılması (kilit/zincir): metal parçaları + kıvılcım + küçük toz bulutu, kalın ve okunur.
- Çekiç darbesi: büyük yıldız + şok halkası + ekran sarsıntısına uygun bir "impact frame".
- Önizlemede efektleri **gerçek 390×844 oyun ekranında**, gerçek boyutlarıyla göster (paftada küçük kutu değil).

## 2. Saat booster'ı: Jigo'nun kendisi yapacak (yeniden)
Turkuaz "hortum kol" iptal. Jigo madalyon penceresinde duruyor: bir elinde fırça, diğer elinde **düğme kalkan**. Akış:
1. Oyuncu saate basar.
2. Jigo **düğmeyi bırakır** (arkasına/aşağı koyar).
3. Aşağıdan **saati alır** (saat alt şeritten Jigo'ya uçar ya da Jigo uzanıp alır).
4. **Kendi kolunu** (turkuaz, eldivensiz yuvarlak el, karakterle aynı çizim) madalyonun **dışına uzatır**, sağa, "Süre" paneline doğru. Saati Süre panelinin önüne/yanına tutar → süre durur.
5. Süre panelinin altında **boşalan bar** (krem/mavi + altın kenar). Bar bitene kadar Jigo saati tutmaya devam eder (tutma döngüsü: hafif zorlanma, ter, sallanma).
6. Bar bitince Jigo kolunu içeri çeker, saati bırakır, **düğmeyi geri alır**.
- Kol bir **parça parça katman** olsun (omuz, üst kol, alt kol, el + saat) ki Claude Code kodla uzatıp hareket ettirebilsin; ayrıca anahtar kareler (düğme bırak, saat al, uzan 3 kare, tut 2 kare, geri çek 3 kare, düğme al).
- Jigo madalyonun içinde biraz daha hareketli: uzanırken gövdesi de sağa eğilir, sanki oyunun içindeymiş gibi pencereden taşar.

## 3. Makas akışı
Makas **bütün tahtayı değil, iki parça arasındaki bağı** (bağlantıyı/zinciri) keser. Kurala Claude Code karar verecek, görseller buna göre:
- Makas ekranın kenarından gelir; **açılıp kapanarak** ("çıt çıt") yürüyerek seçilen iki parçanın arasındaki çizgiye gider (ilerleme döngüsü: açık-kapalı 4 kare).
- Çizgi boyunca keser: kesik çizgi soldan sağa kopar, kırpıntılar saçılır.
- Sonra aşağı doğru kayarak ekrandan çıkar ve kaybolur.

## Teslim
- `docs/art/task42/onizleme.png`: 390×844 ekranda 3 sahne (efektler, saat akışı 6 adım yan yana, makas akışı 4 adım) + yakın planlar.
- Dosyalar `Assets/Art/_yeni/T42/`, `docs/art/task42/LISTE.md` (kare, fps, katman listesi).
- Zeyd referans video (Royal Match çekiç vb.) gönderirse `docs/art/Reference/video/` altında olacak; varsa zamanlamayı ona göre ayarla.
- 3 satır özet, dur.
