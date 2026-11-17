# PM_82: İlk 100 bölüm (sl100)

Model v2 (ortalama oyuncu, ilk deneme kazanma) ile hedefe göre seçildi: `Tools/sl100/Design.cs` (aday havuzu + üretici + süre ayarı), `Tools/sl100/finalize.py` (set, rapor, grafik).
Eğri: `docs/analiz/pm82_egri.png`. Tahminler: `docs/analiz/v2/pred_sl100.csv`.

| Bölüm | Tip | Engeller | Tahta | Parça | Süre | İlk deneme | Kaynak | Aday |
|---|---|---|---|---|---|---|---|---|
| 1 | rehber | — | 3×3 | 9 | 120 sn | %100 | eski (p22 · aynen) | 1 |
| 2 | öğrenme | — | 4×4 | 16 | 72 sn | %97 | eski (p22 · ayarlı) | 2 |
| 3 | ders | kilit | 4×5 | 20 | 90 sn | %97 | eski (p22 · ayarlı) | 1 |
| 4 | öğrenme | kilit | 5×5 | 25 | 69 sn | %93 | yeni (üretici) | 159 |
| 5 | öğrenme | kilit | 4×5 | 20 | 55 sn | %93 | yeni (havuz: fab) | 157 |
| 6 | öğrenme | kilit | 5×5 | 25 | 69 sn | %93 | yeni (üretici) | 155 |
| 7 | ders | zincir | 6×7 | 41 | 90 sn | %92 | eski (p22 · ayarlı) | 1 |
| 8 | öğrenme | kilit, zincir | 6×6 | 34 | 89 sn | %93 | yeni (üretici) | 906 |
| 9 | öğrenme | kilit, zincir | 5×5 | 23 | 62 sn | %93 | yeni (üretici) | 904 |
| 10 | öğrenme | kilit, zincir | 6×6 | 34 | 70 sn | %93 | yeni (havuz: web) | 902 |
| 11 | ders | bomba | 4×4 | 16 | 120 sn | %99 | eski (p22 · ayarlı) | 1 |
| 12 | öğrenme | kilit, zincir, bomba | 5×5 | 21 | 73 sn | %93 | yeni (üretici) | 1471 |
| 13 | öğrenme | bomba | 5×6 | 30 | 83 sn | %93 | eski (p22 · ayarlı) | 1467 |
| 14 | zor | kilit, zincir, bomba | 6×6 | 35 | 64 sn | %42 | yeni (havuz: web) | 1550 |
| 15 | normal | bomba | 5×5 | 25 | 63 sn | %80 | yeni (üretici) | 1170 |
| 16 | normal | kilit, zincir, bomba | 6×6 | 33 | 85 sn | %80 | yeni (üretici) | 1166 |
| 17 | normal | bomba | 5×5 | 25 | 64 sn | %80 | yeni (üretici) | 1162 |
| 18 | normal | bomba | 6×6 | 36 | 93 sn | %80 | yeni (üretici) | 1158 |
| 19 | çok zor | kilit, zincir, bomba | 7×7 | 47 | 122 sn | %22 | yeni (havuz: p22) | 1571 |
| 20 | nefes | kilit, zincir, bomba | 5×5 | 23 | 73 sn | %90 | yeni (üretici) | 953 |
| 21 | ders | duvar | 6×7 | 42 | 90 sn | %90 | eski (p22 · ayarlı) | 1 |
| 22 | normal | kilit, bomba, duvar | 6×6 | 36 | 83 sn | %80 | eski (p22 · ayarlı) | 1723 |
| 23 | normal | kilit, zincir, bomba, duvar | 6×7 | 39 | 104 sn | %80 | eski (p22 · ayarlı) | 1711 |
| 24 | zor | kilit, zincir, bomba, duvar | 7×7 | 47 | 78 sn | %42 | eski (p22 · ayarlı) | 1460 |
| 25 | normal | kilit, zincir, bomba, duvar | 6×6 | 34 | 95 sn | %76 | eski (p22 · ayarlı) | 1703 |
| 26 | normal | kilit, zincir, bomba, duvar | 6×7 | 39 | 86 sn | %79 | eski (p22 · ayarlı) | 1695 |
| 27 | normal | bomba, duvar | 6×6 | 36 | 70 sn | %79 | yeni (üretici) | 1687 |
| 28 | normal | kilit, zincir, bomba, duvar | 6×7 | 40 | 112 sn | %78 | eski (p22 · ayarlı) | 1681 |
| 29 | çok zor | kilit, bomba, duvar | 6×6 | 36 | 60 sn | %27 | eski (p22 · ayarlı) | 1496 |
| 30 | nefes | kilit, zincir, bomba, duvar | 5×5 | 23 | 49 sn | %90 | yeni (üretici) | 1359 |
| 31 | birleşim | zincir, bomba | 7×7 | 46 | 88 sn | %88 | yeni (üretici) | 229 |
| 32 | normal | kilit, zincir, bomba | 6×6 | 32 | 61 sn | %78 | yeni (üretici) | 2073 |
| 33 | normal | kilit, zincir, bomba | 7×7 | 48 | 86 sn | %78 | yeni (üretici) | 2069 |
| 34 | zor | kilit, zincir, bomba, duvar | 7×8 | 54 | 87 sn | %42 | eski (p22 · ayarlı) | 805 |
| 35 | normal | kilit, zincir, bomba, duvar | 6×6 | 35 | 74 sn | %78 | yeni (havuz: fab) | 2065 |
| 36 | normal | zincir, bomba | 7×7 | 45 | 111 sn | %78 | yeni (üretici) | 2057 |
| 37 | normal | kilit, zincir, bomba | 6×6 | 34 | 69 sn | %78 | yeni (havuz: fab) | 2053 |
| 38 | normal | zincir, bomba, duvar | 7×7 | 46 | 96 sn | %78 | yeni (üretici) | 2045 |
| 39 | çok zor | kilit, zincir, bomba, duvar | 8×8 | 62 | 100 sn | %27 | eski (p22 · ayarlı) | 861 |
| 40 | nefes | kilit, zincir, bomba | 5×5 | 24 | 51 sn | %90 | yeni (üretici) | 1351 |
| 41 | birleşim | kilit, bomba | 7×7 | 49 | 107 sn | %86 | yeni (üretici) | 66 |
| 42 | normal | kilit, zincir, bomba | 6×6 | 34 | 77 sn | %77 | yeni (havuz: web) | 2037 |
| 43 | normal | kilit, zincir, bomba, duvar | 7×7 | 46 | 106 sn | %77 | yeni (üretici) | 2033 |
| 44 | zor | kilit, zincir, bomba, duvar | 7×8 | 54 | 90 sn | %42 | eski (p22 · ayarlı) | 790 |
| 45 | normal | kilit, zincir, bomba | 6×6 | 33 | 74 sn | %77 | yeni (üretici) | 2025 |
| 46 | normal | kilit, zincir, bomba, duvar | 7×7 | 46 | 100 sn | %77 | yeni (üretici) | 2021 |
| 47 | normal | kilit, zincir, bomba, duvar | 6×6 | 35 | 88 sn | %77 | yeni (havuz: web) | 2013 |
| 48 | normal | kilit, zincir, bomba | 7×7 | 47 | 116 sn | %76 | yeni (üretici) | 2005 |
| 49 | çok zor | kilit, zincir, bomba, duvar | 8×8 | 62 | 100 sn | %30 | eski (p22 · ayarlı) | 850 |
| 50 | nefes | kilit | 5×5 | 25 | 55 sn | %90 | eski (p22 · ayarlı) | 1343 |
| 51 | birleşim | bomba, duvar | 7×7 | 49 | 90 sn | %85 | yeni (üretici) | 68 |
| 52 | normal | kilit, zincir, bomba, duvar | 6×6 | 33 | 92 sn | %73 | eski (p22 · ayarlı) | 1997 |
| 53 | normal | kilit, zincir, bomba, duvar | 7×7 | 47 | 120 sn | %74 | yeni (üretici) | 1989 |
| 54 | zor | kilit, zincir, bomba, duvar | 6×7 | 39 | 75 sn | %42 | eski (p22 · ayarlı) | 783 |
| 55 | normal | zincir, bomba, duvar | 6×6 | 35 | 73 sn | %76 | yeni (üretici) | 1981 |
| 56 | normal | zincir, bomba | 7×7 | 46 | 118 sn | %76 | yeni (üretici) | 1973 |
| 57 | normal | kilit, zincir, bomba, duvar | 6×6 | 33 | 86 sn | %75 | yeni (üretici) | 1969 |
| 58 | normal | zincir, bomba | 7×7 | 47 | 117 sn | %75 | yeni (üretici) | 1961 |
| 59 | çok zor | kilit, zincir, bomba, duvar | 7×8 | 54 | 87 sn | %28 | eski (p22 · ayarlı) | 845 |
| 60 | nefes | bomba, duvar | 5×5 | 25 | 55 sn | %90 | yeni (üretici) | 1658 |
| 61 | birleşim | zincir, duvar | 6×6 | 35 | 60 sn | %86 | yeni (üretici) | 174 |
| 62 | normal | kilit, zincir, bomba, duvar | 7×8 | 54 | 92 sn | %75 | eski (p22 · ayarlı) | 1855 |
| 63 | normal | zincir, bomba, duvar | 6×6 | 33 | 78 sn | %75 | yeni (havuz: fab) | 1843 |
| 64 | zor | zincir, bomba, duvar | 7×8 | 54 | 87 sn | %45 | eski (p22 · ayarlı) | 592 |
| 65 | normal | zincir, bomba, duvar | 6×6 | 34 | 87 sn | %74 | yeni (havuz: fab) | 1835 |
| 66 | normal | kilit, zincir, bomba, duvar | 7×8 | 54 | 120 sn | %74 | eski (p22 · ayarlı) | 1827 |
| 67 | normal | kilit, zincir, bomba, duvar | 6×6 | 34 | 74 sn | %74 | yeni (havuz: fab) | 1815 |
| 68 | normal | kilit, zincir, bomba | 6×7 | 39 | 98 sn | %74 | eski (p22 · ayarlı) | 1807 |
| 69 | çok zor | kilit, zincir, bomba, duvar | 7×8 | 54 | 87 sn | %29 | eski (p22 · ayarlı) | 645 |
| 70 | nefes | duvar | 5×5 | 25 | 55 sn | %90 | yeni (üretici) | 1652 |
| 71 | birleşim | kilit, zincir, bomba | 6×6 | 33 | 76 sn | %83 | yeni (üretici) | 169 |
| 72 | normal | kilit, zincir, bomba | 6×7 | 40 | 71 sn | %74 | eski (p22 · ayarlı) | 1803 |
| 73 | normal | kilit, zincir, bomba | 6×6 | 35 | 60 sn | %73 | yeni (üretici) | 1799 |
| 74 | zor | kilit, zincir, bomba, duvar | 7×8 | 54 | 106 sn | %42 | yeni (havuz: zb) | 589 |
| 75 | normal | kilit, zincir, bomba | 6×6 | 33 | 92 sn | %73 | yeni (havuz: fab) | 1795 |
| 76 | normal | bomba, duvar | 7×7 | 49 | 120 sn | %70 | yeni (üretici) | 1787 |
| 77 | normal | kilit, zincir, bomba | 6×6 | 33 | 74 sn | %73 | yeni (üretici) | 1781 |
| 78 | normal | kilit, zincir, bomba | 6×7 | 39 | 103 sn | %73 | yeni (havuz: zb) | 1777 |
| 79 | çok zor | kilit, zincir, bomba | 7×8 | 54 | 89 sn | %25 | yeni (havuz: zb) | 636 |
| 80 | nefes | kilit, zincir, duvar | 6×6 | 35 | 69 sn | %90 | eski (p22 · ayarlı) | 1646 |
| 81 | birleşim | kilit, duvar | 7×7 | 49 | 84 sn | %85 | yeni (üretici) | 66 |
| 82 | normal | kilit, zincir, bomba, duvar | 6×6 | 34 | 78 sn | %72 | yeni (havuz: web) | 1769 |
| 83 | normal | kilit, zincir, bomba, duvar | 6×7 | 40 | 82 sn | %72 | yeni (havuz: web) | 1761 |
| 84 | zor | kilit, zincir, bomba, duvar | 7×8 | 54 | 95 sn | %42 | yeni (havuz: zb) | 582 |
| 85 | normal | kilit, zincir, bomba, duvar | 6×6 | 35 | 98 sn | %70 | yeni (havuz: fab) | 1753 |
| 86 | normal | kilit, zincir, bomba, duvar | 7×8 | 54 | 120 sn | %67 | eski (p22 · ayarlı) | 1745 |
| 87 | normal | kilit, zincir, bomba, duvar | 6×6 | 35 | 60 sn | %74 | yeni (havuz: web) | 1737 |
| 88 | normal | kilit, zincir, bomba, duvar | 7×7 | 45 | 84 sn | %72 | yeni (üretici) | 1729 |
| 89 | çok zor | zincir, bomba, duvar | 7×8 | 54 | 87 sn | %26 | yeni (havuz: zb) | 629 |
| 90 | nefes | kilit, duvar | 5×5 | 25 | 55 sn | %90 | yeni (havuz: p22) | 1642 |
| 91 | birleşim | kilit, zincir, bomba, duvar | 6×6 | 33 | 85 sn | %80 | yeni (üretici) | 105 |
| 92 | normal | kilit, zincir, bomba, duvar | 7×7 | 47 | 101 sn | %71 | yeni (üretici) | 1721 |
| 93 | normal | bomba | 6×6 | 36 | 81 sn | %71 | yeni (üretici) | 1713 |
| 94 | zor | kilit, bomba | 7×7 | 49 | 79 sn | %48 | yeni (üretici) | 575 |
| 95 | normal | kilit, zincir, bomba | 6×6 | 35 | 65 sn | %70 | yeni (üretici) | 1709 |
| 96 | normal | kilit, zincir, bomba, duvar | 7×7 | 48 | 87 sn | %70 | yeni (üretici) | 1705 |
| 97 | normal | bomba, duvar | 6×6 | 36 | 73 sn | %70 | yeni (üretici) | 1697 |
| 98 | normal | kilit, zincir, bomba, duvar | 7×7 | 47 | 102 sn | %70 | yeni (üretici) | 1691 |
| 99 | çok zor | zincir, bomba | 7×8 | 54 | 140 sn | %24 | yeni (havuz: zb) | 622 |
| 100 | nefes | kilit, zincir, bomba, duvar | 6×6 | 34 | 94 sn | %90 | yeni (üretici) | 1634 |
