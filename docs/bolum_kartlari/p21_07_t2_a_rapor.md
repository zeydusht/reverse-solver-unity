# p21_07_t2_a · RED

normal · 31 sn · İki farklı fitil arasında duvarı okuyarak sıra seç.
Anahtar: Önce kısa fitilli bomba; ardından uzun fitilli bomba

Eğri: █▇▇▇▆▆▆▆▆▆▇▆▆▆▆▆▆▅▄▄▄▄▄▄▃▃▃▃▂▂▂▂▂▂▂▁ · 11,10,10,10,9,9,8,9,8,9,10,9,9,9,8,8,8,7,6,6,5,5,5,5,4,4,4,4,3,3,3,3,2,2,2,1
36 adım; 0 boğaz, en uzun 1; göreli çığ 0.571.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 10 | >=3 |
| R04 boğaz uzunluğu | KALDI | 1 | [2, 4] |
| R05 boğaz sayısı | KALDI | 0 | >=1 |
| R06 göreli çığ | GEÇTİ | 0.5707562639380821 | >=0.35 |
| R07 ilk tuzak | GEÇTİ | 0.0 | <=0.3 |
| R08 tuzaklı adım | KALDI | 0 | >=1 |
| R09 fitil/adım p7 | GEÇTİ | 9 | <36 |
| R10 fitil/erken çıkış p7 | UYARI | {"fuse": 9, "k": -1} | F<=k+5 |
| R09 fitil/adım p9 | GEÇTİ | 12 | <36 |
| R10 fitil/erken çıkış p9 | UYARI | {"fuse": 12, "k": -1} | F<=k+5 |
| R11 tehdit adımı | KALDI | 11 | <=8 |
| R12 bot_rastgele_bombasiz | GEÇTİ | 0 | <=0.10 |
| R12 bot_bomba_unutan | GEÇTİ | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 0, "median": null, "p25": null, "p75": null} | n>=20 ve kalibre model |
| R14 kalan süre | UYARI | {"source": "FunSim", "n": 0, "median": null, "p25": null, "p75": null} | [0.15, 0.3] |
| R16 final temposu | UYARI | {"tail": {"n": 0, "median": null, "p25": null, "p75": null}, "kalibrasyon": 0.2913306998428328} | kalibrasyon medyanı <= 0.2913306998428328; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 2 | 2 |
| R19 FunSim ilk deneme | KALDI | 0 | >=0.5 |
| R20 süreden kayıp | GEÇTİ | 0.018 | <=0.15 |
| R21 tahta oranı | GEÇTİ | {"board": [6, 6], "shapes": [[6, 6], [7, 7], [6, 7]]} | |w-h|<=1 |
| R22 çeşitlilik | UYARI | 0.7659574468085106 | <0.90 |

Rastgele (1000): kazanma 0.0%, bomba 100.0%, donma 0.0%.
Unutan: Bomb, 9 hamle. Bilinçli: Won, 36 hamle; bombalar [{'p': 7, 'step': 4, 'earliest': -1, 'fuse_margin': 6}, {'p': 9, 'step': 11, 'earliest': -1, 'fuse_margin': 2}].
FunSim: ilk deneme kazanma 0.0%, near-miss 0.4%.
Oyuncu modeli: ZeydBot v1 · kazanma 0.0%; kalan/timer {'n': 0, 'median': None, 'p25': None, 'p75': None}.
En yakın 3 (curve): p21_07_t2_c (aday): 0.946; S52 (SET40): 0.945; S46 (SET40): 0.938
En yakın 3 (mask): S08 (SET40): 0.925; S10 (SET40): 0.900; S13 (SET40): 0.900

siluet maskesi yok: Generator tam dikdörtgen üretir; R15/R22 çeşitlilik UYARI.
FunSim ilk deneme süreden kayıp: 0.018

Eleştirmen: **RED**
- Öneriyi yeni adaylarda yeniden ölç.
- R04 boğaz uzunluğu: ölçülen 1, hedef [2, 4]; tarif `w,h` [6, 6] → [5, 5].
- Öneriyi yeni adaylarda yeniden ölç.
- R05 boğaz sayısı: ölçülen 0, hedef >=1; tarif `seed_from` 401 → 801.
- Yerleşim: bombayı 4. sıradan 5. sıraya al; earliest [5, 7].
- R08 tuzaklı adım: ölçülen 0, hedef >=1; tarif `bombs[0].earliest` [4, 6] → [5, 7].
- Yerleşim: bombayı 4. sıradan 3. sıraya al; earliest [3, 5].
- R11 tehdit adımı: ölçülen 11, hedef <=8; tarif `bombs[0].earliest` [4, 6] → [3, 5].
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R19 FunSim ilk deneme: ölçülen 0, hedef >=0.5; tarif `bombs[0].region` center → center.
