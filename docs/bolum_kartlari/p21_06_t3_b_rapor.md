# p21_06_t3_b · RED

tanitim · 19 sn · Bombayı fark et, yolunu aç ve rahatça bitir.
Anahtar: Tek bombanın kısa fitili

Eğri: █▇▆▇▇▆▅▅▅▅▄▃▃▃▂▂▂▁ · 10,9,8,9,9,8,7,6,6,6,5,4,3,3,2,2,2,1
18 adım; 0 boğaz, en uzun 1; göreli çığ 0.711.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 8 | >=3 |
| R04 boğaz uzunluğu | GEÇTİ | 1 | [0, 1] |
| R05 boğaz sayısı | GEÇTİ | 0 | >=0 |
| R06 göreli çığ | GEÇTİ | 0.7111111111111111 | >=0.35 |
| R07 ilk tuzak | GEÇTİ | 0.0 | <=0 |
| R08 tuzaklı adım | GEÇTİ | 0 | >=0 |
| R09 fitil/adım p13 | GEÇTİ | 7 | <18 |
| R10 fitil/erken çıkış p13 | GEÇTİ | {"fuse": 7, "k": 5} | F==k+2 |
| R11 tehdit adımı | GEÇTİ | 5 | <=5 |
| R12 bot_rastgele_bombasiz | KALDI | 0.007 | [0.15, 0.50] |
| R12 bot_bomba_unutan | GEÇTİ | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 400, "median": 0.5228682890768463, "p25": 0.4307634579152483, "p75": 0.5914326819531545} | n>=20 ve kalibre model |
| R14 kalan süre | GEÇTİ | {"source": "FunSim", "n": 77, "median": 0.3157894736842105, "p25": 0.21052631578947367, "p75": 0.47368421052631576} | [0.25, 0.5] |
| R15 ilk9 siluet | GEÇTİ | 0.6521739130434783 | <0.90 |
| R16 final temposu | UYARI | {"tail": {"n": 400, "median": 0.3398898006232549, "p25": 0.28821249002491334, "p75": 0.39279242579007634}, "kalibrasyon": 0.2921853674850866} | kalibrasyon medyanı <= 0.2921853674850866; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 1 | 1 |
| R19 FunSim ilk deneme | KALDI | 0.077 | >=0.65 |
| R20 çeşitlilik | GEÇTİ | 0.8333333333333334 | <0.90 |

Rastgele (1000): kazanma 0.7%, bomba 99.3%, donma 0.0%.
Unutan: Bomb, 7 hamle. Bilinçli: Won, 18 hamle; bombalar [{'p': 13, 'step': 5, 'earliest': 5, 'fuse_margin': 3}].
FunSim: ilk deneme kazanma 7.7%, near-miss 5.7%.
Oyuncu modeli: ZeydBot v1 · kazanma 100.0%; kalan/timer {'n': 400, 'median': 0.5228682890768463, 'p25': 0.4307634579152483, 'p75': 0.5914326819531545}.
En yakın 3 (curve): S46 (SET40): 0.943; S42 (SET40): 0.943; p21_06_t3_c (aday): 0.933
En yakın 3 (mask): p21_06_t3_a (aday): 0.833; S43 (SET40): 0.652; S45 (SET40): 0.652
İlk 1–9 en yakın siluet: S43 (0.652).

Eleştirmen: **RED**
- Yerleşim: bombayı 5. sıradan 6. sıraya al; earliest [6, 7].
- R12 bot_rastgele_bombasiz: ölçülen 0.007, hedef [0.15, 0.50]; tarif `bombs[0].earliest` [5, 6] → [6, 7].
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R19 FunSim ilk deneme: ölçülen 0.077, hedef >=0.65; tarif `bombs[0].region` center → center.
