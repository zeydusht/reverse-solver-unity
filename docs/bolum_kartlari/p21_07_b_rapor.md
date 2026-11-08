# p21_07_b · RED

normal · 45 sn · İki farklı fitil arasında duvarı okuyarak sıra seç.
Anahtar: Önce kısa fitilli bomba; ardından uzun fitilli bomba

Eğri: ▇▇▇▇▇██▇▇▇▆▆▆▆▅▅▅▅▄▄▄▄▄▃▃▃▃▂▂▁ · 12,12,12,12,12,13,13,12,12,12,11,10,10,10,9,9,9,8,7,7,7,7,6,5,4,4,4,3,2,1
30 adım; 0 boğaz, en uzun 1; göreli çığ 0.857.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 12 | >=3 |
| R04 boğaz uzunluğu | KALDI | 1 | [2, 4] |
| R05 boğaz sayısı | KALDI | 0 | >=1 |
| R06 göreli çığ | GEÇTİ | 0.8565255731922399 | >=0.35 |
| R07 ilk tuzak | GEÇTİ | 0.0 | <=0.3 |
| R08 tuzaklı adım | KALDI | 0 | >=1 |
| R09 fitil/adım p1 | GEÇTİ | 13 | <30 |
| R10 fitil/erken çıkış p1 | KALDI | {"fuse": 13, "k": 5} | F<=k+5 |
| R09 fitil/adım p16 | GEÇTİ | 17 | <30 |
| R10 fitil/erken çıkış p16 | KALDI | {"fuse": 17, "k": 6} | F<=k+5 |
| R11 tehdit adımı | KALDI | 11 | <=8 |
| R12 bot_rastgele_bombasiz | GEÇTİ | 0.001 | <=0.10 |
| R12 bot_bomba_unutan | GEÇTİ | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 400, "median": 0.6308503258248858, "p25": 0.5728159232126043, "p75": 0.6793167842742905} | n>=20 ve kalibre model |
| R14 kalan süre | KALDI | {"source": "FunSim", "n": 242, "median": 0.4666666666666667, "p25": 0.31666666666666665, "p75": 0.5555555555555556} | [0.15, 0.3] |
| R16 final temposu | UYARI | {"tail": {"n": 400, "median": 0.2888439604175004, "p25": 0.2530467945301399, "p75": 0.32887313542448177}, "kalibrasyon": 0.2913306998428328} | kalibrasyon medyanı <= 0.2913306998428328; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 2 | 2 |
| R19 FunSim ilk deneme | KALDI | 0.242 | >=0.5 |
| R20 süreden kayıp | GEÇTİ | 0.017 | <=0.15 |
| R21 tahta oranı | GEÇTİ | {"board": [6, 5], "shapes": [[5, 5], [6, 5], [5, 6]]} | |w-h|<=1 |
| R22 çeşitlilik | UYARI | 0.6944444444444444 | <0.90 |

Rastgele (1000): kazanma 0.1%, bomba 99.9%, donma 0.0%.
Unutan: Bomb, 13 hamle. Bilinçli: Won, 30 hamle; bombalar [{'p': 1, 'step': 6, 'earliest': 5, 'fuse_margin': 8}, {'p': 16, 'step': 14, 'earliest': 6, 'fuse_margin': 4}].
FunSim: ilk deneme kazanma 24.2%, near-miss 4.7%.
Oyuncu modeli: ZeydBot v1 · kazanma 100.0%; kalan/timer {'n': 400, 'median': 0.6308503258248858, 'p25': 0.5728159232126043, 'p75': 0.6793167842742905}.
En yakın 3 (curve): S41 (SET40): 0.936; S20 (SET40): 0.932; S45 (SET40): 0.926
En yakın 3 (mask): S13 (SET40): 0.795; S19 (SET40): 0.775; S44 (SET40): 0.758

siluet maskesi yok: Generator tam dikdörtgen üretir; R15/R22 çeşitlilik UYARI.
FunSim ilk deneme süreden kayıp: 0.017

Eleştirmen: **RED**
- Öneriyi yeni adaylarda yeniden ölç.
- R04 boğaz uzunluğu: ölçülen 1, hedef [2, 4]; tarif `w,h` [5, 5] → [5, 6].
- Öneriyi yeni adaylarda yeniden ölç.
- R05 boğaz sayısı: ölçülen 0, hedef >=1; tarif `seed_from` 1 → 401.
- Yerleşim: bombayı 3. sıradan 4. sıraya al; earliest [4, 6].
- R08 tuzaklı adım: ölçülen 0, hedef >=1; tarif `bombs[0].earliest` [3, 5] → [4, 6].
- Öneriyi yeni adaylarda yeniden ölç.
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R10 fitil/erken çıkış p1: ölçülen {'fuse': 13, 'k': 5}, hedef F<=k+5; tarif `bombs[0].margin` 8 → 5.
- Öneriyi yeni adaylarda yeniden ölç.
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R10 fitil/erken çıkış p16: ölçülen {'fuse': 17, 'k': 6}, hedef F<=k+5; tarif `bombs[1].margin` 11 → 5.
- Yerleşim: bombayı 3. sıradan 2. sıraya al; earliest [2, 4].
- R11 tehdit adımı: ölçülen 11, hedef <=8; tarif `bombs[0].earliest` [3, 5] → [2, 4].
- Öneriyi yeni adaylarda yeniden ölç.
- R14 kalan süre: ölçülen {'source': 'FunSim', 'n': 242, 'median': 0.4666666666666667, 'p25': 0.31666666666666665, 'p75': 0.5555555555555556}, hedef [0.15, 0.3]; tarif `timer` 45 → 31.
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R19 FunSim ilk deneme: ölçülen 0.242, hedef >=0.5; tarif `bombs[0].region` any → center.
