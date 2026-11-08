# p21_06_t3_a · RED

tanitim · 65 sn · Bombayı fark et, yolunu aç ve rahatça bitir.
Anahtar: Alt, üst ve ortadaki bombaların sayaçlarını tanı

Eğri: ██▇▇▇▇▇▇▆▆▅▆▆▅▅▄▄▄▃▃▃▃▃▂▁ · 9,9,8,8,8,8,8,8,7,7,6,7,7,6,6,5,5,4,3,3,3,3,3,2,1
25 adım; 0 boğaz, en uzun 1; göreli çığ 0.722.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 8 | >=3 |
| R04 boğaz uzunluğu | GEÇTİ | 1 | [0, 1] |
| R05 boğaz sayısı | GEÇTİ | 0 | >=0 |
| R06 göreli çığ | GEÇTİ | 0.7223214285714286 | >=0.35 |
| R07 ilk tuzak | UYARI | 0.0 | <=0 |
| R08 tuzaklı adım | GEÇTİ | 0 | >=0 |
| R09 fitil/adım p3 | GEÇTİ | 15 | <25 |
| R09 tanıtım fitil payı p3 | GEÇTİ | {"fuse": 15, "k": 3} | F>=k+4; üst sınır yok |
| R10 fitil/erken çıkış p3 | UYARI | {"fuse": 15, "k": 3} | F>=k+4 |
| R09 fitil/adım p5 | GEÇTİ | 16 | <25 |
| R09 tanıtım fitil payı p5 | GEÇTİ | {"fuse": 16, "k": 2} | F>=k+4; üst sınır yok |
| R10 fitil/erken çıkış p5 | UYARI | {"fuse": 16, "k": 2} | F>=k+4 |
| R09 fitil/adım p16 | GEÇTİ | 12 | <25 |
| R09 tanıtım fitil payı p16 | GEÇTİ | {"fuse": 12, "k": 2} | F>=k+4; üst sınır yok |
| R10 fitil/erken çıkış p16 | UYARI | {"fuse": 12, "k": 2} | F>=k+4 |
| R11 tehdit adımı | UYARI | 10 | <=5 |
| R12 bot_rastgele_bombasiz | UYARI | 0.224 | [0.15, 0.50] |
| R12 bot_bomba_unutan | UYARI | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 400, "median": 0.7758236762460868, "p25": 0.734239527323995, "p75": 0.8101906303206277} | n>=20 ve kalibre model |
| R14 kalan süre | KALDI | {"source": "FunSim", "n": 898, "median": 0.6461538461538462, "p25": 0.5384615384615384, "p75": 0.7230769230769231} | [0.3, 0.5] |
| R15 ilk9 siluet | UYARI | 1.0 | <0.90 |
| R16 final temposu | UYARI | {"tail": {"n": 400, "median": 0.2996349000886708, "p25": 0.25843013294025563, "p75": 0.34144883824230704}, "kalibrasyon": 0.2921853674850866} | kalibrasyon medyanı <= 0.2921853674850866; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 3 | 3 |
| R19 FunSim ilk deneme | KALDI | 0.898 | >=0.9 |
| R20 süreden kayıp | GEÇTİ | 0.002 | <=0.05 |
| R21 tahta oranı | GEÇTİ | {"board": [5, 5], "shapes": [[5, 5], [6, 6], [7, 7]]} | |w-h|<=1 |
| R22 çeşitlilik | UYARI | 0.6944444444444444 | <0.90 |

Rastgele (1000): kazanma 22.4%, bomba 77.6%, donma 0.0%.
Unutan: Bomb, 12 hamle. Bilinçli: Won, 25 hamle; bombalar [{'p': 16, 'step': 2, 'earliest': 2, 'fuse_margin': 11}, {'p': 3, 'step': 5, 'earliest': 3, 'fuse_margin': 11}, {'p': 5, 'step': 7, 'earliest': 2, 'fuse_margin': 10}].
FunSim: ilk deneme kazanma 89.8%, near-miss 0.6%.
Oyuncu modeli: ZeydBot v1 · kazanma 100.0%; kalan/timer {'n': 400, 'median': 0.7758236762460868, 'p25': 0.734239527323995, 'p75': 0.8101906303206277}.
En yakın 3 (curve): S41 (SET40): 0.957; S52 (SET40): 0.942; p21_06_t3_c (aday): 0.940
En yakın 3 (mask): S44 (SET40): 1.000; S46 (SET40): 1.000; S52 (SET40): 0.962
İlk 1–9 en yakın siluet: S44 (1.000).

siluet maskesi yok: Generator tam dikdörtgen üretir; R15/R22 çeşitlilik UYARI.
FunSim ilk deneme süreden kayıp: 0.002

Eleştirmen: **RED**
- Öneriyi yeni adaylarda yeniden ölç.
- R14 kalan süre: ölçülen {'source': 'FunSim', 'n': 898, 'median': 0.6461538461538462, 'p25': 0.5384615384615384, 'p75': 0.7230769230769231}, hedef [0.3, 0.5]; tarif `timer` 65 → 38.
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R19 FunSim ilk deneme: ölçülen 0.898, hedef >=0.9; tarif `bombs[0].region` center → center.
