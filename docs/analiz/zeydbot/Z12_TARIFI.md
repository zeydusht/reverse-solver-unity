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

---

# Tarif v2 (2026-10-21, PM_MESAJ_3, oturum 4'ten sonra)

Kaynak: oturum 4 sonucu (RAPOR.md "Oturum 4 sonucu") ve Zeyd'in sözlü geri bildirimi (PM_MESAJ_3). Fabrika üçünü de ölçüyor (`ZeydDesign.Check` raporundaki M13–M15 satırları; `taslak_ozellik.csv` sütunları `m13_*`, `m14_*`, `m15_kalan_pay`).

## M13 — Anahtar parça anı (birincil keyif metriği)
- **Tanım:** SmartPlayer yolunun bir adımında serbest artı serbest görünen (yanıltıcı) parça sayısı ≥ 4 ve çözümü ilerleten hamle sayısı = 1. Adım adım tabloda "A" ile işaretli.
- Görünen tek bir serbest parça anahtar an değildir; aranacak bir şey yoktur. Ölçüm, yanıltıcı parçaları da "görünür" saydığı için bu ayrımı yapar.
- **Hedef:** Yolun ikinci yarısında ≥ 3 anahtar an ve aralarında ≤ 3 rahat hamle olan art arda en az 3 anahtar an. Raporda: sayı, ikinci yarıdaki sayı, en uzun dizi.

## M14 — Bomba çözümü çeşitliliği
Her bomba için, bombadan önceki 8 çıkış ölçülür:
- Aynı şerit ve aynı yönde art arda en çok çıkış: **≤ 2**. Zeyd'in tarif ettiği "tak tak tak süpürme" bunu bozar.
- Bu 8 çıkışta kullanılan yön sayısı: **≥ 2**.
- Bomba ya da zincirli parça, önceki bomba/zincir ile aynı şeritten ve aynı yöne art arda çıkamaz.
- **Desen imzası:** bomba başına "baskın süpürme yönü + süpürme uzunluğu > bombanın çıkış yönü", ör. `R6>U`. Setteki ardışık 5 zor bölümde imzalar farklı olmalı.

## M15 — Süre gerilimi
- **Hedef:** Zeyd'in kazandığı Z12 tipi bölümlerde bitişte kalan süre, limitin %10–20'si olmalı. Oturum 4'te Z35'te 14/125 = %11, Z31'de %19, Z33'te %23 kaldı.
- **Ölçüm:** Zeyd'in Z12 tipi hızı = Z31, Z33 ve Z35 kazançları, (101 + 96 + 111) s / (3 × 54) hamle = 1,90 s/hamle (`ham_plays.csv`). Kalan pay = 1 − 1,90 × yol / süre.
- Gerilimi süre taşır, bomba "düşünme" baskısı yaratır.

## Kalibrasyon (14 zor bölüm, `python Tools/zeydbot_m13_kalibrasyon.py`)

| Bölüm | Puan | M13 anahtar (2. yarı, dizi) | M14 süpürme / yön / düz şerit | M14 imza | M15 kalan pay | Zeyd: anahtar an sayısı | düşünme medyan anahtar / diğer (ms) | dokunma/hamle anahtar / diğer |
|---|---|---|---|---|---|---|---|---|
| Z07 | 8 | 16 (0, 14) | 3 / 2 / yok | U3>L L3>L L3>L U2>L | %7 | 0 | 0 / 549 | 0.00 / 0.18 |
| Z08 | 7 | 9 (6, 6) | 3 / 2 / var | D2>D D2>D D3>D D2>D | %21 | 12 | 968 / 598 | 0.08 / 0.12 |
| Z12 | 9 | 15 (12, 5) | 3 / 2 / var | L2>L L3>U L2>L L2>L | %18 | 6 | 2368 / 966 | 0.50 / 0.24 |
| Z13 | 9 | 11 (4, 11) | 4 / 3 / var | R2>R R2>D R3>D D4>D | %2 | 0 | 0 / 650 | 0.00 / 0.21 |
| Z18 | 9 | 0 (0, 0) | 4 / 2 / yok | L4>L D2>R R2>R D3>L | %24 | 0 | 0 / 648 | 0.00 / 0.11 |
| Z23 | 5 | 0 (0, 0) | 3 / 2 / var | L3>U L3>U U2>U U3>U | %32 | 0 | 0 / 684 | 0.00 / 0.09 |
| Z25 | 4 | 0 (0, 0) | 3 / 2 / var | U3>U U3>R U3>R R3>R | %29 | 0 | 0 / 599 | 0.00 / 0.06 |
| Z26 | 6 | 0 (0, 0) | 4 / 1 / var | U4>U U4>U U3>U U3>U | %40 | 0 | 0 / 683 | 0.00 / 0.08 |
| Z29 | 6 | 4 (4, 4) | 4 / 2 / yok | R4>U R3>D D3>R D3>R | %11 | 2 | 783 / 565 | 0.00 / 0.12 |
| Z31 | 9 | 14 (9, 9) | 4 / 2 / var | R2>R R4>D R3>R R2>R | %18 | 18 | 701 / 668 | 0.06 / 0.23 |
| Z32 | 6 | 14 (10, 12) | 3 / 2 / var | L3>U L3>U U2>U U3>U | %18 | 5 | 918 / 500 | 0.00 / 0.06 |
| Z33 | 8 | 18 (9, 9) | 4 / 2 / var | U2>U L4>D L3>L L2>L | %18 | 11 | 766 / 532 | 0.09 / 0.14 |
| Z34 | 5 | 14 (10, 12) | 3 / 2 / var | R3>U R3>U U2>U U3>U | %13 | 1 | 518 / 600 | 0.00 / 0.31 |
| Z35 | 9 | 15 (12, 5) | 3 / 2 / var | D2>D R3>U R3>R R3>R | %18 | 10 | 751 / 484 | 0.40 / 0.13 |

Puanla Spearman (14 zor bölüm), M13 anahtar: +0.44
Puanla Spearman (14 zor bölüm), M13 ikinci yarı: +0.33
Puanla Spearman (14 zor bölüm), yanıltıcı/adım: +0.39
Puanla Spearman (14 zor bölüm), M14 süpürme: +0.35
Puanla Spearman (14 zor bölüm), Zeyd anahtar an sayısı: +0.34

Anahtar anda düşünme medyanı diğer hamlelerden uzun: 7/8 bölüm


**Okuma:**
- Zeyd'in kendi oyununda anahtar anlarda düşünme süresi 8 bölümün 7'sinde daha uzun. M13 "aranarak bulunan hamle" anını yakalıyor.
- Hiçbir zor bölüm M14 süpürme ≤ 2 şartını tutmuyor (3–4). Z32 ve Z34'ün imzası aynı (aynalı). Z31, Z33 ve Z35 de R/L süpürmesini tekrarlıyor. Zeyd'in "bir kere bulunca hep aynı" sözü ölçümle uyuşuyor.
- Puanla ilişki (n = 14): M13 +0,44 (en yüksek), yanıltıcı/adım +0,39. Z18 (9 puan) M13 = 0, yani "sıkı kazı" tipi ayrı bir yol.
