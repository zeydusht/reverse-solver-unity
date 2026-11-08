# p21_06_b · RED

tanitim · 58 sn · Bombayı fark et, yolunu aç ve rahatça bitir.
Anahtar: Alt, üst ve ortadaki bombaların sayaçlarını tanı

Eğri: ▆▆▇▇▇████▇▇▇▆▆▅▆▆▅▅▅▄▄▄▄▄▄▃▂▂▁ · 7,7,8,8,8,9,9,9,9,8,8,8,7,7,6,7,7,6,6,6,5,5,5,5,4,4,3,2,2,1
30 adım; 0 boğaz, en uzun 1; göreli çığ 0.753.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 7 | >=3 |
| R04 boğaz uzunluğu | GEÇTİ | 1 | [0, 1] |
| R05 boğaz sayısı | GEÇTİ | 0 | >=0 |
| R06 göreli çığ | GEÇTİ | 0.7531305114638448 | >=0.35 |
| R07 ilk tuzak | UYARI | 0.0 | <=0 |
| R08 tuzaklı adım | GEÇTİ | 0 | >=0 |
| R09 fitil/adım p1 | GEÇTİ | 14 | <30 |
| R09 tanıtım fitil payı p1 | GEÇTİ | {"fuse": 14, "k": 2} | F>=k+4; üst sınır yok |
| R10 fitil/erken çıkış p1 | UYARI | {"fuse": 14, "k": 2} | F>=k+4 |
| R09 fitil/adım p19 | GEÇTİ | 13 | <30 |
| R09 tanıtım fitil payı p19 | GEÇTİ | {"fuse": 13, "k": 3} | F>=k+4; üst sınır yok |
| R10 fitil/erken çıkış p19 | UYARI | {"fuse": 13, "k": 3} | F>=k+4 |
| R09 fitil/adım p22 | GEÇTİ | 17 | <30 |
| R09 tanıtım fitil payı p22 | GEÇTİ | {"fuse": 17, "k": 3} | F>=k+4; üst sınır yok |
| R10 fitil/erken çıkış p22 | UYARI | {"fuse": 17, "k": 3} | F>=k+4 |
| R11 tehdit adımı | UYARI | 11 | <=5 |
| R12 bot_rastgele_bombasiz | UYARI | 0.245 | [0.15, 0.50] |
| R12 bot_bomba_unutan | UYARI | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 400, "median": 0.6838909236982618, "p25": 0.6336808205980762, "p75": 0.724003493294129} | n>=20 ve kalibre model |
| R14 kalan süre | GEÇTİ | {"source": "FunSim", "n": 758, "median": 0.4827586206896552, "p25": 0.3448275862068966, "p75": 0.603448275862069} | [0.3, 0.5] |
| R15 ilk9 siluet | UYARI | 1.0 | <0.90 |
| R16 final temposu | UYARI | {"tail": {"n": 400, "median": 0.28067437078415425, "p25": 0.2453365158939721, "p75": 0.31937028952015906}, "kalibrasyon": 0.2921853674850866} | kalibrasyon medyanı <= 0.2921853674850866; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 3 | 3 |
| R19 FunSim ilk deneme | KALDI | 0.758 | >=0.9 |
| R20 süreden kayıp | GEÇTİ | 0.03 | <=0.05 |
| R21 tahta oranı | GEÇTİ | {"board": [5, 6], "shapes": [[6, 5], [5, 6], [6, 5]]} | |w-h|<=1 |
| R22 çeşitlilik | UYARI | 0.7142857142857143 | <0.90 |

Rastgele (1000): kazanma 24.5%, bomba 75.5%, donma 0.0%.
Unutan: Bomb, 13 hamle. Bilinçli: Won, 30 hamle; bombalar [{'p': 19, 'step': 3, 'earliest': 3, 'fuse_margin': 11}, {'p': 1, 'step': 5, 'earliest': 2, 'fuse_margin': 10}, {'p': 22, 'step': 7, 'earliest': 3, 'fuse_margin': 11}].
FunSim: ilk deneme kazanma 75.8%, near-miss 4.4%.
Oyuncu modeli: ZeydBot v1 · kazanma 100.0%; kalan/timer {'n': 400, 'median': 0.6838909236982618, 'p25': 0.6336808205980762, 'p75': 0.724003493294129}.
En yakın 3 (curve): S45 (SET40): 0.947; p21_06_c (aday): 0.898; S41 (SET40): 0.888
En yakın 3 (mask): S06 (SET40): 1.000; S20 (SET40): 1.000; S47 (SET40): 1.000
İlk 1–9 en yakın siluet: S06 (1.000).

siluet maskesi yok: Generator tam dikdörtgen üretir; R15/R22 çeşitlilik UYARI.
FunSim ilk deneme süreden kayıp: 0.03

Eleştirmen: **RED**
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R19 FunSim ilk deneme: ölçülen 0.758, hedef >=0.9; tarif `bombs[0].region` bottom → center.
