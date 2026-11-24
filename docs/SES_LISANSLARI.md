# Oyunda kullanılan seslerin lisansları

Mağaza ve gizlilik sayfası için tek liste (PM_103, 24 Kasım 2026; PM_107: Zeyd'in 24 Kasım 23:59 seçimleri). Yalnız oyunun bugün çaldığı sesler burada. Seçilmeyen adaylar dinleme sayfasında kalır, mağaza build'ine girmez.

Seçimler `Assets/Config/Resources/ses.json` içinde.

**Lisans.** Hepsi **CC0 1.0** (kamu malı) ya da bizim üretimimiz:

- ticari kullanım serbest, atıf zorunlu değil, değiştirilebilir;
- lisans metni: https://creativecommons.org/publicdomain/zero/1.0/
- OpenGameArt sayfalarının lisans alanı ve atıf notu tek tek kontrol edildi (20–23 Kasım);
- Kenney paketlerinin `License.txt` dosyasında: "Creative Commons Zero, CC0 … Crediting is not mandatory";
- VCSL ve VSCO 2 CE depolarının `LICENSE` dosyası: CC0 1.0 Universal.

Kaynak dosyalar repoda değil; aşağıdaki sayfalardan yeniden indirilebilir. Üzerlerinde yaptığımız kısaltma, süzgeç ve ses düzeyi işleri `Tools/make_*.py` araçlarında. PM_107: oyunun çaldığı her sesin miksaj kopyası (düzey, tiz yumuşatma, saldırı) `Tools/make_miks107.py` ile `Resources/Art/audio/pm107/`de; kaynakları aynı.

## Müzik

| Ses | Parça | Yazar | Kaynak | Lisans |
|---|---|---|---|---|
| Lobi (menü) | Urban Shop | MintoDog | https://opengameart.org/content/urban-shop | CC0 |
| Bölüm | Cat in Space (iki 32 sn yarısı sırayla) | mutantleg | https://opengameart.org/content/cat-in-space | CC0 |
| Kazanma fanfarı | "Piyano ta-da", pizzicato tınısı (PM_107): bizim bestemiz. Çalgılar: keman ve çello pizzicato (VSCO 2 CE), el çanları, trampet, büyük davul, zil, timpani (VCSL) | Versilian Studios (VSCO 2 CE, VCSL) | https://github.com/sgossner/VCSL , https://github.com/sgossner/VSCO-2-CE | CC0 |

## Oyun efektleri

| Ses | Kaynak | Yazar | Sayfa | Lisans |
|---|---|---|---|---|
| Parça çıkışı (eski f, &sescikis=f ile) ve kayma kalkışı | Swishes Sound Pack (swish-3, -4, -7, -8) | artisticdude | https://opengameart.org/content/swishes-sound-pack | CC0 |
| Lastik takılma (çarpma) | Impact Sounds (impactSoft_medium) | Kenney | https://kenney.nl/assets/impact-sounds | CC0 |
| Kırmızı duvara çarpma (PM_105, a) | Impact Sounds (impactWood_heavy_000, _002, _003) | Kenney | https://kenney.nl/assets/impact-sounds | CC0 |
| Zincir şıkırtısı (PM_107: f, oyunda varsayılan) | pickingchain.ogg (Freesound 643863) | Kneeling | https://freesound.org/s/643863/ | CC0 |
| Zincir gerilmesi (PM_107: f, oyunda varsayılan) | 80 CC0 RPG SFX (metal_01, metal_03) + pickingchain.ogg (Freesound 643863) | rubberduck, Kneeling | https://opengameart.org/content/80-cc0-rpg-sfx , https://freesound.org/s/643863/ | CC0 |
| Parça çıkışı (PM_107: u, oyunda varsayılan; tok + uzun ufalanma) | Impact Sounds (impactWood_light_000) + Casino Audio (chips-handle-5) | Kenney | https://kenney.nl/assets/impact-sounds , https://kenney.nl/assets/casino-audio | CC0 |
| Kilitli parçaya dokunma (PM_105, a) | Door Lock Sounds (LockedDoorHandleJiggle) | cough-e | https://opengameart.org/content/door-lock-sounds | CC0 |
| Kilit açılması (PM_105, a) | 100 CC0 SFX (key_open_01, _02) + Impact Sounds (impactMetal_light_001, _004) | rubberduck, Kenney | https://opengameart.org/content/100-cc0-sfx | CC0 |
| Bomba patlaması (PM_105, a) | 100 CC0 SFX (explosion) + Pop sounds (pop3) | rubberduck, cogitollc | https://opengameart.org/content/100-cc0-sfx , https://opengameart.org/content/pop-sounds | CC0 |
| Bomba sönmesi, kilit açılması | bizim sentezimiz (`Tools/make_pm72_sounds.py`) | – | – | bizim |
| Parça kopması (zincir), kaybetme | bizim sentezimiz (özgün oyun sesleri, kayıt örneği yok) | – | – | bizim |

## Arayüz sesleri

| Ses | Kaynak | Yazar | Sayfa | Lisans |
|---|---|---|---|---|
| Anahtar (ses, müzik, titreşim, dil) | UI Audio (switch16, switch18) | Kenney | https://kenney.nl/assets/ui-audio | CC0 |
| Panel açılış ve kapanış vuşu | Swishes Sound Pack (swish-4, swish-5) | artisticdude | https://opengameart.org/content/swishes-sound-pack | CC0 |
| Pasif düğme (tok) | Impact Sounds (impactWood_light) | Kenney | https://kenney.nl/assets/impact-sounds | CC0 |
| Düğme toku: bütün düğmeler (PM_107: c, yumuşak boğuk; tek kayıt) | Impact Sounds | Kenney | https://kenney.nl/assets/impact-sounds | CC0 |

## Booster sesleri

| Booster | Ses | Kaynak | Yazar | Sayfa | Lisans |
|---|---|---|---|---|---|
| Makas | bahçe makası "şık" | Various Scissors (garden_shears_01) | OpenGameArt | https://opengameart.org/content/various-scissors | CC0 |
| Değnek | rüzgâr çanı parıltısı | VCSL Mark Trees | Versilian Studios | https://github.com/sgossner/VCSL | CC0 |
| | "puf" | Swishes Sound Pack (swish-7) | artisticdude | https://opengameart.org/content/swishes-sound-pack | CC0 |
| Çekiç (PM_107: c) | kalın tahta vuruş, yumuşak kırıntı | Impact Sounds + 75 CC0 breaking / falling / hit sfx | Kenney, OpenGameArt | https://kenney.nl/assets/impact-sounds , https://opengameart.org/content/75-cc0-breaking-falling-hit-sfx | CC0 |
| Saat (PM_107: b) | tik-tak | Tick and Tock | OpenGameArt | https://opengameart.org/content/tick-and-tock | CC0 |
| | buz çatırtısı, buz kırılması | Ice Breaking/Shattering | OpenGameArt | https://opengameart.org/content/ice-breakingshattering | CC0 |

## Geçiş sesleri

| Geçiş | Kaynak | Yazar | Sayfa | Lisans |
|---|---|---|---|---|
| Koridor kayışı ve dalışın havası (PM_107) | Wind Whoosh Loop | OpenGameArt | https://opengameart.org/content/wind-whoosh-loop | CC0 |
| Dalış ve geri çekilme | Air Whoosh | OpenGameArt | https://opengameart.org/content/air-whoosh | CC0 |
| Kesik çizgileri (PM_107: c, fırça) | VCSL küçük çalkalayıcı | Versilian Studios | https://github.com/sgossner/VCSL | CC0 |
| HUD tıkı | UI Audio (click2) | Kenney | https://kenney.nl/assets/ui-audio | CC0 |
| Sekme kayışı | Swishes Sound Pack (swish-12) | artisticdude | https://opengameart.org/content/swishes-sound-pack | CC0 |

## Açılış hikâyesi (film)

| Ses | Kaynak | Yazar | Sayfa | Lisans |
|---|---|---|---|---|
| Müzik kutusu altyapısı | 4 Music Box Tracks (cute tune, spooky waltz, happy polka) | Aureolus_Omicron | https://opengameart.org/content/4-music-box-tracks | CC0 |
| Kapı, çuval, ayak, fırça, deri, yüksük | 50 RPG Sound Effects | Kenney | https://opengameart.org/content/50-rpg-sound-effects | CC0 |
| Göz kırpış tınısı | Pleasing Bell Sound Effect | Spring | https://opengameart.org/content/pleasing-bell-sound-effect | CC0 |
| Klip sesleri ve oyunun kendi sesleri | filmin kendi üretimi ve bizim sentezimiz (`docs/sounds/LISANS.md`) | – | – | bizim |

## Ayrıntı

Adaylar ve seçim gerekçeleri:

- `docs/art/pm95/LISANS.md`
- `docs/art/pm96/LISANS.md`
- `docs/art/pm98/LISANS.md`
- `docs/art/pm99/LISANS.md`
- `docs/art/pm100/LISANS.md`
- `docs/art/pm102/LISANS.md`
- `docs/sounds/LISANS.md`
