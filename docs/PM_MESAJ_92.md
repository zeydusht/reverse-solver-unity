# PM_MESAJ_92: Dalış hızı 2. tur: ortadaki duraksama gitsin, hız kesmeden duvara (ÖNİZLEME, yayın yok)

Kodu kendin yaz. `pm91-onizleme` dalında devam (yerel). **Push yok, yayın yok.** Bitiş mesajı kısa, en altta.

Zeyd (19 Kasım 22:16) PM_91 videolarına baktı: "Daha iyi ama ortada bir noktada yavaşlıyormuş gibi hissettirdi, bir struggle yaşanıyor, sonra akıyor. Sondaki kısım çok iyi. Çıktığı ivmeyle hız kesmeden direkt duvara gitsin."

**Kapsam:** yalnız menü → bölüm ve kazanma → bölüm yaklaşma zamanlaması. Resimde kalış (0,27 / 0,23 sn), geri çekilme, yol, çerçeve: aynı.

## Sorun (PM, `docs/art/pm91/hiz.png`)
- **Menü:** Ölçülen büyüme hızında ~0,53. saniyede bir **tepe**, hemen ardından **çukur** (0,88 → 0,82) var. Sonra 0,6–1,0 sn arası düz plato, sonra hafif artış.
  - Göz bunu "hızlandı, frenledi, sonra yine aktı" diye görüyor.
  - Tasarım eğrisinde de 0,5–1,0 sn arası düz plato var.
  - Tepe-çukur büyük ihtimalle yayın bitip düz yaklaşmanın başladığı birleşme noktası: yay dönüşü ile ileri hareketin toplamı orada bir anlık fazla.
- **Kazanma:** 0,2. saniyeden sonra artış yavaşlıyor (dirsek). Daha hafif ama aynı his.

## İstenen
- **Kalkıştan varışa kadar ekranda hissedilen hız sürekli artsın:** hiçbir tepe, çukur ya da plato yok. Kalkış aynı yumuşaklıkta başlar, ivme hiç sıfırlanmadan ya da eksiye dönmeden tabloya dalar.
- **Ölçülen** büyüme hızı (resmin ekranda büyüme hızı ve yay boyunca ekrandaki kayma hızı, ikisi birlikte) monoton artan olsun. Yalnız son ~%8'lik yolda en fazla %15 düşüş (çok kısa iniş). Bugünkü son kısım iyi, korunsun.
- **Yay → yaklaşma birleşimi:** iki hareketin toplam ekran hızı birleşme anında sıçramasın. Yayın dönüşü azalırken ileri hız aynı oranda artsın (birbirine yedirilmiş tek hareket).
- **Toplam süre** bugünkü kadar ya da biraz kısa (menü ≤ 1,6 sn yaklaşma, kazanma ≤ 0,55 sn).

## Önizleme (Zeyd telefonda bakacak)
- `docs/art/pm92/dalis_yan_yana.mp4`: PM_91 | PM_92.
- `docs/art/pm92/dalis_yeni.mp4` ve `docs/art/pm92/kazanma_yeni.mp4`.
- `docs/art/pm92/hiz.png`: tasarım + ölçülen. Ölçülende tepe/çukur olmadığı görünsün.
- 60 fps, 390×844, H.264 `-preset veryfast`, her biri ≤ 10 MB.

## Teslim
- EditMode yeşil (yerel). Push ve yayın yok.
- Son mesaj: süre tablosu (PM_91 → PM_92), video yolları.
