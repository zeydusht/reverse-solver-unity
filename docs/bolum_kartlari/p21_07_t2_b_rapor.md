# p21_07_t2_b · RED

normal · 33 sn · İki farklı fitil arasında duvarı okuyarak sıra seç.
Anahtar: Önce kısa fitilli bomba; ardından uzun fitilli bomba

Eğri: █▇▆▆▆▆▇▆▇▆▆▅▄▄▄▄▄▄▄▄▃▃▃▃▃▂▁▁ · 9,8,7,7,7,7,8,7,8,7,7,6,5,4,5,5,5,4,4,4,3,3,3,3,3,2,1,1
28 adım; 1 boğaz, en uzun 2; göreli çığ 0.585.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 7 | >=3 |
| R04 boğaz uzunluğu | GEÇTİ | 2 | [2, 4] |
| R05 boğaz sayısı | GEÇTİ | 1 | >=1 |
| R06 göreli çığ | GEÇTİ | 0.5849647266313932 | >=0.35 |
| R07 ilk tuzak | GEÇTİ | 0.0 | <=0.3 |
| R08 tuzaklı adım | KALDI | 0 | >=1 |
| R09 fitil/adım p9 | GEÇTİ | 8 | <28 |
| R10 fitil/erken çıkış p9 | GEÇTİ | {"fuse": 8, "k": 6} | F<=k+2 |
| R09 fitil/adım p10 | GEÇTİ | 9 | <28 |
| R10 fitil/erken çıkış p10 | GEÇTİ | {"fuse": 9, "k": 7} | F<=k+2 |
| R11 tehdit adımı | GEÇTİ | 6 | <=8 |
| R12 bot_rastgele_bombasiz | GEÇTİ | 0 | <=0.10 |
| R12 bot_bomba_unutan | GEÇTİ | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 400, "median": 0.5007974851968754, "p25": 0.41836359647999355, "p75": 0.5672447732413697} | n>=20 ve kalibre model |
| R14 kalan süre | GEÇTİ | {"source": "FunSim", "n": 118, "median": 0.2727272727272727, "p25": 0.18181818181818182, "p75": 0.42424242424242425} | [0.15, 0.3] |
| R16 final temposu | UYARI | {"tail": {"n": 400, "median": 0.28889489325049467, "p25": 0.25044417062983876, "p75": 0.3279257233142747}, "kalibrasyon": 0.2913306998428328} | kalibrasyon medyanı <= 0.2913306998428328; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 2 | 2 |
| R20 çeşitlilik | GEÇTİ | 0.7 | <0.90 |

Rastgele (1000): kazanma 0.0%, bomba 100.0%, donma 0.0%.
Unutan: Bomb, 8 hamle. Bilinçli: Won, 28 hamle; bombalar [{'p': 9, 'step': 6, 'earliest': 6, 'fuse_margin': 3}, {'p': 10, 'step': 7, 'earliest': 7, 'fuse_margin': 3}].
FunSim: ilk deneme kazanma 11.8%, near-miss 10.0%.
Oyuncu modeli: ZeydBot v1 · kazanma 100.0%; kalan/timer {'n': 400, 'median': 0.5007974851968754, 'p25': 0.41836359647999355, 'p75': 0.5672447732413697}.
En yakın 3 (curve): S42 (SET40): 0.928; S46 (SET40): 0.927; S52 (SET40): 0.922
En yakın 3 (mask): p21_07_t2_a (aday): 0.700; S44 (SET40): 0.571; S46 (SET40): 0.571

Eleştirmen: **RED**
- Yerleşim: bombayı 4. sıradan 5. sıraya al; earliest [5, 7].
- R08 tuzaklı adım: ölçülen 0, hedef >=1; tarif `bombs[0].earliest` [4, 6] → [5, 7].
