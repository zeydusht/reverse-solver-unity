# p21_06_t2_c · RED

tanitim · 47 sn · Bombayı fark et, yolunu aç ve rahatça bitir.
Anahtar: Alt, üst ve ortadaki bombaların sayaçlarını tanı

Eğri: ███▇▇▇▆▆▅▅▅▅▅▄▄▃▃▃▃▃▃▃▃▃▂▂▂▂▂▁ · 14,14,14,13,13,12,11,10,9,9,8,8,8,7,6,5,5,5,5,5,5,5,4,4,3,3,2,2,2,1
30 adım; 0 boğaz, en uzun 1; göreli çığ 0.655.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 14 | >=3 |
| R04 boğaz uzunluğu | GEÇTİ | 1 | [0, 1] |
| R05 boğaz sayısı | GEÇTİ | 0 | >=0 |
| R06 göreli çığ | GEÇTİ | 0.6548500881834216 | >=0.35 |
| R07 ilk tuzak | UYARI | 0.0 | <=0 |
| R08 tuzaklı adım | GEÇTİ | 0 | >=0 |
| R09 fitil/adım p1 | GEÇTİ | 14 | <30 |
| R09 tanıtım fitil payı p1 | GEÇTİ | {"fuse": 14, "k": 2} | F>=k+4; üst sınır yok |
| R10 fitil/erken çıkış p1 | UYARI | {"fuse": 14, "k": 2} | F>=k+4 |
| R09 fitil/adım p15 | GEÇTİ | 13 | <30 |
| R09 tanıtım fitil payı p15 | GEÇTİ | {"fuse": 13, "k": 3} | F>=k+4; üst sınır yok |
| R10 fitil/erken çıkış p15 | UYARI | {"fuse": 13, "k": 3} | F>=k+4 |
| R09 fitil/adım p16 | GEÇTİ | 16 | <30 |
| R09 tanıtım fitil payı p16 | GEÇTİ | {"fuse": 16, "k": 2} | F>=k+4; üst sınır yok |
| R10 fitil/erken çıkış p16 | UYARI | {"fuse": 16, "k": 2} | F>=k+4 |
| R11 tehdit adımı | UYARI | 11 | <=5 |
| R12 bot_rastgele_bombasiz | UYARI | 0.044 | [0.15, 0.50] |
| R12 bot_bomba_unutan | UYARI | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 367, "median": 0.6209178851596214, "p25": 0.558434066094515, "p75": 0.6656568457809453} | n>=20 ve kalibre model |
| R14 kalan süre | GEÇTİ | {"source": "FunSim", "n": 837, "median": 0.425531914893617, "p25": 0.2765957446808511, "p75": 0.5531914893617021} | [0.3, 0.5] |
| R15 ilk9 siluet | UYARI | 0.8333333333333334 | <0.90 |
| R16 final temposu | UYARI | {"tail": {"n": 367, "median": 0.28337980527958206, "p25": 0.24551169750581092, "p75": 0.3196406724950748}, "kalibrasyon": 0.2921853674850866} | kalibrasyon medyanı <= 0.2921853674850866; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 3 | 3 |
| R19 FunSim ilk deneme | KALDI | 0.837 | >=0.9 |
| R20 süreden kayıp | KALDI | 0.081 | <=0.05 |
| R21 tahta oranı | GEÇTİ | {"board": [6, 5], "shapes": [[6, 5], [5, 6], [6, 5]]} | |w-h|<=1 |
| R22 çeşitlilik | UYARI | 1.0 | <0.90 |

Rastgele (1000): kazanma 4.4%, bomba 95.6%, donma 0.0%.
Unutan: Bomb, 13 hamle. Bilinçli: Won, 30 hamle; bombalar [{'p': 16, 'step': 2, 'earliest': 2, 'fuse_margin': 15}, {'p': 15, 'step': 3, 'earliest': 3, 'fuse_margin': 11}, {'p': 1, 'step': 5, 'earliest': 2, 'fuse_margin': 10}].
FunSim: ilk deneme kazanma 83.7%, near-miss 9.5%.
Oyuncu modeli: ZeydBot v1 · kazanma 91.8%; kalan/timer {'n': 367, 'median': 0.6209178851596214, 'p25': 0.558434066094515, 'p75': 0.6656568457809453}.
En yakın 3 (curve): S42 (SET40): 0.929; S24 (SET40): 0.926; S43 (SET40): 0.913
En yakın 3 (mask): p21_06_t2_a (aday): 1.000; S44 (SET40): 0.833; S46 (SET40): 0.833
İlk 1–9 en yakın siluet: S44 (0.833).

siluet maskesi yok: Generator tam dikdörtgen üretir; R15/R22 çeşitlilik UYARI.
FunSim ilk deneme süreden kayıp: 0.081

Eleştirmen: **RED**
- Öneriyi yeni adaylarda yeniden ölç.
- R20 süreden kayıp: ölçülen 0.081, hedef <=0.05; tarif `timer` 47 → 65.
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R19 FunSim ilk deneme: ölçülen 0.837, hedef >=0.9; tarif `bombs[0].region` center → center.
