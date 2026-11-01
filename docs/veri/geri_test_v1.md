# Geri test v1 (PM_MESAJ_25 adım 14)

Veri: gerçek oyuncuların ilk denemeleri (yarıda bırakılanlar hariç), 91 deneme, 78 bölüm sürümü, 1 oyuncu (zeyd). Kazanma 80/91. Bot: 300 sanal oyuncu/bölüm.

**Oyuncu dışarıda bırakma yapılamadı:** veride tek gerçek oyuncu var (Zeyd'in isimleri tek oyuncu sayıldı). pt1 gelince aynı komut bunu da yapar.

| Kat | Model | n | Brier ↓ | MAE ↓ | Spearman (p, sonuç) ↑ |
|---|---|---|---|---|---|
| bölüm dışarıda | v1 botlar | 91 | 0.208 | 0.289 | 0.39 |
| bölüm dışarıda | sabit ortalama | 91 | 0.109 | 0.216 | -0.67 |
| bölüm dışarıda | yalnız fitil payı | 91 | 0.095 | 0.182 | 0.34 |
| v0 ile ortak bölümler | v1 botlar | 41 | 0.249 | 0.327 | 0.28 |
| v0 ile ortak bölümler | v0 | 41 | 0.135 | 0.285 | 0.22 |
| v0 ile ortak bölümler | sabit ortalama | 41 | 0.091 | 0.198 | -0.55 |

Zeyd'in 1–10 puanıyla sıra korelasyonu: v1 tahmini puan ρ = 0.86 (n 73); v0 p_win1 (ters) ρ = 0.88 (n 30).

**Hüküm:** v1 tabanları GEÇEMİYOR (sabit ortalamadan kötü) (v1 0.208, sabit 0.109, fitil 0.095).
Not: veri tek oyuncudan (uzman tasarımcı, kazanma oranı yüksek); sınıflar dengesiz, n küçük. Bu test boru hattının çalıştığını gösterir; v1'in asıl değeri pt1 verisiyle ölçülecek (VERI_SISTEMI 7.3).
