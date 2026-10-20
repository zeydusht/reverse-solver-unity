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

## Oturum 3 sonucu (20 Ekim 07:49–07:58, "ddd") — dondurulmuş tahminler (fc4ef7c) ne dedi

Kaynak: `node Tools/zeydbot_fetch.js --all` (Supabase plays/moves, anon REST: `plays?level_id=like.Z*&player=in.(TEST_ZEYD,ddd)`) → `ham_plays.csv`; hesap `python Tools/zeydbot_score_session3.py` (tahminleri `git show fc4ef7c:` ile okur). Hücre: kazanma / kalan s / puan.

| Bölüm | Gerçek: sonuç / kalan s / puan | ZeydBot | Rastgele | Bomba payı |
|---|---|---|---|---|
| Z21 | win / 32 / 3 | %100 / 35 / 2.6 | %65 / 30 / 5.6 | %69 / 31 / 5.5 |
| Z22 | win / 39 / 2 | %100 / 38 / 2.5 | %67 / 30 / 5.6 | %69 / 31 / 5.5 |
| Z23 | win / 82 / 5 | %82 / 87 / 3.6 | %53 / 66 / 5.9 | %56 / 66 / 6.1 |
| Z24 | win / 22 / 3 | %100 / 35 / 2.6 | %76 / 31 / 5.3 | %69 / 31 / 5.5 |
| Z25 | win / 82 / 4 | %92 / 83 / 3.0 | %61 / 65 / 5.7 | %56 / 63 / 6.1 |
| Z26 | win / 86 / 6 | %80 / 102 / 3.7 | %93 / 88 / 4.5 | %56 / 74 / 6.1 |
| Z27 | win / 26 / 1 | %100 / 24 / 2.3 | %100 / 25 / 1.0 | %99 / 25 / 1.0 |
| Z28 | win / 45 / 1 | %100 / 40 / 2.4 | %100 / 47 / 1.0 | %99 / 47 / 1.0 |
| Z29 | win / 64 / 6 | %0 / 0 / 8.7 | %53 / 50 / 5.9 | %56 / 50 / 6.1 |
| Z30 | win / 37 / 1 | %100 / 33 / 2.4 | %100 / 40 / 1.0 | %99 / 40 / 1.0 |

| Ölçüt | ZeydBot | Rastgele | Bomba payı |
|---|---|---|---|
| Puan sırası (Spearman, hedef ≥ 0,7) | 0.99 | 0.74 | 0.96 |
| Kazandı/kaybetti (eşik %50) | 9/10 | 10/10 | 10/10 |
| Kalan s ±%25 (10 kazanma) | 8/10 | 9/10 | 9/10 |

- **Önceden kayıtlı ölçüt (Spearman ≥ 0,7) tuttu: 0,99.** Ancak bomba payı tabanı da 0,96 yaptı. n = 10 ve 10 bölümün hepsi kazanıldığı için fark gürültü içinde.
- Kazandı/kaybetti: bot 9/10. Z29 için %0 dedi, Zeyd 64 s kala kazandı. Tabanlar 10/10, çünkü hepsi kazanma dedi.
- Kalan s ±%25: bot 8/10, tabanlar 9/10.
- Zeyd: "Bugünküler o kadar zor gelmedi, 12 gibisi gelmedi bile." Oturum 3'ün en yüksek puanı 6.
- Veri notları: Z03 deneme 3 (quit, 20 Ekim) atıldı. Z20'nin hamle satırları 29'dan 35'e çıktı (gece indirildiğinde son hamleler henüz gönderilmemişti); v2 yeniden oynatmayı bu tam veriyle yapar.
