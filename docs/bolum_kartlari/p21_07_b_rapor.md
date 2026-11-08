# p21_07_b · RED

normal · 65 sn · İki farklı fitil arasında duvarı okuyarak sıra seç.
Anahtar: Önce kısa fitilli bomba; ardından uzun fitilli bomba

Eğri: █▇▆▆▇▇▆▇▇▆▅▅▆▅▄▄▄▄▄▃▂▂▂▁▁▁▁▁ · 9,8,7,7,8,8,7,8,8,7,6,6,7,6,5,5,5,4,4,3,2,2,2,1,1,1,1,1
28 adım; 1 boğaz, en uzun 5; göreli çığ 0.387.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 7 | >=3 |
| R04 boğaz uzunluğu | KALDI | 5 | [2, 4] |
| R05 boğaz sayısı | GEÇTİ | 1 | >=1 |
| R06 göreli çığ | GEÇTİ | 0.3873015873015873 | >=0.35 |
| R07 ilk tuzak | GEÇTİ | 0.0 | <=0.3 |
| R08 tuzaklı adım | KALDI | 0 | >=1 |
| R09 fitil/adım p8 | GEÇTİ | 6 | <28 |
| R10 fitil/erken çıkış p8 | KALDI | {"fuse": 6, "k": 3} | F<=k+2 |
| R09 fitil/adım p13 | GEÇTİ | 12 | <28 |
| R10 fitil/erken çıkış p13 | KALDI | {"fuse": 12, "k": 8} | F<=k+2 |
| R11 tehdit adımı | GEÇTİ | 5 | <=8 |
| R12 bot_rastgele_bombasiz | GEÇTİ | 0.001 | <=0.10 |
| R12 bot_bomba_unutan | GEÇTİ | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 104, "median": 0.7214863855541963, "p25": 0.6713804711981247, "p75": 0.7660786670943744} | n>=20 ve kalibre model |
| R14 kalan süre | KALDI | {"source": "FunSim", "n": 20, "median": 0.5538461538461539, "p25": 0.48846153846153845, "p75": 0.6269230769230769} | [0.15, 0.3] |
| R16 final temposu | UYARI | {"tail": {"n": 104, "median": 0.30521038238717957, "p25": 0.26644777738679104, "p75": 0.3636679672114371}, "kalibrasyon": 0.2913306998428328} | kalibrasyon medyanı <= 0.2913306998428328; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 2 | 2 |
| R20 çeşitlilik | GEÇTİ | 0.6829268292682927 | <0.90 |

Rastgele (1000): kazanma 0.1%, bomba 99.9%, donma 0.0%.
Unutan: Bomb, 6 hamle. Bilinçli: Won, 28 hamle; bombalar [{'p': 8, 'step': 5, 'earliest': 3, 'fuse_margin': 2}, {'p': 13, 'step': 9, 'earliest': 8, 'fuse_margin': 4}].
FunSim: ilk deneme kazanma 2.0%, near-miss 0.0%.
Oyuncu modeli: ZeydBot v1 · kazanma 26.0%; kalan/timer {'n': 104, 'median': 0.7214863855541963, 'p25': 0.6713804711981247, 'p75': 0.7660786670943744}.
En yakın 3 (curve): S52 (SET40): 0.922; S42 (SET40): 0.916; S46 (SET40): 0.912
En yakın 3 (mask): p21_07_c (aday): 0.683; S06 (SET40): 0.649; S20 (SET40): 0.649

Eleştirmen: **RED**
- Öneriyi yeni adaylarda yeniden ölç.
- R04 boğaz uzunluğu: ölçülen 5, hedef [2, 4]; tarif `w,h` [6, 5] → [6, 4].
- Yerleşim: bombayı 3. sıradan 4. sıraya al; earliest [4, 6].
- R08 tuzaklı adım: ölçülen 0, hedef >=1; tarif `bombs[0].earliest` [3, 5] → [4, 6].
- Öneriyi yeni adaylarda yeniden ölç.
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R10 fitil/erken çıkış p8: ölçülen {'fuse': 6, 'k': 3}, hedef F<=k+2; tarif `bombs[0].margin` 3 → 2.
- Öneriyi yeni adaylarda yeniden ölç.
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R10 fitil/erken çıkış p13: ölçülen {'fuse': 12, 'k': 8}, hedef F<=k+2; tarif `bombs[1].margin` 4 → 2.
- Öneriyi yeni adaylarda yeniden ölç.
- R14 kalan süre: ölçülen {'source': 'FunSim', 'n': 20, 'median': 0.5538461538461539, 'p25': 0.48846153846153845, 'p75': 0.6269230769230769}, hedef [0.15, 0.3]; tarif `timer` 65 → 37.
