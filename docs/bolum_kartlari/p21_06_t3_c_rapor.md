# p21_06_t3_c · RED

tanitim · 65 sn · Bombayı fark et, yolunu aç ve rahatça bitir.
Anahtar: Alt, üst ve ortadaki bombaların sayaçlarını tanı

Eğri: ███▇█▇▇▇▆▇▇▆▆▇▇▇▇▆▆▆▆▆▆▆▅▅▅▅▅▅▅▅▅▅▅▄▄▄▃▃▃▃▂▂▂▂▁▁▁ · 15,15,15,14,15,14,14,13,12,13,13,12,12,13,13,13,13,12,12,12,12,11,11,11,10,10,10,10,9,10,10,10,9,9,9,8,8,7,6,6,5,5,4,4,4,3,2,2,1
49 adım; 0 boğaz, en uzun 1; göreli çığ 0.677.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 15 | >=3 |
| R04 boğaz uzunluğu | GEÇTİ | 1 | [0, 1] |
| R05 boğaz sayısı | GEÇTİ | 0 | >=0 |
| R06 göreli çığ | GEÇTİ | 0.6767279017279016 | >=0.35 |
| R07 ilk tuzak | UYARI | 0.0 | <=0 |
| R08 tuzaklı adım | GEÇTİ | 0 | >=0 |
| R09 fitil/adım p1 | GEÇTİ | 14 | <49 |
| R09 tanıtım fitil payı p1 | UYARI | {"fuse": 14, "k": -1} | F>=k+4; üst sınır yok |
| R10 fitil/erken çıkış p1 | UYARI | {"fuse": 14, "k": -1} | F>=k+4 |
| R09 fitil/adım p7 | GEÇTİ | 16 | <49 |
| R09 tanıtım fitil payı p7 | UYARI | {"fuse": 16, "k": -1} | F>=k+4; üst sınır yok |
| R10 fitil/erken çıkış p7 | UYARI | {"fuse": 16, "k": -1} | F>=k+4 |
| R09 fitil/adım p32 | GEÇTİ | 13 | <49 |
| R09 tanıtım fitil payı p32 | UYARI | {"fuse": 13, "k": -1} | F>=k+4; üst sınır yok |
| R10 fitil/erken çıkış p32 | UYARI | {"fuse": 13, "k": -1} | F>=k+4 |
| R11 tehdit adımı | UYARI | 11 | <=5 |
| R12 bot_rastgele_bombasiz | UYARI | 0.008 | [0.15, 0.50] |
| R12 bot_bomba_unutan | UYARI | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 232, "median": 0.5053243794755842, "p25": 0.4459865677158774, "p75": 0.5822467547103674} | n>=20 ve kalibre model |
| R14 kalan süre | KALDI | {"source": "FunSim", "n": 601, "median": 0.2923076923076923, "p25": 0.16923076923076924, "p75": 0.4307692307692308} | [0.3, 0.5] |
| R15 ilk9 siluet | UYARI | 0.6122448979591837 | <0.90 |
| R16 final temposu | UYARI | {"tail": {"n": 232, "median": 0.27328880747390616, "p25": 0.24782814751351687, "p75": 0.3008141937918737}, "kalibrasyon": 0.2921853674850866} | kalibrasyon medyanı <= 0.2921853674850866; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 3 | 3 |
| R19 FunSim ilk deneme | KALDI | 0.601 | >=0.9 |
| R20 süreden kayıp | KALDI | 0.225 | <=0.05 |
| R21 tahta oranı | GEÇTİ | {"board": [7, 7], "shapes": [[5, 5], [6, 6], [7, 7]]} | |w-h|<=1 |
| R22 çeşitlilik | UYARI | 0.7346938775510204 | <0.90 |

Rastgele (1000): kazanma 0.8%, bomba 99.2%, donma 0.0%.
Unutan: Bomb, 13 hamle. Bilinçli: Won, 49 hamle; bombalar [{'p': 32, 'step': 3, 'earliest': -1, 'fuse_margin': 11}, {'p': 1, 'step': 5, 'earliest': -1, 'fuse_margin': 10}, {'p': 7, 'step': 6, 'earliest': -1, 'fuse_margin': 11}].
FunSim: ilk deneme kazanma 60.1%, near-miss 19.3%.
Oyuncu modeli: ZeydBot v1 · kazanma 58.0%; kalan/timer {'n': 232, 'median': 0.5053243794755842, 'p25': 0.4459865677158774, 'p75': 0.5822467547103674}.
En yakın 3 (curve): S20 (SET40): 0.956; S41 (SET40): 0.955; p21_06_t3_a (aday): 0.940
En yakın 3 (mask): S21 (SET40): 0.857; S24 (SET40): 0.857; S34 (SET40): 0.857
İlk 1–9 en yakın siluet: S06 (0.612).

siluet maskesi yok: Generator tam dikdörtgen üretir; R15/R22 çeşitlilik UYARI.
FunSim ilk deneme süreden kayıp: 0.225

Eleştirmen: **RED**
- Öneriyi yeni adaylarda yeniden ölç.
- R20 süreden kayıp: ölçülen 0.225, hedef <=0.05; tarif `timer` 65 → 85.
- Öneriyi yeni adaylarda yeniden ölç.
- R14 kalan süre: ölçülen {'source': 'FunSim', 'n': 601, 'median': 0.2923076923076923, 'p25': 0.16923076923076924, 'p75': 0.4307692307692308}, hedef [0.3, 0.5]; tarif `timer` 65 → 77.
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R19 FunSim ilk deneme: ölçülen 0.601, hedef >=0.9; tarif `bombs[0].region` center → center.
