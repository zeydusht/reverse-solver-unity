# p21_06_a · GEÇTİ

tanitim · 58 sn · Bombayı fark et, yolunu aç ve rahatça bitir.
Anahtar: Alt, üst ve ortadaki bombaların sayaçlarını tanı

Eğri: ██▇▇▇▇▇▇▆▆▆▆▅▄▄▄▄▃▃▃▃▂▂▂▂▂▃▃▂▁ · 9,9,8,8,8,8,8,8,7,7,7,7,6,5,4,4,4,3,3,3,3,2,2,2,2,2,3,3,2,1
30 adım; 0 boğaz, en uzun 1; göreli çığ 0.582.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 8 | >=3 |
| R04 boğaz uzunluğu | GEÇTİ | 1 | [0, 1] |
| R05 boğaz sayısı | GEÇTİ | 0 | >=0 |
| R06 göreli çığ | GEÇTİ | 0.582363315696649 | >=0.35 |
| R07 ilk tuzak | UYARI | 0.0 | <=0 |
| R08 tuzaklı adım | GEÇTİ | 0 | >=0 |
| R09 fitil/adım p1 | GEÇTİ | 14 | <30 |
| R09 tanıtım fitil payı p1 | GEÇTİ | {"fuse": 14, "k": 2} | F>=k+4; üst sınır yok |
| R10 fitil/erken çıkış p1 | UYARI | {"fuse": 14, "k": 2} | F>=k+4 |
| R09 fitil/adım p7 | GEÇTİ | 16 | <30 |
| R09 tanıtım fitil payı p7 | GEÇTİ | {"fuse": 16, "k": 2} | F>=k+4; üst sınır yok |
| R10 fitil/erken çıkış p7 | UYARI | {"fuse": 16, "k": 2} | F>=k+4 |
| R09 fitil/adım p19 | GEÇTİ | 12 | <30 |
| R09 tanıtım fitil payı p19 | GEÇTİ | {"fuse": 12, "k": 2} | F>=k+4; üst sınır yok |
| R10 fitil/erken çıkış p19 | UYARI | {"fuse": 12, "k": 2} | F>=k+4 |
| R11 tehdit adımı | UYARI | 10 | <=5 |
| R12 bot_rastgele_bombasiz | UYARI | 0.119 | [0.15, 0.50] |
| R12 bot_bomba_unutan | UYARI | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 400, "median": 0.6871575878012237, "p25": 0.6442480864183078, "p75": 0.7307154902008306} | n>=20 ve kalibre model |
| R14 kalan süre | GEÇTİ | {"source": "FunSim", "n": 944, "median": 0.5, "p25": 0.3793103448275862, "p75": 0.6206896551724138} | [0.3, 0.5] |
| R15 ilk9 siluet | UYARI | 0.8333333333333334 | <0.90 |
| R16 final temposu | UYARI | {"tail": {"n": 400, "median": 0.2801072510666349, "p25": 0.24669444510576846, "p75": 0.32164433307211604}, "kalibrasyon": 0.2921853674850866} | kalibrasyon medyanı <= 0.2921853674850866; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 3 | 3 |
| R19 FunSim ilk deneme | GEÇTİ | 0.944 | >=0.9 |
| R20 süreden kayıp | GEÇTİ | 0.028 | <=0.05 |
| R21 tahta oranı | GEÇTİ | {"board": [6, 5], "shapes": [[6, 5], [5, 6], [6, 5]]} | |w-h|<=1 |
| R22 çeşitlilik | UYARI | 1.0 | <0.90 |

Rastgele (1000): kazanma 11.9%, bomba 88.1%, donma 0.0%.
Unutan: Bomb, 12 hamle. Bilinçli: Won, 30 hamle; bombalar [{'p': 19, 'step': 2, 'earliest': 2, 'fuse_margin': 11}, {'p': 1, 'step': 4, 'earliest': 2, 'fuse_margin': 11}, {'p': 7, 'step': 6, 'earliest': 2, 'fuse_margin': 11}].
FunSim: ilk deneme kazanma 94.4%, near-miss 3.4%.
Oyuncu modeli: ZeydBot v1 · kazanma 100.0%; kalan/timer {'n': 400, 'median': 0.6871575878012237, 'p25': 0.6442480864183078, 'p75': 0.7307154902008306}.
En yakın 3 (curve): S47 (SET40): 0.946; S42 (SET40): 0.928; S46 (SET40): 0.924
En yakın 3 (mask): p21_06_c (aday): 1.000; S44 (SET40): 0.833; S46 (SET40): 0.833
İlk 1–9 en yakın siluet: S44 (0.833).

siluet maskesi yok: Generator tam dikdörtgen üretir; R15/R22 çeşitlilik UYARI.
FunSim ilk deneme süreden kayıp: 0.028

Eleştirmen: **GEÇTİ**
