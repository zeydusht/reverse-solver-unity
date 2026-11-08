# p21_07_a · RED

normal · 45 sn · İki farklı fitil arasında duvarı okuyarak sıra seç.
Anahtar: Önce kısa fitilli bomba; ardından uzun fitilli bomba

Eğri: ▇▇▇▇██▇▇▇▆▅▅▅▄▄▃▂▂▂▃▃▃▃▂▂ · 6,6,6,6,7,7,6,6,6,5,4,4,4,3,3,2,1,1,1,2,2,2,2,1,1
25 adım; 2 boğaz, en uzun 3; göreli çığ 0.458.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 6 | >=3 |
| R04 boğaz uzunluğu | GEÇTİ | 3 | [2, 4] |
| R05 boğaz sayısı | GEÇTİ | 2 | >=1 |
| R06 göreli çığ | GEÇTİ | 0.45848214285714284 | >=0.35 |
| R07 ilk tuzak | GEÇTİ | 0.0 | <=0.3 |
| R08 tuzaklı adım | KALDI | 0 | >=1 |
| R09 fitil/adım p2 | GEÇTİ | 11 | <25 |
| R10 fitil/erken çıkış p2 | KALDI | {"fuse": 11, "k": 3} | F<=k+5 |
| R09 fitil/adım p7 | GEÇTİ | 18 | <25 |
| R10 fitil/erken çıkış p7 | KALDI | {"fuse": 18, "k": 7} | F<=k+5 |
| R11 tehdit adımı | KALDI | 9 | <=8 |
| R12 bot_rastgele_bombasiz | KALDI | 0.147 | <=0.10 |
| R12 bot_bomba_unutan | GEÇTİ | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 400, "median": 0.6415235173339091, "p25": 0.5733218653888086, "p75": 0.6957021359973348} | n>=20 ve kalibre model |
| R14 kalan süre | KALDI | {"source": "FunSim", "n": 529, "median": 0.4666666666666667, "p25": 0.3111111111111111, "p75": 0.5777777777777777} | [0.15, 0.3] |
| R16 final temposu | UYARI | {"tail": {"n": 400, "median": 0.29527492831700886, "p25": 0.2609667714228232, "p75": 0.34364300641567647}, "kalibrasyon": 0.2913306998428328} | kalibrasyon medyanı <= 0.2913306998428328; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 2 | 2 |
| R19 FunSim ilk deneme | GEÇTİ | 0.529 | >=0.5 |
| R20 süreden kayıp | GEÇTİ | 0.035 | <=0.15 |
| R21 tahta oranı | GEÇTİ | {"board": [5, 5], "shapes": [[5, 5], [6, 5], [5, 6]]} | |w-h|<=1 |
| R22 çeşitlilik | UYARI | 0.6944444444444444 | <0.90 |

Rastgele (1000): kazanma 14.7%, bomba 85.3%, donma 0.0%.
Unutan: Bomb, 11 hamle. Bilinçli: Won, 25 hamle; bombalar [{'p': 2, 'step': 6, 'earliest': 3, 'fuse_margin': 6}, {'p': 7, 'step': 13, 'earliest': 7, 'fuse_margin': 6}].
FunSim: ilk deneme kazanma 52.9%, near-miss 4.9%.
Oyuncu modeli: ZeydBot v1 · kazanma 100.0%; kalan/timer {'n': 400, 'median': 0.6415235173339091, 'p25': 0.5733218653888086, 'p75': 0.6957021359973348}.
En yakın 3 (curve): S27 (SET40): 0.912; S19 (SET40): 0.911; S10 (SET40): 0.910
En yakın 3 (mask): S44 (SET40): 0.893; S46 (SET40): 0.893; S52 (SET40): 0.862

siluet maskesi yok: Generator tam dikdörtgen üretir; R15/R22 çeşitlilik UYARI.
FunSim ilk deneme süreden kayıp: 0.035

Eleştirmen: **RED**
- Yerleşim: bombayı 3. sıradan 4. sıraya al; earliest [4, 6].
- R08 tuzaklı adım: ölçülen 0, hedef >=1; tarif `bombs[0].earliest` [3, 5] → [4, 6].
- Öneriyi yeni adaylarda yeniden ölç.
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R10 fitil/erken çıkış p2: ölçülen {'fuse': 11, 'k': 3}, hedef F<=k+5; tarif `bombs[0].margin` 8 → 5.
- Öneriyi yeni adaylarda yeniden ölç.
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R10 fitil/erken çıkış p7: ölçülen {'fuse': 18, 'k': 7}, hedef F<=k+5; tarif `bombs[1].margin` 11 → 5.
- Yerleşim: bombayı 3. sıradan 2. sıraya al; earliest [2, 4].
- R11 tehdit adımı: ölçülen 9, hedef <=8; tarif `bombs[0].earliest` [3, 5] → [2, 4].
- Yerleşim: bombayı 3. sıradan 4. sıraya al; earliest [4, 6].
- R12 bot_rastgele_bombasiz: ölçülen 0.147, hedef <=0.10; tarif `bombs[0].earliest` [3, 5] → [4, 6].
- Öneriyi yeni adaylarda yeniden ölç.
- R14 kalan süre: ölçülen {'source': 'FunSim', 'n': 529, 'median': 0.4666666666666667, 'p25': 0.3111111111111111, 'p75': 0.5777777777777777}, hedef [0.15, 0.3]; tarif `timer` 45 → 31.
