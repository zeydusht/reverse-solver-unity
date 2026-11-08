# p21_06_t2_a · RED

tanitim · 19 sn · Bombayı fark et, yolunu aç ve rahatça bitir.
Anahtar: Tek bombanın kısa fitili

Eğri: ████▇▇▆▆▅▅▄▃▃▃▂ · 7,7,7,7,6,6,5,5,4,4,3,2,2,2,1
15 adım; 0 boğaz, en uzun 1; göreli çığ 0.753.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 7 | >=3 |
| R04 boğaz uzunluğu | GEÇTİ | 1 | [0, 1] |
| R05 boğaz sayısı | GEÇTİ | 0 | >=0 |
| R06 göreli çığ | GEÇTİ | 0.7533333333333333 | >=0.35 |
| R07 ilk tuzak | GEÇTİ | 0.0 | <=0 |
| R08 tuzaklı adım | GEÇTİ | 0 | >=0 |
| R09 fitil/adım p11 | GEÇTİ | 6 | <15 |
| R10 fitil/erken çıkış p11 | GEÇTİ | {"fuse": 6, "k": 4} | F==k+2 |
| R11 tehdit adımı | GEÇTİ | 4 | <=5 |
| R12 bot_rastgele_bombasiz | KALDI | 0.007 | [0.15, 0.50] |
| R12 bot_bomba_unutan | GEÇTİ | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 400, "median": 0.5983236159998517, "p25": 0.5242925989453668, "p75": 0.6611339734038811} | n>=20 ve kalibre model |
| R14 kalan süre | GEÇTİ | {"source": "FunSim", "n": 38, "median": 0.42105263157894735, "p25": 0.3157894736842105, "p75": 0.5263157894736842} | [0.25, 0.5] |
| R15 ilk9 siluet | GEÇTİ | 0.75 | <0.90 |
| R16 final temposu | UYARI | {"tail": {"n": 400, "median": 0.3474152203144899, "p25": 0.2894114804723109, "p75": 0.3953912071535236}, "kalibrasyon": 0.2921853674850866} | kalibrasyon medyanı <= 0.2921853674850866; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 1 | 1 |
| R19 FunSim ilk deneme | KALDI | 0.038 | >=0.65 |
| R20 çeşitlilik | GEÇTİ | 0.8333333333333334 | <0.90 |

Rastgele (1000): kazanma 0.7%, bomba 99.3%, donma 0.0%.
Unutan: Bomb, 6 hamle. Bilinçli: Won, 15 hamle; bombalar [{'p': 11, 'step': 4, 'earliest': 4, 'fuse_margin': 3}].
FunSim: ilk deneme kazanma 3.8%, near-miss 1.7%.
Oyuncu modeli: ZeydBot v1 · kazanma 100.0%; kalan/timer {'n': 400, 'median': 0.5983236159998517, 'p25': 0.5242925989453668, 'p75': 0.6611339734038811}.
En yakın 3 (curve): p21_06_t2_c (aday): 0.947; S50 (SET40): 0.944; S41 (SET40): 0.929
En yakın 3 (mask): p21_06_t2_b (aday): 0.833; S43 (SET40): 0.750; S45 (SET40): 0.750
İlk 1–9 en yakın siluet: S43 (0.750).

Eleştirmen: **RED**
- Yerleşim: bombayı 4. sıradan 5. sıraya al; earliest [5, 6].
- R12 bot_rastgele_bombasiz: ölçülen 0.007, hedef [0.15, 0.50]; tarif `bombs[0].earliest` [4, 5] → [5, 6].
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R19 FunSim ilk deneme: ölçülen 0.038, hedef >=0.65; tarif `bombs[0].region` center → center.
