# p21_07_t2_b · RED

normal · 31 sn · İki farklı fitil arasında duvarı okuyarak sıra seç.
Anahtar: Önce kısa fitilli bomba; ardından uzun fitilli bomba

Eğri: ████▇▇▇▆▆▆▆▇▆▆▆▆▆▆▅▅▅▅▅▅▅▅▅▅▅▅▅▅▄▄▃▃▃▃▂▂▂▂▂▂▂▂▂▂▁ · 13,13,13,13,12,12,12,11,11,11,11,12,11,11,11,10,10,10,9,8,9,9,9,9,9,9,9,9,9,8,8,8,7,6,5,4,4,4,3,2,2,2,2,2,2,2,2,2,1
49 adım; 0 boğaz, en uzun 1; göreli çığ 0.426.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 13 | >=3 |
| R04 boğaz uzunluğu | KALDI | 1 | [2, 4] |
| R05 boğaz sayısı | KALDI | 0 | >=1 |
| R06 göreli çığ | GEÇTİ | 0.4260491360491361 | >=0.35 |
| R07 ilk tuzak | GEÇTİ | 0.0 | <=0.3 |
| R08 tuzaklı adım | KALDI | 0 | >=1 |
| R09 fitil/adım p3 | GEÇTİ | 9 | <49 |
| R10 fitil/erken çıkış p3 | UYARI | {"fuse": 9, "k": -1} | F<=k+5 |
| R09 fitil/adım p17 | GEÇTİ | 14 | <49 |
| R10 fitil/erken çıkış p17 | UYARI | {"fuse": 14, "k": -1} | F<=k+5 |
| R11 tehdit adımı | GEÇTİ | 7 | <=8 |
| R12 bot_rastgele_bombasiz | GEÇTİ | 0 | <=0.10 |
| R12 bot_bomba_unutan | GEÇTİ | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 179, "median": 0.11161720274838004, "p25": 0.05381691642810461, "p75": 0.17263879631761953} | n>=20 ve kalibre model |
| R14 kalan süre | UYARI | {"source": "FunSim", "n": 2, "median": 0.08064516129032258, "p25": 0.07258064516129033, "p75": 0.08870967741935484} | [0.15, 0.3] |
| R16 final temposu | UYARI | {"tail": {"n": 179, "median": 0.2696396499500619, "p25": 0.24402544995455883, "p75": 0.30416371124956476}, "kalibrasyon": 0.2913306998428328} | kalibrasyon medyanı <= 0.2913306998428328; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 2 | 2 |
| R19 FunSim ilk deneme | KALDI | 0.002 | >=0.5 |
| R20 süreden kayıp | GEÇTİ | 0.031 | <=0.15 |
| R21 tahta oranı | GEÇTİ | {"board": [7, 7], "shapes": [[6, 6], [7, 7], [6, 7]]} | |w-h|<=1 |
| R22 çeşitlilik | UYARI | 0.7924528301886793 | <0.90 |

Rastgele (1000): kazanma 0.0%, bomba 100.0%, donma 0.0%.
Unutan: Bomb, 9 hamle. Bilinçli: Won, 49 hamle; bombalar [{'p': 3, 'step': 4, 'earliest': -1, 'fuse_margin': 6}, {'p': 17, 'step': 12, 'earliest': -1, 'fuse_margin': 3}].
FunSim: ilk deneme kazanma 0.2%, near-miss 1.1%.
Oyuncu modeli: ZeydBot v1 · kazanma 44.8%; kalan/timer {'n': 179, 'median': 0.11161720274838004, 'p25': 0.05381691642810461, 'p75': 0.17263879631761953}.
En yakın 3 (curve): S50 (SET40): 0.931; S41 (SET40): 0.928; S52 (SET40): 0.928
En yakın 3 (mask): S36 (SET40): 0.836; S21 (SET40): 0.824; S24 (SET40): 0.824

siluet maskesi yok: Generator tam dikdörtgen üretir; R15/R22 çeşitlilik UYARI.
FunSim ilk deneme süreden kayıp: 0.031

Eleştirmen: **RED**
- Öneriyi yeni adaylarda yeniden ölç.
- R04 boğaz uzunluğu: ölçülen 1, hedef [2, 4]; tarif `w,h` [6, 6] → [5, 5].
- Öneriyi yeni adaylarda yeniden ölç.
- R05 boğaz sayısı: ölçülen 0, hedef >=1; tarif `seed_from` 401 → 801.
- Yerleşim: bombayı 4. sıradan 5. sıraya al; earliest [5, 7].
- R08 tuzaklı adım: ölçülen 0, hedef >=1; tarif `bombs[0].earliest` [4, 6] → [5, 7].
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R19 FunSim ilk deneme: ölçülen 0.002, hedef >=0.5; tarif `bombs[0].region` center → center.
