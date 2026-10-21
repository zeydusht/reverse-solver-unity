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

## Oturum 4 sonucu (20 Ekim 23:54 – 21 Ekim 00:02, "ddd") — dondurulmuş tahminler (6b602ba)

Kaynak: `node Tools/zeydbot_fetch.js --all` → `ham_plays.csv`; hesap `python Tools/zeydbot_score_session4.py` (tahminleri `git show 6b602ba:` ile okur). İlk gerçek deneme = 0 hamlelik quit sayılmaz (Z32 deneme 1).

| Bölüm | Gerçek: ilk gerçek deneme / kalan s / puan | v1: puan / kazanma / kalan s | Ajan: puan / Z12 gibi mi |
|---|---|---|---|
| Z31 | win (deneme 1) / 24 / 9 | 8.1 / %0 / — | 8 / belki (evet'e yakın) |
| Z32 | win (deneme 2) / 66 / 6 | 6.1 / %38 / 66 | 6 / hayır |
| Z33 | win (deneme 1) / 29 / 8 | 8.1 / %0 / — | 8 / belki |
| Z34 | bomb (deneme 1) / — / 5 | 8.2 / %0 / — | 7 / belki |
| Z35 | win (deneme 1) / 14 / 9 | 8.1 / %0 / 53 | 7.5 / belki |

Puan sırası (Spearman, 5 bölüm): v1 -0.23, ajan 0.66
Kazandı/kaybetti (eşik %50): v1 1/5
Ortalama mutlak puan hatası: v1 1.04, ajan 0.90

- **Oturum 3'ün en yüksek puanı 6 idi; oturum 4'te üç bölüm 8–9 aldı** (Z31 9, Z33 8, Z35 9). Üçü de Z12 türevi; Z23 ailesi (Z32 6, Z34 5) daha düşük kaldı.
- Ajanın tahmini v1'den iyi: puan sırası 0,66'ya karşı −0,23; ortalama hata 0,90'a karşı 1,04. n = 5, gürültülü.
- v1 Z12 türevlerine "%0 kazanma" dedi, Zeyd üçünü de ilk denemede kazandı. Bu, v1'nin yanıltıcı parçayı görmediği bulgusunu doğruluyor.
- Z34'ün puanı (5) 3. denemeden geliyor. 1. denemenin (bomba) satırında puan yok, çünkü Zeyd 9 s sonra "Tekrar dene"ye bastı. Uçtan uca kontrol (`verify_build.js --send --only zeydlosssend`, canlı Build 5z4) kayıptan sonra verilen puanın satıra yazıldığını gösterdi (`Z03 time, zorluk_puani 1`). Yani gönderim çalışıyor, cevap atlanmış. Düzeltme: ZeydBot setinde kayıp kartındaki tekrar düğmeleri puan verilene kadar gizli; Menü açık.

## Oturum 5 sonucu (21 Ekim, "ddd") — dondurulmuş tahminler (d0bf214)

- **Puanlar:** Z36 9, Z37 9, Z38 9, Z39 8, Z40 9.
  - Z40'ta ilk denemede süre bitti ve kayıpta puan geldi (9). Kayıp kartı düzeltmesi çalışıyor.
  - 5 bölümün 5'i 8 ve üstü aldı; ölçüt tuttu.
- **Kalan süre:** 49 / 32 / 34 / 47 / — saniye. Tahmin her biri için 17'ydi.
  - M15, Zeyd'in hızını olduğundan yavaş varsaydı. Gerçek kalan pay %27–41.
- **Puan sırası:** M13 puanının Spearman'ı 0,73, ajanınki 0,56. Ortalama hata 1,08 ve 1,20; ikisi de puanı düşük tahmin etti.
- **Benzerlik** (`python Tools/level_similarity.py`, döndürme ve aynadan bağımsız):
  - Z36/Z38/Z40 Z12'ye %88–91 benziyor, kendi aralarında %85–94.
  - Z37/Z39 Z23 ailesine %80–91 benziyor.
  - Bağımsız 7×8 bölümler arası benzerlik %42–64. Bu taban, tahtaların hepsinin tam dolu 7×8 olmasından geliyor.
  - Zeyd'in "90 derece dönmüş" hissi ölçümle doğrulandı. Hepsi 7×8, 54 parça.
  - **M16 eşiği önerisi: %65.**
