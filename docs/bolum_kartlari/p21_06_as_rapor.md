# p21_06_as · RED

tanitim · 60 sn · Bombayı fark et, yolunu aç ve rahatça bitir.
Anahtar: Tek bombanın kısa fitili

Eğri: █▇▇▇▇▆▅▅▅▄▃▃▃▂▁ · 8,7,7,7,7,6,5,5,5,4,3,3,3,2,1
15 adım; 0 boğaz, en uzun 1; göreli çığ 0.870.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 7 | >=3 |
| R04 boğaz uzunluğu | GEÇTİ | 1 | [0, 1] |
| R05 boğaz sayısı | GEÇTİ | 0 | >=0 |
| R06 göreli çığ | GEÇTİ | 0.8699999999999999 | >=0.35 |
| R07 ilk tuzak | GEÇTİ | 0.0 | <=0 |
| R08 tuzaklı adım | GEÇTİ | 0 | >=0 |
| R09 fitil/adım p4 | GEÇTİ | 6 | <15 |
| R10 fitil/erken çıkış p4 | GEÇTİ | {"fuse": 6, "k": 4} | F==k+2 |
| R11 tehdit adımı | GEÇTİ | 5 | <=5 |
| R12 bot_rastgele_bombasiz | KALDI | 0.014 | [0.15, 0.50] |
| R12 bot_bomba_unutan | GEÇTİ | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 400, "median": 0.6729382543038915, "p25": 0.6050660903787685, "p75": 0.7202483495036074} | n>=20 ve kalibre model |
| R14 kalan süre | GEÇTİ | {"source": "FunSim", "n": 650, "median": 0.4782608695652174, "p25": 0.34782608695652173, "p75": 0.6086956521739131} | [0.25, 0.5] |
| R15 ilk9 siluet | GEÇTİ | 0.75 | <0.90 |
| R16 final temposu | UYARI | {"tail": {"n": 400, "median": 0.34612150753907067, "p25": 0.2875047212323688, "p75": 0.3971657831888347}, "kalibrasyon": 0.2921853674850866} | kalibrasyon medyanı <= 0.2921853674850866; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 1 | 1 |
| R19 FunSim ilk deneme | GEÇTİ | 0.65 | >=0.65 |
| R20 çeşitlilik | GEÇTİ | 0.8333333333333334 | <0.90 |

Rastgele (1000): kazanma 1.4%, bomba 98.6%, donma 0.0%.
Unutan: Bomb, 6 hamle. Bilinçli: Won, 15 hamle; bombalar [{'p': 4, 'step': 5, 'earliest': 4, 'fuse_margin': 2}].
FunSim: ilk deneme kazanma 65.0%, near-miss 7.2%.
Oyuncu modeli: ZeydBot v1 · kazanma 100.0%; kalan/timer {'n': 400, 'median': 0.6729382543038915, 'p25': 0.6050660903787685, 'p75': 0.7202483495036074}.
En yakın 3 (curve): S46 (SET40): 0.941; S41 (SET40): 0.940; S52 (SET40): 0.938
En yakın 3 (mask): p21_06_bs (aday): 0.833; S43 (SET40): 0.750; S45 (SET40): 0.750
İlk 1–9 en yakın siluet: S43 (0.750).

Eleştirmen: **RED**
- Yerleşim: bombayı 3. sıradan 4. sıraya al; earliest [4, 5].
- R12 bot_rastgele_bombasiz: ölçülen 0.014, hedef [0.15, 0.50]; tarif `bombs[0].earliest` [3, 4] → [4, 5].
