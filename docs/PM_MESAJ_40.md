# PM → Claude Code (3 Kasım 21:50): davet öncesi son 4 madde (p20)

**İlk iş:** commit et. PM dosyalarını asla geri alma.

**PM kararları:**
- +1,64 MB onaylı (16,09 MB). Panelleri yeniden çizmek yok.
- `redesign` aracı, ZEYDBOT, İ06/İ08, a6b/card: davetten sonra.

## 1. Sürüm etiketi değişmemiş (veri)
Supabase'de TEST_P39_DV (18:26–18:29 UTC) satırları hâlâ `app_version = web-p20-2026-11-02`. Yeni build verisi eskisinden ayrılamıyor.
- Etiketin gerçekten nerede okunduğunu bul ve `web-p20-2026-11-03` yap.
- Canlıda bir bölüm oyna, Supabase'de etiketi gör.

## 2. 3. bölümdeki jam test aracından değil, eğitim anından
PM incelemesi (`moves`, TEST_P39_DV, S43):
- jam, `t_ms` 16111'de, `time_left` 60'ta, piece 17'de oluyor. Süre henüz başlamamış, yani tanıtım kartı ya da spotlight sırasında.
- Ardından gerçek çözüm başlıyor.

Sebep ya kart kapanırken dokunuşun tahtaya geçmesi (PM_38 kilidi bu durumu kapsamıyor), ya da spotlight sırasında çivili parçaya dokunulması. Gerçek oyuncuda da olur, zorluk verisini kirletir.

Yapılacaklar:
- **Süre başlamadan** (kart, spotlight, eğitim adımı açıkken) tahtadaki hiçbir dokunuş hamle ya da jam olarak sayılmasın, `plays.jams`'e eklenmesin.
- Eğitim adımının kendisi bir sürükleme istiyorsa (bölüm 1'deki "takıldı" adımı gibi), o adım `moves`'a `kind = 'tut'` diye yazılsın, `jams`'e eklenmesin.
- Hangisi olduğunu bul ve OVERNIGHT'a yaz.

## 3. 6. bölümde bomba tanıtımı yerine profil sorusu çıkıyor
Oyuncu bombayı açıklamasız görmemeli.
- Sıra şöyle olsun: 5. bölüm biter → (profil sorusu bu noktadaysa) profil sorusu → 6. bölüm açılır → **bomba tanıtım kartı** → oyun.
- İki kart aynı anda açılmasın; biri diğerini yutmasın.

## 4. Eksik kontroller (PM_39'dan)
- Supabase `zorluk_puani` 7 testi: bölüm 2'yi kazan, 7 seç, satırda 7'yi gör.
- 375×667'de şu kartların sığdığını ekran görüntüsüyle göster:
  - kazanma
  - bomba kaybı
  - bir tanıtım kartı
  - bir booster kartı
  - isim
  - profil sorusu
- Ekran görüntüleri: bölüm 5 (makas), bölüm 28 (bomba kaybı), koleksiyon, koridor.

## Bitiş
- p20'ye yayınla.
- Ekran görüntüleri ve sonuçlar: `docs/qa/2026-11-03-son-kontrol/`
- OVERNIGHT başına 2 satır. Sonra durup Zeyd'e "bitti" de.
