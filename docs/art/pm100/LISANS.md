# PM_100: Kazanma tınıları, efektler ve geçiş seslerinin lisansları

Hepsi **CC0 1.0** (kamu malı): ticari kullanım serbest, atıf gerekmiyor. https://creativecommons.org/publicdomain/zero/1.0/

Çalgılar ve efektler kayıtlı ses. Yalnız müzik kutusundaki yumuşak pad bizim sentezimiz. Üretim araçları:

- `Tools/make_fanfare100.py`: kazanma tınıları;
- `Tools/make_sfx100.py`: efektler ve geçiş sesleri.

## Kazanma tınıları, `&kazanmaton=`

Notalar fanfar b'nin kendisi (Fupi, "Win Jingle", CC0: https://opengameart.org/content/win-jingle). Kayıttan çıkarıldı, yeniden çalındı.

| Bayrak | Çalgılar | Kaynak |
|---|---|---|
| `piyano` (varsayılan) | bugünkü fanfar | Fupi, Win Jingle |
| `marimba` | marimba, glockenspiel | VCSL: https://github.com/sgossner/VCSL |
| `pizz` | keman grubu pizzicato, çello pizzicato, el çanı | VSCO 2 CE: https://github.com/sgossner/VSCO-2-CE · VCSL (Hand Chimes) |
| `ksilofon` | ksilofon, marimba (bas), el çırpma | VCSL |
| `kutu` | glockenspiel (yumuşak), kalimba; altında yumuşak synth pad (bizim) | VCSL |
| `bakir` | trompet staccato, tuba staccato, timpani | VSCO 2 CE · VCSL (Timpani) |

- VCSL ve VSCO 2 CE'nin ikisi de deposunda CC0 1.0 (`LICENSE`).
- Gerçek bir müzik kutusu ya da celesta kaydı bu CC0 kütüphanelerde yok. Müzik kutusu tınısı bu yüzden glockenspiel ile kalimbadan kuruldu; ikisi de çelik dil ya da çubuk sesi.

## Efektler

Kaynakları PM_95 ve PM_99'daki gibi (`docs/art/pm95/LISANS.md`, `docs/art/pm99/LISANS.md`). Bu işte yalnız süzgeç ve düzey değişti:

- **Çıkış f–j:** 450 Hz altı kesildi.
- **Kalkış:** çıkışın ilk 45 ms'si.
- **Çarpma a:** ortası güçlendi, tepesi yumuşadı.
- **Bomba, kilit:** düzey yükseldi.

## Geçiş sesleri, `&gecis…=a|b|c`

| Geçiş | a | b | c |
|---|---|---|---|
| koridor | rüzgâr: "Wind Whoosh Loop" | okyanus davulu: VCSL Ocean Drum | hava: "Air Whoosh" |
| dalış | hava, içeri: Air Whoosh | rüzgâr, yükselen: Wind Whoosh Loop | ters swish: "Swish – bamboo stick" |
| dalış tingi | glockenspiel: VCSL | küçük çan: "Bell Dings/Chimes" | üçgen: VCSL Triangles |
| çizgiler | kâğıt hışırtısı: Kenney Casino Audio (card-shuffle) | sayfa: Kenney RPG Audio (bookFlip1) | fırça: VCSL Shaker, Small |
| geri | hava, dışarı: Air Whoosh | rüzgâr, alçalan: Wind Whoosh Loop | swish, yavaş: bamboo stick |
| HUD tıkı | Kenney UI Audio (click2) | Kenney Interface Sounds (click_002) | Kenney Impact Sounds (impactGlass_light_000) |
| sekme | artisticdude Swishes Sound Pack (swish-12) | aynı paket (swish-2) | Kenney Casino Audio (card-slide-1) |

Sayfalar:

- Air Whoosh: https://opengameart.org/content/air-whoosh
- Wind Whoosh Loop: https://opengameart.org/content/wind-whoosh-loop
- Swish – bamboo stick weapon swhoshes: https://opengameart.org/content/swish-bamboo-stick-weapon-swhoshes
- Bell Dings/Chimes: https://opengameart.org/content/bell-dingschimes
- Swishes Sound Pack: https://opengameart.org/content/swishes-sound-pack
- Kenney: https://kenney.nl/assets. Paketlerin `License.txt` dosyası: "Creative Commons Zero, CC0".

OpenGameArt sayfalarının hepsinde lisans CC0 yazıyor (23 Kasım'da bakıldı). "Weird whooshes" CC-BY olduğu için alınmadı.
