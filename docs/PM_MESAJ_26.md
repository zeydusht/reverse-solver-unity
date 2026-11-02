# PM → Claude Code (1 Kasım 2026) — Menü ekranları entegrasyonu (Task20 parçaları) → `p20/`

## Nereye yayınlanacak
- **`p20/`'ye yayınla.** `p19/` oyun testi (pt1) için donduruldu: davet linki orası, oyun değişirse ölçüm bozulur.
- `p19/`'a, kök adrese, `redesign.json`'a ve iOS zip'ine dokunma.
- p20 aynı SET40 v4 ve aynı veri kaydıyla çalışsın.

## Girdiler
- Parçalar: `Assets/Art/Menu/Task20/`. Ölçü, 9-slice ve pivot bilgisi `manifest.json` ve `HANDOFF.md`'de.
- Tasarımın onaylı hali: `Assets/Art/Menu/Task19D/01…08.png`. Ekran böyle görünmeli.
- Parçalardan kurulmuş önizleme: `Task20/preview-kontak.png`. Codex'in açıkladığı küçük farklar HANDOFF'ta.
- Kurallar:
  - Yazılar oyun tarafından basılır: Fredoka, iki dil.
  - Ekonomi yok.
  - Menüde yıldız yok. Kazanma kartındaki yıldızlar şimdilik kalsın.
  - Güvenli alan: üst 44 pt, alt 34 pt.

## Ekranlar

### 1. Alt sekme çubuğu (her sekmede)
- 4 sekme: Ev / Koleksiyon / Jiggles / Başarılar.
- Seçili yuva kabarık. Etiket her zaman okunur.
- Basınca kısa bir ölçek tweeni (0,12 sn).
- Başarılar rozeti: oyuncunun henüz görmediği yeni başarı sayısı. Sekme açılınca sıfırlanır.
- Oyun ekranında çubuk görünmez, yalnız menüde.

### 2. Ev
- Güncel menü olduğu gibi kalır: profil kutusu, Jiggle sayacı, dişli, tablo, yeşil Level tuşu.
- Eklenenler:
  - Alttaki sekme çubuğu.
  - Level tuşunun solunda küçük **Bölümler** tuşu.
- Profil kutusunda uzun isim: kutu genişlesin ya da 14 karakterden sonra yazı küçülsün. Kesilmesin.

### 3. Koleksiyon + Tablo detayı
- 40 hücrelik ızgara: 3 sütun, dikey 3:4 hücre, aşağı kaydırılır.
- Hücre durumları:
  - **Kazanılmış:** tablonun küçük resmi (`Paintings/painting-XX`, ortadan dikey kırpılmış), altında "Bölüm N".
  - **Sıradaki:** parlak çerçeve, altında "Sıradaki".
  - **Kilitli:** lacivert hücre + kilit.
- Zor bölüm işareti: `hard: true` olan slotlar (19, 23, 25, 30, 35, 36, 38).
- Sayaç "X / 40". X = **kazanılmış** tablo sayısı.
- Kazanılmış tabloya dokununca detay kartı açılır:
  - Büyük tablo
  - Tablo adı (`Paintings/manifest-batch-*.json`)
  - "Bölüm N'de kazanıldı · 0:42" (bu bölümdeki en iyi kazanma süresi, kayıttan)
  - **Tekrar oyna** → o bölümü açar
  - Köşede X

### 4. Jiggles
- Büyük sayaç: toplam kurtarılan Jiggle. Mevcut sayaçla aynı sayı.
- **Renk kuralı (yeni):** her bölümün kurtardığı Jiggle'lar o bölümün rengine yazılır.
  - Renk = `(slot−1) mod 5`, sıra: mavi, pembe, kırmızı, yeşil, mor.
  - Rafta 5 sayaç bu kuralla hesaplanır. Eski kayıtlar için kazanılmış bölümlerden geriye dönük hesapla.
- **Avatarım:** 5 avatar. Mevcut avatar seçimini buraya bağla. Seçilen avatar profil kutusunda da değişir.
- **Kilometre taşları 500 / 1.000 / 2.000:** yalnız görsel hedef. Ulaşılınca tik, ödül mekaniği yok.

### 5. Başarılar
Liste ilerlemeye göre canlı hesaplanır. Sayaç "X / 9".

| Başarı | Koşul | İlerleme gösterimi |
|---|---|---|
| İlk Tablo | 1 bölüm kazan | 0/1 |
| Koleksiyoncu | 10 bölüm kazan | x/10 |
| Müze Sahibi | 40 bölüm kazan | x/40 |
| Bomba İmha Uzmanı | Bombalı 10 bölümü **ilk denemede** kazan | x/10 |
| Soğukkanlı | Bir bölümü kalan süre < 3 sn iken kazan | 0/1 |
| Hatasız | Bir bölümü 0 sıkışmayla (jam) kazan | 0/1 |
| Hızlı Parmaklar | 10 bölümü sürenin en az yarısı kalırken kazan | x/10 |
| Zincir Kıran | Kazanılan bölümlerdeki zincirli parçaların toplamı 25 | x/25 |
| Baron'a Kafa Tut | 7 zor bölümün hepsini kazan | x/7 |

- Satır durumları: tamamlandı (tik), devam ediyor (çubuk), kilitli (henüz ilerleme 0).
- Yeni tamamlanan başarı: sekme rozetine +1, açılınca sıfırla.
- Kazanma kartında ek bildirim **yok**. pt1 ölçümü kazanma akışında değişiklik istemiyor.

### 6. Profil kartı
Profil kutusuna dokununca açılır. 6 istatistik, hepsi yerel ilerlemeden:
- Bölüm (şu anki / 40)
- Kurtarılan Jiggle
- Toplam süre (dk)
- İlk denemede % (ilk denemede kazanılan / oynanan bölüm)
- Başarı x/9
- Ortalama kalan süre (kazanmalarda)

Kalem ikonu mevcut isim girişini açar. "Avatarı değiştir" Jiggles sekmesine gider.

### 7. Ayarlar
Mevcut işlevler aynı, görünüş yeni:
- Ses / Müzik / Titreşim: aç-kapa anahtarları.
- Dil: Türkçe | English.
- **Gizlilik** → `/privacy/` sayfası.
- **Bize yaz** → şimdilik gizlilik sayfasındaki iletişim adresine `mailto:` (adres orada yoksa tuşu gizle, OVERNIGHT'a yaz; Zeyd adres verecek).
- Altta sürüm yazısı.

### 8. Bölümler haritası
- Ev'deki Bölümler tuşundan açılır, geri oku ile kapanır.
- 40 düğümlü dikey yol. Yol `levels-path-repeat` parçasıyla tekrarlanır. Açılınca şu anki düğüme kayar.
- Düğüm boyutu manifestteki gibi: normal 54 pt, şu anki 66 pt. Önizlemede küçük kalmışlar, manifeste uy.
- Düğüm durumları:
  - **Biten:** turkuaz-altın, dokununca o bölümü yeniden oynatır.
  - **Şu anki:** altın, parlama ve avatar.
  - **Kilitli:** gri-lacivert + kilit, dokunulmaz.
  - **Zor:** kırmızı kenar + Baron.
- Her 10 bölümde bir küçük tablo işareti (numarasız).

## Veri kaydı (en önemli kısım)
- **Tekrar oynamalar** (Koleksiyon "Tekrar oyna" ya da Bölümler düğümü) normal deneme olarak kaydedilir.
  - O bölümün `attempt` sayacı kaldığı yerden artar (1'e dönmez). Böylece "ilk deneme" hep gerçekten ilk deneme kalır.
  - Yeniden oynamada 1–10 sorusu tekrar sorulmaz (bölüm başına bir kez kuralı).
- Kohort, device_id, platform ve app_version (`web-p20-…`) her satırda dolu kalsın.
- İsteğe bağlı ama faydalı: sekme açılışlarını `moves`'a yazma. Yeni tür gerekir, **gerekirse PM'e söyle**, şemayı PM açar. Şimdilik yazma.

## Kontrol
1. `verify_build.js`: her sekme ve kart 375×667, 390×844 ve 430×932'de.
   - Ekran görüntüleri → `docs/qa/2026-11-01-p20/`.
   - Taşma ve kesik yazı kontrolü. İki dil, uzun isim ("Zeynep Yılmazoğlu").
2. İlerleme senaryoları:
   - 0 bölüm (yeni oyuncu)
   - 12 bölüm (tasarımdaki örnek)
   - 40 bölüm
   Koleksiyon sayacı, başarılar ve profil sayıları elle hesapla aynı mı?
3. Tekrar oyna → `attempt` 2 olarak kaydediliyor mu? (`?c=ic` ile canlıda, Supabase'de kontrol)
4. Yavaş ağda koleksiyon küçük resimleri: yer tutucu, sonra fade.
5. `OVERNIGHT.md` başına: p20 linki + sonuç + sorunlar.

Bu iş büyükse iki parçaya bölebilirsin:
- **A:** sekme çubuğu + Ev + Koleksiyon + Ayarlar + Bölümler
- **B:** Jiggles + Başarılar + Profil

A'yı yayınla, B'yi sonra yap.
