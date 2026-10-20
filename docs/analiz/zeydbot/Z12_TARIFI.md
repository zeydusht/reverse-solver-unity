# Z12 tarifi ("düşündüren zor")

Yazan: tasarımcı ajan (`seviye-tasarimci`), 2026-10-20. Kaynaklar: `Logs/zdesign/tahtalar.md` (Z12 çözüm sırası), `docs/analiz/zeydbot/bolum_yol.csv` (SmartPlayer yolu, adım adım), `hamleler.csv` (Zeyd'in ilk ve ikinci denemesi), `zorluk_analizi.md` "v2" bölümü. "Hesap" yazan sayıları ajan bu dosyalardan saydı.

## 1. Tek doğru hamle zinciri
- Z12'nin 54 adımının 20'sinde seçenek sayısı 1: adım 17, 25–28, 34–38 ve 44–53. Hamlelerin %37'si zorunlu tek hamle.
- Bu 20 zorunlu hamle 4 öbek halinde geliyor: 25–28 (4), 34–38 (5), 44–53 (10), 17 (1).
- Ortalama seçenek 2,5. Açılışta 5 seçenek var, hepsi doğru. 8 hamle boyunca 5 seçenek kalıyor, sonra düşüyor.
- Zorluk "hangisi doğru" sorusunda değil, "oynanabilir olan hangisi" sorusunda.

## 2. Tuzak sayısı
- Yolda tuzak 0. Karşılaştırma: Z13'te 24, Z07'de 22, Z29'da 17.
- Zeyd'in hata biçimi "yanlış hamle" değil, "dokunup çıkaramama" (touch/jam).

## 3. Fitil payı
- Dört bombanın hepsinin payı 1 (adım 0–22).
- Fitil = çıkış sırası + 1 (hesap):
  - 24 (fitil 9) 8. hamlede çıkıyor,
  - 25 (fitil 21) 20. hamlede,
  - 36 (fitil 23) 22. hamlede,
  - 37 (fitil 24) 23. hamlede.
- Çivi 18 (sayaç 18) 19. hamlede çıkıyor.
- Bombalar ilk 23 hamlede bitiyor; sonrasında baskıyı süre taşıyor.
- Pay 1, "bir boş hamlen var" demek: aramak için bir hamle harcanabilir, iki hamle harcanamaz.

## 4. "Aha" anının yeri
- Zeyd'in en uzun düşünmeleri (ilk deneme): 1. hamlede 24,5 s (okuma), 9. hamlede 21,4 s, 17. hamlede 29,6 s, 21. hamlede 15,9 s, 32. hamlede 10,9 s.
- "Aha" başta değil. Bölümün %17, %26 ve %45'i civarında üç büyük arama duraklaması var.
- Her duraklamanın ardından 5–8 hızlı hamle geliyor (< 1,2 s): "çözüldü, akış" hissi.

## 5. Engel dağılımı (M11) — hesap
- Kilitli dikiş: 97 iç kenarın 48'i (%49).
- Mühür: 30 şeridin 14'ü (%47).
- 1 çivi, 4 bomba, 2 zincir parça.

## 6. Yön karışımı (M12) — hesap
- L 20 (%37), D 16 (%30), R 9 (%17), U 9 (%17).

## 7. Süre
- 125 s / 54 hamle = hamle başına 2,31 s.
- Zeyd ilk denemede 23 parça çıkarabildi, ikinci denemede kazandı.

## 8. Z12 ile Z13 farkı
| | Z12 | Z13 |
|---|---|---|
| Yolda tuzaklı hamle | 0 | 24 |
| Hamle başına seçenek | 2,5 | 3,7 |
| Yanıltıcı parça / adım | 3,80 | 1,65 |
| Zeyd dokunma / çıkış | 0,46 | 0,21 |
| Takılma | 11 | 4 |

Z12'nin bulmacası "aramak", Z13'ünkü "yanlışa girmek".

## 9. Kural (hipotez, n = 1)
- Yanıltıcı parça ≥ 2,5 / adım.
- Tuzak ≤ 2.
- Seçenek ortalaması 2–3.
- Zorunlu tek hamle ≥ %30.
- Bombaların payı 1.
- Süre = yol × 2,3–2,5.
