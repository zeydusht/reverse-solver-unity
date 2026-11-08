# p21_07_t3_b · RED

normal · 35 sn · İki farklı fitil arasında duvarı okuyarak sıra seç.
Anahtar: Önce kısa fitilli bomba; ardından uzun fitilli bomba

Eğri: ▆▇▇█▇▇▇▆▆▆▆▆▅▅▅▅▅▄▄▃▃▃▃▃▃▃▃▂▂▁ · 8,9,9,10,9,9,9,8,8,8,8,8,7,7,7,6,6,5,5,4,3,3,3,3,4,4,3,2,2,1
30 adım; 0 boğaz, en uzun 1; göreli çığ 0.669.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 8 | >=3 |
| R04 boğaz uzunluğu | KALDI | 1 | [2, 4] |
| R05 boğaz sayısı | KALDI | 0 | >=1 |
| R06 göreli çığ | GEÇTİ | 0.6689153439153439 | >=0.35 |
| R07 ilk tuzak | GEÇTİ | 0.0 | <=0.3 |
| R08 tuzaklı adım | KALDI | 0 | >=1 |
| R09 fitil/adım p7 | GEÇTİ | 12 | <30 |
| R10 fitil/erken çıkış p7 | GEÇTİ | {"fuse": 12, "k": 7} | F<=k+5 |
| R09 fitil/adım p12 | GEÇTİ | 14 | <30 |
| R10 fitil/erken çıkış p12 | GEÇTİ | {"fuse": 14, "k": 9} | F<=k+5 |
| R11 tehdit adımı | KALDI | 10 | <=8 |
| R12 bot_rastgele_bombasiz | GEÇTİ | 0 | <=0.10 |
| R12 bot_bomba_unutan | GEÇTİ | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 399, "median": 0.48151276368618046, "p25": 0.39752817662100925, "p75": 0.5452200705897994} | n>=20 ve kalibre model |
| R14 kalan süre | UYARI | {"source": "FunSim", "n": 16, "median": 0.2, "p25": 0.10714285714285714, "p75": 0.3142857142857143} | [0.15, 0.3] |
| R16 final temposu | UYARI | {"tail": {"n": 399, "median": 0.2977036607430291, "p25": 0.25698406904634685, "p75": 0.3383029279560129}, "kalibrasyon": 0.2913306998428328} | kalibrasyon medyanı <= 0.2913306998428328; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 2 | 2 |
| R19 FunSim ilk deneme | KALDI | 0.016 | >=0.5 |
| R20 süreden kayıp | GEÇTİ | 0.007 | <=0.15 |
| R21 tahta oranı | GEÇTİ | {"board": [5, 6], "shapes": [[5, 5], [5, 6], [6, 5]]} | |w-h|<=1 |
| R22 çeşitlilik | UYARI | 0.7142857142857143 | <0.90 |

Rastgele (1000): kazanma 0.0%, bomba 100.0%, donma 0.0%.
Unutan: Bomb, 12 hamle. Bilinçli: Won, 30 hamle; bombalar [{'p': 7, 'step': 7, 'earliest': 7, 'fuse_margin': 6}, {'p': 12, 'step': 9, 'earliest': 9, 'fuse_margin': 6}].
FunSim: ilk deneme kazanma 1.6%, near-miss 1.8%.
Oyuncu modeli: ZeydBot v1 · kazanma 99.8%; kalan/timer {'n': 399, 'median': 0.48151276368618046, 'p25': 0.39752817662100925, 'p75': 0.5452200705897994}.
En yakın 3 (curve): p21_07_t3_c (aday): 0.913; S46 (SET40): 0.913; S20 (SET40): 0.909
En yakın 3 (mask): S06 (SET40): 0.938; S20 (SET40): 0.938; S47 (SET40): 0.938

siluet maskesi yok: Generator tam dikdörtgen üretir; R15/R22 çeşitlilik UYARI.
FunSim ilk deneme süreden kayıp: 0.007

Eleştirmen: **RED**
- Öneriyi yeni adaylarda yeniden ölç.
- R04 boğaz uzunluğu: ölçülen 1, hedef [2, 4]; tarif `w,h` [5, 5] → [5, 6].
- Öneriyi yeni adaylarda yeniden ölç.
- R05 boğaz sayısı: ölçülen 0, hedef >=1; tarif `seed_from` 801 → 1201.
- Yerleşim: bombayı 5. sıradan 6. sıraya al; earliest [6, 8].
- R08 tuzaklı adım: ölçülen 0, hedef >=1; tarif `bombs[0].earliest` [5, 7] → [6, 8].
- Yerleşim: bombayı 5. sıradan 4. sıraya al; earliest [4, 6].
- R11 tehdit adımı: ölçülen 10, hedef <=8; tarif `bombs[0].earliest` [5, 7] → [4, 6].
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R19 FunSim ilk deneme: ölçülen 0.016, hedef >=0.5; tarif `bombs[0].region` center → center.
