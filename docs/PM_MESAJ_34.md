# PM → Claude Code (3 Kasım 2026, 00:05) — Bölüm 1 eğitimi: elin sırtı + doğru parça (p20)

**İlk iş:** bu dosyayı commit et, PM dosyalarını geri alma.

Zeyd p20'yi denedi, iki düzeltme istiyor. Davetler bu işten sonra gidecek.

## 1. El: avuç içi değil, elin SIRTI görünsün
- Şu anki eldiven önden (avuç tarafından) görünüyor. Zeyd elin **arkasını** görmek istiyor: mobil oyunlardaki standart eğitim eli.
  - İşaret parmağı yukarı uzanmış, **tırnağı görünür**.
  - Parmak eklemleri ve elin sırtı bize bakıyor.
  - Diğer parmaklar kıvrık, başparmak yanda.
  - El dik, bilek aşağıda.
- **Codex'e yeni el çizdir:** `Assets/Art/Tutorial/hand-back-v1.png` (normal) + `hand-back-v1-press.png` (basarken hafif küçülmüş, parmak ucu hafif bükük).
  - Açık ten ya da krem-beyaz eldiven.
  - Koyu kalın kontur, yumuşak gölge, `docs/art/STIL_REHBERI_RM.md` dili.
  - Şeffaf PNG, @3x ≈ 360 px yükseklik.
  - Pivot parmak ucunda (manifestte yaz).
- Oyunda tüm eğitim ve booster eğitimi adımlarında bu el kullanılsın. El dönmesin.

## 2. Bölüm 1, adım 2–3: SAĞ ALTTAKİ parça
- Zeyd şunu istedi: **en sağ alttaki parça** (sağ alt köşe hücresi). Önce **sağa** çekilir, takılır ("Takıldı!…"). Sonra **aşağı** çekilir ve çıkar.
- Şu an sol alttaki parça kullanılıyor, yanlış.
- **Önce oyun koduyla kontrol et** (adım 1'den sonraki tahta durumunda):
  - Sağ alt parça sağa çekilince gerçekten takılıyor mu?
  - Aşağı çekilince çıkabiliyor mu?
- İkisi de doğruysa bu parçayı kullan. Değilse en yakın uyan parçayı seç (sağ alt köşeye en yakın, sağa takılan ve aşağı çıkan). Neyi seçtiğini ve nedenini OVERNIGHT'a yaz.
- Kayıtlı çözüm sırasının bozulmaması gerekiyorsa adım 3'ten sonra serbest oyunda çözüm yine kazanılabilir olmalı. SmartPlayer ile doğrula: eğitim hamlelerinden sonra bölüm çözülebilir mi?

## Kontrol
- Bölüm 1 eğitiminin 3 adımının kare dizisi → `docs/qa/2026-11-03-el/`. Elin sırtı görünüyor mu? Adım 2–3 sağ alt parça mı?
- Canlı `?c=ic&unlock=1` kontrolü.
- OVERNIGHT.md başına 2 satır.
