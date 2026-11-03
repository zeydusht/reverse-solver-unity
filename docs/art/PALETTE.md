# Reverse Solver — ZORUNLU RENK PALETİ (v2, Task15, 26 Ekim 2026)

Zeyd'in kararı: Oyundaki her UI öğesi aynı paleti kullanacak. Kaynak, tahta çerçevesindeki "kraliyet altını"dır (`Assets/Art/Gameplay/Task7/board-frame-gold-v1.png`, renkler ölçülerek alındı).

Bu tablonun dışında altın, sarı ya da turuncu tonu kullanılmaz. Booster halkası, HUD rozeti, hap kenarları, kart kenarları ve tuşlar dahil hepsi aynı altın skalasıyla çizilir.

| Rol | Hex | Kullanım |
|---|---|---|
| Altın parlak | `#FBBA0E` | Çerçeve, halka ve kenarların ışık alan yüzü |
| Altın orta | `#F38A03` | Ana altın gövde |
| Altın gölge | `#CE5D01` | Altının gölgesi, iç kenar |
| Altın ışık | `#FFF1C2` | Küçük parlama lekeleri (az kullanılır) |
| Turkuaz iç | `#1C7F8C` | Tuş, hap ve rozetlerin iç zemini |
| Turkuaz koyu | `#0F4F5A` | Turkuazın gölgesi |
| Krem | `#F8EBD0` | Kart içi, bomba etiketi |
| Lacivert kontur / yazı | `#0B2A36` | Rakamlar, ince kontur |
| Mercan vurgu | `#E8574A` | Yalnız urgent süre; adet rozeti ve duvarda kullanılmaz |
| Kırmızı parlak | `#F04438` | Adet rozeti ve duvarın ışık alan kırmızısı |
| Kırmızı orta | `#D62822` | Yeni sıcak, canlı kırmızı ana yüzey |
| Kırmızı gölge | `#94160F` | Kırmızının alt/yan hacim gölgesi |
| Uyarı beyazı | `#FFF9F1` | Duvarın beyaz bantları ve yeni rozetin ince açık konturu |

## Kurallar

- **Booster ikonları** altın + turkuaz + krem ailesindedir. Task14 seti onaylıdır; yeni kırmızı kararı ikonları yeniden boyama izni değildir. Dört ikon yan yana bir set gibi görünmeli. İkonlar arasında ayrı ana renk YOK.
- **Tuş içindeki ikon**, iç dairenin yaklaşık %70'ini kaplar. Halkaya değmez; tuşun dışına taşmaz.
- **Rozet:** Kırmızı skala, ince uyarı beyazı kontur, krem rakam; sayı ayrı katmandır.

## Kırmızı — Task15

Ton referansı `Assets/Art/Reference/booster-bar-layout-reference.jpg` içindeki adet rozetidir; yalnız renk ailesi alınır. Örnek kırmızı bölgeden ölçülen alt/orta/üst örnekler `#902015 / #C82B1C / #D93533` oldu. Yeni skala bu sıcak aileyi daha temiz ışık/gölge basamaklarıyla kullanır. Mercan `#E8574A` yalnız süre aciliyetine ayrıldı. Duvarın beyazı krem karta göre daha aydınlıktır; iki malzeme karıştırılmaz.

## Task15 ana eylem yeşili

Zeyd Task15'te Başla/Devam tuşunu onaylı Level v2 ailesindeki yeşille açıkça istedi. Yeşil ana eylem tuşlarında kullanılabilir; ikincil tuşlar turkuazdır. Altın çerçeve skalası değişmedi.

| Rol | Hex |
|---|---|
| Yeşil ışık | `#B7EA43` |
| Yeşil parlak | `#79C800` |
| Yeşil orta | `#4A9D05` |
| Yeşil gölge | `#1E6305` |

## Jiggle avatar renkleri ve resim testi

Task15'in açık renk talebidir; bu tablo yalnız Jiggle gövdesi/portresi ve test tablosunun resim paletini genişletir. HUD veya booster ikonlarına yeni ana renk atanmaz. Göz/ağız, sıcak ana ışık ve altın/turkuaz UI ailesi korunur.

| Jiggle | Ana renk | Gölge |
|---|---|---|
| Mavi | `#28B8DE` | `#087BA2` |
| Pembe | `#EF83B3` | `#B33E7C` |
| Kırmızı | `#D62822` | `#94160F` |
| Yeşil | `#70C942` | `#368730` |
| Mor | `#AF77E8` | `#7546B1` |

Işıklar krem/uyarı beyazına, gölgeler laciverte karışabilir. Resim testinde deniz/gökyüzü için ek resim mavisi `#3C8DD9`, gölgesi `#245A87`; taş ve ahşap mevcut altın–krem–lacivert karışımlarıdır. Degradeler listelenen renklerin yumuşak karışımlarıdır; yeni bağımsız UI altını yoktur.

## Task17 — Onaylı renkli zincir çiftleri

Kullanıcının açık renk kararıdır; bu ek yalnız Task17 zincir/kilit çiftlerine uygulanır. Mercan `#E8574A` bu zincir ailesinde de kullanılabilir; rozet ve duvar kuralları korunur. Mor artık bu zincirlerin UI paletine de dahildir.

| Çift | Ana / ışık | Gölge | Renkten bağımsız işaret |
|---|---|---|---|
| Altın | `#FBBA0E` / `#FFF1C2` | `#F38A03`, `#CE5D01` | Beş köşeli yıldız |
| Turkuaz | `#1C7F8C`; ışık kremle karışır | `#0F4F5A` | Damla |
| Mor | `#7546B1` / `#AF77E8` | `#3D2858` | Hilal ay |
| Mercan | `#E8574A`; ışık kremle karışır | `#94160F` | Kalp |

Tüm çiftlerde 4 px krem `#F8EBD0` dış hale ve lacivert `#0B2A36` işaret zemini ortaktır. İşaretler de krem kenarlıdır; eşleşme yalnız renge dayanmaz. Degradeler bu renklerin karışımlarıdır.
