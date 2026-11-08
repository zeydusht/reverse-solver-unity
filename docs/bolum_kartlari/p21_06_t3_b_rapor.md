# p21_06_t3_b · RED

tanitim · 65 sn · Bombayı fark et, yolunu aç ve rahatça bitir.
Anahtar: Alt, üst ve ortadaki bombaların sayaçlarını tanı

Eğri: ██▇▇▇▇▆▆▅▅▅▅▅▄▄▄▄▄▆▅▄▄▃▃▄▃▃▃▃▃▃▃▃▂▂▁ · 9,9,8,8,8,8,7,7,6,6,6,6,6,5,5,5,5,5,7,6,5,4,3,3,4,3,3,3,3,3,3,3,3,2,2,1
36 adım; 0 boğaz, en uzun 1; göreli çığ 0.566.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 8 | >=3 |
| R04 boğaz uzunluğu | GEÇTİ | 1 | [0, 1] |
| R05 boğaz sayısı | GEÇTİ | 0 | >=0 |
| R06 göreli çığ | GEÇTİ | 0.5660271546635183 | >=0.35 |
| R07 ilk tuzak | UYARI | 0.0 | <=0 |
| R08 tuzaklı adım | GEÇTİ | 0 | >=0 |
| R09 fitil/adım p1 | GEÇTİ | 14 | <36 |
| R09 tanıtım fitil payı p1 | UYARI | {"fuse": 14, "k": -1} | F>=k+4; üst sınır yok |
| R10 fitil/erken çıkış p1 | UYARI | {"fuse": 14, "k": -1} | F>=k+4 |
| R09 fitil/adım p10 | GEÇTİ | 12 | <36 |
| R09 tanıtım fitil payı p10 | UYARI | {"fuse": 12, "k": -1} | F>=k+4; üst sınır yok |
| R10 fitil/erken çıkış p10 | UYARI | {"fuse": 12, "k": -1} | F>=k+4 |
| R09 fitil/adım p19 | GEÇTİ | 16 | <36 |
| R09 tanıtım fitil payı p19 | UYARI | {"fuse": 16, "k": -1} | F>=k+4; üst sınır yok |
| R10 fitil/erken çıkış p19 | UYARI | {"fuse": 16, "k": -1} | F>=k+4 |
| R11 tehdit adımı | UYARI | 10 | <=5 |
| R12 bot_rastgele_bombasiz | UYARI | 0.164 | [0.15, 0.50] |
| R12 bot_bomba_unutan | UYARI | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 400, "median": 0.6315454943752699, "p25": 0.5735590036965272, "p75": 0.6803123960605231} | n>=20 ve kalibre model |
| R14 kalan süre | GEÇTİ | {"source": "FunSim", "n": 878, "median": 0.4307692307692308, "p25": 0.2923076923076923, "p75": 0.5538461538461539} | [0.3, 0.5] |
| R15 ilk9 siluet | UYARI | 0.8333333333333334 | <0.90 |
| R16 final temposu | UYARI | {"tail": {"n": 400, "median": 0.29089980249968983, "p25": 0.25774298460484385, "p75": 0.3300992097778679}, "kalibrasyon": 0.2921853674850866} | kalibrasyon medyanı <= 0.2921853674850866; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 3 | 3 |
| R19 FunSim ilk deneme | KALDI | 0.878 | >=0.9 |
| R20 süreden kayıp | KALDI | 0.072 | <=0.05 |
| R21 tahta oranı | GEÇTİ | {"board": [6, 6], "shapes": [[5, 5], [6, 6], [7, 7]]} | |w-h|<=1 |
| R22 çeşitlilik | UYARI | 0.7346938775510204 | <0.90 |

Rastgele (1000): kazanma 16.4%, bomba 83.6%, donma 0.0%.
Unutan: Bomb, 12 hamle. Bilinçli: Won, 36 hamle; bombalar [{'p': 10, 'step': 2, 'earliest': -1, 'fuse_margin': 11}, {'p': 1, 'step': 4, 'earliest': -1, 'fuse_margin': 11}, {'p': 19, 'step': 6, 'earliest': -1, 'fuse_margin': 11}].
FunSim: ilk deneme kazanma 87.8%, near-miss 8.5%.
Oyuncu modeli: ZeydBot v1 · kazanma 100.0%; kalan/timer {'n': 400, 'median': 0.6315454943752699, 'p25': 0.5735590036965272, 'p75': 0.6803123960605231}.
En yakın 3 (curve): S46 (SET40): 0.925; S42 (SET40): 0.912; S52 (SET40): 0.901
En yakın 3 (mask): S10 (SET40): 0.973; S13 (SET40): 0.973; S17 (SET40): 0.973
İlk 1–9 en yakın siluet: S06 (0.833).

siluet maskesi yok: Generator tam dikdörtgen üretir; R15/R22 çeşitlilik UYARI.
FunSim ilk deneme süreden kayıp: 0.072

Eleştirmen: **RED**
- Öneriyi yeni adaylarda yeniden ölç.
- R20 süreden kayıp: ölçülen 0.072, hedef <=0.05; tarif `timer` 65 → 85.
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R19 FunSim ilk deneme: ölçülen 0.878, hedef >=0.9; tarif `bombs[0].region` center → center.
