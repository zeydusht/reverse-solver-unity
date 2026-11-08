# p21_07_t3_c · RED

normal · 35 sn · İki farklı fitil arasında duvarı okuyarak sıra seç.
Anahtar: Önce kısa fitilli bomba; ardından uzun fitilli bomba

Eğri: █▇▇█▇▇▆▆▆▆▆▅▅▆▆▆▅▅▄▄▃▃▃▃▃▂▂▂▂▁ · 12,11,11,12,11,11,10,10,10,9,9,8,8,9,9,9,8,7,6,6,5,5,4,4,4,3,2,2,2,1
30 adım; 0 boğaz, en uzun 1; göreli çığ 0.673.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 11 | >=3 |
| R04 boğaz uzunluğu | KALDI | 1 | [2, 4] |
| R05 boğaz sayısı | KALDI | 0 | >=1 |
| R06 göreli çığ | GEÇTİ | 0.67336860670194 | >=0.35 |
| R07 ilk tuzak | GEÇTİ | 0.0 | <=0.3 |
| R08 tuzaklı adım | KALDI | 0 | >=1 |
| R09 fitil/adım p9 | GEÇTİ | 11 | <30 |
| R10 fitil/erken çıkış p9 | GEÇTİ | {"fuse": 11, "k": 6} | F<=k+5 |
| R09 fitil/adım p15 | GEÇTİ | 14 | <30 |
| R10 fitil/erken çıkış p15 | GEÇTİ | {"fuse": 14, "k": 9} | F<=k+5 |
| R11 tehdit adımı | KALDI | 9 | <=8 |
| R12 bot_rastgele_bombasiz | GEÇTİ | 0 | <=0.10 |
| R12 bot_bomba_unutan | GEÇTİ | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 400, "median": 0.5140618876498203, "p25": 0.44139878250062947, "p75": 0.577781068665081} | n>=20 ve kalibre model |
| R14 kalan süre | KALDI | {"source": "FunSim", "n": 98, "median": 0.3142857142857143, "p25": 0.2, "p75": 0.42857142857142855} | [0.15, 0.3] |
| R16 final temposu | UYARI | {"tail": {"n": 400, "median": 0.278949009698654, "p25": 0.24471451077340955, "p75": 0.3177369863200068}, "kalibrasyon": 0.2913306998428328} | kalibrasyon medyanı <= 0.2913306998428328; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 2 | 2 |
| R19 FunSim ilk deneme | KALDI | 0.098 | >=0.5 |
| R20 süreden kayıp | GEÇTİ | 0.035 | <=0.15 |
| R21 tahta oranı | GEÇTİ | {"board": [6, 5], "shapes": [[5, 5], [5, 6], [6, 5]]} | |w-h|<=1 |
| R22 çeşitlilik | UYARI | 0.7142857142857143 | <0.90 |

Rastgele (1000): kazanma 0.0%, bomba 100.0%, donma 0.0%.
Unutan: Bomb, 11 hamle. Bilinçli: Won, 30 hamle; bombalar [{'p': 9, 'step': 6, 'earliest': 6, 'fuse_margin': 6}, {'p': 15, 'step': 9, 'earliest': 9, 'fuse_margin': 6}].
FunSim: ilk deneme kazanma 9.8%, near-miss 7.7%.
Oyuncu modeli: ZeydBot v1 · kazanma 100.0%; kalan/timer {'n': 400, 'median': 0.5140618876498203, 'p25': 0.44139878250062947, 'p75': 0.577781068665081}.
En yakın 3 (curve): S20 (SET40): 0.938; S46 (SET40): 0.936; S52 (SET40): 0.935
En yakın 3 (mask): S19 (SET40): 0.842; S44 (SET40): 0.781; S46 (SET40): 0.781

siluet maskesi yok: Generator tam dikdörtgen üretir; R15/R22 çeşitlilik UYARI.
FunSim ilk deneme süreden kayıp: 0.035

Eleştirmen: **RED**
- Öneriyi yeni adaylarda yeniden ölç.
- R04 boğaz uzunluğu: ölçülen 1, hedef [2, 4]; tarif `w,h` [5, 5] → [5, 6].
- Öneriyi yeni adaylarda yeniden ölç.
- R05 boğaz sayısı: ölçülen 0, hedef >=1; tarif `seed_from` 801 → 1201.
- Yerleşim: bombayı 5. sıradan 6. sıraya al; earliest [6, 8].
- R08 tuzaklı adım: ölçülen 0, hedef >=1; tarif `bombs[0].earliest` [5, 7] → [6, 8].
- Yerleşim: bombayı 5. sıradan 4. sıraya al; earliest [4, 6].
- R11 tehdit adımı: ölçülen 9, hedef <=8; tarif `bombs[0].earliest` [5, 7] → [4, 6].
- Öneriyi yeni adaylarda yeniden ölç.
- R14 kalan süre: ölçülen {'source': 'FunSim', 'n': 98, 'median': 0.3142857142857143, 'p25': 0.2, 'p75': 0.42857142857142855}, hedef [0.15, 0.3]; tarif `timer` 35 → 31.
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R19 FunSim ilk deneme: ölçülen 0.098, hedef >=0.5; tarif `bombs[0].region` center → center.
