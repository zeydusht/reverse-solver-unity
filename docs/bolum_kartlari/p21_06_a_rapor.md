# p21_06_a · GEÇTİ

tanitim · 48 sn · Bombayı fark et, yolunu aç ve rahatça bitir.
Anahtar: Alt, üst ve ortadaki bombaların sayaçlarını tanı

Eğri: ▇▇████▇▆▅▄▄▅▅▅▄▃▃▂▃▃▃▃▂▂▁ · 7,7,8,8,8,8,7,6,5,4,4,5,5,5,4,3,3,2,3,3,3,3,2,2,1
25 adım; 0 boğaz, en uzun 1; göreli çığ 0.649.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 7 | >=3 |
| R04 boğaz uzunluğu | GEÇTİ | 1 | [0, 1] |
| R05 boğaz sayısı | GEÇTİ | 0 | >=0 |
| R06 göreli çığ | GEÇTİ | 0.6494047619047619 | >=0.35 |
| R07 ilk tuzak | UYARI | 0.0 | <=0 |
| R08 tuzaklı adım | GEÇTİ | 0 | >=0 |
| R09 fitil/adım p1 | GEÇTİ | 12 | <25 |
| R09 tanıtım fitil payı p1 | GEÇTİ | {"fuse": 12, "k": 2} | F>=k+4; üst sınır yok |
| R10 fitil/erken çıkış p1 | UYARI | {"fuse": 12, "k": 2} | F>=k+4 |
| R09 fitil/adım p15 | GEÇTİ | 10 | <25 |
| R09 tanıtım fitil payı p15 | GEÇTİ | {"fuse": 10, "k": 2} | F>=k+4; üst sınır yok |
| R10 fitil/erken çıkış p15 | UYARI | {"fuse": 10, "k": 2} | F>=k+4 |
| R09 fitil/adım p18 | GEÇTİ | 14 | <25 |
| R09 tanıtım fitil payı p18 | GEÇTİ | {"fuse": 14, "k": 2} | F>=k+4; üst sınır yok |
| R10 fitil/erken çıkış p18 | UYARI | {"fuse": 14, "k": 2} | F>=k+4 |
| R11 tehdit adımı | UYARI | 8 | <=5 |
| R12 bot_rastgele_bombasiz | UYARI | 0.312 | [0.15, 0.50] |
| R12 bot_bomba_unutan | UYARI | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 400, "median": 0.6888280093954232, "p25": 0.6289664223170895, "p75": 0.7344645852999055} | n>=20 ve kalibre model |
| R14 kalan süre | GEÇTİ | {"source": "FunSim", "n": 921, "median": 0.5, "p25": 0.375, "p75": 0.625} | [0.3, 0.5] |
| R15 ilk9 siluet | UYARI | 1.0 | <0.90 |
| R16 final temposu | UYARI | {"tail": {"n": 400, "median": 0.2930905540912574, "p25": 0.2531437408856946, "p75": 0.3339509245915303}, "kalibrasyon": 0.2921853674850866} | kalibrasyon medyanı <= 0.2921853674850866; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 3 | 3 |
| R19 FunSim ilk deneme | GEÇTİ | 0.921 | >=0.9 |
| R20 süreden kayıp | GEÇTİ | 0.03 | <=0.05 |
| R21 tahta oranı | GEÇTİ | {"board": [5, 5], "shapes": [[5, 5], [6, 5], [5, 6]]} | |w-h|<=1 |
| R22 çeşitlilik | UYARI | 0.8333333333333334 | <0.90 |

Rastgele (1000): kazanma 31.2%, bomba 68.8%, donma 0.0%.
Unutan: Bomb, 10 hamle. Bilinçli: Won, 25 hamle; bombalar [{'p': 15, 'step': 2, 'earliest': 2, 'fuse_margin': 9}, {'p': 1, 'step': 4, 'earliest': 2, 'fuse_margin': 9}, {'p': 18, 'step': 6, 'earliest': 2, 'fuse_margin': 9}].
FunSim: ilk deneme kazanma 92.1%, near-miss 4.1%.
Oyuncu modeli: ZeydBot v1 · kazanma 100.0%; kalan/timer {'n': 400, 'median': 0.6888280093954232, 'p25': 0.6289664223170895, 'p75': 0.7344645852999055}.
En yakın 3 (curve): S47 (SET40): 0.912; S10 (SET40): 0.909; S46 (SET40): 0.900
En yakın 3 (mask): S44 (SET40): 1.000; S46 (SET40): 1.000; S52 (SET40): 0.962
İlk 1–9 en yakın siluet: S44 (1.000).

siluet maskesi yok: Generator tam dikdörtgen üretir; R15/R22 çeşitlilik UYARI.
FunSim ilk deneme süreden kayıp: 0.03

Eleştirmen: **GEÇTİ**
