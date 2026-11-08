# p21_07_t3_c · GEÇTİ

normal · 33 sn · İki farklı fitil arasında duvarı okuyarak sıra seç.
Anahtar: Önce kısa fitilli bomba; ardından uzun fitilli bomba

Eğri: ███▇▇▁▁▁▁███▇▆▆▅▅▅▅▅▅▄▄▃▃▃▃▃▃▃▂▂▂▂▁ · 13,13,13,12,12,1,1,1,1,13,13,13,12,11,10,9,9,9,8,8,8,7,6,5,4,4,4,4,4,4,3,3,3,2,1
35 adım; 1 boğaz, en uzun 4; göreli çığ 0.663.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 13 | >=3 |
| R04 boğaz uzunluğu | GEÇTİ | 4 | [2, 4] |
| R05 boğaz sayısı | GEÇTİ | 1 | >=1 |
| R06 göreli çığ | GEÇTİ | 0.6632887314705497 | >=0.35 |
| R07 ilk tuzak | GEÇTİ | 0.0 | <=0.3 |
| R08 tuzaklı adım | GEÇTİ | 4 | >=1 |
| R09 fitil/adım p7 | GEÇTİ | 8 | <35 |
| R10 fitil/erken çıkış p7 | UYARI | {"fuse": 8, "k": -1} | F<=k+2 |
| R09 fitil/adım p12 | GEÇTİ | 9 | <35 |
| R10 fitil/erken çıkış p12 | UYARI | {"fuse": 9, "k": -1} | F<=k+2 |
| R11 tehdit adımı | GEÇTİ | 7 | <=8 |
| R12 bot_rastgele_bombasiz | GEÇTİ | 0 | <=0.10 |
| R12 bot_bomba_unutan | GEÇTİ | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 392, "median": 0.38364670603976936, "p25": 0.2964984705458225, "p75": 0.4500498696654407} | n>=20 ve kalibre model |
| R14 kalan süre | GEÇTİ | {"source": "FunSim", "n": 47, "median": 0.21212121212121213, "p25": 0.12121212121212122, "p75": 0.2878787878787879} | [0.15, 0.3] |
| R16 final temposu | UYARI | {"tail": {"n": 392, "median": 0.31210810560280033, "p25": 0.277064676711843, "p75": 0.34718200327651993}, "kalibrasyon": 0.2913306998428328} | kalibrasyon medyanı <= 0.2913306998428328; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 2 | 2 |
| R20 çeşitlilik | GEÇTİ | 0.7 | <0.90 |

Rastgele (1000): kazanma 0.0%, bomba 100.0%, donma 0.0%.
Unutan: Bomb, 8 hamle. Bilinçli: Won, 35 hamle; bombalar [{'p': 7, 'step': 8, 'earliest': -1, 'fuse_margin': 1}, {'p': 12, 'step': 9, 'earliest': -1, 'fuse_margin': 1}].
FunSim: ilk deneme kazanma 4.7%, near-miss 7.2%.
Oyuncu modeli: ZeydBot v1 · kazanma 98.0%; kalan/timer {'n': 392, 'median': 0.38364670603976936, 'p25': 0.2964984705458225, 'p75': 0.4500498696654407}.
En yakın 3 (curve): S36 (SET40): 0.823; S08 (SET40): 0.785; S46 (SET40): 0.762
En yakın 3 (mask): S06 (SET40): 0.789; S20 (SET40): 0.789; S47 (SET40): 0.789

Eleştirmen: **GEÇTİ**
