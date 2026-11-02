# PM → Claude Code (20 Ekim 2026, 19:40) — PM_MESAJ.md'nin 6. adımının YERİNE GEÇER

Zeyd onayladı: yeni bölümler artık rastgele üretimle değil, **tasarımcı ajan + fabrika doğrulaması** ile yapılır. Karar kaydına ve PRODUCT.md'ye işle.

## Neden

- Rastgele üretim "zor"u buldu ama "keyifli zor"u bulamadı. Oturum 3'ün zorları Zeyd'e en çok 6 puan aldırdı ("12 gibisi gelmedi bile").
- Zeyd'e "işte bu" dedirten D08 elle, çözücüyle birlikte tasarlanmıştı.
- Hedef artık açık: **Z12 tipi, düşündüren zor.**

## Yöntem (döngü)

1. **Tasarımcı ajan** (`seviye-tasarimci`, Claude alt ajanı) önce Z12'yi ve "cezalandıran zor" Z13'ü inceler: yeniden oynatma verisi, Zeyd'in hamleleri, think_ms, touch. Bundan **yazılı bir tasarım tarifi** çıkarır: `docs/analiz/zeydbot/Z12_TARIFI.md`. Tarifin içeriği:
   - tek doğru hamle zinciri
   - tuzak sayısı
   - fitil payı
   - "aha" anının yeri
   - engellerin dağılımı (M11)
   - yön karışımı (M12)
2. Ajan bu tariften **niyetle** bir taslak bölüm kurar: önce çözüm zincirini tasarlar, tahtayı ona göre yerleştirir. Hamle sayısı ve kısıtları yazılı olarak açıklar.
3. **Fabrika doğrular:**
   - çözülebilir mi
   - supap 0 mı
   - kayıtlı çözüm kazanıyor mu
   - M1–M12
   - rastgele oyuncu kazanma oranı
4. **ZeydBot puanlar** (v1, dondurulmuş haliyle): hedef puan 8–9, ilk deneme kazanma %30–60. Düşünme süresi profili Z12'ye benzemeli.
5. Hedef tutmuyorsa ajan **neden tutmadığını yazar**, taslağı düzeltir ve 3. adıma döner. Bölüm başına en fazla 6 tur; tutmazsa en iyi aday + sapma notuyla devam eder.
6. Her bölüm için kısa bir **tasarım notu**: "Bu bölümün fikri ne, aha anı nerede". Zeyd oynadıktan sonra notla karşılaştırılacak.

## Teslim

- **Z31–Z35:** 5 Z12 tipi bölüm. `?set=zeydbot` altında yayınla. Bayraklar kapalı, canlı 40 ve kök adres değişmez. Testler yeşil olmalı.
- **Tahminler dondurulur** (Zeyd oynamadan önce commit): ZeydBot puanı, kazanma olasılığı ve kalan sn. Ayrıca **ajanın kendi tahmini**: "Zeyd buna kaç verir, Z12 gibi hisseder mi". İkisi de sınanacak.
- Tasarım notları `docs/analiz/zeydbot/Z31_Z35_tasarim.md` dosyasında olsun. Bölüm türü veya tahmin Zeyd'in göreceği yerde yazmasın; OVERNIGHT.md'de yalnızca linkler bulunsun.
- OVERNIGHT.md başına en fazla 8 satır: ne yapıldı, 5 link, "her bölümde 1–10 ver ve Z12 gibi mi söyle".

## Başarı ölçütü (önceden kayıtlı)

- Zeyd 5 bölümün en az 3'üne "Z12 gibi" derse yöntem kabul edilir ve 40'lık set bu yöntemle kurulur.
- 2 veya daha azına derse: tarif ve ajan notları gözden geçirilir. Fabrikaya bağlama yok.

## Sıra

**Önce bu (Z31–Z35).** PM_MESAJ.md'deki v2 model işleri (1–3) ve kayıptan sonra puan sorusu (5) bundan SONRA gelir. Kayıptan sonra puan sorusu Z31–Z35 build'ine girebiliyorsa gir, girmiyorsa bekletme.

## Kurallar

- Zeyd'e soru yok.
- Supabase şemasına dokunma; gerekirse SQL taslağı yaz, PM çalıştırır.
- Uydurma sayı yok; her sayının kaynağı belli olmalı.
