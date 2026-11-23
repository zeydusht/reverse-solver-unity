# PM_96: Lobi, bölüm müziği ve kazanma fanfarı adaylarının lisansları

Hepsi **CC0 1.0** (kamu malı). Ticari kullanım serbest, atıf gerekmiyor, değiştirilebilir. Lisans metni: https://creativecommons.org/publicdomain/zero/1.0/

Her sayfada üç şey tek tek kontrol edildi: lisans alanı, açıklama ve "Attribution Notice" (atıf notu) alanı. Hiçbirinde "atıf zorunlu", "yalnız kişisel kullanım" ya da Content ID ibaresi yok.

Referans oyunun (Block Out!) dosyaları `docs/art/pm96/ref/` altında. Bunlar yalnız ölçüm için kullanıldı (tempo, vurmalı oranı, parlaklık). Oyuna konmadı, repoya da girmedi (yerel dışlama listesinde).

## Lobi müziği (`&lobi=`)

| Bayrak | Parça | Yükleyen (OpenGameArt) | Sayfa | Döngü |
|---|---|---|---|---|
| `c` (varsayılan, Zeyd'in seçimi) | Cozy Puzzle In-Game 2 | MintoDog | https://opengameart.org/content/cozy-puzzle-in-game-2 | PM_86'dan, 128 sn |
| `n` | Urban Shop | MintoDog | https://opengameart.org/content/urban-shop | Bestecinin döngüsü, 83,5 sn (92 BPM) |
| `o` | Bunny Dust | Kistol | https://opengameart.org/content/bunny-dust | Bestecinin döngü sürümü, 69,8 sn ("credit is not required") |
| `p` | Etirwer | Kistol | https://opengameart.org/content/etirwer | Bestecinin döngü sürümü, 130,2 sn |
| `q` | Sci-fi Puzzle Stage Select | MintoDog | https://opengameart.org/content/sci-fi-puzzle-stage-select | Bestecinin döngüsü, 64,0 sn (105 BPM) |

## Bölüm müziği (`&bolummuzik=`)

| Bayrak | Parça | Yükleyen | Sayfa | Döngü |
|---|---|---|---|---|
| `r` (varsayılan) | Drive | Zane Little Music | https://opengameart.org/content/drive-0 | Parçanın kendi tekrarı, 60,0 sn (136 BPM), vuruş başından |
| `s` | Sci-fi Puzzle In-Game 1 | MintoDog | https://opengameart.org/content/sci-fi-puzzle-in-game-1 | Bestecinin döngüsü, 139,1 sn (138 BPM) |
| `t` | Cool Adventure | MintoDog | https://opengameart.org/content/cool-adventure | Bestecinin döngüsü, 108,4 sn (124 BPM) |
| `u` | Cool City | MintoDog | https://opengameart.org/content/cool-city | Bestecinin döngüsü, 108,4 sn (124 BPM) |
| `v` | Sci-fi Puzzle Title | MintoDog | https://opengameart.org/content/sci-fi-puzzle-title | Bestecinin döngüsü, 80,0 sn (120 BPM) |
| `w` | Urban Theme | MintoDog | https://opengameart.org/content/urban-theme | Bestecinin döngüsü, 87,7 sn (115 BPM) |

**Notlar**

- **p** 130 sn, **s** 139 sn: 60–120 sn aralığının biraz üstünde. Bestecinin döngüsünü kesmek bozacağı için kendi uzunluklarında kaldılar.
- **w** 115 BPM, istenen 120–140 aralığının biraz altında.

## Kazanma fanfarı (`&kazanma=`)

| Bayrak | Ses | Kaynak | Sayfa | Lisans |
|---|---|---|---|---|
| `b` (varsayılan, Zeyd'in seçimi) | WinPizzicato | Fupi, "Win Jingle" | https://opengameart.org/content/win-jingle | CC0 |
| `d` | Trop la win (2,5 sn'de kısa bir solmayla biter) | Komiku (loyalty-freak-music) | https://opengameart.org/content/trop-la-win | CC0 |
| `e` | Marimba geçidi | Bizim bestemiz; VCSL kayıtlarıyla çalındı | https://github.com/sgossner/VCSL | VCSL: CC0 |
| `f` | Ksilofon zıplayışı | Bizim bestemiz; VCSL kayıtlarıyla çalındı | aynı | VCSL: CC0 |
| `g` | Piyano ta-da | Bizim bestemiz; VCSL kayıtlarıyla çalındı | aynı | VCSL: CC0 |
| `a`, `c`, `0` | PM_95'in jingle'ları ve en eski ses | `docs/art/pm95/LISANS.md` | | |

**VCSL:** Versilian Community Sample Library. Gerçek çalgıların tek tek nota kayıtları, deposundaki `LICENSE` CC0 1.0 Universal. Kullanılan kayıtlar:

- marimba, glockenspiel, ksilofon, kuyruklu piyano, konser arpı, boru çanları;
- trampet, büyük davul, timpani, zil, el çırpma, zil ağacı, rüzgâr çanı, üçgen, tef.

Melodiler bizim; hiçbir parçadan kopya yok.

**Alınmayanlar**

- **Bouncy Hamster Dancing** (cynicmusic) ve **Bluebonnet** (Kistol): sayfalarında atıf notu dolu ("http://cynicmusic.com…", "Music by Kistol.").
- **Glowsphere**: atıf notu var.
- **Cozy Puzzle** parçaları: hepsi zaten PM_86'da (a–d).
- **Apple Cider Juiced**'ın bölüm döngüleri: 24–27 sn, çok kısa.
- **Village 2018**, **Friends 2018** (Komiku), **Spinning** (Fupi): tekrar eden kısımları 21–41 sn, çok kısa.
- **Freesound** ve **Pixabay**: PM_95'teki gibi otomatik indirmeye kapalı.

## Üretim

- `Tools/make_music96.py`: müzik döngüleri, −18 LUFS. Kaynak dosyalar repoya konmadı; yukarıdaki sayfalardan yeniden indirilebilir.
- `Tools/make_fanfare96.py`: e, f, g fanfarları (VCSL kayıtlarından).
- `Tools/make_beatmap96.py`: bütün fanfarların web dosyaları ve vuruş haritası `Resources/Art/audio/pm96/vurus_haritasi.json`.
