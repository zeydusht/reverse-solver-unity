# p21_06_t3_c · RED

tanitim · 19 sn · Bombayı fark et, yolunu aç ve rahatça bitir.
Anahtar: Tek bombanın kısa fitili

Eğri: █▇▆▆▆▅▄▄▄▄▄▄▃▃▃▂▂▁ · 9,8,7,7,7,6,5,5,5,5,4,4,3,3,3,2,2,1
18 adım; 0 boğaz, en uzun 1; göreli çığ 0.753.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 7 | >=3 |
| R04 boğaz uzunluğu | GEÇTİ | 1 | [0, 1] |
| R05 boğaz sayısı | GEÇTİ | 0 | >=0 |
| R06 göreli çığ | GEÇTİ | 0.7527777777777778 | >=0.35 |
| R07 ilk tuzak | GEÇTİ | 0.0 | <=0 |
| R08 tuzaklı adım | GEÇTİ | 0 | >=0 |
| R09 fitil/adım p8 | GEÇTİ | 7 | <18 |
| R10 fitil/erken çıkış p8 | GEÇTİ | {"fuse": 7, "k": 5} | F==k+2 |
| R11 tehdit adımı | GEÇTİ | 5 | <=5 |
| R12 bot_rastgele_bombasiz | KALDI | 0.002 | [0.15, 0.50] |
| R12 bot_bomba_unutan | GEÇTİ | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 400, "median": 0.4981780647134374, "p25": 0.40414150037452296, "p75": 0.5744476702824612} | n>=20 ve kalibre model |
| R14 kalan süre | GEÇTİ | {"source": "FunSim", "n": 78, "median": 0.3157894736842105, "p25": 0.21052631578947367, "p75": 0.47368421052631576} | [0.25, 0.5] |
| R15 ilk9 siluet | GEÇTİ | 0.5454545454545454 | <0.90 |
| R16 final temposu | UYARI | {"tail": {"n": 400, "median": 0.3163153645823398, "p25": 0.26892714802681794, "p75": 0.36884829170468525}, "kalibrasyon": 0.2921853674850866} | kalibrasyon medyanı <= 0.2921853674850866; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 1 | 1 |
| R19 FunSim ilk deneme | KALDI | 0.078 | >=0.65 |
| R20 çeşitlilik | GEÇTİ | 0.375 | <0.90 |

Rastgele (1000): kazanma 0.2%, bomba 99.8%, donma 0.0%.
Unutan: Bomb, 7 hamle. Bilinçli: Won, 18 hamle; bombalar [{'p': 8, 'step': 5, 'earliest': 5, 'fuse_margin': 3}].
FunSim: ilk deneme kazanma 7.8%, near-miss 4.2%.
Oyuncu modeli: ZeydBot v1 · kazanma 100.0%; kalan/timer {'n': 400, 'median': 0.4981780647134374, 'p25': 0.40414150037452296, 'p75': 0.5744476702824612}.
En yakın 3 (curve): S42 (SET40): 0.944; p21_06_t3_b (aday): 0.933; S46 (SET40): 0.916
En yakın 3 (mask): S42 (SET40): 0.545; S44 (SET40): 0.536; S46 (SET40): 0.536
İlk 1–9 en yakın siluet: S42 (0.545).

Eleştirmen: **RED**
- Yerleşim: bombayı 5. sıradan 6. sıraya al; earliest [6, 7].
- R12 bot_rastgele_bombasiz: ölçülen 0.002, hedef [0.15, 0.50]; tarif `bombs[0].earliest` [5, 6] → [6, 7].
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R19 FunSim ilk deneme: ölçülen 0.078, hedef >=0.65; tarif `bombs[0].region` center → center.
