# p21_07_t3_a · RED

normal · 33 sn · İki farklı fitil arasında duvarı okuyarak sıra seç.
Anahtar: Önce kısa fitilli bomba; ardından uzun fitilli bomba

Eğri: ▆▅▅▁▁▁▁▁▇█▇▆▆▆▆▆▆▅▅▄▃▃▃▃▃▃▂▁▁▁ · 8,7,7,1,1,1,1,1,9,10,9,8,8,8,8,8,8,7,6,5,4,4,4,4,4,3,2,1,1,1
30 adım; 2 boğaz, en uzun 5; göreli çığ 0.568.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 7 | >=3 |
| R04 boğaz uzunluğu | KALDI | 5 | [2, 4] |
| R05 boğaz sayısı | GEÇTİ | 2 | >=1 |
| R06 göreli çığ | GEÇTİ | 0.5684303350970017 | >=0.35 |
| R07 ilk tuzak | GEÇTİ | 0.0 | <=0.3 |
| R08 tuzaklı adım | GEÇTİ | 5 | >=1 |
| R09 fitil/adım p10 | GEÇTİ | 7 | <30 |
| R10 fitil/erken çıkış p10 | GEÇTİ | {"fuse": 7, "k": 5} | F<=k+2 |
| R09 fitil/adım p16 | GEÇTİ | 8 | <30 |
| R10 fitil/erken çıkış p16 | GEÇTİ | {"fuse": 8, "k": 6} | F<=k+2 |
| R11 tehdit adımı | GEÇTİ | 6 | <=8 |
| R12 bot_rastgele_bombasiz | GEÇTİ | 0.001 | <=0.10 |
| R12 bot_bomba_unutan | GEÇTİ | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 400, "median": 0.4877733145440611, "p25": 0.4116860072719203, "p75": 0.5555884652970078} | n>=20 ve kalibre model |
| R14 kalan süre | GEÇTİ | {"source": "FunSim", "n": 28, "median": 0.24242424242424243, "p25": 0.17424242424242425, "p75": 0.36363636363636365} | [0.15, 0.3] |
| R16 final temposu | UYARI | {"tail": {"n": 400, "median": 0.27019045249283047, "p25": 0.23695040656553512, "p75": 0.30780210527145296}, "kalibrasyon": 0.2913306998428328} | kalibrasyon medyanı <= 0.2913306998428328; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 2 | 2 |
| R20 çeşitlilik | GEÇTİ | 0.5555555555555556 | <0.90 |

Rastgele (1000): kazanma 0.1%, bomba 99.9%, donma 0.0%.
Unutan: Bomb, 7 hamle. Bilinçli: Won, 30 hamle; bombalar [{'p': 10, 'step': 7, 'earliest': 5, 'fuse_margin': 1}, {'p': 16, 'step': 8, 'earliest': 6, 'fuse_margin': 1}].
FunSim: ilk deneme kazanma 2.8%, near-miss 2.6%.
Oyuncu modeli: ZeydBot v1 · kazanma 100.0%; kalan/timer {'n': 400, 'median': 0.4877733145440611, 'p25': 0.4116860072719203, 'p75': 0.5555884652970078}.
En yakın 3 (curve): S29 (SET40): 0.726; p21_07_t3_c (aday): 0.718; S36 (SET40): 0.715
En yakın 3 (mask): S52 (SET40): 0.812; S44 (SET40): 0.781; S46 (SET40): 0.781

Eleştirmen: **RED**
- Öneriyi yeni adaylarda yeniden ölç.
- R04 boğaz uzunluğu: ölçülen 5, hedef [2, 4]; tarif `w,h` [6, 5] → [6, 4].
