# PM_102: Kazanma tınıları (g) ve booster seslerinin lisansları

Hepsi **CC0 1.0** (kamu malı): ticari kullanım serbest, atıf gerekmiyor. https://creativecommons.org/publicdomain/zero/1.0/

Hepsi kayıtlı ses. Yalnız müzik kutusundaki yumuşak pad bizim sentezimiz. Üretim araçları:

- `Tools/make_fanfare102.py`: g'nin tınıları;
- `Tools/make_sfx102.py`: booster sesleri, lastik sesi, koridor.

## Kazanma tınıları (g'nin notalarıyla), `&kazanmaton=`

g ("Piyano ta-da") bizim bestemiz (PM_96, `Tools/make_fanfare96.py`); çalgılar VCSL'den. Bu işte aynı notalar başka çalgılarla çalındı:

| Bayrak | Çalgılar | Kaynak |
|---|---|---|
| `piyano` (varsayılan) | g'nin kendisi | VCSL |
| `marimba` | marimba, glockenspiel | VCSL: https://github.com/sgossner/VCSL |
| `pizz` | keman grubu ve çello pizzicato, el çanı | VSCO 2 CE: https://github.com/sgossner/VSCO-2-CE · VCSL |
| `ksilofon` | ksilofon, marimba, el çırpma | VCSL |
| `kutu` | glockenspiel (yumuşak), kalimba; yumuşak synth pad (bizim) | VCSL |

Vurmalılar g'deki gibi (trampet, büyük davul, zil, timpani; müzik kutusunda üçgen ve çan ağacı): VCSL.

## Booster sesleri, `&sesmakas=` `&sesdegnek=` `&sescekic=` `&sessaat=` (a / b / c)

| | a | b | c |
|---|---|---|---|
| Makas | "Scissors" (OpenGameArt) | "Various Scissors": saç makası | "Various Scissors": bahçe makası |
| Değnek | VCSL Mark Trees (rüzgâr çanı) + puf | VSCO 2 CE glockenspiel glisandosu + puf | VCSL Bell Tree (ters) + puf |
| Çekiç | tahta vuruş + çatlama, tahta düşüşü | vuruş + seramik kırılma, düşüş | Kenney plank + tahta çatlama, taş tıkırtısı |
| Saat | "Ticking Clock" + buz kırılması ("Ice Breaking/Shattering") | "Tick and Tock" + cam tıkı + buz kırılması | "Clock tick" + Kenney cam + cam kırılması |

- **Puf:** artisticdude, "Swishes Sound Pack" (swish-7, yumuşatılmış).
- **Kırılma, düşme, tıkırtı:** "75 CC0 breaking / falling / hit sfx".
- **Kenney:** "Impact Sounds".

Sayfalar:

- Scissors: https://opengameart.org/content/scissors
- Various Scissors: https://opengameart.org/content/various-scissors
- 75 CC0 breaking / falling / hit sfx: https://opengameart.org/content/75-cc0-breaking-falling-hit-sfx
- Ticking Clock: https://opengameart.org/content/ticking-clock
- Tick and Tock: https://opengameart.org/content/tick-and-tock
- Clock tick: https://opengameart.org/content/clock-tick-0
- Ice Breaking/Shattering: https://opengameart.org/content/ice-breakingshattering
- Swishes Sound Pack: https://opengameart.org/content/swishes-sound-pack
- Kenney Impact Sounds: https://kenney.nl/assets/impact-sounds
- VCSL: https://github.com/sgossner/VCSL
- VSCO 2 CE: https://github.com/sgossner/VSCO-2-CE

OpenGameArt sayfalarının hepsinde lisans CC0 yazıyor (23 Kasım'da bakıldı).

Bu işte seslere yapılanlar:

- kısaltıldı, tepesi yumuşatıldı ("oyuncak tadı");
- ses düzeyi eşitlendi;
- değnekte parıltı ile puf birleştirildi;
- saatte tik-tak 1 sn'lik döngü yapıldı.

## Lastik çarpma sesi, koridor

Kaynaklar aynı (PM_95, PM_100). Yalnız düzey, uzunluk ve tepe eşitlendi.
