# p21_07_t2_c · RED

normal · 33 sn · İki farklı fitil arasında duvarı okuyarak sıra seç.
Anahtar: Önce kısa fitilli bomba; ardından uzun fitilli bomba

Eğri: █▇▇▇▆▆▆▆▅▅▅▅▄▄▄▃▃▂▃▃▃▃▂▂▂▂▂▂▂▁ · 12,11,11,11,10,10,9,9,8,8,7,7,6,6,6,5,4,3,4,5,5,4,3,3,3,3,3,3,2,1
30 adım; 0 boğaz, en uzun 1; göreli çığ 0.678.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 11 | >=3 |
| R04 boğaz uzunluğu | KALDI | 1 | [2, 4] |
| R05 boğaz sayısı | KALDI | 0 | >=1 |
| R06 göreli çığ | GEÇTİ | 0.6775573192239859 | >=0.35 |
| R07 ilk tuzak | GEÇTİ | 0.0 | <=0.3 |
| R08 tuzaklı adım | KALDI | 0 | >=1 |
| R09 fitil/adım p6 | GEÇTİ | 6 | <30 |
| R10 fitil/erken çıkış p6 | GEÇTİ | {"fuse": 6, "k": 4} | F<=k+2 |
| R09 fitil/adım p7 | GEÇTİ | 8 | <30 |
| R10 fitil/erken çıkış p7 | GEÇTİ | {"fuse": 8, "k": 6} | F<=k+2 |
| R11 tehdit adımı | GEÇTİ | 4 | <=8 |
| R12 bot_rastgele_bombasiz | GEÇTİ | 0 | <=0.10 |
| R12 bot_bomba_unutan | GEÇTİ | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 397, "median": 0.4638469211240432, "p25": 0.3867720220015486, "p75": 0.5371530567871192} | n>=20 ve kalibre model |
| R14 kalan süre | UYARI | {"source": "FunSim", "n": 11, "median": 0.24242424242424243, "p25": 0.16666666666666666, "p75": 0.3484848484848485} | [0.15, 0.3] |
| R16 final temposu | UYARI | {"tail": {"n": 397, "median": 0.28280246295568057, "p25": 0.24403198707675336, "p75": 0.3245954079681301}, "kalibrasyon": 0.2913306998428328} | kalibrasyon medyanı <= 0.2913306998428328; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 2 | 2 |
| R20 çeşitlilik | GEÇTİ | 0.5555555555555556 | <0.90 |

Rastgele (1000): kazanma 0.0%, bomba 100.0%, donma 0.0%.
Unutan: Bomb, 6 hamle. Bilinçli: Won, 30 hamle; bombalar [{'p': 6, 'step': 4, 'earliest': 4, 'fuse_margin': 3}, {'p': 7, 'step': 6, 'earliest': 6, 'fuse_margin': 3}].
FunSim: ilk deneme kazanma 1.1%, near-miss 2.1%.
Oyuncu modeli: ZeydBot v1 · kazanma 99.2%; kalan/timer {'n': 397, 'median': 0.4638469211240432, 'p25': 0.3867720220015486, 'p75': 0.5371530567871192}.
En yakın 3 (curve): S42 (SET40): 0.930; S43 (SET40): 0.915; S24 (SET40): 0.911
En yakın 3 (mask): S06 (SET40): 0.938; S20 (SET40): 0.938; S47 (SET40): 0.938

Eleştirmen: **RED**
- Öneriyi yeni adaylarda yeniden ölç.
- R04 boğaz uzunluğu: ölçülen 1, hedef [2, 4]; tarif `w,h` [7, 5] → [6, 4].
- Öneriyi yeni adaylarda yeniden ölç.
- R05 boğaz sayısı: ölçülen 0, hedef >=1; tarif `seed_from` 401 → 801.
- Yerleşim: bombayı 4. sıradan 5. sıraya al; earliest [5, 7].
- R08 tuzaklı adım: ölçülen 0, hedef >=1; tarif `bombs[0].earliest` [4, 6] → [5, 7].
