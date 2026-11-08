# p21_07_t3_b · RED

normal · 33 sn · İki farklı fitil arasında duvarı okuyarak sıra seç.
Anahtar: Önce kısa fitilli bomba; ardından uzun fitilli bomba

Eğri: █▇▇▇▇███▇▇▆▆▅▅▄▄▄▄▄▃▃▃▃▂▂▂▂▁ · 14,13,12,12,12,14,14,14,13,12,11,10,9,8,7,7,6,6,6,5,4,4,4,3,2,2,2,1
28 adım; 0 boğaz, en uzun 1; göreli çığ 0.673.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 12 | >=3 |
| R04 boğaz uzunluğu | KALDI | 1 | [2, 4] |
| R05 boğaz sayısı | KALDI | 0 | >=1 |
| R06 göreli çığ | GEÇTİ | 0.67336860670194 | >=0.35 |
| R07 ilk tuzak | GEÇTİ | 0.0 | <=0.3 |
| R08 tuzaklı adım | KALDI | 0 | >=1 |
| R09 fitil/adım p10 | GEÇTİ | 8 | <28 |
| R10 fitil/erken çıkış p10 | GEÇTİ | {"fuse": 8, "k": 6} | F<=k+2 |
| R09 fitil/adım p14 | GEÇTİ | 10 | <28 |
| R10 fitil/erken çıkış p14 | GEÇTİ | {"fuse": 10, "k": 8} | F<=k+2 |
| R11 tehdit adımı | GEÇTİ | 6 | <=8 |
| R12 bot_rastgele_bombasiz | GEÇTİ | 0 | <=0.10 |
| R12 bot_bomba_unutan | GEÇTİ | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 400, "median": 0.5416416674775002, "p25": 0.47213726027267744, "p75": 0.6055211161129082} | n>=20 ve kalibre model |
| R14 kalan süre | UYARI | {"source": "FunSim", "n": 3, "median": 0.6363636363636364, "p25": 0.45454545454545453, "p75": 0.6515151515151515} | [0.15, 0.3] |
| R16 final temposu | UYARI | {"tail": {"n": 400, "median": 0.32410973547740496, "p25": 0.28193548863226986, "p75": 0.3697128355430874}, "kalibrasyon": 0.2913306998428328} | kalibrasyon medyanı <= 0.2913306998428328; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 2 | 2 |
| R20 çeşitlilik | GEÇTİ | 0.7 | <0.90 |

Rastgele (1000): kazanma 0.0%, bomba 100.0%, donma 0.0%.
Unutan: Bomb, 8 hamle. Bilinçli: Won, 28 hamle; bombalar [{'p': 10, 'step': 6, 'earliest': 6, 'fuse_margin': 3}, {'p': 14, 'step': 8, 'earliest': 8, 'fuse_margin': 3}].
FunSim: ilk deneme kazanma 0.3%, near-miss 0.2%.
Oyuncu modeli: ZeydBot v1 · kazanma 100.0%; kalan/timer {'n': 400, 'median': 0.5416416674775002, 'p25': 0.47213726027267744, 'p75': 0.6055211161129082}.
En yakın 3 (curve): S46 (SET40): 0.921; S42 (SET40): 0.916; S50 (SET40): 0.913
En yakın 3 (mask): p21_07_t3_c (aday): 0.700; S06 (SET40): 0.667; S20 (SET40): 0.667

Eleştirmen: **RED**
- Öneriyi yeni adaylarda yeniden ölç.
- R04 boğaz uzunluğu: ölçülen 1, hedef [2, 4]; tarif `w,h` [6, 5] → [6, 4].
- Öneriyi yeni adaylarda yeniden ölç.
- R05 boğaz sayısı: ölçülen 0, hedef >=1; tarif `seed_from` 801 → 1201.
- Yerleşim: bombayı 5. sıradan 6. sıraya al; earliest [6, 8].
- R08 tuzaklı adım: ölçülen 0, hedef >=1; tarif `bombs[0].earliest` [5, 7] → [6, 8].
