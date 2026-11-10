# Royal Match oyun ekranı — tersine mühendislik (PM ölçümü, 9 Kasım)

Kaynak: `docs/art/Reference/rm_oyun_ekrani.png` (1170×2532 px, iPhone). Aşağıda tüm ölçüler **390×844 pt**'ye çevrildi (px ÷ 3). Renkler pikselden ölçüldü.
**Kural (Zeyd):** boyut, yerleşim ve katman yapısı **birebir** alınır. Renk ailesi bizim (turkuaz), karakter bizim (Jigo), yazılar bizim. Kral, logo, ikonlar kopyalanmaz.

Neden güzel görünüyor (özet): her öğe **4–6 katmanlı** (iç dolgu → altın → turuncu → koyu lacivert → açık mavi çizgi) ve **her şeridin altında kalın bir gölge** var. Düz renk hiçbir yerde yok; her dolgu **üstte açık, altta koyu** (ya da tersi) gradyan. Bizde katman yok, gradyan yok, gölge yok, boyutlar küçük → "yapıştırılmış" görünüyor.

## 1. Üst şerit
| Öğe | Ölçü (pt) | Not |
|---|---|---|
| Şerit dolgusu | y 0 → 159 (ekranın **%19**'u) | Dikey gradyan: üst koyu `#134383` → alt açık `#1A5EA5` |
| İçerik başlangıcı | y 37 | Üstü durum çubuğu (çentik) |
| Alt bordür, yukarıdan aşağı | 159–160 açık sarı ışık çizgisi (1 pt) → 160–162 altın `#FFBB00` (2 pt) → 162–164 turuncu `#FF7101` (2 pt) | |
| **Gölge** | 164 → 168 (≈ 3–4 pt) | Koyu, %50 opak, aşağı doğru yumuşar. Menü hissini veren bu. |

### 1a. Sol panel (RM "Target") ve sağ panel (RM "Moves")
| Öğe | Ölçü (pt) |
|---|---|
| Sol panel | x 9 → 122 (genişlik 113 = ekranın **%29**'u), y 37 → 155 (yükseklik 118) |
| Sağ panel | x 268 → 381 (aynı boy) |
| Başlık kurdelesi | panelin üstü, y 37 → ~62. Mavi kurdele, kenarları altın+turuncu, yazı beyaz + kalın lacivert kontur |
| Gövde | y ~62 → ~150, krem: üst `#F9F2BF` → alt `#F4E9BB`; iç kenarında 1–2 pt koyu bej bevel `#A4997F` (çukur hissi) |

Panel katmanları **dıştan içe**: açık mavi çizgi `#0075C5` (1 pt) → koyu lacivert `#003D87` (1,5 pt) → turuncu `#F75E00` (2 pt) → açık sarı ışık (1 pt) → altın `#FBD000` (2 pt) → mavi çerçeve bandı (≈4–5 pt, içe doğru koyulaşan yatay gradyan `#0059AE`→`#004A8A`) → bej bevel → krem gövde. Panelin altında da gölge.

Sağ paneldeki sayı: gövde yüksekliğinin **~%55**'i, beyaz dolgu + kalın lacivert kontur (≈3 pt) + alta 2 pt gölge. Uzaktan okunuyor.

### 1b. Orta portre (RM'de kral)
| Öğe | Ölçü (pt) |
|---|---|
| Pencere | x 131 → 258 (genişlik 127 = ekranın **%33**'ü), y 51 → 155. Üstü kemer (yuvarlak), altı panellerle aynı hizada düz |
| Katmanlar dıştan içe | açık mavi `#0075C5` (1 pt) → koyu lacivert `#003D87` (2 pt) → turuncu `#FF7300` (1,5 pt) → açık sarı ışık `#FFCE57` (1 pt) → altın `#FFBD00` (2 pt) → iç zemin |
| İç zemin | dikey gradyan: üst gece moru `#1D142C` → alt camgöbeği `#42A1AD` (karakter öne çıkar) |
| Karakter | **büst** (baş + omuz), pencerenin ~%85'ini doldurur, **dışarı taşmaz**. Yüz büyük, ifade 96 pt'de net |

## 2. Alt şerit (booster)
| Öğe | Ölçü (pt) |
|---|---|
| Şerit | y 771 → 844 (**%8,6**). Üst kenar: 1 pt koyu `#003984` + 1 pt açık mavi ışık `#0075C5`, dolgu `#0054AB`→`#005BAC` |
| Tuşlar | **5 yuva** (4 booster + Ayarlar). Çap ~62 pt (ekran genişliğinin **%16**'sı). Merkez x ≈ 42 / 118 / 195 / 272 / 348, merkez y ≈ 786 |
| Taşma | tuşların üstü y ≈ 755 → şeritten **~16 pt (çapın %25'i) yukarı taşar** |
| Yuva | her tuşun altında koyu lacivert halka/çukur `#003D85` → tuş bara gömülü durur |

Booster tuşu katmanları **dıştan içe**: turuncu halka `#FF6A00` → altın halka `#FFBF00` → açık yeşil ışık kenarı `#A3D93A` → **yeşil gövde** üst açık `#81D62E` → alt koyu `#005F00` (dikey gradyan, alt kenarda koyu gölge) → içte ikinci daire (iç disk, bir ton açık) → ikon. Altta koyu gölge.
Adet rozeti: kırmızı küre, üst `#EB4314` → alt `#A01305`, ince açık kontur, sağ alt köşede halkanın üstüne biner, rakam beyaz + koyu kontur.
Ayarlar tuşu: aynı halkalar, gövde mavi.
**Pause üstten kalkar**, Ayarlar tuşu oyunu durdurur (açılınca menü: Devam / Ses / Çık).

## 3. Tahta
Üst şeridin gölgesinden alt şeridin taşan tuşlarına kadar. Tahta genişliği ekranın ~%98'i. Bizde kare tahta 390−2×8 = 374 pt genişliğe kadar sığar (6×8 tahta 62 pt hücre, 499 pt boy → kullanılabilir ~587 pt). Tahta **küçülmez**.

## 4. Bizim çevirimiz
- RM mavisi yerine **dolgun turkuaz-petrol gradyanı** (Codex 3 aday versin, Zeyd seçsin): A) üst `#0A3A4A` → alt `#138A9C`, B) üst `#0B3550` → alt `#1786A8` (daha mavimsi), C) üst `#0E4A47` → alt `#1A9A8C` (daha yeşilimsi). Panel çerçeve bandı aynı ailenin bir ton koyusu.
- Altın/turuncu/krem: `PALETTE.md` altın skalası (`#FBBA0E`/`#F38A03`/`#CE5D01`, `#FFF1C2`, krem `#F8EBD0`).
- Sol panel kurdelesi "Parça" / "Pieces": içinde parça ikonu + kalan sayı. Seviye no küçük, kurdelenin içinde ya da sol üst köşede rozet.
- Sağ panel kurdelesi "Süre" / "Time": büyük süre (0:42). ≤10 sn'de rakam mercan `#E8574A`, hafif nabız.
- Orta: Jigo büstü (fırça ucu + yüksük), gece→camgöbeği iç zemin.
- Alt: 4 booster + Ayarlar, yeşil katmanlı tuşlar (ana eylem yeşili `PALETTE.md`), kırmızı rozet, mavi çekiç.
