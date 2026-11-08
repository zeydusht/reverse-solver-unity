# p21_06_t3_a · RED

tanitim · 19 sn · Bombayı fark et, yolunu aç ve rahatça bitir.
Anahtar: Tek bombanın kısa fitili

Eğri: ███▆▆▆▆▅▅▄▃▃▃▃▂ · 6,6,6,5,5,5,5,4,4,3,2,2,2,2,1
15 adım; 0 boğaz, en uzun 1; göreli çığ 0.713.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 6 | >=3 |
| R04 boğaz uzunluğu | GEÇTİ | 1 | [0, 1] |
| R05 boğaz sayısı | GEÇTİ | 0 | >=0 |
| R06 göreli çığ | GEÇTİ | 0.7133333333333333 | >=0.35 |
| R07 ilk tuzak | GEÇTİ | 0.0 | <=0 |
| R08 tuzaklı adım | GEÇTİ | 0 | >=0 |
| R09 fitil/adım p7 | GEÇTİ | 8 | <15 |
| R10 fitil/erken çıkış p7 | GEÇTİ | {"fuse": 8, "k": 6} | F==k+2 |
| R11 tehdit adımı | KALDI | 7 | <=5 |
| R12 bot_rastgele_bombasiz | KALDI | 0.015 | [0.15, 0.50] |
| R12 bot_bomba_unutan | GEÇTİ | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 400, "median": 0.5896758036356743, "p25": 0.5119762120562699, "p75": 0.6501466991299156} | n>=20 ve kalibre model |
| R14 kalan süre | GEÇTİ | {"source": "FunSim", "n": 688, "median": 0.3684210526315789, "p25": 0.21052631578947367, "p75": 0.5263157894736842} | [0.25, 0.5] |
| R15 ilk9 siluet | GEÇTİ | 0.75 | <0.90 |
| R16 final temposu | UYARI | {"tail": {"n": 400, "median": 0.3312815418287036, "p25": 0.272940006179869, "p75": 0.3820977091251661}, "kalibrasyon": 0.2921853674850866} | kalibrasyon medyanı <= 0.2921853674850866; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 1 | 1 |
| R19 FunSim ilk deneme | GEÇTİ | 0.688 | >=0.65 |
| R20 çeşitlilik | GEÇTİ | 0.8333333333333334 | <0.90 |

Rastgele (1000): kazanma 1.5%, bomba 98.5%, donma 0.0%.
Unutan: Bomb, 8 hamle. Bilinçli: Won, 15 hamle; bombalar [{'p': 7, 'step': 7, 'earliest': 6, 'fuse_margin': 2}].
FunSim: ilk deneme kazanma 68.8%, near-miss 13.7%.
Oyuncu modeli: ZeydBot v1 · kazanma 100.0%; kalan/timer {'n': 400, 'median': 0.5896758036356743, 'p25': 0.5119762120562699, 'p75': 0.6501466991299156}.
En yakın 3 (curve): S41 (SET40): 0.942; S20 (SET40): 0.933; S50 (SET40): 0.913
En yakın 3 (mask): p21_06_t3_b (aday): 0.833; S43 (SET40): 0.750; S45 (SET40): 0.750
İlk 1–9 en yakın siluet: S43 (0.750).

Eleştirmen: **RED**
- Yerleşim: bombayı 5. sıradan 4. sıraya al; earliest [4, 5].
- R11 tehdit adımı: ölçülen 7, hedef <=5; tarif `bombs[0].earliest` [5, 6] → [4, 5].
- Yerleşim: bombayı 5. sıradan 6. sıraya al; earliest [6, 7].
- R12 bot_rastgele_bombasiz: ölçülen 0.015, hedef [0.15, 0.50]; tarif `bombs[0].earliest` [5, 6] → [6, 7].
