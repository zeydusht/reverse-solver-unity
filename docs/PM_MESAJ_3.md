# PM → Claude Code (21 Ekim 2026, 00:15) — Oturum 4 sonucu ve sıradaki iş

## Oturum 4 sonucu (Supabase, oyuncu "ddd", 20 Ekim 23:54 – 21 Ekim 00:02)

| Bölüm | İlk gerçek deneme | Kalan s | Puan |
|---|---|---|---|
| Z31 | kazandı | 24 | **9** |
| Z32 | kazandı (1. kayıt 0 hamle quit, sayma) | 66 | 6 |
| Z33 | kazandı | 29 | **8** |
| Z34 | bomba (2. quit, 3. kazandı, 61 s) | — | 5 |
| Z35 | kazandı | **14** | **9** |

Oturum 3'ün en yüksek puanı 6 idi; bu oturumda üç bölüm 8–9 aldı.

**Yöntem kabul edildi:** tasarımcı ajan + fabrika doğrulaması + ZeydBot. Karar kaydına işle.

Önce dondurulmuş tahminlerle (bot + ajan) karşılaştır ve RAPOR.md'ye "Oturum 4 sonucu" olarak ekle.

**Hata:** Z34 bomba kaybında `zorluk_puani` boş. Kayıptan sonra puan sorusu ya çıkmıyor ya da gönderilmiyor. Düzelt.

## Zeyd'in geri bildirimi (özet, sözleri korunarak)

### 1. KÖTÜ — bomba çözümü kalıplaştı
> "Bombanın çıkması gereken bir yatay veya dikey oluyor… O yerin en ucuna ard arda 6–7 tane mesela sağa doğru itmeyle çıkan parçalar yerleştiriyor. Tak tak tak, onları çıkarttıktan sonra önü açılmış oluyor ve 4–5 bomba/zincirli parça dümdüz yukarı itmeyle çıkıyor… Bir kere bulunca bütün bölümleri bomba zorluğunu ona göre ayarlamış."

Sorun şu: bütün bölümlerde aynı şablon var. Bir şerit art arda süpürülüyor, ardından bomba ve zincirler tek yönde, düz bir şeritten çıkıyor. Oyuncu şablonu bir kez öğrenince bomba zorluğu kayboluyor.

### 2. ÇOK İYİ — "tek anahtar parça" anları (en değerli bulgu)
> "Bomba çıkardıktan sonra kalan kısımlar çok iyiydi. Hangi parçayı çıkarmak gerekiyor bilemedim, ekrana baktım, doğruyu bulamadım. O an bütün oyunu çözen sadece bir parça oluyor… Onu bulduktan sonra bir tane daha çıkartıyorsun ve tekrardan en doğru parçayı araman gerekiyor… Oyun sadece obstacle'larla zorlaştırılmıyor; o an çözüme giden tek bir yol oluyor ve onu bulman gerekiyor."

### 3. ÇOK İYİ — sonda süreyle kapışma
> "15 saniye falan kalmıştı, başarabilecek miyim hissiyatı oluştu. Gerçekten iyiydi." (Z35: 14 s kala.)

## Tasarım tarifine eklenecekler (Z12_TARIFI.md → v2)

### M13 — Anahtar parça anı (YENİ, birincil keyif metriği)
- **Tanım:** Tahtada görünür seçenek çok, ama ilerleten hamle tek. Yani o anda serbest ya da serbest görünen parça sayısı ≥ 4 ve çözümü ilerleten hamle sayısı = 1.
- **Önemli ayrım:** Tek serbest parçanın olduğu an anahtar an DEĞİLDİR; orada aranacak bir şey yok, cevap ortada. Keyif, kalabalığın içinde tek doğruyu aramaktan geliyor.
- **Hedef (Z12 tipi):** Oyunun orta ve son yarısında art arda gelen anahtar anlar. Yaklaşık 3–6 tane, aralarında 1–3 rahat hamle.
- **Kontrol:** Z31, Z33 ve Z35'te (puan 8–9) Zeyd'in think_ms ve touch verisi bu anlarda sıçrıyor mu? Z32 ve Z34'te bu anlar daha mı az? Kalibrasyonu buradan yap.

### M14 — Bomba çözümü çeşitliliği (YENİ, set düzeyinde)
- Bombanın kaçış yolu tek bir süpürmeyle açılmamalı. Aynı şeritten aynı yöne art arda en fazla 2 çıkış olmalı (M4 bomba yolunda da geçerli).
- Bombanın yolu en az iki farklı yönden açılan parçalardan geçmeli.
- Bomba ve zincirli parçalar aynı düz şeritten art arda çıkmamalı.
- **Set içinde şablon tekrarı yasak:** Ardışık 5 zor bölümde bomba çözüm deseni (şerit yönü, süpürme uzunluğu, bombanın çıkış yönü) birbirinden farklı olmalı. Desen imzası tanımla ve ölç.

### M15 — Süre gerilimi
- **Hedef:** Zeyd'in kazandığı Z12 tipi bölümlerde bitişte kalan süre limitin %10–20'si olmalı. Z35'te bu oran 14 s idi.
- ZeydBot'un kalan saniye tahmini bu hedefe göre ayarlanır. Gerilim süre limitinden gelmeli, bomba ise "düşünme" baskısı yaratmalı.

## Sıradaki iş

1. Oturum 4 karşılaştırması ve puan sorusu hatası (yukarıda).
2. Z12 tarifi v2: M13, M14 ve M15 eklenir. Fabrika bu üçünü ölçer, ZeydBot puanına M13 özellik olarak girer.
3. **Z36–Z40:** Tarif v2 ile 5 bölüm. Hedef: anahtar anlar korunacak, bomba şablonu kırılacak, her bölümde bomba çözüm deseni farklı olacak. Tahminler dondurulur.
4. Z36–Z40 sonucu iyiyse (≥ 3 bölüm puan ≥ 8 ve Zeyd "şablon yok" derse) **40 bölümlük yeni set** bu yöntemle ve eğri önerisiyle kurulur. Bu ayrı bir PM onayı gerektirir.
5. ZeydBot v2 (PM_MESAJ.md 1–3): Z31–Z35 verisini de ekle.

## Kurallar

- Canlı 40'a, kök adrese ve bayraklara dokunma.
- Şema değişikliği gerekirse SQL taslağı yaz, PM çalıştırır.
- Zeyd'e soru yok.
- Bitince OVERNIGHT.md başına en fazla 8 satır ve Z36–Z40 linkleri.
