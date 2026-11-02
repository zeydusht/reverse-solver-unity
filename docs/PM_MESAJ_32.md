# PM → Claude Code (2 Kasım 2026, 21:35) — Tanıtım bölümleri 10 ve 14'ü sadeleştir (p20)

**İLK İŞ:** Bu dosyayı ve `docs/` altındaki takip edilmeyen PM dosyalarını git'e ekle ve commit et ("docs: PM_MESAJ_32"). Daha önce PM'in eklediği bölümler çökme sonrası "yarım değişiklik" sanılıp geri alınmıştı. **PM dosyalarını asla geri alma ya da silme.**

Davetler henüz gitmedi. Bu iş bitince PM iki bölümün tahminini kilitleyecek, sonra davet gidecek.

## Neden
Zeyd: "10. bölüm zincir tanıtımı ama her engel var ve 3 zincir." Tanıtım bölümünde yeni engel tek başına öğretilmeli. 14 (duvar tanıtımı) da aynı durumda.

## Değişiklik

| Slot | Şu an | Yeni sürüm (`version` 1 → 2, aynı id) |
|---|---|---|
| 10 · Z24 (zincir tanıtımı) | 6×7, 3 zincir, 1 bomba (pay 2), 2 çivi, 65 sn | **1 zincir çifti**, **bomba yok**, çivi en fazla 1, süre ≥ 75 sn. Tahta 5×6 ya da 6×6 yeterli. Mümkünse zincirli iki parçadan biri, ötekinin yolu açılmadan çıkamasın ("ikisinin yolunu aç" kuralı yaşansın). |
| 14 · Z21 (duvar tanıtımı) | 6×7, 3 zincir, 1 bomba (pay 2), 2 çivi, 1 duvar, 15 tuzaklı adım | **1 duvar şeridi**, çözümde en az 2 parçanın kolay yönünü kapatsın. En fazla 1 zincir, **bomba yok**, çivi en fazla 1, süre ≥ 75 sn, tuzaklı adım ≤ 3. |

**Nasıl:** Mevcut bölümden sadeleştir: fazla zincirleri tekli parçaya çevir, bombayı kaldır, gerekirse tahtayı küçült. Ya da fabrika ile üret. Görsel tablo aynı kalsın (slot 10 ve 14'ün tabloları).

**Hedef his:** Zeyd'in puanıyla 2–3. Yeni oyuncu ilk denemede büyük ihtimalle geçer.

## Kontrol (hepsi şart)
- SmartPlayer çözüyor, kayıtlı çözüm kazanıyor, 40/40 açılış testi geçiyor.
- FunSim ilk deneme + rastgele kazanma + k0 / tuzak metrikleri → `docs/veri/intro_v2.csv`. Sütunlar `level_sim_v4.csv` ile aynı, `first` ve `mc` dahil.
- `python Tools/veri/level_features.py` çıktısının yalnız bu iki satırı → `docs/veri/intro_v2_features.csv`.
- Eğitim adımları:
  - Bölüm 10 zincir spotlight'ı yeni bölümdeki tek çift.
  - Bölüm 14 duvar spotlight'ı tek şerit.
  - Tanıtım kartları aynı.
- p20 bölümleri hangi dosyadan okuyorsa onu güncelle. Kök Build 5 aynı dosyayı okuyorsa **kökü değiştirme**, bunun yerine p20'ye ayrı yol ver ve OVERNIGHT'a yaz.
- Canlı `?c=ic`: 10 ve 14'ü bir kez oyna. Supabase'de `level_version = 2` görünüyor mu?

## Bitince
OVERNIGHT.md başına:
- iki bölümün yeni özeti (boyut, zincir, çivi, duvar, süre, FunSim ilk deneme)
- iki CSV'nin yeri

PM tahminleri yükleyecek.
