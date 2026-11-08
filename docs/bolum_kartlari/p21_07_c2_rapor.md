# p21_07_c2 · RED

normal · 45 sn · İki farklı fitil arasında duvarı okuyarak sıra seç.
Anahtar: Önce kısa fitilli bomba; ardından uzun fitilli bomba

Eğri: █▇▇██▇▇▇▇▇▆▆▇▆▆▅▄▃▃▃▄▆▆▆▆▅▄▄▃▂ · 7,6,6,7,7,6,6,6,6,6,5,5,6,5,5,4,3,2,2,2,3,5,5,5,5,4,3,3,2,1
30 adım; 0 boğaz, en uzun 1; göreli çığ 0.809.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 6 | >=3 |
| R04 boğaz uzunluğu | KALDI | 1 | [2, 4] |
| R05 boğaz sayısı | KALDI | 0 | >=1 |
| R06 göreli çığ | GEÇTİ | 0.8086860670194004 | >=0.35 |
| R07 ilk tuzak | GEÇTİ | 0.0 | <=0.3 |
| R08 tuzaklı adım | KALDI | 0 | >=1 |
| R09 fitil/adım p2 | GEÇTİ | 15 | <30 |
| R10 fitil/erken çıkış p2 | KALDI | {"fuse": 15, "k": 4} | F<=k+5 |
| R09 fitil/adım p8 | GEÇTİ | 21 | <30 |
| R10 fitil/erken çıkış p8 | KALDI | {"fuse": 21, "k": 7} | F<=k+5 |
| R11 tehdit adımı | KALDI | 13 | <=8 |
| R12 bot_rastgele_bombasiz | KALDI | 0.28 | <=0.10 |
| R12 bot_bomba_unutan | GEÇTİ | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 396, "median": 0.4178120334831491, "p25": 0.321188971556307, "p75": 0.4934855402968892} | n>=20 ve kalibre model |
| R14 kalan süre | GEÇTİ | {"source": "FunSim", "n": 370, "median": 0.2647058823529412, "p25": 0.14705882352941177, "p75": 0.38235294117647056} | [0.15, 0.3] |
| R16 final temposu | UYARI | {"tail": {"n": 396, "median": 0.2754944256480729, "p25": 0.24397378669061393, "p75": 0.316980096661572}, "kalibrasyon": 0.2913306998428328} | kalibrasyon medyanı <= 0.2913306998428328; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 2 | 2 |
| R19 FunSim ilk deneme | KALDI | 0.37 | >=0.5 |
| R20 süreden kayıp | KALDI | 0.31 | <=0.15 |
| R21 tahta oranı | GEÇTİ | {"board": [5, 6], "shapes": [[5, 5], [6, 5], [5, 6]]} | |w-h|<=1 |
| R22 çeşitlilik | UYARI | 0.6944444444444444 | <0.90 |

Rastgele (1000): kazanma 28.0%, bomba 72.0%, donma 0.0%.
Unutan: Bomb, 15 hamle. Bilinçli: Won, 30 hamle; bombalar [{'p': 2, 'step': 4, 'earliest': 4, 'fuse_margin': 12}, {'p': 8, 'step': 7, 'earliest': 7, 'fuse_margin': 15}].
FunSim: ilk deneme kazanma 37.0%, near-miss 23.5%.
Oyuncu modeli: ZeydBot v1 · kazanma 99.0%; kalan/timer {'n': 396, 'median': 0.4178120334831491, 'p25': 0.321188971556307, 'p75': 0.4934855402968892}.
En yakın 3 (curve): S20 (SET40): 0.869; S47 (SET40): 0.860; S41 (SET40): 0.839
En yakın 3 (mask): S06 (SET40): 0.909; S20 (SET40): 0.909; S47 (SET40): 0.909

siluet maskesi yok: Generator tam dikdörtgen üretir; R15/R22 çeşitlilik UYARI.
FunSim ilk deneme süreden kayıp: 0.31

Eleştirmen: **RED**
- Öneriyi yeni adaylarda yeniden ölç.
- R20 süreden kayıp: ölçülen 0.31, hedef <=0.15; tarif `timer` 45 → 60.
- Öneriyi yeni adaylarda yeniden ölç.
- R04 boğaz uzunluğu: ölçülen 1, hedef [2, 4]; tarif `w,h` [5, 5] → [5, 6].
- Öneriyi yeni adaylarda yeniden ölç.
- R05 boğaz sayısı: ölçülen 0, hedef >=1; tarif `seed_from` 1 → 401.
- Yerleşim: bombayı 3. sıradan 4. sıraya al; earliest [4, 6].
- R08 tuzaklı adım: ölçülen 0, hedef >=1; tarif `bombs[0].earliest` [3, 5] → [4, 6].
- Öneriyi yeni adaylarda yeniden ölç.
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R10 fitil/erken çıkış p2: ölçülen {'fuse': 15, 'k': 4}, hedef F<=k+5; tarif `bombs[0].margin` 8 → 5.
- Öneriyi yeni adaylarda yeniden ölç.
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R10 fitil/erken çıkış p8: ölçülen {'fuse': 21, 'k': 7}, hedef F<=k+5; tarif `bombs[1].margin` 11 → 5.
- Yerleşim: bombayı 3. sıradan 2. sıraya al; earliest [2, 4].
- R11 tehdit adımı: ölçülen 13, hedef <=8; tarif `bombs[0].earliest` [3, 5] → [2, 4].
- Yerleşim: bombayı 3. sıradan 4. sıraya al; earliest [4, 6].
- R12 bot_rastgele_bombasiz: ölçülen 0.28, hedef <=0.10; tarif `bombs[0].earliest` [3, 5] → [4, 6].
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R19 FunSim ilk deneme: ölçülen 0.37, hedef >=0.5; tarif `bombs[0].region` any → center.
