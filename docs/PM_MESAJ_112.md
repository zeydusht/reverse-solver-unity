# PM_MESAJ_112: Resimde durma yerine "bungee" dönüşü (ÖNİZLEME, yayın yok)

Kodu kendin yaz. `pm108-onizleme` dalında devam (yerel). **Push yok, yayın yok.** Bitiş mesajı kısa, en altta.

Zeyd (25 Kasım 20:33): "Vol 2 iyi. Ama orada bekleme gözüme garip geliyor: hareket eden ekran bir anlık duruyor. Bungee jumping gibi düşünelim: çok azıcık daha içeri girip, geri çıkarak çıksın."

- **Temel:** gidiş B + vol 2 zamanlaması (toplam ≈ 1,60 sn) varsayılan.
- **Değişen tek şey:** varıştaki duruş (0,07 sn) ve değme (%2,5).

## İstenen: durma yok, bungee dönüşü
- Kamera tabloya varınca **durmaz**. Aynı yönde biraz daha içeri girmeye devam eder, yavaşlar, **en dipte hız tek bir anda sıfırdan geçer**, sonra geri çekilmeye döner.
- **Ekranda hiç donmuş kare yok.** Hız grafiği dipte sivri bir "V" değil, yumuşak bir dönüş: ivme sürekli, hız işaret değiştiriyor.
- **Dönüş süresi** (varış → dipten çıkıp varış noktasına geri gelme): ~0,10–0,14 sn. Toplam süre ≈ vol 2 ile aynı kalsın (±0,05 sn).
- **İçeri girme miktarı** (ölçek, eşit; resim ekranı dolduruyor, kenar boşluğu 0): 3 varyant.

| Varyant | Ek içeri girme | Bayrak |
|---|---|---|
| **bungee 1** | %2 | `&bungee=2` |
| **bungee 2** | %4 | `&bungee=4` |
| **bungee 3** | %6 | `&bungee=6` |

- Dipte resim tam ekran ve keskin kalsın. Fazladan büyümede bulanıklık / piksellenme olursa doku çözünürlüğünü kontrol et.
- **Çizgiler:** varış anında başlar (vol 2 kuralı), bungee boyunca çizilmeye devam eder, geri çekilmenin ~%80'inde biter. Çizgiden tahtaya devir kuralı aynı.
- **Ses:** dalış whoosh'u dipte söner, geri çekilme whoosh'u dönüşle başlar. Arada sessiz boşluk ya da üst üste binme yok.

## Önizleme
- `docs/art/pm112/bungee_yan_yana.mp4`: vol 2 | bungee 1 | bungee 2 | bungee 3. Yan yana, aynı anda, sesli, 60 fps.
- **Yarım hız** versiyonu: `bungee_yan_yana_yarim.mp4` (dönüş anı rahat görülsün).
- Tek tek: `bungee1.mp4`, `bungee2.mp4`, `bungee3.mp4`.
- `hiz.png`: dönüş bölgesi yakın plan. İşaretli hız gösterilsin: içeri +, dışarı −; sıfırdan tek geçiş, düz bölge yok.

## Teslim
- EditMode yeşil (yerel). Push ve yayın yok.
- Son mesaj: süre tablosu, video yolları.
