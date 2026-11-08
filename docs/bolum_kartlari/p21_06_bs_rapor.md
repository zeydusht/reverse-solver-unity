# p21_06_bs · RED

tanitim · 60 sn · Bombayı fark et, yolunu aç ve rahatça bitir.
Anahtar: Tek bombanın kısa fitili

Eğri: █▇▇▇▇███▇▇▆▆▅▅▄▃▃▂ · 7,6,6,6,6,7,7,7,6,6,5,5,4,4,3,2,2,1
18 adım; 0 boğaz, en uzun 1; göreli çığ 0.814.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 6 | >=3 |
| R04 boğaz uzunluğu | GEÇTİ | 1 | [0, 1] |
| R05 boğaz sayısı | GEÇTİ | 0 | >=0 |
| R06 göreli çığ | GEÇTİ | 0.8138888888888888 | >=0.35 |
| R07 ilk tuzak | GEÇTİ | 0.0 | <=0 |
| R08 tuzaklı adım | GEÇTİ | 0 | >=0 |
| R09 fitil/adım p4 | GEÇTİ | 6 | <18 |
| R10 fitil/erken çıkış p4 | GEÇTİ | {"fuse": 6, "k": 4} | F==k+2 |
| R11 tehdit adımı | GEÇTİ | 5 | <=5 |
| R12 bot_rastgele_bombasiz | KALDI | 0.024 | [0.15, 0.50] |
| R12 bot_bomba_unutan | GEÇTİ | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 400, "median": 0.6776087028070044, "p25": 0.6150495195995977, "p75": 0.7221374057346124} | n>=20 ve kalibre model |
| R14 kalan süre | GEÇTİ | {"source": "FunSim", "n": 649, "median": 0.5, "p25": 0.36666666666666664, "p75": 0.6} | [0.25, 0.5] |
| R15 ilk9 siluet | GEÇTİ | 0.6521739130434783 | <0.90 |
| R16 final temposu | UYARI | {"tail": {"n": 400, "median": 0.32752175115508175, "p25": 0.2754963600438677, "p75": 0.37969362333112394}, "kalibrasyon": 0.2921853674850866} | kalibrasyon medyanı <= 0.2921853674850866; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 1 | 1 |
| R19 FunSim ilk deneme | KALDI | 0.649 | >=0.65 |
| R20 çeşitlilik | GEÇTİ | 0.8333333333333334 | <0.90 |

Rastgele (1000): kazanma 2.4%, bomba 97.6%, donma 0.0%.
Unutan: Bomb, 6 hamle. Bilinçli: Won, 18 hamle; bombalar [{'p': 4, 'step': 5, 'earliest': 4, 'fuse_margin': 2}].
FunSim: ilk deneme kazanma 64.9%, near-miss 4.1%.
Oyuncu modeli: ZeydBot v1 · kazanma 100.0%; kalan/timer {'n': 400, 'median': 0.6776087028070044, 'p25': 0.6150495195995977, 'p75': 0.7221374057346124}.
En yakın 3 (curve): S45 (SET40): 0.907; S41 (SET40): 0.899; S20 (SET40): 0.874
En yakın 3 (mask): p21_06_as (aday): 0.833; S43 (SET40): 0.652; S45 (SET40): 0.652
İlk 1–9 en yakın siluet: S43 (0.652).

Eleştirmen: **RED**
- Yerleşim: bombayı 3. sıradan 4. sıraya al; earliest [4, 5].
- R12 bot_rastgele_bombasiz: ölçülen 0.024, hedef [0.15, 0.50]; tarif `bombs[0].earliest` [3, 4] → [4, 5].
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R19 FunSim ilk deneme: ölçülen 0.649, hedef >=0.65; tarif `bombs[0].region` any → center.
