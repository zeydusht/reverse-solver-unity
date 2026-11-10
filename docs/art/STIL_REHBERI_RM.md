# Stil Rehberi — Royal Match'ten öğrendiklerimiz (PM, 1 Kasım 2026)

Kaynak: Zeyd'in kendi telefonundan aldığı ekran görüntüleri (`Assets/Art/References/1.11 ref/`, 17 görüntü + ekran kaydı) ve eski 11 görüntü (`Assets/Art/References/`). Ölçüler 1170×2532 iPhone görüntüsünden (≈ @3x).

**Bu rehber tarz içindir, kopya için değil.** Onların piksellerini, ikonlarını, karakterlerini, logosunu ve sahnelerini kullanma. Yapı ve ölçüleri onlardan al, içeriği ve renkleri bizden (`PALETTE.md`) kullan. Codex ve Claude Code her menü/UI görevinde önce bu dosyayı okur.

## Hangi görüntüde ne var

| Görüntü | İçerik |
|---|---|
| `IMG_2437`, `IMG_2440` | Ana ekran, alt çubuk (Home seçili), üst bar, Level tuşu |
| `IMG_2438 1` … `IMG_2443` | Diğer sekmeler (Areas, Leaderboard, Teams, Collection), profil kartı |
| `IMG_2444` | Üst bar yakın çekim |
| `IMG_2446`, `IMG_2453` | Bölüm başı kartı (Level + Play), Hard Level kartı |
| `IMG_2447`, `IMG_2448` | Oyun içi HUD (Target / Moves) |
| `IMG_2449` | Booster tuşları |
| `IMG_2450`, `IMG_2451` | Kazanma ve sonrası |
| `ScreenRecording_…MP4` | Sekme geçişi, liste kaydırma (atalet) ve animasyon hissi |

## 1. Alt sekme çubuğu (`IMG_2437`, `IMG_2441`)

- **Düz renk, kabartma yok.** Zemin tek düz renk: onlarda koyu mavi (28,84,164).
- **Seçili sekmenin yuvası iki kat geniş ve daha açık:** onlarda (31,116,203).
  - 1170 px ekranda seçili yuva ≈ 390 px, diğerleri ≈ 195 px.
  - Sekme değişince genişlik kayarak değişir.
- **Ayırıcı:** yuvalar arasında 2–3 px koyu dikey çizgi.
- **Üst kenar:** tek ince turuncu-altın şerit (≈ 10 px). Başka süs yok.
- **Konum:** çubuk ekranın en altına kadar iner (ev çubuğu alanı dahil). Yükseklik ≈ 257 px @3x (≈ 86 pt, güvenli alan dahil).
- **İkonlar:**
  - Tam renkli, 3D, kalın konturlu, büyük: yuva yüksekliğinin ~%60'ı.
  - Etraflarında çerçeve ya da daire yok, doğrudan zeminde.
- **Seçili sekme:** ikon ~%25 büyür ve çubuğun üstüne taşar. Altında tek etiket (kalın krem, ince koyu kontur). **Seçili olmayanlarda yazı yok.**
- **Bizde:**
  - Zemin `#10707F`, seçili yuva `#1A9AAE`, ayırıcı `#0B4A55`, üst şerit altın `#F38A03`→`#FBBA0E`.
  - Doygunluk onlarınki kadar yüksek, ama turkuaz.

## 2. Üst bar (`IMG_2440`, `IMG_2444`)

- **Konum:** durum çubuğunun hemen altından başlar (≈ y 140/2532). Boşluk bırakmaz.
- **Sol:** kare avatar kutusu (≈ 270 px @3x, 90 pt). Kalın açık çerçeve, avatar tam ortada, çerçeveye değmez.
  - Oyuncu adı burada yazmaz, adı profil kartında.
  - **Bizde:** isim istiyorsak kutunun sağında ayrı küçük bir plaket olur.
- **Sayaçlar:** her biri ayrı hap. İkon hapın solundan taşar ve haptan büyüktür. Sayı kalın, koyu lacivert, krem zemin üstünde. Haplar arası eşit boşluk.
- **Sağ:** ayar dişlisi koyu daire içinde, haplarla aynı yükseklikte.
- **Hizalama:** sayaç hapları ile avatar kutusunun dikey ortası aynı çizgide.

## 3. Ana eylem tuşu: "Level X" (`IMG_2437`)

- **Konum:** alt çubuğun hemen üstünde (≈ 70 px boşluk). Ekranın alt üçte birinde, ortalı. Geniş: ekranın ~%55'i.
- **Tuş yüzeyi:**
  - Parlak yeşil, iç kenarında daha açık yeşil ikinci bir çerçeve.
  - Dışta kalın turuncu-altın kenar.
  - Altta hafif koyu dudak (basılabilir hissi).
- **Yazı (en önemli):**
  - Kalın, yuvarlak, sıkışık harfli bir font.
  - **Dolgu krem-açık sarı** (#FFF6D8 civarı, yukarıdan aşağı çok hafif koyulaşan).
  - **İnce koyu yeşil kontur** (≈ yazı boyunun %5–6'sı).
  - Belirgin **aşağı doğru koyu gölge/kalınlık** (≈ %8–10, harfler kabartma gibi durur).
  - Yazı tuşun ~%55 yüksekliğinde. Büyük ve okunur.
- **Ek rozet:** sağ üst köşeye taşan küçük sekizgen rozet (x2 gibi). Bizde şimdilik yok.

## 4. Kartlar ve listeler (`IMG_2438`–`IMG_2443`, `IMG_2446`)

- **Başlık:** ekranın tepesinde, kalın krem yazı + koyu kontur + gölge (Level yazısıyla aynı aile). Sekme sayfalarında pankart yok, yazı doğrudan zemin üstünde.
- **Sekme seçici** (Join / Search / Create; Weekly / Friends): yan yana yuvarlak köşeli düğmeler. Seçili olan açık, diğerleri koyu. Hepsi eşit genişlikte.
- **Liste satırları:**
  - Krem karo, solda renkli ikon/rozet, ortada kalın ad, sağda küçük kapasite hapı ve turuncu "View" tuşu.
  - Satırlar arası 8–10 px.
- **Açılır kart** (Level kartı, profil):
  - Koyu mavi zemin, içinde krem panel.
  - Sağ üstte kırmızı yuvarlak X, kartın köşesinden taşar.
  - Alt ana tuş yeşil.
- **Kaydırma** (ekran kaydı): Fırlatınca uzun süre kayar ve yumuşak durur. Uçlarda esner.

## 5. Oyun içi HUD (`IMG_2447`, `IMG_2448`, `IMG_2449`)

- Üstte üç kutu: Target, karakter portresi, Moves. Mavi pankart başlıklı, krem iç.
- Sayılar çok büyük ve kalın, koyu konturlu.
- **Booster'lar:** yeşil daire tuş, turuncu kenar. Sağ altta kırmızı daire rozet, beyaz kalın sayı.
- **Bizde:** HUD ve booster'lar onaylı (Task14/15). Bu bölüm yalnız yazı stili için referans.

## 6. Ortak yazı stili (her yerde aynı aile)

| Yer | Dolgu | Kontur | Gölge |
|---|---|---|---|
| Ana tuş (Level X, Play, Continue) | krem→açık sarı | koyu yeşil `#1E4D05`, %5–6 | aşağı %8–10 koyu |
| Başlıklar, sekme etiketi | krem `#FFF6D8` | lacivert `#0B2A36`, %5–6 | aşağı %6 |
| Sayaç sayıları | lacivert `#0B2A36` | yok | yok (krem hap üstünde) |
| Turkuaz tuş yazısı | krem | lacivert, %5 | aşağı %6 |
| Kart içi açıklama | lacivert | yok | yok |

**Font:** kalın, yuvarlak ve Türkçe harfleri (ğ ş İ ı ç ö ü Ğ Ş Ç Ö Ü) eksiksiz olan bir OFL font. Claude Code adayları karşılaştırır (PM_MESAJ_27).

## 7. Oranlar ve yoğunluk

- **Altın/turuncu:** yalnız çerçeve kenarları, şeritler ve sayaç ikonları. Paneller renkli: mavi, krem, yeşil.
- **Ekran boş kalmaz:** sahne ya da panel her yeri doldurur. Kenar boşluğu dar (≈ 24 px @3x).
- **İkon dili:** her ikon kendi başına tok bir 3D nesne (kupa, kale, kalkan, kart destesi), ince çizgi ikon yok.
