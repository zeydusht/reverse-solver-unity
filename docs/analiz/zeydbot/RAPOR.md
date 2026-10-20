# ZeydBot v1 — gece raporu (2026-10-20)

## Ne yapıldı (commitler)

| Adım | Sonuç | Commit |
|---|---|---|
| 1 Yeniden oynatma | Z01–Z20, 26 deneme, 1137 hamle yeniden oynatıldı. Her hamle için doğru/tuzak/seçenek sayısı, bomba başına pay, düşünme/dokunma/basılı tutma. Ham ve türetilmiş CSV'ler `docs/analiz/zeydbot/` altında | (adım 1 commit'i) |
| 2 Zorluk analizi | `zorluk_analizi.md` | fd806f6 |
| 3 ZeydBot v1 | `Tools/zeydbot_model.py` + `ZeydBot.Paths` (yollar.csv); LOO `loo.md` | fc4ef7c |
| 4 Tahminler donduruldu | `tahmin_Z21_Z30.md`, oturum 3 kilidi açıldı | **fc4ef7c** (03:26) |
| 5 Eğri önerisi | `zorluk_egrisi_onerisi.md` | 1ed59d8 |
| 6 Bonus Z31–Z38 | **Yapılmadı.** Bot taban modeli geçemedi, filtre olarak kullanmanın gerekçesi yok; ayrıca bot yolları 1,5 saat sürdü | — |
| 7 Bağımsız denetçi | Bulgular aşağıda | — |

## ZeydBot nasıl çalışıyor (sade)

1. Unity, her bölümde 12 kez "hep doğru hamleyi seçen" ve Zeyd gibi bombaya yönelen bir oyuncu oynatır. Her adımda kaç seçenek olduğunu, kaçının doğru olduğunu ve bomba payını kaydeder.
2. Python, bu adımların üstüne Zeyd'in alışkanlıklarını koyar:
   - Tuzak varken Zeyd'in tuzağa basma olasılığı (30 fırsatta 5 tuzak).
   - Düşünme süresi: seçenek çok olunca kısalır, tek doğru hamle varsa uzar.
   - Süre biterse kayıp sayılır.
3. Zeyd'in kaybetme olasılığı ve sürenin ne kadarını harcadığı, 1–10 puana doğrusal olarak çevrilir.

## Leave-one-out (Z01–Z20) — tabanları geçti mi?

| Ölçüt | ZeydBot | Rastgele oyuncu | Bomba payı |
|---|---|---|---|
| Puan sırası (Spearman, 18) | 0,73 | 0,61 | **0,79** |
| Kazandı/kaybetti doğruluğu | %70 | %60 | %70 |
| Brier (düşük iyi) | 0,233 | **0,162** | 0,203 |
| Kalan s ±%25 içinde | %73 | **%93** | **%93** |

**Açık cevap: ZeydBot v1 bomba payı tabanını geçmedi.** Rastgele oyuncuyu yalnızca puan sırasında geçti. Kalan saniyede tek sayılık tabanlar daha iyi. İlk sürüm (rastgele doğru hamle seçen bot, `loo_v0_rastgele_dogru.md`) daha kötüydü. Asıl kazanç bulgunun kendisi: Zeyd bombayı hemen kazıyor, bu yüzden bot da böyle oynamalı.

## Denetçi bulguları

- **Kritik:** yok. Test setine sızıntı yok: Z21–Z30 sonuçları indirilmedi ve okunmadı.
- **Önemli 1:** Kaybedilen 5 bölümde (Z07, Z12, Z13, Z16, Z17) 1–10 puan 2. denemeden geliyor, çünkü soru kazanınca soruluyor. Puan "kaybettim sonra geçtim" deneyimini ölçüyor.
- **Önemli 2:** Bombaya yönelme oranı tüm 20 bölümden hesaplanıp LOO'da sabit kullanıldı. LOO bu yüzden biraz iyimser.
- **Önemli 3:** Benim yazdığım "359/377" yanlıştı. Dosyadan yeniden üretilen değer 257/275 = %93,5. Modelde 0,95 kullanıldı; dondurulduğu için değiştirilmedi, yorum düzeltildi.
- **Küçük:** Z01/Z07 (kör olmayabilir) metriklere dahil. Tür (kolay/orta/zor) puanı büyük ölçüde açıklıyor ve tür içinde ayırt etme gücü sınırlı (n = 18).

## Sıradaki (v2, `zeydbot-v2` dalında, oturum 3'ten sonra)

- Oturum 3 sonuçları ile dondurulmuş tahminler aynı ölçütlerle karşılaştırılacak (bot ve iki taban).
- Puan sorusu kayıptan sonra da sorulmalı. Bu oyun değişikliği, PM'in kararı.
- Bombaya yönelme oranı LOO içinde her kat için ayrıca hesaplanmalı (yolları yeniden üretmek gerekir, ~1,5 saat).
