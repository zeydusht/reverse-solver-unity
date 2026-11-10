# Codex Task16: 40 bölüm için 40 tablo resmi

Zeyd, resimli yapboz testini (`painting-test-01`, Akdeniz) çok beğendi. Şimdi 40 bölümün her biri kendi tablosunu kurtaracak. Bu resimler aynı zamanda Kont'un şatosundaki tablo koleksiyonu olacak.

Kurallar:
- Yalnız `Assets/Art/Paintings/` ve `docs/art/` klasörlerine yaz. C# dosyalarına dokunma.
- Stil: `painting-test-01` ile aynı. Parlak, sıcak, el boyaması hissi veren, Royal Match ve Disney arası bir çizim; menü koridorundaki tablolarla aynı dünya.
- Telif: Bilinen bir tabloyu, yeri ya da karakteri kopyalama; sahneler özgün olsun. Yazı, logo ve insan yüzü olmasın.

## Oynanabilirlik kuralları (önemli)

Resim 6×6 ya da 7×7 parçaya bölünecek ve üstüne engeller (bomba, çivi, zincir) gelecek.

- Her bölgede ayırt edilebilir detay ve renk olsun. Büyük tek renk alanlar (düz gökyüzü, düz deniz) olmasın.
- Çok koyu ve çok kırmızı büyük alanlar olmasın. Altın ve krem renkli engeller ile kırmızı-beyaz duvar resmin üstünde okunmalı.
- Kompozisyon merkezde boğulmasın; detay tüm kareye dağılsın.

## Temalar (4 oda × 10 tablo)

| Bölümler | Oda | Örnek sahneler |
|---|---|---|
| 1–10 | Akdeniz salonu | sahil köyü, liman, zeytinlik, fener, çiçekli sokak |
| 11–20 | Orman ve kır salonu | şelale, değirmen, mantar ormanı, lavanta tarlası, göl evi |
| 21–30 | Kış ve dağ salonu | karlı köy, dağ kulübesi, buz pateni gölü, kuzey ışıkları, tren köprüsü |
| 31–40 | Gece ve büyü salonu | fener festivali, yıldızlı kale, sihirli kütüphane, balon şehri, Kont'un bahçesi |

Bölüm 40 en görkemli tablo olsun: Kont'un şatosunun kendisi, ama şatoda Kont'un yüzü görünmesin.

## Teslim (partiler halinde)

- **Parti 1:** bölüm 1–10 için 10 resim. Dur ve Zeyd'in onayını bekle. Sonraki partiler 11–20, 21–30, 31–40.
- Dosya adları: `painting-01.png` … `painting-40.png`.
  - 1536×1536 kaynak: `Source/`
  - 1024×1024 oyun sürümü: kalite 85 JPG, en fazla ~300 KB
- Her partide şunlar olsun:
  - `preview-batch-N.png`: 10 resim küçük ızgarada.
  - `preview-board-N.png`: bir resim 6×6 parçaya bölünmüş, üstünde bir bomba ve bir çivi ile.
- HANDOFF'a satır ekle: dosya / bölüm / tema / boyut.

Her parti bitince "Parti N bitti" yaz.
