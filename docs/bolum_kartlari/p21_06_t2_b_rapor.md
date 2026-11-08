# p21_06_t2_b · RED

tanitim · 47 sn · Bombayı fark et, yolunu aç ve rahatça bitir.
Anahtar: Alt, üst ve ortadaki bombaların sayaçlarını tanı

Eğri: ▇██▇▇▆▆▆▆▅▄▃▂▂▃▂▃▃▃▃▂▂▃▃▂▂▃▃▃▂ · 6,7,7,6,6,5,5,5,5,4,3,2,1,1,2,1,2,2,2,2,1,1,2,2,1,1,2,2,2,1
30 adım; 3 boğaz, en uzun 2; göreli çığ 0.464.

| Kontrol | Sonuç | Ölçüm | Hedef |
|---|---|---|---|
| R01 çözüm | GEÇTİ | true | True |
| R02 kesin ölçüm | GEÇTİ | 0 | 0 |
| R03 açılış | GEÇTİ | 6 | >=3 |
| R04 boğaz uzunluğu | KALDI | 2 | [0, 1] |
| R05 boğaz sayısı | GEÇTİ | 3 | >=0 |
| R06 göreli çığ | GEÇTİ | 0.4644620811287478 | >=0.35 |
| R07 ilk tuzak | UYARI | 0.0 | <=0 |
| R08 tuzaklı adım | GEÇTİ | 0 | >=0 |
| R09 fitil/adım p1 | GEÇTİ | 12 | <30 |
| R09 tanıtım fitil payı p1 | GEÇTİ | {"fuse": 12, "k": 2} | F>=k+4; üst sınır yok |
| R10 fitil/erken çıkış p1 | UYARI | {"fuse": 12, "k": 2} | F>=k+4 |
| R09 fitil/adım p2 | GEÇTİ | 15 | <30 |
| R09 tanıtım fitil payı p2 | GEÇTİ | {"fuse": 15, "k": 3} | F>=k+4; üst sınır yok |
| R10 fitil/erken çıkış p2 | UYARI | {"fuse": 15, "k": 3} | F>=k+4 |
| R09 fitil/adım p5 | GEÇTİ | 16 | <30 |
| R09 tanıtım fitil payı p5 | GEÇTİ | {"fuse": 16, "k": 2} | F>=k+4; üst sınır yok |
| R10 fitil/erken çıkış p5 | UYARI | {"fuse": 16, "k": 2} | F>=k+4 |
| R11 tehdit adımı | UYARI | 10 | <=5 |
| R12 bot_rastgele_bombasiz | UYARI | 0.591 | [0.15, 0.50] |
| R12 bot_bomba_unutan | UYARI | 0 | <=0.10 |
| R13 bilinçli bot | GEÇTİ | true | True |
| R14 ZeydBot örneği | UYARI | {"n": 400, "median": 0.5533980803733222, "p25": 0.4821903645513983, "p75": 0.6091902077894591} | n>=20 ve kalibre model |
| R14 kalan süre | GEÇTİ | {"source": "FunSim", "n": 772, "median": 0.3404255319148936, "p25": 0.19148936170212766, "p75": 0.46808510638297873} | [0.3, 0.5] |
| R15 ilk9 siluet | UYARI | 1.0 | <0.90 |
| R16 final temposu | UYARI | {"tail": {"n": 400, "median": 0.2580783237547517, "p25": 0.22662883011681567, "p75": 0.30265348172374873}, "kalibrasyon": 0.2921853674850866} | kalibrasyon medyanı <= 0.2921853674850866; danışma kontrolü |
| R18 tarifteki bomba sayısı | GEÇTİ | 3 | 3 |
| R19 FunSim ilk deneme | KALDI | 0.772 | >=0.9 |
| R20 süreden kayıp | KALDI | 0.207 | <=0.05 |
| R21 tahta oranı | GEÇTİ | {"board": [5, 6], "shapes": [[6, 5], [5, 6], [6, 5]]} | |w-h|<=1 |
| R22 çeşitlilik | UYARI | 0.7142857142857143 | <0.90 |

Rastgele (1000): kazanma 59.1%, bomba 40.9%, donma 0.0%.
Unutan: Bomb, 12 hamle. Bilinçli: Won, 30 hamle; bombalar [{'p': 1, 'step': 2, 'earliest': 2, 'fuse_margin': 11}, {'p': 2, 'step': 3, 'earliest': 3, 'fuse_margin': 13}, {'p': 5, 'step': 4, 'earliest': 2, 'fuse_margin': 13}].
FunSim: ilk deneme kazanma 77.2%, near-miss 17.1%.
Oyuncu modeli: ZeydBot v1 · kazanma 100.0%; kalan/timer {'n': 400, 'median': 0.5533980803733222, 'p25': 0.4821903645513983, 'p75': 0.6091902077894591}.
En yakın 3 (curve): S43 (SET40): 0.884; p21_06_t2_c (aday): 0.871; Z21 (SET40): 0.866
En yakın 3 (mask): S06 (SET40): 1.000; S20 (SET40): 1.000; S47 (SET40): 1.000
İlk 1–9 en yakın siluet: S06 (1.000).

siluet maskesi yok: Generator tam dikdörtgen üretir; R15/R22 çeşitlilik UYARI.
FunSim ilk deneme süreden kayıp: 0.207

Eleştirmen: **RED**
- Öneriyi yeni adaylarda yeniden ölç.
- R20 süreden kayıp: ölçülen 0.207, hedef <=0.05; tarif `timer` 47 → 65.
- Öneriyi yeni adaylarda yeniden ölç.
- R04 boğaz uzunluğu: ölçülen 2, hedef [0, 1]; tarif `w,h` [6, 5] → [5, 5].
- Yerleşim: bombayı açılıştan uzak orta bölgeye al; region=center.
- R19 FunSim ilk deneme: ölçülen 0.772, hedef >=0.9; tarif `bombs[0].region` center → center.
