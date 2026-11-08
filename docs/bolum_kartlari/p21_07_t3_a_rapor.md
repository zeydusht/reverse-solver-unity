# p21_07_t3_a · RED

normal · 35 sn · İki farklı fitil arasında duvarı okuyarak sıra seç.
Anahtar: Önce kısa fitilli bomba; ardından uzun fitilli bomba

Eğri: █████▇▆▆▁▁▁▅▅▅▄▃▃▃▃▃▃▃▃▂▁ · 10,10,10,10,10,9,8,8,1,1,1,7,7,6,5,4,4,3,3,3,3,3,3,2,1
25 adım; 1 boğaz, en uzun 3; göreli çığ 0.707.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 10 | >=3 |
| R04 boğaz uzunluğu | GEÇTİ | 3 | [2, 4] |
| R05 boğaz sayısı | GEÇTİ | 1 | >=1 |
| R06 göreli çığ | GEÇTİ | 0.7066964285714286 | >=0.35 |
| R07 ilk tuzak | GEÇTİ | 0.0 | <=0.3 |
| R08 tuzaklı adım | GEÇTİ | 3 | >=1 |
| R09 fitil/adım p11 | GEÇTİ | 11 | <25 |
| R10 fitil/erken çıkış p11 | GEÇTİ | {"fuse": 11, "k": 6} | F<=k+5 |
| R09 fitil/adım p12 | GEÇTİ | 10 | <25 |
| R10 fitil/erken çıkış p12 | GEÇTİ | {"fuse": 10, "k": 5} | F<=k+5 |
| R11 tehdit adımı | KALDI | 10 | <=8 |
| R12 bot_rastgele_bombasiz | GEÇTİ | 0 | <=0.10 |
| R12 bot_bomba_unutan | GEÇTİ | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 0, "median": null, "p25": null, "p75": null} | n>=20 ve kalibre model |
| R14 kalan süre | UYARI | {"source": "FunSim", "n": 0, "median": null, "p25": null, "p75": null} | [0.15, 0.3] |
| R16 final temposu | UYARI | {"tail": {"n": 0, "median": null, "p25": null, "p75": null}, "kalibrasyon": 0.2913306998428328} | kalibrasyon medyanı <= 0.2913306998428328; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 2 | 2 |
| R19 FunSim ilk deneme | KALDI | 0 | >=0.5 |
| R20 süreden kayıp | GEÇTİ | 0 | <=0.15 |
| R21 tahta oranı | GEÇTİ | {"board": [5, 5], "shapes": [[5, 5], [5, 6], [6, 5]]} | |w-h|<=1 |
| R22 çeşitlilik | UYARI | 0.7142857142857143 | <0.90 |

Rastgele (1000): kazanma 0.0%, bomba 100.0%, donma 0.0%.
Unutan: Bomb, 10 hamle. Bilinçli: Won, 25 hamle; bombalar [{'p': 12, 'step': 5, 'earliest': 5, 'fuse_margin': 6}, {'p': 11, 'step': 11, 'earliest': 6, 'fuse_margin': 1}].
FunSim: ilk deneme kazanma 0.0%, near-miss 0.1%.
Oyuncu modeli: ZeydBot v1 · kazanma 0.0%; kalan/timer {'n': 0, 'median': None, 'p25': None, 'p75': None}.
En yakın 3 (curve): S43 (SET40): 0.823; Z21 (SET40): 0.793; Z24 (SET40): 0.792
En yakın 3 (mask): S44 (SET40): 0.893; S46 (SET40): 0.893; S52 (SET40): 0.862

siluet maskesi yok: Generator tam dikdörtgen üretir; R15/R22 çeşitlilik UYARI.
FunSim ilk deneme süreden kayıp: 0

Eleştirmen: **RED**
- Yerleşim: bombayı 5. sıradan 4. sıraya al; earliest [4, 6].
- R11 tehdit adımı: ölçülen 10, hedef <=8; tarif `bombs[0].earliest` [5, 7] → [4, 6].
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R19 FunSim ilk deneme: ölçülen 0, hedef >=0.5; tarif `bombs[0].region` center → center.
