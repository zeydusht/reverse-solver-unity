# PM → Claude Code (21 Ekim 2026, 09:00) — Oturum 5 sonucu + 40'lık set kararı

## Oturum 5 sonucu (Supabase, "ddd")

| Bölüm | Sonuç | Kalan süre | Puan |
|---|---|---|---|
| Z36 | 1. denemede kazandı | 49 sn | 9 |
| Z37 | 1. denemede kazandı | 32 sn | 9 |
| Z38 | 1. denemede kazandı | 34 sn | 9 |
| Z39 | 1. denemede kazandı | 47 sn | 8 |
| Z40 | 1. denemede süre bitti, 2. denemede kazandı (39 sn kala) | — | 9 |

Zeyd'in yorumu: "Bölümler iyiydi, beğendim. Zorluğu iyiydi." Bomba şablonu şikâyeti gelmedi.

**YENİ SORUN (AYNEN):** "Sanki birbirinin birebir aynısı bölümleri çözüyorum hissiyatı oluştu. Sanki sadece ekran 90 derece dönmüş, yandan çözüyormuşum gibiydi."

PM yorumu: Muhtemelen M14'ü (bomba deseni çeşitliliği) aynı tasarımı döndürüp aynalayarak geçtik. Metrik geçti ama oyuncu aynı bölümü tanıdı.

**Önce bunu doğrula:**
- Z36–Z40'ı döndürme ve aynalamadan bağımsız olarak karşılaştır: tahta, engel yerleri, çözüm sırası.
- Benzerlik skorunu raporla.
- Hepsi 7×8 / 54 parça mı?

Ardından RAPOR.md'ye "Oturum 5 sonucu" ekle: tahminler ile gerçek sonuçlar, 3–5 satır.

## Karar: 40 bölümlük yeni set bu yöntemle kurulur (Zeyd onayladı)

Önceden koyulan ölçüt tuttu: oturum 4'te 3/5, oturum 5'te 5/5 bölüm 8+ puan aldı.

### Çeşitlilik kuralları (M16, YENİ — set düzeyinde zorunlu)

1. **Döndürme ve aynalama benzerliği:** Setteki hiçbir bölüm çifti, döndürme ve aynalamadan bağımsız ölçüldüğünde %X'ten fazla benzer olamaz. Eşiği öner, ölçerek koy.
2. **Tahta çeşitliliği:**
   - Farklı boyutlar kullanılır. Zor bölümler 7×8'e sıkışmasın; 6×7, 7×7, 8×8 gibi boyutlar da olsun.
   - Parça sayısı değişsin.
   - Mümkünse dikdörtgen olmayan tahtalar olsun: köşe boşlukları, delik.
3. **Fikir çeşitliliği:**
   - Her zor bölümün tek cümlelik bir "fikri" olur (tasarım notu). Aynı fikir set içinde en fazla 2 kez kullanılır.
   - Fikir örnekleri:
     - zincir iki bombayı birbirine bağlıyor
     - çivi sayacı anahtar anla çakışıyor
     - bomba ortada, iki yönden kazılıyor
     - duvar kolay yönü kapatıyor ve oyuncu uzun yoldan dolanıyor
4. **Anahtar parça anı (M13) korunur.** Ama anların tahtadaki yeri ve oyundaki zamanlaması bölümden bölüme değişir.

### Eğri (`zorluk_egrisi_onerisi.md` temel alınır)

- Her bölüm Z12 tipi olmamalı. Zor bölümlerin hepsi 9 alırsa oyun yorar.
- Yeni engeller 3, 7, 12 ve 20. bölümlerde kolay bir tanıtımla gelir.
- Dinlenme bölümleri olur.
- 4–6 bölüm "zirve" olur: hedef puan 9, Z12/Z35 tipi.
- Orta bölümlerin hedef puanı 4–6.
- Her slot için ZeydBot hedef puanı ve kazanma oranı tabloda yer alır.

### Teslim

- 40 bölüm `?set=redesign` altında hazırlanır: bayraklar kapalı, canlı 40 değişmez. Zeyd onay kuyruğunda oynayıp onaylayacak.
- `docs/analiz/zeydbot/SET40.md`:
  - slot tablosu: hedef puan, ZeydBot tahmini, fikir, tahta boyutu
  - M16 benzerlik matrisi özeti
- OVERNIGHT.md başına en fazla 8 satır yaz. Zeyd'in oynaması gereken kısa liste: önce yalnız 5 zirve + 5 rastgele bölüm.

### Sonra (bu işte DEĞİL)

Zeyd onaylarsa: Build 5 (kök adres yeni sete geçer, 👍/👎 açılır) → playtest (~20 kişi). Bu ayrı bir PM onayıdır.

## Token tasarrufu (zorunlu)

- PRODUCT.md'yi baştan okuma; grep kullan. Karar kaydına 3–5 satır ekle.
- Büyük CSV'leri Python ile işle; sen yalnız özeti oku.
- Alt ajan en fazla 2 (tasarımcı + denetçi).
- Uzun koşular arka planda çalışsın; logdan yalnız son 20 satırı oku.
- Raporlar kısa olsun.
- 40 bölümü 5'erli gruplar halinde üret. Her grubun sonunda commit at, bir sonrakine geç. Kesilirse kaldığın gruptan devam et.
