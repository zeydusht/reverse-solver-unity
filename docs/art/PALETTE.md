# Reverse Solver — ZORUNLU RENK PALETİ (v1, 26 Ekim 2026)

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
| Mercan vurgu | `#E8574A` | Yalnız kritik vurgular: sayı rozeti, urgent süre |

## Kurallar

- **Booster ikonları** bu paletten en fazla 3 renk alır: altın + turkuaz + krem veya mercan. Dört ikon yan yana bir set gibi görünmeli. İkonlar arasında ayrı ana renk YOK; kırmızı çekiç ile mor değnek gibi bir karışım yasak.
- **Tuş içindeki ikon**, iç dairenin yaklaşık %70'ini kaplar. Halkaya değmez; tuşun dışına taşmaz.
- **Rozet:** Mercan zemin, krem kontur, beyaz rakam.
