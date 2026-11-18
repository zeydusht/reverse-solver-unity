# PM_83: Sandık ekonomisi, 1→100 sanal oyuncu

`dotnet run -c Release --project Tools/sl100 -- economy 1000`: 1000 sanal oyuncu, sl100'ün 100 bölümü, sandıklar oyunun kendi kuralıyla (Core `ChestEconomy`, `live/config.json` sayıları).

Sandık: 100 bölümde 29 sandık (20 normal, 9 Kont).

| Bölüm sonu | Altın (ort.) | Altın (ortanca) | Elde booster (ort.) | Sandıktan altın | Kazanmadan altın | Devama harcanan | Sandıktan booster | Kullanılan booster | Deneme |
|---|---|---|---|---|---|---|---|---|---|
| 5 | 5299 | 5400 | 0,0 | 100 | 325 | 126 | 0,0 | 0,0 | 5,1 |
| 10 | 5579 | 5800 | 3,2 | 200 | 648 | 270 | 1,0 | 0,8 | 10,3 |
| 19 | 5326 | 5850 | 9,4 | 500 | 1331 | 1505 | 4,0 | 3,6 | 20,6 |
| 25 | 5401 | 5750 | 12,8 | 750 | 1818 | 2167 | 6,0 | 5,2 | 27,4 |
| 50 | 5640 | 5650 | 16,0 | 2400 | 3645 | 5405 | 17,0 | 13,0 | 56,7 |
| 75 | 5830 | 5700 | 17,5 | 4200 | 5566 | 8936 | 26,0 | 20,5 | 86,1 |
| 100 | 6060 | 5850 | 20,6 | 6400 | 7390 | 12730 | 37,0 | 28,4 | 116,2 |

Booster dağılımı (100. bölüm sonu, ortalama elde): scissors 5,2, wand 5,1, hammer 5,1, clock 5,2.

## Sandık içerikleri

| Bölüm | Sandık | Altın | Booster |
|---|---|---|---|
| 5 | normal | 100 | — |
| 10 | normal | 100 | 1 (açılmışlardan: scissors) |
| 14 | zor bölüm bonusu | +100 | — |
| 15 | normal | 100 | 1 (açılmışlardan: scissors, hammer) |
| 19 | Kont | 200 | 2 (açılmışlardan: scissors, hammer, clock) |
| 20 | normal | 100 | 1 (açılmışlardan: scissors, hammer, clock) |
| 24 | zor bölüm bonusu | +100 | — |
| 25 | normal | 150 | 1 (açılmışlardan: scissors, wand, hammer, clock) |
| 29 | Kont | 300 | 2 (açılmışlardan: scissors, wand, hammer, clock) |
| 30 | normal | 150 | 1 (açılmışlardan: scissors, wand, hammer, clock) |
| 34 | zor bölüm bonusu | +100 | — |
| 35 | normal | 150 | 1 (açılmışlardan: scissors, wand, hammer, clock) |
| 39 | Kont | 300 | 2 (açılmışlardan: scissors, wand, hammer, clock) |
| 40 | normal | 150 | 1 (açılmışlardan: scissors, wand, hammer, clock) |
| 44 | zor bölüm bonusu | +100 | — |
| 45 | normal | 150 | 1 (açılmışlardan: scissors, wand, hammer, clock) |
| 49 | Kont | 300 | 2 (açılmışlardan: scissors, wand, hammer, clock) |
| 50 | normal | 150 | 1 (açılmışlardan: scissors, wand, hammer, clock) |
| 54 | zor bölüm bonusu | +100 | — |
| 55 | normal | 200 | 1 (açılmışlardan: scissors, wand, hammer, clock) |
| 59 | Kont | 400 | 2 (açılmışlardan: scissors, wand, hammer, clock) |
| 60 | normal | 200 | 1 (açılmışlardan: scissors, wand, hammer, clock) |
| 64 | zor bölüm bonusu | +100 | — |
| 65 | normal | 200 | 1 (açılmışlardan: scissors, wand, hammer, clock) |
| 69 | Kont | 400 | 2 (açılmışlardan: scissors, wand, hammer, clock) |
| 70 | normal | 200 | 1 (açılmışlardan: scissors, wand, hammer, clock) |
| 74 | zor bölüm bonusu | +100 | — |
| 75 | normal | 200 | 1 (açılmışlardan: scissors, wand, hammer, clock) |
| 79 | Kont | 400 | 2 (açılmışlardan: scissors, wand, hammer, clock) |
| 80 | normal | 200 | 1 (açılmışlardan: scissors, wand, hammer, clock) |
| 84 | zor bölüm bonusu | +100 | — |
| 85 | normal | 200 | 1 (açılmışlardan: scissors, wand, hammer, clock) |
| 89 | Kont | 400 | 2 (açılmışlardan: scissors, wand, hammer, clock) |
| 90 | normal | 200 | 1 (açılmışlardan: scissors, wand, hammer, clock) |
| 94 | zor bölüm bonusu | +100 | — |
| 95 | normal | 200 | 1 (açılmışlardan: scissors, wand, hammer, clock) |
| 99 | Kont | 400 | 2 (açılmışlardan: scissors, wand, hammer, clock) |
| 100 | normal | 200 | 1 (açılmışlardan: scissors, wand, hammer, clock) |

## Oyuncu modeli (varsayımlar)
- İlk deneme model v2 tahminiyle kazanılır (`docs/analiz/v2/pred_sl100.csv`); her tekrar +0,12 (en çok 0,97).
- Booster: zor/çok zor bölümde %60, diğerlerinde %15 kullanılır (elde varsa); kullanılan booster +0,10.
- Süre dolunca: %40 reklamla +30 sn (R1, %55 kurtarır); sonra altınla devam 900 / 1900 / 2900, alma olasılığı %50 / %30 / %15, %55 kurtarır.
- Kazanma +50; %30'u 2× altın reklamını izler (R2). Zor bölüm bonusu ve sandıklar oyunun kuralıyla. R3/R4 ve satın alma yok.
- Booster açılınca 3 ile başlar (BoosterInventory); açılmamış booster sandıktan gelirse açılınca eklenir.
