# Codex Task24 — Tablolar 11–40 yeniden çizim (Task23 bittikten sonra, gece)

Zeyd: "Son 30 tablo çok yapay duruyor, daha iyi çiz."

Kurallar:
- Yalnız `Assets/Art/Paintings_v2/` ve `docs/art/` altına yaz.
- Mevcut `Assets/Art/Paintings/` dosyalarına **dokunma**. Oyuna bağlama yok, silme yok, soru yok.
- Saat başı `docs/art/GECE_LOG.md`'ye not.

## Neden yapay duruyorlar (PM teşhisi)
1. **Her yer dolu:** minik çiçek, taş ve yaprak. Göz dinlenecek alan ve odak noktası yok. Tipik "AI elmas tablo" görüntüsü.
2. **Aynı kompozisyon tekrar ediyor:** merdiven + su + kemer + çiçek. Yan yana konunca kopyaya benziyorlar.
3. **Aşırı doygunluk, düz ışık.** Hikâye ve sahne hissi yok.
4. **Stil kartına aykırılar.** `docs/art/STYLE_CARD.md` tablolar için şunu diyor: "orta kontrast, büyük biçimler, az detay". Üst düzey 3D animasyon filmi hissi, geniş renk alanları, sade siluetler.
5. **Oynanış:** Tablo 6×7–8×8 parçaya bölünüyor. İnce detay parçaları ayırt etmeyi zorlaştırıyor (Task15 kuralı: büyük net şekiller, her bölgede ayırt edilebilir renk).

## Yeni yön
- **Animasyon filmi konsept sanatı gibi:** Boyalı ama sade. Büyük şekiller, yumuşak ama net ışık, sinematik derinlik (ön plan / orta / arka plan).
- **Tek bir odak noktası:** bir yapı, bir köprü, bir ağaç, bir fener… Gözün ilk gittiği yer belli olsun. Etrafında sakin alanlar kalsın: gökyüzü, su, düz duvar. Ama tek renk büyük boş alan da olmasın (parçalar ayırt edilsin).
- **Detay yoğunluğunu yarıya indir.** Çiçek ve yaprak öbekleri büyük kümeler halinde olsun, binlerce minik nokta değil.
- **Doygunluk biraz düşük, ışık yönü belirgin.** Sabah, gün batımı ya da gece. Her tablonun kendi ışık anı olsun.
- **Her tablo farklı bakış:** yakın plan, iç mekân, geniş manzara, kuşbakışı, pencereden… 10'luk her salonda en fazla 2 tabloda su olsun, merdiven en fazla 1 tabloda.
- **Küçük hikâye öğesi (öneri):** Her tabloda 1–2 küçük Jiggle bir şey yapıyor olsun: fener yakıyor, kızakla kayıyor, kitap okuyor… Onaylı Jiggles referansı (`Assets/Art/Characters/Jiggles-reference-v02.png`), ayrı uzuv kuralı. Odak noktasını çalmasın, küçük sürpriz gibi dursun. İnsan yüzü yok.
- **Salon temaları aynı kalır:**
  - 11–20: orman ve kır
  - 21–30: kış ve dağ
  - 31–40: gece ve büyü
  - 40 final şatosu

  Konular değişebilir, her salonda 10 **farklı** konu olsun. Liste `docs/art/T24_KONULAR.md`'de: başlık TR/EN + bir cümle sahne.

## Süreç
1. **Pilot:** önce 11, 21 ve 31'i çiz. `once-sonra-pilot.png`: eski ↔ yeni. Kendin kıyasla: yeni gerçekten daha az yapay ve daha net mi? Değilse yön değiştirip tekrar dene, en fazla 3 tur.
2. **Pilot iyiyse kalan 27 tabloyu aynı yönde çiz.** Zeyd sabah onaylayacak, ama gece boş beklemeyelim.
3. Her tabloyu bitirince parçalanma testi yap:
   - 6×7 ızgarayla kes.
   - Rastgele 6 parçaya bak. Her parçada ayırt edilebilir bir şekil ya da renk var mı?
   - Yoksa o tabloyu düzelt.

## Teslim
- `Assets/Art/Paintings_v2/Source/painting-XX.png` (1536×1536)
- `Assets/Art/Paintings_v2/painting-XX.jpg` (1024×1024, ≤ 300 KB, mevcut kurallar)
- `Assets/Art/Paintings_v2/manifest-v2.json`: başlıklar TR/EN, mevcut manifest biçimi
- `Assets/Art/Paintings_v2/once-sonra-11-20.png`, `-21-30.png`, `-31-40.png`: üst sıra eski, alt sıra yeni
- `Assets/Art/Paintings_v2/inceleme.html`: 30 çift yan yana, numaralı. Zeyd "hepsi" ya da "şunlar hariç" diyecek.
- Sabah özeti `GECE_LOG.md` sonuna.
