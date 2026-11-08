# p21_07_t2_c · RED

normal · 31 sn · İki farklı fitil arasında duvarı okuyarak sıra seç.
Anahtar: Önce kısa fitilli bomba; ardından uzun fitilli bomba

Eğri: █▇▇▇▇▇▆▇▇▇▇▇▇▆▆▆▆▅▅▅▅▅▄▄▄▃▃▃▃▃▃▃▃▃▂▂▃▃▂▂▁▁ · 17,16,16,15,15,15,14,15,15,15,15,16,15,14,13,13,13,12,12,11,10,10,9,9,8,7,7,7,7,7,6,6,5,5,4,4,5,5,4,3,2,1
42 adım; 0 boğaz, en uzun 1; göreli çığ 0.734.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 16 | >=3 |
| R04 boğaz uzunluğu | KALDI | 1 | [2, 4] |
| R05 boğaz sayısı | KALDI | 0 | >=1 |
| R06 göreli çığ | GEÇTİ | 0.7341718110948879 | >=0.35 |
| R07 ilk tuzak | GEÇTİ | 0.0 | <=0.3 |
| R08 tuzaklı adım | KALDI | 0 | >=1 |
| R09 fitil/adım p13 | GEÇTİ | 11 | <42 |
| R10 fitil/erken çıkış p13 | UYARI | {"fuse": 11, "k": -1} | F<=k+5 |
| R09 fitil/adım p14 | GEÇTİ | 14 | <42 |
| R10 fitil/erken çıkış p14 | UYARI | {"fuse": 14, "k": -1} | F<=k+5 |
| R11 tehdit adımı | KALDI | 9 | <=8 |
| R12 bot_rastgele_bombasiz | GEÇTİ | 0 | <=0.10 |
| R12 bot_bomba_unutan | GEÇTİ | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 342, "median": 0.21986802325319574, "p25": 0.1348974768444584, "p75": 0.2964144045197203} | n>=20 ve kalibre model |
| R14 kalan süre | KALDI | {"source": "FunSim", "n": 26, "median": 0.12903225806451613, "p25": 0.06451612903225806, "p75": 0.3225806451612903} | [0.15, 0.3] |
| R16 final temposu | UYARI | {"tail": {"n": 342, "median": 0.2833981233716456, "p25": 0.2506706889257889, "p75": 0.31811754724842806}, "kalibrasyon": 0.2913306998428328} | kalibrasyon medyanı <= 0.2913306998428328; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 2 | 2 |
| R19 FunSim ilk deneme | KALDI | 0.026 | >=0.5 |
| R20 süreden kayıp | GEÇTİ | 0.097 | <=0.15 |
| R21 tahta oranı | GEÇTİ | {"board": [6, 7], "shapes": [[6, 6], [7, 7], [6, 7]]} | |w-h|<=1 |
| R22 çeşitlilik | UYARI | 0.7924528301886793 | <0.90 |

Rastgele (1000): kazanma 0.0%, bomba 100.0%, donma 0.0%.
Unutan: Bomb, 11 hamle. Bilinçli: Won, 42 hamle; bombalar [{'p': 13, 'step': 9, 'earliest': -1, 'fuse_margin': 3}, {'p': 14, 'step': 12, 'earliest': -1, 'fuse_margin': 3}].
FunSim: ilk deneme kazanma 2.6%, near-miss 6.2%.
Oyuncu modeli: ZeydBot v1 · kazanma 85.5%; kalan/timer {'n': 342, 'median': 0.21986802325319574, 'p25': 0.1348974768444584, 'p75': 0.2964144045197203}.
En yakın 3 (curve): p21_07_t2_a (aday): 0.946; S46 (SET40): 0.944; S52 (SET40): 0.941
En yakın 3 (mask): Z21 (SET40): 0.977; S21 (SET40): 0.955; S24 (SET40): 0.955

siluet maskesi yok: Generator tam dikdörtgen üretir; R15/R22 çeşitlilik UYARI.
FunSim ilk deneme süreden kayıp: 0.097

Eleştirmen: **RED**
- Öneriyi yeni adaylarda yeniden ölç.
- R04 boğaz uzunluğu: ölçülen 1, hedef [2, 4]; tarif `w,h` [6, 6] → [5, 5].
- Öneriyi yeni adaylarda yeniden ölç.
- R05 boğaz sayısı: ölçülen 0, hedef >=1; tarif `seed_from` 401 → 801.
- Yerleşim: bombayı 4. sıradan 5. sıraya al; earliest [5, 7].
- R08 tuzaklı adım: ölçülen 0, hedef >=1; tarif `bombs[0].earliest` [4, 6] → [5, 7].
- Yerleşim: bombayı 4. sıradan 3. sıraya al; earliest [3, 5].
- R11 tehdit adımı: ölçülen 9, hedef <=8; tarif `bombs[0].earliest` [4, 6] → [3, 5].
- Öneriyi yeni adaylarda yeniden ölç.
- R14 kalan süre: ölçülen {'source': 'FunSim', 'n': 26, 'median': 0.12903225806451613, 'p25': 0.06451612903225806, 'p75': 0.3225806451612903}, hedef [0.15, 0.3]; tarif `timer` 31 → 35.
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R19 FunSim ilk deneme: ölçülen 0.026, hedef >=0.5; tarif `bombs[0].region` center → center.
