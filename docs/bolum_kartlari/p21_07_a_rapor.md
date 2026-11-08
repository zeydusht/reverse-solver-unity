# p21_07_a · RED

normal · 65 sn · İki farklı fitil arasında duvarı okuyarak sıra seç.
Anahtar: Önce kısa fitilli bomba; ardından uzun fitilli bomba

Eğri: █▇▇█▇▆▆▆▅▅▅▅▄▅▅▅▅▅▄▃▃▂▂▂▁▁▂▂▂▁ · 10,9,9,10,9,8,8,8,7,7,7,6,5,6,7,7,6,6,5,4,3,2,2,2,1,1,2,2,2,1
30 adım; 1 boğaz, en uzun 2; göreli çığ 0.477.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 9 | >=3 |
| R04 boğaz uzunluğu | GEÇTİ | 2 | [2, 4] |
| R05 boğaz sayısı | GEÇTİ | 1 | >=1 |
| R06 göreli çığ | GEÇTİ | 0.47680776014109344 | >=0.35 |
| R07 ilk tuzak | GEÇTİ | 0.0 | <=0.3 |
| R08 tuzaklı adım | KALDI | 0 | >=1 |
| R09 fitil/adım p7 | GEÇTİ | 7 | <30 |
| R10 fitil/erken çıkış p7 | KALDI | {"fuse": 7, "k": 4} | F<=k+2 |
| R09 fitil/adım p9 | GEÇTİ | 10 | <30 |
| R10 fitil/erken çıkış p9 | KALDI | {"fuse": 10, "k": 6} | F<=k+2 |
| R11 tehdit adımı | GEÇTİ | 5 | <=8 |
| R12 bot_rastgele_bombasiz | GEÇTİ | 0 | <=0.10 |
| R12 bot_bomba_unutan | GEÇTİ | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 133, "median": 0.7225127847695312, "p25": 0.6745572293795783, "p75": 0.7640384712791441} | n>=20 ve kalibre model |
| R14 kalan süre | KALDI | {"source": "FunSim", "n": 20, "median": 0.6076923076923076, "p25": 0.55, "p75": 0.6615384615384615} | [0.15, 0.3] |
| R16 final temposu | UYARI | {"tail": {"n": 133, "median": 0.27855921460232236, "p25": 0.2454569624832906, "p75": 0.313144747902872}, "kalibrasyon": 0.2913306998428328} | kalibrasyon medyanı <= 0.2913306998428328; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 2 | 2 |
| R20 çeşitlilik | GEÇTİ | 0.5434782608695652 | <0.90 |

Rastgele (1000): kazanma 0.0%, bomba 100.0%, donma 0.0%.
Unutan: Bomb, 7 hamle. Bilinçli: Won, 30 hamle; bombalar [{'p': 7, 'step': 4, 'earliest': 4, 'fuse_margin': 4}, {'p': 9, 'step': 8, 'earliest': 6, 'fuse_margin': 3}].
FunSim: ilk deneme kazanma 2.0%, near-miss 0.1%.
Oyuncu modeli: ZeydBot v1 · kazanma 33.2%; kalan/timer {'n': 133, 'median': 0.7225127847695312, 'p25': 0.6745572293795783, 'p75': 0.7640384712791441}.
En yakın 3 (curve): S46 (SET40): 0.944; S17 (SET40): 0.932; S42 (SET40): 0.924
En yakın 3 (mask): S44 (SET40): 0.758; S46 (SET40): 0.758; S10 (SET40): 0.750

Eleştirmen: **RED**
- Yerleşim: bombayı 3. sıradan 4. sıraya al; earliest [4, 6].
- R08 tuzaklı adım: ölçülen 0, hedef >=1; tarif `bombs[0].earliest` [3, 5] → [4, 6].
- Öneriyi yeni adaylarda yeniden ölç.
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R10 fitil/erken çıkış p7: ölçülen {'fuse': 7, 'k': 4}, hedef F<=k+2; tarif `bombs[0].margin` 3 → 2.
- Öneriyi yeni adaylarda yeniden ölç.
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R10 fitil/erken çıkış p9: ölçülen {'fuse': 10, 'k': 6}, hedef F<=k+2; tarif `bombs[1].margin` 4 → 2.
- Öneriyi yeni adaylarda yeniden ölç.
- R14 kalan süre: ölçülen {'source': 'FunSim', 'n': 20, 'median': 0.6076923076923076, 'p25': 0.55, 'p75': 0.6615384615384615}, hedef [0.15, 0.3]; tarif `timer` 65 → 33.
