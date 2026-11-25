# PM_MESAJ_111: B seçildi; resimde kalış için 3 varyant (ÖNİZLEME, yayın yok)

Kodu kendin yaz. `pm108-onizleme` dalında devam (yerel). **Push yok, yayın yok.** Bitiş mesajı kısa, en altta.

Zeyd (25 Kasım 17:41): "B hoşuma gitti, güzel."
- `&dalis110=b` **varsayılan** olsun (dalda).
- Kapsam: yalnız **resimde kalış süresi** (tabloya varış → geri çekilmenin başlaması). Gidiş (B), geri çekilme (0,78 sn), yol, çerçeve, sesler aynı.

## İstenen: 3 varyant
Şimdiki resimde kalış ≈ 0,29 sn. Zeyd daha kısa istiyor: "geldi gitti" hissi.

| Varyant | Kalış | Bayrak |
|---|---|---|
| **B vol 1** | **0 sn**: varış karesinde geri çekilme başlar | `&resimbekle=0` |
| **B vol 2** | **1/4**: ~0,07 sn | `&resimbekle=0.07` |
| **B vol 3** | **1/2**: ~0,145 sn | `&resimbekle=0.145` |

**Kurallar:**
- **Hız sürekliliği:** gidişin sonu → (kalış) → geri çekilme. Kalış 0 olsa bile sert dönüş ("sekme") olmasın. Varışta hız kısa bir anda sıfıra iner, geri çekilme yumuşak kalkışla başlar. Gerekirse varıştaki değme (%2,5) vol 1'de kısalsın. Ekran hızı grafiğinde kırık yok.
- **Çizgiler:** şimdiki kurala göre çizgiler kalışta başlayıp geri çekilmede bitiyor. Kalış kısalınca çizgiler **varış anında** başlasın ve geri çekilmenin ~%80'inde tamamlansın. Çizgiden tahtaya devir kuralı (PM_84/86, piksel farkı) aynen.
- **Ses:** dalış whoosh'u ve çizgi sesi (c) yeni zamanlamaya uysun. Kalış 0'da sesler üst üste binip patlamasın.

## Önizleme
- `docs/art/pm111/kalis_yan_yana.mp4`: şimdiki (0,29) | vol 1 | vol 2 | vol 3. Dört panel yan yana, aynı anda başlar, sesli, 60 fps.
- Ayrı ayrı: `kalis_vol1.mp4`, `kalis_vol2.mp4`, `kalis_vol3.mp4` (390×844, sesli, ≤ 10 MB).
- `hiz.png`: dört varyantın ekran hızı, varış ve dönüş bölgesi yakın plan.
- Toplam süre tablosu (Oyna → bölüm başlar): şimdiki ≈ 1,81 sn.

## Teslim
- EditMode yeşil (yerel). Push ve yayın yok.
- Son mesaj: süre tablosu, video yolları.
