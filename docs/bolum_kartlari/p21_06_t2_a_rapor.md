# p21_06_t2_a · RED

tanitim · 47 sn · Bombayı fark et, yolunu aç ve rahatça bitir.
Anahtar: Alt, üst ve ortadaki bombaların sayaçlarını tanı

Eğri: ██▇▇▆▆▆▆▆▆▅▅▅▆▆▆▆▅▅▄▃▃▃▃▂▂▂▂▂▁ · 12,12,11,11,10,9,9,9,9,9,8,8,8,10,10,9,9,8,7,6,5,4,4,4,3,2,2,2,2,1
30 adım; 0 boğaz, en uzun 1; göreli çığ 0.620.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 11 | >=3 |
| R04 boğaz uzunluğu | GEÇTİ | 1 | [0, 1] |
| R05 boğaz sayısı | GEÇTİ | 0 | >=0 |
| R06 göreli çığ | GEÇTİ | 0.6202821869488536 | >=0.35 |
| R07 ilk tuzak | UYARI | 0.0 | <=0 |
| R08 tuzaklı adım | GEÇTİ | 0 | >=0 |
| R09 fitil/adım p3 | GEÇTİ | 15 | <30 |
| R09 tanıtım fitil payı p3 | GEÇTİ | {"fuse": 15, "k": 3} | F>=k+4; üst sınır yok |
| R10 fitil/erken çıkış p3 | UYARI | {"fuse": 15, "k": 3} | F>=k+4 |
| R09 fitil/adım p10 | GEÇTİ | 12 | <30 |
| R09 tanıtım fitil payı p10 | GEÇTİ | {"fuse": 12, "k": 2} | F>=k+4; üst sınır yok |
| R10 fitil/erken çıkış p10 | UYARI | {"fuse": 12, "k": 2} | F>=k+4 |
| R09 fitil/adım p16 | GEÇTİ | 16 | <30 |
| R09 tanıtım fitil payı p16 | GEÇTİ | {"fuse": 16, "k": 2} | F>=k+4; üst sınır yok |
| R10 fitil/erken çıkış p16 | UYARI | {"fuse": 16, "k": 2} | F>=k+4 |
| R11 tehdit adımı | UYARI | 10 | <=5 |
| R12 bot_rastgele_bombasiz | UYARI | 0.042 | [0.15, 0.50] |
| R12 bot_bomba_unutan | UYARI | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 366, "median": 0.6190504518973727, "p25": 0.563684361929451, "p75": 0.6768756745470151} | n>=20 ve kalibre model |
| R14 kalan süre | GEÇTİ | {"source": "FunSim", "n": 756, "median": 0.425531914893617, "p25": 0.2765957446808511, "p75": 0.5531914893617021} | [0.3, 0.5] |
| R15 ilk9 siluet | UYARI | 0.8333333333333334 | <0.90 |
| R16 final temposu | UYARI | {"tail": {"n": 366, "median": 0.2780297954711176, "p25": 0.24539531875114745, "p75": 0.32189103158850396}, "kalibrasyon": 0.2921853674850866} | kalibrasyon medyanı <= 0.2921853674850866; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 3 | 3 |
| R19 FunSim ilk deneme | KALDI | 0.756 | >=0.9 |
| R20 süreden kayıp | KALDI | 0.052 | <=0.05 |
| R21 tahta oranı | GEÇTİ | {"board": [6, 5], "shapes": [[6, 5], [5, 6], [6, 5]]} | |w-h|<=1 |
| R22 çeşitlilik | UYARI | 1.0 | <0.90 |

Rastgele (1000): kazanma 4.2%, bomba 95.8%, donma 0.0%.
Unutan: Bomb, 12 hamle. Bilinçli: Won, 30 hamle; bombalar [{'p': 10, 'step': 2, 'earliest': 2, 'fuse_margin': 11}, {'p': 3, 'step': 8, 'earliest': 3, 'fuse_margin': 8}, {'p': 16, 'step': 10, 'earliest': 2, 'fuse_margin': 7}].
FunSim: ilk deneme kazanma 75.6%, near-miss 6.6%.
Oyuncu modeli: ZeydBot v1 · kazanma 91.5%; kalan/timer {'n': 366, 'median': 0.6190504518973727, 'p25': 0.563684361929451, 'p75': 0.6768756745470151}.
En yakın 3 (curve): S52 (SET40): 0.918; S41 (SET40): 0.916; S20 (SET40): 0.902
En yakın 3 (mask): p21_06_t2_c (aday): 1.000; S44 (SET40): 0.833; S46 (SET40): 0.833
İlk 1–9 en yakın siluet: S44 (0.833).

siluet maskesi yok: Generator tam dikdörtgen üretir; R15/R22 çeşitlilik UYARI.
FunSim ilk deneme süreden kayıp: 0.052

Eleştirmen: **RED**
- Öneriyi yeni adaylarda yeniden ölç.
- R20 süreden kayıp: ölçülen 0.052, hedef <=0.05; tarif `timer` 47 → 65.
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R19 FunSim ilk deneme: ölçülen 0.756, hedef >=0.9; tarif `bombs[0].region` center → center.
