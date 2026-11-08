# p21_06_c · RED

tanitim · 60 sn · Bombayı fark et, yolunu aç ve rahatça bitir.
Anahtar: Tek bombanın kısa fitili

Eğri: ███▇▇▆▅▅▅▄▄▄▄▄▄▄▃▂ · 7,7,7,6,6,5,4,4,4,3,3,3,3,3,3,3,2,1
18 adım; 0 boğaz, en uzun 1; göreli çığ 0.808.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 7 | >=3 |
| R04 boğaz uzunluğu | GEÇTİ | 1 | [0, 1] |
| R05 boğaz sayısı | GEÇTİ | 0 | >=0 |
| R06 göreli çığ | GEÇTİ | 0.8083333333333332 | >=0.35 |
| R07 ilk tuzak | GEÇTİ | 0.0 | <=0 |
| R08 tuzaklı adım | GEÇTİ | 0 | >=0 |
| R09 fitil/adım p3 | GEÇTİ | 5 | <18 |
| R10 fitil/erken çıkış p3 | GEÇTİ | {"fuse": 5, "k": 3} | F==k+2 |
| R11 tehdit adımı | GEÇTİ | 3 | <=5 |
| R12 bot_rastgele_bombasiz | KALDI | 0.027 | [0.15, 0.50] |
| R12 bot_bomba_unutan | GEÇTİ | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 400, "median": 0.8351361945770859, "p25": 0.8052410989184738, "p75": 0.860882447434925} | n>=20 ve kalibre model |
| R14 kalan süre | KALDI | {"source": "FunSim", "n": 234, "median": 0.7333333333333333, "p25": 0.6666666666666666, "p75": 0.8} | [0.25, 0.5] |
| R15 ilk9 siluet | GEÇTİ | 0.5454545454545454 | <0.90 |
| R16 final temposu | UYARI | {"tail": {"n": 400, "median": 0.31550097597307314, "p25": 0.2701661099253747, "p75": 0.3697306995353336}, "kalibrasyon": 0.2921853674850866} | kalibrasyon medyanı <= 0.2921853674850866; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 1 | 1 |
| R19 FunSim ilk deneme | KALDI | 0.234 | >=0.65 |
| R20 çeşitlilik | GEÇTİ | 0.375 | <0.90 |

Rastgele (1000): kazanma 2.7%, bomba 97.3%, donma 0.0%.
Unutan: Bomb, 5 hamle. Bilinçli: Won, 18 hamle; bombalar [{'p': 3, 'step': 3, 'earliest': 3, 'fuse_margin': 3}].
FunSim: ilk deneme kazanma 23.4%, near-miss 0.0%.
Oyuncu modeli: ZeydBot v1 · kazanma 100.0%; kalan/timer {'n': 400, 'median': 0.8351361945770859, 'p25': 0.8052410989184738, 'p75': 0.860882447434925}.
En yakın 3 (curve): S47 (SET40): 0.937; p21_06_a (aday): 0.900; S42 (SET40): 0.892
En yakın 3 (mask): S42 (SET40): 0.545; S44 (SET40): 0.536; S46 (SET40): 0.536
İlk 1–9 en yakın siluet: S42 (0.545).

Eleştirmen: **RED**
- Yerleşim: bombayı 3. sıradan 4. sıraya al; earliest [4, 5].
- R12 bot_rastgele_bombasiz: ölçülen 0.027, hedef [0.15, 0.50]; tarif `bombs[0].earliest` [3, 4] → [4, 5].
- Öneriyi yeni adaylarda yeniden ölç.
- R14 kalan süre: ölçülen {'source': 'FunSim', 'n': 234, 'median': 0.7333333333333333, 'p25': 0.6666666666666666, 'p75': 0.8}, hedef [0.25, 0.5]; tarif `timer` 60 → 26.
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R19 FunSim ilk deneme: ölçülen 0.234, hedef >=0.65; tarif `bombs[0].region` any → center.
