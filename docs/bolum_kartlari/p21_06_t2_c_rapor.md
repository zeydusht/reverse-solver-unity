# p21_06_t2_c · RED

tanitim · 19 sn · Bombayı fark et, yolunu aç ve rahatça bitir.
Anahtar: Tek bombanın kısa fitili

Eğri: ████▇▇▆▆▅▄▄▄▄▃▂▂▂▁ · 9,9,9,9,8,8,7,7,6,5,4,4,4,3,2,2,2,1
18 adım; 0 boğaz, en uzun 1; göreli çığ 0.739.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 9 | >=3 |
| R04 boğaz uzunluğu | GEÇTİ | 1 | [0, 1] |
| R05 boğaz sayısı | GEÇTİ | 0 | >=0 |
| R06 göreli çığ | GEÇTİ | 0.7388888888888889 | >=0.35 |
| R07 ilk tuzak | GEÇTİ | 0.0 | <=0 |
| R08 tuzaklı adım | GEÇTİ | 0 | >=0 |
| R09 fitil/adım p7 | GEÇTİ | 6 | <18 |
| R10 fitil/erken çıkış p7 | GEÇTİ | {"fuse": 6, "k": 4} | F==k+2 |
| R11 tehdit adımı | GEÇTİ | 4 | <=5 |
| R12 bot_rastgele_bombasiz | KALDI | 0.008 | [0.15, 0.50] |
| R12 bot_bomba_unutan | GEÇTİ | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 400, "median": 0.51897810414411, "p25": 0.4270728637690915, "p75": 0.589506502883869} | n>=20 ve kalibre model |
| R14 kalan süre | GEÇTİ | {"source": "FunSim", "n": 198, "median": 0.3157894736842105, "p25": 0.21052631578947367, "p75": 0.47368421052631576} | [0.25, 0.5] |
| R15 ilk9 siluet | GEÇTİ | 0.5454545454545454 | <0.90 |
| R16 final temposu | UYARI | {"tail": {"n": 400, "median": 0.33072282135481523, "p25": 0.2790632783289231, "p75": 0.38608467054783646}, "kalibrasyon": 0.2921853674850866} | kalibrasyon medyanı <= 0.2921853674850866; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 1 | 1 |
| R19 FunSim ilk deneme | KALDI | 0.198 | >=0.65 |
| R20 çeşitlilik | GEÇTİ | 0.375 | <0.90 |

Rastgele (1000): kazanma 0.8%, bomba 99.2%, donma 0.0%.
Unutan: Bomb, 6 hamle. Bilinçli: Won, 18 hamle; bombalar [{'p': 7, 'step': 4, 'earliest': 4, 'fuse_margin': 3}].
FunSim: ilk deneme kazanma 19.8%, near-miss 8.2%.
Oyuncu modeli: ZeydBot v1 · kazanma 100.0%; kalan/timer {'n': 400, 'median': 0.51897810414411, 'p25': 0.4270728637690915, 'p75': 0.589506502883869}.
En yakın 3 (curve): p21_06_t2_a (aday): 0.947; S46 (SET40): 0.940; S50 (SET40): 0.936
En yakın 3 (mask): S42 (SET40): 0.545; S44 (SET40): 0.536; S46 (SET40): 0.536
İlk 1–9 en yakın siluet: S42 (0.545).

Eleştirmen: **RED**
- Yerleşim: bombayı 4. sıradan 5. sıraya al; earliest [5, 6].
- R12 bot_rastgele_bombasiz: ölçülen 0.008, hedef [0.15, 0.50]; tarif `bombs[0].earliest` [4, 5] → [5, 6].
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R19 FunSim ilk deneme: ölçülen 0.198, hedef >=0.65; tarif `bombs[0].region` center → center.
