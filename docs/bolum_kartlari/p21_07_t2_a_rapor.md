# p21_07_t2_a · RED

normal · 33 sn · İki farklı fitil arasında duvarı okuyarak sıra seç.
Anahtar: Önce kısa fitilli bomba; ardından uzun fitilli bomba

Eğri: ████▇▇▇▆▆▆▆▆▆▆▆▆▅▆▆▅▅▅▅▄▃▃▃▃▃▃▃▃▂▂▁ · 12,12,12,12,11,11,11,10,10,9,9,9,9,9,9,9,8,9,9,8,8,8,7,6,5,4,4,4,5,5,5,4,3,2,1
35 adım; 0 boğaz, en uzun 1; göreli çığ 0.759.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 12 | >=3 |
| R04 boğaz uzunluğu | KALDI | 1 | [2, 4] |
| R05 boğaz sayısı | KALDI | 0 | >=1 |
| R06 göreli çığ | GEÇTİ | 0.7587826315099043 | >=0.35 |
| R07 ilk tuzak | GEÇTİ | 0.0 | <=0.3 |
| R08 tuzaklı adım | KALDI | 0 | >=1 |
| R09 fitil/adım p21 | GEÇTİ | 6 | <35 |
| R10 fitil/erken çıkış p21 | UYARI | {"fuse": 6, "k": -1} | F<=k+2 |
| R09 fitil/adım p22 | GEÇTİ | 11 | <35 |
| R10 fitil/erken çıkış p22 | UYARI | {"fuse": 11, "k": -1} | F<=k+2 |
| R11 tehdit adımı | GEÇTİ | 4 | <=8 |
| R12 bot_rastgele_bombasiz | GEÇTİ | 0 | <=0.10 |
| R12 bot_bomba_unutan | GEÇTİ | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 393, "median": 0.35800361472359193, "p25": 0.26344708667671785, "p75": 0.43445288671474436} | n>=20 ve kalibre model |
| R14 kalan süre | UYARI | {"source": "FunSim", "n": 19, "median": 0.24242424242424243, "p25": 0.10606060606060606, "p75": 0.3181818181818182} | [0.15, 0.3] |
| R16 final temposu | UYARI | {"tail": {"n": 393, "median": 0.3055094272672387, "p25": 0.27359986018810345, "p75": 0.3442978970689752}, "kalibrasyon": 0.2913306998428328} | kalibrasyon medyanı <= 0.2913306998428328; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 2 | 2 |
| R20 çeşitlilik | GEÇTİ | 0.7 | <0.90 |

Rastgele (1000): kazanma 0.0%, bomba 100.0%, donma 0.0%.
Unutan: Bomb, 6 hamle. Bilinçli: Won, 35 hamle; bombalar [{'p': 21, 'step': 4, 'earliest': -1, 'fuse_margin': 3}, {'p': 22, 'step': 9, 'earliest': -1, 'fuse_margin': 3}].
FunSim: ilk deneme kazanma 1.9%, near-miss 2.7%.
Oyuncu modeli: ZeydBot v1 · kazanma 98.2%; kalan/timer {'n': 393, 'median': 0.35800361472359193, 'p25': 0.26344708667671785, 'p75': 0.43445288671474436}.
En yakın 3 (curve): S41 (SET40): 0.935; S20 (SET40): 0.931; S52 (SET40): 0.929
En yakın 3 (mask): p21_07_t2_b (aday): 0.700; S08 (SET40): 0.689; S10 (SET40): 0.667

Eleştirmen: **RED**
- Öneriyi yeni adaylarda yeniden ölç.
- R04 boğaz uzunluğu: ölçülen 1, hedef [2, 4]; tarif `w,h` [7, 5] → [6, 4].
- Öneriyi yeni adaylarda yeniden ölç.
- R05 boğaz sayısı: ölçülen 0, hedef >=1; tarif `seed_from` 401 → 801.
- Yerleşim: bombayı 4. sıradan 5. sıraya al; earliest [5, 7].
- R08 tuzaklı adım: ölçülen 0, hedef >=1; tarif `bombs[0].earliest` [4, 6] → [5, 7].
