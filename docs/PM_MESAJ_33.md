# PM → Claude Code (2 Kasım 2026, 22:30) — Davet öncesi son tur: Zeyd'in telefondaki notları (p20)

**İlk iş:** Bu dosyayı ve `docs/PM_MESAJ_32.md`'yi commit et. PM dosyalarını asla geri alma.

**Sıra:**
1. PM_MESAJ_32 (10 ve 14 sadeleştirme) **henüz yapılmadı.** `redesign.json` değişmemiş, `docs/veri/intro_v2*.csv` yok. Önce onu yap.
2. Sonra bu dosyadaki maddeler.

Yayın `p20/`. Davetler bu işten sonra gidecek.

## 0. Test kolaylığı
Zeyd kilitli bölümleri göremediği için 10 ve 14'ü deneyemiyor.
- **Yalnız `?c=ic` ya da `?debug=1` varken** çalışan bir `&unlock=1` parametresi ekle. Tüm bölümler açık olsun, her düğüm ve tablo oynanabilsin.
- pt1 kohortunda bu parametre **çalışmasın**.
- Linki OVERNIGHT'a yaz.

## 1. Bölüm 1 eğitimi: yeni akış (Zeyd)
Karartma ve yazı güzel oldu. Akış şöyle değişecek:

1. **Adım 1 (doğru hamle):** Parça sola. Mevcut gibi, yazı "Parçayı sola sürükle".
2. **Adım 2 (bilerek hata):**
   - Kayıtlı çözümün 2. parçası için **takılan** bir yön seç. Zeyd'in örneği: sağ alttaki parçayı sağa çekmek.
   - El o yönü gösterir. Yazı TR "Bu parçayı sağa çek" / EN "Drag this piece to the right" (yön koddan).
   - Oyuncu çeker → parça takılır, mevcut **kırmızı sıkışma efekti** oynar.
   - Ardından örtü kalkmadan altta uyarı yazısı çıkar:
     - TR: **"Takıldı! Parçaları başka parçalara çarpmadan dışarı çekmelisin."**
     - EN: **"Stuck! Pull pieces out without hitting other pieces."**
   - ~2 sn sonra (ya da dokununca) adım 3'e geçilir.
   - Bu sıkışma veri olarak `jam` **sayılmaz**, eğitim hamlesidir.
3. **Adım 3 (doğrusu):** Aynı parça doğru yöne (çözümdeki yön, ör. aşağı). Yazı "Parçayı aşağı sürükle".
4. **Kural kartı ekranını tamamen kaldır** (Zeyd: "çok kötü duruyor"). Kural cümlesi zaten adım 2'nin alt yazısında.

Takılan yön yoksa (parça her yöne çıkabiliyorsa), hatalı adım için çözümdeki **başka bir parçayı** seç. O da yoksa adım 2'yi atla ve OVERNIGHT'a yaz.

## 2. Eğitim eli (Zeyd: "yatay duruyor, gerçek el gibi değil")
- Mobil oyunlardaki **standart eğitim eli**: elin **dışı (sırtı)** görünür, avuç içi görünmez. İşaret parmağı **yukarı** bakar, el dik durur.
- El **hiç dönmez.** Her yönde dik kalır, yalnız konumu kayar: önce parmak ucu parçaya basar (hafif küçülme), sonra çıkış yönüne kayar, sonra kalkar.
- Parmak ucu dokunulan noktadır. Elin geri kalanı aşağıda durur ve parçayı örtmez.
- **Görsel:** Mevcut el bu tarife uymuyorsa **Codex'e yeni el çizdir** (`Assets/Art/Tutorial/hand-v2.png`, normal + basılı).
  - Krem-beyaz eldiven ya da açık ten.
  - Koyu kalın kontur, hafif gölge.
  - `docs/art/STIL_REHBERI_RM.md` ile aynı dil.
  - Booster eğitimlerindeki el de aynı olsun.

## 3. Bölümler haritası
- 10, 20, 30, 40'taki **mavi tablo işaretlerini kaldır** (Zeyd ne olduğunu anlamadı, kötü duruyor).

## 4. Koleksiyon
- **Şu anki (sıradaki) bölümün tablosu görünsün.** Kilit ya da karanlık silüet yerine net görünsün. Üstünde küçük "Sıradaki" şeridi olsun, dokununca o bölümü başlatsın.
- Diğer kilitli hücreler kilitli kalır.

## 5. Jiggles sekmesi: kilometre taşları
- Mevcut 3 madalyon + noktalı yol sığmıyor, Zeyd "çok kötü" dedi. Kaldır.
- Yerine **tek bir kalın ilerleme çubuğu** koy:
  - Task13 `progress-track` / `progress-fill` stilinde, ekran genişliğinin ~%85'i.
  - Jiggle kurtardıkça dolar. Üstünde "1.248 / 2.000" (A3 yazı stili).
  - Çubuğun üstünde 500 ve 1.000 noktalarında küçük işaret çentikleri olsun. Geçilenler parlak.
  - Çubuğun sonunda küçük bir ödül ikonu (madalyon) olsun.
  - 2.000 geçilince hedef 5.000 olur.
- Hiçbir yazı taşmasın, 375×667'de de sığsın.

## Kontrol
- `unlock=1` ile bölüm 1, 10 ve 14'ün eğitimleri.
- Bölüm 1 eğitiminin 3 adımı, ekran görüntüsü ve kısa GIF ya da kare dizisi → `docs/qa/2026-11-02-son/`.
- Harita, koleksiyon (sıradaki tablo görünüyor mu), Jiggles çubuğu: 3 ekran boyunda.
- Canlı `?c=ic`: eğitimin hatalı adımı `jam` olarak gitmiyor mu? (Supabase `plays.jams`)
- OVERNIGHT.md başına:
  - link + unlock linki
  - PM_32 özeti (iki bölümün yeni değerleri, CSV yerleri)
  - bu turun sonucu
