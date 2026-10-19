# ZeydBot seti — oturum linkleri (19 Ekim 2026)

**Önce (bir kez):** Supabase → SQL Editor → `docs/sql/006_moves.sql`'in tamamını yapıştır → Run → alttaki doğrulama tablosunu getir. (Çalıştırmadan oynarsan hamleler telefonda bekler, sonra gider.)

**Nasıl oynanır:** linkler `?debug=1` OLMADAN: ilk açılışta isim kapısına **TEST_ZEYD** yaz. Her oturumun ilk linkini aç; bölüm bitince "Sonraki" ile oturum sırasında devam et (sıra = Z numarası). Yalnız İLK deneme analize girer; kaybedersen tekrar oynayabilirsin, tekrarlar ayrıca işaretlenir. Süre ilk hamleyle başlar. Tür bilgisi bilerek yazılmadı (kör oynansın).

Build 5z yayında (bayraklar kapalı, canlı 40 aynı). Doğrulama: testler 536/537, yerel 7/7 (Z01 kayıtlı çözümle kazanıyor).

### Oturum 1

| Sıra | Bölüm | Link |
|---|---|---|
| 1 | Z01 | [oyna](https://zeydusht.github.io/reverse-solver-unity/br/?set=zeydbot&lv=Z01) |
| 2 | Z02 | [oyna](https://zeydusht.github.io/reverse-solver-unity/br/?set=zeydbot&lv=Z02) |
| 3 | Z03 | [oyna](https://zeydusht.github.io/reverse-solver-unity/br/?set=zeydbot&lv=Z03) |
| 4 | Z04 | [oyna](https://zeydusht.github.io/reverse-solver-unity/br/?set=zeydbot&lv=Z04) |
| 5 | Z05 | [oyna](https://zeydusht.github.io/reverse-solver-unity/br/?set=zeydbot&lv=Z05) |
| 6 | Z06 | [oyna](https://zeydusht.github.io/reverse-solver-unity/br/?set=zeydbot&lv=Z06) |
| 7 | Z07 | [oyna](https://zeydusht.github.io/reverse-solver-unity/br/?set=zeydbot&lv=Z07) |
| 8 | Z08 | [oyna](https://zeydusht.github.io/reverse-solver-unity/br/?set=zeydbot&lv=Z08) |
| 9 | Z09 | [oyna](https://zeydusht.github.io/reverse-solver-unity/br/?set=zeydbot&lv=Z09) |
| 10 | Z10 | [oyna](https://zeydusht.github.io/reverse-solver-unity/br/?set=zeydbot&lv=Z10) |

### Oturum 2

| Sıra | Bölüm | Link |
|---|---|---|
| 1 | Z11 | [oyna](https://zeydusht.github.io/reverse-solver-unity/br/?set=zeydbot&lv=Z11) |
| 2 | Z12 | [oyna](https://zeydusht.github.io/reverse-solver-unity/br/?set=zeydbot&lv=Z12) |
| 3 | Z13 | [oyna](https://zeydusht.github.io/reverse-solver-unity/br/?set=zeydbot&lv=Z13) |
| 4 | Z14 | [oyna](https://zeydusht.github.io/reverse-solver-unity/br/?set=zeydbot&lv=Z14) |
| 5 | Z15 | [oyna](https://zeydusht.github.io/reverse-solver-unity/br/?set=zeydbot&lv=Z15) |
| 6 | Z16 | [oyna](https://zeydusht.github.io/reverse-solver-unity/br/?set=zeydbot&lv=Z16) |
| 7 | Z17 | [oyna](https://zeydusht.github.io/reverse-solver-unity/br/?set=zeydbot&lv=Z17) |
| 8 | Z18 | [oyna](https://zeydusht.github.io/reverse-solver-unity/br/?set=zeydbot&lv=Z18) |
| 9 | Z19 | [oyna](https://zeydusht.github.io/reverse-solver-unity/br/?set=zeydbot&lv=Z19) |
| 10 | Z20 | [oyna](https://zeydusht.github.io/reverse-solver-unity/br/?set=zeydbot&lv=Z20) |

### Oturum 3

| Sıra | Bölüm | Link |
|---|---|---|
| 1 | Z21 | [oyna](https://zeydusht.github.io/reverse-solver-unity/br/?set=zeydbot&lv=Z21) |
| 2 | Z22 | [oyna](https://zeydusht.github.io/reverse-solver-unity/br/?set=zeydbot&lv=Z22) |
| 3 | Z23 | [oyna](https://zeydusht.github.io/reverse-solver-unity/br/?set=zeydbot&lv=Z23) |
| 4 | Z24 | [oyna](https://zeydusht.github.io/reverse-solver-unity/br/?set=zeydbot&lv=Z24) |
| 5 | Z25 | [oyna](https://zeydusht.github.io/reverse-solver-unity/br/?set=zeydbot&lv=Z25) |
| 6 | Z26 | [oyna](https://zeydusht.github.io/reverse-solver-unity/br/?set=zeydbot&lv=Z26) |
| 7 | Z27 | [oyna](https://zeydusht.github.io/reverse-solver-unity/br/?set=zeydbot&lv=Z27) |
| 8 | Z28 | [oyna](https://zeydusht.github.io/reverse-solver-unity/br/?set=zeydbot&lv=Z28) |
| 9 | Z29 | [oyna](https://zeydusht.github.io/reverse-solver-unity/br/?set=zeydbot&lv=Z29) |
| 10 | Z30 | [oyna](https://zeydusht.github.io/reverse-solver-unity/br/?set=zeydbot&lv=Z30) |

### Setin ölçümü (oynamadan önce; tür sütunu sende bakmaman için en altta)

Kaçırılan: **yeni M1** (zor bölümlerde açılışta 2–5 doğru, tuzak ≈ 0; hedef 1–2 doğru + 4–5 tuzak) · **M12** (zor bölümlerin bir kısmında tek eksen > %60). M11 zor bölümlerin çoğunda 4/4 çeyrek. Hepsi adil (kayıtlı çözüm kazanıyor, supap 0).

<details><summary>Tablo</summary>

| Bölüm | Oturum | Tür | Tahta / parça | Adil (çözüm, supap) | M1 doğru / tuzak | M2 en çok serbest | M4 | M11 çeyrek / kenar | M12 tek eksen | Bomba / çivi / zincir | Rastgele kazanma |
|---|---|---|---|---|---|---|---|---|---|---|---|
| Z01 | 1 | zor | 7×8 / 54 | ✓ | 3 / 0 | 4 | 4 | 4/4 / dolu | %59 | 4 / 0 / 2 | %0 |
| Z02 | 1 | orta | 6×7 / 39 | ✓ | 12 / 0 | 12 | 3 | 4/4 / dolu | %54 | 1 / 2 / 3 | %26 |
| Z03 | 1 | kolay | 6×6 / 35 | ✓ | 8 / 0 | 8 | 4 | 2/4 / dolu | %51 | 1 / 2 / 1 | %73 |
| Z04 | 1 | orta | 6×7 / 39 | ✓ | 11 / 0 | 11 | 4 | 2/4 / dolu | %56 | 1 / 2 / 3 | %5 |
| Z05 | 1 | kolay | 5×5 / 25 | ✓ | 11 / 0 | 11 | 3 | 1/4 / dolu | %60 | 0 / 1 / 0 | %100 |
| Z06 | 1 | kolay | 5×6 / 30 | ✓ | 6 / 0 | 7 | 4 | 1/4 / dolu | %53 | 0 / 1 / 0 | %100 |
| Z07 | 1 | zor | 7×8 / 54 | ✓ | 1 / 2 | 7 | 6 | 4/4 / dolu | %70 | 4 / 1 / 2 | %0 |
| Z08 | 1 | zor | 7×8 / 54 | ✓ | 2 / 0 | 5 | 5 | 4/4 / dolu | %63 | 4 / 3 / 2 | %0 |
| Z09 | 1 | kolay | 5×6 / 30 | ✓ | 10 / 0 | 12 | 2 | 1/4 / boş | %77 | 0 / 1 / 0 | %100 |
| Z10 | 1 | orta | 6×7 / 39 | ✓ | 15 / 0 | 15 | 2 | 3/4 / dolu | %62 | 1 / 2 / 3 | %5 |
| Z11 | 2 | kolay | 6×6 / 34 | ✓ | 8 / 0 | 10 | 3 | 2/4 / dolu | %56 | 0 / 1 / 2 | %100 |
| Z12 | 2 | zor | 7×8 / 54 | ✓ | 5 / 0 | 5 | 6 | 4/4 / dolu | %54 | 4 / 1 / 2 | %0 |
| Z13 | 2 | zor | 7×8 / 54 | ✓ | 4 / 0 | 5 | 5 | 3/4 / dolu | %61 | 4 / 0 / 2 | %0 |
| Z14 | 2 | kolay | 6×6 / 34 | ✓ | 6 / 0 | 7 | 2 | 2/4 / dolu | %71 | 0 / 2 / 2 | %100 |
| Z15 | 2 | orta | 6×7 / 39 | ✓ | 10 / 0 | 10 | 3 | 4/4 / dolu | %51 | 1 / 2 / 3 | %9 |
| Z16 | 2 | orta | 6×7 / 39 | ✓ | 11 / 0 | 11 | 2 | 3/4 / dolu | %56 | 1 / 2 / 3 | %0 |
| Z17 | 2 | orta | 6×7 / 40 | ✓ | 12 / 0 | 12 | 3 | 3/4 / dolu | %60 | 1 / 2 / 2 | %6 |
| Z18 | 2 | zor | 7×8 / 54 | ✓ | 3 / 0 | 3 | 6 | 4/4 / dolu | %63 | 4 / 4 / 2 | %0 |
| Z19 | 2 | orta | 6×7 / 40 | ✓ | 13 / 0 | 13 | 3 | 3/4 / dolu | %58 | 1 / 2 / 2 | %11 |
| Z20 | 2 | kolay | 5×6 / 30 | ✓ | 11 / 0 | 11 | 2 | 0/4 / boş | %80 | 0 / 0 / 0 | %100 |
| Z21 | 3 | orta | 6×7 / 39 | ✓ | 9 / 0 | 9 | 2 | 4/4 / dolu | %51 | 1 / 2 / 3 | %6 |
| Z22 | 3 | orta | 6×7 / 39 | ✓ | 9 / 0 | 9 | 3 | 3/4 / dolu | %51 | 1 / 2 / 3 | %7 |
| Z23 | 3 | zor | 7×8 / 54 | ✓ | 2 / 0 | 4 | 5 | 4/4 / dolu | %67 | 4 / 4 / 2 | %0 |
| Z24 | 3 | orta | 6×7 / 39 | ✓ | 12 / 0 | 12 | 3 | 3/4 / dolu | %54 | 1 / 2 / 3 | %12 |
| Z25 | 3 | zor | 7×8 / 54 | ✓ | 2 / 0 | 3 | 4 | 4/4 / dolu | %56 | 4 / 1 / 2 | %4 |
| Z26 | 3 | zor | 7×8 / 54 | ✓ | 2 / 0 | 4 | 4 | 4/4 / dolu | %72 | 4 / 2 / 2 | %28 |
| Z27 | 3 | kolay | 4×5 / 20 | ✓ | 9 / 0 | 10 | 2 | 0/4 / boş | %75 | 0 / 0 / 0 | %100 |
| Z28 | 3 | kolay | 6×6 / 36 | ✓ | 8 / 0 | 8 | 2 | 2/4 / dolu | %86 | 0 / 2 / 0 | %100 |
| Z29 | 3 | zor | 7×8 / 54 | ✓ | 5 / 0 | 7 | 4 | 4/4 / dolu | %59 | 4 / 2 / 2 | %0 |
| Z30 | 3 | kolay | 5×6 / 29 | ✓ | 8 / 0 | 8 | 3 | 2/4 / dolu | %72 | 0 / 1 / 1 | %100 |

</details>

---

Eski raporlar (D08 dahil): `docs/arsiv/OVERNIGHT_ARSIV.md`.
