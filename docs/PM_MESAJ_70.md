# PM_MESAJ_70: Zorluk modeli v2'yi üreticiye bağla + p22'de 13–40 ayarı

Kodu kendin yaz. Yalnız p22 (p20, p21, kök, pt1'e dokunma). Önce `docs/` commit. **Supabase'e yazma yok.**

Arka plan: `docs/analiz/ZORLUK_MODELI_v2.md` §5. Model dosyaları: `docs/analiz/v2/model_v2.json`, `final.py`, `pool_scored.csv`.

## 1. Model v2'yi koda taşı
`Assets/_Game/Generation/DifficultyModelV2.cs`, `DifficultyV2` ölçüleri üstünde çalışır.

- **Kazanma (ortalama oyuncu, ilk deneme):**
  - `B = −logit(0,85·p_survive_aware + 0,15·p_survive_rand)`. Bombasızda p = 1. `logit(p) = ln((p+0,01)/(1,01−p))`.
  - `d = 1,61 + 0,53·B − 1,71·ln(sec_per_move) − 1,84·(narrow_by_chain/pieces)`.
  - `p_win1 = 1/(1+e^d)`.
- **Düşündürme:** `jam_per_exit = 0,18 + 2,07·nl + 0,57·nw + 0,56·nc + 0,40·dir_ambiguity`, burada `nl`, `nw`, `nc` = `narrow_by_*/pieces`.
- **Test:** `pool_scored.csv`'deki `p_pred` değerlerini ±0,01 içinde üretsin (250 bölüm).
- Üretici ve ayar araçları zorluk tahmini olarak bunu kullansın. Eski FunSim tahmini raporda yan sütun olarak kalsın.

## 2. Tasarım kuralları (kitaba yaz: `docs/kit/KIT_NOTLARI.md` + üretici kısıtları)
- **Boğaz:** normal bölümde `choke_n` ≤ 1; zor bölümde ≤ 2; "ezber merdiveni" (3+ boğaz) yok.
- **Açık tahtada bomba:** `distract_per_step` ≥ 5 ise `p_survive_rand` ≥ 0,25 ya da `slack_true_min` ≥ 8 olsun. Sıkı bombayı (pay ≤ 2) daraltılmış tahtaya (dar an çok, çeldirici az) koy.
- **Süre:** `sec_per_move` 1,7–2,3 arası. Zor bölümde süreyi kısmak yerine engel (dar an) ekle.
- **Düşündürme:** 21–40 arasında her bölümde `jam_per_exit` tahmini ≥ 0,5 (kilit, zincir veya duvar dar anı en az birkaç adım). Bombasız bölümler de düşündürsün.

## 3. Hedef eğri (p22, model v2 tahmini)
| Aralık | Normal | Zor (kartlı) | Nefes |
|---|---|---|---|
| 11–20 | ,55–,75 | ,35–,45, en çok 2, art arda değil | — |
| 21–30 | ,50–,70 | ,35–,45, en çok 2, art arda değil | zorun hemen ardından 1 tane ≥ ,75 |
| 31–40 | ,45–,65 | ,30–,45, en çok 3 (40 dahil, 40 ≈ ,35) | 1–2 tane ≥ ,75 |

Ders ve yumuşak bölümler (1–10, 14) aynı kalsın. Hepsi zaten hedefte.

Şu anki tahminler ve yapılacaklar:
- **11–20:**
  - 13 S34 ,42 → zor olarak kalabilir (kart ekle) **ya da** normal yap. 16 Z04 ,49 ile 20 S24 ,49 normal aralığa çek (+fitil). Sonuçta zor en çok 2 olsun, art arda olmasın.
  - 15 S13 ,80 biraz kolay → ,70 civarı.
- **21–40:** pt1 hâli; çoğu %6–27 (21 Z23, 25 S35, 29 S37, 32 S39, 33 S32, 35 S29, 37 S38, 40 S33).
  - 39 S28 v2 (PM_68'de zor diye taşınmıştı) modelde ,75. Rolüne göre ayarla.

## 4. Ayar sırası (her bölüm için)
1. Fitil (boğaz sayısını azalt, payı aç).
2. Süre.
3. Bombalı parçayı tahtada başka parçaya taşı. Daraltılmış bölgeye koymak hem payı hem çeldiriciyi düzeltir.
4. Engel ekle veya çıkar (eksen B).
5. Olmuyorsa havuzdan (`pool_scored.csv`, kullanılmayan setler) hedefe uyan bölümle değiştir.

Kurallar:
- Tahta düzenini gerekmedikçe değiştirme.
- Her değişen bölüm yeni sürüm numarası alsın; kayıt/sayım uyumluluğu PM_59/64 kuralıyla korunsun.

## Teslim
- p22 güncel, EditMode yeşil (yeni test: p22 13–40 hedef aralıkları + kural kontrolleri), `verify_build` p22 4/4.
- Rapor `ENTEGRASYON_RAPOR.md`'ye "PM_70" eki. Tablo: slot · bölüm · rol · önce/sonra `p_win1` · `jam_per_exit` · `choke_n` · yapılan değişiklik.
- `PROJE_DURUMU`'na tek satır, commit + push. Sonra **dur**.
