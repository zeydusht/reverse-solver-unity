# PM → Claude Code (2 Kasım 2026, gece işi) — Yeni eğitim (tutorial) + ilk açılış ve performans incelemesi

**Sıra:** PM_MESAJ_27 bitmediyse önce onu bitir, sonra bu mesaja geç.

**Gece kuralları:**
- Bir komut 5 dakikadan uzun sürerse durdur, atla, OVERNIGHT'a not düş.
- Bir adımda kalırsan onu bırak, sıradakine geç. Sabah Zeyd her şeyi OVERNIGHT.md'de görecek.

**Yayın:** `p20/`.
- Oyun testi (pt1) artık **p20'den** yapılacak: yeni menüler + yeni eğitim. Davetler henüz gitmedi.
- `p19/` yedek olarak olduğu gibi kalsın.
- Kök adres, `redesign.json`, bölümler ve iOS dokunulmaz.
- Veri kaydı (cohort, device_id, 1–10 sorusu) aynen çalışmalı.

---

## A. Eğitim (tutorial) yeniden — Zeyd'in gözlemi

İlk kez oynayan kişi ne yapacağını anlamıyor:
- **İkinci el yanlış:** parça sola itilmeli, el yukarıyı gösteriyor. Oyuncular yukarı itiyor ve olmuyor.
- Kural anlatılmıyor, oyuncu oyunu kendisi çözmeye çalışıyor. **Biz anlatalım, o oynasın.**

### A1. Yön veriden gelsin (hata kökten çözülsün)
- Elin gösterdiği yön elle yazılmasın. Bölümün kayıtlı çözümündeki (`solution[i].dir`) yönden okunsun: L = sola, R = sağa, U = yukarı, D = aşağı.
- Aynı yön metne de yazılsın.
- Test: her eğitim adımında el yönü = beklenen yön = parçanın gerçekten çıkabildiği yön.

### A2. Yönlendirilmiş ilk iki hamle (Bölüm 1)
Her adımda:
- **Ekran kararır:** ~%60 koyu örtü. Yalnız hedef parça ve parçanın kenara kadar çıkış yolu (şerit) aydınlık kalır (spotlight).
- **El animasyonu:** El parçaya basar, çıkış yönünde tahta kenarına kadar sürükler, geri döner. Oyuncu yapana kadar tekrar eder.
- **Ok:** Elin arkasında parçadan kenara uzanan yarı saydam kalın ok. Yön tek bakışta anlaşılsın, ör. "sağdan sola".
- **Altta yazı** (iki dil, A3 yazı stili, büyük):
  - EN: **"Drag the piece to the left"** (right / up / down)
  - TR: **"Parçayı sola sürükle"** (sağa / yukarı / aşağı)
- **Yalnız hedef parça hareket eder.** Başka bir yere dokunulursa el animasyonu baştan oynar, hata sesi yok. Yanlış yöne çekilirse parça hafif sallanır ve el tekrar gösterir.
- **Doğru hamle yapılınca:** kısa başarı efekti, örtü kalkar, ikinci adım gelir.

İki adımdan sonra **kural kartı** (örtü hafif, ortada kart, dokununca kapanır):
- EN: **"Pull every piece out without hitting other pieces!"**
- TR: **"Tüm parçaları diğer parçalara çarpmadan dışarı çek!"**
- Küçük çizim: çıkış yolu açık parça yeşil tik, yolu kapalı parça kırmızı çarpı. Mevcut görsellerden yap, gerekirse basit şekiller.

Sonra serbest oyun. **Süre**, eğitim adımları bitene kadar başlamasın.

### A3. Yeni engel bölümlerinde tek adımlık anlatım
Mevcut tanıtım kartları kalır. Kart kapandıktan sonra bölümün başında **bir spotlight adımı** gelir:
- Engel aydınlık, altta tek cümle, dokununca geçer.
- Hamle zorlaması yok. Bu adım süre başlamadan olur.

| Bölüm (SET40 v4) | Engel | EN | TR |
|---|---|---|---|
| 3 | Çivi | "Nailed pieces unlock after a few moves." | "Çivili parçalar birkaç hamle sonra açılır." |
| 6 | Bomba | "Free the bomb before its fuse runs out!" | "Fitili bitmeden bombayı çıkar!" |
| 10 | Zincir | "Chained pieces leave together. Clear both paths." | "Zincirli parçalar birlikte çıkar. İkisinin de yolunu aç." |
| 14 | Duvar | "Pieces can't pass through red-white walls." | "Parçalar kırmızı-beyaz duvardan geçemez." |

Metinler bölüm verisinden değil, `docs/i18n` dosyalarından gelsin. Kuralın doğruluğunu oyun koduna göre kontrol et; yanlışsa düzelt ve OVERNIGHT'a yaz (örneğin çivi gerçekten "birkaç hamle sonra" mı açılıyor?).

### A4. Eğitim sistemi veriyle çalışsın
Adımlar küçük bir JSON'da dursun: `{ slot, adim, tur: "surukle"|"goster", parca, metinAnahtari }`. Yön A1'deki gibi çözümden. Böylece ileride yeni eğitim adımı kod yazmadan eklenir.

### A5. Veri
- Eğitim hamleleri normal hamle olarak kaydedilir.
- Eğitimdeki yanlış dokunuşlar `jam` sayılmasın.
- `plays` satırına eğitim gösterildi mi bilgisi eklemek gerekirse yeni sütun açma, **PM'e yaz** (şema PM'de).

---

## B. İlk açılış (FTUE) ve performans incelemesi

### B1. "İlk kez oynayan" turu
Temiz cihaz gibi başla: depolama temiz, `?c=ic`. Telefon boyutunda (390×844) dokunmatik simülasyonla oyna:
1. Yükleme ekranı
2. İsim girişi ve profil sorusu
3. Eğitim (A)
4. Bölüm 1–6
5. Bir **kayıp** (bomba ya da süre, bilerek)
6. Kayıp sonrası 1–10 sorusu
7. Menüye dön, 4 sekmeyi gez, Ayarlar, Bölümler'den bir bölümü tekrar oyna

Her adımın ekran görüntüsü → `docs/qa/2026-11-02-ftue/`.

Sonra `docs/FTUE_PERF_RAPOR.md` yaz:
- **Tıkanma listesi:** Yeni bir oyuncunun takılacağı, anlamayacağı ya da yanlış basacağı her yer.
  - Sütunlar: adım / ne oldu / neden sorun / önerilen çözüm / önem (yüksek-orta-düşük).
  - Yalnız tespit et, ufak metin hataları dışında **düzeltme yapma**. PM ve Zeyd karar verecek.
- **Okunabilirlik:** 375×667'de 12 pt altında kalan yazı var mı? Kontrast düşük olan var mı?
- **Dokunma alanı:** 44 pt altında kalan tuş var mı?

### B2. Performans
Chrome'da telefon emülasyonu, yalnız ölç ve raporla:

| Ölçüm | Koşul |
|---|---|
| Sayfa açılıştan menüye süre | Hızlı 4G / Yavaş 4G / 3G, CPU ×4 yavaşlatma |
| Menüden bölüm 1'in oynanabilir olmasına süre | aynı koşullar |
| Toplam indirme (MB), en büyük 10 dosya | — |
| FPS | bölüm 1, bölüm 30 (büyük tahta), koridor geçişi, koleksiyon kaydırma |
| Bellek | tepe |

Rapora öneri ekle: hangi dokular küçültülebilir ya da sıkıştırılabilir, beklenen kazanç kaç MB ve kaç sn.
- Görsel kaliteyi bozmadan **kesin kazanç** olan basit ayarları (ör. kullanılmayan büyük kaynak dokuların build'den çıkması) uygulayabilirsin. Ne yaptığını yaz.
- Diğerlerini öner, uygulama.

---

## Bitince
- OVERNIGHT.md başına:
  - p20 linki
  - A'nın durumu (her adım çalışıyor mu)
  - FTUE tıkanma listesinin en önemli 5 maddesi
  - Yükleme süreleri (3 ağ hızı)
- Takıldığın ya da atladığın adımlar.
