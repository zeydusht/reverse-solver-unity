# PM_MESAJ_110: Dalış hızı 3. tur: yüksekten başla, uzun plato, dik iniş, daha yavaş ve hafif artan son (ÖNİZLEME, yayın yok)

Kodu kendin yaz. `pm108-onizleme` dalında devam (yerel). **Push yok, yayın yok.** Kapsam PM_108/109 ile aynı: yalnız menü → bölüm dalışının zaman eğrisi. Bitiş mesajı kısa, en altta.

## Zeyd'in 3. çizimi (25 Kasım 11:49)
PM_109'dan farkları:
- **Başlangıç çok daha yüksek:** neredeyse tepe hızında başlar. Kısa bir tırmanışla (ilk ~%8) platoya oturur.
- **Plato düz ve uzun:** ~%8'den ~%60'a kadar. Sonra hafifçe yükselip ~%62'de tepe.
- **Dik iniş:** ~%62'den ~%82'ye kadar belirgin ve hızlı iniş. Yumuşak bitişli, ivme sıçramasız.
- **Son hız daha düşük:** tepenin ~%50'si (PM_109'da %61'di).
- **Son kısım düz değil, hafifçe artıyor:** %82'den sona %50 → %55. Sonra kısa iniş ve değme (aynı).

## Hedef ekran hızı (tepeye göre)

| Süre | Hız |
|---|---|
| t = 0 | %90 |
| 0 → %8 | %97'ye kısa tırmanış |
| %8 → %55 | %97 düz plato |
| %55 → %62 | %100 tepe |
| %62 → %82 | %50'ye dik ama yumuşak iniş |
| %82 → son | %50 → %55 hafif artış, son 0,03 sn kısa iniş + değme |

- **Toplam süre:** kendiliğinden çıkanı raporla; muhtemelen ~1,0 sn, 0,9–1,1 sn bandında tut.
- **Bayraklar:** `&dalis110=0` (PM_109 hâli), `&dalis109=0` (PM_108), `&dalishiz=`, `&dalistepe=`.
- Dalış whoosh'u yeni hıza uysun.
- Kenar boşluğu 0. İlk karede kenar açıklığı kontrolü (başlangıç hızı yüksek).

## Önizleme
- `docs/art/pm110/hiz.png`: hedef + ölçülen; PM_109 ile üst üste.
- `docs/art/pm110/dalis_yan_yana.mp4` (PM_109 | PM_110) ve `dalis_yeni.mp4`: 60 fps, 390×844, sesli, ≤ 10 MB.

## Teslim
- EditMode yeşil (yerel). Push ve yayın yok.
- Son mesaj: süre tablosu (PM_109 → PM_110), video yolları.
