# PM_MESAJ_109: Dalış hızı 2. tur: hızlı tırmanış, uzun yüksek plato, geç iniş (ÖNİZLEME, yayın yok)

Kodu kendin yaz. `pm108-onizleme` dalında devam (yerel). **Push yok, yayın yok.** Kapsam PM_108 ile aynı: yalnız menü → bölüm dalışının zaman eğrisi. Bitiş mesajı kısa, en altta.

## Zeyd'in yeni çizimi (25 Kasım 10:47)
Üstte PM_108'in hâli, altta istenen.

**PM_108:** orta hızla kalkış → tepeye çıkış → tepe kısa (yolun ilk ~%45'i) → erken iniş → uzun sabit düşük hız.

**İstenen:**
1. Daha **düşük bir hızla** başlar ama **çok dik tırmanır**: sürenin ilk ~%10'unda yüksek hıza ulaşır.
2. **Uzun yüksek plato:** sürenin ~%10'undan ~%70'ine kadar yüksek hız. Plato hafifçe artarak gider; en yüksek nokta ~%60–70'te.
3. **Geç iniş:** ~%70–80 arasında yumuşakça daha düşük sabit hıza iner. Kesikli çizgi (PM_108'de inişin olduğu yer) artık platonun içinde kalıyor.
4. Son ~%20: düşük sabit hız (PM_108'deki son hızla aynı seviye), sonra kısa iniş ve değme (aynı).

## Uygulama
Ekran hızına göre eğri (PM_92/108 yöntemi). Hedef ekran hızı şekli:

| Süre | Hız |
|---|---|
| t = 0 | tepenin ~%40'ı (ilk karede hareket var) |
| 0 → %10 | tepenin ~%85'ine dik ama yumuşak tırmanış (ivme sıçramasız) |
| %10 → %65 | %85 → %100 yavaş yükselen plato |
| %65 → %72 | tepe |
| %72 → %82 | tepenin ~%60'ına yumuşak iniş |
| %82 → son | sabit (±%3), sonra kısa iniş + değme |

- **Toplam süre** kısalır: yüksek hız daha uzun sürdüğü için yay + yaklaşma ~1,0 sn civarı. Kendiliğinden çıkan süreyi raporla, 0,9–1,1 sn bandında tut.
- **Bayraklar:** `&dalis109=0` (PM_108 hâli), `&dalishiz=`, `&dalistepe=`.
- Dalış whoosh'u yeni hıza uysun. Kenar boşluğu 0. 20 ms'yi aşan kare raporu.

## Önizleme
- `docs/art/pm109/hiz.png`: Zeyd'in iki çizimine karşılık gelen hedef şekil ve ölçülen hız; PM_108 ile üst üste.
- `docs/art/pm109/dalis_yan_yana.mp4` (PM_108 | PM_109) ve `dalis_yeni.mp4`: 60 fps, 390×844, sesli, ≤ 10 MB.

## Teslim
- EditMode yeşil (yerel). Push ve yayın yok.
- Son mesaj: süre tablosu (PM_108 → PM_109), video yolları.
