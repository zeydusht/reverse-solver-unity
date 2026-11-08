# p21_07_c · RED

normal · 65 sn · İki farklı fitil arasında duvarı okuyarak sıra seç.
Anahtar: Önce kısa fitilli bomba; ardından uzun fitilli bomba

Eğri: █▇▆▆▆▆▆▆▅▅▆▅▅▅▄▃▃▂▂▂▂▂▂▂▂▂▂▁▁▂▂▁▁▁▁ · 12,11,10,9,9,9,10,9,8,8,9,8,7,7,6,5,4,3,2,2,2,2,3,3,3,3,2,1,1,2,2,1,1,1,1
35 adım; 2 boğaz, en uzun 4; göreli çığ 0.353.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 10 | >=3 |
| R04 boğaz uzunluğu | GEÇTİ | 4 | [2, 4] |
| R05 boğaz sayısı | GEÇTİ | 2 | >=1 |
| R06 göreli çığ | GEÇTİ | 0.35267939131575493 | >=0.35 |
| R07 ilk tuzak | GEÇTİ | 0.0 | <=0.3 |
| R08 tuzaklı adım | KALDI | 0 | >=1 |
| R09 fitil/adım p2 | GEÇTİ | 8 | <35 |
| R10 fitil/erken çıkış p2 | UYARI | {"fuse": 8, "k": -1} | F<=k+2 |
| R09 fitil/adım p13 | GEÇTİ | 13 | <35 |
| R10 fitil/erken çıkış p13 | UYARI | {"fuse": 13, "k": -1} | F<=k+2 |
| R11 tehdit adımı | GEÇTİ | 7 | <=8 |
| R12 bot_rastgele_bombasiz | GEÇTİ | 0 | <=0.10 |
| R12 bot_bomba_unutan | GEÇTİ | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 367, "median": 0.6380397885545968, "p25": 0.5883606927645896, "p75": 0.6815909062968253} | n>=20 ve kalibre model |
| R14 kalan süre | UYARI | {"source": "FunSim", "n": 3, "median": 0.5230769230769231, "p25": 0.46923076923076923, "p75": 0.5692307692307692} | [0.15, 0.3] |
| R16 final temposu | UYARI | {"tail": {"n": 367, "median": 0.301877775863054, "p25": 0.26517445785851834, "p75": 0.3365609940206452}, "kalibrasyon": 0.2913306998428328} | kalibrasyon medyanı <= 0.2913306998428328; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 2 | 2 |
| R20 çeşitlilik | GEÇTİ | 0.6829268292682927 | <0.90 |

Rastgele (1000): kazanma 0.0%, bomba 100.0%, donma 0.0%.
Unutan: Bomb, 8 hamle. Bilinçli: Won, 35 hamle; bombalar [{'p': 2, 'step': 7, 'earliest': -1, 'fuse_margin': 2}, {'p': 13, 'step': 11, 'earliest': -1, 'fuse_margin': 3}].
FunSim: ilk deneme kazanma 0.3%, near-miss 0.0%.
Oyuncu modeli: ZeydBot v1 · kazanma 91.8%; kalan/timer {'n': 367, 'median': 0.6380397885545968, 'p25': 0.5883606927645896, 'p75': 0.6815909062968253}.
En yakın 3 (curve): S21 (SET40): 0.928; Z04 (SET40): 0.927; S34 (SET40): 0.895
En yakın 3 (mask): Z21 (SET40): 0.800; S06 (SET40): 0.789; S20 (SET40): 0.789

Eleştirmen: **RED**
- Yerleşim: bombayı 3. sıradan 4. sıraya al; earliest [4, 6].
- R08 tuzaklı adım: ölçülen 0, hedef >=1; tarif `bombs[0].earliest` [3, 5] → [4, 6].
