# p21_06_c · RED

tanitim · 58 sn · Bombayı fark et, yolunu aç ve rahatça bitir.
Anahtar: Alt, üst ve ortadaki bombaların sayaçlarını tanı

Eğri: ▇▇███▇▇▇▆▆▆▅▅▅▅▅▅▅▅▅▅▄▄▄▃▃▃▂▂▁ · 12,12,13,13,13,12,12,12,11,11,10,9,8,8,8,8,8,8,9,8,8,7,7,6,5,5,4,3,2,1
30 adım; 0 boğaz, en uzun 1; göreli çığ 0.927.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 12 | >=3 |
| R04 boğaz uzunluğu | GEÇTİ | 1 | [0, 1] |
| R05 boğaz sayısı | GEÇTİ | 0 | >=0 |
| R06 göreli çığ | GEÇTİ | 0.9270282186948854 | >=0.35 |
| R07 ilk tuzak | UYARI | 0.0 | <=0 |
| R08 tuzaklı adım | GEÇTİ | 0 | >=0 |
| R09 fitil/adım p4 | GEÇTİ | 14 | <30 |
| R09 tanıtım fitil payı p4 | GEÇTİ | {"fuse": 14, "k": 2} | F>=k+4; üst sınır yok |
| R10 fitil/erken çıkış p4 | UYARI | {"fuse": 14, "k": 2} | F>=k+4 |
| R09 fitil/adım p15 | GEÇTİ | 17 | <30 |
| R09 tanıtım fitil payı p15 | GEÇTİ | {"fuse": 17, "k": 3} | F>=k+4; üst sınır yok |
| R10 fitil/erken çıkış p15 | UYARI | {"fuse": 17, "k": 3} | F>=k+4 |
| R09 fitil/adım p18 | GEÇTİ | 13 | <30 |
| R09 tanıtım fitil payı p18 | GEÇTİ | {"fuse": 13, "k": 3} | F>=k+4; üst sınır yok |
| R10 fitil/erken çıkış p18 | UYARI | {"fuse": 13, "k": 3} | F>=k+4 |
| R11 tehdit adımı | UYARI | 11 | <=5 |
| R12 bot_rastgele_bombasiz | UYARI | 0.007 | [0.15, 0.50] |
| R12 bot_bomba_unutan | UYARI | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 367, "median": 0.6933741851468644, "p25": 0.6408298169651053, "p75": 0.7365656690306344} | n>=20 ve kalibre model |
| R14 kalan süre | KALDI | {"source": "FunSim", "n": 559, "median": 0.5172413793103449, "p25": 0.39655172413793105, "p75": 0.6379310344827587} | [0.3, 0.5] |
| R15 ilk9 siluet | UYARI | 0.8333333333333334 | <0.90 |
| R16 final temposu | UYARI | {"tail": {"n": 367, "median": 0.2954376538162838, "p25": 0.2555375440843274, "p75": 0.33057322396329825}, "kalibrasyon": 0.2921853674850866} | kalibrasyon medyanı <= 0.2921853674850866; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 3 | 3 |
| R19 FunSim ilk deneme | KALDI | 0.559 | >=0.9 |
| R20 süreden kayıp | GEÇTİ | 0.01 | <=0.05 |
| R21 tahta oranı | GEÇTİ | {"board": [6, 5], "shapes": [[6, 5], [5, 6], [6, 5]]} | |w-h|<=1 |
| R22 çeşitlilik | UYARI | 1.0 | <0.90 |

Rastgele (1000): kazanma 0.7%, bomba 99.3%, donma 0.0%.
Unutan: Bomb, 13 hamle. Bilinçli: Won, 30 hamle; bombalar [{'p': 18, 'step': 3, 'earliest': 3, 'fuse_margin': 11}, {'p': 4, 'step': 5, 'earliest': 2, 'fuse_margin': 10}, {'p': 15, 'step': 8, 'earliest': 3, 'fuse_margin': 10}].
FunSim: ilk deneme kazanma 55.9%, near-miss 1.9%.
Oyuncu modeli: ZeydBot v1 · kazanma 91.8%; kalan/timer {'n': 367, 'median': 0.6933741851468644, 'p25': 0.6408298169651053, 'p75': 0.7365656690306344}.
En yakın 3 (curve): S20 (SET40): 0.939; S41 (SET40): 0.910; S52 (SET40): 0.905
En yakın 3 (mask): p21_06_a (aday): 1.000; S44 (SET40): 0.833; S46 (SET40): 0.833
İlk 1–9 en yakın siluet: S44 (0.833).

siluet maskesi yok: Generator tam dikdörtgen üretir; R15/R22 çeşitlilik UYARI.
FunSim ilk deneme süreden kayıp: 0.01

Eleştirmen: **RED**
- Öneriyi yeni adaylarda yeniden ölç.
- R14 kalan süre: ölçülen {'source': 'FunSim', 'n': 559, 'median': 0.5172413793103449, 'p25': 0.39655172413793105, 'p75': 0.6379310344827587}, hedef [0.3, 0.5]; tarif `timer` 58 → 47.
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R19 FunSim ilk deneme: ölçülen 0.559, hedef >=0.9; tarif `bombs[0].region` bottom → center.
